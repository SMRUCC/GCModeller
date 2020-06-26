Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Namespace DataStructure

    Public Module GraphEdgeConnector

        <Extension>
        Public Iterator Function FromMetabolismNetwork(g As NetworkGraph, Optional pathwayId As String = "pathwayId") As IEnumerable(Of GraphEdge)
            For Each edge As Edge In g.graphEdges
                Yield New GraphEdge With {
                    .g1 = edge.U.label,
                    .g2 = edge.V.label,
                    .pathwayID = edge.data(pathwayId)
                }
            Next
        End Function
    End Module
End Namespace