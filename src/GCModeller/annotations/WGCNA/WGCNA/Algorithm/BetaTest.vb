#Region "Microsoft.VisualBasic::0f5746867da6d2c9eec9c3cff00fb56f, annotations\WGCNA\WGCNA\Algorithm\BetaTest.vb"

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

    '   Total Lines: 90
    '    Code Lines: 60 (66.67%)
    ' Comment Lines: 20 (22.22%)
    '    - Xml Docs: 95.00%
    ' 
    '   Blank Lines: 10 (11.11%)
    '     File Size: 3.94 KB


    ' Class BetaTest
    ' 
    '     Properties: maxK, meanK, medianK, Power, score
    '                 sftRsq, slope, truncatedRsq
    ' 
    '     Function: Best, BetaTable, BetaTableParallel, getScores, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports Microsoft.VisualBasic.Math.Matrix
Imports Microsoft.VisualBasic.Math.Statistics.Linq
Imports std = System.Math

''' <summary>
''' test for best beta power value
''' </summary>
''' <remarks>
''' 原实现为每个候选 beta 都构造一份完整的 n^2 邻接矩阵（幂运算 + 拷贝 + 阈值化），
''' 20 个候选值意味着 40 余次 n^2 级别的分配与 GC 压力。
''' 
''' <para>
''' 现在改为复用同一份 <c>|cor|</c> 缓冲区，对每个 beta 只做一次<b>流式行和扫描</b>
''' （见 <see cref="WeightedNetwork.Connectivity"/>），全程不再分配任何 n^2 临时矩阵；
''' 候选 beta 之间再用 PLINQ 并行。
''' </para>
''' </remarks>
Public Class BetaTest

    Public Property Power As Double
    Public Property sftRsq As Double
    Public Property slope As Double
    Public Property truncatedRsq As Double
    Public Property meanK As Double
    Public Property medianK As Double
    Public Property maxK As Double

    Public ReadOnly Property score As Double
        Get
            Return (sftRsq - 0.8) - (slope - 1) + meanK
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"[{Power.ToString.PadEnd(2, " "c)}, score={score.ToString("F2")}] {getScores.JoinBy(", ")}"
    End Function

    Private Iterator Function getScores() As IEnumerable(Of String)
        Yield $"SFT.R.sq:{sftRsq.ToString("F3")}"
        Yield $"slope:{slope.ToString("F2")}"
        Yield $"truncated.R.sq:{truncatedRsq.ToString("F3")}"
        Yield $"mean.K:{meanK.ToString("F2")}"
        Yield $"median.K:{medianK.ToString("F2")}"
        Yield $"max.k:{maxK.ToString("F2")}"
    End Function

    ''' <summary>
    ''' 利用一元线性回归取匹配最佳β值，即用不同的β值去试验，寻找最佳的β值
    ''' 在线性回归中，我们要求 R^2 大于0.8，slope位于 -1 左右，而平均连接度要尽可能大
    ''' </summary>
    ''' <param name="cor"></param>
    ''' <param name="betaRange"></param>
    ''' <returns>
    ''' 函数返回得分最高的beta值
    ''' </returns>
    Public Shared Function BetaTable(cor As CorrelationMatrix, betaRange As IEnumerable(Of Double), adjacency As Double) As IEnumerable(Of BetaTest)
        Dim mat As Double()() = CType(cor, NumericMatrix).Array
        Dim n As Integer = mat.Length
        Dim flat As Double() = TensorOps.Flatten(mat, n, If(n = 0, 0, mat(Scan0).Length))

        For i As Integer = 0 To flat.Length - 1
            flat(i) = std.Abs(flat(i))
        Next

        Return BetaTable(flat, n, betaRange, adjacency)
    End Function

    ''' <summary>
    ''' 基于 Tensor 相关矩阵做 beta 扫描（推荐入口）
    ''' </summary>
    ''' <param name="cor">GEMM 得到的相关矩阵</param>
    ''' <param name="betaRange">候选软阈值幂次序列</param>
    ''' <param name="adjacency">边截断阈值</param>
    ''' <returns>按 power 升序排列的候选评估结果</returns>
    Public Shared Function BetaTable(cor As TensorCorrelation, betaRange As IEnumerable(Of Double), adjacency As Double) As IEnumerable(Of BetaTest)
        Return BetaTable(WeightedNetwork.AbsCorrelation(cor), cor.Size, betaRange, adjacency)
    End Function

    ''' <summary>
    ''' 基于相似度矩阵 |cor| 做流式 beta 扫描（零 n^2 临时分配）
    ''' </summary>
    ''' <param name="absCor">行优先 n x n 相似度矩阵 |cor|</param>
    ''' <param name="n">矩阵阶数</param>
    ''' <param name="betaRange">候选软阈值幂次序列</param>
    ''' <param name="adjacency">边截断阈值</param>
    ''' <returns>按 power 升序排列的候选评估结果</returns>
    Public Shared Function BetaTable(absCor As Double(), n As Integer, betaRange As IEnumerable(Of Double), adjacency As Double) As IEnumerable(Of BetaTest)
        Return BetaTableParallel(absCor, n, betaRange, adjacency).OrderBy(Function(p) p.Power)
    End Function

    Private Shared Function BetaTableParallel(absCor As Double(), n As Integer, betaRange As IEnumerable(Of Double), adjacency As Double) As IEnumerable(Of BetaTest)
        Return betaRange _
            .AsParallel _
            .Select(Function(beta)
                        Dim K As Double() = WeightedNetwork.Connectivity(absCor, n, beta, adjacency)
                        Dim Kv As New Vector(K)
                        ' 基于无尺度分布的假设，我们认为p(ki)与ki呈负相关关系
                        Dim linear = SoftLinear.CreateLinear(Kv)

                        Return New BetaTest With {
                            .meanK = Kv.Average,
                            .maxK = Kv.Max,
                            .medianK = Kv.Median,
                            .Power = beta,
                            .sftRsq = If(linear.R_square.IsNaNImaginary, 0, linear.R_square),
                            .slope = If(linear.Slope.IsNaNImaginary, 0, linear.Slope),
                            .truncatedRsq = If(linear.AdjustR_square.IsNaNImaginary, 0, linear.AdjustR_square)
                        }
                    End Function)
    End Function

    ''' <summary>
    ''' get the index of the max beta score from the candidates
    ''' </summary>
    ''' <param name="beta">
    ''' a set of the beta candidates on the correlation matrix
    ''' </param>
    ''' <returns></returns>
    Public Shared Function Best(beta As BetaTest()) As Integer
        Dim sftRsq As Vector = beta.Select(Function(b) If(b.sftRsq <= 0.8, 0, 1 - b.sftRsq)).AsVector
        Dim slope As Vector = (beta.Select(Function(b) b.slope).AsVector + 1).Abs
        Dim meanK As Vector = beta.Select(Function(b) b.meanK).AsVector
        Dim sftRsqMax = sftRsq.Max
        Dim slopeMax = slope.Max
        Dim meanKMax = meanK.Max
        Dim score As Vector = sftRsq / sftRsqMax + slope / slopeMax + meanK / meanKMax

        Return which.Max(score)
    End Function
End Class
