' ============================================================================
' BeamSearch.vb — 逆向路径搜索 [readme.md §3]
' ----------------------------------------------------------------------------
' 状态 = 尚未落入汇集合的化合物集合（RetroPath2.0 的"化合物空间"语义）。
' 扩展 = 取状态中一个化合物 × 规则 × 双向应用 → 新状态（其余 pending 保留 +
'   新前体中未入汇者加入）。
' 终止 = pending 为空（所有前体均落入底盘代谢物集合）[readme.md §一]。
' 循环消除 = 每条分支维护已见化合物指纹集合，前体重复 → 剪枝 [readme.md §3]。
' 束剪枝 = 每层按 (pending 数, 总原子数, ΔG) 确定性排序取前 k [readme.md §3 束搜索]。
' DFS 策略 = 深度优先单路径枚举（beam-width=1 变体）。
' ============================================================================

Imports SMRUCC.genomics.Analysis.RetroPath.Chem

Namespace Search

    ''' <summary>
    ''' 逆向路径搜索：以目标分子为根，逐层把广义规则逆向应用（亦尝试正向）展开搜索树，
    ''' 直到所有前体落入汇集合。
    ''' </summary>
    ''' <remarks>
    ''' 三个核心机制（readme.md §3）：
    ''' <list type="bullet">
    ''' <item>束剪枝：每层按 (待分解数, 原子总数, ΔG) 确定性排序，只保留前 beam-width 个状态；</item>
    ''' <item>循环消除：每条分支维护已见化合物指纹集合，前体重复即剪枝；</item>
    ''' <item>状态去重：同一 Pending 集合只保留一次。</item>
    ''' </list>
    ''' 束搜索并非完备搜索，提高 <see cref="SearchOptions.BeamWidth"/> 与
    ''' <see cref="SearchOptions.MaxDepth"/> 可提升召回。
    ''' </remarks>
    Public Class BeamSearch

        Private ReadOnly _rules As List(Of Rule)
        Private ReadOnly _sinkKeys As HashSet(Of String)
        Private ReadOnly _currencyKeys As HashSet(Of String)
        Private ReadOnly _opts As SearchOptions
        ''' <summary>已收录完整路径的内容指纹（消除对称臂等同构重复路径）</summary>
        Private ReadOnly _pathKeys As New HashSet(Of String)()

        ''' <summary>
        ''' 本次搜索的运行统计（尝试次数、生成状态数、到达深度等）。
        ''' </summary>
        Public ReadOnly Stats As New SearchStats()

        ''' <summary>
        ''' 构造搜索器。
        ''' </summary>
        ''' <param name="rules">广义反应规则集。</param>
        ''' <param name="sinkKeys">底盘内源代谢物的分子指纹集合（汇）。</param>
        ''' <param name="currencyKeys">货币/辅底物的分子指纹集合（产生即忽略，不计入前体）。</param>
        ''' <param name="opts">搜索参数（策略、束宽、深度、路径上限、匹配上限）。</param>
        Public Sub New(rules As List(Of Rule), sinkKeys As HashSet(Of String),
                       currencyKeys As HashSet(Of String), opts As SearchOptions)
            _rules = rules
            _sinkKeys = sinkKeys
            _currencyKeys = currencyKeys
            _opts = opts
        End Sub

        ''' <summary>
        ''' 主入口：从目标分子出发做逆向搜索，返回全部完整路径（深度 ≤ maxDepth）。
        ''' </summary>
        ''' <param name="target">目标分子（源）。</param>
        ''' <returns>
        ''' 完整路径列表（每条为一个 <see cref="SearchState"/>，其 Pending 为空）。
        ''' 若目标本身已属于汇集合，返回空列表——因为"目标已内源"不存在待设计的通路。
        ''' </returns>
        Public Function Search(target As Molecule) As List(Of SearchState)
            Dim tkey = target.MolKey()
            If _sinkKeys.Contains(tkey) Then Return New List(Of SearchState)()
            Dim st0 As New SearchState()
            st0.Pending.Add((tkey, target))
            st0.Used = New HashSet(Of String) From {tkey}
            Dim completed As New List(Of SearchState)()
            Dim frontier As New List(Of SearchState) From {st0}

            For depth = 1 To _opts.MaxDepth
                Stats.MaxDepthReached = depth

                ' 状态去重 + 束剪枝
                Dim pruned = Prune(Search(frontier, completed))

                If pruned.Count = 0 Then
                    Exit For
                Else
                    frontier = pruned
                End If

                ' 收满路径数即停
                If completed.Count >= _opts.MaxPaths Then
                    Exit For
                End If
            Next
            Return completed
        End Function

        Private Function Search(frontier As List(Of SearchState), completed As List(Of SearchState)) As List(Of SearchState)
            Dim nextStates As New List(Of SearchState)()

            For Each st As SearchState In frontier
                For Each pend In st.Pending
                    Dim ckey = pend.Item1
                    Dim cmol = pend.Item2
                    For Each rule As Rule In _rules
                        ' 双向应用
                        Dim apps As New List(Of ApplicationResult)()
                        apps.AddRange(RuleEngine.ApplyForward(cmol, rule, _opts.MatchLimit))
                        apps.AddRange(RuleEngine.ApplyReverse(cmol, rule, _opts.MatchLimit))
                        Stats.ApplicationsTried += 2
                        If apps.Count > 0 Then Stats.RulesApplied += 1
                        For Each appRes In apps
                            ' 碎片分类：货币/自身 → 共产物；其余 → 前体
                            Dim precursors As New List(Of (String, Molecule))()
                            Dim coproducts As Int32 = 0
                            For Each f In appRes.Fragments
                                Dim fk = f.MolKey()
                                If _currencyKeys.Contains(fk) OrElse fk = ckey Then
                                    coproducts += 1
                                Else
                                    precursors.Add((fk, f))
                                End If
                            Next
                            If precursors.Count = 0 Then Continue For
                            ' 循环消除 [readme.md §3]
                            If precursors.Any(Function(p) st.Used.Contains(p.Item1)) Then Continue For
                            Dim newUsed As New HashSet(Of String)(st.Used)
                            Dim newPending As New List(Of (String, Molecule))()
                            For Each p In precursors
                                newUsed.Add(p.Item1)
                                If Not _sinkKeys.Contains(p.Item1) Then newPending.Add(p)
                            Next
                            Dim rest = st.Pending.Where(Function(t) t.Item1 <> ckey).ToList()
                            Dim stepRec As New RetroStep With {
                                .RuleId = rule.Id, .RuleName = rule.Name,
                                .Orientation = "applied",
                                .SubstrateKey = ckey, .SubstrateMol = cmol,
                                .Precursors = precursors,
                                .CoproductCount = coproducts,
                                .DeltaG = rule.DeltaG,
                                .EnzymeTier = rule.EnzymeTier,
                                .AtomMap = appRes.AtomMap.ToList()}
                            Dim ns As New SearchState With {
                                .Pending = rest.Concat(newPending).ToList(),
                                .Steps = st.Steps.Concat({stepRec}).ToList(),
                                .Used = newUsed}
                            Stats.StatesGenerated += 1
                            If ns.Pending.Count = 0 Then
                                ' 完整路径按内容去重：对称臂/不同分解顺序产生的等价路径只保留一条
                                If _pathKeys.Add(PathKey(ns)) Then completed.Add(ns)
                            Else
                                nextStates.Add(ns)
                            End If
                        Next
                    Next
                Next
            Next

            Return nextStates
        End Function

        ''' <summary>
        ''' 路径内容指纹 = 各步（规则 + 被分解物 + 前体集合）的多重集。
        ''' 步骤字符串排序后拼接，使"分解顺序不同但反应集合相同"的等价路径合并为一条。
        ''' </summary>
        Private Shared Function PathKey(st As SearchState) As String
            Dim parts As New List(Of String)()
            For Each s In st.Steps
                Dim pre = s.Precursors.Select(Function(p) p.Item1).
                    OrderBy(Function(x) x, StringComparer.Ordinal)
                parts.Add($"{s.RuleId}|{s.SubstrateKey}|{String.Join(",", pre)}")
            Next
            parts.Sort(StringComparer.Ordinal)
            Return String.Join(";", parts)
        End Function

        Private Function Prune(states As List(Of SearchState)) As List(Of SearchState)
            Dim seen As New HashSet(Of String)()
            Dim ordered = states.OrderBy(Function(s) s.Pending.Count).
                ThenBy(Function(s) s.TotalAtoms()).
                ThenBy(Function(s) If(s.Steps.Count > 0, s.Steps(s.Steps.Count - 1).DeltaG, 0.0)).
                ThenBy(Function(s) s.StateKey(), StringComparer.Ordinal).ToList()
            Dim outList As New List(Of SearchState)()
            For Each st In ordered
                Dim sk = st.StateKey()
                If seen.Contains(sk) Then Continue For
                seen.Add(sk)
                outList.Add(st)
                If outList.Count >= _opts.BeamWidth Then Exit For
            Next
            Return outList
        End Function

    End Class

End Namespace
