' ============================================================================
' CompatibilityAudit.vb - 公开 API 零破坏验证
'
' 这里刻意只用**重构前就已经存在的**签名来调用三个模型，逐个方法覆盖。
' 一旦某次重构无意中改动了公开签名，本文件要么编译失败，要么运行时报错，
' 从而在合入前就暴露出来。
' ============================================================================

Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder
Imports std = System.Math

''' <summary>
''' 公开 API 零破坏验证。
''' </summary>
Public Module CompatibilityAudit

    Public Sub Run()
        Call AuditSupport.RunCase("compat.gmm", AddressOf LegacyGmm)
        Call AuditSupport.RunCase("compat.gmvae", AddressOf LegacyGmvae)
        Call AuditSupport.RunCase("compat.graphvae", AddressOf LegacyGraphVae)
        Call AuditSupport.RunCase("compat.graph-utils", AddressOf LegacyGraphUtils)
        Call AuditSupport.RunCase("compat.layers", AddressOf LegacyLayers)
    End Sub

#Region "GMM"

    Private Sub LegacyGmm()
        Dim X As Tensor = Nothing
        Dim labels As Integer() = Nothing
        Call AuditSupport.GaussianBlobs(2, 50, 3, 0.5, 11, X, labels)

        ' 完全沿用重构前的构造签名
        Dim model As New GaussianMixtureModel(2, 100, 0.000001, 0.000001, False, GaussianMixtureModel.InitMethod.KMeansPP, 11)

        Call model.Fit(X)

        ' 全部公开成员逐个调用
        Dim probabilities As Tensor = model.PredictProba(X)
        Dim clusterLabels As Integer() = model.Predict(X)
        Dim averageLogLikelihood As Double = model.Score(X)
        Dim logDensity As Tensor = model.ScoreSamples(X)
        Dim generated As Tensor = model.Sample(10, 5)
        Dim mean As Tensor = model.GetComponentMean(0)
        Dim variance As Tensor = model.GetComponentVariance(0)
        Dim deviation As Tensor = model.GetComponentStdDev(0)

        Call DebugAudit.Test(model.Weights IsNot Nothing, "Weights 必须可访问")
        Call DebugAudit.Test(model.Means IsNot Nothing, "Means 必须可访问")
        Call DebugAudit.Test(model.Covariances IsNot Nothing, "Covariances 必须可访问")
        Call DebugAudit.Test(model.Converged OrElse Not model.Converged, "Converged 必须可访问")
        Call DebugAudit.Test(model.NIter > 0, "NIter 必须可访问")
        Call DebugAudit.Test(model.LogLikelihoodHistory.Count > 0, "LogLikelihoodHistory 必须可访问")
        Call DebugAudit.Test(probabilities.Shape(0) = X.Shape(0), "PredictProba 形状不符")
        Call DebugAudit.Test(clusterLabels.Length = X.Shape(0), "Predict 长度不符")
        Call DebugAudit.Test(AuditSupport.IsFinite(averageLogLikelihood), "Score 必须有限")
        Call DebugAudit.Test(logDensity.Length = X.Shape(0), "ScoreSamples 长度不符")
        Call DebugAudit.Test(generated.Shape(0) = 10, "Sample 样本数不符")
        Call DebugAudit.Test(mean.Length = 3, "GetComponentMean 长度不符")
        Call DebugAudit.Test(variance.Length = 3, "GetComponentVariance 长度不符")
        Call DebugAudit.Test(deviation.Length = 3, "GetComponentStdDev 长度不符")
        Call DebugAudit.Test(model.ToString().StartsWith("GMM("), "ToString 前缀不符")

        ' 静态线性代数门面
        Dim inverse As Tensor = GaussianMixtureModel.MatrixInverse(
            New Tensor(New Double(,) {{2.0, 0.0}, {0.0, 4.0}}), 2)
        Dim logDet As Double = GaussianMixtureModel.LogDeterminant(
            New Tensor(New Double(,) {{2.0, 0.0}, {0.0, 4.0}}), 2)

        Call DebugAudit.Test(std.Abs(inverse(0, 0) - 0.5) < 1.0E-12, "MatrixInverse 结果不符")
        Call DebugAudit.Test(std.Abs(logDet - std.Log(8.0)) < 1.0E-12, "LogDeterminant 结果不符")

        ' 静态工厂
        Dim sampleCount As Integer = X.Shape(0)
        Dim flat As Double() = New Double(sampleCount * 3 - 1) {}
        Array.Copy(X.Data, flat, flat.Length)

        Dim built As GaussianMixtureModel = GaussianMixtureModel.Predicts(flat, 2, 0.000001, False)
        Call DebugAudit.Test(AuditSupport.IsFinite(built.LogLikelihood), "Predicts 重载必须可用")
    End Sub

#End Region

#Region "GMVAE"

    Private Sub LegacyGmvae()
        Dim X As Tensor = Nothing
        Dim labels As Integer() = Nothing
        Call AuditSupport.GaussianBlobs(2, 40, 3, 0.4, 12, X, labels)

        ' 完全沿用重构前的构造签名
        Dim model As New GMVAE(3, 3, 2, 16, 12)

        ' 7 参数旧签名
        Call model.Fit(X, 30, 16, 0.005, 1.0, 0.5, False)

        ' 公开的 Forward（接收 Random）
        Dim yLogits As Tensor = Nothing
        Dim zMean As Tensor = Nothing
        Dim zLogVar As Tensor = Nothing
        Dim zSample As Tensor = Nothing
        Dim xRecon As Tensor = Nothing

        Call model.Forward(X, New Random(1), yLogits, zMean, zLogVar, zSample, xRecon)

        Call DebugAudit.Test(yLogits IsNot Nothing, "Forward 的 yLogits 输出为空")
        Call DebugAudit.Test(zMean.Shape(1) = 3, "Forward 的 zMean 维度不符")
        Call DebugAudit.Test(zLogVar.Shape(1) = 3, "Forward 的 zLogVar 维度不符")
        Call DebugAudit.Test(zSample.Shape(1) = 3, "Forward 的 zSample 维度不符")
        Call DebugAudit.Test(xRecon.Shape(1) = 3, "Forward 的 xRecon 维度不符")

        ' Encode / Reconstruct / Generate / PredictClusters
        Dim yProba As Tensor = Nothing
        Dim meanValues As Tensor = Nothing
        Dim logVariances As Tensor = Nothing

        Call model.Encode(X, yProba, meanValues, logVariances)

        Dim reconstructed As Tensor = model.Reconstruct(X, True)
        Dim sampled As Tensor = model.Reconstruct(X, False)
        Dim generated As Tensor = model.Generate(5, 0, 3)
        Dim mixedGenerated As Tensor = model.Generate(5)
        Dim clusters As Integer() = model.PredictClusters(X)

        Call DebugAudit.Test(model.InputDim = 3, "InputDim 不符")
        Call DebugAudit.Test(model.LatentDim = 3, "LatentDim 不符")
        Call DebugAudit.Test(model.NComponents = 2, "NComponents 不符")
        Call DebugAudit.Test(model.LossHistory.Count = 30, $"LossHistory 应有 30 条，实际 {model.LossHistory.Count}")
        Call DebugAudit.Test(model.ToString().StartsWith("GMVAE("), "ToString 前缀不符")

        Call DebugAudit.Test(yProba.Shape(1) = 2, "Encode 的 yProba 列数不符")
        Call DebugAudit.Test(AuditSupport.AllFinite(reconstructed), "Reconstruct 必须有限")
        Call DebugAudit.Test(AuditSupport.AllFinite(sampled), "Reconstruct(采样式) 必须有限")
        Call DebugAudit.Test(generated.Shape(0) = 5, "Generate 样本数不符")
        Call DebugAudit.Test(mixedGenerated.Shape(0) = 5, "Generate(默认参数) 样本数不符")
        Call DebugAudit.Test(clusters.Length = X.Shape(0), "PredictClusters 长度不符")
    End Sub

#End Region

#Region "GraphVAE"

    Private Sub LegacyGraphVae()
        Dim nodes As Integer = 6
        Dim config As New GraphVAEConfig With {
            .NumNodes = nodes,
            .InputDim = nodes,
            .Hidden1 = 8,
            .Hidden2 = 4,
            .LatentDim = 3,
            .DecHidden = 4
        }

        Dim adjacency As Double(,) = New Double(nodes - 1, nodes - 1) {}

        For i As Integer = 0 To nodes - 1
            adjacency(i, (i + 1) Mod nodes) = 1.0
            adjacency((i + 1) Mod nodes, i) = 1.0
        Next

        Dim adj As New Tensor(adjacency)
        Dim features As Tensor = Tensor.Identity(nodes)

        Dim model As New GraphVAE(config, 7)

        ' 公开的 Forward / ComputeLoss / Backward / Update
        Dim result As Tuple(Of Tensor, Tensor, Tensor, Tensor, Tensor) = model.Forward(adj, features)
        Dim lossValue As Double = model.ComputeLoss(result.Item1, adj, result.Item2, features,
                                                    result.Item3, result.Item4, 1.0F)

        Call model.Backward(adj, features, result.Item1, result.Item2,
                            result.Item3, result.Item4, result.Item5, 1.0F)
        Call model.Update(0.01F)
        Call model.Update(0.01F, 0.9F, 0.999F, 0.00000001F)

        Call DebugAudit.Test(AuditSupport.IsFinite(lossValue), "ComputeLoss 必须有限")

        ' 旧版 Train 签名
        Dim graphs As New List(Of Tuple(Of Tensor, Tensor)) From {Tuple.Create(adj, features)}
        Dim losses As List(Of Double) = model.Train(graphs, 5, 0.01F, 1.0F, False)

        Call DebugAudit.Test(losses.Count = 5, $"Train 应返回 5 轮损失，实际 {losses.Count}")

        ' Predict / Generate / Evaluate
        Dim predicted As Tuple(Of Tensor, Tensor) = model.Predict(adj)
        Dim predictedWithFeatures As Tuple(Of Tensor, Tensor) = model.Predict(adj, features)
        Dim generated As Tuple(Of Tensor, Tensor) = model.Generate()
        Dim evaluation As Tuple(Of Double, Double, Double) = model.Evaluate(adj)

        Call DebugAudit.Test(predicted.Item1.Shape(0) = nodes, "Predict 邻接形状不符")
        Call DebugAudit.Test(predictedWithFeatures.Item2.Shape(0) = nodes, "Predict 特征形状不符")
        Call DebugAudit.Test(generated.Item1.Shape(0) = nodes, "Generate 邻接形状不符")
        Call DebugAudit.Test(AuditSupport.IsFinite(evaluation.Item1), "Evaluate 损失必须有限")
        ' 重构前 GraphVAE 没有重写 ToString，这里只验证它仍然可调用
        Call DebugAudit.Test(Not String.IsNullOrEmpty(model.ToString()), "ToString 必须可调用")
    End Sub

#End Region

#Region "GraphUtils 与层"

    Private Sub LegacyGraphUtils()
        Dim adjacency As New Tensor(New Double(,) {{0.0, 1.0}, {1.0, 0.0}})
        Dim normalized As Tensor = GraphUtils.NormalizeAdjacency(adjacency)

        Call DebugAudit.Test(normalized.Rank = 2, "NormalizeAdjacency 形状不符")

        Dim value As Double = GraphUtils.Sigmoid(0.0)
        Call DebugAudit.Test(std.Abs(value - 0.5) < 1.0E-12, "Sigmoid(0) 应为 0.5")

        Dim t As New Tensor(New Double() {-1.0, 0.0, 2.0}, 3)
        Dim activated As Tensor = GraphUtils.ApplyReLU(t)
        Dim probabilities As Tensor = GraphUtils.ApplySigmoid(t)

        Call DebugAudit.Test(activated.Data(0) = 0, "ApplyReLU 应把负数截断为 0")
        Call DebugAudit.Test(AuditSupport.AllFinite(probabilities), "ApplySigmoid 必须有限")

        Dim gradient As Tensor = GraphUtils.ReLUDerivFromInput(t, New Tensor(New Double() {1.0, 1.0, 1.0}, 3))
        Call DebugAudit.Test(gradient.Data(0) = 0 AndAlso gradient.Data(2) = 1, "ReLUDerivFromInput 结果不符")

        Dim bce As Double = GraphUtils.BCELossSum(probabilities, New Tensor(New Double() {0.0, 0.0, 1.0}, 3))
        Call DebugAudit.Test(AuditSupport.IsFinite(bce), "BCELossSum 必须有限")

        Dim kl As Double = GraphUtils.KLDivergence(New Tensor(New Double() {0.0}, 1), New Tensor(New Double() {0.0}, 1))
        Call DebugAudit.Test(std.Abs(kl) < 1.0E-12, "μ=0, logσ²=0 时 KL 应为 0")

        Dim biased As Tensor = GraphUtils.AddBias(
            New Tensor(New Double() {1.0, 2.0}, 1, 2), New Tensor(New Double() {10.0, 20.0}, 1, 2))
        Call DebugAudit.Test(biased.Data(0) = 11 AndAlso biased.Data(1) = 22, "AddBias 结果不符")

        Dim broadcast As Tensor = GraphUtils.BroadcastRow(New Tensor(New Double() {1.0, 2.0}, 1, 2), 3)
        Call DebugAudit.Test(broadcast.Shape(0) = 3, "BroadcastRow 形状不符")

        Dim thresholded As Tensor = GraphUtils.Threshold(probabilities, 0.5)
        Call DebugAudit.Test(thresholded.Length = 3, "Threshold 长度不符")

        Dim accuracy As Double = GraphUtils.BinaryAccuracy(thresholded, New Tensor(New Double() {0.0, 0.5, 1.0}, 3), 0.5)
        Call DebugAudit.Test(accuracy >= 0 AndAlso accuracy <= 1, "BinaryAccuracy 应落在 [0,1]")
    End Sub

    Private Sub LegacyLayers()
        Dim layer As New LinearLayer(3, 2, 5)
        Dim input As New Tensor(New Double() {1.0, 2.0, 3.0, 4.0, 5.0, 6.0}, 2, 3)
        Dim output As Tensor = layer.Forward(input)

        Call DebugAudit.Test(output.Shape(0) = 2 AndAlso output.Shape(1) = 2, "LinearLayer.Forward 形状不符")

        Dim dInput As Tensor = layer.Backward(New Tensor(New Double() {1.0, 1.0, 1.0, 1.0}, 2, 2))
        Call DebugAudit.Test(dInput.Shape(0) = 2 AndAlso dInput.Shape(1) = 3, "LinearLayer.Backward 形状不符")
        Call DebugAudit.Test(layer.WeightGrad IsNot Nothing, "WeightGrad 必须可访问")
        Call DebugAudit.Test(layer.BiasGrad IsNot Nothing, "BiasGrad 必须可访问")

        Call layer.Update(0.01F, 0.9F, 0.999F, 0.00000001F)

        Dim gcn As New GCNLayer(3, 2, 6)
        Dim normalized As Tensor = GraphUtils.NormalizeAdjacency(New Tensor(New Double(,) {{0.0, 1.0}, {1.0, 0.0}}))
        Dim gcnOutput As Tensor = gcn.Forward(normalized, input)

        Call DebugAudit.Test(gcnOutput.Shape(0) = 2 AndAlso gcnOutput.Shape(1) = 2, "GCNLayer.Forward 形状不符")

        Dim gcnGradient As Tensor = gcn.Backward(New Tensor(New Double() {1.0, 1.0, 1.0, 1.0}, 2, 2))
        Call DebugAudit.Test(gcnGradient.Shape(0) = 2 AndAlso gcnGradient.Shape(1) = 3, "GCNLayer.Backward 形状不符")

        Call gcn.Update(0.01F, 0.9F, 0.999F, 0.00000001F)
        Call DebugAudit.Test(gcn.WeightGrad IsNot Nothing, "GCNLayer.WeightGrad 必须可访问")
    End Sub

#End Region

End Module
