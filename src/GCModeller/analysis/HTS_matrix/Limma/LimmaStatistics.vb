Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.Statistics.Linq
Imports std = System.Math
Imports cephes = Microsoft.VisualBasic.Math.Statistics.SpecialFunctions

''' <summary>
''' limma 经验贝叶斯调节所需要的统计基元。
''' </summary>
''' <remarks>
''' <para>
''' 这些函数是照 R/Bioconductor 的 <c>limma</c> 实现逐条移植过来的，目的只有一个：
''' 让 <see cref="Limma.LmFit"/> 的输出能够与 R 端 <c>lmFit</c> + <c>eBayes</c> 的结果对拍
''' （见 <c>test/R/hts_matrix_reference.R</c>）。涉及的 R 侧实现：
''' <c>limma::fitFDist</c>、<c>limma:::.squeezeVar</c>、<c>limma:::.ebayes</c>、
''' <c>limma:::tmixture.vector</c>、<c>limma:::trigammaInverse</c> 以及 <c>stats::p.adjust(...,"BH")</c>。
''' </para>
''' <para>
''' **为什么不用 <see cref="Microsoft.VisualBasic.Math.Statistics.Hypothesis.t.Pvalue"/>**：
''' 那个实现用 10000 步的数值积分计算 t 分布的 CDF，再用 <c>1 - cdf</c> 求双尾概率，
''' 在 limma 这类会产生 <c>1e-10</c> 级别 p 值的场景下既有精度问题（尾部落到 0）、
''' 又有性能问题（每个基因一次积分）。这里统一改用正则化不完全贝塔函数：
''' <c>P(|T| &gt; |t|) = I_x(df/2, 1/2)</c>，其中 <c>x = df / (df + t²)</c>，
''' 与 R 的 <c>2 * pt(-abs(t), df)</c> 数值一致。
''' </para>
''' </remarks>
Friend Module LimmaStatistics

    ''' <summary>
    ''' 双伽玛函数 ψ(x) = d/dx lnΓ(x)
    ''' </summary>
    ''' <remarks>递推把参数推到 6 以上之后用渐近展开，双精度下误差在 1e-15 量级。</remarks>
    Public Function Digamma(x As Double) As Double
        If x <= 0 Then
            Return Double.NaN
        End If

        Dim result As Double = 0

        While x < 6.0
            result -= 1.0 / x
            x += 1.0
        End While

        Dim inv As Double = 1.0 / x
        Dim inv2 As Double = inv * inv

        result += std.Log(x) - 0.5 * inv -
            inv2 * (1.0 / 12.0 -
            inv2 * (1.0 / 120.0 -
            inv2 * (1.0 / 252.0 -
            inv2 * (1.0 / 240.0 -
            inv2 * (1.0 / 132.0)))))

        Return result
    End Function

    ''' <summary>
    ''' 三伽玛函数 ψ₁(x) = d²/dx² lnΓ(x)
    ''' </summary>
    Public Function Trigamma(x As Double) As Double
        If x <= 0 Then
            Return Double.NaN
        End If

        Dim result As Double = 0

        While x < 6.0
            result += 1.0 / (x * x)
            x += 1.0
        End While

        Dim inv As Double = 1.0 / x
        Dim inv2 As Double = inv * inv
        Dim inv3 As Double = inv2 * inv
        Dim inv5 As Double = inv3 * inv2
        Dim inv7 As Double = inv5 * inv2
        Dim inv9 As Double = inv7 * inv2
        Dim inv11 As Double = inv9 * inv2

        result += inv + 0.5 * inv2 + inv3 / 6.0 -
            inv5 / 30.0 + inv7 / 42.0 - inv9 / 30.0 + 5.0 * inv11 / 66.0

        Return result
    End Function

    ''' <summary>
    ''' 求解 <c>Trigamma(y) = x</c> 的 y（对应 R 的 <c>limma:::trigammaInverse</c>）
    ''' </summary>
    ''' <param name="x">目标值，必须为正</param>
    ''' <remarks>
    ''' limma 用牛顿迭代（需要四伽玛函数）；这里改成对单调递减的 ψ₁ 做二分，
    ''' 结果一致但不需要额外实现 polygamma(2)。
    ''' </remarks>
    Public Function TrigammaInverse(x As Double) As Double
        If Double.IsNaN(x) OrElse x <= 0 Then
            Return Double.NaN
        End If
        If x > 10000000.0 Then
            Return 1.0 / std.Sqrt(x)
        End If
        If x < 0.000001 Then
            Return 1.0 / x
        End If

        Dim lo As Double = 1.0E-12
        Dim hi As Double = 1.0E+12

        For iter As Integer = 0 To 200
            Dim mid As Double = std.Sqrt(lo * hi)

            If Trigamma(mid) > x Then
                lo = mid
            Else
                hi = mid
            End If
        Next

        Return std.Sqrt(lo * hi)
    End Function

    ''' <summary>
    ''' 双尾 t 检验概率，对应 R 的 <c>2 * pt(-abs(t), df)</c>
    ''' </summary>
    Public Function TwoSidedPvalue(t As Double, df As Double) As Double
        If Double.IsNaN(t) OrElse Double.IsNaN(df) Then
            Return Double.NaN
        End If
        If Double.IsInfinity(t) Then
            Return 0.0
        End If
        If t = 0.0 Then
            Return 1.0
        End If

        Dim x As Double = df / (df + t * t)

        If x >= 1.0 Then
            Return 1.0
        ElseIf x <= 0.0 Then
            Return 0.0
        Else
            Return cephes.RegularizedIncompleteBetaFunction(df / 2.0, 0.5, x)
        End If
    End Function

    ''' <summary>
    ''' 上尾概率 <c>P(T &gt; t)</c>，对应 R 的 <c>pt(t, df, lower.tail=FALSE)</c>
    ''' </summary>
    Public Function UpperTailPvalue(t As Double, df As Double) As Double
        Dim two As Double = TwoSidedPvalue(t, df)

        If Double.IsNaN(two) Then
            Return Double.NaN
        ElseIf t >= 0 Then
            Return two / 2.0
        Else
            Return 1.0 - two / 2.0
        End If
    End Function

    ''' <summary>
    ''' 上尾分位数，对应 R 的 <c>qt(p, df, lower.tail=FALSE)</c>（要求 <paramref name="p"/> 较小）
    ''' </summary>
    Public Function UpperTailQuantile(p As Double, df As Double) As Double
        If Double.IsNaN(p) OrElse p <= 0 Then
            Return Double.PositiveInfinity
        End If

        If p > 0.5 Then
            ' 上尾概率大于 0.5 时，分位数为负数
            Return -UpperTailQuantile(1.0 - p, df)
        End If

        Dim lo As Double = 0.0
        Dim hi As Double = 1.0

        While UpperTailPvalue(hi, df) > p AndAlso hi < 1.0E+12
            hi *= 2.0
        End While

        For iter As Integer = 0 To 200
            Dim mid As Double = (lo + hi) / 2.0

            If UpperTailPvalue(mid, df) > p Then
                lo = mid
            Else
                hi = mid
            End If
        Next

        Return (lo + hi) / 2.0
    End Function

    ''' <summary>
    ''' <c>logmdigamma(x) = log(x) − ψ(x)</c>，与 limma 内部的定义一致
    ''' </summary>
    Public Function LogMDigamma(x As Double) As Double
        Return std.Log(x) - Digamma(x)
    End Function

    ''' <summary>
    ''' 经验贝叶斯方差收缩：移植 <c>limma::fitFDist</c>（等自由度分支）与 <c>limma:::.squeezeVar</c>
    ''' </summary>
    ''' <param name="variances">每个基因的残差方差</param>
    ''' <param name="df">残差自由度（所有基因相同）</param>
    ''' <returns>
    ''' <c>s2Prior</c> = 先验方差；<c>dfPrior</c> = 先验自由度。
    ''' 当 <c>dfPrior = 0</c> 时表示没有收缩（后验方差等于样本方差）。
    ''' </returns>
    ''' <remarks>
    ''' 注意 limma 会把零方差偏移到 <c>1e-5 × median</c> 处，并且用
    ''' <c>median</c> 而非 <c>mean</c> 做尺度基准 —— 这两步对后续 <c>t</c> 统计量的数值影响明显，
    ''' 这里原样保留。
    ''' </remarks>
    Public Function SqueezeVar(variances As Double(), df As Double) As (s2Prior As Double, dfPrior As Double)
        Dim n As Integer = variances.Length

        If n < 3 Then
            ' limma: 基因数太少，不做收缩（后验方差 = 样本方差）
            Return (0.0, 0.0)
        End If
        If df <= 0 Then
            Return (0.0, 0.0)
        End If

        Dim x As Double() = New Double(n - 1) {}
        Dim nok As Integer = 0

        For i As Integer = 0 To n - 1
            If Not Double.IsNaN(variances(i)) AndAlso Not Double.IsInfinity(variances(i)) AndAlso variances(i) > -0.000000000000001 Then
                x(nok) = std.Max(variances(i), 0.0)
                nok += 1
            End If
        Next

        If nok < 3 Then
            Return (0.0, 0.0)
        End If

        If nok < n Then
            Call Array.Resize(x, nok)
        End If

        Dim m As Double = x.Median

        If m = 0 Then
            m = 1.0
        End If

        For i As Integer = 0 To x.Length - 1
            x(i) = std.Max(x(i), 0.00001 * m)
        Next

        Dim e As Double() = New Double(x.Length - 1) {}
        Dim logmd As Double = LogMDigamma(df / 2.0)

        For i As Integer = 0 To x.Length - 1
            e(i) = std.Log(x(i)) + logmd
        Next

        Dim emean As Double = e.Average
        Dim evar As Double = 0

        For i As Integer = 0 To e.Length - 1
            evar += (e(i) - emean) ^ 2
        Next

        evar = evar / (e.Length - 1) - Trigamma(df / 2.0)

        If evar > 0 Then
            Dim df2 As Double = 2.0 * TrigammaInverse(evar)
            Dim s20 As Double = std.Exp(emean - LogMDigamma(df2 / 2.0))

            Return (s20, df2)
        Else
            ' 先验自由度退化为无穷大：所有基因共享同一个后验方差
            Return (x.Average, Double.PositiveInfinity)
        End If
    End Function

    ''' <summary>
    ''' 移植 <c>limma:::tmixture.vector</c>：估计用于计算 <c>lods</c>（B 统计量）的先验方差
    ''' </summary>
    ''' <param name="tstat">调节后的 t 统计量</param>
    ''' <param name="dfTotal">后验自由度（所有基因相同）</param>
    ''' <param name="stdevUnscaled">未缩放的标准误</param>
    ''' <param name="proportion">差异表达基因的先验比例（limma 默认 0.01）</param>
    ''' <param name="v0Low">先验方差下界</param>
    ''' <param name="v0High">先验方差上界</param>
    Public Function TmixtureVector(tstat As Double(),
                                   dfTotal As Double,
                                   stdevUnscaled As Double,
                                   proportion As Double,
                                   v0Low As Double,
                                   v0High As Double) As Double

        Dim ngenes As Integer = tstat.Length
        Dim ntarget As Integer = CInt(std.Ceiling(proportion / 2.0 * ngenes))

        If ntarget < 1 Then
            Return Double.NaN
        End If

        Dim p As Double = std.Max(ntarget / CDbl(ngenes), proportion)
        Dim absT As Double() = New Double(ngenes - 1) {}

        For i As Integer = 0 To ngenes - 1
            absT(i) = std.Abs(tstat(i))
        Next

        Dim order As (x As Double, i As Integer)() = absT _
            .Select(Function(x, i) (x, i)) _
            .OrderByDescending(Function(a) a.x) _
            .Take(ntarget) _
            .ToArray

        Dim v1 As Double = stdevUnscaled * stdevUnscaled
        Dim sum As Double = 0

        For k As Integer = 0 To ntarget - 1
            Dim t As Double = order(k).x
            Dim p0 As Double = 2.0 * UpperTailPvalue(t, dfTotal)
            Dim ptarget As Double = ((k + 0.5) / ngenes - (1.0 - p) * p0) / p
            Dim v0 As Double = 0

            If ptarget > p0 AndAlso ptarget > 0 Then
                Dim q As Double = UpperTailQuantile(ptarget / 2.0, dfTotal)

                If q <> 0 AndAlso Not Double.IsInfinity(q) Then
                    v0 = v1 * ((t / q) ^ 2 - 1.0)
                End If
            End If

            sum += std.Min(std.Max(v0, v0Low), v0High)
        Next

        Return sum / ntarget
    End Function

    ''' <summary>
    ''' 移植 <c>limma:::.ebayes</c> 之中 <c>lods</c> 的计算（即 limma 输出的 <c>B</c> 列）
    ''' </summary>
    ''' <param name="tstat">调节后的 t 统计量</param>
    ''' <param name="dfTotal">后验自由度</param>
    ''' <param name="stdevUnscaled">未缩放标准误</param>
    ''' <param name="s2Prior">先验方差</param>
    ''' <param name="dfPrior">先验自由度</param>
    ''' <param name="proportion">先验比例</param>
    ''' <returns>每个基因的对数后验几率</returns>
    Public Function Lods(tstat As Double(),
                         dfTotal As Double,
                         stdevUnscaled As Double,
                         s2Prior As Double,
                         dfPrior As Double,
                         Optional proportion As Double = 0.01) As Double()

        Const stdevCoefLow As Double = 0.1
        Const stdevCoefHigh As Double = 4.0

        Dim priorLow As Double = stdevCoefLow * stdevCoefLow / s2Prior
        Dim priorHigh As Double = stdevCoefHigh * stdevCoefHigh / s2Prior

        If Double.IsInfinity(dfPrior) OrElse s2Prior <= 0 Then
            priorLow = 0.0
            priorHigh = Double.MaxValue
        End If

        Dim varPrior As Double = TmixtureVector(tstat, dfTotal, stdevUnscaled, proportion, priorLow, priorHigh)

        If Double.IsNaN(varPrior) Then
            ' 目标基因数不足（limma 这里会把 var.prior 设置为 1/s2.prior）
            varPrior = 1.0 / s2Prior
        End If

        Dim r As Double = (stdevUnscaled * stdevUnscaled + varPrior) / (stdevUnscaled * stdevUnscaled)
        Dim result As Double() = New Double(tstat.Length - 1) {}
        Dim logPriorOdds As Double = std.Log(proportion / (1.0 - proportion))

        For i As Integer = 0 To tstat.Length - 1
            Dim t2 As Double = tstat(i) * tstat(i)
            Dim kernel As Double

            If dfPrior > 1000000.0 Then
                kernel = t2 * (1.0 - 1.0 / r) / 2.0
            Else
                kernel = (1.0 + dfTotal) / 2.0 * std.Log((t2 + dfTotal) / (t2 / r + dfTotal))
            End If

            result(i) = logPriorOdds - std.Log(r) / 2.0 + kernel
        Next

        Return result
    End Function

    ''' <summary>
    ''' Benjamini-Hochberg 多重检验校正，与 R 的 <c>p.adjust(p, method="BH")</c> 一致
    ''' </summary>
    ''' <param name="pvalues">原始 p 值</param>
    ''' <returns>与输入顺序一致的校正后 p 值</returns>
    ''' <remarks>
    ''' 与运行时提供的 <c>FDR</c> 扩展相比，这里补上了 R 版本的两个关键步骤：
    ''' 从最大 p 值向小端做单调化（<c>cummin</c>），以及把结果截断到 1 以内。
    ''' 缺少单调化会产生「更小的原始 p 值得出更大的校正 p 值」这种非物理结果。
    ''' </remarks>
    Public Function BHAdjust(pvalues As Double()) As Double()
        Dim n As Integer = pvalues.Length
        Dim result As Double() = New Double(n - 1) {}
        Dim order As Integer() = Enumerable.Range(0, n) _
            .OrderBy(Function(i) pvalues(i)) _
            .ToArray

        Dim running As Double = 1.0

        For rank As Integer = n To 1 Step -1
            Dim i As Integer = order(rank - 1)
            Dim adjusted As Double = n * pvalues(i) / rank

            If adjusted > 1.0 Then
                adjusted = 1.0
            End If
            If adjusted < running Then
                running = adjusted
            End If

            result(i) = running
        Next

        Return result
    End Function

End Module
