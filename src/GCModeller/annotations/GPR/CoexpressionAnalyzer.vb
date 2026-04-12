Imports Microsoft.VisualBasic.Math.Matrix
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.MetabolicModel

''' <summary>
''' 基于表达数据的共表达分析
''' 共表达的基因很可能参与同一通路
''' </summary>
Public Class CoexpressionAnalyzer

    ' 基因对 -> 相关系数
    ReadOnly coexpressionMatrix As CorrelationMatrix
    ReadOnly genome As Genome

    Public Sub New(expressionData As Matrix, genome As Genome)
        ' 从表达数据计算相关系数
        Me.genome = genome
        Me.coexpressionMatrix = expressionData.Correlation(Function(gene) gene.experiments)
    End Sub

    Public Sub ApplyCoexpressionRules(gene As GeneTable,
                                      ByRef geneScores As Dictionary(Of String, Double),
                                      context As ContextIndices)

        ' 寻找与当前基因共表达的基因
        Dim coexpressedGenes = FindCoexpressedGenes(gene.locus_id, threshold:=0.7).ToArray

        For Each coGene As String In coexpressedGenes
            ' 获取共表达基因的关联反应
            Dim coGeneReactions As IEnumerable(Of MetabolicReaction) = genome.GetGeneReactions(coGene)

            ' 对这些反应所在的通路进行增强
            For Each reaction As MetabolicReaction In coGeneReactions
                Dim pathways As Pathway() = context.GetPathwayForReaction(reaction).ToArray

                If pathways.IsNullOrEmpty Then
                    Continue For
                End If

                Dim coexpressionScore = 0.4

                For Each pathway As Pathway In pathways
                    ' 增强该通路中所有反应的分数
                    For Each pwReaction In pathway.metabolicNetwork
                        If Not geneScores.ContainsKey(pwReaction.id) OrElse
                           geneScores(pwReaction.id) < coexpressionScore Then
                            geneScores(pwReaction.id) = coexpressionScore
                        End If
                    Next
                Next
            Next
        Next
    End Sub

    Private Iterator Function FindCoexpressedGenes(geneId As String, threshold As Double) As IEnumerable(Of String)
        If Not coexpressionMatrix.HasObject(geneId) Then
            Return
        End If

        Dim vec As Double() = coexpressionMatrix.GetVector(geneId)
        Dim geneIds As String() = coexpressionMatrix.GetLabels.ToArray

        For i As Integer = 0 To vec.Length - 1
            If vec(i) >= threshold Then
                Yield geneIds(i)
            End If
        Next
    End Function
End Class