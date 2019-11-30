
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("GCModeller")>
Public Module CLI

    <ExportAPI("profiler")>
    Public Function profiler() As GCModellerApps.Profiler
        Return GCModellerApps.Profiler.FromEnvironment(App.HOME)
    End Function

    <ExportAPI("eggHTS")>
    Public Function eggHTS() As GCModellerApps.eggHTS
        Return GCModellerApps.eggHTS.FromEnvironment(App.HOME)
    End Function
End Module
