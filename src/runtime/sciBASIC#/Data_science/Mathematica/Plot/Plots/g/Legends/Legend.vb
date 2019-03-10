﻿#Region "Microsoft.VisualBasic::591fb035bc3894a0001c247c9a1de53a, Data_science\Mathematica\Plot\Plots\g\Legends\Legend.vb"

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

    '     Class Legend
    ' 
    '         Properties: color, fontstyle, style, title
    ' 
    '         Function: GetFont, MeasureTitle, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS

Namespace Graphic.Legend

    ''' <summary>
    ''' 图例
    ''' </summary>
    Public Class Legend

        ''' <summary>
        ''' The shape of this legend
        ''' </summary>
        ''' <returns></returns>
        Public Property style As LegendStyles
        ''' <summary>
        ''' The legend label
        ''' </summary>
        ''' <returns></returns>
        Public Property title As String
        ''' <summary>
        ''' The color of the legend <see cref="style"/> shape.
        ''' </summary>
        ''' <returns></returns>
        Public Property color As String
        ''' <summary>
        ''' CSS expression, which can be parsing by <see cref="CSSFont"/>, drawing font of <see cref="title"/> 
        ''' </summary>
        ''' <returns></returns>
        Public Property fontstyle As String

        ''' <summary>
        ''' <see cref="fontstyle"/> to <see cref="Font"/>
        ''' </summary>
        ''' <returns></returns>
        Public Function GetFont() As Font
            Return CSSFont.TryParse(fontstyle).GDIObject
        End Function

        ''' <summary>
        ''' 计算标题文本的绘制大小
        ''' </summary>
        ''' <param name="g"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function MeasureTitle(g As IGraphics) As SizeF
            Return g.MeasureString(title, GetFont)
        End Function

        Public Overrides Function ToString() As String
            Return title
        End Function
    End Class
End Namespace
