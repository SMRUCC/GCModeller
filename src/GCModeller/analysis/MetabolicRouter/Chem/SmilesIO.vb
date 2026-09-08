' ============================================================================
' SmilesIO.vb — SMILES 子集解析与写出
' ----------------------------------------------------------------------------
' 解析子集：元素（双字母优先）、支链 ()、环闭合数字、键符 - = #、电价
'   [NH4+]/[O-]、显式氢 [NH2]、多组分 "."。芳香性小写不支持（用 Kekulé 式）。
' 写出：确定性 DFS（Morgan 秩排序邻居），支链括号化，环闭合数字——
'   roundtrip 性质：write→parse→同 MolKey（自检覆盖）。
' ============================================================================

Imports System.Text

Namespace RetroPath.Chem

    Public Module SmilesIO

        Private ReadOnly ElementsAlt As String() = {"Cl", "Br", "Si", "C", "N", "O", "S", "P", "F", "I", "B", "H"}

        ''' <summary>解析 SMILES 子集</summary>
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

        ''' <summary>确定性 SMILES 写出</summary>
        Public Function Write(m As Molecule) As String
            If m.NumAtoms() = 0 Then Return ""
            Dim writer As New SmilesWriter(m)
            Return writer.Write()
        End Function

        ''' <summary>写出器（封装递归 DFS 状态）</summary>
        Private Class SmilesWriter

            Private ReadOnly _m As Molecule
            Private ReadOnly _ranks As List(Of String)
            Private ReadOnly _sb As New StringBuilder()
            Private ReadOnly _compSet As HashSet(Of Int32)
            Private ReadOnly _emitted As New HashSet(Of Int32)()
            Private ReadOnly _digitFirst As New Dictionary(Of Int32, Tuple(Of Int32, Int32))()   ' 原子 → (digit, order)
            Private ReadOnly _digitSecond As New Dictionary(Of Int32, Tuple(Of Int32, Int32))()  ' 环闭合后端原子 → (digit, order)

            Public Sub New(m As Molecule)
                _m = m
                _ranks = m.MorganRanks()
                _compSet = New HashSet(Of Int32)(m.Components()(0))
            End Sub

            Public Function Write() As String
                Dim comps = _m.Components()
                Dim first = True
                For Each comp In comps
                    If Not first Then _sb.Append("."c)
                    first = False
                    _compSet.Clear()
                    For Each a In comp
                        _compSet.Add(a)
                    Next
                    WriteOneComponent(comp)
                Next
                Return _sb.ToString()
            End Function

            Private Sub WriteOneComponent(comp As List(Of Int32))
                _emitted.Clear()
                _digitFirst.Clear()
                _digitSecond.Clear()
                Dim start = comp.OrderBy(Function(a) _ranks(a), StringComparer.Ordinal).ThenBy(Function(a) a).First()
                ' 环闭合边探测（DFS 树回边）
                Dim ringEdges As New List(Of Tuple(Of Int32, Int32, Int32))()
                Dim dfsSeen As New HashSet(Of Int32)()
                ScanRings(start, -1, dfsSeen, ringEdges)
                Dim dg As Int32 = 1
                For Each re_ In ringEdges
                    _digitFirst(re_.Item1) = Tuple.Create(dg, re_.Item3)
                    _digitSecond(re_.Item2) = Tuple.Create(dg, re_.Item3)
                    dg += 1
                Next
                EmitDfs(start, -1, "")
            End Sub

            Private Sub ScanRings(a As Int32, parent As Int32, dfsSeen As HashSet(Of Int32),
                                  ringEdges As List(Of Tuple(Of Int32, Int32, Int32)))
                dfsSeen.Add(a)
                For Each nb In OrderedNeighbors(a)
                    Dim b = nb.Item1
                    If b = parent OrElse _compSet.Contains(b) = False Then Continue For
                    If dfsSeen.Contains(b) Then
                        Dim k1 = Math.Min(a, b)
                        Dim k2 = Math.Max(a, b)
                        If Not ringEdges.Any(Function(e) (e.Item1 = k1 AndAlso e.Item2 = k2)) Then
                            ringEdges.Add(Tuple.Create(k1, k2, nb.Item2))
                        End If
                    Else
                        ScanRings(b, a, dfsSeen, ringEdges)
                    End If
                Next
            End Sub

            Private Function OrderedNeighbors(a As Int32) As List(Of Tuple(Of Int32, Int32))
                Return _m.Neighbors(a).
                    Where(Function(t) _compSet.Contains(t.Item1)).
                    OrderBy(Function(t) _ranks(t.Item1), StringComparer.Ordinal).
                    ThenBy(Function(t) t.Item1).ToList()
            End Function

            Private Sub EmitDfs(a As Int32, parent As Int32, bondPrefix As String)
                _emitted.Add(a)
                _sb.Append(bondPrefix)
                _sb.Append(AtomSymbol(a))
                If _digitFirst.ContainsKey(a) Then
                    Dim dd = _digitFirst(a)
                    _sb.Append(BondChar(dd.Item2)).Append(dd.Item1.ToString())
                ElseIf _digitSecond.ContainsKey(a) Then
                    Dim dd = _digitSecond(a)
                    _sb.Append(BondChar(dd.Item2)).Append(dd.Item1.ToString())
                End If
                Dim cont As New List(Of Tuple(Of Int32, Int32))()
                For Each nb In OrderedNeighbors(a)
                    Dim b = nb.Item1
                    If b = parent Then Continue For
                    If _emitted.Contains(b) Then Continue For      ' 环后端：数字已在前端输出
                    cont.Add(nb)
                Next
                For bi = 0 To cont.Count - 2
                    _sb.Append("("c)
                    EmitDfs(cont(bi).Item1, a, BondChar(cont(bi).Item2))
                    _sb.Append(")"c)
                Next
                If cont.Count > 0 Then
                    EmitDfs(cont(cont.Count - 1).Item1, a, BondChar(cont(cont.Count - 1).Item2))
                End If
            End Sub

            Private Function AtomSymbol(a As Int32) As String
                Dim el = _m.Elements(a)
                Dim charge = _m.Charges(a)
                Dim eh = _m.ExplicitH(a)
                If charge = 0 AndAlso eh = 0 Then Return el
                Dim sb As New StringBuilder()
                sb.Append("["c).Append(el)
                If eh > 0 Then
                    sb.Append("H")
                    If eh > 1 Then sb.Append(eh.ToString())
                End If
                If charge > 0 Then
                    sb.Append("+")
                    If charge > 1 Then sb.Append(charge.ToString())
                ElseIf charge < 0 Then
                    sb.Append("-")
                    If charge < -1 Then sb.Append((-charge).ToString())
                End If
                sb.Append("]")
                Return sb.ToString()
            End Function

            Private Shared Function BondChar(order As Int32) As String
                Select Case order
                    Case 2 : Return "="
                    Case 3 : Return "#"
                    Case Else : Return ""
                End Select
            End Function

        End Class

    End Module

End Namespace
