' ============================================================================
' LinearAlgebraOps.vb - 模型层的唯一线性代数入口
'
' 设计约束（单一数值边界）：
'   Tensor 与 NumericMatrix 的互转只允许出现在 VariationalBayes.MatrixBridge；
'   模型层只能依赖本文件，绝不直接 Imports LinearAlgebra.Matrix。
'
' 相对旧实现的关键改进：
'   1. 用 Cholesky 分解替代 Gauss-Jordan 求逆来计算马氏距离，
'      每个成分每轮只需一次 O(D³/3) 分解，而不是两次 O(D³)；
'   2. 分解失败不再直接 Throw，而是按 MatrixBridge.InverseOrPseudo 的
'      对角线抖动策略重试；
'   3. 采样改用 x = μ + L·z，保留协方差矩阵的相关性结构
'      （修正 GMM.vb L694-699 只取对角元的问题）。
' ============================================================================

Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.VariationalBayes
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports std = System.Math

Namespace Core.Numerics

    ''' <summary>
    ''' 模型层使用的线性代数算子：对数行列式、Cholesky 分解、马氏距离、协方差采样。
    ''' </summary>
    Public Module LinearAlgebraOps

        ''' <summary>方差的下限，避免 1/var 爆炸。</summary>
        Public Const MinVariance As Double = 0.0001

        ''' <summary>对数方差的下限。</summary>
        Public Const MinLogVariance As Double = -10.0

        ''' <summary>对数方差的上限。</summary>
        Public Const MaxLogVariance As Double = 4.0

        ''' <summary>协方差对角线抖动的初始幅度（相对矩阵最大元素）。</summary>
        Public Const RelativeJitter As Double = 1.0E-8

        ''' <summary>抖动重试次数上限。</summary>
        Public Const JitterAttempts As Integer = 6

#Region "基础分解"

        ''' <summary>方阵的 ln|A|；奇异矩阵返回 -Infinity。</summary>
        Public Function LogDeterminant(a As Tensor) As Double
            Return MatrixBridge.LogDet(a)
        End Function

        ''' <summary>方阵求逆；奇异时先抖动对角线，再回退到高斯-约当实现。</summary>
        Public Function Inverse(a As Tensor) As Tensor
            Return MatrixBridge.InverseOrPseudo(a)
        End Function

        ''' <summary>判断方阵是否对称正定。</summary>
        Public Function IsPositiveDefinite(a As Tensor) As Boolean
            Return MatrixBridge.IsPositiveDefinite(a)
        End Function

        ''' <summary>
        ''' 计算 Cholesky 下三角因子 L（满足 A = L·Lᵀ）。非正定或非方阵时返回 <c>Nothing</c>。
        ''' </summary>
        Public Function CholeskyFactor(a As Tensor) As Tensor
            If a Is Nothing OrElse a.Rank <> 2 Then
                Return Nothing
            End If

            If a.Shape(0) <> a.Shape(1) Then
                Return Nothing
            End If

            Try
                Dim matrix As NumericMatrix = MatrixBridge.ToMatrix(a)
                Dim chol As New CholeskyDecomposition(matrix)

                If Not chol.SPD Then
                    Return Nothing
                End If

                Return MatrixBridge.FromMatrix(chol.GetL())
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' 为协方差矩阵生成可用的 Cholesky 因子。失败时按对角线抖动重试，
        ''' 抖动幅度从 <see cref="RelativeJitter"/> 开始逐次放大 10 倍。
        ''' </summary>
        ''' <param name="covariance">需要分解的协方差矩阵</param>
        ''' <param name="jitterUsed">实际使用的抖动幅度（0 表示未抖动）</param>
        ''' <returns>下三角因子；全部尝试失败时返回 <c>Nothing</c></returns>
        Public Function CholeskyFactorWithJitter(covariance As Tensor, ByRef jitterUsed As Double) As Tensor
            jitterUsed = 0

            Dim factor As Tensor = CholeskyFactor(covariance)

            If factor IsNot Nothing Then
                Return factor
            End If

            Dim dimension As Integer = covariance.Shape(0)
            Dim scale As Double = 0

            For i As Integer = 0 To covariance.Length - 1
                Dim value As Double = std.Abs(covariance.Data(i))

                If value > scale Then
                    scale = value
                End If
            Next

            If scale <= 0 Then
                scale = 1.0
            End If

            Dim jitter As Double = RelativeJitter * scale

            For attempt As Integer = 1 To JitterAttempts
                Dim data As Double() = CType(covariance.Data.Clone(), Double())

                For i As Integer = 0 To dimension - 1
                    data(i * dimension + i) += jitter
                Next

                factor = CholeskyFactor(New Tensor(data, dimension, dimension))

                If factor IsNot Nothing Then
                    jitterUsed = jitter

                    Return factor
                End If

                jitter *= 10.0
            Next

            Return Nothing
        End Function

        ''' <summary>由 Cholesky 因子计算 ln|A| = 2·Σ ln L_ii。</summary>
        Public Function LogDeterminantFromFactor(lower As Tensor) As Double
            Dim dimension As Integer = lower.Shape(0)
            Dim acc As Double = 0

            For i As Integer = 0 To dimension - 1
                Dim diagonal As Double = lower(i, i)

                If diagonal <= 0 Then
                    Return Double.NegativeInfinity
                End If

                acc += std.Log(diagonal)
            Next

            Return 2.0 * acc
        End Function

#End Region

#Region "马氏距离与密度"

        ''' <summary>
        ''' 计算 (x-μ)ᵀΣ⁻¹(x-μ)。给定 Cholesky 因子 L，通过前代求解 L·y = (x-μ)
        ''' 再取 ‖y‖²，无需显式求逆。
        ''' </summary>
        Public Function MahalanobisSquared(lower As Tensor, diff As Double()) As Double
            Dim dimension As Integer = diff.Length
            Dim y As Double() = New Double(dimension - 1) {}

            For i As Integer = 0 To dimension - 1
                Dim acc As Double = diff(i)

                For j As Integer = 0 To i - 1
                    acc -= lower(i, j) * y(j)
                Next

                Dim diagonal As Double = lower(i, i)

                If std.Abs(diagonal) < 1.0E-300 Then
                    Return Double.PositiveInfinity
                End If

                y(i) = acc / diagonal
            Next

            Dim squared As Double = 0

            For i As Integer = 0 To dimension - 1
                squared += y(i) * y(i)
            Next

            Return squared
        End Function

        ''' <summary>多元正态的对数密度 ln N(x | μ, Σ)，给定 ln|Σ| 与马氏距离平方。</summary>
        Public Function LogPdf(dimension As Integer, logDeterminant As Double, mahalanobisSquared As Double) As Double
            Return -0.5 * (dimension * std.Log(2.0 * std.PI) + logDeterminant + mahalanobisSquared)
        End Function

#End Region

#Region "采样"

        ''' <summary>
        ''' 从 N(mean, Σ) 采样，其中 Σ = L·Lᵀ。
        ''' 使用 <c>x = μ + L·z</c>，完整保留协方差矩阵的非对角相关性。
        ''' </summary>
        Public Function SampleGaussian(rng As RandomGaussian, mean As Double(), lower As Tensor) As Double()
            Dim dimension As Integer = mean.Length
            Dim z As Double() = rng.NextNormals(dimension)
            Dim x As Double() = New Double(dimension - 1) {}

            For i As Integer = 0 To dimension - 1
                Dim acc As Double = mean(i)

                For j As Integer = 0 To i
                    acc += lower(i, j) * z(j)
                Next

                x(i) = acc
            Next

            Return x
        End Function

#End Region

#Region "协方差工具"

        ''' <summary>把协方差矩阵的对角线抬到指定的最小方差之上。</summary>
        Public Function EnsureDiagonalFloor(covariance As Tensor, Optional floor As Double = MinVariance) As Tensor
            Dim dimension As Integer = covariance.Shape(0)
            Dim data As Double() = CType(covariance.Data.Clone(), Double())

            For i As Integer = 0 To dimension - 1
                Dim index As Integer = i * dimension + i

                If data(index) < floor Then
                    data(index) = floor
                End If
            Next

            Return New Tensor(data, dimension, dimension)
        End Function

        ''' <summary>构造对角协方差矩阵（由方差向量）。</summary>
        Public Function DiagonalCovariance(variance As Double()) As Tensor
            Dim dimension As Integer = variance.Length
            Dim data As Double() = New Double(dimension * dimension - 1) {}

            For i As Integer = 0 To dimension - 1
                data(i * dimension + i) = std.Max(variance(i), MinVariance)
            Next

            Return New Tensor(data, dimension, dimension)
        End Function

#End Region

    End Module

End Namespace
