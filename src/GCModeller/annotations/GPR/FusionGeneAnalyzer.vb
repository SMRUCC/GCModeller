Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.MetabolicModel

''' <summary>
''' 检测多域融合酶
''' 如果一个蛋白对应多个EC号，检查这些EC是否在同一个通路中形成连续的酶促步骤
''' </summary>
Public Class FusionGeneAnalyzer

    Private Const MaxGapInPathway As Integer = 3 ' 通路中允许的最大反应间隔

    ReadOnly index As EnhancedIndices

    Sub New(index As EnhancedIndices)
        Me.index = index
    End Sub

    Public Sub AnalyzeFusionGenes(genes As GeneTable(),
                                 pathways As Pathway(),
                                 ByRef geneScores As Dictionary(Of String, Dictionary(Of String, Double)))

        For Each gene In genes
            If gene.EC_Number Is Nothing OrElse gene.EC_Number.Count < 2 Then Continue For

            ' 获取该基因所有EC号参与的反应
            Dim geneReactions = New List(Of MetabolicReaction)()
            For Each ec In gene.EC_Number
                If index.ECtoReactions.ContainsKey(ec) Then
                    geneReactions.AddRange(index.ECtoReactions(ec))
                End If
            Next

            ' 检查这些反应是否在同一个通路中形成连续或相邻步骤
            Dim pathwayContinuityScores = CheckPathwayContinuity(geneReactions, pathways)

            ' 增强这些通路中所有反应的分数
            For Each pathwayScore In pathwayContinuityScores
                Dim pathway = pathwayScore.Key
                Dim continuity = pathwayScore.Value

                For Each reaction In pathway.metabolicNetwork
                    Dim fusionScore = 0.3 + continuity * 0.5

                    If Not geneScores(gene.locus_id).ContainsKey(reaction.id) OrElse
                       geneScores(gene.locus_id)(reaction.id) < fusionScore Then
                        geneScores(gene.locus_id)(reaction.id) = fusionScore
                    End If
                Next
            Next
        Next
    End Sub

    Private Function CheckPathwayContinuity(reactions As List(Of MetabolicReaction),
                                           pathways As Pathway()) As Dictionary(Of Pathway, Double)
        ' 实现检测反应在通路中的连续程度
        ' 返回通路及其连续性分数
    End Function
End Class