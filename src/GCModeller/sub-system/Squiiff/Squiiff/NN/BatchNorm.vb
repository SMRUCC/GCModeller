Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports tfMath = Microsoft.VisualBasic.MachineLearning.TensorFlow.Math
Imports std = System.Math

Namespace NN

    ''' <summary>
    ''' 批归一化（<c>y = γ ⊙ x̂ + β</c>，逐特征可学习的 γ / β）。
    '''
    ''' 训练模式使用当前批统计量并更新滑动统计量；推理模式默认使用滑动统计量，
    ''' 从而让"生成"阶段的每个细胞输出与批组成无关、可复现。
    ''' 可通过 <see cref="UseBatchStatsAtInference"/> 关闭该行为（生成时也用批统计量），
    ''' 这与多数条件 BN 生成模型的实践一致。
    '''
    ''' 反向公式（对输入的梯度，含对批统计量的依赖项）：
    ''' <code>
    ''' dx = invStd ⊙ ( dx̂ − mean(dx̂) − x̂ ⊙ mean(dx̂ ⊙ x̂) )
    ''' </code>
    ''' </summary>
    Public Class BatchNorm
        Inherits LayerModule

        Private ReadOnly _name As String
        Private ReadOnly _params As Parameter()
        Private ReadOnly _runningMean As Tensor
        Private ReadOnly _runningVar As Tensor

        Private _xhat As Tensor
        Private _invStd As Tensor
        Private _batch As Integer
        Private _inference As Boolean

        ''' <summary>缩放参数 γ，形状 <c>[1,H]</c>，初始化为 1。</summary>
        Public ReadOnly Property Gamma As Parameter

        ''' <summary>偏移参数 β，形状 <c>[1,H]</c>，初始化为 0。</summary>
        Public ReadOnly Property Beta As Parameter

        ''' <summary>滑动统计量的动量。</summary>
        Public Property Momentum As Double = 0.1

        ''' <summary>数值稳定项。</summary>
        Public Property Epsilon As Double = 0.00001

        ''' <summary>
        ''' 推理模式是否仍使用当前批统计量。
        ''' False（默认）= 标准批归一化语义，推理用滑动统计量；
        ''' True = 生成时也用批统计量（条件 BN 生成模型的常见做法）。
        ''' </summary>
        Public Property UseBatchStatsAtInference As Boolean = False

        Public Sub New(name As String, features As Integer)
            Me._name = name
            Me.Gamma = New Parameter($"{name}.gamma", TensorUtil.RowConstant(1.0, features))
            Me.Beta = New Parameter($"{name}.beta", TensorUtil.RowConstant(0.0, features))
            Me._runningMean = TensorUtil.RowConstant(0.0, features)
            ' 滑动方差初始化为 1，保证初始 invStd 处于合理量级
            Me._runningVar = TensorUtil.RowConstant(1.0, features)
            Me._params = {Me.Gamma, Me.Beta}
        End Sub

        Public Overrides ReadOnly Property Name As String
            Get
                Return _name
            End Get
        End Property

        Public Overrides ReadOnly Property Parameters As IEnumerable(Of Parameter)
            Get
                Return _params
            End Get
        End Property

        ''' <summary>滑动均值（<c>[1,H]</c>，只读诊断用）。</summary>
        Public ReadOnly Property RunningMean As Tensor
            Get
                Return _runningMean
            End Get
        End Property

        ''' <summary>滑动方差（<c>[1,H]</c>，只读诊断用）。</summary>
        Public ReadOnly Property RunningVariance As Tensor
            Get
                Return _runningVar
            End Get
        End Property

        Public Overrides Function Forward(x As Tensor, training As Boolean) As Tensor
            Dim B = x.Shape(0)
            Dim useBatchStats = training OrElse Not Me.UseBatchStatsAtInference

            Me._batch = B
            Me._inference = Not useBatchStats

            Dim mean As Tensor
            Dim variance As Tensor
            Dim xmu As Tensor

            If useBatchStats Then
                ' μ = mean(x) ；σ² = mean((x-μ)²)
                mean = TensorUtil.Scale(x.Sum(axis:=0), 1.0 / B)
                xmu = TensorUtil.BroadcastSubtract(x, mean)
                variance = TensorUtil.Scale(xmu.ElementwiseMultiply(xmu).Sum(axis:=0), 1.0 / B)

                If training Then Call UpdateRunningStatistics(mean, variance, B)
            Else
                mean = _runningMean
                variance = _runningVar
                xmu = TensorUtil.BroadcastSubtract(x, mean)
            End If

            ' invStd = (σ² + ε)^(-1/2)
            Me._invStd = tfMath.pow(tfMath.add_scalar(variance, Me.Epsilon), -0.5)
            Me._xhat = xmu.ElementwiseMultiply(TensorUtil.BroadcastRow(Me._invStd, B))

            Dim g = TensorUtil.BroadcastRow(Me.Gamma.Value, B)
            Dim b = TensorUtil.BroadcastRow(Me.Beta.Value, B)

            Return Me._xhat.ElementwiseMultiply(g) + b
        End Function

        Public Overrides Function Backward(dOut As Tensor) As Tensor
            Dim B = Me._batch
            Dim g = TensorUtil.BroadcastRow(Me.Gamma.Value, B)

            ' dγ = Σ_batch (dOut ⊙ x̂) ；dβ = Σ_batch dOut
            TensorUtil.Accumulate(Me.Gamma.Gradient, dOut.ElementwiseMultiply(_xhat).Sum(axis:=0))
            TensorUtil.Accumulate(Me.Beta.Gradient, dOut.Sum(axis:=0))

            Dim dxhat = dOut.ElementwiseMultiply(g)

            If Me._inference Then
                ' 推理模式：μ / σ² 视为常量，dx = dx̂ ⊙ invStd
                Return dxhat.ElementwiseMultiply(TensorUtil.BroadcastRow(Me._invStd, B))
            End If

            Dim m1 = TensorUtil.Scale(dxhat.Sum(axis:=0), 1.0 / B)
            Dim m2 = TensorUtil.Scale(dxhat.ElementwiseMultiply(_xhat).Sum(axis:=0), 1.0 / B)

            Dim inner = dxhat -
                TensorUtil.BroadcastRow(m1, B) -
                _xhat.ElementwiseMultiply(TensorUtil.BroadcastRow(m2, B))

            Return inner.ElementwiseMultiply(TensorUtil.BroadcastRow(Me._invStd, B))
        End Function

        ''' <summary>用当前批统计量更新滑动均值 / 无偏滑动方差。</summary>
        Private Sub UpdateRunningStatistics(mean As Tensor, variance As Tensor, B As Integer)
            Dim unbiased = TensorUtil.Scale(variance, If(B > 1, CDbl(B) / (B - 1), 1.0))

            Dim m = _runningMean.Data
            Dim md = mean.Data
            Dim v = _runningVar.Data
            Dim vd = unbiased.Data
            Dim momentum = Me.Momentum

            For i As Integer = 0 To m.Length - 1
                m(i) = (1.0 - momentum) * m(i) + momentum * md(i)
                v(i) = (1.0 - momentum) * v(i) + momentum * vd(i)
            Next

            Call _runningMean.MarkHostModified()
            Call _runningVar.MarkHostModified()
        End Sub
    End Class
End Namespace
