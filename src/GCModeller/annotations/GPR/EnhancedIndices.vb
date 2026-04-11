Public Class EnhancedIndices
    Public Property ECtoReactions As Dictionary(Of String, List(Of Reaction))
    Public Property ReactionToPathways As Dictionary(Of String, List(Of Pathway))
    Public Property PathwayReactions As Dictionary(Of String, List(Of Reaction))

    Public Sub New(pathways As Pathway())
        ECtoReactions = New Dictionary(Of String, List(Of Reaction))(
            StringComparer.OrdinalIgnoreCase)
        ReactionToPathways = New Dictionary(Of String, List(Of Pathway))(
            StringComparer.OrdinalIgnoreCase)
        PathwayReactions = New Dictionary(Of String, List(Of Reaction))(
            StringComparer.OrdinalIgnoreCase)

        BuildIndices(pathways)
    End Sub

    Private Sub BuildIndices(pathways As Pathway())
        For Each pathway In pathways
            PathwayReactions(pathway.Id) = pathway.Reactions.ToList()

            For Each rxn In pathway.Reactions
                ' 构建反应到通路的映射
                If Not ReactionToPathways.ContainsKey(rxn.Id) Then
                    ReactionToPathways(rxn.Id) = New List(Of Pathway)()
                End If
                ReactionToPathways(rxn.Id).Add(pathway)

                ' 构建EC到反应的映射（支持多EC号）
                For Each ec In rxn.EcNumbers
                    If Not ECtoReactions.ContainsKey(ec) Then
                        ECtoReactions(ec) = New List(Of Reaction)()
                    End If
                    ECtoReactions(ec).Add(rxn)
                Next
            Next
        Next
    End Sub
End Class