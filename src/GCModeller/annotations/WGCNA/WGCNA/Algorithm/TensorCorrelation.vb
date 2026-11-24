#Region "Microsoft.VisualBasic::TensorCorrelation, annotations\WGCNA\WGCNA\Algorithm\TensorCorrelation.vb"

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
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports Microsoft.VisualBasic.Math.Matrix
Imports Microsoft.VisualBasic.Math.Statistics
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports std = System.Math

''' <summary>
''' 基于 GEMM 的 Pearson 相关矩阵
''' </summary>
''' <remarks>
''' 传统实现是「对每一对基因调用一次 Pearson」，即 O(n^2) 次向量运算，
''' 每次都带一次函数调用与临时列表分配，常数开销极大。
''' 
''' <para>
''' 这里利用恒等式：把每个基因的表达谱<b>按行去均值并做 L2 归一化</b>得到 Z 之后，
''' 两行之间的内积 (Z·Zᵀ)(i,j) 恰好等于它们的 Pearson 相关系数。
''' 于是整个相关矩阵可以由一次 GEMM 得到，交给 SIMD / CUDA 后端做向量化与并行，
''' 常数因子可以降低一到两个数量级。
''' </para>
''' 
''' <para>
''' 矩阵以行优先一维数组存放，与 <see cref="Tensor"/> 的内部布局一致，
''' 因此 <see cref="ToTensor"/> 是零拷贝的包装。
''' </para>
''' </remarks>
Public Class TensorCorrelation

    ''' <summary>相关矩阵本体（行优先 n x n）</summary>
    ReadOnly cor As Double()
    ''' <summary>基因（特征）数量，即矩阵阶数</summary>
    ReadOnly n As Integer
    ''' <summary>样本数量</summary>
    ReadOnly m As Integer

    ''' <summary>
    ''' 基因数量（矩阵阶数）
    ''' </summary>
    ''' <returns>基因数量</returns>
    Public ReadOnly Property Size As Integer
        Get
            Return n
        End Get
    End Property

    ''' <summary>
    ''' 样本数量（用于换算相关系数的 p 值）
    ''' </summary>
    ''' <returns>样本数量</returns>
    Public ReadOnly Property SampleSize As Integer
        Get
            Return m
        End Get
    End Property

    ''' <summary>
    ''' 相关矩阵的底层行优先缓冲区（只读）
    ''' </summary>
    ''' <returns>长度为 n*n 的一维数组</returns>
    Public ReadOnly Property Buffer As Double()
        Get
            Return cor
        End Get
    End Property

    ''' <summary>
    ''' 取相关矩阵元素 cor(i, j)
    ''' </summary>
    ''' <param name="i">行索引（基因 i）</param>
    ''' <param name="j">列索引（基因 j）</param>
    ''' <returns>Pearson 相关系数，范围 [-1, 1]</returns>
    Default Public ReadOnly Property Item(i As Integer, j As Integer) As Double
        Get
            Return cor(i * n + j)
        End Get
    End Property

    ''' <summary>
    ''' 由基因表达谱矩阵（基因 x 样本）创建相关矩阵
    ''' </summary>
    ''' <param name="expr">基因 x 样本的交错数组</param>
    ''' <returns>相关矩阵对象</returns>
    Public Shared Function Create(expr As Double()()) As TensorCorrelation
        Dim rows As Integer = expr.Length
        Dim cols As Integer = If(rows = 0, 0, expr(Scan0).Length)
        Dim flat As Double() = TensorOps.Flatten(expr, rows, cols)
        Dim z As Double() = TensorOps.RowStandardize(flat, rows, cols)
        Dim zt As Tensor = TensorOps.Wrap(z, rows, cols)
        Dim prod As Tensor = zt.MatMul(zt.Transpose())
        Dim c As Double() = prod.Data

        Call TensorOps.ClampUnitInPlace(c)

        Return New TensorCorrelation(c, rows, cols)
    End Function

    ''' <summary>
    ''' 由 HTS 表达矩阵对象创建相关矩阵
    ''' </summary>
    ''' <param name="samples">基因 x 样本的表达式矩阵</param>
    ''' <returns>相关矩阵对象</returns>
    Public Shared Function Create(samples As Matrix) As TensorCorrelation
        Return Create(samples.ArrayPack)
    End Function

    ''' <summary>
    ''' 由已经算好的行优先相关矩阵直接构造（用于分块内部的子矩阵装配）
    ''' </summary>
    ''' <param name="buffer">行优先 n x n 相关矩阵</param>
    ''' <param name="size">矩阵阶数</param>
    ''' <param name="sampleSize">样本数量</param>
    ''' <returns>相关矩阵对象</returns>
    Public Shared Function FromBuffer(buffer As Double(), size As Integer, sampleSize As Integer) As TensorCorrelation
        Return New TensorCorrelation(buffer, size, sampleSize)
    End Function

    Private Sub New(cor As Double(), n As Integer, m As Integer)
        Me.cor = cor
        Me.n = n
        Me.m = m
    End Sub

    ''' <summary>
    ''' 把相关矩阵包装成二维 <see cref="Tensor"/>（零拷贝，共享底层缓冲区）
    ''' </summary>
    ''' <returns>形状为 (n, n) 的张量</returns>
    ''' <remarks>
    ''' 由于 <see cref="Tensor.Wrap"/> 不拷贝数据，调用方<b>不要</b>通过该张量
    ''' 就地修改矩阵内容；需要可写副本请显式 <c>Clone</c>。
    ''' </remarks>
    Public Function ToTensor() As Tensor
        Return TensorOps.Wrap(cor, n, n)
    End Function

    ''' <summary>
    ''' 导出为交错数组形式的相关矩阵
    ''' </summary>
    ''' <returns>n x n 的交错数组（深拷贝）</returns>
    Public Function ToJagged() As Double()()
        Return TensorOps.ToJagged(cor, n, n)
    End Function

    ''' <summary>
    ''' 导出为 <see cref="CorrelationMatrix"/>（兼容旧 API）
    ''' </summary>
    ''' <param name="keys">基因 ID 列表，顺序必须与矩阵行列一致</param>
    ''' <param name="computePvalue">
    ''' 是否同时物化 p 值矩阵。p 值矩阵的内存占用与相关矩阵相当（n^2），
    ''' 在大规模数据下建议保持 False，改为通过 <see cref="Pvalue"/> 按需计算。
    ''' </param>
    ''' <returns>带 p 值通道的相关矩阵对象</returns>
    Public Function ToCorrelationMatrix(keys As String(), Optional computePvalue As Boolean = False) As CorrelationMatrix
        Dim names As Index(Of String) = keys.Indexing
        Dim mat As Double()() = ToJagged()
        Dim pval As Double()() = Nothing

        If computePvalue Then
            pval = New Double(n - 1)() {}

            For i As Integer = 0 To n - 1
                Dim row(n - 1) As Double

                For j As Integer = 0 To n - 1
                    row(j) = Pvalue(i, j)
                Next

                pval(i) = row
            Next
        Else
            pval = New Double(n - 1)() {}

            For i As Integer = 0 To n - 1
                pval(i) = New Double(n - 1) {}
            Next
        End If

        Return New CorrelationMatrix(names, mat, pval)
    End Function

    ''' <summary>
    ''' 计算相关系数 r 的双侧检验 p 值
    ''' </summary>
    ''' <param name="i">行索引（基因 i）</param>
    ''' <param name="j">列索引（基因 j）</param>
    ''' <returns>双侧 p 值</returns>
    ''' <remarks>
    ''' 用 t 统计量 <c>t = r*sqrt(df/(1-r^2))</c>（df = 样本数-2）配合正则化不完全 Beta 函数
    ''' 直接求解析解：<c>p = I_{df/(df+t^2)}(df/2, 1/2)</c>。
    ''' 相比数值积分求 t 分布 CDF，这里是 O(1) 的闭式计算，适合逐边按需调用。
    ''' </remarks>
    Public Function Pvalue(i As Integer, j As Integer) As Double
        Dim r As Double = cor(i * n + j)
        Dim df As Double = m - 2

        If df <= 0 Then
            Return 1
        End If

        Dim absR As Double = std.Abs(r)

        If absR >= 1 Then
            Return 0
        End If

        Dim t As Double = r * std.Sqrt(df / (1 - r * r))
        Dim x As Double = df / (df + t * t)

        If x <= 0 Then
            Return 0
        End If
        If x >= 1 Then
            Return 1
        End If

        Return SpecialFunctions.RegularizedIncompleteBetaFunction(df / 2, 0.5, x)
    End Function
End Class
