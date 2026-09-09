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
'
' 并行展开：把「状态 × 待分解物 × 规则」摊平成有序工作单元，分块并行后按
'   (单元序号, 单元内序号) 有序归并，结果与串行版本逐位一致。
' ============================================================================

Imports System.Collections.Concurrent
Imports System.Threading
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
        Private ReadOnly _filter As RuleFilter
        ''' <summary>
        ''' 导向键集合（可选）：束剪枝时优先保留「本分支已经触达过其中某个化合物」的状态。
        ''' 用于 SynthesisRoute(A→B) 把 A 的分子指纹放进来，使通往 A 的分支不被剪掉。
        ''' 为 Nothing 或空集时，剪枝排序与历史行为完全一致。
        ''' </summary>
        Private ReadOnly _preferKeys As HashSet(Of String)
        ''' <summary>已收录完整路径的内容指纹（消除对称臂等同构重复路径）</summary>
        Private ReadOnly _pathKeys As New HashSet(Of String)()
        ''' <summary>并行区归并共享结果列表时的锁</summary>
        Private ReadOnly _mergeLock As New Object()

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
        ''' <param name="opts">搜索参数（策略、束宽、深度、路径上限、匹配上限、并行度）。</param>
        ''' <param name="preferKeys">
        ''' 可选的导向键集合：束剪枝时优先保留本分支已触达这些指纹的状态（用于「必须从 A 出发」的定向搜索）。
        ''' 传 Nothing 时与历史行为完全一致。
        ''' </param>
        Public Sub New(rules As List(Of Rule), sinkKeys As HashSet(Of String),
                       currencyKeys As HashSet(Of String), opts As SearchOptions,
                       Optional preferKeys As HashSet(Of String) = Nothing)
            _rules = rules
            _sinkKeys = sinkKeys
            _currencyKeys = currencyKeys
            _opts = opts
            _preferKeys = preferKeys
            _filter = New RuleFilter(rules)
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

        ' --------------------------------------------------------------------
        ' 展开：串行 / 确定性并行
        ' --------------------------------------------------------------------

        ''' <summary>一个待展开的「状态中的某个化合物」</summary>
        Private Class PendingItem
            Public StateIdx As Int32
            Public PendIdx As Int32
            Public State As SearchState
            Public Key As String
            Public Mol As Molecule
            Public Counts As Dictionary(Of String, Int32)
        End Class

        ''' <summary>一个展开工作单元 = (待分解物, 规则)，含预过滤后仍需尝试的方向</summary>
        Private Structure WorkUnit
            Public ItemIdx As Int32
            Public RuleIdx As Int32
            Public TryForward As Boolean
            Public TryReverse As Boolean
            ''' <summary>全局序号 = 工作单元在 (状态序, 待分解物序, 规则序) 字典序中的位置</summary>
            Public Index As Int64
        End Structure

        ''' <summary>一个展开产出，携带其在有序序列中的坐标，用于确定性归并</summary>
        Private Class Expansion
            Public UnitIndex As Int64
            Public Seq As Int32
            Public Produced As SearchState
            Public IsCompleted As Boolean
        End Class

        ''' <summary>并行分区的本地累加器（统计 + 结果缓冲，避免共享集合与锁竞争）</summary>
        Private Class LocalAcc
            Public Apps As Int64
            Public RulesApplied As Int64
            Public StatesGenerated As Int64
            Public Results As New List(Of Expansion)()
        End Class

        Private Function Search(frontier As List(Of SearchState), completed As List(Of SearchState)) As List(Of SearchState)
            ' ---------- 1) 摊平为待分解物列表 ----------
            Dim items As New List(Of PendingItem)()

            For si = 0 To frontier.Count - 1
                Dim st As SearchState = frontier(si)

                For pi = 0 To st.Pending.Count - 1
                    Dim pend = st.Pending(pi)
                    items.Add(New PendingItem With {
                        .StateIdx = si,
                        .PendIdx = pi,
                        .State = st,
                        .Key = pend.Item1,
                        .Mol = pend.Item2,
                        .Counts = RuleFilter.ElementCounts(pend.Item2)})
                Next
            Next

            If items.Count = 0 Then Return New List(Of SearchState)()

            ' ---------- 2) 摊平为工作单元（顺带做元素预过滤） ----------
            Dim units As New List(Of WorkUnit)()
            Dim unitIndex As Int64 = 0

            For ii = 0 To items.Count - 1
                Dim counts = items(ii).Counts

                For ri = 0 To _rules.Count - 1
                    Dim fwd As Boolean = _filter.CanForward(ri, counts)
                    Dim rev As Boolean = _filter.CanReverse(ri, counts)

                    If Not fwd AndAlso Not rev Then
                        Stats.RulesPrefiltered += 1
                        Continue For
                    End If

                    Dim u As New WorkUnit With {
                        .ItemIdx = ii,
                        .RuleIdx = ri,
                        .TryForward = fwd,
                        .TryReverse = rev,
                        .Index = unitIndex}

                    unitIndex += 1
                    units.Add(u)
                Next
            Next

            ' ---------- 3) 展开（并行 or 串行，产出同一套有序结果） ----------
            Dim expanded As New List(Of Expansion)()
            Dim parallelism As Int32 = _opts.EffectiveParallelism()

            If units.Count >= SearchOptions.MinParallelUnits AndAlso parallelism > 1 Then
                Dim opts As New ParallelOptions With {.MaxDegreeOfParallelism = parallelism}

                Parallel.ForEach(Of Tuple(Of Int32, Int32), LocalAcc)(
                    Partitioner.Create(0, units.Count),
                    opts,
                    Function() New LocalAcc(),
                    Function(range, loopState, acc)
                        For ui = range.Item1 To range.Item2 - 1
                            ExpandUnit(units(ui), items, acc)
                        Next
                        Return acc
                    End Function,
                    Sub(acc)
                        Interlocked.Add(Stats.ApplicationsTried, acc.Apps)
                        Interlocked.Add(Stats.RulesApplied, acc.RulesApplied)
                        Interlocked.Add(Stats.StatesGenerated, acc.StatesGenerated)
                        SyncLock _mergeLock
                            expanded.AddRange(acc.Results)
                        End SyncLock
                    End Sub)
            Else
                Dim acc As New LocalAcc()
                For Each u In units
                    ExpandUnit(u, items, acc)
                Next
                Stats.ApplicationsTried += acc.Apps
                Stats.RulesApplied += acc.RulesApplied
                Stats.StatesGenerated += acc.StatesGenerated
                expanded.AddRange(acc.Results)
            End If

            ' ---------- 4) 按 (单元序号, 单元内序号) 有序归并 ----------
            ' 该序对唯一确定每个产出的位置，因此不受并行完成先后与排序稳定性的影响，
            ' 去重与剪枝的输入序列与串行版本完全一致。
            expanded.Sort(Function(a, b)
                              Dim c As Int32 = a.UnitIndex.CompareTo(b.UnitIndex)
                              If c <> 0 Then Return c
                              Return a.Seq.CompareTo(b.Seq)
                          End Function)

            Dim nextStates As New List(Of SearchState)()

            For Each e In expanded
                If e.IsCompleted Then
                    ' 完整路径按内容去重：对称臂/不同分解顺序产生的等价路径只保留一条
                    If _pathKeys.Add(PathKey(e.Produced)) Then completed.Add(e.Produced)
                Else
                    nextStates.Add(e.Produced)
                End If
            Next

            Return nextStates
        End Function

        ''' <summary>
        ''' 展开单个工作单元：把一个规则双向应用到某个待分解化合物上，产出新状态。
        ''' 单元内部的顺序（先正向后逆向、碎片按原序）与串行版本严格一致。
        ''' </summary>
        Private Sub ExpandUnit(u As WorkUnit, items As List(Of PendingItem), acc As LocalAcc)
            Dim item As PendingItem = items(u.ItemIdx)
            Dim st As SearchState = item.State
            Dim ckey As String = item.Key
            Dim cmol As Molecule = item.Mol
            Dim rule As Rule = _rules(u.RuleIdx)
            Dim seq As Int32 = 0

            Dim apps As New List(Of ApplicationResult)()
            If u.TryForward Then apps.AddRange(RuleEngine.ApplyForward(cmol, rule, _opts.MatchLimit))
            If u.TryReverse Then apps.AddRange(RuleEngine.ApplyReverse(cmol, rule, _opts.MatchLimit))

            acc.Apps += If(u.TryForward, 1, 0) + If(u.TryReverse, 1, 0)
            If apps.Count > 0 Then acc.RulesApplied += 1

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
                acc.StatesGenerated += 1

                acc.Results.Add(New Expansion With {
                    .UnitIndex = u.Index,
                    .Seq = seq,
                    .Produced = ns,
                    .IsCompleted = (ns.Pending.Count = 0)})
                seq += 1
            Next
        End Sub

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
            Dim ordered = states.
                OrderBy(Function(s) Preference(s)).
                ThenBy(Function(s) s.Pending.Count).
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

        ''' <summary>
        ''' 导向优先级：0 = 本分支已触达导向键（如起点化合物 A），1 = 尚未触达。
        ''' 未配置导向键时恒返回 0，即退化为原有的纯 (pending, atoms, ΔG) 排序。
        ''' </summary>
        Private Function Preference(st As SearchState) As Integer
            If _preferKeys Is Nothing OrElse _preferKeys.Count = 0 Then Return 0
            If st.Used Is Nothing Then Return 1

            For Each k As String In _preferKeys
                If st.Used.Contains(k) Then Return 0
            Next

            Return 1
        End Function

    End Class

End Namespace
