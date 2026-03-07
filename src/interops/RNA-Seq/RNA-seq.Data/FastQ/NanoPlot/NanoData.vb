Namespace FQ.NanoPlot

    ''' <summary>
    ''' 用于存储单个序列统计指标的结构体
    ''' </summary>
    Public Structure ReadStats
        Public Property Length As Double
        Public Property MeanQuality As Double

        Public Sub New(len As Integer, qual As Double)
            Me.Length = len
            Me.MeanQuality = qual
        End Sub
    End Structure

    ''' <summary>
    ''' 汇总统计结果模型
    ''' </summary>
    Public Class NanoSummary
        Public Property TotalReads As Double
        Public Property TotalBases As Double
        Public Property MeanLength As Double
        Public Property MedianLength As Double
        ''' <summary>
        ''' 长度标准差
        ''' </summary>
        ''' <returns></returns>
        Public Property ReadLengthStdev As Double
        Public Property N50 As Double
        Public Property MinLength As Long
        Public Property MaxLength As Long

        Public Property MeanQuality As Double
        Public Property MedianQuality As Double
        Public Property MinQuality As Double
        Public Property MaxQuality As Double

        ' 新增：Top 5 最长序列 (Item1=Length, Item2=Quality)
        Public Property LongestReads As List(Of Tuple(Of Double, Double))

        ' 新增：Top 5 质量最高序列 (Item1=Quality, Item2=Length)
        Public Property HighestQualityReads As List(Of Tuple(Of Double, Double))

        ' 新增：Q值阈值统计
        Public Class QThresholdStats
            Public Property Count As Long
            Public Property Percentage As Double
            Public Property TotalBasesMb As Double ' 以 Mb 为单位
        End Class

        Public Property Q10 As New QThresholdStats
        Public Property Q15 As New QThresholdStats
        Public Property Q20 As New QThresholdStats
        Public Property Q25 As New QThresholdStats
        Public Property Q30 As New QThresholdStats
    End Class

    ''' <summary>
    ''' 直方图分箱数据，用于绘图
    ''' </summary>
    Public Class HistogramBin
        Public Property Label As String
        Public Property Count As Double
        Public Property Start As Double
        Public Property [End] As Double
    End Class
End Namespace