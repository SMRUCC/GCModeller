#Region "Microsoft.VisualBasic::c2853de71aa8524b8b027ba04577fa47, annotations\WGCNA\WGCNA\Analysis.vb"

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

    '   Total Lines: 140
    '    Code Lines: 112 (80.00%)
    ' Comment Lines: 8 (5.71%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 20 (14.29%)
    '     File Size: 5.78 KB


    ' Module Analysis
    ' 
    '     Function: createGraph, Run, setModules
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports Microsoft.VisualBasic.DataMining.HierarchicalClustering
Imports Microsoft.VisualBasic.DataMining.UMAP
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports Microsoft.VisualBasic.Math.Matrix
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

#If NET48 Then
Imports System.Drawing
#End If

Public Module Analysis

    ''' <summary>
    ''' run WGCNA analysis
    ''' </summary>
    ''' <param name="samples">
    ''' an expression matrix object of gene features in rows and sample id in columns
    ''' </param>
    ''' <param name="adjacency"></param>
    ''' <returns></returns>
    Public Function Run(samples As Matrix, Optional adjacency As Double = 0.6, Optional umapLayout As Boolean = True) As Result
        Call VBDebugger.EchoLine("do pearson correlation matrix evaluation...")
        Dim cor As CorrelationMatrix = samples.Correlation(Function(gene) gene.experiments)
        Dim betaSeq As Double() = seq(1, 10, by:=1).JoinIterates(seq(11, 30, by:=2)).ToArray
        Call VBDebugger.EchoLine("do beta test...")
        Dim betaList As BetaTest() = BetaTest.BetaTable(cor, betaSeq, adjacency).ToArray
        Dim beta As BetaTest = betaList(BetaTest.Best(betaList))
        Call VBDebugger.EchoLine("build network graph!")
        Dim network As NumericMatrix = cor.WeightedCorrelation(beta.Power, pvalue:=False).Adjacency(adjacency)
        Dim K As New Vector(network.RowApply(AddressOf WeightedNetwork.sumK))
        Call VBDebugger.EchoLine("create TOM matrix...")
        Dim tomMat As NumericMatrix = TOM.Matrix(network, K)
        Dim dist As New DistanceMatrix(samples.expression.Keys, 1 - tomMat)
        Dim g As NetworkGraph = network.createGraph(samples, umapLayout, cor, tomMat)
        Call VBDebugger.EchoLine("make tree clustering!")
        Dim alg As ClusteringAlgorithm = New DefaultClusteringAlgorithm With {.debug = True}
        Dim matrix As Double()() = dist.PopulateRows _
            .Select(Function(a) a.ToArray) _
            .ToArray

        Call VBDebugger.EchoLine("make metabolite cluster modules...")

        Dim cluster As Cluster = alg.performClustering(matrix, dist.keys, New AverageLinkageStrategy)
        Dim modules = cluster _
            .CreateModules _
            .ToDictionary(Function(a) a.name,
                            Function(a)
                                Return a.ToArray
                            End Function)

        Call g.ApplyAnalysis
        Call VBDebugger.EchoLine(" ~ done!")

        Return New Result With {
            .beta = beta,
            .hclust = cluster,
            .K = K,
            .network = g.setModules(modules),
            .TOM = tomMat,
            .modules = modules,
            .softBeta = betaList
        }
    End Function

    <Extension>
    Private Function setModules(g As NetworkGraph, modules As Dictionary(Of String, String())) As NetworkGraph
        Dim colors As LoopArray(Of SolidBrush) = Designer _
            .GetColors("paper", n:=modules.Count) _
            .Select(Function(c) New SolidBrush(c)) _
            .ToArray

        For Each module_set In modules
            Dim color As SolidBrush = ++colors

            For Each id As String In module_set.Value
                Dim v = g.GetElementByID(id)

                If Not v Is Nothing Then
                    v.data("module_set") = module_set.Key
                    v.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = module_set.Key
                    v.data.color = color
                End If
            Next
        Next

        Return g
    End Function

    <Extension>
    Private Function createGraph(mat As NumericMatrix, samples As Matrix, umapLayout As Boolean, cor As CorrelationMatrix, TOM As NumericMatrix) As NetworkGraph
        Dim geneId As String() = samples.expression.Keys.UniqueNames.ToArray
        Dim g As New NetworkGraph
        Dim umap As Umap = Nothing
        Dim layout As Double()() = Nothing
        Dim offset As i32 = 0

        If umapLayout Then
            umap = New Umap(dimensions:=3, numberOfNeighbors:=64, localConnectivity:=2)
            umap.InitializeFit(mat.ArrayPack(deepcopy:=True))
            umap = umap.Step(1200)
            layout = umap.GetEmbedding
        End If

        Call VBDebugger.EchoLine("assign the gene id nodes.")

        For Each id As String In geneId
            Dim node As Node = g.CreateNode(id)

            If umapLayout Then
                node.data.initialPostion = New FDGVector3(layout(++offset))
            End If
        Next

        Call VBDebugger.EchoLine("create links between the gene expression.")

        Dim edge As Edge = Nothing

        For Each i As Integer In TqdmWrapper.Range(0, geneId.Length)
            For j As Integer = 0 To geneId.Length - 1
                If i <> j AndAlso mat(i, j) <> 0.0 Then
                    Call g.AddEdge(geneId(i), geneId(j), weight:=mat(i, j), getNewEdge:=edge)

                    edge.data("TOM") = TOM(i, j)
                    edge.data("pearson") = cor(i, j)
                    edge.data("pvalue") = cor.pvalue(i, j)
                End If
            Next
        Next

        Return g
    End Function
End Module
