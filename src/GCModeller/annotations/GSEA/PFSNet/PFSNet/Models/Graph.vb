#Region "Microsoft.VisualBasic::25656d43254ea18ed34d79f4cac0b2b2, GCModeller\annotations\GSEA\PFSNet\PFSNet\Models\Graph.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 117
    '    Code Lines: 79
    ' Comment Lines: 27
    '   Blank Lines: 11
    '     File Size: 6.01 KB


    '     Module Data
    ' 
    '         Function: decompose_graph, Frame, simplify
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Linq.JoinExtensions
Imports SMRUCC.genomics.Analysis.PFSNet.DataStructure

Namespace R.Graph

    Module Data

        ''' <summary>
        ''' This function creates an igraph graph from one or two data frames containing the (symbolic) edge list and edge/vertex attributes.
        ''' </summary>
        ''' <param name="d">A data frame containing a symbolic edge list in the first two columns. Additional columns are considered as edge attributes.</param>
        ''' <param name="vertices">
        ''' If vertices is NULL, then the first two columns of d are used as a symbolic edge list and additional columns as edge attributes. The names of the attributes are taken from the names of the columns.
        ''' If vertices is not NULL, then it must be a data frame giving vertex metadata. The first column of vertices is assumed to contain symbolic vertex names, this will be added to the graphs as the ‘name’ vertex attribute. Other columns will be added as additional vertex attributes. If vertices is not NULL then the symbolic edge list given in d is checked to contain only vertex names listed in vertices.
        ''' Typically, the data frames are exported from some speadsheat software like Excel and are imported into R via read.table, read.delim or read.csv.
        ''' </param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Frame(d As GraphEdge(), Optional directed As Boolean = True, Optional vertices As DataFrame = Nothing) As PFSNetGraph
            Dim allsId As String() = (From edge As GraphEdge
                                      In d
                                      Select {edge.g1, edge.g2}).IteratesALL.Distinct.ToArray
            Dim nodes = (From id As String In allsId
                         Let node As PFSNetGraphNode = New PFSNetGraphNode With {
                             .name = id
                         }
                         Select node).ToArray
            Return New PFSNetGraph With {
                .masked = True,
                .edges = d,
                .nodes = nodes
            }
        End Function

        ''' <summary>
        ''' simplify(graph, remove.multiple = TRUE, remove.loops = TRUE,
        ''' edge.attr.comb = getIgraphOpt("edge.attr.comb"))
        ''' is.simple(graph)
        ''' 
        ''' Simple graphs are graphs which do not contain loop and multiple edges.
        ''' 
        ''' A loop edge is an edge for which the two endpoints are the same vertex. Two edges are multiple edges if they have exactly the same two endpoints (for directed graphs order does matter). A graph is simple is it does not contain loop edges and multiple edges.
        ''' 
        ''' is.simple checks whether a graph is simple.
        ''' 
        ''' simplify removes the loop and/or multiple edges from a graph. If both remove.loops and remove.multiple are TRUE the function returns a simple graph.
        ''' </summary>
        ''' <param name="graph"></param>
        ''' <returns>A new graph object with the edges deleted.</returns>
        ''' <remarks></remarks>
        Public Function simplify(graph As PFSNetGraph) As PFSNetGraph
            Dim edges As GraphEdge() = (From edge As GraphEdge
                                        In graph.edges
                                        Where Not edge.isSelfLoop
                                        Select edge).ToArray
            Return New PFSNetGraph With {
                .nodes = graph.nodes,
                .edges = edges,
                .masked = graph.masked,
                .Id = graph.edges.First.pathwayID
            }
        End Function

        Public Iterator Function decompose_graph(g As PFSNetGraph, min_vertices As Integer) As IEnumerable(Of PFSNetGraph)
            Dim graph As New NetworkGraph

            For Each node As PFSNetGraphNode In g.nodes
                graph.CreateNode(node.name)
                graph.GetElementByID(node.name).data.weights = {
                    node.weight,
                    node.weight2
                }
            Next

            For Each ggi As GraphEdge In g.edges
                Call graph.CreateEdge(ggi.g1, ggi.g2, data:=New EdgeData With {.label = ggi.pathwayID})
            Next

            Dim subnetworks = graph.DecomposeGraph(minVertices:=min_vertices).ToArray
            Dim pfsnet As PFSNetGraph

            For Each part In subnetworks
                pfsnet = New PFSNetGraph With {
                    .Id = g.Id,
                    .masked = g.masked,
                    .pvalue = 1,
                    .statistics = 0,
                    .edges = part.graphEdges _
                        .Select(Function(a)
                                    Return New GraphEdge With {
                                        .g1 = a.U.label,
                                        .g2 = a.V.label,
                                        .pathwayID = a.data.label
                                    }
                                End Function) _
                        .ToArray,
                    .nodes = part.vertex _
                        .Select(Function(a)
                                    Return New PFSNetGraphNode With {
                                        .name = a.label,
                                        .weight = a.data.weights(Scan0),
                                        .weight2 = a.data.weights(1)
                                    }
                                End Function) _
                        .ToArray
                }

                Yield pfsnet
            Next
        End Function

    End Module
End Namespace
