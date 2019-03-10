﻿#Region "Microsoft.VisualBasic::88d91167e51e85229b2393b3f24fdc4c, Data_science\Mathematica\Plot\Plots\Scatter\Data.vb"

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

    ' Class SerialData
    ' 
    '     Properties: DataAnnotations, title
    ' 
    '     Function: GetEnumerator, GetPen, GetPointByX, IEnumerable_GetEnumerator, ToString
    ' 
    '     Sub: AddMarker
    ' 
    ' Structure PointData
    ' 
    '     Constructor: (+3 Overloads) Sub New
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' 一条曲线的绘图数据模型
''' </summary>
Public Class SerialData : Implements INamedValue
    Implements IEnumerable(Of PointData)

    ''' <summary>
    ''' 绘图的点的数据，请注意，这里面的点之间是有顺序之分的
    ''' </summary>
    Public pts As PointData()
    Public lineType As DashStyle = DashStyle.Solid
    Public Property title As String Implements INamedValue.Key

    ''' <summary>
    ''' 点的半径大小
    ''' </summary>
    Public PointSize As Single = 1

    ''' <summary>
    ''' 线条的颜色
    ''' </summary>
    Public color As Color = Color.Black
    Public width As Single = 20
    Public Shape As LegendStyles = LegendStyles.Circle

    ''' <summary>
    ''' 对一系列特定的数据点的注释数据
    ''' </summary>
    ''' <returns></returns>
    Public Property DataAnnotations As Annotation()

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetPen() As Pen
        Return New Pen(color:=color, width:=width) With {
            .DashStyle = lineType
        }
    End Function

    ''' <summary>
    ''' 由于在绘图的时候，需要按照标题查找原始数据，所以请确保绘图的曲线的系列数据之中的<see cref="SerialData.title"/>不会重复
    ''' </summary>
    ''' <param name="x!"></param>
    ''' <param name="title$"></param>
    ''' <param name="color$"></param>
    ''' <param name="font$"></param>
    ''' <param name="style"></param>
    Public Sub AddMarker(x!, title$, color$, Optional font$ = CSSFont.Win10Normal, Optional style As LegendStyles = LegendStyles.Circle)
        If DataAnnotations Is Nothing Then
            DataAnnotations = New Annotation(0) {}
        Else
            ReDim Preserve DataAnnotations(DataAnnotations.Length)
        End If

        DataAnnotations(DataAnnotations.Length - 1) =
            New Annotation With {
                .X = x,
                .Text = title,
                .color = color,
                .Font = font,
                .Legend = style
        }
    End Sub

    Public Function GetPointByX(x As Single) As PointData
        For Each pt As PointData In pts
            If pt.pt.X = x Then
                Return pt
            End If
        Next

        Return Nothing
    End Function

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

    Public Iterator Function GetEnumerator() As IEnumerator(Of PointData) Implements IEnumerable(Of PointData).GetEnumerator
        For Each x In pts
            Yield x
        Next
    End Function

    Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Yield GetEnumerator()
    End Function
End Class

''' <summary>
''' 绘图的点的数据
''' </summary>
Public Structure PointData

    ''' <summary>
    ''' 坐标数据不需要进行额外的转换，绘图函数内部会自动进行mapping转换的
    ''' </summary>
    Public pt As PointF
    ''' <summary>
    ''' 正负误差
    ''' </summary>
    Public errPlus#, errMinus#, Tag$, value#
    ''' <summary>
    ''' 可能会有数据点在<see cref="errPlus"/>或者<see cref="errMinus"/>范围内，或者范围外
    ''' </summary>
    Public Statics#()
    Public color$
    Public stroke$

    Sub New(x!, y!)
        pt = New PointF(x, y)
    End Sub

    Sub New(pt As PointF)
        Me.pt = pt
    End Sub

    Sub New(pt As Point)
        Me.pt = New PointF(pt.X, pt.Y)
    End Sub

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Structure
