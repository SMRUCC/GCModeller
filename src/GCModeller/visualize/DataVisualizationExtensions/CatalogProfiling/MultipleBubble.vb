﻿#Region "Microsoft.VisualBasic::63a821c20e23194ded0d9b8fae824996, visualize\DataVisualizationExtensions\CatalogProfiling\MultipleBubble.vb"

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

    '   Total Lines: 297
    '    Code Lines: 234 (78.79%)
    ' Comment Lines: 24 (8.08%)
    '    - Xml Docs: 79.17%
    ' 
    '   Blank Lines: 39 (13.13%)
    '     File Size: 13.46 KB


    '     Class MultipleBubble
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: getSampleColors
    ' 
    '         Sub: (+2 Overloads) drawRadiusLegend, drawSampleLegends, PlotInternal
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.MIME.Html.Render

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

    ''' <summary>
    ''' kegg enrichment bubble in multiple groups
    ''' 
    ''' 1. x axis is related to the -log10(pvalue)
    ''' 2. y axis is the category of the kegg pathway maps
    ''' 3. bubble size is the impact factor or pathway hit score
    ''' 4. color of the bubble can be related to the another score
    ''' </summary>
    Public Class MultipleBubble : Inherits MultipleCategoryProfiles

        ReadOnly radius As DoubleRange
        ReadOnly alpha As Double = 1

        Sub New(multiples As IEnumerable(Of NamedValue(Of Dictionary(Of String, BubbleTerm()))),
                radius As DoubleRange,
                alpha As Double,
                theme As Theme)

            Call MyBase.New(multiples, theme)

            Me.radius = radius
            Me.alpha = alpha
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Sub drawRadiusLegend(ByRef g As IGraphics, impacts As DoubleRange, canvas As GraphicsRegion)
            Call drawRadiusLegend(g, impacts, radius, canvas, theme)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="impacts">
        ''' the data value range
        ''' </param>
        ''' <param name="radius">
        ''' the bubble radius pixel range
        ''' </param>
        ''' <param name="canvas"></param>
        ''' <param name="theme"></param>
        ''' <param name="title"></param>
        Friend Shared Sub drawRadiusLegend(ByRef g As IGraphics,
                                           impacts As DoubleRange,
                                           radius As DoubleRange,
                                           canvas As GraphicsRegion,
                                           theme As Theme,
                                           Optional title As String = "Enrichment Factor",
                                           Optional tickFormat As String = "F3")

            Dim values As Double() = impacts.Enumerate(4)

            ' no data?
            If values.IsNullOrEmpty Then
                Call "no impacts data???".Warning
                Return
            End If

            Dim css As CSSEnvirnment = g.LoadEnvironment
            Dim x As Double = canvas.PlotRegion(css).Right + css.GetWidth(canvas.Padding.Right) / 5
            Dim y As Double = css.GetHeight(canvas.Padding.Top) * 1.125
            Dim r As Single
            Dim paint As SolidBrush = Brushes.Black
            Dim pos As PointF
            Dim tickFont As Font = css.GetFont(CSSFont.TryParse(theme.axisTickCSS))
            Dim labelFont As Font = css.GetFont(CSSFont.TryParse(theme.legendTitleCSS))

            g.DrawString(title, labelFont, Brushes.Black, New PointF(x - impacts.ScaleMapping(values.Max, radius) * 2, y))
            y += g.MeasureString("A", labelFont).Height * 1.5

            Dim ymin As Double = y
            Dim ymax As Double
            Dim nsize As SizeF = g.MeasureString("0", tickFont)

            For Each ip As Double In values
                r = impacts.ScaleMapping(ip, radius)
                pos = New PointF(x, y)
                ymax = y
                y = y + r * 2.5 + 30

                Call g.DrawCircle(pos, r, paint)
            Next

            x = x + r * 1.5

            Call g.DrawString(values.Min.ToString(tickFormat), tickFont, Brushes.Black, New PointF(x + 5, ymin - nsize.Height / 2))
            Call g.DrawLine(New Pen(Color.Black, 2), New PointF(x, ymin), New PointF(x, ymax))
            Call g.DrawString(values.Max.ToString(tickFormat), tickFont, Brushes.Black, New PointF(x + 5, ymax - nsize.Height / 2))
        End Sub

        Private Function getSampleColors() As Dictionary(Of String, SolidBrush)
            Dim colors As Color() = Designer.GetColors("paper", n:=multiples.Count)
            Dim list As New Dictionary(Of String, SolidBrush)

            For Each sample In multiples.SeqIterator
                Call list.Add(sample.value.Name, New SolidBrush(colors(sample).Alpha(alpha * 255)))
            Next

            Return list
        End Function

        Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
            Dim allScores As Double() = multiples _
                .Select(Function(t) t.Value.Values) _
                .IteratesALL _
                .IteratesALL _
                .Select(Function(b) b.PValue * b.data) _
                .OrderBy(Function(xi) xi) _
                .ToArray
            Dim pvalueTicks As Double() = allScores.CreateAxisTicks
            Dim categories As String() = getCategories()
            Dim pathways As String() = getPathways().Values.IteratesALL.ToArray

            If pathways.IsNullOrEmpty Then
                Return
            End If

            Dim css As CSSEnvirnment = g.LoadEnvironment
            Dim fontsize As SizeF
            Dim pathwayLabelFont As Font = css.GetFont(CSSFont.TryParse(theme.axisLabelCSS))
            Dim categoryFont As New Font(pathwayLabelFont.Name, CSng(pathwayLabelFont.Size * 1.25), FontStyle.Bold)
            Dim viz As IGraphics = g
            Dim maxLabel As SizeF = categories _
                .Select(Function(str) viz.MeasureString(str, categoryFont)) _
                .JoinIterates(pathways.Select(Function(str)
                                                  Return viz.MeasureString(str, pathwayLabelFont)
                                              End Function)) _
                .OrderByDescending(Function(t) t.Width) _
                .First
            Dim plotRect = canvas.PlotRegion(css)
            Dim region As New Rectangle With {
                .X = css.GetWidth(canvas.Padding.Left) + maxLabel.Width,
                .Y = css.GetHeight(canvas.Padding.Top),
                .Width = plotRect.Width - maxLabel.Width,
                .Height = plotRect.Height
            }
            Dim xscale = d3js.scale _
                .linear() _
                .domain(values:=pvalueTicks) _
                .range(values:={region.Left, region.Right})
            Dim dh As Double = region.Height / (pathways.Length + categories.Length + 1)
            Dim y As Double = region.Top - g.MeasureString("A", categoryFont).Height / 2
            Dim x As Double
            Dim impacts As New DoubleRange(multiples _
                .Select(Function(t) t.Value.Values) _
                .IteratesALL _
                .IteratesALL _
                .Select(Function(b) b.Factor))
            Dim r As Single
            Dim colorSet As LoopArray(Of Color) = Designer.GetColors(theme.colorSet)
            Dim paint As SolidBrush
            Dim sampleColors As Dictionary(Of String, SolidBrush) = getSampleColors()

            Call g.DrawRectangle(css.GetPen(Stroke.TryParse(theme.axisStroke)), region)

            ' draw axis
            Call Axis.DrawX(
                g:=g,
                pen:=css.GetPen(Stroke.TryParse(theme.axisStroke)),
                label:=xlabel,
                scaler:=New DataScaler With {.AxisTicks = (pvalueTicks.AsVector, Nothing), .region = region, .X = xscale, .Y = Nothing},
                layout:=XAxisLayoutStyles.Bottom,
                Y0:=0,
                offset:=Nothing,
                labelFont:=theme.axisLabelCSS,
                labelColor:=Brushes.Black,
                tickFont:=css.GetFont(theme.axisTickCSS),
                tickColor:=Brushes.Black,
                htmlLabel:=False
            )

            For Each catName As String In categories
                fontsize = g.MeasureString(catName, categoryFont)
                x = css.GetWidth(canvas.Padding.Left)
                paint = New SolidBrush(++colorSet)

                Call Console.WriteLine(catName)
                Call g.DrawString(catName, categoryFont, Brushes.Black, New PointF(x, y))

                Dim categoryData = multiples _
                    .Where(Function(group) group.Value.ContainsKey(catName)) _
                    .Select(Function(group)
                                Return New NamedValue(Of Dictionary(Of String, BubbleTerm)) With {
                                    .Name = group.Name,
                                    .Value = group.Value(catName) _
                                        .ToDictionary(Function(t)
                                                          Return t.termId
                                                      End Function)
                                }
                            End Function) _
                    .ToArray
                Dim pathwayNames As String() = categoryData _
                    .Select(Function(t) t.Value.Keys) _
                    .IteratesALL _
                    .Distinct _
                    .ToArray

                y = y + fontsize.Height
                fontsize = g.MeasureString("A", pathwayLabelFont)
                y = y + (dh - fontsize.Height) / 2

                ' draw bubbles in multiple groups
                For Each name As String In pathwayNames
                    Call g.DrawString(name, pathwayLabelFont, paint, New PointF(x + fontsize.Width / 2, y))
                    Call g.DrawLine(New Pen(paint, 3) With {.DashStyle = DashStyle.Dash}, New PointF(region.Left, y), New PointF(region.Right, y))

                    For Each group As NamedValue(Of Dictionary(Of String, BubbleTerm)) In categoryData
                        Dim bubble As BubbleTerm = group.Value.TryGetValue(name)
                        Dim fill As SolidBrush = sampleColors(group.Name)

                        If Not bubble Is Nothing Then
                            x = xscale(bubble.PValue * bubble.data)
                            r = impacts.ScaleMapping(bubble.Factor, Me.radius)

                            Call g.DrawCircle(New PointF(x, y), r, color:=fill)
                        End If
                    Next

                    y += dh
                    x = css.GetWidth(canvas.Padding.Left)
                Next
            Next

            Call drawRadiusLegend(g, impacts, canvas)
            Call drawSampleLegends(sampleColors, g, canvas)
            Call DrawMainTitle(g, region)
        End Sub

        Private Sub drawSampleLegends(sampleColors As Dictionary(Of String, SolidBrush), g As IGraphics, canvas As GraphicsRegion)
            Dim legends As LegendObject() = sampleColors _
                .Select(Function(sample)
                            Return New LegendObject With {
                                .color = sample.Value.Color.ToHtmlColor,
                                .fontstyle = theme.legendLabelCSS,
                                .style = LegendStyles.Circle,
                                .title = sample.Key
                            }
                        End Function) _
                .ToArray
            Dim css As CSSEnvirnment = g.LoadEnvironment
            Dim plotRect = canvas.PlotRegion(css)

            theme.legendLayout = New Absolute() With {
                .x = plotRect.Right + 100,
                .y = plotRect.Top + plotRect.Height / 3
            }

            Call DrawLegends(g, legends, showBorder:=False, canvas)
        End Sub
    End Class
End Namespace
