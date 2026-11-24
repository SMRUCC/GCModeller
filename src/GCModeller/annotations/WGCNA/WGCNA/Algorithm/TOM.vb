#Region "Microsoft.VisualBasic::199c75ad642c62c366ee05e5bac9f198, annotations\WGCNA\WGCNA\Algorithm\TOM.vb"

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

    '   Total Lines: 130
    '    Code Lines: 79 (60.77%)
    ' Comment Lines: 39 (30.00%)
    '    - Xml Docs: 69.23%
    ' 
    '   Blank Lines: 12 (9.23%)
    '     File Size: 5.08 KB


    ' Module TOM
    ' 
    '     Function: CreateModules, CreateModulesInternal, Intermediate, Matrix
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.DataMining.HierarchicalClustering
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports std = System.Math

''' <summary>
''' Category 2: Functions for module detection.
''' 
''' Modules are defined as clusters Of densely interconnected genes
'''
''' (TOM矩阵)
''' </summary>
''' <remarks>
''' TOM 是全流水线中计算量最大的一步：中间矩阵 <c>S = A·A</c> 是 O(n^3) 的。
''' 原实现用三重循环 + 交错数组的间接寻址串行求值，在 n=5000 时就已经是小时级。
''' 
''' <para>
''' 这里改为一次 GEMM（<see cref="Tensor.MatMul"/>），由 SIMD / CUDA 后端承担
''' 向量化与并行。这也是切换到 CUDA 后端时收益最大的环节。
''' </para>
''' </remarks>
Public Module TOM

    ''' <summary>
    ''' 计算中间矩阵I (Intermediate Matrix)
    ''' 
    ''' TOM公式中的中间项: w_ij = sum_u(a_iu * a_ju)
    ''' 表示节点i和j的共同邻居的连接强度之和
    ''' </summary>
    ''' <param name="A">邻接矩阵</param>
    ''' <returns>中间矩阵</returns>
    Public Function Intermediate(A As NumericMatrix) As GeneralMatrix
        Dim m As Integer = A.RowDimension
        Dim alpha As Double()() = A.Array
        Dim flat As Double() = TensorOps.Flatten(alpha, m, A.ColumnDimension)
        Dim prod As Double() = Intermediate(flat, m)

        Return New NumericMatrix(TensorOps.ToJagged(prod, m, m))
    End Function

    ''' <summary>
    ''' 计算TOM矩阵 (Topological Overlap Matrix)
    ''' 
    ''' TOM值衡量两个节点在网络拓扑结构上的相似性
    ''' 公式: w_ij = (I(i,j) + A(i,j)) / (min(k_i, k_j) + 1 - A(i,j))
    ''' 其中 k_i 是节点i的连接度
    ''' </summary>
    ''' <param name="A">邻接矩阵</param>
    ''' <param name="K">连接度向量</param>
    ''' <returns>TOM矩阵</returns>
    Public Function Matrix(A As NumericMatrix, K As Vector) As GeneralMatrix
        Dim m As Integer = A.RowDimension
        Dim alpha As Double()() = A.Array
        Dim flat As Double() = TensorOps.Flatten(alpha, m, A.ColumnDimension)
        Dim tom As Double() = Matrix(flat, K.Array, m)

        Return New NumericMatrix(TensorOps.ToJagged(tom, m, m))
    End Function

    ''' <summary>
    ''' 以 GEMM 计算中间矩阵 S = A·A（面向 Tensor 的高性能内核）
    ''' </summary>
    ''' <param name="adj">行优先 n x n 邻接矩阵（对称），只读</param>
    ''' <param name="n">矩阵阶数</param>
    ''' <returns>行优先的 n x n 中间矩阵</returns>
    ''' <remarks>
    ''' 中间项 <c>sum_u a(i,u)*a(u,j)</c> 就是矩阵乘积 <c>(A·A)(i,j)</c>；
    ''' 邻接矩阵对称时 A·Aᵀ = A·A，因此一次 <see cref="Tensor.MatMul"/> 即可。
    ''' </remarks>
    Public Function Intermediate(adj As Double(), n As Integer) As Double()
        Dim a As Tensor = TensorOps.Wrap(adj, n, n)
        Dim prod As Tensor = a.MatMul(a)

        Return prod.Data
    End Function

    ''' <summary>
    ''' 计算 TOM 矩阵（面向 Tensor 的高性能内核）
    ''' </summary>
    ''' <param name="adj">行优先 n x n 邻接矩阵，只读</param>
    ''' <param name="k">长度为 n 的连通度向量</param>
    ''' <param name="n">矩阵阶数</param>
    ''' <returns>行优先的 n x n TOM 矩阵，对角线恒为 1</returns>
    ''' <remarks>
    ''' 组合步骤融合成单次 O(n^2) 遍历并就地写回 GEMM 的结果缓冲区，
    ''' 因此整条 TOM 流水线的额外内存占用为 0（只有 GEMM 结果这一份 n^2）。
    ''' </remarks>
    Public Function Matrix(adj As Double(), k As Double(), n As Integer) As Double()
        Dim prod As Double() = Intermediate(adj, n)

        Return TensorOps.TomCombine(prod, adj, k, n)
    End Function

End Module
