Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.MIME.Html.Render

Namespace PostScript.Elements

    Public Class Line : Inherits PSElement(Of Shapes.Line)

        Sub New()

        End Sub

        Sub New(stroke As Pen, a As PointF, b As PointF)
            shape = New Shapes.Line(stroke, a, b)
        End Sub

        Friend Overrides Sub WriteAscii(ps As Writer)
            Dim a = shape.A
            Dim b = shape.B
            Dim pen As Pen = ps.pen(shape.Stroke)

            Call ps.line(a.X, a.Y, b.X, b.Y)
            Call ps.linewidth(pen.Width)
            Call ps.color(shape.Stroke.fill.TranslateColor)
        End Sub

        Friend Overrides Sub Paint(g As IGraphics)
            Dim a = shape.A
            Dim b = shape.B
            Dim pen As Pen = g.LoadEnvironment.GetPen(shape.Stroke)

            Call g.DrawLine(pen, a, b)
        End Sub
    End Class
End Namespace