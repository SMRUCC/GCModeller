Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Shapes

Namespace PostScript.Elements

    Public Class Rectangle : Inherits PSElement(Of Box)

        Sub New()
        End Sub

        Sub New(rect As System.Drawing.Rectangle, color As Color)
            shape = New Box(rect, color)
        End Sub

        Sub New(rect As RectangleF, color As Color)
            shape = New Box(rect, color)
        End Sub

        Friend Overrides Sub WriteAscii(ps As Writer)

        End Sub

        Friend Overrides Sub Paint(g As IGraphics)
            Throw New NotImplementedException()
        End Sub
    End Class
End Namespace