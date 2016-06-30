Imports Microsoft.VisualBasic.DataVisualization.Network.FileStream

Namespace DataVisualization

    Public Class NetModel : Inherits Network(Of NodeAttributes, Interactions)

        Sub New(nodes As IEnumerable(Of NodeAttributes), edges As IEnumerable(Of Interactions))
            Me.Nodes = nodes.ToArray
            Me.Edges = edges.ToArray
        End Sub

        Sub New(edges As IEnumerable(Of Interactions), nodes As IEnumerable(Of NodeAttributes))
            Me.Nodes = nodes.ToArray
            Me.Edges = edges.ToArray
        End Sub
    End Class
End Namespace