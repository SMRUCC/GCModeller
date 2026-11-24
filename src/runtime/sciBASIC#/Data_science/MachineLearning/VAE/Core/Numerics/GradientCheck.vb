' ============================================================================
' GradientCheck.vb - 中心差分梯度校验
'
' 手工反向传播最容易出错的地方就是梯度方向与缩放因子。本工具用于在审计
' 项目中把「解析梯度」与「数值梯度」对齐，是修复 GMVAE 重参数化梯度
' （GMVAE.vb L511 用 σ 代替 ε）与先验方差梯度（L599-605 方向相反）的依据。
' ============================================================================

Imports std = System.Math

Namespace Core.Numerics

    ''' <summary>
    ''' 中心差分梯度校验。
    ''' </summary>
    Public Module GradientCheck

        ''' <summary>默认的差分步长。</summary>
        Public Const DefaultEpsilon As Double = 1.0E-6

        ''' <summary>
        ''' 对第 <paramref name="index"/> 个分量做中心差分：(f(x+h) - f(x-h)) / 2h。
        ''' </summary>
        Public Function CentralDifference(loss As Func(Of Double(), Double),
                                          point As Double(),
                                          index As Integer,
                                          Optional epsilon As Double = DefaultEpsilon) As Double
            Dim plus As Double() = CType(point.Clone(), Double())
            Dim minus As Double() = CType(point.Clone(), Double())

            plus(index) += epsilon
            minus(index) -= epsilon

            Return (loss(plus) - loss(minus)) / (2.0 * epsilon)
        End Function

        ''' <summary>对全部分量做中心差分。</summary>
        Public Function Elementwise(loss As Func(Of Double(), Double),
                                    point As Double(),
                                    Optional epsilon As Double = DefaultEpsilon) As Double()
            Dim result As Double() = New Double(point.Length - 1) {}

            For i As Integer = 0 To point.Length - 1
                result(i) = CentralDifference(loss, point, i, epsilon)
            Next

            Return result
        End Function

        ''' <summary>
        ''' 解析梯度与数值梯度之间的最大相对误差。
        ''' </summary>
        ''' <param name="analytic">解析梯度</param>
        ''' <param name="numeric">数值梯度</param>
        ''' <param name="absoluteFloor">分母下限，避免梯度接近 0 时误差被放大</param>
        Public Function MaxRelativeError(analytic As Double(), numeric As Double(),
                                         Optional absoluteFloor As Double = 1.0E-8) As Double
            Dim count As Integer = std.Min(analytic.Length, numeric.Length)
            Dim maxError As Double = 0

            For i As Integer = 0 To count - 1
                Dim a As Double = analytic(i)
                Dim n As Double = numeric(i)

                If Double.IsNaN(a) OrElse Double.IsNaN(n) Then
                    Return Double.PositiveInfinity
                End If

                Dim denominator As Double = std.Max(absoluteFloor, std.Abs(a) + std.Abs(n))
                Dim errorValue As Double = std.Abs(a - n) / denominator

                If errorValue > maxError Then
                    maxError = errorValue
                End If
            Next

            Return maxError
        End Function

        ''' <summary>
        ''' 元素级 L2 梯度裁剪：把梯度缩放到指定范数上限之内（原地修改）。
        ''' </summary>
        Public Function ClipByNorm(gradient As Double(), maxNorm As Double) As Double
            If maxNorm <= 0 Then
                Return 0
            End If

            Dim squared As Double = 0

            For i As Integer = 0 To gradient.Length - 1
                squared += gradient(i) * gradient(i)
            Next

            Dim norm As Double = std.Sqrt(squared)

            If norm <= maxNorm OrElse norm <= 0 Then
                Return norm
            End If

            Dim scale As Double = maxNorm / norm

            For i As Integer = 0 To gradient.Length - 1
                gradient(i) *= scale
            Next

            Return maxNorm
        End Function

    End Module

End Namespace
