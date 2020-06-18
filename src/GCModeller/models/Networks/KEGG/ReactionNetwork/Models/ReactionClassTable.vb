Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace ReactionNetwork

    Public Class ReactionClassTable

        Public Property rId As String
        Public Property category As String
        Public Property [from] As String
        Public Property [to] As String
        Public Property define As String

        Public Shared Function IndexTable(table As IEnumerable(Of ReactionClassTable)) As Index(Of String)
            Return table _
                .Select(Function(r) CreateIndexKey(r.from, r.to)) _
                .GroupBy(Function(key) key) _
                .Keys _
                .Indexing
        End Function

        Public Shared Function CreateIndexKey(a As String, b As String) As String
            Return {a, b}.OrderBy(Function(s) s).JoinBy("+")
        End Function

    End Class
End Namespace