Imports System.Text.Json.Serialization

Namespace Model

    Public Class Hsp

        <JsonPropertyName("score")>
        Public Property Score As Double

        <JsonPropertyName("bit_score")>
        Public Property BitScore As Double

        <JsonPropertyName("evalue")>
        Public Property Evalue As Double

        <JsonPropertyName("identities")>
        Public Property Identities As Integer

        <JsonPropertyName("positives")>
        Public Property Positives As Integer

        <JsonPropertyName("gaps")>
        Public Property Gaps As Integer

        <JsonPropertyName("query_from")>
        Public Property QueryFrom As Integer

        <JsonPropertyName("query_to")>
        Public Property QueryTo As Integer

        <JsonPropertyName("subject_from")>
        Public Property SubjectFrom As Integer

        <JsonPropertyName("subject_to")>
        Public Property SubjectTo As Integer

        <JsonPropertyName("query_frame")>
        Public Property QueryFrame As Integer

        <JsonPropertyName("query_seq")>
        Public Property QuerySeq As String

        <JsonPropertyName("midline")>
        Public Property Midline As String

        <JsonPropertyName("subject_seq")>
        Public Property SubjectSeq As String

    End Class

End Namespace