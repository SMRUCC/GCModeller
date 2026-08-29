Imports std = System.Math
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports GNN = Microsoft.VisualBasic.DeepLearning.GNN
Imports SMRUCC.genomics.Analysis.GEARS.Graph
Imports SMRUCC.genomics.Analysis.GEARS.Model

Namespace Training

    ''' <summary>
    ''' GEARS 模型训练器
    ''' </summary>
    ''' <remarks>
    ''' 训练流程严格对应 readme §7.2 的伪代码：
    ''' <list type="number">
    ''' <item><description>构建扰动标记 <c>p</c>（组合扰动为 multi-hot）；</description></item>
    ''' <item><description>构建初始节点特征 <c>h0 = [x̄ ‖ p ‖ e ‖ z_pert]</c>；</description></item>
    ''' <item><description>多层消息传递；</description></item>
    ''' <item><description>解码得到 Δ 预测，损失取 MSE(Δ̂, Δ)；</description></item>
    ''' <item><description>反向传播并用 Adam 更新参数。</description></item>
    ''' </list>
    '''
    ''' 归一化约定：输入表达按基因做 Z-score（减 <c>controlMean</c> 除 <c>controlSD</c>），
    ''' Δ 标签同样除以 <c>controlSD</c>。预测时把 Δ̂ 乘回 <c>controlSD</c> 即可还原到原始表达尺度。
    ''' </remarks>
    Public Class GEARSTrainer

        ''' <summary>待训练的模型</summary>
        ReadOnly model As GEARSModel

        ''' <summary>基因调控图</summary>
        ReadOnly graphData As GeneRegulatoryGraph

        ''' <summary>Adam 优化器</summary>
        ReadOnly optimizer As GNN.AdamOptimizer

        ''' <summary>control 表达均值</summary>
        ReadOnly controlMean As Double()

        ''' <summary>control 表达标准差（归一化尺度）</summary>
        ReadOnly controlSD As Double()

        ''' <summary>L2 正则化系数</summary>
        ReadOnly l2Lambda As Double

        ''' <summary>训练过程中每个 epoch 的平均损失</summary>
        ''' <returns>损失曲线</returns>
        Public ReadOnly Property LossCurve As New List(Of Double)()

        ''' <summary>模型可训练参数（交给优化器原地更新）</summary>
        ''' <returns>参数张量列表</returns>
        Public ReadOnly Property Parameters As List(Of Tensor)

        ''' <summary>模型参数梯度</summary>
        ''' <returns>梯度张量列表</returns>
        Public ReadOnly Property Gradients As List(Of Tensor)

        ''' <summary>
        ''' 创建训练器
        ''' </summary>
        ''' <param name="model">GEARS 模型</param>
        ''' <param name="graphData">基因调控图</param>
        ''' <param name="controlMean">control 表达均值 [numGenes]</param>
        ''' <param name="controlSD">control 表达标准差 [numGenes]</param>
        ''' <param name="learningRate">Adam 学习率</param>
        ''' <param name="l2Lambda">L2 正则化系数，0 表示不启用正则</param>
        Public Sub New(model As GEARSModel,
                       graphData As GeneRegulatoryGraph,
                       controlMean As Double(),
                       controlSD As Double(),
                       Optional learningRate As Single = 0.01F,
                       Optional l2Lambda As Double = 0.0)

            Me.model = model
            Me.graphData = graphData
            Me.controlMean = controlMean
            Me.controlSD = controlSD
            Me.l2Lambda = l2Lambda
            Me.Parameters = model.GetParameters()
            Me.Gradients = model.GetGradients()
            Me.optimizer = New GNN.AdamOptimizer(Me.Parameters, Me.Gradients, learningRate)

            If Me.Parameters.Count <> Me.Gradients.Count Then
                Throw New InvalidProgramException("模型参数数量与梯度数量不一致，无法创建优化器")
            End If
        End Sub

        ''' <summary>
        ''' 执行训练
        ''' </summary>
        ''' <param name="samples">训练样本集合</param>
        ''' <param name="epochs">训练轮数</param>
        ''' <param name="printEvery">每隔多少个 epoch 打印一次损失；0 表示不打印</param>
        ''' <returns>损失曲线（每个 epoch 的平均 MSE）</returns>
        Public Function Train(samples As List(Of PerturbSeqSample),
                              Optional epochs As Integer = 30,
                              Optional printEvery As Integer = 0) As Double()

            Dim n As Integer = model.NumGenes
            Dim dims As Integer = model.FeatureDim

            LossCurve.Clear()

            For epoch As Integer = 1 To epochs
                Dim lossSum As Double = 0
                Dim count As Integer = 0

                For Each sample As PerturbSeqSample In samples
                    Dim xNorm As Double() = New Double(n - 1) {}
                    Dim target As Double() = New Double(n - 1) {}

                    For i As Integer = 0 To n - 1
                        Dim sd As Double = std.Max(controlSD(i), 0.000001)

                        xNorm(i) = (sample.ControlExpression(i) - controlMean(i)) / sd
                        target(i) = (sample.PerturbedExpression(i) - sample.ControlExpression(i)) / sd
                    Next

                    Dim flag As Double() = sample.PerturbationFlag(n)
                    Dim pred As Tensor = model.Forward(xNorm, flag)
                    Dim targetTensor As Tensor = New Tensor(n, 1)

                    Call System.Array.Copy(target, targetTensor.Data, n)

                    Dim loss As Single = GNN.Loss.MeanSquaredError(pred, targetTensor)
                    Dim grad As Tensor = GNN.Loss.MeanSquaredErrorGradient(pred, targetTensor)

                    Call model.BackwardFrom(grad)
                    Call ApplyRegularization()
                    Call optimizer.Step()
                    Call optimizer.ZeroGrad()

                    lossSum += loss
                    count += 1
                Next

                Dim epochLoss As Double = If(count > 0, lossSum / count, 0.0)

                LossCurve.Add(epochLoss)

                If printEvery > 0 AndAlso (epoch = 1 OrElse epoch Mod printEvery = 0 OrElse epoch = epochs) Then
                    Console.WriteLine($"  [GEARS] epoch {epoch,4} / {epochs}  loss = {epochLoss.ToString("F6")}")
                End If
            Next

            Return LossCurve.ToArray()
        End Function

        ''' <summary>
        ''' 把 L2 正则项的梯度累加到参数梯度上（权重衰减）
        ''' </summary>
        Private Sub ApplyRegularization()
            If l2Lambda <= 0 Then
                Return
            End If

            For i As Integer = 0 To Parameters.Count - 1
                Dim pd As Double() = Parameters(i).Data
                Dim gd As Double() = Gradients(i).Data
                Dim m As Integer = std.Min(pd.Length, gd.Length)

                For j As Integer = 0 To m - 1
                    gd(j) += l2Lambda * pd(j)
                Next
            Next
        End Sub

        ''' <summary>
        ''' 在给定样本集上评估模型的平均 MSE（不改变模型参数）
        ''' </summary>
        ''' <param name="samples">评估样本集合</param>
        ''' <returns>平均均方误差（归一化尺度）</returns>
        Public Function Evaluate(samples As List(Of PerturbSeqSample)) As Double
            Dim n As Integer = model.NumGenes
            Dim lossSum As Double = 0
            Dim count As Integer = 0

            For Each sample As PerturbSeqSample In samples
                Dim xNorm As Double() = New Double(n - 1) {}
                Dim target As Double() = New Double(n - 1) {}

                For i As Integer = 0 To n - 1
                    Dim sd As Double = std.Max(controlSD(i), 0.000001)

                    xNorm(i) = (sample.ControlExpression(i) - controlMean(i)) / sd
                    target(i) = (sample.PerturbedExpression(i) - sample.ControlExpression(i)) / sd
                Next

                Dim pred As Tensor = model.Forward(xNorm, sample.PerturbationFlag(n))
                Dim targetTensor As Tensor = New Tensor(n, 1)

                Call System.Array.Copy(target, targetTensor.Data, n)

                lossSum += GNN.Loss.MeanSquaredError(pred, targetTensor)
                count += 1
            Next

            Return If(count > 0, lossSum / count, 0.0)
        End Function
    End Class
End Namespace
