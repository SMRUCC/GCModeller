' ============================================================================
' VariationalLinearRegression.vb - 变分贝叶斯线性回归
'
' 模型：
'   w | α ~ N(0, α^-1 I)         权重
'   α     ~ Gamma(a0, b0)        权重的精度（超参数）
'   t | w, β ~ N(Φw, β^-1 I)     观测噪声
'   β     ~ Gamma(c0, d0)        噪声精度（超参数）
'
' 均值场近似：
'   q(w) = N(w | m, S),  q(α) = Gamma(a, b),  q(β) = Gamma(c, d)
'
' 与基于证据近似（type-II maximum likelihood）的经典贝叶斯线性回归相比，
' 这里对 α 与 β 也做变分近似，因此下界 ELBO 是严格的证据下界。
'
' 参考：Bishop, "Pattern Recognition and Machine Learning", 第 10.3 节
' ============================================================================

Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

Namespace VariationalBayes

    ''' <summary>
    ''' 变分贝叶斯线性回归（Variational Bayesian Linear Regression）。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 对权重 w、权重精度 α 以及噪声精度 β 同时做均值场近似，
    ''' 迭代更新三个变分因子并最大化证据下界。
    ''' </para>
    ''' <para>
    ''' 目标值通过 <see cref="Fit(Tensor, Tensor)"/> 传入；<see cref="Fit(Tensor)"/> 的单参数
    ''' 版本没有意义，调用时会抛出异常。
    ''' </para>
    ''' </remarks>
    Public Class VariationalLinearRegression
        Inherits VariationalInference

#Region "先验超参数"

        ''' <summary>权重精度先验 Gamma(a0, b0) 的形状参数 a0</summary>
        Public Property PriorAlphaA0 As Double = 0.01R

        ''' <summary>权重精度先验 Gamma(a0, b0) 的率参数 b0</summary>
        Public Property PriorAlphaB0 As Double = 0.01R

        ''' <summary>噪声精度先验 Gamma(c0, d0) 的形状参数 c0</summary>
        Public Property PriorBetaC0 As Double = 0.01R

        ''' <summary>噪声精度先验 Gamma(c0, d0) 的率参数 d0</summary>
        Public Property PriorBetaD0 As Double = 0.01R

#End Region

#Region "内部状态"

        Private _targets As Double()
        Private _Phi As Double(,)
        Private _N As Integer
        Private _M As Integer

        Private _wMean As Double()
        Private _S As Double(,)
        Private _phiTphi As Double(,)
        Private _phiTt As Double()

        Private _a As Double
        Private _b As Double
        Private _c As Double
        Private _d As Double

        Private _fitted As Boolean = False

#End Region

#Region "构造"

        ''' <summary>
        ''' 创建变分贝叶斯线性回归模型。
        ''' </summary>
        Public Sub New(Optional maxIter As Integer = 200,
                       Optional tol As Double = 0.0000001R,
                       Optional seed As Integer? = Nothing)
            Me.MaxIter = maxIter
            Me.Tolerance = tol
            Me.Seed = seed
        End Sub

#End Region

#Region "训练结果"

        ''' <summary>权重的后验均值 m [M]</summary>
        Public ReadOnly Property Mean As Tensor
            Get
                If Not _fitted Then
                    Return Nothing
                End If

                Return New Tensor(DirectCast(_wMean.Clone(), Double()), _M)
            End Get
        End Property

        ''' <summary>权重的后验协方差 S [M, M]</summary>
        Public ReadOnly Property Covariance As Tensor
            Get
                If Not _fitted Then
                    Return Nothing
                End If

                Return New Tensor(DirectCast(_S.Clone(), Double()), _M, _M)
            End Get
        End Property

        ''' <summary>权重精度的后验期望 E[α] = a / b</summary>
        Public ReadOnly Property Alpha As Double
            Get
                Return _a / _b
            End Get
        End Property

        ''' <summary>噪声精度的后验期望 E[β] = c / d</summary>
        Public ReadOnly Property Beta As Double
            Get
                Return _c / _d
            End Get
        End Property

        ''' <summary>噪声方差的后验估计 1 / E[β]</summary>
        Public ReadOnly Property ResidualVariance As Double
            Get
                Return 1.0R / Beta
            End Get
        End Property

        ''' <summary>Gamma 后验 q(α) 的形状参数</summary>
        Public ReadOnly Property AlphaShape As Double
            Get
                Return _a
            End Get
        End Property

        ''' <summary>Gamma 后验 q(α) 的率参数</summary>
        Public ReadOnly Property AlphaRate As Double
            Get
                Return _b
            End Get
        End Property

        ''' <summary>Gamma 后验 q(β) 的形状参数</summary>
        Public ReadOnly Property BetaShape As Double
            Get
                Return _c
            End Get
        End Property

        ''' <summary>Gamma 后验 q(β) 的率参数</summary>
        Public ReadOnly Property BetaRate As Double
            Get
                Return _d
            End Get
        End Property

#End Region

#Region "训练"

        ''' <summary>
        ''' 单参数的 <see cref="VariationalInference.Fit(Tensor)"/> 在回归任务中没有意义。
        ''' </summary>
        Public Shadows Sub Fit(X As Tensor)
            Throw New InvalidOperationException(
                "变分贝叶斯线性回归需要同时提供设计矩阵与目标值，请使用 Fit(X, y)")
        End Sub

        ''' <summary>
        ''' 使用设计矩阵 X [N, M] 与目标值 y [N]（或者 [N, 1]）训练模型。
        ''' </summary>
        Public Shadows Sub Fit(X As Tensor, y As Tensor)
            If X Is Nothing Then
                Throw New ArgumentNullException(NameOf(X))
            End If

            If y Is Nothing Then
                Throw New ArgumentNullException(NameOf(y))
            End If

            If X.Rank <> 2 Then
                Throw New ArgumentException($"设计矩阵必须是二维张量 [N, M]，实际秩为 {X.Rank}", NameOf(X))
            End If

            _targets = ExtractTargets(y, X.Shape(0))

            Call MyBase.Fit(X)
        End Sub

        ''' <summary>
        ''' 使用矩形数组训练模型的便捷重载。
        ''' </summary>
        Public Shadows Sub Fit(X As Double(,), y As Double())
            If y Is Nothing Then
                Throw New ArgumentNullException(NameOf(y))
            End If

            Call Fit(New Tensor(X), New Tensor(y, y.Length))
        End Sub

#End Region

#Region "预测"

        ''' <summary>
        ''' 返回新设计矩阵上的预测均值 Φnew · m。
        ''' </summary>
        Public Function Predict(X As Tensor) As Tensor
            Call EnsureFitted()

            Dim phi As Double(,) = ValidateDesign(X)
            Dim n As Integer = phi.GetLength(0)
            Dim result As Double() = New Double(n - 1) {}

            For i As Integer = 0 To n - 1
                Dim acc As Double = 0

                For j As Integer = 0 To _M - 1
                    acc += phi(i, j) * _wMean(j)
                Next

                result(i) = acc
            Next

            Return New Tensor(result, n)
        End Function

        ''' <summary>
        ''' 返回新设计矩阵上的预测方差：1/E[β] + φ' S φ。
        ''' </summary>
        Public Function PredictiveVariance(X As Tensor) As Tensor
            Call EnsureFitted()

            Dim phi As Double(,) = ValidateDesign(X)
            Dim n As Integer = phi.GetLength(0)
            Dim noise As Double = ResidualVariance
            Dim result As Double() = New Double(n - 1) {}

            For i As Integer = 0 To n - 1
                Dim acc As Double = 0

                For j As Integer = 0 To _M - 1
                    Dim rowAcc As Double = 0

                    For k As Integer = 0 To _M - 1
                        rowAcc += _S(j, k) * phi(i, k)
                    Next

                    acc += phi(i, j) * rowAcc
                Next

                result(i) = noise + acc
            Next

            Return New Tensor(result, n)
        End Function

        ''' <summary>
        ''' 同时返回预测均值与预测方差。
        ''' </summary>
        Public Function PredictWithVariance(X As Tensor, ByRef variance As Tensor) As Tensor
            Dim mean As Tensor = Predict(X)

            variance = PredictiveVariance(X)

            Return mean
        End Function

        Public Overrides Function ToString() As String
            If Not _fitted Then
                Return "VB-LinearRegression (未训练)"
            End If

            Return $"VB-LinearRegression: N={_N}, M={_M}, E[α]={Alpha:F4}, E[β]={Beta:F4}, " &
                   $"ELBO={ELBO:F4}, iter={NIter}, converged={Converged}"
        End Function

#End Region

#Region "变分推断钩子"

        Protected Overrides Sub InitializeModel(X As Tensor)
            _N = X.Shape(0)
            _M = X.Shape(1)

            If _M < 1 Then
                Throw New ArgumentException("设计矩阵至少需要包含一列特征", NameOf(X))
            End If

            If _targets Is Nothing OrElse _targets.Length <> _N Then
                Throw New InvalidOperationException("目标值的长度与设计矩阵的样本数不一致")
            End If

            If PriorAlphaA0 <= 0 OrElse PriorAlphaB0 <= 0 OrElse
               PriorBetaC0 <= 0 OrElse PriorBetaD0 <= 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(PriorAlphaA0), "Gamma 先验参数必须为正数")
            End If

            _Phi = X.To2DArrayDouble()
            _phiTphi = New Double(_M - 1, _M - 1) {}
            _phiTt = New Double(_M - 1) {}
            _wMean = New Double(_M - 1) {}
            _S = New Double(_M - 1, _M - 1) {}

            ' Φ'Φ 与 Φ't
            For i As Integer = 0 To _M - 1
                For j As Integer = 0 To _M - 1
                    Dim acc As Double = 0

                    For n As Integer = 0 To _N - 1
                        acc += _Phi(n, i) * _Phi(n, j)
                    Next

                    _phiTphi(i, j) = acc
                Next

                Dim accT As Double = 0

                For n As Integer = 0 To _N - 1
                    accT += _Phi(n, i) * _targets(n)
                Next

                _phiTt(i) = accT
            Next

            ' 初始化 q(α) 与 q(β)：E[α] 取量纲 1 附近，E[β] 取目标值方差的倒数
            Dim variance As Double = SampleVariance(_targets)

            If variance <= VBMath.TinyValue Then
                variance = 1.0R
            End If

            _a = PriorAlphaA0 + 0.5R * _M
            _b = PriorAlphaB0 + 0.5R
            _c = PriorBetaC0 + 0.5R * _N
            _d = PriorBetaD0 + 0.5R * _N * variance

            Call UpdateWeightPosterior()

            _fitted = True
        End Sub

        ''' <summary>
        ''' E 步：固定 q(α) 与 q(β)，更新权重的后验 q(w)。
        ''' </summary>
        Protected Overrides Sub Expectation()
            Call UpdateWeightPosterior()
        End Sub

        ''' <summary>
        ''' M 步：固定 q(w)，更新两个 Gamma 后验 q(α) 与 q(β)。
        ''' </summary>
        Protected Overrides Sub Maximization()
            ' E[||w||²] = m'm + tr(S)
            Dim normW2 As Double = 0

            For i As Integer = 0 To _M - 1
                normW2 += _wMean(i) * _wMean(i)
                normW2 += _S(i, i)
            Next

            _a = PriorAlphaA0 + 0.5R * _M
            _b = PriorAlphaB0 + 0.5R * normW2

            ' E[||t - Φw||²] = ||t - Φm||² + tr(S Φ'Φ)
            Dim residual As Double = 0

            For n As Integer = 0 To _N - 1
                Dim predicted As Double = 0

                For j As Integer = 0 To _M - 1
                    predicted += _Phi(n, j) * _wMean(j)
                Next

                Dim diff As Double = _targets(n) - predicted
                residual += diff * diff
            Next

            For i As Integer = 0 To _M - 1
                For j As Integer = 0 To _M - 1
                    residual += _S(i, j) * _phiTphi(j, i)
                Next
            Next

            If residual < VBMath.TinyValue Then
                residual = VBMath.TinyValue
            End If

            _c = PriorBetaC0 + 0.5R * _N
            _d = PriorBetaD0 + 0.5R * residual
        End Sub

        ''' <summary>
        ''' 计算证据下界 ELBO。
        ''' </summary>
        Protected Overrides Function ComputeELBO() As Double
            Dim eAlpha As Double = _a / _b
            Dim eBeta As Double = _c / _d
            Dim psiAlpha As Double = VBMath.Digamma(_a)
            Dim psiBeta As Double = VBMath.Digamma(_c)
            Dim lnAlpha As Double = psiAlpha - std.Log(_b)
            Dim lnBeta As Double = psiBeta - std.Log(_d)
            Dim lnTwoPi As Double = std.Log(2.0R * std.PI)

            ' E[||w||²] 与 E[||t - Φw||²]
            Dim normW2 As Double = 0

            For i As Integer = 0 To _M - 1
                normW2 += _wMean(i) * _wMean(i) + _S(i, i)
            Next

            Dim residual As Double = 0

            For n As Integer = 0 To _N - 1
                Dim predicted As Double = 0

                For j As Integer = 0 To _M - 1
                    predicted += _Phi(n, j) * _wMean(j)
                Next

                Dim diff As Double = _targets(n) - predicted
                residual += diff * diff
            Next

            For i As Integer = 0 To _M - 1
                For j As Integer = 0 To _M - 1
                    residual += _S(i, j) * _phiTphi(j, i)
                Next
            Next

            ' 1. E[ln p(t|w,β)]
            Dim term1 As Double = 0.5R * _N * lnBeta -
                                  0.5R * _N * lnTwoPi -
                                  0.5R * eBeta * residual

            ' 2. E[ln p(w|α)]
            Dim term2 As Double = 0.5R * _M * lnAlpha -
                                  0.5R * _M * lnTwoPi -
                                  0.5R * eAlpha * normW2

            ' 3. E[ln p(α)]
            Dim term3 As Double = PriorAlphaA0 * std.Log(PriorAlphaB0) -
                                  VBMath.LogGamma(PriorAlphaA0) +
                                  (PriorAlphaA0 - 1.0R) * lnAlpha -
                                  PriorAlphaB0 * eAlpha

            ' 4. E[ln p(β)]
            Dim term4 As Double = PriorBetaC0 * std.Log(PriorBetaD0) -
                                  VBMath.LogGamma(PriorBetaC0) +
                                  (PriorBetaC0 - 1.0R) * lnBeta -
                                  PriorBetaD0 * eBeta

            ' 5. -E[ln q(w)] = H[q(w)]
            Dim term5 As Double = 0.5R * MatrixBridge.LogDet(_S) +
                                  0.5R * _M * (1.0R + lnTwoPi)

            ' 6. -E[ln q(α)]
            Dim term6 As Double = VBMath.LogGamma(_a) - std.Log(_b) -
                                  (_a - 1.0R) * psiAlpha + _a

            ' 7. -E[ln q(β)]
            Dim term7 As Double = VBMath.LogGamma(_c) - std.Log(_d) -
                                  (_c - 1.0R) * psiBeta + _c

            Return term1 + term2 + term3 + term4 + term5 + term6 + term7
        End Function

#End Region

#Region "内部实现"

        Private Sub EnsureFitted()
            If Not _fitted Then
                Throw New InvalidOperationException("模型尚未训练，请先调用 Fit 方法")
            End If
        End Sub

        Private Function ValidateDesign(X As Tensor) As Double(,)
            If X Is Nothing Then
                Throw New ArgumentNullException(NameOf(X))
            End If

            If X.Rank <> 2 Then
                Throw New ArgumentException($"设计矩阵必须是二维张量 [N, M]，实际秩为 {X.Rank}", NameOf(X))
            End If

            If X.Shape(1) <> _M Then
                Throw New ArgumentException($"设计矩阵的列数 {X.Shape(1)} 与模型的权重维度 {_M} 不一致", NameOf(X))
            End If

            Return X.To2DArrayDouble()
        End Function

        Private Sub UpdateWeightPosterior()
            Dim eAlpha As Double = _a / _b
            Dim eBeta As Double = _c / _d

            ' S^-1 = E[α] I + E[β] Φ'Φ
            Dim precision As Double(,) = New Double(_M - 1, _M - 1) {}

            For i As Integer = 0 To _M - 1
                For j As Integer = 0 To _M - 1
                    precision(i, j) = eBeta * _phiTphi(i, j)
                Next

                precision(i, i) += eAlpha
            Next

            _S = MatrixBridge.InverseOrPseudo(precision)
            Call Symmetrize(_S)

            ' m = E[β] S Φ't
            For i As Integer = 0 To _M - 1
                Dim acc As Double = 0

                For j As Integer = 0 To _M - 1
                    acc += _S(i, j) * _phiTt(j)
                Next

                _wMean(i) = eBeta * acc
            Next
        End Sub

        Private Shared Function SampleVariance(values As Double()) As Double
            If values Is Nothing OrElse values.Length < 2 Then
                Return 0
            End If

            Dim mean As Double = 0

            For i As Integer = 0 To values.Length - 1
                mean += values(i)
            Next

            mean /= values.Length

            Dim acc As Double = 0

            For i As Integer = 0 To values.Length - 1
                Dim diff As Double = values(i) - mean
                acc += diff * diff
            Next

            Return acc / (values.Length - 1)
        End Function

        Private Shared Function ExtractTargets(y As Tensor, expected As Integer) As Double()
            Dim result As Double()

            Select Case y.Rank
                Case 1
                    If y.Length <> expected Then
                        Throw New ArgumentException(
                            $"目标值的长度 {y.Length} 与设计矩阵的样本数 {expected} 不一致", NameOf(y))
                    End If

                    result = DirectCast(y.Data.Clone(), Double())
                Case 2
                    If y.Shape(1) <> 1 Then
                        Throw New ArgumentException("二维目标值张量必须是列向量 [N, 1]", NameOf(y))
                    End If

                    If y.Shape(0) <> expected Then
                        Throw New ArgumentException(
                            $"目标值的长度 {y.Shape(0)} 与设计矩阵的样本数 {expected} 不一致", NameOf(y))
                    End If

                    result = New Double(expected - 1) {}

                    For i As Integer = 0 To expected - 1
                        result(i) = y(i, 0)
                    Next
                Case Else
                    Throw New ArgumentException($"目标值必须是一维或者 [N,1] 的二维张量，实际秩为 {y.Rank}", NameOf(y))
            End Select

            Return result
        End Function

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

#End Region

#Region "设计矩阵辅助"

        ''' <summary>
        ''' 在原始特征矩阵的左侧追加一列全 1 的截距项。
        ''' </summary>
        Public Shared Function DesignMatrix(X As Tensor, Optional addIntercept As Boolean = True) As Tensor
            If X Is Nothing Then
                Throw New ArgumentNullException(NameOf(X))
            End If

            If X.Rank <> 2 Then
                Throw New ArgumentException($"特征矩阵必须是二维张量 [N, D]，实际秩为 {X.Rank}", NameOf(X))
            End If

            Dim n As Integer = X.Shape(0)
            Dim d As Integer = X.Shape(1)
            Dim cols As Integer = If(addIntercept, d + 1, d)
            Dim data As Double() = New Double(n * cols - 1) {}
            Dim offset As Integer = 0

            For i As Integer = 0 To n - 1
                If addIntercept Then
                    data(offset) = 1.0R
                    offset += 1
                End If

                For j As Integer = 0 To d - 1
                    data(offset) = X(i, j)
                    offset += 1
                Next
            Next

            Return New Tensor(data, n, cols)
        End Function

        ''' <summary>
        ''' 对特征的每一列做中心化与标准化，并输出所使用的均值与标准差。
        ''' </summary>
        Public Shared Function StandardizeColumns(X As Tensor,
                                                  ByRef mean As Double(),
                                                  ByRef scale As Double()) As Tensor
            If X Is Nothing Then
                Throw New ArgumentNullException(NameOf(X))
            End If

            If X.Rank <> 2 Then
                Throw New ArgumentException($"特征矩阵必须是二维张量 [N, D]，实际秩为 {X.Rank}", NameOf(X))
            End If

            Dim n As Integer = X.Shape(0)
            Dim d As Integer = X.Shape(1)

            mean = New Double(d - 1) {}
            scale = New Double(d - 1) {}

            For j As Integer = 0 To d - 1
                Dim acc As Double = 0

                For i As Integer = 0 To n - 1
                    acc += X(i, j)
                Next

                mean(j) = acc / n

                Dim variance As Double = 0

                For i As Integer = 0 To n - 1
                    Dim diff As Double = X(i, j) - mean(j)
                    variance += diff * diff
                Next

                scale(j) = std.Sqrt(variance / n)

                If scale(j) <= VBMath.TinyValue Then
                    scale(j) = 1.0R
                End If
            Next

            Return ApplyStandardization(X, mean, scale)
        End Function

        ''' <summary>
        ''' 使用给定的均值与标准差对特征矩阵做标准化。
        ''' </summary>
        Public Shared Function ApplyStandardization(X As Tensor, mean As Double(), scale As Double()) As Tensor
            If X Is Nothing Then
                Throw New ArgumentNullException(NameOf(X))
            End If

            If mean Is Nothing OrElse scale Is Nothing Then
                Throw New ArgumentNullException(NameOf(mean), "均值与标准差不能为空")
            End If

            Dim n As Integer = X.Shape(0)
            Dim d As Integer = X.Shape(1)

            If mean.Length <> d OrElse scale.Length <> d Then
                Throw New ArgumentException("均值/标准差的长度与特征维度不一致")
            End If

            Dim data As Double() = New Double(n * d - 1) {}
            Dim offset As Integer = 0

            For i As Integer = 0 To n - 1
                For j As Integer = 0 To d - 1
                    data(offset) = (X(i, j) - mean(j)) / scale(j)
                    offset += 1
                Next
            Next

            Return New Tensor(data, n, d)
        End Function

#End Region

    End Class

End Namespace
