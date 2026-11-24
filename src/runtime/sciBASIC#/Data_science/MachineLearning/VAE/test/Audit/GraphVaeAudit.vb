' ============================================================================
' GraphVaeAudit.vb - 图变分自编码器的生产级审计
'
' 覆盖：持久化往返、变长图掩码训练、稀疏邻接与稠密归一化的一致性、
'       输入校验、TrainingOptions 重载（KL 退火 / 早停）。
' ============================================================================

Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.Core
Imports std = System.Math

''' <summary>
''' 图变分自编码器生产级审计。
''' </summary>
Public Module GraphVaeAudit

    Public Sub Run()
        Call AuditSupport.RunCase("graphvae.persistence", AddressOf ModelMustRoundTrip)
        Call AuditSupport.RunCase("graphvae.masked-training", AddressOf MaskedTrainingMustWork)
        Call AuditSupport.RunCase("graphvae.sparse-equivalence", AddressOf SparseMustMatchDense)
        Call AuditSupport.RunCase("graphvae.validation", AddressOf InputValidationMustThrow)
        Call AuditSupport.RunCase("graphvae.training-options", AddressOf TrainingOptionsMustWork)
    End Sub

#Region "工具"

    Private Function RingGraph(n As Integer) As Tensor
        Dim data As Double(,) = New Double(n - 1, n - 1) {}

        For i As Integer = 0 To n - 1
            data(i, (i + 1) Mod n) = 1.0
            data((i + 1) Mod n, i) = 1.0
        Next

        Return New Tensor(data)
    End Function

    Private Function SampleGraph(numNodes As Integer, inputDim As Integer, seed As Integer) As Tuple(Of Tensor, Tensor)
        Dim rng As New Random(seed)
        Dim adjacency As Double(,) = New Double(numNodes - 1, numNodes - 1) {}

        For i As Integer = 0 To numNodes - 1
            For j As Integer = i + 1 To numNodes - 1
                If rng.NextDouble() < 0.4 Then
                    adjacency(i, j) = 1.0
                    adjacency(j, i) = 1.0
                End If
            Next
        Next

        Dim features As Double() = New Double(numNodes * inputDim - 1) {}

        For i As Integer = 0 To features.Length - 1
            features(i) = rng.NextDouble()
        Next

        Return Tuple.Create(New Tensor(adjacency), New Tensor(features, numNodes, inputDim))
    End Function

    Private Function MakeConfig(numNodes As Integer, inputDim As Integer) As GraphVAEConfig
        Return New GraphVAEConfig With {
            .NumNodes = numNodes,
            .InputDim = inputDim,
            .Hidden1 = 10,
            .Hidden2 = 5,
            .LatentDim = 3,
            .DecHidden = 5
        }
    End Function

#End Region

#Region "持久化"

    Private Sub ModelMustRoundTrip()
        Dim graph As Tuple(Of Tensor, Tensor) = SampleGraph(7, 7, 1234)
        Dim model As New GraphVAE(MakeConfig(7, 7), seed:=1234)

        Dim graphs As New List(Of Tuple(Of Tensor, Tensor)) From {graph}
        Call model.Train(graphs, epochs:=15, lr:=0.01F, beta:=1.0F, verbose:=False)

        Dim path As String = IO.Path.Combine(AppContext.BaseDirectory, "graphvae-audit-model.bin")
        Call model.Save(path)

        Dim restored As New GraphVAE(MakeConfig(7, 7), seed:=1234)
        Dim errorMessage As String = Nothing

        Call DebugAudit.Test(restored.TryLoad(path, errorMessage), $"模型加载失败: {errorMessage}")
        Call DebugAudit.Test(restored.IsFitted, "还原后的模型应处于已训练状态")

        Dim expected As Tuple(Of Tensor, Tensor) = model.Predict(graph.Item1, graph.Item2)
        Dim actual As Tuple(Of Tensor, Tensor) = restored.Predict(graph.Item1, graph.Item2)

        Dim adjacencyError As Double = AuditSupport.MaxAbsDiff(expected.Item1, actual.Item1)
        Dim featureError As Double = AuditSupport.MaxAbsDiff(expected.Item2, actual.Item2)

        DebugAudit.Capture("graphvae.persistence.adj-error", adjacencyError)

        Call DebugAudit.Test(adjacencyError < 1.0E-12,
                             $"还原后的邻接重构应逐元素一致，最大偏差 {adjacencyError}")
        Call DebugAudit.Test(featureError < 1.0E-12,
                             $"还原后的特征重构应逐元素一致，最大偏差 {featureError}")

        If IO.File.Exists(path) Then
            IO.File.Delete(path)
        End If
    End Sub

#End Region

#Region "变长图掩码"

    Private Sub MaskedTrainingMustWork()
        ' 8 个节点的容量，但前 5 个为有效节点，其余为填充
        Dim graph As Tuple(Of Tensor, Tensor) = SampleGraph(8, 8, 2024)
        Dim model As New GraphVAE(MakeConfig(8, 8), seed:=2024)

        Dim graphs As New List(Of Tuple(Of Tensor, Tensor, Integer)) From {
            Tuple.Create(graph.Item1, graph.Item2, 5)
        }

        Dim options As New TrainingOptions With {
            .Epochs = 20,
            .LearningRate = 0.01,
            .Beta = 1.0,
            .Verbose = False
        }

        Dim losses As List(Of Double) = model.TrainMasked(graphs, options)

        Call DebugAudit.Test(losses.Count = 20, $"应完成 20 轮训练，实际 {losses.Count}")
        Call DebugAudit.Test(AuditSupport.AllFinite(losses), "掩码训练的损失必须全部为有限值")
        Call DebugAudit.Test(losses(losses.Count - 1) < losses(0),
                             $"掩码训练应降低损失: {losses(0):F6} -> {losses(losses.Count - 1):F6}")

        ' 无效节点数必须被拒绝
        Call DebugAudit.CheckThrows(Of ArgumentOutOfRangeException)(
            Sub() Call model.ForwardMasked(graph.Item1, graph.Item2, 0),
            "有效节点数为 0 时必须抛出 ArgumentOutOfRangeException")
    End Sub

#End Region

#Region "稀疏邻接"

    Private Sub SparseMustMatchDense()
        Dim adjacency As Tensor = RingGraph(6)
        Dim sparse As SparseAdjacency = SparseAdjacency.FromDense(adjacency)

        Call DebugAudit.Test(sparse.NumNodes = 6, $"节点数应为 6，实际 {sparse.NumNodes}")
        Call DebugAudit.Test(sparse.NonZeroCount = 12, $"环形图应有 12 条有向边，实际 {sparse.NonZeroCount}")

        ' 归一化结果必须与稠密实现完全一致
        Dim denseNormalized As Tensor = GraphUtils.NormalizeAdjacency(adjacency)
        Dim sparseNormalized As SparseAdjacency = sparse.NormalizedWithSelfLoops()
        Dim rebuilt As Tensor = sparseNormalized.ToDense()

        Dim denseError As Double = AuditSupport.MaxAbsDiff(rebuilt, denseNormalized)

        DebugAudit.Capture("graphvae.sparse.normalize-error", denseError)

        Call DebugAudit.Test(denseError < 1.0E-12,
                             $"稀疏归一化应与稠密实现一致，最大偏差 {denseError}")

        ' 稀疏乘法应与稠密乘法一致
        Dim features As Tuple(Of Tensor, Tensor) = SampleGraph(6, 4, 4321)
        Dim nodeFeatures As Tensor = features.Item2

        Dim denseProduct As Tensor = denseNormalized.MatMul(nodeFeatures)
        Dim sparseProduct As Tensor = sparseNormalized.Multiply(nodeFeatures)

        Dim productError As Double = AuditSupport.MaxAbsDiff(denseProduct, sparseProduct)

        Call DebugAudit.Capture("graphvae.sparse.multiply-error", productError)

        Call DebugAudit.Test(productError < 1.0E-12,
                             $"稀疏乘法应与稠密乘法一致，最大偏差 {productError}")

        ' 转置乘法一致性
        Dim transposeError As Double = AuditSupport.MaxAbsDiff(
            denseNormalized.Transpose().MatMul(nodeFeatures),
            sparseNormalized.TransposeMultiply(nodeFeatures))

        Call DebugAudit.Test(transposeError < 1.0E-12,
                             $"稀疏转置乘法应与稠密一致，最大偏差 {transposeError}")
    End Sub

#End Region

#Region "校验与训练选项"

    Private Sub InputValidationMustThrow()
        Dim model As New GraphVAE(MakeConfig(5, 5), seed:=99)
        Dim graph As Tuple(Of Tensor, Tensor) = SampleGraph(5, 5, 99)

        ' 节点数不匹配
        Dim wrong As Tuple(Of Tensor, Tensor) = SampleGraph(6, 5, 98)

        Call DebugAudit.CheckThrows(Of ArgumentException)(
            Sub() Call model.Forward(wrong.Item1, wrong.Item2),
            "邻接矩阵节点数与配置不符时必须抛出 ArgumentException")

        ' 特征行数与节点数不符
        Call DebugAudit.CheckThrows(Of ArgumentException)(
            Sub() Call model.Forward(graph.Item1, Tensor.Identity(4)),
            "特征行数与节点数不符时必须抛出 ArgumentException")
    End Sub

    Private Sub TrainingOptionsMustWork()
        Dim graph As Tuple(Of Tensor, Tensor) = SampleGraph(6, 6, 5678)
        Dim model As New GraphVAE(MakeConfig(6, 6), seed:=5678)

        Dim options As New TrainingOptions With {
            .Epochs = 80,
            .LearningRate = 0.01,
            .Beta = 1.0,
            .KlWarmupEpochs = 10,
            .GradientClip = 1.0,
            .Patience = 5,
            .MinDelta = 1.0E-6,
            .Verbose = False
        }

        Dim graphs As New List(Of Tuple(Of Tensor, Tensor)) From {graph}
        Dim losses As List(Of Double) = model.Train(graphs, options)

        Call DebugAudit.Test(losses.Count > 0, "应至少完成一轮训练")
        Call DebugAudit.Test(AuditSupport.AllFinite(losses), "损失必须全部为有限值")
        Call DebugAudit.Test(model.IsFitted, "训练完成后 IsFitted 应为 True")

        ' 开启早停后轮数不应超过上限
        Call DebugAudit.Test(losses.Count <= 80, $"训练轮数不得超过 80，实际 {losses.Count}")

        DebugAudit.Capture("graphvae.options.epochs", losses.Count)
    End Sub

#End Region

End Module
