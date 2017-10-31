Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Emit.Delegates

''' <summary>
''' + 如果是复杂类型，会被序列化为``list()``类型，
''' + 如果是简单类型，并且为集合，则会被序列化为``data.frame()``
''' + 如果目标集合的元素为复杂类型，则会被序列化为``list()``类型，如果元素实现了<see cref="INamedValue"/>接口，则``list()``的``key``为<see cref="INamedValue.Key"/>反之为序列索引号
''' + 如果目标集合的元素为简单类型，则会被序列化为数组向量
''' </summary>
Public Module rda

    Public Function Push(obj As Object) As String
        Dim type As Type = obj.GetType
        Dim var$ = App.NextTempName

        If DataFramework.IsPrimitive(type) Then
            SyncLock R
                With R
                    .call = ""
                End With
            End SyncLock
        ElseIf type.ImplementInterface(GetType(IEnumerable)) Then

        Else



        End If

        Return var
    End Function
End Module
