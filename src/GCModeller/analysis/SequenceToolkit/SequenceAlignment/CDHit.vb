Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.HashMaps.MinHash
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class CDHit

    ReadOnly k As Integer = 31

    Sub New(Optional k As Integer = 31)
        Me.k = k
    End Sub

    Public Iterator Function FindSimilar(seqs As IEnumerable(Of FastaSeq)) As IEnumerable(Of SimilarHit)
        Dim seqPool = seqs.ToArray
        Dim seqHash = seqPool.SeqIterator.ToArray _
            .AsParallel _
            .Select(Function(s)
                        Return KSeq _
                            .KmerSpans(s.value.SequenceData, k) _
                            .CreateSequenceData(id:=s.i)
                    End Function) _
            .ToArray
        Dim similars As New Dictionary(Of Integer, SimilarHit)

        For Each result As SimilarityIndex In LSH.FindSimilarItems(seqHash, produceUniqueHit:=True)
            If result.IsUniqueHit Then
                Yield New SimilarHit With {.SeqID = seqPool(result.U).Title}
            Else
                If Not similars.ContainsKey(result.U) Then
                    Call similars.Add(result.U, New SimilarHit With {.SeqID = seqPool(result.U).Title})
                End If

                Call similars(result.U).Similar.Add(seqPool(result.V).Title, result.Similarity)
            End If
        Next

        For Each similar As SimilarHit In similars.Values
            Yield similar
        Next
    End Function

End Class

Public Class SimilarHit

    Public Property SeqID As String
    Public Property Similar As Dictionary(Of String, Double)

    Public ReadOnly Property IsUniqued As Boolean
        Get
            Return Similar.IsNullOrEmpty
        End Get
    End Property

End Class
