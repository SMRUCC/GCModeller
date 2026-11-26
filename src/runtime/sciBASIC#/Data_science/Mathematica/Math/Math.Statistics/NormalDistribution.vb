Imports std = System.Math

    ''' <summary>
    ''' 标准正态分布的高精度数值函数：误差函数、累积分布、概率密度、分位数与尾概率。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 这不是一个"分布对象"，而是一组纯函数——因为统计检验（秩和检验的 z→p 转换、
    ''' 元分析的 Stouffer 合并、copula 的正态分位）只需要数值本身，
    ''' 不需要携带均值/方差状态。
    ''' </para>
    ''' <para>
    ''' 数值方法：
    ''' </para>
    ''' <list type="bullet">
    ''' <item><description><b>erf</b>：<c>|x| &lt;= 2</c> 时用幂级数
    ''' <c>2/√π Σ (-1)^n x^(2n+1)/(n!(2n+1))</c>，逐项递推、按相对误差终止；</description></item>
    ''' <item><description><b>erfc</b>：<c>|x| &gt; 2</c> 时用连分式
    ''' <c>erfc(x) = exp(-x²)/√π · 1/(x + (1/2)/(x + 1/(x + (3/2)/(x + ...))))</c>
    ''' 的修正 Lentz 求值，避免 <c>1 - erf(x)</c> 在尾部的大数相减；</description></item>
    ''' <item><description><b>Φ⁻¹</b>：Acklam 有理逼近（相对误差 ~1.15e-9）再走一次 Halley 精化，
    ''' 收敛到双精度极限（~1e-15）。</description></item>
    ''' </list>
    ''' <para>
    ''' 之所以不复用 <c>Distributions.MethodOfMoments.Normal</c>：那个类型的 <c>GetCDF</c> 用梯形数值积分，
    ''' 且 <c>Dim inc As Double = 1 \ 250</c> 与 <c>(1 \ 2)</c> 是 VB 的整数除法（结果为 0），
    ''' 计算路径本身是失效的；<c>GetPDF</c> 的归一化常数也写成了 <c>sd²/√(2π)</c>。
    ''' 本模块是修正后的独立实现，旧的 <c>Normal</c> 类改为委托到这里。
    ''' </para>
    ''' </remarks>
    Public Module NormalDistribution

        ''' <summary>
        ''' <c>√(2π)</c>
        ''' </summary>
        Public ReadOnly Property Sqrt2Pi As Double
            Get
                Return 2.5066282746310002
            End Get
        End Property

        ''' <summary>
        ''' <c>2/√π</c>（erf 幂级数的系数）
        ''' </summary>
        Private ReadOnly TwoOverSqrtPi As Double = 2.0 / std.Sqrt(std.PI)

#Region "Error function"

        ''' <summary>
        ''' 误差函数 <c>erf(x) = 2/√π ∫₀ˣ exp(-t²) dt</c>
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns>落在 <c>[-1, 1]</c></returns>
        Public Function Erf(x As Double) As Double
            If Double.IsNaN(x) Then Return Double.NaN
            If Double.IsInfinity(x) Then Return If(x > 0, 1.0, -1.0)

            Dim a As Double = std.Abs(x)

            If a <= 2.0 Then
                Return If(x < 0, -ErfSeries(a), ErfSeries(a))
            Else
                Dim tail As Double = ErfcContinuedFraction(a)

                Return If(x < 0, tail - 1.0, 1.0 - tail)
            End If
        End Function

        ''' <summary>
        ''' 互补误差函数 <c>erfc(x) = 1 - erf(x)</c>，尾部保持相对精度
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns>落在 <c>[0, 2]</c></returns>
        Public Function Erfc(x As Double) As Double
            If Double.IsNaN(x) Then Return Double.NaN
            If Double.IsInfinity(x) Then Return If(x > 0, 0.0, 2.0)

            Dim a As Double = std.Abs(x)

            If a <= 2.0 Then
                Dim e As Double = ErfSeries(a)

                Return If(x < 0, 1.0 + e, 1.0 - e)
            Else
                Dim tail As Double = ErfcContinuedFraction(a)

                Return If(x < 0, 2.0 - tail, tail)
            End If
        End Function

        ''' <summary>
        ''' <c>|x| &lt;= 2</c> 区间的 erf 幂级数
        ''' </summary>
        ''' <param name="x">非负输入</param>
        ''' <returns></returns>
        Private Function ErfSeries(x As Double) As Double
            Dim x2 As Double = x * x
            Dim term As Double = x
            Dim sum As Double = x
            Dim n As Integer = 0

            Do
                n += 1

                ' t_n = (-1)^n x^(2n+1) / (n! (2n+1))
                ' t_(n+1) / t_n = -x² (2n+1) / ((n+1)(2n+3))
                term *= -x2 * (2.0 * n - 1.0) / (n * (2.0 * n + 1.0))
                sum += term

                If std.Abs(term) <= std.Abs(sum) * 1.0E-17 Then Exit Do
                If n >= 200 Then Exit Do
            Loop

            Return TwoOverSqrtPi * sum
        End Function

        ''' <summary>
        ''' <c>|x| &gt; 2</c> 区间的 erfc 连分式（修正 Lentz 算法）
        ''' </summary>
        ''' <param name="x">非负输入</param>
        ''' <returns></returns>
        Private Function ErfcContinuedFraction(x As Double) As Double
            Const tiny As Double = 1.0E-300

            Dim f As Double = x
            Dim c As Double = f
            Dim d As Double = 0.0
            Dim n As Integer = 0

            Do
                n += 1

                ' a_n = n / 2（a1 = 1/2, a2 = 1, a3 = 3/2, ...），b_n = x
                Dim a As Double = 0.5 * n
                Dim b As Double = x

                d = b + a * d
                If d = 0.0 Then d = tiny
                c = b + a / c
                If c = 0.0 Then c = tiny

                d = 1.0 / d

                Dim delta As Double = c * d

                f *= delta

                If std.Abs(delta - 1.0) <= 1.0E-17 Then Exit Do
                If n >= 500 Then Exit Do
            Loop

            ' 连分式求出的 f 是分母 (x + 1/2/(x + 1/(x + ...)))，因此这里要取倒数
            Return std.Exp(-x * x) / std.Sqrt(std.PI) / f
        End Function

#End Region

#Region "Standard normal distribution"

        ''' <summary>
        ''' 标准正态概率密度 <c>φ(z) = exp(-z²/2)/√(2π)</c>
        ''' </summary>
        ''' <param name="z"></param>
        ''' <returns></returns>
        Public Function PDF(z As Double) As Double
            If Double.IsNaN(z) Then Return Double.NaN

            Return std.Exp(-0.5 * z * z) / Sqrt2Pi
        End Function

        ''' <summary>
        ''' 标准正态对数密度 <c>ln φ(z) = -z²/2 - ln√(2π)</c>（大 <c>|z|</c> 时避免下溢）
        ''' </summary>
        ''' <param name="z"></param>
        ''' <returns></returns>
        Public Function LogPDF(z As Double) As Double
            Return -0.5 * z * z - std.Log(Sqrt2Pi)
        End Function

        ''' <summary>
        ''' 标准正态累积分布 <c>Φ(z) = (1 + erf(z/√2)) / 2</c>
        ''' </summary>
        ''' <param name="z"></param>
        ''' <returns></returns>
        Public Function CDF(z As Double) As Double
            If Double.IsNaN(z) Then Return Double.NaN
            If Double.IsPositiveInfinity(z) Then Return 1.0
            If Double.IsNegativeInfinity(z) Then Return 0.0

            Return 0.5 * Erfc(-z / std.Sqrt(2.0))
        End Function

        ''' <summary>
        ''' 标准正态上尾概率 <c>1 - Φ(z)</c>，在 <c>z &gt; 0</c> 的尾部保持相对精度
        ''' </summary>
        ''' <param name="z"></param>
        ''' <returns></returns>
        Public Function CCDF(z As Double) As Double
            If Double.IsNaN(z) Then Return Double.NaN
            If Double.IsPositiveInfinity(z) Then Return 0.0
            If Double.IsNegativeInfinity(z) Then Return 1.0

            Return 0.5 * Erfc(z / std.Sqrt(2.0))
        End Function

        ''' <summary>
        ''' 双侧 P 值 <c>2 · (1 - Φ(|z|))</c>
        ''' </summary>
        ''' <param name="z">正态分数</param>
        ''' <returns></returns>
        Public Function TwoSidedPValue(z As Double) As Double
            If Double.IsNaN(z) Then Return Double.NaN

            Dim p As Double = 2.0 * CCDF(std.Abs(z))

            Return std.Min(1.0, p)
        End Function

        ''' <summary>
        ''' 单侧（上尾）P 值 <c>1 - Φ(z)</c>
        ''' </summary>
        ''' <param name="z"></param>
        ''' <returns></returns>
        Public Function UpperTailPValue(z As Double) As Double
            Return CCDF(z)
        End Function

        ''' <summary>
        ''' 标准正态分位数（逆累积分布）<c>Φ⁻¹(p)</c>
        ''' </summary>
        ''' <param name="p">概率，落在 <c>(0, 1)</c>；端点返回 ∓∞</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Acklam 有理逼近 + 一次 Halley 精化。当 <paramref name="p"/> 极接近 0 或 1 时，
        ''' 精化步会因为 <c>Φ</c> 的相对精度下降而自动退化为原始逼近（不会变得更差）。
        ''' </remarks>
        Public Function InvCDF(p As Double) As Double
            If Double.IsNaN(p) Then Return Double.NaN
            If p <= 0.0 Then Return Double.NegativeInfinity
            If p >= 1.0 Then Return Double.PositiveInfinity
            If p = 0.5 Then Return 0.0

            Dim x As Double = InvCDFAcklam(p)

            ' Halley 精化：e = Φ(x) - p，u = e·√(2π)·exp(x²/2)，x -= u / (1 + x·u/2)
            Dim e As Double = CDF(x) - p

            If e = 0.0 Then Return x

            Dim u As Double = e * Sqrt2Pi * std.Exp(0.5 * x * x)

            If Double.IsNaN(u) OrElse Double.IsInfinity(u) Then Return x

            Dim delta As Double = u / (1.0 + 0.5 * x * u)

            If Double.IsNaN(delta) OrElse Double.IsInfinity(delta) Then Return x

            Return x - delta
        End Function

        ''' <summary>
        ''' 由上尾概率反解正态分数：<c>z</c> 满足 <c>1 - Φ(z) = q</c>
        ''' </summary>
        ''' <param name="q">上尾概率（通常是很小的 P 值）</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 利用对称性 <c>Φ⁻¹(1 - q) = -Φ⁻¹(q)</c>，因此<b>不会</b>构造 <c>1 - q</c>：
        ''' 当 <paramref name="q"/> 小到 <c>1e-16</c> 量级时，<c>1 - q</c> 在双精度下已经丢掉了全部有效位，
        ''' 而本实现仍然保持相对精度。做"P 值 → z"的往返时请优先使用本函数而不是 <see cref="InvCDF"/>。
        ''' </remarks>
        Public Function InvCCDF(q As Double) As Double
            If Double.IsNaN(q) Then Return Double.NaN
            If q <= 0.0 Then Return Double.PositiveInfinity
            If q >= 1.0 Then Return Double.NegativeInfinity
            If q = 0.5 Then Return 0.0

            ' q 小（上尾）时直接用对称性，避免构造 1-q；
            ' q 大（下尾，z 为负）时 1-q 在 Sterbenz 条件下是精确减法，直接解 Φ(z)=1-q
            If q <= 0.5 Then Return -InvCDF(q)

            Return InvCDF(1.0 - q)
        End Function

        ''' <summary>
        ''' 由双侧 P 值反解正态分数 <c>|z| = Φ⁻¹(1 - p/2)</c>
        ''' </summary>
        ''' <param name="p">双侧 P 值</param>
        ''' <returns>非负的 <c>|z|</c></returns>
        Public Function ZFromTwoSidedPValue(p As Double) As Double
            If Double.IsNaN(p) Then Return Double.NaN
            If p <= 0.0 Then Return Double.PositiveInfinity
            If p >= 1.0 Then Return 0.0

            Return std.Abs(InvCDF(1.0 - 0.5 * p))
        End Function

        ''' <summary>
        ''' 把双侧 P 值转换为带符号的正态分数（符号取自效应量）
        ''' </summary>
        ''' <param name="probability">双侧 P 值</param>
        ''' <param name="effect">效应量，只用于取符号</param>
        ''' <returns>P 值非法时返回 <see cref="Double.NaN"/></returns>
        Public Function SignedScore(probability As Double, effect As Double) As Double
            If Double.IsNaN(probability) OrElse Double.IsNaN(effect) Then Return Double.NaN

            Dim magnitude As Double = ZFromTwoSidedPValue(probability)

            If std.Sign(effect) < 0 Then Return -magnitude

            Return magnitude
        End Function

        ''' <summary>
        ''' Acklam 的 <c>Φ⁻¹</c> 有理逼近（相对误差约 1.15e-9）
        ''' </summary>
        ''' <param name="p"></param>
        ''' <returns></returns>
        Private Function InvCDFAcklam(p As Double) As Double
            Dim a As Double() = {
                -3.969683028665376E+01, 2.209460984245205E+02, -2.759285104469687E+02,
                1.38357751867269E+02, -3.066479806614716E+01, 2.506628277459239
            }
            Dim b As Double() = {
                -5.447609879822406E+01, 1.615858368580409E+02, -1.556989798598866E+02,
                6.680131188771972E+01, -1.328068155288572E+01
            }
            Dim c As Double() = {
                -7.784894002430293E-03, -3.223964580411365E-01, -2.400758277161838,
                -2.549732539343734, 4.374664141464968, 2.938163982698783
            }
            Dim d As Double() = {
                7.784695709041462E-03, 3.224671290700398E-01, 2.445134137142996,
                3.754408661907416
            }
            Const pLow As Double = 0.02425
            Const pHigh As Double = 1.0 - pLow

            Dim q As Double
            Dim r As Double

            If p < pLow Then
                q = std.Sqrt(-2.0 * std.Log(p))

                Return Poly(c, q) / (Poly(d, q) * q + 1.0)
            ElseIf p <= pHigh Then
                q = p - 0.5
                r = q * q

                Return Poly(a, r) * q / (Poly(b, r) * r + 1.0)
            Else
                q = std.Sqrt(-2.0 * std.Log(1.0 - p))

                Return -Poly(c, q) / (Poly(d, q) * q + 1.0)
            End If
        End Function

        ''' <summary>
        ''' Horner 求值：<c>c₀x⁴ + c₁x³ + c₂x² + c₃x + c₄</c>
        ''' </summary>
        Private Function Poly(coefficients As Double(), x As Double) As Double
            Dim y As Double = coefficients(0)

            For i As Integer = 1 To coefficients.Length - 1
                y = y * x + coefficients(i)
            Next

            Return y
        End Function

#End Region

End Module
