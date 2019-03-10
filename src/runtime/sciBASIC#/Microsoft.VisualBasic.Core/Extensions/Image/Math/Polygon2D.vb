﻿#Region "Microsoft.VisualBasic::3de1704040fc26424f9297c075be7162, Microsoft.VisualBasic.Core\Extensions\Image\Math\Polygon2D.vb"

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

    '     Class Polygon2D
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: boundingInside, (+2 Overloads) inside
    ' 
    '         Sub: calculateBounds
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports sys = System.Math

Namespace Imaging.Math2D

    Public Class Polygon2D

        Public npoints As Integer = 0
        Public xpoints As Double() = New Double(3) {}
        Public ypoints As Double() = New Double(3) {}

        Protected Friend bounds1 As Vector2D = Nothing
        Protected Friend bounds2 As Vector2D = Nothing

        Public Sub New()
        End Sub

        Public Sub New(x As Double(), y As Double(), points As Integer)
            Me.npoints = points
            Me.xpoints = New Double(points - 1) {}
            Me.ypoints = New Double(points - 1) {}

            Array.Copy(x, 0, Me.xpoints, 0, points)
            Array.Copy(y, 0, Me.ypoints, 0, points)

            Call calculateBounds(x, y, points)
        End Sub

        Friend Overridable Sub calculateBounds(x As Double(), y As Double(), n As Integer)
            Dim d1 As Double = Double.MaxValue
            Dim d2 As Double = Double.MaxValue
            Dim d3 As Double = Double.MinValue
            Dim d4 As Double = Double.MinValue

            For i As Integer = 0 To n - 1
                Dim d5 As Double = x(i)
                d1 = sys.Min(d1, d5)
                d3 = Math.Max(d3, d5)
                Dim d6 As Double = y(i)
                d2 = sys.Min(d2, d6)
                d4 = Math.Max(d4, d6)
            Next

            Me.bounds1 = New Vector2D(d1, d2)
            Me.bounds2 = New Vector2D(d3, d4)
        End Sub

        Friend Overridable Function boundingInside(x As Double, y As Double) As Boolean
            Return (x >= Me.bounds1.x) AndAlso (x <= Me.bounds2.x) AndAlso (y >= Me.bounds1.y) AndAlso (y <= Me.bounds2.y)
        End Function

        Public Overridable Function inside(paramVector2D As Vector2D) As Boolean
            Return inside(paramVector2D.x, paramVector2D.y)
        End Function

        ''' <summary>
        ''' @deprecated
        ''' </summary>
        Public Overridable Function inside(x As Double, y As Double) As Boolean
            If boundingInside(x, y) Then
                Dim i As Integer = 0
                Dim d1 As Double = 0.0
                Dim j As Integer = 0
                While (j < Me.npoints) AndAlso (Me.ypoints(j) = y)
                    j += 1
                End While
                For k As Integer = 0 To Me.npoints - 1
                    Dim m As Integer = (j + 1) Mod Me.npoints
                    Dim d2 As Double = Me.xpoints(m) - Me.xpoints(j)
                    Dim d3 As Double = Me.ypoints(m) - Me.ypoints(j)
                    If d3 <> 0.0 Then
                        Dim d4 As Double = x - Me.xpoints(j)
                        Dim d5 As Double = y - Me.ypoints(j)
                        If (Me.ypoints(m) = y) AndAlso (Me.xpoints(m) >= x) Then
                            d1 = Me.ypoints(j)
                        End If
                        If (Me.ypoints(j) = y) AndAlso (Me.xpoints(j) >= x) Then
                            If (If(d1 > y, 1, 0)) <> (If(Me.ypoints(m) > y, 1, 0)) Then
                                i -= 1
                            End If
                        End If
                        Dim f As Single = CSng(d5) / CSng(d3)
                        If (f >= 0.0) AndAlso (f <= 1.0) AndAlso (f * d2 >= d4) Then
                            i += 1
                        End If
                    End If
                    j = m
                Next
                Return i Mod 2 <> 0
            End If
            Return False
        End Function
    End Class
End Namespace
