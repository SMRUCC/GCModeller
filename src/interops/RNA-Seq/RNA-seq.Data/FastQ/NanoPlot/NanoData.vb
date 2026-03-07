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

        ''' <summary>
        ''' Top 5 最长序列 (Item1=Length, Item2=Quality)
        ''' </summary>
        ''' <returns></returns>
        Public Property LongestReads As ReadsValue()

        ''' <summary>
        ''' Top 5 质量最高序列 (Item1=Quality, Item2=Length)
        ''' </summary>
        ''' <returns></returns>
        Public Property HighestQualityReads As ReadsValue()

        ''' <summary>
        ''' Q值阈值统计
        ''' </summary>
        Public Class QThresholdStats
            Public Property Count As Long
            Public Property Percentage As Double
            ''' <summary>
            ''' 以 Mb 为单位
            ''' </summary>
            ''' <returns></returns>
            Public Property TotalBasesMb As Double
        End Class

        Public Property Q10 As New QThresholdStats
        Public Property Q15 As New QThresholdStats
        Public Property Q20 As New QThresholdStats
        Public Property Q25 As New QThresholdStats
        Public Property Q30 As New QThresholdStats
    End Class

    Public Class ReadsValue

        Public Property Quality As Double
        Public Property Length As Double

        Public Overrides Function ToString() As String
            Return $"(quality={Quality}, length={Length})"
        End Function

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