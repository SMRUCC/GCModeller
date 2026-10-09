Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.RetroPath.Search
Imports SMRUCC.genomics.MetabolicModel

''' <summary>
''' A helper model for the pathway router
''' </summary>
Public Class MetabolicNetwork

    Public Property compounds As New Dictionary(Of String, MetabolicCompound)
    Public Property reactions As New Dictionary(Of String, MetabolicReaction)

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="rxn"></param>
    ''' <param name="source">
    ''' usually be the taxonomy organism source id
    ''' </param>
    Public Sub Add(rxn As MetabolicReaction, ParamArray source As String())
        If Not reactions.ContainsKey(rxn.id) Then
            rxn.sources = source
            reactions.Add(rxn.id, rxn)
        Else
            reactions(rxn.id).sources = reactions(rxn.id).sources _
                .JoinIterates(source) _
                .Distinct _
                .ToArray
        End If
    End Sub

    ''' <summary>
    ''' add unique metabolic compound model
    ''' </summary>
    ''' <param name="compound"></param>
    Public Sub Add(compound As MetabolicCompound)
        If Not compounds.ContainsKey(compound.id) Then
            Call compounds.Add(compound.id, compound)
        End If
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function LoadRouter(Optional opts As SearchOptions = Nothing, Optional w As ScoreWeights = Nothing) As MetabolicAdapter
        Return New MetabolicAdapter(compounds.Values, reactions.Values, opts, w)
    End Function

    ''' <summary>
    ''' Make sub-network via a given set of the taxonomy organism tags
    ''' </summary>
    ''' <param name="sources"></param>
    ''' <returns></returns>
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

    ''' <summary>
    ''' make union of a new global metabolic network
    ''' </summary>
    ''' <param name="a"></param>
    ''' <param name="b"></param>
    ''' <returns></returns>
    Public Shared Operator &(a As MetabolicNetwork, b As MetabolicNetwork) As MetabolicNetwork
        Dim globals As New MetabolicNetwork

        For Each compound As MetabolicCompound In c(a.compounds, b.compounds)
            Call globals.Add(compound)
        Next

        For Each reaction As MetabolicReaction In c(a.reactions, b.reactions)
            Call globals.Add(reaction, reaction.sources)
        Next

        Return globals
    End Operator

End Class
