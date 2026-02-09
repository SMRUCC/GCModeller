Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.NLP
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Public Class KmerTFIDFVectorizer

    ReadOnly vec As New TFIDF
    ReadOnly k As Integer
    ReadOnly type As SeqTypes

    Sub New(Optional type As SeqTypes = SeqTypes.Protein, Optional k As Integer = 6)
        Me.k = k
        Me.type = type
    End Sub

    Public Sub Add(seq As FastaSeq)
        Dim chars As String = seq.SequenceData

        If type <> SeqTypes.Protein Then
            chars = NucleicAcid.Canonical(chars)
        End If

        Call vec.Add(seq.Title, KSeq.KmerSpans(chars, k))
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
