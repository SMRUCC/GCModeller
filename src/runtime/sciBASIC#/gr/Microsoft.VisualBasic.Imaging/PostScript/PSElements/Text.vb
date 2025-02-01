Imports Microsoft.VisualBasic.MIME.Html.CSS

Namespace PostScript.Elements

    Public Class Text : Inherits PSElement

        Public Property text As String
        Public Property font As CSSFont
        ''' <summary>
        ''' usually be the fill color
        ''' </summary>
        ''' <returns></returns>
        Public Property fill As String
        Public Property rotation As Single

        Friend Overrides Sub WriteAscii(ps As Writer)
            Throw New NotImplementedException()
        End Sub

        Friend Overrides Sub Paint(g As IGraphics)
            Throw New NotImplementedException()
        End Sub
    End Class

End Namespace
