Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging

Public Module ReadsPlot

    Public Function Plot(reads As Generic.IEnumerable(Of ReadsCount)) As Bitmap
        Dim Gr = New Size(reads.Count, 1000).CreateGDIDevice
        Dim readsArray = (From x In reads Select x Order By x.Index Ascending).ToArray

        Dim px As Integer = 100
        Dim py As Integer = 500

        For Each x As ReadsCount In readsArray
            Call Gr.Graphics.DrawLine(Pens.Brown, New Point(px, py), New Point(px, py - x.ReadsPlus))
            Call Gr.Graphics.DrawLine(Pens.Green, New Point(px, py), New Point(px, py - x.SharedPlus))

            Call Gr.Graphics.DrawLine(Pens.SeaGreen, New Point(px, py), New Point(px, py + x.ReadsMinus))
            Call Gr.Graphics.DrawLine(Pens.Blue, New Point(px, py), New Point(px, py + x.SharedMinus))

            px += 1
        Next

        Call Gr.ImageResource.Save("x:\sdfsdf.bmp")

        Return Gr.ImageResource
    End Function
End Module
