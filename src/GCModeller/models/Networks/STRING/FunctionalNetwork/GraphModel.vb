Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Data.STRING

Public Module GraphModel

    <Extension>
    Public Function CreateGraph(edges As IEnumerable(Of InteractExports), nodes As IEnumerable(Of Coordinates), Optional factor# = 1500) As NetworkGraph
        Dim g As New NetworkGraph
        Dim gEdges As New List(Of Edge)
        Dim gNodes = nodes _
            .Select(Function(n)
                        Return New Node With {
                            .ID = n.node,
                            .Data = New NodeData With {
                                .label = n.node,
                                .Color = n.color.GetBrush,
                                .origID = n.node,
                                .initialPostion = New FDGVector2(n.x_position * factor, n.y_position * factor)
                            }
                        }
                    End Function) _
            .ToDictionary

        For Each edge As InteractExports In edges
            gEdges += New Edge With {
                .Source = gNodes(edge.node1),
                .Target = gNodes(edge.node2),
                .Data = New EdgeData With {
                    .weight = edge.combined_score
                }
            }
        Next

        g.nodes = gNodes.Values.AsList
        g.edges = gEdges

        Call g.ComputeNodeDegrees

        Return g
    End Function
End Module
