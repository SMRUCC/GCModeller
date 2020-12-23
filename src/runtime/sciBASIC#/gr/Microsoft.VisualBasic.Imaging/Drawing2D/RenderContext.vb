Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic.Imaging.SVG.CSS
Imports number = System.Double

Namespace Drawing2D

    Public Class RenderShape

    End Class

    Public Class RenderContext

        Public Property fillStyle As String
        Public Property lineWidth As number
        Public Property strokeStyle As String
        Public Property globalAlpha As number

        Dim currentShape As Path2D
        Dim drawingShapes As New List(Of GraphicsPath)

        Public Sub beginPath()
            currentShape = New Path2D
        End Sub

        Public Function createLinearGradient(a As number, b As number, c As number, d As number) As Gradient

        End Function

        Public Sub moveTo(a As number, b As number)

        End Sub

        Public Sub quadraticCurveTo(a As number, b As number, c As number, d As number)

        End Sub

        Public Sub lineTo(a As number, b As number)

        End Sub

        Public Sub stroke()

        End Sub

        Public Sub closePath()
            Call currentShape.CloseAllFigures()
            Call drawingShapes.Add(currentShape.Path)
        End Sub

        Public Sub bezierCurveTo(a As number, b As number, c As number, d As number, end1 As number, end2 As number)

        End Sub
    End Class
End Namespace