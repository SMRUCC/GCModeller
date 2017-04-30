Module Program

    Public Function Main() As Integer
        Return GetType(CLIProgram).RunCLI(args:=App.CommandLine)
    End Function
End Module
