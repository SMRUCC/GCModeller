#Region "Microsoft.VisualBasic::WGCNAConfig, annotations\WGCNA\WGCNA\WGCNAConfig.vb"

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

Imports Microsoft.VisualBasic.Linq

''' <summary>
''' 树剪切方式
''' </summary>
Public Enum TreeCutMethod
    ''' <summary>
    ''' 动态自适应剪枝（<see cref="DynamicTreeCut"/>），与 GNU R 的 dynamicTreeCut::cutreeHybrid 对应
    ''' </summary>
    Dynamic = 0
    ''' <summary>
    ''' 静态剪切（<see cref="StaticCut"/>），按树总距离的百分比一刀切（WGCNA 项目原有的方式）
    ''' </summary>
    <ComponentModel.Description("Static")>
    StaticCut = 1
End Enum

''' <summary>
''' WGCNA 分析配置
''' </summary>
''' <remarks>
''' 这是新增的分块（blockwise）分析入口 <c>Analysis.RunBlockwise</c> 的统一参数对象。
''' 既有的 <c>Analysis.Run</c> 重载内部使用一个默认实例，因此不改变原有调用方的行为。
''' 
''' <para>
''' 命名与默认值尽量对齐 GNU R WGCNA 的 <c>blockwiseModules</c>，便于两边做对照：
''' <c>maxBlockSize</c>、<c>minModuleSize</c>(minClusterSize)、<c>deepSplit</c>、
''' <c>mergeCutHeight</c> 都可以在 R 侧找到同名（或语义相同）的参数。
''' </para>
''' </remarks>
Public Class WGCNAConfig

    ''' <summary>
    ''' 单个块的最大基因数。超过该规模时先做预聚类分块。
    ''' </summary>
    ''' <returns>块大小上限，默认 5000（与 R WGCNA 的 maxBlockSize 默认值一致）</returns>
    ''' <remarks>
    ''' 块内存占用约为 <c>3 * maxBlockSize^2 * 8</c> 字节（相关矩阵 + TOM + pdist）。
    ''' 5000 时约 500 MB，是本流程的内存峰值主要来源。
    ''' </remarks>
    Public Property maxBlockSize As Integer = 5000

    ''' <summary>
    ''' 邻接矩阵的边截断阈值
    ''' </summary>
    ''' <returns>阈值，默认 0.6</returns>
    Public Property adjacency As Double = 0.6

    ''' <summary>
    ''' 软阈值幂次。NaN 时先在抽样基因上自动做 beta 扫描估计。
    ''' </summary>
    ''' <returns>软阈值幂次</returns>
    Public Property power As Double = Double.NaN

    ''' <summary>
    ''' beta 扫描的候选序列。Nothing 时使用 <c>seq(1,10) 拼接 seq(11,30,by=2)</c>。
    ''' </summary>
    ''' <returns>候选幂次数组</returns>
    Public Property betaSeq As Double() = Nothing

    ''' <summary>
    ''' 用于估计全局 beta 的抽样基因数（0 表示取 min(nGenes, 5000)）
    ''' </summary>
    ''' <returns>抽样基因数</returns>
    Public Property betaSampleSize As Integer = 0

    ''' <summary>
    ''' 树剪切方式
    ''' </summary>
    ''' <returns>默认 <see cref="TreeCutMethod.Dynamic"/></returns>
    Public Property treeCut As TreeCutMethod = TreeCutMethod.Dynamic

    ''' <summary>
    ''' 模块的最小基因数（对应 R 的 minClusterSize / minModuleSize）
    ''' </summary>
    ''' <returns>默认 20</returns>
    Public Property minModuleSize As Integer = 20

    ''' <summary>
    ''' 动态剪切的拆分深度（0~4），越大切出的模块越多越小
    ''' </summary>
    ''' <returns>默认 2</returns>
    Public Property deepSplit As Integer = 2

    ''' <summary>
    ''' 动态剪切的高度上限。NaN 时自动取 <c>0.99*(max(h)-refHeight)+refHeight</c>。
    ''' </summary>
    ''' <returns>剪切高度</returns>
    Public Property cutHeight As Double = Double.NaN

    ''' <summary>
    ''' 动态剪切是否执行 PAM 阶段（把未标注基因分配到最近的模块）
    ''' </summary>
    ''' <returns>默认 True</returns>
    Public Property pamStage As Boolean = True

    ''' <summary>
    ''' 静态剪切的树总距离百分比阈值（仅在 <see cref="TreeCutMethod.Static"/> 下生效）
    ''' </summary>
    ''' <returns>默认 0.6</returns>
    Public Property distCut As Double = 0.6

    ''' <summary>
    ''' 跨块模块合并的高度阈值（模块特征基因不相似度小于该值即合并）
    ''' </summary>
    ''' <returns>默认 0.15（R WGCNA 的 mergeCutHeight 默认值）</returns>
    Public Property mergeCutHeight As Double = 0.15

    ''' <summary>
    ''' 是否尝试启用 CUDA GPU 后端
    ''' </summary>
    ''' <returns>默认 False（保持 SIMD CPU，不改变既有调用方行为）</returns>
    Public Property useGpu As Boolean = False

    ''' <summary>
    ''' 同时处理的块数量上限（控制 n^2 内存峰值）
    ''' </summary>
    ''' <returns>默认 1</returns>
    Public Property maxConcurrentBlocks As Integer = 1

    ''' <summary>
    ''' 是否物化 p 值矩阵。False 时 p 值按 (i,j) 现算，可以省掉一半的 n^2 内存。
    ''' </summary>
    ''' <returns>默认 False</returns>
    Public Property computePvalue As Boolean = False

    ''' <summary>
    ''' 分块模式下是否构建网络图对象
    ''' </summary>
    ''' <returns>默认 False（大规模数据下 n^2 条边必然 OOM）</returns>
    Public Property buildGraph As Boolean = False

    ''' <summary>
    ''' 构建网络图时的最大边数（0 表示不限制）
    ''' </summary>
    ''' <returns>默认 0</returns>
    Public Property maxEdges As Integer = 0

    ''' <summary>
    ''' 基因预过滤：只保留方差最大的前 N 个基因（0 表示不过滤）
    ''' </summary>
    ''' <returns>默认 0</returns>
    Public Property filterTopN As Integer = 0

    ''' <summary>
    ''' 基因预过滤：剔除方差小于该阈值的基因（0 表示不过滤）
    ''' </summary>
    ''' <returns>默认 0</returns>
    Public Property minVariance As Double = 0

    ''' <summary>
    ''' 基因预过滤：剔除缺失率高于该阈值的基因（0 表示不过滤）
    ''' </summary>
    ''' <returns>默认 0</returns>
    Public Property maxMissingRate As Double = 0

    ''' <summary>
    ''' 预聚类（projectiveKMeans）的随机种子
    ''' </summary>
    ''' <returns>默认 12345</returns>
    Public Property randomSeed As Integer = 12345

    ''' <summary>
    ''' 是否输出各阶段的进度日志
    ''' </summary>
    ''' <returns>默认 True</returns>
    Public Property verbose As Boolean = True

    ''' <summary>
    ''' 创建一个默认配置
    ''' </summary>
    ''' <returns>默认配置实例</returns>
    Public Shared Function [Default]() As WGCNAConfig
        Return New WGCNAConfig()
    End Function

    ''' <summary>
    ''' 取得 beta 扫描的候选序列
    ''' </summary>
    ''' <returns>候选幂次数组</returns>
    Public Function GetBetaSeq() As Double()
        If betaSeq IsNot Nothing AndAlso betaSeq.Length > 0 Then
            Return betaSeq
        End If

        Dim a As Double() = Enumerable.Range(1, 10).Select(Function(i) CDbl(i)).ToArray()
        Dim b As Double() = Enumerable.Range(0, 10).Select(Function(i) CDbl(11 + i * 2)).ToArray()

        Return a.JoinIterates(b).ToArray()
    End Function
End Class
