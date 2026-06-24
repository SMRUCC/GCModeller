' ============================================================================
' PhylogenyAncestral.vb - 模块2：系统发育与祖先状态重建模块
'
' 论文对应：
'   "整合进化历史信息（phypat+PGL分类器）"
'
' 核心功能：
'   1. 推断祖先基因获得/丢失：利用最大似然法工具GLOOME，基于测序生命树，
'      推断出在进化树的各个分支上，各个蛋白质家族和表型是"获得"还是"丢失"的后验概率
'   2. 联合概率计算：计算在某个进化分支上，蛋白质家族发生获得/丢失事件，
'      且表型同时发生获得/丢失事件的联合概率
'   3. 设定阈值过滤不确定事件：设定阈值t=0.5，只保留高置信度的进化事件
'
' 算法原理：
'   - 最大似然估计：推断祖先状态
'   - 连续时间马尔可夫过程：建模状态转移（拥有/缺失）
'   - Gamma分布采样：模拟获得/丢失速率的方差
'   - 树遍历算法：将生命树(sTOL)映射剪枝到特定表型树
' ============================================================================

Namespace TraitarVB.Modules

    ''' <summary>
    ''' 模块2：系统发育与祖先状态重建模块
    ''' 推断进化树上各分支的蛋白质家族和表型的"获得"与"丢失"事件概率
    ''' </summary>
    Public Class PhylogenyAncestral

        ' 默认参数
        Private Const DEFAULT_GAIN_LOSS_RATE As Double = 0.1
        Private Const DEFAULT_GAMMA_SHAPE As Double = 1.0
        Private Const DEFAULT_GAMMA_CATEGORIES As Integer = 4

        ''' <summary>
        ''' 推断祖先状态（最大似然法）
        ''' 论文：利用最大似然法工具GLOOME，基于测序生命树，
        '''       推断出在进化树的各个分支上，各个蛋白质家族和表型
        '''       是"获得"还是"丢失"的后验概率
        '''
        ''' 算法：使用连续时间马尔可夫过程建模状态转移
        '''       状态空间：{0=缺失, 1=存在}
        '''       转移速率矩阵：
        '''         Q = | -α  α |
        '''             |  β -β |
        '''       其中α=获得速率，β=丢失速率
        ''' </summary>
        ''' <param name="tree">系统发育树根节点</param>
        ''' <param name="leafProfiles">叶节点的特征/表型存在性（节点名 -> PfamID -> 0/1）</param>
        ''' <param name="allFeatures">所有特征ID列表</param>
        ''' <param name="gainRate">获得速率α</param>
        ''' <param name="lossRate">丢失速率β</param>
        Public Sub InferAncestralStates(
            ByVal tree As Models.PhyloTreeNode,
            ByVal leafProfiles As Dictionary(Of String, Dictionary(Of String, Integer)),
            ByVal allFeatures As List(Of String),
            Optional ByVal gainRate As Double = DEFAULT_GAIN_LOSS_RATE,
            Optional ByVal lossRate As Double = DEFAULT_GAIN_LOSS_RATE)

            Console.WriteLine("[模块2] 推断祖先状态（最大似然法）")
            Console.WriteLine("       获得速率 α = {0}", gainRate)
            Console.WriteLine("       丢失速率 β = {0}", lossRate)
            Console.WriteLine("       特征数量 = {0}", allFeatures.Count)

            ' 对每个特征独立推断祖先状态
            For Each featureId As String In allFeatures
                InferSingleFeatureAncestral(tree, leafProfiles, featureId, gainRate, lossRate)
            Next

            ' 计算每个分支的获得/丢失概率
            ComputeGainLossProbabilities(tree, allFeatures)
        End Sub

        ''' <summary>
        ''' 对单个特征推断祖先状态
        ''' 使用Felsenstein的pruning算法（简化版）
        ''' </summary>
        Private Sub InferSingleFeatureAncestral(
            ByVal node As Models.PhyloTreeNode,
            ByVal leafProfiles As Dictionary(Of String, Dictionary(Of String, Integer)),
            ByVal featureId As String,
            ByVal gainRate As Double,
            ByVal lossRate As Double)

            ' 1. 自底向上：计算每个节点的部分似然
            ComputePartialLikelihoods(node, leafProfiles, featureId, gainRate, lossRate)

            ' 2. 自顶向下：计算每个节点的后验概率
            Dim rootPrior As Double() = {0.5, 0.5}  ' 均匀先验
            ComputePosteriorProbabilities(node, featureId, rootPrior, gainRate, lossRate)
        End Sub

        ''' <summary>
        ''' 计算节点的部分似然（pruning算法）
        ''' L_node(s) = Π_children [ Σ_s' P(s'|s,t) × L_child(s') ]
        ''' 其中 P(s'|s,t) 是状态转移概率，t是分支长度
        ''' </summary>
        Private Sub ComputePartialLikelihoods(
            ByVal node As Models.PhyloTreeNode,
            ByVal leafProfiles As Dictionary(Of String, Dictionary(Of String, Integer)),
            ByVal featureId As String,
            ByVal gainRate As Double,
            ByVal lossRate As Double)

            ' 递归处理子节点
            For Each child As Models.PhyloTreeNode In node.Children
                ComputePartialLikelihoods(child, leafProfiles, featureId, gainRate, lossRate)
            Next

            ' 计算当前节点的部分似然
            Dim partialLikelihood As Double() = New Double(1) {}

            If node.IsLeaf Then
                ' 叶节点：根据观测数据设置似然
                Dim observed As Integer = 0
                If leafProfiles.ContainsKey(node.Name) AndAlso
                   leafProfiles(node.Name).ContainsKey(featureId) Then
                    observed = leafProfiles(node.Name)(featureId)
                End If
                If observed = 1 Then
                    partialLikelihood(0) = 0.0  ' P(观测=1 | 状态=0) = 0
                    partialLikelihood(1) = 1.0  ' P(观测=1 | 状态=1) = 1
                Else
                    partialLikelihood(0) = 1.0  ' P(观测=0 | 状态=0) = 1
                    partialLikelihood(1) = 0.0  ' P(观测=0 | 状态=1) = 0
                End If
            Else
                ' 内部节点：聚合子节点的似然
                partialLikelihood(0) = 1.0
                partialLikelihood(1) = 1.0

                For Each child As Models.PhyloTreeNode In node.Children
                    Dim t As Double = If(child.BranchLength > 0, child.BranchLength, 0.01)
                    Dim transProb As Double(,) = ComputeTransitionMatrix(t, gainRate, lossRate)

                    ' 对每个父状态s，聚合子节点的似然
                    Dim childLikelihood0 As Double = 0.0
                    Dim childLikelihood1 As Double = 0.0

                    ' 子节点状态为0的概率
                    If child.PfamPresenceProb.ContainsKey(featureId) Then
                        Dim childPost0 As Double = 1.0 - child.PfamPresenceProb(featureId)
                        Dim childPost1 As Double = child.PfamPresenceProb(featureId)
                        childLikelihood0 = transProb(0, 0) * childPost0 + transProb(0, 1) * childPost1
                        childLikelihood1 = transProb(1, 0) * childPost0 + transProb(1, 1) * childPost1
                    Else
                        childLikelihood0 = 0.5
                        childLikelihood1 = 0.5
                    End If

                    partialLikelihood(0) *= childLikelihood0
                    partialLikelihood(1) *= childLikelihood1
                Next
            End If

            ' 归一化并存储
            Dim sum As Double = partialLikelihood(0) + partialLikelihood(1)
            If sum > 0 Then
                node.PfamPresenceProb(featureId) = partialLikelihood(1) / sum
            Else
                node.PfamPresenceProb(featureId) = 0.5
            End If
        End Sub

        ''' <summary>
        ''' 计算状态转移概率矩阵
        ''' 连续时间马尔可夫过程
        '''
        ''' Q = | -α   α  |
        '''     |  β  -β  |
        '''
        ''' P(t) = exp(Q*t)
        '''
        ''' 对于二状态模型，解析解为：
        ''' P(0→0) = β/(α+β) + α/(α+β)*exp(-(α+β)*t)
        ''' P(0→1) = α/(α+β) - α/(α+β)*exp(-(α+β)*t)
        ''' P(1→0) = β/(α+β) - β/(α+β)*exp(-(α+β)*t)
        ''' P(1→1) = α/(α+β) + β/(α+β)*exp(-(α+β)*t)
        ''' </summary>
        Public Function ComputeTransitionMatrix(ByVal t As Double,
                                                ByVal gainRate As Double,
                                                ByVal lossRate As Double) As Double(,)
            Dim alpha As Double = gainRate    ' 获得速率
            Dim beta As Double = lossRate     ' 丢失速率
            Dim total As Double = alpha + beta

            Dim P As Double(,) = New Double(1, 1) {}

            If total = 0 Then
                ' 无转移
                P(0, 0) = 1.0 : P(0, 1) = 0.0
                P(1, 0) = 0.0 : P(1, 1) = 1.0
                Return P
            End If

            Dim expTerm As Double = Math.Exp(-total * t)
            Dim pGain As Double = alpha / total
            Dim pLoss As Double = beta / total

            P(0, 0) = pLoss + pGain * expTerm
            P(0, 1) = pGain - pGain * expTerm
            P(1, 0) = pLoss - pLoss * expTerm
            P(1, 1) = pGain + pLoss * expTerm

            Return P
        End Function

        ''' <summary>
        ''' 计算后验概率（自顶向下）
        ''' </summary>
        Private Sub ComputePosteriorProbabilities(
            ByVal node As Models.PhyloTreeNode,
            ByVal featureId As String,
            ByVal parentPrior As Double(),
            ByVal gainRate As Double,
            ByVal lossRate As Double)

            ' 当前节点的后验概率
            Dim posterior As Double() = New Double(1) {}
            If node.PfamPresenceProb.ContainsKey(featureId) Then
                Dim likelihood1 As Double = node.PfamPresenceProb(featureId)
                Dim likelihood0 As Double = 1.0 - likelihood1
                posterior(0) = parentPrior(0) * likelihood0
                posterior(1) = parentPrior(1) * likelihood1
            Else
                posterior(0) = parentPrior(0)
                posterior(1) = parentPrior(1)
            End If

            Dim sum As Double = posterior(0) + posterior(1)
            If sum > 0 Then
                posterior(0) /= sum
                posterior(1) /= sum
            End If

            node.PfamPresenceProb(featureId) = posterior(1)

            ' 递归处理子节点
            For Each child As Models.PhyloTreeNode In node.Children
                Dim t As Double = If(child.BranchLength > 0, child.BranchLength, 0.01)
                Dim transProb As Double(,) = ComputeTransitionMatrix(t, gainRate, lossRate)

                Dim childPrior As Double() = New Double(1) {}
                childPrior(0) = posterior(0) * transProb(0, 0) + posterior(1) * transProb(1, 0)
                childPrior(1) = posterior(0) * transProb(0, 1) + posterior(1) * transProb(1, 1)

                ComputePosteriorProbabilities(child, featureId, childPrior, gainRate, lossRate)
            Next
        End Sub

        ''' <summary>
        ''' 计算每个分支的获得/丢失概率
        ''' Gain(node) = P(child=1 | parent=0)
        ''' Loss(node) = P(child=0 | parent=1)
        ''' </summary>
        Public Sub ComputeGainLossProbabilities(ByVal tree As Models.PhyloTreeNode,
                                                ByVal allFeatures As List(Of String))
            For Each featureId As String In allFeatures
                ComputeGainLossForFeature(tree, featureId)
            Next
        End Sub

        Private Sub ComputeGainLossForFeature(ByVal node As Models.PhyloTreeNode,
                                              ByVal featureId As String)
            For Each child As Models.PhyloTreeNode In node.Children
                Dim parentProb As Double = 0.5
                Dim childProb As Double = 0.5

                If node.PfamPresenceProb.ContainsKey(featureId) Then
                    parentProb = node.PfamPresenceProb(featureId)
                End If
                If child.PfamPresenceProb.ContainsKey(featureId) Then
                    childProb = child.PfamPresenceProb(featureId)
                End If

                ' Gain: parent=0 → child=1
                child.PfamGainProb(featureId) = (1 - parentProb) * childProb
                ' Loss: parent=1 → child=0
                child.PfamLossProb(featureId) = parentProb * (1 - childProb)

                ComputeGainLossForFeature(child, featureId)
            Next
        End Sub

        ''' <summary>
        ''' Gamma分布采样
        ''' 论文：Gamma分布采样：模拟获得/丢失速率的方差
        ''' 用于建模位点间速率的异质性
        ''' </summary>
        Public Function GammaSample(ByVal shape As Double, ByVal scale As Double,
                                    Optional ByVal numCategories As Integer = 4) As Double()
            ' 离散Gamma近似：将连续Gamma分布分为numCategories个类别
            ' 每个类别取该区间的均值
            Dim rates As Double() = New Double(numCategories - 1) {}

            For i As Integer = 0 To numCategories - 1
                ' 计算每个类别的边界
                Dim lower As Double = CDbl(i) / numCategories
                Dim upper As Double = CDbl(i + 1) / numCategories

                ' 使用Gamma分布的逆CDF近似（简化版）
                ' 实际应用中应使用数值方法计算
                Dim midQuantile As Double = (lower + upper) / 2.0
                rates(i) = GammaInverseCDF(midQuantile, shape, scale)
            Next

            ' 归一化使均值为1
            Dim mean As Double = 0.0
            For Each r As Double In rates
                mean += r
            Next
            mean /= numCategories

            If mean > 0 Then
                For i As Integer = 0 To numCategories - 1
                    rates(i) /= mean
                Next
            End If

            Return rates
        End Function

        ''' <summary>
        ''' Gamma分布的逆CDF近似（使用Wilson-Hilferty变换）
        ''' </summary>
        Private Function GammaInverseCDF(ByVal p As Double, ByVal shape As Double,
                                         ByVal scale As Double) As Double
            ' Wilson-Hilferty变换：近似Gamma分布的分位数
            ' z = ( (shape * (1 - 1/(9*shape)) + z_p * sqrt(1/(9*shape)) )^3 ) * scale
            ' 其中z_p是标准正态分布的p分位数

            Dim zP As Double = NormalInverseCDF(p)
            Dim k As Double = shape
            Dim term As Double = 1.0 - 1.0 / (9.0 * k)
            Dim sqrtTerm As Double = Math.Sqrt(1.0 / (9.0 * k))

            Dim x As Double = term + zP * sqrtTerm
            If x < 0 Then x = 0.001

            Return Math.Pow(x, 3) * k * scale
        End Function

        ''' <summary>
        ''' 标准正态分布的逆CDF（使用Beasley-Springer-Moro算法）
        ''' </summary>
        Public Function NormalInverseCDF(ByVal p As Double) As Double
            ' 使用Acklam's算法近似标准正态分布的逆CDF
            Dim a As Double() = {-3.969683028665376e+01, 2.209460984245205e+02,
                                  -2.759285104469687e+02, 1.38357751867269e+02,
                                  -3.066479806614716e+01, 2.506628277459239e+00}
            Dim b As Double() = {-5.447609879822406e+01, 1.615858368580409e+02,
                                  -1.556989798598866e+02, 6.680131188771972e+01,
                                  -1.328068155288572e+01}
            Dim c As Double() = {-7.784894002430293e-03, -3.223964580411365e-01,
                                  -2.400758277161838e+00, -2.549732539343734e+00,
                                  4.374664141464968e+00, 2.938163982698783e+00}
            Dim d As Double() = {7.784695709041462e-03, 3.224671290700398e-01,
                                  2.445134137142996e+00, 3.754408661907416e+00}

            Dim pLow As Double = 0.02425
            Dim pHigh As Double = 1 - pLow
            Dim q As Double, r As Double, x As Double

            If p < pLow Then
                q = Math.Sqrt(-2 * Math.Log(p))
                x = (((((c(0) * q + c(1)) * q + c(2)) * q + c(3)) * q + c(4)) * q + c(5)) /
                    ((((d(0) * q + d(1)) * q + d(2)) * q + d(3)) * q + 1)
            ElseIf p <= pHigh Then
                q = p - 0.5
                r = q * q
                x = (((((a(0) * r + a(1)) * r + a(2)) * r + a(3)) * r + a(4)) * r + a(5)) * q /
                    (((((b(0) * r + b(1)) * r + b(2)) * r + b(3)) * r + b(4)) * r + 1)
            Else
                q = Math.Sqrt(-2 * Math.Log(1 - p))
                x = -(((((c(0) * q + c(1)) * q + c(2)) * q + c(3)) * q + c(4)) * q + c(5)) /
                     ((((d(0) * q + d(1)) * q + d(2)) * q + d(3)) * q + 1)
            End If

            Return x
        End Function

        ''' <summary>
        ''' 树剪枝算法
        ''' 论文：树遍历算法：将生命树(sTOL)映射剪枝到特定表型树
        ''' 将完整生命树剪枝为只包含有表型注释的物种的子树
        ''' </summary>
        Public Function PruneTree(ByVal tree As Models.PhyloTreeNode,
                                  ByVal speciesWithPhenotype As HashSet(Of String)) As Models.PhyloTreeNode
            Return PruneTreeRecursive(tree, speciesWithPhenotype)
        End Function

        Private Function PruneTreeRecursive(ByVal node As Models.PhyloTreeNode,
                                            ByVal speciesWithPhenotype As HashSet(Of String)) As Models.PhyloTreeNode
            If node.IsLeaf Then
                ' 叶节点：只保留有表型注释的物种
                If speciesWithPhenotype.Contains(node.Name) Then
                    Return CType(node.Clone(), Models.PhyloTreeNode)
                Else
                    Return Nothing
                End If
            Else
                ' 内部节点：递归剪枝子节点
                Dim prunedChildren As New List(Of Models.PhyloTreeNode)()
                For Each child As Models.PhyloTreeNode In node.Children
                    Dim pruned As Models.PhyloTreeNode = PruneTreeRecursive(child, speciesWithPhenotype)
                    If pruned IsNot Nothing Then
                        prunedChildren.Add(pruned)
                    End If
                Next

                If prunedChildren.Count = 0 Then
                    Return Nothing
                ElseIf prunedChildren.Count = 1 Then
                    ' 只有一个子节点保留：返回该子节点（合并分支长度）
                    Dim onlyChild As Models.PhyloTreeNode = prunedChildren(0)
                    onlyChild.BranchLength += node.BranchLength
                    Return onlyChild
                Else
                    ' 多个子节点保留：创建新的内部节点
                    Dim newNode As New Models.PhyloTreeNode()
                    newNode.Name = node.Name
                    newNode.BranchLength = node.BranchLength
                    For Each c As Models.PhyloTreeNode In prunedChildren
                        c.Parent = newNode
                        newNode.Children.Add(c)
                    Next
                    Return newNode
                End If
            End If
        End Function

    End Class

End Namespace

Namespace TraitarVB.Models
    ''' <summary>
    ''' PhyloTreeNode 的扩展方法：Clone
    ''' </summary>
    Public Module PhyloTreeNodeExtensions
        <System.Runtime.CompilerServices.Extension()>
        Public Function Clone(ByVal node As PhyloTreeNode) As PhyloTreeNode
            Dim copy As New PhyloTreeNode()
            copy.Name = node.Name
            copy.BranchLength = node.BranchLength
            For Each child As PhyloTreeNode In node.Children
                Dim childCopy As PhyloTreeNode = child.Clone()
                childCopy.Parent = copy
                copy.Children.Add(childCopy)
            Next
            Return copy
        End Function
    End Module
End Namespace
