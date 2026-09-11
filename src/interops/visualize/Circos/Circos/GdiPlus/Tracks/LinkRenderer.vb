Imports System.Drawing
Imports SMRUCC.genomics.Visualize.Circos.Configurations.ComponentModel
Imports SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas

Namespace GdiPlus.Tracks

    ''' <summary>
    ''' 顶层 ``&lt;links>`` 块（染色体区段之间的贝塞尔连线/缎带）的绘制
    ''' </summary>
    Public Module LinkRenderer

        Public Sub Render(ctx As GdiRenderContext)
            If ctx.Circos.Plots Is Nothing Then
                Return
            End If

            For Each track As ITrackPlot In ctx.Circos.TracksInBlock(CircosBlocks.links)
                Dim link = TryCast(track, LinkPlot)

                If link IsNot Nothing Then
                    Call RenderLink(ctx, link)
                End If
            Next
        End Sub

        Private Sub RenderLink(ctx As GdiRenderContext, link As LinkPlot)
            If link.tracksData Is Nothing Then
                Return
            End If

            Dim radius As Double = ctx.Radius(link.radius, ctx.Canvas.ImageRadius * 0.8)
            Dim bezierRadius As Double = ctx.Radius(link.bezier_radius, 0)
            Dim color As Color = ctx.ColorOf(link.color, Color.FromArgb(102, 0, 0, 0))
            Dim ribbon As Boolean = CircosUnits.IsYes(link.ribbon)
            Dim width As Double = CircosUnits.ParseNumber(link.thickness, 1)

            If width <= 0 Then width = 1

            For Each data As LinkData In link.tracksData.GetEnumerator().OfType(Of LinkData)()
                Dim a As TrackData = data.A
                Dim b As TrackData = data.B

                If a Is Nothing OrElse b Is Nothing Then
                    Continue For
                End If

                Dim bandA = ChromosomeOf(ctx, a.chr)
                Dim bandB = ChromosomeOf(ctx, b.chr)

                If bandA Is Nothing OrElse bandB Is Nothing Then
                    Continue For
                End If

                Dim a0 As Double = ctx.Layout.AngleOf(bandA, a.start)
                Dim a1 As Double = ctx.Layout.AngleOf(bandA, a.end)
                Dim b0 As Double = ctx.Layout.AngleOf(bandB, b.start)
                Dim b1 As Double = ctx.Layout.AngleOf(bandB, b.end)

                Dim pA0 As PointF = ctx.Canvas.PointAt(a0, radius)
                Dim pA1 As PointF = ctx.Canvas.PointAt(a1, radius)
                Dim pB0 As PointF = ctx.Canvas.PointAt(b0, radius)
                Dim pB1 As PointF = ctx.Canvas.PointAt(b1, radius)

                If ribbon Then
                    Dim c0 As PointF = ControlPoint(ctx, a0, b0, bezierRadius)
                    Dim c1 As PointF = ControlPoint(ctx, a1, b1, bezierRadius)
                    Dim curve0 As PointF() = BezierPoints(pA0, c0, pB0, 32)
                    Dim curve1 As PointF() = BezierPoints(pA1, c1, pB1, 32)
                    Dim poly As List(Of PointF) = curve0.Concat(curve1.Reverse()).ToList()

                    Call ctx.Canvas.FillPolygon(poly, color)
                Else
                    Dim c As PointF = ControlPoint(ctx, (a0 + b0) / 2, (a0 + b0) / 2, bezierRadius)
                    Dim curve As PointF() = BezierPoints(pA0, c, pB0, 48)

                    Call ctx.Canvas.DrawPolyline(curve, color, CSng(width))
                End If
            Next
        End Sub

        Private Function ChromosomeOf(ctx As GdiRenderContext, chr As String) As ChromosomeBand
            Dim band As ChromosomeBand = Nothing

            Call ctx.Layout.TryGetBand(chr, band)

            Return band
        End Function

        ''' <summary>
        ''' 贝塞尔曲线的控制点：位于两个端点角度的中间角、半径为 ``bezier_radius`` 的位置
        ''' </summary>
        Private Function ControlPoint(ctx As GdiRenderContext, angle0 As Double, angle1 As Double, bezierRadius As Double) As PointF
            Dim mid As Double = angle0

            ' 处理跨 0 度的情况
            Dim delta As Double = angle1 - angle0

            If delta > 180 Then
                delta -= 360
            ElseIf delta < -180 Then
                delta += 360
            End If

            mid = angle0 + delta / 2

            Return ctx.Canvas.PointAt(mid, Math.Max(0, bezierRadius))
        End Function

        ''' <summary>
        ''' 采样二次贝塞尔曲线
        ''' </summary>
        Private Function BezierPoints(p0 As PointF, c As PointF, p1 As PointF, steps As Integer) As PointF()
            Dim pts As New List(Of PointF)(steps + 1)

            For i As Integer = 0 To steps
                Dim t As Double = i / CDbl(steps)
                Dim mt As Double = 1 - t

                Dim x As Double = mt * mt * p0.X + 2 * mt * t * c.X + t * t * p1.X
                Dim y As Double = mt * mt * p0.Y + 2 * mt * t * c.Y + t * t * p1.Y

                pts.Add(New PointF(CSng(x), CSng(y)))
            Next

            Return pts.ToArray()
        End Function

    End Module
End Namespace
