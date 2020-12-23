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

        Public Function getMaxTurningAngleValue(node As Node, m1 As number(), m2 As number())
            Dim m2Tom1 = [m1[0] - m2[0], m1[1] - m2[1]],
            m1Tom2 = [-m2Tom1[0], -m2Tom1[1]],
            m1m2Norm = $norm(m2Tom1),
            angle = 0, nodes, vec As  number[], norm As  number, dot, angleValue,
            x, y, coords As  number[], i, l, n

        If (node.data.bundle || node.data.nodes)  Then
                Nodes = node.data.bundle?(node.data.bundle).data.nodes : node.data.nodes;
            For i As Integer = 0 To Nodes.length - 1
                    coords = nodes[i].data.coords;
                vec = [coords[0] - m1[0], coords[1] - m1[1]];
                norm = $norm(vec);
                dot = vec[0] * m2Tom1[0] + vec[1] * m2Tom1[1];
                angleValue = abs(acos(dot / norm / m1m2Norm));
                angle = angle < angleValue ? angleValue : angle;

                vec = [coords[2] - m2[0], coords[3] - m2[1]];
                norm = $norm(vec);
                dot = vec[0] * m1Tom2[0] + vec[1] * m1Tom2[1];
                angleValue = abs(acos(dot / norm / m1m2Norm));
                angle = angle < angleValue ? angleValue : angle;
             Next

                Return angle
            End If

            Return -1
        End Function

        Public Function getCombinedNode(node1 As Node, node2 As Node, data As NodeData) As Node
            node1 = node1.data.bundle || node1
        node2 = node2.data.bundle || node2

        Dim id = node1.ID & "-" & node2.ID,
            name = node1.name & "-" & node2.name,
            nodes1 = node1.data.nodes || [node1],
            nodes2 = node2.data.nodes || [node2],
            weight1 = node1.data.weight || 0,
            weight2 = node2.data.weight || 0,
            Nodes as  Node() = {}, ans as  Node

        If (node1.ID = node2.ID) Then
                Return node1
            End If
            Nodes.push.apply(Nodes, nodes1)
            Nodes.push.apply(Nodes, nodes2)

            data.nodes = Nodes
            data.nodeArray = (node1.data.nodeArray || []).concat(node2.data.nodeArray || [])
        data.weight = weight1 + weight2
            ans = New Node({
            id: id,
            name: name,
            data: data
        })

        computeIntermediateNodePositions(ans)

            Return ans
        End Function


        Public Function coalesceNodes(nodes As Node()) As Node
            Dim node = nodes[0],
            data = node.data,
            m1 = data.m1,
            m2 = data.m2,
            weight = nodes.reduce(Function(acum, n) { return acum + (n.data.weight || 0); }, 0),
            coords = data.coords,
            bundle as  Node =data.bundle,
            nodeArray as  Node() = {},
            i, l

        If Not m1 Is Nothing Then
                coords = [m1[0], m1[1], m2[0], m2[1]]

            ' flattened nodes for cluster.
            For i As Integer = 0 To nodes.Length - 1
                    nodeArray.push.apply(nodeArray, nodes[i].data.nodeArray || (nodes[i].data.parents ? [] :    [nodes[i]]));
            Next

                If options.sort Then
                    nodeArray.sort(options.sort)
                End If

                Return New Node With {
                    id: bundle.id,
                name: bundle.id,
                Data:   {
                    nodeArray: nodeArray,
                    parents: nodes,
                    coords: coords,
                    weight: weight,
                    parentsInk: bundle.data.ink
                }
            }
        End If

            Return nodes[0]
    End Function


        Public Function bundle(combinedNode As Node, node1 As Node, node2 As Node)
            node1.data.bundle = combinedNode
            node2.data.bundle = combinedNode

            node1.data.ink = combinedNode.data.ink
            node1.data.m1 = combinedNode.data.m1
            node1.data.m2 = combinedNode.data.m2
            ' node1.data.nodeArray = combinedNode.data.nodeArray

            node2.data.ink = combinedNode.data.ink
            node2.data.m1 = combinedNode.data.m1
            node2.data.m2 = combinedNode.data.m2
            ' node2.data.nodeArray = combinedNode.data.nodeArray
        End Function

        Public Sub updateGraph(graph As NetworkGraph, groupedNode As Node, nodes As Node(), ids As Dictionary(Of String, String))
            Dim n, connections
            Dim checkConnection = Function(e)
                                      Dim nodeToId = e.nodeTo.id
                                      If ids.ContainsKey(nodeToId) Then
                                          connections.push(e.nodeTo)
                                      End If
                                  End Function
            For i As Integer = 0 To nodes.Length - 1
                n = nodes(i)
                connections = {}
                n.eachEdge(checkConnection)
                graph.RemoveNode(n.id)
            Next
            graph.AddNode(groupedNode)
            For i As Integer = 0 To connections.length - 1
                graph.AddEdge(groupedNode, connections(i))
            Next
        End Sub

        Public Sub coalesceGraph()
            Dim newGraph = New NetworkGraph()
            groupsIds = {},
            maxGroup   as  integer = integer .NegativeInfinity ,
            Nodes 
            Dim ids As Dictionary(Of String, String), groupedNode, connections

            graph.each(Sub(node)
                           Dim group = node.data.group
                           If (maxGroup < group) Then
                               maxGroup = group
                           End If
                           If (!groupsIds[group]) 
                groupsIds[group] = {}
            End If
                           groupsIds[group][node.id] = node
        End Sub)

            maxGroup += 1
            Do While maxGroup > 0
                maxgroup -= 1

                ids = groupsIds[maxGroup]
            Nodes = []
            For Each i In ids.keys
                    Nodes.push(ids[i])
                Next
                If (Nodes.length) Then
                    groupedNode = coalesceNodes(Nodes)
                    updateGraph(graph, groupedNode, Nodes, ids)
                End If
            Loop
        End Sub

        Public Function getMaximumInkSavingNeighbor(n As Node)
            Dim nodeFrom = n,
            inkFrom = getInkValue(nodeFrom),
            inkTotal = Double.PositiveInfinity,
            bundle As Node() = Array(2),
            combinedBundle As Node

            n.eachEdge(Sub(e)
                           Dim nodeTo = e.nodeTo,
                inkTo = getInkValue(nodeTo),
                combined : Node = combineNodes(nodeFrom, nodeTo),
                inkUnion = getInkValue(combined),
                inkValue = inkUnion - (inkFrom + inkTo)

                           If (inkTotal > inkValue) Then
                               inkTotal = inkValue
                               bundle()[0] = nodeFrom
                bundle()[1] = nodeTo
                combinedBundle = combined
                           End If
                       End Sub)

            Return {
            bundle(): bundle,
            inkTotal: inkTotal,
            combined: combinedBundle
        }
    End Function

        Public Sub MINGLE()
            Dim edgeProximityGraph As NetworkGraph = graph,
            that = Me,
            totalGain = 0,
            ungrouped = -1,
            gain = 0,
            k = 0,
            clean = Sub(n) n.data.group = ungrouped,
            nodeMingle = Sub(node As Node)
                             If (node.data.group = ungrouped) Then
                                 Dim ans = that.getMaximumInkSavingNeighbor(node),
                        bundle = ans.bundle,
                        u = bundle()[0],
                        v = bundle()[1],
                        combined = ans.combined,
                        gainUV = -ans.inkTotal

                                 ' graph has been collapsed And Is now only one node
                                 If (!u && !v) Then
                                     gain = Double.NegativeInfinity
                                     Return
                                 End If

                                 If (gainUV > 0) Then
                                     that.bundle(combined, u, v)
                                     gain += gainUV
                                     If (v.data.group! = ungrouped) Then
                                         u.data.group = v.data.group;
                         Else
                                         u.data.group = v.data.group = k;
                        End If
                                 Else
                                     u.data.group = k
                                 End If
                                 k += 1
                             End If
                         End Sub

            Loop
                             gain = 0
                             k = 0
                             edgeProximityGraph.each(clean)
                             edgeProximityGraph.each(nodeMingle)
                             this.coalesceGraph()
                             totalGain += gain
                             While gain > 0
    End Sub
    End Class
End Namespace