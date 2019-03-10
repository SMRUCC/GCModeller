﻿#Region "Microsoft.VisualBasic::0b54975d5aecd969cadb05a679bc23fe, Microsoft.VisualBasic.Core\Language\Linq\Vectorization\BooleanVector.vb"

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

    '     Class BooleanVector
    ' 
    '         Properties: [False], [True], IsLogical
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Sum, ToString
    '         Operators: (+2 Overloads) IsFalse, (+2 Overloads) IsTrue, (+2 Overloads) Not, (+4 Overloads) Or
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Namespace Language.Vectorization

    ''' <summary>
    ''' <see cref="System.Boolean"/> Array
    ''' </summary>
    Public Class BooleanVector : Inherits Vector(Of Boolean)

        ''' <summary>
        ''' Only one boolean value ``True`` in the array list
        ''' </summary>
        ''' <returns></returns>
        Public Shared ReadOnly Property [True] As BooleanVector
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return New BooleanVector({True})
            End Get
        End Property

        ''' <summary>
        ''' Only one boolean value ``False`` in the array list
        ''' </summary>
        ''' <returns></returns>
        Public Shared ReadOnly Property [False] As BooleanVector
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return New BooleanVector({False})
            End Get
        End Property

        Public ReadOnly Property IsLogical As Boolean
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return buffer.Length = 1
            End Get
        End Property

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Sub New(b As IEnumerable(Of Boolean))
            MyBase.New(b)
        End Sub

        Public Overrides Function ToString() As String
            Dim countTrue% = Linq.Which.IsTrue(buffer).Count
            Dim countFalse% = Linq.Which.IsTrue(Not Me).Count

            Return $"ALL({Length}) = {countTrue} true + {countFalse} false"
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function Sum(b As BooleanVector) As Integer
            Return b.Select(Function(x) If(x, 1, 0)).Sum
        End Function

        ''' <summary>
        ''' And
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="y"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Operator &(x As Boolean, y As BooleanVector) As BooleanVector
            Return New BooleanVector(From b As Boolean In y Select b AndAlso x)
        End Operator

        ''' <summary>
        ''' X AndAlso Y
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="y"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Operator &(x As BooleanVector, y As BooleanVector) As BooleanVector
            Return New BooleanVector(From i As SeqValue(Of Boolean) In x.SeqIterator Select i.value AndAlso y(i))
        End Operator

        ''' <summary>
        ''' 将逻辑向量之中的每一个逻辑值都进行翻转
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Operator Not(x As BooleanVector) As BooleanVector
            Return New BooleanVector((From b As Boolean In x Select Not b).ToArray)
        End Operator

        ''' <summary>
        ''' x(0)
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Narrowing Operator CType(x As BooleanVector) As Boolean
            Return x(0)
        End Operator

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Widening Operator CType(b As Boolean()) As BooleanVector
            Return New BooleanVector(b)
        End Operator

        ''' <summary>
        ''' x Or Y
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="y"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Operator Or(x As BooleanVector, y As Boolean()) As BooleanVector
            Return New BooleanVector(x.Select(Function(b, i) b OrElse y(i)))
        End Operator

        ''' <summary>
        ''' x Or Y
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="y"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Operator Or(x As BooleanVector, y As BooleanVector) As BooleanVector
            Return x Or y.ToArray
        End Operator

        ''' <summary>
        ''' <see cref="ToArray"/>
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Narrowing Operator CType(x As BooleanVector) As Boolean()
            Return x.ToArray
        End Operator

        Public Shared Operator IsTrue(b As BooleanVector) As Boolean
            If b.IsNullOrEmpty Then
                Return False
            Else
                Return Not b.Any(Function(x) x = False)
            End If
        End Operator

        Public Shared Operator IsFalse(b As BooleanVector) As Boolean
            If b Then
                ' b是True，则不是False，在这里返回False，表明IsFalse不成立
                Return False
            Else
                Return True
            End If
        End Operator
    End Class
End Namespace
