Imports System.Text.Json.Serialization

Namespace Model

    Public Class Hit

        <JsonPropertyName("id")>
        Public Property Id As String

        <JsonPropertyName("description")>
        Public Property Description As String

        <JsonPropertyName("length")>
        Public Property Length As Integer

        <JsonPropertyName("hsps")>
        Public Property Hsps As List(Of Hsp)

    End Class
End Namespace