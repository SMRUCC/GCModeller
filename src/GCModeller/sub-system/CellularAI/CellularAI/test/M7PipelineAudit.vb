Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Model
Imports std = System.Math

''' <summary>
''' M7 端到端审计：<see cref="ContextPipeline"/> 的形状、报告完整性、门控自洽与可复现性。
''' </summary>
Public Module M7PipelineAudit

    Private Const ReportName As String = "cellularai-m7-report.json"

    Public Function Run() As Integer
        Dim reportPath As String = Path.Combine(AppContext.BaseDirectory, ReportName)

        Call DebugAudit.Reset()
        DebugAudit.MaxEvents = 4096
        Call DebugAudit.Mark("cellularai-m7 audit started")

        Try
            Call CheckPipeline()
        Catch ex As Exception
            Call DebugAudit.Mark("m7.audit.crash", ex.ToString())
            Call DebugAudit.Test(False, $"the M7 audit should not throw unhandled exceptions: {ex.Message}")
        End Try

        Call DebugAudit.SaveReport(reportPath)
        Call Console.WriteLine($"   [audit] report -> {reportPath}")
        Call Console.WriteLine($"   [audit] {DebugAudit.FailureCount} failure(s)")

        Return DebugAudit.FailureCount
    End Function

#Region "cases"

    Private Sub CheckPipeline()
        Dim task As SyntheticTask = SyntheticFixture.Create()
        Dim model As CellularAiModel = CellularAiModel.Default()
        Dim nCells As Integer = 12

        Dim first As ContextPipeline.PredictionResult = ContextPipeline.Predict(
            task.Controls(0), task.Genes, task.Perturbations, task.Bundle, model,
            medianDegs:=4.0, callDepth:=20000.0, context:=task.Contexts(0),
            nCells:=nCells, seed:=20261126)

        Call DebugAudit.Test(first IsNot Nothing, "the pipeline returns a result")
        Call DebugAudit.Test(first.Report IsNot Nothing, "the pipeline returns a report")
        Call DebugAudit.Test(first.Blocks IsNot Nothing AndAlso first.Blocks.Length = task.PerturbationCount,
                             "one block per perturbation")
        Call DebugAudit.AreEqual(nCells * task.PerturbationCount, first.Matrix.RowDimension,
                                 "the stacked matrix holds n_cells per perturbation")
        Call DebugAudit.AreEqual(task.GeneCount, first.Matrix.ColumnDimension, "the stacked matrix keeps the gene axis")

        ' 报告字段完整
        Call DebugAudit.AreEqual(task.Contexts(0), first.Report.Context, "the report keeps the context label")
        Call DebugAudit.Test(first.Report.Gates IsNot Nothing AndAlso first.Report.Gates.Length >= 6,
                             "at least six gates are evaluated")

        For i As Integer = 0 To first.Report.Gates.Length - 1
            Call DebugAudit.Mark($"m7.gate_{first.Report.Gates(i).Name}", If(first.Report.Gates(i).Passed, 1.0, 0.0))
        Next

        ' Accepted 必须与门控自洽
        Dim allPassed As Boolean = True

        For i As Integer = 0 To first.Report.Gates.Length - 1
            If Not first.Report.Gates(i).Passed Then
                allPassed = False
            End If
        Next

        Call DebugAudit.AreEqual(allPassed, first.Report.Accepted, "the accepted flag matches the gates")

        ' 计数非负且为整数
        Dim integral As Boolean = True

        For k As Integer = 0 To first.Matrix.Values.Length - 1
            Dim value As Double = first.Matrix.Values(k)

            If value < 0.0 OrElse value <> std.Floor(value) Then
                integral = False
            End If
        Next

        DebugAudit.Test(integral, "every predicted count is a non-negative integer")

        ' 可复现：同 seed 两次运行逐元素相等
        Dim second As ContextPipeline.PredictionResult = ContextPipeline.Predict(
            task.Controls(0), task.Genes, task.Perturbations, task.Bundle, model,
            medianDegs:=4.0, callDepth:=20000.0, context:=task.Contexts(0),
            nCells:=nCells, seed:=20261126)

        Dim identical As Boolean = False

        If second.Matrix.Values.Length = first.Matrix.Values.Length Then
            identical = True

            For k As Integer = 0 To first.Matrix.Values.Length - 1
                If first.Matrix.Values(k) <> second.Matrix.Values(k) Then
                    identical = False
                    Exit For
                End If
            Next
        End If

        DebugAudit.Test(identical, "the same seed reproduces the prediction element by element")
        Call DebugAudit.AreEqual(first.Report.Margin, second.Report.Margin, "the margin is reproduced exactly")
        Call DebugAudit.AreEqual(first.Report.CreditShare, second.Report.CreditShare, "the credit share is reproduced exactly")

        Call DebugAudit.Mark("m7.margin", first.Report.Margin)
        Call DebugAudit.Mark("m7.credit_share", first.Report.CreditShare)
        Call DebugAudit.Mark("m7.amplitude", first.Report.Amplitude)
        Call DebugAudit.Mark("m7.unmeasured", first.Report.Unmeasured.Length)

        ' 未测靶与护栏快照都会被带出来
        DebugAudit.Test(first.Report.Unmeasured IsNot Nothing, "the unmeasured list is always present")
        DebugAudit.Test(first.Report.Guards IsNot Nothing, "the guards snapshot is always present")
    End Sub

#End Region

End Module
