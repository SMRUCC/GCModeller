#Region "Microsoft.VisualBasic::11ea41978d512db3c3fd90b4b8580ab4, RNA-Seq\RNA-seq.Data\FastQ\NanoPlot\NanoPlot.vb"

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

    '   Total Lines: 68
    '    Code Lines: 44 (64.71%)
    ' Comment Lines: 14 (20.59%)
    '    - Xml Docs: 57.14%
    ' 
    '   Blank Lines: 10 (14.71%)
    '     File Size: 3.11 KB


    '     Class NanoPlotResult
    ' 
    '         Properties: LengthHist, QualHist, ScatterData, Summary
    ' 
    '         Function: ToString
    ' 
    '         Sub: GetNanoPlotReport
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.Imaging.LayoutModel

Namespace FQ.NanoPlot

    Public Class NanoPlotResult

        Public Property Summary As NanoSummary
        Public Property LengthHist As HistogramBin()
        Public Property QualHist As HistogramBin()
        Public Property ScatterData As Point2D()

        ''' <summary>
        ''' generates the summary text
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder
            Call GetNanoPlotReport(sb)
            Return sb.ToString
        End Function

        ''' <summary>
        ''' 获取NanoPlot格式的文本报告
        ''' </summary>
        ''' <param name="sb"></param>
        Public Sub GetNanoPlotReport(sb As StringBuilder)
            Dim s = Me.Summary

            sb.AppendLine("Metrics" & vbTab & "dataset")
            sb.AppendLine($"number_of_reads{vbTab}{s.TotalReads}")
            sb.AppendLine($"number_of_bases{vbTab}{s.TotalBases:F1}")
            sb.AppendLine($"median_read_length{vbTab}{s.MedianLength:F1}")
            sb.AppendLine($"mean_read_length{vbTab}{s.MeanLength:F1}")
            sb.AppendLine($"read_length_stdev{vbTab}{s.ReadLengthStdev:F1}")
            sb.AppendLine($"n50{vbTab}{s.N50:F1}")
            sb.AppendLine($"mean_qual{vbTab}{s.MeanQuality:F1}")
            sb.AppendLine($"median_qual{vbTab}{s.MedianQuality:F1}")

            ' 输出 Longest Reads
            ' 格式: longest_read_(with_Q):1 48062 (20.0)
            If s.LongestReads IsNot Nothing Then
                For i As Integer = 0 To Math.Min(s.LongestReads.Count - 1, 4)
                    Dim item = s.LongestReads(i)
                    sb.AppendLine($"longest_read_(with_Q):{i + 1}{vbTab}{CInt(item.Length)} ({item.Quality:F1})")
                Next
            End If

            ' 输出 Highest Quality Reads
            ' 格式: highest_Q_read_(with_length):1 28.9 (3936)
            If s.HighestQualityReads IsNot Nothing Then
                For i As Integer = 0 To Math.Min(s.HighestQualityReads.Count - 1, 4)
                    Dim item = s.HighestQualityReads(i)
                    sb.AppendLine($"highest_Q_read_(with_length):{i + 1}{vbTab}{item.Quality:F1} ({CInt(item.Length)})")
                Next
            End If

            ' 输出 Q阈值统计
            ' 格式: Reads >Q10: 37404 (99.9%) 114.6Mb
            sb.AppendLine($"Reads >Q10:{vbTab}{s.Q10.Count} ({s.Q10.Percentage:F1}%) {s.Q10.TotalBasesMb:F1}Mb")
            sb.AppendLine($"Reads >Q15:{vbTab}{s.Q15.Count} ({s.Q15.Percentage:F1}%) {s.Q15.TotalBasesMb:F1}Mb")
            sb.AppendLine($"Reads >Q20:{vbTab}{s.Q20.Count} ({s.Q20.Percentage:F1}%) {s.Q20.TotalBasesMb:F1}Mb")
            sb.AppendLine($"Reads >Q25:{vbTab}{s.Q25.Count} ({s.Q25.Percentage:F1}%) {s.Q25.TotalBasesMb:F1}Mb")
            sb.AppendLine($"Reads >Q30:{vbTab}{s.Q30.Count} ({s.Q30.Percentage:F1}%) {s.Q30.TotalBasesMb:F1}Mb")
        End Sub
    End Class

End Namespace
