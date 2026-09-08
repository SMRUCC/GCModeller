Imports System.Text

Namespace Chem

    ''' <summary>写出器（封装递归 DFS 状态）</summary>
    Public Class SmilesWriter

        Private ReadOnly _m As Molecule
        Private ReadOnly _ranks As List(Of String)
        Private ReadOnly _sb As New StringBuilder()
        Private ReadOnly _compSet As HashSet(Of Int32)
        Private ReadOnly _emitted As New HashSet(Of Int32)()
        ''' <summary>原子 → 该原子上待输出的环闭合号列表 [(digit, order)]；回边两端登记同一 digit</summary>
        Private ReadOnly _ringDigits As New Dictionary(Of Int32, List(Of Tuple(Of Int32, Int32)))()
        ''' <summary>DFS 回边端点对 (min, max)：写出时不得作为树边遍历，仅由环号表达</summary>
        Private ReadOnly _ringEdgeKeys As New HashSet(Of (Integer, Integer))()

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
            _ringDigits.Clear()
            _ringEdgeKeys.Clear()
            Dim start = comp.OrderBy(Function(a) _ranks(a), StringComparer.Ordinal).ThenBy(Function(a) a).First()
            ' 环闭合边探测（DFS 树回边）
            Dim ringEdges As New List(Of Tuple(Of Int32, Int32, Int32))()
            Dim dfsSeen As New HashSet(Of Int32)()
            ScanRings(start, -1, dfsSeen, ringEdges)
            Dim dg As Int32 = 1
            For Each re_ In ringEdges
                AppendRingDigit(re_.Item1, dg, re_.Item3)
                AppendRingDigit(re_.Item2, dg, re_.Item3)
                _ringEdgeKeys.Add((re_.Item1, re_.Item2))
                dg += 1
            Next
            EmitDfs(start, -1, "")
        End Sub

        Private Sub AppendRingDigit(atom As Int32, digit As Int32, order As Int32)
            If Not _ringDigits.ContainsKey(atom) Then
                _ringDigits(atom) = New List(Of Tuple(Of Int32, Int32))()
            End If
            _ringDigits(atom).Add(Tuple.Create(digit, order))
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
            If _ringDigits.ContainsKey(a) Then
                For Each dd In _ringDigits(a)
                    _sb.Append(BondChar(dd.Item2)).Append(dd.Item1.ToString())
                Next
            End If
            Dim cont As New List(Of Tuple(Of Int32, Int32))()
            For Each nb In OrderedNeighbors(a)
                Dim b = nb.Item1
                If b = parent Then Continue For
                If _emitted.Contains(b) Then Continue For
                ' 环闭合回边：不作为树边展开，避免同原子被重复发射导致环号无法配对
                If _ringEdgeKeys.Contains((Math.Min(a, b), Math.Max(a, b))) Then Continue For
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

End Namespace