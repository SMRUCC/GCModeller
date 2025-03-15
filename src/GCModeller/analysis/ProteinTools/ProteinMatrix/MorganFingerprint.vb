Imports Microsoft.VisualBasic.Data.GraphTheory.Analysis.MorganFingerprint
Imports Microsoft.VisualBasic.Math.HashMaps

Public Class MorganFingerprint : Inherits GraphMorganFingerprint(Of KmerNode, KmerEdge)

    Public Sub New(size As Integer)
        MyBase.New(size)
    End Sub

    Protected Overrides Function HashAtom(v As KmerNode) As Integer
        Return KMerGraph.HashKMer(v.Type)
    End Function

    Protected Overrides Function HashEdge(atoms() As KmerNode, e As KmerEdge, flip As Boolean) As ULong
        Dim hashcode As ULong

        If flip Then
            hashcode = HashMap.HashCodePair(atoms(e.U).Code, atoms(e.V).Code)
        Else
            hashcode = HashMap.HashCodePair(atoms(e.V).Code, atoms(e.U).Code)
        End If

        Return hashcode Xor CULng(e.NSzie)
    End Function
End Class
