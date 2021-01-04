﻿#Region "Microsoft.VisualBasic::781b185dab07cc8d30b406dfff03edd1, Microsoft.VisualBasic.Core\Extensions\Collection\CollectionValueGetter.vb"

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

    ' Module CollectionValueGetter
    ' 
    '     Function: [Get], ElementAtOrDefault, ElementAtOrNull, FirstNotEmpty, (+2 Overloads) GetItem
    '               GetValueOrNull, NotNull, (+3 Overloads) TryGetValue
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Serialization.JSON

Public Module CollectionValueGetter

    ''' <summary>
    ''' Returns the first not nothing object.
    ''' </summary>
    ''' <typeparam name="T">
    ''' Due to the reason of value type is always not nothing, so that this generic type constrain as Class reference type.
    ''' </typeparam>
    ''' <param name="args"></param>
    ''' <returns></returns>
    Public Function NotNull(Of T As Class)(ParamArray args As T()) As T
        If args.IsNullOrEmpty Then
            Return Nothing
        Else
            For Each x In args
                If Not x Is Nothing Then
                    Return x
                End If
            Next
        End If

        Return Nothing
    End Function

    ''' <summary>
    ''' Returns the first not null or empty string.
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    Public Function FirstNotEmpty(ParamArray args As String()) As String
        If args.IsNullOrEmpty Then
            Return ""
        Else
            For Each s As String In args
                If Not String.IsNullOrEmpty(s) Then
                    Return s
                End If
            Next
        End If

        Return ""
    End Function

    ''' <summary>
    ''' Safe get the specific index element from the target collection, is the index value invalid, then default value will be return.
    ''' (假若下标越界的话会返回默认值)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="array"></param>
    ''' <param name="index"></param>
    ''' <param name="[default]">Default value for invalid index is nothing.</param>
    ''' <returns></returns>
    <Extension> Public Function [Get](Of T)(array As IEnumerable(Of T), index As Integer, Optional [default] As T = Nothing) As T
        If array Is Nothing Then
            Return [default]
        End If

        If index < 0 OrElse index >= array.Count Then
            Return [default]
        End If

        Dim value As T = array(index)
        Return value
    End Function

    ''' <summary>
    ''' This is a safely method for gets the value in a array, if the index was outside of the boundary, then the default value will be return.
    ''' (假若下标越界的话会返回默认值)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="array"></param>
    ''' <param name="index"></param>
    ''' <param name="[default]">Default value for return when the array object is nothing or index outside of the boundary.</param>
    ''' <returns></returns>
    <Extension> Public Function ElementAtOrDefault(Of T)(array As T(), index As Integer, Optional [default] As T = Nothing) As T
        If array.IsNullOrEmpty Then
            Return [default]
        End If

        If index < 0 OrElse index >= array.Length Then
            Return [default]
        End If

        Dim value As T = array(index)
        Return value
    End Function

    <Extension>
    Public Function ElementAtOrNull(Of T)(array As T(), index As Integer) As T
        If array Is Nothing Then
            Return Nothing
        ElseIf index < 0 Then
            index = array.Length + index
        End If

        If index < 0 OrElse index >= array.Length Then
            Return Nothing
        Else
            Return array(index)
        End If
    End Function

    ''' <summary>
    ''' 这个是一个安全的方法，假若下标越界或者目标数据源为空的话，则会返回空值
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    ''' <param name="index"></param>
    ''' <returns></returns>
#If FRAMEWORD_CORE Then
    <ExportAPI("Get.Item")>
    <Extension> Public Function GetItem(Of T)(source As IEnumerable(Of T), index As Integer) As T
#Else
    <Extension> Public Function GetItem(Of T)(source As IEnumerable(Of T), index As Integer) As T
#End If
        If source Is Nothing Then
            Return Nothing
        Else
            Return source.ElementAtOrDefault(index)
        End If
    End Function

    <Extension>
    Public Function GetValueOrNull(Of K, V)(table As IDictionary(Of K, V), key As K) As V
        Dim refOut As V = Nothing
        Call table.TryGetValue(key, value:=refOut)
        Return refOut
    End Function

    ''' <summary>
    ''' 假若不存在目标键名，则返回空值，默认值为空值
    ''' </summary>
    ''' <typeparam name="TKey"></typeparam>
    ''' <typeparam name="TValue"></typeparam>
    ''' <param name="table"></param>
    ''' <param name="keys"></param>
    ''' <param name="[default]"></param>
    ''' <returns></returns>
    <Extension> Public Function TryGetValue(Of TKey, TValue)(table As Dictionary(Of TKey, TValue),
                                                             keys As TKey(),
                                                             Optional [default] As TValue = Nothing,
                                                             Optional mute As Boolean = False,
                                                             <CallerMemberName> Optional trace$ = Nothing) As TValue
        ' 表示空的，或者键名是空的，都意味着键名不存在与表之中
        ' 直接返回默认值
        If table Is Nothing Then
#If DEBUG Then
            Call PrintException("Hash_table is nothing!")
#End If
            Return [default]
        ElseIf keys.IsNullOrEmpty Then
#If DEBUG Then
            Call PrintException("Index key is nothing!")
#End If
            Return [default]
        Else
            For Each key As TKey In keys
                If table.ContainsKey(key) Then
                    Return table(key)
                End If
            Next

#If DEBUG Then
            If Not mute Then
                Call PrintException($"missing_index:={keys.Select(AddressOf Scripting.ToString).GetJson}!", trace)
            End If
#End If
            Return [default]
        End If
    End Function

    ''' <summary>
    ''' 假若不存在目标键名，则返回空值，默认值为空值
    ''' </summary>
    ''' <typeparam name="TKey"></typeparam>
    ''' <typeparam name="TValue"></typeparam>
    ''' <param name="table"></param>
    ''' <param name="index">这个函数会自动处理空键名的情况</param>
    ''' <param name="[default]"></param>
    ''' <returns></returns>
    ''' 
    <DebuggerStepThrough>
    <Extension>
    Public Function TryGetValue(Of TKey, TValue)(table As Dictionary(Of TKey, TValue),
                                                 index As TKey,
                                                 Optional [default] As TValue = Nothing,
                                                 Optional mute As Boolean = False,
                                                 <CallerMemberName>
                                                 Optional trace$ = Nothing) As TValue
        ' 表示空的，或者键名是空的，都意味着键名不存在与表之中
        ' 直接返回默认值
        If table Is Nothing Then
#If DEBUG Then
            Call PrintException("Hash_table is nothing!")
#End If
            Return [default]
        ElseIf index Is Nothing Then
#If DEBUG Then
            Call PrintException("Index key is nothing!")
#End If
            Return [default]
        ElseIf Not table.ContainsKey(index) Then
#If DEBUG Then
            If Not mute Then
                Call PrintException($"missing_index:={Scripting.ToString(index)}!", trace)
            End If
#End If
            Return [default]
        End If

        Return table(index)
    End Function

    <Extension> Public Function TryGetValue(Of TKey, TValue, TProp)(hash As Dictionary(Of TKey, TValue), Index As TKey, prop As String) As TProp
        If hash Is Nothing Then
            Return Nothing
        End If

        If Not hash.ContainsKey(Index) Then
            Return Nothing
        End If

        Dim obj As TValue = hash(Index)
        Dim propertyInfo As PropertyInfo = obj.GetType.GetProperty(prop)

        If propertyInfo Is Nothing Then
            Return Nothing
        End If

        Dim value As Object = propertyInfo.GetValue(obj, Nothing)
        Return DirectCast(value, TProp)
    End Function
End Module
