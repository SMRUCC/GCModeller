Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging

Namespace GdiPlus

    ''' <summary>
    ''' 封装 <see cref="IGraphics"/>，提供圆环绘图所需的高层几何辅助方法。
    ''' 
    ''' 角度约定：0 度为正上方（12 点钟方向），顺时针方向为正。
    ''' （GDI+ 自身以 3 点钟方向为 0 度、顺时针为正，内部会自动做 -90 度的换算）
    ''' </summary>
    Public Class GdiCanvas

        Public ReadOnly Property Graphics As IGraphics
        Public ReadOnly Property CenterX As Double
        Public ReadOnly Property CenterY As Double
        ''' <summary>
        ''' 可用的图像半径（像素），已经扣除了画布留白
        ''' </summary>
        Public ReadOnly Property ImageRadius As Double

        Public Sub New(g As IGraphics, Optional padding As Integer = 0)
            Me.Graphics = g

            CenterX = g.Width / 2.0
            CenterY = g.Height / 2.0

            Dim r As Double = Math.Min(g.Width, g.Height) / 2.0 - padding

            If r < 1 Then
                r = Math.Min(g.Width, g.Height) / 2.0
            End If

            ImageRadius = r
        End Sub

#Region "坐标换算"

        ''' <summary>
        ''' 圆环之上的点（角度：度，0 = 正上方，顺时针；半径：像素）
        ''' </summary>
        Public Function PointAt(angleDeg As Double, radius As Double) As PointF
            Dim rad As Double = angleDeg * Math.PI / 180.0

            Return New PointF(
                CSng(CenterX + radius * Math.Sin(rad)),
                CSng(CenterY - radius * Math.Cos(rad)))
        End Function

        ''' <summary>
        ''' 将本模块使用的角度换算为 GDI+ 的圆弧起始角度
        ''' </summary>
        Public Function GdiAngle(angleDeg As Double) As Single
            Return CSng(angleDeg - 90.0)
        End Function

        ''' <summary>
        ''' 线段围成的外接矩形（用于 DrawArc 的椭圆范围）
        ''' </summary>
        Public Function CircleRect(radius As Double) As RectangleF
            Return New RectangleF(
                CSng(CenterX - radius),
                CSng(CenterY - radius),
                CSng(radius * 2),
                CSng(radius * 2))
        End Function

#End Region

#Region "基本图元"

        ''' <summary>
        ''' 填充圆环扇区（annular sector）。使用多边形逼近以避免依赖 GraphicsPath 的圆弧支持
        ''' </summary>
        Public Sub FillAnnularSector(innerR As Double, outerR As Double, startAngle As Double, sweepAngle As Double, color As Color)
            If sweepAngle <= 0.0001 OrElse outerR <= 0.0001 Then
                Return
            End If

            If innerR < 0 Then
                innerR = 0
            End If
            If innerR > outerR Then
                Dim swap As Double = innerR

                innerR = outerR
                outerR = swap
            End If

            ' 每 0.5 度一个采样点，同时限制最大点数
            Dim steps As Integer = Math.Max(2, Math.Min(720, CInt(Math.Ceiling(sweepAngle / 0.5))))
            Dim pts As New List(Of PointF)(steps * 2 + 2)

            For i As Integer = 0 To steps
                Dim a As Double = startAngle + sweepAngle * i / steps
                pts.Add(PointAt(a, outerR))
            Next
            For i As Integer = steps To 0 Step -1
                Dim a As Double = startAngle + sweepAngle * i / steps
                pts.Add(PointAt(a, innerR))
            Next

            Using brush As New SolidBrush(color)
                Graphics.FillPolygon(brush, pts.ToArray())
            End Using
        End Sub

        ''' <summary>
        ''' 绘制圆环扇区的轮廓
        ''' </summary>
        Public Sub DrawArc(radius As Double, startAngle As Double, sweepAngle As Double, color As Color, Optional width As Single = 1)
            If sweepAngle <= 0.0001 OrElse radius <= 0.0001 Then
                Return
            End If

            Using pen As New Pen(color, width)
                Call Graphics.DrawArc(pen, CircleRect(radius), GdiAngle(startAngle), CSng(sweepAngle))
            End Using
        End Sub

        ''' <summary>
        ''' 绘制一条放射线（沿着半径方向）
        ''' </summary>
        Public Sub DrawRadialLine(angle As Double, innerR As Double, outerR As Double, color As Color, Optional width As Single = 1)
            If width <= 0 Then width = 1

            Using pen As New Pen(color, width)
                Call Graphics.DrawLine(pen, PointAt(angle, innerR), PointAt(angle, outerR))
            End Using
        End Sub

        ''' <summary>
        ''' 绘制折线
        ''' </summary>
        Public Sub DrawPolyline(points As IEnumerable(Of PointF), color As Color, Optional width As Single = 1)
            Dim pts As PointF() = If(points Is Nothing, Nothing, points.ToArray())

            If pts Is Nothing OrElse pts.Length < 2 Then
                Return
            End If

            Using pen As New Pen(color, width)
                Call Graphics.DrawLines(pen, pts)
            End Using
        End Sub

        ''' <summary>
        ''' 填充多边形
        ''' </summary>
        Public Sub FillPolygon(points As IEnumerable(Of PointF), color As Color)
            Dim pts As PointF() = If(points Is Nothing, Nothing, points.ToArray())

            If pts Is Nothing OrElse pts.Length < 3 Then
                Return
            End If

            Using brush As New SolidBrush(color)
                Call Graphics.FillPolygon(brush, pts)
            End Using
        End Sub

        ''' <summary>
        ''' 填充圆形（用于 scatter 的 glyph）
        ''' </summary>
        Public Sub FillCircle(center As PointF, radius As Double, color As Color)
            If radius <= 0 Then
                Return
            End If

            Using brush As New SolidBrush(color)
                Call Graphics.FillEllipse(
                    brush,
                    CSng(center.X - radius),
                    CSng(center.Y - radius),
                    CSng(radius * 2),
                    CSng(radius * 2))
            End Using
        End Sub

#End Region

#Region "文本"

        ''' <summary>
        ''' 在指定的画布坐标位置绘制文本（水平方向）
        ''' </summary>
        Public Sub DrawText(text As String, position As PointF, font As Font, color As Color)
            If String.IsNullOrEmpty(text) Then
                Return
            End If

            Using brush As New SolidBrush(color)
                Call Graphics.DrawString(text, font, brush, position.X, position.Y)
            End Using
        End Sub

        ''' <summary>
        ''' 绘制随圆环旋转的文本标签。文本的中心锚定在 <paramref name="angle"/>/<paramref name="radius"/> 位置上，
        ''' 位于下半圈的文本会自动翻转以保持可读性。
        ''' </summary>
        Public Sub DrawRadialText(text As String, angle As Double, radius As Double, font As Font, color As Color)
            If String.IsNullOrEmpty(text) Then
                Return
            End If

            Dim anchor As PointF = PointAt(angle, radius)
            Dim sz As SizeF = Graphics.MeasureString(text, font)

            Dim rot As Double = angle

            If angle > 90 AndAlso angle < 270 Then
                rot += 180
            End If

            Try
                Call Graphics.TranslateTransform(anchor.X, anchor.Y)
                Call Graphics.RotateTransform(CSng(rot))

                Using brush As New SolidBrush(color)
                    Call Graphics.DrawString(text, font, brush, CSng(-sz.Width / 2), CSng(-sz.Height / 2))
                End Using
            Finally
                Call Graphics.ResetTransform()
            End Try
        End Sub

        ''' <summary>
        ''' 计算文本在画布之上的尺寸
        ''' </summary>
        Public Function MeasureText(text As String, font As Font) As SizeF
            If String.IsNullOrEmpty(text) Then
                Return New SizeF(0, 0)
            End If

            Return Graphics.MeasureString(text, font)
        End Function

#End Region

    End Class
End Namespace
