' ------------------------------------------------------------------------
' 计数合成的审计
'
' 判据（比浮点算子更严格）：
'   * copula 分位：分位锚点与降序名次逐元素相等
'   * 分位取值 + 质量守恒：整数计数逐元素相等，且每个基因的总量精确
'   * 秩和 z：与 CPU 参考的相对误差 <= 1e-12
'   * 平移二分：整数计数逐元素相等（迭代路径一致时必然相同）
'   * 突发散布：整数计数逐元素相等，总量守恒
' ------------------------------------------------------------------------

Imports Microsoft.VisualBasic.Computing.ILCuda.GPUTensor

Namespace Audit

    ''' <summary>CellSynthesis 的验收用例</summary>
    Public Module SynthesisAudit

        Public Function Run() As Integer
            Call Console.WriteLine()
            Call Console.WriteLine("== 计数合成 ==")

            CopulaQuantiles()
            QuantileMass()
            WilcoxonZ()
            ShiftBisect()
            BurstScatter()

            Return AuditRunner.FailureCount
        End Function

        ' ------------------------------------------------------------------
        ' 用例
        ' ------------------------------------------------------------------

        Private Sub CopulaQuantiles()
            Dim g As Integer = 8
            Dim n As Integer = 400
            Dim e0 As Double() = RandomVector(g * n, 401)
            Dim zl As Double() = RandomVector(n, 409)
            Dim correlation As Double = 0.35
            Dim actual As CopulaResult = CellSynthesis.CopulaQuantiles(AuditRunner.Engine, e0, zl, correlation, g, n)
            Dim expected As CopulaResult = CellSynthesis.CopulaQuantilesCpu(e0, zl, correlation, g, n)
            Dim qErr As Double = AuditRunner.MaxAbsError(actual.Quantiles, expected.Quantiles)
            Dim orderOk As Boolean = AuditRunner.SameIntegers(actual.Ordering, expected.Ordering)

            Call AuditRunner.Record("copula 分位锚点与参考一致", qErr = 0.0, $"maxAbsError={qErr:G6}")
            Call AuditRunner.Record("copula 降序名次与参考一致", orderOk)
        End Sub

        Private Sub QuantileMass()
            Dim g As Integer = 6
            Dim n As Integer = 400
            Dim m As Integer = 2000
            Dim ctrl As Single() = SortedControl(g, m, 419)
            Dim copula As CopulaResult = CellSynthesis.CopulaQuantilesCpu(
                RandomVector(g * n, 421), RandomVector(n, 431), 0.2, g, n)
            Dim libs As Double() = RandomLibraries(n, 439)
            Dim totals(g - 1) As Long
            Dim shift(g - 1) As Double

            For item As Integer = 0 To g - 1
                totals(item) = 400000L + item * 1000L
                shift(item) = 0.05 * item
            Next

            Dim actual As Integer() = CellSynthesis.QuantileMass(
                AuditRunner.Engine, ctrl, copula.Quantiles, copula.Ordering,
                libs, totals, shift, g, n, m, ntop:=40)
            Dim expected As Integer() = CellSynthesis.QuantileMassCpu(
                ctrl, copula.Quantiles, copula.Ordering, libs, totals, shift, g, n, m, ntop:=40)

            Dim ok As Boolean = AuditRunner.SameIntegers(actual, expected)
            Dim conserved As Boolean = True

            For item As Integer = 0 To g - 1
                Dim sum As Long = 0

                For c As Integer = 0 To n - 1
                    sum += actual(item * n + c)
                Next

                If sum <> totals(item) Then conserved = False
            Next

            Call AuditRunner.Record("分位取值 + 质量守恒与参考逐元素相等", ok)
            Call AuditRunner.Record("每个基因的计数总量精确等于设计总量", conserved)
        End Sub

        Private Sub WilcoxonZ()
            Dim g As Integer = 6
            Dim n As Integer = 400
            Dim m As Integer = 2000
            Dim ctrl As Single() = SortedControl(g, m, 443)
            Dim counts As Integer() = RandomCounts(g, n, 449)
            Dim libs As Double() = RandomLibraries(n, 457)
            Dim tieSum As Double() = TieSums(g, m, 461)
            Dim actual As Double() = CellSynthesis.WilcoxonZ(
                AuditRunner.Engine, counts, libs, ctrl, tieSum, g, n, m)
            Dim expected As Double() = CellSynthesis.WilcoxonZCpu(
                counts, libs, ctrl, tieSum, g, n, m)
            Dim err As Double = AuditRunner.MaxAbsError(actual, expected)

            Call AuditRunner.Capture("synth.z.maxAbsError", err)
            Call AuditRunner.Record("秩和 z 与参考一致（1e-12）", err <= 1.0E-12, $"maxAbsError={err:G6}")
        End Sub

        Private Sub ShiftBisect()
            Dim g As Integer = 4
            Dim n As Integer = 400
            Dim m As Integer = 2000
            Dim ctrl As Single() = SortedControl(g, m, 463)
            Dim copula As CopulaResult = CellSynthesis.CopulaQuantilesCpu(
                RandomVector(g * n, 467), RandomVector(n, 479), 0.0, g, n)
            Dim libs As Double() = RandomLibraries(n, 487)
            Dim totals(g - 1) As Long
            Dim target(g - 1) As Double

            For item As Integer = 0 To g - 1
                totals(item) = 400000L
                target(item) = 2.0 + item * 0.5
            Next

            Dim tieSum As Double() = TieSums(g, m, 491)
            Dim actual As BisectResult = CellSynthesis.ShiftBisect(
                AuditRunner.Engine, ctrl, tieSum, copula.Quantiles, copula.Ordering,
                libs, totals, target, g, n, m,
                ntop:=40, maxIterations:=24, tolerance:=0.05, lower:=-0.6, upper:=1.6)
            Dim expected As BisectResult = CellSynthesis.ShiftBisectCpu(
                ctrl, tieSum, copula.Quantiles, copula.Ordering,
                libs, totals, target, g, n, m,
                ntop:=40, maxIterations:=24, tolerance:=0.05, lower:=-0.6, upper:=1.6)

            Dim ok As Boolean = AuditRunner.SameIntegers(actual.Counts, expected.Counts)
            Dim zErr As Double = AuditRunner.MaxAbsError(actual.Z, expected.Z)

            Call AuditRunner.Capture("synth.bisect.z.maxAbsError", zErr)
            Call AuditRunner.Record("平移二分后的计数逐元素相等", ok)
            Call AuditRunner.Record("平移二分收敛到同一 z", zErr <= 1.0E-09, $"maxAbsError={zErr:G6}")
        End Sub

        Private Sub BurstScatter()
            Dim g As Integer = 8
            Dim n As Integer = 400
            Dim libs As Double() = RandomLibraries(n, 499)
            Dim totals(g - 1) As Long

            For item As Integer = 0 To g - 1
                totals(item) = 37L * (item + 1)
            Next

            Dim actual As Integer() = CellSynthesis.Burst(AuditRunner.Engine, totals, libs, g, n, 4, 20261126UL)
            Dim expected As Integer() = CellSynthesis.BurstCpu(totals, libs, g, n, 4, 20261126UL)
            Dim conserved As Boolean = True

            For item As Integer = 0 To g - 1
                Dim sum As Long = 0

                For c As Integer = 0 To n - 1
                    sum += actual(item * n + c)
                Next

                If sum <> totals(item) Then conserved = False
            Next

            Call AuditRunner.Record("突发散布与参考逐元素相等", AuditRunner.SameIntegers(actual, expected))
            Call AuditRunner.Record("突发散布保持总量守恒", conserved)
        End Sub

        ' ------------------------------------------------------------------
        ' 数据
        ' ------------------------------------------------------------------

        Private Function RandomVector(count As Integer, seed As Integer) As Double()
            Dim rng As New Random(seed)
            Dim result(count - 1) As Double

            For i As Integer = 0 To count - 1
                result(i) = rng.NextDouble() * 2.0 - 1.0
            Next

            Return result
        End Function

        ''' <summary>每个基因一段升序的对照 CPM（float32 栅格，允许并列）。</summary>
        Private Function SortedControl(g As Integer, m As Integer, seed As Integer) As Single()
            Dim rng As New Random(seed)
            Dim result(g * m - 1) As Single
            Dim buffer(m - 1) As Double

            For item As Integer = 0 To g - 1
                For i As Integer = 0 To m - 1
                    ' 量化到 0.5 的栅格，制造大量并列以覆盖 tie 校正
                    buffer(i) = System.Math.Round(rng.NextDouble() * 2000.0, 0) * 0.5
                Next

                Array.Sort(buffer)

                For i As Integer = 0 To m - 1
                    result(item * m + i) = CSng(buffer(i))
                Next
            Next

            Return result
        End Function

        Private Function RandomLibraries(n As Integer, seed As Integer) As Double()
            Dim rng As New Random(seed)
            Dim result(n - 1) As Double

            For i As Integer = 0 To n - 1
                result(i) = 5000.0 + rng.NextDouble() * 20000.0
            Next

            Return result
        End Function

        Private Function RandomCounts(g As Integer, n As Integer, seed As Integer) As Integer()
            Dim rng As New Random(seed)
            Dim result(g * n - 1) As Integer

            For i As Integer = 0 To result.Length - 1
                result(i) = rng.Next(0, 500)
            Next

            Return result
        End Function

        ''' <summary>按 ctrl 的并列结构算出的 Σ(t³ - t)（与内核同一口径）。</summary>
        Private Function TieSums(g As Integer, m As Integer, seed As Integer) As Double()
            Dim ctrl As Single() = SortedControl(g, m, seed)
            Dim result(g - 1) As Double

            For item As Integer = 0 To g - 1
                Dim acc As Double = 0.0
                Dim i As Integer = 0

                While i < m
                    Dim v As Single = ctrl(item * m + i)
                    Dim j As Integer = i + 1

                    While j < m AndAlso ctrl(item * m + j) = v
                        j += 1
                    End While

                    Dim t As Double = j - i

                    acc += t * t * t - t
                    i = j
                End While

                result(item) = acc
            Next

            Return result
        End Function
    End Module
End Namespace
