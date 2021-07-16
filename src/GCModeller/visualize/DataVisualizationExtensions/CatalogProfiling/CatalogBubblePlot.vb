Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports SMRUCC.genomics.ComponentModel.Annotation

Namespace CatalogProfiling

    Public Class BubbleTerm

        ''' <summary>
        ''' [X]
        ''' </summary>
        ''' <returns></returns>
        Public Property Factor As Double
        ''' <summary>
        ''' [Y] -log10(p-value)
        ''' </summary>
        ''' <returns></returns>
        Public Property PValue As Double
        ''' <summary>
        ''' bubble radius
        ''' </summary>
        ''' <returns></returns>
        Public Property data As Double
        Public Property termId As String

    End Class

    Public Class CatalogBubblePlot : Inherits Plot

        ReadOnly showBubbleBorder As Boolean
        ReadOnly data As Dictionary(Of String, BubbleTerm())
        ReadOnly enrichColors As Dictionary(Of String, Color())

        Public Sub New(data As Dictionary(Of String, BubbleTerm()),
                       enrichColors As Dictionary(Of String, Color()),
                       showBubbleBorder As Boolean,
                       theme As Theme)

            MyBase.New(theme)

            Me.data = data
            Me.showBubbleBorder = showBubbleBorder
            Me.enrichColors = enrichColors
        End Sub

        Private Iterator Function GetCatalogSerialData() As IEnumerable(Of SerialData)
            Return data _
            .Keys _
            .Select(Function(category)
                        ' 这些都是经过筛选的，pvalue阈值符合条件的，
                        ' 剩下的pvalue阈值不符合条件的都被当作为同一个serials
                        Dim color As Color() = enrichColors(category) _
                            .Skip(20) _
                            .Alpha(250) _
                            .ToArray
                        Dim terms = result(category).AsList
                        Dim serial = terms.createModel(category, color, pvalue, r, displays, correlatedPvalue)

                        Return serial
                    End Function) _
            .Join() _
            .ToArray

            Yield data.Values _
                .IteratesALL _
                .unenrichSerial(
                    pvalue:=pvalue,
                    color:=unenrich,
                    r:=r,
                    correlatedPvalue:=correlatedPvalue
                )
        End Function

        Private Function unenrichSerial(catalog As IEnumerable(Of BubbleTerm), pvalue#, color As Color, r As Func(Of Double, Double)) As SerialData
            Dim unenrichs = catalog.Where(Function(term) term.PValue > pvalue).ToArray
            Dim points = unenrichs _
            .Select(Function(gene)
                        Return New PointData With {
                            .value = r(gene.number) + 1,
                            .pt = New PointF(x:=gene.number / gene.Backgrounds, y:=gene.P)
                        }
                    End Function) _
            .ToArray

            Return New SerialData With {
            .color = color,
            .title = "Unenrich terms",
            .pts = points
        }
        End Function

        Protected Overrides Sub PlotInternal(ByRef g As IGraphics, region As GraphicsRegion)
            Dim serials As SerialData() = GetCatalogSerialData().ToArray
            Dim bubbleBorder As Stroke = Nothing

            If showBubbleBorder Then
                bubbleBorder = New Stroke With {
                .dash = DashStyle.Solid,
                .fill = "lightgray",
                .width = 1.5
            }
            End If

            Dim plot As GraphicsData = Bubble.Plot(
                serials,
                padding:=theme.padding,
                size:=$"{region.Size.Width},{region.Size.Height}",
                legend:=False,
                xlabel:="richFactor=(n/background)",
                ylabel:="-log10(p.value)",
                bubbleBorder:=bubbleBorder,
                strokeColorAsMainColor:=True,
                axisLabelFontCSS:=CSSFont.Win10NormalLarge,
                positiveRangeY:=True
            )

            Call g.DrawImageUnscaled(plot, New Point)
            Call DrawBubbleLegends(g, serials, region)

            Dim titleFont As Font = CSSFont.TryParse(theme.mainCSS).GDIObject
            Dim fsize As SizeF = g.MeasureString(main, titleFont)
            Dim tloc As New PointF With {
                .X = (region.Size.Width - fsize.Width) / 2,
                .Y = (region.Padding.Top - fsize.Height) / 2
            }

            Call g.DrawString(main, titleFont, Brushes.Black, tloc)
        End Sub

        Private Sub DrawBubbleLegends(g As IGraphics, serials As SerialData(), region As GraphicsRegion)
            Dim legendFontStyle As String = theme.legendLabelCSS
            Dim legends As LegendObject() = serials _
            .Select(Function(s)
                        Return New LegendObject With {
                            .color = s.color.RGBExpression,
                            .fontstyle = legendFontStyle,
                            .style = LegendStyles.Circle,
                            .title = s.title
                        }
                    End Function) _
            .ToArray
            Dim legendFont As Font = CSSFont.TryParse(legendFontStyle)
            Dim maxWidth As Single = legends _
            .Select(Function(l)
                        Return g.MeasureString(l.title, legendFont).Width
                    End Function) _
            .Max
            Dim ltopLeft As New Point With {
            .X = Plot.Width - maxWidth * 1.2,
            .Y = region.PlotRegion.Top + (region.PlotRegion.Height - (g.MeasureString("0", legendFont).Height + 10) * 3) / 2
        }

            Call g.DrawLegends(
            ltopLeft,
            legends,
            gSize:="60,35",
            regionBorder:=New Stroke With {
                .fill = "Black",
                .dash = DashStyle.Solid,
                .width = 2
            })
        End Sub
    End Class
End Namespace