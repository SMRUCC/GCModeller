Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports SMRUCC.genomics.Model.PathVisio.GPML

Public Module GraphBuilder

    <Extension>
    Public Function CreateGraph(pathway As Pathway) As NetworkGraph
        Dim g As New NetworkGraph
        Dim nodeData As NodeData
        Dim linkData As EdgeData

        For Each node As DataNode In pathway.DataNode
            nodeData = New NodeData With {
                .label = node.TextLabel,
                .origID = node.GraphId,
                .Properties = New Dictionary(Of String, String) From {
                    {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, node.Type.ToString},
                    {"database", node.Xref.Database},
                    {"xref", node.Xref.ID}
                },
                .initialPostion = New FDGVector2(node.Graphics.CenterX, node.Graphics.CenterY)
            }

            Call g.CreateNode(node.GraphId, nodeData)
        Next

        For Each link As Interaction In pathway.Interaction
            Dim u As Point = link.Graphics.Points(0)
            Dim v As Point = link.Graphics.Points(1)

            linkData = New EdgeData With {
                .label = link.GraphId,
                .Properties = New Dictionary(Of String, String) From {
                    {"database", link.Xref.Database},
                    {"xref", link.Xref.ID}
                }
            }

            Call g.CreateEdge(u.GraphRef, v.GraphRef, data:=linkData)
        Next

        Return g
    End Function
End Module
