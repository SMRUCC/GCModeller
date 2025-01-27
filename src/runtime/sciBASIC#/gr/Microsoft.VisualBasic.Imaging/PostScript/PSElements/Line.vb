Imports System.Drawing
Imports Microsoft.VisualBasic.MIME.Html.CSS

Namespace PostScript.Elements

    Public Class Line : Inherits PSElement

        Public Property a As PointF
        Public Property b As PointF
        Public Property stroke As Stroke

    End Class
End Namespace