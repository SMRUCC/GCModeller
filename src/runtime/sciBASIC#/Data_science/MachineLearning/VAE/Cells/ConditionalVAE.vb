' ============================================================================
' ConditionalVAE.vb - 条件变分自编码器（虚拟细胞 / 虚拟扰动）
'
' 模型：
'   q(z | x, c)    编码器同时看到表达谱与实验条件
'   p(x | z, c)    解码器按条件生成表达谱
'   x | z, c ~ 计数似然（NB / ZINB / Poisson / Gaussian）
'
' 之所以需要「条件」：虚拟扰动实验的核心问题是反事实——
' 「同一个细胞，如果施加另一种扰动，表达谱会变成什么样」。
' 只有在解码器显式接受条件 c 时，才能把同一个 z 解码成不同的响应，
' 也就是 <see cref="PredictCounterfactual"/>。
'
' 与普通 GMVAE 的差别：
'   - 似然默认是负二项（单细胞计数是过度离散的整数），不是高斯；
'   - 输入使用 log1p(按文库大小归一化) 的表达谱，解码侧再把文库大小乘回去；
'   - 条件以 one-hot 同时进入编码器与解码器。
' ============================================================================

Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.Core
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.Core.Layers
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.Core.Numerics
Imports std = System.Math

Namespace Cells

    ''' <summary>
    ''' 条件变分自编码器：按实验条件生成表达谱，并支持反事实预测。
    ''' </summary>
    Public Class ConditionalVAE
        Implements IModelPersistable

        ''' <summary>模型文件的数据结构版本。</summary>
        Public Const ModelFileSchemaVersion As Integer = 1

        Private _geneCount As Integer
        Private _conditionCount As Integer
        Private _latentDim As Integer
        Private _hiddenDim As Integer
        Private _seed As Integer?

        Private _encoder As Dense
        Private _encoderMean As Dense
        Private _encoderLogVar As Dense
        Private _decoderHidden As Dense
        Private _decoderOutput As Dense

        Private _rng As RandomGaussian
        Private _fitted As Boolean = False
        Private _loss As New List(Of Double)
        Private _meanLibrarySize As Double = 10000

        ''' <summary>输出似然。</summary>
        Public Property Likelihood As New CountLikelihood(CountLikelihoodKind.NegativeBinomial)

        ''' <summary>训练损失历史。</summary>
        Public ReadOnly Property Loss As List(Of Double)
            Get
                Return _loss
            End Get
        End Property

        Public ReadOnly Property GeneCount As Integer
            Get
                Return _geneCount
            End Get
        End Property

        Public ReadOnly Property ConditionCount As Integer
            Get
                Return _conditionCount
            End Get
        End Property

        Public ReadOnly Property LatentDim As Integer
            Get
                Return _latentDim
            End Get
        End Property

        Public ReadOnly Property IsFitted As Boolean
            Get
                Return _fitted
            End Get
        End Property

        Public Sub New(geneCount As Integer, conditionCount As Integer,
                       Optional latentDim As Integer = 16,
                       Optional hiddenDim As Integer = 64,
                       Optional seed As Integer? = Nothing)
            If geneCount <= 0 Then
                Throw New ArgumentException("基因数量必须大于 0", NameOf(geneCount))
            End If

            If conditionCount <= 0 Then
                Throw New ArgumentException("条件数量必须大于 0", NameOf(conditionCount))
            End If

            _geneCount = geneCount
            _conditionCount = conditionCount
            _latentDim = latentDim
            _hiddenDim = hiddenDim
            _seed = seed

            Call InitializeNetwork()
        End Sub

        Private Sub InitializeNetwork()
            _rng = If(_seed.HasValue, New RandomGaussian(_seed.Value), New RandomGaussian())

            Dim baseSeed As Integer = If(_seed.HasValue, _seed.Value, Environment.TickCount)

            _encoder = New Dense(_geneCount + _conditionCount, _hiddenDim, useRelu:=True, seed:=baseSeed)
            _encoderMean = New Dense(_hiddenDim, _latentDim, seed:=baseSeed + 1)
            _encoderLogVar = New Dense(_hiddenDim, _latentDim, seed:=baseSeed + 2)
            _decoderHidden = New Dense(_latentDim + _conditionCount, _hiddenDim, useRelu:=True, seed:=baseSeed + 3)
            _decoderOutput = New Dense(_hiddenDim, _geneCount, seed:=baseSeed + 4)
        End Sub

#Region "张量辅助"

        Private Shared Function ConcatColumns(first As Tensor, second As Tensor) As Tensor
            Dim rows As Integer = first.Shape(0)

            If second.Shape(0) <> rows Then
                Throw New ArgumentException("拼接要求两个张量的行数一致")
            End If

            Dim columnsA As Integer = first.Shape(1)
            Dim columnsB As Integer = second.Shape(1)
            Dim result As New Tensor(rows, columnsA + columnsB)
            Dim output As Double() = result.Data
            Dim dataA As Double() = first.Data
            Dim dataB As Double() = second.Data

            For i As Integer = 0 To rows - 1
                Dim targetBase As Integer = i * (columnsA + columnsB)
                Dim sourceBase As Integer = i * columnsA

                For j As Integer = 0 To columnsA - 1
                    output(targetBase + j) = dataA(sourceBase + j)
                Next

                sourceBase = i * columnsB

                For j As Integer = 0 To columnsB - 1
                    output(targetBase + columnsA + j) = dataB(sourceBase + j)
                Next
            Next

            Return result
        End Function

        Private Shared Function TakeColumns(source As Tensor, offset As Integer, count As Integer) As Tensor
            Dim rows As Integer = source.Shape(0)
            Dim columns As Integer = source.Shape(1)
            Dim result As New Tensor(rows, count)
            Dim output As Double() = result.Data
            Dim data As Double() = source.Data

            For i As Integer = 0 To rows - 1
                For j As Integer = 0 To count - 1
                    output(i * count + j) = data(i * columns + offset + j)
                Next
            Next

            Return result
        End Function

        ''' <summary>把原始计数转换为编码器输入（log1p + 行文库大小归一化）。</summary>
        Private Shared Function BuildEncoderInput(counts As Tensor, oneHot As Tensor,
                                                  ByRef librarySizes As Double()) As Tensor
            Dim cells As Integer = counts.Shape(0)
            Dim genes As Integer = counts.Shape(1)
            Dim data As Double() = counts.Data
            Dim normalized As New Tensor(cells, genes)
            Dim output As Double() = normalized.Data

            librarySizes = New Double(cells - 1) {}

            For i As Integer = 0 To cells - 1
                Dim total As Double = 0
                Dim baseIndex As Integer = i * genes

                For g As Integer = 0 To genes - 1
                    total += data(baseIndex + g)
                Next

                If total <= 0 Then
                    total = 1.0
                End If

                librarySizes(i) = total
                Dim scale As Double = 10000.0 / total

                For g As Integer = 0 To genes - 1
                    output(baseIndex + g) = std.Log(1.0 + std.Max(data(baseIndex + g) * scale, 0))
                Next
            Next

            Return ConcatColumns(normalized, oneHot)
        End Function

        Private Shared Function ClampLogVariance(logVar As Tensor) As Tensor
            Dim result As New Tensor(logVar.Shape)
            Dim source As Double() = logVar.Data
            Dim output As Double() = result.Data

            For i As Integer = 0 To output.Length - 1
                output(i) = LogSumExpOps.ClipLogVariance(source(i))
            Next

            Return result
        End Function

        Private Shared Function DecodeMeans(logMeanFraction As Tensor, librarySizes As Double(),
                                            likelihood As CountLikelihood) As Double()
            Dim cells As Integer = logMeanFraction.Shape(0)
            Dim genes As Integer = logMeanFraction.Shape(1)
            Dim data As Double() = logMeanFraction.Data
            Dim result As Double() = New Double(cells * genes - 1) {}

            For i As Integer = 0 To cells - 1
                Dim logLibrary As Double = If(librarySizes Is Nothing, 0.0, std.Log(std.Max(librarySizes(i), 1.0E-9)))
                Dim baseIndex As Integer = i * genes

                For g As Integer = 0 To genes - 1
                    result(baseIndex + g) = likelihood.MeanFromLogMean(data(baseIndex + g) + logLibrary)
                Next
            Next

            Return result
        End Function

#End Region

#Region "训练"

        ''' <summary>训练条件变分自编码器。</summary>
        Public Function Fit(dataset As PerturbationDataset,
                            Optional epochs As Integer = 100,
                            Optional batchSize As Integer = 64,
                            Optional learningRate As Double = 0.001,
                            Optional beta As Double = 1.0,
                            Optional verbose As Boolean = True) As ConditionalVAE
            Dim options As New TrainingOptions With {
                .Epochs = epochs,
                .BatchSize = batchSize,
                .LearningRate = learningRate,
                .Beta = beta,
                .Verbose = verbose,
                .Seed = _seed
            }

            options.KlWarmupEpochs = std.Max(1, epochs \ 5)

            Return Fit(dataset, options)
        End Function

        ''' <summary>使用统一的训练选项训练。</summary>
        Public Function Fit(dataset As PerturbationDataset, options As TrainingOptions) As ConditionalVAE
            If dataset Is Nothing Then
                Throw New ArgumentNullException(NameOf(dataset))
            End If

            If dataset.GeneCount <> _geneCount Then
                Throw New ArgumentException($"基因数量不符: 模型为 {_geneCount}，数据为 {dataset.GeneCount}")
            End If

            If dataset.ConditionCount <> _conditionCount Then
                Throw New ArgumentException($"条件数量不符: 模型为 {_conditionCount}，数据为 {dataset.ConditionCount}")
            End If

            Dim cells As Integer = dataset.CellCount
            Dim oneHot As Tensor = dataset.OneHot(dataset.Conditions)
            Dim librarySizes As Double() = Nothing
            Dim inputs As Tensor = BuildEncoderInput(dataset.Counts, oneHot, librarySizes)
            Dim targets As Double() = dataset.Counts.Data
            Dim geneCount As Integer = _geneCount
            Dim conditionCount As Integer = _conditionCount

            _meanLibrarySize = dataset.MeanLibrarySize()
            _loss.Clear()

            Dim observer As ITrainingObserver = options.ResolveObserver(
                New ConsoleTrainingObserver(
                    Function(epoch, total, metrics)
                        Return $"  Epoch {epoch,4}/{total}: loss = {metrics("loss"):F6}"
                    End Function) With {.PrintInterval = 10})

            Dim shuffle As New RandomGaussian(If(_seed.HasValue, _seed.Value + 6151, Environment.TickCount))
            Dim indices As Integer() = New Integer(cells - 1) {}

            For i As Integer = 0 To cells - 1
                indices(i) = i
            Next

            Dim batchSize As Integer = If(options.BatchSize <= 0, cells, options.BatchSize)
            Dim latent As Integer = _latentDim
            Dim denominator As Double = 1.0

            For epoch As Integer = 1 To std.Max(1, options.Epochs)
                Dim beta As Double = options.BetaAt(epoch)
                Dim epochLoss As Double = 0
                Dim batches As Integer = 0

                Call observer.OnEpochBegin(epoch, options.Epochs)

                For i As Integer = cells - 1 To 1 Step -1
                    Dim j As Integer = shuffle.NextIndex(i + 1)
                    Dim temp As Integer = indices(i)
                    indices(i) = indices(j)
                    indices(j) = temp
                Next

                For batchStart As Integer = 0 To cells - 1 Step batchSize
                    Dim count As Integer = std.Min(batchSize, cells - batchStart)
                    Dim batchInput As New Tensor(count, _geneCount + _conditionCount)
                    Dim batchCondition As New Tensor(count, conditionCount)
                    Dim batchLibrary As Double() = New Double(count - 1) {}
                    Dim batchTarget As Double() = New Double(count * geneCount - 1) {}

                    For i As Integer = 0 To count - 1
                        Dim row As Integer = indices(batchStart + i)
                        Array.Copy(inputs.Data, row * (_geneCount + _conditionCount), batchInput.Data,
                                   i * (_geneCount + _conditionCount), _geneCount + _conditionCount)
                        Array.Copy(oneHot.Data, row * conditionCount, batchCondition.Data, i * conditionCount, conditionCount)
                        Array.Copy(targets, row * geneCount, batchTarget, i * geneCount, geneCount)
                        batchLibrary(i) = librarySizes(row)
                    Next

                    denominator = std.Max(1, count * geneCount)

                    ' ---- 前向 ----
                    Dim hidden As Tensor = _encoder.Forward(batchInput)
                    Dim meanValues As Tensor = _encoderMean.Forward(hidden)
                    Dim logVariances As Tensor = ClampLogVariance(_encoderLogVar.Forward(hidden))
                    Dim epsilon As New Tensor(count, latent)
                    Call _rng.NextNormals(epsilon.Data, 0, epsilon.Data.Length)

                    Dim sample As Tensor = Reparameterize(meanValues, logVariances, epsilon)
                    Dim decoderInput As Tensor = ConcatColumns(sample, batchCondition)
                    Dim decoderHidden As Tensor = _decoderHidden.Forward(decoderInput)
                    Dim logMeanFraction As Tensor = _decoderOutput.Forward(decoderHidden)
                    Dim means As Double() = DecodeMeans(logMeanFraction, batchLibrary, Likelihood)

                    ' ---- 损失 ----
                    Dim gradient As Double() = New Double(count * geneCount - 1) {}
                    Dim reconstruction As Double = 0

                    For e As Integer = 0 To gradient.Length - 1
                        Dim observed As Double = batchTarget(e)
                        Dim meanValue As Double = means(e)

                        reconstruction += -Likelihood.LogLikelihood(observed, meanValue)
                        gradient(e) = Likelihood.GradientLogMean(observed, meanValue) / denominator
                    Next

                    Dim klDivergence As Double = 0
                    Dim muData As Double() = meanValues.Data
                    Dim lvData As Double() = logVariances.Data
                    Dim dMean As Double() = New Double(count * latent - 1) {}
                    Dim dLogVar As Double() = New Double(count * latent - 1) {}
                    Dim sampleGradient As Double() = New Double(count * latent - 1) {}

                    ' KL(N(μ,σ²) || N(0,I))
                    For e As Integer = 0 To muData.Length - 1
                        Dim variance As Double = std.Exp(lvData(e))
                        klDivergence += 0.5 * (muData(e) * muData(e) + variance - 1.0 - lvData(e))
                        dMean(e) = beta * muData(e) / latent
                        dLogVar(e) = beta * 0.5 * (variance - 1.0) / latent
                    Next

                    ' ---- 反向 ----
                    Dim dLogMean As New Tensor(gradient, count, geneCount)
                    Dim dDecoderHidden As Tensor = _decoderOutput.Backward(dLogMean)
                    Dim dDecoderInput As Tensor = _decoderHidden.Backward(dDecoderHidden)
                    Array.Copy(dDecoderInput.Data, sampleGradient, sampleGradient.Length)

                    ' 重参数化：∂L/∂μ = ∂L/∂z；∂L/∂logσ² = ∂L/∂z · 0.5 · ε
                    For e As Integer = 0 To sampleGradient.Length - 1
                        dMean(e) += sampleGradient(e)
                        dLogVar(e) += sampleGradient(e) * 0.5 * epsilon.Data(e)
                    Next

                    Dim dHidden As Tensor = _encoderMean.Backward(New Tensor(dMean, count, latent)) +
                                            _encoderLogVar.Backward(New Tensor(dLogVar, count, latent))
                    Call _encoder.Backward(dHidden)

                    ' ---- 更新 ----
                    Dim learningRate As Double = options.LearningRate
                    Call _encoder.Update(learningRate)
                    Call _encoderMean.Update(learningRate)
                    Call _encoderLogVar.Update(learningRate)
                    Call _decoderHidden.Update(learningRate)
                    Call _decoderOutput.Update(learningRate)

                    If Not Double.IsNaN(reconstruction) AndAlso Not Double.IsInfinity(reconstruction) Then
                        epochLoss += (reconstruction / denominator + beta * klDivergence / (count * latent))
                    End If

                    batches += 1
                Next

                If batches = 0 Then
                    batches = 1
                End If

                epochLoss /= batches
                _loss.Add(epochLoss)

                Dim metrics As New Dictionary(Of String, Double) From {{"loss", epochLoss}}
                Call observer.OnEpochEnd(epoch, metrics, 0)
            Next

            _fitted = True

            Return Me
        End Function

        Private Shared Function Reparameterize(meanValues As Tensor, logVariances As Tensor, epsilon As Tensor) As Tensor
            Dim data As Double() = New Double(meanValues.Length - 1) {}
            Dim mu As Double() = meanValues.Data
            Dim lv As Double() = logVariances.Data
            Dim noise As Double() = epsilon.Data

            For i As Integer = 0 To data.Length - 1
                data(i) = mu(i) + std.Exp(0.5 * lv(i)) * noise(i)
            Next

            Return New Tensor(data, meanValues.Shape)
        End Function

#End Region

#Region "推理 / 反事实"

        ''' <summary>编码：返回类别无关的潜变量均值与对数方差。</summary>
        Public Sub Encode(counts As Tensor, oneHot As Tensor, ByRef meanValues As Tensor, ByRef logVariances As Tensor)
            Call EnsureFitted()

            If counts Is Nothing OrElse counts.Rank <> 2 OrElse counts.Shape(1) <> _geneCount Then
                Throw New ArgumentException($"计数矩阵必须是 [细胞数, {_geneCount}]", NameOf(counts))
            End If

            Dim librarySizes As Double() = Nothing
            Dim input As Tensor = BuildEncoderInput(counts, oneHot, librarySizes)
            Dim hidden As Tensor = _encoder.Forward(input)

            meanValues = _encoderMean.Forward(hidden)
            logVariances = ClampLogVariance(_encoderLogVar.Forward(hidden))
        End Sub

        ''' <summary>按给定条件重构表达谱（返回均值）。</summary>
        Public Function Reconstruct(counts As Tensor, oneHot As Tensor) As Tensor
            Dim meanValues As Tensor = Nothing
            Dim logVariances As Tensor = Nothing

            Call Encode(counts, oneHot, meanValues, logVariances)

            Return DecodeCore(meanValues, oneHot, Nothing)
        End Function

        ''' <summary>按条件生成新的表达谱。</summary>
        Public Function Generate(count As Integer, condition As Integer, Optional seed As Integer? = Nothing) As Tensor
            Call EnsureFitted()

            If condition < 0 OrElse condition >= _conditionCount Then
                Throw New ArgumentOutOfRangeException(NameOf(condition))
            End If

            Dim rng As RandomGaussian = If(seed.HasValue, New RandomGaussian(seed.Value), _rng)
            Dim latent As New Tensor(count, _latentDim)
            Call rng.NextNormals(latent.Data, 0, latent.Data.Length)

            Dim oneHot As New Tensor(count, _conditionCount)

            For i As Integer = 0 To count - 1
                oneHot.Data(i * _conditionCount + condition) = 1.0
            Next

            Return DecodeCore(latent, oneHot, Nothing)
        End Function

        ''' <summary>
        ''' 反事实预测：把同一个细胞的状态固定下来，只替换条件，得到两种条件下的表达谱。
        ''' </summary>
        ''' <returns>(在原条件下的重构, 在目标条件下的预测)</returns>
        Public Function PredictCounterfactual(counts As Tensor, conditionFrom As Integer,
                                              conditionTo As Integer) As Tuple(Of Tensor, Tensor)
            Call EnsureFitted()

            Dim cells As Integer = counts.Shape(0)

            Dim fromHot As New Tensor(cells, _conditionCount)
            Dim toHot As New Tensor(cells, _conditionCount)

            For i As Integer = 0 To cells - 1
                fromHot.Data(i * _conditionCount + conditionFrom) = 1.0
                toHot.Data(i * _conditionCount + conditionTo) = 1.0
            Next

            Dim meanValues As Tensor = Nothing
            Dim logVariances As Tensor = Nothing

            Call Encode(counts, fromHot, meanValues, logVariances)

            ' 同一个 z，只换条件
            Return Tuple.Create(DecodeCore(meanValues, fromHot, Nothing),
                                DecodeCore(meanValues, toHot, Nothing))
        End Function

        ''' <summary>
        ''' 反事实的对数倍数变化（log2 fold change），逐基因返回。
        ''' </summary>
        Public Function LogFoldChange(counts As Tensor, conditionFrom As Integer, conditionTo As Integer) As Double()
            Dim result As Tuple(Of Tensor, Tensor) = PredictCounterfactual(counts, conditionFrom, conditionTo)
            Dim before As Double() = result.Item1.Data
            Dim after As Double() = result.Item2.Data
            Dim changes As Double() = New Double(_geneCount - 1) {}
            Dim cells As Integer = counts.Shape(0)

            For g As Integer = 0 To _geneCount - 1
                Dim acc As Double = 0

                For i As Integer = 0 To cells - 1
                    Dim index As Integer = i * _geneCount + g
                    acc += std.Log(1.0 + after(index)) - std.Log(1.0 + before(index))
                Next

                changes(g) = acc / cells / std.Log(2.0)
            Next

            Return changes
        End Function

        Private Function DecodeCore(latent As Tensor, oneHot As Tensor, librarySizes As Double()) As Tensor
            Dim cells As Integer = latent.Shape(0)
            Dim decoderInput As Tensor = ConcatColumns(latent, oneHot)
            Dim hidden As Tensor = _decoderHidden.Forward(decoderInput)
            Dim logMeanFraction As Tensor = _decoderOutput.Forward(hidden)

            Dim sizes As Double() = librarySizes

            If sizes Is Nothing Then
                sizes = New Double(cells - 1) {}

                For i As Integer = 0 To cells - 1
                    sizes(i) = _meanLibrarySize
                Next
            End If

            Dim means As Double() = DecodeMeans(logMeanFraction, sizes, Likelihood)
            Dim result As New Tensor(cells, _geneCount)

            Array.Copy(means, result.Data, means.Length)

            Return result
        End Function

        Private Sub EnsureFitted()
            If Not _fitted Then
                Throw New InvalidOperationException("模型尚未训练, 请先调用 Fit 方法")
            End If
        End Sub

#End Region

#Region "持久化"

        Private ReadOnly Property SchemaVersionValue As Integer Implements IModelPersistable.SchemaVersion
            Get
                Return ModelFileSchemaVersion
            End Get
        End Property

        Public Sub Save(path As String) Implements IModelPersistable.Save
            Dim parameters As New ModelParameters("ConditionalVAE", ModelFileSchemaVersion)

            Call parameters.Add("enc_W", _encoder.Weights)
            Call parameters.Add("enc_b", _encoder.Bias)
            Call parameters.Add("encMu_W", _encoderMean.Weights)
            Call parameters.Add("encMu_b", _encoderMean.Bias)
            Call parameters.Add("encLv_W", _encoderLogVar.Weights)
            Call parameters.Add("encLv_b", _encoderLogVar.Bias)
            Call parameters.Add("decH_W", _decoderHidden.Weights)
            Call parameters.Add("decH_b", _decoderHidden.Bias)
            Call parameters.Add("decO_W", _decoderOutput.Weights)
            Call parameters.Add("decO_b", _decoderOutput.Bias)

            Call parameters.AddValue("geneCount", _geneCount)
            Call parameters.AddValue("conditionCount", _conditionCount)
            Call parameters.AddValue("latentDim", _latentDim)
            Call parameters.AddValue("hiddenDim", _hiddenDim)
            Call parameters.AddValue("meanLibrarySize", _meanLibrarySize)
            Call parameters.AddValue("likelihood", CInt(Likelihood.Kind))
            Call parameters.AddValue("dispersion", Likelihood.Dispersion)
            Call parameters.AddValue("zeroInflation", Likelihood.ZeroInflation)

            Call parameters.Save(path)
        End Sub

        Public Function TryLoad(path As String, ByRef errorMessage As String) As Boolean Implements IModelPersistable.TryLoad
            errorMessage = Nothing

            Try
                Dim parameters As ModelParameters = ModelParameters.Load(path)

                If Not String.Equals(parameters.ModelType, "ConditionalVAE", StringComparison.Ordinal) Then
                    errorMessage = $"模型类型不符: 期望 ConditionalVAE，实际 {parameters.ModelType}"
                    Return False
                End If

                _geneCount = CInt(parameters.GetValue("geneCount", 0))
                _conditionCount = CInt(parameters.GetValue("conditionCount", 0))
                _latentDim = CInt(parameters.GetValue("latentDim", 0))
                _hiddenDim = CInt(parameters.GetValue("hiddenDim", 0))
                _meanLibrarySize = parameters.GetValue("meanLibrarySize", 10000)

                If _geneCount <= 0 OrElse _conditionCount <= 0 OrElse _latentDim <= 0 Then
                    errorMessage = "模型文件缺少有效的维度信息"
                    Return False
                End If

                Call InitializeNetwork()

                Call LoadInto(_encoder.Weights, parameters, "enc_W")
                Call LoadInto(_encoder.Bias, parameters, "enc_b")
                Call LoadInto(_encoderMean.Weights, parameters, "encMu_W")
                Call LoadInto(_encoderMean.Bias, parameters, "encMu_b")
                Call LoadInto(_encoderLogVar.Weights, parameters, "encLv_W")
                Call LoadInto(_encoderLogVar.Bias, parameters, "encLv_b")
                Call LoadInto(_decoderHidden.Weights, parameters, "decH_W")
                Call LoadInto(_decoderHidden.Bias, parameters, "decH_b")
                Call LoadInto(_decoderOutput.Weights, parameters, "decO_W")
                Call LoadInto(_decoderOutput.Bias, parameters, "decO_b")

                Likelihood = New CountLikelihood(CType(CInt(parameters.GetValue("likelihood", 2)), CountLikelihoodKind)) With {
                    .Dispersion = parameters.GetValue("dispersion", 1.0),
                    .ZeroInflation = parameters.GetValue("zeroInflation", 0.05)
                }

                _fitted = True

                Return True
            Catch ex As Exception
                errorMessage = ex.Message
                Return False
            End Try
        End Function

        Private Shared Sub LoadInto(target As Tensor, parameters As ModelParameters, name As String)
            Dim value As Tensor = parameters.RequireTensor(name, target.Shape)

            Array.Copy(value.Data, target.Data, target.Data.Length)
            target.MarkHostModified()
        End Sub

#End Region

        Public Overrides Function ToString() As String
            Return $"ConditionalVAE(genes={_geneCount}, conditions={_conditionCount}, latent={_latentDim})"
        End Function

    End Class

End Namespace
