' ============================================================================
' TestChiSquare.vb — 不完全伽马 / χ² 生存函数 / motif E-value
' ----------------------------------------------------------------------------
' 对应 [em.md §9]：LLR = 2·Σ Z·logR，零假设下近似 χ²，df = (K−1)·W，
' E = 候选窗口总数 × p 值（保守近似）。
'
' 验证手段：
'   1. 与文献分位数对照（df = 1, 2, 4, 10, 20 × α = 0.05 / 0.01 / 0.001）；
'   2. 与解析解对照：Q(1, x) = e^(−x)（指数分布生存函数）；
'   3. 级数展开 / 连分数两条分支在 x = s+1 处的连续性；
'   4. 单调性、边界与数值稳定性（不产生 NaN / Inf）。
' ============================================================================

Option Strict On

Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif.EmMotif.Core

Namespace EmMotif

    Public Module TestChiSquare

        Public Sub RunAll()
            TestGammaLn()
            TestChiSquareQuantiles()
            TestGammaQAnalytic()
            TestBranchContinuity()
            TestMonotonicity()
            TestEValue()
        End Sub

        ''' <summary>log Γ(x)：与解析值对照</summary>
        Private Sub TestGammaLn()
            TestAssert.Section("log Γ(x)（Lanczos 近似）")

            ' Γ(1)=1, Γ(2)=1, Γ(5)=24, Γ(0.5)=√π
            TestAssert.CheckNear(ChiSquare.GammaLn(1.0), 0.0, 0.000000001, "ln Γ(1) = 0")
            TestAssert.CheckNear(ChiSquare.GammaLn(2.0), 0.0, 0.000000001, "ln Γ(2) = 0")
            TestAssert.CheckNear(ChiSquare.GammaLn(5.0), Math.Log(24.0), 0.000000001, "ln Γ(5) = ln 24 = 3.1780538")
            TestAssert.CheckNear(ChiSquare.GammaLn(0.5), Math.Log(Math.Sqrt(Math.PI)), 0.000000001, "ln Γ(0.5) = ln√π = 0.5723649")
            TestAssert.CheckNear(ChiSquare.GammaLn(10.0), Math.Log(362880.0), 0.00000001, "ln Γ(10) = ln 9! = 12.8018275")
        End Sub

        ''' <summary>χ² 生存函数 vs 文献分位数（容差 5e-4）</summary>
        Private Sub TestChiSquareQuantiles()
            TestAssert.Section("χ² 生存函数 vs 文献分位数 [em.md §9]")

            ' {df, 分位数, 期望的生存概率}
            Dim cases As Double()() = {
                New Double() {1.0, 3.841459, 0.05}, New Double() {2.0, 5.991465, 0.05},
                New Double() {4.0, 9.487729, 0.05}, New Double() {10.0, 18.307038, 0.05},
                New Double() {20.0, 31.410433, 0.05},
                New Double() {1.0, 6.634897, 0.01}, New Double() {2.0, 9.210340, 0.01},
                New Double() {4.0, 13.276704, 0.01}, New Double() {10.0, 23.209251, 0.01},
                New Double() {20.0, 37.566235, 0.01},
                New Double() {1.0, 10.827566, 0.001}, New Double() {2.0, 13.815511, 0.001},
                New Double() {4.0, 18.466827, 0.001}, New Double() {10.0, 29.588298, 0.001},
                New Double() {20.0, 45.314745, 0.001}
            }

            Dim worst As Double = 0
            Dim worstName As String = ""
            For Each c In cases
                Dim sf = ChiSquare.ChiSquareSf(c(0), c(1))
                Dim err = Math.Abs(sf - c(2))
                If err > worst Then
                    worst = err
                    worstName = $"χ²({c(0)}) sf({c(1)}) = {sf:F6}，期望 {c(2)}"
                End If
            Next
            TestAssert.Check(worst <= 0.0005, $"15 组文献分位数全部吻合（最大误差 {worst:G3}；最差：{worstName}）")
        End Sub

        ''' <summary>与解析解对照：Q(1, x) = e^(−x)（同时覆盖级数与连分数两条分支）</summary>
        Private Sub TestGammaQAnalytic()
            TestAssert.Section("Q(s,x) 解析解对照：Q(1,x) = e^(−x)")

            For Each x In New Double() {0.1, 0.5, 1.0, 1.5, 1.9, 2.5, 5.0, 10.0, 25.0}
                Dim q = ChiSquare.GammaQ(1.0, x)
                TestAssert.CheckNear(q, Math.Exp(-x), 0.000000001, $"Q(1, {x}) = e^−{x}")
            Next

            ' 边界
            TestAssert.CheckNear(ChiSquare.GammaQ(2.0, 0.0), 1.0, 0.000000001, "Q(s, 0) = 1")
            TestAssert.Check(ChiSquare.GammaQ(2.0, 1000.0) < 0.000000001, "Q(2, 1000) ≈ 0")
            TestAssert.CheckNear(ChiSquare.ChiSquareSf(3.0, 0.0), 1.0, 0.000000001, "χ² sf(df, 0) = 1")
        End Sub

        ''' <summary>级数展开与连分数两条分支在 x = s+1 附近必须连续</summary>
        Private Sub TestBranchContinuity()
            TestAssert.Section("级数/连分数分支连续性（切换点 x = s+1）")

            ' 用极小的步长跨过切换点：真实函数在此处的斜率约 0.15，
            ' Δx = 2e−7 带来的真值变化约 3e−8，远小于容差 1e−5；
            ' 若两条分支不自洽（典型偏差 ≥1e−3），就会被检出。
            For Each s In New Double() {1.0, 3.0, 10.0, 25.5}
                Dim lo = ChiSquare.GammaQ(s, s + 1.0 - 0.0000001)
                Dim hi = ChiSquare.GammaQ(s, s + 1.0 + 0.0000001)
                TestAssert.CheckNear(hi, lo, 0.00001, $"s={s} 时在 x=s+1 两侧连续（{lo:G8} → {hi:G8}）")
            Next
        End Sub

        ''' <summary>生存函数对 x 单调递减，且不产生 NaN / Inf</summary>
        Private Sub TestMonotonicity()
            TestAssert.Section("χ² 生存函数单调性与数值稳定性")

            Dim ok As Boolean = True
            Dim prev As Double = 1.0
            For i = 0 To 200
                Dim x = i * 0.5
                Dim sf = ChiSquare.ChiSquareSf(6.0, x)
                If Double.IsNaN(sf) OrElse Double.IsInfinity(sf) OrElse sf > prev + 0.000000001 Then ok = False
                prev = sf
            Next
            TestAssert.Check(ok, "sf(df=6) 在 x ∈ [0,100] 上单调递减且无 NaN/Inf")

            ' 极端 df / x 不崩
            Dim extremeOk As Boolean = True
            For Each df In New Double() {0.5, 1.0, 100.0, 1000.0}
                For Each x In New Double() {0.0001, 1.0, 500.0, 5000.0}
                    Dim v = ChiSquare.ChiSquareSf(df, x)
                    If Double.IsNaN(v) OrElse v < 0.0 OrElse v > 1.0 Then extremeOk = False
                Next
            Next
            TestAssert.Check(extremeOk, "极端 df/x 输入下 sf ∈ [0,1] 且无 NaN")
        End Sub

        ''' <summary>E-value = 窗口总数 × p 值 [em.md §9]</summary>
        Private Sub TestEValue()
            TestAssert.Section("motif E-value [em.md §9]")

            Dim df = 21.0     ' (K−1)·W = 3×7
            Dim windows = 10000.0

            ' 随 LLR 单调递减
            Dim monotone As Boolean = True
            Dim prev As Double = Double.PositiveInfinity
            For llr = 0 To 200 Step 5
                Dim e = ChiSquare.MotifEValue(CDbl(llr), df, windows)
                If e > prev + 0.000000001 Then monotone = False
                prev = e
            Next
            TestAssert.Check(monotone, "E-value 随 LLR 单调递减")

            ' 随候选窗口数单调递增
            Dim e1 = ChiSquare.MotifEValue(60.0, df, 1000.0)
            Dim e2 = ChiSquare.MotifEValue(60.0, df, 100000.0)
            TestAssert.Check(e2 > e1, $"E-value 随窗口数增大（{e1:G4} → {e2:G4}）")

            ' 与手工公式一致
            Dim manual = windows * ChiSquare.ChiSquareSf(df, 80.0)
            TestAssert.CheckNear(ChiSquare.MotifEValue(80.0, df, windows), manual, 0.000000001,
                                 "E-value = 窗口总数 × χ² sf")

            ' 显著性判读：强信号 E 远小于 0.05
            Dim strong = ChiSquare.MotifEValue(150.0, df, windows)
            TestAssert.Check(strong < 0.05, $"强信号（LLR=150, df=21）E={strong:G3} < 0.05 判显著 [em.md §9]")

            ' 下溢保护
            Dim tiny = ChiSquare.MotifEValue(5000.0, df, windows)
            TestAssert.Check(tiny > 0.0 AndAlso Not Double.IsNaN(tiny), $"极端 LLR 下 E-value 被夹逼为正数（{tiny:G3}）")
        End Sub

    End Module

End Namespace
