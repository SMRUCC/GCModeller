Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Design
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Evidence
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Model
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Planning
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Synthesis
Imports std = System.Math

''' <summary>
''' M4 总量设计审计：剖面离散度、边距/幅度求解、信用抽样、最大余数分摊、
''' 控制主成分，以及带 mock 合成的固定点循环。
''' </summary>
Public Module M4DesignAudit

    Private Const ReportName As String = "cellularai-m4-report.json"

    Public Function Run() As Integer
        Dim reportPath As String = Path.Combine(AppContext.BaseDirectory, ReportName)

        Call DebugAudit.Reset()
        DebugAudit.MaxEvents = 8192
        Call DebugAudit.Mark("cellularai-m4 audit started")

        Try
            Call CheckProfileSpread()
            Call CheckMarginSolver()
            Call CheckBisectAmp()
            Call CheckCredit()
            Call CheckApportion()
            Call CheckControlPcs()
            Call CheckTotalsDesign()
        Catch ex As Exception
            Call DebugAudit.Mark("m4.audit.crash", ex.ToString())
            Call DebugAudit.Test(False, $"the M4 audit should not throw unhandled exceptions: {ex.Message}")
        End Try

        Call DebugAudit.SaveReport(reportPath)
        Call Console.WriteLine($"   [audit] report -> {reportPath}")
        Call Console.WriteLine($"   [audit] {DebugAudit.FailureCount} failure(s)")

        Return DebugAudit.FailureCount
    End Function

#Region "cases"

    ''' <summary>离散度只看形状：整体缩放不变、均匀剖面为零、自身靶被屏蔽。</summary>
    Private Sub CheckProfileSpread()
        Dim nGenes As Integer = 3
        Dim nPerts As Integer = 2
        Dim mask As Boolean()() = ProfileSpread.OwnMask(nGenes, nPerts, New Integer() {0, 1})
        Dim uniform As Double()() = {New Double() {1.0, 1.0}, New Double() {1.0, 1.0}, New Double() {1.0, 1.0}}

        Call DebugAudit.Test(std.Abs(ProfileSpread.FromFractions(uniform, mask)) < 1e-15,
                             "a flat profile has zero spread")

        Dim shaped As Double()() = {New Double() {0.6, 0.2}, New Double() {0.3, 0.5}, New Double() {0.1, 0.3}}
        Dim baseSpread As Double = ProfileSpread.FromFractions(shaped, mask)

        Call DebugAudit.Mark("m4.spread_base", baseSpread)
        DebugAudit.Test(baseSpread > 0.0, "a shaped profile has positive spread")

        ' 按列放大 3 倍后重新归一化（比例不变）→ 离散度不变
        Dim scaled(nGenes - 1)() As Double

        For g As Integer = 0 To nGenes - 1
            scaled(g) = New Double() {shaped(g)(0) * 3.0, shaped(g)(1) * 3.0}
        Next

        Call NormalizeColumns(scaled, nGenes, nPerts)

        Dim scaledSpread As Double = ProfileSpread.FromFractions(scaled, mask)

        Call DebugAudit.Mark("m4.spread_scaled", scaledSpread)
        DebugAudit.Test(std.Abs(scaledSpread - baseSpread) < 1e-12,
                        $"scaling a column should not change the spread, {scaledSpread:F12} vs {baseSpread:F12}")

        ' 自身靶屏蔽：只改被屏蔽的那一格，离散度必须完全不变
        Dim withTarget(nGenes - 1)() As Double

        For g As Integer = 0 To nGenes - 1
            withTarget(g) = DirectCast(shaped(g).Clone(), Double())
        Next

        withTarget(0)(0) = 0.99
        withTarget(1)(1) = 0.99

        Dim maskedSpread As Double = ProfileSpread.FromFractions(withTarget, mask)

        Call DebugAudit.Mark("m4.spread_masked", maskedSpread)
        DebugAudit.Test(std.Abs(maskedSpread - baseSpread) < 1e-12,
                        $"changing a masked cell must not change the spread, {maskedSpread:F12} vs {baseSpread:F12}")

        ' 权重与掩码
        Call DebugAudit.AreEqual(1.0 / 2.0, ProfileSpread.Weights(nGenes, New Integer() {0, 1})(0),
                                 "the weight excludes the own target gene")
        Call DebugAudit.AreEqual(1.0 / 3.0, ProfileSpread.Weights(nGenes, New Integer() {-1, -1})(0),
                                 "a perturbation without a target gene keeps the full gene count")
    End Sub

    ''' <summary>边距：近线性目标三步内命中；不可达时夹紧并标记。</summary>
    Private Sub CheckMarginSolver()
        Dim calls As Integer = 0
        Dim realized As Func(Of Double, Double) =
            Function(m As Double)
                calls += 1
                Return 3.0 * m
            End Function

        Dim solution As MarginSolution = MarginSolver.SolveMargin(realized, 6.0, 1.0)

        Call DebugAudit.Mark("m4.margin", solution.Margin)
        Call DebugAudit.Mark("m4.margin_calls", calls)
        DebugAudit.Test(solution.Reached, "a linear target should be reached")
        DebugAudit.Test(std.Abs(solution.Margin - 2.0) < 1e-6, $"the margin should be 2, got {solution.Margin:F9}")
        DebugAudit.Test(calls <= 4, $"the secant should converge in a few steps, took {calls}")

        Dim flat As Func(Of Double, Double) = Function(m As Double) 1.0
        Dim unreachable As MarginSolution = MarginSolver.SolveMargin(flat, 1000.0, 1.0)

        Call DebugAudit.Mark("m4.margin_clamped", unreachable.Margin)
        DebugAudit.Test(Not unreachable.Reached, "an unreachable target must be flagged")
        DebugAudit.Test(unreachable.Margin > 0.0, "the clamped margin stays positive")
    End Sub

    ''' <summary>幅度：单调目标能命中；上限不足时标记 capped。</summary>
    Private Sub CheckBisectAmp()
        Dim spread As Func(Of Double()(), Double) =
            Function(values As Double()()) values(0)(0)
        Dim design As Func(Of Double, Double()()) =
            Function(a As Double) New Double()() {New Double() {a}}

        Dim solution As AmplitudeSolution = MarginSolver.BisectAmp(spread, design, 2.0)

        Call DebugAudit.Mark("m4.amplitude", solution.Amplitude)
        DebugAudit.Test(std.Abs(solution.Amplitude - 2.0) < 1e-4, $"the amplitude should be 2, got {solution.Amplitude:F9}")
        DebugAudit.Test(Not solution.Capped, "2x is inside the 64x cap")

        Dim capped As AmplitudeSolution = MarginSolver.BisectAmp(spread, design, 128.0)

        Call DebugAudit.Mark("m4.amplitude_capped", capped.Amplitude)
        DebugAudit.Test(capped.Capped, "a target above 64x must be flagged as capped")
        DebugAudit.Test(capped.Amplitude >= 64.0 - 1e-9, "the capped amplitude stops at 64")
    End Sub

    ''' <summary>信用抽样与期望信用。</summary>
    Private Sub CheckCredit()
        Dim task As SyntheticTask = SyntheticFixture.Create()
        Dim options As New TotalsOptions With {.CreditDraws = 8, .CreditSeed = 7}
        Dim draws As Double() = MarginSolver.ControlMoments(task.Controls(0), options, 60)

        Call DebugAudit.AreEqual(8, draws.Length, "one moment per draw")

        Dim finite As Boolean = True

        For Each draw As Double In draws
            If Not Double.IsFinite(draw) Then
                finite = False
            End If
        Next

        DebugAudit.Test(finite, "every draw is finite")
        Call DebugAudit.Mark("m4.credit_mean", draws.Average)
        DebugAudit.Test(draws.Average > 0.0, "the control moments are positive")

        ' cHat = ∞ → 期望信用退化为 Σ w·mean(draws)
        Dim infinity As Double() = {Double.PositiveInfinity, Double.PositiveInfinity}
        Dim weights As Double() = {0.5, 0.5}
        Dim expected As Double = MarginSolver.ExpectedCredit(infinity, draws, weights)
        Dim reference As Double = 0.5 * 2.0 * draws.Average

        Call DebugAudit.Test(std.Abs(expected - reference) < 1e-12,
                             $"cHat = inf should reduce to the mean draw, {expected:F12} vs {reference:F12}")

        ' cHat = 0 → 期望信用为 0
        Call DebugAudit.AreEqual(0.0, MarginSolver.ExpectedCredit(New Double() {0.0, 0.0}, draws, weights),
                                 "a zero moment gives zero credit")
    End Sub

    ''' <summary>最大余数分摊：总量精确、比例近似、确定性。</summary>
    Private Sub CheckApportion()
        Dim fractions As Double() = {0.5, 0.3, 0.2}
        Dim totals As Integer() = TotalsDesign.Apportion(fractions, 1000)

        Call DebugAudit.AreEqual(1000, totals.Sum, "the apportioned totals sum to the target")
        Call DebugAudit.AreEqual(500, totals(0), "the largest fraction takes the largest share")
        Call DebugAudit.AreEqual(300, totals(1), "the second fraction follows")
        Call DebugAudit.AreEqual(200, totals(2), "the remainder is assigned by fractional part")

        ' 非整除时仍然精确求和
        Dim awkward As Integer() = TotalsDesign.Apportion(New Double() {1.0, 1.0, 1.0}, 100)

        Call DebugAudit.AreEqual(100, awkward.Sum, "a non-divisible total is still exact")
        DebugAudit.Test(awkward.Max - awkward.Min <= 1, "the shares differ by at most one")

        ' 全零输入不崩
        Call DebugAudit.AreEqual(0, TotalsDesign.Apportion(New Double() {0.0, 0.0}, 10).Sum,
                                 "a zero fraction vector produces zero totals")
    End Sub

    ''' <summary>控制主成分：秩、正交性、特征值单调。</summary>
    Private Sub CheckControlPcs()
        Dim task As SyntheticTask = SyntheticFixture.Create()
        Dim options As New TotalsOptions With {.PwK = 3, .PcsCells = 60, .PcsSeed = 0}
        Dim pcs As ControlPcs = ControlPcs.Compute(task.Controls(0), options)

        Call DebugAudit.AreEqual(3, pcs.Rank, "three components are requested")
        Call DebugAudit.AreEqual(3, pcs.Eigenvalues.Length, "one eigenvalue per component")
        Call DebugAudit.AreEqual(task.GeneCount, pcs.Loadings(0).Length, "the loadings live in gene space")

        DebugAudit.Test(pcs.Eigenvalues(0) >= pcs.Eigenvalues(1), "the eigenvalues are sorted")
        DebugAudit.Test(pcs.Eigenvalues(2) >= 0.0, "the eigenvalues are non-negative")

        ' V Vᵀ ≈ I
        Dim worst As Double = 0.0

        For a As Integer = 0 To 2
            For b As Integer = 0 To 2
                Dim acc As Double = 0.0

                For g As Integer = 0 To task.GeneCount - 1
                    acc += pcs.Loadings(a)(g) * pcs.Loadings(b)(g)
                Next

                If a = b Then
                    acc -= 1.0
                End If

                worst = std.Max(worst, std.Abs(acc))
            Next
        Next

        Call DebugAudit.Mark("m4.pcs_orthogonality", worst)
        DebugAudit.Test(worst < 1e-9, $"the loadings should be orthonormal, worst = {worst:E3}")
    End Sub

    ''' <summary>端到端：整数总量守恒、固定点循环收敛、报告完整。</summary>
    Private Sub CheckTotalsDesign()
        Dim task As SyntheticTask = SyntheticFixture.Create()
        Dim model As CellularAiModel = CellularAiModel.Default()
        Dim engine As SynthesisEngine = SynthesisEngine.Create(task.Controls(0), model)
        Dim evidence As ContextEvidence = MetaPooling.Pool(task.Bundle, task.Genes, task.Perturbations,
                                                          ControlCpm(task), model)

        Dim effects As New DesignEffects With {
            .L2 = evidence.L2,
            .L2Tot = evidence.L2Tot,
            .Z = evidence.Z,
            .ZTot = evidence.ZTot,
            .Profile = evidence.L2
        }

        ' 每个扰动只调用 2 个基因，靶基因下标从基因轴上查
        Dim tidx(task.PerturbationCount - 1) As Integer
        Dim calls(task.PerturbationCount - 1) As CallEntry

        For p As Integer = 0 To task.PerturbationCount - 1
            Dim target As Integer = Array.IndexOf(task.Genes, task.Perturbations(p))

            tidx(p) = target

            Dim picked As Integer() = Enumerable.Range(0, task.GeneCount).
                Where(Function(g) g <> target).Take(2).ToArray

            calls(p) = New CallEntry With {
                .Index = picked,
                .Sign = picked.Select(Function(g) If(evidence.L2(g)(p) < 0.0, -1.0, 1.0)).ToArray,
                .Priority = {1.0, 0.5},
                .NudgeSign = New SByte(task.GeneCount - 1) {},
                .NudgeStrength = New Double(task.GeneCount - 1) {},
                .NCalls = picked.Length
            }
        Next

        Dim nCells As Integer = 20
        Dim credit As Double = task.PerturbationCount * 0.0

        Dim design As New TotalsDesign(task.Controls(0), engine, effects, calls, tidx,
                                       credit, model, nCells)

        ' 幅度搜索在小信用下应可达
        Call DebugAudit.Mark("m4.amplitude_design", design.Amplitude)
        DebugAudit.Test(design.Amplitude > 0.0, "the amplitude design is positive")
        Call DebugAudit.AreEqual(nCells * engine.Depth, design.TotalCounts, "the total equals n_cells x depth")

        ' 整数总量：每行精确求和、非负
        Dim shaped As ShapedDesignResult = design.Shaped(1.0, 1.0)
        Dim totals As Integer()() = design.Totals(shaped.L)

        Call DebugAudit.AreEqual(task.PerturbationCount, totals.Length, "one total vector per perturbation")

        For p As Integer = 0 To totals.Length - 1
            Dim sum As Integer = totals(p).Sum

            Call DebugAudit.AreEqual(design.TotalCounts, sum, $"perturbation #{p} totals to n_cells x depth")
            DebugAudit.Test(totals(p).All(Function(v) v >= 0), $"perturbation #{p} has no negative total")
        Next

        ' 固定点循环（mock 合成：把总量平均分给 nCells 个细胞）
        Dim synthesize As Func(Of Double()(), CsrMatrix()) =
            Function(t As Double()())
                Dim blocks(t.Length - 1) As CsrMatrix

                For p As Integer = 0 To t.Length - 1
                    Dim dense As Double()() = RectangularArray.Matrix(Of Double)(nCells, task.GeneCount)

                    For c As Integer = 0 To nCells - 1
                        For g As Integer = 0 To task.GeneCount - 1
                            dense(c)(g) = std.Floor(t(p)(g) / nCells)
                        Next
                    Next

                    For g As Integer = 0 To task.GeneCount - 1
                        dense(0)(g) += t(p)(g) - nCells * std.Floor(t(p)(g) / nCells)
                    Next

                    blocks(p) = CompressedBuilder.FromDense(dense)
                Next

                Return blocks
            End Function

        Dim draws As Double() = MarginSolver.ControlMoments(task.Controls(0), model.Totals, nCells)
        Dim solved = design.Solve(task.PerturbationCount * draws.Average, draws, synthesize)

        Call DebugAudit.AreEqual(task.PerturbationCount, solved.Blocks.Length, "one block per perturbation")
        Call DebugAudit.Mark("m4.margin_solved", solved.Report.Margin)
        Call DebugAudit.Mark("m4.margin_share", solved.Report.CreditShare)
        DebugAudit.Test(solved.Report.Margin > 0.0, "the solved margin is positive")
        Call DebugAudit.AreEqual(1, solved.Report.Targets.Length, "the fixed point converges in one pass on a mock")

        For p As Integer = 0 To solved.Blocks.Length - 1
            Call DebugAudit.AreEqual(nCells, solved.Blocks(p).RowDimension, $"block #{p} has n_cells rows")
            Call DebugAudit.AreEqual(task.GeneCount, solved.Blocks(p).ColumnDimension, $"block #{p} covers every gene")
        Next
    End Sub

#End Region

#Region "helpers"

    ''' <summary>按列归一化（原地）。</summary>
    Private Sub NormalizeColumns(fractions As Double()(), nGenes As Integer, nPerts As Integer)
        For p As Integer = 0 To nPerts - 1
            Dim total As Double = 0.0

            For g As Integer = 0 To nGenes - 1
                total += fractions(g)(p)
            Next

            If total <= 0.0 Then
                Continue For
            End If

            For g As Integer = 0 To nGenes - 1
                fractions(g)(p) /= total
            Next
        Next
    End Sub

    Private Function ControlCpm(task As SyntheticTask) As Double()
        Dim counts As Double() = task.Controls(0).ColumnSums()
        Dim total As Double = 0.0

        For i As Integer = 0 To counts.Length - 1
            total += counts(i)
        Next

        If total <= 0.0 Then
            total = 1.0
        End If

        Dim mean As Double() = New Double(counts.Length - 1) {}

        For i As Integer = 0 To counts.Length - 1
            mean(i) = 1000000.0 * counts(i) / total
        Next

        Return mean
    End Function

#End Region

End Module
