#Region "Microsoft.VisualBasic::202e81b42fe170c887ed7b4b75511e7b, WGCNA\WGCNA\Analysis.vb"

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

    ' Module Analysis
    ' 
    '     Function: createGraph, Run
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.DataMining.HierarchicalClustering
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.DataFrame
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Public Module Analysis

    Public Function Run(samples As Matrix, Optional adjacency As Double = 0.6) As Result
        Dim cor As CorrelationMatrix = samples.Correlation(Function(gene) gene.experiments)
        Dim betaSeq As Double() = seq(1, 10, by:=1).JoinIterates(seq(11, 30, by:=2)).ToArray
        Dim betaList As BetaTest() = BetaTest.BetaTable(cor, betaSeq, adjacency).ToArray
        Dim beta As BetaTest = betaList(BetaTest.Best(betaList))
        Dim network As GeneralMatrix = cor.WeightedCorrelation(beta.Power, pvalue:=False).Adjacency(adjacency)
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
            .TOM = tomMat,
            .modules = cluster _
                .CreateModules _
                .ToDictionary(Function(a) a.name,
                              Function(a)
                                  Return a.ToArray
                              End Function),
            .softBeta = betaList
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

