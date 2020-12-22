Imports Microsoft.VisualBasic.Data.GraphTheory.KdTree
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Namespace Layouts.EdgeBundling.Mingle

    Public Class GraphKdNode : Inherits Node

        Friend x, y, z, w As Double

        Sub New(v As Node)
            Me.ID = v.ID
            Me.label = v.label
            Me.data = New NodeData With {
                .size = v.data.size
            }
        End Sub

    End Class
End Namespace