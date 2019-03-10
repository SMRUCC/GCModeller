﻿#Region "Microsoft.VisualBasic::dac2a7257a48da24014e2da396ff89be, Data_science\Mathematica\Plot\Plots\g\Legends\LegendStyles.vb"

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

    '     Enum LegendStyles
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Graphic.Legend

    ''' <summary>
    ''' Vector shapes that drawing of this legend.
    ''' </summary>
    Public Enum LegendStyles As Integer

        ''' <summary>
        ''' 矩形
        ''' </summary>
        Rectangle = 2
        ''' <summary>
        ''' 圆形
        ''' </summary>
        Circle = 4
        ''' <summary>
        ''' 实线
        ''' </summary>
        SolidLine = 8
        ''' <summary>
        ''' 虚线
        ''' </summary>
        DashLine = 16
        ''' <summary>
        ''' 菱形
        ''' </summary>
        Diamond = 32
        ''' <summary>
        ''' 三角形
        ''' </summary>
        Triangle = 64
        ''' <summary>
        ''' 六边形
        ''' </summary>
        Hexagon = 128
        ''' <summary>
        ''' 五角星
        ''' </summary>
        Pentacle = 256
        ''' <summary>
        ''' 正方形
        ''' </summary>
        Square = 512
        ''' <summary>
        ''' 圆角矩形
        ''' </summary>
        RoundRectangle = 1024
    End Enum
End Namespace
