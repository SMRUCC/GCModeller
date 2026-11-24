' ============================================================================
' PhenotypeClusters.vb
'
' 表型 GSEA 知识背景构建的「组装层」。
'
'   1. 把目标基因组的 PfamString 蛋白质组注释一次性构建成 Pfam -> 基因倒排索引，
'      避免对每个表型重复遍历全蛋白质组；
'   2. 把单条 ReportJSON + 已经过权重筛选的 PfamTermScore 序列转换为一个 Cluster。
'
' 两种成员粒度由是否传入 PfamGeneIndex 决定：
'   不传 -> 结构域级（Cluster 成员是筛选后的 Pfam 编号）
'   传入 -> 基因级（Cluster 成员是携带这些 Pfam 的 ProteinId，默认）
' ============================================================================

Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.metaTraits.Traitar
Imports SMRUCC.genomics.ProteinModel

''' <summary>
''' 目标基因组蛋白质组 Pfam 结构域注释的倒排索引：
''' Pfam 编号 -&gt; [基因编号 -&gt; 该基因上命中的 Pfam 编号集合]。
''' </summary>
Public Class PfamGeneIndex

    Private ReadOnly genes As Dictionary(Of String, PfamString)
    Private ReadOnly index As Dictionary(Of String, Dictionary(Of String, HashSet(Of String)))

    Friend Sub New(genes As Dictionary(Of String, PfamString),
                   index As Dictionary(Of String, Dictionary(Of String, HashSet(Of String))))
        Me.genes = genes
        Me.index = index
    End Sub

    ''' <summary>全基因组之中有 Pfam 注释（或无注释但有效）的唯一基因数量</summary>
    Public ReadOnly Property GeneCount As Integer
        Get
            Return genes.Count
        End Get
    End Property

    ''' <summary>全基因组之中出现的唯一 Pfam 结构域数量</summary>
    Public ReadOnly Property DomainCount As Integer
        Get
            Return index.Count
        End Get
    End Property

    ''' <summary>全基因组的唯一基因编号（按字典序）</summary>
    Public ReadOnly Property GeneIds As String()
        Get
            Return genes.Keys _
                .OrderBy(Function(id) id, StringComparer.Ordinal) _
                .ToArray()
        End Get
    End Property

    ''' <summary>判断指定的 Pfam 编号是否存在于该基因组之中</summary>
    Public Function ContainsDomain(pfamId As String) As Boolean
        Return pfamId IsNot Nothing AndAlso index.ContainsKey(pfamId)
    End Function

    ''' <summary>
    ''' 把一组经过权重筛选的 Pfam 词条映射为基因集合。
    ''' 同一个基因命中多个 Pfam 时会合并为一条 <see cref="BackgroundGene"/> 记录，
    ''' 所有命中的 Pfam 编号写入其 <c>term_id</c>。
    ''' </summary>
    ''' <param name="terms">该表型入选的 Pfam 词条</param>
    ''' <returns>按基因编号排序的成员数组，无命中时返回空数组（永远不为 Nothing）</returns>
    Public Function MembersFor(terms As IEnumerable(Of PfamTermScore)) As BackgroundGene()
        Dim hitsByGene As New Dictionary(Of String, HashSet(Of String))(StringComparer.Ordinal)

        If terms IsNot Nothing Then
            For Each term As PfamTermScore In terms
                If term Is Nothing OrElse String.IsNullOrEmpty(term.PfamId) Then
                    Continue For
                End If

                Dim geneHits As Dictionary(Of String, HashSet(Of String)) = Nothing

                If Not index.TryGetValue(term.PfamId, geneHits) Then
                    Continue For
                End If

                For Each hit As KeyValuePair(Of String, HashSet(Of String)) In geneHits
                    If Not hitsByGene.ContainsKey(hit.Key) Then
                        hitsByGene.Add(hit.Key, New HashSet(Of String)(StringComparer.Ordinal))
                    End If

                    For Each pfamId As String In hit.Value
                        Call hitsByGene(hit.Key).Add(pfamId)
                    Next
                Next
            Next
        End If

        Dim members As New List(Of BackgroundGene)()

        For Each geneId As String In hitsByGene.Keys.OrderBy(Function(id) id, StringComparer.Ordinal)
            Dim protein As PfamString = Nothing
            Call genes.TryGetValue(geneId, protein)

            Dim hitIds As String() = hitsByGene(geneId) _
                .OrderBy(Function(id) id, StringComparer.Ordinal) _
                .ToArray()
            Dim displayName As String = If(protein Is Nothing OrElse String.IsNullOrWhiteSpace(protein.Description),
                                           geneId, protein.Description)
            ' New BackgroundGene(id, name) 已经填好 accessionID / locus_tag / alias
            Dim gene As New BackgroundGene(geneId, displayName)

            gene.term_id = BackgroundGene.TermSet("Pfam", hitIds).ToArray()
            members.Add(gene)
        Next

        Return members.ToArray()
    End Function

    Public Overrides Function ToString() As String
        Return $"{GeneCount} gene(s), {DomainCount} pfam domain(s)"
    End Function
End Class

''' <summary>
''' 单条表型注释到一个 GSEA 功能聚类的组装
''' </summary>
Public Module PhenotypeClusters

    ''' <summary>
    ''' 从目标基因组的全部 Pfam 结构域注释构建倒排索引。
    ''' </summary>
    ''' <param name="proteins">该基因组所预测出来的蛋白质组 Pfam 注释</param>
    ''' <returns>索引对象；输入为 Nothing 时返回空索引</returns>
    <Extension>
    Public Function BuildGeneIndex(proteins As IEnumerable(Of PfamString)) As PfamGeneIndex
        Dim genes As New Dictionary(Of String, PfamString)(StringComparer.Ordinal)
        Dim index As New Dictionary(Of String, Dictionary(Of String, HashSet(Of String)))(StringComparer.Ordinal)

        If proteins Is Nothing Then
            Return New PfamGeneIndex(genes, index)
        End If

        For Each protein As PfamString In proteins
            If protein Is Nothing OrElse String.IsNullOrWhiteSpace(protein.ProteinId) Then
                Continue For
            End If

            Dim geneId As String = protein.ProteinId.Trim()

            If Not genes.ContainsKey(geneId) Then
                genes.Add(geneId, protein)
            End If

            For Each pfamId As String In EnumerateDomains(protein)
                If Not index.ContainsKey(pfamId) Then
                    index.Add(pfamId, New Dictionary(Of String, HashSet(Of String))(StringComparer.Ordinal))
                End If

                Dim geneHits As Dictionary(Of String, HashSet(Of String)) = index(pfamId)

                If Not geneHits.ContainsKey(geneId) Then
                    geneHits.Add(geneId, New HashSet(Of String)(StringComparer.Ordinal))
                End If

                Call geneHits(geneId).Add(pfamId)
            Next
        Next

        Return New PfamGeneIndex(genes, index)
    End Function

    ''' <summary>
    ''' 把单条表型注释转换为一个 <see cref="Cluster"/>。
    ''' </summary>
    ''' <param name="report">表型预测报告</param>
    ''' <param name="id">全局唯一的聚类标识（由 AsTermDocuments 生成）</param>
    ''' <param name="terms">该表型经过权重打分与筛选之后的 Pfam 词条</param>
    ''' <param name="geneIndex">
    ''' 目标基因组的倒排索引；传入时生成「基因级」成员（默认用法），
    ''' 省略时生成「Pfam 结构域级」成员。
    ''' </param>
    <Extension>
    Public Function AsCluster(report As ReportJSON,
                              id As String,
                              terms As IEnumerable(Of PfamTermScore),
                              Optional geneIndex As PfamGeneIndex = Nothing) As Cluster

        Dim scores As PfamTermScore() = If(terms Is Nothing,
                                           New PfamTermScore() {},
                                           terms.ToArray())
        Dim members As BackgroundGene() = If(geneIndex Is Nothing,
                                             DomainMembers(scores),
                                             geneIndex.MembersFor(scores))
        Dim category As String() = CategoryParts(report)
        Dim clusterId As String = If(String.IsNullOrWhiteSpace(id),
                                     If(String.IsNullOrWhiteSpace(report.accession), "phenotype", report.accession),
                                     id)

        Return New Cluster With {
            .ID = clusterId,
            .names = If(String.IsNullOrWhiteSpace(report.accession), clusterId, report.accession),
            .description = Describe(report, scores),
            .category = category(0),
            .[class] = category(1),
            .members = members
        }
    End Function

    ''' <summary>
    ''' 单条表型注释 + 蛋白质组注释直接生成「基因级」聚类的便捷重载。
    ''' </summary>
    <Extension>
    Public Function AsCluster(report As ReportJSON,
                              terms As IEnumerable(Of PfamTermScore),
                              proteins As IEnumerable(Of PfamString),
                              Optional id As String = Nothing) As Cluster

        Return report.AsCluster(id, terms, proteins.BuildGeneIndex())
    End Function

    ''' <summary>结构域级成员：每个入选的 Pfam 编号本身作为一个背景成员</summary>
    Private Function DomainMembers(scores As PfamTermScore()) As BackgroundGene()
        Dim members As New List(Of BackgroundGene)()

        For Each term As PfamTermScore In scores
            If term Is Nothing OrElse String.IsNullOrWhiteSpace(term.PfamId) Then
                Continue For
            End If

            Dim gene As New BackgroundGene(term.PfamId,
                                           If(String.IsNullOrWhiteSpace(term.Description),
                                              term.PfamId, term.Description))

            gene.term_id = BackgroundGene.TermSet("Pfam", term.PfamId).ToArray()
            members.Add(gene)
        Next

        Return members.ToArray()
    End Function

    ''' <summary>
    ''' 枚举单条蛋白注释之中出现的全部 Pfam 编号。
    ''' 复用 <see cref="PfamEmbedding.ParsePfamId"/> 去掉版本号并过滤 ChouFasman 二级结构脏数据；
    ''' 当 Pfam-string 缺失时退回使用 Domains 属性。
    ''' </summary>
    Private Iterator Function EnumerateDomains(protein As PfamString) As IEnumerable(Of String)
        Dim tokens As IEnumerable(Of String) = Nothing

        If protein.PfamString IsNot Nothing AndAlso protein.PfamString.Length > 0 Then
            tokens = protein.PfamString
        ElseIf protein.Domains IsNot Nothing Then
            tokens = protein.Domains
        End If

        If tokens Is Nothing Then
            Return
        End If

        For Each token As String In tokens
            Dim pfamId As String = PfamEmbedding.ParsePfamId(token)

            If pfamId IsNot Nothing Then
                Yield pfamId
            End If
        Next
    End Function

    ''' <summary>把 "group_1/group_2" 拆为主分类与次分类，缺失部分补空串</summary>
    Private Function CategoryParts(report As ReportJSON) As String()
        If String.IsNullOrWhiteSpace(report.category) Then
            Return New String() {"", ""}
        End If

        Dim parts As String() = report.category.Split("/"c)
        Dim major As String = If(parts.Length > 0, parts(0).Trim(), "")
        Dim minor As String = If(parts.Length > 1, parts(1).Trim(), "")

        Return New String() {major, minor}
    End Function

    ''' <summary>把表型的元信息与入选 Pfam 词条汇总成可读的聚类描述</summary>
    Private Function Describe(report As ReportJSON, scores As PfamTermScore()) As String
        Dim sb As New StringBuilder()

        Call sb.AppendLine($"phenotype: {If(report.accession, report.phenotypeId)}")
        Call sb.AppendLine($"category: {If(report.category, "")}")
        Call sb.AppendLine($"data_type: {If(report.data_type, "")}; unit: {If(report.unit, "")}")
        Call sb.AppendLine($"predict: {If(report.predict, "")}; result: {report.result}; confidence: {report.confidence:F4}")
        Call sb.AppendLine($"model: status={If(report.status, "")}, cvScore={report.cvScore:F4}, sampleCount={report.sampleCount}")
        Call sb.AppendLine($"key pfam domains: {scores.Length}")

        For Each term As PfamTermScore In scores
            Call sb.AppendLine($"  {term.PfamId} | weight={term.Weight:F4} | importance={term.Importance:F4} | {If(term.Description, "")}")
        Next

        Return sb.ToString().TrimEnd()
    End Function
End Module
