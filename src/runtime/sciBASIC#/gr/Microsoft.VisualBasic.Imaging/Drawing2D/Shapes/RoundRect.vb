﻿#Region "Microsoft.VisualBasic::4b51e549edb08b91e65f7cb62bbb97f8, gr\Microsoft.VisualBasic.Imaging\Drawing2D\Shapes\RoundRect.vb"

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

    '     Class RoundRect
    ' 
    '         Function: GetRoundedRectPath
    ' 
    '         Sub: Draw
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS

Namespace Drawing2D.Shapes

    ''' <summary>
    ''' 绘制圆角矩形
    ''' </summary>
    Public Class RoundRect

        Public Shared Sub Draw(ByRef g As IGraphics,
                               topLeft As Point,
                               size As SizeF,
                               radius%,
                               Optional br As Brush = Nothing,
                               Optional border As Stroke = Nothing)

            Dim rect As New Rectangle(topLeft, size.ToSize)
            Dim path As GraphicsPath = GetRoundedRectPath(rect, radius)

            If Not br Is Nothing Then
                Call g.FillPath(br, path)
            End If
            If Not border Is Nothing Then
                Call g.DrawPath(border, path)
            End If
        End Sub

        Public Shared Function GetRoundedRectPath(rect As Rectangle, radius%) As GraphicsPath
            Dim roundRect As Rectangle
            Dim path As New GraphicsPath

            With rect

                .Offset(-1, -1)
                roundRect = New Rectangle With {
                    .Location = rect.Location,
                    .Size = New Size(radius - 1, radius - 1)
                }

                path.AddArc(roundRect, 180, 90)     ' 左上角
                roundRect.X = .Right - radius       ' 右上角
                path.AddArc(roundRect, 270, 90)
                roundRect.Y = .Bottom - radius      ' 右下角
                path.AddArc(roundRect, 0, 90)
                roundRect.X = .Left                 ' 左下角
                path.AddArc(roundRect, 90, 90)
                path.CloseFigure()

                Return path
            End With
        End Function
    End Class
End Namespace
