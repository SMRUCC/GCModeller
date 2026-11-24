' ============================================================================
' ConvVAE.vb - 卷积变分自编码器（图像生成）
'
' 为什么需要单独的卷积实现：GMVAE 那种「全连接 + 线性输出」的结构只能重建
' 全局模糊轮廓，对图像这种强局部相关的数据几乎无法给出可用的生成结果。
' 这里用仓库已有的卷积内核搭一个真正的卷积 VAE：
'
'   编码器  Conv2D(stride 2) → ReLU → Conv2D(stride 2) → ReLU → 展平 → μ / logσ²
'   解码器  全连接 → 重塑 → 上采样×2 → Conv2D → ReLU → 上采样×2 → Conv2D → sigmoid
'
' 转置卷积不引入任何新内核：
'   最近邻上采样×2 等价于「核为 2×2 单位阵、步长 2」的转置卷积，而转置卷积正是
'   Conv2D 的伴随算子——仓库里已经有 <c>Conv2DBackwardInput</c>
'   （TensorComputeBase.vb L1314 / SIMDTensor.vb L594），直接复用即可。
'   因此上采样的反向传播就是同一个核上的 Conv2D 前向。
'
' 似然：伯努利 + 交叉熵（输出经 sigmoid 严格落在 [0,1]），
' 相比高斯 + MSE 能显著改善像素级重建质量。
' ============================================================================

Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow.Compute
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.Core
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.Core.Layers
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.Core.Numerics
Imports std = System.Math

Namespace Imaging

    ''' <summary>
    ''' 卷积变分自编码器：输入为 channel-last 的图像张量 [N, H, W, C]。
    ''' </summary>
    Public Class ConvVAE
        Implements IModelPersistable

        ''' <summary>模型文件的数据结构版本。</summary>
        Public Const ModelFileSchemaVersion As Integer = 1

        Private _height As Integer
        Private _width As Integer
        Private _channels As Integer
        Private _latentDim As Integer
        Private _filter1 As Integer
        Private _filter2 As Integer
        Private _seed As Integer?

        ' 编码器
        Private _encFilter1 As Tensor   ' (3,3,C,F1)
        Private _encBias1 As Tensor     ' (F1)
        Private _encFilter2 As Tensor   ' (3,3,F1,F2)
        Private _encBias2 As Tensor     ' (F2)

        Private _meanLayer As Dense
        Private _logVarLayer As Dense

        ' 解码器
        Private _expandLayer As Dense
        Private _midFilter As Tensor    ' (3,3,F2,F1)
        Private _midBias As Tensor
        Private _outFilter As Tensor    ' (3,3,F1,C)
        Private _outBias As Tensor

        ' 上采样核（固定，不参与训练）
        Private _upKernel2 As Tensor
        Private _upKernel1 As Tensor

        Private ReadOnly _optimizers As New Dictionary(Of String, Adam)
        Private _rng As RandomGaussian
        Private _loss As New List(Of Double)
        Private _fitted As Boolean = False

        Public Sub New(height As Integer, width As Integer,
                       Optional channels As Integer = 1,
                       Optional latentDim As Integer = 32,
                       Optional filter1 As Integer = 16,
                       Optional filter2 As Integer = 32,
                       Optional seed As Integer? = Nothing)
            If height <= 0 OrElse width <= 0 Then
                Throw New ArgumentException("图像尺寸必须为正")
            End If

            If height Mod 4 <> 0 OrElse width Mod 4 <> 0 Then
                Throw New ArgumentException($"图像尺寸必须是 4 的倍数（两级 stride-2 卷积），实际 {height}x{width}")
            End If

            _height = height
            _width = width
            _channels = channels
            _latentDim = latentDim
            _filter1 = filter1
            _filter2 = filter2
            _seed = seed

            Call InitializeNetwork()
        End Sub

        Public ReadOnly Property Height As Integer
            Get
                Return _height
            End Get
        End Property

        Public ReadOnly Property Width As Integer
            Get
                Return _width
            End Get
        End Property

        Public ReadOnly Property Channels As Integer
            Get
                Return _channels
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

        Public ReadOnly Property Loss As List(Of Double)
            Get
                Return _loss
            End Get
        End Property

#Region "初始化"

        Private Sub InitializeNetwork()
            _rng = If(_seed.HasValue, New RandomGaussian(_seed.Value), New RandomGaussian())

            Dim baseSeed As Integer = If(_seed.HasValue, _seed.Value, Environment.TickCount)
            Dim reducedHeight As Integer = _height \ 4
            Dim reducedWidth As Integer = _width \ 4
            Dim flattened As Integer = reducedHeight * reducedWidth * _filter2

            _encFilter1 = HeFilter(_channels, _filter1, baseSeed)
            _encBias1 = Tensor.Zeros(New Integer() {_filter1})
            _encFilter2 = HeFilter(_filter1, _filter2, baseSeed + 1)
            _encBias2 = Tensor.Zeros(New Integer() {_filter2})

            _meanLayer = New Dense(flattened, _latentDim, seed:=baseSeed + 2)
            _logVarLayer = New Dense(flattened, _latentDim, seed:=baseSeed + 3)
            _expandLayer = New Dense(_latentDim, flattened, seed:=baseSeed + 4)

            _midFilter = HeFilter(_filter2, _filter1, baseSeed + 5)
            _midBias = Tensor.Zeros(New Integer() {_filter1})
            _outFilter = HeFilter(_filter1, _channels, baseSeed + 6)
            _outBias = Tensor.Zeros(New Integer() {_channels})

            _upKernel2 = UpsampleKernel(_filter2)
            _upKernel1 = UpsampleKernel(_filter1)
        End Sub

        ''' <summary>He 初始化：(3,3,fanIn,fanOut) 的卷积核。</summary>
        Private Shared Function HeFilter(fanIn As Integer, fanOut As Integer, seed As Integer) As Tensor
            Dim rng As New Random(seed)
            Dim scale As Double = std.Sqrt(2.0 / (9.0 * fanIn))
            Dim result As New Tensor(New Integer() {3, 3, fanIn, fanOut})
            Dim buffer As Double() = result.Data

            For i As Integer = 0 To buffer.Length - 1
                buffer(i) = rng.NextDouble() * 2.0 * scale - scale
            Next

            Return result
        End Function

        ''' <summary>构造 2×2 的最近邻上采样核：每个通道映射到自身，权重全为 1。</summary>
        Private Shared Function UpsampleKernel(channels As Integer) As Tensor
            Dim result As New Tensor(New Integer() {2, 2, channels, channels})
            Dim buffer As Double() = result.Data
            Dim stride As Integer = channels * channels

            For kh As Integer = 0 To 1
                For kw As Integer = 0 To 1
                    For c As Integer = 0 To channels - 1
                        ' 索引 (kh, kw, cIn, cOut)
                        buffer(((kh * 2 + kw) * channels + c) * channels + c) = 1.0
                    Next
                Next
            Next

            Return result
        End Function

        Private Shared Function Kernel() As ITensorCompute
            Return Tensor.computeKernel
        End Function

#End Region

#Region "前向"

        Private Class Cache
            Public Input As Tensor
            Public Encoded1 As Tensor
            Public Encoded2 As Tensor
            Public Encoded2Activated As Tensor
            Public Mean As Tensor
            Public LogVar As Tensor
            Public Epsilon As Tensor
            Public Sample As Tensor
            Public Expanded As Tensor
            Public Up1 As Tensor
            Public Mid As Tensor
            Public MidActivated As Tensor
            Public Up2 As Tensor
            Public Logits As Tensor
            Public Probabilities As Tensor
        End Class

        Private Function ForwardCore(images As Tensor, useSampling As Boolean) As Cache
            ValidateInput(images)

            Dim batch As Integer = images.Shape(0)
            Dim compute As ITensorCompute = Kernel()
            Dim cache As New Cache With {.Input = images}

            cache.Encoded1 = Relu(compute.Conv2D(images, _encFilter1, _encBias1, 2, 1))
            cache.Encoded2 = compute.Conv2D(cache.Encoded1, _encFilter2, _encBias2, 2, 1)
            cache.Encoded2Activated = Relu(Duplicate(cache.Encoded2))

            Dim flattened As Tensor = Reshape(cache.Encoded2Activated, batch, cache.Encoded2Activated.Length \ batch)

            cache.Mean = _meanLayer.Forward(flattened)
            cache.LogVar = ClampLogVariance(_logVarLayer.Forward(flattened))

            If useSampling Then
                cache.Epsilon = New Tensor(batch, _latentDim)
                Call _rng.NextNormals(cache.Epsilon.Data, 0, cache.Epsilon.Data.Length)
            Else
                cache.Epsilon = Tensor.Zeros(New Integer() {batch, _latentDim})
            End If

            cache.Sample = Reparameterize(cache.Mean, cache.LogVar, cache.Epsilon)

            ' ---- 解码 ----
            cache.Expanded = _expandLayer.Forward(cache.Sample)

            Dim reducedHeight As Integer = _height \ 4
            Dim reducedWidth As Integer = _width \ 4
            Dim restored As Tensor = Reshape(cache.Expanded, batch, reducedHeight, reducedWidth, _filter2)
            Dim activated As Tensor = Relu(Duplicate(restored))

            ' 上采样 ×2：核为 2×2 单位阵、步长 2 的转置卷积 = Conv2DBackwardInput
            cache.Up1 = compute.Conv2DBackwardInput(
                activated, _upKernel2,
                New Integer() {batch, reducedHeight * 2, reducedWidth * 2, _filter2}, 2, 0)

            cache.Mid = compute.Conv2D(cache.Up1, _midFilter, _midBias, 1, 1)
            cache.MidActivated = Relu(Duplicate(cache.Mid))

            cache.Up2 = compute.Conv2DBackwardInput(
                cache.MidActivated, _upKernel1,
                New Integer() {batch, reducedHeight * 4, reducedWidth * 4, _filter1}, 2, 0)

            cache.Logits = compute.Conv2D(cache.Up2, _outFilter, _outBias, 1, 1)
            cache.Probabilities = GraphUtils.ApplySigmoid(cache.Logits)

            Return cache
        End Function

        ''' <summary>深拷贝一个张量（Tensor 只提供返回 Object 的 Clone）。</summary>
        Private Shared Function Duplicate(t As Tensor) As Tensor
            Return New Tensor(CType(t.Data.Clone(), Double()), t.Shape)
        End Function

        Private Shared Function Relu(t As Tensor) As Tensor
            Dim buffer As Double() = t.Data

            For i As Integer = 0 To buffer.Length - 1
                If buffer(i) < 0 Then
                    buffer(i) = 0
                End If
            Next

            t.MarkHostModified()

            Return t
        End Function

        Private Shared Function MaskRelu(gradient As Tensor, activation As Tensor) As Tensor
            Dim result As New Tensor(gradient.Shape)
            Dim source As Double() = gradient.Data
            Dim mask As Double() = activation.Data
            Dim output As Double() = result.Data

            For i As Integer = 0 To output.Length - 1
                output(i) = If(mask(i) > 0, source(i), 0.0)
            Next

            Return result
        End Function

        Private Shared Function Reshape(source As Tensor, ParamArray shape As Integer()) As Tensor
            Return New Tensor(CType(source.Data.Clone(), Double()), shape)
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

#Region "训练"

        ''' <summary>训练卷积 VAE。</summary>
        Public Function Fit(images As Tensor,
                            Optional epochs As Integer = 50,
                            Optional batchSize As Integer = 16,
                            Optional learningRate As Double = 0.001,
                            Optional beta As Double = 1.0,
                            Optional verbose As Boolean = True) As ConvVAE
            Dim options As New TrainingOptions With {
                .Epochs = epochs,
                .BatchSize = batchSize,
                .LearningRate = learningRate,
                .Beta = beta,
                .Verbose = verbose,
                .Seed = _seed
            }

            options.KlWarmupEpochs = std.Max(1, epochs \ 5)

            Return Fit(images, options)
        End Function

        Public Function Fit(images As Tensor, options As TrainingOptions) As ConvVAE
            ValidateInput(images)

            Dim total As Integer = images.Shape(0)
            Dim batchSize As Integer = If(options.BatchSize <= 0, total, options.BatchSize)
            Dim observer As ITrainingObserver = options.ResolveObserver(
                New ConsoleTrainingObserver(
                    Function(epoch, epochs, metrics)
                        Return $"  Epoch {epoch,4}/{epochs}: loss = {metrics("loss"):F6}"
                    End Function) With {.PrintInterval = 10})

            _loss.Clear()

            Dim indices As Integer() = New Integer(total - 1) {}

            For i As Integer = 0 To total - 1
                indices(i) = i
            Next

            For epoch As Integer = 1 To std.Max(1, options.Epochs)
                Dim startTick As Integer = Environment.TickCount
                Dim beta As Double = options.BetaAt(epoch)
                Dim epochLoss As Double = 0
                Dim batches As Integer = 0

                Call observer.OnEpochBegin(epoch, options.Epochs)
                Call Shuffle(indices, _rng)

                For batchStart As Integer = 0 To total - 1 Step batchSize
                    Dim count As Integer = std.Min(batchSize, total - batchStart)
                    Dim batch As Tensor = SliceRows(images, indices, batchStart, count)
                    Dim cache As Cache = ForwardCore(batch, useSampling:=True)
                    Dim lossValue As Double = BackwardAndUpdate(cache, beta, options.LearningRate)

                    epochLoss += lossValue
                    batches += 1
                Next

                If batches = 0 Then
                    batches = 1
                End If

                epochLoss /= batches
                _loss.Add(epochLoss)

                Dim metrics As New Dictionary(Of String, Double) From {{"loss", epochLoss}}
                Call observer.OnEpochEnd(epoch, metrics, Environment.TickCount - startTick)
            Next

            _fitted = True

            Return Me
        End Function

        Private Function BackwardAndUpdate(cache As Cache, beta As Double, learningRate As Double) As Double
            Dim batch As Integer = cache.Input.Shape(0)
            Dim compute As ITensorCompute = Kernel()
            Dim elements As Integer = batch * _height * _width * _channels
            Dim globalScale As Double = 1.0 / elements

            ' ---- 损失（伯努利交叉熵） ----
            Dim target As Double() = cache.Input.Data
            Dim logits As Double() = cache.Logits.Data
            Dim reconstruction As Double = 0
            Dim dLogits As New Tensor(cache.Logits.Shape)
            Dim dLogitsBuffer As Double() = dLogits.Data

            For i As Integer = 0 To elements - 1
                Dim probability As Double = GraphUtils.Sigmoid(logits(i))
                Dim clamped As Double = std.Max(1.0E-12, std.Min(1.0 - 1.0E-12, probability))

                reconstruction += -(target(i) * std.Log(clamped) + (1.0 - target(i)) * std.Log(1.0 - clamped))
                ' ∂BCE(with logits)/∂logits = σ(logit) − y
                dLogitsBuffer(i) = (probability - target(i)) * globalScale
            Next

            reconstruction *= globalScale

            ' ---- KL ----
            Dim muData As Double() = cache.Mean.Data
            Dim lvData As Double() = cache.LogVar.Data
            Dim dMean As New Tensor(cache.Mean.Shape)
            Dim dLogVar As New Tensor(cache.LogVar.Shape)
            Dim klDivergence As Double = 0

            For i As Integer = 0 To muData.Length - 1
                Dim variance As Double = std.Exp(lvData(i))
                klDivergence += 0.5 * (muData(i) * muData(i) + variance - 1.0 - lvData(i)) / _latentDim
                dMean.Data(i) = beta * muData(i) / _latentDim
                dLogVar.Data(i) = beta * 0.5 * (variance - 1.0) / _latentDim
            Next

            klDivergence /= batch

            ' ---- 解码器反向 ----
            Dim dBiasOut As Tensor = compute.Conv2DBackwardBias(dLogits)
            Dim dFilterOut As Tensor = compute.Conv2DBackwardFilter(
                dLogits, cache.Up2, _outFilter.Shape, 1, 1)
            Dim dUp2 As Tensor = compute.Conv2DBackwardInput(
                dLogits, _outFilter, cache.Up2.Shape, 1, 1)

            ' 穿过上采样：上采样的反向就是同一核上的 Conv2D 前向
            Dim dMidActivated As Tensor = compute.Conv2D(dUp2, _upKernel1, Tensor.Zeros(New Integer() {_filter1}), 2, 0)
            Dim dMid As Tensor = MaskRelu(dMidActivated, cache.MidActivated)

            Dim dBiasMid As Tensor = compute.Conv2DBackwardBias(dMid)
            Dim dFilterMid As Tensor = compute.Conv2DBackwardFilter(
                dMid, cache.Up1, _midFilter.Shape, 1, 1)
            Dim dUp1 As Tensor = compute.Conv2DBackwardInput(
                dMid, _midFilter, cache.Up1.Shape, 1, 1)

            Dim dActivated As Tensor = compute.Conv2D(dUp1, _upKernel2, Tensor.Zeros(New Integer() {_filter2}), 2, 0)
            Dim dRestored As Tensor = MaskRelu(dActivated, Relu(Duplicate(Reshape(cache.Expanded, batch, _height \ 4, _width \ 4, _filter2))))

            Dim dExpanded As Tensor = Reshape(dRestored, batch, cache.Expanded.Shape(1))
            Dim dSample As Tensor = _expandLayer.Backward(dExpanded)

            ' ---- 重参数化 ----
            Dim sampleGradient As Double() = dSample.Data
            Dim epsilonData As Double() = cache.Epsilon.Data

            For i As Integer = 0 To sampleGradient.Length - 1
                dMean.Data(i) += sampleGradient(i)
                dLogVar.Data(i) += sampleGradient(i) * 0.5 * epsilonData(i)
            Next

            ' ---- 编码器反向 ----
            Dim dFlattened As Tensor = _meanLayer.Backward(dMean) + _logVarLayer.Backward(dLogVar)
            Dim dEncoded2Activated As Tensor = Reshape(dFlattened, cache.Encoded2Activated.Shape)
            Dim dEncoded2 As Tensor = MaskRelu(dEncoded2Activated, cache.Encoded2Activated)

            Dim dBiasEnc2 As Tensor = compute.Conv2DBackwardBias(dEncoded2)
            Dim dFilterEnc2 As Tensor = compute.Conv2DBackwardFilter(
                dEncoded2, cache.Encoded1, _encFilter2.Shape, 2, 1)
            Dim dEncoded1 As Tensor = compute.Conv2DBackwardInput(
                dEncoded2, _encFilter2, cache.Encoded1.Shape, 2, 1)
            Dim dEncoded1Masked As Tensor = MaskRelu(dEncoded1, cache.Encoded1)

            Dim dBiasEnc1 As Tensor = compute.Conv2DBackwardBias(dEncoded1Masked)
            Dim dFilterEnc1 As Tensor = compute.Conv2DBackwardFilter(
                dEncoded1Masked, cache.Input, _encFilter1.Shape, 2, 1)

            ' ---- 更新 ----
            Call ApplyGradient("encFilter1", _encFilter1, dFilterEnc1, learningRate)
            Call ApplyGradient("encBias1", _encBias1, dBiasEnc1, learningRate)
            Call ApplyGradient("encFilter2", _encFilter2, dFilterEnc2, learningRate)
            Call ApplyGradient("encBias2", _encBias2, dBiasEnc2, learningRate)
            Call ApplyGradient("midFilter", _midFilter, dFilterMid, learningRate)
            Call ApplyGradient("midBias", _midBias, dBiasMid, learningRate)
            Call ApplyGradient("outFilter", _outFilter, dFilterOut, learningRate)
            Call ApplyGradient("outBias", _outBias, dBiasOut, learningRate)

            Call _meanLayer.Update(learningRate)
            Call _logVarLayer.Update(learningRate)
            Call _expandLayer.Update(learningRate)

            Return reconstruction + beta * klDivergence
        End Function

        Private Sub ApplyGradient(name As String, parameter As Tensor, gradient As Tensor, learningRate As Double)
            If parameter Is Nothing OrElse gradient Is Nothing Then
                Return
            End If

            Dim state As Adam = Nothing

            If Not _optimizers.TryGetValue(name, state) Then
                state = New Adam(parameter.Shape, learningRate)
                _optimizers(name) = state
            End If

            state.Update(parameter, gradient, learningRate)
        End Sub

#End Region

#Region "推理与生成"

        ''' <summary>重构图像（使用后验均值，结果确定）。</summary>
        Public Function Reconstruct(images As Tensor) As Tensor
            Call EnsureFitted()

            Return ForwardCore(images, useSampling:=False).Probabilities
        End Function

        ''' <summary>从标准正态先验生成图像。</summary>
        Public Function Generate(count As Integer, Optional seed As Integer? = Nothing) As Tensor
            Call EnsureFitted()

            Dim rng As RandomGaussian = If(seed.HasValue, New RandomGaussian(seed.Value), _rng)
            Dim latent As New Tensor(count, _latentDim)
            Call rng.NextNormals(latent.Data, 0, latent.Data.Length)

            Dim batch As Integer = count
            Dim reducedHeight As Integer = _height \ 4
            Dim reducedWidth As Integer = _width \ 4
            Dim compute As ITensorCompute = Kernel()

            Dim expanded As Tensor = _expandLayer.Forward(latent)
            Dim restored As Tensor = Relu(Duplicate(Reshape(expanded, batch, reducedHeight, reducedWidth, _filter2)))

            Dim up1 As Tensor = compute.Conv2DBackwardInput(
                restored, _upKernel2,
                New Integer() {batch, reducedHeight * 2, reducedWidth * 2, _filter2}, 2, 0)
            Dim mid As Tensor = Relu(compute.Conv2D(up1, _midFilter, _midBias, 1, 1))
            Dim up2 As Tensor = compute.Conv2DBackwardInput(
                mid, _upKernel1,
                New Integer() {batch, reducedHeight * 4, reducedWidth * 4, _filter1}, 2, 0)
            Dim logits As Tensor = compute.Conv2D(up2, _outFilter, _outBias, 1, 1)

            Return GraphUtils.ApplySigmoid(logits)
        End Function

        ''' <summary>评估重构质量：返回 (每像素 BCE, 每像素 MSE, 潜变量 KL 均值)。</summary>
        Public Function Evaluate(images As Tensor) As Tuple(Of Double, Double, Double)
            ValidateInput(images)

            Dim cache As Cache = ForwardCore(images, useSampling:=False)
            Dim target As Double() = images.Data
            Dim predicted As Double() = cache.Probabilities.Data
            Dim elements As Integer = target.Length
            Dim bce As Double = 0
            Dim mse As Double = 0

            For i As Integer = 0 To elements - 1
                Dim probability As Double = std.Max(1.0E-12, std.Min(1.0 - 1.0E-12, predicted(i)))
                Dim diff As Double = target(i) - predicted(i)

                bce += -(target(i) * std.Log(probability) + (1.0 - target(i)) * std.Log(1.0 - probability))
                mse += diff * diff
            Next

            Dim muData As Double() = cache.Mean.Data
            Dim lvData As Double() = cache.LogVar.Data
            Dim klDivergence As Double = 0

            For i As Integer = 0 To muData.Length - 1
                Dim variance As Double = std.Exp(lvData(i))
                klDivergence += 0.5 * (muData(i) * muData(i) + variance - 1.0 - lvData(i))
            Next

            Return Tuple.Create(bce / elements, mse / elements, klDivergence / images.Shape(0))
        End Function

#End Region

#Region "辅助"

        Private Sub ValidateInput(images As Tensor)
            If images Is Nothing Then
                Throw New ArgumentNullException(NameOf(images))
            End If

            If images.Rank <> 4 Then
                Throw New ArgumentException($"需要 channel-last 的 4 维张量 [N, H, W, C]，实际秩为 {images.Rank}")
            End If

            If images.Shape(1) <> _height OrElse images.Shape(2) <> _width OrElse images.Shape(3) <> _channels Then
                Throw New ArgumentException(
                    $"图像尺寸不符：期望 [N, {_height}, {_width}, {_channels}]，实际 [{String.Join(", ", images.Shape)}]")
            End If
        End Sub

        Private Sub EnsureFitted()
            If Not _fitted Then
                Throw New InvalidOperationException("模型尚未训练, 请先调用 Fit 方法")
            End If
        End Sub

        Private Shared Sub Shuffle(indices As Integer(), rng As RandomGaussian)
            For i As Integer = indices.Length - 1 To 1 Step -1
                Dim j As Integer = rng.NextIndex(i + 1)
                Dim temp As Integer = indices(i)
                indices(i) = indices(j)
                indices(j) = temp
            Next
        End Sub

        Private Shared Function SliceRows(source As Tensor, indices As Integer(), start As Integer, count As Integer) As Tensor
            Dim rowSize As Integer = source.Length \ source.Shape(0)
            Dim result As New Tensor(New Integer() {count, source.Shape(1), source.Shape(2), source.Shape(3)})
            Dim buffer As Double() = result.Data

            For i As Integer = 0 To count - 1
                Array.Copy(source.Data, indices(start + i) * rowSize, buffer, i * rowSize, rowSize)
            Next

            Return result
        End Function

#End Region

#Region "持久化"

        Private ReadOnly Property SchemaVersionValue As Integer Implements IModelPersistable.SchemaVersion
            Get
                Return ModelFileSchemaVersion
            End Get
        End Property

        Public Sub Save(path As String) Implements IModelPersistable.Save
            Dim parameters As New ModelParameters("ConvVAE", ModelFileSchemaVersion)

            Call parameters.Add("encF1", _encFilter1)
            Call parameters.Add("encB1", _encBias1)
            Call parameters.Add("encF2", _encFilter2)
            Call parameters.Add("encB2", _encBias2)
            Call parameters.Add("midF", _midFilter)
            Call parameters.Add("midB", _midBias)
            Call parameters.Add("outF", _outFilter)
            Call parameters.Add("outB", _outBias)
            Call parameters.Add("mean_W", _meanLayer.Weights)
            Call parameters.Add("mean_b", _meanLayer.Bias)
            Call parameters.Add("lv_W", _logVarLayer.Weights)
            Call parameters.Add("lv_b", _logVarLayer.Bias)
            Call parameters.Add("expand_W", _expandLayer.Weights)
            Call parameters.Add("expand_b", _expandLayer.Bias)

            Call parameters.AddValue("height", _height)
            Call parameters.AddValue("width", _width)
            Call parameters.AddValue("channels", _channels)
            Call parameters.AddValue("latentDim", _latentDim)
            Call parameters.AddValue("filter1", _filter1)
            Call parameters.AddValue("filter2", _filter2)

            Call parameters.Save(path)
        End Sub

        Public Function TryLoad(path As String, ByRef errorMessage As String) As Boolean Implements IModelPersistable.TryLoad
            errorMessage = Nothing

            Try
                Dim parameters As ModelParameters = ModelParameters.Load(path)

                If Not String.Equals(parameters.ModelType, "ConvVAE", StringComparison.Ordinal) Then
                    errorMessage = $"模型类型不符: 期望 ConvVAE，实际 {parameters.ModelType}"
                    Return False
                End If

                _height = CInt(parameters.GetValue("height", 0))
                _width = CInt(parameters.GetValue("width", 0))
                _channels = CInt(parameters.GetValue("channels", 1))
                _latentDim = CInt(parameters.GetValue("latentDim", 0))
                _filter1 = CInt(parameters.GetValue("filter1", 16))
                _filter2 = CInt(parameters.GetValue("filter2", 32))

                If _height <= 0 OrElse _width <= 0 OrElse _latentDim <= 0 Then
                    errorMessage = "模型文件缺少有效的维度信息"
                    Return False
                End If

                Call InitializeNetwork()

                Call LoadInto(_encFilter1, parameters, "encF1")
                Call LoadInto(_encBias1, parameters, "encB1")
                Call LoadInto(_encFilter2, parameters, "encF2")
                Call LoadInto(_encBias2, parameters, "encB2")
                Call LoadInto(_midFilter, parameters, "midF")
                Call LoadInto(_midBias, parameters, "midB")
                Call LoadInto(_outFilter, parameters, "outF")
                Call LoadInto(_outBias, parameters, "outB")
                Call LoadInto(_meanLayer.Weights, parameters, "mean_W")
                Call LoadInto(_meanLayer.Bias, parameters, "mean_b")
                Call LoadInto(_logVarLayer.Weights, parameters, "lv_W")
                Call LoadInto(_logVarLayer.Bias, parameters, "lv_b")
                Call LoadInto(_expandLayer.Weights, parameters, "expand_W")
                Call LoadInto(_expandLayer.Bias, parameters, "expand_b")

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
            Return $"ConvVAE({_height}x{_width}x{_channels}, latent={_latentDim}, filters={_filter1}/{_filter2})"
        End Function

    End Class

End Namespace
