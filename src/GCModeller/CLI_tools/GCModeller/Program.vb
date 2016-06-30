Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.Terminal.STDIO

Module Program

    Public Function Main() As Integer
        Return New Interpreter(GetType(CLI)).Execute(App.CommandLine)
    End Function
End Module
