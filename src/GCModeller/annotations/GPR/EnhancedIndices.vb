Imports SMRUCC.genomics.MetabolicModel

Public Class EnhancedIndices
    Public Property ECtoReactions As Dictionary(Of String, List(Of MetabolicReaction))
    Public Property ReactionToPathways As Dictionary(Of String, List(Of Pathway))
    Public Property PathwayReactions As Dictionary(Of String, List(Of MetabolicReaction))

    Public Sub New(pathways As Pathway())
        ECtoReactions = New Dictionary(Of String, List(Of MetabolicReaction))(StringComparer.OrdinalIgnoreCase)
        ReactionToPathways = New Dictionary(Of String, List(Of Pathway))(StringComparer.OrdinalIgnoreCase)
        PathwayReactions = New Dictionary(Of String, List(Of MetabolicReaction))(StringComparer.OrdinalIgnoreCase)

        BuildIndices(pathways)
    End Sub

    Private Sub BuildIndices(pathways As Pathway())
        For Each pathway In pathways
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
        Next
    End Sub
End Class