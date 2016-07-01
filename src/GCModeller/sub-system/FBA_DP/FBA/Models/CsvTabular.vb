#Region "Microsoft.VisualBasic::d8c5076e07e3c8364018d8c22439f47c, ..\GCModeller\sub-system\FBA_DP\FBA\Models\CsvTabular.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Collections.ObjectModel
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular
Imports Microsoft.VisualBasic.Linq

Namespace Models

    Public Class CsvTabular : Inherits FBA.lpSolveRModel

        Dim _dataModels As Dictionary(Of DataModel.FluxObject)
        Protected Friend Overrides ReadOnly Property fluxColumns As ReadOnlyCollection(Of String)
        Protected Friend Overrides ReadOnly Property allCompounds As ReadOnlyCollection(Of String)

        Sub New(Model As IEnumerable(Of DataModel.FluxObject), objectiveFuncs As String())
            _dataModels = Model.ToDictionary
            _fluxColumns = New ReadOnlyCollection(Of String)((From flux As DataModel.FluxObject
                                                              In Model
                                                              Select flux.Identifier).ToArray)
            __fluxObjective = New ReadOnlyCollection(Of String)(objectiveFuncs)

            Dim lstMetabolite = (From Flux As DataModel.FluxObject
                                 In _dataModels.Values
                                 Select Flux.Substrates).MatrixAsIterator
            Me.allCompounds = New ReadOnlyCollection(Of String)((From sId As String In lstMetabolite Select sId Distinct Order By sId Ascending).ToArray)
        End Sub

        Protected Friend Overrides Function getConstraint(idx As String) As KeyValuePair(Of String, Double)
            If String.Equals(idx, "ATP") Then
                Return New KeyValuePair(Of String, Double)(">", 0)
            Else
                Return lpSolveRModel._constraint
            End If
        End Function

        Protected Friend Overrides Function __getLowerbound() As Double()
            Return (From Flux In _dataModels.Values Select Flux.Lower_Bound).ToArray
        End Function

        Protected Friend Overrides Function __getUpbound() As Double()
            Return (From Flux In _dataModels.Values Select Flux.Upper_Bound).ToArray
        End Function

        Public Overrides Sub SetObjectiveFunc(factors() As String)
            Me.__fluxObjective = New ReadOnlyCollection(Of String)(factors)
        End Sub

        Protected Friend Overrides Function __getStoichiometry(metabolite As String, rxn As String) As Double
            Dim flux As DataModel.FluxObject = Me._dataModels(rxn)
            Return flux.GetCoefficient(metabolite)
        End Function
    End Class
End Namespace
