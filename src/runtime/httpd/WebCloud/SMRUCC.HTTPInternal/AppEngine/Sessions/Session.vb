Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace AppEngine.Sessions

    Public Class Session : Implements INamedValue

        Public Property ID As String Implements INamedValue.Key
        Public Property Table As Dictionary(Of String, Value)

        Default Public Property Item(name As String) As Value
            Get
                Return Table.TryGetValue(name)
            End Get
            Set(value As Value)
                Table(name) = value
            End Set
        End Property

        Public Sub SetValue(key$, value$)
            Item(key) = value
        End Sub

        Public Overrides Function ToString() As String
            Return $"[{ID} => {Table.Keys.ToArray.GetJson}]"
        End Function
    End Class

    Public Class Value

        Public Property Value As String
        Public Property Table As Dictionary(Of String, Value)

        Public Overrides Function ToString() As String
            If Table.IsNullOrEmpty Then
                Return Value
            Else
                Return Table.GetJson
            End If
        End Function

        Public Shared Widening Operator CType(value As String) As Value
            Return New Value With {
                .Value = value,
                .Table = New Dictionary(Of String, Value)
            }
        End Operator
    End Class
End Namespace