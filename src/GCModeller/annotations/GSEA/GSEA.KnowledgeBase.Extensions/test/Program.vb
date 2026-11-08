Imports System
Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging

Module Program

    Sub Main(args As String())
        Call DebugAudit.Reset()
        Call DebugAudit.Mark("phenotype-background-test", "start")

        Try
            Call PhenotypeBackgroundTest.RunAll()
        Catch ex As Exception
            Call DebugAudit.Test(False, $"unexpected exception: {ex}")
        End Try

        Console.WriteLine()
        Console.WriteLine(DebugAudit.Dump(includeRecords:=True))
        Console.WriteLine()

        Dim reportPath As String = Path.Combine(AppContext.BaseDirectory,
                                               "phenotype_background_test_report.json")

        Call DebugAudit.SaveReport(reportPath)

        Console.WriteLine($"test report: {reportPath}")
        Console.WriteLine($"failureCount: {DebugAudit.FailureCount}")
        Console.WriteLine(If(DebugAudit.FailureCount = 0, "ALL TESTS PASSED", "TESTS FAILED"))

        Environment.ExitCode = If(DebugAudit.FailureCount = 0, 0, 1)
    End Sub

End Module
