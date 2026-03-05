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
        Public Property N50 As Double
        Public Property MinLength As Long
        Public Property MaxLength As Long

        Public Property MeanQuality As Double
        Public Property MedianQuality As Double
        Public Property MinQuality As Double
        Public Property MaxQuality As Double

        ' 可以根据需要添加 Q20, Q7 等比例统计
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