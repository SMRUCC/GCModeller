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
        Dim minHash As New List(Of SequenceItem)

        For i As Integer = start To ends
            ' MinHash.CreateSequenceData
            Dim s As FastaSeq = seqPool(i)
            Dim hash As SequenceItem = KSeq _
                .KmerSpans(s.SequenceData, k) _
                .CreateSequenceData(id:=i)

            minHash.Add(hash)
        Next

        SyncLock Me.minHash
            Call Array.Copy(minHash.ToArray, Scan0, Me.minHash, start, length:=minHash.Count)
        End SyncLock
    End Sub
End Class
