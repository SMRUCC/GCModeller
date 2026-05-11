''' <summary>
''' 单个工具的评估得分
''' </summary>
Public Class ToolScore
    Public Property ToolName As String

    ' 分类评估指标 (0-1之间，越大越好)
    Public Property Precision As Double ' 准确率：预测存在的物种中，真实存在的比例（抗假阳性）
    Public Property Recall As Double    ' 召回率：真实存在的物种中，被预测出来的比例（抗假阴性）
    Public Property F1Score As Double   ' F1分数：Precision和Recall的调和平均

    ' 丰度评估指标 (越小越好，表示误差/距离越小)
    Public Property L1Distance As Double     ' 绝对误差和
    Public Property BrayCurtis As Double     ' Bray-Curtis 相异度 (0-1之间)

    ' 综合得分 (0-100之间，越大越好)
    Public Property OverallScore As Double

    Public Overrides Function ToString() As String
        Return $"{ToolName,-15} | F1: {F1Score:F4} | Precision: {Precision:F4} | Recall: {Recall:F4} | L1: {L1Distance:F4} | BC: {BrayCurtis:F4} | Overall: {OverallScore:F2}"
    End Function
End Class

' ==========================================
' 2. 比较评估模块
' ==========================================

Public Class MetagenomicsBenchmark

    ''' <summary>
    ''' 执行所有工具的比较并生成排名
    ''' </summary>
    ''' <param name="reference">参考真值数据</param>
    ''' <param name="tool_new">新算法结果</param>
    ''' <param name="current_tools">现有算法字典</param>
    ''' <param name="groups">样本分组信息</param>
    ''' <returns>按综合得分降序排列的得分列表</returns>
    Public Function RunBenchmark(reference As OTUTable(), tool_new As OTUTable(), current_tools As Dictionary(Of String, OTUTable()), groups As Dictionary(Of String, String())) As List(Of ToolScore)

        Dim scores As New List(Of ToolScore)()

        ' 1. 评估新工具
        Dim newToolScore = EvaluateTool("Tool_New", reference, tool_new, groups)
        scores.Add(newToolScore)

        ' 2. 评估现有工具
        For Each kvp In current_tools
            Dim toolScore = EvaluateTool(kvp.Key, reference, kvp.Value, groups)
            scores.Add(toolScore)
        Next

        ' 3. 根据综合得分降序排序 (得分越高越好)
        Dim rankedScores = scores.OrderByDescending(Function(s) s.OverallScore).ToList()

        Return rankedScores
    End Function

    ''' <summary>
    ''' 评估单个工具相对于参考数据的表现
    ''' </summary>
    Private Function EvaluateTool(toolName As String, reference As OTUTable(), tool As OTUTable(), groups As Dictionary(Of String, String())) As ToolScore
        Dim score As New ToolScore() With {.ToolName = toolName}

        ' 将数组转为以 Taxonomy 为键的字典，加速查找
        Dim refDict = reference.ToDictionary(Function(g) g.taxonomy, Function(g) g.Properties)
        Dim toolDict = tool.ToDictionary(Function(g) g.taxonomy, Function(g) g.Properties)

        ' 所有物种的并集
        Dim allTaxa = refDict.Keys.Union(toolDict.Keys).ToHashSet()

        ' 累加器，用于在所有样本上计算总体的指标
        Dim totalTP As Integer = 0, totalFP As Integer = 0, totalFN As Integer = 0
        Dim totalL1 As Double = 0
        Dim totalBC_Numerator As Double = 0
        Dim totalBC_Denominator As Double = 0
        Dim sampleCount As Integer = 0

        ' 遍历分组和样本
        For Each grp In groups
            For Each sampleID In grp.Value
                sampleCount += 1

                ' 计算该样本的指标
                Dim tp As Integer = 0, fp As Integer = 0, fn As Integer = 0
                Dim l1_sample As Double = 0
                Dim bc_num_sample As Double = 0
                Dim bc_den_sample As Double = 0

                For Each tax In allTaxa
                    ' 获取参考丰度和工具预测丰度，如果没有则为 0.0
                    Dim refAbund As Double = If(refDict.ContainsKey(tax) AndAlso refDict(tax).ContainsKey(sampleID), refDict(tax)(sampleID), 0.0)
                    Dim toolAbund As Double = If(toolDict.ContainsKey(tax) AndAlso toolDict(tax).ContainsKey(sampleID), toolDict(tax)(sampleID), 0.0)

                    ' 1. 计算 Precision, Recall (基于是否存在，设定阈值为 > 1e-5 避免浮点误差)
                    Dim isRefPresent As Boolean = refAbund > 0.00001
                    Dim isToolPresent As Boolean = toolAbund > 0.00001

                    If isRefPresent AndAlso isToolPresent Then
                        tp += 1
                    ElseIf isToolPresent AndAlso Not isRefPresent Then
                        fp += 1 ' 假阳性：参考没有，工具预测有
                    ElseIf isRefPresent AndAlso Not isToolPresent Then
                        fn += 1 ' 假阴性：参考有，工具预测没有
                    End If

                    ' 2. 计算 L1 Distance 和 Bray-Curtis
                    Dim diff As Double = Math.Abs(refAbund - toolAbund)
                    l1_sample += diff
                    bc_num_sample += diff
                    bc_den_sample += refAbund + toolAbund
                Next

                totalTP += tp
                totalFP += fp
                totalFN += fn
                totalL1 += l1_sample
                totalBC_Numerator += bc_num_sample
                totalBC_Denominator += bc_den_sample
            Next
        Next

        ' 汇总计算最终指标
        score.Precision = If((totalTP + totalFP) > 0, totalTP / (totalTP + totalFP), 0.0)
        score.Recall = If((totalTP + totalFN) > 0, totalTP / (totalTP + totalFN), 0.0)
        score.F1Score = If((score.Precision + score.Recall) > 0, 2 * (score.Precision * score.Recall) / (score.Precision + score.Recall), 0.0)

        score.L1Distance = totalL1 / sampleCount ' 平均每个样本的 L1
        score.BrayCurtis = If(totalBC_Denominator > 0, totalBC_Numerator / totalBC_Denominator, 0.0) ' BC 可以整体算，也可以平均算，这里用整体样本累加算更符合生态学定义

        ' 综合打分 (0-100分)
        ' 思路：F1Score 越大越好 (0-1)；BrayCurtis 越小越好 (0-1)，转化为相似度 1-BC
        ' 权重设置：F1占 50%，BrayCurtis相似度占 50% (可依据侧重点调整)
        Dim bcSimilarity As Double = 1.0 - score.BrayCurtis
        score.OverallScore = (score.F1Score * 0.5 + bcSimilarity * 0.5) * 100.0

        Return score
    End Function

End Class