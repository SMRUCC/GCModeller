#Region "Microsoft.VisualBasic::c79e6a54d3a2dcfcb4793c54e18ae810, engine\GCModeller\ShellScriptAPI.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' Module ShellScriptAPI
    ' 
    '     Function: ChangeConcentration, ChangeTemperature, DumpData, GetDefaultConfigurations, LoadModuleRegistry
    '               Run
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.CommandLine
Imports Oracle.Java.IO.Properties.Reflector
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("GCModeller.Engine_Kernel")>
Public Module ShellScriptAPI

    Dim ModelRegistry As SMRUCC.genomics.GCModeller.ModellingEngine.PlugIns.ModuleRegistry

    <ImportsConstant> Public Const NA As String = "N/A"

    <ExportAPI("Registry.Load_From_Xml")>
    Public Function LoadModuleRegistry(Path As String) As SMRUCC.genomics.GCModeller.ModellingEngine.PlugIns.ModuleRegistry
        ShellScriptAPI.ModelRegistry = SMRUCC.genomics.GCModeller.ModellingEngine.PlugIns.ModuleRegistry.Load(Path)
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

        Return ModellingEngine.EngineSystem.Engine.Run.Invoke(ShellScriptAPI.ModelRegistry, ModellingEngine.EngineSystem.Engine.Configuration.Configurations.Load(Configuration), argvs)
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
