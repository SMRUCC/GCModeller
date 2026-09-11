Imports Microsoft.VisualBasic.Math.Statistics.Distributions
Imports std = System.Math

Namespace Statistics

    ''' <summary>
    ''' 截距型零膨胀 Poisson（zero-inflated Poisson, ZIP）模型的最大似然估计结果。
    ''' </summary>
    Public Structure ZipEstimate

        ''' <summary>
        ''' 结构零（structural zero）的概率 ``φ``。
        ''' </summary>
        Public ReadOnly Phi As Double
        ''' <summary>
        ''' Poisson 分量的均值 ``λ``。
        ''' </summary>
        Public ReadOnly Lambda As Double
        ''' <summary>
        ''' 窗口内结构零个数的期望值，即原始 R 实现之中的 ``expected_value``。
        ''' </summary>
        Public ReadOnly ExpectedStructuralZeros As Double
        ''' <summary>
        ''' 最大似然估计是否收敛；为 ``False`` 时该窗口不可建模，应当被丢弃。
        ''' </summary>
        Public ReadOnly Converged As Boolean

        Public Sub New(phi As Double, lambda As Double, expected As Double, converged As Boolean)
            Me.Phi = phi
            Me.Lambda = lambda
            Me.ExpectedStructuralZeros = expected
            Me.Converged = converged
        End Sub
    End Structure

    ''' <summary>
    ''' 截距型零膨胀 Poisson 模型的最大似然参数估计。
    ''' </summary>
    ''' <remarks>
    ''' 原始 TSSAR 的 R 实现使用 ``VGAM::vglm(y1 ~ x2, zapoisson(zero = 1))``，
    ''' 其中回归元 ``x2 = runif(n)`` 是纯随机噪声，其回归系数约等于 0。
    ''' 因此该模型在数学上等价于**截距型（常数参数）的 ZIP 模型**，
    ''' 本模块直接使用 EM 算法对该 2 参数模型做极大似然估计，
    ''' 避免了对外部 VGAM/R 运行时的依赖，并且更快、结果可控。
    ''' </remarks>
    Public Module ZipRegression

        Const MaxIterations As Integer = 1000
        Const Tolerance As Double = 1.0E-10

        ''' <summary>
        ''' 计算零膨胀 Poisson 分布的概率质量函数 ``dzipois(x, lambda, pstr0)``。
        ''' </summary>
        Public Function Density(x As Double, lambda As Double, phi As Double) As Double
            Return Skellam.dzipois(x, lambda, phi)
        End Function

        ''' <summary>
        ''' 对给定样本执行截距型 ZIP 模型的 EM 最大似然估计。
        ''' </summary>
        ''' <param name="y">窗口内的观测计数。</param>
        ''' <returns>估计结果；<see cref="ZipEstimate.Converged"/> 为 ``False`` 表示不可建模。</returns>
        Public Function Fit(y As Double()) As ZipEstimate
            Return Fit(y, y.Length)
        End Function

        ''' <summary>
        ''' 对给定样本执行截距型 ZIP 模型的 EM 最大似然估计（指定有效长度）。
        ''' </summary>
        ''' <param name="y">观测计数缓冲区。</param>
        ''' <param name="n">有效元素个数，用于复用缓冲区以避免频繁分配。</param>
        ''' <returns>估计结果；<see cref="ZipEstimate.Converged"/> 为 ``False`` 表示不可建模。</returns>
        Public Function Fit(y As Double(), n As Integer) As ZipEstimate

            If n = 0 Then
                Return New ZipEstimate(0, 0, 0, False)
            End If

            Dim zeros As Integer = 0
            Dim sumY As Double = 0
            Dim sumPositive As Double = 0
            Dim nPositive As Integer = 0

            For i As Integer = 0 To n - 1
                Dim v As Double = y(i)

                If v <= 0.0 Then
                    zeros += 1
                Else
                    sumPositive += v
                    nPositive += 1
                End If

                sumY += v
            Next

            ' 初值：用正计数部分的均值作为 lambda，用观测零比例反推 phi
            Dim lambda As Double = If(nPositive > 0, sumPositive / nPositive, 1.0)

            If lambda <= 0.0 Then
                lambda = 1.0
            End If

            Dim pz As Double = std.Exp(-lambda)
            Dim phi As Double

            If pz >= 1.0 Then
                phi = 0.0
            Else
                phi = (zeros / CDbl(n) - pz) / (1.0 - pz)
            End If

            If Double.IsNaN(phi) OrElse phi < 0.0 Then
                phi = 0.0
            ElseIf phi > 0.9 Then
                phi = 0.9
            End If

            Dim converged As Boolean = False

            For iteration As Integer = 1 To MaxIterations
                ' E 步：计算每一个观测零为结构零的后验概率
                Dim p0 As Double = phi + (1.0 - phi) * std.Exp(-lambda)

                If p0 <= 0.0 Then
                    Return New ZipEstimate(phi, lambda, 0, False)
                End If

                Dim w As Double = phi / p0
                Dim sumW As Double = zeros * w

                ' M 步
                Dim phiNew As Double = sumW / n
                Dim denominator As Double = n - sumW

                If denominator <= 1.0E-12 Then
                    ' 全部观测都被判定为结构零，无法继续估计 Poisson 均值
                    Return New ZipEstimate(phiNew, lambda, 0, False)
                End If

                Dim lambdaNew As Double = sumY / denominator
                Dim delta As Double = std.Abs(phiNew - phi) + std.Abs(lambdaNew - lambda)

                phi = phiNew
                lambda = lambdaNew

                If delta < Tolerance Then
                    converged = True
                    Exit For
                End If
            Next

            ' 期望结构零个数 = 零的个数 × φ / dzipois(0, λ, φ)
            Dim d0 As Double = Skellam.dzipois(0.0, lambda, phi)
            Dim expected As Double

            If d0 <= 0.0 Then
                expected = 0.0
            Else
                expected = zeros * (phi / d0)
            End If

            Return New ZipEstimate(phi, lambda, expected, converged)
        End Function
    End Module
End Namespace
