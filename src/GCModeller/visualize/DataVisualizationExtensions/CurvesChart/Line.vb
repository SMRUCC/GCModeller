#Region "Microsoft.VisualBasic::35748b80e2f421ccaddbfc2022f097fe, GCModeller\visualize\DataVisualizationExtensions\CurvesChart\Line.vb"

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

    '   Total Lines: 65
    '    Code Lines: 48
    ' Comment Lines: 2
    '   Blank Lines: 15
    '     File Size: 2.64 KB


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
            Dim pos As New Point With {
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
