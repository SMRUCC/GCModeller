
Public Class PangenomeCurveData

    Public Property GenomeCount As Integer
    Public Property TotalGenes As Integer
    Public Property CoreGenes As Integer

    Public Overrides Function ToString() As String
        Return $"({GenomeCount}, [total:{TotalGenes}, core:{CoreGenes}])"
    End Function

End Class