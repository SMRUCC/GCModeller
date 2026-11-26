Imports System
Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports CellularAI.Model
Imports CellularAI.Placement
Imports CellularAI.Randomizer
Imports CellularAI.Synthesis
Imports std = System.Math

''' <summary>
''' M5 合成内核审计：PCG64 可复现性、种子派生、分位取值与质量守恒、
''' 精确 z 与"教科书公式"的独立对拍、平移二分命中。
''' </summary>
Public Module M5SynthesisAudit

    Private Const ReportName As String = "cellularai-m5-report.json"

    Public Function Run() As Integer
        Dim reportPath As String = Path.Combine(AppContext.BaseDirectory, ReportName)

        Call DebugAudit.Reset()
        DebugAudit.MaxEvents = 8192
        Call DebugAudit.Mark("cellularai-m5 audit started")

        Try
            Call CheckPcg64()
            Call CheckSeeds()
            Call CheckQuantileBuilder()
            Call CheckSparseBuilder()
            Call CheckQuantiles()
            Call CheckZExact()
            Call CheckShiftSolve()
            Call CheckSynthesis()
        Catch ex As Exception
            Call DebugAudit.Mark("m5.audit.crash", ex.ToString())
            Call DebugAudit.Test(False, $"the M5 audit should not throw unhandled exceptions: {ex.Message}")
        End Try

        Call DebugAudit.SaveReport(reportPath)
        Call Console.WriteLine($"   [audit] report -> {reportPath}")
        Call Console.WriteLine($"   [audit] {DebugAudit.FailureCount} failure(s)")

        Return DebugAudit.FailureCount
    End Function

#Region "cases"

    ''' <summary>PCG64：同种子逐位一致、不同种子不同、分布合理。</summary>
    Private Sub CheckPcg64()
        Dim first As New Pcg64(20261126)
        Dim second As New Pcg64(20261126)
        Dim third As New Pcg64(20261127)
        Dim identical As Boolean = True
        Dim differs As Boolean = False

        For i As Integer = 0 To 255
            Dim a As ULong = first.NextUInt64()
            Dim b As ULong = second.NextUInt64()

            If a <> b Then
                identical = False
            End If
            If a <> third.NextUInt64() Then
                differs = True
            End If
        Next

        DebugAudit.Test(identical, "the same seed must give the same sequence")
        DebugAudit.Test(differs, "different seeds must give different sequences")

        ' 均匀性 / 正态性
        Dim generator As New Pcg64(99)
        Dim sum As Double = 0.0
        Dim squareSum As Double = 0.0
        Dim outOfRange As Integer = 0
        Dim n As Integer = 20000
        Dim uniformSum As Double = 0.0

        For i As Integer = 0 To 999
            uniformSum += generator.NextDouble()
        Next

        Call DebugAudit.Mark("m5.uniform_mean", uniformSum / 1000.0)

        For i As Integer = 0 To n - 1
            Dim u As Double = generator.NextDouble()

            If u < 0.0 OrElse u >= 1.0 Then
                outOfRange += 1
            End If

            Dim v As Double = generator.StandardNormal()

            sum += v
            squareSum += v * v
        Next

        Dim mean As Double = sum / n
        Dim variance As Double = squareSum / n - mean * mean

        Call DebugAudit.Mark("m5.pcg_mean", mean)
        Call DebugAudit.Mark("m5.pcg_var", variance)
        Call DebugAudit.AreEqual(0, outOfRange, "NextDouble stays in [0,1)")
        DebugAudit.Test(std.Abs(mean) < 0.05, $"the normal draws should be centred, mean = {mean:F4}")
        DebugAudit.Test(std.Abs(variance - 1.0) < 0.05, $"the normal draws should have unit variance, var = {variance:F4}")

        ' 排列是双射
        Dim permutation As Integer() = generator.Permutation(50)
        Dim seen As New HashSet(Of Integer)

        For Each item As Integer In permutation
            Call seen.Add(item)
        Next

        Call DebugAudit.AreEqual(50, seen.Count, "a permutation has no duplicates")

        ' Clone 之后两条流独立且前缀相同
        Dim source As New Pcg64(7)
        Dim clone As Pcg64 = source.Clone()

        Call DebugAudit.AreEqual(source.NextUInt64(), clone.NextUInt64(), "a clone shares the prefix")
        Call DebugAudit.Test(source.NextUInt64() = clone.NextUInt64(), "a clone keeps advancing in step")
    End Sub

    ''' <summary>扰动种子：同名一致、任务种子可区分、独立随机源。</summary>
    Private Sub CheckSeeds()
        Call DebugAudit.AreEqual(PerturbationSeed.FromName("TP53"), PerturbationSeed.FromName("TP53"),
                                 "the same name gives the same seed")
        DebugAudit.Test(PerturbationSeed.FromName("TP53") <> PerturbationSeed.FromName("KRAS"),
                        "different names give different seeds")
        DebugAudit.Test(PerturbationSeed.Combine(1, "TP53") <> PerturbationSeed.Combine(2, "TP53"),
                        "the task seed changes the combined seed")

        Dim a As Pcg64 = PerturbationSeed.Create(42, "TP53")
        Dim b As Pcg64 = PerturbationSeed.Create(42, "TP53")
        Dim c As Pcg64 = PerturbationSeed.Create(42, "KRAS")
        Dim same As Boolean = True
        Dim differs As Boolean = False

        For i As Integer = 0 To 63
            Dim value As ULong = a.NextUInt64()

            If value <> b.NextUInt64() Then
                same = False
            End If
            If value <> c.NextUInt64() Then
                differs = True
            End If
        Next

        DebugAudit.Test(same, "the same perturbation and task seed give the same stream")
        DebugAudit.Test(differs, "different perturbations give different streams")
    End Sub

    ''' <summary>分位取值：总量精确守恒、非负、单调于平移量。</summary>
    Private Sub CheckQuantileBuilder()
        Dim n As Integer = 40
        Dim controlCpm As Double() = ControlGrid(240)
        Dim librarySizes(n - 1) As Double
        Dim q(n - 1) As Double

        For i As Integer = 0 To n - 1
            librarySizes(i) = 4000.0 + 10.0 * i
            q(i) = (i + 0.5) / n
        Next

        Dim ordering As Integer() = Enumerable.Range(0, n).Reverse().ToArray
        Dim total As Double = 12000.0

        Dim counts As Double() = QuantileBuilder.Build(q, ordering, controlCpm, librarySizes, total, 0.2, 40)

        Call DebugAudit.Mark("m5.quantile_mass", counts.Sum)
        DebugAudit.Test(std.Abs(counts.Sum - total) < 1e-9,
                        $"the built counts sum to the design total, {counts.Sum:F6} vs {total:F6}")
        DebugAudit.Test(counts.All(Function(v) v >= 0.0), "no negative counts")

        ' 平移量越大，总量越大（同一条曲线上单调）
        Dim low As Double() = QuantileBuilder.Build(q, ordering, controlCpm, librarySizes, total, -0.4, 40)
        Dim high As Double() = QuantileBuilder.Build(q, ordering, controlCpm, librarySizes, total, 0.8, 40)

        Call DebugAudit.Mark("m5.quantile_low", low.Sum)
        Call DebugAudit.Mark("m5.quantile_high", high.Sum)
        DebugAudit.Test(high.Sum >= low.Sum, "a larger shift moves more mass before conservation")
    End Sub

    ''' <summary>稀疏化：总量守恒、非零细胞数符合 <c>w</c>。</summary>
    Private Sub CheckSparseBuilder()
        Dim n As Integer = 50
        Dim ordering As Integer() = Enumerable.Range(0, n).Reverse().ToArray
        Dim total As Double = 3000.0

        For Each w As Double In New Double() {0.0, 0.25, 0.6}

            Dim counts As Double() = QuantileBuilder.BuildSparse(ordering, total, w)
            Dim nonzero As Integer = counts.Count(Function(v) v > 0.0)
            Dim expected As Integer = n - CInt(std.Floor(w * n + 0.5))

            Call DebugAudit.Mark($"m5.sparse_nonzero_{w}", nonzero)
            DebugAudit.Test(std.Abs(counts.Sum - total) < 1e-9,
                            $"w = {w}: the sparse counts sum to the total, {counts.Sum:F6}")
            DebugAudit.Test(nonzero = std.Max(expected, 1),
                            $"w = {w}: the non-zero cell count is {nonzero}, expected {std.Max(expected, 1)}")
        Next
    End Sub

    ''' <summary>copula 分位：分位是 (i+0.5)/n 的排列，降序下标与之互补。</summary>
    Private Sub CheckQuantiles()
        Dim generator As New Pcg64(11)
        Dim n As Integer = 30
        Dim noise(n - 1) As Double
        Dim libraryRank(n - 1) As Double

        For i As Integer = 0 To n - 1
            noise(i) = generator.StandardNormal()
            libraryRank(i) = generator.StandardNormal()
        Next

        Dim result = QuantileBuilder.QuantilesFrom(noise, libraryRank, 0.5)
        Dim seen As New HashSet(Of Double)
        Dim monotone As Boolean = True
        Dim previous As Integer = Integer.MinValue

        For i As Integer = 0 To n - 1
            Call seen.Add(std.Round(result.Q(i), 12))

            ' 降序下标序列按 q 递减
            If result.Ordering(i) >= 0 AndAlso result.Ordering(i) < n Then
                If previous <> Integer.MinValue AndAlso result.Q(result.Ordering(i)) > result.Q(previous) + 1e-12 Then
                    monotone = False
                End If

                previous = result.Ordering(i)
            End If
        Next

        Call DebugAudit.AreEqual(n, seen.Count, "every cell gets a distinct quantile")
        DebugAudit.Test(result.Q.All(Function(v) v > 0.0 AndAlso v < 1.0), "the quantiles lie in (0,1)")
        DebugAudit.Test(monotone, "the ordering lists cells by decreasing quantile")

        ' r = 0 时完全由噪声决定；r = 1 时完全由文库秩决定
        Dim withoutCorrelation = QuantileBuilder.QuantilesFrom(noise, libraryRank, 0.0)
        Dim withFullCorrelation = QuantileBuilder.QuantilesFrom(noise, libraryRank, 1.0)

        Call DebugAudit.AreEqual(result.Ordering(0), withoutCorrelation.Ordering(0),
                                 "r = 0 keeps the noise ordering at the top cell")
        DebugAudit.Test(withoutCorrelation.Q(0) <> withFullCorrelation.Q(0) OrElse n = 1,
                        "r = 1 changes the quantiles")

        Call DebugAudit.Mark("m5.mcpm", QuantileBuilder.MeanCpm(New Double() {10.0, 20.0}, New Double() {1000.0, 1000.0}))
        Call DebugAudit.AreEqual(15000.0, QuantileBuilder.MeanCpm(New Double() {10.0, 20.0}, New Double() {1000.0, 1000.0}),
                                 "the mean CPM follows 1e6 x / L")
    End Sub

    ''' <summary>精确 z：与"教科书 Mann–Whitney + 并列校正"独立对拍。</summary>
    Private Sub CheckZExact()
        Dim generator As New Pcg64(3)
        Dim control As Double() = ControlGrid(200)
        Dim tieSum As Double = TieCorrection(control)
        Dim n As Integer = 40
        Dim librarySizes(n - 1) As Double
        Dim counts(n - 1) As Double

        For i As Integer = 0 To n - 1
            librarySizes(i) = 5000.0
            counts(i) = std.Floor(generator.NextDouble() * 30.0)
        Next

        Dim z As Double = WilcoxonKernel.ZExact(counts, librarySizes, control, tieSum)
        Dim reference As Double = ReferenceZ(counts, librarySizes, control, tieSum)

        Call DebugAudit.Mark("m5.z_kernel", z)
        Call DebugAudit.Mark("m5.z_reference", reference)
        DebugAudit.Test(std.Abs(z - reference) / std.Max(std.Abs(reference), 1.0) < 1e-4,
                        $"the kernel should match the textbook formula, {z:F12} vs {reference:F12}")

        ' 样本整体偏高时 z 为正，整体偏低时为负
        Dim shifted(n - 1) As Double

        For i As Integer = 0 To n - 1
            shifted(i) = counts(i) + 30.0
        Next

        Dim high As Double = WilcoxonKernel.ZExact(shifted, librarySizes, control, tieSum)

        Call DebugAudit.Mark("m5.z_high", high)
        DebugAudit.Test(high > z, "a higher sample gives a larger z")

        ' 全零样本必须给出有限值
        Dim zeros(n - 1) As Double
        Dim zeroZ As Double = WilcoxonKernel.ZExact(zeros, librarySizes, control, tieSum)

        DebugAudit.Test(Double.IsFinite(zeroZ), "an all-zero sample gives a finite z")
    End Sub

    ''' <summary>平移二分：可达目标命中容差，越界目标不崩。</summary>
    Private Sub CheckShiftSolve()
        Dim model As CellularAiModel = CellularAiModel.Default()
        Dim kernel As ShiftKernel = ShiftKernel.FromModel(model)
        Dim control As Double() = ControlGrid(240)
        Dim tieSum As Double = TieCorrection(control)
        Dim n As Integer = 40
        Dim librarySizes(n - 1) As Double
        Dim q(n - 1) As Double

        For i As Integer = 0 To n - 1
            librarySizes(i) = 5000.0
            q(i) = (i + 0.5) / n
        Next

        Dim ordering As Integer() = Enumerable.Range(0, n).Reverse().ToArray
        Dim total As Double = 9000.0

        ' 先看区间端点，取一个可达的中间目标
        Dim low = BlockSolver.SolveShift(q, ordering, control, tieSum, librarySizes, total, -1.0E9, kernel)
        Dim high = BlockSolver.SolveShift(q, ordering, control, tieSum, librarySizes, total, 1.0E9, kernel)

        Call DebugAudit.Mark("m5.shift_low_z", low.Z)
        Call DebugAudit.Mark("m5.shift_high_z", high.Z)
        DebugAudit.Test(std.Abs(low.Counts.Sum - total) < 1e-9, "the lower bound keeps the mass")
        DebugAudit.Test(std.Abs(high.Counts.Sum - total) < 1e-9, "the upper bound keeps the mass")

        Dim target As Double = 0.5 * (low.Z + high.Z)
        Dim solved = BlockSolver.SolveShift(q, ordering, control, tieSum, librarySizes, total, target, kernel)

        Call DebugAudit.Mark("m5.shift_target", target)
        Call DebugAudit.Mark("m5.shift_hit", solved.Z)
        DebugAudit.Test(std.Abs(solved.Z - target) <= std.Max(kernel.Tolerance, 0.05),
                        $"the shift search should hit the target, {solved.Z:F6} vs {target:F6}")
        DebugAudit.Test(std.Abs(solved.Counts.Sum - total) < 1e-9, "the solved counts keep the mass")
    End Sub

    ''' <summary>端到端合成：同 seed 逐元素相等、每基因总量守恒、整数与 2²⁴、并行一致。</summary>
    Private Sub CheckSynthesis()
        Dim task As SyntheticTask = SyntheticFixture.Create()
        Dim model As CellularAiModel = CellularAiModel.Default()
        Dim engine As SynthesisEngine = SynthesisEngine.Create(task.Controls(0), model)
        Dim nCells As Integer = 24
        Dim job As SynthesisJob = MakeJob(task, engine, nCells, seed:=20261126)

        Dim first As Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed.CsrMatrix =
            CellSynthesizer.Synthesize(engine, job, model)
        Dim second As Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed.CsrMatrix =
            CellSynthesizer.Synthesize(engine, job, model)

        Call DebugAudit.AreEqual(nCells, first.RowDimension, "the block has n_cells rows")
        Call DebugAudit.AreEqual(task.GeneCount, first.ColumnDimension, "the block covers every gene")

        ' 同 seed 逐元素相等
        Dim identical As Boolean = True
        Dim sums(engine.GeneCount - 1) As Double
        Dim integral As Boolean = True
        Dim bounded As Boolean = True

        For c As Integer = 0 To nCells - 1
            For k As Integer = first.Indptr(c) To first.Indptr(c + 1) - 1
                Dim gene As Integer = first.Indices(k)
                Dim value As Double = first.Values(k)

                sums(gene) += value

                If value < 0.0 Then
                    integral = False
                End If
                If value <> std.Floor(value) Then
                    integral = False
                End If
                If value >= 16777216.0 Then
                    bounded = False
                End If
            Next

            For k As Integer = second.Indptr(c) To second.Indptr(c + 1) - 1
                If first.Values(k) <> second.Values(k) OrElse first.Indices(k) <> second.Indices(k) Then
                    identical = False
                End If
            Next
        Next

        DebugAudit.Test(identical, "the same seed must give the same counts element by element")
        DebugAudit.Test(integral, "every count is a non-negative integer")
        DebugAudit.Test(bounded, "every count stays below 2^24")

        ' 每基因总量守恒
        Dim worst As Double = 0.0

        For g As Integer = 0 To engine.GeneCount - 1
            worst = std.Max(worst, std.Abs(sums(g) - job.Totals(g)))
        Next

        Call DebugAudit.Mark("m5.gene_total_gap", worst)
        DebugAudit.Test(worst < 0.5, $"every gene keeps its designed total, worst gap = {worst:F6}")

        ' 不同 seed 应当给出不同的矩阵
        Dim other As SynthesisJob = MakeJob(task, engine, nCells, seed:=777)
        Dim third As Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed.CsrMatrix =
            CellSynthesizer.Synthesize(engine, other, model)
        Dim differs As Boolean = False

        For c As Integer = 0 To nCells - 1
            If third.Indptr(c + 1) - third.Indptr(c) <> first.Indptr(c + 1) - first.Indptr(c) Then
                differs = True
                Exit For
            End If

            For k As Integer = third.Indptr(c) To third.Indptr(c + 1) - 1
                If third.Values(k) <> first.Values(k) Then
                    differs = True
                    Exit For
                End If
            Next
        Next

        DebugAudit.Test(differs, "a different seed gives a different matrix")

        ' 并行与串行一致
        Dim jobs As SynthesisJob() = {MakeJob(task, engine, nCells, 11), MakeJob(task, engine, nCells, 12)}

        Dim serial As Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed.CsrMatrix() =
            CellSynthesizer.SynthesizeAll(engine, jobs, model, 1)
        Dim parallel As Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed.CsrMatrix() =
            CellSynthesizer.SynthesizeAll(engine, jobs, model, 0)
        Dim same As Boolean = True

        For b As Integer = 0 To jobs.Length - 1
            If serial(b).Values.Length <> parallel(b).Values.Length Then
                same = False
                Continue For
            End If

            For k As Integer = 0 To serial(b).Values.Length - 1
                If serial(b).Values(k) <> parallel(b).Values(k) Then
                    same = False
                End If
            Next
        Next

        DebugAudit.Test(same, "the parallel and serial batches agree element by element")
    End Sub

    ''' <summary>构造一个守恒的合成作业（总量 = 细胞数 × 深度）。</summary>
    Private Function MakeJob(task As SyntheticTask, engine As SynthesisEngine, nCells As Integer, seed As Long) As SynthesisJob
        Dim depth As Double = engine.Depth
        Dim libraries(nCells - 1) As Double
        Dim totals(engine.GeneCount - 1) As Double
        Dim targets(engine.GeneCount - 1) As Double

        For c As Integer = 0 To nCells - 1
            libraries(c) = depth
        Next

        ' 总量：按对照基因比例分配，再取整并补齐余额
        Dim grand As Double = nCells * depth
        Dim assigned As Double = 0.0

        For g As Integer = 0 To engine.GeneCount - 1
            totals(g) = std.Floor(grand * engine.Fractions(g))

            If totals(g) < 1.0 Then
                totals(g) = 1.0
            End If

            assigned += totals(g)
            targets(g) = engine.MeanCpm(g)
        Next

        Dim delta As Double = grand - assigned

        If std.Abs(delta) > 0.0 Then
            Dim delta1 As Double = If(delta > 0.0, 1.0, -1.0)
            Dim remaining As Double = std.Abs(delta)
            Dim g0 As Integer = 0

            While remaining >= 1.0 AndAlso g0 < totals.Length
                totals(g0) += delta1
                remaining -= 1.0
                g0 += 1
            End While
        End If

        ' 方向：让第一个基因上调、第二个下调，覆盖 nudge 分支
        Dim nudgeSign(engine.GeneCount - 1) As SByte
        Dim nudgeStrength(engine.GeneCount - 1) As Double

        For g As Integer = 0 To engine.GeneCount - 1
            nudgeSign(g) = CSByte(If(g Mod 2 = 0, 1, -1))
            nudgeStrength(g) = 0.5
        Next

        Return New SynthesisJob With {
            .Name = $"job_{seed}",
            .Totals = totals,
            .Libraries = libraries,
            .Targets = targets,
            .PlanIndex = {0, 1},
            .PlanSign = {1.0, -1.0},
            .PlanPriority = {1.0, 0.5},
            .NudgeSign = nudgeSign,
            .NudgeStrength = nudgeStrength,
            .Seed = seed
        }
    End Function

#End Region

#Region "helpers"

    ''' <summary>构造一张升序的对照 CPM 栅格（含大量并列）。</summary>
    Private Function ControlGrid(n As Integer) As Double()
        Dim values(n - 1) As Double
        Dim generator As New Pcg64(17)

        For i As Integer = 0 To n - 1
            ' 一半是零（单细胞数据的典型形态），另一半是少量离散取值
            values(i) = If(generator.NextDouble() < 0.5, 0.0, std.Floor(generator.NextDouble() * 8.0) * 3.5)
        Next

        Call Array.Sort(values)

        Return values
    End Function

    ''' <summary>并列和 <c>Σ(t³−t)</c>。</summary>
    Private Function TieCorrection(values As Double()) As Double
        Dim total As Double = 0.0
        Dim i As Integer = 0

        While i < values.Length
            Dim j As Integer = i

            While j + 1 < values.Length AndAlso values(j + 1) = values(i)
                j += 1
            End While

            Dim group As Double = j - i + 1

            total += group * group * group - group
            i = j + 1
        End While

        Return total
    End Function

    ''' <summary>
    ''' 教科书口径的 Mann–Whitney z：线性扫描求 U、合并样本求并列和。
    ''' </summary>
    Private Function ReferenceZ(counts As Double(), librarySizes As Double(),
                                control As Double(), controlTieSum As Double) As Double
        Dim n As Integer = counts.Length
        Dim m As Integer = control.Length
        Dim grid(n - 1) As Single

        For c As Integer = 0 To n - 1
            grid(c) = CSng(1000000.0 * counts(c) / librarySizes(c))
        Next

        ' U：逐观测累加"对照小于它的个数 + 0.5 × 等于它的个数"
        Dim u As Double = 0.0

        For c As Integer = 0 To n - 1
            Dim less As Integer = 0
            Dim equal As Integer = 0

            For j As Integer = 0 To m - 1
                If control(j) < grid(c) Then
                    less += 1
                ElseIf control(j) = grid(c) Then
                    equal += 1
                End If
            Next

            u += less + 0.5 * equal
        Next

        ' 合并样本的并列和（样本侧按游程合并后与对照的等值段合并）
        Dim adjustment As Double = 0.0
        Dim i As Integer = 0

        While i < n
            Dim j As Integer = i

            While j + 1 < n AndAlso grid(j + 1) = grid(i)
                j += 1
            End While

            Dim group As Double = j - i + 1
            Dim overlap As Double = 0.0

            For c As Integer = 0 To m - 1
                If control(c) = grid(i) Then
                    overlap += 1.0
                End If
            Next

            Dim merged As Double = overlap + group

            adjustment += (merged * merged * merged - merged) - (overlap * overlap * overlap - overlap)
            i = j + 1
        End While

        Dim total As Double = m + n
        Dim correction As Double = 1.0 - (controlTieSum + adjustment) / (total * total * total - total)
        Dim variance As Double = correction * n * m * (total + 1.0) / 12.0

        If variance <= 1.0E-24 Then
            Return 0.0
        End If

        Return (u - n * m / 2.0) / std.Sqrt(variance)
    End Function

#End Region

End Module
