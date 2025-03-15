Imports Microsoft.VisualBasic.Data.GraphTheory.Analysis.MorganFingerprint
Imports Microsoft.VisualBasic.Data.GraphTheory.Network
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.HashMaps
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class KMerGraph : Implements MorganGraph(Of KmerNode, KmerEdge)

    Public ReadOnly Property KMers As KmerNode() Implements MorganGraph(Of KmerNode, KmerEdge).Atoms
    Public ReadOnly Property Graph As KmerEdge() Implements MorganGraph(Of KmerNode, KmerEdge).Graph

    Private Sub New(kmers As IEnumerable(Of KmerNode), graph As IEnumerable(Of KmerEdge))
        _KMers = kmers.ToArray
        _Graph = graph.ToArray
    End Sub

    Public Shared Function FromSequence(seq As ISequenceProvider, Optional k As Integer = 3) As KMerGraph
        Dim kmers As New List(Of KmerNode)
        Dim koffset As New Dictionary(Of ULong, Integer)
        Dim u As ULong? = Nothing
        Dim graph As New Dictionary(Of ULong, Dictionary(Of ULong, KmerEdge))

        For Each kmer As KSeq In KSeq.Kmers(seq, k)
            Dim key As ULong = HashKMer(kmer)

            If Not koffset.ContainsKey(key) Then
                Call koffset.Add(key, kmers.Count)
                Call kmers.Add(New KmerNode With {
                    .Code = key,
                    .Index = kmers.Count,
                    .Type = New String(kmer.Seq)
                })
            End If

            If u Is Nothing Then
                u = key
            Else
                If Not graph.ContainsKey(u.Value) Then
                    Call graph.Add(u.Value, New Dictionary(Of ULong, KmerEdge))
                End If
                If Not graph(u.Value).ContainsKey(key) Then
                    Call graph(u.Value).Add(key, New KmerEdge With {
                        .U = koffset(u.Value),
                        .V = koffset(key),
                        .NSzie = 0
                    })
                End If

                graph(u.Value)(key).NSzie += 1
                u = key
            End If
        Next

        Return New KMerGraph(kmers, graph.Values.Select(Function(a) a.Values).IteratesALL)
    End Function

    Public Shared Function HashKMer(kmer As KSeq) As ULong
        Dim hashcode As ULong = 0

        For Each c As Char In kmer.Seq
            hashcode = HashMap.HashCodePair(hashcode, CULng(Asc(c)))
        Next

        Return hashcode
    End Function

End Class

Public Class KmerNode : Implements IMorganAtom

    Public Property Index As Integer Implements IMorganAtom.Index
    Public Property Code As ULong Implements IMorganAtom.Code
    Public Property Type As String Implements IMorganAtom.Type

End Class

Public Class KmerEdge : Implements IndexEdge

    Public Property U As Integer Implements IndexEdge.U
    Public Property V As Integer Implements IndexEdge.V
    Public Property NSzie As Integer

End Class