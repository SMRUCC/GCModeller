#Region "Microsoft.VisualBasic::89ce37a267caf278ed1d42e21fa76db2, engine\vcellkit\Debugger\Debugger.vb"

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
    '     Function: createFluxDynamicsEngine, CreateNetwork, GetFactor, GetFactors, loadDataDriver
    '               ModelPathwayMap
    ' 
    '     Sub: createDynamicsSummary
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
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
Imports SMRUCC.Rsharp.Interpreter.ExecuteEngine
Imports SMRUCC.Rsharp.Interpreter.ExecuteEngine.ExpressionSymbols.Closure
Imports SMRUCC.Rsharp.Interpreter.ExecuteEngine.ExpressionSymbols.DataSets
Imports SMRUCC.Rsharp.Interpreter.ExecuteEngine.ExpressionSymbols.Operators
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports KeggReaction = SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Reaction
Imports MassFactor = SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core.Factor

<Package("debugger", Category:=APICategories.ResearchTools, Publisher:="xie.guigang@gcmodeller.org")>
<RTypeExport("dataset.driver", GetType(DataSetDriver))>
Module Debugger

    <ExportAPI("vcell.summary")>
    Public Sub createDynamicsSummary(inits As Definition, model As CellularModule, dir As String)
        Call Summary.summary(inits, model, dir)
    End Sub

    ''' <summary>
    ''' create dynamics model from a kegg pathway map
    ''' </summary>
    ''' <param name="map"></param>
    ''' <param name="reactions"></param>
    ''' <param name="init"></param>
    ''' <returns></returns>
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

    ''' <summary>
    ''' run network dynamics
    ''' </summary>
    ''' <param name="network">
    ''' the target network graph model
    ''' </param>
    ''' <param name="init0">
    ''' the system initial conditions
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("test_network")>
    <RApiReturn(GetType(Vessel))>
    Public Function CreateNetwork(<RRawVectorArgument> network As Object,
                                  <RListObjectArgument> init0 As list,
                                  Optional env As Environment = Nothing) As Object

        Dim flux As New List(Of Channel)
        Dim dynamicsSystem As pipeline = pipeline.TryCreatePipeline(Of Expression)(network, env)
        Dim vars As New Dictionary(Of String, MassFactor)
        Dim massInit As Dictionary(Of String, Double) = init0.AsGeneric(Of Double)(env, 100)

        If dynamicsSystem.isError Then
            Return dynamicsSystem.getError
        End If

        For Each expr As Expression In dynamicsSystem.populates(Of Expression)(env)
            If TypeOf expr Is DeclareLambdaFunction Then
                Dim id As String = DirectCast(expr, DeclareLambdaFunction).parameterNames(Scan0)
                Dim formula As ValueAssignExpression = DirectCast(DirectCast(expr, DeclareLambdaFunction).closure, ValueAssignExpression)
                Dim left As Variable() = formula.targetSymbols.Select(Function(a) a.GetFactors(vars)).IteratesALL.ToArray
                Dim right As Variable() = formula.value.GetFactors(vars).ToArray
                Dim channel As New Channel(left, right) With {
                    .bounds = New Boundary(5, 5),
                    .ID = id,
                    .forward = New BaselineControls(2),
                    .reverse = New BaselineControls(1)
                }

                flux += channel

            ElseIf TypeOf expr Is FormulaExpression Then
                Throw New NotImplementedException
            Else
                Return Message.InCompatibleType(GetType(DeclareLambdaFunction), expr.GetType, env)
            End If
        Next

        For Each id As String In vars.Keys
            If Not massInit.ContainsKey(id) Then
                massInit.Add(id, 0)
            End If
        Next

        Return New Vessel() _
            .load(flux) _
            .load(vars.Values) _
            .Initialize _
            .Reset(massInit)
    End Function

    <Extension>
    Private Iterator Function GetFactors(dataExpr As Expression, vars As Dictionary(Of String, MassFactor)) As IEnumerable(Of Variable)
        If TypeOf dataExpr Is BinaryExpression Then
            Dim bin As BinaryExpression = DirectCast(dataExpr, BinaryExpression)

            If bin.operator = "*" Then
                Dim factor As Double = DirectCast(bin.left, Literal).Evaluate(Nothing)
                Dim var As New Variable(vars.GetFactor(bin.right), factor)

                Yield var
            ElseIf bin.operator = "+" Then
                For Each v As Variable In bin.left.GetFactors(vars)
                    Yield v
                Next
                For Each v As Variable In bin.right.GetFactors(vars)
                    Yield v
                Next
            Else
                Throw New InvalidExpressionException()
            End If
        ElseIf TypeOf dataExpr Is SymbolReference Then
            Yield New Variable(vars.GetFactor(dataExpr), 1)
        ElseIf TypeOf dataExpr Is Literal Then
            Yield New Variable(vars.GetFactor(New SymbolReference(DirectCast(dataExpr, Literal).ValueStr)), 1)
        End If
    End Function

    <Extension>
    Private Function GetFactor(vars As Dictionary(Of String, MassFactor), symbol As SymbolReference) As MassFactor
        Return vars.ComputeIfAbsent(symbol.symbol, Function() New MassFactor With {.ID = symbol.symbol, .Value = 100})
    End Function
End Module
