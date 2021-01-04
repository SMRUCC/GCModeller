﻿#Region "Microsoft.VisualBasic::95139befeafc8875d31a9c3935bef117, Microsoft.VisualBasic.Core\ComponentModel\Ranges\Selector\IndexSelector.vb"

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

    '     Class IndexSelector
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: FromSortSequence, SelectByRange
    ' 
    '     Class OrderSelector
    ' 
    '         Properties: Count, Desc, Max, Min
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Find, FirstGreaterThan, GetEnumerator, IEnumerable_GetEnumerator, SelectByRange
    '                   SelectUntilGreaterThan, SelectUntilLessThan, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.TagData
Imports Numeric = Microsoft.VisualBasic.Language.Numeric

Namespace ComponentModel.Ranges

    ''' <summary>
    ''' A numeric index helper
    ''' </summary>
    Public Class IndexSelector : Inherits OrderSelector(Of NumericTagged(Of Integer))

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub New(source As IEnumerable(Of Double), Optional asc As Boolean = True)
            MyBase.New(source.Select(Function(d, i) New NumericTagged(Of Integer)(d, i)), asc)
        End Sub

        Private Sub New(sorts As IEnumerable(Of NumericTagged(Of Integer)), asc As Boolean)
            Call MyBase.New({}, asc)
            source = sorts.ToArray
        End Sub

        ''' <summary>
        ''' Get index by a given numeric range
        ''' </summary>
        ''' <param name="min"></param>
        ''' <param name="max"></param>
        ''' <returns></returns>
        Public Overloads Function SelectByRange(min As Double, max As Double) As IEnumerable(Of Integer)
            Dim minValue As New NumericTagged(Of Integer)(min, 0)
            Dim maxvalue As New NumericTagged(Of Integer)(max, 0)

            Return source.SkipWhile(Function(o) Numeric.LessThan(o, minValue)) _
                         .TakeWhile(Function(o) Numeric.LessThanOrEquals(o, maxvalue)) _
                         .Select(Function(tag)
                                     Return tag.value
                                 End Function)
        End Function

        ''' <summary>
        ''' 所使用的序列参数必须是经过了排序操作的
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function FromSortSequence(seq As Double()) As IndexSelector
            Return New IndexSelector(seq.Select(Function(d, i) New NumericTagged(Of Integer)(d, i)), True)
        End Function
    End Class

    Public Class OrderSelector(Of T As IComparable) : Implements IReadOnlyCollection(Of T)

        ''' <summary>
        ''' <see cref="source"/>序列的排序的方向字符串显示
        ''' </summary>
        ReadOnly direct As String

        ''' <summary>
        ''' 目标序列
        ''' </summary>
        Protected source As T()

        ''' <summary>
        ''' 是否为降序排序?
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Desc As Boolean

        Default Public ReadOnly Property Item(index%) As T
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return source(index)
            End Get
        End Property

        Public ReadOnly Property Max As T
            Get
                If Desc Then
                    Return source.First
                Else
                    Return source.Last
                End If
            End Get
        End Property

        Public ReadOnly Property Min As T
            Get
                If Desc Then
                    Return source.Last
                Else
                    Return source.First
                End If
            End Get
        End Property

        Public ReadOnly Property Count As Integer Implements IReadOnlyCollection(Of T).Count
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return source.Length
            End Get
        End Property

        Shared ReadOnly compare As Func(Of T, T) = Function(x) x

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="asc">
        ''' 当这个参数为真的时候为升序排序
        ''' </param>
        Sub New(source As IEnumerable(Of T), Optional asc As Boolean = True)
            If asc Then
                Me.source = source.OrderBy(compare).ToArray
                direct = " -> "
            Else
                Me.source = source _
                    .OrderByDescending(compare) _
                    .ToArray
                direct = " <- "
            End If

            Desc = Not asc
        End Sub

        ''' <summary>
        ''' Find value by key via binary search
        ''' </summary>
        ''' <typeparam name="K"></typeparam>
        ''' <param name="key"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Find(Of K As IComparable(Of K))(key As K, getKey As Func(Of T, K), Optional [default] As T = Nothing) As T
            Return source.BinarySearch(key, getKey, [default])
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return $"[{direct}] {GetType(T).ToString}"
        End Function

        ''' <summary>
        ''' 直到当前元素大于指定值
        ''' </summary>
        ''' <param name="n"></param>
        ''' <returns></returns>
        Public Iterator Function SelectUntilGreaterThan(n As T) As IEnumerable(Of T)
            For Each x In source
                If Numeric.LessThanOrEquals(x, n) Then
                    Yield x
                Else
                    ' 由于是经过排序了的，所以在这里不再小于的话，则后面的元素都不会再比他小了
                    Exit For
                End If
            Next
        End Function

        ''' <summary>
        ''' 直到当前元素小于指定值
        ''' </summary>
        ''' <param name="n"></param>
        ''' <returns></returns>
        Public Iterator Function SelectUntilLessThan(n As T) As IEnumerable(Of T)
            For Each x In source
                If Numeric.GreaterThanOrEquals(x, n) Then
                    Yield x
                Else
                    Exit For
                End If
            Next
        End Function

        ''' <summary>
        ''' 遍历整个列表直到找到第一个大于<paramref name="o"/>的元素，然后函数会返回这第一个元素的index
        ''' </summary>
        ''' <param name="o"></param>
        ''' <returns>
        ''' 返回-1表示这个列表之中没有任何元素是大于输入的参数<paramref name="o"/>的
        ''' </returns>
        Public Function FirstGreaterThan(o As T) As Integer
            For i As Integer = 0 To source.Length - 1
                If Not Numeric.GreaterThan(o, source(i)) Then
                    Return i
                End If
            Next

            Return -1
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function SelectByRange(min As T, max As T) As IEnumerable(Of T)
            Return source.SkipWhile(Function(o) Numeric.LessThan(o, min)) _
                         .TakeWhile(Function(o) Numeric.LessThanOrEquals(o, max))
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
            For Each x As T In source
                Yield x
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace
