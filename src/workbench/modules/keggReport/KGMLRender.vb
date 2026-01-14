Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts.Orthogonal
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.KGML

Public Class KGMLRender

    ReadOnly kgml As pathway
    ReadOnly graph As NetworkGraph

    ReadOnly entryIndex As New Dictionary(Of entry)

    Sub New(kgml As pathway)
        Me.entryIndex = kgml.entries.SafeQuery.ToDictionary
        Me.kgml = kgml
        Me.graph = GetNetwork(kgml)
    End Sub

    Public Shared Function GetNetwork(pathway As pathway) As NetworkGraph
        Dim g As New NetworkGraph

        For Each entry As entry In pathway.entries.SafeQuery
            Dim gr As graphics = entry.graphics
            Dim w As Double = gr.width
            Dim h As Double = gr.height
            Dim x As Double = gr.x
            Dim y As Double = gr.y

            Call g.CreateNode(entry.id, New NodeData With {
                .label = entry.name.JoinBy("; "),
                .mass = entry.name.Length,
                .origID = entry.id,
                .size = {w, h},
                .initialPostion = New FDGVector2(x, y),
                .Properties = New Dictionary(Of String, String) From {
                    {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, entry.type},
                    {"reaction", entry.reaction},
                    {"link", entry.link}
                }
            })
        Next

        For Each rel As relation In pathway.relations.SafeQuery
            Call g.CreateEdge(
                g.GetElementByID(rel.entry1),
                g.GetElementByID(rel.entry2),
                1,
                New EdgeData With {
                    .label = "relation",
                    .Properties = New Dictionary(Of String, String) From {
                        {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, rel.type}
                    }
                }
            )
        Next

        Dim reactionEntries As Dictionary(Of String, entry()) = pathway.entries _
            .SafeQuery _
            .Where(Function(e) Not e.reaction.StringEmpty) _
            .GroupBy(Function(e) e.reaction) _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return a.ToArray
                          End Function)

        For Each rxn As reaction In pathway.reactions.SafeQuery
            For Each entry As entry In reactionEntries.TryGetValue(rxn.name).SafeQuery
                Dim node As Node = g.GetElementByID(entry.id)

                For Each left As compound In rxn.substrates.SafeQuery
                    Call g.CreateEdge(g.GetElementByID(left.id), node, 1)
                Next
                For Each right As compound In rxn.products.SafeQuery
                    Call g.CreateEdge(node, g.GetElementByID(right.id), 1)
                Next
            Next
        Next

        Return g
    End Function

    Public Function Render(nodes As NodeRepresentation) As IGraphicsData
        Dim g As NetworkGraph = nodes.MakeSubNetwork(Me).DoLayout



    End Function

End Class
