Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports tfMath = Microsoft.VisualBasic.MachineLearning.TensorFlow.Math
Imports std = System.Math

Namespace NN

    ''' <summary>
    ''' 条件批归一化（Conditional Batch Normalization）。
    '''
    ''' 与普通 <see cref="BatchNorm"/> 的唯一区别：缩放 γ 与偏移 β **不是逐特征的可学习向量，
    ''' 而是由条件向量逐样本线性投影得到**：
    ''' <code>
    ''' γ = cond · Wγ + bγ      ' [B,H]，逐样本
    ''' β = cond · Wβ + bβ      ' [B,H]，逐样本
    ''' </code>
    ''' 条件向量在本模型中为 <c>concat(z_sem, timeEmbedding)</c>，因此语义隐变量 <c>z_sem</c>
    ''' 是**显式条件输入**（而非 classifier-free guidance 的分数混合），
    ''' 通过条件批归一化注入去噪网络。
    '''
    ''' ### 初始化策略（关键）
    ''' β 投影零初始化（<c>Wβ=0, bβ=0</c>），γ 投影的偏置取 1（<c>bγ=1</c>），二者合起来让条件批归一化
    ''' 在训练起点退化为"标准化 + γ=1, β=0"，保证训练稳定。
    '''
    ''' 但 γ 投影的**权重不能零初始化**：<see cref="Linear.Backward"/> 对输入的梯度是
    ''' <c>dOut · Wᵀ</c>，一旦 <c>Wγ = 0</c>，条件向量 <c>[z_sem, tEmb]</c> 的梯度就恒为 0，
    ''' 也就是说**语义编码器在整个训练初期收不到任何来自去噪损失的梯度**（实测条件敏感度
    ''' <c>‖Δε̂‖/‖ε̂‖ ≈ 0.02</c>，语义条件事实上被去噪网络忽略）。
    ''' 因此 γ 权重改用**小尺度随机初始化**（<see cref="DefaultGammaInitScale"/>），
    ''' 让条件支路从起点就是"活的"，既保留 γ≈1 的近似恒等性，又打通了回传到编码器的梯度通路。
    '''
    ''' 反向传播需要同时回传两个方向：
    ''' 对输入的梯度 <c>dx</c>，以及对条件向量的梯度 <c>dCond</c>（经 <c>ByRef</c> 输出）。
    ''' </summary>
    Public Class ConditionalBatchNorm
        Implements IParameterized, IRunningStatistics

        Private ReadOnly _name As String
        Private ReadOnly _params As Parameter()
        Private ReadOnly _runningMean As Tensor
        Private ReadOnly _runningVar As Tensor

        Private _xhat As Tensor
        Private _invStd As Tensor
        Private _gamma As Tensor
        Private _batch As Integer
        Private _inference As Boolean

        ''' <summary>γ 的条件投影：<c>cond[D] -> [B,H]</c>。</summary>
        Public ReadOnly Property GammaProjection As Linear

        ''' <summary>β 的条件投影：<c>cond[D] -> [B,H]</c>。</summary>
        Public ReadOnly Property BetaProjection As Linear

        Public Property Momentum As Double = 0.1
        Public Property Epsilon As Double = 0.00001

        ''' <summary>推理模式是否仍使用当前批统计量（默认 False = 使用滑动统计量）。</summary>
        Public Property UseBatchStatsAtInference As Boolean = False

        ''' <summary>γ 投影权重的初始化尺度（<c>γ</c> 偏离 1 的标准差量级）。</summary>
        Public Const DefaultGammaInitScale As Double = 0.1

        Public Sub New(name As String, features As Integer, conditionDim As Integer,
                       Optional gammaInitScale As Double = DefaultGammaInitScale)
            Me._name = name
            Me.GammaProjection = New Linear($"{name}.gammaProj", conditionDim, features)
            Me.BetaProjection = New Linear($"{name}.betaProj", conditionDim, features)

            ' γ 支路：偏置取 1（恒等起点），权重取小尺度随机（保证条件梯度通路不死）
            Call RescaleWeight(Me.GammaProjection.Weight.Value, gammaInitScale, conditionDim)
            SetConstant(Me.GammaProjection.Bias.Value, 1.0)

            ' β 支路：全零（恒等起点）
            ZeroWeight(Me.BetaProjection.Weight.Value)
            SetConstant(Me.BetaProjection.Bias.Value, 0.0)

            Me._runningMean = TensorUtil.RowConstant(0.0, features)
            Me._runningVar = TensorUtil.RowConstant(1.0, features)
            Me._params = ParameterGroups.Flatten(Me.GammaProjection, Me.BetaProjection)
        End Sub

        Private Shared Sub ZeroWeight(t As Tensor)
            Array.Clear(t.Data, 0, t.Data.Length)
            Call t.MarkHostModified()
        End Sub

        ''' <summary>
        ''' 把 He 初始化的权重重新缩放为 <c>N(0, (scale/√conditionDim)²)</c>。
        ''' 这样 <c>γ = 1 + cond·Wγ</c> 在条件向量各分量近似标准正态时，偏离 1 的幅度约为
        ''' <paramref name="scale"/>，既保持"近似恒等"又让 <c>dOut·Wᵀ</c> 从第一步起就非零。
        ''' </summary>
        Private Shared Sub RescaleWeight(t As Tensor, scale As Double, conditionDim As Integer)
            Dim factor = scale / (std.Sqrt(2.0) * std.Sqrt(std.Max(1, conditionDim)))
            Dim rescaled = TensorUtil.Scale(TensorUtil.HeNormal(t.Shape), factor)

            Call Array.Copy(rescaled.Data, t.Data, t.Data.Length)
            Call t.MarkHostModified()
        End Sub

        Private Shared Sub SetConstant(t As Tensor, value As Double)
            Dim data = t.Data
            For i As Integer = 0 To data.Length - 1
                data(i) = value
            Next
            Call t.MarkHostModified()
        End Sub

        Public ReadOnly Property Name As String Implements IRunningStatistics.Name
            Get
                Return _name
            End Get
        End Property

        Public ReadOnly Property Parameters As IEnumerable(Of Parameter) Implements IParameterized.Parameters
            Get
                Return _params
            End Get
        End Property

        ''' <summary>滑动均值（<c>[1,H]</c>）。</summary>
        Public ReadOnly Property RunningMean As Tensor Implements IRunningStatistics.RunningMean
            Get
                Return _runningMean
            End Get
        End Property

        ''' <summary>滑动方差（<c>[1,H]</c>）。</summary>
        Public ReadOnly Property RunningVariance As Tensor Implements IRunningStatistics.RunningVariance
            Get
                Return _runningVar
            End Get
        End Property

        ''' <summary>
        ''' 前向：<paramref name="x"/> 为 <c>[B,H]</c>，<paramref name="cond"/> 为 <c>[B,conditionDim]</c>。
        ''' </summary>
        Public Function Forward(x As Tensor, cond As Tensor, training As Boolean) As Tensor
            Dim B = x.Shape(0)
            Dim useBatchStats = training OrElse Not Me.UseBatchStatsAtInference

            Me._batch = B
            Me._inference = Not useBatchStats

            Dim mean As Tensor
            Dim variance As Tensor
            Dim xmu As Tensor

            If useBatchStats Then
                mean = TensorUtil.Scale(x.Sum(axis:=0), 1.0 / B)
                xmu = TensorUtil.BroadcastSubtract(x, mean)
                variance = TensorUtil.Scale(xmu.ElementwiseMultiply(xmu).Sum(axis:=0), 1.0 / B)

                If training Then Call UpdateRunningStatistics(mean, variance, B)
            Else
                mean = _runningMean
                variance = _runningVar
                xmu = TensorUtil.BroadcastSubtract(x, mean)
            End If

            Me._invStd = tfMath.pow(tfMath.add_scalar(variance, Me.Epsilon), -0.5)
            Me._xhat = xmu.ElementwiseMultiply(TensorUtil.BroadcastRow(Me._invStd, B))

            Me._gamma = Me.GammaProjection.Forward(cond, training)      ' [B,H]
            Dim beta = Me.BetaProjection.Forward(cond, training)        ' [B,H]

            Return Me._xhat.ElementwiseMultiply(_gamma) + beta
        End Function

        ''' <summary>
        ''' 反向：返回对 <c>x</c> 的梯度，并把对条件向量的梯度写到 <paramref name="dCond"/>。
        ''' </summary>
        Public Function Backward(dOut As Tensor, ByRef dCond As Tensor) As Tensor
            Dim B = Me._batch

            ' 对 γ / β 的梯度（逐样本）分别回传到条件投影
            Dim dGamma = dOut.ElementwiseMultiply(_xhat)
            Dim dBeta = dOut

            dCond = Me.GammaProjection.Backward(dGamma) + Me.BetaProjection.Backward(dBeta)

            Dim dxhat = dOut.ElementwiseMultiply(_gamma)

            If Me._inference Then
                Return dxhat.ElementwiseMultiply(TensorUtil.BroadcastRow(Me._invStd, B))
            End If

            Dim m1 = TensorUtil.Scale(dxhat.Sum(axis:=0), 1.0 / B)
            Dim m2 = TensorUtil.Scale(dxhat.ElementwiseMultiply(_xhat).Sum(axis:=0), 1.0 / B)

            Dim inner = dxhat -
                TensorUtil.BroadcastRow(m1, B) -
                _xhat.ElementwiseMultiply(TensorUtil.BroadcastRow(m2, B))

            Return inner.ElementwiseMultiply(TensorUtil.BroadcastRow(Me._invStd, B))
        End Function

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
