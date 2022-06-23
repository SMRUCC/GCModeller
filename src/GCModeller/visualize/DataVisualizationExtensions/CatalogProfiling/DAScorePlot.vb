Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D

Namespace CatalogProfiling

    ''' <summary>
    ''' 横坐标是DA-score值，公式为DA-score=（上调物质数-下调物质数）/该通路上差异总物质数，纵坐标是代谢通路，柱形顶部点大小表示该通路上富集的差异代谢物数目。
    ''' </summary>
    Public Class DAScorePlot : Inherits Plot

        Public Sub New(theme As Theme)
            MyBase.New(theme)
        End Sub

        Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
            Throw New NotImplementedException()
        End Sub
    End Class
End Namespace