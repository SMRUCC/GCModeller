' ============================================================================
' DataFusion.vb - 模块3：数据融合与扩展样本构建模块
'
' 论文对应：
'   "整合进化历史信息（phypat+PGL分类器）"
'
' 核心功能：
'   1. 概率联合计算：x = g + l - g·l（计算分支上发生Gain或Loss的联合概率）
'   2. 阈值过滤算法：丢弃概率在阈值t=0.5以下的不确定样本，生成离散标签
'   3. 构建包含现代样本和祖先虚拟样本的联合分类问题
'
' 算法原理：
'   - 概率联合计算：x = g + l - g·l
'   - 阈值过滤算法：丢弃概率在阈值t=0.5以下的不确定样本
' ============================================================================

Imports System.Runtime.InteropServices

Namespace metaTraits.Traitar.Modules

    ''' <summary>
    ''' 模块3：数据融合与扩展样本构建模块
    ''' 将现代样本的特征与祖先进化事件特征融合，生成扩展的分类数据集
    ''' </summary>
    Public Class DataFusion

        ' 默认阈值
        Public Const DEFAULT_THRESHOLD As Double = 0.5

        ''' <summary>
        ''' 计算联合概率
        ''' 论文：x = g + l - g·l
        ''' 计算分支上发生Gain或Loss的联合概率
        '''
        ''' 数学推导：
        '''   P(Gain ∪ Loss) = P(Gain) + P(Loss) - P(Gain ∩ Loss)
        '''   由于Gain和Loss互斥（同一分支不可能同时获得和丢失同一特征），
        '''   P(Gain ∩ Loss) ≈ P(Gain) × P(Loss)（近似独立）
        '''   因此：x = g + l - g·l
        ''' </summary>
        ''' <param name="gainProb">获得概率g</param>
        ''' <param name="lossProb">丢失概率l</param>
        ''' <returns>联合概率</returns>
        Public Function ComputeJointProbability(gainProb As Double,
                                                lossProb As Double) As Double
            Dim g As Double = gainProb
            Dim l As Double = lossProb
            Return g + l - g * l
        End Function

        ''' <summary>
        ''' 阈值过滤：将概率转换为离散标签
        ''' 论文：设定阈值t=0.5，只保留高置信度的进化事件。
        '''       低于该概率的不确定样本会被丢弃
        ''' </summary>
        ''' <param name="prob">概率值</param>
        ''' <param name="threshold">阈值（默认0.5）</param>
        ''' <returns>1=事件发生，0=事件未发生，-1=不确定（需丢弃）</returns>
        Public Function ThresholdFilter(prob As Double,
                                        Optional threshold As Double = DEFAULT_THRESHOLD) As Integer
            If prob >= threshold Then
                Return 1  ' 事件发生
            ElseIf prob <= (1 - threshold) Then
                Return 0  ' 事件未发生
            Else
                Return -1 ' 不确定，丢弃
            End If
        End Function

        ''' <summary>
        ''' 构建扩展数据集
        ''' 论文：将原来基于现代样本蛋白质家族分布的二元分类问题，
        '''       扩展为一个包含"祖先蛋白质家族获得/丢失"特征和
        '''       "表型获得/丢失"标签的联合二元分类问题
        '''
        ''' 扩展数据集包含：
        '''   1. 现代样本：特征=phyletic profile，标签=表型有无
        '''   2. 祖先虚拟样本：特征=Pfam Gain/Loss事件，标签=表型Gain/Loss事件
        ''' </summary>
        ''' <param name="modernFeatures">现代样本特征矩阵</param>
        ''' <param name="modernLabels">现代样本标签</param>
        ''' <param name="ancestralNodes">祖先节点列表</param>
        ''' <param name="allPfamIds">所有Pfam ID</param>
        ''' <param name="phenotypeId">表型ID</param>
        ''' <param name="threshold">阈值</param>
        ''' <param name="extendedFeatures">输出的扩展特征矩阵</param>
        ''' <param name="extendedLabels">输出的扩展标签</param>
        Public Sub BuildExtendedDataset(
            modernFeatures As Integer(,),
            modernLabels As Integer(),
            ancestralNodes As List(Of Models.PhyloTreeNode),
            allPfamIds As List(Of String),
            phenotypeId As String,
            threshold As Double,
            <Out()> ByRef extendedFeatures As Integer(,),
            <Out()> ByRef extendedLabels As Integer())

            Dim nModern As Integer = modernLabels.Length
            Dim nFeatures As Integer = allPfamIds.Count

            ' 收集祖先虚拟样本
            Dim ancestralFeatureList As New List(Of Integer())()
            Dim ancestralLabelList As New List(Of Integer)()

            For Each node As Models.PhyloTreeNode In ancestralNodes
                ' 构建祖先样本特征
                Dim features As Integer() = New Integer(nFeatures - 1) {}
                Dim skipSample As Boolean = False

                For j As Integer = 0 To nFeatures - 1
                    Dim pfamId As String = allPfamIds(j)
                    Dim gainProb As Double = 0.0
                    Dim lossProb As Double = 0.0

                    If node.PfamGainProb.ContainsKey(pfamId) Then
                        gainProb = node.PfamGainProb(pfamId)
                    End If
                    If node.PfamLossProb.ContainsKey(pfamId) Then
                        lossProb = node.PfamLossProb(pfamId)
                    End If

                    ' 联合概率
                    Dim jointProb As Double = ComputeJointProbability(gainProb, lossProb)
                    Dim label As Integer = ThresholdFilter(jointProb, threshold)

                    If label = -1 Then
                        ' 不确定，跳过该样本
                        skipSample = True
                        Exit For
                    End If
                    features(j) = label
                Next

                If skipSample Then Continue For

                ' 构建祖先样本标签（表型Gain/Loss）
                Dim phGainProb As Double = 0.0
                Dim phLossProb As Double = 0.0
                If node.PhenotypeGainProb.ContainsKey(phenotypeId) Then
                    phGainProb = node.PhenotypeGainProb(phenotypeId)
                End If
                If node.PhenotypeLossProb.ContainsKey(phenotypeId) Then
                    phLossProb = node.PhenotypeLossProb(phenotypeId)
                End If

                Dim phJointProb As Double = ComputeJointProbability(phGainProb, phLossProb)
                Dim phLabel As Integer = ThresholdFilter(phJointProb, threshold)

                If phLabel = -1 Then Continue For

                ancestralFeatureList.Add(features)
                ancestralLabelList.Add(phLabel)
            Next

            ' 合并现代样本和祖先样本
            Dim nAncestral As Integer = ancestralFeatureList.Count
            Dim nTotal As Integer = nModern + nAncestral

            extendedFeatures = New Integer(nTotal - 1, nFeatures - 1) {}
            extendedLabels = New Integer(nTotal - 1) {}

            ' 现代样本
            For i As Integer = 0 To nModern - 1
                For j As Integer = 0 To nFeatures - 1
                    extendedFeatures(i, j) = modernFeatures(i, j)
                Next
                extendedLabels(i) = modernLabels(i)
            Next

            ' 祖先样本
            For i As Integer = 0 To nAncestral - 1
                For j As Integer = 0 To nFeatures - 1
                    extendedFeatures(nModern + i, j) = ancestralFeatureList(i)(j)
                Next
                extendedLabels(nModern + i) = ancestralLabelList(i)
            Next

            Console.WriteLine("[模块3] 扩展数据集构建完成:")
            Console.WriteLine("       现代样本: {0}", nModern)
            Console.WriteLine("       祖先虚拟样本: {0}", nAncestral)
            Console.WriteLine("       总样本数: {0}", nTotal)
            Console.WriteLine("       特征数: {0}", nFeatures)
            Console.WriteLine("       阈值: {0}", threshold)
        End Sub

        ''' <summary>
        ''' 将标签从{0,1}转换为{-1,+1}（SVM标准格式）
        ''' </summary>
        Public Function ConvertLabelsToSVMFormat(labels As Integer()) As Integer()
            Dim result As Integer() = New Integer(labels.Length - 1) {}
            For i As Integer = 0 To labels.Length - 1
                result(i) = If(labels(i) = 1, 1, -1)
            Next
            Return result
        End Function

        ''' <summary>
        ''' 将标签从{-1,+1}转换为{0,1}
        ''' </summary>
        Public Function ConvertLabelsFromSVMFormat(labels As Integer()) As Integer()
            Dim result As Integer() = New Integer(labels.Length - 1) {}
            For i As Integer = 0 To labels.Length - 1
                result(i) = If(labels(i) = 1, 1, 0)
            Next
            Return result
        End Function

    End Class

End Namespace
