#Region "Microsoft.VisualBasic::8cda05b3d95906ef1b30552caf9274bf, annotations\GSEA\PFSNet\PFSNet\Models\Graph.vb"

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
    '    Code Lines: 79 (67.52%)
    ' Comment Lines: 27 (23.08%)
    '    - Xml Docs: 85.19%
    ' 
    '   Blank Lines: 11 (9.40%)
    '     File Size: 6.04 KB


    '     Module Data
    ' 
    '         Function: decompose_graph, Frame, simplify
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.Framework.StorageProvider
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Linq.JoinExtensions
Imports SMRUCC.genomics.Analysis.PFSNet.DataStructure

Namespace R.Graph

    Public Module Data

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
        Public Function Frame(d As GraphEdge(), Optional directed As Boolean = True, Optional vertices As DataFrameResolver = Nothing) As PFSNetGraph
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
            Dim source As GraphEdge() = If(graph.edges, Array.Empty(Of GraphEdge)())
            Dim keys As New System.Collections.Generic.HashSet(Of String)(StringComparer.Ordinal)
            Dim unique As New System.Collections.Generic.List(Of GraphEdge)

            For Each edge As GraphEdge In source
                If edge Is Nothing OrElse edge.isSelfLoop Then
                    Continue For
                End If

                Dim gene1 As String = If(edge.g1, "")
                Dim gene2 As String = If(edge.g2, "")
                Dim first As String
                Dim second As String

                If StringComparer.Ordinal.Compare(gene1, gene2) <= 0 Then
                    first = gene1
                    second = gene2
                Else
                    first = gene2
                    second = gene1
                End If

                Dim key As String = If(edge.pathwayID, "") & vbNullChar & first & vbNullChar & second

                If keys.Add(key) Then
                    unique.Add(edge)
                End If
            Next

            Dim graphId As String = graph.Id

            If String.IsNullOrEmpty(graphId) AndAlso unique.Count > 0 Then
                graphId = unique(0).pathwayID
            End If

            Return New PFSNetGraph With {
                .nodes = graph.nodes,
                .edges = unique.ToArray,
                .masked = graph.masked,
                .Id = graphId
            }
        End Function

        Public Iterator Function decompose_graph(g As PFSNetGraph, min_vertices As Integer) As IEnumerable(Of PFSNetGraph)
            Dim nodes As PFSNetGraphNode() = If(g.nodes, Array.Empty(Of PFSNetGraphNode)())
            Dim edges As GraphEdge() = If(g.edges, Array.Empty(Of GraphEdge)())
            Dim nodeLookup As New System.Collections.Generic.Dictionary(Of String, PFSNetGraphNode)(StringComparer.Ordinal)
            Dim adjacency As New System.Collections.Generic.Dictionary(Of String, System.Collections.Generic.List(Of String))(StringComparer.Ordinal)

            For Each node As PFSNetGraphNode In nodes
                If node IsNot Nothing AndAlso Not nodeLookup.ContainsKey(node.name) Then
                    nodeLookup.Add(node.name, node)
                    adjacency.Add(node.name, New System.Collections.Generic.List(Of String))
                End If
            Next

            For Each edge As GraphEdge In edges
                If edge Is Nothing OrElse Not adjacency.ContainsKey(edge.g1) OrElse Not adjacency.ContainsKey(edge.g2) Then
                    Continue For
                End If

                adjacency(edge.g1).Add(edge.g2)
                adjacency(edge.g2).Add(edge.g1)
            Next

            Dim visited As New System.Collections.Generic.HashSet(Of String)(StringComparer.Ordinal)

            For Each seed As PFSNetGraphNode In nodes
                If seed Is Nothing OrElse Not visited.Add(seed.name) Then
                    Continue For
                End If

                Dim queue As New System.Collections.Generic.Queue(Of String)
                Dim component As New System.Collections.Generic.List(Of String)

                queue.Enqueue(seed.name)

                While queue.Count > 0
                    Dim gene As String = queue.Dequeue
                    component.Add(gene)

                    For Each partner As String In adjacency(gene)
                        If visited.Add(partner) Then
                            queue.Enqueue(partner)
                        End If
                    Next
                End While

                If component.Count < min_vertices Then
                    Continue For
                End If

                Dim memberSet As New System.Collections.Generic.HashSet(Of String)(component, StringComparer.Ordinal)
                Dim componentEdges As GraphEdge() = edges _
                    .Where(Function(edge) memberSet.Contains(edge.g1) AndAlso memberSet.Contains(edge.g2)) _
                    .ToArray
                Dim componentNodes As PFSNetGraphNode() = component _
                    .Select(Function(gene)
                                Dim source As PFSNetGraphNode = nodeLookup(gene)

                                Return New PFSNetGraphNode With {
                                    .name = source.name,
                                    .weight = source.weight,
                                    .weight2 = source.weight2
                                }
                            End Function) _
                    .ToArray

                Yield New PFSNetGraph With {
                    .Id = g.Id,
                    .masked = g.masked,
                    .pvalue = 1,
                    .statistics = 0,
                    .edges = componentEdges,
                    .nodes = componentNodes
                }
            Next
        End Function

    End Module
End Namespace
