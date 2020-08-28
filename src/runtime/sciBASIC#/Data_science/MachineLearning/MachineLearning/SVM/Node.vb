' 
 ' * SVM.NET Library
 ' * Copyright (C) 2008 Matthew Johnson
 ' * 
 ' * This program is free software: you can redistribute it and/or modify
 ' * it under the terms of the GNU General Public License as published by
 ' * the Free Software Foundation, either version 3 of the License, or
 ' * (at your option) any later version.
 ' * 
 ' * This program is distributed in the hope that it will be useful,
 ' * but WITHOUT ANY WARRANTY; without even the implied warranty of
 ' * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 ' * GNU General Public License for more details.
 ' * 
 ' * You should have received a copy of the GNU General Public License
 ' * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 


Imports System
Imports stdNum = System.Math

Namespace SVM
    ''' <summary>
    ''' Encapsulates a node in a Problem vector, with an index and a value (for more efficient representation
    ''' of sparse data.
    ''' </summary>
    <Serializable>
    Public Class Node
        Implements IComparable(Of Node)

        Friend _index As Integer
        Friend _value As Double

        ''' <summary>
        ''' Default Constructor.
        ''' </summary>
        Public Sub New()
        End Sub
        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <param name="index">The index of the value.</param>
        ''' <param name="value">The value to store.</param>
        Public Sub New(ByVal index As Integer, ByVal value As Double)
            _index = index
            _value = value
        End Sub

        ''' <summary>
        ''' Index of this Node.
        ''' </summary>
        Public Property Index As Integer
            Get
                Return _index
            End Get
            Set(ByVal value As Integer)
                _index = value
            End Set
        End Property
        ''' <summary>
        ''' Value at Index.
        ''' </summary>
        Public Property Value As Double
            Get
                Return _value
            End Get
            Set(ByVal value As Double)
                _value = value
            End Set
        End Property

        ''' <summary>
        ''' String representation of this Node as {index}:{value}.
        ''' </summary>
        ''' <returns>{index}:{value}</returns>
        Public Overrides Function ToString() As String
            Return String.Format("{0}:{1}", _index, _value.Truncate())
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Dim other As Node = TryCast(obj, Node)
            If other Is Nothing Then Return False
            Return _index = other._index AndAlso _value.Truncate() = other._value.Truncate()
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return _index.GetHashCode() + _value.GetHashCode()
        End Function

#Region "IComparable<Node> Members"

        ''' <summary>
        ''' Compares this node with another.
        ''' </summary>
        ''' <param name="other">The node to compare to</param>
        ''' <returns>A positive number if this node is greater, a negative number if it is less than, or 0 if equal</returns>
        Public Function CompareTo(ByVal other As Node) As Integer Implements IComparable(Of Node).CompareTo
            Return _index.CompareTo(other._index)
        End Function

#End Region
    End Class
End Namespace
