Imports SMRUCC.genomics.ComponentModel.Loci

Namespace Models

    ''' <summary>
    ''' TSS 输出的评分（同时也是聚类时选择代表位点）的模式，
    ''' 对应 TSSAR 的 ``--score`` 选项。
    ''' </summary>
    Public Enum ScoreModes As Integer

        ''' <summary>
        ''' 使用 p 值作为评分（``--score p``）：p 值越小越显著。
        ''' </summary>
        PValue = 0

        ''' <summary>
        ''' 使用 [+] 与 [-] 文库的峰值差作为评分（``--score d``，默认）。
        ''' </summary>
        PeakDifference = 1
    End Enum

    ''' <summary>
    ''' TSSAR 的运行参数。
    ''' </summary>
    Public Class TssarOptions

        ''' <summary>
        ''' [+] 文库（经 TEX 处理、富集了 TSS）的 SAM 比对文件。
        ''' </summary>
        Public Property PlusSam As String

        ''' <summary>
        ''' [-] 文库（未经处理）的 SAM 比对文件。
        ''' </summary>
        Public Property MinusSam As String

        ''' <summary>
        ''' 基因组长度；当为 0 时必须提供 <see cref="Fasta"/>。
        ''' </summary>
        Public Property GenomeSize As Integer = 0

        ''' <summary>
        ''' 参考基因组 fasta 文件路径（仅用于解析基因组长度与序列名称）。
        ''' </summary>
        Public Property Fasta As String

        ''' <summary>
        ''' 输出 BED 文件之中使用的参考序列名称；缺省为 ``chr``。
        ''' </summary>
        Public Property Chromosome As String = "chr"

        ''' <summary>
        ''' 滑动窗口大小（nt），缺省 1000。
        ''' </summary>
        Public Property WindowSize As Integer = 1000

        ''' <summary>
        ''' 噪声阈值：只有 [+] 文库原始 read 起始数不低于该值的位置才会被考虑为 TSS，缺省 3。
        ''' </summary>
        Public Property MinPeakSize As Integer = 3

        ''' <summary>
        ''' p 值阈值（判定为 TSS 的最大 p 值），缺省 ``1e-4``。
        ''' </summary>
        Public Property PValueCutoff As Double = 0.0001

        ''' <summary>
        ''' 评分/聚类模式，缺省为峰值差。
        ''' </summary>
        Public Property ScoreMode As ScoreModes = ScoreModes.PeakDifference

        ''' <summary>
        ''' 是否对连续的 TSS 位置执行聚类（缺省 ``True``）。
        ''' </summary>
        Public Property Clustering As Boolean = True

        ''' <summary>
        ''' 聚类时两个显著位置被视为"连续"的最大间距（nt），缺省 3。
        ''' </summary>
        Public Property ClusterRange As Integer = 3

        ''' <summary>
        ''' 多重检验校正方法（``fdr``/``BH``/``bonferroni``/``holm``/``hochberg``/``hommel``/``BY``/``none``）；
        ''' 为空字符串时不执行校正。
        ''' </summary>
        Public Property MultipleTesting As String = ""

        ''' <summary>
        ''' 是否按 SAM 的 ``NH`` 标签对多重比对的 read 进行按比例计数（``1/n``）。
        ''' </summary>
        Public Property Prorata As Boolean = False

        ''' <summary>
        ''' 结构零随机剔除所使用的随机数种子（保证结果可复现）。
        ''' </summary>
        Public Property Seed As Integer = 12345

        ''' <summary>
        ''' 基因注释（PTT）文件路径；提供后将对 TSS 执行 Primary/Internal/Antisense/Orphan 分类。
        ''' </summary>
        Public Property Ptt As String

        ''' <summary>
        ''' 判定为 Primary TSS 的最大上游距离（nt），缺省 250。
        ''' </summary>
        Public Property PrimaryUpstream As Integer = 250

        ''' <summary>
        ''' 判定为反义下游（Ad）的最大下游距离（nt），缺省 30。
        ''' </summary>
        Public Property AntisenseDownstream As Integer = 30

        ''' <summary>
        ''' 是否输出进度信息。
        ''' </summary>
        Public Property Verbose As Boolean = True
    End Class

    ''' <summary>
    ''' TSSAR 的运行结果。
    ''' </summary>
    Public Class TssarResult

        ''' <summary>
        ''' 参考序列名称。
        ''' </summary>
        Public Property Chromosome As String

        ''' <summary>
        ''' 基因组长度。
        ''' </summary>
        Public Property GenomeSize As Integer

        ''' <summary>
        ''' 全部被注释为 TSS 的位置（可能已经过聚类）。
        ''' </summary>
        Public Property Tss As TssSite()

        ''' <summary>
        ''' 无法用零膨胀 Poisson 回归建模、因而被排除出分析的基因组区间。
        ''' </summary>
        Public Property UnmodeledRegions As DumpRegion()

        ''' <summary>
        ''' 聚类之前的单个 TSS 数量。
        ''' </summary>
        Public Property TotalIndividualTss As Integer

        ''' <summary>
        ''' 聚类之后的 TSS 数量。
        ''' </summary>
        Public Property TotalClusteredTss As Integer

        ''' <summary>
        ''' 未建模区域的总长度（nt）。
        ''' </summary>
        Public Property UnmodeledLength As Long

        ''' <summary>
        ''' 未建模区域占基因组总长度的比例。
        ''' </summary>
        Public ReadOnly Property UnmodeledRate As Double
            Get
                If GenomeSize <= 0 Then Return 0
                Return UnmodeledLength / GenomeSize
            End Get
        End Property

        ''' <summary>
        ''' Primary TSS 的 5'UTR 长度分布（仅当提供了基因注释时可用）。
        ''' </summary>
        Public Property PrimaryUtrLengths As Integer()

        ''' <summary>
        ''' 所有 TSS 的 TEX 处理效率平均值。
        ''' </summary>
        Public Property MeanTexEfficiency As Double
    End Class
End Namespace
