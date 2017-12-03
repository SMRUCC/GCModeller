#Region "Microsoft.VisualBasic::f8bc454e7fc23e81b257c4264e48c711, ..\GCModeller\visualize\GCModeller.DataVisualization\MapLabelLayout.vb"

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
Imports Microsoft.VisualBasic.Linq

Public Structure MapLabelLayout

    Dim ConflictRegion As Rectangle

    ''' <summary>
    ''' 
    ''' </summary>
    Dim baseY!

    Sub New(text$, font As Font, g As Graphics, location As Point, Optional baseY! = 0!)
        With Me
            .baseY = baseY
            .ConflictRegion = New Rectangle With {
                .Location = location,
                .Size = g.MeasureString(text, font).ToSize
            }
        End With
    End Sub

    ''' <summary>
    ''' 当标签是从左到右排列的时候的layout位置的变换
    ''' </summary>
    ''' <param name="rect"></param>
    ''' <returns></returns>
    Public Function ForceNextLocation(rect As Rectangle, Optional ByRef conflict As Boolean = False) As Rectangle
        Dim ptr As Point = rect.Location
        Dim size As Size = rect.Size

        ' 有冲突
        If ConflictRegion.Right >= rect.Left Then
            Dim yconflicts As Boolean

            With ConflictRegion
                yconflicts = rect.Top.InRange(.Top, .Bottom) OrElse rect.Bottom.InRange(.Top, .Bottom)
            End With

            If yconflicts Then
                rect = New Rectangle With {
                    .Location = New Point(ptr.X, ConflictRegion.Top - size.Height),
                    .Size = New Size(size.Width, size.Height)
                }
                conflict = True
            End If
        Else
            conflict = False
        End If

        Return rect
    End Function
End Structure

