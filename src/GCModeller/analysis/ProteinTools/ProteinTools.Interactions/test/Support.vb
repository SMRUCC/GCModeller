Imports Microsoft.VisualBasic.ApplicationServices.Debugging

Namespace test

    ''' <summary>
    ''' 测试工程之中的断言与统计工具。
    ''' </summary>
    ''' <remarks>
    ''' 全部断言都通过 <see cref="DebugAudit"/> 记录，因此除了在控制台上打印之外，
    ''' 还会被写入最终的 JSON 测试报告。
    ''' </remarks>
    Module Support

        ''' <summary>
        ''' 当前测试组已经执行的断言个数
        ''' </summary>
        Public Property Assertions As Integer = 0

        ''' <summary>
        ''' 打印一个分节标题
        ''' </summary>
        Public Sub Section(name As String)
            Call Console.WriteLine()
            Call Console.WriteLine($"==== {name} ====")
        End Sub

        ''' <summary>
        ''' 断言条件成立
        ''' </summary>
        Public Function [True](condition As Boolean, message As String) As Boolean
            Assertions += 1
            Return DebugAudit.Test(condition, message)
        End Function

        ''' <summary>
        ''' 断言两个浮点数在给定的容差之内相等
        ''' </summary>
        Public Function Near(expected As Double, actual As Double, Optional tolerance As Double = 1.0E-09, Optional message As String = Nothing) As Boolean
            Assertions += 1

            Dim text As String = If(message, "")
            Dim delta As Double = Math.Abs(expected - actual)
            Dim ok As Boolean

            If Double.IsPositiveInfinity(expected) Then
                ok = Double.IsPositiveInfinity(actual)
            ElseIf Double.IsNegativeInfinity(expected) Then
                ok = Double.IsNegativeInfinity(actual)
            ElseIf Double.IsNaN(expected) Then
                ok = Double.IsNaN(actual)
            Else
                ok = delta <= tolerance * Math.Max(1.0, Math.Abs(expected))
            End If

            Return DebugAudit.Test(ok, $"{text} expected {expected:R} but got {actual:R} (|Δ|={delta:G4}, tol={tolerance:G4})")
        End Function

        ''' <summary>
        ''' 断言两个整数相等
        ''' </summary>
        Public Function Same(expected As Integer, actual As Integer, Optional message As String = Nothing) As Boolean
            Assertions += 1
            Return DebugAudit.Test(expected = actual, $"{If(message, "")} expected {expected} but got {actual}")
        End Function

        ''' <summary>
        ''' 断言一段代码抛出了指定的异常类型
        ''' </summary>
        Public Function Throws(Of TException As Exception)(code As Action, Optional message As String = Nothing) As Boolean
            Assertions += 1
            Return DebugAudit.CheckThrows(Of TException)(code, message)
        End Function

        ''' <summary>
        ''' 运行一个测试组并记录耗时
        ''' </summary>
        Public Sub RunGroup(name As String, body As Action)
            Call Section(name)

            Using watch As DebugAuditScope = DebugAudit.Watch(name)
                Try
                    Call body()
                Catch ex As Exception
                    Assertions += 1
                    Call DebugAudit.Test(False, $"{name} threw {ex.GetType.Name}: {ex.Message}")
                    Call Console.WriteLine(ex.ToString())
                End Try
            End Using
        End Sub
    End Module
End Namespace
