' 搜索过程的阶段耗时画像 (Search Profile)
'
' 多查询比对的时间分布高度依赖数据与机器:查询索引构建、参考库扫描、
' 局部比对三段的比例差别很大。把三段分别计时暴露出来,便于:
'   1) 判断瓶颈落在哪一段(而不是只看总耗时);
'   2) 在基准测试中量化优化前后的收益;
'   3) 反推 DiamondSearchOptions 的调优方向(块大小 / 候选上限等)。

Namespace DIAMOND

    ''' <summary>
    ''' 一次多查询搜索的阶段耗时与规模画像(只读快照)。
    ''' </summary>
    Public Class DiamondSearchProfile

        ''' <summary>查询序列条数。</summary>
        Public Property Queries As Integer

        ''' <summary>参考库序列条数。</summary>
        Public Property Subjects As Integer

        ''' <summary>参考库残基总数。</summary>
        Public Property Residues As Long

        ''' <summary>查询侧种子索引构建耗时(毫秒)。</summary>
        Public Property IndexMilliseconds As Double

        ''' <summary>参考库扫描(种子连接 + Hamming + 无空位延伸)耗时(毫秒)。</summary>
        Public Property ScanMilliseconds As Double

        ''' <summary>局部比对与结果聚合耗时(毫秒)。</summary>
        Public Property AlignMilliseconds As Double

        ''' <summary>通过过滤链进入候选集的 (查询, 参考) 对数。</summary>
        Public Property Candidates As Long

        ''' <summary>最终产出的命中行数。</summary>
        Public Property Hits As Long

        Public ReadOnly Property TotalMilliseconds As Double
            Get
                Return IndexMilliseconds + ScanMilliseconds + AlignMilliseconds
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"queries={Queries}, subjects={Subjects}, residues={Residues}, " &
                   $"index={IndexMilliseconds:F0}ms, scan={ScanMilliseconds:F0}ms, align={AlignMilliseconds:F0}ms, " &
                   $"total={TotalMilliseconds:F0}ms, candidates={Candidates}, hits={Hits}"
        End Function
    End Class
End Namespace
