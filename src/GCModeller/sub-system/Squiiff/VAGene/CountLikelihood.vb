' ============================================================================
' CountLikelihood.vb - 单细胞计数数据的似然
'
' 单细胞表达量是**过度离散的非负整数计数**，用高斯 + MSE 拟合会把稀有基因
' 平滑掉，生成的数据在生物学上没有参考价值。这里提供四种似然：
'   Gaussian                      连续值（标准化后的表达量）
'   Poisson                       计数的等离散基准
'   NegativeBinomial (NB)         允许方差 > 均值的过度离散（最常用）
'   ZeroInflatedNegativeBinomial  额外建模 dropout 造成的零膨胀
'
' 所有数值实现都走 lgamma 与 log-sum-exp，避免计数较大时的溢出。
' ============================================================================

Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.Core.Numerics
Imports VB = Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.VariationalBayes.VBMath
Imports std = System.Math

''' <summary>计数似然的类型。</summary>
Public Enum CountLikelihoodKind
    ''' <summary>高斯似然（连续值输入）。</summary>
    Gaussian = 0
    ''' <summary>泊松似然（等离散计数）。</summary>
    Poisson = 1
    ''' <summary>负二项似然（过度离散计数，推荐）。</summary>
    NegativeBinomial = 2
    ''' <summary>零膨胀负二项似然（含 dropout 建模）。</summary>
    ZeroInflatedNegativeBinomial = 3
End Enum

''' <summary>
''' 逐元素的计数似然：值域、对数似然与「对 log 均值」的梯度。
''' </summary>
''' <remarks>
''' 梯度以 <c>-∂ln p / ∂ln μ</c> 的形式给出（即负对数似然对 log 均值的梯度），
''' 因为解码器的输出通常直接就是 log 均值。
''' </remarks>
Public Class CountLikelihood

    ''' <summary>似然类型。</summary>
    Public Property Kind As CountLikelihoodKind = CountLikelihoodKind.NegativeBinomial

    ''' <summary>负二项 / 零膨胀负二项的离散度 θ（越大越接近泊松）。</summary>
    Public Property Dispersion As Double = 1.0

    ''' <summary>零膨胀比例 π，取值 [0, 1)。</summary>
    Public Property ZeroInflation As Double = 0.05

    Public Sub New()
    End Sub

    Public Sub New(kind As CountLikelihoodKind)
        Me.Kind = kind
    End Sub

    ''' <summary>用给定的均值计算观测计数的对数似然。</summary>
    Public Function LogLikelihood(observed As Double, mean As Double) As Double
        Select Case Kind
            Case CountLikelihoodKind.Gaussian
                Dim diff As Double = observed - mean
                Return -0.5 * (diff * diff + std.Log(2.0 * std.PI))
            Case CountLikelihoodKind.Poisson
                Dim rate As Double = std.Max(mean, 1.0E-12)
                Return observed * std.Log(rate) - rate - LogFactorial(observed)
            Case CountLikelihoodKind.NegativeBinomial
                Return NegativeBinomialLogProbability(observed, mean)
            Case Else
                Return ZeroInflatedLogProbability(observed, mean)
        End Select
    End Function

    ''' <summary>
    ''' 负对数似然对 <c>ln μ</c> 的梯度（用于解码器反向传播）。
    ''' </summary>
    Public Function GradientLogMean(observed As Double, mean As Double) As Double
        Dim rate As Double = std.Max(mean, 1.0E-12)

        Select Case Kind
            Case CountLikelihoodKind.Gaussian
                ' 高斯模式下 MeanFromLogMean 是恒等映射（解码器输出就是均值），
                ' 因此对输出的梯度就是 ∂(-ln p)/∂μ = μ − y。
                ' -ln p = 0.5(y-μ)² ⇒ ∂/∂μ = μ-y
                Return rate - observed
            Case CountLikelihoodKind.Poisson
                ' ∂(-ln p)/∂μ = 1 - y/μ
                Return rate - observed
            Case CountLikelihoodKind.NegativeBinomial
                Dim theta As Double = std.Max(Dispersion, 1.0E-6)
                ' ∂(-ln p)/∂μ = (θ+y)/(θ+μ) - y/μ
                Return rate * (theta + observed) / (theta + rate) - observed
            Case Else
                Return ZeroInflatedGradient(observed, rate)
        End Select
    End Function

    ''' <summary>
    ''' 由解码器输出得到均值。计数类似然使用 softplus 之外的指数映射并加上文库大小，
    ''' 这里返回 <c>exp(output)</c>（调用方负责把 log 文库大小并入 output）。
    ''' </summary>
    Public Function MeanFromLogMean(logMean As Double) As Double
        If Kind = CountLikelihoodKind.Gaussian Then
            Return logMean
        End If

        Return std.Exp(std.Min(logMean, 30.0))
    End Function

#Region "内部数值实现"

    Private Shared Function LogFactorial(value As Double) As Double
        Return VB.LogGamma(value + 1.0)
    End Function

    Private Function NegativeBinomialLogProbability(observed As Double, mean As Double) As Double
        Dim theta As Double = std.Max(Dispersion, 1.0E-6)
        Dim rate As Double = std.Max(mean, 1.0E-12)

        ' ln Γ(y+θ) - ln Γ(θ) - ln Γ(y+1) + θ·ln(θ/(θ+μ)) + y·ln(μ/(θ+μ))
        Return VB.LogGamma(observed + theta) - VB.LogGamma(theta) - LogFactorial(observed) +
               theta * std.Log(theta / (theta + rate)) +
               observed * std.Log(rate / (theta + rate))
    End Function

    Private Function ZeroInflatedLogProbability(observed As Double, mean As Double) As Double
        Dim rate As Double = std.Max(mean, 1.0E-12)
        Dim pi As Double = std.Max(0, std.Min(0.999999, ZeroInflation))
        Dim logInflation As Double = std.Log(std.Max(pi, 1.0E-12)) 
        Dim logRegular As Double = std.Log(std.Max(1.0 - pi, 1.0E-12))

        If observed > 0 Then
            ' 正计数只能来自 NB 分支
            Return logRegular + NegativeBinomialLogProbability(observed, rate)
        End If

        ' y = 0：两个分支都可能贡献
        Dim logNegativeBinomialZero As Double = NegativeBinomialLogProbability(0, rate)
        Dim values As Double() = {logInflation, logRegular + logNegativeBinomialZero}

        Return LogSumExpOps.Compute(values)
    End Function

    Private Function ZeroInflatedGradient(observed As Double, rate As Double) As Double
        Dim theta As Double = std.Max(Dispersion, 1.0E-6)

        If observed > 0 Then
            ' 与 NB 相同（零膨胀只影响常数项）
            Return rate * (theta + observed) / (theta + rate) - observed
        End If

        Dim pi As Double = std.Max(0, std.Min(0.999999, ZeroInflation))
        Dim probabilityZero As Double = (theta / (theta + rate)) ^ theta
        Dim numerator As Double = pi + (1.0 - pi) * probabilityZero

        If numerator <= 1.0E-300 Then
            Return 0
        End If

        ' dNB(0)/dμ = -θ²/(θ+μ)² · (θ/(θ+μ))^(θ-1)
        Dim derivative As Double =
            -theta * theta / ((theta + rate) * (theta + rate)) *
            std.Pow(theta / (theta + rate), theta - 1.0)

        ' ∂(-ln p0)/∂μ = -(1-π)/p0 · dNB0/dμ ，再乘 μ 得到对 ln μ 的梯度
        Return rate * (-(1.0 - pi) / numerator * derivative)

    End Function

#End Region

    Public Overrides Function ToString() As String
        Return $"CountLikelihood({Kind}, dispersion={Dispersion}, zeroInflation={ZeroInflation})"
    End Function

End Class
