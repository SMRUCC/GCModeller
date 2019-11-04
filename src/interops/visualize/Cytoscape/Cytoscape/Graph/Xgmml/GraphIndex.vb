Imports System.Drawing
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML.File

Namespace CytoscapeGraphView.XGMML

    Public Class GraphIndex

        Dim graph As XGMMLgraph
        Dim nodeTable As Dictionary(Of String, XGMMLnode)

        Sub New(g As XGMMLgraph)
            graph = g
            nodeTable = g.nodes.ToDictionary(Function(n) n.label)
        End Sub

        Public Function GetEdgeNodes(edge As XGMMLedge) As (source As XGMMLnode, target As XGMMLnode)
            Dim s = GetNode(edge.source)
            Dim t = GetNode(edge.target)

            Return (s, t)
        End Function

        Public Function GetEdgeBends(edge As XGMMLedge) As PointF()
            Dim [handles] As Handle() = edge.graphics.edgeBendHandles
            Dim s = GetNode(edge.source)
            Dim t = GetNode(edge.target)
            Dim sx = s.graphics.x
            Dim sy = s.graphics.y
            Dim tx = t.graphics.x
            Dim ty = t.graphics.y
            Dim bends As PointF() = [handles] _
                .Select(Function(b)
                            If b.isDirectPoint Then
                                Return b.originalLocation
                            Else
                                Return b.convert(sx, sy, tx, ty)
                            End If
                        End Function) _
                .ToArray

            Return bends
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="label">Synonym</param>
        ''' <returns></returns>
        Public Function GetNode(label As String) As XGMMLnode
            Return nodeTable.TryGetValue(label)
        End Function

        Public Function GetNode(ID As Long) As XGMMLnode
            Return LinqAPI.DefaultFirst(Of XGMMLnode) _
 _
                () <= From node As XGMMLnode
                      In graph.nodes
                      Where node.id = ID
                      Select node

        End Function

        Public Function DeleteDuplication() As XGMMLgraph
            Dim sw As Stopwatch = Stopwatch.StartNew
            Dim edges = graph.edges

            Call $"{NameOf(edges)}:={edges.Length} in the network model...".__DEBUG_ECHO
            graph.edges = Distinct(graph.edges)
            Call $"{NameOf(edges)}:={edges.Length} left after remove duplicates in {sw.ElapsedMilliseconds}ms....".__DEBUG_ECHO

            Return graph
        End Function

        Public Function ExistEdge(Edge As XGMMLedge) As Boolean
            Return Not (GetNode(Edge.source) Is Nothing OrElse GetNode(Edge.target) Is Nothing)
        End Function
    End Class
End Namespace