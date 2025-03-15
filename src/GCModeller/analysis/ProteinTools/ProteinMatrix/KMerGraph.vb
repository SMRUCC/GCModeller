Imports Microsoft.VisualBasic.Data.GraphTheory.Analysis.MorganFingerprint
Imports Microsoft.VisualBasic.Data.GraphTheory.Network
Imports Microsoft.VisualBasic.Math.HashMaps
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class KMerGraph : Implements MorganGraph(Of KmerNode, KmerEdge)

    Public Property KMers As KmerNode() Implements MorganGraph(Of KmerNode, KmerEdge).Atoms
    Public ReadOnly Property Graph As KmerEdge() Implements MorganGraph(Of KmerNode, KmerEdge).Graph

    Public Shared Function FromSequence(seq As ISequenceProvider) As KMerGraph

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

End Class