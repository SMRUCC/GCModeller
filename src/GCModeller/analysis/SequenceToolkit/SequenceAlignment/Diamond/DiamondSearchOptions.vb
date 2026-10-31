' DIAMOND 多查询搜索参数 (Search Options)
'
' 这些参数直接决定"速度 / 灵敏度 / 内存"的取舍,集中在此以便按机器规格调优,
' 同时让 <see cref="DiamondBlastp.Search"/> 的既有重载保持默认行为不变。

Namespace DIAMOND

    ''' <summary>
    ''' 多查询搜索参数。
    ''' </summary>
    Public Class DiamondSearchOptions

        ''' <summary>
        ''' 扫描参考库时的最小工作单元(残基数)。
        ''' 参考库被按此预算切成若干序列区间并行扫描;该值只影响并行粒度与
        ''' 缓存局部性,不影响比对结果。默认 100 万残基。
        ''' </summary>
        Public Property BlockResidues As Integer = 1_000_000

        ''' <summary>
        ''' 单个种子编码在查询索引中的链长上限(0 表示自动)。
        ''' 出现频率极高的编码(低复杂度区段 / 重复序列)会把命中数量放大若干个
        ''' 数量级,直接截断可避免在无信息量的种子上浪费时间。默认自动 = 48。
        ''' </summary>
        Public Property SeedFrequencyCap As Integer = 0

        ''' <summary>
        ''' 每条查询保留的候选目标上限(按无空位延伸得分截断,0 表示自动)。
        ''' 默认自动 = 4096。
        ''' </summary>
        Public Property MaxCandidatesPerQuery As Integer = 0

        ''' <summary>
        ''' 每条查询最多执行带状 SW 的候选数(0 表示按 maxHits 自动推导)。
        ''' 无空位得分已排序,超出该上限的弱候选不再做昂贵的动态规划。
        ''' </summary>
        Public Property MaxSwPerQuery As Integer = 0

        ''' <summary>
        ''' 进入候选集的最低无空位延伸得分(0 表示不设阈值)。
        ''' 默认 20,约相当于一个 20aa 的良好保守片段。
        ''' </summary>
        Public Property MinUngappedScore As Integer = 20

        ''' <summary>并行度(0 表示 Environment.ProcessorCount)。</summary>
        Public Property DegreeOfParallelism As Integer = 0

        ''' <summary>是否使用带内 SW 实现(False 回退到历史 GSW 全窗口实现)。</summary>
        Public Property EnableBandSw As Boolean = True

        ''' <summary>进度日志的查询/任务输出间隔(0 表示不输出)。</summary>
        Public Property ProgressLogInterval As Integer = 0

        ''' <summary>
        ''' 创建一份默认参数(每次返回新实例,避免共享可变状态)。
        ''' </summary>
        Public Shared Function CreateDefault() As DiamondSearchOptions
            Return New DiamondSearchOptions
        End Function

        ''' <summary>解析实际并行度。</summary>
        Public Function ResolveDegreeOfParallelism() As Integer
            If DegreeOfParallelism > 0 Then
                Return DegreeOfParallelism
            End If

            Return Math.Max(1, Environment.ProcessorCount)
        End Function

        ''' <summary>解析实际种子频率上限。</summary>
        Public Function ResolveSeedFrequencyCap() As Integer
            If SeedFrequencyCap > 0 Then
                Return SeedFrequencyCap
            End If

            Return 48
        End Function

        ''' <summary>解析实际候选目标上限。</summary>
        Public Function ResolveMaxCandidates() As Integer
            If MaxCandidatesPerQuery > 0 Then
                Return MaxCandidatesPerQuery
            End If

            Return 4096
        End Function

        ''' <summary>解析每条查询的 SW 调用上限。</summary>
        Public Function ResolveMaxSwPerQuery(maxHits As Integer) As Integer
            If MaxSwPerQuery > 0 Then
                Return MaxSwPerQuery
            End If

            If maxHits > 0 Then
                Return Math.Max(64, maxHits * 4)
            End If

            Return 128
        End Function

        Public Overrides Function ToString() As String
            Return $"block={BlockResidues}, seedCap={ResolveSeedFrequencyCap()}, maxCandidates={ResolveMaxCandidates()}, minUngapped={MinUngappedScore}, dop={ResolveDegreeOfParallelism()}"
        End Function
    End Class
End Namespace
