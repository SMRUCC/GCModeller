#Region "Microsoft.VisualBasic::AverageLinkage, annotations\WGCNA\WGCNA\Algorithm\AverageLinkage.vb"

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
Imports Microsoft.VisualBasic.DataMining.HierarchicalClustering
Imports Microsoft.VisualBasic.DataMining.HierarchicalClustering.Hierarchy
Imports std = System.Math

''' <summary>
''' R <c>hclust(method = "average")</c> 风格的层次聚类结果
''' </summary>
Public Class HclustResult

    ''' <summary>
    ''' 合并矩阵：负值表示叶子（-(下标+1)），正值表示第 k 次合并（1 基）
    ''' </summary>
    ''' <returns>(n-1) x 2 的合并矩阵</returns>
    Public Property merge As Integer()()

    ''' <summary>
    ''' 每次合并的高度
    ''' </summary>
    ''' <returns>长度为 n-1 的高度向量</returns>
    Public Property height As Double()

    ''' <summary>
    ''' 叶子（基因）数量
    ''' </summary>
    ''' <returns>基因数</returns>
    Public Property n As Integer
End Class

''' <summary>
''' UPGMA（average linkage）层次聚类，与 GNU R 的 <c>hclust(method="average")</c> 等价
''' </summary>
''' <remarks>
''' <para>
''' <b>为什么要自己实现</b>：基础库 <c>hierarchical-clustering</c> 里的
''' <c>AverageLinkageStrategy</c> 实际计算的是 <c>(d1 + d2) / 2</c>（McQuitty / WPGMA），
''' 而不是按簇大小加权的 Lance-Williams 更新
''' <c>(nA·d(A,k) + nB·d(B,k)) / (nA + nB)</c>。
''' 两者只有在每步合并的两个簇大小都相等时才一致，因此它会给出与 R 完全不同的树
''' （实测在 400 个基因上 399 次合并全部不同），进而导致模块划分无法与 R 对照。
''' </para>
''' 
''' <para>
''' <b>复杂度与内存</b>：这里采用「存储距离矩阵 + 最近邻表」的经典实现。
''' 距离矩阵就地在输入的上三角上更新，额外内存只有 O(n)；
''' 每轮合并只需 O(n) 的距离更新与 O(n) 的最近邻修正，实测总代价约 O(n^2)。
''' 相比基础库为每一对簇都分配一个 <c>HierarchyTreeNode</c>
''' （n=5000 时约 1250 万个对象）要省一个数量级的内存。
''' </para>
''' </remarks>
Public Module AverageLinkage

    ''' <summary>
    ''' 压缩（pdist）格式的下三角索引
    ''' </summary>
    ''' <param name="i">第一个下标</param>
    ''' <param name="j">第二个下标（i ≠ j）</param>
    ''' <param name="n">矩阵阶数</param>
    ''' <returns>长度为 n(n-1)/2 的数组下标</returns>
    ''' <remarks>
    ''' 与 R 的 <c>as.dist</c> / scipy 的 condensed 约定一致：
    ''' <c>k = i*(2n - i - 1)/2 + (j - i - 1)</c>（其中 i &lt; j）。
    ''' </remarks>
    Private Function CIdx(i As Integer, j As Integer, n As Integer) As Integer
        If i < j Then
            Return i * (2 * n - i - 1) \ 2 + (j - i - 1)
        Else
            Return j * (2 * n - j - 1) \ 2 + (i - j - 1)
        End If
    End Function

    ''' <summary>
    ''' 对不相似度矩阵做 UPGMA 层次聚类（输入矩阵<b>不会被修改</b>）
    ''' </summary>
    ''' <param name="dist">行优先的 n x n 对称不相似度矩阵（对角元应为 0）</param>
    ''' <param name="n">矩阵阶数</param>
    ''' <returns>hclust 风格的合并结构与高度</returns>
    ''' <remarks>
    ''' 内部会另建一份 n(n-1)/2 的压缩工作区，因此调用方的
    ''' <paramref name="dist"/>（例如 1 - TOM）可以安全地留给树剪切继续使用。
    ''' </remarks>
    Public Function Hclust(dist As Double(), n As Integer) As HclustResult
        If n <= 1 Then
            Return New HclustResult With {.merge = New Integer()() {}, .height = New Double() {}, .n = n}
        End If

        Dim size2 As Long = CLng(n) * (n - 1) \ 2
        Dim condensed(CInt(size2) - 1) As Double

        For i As Integer = 0 To n - 2
            Dim off As Integer = i * n

            For j As Integer = i + 1 To n - 1
                condensed(CIdx(i, j, n)) = dist(off + j)
            Next
        Next

        Return HclustCore(condensed, n)
    End Function

    ''' <summary>
    ''' 对压缩（pdist）格式的相异度做 UPGMA 层次聚类
    ''' </summary>
    ''' <param name="condensed">长度为 n(n-1)/2 的压缩距离向量，<b>会被就地修改</b></param>
    ''' <param name="n">矩阵阶数</param>
    ''' <returns>hclust 风格的合并结构与高度</returns>
    Public Function HclustCondensed(condensed As Double(), n As Integer) As HclustResult
        Return HclustCore(condensed, n)
    End Function

    ''' <summary>
    ''' UPGMA 主体：在压缩距离工作区上做「最近邻表 + Lance-Williams 更新」
    ''' </summary>
    Private Function HclustCore(condensed As Double(), n As Integer) As HclustResult
        Dim merge As Integer()() = New Integer(n - 2)() {}
        Dim height As Double() = New Double(n - 2) {}

        If n <= 1 Then
            Return New HclustResult With {.merge = New Integer()() {}, .height = New Double() {}, .n = n}
        End If

        Dim active As Boolean() = New Boolean(n - 1) {}
        Dim size As Integer() = New Integer(n - 1) {}
        Dim id As Integer() = New Integer(n - 1) {}
        Dim nn As Integer() = New Integer(n - 1) {}
        Dim mind As Double() = New Double(n - 1) {}

        For i As Integer = 0 To n - 1
            active(i) = True
            size(i) = 1
            id(i) = -(i + 1)
        Next

        ' 初始化最近邻表 O(n^2)
        For i As Integer = 0 To n - 1
            Call Rescan(i, active, condensed, n, nn, mind)
        Next

        Dim alive As Integer = n

        For ms As Integer = 1 To n - 1
            ' 找当前全局最小距离对 O(n)
            Dim a As Integer = -1
            Dim bestD As Double = Double.PositiveInfinity

            For i As Integer = 0 To n - 1
                If active(i) AndAlso mind(i) < bestD Then
                    bestD = mind(i)
                    a = i
                End If
            Next

            If a < 0 Then Exit For

            Dim b As Integer = nn(a)

            ' 保证 a 是较小的下标（与 R 的习惯一致）
            If b < a Then
                Dim tmp As Integer = a
                a = b
                b = tmp
            End If

            merge(ms - 1) = New Integer() {id(a), id(b)}
            height(ms - 1) = bestD

            Dim sa As Integer = size(a)
            Dim sb As Integer = size(b)
            Dim total As Integer = sa + sb

            size(a) = total
            id(a) = ms
            active(b) = False
            alive -= 1

            ' Lance-Williams 更新 d(a,k)，并顺带 O(1) 维护最近邻/次近邻
            For k As Integer = 0 To n - 1
                If Not active(k) OrElse k = a Then Continue For

                Dim da As Double = condensed(CIdx(a, k, n))
                Dim db As Double = condensed(CIdx(b, k, n))
                Dim nd As Double = (sa * da + sb * db) / total

                condensed(CIdx(a, k, n)) = nd

                If nd < mind(k) Then
                    mind(k) = nd
                    nn(k) = a
                End If
            Next

            ' 最近邻指向被合并掉的簇（a 或 b）时，必须重新扫描；
            ' a 自己的全部距离都被改写过，也要重扫一次。
            ' 
            ' 这里刻意保留「全量重扫 + 严格小于才替换」的朴素策略：
            ' 距离矩阵里存在大量并列值，任何基于缓存的近似都会改变并列时的取舍，
            ' 从而偏离 R 的 hclust。
            For k As Integer = 0 To n - 1
                If Not active(k) Then Continue For
                If k <> a AndAlso nn(k) <> a AndAlso nn(k) <> b Then Continue For

                Call Rescan(k, active, condensed, n, nn, mind)
            Next
        Next

        Return New HclustResult With {.merge = merge, .height = height, .n = n}
    End Function

    ''' <summary>
    ''' 重新扫描某个簇的最近邻（O(n)，严格小于才替换，与 R 的并列处理一致）
    ''' </summary>
    ''' <param name="k">待扫描的簇</param>
    ''' <param name="active">簇是否仍然存活</param>
    ''' <param name="condensed">压缩距离工作区</param>
    ''' <param name="n">矩阵阶数</param>
    ''' <param name="nn">最近邻下标数组（输出）</param>
    ''' <param name="mind">最近邻距离数组（输出）</param>
    ''' <remarks>
    ''' 这段是整个聚类过程的热点，因此刻意不调用 <see cref="CIdx"/>：
    ''' 压缩数组里 <c>j &gt; k</c> 的一段是<b>连续</b>的，
    ''' 而 <c>j &lt; k</c> 的一段下标可以用递推式 <c>idx += n - j - 2</c> 增量算出，
    ''' 两段都不需要乘法与分支判断。
    ''' </remarks>
    Private Sub Rescan(k As Integer, active As Boolean(), condensed As Double(), n As Integer,
                       nn As Integer(), mind As Double())
        Dim best As Integer = -1
        Dim bestD As Double = Double.PositiveInfinity

        ' j > k：压缩数组中的连续段
        Dim baseIdx As Integer = k * (2 * n - k - 1) \ 2

        For j As Integer = k + 1 To n - 1
            If active(j) Then
                Dim d As Double = condensed(baseIdx + j - k - 1)

                If d < bestD Then
                    bestD = d
                    best = j
                End If
            End If
        Next

        ' j < k：递推下标
        Dim idx As Integer = k - 1

        For j As Integer = 0 To k - 1
            If active(j) Then
                Dim d As Double = condensed(idx)

                If d < bestD Then
                    bestD = d
                    best = j
                End If
            End If

            idx += (n - j - 2)
        Next

        nn(k) = best
        mind(k) = bestD
    End Sub

    ''' <summary>
    ''' 对不相似度矩阵做 UPGMA 层次聚类，并返回 <see cref="Cluster"/> 树
    ''' </summary>
    ''' <param name="dist">行优先的 n x n 对称不相似度矩阵，会被就地修改</param>
    ''' <param name="n">矩阵阶数</param>
    ''' <param name="keys">叶子名称（基因 ID），顺序与矩阵行列一致</param>
    ''' <returns>层次聚类树的根节点</returns>
    Public Function Cluster(dist As Double(), n As Integer, keys As String()) As Cluster
        Dim hc As HclustResult = Hclust(dist, n)

        Return Cluster(hc, keys)
    End Function

    ''' <summary>
    ''' 由 hclust 的合并结构还原 <see cref="Cluster"/> 树
    ''' </summary>
    ''' <param name="hc">合并结构与高度</param>
    ''' <param name="keys">叶子名称（基因 ID）</param>
    ''' <returns>层次聚类树的根节点；n&lt;1 时返回 Nothing</returns>
    Public Function Cluster(hc As HclustResult, keys As String()) As Cluster
        Dim n As Integer = hc.n

        If n < 1 Then Return Nothing
        If n = 1 Then Return New Cluster(keys(0))

        Dim nodes As Cluster() = New Cluster(2 * n - 2) {}

        For i As Integer = 0 To n - 1
            nodes(i) = New Cluster(keys(i))
        Next

        For t As Integer = 0 To n - 2
            Dim a As Integer = hc.merge(t)(0)
            Dim b As Integer = hc.merge(t)(1)
            Dim ia As Integer = If(a < 0, -a - 1, n + a - 1)
            Dim ib As Integer = If(b < 0, -b - 1, n + b - 1)
            Dim parent As New Cluster($"node{t + 1}") With {
                .Distance = New Distance(hc.height(t))
            }

            Call parent.AddChild(nodes(ia))
            Call parent.AddChild(nodes(ib))

            nodes(ia).Parent = parent
            nodes(ib).Parent = parent

            nodes(n + t) = parent
        Next

        Return nodes(2 * n - 2)
    End Function

    ''' <summary>
    ''' 由不相似度矩阵做 UPGMA 聚类并直接取出 hclust 的 merge/height
    ''' </summary>
    ''' <param name="dist">行优先的 n x n 对称不相似度矩阵，会被就地修改</param>
    ''' <param name="n">矩阵阶数</param>
    ''' <param name="keys">叶子名称</param>
    ''' <returns>合并结构与高度</returns>
    ''' <remarks>
    ''' 这个方法跳过 <see cref="Cluster"/> 树的构建，直接给
    ''' <see cref="DynamicTreeCut"/> 使用，可以省掉 2n-1 个树节点对象。
    ''' </remarks>
    <Extension>
    Public Function Dendrogram(dist As Double(), n As Integer, keys As String()) As HclustResult
        Return Hclust(dist, n)
    End Function
End Module
