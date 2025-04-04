﻿#Region "Microsoft.VisualBasic::6a1ca06efed070e840d7dcd8012a905a, annotations\WGCNA\WGCNA\BetaTest.vb"

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
Imports Microsoft.VisualBasic.Math.Matrix
Imports Microsoft.VisualBasic.Math.Statistics.Linq

''' <summary>
''' test for best beta power value
''' </summary>
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
        Return BetaTableParallel(cor, betaRange, adjacency).OrderBy(Function(p) p.Power)
    End Function

    Private Shared Function BetaTableParallel(cor As CorrelationMatrix, betaRange As IEnumerable(Of Double), adjacency As Double) As IEnumerable(Of BetaTest)
        Return betaRange _
            .AsParallel _
            .Select(Function(beta)
                        Dim K = WeightedNetwork.Connectivity(cor, beta, adjacency)
                        ' 基于无尺度分布的假设，我们认为p(ki)与ki呈负相关关系
                        Dim linear = SoftLinear.CreateLinear(K)

                        Return New BetaTest With {
                            .meanK = K.Average,
                            .maxK = K.Max,
                            .medianK = K.Median,
                            .Power = beta,
                            .sftRsq = If(linear.R_square.IsNaNImaginary, 0, linear.R_square),
                            .slope = If(linear.Slope.IsNaNImaginary, 0, linear.R_square),
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
