Imports Microsoft.VisualBasic.Math.HashMaps.MinHash
Imports Microsoft.VisualBasic.Parallel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.Slicer

Public Class CDHashTask : Inherits VectorTask

    Friend ReadOnly seqPool As FastaSeq()
    Friend ReadOnly minHash As SequenceItem()
    Friend k As Integer

    Public Sub New(seqPool As FastaSeq(), Optional verbose As Boolean = False, Optional workers As Integer? = Nothing)
        MyBase.New(seqPool.Length, verbose, workers)

        Me.seqPool = seqPool
        Me.minHash = New SequenceItem(seqPool.Length - 1) {}
    End Sub

    Protected Overrides Sub Solve(start As Integer, ends As Integer, cpu_id As Integer)
        For i As Integer = start To ends
            ' MinHash.CreateSequenceData
            Dim s As FastaSeq = seqPool(i)
            Dim hash As SequenceItem = KSeq _
                .KmerSpans(s.SequenceData, k) _
                .CreateSequenceData(id:=i)

            minHash(i) = hash
        Next
    End Sub
End Class
