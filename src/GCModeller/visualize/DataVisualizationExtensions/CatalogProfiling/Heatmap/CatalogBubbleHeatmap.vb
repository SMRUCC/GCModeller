#Region "Microsoft.VisualBasic::97a97defd74ef7f3aaa46e155b66344f, visualize\DataVisualizationExtensions\CatalogProfiling\Heatmap\CatalogBubbleHeatmap.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 171
    '    Code Lines: 148 (86.55%)
    ' Comment Lines: 1 (0.58%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 22 (12.87%)
    '     File Size: 7.48 KB


    '     Class CatalogBubbleHeatmap
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: getSampleMatrix
    ' 
    '         Sub: PlotInternal
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.MIME.Html.Render
Imports std = System.Math

#If NET48 Then
Imports Pen = System.Drawing.Pen
Imports Pens = System.Drawing.Pens
Imports Brush = System.Drawing.Brush
Imports Font = System.Drawing.Font
Imports Brushes = System.Drawing.Brushes
Imports SolidBrush = System.Drawing.SolidBrush
Imports DashStyle = System.Drawing.Drawing2D.DashStyle
Imports Image = System.Drawing.Image
Imports Bitmap = System.Drawing.Bitmap
Imports GraphicsPath = System.Drawing.Drawing2D.GraphicsPath
Imports FontStyle = System.Drawing.FontStyle
#Else
Imports Pen = Microsoft.VisualBasic.Imaging.Pen
Imports Pens = Microsoft.VisualBasic.Imaging.Pens
Imports Brush = Microsoft.VisualBasic.Imaging.Brush
Imports Font = Microsoft.VisualBasic.Imaging.Font
Imports Brushes = Microsoft.VisualBasic.Imaging.Brushes
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush
Imports DashStyle = Microsoft.VisualBasic.Imaging.DashStyle
Imports Image = Microsoft.VisualBasic.Imaging.Image
Imports Bitmap = Microsoft.VisualBasic.Imaging.Bitmap
Imports GraphicsPath = Microsoft.VisualBasic.Imaging.GraphicsPath
Imports FontStyle = Microsoft.VisualBasic.Imaging.FontStyle
#End If

Namespace CatalogProfiling

    Public Class CatalogBubbleHeatmap : Inherits MultipleCatalogHeatmap

        ReadOnly cellRange As DoubleRange

        Public Sub New(multiples As IEnumerable(Of NamedValue(Of Dictionary(Of String, BubbleTerm()))),
                       mapLevels As Integer,
                       cellRange As DoubleRange,
                       theme As Theme)

            Call MyBase.New(multiples, mapLevels, colorMissing:=Nothing, rankOrder:=True, theme)

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
            Dim pathways As String() = getPathways(-1, 30).Values.IteratesALL.Distinct.ToArray
            Dim css As CSSEnvirnment = g.LoadEnvironment
            Dim pathwayNameFont As Font = css.GetFont(CSSFont.TryParse(theme.axisTickCSS))
            Dim pad = g.MeasureString("A", pathwayNameFont)
            Dim labelSize As SizeF
            Dim pvalues As DoubleRange = multiples _
                .Select(Function(v) v.Value.Values) _
                .IteratesALL _
                .IteratesALL _
                .Select(Function(b) If(b.PValue.IsNaNImaginary, 1, b.PValue)) _
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
            Dim canvasRect = canvas.PlotRegion(css)
            Dim region As New Rectangle With {
                .X = canvasRect.Left + maxTag.Width * 1.125,
                .Y = canvasRect.Top,
                .Width = canvasRect.Width - maxTag.Width * 1.125,
                .Height = canvasRect.Height
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
                .Width = std.Min(dx, dy),
                .Height = std.Min(dx, dy)
            }
            Dim x, y As Double
            Dim gridStroke As Pen = css.GetPen(Stroke.TryParse(theme.gridStrokeX))

            x = region.Left + dx / 2
            y = region.Top + dy / 2
            g.DrawRectangle(css.GetPen(Stroke.TryParse(theme.axisStroke)), region)

            For Each sample In multiples
                Call g.DrawLine(gridStroke, New PointF(x, region.Top), New PointF(x, region.Bottom))
                x += dx
            Next

            For Each pid As String In pathways
                x = region.Left + dx / 2

                Call g.DrawLine(gridStroke, New PointF(region.Left, y), New PointF(region.Right, y))
                Call g.DrawLine(css.GetPen(Stroke.TryParse(theme.axisStroke)), New PointF(region.Left, y), New PointF(region.Left - pad.Width / 2, y))

                For Each sample In matrix
                    Dim bubble As BubbleTerm = sample.Value.TryGetValue(pid)

                    If Not bubble Is Nothing AndAlso Not bubble.PValue.IsNaNImaginary Then
                        Dim color As SolidBrush = paints(CInt(pvalues.ScaleMapping(bubble.PValue, colorIndex)))
                        Dim radius As Single = impacts.ScaleMapping(bubble.data, cellRange)

                        Call g.DrawCircle(New PointF(x, y), radius, color)
                    End If

                    x += dx
                Next

                labelSize = g.MeasureString(pid, pathwayNameFont)
                x = region.Left - labelSize.Width / 1.125 - dx / 2
                g.DrawString(pid, pathwayNameFont, Brushes.Black, New PointF(x, y - dy / 2 + (dy - pad.Height) / 2))

                y += dy
            Next

            ' draw sample labels
            x = region.Left + dx
            y += dy

            For Each sample In multiples
                g.DrawString(sample.Name, pathwayNameFont, Brushes.Black, x, y, angle:=45)
                x += dx
            Next

            Call DrawMainTitle(g, region)
            Call MultipleBubble.drawRadiusLegend(g, impacts, cellRange, canvas, theme, title:="Enrichment Hits", tickFormat:="F0")
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
