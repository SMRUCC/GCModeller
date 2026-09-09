' ============================================================================
' Molecule.vb — 分子图模型：原子/键、价态与隐式氢、连通分量、Morgan EC 规范键
' ----------------------------------------------------------------------------
' [readme.md §2.1 原子映射/EC 方法] 基于 Morgan 迭代精化的扩展连通性标注：
'   初始不变量 = (元素, 电荷, 显式H, 重原子度, 键级多重集)；
'   迭代 = (旧标签, 邻居(标签, 键级) 多重集)，分区数不再增长即收敛。
' mol_key = 原子不变量 + 键三元组（秩编号）的确定性指纹——同构图同键、
'   异构图（几乎必然）异键，用于搜索去重与汇集合匹配 [readme.md §3 去重]。
' 价态模型：隐式 H = max(0, 价态 − Σ键级 − 显式H)；价态违规的应用被拒绝。
' ============================================================================

Imports SMRUCC.genomics.ComponentModel.Chemical

Namespace Chem

    ''' <summary>
    ''' 分子图模型：以原子数组 + 键列表描述的共价分子（可含多个连通分量）。
    ''' </summary>
    ''' <remarks>
    ''' <list type="bullet">
    ''' <item>原子以 0 基索引标识，四个平行数组 <see cref="Elements"/>、<see cref="Charges"/>、
    ''' <see cref="ExplicitH"/> 与键列表 <see cref="Bonds"/> 共同构成分子；</item>
    ''' <item>氢以"隐式 + 显式"两段表示：隐式氢由价态与已用键级反推（见 <see cref="ImplicitH"/>），
    ''' 显式氢用于括号内写出的 <c>[NH3+]</c> 之类记号；</item>
    ''' <item>不感知芳香性——芳香体系一律以 Kekulé 式的单/双键表达；不感知立体化学。</item>
    ''' </list>
    ''' 该模型是整个搜索的化学基座：<see cref="MolKey"/> 提供同构无关的规范指纹，
    ''' 用于搜索去重与汇集合判定。
    ''' </remarks>
    Public Class Molecule

        ''' <summary>
        ''' 各原子的元素符号（如 "C"、"N"、"O"、"Cl"）。
        ''' </summary>
        ''' <returns>长度等于 <see cref="NumAtoms"/> 的元素符号列表，下标即原子索引。</returns>
        Public Property Elements As List(Of String)

        ''' <summary>
        ''' 各原子的形式电荷（如羧基氧为 -1、铵根氮为 +1）。
        ''' </summary>
        ''' <returns>长度等于 <see cref="NumAtoms"/> 的电荷列表。</returns>
        Public Property Charges As List(Of Int32)

        ''' <summary>
        ''' 各原子的显式氢数目（来自 <c>[NH3+]</c> 这类括号记号；隐式氢不计入此处）。
        ''' </summary>
        ''' <returns>长度等于 <see cref="NumAtoms"/> 的显式氢计数列表。</returns>
        Public Property ExplicitH As List(Of Int32)

        ''' <summary>
        ''' 化学键列表，每项为 (a, b, order)，其中 order 取 1/2/3（单/双/三键）。
        ''' </summary>
        ''' <returns>分子内全部共价键。</returns>
        ''' <remarks>
        ''' 请优先使用 <see cref="AddBond"/>、<see cref="SetBondOrder"/>、<see cref="RemoveBond"/>
        ''' 修改键表：直接改这个列表会让下面几个派生缓存失效不了，从而读到陈旧结果。
        ''' </remarks>
        Public Property Bonds As List(Of Bond)

        ''' <summary>邻接表缓存（惰性构建）：每个原子一个 (邻居索引, 键级) 列表。</summary>
        Private _adjacency As List(Of List(Of (Int32, Int32))) = Nothing
        ''' <summary>Morgan 秩缓存（惰性构建）。</summary>
        Private _morgan As List(Of String) = Nothing
        ''' <summary><see cref="MolKey"/> 结果缓存（惰性构建）。</summary>
        Private _molKey As String = Nothing

        ''' <summary>
        ''' 创建一个不含任何原子与键的空分子。
        ''' </summary>
        Public Sub New()
            Elements = New List(Of String)()
            Charges = New List(Of Int32)()
            ExplicitH = New List(Of Int32)()
            Bonds = New List(Of Bond)()
        End Sub

        ''' <summary>
        ''' 作废全部派生缓存（邻接表 / Morgan 秩 / 分子指纹）。
        ''' 任何改动元素、电荷、显式氢或键表的操作都必须调用它。
        ''' </summary>
        Private Sub InvalidateStructure()
            _adjacency = Nothing
            _morgan = Nothing
            _molKey = Nothing
        End Sub

        ''' <summary>
        ''' 邻接表（惰性构建，O(键数)）：每个原子一个 (邻居索引, 键级) 列表。
        ''' </summary>
        ''' <returns>长度等于原子数的邻接表。</returns>
        ''' <remarks>
        ''' 原先 <see cref="Neighbors"/> 每次调用都要扫描整张键表并新建一个列表，
        ''' 在 Morgan 迭代（每原子 × 多轮）、价态校验、连通分量里被反复调用，
        ''' 实际复杂度退化到 O(原子数 × 键数 × 轮数)。热路径统一走这里。
        ''' </remarks>
        Public Function Adjacency() As List(Of List(Of (Int32, Int32)))
            If _adjacency IsNot Nothing Then Return _adjacency

            Dim adj As New List(Of List(Of (Int32, Int32)))()
            For i = 0 To NumAtoms() - 1
                adj.Add(New List(Of (Int32, Int32))())
            Next
            For Each bd In Bonds
                If bd.a >= 0 AndAlso bd.a < adj.Count AndAlso bd.b >= 0 AndAlso bd.b < adj.Count Then
                    adj(bd.a).Add((bd.b, bd.order))
                    adj(bd.b).Add((bd.a, bd.order))
                End If
            Next

            _adjacency = adj
            Return adj
        End Function

        ''' <summary>
        ''' 分子中的原子总数（含氢以外的全部显式原子）。
        ''' </summary>
        ''' <returns>原子个数。</returns>
        Public Function NumAtoms() As Int32
            Return Elements.Count
        End Function

        ''' <summary>
        ''' 取某原子的全部邻接原子及其键级。
        ''' </summary>
        ''' <param name="a">原子索引。</param>
        ''' <returns>邻居列表，每项为 (邻接原子索引, 键级)；无邻居时返回空列表。</returns>
        Public Function Neighbors(a As Int32) As List(Of Tuple(Of Int32, Int32))
            Dim outList As New List(Of Tuple(Of Int32, Int32))()
            For Each nb In Adjacency()(a)
                outList.Add(Tuple.Create(nb.Item1, nb.Item2))
            Next
            Return outList
        End Function

        ''' <summary>
        ''' 某原子的重原子度（与之成键的原子个数，键级不参与计数）。
        ''' </summary>
        ''' <param name="a">原子索引。</param>
        ''' <returns>邻居原子个数。</returns>
        Public Function Degree(a As Int32) As Int32
            Return Adjacency()(a).Count
        End Function

        ''' <summary>
        ''' 查询两个原子之间的键级。
        ''' </summary>
        ''' <param name="a">第一个原子索引。</param>
        ''' <param name="b">第二个原子索引。</param>
        ''' <returns>键级 1/2/3；两原子间无键时返回 0。</returns>
        Public Function BondOrder(a As Int32, b As Int32) As Int32
            For Each nb In Adjacency()(a)
                If nb.Item1 = b Then Return nb.Item2
            Next
            Return 0
        End Function

        ''' <summary>
        ''' 修改已存在键的键级（如单键改双键）；键不存在时静默忽略。
        ''' </summary>
        ''' <param name="a">键一端原子索引。</param>
        ''' <param name="b">键另一端原子索引。</param>
        ''' <param name="newOrder">新的键级（1/2/3）。</param>
        Public Sub SetBondOrder(a As Int32, b As Int32, newOrder As Int32)
            For i = 0 To Bonds.Count - 1
                Dim bd = Bonds(i)
                If (bd.a = a AndAlso bd.b = b) OrElse (bd.a = b AndAlso bd.b = a) Then
                    Bonds(i) = (bd.a, bd.b, newOrder)
                    InvalidateStructure()
                    Return
                End If
            Next
        End Sub

        ''' <summary>
        ''' 删除两个原子之间的键（若存在）。断键后两者可能落入不同连通分量。
        ''' </summary>
        ''' <param name="a">键一端原子索引。</param>
        ''' <param name="b">键另一端原子索引。</param>
        Public Sub RemoveBond(a As Int32, b As Int32)
            Dim next_ As New List(Of Bond)()
            Dim removed As Boolean = False

            For Each bd In Bonds
                If (bd.a = a AndAlso bd.b = b) OrElse (bd.a = b AndAlso bd.b = a) Then
                    removed = True
                Else
                    next_.Add(bd)
                End If
            Next

            If removed Then
                Bonds = next_
                InvalidateStructure()
            End If
        End Sub

        ''' <summary>
        ''' 追加一条键（唯一推荐的"加键"入口，会自动作废派生缓存）。
        ''' </summary>
        ''' <param name="a">键一端原子索引。</param>
        ''' <param name="b">键另一端原子索引。</param>
        ''' <param name="order">键级 1/2/3。</param>
        Public Sub AddBond(a As Int32, b As Int32, order As Int32)
            Bonds.Add((a, b, order))
            InvalidateStructure()
        End Sub

        ''' <summary>
        ''' 修改某个原子的元素符号（会作废派生缓存）。
        ''' </summary>
        Public Sub SetElement(a As Int32, el As String)
            Elements(a) = el
            InvalidateStructure()
        End Sub

        ''' <summary>
        ''' 修改某个原子的形式电荷（会作废派生缓存）。
        ''' </summary>
        Public Sub SetCharge(a As Int32, charge As Int32)
            Charges(a) = charge
            InvalidateStructure()
        End Sub

        ''' <summary>
        ''' 修改某个原子的显式氢数（会作废派生缓存）。
        ''' </summary>
        Public Sub SetExplicitH(a As Int32, h As Int32)
            ExplicitH(a) = h
            InvalidateStructure()
        End Sub

        ''' <summary>
        ''' 向分子追加一个原子（不带任何键），返回其索引。
        ''' </summary>
        ''' <param name="el">元素符号，须能被价态模型识别（如 "C"、"N"、"O"、"P"、"S"）。</param>
        ''' <param name="charge">形式电荷。</param>
        ''' <returns>新原子的索引（等于追加前的原子总数）。</returns>
        Public Function AddAtom(el As String, charge As Int32) As Int32
            Elements.Add(el)
            Charges.Add(charge)
            ExplicitH.Add(0)
            InvalidateStructure()
            Return Elements.Count - 1
        End Function

        ''' <summary>
        ''' 深拷贝当前分子（元素/电荷/显式氢/键均为新列表，改动互不影响）。
        ''' </summary>
        ''' <returns>与当前分子结构相同的新 <see cref="Molecule"/> 实例。</returns>
        Public Function Copy() As Molecule
            Dim m As New Molecule()
            m.Elements = New List(Of String)(Elements)
            m.Charges = New List(Of Int32)(Charges)
            m.ExplicitH = New List(Of Int32)(ExplicitH)
            m.Bonds = Bonds.ToList()
            Return m
        End Function

        ''' <summary>
        ''' 计算某原子的隐式氢数：max(0, 价态 − Σ键级 − 显式氢)。
        ''' </summary>
        ''' <param name="a">原子索引。</param>
        ''' <returns>隐式氢个数。</returns>
        Public Function ImplicitH(a As Int32) As Int32
            Dim used As Int32 = 0
            For Each nb In Adjacency()(a)
                used += nb.Item2
            Next
            Return Math.Max(0, ChemicalExtensions.ValenceOf(Elements(a), Charges(a)) - used - ExplicitH(a))
        End Function

        ''' <summary>
        ''' 某原子上的氢总数 = 显式氢 + 隐式氢（模式匹配中的 Hn 约束即与此值比较）。
        ''' </summary>
        ''' <param name="a">原子索引。</param>
        ''' <returns>氢总数。</returns>
        Public Function TotalH(a As Int32) As Int32
            Return ExplicitH(a) + ImplicitH(a)
        End Function

        ''' <summary>
        ''' 价态校验：返回价态超限（已成键级 + 显式氢 &gt; 容许价态）的原子列表。
        ''' </summary>
        ''' <returns>违规原子索引列表；为空表示全分子价态合法。</returns>
        ''' <remarks>
        ''' 这是规则应用的"化学合理性闸门"——变换后价态非法的产物一律被丢弃。
        ''' </remarks>
        Public Function ValenceViolations() As List(Of Int32)
            Dim bad As New List(Of Int32)()
            Dim adj = Adjacency()

            For a = 0 To NumAtoms() - 1
                Dim used As Int32 = 0
                For Each nb In adj(a)
                    used += nb.Item2
                Next
                used += ExplicitH(a)
                If used > ValenceOf(Elements(a), Charges(a)) Then bad.Add(a)
            Next
            Return bad
        End Function

        ''' <summary>
        ''' 计算连通分量，即把多组分分子（如 "A.B.C"）拆成若干个独立片段的原子集合。
        ''' </summary>
        ''' <returns>每个分量一个原子索引列表（内部已按索引升序排列）。</returns>
        Public Function Components() As List(Of List(Of Int32))
            Dim seen(NumAtoms() - 1) As Boolean
            Dim comps As New List(Of List(Of Int32))()
            Dim adj = Adjacency()

            For a = 0 To NumAtoms() - 1
                If seen(a) Then Continue For
                Dim comp As New List(Of Int32)()
                Dim stack As New Stack(Of Int32)()
                stack.Push(a)
                seen(a) = True
                While stack.Count > 0
                    Dim x = stack.Pop()
                    comp.Add(x)
                    For Each nb In adj(x)
                        If Not seen(nb.Item1) Then
                            seen(nb.Item1) = True
                            stack.Push(nb.Item1)
                        End If
                    Next
                End While
                comp.Sort()
                comps.Add(comp)
            Next
            Return comps
        End Function

        ''' <summary>
        ''' 把每个连通分量提取成独立的 <see cref="Molecule"/> 实例（原子索引重新编号）。
        ''' </summary>
        ''' <returns>独立分子列表；单组分分子返回仅含自身的列表。</returns>
        Public Function SplitComponents() As List(Of Molecule)
            Dim outList As New List(Of Molecule)()
            For Each comp In Components()
                Dim fm As New Molecule()
                Dim remap As New Dictionary(Of Int32, Int32)()
                For Each a In comp
                    remap(a) = fm.AddAtom(Elements(a), Charges(a))
                    fm.SetExplicitH(remap(a), ExplicitH(a))
                Next
                For Each bd In Bonds
                    If remap.ContainsKey(bd.a) AndAlso remap.ContainsKey(bd.b) Then
                        fm.AddBond(remap(bd.a), remap(bd.b), bd.order)
                    End If
                Next
                outList.Add(fm)
            Next
            Return outList
        End Function

        ''' <summary>
        ''' 计算 Morgan/扩展连通性（EC）迭代精化标签：以原子化学环境为初值反复用邻居
        ''' 标签精化，直到分区数不再增长（或达到轮数上限）。
        ''' </summary>
        ''' <param name="rounds">最大迭代轮数；分区数收敛时会提前退出。</param>
        ''' <returns>每个原子一个标签字符串；标签相同表示化学环境等价（用于规范化排序与指纹）。</returns>
        ''' <remarks>
        ''' 初值为 (元素, 电荷, 氢总数, 重原子度, 邻接键级多重集)；迭代式为
        ''' (旧标签, 邻居(标签, 键级) 多重集)。这是 <see cref="MolKey"/> 与 SMILES
        ''' 确定性写出的排序依据。
        ''' </remarks>
        Public Function MorganRanks(Optional rounds As Int32 = 8) As List(Of String)
            If _morgan IsNot Nothing Then Return _morgan

            Dim adj = Adjacency()
            Dim labels As New List(Of String)()
            For a = 0 To NumAtoms() - 1
                Dim orders = adj(a).Select(Function(nb) nb.Item2).OrderBy(Function(x) x)
                labels.Add($"{Elements(a)}|{Charges(a)}|{TotalH(a)}|{adj(a).Count}|" &
                           String.Join(",", orders))
            Next
            Dim prevCount = labels.Distinct().Count()
            For r = 1 To rounds
                Dim newLabels As New List(Of String)()
                For a = 0 To NumAtoms() - 1
                    Dim nbStr = String.Join(";", Neighbors(a).
                        Select(Function(nb) labels(nb.Item1) & ":" & nb.Item2).OrderBy(Function(x) x, StringComparer.Ordinal))
                    newLabels.Add(labels(a) & "#" & nbStr)
                Next
                ' 压缩标签
                Dim uniq = newLabels.Distinct().OrderBy(Function(x) x, StringComparer.Ordinal).ToList()
                Dim idxMap As New Dictionary(Of String, Int32)()
                For i = 0 To uniq.Count - 1
                    idxMap(uniq(i)) = i
                Next
                labels = newLabels.Select(Function(x) idxMap(x).ToString()).ToList()
                Dim cnt = labels.Distinct().Count()
                If cnt = prevCount Then Exit For
                prevCount = cnt
            Next
            _morgan = labels
            Return labels
        End Function

        ''' <summary>
        ''' 分子规范指纹：同构的分子必然得到相同字符串，异构分子（几乎必然）得到不同字符串。
        ''' </summary>
        ''' <returns>由 Morgan 秩编号后的原子不变量与键三元组拼成的确定性字符串。</returns>
        ''' <remarks>
        ''' 搜索用它做三件事：状态去重、循环消除、以及判定某个化合物是否属于底盘汇集合。
        ''' 注意它并非严格的规范 SMILES——正则图的极端情形下理论上存在碰撞，但在代谢物
        ''' 尺度可忽略。
        ''' 
        ''' 指纹一律采用"氢总数"（<see cref="TotalH"/> = 显式 + 隐式）而非"显式氢"：对价态合法的
        ''' 原子，氢总数由元素、电荷与键级唯一决定，而显式氢数只反映 SMILES 的书写方式。若用
        ''' 显式氢，同一个分子写成 <c>N</c> 还是 <c>[NH3+]</c> 会得到两个不同的指纹，导致规则
        ''' 逆向生成的化合物与库里的同一化合物对不上（通路无法在起点 A 处收束）。
        ''' </remarks>
        Public Function MolKey() As String
            If _molKey IsNot Nothing Then Return _molKey

            Dim ranks = MorganRanks()
            Dim uniq = ranks.Distinct().OrderBy(Function(x) x, StringComparer.Ordinal).ToList()
            Dim idxMap As New Dictionary(Of String, Int32)()
            For i = 0 To uniq.Count - 1
                idxMap(uniq(i)) = i
            Next
            Dim rk = ranks.Select(Function(x) idxMap(x)).ToList()
            Dim atomList As New List(Of String)()
            For a = 0 To NumAtoms() - 1
                atomList.Add($"{Elements(a)}|{Charges(a)}|{TotalH(a)}|{rk(a)}")
            Next
            atomList.Sort(StringComparer.Ordinal)
            Dim bondList As New List(Of String)()
            For Each bd In Bonds
                Dim ra = rk(bd.a)
                Dim rb = rk(bd.b)
                bondList.Add($"{Math.Min(ra, rb)}-{Math.Max(ra, rb)}:{bd.order}")
            Next
            bondList.Sort(StringComparer.Ordinal)
            _molKey = String.Join(";", atomList) & "#" & String.Join(";", bondList)
            Return _molKey
        End Function

    End Class

End Namespace
