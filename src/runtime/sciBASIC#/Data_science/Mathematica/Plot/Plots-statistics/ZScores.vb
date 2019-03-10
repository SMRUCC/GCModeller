﻿#Region "Microsoft.VisualBasic::7051fd0e8abb85e4f160903cc95f6fd4, Data_science\Mathematica\Plot\Plots-statistics\ZScores.vb"

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

    ' Module ZScoresPlot
    ' 
    '     Function: Plot
    ' 
    ' Structure ZScores
    ' 
    '     Function: (+3 Overloads) Load, Range
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Scripting
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports Microsoft.VisualBasic.Scripting.Runtime

''' <summary>
''' Plot of the <see cref="Distributions.Z"/>
''' </summary>
Public Module ZScoresPlot

    <Extension>
    Public Function Plot(data As ZScores,
                         Optional size$ = "3300,3400",
                         Optional margin$ = Canvas.Resolution2K.PaddingWithRightLegendAndBottomTitle,
                         Optional bg$ = "white",
                         Optional title$ = "Z-scores",
                         Optional titleFontCSS$ = CSSFont.Win7VeryVeryLarge,
                         Optional serialLabelFontCSS$ = CSSFont.Win7VeryLarge,
                         Optional legendLabelFontCSS$ = CSSFont.Win7VeryLarge,
                         Optional tickFontCSS$ = CSSFont.Win7LargeBold,
                         Optional pointWidth! = 50,
                         Optional axisStrokeCSS$ = Stroke.AxisStroke,
                         Optional legendBoxStroke$ = Stroke.AxisStroke,
                         Optional displayZERO As Boolean = True,
                         Optional ZEROStrokeCSS$ = Stroke.AxisGridStroke) As GraphicsData

        Dim ticks#() = data.Range.CreateAxisTicks
        Dim range As DoubleRange = ticks
        Dim maxGroupLabel$ = data.groups.Keys.MaxLengthString
        Dim maxSerialsLabel$ = data.serials.Keys.MaxLengthString
        Dim serialLabelFont As Font = CSSFont.TryParse(serialLabelFontCSS)
        Dim legendLabelFont As Font = CSSFont.TryParse(legendLabelFontCSS)
        Dim titleFont As Font = CSSFont.TryParse(titleFontCSS)
        Dim tickFont As Font = CSSFont.TryParse(tickFontCSS)
        Dim groups = data.groups
        Dim colors = data.colors
        Dim pointSize As New SizeF(pointWidth, pointWidth)
        Dim axisStroke As Pen = Stroke.TryParse(axisStrokeCSS).GDIObject

        Dim plotInternal =
            Sub(ByRef g As IGraphics, rect As GraphicsRegion)

                Dim maxSerialLabelSize As SizeF = g.MeasureString(maxSerialsLabel, serialLabelFont)
                Dim maxLegendLabelSize As SizeF = g.MeasureString(maxGroupLabel, legendLabelFont)

                ' 计算出layout信息
                Dim plotWidth% = rect.PlotRegion.Width _
                                 - maxSerialLabelSize.Width _
                                 - maxLegendLabelSize.Width _
                                 - maxLegendLabelSize.Height _
                                 - 30
                Dim plotHeight = rect.PlotRegion.Height - titleFont.Height - tickFont.Height - 20
                Dim plotWidthRange As DoubleRange = {0, plotWidth}
                Dim X = Function(Z#)
                            Return rect.Padding.Left _
                                   + maxSerialLabelSize.Width _
                                   + 5 _
                                   + range.ScaleMapping(Z, plotWidthRange)
                        End Function
                Dim dy! = plotHeight / (data.serials.Length)
                Dim yTop! = rect.Padding.Top
                Dim left! = X(range.Min)
                Dim labelSize As SizeF
                Dim labelPosition As PointF
                Dim pt As PointF

                ' 分别绘制出X坐标轴和Y坐标轴
                g.DrawLine(axisStroke, New PointF(left, yTop), New PointF(left, yTop + plotHeight))
                g.DrawLine(axisStroke,
                           New PointF(left, yTop + plotHeight),
                           New PointF(left + plotWidth, yTop + plotHeight))

                If displayZERO Then
                    Dim zeroPen As Pen = Stroke.TryParse(ZEROStrokeCSS).GDIObject

                    g.DrawLine(zeroPen,
                               New PointF(X(0), yTop),
                               New PointF(X(0), yTop + plotHeight))
                End If

                ' 绘制出每一个系列的点和相应的标签字符串
                For Each serial As DataSet In data.serials
                    Dim labelY = yTop + (dy - serialLabelFont.Height) / 2
                    Dim yPoints! = yTop + (dy - pointWidth) / 2

                    labelSize = g.MeasureString(serial.ID, serialLabelFont)
                    labelPosition = New PointF(left - labelSize.Width, labelY)
                    g.DrawString(serial.ID, serialLabelFont, Brushes.Black, labelPosition)

                    For Each group As KeyValuePair(Of String, String()) In groups
                        Dim color As New SolidBrush(colors(group.Key))

                        For Each Z As Double In serial(group.Value) _
                            .Where(Function(n)
                                       Return Not n.IsNaNImaginary
                                   End Function)

                            pt = New PointF(X(Z), yPoints)
                            g.FillEllipse(color, New RectangleF(pt, pointSize))
                        Next
                    Next

                    yTop += dy
                Next

                ' 绘制出X轴的ticks
                For Each tick As Double In ticks
                    labelSize = g.MeasureString(tick, tickFont)
                    pt = New PointF(X(tick), yTop)
                    labelPosition = New PointF With {
                        .X = pt.X - labelSize.Width / 2,
                        .Y = yTop + 10
                    }

                    g.DrawString(tick, tickFont, Brushes.Black, labelPosition)
                    g.DrawLine(Pens.Black, New PointF(pt.X, yTop), New PointF(pt.X, yTop + 8))
                Next

                ' 绘制出标题
                yTop! = rect.Padding.Top
                labelSize = g.MeasureString(title, titleFont)
                labelPosition = New PointF With {
                    .X = left + (plotWidth - labelSize.Width) / 2,
                    .Y = yTop + plotHeight + tickFont.Height + 20
                }

                g.DrawString(title, titleFont, Brushes.Black, labelPosition)

                ' 绘制legend
                Dim legendHeight! = (legendLabelFont.Height + 5) * groups.Count
                Dim maxWidth = maxLegendLabelSize.Width + legendLabelFont.Height * 3
                Dim legendLocation As New Point With {
                    .X = X(range.Max) + (rect.Padding.Right - maxWidth) / 2,
                    .Y = yTop + (plotHeight - legendHeight) / 2
                }
                Dim shapes = data.shapes
                Dim legends = groups _
                    .Keys _
                    .Select(Function(label)
                                Return New Legend With {
                                    .title = label,
                                    .color = colors(label).RGBExpression,
                                    .fontstyle = legendLabelFontCSS,
                                    .style = shapes(label)
                                }
                            End Function) _
                    .ToArray
                Dim legendBoxBorder As Stroke = Stroke.TryParse(legendBoxStroke)

                Call g.DrawLegends(
                    topLeft:=legendLocation,
                    legends:=legends,
                    gSize:=$"{legendLabelFont.Height * 2},{legendLabelFont.Height}",
                    regionBorder:=legendBoxBorder
                )
            End Sub

        Return g.GraphicsPlots(
            size.SizeParser, margin,
            bg,
            plotInternal
        )
    End Function
End Module

Public Structure ZScores

    Dim serials As DataSet()

    Dim groups As Dictionary(Of String, String())
    ''' <summary>
    ''' Colors for the <see cref="groups"/>
    ''' </summary>
    Dim colors As Dictionary(Of String, Color)
    Dim shapes As Dictionary(Of String, LegendStyles)

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Range() As DoubleRange
        Return serials _
            .Select(Function(d) d.Properties.Values) _
            .IteratesALL _
            .Where(Function(x) Not x.IsNaNImaginary) _
            .Range
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="path$"></param>
    ''' <param name="groups"></param>
    ''' <param name="colors"></param>
    ''' <param name="transpose">是否需要将<paramref name="path"/>所加载上来的矩阵进行转置处理</param>
    ''' <param name="labelTranslate"></param>
    ''' <returns></returns>
    Public Shared Function Load(path$,
                                groups As Dictionary(Of String, String()),
                                colors As Color(),
                                Optional transpose As Boolean = False,
                                Optional labelTranslate As Dictionary(Of String, String) = Nothing) As ZScores

        Dim datalist As DataSet() = DataSet _
            .LoadDataSet(path) _
            .ToArray

        If transpose Then
            datalist = datalist.Transpose
        End If

        Return ZScores.Load(datalist, groups, colors, labelTranslate)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="groups"></param>
    ''' <param name="colors"></param>
    ''' <returns></returns>
    Public Shared Function Load(datalist As DataSet(),
                                groups As Dictionary(Of String, String()),
                                colors As Color(),
                                Optional labelTranslate As Dictionary(Of String, String) = Nothing) As ZScores

        Dim colorlist As LoopArray(Of Color) = colors

        If labelTranslate Is Nothing Then
            labelTranslate = New Dictionary(Of String, String)
        End If

        Dim names As New NamedVectorFactory(datalist.PropertyNames)
        Dim zscores = datalist _
            .Select(Function(serial)
                        Dim z As Vector = names _
                            .AsVector(serial.Properties) _
                            .Z()
                        Dim label$ = labelTranslate.TryGetValue(
                            index:=serial.ID,
                            [default]:=serial.ID
                        )

                        Return New DataSet With {
                            .ID = label,
                            .Properties = names.Translate(z)
                        }
                    End Function) _
            .ToArray
        Dim colorProfile = groups _
            .ToDictionary(Function(x) x.Key,
                          Function(x) colorlist.Next)
        Dim shapeProfile = groups _
            .ToDictionary(Function(x) x.Key,
                          Function(x) LegendStyles.Circle)

        Return New ZScores With {
            .serials = zscores,
            .groups = groups,
            .colors = colorProfile,
            .shapes = shapeProfile
        }
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="path$"></param>
    ''' <param name="groups"></param>
    ''' <param name="colors$"></param>
    ''' <param name="transpose">是否需要将<paramref name="path"/>所加载上来的矩阵进行转置处理</param>
    ''' <param name="labelTranslate">
    ''' 对<paramref name="path"/>所加载的矩阵之中的rownames进行显示标题的转换
    ''' </param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function Load(path$, groups As Dictionary(Of String, String()),
                                Optional colors$ = ColorBrewer.QualitativeSchemes.Set2_8,
                                Optional transpose As Boolean = False,
                                Optional labelTranslate As Dictionary(Of String, String) = Nothing) As ZScores
        Return ZScores.Load(
            path, groups,
            Designer.GetColors(colors),
            transpose:=transpose,
            labelTranslate:=labelTranslate
        )
    End Function
End Structure
