#Region "Microsoft.VisualBasic::16ae89138cb3abc26284288a320ca668, RDotNET\Graphics\Point.vb"

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

    '     Structure Point
    ' 
    '         Properties: X, Y
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: (+2 Overloads) Equals, GetHashCode
    '         Operators: <>, =
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Graphics
    Public Structure Point : Implements IEquatable(Of Point)

        Public Sub New(x As Double, y As Double)
            _X = x
            _Y = y
        End Sub

        Public Property X() As Double
        Public Property Y() As Double

#Region "IEquatable<Point> Members"

        Public Overloads Function Equals(other As Point) As Boolean Implements IEquatable(Of Point).Equals
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
