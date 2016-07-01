#Region "Microsoft.VisualBasic::1825b2249f2af0cea845838da5943d39, ..\GCModeller\analysis\PFSNet\R.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.AnalysisTools.CellularNetwork.PFSNet.DataStructure
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace R

    Module Base

        ''' <summary>
        ''' The generic function quantile produces sample quantiles corresponding to the given probabilities. The smallest observation corresponds to a probability of 0 and the largest to a probability of 1.
        ''' </summary>
        ''' <param name="x">numeric vector whose sample quantiles are wanted, or an object of a class for which a method has been defined (see also ‘details’). NA and NaN values are not allowed in numeric vectors unless na.rm is TRUE.</param>
        ''' <param name="probs">numeric vector of probabilities with values in [0,1]. (Values up to 2e-14 outside that range are accepted and moved to the nearby endpoint.)</param>
        ''' <remarks></remarks>
        Public Function Quantile(x As Double(), probs As Double) As Double
            'Dim groupedList = (From n As Double In x Select n Group n By n Into Group).ToArray
            'Dim xLen As Integer = x.Length
            'Dim dist = (From n In groupedList Let data As Double() = n.Group.ToArray Select n.n, pro = data.Count / xLen, data Order By data.Count Descending).ToArray
            'Dim ChunkBuffer = (From item In dist Where item.pro >= probs Select item.data).ToArray.MatrixToList
            Dim q As Double

            'If ChunkBuffer.IsNullOrEmpty Then
            '    q = dist.First.n
            'Else
            '    q = ChunkBuffer.Average
            'End If

            'Return q

            Call ALGLIB.alglib.samplepercentile(x, probs, q)

            Return q
        End Function

        <Extension> Public Function [Select](data As DataFrameRow(), names As String()) As DataFrameRow()
            Dim LQuery = (From item In data Where Array.IndexOf(names, item.Name) > -1 Select item).ToArray
            Return LQuery
        End Function

        <Extension> Public Function [Select](data As DataFrameRow(), name As String) As DataFrameRow
            Dim LQuery = (From item In data Where String.Equals(item.Name, name) Select item).ToArray
            Return LQuery.FirstOrDefault
        End Function

        ''' <summary>
        ''' sample.int(n, size = n, replace = FALSE, prob = NULL)
        ''' sample takes a sample of the specified size from the elements of x using either with or without replacement.
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="size"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Sample(Of T)(x As T(), size As Integer) As T()
            Dim rndCollection = x.Randomize
            Return rndCollection.Take(size).ToArray
        End Function

        Public Function Sample(n As Integer, size As Integer) As Integer()
            Return Sample(Of Integer)(x:=n.Sequence, size:=size)
        End Function

        Public Function cbind(d1 As DataFrameRow(), d2 As DataFrameRow()) As DataFrameRow()
            Dim LQuery = (From item In {d1, d2}.MatrixAsIterator Select item Group By item.Name Into Group).ToArray
            Dim ChunkBuffer As DataFrameRow() = (From item In LQuery
                                                 Let expr As Double() = (From data As DataFrameRow
                                                                         In item.Group
                                                                         Select data.ExperimentValues).MatrixToVector
                                                 Select New DataFrameRow With {
                                                     .Name = item.Name,
                                                     .ExperimentValues = expr}).ToArray
            Return ChunkBuffer
        End Function

        Public Function rep(q As Boolean, n As Integer) As Double()
            Return New Double(n - 1) {}
        End Function
    End Module
End Namespace

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
                                      Select {edge.g1, edge.g2}).MatrixAsIterator.Distinct.ToArray
            Dim nodes = (From id As String In allsId
                         Let node As PFSNetGraphNode = New PFSNetGraphNode With {
                             .Name = id
                         }
                         Select node).ToArray
            Return New PFSNetGraph With {
                .masked = True,
                .Edges = d,
                .Nodes = nodes
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
                                        In graph.Edges
                                        Where Not edge.SelfLoop
                                        Select edge).ToArray
            Return New PFSNetGraph With {
                .Nodes = graph.Nodes,
                .Edges = edges,
                .masked = graph.masked,
                .Id = graph.Edges.First.PathwayID
            }
        End Function
    End Module
End Namespace
