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

Namespace Chem

    ''' <summary>
    ''' 模式中的一个原子（SMARTS 子集的一个原子约束）。
    ''' </summary>
    ''' <remarks>
    ''' 语法 <c>[元素(Hn)(Dn)(!O)(!=O)(+/-)(:类号)]</c>，例 <c>[C]</c>、<c>[CH2]</c>、
    ''' <c>[CD3H0!O:1]</c>、<c>[O-]</c>。除类号外各项都是可选约束，取哨兵值表示不约束。
    ''' </remarks>
    Public Class PatternAtom

        ''' <summary>
        ''' 元素符号；Nothing 表示通配（本规则库未使用通配原子）。
        ''' </summary>
        Public Element As String            ' Nothing = 通配（本库未用）

        ''' <summary>
        ''' 形式电荷约束；-999 表示"任意电荷"。
        ''' </summary>
        Public Charge As Int32 = -999       ' -999 = 任意

        ''' <summary>
        ''' 氢总数（显式 + 隐式）约束；-999 表示"不约束氢数"。
        ''' </summary>
        Public HCount As Int32 = -999       ' -999 = 任意

        ''' <summary>
        ''' 重原子度约束；-999 表示"不约束度数"。
        ''' </summary>
        Public Degree As Int32 = -999       ' -999 = 任意

        ''' <summary>
        ''' !O：要求该原子没有"以单键相连的羟基氧"邻居（用于区分酮/醛碳与羧基碳）。
        ''' </summary>
        Public NoOhNeighbor As Boolean = False

        ''' <summary>
        ''' !=O：要求该原子没有"以双键相连的羰基氧"邻居（用于排除羧基碳、羰基碳）。
        ''' </summary>
        Public NoOxoNeighbor As Boolean = False

        ''' <summary>
        ''' 原子类号（SMIRKS 的原子映射"货币"）：规则两侧同一类号代表同一个原子。
        ''' </summary>
        Public Cls As Int32 = -1

    End Class

    ''' <summary>
    ''' 一个完整的子结构模式（规则的一侧）：模式原子 + 模式内键 + 逐原子的 !O 标记。
    ''' </summary>
    Public Class Pattern

        ''' <summary>
        ''' 模式原子列表，下标即模式内索引。
        ''' </summary>
        Public Atoms As New List(Of PatternAtom)()

        ''' <summary>
        ''' 模式内的键 (a, b, order)；order 为 0 表示"任意键级"（<c>~</c>）。
        ''' </summary>
        Public Bonds As New List(Of Bond)()

        ''' <summary>
        ''' 与 <see cref="Atoms"/> 逐项对应的 !O 标记缓存。
        ''' </summary>
        Public NoOh As New List(Of Boolean)()

    End Class

    ''' <summary>
    ''' SMARTS 子集的模式解析与子图单射匹配。
    ''' </summary>
    ''' <remarks>
    ''' 关键语义约定：只有"有键的原子"参与匹配。模式中不带任何键的独立组分不会被匹配，
    ''' 而是在规则应用时被当作"生成/离去的辅底物模板"。因此构造模式时，凡需要参与匹配的
    ''' 原子都必须在模式里至少有一条键。
    ''' </remarks>
    Public Module PatternMatcher

        ''' <summary>
        ''' 解析模式字符串，构造 <see cref="Pattern"/>。
        ''' </summary>
        ''' <param name="s">
        ''' 模式串，语法见 <see cref="PatternAtom"/>；键符支持 <c>-</c> 单键、<c>=</c> 双键、
        ''' <c>#</c> 三键、<c>~</c> 任意，默认单键；<c>.</c> 用作多组分分隔。
        ''' </param>
        ''' <returns>解析得到的模式对象。</returns>
        ''' <exception cref="ArgumentException">括号不匹配/未闭合，或元素、旗标无法解析时抛出。</exception>
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
                        p.Bonds.Add((prev, idx, If(pending >= 0, pending, 1)))
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
                ElseIf core(pos) = "!"c AndAlso pos + 2 < core.Length AndAlso
                       core(pos + 1) = "="c AndAlso core(pos + 2) = "O"c Then
                    pa.NoOxoNeighbor = True
                    pos += 3
                ElseIf core(pos) = "!"c AndAlso pos + 1 < core.Length AndAlso core(pos + 1) = "O"c Then
                    pa.NoOhNeighbor = True
                    pos += 2
                ElseIf core(pos) = "+"c OrElse core(pos) = "-"c Then
                    ' 电荷旗标：+ / -，可带数字（+2 / -2）；单独符号视为 ±1
                    Dim sign As Int32 = If(core(pos) = "+"c, 1, -1)
                    pos += 1
                    Dim mag As Int32 = 0
                    Dim hasMag As Boolean = False
                    While pos < core.Length AndAlso Char.IsDigit(core(pos))
                        mag = mag * 10 + (AscW(core(pos)) - AscW("0"c))
                        hasMag = True
                        pos += 1
                    End While
                    pa.Charge = sign * If(hasMag, mag, 1)
                Else
                    Throw New ArgumentException($"模式旗标解析失败: {body} @ {pos}")
                End If
            End While
            Return pa
        End Function

        ''' <summary>
        ''' 在分子中枚举模式的全部子图单射匹配（回溯搜索，最多 <paramref name="limit"/> 个）。
        ''' </summary>
        ''' <param name="m">被搜索的目标分子。</param>
        ''' <param name="pat">待匹配的模式。</param>
        ''' <param name="limit">返回结果数量上限，用于控制组合爆炸。</param>
        ''' <returns>
        ''' 匹配列表，每项为一个"类号 → 分子原子索引"的字典，已按内容去重。
        ''' 未命中时返回空列表。
        ''' </returns>
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
                    bondedCls.Add(_pat.Atoms(b.a).Cls)
                    bondedCls.Add(_pat.Atoms(b.b).Cls)
                Next
                If bondedCls.Count = 0 Then
                    For Each a In _pat.Atoms
                        bondedCls.Add(a.Cls)
                    Next
                End If
                ' 匹配子图邻接
                Dim adj As New Dictionary(Of Int32, List(Of Int32))()
                For Each b In _pat.Bonds
                    Dim cx = _pat.Atoms(b.a).Cls
                    Dim cy = _pat.Atoms(b.b).Cls
                    If bondedCls.Contains(cx) AndAlso bondedCls.Contains(cy) Then
                        If Not adj.ContainsKey(b.a) Then adj(b.a) = New List(Of Int32)()
                        If Not adj.ContainsKey(b.b) Then adj(b.b) = New List(Of Int32)()
                        adj(b.a).Add(b.b)
                        adj(b.b).Add(b.a)
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
                If pa.NoOxoNeighbor Then
                    For Each nb In _m.Neighbors(ma)
                        If nb.Item2 = 2 AndAlso _m.Elements(nb.Item1) = "O" Then
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
                    If b.b = pai AndAlso _assign.ContainsKey(b.a) Then
                        other = _assign(b.a)
                    ElseIf b.a = pai AndAlso _assign.ContainsKey(b.b) Then
                        other = _assign(b.b)
                    Else
                        Continue For
                    End If
                    Dim bo = _m.BondOrder(other, ma)
                    If b.order = 0 Then
                        If bo = 0 Then Return False
                    ElseIf bo <> b.order Then
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
