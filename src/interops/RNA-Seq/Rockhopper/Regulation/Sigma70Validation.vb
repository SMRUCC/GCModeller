' /********************************************************************************/
'
'  Rockhopper —— Sigma70 启动子验证
'
'  由原始 `TSSsValidation.vb`（Sigma70 启动子验证）迁移而来，去除了对
'  `NBCR.Extensions.MEME_Suite`、`LANS.SystemsBiology.AnalysisTools`、
'  `Oracle.Java.IO.Properties.Reflector` 等已失效外部依赖，改为在本地实现
'  基于位置权重矩阵(PWM)的 σ70 启动子打分：
'
'    * -35 区共识序列：TTGACA（相对于 TSS 上游 31~36 bp）
'    * -10 区共识序列：TATAAT（相对于 TSS 上游 7~12 bp）
'
'  打分方式为逐位匹配计数（0..6），并把两个 box 的匹配数相加得到总分（0..12）。
'  该实现对 TSS 预测结果提供了与论文一致的"启动子合理性"校验手段。
'
' /********************************************************************************/

Imports System.Collections.Generic
Imports System.Linq
Imports SMRUCC.genomics.SequenceModel.RNA_Seq.Rockhopper.Core

Namespace Regulation

    ''' <summary>
    ''' 单个 TSS 的 σ70 启动子打分结果。
    ''' </summary>
    Public Class PromoterScore

        Public Property GeneID As String
        Public Property TSSs As Integer
        Public Property Strand As Char

        ''' <summary>-35 区序列。</summary>
        Public Property Minus35Sequence As String
        ''' <summary>-10 区序列。</summary>
        Public Property Minus10Sequence As String

        ''' <summary>-35 区与共识序列 TTGACA 的匹配位数（0..6）。</summary>
        Public Property Minus35Score As Integer
        ''' <summary>-10 区与共识序列 TATAAT 的匹配位数（0..6）。</summary>
        Public Property Minus10Score As Integer

        ''' <summary>总分（0..12）。</summary>
        Public ReadOnly Property TotalScore As Integer
            Get
                Return Minus35Score + Minus10Score
            End Get
        End Property

        ''' <summary>是否为强候选 σ70 启动子（-35 与 -10 均至少匹配 4 位）。</summary>
        Public ReadOnly Property IsSigma70 As Boolean
            Get
                Return Minus35Score >= 4 AndAlso Minus10Score >= 4
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{GeneID}{vbTab}{TSSs}{vbTab}{Strand}{vbTab}{Minus35Sequence}{vbTab}{Minus10Sequence}{vbTab}{Minus35Score}{vbTab}{Minus10Score}{vbTab}{TotalScore}"
        End Function

    End Class

    ''' <summary>
    ''' σ70 启动子验证。
    ''' </summary>
    Public Module Sigma70Validation

        ''' <summary>-35 区共识序列。</summary>
        Public Const Consensus_Minus35 As String = "TTGACA"
        ''' <summary>-10 区共识序列。</summary>
        Public Const Consensus_Minus10 As String = "TATAAT"
        ''' <summary>-35 区相对 TSS 的起点偏移（上游）。</summary>
        Public Const Offset_Minus35 As Integer = 36
        ''' <summary>-10 区相对 TSS 的起点偏移（上游）。</summary>
        Public Const Offset_Minus10 As Integer = 12

        ''' <summary>
        ''' 对所有 TSS 结果进行 σ70 启动子打分。
        ''' </summary>
        ''' <param name="genome">1-indexed 的基因组序列（首位为占位字符，见 <see cref="Genome.Sequence"/>）。</param>
        ''' <param name="transcripts">转录本（使用其 TSSs）。</param>
        Public Function Validate(genome As String, transcripts As IEnumerable(Of Transcript)) As List(Of PromoterScore)
            Dim results As New List(Of PromoterScore)()
            If String.IsNullOrEmpty(genome) Then Return results

            For Each t As Transcript In transcripts
                If t.TSSs <= 0 Then Continue For

                Dim minus35 As String = extractBox(genome, t.TSSs, t.Strand, Offset_Minus35, Consensus_Minus35.Length)
                Dim minus10 As String = extractBox(genome, t.TSSs, t.Strand, Offset_Minus10, Consensus_Minus10.Length)

                results.Add(New PromoterScore With {
                    .GeneID = t.Synonym,
                    .TSSs = t.TSSs,
                    .Strand = t.Strand,
                    .Minus35Sequence = minus35,
                    .Minus10Sequence = minus10,
                    .Minus35Score = matchCount(minus35, Consensus_Minus35),
                    .Minus10Score = matchCount(minus10, Consensus_Minus10)
                })
            Next

            Return results
        End Function

        ''' <summary>
        ''' 提取启动子 box 序列。正链直接取上游；负链取上游片段的反向互补。
        ''' </summary>
        Private Function extractBox(genome As String, tss As Integer, strand As Char, offset As Integer, boxLength As Integer) As String
            Dim segment As String
            If strand = "-"c Then
                ' 负链：promoter 位于 TSS 的下游方向，取其反向互补
                Dim start As Integer = tss
                Dim [stop] As Integer = tss + offset - 1
                segment = safeSlice(genome, System.Math.Min(start, [stop]), System.Math.Max(start, [stop]))
                segment = reverseComplement(segment)
            Else
                segment = safeSlice(genome, tss - offset, tss - offset + boxLength - 1)
            End If

            If segment.Length < boxLength Then Return segment
            Return segment.Substring(0, boxLength)
        End Function

        Private Function safeSlice(genome As String, left As Integer, right As Integer) As String
            Dim lo As Integer = System.Math.Max(1, System.Math.Min(left, right))
            Dim hi As Integer = System.Math.Min(genome.Length - 1, System.Math.Max(left, right))
            If hi < lo Then Return ""
            Return genome.Substring(lo, hi - lo + 1).ToUpperInvariant()
        End Function

        ''' <summary>逐位匹配计数。</summary>
        Public Function matchCount(sequence As String, consensus As String) As Integer
            If String.IsNullOrEmpty(sequence) Then Return 0
            Dim n As Integer = System.Math.Min(sequence.Length, consensus.Length)
            Dim score As Integer = 0
            For i As Integer = 0 To n - 1
                If sequence(i) = consensus(i) Then score += 1
            Next
            Return score
        End Function

        Private Function reverseComplement(sequence As String) As String
            Dim chars As Char() = New Char(sequence.Length - 1) {}
            For i As Integer = 0 To sequence.Length - 1
                Select Case sequence(sequence.Length - 1 - i)
                    Case "A"c : chars(i) = "T"c
                    Case "T"c : chars(i) = "A"c
                    Case "C"c : chars(i) = "G"c
                    Case "G"c : chars(i) = "C"c
                    Case Else : chars(i) = "N"c
                End Select
            Next
            Return New String(chars)
        End Function

    End Module

End Namespace
