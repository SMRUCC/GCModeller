#Region "Microsoft.VisualBasic::53df2c7102d6c0ad50f209e028cc306b, RDotNET\Graphics\Size.vb"

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

    '     Structure Size
    ' 
    '         Properties: Height, Width
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: (+2 Overloads) Equals, GetHashCode
    '         Operators: <>, =
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Graphics
    Public Structure Size
        Implements IEquatable(Of Size)
        Private m_height As Double
        Private m_width As Double

        Public Sub New(width As Double, height As Double)
            Me.m_width = width
            Me.m_height = height
        End Sub

        Public Property Width() As Double
            Get
                Return Me.m_width
            End Get
            Set
                Me.m_width = value
            End Set
        End Property

        Public Property Height() As Double
            Get
                Return Me.m_height
            End Get
            Set
                Me.m_height = value
            End Set
        End Property

#Region "IEquatable<Size> Members"

        Public Overloads Function Equals(other As Size) As Boolean Implements IEquatable(Of Size).Equals
            Return (Me = other)
        End Function

#End Region

        Public Shared Operator =(size1 As Size, size2 As Size) As Boolean
            Return size1.Width = size2.Width AndAlso size1.Height = size2.Height
        End Operator

        Public Shared Operator <>(size1 As Size, size2 As Size) As Boolean
            Return Not (size1 = size2)
        End Operator

        Public Overrides Function GetHashCode() As Integer
            Const Prime As Integer = 31
            Return Prime * Width.GetHashCode() + Height.GetHashCode()
        End Function

        Public Overrides Function Equals(obj As Object) As Boolean
            If TypeOf obj Is Size Then
                Dim size = CType(obj, Size)
                Return (Me = size)
            End If
            Return False
        End Function
    End Structure
End Namespace
