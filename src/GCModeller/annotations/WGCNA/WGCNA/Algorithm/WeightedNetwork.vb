#Region "Microsoft.VisualBasic::1c899cf15ce9779fa5385b61eed7a1de, annotations\WGCNA\WGCNA\Algorithm\WeightedNetwork.vb"

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

    '   Total Lines: 95
    '    Code Lines: 40 (42.11%)
    ' Comment Lines: 44 (46.32%)
    '    - Xml Docs: 93.18%
    ' 
    '   Blank Lines: 11 (11.58%)
    '     File Size: 3.46 KB


    ' Module WeightedNetwork
    ' 
    '     Function: Adjacency, Connectivity, sumK, WeightedCorrelation
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports Microsoft.VisualBasic.Math.Matrix
Imports std = System.Math

''' <summary>
''' Category 1: Functions for network construction
''' </summary>
''' <remarks>
''' 本模块的 n^2 级运算全部基于行优先一维 <see cref="Double"/> 缓冲区，
''' 软阈值幂运算与硬阈值化都是<b>就地/单次遍历</b>完成的，不再产生多份 n^2 临时矩阵。
''' </remarks>
Public Module WeightedNetwork

    ''' <summary>
    ''' set the element which value less than threshold to zero
    ''' </summary>
    ''' <param name="cor"></param>
    ''' <param name="threshold">edge cutoff value</param>
    ''' <returns></returns>
    <Extension>
    Friend Function Adjacency(cor As NumericMatrix, threshold As Double) As GeneralMatrix
        Dim adj As NumericMatrix = cor.Copy
        Dim X As Double()() = adj.Array
        Dim w As Integer = X(Scan0).Length
        Dim flat As Double() = TensorOps.Flatten(X, X.Length, w)

        Call TensorOps.ThresholdInPlace(flat, threshold)

        Dim out As Double()() = TensorOps.ToJagged(flat, X.Length, w)

        For i As Integer = 0 To X.Length - 1
            Call Array.Copy(out(i), 0, X(i), 0, w)
        Next

        Return adj
    End Function

    ''' <summary>
    ''' 得到权重关联网络A
    ''' </summary>
    ''' <param name="cor">
    ''' ``cor(gi, gj)``
    ''' </param>
    ''' <param name="betaPow">
    ''' 权重
    ''' </param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 1. a trait-based node significance measure can be defined as the absolute 
    '''    value of the correlation between the i-th node profile x i and the 
    '''    sample trait
    ''' 2. alternatively, a correlation test p-value or a regression-based p-value for 
    '''    assessing the statistical significance between x i and the sample trait T 
    '''    can be used to define a p-value based node significance measure
    ''' </remarks>
    <Extension>
    Public Function WeightedCorrelation(cor As CorrelationMatrix, betaPow As Double, Optional pvalue As Boolean = False) As NumericMatrix
        ' The default method defines the coexpression
        ' Similarity sij as the absolute value of the correlation
        ' coefficient between the profiles of nodes i And j
        Dim S As NumericMatrix = If(pvalue, -(DirectCast(cor.GetPvalueMatrix, NumericMatrix).Log(newBase:=10)), CType(cor, NumericMatrix)).Abs
        Dim A As GeneralMatrix = S ^ betaPow

        Return A
    End Function

    ''' <summary>
    ''' 连通度K
    ''' </summary>
    ''' <param name="cor">
    ''' A network is fully specified by its adjacency matrix aij, a
    ''' symmetric n × n matrix With entries In [0, 1] whose component
    ''' aij encodes the network connection strength
    ''' between nodes i And j.
    ''' </param>
    ''' <param name="betaPow"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 连接度ki表示第 i 个基因和其他基因的α值加和
    ''' </remarks>
    Public Function Connectivity(cor As CorrelationMatrix, betaPow As Double, adjacency As Double, Optional pvalue As Boolean = False) As Vector
        Dim A As NumericMatrix = cor.WeightedCorrelation(betaPow, pvalue).Adjacency(adjacency)
        Dim K As New Vector(A.RowApply(AddressOf sumK))

        Return K
    End Function

    Friend Function sumK(r As Double(), i As Integer) As Double
        Dim sum As Double = 0

        For j As Integer = 0 To r.Length - 1
            If i <> j Then
                sum += r(j)
            End If
        Next

        Return sum
    End Function

    ' ---------------------------------------------------------------------------------
    ' 以下为面向 Tensor 的高性能内核
    ' ---------------------------------------------------------------------------------

    ''' <summary>
    ''' 取相关矩阵的绝对值副本：S = |cor|
    ''' </summary>
    ''' <param name="cor">相关矩阵对象</param>
    ''' <returns>行优先的 n x n 相似度矩阵（新分配的缓冲区）</returns>
    Public Function AbsCorrelation(cor As TensorCorrelation) As Double()
        Dim src As Double() = cor.Buffer
        Dim s(src.Length - 1) As Double

        Call System.Threading.Tasks.Parallel.For(0, src.Length,
            Sub(i)
                s(i) = std.Abs(src(i))
            End Sub)

        Return s
    End Function

    ''' <summary>
    ''' 软阈值化：A = S^beta（就地）
    ''' </summary>
    ''' <param name="s">相似度矩阵 S = |cor|，就地修改为邻接矩阵</param>
    ''' <param name="beta">软阈值幂次</param>
    ''' <remarks>
    ''' 由于 S 的元素非负，这里等价于 <c>Math.pow(S, beta)</c>。
    ''' 直接遍历底层数组可以避免一次 n^2 的结果张量分配之外的额外拷贝。
    ''' </remarks>
    Public Sub SoftThresholdInPlace(s As Double(), beta As Double)
        Call TensorOps.AbsPowInPlace(s, beta)
    End Sub

    ''' <summary>
    ''' 硬阈值化：小于 <paramref name="threshold"/> 的连接强度一律置零（就地）
    ''' </summary>
    ''' <param name="adj">邻接矩阵，就地修改</param>
    ''' <param name="threshold">边截断阈值</param>
    Public Sub CutInPlace(adj As Double(), threshold As Double)
        Call TensorOps.ThresholdInPlace(adj, threshold)
    End Sub

    ''' <summary>
    ''' 计算给定软阈值下的邻接矩阵：A = threshold(|cor|^beta)，并把对角线置零
    ''' </summary>
    ''' <param name="absCor">相似度矩阵 |cor|（行优先 n x n），不会被修改</param>
    ''' <param name="beta">软阈值幂次</param>
    ''' <param name="adjacency">边截断阈值；设为 0 可以关闭硬阈值，得到纯软阈值邻接矩阵</param>
    ''' <returns>行优先的 n x n 邻接矩阵（对角线恒为 0）</returns>
    ''' <remarks>
    ''' 先做一次 n^2 的幂运算再就地阈值化，全程只有一份 n^2 缓冲区。
    ''' 
    ''' <para>
    ''' 对角线置零是 GNU R WGCNA <c>adjacency()</c> 的约定（基因不与自身相连）。
    ''' 这一步很重要：若保留对角线（值为 1），TOM 的中间矩阵
    ''' <c>S = A·A</c> 会额外多出 <c>2·A(i,j)</c>，导致 TOM 与 R 系统性偏离。
    ''' </para>
    ''' </remarks>
    Public Function BuildAdjacency(absCor As Double(), beta As Double, adjacency As Double) As Double()
        Dim a(absCor.Length - 1) As Double

        Call Array.Copy(absCor, a, a.Length)
        Call SoftThresholdInPlace(a, beta)
        Call CutInPlace(a, adjacency)

        ' 去掉自连接（与 R 的 adjacency() 一致）
        Dim n As Integer = CInt(std.Sqrt(a.Length))

        For i As Integer = 0 To n - 1
            a(i * n + i) = 0
        Next

        Return a
    End Function

    ''' <summary>
    ''' 流式连通度：直接扫描 |cor| 求每个基因的连接度，不分配 n^2 邻接矩阵
    ''' </summary>
    ''' <param name="absCor">相似度矩阵 |cor|（行优先 n x n）</param>
    ''' <param name="n">矩阵阶数（基因数）</param>
    ''' <param name="beta">软阈值幂次</param>
    ''' <param name="adjacency">边截断阈值</param>
    ''' <returns>长度为 n 的连通度向量</returns>
    ''' <remarks>
    ''' <para>
    ''' 关键优化：邻接矩阵的判定条件是 <c>|cor|^beta &gt;= τ</c>，
    ''' 由于 |cor| 非负且 beta &gt; 0，这等价于 <c>|cor| &gt;= τ^(1/beta)</c>。
    ''' 于是可以先算一次 <c>cut = τ^(1/beta)</c>，把所有注定被截断的元素直接跳过，
    ''' 免去整份矩阵的 <c>Pow</c> 调用。
    ''' </para>
    ''' <para>
    ''' 在 beta 较大（例如 12）时 cut 接近 1，剪枝率极高，
    ''' beta 扫描（约 20 个候选值）的总体开销可以下降一个数量级。
    ''' </para>
    ''' </remarks>
    Public Function Connectivity(absCor As Double(), n As Integer, beta As Double, adjacency As Double) As Double()
        Dim cut As Double = std.Pow(adjacency, 1 / beta)
        Dim k(n - 1) As Double

        Call System.Threading.Tasks.Parallel.For(0, n,
            Sub(i)
                Dim offset As Integer = i * n
                Dim s As Double = 0

                For j As Integer = 0 To n - 1
                    Dim c As Double = absCor(offset + j)

                    If c >= cut Then
                        s += std.Pow(c, beta)
                    End If
                Next

                ' 排除自连接
                If absCor(offset + i) >= cut Then
                    s -= std.Pow(absCor(offset + i), beta)
                End If

                k(i) = s
            End Sub)

        Return k
    End Function

    ''' <summary>
    ''' 由邻接矩阵直接求连通度（排除对角线）
    ''' </summary>
    ''' <param name="adj">行优先 n x n 邻接矩阵</param>
    ''' <param name="n">矩阵阶数</param>
    ''' <returns>长度为 n 的连通度向量</returns>
    Public Function ConnectivityOf(adj As Double(), n As Integer) As Double()
        Return TensorOps.RowSumsExclDiag(adj, n)
    End Function

    ''' <summary>
    ''' 把邻接矩阵包装成二维 <see cref="Tensor"/>（零拷贝），供 GEMM 使用
    ''' </summary>
    ''' <param name="adj">行优先 n x n 邻接矩阵</param>
    ''' <param name="n">矩阵阶数</param>
    ''' <returns>形状为 (n, n) 的张量视图</returns>
    Public Function AsTensor(adj As Double(), n As Integer) As Tensor
        Return TensorOps.Wrap(adj, n, n)
    End Function
End Module
