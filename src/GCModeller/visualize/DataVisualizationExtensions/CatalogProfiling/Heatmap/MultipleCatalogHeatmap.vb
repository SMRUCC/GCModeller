Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.MIME.Html.CSS

Namespace CatalogProfiling

    Public MustInherit Class MultipleCatalogHeatmap : Inherits MultipleCategoryProfiles

        Protected ReadOnly mapLevels As Integer
        Protected ReadOnly colorMissing As String

        Protected Sub New(multiples As IEnumerable(Of NamedValue(Of Dictionary(Of String, BubbleTerm()))),
                          mapLevels As Integer,
                          colorMissing As String,
                          theme As Theme
            )

            Call MyBase.New(multiples, theme)

            Dim orders As String() = TreeOrder.OrderByTree(Me)

            Me.mapLevels = mapLevels
            Me.colorMissing = colorMissing

            Call Me.ReOrder(orders)
        End Sub

        Protected Sub drawColorLegends(pvalues As DoubleRange, right As Double, ByRef g As IGraphics, canvas As GraphicsRegion, Optional y As Double = Double.NaN)
            Dim maps As New ColorMapLegend(palette:=theme.colorSet, mapLevels) With {
                .Format = "F2",
                .noblank = False,
                .tickAxisStroke = Stroke.TryParse(theme.legendTickAxisStroke).GDIObject,
                .tickFont = CSSFont.TryParse(theme.legendTickCSS).GDIObject(g.Dpi),
                .ticks = pvalues.CreateAxisTicks,
                .title = "-log10(pvalue)",
                .titleFont = CSSFont.TryParse(theme.legendTitleCSS).GDIObject(g.Dpi),
                .unmapColor = colorMissing,
                .ruleOffset = 5,
                .legendOffsetLeft = 5
            }
            Dim layout As New Rectangle With {
                .X = right,
                .Width = canvas.Padding.Right * (2 / 3),
                .Height = canvas.PlotRegion.Height / 2,
                .y = If(y.IsNaNImaginary, canvas.Padding.Top, y)
            }

            Call maps.Draw(g, layout)
        End Sub
    End Class

End Namespace