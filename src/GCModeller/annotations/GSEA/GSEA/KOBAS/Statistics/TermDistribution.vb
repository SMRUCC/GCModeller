Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Namespace KOBAS

    ''' <summary>
    ''' 一个功能词条（通路 / GO 词条 / 疾病）以及当前分析集合之中命中它的基因。
    ''' </summary>
    Public Class TermMembers

        ''' <summary>
        ''' 词条编号。
        ''' </summary>
        ''' <returns></returns>
        Public Property ID As String

        ''' <summary>
        ''' 词条名称。
        ''' </summary>
        ''' <returns></returns>
        Public Property Names As String

        ''' <summary>
        ''' 词条所属的数据库缩写（<c>K</c>/<c>R</c>/<c>G</c> 等）。
        ''' </summary>
        ''' <returns></returns>
        Public Property Database As String

        ''' <summary>
        ''' 当前分析集合之中命中该词条的基因集合。
        ''' </summary>
        ''' <returns></returns>
        Public Property Genes As HashSet(Of String)

        ''' <summary>
        ''' 命中基因的数量。
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Size As Integer
            Get
                Return If(Genes Is Nothing, 0, Genes.Count)
            End Get
        End Property
    End Class

    ''' <summary>
    ''' 「词条 → 基因集合」的分布（KOBAS <c>discover.Distr</c> 的等价物）。
    ''' </summary>
    ''' <remarks>
    ''' KOBAS 的 <c>Distr</c> 是一个 <c>dict</c> 子类，键为
    ''' <c>(term_name, database, term_id)</c> 三元组、值为命中的基因集合。
    ''' 这里以词条编号作为字典键，词条名称与数据库标记放在 <see cref="TermMembers"/> 之中。
    ''' </remarks>
    Public Class TermDistribution

        ''' <summary>
        ''' 词条编号 → 词条成员。
        ''' </summary>
        ''' <returns></returns>
        Public Property Terms As New Dictionary(Of String, TermMembers)(StringComparer.Ordinal)

        Private cachedGeneCount As Integer = -1

        ''' <summary>
        ''' 当前分布之中出现过的基因并集大小（对应 KOBAS 的 <c>DistrGnum.gnum</c>）。
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property GeneCount As Integer
            Get
                If cachedGeneCount < 0 Then
                    Dim uniques As New HashSet(Of String)(StringComparer.Ordinal)

                    For Each term As TermMembers In Terms.Values
                        Call uniques.UnionWith(term.Genes)
                    Next

                    cachedGeneCount = uniques.Count
                End If

                Return cachedGeneCount
            End Get
        End Property

        ''' <summary>
        ''' 词条数量。
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property TermCount As Integer
            Get
                Return Terms.Count
            End Get
        End Property

        ''' <summary>
        ''' 取得指定词条的命中基因数量（词条不存在时返回 0）。
        ''' </summary>
        ''' <param name="term">词条编号。</param>
        ''' <returns></returns>
        Public Function Count(term As String) As Integer
            Dim members As TermMembers = Nothing

            If Terms.TryGetValue(term, members) Then
                Return members.Size
            Else
                Return 0
            End If
        End Function

        ''' <summary>
        ''' 取得「词条编号 → 命中基因数量」的计数表。
        ''' </summary>
        ''' <returns></returns>
        Public Function Counts() As Dictionary(Of String, Integer)
            Return Terms.ToDictionary(Function(term) term.Key,
                                      Function(term) term.Value.Size)
        End Function

        ''' <summary>
        ''' 从基因集背景模型构建词条分布。
        ''' </summary>
        ''' <param name="background">基因集背景模型。</param>
        ''' <param name="genes">
        ''' 参与统计的基因编号；为 <c>Nothing</c> 时使用背景模型之中的全部基因。
        ''' 前景与背景应当分别用各自的基因列表调用本方法。
        ''' </param>
        ''' <param name="database">词条所属数据库的缩写标记。</param>
        ''' <returns></returns>
        Public Shared Function Create(background As Background,
                                      Optional genes As IEnumerable(Of String) = Nothing,
                                      Optional database As String = "") As TermDistribution

            If background Is Nothing OrElse background.clusters.IsNullOrEmpty Then
                Throw New ArgumentException("The gene set background model contains no cluster.", NameOf(background))
            End If

            Dim filter As HashSet(Of String) = Nothing

            If genes IsNot Nothing Then
                filter = New HashSet(Of String)(genes.Where(Function(id) Not String.IsNullOrEmpty(id)), StringComparer.Ordinal)
            End If

            Dim distr As New TermDistribution

            For Each cluster As Cluster In background.clusters
                Dim term As TermMembers = Nothing

                For Each gene As BackgroundGene In If(cluster.members, New BackgroundGene() {})
                    Dim id As String = gene.accessionID

                    If String.IsNullOrEmpty(id) Then
                        Continue For
                    End If
                    If filter IsNot Nothing AndAlso Not filter.Contains(id) Then
                        Continue For
                    End If

                    If term Is Nothing Then
                        term = New TermMembers With {
                            .ID = cluster.ID,
                            .Names = cluster.names,
                            .Database = database,
                            .Genes = New HashSet(Of String)(StringComparer.Ordinal)
                        }
                        distr.Terms(cluster.ID) = term
                    End If

                    Call term.Genes.Add(id)
                Next
            Next

            Return distr
        End Function

        ''' <summary>
        ''' 只保留与给定的基因集合有交集的词条（KOBAS <c>TwoSampleTest.__call__</c>
        ''' 之中「只保留在前景与背景之中都出现的词条」的等价实现）。
        ''' </summary>
        ''' <param name="termIds">需要保留的词条编号集合。</param>
        ''' <returns>过滤之后的新的分布对象。</returns>
        Public Function RestrictTo(termIds As IEnumerable(Of String)) As TermDistribution
            Dim idSet As New HashSet(Of String)(termIds, StringComparer.Ordinal)
            Dim result As New TermDistribution

            For Each term As KeyValuePair(Of String, TermMembers) In Terms
                If idSet.Contains(term.Key) Then
                    result.Terms(term.Key) = term.Value
                End If
            Next

            Return result
        End Function
    End Class
End Namespace
