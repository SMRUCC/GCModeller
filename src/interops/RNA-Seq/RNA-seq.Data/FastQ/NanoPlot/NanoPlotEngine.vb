Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging.LayoutModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Distributions.BinBox
Imports Microsoft.VisualBasic.Math.Statistics.Linq
Imports Microsoft.VisualBasic.Scripting.Expressions
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

Namespace FQ.NanoPlot
    ''' <summary>
    ''' 模拟 NanoPlot 的统计计算模块
    ''' </summary>
    Public Module NanoPlotEngine

        ''' <summary>
        ''' 主入口：计算所有统计数据
        ''' </summary>
        ''' <param name="reads">解析后的 FastQ 集合</param>
        ''' <returns>包含汇总统计和绘图数据的对象</returns>
        ''' 
        <Extension>
        Public Function CalculateNanoPlotData(reads As IEnumerable(Of FastQ)) As NanoPlotResult
            ' 1. 提取基础数据 (长度和质量)
            ' 优化：使用并行处理加速大数据集计算
            Dim rawData As List(Of ReadStats) = reads.ToList.AsParallel().Select(Function(r) New ReadStats(r.Length, CalculateMeanQuality(r))).AsList()

            If rawData.Count = 0 Then
                Return Nothing
            End If

            ' 2. 计算汇总统计
            Dim summary As NanoSummary = CalculateSummary(rawData)

            ' 3. 生成绘图数据
            ' 长度直方图 (通常使用对数坐标或较大的分箱)
            Dim lenHist = CreateHistogram(rawData.Select(Function(r) CDbl(r.Length)), 50, doLog:=True).ToArray

            ' 质量直方图
            Dim qualHist = CreateHistogram(rawData.Select(Function(r) r.MeanQuality), 50, doLog:=False).ToArray

            ' 长度 vs 质量 散点图 (进行降采样以提高前端渲染性能)
            Dim scatter = CreateScatterData(rawData, sampleSize:=10000).ToArray

            Return New NanoPlotResult With {
                .Summary = summary,
                .LengthHist = lenHist,
                .QualHist = qualHist,
                .ScatterData = scatter
            }
        End Function

        ''' <summary>
        ''' 计算一条序列的平均质量
        ''' </summary>
        Private Function CalculateMeanQuality(read As FastQ) As Double
            If String.IsNullOrEmpty(read.Quality) OrElse read.Quality.Length = 0 Then
                Return 0.0
            End If

            Dim q = FastQ.GetQualityOrder(read.Quality).ToArray
            Return q.Average
        End Function

        ''' <summary>
        ''' 计算汇总指标，包括 N50
        ''' </summary>
        Private Function CalculateSummary(data As List(Of ReadStats)) As NanoSummary
            Dim summary As New NanoSummary With {
                .TotalReads = data.Count,
                .TotalBases = data.Sum(Function(r) r.Length),
                .MinLength = data.Min(Function(r) r.Length),
                .MaxLength = data.Max(Function(r) r.Length),
                .MinQuality = data.Min(Function(r) r.MeanQuality),
                .MaxQuality = data.Max(Function(r) r.MeanQuality),
                .MeanLength = .TotalBases / .TotalReads  ' 计算平均长度
            }


            ' 计算中位数 (注意：大数据集排序可能较慢)
            Dim sortedLengths = data.Select(Function(r) r.Length).OrderBy(Function(x) x).ToArray
            summary.MedianLength = sortedLengths.Median

            Dim sortedQuals = data.Select(Function(r) r.MeanQuality).OrderBy(Function(x) x).ToArray
            summary.MedianQuality = sortedQuals.Median

            ' 计算平均质量
            summary.MeanQuality = data.Average(Function(r) r.MeanQuality)

            ' 计算 N50
            summary.N50 = CalculateN50(sortedLengths, summary.TotalBases)

            Return summary
        End Function

        ''' <summary>
        ''' 计算 N50 值
        ''' </summary>
        Private Function CalculateN50(sortedLengths As Double(), totalBases As Double) As Double
            Dim halfTotal As Double = totalBases / 2
            Dim runningSum As Double = 0

            ' sortedLengths 应该已经是升序排列，但N50通常是从大到小累加
            ' 为了性能，我们从后向前遍历（相当于降序）
            For i As Integer = sortedLengths.Length - 1 To 0 Step -1
                runningSum += sortedLengths(i)
                If runningSum >= halfTotal Then
                    Return sortedLengths(i)
                End If
            Next
            Return 0
        End Function

        ''' <summary>
        ''' 生成分箱数据用于直方图展示
        ''' </summary>
        Private Iterator Function CreateHistogram(values As IEnumerable(Of Double), bins As Integer, doLog As Boolean) As IEnumerable(Of HistogramBin)
            Dim list = values.ToArray
            If list.Count = 0 Then Return

            ' 如果是对数分箱，先对数据取 Log10
            Dim processedValues = If(doLog, list.Where(Function(v) v > 0).Select(Function(v) Math.Log10(v)).ToArray, list)

            If processedValues.Count = 0 Then Return

            Dim histogram As DataBinBox(Of Double)() = CutBins.FixedWidthBins(processedValues, k:=bins, eval:=Function(x) x).ToArray

            ' 转换为绘图用的 Bin 列表
            For Each bin As DataBinBox(Of Double) In histogram
                Dim startVal = bin.Boundary.Min
                Dim endVal = bin.Boundary.Max
                ' 如果之前做了 Log 转换，这里还原回去作为 Label
                Dim label As String

                If doLog Then
                    label = $"{Math.Pow(10, startVal):F0} - {Math.Pow(10, endVal):F0}"
                Else
                    label = $"{startVal:F1} - {endVal:F1}"
                End If

                Yield New HistogramBin With {
                    .Start = startVal,
                    .End = endVal,
                    .Count = bin.Count,
                    .Label = label
                }
            Next
        End Function

        ''' <summary>
        ''' 生成散点图数据，包含降采样逻辑
        ''' </summary>
        Private Iterator Function CreateScatterData(data As List(Of ReadStats), sampleSize As Integer) As IEnumerable(Of Point2D)
            ' 如果数据量小于采样数，直接返回全部
            If data.Count <= sampleSize Then
                For Each r As ReadStats In data
                    Yield New Point2D(r.Length, r.MeanQuality)
                Next
            Else
                ' 简单的随机降采样
                ' 使用蓄水池抽样算法 或者简单的随机索引
                Dim indices As New HashSet(Of Integer)()

                While indices.Count < sampleSize
                    indices.Add(randf.NextInteger(0, data.Count))
                End While

                For Each idx In indices
                    Yield New Point2D With {
                        .X = data(idx).Length,
                        .Y = data(idx).MeanQuality
                    }
                Next
            End If
        End Function

    End Module
End Namespace