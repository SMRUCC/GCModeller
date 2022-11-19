#Region "Microsoft.VisualBasic::88e8fa63267880bdea0e14e731fba053, GCModeller\visualize\DataVisualizationExtensions\Abstract\MapModelCommon.vb"

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

    '   Total Lines: 224
    '    Code Lines: 105
    ' Comment Lines: 76
    '   Blank Lines: 43
    '     File Size: 7.63 KB


    ' Class MapModelCommon
    ' 
    '     Properties: Color, Direction, HeadLength, Height, Left
    '                 Length, Right
    ' 
    '     Function: CreateBackwardModel, CreateForwardModel, CreateNoneDirectionModel
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D

''' <summary>
''' 绘图模型的通用基本类型结构
''' </summary>
Public MustInherit Class MapModelCommon

    ''' <summary>
    ''' 0表示没有方向，1表示正向，-1表示反向；默认为正向
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Direction As Integer = 1
    Public Property Color As Brush

    ''' <summary>
    ''' 片段最左端的位置,这个位置和方向无关
    ''' </summary>
    ''' <returns></returns>
    Public Property Left As Integer
    ''' <summary>
    ''' 片段最右端的位置，这个位置和方向无关
    ''' </summary>
    ''' <returns></returns>
    Public Property Right As Integer

    ''' <summary>
    ''' 基因片段对象在绘制的时候的高度
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Height As Integer = 85

    Public Const OFFSET As Integer = 20

    ''' <summary>
    ''' 箭头长度的最低限制
    ''' </summary>
    ''' <remarks></remarks>
    Public Const HeadLengthLowerBound As Integer = 100
    Public Const HeaderMaxLength% = 200

    ''' <summary>
    ''' 所绘制的基因对象的箭头的长度，其单位与<see cref="Length"></see>属性一致，都是像素，即这个属性值是已经换算过的
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable ReadOnly Property HeadLength As Integer
        Get
            Dim Length As Integer = Math.Abs(Left - Right)
            Dim n = Length * 0.45

            ' 如果长度过小，则直接将基因对象画为一个三角形
            If n < HeadLengthLowerBound Then
                Return -1
            ElseIf n > HeaderMaxLength Then
                n = HeaderMaxLength
            End If

            Return n * ConvertFactor
        End Get
    End Property

    Protected ConvertFactor As Double

    ''' <summary>
    ''' 当前的基因对想在图形上面的绘制长度
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overridable ReadOnly Property Length As Integer
        Get
            Return Math.Abs(Left - Right) * ConvertFactor
        End Get
    End Property

    ''' <summary>
    ''' 假若所绘制出来的模型的右部分的坐标超过了<paramref name="RightLimit"></paramref>这个参数，则会被缩短
    ''' </summary>
    ''' <param name="refLoci"></param>
    ''' <param name="RightLimit"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overridable Function CreateForwardModel(refLoci As Point, RightLimit As Integer) As GraphicsPath

        ' ->

        Dim shape As New GraphicsPath

        If HeadLength <= 0 Then
            ' 太短了，直接绘制为一个三角形
            '
            ' b \
            ' |  a
            ' c /
            Dim a As New Point(refLoci.X + Length, refLoci.Y + Height / 2)
            Dim b As New Point(refLoci.X, refLoci.Y)
            Dim c As New Point(refLoci.X, refLoci.Y + Height)

            Call shape.AddLine(b, a)
            Call shape.AddLine(a, c)
            Call shape.AddLine(c, b)

        Else
            Dim leftTop As New Point(refLoci.X, refLoci.Y)
            Dim leftBottom As New Point(refLoci.X, refLoci.Y + Height)

            Dim rightBottom As New Point(refLoci.X + Length - HeadLength, leftBottom.Y)

            If rightBottom.X > RightLimit Then
                rightBottom = New Point(RightLimit, rightBottom.Y)
            End If

            Dim rightBottomBottom = New Point(rightBottom.X, rightBottom.Y + OFFSET)

            Dim arrowHead = New Point(rightBottom.X + HeadLength, rightBottom.Y - 0.5 * Height)

            Dim rightTopTop = New Point(rightBottom.X, refLoci.Y - OFFSET)
            Dim rightTop As New Point(rightBottom.X, refLoci.Y)

            Call shape.AddLine(leftTop, leftBottom)
            Call shape.AddLine(leftBottom, rightBottom)
            Call shape.AddLine(rightBottom, rightBottomBottom)

            Call shape.AddLine(rightBottomBottom, arrowHead)
            Call shape.AddLine(arrowHead, rightTopTop)
            Call shape.AddLine(rightTopTop, rightTop)
            Call shape.AddLine(rightTop, leftTop)
        End If

        Return shape
    End Function

    ''' <summary>
    ''' 假若所绘制出来的模型的右部分的坐标超过了<paramref name="RightLimit"></paramref>这个参数，则会被缩短
    ''' </summary>
    ''' <param name="refLoci"></param>
    ''' <param name="RightLimit"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overridable Function CreateBackwardModel(refLoci As Point, RightLimit As Integer) As GraphicsPath

        ' <-

        Dim shape As New GraphicsPath

        If HeadLength <= 0 Then
            ' 太短了，直接绘制为一个三角形
            '
            '  /b
            ' a |
            '  \c
            Dim a As New Point(refLoci.X, refLoci.Y + Height / 2)
            Dim b As New Point(refLoci.X + Length, refLoci.Y)
            Dim c As New Point(refLoci.X + Length, refLoci.Y + Height)

            Call shape.AddLine(a, b)
            Call shape.AddLine(b, c)
            Call shape.AddLine(c, a)

        Else
            Dim leftTop As New Point(refLoci.X + HeadLength, refLoci.Y)
            Dim leftTopTop As New Point(leftTop.X, leftTop.Y - OFFSET)

            Dim arrowHead As New Point(refLoci.X, leftTop.Y + 0.5 * Height)

            Dim leftBottomBottom As New Point(leftTop.X, leftTop.Y + Height + OFFSET)
            Dim leftBottom As New Point(leftTop.X, refLoci.Y + Height)

            Dim rightTop As New Point(refLoci.X + Length, refLoci.Y)
            If rightTop.X > RightLimit Then
                rightTop = New Point(RightLimit, rightTop.Y)
            End If
            Dim rightBottom As New Point(rightTop.X, leftBottom.Y)

            Call shape.AddLine(leftTop, leftTopTop)
            Call shape.AddLine(leftTopTop, arrowHead)

            Call shape.AddLine(arrowHead, leftBottomBottom)
            Call shape.AddLine(leftBottomBottom, leftBottom)

            Call shape.AddLine(leftBottom, rightBottom)
            Call shape.AddLine(rightTop, rightBottom)
            Call shape.AddLine(rightTop, leftTop)
        End If

        Return shape
    End Function

    ''' <summary>
    ''' 假若所绘制出来的模型的右部分的坐标超过了<paramref name="RightLimit"></paramref>这个参数，则会被缩短
    ''' </summary>
    ''' <param name="refLoci"></param>
    ''' <param name="RightLimit"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 就是一个矩形区域
    ''' </remarks>
    Protected Overridable Function CreateNoneDirectionModel(refLoci As Point, RightLimit As Integer) As GraphicsPath

        ' ==

        Dim shape As New GraphicsPath
        Dim leftTop As New Point(refLoci.X, refLoci.Y)
        Dim leftBottom As New Point(refLoci.X, refLoci.Y + Height)
        Dim rightTop As New Point(refLoci.X + Length, refLoci.Y)
        If rightTop.X > RightLimit Then
            rightTop = New Point(RightLimit, rightTop.Y)
        End If
        Dim rightBottom As New Point(rightTop.X, leftBottom.Y)

        Call shape.AddLine(leftTop, rightTop)
        Call shape.AddLine(leftTop, leftBottom)
        Call shape.AddLine(rightTop, rightBottom)
        Call shape.AddLine(leftBottom, rightBottom)

        Return shape
    End Function
End Class
