Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging

Module Program

    Sub Main(args As String())
        Call DebugAudit.Reset()
        Call Assert.Reset()
        Call DebugAudit.Mark("metaTraits-ensemble", "start")

        Try
            Call EnsembleTrainingTest.Run()
        Catch ex As Exception
            Call Assert.Test(False, $"synthetic suite threw: {ex}")
        End Try

        Try
            Call RealDataTrainingTest.Run()
        Catch ex As Exception
            Call Assert.Test(False, $"real-data suite threw: {ex}")
        End Try

        Console.WriteLine()
        Console.WriteLine(DebugAudit.Dump(includeRecords:=False))
        Console.WriteLine()

        Dim report As String = Path.Combine(AppContext.BaseDirectory, "metaTraits_ensemble_test_report.json")

        Call DebugAudit.SaveReport(report)

        Console.WriteLine($"report: {report}")
        Console.WriteLine($"assertion failures: {Assert.Failures}")

        For Each message As String In Assert.Messages
            Console.WriteLine($"  FAILED: {message}")
        Next

        Console.WriteLine(If(Assert.Failures = 0, "ALL TESTS PASSED", "TESTS FAILED"))

        Environment.ExitCode = If(Assert.Failures = 0, 0, 1)
    End Sub

End Module

''' <summary>
''' 测试断言计数器。DebugAudit 是全局模块，底层库对空集合的提示也会写入
''' FailureCount，因此在测试侧单独统计自身的断言结果，避免误判。
''' </summary>
Friend Module Assert

    Public Property Failures As Integer = 0
    Public ReadOnly Property Messages As New List(Of String)()

    Public Sub Reset()
        Failures = 0
        Call Messages.Clear()
    End Sub

    Public Sub Test(condition As Boolean, message As String)
        If Not condition Then
            Failures += 1
            Call Messages.Add(message)
        End If

        Call DebugAudit.Test(condition, message)
    End Sub

    Public Sub AreEqual(Of T)(expected As T, actual As T, message As String)
        If Not EqualityComparer(Of T).Default.Equals(expected, actual) Then
            Failures += 1
            Call Messages.Add($"{message}: expected '{expected}', actual '{actual}'")
        End If

        Call DebugAudit.AreEqual(expected, actual, message)
    End Sub

End Module
