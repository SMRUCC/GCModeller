Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging.SVG.CSS
Imports Microsoft.VisualBasic.Linq
Imports number = System.Double

Namespace Drawing2D

    Public Class RenderContext

        Public Property fillStyle As String
        Public Property lineWidth As number
        Public Property strokeStyle As String
        Public Property globalAlpha As number

        Dim currentShape As Path2D
        Dim drawingShapes As New List(Of RenderShape)

        Public Sub Render(g As IGraphics)
            For Each shape As RenderShape In drawingShapes
                Call g.FillPath(shape.fill, shape)
                Call g.DrawPath(shape.pen, shape)
            Next
        End Sub

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

        Private Function getCurrentPen() As Pen

        End Function

        Private Function getCurrentFill() As Brush

        End Function

        Public Sub closePath()
            Call currentShape.CloseAllFigures()
            Call New RenderShape With {
                .shape = currentShape.Path,
                .pen = getCurrentPen(),
                .fill = getCurrentFill()
            }.DoCall(AddressOf drawingShapes.Add)
        End Sub

        Public Sub bezierCurveTo(a As number, b As number, c As number, d As number, end1 As number, end2 As number)

        End Sub
    End Class
End Namespace