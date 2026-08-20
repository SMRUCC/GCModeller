' Linclust 聚类结果模型与配置项
Namespace Linclust

    ''' <summary>
    ''' Linclust 运行配置
    ''' </summary>
    Public Class LinclustOptions
        ''' <summary>每序列保留的最小哈希 k-mer 个数(默认 20)</summary>
        Public Property m As Integer = 20
        ''' <summary>一致性(identity)阈值,0-1。>=0.9 时 k_seqid=14,否则 10</summary>
        Public Property seqidThreshold As Double = 0.9
        ''' <summary>覆盖率(coverage)阈值,0-1</summary>
        Public Property coverage As Double = 0.8
        ''' <summary>E-value 阈值(预留,当前判据以一致性+覆盖率为主)</summary>
        Public Property evalue As Double = 0.001
        ''' <summary>阶段三快速过滤的一致性阈值(通常等于 seqidThreshold)</summary>
        Public Property fastFilterSeqid As Double = 0.9
        ''' <summary>阶段三快速过滤的覆盖率阈值(通常等于 coverage)</summary>
        Public Property fastFilterCoverage As Double = 0.8
        ''' <summary>缩减字母表有效大小 A_eff(用于 k 长度自动选择)</summary>
        Public Property Aeff As Double = 8.7
    End Class

    ''' <summary>
    ''' 单个聚类簇
    ''' </summary>
    Public Class Cluster
        ''' <summary>代表序列 ID(簇中最长成员)</summary>
        Public Property representative As Integer
        ''' <summary>簇内全部成员序列 ID(含代表)</summary>
        Public Property members As List(Of Integer)

        Public Overrides Function ToString() As String
            Return $"repr={representative}, size={If(members Is Nothing, 0, members.Count)}"
        End Function
    End Class

    ''' <summary>
    ''' 聚类结果
    ''' </summary>
    Public Class ClusterResult
        ''' <summary>所有簇</summary>
        Public Property clusters As List(Of Cluster)
        ''' <summary>实际使用的 k-mer 长度</summary>
        Public Property k As Integer
        ''' <summary>序列总数</summary>
        Public Property nSeq As Integer
        ''' <summary>簇数</summary>
        Public ReadOnly Property nClusters As Integer
            Get
                Return If(clusters Is Nothing, 0, clusters.Count)
            End Get
        End Property
    End Class
End Namespace
