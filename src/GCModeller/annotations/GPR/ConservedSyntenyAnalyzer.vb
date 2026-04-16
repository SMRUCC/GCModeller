Imports SMRUCC.genomics.ComponentModel.Annotation

''' <summary>
''' 基于多个基因组的保守基因邻接进行推断
''' 如果多个物种中某些基因总是相邻，则它们的功能很可能相关
''' </summary>
Public Class ConservedSyntenyAnalyzer

    ''' <summary>
    ''' 存储保守基因簇的模式
    ''' 
    ''' [cluster_id, gene_id_set[]]
    ''' </summary>
    ReadOnly conservedClusters As Dictionary(Of String, ConservedCluster)

    Public Sub New(conservationData As Dictionary(Of String, ConservedCluster))
        conservedClusters = conservationData
    End Sub

    Public Sub ApplyConservationRules(gene As GeneTable,
                                      genome As GeneTable(),
                                      geneIndex As Integer,
                                      ByRef geneScores As Dictionary(Of String, Double))

        ' 检查该基因是否在已知的保守簇中
        Dim clusterKey = FindClusterKey(gene, genome, geneIndex)
        If String.IsNullOrEmpty(clusterKey) Then Return

        ' 获取该保守簇在其他物种中的功能
        Dim conservedFunctions As IEnumerable(Of String) = conservedClusters(clusterKey).functions

        ' 根据保守性增强相关反应的分数
        For Each func In conservedFunctions
            ' func可以是EC号、反应ID或通路ID
            If func.StartsWith("R") Then ' 假设反应ID以R开头
                Dim conservationScore = 0.6
                If Not geneScores.ContainsKey(func) OrElse geneScores(func) < conservationScore Then
                    geneScores(func) = conservationScore
                End If
            End If
        Next
    End Sub

    Private Function FindClusterKey(gene As GeneTable, genome As GeneTable(), geneIndex As Integer) As String
        ' 检测基因周围的基因簇模式
        Dim clusterSize = 5
        Dim startIdx = Math.Max(0, geneIndex - clusterSize)
        Dim endIdx = Math.Min(genome.Length - 1, geneIndex + clusterSize)

        Dim clusterGenes = New List(Of String)()
        For i = startIdx To endIdx
            clusterGenes.Add(genome(i).locus_id)
        Next

        ' 在已知保守簇中寻找匹配
        For Each kvp In conservedClusters
            Dim overlap = clusterGenes.Intersect(kvp.Value).Count()
            Dim similarity = overlap / Math.Min(clusterGenes.Count, kvp.Value.GeneSetSize)

            If similarity > 0.7 Then ' 70%以上匹配
                Return kvp.Key
            End If
        Next

        Return Nothing
    End Function
End Class