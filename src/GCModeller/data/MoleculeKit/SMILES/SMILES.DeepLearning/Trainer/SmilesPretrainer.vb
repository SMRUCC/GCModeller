Imports System.IO
Imports Microsoft.VisualBasic.MachineLearning.DataStorage.PT
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.MachineLearning.Transformer
Imports SMRUCC.genomics.Molecule.SMILES.DeepLearning.Corpus
Imports std = System.Math

Namespace Trainer

    ''' <summary>
    ''' 预训练超参数（默认值取自 smiles-transformer 的 <c>pretrain_trfm.py</c>）。
    ''' </summary>
    Public Class SmilesPretrainOptions

        ''' <summary>训练轮数，默认 5。</summary>
        Public Property Epochs As Integer = 5

        ''' <summary>批大小，默认 8。</summary>
        Public Property BatchSize As Integer = 8

        ''' <summary>Adam 学习率，默认 1e-4。</summary>
        Public Property LearningRate As Double = 0.0001

        ''' <summary>定长序列长度，默认 220。</summary>
        Public Property SequenceLength As Integer = 220

        ''' <summary>是否对每个样本做 SMILES 随机枚举（论文的语料增强手段）。</summary>
        Public Property EnumerateSmiles As Boolean = True

        ''' <summary>每个 epoch 是否重新洗牌。</summary>
        Public Property Shuffle As Boolean = True

        ''' <summary>随机种子；<c>Nothing</c> 表示不固定。</summary>
        Public Property Seed As Integer? = Nothing

        ''' <summary>每多少步做一次验证（&lt;=0 表示不做验证）。</summary>
        Public Property ValidateEverySteps As Integer = 1000

        ''' <summary>最优权重的落盘路径；<c>Nothing</c> 表示只留在内存。</summary>
        Public Property CheckpointPath As String = Nothing

        ''' <summary>被忽略的目标下标（<c>&lt;pad&gt;</c>）。</summary>
        Public Property IgnoreIndex As Integer = WordVocab.PadIndex

    End Class

    ''' <summary>
    ''' 一次训练步的观测值。
    ''' </summary>
    Public Class PretrainProgress

        ''' <summary>轮次（从 1 开始）。</summary>
        Public Property Epoch As Integer

        ''' <summary>全局迭代序号（从 1 开始）。</summary>
        Public Property Iteration As Integer

        ''' <summary>训练损失。</summary>
        Public Property Loss As Double

        ''' <summary>困惑度 <c>exp(loss)</c>。</summary>
        Public Property Perplexity As Double

        ''' <summary>验证损失；未做验证时为 <c>Nothing</c>。</summary>
        Public Property ValidationLoss As Double?

        Public Overrides Function ToString() As String
            Dim val As String = If(ValidationLoss.HasValue, $", val={ValidationLoss.Value:F3}", "")

            Return $"epoch {Epoch}, iter {Iteration}, loss {Loss:F3}, ppl {Perplexity:F3}{val}"
        End Function

    End Class

    ''' <summary>
    ''' SMILES Transformer 预训练脚手架：洗牌 → 前向 → NLL → 反向 → Adam，并跟踪最优验证损失。
    ''' </summary>
    Public Class SmilesPretrainer

        Private ReadOnly _model As SequenceTransformer
        Private ReadOnly _vocab As WordVocab
        Private ReadOnly _options As SmilesPretrainOptions
        Private ReadOnly _history As New List(Of PretrainProgress)
        Private ReadOnly _bestWeights As New Dictionary(Of String, Tensor)(StringComparer.Ordinal)

        Private _bestLoss As Double? = Nothing

        ''' <summary>日志回调（控制台、<c>DebugAudit</c> 或文件都行）。</summary>
        Public Property Logger As Action(Of String) = Nothing

        ''' <summary>最优验证损失；尚未验证过时为 <c>Nothing</c>。</summary>
        Public ReadOnly Property BestValidationLoss As Double?
            Get
                Return _bestLoss
            End Get
        End Property

        ''' <summary>训练历史。</summary>
        Public ReadOnly Property History As IReadOnlyList(Of PretrainProgress)
            Get
                Return _history
            End Get
        End Property

        ''' <summary>
        ''' 创建预训练器。
        ''' </summary>
        ''' <param name="model">待训练的模型（其 <c>SequenceLength</c> 必须不小于选项中的序列长度）。</param>
        ''' <param name="vocab">词表。</param>
        ''' <param name="options">超参数；<c>Nothing</c> 使用默认值。</param>
        Public Sub New(model As SequenceTransformer, vocab As WordVocab, Optional options As SmilesPretrainOptions = Nothing)
            If model Is Nothing Then Throw New ArgumentNullException(NameOf(model))
            If vocab Is Nothing Then Throw New ArgumentNullException(NameOf(vocab))

            _model = model
            _vocab = vocab
            _options = If(options, New SmilesPretrainOptions())
            _bestWeights = New Dictionary(Of String, Tensor)(StringComparer.Ordinal)
        End Sub

        ''' <summary>
        ''' 执行预训练。
        ''' </summary>
        ''' <param name="trainSmiles">训练语料。</param>
        ''' <param name="validSmiles">验证语料；<c>Nothing</c> 表示不做验证。</param>
        ''' <returns>训练历史。</returns>
        Public Function Train(trainSmiles As IEnumerable(Of String),
                              Optional validSmiles As IEnumerable(Of String) = Nothing) As IReadOnlyList(Of PretrainProgress)
            If trainSmiles Is Nothing Then Throw New ArgumentNullException(NameOf(trainSmiles))

            Dim trainLoader As New SequenceDataLoader(trainSmiles, _vocab, _options.SequenceLength,
                                                      _options.EnumerateSmiles, _options.Seed)
            Dim validLoader As SequenceDataLoader = Nothing

            If validSmiles IsNot Nothing Then
                validLoader = New SequenceDataLoader(validSmiles, _vocab, _options.SequenceLength, False, _options.Seed)
            End If

            Dim iteration As Integer = 0

            For epoch As Integer = 1 To _options.Epochs
                For Each batch As SmilesBatch In trainLoader.Batches(_options.BatchSize, _options.Shuffle)
                    iteration += 1

                    Dim loss As Double = _model.TrainStep(batch.Ids, _options.LearningRate, iteration, _options.IgnoreIndex)
                    Dim progress As New PretrainProgress With {
                        .Epoch = epoch,
                        .Iteration = iteration,
                        .Loss = loss,
                        .Perplexity = std.Exp(loss)
                    }

                    If validLoader IsNot Nothing AndAlso _options.ValidateEverySteps > 0 AndAlso
                       iteration Mod _options.ValidateEverySteps = 0 Then
                        Dim validation As Double = evaluate(validLoader)

                        progress.ValidationLoss = validation
                        log($"Val {epoch,3}: iter {iteration,5} | loss {validation:F3} | ppl {std.Exp(validation):F3}")

                        If Not _bestLoss.HasValue OrElse validation < _bestLoss.Value Then
                            _bestLoss = validation
                            captureBest()
                            log("[!] best model updated")

                            If Not String.IsNullOrEmpty(_options.CheckpointPath) Then
                                Call SaveBest(_options.CheckpointPath)
                            End If
                        End If
                    End If

                    _history.Add(progress)

                    If iteration = 1 OrElse iteration Mod 100 = 0 Then
                        log($"Train {epoch,3}: iter {iteration,5} | loss {loss:F3} | ppl {std.Exp(loss):F3}")
                    End If
                Next
            Next

            Return _history
        End Function

        ''' <summary>
        ''' 计算验证集上的平均 NLL（不更新参数、不应用 dropout）。
        ''' </summary>
        ''' <param name="loader">验证数据。</param>
        ''' <param name="batchSize">批大小。</param>
        Public Function EvaluateValidation(loader As SequenceDataLoader,
                                           Optional batchSize As Integer = 8) As Double
            Return evaluate(loader, batchSize)
        End Function

        ''' <summary>把最优权重写入 <c>.pt</c> 归档。</summary>
        ''' <param name="path">目标文件。</param>
        Public Sub SaveBest(path As String)
            Dim folder As String = IO.Path.GetDirectoryName(IO.Path.GetFullPath(path))

            If Not String.IsNullOrEmpty(folder) AndAlso Not Directory.Exists(folder) Then
                Call Directory.CreateDirectory(folder)
            End If

            Dim weights As Dictionary(Of String, Tensor)

            If _bestWeights.Count > 0 Then
                weights = New Dictionary(Of String, Tensor)(_bestWeights, StringComparer.Ordinal)
            Else
                weights = TransformerStateDict.Export(_model)
            End If

            Call PtStateDict.FromTensors(weights, PtElementType.Float32).Save(path)
        End Sub

        Private Function evaluate(loader As SequenceDataLoader, Optional batchSize As Integer = 8) As Double
            Dim total As Double = 0.0
            Dim batches As Integer = 0

            For Each batch As SmilesBatch In loader.Batches(batchSize, False)
                Dim logProbs As Tensor = _model.Predict(batch.Ids)
                Dim targets As Integer() = flatten(batch.Ids)
                Dim dLogits As Tensor = Nothing

                total += TensorOpsEx.NllLoss(logProbs, targets, _options.IgnoreIndex, dLogits)
                batches += 1
            Next

            If batches = 0 Then Return 0.0

            Return total / batches
        End Function

        Private Sub captureBest()
            Dim snapshot As Dictionary(Of String, Tensor) = TransformerStateDict.Export(_model)

            _bestWeights.Clear()

            For Each kv As KeyValuePair(Of String, Tensor) In snapshot
                ' 复制数值，避免后续训练把最优快照覆盖掉
                _bestWeights(kv.Key) = New Tensor(CType(kv.Value.Data.Clone(), Double()), kv.Value.Shape)
            Next
        End Sub

        Private Shared Function flatten(ids As Integer(,)) As Integer()
            Dim batch As Integer = ids.GetLength(0)
            Dim time As Integer = ids.GetLength(1)
            Dim targets(batch * time - 1) As Integer

            For b As Integer = 0 To batch - 1
                For t As Integer = 0 To time - 1
                    targets(b * time + t) = ids(b, t)
                Next
            Next

            Return targets
        End Function

        Private Sub log(message As String)
            If Logger IsNot Nothing Then
                Call Logger.Invoke(message)
            Else
                Console.WriteLine(message)
            End If
        End Sub

    End Class

End Namespace
