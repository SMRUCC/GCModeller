#Region "Microsoft.VisualBasic::01f35da76433f7b0ae2f056223d8768b, sub-system\FBA\FBA_DP\FBA\Models\CsvTabular.vb"

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

    ' 
    ' /********************************************************************************/

#End Region

'#Region "Microsoft.VisualBasic::0a3a1fbf7d382a7ae45836ba50bd7863, sub-system\FBA\FBA_DP\FBA\Models\CsvTabular.vb"

'    ' Author:
'    ' 
'    '       asuka (amethyst.asuka@gcmodeller.org)
'    '       xie (genetics@smrucc.org)
'    '       xieguigang (xie.guigang@live.com)
'    ' 
'    ' Copyright (c) 2018 GPL3 Licensed
'    ' 
'    ' 
'    ' GNU GENERAL PUBLIC LICENSE (GPL3)
'    ' 
'    ' 
'    ' This program is free software: you can redistribute it and/or modify
'    ' it under the terms of the GNU General Public License as published by
'    ' the Free Software Foundation, either version 3 of the License, or
'    ' (at your option) any later version.
'    ' 
'    ' This program is distributed in the hope that it will be useful,
'    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
'    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'    ' GNU General Public License for more details.
'    ' 
'    ' You should have received a copy of the GNU General Public License
'    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



'    ' /********************************************************************************/

'    ' Summaries:

'    '     Class CsvTabular
'    ' 
'    '         Properties: allCompounds, fluxColumns
'    ' 
'    '         Constructor: (+1 Overloads) Sub New
'    ' 
'    '         Function: __getLowerbound, __getStoichiometry, __getUpbound, getConstraint
'    ' 
'    '         Sub: SetObjectiveFunc
'    ' 
'    ' 
'    ' /********************************************************************************/

'#End Region

'Imports System.Collections.ObjectModel
'Imports Microsoft.VisualBasic.ComponentModel.Collection
'Imports Microsoft.VisualBasic.Linq
'Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular

'Namespace Models

'    Public Class CsvTabular : Inherits lpSolveRModel

'        Dim _dataModels As Dictionary(Of DataModel.FluxObject)
'        Protected Friend Overrides ReadOnly Property fluxColumns As ReadOnlyCollection(Of String)
'        Protected Friend Overrides ReadOnly Property allCompounds As ReadOnlyCollection(Of String)

'        Sub New(Model As IEnumerable(Of DataModel.FluxObject), objectiveFuncs As String())
'            _dataModels = Model.ToDictionary
'            _fluxColumns = New ReadOnlyCollection(Of String)((From flux As DataModel.FluxObject
'                                                              In Model
'                                                              Select flux.Identifier).ToArray)
'            __fluxObjective = New ReadOnlyCollection(Of String)(objectiveFuncs)

'            Dim lstMetabolite = (From Flux As DataModel.FluxObject
'                                 In _dataModels.Values
'                                 Select Flux.Substrates).IteratesALL
'            Me.allCompounds = New ReadOnlyCollection(Of String)((From sId As String In lstMetabolite Select sId Distinct Order By sId Ascending).ToArray)
'        End Sub

'        Protected Friend Overrides Function getConstraint(idx As String) As KeyValuePair(Of String, Double)
'            If String.Equals(idx, "ATP") Then
'                Return New KeyValuePair(Of String, Double)(">", 0)
'            Else
'                Return lpSolveRModel._constraint
'            End If
'        End Function

'        Protected Friend Overrides Function __getLowerbound() As Double()
'            Return (From Flux In _dataModels.Values Select Flux.Lower_Bound).ToArray
'        End Function

'        Protected Friend Overrides Function __getUpbound() As Double()
'            Return (From Flux In _dataModels.Values Select Flux.Upper_Bound).ToArray
'        End Function

'        Public Overrides Sub SetObjectiveFunc(factors() As String)
'            Me.__fluxObjective = New ReadOnlyCollection(Of String)(factors)
'        End Sub

'        Protected Friend Overrides Function __getStoichiometry(metabolite As String, rxn As String) As Double
'            Dim flux As DataModel.FluxObject = Me._dataModels(rxn)
'            Return flux.GetCoefficient(metabolite)
'        End Function
'    End Class
'End Namespace
