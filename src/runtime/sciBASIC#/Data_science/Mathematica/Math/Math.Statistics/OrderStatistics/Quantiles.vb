Imports std = System.Math

Namespace OrderStatistics

    ''' <summary>
    ''' 分位数的无状态工具函数：分层分位、R type-7 插值、中位数、名次换算。
    ''' </summary>
    ''' <remarks>
    ''' 需要"缓存排序结果并反复查询"时请使用 <see cref="OrderStatisticsTable"/> 或
    ''' <see cref="QuantileProfile"/>；本模块只处理一次性的小数组。
    ''' </remarks>
    Public Module Quantiles

        ''' <summary>
        ''' 分层分位向量：<c>(k + 0.5) / n</c>，<c>k = 0 .. n-1</c>
        ''' </summary>
        ''' <param name="n">分位点个数</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 与 <c>(k + 1) / (n + 1)</c> 相比，分层分位不会取到 0 与 1 两个端点，
        ''' 因此不会把"经验分布的极值"当成"理论分布的端点"，更适合做分位数映射。
        ''' </remarks>
        Public Function Stratified(n As Integer) As Double()
            If n < 0 Then Throw New ArgumentOutOfRangeException(NameOf(n), n, "the quantile count must not be negative")
            If n = 0 Then Return New Double() {}

            Dim out As Double() = New Double(n - 1) {}

            For k As Integer = 0 To n - 1
                out(k) = (k + 0.5) / n
            Next

            Return out
        End Function

        ''' <summary>
        ''' R 的 <c>type = 7</c> 分位数：<c>h = (n - 1) · p</c>，在相邻名次间线性插值
        ''' </summary>
        ''' <param name="sorted">升序数组</param>
        ''' <param name="p">概率，落在 <c>[0, 1]</c></param>
        ''' <returns>空数组返回 <see cref="Double.NaN"/></returns>
        Public Function Type7(sorted As Double(), p As Double) As Double
            If sorted Is Nothing Then Throw New ArgumentNullException(NameOf(sorted))
            If sorted.Length = 0 Then Return Double.NaN
            If sorted.Length = 1 Then Return sorted(0)
            If p <= 0.0 Then Return sorted(0)
            If p >= 1.0 Then Return sorted(sorted.Length - 1)

            Dim h As Double = (sorted.Length - 1) * p

            Return Interpolate(sorted, h)
        End Function

        ''' <summary>
        ''' 按"名次坐标"插值：<c>h = 0</c> 对应最小值，<c>h = n-1</c> 对应最大值
        ''' </summary>
        ''' <param name="sorted">升序数组</param>
        ''' <param name="h">名次坐标（可以是小数）</param>
        ''' <returns></returns>
        Public Function Interpolate(sorted As Double(), h As Double) As Double
            If sorted Is Nothing Then Throw New ArgumentNullException(NameOf(sorted))
            If sorted.Length = 0 Then Return Double.NaN

            If h <= 0.0 Then Return sorted(0)
            If h >= sorted.Length - 1 Then Return sorted(sorted.Length - 1)

            Dim lo As Integer = CInt(std.Floor(h))
            Dim fraction As Double = h - lo

            Return sorted(lo) * (1.0 - fraction) + sorted(lo + 1) * fraction
        End Function

        ''' <summary>
        ''' 已排序数组的中位数（偶数长度取中间两个的均值）
        ''' </summary>
        ''' <param name="sorted">升序数组</param>
        ''' <returns></returns>
        Public Function MedianOfSorted(sorted As Double()) As Double
            If sorted Is Nothing OrElse sorted.Length = 0 Then Return Double.NaN

            Dim n As Integer = sorted.Length

            If n Mod 2 = 1 Then Return sorted(n \ 2)

            Return 0.5 * (sorted(n \ 2 - 1) + sorted(n \ 2))
        End Function

        ''' <summary>
        ''' 未排序数组的中位数（内部会排序，会修改传入数组的副本）
        ''' </summary>
        ''' <param name="values"></param>
        ''' <returns></returns>
        Public Function Median(values As Double()) As Double
            If values Is Nothing OrElse values.Length = 0 Then Return Double.NaN

            Dim sorted As Double() = DirectCast(values.Clone(), Double())

            Call Array.Sort(sorted)

            Return MedianOfSorted(sorted)
        End Function

        ''' <summary>
        ''' 名次换算：<c>index = floor((quantile + shift - 0.5) · count)</c>
        ''' </summary>
        ''' <param name="quantile">分位数</param>
        ''' <param name="shift">平移量</param>
        ''' <param name="count">观测数量</param>
        ''' <returns>名次；返回 -1 表示"低于最小观测"（调用方应取 0 值）</returns>
        Public Function ShiftedIndex(quantile As Double, shift As Double, count As Integer) As Integer
            Dim position As Double = shift + quantile - 0.5

            If position <= 0.0 Then Return -1

            Dim index As Integer = CInt(std.Floor(position * count))

            If index >= count Then index = count - 1

            Return index
        End Function

    End Module
End Namespace
