Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Public Module ReactionNetwork

    <Extension>
    Public Function BuildModel(br08901 As IEnumerable(Of ReactionTable), compounds As IEnumerable(Of NamedValue(Of String))) As NetworkTables
        Dim nodes As Dictionary(Of Node) = compounds _
            .Select(Function(cpd)
                        Return New Node With {
                            .ID = cpd.Name,
                            .NodeType = "KEGG Compound",
                            .Properties = New Dictionary(Of String, String) From {
                                {"name", cpd.Value}
                            }
                        }
                    End Function) _
            .ToDictionary
        Dim edges As New Dictionary(Of String, NetworkEdge)
        Dim cpdGroups = br08901 _
            .Select(Function(x)
                        Return x.substrates _
                            .JoinIterates(x.products) _
                            .Select(Function(id) (id, x))
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(x) x.Item1) _
            .ToDictionary(Function(x) x.Key,
                          Function(reactions)
                              Return reactions _
                                  .Select(Function(x) x.Item2.entry) _
                                  .Distinct _
                                  .ToArray
                          End Function)
        Dim commons As Value(Of String()) = {}

        For Each a As Node In nodes.Values
            Dim reactionA = cpdGroups.TryGetValue(a.ID)

            If reactionA.IsNullOrEmpty Then
                Continue For
            End If

            For Each b As Node In nodes.Values.Where(Function(x) x.ID <> a.ID)
                Dim rB = cpdGroups.TryGetValue(b.ID)

                If rB.IsNullOrEmpty Then
                    Continue For
                End If

                If Not (commons = reactionA.Intersect(rB)).IsNullOrEmpty Then
                    Dim edge As New NetworkEdge With {
                        .FromNode = a.ID,
                        .ToNode = b.ID,
                        .value = commons.value.Length,
                        .Interaction = commons.value.JoinBy("|")
                    }

                    With edge.GetNullDirectedGuid(True)
                        If Not edges.ContainsKey(.ref) Then
                            Call edges.Add(.ref, edge)
                        End If
                    End With
                End If
            Next
        Next

        Return New NetworkTables(nodes.Values, edges.Values)
    End Function
End Module
