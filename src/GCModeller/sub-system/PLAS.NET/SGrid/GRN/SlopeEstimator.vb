Imports System.Drawing
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.Math.Interpolation
Imports std = System.Math

Namespace GRN

    ''' <summary>
    ''' The numerical derivative estimation of the gene expression time series data.
    ''' (基因表达时序数据的数值微分，即 method.md 之中的 s_diff 步骤)
    ''' </summary>
    ''' <remarks>
    ''' 通过对表达量做三次样条插值之后再求斜率，可以把 ODE 拟合问题转换为
    ''' 针对斜率的回归问题，从而避免对整个系统做反复的数值积分。
    ''' 注意 <see cref="CubicSpline.GetPoint(Single)"/> 的参数是归一化到 [0,1] 区间的
    ''' 曲线参数（与之对应的采样下标线性对应），而不是时间轴的绝对坐标，
    ''' 因此求斜率的时候需要同时取得插值点的 X 与 Y，再用差分比值得到 dY/dX。
    ''' </remarks>
    Public Module SlopeEstimator

        ''' <summary>
        ''' 估计每一个基因在每一个时间点之上的斜率 dX/dt
        ''' </summary>
        ''' <param name="series"></param>
        ''' <param name="denoise">
        ''' 是否先做三次样条平滑之后再求斜率；当时间点的数量不足（&lt; 4）的时候会自动退化为直接的中心差分
        ''' </param>
        ''' <param name="smoothWindow">
        ''' 求导之前的滑动平均窗口（奇数，零或者小于 3 表示不做滑动平均）。
        ''' 单细胞 pseudotime 分箱之后的轨迹仍然含有很强的高频噪声，
        ''' 直接做插值样条会对噪声求导（斜率几乎全部是噪声），
        ''' 故这里先对轨迹做一次滑动平均再插值
        ''' </param>
        ''' <param name="parallel">
        ''' 是否对基因并行求导（默认打开）。
        ''' 每一个基因只读取自己的那一行、只写入 <c>slopes(i)</c>，
        ''' 因此这是严格的数据并行：结果与线程数无关，逐位可复现。
        ''' 若调用方已经处在并行区域之内，应当显式传入 <c>False</c> 以避免线程超订
        ''' </param>
        ''' <param name="threads">并发上限，零或者负数表示使用运行时的默认值</param>
        ''' <returns>第一维是基因、第二维是时间点的斜率矩阵</returns>
        ''' <remarks>
        ''' 在完整的表达矩阵（约 1.8 万个基因）之上，本函数原本是拟合流程之中最后一个
        ''' 完全串行的阶段；改为 <see cref="Parallel.For(Int32, Int32, Action(Of Int32))"/>
        ''' 之后可以随逻辑核数线性提速（实测 12 线程约 9~10 倍）。
        ''' </remarks>
        Public Function Estimate(series As ExpressionTimeSeries,
                                 Optional denoise As Boolean = True,
                                 Optional smoothWindow As Integer = 0,
                                 Optional parallel As Boolean = True,
                                 Optional threads As Integer = 0) As Double()()
            If series Is Nothing Then
                Throw New ArgumentNullException(NameOf(series))
            End If

            Dim n As Integer = series.nGenes
            Dim m As Integer = series.nTimePoints
            Dim t As Double() = series.TimePoints
            Dim slopes As Double()() = New Double(n - 1)() {}
            Dim window As Integer = normalizeWindow(smoothWindow, m)

            If m < 2 Then
                Throw New ArgumentException("At least 2 time points are required by the slope estimation!")
            End If

            ' 每个基因独立：只读 series.X(i)，只写 slopes(i)
            Dim body As Action(Of Integer) =
                Sub(i)
                    Dim y As Double() = series.X(i)

                    If denoise AndAlso m >= 4 Then
                        If window >= 3 AndAlso lag1(y) < SmoothnessTolerance Then
                            y = movingAverage(y, window)
                        End If

                        slopes(i) = splineSlope(t, y)
                    Else
                        slopes(i) = centralDifference(t, y)
                    End If
                End Sub

            ' 基因数很少的时候（例如金标准自测只有 3 个基因）串行更快，
            ' 因为此时线程调度与分块的开销会超过计算本身
            If parallel AndAlso n >= MinParallelGenes Then
                Dim opts As New System.Threading.Tasks.ParallelOptions()

                If threads > 0 Then
                    opts.MaxDegreeOfParallelism = threads
                End If

                ' 进度条在并行路径之上既不是线程安全的，也会因为每个基因刷新一次控制台
                ' 而带来可观的开销，故并行路径不使用 TqdmWrapper
                Call System.Threading.Tasks.Parallel.For(0, n, opts, body)
            Else
                For Each i As Integer In TqdmWrapper.Range(0, n)
                    Call body(i)
                Next
            End If

            Return slopes
        End Function

        ''' <summary>
        ''' 启用并行的最小基因数：低于该值时线程调度开销大于收益
        ''' </summary>
        Public Const MinParallelGenes As Integer = 64

        ''' <summary>
        ''' 轨迹的一阶自相关阈值：高于该值的轨迹被认为是已经足够光滑的，
        ''' 不需要再做滑动平均（例如由解析动力学方程积分得到的合成数据）
        ''' </summary>
        Public Const SmoothnessTolerance As Double = 0.9R

        ''' <summary>
        ''' 计算轨迹的一阶自相关系数，用于判断是否需要平滑
        ''' </summary>
        ''' <param name="y"></param>
        ''' <returns></returns>
        Public Function lag1(y As Double()) As Double
            If y Is Nothing OrElse y.Length < 3 Then
                Return 0
            End If

            Dim mean As Double = 0

            For i As Integer = 0 To y.Length - 1
                mean += y(i)
            Next

            mean /= y.Length

            Dim num As Double = 0
            Dim den As Double = 0

            For i As Integer = 0 To y.Length - 1
                Dim d As Double = y(i) - mean

                den += d * d

                If i > 0 Then
                    num += d * (y(i - 1) - mean)
                End If
            Next

            If den <= 0.000000000001 Then
                Return 1
            End If

            Return num / den
        End Function

        ''' <summary>
        ''' 把滑动窗口规整化为不超过采样点数量的奇数
        ''' </summary>
        ''' <param name="window"></param>
        ''' <param name="m"></param>
        ''' <returns></returns>
        Private Function normalizeWindow(window As Integer, m As Integer) As Integer
            If window < 3 OrElse m < 5 Then
                Return 0
            End If

            Dim half As Integer = window \ 2
            Dim maxHalf As Integer = (m - 2) \ 2

            If half > maxHalf Then
                half = maxHalf
            End If
            If half < 1 Then
                Return 0
            End If

            Return half * 2 + 1
        End Function

        ''' <summary>
        ''' 居中滑动平均（边界处自动收缩窗口）
        ''' </summary>
        ''' <param name="y"></param>
        ''' <param name="window"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 使用前缀和实现，单点的窗口求和是 O(1) 而不是 O(window)：
        ''' 完整矩阵（1.8 万基因 × 数百个时间点）上这一步从 <c>O(m·w)</c> 降到 <c>O(m)</c>。
        ''' </remarks>
        Public Function movingAverage(y As Double(), window As Integer) As Double()
            If y Is Nothing OrElse y.Length = 0 Then
                Return y
            End If

            Dim half As Integer = std.Max(window, 1) \ 2
            Dim len As Integer = y.Length
            Dim out As Double() = New Double(len - 1) {}
            Dim prefix As Double() = New Double(len) {}

            For i As Integer = 0 To len - 1
                prefix(i + 1) = prefix(i) + y(i)
            Next

            For i As Integer = 0 To len - 1
                Dim lo As Integer = std.Max(0, i - half)
                Dim hi As Integer = std.Min(len - 1, i + half)

                out(i) = (prefix(hi + 1) - prefix(lo)) / (hi - lo + 1)
            Next

            Return out
        End Function

        ''' <summary>
        ''' 直接的中心差分
        ''' </summary>
        Private Function centralDifference(t As Double(), y As Double()) As Double()
            Dim m As Integer = y.Length
            Dim s As Double() = New Double(m - 1) {}

            For k As Integer = 0 To m - 1
                If k = 0 Then
                    s(k) = (y(1) - y(0)) / (t(1) - t(0))
                ElseIf k = m - 1 Then
                    s(k) = (y(m - 1) - y(m - 2)) / (t(m - 1) - t(m - 2))
                Else
                    s(k) = (y(k + 1) - y(k - 1)) / (t(k + 1) - t(k - 1))
                End If
            Next

            Return s
        End Function

        ''' <summary>
        ''' 先做三次样条插值，再在插值函数之上做中心差分求斜率
        ''' </summary>
        ''' <param name="t">时间轴</param>
        ''' <param name="y">表达量</param>
        Private Function splineSlope(t As Double(), y As Double()) As Double()
            Dim m As Integer = y.Length
            Dim s As Double() = New Double(m - 1) {}

            Try
                Dim points As PointF() = New PointF(m - 1) {}

                For k As Integer = 0 To m - 1
                    points(k) = New PointF(CSng(t(k)), CSng(y(k)))
                Next

                Dim spline As New CubicSpline(points)

                Call spline.CalcSpline()

                ' 曲线参数是与采样下标线性对应的归一化参数
                Dim ds As Double = 0.5R / (m - 1)
                Dim fallback As Double() = Nothing

                For k As Integer = 0 To m - 1
                    Dim position As Double = CDbl(k) / (m - 1)
                    Dim a As Double = std.Max(position - ds, 0)
                    Dim b As Double = std.Min(position + ds, 1)

                    Dim pa As PointF = spline.GetPoint(CSng(a))
                    Dim pb As PointF = spline.GetPoint(CSng(b))
                    Dim dx As Double = pb.X - pa.X

                    If std.Abs(dx) < 0.000000001 Then
                        ' 兜底的中心差分只计算一次：
                        ' 原实现在循环体内重复计算整条差分（O(m²) 且 O(m) 次分配）
                        If fallback Is Nothing Then
                            fallback = centralDifference(t, y)
                        End If

                        s(k) = fallback(k)
                    Else
                        s(k) = (pb.Y - pa.Y) / dx
                    End If

                    If Double.IsNaN(s(k)) OrElse Double.IsInfinity(s(k)) Then
                        s(k) = 0
                    End If
                Next
            Catch ex As Exception
                ' 样条插值失败的时候退化为直接的中心差分
                Return centralDifference(t, y)
            End Try

            Return s
        End Function
    End Module
End Namespace
