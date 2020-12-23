Imports System.Drawing
Imports System.Drawing.Drawing2D

Namespace Drawing2D

    Public Class RenderShape

        Public Property pen As Pen
        Public Property fill As Brush
        Public Property shape As GraphicsPath

        Public Shared Narrowing Operator CType(s As RenderShape) As GraphicsPath
            Return s.shape
        End Operator
    End Class
End Namespace