Imports Microsoft.VisualBasic.Data.NLP
Imports SMRUCC.genomics.SequenceModel

Public Class KmerTFIDFVectorizer

    ReadOnly vec As New TFIDF
    ReadOnly kmers As String()
    ReadOnly k As Integer

    Sub New(Optional type As SeqTypes = SeqTypes.Protein, Optional k As Integer = 6)
        Me.k = k
        Me.kmers = New KSeqCartesianProduct(type).KmerSeeds(k).ToArray
    End Sub

End Class
