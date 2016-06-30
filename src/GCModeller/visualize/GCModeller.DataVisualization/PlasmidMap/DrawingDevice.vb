Imports Microsoft.VisualBasic.Imaging

Namespace PlasmidMap

    Public Class DrawingDevice

        Sub New()

        End Sub

        Public Function InvokeDrawing(ObjectModel As PlasmidMapDrawingModel) As Bitmap
            Dim GRHandle = (New Size(1000, 1000)).CreateGDIDevice(filled:=Color.White)
            Dim Center As Point = New Point(GRHandle.ImageResource.Size.Width / 2, GRHandle.ImageResource.Size.Height / 2)

            Dim r1 As Double = Math.Min(GRHandle.Width, GRHandle.Height) * 0.95
            Dim r2 As Double = Math.Min(GRHandle.Width, GRHandle.Height) * 0.85

#Const DEBUG = 1
#If DEBUG Then
            Call GRHandle.Graphics.DrawPie(Pens.Black, New Rectangle(New Point(Center.X - 5, Center.Y - 5), New Size(10, 10)), 0, 360)
#End If
            For i As Integer = 0 To ObjectModel.GeneObjects.Count - 1
                Dim Gene = ObjectModel.GeneObjects(i)
                Dim size = Gene.Draw(GRHandle.Graphics, Center, r1, r2)
            Next

            Return GRHandle.ImageResource
        End Function
    End Class
End Namespace