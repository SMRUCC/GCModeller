Public Class FormChartPlotDevice : Inherits Microsoft.VisualBasic.DocumentFormat.Csv.CsvChartDevice

    Public Function CopyChartImage() As Image
        Dim Bitmap As Bitmap = New Bitmap(Me._chart.Size.Width, Me._chart.Size.Height)
        Call Me._chart.DrawToBitmap(Bitmap, New Rectangle(New Point, Me._chart.Size))

        Return Bitmap
    End Function
End Class