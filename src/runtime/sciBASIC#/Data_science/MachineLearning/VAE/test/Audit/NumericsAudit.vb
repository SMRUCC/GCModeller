' ============================================================================
' NumericsAudit.vb - Core 基础设施的自检
'
' 覆盖：LogSumExp 的极值稳定性、Box-Muller 成对采样的矩与独立性、
'       RandomGaussian 的可复现性、Cholesky 分解与协方差还原、
'       马氏距离与显式求逆的一致性、ModelParameters 二进制往返、
'       Adam 的收敛性、BIC 公式。
' ============================================================================

Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.Core
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.Core.Layers
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.Core.Numerics
Imports std = System.Math

''' <summary>
''' Core 基础设施自检。
''' </summary>
Public Module NumericsAudit

    Public Sub Run()
        Call AuditSupport.RunCase("numerics.log-sum-exp", AddressOf LogSumExpMustBeStable)
        Call AuditSupport.RunCase("numerics.box-muller", AddressOf BoxMullerMustMatchMoments)
        Call AuditSupport.RunCase("numerics.random-reproducible", AddressOf RandomGaussianMustBeReproducible)
        Call AuditSupport.RunCase("numerics.cholesky", AddressOf CholeskyMustReconstructCovariance)
        Call AuditSupport.RunCase("numerics.mahalanobis", AddressOf MahalanobisMustMatchInverse)
        Call AuditSupport.RunCase("numerics.gaussian-sampling", AddressOf GaussianSamplingMustPreserveCorrelation)
        Call AuditSupport.RunCase("numerics.jitter-fallback", AddressOf CholeskyMustRecoverFromSingular)
        Call AuditSupport.RunCase("numerics.persistence", AddressOf ModelParametersMustRoundTrip)
        Call AuditSupport.RunCase("numerics.adam", AddressOf AdamMustMinimiseQuadratic)
        Call AuditSupport.RunCase("numerics.bic", AddressOf BicAndAicMustFollowFormula)
    End Sub

#Region "LogSumExp"

    Private Sub LogSumExpMustBeStable()
        Dim balanced As Double() = {0.0, 0.0}
        Call DebugAudit.Test(std.Abs(LogSumExp.Compute(balanced) - std.Log(2.0)) < 1.0E-12,
                             "ln(e^0 + e^0) 应等于 ln2")

        ' 极小的对数权重：朴素实现会下溢为 ln(0) = -Infinity
        Dim tiny As Double() = {-1000.0, -1000.0}
        Dim tinyResult As Double = LogSumExp.Compute(tiny)

        Call DebugAudit.Test(std.Abs(tinyResult - (-1000.0 + std.Log(2.0))) < 1.0E-9,
                             $"极小的对数权重必须保持精度，实际为 {tinyResult}")

        ' 全部为 -Infinity 时必须是 -Infinity，而不是 NaN
        Dim zeros As Double() = {Double.NegativeInfinity, Double.NegativeInfinity}
        Call DebugAudit.Test(Double.IsNegativeInfinity(LogSumExp.Compute(zeros)),
                             "全部为 -Infinity 时应返回 -Infinity 而非 NaN")

        ' 含 +Infinity 时必须是 +Infinity，而不是 NaN
        Dim infinite As Double() = {Double.PositiveInfinity, 0.0}
        Call DebugAudit.Test(Double.IsPositiveInfinity(LogSumExp.Compute(infinite)),
                             "含 +Infinity 时应返回 +Infinity 而非 NaN")

        ' 混合主导项：ln(e^10 + e^0) ≈ 10
        Dim dominated As Double() = {10.0, 0.0}
        Call DebugAudit.Test(std.Abs(LogSumExp.Compute(dominated) - 10.0000453999) < 1.0E-6,
                             "主导项情形下的 log-sum-exp 精度不足")

        ' log-softmax 的行归一性
        Dim logValues As Double(,) = {
            {1.0, 2.0, 3.0},
            {-5.0, 5.0, 0.0}
        }

        Dim softmaxValues As Double(,) = LogSumExp.SoftmaxRows(logValues)

        For i As Integer = 0 To 1
            Dim total As Double = softmaxValues(i, 0) + softmaxValues(i, 1) + softmaxValues(i, 2)

            Call DebugAudit.Test(std.Abs(total - 1.0) < 1.0E-12,
                                 $"softmax 第 {i} 行之和应为 1，实际为 {total}")
        Next
    End Sub

#End Region

#Region "Box-Muller 与随机数服务"

    Private Sub BoxMullerMustMatchMoments()
        Dim rng As New Random(20261124)
        Const count As Integer = 200000

        Dim samples As Double() = BoxMuller.Fill(rng, count)
        Dim mean As Double = 0
        Dim variance As Double = 0

        For i As Integer = 0 To count - 1
            mean += samples(i)
        Next

        mean /= count

        For i As Integer = 0 To count - 1
            Dim diff As Double = samples(i) - mean
            variance += diff * diff
        Next

        variance /= (count - 1)

        Call DebugAudit.Capture("boxmuller.mean", mean)
        Call DebugAudit.Capture("boxmuller.variance", variance)

        Call DebugAudit.Test(std.Abs(mean) < 0.01, $"Box-Muller 均值应接近 0，实际 {mean}")
        Call DebugAudit.Test(std.Abs(variance - 1.0) < 0.02, $"Box-Muller 方差应接近 1，实际 {variance}")

        ' 相邻样本（来自同一对 cos/sin 分支）必须相互独立
        Dim covariance As Double = 0

        For i As Integer = 0 To count - 2 Step 2
            covariance += (samples(i) - mean) * (samples(i + 1) - mean)
        Next

        covariance /= (count \ 2)

        Call DebugAudit.Capture("boxmuller.pair-covariance", covariance)

        Call DebugAudit.Test(std.Abs(covariance) < 0.02,
                             $"同一对 cos/sin 分支应相互独立，实际协方差 {covariance}")
    End Sub

    Private Sub RandomGaussianMustBeReproducible()
        Dim first As New RandomGaussian(1234)
        Dim second As New RandomGaussian(1234)

        For i As Integer = 0 To 99
            Call DebugAudit.AreEqual(first.NextNormal(), second.NextNormal(),
                                     $"同种子的第 {i} 个正态样本应完全一致")
        Next

        Dim third As New RandomGaussian(5678)
        Dim different As Boolean = False

        Dim replayed As New RandomGaussian(1234)

        For i As Integer = 0 To 9
            If std.Abs(replayed.NextNormal() - third.NextNormal()) > 1.0E-12 Then
                different = True
            End If
        Next

        Call DebugAudit.Test(different, "不同种子应产生不同的采样序列")

        ' 类别采样应集中在高概率类别上
        Dim probability As Double() = {0.05, 0.9, 0.05}
        Dim counter As Integer() = {0, 0, 0}
        Dim sampler As New RandomGaussian(99)

        For i As Integer = 0 To 9999
            counter(sampler.NextCategorical(probability)) += 1
        Next

        Call DebugAudit.Capture("rng.categorical", CObj($"{counter(0)}/{counter(1)}/{counter(2)}"))

        Call DebugAudit.Test(std.Abs(CDbl(counter(1)) / 10000.0 - 0.9) < 0.02,
                             $"类别采样频率应接近目标概率 0.9，实际 {CDbl(counter(1)) / 10000.0}")
    End Sub

#End Region

#Region "线性代数"

    Private Function CorrelatedCovariance() As Tensor
        ' 2 维强相关协方差：[[2.0, 1.5], [1.5, 2.0]]
        Dim data As Double(,) = {
            {2.4, 1.5},
            {1.5, 2.4}
        }

        Return New Tensor(data)
    End Function

    Private Sub CholeskyMustReconstructCovariance()
        Dim covariance As Tensor = CorrelatedCovariance()
        Dim factor As Tensor = LinearAlgebraOps.CholeskyFactor(covariance)

        Call DebugAudit.Test(factor IsNot Nothing, "正定协方差矩阵必须能够完成 Cholesky 分解")

        If factor Is Nothing Then
            Return
        End If

        Dim reconstructed As Tensor = factor.MatMul(factor.Transpose())
        Dim errorValue As Double = AuditSupport.MaxAbsDiff(reconstructed, covariance)

        Call DebugAudit.Capture("cholesky.reconstruction-error", errorValue)

        Call DebugAudit.Test(errorValue < 1.0E-12,
                             $"L·Lᵀ 应还原协方差矩阵，最大误差 {errorValue}")

        ' 下三角性
        Call DebugAudit.Test(std.Abs(factor(0, 1)) < 1.0E-15,
                             $"Cholesky 因子必须是下三角，实际 L[0,1]={factor(0, 1)}")

        ' ln|A| 一致性
        Dim viaFactor As Double = LinearAlgebraOps.LogDeterminantFromFactor(factor)
        Dim viaMatrix As Double = LinearAlgebraOps.LogDeterminant(covariance)

        Call DebugAudit.Test(std.Abs(viaFactor - viaMatrix) < 1.0E-10,
                             $"由因子计算的 ln|A| 应与矩阵一致: {viaFactor} vs {viaMatrix}")
    End Sub

    Private Sub MahalanobisMustMatchInverse()
        Dim covariance As Tensor = CorrelatedCovariance()
        Dim factor As Tensor = LinearAlgebraOps.CholeskyFactor(covariance)
        Dim inverse As Tensor = LinearAlgebraOps.Inverse(covariance)

        Dim diff As Double() = {0.7, -1.3}

        Dim viaFactor As Double = LinearAlgebraOps.MahalanobisSquared(factor, diff)

        Dim viaInverse As Double = 0

        For r As Integer = 0 To 1
            Dim acc As Double = 0

            For c As Integer = 0 To 1
                acc += inverse(r, c) * diff(c)
            Next

            viaInverse += diff(r) * acc
        Next

        Call DebugAudit.Capture("mahalanobis.via-factor", viaFactor)
        Call DebugAudit.Capture("mahalanobis.via-inverse", viaInverse)

        Call DebugAudit.Test(std.Abs(viaFactor - viaInverse) < 1.0E-10,
                             $"前代求解的马氏距离应与显式求逆一致: {viaFactor} vs {viaInverse}")
    End Sub

    Private Sub GaussianSamplingMustPreserveCorrelation()
        Dim covariance As Tensor = CorrelatedCovariance()
        Dim factor As Tensor = LinearAlgebraOps.CholeskyFactor(covariance)
        Dim rng As New RandomGaussian(4321)
        Dim mean As Double() = {0.0, 0.0}

        Const count As Integer = 40000

        Dim sumX As Double = 0
        Dim sumY As Double = 0
        Dim sumXX As Double = 0
        Dim sumXY As Double = 0
        Dim sumYY As Double = 0

        For i As Integer = 0 To count - 1
            Dim sample As Double() = LinearAlgebraOps.SampleGaussian(rng, mean, factor)
            Dim x As Double = sample(0)
            Dim y As Double = sample(1)

            sumX += x
            sumY += y
            sumXX += x * x
            sumXY += x * y
            sumYY += y * y
        Next

        Dim mX As Double = sumX / count
        Dim mY As Double = sumY / count
        Dim varX As Double = sumXX / count - mX * mX
        Dim varY As Double = sumYY / count - mY * mY
        Dim covXY As Double = sumXY / count - mX * mY

        Call DebugAudit.Capture("sampling.var-x", varX)
        Call DebugAudit.Capture("sampling.cov-xy", covXY)

        Call DebugAudit.Test(std.Abs(varX - 2.4) < 0.1, $"采样方差应还原为 2.4，实际 {varX}")
        Call DebugAudit.Test(std.Abs(varY - 2.4) < 0.1, $"采样方差应还原为 2.4，实际 {varY}")

        ' 这一项是旧实现（只取对角元）必然失败的判据：它恒为 0
        Call DebugAudit.Test(std.Abs(covXY - 1.5) < 0.1,
                             $"采样协方差必须保留非对角相关性 1.5，实际 {covXY}（只取对角元的旧实现恒为 0）")
    End Sub

    Private Sub CholeskyMustRecoverFromSingular()
        ' 秩亏矩阵：第二行等于第一行
        Dim data As Double(,) = {
            {1.0, 1.0},
            {1.0, 1.0}
        }

        Dim singular As New Tensor(data)

        Call DebugAudit.Test(LinearAlgebraOps.CholeskyFactor(singular) Is Nothing,
                             "秩亏矩阵不应直接分解成功")

        Dim jitter As Double = 0
        Dim recovered As Tensor = LinearAlgebraOps.CholeskyFactorWithJitter(singular, jitter)

        Call DebugAudit.Capture("cholesky.jitter", jitter)

        Call DebugAudit.Test(recovered IsNot Nothing, "抖动回退后应能获得可用的 Cholesky 因子")
        Call DebugAudit.Test(jitter > 0, $"抖动回退必须记录实际使用的抖动幅度，实际 {jitter}")
    End Sub

#End Region

#Region "持久化"

    Private Sub ModelParametersMustRoundTrip()
        Dim path As String = IO.Path.Combine(AppContext.BaseDirectory, "numerics-audit-model.bin")

        Dim parameters As New ModelParameters("audit", 3)
        Dim weights As New Tensor(New Double() {1.0, 2.0, 3.0, 4.0, 5.0, 6.0}, 2, 3)

        Call parameters.Add("weights", weights)
        Call parameters.AddValue("learningRate", 0.0125)
        Call parameters.AddText("activation", "relu")
        Call parameters.Save(path)

        Dim restored As ModelParameters = ModelParameters.Load(path)

        Call DebugAudit.AreEqual("audit", restored.ModelType, "ModelType 必须往返一致")
        Call DebugAudit.AreEqual(3, restored.SchemaVersion, "SchemaVersion 必须往返一致")
        Call DebugAudit.AreEqual(0.0125, restored.GetValue("learningRate"), "标量参数必须往返一致")

        Dim text As String = ""
        Call DebugAudit.Test(restored.TryGetText("activation", text) AndAlso text = "relu",
                             "文本参数必须往返一致")

        Dim restoredWeights As Tensor = restored.RequireTensor("weights", New Integer() {2, 3})
        Call DebugAudit.Test(AuditSupport.MaxAbsDiff(restoredWeights, weights) < 1.0E-15,
                             "张量参数必须逐元素往返一致")

        ' 损坏的文件必须被拒绝，而不是静默还原出垃圾
        Call DebugAudit.CheckThrows(Of IO.InvalidDataException)(
            Sub()
                IO.File.WriteAllBytes(path, New Byte() {1, 2, 3, 4, 5, 6, 7, 8, 9, 10})
                Call ModelParameters.Load(path)
            End Sub,
            "魔数不匹配的文件必须被拒绝")

        If IO.File.Exists(path) Then
            IO.File.Delete(path)
        End If
    End Sub

#End Region

#Region "优化器与模型选择"

    Private Sub AdamMustMinimiseQuadratic()
        ' 最小化 f(w) = Σ (w_i - target_i)^2，梯度 2(w - target)
        Dim target As Double() = {1.0, -2.0, 0.5}
        Dim parameter As New Tensor(New Double() {0.0, 0.0, 0.0}, 1, 3)
        Dim gradient As New Tensor(New Double() {0.0, 0.0, 0.0}, 1, 3)

        Dim optimizer As New Adam(New Integer() {1, 3}, 0.1)
        Dim initial As Double = QuadraticLoss(parameter, target)

        For iteration As Integer = 1 To 300
            For i As Integer = 0 To 2
                gradient.Data(i) = 2.0 * (parameter.Data(i) - target(i))
            Next

            optimizer.Update(parameter, gradient)
        Next

        Dim finalValue As Double = QuadraticLoss(parameter, target)

        Call DebugAudit.Capture("adam.initial", initial)
        Call DebugAudit.Capture("adam.final", finalValue)

        Call DebugAudit.Test(finalValue < initial * 1.0E-6,
                             $"Adam 应显著降低二次型损失: {initial} -> {finalValue}")
        Call DebugAudit.Test(optimizer.StepCount = 300,
                             $"Adam 步数应为 300，实际 {optimizer.StepCount}")
    End Sub

    Private Function QuadraticLoss(parameter As Tensor, target As Double()) As Double
        Dim acc As Double = 0

        For i As Integer = 0 To target.Length - 1
            Dim diff As Double = parameter.Data(i) - target(i)
            acc += diff * diff
        Next

        Return acc
    End Function

    Private Sub BicAndAicMustFollowFormula()
        Dim logLik As Double = -120.5
        Dim parameters As Integer = 12
        Dim samples As Integer = 500

        Dim expectedBic As Double = -2.0 * logLik + parameters * std.Log(samples)
        Dim expectedAic As Double = -2.0 * logLik + 2.0 * parameters

        Call DebugAudit.AreEqual(expectedBic, ModelSelection.Bic(logLik, parameters, samples),
                                 "BIC 公式不符")
        Call DebugAudit.AreEqual(expectedAic, ModelSelection.Aic(logLik, parameters),
                                 "AIC 公式不符")

        ' 高斯混合的自由参数个数
        Call DebugAudit.AreEqual(2 + 9 + 18, ModelSelection.GaussianMixtureParameters(3, 3, 0),
                                 "full 协方差 GMM 的参数个数不符")
        Call DebugAudit.AreEqual(2 + 9 + 9, ModelSelection.GaussianMixtureParameters(3, 3, 1),
                                 "对角协方差 GMM 的参数个数不符")
    End Sub

#End Region

End Module
