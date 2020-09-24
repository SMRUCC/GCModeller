#Region "Microsoft.VisualBasic::75a77939eff5fdc097cf24d7ff8c1578, engine\GCModeller\EngineSystem\ObjectModels\ExperimentSystem\ShellScriptAPI.vb"

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

    '     Module ShellScriptAPI
    ' 
    '         Function: CurrentTimeIs, DrawMetabolism, ExportDynamicsCellSystem, Get_currentTime, SaveNetwork
    ' 
    '         Sub: set_EngineKernel
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.GCModeller.ModellingEngine.DataVisualization.DynamicMap
Imports SMRUCC.genomics.GCModeller.ModellingEngine.DataVisualization.DynamicMap.IMapBuilder

Namespace EngineSystem.ObjectModels.ExperimentSystem

    <Package("GCModeller.EngineKernel.ExperimentSystem")>
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
