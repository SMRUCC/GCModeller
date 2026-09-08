Imports Microsoft.VisualBasic.Data.GraphTheory.Network

Namespace Chem

    Public Class Bond : Implements IndexEdge

        Public Property a As Integer Implements IndexEdge.U
        Public Property b As Integer Implements IndexEdge.V
        Public Property order As Integer

        Public Shared Widening Operator CType(tri As (Integer, Integer, Integer)) As Bond
            Return New Bond With {.a = tri.Item1, .b = tri.Item2, .order = tri.Item3}
        End Operator

    End Class
End Namespace