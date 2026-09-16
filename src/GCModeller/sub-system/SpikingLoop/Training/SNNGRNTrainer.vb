' ============================================================================
' SNNGRNTrainer.vb — readme 三 · 训练模块
'
' 训练目标（readme 三.1）：
'   loss = MSE(y_hat, U_seq[t+Δt]) + α · loss_prior + β · loss_sparse
'
'   loss_prior  = MEAN( mask ⊙ confidence ⊙ (W − W₀)² )   结构先验正则
'                 —— 高置信度的 TF→Target 边被更强地钉在先验权重附近，防止漂移；
'   loss_sparse = Σ|W|                                    稀疏正则
'                 —— 抑制脉冲冗余与过拟合（readme 六的风险清单最后一条）。
'
' 工程实现要点（对应 readme 3.2 的六个步骤）：
'   Step 1 编码        ENCODE_INPUT(U_seq[t], mode="current")
'   Step 2 前向        u0 = gain·U_seq[t] → LIF_FORWARD(I_ext, u0, W, A)
'   Step 3 解码        DECODE_OUTPUT(S, u_final)
'   Step 4 损失        MSE + 结构正则 + 稀疏正则
'   Step 5 反向        BACKWARD_THROUGH_TIME（代理梯度）—— 掩码冻结，只更新 W 的自由参数
'   Step 6 更新        Adam（含梯度范数裁剪）
'
' 批处理策略：把"留出区间之外的全部时间窗"作为<b>一个 batch</b> 一次性前向。
'   每个时间窗（t → t+Δt）都是独立的 SNN 轨迹（各自从 U_seq[t] 初始化膜电位），
'   因此 batch 维天然可用；一次性前向能把 O(N²) 的突触乘法开销摊薄，
'   也避免了逐窗循环带来的重复矩阵分配。
'
' 验证与早停（readme 三.2 的"留出部分伪时间区间"）：
'   按伪时间顺序把训练窗与验证窗切分（验证集取尾部区间，模拟"外推预测"），
'   以验证损失作为早停依据，同时报告验证集 PCC（readme P3 的经验目标 PCC > 0.6）。
'
' 自适应替代梯度形状（readme 3.3 第 1 条）：
'   每个 epoch 结束后按 AlphaGrowth 调整替代梯度陡度 α，缓解"梯度不匹配"随
'   时间步累积带来的偏差。
' ============================================================================

Imports Microsoft.VisualBasic.DeepLearning.SpikingNeuralNetwork
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.SpikingLoop.Data
Imports SMRUCC.genomics.Analysis.SpikingLoop.Evaluation
Imports SMRUCC.genomics.Analysis.SpikingLoop.Model
Imports std = System.Math

Namespace Training

    ''' <summary>单轮训练记录（用于损失曲线与诊断）</summary>
    Public Structure TrainingRecord

        Public Epoch As Integer
        Public TrainLoss As Double
        Public ValidationLoss As Double
        Public ValidationPcc As Double
        Public PriorPenalty As Double
        Public SparsityPenalty As Double
        Public MeanFiringRate As Double
        Public Alpha As Double
        Public ElapsedMs As Long

        Public Overrides Function ToString() As String
            Return $"epoch {Epoch,4}  train={TrainLoss:F6}  val={ValidationLoss:F6}  " &
                   $"valPCC={ValidationPcc:F4}  prior={PriorPenalty:E2}  sparse={SparsityPenalty:E2}  " &
                   $"rate={MeanFiringRate:P1}  α={Alpha:F2}"
        End Function

    End Structure

    ''' <summary>训练结果汇总</summary>
    Public Class TrainingResult

        ''' <summary>逐轮训练记录</summary>
        Public Property History As New List(Of TrainingRecord)()

        ''' <summary>验证损失最优的轮次（无验证集时为 −1）</summary>
        Public Property BestEpoch As Integer = -1

        ''' <summary>最优验证损失</summary>
        Public Property BestValidationLoss As Double = Double.NaN

        ''' <summary>最优轮次的验证 PCC</summary>
        Public Property BestValidationPcc As Double = Double.NaN

        ''' <summary>最后一轮（或早停时）的训练损失</summary>
        Public Property FinalTrainLoss As Double

        ''' <summary>是否因验证集不再改善而提前停止</summary>
        Public Property StoppedEarly As Boolean

        ''' <summary>训练窗口（batch）数量</summary>
        Public Property TrainWindows As Integer

        ''' <summary>验证窗口数量</summary>
        Public Property ValidationWindows As Integer

        ''' <summary>总耗时（毫秒）</summary>
        Public Property ElapsedMs As Long

        ''' <summary>是否已把参数回滚到验证损失最优的轮次</summary>
        Public Property BestWeightsRestored As Boolean

        ''' <summary>最后一轮的验证损失（与 <see cref="BestValidationLoss"/> 对比可判断过拟合程度）</summary>
        Public Property FinalValidationLoss As Double = Double.NaN

        Public Overrides Function ToString() As String
            Dim restore = If(BestWeightsRestored, "已回滚至最优轮", "保持最后一轮")
            Return $"TrainingResult(epochs={History.Count}, best@{If(BestEpoch < 0, "-", BestEpoch.ToString())}, " &
                   $"val={BestValidationLoss:F6}, valPCC={BestValidationPcc:F4}, earlyStop={StoppedEarly}, " &
                   $"{restore}, {ElapsedMs} ms)"
        End Function

    End Class

    ''' <summary>SNN-GRN 训练器（代理梯度 + BPTT + Adam）</summary>
    Public Class SNNGRNTrainer

        ''' <summary>
        ''' 允许"按验证损失回滚最优轮次"所需的最少验证窗口数。
        ''' 低于该值时验证指标本身噪声过大，回滚可能选中欠训练的早期轮次。
        ''' </summary>
        Public Const MinWindowsForBestEpochSelection As Integer = 10

        Private ReadOnly _model As SNNGRNModel
        Private ReadOnly _config As SpikingLoopConfig
        Private ReadOnly _trajectory As PseudotimeResult
        Private ReadOnly _optimizer As AdamOptimizer

        ''' <summary>训练窗口总数（= NumBins − Horizon）</summary>
        Public ReadOnly Property TotalWindows As Integer

        ''' <summary>实际用于训练的窗口数</summary>
        Public ReadOnly Property TrainWindows As Integer

        ''' <summary>实际用于验证的窗口数（0 表示未留出验证集）</summary>
        Public ReadOnly Property ValidationWindows As Integer

        Public Sub New(model As SNNGRNModel, trajectory As PseudotimeResult,
                       Optional config As SpikingLoopConfig = Nothing)

            If model Is Nothing Then
                Throw New ArgumentNullException(NameOf(model))
            End If
            If trajectory Is Nothing Then
                Throw New ArgumentNullException(NameOf(trajectory))
            End If

            _model = model
            _trajectory = trajectory
            _config = If(config, model.Config)

            If _trajectory.NumGenes <> model.NumGenes Then
                Throw New ArgumentException(
                    $"训练轨迹的基因数({_trajectory.NumGenes})与模型神经元数({model.NumGenes})不一致；" &
                    "请确保先验图与训练轨迹基于同一份子网络基因列表")
            End If

            TotalWindows = _trajectory.NumBins - _config.Horizon
            If TotalWindows < 1 Then
                Throw New InvalidOperationException(
                    $"可用训练窗口为 {TotalWindows}：NumBins({_trajectory.NumBins}) " &
                    $"必须大于 Horizon({_config.Horizon})")
            End If

            TrainWindows = std.Max(1, CInt(std.Floor(TotalWindows * _config.TrainSplit)))
            ValidationWindows = TotalWindows - TrainWindows

            _optimizer = New AdamOptimizer(_config.LearningRate, _config.ClipNorm)
        End Sub

#Region "训练"

        ''' <summary>
        ''' 执行训练。
        ''' </summary>
        ''' <param name="onEpoch">
        ''' 每轮结束后的回调（用于打印日志或画损失曲线）；为 Nothing 时不回调。
        ''' </param>
        Public Function Train(Optional onEpoch As Action(Of TrainingRecord) = Nothing) As TrainingResult

            Dim watch = System.Diagnostics.Stopwatch.StartNew()
            Dim result As New TrainingResult With {
                .TrainWindows = TrainWindows,
                .ValidationWindows = ValidationWindows
            }

            Dim best = Double.NaN
            Dim patienceLeft = _config.EarlyStopPatience
            Dim snapshot As (w As Double(), rw As Double(), rb As Double()) = (Nothing, Nothing, Nothing)

            For epoch = 1 To _config.Epochs
                Dim record = TrainEpoch(epoch)

                result.History.Add(record)
                If onEpoch IsNot Nothing Then onEpoch(record)

                If _config.Verbose AndAlso _config.PrintEvery > 0 AndAlso epoch Mod _config.PrintEvery = 0 Then
                    Call $"[SpikingLoop] {record}".info
                End If

                ' ---- 最优轮次与早停 ----
                If Not Double.IsNaN(record.ValidationLoss) Then
                    If Double.IsNaN(best) OrElse record.ValidationLoss < best Then
                        best = record.ValidationLoss
                        result.BestEpoch = epoch
                        result.BestValidationLoss = record.ValidationLoss
                        result.BestValidationPcc = record.ValidationPcc
                        patienceLeft = _config.EarlyStopPatience

                        If _config.RestoreBestWeights Then
                            snapshot = (_model.Layer.Weight.ToDoubleArray(),
                                        _model.Readout.Weight.ToDoubleArray(),
                                        _model.Readout.Bias.ToDoubleArray())
                        End If
                    ElseIf _config.EarlyStopPatience > 0 Then
                        patienceLeft -= 1
                        If patienceLeft <= 0 Then
                            result.StoppedEarly = True
                            If _config.Verbose Then
                                Call $"[SpikingLoop] 验证损失连续 {_config.EarlyStopPatience} 轮无改善，提前停止于第 {epoch} 轮".info
                            End If
                            Exit For
                        End If
                    End If
                End If

                ' ---- 自适应替代梯度形状 ----
                _model.AnnealAlpha(_config.AlphaGrowth)
            Next

            watch.Stop()
            result.ElapsedMs = watch.ElapsedMilliseconds

            If result.History.Count > 0 Then
                Dim last = result.History(result.History.Count - 1)
                result.FinalTrainLoss = last.TrainLoss
                result.FinalValidationLoss = last.ValidationLoss
            Else
                result.FinalTrainLoss = Double.NaN
            End If

            ' ---- 回滚到验证损失最优的轮次 ----
            ' 仅在验证窗足够多时才回滚：验证集只有几~十几个窗口时，"按验证损失挑轮次"的
            ' 选择噪声会大于收益——被选中的可能是欠训练的早期轮次，导致整体指标反而变差。
            If _config.RestoreBestWeights AndAlso snapshot.w IsNot Nothing Then
                If ValidationWindows >= MinWindowsForBestEpochSelection Then
                    RestoreWeights(snapshot.w, snapshot.rw, snapshot.rb)
                    result.BestWeightsRestored = True
                ElseIf _config.Verbose Then
                    Dim skip = $"[SpikingLoop] 验证集仅 {ValidationWindows} 个窗口（< {MinWindowsForBestEpochSelection}），" &
                               "按验证损失挑选轮次的噪声过大，已跳过参数回滚；最终指标对应最后一轮" &
                               "（可在配置中增大 NumBins 或降低 TrainSplit 以扩大验证集）"
                    Call skip.info
                End If
            End If

            ' ---- 过拟合诊断：小样本场景下最容易被忽视的失败模式 ----
            If _config.Verbose AndAlso Not Double.IsNaN(result.FinalValidationLoss) AndAlso
               Not Double.IsNaN(result.BestValidationLoss) AndAlso
               result.FinalValidationLoss > result.BestValidationLoss * 1.5 Then
                Dim advice = $"[SpikingLoop] 过拟合提示：最优验证损失 {result.BestValidationLoss:F6}（第 {result.BestEpoch} 轮）" &
                             $"明显优于最后一轮 {result.FinalValidationLoss:F6}；已" &
                             $"{If(result.BestWeightsRestored, "回滚参数到最优轮", "未回滚（RestoreBestWeights=False）")}。" &
                             "可减小 Epochs、增大 PriorRegAlpha/SparsityBeta，或增加训练数据（更多伪时间分箱）"
                Call advice.warning
            End If

            ' 收敛性诊断：readme P3 的经验目标是验证集 PCC > 0.6
            If _config.Verbose Then
                If Not Double.IsNaN(result.BestValidationPcc) AndAlso result.BestValidationPcc < 0.6 Then
                    Dim advice = $"[SpikingLoop] 注意：验证集 PCC = {result.BestValidationPcc:F4} 低于 P3 经验目标 0.6；" &
                                 "可尝试减小学习率、增大 NumBins/SimulationSteps，或扫描 β(λ) 与复位模式"
                    Call advice.warning
                End If
            End If

            Return result
        End Function

        ''' <summary>单轮训练：前向 → 损失 → BPTT → 正则 → Adam 更新 → 验证</summary>
        Private Function TrainEpoch(epoch As Integer) As TrainingRecord
            Dim watch = System.Diagnostics.Stopwatch.StartNew()

            ' ---- Step 1-2：编码 + 前向演化（整批） ----
            Dim batch = BuildBatch(0, TrainWindows)
            _model.ZeroGrad()

            Dim output = _model.Forward(_model.EncodeCurrents(batch.x), _model.MapToMembrane(batch.x))

            ' ---- Step 3-4：MSE 损失 ----
            Dim mse = RegressionLosses.MeanSquaredError(output.YHat, batch.y)

            ' ---- Step 5：BPTT（掩码冻结，只更新 W 的自由参数） ----
            _model.Backward(mse.Grad)

            ' ---- 正则项：并入权重梯度 ----
            Dim gradW = New Tensor(_model.MaskedWeightGrad().ToDoubleArray(), _model.NumGenes, _model.NumGenes)
            Dim priorLoss = 0.0
            Dim sparseLoss = 0.0

            If _config.PriorRegAlpha > 0.0 Then
                Dim prior = RegressionLosses.MaskedDeviationPenalty(
                    _model.LearnedWeights, _model.Graph.WInit, _model.Graph.Mask, _model.Graph.Confidence)
                priorLoss = prior.Loss
                RegressionLosses.AddScaledGradientInPlace(gradW, prior.Grad, _config.PriorRegAlpha)
            End If

            If _config.SparsityBeta > 0.0 Then
                sparseLoss = RegressionLosses.L1Sum(_model.LearnedWeights)
                RegressionLosses.AddScaledGradientInPlace(
                    gradW, RegressionLosses.L1Gradient(_model.LearnedWeights, 1.0), _config.SparsityBeta)
            End If

            Dim totalLoss = mse.Loss + _config.PriorRegAlpha * priorLoss + _config.SparsityBeta * sparseLoss
            If Double.IsNaN(totalLoss) OrElse Double.IsInfinity(totalLoss) Then
                Throw New InvalidOperationException(
                    $"第 {epoch} 轮损失出现 NaN/Infinity（MSE={mse.Loss}）：" &
                    "请降低学习率、减小 SimulationSteps，或检查输入电流与阈值的量级是否匹配")
            End If

            ' ---- Step 6：Adam 更新 ----
            _optimizer.Update(_model.Layer.Weight, gradW, "GRN.synapses")
            _optimizer.Update(_model.Readout.Weight, _model.Readout.WeightGrad, "readout.weight")
            _optimizer.Update(_model.Readout.Bias, _model.Readout.BiasGrad, "readout.bias")

            If _config.EnforceMaskAfterUpdate Then _model.ApplyMask()

            ' ---- 验证（留出尾部伪时间区间） ----
            Dim valLoss = Double.NaN
            Dim valPcc = Double.NaN

            If ValidationWindows >= 2 Then
                Dim val = EvaluateWindows(TrainWindows, ValidationWindows)
                valLoss = val.loss
                valPcc = val.metrics.Pcc
            End If

            watch.Stop()

            Return New TrainingRecord With {
                .Epoch = epoch,
                .TrainLoss = mse.Loss,
                .ValidationLoss = valLoss,
                .ValidationPcc = valPcc,
                .PriorPenalty = priorLoss,
                .SparsityPenalty = sparseLoss,
                .MeanFiringRate = MeanFiringRate(output.SHistory),
                .Alpha = _model.Alpha,
                .ElapsedMs = watch.ElapsedMilliseconds
            }
        End Function

#End Region

#Region "评估"

        ''' <summary>
        ''' 在指定的时间窗区间上做前向评估（不更新参数）。
        ''' </summary>
        ''' <param name="startWindow">起始窗口索引</param>
        ''' <param name="count">窗口数量</param>
        Public Function EvaluateWindows(startWindow As Integer, count As Integer) As (loss As Double, metrics As RegressionMetrics)
            Dim batch = BuildBatch(startWindow, count)
            Dim output = _model.Forward(_model.EncodeCurrents(batch.x), _model.MapToMembrane(batch.x))
            Dim mse = RegressionLosses.MeanSquaredError(output.YHat, batch.y)

            Return (mse.Loss, RegressionMetrics.Compute(output.YHat, batch.y))
        End Function

        ''' <summary>
        ''' 在整条训练轨迹上做预测，返回"预测表达"与"真实表达"两张 [T_train, N] 表，
        ''' 供输出预测散点/时序对比与最终指标汇报。
        ''' </summary>
        Public Function PredictAll() As (predicted As Tensor, actual As Tensor)
            Dim batch = BuildBatch(0, TotalWindows)
            Dim output = _model.Forward(_model.EncodeCurrents(batch.x), _model.MapToMembrane(batch.x))
            Return (output.YHat, batch.y)
        End Function

#End Region

#Region "批构造"

        ''' <summary>
        ''' 构造 batch：第 k 个样本 = (U_seq[start+k], U_seq[start+k+Δt])。
        ''' </summary>
        Private Function BuildBatch(startWindow As Integer, count As Integer) As (x As Tensor, y As Tensor)
            Dim n = _trajectory.NumGenes
            Dim u = _trajectory.U.Data

            Dim xd(count * n - 1) As Double
            Dim yd(count * n - 1) As Double

            For k = 0 To count - 1
                Dim t = startWindow + k
                Array.Copy(u, t * n, xd, k * n, n)
                Array.Copy(u, (t + _config.Horizon) * n, yd, k * n, n)
            Next

            Return (Tensor.Wrap(xd, count, n), Tensor.Wrap(yd, count, n))
        End Function

        ''' <summary>
        ''' 把参数就地回滚到快照。绕过 Tensor 索引器写入底层数组后，
        ''' 按既有设备端缓存契约调用 <see cref="Tensor.MarkHostModified"/> 声明主机数据已修改。
        ''' </summary>
        Private Sub RestoreWeights(w As Double(), readoutW As Double(), readoutB As Double())
            Array.Copy(w, _model.Layer.Weight.Data, w.Length)
            _model.Layer.Weight.MarkHostModified()

            Array.Copy(readoutW, _model.Readout.Weight.Data, readoutW.Length)
            _model.Readout.Weight.MarkHostModified()

            Array.Copy(readoutB, _model.Readout.Bias.Data, readoutB.Length)
            _model.Readout.Bias.MarkHostModified()
        End Sub

        Private Shared Function MeanFiringRate(sHistory As List(Of Tensor)) As Double
            If sHistory Is Nothing OrElse sHistory.Count = 0 Then Return 0.0

            Dim total = SpikeDecoders.TotalSpikeCount(sHistory)
            Dim units = sHistory(0).Shape(1)
            Dim batch = sHistory(0).Shape(0)

            Return total / (sHistory.Count * CDbl(batch) * units)
        End Function

#End Region

    End Class

End Namespace
