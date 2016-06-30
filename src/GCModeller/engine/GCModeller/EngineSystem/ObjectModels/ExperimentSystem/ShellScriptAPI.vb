Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.DataVisualization.DynamicMap.IMapBuilder
Imports Microsoft.VisualBasic.Scripting.ShoalShell
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.DataVisualization.DynamicMap

Namespace EngineSystem.ObjectModels.ExperimentSystem

    <[PackageNamespace]("GCModeller.EngineKernel.ExperimentSystem")>
    Module ShellScriptAPI

        Dim EngineKernel As Engine.GCModeller

        Public Sub set_EngineKernel(obj As Engine.GCModeller)
            ShellScriptAPI.EngineKernel = obj
        End Sub

        <ExportAPI("get.current_time")>
        Public Function Get_currentTime() As Integer
            Return EngineKernel.RuntimeTicks
        End Function

        <ExportAPI("build.dynamic.metabolism")>
        Public Function DrawMetabolism() As KeyValuePairObject(Of Component(), ComponentInteraction())
            Return New DataVisualization.DynamicMap.DynamicMapBuilder(EngineKernel.KernelModule).ExportMetabolismNetwork
        End Function

        <ExportAPI("build.dynamics.cell_network")>
        Public Function ExportDynamicsCellSystem() As KeyValuePairObject(Of Component(), ComponentInteraction())
            Return New DataVisualization.DynamicMap.DynamicMapBuilder(EngineKernel.KernelModule).ExportDynamicsCellNetwork
        End Function

        <Runtime.DeviceDriver.DriverHandles.IO_DeviceHandle(GetType(KeyValuePairObject(Of Component(), ComponentInteraction())))>
        <ExportAPI("write.network")>
        Public Function SaveNetwork(network As KeyValuePairObject(Of Component(), ComponentInteraction()), saveto As String) As Boolean
            Dim NetworkCsv As String = String.Format("{0}/Network.csv", saveto)
            Dim NodesCsv As String = String.Format("{0}/ComponentNodes.csv", saveto)

            Call network.Value.SaveTo(NetworkCsv, True)
            Call network.Key.SaveTo(NodesCsv, False)

            Dim TempChunk As String() = IO.File.ReadAllLines(NetworkCsv)
            Call IO.File.WriteAllLines(NetworkCsv, (From strValue As String In TempChunk Select strValue Distinct).ToArray)
            TempChunk = IO.File.ReadAllLines(NodesCsv)
            Call IO.File.WriteAllLines(NodesCsv, (From strValue As String In TempChunk Select strValue Distinct).ToArray)

            Return True
        End Function

        <ExportAPI("get.current_time.equals")>
        Public Function CurrentTimeIs(value As Integer) As Boolean
            Return value = EngineKernel.RuntimeTicks
        End Function
    End Module
End Namespace