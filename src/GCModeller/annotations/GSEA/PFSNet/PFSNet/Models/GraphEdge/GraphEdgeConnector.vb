Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Text

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

        <Extension>
        Public Sub SaveTabular(edges As IEnumerable(Of GraphEdge), file As Stream)
            Using writer As New StreamWriter(file, Encodings.UTF8WithoutBOM.CodePage) With {
                .NewLine = vbLf
            }
                For Each edge As GraphEdge In edges
                    Call writer.WriteLine(edge.ToString)
                Next
            End Using
        End Sub
    End Module
End Namespace