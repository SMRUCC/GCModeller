Namespace SymbolBuilder

    Public Class SafeTable

        ReadOnly internal As Dictionary(Of String, String)

        Default Public ReadOnly Property Item(key As String) As String
            Get
                If internal.ContainsKey(key) Then
                    Return internal(key)
                Else
                    Return "NA"
                End If
            End Get
        End Property

        Sub New(metadata As IDictionary(Of String, String))
            internal = New Dictionary(Of String, String)(metadata)
        End Sub
    End Class
End Namespace