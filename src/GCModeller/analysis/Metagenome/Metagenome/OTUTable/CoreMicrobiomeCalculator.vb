
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

''' <summary>
''' 从一个OTU相对丰度表格中筛选出核心微生物群落
''' </summary>
Public Class CoreMicrobiomeCalculator

    ReadOnly data As OTUTable()
    ReadOnly groupData As SampleGroup()
    ReadOnly totalSamples As Integer

    Sub New(data As IReadOnlyCollection(Of OTUTable), Optional groupData As IReadOnlyCollection(Of SampleGroup) = Nothing)
        Me.data = data.ToArray
        Me.groupData = If(groupData Is Nothing, Nothing, groupData.ToArray)
        Me.totalSamples = Me.data.PropertyNames.Length
    End Sub

    ' ========================================================================
    ' 辅助函数：计算物种在各个分组中的最小普遍性
    ' 如果未提供分组信息，则返回全局普遍性
    ' ========================================================================
    Private Function CalculatePrevalence(samples As Dictionary(Of String, Double), detectionLimit As Double) As Double
        ' 如果没有分组信息，计算全局普遍性
        If groupData.IsNullOrEmpty Then
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

            For Each sName As String In sampleNamesInGroup
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
    Public Function FilterCoreByThreshold(prevalenceThreshold As Double, abundanceThreshold As Double, Optional detectionLimit As Double = 0.00001) As IEnumerable(Of OTUTable)
        If data.IsNullOrEmpty OrElse totalSamples = 0 Then
            Return New List(Of OTUTable)()
        End If

        Dim result As New List(Of OTUTable)()

        For Each otu As OTUTable In data
            ' 1. 计算普遍性 (如果传入 groupData，内部会自动计算最小分组普遍性)
            Dim prevalence As Double = CalculatePrevalence(otu.Properties, detectionLimit)

            ' 2. 计算平均丰度
            Dim meanAbundance As Double = otu.Vector.Average()

            ' 3. 判断是否同时满足阈值
            If prevalence >= prevalenceThreshold AndAlso meanAbundance >= abundanceThreshold Then
                result.Add(otu)
            End If
        Next

        Return From x
               In result
               Order By x.Vector.Average Descending
    End Function

    ' ========================================================================
    ' 方法2：连续得分 + 排序 + 取前N法 (支持多分组)
    ' ========================================================================
    Public Function FilterCoreByScoreAndTopN(topN As Integer, Optional detectionLimit As Double = 0.00001) As IEnumerable(Of OTUTable)
        If data.IsNullOrEmpty OrElse totalSamples = 0 Then
            Return New List(Of OTUTable)()
        End If

        ' 1. 计算原始普遍性(或最小分组普遍性)和平均丰度
        Dim stats = data _
            .Select(Function(otu)
                        Return New With {
                            .OTU = otu,
                            .Prevalence = CalculatePrevalence(otu.Properties, detectionLimit),
                            .MeanAbundance = otu.Vector.Average()
                        }
                    End Function) _
            .ToList()

        ' 2. 获取全局最大最小值
        Dim minPrev As Double = stats.Min(Function(s) s.Prevalence)
        Dim maxPrev As Double = stats.Max(Function(s) s.Prevalence)
        Dim minAbund As Double = stats.Min(Function(s) s.MeanAbundance)
        Dim maxAbund As Double = stats.Max(Function(s) s.MeanAbundance)

        ' 3. 计算核心指数得分
        Dim scoredStats = stats _
            .Select(Function(s)
                        Dim normPrev As Double = If(maxPrev > minPrev, (s.Prevalence - minPrev) / (maxPrev - minPrev), 1.0)
                        Dim normAbund As Double = If(maxAbund > minAbund, (s.MeanAbundance - minAbund) / (maxAbund - minAbund), 1.0)
                        Dim corenessScore As Double = normPrev * normAbund

                        Return New With {
                            .OTU = s.OTU,
                            .Score = corenessScore
                        }
                    End Function)

        ' 4. 排序取前N
        Return From x
               In scoredStats
               Order By x.Score Descending
               Take topN
               Select x.OTU
    End Function

End Class