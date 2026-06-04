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

Namespace ParameterLearning

    ''' <summary>
    ''' 参数学习结果
    ''' </summary>
    Public Class ParameterLearningResult

        ''' <summary>拟合后的网络（含CPD参数）</summary>
        Public Property Network As Core.BayesianNetwork

        ''' <summary>总对数似然</summary>
        Public Property TotalLogLikelihood As Double

        ''' <summary>总 BIC</summary>
        Public Property TotalBIC As Double

        ''' <summary>平均 R²</summary>
        Public Property AverageRSquared As Double

        ''' <summary>参数学习耗时（毫秒）</summary>
        Public Property ElapsedMs As Long

    End Class

    ''' <summary>
    ''' 贝叶斯网络参数学习器
    ''' </summary>
    Public Class BnParameterLearner

        Private _data As Core.GeneExpressionData

        ''' <summary>
        ''' 从数据学习网络参数
        ''' </summary>
        Public Function Learn(network As Core.BayesianNetwork,
                              data As Core.GeneExpressionData) As ParameterLearningResult

            Dim t0 As Date = Now

            _data = data
            Dim nS As Integer = data.NSample
            Dim totalLL As Double = 0
            Dim totalBIC As Double = 0
            Dim sumR2 As Double = 0

            ' 按拓扑排序依次拟合每个节点
            Dim topoOrder As Integer() = network.TopologicalSort()

            For Each nodeIdx In topoOrder
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
                    If cpd.ResidualVariance > 1e-15 Then
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
                    Dim beta As Double() = OLS(X, y, nS, nP + 1)

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
                    If cpd.ResidualVariance > 1e-15 Then
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

        ''' <summary>
        ''' OLS 最小二乘法
        ''' </summary>
        Private Function OLS(X As Double(,), y As Double(), nS As Integer, nP As Integer) As Double()
            ' X'X
            Dim XtX As Double(,) = New Double(nP - 1, nP - 1) {}
            For i = 0 To nP - 1
                For j = 0 To nP - 1
                    Dim sum As Double = 0
                    For k = 0 To nS - 1
                        sum += X(k, i) * X(k, j)
                    Next
                    XtX(i, j) = sum
                Next
            Next

            ' X'y
            Dim Xty As Double() = New Double(nP - 1) {}
            For i = 0 To nP - 1
                Dim sum As Double = 0
                For k = 0 To nS - 1
                    sum += X(k, i) * y(k)
                Next
                Xty(i) = sum
            Next

            ' 求逆
            Dim invXtX As Double(,) = StructureLearning.BnStructureLearner.MatrixInverse(XtX, nP)
            If invXtX Is Nothing Then
                Dim result As Double() = New Double(nP - 1) {}
                result(0) = y.Average()
                Return result
            End If

            ' β = (X'X)^(-1) X'y
            Dim beta As Double() = New Double(nP - 1) {}
            For i = 0 To nP - 1
                Dim sum As Double = 0
                For j = 0 To nP - 1
                    sum += invXtX(i, j) * Xty(j)
                Next
                beta(i) = sum
            Next

            Return beta
        End Function

    End Class

End Namespace
