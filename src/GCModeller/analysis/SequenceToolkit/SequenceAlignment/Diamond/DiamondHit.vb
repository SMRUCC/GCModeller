' DIAMOND 比对结果模型 (DiamondHit)
'
' 对应 BLAST m8 / tabular 输出的一行,供 GCModeller 内部 API 返回。
' 字段命名与 m8 格式对齐,便于后续对接外部分析流程。

Imports SMRUCC.genomics.Analysis.SequenceAlignment

Namespace DIAMOND

    ''' <summary>
    ''' 单条 DIAMOND 比对命中(m8 风格)。
    ''' </summary>
    Public Class DiamondHit

        ''' <summary>查询序列标识。</summary>
        Public Property QueryTitle As String
        ''' <summary>参考(主题)序列标识。</summary>
        Public Property SubjectTitle As String

        ''' <summary>百分比一致性 (0-100)。</summary>
        Public Property PercentIdentity As Double
        ''' <summary>比对长度(对齐列数)。</summary>
        Public Property AlignmentLength As Integer
        ''' <summary>错配数。</summary>
        Public Property Mismatches As Integer
        ''' <summary>缺口数。</summary>
        Public Property GapOpens As Integer

        ''' <summary>查询起始 (1-based)。</summary>
        Public Property QueryStart As Integer
        ''' <summary>查询结束 (1-based, 含)。</summary>
        Public Property QueryEnd As Integer
        ''' <summary>主题起始 (1-based)。</summary>
        Public Property SubjectStart As Integer
        ''' <summary>主题结束 (1-based, 含)。</summary>
        Public Property SubjectEnd As Integer

        ''' <summary>原始 SW 比对得分 (bits 之前的原始分)。</summary>
        Public Property RawScore As Double
        ''' <summary>bit-score(本阶段用简易换算,后续可接入统计模型)。</summary>
        Public Property BitScore As Double
        ''' <summary>e-value(本阶段为占位估算,后续接入 Karlin-Altschul 模型)。</summary>
        Public Property Evalue As Double

        ''' <summary>查询序列比对片段。</summary>
        Public Property QueryFragment As String
        ''' <summary>主题序列比对片段。</summary>
        Public Property SubjectFragment As String

        ''' <summary>
        ''' 将 BandHit(0-based 全局坐标)转换为 1-based 的 DiamondHit。
        ''' </summary>
        Public Shared Function FromBandHit(globalQuery As String,
                                           globalSubject As String,
                                           queryTitle As String,
                                           subjectTitle As String,
                                           band As BandHit) As DiamondHit
            Dim qLen = band.QueryEnd - band.QueryStart + 1
            Dim sLen = band.SubjectEnd - band.SubjectStart + 1
            Dim alnLen = Math.Max(qLen, sLen)

            Dim mismatches As Integer = 0
            Dim gaps As Integer = 0
            Dim matches As Integer = 0

            For i As Integer = 0 To alnLen - 1
                Dim qaa = If(i < band.QueryFragment.Length, band.QueryFragment(i), "-"c)
                Dim saa = If(i < band.SubjectFragment.Length, band.SubjectFragment(i), "-"c)

                If qaa = "-"c OrElse saa = "-"c Then
                    gaps += 1
                ElseIf qaa = saa Then
                    matches += 1
                Else
                    mismatches += 1
                End If
            Next

            ' 接入 Karlin-Altschul 统计模型(BLOSUM62 统计量)
            Dim queryLen = globalQuery.Length
            Dim subjectLen = globalSubject.Length
            ' λ=0.267, K=0.041 (BLOSUM62 经验标定,源自 EValue 模块常量)
            Dim bitScore = (0.267 * band.Score - Math.Log(0.041)) / Math.Log(2)
            Dim eValCalc = SMRUCC.genomics.Analysis.SequenceAlignment.EValue.Compute(band.Score, queryLen, subjectLen)

            Dim hit As New DiamondHit With {
                .QueryTitle = queryTitle,
                .SubjectTitle = subjectTitle,
                .RawScore = band.Score,
                .BitScore = bitScore,
                .Evalue = eValCalc,
                .AlignmentLength = alnLen,
                .Matches = matches,
                .Mismatches = mismatches,
                .GapOpens = gaps,
                .PercentIdentity = If(alnLen > 0, 100.0 * matches / alnLen, 0),
                .QueryStart = band.QueryStart + 1,
                .QueryEnd = band.QueryEnd + 1,
                .SubjectStart = band.SubjectStart + 1,
                .SubjectEnd = band.SubjectEnd + 1,
                .QueryFragment = band.QueryFragment,
                .SubjectFragment = band.SubjectFragment
            }

            Return hit
        End Function

        ''' <summary>比对中的一致列数(内部计算用)。</summary>
        Friend Property Matches As Integer

        Public Overrides Function ToString() As String
            Return $"{QueryTitle}{vbTab}{SubjectTitle}{vbTab}{PercentIdentity:F1}{vbTab}{AlignmentLength}{vbTab}{Mismatches}{vbTab}{GapOpens}{vbTab}{QueryStart}{vbTab}{QueryEnd}{vbTab}{SubjectStart}{vbTab}{SubjectEnd}{vbTab}{RawScore:F0}{vbTab}{Evalue}"
        End Function
    End Class
End Namespace
