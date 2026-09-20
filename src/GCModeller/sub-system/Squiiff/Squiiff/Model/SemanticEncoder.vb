Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.Squiiff.NN
Imports tfMath = Microsoft.VisualBasic.MachineLearning.TensorFlow.Math
Imports std = System.Math

Namespace Model

    ''' <summary>
    ''' 语义编码器 <c>Enc(x_0) → z_sem</c>（VAE 形式）。
    '''
    ''' 结构（对应 README 第四节"采用条件批归一化 + 残差连接结构的 MLP"）：
    ''' <code>
    ''' h    = Act(Linear_in(x_0))              ' geneCount → hidden
    ''' h    = EncoderResidualBlock₁(h[, cond]) ' 残差块，块内为（条件）批归一化
    ''' ...
    ''' h    = EncoderResidualBlockₙ(h[, cond])
    ''' μ     = Linear_mu(h)                    ' hidden → SemanticDim
    ''' logσ² = Linear_logVar(h)                ' hidden → SemanticDim
    ''' </code>
    '''
    ''' 训练时按 <c>z = μ + σ ⊙ ε</c> 重参数化采样，推理时取 <c>μ</c>（确定性），
    ''' 从而让 <c>Δz_sem = z_sem^pert − z_sem^ctrl</c> 的向量算术稳定可复现。
    '''
    ''' <c>z_sem</c> 携带细胞类型 / 状态 / 扰动等**高层语义**，作为条件去噪网络的显式条件
    ''' （经条件批归一化注入），决定生成的"内容"。
    '''
    ''' 关于 <see cref="SquiiffConfig.EncoderConditioned"/>：默认 False，即编码侧用普通批归一化。
    ''' 若编码侧也以扰动标签为条件，<c>Δz</c> 会出现标签泄漏、破坏隐空间向量算术的可解释性；
    ''' 该开关仅用于对比实验（置 True 后需通过 <see cref="Forward(Tensor, Boolean, Tensor)"/>
    ''' 传入 <c>[B, EncoderConditionDim]</c> 的条件向量）。
    ''' </summary>
    Public Class SemanticEncoder
        Implements IParameterized

        Private ReadOnly _config As SquiiffConfig
        Private ReadOnly _fcIn As Linear
        Private ReadOnly _blocks As EncoderResidualBlock()
        Private ReadOnly _muHead As Linear
        Private ReadOnly _logVarHead As Linear
        Private ReadOnly _params As Parameter()
        Private ReadOnly _rng As Random
        Private ReadOnly _activation As ActivationKind

        Private _preActivationIn As Tensor
        Private _bodyOutput As Tensor
        Private _mu As Tensor
        Private _logVar As Tensor
        Private _eps As Tensor

        Public Sub New(config As SquiiffConfig, geneCount As Integer, rng As Random)
            If config Is Nothing Then Throw New ArgumentNullException(NameOf(config))
            If rng Is Nothing Then Throw New ArgumentNullException(NameOf(rng))

            Me._config = config
            Me._rng = rng
            Me._activation = config.Activation

            Dim hidden = config.EncoderHiddenDim
            Dim conditionDim = If(config.EncoderConditioned, config.EncoderConditionDim, 0)

            Me._fcIn = New Linear("encoder.fcIn", geneCount, hidden)

            ReDim Me._blocks(config.EncoderBlocks - 1)
            For i As Integer = 0 To Me._blocks.Length - 1
                Me._blocks(i) = New EncoderResidualBlock(
                    $"encoder.block{i}", hidden, hidden,
                    activation:=config.Activation,
                    conditioned:=config.EncoderConditioned,
                    conditionDim:=conditionDim)
            Next

            Me._muHead = New Linear("encoder.mu", hidden, config.SemanticDim)
            Me._logVarHead = New Linear("encoder.logVar", hidden, config.SemanticDim)

            Dim items As New List(Of IParameterized) From {Me._fcIn}
            items.AddRange(Me._blocks)
            items.Add(Me._muHead)
            items.Add(Me._logVarHead)

            Me._params = ParameterGroups.FlattenMany(items)

            Call ApplyInferenceNormalizationMode()
        End Sub

        Public ReadOnly Property Parameters As IEnumerable(Of Parameter) Implements IParameterized.Parameters
            Get
                Return _params
            End Get
        End Property

        ''' <summary>残差块个数。</summary>
        Public ReadOnly Property BlockCount As Integer
            Get
                Return _blocks.Length
            End Get
        End Property

        ''' <summary>全部残差块内的归一化层（存档滑动统计量用，层名全局唯一）。</summary>
        Public ReadOnly Property Normalizations As IReadOnlyList(Of IRunningStatistics)
            Get
                Dim bag As New List(Of IRunningStatistics)
                For Each block In _blocks
                    bag.AddRange(block.Normalizations)
                Next
                Return bag
            End Get
        End Property

        ''' <summary>把 <see cref="SquiiffConfig.UseBatchStatsAtInference"/> 同步到全部残差块的归一化层。</summary>
        Public Sub ApplyInferenceNormalizationMode()
            For Each block In _blocks
                Call block.SetUseBatchStatsAtInference(_config.UseBatchStatsAtInference)
            Next
        End Sub

        ''' <summary>最近一次前向得到的 <c>μ</c>（<c>[B,dz]</c>）。</summary>
        Public ReadOnly Property LastMean As Tensor
            Get
                Return _mu
            End Get
        End Property

        ''' <summary>最近一次前向得到的 <c>log σ²</c>（<c>[B,dz]</c>）。</summary>
        Public ReadOnly Property LastLogVariance As Tensor
            Get
                Return _logVar
            End Get
        End Property

        ''' <summary>
        ''' 前向：返回语义隐变量 <c>z_sem</c>。
        ''' <paramref name="training"/> 为 False 时忽略重参数化，直接返回 <c>μ</c>。
        ''' </summary>
        Public Function Forward(x0 As Tensor, training As Boolean) As Tensor
            Return Forward(x0, training, Nothing)
        End Function

        ''' <summary>
        ''' 带条件向量的前向（<see cref="SquiiffConfig.EncoderConditioned"/> 为 True 时才有意义）。
        ''' </summary>
        ''' <param name="cond">条件向量 <c>[B, EncoderConditionDim]</c>；为 Nothing 时取全零（退化为非条件）。</param>
        Public Function Forward(x0 As Tensor, training As Boolean, cond As Tensor) As Tensor
            Me._preActivationIn = Me._fcIn.Forward(x0, training)
            Dim h = Activations.Forward(_activation, Me._preActivationIn)

            For Each block In _blocks
                h = block.Forward(h, cond, training)
            Next

            Me._bodyOutput = h
            Me._mu = Me._muHead.Forward(h, training)
            Me._logVar = Me._logVarHead.Forward(h, training)

            If _config.UseReparameterization AndAlso training Then
                Me._eps = TensorUtil.StandardNormal(Me._mu.Shape, _rng)
                Dim sigma = tfMath.exp(tfMath.multiply_scalar(Me._logVar, 0.5))
                Return Me._mu + sigma.ElementwiseMultiply(Me._eps)
            End If

            Me._eps = Nothing
            Return Me._mu
        End Function

        ''' <summary>
        ''' 最近一次前向的 KL 散度（按批量平均，逐样本为
        ''' <c>−0.5·Σ_j (1 + log σ²_j − μ_j² − σ²_j)</c>）。
        ''' </summary>
        Public Function KLDivergence() As Double
            Dim onePlusLogVar = tfMath.add_scalar(Me._logVar, 1.0)
            Dim muSquare = Me._mu.ElementwiseMultiply(Me._mu)
            Dim expLogVar = tfMath.exp(Me._logVar)

            Dim term = onePlusLogVar - muSquare - expLogVar
            Dim perSample = Tensor.computeKernel.Sum(term, 1, True)

            Return TensorUtil.MeanAll(TensorUtil.Scale(perSample, -0.5))
        End Function

        ''' <summary>
        ''' 反向传播：把 <paramref name="dZ"/>（来自去噪损失的 <c>dL/dz_sem</c>）
        ''' 与 KL 正则的梯度一起回传到主干。
        ''' </summary>
        ''' <param name="dZ">去噪网络回传的语义梯度。</param>
        ''' <param name="betaKL">KL 正则权重，与训练损失中的权重一致。</param>
        Public Sub Backward(dZ As Tensor, betaKL As Double)
            ' 1) 重参数化：z = μ + exp(0.5·log σ²) ⊙ ε
            Dim dMu = dZ
            Dim dLogVar As Tensor

            If Me._eps IsNot Nothing Then
                Dim sigma = tfMath.exp(tfMath.multiply_scalar(Me._logVar, 0.5))
                dLogVar = dZ.ElementwiseMultiply(Me._eps).ElementwiseMultiply(TensorUtil.Scale(sigma, 0.5))
            Else
                dLogVar = New Tensor(Me._logVar.Shape)
            End If

            ' 2) KL 正则：Loss 含 +β·KL，故
            '    ∂(β·KL)/∂μ      = β·μ
            '    ∂(β·KL)/∂log σ² = −β/2·(1 − σ²)
            If betaKL > 0.0 Then
                dMu = dMu + TensorUtil.Scale(Me._mu, betaKL)

                Dim klLogVar = TensorUtil.Scale(
                    tfMath.add_scalar(tfMath.negative(tfMath.exp(Me._logVar)), 1.0),
                    -0.5 * betaKL)
                dLogVar = dLogVar + klLogVar
            End If

            ' 3) 经两个线性头回到共享主干
            Dim d = Me._muHead.Backward(dMu) + Me._logVarHead.Backward(dLogVar)

            ' 4) 逆序穿过残差块
            For i As Integer = Me._blocks.Length - 1 To 0 Step -1
                Dim dCond As Tensor = Nothing
                d = Me._blocks(i).Backward(d, dCond)
                ' 编码侧的条件向量由调用方以元数据形式提供（不可训练），梯度到此终止
            Next

            d = Activations.Backward(_activation, Me._preActivationIn, d)
            Call Me._fcIn.Backward(d)
        End Sub
    End Class
End Namespace
