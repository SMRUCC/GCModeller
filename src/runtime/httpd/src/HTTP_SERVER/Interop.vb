Public Module Interop

    Public Function CreateServer() As CLI.Fluteway
        Return CLI.Fluteway.FromEnvironment(App.HOME)
    End Function
End Module
