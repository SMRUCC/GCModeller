﻿#Region "Microsoft.VisualBasic::a18264445eeabe26155afee831e88e68, Data_science\Mathematica\Plot\Plots\3D\Scatter.vb"

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

    '     Module Scatter
    ' 
    '         Function: (+2 Overloads) Plot
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Data.ChartPlots.Plot3D.Device
Imports Microsoft.VisualBasic.Data.ChartPlots.Plot3D.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing3D
Imports Microsoft.VisualBasic.Imaging.Drawing3D.Math3D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports Microsoft.VisualBasic.Scripting.Runtime

Namespace Plot3D

    ''' <summary>
    ''' 3D scatter charting
    ''' </summary>
    Public Module Scatter

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="serials"></param>
        ''' <param name="camera"></param>
        ''' <param name="bg$"></param>
        ''' <param name="padding$"></param>
        ''' <param name="axisLabelFontCSS$"></param>
        ''' <param name="boxStroke$"></param>
        ''' <param name="axisStroke$"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 首先要生成3维图表的模型元素，然后将这些元素混合在一起，最后按照Z深度的排序结果顺序绘制出来，才能够生成一幅有层次感的3维图表
        ''' </remarks>
        <Extension>
        Public Function Plot(serials As IEnumerable(Of Serial3D),
                             camera As Camera,
                             Optional bg$ = "white",
                             Optional padding$ = g.DefaultPadding,
                             Optional axisLabelFontCSS$ = CSSFont.Win7Normal,
                             Optional boxStroke$ = Stroke.StrongHighlightStroke,
                             Optional axisStroke$ = Stroke.AxisStroke,
                             Optional labX$ = "X",
                             Optional labY$ = "Y",
                             Optional labZ$ = "Z",
                             Optional legendWidth! = 20,
                             Optional arrowFactor$ = "2,2") As GraphicsData

            Dim list As Serial3D() = serials.ToArray
            Dim points = list _
                .Select(Function(s) s.Points) _
                .IteratesALL _
                .ToArray
            Dim font As Font = CSSFont.TryParse(axisLabelFontCSS).GDIObject

            ' 首先需要获取得到XYZ值的范围
            Dim X, Y, Z As Vector

            With points.VectorShadows
                X = DirectCast(.X, IEnumerable(Of Single)).AsDouble.Range.CreateAxisTicks
                Y = DirectCast(.Y, IEnumerable(Of Single)).AsDouble.Range.CreateAxisTicks
                Z = DirectCast(.Z, IEnumerable(Of Single)).AsDouble.Range.CreateAxisTicks
            End With

            ' 然后生成底部的网格
            Dim model As New List(Of Element3D)

            model += GridBottom.Grid(X, Y, (X(1) - X(0), Y(1) - Y(0)), Z.Min)
            model += AxisDraw.Axis(
                X, Y, Z, font,
                (labX, labY, labZ),
                axisStroke,
                arrowFactor)

            ' 最后混合进入系列点
            For Each serial As Serial3D In list

                Dim data As Point3D() = serial.Points
                Dim size As New Size With {
                    .Width = serial.PointSize,
                    .Height = serial.PointSize
                }
                Dim color As New SolidBrush(serial.Color)

                model += data _
                    .Select(Function(pt)
                                Return New ShapePoint With {
                                    .Fill = color,
                                    .Location = pt,
                                    .Size = size,
                                    .Style = serial.Shape
                                }
                            End Function)
            Next

            Dim legends As Legend() = list _
                .Select(Function(s)
                            Return New Legend With {
                                .color = s.Color.RGBExpression,
                                .fontstyle = axisLabelFontCSS,
                                .style = s.Shape,
                                .title = s.Title
                            }
                        End Function) _
                .ToArray

            Dim plotInternal =
                Sub(ByRef g As IGraphics, region As GraphicsRegion)

                    ' 绘制图例？？
                    Dim legendHeight! = (legends.Length * (font.Height + 5))
                    Dim legendTop! = (region.PlotRegion.Height - legendHeight) / 2
                    Dim maxL! = legends.Select(Function(s) s.title) _
                        .MaxLengthString _
                        .MeasureSize(g, font) _
                        .Width
                    Dim legendLeft! = region.PlotRegion.Right - maxL - legendWidth
                    Dim topLeft As New Point With {
                        .X = legendLeft,
                        .Y = legendTop
                    }
                    Dim legendSize$ = $"{legendWidth},{legendWidth}"

                    ' 要先绘制三维图形，要不然会将图例遮住的
                    Call model.RenderAs3DChart(g, camera, region)
                    Call g.DrawLegends(
                        topLeft:=topLeft,
                        legends:=legends,
                        gSize:=legendSize,
                        d:=5,
                        regionBorder:=Stroke.AxisStroke
                    )

                End Sub
            Dim plotRegion As New GraphicsRegion With {
                .Size = camera.screen,
                .Padding = padding
            }

            Return plotRegion _
                .Size _
                .GraphicsPlots(padding, bg, plotInternal)
        End Function

        ''' <summary>
        ''' 绘制三维散点图
        ''' </summary>
        ''' <param name="func"></param>
        ''' <param name="x"></param>
        ''' <param name="y"></param>
        ''' <param name="camera"></param>
        ''' <param name="xsteps!"></param>
        ''' <param name="ysteps!"></param>
        ''' <param name="lineColor$"></param>
        ''' <param name="font"></param>
        ''' <param name="bg$"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Plot(func As Func(Of Double, Double, Double),
                             x As DoubleRange,
                             y As DoubleRange,
                             camera As Camera,
                             Optional xsteps! = 0.1,
                             Optional ysteps! = 0.1,
                             Optional lineColor$ = "red",
                             Optional font As Font = Nothing,
                             Optional bg$ = "white",
                             Optional padding$ = "padding: 5px 5px 5px 5px;") As GraphicsData

            Dim data As Point3D() = func _
                .Evaluate(x, y, xsteps, ysteps) _
                .IteratesALL _
                .Select(Function(o) New Point3D(o.X, o.y, o.z))
            Dim rect As Rectangle
            Dim previous As Point
            Dim cur As Point
            Dim lcolor As New Pen(lineColor.ToColor)

            Dim plotInternal =
                Sub(ByRef g As IGraphics, region As GraphicsRegion)
                    '  Call AxisDraw.DrawAxis(g, data, camera, font)

                    With camera

                        data(Scan0) = .Project(.Rotate(data(Scan0)))
                        previous = data(Scan0).PointXY(camera.screen)

                        For Each pt As Point3D In data.Skip(1)
                            pt = .Project(.Rotate(pt))   ' 3d project to 2d
                            cur = pt.PointXY(camera.screen)
                            rect = New Rectangle(cur, New Size(5, 5))

                            Call g.FillPie(Brushes.Red, rect, 0, 360)  ' 画点
                            Call g.DrawLine(lcolor, previous.X, previous.Y, cur.X, cur.Y)       ' 画线

                            previous = cur
                        Next
                    End With
                End Sub

            Return camera.screen.GraphicsPlots(padding, bg, plotInternal)
        End Function
    End Module
End Namespace
