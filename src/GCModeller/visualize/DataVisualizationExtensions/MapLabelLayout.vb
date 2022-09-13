#Region "Microsoft.VisualBasic::a1d4340af6455740d001ec9405453845, GCModeller\visualize\DataVisualizationExtensions\MapLabelLayout.vb"

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

    '   Total Lines: 69
    '    Code Lines: 43
    ' Comment Lines: 15
    '   Blank Lines: 11
    '     File Size: 2.16 KB


    ' Structure MapLabelLayout
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: ForceNextLocation
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.Linq

Public Structure MapLabelLayout

    Dim OverlapRegion As Rectangle

    ''' <summary>
    ''' 
    ''' </summary>
    Dim baseY!

    Sub New(text$, font As Font, g As Graphics, location As Point, Optional baseY! = 0!)
        With Me
            .baseY = baseY
            .OverlapRegion = New Rectangle With {
                .Location = location,
                .Size = g.MeasureString(text, font).ToSize
            }
        End With
    End Sub

    ''' <summary>
    ''' 当标签是从左到右排列的时候的layout位置的变换
    ''' </summary>
    ''' <param name="rect"></param>
    ''' <param name="conflict">
    ''' 通过这个函数参数来标记是否产生了冲突
    ''' </param>
    ''' <param name="directionDown">
    ''' 当产生冲突的时候，是否向下延申，默认是向上延申
    ''' </param>
    ''' <returns></returns>
    Public Function ForceNextLocation(rect As Rectangle,
                                      Optional ByRef conflict As Boolean = False,
                                      Optional directionDown As Boolean = False) As Rectangle

        Dim ptr As Point = rect.Location
        Dim size As Size = rect.Size

        ' 有冲突
        If OverlapRegion.Right >= rect.Left Then
            Dim yconflicts As Boolean
            Dim y%

            With OverlapRegion
                yconflicts = rect.Top.InRange(.Top, .Bottom) OrElse rect.Bottom.InRange(.Top, .Bottom)
            End With

            If yconflicts Then
                If directionDown Then
                    y = OverlapRegion.Top + size.Height
                Else
                    y = OverlapRegion.Top - size.Height
                End If

                rect = New Rectangle With {
                    .Location = New Point(ptr.X, y),
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
