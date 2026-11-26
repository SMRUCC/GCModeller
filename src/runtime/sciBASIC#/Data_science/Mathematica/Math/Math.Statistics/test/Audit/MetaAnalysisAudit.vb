Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.Math.Statistics
Imports Microsoft.VisualBasic.Math.Statistics.MetaAnalysis
Imports sysmath = System.Math

''' <summary>
''' 元分析算子的验收测试：手算基准、退化行为、权重口径与批量一致性。
''' </summary>
Friend Module MetaAnalysisAudit

    Private Const Section As String = "meta"

    Friend Function Run() As Integer
        Dim failures As Integer = 0

        ' ---- K = 1 退化：效应与标准误应原样返回 ----
        Dim pooled1 As PooledEvidence = RandomEffects.Pool({StudyEffect.Create(1.5, 2.0)})

        failures += Assert("single study converged", pooled1.Converged, pooled1.Diagnostics)
        failures += AssertClose("single study effect", pooled1.Effect, 1.5, 1.0E-12, Section)
        failures += AssertClose("single study se", pooled1.StandardError, sysmath.Sqrt(2.0), 1.0E-12, Section)
        failures += AssertClose("single study tau2", pooled1.Tau2, 0.0, 0.0, Section)
        failures += AssertClose("single study I2", pooled1.I2, 0.0, 0.0, Section)
        failures += Assert("single study diagnosis mentions homogeneous", pooled1.Diagnostics IsNot Nothing)

        ' ---- 同质研究：τ² = 0，等价于固定效应 ----
        Dim homogeneous As PooledEvidence = RandomEffects.Pool({
            StudyEffect.Create(2.0, 1.0),
            StudyEffect.Create(2.0, 1.0),
            StudyEffect.Create(2.0, 1.0)
        })

        failures += Assert("homogeneous converged", homogeneous.Converged)
        failures += AssertClose("homogeneous tau2", homogeneous.Tau2, 0.0, 0.0, Section)
        failures += AssertClose("homogeneous effect", homogeneous.Effect, 2.0, 1.0E-12, Section)
        failures += AssertClose("homogeneous se", homogeneous.StandardError, 1.0 / sysmath.Sqrt(3.0), 1.0E-12, Section)
        failures += AssertClose("homogeneous n_eff", homogeneous.NEffective, 3.0, 1.0E-10, Section)
        failures += AssertClose("homogeneous max weight share", homogeneous.MaxWeightShare, 1.0 / 3.0, 1.0E-12, Section)

        ' ---- 手算基准：y = {0, 2, 10}，v = {1, 1, 1} ----
        '   S0 = 3, S1 = 12, S2 = 104 ⇒ Q = 104 - 144/3 = 56, df = 2, C = 3 - 3/3 = 2
        '   τ² = (56 - 2)/2 = 27，w' = 1/28，effect = 4，se = √(28/3)
        Dim heterogeneous As PooledEvidence = RandomEffects.Pool({
            StudyEffect.Create(0.0, 1.0),
            StudyEffect.Create(2.0, 1.0),
            StudyEffect.Create(10.0, 1.0)
        })

        failures += Assert("heterogeneous converged", heterogeneous.Converged)
        failures += AssertClose("heterogeneous Q", heterogeneous.Q, 56.0, 1.0E-10, Section)
        failures += AssertClose("heterogeneous tau2", heterogeneous.Tau2, 27.0, 1.0E-10, Section)
        failures += AssertClose("heterogeneous I2", heterogeneous.I2, 54.0 / 56.0, 1.0E-12, Section)
        failures += AssertClose("heterogeneous effect", heterogeneous.Effect, 4.0, 1.0E-10, Section)
        failures += AssertClose("heterogeneous se", heterogeneous.StandardError, sysmath.Sqrt(28.0 / 3.0), 1.0E-10, Section)
        failures += AssertClose("heterogeneous z", heterogeneous.Z, 4.0 / sysmath.Sqrt(28.0 / 3.0), 1.0E-10, Section)

        ' ---- 逆方差口径：τ² = 0 时应等于固定效应加权均值 ----
        Dim ivw As PooledEvidence = RandomEffects.Pool({
            StudyEffect.Create(2.0, 1.0),
            StudyEffect.Create(2.0, 4.0)
        })

        failures += AssertClose("IVW effect", ivw.Effect, 2.0, 1.0E-12, Section)
        failures += AssertClose("IVW se", ivw.StandardError, 1.0 / sysmath.Sqrt(1.25), 1.0E-12, Section)

        ' ---- 额外权重倍数：mult = {4, 1} ⇒ (0*4 + 10*1)/5 = 2 ----
        Dim weighted As PooledEvidence = RandomEffects.Pool({
            StudyEffect.Create(0.0, 1.0, 4.0),
            StudyEffect.Create(10.0, 1.0, 1.0)
        })

        failures += AssertClose("weighted effect", weighted.Effect, 2.0, 1.0E-12, Section)

        ' ---- 缺失方差：按同组中位数回填 ----
        Dim fallback As PooledEvidence = RandomEffects.Pool({
            StudyEffect.Create(1.0, 1.0),
            StudyEffect.Create(2.0, Double.NaN),
            StudyEffect.Create(3.0, 1.0)
        })

        failures += Assert("missing variance falls back to median", fallback.Converged, fallback.Diagnostics)
        failures += Assert("fallback keeps all studies", fallback.Studies = 3, fallback.Studies.ToString)
        failures += AssertClose("fallback effect", fallback.Effect, 2.0, 1.0E-12, Section)

        ' ---- 全部方差缺失且无法回填：未收敛，而不是静默返回 0 ----
        Dim noVariance As PooledEvidence = RandomEffects.Pool({StudyEffect.Create(5.0, Double.NaN)})

        failures += Assert("no usable variance is not converged", Not noVariance.Converged)
        failures += Assert("no usable variance keeps NaN effect", Double.IsNaN(noVariance.Effect))
        failures += Assert("no usable variance is diagnosed", noVariance.Diagnostics IsNot Nothing)

        ' ---- 未测量的研究被排除 ----
        Dim unmeasured As PooledEvidence = RandomEffects.Pool({
            StudyEffect.Create(1.0, 1.0),
            StudyEffect.Create(100.0, 1.0, 1.0, measured:=False)
        })

        failures += AssertClose("unmeasured study is excluded", unmeasured.Effect, 1.0, 1.0E-12, Section)
        failures += Assert("unmeasured study not counted", unmeasured.Studies = 1, unmeasured.Studies.ToString)

        ' ---- DerSimonian-Laird 独立入口 ----
        failures += AssertClose("DL tau2 standalone",
                               RandomEffects.DerSimonianLaird({0.0, 2.0, 10.0}, {1.0, 1.0, 1.0}),
                               27.0, 1.0E-10, Section)
        failures += AssertClose("DL tau2 with 2 studies is 0",
                               RandomEffects.DerSimonianLaird({2.0, 2.0}, {1.0, 1.0}),
                               0.0, 1.0E-12, Section)

        ' ---- 逆方差权重 ----
        Dim weights As Double() = RandomEffects.InverseVarianceWeights({1.0, 4.0, Double.NaN})

        failures += AssertClose("IVW weight 1", weights(0), 1.0, 1.0E-15, Section)
        failures += AssertClose("IVW weight 2", weights(1), 0.25, 1.0E-15, Section)
        failures += AssertClose("IVW weight missing", weights(2), 0.0, 0.0, Section)

        ' ---- Stouffer / Liptak ----
        failures += AssertClose("Stouffer equal weights",
                               EvidenceCombination.Stouffer({1.0, 1.0, 1.0}, {1.0, 1.0, 1.0}),
                               sysmath.Sqrt(3.0), 1.0E-12, Section)
        failures += AssertClose("Stouffer skips unmeasured",
                               EvidenceCombination.Stouffer({1.0, 100.0}, {1.0, 1.0}, {True, False}),
                               1.0, 1.0E-12, Section)

        Dim liptak As Double = EvidenceCombination.Liptak({0.05, 0.05}, {1.0, 1.0}, {1.0, 1.0})

        failures += AssertClose("Liptak of two p=0.05", liptak, sysmath.Sqrt(2.0) * 1.959963984540054, 1.0E-9, Section)

        ' ---- 加权均值的两种缺失语义 ----
        failures += AssertClose("weighted mean missing as zero",
                               EvidenceCombination.WeightedMean({2.0, 3.0}, {1.0, 1.0}, {True, False}, missingAsZero:=True),
                               1.0, 1.0E-12, Section)
        failures += AssertClose("weighted mean missing dropped",
                               EvidenceCombination.WeightedMean({2.0, 3.0}, {1.0, 1.0}, {True, False}, missingAsZero:=False),
                               2.0, 1.0E-12, Section)

        ' ---- 权重诊断 ----
        failures += AssertClose("normalized weights sum",
                               Sum(EvidenceCombination.NormalizeWeights({2.0, 1.0, 1.0})), 1.0, 1.0E-15, Section)
        failures += AssertClose("effective sample size of equal weights",
                               EvidenceCombination.EffectiveSampleSize({1.0, 1.0, 1.0, 1.0}), 4.0, 1.0E-12, Section)
        failures += AssertClose("weight share of a dominant weight",
                               EvidenceCombination.WeightShare({9.0, 1.0}), 0.9, 1.0E-12, Section)

        ' ---- 批量入口与单基因入口一致 ----
        Dim effects(,) As Double = {{1.0, 0.0}, {2.0, 2.0}, {3.0, 10.0}}
        Dim variances(,) As Double = {{1.0, 1.0}, {1.0, 1.0}, {1.0, 1.0}}
        Dim batch As PooledEvidence() = RandomEffects.PoolMatrix(effects, variances)

        For g As Integer = 0 To 1
            Dim manual As PooledEvidence = RandomEffects.Pool({
                StudyEffect.Create(effects(0, g), variances(0, g)),
                StudyEffect.Create(effects(1, g), variances(1, g)),
                StudyEffect.Create(effects(2, g), variances(2, g))
            })

            failures += AssertClose($"batch effect equals single [{g}]", batch(g).Effect, manual.Effect, 1.0E-12, Section)
            failures += AssertClose($"batch tau2 equals single [{g}]", batch(g).Tau2, manual.Tau2, 1.0E-12, Section)
            failures += AssertClose($"batch p equals single [{g}]", batch(g).PValue, manual.PValue, 1.0E-12, Section)
        Next

        ' ---- 合并结果的 p 值与 z 自洽 ----
        failures += AssertClose("p value from z",
                               heterogeneous.PValue,
                               NormalDistribution.TwoSidedPValue(heterogeneous.Z),
                               1.0E-15, Section)

        Call DebugAudit.Mark($"{Section} audit completed", failures)

        Return failures
    End Function

    Private Function Sum(values As Double()) As Double
        Dim total As Double = 0.0

        For i As Integer = 0 To values.Length - 1
            total += values(i)
        Next

        Return total
    End Function

End Module
