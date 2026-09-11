' /********************************************************************************/
'
'  Rockhopper —— 运行参数模型
'
'  原始实现把参数分散在 `CLI_API`（静态字段）、`Peregrine`（静态字段）、
'  `Assembler`（静态字段）与 `Parameters.java` 中。重写后统一为强类型参数对象，
'  默认值严格保持与原 CLI 一致（见 Java/Rockhopper/CLI_API.vb 的帮助文本）。
'
' /********************************************************************************/

Imports System.Collections.Generic

''' <summary>
''' Rockhopper 的全部可配置运行参数。
''' </summary>
Public Class RockhopperParameters

#Region "通用参数"

    ''' <summary>每个实验条件的读段文件列表（条件之间以空格分隔，条件内以逗号分隔，mate-pair 以 '%' 分隔）。</summary>
    Public Property ConditionFiles As New List(Of String)()
    ''' <summary>参考基因组目录列表（每个目录含 *.fna / *.ptt / *.rnt）；为空表示 de novo 模式。</summary>
    Public Property GenomeDirectories As List(Of String)
    ''' <summary>结果输出目录（默认 Rockhopper_Results/）。</summary>
    Public Property OutputDirectory As String = "Rockhopper_Results/"
    ''' <summary>基因组浏览器文件子目录。</summary>
    Public Property BrowserDirectory As String = "genomeBrowserFiles/"
    ''' <summary>是否计算差异表达（CLI -e，默认 True）。</summary>
    Public Property ComputeExpression As Boolean = True
    ''' <summary>是否预测操纵子（CLI -y，默认 True）。</summary>
    Public Property ComputeOperons As Boolean = True
    ''' <summary>是否识别转录边界（UTR / ncRNA，CLI -t，默认 True）。</summary>
    Public Property ComputeTranscripts As Boolean = True
    ''' <summary>单个命中即返回（CLI -a，默认 True）。</summary>
    Public Property StopAfterOneHit As Boolean = True
    ''' <summary>处理器数量（CLI -p，默认自动检测）。</summary>
    Public Property NumThreads As Integer = 0
    ''' <summary>RNA-seq 是否为链非特异（CLI -s，默认 False 即链特异）。</summary>
    Public Property Unstranded As Boolean = False
    ''' <summary>条件标签（CLI -L）。</summary>
    Public Property Labels As String()
    ''' <summary>verbose 输出（CLI -v，默认 False）。</summary>
    Public Property Verbose As Boolean = False
    ''' <summary>输出 SAM（CLI -SAM）。</summary>
    Public Property OutputSAM As Boolean = False
    ''' <summary>输出运行耗时（CLI -TIME）。</summary>
    Public Property Time As Boolean = False
    ''' <summary>是否 de novo 模式（无 -g 时为 True）。</summary>
    Public Property IsDeNovo As Boolean = False

#End Region

#Region "参考依赖比对参数"

    ''' <summary>允许的错配比例（CLI -m，默认 0.15）。</summary>
    Public Property PercentMismatches As Double = 0.15
    ''' <summary>最小种子长度占读长比例（CLI -l，默认 0.33）。</summary>
    Public Property PercentSeedLength As Double = 0.33
    ''' <summary>单端读段反向互补（CLI -c，默认 False）。</summary>
    Public Property SingleEndOrientationReverseComplement As Boolean = False
    ''' <summary>双端方向（CLI -ff/-fr/-rf/-rr，默认 fr）。</summary>
    Public Property PairedEndOrientation As String = "fr"
    ''' <summary>双端 mate 之间最大碱基数（CLI -d，默认 500）。</summary>
    Public Property MaxPairedEndLength As Integer = 500
    ''' <summary>UTR / ncRNA 的最小表达灵敏度 [0,1]（CLI -z，默认 0.5）。</summary>
    Public Property TranscriptSensitivity As Double = 0.5

#End Region

#Region "de novo 组装参数"

    ''' <summary>k-mer 长度，范围 15–31（CLI -k，默认 25）。</summary>
    Public Property K As Integer = 25
    ''' <summary>使用读段前所需的最小长度（CLI -j，默认 35）。</summary>
    Public Property MinReadLength As Integer = 35
    ''' <summary>k-mer 哈希表容量幂次 2^n（CLI -n，默认 25）。</summary>
    Public Property CapacityPower As Integer = 25
    ''' <summary>映射到 de novo 转录本所需的最少全长读段数（CLI -b，默认 20）。</summary>
    Public Property MinReadsMapping As Integer = 20
    ''' <summary>de novo 转录本最小长度（CLI -u，默认 2*k；0 表示取 2*k）。</summary>
    Public Property MinTranscriptLength As Integer = 0
    ''' <summary>作为转录本起点的 k-mer 最小计数（CLI -w，默认 50）。</summary>
    Public Property MinSeedExpression As Integer = 50
    ''' <summary>延伸转录本所需的 k-mer 最小计数（CLI -x，默认 5）。</summary>
    Public Property MinExpression As Integer = 5

#End Region

#Region "输出文件名"

    Public Property SummaryFile As String = "summary.txt"
    Public Property ExpressionFile As String = "transcripts.txt"
    Public Property OperonGenePairFile As String = "operonGenePairs.txt"
    Public Property OperonMergedFile As String = "operons.txt"

#End Region

    ''' <summary>
    ''' 规范化：补齐默认值、解析标签、确定线程数、判定 de novo 模式。
    ''' </summary>
    Public Function Normalize() As RockhopperParameters
        If String.IsNullOrEmpty(OutputDirectory) Then OutputDirectory = "Rockhopper_Results/"
        If Not OutputDirectory.EndsWith("/") AndAlso Not OutputDirectory.EndsWith("\") Then
            OutputDirectory += "/"
        End If

        IsDeNovo = GenomeDirectories Is Nothing OrElse GenomeDirectories.Count = 0

        If NumThreads <= 0 Then
            NumThreads = Environment.ProcessorCount
        End If

        ' 参考依赖阶段 >4 核时按原逻辑收缩（Peregrine/Assembler 原有行为）
        If NumThreads > 4 Then
            NumThreads = CInt(System.Math.Truncate(System.Math.Min(NumThreads * 0.75, 8.0)))
        End If

        If MinTranscriptLength <= 0 Then MinTranscriptLength = 2 * K

        Return Me
    End Function

End Class
