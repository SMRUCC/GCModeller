' ============================================================================
' ProdigalModels.vb - Prodigal VB.NET 基因预测程序 数据模型定义
' 基于 Prodigal (PROkaryotic DYnamic Programming Gene-finding ALgorithm) 算法
' ============================================================================

''' <summary>
''' FASTA 格式序列记录
''' </summary>
Public Class FastaSequence
    ''' <summary>序列头部信息（>后的内容）</summary>
    Public Property Header As String

    ''' <summary>DNA序列（大写字母）</summary>
    Public Property Sequence As String

    ''' <summary>序列ID（Header的第一个单词）</summary>
    Public ReadOnly Property SeqId As String
        Get
            If String.IsNullOrEmpty(Header) Then Return "unknown"
            Dim parts = Header.Split({" "c, ControlChars.Tab}, 2)
            Return parts(0)
        End Get
    End Property

    ''' <summary>序列长度</summary>
    Public ReadOnly Property Length As Integer
        Get
            Return If(Sequence?.Length, 0)
        End Get
    End Property
End Class

''' <summary>
''' 候选ORF（开放阅读框）
''' </summary>
Public Class CandidateOrf
    ''' <summary>所属序列ID</summary>
    Public Property SeqId As String

    ''' <summary>起始位置（1-based，基因组坐标）</summary>
    Public Property Start As Integer

    ''' <summary>终止位置（1-based，基因组坐标）</summary>
    Public Property [End] As Integer

    ''' <summary>链方向：'+' 正向，'-' 反向</summary>
    Public Property Strand As Char

    ''' <summary>阅读框编号（0, 1, 2）</summary>
    Public Property Frame As Integer

    ''' <summary>起始密码子（ATG/GTG/TTG）</summary>
    Public Property StartCodon As String

    ''' <summary>终止密码子（TAA/TAG/TGA）</summary>
    Public Property StopCodon As String

    ''' <summary>ORF长度（核苷酸数）</summary>
    Public Property Length As Integer

    ''' <summary>编码区得分（coding score）</summary>
    Public Property CodingScore As Double

    ''' <summary>起始位点得分（start score）</summary>
    Public Property StartScore As Double

    ''' <summary>RBS得分</summary>
    Public Property RbsScore As Double

    ''' <summary>上游序列得分</summary>
    Public Property UpstreamScore As Double

    ''' <summary>起始密码子类型得分</summary>
    Public Property TypeScore As Double

    ''' <summary>总得分 = CodingScore + StartScore</summary>
    Public Property TotalScore As Double

    ''' <summary>氨基酸序列</summary>
    Public Property AaSequence As String

    ''' <summary>核苷酸序列</summary>
    Public Property NtSequence As String

    ''' <summary>检测到的RBS模体</summary>
    Public Property RbsMotif As String

    ''' <summary>RBS与起始密码子的间距</summary>
    Public Property RbsSpacing As Integer

    ''' <summary>是否被DP选中</summary>
    Public Property Selected As Boolean

    ''' <summary>在原始序列上的起始位置（0-based，用于ORF查找内部）</summary>
    Public Property RawStart As Integer

    ''' <summary>在原始序列上的终止位置（0-based，用于ORF查找内部）</summary>
    Public Property RawEnd As Integer

    ''' <summary>ORF在排序后的索引（DP用）</summary>
    Public Property SortIndex As Integer

    ''' <summary>DP前驱索引</summary>
    Public Property PrevIndex As Integer = -1

    ''' <summary>DP累积得分</summary>
    Public Property DpScore As Double = Double.MinValue
    ''' <summary>部分基因标记（5'端或3'端截断）</summary>
    Public Property PartialType As String = ""
End Class

''' <summary>
''' 预测基因（从DP选出的最终结果）
''' </summary>
Public Class PredictedGene
    Inherits CandidateOrf

    ''' <summary>置信度</summary>
    Public Property Confidence As Double

    ''' <summary>基因编号（在序列内的顺序号）</summary>
    Public Property GeneIndex As Integer
End Class

''' <summary>
''' 训练模型 - 包含编码区模型、RBS模型、起始密码子模型
''' </summary>
Public Class TrainingModel
    ''' <summary>编码区六聚体频率（4096个值，索引=HexamerToIndex）</summary>
    Public Property CodingHexamerCount As Double()

    ''' <summary>非编码区六聚体频率</summary>
    Public Property NoncodingHexamerCount As Double()

    ''' <summary>六聚体对数似然比得分</summary>
    Public Property HexamerScores As Double()

    ''' <summary>起始密码子频率</summary>
    Public Property StartCodonFreq As Dictionary(Of String, Double)

    ''' <summary>RBS位置权重矩阵（行=位置，列=A/C/G/T）</summary>
    Public Property RbsPwm As Double(,)

    ''' <summary>RBS模体列表</summary>
    Public Property RbsMotifs As List(Of String)

    ''' <summary>RBS模体得分表</summary>
    Public Property RbsMotifScores As Dictionary(Of String, Double)

    ''' <summary>GC含量</summary>
    Public Property GcContent As Double

    ''' <summary>平均基因长度</summary>
    Public Property AvgGeneLength As Double

    ''' <summary>模型是否已训练</summary>
    Public Property Trained As Boolean

    ''' <summary>训练迭代次数</summary>
    Public Property IterationCount As Integer

    ''' <summary>训练使用的基因数量</summary>
    Public Property TrainingGeneCount As Integer

    ''' <summary>编码区六聚体总数</summary>
    Public Property CodingHexamerTotal As Double

    ''' <summary>非编码区六聚体总数</summary>
    Public Property NoncodingHexamerTotal As Double

    ''' <summary>模型版本号</summary>
    Public Property Version As String = "1.0"

    Public Sub New()
        ReDim CodingHexamerCount(4095)
        ReDim NoncodingHexamerCount(4095)
        ReDim HexamerScores(4095)
        StartCodonFreq = New Dictionary(Of String, Double) From {
            {"ATG", 0.75}, {"GTG", 0.15}, {"TTG", 0.1}
        }
        RbsPwm = New Double(5, 3) {}
        RbsMotifs = New List(Of String) From {
            "AGGAGG", "AGGAG", "GGAGG", "AGGA", "GAGG", "GGAG", "GAG", "AGG", "GGG"
        }
        RbsMotifScores = New Dictionary(Of String, Double)
        GcContent = 0.5
        AvgGeneLength = 900
        Trained = False
        IterationCount = 0
        TrainingGeneCount = 0
        CodingHexamerTotal = 0
        NoncodingHexamerTotal = 0
    End Sub
End Class

''' <summary>
''' 基因预测结果
''' </summary>
Public Class PredictionResult
    ''' <summary>序列ID</summary>
    Public Property SeqId As String

    ''' <summary>序列长度</summary>
    Public Property SeqLength As Integer

    ''' <summary>预测的基因列表</summary>
    Public Property Genes As List(Of PredictedGene)

    ''' <summary>使用的训练模型</summary>
    Public Property Model As TrainingModel

    Public Sub New()
        Genes = New List(Of PredictedGene)()
    End Sub
End Class

''' <summary>
''' DP节点（用于动态规划选基因）
''' </summary>
Public Class DpNode
    ''' <summary>位置（基因组坐标）</summary>
    Public Property Position As Integer

    ''' <summary>到该位置的最优累积得分</summary>
    Public Property Score As Double

    ''' <summary>前驱节点索引</summary>
    Public Property PrevNode As Integer = -1

    ''' <summary>关联的ORF索引（-1表示非基因节点）</summary>
    Public Property OrfIndex As Integer = -1
End Class


