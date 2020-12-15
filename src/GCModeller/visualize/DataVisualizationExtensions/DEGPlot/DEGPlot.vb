Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver

Public Module DEGPlot

    <Extension>
    Public Function ClassChangePlot(deg As IEnumerable(Of DEGModel),
                                    Optional size$ = "2700,2100",
                                    Optional padding$ = g.DefaultPadding,
                                    Optional bg$ = "white",
                                    Optional colorSet$ = "Set1:c12",
                                    Optional dpi% = 300) As GraphicsData

        Dim theme As New Theme With {
            .padding = padding,
            .colorSet = colorSet,
            .background = bg
        }

        Return New ClassChanges(deg, theme).Plot(size, ppi:=dpi)
    End Function
End Module
