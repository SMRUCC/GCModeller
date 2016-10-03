#Region "Microsoft.VisualBasic::29fd11b019a57179f9441d0474d7f243, ..\R.Bioconductor\RDotNET\Graphics\Rectangle.vb"

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

    Public Structure Rectangle
        Implements IEquatable(Of Rectangle)
        Private m_height As Double
        Private m_width As Double
        Private m_x As Double
        Private m_y As Double

        Public Sub New(x As Double, y As Double, width As Double, height As Double)
            Me.m_x = x
            Me.m_y = y
            Me.m_width = width
            Me.m_height = height
        End Sub

        Public Sub New(location As Point, size As Size)
            Me.m_x = location.X
            Me.m_y = location.Y
            Me.m_width = size.Width
            Me.m_height = size.Height
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

        Public Property Width() As Double
            Get
                Return Me.m_width
            End Get
            Set(value As Double)
                Me.m_width = Value
            End Set
        End Property

        Public Property Height() As Double
            Get
                Return Me.m_height
            End Get
            Set(value As Double)
                Me.m_height = Value
            End Set
        End Property

        Public ReadOnly Property Left() As Double
            Get
                Return X
            End Get
        End Property

        Public ReadOnly Property Right() As Double
            Get
                Return X + Width
            End Get
        End Property

        Public ReadOnly Property Bottom() As Double
            Get
                Return Y
            End Get
        End Property

        Public ReadOnly Property Top() As Double
            Get
                Return Y + Height
            End Get
        End Property

        Public Property Location() As Point
            Get
                Return New Point(X, Y)
            End Get
            Set(value As Point)
                X = Value.X
                Y = Value.Y
            End Set
        End Property

        Public Property Size() As Size
            Get
                Return New Size(Width, Height)
            End Get
            Set(value As Size)
                Width = Value.Width
                Height = Value.Height
            End Set
        End Property

#Region "IEquatable<Rectangle> Members"

        Public Overloads Function Equals(other As Rectangle) As Boolean Implements IEquatable(Of RDotNet.Graphics.Rectangle).Equals
            Return (Me = other)
        End Function

#End Region

        Public Shared Operator =(r1 As Rectangle, r2 As Rectangle) As Boolean
            Return r1.Location = r2.Location AndAlso r1.Size = r2.Size
        End Operator

        Public Shared Operator <>(r1 As Rectangle, r2 As Rectangle) As Boolean
            Return Not (r1 = r2)
        End Operator

        Public Overrides Function GetHashCode() As Integer
            Return Location.GetHashCode() Xor Size.GetHashCode()
        End Function

        Public Overrides Function Equals(obj As Object) As Boolean
            If TypeOf obj Is Rectangle Then
                Dim rectangle = CType(obj, Rectangle)
                Return (Me = rectangle)
            End If
            Return False
        End Function
    End Structure
End Namespace
