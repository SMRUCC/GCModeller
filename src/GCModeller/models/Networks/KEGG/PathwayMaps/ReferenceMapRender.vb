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

            For Each node As Node In graph.vertex
                If node.label.IsPattern("C\d+") Then
                    node.data.color = Brushes.Red
                Else
                    node.data.color = Brushes.SkyBlue
                End If
            Next

            Dim drawNode As DrawNodeShape =
                Sub(id$, g As IGraphics, br As Brush, radius!, center As PointF)

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