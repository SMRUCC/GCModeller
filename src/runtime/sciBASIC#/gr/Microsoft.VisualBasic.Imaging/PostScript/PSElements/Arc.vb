Imports Microsoft.VisualBasic.MIME.Html.CSS

Namespace PostScript.Elements

    Public Class Arc : Inherits PSElement

        Public Property stroke As Stroke
        Public Property x As Single
        Public Property y As Single
        Public Property width As Single
        Public Property height As Single
        Public Property startAngle As Single
        Public Property sweepAngle As Single

        Friend Overrides Sub WriteAscii(ps As Writer)
            Throw New NotImplementedException()
        End Sub

        Friend Overrides Sub Paint(g As IGraphics)
            Throw New NotImplementedException()
        End Sub
    End Class
End Namespace