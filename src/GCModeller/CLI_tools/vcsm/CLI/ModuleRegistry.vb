Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Terminal.STDIO

Partial Module CommandLines

    <ExportAPI("registry", Info:="",
    Usage:="registry <assembly_file>", Example:="resistry /home/xieguigang/gcmodeller/models/plas.dll")>
    Public Function RegistryModule(CommandLine As Microsoft.VisualBasic.CommandLine.CommandLine) As Integer
        Using Registry As LANS.SystemsBiology.GCModeller.ModellingEngine.PlugIns.ModuleRegistry =
            LANS.SystemsBiology.GCModeller.ModellingEngine.PlugIns.ModuleRegistry.Load(LANS.SystemsBiology.GCModeller.ModellingEngine.PlugIns.ModuleRegistry.XmlFile)
            Dim Entry = Registry.Registry(CommandLine.Parameters.First)
            Call Printf("Registry target external module successfully!\nAssembly: %s\n\n%s", CommandLine.Parameters.First, Entry.GetDescription)
        End Using
        Return 0
    End Function

    <ExportAPI("unregistry", Info:="", Usage:="unregistry <assembly_file>", Example:="unregistry ~/gcmodeller/models/plas.dll")>
    Public Function UnRegistry(CommandLine As Microsoft.VisualBasic.CommandLine.CommandLine) As Integer
        Using Registry As LANS.SystemsBiology.GCModeller.ModellingEngine.PlugIns.ModuleRegistry =
            LANS.SystemsBiology.GCModeller.ModellingEngine.PlugIns.ModuleRegistry.Load(LANS.SystemsBiology.GCModeller.ModellingEngine.PlugIns.ModuleRegistry.XmlFile)
            Call Registry.UnRegistry(CommandLine.Parameters.First)
        End Using
        Return 0
    End Function
End Module