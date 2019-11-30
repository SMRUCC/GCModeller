
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("GCModeller")>
Public Module CLI

    Public Function profiler() As GCModellerApps.Profiler
        Return GCModellerApps.Profiler.FromEnvironment(App.HOME)
    End Function
End Module
