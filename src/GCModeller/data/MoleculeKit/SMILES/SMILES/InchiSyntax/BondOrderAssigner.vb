Imports G = SMRUCC.genomics.Molecule.SMILES.Graph
Namespace InchiSyntax

    ''' <summary>
    ''' 由"价态守恒 + 氢层计数 + 最小自由基"反推 InChI 分子图的键级。
    ''' </summary>
    ''' <remarks>
    ''' InChI 的连接层只描述拓扑，不含键级。这里采用贪心策略：
    ''' <list type="number">
    ''' <item>先按元素/电荷定出每个原子的目标价态，减去已声明的氢与当前单键数，得到"价键缺口"；</item>
    ''' <item>反复扫描键表，只要一条键两端的缺口都为正就把键级 +1（上限 3），
    ''' 于是单键缺口会自然配对成双键，双缺口配对成叁键；</item>
    ''' <item>剩余的单缺口由移动氢（<c>/h</c> 层括号组）补足；</item>
    ''' <item>仍有缺口时写入告警，不再强行制造化学上不合理的结构。</item>
    ''' </list>
    ''' </remarks>
    Public Module BondOrderAssigner

        ''' <summary>
        ''' 为分子图还原键级。
        ''' </summary>
        ''' <param name="graph">已建好原子、单键与电荷的分子图。</param>
        ''' <param name="data">InChI 层数据（提供移动氢组）。</param>
        Public Sub Assign(graph As G.MoleculeGraph, data As InChIData)
            Dim n As Integer = graph.AtomCount

            If n = 0 Then Return

            Dim deficits(n - 1) As Integer

            For i = 0 To n - 1
                deficits(i) = deficitOf(graph, i)
            Next

            Dim changed As Boolean = True
            Dim guard As Integer = 0

            While changed AndAlso guard < 64
                changed = False
                guard += 1

                For Each bond As G.Bond In graph.Bonds
                    If bond.order >= 3 Then Continue For
                    If deficits(bond.a) <= 0 OrElse deficits(bond.b) <= 0 Then Continue For

                    bond.order += 1
                    deficits(bond.a) -= 1
                    deficits(bond.b) -= 1
                    changed = True
                Next
            End While

            graph.Invalidate()

            ' 移动氢补偿
            For Each group As MobileHydrogen In data.MobileHydrogens
                Dim remaining As Integer = group.Count
                Dim candidates As IEnumerable(Of Integer) = group.Atoms.
                    Where(Function(a) a >= 0 AndAlso a < n).
                    OrderByDescending(Function(a) deficits(a))

                For Each atom As Integer In candidates
                    If remaining <= 0 Then Exit For
                    If deficits(atom) <= 0 Then Continue For

                    graph.Atoms(atom).ExplicitH += 1
                    deficits(atom) -= 1
                    remaining -= 1
                Next

                If remaining > 0 Then
                    With graph
                        .Warnings.Add($"mobile hydrogen group {group} left {remaining} hydrogen(s) unassigned")
                    End With
                End If
            Next

            Dim leftover As New List(Of Integer)

            For i = 0 To n - 1
                If deficits(i) > 0 Then leftover.Add(i)
            Next

            If leftover.Count > 0 Then
                graph.Warnings.Add(
                    "bond order assignment left unsatisfied valence on atom(s): " &
                    String.Join(", ", leftover.Select(Function(a) $"#{a} {graph.Atoms(a).Element}")))
            End If

            Call G.ChemistryRules.PerceiveAromaticity(graph)
        End Sub

        ''' <summary>
        ''' 计算某个原子当前的价键缺口。
        ''' </summary>
        ''' <param name="graph">分子图。</param>
        ''' <param name="index">原子下标。</param>
        Public Function DeficitOf(graph As G.MoleculeGraph, index As Integer) As Integer
            Dim atom As G.Atom = graph.Atoms(index)
            Dim used As Integer = atom.ExplicitH

            For Each bond As G.Bond In graph.BondsOf(index)
                used += CInt(Math.Round(bond.order))
            Next

            Dim target As Integer = G.ChemistryRules.EffectiveValence(atom.Element, atom.Charge, used)

            If target <= used Then Return 0

            Return target - used
        End Function

    End Module

End Namespace
