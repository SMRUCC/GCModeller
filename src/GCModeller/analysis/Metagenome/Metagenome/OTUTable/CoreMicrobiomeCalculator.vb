
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Public Class CoreMicrobiomeCalculator

    ' ========================================================================
    ' 辅助函数：计算物种在各个分组中的最小普遍性
    ' 如果未提供分组信息，则返回全局普遍性
    ' ========================================================================
    Private Function CalculatePrevalence(samples As Dictionary(Of String, Double), totalSamples As Integer, detectionLimit As Double, Optional groupData As List(Of SampleGroup) = Nothing) As Double
        ' 如果没有分组信息，计算全局普遍性
        If groupData Is Nothing OrElse groupData.Count = 0 Then
            Return samples.Values.Where(Function(x) x > detectionLimit).Count() / totalSamples
        End If

        ' 如果有分组信息，计算最小分组普遍性
        Dim groupedSamples = groupData.GroupBy(Function(g) g.sample_info)
        Dim minGroupPrev As Double = 1.0 ' 初始化为最大值1.0

        For Each grp In groupedSamples
            ' 获取当前分组的样本ID集合
            Dim sampleNamesInGroup = grp.Select(Function(g) g.sample_name).ToList()

            ' 统计当前物种在该分组内的检出数
            Dim presentCount As Integer = 0
            For Each sName In sampleNamesInGroup
                ' 使用 TryGetValue 防止某些样本在 OTU 表中缺失引发的异常
                Dim abundance As Double = 0.0
                If samples.TryGetValue(sName, abundance) AndAlso abundance > detectionLimit Then
                    presentCount += 1
                End If
            Next

            ' 计算该分组的普遍性
            Dim groupPrev As Double = presentCount / sampleNamesInGroup.Count

            ' 更新最小分组普遍性
            If groupPrev < minGroupPrev Then
                minGroupPrev = groupPrev
            End If
        Next

        Return minGroupPrev
    End Function


    ' ========================================================================
    ' 方法1：普遍性 + 丰度 阈值法 (支持多分组)
    ' ========================================================================
    Public Function FilterCoreByThreshold(
        data As List(Of OTUTable),
        prevalenceThreshold As Double,
        abundanceThreshold As Double,
        Optional detectionLimit As Double = 0.00001,
        Optional groupData As List(Of SampleGroup) = Nothing ' 新增可选参数
    ) As List(Of OTUTable)

        If data Is Nothing OrElse data.Count = 0 Then Return New List(Of OTUTable)()
        Dim totalSamples As Integer = data.PropertyNames.Length
        If totalSamples = 0 Then Return New List(Of OTUTable)()

        Dim result As New List(Of OTUTable)()

        For Each otu In data
            ' 1. 计算普遍性 (如果传入 groupData，内部会自动计算最小分组普遍性)
            Dim prevalence As Double = CalculatePrevalence(otu.Properties, totalSamples, detectionLimit, groupData)

            ' 2. 计算平均丰度
            Dim meanAbundance As Double = otu.Vector.Average()

            ' 3. 判断是否同时满足阈值
            If prevalence >= prevalenceThreshold AndAlso meanAbundance >= abundanceThreshold Then
                result.Add(otu)
            End If
        Next

        Return result.OrderByDescending(Function(x) x.Vector.Average()).ToList()
    End Function


    ' ========================================================================
    ' 方法2：连续得分 + 排序 + 取前N法 (支持多分组)
    ' ========================================================================
    Public Function FilterCoreByScoreAndTopN(
        data As List(Of OTUTable),
        topN As Integer,
        Optional detectionLimit As Double = 0.00001,
        Optional groupData As List(Of SampleGroup) = Nothing ' 新增可选参数
    ) As IEnumerable(Of OTUTable)

        If data Is Nothing OrElse data.Count = 0 Then Return New List(Of OTUTable)()
        Dim totalSamples As Integer = data.PropertyNames.Length
        If totalSamples = 0 Then Return New List(Of OTUTable)()

        ' 1. 计算原始普遍性(或最小分组普遍性)和平均丰度
        Dim stats = data.Select(Function(otu) New With {
            .OTU = otu,
            .Prevalence = CalculatePrevalence(otu.Properties, totalSamples, detectionLimit, groupData),
            .MeanAbundance = otu.Vector.Average()
        }).ToList()

        ' 2. 获取全局最大最小值
        Dim minPrev As Double = stats.Min(Function(s) s.Prevalence)
        Dim maxPrev As Double = stats.Max(Function(s) s.Prevalence)
        Dim minAbund As Double = stats.Min(Function(s) s.MeanAbundance)
        Dim maxAbund As Double = stats.Max(Function(s) s.MeanAbundance)

        ' 3. 计算核心指数得分
        Dim scoredStats = stats.Select(Function(s)
                                           Dim normPrev As Double = If(maxPrev > minPrev, (s.Prevalence - minPrev) / (maxPrev - minPrev), 1.0)
                                           Dim normAbund As Double = If(maxAbund > minAbund, (s.MeanAbundance - minAbund) / (maxAbund - minAbund), 1.0)
                                           Dim corenessScore As Double = normPrev * normAbund

                                           Return New With {
                .OTU = s.OTU,
                .Score = corenessScore
            }
                                       End Function)

        ' 4. 排序取前N
        Return scoredStats.OrderByDescending(Function(x) x.Score).Take(topN).Select(Function(x) x.OTU).ToList()
    End Function

End Class