Imports Microsoft.VisualBasic.DataMining.UMAP.KNN
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports Microsoft.VisualBasic.Math.Scripting.Rscript
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports System.Runtime.CompilerServices

Public Module Clustering

    Public Function CreatePhenoGraph(data As DataSet(), Optional k As Integer = 30)

    End Function

    Public Function CreatePhenoGraph(data As GeneralMatrix, Optional k As Integer = 30) As Communities
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
        Dim t1 As Double = App.ElapsedMilliseconds
        Dim neighborMatrix = KNearestNeighbour.FindNeighbors(data, k:=k + 1).knnIndices
        cat("DONE ~", App.ElapsedMilliseconds - t1, "s\n", " Compute jaccard coefficient between nearest-neighbor sets...")
        ' t2 <- system.time(links <- jaccard_coeff(neighborMatrix))
        Dim t2 As Double = App.ElapsedMilliseconds
        Dim links As GeneralMatrix = jaccard_coeff(neighborMatrix)
        cat("DONE ~", App.ElapsedMilliseconds - t2, "s\n", " Build undirected graph from the weighted links...")

        ' take rows
        ' colnames(relations)<- c("from","to","weight")
        links = links(links(0, byRow:=False) > 0)

        Dim t3 = App.ElapsedMilliseconds
        Dim g = links.AsGraph()
        cat("DONE ~", App.ElapsedMilliseconds - t3, "s\n", " Run louvain clustering on the graph ...")
        Dim t4 = App.ElapsedMilliseconds

        ' Other community detection algorithms: 
        '    cluster_walktrap, cluster_spinglass, 
        '    cluster_leading_eigen, cluster_edge_betweenness, 
        '    cluster_fast_greedy, cluster_label_prop  
        Dim community = cluster_louvain(g)

        cat("DONE ~", App.ElapsedMilliseconds - t4, "s\n")

        message("Run Rphenograph DONE, totally takes ", Sum(C(t1, t2, t3, t4)), "s.")
        cat("  Return a community class\n  -Modularity value:", modularity(community), "\n")
        cat("  -Number of clusters:", Length(unique(membership(community))))


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
    Private Function AsGraph(links As GeneralMatrix) As NetworkGraph

    End Function
End Module
