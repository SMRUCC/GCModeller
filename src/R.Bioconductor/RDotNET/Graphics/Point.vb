#Region "Microsoft.VisualBasic::9fa087b2e8604451fc586ba4dc80196f, ..\R.Bioconductor\RDotNET\Graphics\Point.vb"

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

Namespace Graphics

    Public Structure Point
        Implements IEquatable(Of Point)
        Private m_x As Double
        Private m_y As Double

        Public Sub New(x As Double, y As Double)
            Me.m_x = x
            Me.m_y = y
        End Sub

        Public Property X() As Double
            Get
                Return Me.m_x
            End Get
            Set(value As Double)
                Me.m_x = Value
            End Set
        End Property

        Public Property Y() As Double
            Get
                Return Me.m_y
            End Get
            Set(value As Double)
                Me.m_y = Value
            End Set
        End Property

#Region "IEquatable<Point> Members"

        Public Overloads Function Equals(other As Point) As Boolean Implements IEquatable(Of RDotNet.Graphics.Point).Equals
            Return (Me = other)
        End Function

#End Region

        Public Shared Operator =(p1 As Point, p2 As Point) As Boolean
            Return p1.X = p2.X AndAlso p1.Y = p2.Y
        End Operator

        Public Shared Operator <>(p1 As Point, p2 As Point) As Boolean
            Return Not (p1 = p2)
        End Operator

        Public Overrides Function GetHashCode() As Integer
            Const Prime As Integer = 31
            Return Prime * X.GetHashCode() + Y.GetHashCode()
        End Function

        Public Overrides Function Equals(obj As Object) As Boolean
            If TypeOf obj Is Point Then
                Dim point = CType(obj, Point)
                Return (Me = point)
            End If
            Return False
        End Function
    End Structure
End Namespace
