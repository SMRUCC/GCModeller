Imports System.Drawing
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.MIME.Html.Render

Namespace PostScript.Elements

    Public Class Poly : Inherits PSElement

        Public Property points As PointF()
        Public Property stroke As Stroke
        Public Property fill As String
        Public Property closedPath As Boolean = True

        Friend Overrides Sub WriteAscii(ps As Writer)
            Dim pen As Pen = ps.pen(stroke)

            If points Is Nothing OrElse points.Length < 3 Then
                Throw New ArgumentException("At least 3 data points is required for draw a closed shape!")
            End If

            Call ps.color(pen.Color)
            Call ps.linewidth(pen.Width)
            Call ps.moveto(_points(0))

            For i As Integer = 1 To points.Length - 1
                Call ps.lineto(_points(i).X, _points(i).Y)
            Next

            If closedPath Then
                Call ps.closepath()
            End If

            Call ps.stroke()
        End Sub

        Friend Overrides Sub Paint(g As IGraphics)
            If Not fill.StringEmpty(, True) Then
                Call g.FillClosedCurve(fill.GetBrush, points)
            End If

            Call g.DrawClosedCurve(g.LoadEnvironment.GetPen(stroke), points)
        End Sub
    End Class
End Namespace