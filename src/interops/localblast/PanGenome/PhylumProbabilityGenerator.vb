Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

''' <summary>
''' Phylum probability matrix generator for operon predict 
''' </summary>
Public Module PhylumProbabilityGenerator

    ''' <summary>
    ''' 基于泛基因组分析结果生成 phylumProbabilities 字典
    ''' </summary>
    ''' <param name="hits">DIAMOND 比对结果列表</param>
    ''' <param name="gene_source">基因ID到基因组ID的映射字典</param>
    ''' <param name="genome_phylum">基因组ID到门分类的映射字典</param>
    ''' <param name="identityThreshold">用于过滤比对结果的相似度阈值 (例如 0.3 表示 30%)</param>
    ''' <returns>嵌套字典：基因ID -> (门 -> 存在概率pik)</returns>
    ''' 
    <Extension>
    Public Function GeneratePhylumProbabilities(hits As IEnumerable(Of BiDirectionalBesthit), gene_source As Dictionary(Of String, String), genome_phylum As Dictionary(Of String, String), Optional identityThreshold As Double = 0.0) As Dictionary(Of String, Dictionary(Of String, Double))
        ' 1. 初始化并查集，进行基因家族聚类
        Dim uf As New UnionFind()

        For Each hit In hits
            ' 根据相似度阈值过滤
            If hit.identities >= identityThreshold Then
                uf.AddElement(hit.QueryName)
                uf.AddElement(hit.HitName)
                ' 使用 gene1 作为参考ID合并
                uf.Union(hit.QueryName, hit.HitName)
            End If
        Next

        Return uf.GeneratePhylumProbabilities(gene_source, genome_phylum, identityThreshold:=identityThreshold)
    End Function

    ''' <summary>
    ''' 基于泛基因组分析结果生成 phylumProbabilities 字典
    ''' </summary>
    ''' <param name="gene_source">基因ID到基因组ID的映射字典</param>
    ''' <param name="genome_phylum">基因组ID到门分类的映射字典</param>
    ''' <param name="identityThreshold">用于过滤比对结果的相似度阈值 (例如 0.3 表示 30%)</param>
    ''' <returns>嵌套字典：基因ID -> (门 -> 存在概率pik)</returns>
    ''' 
    <Extension>
    Public Function GeneratePhylumProbabilities(orthologGroup As UnionFind, gene_source As Dictionary(Of String, String), genome_phylum As Dictionary(Of String, String), Optional identityThreshold As Double = 0.0) As Dictionary(Of String, Dictionary(Of String, Double))
        ' 获取聚类结果：Key为家族代表基因ID，Value为该家族所有基因的列表
        Dim clusters As Dictionary(Of String, List(Of String)) = orthologGroup.GetClusters()

        ' 2. 预计算：统计每个门下的总基因组数量
        ' Key: 门, Value: 该门下所有基因组的 HashSet (自动去重)
        Dim phylumGenomeCounts As New Dictionary(Of String, HashSet(Of String))()
        For Each kvp In gene_source
            Dim genomeId As String = kvp.Value
            If genome_phylum.ContainsKey(genomeId) Then
                Dim phylum As String = genome_phylum(genomeId)
                If Not phylumGenomeCounts.ContainsKey(phylum) Then
                    phylumGenomeCounts.Add(phylum, New HashSet(Of String)())
                End If
                phylumGenomeCounts(phylum).Add(genomeId)
            End If
        Next

        ' 3. 计算每个基因家族在各个门中的存在频率，并构建最终字典
        Dim phylumProbabilities As New Dictionary(Of String, Dictionary(Of String, Double))()

        For Each cluster In clusters
            Dim familyRoot As String = cluster.Key
            Dim genesInFamily As List(Of String) = cluster.Value

            ' 临时统计该家族在各个门中覆盖了多少个不同的基因组
            ' Key: 门, Value: 包含该家族基因的基因组集合
            Dim phylumPresence As New Dictionary(Of String, HashSet(Of String))()

            For Each geneId In genesInFamily
                If gene_source.ContainsKey(geneId) Then
                    Dim genomeId As String = gene_source(geneId)
                    If genome_phylum.ContainsKey(genomeId) Then
                        Dim phylum As String = genome_phylum(genomeId)

                        If Not phylumPresence.ContainsKey(phylum) Then
                            phylumPresence.Add(phylum, New HashSet(Of String)())
                        End If

                        ' HashSet 自动去重，解决同一基因组中有多个旁系同源基因的问题
                        phylumPresence(phylum).Add(genomeId)
                    End If
                End If
            Next

            ' 计算该家族在每个门中的概率，并将结果赋予该家族下的所有基因
            For Each geneId In genesInFamily
                Dim probDict As New Dictionary(Of String, Double)

                For Each phylumKvp In phylumGenomeCounts
                    Dim phylum As String = phylumKvp.Key
                    Dim totalGenomesInPhylum As Integer = phylumKvp.Value.Count

                    Dim presentGenomes As Integer = 0
                    If phylumPresence.ContainsKey(phylum) Then
                        presentGenomes = phylumPresence(phylum).Count
                    End If

                    ' 计算 pik = 存在该家族的基因组数 / 该门总基因组数
                    Dim pik As Double = If(totalGenomesInPhylum > 0, presentGenomes / totalGenomesInPhylum, 0.0)
                    probDict.Add(phylum, pik)
                Next

                phylumProbabilities.Add(geneId, probDict)
            Next
        Next

        Return phylumProbabilities
    End Function
End Module
