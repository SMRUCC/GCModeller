' ============================================================================
' VariationalGaussianMixture.vb - 变分贝叶斯高斯混合模型 (VB-GMM)
'
' 模型：
'   π   ~ Dirichlet(α0)                        混合权重
'   μ_k ~ N(m0, (β0 Λ_k)^-1)                   各成分的均值
'   Λ_k ~ Wishart(W0, ν0)                      各成分的精度矩阵
'   z_n ~ Cat(π)                               隐变量（成分指示）
'   x_n | z_nk=1 ~ N(μ_k, Λ_k^-1)
'
' 均值场近似：
'   q(Z)     = Π_n Π_k r_nk^{z_nk}
'   q(π)     = Dirichlet(α)
'   q(μ, Λ)  = Π_k N(μ_k | m_k, (β_k Λ_k)^-1) · Wishart(Λ_k | W_k, ν_k)
'
' 参考：Bishop, "Pattern Recognition and Machine Learning", 第 10.2 节
' ============================================================================

Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

Namespace VariationalBayes

    ''' <summary>
    ''' 变分贝叶斯高斯混合模型（Variational Bayesian Gaussian Mixture Model）。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 与项目内基于 EM 的 <c>GaussianMixtureModel</c> 相比，这里对混合权重与各成分的
    ''' 均值/精度都引入了共轭先验，通过最大化变分下界（ELBO）做坐标上升，
    ''' 因此不需要模型选择就可以自动抑制多余成分（ARD 式的成分剪枝）。
    ''' </para>
    ''' <para>
    ''' 使用 <see cref="DirichletAlpha0"/> 取一个很小的值（默认 1e-3）时，
    ''' 没有被数据支撑的成分其 α_k 会停留在 α0 附近，对应的混合权重趋近于 0。
    ''' </para>
    ''' </remarks>
    Public Class VariationalGaussianMixture
        Inherits VariationalInference

#Region "超参数"

        ''' <summary>混合成分的数量上限 K</summary>
        Public Property NComponents As Integer

        ''' <summary>Dirichlet 先验的浓度参数 α0，取小值可以驱动成分剪枝</summary>
        Public Property DirichletAlpha0 As Double = 0.001R

        ''' <summary>均值先验的缩放系数 β0</summary>
        Public Property PriorBeta0 As Double = 0.001R

        ''' <summary>Wishart 先验的自由度 ν0；小于等于 0 时使用数据的维度 D</summary>
        Public Property WishartDof As Double = 0

        ''' <summary>
        ''' 判定一个成分为“有效成分”的阈值：α_k - α0 大于该值的时候视为被数据支撑。
        ''' </summary>
        Public Property PruneThreshold As Double = 1.0R

        Private _priorMean As Double() = Nothing
        Private _wishartScale As Double(,) = Nothing

        ''' <summary>
        ''' 均值先验 m0。未设置的时候使用训练数据的列均值。
        ''' </summary>
        Public Property PriorMean As Double()
            Get
                Return _priorMean
            End Get
            Set(value As Double())
                _priorMean = value
            End Set
        End Property

        ''' <summary>
        ''' Wishart 先验的尺度矩阵 W0 [D, D]。未设置的时候使用单位矩阵。
        ''' </summary>
        Public Property WishartScale As Tensor
            Get
                If _wishartScale Is Nothing Then
                    Return Nothing
                End If

                Return New Tensor(_wishartScale)
            End Get
            Set(value As Tensor)
                If value Is Nothing Then
                    _wishartScale = Nothing
                Else
                    If value.Rank <> 2 Then
                        Throw New ArgumentException("Wishart 尺度矩阵必须是二维方阵 [D, D]", NameOf(value))
                    End If

                    _wishartScale = value.To2DArrayDouble()
                End If
            End Set
        End Property

#End Region

#Region "内部状态"

        Private _X As Double(,)
        Private _N As Integer
        Private _D As Integer
        Private _K As Integer

        Private _alpha As Double()
        Private _beta As Double()
        Private _nu As Double()
        Private _m As Double(,)
        Private _W As Tensor()
        Private _logDetW As Double()

        Private _m0 As Double()
        Private _alpha0 As Double
        Private _beta0 As Double
        Private _nu0 As Double
        Private _W0inv As Double(,)
        Private _logDetW0 As Double

        Private _resp As Double(,)
        Private _logNorm As Double()
        Private _Nk As Double()
        Private _xbar As Double(,)
        Private _S As Tensor()
        Private _fitted As Boolean = False

#End Region

#Region "构造"

        ''' <summary>
        ''' 创建一个变分贝叶斯高斯混合模型。
        ''' </summary>
        ''' <param name="nComponents">混合成分的数量上限 K</param>
        ''' <param name="maxIter">最大迭代次数</param>
        ''' <param name="tol">下界收敛阈值</param>
        ''' <param name="seed">随机种子</param>
        Public Sub New(Optional nComponents As Integer = 3,
                       Optional maxIter As Integer = 200,
                       Optional tol As Double = 0.000001R,
                       Optional seed As Integer? = Nothing)
            Me.NComponents = nComponents
            Me.MaxIter = maxIter
            Me.Tolerance = tol
            Me.Seed = seed
        End Sub

#End Region

#Region "训练结果"

        ''' <summary>Dirichlet 后验参数 α_k [K]</summary>
        Public ReadOnly Property Alpha As Tensor
            Get
                If Not _fitted Then
                    Return Nothing
                End If

                Return New Tensor(DirectCast(_alpha.Clone(), Double()), _K)
            End Get
        End Property

        ''' <summary>各成分的混合权重 E[π_k] = α_k / Σα [K]</summary>
        Public ReadOnly Property Weights As Tensor
            Get
                If Not _fitted Then
                    Return Nothing
                End If

                Dim sumAlpha As Double = 0

                For k As Integer = 0 To _K - 1
                    sumAlpha += _alpha(k)
                Next

                Dim data As Double() = New Double(_K - 1) {}

                For k As Integer = 0 To _K - 1
                    data(k) = _alpha(k) / sumAlpha
                Next

                Return New Tensor(data, _K)
            End Get
        End Property

        ''' <summary>各成分的均值 m_k [K, D]</summary>
        Public ReadOnly Property Means As Tensor
            Get
                If Not _fitted Then
                    Return Nothing
                End If

                Dim data As Double() = New Double(_K * _D - 1) {}

                For k As Integer = 0 To _K - 1
                    For d As Integer = 0 To _D - 1
                        data(k * _D + d) = _m(k, d)
                    Next
                Next

                Return New Tensor(data, _K, _D)
            End Get
        End Property

        ''' <summary>
        ''' 各成分的协方差矩阵 Σ_k = (ν_k W_k)^-1，形状 [K, D, D]。
        ''' </summary>
        Public ReadOnly Property Covariances As Tensor
            Get
                If Not _fitted Then
                    Return Nothing
                End If

                Dim data As Double() = New Double(_K * _D * _D - 1) {}
                Dim offset As Integer = 0

                For k As Integer = 0 To _K - 1
                    Dim scaled As Double(,) = New Double(_D - 1, _D - 1) {}

                    For i As Integer = 0 To _D - 1
                        For j As Integer = 0 To _D - 1
                            scaled(i, j) = _nu(k) * _W(k)(i, j)
                        Next
                    Next

                    Dim cov As Double(,) = MatrixBridge.InverseOrPseudo(scaled)

                    For i As Integer = 0 To _D - 1
                        For j As Integer = 0 To _D - 1
                            data(offset) = cov(i, j)
                            offset += 1
                        Next
                    Next
                Next

                Return New Tensor(data, _K, _D, _D)
            End Get
        End Property

        ''' <summary>训练得到的责任度矩阵 r_nk，形状 [N, K]</summary>
        Public ReadOnly Property Responsibilities As Tensor
            Get
                If Not _fitted Then
                    Return Nothing
                End If

                Return New Tensor(_resp)
            End Get
        End Property

        ''' <summary>
        ''' 有效成分的数量：满足 α_k - α0 &gt; <see cref="PruneThreshold"/> 的成分个数。
        ''' </summary>
        Public ReadOnly Property EffectiveComponents As Integer
            Get
                Return ActiveComponents.Length
            End Get
        End Property

        ''' <summary>所有有效成分的下标（按 α_k 从大到小排序）</summary>
        Public ReadOnly Property ActiveComponents As Integer()
            Get
                If Not _fitted Then
                    Return New Integer() {}
                End If

                Dim indices As New List(Of Integer)

                For k As Integer = 0 To _K - 1
                    If (_alpha(k) - _alpha0) > PruneThreshold Then
                        indices.Add(k)
                    End If
                Next

                Dim result As Integer() = indices.ToArray()
                Dim weights As Double() = New Double(result.Length - 1) {}

                For i As Integer = 0 To result.Length - 1
                    weights(i) = _alpha(result(i))
                Next

                Array.Sort(weights, result)
                Array.Reverse(result)

                Return result
            End Get
        End Property

        ''' <summary>平均到每个样本上的变分下界</summary>
        Public ReadOnly Property AverageBound As Double
            Get
                If Not _fitted OrElse _N <= 0 Then
                    Return Double.NaN
                End If

                Return ELBO / _N
            End Get
        End Property

#End Region

#Region "变分推断钩子"

        ''' <summary>
        ''' 解析并校验先验参数，随后用 K-Means++ 初始化各成分的均值与硬责任度。
        ''' </summary>
        Protected Overrides Sub InitializeModel(X As Tensor)
            _K = NComponents
            _N = X.Shape(0)
            _D = X.Shape(1)

            If _K < 1 Then
                Throw New ArgumentOutOfRangeException(NameOf(NComponents), _K, "混合成分数量必须大于 0")
            End If

            If _K > _N Then
                Throw New ArgumentException($"混合成分数量 K={_K} 不能超过样本数量 N={_N}", NameOf(NComponents))
            End If

            If DirichletAlpha0 <= 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(DirichletAlpha0), DirichletAlpha0, "Dirichlet 浓度参数必须为正数")
            End If

            If PriorBeta0 <= 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(PriorBeta0), PriorBeta0, "均值先验的缩放系数必须为正数")
            End If

            _X = X.To2DArrayDouble()
            _alpha0 = DirichletAlpha0
            _beta0 = PriorBeta0
            _nu0 = If(WishartDof > 0, WishartDof, CDbl(_D))

            If _nu0 <= (_D - 1) Then
                Throw New ArgumentOutOfRangeException(NameOf(WishartDof), _nu0,
                    $"Wishart 先验的自由度必须大于 D-1 = {_D - 1}")
            End If

            Call InitializePriorMean()
            Call InitializeWishartScale()

            _alpha = New Double(_K - 1) {}
            _beta = New Double(_K - 1) {}
            _nu = New Double(_K - 1) {}
            _m = New Double(_K - 1, _D - 1) {}
            _W = New Tensor(_K - 1) {}
            _logDetW = New Double(_K - 1) {}
            _Nk = New Double(_K - 1) {}
            _xbar = New Double(_K - 1, _D - 1) {}
            _S = New Tensor(_K - 1) {}
            _resp = New Double(_N - 1, _K - 1) {}
            _logNorm = New Double(_N - 1) {}

            Call InitializeByKMeansPlusPlus()

            ' 使用硬分配做一次 M 步，得到合法的初始超参数
            Call Maximization()

            _fitted = True
        End Sub

        ''' <summary>
        ''' E 步：用当前的 q(π) 与 q(μ, Λ) 更新责任度 r_nk。
        ''' </summary>
        Protected Overrides Sub Expectation()
            _logNorm = New Double(_N - 1) {}
            _resp = BuildLogResponsibilities(_X, _logNorm)
        End Sub

        ''' <summary>
        ''' M 步：用当前的责任度更新 q(π) 与 q(μ, Λ)。
        ''' </summary>
        Protected Overrides Sub Maximization()
            Call AccumulateSufficientStatistics()

            For k As Integer = 0 To _K - 1
                Dim nk As Double = _Nk(k)

                _alpha(k) = _alpha0 + nk
                _beta(k) = _beta0 + nk
                _nu(k) = _nu0 + nk

                ' 均值：m_k = (β0 m0 + N_k x̄_k) / β_k
                For d As Integer = 0 To _D - 1
                    _m(k, d) = (_beta0 * _m0(d) + nk * _xbar(k, d)) / _beta(k)
                Next

                ' 尺度：W_k^-1 = W0^-1 + N_k S_k + (β0 N_k / β_k)(x̄_k - m0)(x̄_k - m0)'
                Dim winv As Double(,) = New Double(_D - 1, _D - 1) {}
                Dim coefficient As Double = If(nk > 0, _beta0 * nk / _beta(k), 0.0R)

                For i As Integer = 0 To _D - 1
                    For j As Integer = 0 To _D - 1
                        Dim diff As Double = _xbar(k, i) - _m0(i)
                        Dim diff2 As Double = _xbar(k, j) - _m0(j)

                        winv(i, j) = _W0inv(i, j) +
                                     nk * _S(k)(i, j) +
                                     coefficient * diff * diff2
                    Next
                Next

                Call Symmetrize(winv)

                Dim w As Double(,) = MatrixBridge.InverseOrPseudo(winv)

                Call Symmetrize(w)
                _W(k) = New Tensor(w)

                Dim logDetWinv As Double = MatrixBridge.LogDet(winv)

                If Double.IsNegativeInfinity(logDetWinv) Then
                    ' W0 与样本协方差都退化：退回到先验尺度矩阵
                    _W(k) = New Tensor(MatrixBridge.InverseOrPseudo(_W0inv))
                    _logDetW(k) = _logDetW0
                Else
                    _logDetW(k) = -logDetWinv
                End If

                ' 数值保护：ν_k 必须大于 D-1
                If _nu(k) <= (_D - 1) Then
                    _nu(k) = _D + 1.0E-3R
                End If
            Next
        End Sub

        ''' <summary>
        ''' 计算变分下界 ELBO（Bishop 10.70 – 10.77 的七项分解）。
        ''' </summary>
        Protected Overrides Function ComputeELBO() As Double
            Dim d As Integer = _D
            Dim k As Integer = _K
            Dim lnTwoPi As Double = std.Log(2.0R * std.PI)
            Dim lnTwo As Double = std.Log(2.0R)

            Dim sumAlpha As Double = 0

            For i As Integer = 0 To k - 1
                sumAlpha += _alpha(i)
            Next

            Dim psiSumAlpha As Double = VBMath.Digamma(sumAlpha)

            Dim elnPi As Double() = New Double(k - 1) {}
            Dim elnLambda As Double() = New Double(k - 1) {}
            Dim sumElnPi As Double = 0
            Dim sumElnLambda As Double = 0

            For i As Integer = 0 To k - 1
                elnPi(i) = VBMath.Digamma(_alpha(i)) - psiSumAlpha

                Dim acc As Double = d * lnTwo + _logDetW(i)

                For j As Integer = 0 To d - 1
                    acc += VBMath.Digamma(0.5R * (_nu(i) - j))
                Next

                elnLambda(i) = acc
                sumElnPi += elnPi(i)
                sumElnLambda += acc
            Next

            ' ---- 1. E[ln p(X|Z,μ,Λ)] ----
            Dim term1 As Double = 0

            For i As Integer = 0 To k - 1
                Dim nk As Double = _Nk(i)
                Dim quadSum As Double = nk * TraceProduct(_S(i), _W(i))

                Dim diff As Double() = New Double(d - 1) {}

                For j As Integer = 0 To d - 1
                    diff(j) = _xbar(i, j) - _m(i, j)
                Next

                quadSum += nk * Quadratic(diff, _W(i))

                term1 += nk * elnLambda(i) -
                         nk * d * lnTwoPi -
                         nk * d / _beta(i) -
                         _nu(i) * quadSum
            Next

            term1 *= 0.5R

            ' ---- 2. E[ln p(Z|π)] ----
            Dim term2 As Double = 0

            For i As Integer = 0 To k - 1
                term2 += _Nk(i) * elnPi(i)
            Next

            ' ---- 3. E[ln p(π)] ----
            Dim lnCAlpha0 As Double = VBMath.LogGamma(k * _alpha0) - k * VBMath.LogGamma(_alpha0)
            Dim term3 As Double = lnCAlpha0 + (_alpha0 - 1.0R) * sumElnPi

            ' ---- 4. E[ln p(μ,Λ)] ----
            Dim term4 As Double = 0

            For i As Integer = 0 To k - 1
                Dim diff As Double() = New Double(d - 1) {}

                For j As Integer = 0 To d - 1
                    diff(j) = _m(i, j) - _m0(j)
                Next

                term4 += 0.5R * (d * std.Log(_beta0 / (2.0R * std.PI)) +
                                 elnLambda(i) -
                                 d * _beta0 / _beta(i) -
                                 _beta0 * _nu(i) * Quadratic(diff, _W(i)))
            Next

            term4 += k * VBMath.LogWishartNormalizer(_logDetW0, _nu0, d)
            term4 += 0.5R * (_nu0 - d - 1.0R) * sumElnLambda
            ' E[tr(W0^-1 Λ_k)] = ν0 D
            term4 -= 0.5R * _nu0 * d * k

            ' ---- 5. -E[ln q(Z)] ----
            Dim term5 As Double = 0

            For n As Integer = 0 To _N - 1
                For i As Integer = 0 To k - 1
                    Dim r As Double = _resp(n, i)

                    If r > 1.0E-12R Then
                        term5 -= r * std.Log(r)
                    End If
                Next
            Next

            ' ---- 6. -E[ln q(π)] ----
            Dim lnCAlpha As Double = VBMath.LogGamma(sumAlpha)

            For i As Integer = 0 To k - 1
                lnCAlpha -= VBMath.LogGamma(_alpha(i))
            Next

            Dim term6 As Double = -lnCAlpha

            For i As Integer = 0 To k - 1
                term6 -= (_alpha(i) - 1.0R) * elnPi(i)
            Next

            ' ---- 7. -E[ln q(μ,Λ)] = H[q(μ|Λ)] + H[q(Λ)] ----
            Dim term7 As Double = 0

            For i As Integer = 0 To k - 1
                ' H[q(μ_k|Λ_k)] = ½(D ln2π + D - D lnβ_k - E[ln|Λ_k|])
                ' H[q(Λ_k)]    = -lnB(W_k,ν_k) - ½(ν_k-D-1)E[ln|Λ_k|] + ½ν_k D
                term7 += 0.5R * d * std.Log(2.0R * std.PI) +
                         0.5R * d -
                         0.5R * d * std.Log(_beta(i)) -
                         0.5R * elnLambda(i) -
                         VBMath.LogWishartNormalizer(_logDetW(i), _nu(i), d) -
                         0.5R * (_nu(i) - d - 1.0R) * elnLambda(i) +
                         0.5R * _nu(i) * d
            Next

            Return term1 + term2 + term3 + term4 + term5 + term6 + term7
        End Function

#End Region

#Region "预测"

        ''' <summary>
        ''' 计算新样本的成分责任度（后验概率）r_nk。
        ''' </summary>
        Public Function PredictProba(X As Tensor) As Tensor
            Call EnsureFitted()

            Dim data As Double(,) = ValidateInput(X)
            Dim logNorm As Double() = Nothing
            Dim resp As Double(,) = BuildLogResponsibilities(data, logNorm)

            Return New Tensor(resp)
        End Function

        ''' <summary>
        ''' 返回每个样本最可能的成分下标（硬聚类标签）。
        ''' </summary>
        Public Function Predict(X As Tensor) As Integer()
            Dim resp As Tensor = PredictProba(X)
            Dim n As Integer = resp.Shape(0)
            Dim k As Integer = resp.Shape(1)
            Dim labels As Integer() = New Integer(n - 1) {}

            For i As Integer = 0 To n - 1
                Dim maxValue As Double = Double.NegativeInfinity
                Dim maxIndex As Integer = 0

                For j As Integer = 0 To k - 1
                    If resp(i, j) > maxValue Then
                        maxValue = resp(i, j)
                        maxIndex = j
                    End If
                Next

                labels(i) = maxIndex
            Next

            Return labels
        End Function

        ''' <summary>
        ''' 返回样本的平均变分对数似然下界（越大越好）。
        ''' </summary>
        Public Function Score(X As Tensor) As Double
            Call EnsureFitted()

            Dim data As Double(,) = ValidateInput(X)
            Dim logNorm As Double() = Nothing

            Call BuildLogResponsibilities(data, logNorm)

            Dim acc As Double = 0

            For i As Integer = 0 To logNorm.Length - 1
                acc += logNorm(i)
            Next

            Return acc / logNorm.Length
        End Function

        ''' <summary>
        ''' 计算每个样本的变分对数似然下界 ln p(x_n) 的近似值。
        ''' </summary>
        Public Function ScoreSamples(X As Tensor) As Tensor
            Call EnsureFitted()

            Dim data As Double(,) = ValidateInput(X)
            Dim logNorm As Double() = Nothing

            Call BuildLogResponsibilities(data, logNorm)

            Return New Tensor(DirectCast(logNorm.Clone(), Double()), logNorm.Length)
        End Function

        Public Overrides Function ToString() As String
            If Not _fitted Then
                Return "VB-GMM (未训练)"
            End If

            Return $"VB-GMM: K={_K}, effective={EffectiveComponents}, N={_N}, D={_D}, " &
                   $"ELBO={ELBO:F4}, iter={NIter}, converged={Converged}"
        End Function

#End Region

#Region "内部实现"

        Private Sub EnsureFitted()
            If Not _fitted Then
                Throw New InvalidOperationException("模型尚未训练，请先调用 Fit 方法")
            End If
        End Sub

        Private Function ValidateInput(X As Tensor) As Double(,)
            If X Is Nothing Then
                Throw New ArgumentNullException(NameOf(X))
            End If

            If X.Rank <> 2 Then
                Throw New ArgumentException($"输入数据必须是二维张量 [N, D]，实际秩为 {X.Rank}", NameOf(X))
            End If

            If X.Shape(1) <> _D Then
                Throw New ArgumentException($"输入数据的维度 D={X.Shape(1)} 与模型的维度 D={_D} 不一致", NameOf(X))
            End If

            Return X.To2DArrayDouble()
        End Function

        Private Sub InitializePriorMean()
            _m0 = New Double(_D - 1) {}

            If _priorMean Is Nothing Then
                For d As Integer = 0 To _D - 1
                    Dim acc As Double = 0

                    For n As Integer = 0 To _N - 1
                        acc += _X(n, d)
                    Next

                    _m0(d) = acc / _N
                Next
            Else
                If _priorMean.Length <> _D Then
                    Throw New ArgumentException(
                        $"均值先验的长度 {_priorMean.Length} 与数据维度 D={_D} 不一致", NameOf(PriorMean))
                End If

                Array.Copy(_priorMean, _m0, _D)
            End If
        End Sub

        Private Sub InitializeWishartScale()
            Dim w0 As Double(,) = New Double(_D - 1, _D - 1) {}

            If _wishartScale Is Nothing Then
                For i As Integer = 0 To _D - 1
                    w0(i, i) = 1.0R
                Next
            Else
                If _wishartScale.GetLength(0) <> _D OrElse _wishartScale.GetLength(1) <> _D Then
                    Throw New ArgumentException(
                        $"Wishart 尺度矩阵的形状 [{_wishartScale.GetLength(0)}, {_wishartScale.GetLength(1)}] " &
                        $"与数据维度 D={_D} 不一致", NameOf(WishartScale))
                End If

                Array.Copy(_wishartScale, w0, _wishartScale.Length)
            End If

            Call Symmetrize(w0)

            _logDetW0 = MatrixBridge.LogDet(w0)

            If Double.IsNegativeInfinity(_logDetW0) Then
                ' 尺度矩阵奇异：加入一个很小的对角线抖动
                For i As Integer = 0 To _D - 1
                    w0(i, i) += VBMath.DefaultJitter
                Next

                _logDetW0 = MatrixBridge.LogDet(w0)
            End If

            _W0inv = MatrixBridge.InverseOrPseudo(w0)
        End Sub

        ''' <summary>
        ''' K-Means++ 风格的初始化：按距离平方加权选择 K 个初始中心，
        ''' 然后按最近中心做硬分配，得到初始责任度。
        ''' </summary>
        Private Sub InitializeByKMeansPlusPlus()
            Dim chosen As New List(Of Integer)
            Dim first As Integer = RNG.Next(_N)

            chosen.Add(first)

            For d As Integer = 0 To _D - 1
                _m(0, d) = _X(first, d)
            Next

            Dim distances As Double() = New Double(_N - 1) {}

            For k As Integer = 1 To _K - 1
                Dim total As Double = 0

                For n As Integer = 0 To _N - 1
                    Dim minDistance As Double = Double.MaxValue

                    For Each center As Integer In chosen
                        Dim distance As Double = 0

                        For d As Integer = 0 To _D - 1
                            Dim diff As Double = _X(n, d) - _X(center, d)
                            distance += diff * diff
                        Next

                        If distance < minDistance Then
                            minDistance = distance
                        End If
                    Next

                    distances(n) = minDistance
                    total += minDistance
                Next

                Dim pick As Integer

                If total <= 0 Then
                    ' 所有点都重合：随机挑选一个
                    pick = RNG.Next(_N)
                Else
                    Dim target As Double = RNG.NextDouble() * total
                    Dim cumulative As Double = 0
                    pick = _N - 1

                    For n As Integer = 0 To _N - 1
                        cumulative += distances(n)

                        If cumulative >= target Then
                            pick = n
                            Exit For
                        End If
                    Next
                End If

                chosen.Add(pick)

                For d As Integer = 0 To _D - 1
                    _m(k, d) = _X(pick, d)
                Next
            Next

            ' 硬分配
            For n As Integer = 0 To _N - 1
                Dim best As Integer = 0
                Dim bestDistance As Double = Double.MaxValue

                For k As Integer = 0 To _K - 1
                    Dim distance As Double = 0

                    For d As Integer = 0 To _D - 1
                        Dim diff As Double = _X(n, d) - _m(k, d)
                        distance += diff * diff
                    Next

                    If distance < bestDistance Then
                        bestDistance = distance
                        best = k
                    End If
                Next

                For k As Integer = 0 To _K - 1
                    _resp(n, k) = If(k = best, 1.0R, 0.0R)
                Next
            Next
        End Sub

        ''' <summary>
        ''' 根据当前超参数计算每个样本的对数责任度 ln ρ_nk。
        ''' </summary>
        Private Function BuildLogResponsibilities(x As Double(,),
                                                 ByRef logNorm As Double()) As Double(,)
            Dim n As Integer = x.GetLength(0)
            Dim d As Integer = _D
            Dim kk As Integer = _K

            Dim logRho As Double(,) = New Double(n - 1, kk - 1) {}
            Dim constant As Double() = New Double(kk - 1) {}

            Dim sumAlpha As Double = 0

            For k As Integer = 0 To kk - 1
                sumAlpha += _alpha(k)
            Next

            Dim psiSumAlpha As Double = VBMath.Digamma(sumAlpha)

            For k As Integer = 0 To kk - 1
                Dim elnPi As Double = VBMath.Digamma(_alpha(k)) - psiSumAlpha
                Dim elnLambda As Double = d * std.Log(2.0R) + _logDetW(k)

                For j As Integer = 0 To d - 1
                    elnLambda += VBMath.Digamma(0.5R * (_nu(k) - j))
                Next

                constant(k) = elnPi + 0.5R * elnLambda - 0.5R * d * std.Log(2.0R * std.PI)
            Next

            Dim diff As Double() = New Double(d - 1) {}

            For i As Integer = 0 To n - 1
                For k As Integer = 0 To kk - 1
                    For j As Integer = 0 To d - 1
                        diff(j) = x(i, j) - _m(k, j)
                    Next

                    Dim quad As Double = Quadratic(diff, _W(k))

                    logRho(i, k) = constant(k) -
                                   0.5R * (d / _beta(k) + _nu(k) * quad)
                Next
            Next

            Return VBMath.SoftmaxRows(logRho, logNorm)
        End Function

        ''' <summary>
        ''' 一次扫描聚集所有成分的充分统计量 N_k、x̄_k 与 S_k。
        ''' </summary>
        Private Sub AccumulateSufficientStatistics()
            Array.Clear(_Nk, 0, _Nk.Length)
            Array.Clear(_xbar, 0, _xbar.Length)

            Dim sumX As Double() = New Double(_K * _D - 1) {}
            Dim sumXX As Double() = New Double(_K * _D * _D - 1) {}

            For n As Integer = 0 To _N - 1
                For k As Integer = 0 To _K - 1
                    Dim r As Double = _resp(n, k)

                    If r <= 1.0E-12R Then
                        Continue For
                    End If

                    _Nk(k) += r

                    For i As Integer = 0 To _D - 1
                        Dim xi As Double = _X(n, i)
                        sumX(k * _D + i) += r * xi

                        For j As Integer = 0 To _D - 1
                            sumXX(k * _D * _D + i * _D + j) += r * xi * _X(n, j)
                        Next
                    Next
                Next
            Next

            For k As Integer = 0 To _K - 1
                Dim nk As Double = _Nk(k)
                Dim s As Double(,) = New Double(_D - 1, _D - 1) {}

                If nk > 1.0E-10R Then
                    For i As Integer = 0 To _D - 1
                        _xbar(k, i) = sumX(k * _D + i) / nk
                    Next

                    For i As Integer = 0 To _D - 1
                        For j As Integer = 0 To _D - 1
                            Dim secondMoment As Double = sumXX(k * _D * _D + i * _D + j) / nk

                            s(i, j) = secondMoment - _xbar(k, i) * _xbar(k, j)
                        Next
                    Next

                    Call Symmetrize(s)
                Else
                    ' 空成分：把充分统计量退化到先验均值处，S_k = 0
                    For i As Integer = 0 To _D - 1
                        _xbar(k, i) = _m0(i)
                    Next
                End If

                _S(k) = New Tensor(s)
            Next
        End Sub

        Private Shared Sub Symmetrize(a As Double(,))
            Dim n As Integer = a.GetLength(0)

            For i As Integer = 0 To n - 1
                For j As Integer = i + 1 To n - 1
                    Dim average As Double = 0.5R * (a(i, j) + a(j, i))

                    a(i, j) = average
                    a(j, i) = average
                Next
            Next
        End Sub

        Private Shared Function Quadratic(diff As Double(), w As Tensor) As Double
            Dim n As Integer = diff.Length
            Dim acc As Double = 0

            For i As Integer = 0 To n - 1
                Dim rowSum As Double = 0

                For j As Integer = 0 To n - 1
                    rowSum += w(i, j) * diff(j)
                Next

                acc += diff(i) * rowSum
            Next

            Return acc
        End Function

        Private Shared Function TraceProduct(a As Tensor, b As Tensor) As Double
            Dim n As Integer = a.Shape(0)
            Dim acc As Double = 0

            For i As Integer = 0 To n - 1
                For j As Integer = 0 To n - 1
                    acc += a(i, j) * b(j, i)
                Next
            Next

            Return acc
        End Function

#End Region

    End Class

End Namespace
