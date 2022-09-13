#Region "Microsoft.VisualBasic::c4d6d54f0974dfbe7463cc29a566d6b8, GCModeller\visualize\DataVisualizationExtensions\CatalogProfiling\CatalogProfiling.vb"

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

    '   Total Lines: 423
    '    Code Lines: 297
    ' Comment Lines: 76
    '   Blank Lines: 50
    '     File Size: 20.02 KB


    '     Module CatalogProfilingPlot
    ' 
    '         Function: AsDouble, GetTicks, ProfilesPlot, removesNotAssign
    ' 
    '         Sub: internalPlotImpl
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Imaging.SVG
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.ComponentModel.Annotation

Namespace CatalogProfiling

    ''' <summary>
    ''' COG, GO, KEGG
    ''' </summary>
    Public Module CatalogProfilingPlot

        <Extension>
        Public Function AsDouble(data As Dictionary(Of String, NamedValue(Of Integer)())) As Dictionary(Of String, NamedValue(Of Double)())
            Dim out As New Dictionary(Of String, NamedValue(Of Double)())

            For Each [class] In data
                out([class].Key) = [class].Value _
                    .Select(Function(c)
                                Return New NamedValue(Of Double) With {
                                    .Name = c.Name,
                                    .Value = c.Value
                                }
                            End Function) _
                    .ToArray
            Next

            Return out
        End Function

        ''' <summary>
        ''' No classification
        ''' </summary>
        Public Const NOT_ASSIGN As String = NameOf(NOT_ASSIGN)

        ''' <summary>
        ''' 默认的柱状图大分类下的每一个柱子的颜色
        ''' </summary>
        Public ReadOnly DefaultColorSchema As [Default](Of String) = "Set1:c6"
        ''' <summary>
        ''' 解决黄色看不清的问题？
        ''' </summary>
        Public ReadOnly DefaultKEGGColorSchema As [Default](Of String) = "#E41A1C,#377EB8,#4DAF4A,#984EA3,#FF7F00,#CECE00"

        <Extension>
        Private Function removesNotAssign(profile As CatalogProfiles) As CatalogProfiles
            profile = New CatalogProfiles(profile)

            For Each keyRemove As String In {NOT_ASSIGN, "Brite Hierarchies", "Not Included in Pathway or Brite"}
                With profile.Keys.FirstOrDefault(Function(key) InStr(key, keyRemove, CompareMethod.Text) > 0)
                    If Not .StringEmpty Then
                        Call .DoCall(AddressOf profile.delete)
                    End If
                End With
            Next

            Return profile
        End Function

        ''' <summary>
        ''' Catalog profiling bar plot
        ''' </summary>
        ''' <param name="profile"></param>
        ''' <param name="title"></param>
        ''' <param name="colorSchema"></param>
        ''' <param name="bg"></param>
        ''' <param name="size"></param>
        ''' <param name="classFontStyle"></param>
        ''' <param name="catalogFontStyle"></param>
        ''' <param name="titleFontStyle"></param>
        ''' <param name="tick">
        ''' parameter value of ``-1`` means create ticks value sequence automatically. 
        ''' </param>
        ''' <returns></returns>
        <Extension>
        Public Function ProfilesPlot(profile As CatalogProfiles,
                                     Optional title$ = "Profiling Plot",
                                     Optional axisTitle$ = "Number Of Gene",
                                     Optional colorSchema$ = "Set1:c6",
                                     Optional bg$ = "white",
                                     Optional size$ = "2200,2000",
                                     Optional padding$ = "padding: 125 25 25 25",
                                     Optional classFontStyle$ = CSSFont.Win7LargerBold,
                                     Optional catalogFontStyle$ = CSSFont.Win7Bold,
                                     Optional titleFontStyle$ = CSSFont.PlotTitle,
                                     Optional valueFontStyle$ = CSSFont.Win7Bold,
                                     Optional tickFontStyle$ = CSSFont.Win7LargerBold,
                                     Optional tick# = 50,
                                     Optional removeNotAssign As Boolean = True,
                                     Optional gray As Boolean = False,
                                     Optional labelRightAlignment As Boolean = False,
                                     Optional disableLabelColor As Boolean = False,
                                     Optional valueFormat$ = "F2",
                                     Optional labelTrimLength% = 64,
                                     Optional dpi As Integer = 300) As GraphicsData

            If removeNotAssign Then
                profile = profile.removesNotAssign
            End If

            Dim colors As ColorProfile = profile.GetColors(colorSchema, logarithm:=-1)
            Dim mapperValues As Double() = profile.catalogs.Values _
                .Select(Function(c)
                            Return c.AsEnumerable.Select(Function(v) v.Value)
                        End Function) _
                .IteratesALL _
                .ToArray
            Dim plotInternal =
                Sub(ByRef g As IGraphics, region As GraphicsRegion)
                    If mapperValues.IsNullOrEmpty Then
                        ' just do nothing?
                    Else
                        Dim mapper As New Scaling(mapperValues, horizontal:=True)

                        Call g.internalPlotImpl(
                           region, profile, title,
                           colors,
                           titleFontStyle, catalogFontStyle, classFontStyle, valueFontStyle,
                           New Mapper(mapper, ignoreAxis:=True),
                           tickFontStyle, tick,
                           axisTitle,
                           gray:=gray,
                           labelAlignmentRight:=labelRightAlignment,
                           valueFormat:=valueFormat,
                           disableLabelColor:=disableLabelColor,
                           labelTrimLength:=labelTrimLength,
                           dpi:=dpi
                        )
                    End If
                End Sub

            Call $"Run catalog profile bar plot with size={size}, dpi={dpi}".__DEBUG_ECHO

            Return g.GraphicsPlots(size.SizeParser, padding, bg, plotInternal, Drivers.GDI, $"{dpi},{dpi}")
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="region"></param>
        ''' <param name="profile"></param>
        ''' <param name="title$">图形上方的标题字符串</param>
        ''' <param name="colors"></param>
        ''' <param name="titleFontStyle$"></param>
        ''' <param name="catalogFontStyle$"></param>
        ''' <param name="classFontStyle$"></param>
        ''' <param name="valueFontStyle$"></param>
        ''' <param name="mapper"></param>
        ''' <param name="tickFontStyle$"></param>
        ''' <param name="tick#"></param>
        ''' <param name="axisTitle$"></param>
        ''' <param name="gray">条形图使用灰色的颜色，不再根据分类而产生不同颜色了</param>
        <Extension>
        Private Sub internalPlotImpl(ByRef g As IGraphics, region As GraphicsRegion,
                                     profile As CatalogProfiles,
                                     title$,
                                     colors As ColorProfile,
                                     titleFontStyle$,
                                     catalogFontStyle$,
                                     classFontStyle$, valueFontStyle$,
                                     mapper As Mapper,
                                     tickFontStyle$,
                                     tick#,
                                     axisTitle$,
                                     gray As Boolean,
                                     labelAlignmentRight As Boolean,
                                     disableLabelColor As Boolean,
                                     valueFormat$,
                                     labelTrimLength%,
                                     dpi%)
            ' 这里是大标签的字符串向量
            Dim classes$() = profile.Keys.ToArray
            Dim titleFont As Font = CSSFont.TryParse(titleFontStyle).GDIObject(dpi)
            Dim catalogFont As Font = CSSFont.TryParse(catalogFontStyle).GDIObject(dpi)
            Dim catalogCharWidth! = g.MeasureString("A", catalogFont).Width
            Dim classFont As Font = CSSFont.TryParse(classFontStyle).GDIObject(dpi)
            Dim padding As Padding = region.Padding
            Dim size As Size = region.Size
            Dim maxLenSubKey$ = profile.catalogs.Values _
                .Select(Function(o) o.AsEnumerable.Select(Function(oo) oo.Name)) _
                .IteratesALL _
                .Where(Function(name)
                           ' 2017-12-27
                           ' 因为下面会将长度大于64的名字给截断，所以在这里需要将这些比较长的名字给过滤掉
                           ' 否则会出现很大的空白的绘图bug
                           If labelTrimLength <= 0 Then
                               ' no trim
                               Return True
                           Else
                               Return name.Length <= labelTrimLength
                           End If
                       End Function) _
                .OrderByDescending(Function(s) s.Length) _
                .First
            Dim maxLenClsKey$ = classes _
                .OrderByDescending(Function(s) s.Length) _
                .First

            ' 这里要判断一下，否则绘制结果仍然是和没有限制长度的结果一样
            If labelTrimLength > 0 AndAlso maxLenSubKey.Length > labelTrimLength Then
                ' 2017-9-30
                ' 因为用的不是等宽字体，所以在这里使用字母来作为计算长度的单位会更加的合理一些
                maxLenSubKey = New String("a"c, labelTrimLength + 4)
            End If

            Dim maxLenSubKeySize As SizeF = g.MeasureString(maxLenSubKey, catalogFont)
            Dim maxLenClsKeySize As SizeF = g.MeasureString(maxLenClsKey, classFont)
            Dim valueFont As Font = CSSFont.TryParse(valueFontStyle).GDIObject(dpi)

            ' 所绘制的图形的总的高度
            Dim totalHeight = classes.Length * (maxLenClsKeySize.Height + 5) +
                profile.TotalTerms * (maxLenSubKeySize.Height + 4) +
                classes.Length * 20
            Dim left!
            Dim y! = region.Padding.Top + (region.PlotRegion.Height - totalHeight) / 2

            ' barPlot的最左边的坐标
            Dim maxLabeLength% = CInt(Math.Max(maxLenSubKeySize.Width, maxLenClsKeySize.Width))
            Dim barRect As New RectangleF With {
                .X = CSng(padding.Left * 1.5 + maxLabeLength),
                .Y = y,
                .Width = CSng(size.Width - padding.Horizontal - maxLabeLength - padding.Left / 2),
                .Height = totalHeight
            }

            left = barRect.Left - padding.Left
            left = (size.Width - padding.Horizontal - left) / 2 + left + padding.Left

            Dim titleSize As SizeF = g.MeasureString(title, titleFont)
            Dim anchor As New PointF With {
                .X = barRect.Left + (barRect.Width - titleSize.Width) / 2,
                .Y = (y - titleSize.Height) / 2.0!
            }
            Dim rectangleStroke As New Pen(Color.Black, 5)

            ' 在这里进行plot的标题的绘制操作
            Call g.DrawString(title, titleFont, Brushes.Black, anchor)

            If TypeOf g Is GraphicsSVG Then
                ' 如果是SVG文档，则还需要设置一下rectangle的填充，要不然会被默认填充为黑色背景
                DirectCast(g, GraphicsSVG).DrawRectangle(rectangleStroke, barRect, Color.White)
            Else
                Call g.DrawRectangle(rectangleStroke, barRect)
            End If

            Dim gap! = 10.0!
            Dim grayColor As [Default](Of Color) = Color.Gray.AsDefault(Function() gray)

            left = padding.Left

            For Each [class] As SeqValue(Of String) In classes.SeqIterator
                Dim yPlot!
                Dim barWidth!
                Dim barRectPlot As Rectangle
                Dim valueSize As SizeF
                Dim valueLeft!
                Dim valueLabel$
                Dim offset!

                ' 绘制Class大分类的标签
                g.DrawString([class], classFont, Brushes.Black, New PointF(left, y))
                y += maxLenClsKeySize.Height + 5

                ' 绘制统计的小分类标签以及barplot图形
                For Each term As NamedValue(Of Double) In profile([class].value).AsEnumerable
                    Dim color As New SolidBrush(colors.GetColor(term))
                    Dim penColor As Color = color.Color Or grayColor
                    Dim linePen As New Pen(penColor, 2) With {
                        .DashStyle = DashStyle.Dot
                    }
                    Dim pos As PointF
                    Dim label$

                    If gray Then
                        color = New SolidBrush("rgb(30,30,30)".TranslateColor)
                    End If

                    If labelTrimLength > 0 AndAlso term.Name.Length > labelTrimLength Then
                        label = Mid(term.Name, 1, labelTrimLength - 1) & "..."
                    Else
                        label = term.Name
                    End If

                    If labelAlignmentRight Then
                        ' 重新计算位置进行右对齐操作
                        offset! = CSng(label.Length / (region.Width / 20) * catalogCharWidth)
                        offset! = barRect.Left - 25 - g.MeasureString(label, catalogFont).Width + offset
                        pos = New PointF With {
                            .X = offset,
                            .Y = y
                        }
                    Else
                        ' 分类标签相对于大分类标签而言在水平方向上有25个像素的偏移
                        pos = New PointF(left + 25, y)
                    End If

                    If TypeOf colors Is CategoryColorProfile Then
                        If Not disableLabelColor Then
                            Call g.DrawString(label, catalogFont, color, pos)
                        Else
                            Call g.DrawString(label, catalogFont, Brushes.Black, pos)
                        End If
                    Else
                        Call g.DrawString(label, catalogFont, Brushes.Black, pos)
                    End If

                    ' 绘制虚线
                    yPlot = y + maxLenSubKeySize.Height / 2
                    barWidth = mapper.ScallingWidth(term.Value, barRect.Width - gap)
                    barRectPlot = New Rectangle With {
                        .Location = New Point(barRect.Left, y),
                        .Size = New Size With {
                            .Width = barWidth - gap,
                            .Height = maxLenSubKeySize.Height
                        }
                    }

                    valueLabel = term.Value.ToString(valueFormat)
                    valueSize = g.MeasureString(valueLabel, valueFont)
                    valueLeft = barRectPlot.Right - valueSize.Width

                    If valueLeft < barRect.Left Then
                        valueLeft = barRect.Left + 2
                    End If

                    Call g.DrawLine(linePen, New Point(barRect.Left, yPlot), New Point(barRect.Right, yPlot))
                    Call g.FillRectangle(color, barRectPlot)

                    If Not gray Then
                        ' 如果是灰度的图，就不需要再绘制值得标签字符串了，
                        ' 因为灰色和黑色的颜色太相近了， 看不清楚
                        anchor = New PointF With {
                            .X = valueLeft,
                            .Y = y - valueSize.Height / 3
                        }
                        Call g.DrawString(valueLabel, valueFont, Brushes.Black, anchor)
                    End If

                    y += maxLenSubKeySize.Height + 4
                Next

                y += 20
            Next

            ' 开始进行标尺的绘制
            Dim maxValue# = profile.MaxValue
            Dim axisTicks#() = GetTicks(maxValue, tick)
            Dim d# = 25
            Dim tickFont As Font = CSSFont.TryParse(tickFontStyle).GDIObject(dpi)
            Dim tickSize As SizeF
            Dim tickPen As New Pen(Color.Black, 3)
            Dim tickX!
            Dim isIntCount As Boolean = axisTicks.All(Function(ti) Math.Floor(ti) = Math.Ceiling(ti))

            For Each tick In axisTicks.Where(Function(v) v < maxValue)
                tickX = barRect.Left + mapper.ScallingWidth(tick, barRect.Width - gap)
                tickSize = g.MeasureString(tick, tickFont)

                If TypeOf g Is GraphicsSVG Then
                    anchor = New PointF With {
                        .X = tickX - tickSize.Width,
                        .Y = y + d + 10
                    }
                Else
                    anchor = New PointF With {
                        .X = tickX - tickSize.Width / 2,
                        .Y = y + d + 10
                    }
                End If

                Call g.DrawLine(tickPen, New PointF(tickX, y), New PointF(tickX, y + d))

                If isIntCount Then
                    Call g.DrawString(CInt(tick).ToString, tickFont, Brushes.Black, anchor)
                Else
                    Call g.DrawString(tick.ToString("F2"), tickFont, Brushes.Black, anchor)
                End If
            Next

            y += d + 10 + g.MeasureString("0", tickFont).Height

            titleFont = CSSFont.TryParse(CSSFont.Win7LargerBold).GDIObject(dpi)
            titleSize = g.MeasureString(title, titleFont)
            left = barRect.Left + (barRect.Width - titleSize.Width) / 2

            Call g.DrawString(axisTitle, titleFont, Brushes.Black, New PointF(left, y))
        End Sub

        ''' <summary>
        ''' ``<paramref name="tick"/>=-1``的时候表示自动生成序列
        ''' 反之表示手动生成序列
        ''' </summary>
        ''' <param name="max#"></param>
        ''' <param name="tick!">
        ''' 如果间隔参数是小于零的话，函数则会自动根据[0, <paramref name="max"/>]区间来生成tick，
        ''' 否则会直接使用<paramref name="tick"/>间隔叠加到<paramref name="max"/>产生tick序列
        ''' </param>
        ''' <returns></returns>
        Public Function GetTicks(max#, tick!) As Double()
            If tick <= 0 Then
                ' 自动生成
                Call "Ticks created from auto axis ticking...".__INFO_ECHO
                Return AxisScalling.CreateAxisTicks({0, max}.AsEnumerable, ticks:=5)
            Else
                Call "Ticks created from tick sequence...".__INFO_ECHO
                Return AxisScalling.GetAxisByTick(max, tick)
            End If
        End Function
    End Module
End Namespace
