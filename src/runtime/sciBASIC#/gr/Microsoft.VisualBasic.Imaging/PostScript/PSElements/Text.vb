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

    End Class

End Namespace
