' ============================================================================
' BioCycRuleMiner.vb — BioCyc 反应实例 → RetroPath 广义反应规则
' ----------------------------------------------------------------------------
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
Imports SMRUCC.genomics.Data.BioCyc
Imports SMRUCC.genomics.Data.BioCyc.Assembly.MetaCyc.Schema.Metabolism

''' <summary>
''' 一个可被 RetroPath 使用的化合物结构（净化后的 SMILES + 分子图）
''' </summary>
Public Class CompoundStructure

    ''' <summary>BioCyc 的 compound frame id（UNIQUE-ID）</summary>
    Public Property Id As String
    ''' <summary>净化后的 SMILES（剔除立体标记之后的字符串）</summary>
    Public Property Smiles As String
    ''' <summary>该 SMILES 对应的分子图</summary>
    Public Property Mol As Molecule

    Public Overrides Function ToString() As String
        Return $"{Id} ({Smiles})"
    End Function

End Class

Public Module BioCycRuleMiner

    ''' <summary>
    ''' 逐条反应挖掘广义反应规则。
    ''' </summary>
    ''' <param name="reactionList">BioCyc 反应集合（建议按 uniqueId 排序以保证确定性）</param>
    ''' <param name="structures">compound frame id → 已净化结构（缺失即无法解析的化合物）</param>
    ''' <param name="maxMoleculeAtoms">参与反应的分子重原子数上限（超出的反应跳过，控耗时）</param>
    ''' <param name="maxPatternAtoms">模式原子数上限（超出的规则过特异，跳过）</param>
    ''' <param name="mcsNodeBudget">MCS 回溯搜索的节点预算</param>
    ''' <param name="includeBuiltin">是否叠加 RetroPath 内置的 9 条广义规则</param>
    ''' <param name="skipped">跳过原因计数（可为 Nothing）</param>
    Public Function Mine(reactionList As IEnumerable(Of reactions),
                         structures As Dictionary(Of String, CompoundStructure),
                         Optional maxMoleculeAtoms As Integer = 80,
                         Optional maxPatternAtoms As Integer = 32,
                         Optional mcsNodeBudget As Integer = 60000,
                         Optional maxUnmappedAtoms As Integer = 3,
                         Optional includeBuiltin As Boolean = False,
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

        For Each rxn As reactions In reactionList
            If rxn Is Nothing Then
                CountSkip(skipped, "null-reaction")
                Continue For
            End If

            Dim rule As Rule = MineOne(rxn, structures, maxMoleculeAtoms,
                                       maxPatternAtoms, mcsNodeBudget, maxUnmappedAtoms, skipped, trace)
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
    Private Function MineOne(rxn As reactions,
                             structures As Dictionary(Of String, CompoundStructure),
                             maxMoleculeAtoms As Integer,
                             maxPatternAtoms As Integer,
                             mcsNodeBudget As Integer,
                             maxUnmappedAtoms As Integer,
                             skipped As Dictionary(Of String, Integer),
                             Optional trace As Dictionary(Of String, String) = Nothing) As Rule

        Dim rxnId As String = If(rxn Is Nothing, Nothing, rxn.uniqueId)

        ' ---- 1) 方向归正：不要直接用 left/right，equation 已按 REACTION-DIRECTION 归正
        Dim eq As Equation = Nothing

        Try
            eq = rxn.equation
        Catch ex As Exception
            Bail(skipped, trace, rxnId, "bad-equation")
            Return Nothing
        End Try

        If eq Is Nothing Then
            Bail(skipped, trace, rxnId, "no-equation")
            Return Nothing
        End If

        Dim rIds As List(Of String) = CompoundIds(eq.Reactants)
        Dim pIds As List(Of String) = CompoundIds(eq.Products)

        If rIds.Count = 0 OrElse pIds.Count = 0 Then
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

        ' ---- 4) 反应中心 = 未映射原子 ∪ 其 1 层邻居 [§2.2 / §2.3]
        If map.ReactantOnly.Count = 0 AndAlso map.ProductOnly.Count = 0 Then
            Bail(skipped, trace, rxnId, "no-reaction-center")
            Return Nothing
        End If

        Dim centerR As New SortedSet(Of Integer)()
        Dim centerP As New SortedSet(Of Integer)()

        ' (a) 种子 = 未映射原子（断键/成键/键级改变的位点）+ 其 1 层邻居作为泛化环境。
        '     只从种子扩展一层，避免不动点迭代把整个分子都吞进模式。
        For Each r As Integer In map.ReactantOnly
            centerR.Add(r)
            For Each nb In rMol.Neighbors(r)
                centerR.Add(nb.Item1)
            Next
        Next
        For Each p As Integer In map.ProductOnly
            centerP.Add(p)
            For Each nb In pMol.Neighbors(p)
                centerP.Add(nb.Item1)
            Next
        Next

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
            If Not (centerR.Contains(pr.r) OrElse centerP.Contains(pr.p)) Then Continue For

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

        ' ---- 7) 生成森林：优先保留发生变化的键（它们才是反应中心的化学含义）
        Dim edges As New List(Of PatternEdge)(edgeMap.Values)
        edges.Sort()

        Dim parent As New Dictionary(Of Integer, Integer)()
        For Each c As Integer In clsR.Values
            parent(c) = c
        Next
        For Each c As Integer In clsP.Values
            If Not parent.ContainsKey(c) Then parent(c) = c
        Next

        Dim forest As New List(Of PatternEdge)()

        For Each e As PatternEdge In edges
            Dim ra As Integer = Find(parent, e.A)
            Dim rb As Integer = Find(parent, e.B)
            If ra = rb Then Continue For
            parent(ra) = rb
            forest.Add(e)
        Next

        ' ---- 8) 发射两侧模式（每个类号在所在侧都必须至少有一条键）
        Dim rText As String = EmitSide(clsR.Values.ToList(), forest, True, rMol, atomOfClsR)
        Dim pText As String = EmitSide(clsP.Values.ToList(), forest, False, pMol, atomOfClsP)

        If rText Is Nothing OrElse pText Is Nothing Then
            Bail(skipped, trace, rxnId, "emit-isolated")
            Return Nothing
        End If
        If clsR.Count > maxPatternAtoms OrElse clsP.Count > maxPatternAtoms Then
            Bail(skipped, trace, rxnId, "pattern-too-large")
            Return Nothing
        End If

        ' ---- 9) 规则评分参数：ΔG 取 BioCyc GIBBS-0，酶层级按 EC 号判定
        Dim dg As Double = rxn.gibbs0
        If Double.IsNaN(dg) OrElse Double.IsInfinity(dg) Then dg = 0

        Dim tier As EnzymeTiers
        If rxn.ec_number IsNot Nothing AndAlso rxn.ec_number.Length > 0 Then
            tier = EnzymeTiers.Common
        ElseIf rxn.spontaneous Then
            tier = EnzymeTiers.General
        Else
            tier = EnzymeTiers.Specialized
        End If

        Dim name As String = rxn.commonName
        If String.IsNullOrEmpty(name) Then name = rxn.systematicName
        If String.IsNullOrEmpty(name) Then name = rxn.uniqueId

        Dim reversible As Boolean = (rxn.reactionDirection = ReactionDirections.Reversible)

        Dim rule As Rule = Nothing

        Try
            rule = New Rule(rxn.uniqueId, name, rText, pText, dg, tier, reversible)
        Catch ex As Exception
            Bail(skipped, trace, rxnId, "pattern-error")
            Return Nothing
        End Try

        ' ---- 10) 自检：规则必须能匹配它自己的底物/产物，否则是无效泛化
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

        Return rule
    End Function

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
    Private Function CompoundIds(side As IEnumerable(Of CompoundSpecieReference)) As List(Of String)
        Dim ids As New SortedSet(Of String)(StringComparer.Ordinal)

        If side IsNot Nothing Then
            For Each c In side
                If c Is Nothing OrElse String.IsNullOrEmpty(c.ID) Then Continue For
                ' BioCyc 偶见 "CPD-1,CPD-2" 形式的并列写法
                ids.Add(c.ID.Split(","c)(0).Trim())
            Next
        End If

        Return ids.ToList()
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
                              forest As List(Of PatternEdge),
                              onReactant As Boolean,
                              mol As Molecule,
                              atomOfCls As Dictionary(Of Integer, Integer)) As String

        Dim adj As New Dictionary(Of Integer, List(Of (nb As Integer, order As Integer)))()

        For Each e As PatternEdge In forest
            Dim order As Integer = If(onReactant, e.OrderR, e.OrderP)
            If order <= 0 Then Continue For

            If Not adj.ContainsKey(e.A) Then adj(e.A) = New List(Of (Integer, Integer))()
            If Not adj.ContainsKey(e.B) Then adj(e.B) = New List(Of (Integer, Integer))()
            adj(e.A).Add((e.B, order))
            adj(e.B).Add((e.A, order))
        Next

        Dim sorted As New List(Of Integer)(classes)
        sorted.Sort()

        Dim seen As New HashSet(Of Integer)()
        Dim parts As New List(Of String)()

        For Each c As Integer In sorted
            If seen.Contains(c) Then Continue For

            Dim sb As New StringBuilder()
            EmitAtom(c, -1, sb, adj, seen, mol, atomOfCls)
            parts.Add(sb.ToString())
        Next

        Return String.Join(".", parts)
    End Function

    Private Sub EmitAtom(cls As Integer, parent As Integer, sb As StringBuilder,
                         adj As Dictionary(Of Integer, List(Of (nb As Integer, order As Integer))),
                         seen As HashSet(Of Integer),
                         mol As Molecule, atomOfCls As Dictionary(Of Integer, Integer))

        seen.Add(cls)

        Dim atom As Integer = atomOfCls(cls)
        sb.Append(AtomToken(cls, mol.Elements(atom), mol.Charges(atom), mol.TotalH(atom)))

        If Not adj.ContainsKey(cls) Then Return

        For Each nb In adj(cls)
            If nb.nb = parent Then Continue For
            If seen.Contains(nb.nb) Then Continue For

            sb.Append("("c)
            sb.Append(BondSymbol(nb.order))
            EmitAtom(nb.nb, cls, sb, adj, seen, mol, atomOfCls)
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
