' /********************************************************************************/
'
'  Rockhopper —— API 数据模型（AnalysisAPI）
'
'  由原始 `API/DataModels.vb` 迁移而来：
'    * CSV 特性从 `Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection`
'      改为 `Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection`；
'    * `LANS.SystemsBiology.*`（NucleotideLocation / SegmentReader / FastaSeq）改为
'      `SMRUCC.genomics.*`；
'    * 原 `SegmentReader.TryParse(loci).SequenceData` 在新框架已无对应类型，
'      改为直接对 <see cref="FastaSeq"/> 的序列做 0-based 切片（见 <see cref="GetSegment"/>）。
'
'  TSS 六分类（MTU 最小转录单元模型）的规范说明完整保留在 Categories 枚举中。
'
' /********************************************************************************/

Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace AnalysisAPI

    ''' <summary>
    ''' 预测得到的一个操纵子。
    ''' </summary>
    Public Class Operon

        Public Property Start As Long
        Public Property [Stop] As Long
        Public Property Strand As String

        Public ReadOnly Property NumberOfGenes As Integer
            Get
                Return If(Genes Is Nothing, 0, Genes.Length)
            End Get
        End Property

        <Collection("Genes", ", ")> Public Property Genes As String()

        Public Overrides Function ToString() As String
            Return $"[{Strand}]{Start},{[Stop]};    {String.Join(", ", If(Genes, New String() {}))}"
        End Function

    End Class

    ''' <summary>
    ''' 从 RNA-seq 数据之中分析出来的基因/转录本结构。
    ''' </summary>
    Public Class Transcripts

        ''' <summary>Transcription Start Sites（转录起始位点）。</summary>
        <Column("Transcription Start")> Public Property TSSs As Long
        ''' <summary>Translation Start（翻译起始位点）。</summary>
        <Column("Translation Start")> Public Property ATG As Long
        ''' <summary>Translation Stop（翻译终止位点）。</summary>
        <Column("Translation Stop")> Public Property TGA As Long
        ''' <summary>Transcription Stop Sites（转录终止位点）。</summary>
        <Column("Transcription Stop")> Public Property TTSs As Long

        ''' <summary>所在链："+" / "-"。</summary>
        Public Property Strand As String
        Public Property Name As String
        ''' <summary>基因号 / locus_tag。</summary>
        Public Property Synonym As String
        Public Property Product As String

        ''' <summary>表达量（平均归一化表达）。</summary>
        Public Property Expression As Long

        ''' <summary>差异表达 q 值（未做差异分析时为 1.0）。</summary>
        Public Property QValue As Double = 1.0

        ''' <summary>ATG 为 0 即不是 ORF。</summary>
        Public ReadOnly Property IsRNA As Boolean
            Get
                Return ATG <= 0
            End Get
        End Property

        ''' <summary>TSS 与 ATG 绝对重合（lmTSS）。</summary>
        Public ReadOnly Property Leaderless As Boolean
            Get
                Return TSSs > 0 AndAlso ATG > 0 AndAlso TSSs = ATG
            End Get
        End Property

        ''' <summary>-35 区（TSS 上游约 60 bp）。</summary>
        Public ReadOnly Property Minus35BoxLoci As Integer
            Get
                If TSSs <= 0 Then Return -1
                If GetLociStrand() = Strands.Forward Then Return TSSs - 60
                Return TSSs + 60
            End Get
        End Property

        Public ReadOnly Property IsPredictedRNA As Boolean
            Get
                Return String.IsNullOrEmpty(Synonym) OrElse String.Equals("predicted RNA", Synonym)
            End Get
        End Property

        Public ReadOnly Property TranscriptLength As Integer
            Get
                Return GetTULoci().FragmentSize
            End Get
        End Property

        ''' <summary>小 RNA 阈值（长度 ≤ 150 nt 且非 ORF）。</summary>
        Const SRNA_LEN As Integer = 150

        Public ReadOnly Property Is_sRNA As Boolean
            Get
                If ATG > 0 Then Return False
                Return TranscriptLength <= SRNA_LEN
            End Get
        End Property

        Public Function GetLociStrand() As Strands
            Select Case Strand
                Case "-" : Return Strands.Reverse
                Case "+" : Return Strands.Forward
                Case Else : Return Strands.Unknown
            End Select
        End Function

        ''' <summary>转录单元（TSS..TTS）的基因组位点。</summary>
        Public Function GetTULoci() As NucleotideLocation
            Dim start As Integer = CInt(If(TSSs > 0, TSSs, ATG))
            Dim [stop] As Integer = CInt(If(TTSs > 0, TTSs, If(TGA > 0, TGA, start)))
            If start <= 0 Then start = 1
            If [stop] <= 0 Then [stop] = start
            Return New NucleotideLocation(start, [stop], GetLociStrand())
        End Function

        ''' <summary>ORF（ATG..TGA）的基因组位点。</summary>
        Public Function GetORFLoci() As NucleotideLocation
            If ATG <= 0 OrElse TGA <= 0 Then Return GetTULoci()
            Return New NucleotideLocation(CInt(System.Math.Min(ATG, TGA)), CInt(System.Math.Max(ATG, TGA)), GetLociStrand())
        End Function

#Region "Sequence helpers"

        ''' <summary>
        ''' 提取 5' UTR（TSS → ATG）序列。
        ''' </summary>
        ''' <param name="Genome">基因组 FASTA 序列（大写的 0-based 序列）。</param>
        Public Function Get5UTRLeader(Genome As FastaSeq) As FastaSeq
            If TSSs <= 0 OrElse Leaderless OrElse ATG <= 0 Then Return Nothing
            Dim loci As NucleotideLocation = If(GetLociStrand() = Strands.Forward,
                                                New NucleotideLocation(CInt(TSSs), CInt(ATG)),
                                                New NucleotideLocation(CInt(ATG), CInt(TSSs), True))
            Return makeFasta(Genome, loci, {"5UTR", "TSSs=" & TSSs, "ATG=" & ATG})
        End Function

        ''' <summary>提取 TSS 上下游各 5 bp 的序列。</summary>
        Public Function GetTSSLoci(Genome As FastaSeq) As FastaSeq
            If TSSs <= 0 Then Return Nothing
            Dim loci As New NucleotideLocation(CInt(TSSs) - 5, CInt(TSSs) + 5, GetLociStrand() = Strands.Reverse)
            Return makeFasta(Genome, loci, {"TSS", "TSSs=" & TSSs})
        End Function

        ''' <summary>提取 3' UTR（TGA → TTS）序列。</summary>
        Public Function GetTTSsLoci(Genome As FastaSeq) As FastaSeq
            If TTSs <= 0 OrElse TGA <= 0 Then Return Nothing
            Dim loci As NucleotideLocation = If(GetLociStrand() = Strands.Forward,
                                                New NucleotideLocation(CInt(TGA), CInt(TTSs)),
                                                New NucleotideLocation(CInt(TTSs), CInt(TGA), True))
            Return makeFasta(Genome, loci, {"3UTR", "TGA=" & TGA, "TTS=" & TTSs})
        End Function

        ''' <summary>提取 -35 区到 TSS 的启动子序列。</summary>
        Public Function GetPromoterBoxLoci(Genome As FastaSeq) As FastaSeq
            If TSSs <= 0 Then Return Nothing
            Dim loci As NucleotideLocation = If(GetLociStrand() = Strands.Forward,
                                                New NucleotideLocation(Minus35BoxLoci, CInt(TSSs), False),
                                                New NucleotideLocation(CInt(TSSs), Minus35BoxLoci, True))
            Return makeFasta(Genome, loci, {"promoter", "TSSs=" & TSSs, "-35=" & Minus35BoxLoci})
        End Function

        ''' <summary>
        ''' 按基因组位点切片（1-based 坐标，含两端）。
        ''' </summary>
        Private Function makeFasta(Genome As FastaSeq, loci As NucleotideLocation, headers As String()) As FastaSeq
            Dim sequence As String = GetSegment(Genome, loci)
            If String.IsNullOrEmpty(sequence) Then Return Nothing

            Return New FastaSeq With {
                .SequenceData = sequence,
                .Headers = New String() {Synonym}.Join(headers)
            }
        End Function

        ''' <summary>
        ''' 从 0-based 序列中按下标区间取片段（1-based 输入，含两端；负链取反向互补）。
        ''' </summary>
        Public Shared Function GetSegment(Genome As FastaSeq, loci As NucleotideLocation) As String
            If Genome Is Nothing OrElse String.IsNullOrEmpty(Genome.SequenceData) Then Return Nothing

            Dim sequence As String = Genome.SequenceData
            Dim left As Integer = System.Math.Max(1, System.Math.Min(loci.Min, loci.Max))
            Dim right As Integer = System.Math.Min(sequence.Length, System.Math.Max(loci.Min, loci.Max))
            If right < left Then Return Nothing

            Dim segment As String = sequence.Substring(left - 1, right - left + 1).ToUpperInvariant()
            If loci.Strand = Strands.Reverse Then segment = ReverseComplement(segment)
            Return segment
        End Function

        ''' <summary>反向互补。</summary>
        Public Shared Function ReverseComplement(sequence As String) As String
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

#End Region

        ''' <summary>
        ''' Figure 1 Identification of transcription start sites (TSS) in the S. meliloti genome:
        ''' 依据最小转录单元(MTU)模型将 TSS 分为六类。
        '''
        ''' (i)   TSS Of an mRNA (mTSS)：ATG 上游 54 nt 区域内（本实现取 0–300 bp）
        '''       覆盖的显著 5' 端；54 nt 是能包含启动子 motif 与核糖体结合位点的最小长度。
        ''' (ii)  TSS Of a leaderless transcript (lmTSS)：与翻译起始密码子第一位重叠。
        ''' (iii) TSS Of a putative mRNA (pmTSS)：难以区分是长 5'UTR 的 mRNA 还是 trans-encoded sRNA。
        ''' (iv)  TSS of a sense transcript (seTSS)：位于蛋白编码基因内部且与其同向。
        ''' (v)   TSS of a cis-encoded antisense RNA (asTSS)：与蛋白编码目标基因反向。
        ''' (vi)  TSS of a trans-encoded sRNA (sTSS)：位于基因间区且与相邻基因有确定距离。
        ''' </summary>
        Public Enum Categories As Integer

            ''' <summary>无法进行 TSS 的分类定义。</summary>
            UnClassified = -100
            ''' <summary>(i) TSS Of an mRNA.</summary>
            mTSS = 1
            ''' <summary>(ii) TSS Of a leaderless transcript.</summary>
            lmTSS = 2
            ''' <summary>Ultra Long mRNA TSSs（TSS 距 ATG 300–500 bp）。</summary>
            ULmTSS = 3
            ''' <summary>(iii) TSS Of a putative mRNA.</summary>
            pmTSS = 4
            ''' <summary>(v) TSS of a cis-encoded antisense RNA.</summary>
            asTSS = 5
            ''' <summary>(iv) TSS of a sense transcript.</summary>
            seTSS = 6
            ''' <summary>(vi) TSS of a trans-encoded sRNA.</summary>
            sTSS = 7

        End Enum

        Public Overrides Function ToString() As String
            Return Synonym
        End Function

    End Class

End Namespace
