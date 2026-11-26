Imports System
Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed
Imports CellularAI
Imports CellularAI.Model
Imports CellularAI.Scoring
Imports CellularAI.Synthesis
Imports std = System.Math

''' <summary>
''' M6 评分审计：完美预测六项全优、噪声预测退化、锚点缩放的方向与钳制。
''' </summary>
Public Module M6ScoringAudit

    Private Const ReportName As String = "cellularai-m6-report.json"

    Public Function Run() As Integer
        Dim reportPath As String = Path.Combine(AppContext.BaseDirectory, ReportName)

        Call DebugAudit.Reset()
        DebugAudit.MaxEvents = 4096
        Call DebugAudit.Mark("cellularai-m6 audit started")

        Try
            Call CheckPerfectPrediction()
            Call CheckNoiseDegrades()
            Call CheckAnchorScaling()
        Catch ex As Exception
            Call DebugAudit.Mark("m6.audit.crash", ex.ToString())
            Call DebugAudit.Test(False, $"the M6 audit should not throw unhandled exceptions: {ex.Message}")
        End Try

        Call DebugAudit.SaveReport(reportPath)
        Call Console.WriteLine($"   [audit] report -> {reportPath}")
        Call Console.WriteLine($"   [audit] {DebugAudit.FailureCount} failure(s)")

        Return DebugAudit.FailureCount
    End Function

#Region "cases"

    ''' <summary>预测与参考完全一致时，六项指标全部取到最优。</summary>
    Private Sub CheckPerfectPrediction()
        Dim task As SyntheticTask = SyntheticFixture.Create()
        Dim model As CellularAiModel = CellularAiModel.Default()
        Dim engine As SynthesisEngine = SynthesisEngine.Create(task.Controls(0), model)
        Dim blocks As CsrMatrix() = MakeBlocks(task, engine, nCells:=30)

        Dim reference As SideStats = Metrics.BuildSide(blocks, task.Perturbations, task.Genes,
                                                       Function(g As Integer) engine.SortedCpm(g),
                                                       Function(g As Integer) engine.TieSum(g))

        Dim scores As Dictionary(Of String, Double) = Metrics.Score(reference, reference, task.Perturbations)

        For Each name As String In New String() {"pds", "expr_mse", "fid", "reach", "jac"}
            Call DebugAudit.Mark($"m6.perfect_{name}", scores(name))
            DebugAudit.Test(std.Abs(scores(name) - 1.0) < 1e-9, $"a perfect prediction scores 1 on {name}, got {scores(name):F9}")
        Next

        Call DebugAudit.Mark("m6.perfect_nmae", scores("nmae"))
        DebugAudit.Test(scores("nmae") < 1e-9, $"a perfect prediction has zero nmae, got {scores("nmae"):F9}")

        Dim total As Double = Metrics.AnchorScore(scores)

        Call DebugAudit.Mark("m6.perfect_total", total)
        DebugAudit.Test(std.Abs(total - 100.0) < 1e-9, $"a perfect prediction totals 100, got {total:F6}")
    End Sub

    ''' <summary>把预测换成另一组细胞后，剖面与表达误差必须明显变差。</summary>
    Private Sub CheckNoiseDegrades()
        Dim task As SyntheticTask = SyntheticFixture.Create()
        Dim model As CellularAiModel = CellularAiModel.Default()
        Dim engine As SynthesisEngine = SynthesisEngine.Create(task.Controls(0), model)
        Dim referenceBlocks As CsrMatrix() = MakeBlocks(task, engine, nCells:=30)
        Dim noiseBlocks As CsrMatrix() = Shuffle(referenceBlocks, task.GeneCount)

        Dim reference As SideStats = Metrics.BuildSide(referenceBlocks, task.Perturbations, task.Genes,
                                                       Function(g As Integer) engine.SortedCpm(g),
                                                       Function(g As Integer) engine.TieSum(g))
        Dim predicted As SideStats = Metrics.BuildSide(noiseBlocks, task.Perturbations, task.Genes,
                                                       Function(g As Integer) engine.SortedCpm(g),
                                                       Function(g As Integer) engine.TieSum(g))

        Dim scores As Dictionary(Of String, Double) = Metrics.Score(predicted, reference, task.Perturbations)

        Call DebugAudit.Mark("m6.noise_pds", scores("pds"))
        Call DebugAudit.Mark("m6.noise_expr", scores("expr_mse"))
        DebugAudit.Test(scores("pds") <= 1.0, "pds never exceeds 1")
        DebugAudit.Test(scores("expr_mse") < 1.0, $"a mismatched prediction loses expression accuracy, got {scores("expr_mse"):F6}")
        DebugAudit.Test(Metrics.AnchorScore(scores) < 100.0, "the total drops below the perfect score")
    End Sub

    ''' <summary>锚点缩放：nmae 方向相反、缺失指标被跳过、越界被钳制。</summary>
    Private Sub CheckAnchorScaling()
        Dim perfect As New Dictionary(Of String, Double)(StringComparer.Ordinal) From {
            {"pds", 1.0}, {"expr_mse", 1.0}, {"fid", 1.0}, {"reach", 1.0}, {"jac", 1.0}, {"nmae", 0.0}
        }
        Dim worst As New Dictionary(Of String, Double)(StringComparer.Ordinal) From {
            {"pds", 0.0}, {"expr_mse", 0.0}, {"fid", 0.0}, {"reach", 0.0}, {"jac", 0.0}, {"nmae", 1.0}
        }

        Call DebugAudit.AreEqual(100.0, Metrics.AnchorScore(perfect), "the perfect anchors give 100")
        Call DebugAudit.AreEqual(0.0, Metrics.AnchorScore(worst), "the worst anchors give 0")

        ' nmae 越大越差：只改 nmae 应当降低总分
        Dim withBadNmae As New Dictionary(Of String, Double)(perfect, StringComparer.Ordinal)

        withBadNmae("nmae") = 1.0
        DebugAudit.Test(Metrics.AnchorScore(withBadNmae) < 100.0, "a larger nmae lowers the total")

        ' 越界值被钳制在 [0,1]
        Dim clamped As New Dictionary(Of String, Double)(StringComparer.Ordinal) From {
            {"pds", 2.0}, {"expr_mse", 5.0}, {"fid", 1.0}, {"reach", 1.0}, {"jac", 1.0}, {"nmae", -3.0}
        }

        Call DebugAudit.AreEqual(100.0, Metrics.AnchorScore(clamped), "out-of-range metrics are clamped")

        ' 缺失指标被跳过而不是当成 0
        Dim subset As New Dictionary(Of String, Double)(StringComparer.Ordinal) From {
            {"pds", 1.0}, {"expr_mse", 1.0}, {"fid", 1.0}
        }

        Call DebugAudit.AreEqual(100.0, Metrics.AnchorScore(subset), "missing metrics are skipped")
    End Sub

#End Region

#Region "helpers"

    Private Function MakeBlocks(task As SyntheticTask, engine As SynthesisEngine, nCells As Integer) As CsrMatrix()
        Dim blocks(task.PerturbationCount - 1) As CsrMatrix

        For p As Integer = 0 To task.PerturbationCount - 1
            Dim dense As Double()() = RectangularArray.Matrix(Of Double)(nCells, task.GeneCount)
            Dim generator As New CellularAI.Randomizer.Pcg64(100 + p)

            For c As Integer = 0 To nCells - 1
                For g As Integer = 0 To task.GeneCount - 1
                    Dim rate As Double = std.Max(engine.MeanCpm(g), 0.0) * 10000.0 / 1000000.0
                    Dim signal As Double = If(g Mod 5 = p Mod 5, 3.0, 1.0)

                    dense(c)(g) = std.Floor(rate * signal * (0.5 + generator.NextDouble()))
                Next
            Next

            blocks(p) = CompressedBuilder.FromDense(dense)
        Next

        Return blocks
    End Function

    ''' <summary>把基因轴整体置换（保持总量、破坏剖面形状）。</summary>
    Private Function Shuffle(blocks As CsrMatrix(), nGenes As Integer) As CsrMatrix()
        Dim result(blocks.Length - 1) As CsrMatrix
        Dim generator As New CellularAI.Randomizer.Pcg64(7)

        For b As Integer = 0 To blocks.Length - 1
            Dim dense As Double()() = Metrics.ToDense(blocks(b), nGenes)
            Dim permutation As Integer() = generator.Permutation(nGenes)
            Dim shuffled As Double()() = RectangularArray.Matrix(Of Double)(dense.Length, nGenes)

            For c As Integer = 0 To dense.Length - 1
                For g As Integer = 0 To nGenes - 1
                    shuffled(c)(g) = dense(c)(permutation(g))
                Next
            Next

            result(b) = CompressedBuilder.FromDense(shuffled)
        Next

        Return result
    End Function

#End Region

End Module
