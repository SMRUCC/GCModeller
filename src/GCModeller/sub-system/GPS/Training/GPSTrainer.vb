Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.GPS.Config
Imports SMRUCC.genomics.Analysis.GPS.Data
Imports SMRUCC.genomics.Analysis.GPS.Model

Namespace Training

    ''' <summary>
    ''' 两阶段训练的结果记录。
    ''' </summary>
    Public Class GPSTrainingResult

        ''' <summary>掩码重建预训练每个 epoch 的平均损失</summary>
        Public Property PretrainLoss As Double()

        ''' <summary>微调阶段每个 epoch 的训练损失</summary>
        Public Property TrainLoss As Double()

        ''' <summary>微调阶段每个 epoch 的验证损失（无验证集时为空）</summary>
        Public Property ValidationLoss As Double()

        ''' <summary>最优验证损失对应的 epoch（从 1 开始；无验证集时为 0）</summary>
        Public Property BestEpoch As Integer

        ''' <summary>最优验证损失</summary>
        Public Property BestValidationLoss As Double = Double.PositiveInfinity

        ''' <summary>实际执行的微调轮数</summary>
        Public ReadOnly Property FinetuneEpochs As Integer
            Get
                Return TrainLoss.Length
            End Get
        End Property

    End Class

    ''' <summary>
    ''' GPS 模型的两阶段训练器（method.md §5.2）。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 阶段一（掩码重建自监督预训练）：把输入窗口最后一帧的部分基因置零（z-score 空间即均值），
    ''' 只在被掩码的基因上计算 autofocus 损失。本实现把"重建被掩码的当前值"
    ''' 与"预测下一时刻"合并为同一个前向任务：模型必须在缺失当前值的情况下
    ''' 借助历史与图结构推断未来，等价于原文的掩码自监督目标但无需额外的重建头。
    ''' </para>
    ''' <para>
    ''' 阶段二（下一时刻预测微调）：autofocus 主损失 + 方向感知损失；
    ''' 启用 <see cref="GPSConfig.UseUncertainty"/> 时主损失改为高斯负对数似然。
    ''' 每步做全局梯度范数裁剪（<see cref="GPSConfig.GradientClip"/>），
    ''' 按验证集早停并回滚到最优参数。
    ''' </para>
    ''' <para>
    ''' 关键约束：始终按时间顺序划分窗口（<see cref="TimeSeriesWindowDataset"/> 已保证），
    ''' 这里只对训练集内部做随机重排，不会造成时间信息泄漏。
    ''' </para>
    ''' </remarks>
    Public Class GPSTrainer

        Private ReadOnly targetModel As GRNModel
        Private ReadOnly cfg As GPSConfig

        Private stepIndex As Integer
        Private ReadOnly bestParameters As New Dictionary(Of String, Double())()

        ' ---- 早停状态（全新训练为默认值；RestoreState 从 checkpoint 回填） ----
        Private _bestValidationLoss As Double = Double.PositiveInfinity
        Private _bestEpoch As Integer = -1
        Private _hasSnapshot As Boolean = False
        Private _patience As Integer = 0

        ' ---- 阶段进度与损失曲线前缀（续训时与新段拼接） ----
        Private _phase As Integer = GPSTrainingState.PhasePretrain
        Private _pretrainCurve As New List(Of Double)()
        Private _trainCurve As New List(Of Double)()
        Private _validationCurve As New List(Of Double)()
        Private _pretrainStart As Integer = 0
        Private _finetuneStart As Integer = 0

        ' ---- 周期性 checkpoint ----
        Private _checkpointInterval As Integer = 0
        Private _onCheckpoint As Action(Of GPSTrainingState)

        ''' <summary>被训练的模型</summary>
        Public ReadOnly Property Model As GRNModel
            Get
                Return targetModel
            End Get
        End Property

        ''' <summary>已经执行的参数更新步数</summary>
        Public ReadOnly Property StepCount As Integer
            Get
                Return stepIndex
            End Get
        End Property

        ''' <summary>
        ''' checkpoint 间隔（单位：epoch）；0 表示关闭。仅在 epoch 边界生效。
        ''' </summary>
        Public Property CheckpointInterval As Integer
            Get
                Return _checkpointInterval
            End Get
            Set(value As Integer)
                _checkpointInterval = System.Math.Max(0, value)
            End Set
        End Property

        ''' <summary>
        ''' checkpoint 委托：每个间隔 epoch 结束时触发，参数为当前完整训练状态。
        ''' 由 <see cref="GPSModel"/> 挂接（负责落盘）；委托内部异常会被捕获并降级为警告，
        ''' 不会中断训练。
        ''' </summary>
        Public Property OnCheckpoint As Action(Of GPSTrainingState)
            Get
                Return _onCheckpoint
            End Get
            Set(value As Action(Of GPSTrainingState))
                _onCheckpoint = value
            End Set
        End Property

        ''' <summary>
        ''' 创建训练器。
        ''' </summary>
        ''' <param name="model">待训练模型</param>
        ''' <param name="config">配置（学习率、损失权重、早停等）</param>
        Public Sub New(model As GRNModel, config As GPSConfig)
            If model Is Nothing Then Throw New ArgumentNullException(NameOf(model))
            If config Is Nothing Then Throw New ArgumentNullException(NameOf(config))

            Me.targetModel = model
            Me.cfg = config
        End Sub

        ''' <summary>
        ''' 阶段一：掩码重建自监督预训练。
        ''' </summary>
        ''' <param name="windows">训练窗口</param>
        ''' <param name="epochs">轮数；&lt; 0 时取配置值</param>
        ''' <param name="printEvery">每隔多少轮打印；&lt; 0 时取配置值</param>
        ''' <returns>每个 epoch 的平均损失</returns>
        Public Function Pretrain(windows As List(Of TimeSeriesWindow),
                                 Optional epochs As Integer = -1,
                                 Optional printEvery As Integer = -1) As List(Of Double)

            Dim total As Integer = If(epochs >= 0, epochs, cfg.PretrainEpochs)
            Dim print As Integer = If(printEvery >= 0, printEvery, cfg.PrintEvery)

            If total = 0 OrElse windows Is Nothing OrElse windows.Count = 0 Then
                Return New List(Of Double)(_pretrainCurve)
            End If

            _phase = GPSTrainingState.PhasePretrain
            Dim bar As ProgressBar = Nothing

            ' 每个 epoch 使用由 (种子, epoch) 决定的独立随机源：洗牌顺序与掩码采样
            ' 只依赖 epoch 序号，与"从头跑"还是"从 checkpoint 续跑"无关，保证轨迹可复现。
            For Each epoch As Integer In TqdmWrapper.Range(_pretrainStart, total - _pretrainStart, bar:=bar)
                Dim epochRand As New Random(cfg.Seed + 7919 + epoch * 1000003)
                Dim lossSum As Double = 0.0
                Dim count As Integer = 0

                For Each window As TimeSeriesWindow In Shuffle(windows, epochRand)
                    Dim loss As Double = TrainMaskedSample(window, epochRand)

                    lossSum += loss
                    count += 1
                Next

                Dim epochLoss As Double = If(count > 0, lossSum / count, 0.0)

                Call _pretrainCurve.Add(epochLoss)
                Call bar.SetLabel($"  [GPS] 预训练 epoch {epoch,4} / {total}  loss = {epochLoss:F6}")
                Call MaybeCheckpoint()
            Next

            Return New List(Of Double)(_pretrainCurve)
        End Function

        ''' <summary>
        ''' 阶段二：下一时刻预测微调（含早停与最优参数回滚）。
        ''' </summary>
        ''' <param name="trainWindows">训练窗口（时间上最早的一段）</param>
        ''' <param name="validationWindows">验证窗口（时间上紧随其后的一段，可为空）</param>
        ''' <param name="epochs">轮数；&lt; 0 时取配置值</param>
        ''' <param name="printEvery">每隔多少轮打印；&lt; 0 时取配置值</param>
        ''' <returns>训练结果（含损失曲线与最优 epoch）</returns>
        Public Function Finetune(trainWindows As List(Of TimeSeriesWindow),
                                 Optional validationWindows As List(Of TimeSeriesWindow) = Nothing,
                                 Optional epochs As Integer = -1,
                                 Optional printEvery As Integer = -1) As GPSTrainingResult

            Dim result As New GPSTrainingResult()
            Dim total As Integer = If(epochs >= 0, epochs, cfg.FinetuneEpochs)
            Dim print As Integer = If(printEvery >= 0, printEvery, cfg.PrintEvery)
            Dim hasValidation As Boolean = validationWindows IsNot Nothing AndAlso validationWindows.Count > 0

            If trainWindows Is Nothing OrElse trainWindows.Count = 0 Then
                result.TrainLoss = _trainCurve.ToArray()
                result.ValidationLoss = _validationCurve.ToArray()
                result.BestEpoch = _bestEpoch
                result.BestValidationLoss = _bestValidationLoss
                Return result
            End If

            _phase = GPSTrainingState.PhaseFinetune
            Dim bar As ProgressBar = Nothing

            ' 与 Pretrain 相同：每 epoch 独立随机源，保证续跑轨迹与连续训练一致
            For Each epoch As Integer In TqdmWrapper.Range(_finetuneStart, total - _finetuneStart, bar:=bar)
                Dim epochRand As New Random(cfg.Seed + 7919 + epoch * 1000003)
                Dim lossSum As Double = 0.0
                Dim count As Integer = 0

                For Each window As TimeSeriesWindow In Shuffle(trainWindows, epochRand)
                    Dim loss As Double = TrainPredictionSample(window)

                    lossSum += loss
                    count += 1
                Next

                Dim epochLoss As Double = If(count > 0, lossSum / count, 0.0)

                Call _trainCurve.Add(epochLoss)

                Dim validationLoss As Double = 0.0

                If hasValidation Then
                    validationLoss = EvaluateLoss(validationWindows)

                    Call _validationCurve.Add(validationLoss)

                    If validationLoss < _bestValidationLoss Then
                        _bestValidationLoss = validationLoss
                        _bestEpoch = epoch
                        Call Snapshot()
                        _hasSnapshot = True
                        _patience = 0
                    Else
                        _patience += 1
                    End If
                End If

                If hasValidation Then
                    Call bar.SetLabel($"  [GPS] 微调 epoch {epoch,4} / {total}  train = {epochLoss:F6}  val = {validationLoss:F6}")
                Else
                    Call bar.SetLabel($"  [GPS] 微调 epoch {epoch,4} / {total}  train = {epochLoss:F6}")
                End If

                If hasValidation AndAlso cfg.EarlyStopPatience > 0 AndAlso _patience >= cfg.EarlyStopPatience Then
                    Console.WriteLine($"  [GPS] 验证集连续 {_patience} 轮未改善，提前停止")
                    Exit For
                End If

                Call MaybeCheckpoint()
            Next

            ' 回滚到验证集最优参数。
            ' 注意 epoch 为 0 基：最优恰为第 0 轮时同样需要回滚（旧实现用
            ' BestEpoch > 0 判断，会漏掉这种情况），故以"是否发生过快照"为准。
            If hasValidation AndAlso _hasSnapshot Then
                Call Restore()
            End If

            result.TrainLoss = _trainCurve.ToArray()
            result.ValidationLoss = _validationCurve.ToArray()
            result.BestEpoch = _bestEpoch
            result.BestValidationLoss = _bestValidationLoss

            Return result
        End Function

        ''' <summary>
        ''' 在一个窗口上执行一次掩码重建训练步。
        ''' </summary>
        ''' <param name="window">训练窗口</param>
        ''' <param name="window">训练窗口</param>
        ''' <param name="rand">本 epoch 的随机源（掩码采样）</param>
        ''' <returns>该样本的损失</returns>
        Private Function TrainMaskedSample(window As TimeSeriesWindow, rand As Random) As Double
            Dim n As Integer = window.NGene
            Dim mask As Boolean() = New Boolean(n - 1) {}
            Dim input As Double(,) = CType(window.Input.Clone(), Double(,))

            For i As Integer = 0 To n - 1
                If rand.NextDouble() < cfg.MaskRatio Then
                    mask(i) = True
                    ' z-score 空间下 0 即该基因的时间均值，作为"缺失"的占位
                    input(i, window.WindowSize - 1) = 0.0
                End If
            Next

            ' 至少掩码一个基因，否则该样本没有监督信号
            If Not mask.Contains(True) Then
                mask(rand.Next(n)) = True
                input(Array.IndexOf(mask, True), window.WindowSize - 1) = 0.0
            End If

            targetModel.Parameters.ZeroGradients()

            Dim prediction As Tensor = targetModel.Forward(input, True)
            Dim target As Tensor = ToColumn(window.Target)

            Dim loss As Double = LossFunctions.MaskedAutofocus(prediction, target, mask, cfg.AutofocusGamma)
            Dim gradient As Tensor = LossFunctions.MaskedAutofocusGradient(prediction, target, mask, cfg.AutofocusGamma)

            Call targetModel.Backward(gradient)
            Call ApplyStep()

            Return loss
        End Function

        ''' <summary>
        ''' 在一个窗口上执行一次下一时刻预测训练步。
        ''' </summary>
        ''' <param name="window">训练窗口</param>
        ''' <returns>该样本的复合损失</returns>
        Private Function TrainPredictionSample(window As TimeSeriesWindow) As Double
            targetModel.Parameters.ZeroGradients()

            Dim prediction As Tensor = targetModel.Forward(window.Input, True)
            Dim target As Tensor = ToColumn(window.Target)
            Dim loss As Double
            Dim gradient As Tensor
            Dim dSigma As Tensor = Nothing

            If cfg.UseUncertainty AndAlso targetModel.LastSigma IsNot Nothing Then
                loss = LossFunctions.GaussianNll(prediction, targetModel.LastSigma, target, cfg.AutofocusGamma)
                gradient = LossFunctions.GaussianNllGradients(prediction, targetModel.LastSigma, target,
                                                              cfg.AutofocusGamma, dSigma)
            Else
                loss = LossFunctions.Autofocus(prediction, target, cfg.AutofocusGamma)
                gradient = LossFunctions.AutofocusGradient(prediction, target, cfg.AutofocusGamma)
            End If

            If cfg.LambdaDir > 0.0 Then
                loss += cfg.LambdaDir * LossFunctions.SoftDirection(prediction, target, window.Previous, 2.0)

                Dim directionGradient As Tensor = LossFunctions.SoftDirectionGradient(
                    prediction, target, window.Previous, 2.0)

                Call TensorOpsAccumulate(gradient, directionGradient, cfg.LambdaDir)
            End If

            Call targetModel.Backward(gradient, dSigma)
            Call ApplyStep()

            Return loss
        End Function

        ''' <summary>
        ''' 在给定窗口集合上计算平均 autofocus 损失（不更新参数）。
        ''' </summary>
        ''' <param name="windows">窗口集合</param>
        ''' <returns>平均损失</returns>
        Public Function EvaluateLoss(windows As List(Of TimeSeriesWindow)) As Double
            If windows Is Nothing OrElse windows.Count = 0 Then Return 0.0

            Dim sum As Double = 0.0

            For Each window As TimeSeriesWindow In windows
                Dim prediction As Tensor = targetModel.Forward(window.Input, False)
                Dim target As Tensor = ToColumn(window.Target)

                sum += LossFunctions.Autofocus(prediction, target, cfg.AutofocusGamma)
            Next

            Return sum / windows.Count
        End Function

        ''' <summary>执行梯度裁剪与一步 Adam 更新。</summary>
        Private Sub ApplyStep()
            Call targetModel.Parameters.ClipGradients(cfg.GradientClip)

            stepIndex += 1
            Call targetModel.Parameters.MakeTrainingStep(cfg.LearningRate, stepIndex)
        End Sub

        ''' <summary>把一维向量包装为列向量 <c>[N, 1]</c> 张量。</summary>
        ''' <param name="values">一维数组</param>
        ''' <returns>列向量张量</returns>
        Private Shared Function ToColumn(values As Double()) As Tensor
            Dim t As Tensor = New Tensor(values.Length, 1)

            Call System.Array.Copy(values, 0, t.Data, 0, values.Length)
            Call t.MarkHostModified()

            Return t
        End Function

        ''' <summary>把梯度按权重累加到主梯度上。</summary>
        ''' <param name="target">主梯度</param>
        ''' <param name="source">附加梯度</param>
        ''' <param name="weight">权重</param>
        Private Shared Sub TensorOpsAccumulate(target As Tensor, source As Tensor, weight As Double)
            Dim td As Double() = target.Data
            Dim sd As Double() = source.Data

            For i As Integer = 0 To td.Length - 1
                td(i) += weight * sd(i)
            Next

            Call target.MarkHostModified()
        End Sub

        ''' <summary>训练集内部随机重排（不跨时间切分边界）。</summary>
        ''' <param name="windows">窗口列表</param>
        ''' <param name="rand">随机源（由调用方按 epoch 播种）</param>
        ''' <returns>打乱后的副本</returns>
        Private Shared Function Shuffle(windows As List(Of TimeSeriesWindow), rand As Random) As List(Of TimeSeriesWindow)
            Dim copy As New List(Of TimeSeriesWindow)(windows)

            For i As Integer = copy.Count - 1 To 1 Step -1
                Dim j As Integer = rand.Next(i + 1)
                Dim tmp As TimeSeriesWindow = copy(i)

                copy(i) = copy(j)
                copy(j) = tmp
            Next

            Return copy
        End Function

        ''' <summary>保存当前全部参数（早停回滚用）。</summary>
        Private Sub Snapshot()
            bestParameters.Clear()

            For Each entry As ParameterEntry In targetModel.Parameters.Entries
                bestParameters(entry.Name) = CType(entry.Value.Data.Clone(), Double())
            Next
        End Sub

        ''' <summary>恢复最近一次 <see cref="Snapshot"/> 保存的参数。</summary>
        Private Sub Restore()
            For Each entry As ParameterEntry In targetModel.Parameters.Entries
                Dim saved As Double() = Nothing

                If bestParameters.TryGetValue(entry.Name, saved) Then
                    Call System.Array.Copy(saved, entry.Value.Data, saved.Length)
                    Call entry.Value.MarkHostModified()
                End If
            Next
        End Sub

        ' ==================== 中断恢复 ====================

        ''' <summary>
        ''' 从 checkpoint 状态恢复 trainer 内部状态：步计数、早停计数、最优 epoch 快照、
        ''' 曲线前缀与阶段进度。恢复后 <see cref="Pretrain"/>/<see cref="Finetune"/>
        ''' 自动从下一 epoch 续跑。
        ''' </summary>
        ''' <param name="state">持久化的训练状态；Nothing 时等价于 <see cref="ResetTrainingState"/></param>
        Public Sub RestoreState(state As GPSTrainingState)
            If state Is Nothing Then
                Call ResetTrainingState()
                Return
            End If

            stepIndex = System.Math.Max(0, state.StepIndex)
            _patience = System.Math.Max(0, state.Patience)
            _bestEpoch = state.BestEpoch
            _bestValidationLoss = state.BestValidationLoss
            _hasSnapshot = state.BestEpoch >= 0

            bestParameters.Clear()

            For Each kv As KeyValuePair(Of String, Double()) In state.BestParameters
                bestParameters(kv.Key) = CType(kv.Value.Clone(), Double())
            Next

            Call ReplaceCurve(_pretrainCurve, state.PretrainLoss)
            Call ReplaceCurve(_trainCurve, state.TrainLoss)
            Call ReplaceCurve(_validationCurve, state.ValidationLoss)

            _pretrainStart = _pretrainCurve.Count
            _finetuneStart = _trainCurve.Count
            _phase = state.Phase
        End Sub

        ''' <summary>
        ''' 清空全部训练状态（步计数、早停、曲线、阶段进度），
        ''' 使下一次 <see cref="Pretrain"/>/<see cref="Finetune"/> 从第 0 epoch 全新开始。
        ''' </summary>
        Public Sub ResetTrainingState()
            stepIndex = 0
            _patience = 0
            _bestEpoch = -1
            _bestValidationLoss = Double.PositiveInfinity
            _hasSnapshot = False
            bestParameters.Clear()
            _pretrainCurve.Clear()
            _trainCurve.Clear()
            _validationCurve.Clear()
            _pretrainStart = 0
            _finetuneStart = 0
            _phase = GPSTrainingState.PhasePretrain
        End Sub

        ''' <summary>
        ''' 捕获当前完整训练状态（供 checkpoint 落盘）。
        ''' </summary>
        ''' <returns>训练状态快照</returns>
        Public Function CaptureState() As GPSTrainingState
            Dim best As New Dictionary(Of String, Double())(StringComparer.Ordinal)

            For Each kv As KeyValuePair(Of String, Double()) In bestParameters
                best(kv.Key) = CType(kv.Value.Clone(), Double())
            Next

            Return New GPSTrainingState With {
                .Phase = _phase,
                .PretrainEpochsDone = _pretrainCurve.Count,
                .FinetuneEpochsDone = _trainCurve.Count,
                .StepIndex = stepIndex,
                .Patience = _patience,
                .BestEpoch = _bestEpoch,
                .BestValidationLoss = _bestValidationLoss,
                .BestParameters = best,
                .PretrainLoss = _pretrainCurve.ToArray(),
                .TrainLoss = _trainCurve.ToArray(),
                .ValidationLoss = _validationCurve.ToArray()
            }
        End Function

        ''' <summary>到达 checkpoint 间隔时触发委托；失败仅警告，不中断训练。</summary>
        Private Sub MaybeCheckpoint()
            If _checkpointInterval <= 0 OrElse _onCheckpoint Is Nothing Then Return

            Dim state As GPSTrainingState = CaptureState()
            Dim done As Integer = If(state.Phase = GPSTrainingState.PhasePretrain,
                                     state.PretrainEpochsDone,
                                     state.FinetuneEpochsDone)

            If done Mod _checkpointInterval <> 0 Then Return

            Try
                Call _onCheckpoint(state)
            Catch ex As Exception
                Console.WriteLine($"  [GPS] checkpoint 写入失败（训练继续）：{ex.Message}")
            End Try
        End Sub

        Private Shared Sub ReplaceCurve(target As List(Of Double), values As Double())
            target.Clear()

            If values Is Nothing Then Return

            Call target.AddRange(values)
        End Sub

    End Class

End Namespace
