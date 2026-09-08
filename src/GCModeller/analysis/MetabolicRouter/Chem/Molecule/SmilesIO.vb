' ============================================================================
' SmilesIO.vb — SMILES 子集解析与写出
' ----------------------------------------------------------------------------
' 解析子集：元素（双字母优先）、支链 ()、环闭合数字、键符 - = #、电价
'   [NH4+]/[O-]、显式氢 [NH2]、多组分 "."。芳香性小写不支持（用 Kekulé 式）。
' 写出：确定性 DFS（Morgan 秩排序邻居），支链括号化，环闭合数字——
'   roundtrip 性质：write→parse→同 MolKey（自检覆盖）。
' ============================================================================

Namespace Chem

    ''' <summary>
    ''' SMILES 的输入/输出：一个"够用即可"的 SMILES 子集解析与确定性写出实现。
    ''' </summary>
    ''' <remarks>
    ''' 解析支持：元素（双字母优先）、支链 <c>()</c>、环闭合数字、键符 <c>- = #</c>、
    ''' 电价（<c>[NH4+]</c>、<c>[O-]</c>）、显式氢（<c>[NH2]</c>）、多组分 <c>.</c>。
    ''' 不支持：芳香小写记号（请改用 Kekulé 式）、<c>@/@@</c> 与 <c>/ \</c> 立体标记、
    ''' <c>%nn</c> 多位环号、通配原子 <c>*</c>；元素表仅含
    ''' Cl、Br、Si、C、N、O、S、P、F、I、B、H。
    ''' 写出具备 roundtrip 性质：write → parse → 同一 <see cref="Molecule.MolKey"/>。
    ''' </remarks>
    Public Module SmilesIO

        ''' <summary>
        ''' 可解析的元素符号表；双字母元素排在单字母之前，保证 "Cl"/"Br"/"Si" 优先匹配。
        ''' </summary>
        Private ReadOnly ElementsAlt As String() = {"Cl", "Br", "Si", "C", "N", "O", "S", "P", "F", "I", "B", "H"}

        ''' <summary>
        ''' 解析 SMILES 字符串，构造对应的分子图。
        ''' </summary>
        ''' <param name="smiles">待解析的 SMILES（须落在本模块支持的子集内）。</param>
        ''' <returns>解析得到的 <see cref="Molecule"/>。</returns>
        ''' <exception cref="ArgumentException">
        ''' SMILES 括号不匹配、环号未闭合、或遇到无法识别的元素/记号时抛出。
        ''' </exception>
        Public Function Parse(smiles As String) As Molecule
            Dim m As New Molecule()
            Dim stack As New Stack(Of Int32)()
            Dim prev As Int32 = -1
            Dim pendingOrder As Int32 = 1
            Dim ringOpen As New Dictionary(Of String, Tuple(Of Int32, Int32))()
            Dim i As Int32 = 0
            Dim n = smiles.Length

            While i < n
                Dim c = smiles(i)
                If c = "("c Then
                    stack.Push(prev)
                    i += 1
                ElseIf c = ")"c Then
                    If stack.Count = 0 Then Throw New ArgumentException("SMILES 括号不匹配")
                    prev = stack.Pop()
                    i += 1
                ElseIf c = "-"c OrElse c = "="c OrElse c = "#"c Then
                    pendingOrder = If(c = "-"c, 1, If(c = "="c, 2, 3))
                    i += 1
                ElseIf Char.IsDigit(c) Then
                    Dim d = c.ToString()
                    i += 1
                    If ringOpen.ContainsKey(d) Then
                        Dim opened = ringOpen(d)
                        ringOpen.Remove(d)
                        Dim order = If(pendingOrder > 1, pendingOrder, opened.Item2)
                        m.Bonds.Add((opened.Item1, prev, order))
                        pendingOrder = 1
                    Else
                        ringOpen(d) = Tuple.Create(prev, If(pendingOrder > 1, pendingOrder, 1))
                        pendingOrder = 1
                    End If
                ElseIf c = "."c Then
                    prev = -1
                    i += 1
                ElseIf c = "["c Then
                    Dim closeIdx = smiles.IndexOf("]"c, i)
                    If closeIdx < 0 Then Throw New ArgumentException("SMILES 括号未闭合")
                    Dim body = smiles.Substring(i + 1, closeIdx - i - 1)
                    Dim el As String = Nothing
                    Dim eh As Int32 = 0
                    Dim charge As Int32 = 0
                    ParseBracket(body, el, eh, charge)
                    Dim idx = m.AddAtom(el, charge)
                    m.ExplicitH(idx) = eh
                    If prev >= 0 Then m.Bonds.Add((prev, idx, pendingOrder))
                    pendingOrder = 1
                    prev = idx
                    i = closeIdx + 1
                Else
                    Dim el = MatchElement(smiles, i)
                    If el Is Nothing Then Throw New ArgumentException($"SMILES 解析失败 @ {i}: {smiles}")
                    Dim idx = m.AddAtom(el, 0)
                    If prev >= 0 Then m.Bonds.Add((prev, idx, pendingOrder))
                    pendingOrder = 1
                    prev = idx
                    i += el.Length
                End If
            End While
            If ringOpen.Count > 0 Then Throw New ArgumentException("SMILES 环未闭合")
            Return m
        End Function

        Private Sub ParseBracket(body As String, ByRef el As String, ByRef eh As Int32, ByRef charge As Int32)
            ' [元素][Hn?][电荷?]；例 N, NH2, NH4+, O-, P+3
            el = Nothing
            eh = 0
            charge = 0
            Dim pos = 0
            For Each cand In ElementsAlt
                If body.Length - pos >= cand.Length AndAlso body.Substring(pos, cand.Length) = cand Then
                    el = cand
                    pos += cand.Length
                    Exit For
                End If
            Next
            If el Is Nothing Then Throw New ArgumentException($"括号原子解析失败: {body}")
            If pos < body.Length AndAlso body(pos) = "H"c Then
                pos += 1
                If pos < body.Length AndAlso Char.IsDigit(body(pos)) Then
                    eh = Int32.Parse(body(pos).ToString())
                    pos += 1
                Else
                    eh = 1
                End If
            End If
            If pos < body.Length Then
                Dim rest = body.Substring(pos)
                If rest = "+" Then
                    charge = 1
                ElseIf rest = "-" Then
                    charge = -1
                ElseIf rest.Length >= 2 AndAlso (rest(0) = "+"c OrElse rest(0) = "-"c) AndAlso
                       rest.Substring(1).All(Function(ch) Char.IsDigit(ch)) Then
                    charge = Int32.Parse(rest.Substring(1))
                    If rest(0) = "-"c Then charge = -charge
                End If
            End If
        End Sub

        Private Function MatchElement(s As String, pos As Int32) As String
            For Each cand In ElementsAlt
                If pos + cand.Length <= s.Length AndAlso s.Substring(pos, cand.Length) = cand Then
                    Return cand
                End If
            Next
            Return Nothing
        End Function

        ''' <summary>
        ''' 以确定性 DFS（邻居按 Morgan 秩排序）写出 SMILES：支链括号化、环用数字闭合。
        ''' </summary>
        ''' <param name="m">待写出的分子；多组分分子各分量以 <c>.</c> 分隔。</param>
        ''' <returns>规范 SMILES 字符串；空分子返回空串。</returns>
        ''' <remarks>
        ''' 同一分子（含同构的不同写法）总是得到同一输出，因此可作为结构比较与去重的依据。
        ''' </remarks>
        Public Function Write(m As Molecule) As String
            If m.NumAtoms() = 0 Then Return ""
            Dim writer As New SmilesWriter(m)
            Return writer.Write()
        End Function

    End Module

End Namespace
