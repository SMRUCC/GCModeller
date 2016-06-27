Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Linq
Imports LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles

Namespace Assembly.MetaCyc.File

    ''' <summary>
    ''' 每一个数据文件里面的每一个对象的模型
    ''' </summary>
    Public Class ObjectModel : Inherits DynamicPropertyBase(Of String())
        Implements sIdEnumerable

        Public Property uid As String Implements sIdEnumerable.Identifier

        Const UNIQUE_ID As String = "UNIQUE-ID"

        Public Shared Function ModelParser(buf As String()) As ObjectModel
            Dim lastKey As String = ""
            Dim lastValue As String = ""
            Dim list As New List(Of KeyValuePair(Of String, String))

            For Each line As String In buf
                If line.First = "/"c Then
                    lastValue = lastValue & " " & line
                Else
                    Call list.Add(lastKey, lastValue)

                    Dim pos As Integer = InStr(line, " - ")
                    lastKey = Mid(line, 1, pos).Trim
                    lastValue = Mid(line, pos + 3).Trim
                End If
            Next

            Call list.Add(lastKey, lastValue)

            Dim hash = (From x In list
                        Select x
                        Group x By x.Key Into Group) _
                             .ToDictionary(Function(x) x.Key,
                                           Function(x) x.Group.ToArray(Function(v) v.Value))
            Return New ObjectModel With {
                .uid = hash.TryGetValue(UNIQUE_ID).FirstOrDefault,
                .Properties = hash
            }
        End Function

        Public Shared Function CreateDictionary(om As ObjectModel) As Slots.Object
            Return New Slots.Object(om.Properties)
        End Function
    End Class
End Namespace