Imports SMRUCC.genomics.ComponentModel.Annotation

''' <summary>
''' 检查反应之间的化学相容性
''' 如果一个反应的产物是下一个反应的底物，增强这些反应的分数
''' </summary>
Public Class ReactionContinuityChecker
    ' 反应ID -> 底物/产物映射
    Private reactionCompounds As Dictionary(Of String, (Substrates As List(Of String),
                                                       Products As List(Of String)))

    Public Sub New(reactionData As Dictionary(Of String, (List(Of String), List(Of String))))
        reactionCompounds = reactionData
    End Sub

    Public Sub CheckContinuity(pathway As Pathway,
                               geneScores As Dictionary(Of String, Double),
                               genome As GeneTable())

        ' 对通路中的每个反应对检查连续性
        For i = 0 To pathway.metabolicNetwork.Length - 2
            Dim currRxn = pathway.metabolicNetwork(i)
            Dim nextRxn = pathway.metabolicNetwork(i + 1)

            If Not reactionCompounds.ContainsKey(currRxn.Id) Or
               Not reactionCompounds.ContainsKey(nextRxn.Id) Then Continue For

            Dim currProducts = reactionCompounds(currRxn.Id).Products
            Dim nextSubstrates = reactionCompounds(nextRxn.Id).Substrates

            ' 检查化学相容性
            Dim overlap = currProducts.Intersect(nextSubstrates).Count()
            If overlap > 0 Then
                ' 增强这两个反应的关联分数
                Dim continuityScore = 0.3 + (overlap / Math.Max(currProducts.Count, nextSubstrates.Count)) * 0.3

                ' 如果基因已经被关联到这些反应，增强分数
                For Each geneId In GetGenesForReaction(currRxn.Id, genome)
                    If geneScores.ContainsKey(geneId) Then
                        geneScores(geneId) = Math.Max(geneScores(geneId), continuityScore)
                    End If
                Next
            End If
        Next
    End Sub
End Class