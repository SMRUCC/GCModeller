#Region "Microsoft.VisualBasic::9b14035be9af89047ad03ac8dc29190d, annotations\WGCNA\WGCNA\Analysis.vb"

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

    '   Total Lines: 265
    '    Code Lines: 175 (66.04%)
    ' Comment Lines: 49 (18.49%)
    '    - Xml Docs: 77.55%
    ' 
    '   Blank Lines: 41 (15.47%)
    '     File Size: 12.03 KB


    ' Module Analysis
    ' 
    '     Function: createGraph, Run, (+2 Overloads) RunWithPhenotype, setModules
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Diagnostics
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports Microsoft.VisualBasic.DataMining.HierarchicalClustering
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports Microsoft.VisualBasic.Math.Matrix
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis.ANOVA
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

''' <summary>
''' WGCNA分析主模块
''' </summary>
Public Module Analysis

    ''' <summary>
    ''' run WGCNA analysis
    ''' </summary>
    ''' <param name="samples">
    ''' an expression matrix object of gene features in rows and sample id in columns
    ''' </param>
    ''' <param name="adjacency">邻接矩阵的边截断阈值</param>
    ''' <param name="pcaLayout">是否用 PCA 前三主成分作为网络节点的初始坐标</param>
    ''' <param name="treeCut">树剪切方式，默认动态（与 R 的 cutreeHybrid 对应）</param>
    ''' <param name="maxEdges">
    ''' 构建网络图时的最大边数（0 表示不限制）。
    ''' 邻接矩阵是稠密的，n 较大时 n^2 条边会直接耗尽内存，建议设置上限或直接改用
    ''' <see cref="RunBlockwise"/>（默认不建图）。
    ''' </param>
    ''' <param name="buildGraph">
    ''' 是否构建网络图对象。邻接矩阵是稠密的，n 较大时 n^2 条边会耗尽内存，
    ''' 只需要模块划分时可以设为 False。
    ''' </param>
    ''' <param name="power">
    ''' 软阈值幂次。NaN（默认）时先做 beta 扫描自动估计；
    ''' 与 GNU R 做对照时可以显式指定，保证两边使用同一个幂次。
    ''' </param>
    ''' <returns>WGCNA 分析结果</returns>
    ''' <remarks>
    ''' 本函数一次性把全部基因放进一个块，因此相关矩阵与 TOM 都是 O(n^2) 内存。
    ''' 基因数超过几千时请改用 <see cref="RunBlockwise"/>。
    ''' </remarks>
    Public Function Run(samples As Matrix,
                        Optional adjacency As Double = 0.6,
                        Optional pcaLayout As Boolean = True,
                        Optional treeCut As TreeCutMethod = TreeCutMethod.Dynamic,
                        Optional maxEdges As Integer = 0,
                        Optional buildGraph As Boolean = True,
                        Optional power As Double = Double.NaN) As Result

        Dim config As New WGCNAConfig With {
            .adjacency = adjacency,
            .pcaLayout = pcaLayout,
            .treeCut = treeCut,
            .maxEdges = maxEdges,
            .buildGraph = buildGraph,
            .power = power
        }

        Return Run(samples, config)
    End Function

    ''' <summary>
    ''' 单块 WGCNA 分析（配置驱动）
    ''' </summary>
    ''' <param name="samples">基因 x 样本的表达矩阵</param>
    ''' <param name="config">分析配置</param>
    ''' <returns>WGCNA 分析结果</returns>
    ''' <remarks>
    ''' 与 <see cref="RunBlockwise"/> 使用同一套配置对象，区别只在于是否分块。
    ''' 基因数超过几千时请改用 <see cref="RunBlockwise"/>。
    ''' </remarks>
    Public Function Run(samples As Matrix, config As WGCNAConfig) As Result

        Dim n As Integer = samples.size
        Dim geneIds As String() = samples.expression _
            .Select(Function(gene) gene.geneID) _
            .ToArray()
        Dim timing As New Dictionary(Of String, Double)
        Dim sw As New Stopwatch()

        Call VBDebugger.EchoLine("do pearson correlation matrix evaluation...")

        ' GEMM 版相关矩阵：行标准化后一次 Z*Zᵀ，取代逐对 Pearson
        sw.Start()

        Dim cor As TensorCorrelation = TensorCorrelation.Create(samples)

        sw.Stop()

        timing("cor") = sw.ElapsedMilliseconds

        Call VBDebugger.EchoLine("do beta test...")

        sw.Restart()

        Dim betaList As BetaTest()
        Dim beta As BetaTest

        If Double.IsNaN(config.power) Then
            Dim betaSeq As Double() = config.GetBetaSeq()

            betaList = BetaTest.BetaTable(cor, betaSeq, config.adjacency).ToArray
            beta = betaList(BetaTest.Best(betaList))
        Else
            beta = New BetaTest With {.Power = config.power}
            betaList = New BetaTest() {beta}
        End If

        sw.Stop()

        timing("beta") = sw.ElapsedMilliseconds

        Call VBDebugger.EchoLine("build network graph!")

        sw.Restart()

        Dim absCor As Double() = WeightedNetwork.AbsCorrelation(cor)
        Dim network As Double() = WeightedNetwork.BuildAdjacency(absCor, beta.Power, config.adjacency)
        Dim K As New Vector(WeightedNetwork.ConnectivityOf(network, n))

        absCor = Nothing
        sw.Stop()

        timing("adjacency") = sw.ElapsedMilliseconds

        Call VBDebugger.EchoLine("create TOM matrix...")

        sw.Restart()

        Dim tomMat As Double() = TOM.Matrix(network, K.Array, n)
        Dim distBuf As Double() = CType(tomMat.Clone(), Double())

        Call TensorOps.DissimilarityInPlace(distBuf)

        ' pdist 压缩格式，相比交错距离矩阵省掉 n 个数组对象
        Dim pdist As Double() = TensorOps.ToPdist(distBuf, n)

        sw.Stop()

        timing("tom") = sw.ElapsedMilliseconds

        Call VBDebugger.EchoLine("make tree clustering!")

        sw.Restart()

        Dim alg As ClusteringAlgorithm = New PDistClusteringAlgorithm()
        Dim cluster As Cluster = alg.performClustering(New Double()() {pdist}, geneIds, New AverageLinkageStrategy)

        pdist = Nothing
        sw.Stop()

        timing("hclust") = sw.ElapsedMilliseconds

        Call VBDebugger.EchoLine("make metabolite cluster modules...")

        sw.Restart()

        Dim modules As Dictionary(Of String, String())

        If config.treeCut = TreeCutMethod.Dynamic Then
            Dim options As New DynamicTreeCut.CutOptions With {
                .minClusterSize = config.minModuleSize,
                .deepSplit = config.deepSplit,
                .cutHeight = config.cutHeight,
                .pamStage = config.pamStage,
                .verbose = False
            }
            Dim labels As Integer() = DynamicTreeCut.CutreeHybrid(cluster, geneIds, distBuf, options)

            modules = DynamicTreeCut.ToModules(labels, geneIds)
        Else
            modules = StaticCut.Cutree(cluster, config.distCut)
        End If

        distBuf = Nothing
        sw.Stop()

        timing("cut") = sw.ElapsedMilliseconds

        Dim g As NetworkGraph = Nothing

        If config.buildGraph Then
            sw.Restart()

            g = createGraph(network, n, samples, config.pcaLayout, cor, tomMat, config.maxEdges)

            Call g.ApplyAnalysis
            Call g.setModules(modules)

            sw.Stop()

            timing("graph") = sw.ElapsedMilliseconds
        End If

        Call VBDebugger.EchoLine(" ~ done!")

        Return New Result With {
            .beta = beta,
            .hclust = cluster,
            .K = K,
            .network = g,
            .TOM = New NumericMatrix(TensorOps.ToJagged(tomMat, n, n)),
            .modules = modules,
            .softBeta = betaList,
            .timing = timing
        }
    End Function

    ''' <summary>
    ''' 分块（blockwise）WGCNA 分析
    ''' </summary>
    ''' <param name="samples">基因 x 样本的表达矩阵</param>
    ''' <param name="config">分析配置，Nothing 时使用默认配置</param>
    ''' <param name="phenotypeData">可选的表型数据（表型名 → 样本数长度的值数组）</param>
    ''' <returns>WGCNA 分析结果</returns>
    ''' <remarks>
    ''' 这是面向大型数据集的入口，对应 GNU R WGCNA 的 <c>blockwiseModules</c>：
    ''' 基因数超过 <c>maxBlockSize</c> 时先做预聚类分块，再逐块建网与切模块，
    ''' 最后合并跨块的相似模块。内存峰值由块大小而非基因总数决定。
    ''' </remarks>
    Public Function RunBlockwise(samples As Matrix,
                                 Optional config As WGCNAConfig = Nothing,
                                 Optional phenotypeData As Dictionary(Of String, Double()) = Nothing) As Result
        Return BlockwiseModules.Run(samples, config, phenotypeData)
    End Function

    ''' <summary>
    ''' 尝试把 Tensor 计算后端切换到 CUDA GPU
    ''' </summary>
    ''' <param name="cacheBytes">显存 LRU 缓存容量（字节），0 表示自适应</param>
    ''' <param name="useFp32Gemm">矩阵乘是否走单精度内核</param>
    ''' <returns>注册成功返回 True；设备不可用时返回 False 并保持 CPU 后端</returns>
    ''' <remarks>
    ''' 默认使用 SIMD CPU 后端。调用本方法后，相关矩阵与 TOM 的 GEMM 会透明地落到 GPU 上。
    ''' </remarks>
    Public Function EnableGpu(Optional cacheBytes As Long = 0, Optional useFp32Gemm As Boolean = True) As Boolean
        Return TensorBackend.EnableGpu(cacheBytes, useFp32Gemm)
    End Function

    ''' <summary>
    ''' 把 Tensor 计算后端切回默认的 SIMD CPU 实现
    ''' </summary>
    Public Sub DisableGpu()
        Call TensorBackend.DisableGpu()
    End Sub

    ''' <summary>
    ''' 当前 Tensor 计算后端的名称（<c>SIMD</c> 或 <c>CUDA</c>）
    ''' </summary>
    ''' <returns>后端名称</returns>
    Public ReadOnly Property Backend As String
        Get
            Return TensorBackend.BackendName
        End Get
    End Property

    ''' <summary>
    ''' 运行完整的WGCNA分析（包含表型相关性分析）
    ''' 
    ''' 这是新增的主要分析函数，在基础WGCNA分析的基础上，
    ''' 增加了模块与表型相关性的计算功能。
    ''' </summary>
    ''' <param name="samples">基因表达矩阵（基因×样本）</param>
    ''' <param name="phenotypeData">表型数据字典（表型名→值数组），数组长度应与样本数相同</param>
    ''' <param name="adjacency">邻接矩阵阈值，默认0.6</param>
    ''' <param name="pcaLayout">是否使用PCA布局，默认True</param>
    ''' <returns>完整的WGCNA分析结果，包含模块-表型相关性</returns>
    Public Function RunWithPhenotype(samples As Matrix,
                                      phenotypeData As Dictionary(Of String, Double()),
                                      Optional adjacency As Double = 0.6,
                                      Optional pcaLayout As Boolean = True) As Result
        ' 首先运行基础WGCNA分析
        Call VBDebugger.EchoLine("=== Starting WGCNA Analysis with Phenotype Correlation ===")

        Dim result As Result = Run(samples, adjacency, pcaLayout)

        ' 验证表型数据
        If phenotypeData Is Nothing OrElse phenotypeData.Count = 0 Then
            Call VBDebugger.EchoLine("Warning: No phenotype data provided, skipping phenotype correlation analysis.")
            Return result
        End If

        ' 获取样本数量
        Dim nSamples As Integer = samples.sampleID.Length

        ' 验证表型数据长度
        For Each kvp In phenotypeData
            If kvp.Value.Length <> nSamples Then
                Throw New ArgumentException($"表型 '{kvp.Key}' 的数据长度({kvp.Value.Length})与样本数量({nSamples})不匹配")
            End If
        Next

        Call VBDebugger.EchoLine("=== Calculating Module Eigengenes ===")

        ' 计算每个模块的特征基因
        Dim eigengeneDict As New Dictionary(Of String, Double())
        Dim eigengeneResults As New List(Of ModuleEigengeneResult)

        For Each moduleKvp In result.modules
            Dim meResult = ModulePhenotype.CalculateModuleEigengene(samples, moduleKvp.Value, moduleKvp.Key)
            eigengeneDict(moduleKvp.Key) = meResult.Eigengene
            eigengeneResults.Add(meResult)
            Call VBDebugger.EchoLine($"  Module '{moduleKvp.Key}': {moduleKvp.Value.Length} genes, variance explained: {meResult.VarianceExplained:P}")
        Next

        result.moduleEigengenes = eigengeneDict
        result.moduleEigengeneResults = eigengeneResults

        Call VBDebugger.EchoLine("=== Calculating Module-Phenotype Correlations ===")

        ' 计算模块与表型的相关性
        Dim modulePhenotypeCorrs = ModulePhenotype.CalculateAllModulePhenotypeCorrelations(samples, result.modules, phenotypeData)
        result.modulePhenotypeCorrelations = modulePhenotypeCorrs

        ' 输出显著相关的模块
        Dim significantCorrs = modulePhenotypeCorrs.Where(Function(c) c.PValue < 0.05).ToList()
        Call VBDebugger.EchoLine($"  Found {significantCorrs.Count} significant module-phenotype correlations (p < 0.05)")
        For Each corr In significantCorrs.OrderByDescending(Function(c) c.AbsoluteCorrelation).Take(10)
            Call VBDebugger.EchoLine($"    {corr.ModuleName} vs {corr.PhenotypeName}: r={corr.Correlation:F3}, p={corr.PValue:F4}")
        Next

        Call VBDebugger.EchoLine("=== Calculating Gene Significance ===")

        ' 计算基因显著性
        Dim allGeneSignificance As New List(Of GeneSignificanceResult)
        For Each phenotypeKvp In phenotypeData
            Dim gsResults = ModulePhenotype.CalculateAllGeneSignificance(samples, phenotypeKvp.Value, phenotypeKvp.Key)
            allGeneSignificance.AddRange(gsResults)
        Next
        result.geneSignificance = allGeneSignificance

        Call VBDebugger.EchoLine("=== Calculating Module Membership ===")

        ' 计算模块成员
        Dim moduleMembershipResults = ModulePhenotype.CalculateAllModuleMembership(samples, result.modules)
        result.moduleMembership = moduleMembershipResults

        Call VBDebugger.EchoLine("=== WGCNA Analysis Complete ===")

        Return result
    End Function

    ''' <summary>
    ''' 运行WGCNA分析（使用单个表型）
    ''' </summary>
    ''' <param name="samples">基因表达矩阵</param>
    ''' <param name="phenotypeName">表型名称</param>
    ''' <param name="phenotypeValues">表型值数组</param>
    ''' <param name="adjacency">邻接矩阵阈值</param>
    ''' <param name="pcaLayout">是否使用PCA布局</param>
    ''' <returns>WGCNA分析结果</returns>
    Public Function RunWithPhenotype(samples As HTS.DataFrame.Matrix,
                                     phenotypeName As String,
                                     phenotypeValues As Double(),
                                     Optional adjacency As Double = 0.6,
                                     Optional pcaLayout As Boolean = True) As Result

        Dim phenotypeData As New Dictionary(Of String, Double()) From {
            {phenotypeName, phenotypeValues}
        }
        Return RunWithPhenotype(samples, phenotypeData, adjacency, pcaLayout)
    End Function

    <Extension>
    Private Function setModules(g As NetworkGraph, modules As Dictionary(Of String, String())) As NetworkGraph
        Dim colors As LoopArray(Of String) = Designer.GetColors("paper", n:=modules.Count) _
            .Select(Function(c) Imaging.ToHtmlColor(c)) _
            .ToArray

        For Each module_set In modules
            Dim color As String = ++colors

            For Each id As String In module_set.Value
                Dim v = g.GetElementByID(id)

                If Not v Is Nothing Then
                    v.data("module_set") = module_set.Key
                    v.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = module_set.Key
                    v.data.color = Imaging.GetBrush(color)
                End If
            Next
        Next

        Return g
    End Function

    ''' <summary>
    ''' 由邻接矩阵构建共表达网络图
    ''' </summary>
    ''' <param name="mat">行优先的 n x n 邻接矩阵</param>
    ''' <param name="n">矩阵阶数（基因数）</param>
    ''' <param name="samples">表达矩阵（提供基因 ID）</param>
    ''' <param name="pcaLayout">是否用 PCA 前三主成分作为初始坐标</param>
    ''' <param name="cor">相关矩阵（提供 pearson 与 pvalue 边属性）</param>
    ''' <param name="TOM">行优先的 n x n TOM 矩阵</param>
    ''' <param name="maxEdges">最大边数上限，0 表示不限制</param>
    ''' <returns>网络图对象</returns>
    ''' <remarks>
    ''' 邻接矩阵是稠密的，理论上会产生 O(n^2) 条边。
    ''' 这里在超过 <paramref name="maxEdges"/> 时按权重降序截断，避免大规模数据下直接 OOM。
    ''' </remarks>
    Private Function createGraph(mat As Double(), n As Integer, samples As Matrix,
                                 pcaLayout As Boolean, cor As TensorCorrelation,
                                 TOM As Double(), Optional maxEdges As Integer = 0) As NetworkGraph
        Dim geneId As String() = samples.expression.Keys.UniqueNames.ToArray
        Dim g As New NetworkGraph
        Dim proj As MultivariateAnalysisResult = Nothing
        Dim layout As Double()() = Nothing
        Dim offset As i32 = 0

        If pcaLayout Then
            proj = TensorOps.ToJagged(mat, n, n) _
                .Select(Function(r, i) New NamedCollection(Of Double)(geneId(i), r)) _
                .CommonDataSet(geneId) _
                .PrincipalComponentAnalysis(maxPC:=3)
            layout = proj.GetPCAScore _
                .NumericMatrix _
                .Select(Function(r) r.value) _
                .ToArray
        End If

        Call VBDebugger.EchoLine("assign the gene id nodes.")

        For Each id As String In geneId
            Dim node As Node = g.CreateNode(id)

            If pcaLayout Then
                node.data.initialPostion = New FDGVector3(layout(++offset))
            End If
        Next

        Call VBDebugger.EchoLine("create links between the gene expression.")

        Dim edge As Edge = Nothing

        ' 先收集候选边，必要时按权重截断
        Dim links As New List(Of (i As Integer, j As Integer, w As Double))

        For i As Integer = 0 To n - 1
            Dim rowOffset As Integer = i * n

            For j As Integer = i + 1 To n - 1
                Dim w As Double = mat(rowOffset + j)

                If w <> 0.0 Then
                    Call links.Add((i, j, w))
                End If
            Next
        Next

        If maxEdges > 0 AndAlso links.Count > maxEdges Then
            links = links _
                .OrderByDescending(Function(l) l.w) _
                .Take(maxEdges) _
                .ToList()
        End If

        For Each link In links
            Dim i As Integer = link.i
            Dim j As Integer = link.j

            Call g.AddEdge(geneId(i), geneId(j), weight:=link.w, getNewEdge:=edge)

            edge.data("TOM") = TOM(i * n + j)
            edge.data("pearson") = cor(i, j)
            edge.data("pvalue") = cor.Pvalue(i, j)
        Next

        Return g
    End Function
End Module
