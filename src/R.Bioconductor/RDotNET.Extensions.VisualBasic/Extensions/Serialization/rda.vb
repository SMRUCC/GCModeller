Imports System.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Emit.Delegates
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports RDotNET.Extensions.VisualBasic.API
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder

''' <summary>
''' + 如果是复杂类型，会被序列化为``list()``类型，
''' + 如果是简单类型，并且为集合，则会被序列化为``data.frame()``
''' + 如果目标集合的元素为复杂类型，则会被序列化为``list()``类型，如果元素实现了<see cref="INamedValue"/>接口，则``list()``的``key``为<see cref="INamedValue.Key"/>反之为序列索引号
''' + 如果目标集合的元素为简单类型，则会被序列化为数组向量
''' </summary>
Public Module rda

    ''' <summary>
    ''' Save Any .NET object as a *.rda data file. And then you can load the saved <paramref name="obj"/> 
    ''' by using ``data()`` or ``load()`` function in R language.
    ''' </summary>
    ''' <param name="obj"></param>
    ''' <param name="file$"></param>
    ''' <param name="name$"></param>
    ''' <returns></returns>
    Public Function save(obj As Object, file$, Optional name$ = ".save") As Boolean
        Dim var$ = rda.Push(obj)

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
    Public Function Push(obj As Object) As String
        Dim type As Type = obj.GetType

        If DataFramework.IsPrimitive(type) Then
            Dim var$ = App.NextTempName
            WriteMemoryInternal.WritePrimitive(var, obj)
            Return var
        ElseIf type.ImplementInterface(GetType(IEnumerable)) Then
            Return PushList(DirectCast(obj, IEnumerable))
        Else
            Return PushComplexObject(obj)
        End If
    End Function

    Public Function PushList(list As IEnumerable) As String
        Dim type As Type = CObj(list).GetType
        Dim base As Type = type.GetTypeElement(False)
        Dim var$ = App.NextTempName

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

        ' 是非基础类型，如果是简单的非基础类型，则写为一个data.frame
        ' 反之复杂的非基础类型写为一个list
        If DataFramework.IscomplexType(base) Then

            ' write as list
            ' 如果实现了INamedValue接口，则使用key属性作为键名
            ' 反之使用索引号

            If base.ImplementInterface(GetType(INamedValue)) Then
                For Each x As Object In list
                    Dim key$ = DirectCast(x, INamedValue).Key

                    SyncLock R
                        With R
                            .call = $"{var}[[{Rstring(key)}]] <- {PushComplexObject(x)};"
                        End With
                    End SyncLock
                Next
            Else
                For Each x As SeqValue(Of Object) In list.SeqIterator
                    Dim key$ = x.i + 1 ' R 之中的下标是从1开始的 

                    SyncLock R
                        With R
                            .call = $"{var}[[{key}]] <- {PushComplexObject(x)};"
                        End With
                    End SyncLock
                Next
            End If

        Else
            ' write as dataframe
            With App.GetAppSysTempFile(, App.PID)
                Call list.SaveTable(.ref, type:=base)
                Return utils.read.csv(.ref)
            End With
        End If

        Return var
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="obj">这个类型为单个的class或者structure类型</param>
    ''' <returns></returns>
    Public Function PushComplexObject(obj As Object) As String
        If obj Is Nothing Then
            With App.NextTempName
                Call WriteMemoryInternal.WriteNothing(.ref)
                Return .ref
            End With
        End If

        Dim schema = obj.GetType.Schema(PropertyAccess.Readable, , True)
        Dim var$ = App.NextTempName

        SyncLock R
            With R
                .call = $"{var} <- list();"

                For Each [property] As PropertyInfo In schema.Values
                    .call = $"{var}[[{Rstring([property].Name)}]] <- {Push([property].GetValue(obj))}"
                Next
            End With
        End SyncLock

        Return var
    End Function
End Module
