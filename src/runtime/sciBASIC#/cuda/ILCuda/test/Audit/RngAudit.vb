' ------------------------------------------------------------------------
' 设备端随机数的审计
'
' 判据：
'   * 均匀分布：与 CPU 参考实现逐位相等（整数位运算 + 一次缩放，没有超越函数）
'   * 正态分布：Box-Muller 里的 sqrt/log/cos 属超越函数，按 1e-12 判据
'   * 置换：双射性（0..n-1 各出现一次）且与参考实现完全一致
'   * 加权抽样：频次落在二项分布的合理区间内
'   * 不同 (seed, subsequence) 之间序列不相关
' ------------------------------------------------------------------------

Imports Microsoft.VisualBasic.Computing.ILCuda.Math

Namespace Audit

    ''' <summary>GpuRandom 的验收用例</summary>
    Public Module RngAudit

        Public Function Run() As Integer
            Call Console.WriteLine()
            Call Console.WriteLine("== 设备端随机数 ==")

            UniformMatchesReference()
            NormalMatchesReference()
            PermutationIsBijective()
            WeightedChoiceFrequencies()
            IndependentStreams()
            OffsetShiftsSequence()

            Return AuditRunner.FailureCount
        End Function

        ' ------------------------------------------------------------------
        ' 用例
        ' ------------------------------------------------------------------

        Private Sub UniformMatchesReference()
            Dim count As Integer = 200000
            Dim seed As ULong = &H1234_5678_9ABC_DEF0UL
            Dim actual As Double() = GpuRandom.Uniform(AuditRunner.Engine, count, seed)
            Dim expected As Double() = GpuRandom.UniformCpu(count, seed)
            Dim err As Double = AuditRunner.MaxAbsError(actual, expected)
            Dim inRange As Boolean = True

            For i As Integer = 0 To count - 1
                If actual(i) < 0.0 OrElse actual(i) >= 1.0 Then inRange = False
            Next

            Call AuditRunner.Capture("rng.uniform.maxAbsError", err)
            Call AuditRunner.Record($"均匀分布与参考实现逐位一致（{count} 个）", err = 0.0, $"maxAbsError={err:G6}")
            Call AuditRunner.Record("均匀分布落在 [0, 1) 内", inRange)
        End Sub

        Private Sub NormalMatchesReference()
            Dim count As Integer = 100000
            Dim seed As ULong = 20261126UL
            Dim actual As Double() = GpuRandom.Normal(AuditRunner.Engine, count, seed)
            Dim expected As Double() = GpuRandom.NormalCpu(count, seed)
            Dim err As Double = AuditRunner.MaxAbsError(actual, expected)

            ' 均值/标准差做一次合理性检查（正态性不必严格，只看量级）
            Dim mean As Double = 0.0
            Dim square As Double = 0.0

            For i As Integer = 0 To count - 1
                mean += actual(i)
                square += actual(i) * actual(i)
            Next

            mean /= count

            Dim sd As Double = System.Math.Sqrt(System.Math.Max(square / count - mean * mean, 0.0))

            Call AuditRunner.Capture("rng.normal.maxAbsError", err)
            Call AuditRunner.Capture("rng.normal.mean", mean)
            Call AuditRunner.Capture("rng.normal.sd", sd)
            Call AuditRunner.Record("正态分布与参考实现一致（1e-12）", err <= 1.0E-12, $"maxAbsError={err:G6}")
            Call AuditRunner.Record("正态分布均值接近 0、标准差接近 1",
                                    System.Math.Abs(mean) < 0.02 AndAlso System.Math.Abs(sd - 1.0) < 0.02,
                                    $"mean={mean:F6}, sd={sd:F6}")
        End Sub

        Private Sub PermutationIsBijective()
            Dim n As Integer = 400
            Dim seed As ULong = 7UL
            Dim actual As Integer() = GpuRandom.Permutation(AuditRunner.Engine, n, seed)
            Dim expected As Integer() = GpuRandom.PermutationCpu(n, seed)
            Dim seen(n - 1) As Boolean
            Dim bijective As Boolean = True

            For Each index As Integer In actual
                If index < 0 OrElse index >= n OrElse seen(index) Then
                    bijective = False
                Else
                    seen(index) = True
                End If
            Next

            Call AuditRunner.Record("置换是双射（0..n-1 各出现一次）", bijective, $"n={n}")
            Call AuditRunner.Record("置换与参考实现一致", AuditRunner.SameIntegers(actual, expected))

            ' 至少要有一定比例的错位，否则"置换"实际上没打乱
            Dim moved As Integer = 0

            For i As Integer = 0 To n - 1
                If actual(i) <> i Then moved += 1
            Next

            Call AuditRunner.Record("置换确实发生了重排", moved > n \ 2, $"moved={moved}/{n}")
        End Sub

        Private Sub WeightedChoiceFrequencies()
            Dim cdf As Double() = {0.1, 0.3, 0.6, 1.0}
            Dim draws As Integer = 20000
            Dim actual As Integer() = GpuRandom.WeightedChoice(AuditRunner.Engine, cdf, draws, 99UL)
            Dim expected As Integer() = GpuRandom.WeightedChoiceCpu(cdf, draws, 99UL)
            Dim counts(3) As Integer

            For Each index As Integer In actual
                If index >= 0 AndAlso index < 4 Then counts(index) += 1
            Next

            Dim ok As Boolean = True

            ' 期望比例 0.1 / 0.2 / 0.3 / 0.4，允许 1.5 个百分点的绝对偏差
            For i As Integer = 0 To 3
                Dim share As Double = CDbl(counts(i)) / draws
                Dim want As Double = If(i = 0, 0.1, cdf(i) - cdf(i - 1))

                If System.Math.Abs(share - want) > 0.015 Then ok = False
            Next

            Call AuditRunner.Record("加权抽样频次接近目标比例", ok,
                                    $"counts=[{counts(0)},{counts(1)},{counts(2)},{counts(3)}] of {draws}")
            Call AuditRunner.Record("加权抽样与参考实现一致", AuditRunner.SameIntegers(actual, expected))
        End Sub

        Private Sub IndependentStreams()
            Dim count As Integer = 5000
            Dim seed As ULong = 42UL
            Dim a As Double() = GpuRandom.UniformCpu(count, seed, 0UL, 0UL)
            Dim b As Double() = GpuRandom.UniformCpu(count, seed, 1UL, 0UL)
            Dim c As Double() = GpuRandom.UniformCpu(count, seed, 0UL, 100000UL)
            Dim sameAB As Integer = 0
            Dim sameAC As Integer = 0

            For i As Integer = 0 To count - 1
                If a(i) = b(i) Then sameAB += 1
                If a(i) = c(i) Then sameAC += 1
            Next

            Call AuditRunner.Record("不同子序列产生不同序列", sameAB = 0, $"重复={sameAB}")
            Call AuditRunner.Record("不同偏移产生不同序列", sameAC = 0, $"重复={sameAC}")
        End Sub

        Private Sub OffsetShiftsSequence()
            Dim count As Integer = 5000
            Dim seed As ULong = 2026UL
            Dim head As Double() = GpuRandom.UniformCpu(count, seed, 0UL, 0UL)
            Dim shifted As Double() = GpuRandom.UniformCpu(count, seed, 0UL, 1UL)
            Dim consistent As Boolean = True

            ' 偏移 1 之后的序列应当等于原序列整体左移一位
            For i As Integer = 0 To count - 2
                If head(i + 1) <> shifted(i) Then consistent = False
            Next

            Call AuditRunner.Record("偏移等价于序列平移（计数器语义）", consistent)
        End Sub
    End Module
End Namespace
