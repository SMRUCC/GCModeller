Imports System.Drawing
Imports LANS.SystemsBiology.ComponentModel.Loci
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Imaging

Public Class Histogram : Inherits CurvesModel

    Public Property TokenWidth As Integer = 6
    Public Property Lines As Integer = 20

    Sub New()
        MyBase.PlotBrush = New SolidBrush(Color.DarkCyan)
    End Sub

    Private Shared Function __average(x As Double()) As Double
        Return x.Average
    End Function

    Private Function __trimData(data As DataSample(Of Double), size As Size) As DataSample(Of Double)
        Dim n As Integer = size.Width / TokenWidth
        n = data.Length / n
        data = DataSampleAPI.DoubleSample(data.Split(n).Select(AddressOf __average))
        Return data
    End Function

    Protected Overrides Sub Draw(ByRef g As IGraphics, data As DataSample(Of Double), location As Point, size As Size)
        data = __trimData(data, size)

        'Y箭头向上
        'Call Gr.Gr_Device.DrawLine(LinePen, Vertex, New Point(Vertex.X - 5, Vertex.Y + 20))
        'Vertex = New Point(Location.X + size.Width, Location.Y)
        'Call Gr.Gr_Device.DrawLine(LinePen, Location, Vertex) 'X
        'Call Gr.Gr_Device.DrawString(Loci.Left & " bp", TagFont, Brushes.Black, New Point(Location.X, Location.Y + 30)) '基因组上面的位置信息
        'Call Gr.Gr_Device.DrawString(Loci.Right & " bp", TagFont, Brushes.Black, New Point(Vertex.X, Vertex.Y + 30))
        'X箭头向右
        'Call Gr.Gr_Device.DrawLine(LinePen, Vertex, New Point(Vertex.X - 20, Vertex.Y - 5))

        Dim LinePen As New Pen(color:=Color.FromArgb(90, Color.LightGray), width:=0.3)
        Dim tagFont As New Font(FontFace.Ubuntu, 12)

        Call DrawAixs(g, location, size, tagFont, data.Min, data.Max)

        Dim X_ScaleFactor As Double = size.Width / data.Length
        Dim Y_ScaleFactor As Double = size.Height / (data.Max - data.Min)
        Dim X As Double = location.X, Y As Integer
        Dim Y_avg As Double = location.Y - (data.Average - data.Min) * Y_ScaleFactor
        Dim dddd As Double = size.Height / Lines

        Y = location.Y - size.Height

        For i As Integer = 0 To Lines - 1
            Call g.DrawLine(LinePen, New Point(location.X, Y), New Point(location.X + size.Width, Y))
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
                prePt = pt
            Else
                Dim pt As New Point(X, Y)
                Region = New Rectangle(pt, New Size(X_ScaleFactor, Y_avg - Y))
                pt = New Point(X + 0.5 * X_ScaleFactor, Region.Top)
                prePt = pt
            End If

            Call g.FillRectangle(PlotBrush, Region)

            X += X_ScaleFactor
        Next
    End Sub
End Class

