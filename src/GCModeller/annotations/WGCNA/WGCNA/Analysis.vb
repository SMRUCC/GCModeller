Imports Microsoft.VisualBasic.Math.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.DataMining.HierarchicalClustering
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Public Module Analysis

    Public Function Run(samples As Matrix) As Result
        Dim cor As CorrelationMatrix = samples.Correlation(Function(gene) gene.experiments)
        Dim betaList As Double() = seq(1, 30, 0.5).ToArray
        Dim beta As BetaTest = BetaTest.Best(cor, betaList)
        Dim network As GeneralMatrix = cor.WeightedCorrelation(beta.Power, pvalue:=False)
        Dim K As New Vector(network.RowApply(AddressOf WeightedNetwork.sumK))
        Dim tomMat As GeneralMatrix = TOM.Matrix(network, K)
        Dim dist As New DistanceMatrix(samples.expression.Keys, 1 - tomMat)
        Dim alg As ClusteringAlgorithm = New DefaultClusteringAlgorithm With {.debug = True}
        Dim matrix As Double()() = dist.PopulateRows _
            .Select(Function(a) a.ToArray) _
            .ToArray
        Dim cluster As Cluster = alg.performClustering(matrix, dist.keys, New AverageLinkageStrategy)

        Return New Result With {
            .beta = beta,
            .hclust = cluster,
            .K = K,
            .network = createGraph(network, samples),
            .TOM = tomMat
        }
    End Function

    Private Function createGraph(mat As GeneralMatrix, samples As Matrix) As NetworkGraph
        Dim geneId As String() = samples.expression.Keys.ToArray
        Dim g As New NetworkGraph

        For Each id As String In geneId
            Call g.CreateNode(id)
        Next

        For i As Integer = 0 To geneId.Length - 1
            For j As Integer = 0 To geneId.Length - 1
                If i <> j AndAlso mat(i, j) <> 0.0 Then
                    Call g.AddEdge(geneId(i), geneId(j), weight:=mat(i, j))
                End If
            Next
        Next

        Return g
    End Function
End Module
