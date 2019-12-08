Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver

Public Module EnrichmentVisualize

    Public Function DrawGraph(dag As NetworkGraph,
                              Optional size$ = "3000,4000",
                              Optional bg$ = "white",
                              Optional margin$ = g.DefaultPadding,
                              Optional doNetworkLayout As Boolean = True) As GraphicsData

        If doNetworkLayout Then
            Call dag.doForceLayout
        End If

        Return dag.DrawImage(
            canvasSize:=size,
            padding:=margin,
            background:=bg
        )
    End Function
End Module
