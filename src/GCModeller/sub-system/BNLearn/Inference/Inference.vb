' ============================================================
' Inference.vb - 概率推断引擎
' ============================================================
' 实现基于拟合好的高斯贝叶斯网络的概率推断：
'   1. 精确推断：利用 GBN 的线性高斯特性，直接计算条件分布
'   2. 采样推断：从网络中采样生成模拟数据
'   3. 预测推断：给定部分变量观测值，预测其余变量
' ============================================================

Namespace Inference

    ''' <summary>
    ''' 概率推断引擎
    ''' </summary>
    Public Class BnInferenceEngine

        Private _network As Core.BayesianNetwork

        ''' <summary>初始化推断引擎</summary>
        Public Sub New(network As Core.BayesianNetwork)
            _network = network
        End Sub

        ' ==================== 采样 ====================

        ''' <summary>
        ''' 从拟合的网络中采样生成模拟数据
        ''' 按拓扑排序依次采样：Xi ~ N(β0 + Σβj·Pa_j, σ²)
        ''' </summary>
        Public Function Sample(nSamples As Integer, Optional seed As Integer = 0) As Double(,)
            Dim rng As New Random(seed)
            Dim nG As Integer = _network.Nodes.Count
            Dim topoOrder As Integer() = _network.TopologicalSort()
            Dim samples As Double(,) = New Double(nG - 1, nSamples - 1) {}

            For s = 0 To nSamples - 1
                ' 按拓扑序依次采样每个节点
                For Each nodeIdx In topoOrder
                    Dim node As Core.BnNode = _network.Nodes(nodeIdx)
                    Dim cpd As Core.BnCPD = node.CPD
                    If cpd Is Nothing Then Continue For

                    ' 获取父节点值
                    Dim parentValues As Double() = New Double(node.Parents.Count - 1) {}
                    For p = 0 To node.Parents.Count - 1
                        parentValues(p) = samples(node.Parents(p), s)
                    Next

                    ' 从条件分布采样
                    samples(nodeIdx, s) = cpd.Sample(parentValues, rng)
                Next
            Next

            Return samples
        End Function

        ''' <summary>
        ''' 采样并返回 GeneExpressionData 格式
        ''' </summary>
        Public Function SampleAsGeneData(nSamples As Integer, Optional seed As Integer = 0) As Core.GeneExpressionData
            Dim matrix As Double(,) = Sample(nSamples, seed)
            Dim sampleNames As String() = Enumerable.Range(0, nSamples).Select(Function(i) "Sample_" & i).ToArray()

            Return New Core.GeneExpressionData() With {
                .GeneNames = _network.Nodes.Select(Function(n) n.Name).ToArray(),
                .SampleNames = sampleNames,
                .Matrix = matrix,
                .TimePoints = Enumerable.Repeat(0.0, nSamples).ToArray()
            }
        End Function

        ' ==================== 精确推断（GBN 闭式解） ====================

        ''' <summary>
        ''' 精确条件推断（高斯贝叶斯网络闭式解）
        ''' 给定证据变量值，计算查询变量的条件分布
        ''' 
        ''' 对于线性高斯模型：
        '''   P(X_query | X_evidence = e) = N(μ_cond, Σ_cond)
        '''   μ_cond = μ_q + Σ_qe · Σ_ee^(-1) · (e - μ_e)
        '''   Σ_cond = Σ_qq - Σ_qe · Σ_ee^(-1) · Σ_eq
        ''' </summary>
        Public Function ConditionalInference(evidenceIndices As Integer(),
                                             evidenceValues As Double(),
                                             queryIndices As Integer()) As (Means As Double(), Variances As Double())

            Dim nG As Integer = _network.Nodes.Count

            ' 1. 计算联合分布的均值向量和协方差矩阵
            Dim mu As Double() = ComputeJointMeans()
            Dim sigma As Double(,) = ComputeJointCovariance()

            ' 2. 提取子矩阵
            Dim nE As Integer = evidenceIndices.Length
            Dim nQ As Integer = queryIndices.Length

            ' Σ_ee
            Dim sigmaEE As Double(,) = New Double(nE - 1, nE - 1) {}
            For i = 0 To nE - 1
                For j = 0 To nE - 1
                    sigmaEE(i, j) = sigma(evidenceIndices(i), evidenceIndices(j))
                Next
            Next

            ' Σ_qe
            Dim sigmaQE As Double(,) = New Double(nQ - 1, nE - 1) {}
            For i = 0 To nQ - 1
                For j = 0 To nE - 1
                    sigmaQE(i, j) = sigma(queryIndices(i), evidenceIndices(j))
                Next
            Next

            ' Σ_qq
            Dim sigmaQQ As Double(,) = New Double(nQ - 1, nQ - 1) {}
            For i = 0 To nQ - 1
                For j = 0 To nQ - 1
                    sigmaQQ(i, j) = sigma(queryIndices(i), queryIndices(j))
                Next
            Next

            ' 3. 计算条件分布
            Dim invSigmaEE As Double(,) = StructureLearning.BnStructureLearner.MatrixInverse(sigmaEE, nE)
            If invSigmaEE Is Nothing Then
                ' 奇异矩阵，返回先验均值
                Dim means As Double() = New Double(nQ - 1) {}
                Dim vars As Double() = New Double(nQ - 1) {}
                For i = 0 To nQ - 1
                    means(i) = mu(queryIndices(i))
                    vars(i) = sigma(queryIndices(i), queryIndices(i))
                Next
                Return (means, vars)
            End If

            ' e - μ_e
            Dim eMinusMuE As Double() = New Double(nE - 1) {}
            For i = 0 To nE - 1
                eMinusMuE(i) = evidenceValues(i) - mu(evidenceIndices(i))
            Next

            ' Σ_qe · Σ_ee^(-1) · (e - μ_e)
            Dim sigmaQEInvEE As Double() = New Double(nQ - 1) {}
            For i = 0 To nQ - 1
                Dim sum As Double = 0
                For j = 0 To nE - 1
                    For k = 0 To nE - 1
                        sum += sigmaQE(i, k) * invSigmaEE(k, j) * eMinusMuE(j)
                    Next
                Next
                sigmaQEInvEE(i) = sum
            Next

            ' 条件均值
            Dim condMeans As Double() = New Double(nQ - 1) {}
            For i = 0 To nQ - 1
                condMeans(i) = mu(queryIndices(i)) + sigmaQEInvEE(i)
            Next

            ' 条件方差（对角线）
            Dim condVars As Double() = New Double(nQ - 1) {}
            For i = 0 To nQ - 1
                Dim sum As Double = sigmaQQ(i, i)
                For j = 0 To nE - 1
                    For k = 0 To nE - 1
                        sum -= sigmaQE(i, k) * invSigmaEE(k, j) * sigmaQE(i, j)  ' 简化
                    Next
                Next
                condVars(i) = Math.Max(1e-10, sum)
            Next

            Return (condMeans, condVars)
        End Function

        ''' <summary>
        ''' 计算联合分布均值向量
        ''' 对于 GBN：μ = (I - B)^(-1) · β0
        ''' 其中 B 是回归系数矩阵，β0 是截距向量
        ''' </summary>
        Private Function ComputeJointMeans() As Double()
            Dim nG As Integer = _network.Nodes.Count
            Dim topoOrder As Integer() = _network.TopologicalSort()

            ' 构建回归系数矩阵 B 和截距向量
            Dim B As Double(,) = New Double(nG - 1, nG - 1) {}
            Dim beta0 As Double() = New Double(nG - 1) {}

            For Each nodeIdx In topoOrder
                Dim node As Core.BnNode = _network.Nodes(nodeIdx)
                Dim cpd As Core.BnCPD = node.CPD
                If cpd Is Nothing Then Continue For

                beta0(nodeIdx) = cpd.Intercept
                For p = 0 To node.Parents.Count - 1
                    B(nodeIdx, node.Parents(p)) = cpd.Coeffs(p)
                Next
            Next

            ' μ = (I - B)^(-1) · β0
            Dim ImB As Double(,) = New Double(nG - 1, nG - 1) {}
            For i = 0 To nG - 1
                For j = 0 To nG - 1
                    ImB(i, j) = -B(i, j)
                Next
                ImB(i, i) += 1.0
            Next

            Dim invImB As Double(,) = StructureLearning.BnStructureLearner.MatrixInverse(ImB, nG)
            If invImB Is Nothing Then Return beta0

            Dim mu As Double() = New Double(nG - 1) {}
            For i = 0 To nG - 1
                For j = 0 To nG - 1
                    mu(i) += invImB(i, j) * beta0(j)
                Next
            Next

            Return mu
        End Function

        ''' <summary>
        ''' 计算联合分布协方差矩阵
        ''' Σ = (I - B)^(-1) · D · ((I - B)^(-1))'
        ''' 其中 D = diag(σ1², σ2², ..., σn²)
        ''' </summary>
        Private Function ComputeJointCovariance() As Double(,)
            Dim nG As Integer = _network.Nodes.Count
            Dim topoOrder As Integer() = _network.TopologicalSort()

            ' 构建回归系数矩阵 B 和残差方差对角阵 D
            Dim B As Double(,) = New Double(nG - 1, nG - 1) {}
            Dim D As Double(,) = New Double(nG - 1, nG - 1) {}

            For Each nodeIdx In topoOrder
                Dim node As Core.BnNode = _network.Nodes(nodeIdx)
                Dim cpd As Core.BnCPD = node.CPD
                If cpd Is Nothing Then Continue For

                D(nodeIdx, nodeIdx) = cpd.ResidualVariance
                For p = 0 To node.Parents.Count - 1
                    B(nodeIdx, node.Parents(p)) = cpd.Coeffs(p)
                Next
            Next

            ' (I - B)^(-1)
            Dim ImB As Double(,) = New Double(nG - 1, nG - 1) {}
            For i = 0 To nG - 1
                For j = 0 To nG - 1
                    ImB(i, j) = -B(i, j)
                Next
                ImB(i, i) += 1.0
            Next

            Dim invImB As Double(,) = StructureLearning.BnStructureLearner.MatrixInverse(ImB, nG)
            If invImB Is Nothing Then Return D

            ' Σ = invImB · D · invImB'
            Dim sigma As Double(,) = New Double(nG - 1, nG - 1) {}
            For i = 0 To nG - 1
                For j = 0 To nG - 1
                    Dim sum As Double = 0
                    For k = 0 To nG - 1
                        sum += invImB(i, k) * D(k, k) * invImB(j, k)
                    Next
                    sigma(i, j) = sum
                Next
            Next

            Return sigma
        End Function

        ' ==================== 预测 ====================

        ''' <summary>
        ''' 给定部分基因的表达值，预测其余基因的表达值
        ''' 使用采样推断（更稳健）
        ''' </summary>
        Public Function Predict(observedGeneIndices As Integer(),
                                observedValues As Double(),
                                nSamples As Integer,
                                Optional seed As Integer = 0) As (PredictedMeans As Double(), PredictedSDs As Double())

            Dim nG As Integer = _network.Nodes.Count
            Dim rng As New Random(seed)
            Dim topoOrder As Integer() = _network.TopologicalSort()

            ' 记录每个基因的采样值
            Dim sampleValues As New List(Of Double())()

            For s = 0 To nSamples - 1
                Dim values As Double() = New Double(nG - 1) {}
                Dim isObserved As Boolean() = New Boolean(nG - 1) {}

                ' 设置观测值
                For i = 0 To observedGeneIndices.Length - 1
                    values(observedGeneIndices(i)) = observedValues(i)
                    isObserved(observedGeneIndices(i)) = True
                Next

                ' 按拓扑序采样未观测的节点
                For Each nodeIdx In topoOrder
                    If isObserved(nodeIdx) Then Continue For

                    Dim node As Core.BnNode = _network.Nodes(nodeIdx)
                    Dim cpd As Core.BnCPD = node.CPD
                    If cpd Is Nothing Then Continue For

                    Dim parentValues As Double() = New Double(node.Parents.Count - 1) {}
                    For p = 0 To node.Parents.Count - 1
                        parentValues(p) = values(node.Parents(p))
                    Next

                    values(nodeIdx) = cpd.Sample(parentValues, rng)
                Next

                sampleValues.Add(values)
            Next

            ' 计算均值和标准差
            Dim means As Double() = New Double(nG - 1) {}
            Dim sds As Double() = New Double(nG - 1) {}

            For i = 0 To nG - 1
                Dim sum As Double = 0
                For s = 0 To nSamples - 1
                    sum += sampleValues(s)(i)
                Next
                means(i) = sum / nSamples

                Dim ss As Double = 0
                For s = 0 To nSamples - 1
                    ss += (sampleValues(s)(i) - means(i)) ^ 2
                Next
                sds(i) = Math.Sqrt(ss / Math.Max(1, nSamples - 1))
            Next

            Return (means, sds)
        End Function

    End Class

End Namespace
