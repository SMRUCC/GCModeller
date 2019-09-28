Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core

Public Module Visualizer

    <Extension>
    Public Function ToGraph(cell As Vessel) As NetworkGraph
        Dim g As New NetworkGraph
        Dim node As Node

        For Each mass As Node In cell.Mass _
            .Select(Function(m)
                        Return New Node With {
                            .label = m.ID,
                            .data = New NodeData With {
                                .mass = m.Value,
                                .origID = m.ID,
                                .label = m.ID,
                                .radius = m.Value,
                                .Properties = New Dictionary(Of String, String) From {
                                    {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, "metabolite"}
                                }
                            }
                        }
                    End Function)

            Call g.AddNode(mass)
        Next

        For Each reaction As Channel In cell.Channels
            node = New Node With {
                .label = reaction.ID,
                .data = New NodeData With {
                    .label = reaction.ID,
                    .origID = reaction.ID,
                    .mass = reaction.Direction,
                    .Properties = New Dictionary(Of String, String) From {
                        {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, "reaction"}
                    }
                }
            }

            Call g.AddNode(node)
        Next

        For Each reaction As Channel In cell.Channels
            For Each left As Variable In reaction.GetReactants
                Call g.AddEdge(left.Mass.ID, reaction.ID, left.Coefficient)
            Next

            For Each right As Variable In reaction.GetProducts
                Call g.AddEdge(right.Mass.ID, reaction.ID, right.Coefficient)
            Next
        Next

        Return g
    End Function
End Module
