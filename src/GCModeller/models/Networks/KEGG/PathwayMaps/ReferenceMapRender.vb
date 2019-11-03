Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Driver
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML.File

Namespace PathwayMaps

    Public Module ReferenceMapRender

        ''' <summary>
        ''' 将完成node和edge布局操作的网络模型进行渲染
        ''' </summary>
        ''' <param name="model"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Render(model As XGMMLgraph) As GraphicsData
            Dim graph As NetworkGraph = model.ToNetworkGraph
            Dim nodes As New Dictionary(Of String, Node)

            For Each node As Node In graph.vertex
                If node.label.IsPattern("C\d+") Then
                    node.data.color = Brushes.Red
                Else
                    node.data.color = Brushes.SkyBlue
                End If

                nodes.Add(node.label, node)
            Next

            Dim drawNode As DrawNodeShape =
                Sub(id$, g As IGraphics, br As Brush, radius!, center As PointF)
                    Dim node As Node = nodes(id)

                    If node.label.IsPattern("C\d+") Then
                        ' 圆形
                        Dim rect As New Rectangle With {
                            .X = center.X - radius / 2,
                            .Y = center.Y - radius / 2,
                            .Width = radius,
                            .Height = radius
                        }

                        Call g.FillEllipse(br, rect)
                    Else
                        ' 方形
                        Dim rect As New Rectangle With {
                            .X = center.X - radius / 2,
                            .Y = center.Y - radius / 4,
                            .Width = radius,
                            .Height = radius / 2
                        }

                        Call g.FillRectangle(br, rect)
                    End If
                End Sub

            Return NetworkVisualizer.DrawImage(
                net:=graph,
                canvasSize:="20480,19200",
                labelerIterations:=500,
                doEdgeBundling:=True,
                drawNodeShape:=drawNode
            )
        End Function
    End Module
End Namespace