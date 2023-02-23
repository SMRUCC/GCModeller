Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports g = Microsoft.VisualBasic.Data.visualize.Network.Graph.NetworkGraph

Namespace Metabolism.Metpa

    Public Class graph

        Public Property nodes As Node()
        Public Property edges As NetworkEdge()
        Public Property name As String

        Sub New()
        End Sub

        ''' <summary>
        ''' create a network graph model from the table data
        ''' </summary>
        ''' <returns></returns>
        Public Function Create() As g
            Dim network As New NetworkTables With {
                .edges = edges,
                .nodes = nodes
            }
            Dim g As g = network.CreateGraph

            Return g
        End Function

        Public Shared Function Create(g As g, name As String) As graph
            Dim table As NetworkTables = g.Tabular({"*"}, meta:=New MetaData)
            Dim graph As New graph With {
                .edges = table.edges,
                .nodes = table.nodes,
                .name = name
            }

            Return graph
        End Function

    End Class

    Public Class graphList

        Public Property graphs As Dictionary(Of String, graph)

    End Class
End Namespace