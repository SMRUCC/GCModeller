﻿#Region "Microsoft.VisualBasic::be0b5b537da0f446e63d10fe47f55aec, Data_science\Graph\Model\Grid.vb"

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

    ' Class Grid
    ' 
    '     Properties: Layout, Steps
    ' 
    '     Constructor: (+3 Overloads) Sub New
    '     Function: Index, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Imaging

''' <summary>
''' 网格也可以看作为一种网络
''' </summary>
Public Class Grid

    ReadOnly X, Y As OrderSelector(Of Double)
    ReadOnly rect As RectangleF

    Public ReadOnly Property Layout As RectangleF
        Get
            Return rect
        End Get
    End Property

    Public ReadOnly Property Steps As SizeF

    Sub New(size As Size, steps As SizeF)
        Call Me.New(New Rectangle(New Point, size), steps)
    End Sub

    Sub New(layout As Rectangle, steps As SizeF)
        Call Me.New(layout.ToFloat, steps)
    End Sub

    Sub New(layout As RectangleF, steps As SizeF)
        X = New OrderSelector(Of Double)(Math.seq(layout.X, layout.Right, steps.Width))
        Y = New OrderSelector(Of Double)(Math.seq(layout.Y, layout.Bottom, steps.Height))

        Me.steps = steps
        Me.rect = layout
    End Sub

    ''' <summary>
    ''' 返回数据点在网格之中的``X,Y``方格的顶点编号
    ''' </summary>
    ''' <param name="p"></param>
    ''' <returns></returns>
    Public Function Index(p As PointF) As Point
        Dim xi = X.FirstGreaterThan(p.X)
        Dim yi = Y.FirstGreaterThan(p.Y)

        Return New Point(xi, yi)
    End Function

    Public Overrides Function ToString() As String
        Return $"{rect} @ {steps}"
    End Function
End Class
