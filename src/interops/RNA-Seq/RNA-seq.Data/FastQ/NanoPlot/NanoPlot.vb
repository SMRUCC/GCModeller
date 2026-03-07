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