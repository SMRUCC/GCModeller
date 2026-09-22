Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.MetabolicModel

''' <summary>
''' 基于多个基因组的保守基因邻接进行功能推断。
''' 
''' 如果多个物种中某些基因总是相邻出现，那么它们的功能很可能相关。
''' </summary>
Public Class ConservedSyntenyAnalyzer

    ''' <summary>
    ''' 保守簇编号 -&gt; 保守簇
    ''' </summary>
    ReadOnly conservedClusters As Dictionary(Of String, ConservedCluster)

    Public Sub New(conservationData As Dictionary(Of String, ConservedCluster))
        Me.conservedClusters = If(
            conservationData,
            New Dictionary(Of String, ConservedCluster)(StringComparer.OrdinalIgnoreCase))
    End Sub

    ''' <summary>
    ''' 已知保守簇的数量
    ''' </summary>
    Public ReadOnly Property ClusterCount As Integer
        Get
            Return conservedClusters.Count
        End Get
    End Property

    ''' <summary>
    ''' 在目标基因周围构建滑动窗口，并在已知保守簇中查找匹配项。
    ''' 
    ''' 返回匹配到的保守簇编号，未匹配时返回 <c>Nothing</c>。
    ''' </summary>
    Public Function FindClusterKey(genome As GeneTable(), geneIndex As Integer, opt As GPRParameters) As String
        If genome Is Nothing OrElse genome.Length = 0 Then Return Nothing
        If opt Is Nothing Then opt = New GPRParameters
        If conservedClusters.Count = 0 Then Return Nothing
        If geneIndex < 0 OrElse geneIndex >= genome.Length Then Return Nothing

        Dim window As String() = GetClusterGeneIds(genome, geneIndex, opt.SyntenyClusterSize)
        If window.Length = 0 Then Return Nothing

        Dim windowSet As New HashSet(Of String)(window, StringComparer.OrdinalIgnoreCase)

        Dim bestKey As String = Nothing
        Dim bestSimilarity As Double = 0

        For Each item As KeyValuePair(Of String, ConservedCluster) In conservedClusters
            Dim cluster As ConservedCluster = item.Value
            If cluster Is Nothing Then Continue For

            ' 修复：原实现把 ConservedCluster 对象本身当作集合传给 Intersect，
            ' 在 Option Strict Off 下可以编译，但运行时会抛出类型转换异常。
            ' 这里使用真正承载基因集合的 GetGeneIDs()。
            Dim geneIds As String() = cluster.GetGeneIDs()
            Dim denominator As Integer = Math.Min(windowSet.Count, geneIds.Length)

            ' 修复：原实现未做除零保护
            If denominator <= 0 Then Continue For

            Dim overlap As Integer = windowSet _
                .Where(Function(id) geneIds.Contains(id, StringComparer.OrdinalIgnoreCase)) _
                .Count()
            Dim similarity As Double = CDbl(overlap) / denominator

            If similarity >= opt.SyntenySimilarityThreshold AndAlso similarity > bestSimilarity Then
                bestSimilarity = similarity
                bestKey = If(String.IsNullOrEmpty(cluster.ClusterID), item.Key, cluster.ClusterID)
            End If
        Next

        Return bestKey
    End Function

    ''' <summary>
    ''' 收集某个基因的保守共线性证据
    ''' </summary>
    Public Function CollectEvidence(gene As GeneTable,
                                    genome As GeneTable(),
                                    geneIndex As Integer,
                                    context As ContextIndices,
                                    opt As GPRParameters) As IEnumerable(Of ReactionEvidence)

        Dim results As New List(Of ReactionEvidence)

        If gene Is Nothing OrElse context Is Nothing Then Return results
        If opt Is Nothing Then opt = New GPRParameters

        Dim clusterKey As String = FindClusterKey(genome, geneIndex, opt)
        If String.IsNullOrEmpty(clusterKey) Then Return results

        Dim cluster As ConservedCluster = Nothing
        If Not conservedClusters.TryGetValue(clusterKey, cluster) Then
            ' ClusterID 可能与字典键不一致，退化为一次线性查找
            cluster = conservedClusters.Values.FirstOrDefault(
                Function(c) c IsNot Nothing AndAlso String.Equals(c.ClusterID, clusterKey, StringComparison.OrdinalIgnoreCase))
        End If

        If cluster Is Nothing Then Return results

        Dim similarity As Double = 1.0
        Dim functions As String() = cluster.GetFunctions()

        For Each func As String In functions
            ' 修复：不再使用 StartsWith("R") 猜测标识符前缀，
            ' 而是依次尝试按反应编号、EC 编号、通路编号进行解析。
            For Each reactionId As String In ResolveFunction(func, context)
                results.Add(New ReactionEvidence(reactionId, New AssociationEvidence With {
                    .Kind = EvidenceKind.ConservedSynteny,
                    .Weight = opt.BaseSyntenyScore,
                    .RawScore = similarity,
                    .Source = $"{clusterKey} ({func})"
                }))
            Next
        Next

        Return results
    End Function

    ''' <summary>
    ''' 把一个保守簇中记录的功能标识解析为具体的反应集合
    ''' </summary>
    Private Iterator Function ResolveFunction(func As String, context As ContextIndices) As IEnumerable(Of String)
        If String.IsNullOrEmpty(func) Then Return

        Dim key As String = func.Trim

        ' 1) 直接是反应编号
        If context.ReactionIndex.ContainsKey(key) Then
            Yield key
            Return
        End If

        ' 2) 是 EC 编号
        Dim byEC As List(Of MetabolicReaction) = context.GetReactionsByEC(key)
        If byEC.Count > 0 Then
            For Each reaction As MetabolicReaction In byEC
                Yield reaction.id
            Next

            Return
        End If

        ' 3) 是通路编号
        Dim members As List(Of MetabolicReaction) = Nothing
        If context.PathwayReactions.TryGetValue(key, members) Then
            For Each reaction As MetabolicReaction In members
                Yield reaction.id
            Next
        End If
    End Function

    ''' <summary>
    ''' 以目标基因为中心构建滑动窗口内的基因编号集合
    ''' </summary>
    Private Shared Function GetClusterGeneIds(genome As GeneTable(), geneIndex As Integer, radius As Integer) As String()
        Dim size As Integer = Math.Max(0, radius)
        Dim startIdx As Integer = Math.Max(0, geneIndex - size)
        Dim endIdx As Integer = Math.Min(genome.Length - 1, geneIndex + size)

        Dim ids As New List(Of String)

        For i As Integer = startIdx To endIdx
            Dim gene As GeneTable = genome(i)
            If gene Is Nothing OrElse String.IsNullOrEmpty(gene.locus_id) Then Continue For

            ids.Add(gene.locus_id)
        Next

        Return ids.ToArray
    End Function

End Class
