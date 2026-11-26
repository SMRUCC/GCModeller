Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.Math.Numerics.RootFinding
Imports Microsoft.VisualBasic.Math.Statistics
Imports Microsoft.VisualBasic.Math.Statistics.Apportionment
Imports Microsoft.VisualBasic.Math.Statistics.Copula
Imports Microsoft.VisualBasic.Math.Statistics.OrderStatistics
Imports sysmath = System.Math

''' <summary>
''' 整数分摊与 Gaussian copula 的验收测试。
''' </summary>
Friend Module ApportionCopulaAudit

    Private Const Section As String = "apportion"

    Friend Function Run() As Integer
        Dim failures As Integer = 0

        ' ---- 最大余数法：基础案例 ----
        Dim three As Long() = Apportion.LargestRemainder({1.0, 1.0, 1.0}, 10)

        failures += Assert("apportion keeps the total", Sum(three) = 10L, Sum(three).ToString)
        failures += Assert("apportion breaks ties by index", three(0) = 4L, three(0).ToString)

        Dim halves As Long() = Apportion.LargestRemainder({0.5, 0.5}, 3)

        failures += Assert("apportion of two halves", halves(0) = 2L AndAlso halves(1) = 1L, $"{halves(0)},{halves(1)}")

        Dim concentrated As Long() = Apportion.LargestRemainder({0.0, 1.0}, 5)

        failures += Assert("apportion of a point mass", concentrated(0) = 0L AndAlso concentrated(1) = 5L,
                           $"{concentrated(0)},{concentrated(1)}")

        ' ---- 最大余数法：守恒性与"偏差 < 1"对任意输入成立 ----
        Dim fractions As Double() = {0.11, 0.29, 0.37, 0.23, 0.0}
        Dim apportioned As Long() = Apportion.LargestRemainder(fractions, 1000)
        Dim totalMass As Double = 0.0

        For i As Integer = 0 To fractions.Length - 1
            totalMass += fractions(i)
        Next

        For i As Integer = 0 To fractions.Length - 1
            Dim ideal As Double = fractions(i) / totalMass * 1000

            failures += Assert($"apportion deviation < 1 [{i}]", sysmath.Abs(apportioned(i) - ideal) < 1.0,
                               $"count={apportioned(i)}, ideal={ideal:G6}")
        Next

        failures += Assert("apportion mass is exact", Apportion.IsIntegerMass(apportioned, 1000L))

        ' ---- 负路径 ----
        failures += AssertThrows("negative fraction rejected",
                                 Sub()
                                     Dim bad As Long() = Apportion.LargestRemainder({1.0, -1.0}, 10)
                                 End Sub)
        failures += AssertThrows("zero mass rejected",
                                 Sub()
                                     Dim bad As Long() = Apportion.LargestRemainder({0.0, 0.0}, 10)
                                 End Sub)

        ' ---- 质量守恒：补足 / 削平 ----
        Dim filled As Long() = Apportion.ConserveMass(New Long() {3L, 3L, 3L}, 12L)

        failures += Assert("conserve by filling", filled(0) = 4L AndAlso filled(1) = 4L AndAlso filled(2) = 4L,
                           $"{filled(0)},{filled(1)},{filled(2)}")

        Dim levelled As Long() = Apportion.ConserveMass(New Long() {3L, 3L, 3L}, 7L)

        failures += Assert("conserve by levelling keeps total", Sum(levelled) = 7L, Sum(levelled).ToString)
        failures += Assert("conserve by levelling stays non-negative",
                           levelled(0) >= 0L AndAlso levelled(1) >= 0L AndAlso levelled(2) >= 0L)

        Dim ordered As Long() = Apportion.ConserveMass(New Long() {0L, 0L}, 5L, New Integer() {1, 0})

        failures += Assert("conserve honours the priority order", ordered(1) >= ordered(0),
                           $"{ordered(0)},{ordered(1)}")
        failures += Assert("conserve with order keeps total", Sum(ordered) = 5L, Sum(ordered).ToString)

        ' ---- 分位剖面转整数 ----
        Dim profile As QuantileProfile = QuantileProfile.FromValues({1.375, 2.125, 2.875, 3.625})
        Dim integers As Long() = profile.AsIntegers(10)

        failures += Assert("profile as integers keeps total", Sum(integers) = 10L, Sum(integers).ToString)
        failures += Assert("profile as integers is monotone",
                           integers(0) <= integers(1) AndAlso integers(1) <= integers(2) AndAlso integers(2) <= integers(3))

        ' ---- copula：端点行为 ----
        Dim ranks As Double() = {1.0, 2.0, 3.0, 4.0}
        Dim noise As Double() = {0.5, -0.5, 1.5, -1.5}
        Dim independent As Double() = GaussianCopula.Mix(ranks, noise, 0.0)
        Dim identical As Double() = GaussianCopula.Mix(ranks, noise, 1.0)

        For i As Integer = 0 To 3
            failures += AssertClose($"r=0 keeps noise [{i}]", independent(i), noise(i), 1.0E-15, Section)
            failures += AssertClose($"r=1 keeps ranks [{i}]", identical(i), ranks(i), 1.0E-15, Section)
        Next

        ' ---- copula：名次 → 分位锚点 ----
        Dim ranksResult As QuantileRanks = GaussianCopula.RanksToQuantiles({3.0, 1.0, 2.0})

        failures += AssertClose("quantile of the smallest", ranksResult.Quantiles(1), 0.5 / 3.0, 1.0E-15, Section)
        failures += AssertClose("quantile of the middle", ranksResult.Quantiles(2), 1.5 / 3.0, 1.0E-15, Section)
        failures += AssertClose("quantile of the largest", ranksResult.Quantiles(0), 2.5 / 3.0, 1.0E-15, Section)
        failures += Assert("descending order starts at the largest",
                           ranksResult.[Order](0) = 0 AndAlso ranksResult.[Order](2) = 1,
                           $"{ranksResult.[Order](0)},{ranksResult.[Order](1)},{ranksResult.[Order](2)}")

        ' ---- copula：秩相关随 r 单调 ----
        Dim reference As Double() = NormalScores(8)
        Dim unitNoise As Double() = {-1.4, 0.3, 1.1, -0.7, 0.9, -1.1, 0.1, 0.8}
        Dim previous As Double = -2.0
        Dim monotone As Boolean = True

        For Each r As Double In {-0.9, -0.5, 0.0, 0.5, 0.9}
            Dim mixed As Double() = GaussianCopula.Mix(reference, unitNoise, r)
            Dim rho As Double = GaussianCopula.Spearman(reference, mixed)

            If rho < previous - 1.0E-9 Then monotone = False

            previous = rho
        Next

        failures += Assert("spearman rises monotonically with r", monotone)
        failures += Assert("spearman at r=0.9 exceeds 0.8", previous > 0.8, previous.ToString("G6"))

        ' ---- copula：完整采样 ----
        Dim sample As CopulaSample = GaussianCopula.Sample(reference, unitNoise, 0.6)
        Dim quantiles As Double() = sample.Quantiles
        Dim quantileSet As New HashSet(Of Double)

        For i As Integer = 0 To quantiles.Length - 1
            Call quantileSet.Add(sysmath.Round(quantiles(i), 12))
        Next

        failures += Assert("copula sample has n quantiles", quantileSet.Count = reference.Length, quantileSet.Count.ToString)
        failures += Assert("copula sample reports its correlation", sample.Correlation = 0.6)
        failures += Assert("copula sample is converged", sample.Converged)

        ' ---- 相关系数搜索 ----
        Dim linear As New Func(Of Double, Double)(Function(r As Double) 10.0 * (r + 1.0))
        Dim found As RootResult = GaussianCopula.FindCorrelation(linear, 15.0)

        failures += Assert("correlation search converged", found.Converged, found.Message)
        failures += AssertClose("correlation search value", found.Value, 0.5, 1.0E-3, Section)

        ' 目标不可达：常数函数应返回 Capped 而不是抛异常
        Dim constant As New Func(Of Double, Double)(Function(r As Double) 0.0)
        Dim unreachable As RootResult = GaussianCopula.FindCorrelation(constant, 1.0, -0.5, 0.5)

        failures += Assert("unreachable target is capped", unreachable.Capped, unreachable.Message)
        failures += Assert("unreachable target is not converged", Not unreachable.Converged)

        Call DebugAudit.Mark($"{Section} audit completed", failures)

        Return failures
    End Function

    ''' <summary>
    ''' 生成 <c>n</c> 个标准正态分数（用于 copula 的自洽测试，不依赖随机数）
    ''' </summary>
    Private Function NormalScores(n As Integer) As Double()
        Dim out As Double() = New Double(n - 1) {}

        For i As Integer = 0 To n - 1
            out(i) = NormalDistribution.InvCDF((i + 0.5) / n)
        Next

        Return out
    End Function

    Private Function Sum(values As Long()) As Long
        Dim total As Long = 0

        For i As Integer = 0 To values.Length - 1
            total += values(i)
        Next

        Return total
    End Function

    Private Function AssertThrows(name As String, code As Action) As Integer
        Try
            Call code()
        Catch ex As Exception
            Return 0
        End Try

        Call DebugAudit.Check(False, $"assertion failed: {name} should throw but did not")

        Return 1
    End Function

End Module
