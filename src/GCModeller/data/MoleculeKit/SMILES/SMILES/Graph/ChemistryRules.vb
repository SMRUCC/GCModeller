Namespace Graph

    ''' <summary>
    ''' 化学规则：价态模型、隐式氢补全、芳香性 Kekulé 化与杂化推断。
    ''' </summary>
    ''' <remarks>
    ''' 这里的规则服务于"从结构式字符串还原出可计算的分子图"这一目标，因此优先保证
    ''' 常见有机分子（C/H/N/O/S/P/卤素）的正确性，其次是过渡金属与超价体系的健壮性。
    ''' 无法判定的情形一律写入 <see cref="MoleculeGraph.Warnings"/>，不抛出异常。
    ''' </remarks>
    Public Module ChemistryRules

        ''' <summary>
        ''' 原子的目标价态（按形式电荷修正后的标准价态）。
        ''' </summary>
        ''' <param name="element">元素符号。</param>
        ''' <param name="charge">形式电荷。</param>
        ''' <returns>目标价态；未知元素返回 <c>0</c>，表示不参与隐式氢补全。</returns>
        Public Function TargetValence(element As String, charge As Integer) As Integer
            Select Case element
                Case "H", "D", "T"
                    Return 1
                Case "C", "Si"
                    Return If(charge = 0, 4, 3)
                Case "N"
                    Return 3 + charge
                Case "O"
                    Return 2 + charge
                Case "B"
                    Return 3 - charge
                Case "P"
                    Return If(charge > 0, 4, 3)
                Case "S", "Se", "Te"
                    Return 2
                Case "F", "Cl", "Br", "I", "At"
                    Return 1
                Case Else
                    Return 0
            End Select
        End Function

        ''' <summary>
        ''' 在"已用键级"为 <paramref name="used"/> 时该原子应当达到的价态：
        ''' 对 S/P/卤素等可超价的元素会向上取下一个允许价态。
        ''' </summary>
        ''' <param name="element">元素符号。</param>
        ''' <param name="charge">形式电荷。</param>
        ''' <param name="used">当前已占用的键级总和。</param>
        Public Function EffectiveValence(element As String, charge As Integer, used As Integer) As Integer
            Dim target As Integer = TargetValence(element, charge)

            If target = 0 Then Return 0
            If used <= target Then Return target

            Dim valences As Integer() = ElementTable.GetElement(element).Valences

            If valences IsNot Nothing Then
                For i = 0 To valences.Length - 1
                    If valences(i) >= used Then Return valences(i)
                Next
            End If

            Return used
        End Function

        ''' <summary>
        ''' 计算某个原子的隐式氢数目。
        ''' </summary>
        ''' <param name="graph">所属分子图。</param>
        ''' <param name="index">原子下标。</param>
        ''' <returns>需要补足的隐式氢个数。</returns>
        Public Function ImplicitHydrogens(graph As MoleculeGraph, index As Integer) As Integer
            Dim atom As Atom = graph.Atoms(index)

            If atom.NoImplicitH Then Return 0

            Dim used As Integer = atom.ExplicitH

            For Each bond As Bond In graph.BondsOf(index)
                used += CInt(Math.Round(bond.order))
            Next

            Dim target As Integer = EffectiveValence(atom.Element, atom.Charge, used)

            If target <= used Then Return 0

            Return target - used
        End Function

        ''' <summary>
        ''' 依据成键模式推断杂化类型。
        ''' </summary>
        ''' <param name="graph">所属分子图。</param>
        ''' <param name="index">原子下标。</param>
        Public Function HybridizationOf(graph As MoleculeGraph, index As Integer) As Hybridization
            Dim doubles As Integer = 0
            Dim hasTriple As Boolean = False
            Dim hasAromatic As Boolean = False

            For Each bond As Bond In graph.BondsOf(index)
                Dim order As Integer = bond.OrderInt

                If order >= 3 Then
                    hasTriple = True
                ElseIf order = 2 Then
                    doubles += 1
                End If

                If bond.IsAromatic Then hasAromatic = True
            Next

            If hasTriple OrElse doubles >= 2 Then Return Hybridization.SP
            If doubles = 1 OrElse hasAromatic OrElse graph.Atoms(index).IsAromatic Then Return Hybridization.SP2

            Return Hybridization.SP3
        End Function

        ''' <summary>
        ''' 对芳香子图执行 Kekulé 化：为每个需要双键的芳香原子配出一组双键，
        ''' 使键级回到 1/2 的经典表示。
        ''' </summary>
        ''' <param name="graph">需要处理的分子图。</param>
        ''' <returns>成功配对的芳香键条数。</returns>
        Public Function Kekulize(graph As MoleculeGraph) As Integer
            Dim aromaticBonds As List(Of Bond) = graph.Bonds.Where(Function(b) b.IsAromatic).ToList()

            If aromaticBonds.Count = 0 Then Return 0

            Dim need(graph.Atoms.Count - 1) As Integer
            Dim demand As New Dictionary(Of Integer, Integer)

            For Each bond As Bond In aromaticBonds
                bond.order = 1
            Next

            For i = 0 To graph.Atoms.Count - 1
                Dim atom As Atom = graph.Atoms(i)

                If Not atom.IsAromatic Then Continue For

                Dim aromaticDegree As Integer = 0
                Dim otherOrders As Integer = 0

                For Each bond As Bond In graph.BondsOf(i)
                    If bond.IsAromatic Then
                        aromaticDegree += 1
                    Else
                        otherOrders += CInt(Math.Round(bond.order))
                    End If
                Next

                Dim target As Integer = EffectiveValence(atom.Element, atom.Charge, 0)
                Dim deficit As Integer = target - atom.ExplicitH - otherOrders - aromaticDegree

                If deficit < 0 Then deficit = 0
                If deficit > 1 Then deficit = 1

                need(i) = deficit
                demand(i) = deficit
            Next

            Dim matched(graph.Atoms.Count - 1) As Boolean
            Dim assign As New List(Of Bond)

            If matchAromatic(graph, aromaticBonds, need, matched, 0, assign) Then
                For Each bond As Bond In assign
                    bond.order = 2
                Next

                Return assign.Count
            Else
                ' 无法完美匹配（例如含奇数个需要双键的原子），退化为全单键并记录告警
                For Each bond As Bond In aromaticBonds
                    bond.order = 1
                Next

                graph.Warnings.Add(
                    $"aromatic system could not be kekulized ({demand.Values.Sum()} pi-needy atom(s)); " &
                    "kept as alternating single bonds")
                Return 0
            End If
        End Function

        ''' <summary>
        ''' 依据 Kekulé 键级感知芳香性（InChI 没有芳香性信息，需要事后推断）。
        ''' </summary>
        ''' <param name="graph">已还原键级的分子图。</param>
        ''' <returns>被标记为芳香环的环个数。</returns>
        Public Function PerceiveAromaticity(graph As MoleculeGraph) As Integer
            Dim marked As Integer = 0

            For Each ring As Ring In graph.Rings()
                If ring.Size < 5 OrElse ring.Size > 7 Then Continue For

                Dim ringBonds As New HashSet(Of Integer)(ring.BondIndices)
                Dim doubles As Integer = 0
                Dim viable As Boolean = True

                For Each id As Integer In ring.BondIndices
                    Dim bond As Bond = graph.Bonds(id)

                    If bond.order >= 3 Then
                        viable = False
                        Exit For
                    End If

                    If bond.order = 2 Then doubles += 1
                Next

                If Not viable OrElse doubles <> ring.Size \ 2 Then Continue For

                For Each atom As Integer In ring.Atoms
                    Select Case graph.Atoms(atom).Element
                        Case "C", "N", "O", "S", "P", "B", "Se", "Te"
                            ' 可参与共轭
                        Case Else
                            viable = False
                            Exit For
                    End Select

                    For Each id As Integer In graph.Adjacency()(atom)
                        If ringBonds.Contains(id) Then Continue For
                        If graph.Bonds(id).order >= 2 Then
                            viable = False
                            Exit For
                        End If
                    Next

                    If Not viable Then Exit For
                Next

                If Not viable Then Continue For

                For Each atom As Integer In ring.Atoms
                    graph.Atoms(atom).IsAromatic = True
                Next

                For Each id As Integer In ring.BondIndices
                    graph.Bonds(id).IsAromatic = True
                Next

                marked += 1
            Next

            Return marked
        End Function

        Private Function matchAromatic(graph As MoleculeGraph,
                                       aromaticBonds As List(Of Bond),
                                       need As Integer(),
                                       matched As Boolean(),
                                       cursor As Integer,
                                       assign As List(Of Bond)) As Boolean

            While cursor < need.Length AndAlso (need(cursor) = 0 OrElse matched(cursor))
                cursor += 1
            End While

            If cursor >= need.Length Then Return True

            For Each bond As Bond In graph.BondsOf(cursor)
                If Not bond.IsAromatic Then Continue For

                Dim other As Integer = If(bond.a = cursor, bond.b, bond.a)

                If need(other) = 0 OrElse matched(other) Then Continue For

                matched(cursor) = True
                matched(other) = True
                assign.Add(bond)

                If matchAromatic(graph, aromaticBonds, need, matched, cursor + 1, assign) Then Return True

                assign.RemoveAt(assign.Count - 1)
                matched(cursor) = False
                matched(other) = False
            Next

            Return False
        End Function

    End Module

End Namespace
