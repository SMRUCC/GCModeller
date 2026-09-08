' ============================================================================
' AtomMapping.vb — 基于最大公共子图(MCS)的原子映射与反应中心定位
' ----------------------------------------------------------------------------
' [readme_alg.md §2.1 原子映射] 原子映射是 NP 难问题（其包含子图同构问题作为特例）。
'   此处采用"键级保守的诱导式最大公共子图"：任意两个已映射原子对之间，键的存在性
'   与键级必须完全一致（ordA(i,j) = ordB(i,j)，不存在的键记为 0）。因此反应中心处
'   （键级改变/断键/成键）的原子会自然落出 MCS，表现为未映射原子——这正是
'   [§2.2 反应中心识别] 所需要的差分信息。
' 搜索策略：
'   1) 先跑一遍贪心（按 Morgan 型不变量排序的候选优先）得到下界；
'   2) 若贪心已达到"足够好"的目标（min(|A|,|B|) - maxUnmapped）则直接返回——
'      反应中心通常很小，不必求真正的最优解；
'   3) 否则带节点预算的回溯搜索：上界剪枝 + 预算耗尽保留当前最优（随时算法）。
' ============================================================================

Imports SMRUCC.genomics.Analysis.RetroPath.Chem

''' <summary>
''' 一次反应两侧（反应物集合 ↔ 产物集合）的原子映射结果
''' </summary>
Public Class MappingResult

    ''' <summary>(反应物原子索引, 产物原子索引)</summary>
    Public ReadOnly Property Pairs As New List(Of (r As Integer, p As Integer))

    ''' <summary>反应物原子 → 产物原子（-1 = 未映射）</summary>
    Public Property ReactantToProduct As Integer()

    ''' <summary>产物原子 → 反应物原子（-1 = 未映射）</summary>
    Public Property ProductToReactant As Integer()

    ''' <summary>反应物侧的未映射原子（离去基团）</summary>
    Public ReadOnly Property ReactantOnly As New List(Of Integer)

    ''' <summary>产物侧的未映射原子（加入基团）</summary>
    Public ReadOnly Property ProductOnly As New List(Of Integer)

End Class

Public Module AtomMapping

    ''' <summary>
    ''' 对反应两侧做原子映射。两侧都可以是多组分分子（用 "." 拼起来的底物集合/产物集合），
    ''' 匹配得到的公共子图允许跨组分断开——这正是"辅因子部分保持不变"所需要的语义。
    ''' </summary>
    ''' <param name="reactants">反应物侧分子（可含多个连通分量）</param>
    ''' <param name="products">产物侧分子（可含多个连通分量）</param>
    ''' <param name="nodeBudget">回溯搜索的节点预算，耗尽即返回当前最优</param>
    ''' <param name="maxUnmapped">允许的未映射原子数（≈反应中心规模），用于提前终止</param>
    Public Function Map(reactants As Molecule, products As Molecule,
                        Optional nodeBudget As Integer = 60000,
                        Optional maxUnmapped As Integer = 10) As MappingResult

        Dim res As New MappingResult()
        Dim nR As Integer = If(reactants Is Nothing, 0, reactants.NumAtoms())
        Dim nP As Integer = If(products Is Nothing, 0, products.NumAtoms())

        res.ReactantToProduct = If(nR > 0, New Integer(nR - 1) {}, Array.Empty(Of Integer)())
        res.ProductToReactant = If(nP > 0, New Integer(nP - 1) {}, Array.Empty(Of Integer)())

        For i As Integer = 0 To nR - 1
            res.ReactantToProduct(i) = -1
        Next
        For j As Integer = 0 To nP - 1
            res.ProductToReactant(j) = -1
        Next

        If nR = 0 OrElse nP = 0 Then Return res

        ' 以较小的一侧作为搜索探针，压缩搜索深度
        Dim swap As Boolean = nP < nR
        Dim a As Molecule = If(swap, products, reactants)
        Dim b As Molecule = If(swap, reactants, products)

        Dim search As New McsSearch(a, b, nodeBudget, maxUnmapped)
        Dim best As List(Of (Integer, Integer)) = search.Run()

        For Each pr In best
            Dim r As Integer = If(swap, pr.Item2, pr.Item1)
            Dim p As Integer = If(swap, pr.Item1, pr.Item2)

            res.Pairs.Add((r, p))
            res.ReactantToProduct(r) = p
            res.ProductToReactant(p) = r
        Next

        For i As Integer = 0 To nR - 1
            If res.ReactantToProduct(i) < 0 Then res.ReactantOnly.Add(i)
        Next
        For j As Integer = 0 To nP - 1
            If res.ProductToReactant(j) < 0 Then res.ProductOnly.Add(j)
        Next

        Return res
    End Function

    ''' <summary>键级保守的诱导式最大公共子图搜索（贪心下界 + 带预算回溯）</summary>
    Private Class McsSearch

        Private ReadOnly a As Molecule
        Private ReadOnly b As Molecule
        Private ReadOnly nA As Integer
        Private ReadOnly nB As Integer
        Private ReadOnly ordA As Integer(,)
        Private ReadOnly ordB As Integer(,)
        Private ReadOnly invA As String()
        Private ReadOnly invB As String()
        Private ReadOnly cands As List(Of Integer)()
        Private ReadOnly order As Integer()
        Private ReadOnly budget As Integer
        Private ReadOnly target As Integer

        Private mapAB As Integer()
        Private mapBA As Integer()
        Private cur As New List(Of (Integer, Integer))
        Private best As New List(Of (Integer, Integer))
        Private nodes As Integer = 0
        Private stopped As Boolean = False

        Sub New(a As Molecule, b As Molecule, budget As Integer, maxUnmapped As Integer)
            Me.a = a
            Me.b = b
            Me.nA = a.NumAtoms()
            Me.nB = b.NumAtoms()
            Me.budget = budget
            Me.ordA = BondMatrix(a)
            Me.ordB = BondMatrix(b)
            Me.invA = Invariants(a, ordA)
            Me.invB = Invariants(b, ordB)
            Me.order = BuildOrder()
            Me.cands = BuildCandidates()
            Me.target = Math.Max(0, Math.Min(nA, nB) - Math.Max(0, maxUnmapped))
        End Sub

        Public Function Run() As List(Of (Integer, Integer))
            best = Greedy()
            If best.Count >= target Then Return best

            ' 重置后进入回溯搜索（以贪心结果为下界）
            mapAB = New Integer(nA - 1) {}
            mapBA = New Integer(nB - 1) {}
            For i As Integer = 0 To nA - 1
                mapAB(i) = -1
            Next
            For j As Integer = 0 To nB - 1
                mapBA(j) = -1
            Next
            cur = New List(Of (Integer, Integer))()

            Dfs(0)
            Return best
        End Function

        Private Sub ResetMaps()
            mapAB = New Integer(nA - 1) {}
            mapBA = New Integer(nB - 1) {}
            For i As Integer = 0 To nA - 1
                mapAB(i) = -1
            Next
            For j As Integer = 0 To nB - 1
                mapBA(j) = -1
            Next
        End Sub

        ''' <summary>贪心：按不变量优先顺序尽可能扩展，得到下界</summary>
        Private Function Greedy() As List(Of (Integer, Integer))
            ResetMaps()
            Dim res As New List(Of (Integer, Integer))()

            For Each ai As Integer In order
                For Each bj As Integer In cands(ai)
                    If mapBA(bj) >= 0 Then Continue For
                    If Not Consistent(ai, bj, res) Then Continue For

                    mapAB(ai) = bj
                    mapBA(bj) = ai
                    res.Add((ai, bj))
                    Exit For
                Next
            Next

            Return res
        End Function

        Private Sub Dfs(k As Integer)
            If stopped Then Return

            nodes += 1
            If nodes > budget Then
                stopped = True
                Return
            End If

            ' 上界剪枝：剩余原子全部映射也不及当前最优
            If cur.Count + (order.Length - k) <= best.Count Then Return

            If k = order.Length Then
                If cur.Count > best.Count Then
                    best = New List(Of (Integer, Integer))(cur)
                    If best.Count >= target Then stopped = True
                End If
                Return
            End If

            Dim ai As Integer = order(k)

            ' 分支一：把 ai 映射到某个兼容的产物原子
            For Each bj As Integer In cands(ai)
                If mapBA(bj) >= 0 Then Continue For
                If Not Consistent(ai, bj, cur) Then Continue For

                mapAB(ai) = bj
                mapBA(bj) = ai
                cur.Add((ai, bj))

                Dfs(k + 1)

                cur.RemoveAt(cur.Count - 1)
                mapBA(bj) = -1
                mapAB(ai) = -1

                If stopped Then Return
            Next

            ' 分支二：ai 不映射（即它属于反应中心/离去基团）
            Dfs(k + 1)
        End Sub

        ''' <summary>诱导式 MCS 约束：与所有已映射原子对之间的键（含"不存在"）必须完全一致</summary>
        Private Function Consistent(ai As Integer, bj As Integer, mapped As List(Of (Integer, Integer))) As Boolean
            For Each pr In mapped
                If ordA(ai, pr.Item1) <> ordB(bj, pr.Item2) Then Return False
            Next
            Return True
        End Function

        Private Shared Function BondMatrix(m As Molecule) As Integer(,)
            Dim n As Integer = m.NumAtoms()
            Dim ord(n - 1, n - 1) As Integer

            For Each bd In m.Bonds
                If bd.a >= 0 AndAlso bd.a < n AndAlso bd.b >= 0 AndAlso bd.b < n Then
                    ord(bd.a, bd.b) = bd.order
                    ord(bd.b, bd.a) = bd.order
                End If
            Next

            Return ord
        End Function

        ''' <summary>Morgan 型迭代不变量（两侧同一套编码，可跨分子比较）</summary>
        Private Shared Function Invariants(m As Molecule, ord As Integer(,)) As String()
            Dim n As Integer = m.NumAtoms()
            Dim lab(n - 1) As String

            For i As Integer = 0 To n - 1
                Dim orders As New List(Of Integer)()
                For j As Integer = 0 To n - 1
                    If ord(i, j) > 0 Then orders.Add(ord(i, j))
                Next
                orders.Sort()
                lab(i) = $"{m.Elements(i)}|{m.Charges(i)}|{m.ExplicitH(i)}|{orders.Count}|{String.Join(",", orders)}"
            Next

            For r As Integer = 1 To 2
                Dim refined(n - 1) As String
                For i As Integer = 0 To n - 1
                    Dim nb As New List(Of String)()
                    For j As Integer = 0 To n - 1
                        If ord(i, j) > 0 Then nb.Add(lab(j) & ":" & ord(i, j))
                    Next
                    nb.Sort(StringComparer.Ordinal)
                    refined(i) = lab(i) & "#" & String.Join(";", nb)
                Next
                lab = refined
            Next

            Return lab
        End Function

        ''' <summary>原子遍历序：以度降序为根做 BFS，保证除根之外的原子都有前驱相邻</summary>
        Private Function BuildOrder() As Integer()
            Dim keys As New List(Of (deg As Integer, idx As Integer))()

            For i As Integer = 0 To nA - 1
                keys.Add((a.Degree(i), i))
            Next
            keys.Sort(Function(x, y) If(y.deg <> x.deg, y.deg.CompareTo(x.deg), x.idx.CompareTo(y.idx)))

            Dim seen(nA - 1) As Boolean
            Dim res As New List(Of Integer)()

            For Each k In keys
                If seen(k.idx) Then Continue For

                Dim q As New Queue(Of Integer)()
                q.Enqueue(k.idx)
                seen(k.idx) = True

                While q.Count > 0
                    Dim x As Integer = q.Dequeue()
                    res.Add(x)

                    For Each nbr In a.Neighbors(x)
                        If Not seen(nbr.Item1) Then
                            seen(nbr.Item1) = True
                            q.Enqueue(nbr.Item1)
                        End If
                    Next
                End While
            Next

            Return res.ToArray()
        End Function

        ''' <summary>候选对：同元素同电荷，按不变量匹配度排序（越像越先试）</summary>
        Private Function BuildCandidates() As List(Of Integer)()
            Dim lists(nA - 1) As List(Of Integer)

            For i As Integer = 0 To nA - 1
                lists(i) = New List(Of Integer)()
                Dim ia As String = invA(i)
                Dim degA As Integer = a.Degree(i)

                For j As Integer = 0 To nB - 1
                    If b.Elements(j) = a.Elements(i) AndAlso b.Charges(j) = a.Charges(i) Then
                        lists(i).Add(j)
                    End If
                Next

                lists(i).Sort(Function(x, y) CandScore(x, ia, degA).CompareTo(CandScore(y, ia, degA)))
            Next

            Return lists
        End Function

        Private Function CandScore(bj As Integer, ia As String, degAi As Integer) As Long
            Dim s As Long = If(invB(bj) = ia, 0, 1000)
            s += Math.Abs(b.Degree(bj) - degAi) * 10L
            Return s * 100000L + bj
        End Function

    End Class

End Module
