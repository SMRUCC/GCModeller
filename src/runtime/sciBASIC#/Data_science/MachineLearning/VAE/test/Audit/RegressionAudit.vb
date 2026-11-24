' ============================================================================
' RegressionAudit.vb - 已确认缺陷的回归用例
'
' 这些用例在重构之前应当失败，重构之后必须全部通过。它们把「原本错在哪」固化为
' 可执行的事实，防止后续改动悄悄退回旧行为。
'
' 覆盖的三个缺陷：
'   1. GMM.vb L246-251   权重初始化 wData(K) = 1.0 / K 的循环变量遮蔽
'                        → 首次迭代 1.0/0 = Infinity → E 步全 NaN
'   2. GMVAE.vb L555-565 类别隐变量 q(y|x) 只接收 KL(q(y|x)||p(y)) 的梯度，
'                        缺少 ∂KL(z)/∂q(y|x) 项 → 聚类无信息
'   3. GraphVAE.vb L986-1010 Evaluate 通过 Forward 隐式采样 ε → 评估不可复现
' ============================================================================

Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder
Imports std = System.Math

''' <summary>
''' 已确认缺陷的回归审计。
''' </summary>
Public Module RegressionAudit

    Public Sub Run()
        Call AuditSupport.RunCase("gmm.weight-init-finite", AddressOf GmmWeightInitMustBeFinite)
        Call AuditSupport.RunCase("gmm.det-and-inverse-consistency", AddressOf GmmDeterminantMustMatchReference)
        Call AuditSupport.RunCase("gmvae.categorical-signal", AddressOf GmvaeCategoricalMustCarryClusterSignal)
        Call AuditSupport.RunCase("gmvae.loss-finite", AddressOf GmvaeLossMustBeFinite)
        Call AuditSupport.RunCase("graphvae.evaluate-deterministic", AddressOf GraphVaeEvaluateMustBeDeterministic)
        Call AuditSupport.RunCase("graphvae.kl-gradient-reference", AddressOf GraphVaeKlGradientMatchesReference)
    End Sub

#Region "GMM"

    ''' <summary>
    ''' GMM 的混合权重必须是有限值并且和为 1；由权重派生的均值与协方差也必须有限。
    ''' 重构前：<c>wData(K) = 1.0 / K</c> 在 K=0 时产生 <c>Double.PositiveInfinity</c>，
    ''' 导致整条 EM 链路静默产出 NaN。
    ''' </summary>
    Private Sub GmmWeightInitMustBeFinite()
        Dim X As Tensor = Nothing
        Dim labels As Integer() = Nothing
        Call AuditSupport.GaussianBlobs(3, 120, 4, 0.5, 7, X, labels)

        Dim model As New GaussianMixtureModel(nComponents:=3, maxIter:=60,
                                              tol:=0.0000001, regCovar:=0.000001,
                                              diagCovariance:=False, seed:=7)
        Call model.Fit(X)

        DebugAudit.Capture("gmm.weights", model.Weights)
        DebugAudit.Capture("gmm.logLik", model.LogLikelihood)

        Call DebugAudit.Test(AuditSupport.AllFinite(model.Weights),
                             $"GMM 混合权重必须全部为有限值 (weights[0]={model.Weights.Data(0)})")
        Call DebugAudit.Test(AuditSupport.AllFinite(model.Means), "GMM 成分均值必须全部为有限值")
        Call DebugAudit.Test(AuditSupport.AllFinite(model.Covariances), "GMM 协方差必须全部为有限值")

        Dim total As Double = AuditSupport.SumOf(model.Weights)
        Call DebugAudit.Test(std.Abs(total - 1.0) < 0.000001,
                             $"GMM 混合权重之和必须为 1，实际为 {total}")

        Call DebugAudit.Test(AuditSupport.IsFinite(model.LogLikelihood),
                             $"GMM 对数似然必须为有限值，实际为 {model.LogLikelihood}")
        Call DebugAudit.Test(AuditSupport.AllFinite(model.LogLikelihoodHistory),
                             "GMM 对数似然历史必须全部为有限值")

        ' 收敛性：三个可分离高斯簇应当被准确还原
        Dim predicted As Integer() = model.Predict(X)
        Dim accuracy As Double = AuditSupport.ClusterAccuracy(predicted, labels, 3)
        Call DebugAudit.Capture("gmm.accuracy", accuracy)
        Call DebugAudit.Test(accuracy > 0.95,
                             $"GMM 应能准确还原 3 个可分离高斯簇，实际准确率 {accuracy:P2}")
    End Sub

    ''' <summary>
    ''' <see cref="GaussianMixtureModel.LogDeterminant"/> 必须等同于「特征值之积取对数」。
    ''' 重构前：L807 的 <c>logDet = -logDet</c> 在处理行交换时取负，
    ''' 而累计的是 <c>log|det|</c>，行交换本不影响结果。
    ''' </summary>
    Private Sub GmmDeterminantMustMatchReference()
        ' 对角占优、需要行交换的对称正定矩阵
        Dim data As Double(,) = {
            {4.0, 1.0, 0.5},
            {1.0, 3.0, 0.25},
            {0.5, 0.25, 2.0}
        }

        Dim a As New Tensor(data)

        Dim actual As Double = GaussianMixtureModel.LogDeterminant(a, 3)

        ' 参考值：用 Cholesky 分解求 ln|A| = 2 * Σ ln L_ii
        Dim reference As Double = ReferenceLogDet(data)
        Dim errorValue As Double = std.Abs(actual - reference)

        DebugAudit.Capture("gmm.logDet", actual)
        DebugAudit.Capture("gmm.logDet.reference", reference)

        Call DebugAudit.Test(errorValue < 0.000001,
                             $"GMM.LogDeterminant 与 Cholesky 参考值不符: {actual} vs {reference}")

        ' 再取一个中途需要行交换的 3x3 矩阵：列主元会在 t=1 才触发，
        ' 此时 logDet 已经累积了 log(2)，重构前的 logDet = -logDet 会翻转符号
        Dim swapped As Double(,) = {
            {2.0, 0.0, 0.0},
            {0.0, 1.0E-9, 1.0},
            {0.0, 1.0, 1.0}
        }

        Dim swappedTensor As New Tensor(swapped)
        Dim swappedLogDet As Double = GaussianMixtureModel.LogDeterminant(swappedTensor, 3)

        ' det = 2 * (1e-9*1 - 1*1) = -2，故 ln|det| = ln2
        Call DebugAudit.Test(std.Abs(swappedLogDet - std.Log(2.0)) < 0.000001,
                             $"行交换情形下的 ln|det| 应为 {std.Log(2.0)}，实际为 {swappedLogDet}")
    End Sub

    Private Function ReferenceLogDet(a As Double(,)) As Double
        Dim n As Integer = a.GetLength(0)
        Dim l As Double(,) = New Double(n - 1, n - 1) {}

        For i As Integer = 0 To n - 1
            For j As Integer = 0 To i
                Dim acc As Double = a(i, j)

                For k As Integer = 0 To j - 1
                    acc -= l(i, k) * l(j, k)
                Next

                If i = j Then
                    l(i, i) = std.Sqrt(acc)
                Else
                    l(i, j) = acc / l(j, j)
                End If
            Next
        Next

        Dim acc2 As Double = 0

        For i As Integer = 0 To n - 1
            acc2 += std.Log(l(i, i))
        Next

        Return 2.0 * acc2
    End Function

#End Region

#Region "GMVAE"

    ''' <summary>
    ''' 在三个分离良好的高斯簇上，类别隐变量必须真正携带聚类信号。
    ''' 重构前：<c>_encY_*</c> 只从 <c>KL(q(y|x)||p(y))</c> 获得梯度，而 <c>p(y)</c> 又被
    ''' 学习成 <c>mean(q(y|x))</c>，使 q 恒为近均匀分布 → 聚类退化为常数标签。
    ''' </summary>
    Private Sub GmvaeCategoricalMustCarryClusterSignal()
        Dim X As Tensor = Nothing
        Dim labels As Integer() = Nothing
        Call AuditSupport.GaussianBlobs(3, 100, 4, 0.4, 11, X, labels)

        Dim model As New GMVAE(inputDim:=4, latentDim:=4, nComponents:=3,
                               hiddenDim:=32, seed:=11)
        Call model.Fit(X, epochs:=150, batchSize:=32, learningRate:=0.005,
                       beta:=1.0, gamma:=0.5, verbose:=False)

        Dim predicted As Integer() = model.PredictClusters(X)
        Dim accuracy As Double = AuditSupport.ClusterAccuracy(predicted, labels, 3)

        DebugAudit.Capture("gmvae.accuracy", accuracy)

        Dim uniqueCount As Integer = 0
        Dim seen As New HashSet(Of Integer)

        For Each value As Integer In predicted
            If seen.Add(value) Then
                uniqueCount += 1
            End If
        Next

        Call DebugAudit.Test(uniqueCount > 1,
                             $"GMVAE 的预测标签不应退化为单一类别（实际仅出现 {uniqueCount} 个类别）")
        Call DebugAudit.Test(accuracy > 0.85,
                             $"GMVAE 类别隐变量应携带聚类信号，实际准确率 {accuracy:P2}")

        ' 条件生成：指定成分生成的样本应聚拢在对应簇心附近
        Dim generated As Tensor = model.Generate(30, componentIdx:=0, seed:=101)
        Call DebugAudit.Test(AuditSupport.AllFinite(generated), "GMVAE 条件生成的样本必须为有限值")
    End Sub

    ''' <summary>
    ''' GMVAE 的训练损失必须全程有限。重构前：先验方差梯度的符号错误会造成
    ''' <c>var2 → 0</c> 的正反馈坍缩，最终使 <c>1/var2</c> 爆炸为 NaN。
    ''' </summary>
    Private Sub GmvaeLossMustBeFinite()
        Dim X As Tensor = Nothing
        Dim labels As Integer() = Nothing
        Call AuditSupport.GaussianBlobs(2, 60, 3, 0.5, 5, X, labels)

        Dim model As New GMVAE(inputDim:=3, latentDim:=3, nComponents:=2,
                               hiddenDim:=24, seed:=5)
        Call model.Fit(X, epochs:=40, batchSize:=16, learningRate:=0.003,
                       beta:=1.0, gamma:=1.0, verbose:=False)

        DebugAudit.Capture("gmvae.loss.first", model.LossHistory(0))
        DebugAudit.Capture("gmvae.loss.last", model.LossHistory(model.LossHistory.Count - 1))

        Call DebugAudit.Test(AuditSupport.AllFinite(model.LossHistory),
                             "GMVAE 训练损失历史必须全部为有限值")

        Dim reconstructed As Tensor = model.Reconstruct(X, useMean:=True)
        Call DebugAudit.Test(AuditSupport.AllFinite(reconstructed), "GMVAE 重构结果必须为有限值")
    End Sub

#End Region

#Region "GraphVAE"

    ''' <summary>
    ''' 同一个模型、同一份输入，评估必须得到完全相同的结果。
    ''' 重构前：<c>Evaluate</c> 调用 <c>Forward</c>，而 <c>Forward</c> 内部采样 ε，
    ''' 使评估值随调用次数波动。
    ''' </summary>
    Private Sub GraphVaeEvaluateMustBeDeterministic()
        Dim config As New GraphVAEConfig With {
            .NumNodes = 6,
            .InputDim = 6,
            .Hidden1 = 12,
            .Hidden2 = 6,
            .LatentDim = 3,
            .DecHidden = 6
        }

        Dim adj As Tensor = RingGraph(6, 1.0)
        Dim features As Tensor = Tensor.Identity(6)

        Dim model As New GraphVAE(config, seed:=42)

        Dim graphs As New List(Of Tuple(Of Tensor, Tensor))
        graphs.Add(Tuple.Create(adj, features))

        Call model.Train(graphs, epochs:=10, lr:=0.01F, beta:=1.0F, verbose:=False)

        Dim first As Tuple(Of Double, Double, Double) = model.Evaluate(adj, features)
        Dim second As Tuple(Of Double, Double, Double) = model.Evaluate(adj, features)
        Dim third As Tuple(Of Double, Double, Double) = model.Evaluate(adj, features)

        DebugAudit.Capture("graphvae.eval.loss", first.Item1)
        DebugAudit.Capture("graphvae.eval.acc", first.Item2)

        Call DebugAudit.Test(std.Abs(first.Item1 - second.Item1) < 1.0E-12,
                             $"GraphVAE 评估损失应可复现: {first.Item1} vs {second.Item1}")
        Call DebugAudit.Test(std.Abs(second.Item1 - third.Item1) < 1.0E-12,
                             $"GraphVAE 评估损失应可复现: {second.Item1} vs {third.Item1}")
        Call DebugAudit.Test(std.Abs(first.Item2 - second.Item2) < 1.0E-12,
                             $"GraphVAE 评估准确率应可复现: {first.Item2} vs {second.Item2}")
        Call DebugAudit.Test(AuditSupport.IsFinite(first.Item1),
                             $"GraphVAE 评估损失必须为有限值，实际为 {first.Item1}")
    End Sub

    ''' <summary>
    ''' <see cref="GraphUtils.KLDivergence"/> 的解析梯度必须与中心差分一致。
    ''' 这个实现（GraphVAE.vb L798-806）是正确的，作为 GMVAE 梯度修复的参照标准。
    ''' </summary>
    Private Sub GraphVaeKlGradientMatchesReference()
        Dim muData As Double() = {0.3, -0.7, 1.1}
        Dim logVarData As Double() = {-0.5, 0.25, 0.8}

        Dim latent As Integer = muData.Length
        Dim epsilon As Double = 1.0E-6

        For i As Integer = 0 To latent - 1
            ' 解析梯度：KLDivergence 返回的是对 latent 维度的求和，
            ' 因此 dKL_sum/dmu = mu；除以 L 的缩放由调用方（Backward 的 klScale）施加
            Dim analyticMu As Double = muData(i)

            Dim muPlus As Double() = CType(muData.Clone(), Double())
            Dim muMinus As Double() = CType(muData.Clone(), Double())

            muPlus(i) += epsilon
            muMinus(i) -= epsilon

            Dim numericMu As Double =
                (GraphUtils.KLDivergence(New Tensor(muPlus, latent), New Tensor(logVarData, latent)) -
                 GraphUtils.KLDivergence(New Tensor(muMinus, latent), New Tensor(logVarData, latent))) / (2.0 * epsilon)

            Call DebugAudit.Test(std.Abs(analyticMu - numericMu) < 0.000001,
                                 $"GraphVAE 的 dKL/dmu 与差分不符: 解析值 {analyticMu}，数值值 {numericMu}")

            ' 解析梯度：dKL_sum/dlogVar = 0.5*(exp(logVar)-1)
            Dim analyticLogVar As Double = 0.5 * (std.Exp(logVarData(i)) - 1.0)

            Dim lvPlus As Double() = CType(logVarData.Clone(), Double())
            Dim lvMinus As Double() = CType(logVarData.Clone(), Double())

            lvPlus(i) += epsilon
            lvMinus(i) -= epsilon

            Dim numericLogVar As Double =
                (GraphUtils.KLDivergence(New Tensor(muData, latent), New Tensor(lvPlus, latent)) -
                 GraphUtils.KLDivergence(New Tensor(muData, latent), New Tensor(lvMinus, latent))) / (2.0 * epsilon)

            Call DebugAudit.Test(std.Abs(analyticLogVar - numericLogVar) < 0.000001,
                                 $"GraphVAE 的 dKL/dlogVar 与差分不符: 解析值 {analyticLogVar}，数值值 {numericLogVar}")
        Next
    End Sub

    Private Function RingGraph(n As Integer, weight As Double) As Tensor
        Dim data As Double(,) = New Double(n - 1, n - 1) {}

        For i As Integer = 0 To n - 1
            data(i, (i + 1) Mod n) = weight
            data((i + 1) Mod n, i) = weight
        Next

        Return New Tensor(data)
    End Function

#End Region

End Module
