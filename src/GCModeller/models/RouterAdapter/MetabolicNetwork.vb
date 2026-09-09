Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.RetroPath.Search
Imports SMRUCC.genomics.MetabolicModel

Public Class MetabolicNetwork

    Public Property compounds As New Dictionary(Of String, MetabolicCompound)
    Public Property reactions As New Dictionary(Of String, MetabolicReaction)

    Public Sub Add(rxn As MetabolicReaction, source As String)
        If Not reactions.ContainsKey(rxn.id) Then
            Call reactions.Add(rxn.id, rxn)
        End If

        reactions(rxn.id).sources = reactions(rxn.id).sources _
            .JoinIterates({source}) _
            .Distinct _
            .ToArray
    End Sub

    Public Sub Add(compound As MetabolicCompound)
        If Not compounds.ContainsKey(compound.id) Then
            Call compounds.Add(compound.id, compound)
        End If
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function LoadRouter(Optional opts As SearchOptions = Nothing, Optional w As ScoreWeights = Nothing) As MetabolicAdapter
        Return New MetabolicAdapter(compounds.Values, reactions.Values, opts, w)
    End Function

    Public Function SubNetwork(sources As IEnumerable(Of String)) As MetabolicNetwork
        Dim check As Index(Of String) = sources.Indexing
        Dim subs As MetabolicReaction() = reactions.Values _
            .Where(Function(r)
                       Return r.sources.Any(Function(tag) tag Like check)
                   End Function) _
            .ToArray
        Dim network As New MetabolicNetwork
        Dim metaboliteIds As String() = subs.Select(Function(r) r.left.JoinIterates(r.right)) _
            .IteratesALL _
            .Keys _
            .Distinct _
            .ToArray

        network.compounds = compounds.Subset(metaboliteIds)

        For Each rxn As MetabolicReaction In subs
            rxn = New MetabolicReaction(rxn)
            rxn.sources = check _
                .Intersect(collection:=rxn.sources) _
                .ToArray

            Call network.reactions.Add(rxn.id, rxn)
        Next

        Return network
    End Function

End Class
