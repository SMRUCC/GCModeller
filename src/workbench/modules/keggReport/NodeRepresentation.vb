Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging.Driver
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.KGML

Public Class NodeRepresentation

    Public Property images As Dictionary(Of String, IGraphicsData)

    Public Const Representation As String = "representation"

    Public Function MakeSubNetwork(pathway As KGMLRender) As NetworkGraph
        Dim g As New NetworkGraph

        For Each id As String In images.Keys
            Dim entry As entry = pathway(id)
            Dim data As NodeData = pathway.graph.GetElementByID(entry.id).data.Clone

            data(Representation) = id
            g.CreateNode(entry.id, data)
        Next

        For Each edge As Edge In pathway.graph.graphEdges
            Dim u = pathway.graph.GetElementByID(edge.U.label)
            Dim v = pathway.graph.GetElementByID(edge.V.label)

            If u IsNot Nothing AndAlso v IsNot Nothing Then
                Call g.CreateEdge(u, v, edge.weight, edge.data.Clone)
            End If
        Next

        Return g
    End Function

End Class
