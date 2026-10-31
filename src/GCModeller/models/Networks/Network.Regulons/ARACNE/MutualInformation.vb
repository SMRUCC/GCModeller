Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.SIMD
Imports std = System.Math

Namespace ARACNE

    ''' <summary>
    ''' 互信息的估计函数：输入两个长度相同的表达向量，输出以<b>比特</b>为单位的互信息。
    ''' </summary>
    ''' <param name="x">第一个基因的表达向量</param>
    ''' <param name="y">第二个基因的表达向量</param>
    ''' <returns></returns>
    Public Delegate Function MiEstimatorFn(x As Double(), y As Double()) As Double

    ''' <summary>
    ''' 互信息(Mutual Information)估计器的实现集合。
    ''' 
    ''' 对于基因 ``X`` 与基因 ``Y``，互信息的定义是
    ''' 
    ''' ```
    ''' MI(X, Y) = sum_x sum_y p(x, y) * log2( p(x, y) / (p(x) * p(y)) )
    ''' ```
    ''' 
    ''' 基因表达量是连续变量，因此 ``p(x)``、``p(x, y)`` 都需要估计，本模块提供了
    ''' 四种估计方法(见 <see cref="MiEstimator"/>)：
    ''' 
    ''' + <see cref="GaussianCopulaMi(Double)"/>：解析解 ``-0.5 * log2(1 - r^2)``，``O(n)``；
    ''' + <see cref="AdaptivePartitioning(Double(), Double(), Integer, Integer)"/>：
    '''   ARACNE 原论文之中使用的自适应分区估计器，``O(n log n)``；
    ''' + <see cref="EqualFrequencyHistogram(Double(), Double(), Integer)"/>：
    '''   分位数等频分箱，并且带有 Miller-Madow 偏差校正；
    ''' + <see cref="KernelDensity(Double(), Double(), Double)"/>：
    '''   高斯核密度的留一法估计，``O(n^2)``。
    ''' 
    ''' 所有估计器统一以 ``2`` 为底的对数(比特)输出，与 readme 的公式保持一致。
    ''' </summary>
    <HideModuleName>
    Public Module MutualInformation

        ''' <summary>
        ''' <c>ln(2)</c>，用于把自然对数换算成以 2 为底的对数
        ''' </summary>
        Private ReadOnly LN2 As Double = std.Log(2.0R)

        ''' <summary>
        ''' 以 2 为底的对数(比特)
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Log2(x As Double) As Double
            Return std.Log(x) / LN2
        End Function

        ''' <summary>
        ''' 高斯 copula 估计器：对于服从二元正态分布的变量，
        ''' 
        ''' ```
        ''' MI(X, Y) = -0.5 * log2(1 - r^2)
        ''' ```
        ''' 
        ''' 其中 ``r`` 为两个变量的 pearson(或者秩变换之后的 spearman)相关系数。
        ''' </summary>
        ''' <param name="r">相关系数，取值区间为 ``[-1, 1]``</param>
        ''' <returns>互信息(比特)</returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GaussianCopulaMi(r As Double) As Double
            Dim rr As Double = r * r

            If rr <= 0.0R Then
                Return 0.0R
            ElseIf rr >= 1.0R Then
                ' 完全相关：理论上互信息无穷大，这里取出一个足够大的有限值
                ' 以避免 DPI 与置换阈值之中出现无穷大的比较
                Return 64.0R
            Else
                Return -0.5R * Log2(1.0R - rr)
            End If
        End Function

        ''' <summary>
        ''' 直接由两个已经中心化并且 L2 归一化的向量计算互信息。
        ''' 
        ''' 归一化之后 ``dot(x, y) / n`` 就是相关系数，因此一次长度为 ``n`` 的内积
        ''' (SIMD 加速)就可以得到一个基因对的互信息，这是大样本单细胞表达矩阵之上
        ''' 的默认计算路径。
        ''' </summary>
        ''' <param name="zx">中心化 + L2 归一化之后的表达向量</param>
        ''' <param name="zy">中心化 + L2 归一化之后的表达向量</param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function CopulaMiFromUnitVectors(zx As Double(), zy As Double()) As Double
            ' 已经中心化 + L2 归一化的两个向量的内积就是 pearson 相关系数
            Return GaussianCopulaMi(SimdReduce.Dot(zx, zy))
        End Function

        ''' <summary>
        ''' pearson 相关系数
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="y"></param>
        ''' <returns>任何一个变量为零方差的时候返回零</returns>
        Public Function Pearson(x As Double(), y As Double()) As Double
            Dim n As Integer = x.Length

            If n = 0 Then
                Return 0.0R
            ElseIf y.Length <> n Then
                Throw New ArgumentException($"the length of the two vectors not agree: {n} vs {y.Length}!", NameOf(y))
            End If

            Dim mx As Double = SimdReduce.Mean(x)
            Dim my As Double = SimdReduce.Mean(y)
            Dim sxy As Double = 0.0R
            Dim sxx As Double = 0.0R
            Dim syy As Double = 0.0R
            Dim dx As Double
            Dim dy As Double

            For i As Integer = 0 To n - 1
                dx = x(i) - mx
                dy = y(i) - my

                sxy += dx * dy
                sxx += dx * dx
                syy += dy * dy
            Next

            If sxx <= 0.0R OrElse syy <= 0.0R Then
                Return 0.0R
            Else
                Return sxy / std.Sqrt(sxx * syy)
            End If
        End Function

        ''' <summary>
        ''' 高斯 copula 估计器(带标准差与均值的标准化)
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="y"></param>
        ''' <returns></returns>
        Public Function GaussianCopula(x As Double(), y As Double()) As Double
            Return GaussianCopulaMi(Pearson(x, y))
        End Function

        ''' <summary>
        ''' ARACNE 原论文(Algorithm 1)之中使用的自适应分区估计器。
        ''' 
        ''' 算法过程：
        ''' 
        ''' 1. 初始时整个散点图就是一个 bin；
        ''' 2. 反复找出“点数最多”的 bin，沿着<b>极差更大</b>的那个轴在中位数处把它一分为二；
        ''' 3. 当 bin 的数量达到上限，或者点数最多的 bin 已经无法再分裂
        '''    (点数少于两倍的最小点数)的时候停止；
        ''' 4. 用分区得到的单元格计算
        ''' 
        ''' ```
        ''' MI(X, Y) = sum_cell p(cell) * log2( p(cell) / (p(x in cellX) * p(y in cellY)) )
        ''' ```
        ''' 
        ''' 其中边缘概率 ``p(x in cellX)`` 通过把全部样本投影到该轴上做区间计数得到
        ''' (使用预排序的投影数组 + 二分查找，``O(log n)``)。
        ''' 
        ''' 与固定的等频分箱相比，自适应分区会把更多的分辨率分配给点密集的区域，
        ''' 因此对长尾的基因表达数据更加稳定。
        ''' </summary>
        ''' <param name="x">第一个基因的表达向量</param>
        ''' <param name="y">第二个基因的表达向量</param>
        ''' <param name="maxBins">bin 数量的上限，小于等于零时取 ``round(sqrt(n))``</param>
        ''' <param name="minPoints">每个 bin 的最小点数，小于等于零时自动推算</param>
        ''' <returns>互信息(比特)</returns>
        Public Function AdaptivePartitioning(x As Double(), y As Double(),
                                             Optional maxBins As Integer = 0,
                                             Optional minPoints As Integer = 0) As Double

            Dim n As Integer = x.Length

            If y.Length <> n Then
                Throw New ArgumentException($"the length of the two vectors not agree: {n} vs {y.Length}!", NameOf(y))
            End If
            If n < 8 Then
                ' 样本太少的时候分区没有意义，直接退回 copula 估计器
                Return GaussianCopula(x, y)
            End If

            If maxBins <= 0 Then
                maxBins = CInt(std.Round(std.Sqrt(n)))
            End If

            maxBins = std.Max(2, std.Min(maxBins, 128))

            If minPoints <= 0 Then
                minPoints = std.Max(3, n \ (4 * maxBins))
            End If

            Dim boxes As New List(Of PartitionBox)
            Dim all As Integer() = Enumerable.Range(0, n).ToArray()

            Call boxes.Add(MakeBox(all, x, y))

            While boxes.Count < maxBins
                Dim pick As Integer = -1
                Dim size As Integer = 0

                ' 找出点数最多的 bin
                For i As Integer = 0 To boxes.Count - 1
                    If boxes(i).indices.Length > size Then
                        size = boxes(i).indices.Length
                        pick = i
                    End If
                Next

                If pick < 0 OrElse size < 2 * minPoints Then
                    Exit While
                End If

                Dim box As PartitionBox = boxes(pick)
                Dim alongX As Boolean = (box.xhi - box.xlo) >= (box.yhi - box.ylo)
                Dim order As Integer() = box.indices _
                    .OrderBy(Function(i) If(alongX, x(i), y(i))) _
                    .ToArray
                Dim half As Integer = size \ 2
                Dim left As PartitionBox = MakeBox(order.Take(half).ToArray, x, y)
                Dim right As PartitionBox = MakeBox(order.Skip(half).ToArray, x, y)

                If left.indices.Length < minPoints OrElse right.indices.Length < minPoints Then
                    ' 再继续分裂下去就会出现空 bin，停止分区
                    Exit While
                End If

                boxes(pick) = left
                Call boxes.Add(right)
            End While

            ' 预排序的投影数组，用于 O(log n) 的区间计数
            Dim xsorted As Double() = x.OrderBy(Function(v) v).ToArray
            Dim ysorted As Double() = y.OrderBy(Function(v) v).ToArray
            Dim mi As Double = 0.0R

            For Each box As PartitionBox In boxes
                Dim p As Double = box.indices.Length / CDbl(n)
                Dim px As Double = CountInRange(xsorted, box.xlo, box.xhi) / CDbl(n)
                Dim py As Double = CountInRange(ysorted, box.ylo, box.yhi) / CDbl(n)

                If p <= 0.0R OrElse px <= 0.0R OrElse py <= 0.0R Then
                    Continue For
                End If

                mi += p * Log2(p / (px * py))
            Next

            If mi < 0.0R Then
                Return 0.0R
            Else
                Return mi
            End If
        End Function

        ''' <summary>
        ''' 分位数等频直方图估计器(readme 伪代码第二步之中的“按分位数离散化”)，
        ''' 并且采用 Miller-Madow 偏差校正：
        ''' 
        ''' ```
        ''' MI_corrected = MI_plugin + (m_x - 1) * (m_y - 1) / (2n)
        ''' ```
        ''' 
        ''' 其中 ``m_x``、``m_y`` 分别是两个变量之中非空 bin 的数量。
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="y"></param>
        ''' <param name="bins">每一个变量的等频 bin 数量，默认 4</param>
        ''' <returns></returns>
        Public Function EqualFrequencyHistogram(x As Double(), y As Double(), Optional bins As Integer = 4) As Double
            Dim n As Integer = x.Length

            If y.Length <> n Then
                Throw New ArgumentException($"the length of the two vectors not agree: {n} vs {y.Length}!", NameOf(y))
            End If
            If bins < 2 Then
                bins = 2
            End If
            If n < 4 * bins Then
                ' 每个 bin 至少需要几个样本，否则计数噪声会完全淹没信号
                Return GaussianCopula(x, y)
            End If

            Dim bx As Integer() = QuantileBins(x, bins)
            Dim by As Integer() = QuantileBins(y, bins)
            Dim joint As Double(,) = New Double(bins - 1, bins - 1) {}
            Dim px As Double() = New Double(bins - 1) {}
            Dim py As Double() = New Double(bins - 1) {}

            For s As Integer = 0 To n - 1
                joint(bx(s), by(s)) += 1.0R
                px(bx(s)) += 1.0R
                py(by(s)) += 1.0R
            Next

            Dim mi As Double = 0.0R

            For a As Integer = 0 To bins - 1
                If px(a) <= 0.0R Then
                    Continue For
                End If

                For b As Integer = 0 To bins - 1
                    If joint(a, b) <= 0.0R OrElse py(b) <= 0.0R Then
                        Continue For
                    End If

                    Dim pxy As Double = joint(a, b) / n

                    mi += pxy * Log2(pxy / ((px(a) / n) * (py(b) / n)))
                Next
            Next

            Dim mx As Integer = px.Count(Function(v) v > 0.0R)
            Dim my As Integer = py.Count(Function(v) v > 0.0R)

            mi += (mx - 1) * (my - 1) / (2.0R * n)

            If mi < 0.0R Then
                Return 0.0R
            Else
                Return mi
            End If
        End Function

        ''' <summary>
        ''' 高斯核密度估计器(留一法)：
        ''' 
        ''' ```
        ''' MI = (1/n) * sum_s log2( f(x_s, y_s) / (f(x_s) * f(y_s)) )
        ''' ```
        ''' 
        ''' 其中密度由高斯核估计，并且计算 ``f(x_s)`` 的时候排除样本 ``s`` 自身的贡献
        ''' (留一法)，否则核函数的峰值会带来虚假的正互信息。
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="y"></param>
        ''' <param name="bandwidth">核带宽，小于等于零时使用 Silverman 经验公式</param>
        ''' <returns></returns>
        Public Function KernelDensity(x As Double(), y As Double(), Optional bandwidth As Double = 0) As Double
            Dim n As Integer = x.Length

            If y.Length <> n Then
                Throw New ArgumentException($"the length of the two vectors not agree: {n} vs {y.Length}!", NameOf(y))
            End If
            If n < 8 Then
                Return GaussianCopula(x, y)
            End If

            Dim hx As Double = If(bandwidth > 0.0R, bandwidth, SilvermanBandwidth(x))
            Dim hy As Double = If(bandwidth > 0.0R, bandwidth, SilvermanBandwidth(y))

            If hx <= 0.0R OrElse hy <= 0.0R Then
                ' 零方差(常量基因)的情况
                Return GaussianCopula(x, y)
            End If

            Dim inv2hx2 As Double = 1.0R / (2.0R * hx * hx)
            Dim inv2hy2 As Double = 1.0R / (2.0R * hy * hy)
            Dim norm As Double = (n - 1) * 2.0R * std.PI * hx * hy
            Dim normX As Double = (n - 1) * std.Sqrt(2.0R * std.PI) * hx
            Dim normY As Double = (n - 1) * std.Sqrt(2.0R * std.PI) * hy
            Dim mi As Double = 0.0R

            For s As Integer = 0 To n - 1
                Dim sxy As Double = 0.0R
                Dim sx As Double = 0.0R
                Dim sy As Double = 0.0R

                For t As Integer = 0 To n - 1
                    If s = t Then
                        Continue For
                    End If

                    Dim dx As Double = x(s) - x(t)
                    Dim dy As Double = y(s) - y(t)
                    Dim ex As Double = std.Exp(-dx * dx * inv2hx2)
                    Dim ey As Double = std.Exp(-dy * dy * inv2hy2)

                    sxy += ex * ey
                    sx += ex
                    sy += ey
                Next

                Dim fxy As Double = sxy / norm
                Dim fx As Double = sx / normX
                Dim fy As Double = sy / normY

                If fxy > 0.0R AndAlso fx > 0.0R AndAlso fy > 0.0R Then
                    mi += Log2(fxy / (fx * fy))
                End If
            Next

            mi /= n

            If mi < 0.0R Then
                Return 0.0R
            Else
                Return mi
            End If
        End Function

        ''' <summary>
        ''' Silverman 经验带宽：
        ''' 
        ''' ```
        ''' h = 0.9 * min(sd, IQR / 1.34) * n^(-1/5)
        ''' ```
        ''' </summary>
        ''' <param name="v"></param>
        ''' <returns></returns>
        Public Function SilvermanBandwidth(v As Double()) As Double
            Dim n As Integer = v.Length

            If n < 2 Then
                Return 0.0R
            End If

            Dim mean As Double = SimdReduce.Mean(v)
            Dim sd As Double = std.Sqrt(SimdReduce.SumSquares(v.Select(Function(x) x - mean).ToArray) / (n - 1))
            Dim sorted As Double() = v.OrderBy(Function(x) x).ToArray
            Dim q1 As Double = Quantile(sorted, 0.25R)
            Dim q3 As Double = Quantile(sorted, 0.75R)
            Dim iqr As Double = (q3 - q1) / 1.34R
            Dim scale As Double = If(iqr > 0.0R, std.Min(sd, iqr), sd)

            If scale <= 0.0R Then
                Return 0.0R
            Else
                Return 0.9R * scale * std.Pow(n, -0.2R)
            End If
        End Function

        ''' <summary>
        ''' 线性插值分位数
        ''' </summary>
        ''' <param name="sorted">已经升序排列的数组</param>
        ''' <param name="p">分位点，取值区间为 ``[0, 1]``</param>
        ''' <returns></returns>
        Public Function Quantile(sorted As Double(), p As Double) As Double
            Dim n As Integer = sorted.Length

            If n = 0 Then
                Return 0.0R
            ElseIf n = 1 Then
                Return sorted(0)
            End If

            p = std.Max(0.0R, std.Min(p, 1.0R))

            Dim pos As Double = p * (n - 1)
            Dim lo As Integer = CInt(std.Floor(pos))
            Dim hi As Integer = std.Min(n - 1, lo + 1)
            Dim frac As Double = pos - lo

            Return sorted(lo) * (1.0R - frac) + sorted(hi) * frac
        End Function

        ''' <summary>
        ''' 按照分位数把向量切成等频的 bin，返回每一个样本所属的 bin 下标。
        ''' </summary>
        ''' <param name="v"></param>
        ''' <param name="bins"></param>
        ''' <returns></returns>
        Public Function QuantileBins(v As Double(), bins As Integer) As Integer()
            Dim n As Integer = v.Length
            Dim order As Integer() = Enumerable.Range(0, n).OrderBy(Function(i) v(i)).ToArray
            Dim result As Integer() = New Integer(n - 1) {}

            For pos As Integer = 0 To n - 1
                Dim bi As Integer = CInt(std.Floor(pos * bins / CDbl(n)))

                If bi > bins - 1 Then
                    bi = bins - 1
                End If

                result(order(pos)) = bi
            Next

            Return result
        End Function

        ''' <summary>
        ''' 把表达量转换成秩(1 为最小值)，相同数值取平均秩，也就是 spearman 相关所使用
        ''' 的秩变换。
        ''' </summary>
        ''' <param name="v"></param>
        ''' <returns></returns>
        Public Function Ranks(v As Double()) As Double()
            Dim n As Integer = v.Length
            Dim order As Integer() = Enumerable.Range(0, n).OrderBy(Function(idx) v(idx)).ToArray
            Dim result As Double() = New Double(n - 1) {}
            Dim i As Integer = 0

            While i < n
                Dim j As Integer = i

                While j + 1 < n AndAlso v(order(j + 1)) = v(order(i))
                    j += 1
                End While

                ' 平局取平均秩
                Dim rank As Double = (i + j) / 2.0R + 1.0R

                For k As Integer = i To j
                    result(order(k)) = rank
                Next

                i = j + 1
            End While

            Return result
        End Function

        ''' <summary>
        ''' 中心化并且做 L2 归一化，使得任意两个向量的内积除以 ``n`` 就是相关系数。
        ''' </summary>
        ''' <param name="v"></param>
        ''' <returns></returns>
        Public Function UnitVector(v As Double()) As Double()
            Dim n As Integer = v.Length
            Dim mean As Double = SimdReduce.Mean(v)
            Dim z As Double() = New Double(n - 1) {}

            For i As Integer = 0 To n - 1
                z(i) = v(i) - mean
            Next

            Dim norm As Double = SimdReduce.L2Norm(z)

            If norm <= 0.0R Then
                Return z
            End If

            For i As Integer = 0 To n - 1
                z(i) /= norm
            Next

            Return z
        End Function

        ''' <summary>
        ''' z-score 标准化
        ''' </summary>
        ''' <param name="v"></param>
        ''' <returns></returns>
        Public Function Standardize(v As Double()) As Double()
            Dim n As Integer = v.Length
            Dim mean As Double = SimdReduce.Mean(v)
            Dim z As Double() = New Double(n - 1) {}
            Dim ss As Double = 0.0R

            For i As Integer = 0 To n - 1
                z(i) = v(i) - mean
                ss += z(i) * z(i)
            Next

            Dim sd As Double = std.Sqrt(ss / std.Max(1, n - 1))

            If sd <= 0.0R Then
                Return z
            End If

            For i As Integer = 0 To n - 1
                z(i) /= sd
            Next

            Return z
        End Function

        ''' <summary>
        ''' 边缘信息熵 ``H(X) = -sum p(x) log2 p(x)``(用于自检与统计)
        ''' </summary>
        ''' <param name="v"></param>
        ''' <param name="bins"></param>
        ''' <returns></returns>
        Public Function Entropy(v As Double(), Optional bins As Integer = 4) As Double
            Dim counts As Integer() = QuantileBins(v, std.Max(2, bins))
            Dim n As Integer = v.Length
            Dim freq As New Dictionary(Of Integer, Integer)

            For Each i As Integer In counts
                If freq.ContainsKey(i) Then
                    freq(i) += 1
                Else
                    freq(i) = 1
                End If
            Next

            Dim h As Double = 0.0R

            For Each cnt As Integer In freq.Values
                Dim p As Double = cnt / CDbl(n)

                h -= p * Log2(p)
            Next

            Return h
        End Function

        ''' <summary>
        ''' 生成一个可复现的洗牌结果(不修改输入的数组)
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="data"></param>
        ''' <param name="rng"></param>
        ''' <returns></returns>
        Public Function Shuffle(Of T)(data As T(), rng As Random) As T()
            Dim copy As T() = DirectCast(data.Clone(), T())
            Dim n As Integer = copy.Length

            For i As Integer = n - 1 To 1 Step -1
                Dim j As Integer = rng.Next(i + 1)
                Dim tmp As T = copy(i)

                copy(i) = copy(j)
                copy(j) = tmp
            Next

            Return copy
        End Function

        ''' <summary>
        ''' 从 ``[0, n)`` 之中随机抽取 ``k`` 个互不相同的下标，返回升序排列的结果。
        ''' 
        ''' 采用部分洗牌(而不是“随机抽取 + 查重”)，因此在 ``k`` 接近 ``n`` 的
        ''' 时候也不会退化。
        ''' </summary>
        ''' <param name="n"></param>
        ''' <param name="k"></param>
        ''' <param name="rng"></param>
        ''' <returns></returns>
        Public Function SampleIndices(n As Integer, k As Integer, rng As Random) As Integer()
            If k <= 0 Then
                Return New Integer() {}
            ElseIf k >= n Then
                Return Enumerable.Range(0, n).ToArray
            End If

            Dim pool As Integer() = Enumerable.Range(0, n).ToArray

            For i As Integer = 0 To k - 1
                Dim j As Integer = rng.Next(i, n)
                Dim tmp As Integer = pool(i)

                pool(i) = pool(j)
                pool(j) = tmp
            Next

            Dim result As Integer() = pool.Take(k).ToArray

            Array.Sort(result)

            Return result
        End Function

        ''' <summary>
        ''' 依据估计方法创建互信息估计函数
        ''' </summary>
        ''' <param name="estimator"></param>
        ''' <param name="options"></param>
        ''' <returns></returns>
        Public Function GetEstimator(estimator As MiEstimator, Optional options As AracneOptions = Nothing) As MiEstimatorFn
            Dim opt As AracneOptions = If(options, New AracneOptions())

            Select Case estimator
                Case MiEstimator.AdaptivePartitioning
                    Return Function(x, y) AdaptivePartitioning(x, y, opt.adaptiveMaxBins, opt.adaptiveMinPoints)
                Case MiEstimator.EqualFrequencyHistogram
                    Return Function(x, y) EqualFrequencyHistogram(x, y, opt.histogramBins)
                Case MiEstimator.KernelDensity
                    Return Function(x, y)
                               Dim n As Integer = x.Length

                               If opt.kernelMaxSamples > 0 AndAlso n > opt.kernelMaxSamples Then
                                   ' 均匀抽样而不是随机抽样：保持表达量的分布形状，
                                   ' 并且不依赖随机种子
                                   Dim step_ As Double = n / CDbl(opt.kernelMaxSamples)
                                   Dim xs As Double() = New Double(opt.kernelMaxSamples - 1) {}
                                   Dim ys As Double() = New Double(opt.kernelMaxSamples - 1) {}

                                   For i As Integer = 0 To opt.kernelMaxSamples - 1
                                       Dim idx As Integer = std.Min(n - 1, CInt(std.Floor(i * step_)))

                                       xs(i) = x(idx)
                                       ys(i) = y(idx)
                                   Next

                                   Return KernelDensity(xs, ys, opt.kernelBandwidth)
                               Else
                                   Return KernelDensity(x, y, opt.kernelBandwidth)
                               End If
                           End Function
                Case Else
                    Return AddressOf GaussianCopula
            End Select
        End Function

#Region "helpers"

        ''' <summary>
        ''' 自适应分区过程之中的一个单元格
        ''' </summary>
        Private Structure PartitionBox

            ''' <summary>
            ''' 落在当前单元格之中的样本下标
            ''' </summary>
            Public indices As Integer()
            Public xlo As Double
            Public xhi As Double
            Public ylo As Double
            Public yhi As Double

        End Structure

        ''' <summary>
        ''' 构造一个单元格，同时记录它的 x/y 投影区间
        ''' </summary>
        Private Function MakeBox(indices As Integer(), x As Double(), y As Double()) As PartitionBox
            Dim xlo As Double = Double.MaxValue
            Dim xhi As Double = Double.MinValue
            Dim ylo As Double = Double.MaxValue
            Dim yhi As Double = Double.MinValue

            For Each i As Integer In indices
                If x(i) < xlo Then
                    xlo = x(i)
                End If
                If x(i) > xhi Then
                    xhi = x(i)
                End If
                If y(i) < ylo Then
                    ylo = y(i)
                End If
                If y(i) > yhi Then
                    yhi = y(i)
                End If
            Next

            Return New PartitionBox With {
                .indices = indices,
                .xlo = xlo,
                .xhi = xhi,
                .ylo = ylo,
                .yhi = yhi
            }
        End Function

        ''' <summary>
        ''' 统计升序数组之中落在闭区间 ``[lo, hi]`` 之内的元素个数(二分查找)
        ''' </summary>
        Private Function CountInRange(sorted As Double(), lo As Double, hi As Double) As Integer
            Dim left As Integer = LowerBound(sorted, lo)
            Dim right As Integer = UpperBound(sorted, hi)

            Return std.Max(0, right - left)
        End Function

        ''' <summary>
        ''' 第一个 ``&gt;= value`` 的元素下标
        ''' </summary>
        Private Function LowerBound(sorted As Double(), value As Double) As Integer
            Dim start As Integer = 0
            Dim ends As Integer = sorted.Length

            While start < ends
                Dim mid As Integer = (start + ends) \ 2

                If sorted(mid) < value Then
                    start = mid + 1
                Else
                    ends = mid
                End If
            End While

            Return start
        End Function

        ''' <summary>
        ''' 第一个 ``&gt; value`` 的元素下标
        ''' </summary>
        Private Function UpperBound(sorted As Double(), value As Double) As Integer
            Dim start As Integer = 0
            Dim ends As Integer = sorted.Length

            While start < ends
                Dim mid As Integer = (start + ends) \ 2

                If sorted(mid) <= value Then
                    start = mid + 1
                Else
                    ends = mid
                End If
            End While

            Return start
        End Function

#End Region
    End Module
End Namespace
