Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Scripting

Public Module EnrichPlot

    Public Function BubblePlot(data As Dictionary(Of String, NamedValue(Of Double)()),
                               Optional size$ = "1600,1200",
                               Optional padding$ = g.DefaultPadding,
                               Optional bg$ = "white",
                               Optional unenrichColor$ = "gray",
                               Optional enrichColorSchema$ = "darkblue,red,green") As Image

        Return g.GraphicsPlots(
            size.SizeParser, padding,
            bg,
            Sub(ByRef g, region)

            End Sub)
    End Function

    Private Sub __plotInternal(g As Graphics, region As GraphicsRegion, unenrich As Color, enrichColors As Color())

    End Sub
End Module
