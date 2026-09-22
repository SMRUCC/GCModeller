' ============================================================================
' SampleNameTimeParser.vb — 从样本名解析时间标签（伪时间的最后一道回退）
'
' readme 一.2 假设上游已经提供 Monocle3 的伪时间；但真实工作流里经常遇到
' 两种"拿不到显式伪时间"的情形：
'   1. 表达矩阵本身是时间序列实验，样本名里就写着时间（如 T1.2h_Rep1、t12、time_5.5）；
'   2. 数据读取时没有传递 SampleInfo，TimePoints 全为 0。
' 此时按样本名里的时间标签排序，比"随机顺序"或"直接报错"要合理得多，
' 因此这里作为伪时间来源的最后一道回退（见 PseudotimeDiscretizer 的解析优先级）。
'
' 识别顺序（先精确、后宽松，避免把 Rep1/Rep2 的重复编号误当成时间）：
'   ① T<num>h / t<num>h        例：T1.2h_Rep1  → 1.2
'   ② <num>h                   例：1.2h_rep1   → 1.2
'   ③ time_<num> / time-<num>  例：time_5.5    → 5.5
'   ④ 末尾的 T<num>            例：sample_T12  → 12
' 以上均失败时返回 False，由调用方统计失败样本数并给出诊断。
' ============================================================================

Imports System.Text.RegularExpressions

Namespace Data

    ''' <summary>批量解析结果</summary>
    Public Class SampleTimeParseResult

        ''' <summary>每个样本解析出的时间值（解析失败的样本为 <see cref="Double.NaN"/>)</summary>
        Public Property TimePoints As Double()

        ''' <summary>成功解析的样本数</summary>
        Public Property Parsed As Integer

        ''' <summary>解析失败的样本数</summary>
        Public Property Failed As Integer

        ''' <summary>解析失败的样本名（诊断用）</summary>
        Public Property FailedSamples As String()

        ''' <summary>是否全部样本都解析成功</summary>
        Public ReadOnly Property AllParsed As Boolean
            Get
                Return Failed = 0
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"SampleTimeParse(parsed={Parsed}, failed={Failed})"
        End Function

    End Class

    ''' <summary>样本名 → 时间标签 的解析器</summary>
    Public Module SampleNameTimeParser

        ' 注意顺序：先匹配带 T/t 前缀的形式，避免把 "_Rep1" 里的 1 误识别为时间
        Private ReadOnly PatternPrefixHour As New Regex(
            "[Tt]\s*([0-9]+(?:\.[0-9]+)?)\s*h", RegexOptions.Compiled)
        Private ReadOnly PatternHour As New Regex(
            "([0-9]+(?:\.[0-9]+)?)\s*h", RegexOptions.Compiled)
        Private ReadOnly PatternTimeWord As New Regex(
            "[Tt]ime[\s_\-]*([0-9]+(?:\.[0-9]+)?)", RegexOptions.Compiled)
        Private ReadOnly PatternTrailingT As New Regex(
            "[Tt]([0-9]+(?:\.[0-9]+)?)\s*$", RegexOptions.Compiled)

        Private ReadOnly Patterns As Regex() = {
            PatternPrefixHour,
            PatternHour,
            PatternTimeWord,
            PatternTrailingT
        }

        ''' <summary>
        ''' 尝试从样本名中解析时间标签。
        ''' </summary>
        ''' <param name="sampleName">样本名，如 T1.2h_Rep1</param>
        ''' <param name="value">解析出的时间值</param>
        ''' <returns>解析成功返回 True</returns>
        Public Function TryParse(sampleName As String, ByRef value As Double) As Boolean
            value = Double.NaN
            If String.IsNullOrWhiteSpace(sampleName) Then Return False

            For Each pattern In Patterns
                Dim m = pattern.Match(sampleName)
                If m.Success Then
                    Dim text = m.Groups(1).Value
                    Dim parsed As Double
                    If Double.TryParse(text, Globalization.NumberStyles.Float,
                                       Globalization.CultureInfo.InvariantCulture, parsed) Then
                        value = parsed
                        Return True
                    End If
                End If
            Next

            Return False
        End Function

        ''' <summary>批量解析一组样本名</summary>
        Public Function ParseAll(sampleNames As String()) As SampleTimeParseResult
            If sampleNames Is Nothing Then
                Throw New ArgumentNullException(NameOf(sampleNames))
            End If

            Dim values(sampleNames.Length - 1) As Double
            Dim failed As New List(Of String)()
            Dim parsed = 0

            For i = 0 To sampleNames.Length - 1
                Dim v As Double
                If TryParse(sampleNames(i), v) Then
                    values(i) = v
                    parsed += 1
                Else
                    values(i) = Double.NaN
                    failed.Add(sampleNames(i))
                End If
            Next

            Return New SampleTimeParseResult With {
                .TimePoints = values,
                .Parsed = parsed,
                .Failed = failed.Count,
                .FailedSamples = failed.ToArray()
            }
        End Function

    End Module

End Namespace
