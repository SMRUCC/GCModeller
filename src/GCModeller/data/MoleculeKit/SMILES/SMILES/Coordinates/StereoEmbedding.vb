Imports G = SMRUCC.genomics.Molecule.SMILES.Graph
Namespace Coordinates

    ''' <summary>
    ''' 3D 立体坐标重建：VSEPR 理想几何生长 + 简易力场弛豫 + 手性校正。
    ''' </summary>
    ''' <remarks>
    ''' <list type="number">
    ''' <item>按杂化类型（sp / sp² / sp³）为每个原子选择理想键角；</item>
    ''' <item>以 BFS 顺序把原子逐个放到"与已放置邻居夹角最接近理想值"的方向上；</item>
    ''' <item>用 Gauss-Seidel 式约束松弛同时优化键长、键角（以 1-3 距离表达）、环平面性与非键排斥；</item>
    ''' <item>最后按 <see cref="G.Atom.Stereo"/> 标记检查四面体手性，必要时交换两个取代基并重新弛豫。</item>
    ''' </list>
    ''' 输出坐标单位为埃（Å），质心位于原点。
    ''' </remarks>
    Public Module StereoEmbedding

        Const EPS As Double = 1.0E-9

        ''' <summary>
        ''' 对指定分子图执行 3D 坐标重建。
        ''' </summary>
        ''' <param name="graph">目标分子图（若需要显式氢，请先调用 <see cref="G.MoleculeGraph.AddHydrogens"/>）。</param>
        ''' <param name="options">重建参数。</param>
        Public Sub Run(graph As G.MoleculeGraph, options As G.Embed3DOptions)
            Dim n As Integer = graph.AtomCount

            If n = 0 Then Return

            Dim positions(n - 1) As Vec3
            Dim placed(n - 1) As Boolean

            Call growComponents(graph, positions, placed)
            Call relax(graph, positions, options)

            If options.RespectStereo Then
                If correctStereo(graph, positions, options) Then
                    Call relax(graph, positions, options)
                End If
            End If

            Dim list As New List(Of Vec3)

            For i = 0 To n - 1
                list.Add(positions(i))
            Next

            Call Geometry.CenterAtOrigin(list)

            For i = 0 To n - 1
                Call Geometry.SetPoint(graph, i, list(i))
            Next
        End Sub

#Region "初始几何生长"

        Private Sub growComponents(graph As G.MoleculeGraph, positions As Vec3(), placed As Boolean())
            Dim components As List(Of List(Of Integer)) = graph.Components()
            Dim offset As Vec3 = Vec3.Zero

            For Each component As List(Of Integer) In components
                Dim start As Integer = component(0)

                positions(start) = offset
                placed(start) = True

                Dim queue As New Queue(Of Integer)
                queue.Enqueue(start)

                While queue.Count > 0
                    Dim a As Integer = queue.Dequeue()

                    For Each neighbor In graph.Neighbors(a)
                        Dim b As Integer = neighbor.neighbor

                        If placed(b) Then Continue For

                        Dim direction As Vec3 = chooseDirection(graph, positions, placed, a)
                        Dim length As Double = G.ElementTable.DefaultBondLength(graph.Atoms(a).Element, graph.Atoms(b).Element)

                        If length < 0.6 Then length = 1.5

                        positions(b) = positions(a) + direction * length
                        placed(b) = True
                        queue.Enqueue(b)
                    Next
                End While

                Dim points As New List(Of Vec3)

                For Each index As Integer In component
                    points.Add(positions(index))
                Next

                offset += New Vec3(0, 0, Geometry.BoundingRadius(points) * 2 + 1.5)
            Next
        End Sub

        Private Function chooseDirection(graph As G.MoleculeGraph, positions As Vec3(), placed As Boolean(), a As Integer) As Vec3
            Dim directions As New List(Of Vec3)

            For Each neighbor In graph.Neighbors(a)
                Dim other As Integer = neighbor.neighbor

                If Not placed(other) Then Continue For

                Dim v As Vec3 = positions(other) - positions(a)

                If v.Length > EPS Then directions.Add(v.Normalize())
            Next

            Dim hybrid As G.Hybridization = graph.Hybridization(a)
            Dim theta As Double = idealAngle(hybrid)

            Select Case directions.Count
                Case 0
                    Return New Vec3(1, 0, 0)

                Case 1
                    ' 只有一个已知邻居：新键与它成理想夹角（sp 为 180°，其余按 VSEPR 收敛）
                    Dim u As Vec3 = directions(0)

                    If theta >= 175 Then Return -u

                    Dim rad1 As Double = theta * Math.PI / 180.0
                    Dim perp As Vec3 = Vec3.AnyPerpendicular(u)
                    Dim best As Vec3 = -u
                    Dim bestScore As Double = Double.MinValue

                    For k = 0 To 5
                        Dim spin As Vec3 = Vec3.Rotate(perp, u, 60 * k)
                        Dim candidate As Vec3 = (u * Math.Cos(rad1) + spin * Math.Sin(rad1)).Normalize()

                        If candidate.Length < 0.2 Then Continue For

                        Dim score As Double = clearance(graph, positions, placed, a, candidate)

                        If score > bestScore Then
                            bestScore = score
                            best = candidate
                        End If
                    Next

                    Return best

                Case 2
                    Dim sum As Vec3 = directions(0) + directions(1)
                    Dim bisector As Vec3

                    If sum.Length < 0.1 Then
                        bisector = Vec3.AnyPerpendicular(directions(0))
                    Else
                        bisector = (-sum).Normalize()
                    End If

                    Dim perpendicular As Vec3 = Vec3.Cross(directions(0), directions(1)).Normalize()

                    If perpendicular.Length < 0.2 Then
                        perpendicular = Vec3.AnyPerpendicular(bisector)
                    End If

                    Dim rad As Double = theta * Math.PI / 180.0
                    Dim candidate1 As Vec3 = (bisector * Math.Cos(rad / 2) + perpendicular * Math.Sin(rad / 2)).Normalize()
                    Dim candidate2 As Vec3 = (bisector * Math.Cos(rad / 2) - perpendicular * Math.Sin(rad / 2)).Normalize()

                    Return If(clearance(graph, positions, placed, a, candidate1) >= clearance(graph, positions, placed, a, candidate2),
                              candidate1, candidate2)

                Case 3
                    Dim sum As Vec3 = directions(0) + directions(1) + directions(2)
                    Dim direction As Vec3 = (-sum).Normalize()

                    If direction.Length < 0.2 Then direction = Vec3.AnyPerpendicular(directions(0))

                    Return direction

                Case Else
                    Dim sum As Vec3 = Vec3.Zero

                    For Each direction As Vec3 In directions
                        sum += direction
                    Next

                    Dim result As Vec3 = (-sum).Normalize()

                    If result.Length < 0.2 Then result = Vec3.AnyPerpendicular(directions(0))

                    Return result
            End Select
        End Function

        Private Function clearance(graph As G.MoleculeGraph, positions As Vec3(), placed As Boolean(), a As Integer, direction As Vec3) As Double
            Dim probe As Vec3 = positions(a) + direction * 1.5
            Dim score As Double = Double.MaxValue

            For i = 0 To placed.Length - 1
                If Not placed(i) OrElse i = a Then Continue For
                If graph.BondOrder(a, i) > 0 Then Continue For

                Dim d As Double = (positions(i) - probe).Length

                If d < score Then score = d
            Next

            If score = Double.MaxValue Then score = 10

            Return score
        End Function

        Private Function idealAngle(hybrid As G.Hybridization) As Double
            Select Case hybrid
                Case G.Hybridization.SP
                    Return 180
                Case G.Hybridization.SP2
                    Return 120
                Case Else
                    Return 109.47
            End Select
        End Function

#End Region

#Region "力场弛豫"

        Private Sub relax(graph As G.MoleculeGraph, positions As Vec3(), options As G.Embed3DOptions)
            Dim n As Integer = graph.AtomCount

            If n < 2 Then Return

            Dim bonds As New List(Of (a As Integer, b As Integer, target As Double))
            Dim angles As New List(Of (a As Integer, c As Integer, target As Double))
            Dim repulsion As New List(Of (a As Integer, b As Integer))

            For Each bond As G.Bond In graph.Bonds
                If bond.a = bond.b Then Continue For

                Dim target As Double = G.ElementTable.DefaultBondLength(graph.Atoms(bond.a).Element, graph.Atoms(bond.b).Element)

                If target < 0.6 Then target = 1.5

                ' 双键/叁键略微缩短，体现真实的键长差异
                If bond.OrderInt = 2 Then target *= 0.93
                If bond.OrderInt >= 3 Then target *= 0.86

                bonds.Add((bond.a, bond.b, target))
            Next

            For center = 0 To n - 1
                Dim neighbors As List(Of (neighbor As Integer, order As Double)) = graph.Neighbors(center)
                Dim theta As Double = idealAngle(graph.Hybridization(center)) * Math.PI / 180.0

                For i = 0 To neighbors.Count - 1
                    For j = i + 1 To neighbors.Count - 1
                        Dim a As Integer = neighbors(i).neighbor
                        Dim b As Integer = neighbors(j).neighbor
                        Dim la As Double = bondGoal(graph, a, center)
                        Dim lb As Double = bondGoal(graph, b, center)
                        Dim d13 As Double = Math.Sqrt(la * la + lb * lb - 2 * la * lb * Math.Cos(theta))

                        angles.Add((a, b, d13))
                    Next
                Next
            Next

            Dim distance(graph.AtomCount - 1, graph.AtomCount - 1) As Integer

            Call graphDistances(graph, distance)

            For a = 0 To n - 1
                For b = a + 1 To n - 1
                    If distance(a, b) >= 3 Then repulsion.Add((a, b))
                Next
            Next

            Dim iterations As Integer = Math.Max(50, options.Iterations)
            Dim cutoff As Double = options.RepulsionCutoff

            For iteration = 1 To iterations
                For Each item In bonds
                    Call relaxPair(positions, item.a, item.b, item.target, options.BondWeight)
                Next

                For Each item In angles
                    Call relaxPair(positions, item.a, item.c, item.target, options.AngleWeight)
                Next

                For Each item In repulsion
                    Dim delta As Vec3 = positions(item.b) - positions(item.a)
                    Dim d As Double = delta.Length

                    If d < cutoff AndAlso d > EPS Then
                        Dim push As Double = (cutoff - d) / d * 0.5 * options.RepulsionWeight

                        positions(item.a) -= delta * push
                        positions(item.b) += delta * push
                    End If
                Next

                If options.PlanarityWeight > 0 Then
                    Call enforcePlanarity(graph, positions, options.PlanarityWeight)
                End If
            Next
        End Sub

        Private Function bondGoal(graph As G.MoleculeGraph, a As Integer, b As Integer) As Double
            Dim target As Double = G.ElementTable.DefaultBondLength(graph.Atoms(a).Element, graph.Atoms(b).Element)

            Return If(target < 0.6, 1.5, target)
        End Function

        Private Sub relaxPair(positions As Vec3(), a As Integer, b As Integer, target As Double, weight As Double)
            Dim delta As Vec3 = positions(b) - positions(a)
            Dim d As Double = delta.Length

            If d < EPS Then
                positions(b) += New Vec3(0.01, 0.01, 0.01)
                Return
            End If

            ' 把单步修正量限制在当前间距的一半以内，避免极短距离下出现过冲振荡
            Dim factor As Double = (d - target) / d * weight * 0.5

            If factor > 0.5 Then factor = 0.5
            If factor < -0.5 Then factor = -0.5

            Dim move As Vec3 = delta * factor

            positions(a) += move
            positions(b) -= move
        End Sub

        ''' <summary>
        ''' 环平面性约束：把全为 sp² 的环拉向其最佳拟合平面。
        ''' </summary>
        Private Sub enforcePlanarity(graph As G.MoleculeGraph, positions As Vec3(), weight As Double)
            For Each ring As G.Ring In graph.Rings()
                Dim sp2 As Boolean = True

                For Each atom As Integer In ring.Atoms
                    If graph.Hybridization(atom) <> G.Hybridization.SP2 Then
                        sp2 = False
                        Exit For
                    End If
                Next

                If Not sp2 Then Continue For

                Dim center As New List(Of Vec3)

                For Each atom As Integer In ring.Atoms
                    center.Add(positions(atom))
                Next

                Dim centroid As Vec3 = Geometry.Centroid(center)
                Dim normal As Vec3 = Vec3.Zero
                Dim count As Integer = ring.Atoms.Length

                For k = 0 To count - 1
                    Dim p1 As Vec3 = positions(ring.Atoms(k)) - centroid
                    Dim p2 As Vec3 = positions(ring.Atoms((k + 1) Mod count)) - centroid

                    normal += Vec3.Cross(p1, p2)
                Next

                normal = normal.Normalize()

                If normal.Length < 0.2 Then Continue For

                For Each atom As Integer In ring.Atoms
                    Dim offset As Vec3 = positions(atom) - centroid
                    Dim height As Double = offset * normal

                    positions(atom) -= normal * (height * weight)
                Next
            Next
        End Sub

        Private Sub graphDistances(graph As G.MoleculeGraph, distance As Integer(,))
            Dim n As Integer = graph.AtomCount

            For i = 0 To n - 1
                For j = 0 To n - 1
                    distance(i, j) = If(i = j, 0, 99)
                Next
            Next

            Dim adjacency As List(Of List(Of Integer)) = graph.Adjacency()

            For start = 0 To n - 1
                Dim queue As New Queue(Of Integer)
                Dim visited(n - 1) As Boolean

                visited(start) = True
                queue.Enqueue(start)

                While queue.Count > 0
                    Dim current As Integer = queue.Dequeue()

                    For Each id As Integer In adjacency(current)
                        Dim bond As G.Bond = graph.Bonds(id)
                        Dim nextNode As Integer = If(bond.a = current, bond.b, bond.a)

                        If visited(nextNode) Then Continue For

                        visited(nextNode) = True
                        distance(start, nextNode) = distance(start, current) + 1
                        queue.Enqueue(nextNode)
                    Next
                End While
            Next
        End Sub

#End Region

#Region "手性校正"

        ''' <summary>
        ''' 按 <see cref="G.Atom.Stereo"/> 检查并校正四面体手性。
        ''' </summary>
        ''' <returns>是否发生过取代基交换（发生交换后需要重新弛豫）。</returns>
        Private Function correctStereo(graph As G.MoleculeGraph, positions As Vec3(), options As G.Embed3DOptions) As Boolean
            Dim changed As Boolean = False

            For a = 0 To graph.AtomCount - 1
                Dim atom As G.Atom = graph.Atoms(a)

                If atom.Stereo = G.AtomStereo.None OrElse atom.Stereo = G.AtomStereo.Unsupported Then Continue For
                If graph.Degree(a) < 3 Then Continue For

                Dim order As New List(Of Integer)

                If atom.StereoNeighbors IsNot Nothing Then
                    For Each index As Integer In atom.StereoNeighbors
                        If index >= 0 AndAlso index < graph.AtomCount AndAlso index <> a AndAlso graph.BondOrder(a, index) > 0 Then
                            If Not order.Contains(index) Then order.Add(index)
                        End If
                    Next
                End If

                For Each neighbor In graph.Neighbors(a)
                    If Not order.Contains(neighbor.neighbor) Then order.Add(neighbor.neighbor)
                Next

                If order.Count < 3 Then Continue For

                Dim points As New List(Of Integer) From {order(0), order(1), order(2)}
                Dim fourth As Integer = If(order.Count >= 4, order(3), -1)

                Dim sign As Integer = stereoSign(positions, a, points, fourth)
                Dim expected As Integer = If(atom.Stereo = G.AtomStereo.CounterClockwise, 1, -1)

                If sign = 0 OrElse sign = expected Then Continue For

                ' 交换两个取代基的位置即可翻转构型
                Dim first As Integer = order(1)
                Dim second As Integer = order(2)
                Dim buffer As Vec3 = positions(first)

                positions(first) = positions(second)
                positions(second) = buffer
                changed = True

                Dim now As Integer = stereoSign(positions, a, points, fourth)

                If now <> expected Then
                    graph.Warnings.Add($"stereo center #{a} could not be inverted to the declared configuration")
                End If
            Next

            Return changed
        End Function

        Private Function stereoSign(positions As Vec3(), center As Integer, points As List(Of Integer), fourthIndex As Integer) As Integer
            Dim origin As Vec3 = positions(center)
            Dim vectors As New List(Of Vec3)

            For Each index As Integer In points
                Dim v As Vec3 = positions(index) - origin

                If v.Length < EPS Then Return 0

                vectors.Add(v.Normalize())
            Next

            Dim fourth As Vec3

            If fourthIndex >= 0 Then
                fourth = (positions(fourthIndex) - origin).Normalize()
            Else
                fourth = -(vectors(0) + vectors(1) + vectors(2))

                If fourth.Length < EPS Then Return 0

                fourth = fourth.Normalize()
            End If

            Dim volume As Double = Vec3.Triple(vectors(0), vectors(1), fourth)

            If Math.Abs(volume) < 1.0E-6 Then Return 0

            Return If(volume > 0, 1, -1)
        End Function

#End Region

    End Module

End Namespace
