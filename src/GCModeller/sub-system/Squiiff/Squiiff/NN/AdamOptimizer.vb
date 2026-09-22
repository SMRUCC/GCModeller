Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

Namespace NN

    ''' <summary>
    ''' Adam / AdamW 优化器：自行维护一阶矩（动量）与二阶矩（自适应步长），
    ''' 支持解耦权重衰减与**全局 L2 梯度裁剪**。
    '''
    ''' 实现策略：
    ''' <list type="number">
    ''' <item>若 <see cref="UseDeviceKernel"/> 为 True，先尝试后端的设备端快路径
    '''       <c>ITensorCompute.TryAdamWStep</c>（CUDA 后端在参数已常驻显存时返回 True）；
    '''       默认关闭，因为该快路径要求参数被 <c>PinDevice</c> 常驻，本模型不启用显存常驻。</item>
    ''' <item>否则在主机侧用标量循环原地更新，与 DeepLearning 中
    '''       <c>Transformer.Optimizer.MakeTrainingStep</c> 的公式完全一致
    '''       （<c>p -= lr · m̂ / (√v̂ + ε)</c>）。Adam 是逐元素的，其开销相对前/反向的矩阵乘可忽略，
    '''       因此主机循环不会成为瓶颈。</item>
    ''' </list>
    ''' </summary>
    Public Class AdamOptimizer

        Private ReadOnly _params As Parameter()
        Private ReadOnly _m As Tensor()
        Private ReadOnly _v As Tensor()
        Private _step As Integer

        Public Property LearningRate As Double = 0.001
        Public Property Beta1 As Double = 0.9
        Public Property Beta2 As Double = 0.999
        Public Property Epsilon As Double = 0.00000001

        ''' <summary>解耦权重衰减系数（AdamW 风格）。</summary>
        Public Property WeightDecay As Double = 0.0

        ''' <summary>全局 L2 梯度裁剪阈值；&lt;= 0 表示禁用裁剪。</summary>
        Public Property ClipNorm As Double = 1.0

        ''' <summary>是否尝试设备端 AdamW 快路径（默认关闭，见类型注释）。</summary>
        Public Property UseDeviceKernel As Boolean = False

        Public Sub New(parameters As IEnumerable(Of Parameter))
            If parameters Is Nothing Then Throw New ArgumentNullException(NameOf(parameters))

            Me._params = parameters.ToArray()
            ReDim Me._m(Me._params.Length - 1)
            ReDim Me._v(Me._params.Length - 1)

            For i As Integer = 0 To Me._params.Length - 1
                Me._m(i) = New Tensor(Me._params(i).Value.Shape)
                Me._v(i) = New Tensor(Me._params(i).Value.Shape)
            Next
        End Sub

        ''' <summary>已执行的更新步数（用于 Adam 偏差校正，从 1 开始）。</summary>
        Public ReadOnly Property StepCount As Integer
            Get
                Return _step
            End Get
        End Property

        ''' <summary>被优化的参数个数。</summary>
        Public ReadOnly Property ParameterCount As Integer
            Get
                Return _params.Length
            End Get
        End Property

        ''' <summary>被优化的参数元素总数（诊断用）。</summary>
        Public ReadOnly Property ParameterSize As Integer
            Get
                Dim n As Integer = 0
                For Each p In _params
                    n += p.Size
                Next
                Return n
            End Get
        End Property

        ''' <summary>清空全部参数的梯度槽。</summary>
        Public Sub ZeroGrad()
            For Each p In _params
                Call p.ZeroGrad()
            Next
        End Sub

        ''' <summary>全部参数梯度的全局 L2 范数。</summary>
        Public Function GradientNorm() As Double
            Dim sum As Double = 0.0
            For Each p In _params
                Dim g = p.Gradient.Data
                For i As Integer = 0 To g.Length - 1
                    sum += g(i) * g(i)
                Next
            Next
            Return std.Sqrt(sum)
        End Function

        ''' <summary>按 <see cref="ClipNorm"/> 对全部参数梯度做原地等比缩放（若启用）。</summary>
        ''' <returns>裁剪前的全局梯度范数。</returns>
        Public Function ClipGradients() As Double
            Dim norm = GradientNorm()
            If Me.ClipNorm <= 0.0 OrElse norm <= Me.ClipNorm OrElse norm <= 0.0 Then Return norm

            Dim scaleFactor = Me.ClipNorm / norm
            For Each p In _params
                Dim grad = p.Gradient
                Dim g = grad.Data
                For i As Integer = 0 To g.Length - 1
                    g(i) *= scaleFactor
                Next
                Call grad.MarkHostModified()
            Next

            Return norm
        End Function

        ''' <summary>执行一次参数更新（消费并清零梯度）。</summary>
        Public Sub [Step]()
            _step += 1

            Dim bc1 = 1.0 - std.Pow(Me.Beta1, _step)
            Dim bc2 = 1.0 - std.Pow(Me.Beta2, _step)
            Dim lr = Me.LearningRate
            Dim b1 = Me.Beta1
            Dim b2 = Me.Beta2
            Dim eps = Me.Epsilon
            Dim wd = Me.WeightDecay

            For i As Integer = 0 To _params.Length - 1
                Dim p = _params(i)
                Dim value = p.Value
                Dim grad = p.Gradient
                Dim m = _m(i)
                Dim v = _v(i)

                If Me.UseDeviceKernel AndAlso
                   Tensor.computeKernel.TryAdamWStep(value, grad, m, v, lr, b1, b2, eps, bc1, bc2, wd) Then
                    Call p.ZeroGrad()
                    Continue For
                End If

                Dim pd = value.Data
                Dim gd = grad.Data
                Dim md = m.Data
                Dim vd = v.Data

                For k As Integer = 0 To pd.Length - 1
                    Dim g = gd(k)

                    md(k) = b1 * md(k) + (1.0 - b1) * g
                    vd(k) = b2 * vd(k) + (1.0 - b2) * g * g

                    Dim mHat = md(k) / bc1
                    Dim vHat = vd(k) / bc2
                    Dim update = mHat / (std.Sqrt(vHat) + eps)

                    If wd > 0.0 Then update += wd * pd(k)

                    pd(k) -= lr * update
                Next

                Call value.MarkHostModified()
                Call m.MarkHostModified()
                Call v.MarkHostModified()
                Call p.ZeroGrad()
            Next
        End Sub
    End Class
End Namespace
