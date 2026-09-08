Imports System.Text

Namespace Chem

    ''' <summary>写出器（封装递归 DFS 状态）</summary>
    Public Class SmilesWriter

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

End Namespace