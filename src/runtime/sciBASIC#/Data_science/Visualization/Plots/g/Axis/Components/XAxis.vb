﻿Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.d3js.scale
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Text
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports stdNum = System.Math

Namespace Graphic.Axis

    Public Class XAxis

        ReadOnly plotRegion As Rectangle
        ReadOnly overridesTickLine As Double
        ReadOnly pen As Pen
        ReadOnly noTicks As Boolean
        ReadOnly axisTicks As Vector
        ReadOnly scaler As Scaler
        ReadOnly tickFormat As String
        ReadOnly tickFont As Font
        ReadOnly tickColor As Brush
        ReadOnly label As String
        ReadOnly htmlLabel As Boolean
        ReadOnly xRotate As Double
        ReadOnly labelFont As String
        ReadOnly labelColor As Brush

        Sub New(scaler As DataScaler,
                pen As Pen,
                overridesTickLine As Double,
                noTicks As Boolean,
                tickFormat As String,
                tickfont As Font,
                tickColor As Brush,
                label As String,
                labelFont As String,
                labelColor As Brush,
                htmlLabel As Boolean,
                xRotate As Double)

            Me.plotRegion = scaler.region
            Me.overridesTickLine = overridesTickLine
            Me.pen = pen
            Me.noTicks = noTicks
            Me.axisTicks = scaler.AxisTicks.X
            Me.scaler = scaler.X
            Me.tickFormat = tickFormat
            Me.tickFont = tickfont
            Me.tickColor = tickColor
            Me.label = label
            Me.labelFont = labelFont
            Me.htmlLabel = htmlLabel
            Me.xRotate = xRotate
            Me.labelColor = labelColor
        End Sub

        Sub New(plotRegion As Rectangle,
                scaler As Scaler,
                ticks As Vector,
                pen As Pen,
                overridesTickLine As Double,
                noTicks As Boolean,
                tickFormat As String,
                tickfont As Font,
                tickColor As Brush,
                label As String,
                labelFont As String,
                labelColor As Brush,
                htmlLabel As Boolean,
                xRotate As Double)

            Me.plotRegion = plotRegion
            Me.scaler = scaler
            Me.overridesTickLine = overridesTickLine
            Me.pen = pen
            Me.noTicks = noTicks
            Me.axisTicks = ticks
            Me.tickFormat = tickFormat
            Me.tickFont = tickfont
            Me.tickColor = tickColor
            Me.label = label
            Me.labelFont = labelFont
            Me.htmlLabel = htmlLabel
            Me.xRotate = xRotate
            Me.labelColor = labelColor
        End Sub

        Public Sub Draw(g As IGraphics, layout As XAxisLayoutStyles, y0 As Double, offset As PointF)
            Dim Y% = plotRegion.Top + offset.Y
            Dim size As Size = plotRegion.Size

            Select Case layout
                Case XAxisLayoutStyles.Centra
                    Y += size.Height / 2 + offset.Y
                Case XAxisLayoutStyles.Top
                    Y += 0
                Case XAxisLayoutStyles.ZERO
                    Y = y0 + offset.Y
                Case Else
                    Y += size.Height
            End Select

            ' 坐标轴原点
            Dim ZERO As New Point(plotRegion.Left + offset.X, Y)
            ' X轴
            Dim right As New Point(ZERO.X + size.Width, Y)
            Dim d! = If(overridesTickLine <= 0, 10, overridesTickLine)

            ' X轴
            Call g.DrawLine(Pen, ZERO, right)

            If Not noTicks AndAlso Not axisTicks.IsNullOrEmpty Then
                Call drawTicks(g, offset, ZERO, d)
            End If

            If Not label.StripHTMLTags(stripBlank:=True).StringEmpty Then
                Call drawLabel(g, size, d)
            End If
        End Sub

        Private Sub drawTicks(g As IGraphics, offset As PointF, ZERO As Point, d As Single)
            ' 绘制坐标轴标签
            Dim ticks As (x#, label$)()

            If TypeOf scaler Is LinearScale Then
                ticks = axisTicks _
                    .Select(Function(tick)
                                If stdNum.Abs(tick) <= 0.000001 Then
                                    Return (scaler(tick), "0")
                                Else
                                    Return (scaler(tick), (tick).ToString(tickFormat))
                                End If
                            End Function) _
                    .ToArray
            Else
                ticks = DirectCast(scaler, OrdinalScale) _
                    .getTerms _
                    .Select(Function(tick) (scaler(tick.value), tick.value)) _
                    .ToArray
            End If

            For Each tick As (X#, label$) In ticks
                Dim x As Single = tick.X + offset.X
                Dim axisX As New PointF(x, ZERO.Y)
                Dim labelText = tick.label
                Dim sz As SizeF = g.MeasureString(labelText, tickFont)

                Call g.DrawLine(pen, axisX, New PointF(x, ZERO.Y + d!))

                If xRotate <> 0 AndAlso TypeOf g Is Graphics2D Then
                    Dim text As New GraphicsText(g)

                    If xRotate > 0 Then
                        Call text.DrawString(labelText, tickFont, tickColor, New Point(x, ZERO.Y + d * 1.2), angle:=xRotate)
                    Else
                        Call text.DrawString(labelText, tickFont, tickColor, New Point(x, ZERO.Y + sz.Height * stdNum.Sin(xRotate * 180 / stdNum.PI)), angle:=xRotate)
                    End If
                Else
                    Call g.DrawString(labelText, tickFont, tickColor, New Point(x - sz.Width / 2, ZERO.Y + d * 1.2))
                End If
            Next
        End Sub

        Private Sub drawLabel(g As IGraphics, size As Size, d As Double)
            If htmlLabel Then
                Dim labelImage As Image = label.__plotLabel(labelFont, False)
                Dim point As New Point With {
                    .X = (size.Width - labelImage.Width) / 2 + plotRegion.Left,
                    .Y = plotRegion.Top + size.Height + tickFont.Height + d * 4
                }

                Call g.DrawImageUnscaled(labelImage, point)
            Else
                Dim font As Font = CSSFont.TryParse(labelFont).GDIObject(g.Dpi)
                Dim fSize As SizeF = g.MeasureString(label, font)
                Dim y1 As Double = plotRegion.Bottom + tickFont.Height + d * 5
                Dim y2 As Double = plotRegion.Bottom + ((g.Size.Height - plotRegion.Bottom) - fSize.Height) / 2
                Dim point As New PointF With {
                    .X = (size.Width - fSize.Width) / 2 + plotRegion.Left,
                    .Y = stdNum.Max(y1, y2)
                }

                ' Call $"[X:={label}] {point.ToString}".__INFO_ECHO
                Call g.DrawString(label, font, labelColor, point)
            End If
        End Sub

    End Class
End Namespace