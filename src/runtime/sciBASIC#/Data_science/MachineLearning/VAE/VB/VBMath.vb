' ============================================================================
' VBMath.vb - 变分贝叶斯模型族共享的数学层
'
' 该模块补齐了 sciBASIC# 之中缺失的特殊函数（digamma / trigamma），
' 并提供对数域稳定计算（logSumExp / 行归一化）与若干矩阵辅助函数。
'
' 依赖：
'   - SpecialFunctions.gammaln  (Microsoft.VisualBasic.Math.Statistics)
'   - Microsoft.VisualBasic.MachineLearning.TensorFlow.Tensor
'
' 参考：
'   - Bishop, "Pattern Recognition and Machine Learning", 附录 B / 第 10 章
'   - Abramowitz & Stegun, "Handbook of Mathematical Functions", 6.3 / 6.4
' ============================================================================

Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.Math.Statistics
Imports std = System.Math

Namespace VariationalBayes

    ''' <summary>
    ''' 变分贝叶斯模型族共享的数学函数库。
    ''' </summary>
    ''' <remarks>
    ''' 所有函数都是无状态的，可以直接在迭代循环的热路径上面调用。
    ''' 当输入非法（NaN / 非正数 / 维度不匹配）的时候直接抛出异常，
    ''' 而不是静默返回一个看似合理的结果。
    ''' </remarks>
    Public Module VBMath

        ''' <summary>
        ''' 欧拉-马歇罗尼常数 γ
        ''' </summary>
        Public Const EulerGamma As Double = 0.5772156649015328606R

        ''' <summary>
        ''' 判定数值是否为零的下限
        ''' </summary>
        Public Const TinyValue As Double = 1.0E-300

        ''' <summary>
        ''' 正定矩阵求逆失败的时候所使用的默认抖动系数
        ''' </summary>
        Public Const DefaultJitter As Double = 0.00000001R

#Region "Gamma 家族特殊函数"

        ''' <summary>
        ''' 对数 Gamma 函数 ln Γ(x)，委托给统计库的 <c>SpecialFunctions.gammaln</c>。
        ''' </summary>
        Public Function LogGamma(x As Double) As Double
            If x <= 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(x), x, "ln Γ(x) 要求 x > 0")
            End If

            Return SpecialFunctions.gammaln(x)
        End Function

        ''' <summary>
        ''' 双伽马函数 ψ(x) = d/dx ln Γ(x)。
        ''' </summary>
        ''' <remarks>
        ''' 通过递推关系 ψ(x) = ψ(x+1) - 1/x 把参数抬升到 x >= 6，
        ''' 然后使用渐近展开式：
        ''' ψ(x) ~ ln x - 1/(2x) - Σ B_2k / (2k x^2k)
        ''' 对于非正参数使用反射公式 ψ(1-x) - ψ(x) = π·cot(πx)。
        ''' </remarks>
        Public Function Digamma(x As Double) As Double
            If Double.IsNaN(x) Then
                Return Double.NaN
            End If

            If Double.IsPositiveInfinity(x) Then
                Return Double.PositiveInfinity
            End If

            If x <= 0 Then
                ' 非正整数是极点
                If std.Abs(x - std.Round(x)) < 0.000000000001R Then
                    Return Double.NaN
                End If

                ' 反射公式
                Return Digamma(1.0R - x) - std.PI / std.Tan(std.PI * x)
            End If

            Dim acc As Double = 0

            While x < 6.0R
                acc -= 1.0R / x
                x += 1.0R
            End While

            Dim inv As Double = 1.0R / x
            Dim inv2 As Double = inv * inv
            ' Bernoulli 项：B2=1/6, B4=-1/30, B6=1/42, B8=-1/30, B10=5/66, B12=-691/2730
            Dim series As Double =
                inv2 * (1.0R / 12.0R -
                inv2 * (1.0R / 120.0R -
                inv2 * (1.0R / 252.0R -
                inv2 * (1.0R / 240.0R -
                inv2 * (1.0R / 132.0R -
                inv2 * (691.0R / 32760.0R))))))

            Return acc + std.Log(x) - 0.5R * inv - series
        End Function

        ''' <summary>
        ''' 三伽马函数 ψ'(x) = d²/dx² ln Γ(x)。
        ''' </summary>
        ''' <remarks>
        ''' 使用递推 ψ'(x) = ψ'(x+1) + 1/x² 与渐近展开：
        ''' ψ'(x) ~ 1/x + 1/(2x²) + 1/(6x³) - 1/(30x⁵) + 1/(42x⁷) - 1/(30x⁹) + 5/(66x¹¹)
        ''' </remarks>
        Public Function Trigamma(x As Double) As Double
            If Double.IsNaN(x) Then
                Return Double.NaN
            End If

            If x <= 0 Then
                If std.Abs(x - std.Round(x)) < 0.000000000001R Then
                    Return Double.NaN
                End If

                Dim sinValue As Double = std.Sin(std.PI * x)

                Return Trigamma(1.0R - x) + std.PI * std.PI / (sinValue * sinValue)
            End If

            Dim acc As Double = 0

            While x < 6.0R
                acc += 1.0R / (x * x)
                x += 1.0R
            End While

            Dim inv As Double = 1.0R / x
            Dim inv2 As Double = inv * inv
            Dim series As Double =
                inv +
                0.5R * inv * inv +
                (1.0R / 6.0R) * inv * inv2 -
                (1.0R / 30.0R) * inv * inv2 * inv2 +
                (1.0R / 42.0R) * inv * inv2 * inv2 * inv2 -
                (1.0R / 30.0R) * inv * inv2 * inv2 * inv2 * inv2 +
                (5.0R / 66.0R) * inv * inv2 * inv2 * inv2 * inv2 * inv2

            Return acc + series
        End Function

        ''' <summary>
        ''' 对数 Beta 函数 ln B(a, b) = ln Γ(a) + ln Γ(b) - ln Γ(a+b)
        ''' </summary>
        Public Function LogBeta(a As Double, b As Double) As Double
            Return LogGamma(a) + LogGamma(b) - LogGamma(a + b)
        End Function

        ''' <summary>
        ''' 多项 Beta 函数 B(α) 的对数值：
        ''' ln B(α) = Σ ln Γ(α_k) - ln Γ(Σ α_k)
        ''' </summary>
        Public Function LogMultiBeta(alpha As Double()) As Double
            If alpha Is Nothing OrElse alpha.Length = 0 Then
                Throw New ArgumentException("alpha 不能为空", NameOf(alpha))
            End If

            Dim sumAlpha As Double = 0
            Dim acc As Double = 0

            For Each a As Double In alpha
                If a <= 0 Then
                    Throw New ArgumentOutOfRangeException(NameOf(alpha), a, "Dirichlet 参数必须为正数")
                End If

                sumAlpha += a
                acc += LogGamma(a)
            Next

            Return acc - LogGamma(sumAlpha)
        End Function

        ''' <summary>
        ''' Wishart 分布归一化常数的对数值：
        ''' ln B(W, ν) = -(ν/2)·ln|W| - (νD/2)·ln2 - Σ_{d=1..D} ln Γ((ν+1-d)/2)
        ''' </summary>
        ''' <param name="logDetW">ln|W|</param>
        ''' <param name="nu">自由度 ν</param>
        ''' <param name="D">维度 D</param>
        Public Function LogWishartNormalizer(logDetW As Double, nu As Double, D As Integer) As Double
            If D < 1 Then
                Throw New ArgumentOutOfRangeException(NameOf(D), D, "维度必须为正数")
            End If

            Dim acc As Double = -0.5R * nu * logDetW - 0.5R * nu * D * std.Log(2.0R)

            For k As Integer = 1 To D
                acc -= LogGamma(0.5R * (nu + 1.0R - k))
            Next

            Return acc
        End Function

#End Region

#Region "对数域稳定计算"

        ''' <summary>
        ''' 对数求和指数 ln Σ exp(values_i)，使用最大值平移避免上溢/下溢。
        ''' </summary>
        Public Function LogSumExp(values As Double()) As Double
            If values Is Nothing OrElse values.Length = 0 Then
                Return Double.NegativeInfinity
            End If

            Return LogSumExp(values, 0, values.Length)
        End Function

        ''' <summary>
        ''' 对数求和指数 ln Σ exp(values_i)，只处理 [start, start+length) 区间。
        ''' </summary>
        Public Function LogSumExp(values As Double(), start As Integer, length As Integer) As Double
            If values Is Nothing OrElse length <= 0 Then
                Return Double.NegativeInfinity
            End If

            Dim maxValue As Double = Double.NegativeInfinity

            For i As Integer = start To start + length - 1
                If values(i) > maxValue Then
                    maxValue = values(i)
                End If
            Next

            If Double.IsNegativeInfinity(maxValue) Then
                Return Double.NegativeInfinity
            End If

            If Double.IsPositiveInfinity(maxValue) Then
                Return maxValue
            End If

            Dim sum As Double = 0

            For i As Integer = start To start + length - 1
                sum += std.Exp(values(i) - maxValue)
            Next

            If sum <= 0 Then
                Return Double.NegativeInfinity
            End If

            Return maxValue + std.Log(sum)
        End Function

        ''' <summary>
        ''' 对二维数组的每一行做 logSumExp。
        ''' </summary>
        ''' <param name="logValues">[rows, cols] 的对数值</param>
        ''' <returns>长度为 rows 的行归一化常数</returns>
        Public Function LogSumExpRows(logValues As Double(,),
                                      Optional start As Integer = 0,
                                      Optional length As Integer = -1) As Double()
            If logValues Is Nothing Then
                Throw New ArgumentNullException(NameOf(logValues))
            End If

            Dim rows As Integer = logValues.GetLength(0)
            Dim cols As Integer = logValues.GetLength(1)

            If length < 0 Then
                length = cols
            End If

            If start < 0 OrElse length <= 0 OrElse (start + length) > cols Then
                Throw New ArgumentOutOfRangeException(NameOf(length), "非法的列区间")
            End If

            Dim result As Double() = New Double(rows - 1) {}

            For i As Integer = 0 To rows - 1
                Dim maxValue As Double = Double.NegativeInfinity

                For j As Integer = start To start + length - 1
                    If logValues(i, j) > maxValue Then
                        maxValue = logValues(i, j)
                    End If
                Next

                If Double.IsNegativeInfinity(maxValue) Then
                    result(i) = maxValue
                    Continue For
                End If

                Dim sum As Double = 0

                For j As Integer = start To start + length - 1
                    sum += std.Exp(logValues(i, j) - maxValue)
                Next

                result(i) = If(sum <= 0, Double.NegativeInfinity, maxValue + std.Log(sum))
            Next

            Return result
        End Function

        ''' <summary>
        ''' 对数域的行 softmax：每一行减去自身的 logSumExp 之后再取指数。
        ''' </summary>
        ''' <param name="logValues">[rows, cols] 的对数值，函数不会修改输入</param>
        ''' <param name="logNorm">输出的每一行的 logSumExp</param>
        ''' <returns>[rows, cols] 的概率值，每一行的和为 1</returns>
        Public Function SoftmaxRows(logValues As Double(,), ByRef logNorm As Double()) As Double(,)
            If logValues Is Nothing Then
                Throw New ArgumentNullException(NameOf(logValues))
            End If

            Dim rows As Integer = logValues.GetLength(0)
            Dim cols As Integer = logValues.GetLength(1)

            logNorm = LogSumExpRows(logValues)

            Dim result As Double(,) = New Double(rows - 1, cols - 1) {}

            For i As Integer = 0 To rows - 1
                Dim norm As Double = logNorm(i)

                If Double.IsNegativeInfinity(norm) Then
                    ' 整行都是 -Inf：退化为均匀分布，保证责任度仍然合法
                    Dim uniform As Double = 1.0R / cols

                    For j As Integer = 0 To cols - 1
                        result(i, j) = uniform
                    Next

                    logNorm(i) = std.Log(uniform)
                    Continue For
                End If

                For j As Integer = 0 To cols - 1
                    result(i, j) = std.Exp(logValues(i, j) - norm)
                Next
            Next

            Return result
        End Function

        ''' <summary>
        ''' 安全版的 ln(1 + exp(x))，在 x 很大或者很小时都不会溢出。
        ''' </summary>
        Public Function Log1pExp(x As Double) As Double
            If x > 0 Then
                Return x + std.Log(1.0R + std.Exp(-x))
            Else
                Return std.Log(1.0R + std.Exp(x))
            End If
        End Function

#End Region

#Region "张量辅助"

        ''' <summary>
        ''' 矩阵的迹 tr(A)。
        ''' </summary>
        Public Function Trace(a As Tensor) As Double
            If a Is Nothing Then
                Throw New ArgumentNullException(NameOf(a))
            End If

            Dim rows As Integer = a.Shape(0)
            Dim cols As Integer = a.Shape(1)
            Dim n As Integer = std.Min(rows, cols)
            Dim acc As Double = 0

            For i As Integer = 0 To n - 1
                acc += a(i, i)
            Next

            Return acc
        End Function

        ''' <summary>
        ''' 返回一个严格对称的副本 (A + A') / 2，用于消除数值误差造成的非对称性。
        ''' </summary>
        Public Function Symmetrize(a As Tensor) As Tensor
            If a Is Nothing Then
                Throw New ArgumentNullException(NameOf(a))
            End If

            Dim rows As Integer = a.Shape(0)
            Dim cols As Integer = a.Shape(1)

            If rows <> cols Then
                Throw New ArgumentException("对称化操作要求方阵", NameOf(a))
            End If

            Dim data As Double() = New Double(rows * cols - 1) {}

            For i As Integer = 0 To rows - 1
                For j As Integer = 0 To cols - 1
                    data(i * cols + j) = 0.5R * (a(i, j) + a(j, i))
                Next
            Next

            Return New Tensor(data, rows, cols)
        End Function

        ''' <summary>
        ''' 计算二次型 x' A x。
        ''' </summary>
        Public Function QuadraticForm(x As Double(), a As Tensor) As Double
            If x Is Nothing Then
                Throw New ArgumentNullException(NameOf(x))
            End If

            Dim n As Integer = x.Length
            Dim acc As Double = 0

            For i As Integer = 0 To n - 1
                Dim rowSum As Double = 0

                For j As Integer = 0 To n - 1
                    rowSum += a(i, j) * x(j)
                Next

                acc += x(i) * rowSum
            Next

            Return acc
        End Function

        ''' <summary>
        ''' 对张量的所有元素求和。
        ''' </summary>
        Public Function TotalSum(a As Tensor) As Double
            If a Is Nothing Then
                Throw New ArgumentNullException(NameOf(a))
            End If

            Dim data As Double() = a.Data
            Dim acc As Double = 0

            For i As Integer = 0 To data.Length - 1
                acc += data(i)
            Next

            Return acc
        End Function

#End Region

#Region "自检"

        ''' <summary>
        ''' 与 <see cref="LogGamma"/> 的差分关系做一致性自检：
        ''' ψ(x) ≈ ln Γ(x+ε) - ln Γ(x) / ε（中心差分），返回最大绝对误差。
        ''' </summary>
        ''' <remarks>
        ''' 该函数只用于单元测试与调试诊断，不会在训练过程中被调用。
        ''' </remarks>
        Public Function DigammaConsistencyError(Optional x As Double = 3.7R,
                                                Optional epsilon As Double = 1.0E-5R) As Double
            Dim numeric As Double =
                (LogGamma(x + epsilon) - LogGamma(x - epsilon)) / (2.0R * epsilon)

            Return std.Abs(Digamma(x) - numeric)
        End Function

#End Region

    End Module

End Namespace
