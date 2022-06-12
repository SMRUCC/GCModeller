Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Text
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports stdNum = System.Math

Namespace CatalogProfiling

    Public Class CatalogBubbleHeatmap : Inherits MultipleCatalogHeatmap

        ReadOnly cellRange As DoubleRange

        Public Sub New(multiples As IEnumerable(Of NamedValue(Of Dictionary(Of String, BubbleTerm()))),
                       mapLevels As Integer,
                       cellRange As DoubleRange,
                       theme As Theme)

            Call MyBase.New(multiples, mapLevels, "black", theme)

            Me.cellRange = cellRange
        End Sub

        Private Iterator Function getSampleMatrix() As IEnumerable(Of NamedValue(Of Dictionary(Of String, BubbleTerm)))
            For Each sample In multiples
                Yield New NamedValue(Of Dictionary(Of String, BubbleTerm)) With {
                    .Name = sample.Name,
                    .Description = sample.Description,
                    .Value = sample.Value.Values _
                        .IteratesALL _
                        .ToDictionary(Function(v)
                                          Return v.termId
                                      End Function)
                }
            Next
        End Function

        Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
            Dim pathways As String() = getPathways().Values.IteratesALL.Distinct.ToArray
            Dim pathwayNameFont As Font = CSSFont.TryParse(theme.axisTickCSS).GDIObject(g.Dpi)
            Dim pad = g.MeasureString("A", pathwayNameFont)
            Dim labelSize As SizeF
            Dim pvalues As DoubleRange = multiples _
                .Select(Function(v) v.Value.Values) _
                .IteratesALL _
                .IteratesALL _
                .Select(Function(b) b.PValue) _
                .Range
            Dim impacts As DoubleRange = multiples _
                .Select(Function(v) v.Value.Values) _
                .IteratesALL _
                .IteratesALL _
                .Select(Function(p) p.data) _
                .Range
            Dim viz As IGraphics = g
            Dim maxTag As SizeF = pathways _
                .Select(Function(str)
                            Return viz.MeasureString(str, pathwayNameFont)
                        End Function) _
                .OrderByDescending(Function(sz) sz.Width) _
                .First
            Dim region As New Rectangle With {
                .X = canvas.PlotRegion.Left + maxTag.Width * 1.125,
                .Y = canvas.PlotRegion.Top,
                .Width = canvas.PlotRegion.Width - maxTag.Width * 1.125,
                .Height = canvas.PlotRegion.Height
            }
            Dim paints As SolidBrush() = Designer _
                .GetColors(theme.colorSet, mapLevels) _
                .Select(Function(c) New SolidBrush(c)) _
                .ToArray
            Dim colorIndex As DoubleRange = New Double() {0, paints.Length - 1}
            Dim matrix = getSampleMatrix.ToArray
            Dim dx As Double = region.Width / matrix.Length
            Dim dy As Double = region.Height / pathways.Length
            Dim cellSize As New SizeF With {
                .Width = stdNum.Min(dx, dy),
                .Height = stdNum.Min(dx, dy)
            }
            Dim x, y As Double
            Dim gridStroke As Pen = Stroke.TryParse(theme.gridStrokeX).GDIObject

            x = region.Left + dx / 2
            y = region.Top + dy / 2
            g.DrawRectangle(Stroke.TryParse(theme.axisStroke).GDIObject, region)

            For Each sample In multiples
                Call g.DrawLine(gridStroke, New PointF(x, region.Top), New PointF(x, region.Bottom))
                x += dx
            Next

            For Each pid As String In pathways
                x = region.Left + dx / 2

                Call g.DrawLine(gridStroke, New PointF(region.Left, y), New PointF(region.Right, y))
                Call g.DrawLine(Stroke.TryParse(theme.axisStroke).GDIObject, New PointF(region.Left, y), New PointF(region.Left - pad.Width / 2, y))

                For Each sample In matrix
                    Dim bubble As BubbleTerm = sample.Value.TryGetValue(pid)

                    If Not bubble Is Nothing Then
                        Dim color As SolidBrush = paints(CInt(pvalues.ScaleMapping(bubble.PValue, colorIndex)))
                        Dim radius As Double = impacts.ScaleMapping(bubble.data, cellRange)

                        Call g.DrawCircle(New PointF(x, y), radius, color)
                    End If

                    x += dx
                Next

                labelSize = g.MeasureString(pid, pathwayNameFont)
                x = region.Left - labelSize.Width - dx / 2
                g.DrawString(pid, pathwayNameFont, Brushes.Black, New PointF(x, y - dy / 2 + (dy - pad.Height) / 2))

                y += dy
            Next

            ' draw sample labels
            x = region.Left + dx / 2
            y -= dy / 3

            Dim text As New GraphicsText(DirectCast(g, Graphics2D).Graphics)

            For Each sample In multiples
                text.DrawString(sample.Name, pathwayNameFont, Brushes.Black, New PointF(x, y), angle:=45)
                x += dx
            Next

            Call DrawMainTitle(g, region)
            Call MultipleBubble.drawRadiusLegend(g, impacts, cellRange, canvas, theme)
            Call drawColorLegends(
                pvalues:=pvalues,
                right:=region.Right + pad.Width * 1.125,
                g:=g,
                canvas:=canvas,
                y:=region.Top + region.Height / 3
            )
        End Sub
    End Class
End Namespace