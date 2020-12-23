Imports Microsoft.VisualBasic.Data.GraphTheory.KdTree
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports stdNum = System.Math
Imports number = System.Double

Namespace Layouts.EdgeBundling.Mingle

    ' coords = data.size

    ''' <summary>
    ''' Edge bundling algorithm class.
    ''' </summary>
    Public Class Bundler

        ReadOnly graph As NetworkGraph
        ReadOnly options As Options

        Dim kdTree As KdTree(Of GraphKdNode)

        Public Sub setNodes(nodes As Node())
            Call graph.Clear()

            For i As Integer = 0 To nodes.Length
                graph.AddNode(nodes(i))
            Next
        End Sub

        Public Sub buildKdTree()
            Dim nodeArray As New List(Of GraphKdNode)

            For Each v In graph.vertex
                Dim coords = v.data.size
                Dim n = New GraphKdNode(v)

                n.x = coords(0)
                n.y = coords(1)
                n.z = coords(2)
                n.w = coords(3)
                nodeArray.Add(n)
            Next

            kdTree = New KdTree(Of GraphKdNode)(nodeArray.ToArray, New Accessor)
        End Sub

        Public Sub buildNearestNeighborGraph(Optional k As Integer = 10)
            Dim node As KdTreeNode(Of GraphKdNode), dist As Double

            Call buildKdTree()

            For Each n As Node In graph.vertex
                Dim nodes As KdNodeHeapItem(Of GraphKdNode)() = kdTree.nearest(n, k).ToArray

                For i As Integer = 0 To nodes.Length - 1
                    node = nodes(i).node
                    dist = nodes(i).distance

                    If (node.ID = n.ID) Then
                        graph.AddEdge(n, node)
                    End If
                Next
            Next
        End Sub

        Public Sub computeIntermediateNodePositions(node As Node)
            Dim m1, m2, centroids As Double()()
            Dim a As number, b As number, c As number, tau As number
            Dim f, res

            If Not node.data.nodes Then
                Return
            End If
            centroids = getCentroids(node.data.nodes)
            f = Function(x) costFunction(node, centroids, x)
            a = 0
            b = 1
            c = 0.72 ' because computers
            tau = 0.1
            res = goldenSectionSearch(a, b, c, tau, f)
            f(res) ' Set m1 And m2;
        End Sub


        Public Function costFunction(node As Node, centroids As number()(), x As number) As number
            Dim top, bottom, m1, m2, ink, alpha, p As Double
            x /= 2
            top = centroids(0)
            bottom = centroids(1)
            m1 = $lerp(top, bottom, x)
        m2 = $lerp(top, bottom, 1 - x)
        node.data.m1 = m1
            node.data.m2 = m2
            ' Delete node.data.ink
            ink = getInkValue(node)
            alpha = getMaxTurningAngleValue(node, m1, m2)
            p = options.angleStrength || 1.2
        Return ink * (1 + stdNum.Sin(alpha) / p)
        End Function

        Public Function goldenSectionSearch(a As number, b As number, c As number, tau As number, f) As number
            Dim phi = phi,
            resphi = 2 - phi,
             x As number

            If (c - b > b - a) Then
                x = b + resphi * (c - b)
            Else
                x = b - resphi * (b - a)
            End If
            If (stdNum.Abs(c - a) < tau * (stdNum.Abs(b) + stdNum.Abs(x))) Then
                Return (c + a) / 2
            End If
            If (f(x) < f(b)) Then
                If (c - b > b - a) Then
                    Return goldenSectionSearch(b, x, c, tau, f)
                End If
                Return goldenSectionSearch(a, x, b, tau, f)
            End If
            If (c - b > b - a) Then
                Return goldenSectionSearch(a, b, x, tau, f)
            End If
            Return goldenSectionSearch(x, b, c, tau, f)
        End Function

        Public Function getCentroids(nodes As Node()) As number()()
            Dim topCentroid As Double() = {0, 0},
            bottomCentroid As Double() = {0, 0},
            coords As number()
            Dim l As Integer = nodes.Length

            For i As Integer = 0 To nodes.Length - 1
                coords = nodes(i).data.coords
                topCentroid(0) += coords(0)
                topCentroid(1) += coords(1)
                bottomCentroid(0) += coords(2)
                bottomCentroid(1) += coords(3)
            Next

            topCentroid(0) /= l
            topCentroid(1) /= l
            bottomCentroid(0) /= l
            bottomCentroid(1) /= l

            Return {topCentroid, bottomCentroid}
        End Function

        Public Function getInkValue(Node As Node, Optional depth As number = 0)
            Dim data = Node.data,
            coords, diffX, diffY,
            m1, m2, acum As number, l As Integer, nodes As Node(),
            ni As Node

            ' bundled node
            If (!depth && (data.bundle || data.nodes))  Then
                nodes = data.bundle ? data.bundle.data.nodes : data.nodes
                m1 = data.m1
                m2 = data.m2
                acum = 0
                l = nodes.Length

                For i As Integer = 0 To nodes.Length - 1
                    ni = nodes(i)
                    coords = ni.data.coords
                    diffX = m1(0) - coords(0)
                    diffY = m1(1) - coords(1)
                    acum += $norm((diffX, diffY))
                diffX = m2(0) - coords(2)
                    diffY = m2(1) - coords(3)
                    acum += $norm((diffX, diffY))
                acum += getInkValue(ni, depth + 1)
                Next
                If (!depth) Then
                    acum += $dist(m1, m2)
            End If
                Return (Node.data.ink = acum)
        End If

            ' coalesced node
            If (data.parents) Then
                nodes = data.parents
                m1 = (data.coords(0), data.coords(1))
                m2 = (data.coords(2), data.coords(3))
                acum = 0
                For i As Integer = 0 To nodes.Length - 1
                    ni = nodes(i)
                    coords = ni.data.coords
                    diffX = m1(0) - coords(0)
                    diffY = m1(1) - coords(1)
                    acum += $norm((diffX, diffY))
                diffX = m2(0) - coords(2)
                    diffY = m2(1) - coords(3)
                    acum += $norm((diffX, diffY))
                acum += this.getInkValue(ni, depth + 1)
                Next
                ' only add the distance if this Is the first recursion
                If (!depth) Then
                    acum += $dist(m1, m2)
            End If
                Return (Node.data.ink = acum)
            End If

            ' simple node
            If (depth) Then
                Return (Node.data.ink = 0)
            End If
            coords = Node.data.coords
            diffX = coords(0) - coords(2)
            diffY = coords(1) - coords(3)
            Return (Node.data.ink = $norm((diffX, diffY)))
    End Function
    End Class
End Namespace