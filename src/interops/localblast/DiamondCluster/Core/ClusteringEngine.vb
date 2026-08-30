' ============================================================================
' ClusteringEngine.vb — 聚类引擎
' ----------------------------------------------------------------------------
''' 接收 DIAMOND 比对记录，应用阈值过滤后执行 Union-Find 合并操作。
'
' 算法：
'   对每条比对记录 (qid, sid, pident, qcov, scov)：
'     1. 验证阈值：pident ≥ min_identity AND qcov ≥ min_coverage AND scov ≥ min_coverage
'     2. 跳过自比对：qid ≠ sid
'     3. 执行 UnionFind.Union(qid, sid)
'
' 聚类结果：
'   - 通过传递性，A~B 且 B~C → A,B,C 同属一个聚类
'   - 每个聚类的根节点即为蛋白质家族 ID
' ============================================================================

Imports System

Namespace Core

    ''' <summary>
    ''' 聚类统计信息
    ''' </summary>
    Public Class ClusteringStats

        ''' <summary>处理的总比对数</summary>
        Public Property TotalAlignments As Long = 0

        ''' <summary>通过阈值的比对数</summary>
        Public Property PassedAlignments As Long = 0

        ''' <summary>跳过的自比对数</summary>
        Public Property SelfHitsSkipped As Long = 0

        ''' <summary>执行的 Union 操作数</summary>
        Public Property UnionsPerformed As Long = 0

        ''' <summary>处理的 chunk 数</summary>
        Public Property ChunksProcessed As Integer = 0

        Public Overrides Function ToString() As String
            Return $"比对: {PassedAlignments:N0}/{TotalAlignments:N0} | " &
                   $"Union: {UnionsPerformed:N0} | " &
                   $"Chunk: {ChunksProcessed}"
        End Function
    End Class

    ''' <summary>
    ''' 聚类引擎：将 DIAMOND 比对结果转化为 Union-Find 操作。
    ''' </summary>
    Public Class ClusteringEngine

        Private ReadOnly _dsu As UnionFind
        Private ReadOnly _minIdentity As Double
        Private ReadOnly _minCoverage As Double
        Private ReadOnly _stats As New ClusteringStats()

        ''' <summary>统计信息</summary>
        Public ReadOnly Property Stats As ClusteringStats
            Get
                Return _stats
            End Get
        End Property

        ''' <summary>
        ''' 构造函数
        ''' </summary>
        ''' <param name="dsu">并查集实例</param>
        ''' <param name="minIdentity">最小序列相似性百分比</param>
        ''' <param name="minCoverage">最小覆盖度百分比</param>
        Public Sub New(dsu As UnionFind, minIdentity As Double, minCoverage As Double)
            _dsu = dsu
            _minIdentity = minIdentity
            _minCoverage = minCoverage
        End Sub

        ''' <summary>
        ''' 处理一条比对记录
        ''' </summary>
        ''' <param name="record">比对记录</param>
        ''' <returns>True 表示触发了 Union 操作</returns>
        Public Function ProcessAlignment(record As AlignmentRecord) As Boolean
            _stats.TotalAlignments += 1

            ' 跳过自比对
            If record.QueryId = record.SubjectId Then
                _stats.SelfHitsSkipped += 1
                Return False
            End If

            ' 阈值过滤（DIAMOND 已预过滤，此处双重保险）
            If record.PercentIdentity < _minIdentity Then Return False
            If record.QueryCoverage < _minCoverage Then Return False
            If record.SubjectCoverage < _minCoverage Then Return False

            _stats.PassedAlignments += 1

            ' 执行 Union
            _dsu.Union(record.QueryId, record.SubjectId)
            _stats.UnionsPerformed += 1
            Return True
        End Function

        ''' <summary>
        ''' 批量处理比对记录（从解析器读取直到结束）
        ''' </summary>
        ''' <param name="parser">DIAMOND 结果解析器</param>
        ''' <returns>处理的记录数</returns>
        Public Function ProcessAll(parser As DiamondResultParser) As Long
            Dim count As Long = 0
            Dim record = parser.ReadNext()
            Do While record.HasValue
                ProcessAlignment(record.Value)
                count += 1
                record = parser.ReadNext()
            Loop
            Return count
        End Function

        ''' <summary>标记一个 chunk 处理完成</summary>
        Public Sub MarkChunkComplete()
            _stats.ChunksProcessed += 1
        End Sub

    End Class

End Namespace
