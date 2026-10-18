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
        Public Property PretrainLoss As New List(Of Double)()

        ''' <summary>微调阶段每个 epoch 的训练损失</summary>
        Public Property TrainLoss As New List(Of Double)()

        ''' <summary>微调阶段每个 epoch 的验证损失（无验证集时为空）</summary>
        Public Property ValidationLoss As New List(Of Double)()

        ''' <summary>最优验证损失对应的 epoch（从 1 开始；无验证集时为 0）</summary>
        Public Property BestEpoch As Integer

        ''' <summary>最优验证损失</summary>
        Public Property BestValidationLoss As Double = Double.PositiveInfinity

        ''' <summary>实际执行的微调轮数</summary>
        Public ReadOnly Property FinetuneEpochs As Integer
            Get
                Return TrainLoss.Count
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
        Private ReadOnly rand As Random

        Private stepIndex As Integer
        Private ReadOnly bestParameters As New Dictionary(Of String, Double())()

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
        ''' 创建训练器。
        ''' </summary>
        ''' <param name="model">待训练模型</param>
        ''' <param name="config">配置（学习率、损失权重、早停等）</param>
        Public Sub New(model As GRNModel, config As GPSConfig)
            If model Is Nothing Then Throw New ArgumentNullException(NameOf(model))
            If config Is Nothing Then Throw New ArgumentNullException(NameOf(config))

            Me.targetModel = model
            Me.cfg = config
            Me.rand = New Random(config.Seed + 7919)
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

            Dim result As New List(Of Double)()
            Dim total As Integer = If(epochs >= 0, epochs, cfg.PretrainEpochs)
            Dim print As Integer = If(printEvery >= 0, printEvery, cfg.PrintEvery)

            If total = 0 OrElse windows Is Nothing OrElse windows.Count = 0 Then
                Return result
            End If

            For epoch As Integer = 1 To total
                Dim lossSum As Double = 0.0
                Dim count As Integer = 0

                For Each window As TimeSeriesWindow In Shuffle(windows)
                    Dim loss As Double = TrainMaskedSample(window)

                    lossSum += loss
                    count += 1
                Next

                Dim epochLoss As Double = If(count > 0, lossSum / count, 0.0)

                Call result.Add(epochLoss)

                If print > 0 AndAlso (epoch = 1 OrElse epoch Mod print = 0 OrElse epoch = total) Then
                    Console.WriteLine($"  [GPS] 预训练 epoch {epoch,4} / {total}  loss = {epochLoss:F6}")
                End If
            Next

            Return result
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
                Return result
            End If

            Dim patience As Integer = 0

            For epoch As Integer = 1 To total
                Dim lossSum As Double = 0.0
                Dim count As Integer = 0

                For Each window As TimeSeriesWindow In Shuffle(trainWindows)
                    Dim loss As Double = TrainPredictionSample(window)

                    lossSum += loss
                    count += 1
                Next

                Dim epochLoss As Double = If(count > 0, lossSum / count, 0.0)

                Call result.TrainLoss.Add(epochLoss)

                Dim validationLoss As Double = 0.0

                If hasValidation Then
                    validationLoss = EvaluateLoss(validationWindows)

                    Call result.ValidationLoss.Add(validationLoss)

                    If validationLoss < result.BestValidationLoss Then
                        result.BestValidationLoss = validationLoss
                        result.BestEpoch = epoch
                        Call Snapshot()
                        patience = 0
                    Else
                        patience += 1
                    End If
                End If

                If print > 0 AndAlso (epoch = 1 OrElse epoch Mod print = 0 OrElse epoch = total) Then
                    If hasValidation Then
                        Console.WriteLine($"  [GPS] 微调 epoch {epoch,4} / {total}  train = {epochLoss:F6}  val = {validationLoss:F6}")
                    Else
                        Console.WriteLine($"  [GPS] 微调 epoch {epoch,4} / {total}  train = {epochLoss:F6}")
                    End If
                End If

                If hasValidation AndAlso cfg.EarlyStopPatience > 0 AndAlso patience >= cfg.EarlyStopPatience Then
                    Console.WriteLine($"  [GPS] 验证集连续 {patience} 轮未改善，提前停止")
                    Exit For
                End If
            Next

            ' 回滚到验证集最优参数
            If hasValidation AndAlso result.BestEpoch > 0 Then
                Call Restore()
            End If

            Return result
        End Function

        ''' <summary>
        ''' 在一个窗口上执行一次掩码重建训练步。
        ''' </summary>
        ''' <param name="window">训练窗口</param>
        ''' <returns>该样本的损失</returns>
        Private Function TrainMaskedSample(window As TimeSeriesWindow) As Double
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
        ''' <returns>打乱后的副本</returns>
        Private Function Shuffle(windows As List(Of TimeSeriesWindow)) As List(Of TimeSeriesWindow)
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

    End Class

End Namespace
