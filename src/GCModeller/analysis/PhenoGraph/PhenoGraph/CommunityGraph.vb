Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.GraphTheory.KdTree
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.DataMining.UMAP.KNN
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports Microsoft.VisualBasic.Math.Scripting.Rscript

''' <summary>
''' implementation of the PhenoGraph algorithm
''' 
''' A simple R implementation of the [PhenoGraph](http://www.cell.com/cell/abstract/S0092-8674(15)00637-6) algorithm, 
''' which is a clustering method designed for high-dimensional single-cell data analysis. It works by creating a graph ("network") representing 
''' phenotypic similarities between cells by calclating the Jaccard coefficient between nearest-neighbor sets, and then identifying communities 
''' using the well known [Louvain method](https://sites.google.com/site/findcommunities/) in this graph. 
''' </summary>
Public Module CommunityGraph

    Public Function CreatePhenoGraph(data As DataSet(), Optional k As Integer = 30, Optional cutoff As Double = 0) As NetworkGraph
        Dim propertyNames As String() = data.PropertyNames
        Dim matrix As New List(Of Double())

        For Each row As DataSet In data
            matrix.Add(row(keys:=propertyNames))
        Next

        Dim dataMat As New NumericMatrix(matrix.ToArray)
        Dim graph As NetworkGraph = CreatePhenoGraph(dataMat, k)

        For Each v As Node In graph.vertex
            v.label = data(v.ID).ID
        Next

        Return graph
    End Function

    ''' <summary>
    ''' Jacob H. Levine and et.al. Data-Driven Phenotypic Dissection of AML Reveals Progenitor-like Cells that Correlate with Prognosis. Cell, 2015.
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="k"></param>
    ''' <returns></returns>
    Public Function CreatePhenoGraph(data As GeneralMatrix, Optional k As Integer = 30, Optional cutoff As Double = 0) As NetworkGraph
        If k < 1 Then
            Throw New ArgumentException("k must be a positive integer!")
        ElseIf k > data.RowDimension - 2 Then
            Throw New ArgumentException("k must be smaller than the total number of points!")
        End If

        message("Run Rphenograph starts:", "\n",
        "  -Input data of ", nrow(data), " rows and ", ncol(data), " columns", "\n",
        "  -k is set to ", k)

        cat("  Finding nearest neighbors...")

        ' t1 <- system.time(neighborMatrix <- find_neighbors(data, k=k+1)[,-1])
        Dim t1 As Value(Of Double) = App.ElapsedMilliseconds
        Dim neighborMatrix = ApproximateNearNeighbor _
            .FindNeighbors(data, k:=k + 1) _
            .Select(Function(row)
                        If row.size = k + 1 Then
                            Return row.indices
                        Else
                            Return row.indices _
                                .JoinIterates((-1).Repeats(k + 1 - row.size)) _
                                .ToArray
                        End If
                    End Function) _
            .ToArray

        cat("DONE ~", t1 = App.ElapsedMilliseconds - CDbl(t1), "s\n", " Compute jaccard coefficient between nearest-neighbor sets...")
        ' t2 <- system.time(links <- jaccard_coeff(neighborMatrix))
        Dim t2 As Value(Of Double) = App.ElapsedMilliseconds
        Dim links As GeneralMatrix = jaccard_coeff(neighborMatrix)
        cat("DONE ~", t2 = App.ElapsedMilliseconds - CDbl(t2), "s\n", " Build undirected graph from the weighted links...")

        ' take rows
        ' colnames(relations)<- c("from","to","weight")
        ' which its coefficient should be greater than ZERO
        links = links(links(2, byRow:=False) > cutoff)

        Dim t3 As Value(Of Double) = App.ElapsedMilliseconds
        Dim g = DirectCast(links, NumericMatrix).AsGraph()
        cat("DONE ~", t3 = App.ElapsedMilliseconds - CDbl(t3), "s\n", " Run louvain clustering on the graph ...")
        Dim t4 As Value(Of Double) = App.ElapsedMilliseconds

        ' Other community detection algorithms: 
        '    cluster_walktrap, cluster_spinglass, 
        '    cluster_leading_eigen, cluster_edge_betweenness, 
        '    cluster_fast_greedy, cluster_label_prop  
        Dim community As NetworkGraph = Communities.Analysis(g)

        cat("DONE ~", t4 = App.ElapsedMilliseconds - CDbl(t4), "s\n")

        message("Run Rphenograph DONE, totally takes ", {CDbl(t1), CDbl(t2), CDbl(t3), CDbl(t4)}.Sum / 1000, "s.")
        cat("  Return a community class\n  -Modularity value:", Communities.Modularity(community), "\n")
        cat("  -Number of clusters:", Communities.Community(g).Values.Distinct.Count)

        Return community
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="links">
    ''' ["from","to","weight"]
    ''' </param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Private Function AsGraph(links As NumericMatrix) As NetworkGraph
        Dim g As New NetworkGraph
        Dim from As String
        Dim [to] As String
        Dim weight As Double

        VBDebugger.Mute = True

        For Each row As Vector In links.RowVectors
            from = row(0)
            [to] = row(1)
            weight = row(2)

            If g.GetElementByID(from) Is Nothing Then
                Call g.CreateNode(from)
            End If
            If g.GetElementByID([to]) Is Nothing Then
                Call g.CreateNode([to])
            End If

            Call g.CreateEdge(
                u:=g.GetElementByID(from),
                v:=g.GetElementByID([to]),
                weight:=weight
            )
        Next

        VBDebugger.Mute = False

        Return g
    End Function
End Module
