#Region "Microsoft.VisualBasic::970b425dc9b20431bfbcf18481c1eb66, engine\vcellkit\Simulator.vb"

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

    ' Enum ModuleSystemLevels
    ' 
    '     Metabolome, Proteome, Transcriptome
    ' 
    '  
    ' 
    ' 
    ' 
    ' Module Simulator
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: ApplyModuleProfile, CreateObjectModel, CreateUnifyDefinition, CreateVCellEngine, FluxIndex
    '               GetDefaultDynamics, MassIndex
    ' 
    '     Sub: TakeStatusSnapshot
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Development
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.ModellingEngine.BootstrapLoader.Definitions
Imports SMRUCC.genomics.GCModeller.ModellingEngine.BootstrapLoader.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular
Imports SMRUCC.Rsharp.Runtime.Internal.ConsolePrinter

Public Enum ModuleSystemLevels
    Transcriptome
    Proteome
    Metabolome
End Enum

''' <summary>
''' 
''' </summary>
<Package("simulator", Category:=APICategories.ResearchTools)>
Public Module Simulator

    Sub New()
        Call VBDebugger.WaitOutput()
        Call GetType(Engine).Assembly _
            .FromAssembly _
            .DoCall(Sub(assm)
                        CLITools.AppSummary(assm, "Welcome to use SMRUCC/GCModeller virtual cell simulator!", Nothing, App.StdOut)
                    End Sub)
        Call Console.WriteLine()
        Call printer.AttachConsoleFormatter(Of VirtualCell)(AddressOf VirtualCell.Summary)
    End Sub

    ''' <summary>
    ''' Create a new status profile data object with unify mass contents.
    ''' </summary>
    ''' <param name="vcell"></param>
    ''' <param name="mass#"></param>
    ''' <returns></returns>
    <ExportAPI("vcell.mass.kegg")>
    <Extension>
    Public Function CreateUnifyDefinition(vcell As VirtualCell, Optional mass# = 5000) As Definition
        Return vcell.metabolismStructure.compounds _
            .Select(Function(c) c.ID) _
            .DoCall(Function(compounds)
                        Return Definition.KEGG(compounds, initMass:=mass)
                    End Function)
    End Function

    <ExportAPI("mass0")>
    Public Function mass0(vcell As VirtualCell) As Definition
        Return New Definition With {
            .status = vcell _
                .metabolismStructure _
                .compounds _
                .ToDictionary(Function(c) c.ID,
                              Function(c)
                                  Return c.mass0
                              End Function)
        }
    End Function

    ''' <summary>
    ''' create a generic vcell object model from a loaded vcell xml file model
    ''' </summary>
    ''' <param name="vcell">the file model data of the GCModeller vcell</param>
    ''' <returns></returns>
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

    ''' <summary>
    ''' create a new virtual cell engine
    ''' </summary>
    ''' <param name="inits"></param>
    ''' <param name="vcell"></param>
    ''' <param name="iterations%"></param>
    ''' <param name="time_resolutions%"></param>
    ''' <param name="deletions$"></param>
    ''' <param name="dynamics"></param>
    ''' <returns></returns>
    <ExportAPI("engine.load")>
    Public Function CreateVCellEngine(vcell As CellularModule,
                                      Optional inits As Definition = Nothing,
                                      Optional iterations% = 100,
                                      Optional time_resolutions% = 10000,
                                      Optional deletions$() = Nothing,
                                      Optional dynamics As FluxBaseline = Nothing,
                                      Optional showProgress As Boolean = True) As Engine

        Static defaultDynamics As [Default](Of FluxBaseline) = New FluxBaseline
        ' do initialize of the virtual cell engine
        ' and then load virtual cell model into 
        ' engine kernel
        Return New Engine(
            def:=inits,
            dynamics:=dynamics Or defaultDynamics,
            iterations:=iterations,
            showProgress:=showProgress,
            timeResolution:=time_resolutions
        ) _
        .LoadModel(vcell, deletions)
    End Function

    ''' <summary>
    ''' Create the default cell dynamics parameters
    ''' </summary>
    ''' <returns></returns>
    <ExportAPI("dynamics.default")>
    Public Function GetDefaultDynamics() As FluxBaseline
        Return New FluxBaseline
    End Function

    <ExportAPI("apply.module_profile")>
    Public Function ApplyModuleProfile(engine As Engine, profile As Dictionary(Of String, Double), Optional system As ModuleSystemLevels = ModuleSystemLevels.Transcriptome) As Engine
        If engine Is Nothing OrElse profile.IsNullOrEmpty Then
            Return engine
        End If

        Dim status As Definition = engine.initials

        Select Case system
            Case ModuleSystemLevels.Transcriptome

            Case ModuleSystemLevels.Proteome

            Case ModuleSystemLevels.Metabolome
                For Each compound In profile
                    status.status(compound.Key) = compound.Value
                Next

            Case Else
                Return engine
        End Select

        Return engine
    End Function

    <ExportAPI("vcell.snapshot")>
    <Extension>
    Public Sub TakeStatusSnapshot(engine As Engine, massIndex As OmicsTuple(Of String()), fluxIndex As OmicsTuple(Of String()), save$)
        Dim massSnapshot = DirectCast(engine.dataStorageDriver, FinalSnapshotDriver).mass
        Dim fluxSnapshot = DirectCast(engine.dataStorageDriver, FinalSnapshotDriver).flux

        ' rRNA, tRNA会在这产生重复
        ' 所以在这里会需要进行一次去重操作
        Call massSnapshot.Subset(massIndex.transcriptome.Distinct.ToArray, ignoreMissing:=True).GetJson.SaveTo($"{save}/mass/transcriptome.json")
        Call massSnapshot.Subset(massIndex.proteome, ignoreMissing:=True).GetJson.SaveTo($"{save}/mass/proteome.json")
        Call massSnapshot.Subset(massIndex.metabolome, ignoreMissing:=True).GetJson.SaveTo($"{save}/mass/metabolome.json")

        Call fluxSnapshot.Subset(fluxIndex.transcriptome, ignoreMissing:=True).GetJson.SaveTo($"{save}/flux/transcriptome.json")
        Call fluxSnapshot.Subset(fluxIndex.proteome, ignoreMissing:=True).GetJson.SaveTo($"{save}/flux/proteome.json")
        Call fluxSnapshot.Subset(fluxIndex.metabolome, ignoreMissing:=True).GetJson.SaveTo($"{save}/flux/metabolome.json")
    End Sub
End Module
