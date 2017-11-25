Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Emit.Delegates
Imports Microsoft.VisualBasic.Language

Module ReflectionExtensions

    Public Function IsVariableType(obj As Object) As Boolean
        With obj.GetType
            If .ref Is GetType(String) Then
                Return True
            ElseIf Not .ImplementInterface(GetType(IEnumerable)) Then
                Return True
            Else
                Return False
            End If
        End With
    End Function

    Public Function IsCollectionType(obj As Object) As Boolean
        With obj.GetType
            If Not .ref Is GetType(String) AndAlso .ImplementInterface(GetType(IEnumerable)) Then
                Return True
            Else
                Return False
            End If
        End With
    End Function

    Private Function FlatObject(key$, obj As Object) As Object
        If obj.GetType Is GetType(String) Then
            Return obj
        Else
            Return obj _
                .GetType _
                .Schema(PropertyAccess.Readable, PublicProperty, True) _
                .GetVariables(key, obj)
        End If
    End Function

    <Extension>
    Public Function CreateVariables(values As IEnumerable(Of KeyValuePair(Of String, Object))) As Dictionary(Of String, String)
        Dim table As New Dictionary(Of String, String)
        Dim value$

        For Each var In values
            With var
                If .Value.GetType Is GetType(String) Then
                    value = DirectCast(.Value, String)
                    table(.Key) = value
                Else
                    Dim vars = .Value _
                        .GetType _
                        .Schema(PropertyAccess.Readable, PublicProperty, True) _
                        .GetVariables(.Key, .Value)

                    For Each tuple As NamedValue(Of String) In vars
                        Call table.Add(tuple.Name, tuple.Value)
                    Next
                End If
            End With
        Next

        Return table
    End Function
End Module
