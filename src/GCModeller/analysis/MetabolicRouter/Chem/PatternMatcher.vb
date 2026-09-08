' ============================================================================
' PatternMatcher.vb — SMARTS 子集模式解析 + 子图单射匹配（回溯）
' ----------------------------------------------------------------------------
' 模式原子语法: [元素(Hn)(Dn)(!O)(:类)]，例 [C] [CH2] [CD3H0!O:1] [O-] [NH2:3]
'   Hn  = 总氢数约束（显式+隐式）；Dn = 重原子度精确约束；
'   !O  = 否定约束（无单键 OH 邻居，用于区分酮 C 与羧基 C）；
'   :n  = 类号（SMIRKS 原子映射货币，规则两侧由类号对应）。
' 键约束: - 单键 = 双键 # 三键 ~ 任意（默认单键）。
' 匹配语义：仅匹配"有键类"（模式中的孤立组分不参与匹配——它们是生成/离去
'   辅底物模板 [readme.md §2 规则泛化]）；匹配 = 单射 + 原子约束 + 键级约束。
' ============================================================================

Namespace RetroPath.Chem

    Public Class PatternAtom

        Public Element As String            ' Nothing = 通配（本库未用）
        Public Charge As Int32 = -999       ' -999 = 任意
        Public HCount As Int32 = -999       ' -999 = 任意
        Public Degree As Int32 = -999       ' -999 = 任意
        Public NoOhNeighbor As Boolean = False
        Public Cls As Int32 = -1

    End Class

    Public Class Pattern

        Public Atoms As New List(Of PatternAtom)()
        ''' <summary>
        ''' (a, b, order；0=任意)
        ''' </summary>
        Public Bonds As New List(Of Tuple(Of Int32, Int32, Int32))()
        Public NoOh As New List(Of Boolean)()

    End Class

    Public Module PatternMatcher

        ''' <summary>解析模式字符串</summary>
        Public Function ParsePattern(s As String) As Pattern
            Dim p As New Pattern()
            Dim stack As New Stack(Of Int32)()
            Dim prev As Int32 = -1
            Dim pending As Int32 = -1        ' -1 = 未指定（默认单键）
            Dim i As Int32 = 0
            While i < s.Length
                Dim c = s(i)
                If c = "("c Then
                    stack.Push(prev)
                    i += 1
                ElseIf c = ")"c Then
                    If stack.Count = 0 Then Throw New ArgumentException("模式括号不匹配")
                    prev = stack.Pop()
                    i += 1
                ElseIf c = "-"c OrElse c = "="c OrElse c = "#"c OrElse c = "~"c Then
                    pending = If(c = "-"c, 1, If(c = "="c, 2, If(c = "#"c, 3, 0)))
                    i += 1
                ElseIf c = "."c Then
                    prev = -1                ' 多组分：不与前一原子成键
                    i += 1
                ElseIf c = "["c Then
                    Dim closeIdx = s.IndexOf("]"c, i)
                    If closeIdx < 0 Then Throw New ArgumentException("模式括号未闭合")
                    Dim body = s.Substring(i + 1, closeIdx - i - 1)
                    Dim pa = ParsePatternAtom(body)
                    p.Atoms.Add(pa)
                    p.NoOh.Add(pa.NoOhNeighbor)
                    Dim idx = p.Atoms.Count - 1
                    If prev >= 0 Then
                        p.Bonds.Add(Tuple.Create(prev, idx, If(pending >= 0, pending, 1)))
                    End If
                    pending = -1
                    prev = idx
                    i = closeIdx + 1
                Else
                    Throw New ArgumentException($"模式解析失败 @ {i}: {s}")
                End If
            End While
            Return p
        End Function

        Private Function ParsePatternAtom(body As String) As PatternAtom
            Dim pa As New PatternAtom()
            Dim core = body
            Dim ci = core.LastIndexOf(":"c)
            If ci >= 0 Then
                pa.Cls = Int32.Parse(core.Substring(ci + 1))
                core = core.Substring(0, ci)
            End If
            Dim pos = 0
            Dim twoLetter() As String = {"Cl", "Br", "Si"}
            For Each cand In twoLetter
                If core.Length - pos >= cand.Length AndAlso core.Substring(pos, cand.Length) = cand Then
                    pa.Element = cand
                    pos += cand.Length
                    Exit For
                End If
            Next
            If pa.Element Is Nothing AndAlso pos < core.Length Then
                Dim c1 = core(pos)
                If "CNOSPFIBH".IndexOf(c1) >= 0 Then
                    pa.Element = c1.ToString()
                    pos += 1
                Else
                    Throw New ArgumentException($"模式元素解析失败: {body}")
                End If
            End If
            While pos < core.Length
                If core(pos) = "H"c AndAlso pos + 1 < core.Length AndAlso Char.IsDigit(core(pos + 1)) Then
                    pa.HCount = Int32.Parse(core(pos + 1).ToString())
                    pos += 2
                ElseIf core(pos) = "D"c AndAlso pos + 1 < core.Length AndAlso Char.IsDigit(core(pos + 1)) Then
                    pa.Degree = Int32.Parse(core(pos + 1).ToString())
                    pos += 2
                ElseIf core(pos) = "!"c AndAlso pos + 1 < core.Length AndAlso core(pos + 1) = "O"c Then
                    pa.NoOhNeighbor = True
                    pos += 2
                Else
                    Throw New ArgumentException($"模式旗标解析失败: {body} @ {pos}")
                End If
            End While
            Return pa
        End Function

        ''' <summary>
        ''' 子图单射匹配（回溯枚举，上限 limit）。返回 [{类号 → 分子原子}]（去重）。
        ''' </summary>
        Public Function Match(m As Molecule, pat As Pattern, Optional limit As Int32 = 100) As List(Of Dictionary(Of Int32, Int32))
            Dim matcher As New MatcherState(m, pat, limit)
            matcher.Run()
            Return matcher.Results
        End Function

        ''' <summary>匹配器状态（封装递归回溯）</summary>
        Private Class MatcherState

            Private ReadOnly _m As Molecule
            Private ReadOnly _pat As Pattern
            Private ReadOnly _limit As Int32
            Private ReadOnly _order As New List(Of Int32)()
            Private ReadOnly _cand As New Dictionary(Of Int32, List(Of Int32))()
            Private ReadOnly _used As New HashSet(Of Int32)()
            Private ReadOnly _assign As New Dictionary(Of Int32, Int32)()
            Public ReadOnly Results As New List(Of Dictionary(Of Int32, Int32))()

            Public Sub New(m As Molecule, pat As Pattern, limit As Int32)
                _m = m
                _pat = pat
                _limit = limit
            End Sub

            Public Sub Run()
                If _pat.Atoms.Count = 0 Then Return
                BuildOrderAndCandidates()
                If _order.Count = 0 Then Return
                Backtrack(0)
                Dedup()
            End Sub

            Private Sub BuildOrderAndCandidates()
                ' 匹配类 = 有键类；无键模式 → 全部类
                Dim bondedCls As New HashSet(Of Int32)()
                For Each b In _pat.Bonds
                    bondedCls.Add(_pat.Atoms(b.Item1).Cls)
                    bondedCls.Add(_pat.Atoms(b.Item2).Cls)
                Next
                If bondedCls.Count = 0 Then
                    For Each a In _pat.Atoms
                        bondedCls.Add(a.Cls)
                    Next
                End If
                ' 匹配子图邻接
                Dim adj As New Dictionary(Of Int32, List(Of Int32))()
                For Each b In _pat.Bonds
                    Dim cx = _pat.Atoms(b.Item1).Cls
                    Dim cy = _pat.Atoms(b.Item2).Cls
                    If bondedCls.Contains(cx) AndAlso bondedCls.Contains(cy) Then
                        If Not adj.ContainsKey(b.Item1) Then adj(b.Item1) = New List(Of Int32)()
                        If Not adj.ContainsKey(b.Item2) Then adj(b.Item2) = New List(Of Int32)()
                        adj(b.Item1).Add(b.Item2)
                        adj(b.Item2).Add(b.Item1)
                    End If
                Next
                ' 连通序（DFS，栈实现）
                Dim matchIdx As New List(Of Int32)()
                For i = 0 To _pat.Atoms.Count - 1
                    If bondedCls.Contains(_pat.Atoms(i).Cls) Then matchIdx.Add(i)
                Next
                Dim seen As New HashSet(Of Int32)()
                Dim stack As New Stack(Of Int32)()
                For Each root In matchIdx
                    If seen.Contains(root) Then Continue For
                    stack.Push(root)
                    While stack.Count > 0
                        Dim x = stack.Pop()
                        If seen.Contains(x) Then Continue For
                        seen.Add(x)
                        _order.Add(x)
                        If adj.ContainsKey(x) Then
                            For Each nb In adj(x)
                                If Not seen.Contains(nb) Then stack.Push(nb)
                            Next
                        End If
                    End While
                Next
                ' 候选
                For Each pai In _order
                    Dim lst As New List(Of Int32)()
                    For ma = 0 To _m.NumAtoms() - 1
                        If OkAtom(ma, pai) Then lst.Add(ma)
                    Next
                    _cand(pai) = lst
                Next
            End Sub

            Private Function OkAtom(ma As Int32, pai As Int32) As Boolean
                Dim pa = _pat.Atoms(pai)
                If pa.Element IsNot Nothing AndAlso _m.Elements(ma) <> pa.Element Then Return False
                If pa.Charge <> -999 AndAlso _m.Charges(ma) <> pa.Charge Then Return False
                If pa.HCount <> -999 AndAlso _m.TotalH(ma) <> pa.HCount Then Return False
                If pa.Degree <> -999 AndAlso _m.Degree(ma) <> pa.Degree Then Return False
                If pa.NoOhNeighbor Then
                    For Each nb In _m.Neighbors(ma)
                        If nb.Item2 = 1 AndAlso _m.Elements(nb.Item1) = "O" AndAlso _m.TotalH(nb.Item1) >= 1 Then
                            Return False
                        End If
                    Next
                End If
                Return True
            End Function

            Private Sub Backtrack(k As Int32)
                If Results.Count >= _limit Then Return
                If k = _order.Count Then
                    Dim cm As New Dictionary(Of Int32, Int32)()
                    For Each kvp In _assign
                        cm(_pat.Atoms(kvp.Key).Cls) = kvp.Value
                    Next
                    Results.Add(cm)
                    Return
                End If
                Dim pai = _order(k)
                For Each ma In _cand(pai)
                    If _used.Contains(ma) Then Continue For
                    If Not BondsConsistent(pai, ma) Then Continue For
                    _used.Add(ma)
                    _assign(pai) = ma
                    Backtrack(k + 1)
                    _used.Remove(ma)
                    _assign.Remove(pai)
                Next
            End Sub

            Private Function BondsConsistent(pai As Int32, ma As Int32) As Boolean
                For Each b In _pat.Bonds
                    Dim other As Int32 = -1
                    If b.Item2 = pai AndAlso _assign.ContainsKey(b.Item1) Then
                        other = _assign(b.Item1)
                    ElseIf b.Item1 = pai AndAlso _assign.ContainsKey(b.Item2) Then
                        other = _assign(b.Item2)
                    Else
                        Continue For
                    End If
                    Dim bo = _m.BondOrder(other, ma)
                    If b.Item3 = 0 Then
                        If bo = 0 Then Return False
                    ElseIf bo <> b.Item3 Then
                        Return False
                    End If
                Next
                Return True
            End Function

            Private Sub Dedup()
                Dim added As New HashSet(Of String)()
                Dim uniq As New List(Of Dictionary(Of Int32, Int32))()
                For Each cm In Results
                    Dim key = KeyOf(cm)
                    If Not added.Contains(key) Then
                        added.Add(key)
                        uniq.Add(cm)
                    End If
                Next
                Results.Clear()
                Results.AddRange(uniq)
            End Sub

            Private Shared Function KeyOf(cm As Dictionary(Of Int32, Int32)) As String
                Return String.Join(";", cm.OrderBy(Function(k) k.Key).Select(Function(k) $"{k.Key}:{cm(k.Key)}"))
            End Function

        End Class

    End Module

End Namespace
