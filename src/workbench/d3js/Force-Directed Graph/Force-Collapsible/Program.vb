Imports Microsoft.VisualBasic.CommandLine

Module Program

    Public Function Main() As Integer
        Return GetType(Program).RunCLI(App.CommandLine, executeFile:=AddressOf Program.ExecuteFile)
    End Function

    Private Function ExecuteFile(file As String, args As CommandLine) As Integer
        Dim out As String = file.TrimFileExt & ".json"
        Dim json As String = NetworkGenerator.FromRegulations(file)
        Return json.SaveTo(out)
    End Function
End Module
