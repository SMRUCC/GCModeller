#Region "Microsoft.VisualBasic::297fe11797362ed81ea1456a04672557, visualize\DataVisualizationExtensions\CatalogProfiling\CatalogBubblePlot.vb"

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

'     Class BubbleTerm
' 
'         Properties: data, Factor, PValue, termId
' 
'     Class CatalogBubblePlot
' 
'         Constructor: (+1 Overloads) Sub New
' 
'         Function: GetCatalogSerialData, GetColorIndex, unenrichSerial
' 
'         Sub: DrawBubbleLegends, PlotInternal
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports stdNum = System.Math

Namespace CatalogProfiling

    Public Class CatalogBubblePlot : Inherits Plot

        ReadOnly showBubbleBorder As Boolean
        ReadOnly data As Dictionary(Of String, BubbleTerm())
        ReadOnly enrichColors As Dictionary(Of String, Color)
        ReadOnly displays As LabelDisplayStrategy = LabelDisplayStrategy.Default
        ''' <summary>
        ''' the pvalue cutoff between the enriched terms 
        ''' and the un-enriched terms. default value of 
        ''' this cutoff is -log10(0.05)
        ''' </summary>
        ReadOnly pvalue As Double = -stdNum.Log10(0.05)
        ReadOnly unenrich As Color
        ReadOnly bubbleResize As DoubleRange

        Public Property radiusTitle As String = "Enrichment Hits"
        Public Property radiusFormat As String = "F0"

        Public Sub New(data As Dictionary(Of String, BubbleTerm()),
                       enrichColors As Dictionary(Of String, Color),
                       showBubbleBorder As Boolean,
                       displays As LabelDisplayStrategy,
                       pvalue As Double,
                       unenrich As Color,
                       bubbleSize As DoubleRange,
                       theme As Theme)

            MyBase.New(theme)

            Me.data = data
            Me.showBubbleBorder = showBubbleBorder
            Me.enrichColors = enrichColors
            Me.unenrich = unenrich
            Me.displays = displays
            Me.pvalue = pvalue
            Me.bubbleResize = bubbleSize

            xlabel = "richFactor=(n/background)"
            ylabel = "-log10(p.value)"
        End Sub

        Private Function GetColorIndex(ByRef catalog As List(Of BubbleTerm), colors As Color()) As Integer()
            Dim pv As Vector = catalog.Select(Function(gene) gene.PValue).AsVector
            Dim enrichResults = catalog(which.IsTrue(pv > pvalue))
            Dim colorIndex%()
            Dim dataRange As DoubleRange = enrichResults _
                .Select(Function(gene) gene.PValue) _
                .Range
            Dim indexRange As DoubleRange = {0, colors.Length - 1}

            If dataRange.Length = 0 Then
                colorIndex = enrichResults _
                    .Select(Function(any) colors.Length - 1) _
                    .ToArray
            Else
                colorIndex = enrichResults _
                    .Select(Function(t) dataRange.ScaleMapping(t.PValue, indexRange)) _
                    .Select(Function(i)
                                Return CInt(i)
                            End Function) _
                    .ToArray
            End If

            catalog = enrichResults

            Return colorIndex
        End Function

        Private Iterator Function GetCatalogSerialData(allValues As DoubleRange) As IEnumerable(Of SerialData)
            For Each category As String In data.Keys
                ' 这些都是经过筛选的，pvalue阈值符合条件的，
                ' 剩下的pvalue阈值不符合条件的都被当作为同一个serials
                Dim color As Color = enrichColors(category).Alpha(250)
                Dim terms = data(category).AsList
                Dim pt As PointData = Nothing
                Dim serial As New SerialData With {
                    .color = color,
                    .title = category,
                    .pts = terms _
                        .SeqIterator _
                        .Select(Function(obj)
                                    Dim gene As BubbleTerm = obj
                                    Dim c As Color = color

                                    Return New PointData With {
                                        .value = allValues.ScaleMapping(gene.data, bubbleResize),
                                        .pt = New PointF(x:=gene.Factor, y:=gene.PValue),
                                        .tag = gene.termId,
                                        .color = c.ARGBExpression
                                    }
                                End Function) _
                        .Where(Function(t) t.pt.Y >= pvalue) _
                        .OrderByDescending(Function(bubble)
                                               ' 按照y也就是pvalue倒序排序
                                               Return bubble.pt.Y
                                           End Function) _
                        .ToArray
                }

                If serial.pts.Length > 0 Then
                    Yield serial
                End If
            Next

            Yield unenrichSerial(catalog:=data.Values.IteratesALL, allValues)
        End Function

        Friend Const unenrichTerm As String = "Unenrich terms"

        ''' <summary>
        ''' the title that created via this function is <see cref="unenrichTerm"/>
        ''' </summary>
        ''' <param name="catalog"></param>
        ''' <param name="allValues"></param>
        ''' <returns></returns>
        Private Function unenrichSerial(catalog As IEnumerable(Of BubbleTerm), allValues As DoubleRange) As SerialData
            Dim unenrichs As BubbleTerm() = catalog _
                .Where(Function(term) term.PValue <= pvalue) _
                .ToArray
            Dim points As PointData() = unenrichs _
                .Select(Function(gene)
                            Return New PointData With {
                                .value = allValues.ScaleMapping(gene.data, bubbleResize),
                                .pt = New PointF(x:=gene.Factor, y:=gene.PValue)
                            }
                        End Function) _
                .ToArray

            Return New SerialData With {
                .color = unenrich,
                .title = unenrichTerm,
                .pts = points
            }
        End Function

        Protected Overrides Sub PlotInternal(ByRef g As IGraphics, region As GraphicsRegion)
            Dim allValues As DoubleRange = data.Values _
                .IteratesALL _
                .Select(Function(gene) gene.data) _
                .Range
            Dim serials As SerialData() = GetCatalogSerialData(allValues).ToArray
            Dim bubbleBorder As Stroke = Nothing
            Dim radiusData As DoubleRange = data.Values _
                .IteratesALL _
                .Select(Function(gene) gene.data) _
                .Range

            If Not displays Is Nothing Then
                serials = displays.filterLabelDisplays(serials)
            End If
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
                xlabel:=xlabel,
                ylabel:=ylabel,
                bubbleBorder:=bubbleBorder,
                strokeColorAsMainColor:=True,
                axisLabelFontCSS:=CSSFont.Win10NormalLarge,
                positiveRangeY:=True
            )

            Call g.DrawImageUnscaled(plot, New Point)
            Call DrawBubbleLegends(g, serials, region)

            Dim titleFont As Font = CSSFont.TryParse(theme.mainCSS).GDIObject(g.Dpi)
            Dim fsize As SizeF = g.MeasureString(main, titleFont)
            Dim tloc As New PointF With {
                .X = (region.Size.Width - fsize.Width) / 2,
                .Y = (region.Padding.Top - fsize.Height) / 2
            }

            Call g.DrawString(main, titleFont, Brushes.Black, tloc)
            Call MultipleBubble.drawRadiusLegend(
                g:=g,
                impacts:=radiusData,
                radius:=bubbleResize,
                canvas:=region,
                theme:=theme,
                title:=radiusTitle,
                tickFormat:=radiusFormat
            )
        End Sub

        ''' <summary>
        ''' show category legend
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="serials"></param>
        ''' <param name="region"></param>
        Private Sub DrawBubbleLegends(g As IGraphics, serials As SerialData(), region As GraphicsRegion)
            Dim legendFontStyle As String = theme.legendLabelCSS
            Dim plot As Rectangle = region.PlotRegion
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
            Dim legendFont As Font = CSSFont.TryParse(legendFontStyle).GDIObject(g.Dpi)
            Dim cSize As SizeF = g.MeasureString("0", legendFont)
            Dim legendSize As New SizeF(stdNum.Max(cSize.Width, cSize.Height), stdNum.Max(cSize.Width, cSize.Height))
            Dim maxWidth As Single = legends _
                .Select(Function(l)
                            Return g.MeasureString(l.title, legendFont).Width
                        End Function) _
                .Max
            Dim ltopLeft As New Point With {
                .X = plot.Right + legendSize.Width * 1.25,
                .Y = region.PlotRegion.Top + (region.PlotRegion.Height - (cSize.Height + 10) * 3) / 2
            }

            Call g.DrawLegends(
                ltopLeft,
                legends,
                gSize:=$"{legendSize.Width},{legendSize.Height}",
                regionBorder:=New Stroke With {
                    .fill = "Black",
                    .dash = DashStyle.Solid,
                    .width = 2
                }
            )
        End Sub
    End Class
End Namespace
