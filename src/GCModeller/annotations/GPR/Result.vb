Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Statistics.Linq
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ContextModel
Imports SMRUCC.genomics.MetabolicModel

''' <summary>
''' 一条"基因 -&gt; 反应"关联的打分结果。
''' 
''' 除了最终分数之外，还保留了推导出该分数的全部 <see cref="Evidences"/>，
''' 因此结果表可以直接展示"评分依据"，任何异常分数都可以被追溯。
''' </summary>
Public Class ScoredReaction : Implements INamedValue

    Public Property Id As String Implements INamedValue.Key

    ''' <summary>
    ''' 聚合后的置信分数，取值 [0, <see cref="GPRParameters.ScoreCap"/>]
    ''' </summary>
    Public Property Score As Double

    ''' <summary>
    ''' 该条目是否为"未映射 EC"的占位标记
    ''' </summary>
    Public Property Unmapped As Boolean = False

    ''' <summary>
    ''' 推导出 <see cref="Score"/> 的全部证据
    ''' </summary>
    Public Property Evidences As New List(Of AssociationEvidence)

    ''' <summary>
    ''' 证据类型的去重列表
    ''' </summary>
    Public ReadOnly Property EvidenceKinds As String()
        Get
            If Evidences Is Nothing Then Return New String() {}
            Return Evidences.Select(Function(e) e.Kind.ToString).Distinct().OrderBy(Function(x) x).ToArray
        End Get
    End Property

    ''' <summary>
    ''' 证据来源摘要，例如 ``DirectEC+OperonContext``
    ''' </summary>
    Public ReadOnly Property EvidenceSummary As String
        Get
            Return Evidences.Summarize()
        End Get
    End Property

    ''' <summary>
    ''' 证据来源的详细描述，例如 ``OperonContext(1.000x0.650) &lt;- g02 (2.7.1.1)``
    ''' </summary>
    Public ReadOnly Property EvidenceDetail As String
        Get
            Return Evidences.Describe()
        End Get
    End Property

    ''' <summary>
    ''' 置信等级：high / medium / low / unmapped
    ''' </summary>
    Public ReadOnly Property ConfidenceLevel As String
        Get
            If Unmapped Then Return "unmapped"
            If Score >= 0.8 Then Return "high"
            If Score >= 0.5 Then Return "medium"
            Return "low"
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"{Id}: {Score.ToString("F4")} [{EvidenceSummary}]"
    End Function

End Class

''' <summary>
''' 一个基因的全部基因-反应关联结果
''' </summary>
Public Class GeneAssociation : Implements INamedValue

    Public Property GeneId As String Implements INamedValue.Key

    ''' <summary>
    ''' 反应编号 -&gt; 打分结果
    ''' </summary>
    Public Property Reactions As New Dictionary(Of String, ScoredReaction)(StringComparer.OrdinalIgnoreCase)

    ''' <summary>
    ''' 该基因携带、但在参考反应网络中找不到任何对应反应的 EC 编号。
    ''' 
    ''' 这些 EC 编号以独立字段保存，不会被伪装成反应编号写入 <see cref="Reactions"/>，
    ''' 从而保证结果表中的反应编号始终是真实的反应标识。
    ''' </summary>
    Public Property UnmappedECNumbers As String() = New String() {}

    ''' <summary>
    ''' 关联的反应条目总数
    ''' </summary>
    Public ReadOnly Property GPRLinks As Integer
        Get
            Return Reactions.Count
        End Get
    End Property

    ''' <summary>
    ''' 已映射到真实反应的关联条目数
    ''' </summary>
    Public ReadOnly Property MappedLinks As Integer
        Get
            Return Reactions.Values.Count(Function(r) Not r.Unmapped)
        End Get
    End Property

    Public ReadOnly Property MeanScore As Double
        Get
            If Reactions.Count = 0 Then Return 0
            Return Reactions.Values.Average(Function(a) a.Score)
        End Get
    End Property

    Public ReadOnly Property MedianScore As Double
        Get
            If Reactions.Count = 0 Then Return 0
            Return Reactions.Values.Select(Function(a) a.Score).Median
        End Get
    End Property

    Public ReadOnly Property MaxScore As Double
        Get
            If Reactions.Count = 0 Then Return 0
            Return Reactions.Values.Max(Function(a) a.Score)
        End Get
    End Property

    ''' <summary>
    ''' 高于该基因平均分的关联（按分数降序）
    ''' </summary>
    Public ReadOnly Property TopGPRLinks As String()
        Get
            If Reactions.Count = 0 Then Return New String() {}

            Dim cutoff As Double = MeanScore

            Return Reactions.Values _
                .Where(Function(r) Not r.Unmapped AndAlso r.Score >= cutoff) _
                .OrderByDescending(Function(r) r.Score) _
                .Select(Function(r) r.Id) _
                .ToArray
        End Get
    End Property

    ''' <summary>
    ''' 该基因出现过的全部证据类型
    ''' </summary>
    Public ReadOnly Property EvidenceKinds As String()
        Get
            Return Reactions.Values _
                .SelectMany(Function(r) r.EvidenceKinds) _
                .Distinct() _
                .OrderBy(Function(x) x) _
                .ToArray
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"{GeneId} - [{Reactions.Count}] {String.Join(", ", Reactions.Keys)}"
    End Function

End Class

''' <summary>
''' 基因组上下文 + 基因-反应关联结果的统一容器。
''' 
''' <see cref="MetabolicNetwork"/>、反应到基因的反向索引以及直接证据种子集合，
''' 全部由 <see cref="RebuildNetwork"/> / <see cref="SetSeedReactions"/> 这两个统一入口维护，
''' 避免出现"算法内部使用一份数据、对外输出另一份数据"的不一致问题。
''' </summary>
Public Class Genome : Inherits GenomeContext(Of GeneTable)

    ''' <summary>
    ''' 基因编号 -&gt; 关联结果
    ''' </summary>
    Public Property MetabolicNetwork As New Dictionary(Of String, GeneAssociation)(StringComparer.OrdinalIgnoreCase)

    ''' <summary>
    ''' 反应编号 -&gt; 支持该反应的基因编号（O(1) 反向索引）
    ''' </summary>
    Private ReadOnly reaction2genes As New Dictionary(Of String, List(Of String))(StringComparer.OrdinalIgnoreCase)

    ''' <summary>
    ''' 基因编号 -&gt; 该基因的直接证据反应编号
    ''' </summary>
    Private ReadOnly gene2seed As New Dictionary(Of String, String())(StringComparer.OrdinalIgnoreCase)

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="genome"></param>
    ''' <param name="name"></param>
    ''' <remarks>
    ''' impute <paramref name="genome"/> context data has been sorted by left in asc order
    ''' </remarks>
    Public Sub New(genome As IEnumerable(Of GeneTable), Optional name As String = "unnamed")
        MyBase.New(genome, name)
    End Sub

    ''' <summary>
    ''' 登记每个基因的"直接证据"反应集合，供后续阶段（共表达推断等）查询使用。
    ''' 
    ''' 必须在阶段 1 结束之后立即调用，否则共表达等跨基因证据会因为网络为空而完全失效。
    ''' </summary>
    Public Sub SetSeedReactions(seeds As Dictionary(Of String, String()))
        gene2seed.Clear()

        If seeds Is Nothing Then Return

        For Each item As KeyValuePair(Of String, String()) In seeds
            gene2seed(item.Key) = If(item.Value, New String() {})
        Next
    End Sub

    ''' <summary>
    ''' 获取某个基因的直接证据反应集合
    ''' </summary>
    Public Function GetSeedReactions(geneId As String) As String()
        Dim seeds As String() = Nothing
        If String.IsNullOrEmpty(geneId) Then Return New String() {}
        If gene2seed.TryGetValue(geneId, seeds) Then Return seeds

        Return New String() {}
    End Function

    ''' <summary>
    ''' 统一物化入口：同时重建正向关联表与"反应 -&gt; 基因"的反向索引
    ''' </summary>
    Public Sub RebuildNetwork(associations As IEnumerable(Of GeneAssociation))
        MetabolicNetwork.Clear()
        reaction2genes.Clear()

        If associations Is Nothing Then Return

        For Each association As GeneAssociation In associations
            If association Is Nothing OrElse String.IsNullOrEmpty(association.GeneId) Then Continue For

            MetabolicNetwork(association.GeneId) = association

            For Each reaction As ScoredReaction In association.Reactions.Values
                If reaction.Unmapped Then Continue For

                If Not reaction2genes.ContainsKey(reaction.Id) Then
                    reaction2genes(reaction.Id) = New List(Of String)
                End If

                Dim owners As List(Of String) = reaction2genes(reaction.Id)
                If Not owners.Contains(association.GeneId) Then owners.Add(association.GeneId)
            Next
        Next
    End Sub

    ''' <summary>
    ''' 获取某个基因的关联结果
    ''' </summary>
    Public Function GetGeneAssociation(geneId As String) As GeneAssociation
        Dim result As GeneAssociation = Nothing
        If String.IsNullOrEmpty(geneId) Then Return Nothing
        If MetabolicNetwork.TryGetValue(geneId, result) Then Return result

        Return Nothing
    End Function

    ''' <summary>
    ''' 获取某个基因关联的反应打分结果
    ''' </summary>
    Public Function GetGeneReactions(geneId As String) As IEnumerable(Of ScoredReaction)
        Dim association As GeneAssociation = GetGeneAssociation(geneId)
        If association Is Nothing Then Return New ScoredReaction() {}

        Return association.Reactions.Values
    End Function

    ''' <summary>
    ''' 获取支持某个反应的全部基因（基于反向索引，O(1) 命中）
    ''' </summary>
    Public Iterator Function GetGenesForReaction(id As String) As IEnumerable(Of GeneTable)
        Dim owners As List(Of String) = Nothing
        If String.IsNullOrEmpty(id) Then Return
        If Not reaction2genes.TryGetValue(id, owners) Then Return

        For Each geneId As String In owners
            Dim gene As GeneTable = Me(geneId)
            If gene IsNot Nothing Then Yield gene
        Next
    End Function

End Class
