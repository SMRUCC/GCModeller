﻿#Region "Microsoft.VisualBasic::2391e61a3db0bb1443641c7f768daa3d, Data_science\Visualization\Plots\Scatter\Bubble.vb"

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

    '   Total Lines: 358
    '    Code Lines: 294 (82.12%)
    ' Comment Lines: 21 (5.87%)
    '    - Xml Docs: 80.95%
    ' 
    '   Blank Lines: 43 (12.01%)
    '     File Size: 14.88 KB


    ' Class Bubble
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: logRadius, Plot
    ' 
    '     Sub: drawLegend, Plot, PlotInternal
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.d3js.Layout
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
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
Imports LineCap = System.Drawing.Drawing2D.LineCap
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
Imports LineCap = Microsoft.VisualBasic.Imaging.LineCap
#End If

''' <summary>
''' the bubble plots
''' </summary>
Public Class Bubble : Inherits Plot

    Private Shared Function logRadius(R#) As Double
        Return std.Log(R + 1) + 1
    End Function

    ReadOnly usingLogRadius As New [Default](Of Func(Of Double, Double))(AddressOf logRadius)

    Dim data As SerialData()
    Dim xAxis As String
    Dim yAxis As String
    Dim usingLogScaleRadius As Boolean
    Dim positiveRangeY As Boolean
    Dim bubbleBorder As Stroke
    Dim strokeColorAsMainColor As Boolean

    Sub New(data As IEnumerable(Of SerialData), usingLogScaleRadius As Boolean, positiveRangeY As Boolean, theme As Theme)
        Call MyBase.New(theme)

        Me.data = data.ToArray
        Me.usingLogScaleRadius = usingLogScaleRadius
        Me.positiveRangeY = positiveRangeY
        Me.bubbleBorder = Stroke.TryParse(theme.shapeStroke)
    End Sub

    ''' <summary>
    ''' <see cref="PointData.value"/>是Bubble的半径大小
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="size"></param>
    ''' <param name="bg"></param>
    ''' <param name="legend"></param>
    ''' <param name="xAxis"><see cref="AxisProvider"/></param>
    ''' <returns></returns>
    Public Overloads Shared Function Plot(data As IEnumerable(Of SerialData),
                                          Optional size As String = Nothing,
                                          Optional padding$ = g.DefaultPadding,
                                          Optional bg As String = "white",
                                          Optional legend As Boolean = True,
                                          Optional usingLogScaleRadius As Boolean = False,
                                          Optional legendBorder As Stroke = Nothing,
                                          Optional bubbleBorder As String = Nothing,
                                          Optional xAxis$ = Nothing,
                                          Optional yAxis$ = Nothing,
                                          Optional xlabel$ = "",
                                          Optional ylabel$ = "",
                                          Optional title$ = Nothing,
                                          Optional axisLabelFontCSS$ = CSSFont.Win7LargeBold,
                                          Optional tagFontCSS$ = CSSFont.Win10Normal,
                                          Optional titleFontCSS$ = CSSFont.PlotTitle,
                                          Optional strokeColorAsMainColor As Boolean = False,
                                          Optional positiveRangeY As Boolean = False,
                                          Optional legendTitleFontCSS$ = CSSFont.PlotSubTitle,
                                          Optional legendAnchor As PointF = Nothing,
                                          Optional ylayout As YAxisLayoutStyles = YAxisLayoutStyles.Left,
                                          Optional gridFill$ = "rgb(250,250,250)",
                                          Optional driver As Drivers = Drivers.Default) As GraphicsData

        Dim theme As New Theme With {
            .background = bg,
            .mainCSS = titleFontCSS,
            .padding = padding,
            .legendTitleCSS = legendTitleFontCSS,
            .tagCSS = tagFontCSS,
            .xAxisLayout = XAxisLayoutStyles.Bottom,
            .yAxisLayout = ylayout,
            .drawLegend = legend,
            .legendLayout = New Absolute(legendAnchor),
            .legendBoxStroke = legendBorder?.ToString,
            .axisLabelCSS = axisLabelFontCSS,
            .gridFill = gridFill,
            .shapeStroke = bubbleBorder
        }

        Return New Bubble(data, usingLogScaleRadius, positiveRangeY, theme) With {
            .xlabel = xlabel,
            .ylabel = ylabel,
            .main = title
        }.Plot(size, driver:=driver)
    End Function

    ''' <summary>
    ''' Just plot the scatter bubble
    ''' </summary>
    ''' <param name="g"></param>
    ''' <param name="s"></param>
    Public Overloads Shared Sub Plot(g As IGraphics, s As SerialData, scaler As DataScaler,
                                     Optional tagLabelFont As Font = Nothing,
                                     Optional labels As List(Of Label) = Nothing,
                                     Optional ByRef anchors As List(Of Anchor) = Nothing,
                                     Optional bubblePen As Pen = Nothing,
                                     Optional scale As Func(Of Double, Double) = Nothing)

        Dim b As SolidBrush = Nothing
        Dim labelSize As SizeF

        If scale Is Nothing Then
            scale = Function(r) r
        End If

        Dim getRadius = Function(pt As PointData)
                            Dim r# = scale(pt.value)

                            If r = 0R Then
                                Return s.pointSize
                            Else
                                Return r
                            End If
                        End Function
        Dim device As IGraphics = g
        Dim css As CSSEnvirnment = g.LoadEnvironment

        If Not (s.color.IsEmpty) Then
            b = New SolidBrush(s.color)
        End If

        For Each pt As PointData In s.pts
            Dim r As Double = getRadius(pt)
            Dim p As PointF = scaler.Translate(pt.pt.X, pt.pt.Y)
            Dim rect As New RectangleF(New PointF(p.X - r, p.Y - r), New Size(r * 2, r * 2))

            If r.IsNaNImaginary Then
                Call $"invalid radius value of {pt}".Warning
                Continue For
            End If

            With pt.color
                If .StringEmpty Then
                    Call g.FillPie(b, rect, 0, 360)
                Else
                    Call g.FillPie(New SolidBrush(.TranslateColor), rect, 0, 360)
                End If
            End With

            If pt.stroke.StringEmpty Then
                If Not bubblePen Is Nothing Then
                    Call g.DrawCircle(pt.pt, r, bubblePen, fill:=False)
                End If
            Else
                Call css _
                    .GetPen(Stroke.TryParse(pt.stroke)) _
                    .DoCall(Sub(pen)
                                Call device.DrawCircle(pt.pt, r, pen, fill:=False)
                            End Sub)

            End If

            If (labels IsNot Nothing AndAlso anchors IsNot Nothing) AndAlso Not pt.tag.StringEmpty Then
                labelSize = g.MeasureString(pt.tag, tagLabelFont)
                labels += New Label With {
                    .text = pt.tag,
                    .X = rect.Right,
                    .Y = rect.Top,
                    .width = labelSize.Width,
                    .height = labelSize.Height
                }
                anchors += New Anchor With {
                    .r = r,
                    .x = rect.Right - r,
                    .y = rect.Top + r
                }
            End If
        Next
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim mapper As Mapper
        Dim css As CSSEnvirnment = g.LoadEnvironment
        Dim rangeData As New Scaling(data, False)
        Dim tagLabelFont As Font = css.GetFont(CSSFont.TryParse(theme.tagCSS))
        Dim titleFont As Font = css.GetFont(CSSFont.TryParse(theme.mainCSS))

        If xAxis.StringEmpty Then
            ' 任意一个位空值就会使用普通的axis数据计算方法
            ' 这个并不是以y值来表示数量上的关系的，point是随机位置，所以在这里使用相对scalling
            mapper = New Mapper(rangeData)
        Else
            mapper = rangeData.yrange _
                .GetAxisValues _
                .DoCall(Function(axisdata) New AxisProvider(axisdata)) _
                .DoCall(Function(ya)
                            Return New Mapper(x:=xAxis, y:=ya, range:=rangeData)
                        End Function)
        End If

        Dim scale As Func(Of Double, Double) = New Func(Of Double, Double)(Function(r) r) Or usingLogRadius.When(usingLogScaleRadius)
        Dim x, y As d3js.scale.LinearScale
        Dim xTicks = mapper.xAxis.CreateAxisTicks(ticks:=5, decimalDigits:=If(mapper.xAxis.Max > 0.01, 2, -1))
        Dim yTicks = mapper.yAxis.CreateAxisTicks(, decimalDigits:=If(mapper.xAxis.Max > 0.01, 2, -1))
        Dim labels As New List(Of Label)
        Dim anchors As New List(Of Anchor)
        Dim plotRect As Rectangle = canvas.PlotRegion(css)

        If positiveRangeY Then
            yTicks = yTicks.Where(Function(t) t >= 0).ToArray
        End If

        With plotRect
            x = d3js.scale.linear.domain(values:=xTicks).range(integers:={ .Left, .Right})
            y = d3js.scale.linear.domain(values:=yTicks).range(integers:={ .Top, .Bottom})
        End With

        Dim device = g
        Dim scaler As New DataScaler With {
            .AxisTicks = (xTicks, yTicks),
            .region = plotRect,
            .X = x,
            .Y = y
        }

        Call g.DrawAxis(
            region:=canvas,
            scaler:=scaler,
            showGrid:=True,
            xlabel:=xlabel,
            ylabel:=ylabel,
            labelFont:=theme.axisLabelCSS,
            htmlLabel:=False,
            ylayout:=theme.yAxisLayout,
            gridFill:=theme.gridFill,
            XtickFormat:=If(mapper.xAxis.Max > 0.01, "F2", "G2")
        )

        Dim bubblePen As Pen = css.GetPen(bubbleBorder, allowNull:=True)

        For Each s As SerialData In data
            Call Bubble.Plot(g, s, scaler, tagLabelFont, labels, anchors, bubblePen, scale)
        Next

        Call d3js.labeler(30, 1) _
            .Width(plotrect.Width) _
            .Height(plotrect.Height) _
            .Anchors(anchors) _
            .Labels(labels) _
            .Start(showProgress:=False, nsweeps:=3000)

        Dim anchor As Anchor
        Dim label As Label
        Dim labelPointer As New Pen(Color.Gray, 2) With {
            .EndCap = LineCap.ArrowAnchor
        }

        For Each index As SeqValue(Of Label) In labels.SeqIterator
            label = index
            anchor = anchors(index)
            ' labelPos = New PointF(label.X, label.Y)

            If label.X + label.width > plotRect.Right Then
                ' labelPos = New PointF(canvas.PlotRegion.Right - label.width, labelPos.Y)
                label.X = plotRect.Right - label.width
            End If

            Call g.DrawLine(labelPointer, anchor, label.GetTextAnchor(anchor))
            Call g.DrawString(label.text, tagLabelFont, Brushes.Black, label.location)
        Next

        If theme.drawLegend Then
            Call drawLegend(g, canvas)
        End If
    End Sub

    Private Sub drawLegend(g As IGraphics, canvas As GraphicsRegion)
        Dim env As CSSEnvirnment = g.LoadEnvironment
        Dim legendLabelFont As Font = env.GetFont(CSSFont.TryParse(theme.axisLabelCSS))
        Dim maxSize! = data _
            .Select(Function(s) s.title) _
            .Select(Function(str) g.MeasureString(str, legendLabelFont).Width) _
            .Max
        Dim topLeft As Point
        Dim grect = canvas.PlotRegion(env)

        If theme.legendLayout.IsNullOrEmpty Then
            topLeft = New Point With {
                .X = grect.Right - maxSize * 1.5,
                .Y = grect.Top + grect.Height * 0.05
            }
        Else
            Dim px As Double
            Dim py As Double
            Dim legendAnchor As PointF = theme.legendLayout.GetLocation(canvas, New LayoutDependency(canvas))

            If legendAnchor.X < 1 Then
                px = grect.Right - grect.Width * legendAnchor.X
            Else
                px = legendAnchor.X
            End If
            If legendAnchor.Y < 1 Then
                py = grect.Top + grect.Height * legendAnchor.Y
            Else
                py = legendAnchor.Y
            End If

            topLeft = New Point With {.X = px, .Y = py}
        End If

        Dim legends = LinqAPI.Exec(Of LegendObject) <=
                                                      _
            From serial As SerialData
            In data
            Let color As String = If(
                strokeColorAsMainColor,
                Stroke.TryParse(serial.pts(serial.pts.Length \ 2).stroke).fill,
                serial.color.RGBExpression)
            Select New LegendObject With {
                .color = color,
                .fontstyle = theme.axisLabelCSS,
                .style = LegendStyles.Circle,
                .title = serial.title
            }

        Call g.DrawLegends(topLeft, legends,,, shapeBorder:=Stroke.TryParse(theme.legendBoxStroke))
    End Sub
End Class
