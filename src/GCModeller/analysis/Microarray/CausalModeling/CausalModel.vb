Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Data.GraphTheory.Network
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Public Class CausalModel : Implements IndexGraph(Of Path)

    Public Class Path : Implements IndexEdge

        Public Property U As Integer Implements IndexEdge.U
        Public Property V As Integer Implements IndexEdge.V

        Sub New()
        End Sub

        Friend Sub New(i As Integer, j As Integer)
            U = i
            V = j
        End Sub

    End Class

    Public Property data As Double(,)
    Public Property varNames As String() Implements IndexGraph(Of Path).Nodes
    Public Property paths As Path() Implements IndexGraph(Of Path).Edges
    Public Property sampleIds As String()

    Friend Iterator Function AsPathTuple() As IEnumerable(Of (from As Integer, [to] As Integer))
        For Each node As Path In paths.SafeQuery
            Yield (node.U, node.V)
        Next
    End Function

    Public Shared Function Create(Of T As SparseGraph.IInteraction)(x As Matrix, paths As IEnumerable(Of T)) As CausalModel
        Dim tensor As Double(,) = x.AsTensorArray
        Dim model As CausalModel = IndexGraphExtensions.FromNetwork(Of T, Path, CausalModel)(paths, Function(i, j) New Path(i, j))
        model.data = tensor
        Return model
    End Function

    Public Function MakeDataFrame() As DataFrame
        Dim df As New DataFrame With {
            .rownames = sampleIds,
            .features = New Dictionary(Of String, FeatureVector)
        }
        Dim n As Integer = data.GetLength(0)
        Dim p As Integer = data.GetLength(1)

        For j As Integer = 0 To p - 1
            Dim col(n - 1) As Double

            For i As Integer = 0 To n - 1
                col(i) = data(i, j)
            Next

            Call df.add(varNames(j), col)
        Next

        Return df
    End Function

End Class
