﻿#Region "Microsoft.VisualBasic::065b056b75913532205184148d4089e5, Data_science\Mathematica\Plot\Plots\Scatter\Scatter.vb"

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

    ' Module Scatter
    ' 
    '     Function: CreateAxisTicks
    ' 
    '     Sub: drawErrorLine
    '     Enum Splines
    ' 
    '         B_Spline, Bezier, CatmullRomSpline, CentripetalCatmullRomSpline, CubicSpline
    ' 
    ' 
    ' 
    '  
    ' 
    '     Function: (+2 Overloads) FromPoints, FromVector, (+5 Overloads) Plot, PlotFunction
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.d3js.scale
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Shapes
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Scripting
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports Microsoft.VisualBasic.Scripting.Runtime

Public Module Scatter

    <Extension>
    Private Sub drawErrorLine(canvas As IGraphics, scaler As DataScaler, pt As PointF, value#, width!, color As SolidBrush)
        Dim p0 As New PointF With {
            .X = pt.X,
            .Y = scaler.TranslateY(value)
        }

        ' 下面分别绘制竖线误差线以及横线
        Call canvas.DrawLine(New Pen(color), pt, p0)
        Call canvas.DrawLine(New Pen(color), CSng(p0.X - width), p0.Y, CSng(p0.X + width), p0.Y)
    End Sub

    <Extension>
    Public Function CreateAxisTicks(array As SerialData(), Optional preferPositive As Boolean = False, Optional scale# = 1.2) As (x As Double(), y As Double())
        Dim ptX#() = array _
            .Select(Function(s)
                        Return s.pts.Select(Function(pt) CDbl(pt.pt.X))
                    End Function) _
           .IteratesALL _
           .ToArray
        Dim XTicks = ptX _
           .Range(scale) _
           .CreateAxisTicks
        Dim YTicks = array _
            .Select(Function(s)
                        Return s.pts _
                            .Select(Function(pt)
                                        Return {
                                            pt.pt.Y - pt.errMinus,
                                            pt.pt.Y + pt.errPlus
                                        }
                                    End Function)
                    End Function) _
            .IteratesALL _
            .IteratesALL _
            .Range _
            .CreateAxisTicks

        If preferPositive AndAlso Not ptX.Any(Function(n) n < 0) Then
            ' 全部都是正实数，则将可能的负实数去掉
            '
            ' 因为在下面的Range函数里面，是根据scale来将最大值加上一个delta值，最小值减去一个delta值来得到scale之后的结果，
            ' 所以假若X有比较接近于零的值得花， scale之后会出现负数
            ' 这个负数很明显是不合理的，所以在这里将负数删除掉
            With ptX.Range(scale)
                XTicks = New DoubleRange(0, .Max).CreateAxisTicks
            End With
        End If

        Return (XTicks, YTicks)
    End Function

    ''' <summary>
    ''' 线条插值类型
    ''' </summary>
    Public Enum Splines As Byte
        ''' <summary>
        ''' 无插值操作
        ''' </summary>
        None = 0
        ''' <summary>
        ''' 二次插值
        ''' </summary>
        B_Spline
        ''' <summary>
        ''' 贝塞尔曲线插值
        ''' </summary>
        Bezier
        CatmullRomSpline
        CentripetalCatmullRomSpline
        ''' <summary>
        ''' 三次插值处理
        ''' </summary>
        CubicSpline
    End Enum

    ''' <summary>
    ''' Scatter plot function.(绘图函数，默认的输出大小为``4300px,2000px``)
    ''' </summary>
    ''' <param name="c"></param>
    ''' <param name="size"></param>
    ''' <param name="bg"></param>
    ''' <param name="fill">是否对曲线下的区域进行填充？这个参数只有在<paramref name="drawLine"/>开启的情况下才会发生作用</param>
    ''' <param name="drawLine">
    ''' 是否绘制两个点之间的连接线段，当这个参数为False的时候，将不会绘制连线，就相当于绘制散点图了，而非折线图
    ''' </param>
    ''' <param name="xaxis">
    ''' 参数<paramref name="xaxis"/>和<paramref name="yaxis"/>必须要同时不为空才会起作用
    ''' </param>
    ''' <param name="legendSize">默认为(120,40)</param>
    ''' <param name="preferPositive"><see cref="CreateAxisTicks"/></param>
    ''' <returns></returns>
    <Extension>
    Public Function Plot(c As IEnumerable(Of SerialData),
                         Optional size$ = "3600,2700",
                         Optional padding$ = g.DefaultUltraLargePadding,
                         Optional bg$ = "white",
                         Optional showGrid As Boolean = True,
                         Optional showLegend As Boolean = True,
                         Optional legendPosition As Point = Nothing,
                         Optional legendSize$ = "100,50",
                         Optional drawLine As Boolean = True,
                         Optional legendBorder As Stroke = Nothing,
                         Optional legendRegionBorder As Stroke = Nothing,
                         Optional fill As Boolean = False,
                         Optional fillPie As Boolean = True,
                         Optional legendFontSize! = 48,
                         Optional absoluteScaling As Boolean = True,
                         Optional XaxisAbsoluteScalling As Boolean = False,
                         Optional YaxisAbsoluteScalling As Boolean = False,
                         Optional drawAxis As Boolean = True,
                         Optional Xlabel$ = "X",
                         Optional Ylabel$ = "Y",
                         Optional yaxis$ = Nothing,
                         Optional xaxis$ = Nothing,
                         Optional ablines As Line() = Nothing,
                         Optional htmlLabel As Boolean = True,
                         Optional ticksY# = -1,
                         Optional preferPositive As Boolean = False,
                         Optional interplot As Splines = Splines.None,
                         Optional densityColor As Boolean = False,
                         Optional tickFontStyle$ = CSSFont.Win7VeryLarge,
                         Optional labelFontStyle$ = CSSFont.Win7VeryVeryLarge) As GraphicsData

        Dim margin As Padding = padding
        Dim array As SerialData() = c.ToArray
        Dim XTicks#(), YTicks#()

        With array.CreateAxisTicks(preferPositive)
            XTicks = .x
            YTicks = .y
        End With

        If ticksY > 0 Then
            YTicks = AxisScalling.GetAxisByTick(YTicks, tick:=ticksY)
        End If

        Dim plotInternal =
            Sub(ByRef g As IGraphics, rect As GraphicsRegion)

                Dim region As Rectangle = rect.PlotRegion
                Dim X, Y As d3js.scale.LinearScale

                If Not xaxis.StringEmpty AndAlso Not yaxis.StringEmpty Then
                    XTicks = AxisProvider.TryParse(xaxis).AxisTicks
                    YTicks = AxisProvider.TryParse(yaxis).AxisTicks
                    X = XTicks.LinearScale.range(integers:={region.Left, region.Right})
                    Y = YTicks.LinearScale.range(integers:={region.Bottom, region.Top})
                Else
                    X = d3js.scale.linear.domain(XTicks).range(integers:={region.Left, region.Right})
                    Y = d3js.scale.linear.domain(YTicks).range(integers:={region.Bottom, region.Top})
                End If

                Dim scaler As New DataScaler With {
                    .X = X,
                    .Y = Y,
                    .Region = region,
                    .AxisTicks = (XTicks, YTicks)
                }
                Dim gSize As Size = rect.Size

                If drawAxis Then
                    Call g.DrawAxis(
                        rect, scaler, showGrid,
                        xlabel:=Xlabel, ylabel:=Ylabel,
                        htmlLabel:=htmlLabel,
                        tickFontStyle:=tickFontStyle,
                        labelFont:=labelFontStyle
                    )
                End If

                Dim width = rect.PlotRegion.Width / 200

                For Each line As SerialData In array
                    Dim pts = line.pts.SlideWindows(2)
                    Dim pen As Pen = line.GetPen
                    Dim br As New SolidBrush(line.color)
                    Dim fillBrush As New SolidBrush(Color.FromArgb(100, baseColor:=line.color))
                    Dim d! = line.PointSize
                    Dim r As Single = line.PointSize / 2
                    Dim bottom! = gSize.Height - margin.Bottom
                    Dim getPointBrush = Function(pt As PointData)
                                            If pt.color.StringEmpty Then
                                                Return br
                                            Else
                                                Return pt.color.GetBrush
                                            End If
                                        End Function
                    Dim pt1, pt2 As PointF

                    For Each pt As SlideWindow(Of PointData) In pts
                        Dim a As PointData = pt.First
                        Dim b As PointData = pt.Last

                        pt1 = scaler.Translate(a.pt.X, a.pt.Y)
                        pt2 = scaler.Translate(b.pt.X, b.pt.Y)

                        If drawLine Then
                            Call g.DrawLine(pen, pt1, pt2)
                        End If

                        If fill Then
                            Dim path As New GraphicsPath
                            Dim ptc As New PointF(pt2.X, bottom) ' c
                            Dim ptd As New PointF(pt1.X, bottom) ' d


                            '   /-b
                            ' a-  |
                            ' |   |
                            ' |   |
                            ' d---c

                            path.AddLine(pt1, pt2)
                            path.AddLine(pt2, ptc)
                            path.AddLine(ptc, ptd)
                            path.AddLine(ptd, pt1)
                            path.CloseFigure()

                            Call g.FillPath(fillBrush, path)
                        End If

                        If fillPie Then
                            Call g.FillPie(getPointBrush(a), pt1.X - r, pt1.Y - r, d, d, 0, 360)
                            Call g.FillPie(getPointBrush(b), pt2.X - r, pt2.Y - r, d, d, 0, 360)
                        End If

                        ' 绘制误差线
                        ' 首先计算出误差的长度，然后可pt1,pt2的Y相加减即可得到新的位置
                        ' 最后划线即可
                        If a.errPlus > 0 Then
                            Call g.drawErrorLine(scaler, pt1, a.errPlus + a.pt.Y, width, br)
                        End If
                        If a.errMinus > 0 Then
                            Call g.drawErrorLine(scaler, pt1, a.pt.Y - a.errMinus, width, br)
                        End If
                        If b.errPlus > 0 Then
                            Call g.drawErrorLine(scaler, pt2, b.errPlus + b.pt.Y, width, br)
                        End If
                        If b.errMinus > 0 Then
                            Call g.drawErrorLine(scaler, pt2, b.pt.Y - b.errMinus, width, br)
                        End If
                    Next

                    If Not line.DataAnnotations.IsNullOrEmpty Then
                        Dim raw = array.Where(Function(s) s.title = line.title).First

                        For Each annotation As Annotation In line.DataAnnotations
                            Call annotation.Draw(g, scaler, raw, rect)
                        Next
                    End If

                    If showLegend Then
                        Dim legends = LinqAPI.Exec(Of Legend) _
 _
                        () <= From s As SerialData
                              In array
                              Let sColor As String = s.color.RGBExpression
                              Let legendFont = CSSFont.GetFontStyle(
                                  FontFace.SegoeUI,
                                  FontStyle.Regular,
                                  legendFontSize)
                              Select New Legend With {
                                  .color = sColor,
                                  .fontstyle = legendFont,
                                  .style = LegendStyles.Circle,
                                  .title = s.title
                                  }
                        Dim lsize As Size = legendSize.SizeParser

                        If legendPosition.IsEmpty Then
                            Dim maxLen = legends.Select(Function(l) l.title).MaxLengthString
                            Dim lFont As Font = CSSFont.TryParse(legends.First.fontstyle).GDIObject
                            Dim maxWidth = g.MeasureString(maxLen, lFont).Width

                            legendPosition = New Point With {
                                .X = region.Size.Width - lsize.Width / 2 - maxWidth,
                                .Y = margin.Top + lFont.Height
                            }
                        End If

                        Call g.DrawLegends(
                            legendPosition, legends, legendSize,,
                            legendBorder,
                            legendRegionBorder)
                    End If
                Next

                ' draw ablines
                For Each line As Line In ablines.SafeQuery
                    Dim a As PointF = scaler.Translate(line.A)
                    Dim b As PointF = scaler.Translate(line.B)

                    Call g.DrawLine(line.Stroke, a, b)
                Next
            End Sub

        Return g.GraphicsPlots(size.SizeParser, margin, bg, plotInternal)
    End Function

    Public Function Plot(x As Vector,
                         Optional size$ = "1600,1200",
                         Optional padding$ = g.DefaultPadding,
                         Optional bg As String = "white",
                         Optional ptSize As Single = 15,
                         Optional width As Single = 5,
                         Optional drawLine As Boolean = False) As GraphicsData
        Return {
            FromVector(x,,, ptSize, width)
        }.Plot(size, padding, bg, True, False, , drawLine:=drawLine)
    End Function

    Public Function FromVector(y As IEnumerable(Of Double),
                               Optional color As String = "black",
                               Optional dash As DashStyle = DashStyle.Dash,
                               Optional ptSize! = 30,
                               Optional width As Single = 5,
                               Optional xrange As IEnumerable(Of Double) = Nothing,
                               Optional title$ = "Vector Plot",
                               Optional alpha% = 255) As SerialData
        Dim array#()
        Dim y0#() = y.ToArray

        If xrange Is Nothing Then
            array = Math.seq(0, y0.Length, 1)
        Else
            array = xrange.ToArray
        End If

        Return New SerialData With {
            .color = Drawing.Color.FromArgb(alpha, color.ToColor),
            .lineType = dash,
            .PointSize = ptSize,
            .title = title,
            .width = width,
            .pts = LinqAPI.Exec(Of PointData) <=
 _
                From o As SeqValue(Of Double)
                In y0.SeqIterator
                Where Not o.value.IsNaNImaginary
                Select New PointData With {
                    .pt = New PointF(array(o.i), CSng(o.value))
                }
                    }
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="range">这个参数之中的Name属性是为了设置表达式计算之中的变量的目的</param>
    ''' <param name="expression$"></param>
    ''' <param name="steps#"></param>
    ''' <param name="lineColor$"></param>
    ''' <param name="lineWidth!"></param>
    ''' <param name="bg$"></param>
    ''' <param name="yline#">
    ''' Combine with y line for visualize the numeric solve of the equation.
    ''' </param>
    ''' <param name="ylineColor$"></param>
    ''' <returns></returns>
    <Extension>
    Public Function PlotFunction(range As NamedValue(Of DoubleRange),
                                 expression$,
                                 Optional steps# = 0.01,
                                 Optional lineColor$ = "black",
                                 Optional lineWidth! = 10,
                                 Optional bg$ = "white",
                                 Optional variables As Dictionary(Of String, String) = Nothing,
                                 Optional yline# = Double.NaN,
                                 Optional ylineColor$ = "red") As GraphicsData

        Dim engine As New Expression
        Dim ranges As Double() = range.Value.seq(steps).ToArray
        Dim y As New List(Of Double)

        If Not variables.IsNullOrEmpty Then
            For Each var In variables
                Call engine.SetVariable(var.Key, var.Value)
            Next
        End If

        For Each x As Double In ranges
            Call engine.SetVariable(range.Name, x)
            y += engine.Evaluation(expression)
        Next

        Dim serial As SerialData = FromVector(y, lineColor,,, lineWidth, ranges, expression,)

        If Double.IsNaN(yline) Then
            Return Plot({serial}, ,, bg)
        Else
            Dim syline As New SerialData With {
                .color = ylineColor.ToColor,
                .PointSize = 3,
                .width = 3,
                .title = $"y={yline}",
                .pts = {
                    New PointData With {.pt = New PointF(range.Value.Min, yline)},
                    New PointData With {.pt = New PointF(range.Value.Max, yline)}
                }
            }
            Return Plot({syline, serial}, ,, bg)
        End If
    End Function

    <Extension>
    Public Function Plot(range As DoubleRange,
                         expression As Func(Of Double, Double),
                         Optional steps# = 0.01,
                         Optional lineColor$ = "black",
                         Optional lineWidth! = 10,
                         Optional bg$ = "white",
                         Optional title$ = "Function Plot") As GraphicsData

        Dim ranges As Double() = range.seq(steps).ToArray
        Dim y As New List(Of Double)

        For Each x As Double In ranges
            y += expression(x#)
        Next

        Dim serial As SerialData = FromVector(y, lineColor,,, lineWidth, ranges, title,)
        Return Plot({serial}, ,, bg)
    End Function

    Public Function Plot(points As IEnumerable(Of Point),
                         Optional size As Size = Nothing,
                         Optional padding$ = g.DefaultPadding,
                         Optional lineColor$ = "black",
                         Optional bg$ = "white",
                         Optional title$ = "Plot Of Points",
                         Optional lineWidth! = 5.0!,
                         Optional ptSize! = 15.0!,
                         Optional lineType As DashStyle = DashStyle.Solid) As GraphicsData
        Dim s As SerialData = points _
            .FromPoints(lineColor$,
                        title$,
                        lineWidth!,
                        ptSize!,
                        lineType)
        Return {s}.Plot(size:=size, padding:=padding, bg:=bg)
    End Function

    Public Function Plot(points As IEnumerable(Of PointF),
                         Optional size As Size = Nothing,
                         Optional padding$ = g.DefaultPadding,
                         Optional lineColor$ = "black",
                         Optional bg$ = "white",
                         Optional title$ = "Plot Of Points",
                         Optional lineWidth! = 5.0!,
                         Optional ptSize! = 15.0!,
                         Optional lineType As DashStyle = DashStyle.Solid) As GraphicsData
        Dim s As SerialData = points _
            .FromPoints(lineColor$,
                        title$,
                        lineWidth!,
                        ptSize!,
                        lineType)
        Return {s}.Plot(size:=size, padding:=padding, bg:=bg)
    End Function

    <Extension>
    Public Function FromPoints(points As IEnumerable(Of Point),
                               Optional lineColor$ = "black",
                               Optional title$ = "Plot Of Points",
                               Optional lineWidth! = 5.0!,
                               Optional ptSize! = 15.0!,
                               Optional lineType As DashStyle = DashStyle.Solid) As SerialData
        Return FromPoints(
            points.Select(
            Function(pt) New PointF With {
                .X = pt.X,
                .Y = pt.Y
            }),
            lineColor,
            title,
            lineWidth,
            ptSize,
            lineType)
    End Function

    <Extension>
    Public Function FromPoints(points As IEnumerable(Of PointF),
                               Optional lineColor$ = "black",
                               Optional title$ = "Plot Of Points",
                               Optional lineWidth! = 5.0!,
                               Optional ptSize! = 15.0!,
                               Optional lineType As DashStyle = DashStyle.Solid) As SerialData

        Return New SerialData With {
            .color = lineColor.ToColor,
            .lineType = lineType,
            .PointSize = ptSize,
            .width = lineWidth,
            .pts = points.Select(
                Function(pt) New PointData With {
                    .pt = pt
            }).ToArray,
            .title = title
        }
    End Function
End Module
