Namespace siRNAHit

    Public Class BlastnMapTable

        Public Property qseqid As String
        Public Property sseqid As String
        Public Property sstart As Integer
        Public Property send As Integer
        Public Property qstart As Integer
        Public Property qend As Integer
        Public Property sstrand As String
        Public Property qseq As String
        Public Property sseq As String
        Public Property length As Integer
        Public Property evalue As Double
        Public Property bitscore As Double

        Public Shared Function Parse() As BlastnMapTable

        End Function

    End Class
End Namespace