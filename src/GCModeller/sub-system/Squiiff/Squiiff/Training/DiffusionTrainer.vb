Imports System.Diagnostics
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.Squiiff.Model
Imports SMRUCC.genomics.Analysis.Squiiff.NN
Imports std = System.Math

Namespace Training

    ''' <summary>
    ''' 扩散自编码器训练器：数据洗牌 + 小批量 + Adam + 全局梯度裁剪 +
    ''' warmup/余弦退火学习率 + 早停 + 损失曲线记录。
    '''
    ''' 训练循环本身极简（扩散模型的标准简化噪声预测损失），
    ''' 工程上的关键在于稳定训练：梯度裁剪与学习率调度对扩散模型收敛影响很大。
    ''' </summary>
    Public Class DiffusionTrainer

        Private ReadOnly _model As DiffusionAutoEncoder
        Private ReadOnly _verbose As Boolean
        Private ReadOnly _history As New TrainingHistory
        Private ReadOnly _optimizer As AdamOptimizer
        Private ReadOnly _rng As Random

        Public Sub New(model As DiffusionAutoEncoder, Optional verbose As Boolean = True, Optional logInterval As Integer = 10)
            If model Is Nothing Then Throw New ArgumentNullException(NameOf(model))

            Me._model = model
            Me._verbose = verbose
            Me.LogInterval = logInterval
            Me._rng = New Random(model.Config.Seed + 777)
            Me._optimizer = New AdamOptimizer(model.Parameters) With {
                .LearningRate = model.Config.LearningRate,
                .WeightDecay = model.Config.WeightDecay,
                .ClipNorm = model.Config.ClipNorm
            }
        End Sub

        ''' <summary>每隔多少轮在控制台输出一次进度。</summary>
        Public Property LogInterval As Integer = 10

        Public ReadOnly Property Model As DiffusionAutoEncoder
            Get
                Return _model
            End Get
        End Property

        Public ReadOnly Property Optimizer As AdamOptimizer
            Get
                Return _optimizer
            End Get
        End Property

        Public ReadOnly Property History As TrainingHistory
            Get
                Return _history
            End Get
        End Property

        ''' <summary>训练模型。</summary>
        ''' <param name="x0">全部训练细胞（干净表达谱）<c>[N,G]</c>。</param>
        ''' <param name="epochs">覆盖配置中的轮数（为空则使用 <see cref="SquiiffConfig.Epochs"/>）。</param>
        Public Function Fit(x0 As Tensor, Optional epochs As Integer? = Nothing) As TrainingHistory
            Dim config = _model.Config
            Dim totalEpochs = If(epochs.HasValue, epochs.Value, config.Epochs)
            Dim sampleCount = x0.Shape(0)
            Dim batchSize = std.Min(config.BatchSize, sampleCount)
            Dim stepsPerEpoch = std.Max(1, sampleCount \ batchSize)
            Dim totalSteps = std.Max(1, totalEpochs * stepsPerEpoch)
            Dim warmupSteps = std.Max(1, CInt(totalSteps * config.WarmupRatio))

            Dim globalWatch As Stopwatch = Stopwatch.StartNew()
            Dim epochWatch As New Stopwatch()
            Dim stepIndex As Integer = 0
            Dim stagnantEpochs As Integer = 0
            Dim lastLearningRate As Double = config.LearningRate

            If _verbose Then
                Console.WriteLine($"  模型参数总量 = {_model.ParameterSize:N0}")
                Console.WriteLine($"  训练细胞数 = {sampleCount}  批量 = {batchSize}  每轮步数 = {stepsPerEpoch}  总步数 = {totalSteps}")
                Console.WriteLine($"  学习率 warmup = {warmupSteps} 步；梯度裁剪阈值 = {config.ClipNorm}")
            End If

            For epoch As Integer = 1 To totalEpochs
                Dim order = ShuffledIndices(sampleCount)
                Dim sumDiffusion As Double = 0.0
                Dim sumKL As Double = 0.0
                Dim sumTotal As Double = 0.0
                Dim sumGradient As Double = 0.0

                epochWatch.Restart()

                For s As Integer = 0 To stepsPerEpoch - 1
                    Dim batch = TakeBatch(x0, order, s * batchSize, batchSize)
                    Dim lr = LearningRateAt(stepIndex, totalSteps, warmupSteps)
                    Dim result = _model.TrainStep(batch, _optimizer, lr)

                    sumDiffusion += result.DiffusionLoss
                    sumKL += result.KLLoss
                    sumTotal += result.TotalLoss
                    sumGradient += result.GradientNorm
                    lastLearningRate = lr
                    stepIndex += 1
                Next

                epochWatch.Stop()

                Dim record As New EpochRecord With {
                    .Epoch = epoch,
                    .Steps = stepsPerEpoch,
                    .DiffusionLoss = sumDiffusion / stepsPerEpoch,
                    .KLLoss = sumKL / stepsPerEpoch,
                    .TotalLoss = sumTotal / stepsPerEpoch,
                    .LearningRate = lastLearningRate,
                    .GradientNorm = sumGradient / stepsPerEpoch,
                    .ElapsedSeconds = epochWatch.Elapsed.TotalSeconds
                }

                Dim improved = record.TotalLoss < _history.BestLoss
                _history.Add(record)

                If improved Then
                    stagnantEpochs = 0
                Else
                    stagnantEpochs += 1
                End If

                If _verbose AndAlso (epoch = 1 OrElse epoch = totalEpochs OrElse epoch Mod std.Max(1, LogInterval) = 0) Then
                    Console.WriteLine($"    epoch {epoch,4}/{totalEpochs}  " &
                                      $"噪声损失={record.DiffusionLoss:F5}  KL={record.KLLoss:E3}  " &
                                      $"总损失={record.TotalLoss:F5}  lr={record.LearningRate:E3}  " &
                                      $"|g|={record.GradientNorm:F3}  {record.ElapsedSeconds:F1}s")
                End If

                If config.EarlyStoppingPatience > 0 AndAlso stagnantEpochs >= config.EarlyStoppingPatience Then
                    If _verbose Then
                        Console.WriteLine($"    早停触发：连续 {stagnantEpochs} 轮无改善（最佳轮次 = {_history.BestEpoch}）")
                    End If
                    Exit For
                End If
            Next

            globalWatch.Stop()
            _history.TotalSeconds = globalWatch.Elapsed.TotalSeconds

            If _verbose Then
                Console.WriteLine($"  训练完成：{_history.Records.Count} 轮，耗时 {_history.TotalSeconds:F1}s，最佳总损失 {_history.BestLoss:F5}（第 {_history.BestEpoch} 轮）")
            End If

            Return _history
        End Function

        ''' <summary>warmup + 余弦退火学习率。</summary>
        Private Function LearningRateAt(stepIndex As Integer, totalSteps As Integer, warmupSteps As Integer) As Double
            Dim config = _model.Config
            Dim baseRate = config.LearningRate

            If stepIndex < warmupSteps Then
                Return baseRate * CDbl(stepIndex + 1) / warmupSteps
            End If

            Dim span = std.Max(1, totalSteps - warmupSteps)
            Dim progress = std.Min(1.0, std.Max(0.0, CDbl(stepIndex - warmupSteps) / span))
            Dim minRatio = std.Min(1.0, std.Max(0.0, config.MinLearningRateRatio))

            Return baseRate * (minRatio + (1.0 - minRatio) * 0.5 * (1.0 + std.Cos(std.PI * progress)))
        End Function

        Private Function ShuffledIndices(count As Integer) As Integer()
            Dim order(count - 1) As Integer
            For i As Integer = 0 To count - 1
                order(i) = i
            Next

            For i As Integer = 0 To count - 2
                Dim j = _rng.Next(i, count)
                Dim tmp = order(i)
                order(i) = order(j)
                order(j) = tmp
            Next

            Return order
        End Function

        ''' <summary>按行索引取出一个小批量（行优先拷贝）。</summary>
        Private Shared Function TakeBatch(source As Tensor, order As Integer(), offset As Integer, batchSize As Integer) As Tensor
            Dim columns = source.Shape(1)
            Dim batch = New Tensor(New Integer() {batchSize, columns})
            Dim src = source.Data
            Dim dst = batch.Data

            For i As Integer = 0 To batchSize - 1
                Dim row = order(offset + i)
                Call Array.Copy(src, row * columns, dst, i * columns, columns)
            Next

            Call batch.MarkHostModified()
            Return batch
        End Function
    End Class
End Namespace
