Imports SMRUCC.genomics.MetabolicModel

Public Class ContextIndices

    Public Property ECtoReactions As Dictionary(Of String, List(Of MetabolicReaction))
    Public Property ReactionToPathways As Dictionary(Of String, List(Of Pathway))
    Public Property PathwayReactions As Dictionary(Of String, List(Of MetabolicReaction))

    Public Property Pathways As Pathway()

    Public Sub New(pathways As IEnumerable(Of Pathway))
        Me.ECtoReactions = New Dictionary(Of String, List(Of MetabolicReaction))(StringComparer.OrdinalIgnoreCase)
        Me.ReactionToPathways = New Dictionary(Of String, List(Of Pathway))(StringComparer.OrdinalIgnoreCase)
        Me.PathwayReactions = New Dictionary(Of String, List(Of MetabolicReaction))(StringComparer.OrdinalIgnoreCase)
        Me.Pathways = BuildIndices(pathways).ToArray
    End Sub

    Private Iterator Function BuildIndices(pathways As IEnumerable(Of Pathway)) As IEnumerable(Of Pathway)
        For Each pathway As Pathway In pathways
            PathwayReactions(pathway.ID) = pathway.metabolicNetwork.ToList()

            For Each rxn In pathway.metabolicNetwork
                ' 构建反应到通路的映射
                If Not ReactionToPathways.ContainsKey(rxn.id) Then
                    ReactionToPathways(rxn.id) = New List(Of Pathway)()
                End If
                ReactionToPathways(rxn.id).Add(pathway)

                ' 构建EC到反应的映射（支持多EC号）
                For Each ec In rxn.ECNumbers
                    If Not ECtoReactions.ContainsKey(ec) Then
                        ECtoReactions(ec) = New List(Of MetabolicReaction)()
                    End If
                    ECtoReactions(ec).Add(rxn)
                Next
            Next

            Yield pathway
        Next
    End Function

    Public Function GetPathwayForReaction(reaction As MetabolicReaction) As IEnumerable(Of Pathway)
        If ReactionToPathways.ContainsKey(reaction.id) Then
            Return ReactionToPathways(reaction.id)
        Else
            Return {}
        End If
    End Function

    Public Iterator Function FindCommonPathways(allECs As IReadOnlyCollection(Of String)) As IEnumerable(Of Pathway)
        If allECs Is Nothing OrElse allECs.Count = 0 Then
            Return
        End If

        For Each pathway As Pathway In Pathways
            If pathway.CheckAllECNumberExists(allECs) Then
                Yield pathway
            End If
        Next
    End Function
End Class