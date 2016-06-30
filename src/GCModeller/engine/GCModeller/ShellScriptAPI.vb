Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.CommandLine
Imports Oracle.Java.IO.Properties.Reflector
Imports Microsoft.VisualBasic.Scripting.MetaData

<[PackageNamespace]("GCModeller.Engine_Kernel")>
Public Module ShellScriptAPI

    Dim ModelRegistry As LANS.SystemsBiology.GCModeller.ModellingEngine.PlugIns.ModuleRegistry

    <ImportsConstant> Public Const NA As String = "N/A"

    <ExportAPI("Registry.Load_From_Xml")>
    Public Function LoadModuleRegistry(Path As String) As LANS.SystemsBiology.GCModeller.ModellingEngine.PlugIns.ModuleRegistry
        ShellScriptAPI.ModelRegistry = LANS.SystemsBiology.GCModeller.ModellingEngine.PlugIns.ModuleRegistry.Load(Path)
        Return ModelRegistry
    End Function

    <ExportAPI("Run", info:="""-i"" parameter for input model file." & vbCrLf &
                          """-with_configurations"" parameter for specific the gcmodeller engine kernel settings profile.")>
    Public Function Run(argvs As CommandLine) As Integer
        Dim modelXml As String = argvs("-i")
        If String.IsNullOrEmpty(modelXml) Then '只包含有模型文件名
            modelXml = argvs.Parameters.First
            argvs = $"run -i ""{modelXml}"" -f csv_tabular"
        End If

        Dim Configuration As String = argvs("-with_configurations")
        If String.IsNullOrEmpty(Configuration) Then
            Configuration = FileIO.FileSystem.GetParentPath(modelXml)
            Configuration = FileIO.FileSystem.GetFiles(Configuration, FileIO.SearchOption.SearchTopLevelOnly, "*.inf").First
        End If

        Return Global.LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Engine.Run.Invoke(
            ShellScriptAPI.ModelRegistry, GCModeller.ModellingEngine.EngineSystem.Engine.Configuration.Configurations.Load(Configuration), argvs)
    End Function

    <ExportAPI("Default_Configuration.Create")>
    Public Function GetDefaultConfigurations(savedFile As String) As EngineSystem.Engine.Configuration.Configurations
        Dim df = EngineSystem.Engine.Configuration.Configurations.DefaultValue
        Call df.ToConfigDoc.SaveTo(savedFile)
        Return df
    End Function

    <ExportAPI("Experiment.Modify.Temperature")>
    Public Function ChangeTemperature(GCModeller As EngineSystem.Engine.GCModeller, NewTemperature As Double) As Double
        Dim oldValue = GCModeller.CultivatingMediums.Temperature
        GCModeller.CultivatingMediums.Temperature = NewTemperature
        Return oldValue
    End Function

    <ExportAPI("Experiment.Modify.Metabolite_Concentration")>
    Public Function ChangeConcentration(Cell As EngineSystem.ObjectModels.SubSystem.CellSystem, Id As String, NewValue As Double, modifyType As String) As Double
        Dim Target = Cell.Metabolism.Metabolites.GetItem(Id)
        If Target Is Nothing Then
            Call Console.WriteLine("Target metabolite {0} is not exists!", Id)
            Return -1
        Else
            Dim OldValue = Target.DataSource.Value

            If String.Equals(modifyType, "changeto") Then
                Target.Quantity = NewValue
            ElseIf String.Equals(modifyType, "plus") Then
                Target.Quantity += NewValue
            ElseIf String.Equals(modifyType, "multiply") Then
                Target.Quantity *= NewValue
            End If

            Return OldValue
        End If
    End Function

    <ExportAPI("Memory_Dump")>
    Public Function DumpData(GCModeller As EngineSystem.Engine.GCModeller, DumpDir As String) As Boolean
        Call GCModeller.MemoryDump(DumpDir)
        Return True
    End Function
End Module
