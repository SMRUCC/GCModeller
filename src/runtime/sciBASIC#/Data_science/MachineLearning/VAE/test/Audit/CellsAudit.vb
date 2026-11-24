' ============================================================================
' CellsAudit.vb - 单细胞条件生成模块的审计
'
' 覆盖：负二项 / 零膨胀负二项的对数似然闭式校验、似然梯度与中心差分的一致性、
'       文库大小归一化、one-hot 与对照条件识别、反事实预测的方向性、持久化往返。
' ============================================================================

Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.Cells
Imports std = System.Math

''' <summary>
''' 单细胞条件生成模块审计。
''' </summary>
Public Module CellsAudit

    Public Sub Run()
        Call AuditSupport.RunCase("cells.nb-loglikelihood", AddressOf NegativeBinomialMustMatchClosedForm)
        Call AuditSupport.RunCase("cells.zinb-loglikelihood", AddressOf ZeroInflationMustBehave)
        Call AuditSupport.RunCase("cells.likelihood-gradient", AddressOf LikelihoodGradientMustMatchFiniteDifference)
        Call AuditSupport.RunCase("cells.dataset", AddressOf DatasetMustNormalize)
        Call AuditSupport.RunCase("cells.counterfactual", AddressOf CounterfactualMustFollowDirection)
        Call AuditSupport.RunCase("cells.persistence", AddressOf ModelMustRoundTrip)
    End Sub

#Region "似然"

    Private Function LogGammaOf(value As Double) As Double
        Return Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.VariationalBayes.VBMath.LogGamma(value)
    End Function

    Private Sub NegativeBinomialMustMatchClosedForm()
        Dim likelihood As New CountLikelihood(CountLikelihoodKind.NegativeBinomial) With {.Dispersion = 2.5}

        Dim observed As Double = 7
        Dim meanValue As Double = 3.2
        Dim theta As Double = likelihood.Dispersion

        ' ln Γ(y+θ) − ln Γ(θ) − ln Γ(y+1) + θ·ln(θ/(θ+μ)) + y·ln(μ/(θ+μ))
        Dim expected As Double =
            LogGammaOf(observed + theta) - LogGammaOf(theta) - LogGammaOf(observed + 1.0) +
            theta * std.Log(theta / (theta + meanValue)) +
            observed * std.Log(meanValue / (theta + meanValue))

        Dim actual As Double = likelihood.LogLikelihood(observed, meanValue)

        DebugAudit.Capture("cells.nb.loglik", actual)
        DebugAudit.Capture("cells.nb.reference", expected)

        Call DebugAudit.Test(std.Abs(actual - expected) < 1.0E-10,
                             $"负二项对数似然与闭式不符: {actual} vs {expected}")

        ' 泊松是 θ → ∞ 的极限；θ 很大时两者应接近
        Dim poisson As New CountLikelihood(CountLikelihoodKind.Poisson)
        Dim largeTheta As New CountLikelihood(CountLikelihoodKind.NegativeBinomial) With {.Dispersion = 1.0E7}

        Dim poissonValue As Double = poisson.LogLikelihood(observed, meanValue)
        Dim nbValue As Double = largeTheta.LogLikelihood(observed, meanValue)

        Call DebugAudit.Test(std.Abs(poissonValue - nbValue) < 1.0E-4,
                             $"θ→∞ 时负二项应收敛到泊松: {poissonValue} vs {nbValue}")
    End Sub

    Private Sub ZeroInflationMustBehave()
        Dim pi As Double = 0.2
        Dim nb As New CountLikelihood(CountLikelihoodKind.NegativeBinomial) With {.Dispersion = 1.5}
        Dim zinb As New CountLikelihood(CountLikelihoodKind.ZeroInflatedNegativeBinomial) With {
            .Dispersion = 1.5,
            .ZeroInflation = pi
        }

        ' 正计数：ZINB = ln(1-π) + NB
        Dim positive As Double = 6
        Dim expected As Double = std.Log(1.0 - pi) + nb.LogLikelihood(positive, 4.0)
        Dim actual As Double = zinb.LogLikelihood(positive, 4.0)

        Call DebugAudit.Test(std.Abs(actual - expected) < 1.0E-10,
                             $"正计数的 ZINB 应为 ln(1-π)+NB: {actual} vs {expected}")

        ' 零计数：ZINB 必须比纯 NB 更大（零膨胀抬高零概率）
        Dim zinbZero As Double = zinb.LogLikelihood(0, 4.0)
        Dim nbZero As Double = nb.LogLikelihood(0, 4.0)

        DebugAudit.Capture("cells.zinb.zero-loglik", zinbZero)

        Call DebugAudit.Test(zinbZero > nbZero,
                             $"零膨胀应抬高零计数的概率: ZINB={zinbZero} 应大于 NB={nbZero}")

        ' π = 0 时 ZINB 退化为 NB
        zinb.ZeroInflation = 0.0

        Call DebugAudit.Test(std.Abs(zinb.LogLikelihood(0, 4.0) - nbZero) < 1.0E-9,
                             "π=0 时 ZINB 应退化为 NB")
    End Sub

    Private Sub LikelihoodGradientMustMatchFiniteDifference()
        Dim kinds As CountLikelihoodKind() = {CountLikelihoodKind.Gaussian,
                                              CountLikelihoodKind.Poisson,
                                              CountLikelihoodKind.NegativeBinomial,
                                              CountLikelihoodKind.ZeroInflatedNegativeBinomial}

        Dim observations As Double() = {0.0, 1.0, 7.0, 42.0}

        For Each kind As CountLikelihoodKind In kinds
            Dim likelihood As New CountLikelihood(kind) With {.Dispersion = 1.7, .ZeroInflation = 0.15}

            For Each observed As Double In observations
                Dim logMean As Double = 1.1
                Dim epsilon As Double = 1.0E-6

                Dim plus As Double = likelihood.LogLikelihood(observed, likelihood.MeanFromLogMean(logMean + epsilon))
                Dim minus As Double = likelihood.LogLikelihood(observed, likelihood.MeanFromLogMean(logMean - epsilon))
                Dim numeric As Double = -(plus - minus) / (2.0 * epsilon)

                Dim analytic As Double = likelihood.GradientLogMean(observed, likelihood.MeanFromLogMean(logMean))

                Dim errorValue As Double = std.Abs(analytic - numeric) / std.Max(1.0, std.Abs(numeric))

                If errorValue > 0.01 Then
                    Call DebugAudit.Test(False,
                                         $"{kind} 在 y={observed} 处的梯度与差分不符: 解析 {analytic}，数值 {numeric}")
                End If
            Next
        Next

        Call DebugAudit.Test(True, "所有似然的梯度均与中心差分一致")
    End Sub

#End Region

#Region "数据集"

    Private Function BuildDataset() As PerturbationDataset
        ' 2 个条件、4 个基因、200 个细胞；条件 1 把基因 0 上调 5 倍
        Dim cells As Integer = 200
        Dim genes As Integer = 4
        Dim rng As New Random(20261124)
        Dim counts As Double() = New Double(cells * genes - 1) {}
        Dim conditions As Integer() = New Integer(cells - 1) {}

        For i As Integer = 0 To cells - 1
            Dim condition As Integer = i Mod 2
            conditions(i) = condition

            For g As Integer = 0 To genes - 1
                Dim baseline As Double = 5.0 + g * 2.0
                Dim meanValue As Double = baseline

                If condition = 1 AndAlso g = 0 Then
                    meanValue = baseline * 5.0
                End If

                counts(i * genes + g) = std.Round(meanValue + AuditSupport.Randn(rng) * 1.5)
                counts(i * genes + g) = std.Max(0, counts(i * genes + g))
            Next
        Next

        Return PerturbationDataset.FromCounts(
            New Tensor(counts, cells, genes),
            conditions,
            New String() {"control", "drug"})
    End Function

    Private Sub DatasetMustNormalize()
        Dim dataset As PerturbationDataset = BuildDataset()

        Call DebugAudit.Test(dataset.CellCount = 200, $"细胞数应为 200，实际 {dataset.CellCount}")
        Call DebugAudit.Test(dataset.GeneCount = 4, $"基因数应为 4，实际 {dataset.GeneCount}")
        Call DebugAudit.Test(dataset.ControlCondition() = 0,
                             $"对照条件应被识别为 0，实际 {dataset.ControlCondition()}")

        ' 文库大小归一化后每行之和应为 1 万
        Dim normalized As Tensor = dataset.NormalizeLibrarySize(10000)

        For i As Integer = 0 To dataset.CellCount - 1
            Dim total As Double = 0

            For g As Integer = 0 To dataset.GeneCount - 1
                total += normalized(i, g)
            Next

            If std.Abs(total - 10000) > 0.01 Then
                Call DebugAudit.Test(False, $"第 {i} 个细胞归一化后总和应为 1 万，实际 {total}")
                Exit For
            End If
        Next

        ' one-hot 编码
        Dim oneHot As Tensor = dataset.OneHot(dataset.Conditions)
        Call DebugAudit.Test(oneHot.Shape(1) = 2, "one-hot 列数应等于条件数")

        For i As Integer = 0 To 9
            Dim rowSum As Double = 0

            For c As Integer = 0 To 1
                rowSum += oneHot(i, c)
            Next

            Call DebugAudit.Test(std.Abs(rowSum - 1.0) < 1.0E-12, "每个细胞的 one-hot 行和应为 1")
        Next

        ' 按条件切片
        Dim control As PerturbationDataset = dataset.SelectCondition(0)
        Call DebugAudit.Test(control.CellCount = 100, $"对照组的细胞数应为 100，实际 {control.CellCount}")
    End Sub

#End Region

#Region "反事实"

    Private Sub CounterfactualMustFollowDirection()
        Dim dataset As PerturbationDataset = BuildDataset()
        Dim model As New ConditionalVAE(geneCount:=4, conditionCount:=2, latentDim:=4,
                                        hiddenDim:=16, seed:=20261124)

        Call model.Fit(dataset, epochs:=80, batchSize:=32, learningRate:=0.01, beta:=0.5, verbose:=False)

        Call DebugAudit.Test(model.IsFitted, "训练完成后 IsFitted 应为 True")
        Call DebugAudit.Test(AuditSupport.AllFinite(model.Loss), "训练损失必须全部为有限值")

        Dim changes As Double() = model.LogFoldChange(dataset.Counts, 0, 1)

        DebugAudit.Capture("cells.counterfactual.gene0", changes(0))
        DebugAudit.Capture("cells.counterfactual.gene1", changes(1))

        Call DebugAudit.Test(changes(0) > 0,
                             $"扰动应上调基因 0 的预测表达（log2FC={changes(0):F3}）")

        ' 未受扰动的基因变化应明显小于目标基因
        Call DebugAudit.Test(changes(0) > changes(1),
                             $"目标基因的响应应强于无关基因: {changes(0):F3} vs {changes(1):F3}")

        ' 反事实预测的两个输出都应有限
        Dim result As Tuple(Of Tensor, Tensor) = model.PredictCounterfactual(dataset.Counts, 0, 1)

        Call DebugAudit.Test(AuditSupport.AllFinite(result.Item1), "原条件下的重构必须为有限值")
        Call DebugAudit.Test(AuditSupport.AllFinite(result.Item2), "目标条件下的预测必须为有限值")

        Dim generated As Tensor = model.Generate(20, condition:=1, seed:=7)
        Call DebugAudit.Test(AuditSupport.AllFinite(generated), "条件生成必须为有限值")
    End Sub

#End Region

#Region "持久化"

    Private Sub ModelMustRoundTrip()
        Dim dataset As PerturbationDataset = BuildDataset()
        Dim model As New ConditionalVAE(geneCount:=4, conditionCount:=2, latentDim:=3,
                                        hiddenDim:=12, seed:=5150)

        Call model.Fit(dataset, epochs:=20, batchSize:=32, learningRate:=0.01, beta:=0.5, verbose:=False)

        Dim path As String = IO.Path.Combine(AppContext.BaseDirectory, "cells-audit-model.bin")
        Call model.Save(path)

        Dim restored As New ConditionalVAE(geneCount:=4, conditionCount:=2, latentDim:=3, hiddenDim:=12)
        Dim errorMessage As String = Nothing

        Call DebugAudit.Test(restored.TryLoad(path, errorMessage), $"模型加载失败: {errorMessage}")

        Dim expected As Tensor = model.Reconstruct(dataset.Counts, dataset.OneHot(dataset.Conditions))
        Dim actual As Tensor = restored.Reconstruct(dataset.Counts, dataset.OneHot(dataset.Conditions))

        Dim errorValue As Double = AuditSupport.MaxAbsDiff(expected, actual)

        DebugAudit.Capture("cells.persistence.error", errorValue)

        Call DebugAudit.Test(errorValue < 1.0E-9,
                             $"还原后的重构结果应一致，最大偏差 {errorValue}")

        If IO.File.Exists(path) Then
            IO.File.Delete(path)
        End If
    End Sub

#End Region

End Module
