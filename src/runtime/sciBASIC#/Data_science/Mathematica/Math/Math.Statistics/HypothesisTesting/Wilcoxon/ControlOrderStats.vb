Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed
Imports Microsoft.VisualBasic.Math.Statistics.OrderStatistics
Imports sysmath = System.Math

Namespace Hypothesis

    ''' <summary>
    ''' 对照组序统计缓存：把"对照组每个基因在所有细胞上的 CPM"排序后缓存，
    ''' 之后对任意数量的实验组做秩和检验都只做二分查找。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 这是"一次建表、多次检验"的核心对象：构建是 <c>O(G·m·log m)</c>，
    ''' 之后每个基因每次检验是 <c>O(n log m)</c>（n 为实验组细胞数）。
    ''' 在单细胞差异表达里 n 通常只有几百、m 上万，这个缓存能把批量检验从数小时压到数秒。
    ''' </para>
    ''' <para>
    ''' 对象构建后不可变，可以安全地被多个线程共享读取。
    ''' </para>
    ''' </remarks>
    Public Class ControlOrderStats

        Private ReadOnly m_table As OrderStatisticsTable

        Private Sub New(table As OrderStatisticsTable)
            m_table = table
        End Sub

        ''' <summary>
        ''' 由对照组计数矩阵（细胞 × 基因）构建：内部先转成每细胞 CPM 再排序
        ''' </summary>
        ''' <param name="controlCounts">对照组计数矩阵，非负整数</param>
        ''' <param name="precision">存储精度</param>
        ''' <returns></returns>
        Public Shared Function Create(controlCounts As CsrMatrix,
                                     Optional precision As ValuePrecision = ValuePrecision.Float32) As ControlOrderStats

            If controlCounts Is Nothing Then Throw New ArgumentNullException(NameOf(controlCounts))

            Return New ControlOrderStats(OrderStatisticsTable.CreateFromCounts(controlCounts, precision))
        End Function

        ''' <summary>
        ''' 由已构建好的序统计表构造（用于复用或测试）
        ''' </summary>
        ''' <param name="table"></param>
        ''' <returns></returns>
        Public Shared Function FromTable(table As OrderStatisticsTable) As ControlOrderStats
            If table Is Nothing Then Throw New ArgumentNullException(NameOf(table))

            Return New ControlOrderStats(table)
        End Function

        ''' <summary>
        ''' 底层的序统计表
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Table As OrderStatisticsTable
            Get
                Return m_table
            End Get
        End Property

        ''' <summary>
        ''' 基因数量
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property GeneCount As Integer
            Get
                Return m_table.GeneCount
            End Get
        End Property

        ''' <summary>
        ''' 每个基因的细胞数量
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property CellCount As Integer
            Get
                Return m_table.CellCount
            End Get
        End Property

        ''' <summary>
        ''' 逐基因的平均每细胞 CPM
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property MeanCpm As Double()
            Get
                Return m_table.MeanCpm
            End Get
        End Property

        ''' <summary>
        ''' 逐基因的并列和 <c>Σ(t³ - t)</c>
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property TieSum As Double()
            Get
                Return m_table.TieSum
            End Get
        End Property

        ''' <summary>
        ''' 计算单个基因的 Mann-Whitney U 统计量
        ''' </summary>
        ''' <param name="gene">基因下标</param>
        ''' <param name="sampleValues">实验组在该基因上的每细胞 CPM</param>
        ''' <returns>
        ''' 对每个实验组观测，累加"对照组中小于它的个数"与"相等个数的一半"
        ''' </returns>
        Public Function RankSum(gene As Integer, sampleValues As Double()) As Double
            Return Core(gene, sampleValues).U
        End Function

        ''' <summary>
        ''' 计算单个基因的合并并列和（对照组 + 实验组的并列结构合并之后）
        ''' </summary>
        ''' <param name="gene">基因下标</param>
        ''' <param name="sampleValues">实验组在该基因上的每细胞 CPM</param>
        ''' <returns><c>Σ(t³ - t)</c>，用于方差校正</returns>
        Public Function PooledTieSum(gene As Integer, sampleValues As Double()) As Double
            Return Core(gene, sampleValues).PooledTie
        End Function

        ''' <summary>
        ''' 一次遍历同时求出 U 与合并并列和（批量检验内部使用）
        ''' </summary>
        ''' <param name="gene"></param>
        ''' <param name="sampleValues"></param>
        ''' <returns></returns>
        Friend Function Core(gene As Integer, sampleValues As Double()) As RankSumCore
            If sampleValues Is Nothing Then Throw New ArgumentNullException(NameOf(sampleValues))

            Dim counts As New Dictionary(Of Double, Integer)(sampleValues.Length)
            Dim u As Double = 0.0

            For i As Integer = 0 To sampleValues.Length - 1
                Dim v As Double = sampleValues(i)

                If Double.IsNaN(v) Then
                    Throw New ArgumentException($"sample value at index {i} is NaN", NameOf(sampleValues))
                End If

                Dim lo As Integer = m_table.LowerBound(gene, v)
                Dim hi As Integer = m_table.UpperBound(gene, v)

                ' 小于 v 的计 1，等于 v 的计 0.5 —— Mann-Whitney U 的并列口径
                u += lo + 0.5 * (hi - lo)

                Dim key As Double = GridKey(v)
                Dim count As Integer

                If counts.TryGetValue(key, count) Then
                    counts(key) = count + 1
                Else
                    counts(key) = 1
                End If
            Next

            Dim pooled As Double = m_table.TieSum(gene)

            For Each pair As KeyValuePair(Of Double, Integer) In counts
                Dim controlTies As Integer = m_table.CountEqual(gene, pair.Key)
                Dim merged As Double = controlTies + pair.Value

                pooled += (merged * merged * merged - merged) - (controlTies * controlTies * controlTies - controlTies)
            Next

            Return New RankSumCore With {
                .U = u,
                .PooledTie = pooled,
                .N1 = sampleValues.Length,
                .N2 = m_table.CellCount
            }
        End Function

        ''' <summary>
        ''' 把取值对齐到序统计表所用的数值栅格（float32 表的并列判定必须按栅格进行）
        ''' </summary>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Private Function GridKey(value As Double) As Double
            If m_table.Precision = ValuePrecision.Float32 Then Return CDbl(CSng(value))

            Return value
        End Function

        Public Overrides Function ToString() As String
            Return $"control order statistics: {GeneCount} gene x {CellCount} cell"
        End Function

    End Class

    ''' <summary>
    ''' 单个基因的秩和核心中间量
    ''' </summary>
    Friend Structure RankSumCore
        Public U As Double
        Public PooledTie As Double
        Public N1 As Integer
        Public N2 As Integer
    End Structure
End Namespace
