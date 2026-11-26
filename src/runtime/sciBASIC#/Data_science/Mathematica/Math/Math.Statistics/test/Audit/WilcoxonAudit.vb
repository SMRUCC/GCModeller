Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis.ANOVA
Imports Microsoft.VisualBasic.Math.Statistics.OrderStatistics
Imports Microsoft.VisualBasic.Math.Statistics.SingleCell
Imports sysmath = System.Math

''' <summary>
''' Wilcoxon 秩和检验的验收测试：解析解、恒等式、与 Kruskal-Wallis 的交叉验证、批量与单点一致。
''' </summary>
Friend Module WilcoxonAudit

    Private Const Section As String = "wilcoxon"

    Friend Function Run() As Integer
        Dim failures As Integer = 0

        ' ---- 解析解：完全分离的两组，精确法双侧 p = 2 / C(6,3) = 0.1 ----
        Dim separated As WilcoxonResult = Wilcoxon.Test({10.0, 11.0, 12.0}, {1.0, 2.0, 3.0})

        failures += AssertClose("separated U", separated.U, 9.0, 1.0E-12, Section)
        failures += Assert("separated uses exact method", separated.Method = RankSumMethod.Exact, separated.Method.ToString)
        failures += AssertClose("separated exact p", separated.PValue, 0.1, 1.0E-12, Section)

        ' ---- 解析解：5 vs 5 完全分离，精确法双侧 p = 2 / C(10,5) = 1/126 ----
        Dim separated5 As WilcoxonResult = Wilcoxon.Test({6.0, 7.0, 8.0, 9.0, 10.0}, {1.0, 2.0, 3.0, 4.0, 5.0})

        failures += AssertClose("separated5 U", separated5.U, 25.0, 1.0E-12, Section)
        failures += AssertClose("separated5 exact p", separated5.PValue, 2.0 / 252.0, 1.0E-12, Section)

        ' ---- 完整并列：方差为 0，返回 p = 1 而不是异常 ----
        Dim tied As WilcoxonResult = Wilcoxon.Test({1.0, 1.0, 1.0}, {1.0, 1.0, 1.0})

        failures += AssertClose("fully tied p", tied.PValue, 1.0, 0.0, Section)
        failures += AssertClose("fully tied z", tied.Z, 0.0, 0.0, Section)
        failures += Assert("fully tied is diagnosed", tied.Diagnostics IsNot Nothing)

        ' ---- 恒等式：U(x,y) + U(y,x) = n1·n2，z 反号 ----
        Dim groupX As Double() = {2.1, 3.4, 1.7, 4.2, 2.8, 3.9}
        Dim groupY As Double() = {1.3, 2.6, 0.9, 3.1, 1.8, 2.4, 1.1}
        Dim forward As WilcoxonResult = Wilcoxon.Test(groupX, groupY)
        Dim backward As WilcoxonResult = Wilcoxon.Test(groupY, groupX)

        failures += AssertClose("U(x,y)+U(y,x) = n1·n2",
                               forward.U + backward.U, groupX.Length * groupY.Length, 1.0E-10, Section)
        failures += AssertClose("z is antisymmetric", forward.Z + backward.Z, 0.0, 1.0E-10, Section)
        failures += AssertClose("two-sided p is symmetric", forward.PValue, backward.PValue, 1.0E-12, Section)

        ' ---- 与 Kruskal-Wallis 交叉验证：双组时 H = z² ----
        Dim kw As KWResult = KruskalWallisModule.KruskalWallisTest({groupX, groupY})

        failures += Assert("KW result is valid", kw.IsValid, kw.InvalidReason)
        failures += AssertClose("two-group KW H equals wilcoxon z²",
                               kw.HStatistic, forward.Z * forward.Z, 1.0E-9 * sysmath.Max(1.0, kw.HStatistic), Section)

        ' Kruskal-Wallis 用卡方近似，因此只能与 Wilcoxon 的正态近似 p 对拍；
        ' 小样本时 Wilcoxon 默认走精确法，两者的 p 本来就不相等
        Dim normalP As Double = Wilcoxon.PValueFromZ(forward.Z, Hypothesis.TwoSided)

        failures += AssertClose("two-group KW p equals wilcoxon normal-approx p",
                               kw.PValue, normalP, 1.0E-8, Section)
        failures += Assert("exact p differs from chi-square approximation",
                           sysmath.Abs(forward.PValue - kw.PValue) > 1.0E-6,
                           $"exact={forward.PValue:G6}, kw={kw.PValue:G6}")

        ' ---- 秩和表：批量与单点一致 ----
        Dim controlCounts As Double()() = {
            New Double() {0.0, 10.0, 3.0},
            New Double() {1.0, 12.0, 0.0},
            New Double() {0.0, 9.0, 4.0},
            New Double() {2.0, 14.0, 1.0}
        }
        Dim sampleCounts As Double()() = {
            New Double() {0.0, 30.0, 1.0},
            New Double() {1.0, 28.0, 2.0},
            New Double() {0.0, 33.0, 0.0}
        }
        Dim control As ControlOrderStats = ControlOrderStats.Create(CompressedBuilder.FromDense(controlCounts))
        Dim table As RankSumTable = Wilcoxon.TestCounts(control, CompressedBuilder.FromDense(sampleCounts))

        failures += Assert("rank sum table size", table.GeneCount = 3, table.GeneCount.ToString)

        Dim sampleGrid As Double()() = CountStatistics.ToCpmGrid(CompressedBuilder.FromDense(sampleCounts))
        Dim controlGrid As Double()() = CountStatistics.ToCpmGrid(CompressedBuilder.FromDense(controlCounts))

        ' 批量路径固定使用正态近似（逐基因做精确枚举在 18k 基因规模下不可行），
        ' 因此这里单点对拍也强制正态近似
        Dim normalOnly As RankSumOptions = RankSumOptions.Default()
        normalOnly.ExactThreshold = 0

        For g As Integer = 0 To 2
            Dim x As Double() = Column(sampleGrid, g)
            Dim y As Double() = Column(controlGrid, g)
            Dim singleResult As WilcoxonResult = Wilcoxon.Test(x, y, normalOnly)

            failures += AssertClose($"batch U equals single [{g}]", table.U(g), singleResult.U, 1.0E-9, Section)
            failures += AssertClose($"batch z equals single [{g}]", table.Z(g), singleResult.Z, 1.0E-9, Section)
            failures += AssertClose($"batch p equals single [{g}]", table.PValue(g), singleResult.PValue, 1.0E-9, Section)
            failures += AssertClose($"batch effect equals single [{g}]", table.Effect(g), singleResult.Effect, 1.0E-9, Section)
        Next

        ' ---- 表达门槛：对照组平均 CPM 过低的基因不参与检验 ----
        Dim strict As RankSumOptions = RankSumOptions.Default()
        strict.MinControlCpm = 200000.0

        Dim strictTable As RankSumTable = Wilcoxon.TestCounts(control, CompressedBuilder.FromDense(sampleCounts), strict)

        failures += Assert("low expression gene is ineligible", Not strictTable.Eligible(0))
        failures += Assert("ineligible gene has NaN p", Double.IsNaN(strictTable.PValue(0)))
        failures += Assert("ineligible gene is not significant", Not strictTable.Significant(0))
        failures += Assert("high expression gene stays eligible", strictTable.Eligible(1))

        ' ---- 多重校正：padj >= p，且显著性标记一致 ----
        Dim options As RankSumOptions = RankSumOptions.Default()
        Dim adjusted As RankSumTable = Wilcoxon.TestCounts(control, CompressedBuilder.FromDense(sampleCounts), options)

        For g As Integer = 0 To adjusted.GeneCount - 1
            If adjusted.Eligible(g) Then
                failures += Assert($"padj >= p [{g}]", adjusted.PAdjusted(g) >= adjusted.PValue(g) - 1.0E-12,
                                   $"{adjusted.PAdjusted(g):G6} < {adjusted.PValue(g):G6}")
                failures += Assert($"significance flag [{g}]",
                                   adjusted.Significant(g) = (adjusted.PAdjusted(g) < options.Alpha))
            End If
        Next

        ' ---- 方向：单侧检验应给出更小的 p ----
        Dim greater As RankSumOptions = RankSumOptions.Default()
        greater.Alternative = Hypothesis.Greater

        Dim oneSided As WilcoxonResult = Wilcoxon.Test(groupX, groupY, greater)

        failures += Assert("one-sided p <= two-sided p", oneSided.PValue <= forward.PValue + 1.0E-15,
                           $"{oneSided.PValue:G6} vs {forward.PValue:G6}")
        failures += Assert("one-sided p ≈ two-sided p / 2",
                           sysmath.Abs(oneSided.PValue - forward.PValue / 2.0) < 1.0E-9)

        Call DebugAudit.Mark($"{Section} audit completed", failures)

        Return failures
    End Function

    Private Function Column(grid As Double()(), index As Integer) As Double()
        Dim out As Double() = New Double(grid.Length - 1) {}

        For i As Integer = 0 To grid.Length - 1
            out(i) = grid(i)(index)
        Next

        Return out
    End Function

End Module
