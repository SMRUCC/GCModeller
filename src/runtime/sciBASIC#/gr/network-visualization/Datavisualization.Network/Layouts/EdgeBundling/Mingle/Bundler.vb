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
    End Class
End Namespace