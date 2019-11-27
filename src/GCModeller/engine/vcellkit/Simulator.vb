#Region "Microsoft.VisualBasic::6d73aeae106d7bb033d98b4633436add, vcellkit\Simulator.vb"

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

' Module Simulator
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine.Definitions
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

<Package("vcellkit.simulator", Category:=APICategories.ResearchTools)>
Public Module Simulator

    <ExportAPI("read.vcell")>
    Public Function LoadVirtualCell(path As String) As VirtualCell
        Return path.LoadXml(Of VirtualCell)
    End Function

    <ExportAPI("vcell.mass.kegg")>
    <Extension>
    Public Function CreateUnifyDefinition(vcell As VirtualCell, Optional mass# = 5000) As Definition
        Return vcell.metabolismStructure _
            .compounds _
            .Select(Function(c) c.ID) _
            .DoCall(Function(compounds)
                        Return Definition.KEGG(compounds, initMass:=mass)
                    End Function)
    End Function

    <ExportAPI("vcell.model")>
    Public Function CreateObjectModel(vcell As VirtualCell) As CellularModule
        Return vcell.CreateModel
    End Function

    <ExportAPI("vcell.mass.index")>
    Public Function MassIndex(vcell As CellularModule) As OmicsTuple(Of String())
        Return vcell.DoCall(AddressOf OmicsDataAdapter.GetMassTuples)
    End Function

    <ExportAPI("vcell.flux.index")>
    Public Function FluxIndex(vcell As CellularModule) As OmicsTuple(Of String())
        Return vcell.DoCall(AddressOf OmicsDataAdapter.GetFluxTuples)
    End Function

    <ExportAPI("engine.load")>
    Public Function CreateVCellEngine(def As Definition, vcell As CellularModule,
                                      Optional iterations% = 5000,
                                      Optional time_resolutions% = 1000,
                                      Optional deletions$() = Nothing) As Engine

        Return New Engine(def, iterations).LoadModel(vcell, deletions, time_resolutions)
    End Function

    <ExportAPI("vcell.snapshot")>
    <Extension>
    Public Sub TakeStatusSnapshot(engine As Engine, massIndex As OmicsTuple(Of String()), fluxIndex As OmicsTuple(Of String()), save$)
        Dim massSnapshot = engine.snapshot.mass
        Dim fluxSnapshot = engine.snapshot.flux

        Call massSnapshot.Subset(massIndex.transcriptome).GetJson.SaveTo($"{save}/mass/transcriptome.json")
        Call massSnapshot.Subset(massIndex.proteome).GetJson.SaveTo($"{save}/mass/proteome.json")
        Call massSnapshot.Subset(massIndex.metabolome).GetJson.SaveTo($"{save}/mass/metabolome.json")

        Call fluxSnapshot.Subset(fluxIndex.transcriptome).GetJson.SaveTo($"{save}/flux/transcriptome.json")
        Call fluxSnapshot.Subset(fluxIndex.proteome).GetJson.SaveTo($"{save}/flux/proteome.json")
        Call fluxSnapshot.Subset(fluxIndex.metabolome).GetJson.SaveTo($"{save}/flux/metabolome.json")
    End Sub
End Module

