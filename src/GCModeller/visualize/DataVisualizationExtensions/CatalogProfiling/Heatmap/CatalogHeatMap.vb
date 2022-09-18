#Region "Microsoft.VisualBasic::eefc5012bb4cfca62ba3e91acfed013b, GCModeller\visualize\DataVisualizationExtensions\CatalogProfiling\Heatmap\CatalogHeatMap.vb"

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

    '   Total Lines: 159
    '    Code Lines: 123
    ' Comment Lines: 16
    '   Blank Lines: 20
    '     File Size: 7.10 KB


    '     Class CatalogHeatMap
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Sub: PlotInternal
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Text
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Html.CSS

Namespace CatalogProfiling

    ''' <summary>
    ''' heatmap data of the KEGG enrichment between 
    ''' multiple groups data:
    ''' 
    ''' 1. x axis is the sample id
    ''' 2. y axis is the pathway name and category data
    ''' 3. cell size is the impact value or enrich factor
    ''' 4. cell color is scaled via -log10(pvalue)
    ''' </summary>
    Public Class CatalogHeatMap : Inherits MultipleCatalogHeatmap

        Public Sub New(profile As IEnumerable(Of NamedValue(Of Dictionary(Of String, BubbleTerm()))), mapLevels As Integer, theme As Theme)
            Call MyBase.New(profile, mapLevels, "black", theme)
        End Sub

        ''' <summary>
        ''' heatmap是按照代谢途径分块绘制的
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="canvas"></param>
        Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
            Dim pathways As Dictionary(Of String, String()) = getPathways()
            Dim pathwayNameFont As Font = CSSFont.TryParse(theme.axisTickCSS).GDIObject(g.Dpi)
            Dim categoryFont As Font = CSSFont.TryParse(theme.axisLabelCSS).GDIObject(g.Dpi)
            Dim labelHeight As Double = g.MeasureString("A", categoryFont).Height
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
            Dim maxTag As SizeF = pathways.Values _
                .IteratesALL _
                .Select(Function(str)
                            Return viz.MeasureString(str, pathwayNameFont)
                        End Function) _
                .JoinIterates(pathways.Keys.Select(Function(str) viz.MeasureString(str, categoryFont))) _
                .OrderByDescending(Function(sz) sz.Width) _
                .First
            Dim region As New Rectangle With {
                .X = canvas.PlotRegion.Left,
                .Y = canvas.PlotRegion.Top,
                .Width = canvas.PlotRegion.Width - maxTag.Width,
                .Height = canvas.PlotRegion.Height
            }
            Dim gap As Double = labelHeight * 1.5
            Dim dh As Double = (region.Height - gap * (pathways.Count - 1)) / (pathways.Values.IteratesALL.Count)
            Dim dw As Double = region.Width / multiples.Count
            Dim sizeRange As DoubleRange = New Double() {0, dw}
            Dim colors As SolidBrush() = Designer _
                .GetColors(theme.colorSet, mapLevels) _
                .Select(Function(c) New SolidBrush(c)) _
                .ToArray
            Dim indexRange As DoubleRange = New Double() {0, mapLevels - 1}
            Dim y As Double = region.Top
            Dim x As Double
            Dim categoryColors As LoopArray(Of SolidBrush) = Designer _
                .GetColors("Set1:c8") _
                .Select(Function(c) New SolidBrush(c)) _
                .ToArray

            For Each catName As String In pathways.Keys
                Dim pathIds As String() = pathways(catName)
                Dim foreColor = ++categoryColors
                Dim samples = multiples _
                    .Select(Function(v)
                                Dim list = v.Value.TryGetValue(catName)
                                Dim maps As Dictionary(Of String, BubbleTerm) = Nothing

                                If Not list Is Nothing Then
                                    maps = list.ToDictionary(Function(p) p.termId)
                                End If

                                Return New NamedValue(Of Dictionary(Of String, BubbleTerm)) With {
                                    .Name = v.Name,
                                    .Value = maps
                                }
                            End Function) _
                    .ToArray

                Call g.DrawString(catName, categoryFont, Brushes.Black, New PointF(region.Right, y - labelHeight))

                Dim deltaY As Double = dh * pathIds.Length
                Dim block As New Rectangle(region.Left, y, region.Width, deltaY)
                ' Dim v As New List(Of Double)

                Call g.FillRectangle(colorMissing.GetBrush, block)
                Call g.DrawRectangle(Stroke.TryParse(theme.axisStroke).GDIObject, block)

                For Each id As String In pathIds
                    x = region.Left

                    For Each sample In samples
                        If (Not sample.Value.IsNullOrEmpty) AndAlso sample.Value.ContainsKey(id) Then
                            Dim bubble As BubbleTerm = sample.Value(id)
                            Dim index As Integer = pvalues.ScaleMapping(bubble.PValue, indexRange)
                            Dim paint As Brush = colors(index)
                            Dim size As Double = impacts.ScaleMapping(bubble.data, sizeRange)
                            Dim cell As New RectangleF With {
                                .X = x,
                                .Y = y,
                                .Width = dw,
                                .Height = dh
                            }

                            Call g.FillRectangle(paint, cell)
                        End If

                        x += dw
                    Next

                    g.DrawString(id, pathwayNameFont, foreColor, New PointF(x, y))
                    y += dh
                Next

                y += gap
            Next

            ' draw sample labels
            x = region.Left + dw / 2
            y -= gap

            Dim text As New GraphicsText(DirectCast(g, Graphics2D).Graphics)

            For Each sample In multiples
                text.DrawString(sample.Name, pathwayNameFont, Brushes.Black, New PointF(x, y), angle:=45)
                x += dw
            Next

            Call drawColorLegends(
                pvalues:=pvalues,
                right:=region.Right + maxTag.Width * 0.975,
                g:=g,
                canvas:=canvas
            )
        End Sub
    End Class
End Namespace
