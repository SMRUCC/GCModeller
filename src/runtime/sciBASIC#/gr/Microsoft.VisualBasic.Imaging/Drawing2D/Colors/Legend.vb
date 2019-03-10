﻿#Region "Microsoft.VisualBasic::e7765c4c890b4db07f11093ed1fd113b, gr\Microsoft.VisualBasic.Imaging\Drawing2D\Colors\Legend.vb"

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

    '     Module Legends
    ' 
    '         Function: ColorLegendHorizontal, (+2 Overloads) ColorMapLegend
    ' 
    '         Sub: ColorLegendHorizontal, ColorMapLegend
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS

Namespace Drawing2D.Colors

    Public Module Legends

        ''' <summary>
        ''' Draw color legend for the color sequnece.
        ''' (通过这个函数只是生成了legend的图片，还需要自己将图片放置到图表上的合适的位置)
        ''' </summary>
        ''' <param name="designer"></param>
        ''' <param name="title$">The legend title</param>
        ''' <param name="min$"></param>
        ''' <param name="max$"></param>
        ''' <param name="bg$"></param>
        ''' <param name="haveUnmapped"></param>
        ''' <param name="lsize"></param>
        ''' <returns></returns>
        <Extension>
        Public Function ColorMapLegend(designer As Color(),
                                       title$,
                                       min$, max$,
                                       Optional bg$ = "transparent",
                                       Optional haveUnmapped As Boolean = True,
                                       Optional lsize As Size = Nothing,
                                       Optional padding$ = DefaultPadding,
                                       Optional titleFont As Font = Nothing,
                                       Optional labelFont As Font = Nothing,
                                       Optional legendWidth! = -1) As GraphicsData
            Dim br As SolidBrush() =
                designer.Select(Function(c) New SolidBrush(c)).ToArray
            Return br.ColorMapLegend(
                title,
                min, max,
                bg,
                haveUnmapped,
                lsize, padding,
                titleFont, labelFont,
                legendWidth)
        End Function

        Public Const DefaultPadding$ = "padding:50px 50px 100px 50px;"

        ReadOnly defaultLegendSize As DefaultValue(Of Size) = New Size(800, 1024)

        ''' <summary>
        ''' 竖直的颜色图例，输出的图例的大小默认为：``{800, 1024}``
        ''' </summary>
        ''' <param name="designer"></param>
        ''' <param name="title$"></param>
        ''' <param name="min$"></param>
        ''' <param name="max$"></param>
        ''' <param name="bg$"></param>
        ''' <param name="haveUnmapped"></param>
        ''' <param name="lsize"></param>
        ''' <param name="titleFont"></param>
        ''' <returns></returns>
        <Extension>
        Public Function ColorMapLegend(designer As SolidBrush(),
                                       title$,
                                       min$, max$,
                                       Optional bg$ = "transparent",
                                       Optional haveUnmapped As Boolean = True,
                                       Optional lsize As Size = Nothing,
                                       Optional padding$ = DefaultPadding,
                                       Optional titleFont As Font = Nothing,
                                       Optional labelFont As Font = Nothing,
                                       Optional legendWidth! = -1) As GraphicsData

            Dim margin As Padding = padding

            If titleFont Is Nothing Then
                titleFont = New Font(FontFace.MicrosoftYaHei, 36)
            End If
            If labelFont Is Nothing Then
                labelFont = New Font(FontFace.BookmanOldStyle, 24)
            End If

            Dim plotInternal =
                Sub(ByRef g As IGraphics, region As GraphicsRegion)
                    Dim layout As New Rectangle With {
                        .X = 0,
                        .Y = 0,
                        .Width = region.Width,
                        .Height = region.Height - titleFont.Height - 5
                    }

                    Call g.ColorMapLegend(
                        layout:=layout, designer:=designer,
                        ticks:={Val(min), Val(max)},
                        titleFont:=titleFont,
                        title:=title,
                        tickFont:=labelFont,
                        tickAxisStroke:=Pens.Black
                    )
                End Sub

            Return GraphicsPlots(lsize Or defaultLegendSize, margin, bg, plotInternal)
        End Function

        ''' <summary>
        ''' 垂直的颜色谱的绘制：左边为颜色谱，右边为标尺，左边的颜色谱的上方为标题
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="layout">legend的大小和位置</param>
        ''' 
        <Extension>
        Public Sub ColorMapLegend(ByRef g As IGraphics, layout As Rectangle,
                                  designer As SolidBrush(),
                                  ticks#(),
                                  titleFont As Font, title$,
                                  tickFont As Font,
                                  tickAxisStroke As Pen,
                                  Optional unmapColor$ = Nothing,
                                  Optional ruleOffset! = 10,
                                  Optional roundDigit% = 2)

            Dim titleSize As SizeF = g.MeasureString(title, titleFont)
            Dim legendOffsetLeft!, legendOffsetTop!
            Dim legendWidth! = layout.Width / 3 ' 颜色谱的宽度为layout的 1/3
            Dim legendHeight!
            Dim d!

            ' 首先计算出layout
            legendOffsetTop = titleSize.Height * 2 + 5

            ' 下面的三个元素在宽度上面各自占1/3
            ' 空白 | legend | 标尺
            legendOffsetLeft = legendWidth

            If unmapColor.StringEmpty Then
                ' 没有unmap的颜色，则颜色谱的高度占据剩下的所有高度
                legendHeight = layout.Height - legendOffsetTop
                d = legendHeight / designer.Length
            Else
                legendHeight = layout.Height - legendOffsetTop
                d = legendHeight / (designer.Length + 2)
                legendHeight -= 2 * d
            End If

            Dim point As PointF
            Dim x!, y!
            Dim rect As RectangleF

            ' 绘制标题
            x = layout.Left + legendOffsetLeft + (legendWidth - titleSize.Width) / 2
            y = layout.Top
            point = New PointF(x, y)

            Call g.DrawString(title, titleFont, Brushes.Black, point)

            ' 绘制出颜色谱
            y = legendOffsetTop + layout.Top
            legendOffsetLeft += layout.Left

            For i As Integer = designer.Length - 1 To 0 Step -1
                rect = New RectangleF With {
                    .Location = New PointF(legendOffsetLeft, y),
                    .Size = New SizeF(legendWidth, d)
                }
                g.FillRectangle(brush:=designer(i), rect:=rect)
                y += d
            Next

            y += d

            If Not unmapColor.StringEmpty Then
                Dim color As Brush = unmapColor.GetBrush

                rect = New RectangleF With {
                    .Location = New PointF(legendOffsetLeft, y),
                    .Size = New SizeF(legendWidth, d)
                }
                point = New PointF With {
                    .X = legendOffsetLeft + legendWidth + 5,
                    .Y = y + (d - tickFont.Height) / 2
                }
                g.FillRectangle(color, rect:=rect)
                g.DrawString("Unknown", tickFont, Brushes.Black, point)
            End If

            ' 绘制出标尺
            x = legendOffsetLeft + legendWidth + ruleOffset
            y = layout.Top + legendOffsetTop
            g.DrawLine(tickAxisStroke, x, y, x, y + legendHeight)

            ' 绘制最大值和最小值
            g.DrawLine(Pens.Black, x, y, x + ruleOffset, y)
            g.DrawLine(Pens.Black, x, y + legendHeight, x + ruleOffset, y + legendHeight)

            x += ruleOffset + 5
            point = New PointF(x, y - tickFont.Height / 2)
            g.DrawString(ticks.Max.ToString("F" & roundDigit), tickFont, Brushes.Black, point)

            point = New PointF(x, y + legendHeight - tickFont.Height / 2)
            g.DrawString(ticks.Min.ToString("F" & roundDigit), tickFont, Brushes.Black, point)

            ticks = ticks _
                .Skip(1) _
                .Take(ticks.Length - 2) _
                .OrderByDescending(Function(n) n) _
                .ToArray

            Dim delta = legendHeight / (ticks.Length + 1)

            y += delta
            x -= ruleOffset
            tickFont = New Font(tickFont.FontFamily, tickFont.Size * 2.5 / 3)

            ' 画出剩余的小标尺
            For Each tick In ticks

                point = New PointF With {
                    .X = x + 2,
                    .Y = y - tickFont.Height / 2
                }
                g.DrawLine(Pens.Black, x, y, x - 5, y)
                g.DrawString(tick.ToString($"F{roundDigit}"), tickFont, Brushes.Gray, point)

                y += delta
            Next
        End Sub

        ''' <summary>
        ''' 横向的颜色legend
        ''' </summary>
        ''' <param name="designer"></param>
        ''' <param name="size"></param>
        ''' <param name="padding$"></param>
        ''' <param name="labelFontCSS$"></param>
        ''' <returns></returns>
        <Extension>
        Public Function ColorLegendHorizontal(designer As SolidBrush(),
                                              ticks#(),
                                              size As Size,
                                              Optional padding$ = g.ZeroPadding,
                                              Optional labelFontCSS$ = CSSFont.Win7Normal) As GraphicsData
            Dim plotInternal =
                Sub(ByRef g As IGraphics, region As GraphicsRegion)
                    Call designer.ColorLegendHorizontal(
                        ticks,
                        g, region.PlotRegion,
                        padding,
                        labelFontCSS)
                End Sub

            Return g.GraphicsPlots(
                size, padding,
                "transparent",
                plotInternal)
        End Function

        <Extension>
        Public Sub ColorLegendHorizontal(designer As SolidBrush(),
                                         ticks#(),
                                         ByRef g As IGraphics,
                                         region As Rectangle,
                                         Optional padding$ = g.ZeroPadding,
                                         Optional labelFontCSS$ = CSSFont.Win7Normal,
                                         Optional AxisStroke$ = Stroke.AxisStroke,
                                         Optional scientificNotation As Boolean = False)

            Dim font As Font = CSSFont.TryParse(labelFontCSS)
            Dim l = designer.Length
            Dim dx = region.Width / l
            Dim h = region.Height * (2 / 3)
            Dim x = region.Left, y = region.Top

            ' 绘制出水平的颜色渐变条
            For i As Integer = 0 To l - 1
                Dim b = designer(i)
                Dim rect As New Rectangle(x, y, dx, h)

                g.FillRectangle(b, rect)
                x += dx
            Next

            ' 绘制出水平标尺刻度
            y = y + h + 10

            With region

                g.DrawLine(Stroke.TryParse(AxisStroke).GDIObject, New Point(.Left, y), New Point(x, y))
                y += 5

                For Each i As SeqValue(Of Double) In ticks _
                    .RangeTransform({ .Left, x}) _
                    .SeqIterator

                    Dim tick$ = If(scientificNotation, ticks(i).ToString("G2"), ticks(i))
                    Dim fsize = g.MeasureString(tick, font)

                    x = i.value
                    g.DrawLine(Pens.Black, New PointF(x, y), New Point(x, y - 5))
                    g.DrawString(tick, font, Brushes.Black, New PointF(x - fsize.Width / 2, y))
                Next
            End With

        End Sub
    End Module
End Namespace
