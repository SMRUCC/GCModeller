Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Unit
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.Slicer

Namespace Kmers

    Public Class KmerHashIndexFilter : Inherits KmerFilter

        ReadOnly hashFilter As HashSet(Of String)

        Sub New(k As Integer, name As IEnumerable(Of String), ncbi_taxid As Integer, filter As HashSet(Of String))
            Me.k = k
            Me.names = name.ToArray
            Me.ncbi_taxid = ncbi_taxid
            Me.hashFilter = filter
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

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <typeparam name="Fasta"></typeparam>
        ''' <param name="genomics">the genomics sequence data of a organism, single chromosome sequence data</param>
        ''' <param name="ncbi_taxid"></param>
        ''' <param name="k"></param>
        ''' <param name="spanSize"></param>
        ''' <returns></returns>
        Public Shared Function Create(Of Fasta As IFastaProvider)(genomics As Fasta,
                                                                  ncbi_taxid As Integer,
                                                                  Optional k As Integer = 35,
                                                                  Optional spanSize As Integer = 50 * ByteSize.MB) As KmerHashIndexFilter

            Dim estimatedKmers As Integer = Math.Max(0, Math.Min(spanSize, genomics.length - k + 1))
            Dim filter As New HashSet(Of String)
            Dim ntseq As String = genomics.GetSequenceData

            For i As Integer = 0 To ntseq.Length Step spanSize
                Dim len As Integer = spanSize

                If i + len > ntseq.Length Then
                    len = ntseq.Length - i
                End If

                For Each kmer As String In KSeq.KmerSpans(ntseq.Substring(i, len), k)
                    Call filter.Add(kmer)
                Next
            Next

            Return New KmerHashIndexFilter(k, {genomics.title}, ncbi_taxid, filter)
        End Function
    End Class
End Namespace