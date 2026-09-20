Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.Squiiff.NN
Imports std = System.Math

Namespace Diffusion

    ''' <summary>
    ''' 扩散噪声调度（前向加噪过程）。
    '''
    ''' <code>
    ''' q(x_t | x_{t-1}) = N( x_t; √(1−β_t)·x_{t-1}, β_t·I )
    ''' </code>
    ''' 利用重参数化技巧，任意时刻可一步从 <c>x_0</c> 采样：
    ''' <code>
    ''' x_t = √ᾱ_t · x_0 + √(1−ᾱ_t) · ε,     ε ~ N(0, I)
    ''' ᾱ_t = Π_{s=1..t} (1 − β_s)
    ''' </code>
    ''' <c>β_t</c> 在 <c>[betaStart, betaEnd]</c> 上线性取值（SquiDiff 取 [0.001, 0.01]）。
    ''' 索引约定：<c>AlphaBar(0) = 1</c>，时间步 <c>t</c> 取值 <c>1..T</c>（<c>t=0</c> 表示干净数据）。
    ''' </summary>
    Public NotInheritable Class DiffusionSchedule

        Private ReadOnly _beta As Double()
        Private ReadOnly _alphaBar As Double()
        Private ReadOnly _alphaBarFloor As Double

        ''' <summary>
        ''' </summary>
        ''' <param name="timesteps">扩散总步数 T。</param>
        ''' <param name="betaStart">β 下限（t=1 处）。</param>
        ''' <param name="betaEnd">β 上限（t=T 处）。</param>
        ''' <param name="alphaBarFloor">
        ''' <c>ᾱ</c> 的下限裁剪。当 <c>ᾱ_t</c> 极小时，由 x_t 反推 <c>x̂_0</c> 的除法会放大误差，
        ''' 因此对分母做下限保护（文档亦强调这一点）。
        ''' </param>
        Public Sub New(timesteps As Integer,
                       Optional betaStart As Double = 0.001,
                       Optional betaEnd As Double = 0.01,
                       Optional alphaBarFloor As Double = 0.00001)
            If timesteps < 1 Then Throw New ArgumentOutOfRangeException(NameOf(timesteps), "扩散步数必须 >= 1")

            Me.T = timesteps
            Me._alphaBarFloor = alphaBarFloor

            ReDim Me._beta(timesteps)
            ReDim Me._alphaBar(timesteps)

            Me._alphaBar(0) = 1.0

            If timesteps = 1 Then
                Me._beta(1) = betaStart
            Else
                For t As Integer = 1 To timesteps
                    ' t=1 对应 betaStart，t=T 对应 betaEnd，中间线性插值
                    Me._beta(t) = betaStart + (betaEnd - betaStart) * (t - 1) / (timesteps - 1)
                Next
            End If

            For t As Integer = 1 To timesteps
                Me._alphaBar(t) = Me._alphaBar(t - 1) * (1.0 - Me._beta(t))
            Next
        End Sub

        ''' <summary>扩散总步数 T。</summary>
        Public ReadOnly Property T As Integer

        ''' <summary><c>β</c> 序列（索引 1..T）。</summary>
        Public ReadOnly Property Beta As Double()
            Get
                Return _beta
            End Get
        End Property

        ''' <summary>累积乘积 <c>ᾱ</c>（索引 0..T，<c>ᾱ_0 = 1</c>）。</summary>
        Public ReadOnly Property AlphaBar As Double()
            Get
                Return _alphaBar
            End Get
        End Property

        ''' <summary><c>√ᾱ_t</c>。</summary>
        Public Function SqrtAlphaBar(t As Integer) As Double
            Return std.Sqrt(std.Max(_alphaBar(t), Me._alphaBarFloor))
        End Function

        ''' <summary><c>√(1 − ᾱ_t)</c>。</summary>
        Public Function SqrtOneMinusAlphaBar(t As Integer) As Double
            Return std.Sqrt(std.Max(1.0 - _alphaBar(t), 0.0))
        End Function

        ''' <summary>按信号保留比例报告调度摘要（诊断用）。</summary>
        Public Function Describe() As String
            Return $"T={T}, β∈[{_beta(1):F4}, {_beta(T):F4}], ᾱ_T={_alphaBar(T):E3}, √ᾱ_T={SqrtAlphaBar(T):F4}"
        End Function

        ''' <summary>批量采样互独立的时间步 <c>t ∈ [1, T]</c>。</summary>
        Public Function SampleTimesteps(batchSize As Integer, rng As Random) As Integer()
            Dim result(batchSize - 1) As Integer
            For i As Integer = 0 To batchSize - 1
                result(i) = rng.Next(1, Me.T + 1)
            Next
            Return result
        End Function

        ''' <summary>
        ''' 前向加噪：<c>x_t = √ᾱ_t · x_0 + √(1−ᾱ_t) · ε</c>，<paramref name="t"/> 为逐样本时间步。
        ''' </summary>
        Public Function AddNoise(x0 As Tensor, t As Integer(), eps As Tensor) As Tensor
            Return TensorUtil.RowScale(x0, ColumnOfVector(t, AddressOf SqrtAlphaBar)) +
                   TensorUtil.RowScale(eps, ColumnOfVector(t, AddressOf SqrtOneMinusAlphaBar))
        End Function

        ''' <summary>
        ''' 由当前噪声估计反推"干净数据" <c>x̂_0 = (x_t − √(1−ᾱ_t)·ε̂) / √ᾱ_t</c>。
        ''' </summary>
        Public Function PredictX0(xt As Tensor, epsHat As Tensor, t As Integer()) As Tensor
            Dim denom(t.Length - 1) As Double
            For i As Integer = 0 To t.Length - 1
                denom(i) = 1.0 / SqrtAlphaBar(t(i))
            Next

            Dim residual = xt - TensorUtil.RowScale(epsHat, ColumnOfVector(t, AddressOf SqrtOneMinusAlphaBar))
            Return TensorUtil.RowScale(residual, TensorUtil.ColumnVector(denom))
        End Function

        ''' <summary>
        ''' 重组合：用 <c>x̂_0</c> 与噪声 <c>ε</c> 重建时刻 <paramref name="t"/> 的样本
        ''' <c>√ᾱ_t·x̂_0 + √(1−ᾱ_t)·ε</c>（DDIM 确定性轨迹的单步更新）。
        ''' 此处批量共享同一步 <paramref name="t"/>，可用标量算子直接逐元素缩放。
        ''' </summary>
        Public Function Recombine(x0Hat As Tensor, eps As Tensor, t As Integer) As Tensor
            Dim sa = CSng(SqrtAlphaBar(t))
            Dim sb = CSng(SqrtOneMinusAlphaBar(t))
            Return x0Hat * sa + eps * sb
        End Function

        ''' <summary>把逐样本时间步映射为 <c>[B,1]</c> 的系数列向量。</summary>
        Private Shared Function ColumnOfVector(t As Integer(), value As Func(Of Integer, Double)) As Tensor
            Dim values(t.Length - 1) As Double
            For i As Integer = 0 To t.Length - 1
                values(i) = value(t(i))
            Next
            Return TensorUtil.ColumnVector(values)
        End Function
    End Class
End Namespace
