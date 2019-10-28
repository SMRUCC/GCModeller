Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS

Namespace BarPlot

    ''' <summary>
    ''' 只针对单组数据的条形图绘制
    ''' </summary>
    Public Module LevelBarplot

        Public Function Plot(data As NamedValue(Of Double)(),
                             Optional size$ = "2000,1600",
                             Optional margin$ = Resolution2K.PaddingWithRightLegend,
                             Optional bg$ = "white",
                             Optional title$ = "BarPlot",
                             Optional titleFontCSS$ = CSSFont.Win7VeryLarge) As GraphicsData

        End Function
    End Module
End Namespace