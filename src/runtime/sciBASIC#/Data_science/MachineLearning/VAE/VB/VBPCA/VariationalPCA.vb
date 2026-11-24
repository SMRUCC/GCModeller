' ============================================================================
' VariationalPCA.vb - 变分贝叶斯主成分分析 (VB-PCA)
'
' 模型：
'   x_n = W z_n + μ + ε,  ε ~ N(0, β^-1 I_D)
'   z_n ~ N(0, I_L)
'   W   的行 w_betaRate ~ N(0, diag(α)^-1)     （ARD 先验，实现自动确定有效维度）
'   α_i ~ Gamma(a0, b0),  β ~ Gamma(c0, d0)
'
' 均值场近似：
'   q(Z) = Π_n N(z_n | m_n, Σ_z,n)
'   q(W) = Π_betaRate N(w_betaRate | m_wd, Σ_w)       （Σ_w 与 d 无关，只需要求一次逆）
'   q(α) = Π_i Gamma(a_i, b_i),  q(β) = Gamma(c, d)
'
' 均值 μ 使用观测数据的列均值（经典的 PCA 中心化处理），不作为随机变量。
' 缺失值（NaN）会被自动掩码，只使用观测到的维度参与更新与下界计算。
'
' 参考：Bishop, "Pattern Recognition and Machine Learning", 第 12.2 节
' ============================================================================

Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

Namespace VariationalBayes

    ''' <summary>
    ''' 带自动相关性判定（ARD）的变分贝叶斯主成分分析。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 载荷矩阵 W 的每一列带有一个独立的 Gamma 精度先验，多余潜在维度的精度会
    ''' 在迭代中不断增大（对应载荷被压向 0），因此可以自动确定有效维度。
    ''' </para>
    ''' <para>
    ''' 输入数据中的 <see cref="Double.NaN"/> 被视作缺失值，只用观测到的维度参与
    ''' 后验更新与下界计算。
    ''' </para>
    ''' </remarks>
    Public Class VariationalPCA
        Inherits VariationalInference

#Region "配置"

        ''' <summary>潜在维度上限 L</summary>
        Public Property LatentDim As Integer

        ''' <summary>载荷精度先验 Gamma(a0, b0) 的形状参数</summary>
        Public Property PriorAlphaA0 As Double = 0.01R

        ''' <summary>载荷精度先验 Gamma(a0, b0) 的率参数</summary>
        Public Property PriorAlphaB0 As Double = 0.01R

        ''' <summary>噪声精度先验 Gamma(c0, d0) 的形状参数</summary>
        Public Property PriorBetaC0 As Double = 0.01R

        ''' <summary>噪声精度先验 Gamma(c0, d0) 的率参数</summary>
        Public Property PriorBetaD0 As Double = 0.01R

        ''' <summary>
        ''' 判定有效潜在维度的相对阈值：载荷能量 E[||w_i||²] 小于
        ''' <c>PruneRatio × 最大载荷能量</c> 的维度视为已经被 ARD 剪枝。
        ''' </summary>
        Public Property PruneRatio As Double = 0.01R

        ''' <summary>
        ''' 是否使用 PCA（幂迭代）初始化载荷矩阵。
        ''' </summary>
        ''' <remarks>
        ''' 变分贝叶斯 PCA 对初值比较敏感：用中心化协方差矩阵的前 L 个主方向初始化
        ''' 可以显著提高收敛到全局最优（正确秩）的概率。默认开启。
        ''' </remarks>
        Public Property PcaInitialization As Boolean = True

#End Region

#Region "内部状态"

        Private _X As Double(,)
        Private _mask As Boolean(,)
        Private _mean As Double()
        Private _N As Integer
        Private _D As Integer
        Private _L As Integer
        Private _hasMissing As Boolean
        Private _observedCount As Integer

        Private _mw As Double(,)
        Private _Sw As Double(,)
        Private _mz As Double(,)
        Private _Sz As Double()
        Private _SzLogDet As Double()

        Private _a As Double()
        Private _b As Double()
        Private _c As Double
        Private _betaRate As Double

        Private _residualExpected As Double
        Private _fitted As Boolean = False

#End Region

#Region "构造"

        ''' <summary>
        ''' 创建变分贝叶斯主成分分析模型。
        ''' </summary>
        ''' <param name="latentDim">潜在维度上限 L</param>
        Public Sub New(Optional latentDim As Integer = 2,
                       Optional maxIter As Integer = 200,
                       Optional tol As Double = 0.000001R,
                       Optional seed As Integer? = Nothing)
            Me.LatentDim = latentDim
            Me.MaxIter = maxIter
            Me.Tolerance = tol
            Me.Seed = seed
        End Sub

#End Region

#Region "训练结果"

        ''' <summary>载荷矩阵的后验均值 E[W]，形状 [D, L]</summary>
        Public ReadOnly Property Components As Tensor
            Get
                If Not _fitted Then
                    Return Nothing
                End If

                Dim data As Double() = New Double(_D * _L - 1) {}

                For j As Integer = 0 To _D - 1
                    For i As Integer = 0 To _L - 1
                        data(j * _L + i) = _mw(j, i)
                    Next
                Next

                Return New Tensor(data, _D, _L)
            End Get
        End Property

        ''' <summary>隐变量的后验均值 E[Z]，形状 [N, L]</summary>
        Public ReadOnly Property Scores As Tensor
            Get
                If Not _fitted Then
                    Return Nothing
                End If

                Dim data As Double() = New Double(_N * _L - 1) {}

                For n As Integer = 0 To _N - 1
                    For i As Integer = 0 To _L - 1
                        data(n * _L + i) = _mz(n, i)
                    Next
                Next

                Return New Tensor(data, _N, _L)
            End Get
        End Property

        ''' <summary>每一列载荷的先验精度后验期望 E[α_i]，形状 [L]</summary>
        Public ReadOnly Property LatentPrecision As Tensor
            Get
                If Not _fitted Then
                    Return Nothing
                End If

                Dim data As Double() = New Double(_L - 1) {}

                For i As Integer = 0 To _L - 1
                    data(i) = _a(i) / _b(i)
                Next

                Return New Tensor(data, _L)
            End Get
        End Property

        ''' <summary>噪声精度的后验期望 E[β]</summary>
        Public ReadOnly Property NoisePrecision As Double
            Get
                Return _c / _betaRate
            End Get
        End Property

        ''' <summary>噪声方差的后验估计 1 / E[β]</summary>
        Public ReadOnly Property ResidualVariance As Double
            Get
                Return 1.0R / NoisePrecision
            End Get
        End Property

        ''' <summary>数据的列均值（PCA 的中心化位置）</summary>
        Public ReadOnly Property Center As Tensor
            Get
                If Not _fitted Then
                    Return Nothing
                End If

                Return New Tensor(DirectCast(_mean.Clone(), Double()), _D)
            End Get
        End Property

        ''' <summary>判定为有效的潜在维度下标（按载荷能量从大到小排序）</summary>
        Public ReadOnly Property EffectiveLatentDimensions As Integer()
            Get
                If Not _fitted Then
                    Return New Integer() {}
                End If

                Dim energy As Double() = New Double(_L - 1) {}
                Dim maximum As Double = 0

                For i As Integer = 0 To _L - 1
                    Dim acc As Double = 0

                    For j As Integer = 0 To _D - 1
                        acc += _mw(j, i) * _mw(j, i)
                    Next

                    acc += _D * _Sw(i, i)
                    energy(i) = acc

                    If acc > maximum Then
                        maximum = acc
                    End If
                Next

                If maximum <= VBMath.TinyValue Then
                    Return New Integer() {}
                End If

                Dim threshold As Double = PruneRatio * maximum
                Dim indices As New List(Of Integer)

                For i As Integer = 0 To _L - 1
                    If energy(i) >= threshold Then
                        indices.Add(i)
                    End If
                Next

                Dim result As Integer() = indices.ToArray()

                If result.Length > 1 Then
                    Dim selected As Double() = New Double(result.Length - 1) {}

                    For i As Integer = 0 To result.Length - 1
                        selected(i) = energy(result(i))
                    Next

                    Array.Sort(selected, result)
                    Array.Reverse(result)
                End If

                Return result
            End Get
        End Property

        ''' <summary>有效潜在维度的数量（ARD 自动确定的结果）</summary>
        Public ReadOnly Property EffectiveDimension As Integer
            Get
                Return EffectiveLatentDimensions.Length
            End Get
        End Property

#End Region

#Region "变分推断钩子"

        Protected Overrides Sub InitializeModel(X As Tensor)
            _N = X.Shape(0)
            _D = X.Shape(1)
            _L = LatentDim

            If _L < 1 Then
                Throw New ArgumentOutOfRangeException(NameOf(LatentDim), _L, "潜在维度必须大于 0")
            End If

            If _L > std.Min(_N, _D) Then
                Throw New ArgumentException(
                    $"潜在维度 L={_L} 不能超过 min(N, D) = {std.Min(_N, _D)}", NameOf(LatentDim))
            End If

            If PriorAlphaA0 <= 0 OrElse PriorAlphaB0 <= 0 OrElse
               PriorBetaC0 <= 0 OrElse PriorBetaD0 <= 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(PriorAlphaA0), "Gamma 先验参数必须为正数")
            End If

            _X = X.To2DArrayDouble()
            _mask = New Boolean(_N - 1, _D - 1) {}
            _mean = New Double(_D - 1) {}
            _hasMissing = False
            _observedCount = 0

            For n As Integer = 0 To _N - 1
                For j As Integer = 0 To _D - 1
                    Dim observed As Boolean = Not Double.IsNaN(_X(n, j))

                    _mask(n, j) = observed

                    If observed Then
                        _observedCount += 1
                    Else
                        _hasMissing = True
                    End If
                Next
            Next

            If _observedCount < 1 Then
                Throw New ArgumentException("输入数据中没有任何可用的观测值", NameOf(X))
            End If

            Call ComputeColumnMean()

            _mw = New Double(_D - 1, _L - 1) {}
            _Sw = New Double(_L - 1, _L - 1) {}
            _mz = New Double(_N - 1, _L - 1) {}
            _Sz = New Double(_N * _L * _L - 1) {}
            _SzLogDet = New Double(_N - 1) {}

            ' 初始载荷：默认使用中心化协方差矩阵的前 L 个主方向（幂迭代）
            If PcaInitialization Then
                Call InitializeLoadingsByPca()
            Else
                For j As Integer = 0 To _D - 1
                    For i As Integer = 0 To _L - 1
                        _mw(j, i) = MathUtils.Randn(RNG, 0, 0.1R)
                    Next
                Next
            End If

            For i As Integer = 0 To _L - 1
                _Sw(i, i) = 1.0R
            Next

            For n As Integer = 0 To _N - 1
                For i As Integer = 0 To _L - 1
                    Call SetSz(n, i, i, 1.0R)
                Next
            Next

            ' 初始 Gamma 后验
            _a = New Double(_L - 1) {}
            _b = New Double(_L - 1) {}

            For i As Integer = 0 To _L - 1
                _a(i) = PriorAlphaA0 + 0.5R * _D
                _b(i) = PriorAlphaB0 + 0.5R
            Next

            _c = PriorBetaC0 + 0.5R * _observedCount

            Dim totalVariance As Double = 0

            For n As Integer = 0 To _N - 1
                For j As Integer = 0 To _D - 1
                    If _mask(n, j) Then
                        Dim diff As Double = _X(n, j) - _mean(j)
                        totalVariance += diff * diff
                    End If
                Next
            Next

            If totalVariance <= VBMath.TinyValue Then
                totalVariance = 1.0R
            End If

            _betaRate = PriorBetaD0 + 0.5R * totalVariance
            _residualExpected = totalVariance

            _fitted = True
        End Sub

        ''' <summary>
        ''' E 步：固定 q(W) 与 q(β)，更新隐变量的后验 q(Z)。
        ''' </summary>
        Protected Overrides Sub Expectation()
            Dim eBeta As Double = _c / _betaRate
            Dim eww As Double(,) = ComputeExpectedWTimesW()

            Dim sharedPrecision As Double(,) = New Double(_L - 1, _L - 1) {}

            For i As Integer = 0 To _L - 1
                For j As Integer = 0 To _L - 1
                    sharedPrecision(i, j) = eBeta * eww(i, j)
                Next

                sharedPrecision(i, i) += 1.0R
            Next

            Dim sharedCovariance As Double(,) = Nothing
            Dim sharedLogDet As Double = 0

            If Not _hasMissing Then
                sharedCovariance = MatrixBridge.InverseOrPseudo(sharedPrecision)
                Call Symmetrize(sharedCovariance)
                sharedLogDet = -MatrixBridge.LogDet(sharedPrecision)
            End If

            Dim rhs As Double() = New Double(_L - 1) {}

            For n As Integer = 0 To _N - 1
                Dim covariance As Double(,) = Nothing
                Dim logDet As Double = 0

                If _hasMissing Then
                    Dim precision As Double(,) = New Double(_L - 1, _L - 1) {}

                    For i As Integer = 0 To _L - 1
                        precision(i, i) = 1.0R
                    Next

                    Array.Clear(rhs, 0, _L)

                    For j As Integer = 0 To _D - 1
                        If Not _mask(n, j) Then
                            Continue For
                        End If

                        Dim centered As Double = _X(n, j) - _mean(j)

                        For i As Integer = 0 To _L - 1
                            rhs(i) += eBeta * centered * _mw(j, i)

                            For k As Integer = 0 To _L - 1
                                precision(i, k) += eBeta * (_mw(j, i) * _mw(j, k) + _Sw(i, k))
                            Next
                        Next
                    Next

                    covariance = MatrixBridge.InverseOrPseudo(precision)
                    Call Symmetrize(covariance)
                    logDet = -MatrixBridge.LogDet(precision)
                Else
                    covariance = sharedCovariance
                    logDet = sharedLogDet

                    Array.Clear(rhs, 0, _L)

                    For j As Integer = 0 To _D - 1
                        Dim centered As Double = _X(n, j) - _mean(j)

                        For i As Integer = 0 To _L - 1
                            rhs(i) += eBeta * centered * _mw(j, i)
                        Next
                    Next
                End If

                For i As Integer = 0 To _L - 1
                    Dim acc As Double = 0

                    For k As Integer = 0 To _L - 1
                        acc += covariance(i, k) * rhs(k)
                    Next

                    _mz(n, i) = acc
                Next

                For i As Integer = 0 To _L - 1
                    For k As Integer = 0 To _L - 1
                        Call SetSz(n, i, k, covariance(i, k))
                    Next
                Next

                _SzLogDet(n) = logDet
            Next
        End Sub

        ''' <summary>
        ''' M 步：固定 q(Z)，更新载荷 q(W)、q(α) 与噪声 q(β)。
        ''' </summary>
        Protected Overrides Sub Maximization()
            Dim eBeta As Double = _c / _betaRate

            ' E[Z'Z] = Σ_n (m_n m_n' + Σ_z,n)
            Dim ezz As Double(,) = New Double(_L - 1, _L - 1) {}

            For n As Integer = 0 To _N - 1
                For i As Integer = 0 To _L - 1
                    For k As Integer = 0 To _L - 1
                        ezz(i, k) += _mz(n, i) * _mz(n, k) + SzAt(n, i, k)
                    Next
                Next
            Next

            ' Σ_w = (diag(E[α]) + E[β] E[Z'Z])^-1（与数据维度 j 无关，只求一次逆）
            Dim precision As Double(,) = New Double(_L - 1, _L - 1) {}

            For i As Integer = 0 To _L - 1
                For k As Integer = 0 To _L - 1
                    precision(i, k) = eBeta * ezz(i, k)
                Next

                precision(i, i) += _a(i) / _b(i)
            Next

            _Sw = MatrixBridge.InverseOrPseudo(precision)
            Call Symmetrize(_Sw)

            ' m_wd = E[β] Σ_w E[Z]'(x_betaRate - μ_betaRate)
            Dim rhs As Double() = New Double(_L - 1) {}

            For j As Integer = 0 To _D - 1
                Array.Clear(rhs, 0, _L)

                For n As Integer = 0 To _N - 1
                    If Not _mask(n, j) Then
                        Continue For
                    End If

                    Dim centered As Double = _X(n, j) - _mean(j)

                    For i As Integer = 0 To _L - 1
                        rhs(i) += centered * _mz(n, i)
                    Next
                Next

                For i As Integer = 0 To _L - 1
                    Dim acc As Double = 0

                    For k As Integer = 0 To _L - 1
                        acc += _Sw(i, k) * rhs(k)
                    Next

                    _mw(j, i) = eBeta * acc
                Next
            Next

            ' q(α)
            For i As Integer = 0 To _L - 1
                Dim acc As Double = 0

                For j As Integer = 0 To _D - 1
                    acc += _mw(j, i) * _mw(j, i)
                Next

                acc += _D * _Sw(i, i)

                _a(i) = PriorAlphaA0 + 0.5R * _D
                _b(i) = PriorAlphaB0 + 0.5R * acc

                If _b(i) <= VBMath.TinyValue Then
                    _b(i) = VBMath.TinyValue
                End If
            Next

            ' q(β)：残差的期望
            Dim residual As Double = 0

            For n As Integer = 0 To _N - 1
                For j As Integer = 0 To _D - 1
                    If Not _mask(n, j) Then
                        Continue For
                    End If

                    residual += ExpectedSquaredResidual(n, j)
                Next
            Next

            If residual <= VBMath.TinyValue Then
                residual = VBMath.TinyValue
            End If

            _residualExpected = residual
            _c = PriorBetaC0 + 0.5R * _observedCount
            _betaRate = PriorBetaD0 + 0.5R * residual
        End Sub

        ''' <summary>
        ''' 计算变分下界 ELBO。
        ''' </summary>
        Protected Overrides Function ComputeELBO() As Double
            Dim latent As Integer = _L
            Dim dims As Integer = _D
            Dim lnTwoPi As Double = std.Log(2.0R * std.PI)
            Dim eBeta As Double = _c / _betaRate
            Dim psiBeta As Double = VBMath.Digamma(_c)
            Dim lnBeta As Double = psiBeta - std.Log(_betaRate)

            Dim sumLnAlpha As Double = 0
            Dim term3W As Double = 0

            For i As Integer = 0 To latent - 1
                Dim eAlpha As Double = _a(i) / _b(i)

                sumLnAlpha += VBMath.Digamma(_a(i)) - std.Log(_b(i))

                Dim acc As Double = 0

                For j As Integer = 0 To dims - 1
                    acc += _mw(j, i) * _mw(j, i)
                Next

                acc += dims * _Sw(i, i)
                term3W += eAlpha * acc
            Next

            ' 1. E[ln p(X|W,Z,β)]
            Dim term1 As Double = 0.5R * _observedCount * (lnBeta - lnTwoPi) -
                                  0.5R * eBeta * _residualExpected

            ' 2. E[ln p(Z)]
            Dim term2 As Double = 0

            For n As Integer = 0 To _N - 1
                Dim acc As Double = 0

                For i As Integer = 0 To latent - 1
                    acc += _mz(n, i) * _mz(n, i) + SzAt(n, i, i)
                Next

                term2 += -0.5R * latent * lnTwoPi - 0.5R * acc
            Next

            ' 3. E[ln p(W|α)]
            Dim term3 As Double = 0.5R * dims * sumLnAlpha -
                                  0.5R * dims * latent * lnTwoPi -
                                  0.5R * term3W

            ' 4. E[ln p(α)]
            Dim term4 As Double = 0

            For i As Integer = 0 To latent - 1
                term4 += PriorAlphaA0 * std.Log(PriorAlphaB0) -
                         VBMath.LogGamma(PriorAlphaA0) +
                         (PriorAlphaA0 - 1.0R) * (VBMath.Digamma(_a(i)) - std.Log(_b(i))) -
                         PriorAlphaB0 * (_a(i) / _b(i))
            Next

            ' 5. E[ln p(β)]
            Dim term5 As Double = PriorBetaC0 * std.Log(PriorBetaD0) -
                                  VBMath.LogGamma(PriorBetaC0) +
                                  (PriorBetaC0 - 1.0R) * lnBeta -
                                  PriorBetaD0 * eBeta

            ' 6. -E[ln q(Z)]
            Dim term6 As Double = 0

            For n As Integer = 0 To _N - 1
                term6 += 0.5R * latent * (1.0R + lnTwoPi) + 0.5R * _SzLogDet(n)
            Next

            ' 7. -E[ln q(W)] = D · H[q(w_betaRate)]
            Dim term7 As Double = dims * (0.5R * latent * (1.0R + lnTwoPi) +
                                          0.5R * MatrixBridge.LogDet(_Sw))

            ' 8. -E[ln q(α)]
            Dim term8 As Double = 0

            For i As Integer = 0 To latent - 1
                Dim psi As Double = VBMath.Digamma(_a(i))

                term8 += VBMath.LogGamma(_a(i)) - std.Log(_b(i)) -
                         (_a(i) - 1.0R) * psi + _a(i)
            Next

            ' 9. -E[ln q(β)]
            Dim term9 As Double = VBMath.LogGamma(_c) - std.Log(_betaRate) -
                                  (_c - 1.0R) * psiBeta + _c

            Return term1 + term2 + term3 + term4 + term5 + term6 + term7 + term8 + term9
        End Function

#End Region

#Region "变换与重构"

        ''' <summary>
        ''' 计算新样本的隐变量后验均值 E[z]。
        ''' </summary>
        Public Function Transform(X As Tensor) As Tensor
            Call EnsureFitted()

            Dim data As Double(,) = ValidateInput(X)
            Dim n As Integer = data.GetLength(0)
            Dim result As Double() = New Double(n * _L - 1) {}
            Dim eBeta As Double = NoisePrecision

            Dim eww As Double(,) = ComputeExpectedWTimesW()
            Dim sharedPrecision As Double(,) = New Double(_L - 1, _L - 1) {}

            For i As Integer = 0 To _L - 1
                For k As Integer = 0 To _L - 1
                    sharedPrecision(i, k) = eBeta * eww(i, k)
                Next

                sharedPrecision(i, i) += 1.0R
            Next

            Dim sharedCovariance As Double(,) = MatrixBridge.InverseOrPseudo(sharedPrecision)

            For row As Integer = 0 To n - 1
                Dim rowHasMissing As Boolean = False

                For j As Integer = 0 To _D - 1
                    If Double.IsNaN(data(row, j)) Then
                        rowHasMissing = True
                        Exit For
                    End If
                Next

                Dim covariance As Double(,) = sharedCovariance

                If rowHasMissing Then
                    Dim precision As Double(,) = New Double(_L - 1, _L - 1) {}

                    For i As Integer = 0 To _L - 1
                        precision(i, i) = 1.0R
                    Next

                    For j As Integer = 0 To _D - 1
                        Dim value As Double = data(row, j)

                        If Double.IsNaN(value) Then
                            Continue For
                        End If

                        Dim centered As Double = value - _mean(j)

                        For i As Integer = 0 To _L - 1
                            For k As Integer = 0 To _L - 1
                                precision(i, k) += eBeta * (_mw(j, i) * _mw(j, k) + _Sw(i, k))
                            Next
                        Next
                    Next

                    covariance = MatrixBridge.InverseOrPseudo(precision)
                End If

                Dim rhs As Double() = New Double(_L - 1) {}

                For j As Integer = 0 To _D - 1
                    Dim value As Double = data(row, j)

                    If Double.IsNaN(value) Then
                        Continue For
                    End If

                    Dim centered As Double = value - _mean(j)

                    For i As Integer = 0 To _L - 1
                        rhs(i) += eBeta * centered * _mw(j, i)
                    Next
                Next

                For i As Integer = 0 To _L - 1
                    Dim acc As Double = 0

                    For k As Integer = 0 To _L - 1
                        acc += covariance(i, k) * rhs(k)
                    Next

                    result(row * _L + i) = acc
                Next
            Next

            Return New Tensor(result, n, _L)
        End Function

        ''' <summary>
        ''' 使用隐变量的后验均值重构输入数据。
        ''' </summary>
        Public Function Reconstruct(X As Tensor) As Tensor
            Dim scores As Tensor = Transform(X)
            Dim n As Integer = scores.Shape(0)
            Dim data As Double() = New Double(n * _D - 1) {}

            For row As Integer = 0 To n - 1
                For j As Integer = 0 To _D - 1
                    Dim acc As Double = _mean(j)

                    For i As Integer = 0 To _L - 1
                        acc += _mw(j, i) * scores(row, i)
                    Next

                    data(row * _D + j) = acc
                Next
            Next

            Return New Tensor(data, n, _D)
        End Function

        ''' <summary>
        ''' 在观测到的元素上计算重构的均方根误差。
        ''' </summary>
        Public Function ReconstructionError(X As Tensor) As Double
            Dim reconstruction As Tensor = Reconstruct(X)
            Dim n As Integer = X.Shape(0)
            Dim acc As Double = 0
            Dim count As Integer = 0

            For row As Integer = 0 To n - 1
                For j As Integer = 0 To _D - 1
                    Dim value As Double = X(row, j)

                    If Double.IsNaN(value) Then
                        Continue For
                    End If

                    Dim diff As Double = value - reconstruction(row, j)
                    acc += diff * diff
                    count += 1
                Next
            Next

            If count = 0 Then
                Return Double.NaN
            End If

            Return std.Sqrt(acc / count)
        End Function

        Public Overrides Function ToString() As String
            If Not _fitted Then
                Return "VB-PCA (未训练)"
            End If

            Return $"VB-PCA: N={_N}, D={_D}, L={_L}, effective={EffectiveDimension}, " &
                   $"E[β]={NoisePrecision:F4}, ELBO={ELBO:F4}, iter={NIter}, converged={Converged}"
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

        Private Sub ComputeColumnMean()
            For j As Integer = 0 To _D - 1
                Dim acc As Double = 0
                Dim count As Integer = 0

                For n As Integer = 0 To _N - 1
                    If _mask(n, j) Then
                        acc += _X(n, j)
                        count += 1
                    End If
                Next

                _mean(j) = If(count > 0, acc / count, 0.0R)
            Next
        End Sub

        ''' <summary>
        ''' E[W'W]，其中不确定性项为 Σ_betaRate Σ_w。
        ''' </summary>
        Private Function ComputeExpectedWTimesW() As Double(,)
            Dim result As Double(,) = New Double(_L - 1, _L - 1) {}

            For i As Integer = 0 To _L - 1
                For k As Integer = 0 To _L - 1
                    Dim acc As Double = 0

                    For j As Integer = 0 To _D - 1
                        acc += _mw(j, i) * _mw(j, k)
                    Next

                    result(i, k) = acc + _D * _Sw(i, k)
                Next
            Next

            Return result
        End Function

        ''' <summary>
        ''' 单个观测元素的期望平方残差
        ''' E[(x_nd - μ_betaRate - w_betaRate'z_n)²]。
        ''' </summary>
        Private Function ExpectedSquaredResidual(n As Integer, j As Integer) As Double
            Dim predicted As Double = 0

            For i As Integer = 0 To _L - 1
                predicted += _mw(j, i) * _mz(n, i)
            Next

            Dim diff As Double = _X(n, j) - _mean(j) - predicted
            Dim acc As Double = diff * diff

            ' w_betaRate' Σ_z,n w_betaRate
            For i As Integer = 0 To _L - 1
                Dim rowAcc As Double = 0

                For k As Integer = 0 To _L - 1
                    rowAcc += SzAt(n, i, k) * _mw(j, k)
                Next

                acc += _mw(j, i) * rowAcc
            Next

            ' m_n' Σ_w m_n
            For i As Integer = 0 To _L - 1
                Dim rowAcc As Double = 0

                For k As Integer = 0 To _L - 1
                    rowAcc += _Sw(i, k) * _mz(n, k)
                Next

                acc += _mz(n, i) * rowAcc
            Next

            ' tr(Σ_z,n Σ_w)
            For i As Integer = 0 To _L - 1
                For k As Integer = 0 To _L - 1
                    acc += SzAt(n, i, k) * _Sw(k, i)
                Next
            Next

            Return acc
        End Function

        ''' <summary>
        ''' 使用幂迭代（带收缩 deflation）计算协方差矩阵的前 L 个主方向，
        ''' 并用主方向的尺度作为载荷矩阵的初始值。
        ''' </summary>
        Private Sub InitializeLoadingsByPca()
            Dim covariance As Double(,) = ComputeCovariance()
            Dim deflated As Double(,) = DirectCast(covariance.Clone(), Double(,))
            Dim vector As Double() = New Double(_D - 1) {}

            For component As Integer = 0 To _L - 1
                Dim norm As Double = 0

                For j As Integer = 0 To _D - 1
                    vector(j) = MathUtils.Randn(RNG, 0, 1)
                    norm += vector(j) * vector(j)
                Next

                norm = std.Sqrt(norm)

                If norm <= VBMath.TinyValue Then
                    Array.Clear(vector, 0, _D)
                    vector(0) = 1.0R
                Else
                    For j As Integer = 0 To _D - 1
                        vector(j) /= norm
                    Next
                End If

                For iter As Integer = 1 To 200
                    Dim candidate As Double() = MultiplyMatrixVector(deflated, vector)
                    Dim candidateNorm As Double = VectorNorm(candidate)

                    If candidateNorm <= VBMath.TinyValue Then
                        Exit For
                    End If

                    Dim delta As Double = 0

                    For j As Integer = 0 To _D - 1
                        candidate(j) /= candidateNorm
                        delta += std.Abs(candidate(j) - vector(j))
                    Next

                    vector = candidate

                    If delta < 0.0000000001R Then
                        Exit For
                    End If
                Next

                Dim eigenvalue As Double = RayleighQuotient(deflated, vector)

                If eigenvalue > VBMath.TinyValue Then
                    Dim scale As Double = std.Sqrt(eigenvalue)

                    For j As Integer = 0 To _D - 1
                        _mw(j, component) = vector(j) * scale
                    Next
                End If

                ' 收缩：去掉当前主方向对剩余矩阵的贡献
                For i As Integer = 0 To _D - 1
                    For j As Integer = 0 To _D - 1
                        deflated(i, j) -= eigenvalue * vector(i) * vector(j)
                    Next
                Next
            Next
        End Sub

        ''' <summary>
        ''' 中心化数据的协方差矩阵（缺失值使用成对删除）。
        ''' </summary>
        Private Function ComputeCovariance() As Double(,)
            Dim covariance As Double(,) = New Double(_D - 1, _D - 1) {}

            For i As Integer = 0 To _D - 1
                For j As Integer = 0 To _D - 1
                    Dim acc As Double = 0
                    Dim count As Integer = 0

                    For n As Integer = 0 To _N - 1
                        If _mask(n, i) AndAlso _mask(n, j) Then
                            acc += (_X(n, i) - _mean(i)) * (_X(n, j) - _mean(j))
                            count += 1
                        End If
                    Next

                    covariance(i, j) = If(count > 1, acc / (count - 1), 0.0R)
                Next
            Next

            Return covariance
        End Function

        Private Shared Function MultiplyMatrixVector(a As Double(,), v As Double()) As Double()
            Dim n As Integer = a.GetLength(0)
            Dim result As Double() = New Double(n - 1) {}

            For i As Integer = 0 To n - 1
                Dim acc As Double = 0

                For j As Integer = 0 To n - 1
                    acc += a(i, j) * v(j)
                Next

                result(i) = acc
            Next

            Return result
        End Function

        Private Shared Function VectorNorm(v As Double()) As Double
            Dim acc As Double = 0

            For i As Integer = 0 To v.Length - 1
                acc += v(i) * v(i)
            Next

            Return std.Sqrt(acc)
        End Function

        Private Shared Function RayleighQuotient(a As Double(,), v As Double()) As Double
            Dim product As Double() = MultiplyMatrixVector(a, v)
            Dim acc As Double = 0

            For i As Integer = 0 To v.Length - 1
                acc += v(i) * product(i)
            Next

            Return acc
        End Function

        Private Function SzAt(n As Integer, i As Integer, k As Integer) As Double
            Return _Sz(n * _L * _L + i * _L + k)
        End Function

        Private Sub SetSz(n As Integer, i As Integer, k As Integer, value As Double)
            _Sz(n * _L * _L + i * _L + k) = value
        End Sub

        Private Shared Sub Symmetrize(a As Double(,))
            Dim n As Integer = a.GetLength(0)

            For i As Integer = 0 To n - 1
                For k As Integer = i + 1 To n - 1
                    Dim average As Double = 0.5R * (a(i, k) + a(k, i))

                    a(i, k) = average
                    a(k, i) = average
                Next
            Next
        End Sub

#End Region

    End Class

End Namespace
