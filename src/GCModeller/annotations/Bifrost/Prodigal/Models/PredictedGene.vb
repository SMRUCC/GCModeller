' ============================================================================
' ProdigalModels.vb - Prodigal VB.NET 基因预测程序 数据模型定义
' 基于 Prodigal (PROkaryotic DYnamic Programming Gene-finding ALgorithm) 算法
' ============================================================================

Imports SMRUCC.genomics.SequenceModel.FASTA

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

    Public Function CreateProteinFasta(seq_id As String) As FastaSeq
        Dim title As String = $"{seq_id}_{GeneIndex} # {Start} # {[End]} # {Strand} # ID=gene_{GeneIndex};partial={PartialType}"
        ' 每行60个氨基酸
        Dim aa = AaSequence

        Return New FastaSeq(aa, title:=title)
    End Function

    Public Function CreateGeneFasta(seq_id As String) As FastaSeq
        Dim title As String = $"{seq_id}_{GeneIndex} # {Start} # {[End]} # {Strand} # ID=gene_{GeneIndex};partial={PartialType}"
        ' 每行60个氨基酸
        Dim aa = NtSequence

        Return New FastaSeq(aa, title:=title)
    End Function

End Class







