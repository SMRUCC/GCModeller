Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports SMRUCC.genomics.MetabolicModel

''' <summary>
''' 检查反应之间的化学相容性
''' 如果一个反应的产物是下一个反应的底物，增强这些反应的分数
''' </summary>
Public Class ReactionContinuityChecker

    ' 反应ID -> 底物/产物映射
    Private reactionCompounds As Dictionary(Of String, MetabolicReaction)

    Public Sub New(reactionData As Dictionary(Of String, MetabolicReaction))
        reactionCompounds = reactionData
    End Sub

    ''' <summary>
    ''' 对通路中的每个反应对检查连续性
    ''' </summary>
    ''' <param name="pathway"></param>
    ''' <param name="geneScores"></param>
    ''' <param name="genome"></param>
    Public Sub CheckContinuity(pathway As Pathway, geneScores As Dictionary(Of String, Double), genome As Genome)
        For i As Integer = 0 To pathway.metabolicNetwork.Length - 2
            Dim currRxn = pathway.metabolicNetwork(i)
            Dim nextRxn = pathway.metabolicNetwork(i + 1)

            If Not reactionCompounds.ContainsKey(currRxn.id) Or
               Not reactionCompounds.ContainsKey(nextRxn.id) Then Continue For

            Dim currProducts As String() = reactionCompounds(currRxn.id).right.Keys
            Dim nextSubstrates As String() = reactionCompounds(nextRxn.id).left.Keys

            ' 检查化学相容性
            Dim overlap = currProducts.Intersect(nextSubstrates).Count()
            If overlap > 0 Then
                ' 增强这两个反应的关联分数
                Dim continuityScore = 0.3 + (overlap / Math.Max(currProducts.Count, nextSubstrates.Count)) * 0.3

                ' 如果基因已经被关联到这些反应，增强分数
                For Each geneId As String In genome.GetGenesForReaction(currRxn.id).Keys
                    If geneScores.ContainsKey(geneId) Then
                        geneScores(geneId) = Math.Max(geneScores(geneId), continuityScore)
                    End If
                Next
            End If
        Next
    End Sub

    Public Shared Function LoadFromContext(context As ContextIndices) As ReactionContinuityChecker
        Dim rxnIndex = context.ECtoReactions.Values _
            .SelectMany(Function(v) v) _
            .GroupBy(Function(r) r.id) _
            .ToDictionary(Function(r) r.Key,
                          Function(r)
                              Return r.First
                          End Function)

        Return New ReactionContinuityChecker(rxnIndex)
    End Function

End Class