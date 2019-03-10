﻿#Region "Microsoft.VisualBasic::5610a008c9d66e8beaf13a61c847e6aa, gr\Microsoft.VisualBasic.Imaging\Drawing2D\Math2D\HullPolygonDraw.vb"

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

    '     Module HullPolygonDraw
    ' 
    '         Function: buildPath
    ' 
    '         Sub: DrawHullPolygon
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging.Math2D

Namespace Drawing2D.Math2D

    Public Module HullPolygonDraw

        <Extension>
        Public Sub DrawHullPolygon(g As IGraphics,
                                   polygon As IEnumerable(Of PointF),
                                   color As Color,
                                   Optional strokeWidth! = 8.5,
                                   Optional alpha% = 95,
                                   Optional shadow As Boolean = True)

            Dim shape As PointF() = polygon.ToArray
            Dim alphaBrush As New SolidBrush(color.Alpha(alpha))
            Dim path = shape.buildPath(Nothing)
            Dim shadowPath = shape.buildPath(New PointF(strokeWidth / 2, strokeWidth))
            Dim stroke As New Pen(color, strokeWidth) With {
                .DashStyle = DashStyle.Dash
            }
            Dim shadowStroke As New Pen(Color.LightGray, strokeWidth) With {
                .DashStyle = stroke.DashStyle
            }

            Call g.FillPath(alphaBrush, path)
            Call g.DrawPath(shadowStroke, shadowPath)
            Call g.DrawPath(stroke, path)
        End Sub

        <Extension>
        Private Function buildPath(polygon As IEnumerable(Of PointF), offset As PointF) As GraphicsPath
            Dim path As New GraphicsPath

            Call path.AddPolygon(polygon.Select(Function(p) p.OffSet2D(offset)).ToArray)
            Call path.CloseFigure()

            Return path
        End Function
    End Module
End Namespace
