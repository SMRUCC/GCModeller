#Region "Microsoft.VisualBasic::DynamicTreeCut, annotations\WGCNA\WGCNA\Algorithm\DynamicTreeCut.vb"

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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.DataMining.HierarchicalClustering
Imports Microsoft.VisualBasic.Linq
Imports std = System.Math

''' <summary>
''' R 包 dynamicTreeCut 中 <c>cutreeHybrid</c> 的 VB 实现（动态自适应剪枝）
''' </summary>
''' <remarks>
''' 与「按固定高度/总距离百分比一刀切」的静态剪切不同，动态剪切会自顶向下地
''' 考察每一个分支，用两条判据决定它是「成为一个模块」还是「继续往下拆」：
''' 
''' <list type="bullet">
''' <item><description>
''' <b>Core scatter 判据</b>：分支核心（按 <c>.CoreSize</c> 取出的代表性子集）内部的
''' 平均不相似度必须足够小，否则说明该分支太"松散"，不能算一个模块。
''' </description></item>
''' <item><description>
''' <b>Gap 判据</b>：分支的合并高度与核心散布之间必须有足够大的间隔，
''' 否则说明分支与其父分支之间没有明显的分离。
''' </description></item>
''' </list>
''' 
''' <para>
''' 参数 <c>deepSplit</c>（0~4）通过
''' <c>defMCS = (0.64, 0.73, 0.82, 0.91, 0.95)</c> 与
''' <c>defMG  = (1 - defMCS) * 3/4</c> 同时调节这两条判据的松紧：
''' 值越大，允许的散布越大、要求的间隔越小，因而切出的模块越多越小。
''' </para>
''' 
''' <para>
''' 本实现以 <c>dynamicTreeCut 1.63-1</c> 的 <c>cutreeHybrid</c> 为蓝本，
''' 保持 <c>pamStage=TRUE</c>、<c>useMedoids=FALSE</c>、
''' <c>pamRespectsDendro=TRUE</c>、<c>respectSmallClusters=TRUE</c> 的默认路径。
''' </para>
''' </remarks>
Public Module DynamicTreeCut

    ''' <summary>
    ''' 动态剪切的默认参数集
    ''' </summary>
    Public Class CutOptions

        ''' <summary>模块的最小基因数（对应 R 的 minClusterSize）</summary>
        ''' <returns>最小模块大小，默认 20</returns>
        Public Property minClusterSize As Integer = 20

        ''' <summary>
        ''' 拆分深度（0~4）。越大切出的模块越多越小。
        ''' </summary>
        ''' <returns>深度档位，默认 2</returns>
        Public Property deepSplit As Integer = 2

        ''' <summary>
        ''' 剪切高度上限。Nothing（NaN）时按 <c>0.99*(max(h)-refHeight)+refHeight</c> 自动确定。
        ''' </summary>
        ''' <returns>剪切高度</returns>
        Public Property cutHeight As Double = Double.NaN

        ''' <summary>是否执行 PAM 阶段（把未标注基因分配到最近的已有模块）</summary>
        ''' <returns>默认 True</returns>
        Public Property pamStage As Boolean = True

        ''' <summary>PAM 阶段是否尊重树结构（只在同分支的模块里找最近邻）</summary>
        ''' <returns>默认 True</returns>
        Public Property pamRespectsDendro As Boolean = True

        ''' <summary>是否保留未达 minClusterSize 的小分支作为独立模块</summary>
        ''' <returns>默认 True</returns>
        Public Property respectSmallClusters As Boolean = True

        ''' <summary>是否输出进度日志</summary>
        ''' <returns>默认 False</returns>
        Public Property verbose As Boolean = False

        ''' <summary>
        ''' deepSplit 对应的默认最大核心散布
        ''' </summary>
        ''' <returns>五个档位的散布阈值</returns>
        Public Shared ReadOnly Property DefaultMaxCoreScatter As Double()
            Get
                Return New Double() {0.64, 0.73, 0.82, 0.91, 0.95}
            End Get
        End Property
    End Class

    ''' <summary>
    ''' 对层次聚类树做动态剪切，返回每个基因的模块标签
    ''' </summary>
    ''' <param name="tree">层次聚类树（由 <c>performClustering</c> 得到）</param>
    ''' <param name="keys">基因 ID 列表，顺序必须与 <paramref name="dist"/> 的行列一致</param>
    ''' <param name="dist">行优先的 n x n 不相似度矩阵（如 1 - TOM）</param>
    ''' <param name="options">剪切参数，Nothing 时使用默认值</param>
    ''' <returns>
    ''' 长度为 n 的标签数组：<c>0</c> 表示未归入任何模块（对应 WGCNA 的灰色），
    ''' <c>1..K</c> 为模块编号，按模块大小降序排列。
    ''' </returns>
    Public Function CutreeHybrid(tree As Cluster, keys As String(), dist As Double(),
                                 Optional options As CutOptions = Nothing) As Integer()
        If options Is Nothing Then options = New CutOptions()

        Dim n As Integer = keys.Length

        If n < 2 Then
            Return New Integer(n - 1) {}
        End If

        Dim dendro As Dendrogram = Dendrogram.FromTree(tree, keys)
        Dim nMerge As Integer = dendro.height.Length

        If nMerge < 1 Then
            Return New Integer(n - 1) {}
        End If

        Dim minClusterSize As Integer = options.minClusterSize
        If minClusterSize < 2 Then minClusterSize = 2

        ' ---- 参考高度与剪切高度 -------------------------------------------------
        Dim sortedHeight As Double() = CType(dendro.height.Clone(), Double())

        Call Array.Sort(sortedHeight)

        Dim refMerge As Integer = CInt(std.Round(nMerge * 0.05))

        If refMerge < 1 Then refMerge = 1
        If refMerge > nMerge Then refMerge = nMerge

        Dim refHeight As Double = sortedHeight(refMerge - 1)
        Dim maxHeight As Double = sortedHeight(nMerge - 1)
        Dim cutHeight As Double = options.cutHeight

        If Double.IsNaN(cutHeight) Then
            cutHeight = 0.99 * (maxHeight - refHeight) + refHeight
        ElseIf cutHeight > maxHeight Then
            cutHeight = maxHeight
        End If

        Dim nMergeBelowCut As Integer = 0

        For Each h As Double In dendro.height
            If h <= cutHeight Then nMergeBelowCut += 1
        Next

        If nMergeBelowCut < minClusterSize Then
            Return New Integer(n - 1) {}
        End If

        ' ---- deepSplit 参数 -----------------------------------------------------
        Dim defMCS As Double() = CutOptions.DefaultMaxCoreScatter
        Dim defMG As Double() = defMCS.Select(Function(x) (1 - x) * 3 / 4).ToArray()
        Dim ds As Integer = options.deepSplit

        If ds < 0 Then ds = 0
        If ds > 4 Then ds = 4

        Dim maxCoreScatter As Double = defMCS(ds)
        Dim minGap As Double = defMG(ds)
        Dim maxAbsCoreScatter As Double = refHeight + maxCoreScatter * (cutHeight - refHeight)
        Dim minAbsGap As Double = minGap * (cutHeight - refHeight)
        Dim minAbsSplitHeight As Double = refHeight

        If options.verbose Then
            Call VBDebugger.EchoLine($"cutreeHybrid: n={n}, cutHeight={cutHeight.ToString("F4")}, refHeight={refHeight.ToString("F4")}, maxCoreScatter={maxCoreScatter}, minGap={minGap.ToString("F4")}")
        End If

        ' ---- 阶段一：遍历合并树，构造分支 ---------------------------------------
        Dim nPoints As Integer = nMerge + 1
        Dim indMergeToBranch As Integer() = New Integer(nMerge - 1) {}
        Dim onBranch As Integer() = New Integer(nPoints - 1) {}
        Dim branches As New List(Of BranchState)
        Dim rootBranch As Integer = 0

        ' 占位，使分支编号从 1 开始（与 R 一致）
        Call branches.Add(New BranchState())

        For merge As Integer = 0 To nMerge - 1
            Dim h As Double = dendro.height(merge)

            If h > cutHeight Then
                Continue For
            End If

            Dim a As Integer = dendro.merge(merge)(0)
            Dim b As Integer = dendro.merge(merge)(1)

            If a < 0 AndAlso b < 0 Then
                ' 两个叶子合并成一个新的基础分支
                Dim br As New BranchState With {
                    .isBasic = True,
                    .isTopBasic = True,
                    .size = 2,
                    .nMerge = 1,
                    .nSingletons = 2
                }

                Call br.singletons.Add(LeafIndex(a))
                Call br.singletons.Add(LeafIndex(b))
                Call br.singletonHeights.Add(h)
                Call br.singletonHeights.Add(h)
                Call br.mergingHeights.Add(h)

                Call branches.Add(br)

                rootBranch = branches.Count - 1
                indMergeToBranch(merge) = rootBranch
            ElseIf Sign(a) * Sign(b) < 0 Then
                ' 一个叶子挂到一个已有分支上
                Dim clust As Integer = indMergeToBranch(std.Max(a, b) - 1)
                Dim gene As Integer = LeafIndex(std.Min(a, b))
                Dim br As BranchState = branches(clust)

                If br.isBasic Then
                    Call br.singletons.Add(gene)
                    Call br.singletonHeights.Add(h)
                    br.nSingletons += 1
                Else
                    onBranch(gene) = clust
                End If

                br.size += 1
                br.nMerge += 1
                Call br.mergingHeights.Add(h)

                indMergeToBranch(merge) = clust
                rootBranch = clust
            Else
                ' 两个分支的合并：判断是否真的合并
                Dim clusts As Integer() = {indMergeToBranch(a - 1), indMergeToBranch(b - 1)}
                Dim sizes0 As Integer() = {branches(clusts(0)).size, branches(clusts(1)).size}
                Dim small As Integer
                Dim large As Integer
                Dim sizes As Integer()

                If sizes0(0) <= sizes0(1) Then
                    small = clusts(0) : large = clusts(1) : sizes = sizes0
                Else
                    small = clusts(1) : large = clusts(0) : sizes = {sizes0(1), sizes0(0)}
                End If

                Dim smAveDist As Double = If(branches(small).isBasic, CoreScatter(branches(small), minClusterSize, dist, n), 0)
                Dim lgAveDist As Double = If(branches(large).isBasic, CoreScatter(branches(large), minClusterSize, dist, n), 0)
                Dim doMerge As Boolean = False
                Dim smallerFailSize As Boolean = False

                If branches(small).isBasic AndAlso
                    FailCount(branches(small).size, smAveDist, h, minClusterSize, maxAbsCoreScatter, minAbsGap, minAbsSplitHeight) > 0 Then

                    doMerge = True
                    smallerFailSize = Not (smAveDist > maxAbsCoreScatter OrElse (h - smAveDist) < minAbsGap)
                ElseIf branches(large).isBasic AndAlso
                    FailCount(branches(large).size, lgAveDist, h, minClusterSize, maxAbsCoreScatter, minAbsGap, minAbsSplitHeight) > 0 Then

                    doMerge = True
                    smallerFailSize = Not (lgAveDist > maxAbsCoreScatter OrElse (h - lgAveDist) < minAbsGap)

                    Dim swap As Integer = small
                    small = large
                    large = swap
                    sizes = {sizes(1), sizes(0)}
                End If

                If doMerge Then
                    branches(small).failSize = smallerFailSize
                    branches(small).mergedInto = large
                    branches(small).attachHeight = h
                    branches(small).isTopBasic = False

                    Dim nss As Integer = branches(small).nSingletons
                    Dim nsl As Integer = branches(large).nSingletons

                    If branches(large).isBasic Then
                        Call branches(large).singletons.AddRange(branches(small).singletons)
                        Call branches(large).singletonHeights.AddRange(branches(small).singletonHeights)
                        branches(large).nSingletons = nss + nsl
                    Else
                        For Each gene As Integer In branches(small).singletons
                            onBranch(gene) = large
                        Next
                    End If

                    branches(large).nMerge += 1
                    Call branches(large).mergingHeights.Add(h)
                    branches(large).size = branches(small).size + branches(large).size

                    indMergeToBranch(merge) = large
                    rootBranch = large
                Else
                    If branches(large).isBasic AndAlso Not branches(small).isBasic Then
                        Dim swap As Integer = large
                        large = small
                        small = swap
                        sizes = {sizes(1), sizes(0)}
                    End If

                    If branches(large).isBasic OrElse (options.pamStage AndAlso options.pamRespectsDendro) Then
                        ' 新建一个复合分支
                        Dim br As New BranchState With {
                            .isBasic = False,
                            .isTopBasic = False,
                            .nMerge = 2,
                            .size = sizes(0) + sizes(1)
                        }

                        Call br.mergingHeights.Add(h)
                        Call br.mergingHeights.Add(h)

                        branches(large).attachHeight = h
                        branches(small).attachHeight = h
                        branches(large).mergedInto = branches.Count
                        branches(small).mergedInto = branches.Count

                        If branches(small).isBasic Then
                            Call br.basicClusters.Add(small)
                        Else
                            Call br.basicClusters.AddRange(branches(small).basicClusters)
                        End If

                        If branches(large).isBasic Then
                            Call br.basicClusters.Add(large)
                        Else
                            Call br.basicClusters.AddRange(branches(large).basicClusters)
                        End If

                        br.nBasicClusters = br.basicClusters.Count

                        Call branches.Add(br)

                        indMergeToBranch(merge) = branches.Count - 1
                        rootBranch = branches.Count - 1
                    Else
                        If branches(small).isBasic Then
                            Call branches(large).basicClusters.Add(small)
                        Else
                            Call branches(large).basicClusters.AddRange(branches(small).basicClusters)
                        End If

                        branches(large).nBasicClusters = branches(large).basicClusters.Count
                        branches(large).size += branches(small).size
                        branches(large).nMerge += 1
                        Call branches(large).mergingHeights.Add(h)

                        branches(small).attachHeight = h
                        branches(small).mergedInto = large

                        indMergeToBranch(merge) = large
                        rootBranch = large
                    End If
                End If
            End If
        Next

        Dim nBranches As Integer = branches.Count - 1

        ' ---- 阶段二：标记哪些分支是真正的模块 -----------------------------------
        Dim isCluster As Boolean() = New Boolean(nBranches) {}
        Dim smallLabels As Integer() = New Integer(nPoints - 1) {}

        For clust As Integer = 1 To nBranches
            Dim br As BranchState = branches(clust)

            If Double.IsNaN(br.attachHeight) Then br.attachHeight = cutHeight

            If br.isTopBasic Then
                Dim nCore As Integer = CoreSize(br.nSingletons, minClusterSize)
                Dim core As Integer() = br.singletons.Take(nCore).ToArray()
                Dim scatter As Double = CoreScatter(core, dist, n)

                isCluster(clust) = (br.size >= minClusterSize) AndAlso
                                   (scatter < maxAbsCoreScatter) AndAlso
                                   (br.attachHeight - scatter > minAbsGap)
            End If

            If br.failSize Then
                For Each gene As Integer In br.singletons
                    smallLabels(gene) = clust
                Next
            End If
        Next

        If Not options.respectSmallClusters Then
            smallLabels = New Integer(nPoints - 1) {}
        End If

        ' ---- 阶段三：给模块分配标签 ---------------------------------------------
        Dim colors As Integer() = New Integer(nPoints - 1) {}
        Dim coreLabels As Integer() = New Integer(nPoints - 1) {}
        Dim branchLabels As Integer() = New Integer(nBranches) {}
        Dim color As Integer = 0

        For clust As Integer = 1 To nBranches
            If Not isCluster(clust) Then Continue For

            color += 1

            For Each gene As Integer In branches(clust).singletons
                colors(gene) = color
                smallLabels(gene) = 0
            Next

            Dim nCore As Integer = CoreSize(branches(clust).nSingletons, minClusterSize)

            For i As Integer = 0 To nCore - 1
                If i < branches(clust).singletons.Count Then
                    coreLabels(branches(clust).singletons(i)) = color
                End If
            Next

            branchLabels(clust) = color
        Next

        Dim nProperLabels As Integer = color

        ' ---- 阶段四：PAM 阶段，把未标注基因分配到最近的模块 ---------------------
        If options.pamStage AndAlso nProperLabels > 0 AndAlso colors.Any(Function(c) c = 0) Then
            colors = PamStage(colors, smallLabels, onBranch, branches, branchLabels,
                              dist, n, nProperLabels, cutHeight,
                              options.pamRespectsDendro, options.respectSmallClusters)

            For i As Integer = 0 To colors.Length - 1
                If colors(i) < 0 Then colors(i) = 0
            Next
        End If

        Return RenumberLabels(colors)
    End Function

    ''' <summary>
    ''' 统计某分支「触发合并」的判据条数（R 中 SmallerScores[-1] 的和）
    ''' </summary>
    Private Function FailCount(size As Integer, aveDist As Double, h As Double,
                               minClusterSize As Integer, maxAbsCoreScatter As Double,
                               minAbsGap As Double, minAbsSplitHeight As Double) As Integer
        Dim n As Integer = 0

        If size < minClusterSize Then n += 1
        If aveDist > maxAbsCoreScatter Then n += 1
        If (h - aveDist) < minAbsGap Then n += 1
        If h < minAbsSplitHeight Then n += 1

        Return n
    End Function

    Private Function Sign(x As Integer) As Integer
        Return If(x < 0, -1, 1)
    End Function

    ''' <summary>
    ''' R 中 <c>merge</c> 的叶子约定：负值为 -(观测下标)，还原成 0 基下标
    ''' </summary>
    Private Function LeafIndex(x As Integer) As Integer
        Return -x - 1
    End Function

    ''' <summary>
    ''' 分支核心大小：R dynamicTreeCut 的 <c>.CoreSize</c>
    ''' </summary>
    Private Function CoreSize(branchSize As Integer, minClusterSize As Integer) As Integer
        Dim baseCoreSize As Double = minClusterSize / 2 + 1

        If baseCoreSize < branchSize Then
            Return CInt(baseCoreSize + std.Sqrt(branchSize - baseCoreSize))
        Else
            Return branchSize
        End If
    End Function

    ''' <summary>
    ''' 分支核心内部的平均不相似度（R 中的 CoreScatter）
    ''' </summary>
    ''' <remarks>
    ''' R 的算法是 <c>mean(colSums(distM[Core, Core])/(nCore-1))</c>，
    ''' 由于对角元为 0，这等价于「核心内所有两两距离之和 / (nCore*(nCore-1))」。
    ''' </remarks>
    Private Function CoreScatter(core As Integer(), dist As Double(), n As Integer) As Double
        Dim cs As Integer = core.Length

        If cs < 2 Then Return 0

        Dim sum As Double = 0

        For i As Integer = 0 To cs - 1
            Dim offset As Integer = core(i) * n

            For j As Integer = 0 To cs - 1
                sum += dist(offset + core(j))
            Next
        Next

        Return sum / (cs * (cs - 1))
    End Function

    ''' <summary>
    ''' 取分支核心并计算其散布
    ''' </summary>
    Private Function CoreScatter(br As BranchState, minClusterSize As Integer, dist As Double(), n As Integer) As Double
        Dim nCore As Integer = CoreSize(br.nSingletons, minClusterSize)
        Dim core As Integer() = br.singletons.Take(nCore).ToArray()

        Return CoreScatter(core, dist, n)
    End Function

    ''' <summary>
    ''' PAM 阶段：把小分支与未标注对象分配到最近的模块
    ''' </summary>
    Private Function PamStage(colors As Integer(), smallLabels As Integer(), onBranch As Integer(),
                              branches As List(Of BranchState), branchLabels As Integer(),
                              dist As Double(), n As Integer, nProperLabels As Integer,
                              maxPamDist As Double, pamRespectsDendro As Boolean,
                              respectSmallClusters As Boolean) As Integer()

        Dim out As Integer() = CType(colors.Clone(), Integer())

        ' 每个模块的直径：模块内各点平均距离的最大值
        Dim clusterDiam As Double() = New Double(nProperLabels) {}
        Dim members As List(Of Integer)() = New List(Of Integer)(nProperLabels) {}

        For c As Integer = 1 To nProperLabels
            members(c) = New List(Of Integer)()
        Next

        For i As Integer = 0 To n - 1
            If out(i) > 0 Then members(out(i)).Add(i)
        Next

        For c As Integer = 1 To nProperLabels
            Dim inc As List(Of Integer) = members(c)

            If inc.Count > 1 Then
                Dim maxAve As Double = 0

                For Each i As Integer In inc
                    Dim offset As Integer = i * n
                    Dim s As Double = 0

                    For Each j As Integer In inc
                        s += dist(offset + j)
                    Next

                    Dim ave As Double = s / (inc.Count - 1)

                    If ave > maxAve Then maxAve = ave
                Next

                clusterDiam(c) = maxAve
            End If
        Next

        Dim colorsX As Integer() = CType(out.Clone(), Integer())

        ' 先处理未达到 minClusterSize 的小分支
        If respectSmallClusters Then
            Dim groups As New Dictionary(Of Integer, List(Of Integer))

            For i As Integer = 0 To n - 1
                If smallLabels(i) <> 0 Then
                    If Not groups.ContainsKey(smallLabels(i)) Then
                        groups(smallLabels(i)) = New List(Of Integer)()
                    End If

                    Call groups(smallLabels(i)).Add(i)
                End If
            Next

            For Each kv In groups
                Dim inCluster As List(Of Integer) = kv.Value
                Dim onBr As Integer = onBranch(inCluster(0))

                If onBr <= 0 Then Continue For

                Dim labelsOnBranch As Integer() = branches(onBr).basicClusters _
                    .Select(Function(b) branchLabels(b)) _
                    .Where(Function(l) l > 0) _
                    .Distinct _
                    .ToArray()

                If labelsOnBranch.Length = 0 Then Continue For

                Dim useObjects As Integer() = Enumerable _
                    .Range(0, n) _
                    .Where(Function(i) labelsOnBranch.Contains(colorsX(i))) _
                    .ToArray()

                If useObjects.Length = 0 Then Continue For

                ' 对每个候选对象求「到该小分支的平均距离」，再按模块求均值
                Dim meanDist As New Dictionary(Of Integer, Double)
                Dim meanCount As New Dictionary(Of Integer, Double)

                For Each obj As Integer In useObjects
                    Dim s As Double = 0

                    For Each i As Integer In inCluster
                        s += dist(i * n + obj)
                    Next

                    Dim c As Integer = colorsX(obj)
                    Dim d As Double = s / inCluster.Count

                    If Not meanDist.ContainsKey(c) Then
                        meanDist(c) = 0
                        meanCount(c) = 0
                    End If

                    meanDist(c) += d
                    meanCount(c) += 1
                Next

                Dim nearest As Integer = -1
                Dim nearestDist As Double = Double.PositiveInfinity

                For Each kv2 In meanDist
                    Dim d As Double = kv2.Value / meanCount(kv2.Key)

                    If d < nearestDist Then
                        nearestDist = d
                        nearest = kv2.Key
                    End If
                Next

                If nearest > 0 AndAlso (nearestDist < clusterDiam(nearest) OrElse nearestDist < maxPamDist) Then
                    For Each i As Integer In inCluster
                        out(i) = nearest
                    Next
                Else
                    For Each i As Integer In inCluster
                        out(i) = -1
                    Next
                End If
            Next
        End If

        ' 再处理依然未标注的散点
        Dim unlabeled As New List(Of Integer)

        For i As Integer = 0 To n - 1
            If out(i) = 0 Then unlabeled.Add(i)
        Next

        If unlabeled.Count > 0 Then
            If pamRespectsDendro Then
                For Each obj As Integer In unlabeled
                    Dim onBr As Integer = onBranch(obj)

                    If onBr <= 0 Then Continue For

                    Dim labelsOnBranch As Integer() = branches(onBr).basicClusters _
                        .Select(Function(b) branchLabels(b)) _
                        .Where(Function(l) l > 0) _
                        .Distinct _
                        .ToArray()

                    If labelsOnBranch.Length = 0 Then Continue For

                    Dim nearest As Integer = -1
                    Dim nearestDist As Double = Double.PositiveInfinity
                    Dim acc As New Dictionary(Of Integer, Double)
                    Dim cnt As New Dictionary(Of Integer, Double)

                    For i As Integer = 0 To n - 1
                        Dim c As Integer = colorsX(i)

                        If c <= 0 OrElse Not labelsOnBranch.Contains(c) Then Continue For

                        If Not acc.ContainsKey(c) Then
                            acc(c) = 0
                            cnt(c) = 0
                        End If

                        acc(c) += dist(i * n + obj)
                        cnt(c) += 1
                    Next

                    For Each kv In acc
                        Dim d As Double = kv.Value / cnt(kv.Key)

                        If d < nearestDist Then
                            nearestDist = d
                            nearest = kv.Key
                        End If
                    Next

                    If nearest > 0 AndAlso (nearestDist < clusterDiam(nearest) OrElse nearestDist < maxPamDist) Then
                        out(obj) = nearest
                    End If
                Next
            Else
                For Each obj As Integer In unlabeled
                    Dim nearest As Integer = -1
                    Dim nearestDist As Double = Double.PositiveInfinity
                    Dim acc As New Dictionary(Of Integer, Double)
                    Dim cnt As New Dictionary(Of Integer, Double)

                    For i As Integer = 0 To n - 1
                        Dim c As Integer = colorsX(i)

                        If c <= 0 Then Continue For

                        If Not acc.ContainsKey(c) Then
                            acc(c) = 0
                            cnt(c) = 0
                        End If

                        acc(c) += dist(i * n + obj)
                        cnt(c) += 1
                    Next

                    For Each kv In acc
                        Dim d As Double = kv.Value / cnt(kv.Key)

                        If d < nearestDist Then
                            nearestDist = d
                            nearest = kv.Key
                        End If
                    Next

                    If nearest > 0 AndAlso (nearestDist < clusterDiam(nearest) OrElse nearestDist < maxPamDist) Then
                        out(obj) = nearest
                    End If
                Next
            End If
        End If

        Return out
    End Function

    ''' <summary>
    ''' 把模块标签按大小降序重新编号（未标注保持 0）
    ''' </summary>
    Private Function RenumberLabels(colors As Integer()) As Integer()
        Dim levels As Integer() = colors.Distinct().OrderBy(Function(x) x).ToArray()
        Dim levelIndex As New Dictionary(Of Integer, Integer)

        For i As Integer = 0 To levels.Length - 1
            levelIndex(levels(i)) = i + 1
        Next

        Dim numLabs As Integer() = colors.Select(Function(c) levelIndex(c)).ToArray()
        Dim sizes As Integer() = New Integer(levels.Length) {}

        For Each l As Integer In numLabs
            sizes(l - 1) += 1
        Next

        Dim unlabeledExist As Boolean = colors.Any(Function(c) c = 0)
        Dim sizeRank As Integer() = New Integer(levels.Length) {}
        Dim startIdx As Integer = If(unlabeledExist, 1, 0)
        Dim ranks As Integer() = RankDescending(sizes, startIdx, levels.Length - 1)

        For i As Integer = 0 To levels.Length - 1
            sizeRank(i) = If(unlabeledExist AndAlso i = 0, 1, ranks(i))
        Next

        Dim offset As Integer = If(unlabeledExist, 1, 0)

        Return numLabs.Select(Function(l) sizeRank(l - 1) - offset).ToArray()
    End Function

    ''' <summary>
    ''' 对 <paramref name="sizes"/> 的 [from..to] 区间按「降序、同值取先」计算秩
    ''' </summary>
    Private Function RankDescending(sizes As Integer(), from As Integer, [to] As Integer) As Integer()
        Dim n As Integer = [to] - from + 1
        Dim order As Integer() = Enumerable.Range(from, n).OrderBy(Function(i) -sizes(i)).ToArray()
        Dim rank As Integer() = New Integer(sizes.Length - 1) {}

        For r As Integer = 0 To n - 1
            rank(order(r)) = r + 1
        Next

        Return rank
    End Function

    ''' <summary>
    ''' 把动态剪切的标签数组转成「模块名 → 基因列表」的字典
    ''' </summary>
    ''' <param name="labels"><see cref="CutreeHybrid"/> 的输出</param>
    ''' <param name="keys">基因 ID 列表，顺序与 <paramref name="labels"/> 一致</param>
    ''' <param name="prefix">模块名前缀，默认 <c>M</c></param>
    ''' <returns>模块字典；未标注（标签 0）的基因会被丢弃</returns>
    Public Function ToModules(labels As Integer(), keys As String(), Optional prefix As String = "M") As Dictionary(Of String, String())
        Dim lists As New Dictionary(Of String, List(Of String))

        For i As Integer = 0 To labels.Length - 1
            If labels(i) <= 0 Then Continue For

            Dim name As String = $"{prefix}{labels(i)}"

            If Not lists.ContainsKey(name) Then
                lists(name) = New List(Of String)()
            End If

            Call lists(name).Add(keys(i))
        Next

        Dim result As New Dictionary(Of String, String())

        For Each kv In lists
            result(kv.Key) = kv.Value.ToArray()
        Next

        Return result
    End Function

#Region "内部数据结构"

    ''' <summary>
    ''' hclust 风格的合并矩阵（与 R 的 <c>dendro$merge</c> / <c>dendro$height</c> 同构）
    ''' </summary>
    Private Class Dendrogram
        ''' <summary>合并矩阵，负值表示叶子（-(下标+1)），正值表示第 k 次合并</summary>
        Public merge As Integer()()
        ''' <summary>每次合并的高度</summary>
        Public height As Double()

        ''' <summary>
        ''' 由 <see cref="Cluster"/> 树还原出 hclust 的 merge/height
        ''' </summary>
        ''' <param name="tree">凝聚层次聚类得到的树</param>
        ''' <param name="keys">叶子名称列表（决定叶子在原始数据中的下标）</param>
        ''' <returns>合并矩阵与高度向量</returns>
        ''' <remarks>
        ''' 这里用<b>迭代式后序遍历</b>而不是递归：退化树（链状合并）的深度可能达到 O(n)，
        ''' 递归会直接栈溢出。
        ''' </remarks>
        Public Shared Function FromTree(tree As Cluster, keys As String()) As Dendrogram
            Dim idx As New Dictionary(Of String, Integer)

            For i As Integer = 0 To keys.Length - 1
                If Not idx.ContainsKey(keys(i)) Then
                    idx(keys(i)) = i
                End If
            Next

            Dim merge As New List(Of Integer())()
            Dim height As New List(Of Double)()
            Dim stack As New Stack(Of Frame)()

            Call stack.Push(New Frame With {.node = tree, .children = tree.Children.ToArray()})

            While stack.Count > 0
                Dim f As Frame = stack.Peek()

                If f.next < f.children.Length Then
                    Dim child As Cluster = f.children(f.next)

                    f.next += 1

                    If child.isLeaf Then
                        Call f.childIds.Add(-(idx(child.Name) + 1))
                    Else
                        Call stack.Push(New Frame With {.node = child, .children = child.Children.ToArray()})
                    End If
                Else
                    Call stack.Pop()

                    If f.childIds.Count < 2 Then
                        ' 退化情形：只有一个子节点，直接把它的 id 透传上去
                        Dim only As Integer = If(f.childIds.Count = 1, f.childIds(0), -(idx(f.node.Name) + 1))

                        If stack.Count > 0 Then
                            Call stack.Peek().childIds.Add(only)
                        End If

                        Continue While
                    End If

                    Call merge.Add(New Integer() {f.childIds(0), f.childIds(1)})
                    Call height.Add(f.node.Distance.Distance)

                    If stack.Count > 0 Then
                        Call stack.Peek().childIds.Add(merge.Count)
                    End If
                End If
            End While

            Return New Dendrogram With {
                .merge = merge.ToArray(),
                .height = height.ToArray()
            }
        End Function

        Private Class Frame
            Public node As Cluster
            Public children As Cluster()
            Public childIds As New List(Of Integer)()
            Public [next] As Integer = 0
        End Class
    End Class

    ''' <summary>
    ''' cutreeHybrid 的分支状态（对应 R 里的各个 branch.* 向量）
    ''' </summary>
    Private Class BranchState
        Public Property isBasic As Boolean = True
        Public Property isTopBasic As Boolean = True
        Public Property failSize As Boolean = False
        Public Property size As Integer = 2
        Public Property nMerge As Integer = 1
        Public Property nSingletons As Integer = 2
        Public Property nBasicClusters As Integer = 0
        Public Property mergedInto As Integer = 0
        Public Property attachHeight As Double = Double.NaN
        Public ReadOnly singletons As New List(Of Integer)()
        Public ReadOnly basicClusters As New List(Of Integer)()
        Public ReadOnly mergingHeights As New List(Of Double)()
        Public ReadOnly singletonHeights As New List(Of Double)()
    End Class

#End Region
End Module
