#Region "Microsoft.VisualBasic::d83a45f7783f59b0efc2ab87e54ca612, visualize\DataVisualizationExtensions\CurvesChart\Histogram.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 118
    '    Code Lines: 88 (74.58%)
    ' Comment Lines: 9 (7.63%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 21 (17.80%)
    '     File Size: 4.99 KB


    ' Class Histogram
    ' 
    '     Properties: Lines, TokenWidth
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: __average, __trimData
    ' 
    '     Sub: Draw
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Math.Statistics

#If NET48 Then
Imports Pen = System.Drawing.Pen
Imports Pens = System.Drawing.Pens
Imports Brush = System.Drawing.Brush
Imports Font = System.Drawing.Font
Imports Brushes = System.Drawing.Brushes
Imports SolidBrush = System.Drawing.SolidBrush
Imports DashStyle = System.Drawing.Drawing2D.DashStyle
Imports Image = System.Drawing.Image
Imports Bitmap = System.Drawing.Bitmap
Imports GraphicsPath = System.Drawing.Drawing2D.GraphicsPath
Imports FontStyle = System.Drawing.FontStyle
#Else
Imports Pen = Microsoft.VisualBasic.Imaging.Pen
Imports Pens = Microsoft.VisualBasic.Imaging.Pens
Imports Brush = Microsoft.VisualBasic.Imaging.Brush
Imports Font = Microsoft.VisualBasic.Imaging.Font
Imports Brushes = Microsoft.VisualBasic.Imaging.Brushes
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush
Imports DashStyle = Microsoft.VisualBasic.Imaging.DashStyle
Imports Image = Microsoft.VisualBasic.Imaging.Image
Imports Bitmap = Microsoft.VisualBasic.Imaging.Bitmap
Imports GraphicsPath = Microsoft.VisualBasic.Imaging.GraphicsPath
Imports FontStyle = Microsoft.VisualBasic.Imaging.FontStyle
#End If

Public Class Histogram : Inherits CurvesModel

    Public Property TokenWidth As Integer = 6
    Public Property Lines As Integer = 10

    Sub New()
        MyBase.PlotBrush = New SolidBrush(Color.DarkCyan)
    End Sub

    Private Shared Function __average(x As Double()) As Double
        Return x.Average
    End Function

    Private Function __trimData(data As DataSample(Of Double), size As Size) As DataSample(Of Double)
        Dim n As Integer = size.Width / TokenWidth
        n = data.SampleSize / n
        data = data.Split(n).Select(AddressOf __average).DoubleSample
        Return data
    End Function

    Protected Overrides Sub Draw(ByRef g As IGraphics, data As DataSample(Of Double), location As Point, size As Size)
        ' data = __trimData(data, size)

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

        Dim X_ScaleFactor As Double = size.Width / data.SampleSize
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
            Dim pos As New PointF With {
                .X = location.X - YValueOffset,
                .Y = Y_avg - "0".MeasureSize(g, tagFont).Height / 2
            }
            Call g.DrawLine(New Pen(Brushes.LightGray, 3), New Point(location.X, Y_avg), New Point(location.X + size.Width, Y_avg))
            Call g.DrawString(data.Average.ToString("F2"), tagFont, Brushes.Black, pos)
        End If

        Dim rect As Rectangle

        For Each n As Double In data
            Y = location.Y - (n - data.Min) * Y_ScaleFactor

            If Y > Y_avg Then '小于平均值，则Y颠倒过来
                Dim pt As New Point(X, Y_avg)
                rect = New Rectangle(pt, New Size(X_ScaleFactor, Y - Y_avg))
                pt = New Point(X + 0.5 * X_ScaleFactor, rect.Bottom)
                prePt = pt
            Else
                Dim pt As New Point(X, Y)
                rect = New Rectangle(pt, New Size(X_ScaleFactor, Y_avg - Y))
                pt = New Point(X + 0.5 * X_ScaleFactor, rect.Top)
                prePt = pt
            End If

            Call g.FillRectangle(PlotBrush, rect)
            Call g.DrawRectangle(Pens.White, rect)

            X += X_ScaleFactor
        Next
    End Sub
End Class
