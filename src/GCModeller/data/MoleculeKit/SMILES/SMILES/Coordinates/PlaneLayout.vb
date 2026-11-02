Namespace Coordinates

    ''' <summary>
    ''' 2D 平面坐标重建：环模板布局 + 取代基生长 + 力导向精修。
    ''' </summary>
    ''' <remarks>
    ''' <list type="number">
    ''' <item>先用最小环基把环系摆成正多边形（稠合环以共享键为基准拼接、螺环以共享原子为基准旋转）；</item>
    ''' <item>再以 BFS 顺序向外生长取代基，逐个尝试候选方向并挑选"碰撞最少"的方向；</item>
    ''' <item>最后做 Gauss-Seidel 式的约束松弛（等键长 + 1-3 距离 + 1-4 排斥），并归一化键长。</item>
    ''' </list>
    ''' 输出的坐标位于化学坐标系（Y 轴向上），键长被归一化为
    ''' <see cref="Graph.Layout2DOptions.BondLength"/>，质心位于原点。
    ''' </remarks>
    Public Module PlaneLayout

        Const BOND_LENGTH As Double = 1.0
        Const MIN_SEPARATION As Double = 0.85
        Const EPS As Double = 1.0E-9

        ''' <summary>
        ''' 对指定分子图执行 2D 坐标重建。
        ''' </summary>
        ''' <param name="graph">目标分子图。</param>
        ''' <param name="options">布局参数。</param>
        Public Sub Run(graph As Graph.MoleculeGraph, options As Graph.Layout2DOptions)
            Dim n As Integer = graph.AtomCount

            If n = 0 Then Return

            Dim px(n - 1) As Double
            Dim py(n - 1) As Double
            Dim placed(n - 1) As Boolean

            Call placeRings(graph, px, py, placed)
            Call growSubstituents(graph, px, py, placed)
            Call refine(graph, px, py, options)

            For i = 0 To n - 1
                If Not placed(i) Then
                    px(i) = Math.Cos(i * 2.4) * 2.0
                    py(i) = Math.Sin(i * 2.4) * 2.0
                End If
            Next

            Call normalize(graph, px, py, options)

            For i = 0 To n - 1
                With graph.Atoms(i)
                    .X = px(i)
                    .Y = py(i)
                    .Z = 0
                End With
            Next
        End Sub

#Region "环模板布局"

        Private Sub placeRings(graph As Graph.MoleculeGraph, px As Double(), py As Double(), placed As Boolean())
            Dim rings As List(Of Graph.Ring) = graph.Rings()

            If rings.Count = 0 Then Return

            Dim systems As List(Of List(Of Graph.Ring)) = Graph.Rings.RingSystems(rings)

            systems.Sort(Function(x, y) y.Count.CompareTo(x.Count))

            For Each system As List(Of Graph.Ring) In systems
                For Each ring As Graph.Ring In system.OrderByDescending(Function(r) r.Size)
                    Dim shared As Integer = ring.Atoms.Count(Function(a) placed(a))

                    If shared = 0 Then
                        Dim center As (x As Double, y As Double) = findFreeCenter(px, py, placed, ringRadius(ring.Size))
                        Call placePolygon(ring, center.x, center.y, 0, 1, px, py, placed)
                    ElseIf shared >= 2 Then
                        Dim edge As Integer = findPlacedEdge(ring, placed)

                        If edge >= 0 AndAlso placeFromEdge(ring, edge, px, py, placed) Then
                            Continue For
                        End If

                        Dim center2 As (x As Double, y As Double) = findFreeCenter(px, py, placed, ringRadius(ring.Size))
                        Call placePolygon(ring, center2.x, center2.y, 0, 1, px, py, placed)
                    Else
                        Dim anchor As Integer = -1

                        For Each a As Integer In ring.Atoms
                            If placed(a) Then
                                anchor = a
                                Exit For
                            End If
                        Next

                        If anchor >= 0 Then
                            Call placeFromAtom(ring, anchor, px, py, placed)
                        Else
                            Dim center3 As (x As Double, y As Double) = findFreeCenter(px, py, placed, ringRadius(ring.Size))
                            Call placePolygon(ring, center3.x, center3.y, 0, 1, px, py, placed)
                        End If
                    End If
                Next
            Next
        End Sub

        Private Function ringRadius(size As Integer) As Double
            If size < 3 Then Return BOND_LENGTH

            Return BOND_LENGTH / (2 * Math.Sin(Math.PI / size))
        End Function

        Private Sub placePolygon(ring As Graph.Ring, cx As Double, cy As Double, startAngle As Double, orientation As Integer,
                                 px As Double(), py As Double(), placed As Boolean())

            Dim n As Integer = ring.Size
            Dim radius As Double = ringRadius(n)

            For k = 0 To n - 1
                Dim angle As Double = startAngle + orientation * (2 * Math.PI * k / n)
                Dim atom As Integer = ring.Atoms(k)

                px(atom) = cx + radius * Math.Cos(angle)
                py(atom) = cy + radius * Math.Sin(angle)
                placed(atom) = True
            Next
        End Sub

        Private Function findFreeCenter(px As Double(), py As Double(), placed As Boolean(), radius As Double) As (x As Double, y As Double)
            Dim count As Integer = 0
            Dim cx As Double = 0
            Dim cy As Double = 0

            For i = 0 To placed.Length - 1
                If placed(i) Then
                    cx += px(i)
                    cy += py(i)
                    count += 1
                End If
            Next

            If count = 0 Then Return (0, 0)

            cx /= count
            cy /= count

            Dim spread As Double = 0

            For i = 0 To placed.Length - 1
                If placed(i) Then
                    Dim d As Double = Math.Sqrt((px(i) - cx) ^ 2 + (py(i) - cy) ^ 2)

                    If d > spread Then spread = d
                End If
            Next

            Dim best As (x As Double, y As Double) = (cx + spread + radius + 1.5, cy)
            Dim bestScore As Double = Double.MinValue

            For step = 0 To 23
                Dim angle As Double = step * Math.PI / 12
                Dim x As Double = cx + (spread + radius + 1.5) * Math.Cos(angle)
                Dim y As Double = cy + (spread + radius + 1.5) * Math.Sin(angle)
                Dim score As Double = clearScore(px, py, placed, x, y, radius)

                If score > bestScore Then
                    bestScore = score
                    best = (x, y)
                End If
            Next

            Return best
        End Function

        Private Function clearScore(px As Double(), py As Double(), placed As Boolean(), cx As Double, cy As Double, radius As Double) As Double
            Dim score As Double = Double.MaxValue

            For i = 0 To placed.Length - 1
                If Not placed(i) Then Continue For

                Dim d As Double = Math.Sqrt((px(i) - cx) ^ 2 + (py(i) - cy) ^ 2) - radius

                If d < score Then score = d
            Next

            If score = Double.MaxValue Then score = 10

            Return score
        End Function

        Private Function findPlacedEdge(ring As Graph.Ring, placed As Boolean()) As Integer
            Dim n As Integer = ring.Size

            For k = 0 To n - 1
                If placed(ring.Atoms(k)) AndAlso placed(ring.Atoms((k + 1) Mod n)) Then Return k
            Next

            Return -1
        End Function

        ''' <summary>
        ''' 以环上已布局的一条键为基准，把整个环按正多边形拼接上去。
        ''' </summary>
        Private Function placeFromEdge(ring As Graph.Ring, edge As Integer,
                                       px As Double(), py As Double(), placed As Boolean()) As Boolean

            Dim n As Integer = ring.Size
            Dim a As Integer = ring.Atoms(edge)
            Dim b As Integer = ring.Atoms((edge + 1) Mod n)

            Dim ax As Double = px(a)
            Dim ay As Double = py(a)
            Dim bx As Double = px(b)
            Dim by As Double = py(b)

            Dim length As Double = Math.Sqrt((bx - ax) ^ 2 + (by - ay) ^ 2)

            If length < EPS Then Return False

            Dim baseAngle As Double = Math.Atan2(by - ay, bx - ax)
            Dim bestOrientation As Integer = 1
            Dim bestScore As Double = Double.MinValue

            For Each orientation As Integer In New Integer() {1, -1}
                Dim score As Double = 0
                Dim cx As Double = ax
                Dim cy As Double = ay
                Dim direction As Double = baseAngle
                Dim turn As Double = orientation * 2 * Math.PI / n

                For k = 1 To n - 1
                    If k > 1 Then direction += turn

                    Dim x As Double = cx + length * Math.Cos(direction)
                    Dim y As Double = cy + length * Math.Sin(direction)
                    Dim atom As Integer = ring.Atoms((edge + k) Mod n)

                    If placed(atom) Then
                        score -= 20  ' 覆盖已布局原子说明取错了方向
                        score -= Math.Sqrt((px(atom) - x) ^ 2 + (py(atom) - y) ^ 2) * 10
                    Else
                        score += Math.Min(3.0, clearScoreFor(px, py, placed, atom, x, y, ring))
                    End If

                    cx = x
                    cy = y
                Next

                If score > bestScore Then
                    bestScore = score
                    bestOrientation = orientation
                End If
            Next

            Dim currentX As Double = ax
            Dim currentY As Double = ay
            Dim dir As Double = baseAngle
            Dim step As Double = bestOrientation * 2 * Math.PI / n

            For k = 1 To n - 1
                If k > 1 Then dir += step

                Dim x As Double = currentX + length * Math.Cos(dir)
                Dim y As Double = currentY + length * Math.Sin(dir)
                Dim atom As Integer = ring.Atoms((edge + k) Mod n)

                If Not placed(atom) Then
                    px(atom) = x
                    py(atom) = y
                    placed(atom) = True
                End If

                currentX = x
                currentY = y
            Next

            Return True
        End Function

        Private Function clearScoreFor(px As Double(), py As Double(), placed As Boolean(), self As Integer, x As Double, y As Double, ring As Graph.Ring) As Double
            Dim score As Double = Double.MaxValue
            Dim skip As New HashSet(Of Integer)(ring.Atoms)
            Dim count As Integer = 0

            For i = 0 To placed.Length - 1
                If Not placed(i) OrElse skip.Contains(i) OrElse i = self Then Continue For

                Dim d As Double = Math.Sqrt((px(i) - x) ^ 2 + (py(i) - y) ^ 2)

                If d < score Then score = d
                count += 1
            Next

            If count = 0 Then Return 3.0

            Return score
        End Function

        ''' <summary>
        ''' 以环上唯一的已布局原子为基准（螺环）放置整个环。
        ''' </summary>
        Private Sub placeFromAtom(ring As Graph.Ring, anchor As Integer, px As Double(), py As Double(), placed As Boolean())
            Dim n As Integer = ring.Size
            Dim position As Integer = Array.IndexOf(ring.Atoms, anchor)
            Dim orientation As Integer = 1
            Dim bestScore As Double = Double.MinValue
            Dim startAngle As Double = 0

            For step = 0 To 23
                For Each turn As Integer In New Integer() {1, -1}
                    Dim angle As Double = step * Math.PI / 12
                    Dim score As Double = 0
                    Dim cx As Double = px(anchor)
                    Dim cy As Double = py(anchor)
                    Dim direction As Double = angle

                    For k = 1 To n - 1
                        If k > 1 Then direction += turn * 2 * Math.PI / n

                        Dim x As Double = cx + BOND_LENGTH * Math.Cos(direction)
                        Dim y As Double = cy + BOND_LENGTH * Math.Sin(direction)
                        Dim atom As Integer = ring.Atoms((position + k) Mod n)

                        If placed(atom) Then
                            score -= 20
                        Else
                            score += Math.Min(3.0, clearScoreFor(px, py, placed, atom, x, y, ring))
                        End If

                        cx = x
                        cy = y
                    Next

                    If score > bestScore Then
                        bestScore = score
                        startAngle = angle
                        orientation = turn
                    End If
                Next
            Next

            Dim currentX As Double = px(anchor)
            Dim currentY As Double = py(anchor)
            Dim dir As Double = startAngle

            For k = 1 To n - 1
                If k > 1 Then dir += orientation * 2 * Math.PI / n

                Dim x As Double = currentX + BOND_LENGTH * Math.Cos(dir)
                Dim y As Double = currentY + BOND_LENGTH * Math.Sin(dir)
                Dim atom As Integer = ring.Atoms((position + k) Mod n)

                If Not placed(atom) Then
                    px(atom) = x
                    py(atom) = y
                    placed(atom) = True
                End If

                currentX = x
                currentY = y
            Next
        End Sub

#End Region

#Region "取代基生长"

        Private Sub growSubstituents(graph As Graph.MoleculeGraph, px As Double(), py As Double(), placed As Boolean())
            Dim queue As New Queue(Of Integer)

            For i = 0 To graph.AtomCount - 1
                If placed(i) Then queue.Enqueue(i)
            Next

            Dim guard As Integer = 0

            While queue.Count > 0 AndAlso guard < graph.AtomCount * 64 + 64
                guard += 1

                Dim a As Integer = queue.Dequeue()

                If Not placed(a) Then Continue While

                For Each neighbor In graph.Neighbors(a)
                    Dim b As Integer = neighbor.neighbor

                    If placed(b) Then Continue For

                    Call placeNextTo(graph, a, b, px, py, placed)
                    placed(b) = True
                    queue.Enqueue(b)
                Next
            End While

            ' 剩下的（未成键的独立原子，如离子）沿圆周排布
            Dim index As Integer = 0

            For i = 0 To graph.AtomCount - 1
                If placed(i) Then Continue For

                Dim radius As Double = 3.0 + 1.5 * index
                px(i) = radius * Math.Cos(index * 2.4)
                py(i) = radius * Math.Sin(index * 2.4)
                placed(i) = True
                index += 1
            Next
        End Sub

        Private Sub placeNextTo(graph As Graph.MoleculeGraph, a As Integer, b As Integer,
                                px As Double(), py As Double(), placed As Boolean())

            Dim preferred As Double = 0
            Dim sumX As Double = 0
            Dim sumY As Double = 0
            Dim count As Integer = 0

            For Each neighbor In graph.Neighbors(a)
                Dim other As Integer = neighbor.neighbor

                If Not placed(other) OrElse other = b Then Continue For

                Dim dx As Double = px(other) - px(a)
                Dim dy As Double = py(other) - py(a)
                Dim len As Double = Math.Sqrt(dx * dx + dy * dy)

                If len < EPS Then Continue For

                sumX += dx / len
                sumY += dy / len
                count += 1
            Next

            If count = 0 Then
                preferred = 0
            Else
                preferred = Math.Atan2(-sumY, -sumX)

                If Math.Sqrt(sumX * sumX + sumY * sumY) < 0.2 Then
                    preferred = Math.Atan2(-sumY, -sumX) + Math.PI / 2
                End If
            End If

            Dim bestScore As Double = Double.MinValue
            Dim bestX As Double = px(a) + BOND_LENGTH * Math.Cos(preferred)
            Dim bestY As Double = py(a) + BOND_LENGTH * Math.Sin(preferred)

            For step = 0 To 23
                Dim offset As Double = (If(step Mod 2 = 0, 1, -1)) * ((step + 1) \ 2) * Math.PI / 12
                Dim angle As Double = preferred + offset
                Dim x As Double = px(a) + BOND_LENGTH * Math.Cos(angle)
                Dim y As Double = py(a) + BOND_LENGTH * Math.Sin(angle)
                Dim score As Double = Double.MaxValue

                For i = 0 To placed.Length - 1
                    If Not placed(i) OrElse i = a Then Continue For
                    If graph.BondOrder(a, i) > 0 Then Continue For

                    Dim d As Double = Math.Sqrt((px(i) - x) ^ 2 + (py(i) - y) ^ 2)

                    If d < score Then score = d
                Next

                If score = Double.MaxValue Then score = 10
                ' 偏好接近理想夹角的方向（每偏离 15° 扣 0.05）
                score -= Math.Abs(offset) * 0.08

                If score > bestScore Then
                    bestScore = score
                    bestX = x
                    bestY = y
                End If
            Next

            px(b) = bestX
            py(b) = bestY
        End Sub

#End Region

#Region "力导向精修"

        Private Sub refine(graph As Graph.MoleculeGraph, px As Double(), py As Double(), options As Graph.Layout2DOptions)
            Dim n As Integer = graph.AtomCount

            If n < 2 Then Return

            Dim bonds As New List(Of (a As Integer, b As Integer))

            For Each bond As Graph.Bond In graph.Bonds
                If bond.a <> bond.b Then bonds.Add((bond.a, bond.b))
            Next

            Dim angles As New List(Of (a As Integer, c As Integer, target As Double))

            For center = 0 To n - 1
                Dim neighbors As List(Of (neighbor As Integer, order As Double)) = graph.Neighbors(center)
                Dim target As Double = target13(graph, center)

                For i = 0 To neighbors.Count - 1
                    For j = i + 1 To neighbors.Count - 1
                        angles.Add((neighbors(i).neighbor, neighbors(j).neighbor, target))
                    Next
                Next
            Next

            Dim repulsion As New List(Of (a As Integer, b As Integer))
            Dim distance(graph.AtomCount - 1, graph.AtomCount - 1) As Integer

            Call graphDistances(graph, distance)

            For a = 0 To n - 1
                For b = a + 1 To n - 1
                    Dim d As Integer = distance(a, b)

                    If d = 3 Then repulsion.Add((a, b))
                Next
            Next

            Dim iterations As Integer = Math.Max(50, options.Iterations)
            Dim minSep As Double = options.MinSeparation * options.BondLength
            Dim goal As Double = options.BondLength

            For iteration = 1 To iterations
                ' 1) 键长约束
                For Each pair In bonds
                    Call relaxPair(px, py, pair.a, pair.b, goal, 0.5)
                Next

                ' 2) 1-3 距离约束（等价于键角约束）
                For Each item In angles
                    Call relaxPair(px, py, item.a, item.c, item.target * options.BondLength, 0.18)
                Next

                ' 3) 1-4 及更远的排斥（只推开过近的原子对）
                For Each pair In repulsion
                    Dim dx As Double = px(pair.b) - px(pair.a)
                    Dim dy As Double = py(pair.b) - py(pair.a)
                    Dim d As Double = Math.Sqrt(dx * dx + dy * dy)

                    If d < minSep AndAlso d > EPS Then
                        Dim push As Double = (minSep - d) / d * 0.25

                        px(pair.a) -= dx * push
                        py(pair.a) -= dy * push
                        px(pair.b) += dx * push
                        py(pair.b) += dy * push
                    End If
                Next
            Next
        End Sub

        Private Sub relaxPair(px As Double(), py As Double(), a As Integer, b As Integer, target As Double, weight As Double)
            Dim dx As Double = px(b) - px(a)
            Dim dy As Double = py(b) - py(a)
            Dim d As Double = Math.Sqrt(dx * dx + dy * dy)

            If d < EPS Then
                px(b) += 0.01
                Return
            End If

            Dim delta As Double = (d - target) / d * weight * 0.5

            px(a) += dx * delta
            py(a) += dy * delta
            px(b) -= dx * delta
            py(b) -= dy * delta
        End Sub

        Private Function target13(graph As Graph.MoleculeGraph, center As Integer) As Double
            Select Case graph.Hybridization(center)
                Case Graph.Hybridization.SP
                    Return 2.0
                Case Graph.Hybridization.SP2
                    Return 1.732
                Case Else
                    Return 1.633
            End Select
        End Function

        Private Sub graphDistances(graph As Graph.MoleculeGraph, distance As Integer(,))
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
                        Dim bond As Graph.Bond = graph.Bonds(id)
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

        Private Sub normalize(graph As Graph.MoleculeGraph, px As Double(), py As Double(), options As Graph.Layout2DOptions)
            Dim n As Integer = graph.AtomCount

            If n = 0 Then Return

            Dim total As Double = 0
            Dim count As Integer = 0

            For Each bond As Graph.Bond In graph.Bonds
                Dim d As Double = Math.Sqrt((px(bond.b) - px(bond.a)) ^ 2 + (py(bond.b) - py(bond.a)) ^ 2)

                If d > EPS Then
                    total += d
                    count += 1
                End If
            Next

            Dim scale As Double = If(count > 0 AndAlso total > EPS, options.BondLength * count / total, 1.0)

            Dim cx As Double = 0
            Dim cy As Double = 0

            For i = 0 To n - 1
                cx += px(i)
                cy += py(i)
            Next

            cx /= n
            cy /= n

            For i = 0 To n - 1
                px(i) = (px(i) - cx) * scale
                py(i) = (py(i) - cy) * scale
            Next
        End Sub

    End Module

End Namespace
