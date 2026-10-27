Imports Microsoft.VisualBasic.Math.Statistics.Distributions
Imports F = Microsoft.VisualBasic.Math.Statistics.Hypothesis.FishersExact.FishersExactTest
Imports std = System.Math

Namespace KOBAS

    ''' <summary>
    ''' 富集计算所使用的精确检验原子函数。
    ''' </summary>
    ''' <remarks>
    ''' 本模块覆盖 KOBAS 3.0 <c>src/kobas/discover.py</c> 中 <c>hyper</c>、
    ''' <c>fisher_test</c>、<c>binom_test</c>、<c>chisq_test</c> 与
    ''' <c>improved_chisq_test</c> 的数值计算部分。
    ''' 
    ''' 与 KOBAS 原实现的差异：
    ''' <list type="bullet">
    ''' <item>KOBAS 通过 rpy2 调用 R 的 <c>phyper/fisher.test/binom.test/chisq.test</c>，
    ''' 这里改为纯 .NET 实现，不依赖 R 运行时。</item>
    ''' <item>KOBAS 的 Fisher / 卡方列联表写成
    ''' <c>matrix(c(m1, n1-m1, m2, n2-m2), nc=2)</c>，其行列边际并不是
    ''' 「前景是否属于某通路」的标准 2x2 表；这里统一为标准表
    ''' <c>[[a, n1-a], [m2-a, n2-m2-(n1-a)]]</c>，与
    ''' <c>FisherCore.Enrichment.calcResult</c> 的口径一致。</item>
    ''' <item>卡方检验显式实现 Yates 连续性校正（对应 R <c>chisq.test</c> 的默认行为），
    ''' 并在任一期望频数小于 10 时退回 Fisher 精确检验。</item>
    ''' </list>
    ''' </remarks>
    Public Module ExactTests

        ''' <summary>
        ''' 期望频数低于该阈值时，卡方检验退回 Fisher 精确检验（KOBAS 的 <c>max_e</c>）。
        ''' </summary>
        Public Const ChiSquareExpectedThreshold As Double = 10

        Private ReadOnly logFactorialCache As New List(Of Double) From {0.0}
        Private ReadOnly cacheLock As New Object

        ''' <summary>
        ''' 计算 <c>ln(n!)</c>（内部带缓存，线程安全）。
        ''' </summary>
        ''' <param name="n">非负整数。</param>
        ''' <returns></returns>
        Public Function LogFactorial(n As Integer) As Double
            If n < 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(n), "n must be a non-negative integer.")
            End If

            SyncLock cacheLock
                While logFactorialCache.Count <= n
                    Dim k As Integer = logFactorialCache.Count

                    logFactorialCache.Add(logFactorialCache(k - 1) + std.Log(k))
                End While

                Return logFactorialCache(n)
            End SyncLock
        End Function

        ''' <summary>
        ''' 计算二项式系数的对数 <c>ln C(n, k)</c>。
        ''' </summary>
        ''' <param name="n">总数。</param>
        ''' <param name="k">选取数。</param>
        ''' <returns></returns>
        Public Function LogChoose(n As Integer, k As Integer) As Double
            If k < 0 OrElse k > n Then
                Return Double.NegativeInfinity
            End If

            Return LogFactorial(n) - LogFactorial(k) - LogFactorial(n - k)
        End Function

        ''' <summary>
        ''' 对数空间求和：<c>ln(Σ exp(x_i))</c>，用于避免上溢/下溢。
        ''' </summary>
        ''' <param name="values">对数域上的取值。</param>
        ''' <returns></returns>
        Public Function LogSumExp(values As IEnumerable(Of Double)) As Double
            Dim max As Double = Double.NegativeInfinity

            For Each value As Double In values
                If value > max Then
                    max = value
                End If
            Next

            If Double.IsNegativeInfinity(max) Then
                Return Double.NegativeInfinity
            End If

            Dim sum As Double = 0

            For Each value As Double In values
                sum += std.Exp(value - max)
            Next

            Return max + std.Log(sum)
        End Function

        ''' <summary>
        ''' 超几何分布的上尾概率 <c>P(X &gt;= observed)</c>。
        ''' </summary>
        ''' <param name="observed">观测到的命中数量（<c>q</c>）。</param>
        ''' <param name="populationSuccesses">总体之中的「成功」数量（<c>m</c>，即背景基因集中的成员数）。</param>
        ''' <param name="populationSize">总体规模（<c>N</c>）。</param>
        ''' <param name="draws">抽样数量（<c>k</c>，即前景基因列表的大小）。</param>
        ''' <returns>
        ''' 等价于 R 的 <c>1 - phyper(observed - 1, populationSuccesses, populationSize - populationSuccesses, draws)</c>。
        ''' </returns>
        Public Function HypergeometricUpperTail(observed As Integer,
                                                populationSuccesses As Integer,
                                                populationSize As Integer,
                                                draws As Integer) As Double

            If populationSize <= 0 OrElse draws <= 0 OrElse populationSuccesses <= 0 Then
                Return 1
            End If
            If draws > populationSize Then
                Throw New ArgumentException("The draw size cannot be larger than the population size.")
            End If

            Dim upper As Integer = std.Min(populationSuccesses, draws)

            If observed <= 0 Then
                Return 1
            End If
            If observed > upper Then
                Return 0
            End If

            Dim failures As Integer = populationSize - populationSuccesses
            Dim denominator As Double = LogChoose(populationSize, draws)
            Dim logs As New List(Of Double)

            For i As Integer = observed To upper
                Dim successes As Integer = i
                Dim rest As Integer = draws - i

                If rest > failures Then
                    Continue For
                End If

                logs.Add(LogChoose(populationSuccesses, successes) +
                         LogChoose(failures, rest) -
                         denominator)
            Next

            If logs.Count = 0 Then
                Return 0
            End If

            Dim total As Double = LogSumExp(logs)

            Return If(Double.IsNegativeInfinity(total), 0, std.Min(1.0, std.Exp(total)))
        End Function

        ''' <summary>
        ''' Fisher 精确检验的单尾（greater）p 值。
        ''' </summary>
        ''' <param name="a">前景基因列表与基因集的交集大小。</param>
        ''' <param name="b">前景基因列表之中不属于该基因集的数量。</param>
        ''' <param name="c">背景之中属于该基因集但不属于前景的数量。</param>
        ''' <param name="d">背景之中既不属于该基因集也不属于前景的数量。</param>
        ''' <returns>等价于 R 的 <c>fisher.test(matrix(c(a, b, c, d), nc = 2), alternative = "greater")$p.value</c>。</returns>
        Public Function FisherGreater(a As Integer, b As Integer, c As Integer, d As Integer) As Double
            If a < 0 OrElse b < 0 OrElse c < 0 OrElse d < 0 Then
                Throw New ArgumentException(
                    "All cells of the 2x2 contingency table must be non-negative. " &
                    "Please make sure that the foreground gene list is a subset of the background gene list.")
            End If

            If a + b + c + d = 0 Then
                Return 1
            End If

            Return F.FishersExact(a, b, c, d).greater_pvalue
        End Function

        ''' <summary>
        ''' 精确二项检验的上尾概率 <c>P(X &gt;= successes)</c>。
        ''' </summary>
        ''' <param name="successes">观测到的成功次数。</param>
        ''' <param name="trials">试验次数。</param>
        ''' <param name="probability">单次试验的成功概率。</param>
        ''' <returns>等价于 R 的 <c>binom.test(successes, trials, p = probability, alternative = "greater")$p.value</c>。</returns>
        Public Function BinomialUpperTail(successes As Integer, trials As Integer, probability As Double) As Double
            If trials <= 0 Then
                Return 1
            End If
            If successes <= 0 Then
                Return 1
            End If
            If successes > trials Then
                Return 0
            End If
            If probability <= 0 Then
                Return 0
            End If
            If probability >= 1 Then
                Return 1
            End If

            Dim logP As Double = std.Log(probability)
            Dim logQ As Double = std.Log(1 - probability)
            Dim logs As New List(Of Double)

            For i As Integer = successes To trials
                logs.Add(LogChoose(trials, i) + i * logP + (trials - i) * logQ)
            Next

            Dim total As Double = LogSumExp(logs)

            Return If(Double.IsNegativeInfinity(total), 0, std.Min(1.0, std.Exp(total)))
        End Function

        ''' <summary>
        ''' 2x2 列联表的期望频数最小值。
        ''' </summary>
        ''' <param name="a">左上格。</param>
        ''' <param name="b">右上格。</param>
        ''' <param name="c">左下格。</param>
        ''' <param name="d">右下格。</param>
        ''' <returns>四个期望频数之中的最小值。</returns>
        Public Function MinimumExpectedFrequency(a As Integer, b As Integer, c As Integer, d As Integer) As Double
            Dim total As Double = a + b + c + d

            If total = 0 Then
                Return 0
            End If

            Dim row1 As Double = a + b
            Dim row2 As Double = c + d
            Dim col1 As Double = a + c
            Dim col2 As Double = b + d

            Return std.Min(std.Min(row1 * col1, row1 * col2), std.Min(row2 * col1, row2 * col2)) / total
        End Function

        ''' <summary>
        ''' 带 Yates 连续性校正的 2x2 卡方检验 p 值。
        ''' </summary>
        ''' <param name="a">左上格。</param>
        ''' <param name="b">右上格。</param>
        ''' <param name="c">左下格。</param>
        ''' <param name="d">右下格。</param>
        ''' <returns>等价于 R 的 <c>chisq.test(matrix(c(a, b, c, d), nc = 2))$p.value</c>。</returns>
        ''' <remarks>
        ''' 使用 2x2 表的等价公式
        ''' <c>χ² = N (|ad - bc| - N/2)² / (r1 r2 c1 c2)</c>，
        ''' 当 <c>|ad - bc| &lt; N/2</c> 时统计量截断为 0。
        ''' </remarks>
        Public Function ChiSquareYates(a As Integer, b As Integer, c As Integer, d As Integer) As Double
            Dim total As Double = a + b + c + d

            If total = 0 Then
                Return 1
            End If

            Dim row1 As Double = a + b
            Dim row2 As Double = c + d
            Dim col1 As Double = a + c
            Dim col2 As Double = b + d
            Dim denominator As Double = row1 * row2 * col1 * col2

            If denominator = 0 Then
                Return 1
            End If

            Dim delta As Double = std.Abs(CDbl(a) * d - CDbl(b) * c) - total / 2

            If delta < 0 Then
                delta = 0
            End If

            Dim chiSquare As Double = total * delta * delta / denominator

            Return Distribution.ChiSquare(chiSquare, 1)
        End Function

        ''' <summary>
        ''' 改进的卡方检验：期望频数过小时自动退回 Fisher 精确检验（KOBAS 的
        ''' <c>improved_chisq_test</c>）。
        ''' </summary>
        ''' <param name="a">左上格。</param>
        ''' <param name="b">右上格。</param>
        ''' <param name="c">左下格。</param>
        ''' <param name="d">右下格。</param>
        ''' <returns>卡方检验或 Fisher 精确检验的 p 值。</returns>
        Public Function ImprovedChiSquare(a As Integer, b As Integer, c As Integer, d As Integer) As Double
            If MinimumExpectedFrequency(a, b, c, d) < ChiSquareExpectedThreshold Then
                Return FisherGreater(a, b, c, d)
            End If

            Return ChiSquareYates(a, b, c, d)
        End Function
    End Module
End Namespace
