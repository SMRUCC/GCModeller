#Region "Microsoft.VisualBasic::d63ab4450407930c16784942f1e84720, engine\vcellkit\Debugger\Debugger.vb"

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

' Module Debugger
' 
'     Function: createFluxDynamicsEngine, loadDataDriver, ModelPathwayMap
' 
'     Sub: createDynamicsSummary
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.GCModeller.ModellingEngine.BootstrapLoader.Definitions
Imports SMRUCC.genomics.GCModeller.ModellingEngine.BootstrapLoader.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Process
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports KeggReaction = SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Reaction

<Package("debugger", Category:=APICategories.ResearchTools, Publisher:="xie.guigang@gcmodeller.org")>
<RTypeExport("dataset.driver", GetType(DataSetDriver))>
Module Debugger

    <ExportAPI("vcell.summary")>
    Public Sub createDynamicsSummary(inits As Definition, model As CellularModule, dir As String)
        Call Summary.summary(inits, model, dir)
    End Sub

    <ExportAPI("map.flux")>
    Public Function ModelPathwayMap(map As Map, reactions As ReactionRepository, Optional init As Double = 1000) As Vessel
        Dim mass As New MassTable
        Dim list As String() = map.GetMembers _
            .Where(Function(id)
                       Return id.IsPattern("R\d+")
                   End Function) _
            .Distinct _
            .ToArray
        Dim fluxes As New List(Of Channel)
        Dim model As DefaultTypes.Equation
        Dim left As String()
        Dim right As List(Of String)

        For Each reaction As KeggReaction In list _
            .Select(AddressOf reactions.GetByKey) _
            .Where(Function(r)
                       Return Not r Is Nothing
                   End Function)

            model = reaction.ReactionModel
            left = model.Reactants.Select(Function(a) a.ID).ToArray
            right = model.Products.Select(Function(a) a.ID).AsList

            For Each id As String In left + right
                Call mass.AddNew(id, MassRoles.compound)
            Next

            fluxes += New Channel(mass(model.Reactants), mass(model.Products)) With {
                .ID = reaction.ID,
                .bounds = {10, 10},
                .forward = New AdditiveControls With {.baseline = 1, .activation = mass.variables(left, 1).ToArray, .inhibition = mass.variables(right, 0.5).ToArray},
                .reverse = New AdditiveControls With {.baseline = 1, .activation = mass.variables(right, 1).ToArray, .inhibition = mass.variables(left, 0.5).ToArray}
            }
        Next

        Dim reset_data As Dictionary(Of String, Double) = mass _
            .AsEnumerable _
            .ToDictionary(Function(a) a.ID,
                          Function()
                              Return init
                          End Function)

        Return New Vessel() _
            .load(mass.AsEnumerable) _
            .load(fluxes) _
            .Initialize _
            .Reset(reset_data)
    End Function

    <ExportAPI("flux.dynamics")>
    Public Function createFluxDynamicsEngine(core As Vessel,
                                             Optional time% = 50,
                                             Optional resolution% = 10000,
                                             Optional showProgress As Boolean = True) As FluxEmulator

        Return New FluxEmulator(core, time, resolution, showProgress)
    End Function

    <ExportAPI("flux.load_driver")>
    Public Function loadDataDriver(core As FluxEmulator, mass As DataSetDriver, flux As DataSetDriver) As FluxEmulator
        Return core _
            .AttatchMassDriver(AddressOf mass.SnapshotDriver) _
            .AttatchFluxDriver(AddressOf flux.SnapshotDriver)
    End Function

    <ExportAPI("test_network")>
    Public Function CreateNetwork(<RRawVectorArgument> network As Object, <RListObjectArgument> init0 As list) As Vessel

    End Function
End Module
