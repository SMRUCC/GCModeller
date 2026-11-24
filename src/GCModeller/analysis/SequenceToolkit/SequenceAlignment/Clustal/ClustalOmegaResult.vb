Imports SMRUCC.genomics.Analysis.SequenceAlignment.MSA

Namespace Clustal

    ''' <summary>
    ''' 比对结果的输出格式
    ''' </summary>
    Public Enum MSAFormat
        ''' <summary>
        ''' FASTA(已比对)
        ''' </summary>
        Fasta = 0
        ''' <summary>
        ''' Clustal 格式(.aln)
        ''' </summary>
        Clustal = 1
        ''' <summary>
        ''' Stockholm 格式(可被 <c>MSA.Tabular.Reader</c> 回读)
        ''' </summary>
        Stockholm = 2
        ''' <summary>
        ''' PHYLIP 顺序格式
        ''' </summary>
        Phylip = 3
    End Enum

    ''' <summary>
    ''' Clustal Omega 流水线的输出结果。
    ''' </summary>
    ''' <remarks>
    ''' 继承 <see cref="MSAOutput"/> 以复用既有的结果容器语义：
    ''' <c>names</c>/<c>MSA</c>/<c>edits</c>/<c>cost</c>，其中 <c>cost</c> 在这里等于 SP 得分。
    ''' </remarks>
    Public Class ClustalOmegaResult
        Inherits MSAOutput

        ''' <summary>
        ''' 引导树
        ''' </summary>
        Public Property GuideTree As GuideTree
        ''' <summary>
        ''' 完整的距离矩阵(仅在 <see cref="ClustalOmegaOptions.FullDistanceMatrix"/> 打开时有效)
        ''' </summary>
        Public Property DistanceMatrix As Double()()
        ''' <summary>
        ''' 比对统计信息(SP 得分、一致序列、保守列标记)
        ''' </summary>
        Public Property Statistics As AlignmentStatistics
        ''' <summary>
        ''' 各阶段的耗时(毫秒)
        ''' </summary>
        Public Property StageTimings As Dictionary(Of String, Double)
        ''' <summary>
        ''' 每个簇包含的序列条数
        ''' </summary>
        Public Property ClusterSizes As Integer()
        ''' <summary>
        ''' 本次运行实际使用的参数
        ''' </summary>
        Public Property Options As ClustalOmegaOptions
        ''' <summary>
        ''' 实际使用的序列类型
        ''' </summary>
        Public Property SequenceTypeUsed As SequenceType
        ''' <summary>
        ''' 实际使用的 k-mer 长度
        ''' </summary>
        Public Property KmerSizeUsed As Integer
        ''' <summary>
        ''' 执行的 profile-profile 比对次数
        ''' </summary>
        Public Property MergeCount As Integer
        ''' <summary>
        ''' 参与运算的 DP 单元总数
        ''' </summary>
        Public Property ProfileCells As Long
        ''' <summary>
        ''' 因为内存上限而降级为 Viterbi 解码的次数
        ''' </summary>
        Public Property MacFallbacks As Integer
        ''' <summary>
        ''' 归一化之后的原始序列(去空位)
        ''' </summary>
        Public Property Sequences As String()

        ''' <summary>
        ''' 输出引导树(Newick 格式)
        ''' </summary>
        Public Function TreeNewick() As String
            If GuideTree Is Nothing Then
                Return ";"
            Else
                Return GuideTree.ToNewick(names)
            End If
        End Function

        ''' <summary>
        ''' 按指定格式输出比对结果
        ''' </summary>
        Public Function Write(format As MSAFormat) As String
            Select Case format
                Case MSAFormat.Clustal
                    Return MSAIO.Clustal(Me)
                Case MSAFormat.Stockholm
                    Return MSAIO.Stockholm(Me)
                Case MSAFormat.Phylip
                    Return MSAIO.Phylip(Me)
                Case Else
                    Return MSAIO.Fasta(Me)
            End Select
        End Function

        ''' <summary>
        ''' 按指定格式保存比对结果
        ''' </summary>
        Public Function SaveAs(format As MSAFormat, path As String) As Boolean
            Return MSAIO.Save(Me, format, path)
        End Function

        Public Overrides Function ToString() As String
            Dim text As String = MyBase.ToString()

            If Statistics IsNot Nothing Then
                text &= vbCrLf & $"> {Statistics}"
            End If

            Return text
        End Function
    End Class
End Namespace
