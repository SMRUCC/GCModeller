#Region "Microsoft.VisualBasic::c05a6ec0aff7966ca9c65db960f54cf4, visualize\DataVisualizationExtensions\CurvesChart\Line.vb"

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

    '   Total Lines: 91
    '    Code Lines: 73 (80.22%)
    ' Comment Lines: 2 (2.20%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 16 (17.58%)
    '     File Size: 3.77 KB


    ' Class Line
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

Public Class Line : Inherits CurvesModel

    Protected Overrides Sub Draw(ByRef g As IGraphics, data As DataSample(Of Double), location As Point, size As Size)
        Dim LinePen As New Pen(color:=Color.FromArgb(30, Color.LightGray), width:=0.3)
        Dim tagFont As New Font(FontFace.Ubuntu, 12)

        Call DrawAixs(g, location, size, tagFont, data.Min, data.Max)

        Dim X_ScaleFactor As Double = size.Width / data.SampleSize
        Dim Y_ScaleFactor As Double = size.Height / (data.Max - data.Min)
        Dim X As Double = location.X, Y As Integer
        Dim Y_avg As Double = location.Y - (data.Average - data.Min) * Y_ScaleFactor
        Dim dddd As Double = size.Height / 10

        Y = location.Y - size.Height

        For i As Integer = 0 To 9
            'Call Gr.Gr_Device.DrawLine(LinePen, New Point(Location.X, Y), New Point(Location.X + size.Width, Y))
            Y += dddd
        Next

        Dim prePt As New Point(X, location.Y - (data.First - data.Min) * Y_ScaleFactor)

        LinePen = New Pen(PlotBrush, 2)

        If ShowAverageLine Then  ' 绘制中间的平均线
            Dim pos As New PointF With {
                .X = location.X - YValueOffset,
                .Y = Y_avg - "0".MeasureSize(g, tagFont).Height / 2
            }

            Call g.DrawLine(New Pen(Brushes.LightGray, 3), New Point(location.X, Y_avg), New Point(location.X + size.Width, Y_avg))
            Call g.DrawString(Mid(data.Average, 1, 5), tagFont, Brushes.Black, pos)
        End If

        Dim Region As Rectangle

        For Each n As Double In data
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
