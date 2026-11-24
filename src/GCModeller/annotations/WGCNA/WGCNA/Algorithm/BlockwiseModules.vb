#Region "Microsoft.VisualBasic::BlockwiseModules, annotations\WGCNA\WGCNA\Algorithm\BlockwiseModules.vb"

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

#End Region

Imports System.Diagnostics
Imports System.Threading.Tasks
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.DataMining.HierarchicalClustering
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports CorStore = Microsoft.VisualBasic.Math.Matrix
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports std = System.Math

''' <summary>
''' 单个块的分析结果
''' </summary>
Public Class BlockResult

    ''' <summary>块序号（从 1 开始）</summary>
    ''' <returns>块序号</returns>
    Public Property index As Integer
    ''' <summary>块内基因 ID（顺序与块内矩阵行列一致）</summary>
    ''' <returns>基因 ID 数组</returns>
    Public Property keys As String()
    ''' <summary>块内连通度向量</summary>
    ''' <returns>连通度数组</returns>
    Public Property K As Double()
    ''' <summary>块内的层次聚类树</summary>
    ''' <returns>聚类树，未构建时为 Nothing</returns>
    Public Property tree As Cluster
    ''' <summary>块内模块的基因 ID 字典（模块名为块局部名）</summary>
    ''' <returns>模块字典</returns>
    Public Property modules As Dictionary(Of String, String())
    ''' <summary>块内耗时（毫秒）</summary>
    ''' <returns>耗时字典</returns>
    Public Property timing As New Dictionary(Of String, Double)
End Class

''' <summary>
''' 分块（blockwise）WGCNA 主流程
''' </summary>
''' <remarks>
''' 对应 GNU R WGCNA 的 <c>blockwiseModules</c>：
''' 
''' <list type="number">
''' <item><description>可选的基因预过滤；</description></item>
''' <item><description>基因数超过 <c>maxBlockSize</c> 时用 <see cref="ProjectiveKMeans"/> 预聚类分块；</description></item>
''' <item><description>在抽样基因上估计<b>全局</b>软阈值 beta（不逐块重复估计）；</description></item>
''' <item><description>逐块执行 相关矩阵 → 邻接 → TOM → 不相似度 → 层次聚类 → 模块剪切；</description></item>
''' <item><description>用 <see cref="MergeCloseModules"/> 合并跨块的相似模块；</description></item>
''' <item><description>计算模块特征基因 / 基因显著性 / 模块成员。</description></item>
''' </list>
''' 
''' <para>
''' 复杂度从「全表」的 O(n^3)（TOM）降为 O(n·B^2)，其中 B 为块大小。
''' 内存峰值也从 O(n^2) 降为 O(B^2)，这正是它能在几万基因规模上跑起来的原因。
''' </para>
''' </remarks>
Public Module BlockwiseModules

    ''' <summary>
    ''' 运行分块 WGCNA 分析
    ''' </summary>
    ''' <param name="samples">基因 x 样本的表达矩阵</param>
    ''' <param name="config">分析配置，Nothing 时使用默认配置</param>
    ''' <param name="phenotypeData">可选的表型数据（表型名 → 样本数长度的值数组）</param>
    ''' <returns>WGCNA 分析结果</returns>
    Public Function Run(samples As Matrix,
                        Optional config As WGCNAConfig = Nothing,
                        Optional phenotypeData As Dictionary(Of String, Double()) = Nothing) As Result

        If config Is Nothing Then config = New WGCNAConfig()

        Dim timing As New Dictionary(Of String, Double)
        Dim sw As New Stopwatch()
        Dim watch As New Stopwatch()

        Call watch.Start()

        ' ------------------------------------------------------------------ 后端
        sw.Start()
        Dim backend As String = TensorBackend.ApplyBackend(config.useGpu)
        sw.Stop()

        timing("backend") = sw.ElapsedMilliseconds

        If config.verbose Then
            Call VBDebugger.EchoLine($"WGCNA blockwise: backend = {backend}")
        End If

        ' ------------------------------------------------------------------ 预处理
        sw.Restart()
        Dim input As Matrix = GeneFilter.Filter(samples, config)
        Dim geneIds As String() = input.expression _
            .Select(Function(g) g.geneID) _
            .ToArray()
        Dim nGenes As Integer = geneIds.Length
        Dim nSamples As Integer = input.sampleID.Length
        sw.Stop()

        timing("filter") = sw.ElapsedMilliseconds

        If config.verbose Then
            Call VBDebugger.EchoLine($"WGCNA blockwise: {nGenes} genes x {nSamples} samples")
        End If

        If nGenes < config.minModuleSize * 2 Then
            Throw New ArgumentException($"基因数量({nGenes})太少，无法进行 WGCNA 分析。")
        End If

        ' ------------------------------------------------------------------ 相关矩阵存储（可选；校验失败回退表达矩阵重算）
        Dim storeInfo = OpenCorrelationStore(config, geneIds, nSamples)
        Dim storeHandle As CorrelationStoreHandle = storeInfo.handle
        Dim store As CorStore.CorrelationMatrixStore = If(storeHandle Is Nothing, Nothing, storeHandle.Store)
        Dim storeRows As Integer() = storeInfo.rows
        Dim corSource As String = storeInfo.source

        If Not String.IsNullOrEmpty(config.correlationStore) Then
            timing("correlationStore") = storeInfo.ms
        End If

        ' ------------------------------------------------------------------ 分块
        sw.Restart()

        Dim blocks As Integer()() = Partition(input, config)
        sw.Stop()

        timing("precluster") = sw.ElapsedMilliseconds

        If config.verbose Then
            Call VBDebugger.EchoLine($"WGCNA blockwise: {blocks.Length} block(s), sizes = {blocks.Select(Function(b) b.Length).JoinBy(", ")}")
        End If

        ' ------------------------------------------- 全局 beta 与逐块分析（相关矩阵存储用完即释放）
        Dim betaSeq As Double() = config.GetBetaSeq()
        Dim betaList As BetaTest()
        Dim beta As BetaTest
        Dim results As BlockResult()
        Dim degree As Integer = std.Max(1, config.maxConcurrentBlocks)

        Try
            ' -------------------------------------------------------------- 全局 beta
            sw.Restart()

            If Double.IsNaN(config.power) Then
                betaList = EstimateBeta(input, config, betaSeq, store, storeRows).ToArray()
                beta = betaList(BetaTest.Best(betaList))
            Else
                beta = New BetaTest With {.Power = config.power}
                betaList = New BetaTest() {beta}
            End If

            sw.Stop()

            timing("beta") = sw.ElapsedMilliseconds

            If config.verbose Then
                Call VBDebugger.EchoLine($"WGCNA blockwise: soft power = {beta.Power}")
            End If

            ' -------------------------------------------------------------- 逐块分析
            sw.Restart()

            results = New BlockResult(blocks.Length - 1) {}

            If degree = 1 Then
                For i As Integer = 0 To blocks.Length - 1
                    results(i) = ProcessBlock(input, blocks(i), i + 1, beta.Power, config, store, storeRows)
                Next
            Else
                Call Parallel.For(0, blocks.Length,
                    New ParallelOptions With {.MaxDegreeOfParallelism = degree},
                    Sub(i)
                        results(i) = ProcessBlock(input, blocks(i), i + 1, beta.Power, config, store, storeRows)
                    End Sub)
            End If

            sw.Stop()

            timing("blocks") = sw.ElapsedMilliseconds
        Finally
            ' 相关矩阵存储只服务 beta 估计与逐块相关矩阵；此处立即释放文件句柄，
            ' 后续 eigengene / merge / phenotype 阶段不再持有
            If storeHandle IsNot Nothing Then
                Call storeHandle.Dispose()
                storeHandle = Nothing
            End If
        End Try

        ' ------------------------------------------------------------------ 汇总模块
        sw.Restart()

        Dim allModules As New Dictionary(Of String, List(Of String))
        Dim lastTree As Cluster = Nothing
        Dim lastK As Double() = Nothing

        For i As Integer = 0 To results.Length - 1
            Dim br As BlockResult = results(i)

            If br Is Nothing Then Continue For

            For Each kv In br.modules
                Dim name As String = $"B{i + 1}.{kv.Key}"

                If Not allModules.ContainsKey(name) Then
                    allModules(name) = New List(Of String)()
                End If

                Call allModules(name).AddRange(kv.Value)
            Next

            lastTree = br.tree
            lastK = br.K
        Next

        Dim flatModules As New Dictionary(Of String, String())

        For Each kv In allModules
            flatModules(kv.Key) = kv.Value.Distinct.ToArray()
        Next

        sw.Stop()

        timing("collect") = sw.ElapsedMilliseconds

        ' ------------------------------------------------------------------ 跨块合并
        sw.Restart()

        Dim merged As Dictionary(Of String, String()) = MergeCloseModules _
            .Merge(input, flatModules, config.mergeCutHeight)

        sw.Stop()

        timing("merge") = sw.ElapsedMilliseconds

        If config.verbose Then
            Call VBDebugger.EchoLine($"WGCNA blockwise: {flatModules.Count} raw module(s) -> {merged.Count} merged module(s)")
        End If

        ' ------------------------------------------------------------------ 特征基因/显著性/成员
        sw.Restart()

        Dim eigengenes As New Dictionary(Of String, Double())
        Dim eigengeneResults As New List(Of ModuleEigengeneResult)

        For Each kv In merged
            Dim eigengene As Double() = ModuleEigengene.Compute(input, kv.Value)

            eigengenes(kv.Key) = eigengene
            eigengeneResults.Add(New ModuleEigengeneResult With {
                .ModuleName = kv.Key,
                .Eigengene = eigengene,
                .GeneCount = kv.Value.Length,
                .VarianceExplained = 0
            })
        Next

        sw.Stop()

        timing("eigengene") = sw.ElapsedMilliseconds

        ' ------------------------------------------------------------------ 组装结果
        Dim result As New Result With {
            .beta = beta,
            .softBeta = betaList,
            .hclust = lastTree,
            .K = If(lastK Is Nothing, Nothing, New Vector(lastK)),
            .modules = merged,
            .moduleEigengenes = eigengenes,
            .moduleEigengeneResults = eigengeneResults,
            .blocks = blocks _
                .Select(Function(b) b.Select(Function(i) geneIds(i)).ToArray()) _
                .ToArray(),
            .blockResults = results,
            .timing = timing,
            .correlationSource = corSource,
            .TOM = Nothing,
            .network = Nothing
        }

        ' ------------------------------------------------------------------ 表型相关分析
        If phenotypeData IsNot Nothing AndAlso phenotypeData.Count > 0 Then
            sw.Restart()

            result.modulePhenotypeCorrelations = ModulePhenotype _
                .CalculateAllModulePhenotypeCorrelations(input, merged, phenotypeData) _
                .ToList()

            Dim gs As New List(Of GeneSignificanceResult)

            For Each kv In phenotypeData
                Call gs.AddRange(ModulePhenotype.CalculateAllGeneSignificance(input, kv.Value, kv.Key))
            Next

            result.geneSignificance = gs
            result.moduleMembership = ModulePhenotype _
                .CalculateAllModuleMembership(input, merged) _
                .ToList()

            sw.Stop()

            timing("phenotype") = sw.ElapsedMilliseconds
        End If

        Call watch.Stop()

        timing("total") = watch.ElapsedMilliseconds

        If config.verbose Then
            Call VBDebugger.EchoLine($"WGCNA blockwise: done in {watch.ElapsedMilliseconds / 1000:F1}s")
        End If

        Return result
    End Function

    ''' <summary>
    ''' 把基因划分成块
    ''' </summary>
    ''' <param name="input">过滤后的表达矩阵</param>
    ''' <param name="config">分析配置</param>
    ''' <returns>块数组，元素为基因在矩阵中的 0 基行下标</returns>
    Private Function Partition(input As Matrix, config As WGCNAConfig) As Integer()()
        Dim n As Integer = input.size

        If n <= config.maxBlockSize Then
            Return New Integer()() {Enumerable.Range(0, n).ToArray()}
        End If

        Dim expr As Double()() = input.expression _
            .Select(Function(g) g.experiments) _
            .ToArray()

        Return ProjectiveKMeans.Partition(expr, config.maxBlockSize, config.randomSeed)
    End Function

    ''' <summary>
    ''' 在抽样基因上估计全局软阈值 beta
    ''' </summary>
    ''' <param name="input">完整表达矩阵</param>
    ''' <param name="config">分析配置</param>
    ''' <param name="betaSeq">候选幂次序列</param>
    ''' <param name="store">可选的相关矩阵存储；非 Nothing 时抽样子矩阵从 store 切片而非重算</param>
    ''' <param name="storeRows">输入基因 → store 行号映射（<see cref="StoreCorrelation.MapGenes"/> 的产物）</param>
    ''' <returns>候选评估结果</returns>
    ''' <remarks>
    ''' 全表相关矩阵在大规模数据下根本装不下，因此 beta 只在一份随机抽样上估计。
    ''' R 的 <c>blockwiseModules</c> 也是这么做的。
    ''' </remarks>
    Private Function EstimateBeta(input As Matrix, config As WGCNAConfig, betaSeq As Double(),
                                  Optional store As CorStore.CorrelationMatrixStore = Nothing,
                                  Optional storeRows As Integer() = Nothing) As IEnumerable(Of BetaTest)
        Dim n As Integer = input.size
        Dim size As Integer = config.betaSampleSize

        If size <= 0 Then
            size = std.Min(n, 5000)
        End If

        Dim rows As Integer()

        If size >= n Then
            rows = Enumerable.Range(0, n).ToArray()
        Else
            Dim rnd As New Random(config.randomSeed)
            Dim idx As Integer() = Enumerable.Range(0, n).ToArray()

            For i As Integer = 0 To size - 1
                Dim j As Integer = rnd.Next(i, n)
                Dim tmp As Integer = idx(i)
                idx(i) = idx(j)
                idx(j) = tmp
            Next

            rows = idx.Take(size).ToArray()
        End If

        Dim cor As TensorCorrelation = CorrelationOf(input, rows, store, storeRows)

        Return BetaTest.BetaTable(cor, betaSeq, config.adjacency)
    End Function

    ''' <summary>
    ''' 处理单个块：相关 → 邻接 → TOM → 不相似度 → 聚类 → 剪切
    ''' </summary>
    ''' <param name="input">完整表达矩阵</param>
    ''' <param name="rows">块内基因的行下标</param>
    ''' <param name="index">块序号</param>
    ''' <param name="beta">全局软阈值幂次</param>
    ''' <param name="config">分析配置</param>
    ''' <param name="store">可选的相关矩阵存储；非 Nothing 时块内相关子矩阵从 store 切片而非重算</param>
    ''' <param name="storeRows">输入基因 → store 行号映射（<see cref="StoreCorrelation.MapGenes"/> 的产物）</param>
    ''' <returns>块分析结果</returns>
    Private Function ProcessBlock(input As Matrix, rows As Integer(), index As Integer,
                                  beta As Double, config As WGCNAConfig,
                                  Optional store As CorStore.CorrelationMatrixStore = Nothing,
                                  Optional storeRows As Integer() = Nothing) As BlockResult

        Dim t As New Dictionary(Of String, Double)
        Dim sw As New Stopwatch()
        Dim keys As String() = rows.Select(Function(i) input.expression(i).geneID).ToArray()
        Dim n As Integer = rows.Length

        If config.verbose Then
            Call VBDebugger.EchoLine($"  [block {index}] {n} genes")
        End If

        ' --- 相关矩阵（预计算 store 切片，或表达矩阵 GEMM 重算）
        sw.Restart()

        Dim cor As TensorCorrelation = CorrelationOf(input, rows, store, storeRows)

        sw.Stop()

        t("cor") = sw.ElapsedMilliseconds

        ' --- 邻接矩阵
        sw.Restart()

        Dim absCor As Double() = WeightedNetwork.AbsCorrelation(cor)
        Dim adj As Double() = WeightedNetwork.BuildAdjacency(absCor, beta, config.adjacency)
        Dim k As Double() = WeightedNetwork.ConnectivityOf(adj, n)

        absCor = Nothing
        sw.Stop()

        t("adjacency") = sw.ElapsedMilliseconds

        ' --- TOM（GEMM + 融合组合）
        sw.Restart()

        Dim tomBuf As Double() = TOM.Matrix(adj, k, n)

        adj = Nothing
        sw.Stop()

        t("tom") = sw.ElapsedMilliseconds

        ' --- 不相似度（dist = 1 - TOM）
        sw.Restart()

        Call TensorOps.DissimilarityInPlace(tomBuf)

        sw.Stop()

        t("dissim") = sw.ElapsedMilliseconds

        ' --- 层次聚类（UPGMA，与 R 的 hclust(method="average") 等价）
        sw.Restart()

        Dim dendro As HclustResult = AverageLinkage.Hclust(tomBuf, n)
        Dim tree As Cluster = AverageLinkage.Cluster(dendro, keys)

        sw.Stop()

        t("hclust") = sw.ElapsedMilliseconds

        ' --- 模块剪切
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
            Dim labels As Integer() = DynamicTreeCut.CutreeHybrid(dendro, keys, tomBuf, options)

            modules = DynamicTreeCut.ToModules(labels, keys, prefix:="M")
        Else
            modules = StaticCut.Cutree(tree, config.distCut)
        End If

        tomBuf = Nothing
        sw.Stop()

        t("cut") = sw.ElapsedMilliseconds

        If config.verbose Then
            Call VBDebugger.EchoLine($"  [block {index}] {modules.Count} module(s), " &
                                     $"cor={t("cor")}ms, tom={t("tom")}ms, hclust={t("hclust")}ms, cut={t("cut")}ms")
        End If

        Return New BlockResult With {
            .index = index,
            .keys = keys,
            .K = k,
            .tree = tree,
            .modules = modules,
            .timing = t
        }
    End Function

    ''' <summary>
    ''' 计算给定基因子集的相关矩阵：优先从预计算 store 切片，否则从表达矩阵 GEMM 重算。
    ''' </summary>
    ''' <param name="input">完整表达矩阵</param>
    ''' <param name="rows">子集内基因在输入矩阵中的行下标</param>
    ''' <param name="store">可选的相关矩阵存储（校验通过时非 Nothing）</param>
    ''' <param name="storeRows">输入基因 → store 行号映射</param>
    ''' <returns>相关矩阵对象（行主 n x n）</returns>
    Private Function CorrelationOf(input As Matrix, rows As Integer(),
                                   store As CorStore.CorrelationMatrixStore,
                                   storeRows As Integer()) As TensorCorrelation
        If store IsNot Nothing AndAlso storeRows IsNot Nothing Then
            Dim blockRows(rows.Length - 1) As Integer

            For i As Integer = 0 To rows.Length - 1
                blockRows(i) = storeRows(rows(i))
            Next

            Return StoreCorrelation.Correlation(store, blockRows)
        Else
            Dim expr As Double()() = rows _
                .Select(Function(i) input.expression(i).experiments) _
                .ToArray()

            Return TensorCorrelation.Create(expr)
        End If
    End Function

    ''' <summary>
    ''' 打开并校验预计算相关矩阵存储；任何不匹配都释放句柄并回退（按现状从表达矩阵重算）。
    ''' </summary>
    ''' <param name="config">分析配置（读取 <see cref="WGCNAConfig.correlationStore"/> 目录）</param>
    ''' <param name="geneIds">过滤后输入的基因 ID（行序）</param>
    ''' <param name="nSamples">输入的样本数</param>
    ''' <returns>
    ''' 句柄 + 基因行号映射 + 来源标记 + 打开耗时（毫秒）。
    ''' 未配置、目录/索引缺失、基因不匹配或样本数不符时句柄为 Nothing、来源为 <c>expression</c>。
    ''' </returns>
    Private Function OpenCorrelationStore(config As WGCNAConfig, geneIds As String(), nSamples As Integer) _
        As (handle As CorrelationStoreHandle, rows As Integer(), source As String, ms As Double)

        If String.IsNullOrEmpty(config.correlationStore) Then
            Return (Nothing, Nothing, "expression", 0.0)
        End If

        Dim sw As New Stopwatch()
        Call sw.Start()

        Try
            Dim handle As CorrelationStoreHandle = CorrelationStoreHandle.Open(config.correlationStore)
            Dim store As CorStore.CorrelationMatrixStore = handle.Store
            Dim rows As Integer() = StoreCorrelation.MapGenes(store, geneIds)

            If rows Is Nothing Then
                ' 诊断信息先于句柄释放收集（字段在 Dispose 后仍可读，但显式收集更稳）
                Dim sizeN As Integer = store.size
                Dim missing As String() = StoreCorrelation.MissingGenes(store, geneIds)
                Dim dupes As String() = StoreCorrelation.DuplicateGenes(geneIds)

                Call handle.Dispose()
                Call sw.Stop()

                Call ($"WGCNA blockwise: correlation store '{config.correlationStore}' 与输入基因不匹配 " &
                      $"(store N={sizeN}，输入缺失 {missing.Length} 个、重复 {dupes.Length} 个)，回退到表达矩阵重算").warning

                Return (Nothing, Nothing, "expression", sw.ElapsedMilliseconds)
            End If

            If store.SampleN <> nSamples Then
                Dim sampleN As Integer = store.SampleN

                Call handle.Dispose()
                Call sw.Stop()

                Call ($"WGCNA blockwise: correlation store '{config.correlationStore}' 样本数不符 " &
                      $"(store={sampleN}, input={nSamples})，回退到表达矩阵重算").warning

                Return (Nothing, Nothing, "expression", sw.ElapsedMilliseconds)
            End If

            If config.verbose Then
                Call VBDebugger.EchoLine($"WGCNA blockwise: correlation store '{config.correlationStore}' " &
                                         $"(kind={handle.Kind}, N={store.size}, SampleN={store.SampleN}) " &
                                         $"覆盖全部 {geneIds.Length} 个基因，复用预计算相关矩阵")
            End If

            Call sw.Stop()

            Return (handle, rows, $"{handle.Kind}-store", sw.ElapsedMilliseconds)
        Catch ex As Exception
            Call sw.Stop()

            Call $"WGCNA blockwise: 打开相关矩阵存储 '{config.correlationStore}' 失败：{ex.Message}，回退到表达矩阵重算".warning

            Return (Nothing, Nothing, "expression", sw.ElapsedMilliseconds)
        End Try
    End Function
End Module
