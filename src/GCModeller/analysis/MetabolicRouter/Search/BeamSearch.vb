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

Imports SMRUCC.genomics.Analysis.RetroPath.RetroPath.Chem

Namespace RetroPath.Search

    Public Class SearchOptions

        Public Strategy As String = "beam"        ' beam | dfs
        Public BeamWidth As Int32 = 50
        Public MaxDepth As Int32 = 6
        Public MaxPaths As Int32 = 20
        Public MatchLimit As Int32 = 50           ' 每规则每分子最大匹配数

    End Class

    Public Class RetroStep

        Public RuleId As String
        Public RuleName As String
        Public Orientation As String              ' forward / reverse（规则定义方向）
        Public SubstrateKey As String             ' 被分解化合物
        Public SubstrateMol As Molecule
        Public Precursors As New List(Of Tuple(Of String, Molecule))()   ' (key, mol)
        Public CoproductCount As Int32
        Public DeltaG As Double
        Public EnzymeTier As Int32
        Public AtomMap As New List(Of Tuple(Of Int32, Int32))()

    End Class

    Public Class SearchState

        Public Pending As New List(Of Tuple(Of String, Molecule))()
        Public Steps As New List(Of RetroStep)()
        Public Used As HashSet(Of String)

        Public Function StateKey() As String
            Return String.Join("|", Pending.Select(Function(t) t.Item1).OrderBy(Function(x) x, StringComparer.Ordinal))
        End Function

        Public Function TotalAtoms() As Int32
            Return Pending.Sum(Function(t) t.Item2.NumAtoms())
        End Function

    End Class

    Public Class SearchStats

        Public ApplicationsTried As Int64 = 0
        Public StatesGenerated As Int64 = 0
        Public RulesApplied As Int64 = 0
        Public MaxDepthReached As Int32 = 0
        Public ElapsedMs As Int64 = 0

    End Class

    Public Class BeamSearch

        Private ReadOnly _rules As List(Of Rule)
        Private ReadOnly _sinkKeys As HashSet(Of String)
        Private ReadOnly _currencyKeys As HashSet(Of String)
        Private ReadOnly _opts As SearchOptions
        Public ReadOnly Stats As New SearchStats()

        Public Sub New(rules As List(Of Rule), sinkKeys As HashSet(Of String),
                       currencyKeys As HashSet(Of String), opts As SearchOptions)
            _rules = rules
            _sinkKeys = sinkKeys
            _currencyKeys = currencyKeys
            _opts = opts
        End Sub

        ''' <summary>主入口：返回全部完整路径（深度 ≤ maxDepth）</summary>
        Public Function Search(target As Molecule) As List(Of SearchState)
            Dim tkey = target.MolKey()
            If _sinkKeys.Contains(tkey) Then Return New List(Of SearchState)()
            Dim st0 As New SearchState()
            st0.Pending.Add(Tuple.Create(tkey, target))
            st0.Used = New HashSet(Of String) From {tkey}
            Dim completed As New List(Of SearchState)()
            Dim frontier As New List(Of SearchState) From {st0}

            For depth = 1 To _opts.MaxDepth
                Stats.MaxDepthReached = depth
                Dim nextStates As New List(Of SearchState)()
                For Each st In frontier
                    For Each pend In st.Pending
                        Dim ckey = pend.Item1
                        Dim cmol = pend.Item2
                        For Each rule In _rules
                            ' 双向应用
                            Dim apps As New List(Of ApplicationResult)()
                            apps.AddRange(RuleEngine.ApplyForward(cmol, rule, _opts.MatchLimit))
                            apps.AddRange(RuleEngine.ApplyReverse(cmol, rule, _opts.MatchLimit))
                            Stats.ApplicationsTried += 2
                            If apps.Count > 0 Then Stats.RulesApplied += 1
                            For Each appRes In apps
                                ' 碎片分类：货币/自身 → 共产物；其余 → 前体
                                Dim precursors As New List(Of Tuple(Of String, Molecule))()
                                Dim coproducts As Int32 = 0
                                For Each f In appRes.Fragments
                                    Dim fk = f.MolKey()
                                    If _currencyKeys.Contains(fk) OrElse fk = ckey Then
                                        coproducts += 1
                                    Else
                                        precursors.Add(Tuple.Create(fk, f))
                                    End If
                                Next
                                If precursors.Count = 0 Then Continue For
                                ' 循环消除 [readme.md §3]
                                If precursors.Any(Function(p) st.Used.Contains(p.Item1)) Then Continue For
                                Dim newUsed As New HashSet(Of String)(st.Used)
                                Dim newPending As New List(Of Tuple(Of String, Molecule))()
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
                                    completed.Add(ns)
                                Else
                                    nextStates.Add(ns)
                                End If
                            Next
                        Next
                    Next
                Next
                ' 状态去重 + 束剪枝
                Dim pruned = Prune(nextStates)
                frontier = pruned
                If frontier.Count = 0 Then Exit For
                ' 收满路径数即停
                If completed.Count >= _opts.MaxPaths Then Exit For
            Next
            Return completed
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
