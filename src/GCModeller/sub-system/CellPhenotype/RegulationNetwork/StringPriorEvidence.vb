Imports SMRUCC.genomics.Data.STRING.Tabular.Tsv

Namespace RegulationNetwork

    ''' <summary>
    ''' STRING 蛋白互作证据源
    ''' </summary>
    ''' <remarks>
    ''' STRING 的 <c>9606.protein.links.vXX.txt</c> 通常有上千万行，
    ''' 因此这里采用流式读取 + 边读边过滤的方式：只为网络内出现的基因建立 STRING id 索引，
    ''' 命中两端都在基因集合内的边才物化到内存，避免整体加载导致的内存溢出。
    '''
    ''' 注意：STRING 的 PPI 是「蛋白层面的物理 / 功能关联」，本身不带方向、也不带激活 / 抑制符号，
    ''' 与 mRNA 层面的共表达是不同语义的证据，因此本类只提供分数查询，
    ''' 方向判定与符号判定仍由 TF 注释与共表达符号负责。
    ''' </remarks>
    Public Class StringPriorEvidence

        ReadOnly scores As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)
        ReadOnly minScoreValue As Double

        ''' <summary>
        ''' 已物化的互作对数
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Count As Integer
            Get
                Return scores.Count
            End Get
        End Property

        ''' <summary>
        ''' 生效的最低 combined_score
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property MinScore As Double
            Get
                Return minScoreValue
            End Get
        End Property

        Private Sub New(minScore As Double)
            minScoreValue = minScore
        End Sub

        ''' <summary>
        ''' 从 STRING links 文件加载证据
        ''' </summary>
        ''' <param name="linksFile"><c>9606.protein.links.vXX.txt</c> 文件路径</param>
        ''' <param name="geneSet">网络中出现的基因 ID 集合（表达矩阵的 geneID）</param>
        ''' <param name="idMap">基因 ID → STRING protein id 的映射；为 Nothing 时用基因 ID 直接匹配</param>
        ''' <param name="minScore">最低 combined_score（0-1000）</param>
        ''' <param name="verbose">是否输出加载日志</param>
        ''' <returns>证据源对象；文件为空时返回空证据源</returns>
        Public Shared Function Load(linksFile As String,
                                    geneSet As IEnumerable(Of String),
                                    Optional idMap As Dictionary(Of String, String) = Nothing,
                                    Optional minScore As Double = 700,
                                    Optional verbose As Boolean = True) As StringPriorEvidence

            Dim evidence As New StringPriorEvidence(minScore)

            If String.IsNullOrEmpty(linksFile) OrElse geneSet Is Nothing Then
                Return evidence
            End If

            Dim accepted As Dictionary(Of String, String) = BuildAcceptedIds(geneSet, idMap)

            If verbose Then
                Call $"STRING: load protein links from '{linksFile}' with {accepted.Count} accepted protein id forms (min score = {minScore})...".info
            End If

            Dim n As Integer = 0

            For Each link As linksDetail In linksDetail.IteratesLinks(linksFile)
                If link.combined_score < minScore Then
                    Continue For
                End If

                Dim a As String = Nothing
                Dim b As String = Nothing

                If Not accepted.TryGetValue(link.protein1, a) Then Continue For
                If Not accepted.TryGetValue(link.protein2, b) Then Continue For
                If String.Equals(a, b, StringComparison.OrdinalIgnoreCase) Then Continue For

                Dim key As String = PairKey(a, b)
                Dim score As Double = link.combined_score

                If Not evidence.scores.ContainsKey(key) OrElse evidence.scores(key) < score Then
                    evidence.scores(key) = score
                End If

                n += 1
            Next

            If verbose Then
                Call $"STRING: {evidence.scores.Count} interactions kept for {geneSet.Count} genes ({n} raw hits).".info
            End If

            Return evidence
        End Function

        ''' <summary>
        ''' 查询两个基因之间的 STRING combined_score
        ''' </summary>
        ''' <param name="a">基因 ID</param>
        ''' <param name="b">基因 ID</param>
        ''' <returns>combined_score（0-1000）；无互作记录或低于阈值时返回 0</returns>
        Public Function Lookup(a As String, b As String) As Double
            Dim score As Double = 0

            If scores.TryGetValue(PairKey(a, b), score) Then
                Return score
            End If

            Return 0
        End Function

        ''' <summary>
        ''' 是否存在达到阈值的互作记录
        ''' </summary>
        ''' <param name="a">基因 ID</param>
        ''' <param name="b">基因 ID</param>
        ''' <returns></returns>
        Public Function HasLink(a As String, b As String) As Boolean
            Return scores.ContainsKey(PairKey(a, b))
        End Function

        ''' <summary>
        ''' 生成无序基因对的字典键
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <returns></returns>
        Public Shared Function PairKey(a As String, b As String) As String
            If a Is Nothing Then a = ""
            If b Is Nothing Then b = ""

            If String.Compare(a, b, StringComparison.OrdinalIgnoreCase) <= 0 Then
                Return a & "|" & b
            End If

            Return b & "|" & a
        End Function

        ''' <summary>
        ''' 把基因集合展开为可接受的 STRING id 形式（原样 + 去掉物种前缀 + 映射表形式）
        ''' </summary>
        Private Shared Function BuildAcceptedIds(geneSet As IEnumerable(Of String),
                                                 idMap As Dictionary(Of String, String)) As Dictionary(Of String, String)
            Dim accepted As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

            For Each gene As String In geneSet.Distinct
                If String.IsNullOrEmpty(gene) Then Continue For

                accepted(gene) = gene

                Dim stripped As String = StripSpecies(gene)

                If Not String.IsNullOrEmpty(stripped) AndAlso Not accepted.ContainsKey(stripped) Then
                    accepted(stripped) = gene
                End If

                If idMap IsNot Nothing Then
                    Dim mapped As String = Nothing

                    If idMap.TryGetValue(gene, mapped) AndAlso Not String.IsNullOrEmpty(mapped) Then
                        If Not accepted.ContainsKey(mapped) Then
                            accepted(mapped) = gene
                        End If

                        Dim mappedStripped As String = StripSpecies(mapped)

                        If Not String.IsNullOrEmpty(mappedStripped) AndAlso Not accepted.ContainsKey(mappedStripped) Then
                            accepted(mappedStripped) = gene
                        End If
                    End If
                End If
            Next

            Return accepted
        End Function

        ''' <summary>
        ''' 去掉 STRING id 的物种前缀（<c>9606.ENSP00000000233</c> → <c>ENSP00000000233</c>）
        ''' </summary>
        Private Shared Function StripSpecies(id As String) As String
            If String.IsNullOrEmpty(id) Then
                Return id
            End If

            Dim dot As Integer = id.IndexOf("."c)

            If dot <= 0 OrElse dot = id.Length - 1 Then
                Return Nothing
            End If

            Dim prefix As String = id.Substring(0, dot)

            For Each c As Char In prefix
                If Not Char.IsDigit(c) Then
                    Return Nothing
                End If
            Next

            Return id.Substring(dot + 1)
        End Function
    End Class
End Namespace
