﻿#Region "Microsoft.VisualBasic::ce0a7c75e93afd4df63daf7351127a7f, gr\Microsoft.VisualBasic.Imaging\Drawing2D\Colors\ColorMapLegend.vb"

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

    '     Class ColorMapLegend
    ' 
    '         Properties: designer, format, legendOffsetLeft, noblank, ruleOffset
    '                     tickAxisStroke, tickFont, ticks, title, titleFont
    '                     unmapColor
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Sub: Draw
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing

Namespace Drawing2D.Colors

    Public Class ColorMapLegend

        Public Property designer As SolidBrush()
        Public Property title As String
        Public Property titleFont As Font
        Public Property ticks As Double()
        Public Property tickFont As Font
        Public Property tickAxisStroke As Pen
        Public Property unmapColor As String = Nothing
        Public Property ruleOffset As Single = 10
        Public Property format As String = "F2"
        Public Property legendOffsetLeft As Single = -99999
        Public Property noblank As Boolean = True

        Sub New(palette As String, Optional mapLevels As Integer = 30)
            designer = GetColors(palette, mapLevels) _
                .Select(Function(c) New SolidBrush(c)) _
                .ToArray
        End Sub

        Public Sub Draw(ByRef g As IGraphics, layout As Rectangle)
            Call g.ColorMapLegend(
                layout:=layout,
                designer:=designer,
                ticks:=ticks,
                titleFont:=titleFont,
                title:=title,
                tickFont:=tickFont,
                tickAxisStroke:=tickAxisStroke,
                unmapColor:=unmapColor,
                ruleOffset:=ruleOffset,
                format:=format,
                legendOffsetLeft:=legendOffsetLeft,
                noLeftBlank:=noblank
            )
        End Sub
    End Class
End Namespace
