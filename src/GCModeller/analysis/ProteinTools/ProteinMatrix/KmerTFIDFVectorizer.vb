Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.NLP
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class KmerTFIDFVectorizer

    ReadOnly vec As New TFIDF
    ReadOnly kmers As String()
    ReadOnly k As Integer
    ReadOnly type As SeqTypes

    Sub New(Optional type As SeqTypes = SeqTypes.Protein, Optional k As Integer = 6)
        Me.k = k
        ' Me.kmers = New KSeqCartesianProduct(type).KmerSeeds(k).ToArray
        Me.type = type
    End Sub

    Public Sub Add(seq As FastaSeq)
        Call vec.Add(seq.Title, KSeq.KmerSpans(seq.SequenceData, k))
    End Sub

    Public Sub AddRange(seqs As IEnumerable(Of FastaSeq))
        For Each seq As FastaSeq In seqs
            Call Add(seq)
        Next
    End Sub

    Public Function TfidfVectorizer(Optional normalize As Boolean = False) As DataFrame
        ' Call vec.SetWords(kmers)
        Return vec.TfidfVectorizer(normalize)
    End Function

    ''' <summary>
    ''' n-gram One-hot(Bag-of-n-grams)
    ''' </summary>
    ''' <returns></returns>
    Public Function OneHotVectorizer() As DataFrame
        Return vec.OneHotVectorizer
    End Function

End Class
