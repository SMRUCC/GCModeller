#Region "Microsoft.VisualBasic::7586927c2fcff5ba5ebcb77ca7a20480, ..\GCModeller\visualize\visualizeTools\MapModelCommon.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Drawing

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
    ''' 箭头长度的最大限制
    ''' </summary>
    ''' <remarks></remarks>
    Public Const HeadLengthLimits As Integer = 160
    ''' <summary>
    ''' 箭头长度的最低限制
    ''' </summary>
    ''' <remarks></remarks>
    Public Const HeadLengthLowerBound As Integer = 25

    ''' <summary>
    ''' 所绘制的基因对象的箭头的长度，其单位与<see cref="Length"></see>属性一致，都是像素，即这个属性值是已经换算过的
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable ReadOnly Property HeadLength As Integer
        Get
            Dim Length As Integer = Math.Abs(Left - Right)
            Dim n = Length * 0.1
            If n > HeadLengthLimits Then
                n = HeadLengthLimits
            ElseIf n < HeadLengthLowerBound Then
                n = HeadLengthLowerBound
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
    Protected Overridable Function CreateForwardModel(refLoci As Point, RightLimit As Integer) As Drawing2D.GraphicsPath
        Dim Graphic = New System.Drawing.Drawing2D.GraphicsPath
        Dim pt_lefttop As System.Drawing.Point = New System.Drawing.Point(refLoci.X, refLoci.Y)
        Dim pt_leftbottom As System.Drawing.Point = New System.Drawing.Point(refLoci.X, refLoci.Y + Height)

        Dim pt_rightbottom As System.Drawing.Point = New System.Drawing.Point(refLoci.X + Length - HeadLength, pt_leftbottom.Y)
        If pt_rightbottom.X > RightLimit Then
            pt_rightbottom = New Point(RightLimit, pt_rightbottom.Y)
        End If
        Dim pt_rightbottombottom = New System.Drawing.Point(pt_rightbottom.X, pt_rightbottom.Y + OFFSET)

        Dim pt_arrowHead = New System.Drawing.Point(pt_rightbottom.X + HeadLength, pt_rightbottom.Y - 0.5 * Height)

        Dim pt_righttoptop = New System.Drawing.Point(pt_rightbottom.X, refLoci.Y - OFFSET)
        Dim pt_righttop As System.Drawing.Point = New System.Drawing.Point(pt_rightbottom.X, refLoci.Y)

        Call Graphic.AddLine(pt_lefttop, pt_leftbottom)
        Call Graphic.AddLine(pt_leftbottom, pt_rightbottom)
        Call Graphic.AddLine(pt_rightbottom, pt_rightbottombottom)

        Call Graphic.AddLine(pt_rightbottombottom, pt_arrowHead)
        Call Graphic.AddLine(pt_arrowHead, pt_righttoptop)
        Call Graphic.AddLine(pt_righttoptop, pt_righttop)
        Call Graphic.AddLine(pt_righttop, pt_lefttop)

        Return Graphic
    End Function

    ''' <summary>
    ''' 假若所绘制出来的模型的右部分的坐标超过了<paramref name="RightLimit"></paramref>这个参数，则会被缩短
    ''' </summary>
    ''' <param name="refLoci"></param>
    ''' <param name="RightLimit"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overridable Function CreateBackwardModel(refLoci As Point, RightLimit As Integer) As Drawing2D.GraphicsPath
        Dim Graphic As New Drawing2D.GraphicsPath
        Dim pt_lefttop As System.Drawing.Point = New System.Drawing.Point(refLoci.X + HeadLength, refLoci.Y)
        Dim pt_lefttoptop As System.Drawing.Point = New System.Drawing.Point(pt_lefttop.X, pt_lefttop.Y - OFFSET)

        Dim pt_arrowHead As System.Drawing.Point = New System.Drawing.Point(refLoci.X, pt_lefttop.Y + 0.5 * Height)

        Dim pt_leftbottombottom As System.Drawing.Point = New System.Drawing.Point(pt_lefttop.X, pt_lefttop.Y + Height + OFFSET)
        Dim pt_leftbottom As System.Drawing.Point = New System.Drawing.Point(pt_lefttop.X, refLoci.Y + Height)

        Dim pt_righttop As System.Drawing.Point = New System.Drawing.Point(refLoci.X + Length, refLoci.Y)
        If pt_righttop.X > RightLimit Then
            pt_righttop = New Point(RightLimit, pt_righttop.Y)
        End If
        Dim pt_rightbottom As System.Drawing.Point = New System.Drawing.Point(pt_righttop.X, pt_leftbottom.Y)

        Call Graphic.AddLine(pt_lefttop, pt_lefttoptop)
        Call Graphic.AddLine(pt_lefttoptop, pt_arrowHead)

        Call Graphic.AddLine(pt_arrowHead, pt_leftbottombottom)
        Call Graphic.AddLine(pt_leftbottombottom, pt_leftbottom)

        Call Graphic.AddLine(pt_leftbottom, pt_rightbottom)
        Call Graphic.AddLine(pt_righttop, pt_rightbottom)
        Call Graphic.AddLine(pt_righttop, pt_lefttop)

        Return Graphic
    End Function

    ''' <summary>
    ''' 假若所绘制出来的模型的右部分的坐标超过了<paramref name="RightLimit"></paramref>这个参数，则会被缩短
    ''' </summary>
    ''' <param name="refLoci"></param>
    ''' <param name="RightLimit"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overridable Function CreateNoneDirectionModel(refLoci As Point, RightLimit As Integer) As Drawing2D.GraphicsPath
        Dim Graphic As New Drawing2D.GraphicsPath
        Dim pt_lefttop As New Point(refLoci.X, refLoci.Y)
        Dim pt_leftbottom As New Point(refLoci.X, refLoci.Y + Height)
        Dim pt_righttop As New Point(refLoci.X + Length, refLoci.Y)
        If pt_righttop.X > RightLimit Then
            pt_righttop = New Point(RightLimit, pt_righttop.Y)
        End If
        Dim pt_rightbottom As New Point(pt_righttop.X, pt_leftbottom.Y)

        Call Graphic.AddLine(pt_lefttop, pt_righttop)
        Call Graphic.AddLine(pt_lefttop, pt_leftbottom)
        Call Graphic.AddLine(pt_righttop, pt_rightbottom)
        Call Graphic.AddLine(pt_leftbottom, pt_rightbottom)

        Return Graphic
    End Function
End Class
