#Region "Microsoft.VisualBasic::92133440a9ca1f745486a53f29774711, RNA-Seq\RNA-seq.Data\FastQ\NanoPlot\NanoData.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 90
    '    Code Lines: 49 (54.44%)
    ' Comment Lines: 28 (31.11%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 13 (14.44%)
    '     File Size: 2.85 KB


    '     Structure ReadStats
    ' 
    '         Properties: Length, MeanQuality
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class NanoSummary
    ' 
    '         Properties: HighestQualityReads, LongestReads, MaxLength, MaxQuality, MeanLength
    '                     MeanQuality, MedianLength, MedianQuality, MinLength, MinQuality
    '                     N50, Q10, Q15, Q20, Q25
    '                     Q30, ReadLengthStdev, TotalBases, TotalReads
    '         Class QThresholdStats
    ' 
    '             Properties: Count, Percentage, TotalBasesMb
    ' 
    ' 
    ' 
    '     Class ReadsValue
    ' 
    '         Properties: Length, Quality
    ' 
    '         Function: ToString
    ' 
    '     Class HistogramBin
    ' 
    '         Properties: [End], Count, Label, Start
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
