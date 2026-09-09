' ============================================================================
' SinkBuilder.vb — 与数据源无关的底盘内源代谢物集合（汇）构建
' ----------------------------------------------------------------------------
' [readme_alg.md §一] 汇 = 底盘菌株（如 E. coli）内源代谢物集合，通常从 GEM 模型提取。
' 逆向搜索的终止条件就是"全部前体落入汇集合"。
'
' 两种模式：
'   Core（默认）：枢纽代谢物（在反应中出现的次数 ≥ coreDegree）+ 白名单成员。
'     次级代谢途径的专有中间体不会被收录，因此搜索必须一路反推到中心代谢分支点
'     （如 chorismate），得到的是真正的多步生物合成通路。
'   All：全部有可用结构的化合物都算内源。此时目标自身若也在库内，BeamSearch 会
'     直接判定"目标已属于汇"而返回 0 条路径，因此查询时需要排除目标自身。
'
' 白名单分两种，可同时使用：
'   idWhitelist  —— 按数据源的 id 精确匹配（如 BioCyc 的 "PYRUVATE"），换库即失效；
'   nameWhitelist—— 按 id/名称/同义名归一化后匹配（如 "pyruvate"、"acetyl-coa"），
'                   跨数据库可移植，适用于来源不明的内部代谢模型。
'
' 辅因子（NAD/ATP/CoA/ACP/SAM…）在两种模式下都应进汇：它们确实内源存在，逆向生成时
' 即刻终止分支，等价于货币分子语义——避免把 ATP/CoA 当成"待合成前体"导致搜索爆炸。
' ============================================================================

Imports System.Text

Public Enum SinkModes
    ''' <summary>核心中心代谢子集（枢纽代谢物 + 白名单 + 辅因子）</summary>
    Core = 0
    ''' <summary>全部有可用结构的化合物（查询时排除目标自身）</summary>
    All = 1
End Enum

Public Module SinkBuilder

    ''' <summary>
    ''' 构建汇集合。
    ''' </summary>
    ''' <param name="structures">compound id → 已净化结构。</param>
    ''' <param name="degrees">compound id → 在反应中出现的总次数（可用
    ''' <see cref="CompoundIndex.DegreeOf(IEnumerable(Of ReactionSpec))"/> 计算）。</param>
    ''' <param name="mode">Core / All。</param>
    ''' <param name="coreDegree">Core 模式下判定"枢纽代谢物"的最少反应出现次数。</param>
    ''' <param name="idWhitelist">按 id 精确匹配的白名单（大小写不敏感）。</param>
    ''' <param name="nameWhitelist">
    ''' 按 id/名称/同义名归一化匹配的白名单（大小写、连字符、空格不敏感），跨数据库可移植。
    ''' </param>
    ''' <returns>汇集合，每项为 (化合物 id, 净化后的 SMILES)，按 id 排序。</returns>
    Public Function Build(structures As Dictionary(Of String, CompoundStructure),
                          degrees As Dictionary(Of String, Integer),
                          mode As SinkModes,
                          Optional coreDegree As Integer = 4,
                          Optional idWhitelist As IEnumerable(Of String) = Nothing,
                          Optional nameWhitelist As IEnumerable(Of String) = Nothing) As List(Of (String, smiles As String))

        Dim sink As New List(Of (String, smiles As String))()
        Dim ids As New List(Of String)(structures.Keys)

        ids.Sort(StringComparer.Ordinal)

        Dim byId As New HashSet(Of String)(
            If(idWhitelist, Array.Empty(Of String)()).Where(Function(s) s IsNot Nothing).Select(Function(s) s.Trim()),
            StringComparer.OrdinalIgnoreCase)
        Dim byName As New HashSet(Of String)(
            If(nameWhitelist, Array.Empty(Of String)()).Where(Function(s) s IsNot Nothing).Select(Function(s) Normalize(s)),
            StringComparer.Ordinal)

        For Each id As String In ids
            Dim st As CompoundStructure = structures(id)

            If mode = SinkModes.All Then
                sink.Add((id, st.Smiles))
                Continue For
            End If

            Dim deg As Integer = 0
            degrees.TryGetValue(id, deg)

            If deg >= coreDegree OrElse
               byId.Contains(id) OrElse
               MatchesName(st, byName) Then
                sink.Add((id, st.Smiles))
            End If
        Next

        Return sink
    End Function

    ''' <summary>
    ''' 名称归一化：转小写并去掉所有非字母数字字符，
    ''' 使 "Acetyl-CoA"、"acetyl coa"、"ACETYL-COA" 归一到同一形式。
    ''' </summary>
    Public Function Normalize(text As String) As String
        If String.IsNullOrEmpty(text) Then Return ""

        Dim sb As New StringBuilder(text.Length)

        For Each ch As Char In text.ToLowerInvariant()
            If Char.IsLetterOrDigit(ch) Then sb.Append(ch)
        Next

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 化合物的 id / 名称 / 同义名中是否有任一项命中归一化名称白名单。
    ''' </summary>
    Private Function MatchesName(st As CompoundStructure, byName As HashSet(Of String)) As Boolean
        If byName.Count = 0 OrElse st Is Nothing Then Return False
        If byName.Contains(Normalize(st.Id)) Then Return True
        If byName.Contains(Normalize(st.Name)) Then Return True

        If st.Synonyms IsNot Nothing Then
            For Each syn As String In st.Synonyms
                If byName.Contains(Normalize(syn)) Then Return True
            Next
        End If

        Return False
    End Function

End Module
