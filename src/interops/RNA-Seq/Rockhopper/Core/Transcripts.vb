' /********************************************************************************/
'
'  Rockhopper —— 转录本模型
'
'  复刻自原始 Rockhopper `Java/ObjectModels/Transcripts.vb` 的核心字段与语义：
'  一个转录本记录其转录起始/终止位点(TSS/TTS)、翻译起始/终止位点(ATG/TGA)、
'  链方向、表达量以及（差异表达时的）q 值。
'
'  边界推断算法本身见 `Transcription/TSSsInference.vb`（对应论文的贝叶斯/覆盖度判据）。
'  原始实现中的 `AnalysisAPI.Transcripts`（CSV 视图模型）保留在 API 层，
'  两者职责不同：本类为分析内核使用的强类型模型。
'
' /********************************************************************************/

Imports System.Collections.Generic
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace Core

    ''' <summary>
    ''' 转录本（基因或预测得到的新转录本/RNA）。
    ''' </summary>
    Public Class Transcript

        ''' <summary>基因名。</summary>
        Public Property Name As String
        ''' <summary>基因号 / locus_tag（预测得到的新 RNA 使用 "predicted RNA"）。</summary>
        Public Property Synonym As String
        ''' <summary>产物描述。</summary>
        Public Property Product As String
        ''' <summary>所在链：'+' / '-'。</summary>
        Public Property Strand As Char = "+"c

        ''' <summary>转录起始位点（TSS）。</summary>
        Public Property TSSs As Integer
        ''' <summary>翻译起始位点（ATG）。</summary>
        Public Property ATG As Integer
        ''' <summary>翻译终止位点（TGA）。</summary>
        Public Property TGA As Integer
        ''' <summary>转录终止位点（TTS）。</summary>
        Public Property TTSs As Integer

        ''' <summary>表达量（平均归一化表达）。</summary>
        Public Property Expression As Double
        ''' <summary>差异表达 q 值（未做差异分析时为 1.0）。</summary>
        Public Property QValue As Double = 1.0

        ''' <summary>是否为预测得到的新转录本（而非已有注释）。</summary>
        Public Property IsPredicted As Boolean

        ''' <summary>复制子名称（多 replicon 时用于定位）。</summary>
        Public Property Replicon As String

        ''' <summary>是否蛋白编码（有 ATG/TGA 即为 ORF）。</summary>
        Public ReadOnly Property IsORF As Boolean
            Get
                Return ATG > 0 AndAlso TGA > 0
            End Get
        End Property

        ''' <summary>是否为 RNA（没有翻译起止位点）。</summary>
        Public ReadOnly Property IsRNA As Boolean
            Get
                Return ATG <= 0
            End Get
        End Property

        ''' <summary>TSS 与 ATG 完全重合（leaderless 转录本）。</summary>
        Public ReadOnly Property Leaderless As Boolean
            Get
                Return TSSs > 0 AndAlso ATG > 0 AndAlso TSSs = ATG
            End Get
        End Property

        ''' <summary>-35 区（TSS 上游约 60 bp 处）。</summary>
        Public ReadOnly Property Minus35BoxLoci As Integer
            Get
                If TSSs <= 0 Then Return -1
                If GetLociStrand() = Strands.Forward Then
                    Return TSSs - 60
                Else
                    Return TSSs + 60
                End If
            End Get
        End Property

        ''' <summary>转录单元长度（TSS→TTS）。</summary>
        Public ReadOnly Property TranscriptLength As Integer
            Get
                Return GetTULoci().FragmentSize
            End Get
        End Property

        ''' <summary>长度小于 150 nt 的 RNA 视为小 RNA。</summary>
        Public ReadOnly Property IsSRNA As Boolean
            Get
                If ATG > 0 Then Return False
                Return TranscriptLength <= 150
            End Get
        End Property

        ''' <summary>链方向枚举。</summary>
        Public Function GetLociStrand() As Strands
            Select Case Strand
                Case "-"c : Return Strands.Reverse
                Case "+"c : Return Strands.Forward
                Case Else : Return Strands.Unknown
            End Select
        End Function

        ''' <summary>
        ''' 转录单元（TSS..TTS）的基因组位点；缺失的位点回退到翻译坐标。
        ''' </summary>
        Public Function GetTULoci() As NucleotideLocation
            Dim start As Integer = If(TSSs > 0, TSSs, ATG)
            Dim [stop] As Integer = If(TTSs > 0, TTSs, If(TGA > 0, TGA, start))
            If start <= 0 Then start = 1
            If [stop] <= 0 Then [stop] = start
            Return New NucleotideLocation(start, [stop], GetLociStrand())
        End Function

        ''' <summary>翻译区（ATG..TGA）的基因组位点。</summary>
        Public Function GetORFLoci() As NucleotideLocation
            If Not IsORF Then Return GetTULoci()
            Return New NucleotideLocation(System.Math.Min(ATG, TGA), System.Math.Max(ATG, TGA), GetLociStrand())
        End Function

        Public Overrides Function ToString() As String
            Return Synonym
        End Function

    End Class

    ''' <summary>
    ''' 转录本集合辅助方法。
    ''' </summary>
    Public Module TranscriptExtensions

        ''' <summary>按基因号建立索引（重复时保留首个）。</summary>
        Public Function IndexBySynonym(transcripts As IEnumerable(Of Transcript)) As Dictionary(Of String, Transcript)
            Dim map As New Dictionary(Of String, Transcript)()
            For Each t As Transcript In transcripts
                If String.IsNullOrEmpty(t.Synonym) Then Continue For
                If Not map.ContainsKey(t.Synonym) Then map.Add(t.Synonym, t)
            Next
            Return map
        End Function

    End Module

End Namespace
