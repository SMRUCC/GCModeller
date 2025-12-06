Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Kmers

    Public Class Classifier : Implements IDisposable

        Dim kmers As DatabaseReader

        Sub New(kmers As DatabaseReader)
            Me.kmers = kmers
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns>
        ''' the ncbi taxonomy id that this reads data be classfied, zero value means no class
        ''' </returns>
        Public Function MakeClassify(reads As String) As SequenceHit
            ' parse input reads sequence as kmers
            ' and then get kmer hits from the database
            Dim kmerHits As KmerSeed() = KSeq.KmerSpans(reads, kmers.k) _
                .Select(Function(k) kmers.GetKmer(k)) _
                .ToArray
            Dim total As Integer = kmerHits.Length
            Dim scoreMap As New Dictionary(Of UInteger, Double)
            Dim hitsMap As New Dictionary(Of UInteger, Integer)
            Dim totalScore As Double = 0
            Dim w As Double

            For Each seed As KmerSeed In kmerHits
                If seed Is Nothing Then
                    Continue For
                Else
                    w = seed.weight
                    totalScore += w
                End If

                ' 一个k-mer可能对应多个来源，需要将权重分配给所有来源
                For Each src As KmerSource In seed.source
                    If scoreMap.ContainsKey(src.seqid) Then
                        scoreMap(src.seqid) += w
                        hitsMap(src.seqid) += 1
                    Else
                        scoreMap.Add(src.seqid, w)
                        hitsMap.Add(src.seqid, 1)
                    End If
                Next
            Next

            If scoreMap.Count = 0 Then
                Return SequenceHit.Unknown
            End If

            Dim topHitSeq = scoreMap.OrderByDescending(Function(a) a.Value).First
            Dim topdata = kmers.SequenceInfomation(topHitSeq.Key)

            If topdata Is Nothing Then
                Return SequenceHit.Unknown
            End If

            Dim ratio As Double = hitsMap(topHitSeq.Key) / total
            Dim identifies As Double = topHitSeq.Value / totalScore

            Return New SequenceHit(topdata) With {
                .identities = identifies,
                .total = totalScore,
                .score = topHitSeq.Value,
                .ratio = ratio
            }
        End Function

        Dim disposedValue As Boolean

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects)
                    Call kmers.Dispose()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
                ' TODO: set large fields to null
                disposedValue = True
            End If
        End Sub

        ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
        ' Protected Overrides Sub Finalize()
        '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        '     Dispose(disposing:=False)
        '     MyBase.Finalize()
        ' End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class
End Namespace