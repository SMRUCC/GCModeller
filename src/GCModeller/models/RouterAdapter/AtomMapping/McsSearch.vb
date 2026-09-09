
Imports SMRUCC.genomics.Analysis.RetroPath.Chem

''' <summary>键级保守的诱导式最大公共子图搜索（贪心下界 + 带预算回溯）</summary>
Friend Class McsSearch

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
    Private bestChanged As Integer = 0
    Private curChanged As Integer = 0
    Private ReadOnly maxChanged As Integer
    Private nodes As Integer = 0
    Private stopped As Boolean = False

    Sub New(a As Molecule, b As Molecule, budget As Integer, maxUnmapped As Integer,
            Optional maxChangedBonds As Integer = 4)
        Me.a = a
        Me.b = b
        Me.nA = a.NumAtoms()
        Me.nB = b.NumAtoms()
        Me.budget = budget
        Me.maxChanged = Math.Max(0, maxChangedBonds)
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

    ''' <summary>贪心：优先取"不引入键变化"的配对，其次取变化最少的，得到下界</summary>
    Private Function Greedy() As List(Of (Integer, Integer))
        ResetMaps()
        Dim res As New List(Of (Integer, Integer))()
        Dim total As Integer = 0

        For Each ai As Integer In order
            Dim bestBj As Integer = -1
            Dim bestDc As Integer = maxChanged + 1

            For Each bj As Integer In cands(ai)
                If mapBA(bj) >= 0 Then Continue For

                Dim dc As Integer = ChangeCount(ai, bj, res)
                If dc < 0 Then Continue For

                If dc < bestDc Then
                    bestDc = dc
                    bestBj = bj
                    ' 完全一致是常态，尽早退出避免全量扫描
                    If dc = 0 Then Exit For
                End If
            Next

            If bestBj < 0 Then Continue For

            mapAB(ai) = bestBj
            mapBA(bestBj) = ai
            res.Add((ai, bestBj))
            total += bestDc
        Next

        bestChanged = total
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
            If cur.Count > best.Count OrElse
               (cur.Count = best.Count AndAlso curChanged < bestChanged) Then
                best = New List(Of (Integer, Integer))(cur)
                bestChanged = curChanged
                If best.Count >= target AndAlso bestChanged = 0 Then stopped = True
            End If
            Return
        End If

        Dim ai As Integer = order(k)

        ' 分支一：把 ai 映射到某个兼容的产物原子
        For Each bj As Integer In cands(ai)
            If mapBA(bj) >= 0 Then Continue For

            Dim dc As Integer = ChangeCount(ai, bj, cur)
            If dc < 0 Then Continue For

            mapAB(ai) = bj
            mapBA(bj) = ai
            cur.Add((ai, bj))
            curChanged += dc

            Dfs(k + 1)

            curChanged -= dc
            cur.RemoveAt(cur.Count - 1)
            mapBA(bj) = -1
            mapAB(ai) = -1

            If stopped Then Return
        Next

        ' 分支二：ai 不映射（即它属于反应中心/离去基团）
        Dfs(k + 1)
    End Sub

    ''' <summary>
    ''' 计算把 (ai,bj) 加入映射会引入多少处"键不一致"（键级改变 / 断键 / 成键）；
    ''' 返回 -1 表示超过容忍上限，该配对不可用。
    '''
    ''' 与严格的诱导式 MCS（要求键完全一致）不同，这里允许少量键发生变化：反应中心
    ''' 本来就靠键级/断键/成键来定义，若强制完全一致，反应中心处的原子会被挤出映射，
    ''' 规则就会退化成"删掉一个原子 / 凭空造一个原子"，逆推时产生游离碎片（实测会把
    ''' 分支酸逆推成一堆碎片，路径永远无法收敛到汇集合）。
    ''' </summary>
    Private Function ChangeCount(ai As Integer, bj As Integer, mapped As List(Of (Integer, Integer))) As Integer
        Dim n As Integer = 0
        For Each pr In mapped
            If ordA(ai, pr.Item1) <> ordB(bj, pr.Item2) Then
                n += 1
                If n > maxChanged Then Return -1
            End If
        Next
        Return n
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
