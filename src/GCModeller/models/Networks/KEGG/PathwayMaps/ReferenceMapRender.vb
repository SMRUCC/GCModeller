Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
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
            Dim g As NetworkGraph = model.ToNetworkGraph
            Dim img As GraphicsData = NetworkVisualizer.DrawImage(g)

            Return img
        End Function
    End Module
End Namespace