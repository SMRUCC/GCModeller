#Region "Microsoft.VisualBasic::5cafb6693a32dfd7be1e29e02c39fc55, analysis\SequenceToolkit\SequenceAlignment\siRNAHit\TargetFinder.vb"

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

    '   Total Lines: 220
    '    Code Lines: 147 (66.82%)
    ' Comment Lines: 43 (19.55%)
    '    - Xml Docs: 76.74%
    ' 
    '   Blank Lines: 30 (13.64%)
    '     File Size: 9.54 KB


    '     Class TargetFinder
    ' 
    '         Properties: MaxStrictMismatch, MaxTotalMismatch, MaxWobble, MinHitLength, ScoreCutoff
    ' 
    '         Function: HasCenterMismatch, MaskSite, ParsePenalty, PassFilter, PositionWeight
    '                   Run, Score, ScoreByPosition
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.SmithWaterman
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace siRNAHit

    ''' <summary>
    ''' TargetFinder 小RNA靶标预测算法实现。
    ''' 
    ''' 核心：将 miRNA 反向互补为正向 query，与候选 mRNA 做 Smith-Waterman 局部
    ''' 比对，再从比对串中按 miRNA 5'→3' 逐位施加位置权重罚分，最后按四条规则过滤。
    ''' 参考 siRNA.md 给出的 ssearch35 参数：<c>-r +15/-10</c>。
    ''' </summary>
    Public Class TargetFinder : Implements miRNAMapper

        ' 位置权重系数（从 miRNA 5' 端、1-based 计）：
        '   第 1 位：1×
        '   第 2–13 位：2× （含切割位点 10–11）
        '   第 14–21 位：1×
        Private Const CORE_START As Integer = 2
        Private Const CORE_END As Integer = 13

        ' 碱基配对基础罚分
        Private Const PEN_WC As Double = 0.0
        Private Const PEN_WOBBLE As Double = 0.5
        Private Const PEN_MISMATCH As Double = 1.0
        Private Const PEN_GAP As Double = 1.0

        ' 过滤阈值（文档：Score ≤ 4.0 严格 / 5.0 标准 / 7.0 宽松）
        Public Property ScoreCutoff As Double = 5.0
        Public Property MaxTotalMismatch As Integer = 7     ' mismatch + wobble 总数
        Public Property MaxStrictMismatch As Integer = 4    ' 严格错配（不含 wobble/gap）
        Public Property MaxWobble As Integer = 4
        ' 最小比对长度（HSP size），避免极短随机命中
        Public Property MinHitLength As Integer = 17

        Public Overrides Function ToString() As String
            Return $"[*TargetFinder score_cut={ScoreCutoff}]"
        End Function

        ''' <summary>
        ''' 对单条 miRNA 在整条 mRNA 上做位置加权罚分统计。
        ''' </summary>
        ''' <param name="mirna">miRNA 正向序列</param>
        ''' <param name="mrna">mRNA 正向序列</param>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Score(mirna As String, mrna As String) As siRNAHit
            Dim revComp As String = mirna.ReverseComplementRNA()
            Dim hsp As LocalHSPMatch(Of Char) = RNASeqHelper.BestLocalHit(revComp, mrna)

            If hsp Is Nothing OrElse (hsp.toB - hsp.fromB + 1) < MinHitLength Then
                Return Nothing
            End If

            Dim hit As siRNAHit = siRNAHit.FromHSP(mirna, hsp, "TargetFinder")
            Dim raw As String = ScoreByPosition(mirna, hsp)
            Dim penalty As Double = ParsePenalty(raw)
            hit.Expectation = penalty
            hit.TranslationInhibition = HasCenterMismatch(mirna, hsp)

            Return hit
        End Function

        ''' <summary>
        ''' 由 HSP 的比对串构造位置加权罚分明细字符串，并统计总罚分。
        ''' 位置权重按 miRNA 5'→3' 1-based 计数（由比对列经
        ''' <see cref="MirnaPosition"/> 换算，因为 query 是 miRNA 的反向互补链）。
        ''' </summary>
        Friend Function ScoreByPosition(mirna As String, hsp As LocalHSPMatch(Of Char)) As String
            Dim s1 As Char() = hsp.seq1
            Dim s2 As Char() = hsp.seq2
            Dim n As Integer = Math.Min(s1.Length, s2.Length)
            Dim sb As New System.Text.StringBuilder()

            For i As Integer = 0 To n - 1
                ' query 是 miRNA 的反向互补，需换算回 miRNA 的 5'->3' 1-based 坐标
                Dim pos As Integer = MirnaPosition(mirna, hsp, i)
                Dim t As RNASeqHelper.PairType = RNASeqHelper.ClassifyPair(s1(i), s2(i))
                Dim w As Double = PositionWeight(pos)

                Dim base As Double
                Select Case t
                    Case RNASeqHelper.PairType.WC : base = PEN_WC
                    Case RNASeqHelper.PairType.Wobble : base = PEN_WOBBLE
                    Case RNASeqHelper.PairType.Mismatch : base = PEN_MISMATCH
                    Case RNASeqHelper.PairType.Gap : base = PEN_GAP
                End Select

                Dim pen As Double = w * base
                sb.Append($"[{pos}] {s1(i)}:{s2(i)} t={t} w={w} pen={pen:F2}; ")
            Next

            Return sb.ToString()
        End Function

        ''' <summary>
        ''' 从 <see cref="ScoreByPosition"/> 的明细串中解析总罚分（仅用于演示/调试）。
        ''' 实际 scoring 走 <see cref="Score"/> 内部的 penalty 计算。
        ''' </summary>
        Friend Function ParsePenalty(detail As String) As Double
            Dim total As Double = 0.0
            Dim idx As Integer = detail.IndexOf("pen=")

            While idx >= 0
                Dim endIdx As Integer = detail.IndexOf(";", idx)
                If endIdx < 0 Then endIdx = detail.Length
                Dim valStr As String = detail.Substring(idx + 4, endIdx - (idx + 4)).Trim()
                Dim v As Double
                If Double.TryParse(valStr, v) Then
                    total += v
                End If
                idx = detail.IndexOf("pen=", endIdx)
            End While

            Return total
        End Function

        ''' <summary>miRNA 5'→3' 位置权重（1-based）。</summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function PositionWeight(mirnaPos As Integer) As Double
            If mirnaPos >= CORE_START AndAlso mirnaPos <= CORE_END Then
                Return 2.0
            Else
                Return 1.0
            End If
        End Function

        ''' <summary>
        ''' 切割位点（miRNA 第 10–11 位）是否存在错配 → 翻译抑制候选。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Friend Function HasCenterMismatch(mirna As String, hsp As LocalHSPMatch(Of Char)) As Boolean
            Dim s1 As Char() = hsp.seq1
            Dim s2 As Char() = hsp.seq2
            Dim n As Integer = Math.Min(s1.Length, s2.Length)

            For i As Integer = 0 To n - 1
                Dim pos As Integer = MirnaPosition(mirna, hsp, i)
                If pos = 10 OrElse pos = 11 Then
                    Dim t As RNASeqHelper.PairType = RNASeqHelper.ClassifyPair(s1(i), s2(i))
                    If t = RNASeqHelper.PairType.Mismatch Then
                        Return True
                    End If
                End If
            Next

            Return False
        End Function

        ''' <summary>
        ''' 应用四条过滤规则：总错配≤7、严格错配≤4、G:U≤4、加权罚分≤cutoff，且长度达标。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function PassFilter(hit As siRNAHit) As Boolean
            If hit Is Nothing Then
                Return False
            End If
            If hit.Length < MinHitLength Then
                Return False
            End If
            If hit.MismatchCount + hit.WobbleCount > MaxTotalMismatch Then
                Return False
            End If
            If hit.MismatchCount > MaxStrictMismatch Then
                Return False
            End If
            If hit.WobbleCount > MaxWobble Then
                Return False
            End If
            If hit.Expectation > ScoreCutoff Then
                Return False
            End If
            Return True
        End Function

        ''' <summary>
        ''' 对一组候选 mRNA 执行预测，并额外进行 two-hits 掩蔽重搜
        ''' （get_additional：将已命中区域用 N 掩蔽后重搜额外靶位点）。
        ''' </summary>
        ''' <param name="mirna">miRNA 序列对象（FASTA）</param>
        ''' <param name="targets">候选 mRNA 序列集合</param>
        Public Iterator Function Run(mirna As FastaSeq, targets As IEnumerable(Of FastaSeq)) As IEnumerable(Of siRNAHit) Implements miRNAMapper.Run
            Dim query As String = mirna.Title.TrimStart(">"c)
            Dim mirnaSeq As String = mirna.SequenceData.ToUpper

            For Each t In targets
                Dim id As String = t.Title.TrimStart(">"c)
                Dim seq As String = t.SequenceData.ToUpper

                ' 首次命中
                Dim hit As siRNAHit = Score(mirnaSeq, seq)
                If PassFilter(hit) Then
                    hit.miRNA = query
                    hit.Target = id
                    Yield hit
                End If

                ' get_additional：掩蔽已命中区域后重搜额外靶位点
                If hit IsNot Nothing Then
                    seq = MaskSite(seq, hit.StartSite, hit.EndSite)
                    Dim extra As siRNAHit = Score(mirnaSeq, seq)

                    If PassFilter(extra) Then
                        extra.miRNA = query
                        extra.Target = id & "_secondary"
                        Yield extra
                    End If
                End If
            Next
        End Function

        ''' <summary>将 mRNA 上 [start,end]（1-based）区域掩蔽为 N，用于 two-hits 重搜。</summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function MaskSite(seq As String, start1 As Integer, end1 As Integer) As String
            Dim chars As Char() = seq.ToCharArray()
            Dim a As Integer = Math.Max(0, start1 - 1)
            Dim b As Integer = Math.Min(chars.Length - 1, end1 - 1)

            For i As Integer = a To b
                chars(i) = "N"c
            Next

            Return New String(chars)
        End Function
    End Class
End Namespace

