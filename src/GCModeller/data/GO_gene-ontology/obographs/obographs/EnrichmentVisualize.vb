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
                              Optional networkLayoutIteration% = 100) As GraphicsData

        If networkLayoutIteration > 0 Then
            Call VBDebugger.WaitOutput()
            Call Console.Clear()
            Call dag.doForceLayout(iterations:=networkLayoutIteration, showProgress:=True)
            Call Console.Clear()
        End If

        Return dag.DrawImage(
            canvasSize:=size,
            padding:=margin,
            background:=bg,
            labelerIterations:=-1
        )
    End Function
End Module
