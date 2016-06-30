Imports System.Collections.ObjectModel
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular
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