Module Program

    Public Function Main() As Integer
        Return GetType(CLI).RunCLI(App.CommandLine)
    End Function

    Sub New()
        Call Settings.Session.Initialize()
    End Sub
End Module
