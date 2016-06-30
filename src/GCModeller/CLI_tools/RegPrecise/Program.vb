Module Program

    Sub New()
        Call Settings.Session.Initialize()
    End Sub

    Public Function Main() As Integer
        Return GetType(CLI).RunCLI(App.CommandLine)
    End Function
End Module
