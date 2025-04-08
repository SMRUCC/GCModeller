Imports Microsoft.VisualBasic.Data.GraphTheory.Network

Namespace Kmer

    Public Class KmerEdge : Implements IndexEdge

        Public Property U As Integer Implements IndexEdge.U
        Public Property V As Integer Implements IndexEdge.V
        Public Property NSize As Integer

        Public Overrides Function ToString() As String
            Return $"{U} -> {V} (n_size: {NSize})"
        End Function

    End Class
End Namespace