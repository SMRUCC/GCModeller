' ============================================================================
' RuleFilter.vb — 规则「元素多重集」预过滤
' ----------------------------------------------------------------------------
' 展开时原本要对「每条规则 × 每个方向」都做一次子图匹配，而绝大多数规则的模式
' 元素在分子里根本不存在（例如含 P 的磷酸化规则作用在烃上），匹配必然返回空。
'
' 子图单射匹配要求：模式里参与匹配的每个原子都能在分子中找到同元素原子。
' 因此「模式有键原子的元素多重集 ⊆ 分子元素多重集」是匹配的**必要条件**，
' 不满足即可安全跳过——不会产生任何漏解。
' ----------------------------------------------------------------------------
' 注意「有键原子」这一限定：模式中不带键的孤立组分不参与匹配，它们是被创建/
' 离去的辅底物模板（见 PatternMatcher 的 bondedCls 语义），不应计入需求。
' ============================================================================

Imports SMRUCC.genomics.Analysis.RetroPath.Chem

Namespace Search

    ''' <summary>
    ''' 规则集的按方向元素需求表：用于在实际匹配前快速排除不可能命中的规则。
    ''' </summary>
    ''' <remarks>
    ''' 需求表在构造期算一次（规则数 × 2，开销可忽略），之后只读共享、线程安全。
    ''' </remarks>
    Public Class RuleFilter

        ''' <summary>第 i 条规则正向（反应物模式）的元素需求：每项为 (元素, 需要几个)。</summary>
        Private ReadOnly _forward As KeyValuePair(Of String, Int32)()()
        ''' <summary>第 i 条规则逆向（产物模式）的元素需求。</summary>
        Private ReadOnly _reverse As KeyValuePair(Of String, Int32)()()

        ''' <summary>
        ''' 为规则集构建元素需求表。
        ''' </summary>
        ''' <param name="rules">参与搜索的广义规则集。</param>
        Public Sub New(rules As List(Of Rule))
            Dim n As Int32 = rules.Count

            _forward = New KeyValuePair(Of String, Int32)(n - 1)() {}
            _reverse = New KeyValuePair(Of String, Int32)(n - 1)() {}

            For i = 0 To n - 1
                _forward(i) = ElementNeeds(rules(i).Reactant)
                _reverse(i) = ElementNeeds(rules(i).Product)
            Next
        End Sub

        ''' <summary>
        ''' 统计分子中各元素的原子数（空分子返回空表）。
        ''' </summary>
        Public Shared Function ElementCounts(m As Molecule) As Dictionary(Of String, Int32)
            Dim counts As New Dictionary(Of String, Int32)()

            For Each el As String In m.Elements
                Dim n As Int32 = 0
                counts.TryGetValue(el, n)
                counts(el) = n + 1
            Next

            Return counts
        End Function

        ''' <summary>该规则正向是否可能匹配给定分子</summary>
        Public Function CanForward(ruleIndex As Int32, counts As Dictionary(Of String, Int32)) As Boolean
            Return Satisfies(_forward(ruleIndex), counts)
        End Function

        ''' <summary>该规则逆向是否可能匹配给定分子</summary>
        Public Function CanReverse(ruleIndex As Int32, counts As Dictionary(Of String, Int32)) As Boolean
            Return Satisfies(_reverse(ruleIndex), counts)
        End Function

        Private Shared Function Satisfies(needs As KeyValuePair(Of String, Int32)(),
                                          counts As Dictionary(Of String, Int32)) As Boolean
            For Each kv In needs
                Dim have As Int32 = 0
                If Not counts.TryGetValue(kv.Key, have) Then Return False
                If have < kv.Value Then Return False
            Next
            Return True
        End Function

        ''' <summary>
        ''' 模式中「参与匹配的原子」的元素多重集——即 <c>PatternMatcher</c> 里
        ''' <c>bondedCls</c> 所对应的那些模式原子。元素通配（<c>Nothing</c>）不构成约束。
        ''' </summary>
        Private Shared Function ElementNeeds(pat As Pattern) As KeyValuePair(Of String, Int32)()
            Dim bonded As New HashSet(Of Int32)()

            For Each b In pat.Bonds
                bonded.Add(pat.Atoms(b.a).Cls)
                bonded.Add(pat.Atoms(b.b).Cls)
            Next
            If bonded.Count = 0 Then
                ' 模式里没有任何键：此时匹配器会把全部模式原子都当作参与匹配
                For Each a In pat.Atoms
                    bonded.Add(a.Cls)
                Next
            End If

            Dim counts As New Dictionary(Of String, Int32)()

            For Each a In pat.Atoms
                If Not bonded.Contains(a.Cls) Then Continue For
                If a.Element Is Nothing Then Continue For

                Dim n As Int32 = 0
                counts.TryGetValue(a.Element, n)
                counts(a.Element) = n + 1
            Next

            Dim outList As New List(Of KeyValuePair(Of String, Int32))()
            For Each kv In counts
                outList.Add(kv)
            Next
            Return outList.ToArray()
        End Function

    End Class

End Namespace
