#Region "Microsoft.VisualBasic::de81980cdb204d1fb7ebfcaba6b0cafb, sub-system\BNLearn\ParameterLearning\ParameterLearning.vb"

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

'   Total Lines: 189
'    Code Lines: 125 (66.14%)
' Comment Lines: 36 (19.05%)
'    - Xml Docs: 25.00%
' 
'   Blank Lines: 28 (14.81%)
'     File Size: 7.11 KB


'     Module BnParameterLearner
' 
'         Function: Learn, OLS
' 
' 
' /********************************************************************************/

#End Region

' ============================================================
' ParameterLearning.vb - 参数学习
' ============================================================
' 在给定 DAG 结构下，估计每个节点的条件概率分布参数
' 
' 高斯贝叶斯网络（GBN）参数学习：
'   Xi | Pa(Xi) ~ N(β0 + Σ βj·Pa_j, σ²)
' 
' 使用 MLE（最大似然估计）：
'   β = (X'X)^(-1) X'y  （最小二乘法）
'   σ² = RSS / n
' ============================================================

Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Solvers

Namespace ParameterLearning

    ''' <summary>
    ''' 贝叶斯网络参数学习器
    ''' </summary>
    Public Module BnParameterLearner

        ''' <summary>
        ''' 从数据学习网络参数
        ''' </summary>
        Public Function Learn(network As Core.BayesianNetwork, data As Core.GeneExpressionData) As ParameterLearningResult
            Dim t0 As Date = Now
            Dim nS As Integer = data.NSample
            Dim totalLL As Double = 0
            Dim totalBIC As Double = 0
            Dim sumR2 As Double = 0

            Call "make network topological sort!".debug

            ' 按拓扑排序依次拟合每个节点
            Dim topoOrder As Integer() = network.TopologicalSort()

            Call "run bnlearn network parameter learning...".debug

            For Each nodeIdx As Integer In TqdmWrapper.Wrap(topoOrder)
                Dim node As Core.BnNode = network.Nodes(nodeIdx)
                Dim cpd As New Core.BnCPD()
                cpd.NodeIndex = nodeIdx
                cpd.NSamples = nS

                Dim y As Double() = data.GetGeneExpression(nodeIdx)

                If node.Parents.Count = 0 Then
                    ' 无父节点：边际分布 N(μ, σ²)
                    Dim mean As Double = 0
                    For j = 0 To nS - 1
                        mean += y(j)
                    Next
                    mean /= nS

                    Dim rss As Double = 0
                    For j = 0 To nS - 1
                        rss += (y(j) - mean) ^ 2
                    Next

                    cpd.Intercept = mean
                    cpd.Coeffs = New Double() {}
                    cpd.ParentIndices = New Integer() {}
                    cpd.ResidualVariance = rss / nS
                    cpd.ResidualSD = Math.Sqrt(cpd.ResidualVariance)
                    cpd.RSquared = 0.0

                    ' 对数似然
                    If cpd.ResidualVariance > 0.000000000000001 Then
                        totalLL += -nS / 2.0 * Math.Log(2 * Math.PI * cpd.ResidualVariance) - rss / (2 * cpd.ResidualVariance)
                    End If

                    ' BIC
                    Dim k As Integer = 2  ' μ, σ²
                    totalBIC += -2 * totalLL + k * Math.Log(nS)
                Else
                    ' 有父节点：线性回归 Xi = β0 + Σ βj·Paj + ε
                    Dim parentIndices As Integer() = node.Parents.ToArray()
                    Dim nP As Integer = parentIndices.Length

                    ' 构建设计矩阵
                    Dim X As Double(,) = New Double(nS - 1, nP) {}
                    For j = 0 To nS - 1
                        X(j, 0) = 1.0  ' 截距
                        For p = 0 To nP - 1
                            X(j, p + 1) = data.Matrix(parentIndices(p), j)
                        Next
                    Next

                    ' 最小二乘法求解 β = (X'X)^(-1) X'y
                    Dim beta As Double() = OLS.Solve(X, y, nS, nP + 1)

                    ' 计算残差
                    Dim predicted As Double() = New Double(nS - 1) {}
                    Dim rss As Double = 0
                    For j = 0 To nS - 1
                        predicted(j) = beta(0)
                        For p = 0 To nP - 1
                            predicted(j) += beta(p + 1) * X(j, p + 1)
                        Next
                        rss += (y(j) - predicted(j)) ^ 2
                    Next

                    ' 总平方和
                    Dim yMean As Double = y.Average()
                    Dim tss As Double = 0
                    For j = 0 To nS - 1
                        tss += (y(j) - yMean) ^ 2
                    Next

                    cpd.Intercept = beta(0)
                    cpd.Coeffs = New Double(nP - 1) {}
                    cpd.ParentIndices = parentIndices
                    For p = 0 To nP - 1
                        cpd.Coeffs(p) = beta(p + 1)
                    Next

                    cpd.ResidualVariance = rss / Math.Max(1, nS - nP - 1)
                    cpd.ResidualSD = Math.Sqrt(cpd.ResidualVariance)
                    cpd.RSquared = If(tss > 0, 1.0 - rss / tss, 0.0)

                    ' 对数似然
                    If cpd.ResidualVariance > 0.000000000000001 Then
                        totalLL += -nS / 2.0 * Math.Log(2 * Math.PI * cpd.ResidualVariance) - rss / (2 * cpd.ResidualVariance)
                    End If

                    ' BIC
                    Dim k As Integer = nP + 2  ' β0, β1...βp, σ²
                    Dim nodeBIC As Double = -2 * (-nS / 2.0 * Math.Log(2 * Math.PI * cpd.ResidualVariance) - rss / (2 * cpd.ResidualVariance)) + k * Math.Log(nS)
                    totalBIC += nodeBIC
                End If

                node.CPD = cpd
                sumR2 += cpd.RSquared
            Next

            Return New ParameterLearningResult() With {
                .Network = network,
                .TotalLogLikelihood = totalLL,
                .TotalBIC = totalBIC,
                .AverageRSquared = sumR2 / network.Nodes.Count,
                .ElapsedMs = (Now - t0).TotalMilliseconds
            }
        End Function
    End Module

End Namespace

