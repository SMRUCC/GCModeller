Namespace Kmers

    Public Class KmerHashIndexFilter : Inherits KmerFilter

        ReadOnly hashFilter As HashSet(Of String)

        Sub New(k As Integer, name As IEnumerable(Of String), ncbi_taxid As Integer)
            Me.k = k
            Me.names = name.ToArray
            Me.ncbi_taxid = ncbi_taxid
        End Sub

        Public Overrides Function KmerHitNumber(kmers As IEnumerable(Of String)) As Integer
            Dim hits As Integer = 0

            For Each kmer As String In kmers
                If hashFilter.Contains(kmer) Then
                    hits += 1
                End If
            Next

            Return hits
        End Function

        Public Overrides Function KmerHits(kmers As IEnumerable(Of String)) As Dictionary(Of String, Integer)
            Dim hits As New Dictionary(Of String, Integer)

            For Each kmer As String In kmers
                If hashFilter.Contains(kmer) Then
                    If Not hits.ContainsKey(kmer) Then
                        hits.Add(kmer, 1)
                    Else
                        hits(kmer) += 1
                    End If
                End If
            Next

            Return hits
        End Function
    End Class
End Namespace