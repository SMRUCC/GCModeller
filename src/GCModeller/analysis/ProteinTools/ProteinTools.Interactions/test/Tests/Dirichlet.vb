Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Alignment
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Statistics

Namespace test.Tests

    ''' <summary>
    ''' Dirichlet-多项边际似然与似然比 <c>R_ij</c> 的数值对照。
    ''' </summary>
    ''' <remarks>
    ''' 使用 2 个符号的小字母表，此时所有 Gamma 函数值都可以手算：
    ''' <c>λ' = 1/2</c> ⇒ <c>λ = 1</c>，于是
    ''' <c>P(n) = Γ(2)/Γ(n+2) · Π_α Γ(n_α+1)/Γ(1) = 1/Γ(n+2) · Π n_α!</c>。
    ''' </remarks>
    Module Dirichlet

        Public Sub Run()
            Call SingleColumnChecks()
            Call PairColumnChecks()
            Call LikelihoodRatioChecks()
        End Sub

        Private Sub SingleColumnChecks()
            Dim model As New DirichletMultinomial(2, 0.5)

            Call Support.Near(1.0, model.LambdaSingle, 1.0E-12, "λ = K·λ'")

            ' n = (0, 0)：Π 是空积，结果为 Γ(2)/Γ(2) = 1
            Call Support.Near(0.0, model.LogMarginal({0, 0}), 1.0E-12, "log P(D_i) of an empty column")

            ' n = (3, 1)：1/Γ(6) · 3! · 1! = 6/120 = 0.05
            Call Support.Near(Math.Log(0.05), model.LogMarginal({3, 1}), 1.0E-10, "log P(D_i) of n=(3,1)")

            ' n = (4, 0)：1/Γ(6) · 4! = 24/120 = 0.2
            Call Support.Near(Math.Log(0.2), model.LogMarginal({4, 0}), 1.0E-10, "log P(D_i) of n=(4,0)")

            ' 计数越多，边际似然越小(概率质量被摊薄到更多的可能观测上)
            Call Support.True(
                model.LogMarginal({10, 10}) < model.LogMarginal({1, 1}),
                "the marginal likelihood must decrease with the sample size")

            ' 非法参数
            Call Support.Throws(Of ArgumentException)(
                Sub() model.LogMarginal({1, 2, 3}),
                "the residue count array must match the alphabet size")
        End Sub

        Private Sub PairColumnChecks()
            Dim model As New DirichletMultinomial(2, 0.5)

            ' 联合分布有 K² = 4 个类别，Σλ' = 2
            ' n 合计为 4，其中 (α,β) = (0,0) 出现 1 次、(1,1) 出现 3 次：
            ' Γ(2)/Γ(6) · [Γ(1.5)/Γ(0.5)] · [Γ(3.5)/Γ(0.5)] = (1/120)·0.5·1.875 = 0.0078125
            Dim joint As New Dictionary(Of Integer, Integer) From {{0, 1}, {3, 3}}

            Call Support.Near(
                Math.Log(0.0078125),
                model.LogMarginalPair(joint, 4),
                1.0E-10,
                "log P(D_ij) of a hand-computed joint count")

            ' 全部计数集中在一个类别上时概率最大
            Dim concentrated As New Dictionary(Of Integer, Integer) From {{0, 4}}
            Call Support.True(
                model.LogMarginalPair(concentrated, 4) > model.LogMarginalPair(joint, 4),
                "a concentrated joint count must have a larger marginal likelihood")
        End Sub

        Private Sub LikelihoodRatioChecks()
            ' 400 行：第 0 列是基准，第 1 列独立，第 2 列与第 0 列完全相同
            Dim alignment As JointAlignment = Fixtures.IndependentVersusCoupled(400, 1, 2, 20261102UL)
            Dim counts As New ColumnCounts(alignment)
            Dim ratios As New LikelihoodRatioMatrix(counts)

            Dim independent As Double = ratios.LogR(0, 1)
            Dim coupled As Double = ratios.LogR(0, 2)

            Call Support.Near(0.0, independent, 1.0, "two independent columns must have log R ≈ 0")
            Call Support.True(coupled > 10.0, $"two identical columns must have a large log R (got {coupled:G4})")
            Call Support.True(coupled > independent, "the coupled pair must score higher than the independent pair")

            ' 对称性
            Call Support.Near(coupled, ratios.LogR(2, 0), 1.0E-12, "log R must be symmetric")

            ' 对数与线性值一致
            Call Support.Near(Math.Exp(independent), ratios.R(0, 1), 1.0E-09, "R = exp(log R)")

            ' 大计数下 log R_ij 与互信息应当正相关
            Dim mi As New MutualInformation(counts)
            Dim correlated As Boolean = (coupled > independent) = (mi.MI(0, 2) > mi.MI(0, 1))

            Call Support.True(correlated, "log R_ij and the mutual information must order the same pair of columns consistently")
            Call Support.True(mi.MI(0, 2) > 0, "identical columns must have positive mutual information")

            ' 缓存
            ratios.Invalidate()
            Call Support.Near(coupled, ratios.LogR(0, 2), 1.0E-12, "log R must be reproducible after invalidation")
        End Sub
    End Module
End Namespace
