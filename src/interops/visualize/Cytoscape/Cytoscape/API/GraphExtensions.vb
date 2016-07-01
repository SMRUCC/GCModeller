Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.AnalysisTools.DataVisualization.Interaction.Cytoscape.CytoscapeGraphView
Imports SMRUCC.genomics.AnalysisTools.DataVisualization.Interaction.Cytoscape.CytoscapeGraphView.XGMML
Imports Microsoft.VisualBasic.DataVisualization
Imports Microsoft.VisualBasic.DataVisualization.Network.Graph
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic
Imports System.Drawing

Namespace API

    Public Module GraphExtensions

        ''' <summary>
        ''' Creates the network graph model from the Cytoscape data model to generates the network layout or visualization 
        ''' </summary>
        ''' <param name="g"></param>
        ''' <returns></returns>
        <Extension>
        Public Function CreateGraph(g As Graph) As NetworkGraph
            Dim nodes As Network.Graph.Node() =
                LinqAPI.Exec(Of Network.Graph.Node) <= From n As XGMML.Node
                                                       In g.Nodes
                                                       Select n.__node()
            Dim nodeHash As New Dictionary(Of Network.Graph.Node)(nodes)
            Dim edges As Network.Graph.Edge() =
                LinqAPI.Exec(Of Network.Graph.Edge) <= From edge As XGMML.Edge
                                                       In g.Edges
                                                       Select edge.__edge(nodeHash)
            Dim net As New NetworkGraph() With {
                .nodes = New List(Of Network.Graph.Node)(nodes),
                .edges = New List(Of Network.Graph.Edge)(edges)
            }

            Return net
        End Function

        <Extension>
        Private Function __node(n As XGMML.Node) As Network.Graph.Node
            Dim data As New NodeData With {
                .Color = New SolidBrush(n.Graphics.FillColor),
                .radius = n.Graphics.radius
            }

            Return New Network.Graph.Node(n.id, data)
        End Function

        <Extension>
        Private Function __edge(edge As XGMML.Edge, nodeHash As Dictionary(Of Network.Graph.Node)) As Network.Graph.Edge
            Dim data As New EdgeData

            Return New Network.Graph.Edge(
                CStr(edge.Id),
                nodeHash(edge.source),
                nodeHash(edge.target),
                data)
        End Function
    End Module
End Namespace