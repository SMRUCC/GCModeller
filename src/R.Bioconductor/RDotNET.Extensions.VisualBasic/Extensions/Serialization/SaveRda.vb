#Region "Microsoft.VisualBasic::a7f57470c69ede2cd599975c23eebf89, RDotNET.Extensions.VisualBasic\Extensions\Serialization\SaveRda.vb"

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

'     Module SaveRda
' 
'         Function: Push, PushComplexObject, PushList, save
' 
' 
' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Emit.Delegates
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports RDotNET.Extensions.VisualBasic.API
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder
Imports Rbase = RDotNET.Extensions.VisualBasic.API.base

Namespace Serialization

    ''' <summary>
    ''' + 如果是复杂类型，会被序列化为``list()``类型，
    ''' + 如果是简单类型，并且为集合，则会被序列化为``data.frame()``
    ''' + 如果目标集合的元素为复杂类型，则会被序列化为``list()``类型，如果元素实现了<see cref="INamedValue"/>接口，则``list()``的``key``为<see cref="INamedValue.Key"/>反之为序列索引号
    ''' + 如果目标集合的元素为简单类型，则会被序列化为数组向量
    ''' </summary>
    Public Module SaveRda

        ''' <summary>
        ''' Save Any .NET object as a *.rda data file. And then you can load the saved <paramref name="obj"/> 
        ''' by using ``data()`` or ``load()`` function in R language.
        ''' </summary>
        ''' <param name="obj">Any .NET object.</param>
        ''' <param name="file">The rda file path.</param>
        ''' <param name="name">The dataset name, a R variable name. By default is named as ``.save``, an hidden object in R environment.</param>
        ''' <returns></returns>
        Public Function save(obj As Object, file$, Optional name$ = ".save", Optional encoding As Encoding = Nothing) As Boolean
            Dim var$ = SaveRda.Push(obj, encoding Or Encoding.ASCII.AsDefault)

            SyncLock R
                With R
                    .call = $"{name} <- {var};"
                    base.save({name}, file)
                End With
            End SyncLock

            Return True
        End Function

        ''' <summary>
        ''' Write Any .NET object into R memory
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        Public Function Push(obj As Object, encoding As Encoding) As String
            Dim type As Type = obj?.GetType

            If type Is Nothing Then
                Return "NULL"
            End If

            If DataFramework.IsPrimitive(type) Then
                Dim var$ = RDotNetGC.Allocate
                WriteMemoryInternal.WritePrimitive(var, obj)
                Return var
            ElseIf type.ImplementInterface(GetType(IEnumerable)) Then
                Return PushList(DirectCast(obj, IEnumerable), encoding)
            Else
                Return PushComplexObject(obj, encoding)
            End If
        End Function

        <Extension>
        Private Function isKeyValueTuple(properties As PropertyInfo()) As Boolean
            Return properties.Length = 2 AndAlso
                properties.Any(Function(p) p.Name.ToLower = "key" AndAlso p.PropertyType Is GetType(String) OrElse p.PropertyType Is GetType(Integer) OrElse p.PropertyType Is GetType(Long)) AndAlso
                properties.Any(Function(p) p.Name.ToLower = "value")
        End Function

        <Extension>
        Private Sub writeKeyValueTuples(list As IEnumerable, var$, key As PropertyInfo, value As PropertyInfo, encoding As Encoding)
            SyncLock R
                With R
                    For Each x As Object In list
                        Dim keyStr$ = Scripting.ToString(key.GetValue(x))
                        Dim valStr$ = SaveRda.Push(value.GetValue(x), encoding)

                        .call = $"{var}[[{Rstring(keyStr)}]] <- {valStr};"
                    Next
                End With
            End SyncLock
        End Sub

        Public Function PushList(list As IEnumerable, encoding As Encoding) As String
            If list Is Nothing Then
                Return "NULL"
            End If

            Dim type As Type = CObj(list).GetType
            Dim base As Type = type.GetTypeElement(False)
            Dim var$ = RDotNetGC.Allocate

            If DataFramework.IsPrimitive(base) Then
                ' 全部都是基础类型，则写为一个向量
                Call WriteMemoryInternal.WritePrimitive(var, list)
                Return var
            End If

            SyncLock R
                With R
                    .call = $"{var} <- list();"
                End With
            End SyncLock

            Static keyValTupleCache As New Dictionary(Of Type, (isTuple As Boolean, key As PropertyInfo, val As PropertyInfo))
            Static complexTypeCache As New Dictionary(Of Type, Boolean)

            If Not keyValTupleCache.ContainsKey(base) Then
                Dim properties = base.GetProperties(PublicProperty)

                If properties.isKeyValueTuple Then
                    Call keyValTupleCache.Add(base, (True, properties.First(Function(p) p.Name.ToLower = "key"), properties.First(Function(p) p.Name.ToLower = "value")))
                Else
                    Call keyValTupleCache.Add(base, (False, Nothing, Nothing))
                End If
            End If
            If Not complexTypeCache.ContainsKey(base) Then
                Call complexTypeCache.Add(base, DataFramework.IsComplexType(base, EnumCastTo.string))
            End If

            If keyValTupleCache(base).isTuple Then
                Call list.writeKeyValueTuples(var, keyValTupleCache(base).key, keyValTupleCache(base).val, encoding)
            ElseIf complexTypeCache(base) Then
                ' 是非基础类型，如果是简单的非基础类型，则写为一个data.frame
                ' 反之复杂的非基础类型写为一个list

                ' write as list
                ' 如果实现了INamedValue接口，则使用key属性作为键名
                ' 反之使用索引号

                If base.ImplementInterface(GetType(INamedValue)) Then
                    For Each x As Object In list
                        Dim key$ = DirectCast(x, INamedValue).Key

                        SyncLock R
                            With R
                                .call = $"{var}[[{Rstring(key)}]] <- {PushComplexObject(x, encoding)};"
                            End With
                        End SyncLock
                    Next
                Else
                    For Each x As SeqValue(Of Object) In list.SeqIterator
                        ' R 之中的下标是从1开始的 
                        Dim key$ = x.i + 1

                        SyncLock R
                            With R
                                .call = $"{var}[[{key}]] <- {PushComplexObject(x, encoding)};"
                            End With
                        End SyncLock
                    Next
                End If
            Else
                var = PushTable(list, encoding)
            End If

            Return var
        End Function

        Public Function PushTable(list As IEnumerable, encoding As Encoding) As String
            Dim type As Type = CObj(list).GetType
            Dim base As Type = type.GetTypeElement(False)

            ' write as dataframe
            With App.GetAppSysTempFile(, App.PID)
                Call .DoCall(Sub(file)
                                 Call list.SaveTable(
                                    path:=file,
                                    type:=base,
                                    encoding:=encoding,
                                    silent:=True
                                 )
                             End Sub)

                Return .DoCall(AddressOf utils.read.csv)
            End With
        End Function

        ''' <summary>
        ''' 主要是保存用户自定义类型,例如class或者structure这些非基元类型或者非数组类型等
        ''' </summary>
        ''' <param name="obj">这个类型为单个的class或者structure类型</param>
        ''' <param name="filters">可以通过这个参数来屏蔽不希望写入R内存的属性</param>
        ''' <returns></returns>
        Public Function PushComplexObject(obj As Object, encoding As Encoding, Optional filters As String() = Nothing) As String
            If obj Is Nothing Then
                With RDotNetGC.Allocate
                    Call WriteMemoryInternal.WriteNothing(.ByRef)
                    Return .ByRef
                End With
            End If

            SyncLock R
                With R
                    Dim schema = obj.GetType.Schema(PropertyAccess.Readable, , True)
                    Dim var$ = Rbase.list
                    Dim tmpname$
                    Dim ref$

                    If Not filters.IsNullOrEmpty Then
                        ' 删除指定的属性
                        For Each delete As String In filters
                            Call schema.Remove(key:=delete)
                        Next
                    End If

                    For Each [property] As PropertyInfo In schema.Values
                        tmpname = SaveRda.Push([property].GetValue(obj), encoding)
                        ref = Rstring([property].Name)

                        .call = $"{var}[[{ref}]] <- {tmpname}"
                    Next

                    Return var
                End With
            End SyncLock
        End Function
    End Module
End Namespace
