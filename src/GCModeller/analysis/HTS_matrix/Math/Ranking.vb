Public Class Ranking

    Public Property geneID As String
    Public Property ranking As Dictionary(Of String, Double)
    Public Property pvalue As Dictionary(Of String, Double)
    Public Property expression As Dictionary(Of String, Double)

    Public Overrides Function ToString() As String
        Return geneID
    End Function

End Class