#Region "Microsoft.VisualBasic::GeneFilter, annotations\WGCNA\WGCNA\Algorithm\GeneFilter.vb"

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
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports std = System.Math

''' <summary>
''' 基因预过滤
''' </summary>
''' <remarks>
''' 大型表达矩阵里通常有相当比例的基因是「常数行」或极低方差行
''' （例如全零、检出率过低），它们对共表达网络没有任何贡献，
''' 却会成倍放大 O(n^2) 的相关矩阵与 O(n·B^2) 的 TOM 开销。
''' 
''' <para>
''' 过滤是可选的（默认关闭），但 R WGCNA 的标准流程也建议在
''' <c>blockwiseModules</c> 之前先做一遍质量过滤。
''' </para>
''' </remarks>
Public Module GeneFilter

    ''' <summary>
    ''' 计算每个基因的表达方差（样本方差，分母 n-1）
    ''' </summary>
    ''' <param name="samples">基因 x 样本表达矩阵</param>
    ''' <returns>长度与基因数相同的方差向量</returns>
    Public Function Variances(samples As Matrix) As Double()
        Dim rows As DataFrameRow() = samples.expression
        Dim vars(rows.Length - 1) As Double

        Call Parallel.For(0, rows.Length,
            Sub(i)
                Dim x As Double() = rows(i).experiments
                Dim n As Integer = x.Length
                Dim mean As Double = 0
                Dim count As Integer = 0

                For j As Integer = 0 To n - 1
                    If Not Double.IsNaN(x(j)) Then
                        mean += x(j)
                        count += 1
                    End If
                Next

                If count < 2 Then
                    vars(i) = 0
                    Return
                End If

                mean /= count

                Dim ss As Double = 0

                For j As Integer = 0 To n - 1
                    If Not Double.IsNaN(x(j)) Then
                        Dim d As Double = x(j) - mean
                        ss += d * d
                    End If
                Next

                vars(i) = ss / (count - 1)
            End Sub)

        Return vars
    End Function

    ''' <summary>
    ''' 按方差过滤：剔除低方差基因，并可只保留方差最大的前 N 个基因
    ''' </summary>
    ''' <param name="samples">基因 x 样本表达矩阵</param>
    ''' <param name="topN">只保留方差最大的前 N 个基因（0 表示不限）</param>
    ''' <param name="minVariance">方差下限（0 表示不限）</param>
    ''' <returns>过滤后的新矩阵</returns>
    Public Function ByVariance(samples As Matrix, Optional topN As Integer = 0, Optional minVariance As Double = 0) As Matrix
        Dim vars As Double() = Variances(samples)
        Dim order As Integer() = Enumerable.Range(0, vars.Length).ToArray()

        If topN > 0 AndAlso topN < order.Length Then
            order = order _
                .OrderByDescending(Function(i) vars(i)) _
                .Take(topN) _
                .OrderBy(Function(i) i) _
                .ToArray()
        Else
            order = order _
                .Where(Function(i) vars(i) >= minVariance AndAlso Not Double.IsNaN(vars(i))) _
                .ToArray()
        End If

        If minVariance > 0 Then
            order = order.Where(Function(i) vars(i) >= minVariance).ToArray()
        End If

        Return Subset(samples, order)
    End Function

    ''' <summary>
    ''' 按配置执行基因预过滤
    ''' </summary>
    ''' <param name="samples">基因 x 样本表达矩阵</param>
    ''' <param name="config">分析配置</param>
    ''' <returns>过滤后的矩阵；未启用任何过滤时返回原对象</returns>
    Public Function Filter(samples As Matrix, config As WGCNAConfig) As Matrix
        Dim needFilter As Boolean = config.filterTopN > 0 OrElse
                                    config.minVariance > 0 OrElse
                                    config.maxMissingRate > 0

        If Not needFilter Then
            Return samples
        End If

        Dim result As Matrix = samples

        If config.maxMissingRate > 0 Then
            result = DropMissing(result, config.maxMissingRate)
        End If

        Return ByVariance(result, config.filterTopN, config.minVariance)
    End Function

    ''' <summary>
    ''' 剔除缺失率过高的基因
    ''' </summary>
    ''' <param name="samples">基因 x 样本表达矩阵</param>
    ''' <param name="maxMissingRate">允许的最大缺失（NaN）比例</param>
    ''' <returns>过滤后的新矩阵</returns>
    Public Function DropMissing(samples As Matrix, maxMissingRate As Double) As Matrix
        Dim rows As DataFrameRow() = samples.expression
        Dim keep As New List(Of Integer)

        For i As Integer = 0 To rows.Length - 1
            Dim x As Double() = rows(i).experiments
            Dim missing As Integer = 0

            For j As Integer = 0 To x.Length - 1
                If Double.IsNaN(x(j)) Then missing += 1
            Next

            If missing <= x.Length * maxMissingRate Then
                keep.Add(i)
            End If
        Next

        Return Subset(samples, keep.ToArray())
    End Function

    ''' <summary>
    ''' 按行下标取子矩阵（保持原有行顺序）
    ''' </summary>
    ''' <param name="samples">原始矩阵</param>
    ''' <param name="rows">要保留的行下标（升序）</param>
    ''' <returns>子矩阵</returns>
    Public Function Subset(samples As Matrix, rows As Integer()) As Matrix
        Dim expr(rows.Length - 1) As DataFrameRow

        For i As Integer = 0 To rows.Length - 1
            expr(i) = samples.expression(rows(i))
        Next

        Return New Matrix With {
            .sampleID = samples.sampleID,
            .tag = $"filtered({samples.tag})",
            .expression = expr
        }
    End Function
End Module
