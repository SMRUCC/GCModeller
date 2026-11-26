Imports System

''' <summary>
''' CellularAI 复现工程的自检入口：按里程碑分派到对应的审计模块。
''' </summary>
''' <remarks>
''' 用法：<c>dotnet run -- m0</c>；不传参数时默认跑 <c>m0</c>。
''' 每个审计都会用 <c>DebugAudit.SaveReport</c> 生成 JSON 报告，返回失败断言数。
''' </remarks>
Module Program

    Function Main(args As String()) As Integer
        Dim selector As String = If(args.Length > 0, args(0).Trim().ToLowerInvariant(), "m0")

        Select Case selector
            Case "m0"
                Return M0ModelAudit.Run()
            Case "m1"
                Return M1PoolingAudit.Run()
            Case Else
                Call Console.WriteLine($"unknown audit '{selector}'; known: m0")
                Return 2
        End Select
    End Function

End Module
