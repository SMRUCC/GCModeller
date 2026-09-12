Namespace Evolution.MaximumLikelihood

    ''' <summary>
    ''' 离散 Gamma 速率异质性模型（Yang 1994）：将位点间的替换速率差异用一个离散化的
    ''' Gamma 分布（形状参数 α，均值归一为 1）近似，并可附加不变位点比例 I。
    ''' </summary>
    ''' <remarks>
    ''' 每个速率类别的概率相等（1/K），类别速率 <c>r_k = K * ∫_{q_{k-1}}^{q_k} x f(x) dx</c>，
    ''' 其中 <c>q_k</c> 是 Gamma(α, α) 的 1/K 分位数。当存在不变位点比例 <c>p_inv</c> 时，
    ''' 额外增加一个速率为 0 的类别，并把可变类别的速率整体放大 <c>1/(1 - p_inv)</c>，
    ''' 从而保证全模型的平均速率为 1。
    ''' </remarks>
    Public Class DiscreteGamma

        Public ReadOnly Property Shape As Double
        Public ReadOnly Property Categories As Integer
        Public ReadOnly Property InvariantProportion As Double

        ''' <summary>
        ''' 各速率类别的速率值（若包含不变位点，第一个元素为 0）
        ''' </summary>
        Public ReadOnly Property Rates As Double()

        ''' <summary>
        ''' 各速率类别的权重（和为 1）
        ''' </summary>
        Public ReadOnly Property Weights As Double()

        Public Sub New(shape As Double, categories As Integer, Optional invariantProportion As Double = 0)
            If categories < 1 Then
                categories = 1
            End If
            If invariantProportion < 0 Then
                invariantProportion = 0
            ElseIf invariantProportion > 0.99 Then
                invariantProportion = 0.99
            End If

            Me.Shape = shape
            Me.Categories = categories
            Me.InvariantProportion = invariantProportion

            Dim variableRates As Double() = ComputeRates(shape, categories)
            Dim hasInvariant As Boolean = invariantProportion > 0
            Dim rateList As New List(Of Double)
            Dim weightList As New List(Of Double)

            If hasInvariant Then
                rateList.Add(0)
                weightList.Add(invariantProportion)
            End If

            Dim variableWeight As Double = (1 - invariantProportion) / categories
            Dim scale As Double = If(hasInvariant, 1 / (1 - invariantProportion), 1)

            For k As Integer = 0 To categories - 1
                rateList.Add(variableRates(k) * scale)
                weightList.Add(variableWeight)
            Next

            Me.Rates = rateList.ToArray
            Me.Weights = weightList.ToArray
        End Sub

        ''' <summary>
        ''' 计算 K 个等概率 Gamma 速率类别的速率（平均值归一为 1）。
        ''' </summary>
        Private Shared Function ComputeRates(shape As Double, categories As Integer) As Double()
            If categories <= 1 OrElse Double.IsInfinity(shape) OrElse shape <= 0 OrElse shape > 1000 Then
                ' 无速率异质性
                Dim flat(categories - 1) As Double

                For i As Integer = 0 To categories - 1
                    flat(i) = 1
                Next

                Return flat
            End If

            Dim cumulative(categories) As Double
            cumulative(0) = 0
            cumulative(categories) = 1

            For k As Integer = 1 To categories - 1
                Dim target As Double = k / categories
                Dim q As Double = GammaFunctions.InverseRegularizedGammaP(shape, target)
                cumulative(k) = GammaFunctions.RegularizedGammaP(shape + 1, shape * q)
            Next

            Dim rates(categories - 1) As Double

            For k As Integer = 0 To categories - 1
                rates(k) = categories * (cumulative(k + 1) - cumulative(k))
            Next

            Return rates
        End Function
    End Class

    ''' <summary>
    ''' 离散 Gamma 模型所需的特殊函数（Gamma 函数、正则化不完全 Gamma 函数及其反函数）。
    ''' </summary>
    ''' <remarks>
    ''' 这里采用自包含实现（Lanczos 近似的 lnΓ + 级数/连分式的不完全 Gamma + 二分法求分位数），
    ''' 以避免依赖基础库中数值行为不稳定的实现。
    ''' </remarks>
    Friend Module GammaFunctions

        Private ReadOnly LanczosG As Double = 7
        Private ReadOnly LanczosCoef As Double() = {
            0.99999999999980993,
            676.5203681218851,
            -1259.1392167224028,
            771.32342877765313,
            -176.61502916214059,
            12.507343278686905,
            -0.13857109526572012,
            9.9843695780195716E-06,
            1.5056327351493116E-07
        }

        ''' <summary>
        ''' ln Γ(x)（Lanczos 近似，g=7, n=9）
        ''' </summary>
        Public Function LogGamma(x As Double) As Double
            If x < 0.5 Then
                ' 反射公式
                Return Math.Log(Math.PI / Math.Sin(Math.PI * x)) - LogGamma(1 - x)
            End If

            x -= 1

            Dim a As Double = LanczosCoef(0)
            Dim t As Double = x + LanczosG + 0.5

            For i As Integer = 1 To 8
                a += LanczosCoef(i) / (x + i)
            Next

            Return 0.5 * Math.Log(2 * Math.PI) + (x + 0.5) * Math.Log(t) - t + Math.Log(a)
        End Function

        ''' <summary>
        ''' 正则化下不完全 Gamma 函数 <c>P(a, x) = γ(a, x) / Γ(a)</c>。
        ''' </summary>
        Public Function RegularizedGammaP(a As Double, x As Double) As Double
            If x <= 0 Then
                Return 0
            End If

            If x < a + 1 Then
                ' 级数展开
                Dim ap As Double = a
                Dim sum As Double = 1 / a
                Dim del As Double = sum

                For n As Integer = 1 To 1000
                    ap += 1
                    del *= x / ap
                    sum += del

                    If Math.Abs(del) < Math.Abs(sum) * 1.0E-15 Then
                        Exit For
                    End If
                Next

                Return sum * Math.Exp(-x + a * Math.Log(x) - LogGamma(a))
            Else
                ' 连分式求 Q(a,x)，P = 1 - Q
                Return 1 - RegularizedGammaQ(a, x)
            End If
        End Function

        ''' <summary>
        ''' 正则化上不完全 Gamma 函数 <c>Q(a, x) = 1 - P(a, x)</c>。
        ''' </summary>
        Public Function RegularizedGammaQ(a As Double, x As Double) As Double
            Const tiny As Double = 1.0E-300

            Dim b As Double = x + 1 - a
            Dim c As Double = 1 / tiny
            Dim d As Double = 1 / b
            Dim h As Double = d

            For i As Integer = 1 To 1000
                Dim an As Double = -i * (i - a)
                b += 2
                d = an * d + b

                If Math.Abs(d) < tiny Then
                    d = tiny
                End If

                c = b + an / c

                If Math.Abs(c) < tiny Then
                    c = tiny
                End If

                d = 1 / d

                Dim del As Double = d * c
                h *= del

                If Math.Abs(del - 1) < 1.0E-15 Then
                    Exit For
                End If
            Next

            Return Math.Exp(-x + a * Math.Log(x) - LogGamma(a)) * h
        End Function

        ''' <summary>
        ''' 求解 <c>P(a, x) = p</c> 的 x（即 Gamma(a, 1) 分布的 p 分位数），采用二分法。
        ''' </summary>
        Public Function InverseRegularizedGammaP(a As Double, p As Double) As Double
            If p <= 0 Then
                Return 0
            ElseIf p >= 1 Then
                Return Double.PositiveInfinity
            End If

            ' 指数式扩张寻找上界
            Dim hi As Double = Math.Max(1.0, a)
            Dim guard As Integer = 0

            While RegularizedGammaP(a, hi) < p AndAlso guard < 200
                hi *= 2
                guard += 1
            End While

            Dim lo As Double = 0

            For iter As Integer = 1 To 200
                Dim mid As Double = (lo + hi) / 2

                If RegularizedGammaP(a, mid) < p Then
                    lo = mid
                Else
                    hi = mid
                End If
            Next

            Return (lo + hi) / 2
        End Function
    End Module

End Namespace
