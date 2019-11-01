Namespace PathwayProfile

    Public Class EnrichmentProfiles
        Public Property RankGroup As String
        Public Property pathway As String
        Public Property profile As Double
        Public Property pvalue As Double

        Public Overrides Function ToString() As String
            Return $"{pathway}: {pvalue.ToString("G3")}"
        End Function
    End Class

End Namespace