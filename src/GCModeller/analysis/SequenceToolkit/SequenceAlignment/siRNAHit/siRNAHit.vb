Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace siRNAHit

    ''' <summary>
    ''' 小RNA（miRNA/siRNA）与靶标 mRNA 的互补匹配命中结果模型。
    ''' 两个算法（psRNATarget / TargetFinder）共用此结构，便于后续求交集。
    ''' </summary>
    Public Class siRNAHit

        ''' <summary>小RNA 序列唯一标识（FastaSeq.Title 或 locus_tag）</summary>
        Public Property miRNA As String
        ''' <summary>靶标 mRNA 序列唯一标识</summary>
        Public Property Target As String

        ''' <summary>mRNA 靶位点起点（1-based，对应 HSP.fromB）</summary>
        Public Property StartSite As Integer
        ''' <summary>mRNA 靶位点终点（1-based，对应 HSP.toB）</summary>
        Public Property EndSite As Integer
        ''' <summary>靶位点长度（nt）</summary>
        Public Property Length As Integer

        ''' <summary>
        ''' 量化互补质量的分数：
        ''' psRNATarget 为位置加权期望值（越低越好），
        ''' TargetFinder 为加权罚分总和（越低越好）。
        ''' </summary>
        Public Property Expectation As Double

        ''' <summary>严格错配数（不含 G:U 与 gap）</summary>
        Public Property MismatchCount As Integer
        ''' <summary>G:U wobble 配对数</summary>
        Public Property WobbleCount As Integer
        ''' <summary>缺口（单侧凸起）数</summary>
        Public Property GapCount As Integer

        ''' <summary>比对可视化字符串：miRNA 行 / mRNA 行</summary>
        Public Property Alignment As String

        ''' <summary>
        ''' 是否翻译抑制候选：互补区中心（约 miRNA 第 10–11 位切割位点）存在错配时为真。
        ''' </summary>
        Public Property TranslationInhibition As Boolean

        ''' <summary>产生该命中的算法来源（"psRNATarget" / "TargetFinder"）</summary>
        Public Property Source As String

        Public Overrides Function ToString() As String
            Return $"{miRNA} -> {Target} [{StartSite},{EndSite}] score={Expectation:F2} mis={MismatchCount} wob={WobbleCount} gap={GapCount}"
        End Function

        ''' <summary>
        ''' 由 SW 比对结果（已以 miRNA 反向互补为正向 query）与 miRNA 正向序列
        ''' 计算逐位配对类型，并统计错配 / G:U / gap 计数，生成比对可视化串。
        ''' </summary>
        ''' <param name="mirna">miRNA 正向序列（用于 position 权重定位，非比对字符）</param>
        ''' <param name="hsp">最佳局部 HSP（seq1=miRNA rev-comp 片段, seq2=mRNA 片段）</param>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function FromHSP(mirna As String, hsp As Microsoft.VisualBasic.DataMining.DynamicProgramming.SmithWaterman.LocalHSPMatch(Of Char), source As String) As siRNAHit
            Dim s1 As Char() = hsp.seq1
            Dim s2 As Char() = hsp.seq2
            Dim n As Integer = Math.Min(s1.Length, s2.Length)

            Dim mis As Integer = 0
            Dim wob As Integer = 0
            Dim gap As Integer = 0

            ' 仅统计 gap：比对串中任一侧为 '-' 的即为凸起
            For i As Integer = 0 To n - 1
                Dim t As RNASeqHelper.PairType = RNASeqHelper.ClassifyPair(s1(i), s2(i))

                Select Case t
                    Case RNASeqHelper.PairType.Mismatch : mis += 1
                    Case RNASeqHelper.PairType.Wobble : wob += 1
                    Case RNASeqHelper.PairType.Gap : gap += 1
                End Select
            Next

            Dim hit As New siRNAHit With {
                .miRNA = "",
                .Target = "",
                .StartSite = hsp.fromB,
                .EndSite = hsp.toB,
                .Length = hsp.toB - hsp.fromB + 1,
                .MismatchCount = mis,
                .WobbleCount = wob,
                .GapCount = gap,
                .Alignment = hsp.ToString(),
                .Source = source
            }

            Return hit
        End Function
    End Class

    ''' <summary>
    ''' psRNATarget 靶标可及性（UPE）评估接口，默认关闭（返回 0）。
    ''' 如需基于 RNAup 计算解开能，可实现该接口后注入 <see cref="psRNATarget"/>。
    ''' </summary>
    Public Interface IAccessibilityEvaluator
        ''' <summary>
        ''' 计算靶位点上游 17 nt / 下游 13 nt 侧翼区域的解开能（kcal/mol）。
        ''' UPE 越低，靶位点越易被小RNA结合。
        ''' </summary>
        Function UPE(mrna As String, siteStart As Integer, siteEnd As Integer) As Double
    End Interface

    ''' <summary>默认关闭的 UPE 评估器：始终返回 0（不影响期望计算）。</summary>
    Public Class DisabledAccessibility : Implements IAccessibilityEvaluator
        Public Function UPE(mrna As String, siteStart As Integer, siteEnd As Integer) As Double Implements IAccessibilityEvaluator.UPE
            Return 0.0
        End Function
    End Class

    Public Interface miRNAMapper

        Function Run(miRNA As FastaSeq, db As IEnumerable(Of FastaSeq)) As IEnumerable(Of siRNAHit)

    End Interface
End Namespace
