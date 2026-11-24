#Region "Microsoft.VisualBasic::TensorOps, annotations\WGCNA\WGCNA\Algorithm\TensorOps.vb"

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
Imports System.Threading.Tasks
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

''' <summary>
''' Tensor 互操作辅助与融合算子
''' </summary>
''' <remarks>
''' 本模块是 WGCNA 中所有 n^2 级稠密矩阵运算的统一入口。
''' 
''' 设计约定：
''' <list type="bullet">
''' <item><description>
''' 矩阵一律以「行优先(row-major) 一维 <see cref="Double"/> 数组」的形式存放，
''' 与 <see cref="Tensor"/> 的内部布局完全一致，从而可以在需要 GEMM 时零拷贝地
''' 包成 <see cref="Tensor"/>，在需要融合遍历时又直接操作底层数组。
''' </description></item>
''' <item><description>
''' GEMM（相关矩阵、TOM 中间矩阵）走 <see cref="Tensor.MatMul"/>，这样切换到 CUDA 后端
''' (<c>ILCuda.GPUTensor.CudaTensor.Register</c>) 时无需改动任何业务代码。
''' </description></item>
''' <item><description>
''' 逐元素/归约类运算走 <see cref="Parallel"/> 融合内核：直接读写底层一维数组，
''' 避免为每一步都分配一份 n^2 的临时 <see cref="Tensor"/>。
''' </description></item>
''' </list>
''' </remarks>
Public Module TensorOps

    ''' <summary>
    ''' 把交错数组按行优先展平成一维数组
    ''' </summary>
    ''' <param name="mat">rows x cols 的交错数组</param>
    ''' <param name="rows">行数</param>
    ''' <param name="cols">列数</param>
    ''' <returns>长度为 rows*cols 的一维数组</returns>
    Public Function Flatten(mat As Double()(), rows As Integer, cols As Integer) As Double()
        Dim flat(rows * cols - 1) As Double

        For i As Integer = 0 To rows - 1
            Call Array.Copy(mat(i), 0, flat, i * cols, cols)
        Next

        Return flat
    End Function

    ''' <summary>
    ''' 由交错数组创建二维 <see cref="Tensor"/>
    ''' </summary>
    ''' <param name="mat">rows x cols 的交错数组</param>
    ''' <returns>形状为 (rows, cols) 的张量</returns>
    Public Function ToTensor(mat As Double()()) As Tensor
        Dim rows As Integer = mat.Length
        Dim cols As Integer = If(rows = 0, 0, mat(Scan0).Length)

        Return New Tensor(Flatten(mat, rows, cols), rows, cols)
    End Function

    ''' <summary>
    ''' 把一维行优先数组还原成交错数组
    ''' </summary>
    ''' <param name="flat">长度为 rows*cols 的一维数组</param>
    ''' <param name="rows">行数</param>
    ''' <param name="cols">列数</param>
    ''' <returns>rows x cols 的交错数组</returns>
    Public Function ToJagged(flat As Double(), rows As Integer, cols As Integer) As Double()()
        Dim mat(rows - 1)() As Double

        For i As Integer = 0 To rows - 1
            Dim row(cols - 1) As Double
            Call Array.Copy(flat, i * cols, row, 0, cols)
            mat(i) = row
        Next

        Return mat
    End Function

    ''' <summary>
    ''' 把二维张量还原成交错数组
    ''' </summary>
    ''' <param name="t">二维张量</param>
    ''' <returns>rows x cols 的交错数组</returns>
    Public Function ToJagged(t As Tensor) As Double()()
        Return ToJagged(t.Data, t.Shape(0), t.Shape(1))
    End Function

    ''' <summary>
    ''' 逐行求和（并行）
    ''' </summary>
    ''' <param name="x">行优先一维数组</param>
    ''' <param name="rows">行数</param>
    ''' <param name="cols">列数</param>
    ''' <returns>长度为 rows 的行和向量</returns>
    Public Function RowSums(x As Double(), rows As Integer, cols As Integer) As Double()
        Dim sums(rows - 1) As Double

        Call Parallel.For(0, rows,
            Sub(i)
                Dim offset As Integer = i * cols
                Dim s As Double = 0

                For j As Integer = 0 To cols - 1
                    s += x(offset + j)
                Next

                sums(i) = s
            End Sub)

        Return sums
    End Function

    ''' <summary>
    ''' 逐行求和并排除对角线元素（对称方阵的连通度）
    ''' </summary>
    ''' <param name="x">n x n 对称方阵（行优先）</param>
    ''' <param name="n">矩阵阶数</param>
    ''' <returns>长度为 n 的连通度向量，k(i) = sum_j!=i x(i,j)</returns>
    Public Function RowSumsExclDiag(x As Double(), n As Integer) As Double()
        Dim k(n - 1) As Double

        Call Parallel.For(0, n,
            Sub(i)
                Dim offset As Integer = i * n
                Dim s As Double = 0

                For j As Integer = 0 To n - 1
                    s += x(offset + j)
                Next

                k(i) = s - x(offset + i)
            End Sub)

        Return k
    End Function

    ''' <summary>
    ''' 行标准化：先按行去均值，再做 L2 归一化。
    ''' 这样两行的点积即等于它们的 Pearson 相关系数。
    ''' </summary>
    ''' <param name="x">rows x cols 的行优先输入（基因 x 样本）</param>
    ''' <param name="rows">行数（基因数）</param>
    ''' <param name="cols">列数（样本数）</param>
    ''' <returns>标准化后的行优先数组；零方差行全部置零</returns>
    ''' <remarks>
    ''' 零方差（常数表达谱）的行无法定义相关系数，这里一律置零，
    ''' 使得它与任何基因的相关系数都是 0，避免出现 NaN。
    ''' </remarks>
    Public Function RowStandardize(x As Double(), rows As Integer, cols As Integer) As Double()
        Dim z(rows * cols - 1) As Double

        Call Parallel.For(0, rows,
            Sub(i)
                Dim offset As Integer = i * cols
                Dim mean As Double = 0

                For j As Integer = 0 To cols - 1
                    mean += x(offset + j)
                Next

                mean /= cols

                Dim ss As Double = 0

                For j As Integer = 0 To cols - 1
                    Dim d As Double = x(offset + j) - mean

                    z(offset + j) = d
                    ss += d * d
                Next

                If ss > 0 Then
                    Dim inv As Double = 1 / std.Sqrt(ss)

                    For j As Integer = 0 To cols - 1
                        z(offset + j) *= inv
                    Next
                Else
                    For j As Integer = 0 To cols - 1
                        z(offset + j) = 0
                    Next
                End If
            End Sub)

        Return z
    End Function

    ''' <summary>
    ''' 就地取绝对值后做幂运算：x = |x|^p
    ''' </summary>
    ''' <param name="x">待处理的行优先数组（就地修改）</param>
    ''' <param name="p">幂指数（软阈值 beta）</param>
    Public Sub AbsPowInPlace(x As Double(), p As Double)
        Call Parallel.For(0, x.Length,
            Sub(i)
                x(i) = std.Pow(std.Abs(x(i)), p)
            End Sub)
    End Sub

    ''' <summary>
    ''' 就地阈值化：小于 <paramref name="threshold"/> 的元素一律置零（邻接矩阵硬阈值）
    ''' </summary>
    ''' <param name="x">待处理的行优先数组（就地修改）</param>
    ''' <param name="threshold">边截断阈值</param>
    Public Sub ThresholdInPlace(x As Double(), threshold As Double)
        Call Parallel.For(0, x.Length,
            Sub(i)
                If x(i) < threshold Then
                    x(i) = 0
                End If
            End Sub)
    End Sub

    ''' <summary>
    ''' 就地把方阵裁剪到 [-1, 1]（消除 GEMM 累积的浮点误差）
    ''' </summary>
    ''' <param name="x">待处理的行优先数组（就地修改）</param>
    Public Sub ClampUnitInPlace(x As Double())
        Call Parallel.For(0, x.Length,
            Sub(i)
                If x(i) > 1 Then
                    x(i) = 1
                ElseIf x(i) < -1 Then
                    x(i) = -1
                ElseIf Double.IsNaN(x(i)) Then
                    x(i) = 0
                End If
            End Sub)
    End Sub

    ''' <summary>
    ''' 融合计算 TOM 矩阵：w(i,j) = (S(i,j) + a(i,j)) / (min(k_i, k_j) + 1 - a(i,j))
    ''' </summary>
    ''' <param name="prod">中间矩阵 sum_u a(i,u)*a(u,j)（行优先 n x n），会被就地覆盖为 TOM</param>
    ''' <param name="adj">邻接矩阵（行优先 n x n），只读</param>
    ''' <param name="k">连通度向量</param>
    ''' <param name="n">矩阵阶数</param>
    ''' <returns>TOM 矩阵（行优先 n x n），对角线恒为 1</returns>
    ''' <remarks>
    ''' 这一步融合成单次 O(n^2) 遍历，避免「分子相加」与「除法」各自分配一份 n^2 中间矩阵。
    ''' 由于结果只依赖 <paramref name="prod"/> 与 <paramref name="adj"/> 的对应位置，
    ''' 可以安全地复用 <paramref name="prod"/> 的缓冲区就地写回。
    ''' </remarks>
    Public Function TomCombine(prod As Double(), adj As Double(), k As Double(), n As Integer) As Double()
        Call Parallel.For(0, n,
            Sub(r)
                Dim offset As Integer = r * n
                Dim kr As Double = k(r)

                For c As Integer = 0 To n - 1
                    Dim idx As Integer = offset + c

                    If r = c Then
                        prod(idx) = 1.0
                    Else
                        Dim a As Double = adj(idx)
                        Dim denominator As Double = std.Min(kr, k(c)) + 1 - a

                        If denominator > 0 Then
                            prod(idx) = (prod(idx) + a) / denominator
                        Else
                            prod(idx) = 0
                        End If
                    End If
                Next
            End Sub)

        Return prod
    End Function

    ''' <summary>
    ''' 就地求 TOM 不相似度：d = 1 - TOM
    ''' </summary>
    ''' <param name="tom">TOM 矩阵（就地修改）</param>
    Public Sub DissimilarityInPlace(tom As Double())
        Call Parallel.For(0, tom.Length,
            Sub(i)
                tom(i) = 1 - tom(i)
            End Sub)
    End Sub

    ''' <summary>
    ''' 把 n x n 对称方阵（行优先）转换成 pdist 压缩格式的下三角向量
    ''' </summary>
    ''' <param name="x">n x n 行优先对称矩阵</param>
    ''' <param name="n">矩阵阶数</param>
    ''' <returns>长度为 n(n-1)/2 的压缩向量，索引约定见 <c>accessFunction(i,j,n) = n*j - j*(j+1)\2 + i - 1 - j</c></returns>
    ''' <remarks>
    ''' <see cref="Microsoft.VisualBasic.DataMining.HierarchicalClustering.PDistClusteringAlgorithm"/>
    ''' 接受该格式作为输入，相比交错距离矩阵可以省掉 n 个数组对象的开销。
    ''' </remarks>
    Public Function ToPdist(x As Double(), n As Integer) As Double()
        Dim size As Long = CLng(n) * (n - 1) \ 2
        Dim condensed(CInt(size) - 1) As Double

        Call Parallel.For(0, n - 1,
            Sub(j)
                Dim offsetJ As Integer = j * n
                Dim baseIdx As Integer = n * j - j * (j + 1) \ 2 - 1 - j

                For i As Integer = j + 1 To n - 1
                    condensed(baseIdx + i) = x(offsetJ + i)
                Next
            End Sub)

        Return condensed
    End Function

    ''' <summary>
    ''' 把二维张量按行优先包成新的 <see cref="Tensor"/>（共享底层数组引用，不拷贝）
    ''' </summary>
    ''' <param name="flat">行优先一维数组</param>
    ''' <param name="rows">行数</param>
    ''' <param name="cols">列数</param>
    ''' <returns>形状为 (rows, cols) 的张量</returns>
    ''' <remarks>
    ''' <see cref="Tensor.Wrap"/> 不拷贝输入数组（构造函数会 Clone），因此可以零拷贝地
    ''' 把融合内核产出的缓冲区直接交给 GEMM 使用。
    ''' </remarks>
    Public Function Wrap(flat As Double(), rows As Integer, cols As Integer) As Tensor
        Return Tensor.Wrap(flat, rows, cols)
    End Function
End Module
