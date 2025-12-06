Namespace Kmers

    Public Interface DatabaseReader : Inherits IDisposable

        ''' <summary>
        ''' the char length of the k-mer span in this database
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property k As Integer

        Function GetKmer(kmer As String, Optional loadLocis As Boolean = False) As KmerSeed
        Function SequenceToTaxonomyId(seqid As UInteger) As UInteger
        Function SequenceInfomation(seqid As UInteger) As SequenceSource

    End Interface
End Namespace