﻿#Region "Microsoft.VisualBasic::320a1f4cad52c5c786ff6202641b40b1, gr\Microsoft.VisualBasic.Imaging\Drawing2D\Shapes\Shape.vb"

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

    '   Total Lines: 94
    '    Code Lines: 42 (44.68%)
    ' Comment Lines: 38 (40.43%)
    '    - Xml Docs: 97.37%
    ' 
    '   Blank Lines: 14 (14.89%)
    '     File Size: 3.10 KB


    '     Class Shape
    ' 
    '         Properties: DrawingRegion, EnableAutoLayout, Location, TooltipTag
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: Draw, MoveOffset, (+2 Overloads) MoveTo, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging

Namespace Drawing2D.Shapes

    ''' <summary>
    ''' An abstract shape element with layout information data.
    ''' </summary>
    ''' <remarks>
    ''' (矢量图形)
    ''' </remarks>
    Public MustInherit Class Shape

        ''' <summary>
        ''' the location of the current shape element
        ''' </summary>
        ''' <returns></returns>
        Public Property Location As PointF
        ''' <summary>
        ''' the metadata string of current shape element
        ''' </summary>
        ''' <returns></returns>
        Public Property TooltipTag As String

        Public MustOverride ReadOnly Property Size As SizeF

        ''' <summary>
        ''' create the layout rectangle value based on the shape <see cref="Location"/> and its shape <see cref="Size"/>.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property DrawingRegion As RectangleF
            Get
                Return New RectangleF(Location, Size)
            End Get
        End Property

        ''' <summary>
        ''' 默认是允许自动组织布局的
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property EnableAutoLayout As Boolean = True

        ''' <summary>
        ''' create new shape element with a given <see cref="Location"/> value
        ''' </summary>
        ''' <param name="initLoci"></param>
        Sub New(initLoci As Point)
            Location = initLoci.PointF
        End Sub

        ''' <summary>
        ''' create new shape element with a given <see cref="Location"/> value
        ''' </summary>
        ''' <param name="loc"></param>
        Sub New(loc As PointF)
            Location = loc
        End Sub

        Public Function MoveTo(pt As Point) As Shape
            Location = pt.PointF
            Return Me
        End Function

        Public Function MoveTo(pt As PointF) As Shape
            Location = pt
            Return Me
        End Function

        Public Function MoveOffset(offset As PointF) As Shape
            Location = New PointF(Location.X + offset.X, Location.Y + offset.Y)
            Return Me
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="OverridesLoci">假若需要进行绘制到的时候复写当前的元素的位置，则请使用这个参数</param>
        ''' <returns>函数返回当前元素在绘制之后所占据的区域</returns>
        ''' <remarks></remarks>
        Public Overridable Function Draw(ByRef g As IGraphics, Optional overridesLoci As Point = Nothing) As RectangleF
            If Not overridesLoci.IsEmpty Then
                Me.Location = overridesLoci
            End If

            Return DrawingRegion
        End Function

        Public Overrides Function ToString() As String
            Return DrawingRegion.ToString
        End Function
    End Class
End Namespace
