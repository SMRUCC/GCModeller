Imports Microsoft.VisualBasic.Linq

Namespace Session

    Public Class cyTables : Implements Enumeration(Of virtualColumn)

        Public Property virtualColumns As virtualColumn()

        Public Iterator Function GenericEnumerator() As IEnumerator(Of virtualColumn) Implements Enumeration(Of virtualColumn).GenericEnumerator
            For Each item In virtualColumns
                Yield item
            Next
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator Implements Enumeration(Of virtualColumn).GetEnumerator
            Yield GenericEnumerator()
        End Function
    End Class

    Public Class virtualColumn

        Public Property name As String
        Public Property sourceColumn As String
        Public Property sourceTable As String
        Public Property sourceJoinKey As String
        Public Property targetTable As String
        Public Property targetJoinKey As String
        Public Property immutable As Boolean

    End Class
End Namespace