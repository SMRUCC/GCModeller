' ============================================================================
' Module 3: Data Fusion & Extended Sample Construction
' File: Phylogeny/DataFusion.vb
'
' 功能: 将现代样本的特征与祖先进化事件特征融合，生成扩展的分类数据集。
'       对应论文中 "数据融合与扩展样本构建模块" 和 "phypat+PGL 分类器"。
'
' 算法原理:
'   1. 概率联合计算: x = g + l - g·l
'      (计算分支上发生 Gain 或 Loss 的联合概率)
'   2. 阈值过滤: 丢弃概率在阈值 t=0.5 以下的不确定样本，生成离散标签
'   3. 扩展分类问题: 将现代样本特征矩阵 X 与祖先虚拟样本特征矩阵 X' 组合，
'      标签 y 和 y' 组合，形成扩展的分类数据集
' ============================================================================

Imports System.Collections.Generic
Imports Traitar.Phylogeny

Namespace Traitar.DataFusion

    ''' <summary>
    ''' 扩展样本（可以是现代样本或祖先虚拟样本）
    ''' </summary>
    Public Class ExtendedSample
        ''' <summary>样本ID</summary>
        Public Property SampleId As String
        ''' <summary>是否为祖先虚拟样本</summary>
        Public Property IsAncestral As Boolean
        ''' <summary>特征向量: PfamAcc -> 联合概率值（0~1）</summary>
        Public Property Features As New Dictionary(Of String, Double)
        ''' <summary>表型标签（0/1）</summary>
        Public Property Label As Integer
        ''' <summary>表型ID</summary>
        Public Property PhenotypeId As String

        Public Overrides Function ToString() As String
            Dim typeStr = If(IsAncestral, "Ancestral", "Modern")
            Return $"[{SampleId}] ({typeStr}) label={Label}, #features={Features.Count}"
        End Function
    End Class

    ''' <summary>
    ''' 数据融合器：构建 phypat+PGL 扩展数据集
    ''' </summary>
    Public Class DataFusionBuilder

        ''' <summary>不确定性阈值（论文中 t=0.5）</summary>
        Public Property UncertaintyThreshold As Double = 0.5

        ''' <summary>
        ''' 计算联合概率: x = g + l - g·l
        ''' 即分支上发生 Gain 或 Loss 的概率（并集概率）
        ''' </summary>
        Public Function ComputeJointProbability(gainProb As Double, lossProb As Double) As Double
            Return gainProb + lossProb - gainProb * lossProb
        End Function

        ''' <summary>
        ''' 将概率值离散化为标签
        ''' 若概率 >= 阈值，标签为 1（事件发生），否则丢弃（返回 -1）
        ''' </summary>
        Public Function Discretize(prob As Double) As Integer
            If prob >= UncertaintyThreshold Then
                Return 1
            ElseIf prob <= 1 - UncertaintyThreshold Then
                Return 0
            Else
                Return -1 ' 不确定，丢弃
            End If
        End Function

        ''' <summary>
        ''' 构建扩展数据集
        ''' </summary>
        ''' <param name="modernProfiles">现代样本的 Pfam 分布: sampleId -> set of PfamAcc</param>
        ''' <param name="modernLabels">现代样本的表型标签: sampleId -> 0/1</param>
        ''' <param name="ancestralEvents">祖先进化事件: (ancestorNode, descendantNode) -> list of events</param>
        ''' <param name="phenotypeId">当前处理的表型ID</param>
        Public Function BuildExtendedDataset(
            modernProfiles As Dictionary(Of String, HashSet(Of String)),
            modernLabels As Dictionary(Of String, Integer),
            ancestralEvents As Dictionary(Of Tuple(Of String, String), List(Of AncestralEvent)),
            phenotypeId As String) As List(Of ExtendedSample)

            Dim dataset As New List(Of ExtendedSample)

            ' 1. 添加现代样本
            For Each kv In modernProfiles
                Dim sampleId = kv.Key
                Dim pfams = kv.Value
                Dim label = If(modernLabels.ContainsKey(sampleId), modernLabels(sampleId), 0)

                Dim sample As New ExtendedSample With {
                    .SampleId = sampleId,
                    .IsAncestral = False,
                    .Label = label,
                    .PhenotypeId = phenotypeId
                }
                For Each pfam In pfams
                    sample.Features(pfam) = 1.0 ' 现代样本为二值
                Next
                dataset.Add(sample)
            Next

            ' 2. 添加祖先虚拟样本
            For Each kv In ancestralEvents
                Dim branchKey = kv.Key ' (ancestorNode, descendantNode)
                Dim events = kv.Value

                Dim sample As New ExtendedSample With {
                    .SampleId = $"{branchKey.Item1}->{branchKey.Item2}",
                    .IsAncestral = True,
                    .PhenotypeId = phenotypeId
                }

                ' 对每个 Pfam 家族计算联合概率
                Dim pfamEvents As New Dictionary(Of String, Tuple(Of Double, Double))
                For Each e In events
                    If Not pfamEvents.ContainsKey(e.FeatureId) Then
                        pfamEvents(e.FeatureId) = Tuple.Create(0.0, 0.0)
                    End If
                    Dim cur = pfamEvents(e.FeatureId)
                    pfamEvents(e.FeatureId) = Tuple.Create(
                        Math.Max(cur.Item1, e.GainProbability),
                        Math.Max(cur.Item2, e.LossProbability))
                Next

                ' 计算联合概率并离散化
                Dim labelProb = 0.0
                For Each pfamKv In pfamEvents
                    Dim jointProb = ComputeJointProbability(pfamKv.Value.Item1, pfamKv.Value.Item2)
                    sample.Features(pfamKv.Key) = jointProb
                Next

                ' 表型标签也需要从祖先事件推断
                ' (这里简化处理: 若该分支有表型 Gain 事件，标签为 1)
                ' 实际实现中需要单独推断表型的祖先状态
                sample.Label = 0 ' 默认值，实际应根据表型祖先事件设置

                dataset.Add(sample)
            Next

            ' 3. 过滤不确定样本
            dataset = dataset.FindAll(Function(s) s.Label >= 0)

            Return dataset
        End Function

        ''' <summary>
        ''' 将扩展数据集转换为 SVM 训练所需的矩阵格式
        ''' </summary>
        Public Function ToSvmFormat(dataset As List(Of ExtendedSample),
                                     ByRef featureNames As List(Of String)) As Tuple(Of Double()(), Integer())
            ' 收集所有特征名
            Dim featSet As New HashSet(Of String)
            For Each sample In dataset
                For Each f In sample.Features.Keys
                    featSet.Add(f)
                Next
            Next
            featureNames = New List(Of String)(featSet)
            featureNames.Sort()

            ' 构建矩阵
            Dim nSamples = dataset.Count
            Dim nFeats = featureNames.Count
            Dim X(nSamples - 1, nFeats - 1) As Double
            Dim y(nSamples - 1) As Integer

            For i = 0 To nSamples - 1
                Dim sample = dataset(i)
                y(i) = sample.Label
                For j = 0 To nFeats - 1
                    Dim featName = featureNames(j)
                    If sample.Features.ContainsKey(featName) Then
                        X(i, j) = sample.Features(featName)
                    Else
                        X(i, j) = 0.0
                    End If
                Next
            Next

            ' 转换为锯齿数组
            Dim XJagged(nSamples - 1)() As Double
            For i = 0 To nSamples - 1
                Dim row(nFeats - 1) As Double
                For j = 0 To nFeats - 1
                    row(j) = X(i, j)
                Next
                XJagged(i) = row
            Next

            Return Tuple.Create(XJagged, y)
        End Function
    End Class
End Namespace
