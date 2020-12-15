Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS

Public Module DEGPlot

    <Extension>
    Public Function ClassChangePlot(deg As IEnumerable(Of DEGModel),
                                    Optional size$ = "2700,2100",
                                    Optional padding$ = g.DefaultPadding,
                                    Optional bg$ = "white",
                                    Optional colorSet$ = "Set1:c9",
                                    Optional axisTickCSS$ = CSSFont.Win10Normal,
                                    Optional axisStroke$ = Stroke.AxisStroke,
                                    Optional axisTickStroke$ = Stroke.AxisStroke,
                                    Optional axisLabelCSS$ = CSSFont.Win10NormalLarger,
                                    Optional labelCSS$ = CSSFont.Win7Normal,
                                    Optional radius As String = "5,30",
                                    Optional xlab$ = "X",
                                    Optional dpi% = 300) As GraphicsData

        Dim theme As New Theme With {
            .padding = padding,
            .colorSet = colorSet,
            .background = bg,
            .axisTickCSS = axisTickCSS,
            .axisStroke = axisStroke,
            .axisTickStroke = axisTickStroke,
            .axisLabelCSS = axisLabelCSS,
            .tagCSS = labelCSS,
            .xlabel = xlab
        }

        Return New ClassChanges(
            deg:=deg,
            radius:=radius,
            theme:=theme
        ).Plot(size, ppi:=dpi)
    End Function
End Module
