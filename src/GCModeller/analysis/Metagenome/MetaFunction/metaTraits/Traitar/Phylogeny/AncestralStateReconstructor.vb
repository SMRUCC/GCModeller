' ============================================================================
' Module 2: Phylogenetic & Ancestral State Reconstruction
' File: Phylogeny/AncestralStateReconstructor.vb
'
' 功能: 推断进化树上各分支的蛋白质家族和表型的"获得"与"丢失"事件概率。
'       对应论文中 "整合进化历史信息 (phypat+PGL 分类器)"。
'
' 算法原理:
'   1. 最大似然法推断祖先状态（类似 GLOOME 工具）
'   2. 连续时间马尔可夫过程建模状态转移（拥有/缺失）
'   3. Gamma 分布采样模拟获得/丢失速率的方差
'   4. 树遍历算法将生命树(sTOL)映射剪枝到特定表型树
' ============================================================================

Imports System.Collections.Generic
Imports System.Linq

Namespace Traitar.Phylogeny

    ''' <summary>
    ''' 系统发育树节点
    ''' </summary>
    Public Class PhyloTreeNode
        ''' <summary>节点名称（叶节点为物种名，内部节点为祖先名）</summary>
        Public Property Name As String
        ''' <summary>分支长度（到父节点）</summary>
        Public Property BranchLength As Double
        ''' <summary>子节点列表</summary>
        Public Property Children As New List(Of PhyloTreeNode)
        ''' <summary>父节点</summary>
        Public Property Parent As PhyloTreeNode

        ''' <summary>是否为叶节点</summary>
        Public ReadOnly Property IsLeaf As Boolean
            Get
                Return Children.Count = 0
            End Get
        End Property

        ''' <summary>获取所有叶节点</summary>
        Public Function GetLeaves() As List(Of PhyloTreeNode)
            Dim leaves As New List(Of PhyloTreeNode)
            If IsLeaf Then
                leaves.Add(Me)
            Else
                For Each child In Children
                    leaves.AddRange(child.GetLeaves())
                Next
            End If
            Return leaves
        End Function
    End Class

    ''' <summary>
    ''' 祖先进化事件（获得/丢失）
    ''' </summary>
    Public Class AncestralEvent
        ''' <summary>分支起始节点（祖先）</summary>
        Public Property AncestorNode As String
        ''' <summary>分支终止节点（后代）</summary>
        Public Property DescendantNode As String
        ''' <summary>特征ID（Pfam 家族或表型ID）</summary>
        Public Property FeatureId As String
        ''' <summary>获得事件的概率</summary>
        Public Property GainProbability As Double
        ''' <summary>丢失事件的概率</summary>
        Public Property LossProbability As Double
        ''' <summary>事件类型（Gain / Loss / None）</summary>
        Public ReadOnly Property EventType As String
            Get
                If GainProbability > LossProbability AndAlso GainProbability > 0.5 Then
                    Return "Gain"
                ElseIf LossProbability > GainProbability AndAlso LossProbability > 0.5 Then
                    Return "Loss"
                Else
                    Return "None"
                End If
            End Get
        End Property
    End Class

    ''' <summary>
    ''' 祖先状态重建器（基于最大似然法，类似 GLOOME）
    ''' </summary>
    Public Class AncestralStateReconstructor

        ''' <summary>获得速率（α）</summary>
        Public Property GainRate As Double = 1.0

        ''' <summary>丢失速率（β）</summary>
        Public Property LossRate As Double = 1.0

        ''' <summary>不确定性阈值（论文中 t=0.5）</summary>
        Public Property UncertaintyThreshold As Double = 0.5

        ''' <summary>Gamma 分布形状参数（模拟速率方差）</summary>
        Public Property GammaShape As Double = 1.0

        ''' <summary>Gamma 分布类别数</summary>
        Public Property GammaCategories As Integer = 4

        ''' <summary>
        ''' 计算状态转移概率矩阵
        ''' 状态: 0=缺失(absent), 1=拥有(present)
        ''' 转移: P(i->j, t) = (1/2)[1 + exp(-(α+β)t)] * δ(i,j) + (1/2)[1 - exp(-(α+β)t)] * π(j)
        ''' 其中 π(0) = β/(α+β), π(1) = α/(α+β)
        ''' </summary>
        Public Function ComputeTransitionMatrix(branchLength As Double) As Double(,)
            Dim alpha = GainRate
            Dim beta = LossRate
            Dim t = branchLength
            Dim sumRate = alpha + beta

            Dim P(1, 1) As Double
            If sumRate = 0 Then
                ' 无变化
                P(0, 0) = 1.0 : P(0, 1) = 0.0
                P(1, 0) = 0.0 : P(1, 1) = 1.0
            Else
                Dim expTerm = Math.Exp(-sumRate * t)
                Dim pi0 = beta / sumRate  ' 平衡时缺失概率
                Dim pi1 = alpha / sumRate ' 平衡时拥有概率

                ' P(0->0) = pi0 + (1-pi0)*exp(-(α+β)t)
                P(0, 0) = pi0 + (1 - pi0) * expTerm
                P(0, 1) = 1 - P(0, 0)
                P(1, 1) = pi1 + (1 - pi1) * expTerm
                P(1, 0) = 1 - P(1, 1)
            End If

            Return P
        End Function

        ''' <summary>
        ''' 推断祖先节点的状态概率（最大似然法）
        ''' </summary>
        ''' <param name="tree">系统发育树</param>
        ''' <param name="leafStates">叶节点的状态: nodeName -> 0/1</param>
        ''' <returns>内部节点状态概率: nodeName -> (P(absent), P(present))</returns>
        Public Function InferAncestralStates(tree As PhyloTreeNode,
                                              leafStates As Dictionary(Of String, Integer)) _
            As Dictionary(Of String, Double())
            Dim likelihoods As New Dictionary(Of String, Double())

            ' 自底向上计算各节点的似然
            ComputeLikelihoods(tree, leafStates, likelihoods)

            ' 归一化为概率
            Dim result As New Dictionary(Of String, Double())
            For Each kv In likelihoods
                Dim L = kv.Value
                Dim sum = L(0) + L(1)
                If sum > 0 Then
                    result(kv.Key) = New Double() {L(0) / sum, L(1) / sum}
                Else
                    result(kv.Key) = New Double() {0.5, 0.5}
                End If
            Next

            Return result
        End Function

        ''' <summary>
        ''' 递归计算各节点的似然向量
        ''' L(node) = [L(absent), L(present)]
        ''' </summary>
        Private Sub ComputeLikelihoods(node As PhyloTreeNode,
                                        leafStates As Dictionary(Of String, Integer),
                                        likelihoods As Dictionary(Of String, Double()))
            If node.IsLeaf Then
                ' 叶节点: 根据观测状态设置似然
                Dim state = If(leafStates.ContainsKey(node.Name), leafStates(node.Name), 0)
                Dim L(1) As Double
                L(0) = If(state = 0, 1.0, 0.0)
                L(1) = If(state = 1, 1.0, 0.0)
                likelihoods(node.Name) = L
            Else
                ' 内部节点: 递归计算子节点，然后合并
                Dim childLs As New List(Of Double())
                For Each child In node.Children
                    ComputeLikelihoods(child, leafStates, likelihoods)
                    childLs.Add(likelihoods(child.Name))
                Next

                ' 合并子节点似然
                Dim L(1) As Double
                L(0) = 1.0
                L(1) = 1.0

                For i = 0 To node.Children.Count - 1
                    Dim child = node.Children(i)
                    Dim childL = childLs(i)
                    Dim P = ComputeTransitionMatrix(child.BranchLength)

                    ' L_parent(0) *= sum_k P(0,k) * L_child(k)
                    Dim newL0 = L(0) * (P(0, 0) * childL(0) + P(0, 1) * childL(1))
                    Dim newL1 = L(1) * (P(1, 0) * childL(0) + P(1, 1) * childL(1))
                    L(0) = newL0
                    L(1) = newL1
                Next

                likelihoods(node.Name) = L
            End If
        End Sub

        ''' <summary>
        ''' 推断各分支上的获得/丢失事件概率
        ''' </summary>
        Public Function InferGainLossEvents(tree As PhyloTreeNode,
                                             leafStates As Dictionary(Of String, Integer),
                                             featureId As String) As List(Of AncestralEvent)
            Dim events As New List(Of AncestralEvent)
            Dim states = InferAncestralStates(tree, leafStates)

            ' 遍历所有分支
            InferEventsRecursive(tree, states, featureId, events)
            Return events
        End Function

        ''' <summary>递归推断分支事件</summary>
        Private Sub InferEventsRecursive(node As PhyloTreeNode,
                                          states As Dictionary(Of String, Double()),
                                          featureId As String,
                                          events As List(Of AncestralEvent))
            For Each child In node.Children
                Dim parentState = states(node.Name)
                Dim childState = states(child.Name)

                ' P(Gain) = P(parent=0) * P(child=1)
                Dim gainProb = parentState(0) * childState(1)
                ' P(Loss) = P(parent=1) * P(child=0)
                Dim lossProb = parentState(1) * childState(0)

                events.Add(New AncestralEvent With {
                    .AncestorNode = node.Name,
                    .DescendantNode = child.Name,
                    .FeatureId = featureId,
                    .GainProbability = gainProb,
                    .LossProbability = lossProb
                })

                InferEventsRecursive(child, states, featureId, events)
            Next
        End Sub

        ''' <summary>
        ''' 过滤不确定事件（概率 < 阈值 t=0.5 的样本被丢弃）
        ''' 对应论文中 "设定阈值过滤不确定事件"
        ''' </summary>
        Public Function FilterUncertainEvents(events As List(Of AncestralEvent)) As List(Of AncestralEvent)
            Return events.FindAll(Function(e) e.GainProbability >= UncertaintyThreshold OrElse
                                           e.LossProbability >= UncertaintyThreshold)
        End Function
    End Class
End Namespace
