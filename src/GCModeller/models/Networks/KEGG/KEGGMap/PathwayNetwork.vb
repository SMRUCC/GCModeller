Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Public Module PathwayNetwork

    <Extension>
    Public Function BuildNetwork(ref As IEnumerable(Of Map), nodeValue As Action(Of Node)) As NetworkGraph
        Dim graph As New NetworkGraph
        Dim maps As Map() = ref.ToArray

        For Each map As Map In maps
            Call nodeValue(graph.CreateNode(map.ID))
        Next

        For Each A As Map In maps
            Dim compoundsA = A _
                .GetMembers _
                .Where(Function(id) id.IsPattern("C\d+")) _
                .ToArray

            For Each B In maps.Where(Function(bb) Not A Is bb)
                With B.GetMembers _
                    .Where(Function(id) id.IsPattern("C\d+")) _
                    .Intersect(compoundsA) _
                    .ToArray

                    If Not .IsNullOrEmpty Then
                        Dim edge As Edge = graph.CreateEdge(A.ID, B.ID)
                        edge.Weight = .Length
                    End If
                End With
            Next
        Next

        Return graph
    End Function
End Module
