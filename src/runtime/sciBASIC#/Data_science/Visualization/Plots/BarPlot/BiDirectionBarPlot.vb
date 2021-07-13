
Imports System.Drawing
Imports Microsoft.VisualBasic.Data.ChartPlots.BarPlot.Data
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend

Namespace BarPlot

    ''' <summary>
    ''' compare two data set
    ''' </summary>
    Public Class BiDirectionBarPlot : Inherits Plot

        ReadOnly data As BiDirectionData
        ReadOnly colorFactor1 As SolidBrush
        ReadOnly colorFactor2 As SolidBrush

        Public Sub New(data As BiDirectionData, color1 As Color, color2 As Color, theme As Theme)
            MyBase.New(theme)

            Me.data = data
            Me.colorFactor1 = New SolidBrush(color1)
            Me.colorFactor2 = New SolidBrush(color2)
        End Sub

        Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
            Dim rect As Rectangle = canvas.PlotRegion
            Dim dh As Double = rect.Height / data.size
            Dim barHeight As Double = dh * 0.8
            Dim labelFont As Font = CSSFont.TryParse(theme.axisLabelCSS)
            Dim maxLen As Double = g.MeasureString(data.samples.Select(Function(d) d.tag).MaxLengthString, labelFont).Width
            Dim boxLeft As Double = rect.Left + maxLen
            Dim boxWidth As Double = rect.Right - boxLeft
            Dim center As Double = boxLeft + boxWidth / 2
            Dim dataValues = data.samples.Select(Function(d) d.data).IteratesALL.Range(scale:=1.125)
            Dim scale = d3js.scale.linear().domain({dataValues.Min, dataValues.Max}).range({0.0, center})

            Call g.DrawRectangle(Stroke.TryParse(theme.axisStroke).GDIObject, rect)
            Call g.DrawLine(Stroke.TryParse(theme.gridStrokeY).GDIObject, New PointF(center, rect.Top), New PointF(center, rect.Bottom))

            Dim y As Double = rect.Top

            For i As Integer = 0 To data.size - 1
                Dim sample As BarDataSample = data(i)

                y += dh

                ' draw left
                Dim len1 = scale(sample.data(0))
                Dim bar As New Rectangle(center - len1, y, len1, barHeight)

                Call g.FillRectangle(colorFactor1, bar)

                Dim len2 = scale(sample.data(1))

                bar = New Rectangle(center, y, len2, barHeight)
                g.FillRectangle(colorFactor2, bar)

                ' draw label
                Dim labelSize As SizeF = g.MeasureString(sample.tag, labelFont)
                Dim labelPos As New Point With {
                    .X = boxLeft * 10 - labelSize.Width,
                    .Y = y + (dh - labelSize.Height) / 2
                }

                Call g.DrawString(sample.tag, labelFont, Brushes.Black, labelPos)
            Next

            Dim legends As LegendObject() = {
                New LegendObject With {.color = colorFactor1.Color.ToHtmlColor, .fontstyle = theme.legendLabelCSS, .style = LegendStyles.Square, .title = data.Factor1},
                New LegendObject With {.color = colorFactor2.Color.ToHtmlColor, .fontstyle = theme.legendLabelCSS, .style = LegendStyles.Square, .title = data.Factor2}
            }

            Call DrawLegends(g, legends, canvas)
        End Sub
    End Class
End Namespace