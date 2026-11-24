Imports G = SMRUCC.genomics.Molecule.SMILES.Graph
Namespace InchiSyntax

    ''' <summary>
    ''' 把 <see cref="InChIData"/> 构建成统一的分子图对象。
    ''' </summary>
    ''' <remarks>
    ''' InChI 只给出"重原子拓扑 + 每类原子上的氢数 + 形式电荷"，不含键级。
    ''' 因此构建过程分三步：先建原子与单键、再按电荷层与质子层定出带电中心、
    ''' 最后由 <see cref="BondOrderAssigner"/> 还原键级。
    ''' 所有无法确定的部分都写入 <see cref="G.MoleculeGraph.Warnings"/>。
    ''' </remarks>
    Public Module InChIBuilder

        ''' <summary>
        ''' 解析 InChI 文本并直接构建分子图。
        ''' </summary>
        ''' <param name="inchi">InChI 文本。</param>
        Public Function Parse(inchi As String) As G.MoleculeGraph
            Return Build(InChIReader.Parse(inchi), inchi)
        End Function

        ''' <summary>
        ''' 由层数据构建分子图。
        ''' </summary>
        ''' <param name="data">InChI 层数据。</param>
        ''' <param name="source">原始 InChI 字符串（写入 <see cref="G.MoleculeGraph.SourceText"/>）。</param>
        Public Function Build(data As InChIData, Optional source As String = Nothing) As G.MoleculeGraph
            Dim graph As New G.MoleculeGraph With {
                .Format = G.MoleculeFormat.InChI,
                .SourceText = If(source, data.Formula)
            }

            For Each element As String In data.Elements
                Dim atom As G.Atom = graph.AddAtom(element, 0)

                atom.NoImplicitH = True
            Next

            For Each item In data.Hydrogens
                If item.Key >= 0 AndAlso item.Key < graph.AtomCount Then
                    graph.Atoms(item.Key).ExplicitH = item.Value
                End If
            Next

            For Each bond In data.Bonds
                If bond.a < 0 OrElse bond.b < 0 Then Continue For
                If bond.a >= graph.AtomCount OrElse bond.b >= graph.AtomCount Then Continue For
                If bond.a = bond.b Then Continue For

                Call graph.AddBond(bond.a, bond.b, 1)
            Next

            Call assignCharges(graph, data)
            Call assignProtons(graph, data)
            Call assignStereo(graph, data)
            Call BondOrderAssigner.Assign(graph, data)

            graph.Invalidate()

            Call validateHydrogen(graph, data)

            Return graph
        End Function

        Private Sub assignCharges(graph As G.MoleculeGraph, data As InChIData)
            For i = 0 To data.ComponentOffsets.Count - 1
                Dim range As (start As Integer, [end] As Integer) = data.ComponentOffsets(i)

                If i >= data.ComponentCharges.Count Then Continue For

                Dim charge As Integer = data.ComponentCharges(i)

                If charge = 0 Then Continue For
                If range.start > range.[end] Then Continue For

                Dim members As List(Of Integer) = Enumerable.Range(range.start, range.[end] - range.start + 1).ToList()

                If members.Count = 1 Then
                    graph.Atoms(members(0)).Charge = charge
                    Continue For
                End If

                Dim remaining As Integer = charge
                Dim candidates As List(Of Integer)

                If charge > 0 Then
                    candidates = members.
                        Where(Function(a) isBasicCenter(graph.Atoms(a).Element)).
                        OrderByDescending(Function(a) graph.Degree(a)).
                        ToList()
                Else
                    candidates = members.
                        Where(Function(a) graph.Atoms(a).Element = "O" AndAlso graph.Degree(a) = 1).
                        ToList()

                    If candidates.Count = 0 Then
                        candidates = members.Where(Function(a) isAcidicCenter(graph.Atoms(a).Element)).ToList()
                    End If
                End If

                For Each atom As Integer In candidates
                    If remaining = 0 Then Exit For

                    If remaining > 0 Then
                        graph.Atoms(atom).Charge = 1
                        remaining -= 1
                    Else
                        graph.Atoms(atom).Charge = -1
                        remaining += 1
                    End If
                Next

                If remaining <> 0 Then
                    graph.Warnings.Add(
                        $"component {i + 1} total charge {charge} could not be placed on a specific atom")
                End If
            Next
        End Sub

        Private Function isBasicCenter(element As String) As Boolean
            Select Case element
                Case "N", "P", "S", "O"
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        Private Function isAcidicCenter(element As String) As Boolean
            Select Case element
                Case "O", "S", "N", "C"
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        Private Sub assignProtons(graph As G.MoleculeGraph, data As InChIData)
            For i = 0 To data.ComponentProtons.Count - 1
                Dim protons As Integer = data.ComponentProtons(i)

                If protons = 0 Then Continue For
                If i >= data.ComponentOffsets.Count Then Continue For

                Dim range As (start As Integer, [end] As Integer) = data.ComponentOffsets(i)

                If range.start > range.[end] Then Continue For

                Dim sign As Integer = If(protons > 0, 1, -1)
                Dim remaining As Integer = Math.Abs(protons)

                While remaining > 0
                    Dim target As Integer = -1

                    For atom = range.start To range.[end]
                        Dim element As String = graph.Atoms(atom).Element

                        If element = "O" AndAlso graph.Degree(atom) = 1 AndAlso graph.Atoms(atom).ExplicitH = 0 Then
                            target = atom
                            Exit For
                        End If
                    Next

                    If target < 0 Then
                        For atom = range.start To range.[end]
                            If graph.Atoms(atom).Element = "N" Then
                                target = atom
                                Exit For
                            End If
                        Next
                    End If

                    If target < 0 Then
                        graph.Warnings.Add($"protonation layer '/p{protons}' could not be placed on a specific atom")
                        Exit While
                    End If

                    graph.Atoms(target).ExplicitH += 1
                    graph.Atoms(target).Charge += sign
                    remaining -= 1
                End While
            Next
        End Sub

        Private Sub assignStereo(graph As G.MoleculeGraph, data As InChIData)
            For Each item In data.StereoCenters
                If item.Key < 0 OrElse item.Key >= graph.AtomCount Then Continue For

                Dim atom As G.Atom = graph.Atoms(item.Key)

                atom.StereoText = $"t{item.Key + 1}{If(item.Value > 0, "+", "-")}"

                ' InChI 的奇偶符号与 SMILES 的 @/@@ 不是同一套基准，
                ' 这里按"符号相同即同向"的近似对应写入，供 3D 重建做一致性校正
                atom.Stereo = If(item.Value > 0, G.AtomStereo.CounterClockwise, G.AtomStereo.Clockwise)

                graph.Warnings.Add(
                    $"stereo center #{item.Key} from InChI layer uses the InChI parity convention " &
                    "(applied as a best-effort approximation)")
            Next
        End Sub

        Private Sub validateHydrogen(graph As G.MoleculeGraph, data As InChIData)
            Dim counted As Integer = 0

            For i = 0 To graph.AtomCount - 1
                counted += graph.TotalH(i)
            Next

            If data.HydrogenCount > 0 AndAlso counted <> data.HydrogenCount Then
                graph.Warnings.Add(
                    $"hydrogen count mismatch: formula declares {data.HydrogenCount} but the graph holds {counted}")
            End If
        End Sub

    End Module

End Namespace
