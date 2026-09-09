Imports System.Runtime.CompilerServices
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

End Class
