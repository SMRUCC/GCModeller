Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Data.GraphTheory.Network
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Public Class CausalModel : Implements IndexGraph(Of Path)

    Public Class Path : Implements IndexEdge

        Public Property U As Integer Implements IndexEdge.U
        Public Property V As Integer Implements IndexEdge.V

    End Class

    Public Property data As Double(,)
    Public Property varNames As String() Implements IndexGraph(Of Path).Nodes
    Public Property paths As Path() Implements IndexGraph(Of Path).Edges

    Friend Iterator Function AsPathTuple() As IEnumerable(Of (from As Integer, [to] As Integer))
        For Each node As Path In paths.SafeQuery
            Yield (node.U, node.V)
        Next
    End Function

    Public Shared Function Create(Of T As SparseGraph.IInteraction)(x As Matrix, paths As IEnumerable(Of T)) As CausalModel

    End Function

End Class
