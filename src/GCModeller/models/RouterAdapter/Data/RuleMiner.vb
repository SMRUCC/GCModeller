' ============================================================================
' RuleMiner.vb — 与数据源无关的通用规则挖掘引擎：反应契约 → RetroPath 广义反应规则
' ----------------------------------------------------------------------------
' 输入是中立的 ReactionSpec + CompoundStructure 结构索引，因此 BioCyc（PGDB）与
' GCModeller 内部代谢模型（MetabolicCompound / MetabolicReaction）可以共用同一套
' 挖掘逻辑，两端挖出的规则语义完全一致。各数据源只需提供一层"字段映射"：
'   BioCyc      → BioCyc/BioCycRuleMiner.vb（含 REACTION-DIRECTION 方向归正）
'   内部模型    → Metabolic/MetabolicRuleMiner.vb
'
' [readme_alg.md §2] 规则由酶催化的化学逻辑而非具体反应实例抽象而来：
'   §2.1 原子映射（MCS，见 AtomMapping.vb）
'   §2.2 反应中心识别（未映射原子 = 断键/成键/键级改变的位点）
'   §2.3 规则泛化与编码（反应中心 + 1 层邻居 → 带类号的 SMARTS/SMIRKS）
'
' 类号（:n）是 RetroPath 中原子映射的"货币"：两侧同号 = 存活原子（元素/电荷按
' 目标侧覆盖）；仅目标侧有键的类 = 新生成原子（离去/加入的辅底物）；仅匹配侧有键
' 的类 = 断键。因此生成模式时必须保证：
'   1) 同一个类号在两侧（若都存在）都必须至少有一条键，否则该原子既不参与匹配
'      也不会被创建（会凭空消失）；
'   2) 两侧都存在的键，要么在两侧模式中都出现（环境键，键级一致），要么都不出现
'      （视为环境键保留）——绝不能只在一侧出现，否则会被误判为成键/断键。
' 为此统一在"类图"上做一次生成森林（优先保留发生变化的键），再按侧裁剪发射。
'
' 化学合理性由 RuleEngine 的价态闸门兜底；本模块只负责"泛化"，不加 Hn/Dn 约束，
' 以保留酶的底物混杂性（README §2.3）。
' ============================================================================

Imports System.Text
Imports SMRUCC.genomics.Analysis.RetroPath.Chem
Imports SMRUCC.genomics.Analysis.RetroPath.Search
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes

Public Module RuleMiner

    ''' <summary>
    ''' 逐条反应挖掘广义反应规则。
    ''' </summary>
    ''' <param name="reactionList">已映射为 <see cref="ReactionSpec"/> 的反应集合（建议按 id 排序以保证确定性）</param>
    ''' <param name="structures">compound frame id → 已净化结构（缺失即无法解析的化合物）</param>
    ''' <param name="maxMoleculeAtoms">参与反应的分子重原子数上限（超出的反应跳过，控耗时）</param>
    ''' <param name="maxPatternAtoms">模式原子数上限（超出的规则过特异，跳过）</param>
    ''' <param name="mcsNodeBudget">MCS 回溯搜索的节点预算</param>
    ''' <param name="includeBuiltin">是否叠加 RetroPath 内置的 9 条广义规则</param>
    ''' <param name="skipped">跳过原因计数（可为 Nothing）</param>
    Public Function Mine(reactionList As IEnumerable(Of ReactionSpec),
                         structures As Dictionary(Of String, CompoundStructure),
                         Optional maxMoleculeAtoms As Integer = 80,
                         Optional maxPatternAtoms As Integer = 32,
                         Optional mcsNodeBudget As Integer = 60000,
                         Optional maxUnmappedAtoms As Integer = 3,
                         Optional shellRadius As Integer = 2,
                         Optional includeBuiltin As Boolean = False,
                         Optional strictSelfCheck As Boolean = False,
                         Optional skipped As Dictionary(Of String, Integer) = Nothing,
                         Optional trace As Dictionary(Of String, String) = Nothing) As List(Of Rule)

        Dim rules As New List(Of Rule)()
        Dim seenRule As New Dictionary(Of String, Integer)()   ' 模式文本 → rules 下标

        If includeBuiltin Then
            For Each r In RuleLibrary.BuiltinRules()
                rules.Add(r)
                seenRule(r.ReactantText & ">>" & r.ProductText) = rules.Count - 1
            Next
        End If

        For Each spec As ReactionSpec In reactionList
            If spec Is Nothing Then
                CountSkip(skipped, "null-reaction")
                Continue For
            End If

            Dim rule As Rule = MineOne(spec, structures, maxMoleculeAtoms,
                                       maxPatternAtoms, mcsNodeBudget, maxUnmappedAtoms, shellRadius,
                                       strictSelfCheck, skipped, trace)
            If rule Is Nothing Then Continue For

            Dim key As String = rule.ReactantText & ">>" & rule.ProductText

            If seenRule.ContainsKey(key) Then
                ' 同一变换的多条反应：保留酶可得性更好（tier 更小）的那条
                Dim old As Rule = rules(seenRule(key))
                If rule.EnzymeTier < old.EnzymeTier Then
                    rules(seenRule(key)) = rule
                Else
                    Bail(skipped, trace, rule.Id, "duplicate-rule")
                End If
            Else
                seenRule(key) = rules.Count
                rules.Add(rule)
            End If
        Next

        Return rules
    End Function

    ''' <summary>从单条反应抽取一条广义规则；失败（跳过）返回 Nothing</summary>
    Private Function MineOne(spec As ReactionSpec,
                             structures As Dictionary(Of String, CompoundStructure),
                             maxMoleculeAtoms As Integer,
                             maxPatternAtoms As Integer,
                             mcsNodeBudget As Integer,
                             maxUnmappedAtoms As Integer,
                             shellRadius As Integer,
                             strictSelfCheck As Boolean,
                             skipped As Dictionary(Of String, Integer),
                             Optional trace As Dictionary(Of String, String) = Nothing) As Rule

        Dim rxnId As String = If(spec Is Nothing, Nothing, spec.Id)

        ' ---- 1) 两侧底物 id（方向归正由各数据源的映射层负责）
        Dim rIds As List(Of String) = spec.ReactantIds
        Dim pIds As List(Of String) = spec.ProductIds

        If rIds Is Nothing OrElse pIds Is Nothing OrElse rIds.Count = 0 OrElse pIds.Count = 0 Then
            Bail(skipped, trace, rxnId, "empty-side")
            Return Nothing
        End If

        ' ---- 2) 结构解析（无 SMILES / 含金属 / 超限 → 跳过）
        Dim rSmiles As String = Nothing
        Dim pSmiles As String = Nothing

        If Not JoinSmiles(rIds, structures, rSmiles) OrElse Not JoinSmiles(pIds, structures, pSmiles) Then
            Bail(skipped, trace, rxnId, "missing-structure")
            Return Nothing
        End If

        Dim rMol As Molecule = Nothing
        Dim pMol As Molecule = Nothing

        Try
            rMol = SmilesIO.Parse(rSmiles)
            pMol = SmilesIO.Parse(pSmiles)
        Catch ex As Exception
            Bail(skipped, trace, rxnId, "parse-error")
            Return Nothing
        End Try

        If rMol.NumAtoms() > maxMoleculeAtoms OrElse pMol.NumAtoms() > maxMoleculeAtoms Then
            Bail(skipped, trace, rxnId, "too-large")
            Return Nothing
        End If

        ' ---- 3) 原子映射 [§2.1]
        Dim map As MappingResult = AtomMapping.Map(rMol, pMol, mcsNodeBudget, maxUnmappedAtoms)

        ' ---- 4) 反应中心 [§2.2]
        ' 种子由两部分组成：
        '   (i)  未映射原子——在本侧出现、在另一侧没有对应物的基团（加入/离去基团）；
        '   (ii) 已映射但键发生变化的原子——键级改变、断键、成键。
        ' 之所以要用 (ii)：MCS 允许少量键变化以保持原子对应（否则反应中心处的原子会被
        ' 挤出映射，规则就退化成"凭空造原子/删原子"），此时"差异"体现在键上而不是原子上。
        Dim centerR As New SortedSet(Of Integer)()
        Dim centerP As New SortedSet(Of Integer)()

        For Each pr In map.Pairs
            For Each nb In rMol.Neighbors(pr.r)
                Dim p2 As Integer = map.ReactantToProduct(nb.Item1)
                If p2 < 0 Then Continue For
                If pMol.BondOrder(pr.p, p2) <> nb.Item2 Then
                    centerR.Add(pr.r) : centerP.Add(pr.p)
                    centerR.Add(nb.Item1) : centerP.Add(p2)
                End If
            Next
            For Each nb In pMol.Neighbors(pr.p)
                Dim r2 As Integer = map.ProductToReactant(nb.Item1)
                If r2 < 0 Then Continue For
                If rMol.BondOrder(pr.r, r2) <> nb.Item2 Then
                    centerR.Add(pr.r) : centerP.Add(pr.p)
                    centerR.Add(r2) : centerP.Add(nb.Item1)
                End If
            Next
        Next

        If centerR.Count = 0 AndAlso centerP.Count = 0 AndAlso
           map.ReactantOnly.Count = 0 AndAlso map.ProductOnly.Count = 0 Then
            Bail(skipped, trace, rxnId, "no-reaction-center")
            Return Nothing
        End If

        ' (a) 以反应中心为种子，按 shellRadius 层向外扩展作为泛化环境。
        '     半径过小（1 层）会抽出 "[C]-[O-]" 这种无意义的局部模式，实测会匹配到任何
        '     羟基/羧基上并生成伪通路；半径越大规则越特异。
        Dim seedR As New List(Of Integer)(centerR)
        Dim seedP As New List(Of Integer)(centerP)
        seedR.AddRange(map.ReactantOnly)
        seedP.AddRange(map.ProductOnly)

        ExpandShell(rMol, centerR, seedR, shellRadius)
        ExpandShell(pMol, centerP, seedP, shellRadius)

        ' (b)/(c) 不动点迭代：映射闭包 + 连通性修补
        Dim changed As Boolean = True
        Dim oversized As Boolean = False
        Dim rounds As Integer = 0

        While changed
            changed = False
            rounds += 1

            ' 映射闭包：已映射对的另一侧必须同时进入模式，否则该类号只在一侧出现
            For Each pr In map.Pairs
                If centerR.Contains(pr.r) AndAlso centerP.Add(pr.p) Then changed = True
                If centerP.Contains(pr.p) AndAlso centerR.Add(pr.r) Then changed = True
            Next

            ' 连通性修补：模式原子若在本侧没有任何键，就既不参与匹配也不会被创建，
            ' 必须补进来一个邻居（典型如 R-OH → R-O-PO3(2-)：只有一侧加入基团时，
            ' 反应物侧的中心原子最初是孤立的）。
            If RepairIsolated(rMol, centerR) Then changed = True
            If RepairIsolated(pMol, centerP) Then changed = True

            If centerR.Count > maxPatternAtoms OrElse centerP.Count > maxPatternAtoms Then
                oversized = True
                Exit While
            End If
            If rounds > 8 Then
                oversized = True
                Exit While
            End If
        End While

        If oversized Then
            Bail(skipped, trace, rxnId, "center-too-large")
            Return Nothing
        End If

        ' (d) 每侧只保留最大的连通分量。
        '     逆向搜索是把规则的"产物侧"模式匹配到目标分子上的，而目标永远是一个
        '     单独的分子：若产物侧模式里还带了 Pi / 丙酮酸 之类的共产物组分，规则就
        '     永远无法命中（实测分支酸合酶 EPSP → 分支酸 + Pi 就是因此失效）。
        '     被丢弃的共产物在模式里消失，等价于"作为货币分子忽略"；而它们在另一侧
        '     若存在（如底物上挂着的磷酸基）会由 RuleEngine 自动创建出来。
        KeepLargestComponent(rMol, centerR)
        KeepLargestComponent(pMol, centerP)

        ' 注：此时仍可能存在"孤立"的模式原子——典型是水/质子/氨这类单原子共底物
        ' （它们没有任何邻居可补）。这类类号在模式中没有键，因此不参与匹配、也不会被
        ' 创建，等价于"货币分子被忽略"的语义，属预期行为，不再视为失败。

        ' ---- 5) 类号分配：映射对同号，未映射原子各自新号
        Dim clsR As New Dictionary(Of Integer, Integer)()
        Dim clsP As New Dictionary(Of Integer, Integer)()
        Dim atomOfClsR As New Dictionary(Of Integer, Integer)()
        Dim atomOfClsP As New Dictionary(Of Integer, Integer)()
        Dim nextCls As Integer = 1

        For Each pr In map.Pairs.OrderBy(Function(x) x.r).ThenBy(Function(x) x.p)
            ' 只给"两侧都保留下来"的原子分配共用类号；只在一侧的类号会由 RuleEngine
            ' 视为新生成/离去的基团自动创建出来（等价于货币分子语义）。
            If Not centerR.Contains(pr.r) Then Continue For
            If Not centerP.Contains(pr.p) Then Continue For

            Dim c As Integer = nextCls
            nextCls += 1

            clsR(pr.r) = c
            clsP(pr.p) = c
            atomOfClsR(c) = pr.r
            atomOfClsP(c) = pr.p
        Next

        For Each r As Integer In centerR
            If clsR.ContainsKey(r) Then Continue For
            Dim c As Integer = nextCls
            nextCls += 1
            clsR(r) = c
            atomOfClsR(c) = r
        Next
        For Each p As Integer In centerP
            If clsP.ContainsKey(p) Then Continue For
            Dim c As Integer = nextCls
            nextCls += 1
            clsP(p) = c
            atomOfClsP(c) = p
        Next

        ' ---- 6) 类图上的边：两侧键级（不存在记 0），OrderR <> OrderP 即发生变化
        Dim edgeMap As New Dictionary(Of String, PatternEdge)()

        For Each bd In rMol.Bonds
            Dim ca As Integer = 0, cb As Integer = 0
            If clsR.TryGetValue(bd.a, ca) AndAlso clsR.TryGetValue(bd.b, cb) Then
                AddEdge(edgeMap, ca, cb, bd.order, True)
            End If
        Next
        For Each bd In pMol.Bonds
            Dim ca As Integer = 0, cb As Integer = 0
            If clsP.TryGetValue(bd.a, ca) AndAlso clsP.TryGetValue(bd.b, cb) Then
                AddEdge(edgeMap, ca, cb, bd.order, False)
            End If
        Next

        If edgeMap.Count = 0 Then
            Bail(skipped, trace, rxnId, "no-pattern-bond")
            Return Nothing
        End If

        ' ---- 7) 选择进入模式的边集 E*
        ' 边分两类：
        '   双侧键（两侧都存在，含"键级改变"）——必须同时在两侧模式中出现，否则键级
        '     变化无法表达，或环境键被误判为成键/断键；
        '   单侧键（只在一侧存在）——就是成键/断键本身，天然只属于一侧。
        ' 做法：先在双侧键上做 Kruskal（键级改变的边即使成环也保留），再让每一侧用
        '   各自的单侧键把连通性补齐。这样"每侧一个连通分量"能保证——否则产物侧模式
        '   会碎成多块，逆向搜索时要求目标分子同时含有这几块，规则就永远命中不了
        '   （分支酸合酶 EPSP → 分支酸 + Pi 正是栽在这里）。
        Dim bothSides As New List(Of PatternEdge)()
        Dim rOnly As New List(Of PatternEdge)()
        Dim pOnly As New List(Of PatternEdge)()
        Dim allEdges As New List(Of PatternEdge)(edgeMap.Values)

        For Each e As PatternEdge In allEdges
            If e.OrderR > 0 AndAlso e.OrderP > 0 Then
                bothSides.Add(e)
            ElseIf e.OrderR > 0 Then
                rOnly.Add(e)
            Else
                pOnly.Add(e)
            End If
        Next

        Dim parent As New Dictionary(Of Integer, Integer)()
        For Each c As Integer In clsR.Values
            parent(c) = c
        Next
        For Each c As Integer In clsP.Values
            If Not parent.ContainsKey(c) Then parent(c) = c
        Next

        Dim eStar As New HashSet(Of String)()

        bothSides.Sort()
        For Each e As PatternEdge In bothSides
            Dim ra As Integer = Find(parent, e.A)
            Dim rb As Integer = Find(parent, e.B)
            If ra = rb Then
                ' 键级改变的边必须保留（成环也无妨：发射时跳过已访问原子）
                If e.Changed Then eStar.Add(EdgeKey(e))
            Else
                parent(ra) = rb
                eStar.Add(EdgeKey(e))
            End If
        Next

        ' 两侧各自独立补边（互不影响对方的连通性判断）
        CompleteSide(rOnly, New Dictionary(Of Integer, Integer)(parent), eStar)
        CompleteSide(pOnly, New Dictionary(Of Integer, Integer)(parent), eStar)

        Dim forest As New List(Of PatternEdge)()
        For Each e As PatternEdge In allEdges
            If eStar.Contains(EdgeKey(e)) Then forest.Add(e)
        Next

        ' ---- 8) 发射两侧模式
        ' SMARTS 子集不支持环闭合写法，模式只能是一棵树；而中心子图常常带环，于是
        ' "哪些键被写进模式"必须两侧一致：若某条键两侧分子里都存在且键级相同，却在
        ' 一侧模式里出现、另一侧被省略，RuleEngine 会把它当成成键/断键处理，规则就
        ' 无法重现它自己的反应（自检发现 78% 的规则因此失效）。
        ' 做法：两侧各自做 DFS 生成树，比对后把"只在一侧出现"的共有键加入 forbidden
        ' 重新生成，直到两侧一致（最多迭代 4 轮）。
        Dim forbidden As New HashSet(Of String)()
        Dim sharedEqual As New HashSet(Of String)()
        For Each e As PatternEdge In allEdges
            If e.OrderR > 0 AndAlso e.OrderR = e.OrderP Then sharedEqual.Add(EdgeKey(e))
        Next

        Dim rText As String = Nothing
        Dim pText As String = Nothing
        Dim er As New HashSet(Of String)()
        Dim ep As New HashSet(Of String)()

        For iter As Integer = 1 To 4
            er = New HashSet(Of String)()
            ep = New HashSet(Of String)()
            rText = EmitSide(clsR.Values.ToList(), forest, forbidden, True, rMol, atomOfClsR, er)
            pText = EmitSide(clsP.Values.ToList(), forest, forbidden, False, pMol, atomOfClsP, ep)

            Dim bad As New List(Of String)()
            For Each k As String In er
                If Not ep.Contains(k) AndAlso sharedEqual.Contains(k) Then bad.Add(k)
            Next
            For Each k As String In ep
                If Not er.Contains(k) AndAlso sharedEqual.Contains(k) Then bad.Add(k)
            Next

            If bad.Count = 0 Then Exit For
            For Each k As String In bad
                forbidden.Add(k)
            Next
        Next

        If rText Is Nothing OrElse pText Is Nothing Then
            Bail(skipped, trace, rxnId, "emit-isolated")
            Return Nothing
        End If
        If clsR.Count > maxPatternAtoms OrElse clsP.Count > maxPatternAtoms Then
            Bail(skipped, trace, rxnId, "pattern-too-large")
            Return Nothing
        End If

        ' ---- 9) 规则评分参数：ΔG 取反应的 Gibbs 自由能，酶层级按 EC 号判定
        Dim dg As Double = spec.Gibbs
        If Double.IsNaN(dg) OrElse Double.IsInfinity(dg) Then dg = 0

        Dim tier As EnzymeTiers
        If spec.ECNumbers IsNot Nothing AndAlso spec.ECNumbers.Length > 0 Then
            tier = EnzymeTiers.Common
        ElseIf spec.IsSpontaneous Then
            tier = EnzymeTiers.General
        Else
            tier = EnzymeTiers.Specialized
        End If

        ' 名称由各数据源映射层按"常用名 → 系统名 → id"回退填好，这里只做最后的兜底
        Dim name As String = spec.Name
        If String.IsNullOrEmpty(name) Then name = rxnId

        Dim reversible As Boolean = spec.IsReversible

        Dim rule As Rule = Nothing

        Try
            rule = New Rule(rxnId, name, rText, pText, dg, tier, reversible)
        Catch ex As Exception
            Bail(skipped, trace, rxnId, "pattern-error")
            Return Nothing
        End Try

        ' ---- 10) 自检：规则必须能匹配它自己的底物/产物
        Try
            If PatternMatcher.Match(pMol, rule.Product, 1).Count = 0 OrElse
               PatternMatcher.Match(rMol, rule.Reactant, 1).Count = 0 Then
                Bail(skipped, trace, rxnId, "no-self-match")
                Return Nothing
            End If
        Catch ex As Exception
            Bail(skipped, trace, rxnId, "self-match-error")
            Return Nothing
        End Try

        ' ---- 11) 自检（关键）：把规则正向施加到它自己的底物上，必须真的得到它自己的产物。
        '     这一步能滤掉"退化规则"——两侧模式拓扑相同、只是把一段结构原样替换成另一段
        '     相同结构的伪规则（如把共轭二烯换成共轭二烯）。这类规则能匹配大量无关分子，
        '     却是零信息量的空转步骤，实测会污染搜索结果的 Top 路径。
        Dim mainProduct As Molecule = LargestComponent(pMol)
        Dim mainReactant As Molecule = LargestComponent(rMol)

        If mainProduct IsNot Nothing AndAlso mainReactant IsNot Nothing Then
            Dim pKey As String = mainProduct.MolKey()
            Dim rKey As String = mainReactant.MolKey()
            Dim ok As Boolean = False

            Try
                For Each a As ApplicationResult In RuleEngine.ApplyForward(rMol, rule, 20)
                    For Each f As Molecule In a.Fragments
                        If f.NumAtoms() = mainProduct.NumAtoms() AndAlso f.MolKey() = pKey Then
                            ok = True
                            Exit For
                        End If
                    Next
                    If ok Then Exit For
                Next
                If Not ok Then
                    For Each a As ApplicationResult In RuleEngine.ApplyReverse(pMol, rule, 20)
                        For Each f As Molecule In a.Fragments
                            If f.NumAtoms() = mainReactant.NumAtoms() AndAlso f.MolKey() = rKey Then
                                ok = True
                                Exit For
                            End If
                        Next
                        If ok Then Exit For
                    Next
                End If
            Catch ex As Exception
                ok = False
            End Try

            If Not ok Then
                ' 严格模式下直接丢弃；否则只记数（rejectedSelfCheck 由调用方汇总），
                ' 规则仍保留——这是"召回 vs 化学严格性"的取舍开关。
                If strictSelfCheck Then
                    Bail(skipped, trace, rxnId, "self-apply-failed")
                    Return Nothing
                End If
                CountSkip(skipped, "self-apply-failed(kept)")
            End If
        End If

        Return rule
    End Function

    ''' <summary>取分子中最大的连通分量（多组分体系中即"主底物/主产物"）</summary>
    Private Function LargestComponent(mol As Molecule) As Molecule
        Dim best As Molecule = Nothing

        For Each f As Molecule In mol.SplitComponents()
            If best Is Nothing OrElse f.NumAtoms() > best.NumAtoms() Then best = f
        Next

        Return best
    End Function

    ''' <summary>用该侧独有的键（成键/断键）把模式的连通性补齐</summary>
    Private Sub CompleteSide(edges As List(Of PatternEdge),
                             local As Dictionary(Of Integer, Integer),
                             eStar As HashSet(Of String))
        edges.Sort()

        For Each e As PatternEdge In edges
            Dim ra As Integer = Find(local, e.A)
            Dim rb As Integer = Find(local, e.B)
            If ra = rb Then Continue For
            local(ra) = rb
            eStar.Add(EdgeKey(e))
        Next
    End Sub

    Private Function EdgeKey(e As PatternEdge) As String
        Return e.A & "_" & e.B
    End Function

    ''' <summary>只保留中心集合里最大的那个连通分量（丢弃游离的共底物/共产物组分）</summary>
    Private Sub KeepLargestComponent(mol As Molecule, center As SortedSet(Of Integer))
        If center.Count <= 1 Then Return

        Dim seen As New HashSet(Of Integer)()
        Dim best As New List(Of Integer)()

        For Each a As Integer In center
            If seen.Contains(a) Then Continue For

            Dim comp As New List(Of Integer)()
            Dim stack As New Stack(Of Integer)()

            stack.Push(a)
            seen.Add(a)

            While stack.Count > 0
                Dim x As Integer = stack.Pop()
                comp.Add(x)

                For Each nb In mol.Neighbors(x)
                    If center.Contains(nb.Item1) AndAlso seen.Add(nb.Item1) Then
                        stack.Push(nb.Item1)
                    End If
                Next
            End While

            ' 同规模时取原子序最小者，保证结果确定
            If comp.Count > best.Count Then best = comp
        Next

        center.Clear()
        For Each a As Integer In best
            center.Add(a)
        Next
    End Sub

    ''' <summary>从种子原子出发按指定半径向外扩展，作为规则的成键环境（泛化上下文）</summary>
    Private Sub ExpandShell(mol As Molecule, center As SortedSet(Of Integer),
                            seeds As List(Of Integer), radius As Integer)
        Dim frontier As New List(Of Integer)()

        For Each s As Integer In seeds
            If center.Add(s) Then frontier.Add(s)
        Next

        For d As Integer = 1 To Math.Max(0, radius)
            Dim nextFrontier As New List(Of Integer)()

            For Each a As Integer In frontier
                For Each nb In mol.Neighbors(a)
                    If center.Add(nb.Item1) Then nextFrontier.Add(nb.Item1)
                Next
            Next

            frontier = nextFrontier
            If frontier.Count = 0 Then Exit For
        Next
    End Sub

    ''' <summary>该中心原子在本侧是否与另一个中心原子成键</summary>
    Private Function HasCenterNeighbor(mol As Molecule, center As SortedSet(Of Integer), a As Integer) As Boolean
        For Each nb In mol.Neighbors(a)
            If center.Contains(nb.Item1) Then Return True
        Next
        Return False
    End Function

    ''' <summary>
    ''' 为本侧孤立的中心原子补进一个邻居，使模式中的每个原子都至少有一条键。
    ''' 返回是否发生了修改。
    ''' </summary>
    Private Function RepairIsolated(mol As Molecule, center As SortedSet(Of Integer)) As Boolean
        Dim changed As Boolean = False

        For Each a As Integer In center.ToList()
            If HasCenterNeighbor(mol, center, a) Then Continue For

            For Each nb In mol.Neighbors(a)
                If center.Contains(nb.Item1) Then Continue For
                center.Add(nb.Item1)
                changed = True
                Exit For
            Next
        Next

        Return changed
    End Function

    ''' <summary>反应某一侧的化合物 id（去重、去空、保持字典序）</summary>
    ''' <summary>
    ''' 从反应某一侧的化合物引用中取出 id（去重、去空、保持字典序）。
    ''' </summary>
    ''' <param name="side">化合物引用序列；BioCyc 与 GCModeller 内部代谢模型共用
    ''' <see cref="CompoundSpecieReference"/> 这一类型，因此可直接复用。</param>
    ''' <returns>排序后的 id 列表。</returns>
    Public Function CompoundIds(side As IEnumerable(Of CompoundSpecieReference)) As List(Of String)
        Dim raw As New List(Of String)()

        If side IsNot Nothing Then
            For Each ref As CompoundSpecieReference In side
                raw.Add(If(ref Is Nothing, Nothing, ref.ID))
            Next
        End If

        Return CompoundIds(raw)
    End Function

    ''' <summary>
    ''' 从化合物 id 序列中整理出规范 id 列表（去重、去空、保持字典序）。
    ''' </summary>
    ''' <param name="ids">原始 id 序列，允许含空值或 "A,B" 形式的并列写法。</param>
    ''' <returns>排序后的 id 列表。</returns>
    Public Function CompoundIds(ids As IEnumerable(Of String)) As List(Of String)
        Dim result As New SortedSet(Of String)(StringComparer.Ordinal)

        If ids IsNot Nothing Then
            For Each raw As String In ids
                If String.IsNullOrEmpty(raw) Then Continue For
                ' 数据源偶见 "CPD-1,CPD-2" 形式的并列写法，取第一项
                result.Add(raw.Split(","c)(0).Trim())
            Next
        End If

        Return result.ToList()
    End Function

    Private Function JoinSmiles(ids As List(Of String),
                                structures As Dictionary(Of String, CompoundStructure),
                                ByRef out As String) As Boolean
        out = Nothing
        Dim parts As New List(Of String)()

        For Each id As String In ids
            Dim st As CompoundStructure = Nothing
            If Not structures.TryGetValue(id, st) OrElse st Is Nothing Then Return False
            parts.Add(st.Smiles)
        Next

        If parts.Count = 0 Then Return False

        ' 化学计量数（如 3 × DHB-Ser）在当前规则模型里无法表达，每种参与者只取一份
        out = String.Join(".", parts)
        Return True
    End Function

    Private Sub AddEdge(map As Dictionary(Of String, PatternEdge),
                        ca As Integer, cb As Integer, order As Integer, onReactant As Boolean)
        Dim lo As Integer = Math.Min(ca, cb)
        Dim hi As Integer = Math.Max(ca, cb)
        Dim key As String = lo & "_" & hi
        Dim e As PatternEdge = Nothing

        If Not map.TryGetValue(key, e) Then
            e = New PatternEdge With {.A = lo, .B = hi}
            map(key) = e
        End If

        If onReactant Then
            e.OrderR = order
        Else
            e.OrderP = order
        End If
    End Sub

    ''' <summary>
    ''' 发射一侧的模式串：按生成森林做 DFS，支链用括号包裹，多组分用 "." 分隔。
    ''' 返回 Nothing 表示存在孤立的类号（不参与匹配也不会被创建，属非法模式）。
    ''' </summary>
    Private Function EmitSide(classes As List(Of Integer),
                              eStar As List(Of PatternEdge),
                              forbidden As HashSet(Of String),
                              onReactant As Boolean,
                              mol As Molecule,
                              atomOfCls As Dictionary(Of Integer, Integer),
                              ByRef emitted As HashSet(Of String)) As String

        Dim adj As New Dictionary(Of Integer, List(Of (nb As Integer, order As Integer)))()

        For Each e As PatternEdge In eStar
            Dim order As Integer = If(onReactant, e.OrderR, e.OrderP)
            If order <= 0 Then Continue For
            If forbidden.Contains(EdgeKey(e)) Then Continue For

            If Not adj.ContainsKey(e.A) Then adj(e.A) = New List(Of (Integer, Integer))()
            If Not adj.ContainsKey(e.B) Then adj(e.B) = New List(Of (Integer, Integer))()
            adj(e.A).Add((e.B, order))
            adj(e.B).Add((e.A, order))
        Next

        ' 邻接按类号升序，保证两侧的生成树选择尽可能一致（确定性）
        For Each kv In adj
            kv.Value.Sort(Function(x, y) x.nb.CompareTo(y.nb))
        Next

        Dim sorted As New List(Of Integer)(classes)
        sorted.Sort()

        Dim seen As New HashSet(Of Integer)()
        Dim parts As New List(Of String)()

        For Each c As Integer In sorted
            If seen.Contains(c) Then Continue For

            Dim sb As New StringBuilder()
            EmitAtom(c, -1, sb, adj, seen, mol, atomOfCls, emitted)
            parts.Add(sb.ToString())
        Next

        Return String.Join(".", parts)
    End Function

    Private Sub EmitAtom(cls As Integer, parent As Integer, sb As StringBuilder,
                         adj As Dictionary(Of Integer, List(Of (nb As Integer, order As Integer))),
                         seen As HashSet(Of Integer),
                         mol As Molecule, atomOfCls As Dictionary(Of Integer, Integer),
                         emitted As HashSet(Of String))

        seen.Add(cls)

        Dim atom As Integer = atomOfCls(cls)
        sb.Append(AtomToken(cls, mol.Elements(atom), mol.Charges(atom), mol.TotalH(atom)))

        If Not adj.ContainsKey(cls) Then Return

        For Each nb In adj(cls)
            If nb.nb = parent Then Continue For
            If seen.Contains(nb.nb) Then Continue For

            Dim lo As Integer = Math.Min(cls, nb.nb)
            Dim hi As Integer = Math.Max(cls, nb.nb)
            emitted.Add(lo & "_" & hi)

            sb.Append("("c)
            sb.Append(BondSymbol(nb.order))
            EmitAtom(nb.nb, cls, sb, adj, seen, mol, atomOfCls, emitted)
            sb.Append(")"c)
        Next
    End Sub

    ''' <summary>
    ''' 模式原子记号：[元素(Hn)(电荷):类号]。
    ''' Hn = 源分子中该原子的总氢数（显式 + 隐式）。保留氢数约束是必要的：只靠
    ''' "元素 + 键级" 的模式过于宽松，会把目标分子误切成碰巧落入汇集合的碎片
    ''' （实测出现过把分支酸"水解"成甲醇 + 甲酸的伪通路）。代价是牺牲一部分底物
    ''' 混杂性——这正是 readme §6 所述"化学合理性 vs 泛化能力"的权衡。
    ''' </summary>
    Private Function AtomToken(cls As Integer, el As String, charge As Integer, hCount As Integer) As String
        Dim sb As New StringBuilder()

        sb.Append("["c).Append(el)

        If hCount > 0 Then sb.Append("H").Append(hCount.ToString())

        If charge > 0 Then
            sb.Append("+"c)
            If charge > 1 Then sb.Append(charge.ToString())
        ElseIf charge < 0 Then
            sb.Append("-"c)
            If charge < -1 Then sb.Append(Math.Abs(charge).ToString())
        End If

        sb.Append(":"c).Append(cls).Append("]"c)

        Return sb.ToString()
    End Function

    Private Function BondSymbol(order As Integer) As String
        Select Case order
            Case 2 : Return "="
            Case 3 : Return "#"
            Case Else : Return "-"
        End Select
    End Function

    Private Function Find(parent As Dictionary(Of Integer, Integer), x As Integer) As Integer
        Dim root As Integer = x
        While parent(root) <> root
            root = parent(root)
        End While
        ' 路径压缩
        Dim cur As Integer = x
        While parent(cur) <> root
            Dim nxt As Integer = parent(cur)
            parent(cur) = root
            cur = nxt
        End While
        Return root
    End Function

    Private Sub CountSkip(skipped As Dictionary(Of String, Integer), reason As String)
        If skipped Is Nothing Then Return
        Dim n As Integer = 0
        skipped.TryGetValue(reason, n)
        skipped(reason) = n + 1
    End Sub

    ''' <summary>记录一条反应被跳过的原因（计数 + 逐条 trace，便于排查"某个反应为什么没进规则库"）</summary>
    Private Sub Bail(skipped As Dictionary(Of String, Integer),
                     trace As Dictionary(Of String, String),
                     id As String, reason As String)
        CountSkip(skipped, reason)
        If trace IsNot Nothing AndAlso id IsNot Nothing Then trace(id) = reason
    End Sub

    ''' <summary>类图上的一条边：两侧键级（0 = 该侧不存在此键）</summary>
    Private Class PatternEdge : Implements IComparable(Of PatternEdge)

        Public Property A As Integer
        Public Property B As Integer
        Public Property OrderR As Integer
        Public Property OrderP As Integer

        ''' <summary>键级改变或只在一侧存在 = 该键属于反应中心</summary>
        Public ReadOnly Property Changed As Boolean
            Get
                Return OrderR <> OrderP
            End Get
        End Property

        ''' <summary>生成森林时优先保留发生变化的键</summary>
        Public Function CompareTo(other As PatternEdge) As Integer Implements IComparable(Of PatternEdge).CompareTo
            If other Is Nothing Then Return 1
            If Changed <> other.Changed Then Return If(Changed, -1, 1)
            If A <> other.A Then Return A.CompareTo(other.A)
            Return B.CompareTo(other.B)
        End Function

        Public Overrides Function ToString() As String
            Return $"[{A}:{B}] R={OrderR} P={OrderP}"
        End Function

    End Class

End Module
