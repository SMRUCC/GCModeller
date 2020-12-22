Imports Microsoft.VisualBasic.Data.GraphTheory.KdTree
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Namespace Layouts.EdgeBundling.Mingle

    ' coords = data.size

    ''' <summary>
    ''' Edge bundling algorithm class.
    ''' </summary>
    Public Class Bundler

        ReadOnly graph As NetworkGraph
        ReadOnly kdTree As KdTree
        ReadOnly options As Options

        Public Sub setNodes(nodes As Node())
            Call graph.Clear()

            For i As Integer = 0 To nodes.Length
                graph.AddNode(nodes(i))
            Next
        End Sub

        Public Sub buildKdTree()
            Dim nodeArray As New List(Of KdTreeNode)

            For Each n In graph.vertex
                Dim coords = n.data.size
                n.x = coords(0)
                n.y = coords(1)
                n.z = coords(2)
                n.w = coords(3)
                nodeArray.Add(n)
            Next

            this.kdTree = New KdTree(nodeArray, Function(a, b) {
            var diff0 = a.x - b.x,
                diff1 = a.y - b.y,
                diff2 = a.z - b.z,
                diff3 = a.w - b.w;

            Return Math.sqrt(diff0 * diff0 + diff1 * diff1 + diff2 * diff2 + diff3 * diff3);
        }, ['x', 'y', 'z', 'w']);
    End Sub
    End Class
End Namespace