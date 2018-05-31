Public Class EnrichmentResult

    Public Property term As String
    Public Property geneIDs As String()
    Public Property score As Double
    Public Property pvalue As Double
    Public Property FDR As Double
    Public Property cluster As Integer
    Public Property enriched As String

    Public Overrides Function ToString() As String
        Return term
    End Function

End Class
