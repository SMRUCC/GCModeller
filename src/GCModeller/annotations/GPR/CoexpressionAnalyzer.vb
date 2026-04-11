Imports SMRUCC.genomics.ComponentModel.Annotation

''' <summary>
''' 基于表达数据的共表达分析
''' 共表达的基因很可能参与同一通路
''' </summary>
Public Class CoexpressionAnalyzer
    ' 基因对 -> 相关系数
    Private coexpressionMatrix As Dictionary(Of String, Dictionary(Of String, Double))

    Public Sub New(expressionData As Dictionary(Of String, Double()))
        ' 从表达数据计算相关系数
        coexpressionMatrix = CalculateCorrelationMatrix(expressionData)
    End Sub

    Public Sub ApplyCoexpressionRules(gene As GeneTable,
                                      genome As GeneTable(),
                                      ByRef geneScores As Dictionary(Of String, Double),
                                      pathways As List(Of Pathway))

        ' 寻找与当前基因共表达的基因
        Dim coexpressedGenes = FindCoexpressedGenes(gene.locus_id, threshold:=0.7)

        For Each coGene In coexpressedGenes
            ' 获取共表达基因的关联反应
            Dim coGeneReactions = GetGeneReactions(coGene, genome)

            ' 对这些反应所在的通路进行增强
            For Each reaction In coGeneReactions
                Dim pathway = GetPathwayForReaction(reaction, pathways)
                If pathway Is Nothing Then Continue For

                Dim coexpressionScore = 0.4

                ' 增强该通路中所有反应的分数
                For Each pwReaction In pathway.Reactions
                    If Not geneScores.ContainsKey(pwReaction.Id) OrElse
                       geneScores(pwReaction.Id) < coexpressionScore Then
                        geneScores(pwReaction.Id) = coexpressionScore
                    End If
                Next
            Next
        Next
    End Sub

    Private Function FindCoexpressedGenes(geneId As String, threshold As Double) As List(Of String)
        Dim result = New List(Of String)()
        If Not coexpressionMatrix.ContainsKey(geneId) Then Return result

        For Each kvp In coexpressionMatrix(geneId)
            If kvp.Value >= threshold Then
                result.Add(kvp.Key)
            End If
        Next

        Return result
    End Function
End Class