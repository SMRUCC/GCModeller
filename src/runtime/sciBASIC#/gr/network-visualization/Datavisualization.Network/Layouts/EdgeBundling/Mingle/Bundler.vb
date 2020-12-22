Imports Microsoft.VisualBasic.Data.GraphTheory.KdTree
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports stdNum = System.Math

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

            kdTree = New KdTree(Of GraphKdNode)(
                nodeArray.ToArray,
                Function(a, b)
                    Dim diff0 = a.x - b.x
                    Dim diff1 = a.y - b.y
                    Dim diff2 = a.z - b.z
                    Dim diff3 = a.w - b.w

                    Return stdNum.Sqrt(diff0 * diff0 + diff1 * diff1 + diff2 * diff2 + diff3 * diff3)
                End Function, {"x", "y", "z", "w"})
        End Sub

        Public Sub buildNearestNeighborGraph(Optional k As Integer = 10)
            Dim node As KdTreeNode, dist As Double

            Call buildKdTree()

            For Each n As Node In graph.vertex
                Dim Nodes = kdTree.nearest(n, k)
                For i As Integer = 0 To Nodes.length - 1
                    node = Nodes(i).Item1
                    dist = Nodes(i).Item2
                    If (node.ID = n.ID) Then
                        graph.AddEdge(n, node)
                    End If
                Next
            Next
        End Sub
    End Class
End Namespace