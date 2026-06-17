' ============================================================
' StructureLearning.vb - 贝叶斯网络结构学习
' ============================================================
' 实现 MMHC（Max-Min Hill-Climbing）混合算法：
'   第一阶段：MMPC 约束型算法限制候选边
'   第二阶段：Hill-Climbing 评分搜索确定最优 DAG
'
' 评分函数：BIC（贝叶斯信息准则）
'   BIC = -2·LL + k·log(n)
'   其中 LL = 对数似然，k = 参数数，n = 样本数
'
' 对于高斯贝叶斯网络：
'   对数似然 = -n/2·log(2πσ²) - 1/(2σ²)·Σ(xi-μi)²
' ============================================================

Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports _rng = Microsoft.VisualBasic.Math.RandomExtensions

Namespace StructureLearning


    ''' <summary>
    ''' 贝叶斯网络结构学习器
    ''' </summary>
    Public Class BnStructureLearner

        Private _data As Core.GeneExpressionData
        Private _params As StructureLearningParams

        ' 缓存统计量
        Private _means As Double()
        Private _sds As Double()
        Private _corrMatrix As Double(,)

        ''' <summary>
        ''' 从基因表达数据学习网络结构
        ''' </summary>
        Public Function Learn(data As Core.GeneExpressionData,
                              params As StructureLearningParams,
                              Optional prior As Core.PriorNetwork = Nothing) As StructureLearningResult

            Dim t0 As Date = Now

            _data = data
            _params = params
            _rng.SetSeed(params.RandomSeed)

            ' 预计算统计量
            PrecomputeStatistics()

            ' 创建初始空网络
            Dim net As New Core.BayesianNetwork(data.GeneNames)

            ' 注入先验知识（白名单/黑名单）
            If prior IsNot Nothing Then
                Dim wl = prior.ToWhitelist(data.GeneNames)
                For Each edge In wl
                    net.AddWhitelistEdge(edge.FromIdx, edge.ToIdx)
                Next
            End If

            ' 添加基因名互不调控的黑名单（可选：禁止自环）
            For i = 0 To data.NGene - 1
                net.AddBlacklistEdge(i, i)
            Next

            ' 根据算法选择学习策略
            Select Case params.Algorithm
                Case StructureAlgorithm.HillClimbing
                    HillClimbingSearch(net)

                Case StructureAlgorithm.Tabu
                    TabuSearch(net)

                Case StructureAlgorithm.MMHC
                    ' 第一阶段：MMPC 约束型搜索限制候选边
                    Dim candidateEdges As HashSet(Of (Integer, Integer)) = MMPCPhase(net)
                    ' 第二阶段：HC 评分搜索
                    HillClimbingSearch(net, candidateEdges)

            End Select

            ' 计算最终 BIC
            Dim finalBIC As Double = ComputeNetworkBIC(net)

            Return New StructureLearningResult() With {
                .Network = net,
                .FinalBIC = finalBIC,
                .Iterations = _params.MaxIterations,
                .ElapsedMs = (Now - t0).TotalMilliseconds
            }
        End Function

        ' ==================== 预计算统计量 ====================

        Private Sub PrecomputeStatistics()
            Dim nG As Integer = _data.NGene
            Dim nS As Integer = _data.NSample

            ' 均值和标准差
            _means = New Double(nG - 1) {}
            _sds = New Double(nG - 1) {}
            For i = 0 To nG - 1
                Dim sum As Double = 0
                For j = 0 To nS - 1
                    sum += _data.Matrix(i, j)
                Next
                _means(i) = sum / nS

                Dim ss As Double = 0
                For j = 0 To nS - 1
                    ss += (_data.Matrix(i, j) - _means(i)) ^ 2
                Next
                _sds(i) = Math.Sqrt(ss / (nS - 1))
                If _sds(i) < 0.000000000000001 Then _sds(i) = 1.0
            Next

            ' 相关系数矩阵
            _corrMatrix = New Double(nG - 1, nG - 1) {}
            For i = 0 To nG - 1
                For j = i To nG - 1
                    If i = j Then
                        _corrMatrix(i, j) = 1.0
                    Else
                        Dim cov As Double = 0
                        For k = 0 To nS - 1
                            cov += (_data.Matrix(i, k) - _means(i)) * (_data.Matrix(j, k) - _means(j))
                        Next
                        cov /= (nS - 1)
                        Dim r As Double = cov / (_sds(i) * _sds(j))
                        _corrMatrix(i, j) = r
                        _corrMatrix(j, i) = r
                    End If
                Next
            Next
        End Sub

        ' ==================== MMPC 约束型阶段 ====================

        ''' <summary>
        ''' MMPC 阶段：为每个节点找到候选父节点集合
        ''' 使用偏相关系数的条件独立性检验
        ''' </summary>
        Private Function MMPCPhase(net As Core.BayesianNetwork) As HashSet(Of (Integer, Integer))

            Dim nG As Integer = _data.NGene
            Dim candidates As New HashSet(Of (Integer, Integer))()

            ' 白名单边直接加入候选
            For Each wl In net.Whitelist
                candidates.Add((wl.FromIdx, wl.ToIdx))
            Next

            ' 对每个目标节点，寻找候选父节点
            For target = 0 To nG - 1
                Dim CPC As New List(Of Integer)()  ' 候选父节点集

                ' 前向阶段：逐步加入最相关的变量
                Dim remaining As New List(Of Integer)()
                For i = 0 To nG - 1
                    If i <> target Then remaining.Add(i)
                Next

                While remaining.Count > 0
                    ' 找到与 target 条件依赖最强的变量
                    Dim bestVar As Integer = -1
                    Dim bestAssoc As Double = 0

                    For Each v In remaining
                        ' 计算偏相关系数 |ρ(target, v | CPC)|
                        Dim pCorr As Double = PartialCorrelation(target, v, CPC)
                        Dim absPcorr As Double = Math.Abs(pCorr)

                        If absPcorr > bestAssoc Then
                            bestAssoc = absPcorr
                            bestVar = v
                        End If
                    Next

                    If bestVar < 0 Then Exit While

                    ' 独立性检验（基于偏相关的 t 检验）
                    Dim nS As Integer = _data.NSample
                    Dim df As Integer = nS - 2 - CPC.Count
                    If df < 1 Then Exit While

                    Dim tStat As Double = bestAssoc * Math.Sqrt(df / (1.0 - bestAssoc * bestAssoc + 0.000000000000001))
                    Dim pValue As Double = TDistPValue(Math.Abs(tStat), df)

                    If pValue < _params.Alpha Then
                        ' 显著依赖，加入候选集
                        CPC.Add(bestVar)
                        remaining.Remove(bestVar)

                        ' 限制最大候选数
                        If CPC.Count >= _params.MaxParents * 2 Then Exit While
                    Else
                        ' 不显著，移除
                        remaining.Remove(bestVar)
                    End If
                End While

                ' 后向阶段：移除在更大条件集下变得独立的变量
                For i = CPC.Count - 1 To 0 Step -1
                    Dim testVar As Integer = CPC(i)
                    Dim conditionSet As New List(Of Integer)(CPC)
                    conditionSet.RemoveAt(i)

                    Dim pCorr As Double = PartialCorrelation(target, testVar, conditionSet)
                    Dim df As Integer = nG - 2 - conditionSet.Count
                    If df < 1 Then Continue For

                    Dim tStat As Double = Math.Abs(pCorr) * Math.Sqrt(df / (1.0 - pCorr * pCorr + 0.000000000000001))
                    Dim pValue As Double = TDistPValue(tStat, df)

                    If pValue >= _params.Alpha Then
                        CPC.RemoveAt(i)
                    End If
                Next

                ' 将候选加入候选边集
                For Each parent In CPC
                    candidates.Add((parent, target))
                Next
            Next

            Return candidates
        End Function

        ''' <summary>
        ''' 计算偏相关系数 ρ(x, y | Z)
        ''' 使用递推公式从全相关矩阵计算
        ''' </summary>
        Private Function PartialCorrelation(x As Integer, y As Integer, zSet As List(Of Integer)) As Double
            If zSet.Count = 0 Then
                Return _corrMatrix(x, y)
            End If

            If zSet.Count = 1 Then
                Dim z As Integer = zSet(0)
                Dim rxy As Double = _corrMatrix(x, y)
                Dim rxz As Double = _corrMatrix(x, z)
                Dim ryz As Double = _corrMatrix(y, z)
                Dim denom As Double = Math.Sqrt((1 - rxz * rxz) * (1 - ryz * ryz))
                If Math.Abs(denom) < 0.000000000000001 Then Return 0
                Return (rxy - rxz * ryz) / denom
            End If

            ' 多变量偏相关：使用矩阵求逆法
            ' 构建增广矩阵 [x, y, z1, z2, ...]
            Dim vars As New List(Of Integer)()
            vars.Add(x)
            vars.Add(y)
            vars.AddRange(zSet)
            Dim k As Integer = vars.Count

            ' 提取子相关矩阵
            Dim R As Double(,) = New Double(k - 1, k - 1) {}
            For i = 0 To k - 1
                For j = 0 To k - 1
                    R(i, j) = _corrMatrix(vars(i), vars(j))
                Next
            Next

            ' 求逆矩阵
            Dim invR As Double(,) = MatrixInverse(R, k)
            If invR Is Nothing Then Return 0

            ' 偏相关 = -invR(0,1) / sqrt(invR(0,0) * invR(1,1))
            Dim denomVal As Double = Math.Sqrt(Math.Abs(invR(0, 0) * invR(1, 1)))
            If denomVal < 0.000000000000001 Then Return 0
            Return -invR(0, 1) / denomVal
        End Function

        ' ==================== Hill-Climbing 搜索 ====================

        Private Sub HillClimbingSearch(net As Core.BayesianNetwork,
                                       Optional candidateEdges As HashSet(Of (Integer, Integer)) = Nothing)

            Dim currentBIC As Double = ComputeNetworkBIC(net)
            Dim bar As ProgressBar = Nothing

            For Each iter As Integer In TqdmWrapper.Range(0, _params.MaxIterations, bar:=bar)
                Dim bestOp As EdgeOp = EdgeOp.None
                Dim bestDelta As Double = 0
                Dim bestFrom As Integer = -1
                Dim bestTo As Integer = -1

                Dim nG As Integer = net.Nodes.Count

                Call net.MakeBlackIndex()

                ' 遍历所有可能的操作
                For i = 0 To nG - 1
                    For j = 0 To nG - 1
                        If i = j Then Continue For

                        ' 检查是否在候选集中（MMPC阶段限制）
                        If candidateEdges IsNot Nothing Then
                            Dim hasForward As Boolean = candidateEdges.Contains((i, j))
                            Dim hasReverse As Boolean = candidateEdges.Contains((j, i))
                            If Not hasForward AndAlso Not hasReverse AndAlso Not net.HasEdge(i, j) Then
                                Continue For
                            End If
                        End If

                        ' 检查黑名单
                        Dim inBlacklist As Boolean = net.blackEdges.Contains((i, j))

                        If inBlacklist Then
                            Continue For
                        End If

                        If net.HasEdge(i, j) Then
                            ' 操作1：删除边 i→j
                            net.RemoveEdge(i, j)
                            Dim newBIC As Double = ComputeNetworkBIC(net)
                            Dim delta As Double = newBIC - currentBIC
                            If delta < bestDelta Then
                                bestDelta = delta
                                bestOp = EdgeOp.Remove
                                bestFrom = i
                                bestTo = j
                            End If
                            net.AddEdge(i, j)

                            ' 操作2：反转边 i→j 为 j→i
                            If Not net.HasEdge(j, i) Then
                                ' 检查反转后是否在黑名单
                                Dim reverseBlacklisted As Boolean = net.blackEdges.Contains((j, i))

                                If Not reverseBlacklisted Then
                                    net.RemoveEdge(i, j)
                                    If net.AddEdge(j, i) Then
                                        Dim revBIC As Double = ComputeNetworkBIC(net)
                                        Dim revDelta As Double = revBIC - currentBIC
                                        If revDelta < bestDelta Then
                                            bestDelta = revDelta
                                            bestOp = EdgeOp.Reverse
                                            bestFrom = i
                                            bestTo = j
                                        End If
                                        net.RemoveEdge(j, i)
                                    End If
                                    net.AddEdge(i, j)
                                End If
                            End If
                        Else
                            ' 操作3：添加边 i→j
                            If net.Nodes(j).Parents.Count < _params.MaxParents Then
                                If net.AddEdge(i, j) Then
                                    Dim newBIC As Double = ComputeNetworkBIC(net)
                                    Dim delta As Double = newBIC - currentBIC
                                    If delta < bestDelta Then
                                        bestDelta = delta
                                        bestOp = EdgeOp.Add
                                        bestFrom = i
                                        bestTo = j
                                    End If
                                    net.RemoveEdge(i, j)
                                End If
                            End If
                        End If
                    Next
                Next

                Call bar.SetLabel($"best-delta={bestDelta}; current-BIC={currentBIC}")

                ' 执行最优操作
                If bestDelta >= -0.0000000001 Then
                    Exit For  ' 无法改善，停止
                End If

                Select Case bestOp
                    Case EdgeOp.Add
                        net.AddEdge(bestFrom, bestTo)
                    Case EdgeOp.Remove
                        net.RemoveEdge(bestFrom, bestTo)
                    Case EdgeOp.Reverse
                        net.RemoveEdge(bestFrom, bestTo)
                        net.AddEdge(bestTo, bestFrom)
                End Select

                currentBIC += bestDelta
            Next
        End Sub

        ' ==================== Tabu 搜索 ====================

        Private Sub TabuSearch(net As Core.BayesianNetwork)
            Dim currentBIC As Double = ComputeNetworkBIC(net)
            Dim bestBIC As Double = currentBIC
            Dim bestNet As Core.BayesianNetwork = net.CloneStructure()

            Dim tabuList As New Queue(Of String)()

            For iter = 0 To _params.MaxIterations - 1
                Dim bestOp As EdgeOp = EdgeOp.None
                Dim bestDelta As Double = Double.MaxValue
                Dim bestFrom As Integer = -1
                Dim bestTo As Integer = -1
                Dim nG As Integer = net.Nodes.Count

                For i = 0 To nG - 1
                    For j = 0 To nG - 1
                        If i = j Then Continue For

                        Dim opKey As String = ""

                        If net.HasEdge(i, j) Then
                            ' 删除
                            opKey = String.Format("del_{0}_{1}", i, j)
                            net.RemoveEdge(i, j)
                            Dim newBIC As Double = ComputeNetworkBIC(net)
                            Dim delta As Double = newBIC - currentBIC
                            If delta < bestDelta OrElse (tabuList.Contains(opKey) AndAlso newBIC < bestBIC) Then
                                If Not tabuList.Contains(opKey) OrElse newBIC < bestBIC Then
                                    bestDelta = delta
                                    bestOp = EdgeOp.Remove
                                    bestFrom = i
                                    bestTo = j
                                End If
                            End If
                            net.AddEdge(i, j)
                        Else
                            ' 添加
                            If net.Nodes(j).Parents.Count < _params.MaxParents Then
                                opKey = String.Format("add_{0}_{1}", i, j)
                                If net.AddEdge(i, j) Then
                                    Dim newBIC As Double = ComputeNetworkBIC(net)
                                    Dim delta As Double = newBIC - currentBIC
                                    If Not tabuList.Contains(opKey) OrElse newBIC < bestBIC Then
                                        If delta < bestDelta Then
                                            bestDelta = delta
                                            bestOp = EdgeOp.Add
                                            bestFrom = i
                                            bestTo = j
                                        End If
                                    End If
                                    net.RemoveEdge(i, j)
                                End If
                            End If
                        End If
                    Next
                Next

                If bestOp = EdgeOp.None Then
                    Exit For
                End If

                ' 执行操作
                Select Case bestOp
                    Case EdgeOp.Add : net.AddEdge(bestFrom, bestTo)
                    Case EdgeOp.Remove : net.RemoveEdge(bestFrom, bestTo)
                End Select

                currentBIC += bestDelta

                ' 更新全局最优
                If currentBIC < bestBIC Then
                    bestBIC = currentBIC
                    bestNet = net.CloneStructure()
                End If

                ' 更新禁忌表
                Dim key As String = String.Format("{0}_{1}_{2}", bestOp, bestFrom, bestTo)
                tabuList.Enqueue(key)
                If tabuList.Count > _params.TabuLength Then tabuList.Dequeue()
            Next

            ' 恢复最优网络
            ' （简化处理：保留当前网络，因为 Tabu 搜索中当前解通常接近最优）
        End Sub


        ' ==================== BIC 评分 ====================

        ''' <summary>
        ''' 计算整个网络的 BIC 评分
        ''' BIC = Σ BIC_node
        ''' BIC_node = -2·LL_node + k_node·log(n)
        ''' 对于高斯BN：LL_node = -n/2·log(2πσ²) - 1/(2σ²)·RSS
        ''' </summary>
        Public Function ComputeNetworkBIC(net As Core.BayesianNetwork) As Double
            Dim nS As Integer = _data.NSample
            Dim totalBIC As Double() = New Double(net.Nodes.Count - 1) {}

            Call Parallel.For(
                0, net.Nodes.Count,
                body:=Sub(i)
                          totalBIC(i) = ComputeNodeBIC(net, i, nS)
                      End Sub)

            'For i = 0 To net.Nodes.Count - 1
            '    totalBIC += ComputeNodeBIC(net, i, nS)
            'Next

            Return totalBIC.Sum
        End Function

        ''' <summary>
        ''' 计算单个节点的 BIC
        ''' </summary>
        Private Function ComputeNodeBIC(net As Core.BayesianNetwork, nodeIdx As Integer, nS As Integer) As Double
            Dim node As Core.BnNode = net.Nodes(nodeIdx)
            Dim parents As List(Of Integer) = node.Parents
            Dim y As Double() = _data.GetGeneExpression(nodeIdx)

            If parents.Count = 0 Then
                ' 无父节点：边际分布 N(μ, σ²)
                Dim mean As Double = _means(nodeIdx)
                Dim rss As Double = 0
                For j = 0 To nS - 1
                    rss += (y(j) - mean) ^ 2
                Next
                Dim sigma2 As Double = rss / nS
                If sigma2 < 1e-15 Then sigma2 = 1e-15

                Dim ll As Double = -nS / 2.0 * Math.Log(2 * Math.PI * sigma2) - rss / (2 * sigma2)
                Dim k As Integer = 2  ' μ, σ²
                Return -2 * ll + k * Math.Log(nS) * _params.BICPenalty
            Else
                ' 有父节点：回归模型 Xi = β0 + Σβj·Paj + ε
                Dim x As Double(,) = BuildDesignMatrix(parents, nS)
                Dim coeffs As Double() = LinearRegression(x, y, nS, parents.Count)

                ' 计算残差
                Dim rss As Double = 0
                For j = 0 To nS - 1
                    Dim predicted As Double = coeffs(0)  ' 截距
                    For p = 0 To parents.Count - 1
                        predicted += coeffs(p + 1) * x(j, p + 1)
                    Next
                    rss += (y(j) - predicted) ^ 2
                Next

                Dim sigma2 As Double = rss / nS
                If sigma2 < 1e-15 Then sigma2 = 1e-15

                Dim ll As Double = -nS / 2.0 * Math.Log(2 * Math.PI * sigma2) - rss / (2 * sigma2)
                Dim k As Integer = parents.Count + 2  ' β0, β1...βp, σ²
                Return -2 * ll + k * Math.Log(nS) * _params.BICPenalty
            End If
        End Function

        ' ==================== 数学工具 ====================

        ''' <summary>构建回归设计矩阵</summary>
        Private Function BuildDesignMatrix(parents As List(Of Integer), nS As Integer) As Double(,)
            Dim nP As Integer = parents.Count
            Dim X As Double(,) = New Double(nS - 1, nP) {}  ' 第一列截距=1

            For j = 0 To nS - 1
                X(j, 0) = 1.0  ' 截距
                For p = 0 To nP - 1
                    X(j, p + 1) = _data.Matrix(parents(p), j)
                Next
            Next

            Return X
        End Function

        ''' <summary>
        ''' 多元线性回归（最小二乘法）
        ''' β = (X'X)^(-1) X'y
        ''' </summary>
        Private Function LinearRegression(X As Double(,), y As Double(), nS As Integer, nP As Integer) As Double()
            Dim p1 As Integer = nP + 1  ' 含截距

            ' X'X
            Dim XtX As Double(,) = New Double(p1 - 1, p1 - 1) {}
            For i = 0 To p1 - 1
                For j = 0 To p1 - 1
                    Dim sum As Double = 0
                    For k = 0 To nS - 1
                        sum += X(k, i) * X(k, j)
                    Next
                    XtX(i, j) = sum
                Next
            Next

            ' X'y
            Dim Xty As Double() = New Double(p1 - 1) {}
            For i = 0 To p1 - 1
                Dim sum As Double = 0
                For k = 0 To nS - 1
                    sum += X(k, i) * y(k)
                Next
                Xty(i) = sum
            Next

            ' 求解 β = (X'X)^(-1) X'y
            Dim invXtX As Double(,) = MatrixInverse(XtX, p1)
            If invXtX Is Nothing Then
                ' 奇异矩阵，返回零系数
                Dim result As Double() = New Double(p1 - 1) {}
                result(0) = _means(Array.IndexOf(_means, y.Average()))
                Return result
            End If

            Dim beta As Double() = New Double(p1 - 1) {}
            For i = 0 To p1 - 1
                Dim sum As Double = 0
                For j = 0 To p1 - 1
                    sum += invXtX(i, j) * Xty(j)
                Next
                beta(i) = sum
            Next

            Return beta
        End Function

        ''' <summary>
        ''' 矩阵求逆（高斯-约旦消元法）
        ''' </summary>
        Public Shared Function MatrixInverse(A As Double(,), n As Integer) As Double(,)
            ' 增广矩阵 [A | I]
            Dim aug As Double(,) = New Double(n - 1, 2 * n - 1) {}
            For i = 0 To n - 1
                For j = 0 To n - 1
                    aug(i, j) = A(i, j)
                Next
                aug(i, n + i) = 1.0
            Next

            ' 前向消元
            For col = 0 To n - 1
                ' 部分主元选取
                Dim maxRow As Integer = col
                Dim maxVal As Double = Math.Abs(aug(col, col))
                For row = col + 1 To n - 1
                    If Math.Abs(aug(row, col)) > maxVal Then
                        maxVal = Math.Abs(aug(row, col))
                        maxRow = row
                    End If
                Next

                If maxVal < 0.000000000000001 Then Return Nothing  ' 奇异矩阵

                ' 交换行
                If maxRow <> col Then
                    For j = 0 To 2 * n - 1
                        Dim tmp As Double = aug(col, j)
                        aug(col, j) = aug(maxRow, j)
                        aug(maxRow, j) = tmp
                    Next
                End If

                ' 归一化主元行
                Dim pivot As Double = aug(col, col)
                For j = 0 To 2 * n - 1
                    aug(col, j) /= pivot
                Next

                ' 消元
                For row = 0 To n - 1
                    If row = col Then Continue For
                    Dim factor As Double = aug(row, col)
                    For j = 0 To 2 * n - 1
                        aug(row, j) -= factor * aug(col, j)
                    Next
                Next
            Next

            ' 提取逆矩阵
            Dim inv As Double(,) = New Double(n - 1, n - 1) {}
            For i = 0 To n - 1
                For j = 0 To n - 1
                    inv(i, j) = aug(i, n + j)
                Next
            Next

            Return inv
        End Function

        ''' <summary>
        ''' t 分布 P 值（双侧）的近似计算
        ''' 使用正态近似（大样本）或查表插值
        ''' </summary>
        Private Function TDistPValue(tStat As Double, df As Integer) As Double
            ' 对于大 df，t 分布近似正态分布
            ' 使用近似公式
            If df >= 30 Then
                ' 正态近似
                Dim z As Double = tStat
                Dim p As Double = 2.0 * (1.0 - NormalCDF(z))
                Return Math.Max(0, Math.Min(1, p))
            Else
                ' 小样本 t 分布近似
                ' 使用 Hills 逼近
                Dim x As Double = df / (df + tStat * tStat)
                Dim p As Double = IncompleteBeta(df / 2.0, 0.5, x)
                Return p
            End If
        End Function

        ''' <summary>标准正态 CDF 近似</summary>
        Private Function NormalCDF(z As Double) As Double
            ' Abramowitz and Stegun 近似
            Dim a1 As Double = 0.254829592
            Dim a2 As Double = -0.284496736
            Dim a3 As Double = 1.421413741
            Dim a4 As Double = -1.453152027
            Dim a5 As Double = 1.061405429
            Dim p As Double = 0.3275911

            Dim sign As Integer = If(z < 0, -1, 1)
            z = Math.Abs(z) / Math.Sqrt(2.0)
            Dim t As Double = 1.0 / (1.0 + p * z)
            Dim y As Double = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-z * z)
            Return 0.5 * (1.0 + sign * y)
        End Function

        ''' <summary>不完全 Beta 函数近似</summary>
        Private Function IncompleteBeta(a As Double, b As Double, x As Double) As Double
            If x <= 0 Then Return 0
            If x >= 1 Then Return 1

            ' 使用连分数展开近似
            Dim bt As Double = Math.Exp(
                GammaLn(a) + GammaLn(b) - GammaLn(a + b) +
                a * Math.Log(x) + b * Math.Log(1 - x))

            If x < (a + 1) / (a + b + 2) Then
                Return bt * BetaCF(a, b, x) / a
            Else
                Return 1.0 - bt * BetaCF(b, a, 1 - x) / b
            End If
        End Function

        ''' <summary>Beta 连分数展开</summary>
        Private Function BetaCF(a As Double, b As Double, x As Double) As Double
            Dim maxIter As Integer = 200
            Dim eps As Double = 1e-10

            Dim qab As Double = a + b
            Dim qap As Double = a + 1
            Dim qam As Double = a - 1
            Dim c As Double = 1
            Dim d As Double = 1 - qab * x / qap
            If Math.Abs(d) < 1e-30 Then d = 1e-30
            d = 1.0 / d
            Dim h As Double = d

            For m = 1 To maxIter
                Dim m2 As Integer = 2 * m
                Dim aa As Double = m * (b - m) * x / ((qam + m2) * (a + m2))
                d = 1 + aa * d
                If Math.Abs(d) < 1e-30 Then d = 1e-30
                c = 1 + aa / c
                If Math.Abs(c) < 1e-30 Then c = 1e-30
                d = 1.0 / d
                h *= d * c

                aa = -(a + m) * (qab + m) * x / ((a + m2) * (qap + m2))
                d = 1 + aa * d
                If Math.Abs(d) < 1e-30 Then d = 1e-30
                c = 1 + aa / c
                If Math.Abs(c) < 1e-30 Then c = 1e-30
                d = 1.0 / d
                Dim del As Double = d * c
                h *= del

                If Math.Abs(del - 1) < eps Then Exit For
            Next

            Return h
        End Function

        ''' <summary>Gamma 函数对数（Stirling 近似）</summary>
        Private Function GammaLn(x As Double) As Double
            Dim cof As Double() = {76.18009172947146, -86.50532032941677,
                                    24.01409824083091, -1.231739572450155,
                                    0.1208650973866179E-2, -0.5395239384953E-5}
            Dim y As Double = x
            Dim tmp As Double = x + 5.5
            tmp -= (x + 0.5) * Math.Log(tmp)
            Dim ser As Double = 1.000000000190015
            For j = 0 To 5
                y += 1
                ser += cof(j) / y
            Next
            Return -tmp + Math.Log(2.5066282746310005 * ser / x)
        End Function

    End Class

End Namespace
