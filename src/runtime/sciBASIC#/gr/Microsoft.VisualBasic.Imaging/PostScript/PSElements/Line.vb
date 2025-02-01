Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging.Drawing2D

Namespace PostScript.Elements

    Public Class Line : Inherits PSElement(Of Shapes.Line)

        Sub New()

        End Sub

        Sub New(stroke As Pen, a As PointF, b As PointF)
            shape = New Shapes.Line(stroke, a, b)
        End Sub

        Friend Overrides Sub WriteAscii(ps As Writer)
            Throw New NotImplementedException()
        End Sub

        Friend Overrides Sub Paint(g As IGraphics)
            Throw New NotImplementedException()
        End Sub
    End Class
End Namespace