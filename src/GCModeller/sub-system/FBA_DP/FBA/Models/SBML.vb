Imports System.Collections.ObjectModel
Imports SMRUCC.genomics.Assembly.SBML
Imports SMRUCC.genomics.Assembly.SBML.Level2.Elements
Imports SMRUCC.genomics.Assembly.SBML.Specifics.MetaCyc
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat
Imports Microsoft.VisualBasic.Linq

Namespace Models

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Class SBML : Inherits lpSolveRModel

        Protected Friend SBMLData As Level2.XmlFile
        Protected Friend Overrides ReadOnly Property fluxColumns As ReadOnlyCollection(Of String)
        Protected Friend Overrides ReadOnly Property allCompounds As ReadOnlyCollection(Of String)

        Protected Friend ReadOnly _fluxs As IReadOnlyDictionary(Of String, Reaction)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="SBMl"></param>
        ''' <param name="objectiveFunction">UniqueId list for the target metabolism reactions.(代谢反应对象的UniqueId列表)</param>
        ''' <param name="forceEnzymeRev">强制所有的酶促反应过程为可逆类型的</param>
        ''' <remarks></remarks>
        Sub New(SBMl As Level2.XmlFile, objectiveFunction As String(), forceEnzymeRev As Boolean)
            Me.SBMLData = SBMl
            Me._fluxs = SBMl.Model.listOfReactions.ToDictionary(Function(x) x.id)
            Me.__fluxObjective = New ReadOnlyCollection(Of String)(objectiveFunction)
            Me.fluxColumns = New ReadOnlyCollection(Of String)(_fluxs.Keys.ToArray)
            Me.allCompounds = New ReadOnlyCollection(Of String)((From x As Reaction In _fluxs.Values.AsParallel
                                                                 Select x.GetMetabolites.ToArray(
                                                                     Function(m) m.species)).MatrixAsIterator.Distinct.ToArray)
            If forceEnzymeRev Then
                For Each x In Me._fluxs.Values
                    Dim props As New FluxPropReader(x.Notes)
                    If Not StringHelpers.IsNullOrEmpty(props.GENE_ASSOCIATION) Then
                        x.reversible = True
                    End If
                Next
            End If
        End Sub

        Protected Friend Overrides Function getConstraint(idx As String) As KeyValuePair(Of String, Double)
            Return SBML._constraint
        End Function

        Protected Friend Overrides Function __getLowerbound() As Double()
            Return (From rxn In _fluxs Select rxn.Value.LowerBound).ToArray
        End Function

        Protected Friend Overrides Function __getUpbound() As Double()
            Return (From rxn In _fluxs Select rxn.Value.UpperBound).ToArray
        End Function

        Public Overrides Sub SetObjectiveFunc(factors() As String)
            __fluxObjective = New ReadOnlyCollection(Of String)(factors)
        End Sub

        Protected Friend Overrides Function __getStoichiometry(metabolite As String, rxn As String) As Double
            Dim flux As Reaction = _fluxs(rxn)
            Return flux.GetCoEfficient(metabolite)
        End Function
    End Class
End Namespace