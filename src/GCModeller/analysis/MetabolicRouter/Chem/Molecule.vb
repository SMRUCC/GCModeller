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

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace RetroPath.Chem

    Public Class Molecule

        Public Elements As List(Of String)
        Public Charges As List(Of Int32)
        Public ExplicitH As List(Of Int32)
        Public Bonds As List(Of Tuple(Of Int32, Int32, Int32))    ' (a, b, order 1/2/3)

        Public Sub New()
            Elements = New List(Of String)()
            Charges = New List(Of Int32)()
            ExplicitH = New List(Of Int32)()
            Bonds = New List(Of Tuple(Of Int32, Int32, Int32))()
        End Sub

        Public Function NumAtoms() As Int32
            Return Elements.Count
        End Function

        ''' <summary>邻居列表 (atom, bondOrder)</summary>
        Public Function Neighbors(a As Int32) As List(Of Tuple(Of Int32, Int32))
            Dim outList As New List(Of Tuple(Of Int32, Int32))()
            For Each b In Bonds
                If b.Item1 = a Then
                    outList.Add(Tuple.Create(b.Item2, b.Item3))
                ElseIf b.Item2 = a Then
                    outList.Add(Tuple.Create(b.Item1, b.Item3))
                End If
            Next
            Return outList
        End Function

        Public Function Degree(a As Int32) As Int32
            Return Neighbors(a).Count
        End Function

        Public Function BondOrder(a As Int32, b As Int32) As Int32
            For Each bd In Bonds
                If (bd.Item1 = a AndAlso bd.Item2 = b) OrElse (bd.Item1 = b AndAlso bd.Item2 = a) Then
                    Return bd.Item3
                End If
            Next
            Return 0
        End Function

        ''' <summary>设置键级（须已存在）</summary>
        Public Sub SetBondOrder(a As Int32, b As Int32, newOrder As Int32)
            For i = 0 To Bonds.Count - 1
                Dim bd = Bonds(i)
                If (bd.Item1 = a AndAlso bd.Item2 = b) OrElse (bd.Item1 = b AndAlso bd.Item2 = a) Then
                    Bonds(i) = Tuple.Create(bd.Item1, bd.Item2, newOrder)
                    Return
                End If
            Next
        End Sub

        Public Sub RemoveBond(a As Int32, b As Int32)
            Bonds = Bonds.Where(Function(bd) Not ((bd.Item1 = a AndAlso bd.Item2 = b) OrElse
                                                   (bd.Item1 = b AndAlso bd.Item2 = a))).ToList()
        End Sub

        Public Function AddAtom(el As String, charge As Int32) As Int32
            Elements.Add(el)
            Charges.Add(charge)
            ExplicitH.Add(0)
            Return Elements.Count - 1
        End Function

        Public Function Copy() As Molecule
            Dim m As New Molecule()
            m.Elements = New List(Of String)(Elements)
            m.Charges = New List(Of Int32)(Charges)
            m.ExplicitH = New List(Of Int32)(ExplicitH)
            m.Bonds = Bonds.ToList()
            Return m
        End Function

        Public Function ValenceOf(el As String, charge As Int32) As Int32
            Dim v As Int32
            Select Case el
                Case "C" : v = 4
                Case "N" : v = 3
                Case "O" : v = 2
                Case "S" : v = 6
                Case "P" : v = 5
                Case "F", "Cl", "Br", "I" : v = 1
                Case "B" : v = 3
                Case "H" : v = 1
                Case Else : v = 4
            End Select
            If el = "N" Then
                If charge > 0 Then
                    v = 4
                ElseIf charge < 0 Then
                    v = 2
                End If
            ElseIf el = "O" Then
                If charge > 0 Then
                    v = 3
                ElseIf charge < 0 Then
                    v = 1
                End If
            ElseIf el = "C" AndAlso charge <> 0 Then
                v = 3
            End If
            Return v
        End Function

        Public Function ImplicitH(a As Int32) As Int32
            Dim used As Int32 = 0
            For Each nb In Neighbors(a)
                used += nb.Item2
            Next
            Return Math.Max(0, ValenceOf(Elements(a), Charges(a)) - used - ExplicitH(a))
        End Function

        Public Function TotalH(a As Int32) As Int32
            Return ExplicitH(a) + ImplicitH(a)
        End Function

        ''' <summary>价态校验：违规原子列表（空 = 合法）</summary>
        Public Function ValenceViolations() As List(Of Int32)
            Dim bad As New List(Of Int32)()
            For a = 0 To NumAtoms() - 1
                Dim used As Int32 = 0
                For Each nb In Neighbors(a)
                    used += nb.Item2
                Next
                used += ExplicitH(a)
                If used > ValenceOf(Elements(a), Charges(a)) Then bad.Add(a)
            Next
            Return bad
        End Function

        ''' <summary>连通分量（原子索引列表）</summary>
        Public Function Components() As List(Of List(Of Int32))
            Dim seen(NumAtoms() - 1) As Boolean
            Dim comps As New List(Of List(Of Int32))()
            For a = 0 To NumAtoms() - 1
                If seen(a) Then Continue For
                Dim comp As New List(Of Int32)()
                Dim stack As New Stack(Of Int32)()
                stack.Push(a)
                seen(a) = True
                While stack.Count > 0
                    Dim x = stack.Pop()
                    comp.Add(x)
                    For Each nb In Neighbors(x)
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

        ''' <summary>提取连通分量为独立分子</summary>
        Public Function SplitComponents() As List(Of Molecule)
            Dim outList As New List(Of Molecule)()
            For Each comp In Components()
                Dim fm As New Molecule()
                Dim remap As New Dictionary(Of Int32, Int32)()
                For Each a In comp
                    remap(a) = fm.AddAtom(Elements(a), Charges(a))
                    fm.ExplicitH(remap(a)) = ExplicitH(a)
                Next
                For Each bd In Bonds
                    If remap.ContainsKey(bd.Item1) AndAlso remap.ContainsKey(bd.Item2) Then
                        fm.Bonds.Add(Tuple.Create(remap(bd.Item1), remap(bd.Item2), bd.Item3))
                    End If
                Next
                outList.Add(fm)
            Next
            Return outList
        End Function

        ''' <summary>Morgan/EC 迭代精化标签</summary>
        Public Function MorganRanks(Optional rounds As Int32 = 8) As List(Of String)
            Dim labels As New List(Of String)()
            For a = 0 To NumAtoms() - 1
                Dim orders = Neighbors(a).Select(Function(nb) nb.Item2).OrderBy(Function(x) x)
                labels.Add($"{Elements(a)}|{Charges(a)}|{ExplicitH(a)}|{Degree(a)}|" &
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
            Return labels
        End Function

        ''' <summary>分子指纹键（同构图同键）[readme.md §3 去重/汇匹配]</summary>
        Public Function MolKey() As String
            Dim ranks = MorganRanks()
            Dim uniq = ranks.Distinct().OrderBy(Function(x) x, StringComparer.Ordinal).ToList()
            Dim idxMap As New Dictionary(Of String, Int32)()
            For i = 0 To uniq.Count - 1
                idxMap(uniq(i)) = i
            Next
            Dim rk = ranks.Select(Function(x) idxMap(x)).ToList()
            Dim atomList As New List(Of String)()
            For a = 0 To NumAtoms() - 1
                atomList.Add($"{Elements(a)}|{Charges(a)}|{ExplicitH(a)}|{rk(a)}")
            Next
            atomList.Sort(StringComparer.Ordinal)
            Dim bondList As New List(Of String)()
            For Each bd In Bonds
                Dim ra = rk(bd.Item1)
                Dim rb = rk(bd.Item2)
                bondList.Add($"{Math.Min(ra, rb)}-{Math.Max(ra, rb)}:{bd.Item3}")
            Next
            bondList.Sort(StringComparer.Ordinal)
            Return String.Join(";", atomList) & "#" & String.Join(";", bondList)
        End Function

    End Class

End Namespace
