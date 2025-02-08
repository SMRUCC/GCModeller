Imports System.Drawing
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.MIME.Html.Render

Namespace PostScript.Elements

    Public Class Poly : Inherits PSElement

        Public Property polygon As PointF()
        Public Property stroke As Stroke
        Public Property fill As String

        Friend Overrides Sub WriteAscii(ps As Writer)
            Throw New NotImplementedException()
        End Sub

        Friend Overrides Sub Paint(g As IGraphics)
            If Not fill.StringEmpty(, True) Then
                Call g.FillClosedCurve(fill.GetBrush, polygon)
            End If

            Call g.DrawClosedCurve(g.LoadEnvironment.GetPen(stroke), polygon)
        End Sub
    End Class
End Namespace