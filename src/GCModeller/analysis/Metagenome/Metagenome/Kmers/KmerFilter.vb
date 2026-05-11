Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.Slicer

Namespace Kmers

    Public MustInherit Class KmerFilter

        ''' <summary>
        ''' the genome name(multiple chromosome name)
        ''' </summary>
        Protected names As String()

        Dim _k As Integer
        Dim _ncbi_taxid As Integer

        ''' <summary>
        ''' the length of the k-mer
        ''' </summary>
        Public Property k As Integer
            Get
                Return _k
            End Get
            Protected Set(value As Integer)
                _k = value
            End Set
        End Property

        ''' <summary>
        ''' the genome taxonomy id
        ''' </summary>
        ''' <returns></returns>
        Public Property ncbi_taxid As Integer
            Get
                Return _ncbi_taxid
            End Get
            Protected Set(value As Integer)
                _ncbi_taxid = value
            End Set
        End Property

        Public Function KmerHits(seq As ISequenceProvider) As Dictionary(Of String, Integer)
            Return KmerHits(KSeq.KmerSpans(seq.GetSequenceData, k))
        End Function

        Public MustOverride Function KmerHitNumber(kmers As IEnumerable(Of String)) As Integer
        Public MustOverride Function KmerHits(kmers As IEnumerable(Of String)) As Dictionary(Of String, Integer)

        Public Overrides Function ToString() As String
            Return $"ncbi_taxid: {ncbi_taxid}; " & names(0)
        End Function

    End Class
End Namespace