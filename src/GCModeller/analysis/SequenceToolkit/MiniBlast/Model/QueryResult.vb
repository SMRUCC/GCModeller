Imports System.Text.Json.Serialization

Namespace Model

    Public Class QueryResult

        <JsonPropertyName("id")>
        Public Property Id As String

        <JsonPropertyName("description")>
        Public Property Description As String

        <JsonPropertyName("length")>
        Public Property Length As Integer

        <JsonPropertyName("hits")>
        Public Property Hits As List(Of Hit)

    End Class


End Namespace