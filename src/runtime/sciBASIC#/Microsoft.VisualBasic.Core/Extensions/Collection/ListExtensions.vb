﻿#Region "Microsoft.VisualBasic::6150a3e21796659b00ef499866c6d33d, Microsoft.VisualBasic.Core\Extensions\Collection\ListExtensions.vb"

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

    ' Module ListExtensions
    ' 
    '     Function: __reversedTake, AppendAfter, AsHashList, AsHashSet, AsList
    '               AsLoop, HasKey, Indexing, rand, Random
    '               ReorderByKeys, Takes, (+2 Overloads) ToList, TopMostFrequent
    ' 
    '     Sub: DoEach, ForEach, Swap
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' Initializes a new instance of the <see cref="List"/>`1 class that
''' contains elements copied from the specified collection and has sufficient capacity
''' to accommodate the number of elements copied.
''' </summary>
Public Module ListExtensions

    ''' <summary>
    ''' 将<paramref name="join"/>加入到<paramref name="list"/>序列后面
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="join"></param>
    ''' <param name="list"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function AppendAfter(Of T)(join As IEnumerable(Of T), list As IEnumerable(Of T)) As IEnumerable(Of T)
        For Each x In list.SafeQuery
            Yield x
        Next
        For Each x In join.SafeQuery
            Yield x
        Next
    End Function

    ''' <summary>
    ''' 查找出序列之中最频繁出现的对象(这个函数会自动跳过空值)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="list"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function TopMostFrequent(Of T)(list As IEnumerable(Of T), Optional equals As IEqualityComparer(Of T) = Nothing) As T
        ' 因为可能会碰到list是空的情况，所以在这里需要使用FirstOrDefault
        If equals Is Nothing Then
            Return list _
                .SafeQuery _
                .Where(Function(x) Not x Is Nothing) _
                .GroupBy(Function(x) x) _
                .OrderByDescending(Function(g) g.Count) _
                .FirstOrDefault _
                .SafeQuery _
                .FirstOrDefault
        Else
            Return list _
                .SafeQuery _
                .Where(Function(x) Not x Is Nothing) _
                .GroupBy(Function(x) x, equals) _
                .OrderByDescending(Function(g) g.Count) _
                .FirstOrDefault _
                .SafeQuery _
                .FirstOrDefault
        End If
    End Function

    ''' <summary>
    ''' ForEach拓展的简化版本
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="collection"></param>
    ''' <param name="[do]"></param>
    <Extension> Public Sub DoEach(Of T)(collection As IEnumerable(Of T), [do] As Action(Of T))
        For Each x As T In collection.SafeQuery
            Call [do](x)
        Next
    End Sub

    Private Function rand(min%, max%) As Integer
        Static rnd As New Random
        SyncLock rnd
            Return rnd.Next(min, max)
        End SyncLock
    End Function

    ''' <summary>
    ''' 返回数组集合之中的一个随机位置的元素
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="v"></param>
    ''' <returns></returns>
    <Extension> Public Function Random(Of T)(v As T()) As T
        Dim l% = rand(0, v.Length)
        Return v(l)
    End Function

    ''' <summary>
    ''' 根据对象的键名来进行重排序，请注意，要确保对象<paramref name="getKey"/>能够从泛型对象之中获取得到唯一的键名
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="list"></param>
    ''' <param name="getKey"></param>
    ''' <param name="customOrder">可能会出现大小写不对的情况？</param>
    ''' <returns></returns>
    <Extension>
    Public Function ReorderByKeys(Of T)(list As IEnumerable(Of T), getKey As Func(Of T, String), customOrder$()) As List(Of T)
        Dim ls As List(Of T) = list.AsList
        Dim list2 As New List(Of T)
        Dim internalGet_geneObj =
            Function(id As String)
                Dim query = From x
                            In ls.AsParallel
                            Let key As String = getKey(x)
                            Where key.TextEquals(id) OrElse
                                InStr(key, id, CompareMethod.Text) > 0 ' 假若是对基因组进行排序，可能getkey函数只获取得到的是编号，而customOrder之中还会包含有全称，所以用InStr判断一下？
                            Select x '
                Return query.FirstOrDefault
            End Function

        For Each ID As String In customOrder
            Dim selectedItem As T = internalGet_geneObj(ID)

            If Not selectedItem Is Nothing Then ' 由于是倒序的，故而将对象移动到最后一个元素即可
                Call list2.Add(selectedItem)
                Call ls.Remove(selectedItem)
            End If
        Next

        Call list2.AddRange(ls) ' 添加剩余的没有在customOrder之中找到的数据

        Return list2
    End Function

    ''' <summary>
    ''' Take elements by <paramref name="index"/> list. 
    ''' (将指定<paramref name="index"/>下标的元素从原始数据<paramref name="source"/>序列之中提取出来)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    ''' <param name="index">所要获取的目标对象的下表的集合</param>
    ''' <param name="reversed">是否为反向选择，即返回所有不在目标index集合之中的元素列表</param>
    ''' <param name="OffSet">当进行反选的时候，本参数将不会起作用</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' ###### 2018-3-30 函数经过测试没有问题
    ''' </remarks>
    <ExportAPI("takes")>
    <Extension> Public Function Takes(Of T)(source As IEnumerable(Of T),
                                            index%(),
                                            Optional offSet% = 0,
                                            Optional reversed As Boolean = False) As T()
        If reversed Then
            Return source.__reversedTake(index).ToArray
        End If

        Dim result As T() = New T(index.Length - 1) {}
        Dim indices As Index(Of Integer) = index _
            .Select(Function(oi) oi + offSet) _
            .Indexing

        For Each x As SeqValue(Of T) In source.SeqIterator
            ' 在这里得到的是x的index在indexs参数之中的索引位置
            Dim i% = indices.IndexOf(x:=x.i)

            ' 当前的原始的下表位于indexs参数值中，则第i个indexs元素所指向的source的元素
            ' 就是x， 将其放入对应的结果列表之中
            If i > -1 Then
                result(i) = x.value
            End If
        Next

        Return result
    End Function

    ''' <summary>
    ''' 反选，即将所有不出现在<paramref name="index"></paramref>之中的元素都选取出来
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="collection"></param>
    ''' <param name="index"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <Extension>
    Private Iterator Function __reversedTake(Of T)(collection As IEnumerable(Of T), index As Integer()) As IEnumerable(Of T)
        Dim indices As New Index(Of Integer)(index)

        For Each x As SeqValue(Of T) In collection.SeqIterator
            ' 不存在于顶点的列表之中，即符合反选的条件，则添加进入结果之中
            If indices.IndexOf(x:=x.i) = -1 Then
                Yield x.value
            End If
        Next
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function Indexing(Of T)(source As IEnumerable(Of T)) As Index(Of T)
        Return New Index(Of T)(source)
    End Function

    <Extension>
    Public Sub Swap(Of T)(ByRef l As System.Collections.Generic.List(Of T), i%, j%)
        Dim tmp = l(i)
        l(i) = l(j)
        l(j) = tmp
    End Sub

    <Extension>
    Public Sub ForEach(Of T)(source As IEnumerable(Of T), action As Action(Of T, Integer))
        For Each x As SeqValue(Of T) In source.SeqIterator
            Call action(x.value, x.i)
        Next
    End Sub

    ''' <summary>
    ''' Initializes a new instance of the <see cref="List"/>`1 class that
    ''' contains elements copied from the specified collection and has sufficient capacity
    ''' to accommodate the number of elements copied.
    ''' </summary>
    ''' <param name="source">The collection whose elements are copied to the new list.</param>
    <Extension> Public Function ToList(Of T, TOut)(
                                  source As IEnumerable(Of T),
                                 [CType] As Func(Of T, TOut),
                       Optional parallel As Boolean = False) As List(Of TOut)

        If source Is Nothing Then
            Return New List(Of TOut)
        End If

        Dim result As List(Of TOut)

        If parallel Then
            result = (From x As T In source.AsParallel Select [CType](x)).AsList
        Else
            result = (From x As T In source Select [CType](x)).AsList
        End If

        Return result
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function AsLoop(Of T)(src As IEnumerable(Of T)) As LoopArray(Of T)
        Return New LoopArray(Of T)(src)
    End Function

    ''' <summary>
    ''' Initializes a new instance of the <see cref="List(Of T)"/> class that
    ''' contains elements copied from the specified collection and has sufficient capacity
    ''' to accommodate the number of elements copied.
    ''' </summary>
    ''' <param name="source">
    ''' The collection whose elements are copied to the new list.
    ''' </param>
    <Extension> Public Function AsList(Of T)(source As IEnumerable(Of T)) As List(Of T)
        ' 如果source集合是空值的话，不会抛错
        Return New List(Of T)(source)
    End Function

    ''' <summary>
    ''' Function name alias of the function <see cref="Hashtable.ContainsKey(Object)"/>
    ''' </summary>
    ''' <param name="hashtable"></param>
    ''' <param name="key"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function HasKey(hashtable As Hashtable, key As Object) As Boolean
        Return hashtable.ContainsKey(key)
    End Function

    ''' <summary>
    ''' Just using for the element index in a large collection
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="collection">
    ''' If the element in this collection have some duplicated member, then only the first element will be keeped.
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function AsHashSet(Of T)(collection As IEnumerable(Of T)) As Hashtable
        Dim table As New Hashtable

        For Each x As SeqValue(Of T) In collection.SeqIterator
            With x
                If Not table.ContainsKey(.value) Then
                    Call table.Add(.value, .i)
                End If
            End With
        Next

        Return table
    End Function

    ''' <summary>
    ''' <see cref="HashList(Of T)"/>
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function AsHashList(Of T As IAddressOf)(source As IEnumerable(Of T)) As HashList(Of T)
        Return New HashList(Of T)(source)
    End Function

    ''' <summary>
    ''' Initializes a new instance of the <see cref="List"/> class that
    ''' contains elements copied from the specified collection and has sufficient capacity
    ''' to accommodate the number of elements copied.
    ''' </summary>
    ''' <param name="linq">The collection whose elements are copied to the new list.</param>
    <Extension> Public Function ToList(Of T)(linq As ParallelQuery(Of T)) As List(Of T)
        Return New List(Of T)(linq)
    End Function
End Module
