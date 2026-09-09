Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

''' <summary>
''' Adam 优化器（自适应矩估计）
''' </summary>
''' <remarks>
''' 论文使用 Adam 作为 P-NET 的优化器，并配合每 50 个 epoch 主动衰减的学习率策略。
''' Adam 对每个参数维护梯度的一阶矩估计与二阶矩估计，并做偏置校正，
''' 对 P-NET 这种大量梯度恒为 0 的稀疏梯度场景尤其稳健。
'''
''' 参数更新规则为：
'''
''' <code>
''' m_t = beta1 · m_(t-1) + (1 - beta1) · g_t
''' v_t = beta2 · v_(t-1) + (1 - beta2) · g_t²
''' m̂_t = m_t / (1 - beta1^t),  v̂_t = v_t / (1 - beta2^t)
''' θ_t = θ_(t-1) - lr · m̂_t / (sqrt(v̂_t) + eps)
''' </code>
'''
''' 由于被掩码屏蔽的连接其梯度恒为 0，对应位置的 <c>m</c> 与 <c>v</c> 也恒为 0，
''' 因此参数更新量恒为 0，即这些权重永远不会被更新。
''' </remarks>
Public Class AdamOptimizer

    ''' <summary>
    ''' 学习率，可以在训练过程中被外部修改以实现学习率衰减
    ''' </summary>
    ''' <returns>当前学习率</returns>
    Public Property LearningRate As Double

    ''' <summary>
    ''' 一阶矩估计的指数衰减率
    ''' </summary>
    ''' <returns>beta1，默认 0.9</returns>
    Public Property Beta1 As Double

    ''' <summary>
    ''' 二阶矩估计的指数衰减率
    ''' </summary>
    ''' <returns>beta2，默认 0.999</returns>
    Public Property Beta2 As Double

    ''' <summary>
    ''' 数值稳定性常数
    ''' </summary>
    ''' <returns>epsilon，默认 1e-8</returns>
    Public Property Epsilon As Double

    ''' <summary>
    ''' 当前已经执行的更新步数
    ''' </summary>
    ''' <returns>时间步 t</returns>
    Public ReadOnly Property StepCount As Integer
        Get
            Return _t
        End Get
    End Property

    Private ReadOnly _parameters As List(Of Tensor)
    Private ReadOnly _gradients As List(Of Tensor)
    Private ReadOnly _m As List(Of Tensor)
    Private ReadOnly _v As List(Of Tensor)
    Private _t As Integer = 0

    ''' <summary>
    ''' 创建 Adam 优化器
    ''' </summary>
    ''' <param name="parameters">待优化的参数列表</param>
    ''' <param name="gradients">与 <paramref name="parameters"/> 一一对应的梯度列表</param>
    ''' <param name="learningRate">初始学习率，论文取 0.001</param>
    ''' <param name="beta1">一阶矩衰减率</param>
    ''' <param name="beta2">二阶矩衰减率</param>
    ''' <param name="epsilon">数值稳定性常数</param>
    Public Sub New(parameters As List(Of Tensor), gradients As List(Of Tensor),
                   Optional learningRate As Double = 0.001,
                   Optional beta1 As Double = 0.9,
                   Optional beta2 As Double = 0.999,
                   Optional epsilon As Double = 0.00000001)

        If parameters Is Nothing OrElse gradients Is Nothing Then
            Throw New ArgumentNullException("参数列表与梯度列表均不能为空")
        End If
        If parameters.Count <> gradients.Count Then
            Throw New ArgumentException("参数列表与梯度列表的长度必须一致")
        End If

        _parameters = parameters
        _gradients = gradients
        _m = New List(Of Tensor)()
        _v = New List(Of Tensor)()

        For Each p As Tensor In parameters
            _m.Add(New Tensor(p.Shape))
            _v.Add(New Tensor(p.Shape))
        Next

        Me.LearningRate = learningRate
        Me.Beta1 = beta1
        Me.Beta2 = beta2
        Me.Epsilon = epsilon
        Me._t = 0
    End Sub

    ''' <summary>
    ''' 执行一次参数更新
    ''' </summary>
    Public Sub [Step]()
        _t += 1

        Dim biasCorrection1 As Double = 1.0 - std.Pow(Beta1, _t)
        Dim biasCorrection2 As Double = 1.0 - std.Pow(Beta2, _t)

        For i As Integer = 0 To _parameters.Count - 1
            Dim paramData As Double() = _parameters(i).Data
            Dim gradData As Double() = _gradients(i).Data
            Dim mData As Double() = _m(i).Data
            Dim vData As Double() = _v(i).Data

            For j As Integer = 0 To paramData.Length - 1
                Dim g As Double = gradData(j)

                mData(j) = Beta1 * mData(j) + (1.0 - Beta1) * g
                vData(j) = Beta2 * vData(j) + (1.0 - Beta2) * g * g

                Dim mHat As Double = mData(j) / biasCorrection1
                Dim vHat As Double = vData(j) / biasCorrection2

                paramData(j) -= LearningRate * mHat / (std.Sqrt(vHat) + Epsilon)
            Next
        Next
    End Sub

    ''' <summary>
    ''' 清零全部梯度
    ''' </summary>
    Public Sub ZeroGrad()
        For Each g As Tensor In _gradients
            Array.Clear(g.Data, 0, g.Length)
        Next
    End Sub

    ''' <summary>
    ''' 生成优化器的字符串描述
    ''' </summary>
    ''' <returns>形如 ``Adam[lr=0.001, step=120]`` 的描述文本</returns>
    Public Overrides Function ToString() As String
        Return $"Adam[lr={LearningRate.ToString("F6")}, step={_t}]"
    End Function

End Class
