Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.genomics.ComponentModel.Loci

Public Class Line : Inherits CurvesModel

    Protected Overrides Sub Draw(ByRef g As IGraphics, data As DataSample(Of Double), location As Point, size As Size)
        Dim LinePen As New Pen(color:=Color.FromArgb(30, Color.LightGray), width:=0.3)
        Dim tagFont As New Font(FontFace.Ubuntu, 12)

        Call DrawAixs(g, location, size, tagFont, data.Min, data.Max)

        Dim X_ScaleFactor As Double = size.Width / data.Length
        Dim Y_ScaleFactor As Double = size.Height / (data.Max - data.Min)
        Dim X As Double = location.X, Y As Integer
        Dim Y_avg As Double = location.Y - (data.Average - data.Min) * Y_ScaleFactor
        Dim dddd As Double = size.Height / 10

        Y = location.Y - size.Height

        For i As Integer = 0 To 9
            'Call Gr.Gr_Device.DrawLine(LinePen, New Point(Location.X, Y), New Point(Location.X + size.Width, Y))
            Y += dddd
        Next

        Dim prePt As Point = New Point(X, location.Y - (data.First - data.Min) * Y_ScaleFactor)

        LinePen = New Pen(PlotBrush, 2)

        If ShowAverageLine Then  ' 绘制中间的平均线
            Call g.DrawLine(New Pen(Brushes.LightGray, 3), New Point(location.X, Y_avg), New Point(location.X + size.Width, Y_avg))
            Call g.DrawString(Mid(data.Average, 1, 5), tagFont, Brushes.Black, New Point(location.X - YValueOffset, Y_avg - "0".MeasureString(tagFont).Height / 2))
        End If

        Dim Region As Rectangle

        For Each n As Double In data.data
            Y = location.Y - (n - data.Min) * Y_ScaleFactor

            If Y > Y_avg Then '小于平均值，则Y颠倒过来
                Dim pt As New Point(X, Y_avg)
                Region = New Rectangle(pt, New Size(X_ScaleFactor, Y - Y_avg))
                pt = New Point(X + 0.5 * X_ScaleFactor, Region.Bottom)
                Call g.DrawLine(LinePen, prePt, pt)
                prePt = pt
            Else
                Dim pt As New Point(X, Y)
                Region = New Rectangle(pt, New Size(X_ScaleFactor, Y_avg - Y))
                pt = New Point(X + 0.5 * X_ScaleFactor, Region.Top)
                Call g.DrawLine(LinePen, prePt, pt)
                prePt = pt
            End If

            'Call Gr.Gr_Device.FillRectangle(PlotBrush, Region)

            X += X_ScaleFactor
        Next
    End Sub
End Class
