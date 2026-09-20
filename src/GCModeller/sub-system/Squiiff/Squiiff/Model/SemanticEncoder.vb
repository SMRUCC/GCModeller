Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.Squiiff.NN
Imports tfMath = Microsoft.VisualBasic.MachineLearning.TensorFlow.Math
Imports std = System.Math

Namespace Model

    ''' <summary>
    ''' 语义编码器 <c>Enc(x_0) → z_sem</c>（VAE 形式）。
    '''
    ''' 结构：<c>Linear → Act → [BatchNorm → Act → Linear] × blocks</c> 组成的共享主干，
    ''' 末端分出两个线性头分别预测 <c>μ</c> 与 <c>log σ²</c>。
    ''' 训练时按 <c>z = μ + σ ⊙ ε</c> 重参数化采样，推理时取 <c>μ</c>（确定性），
    ''' 从而让 <c>Δz_sem = z_sem^pert − z_sem^ctrl</c> 的向量算术稳定可复现。
    '''
    ''' <c>z_sem</c> 携带细胞类型 / 状态 / 扰动等**高层语义**，作为条件去噪网络的显式条件
    ''' （经条件批归一化注入），决定生成的"内容"。
    ''' </summary>
    Public Class SemanticEncoder
        Implements IParameterized

        Private ReadOnly _config As SquiiffConfig
        Private ReadOnly _body As Sequential
        Private ReadOnly _muHead As Linear
        Private ReadOnly _logVarHead As Linear
        Private ReadOnly _params As Parameter()
        Private ReadOnly _rng As Random

        Private _bodyOutput As Tensor
        Private _mu As Tensor
        Private _logVar As Tensor
        Private _eps As Tensor

        Public Sub New(config As SquiiffConfig, geneCount As Integer, rng As Random)
            If config Is Nothing Then Throw New ArgumentNullException(NameOf(config))
            If rng Is Nothing Then Throw New ArgumentNullException(NameOf(rng))

            Me._config = config
            Me._rng = rng

            Dim hidden = config.EncoderHiddenDim

            Me._body = New Sequential("encoder.body")
            Call Me._body.Add(New Linear("encoder.fcIn", geneCount, hidden))
            Call Me._body.Add(New ActivationLayer("encoder.actIn", config.Activation))

            For i As Integer = 0 To config.EncoderBlocks - 1
                Call Me._body.Add(New BatchNorm($"encoder.bn{i}", hidden))
                Call Me._body.Add(New ActivationLayer($"encoder.act{i}", config.Activation))
                Call Me._body.Add(New Linear($"encoder.fc{i}", hidden, hidden))
            Next

            Me._muHead = New Linear("encoder.mu", hidden, config.SemanticDim)
            Me._logVarHead = New Linear("encoder.logVar", hidden, config.SemanticDim)

            Me._params = ParameterGroups.FlattenMany(
                New IParameterized() {Me._body, Me._muHead, Me._logVarHead})
        End Sub

        Public ReadOnly Property Parameters As IEnumerable(Of Parameter) Implements IParameterized.Parameters
            Get
                Return _params
            End Get
        End Property

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
            Me._bodyOutput = Me._body.Forward(x0, training)
            Me._mu = Me._muHead.Forward(Me._bodyOutput, training)
            Me._logVar = Me._logVarHead.Forward(Me._bodyOutput, training)

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
            Dim dBody = Me._muHead.Backward(dMu) + Me._logVarHead.Backward(dLogVar)
            Call Me._body.Backward(dBody)
        End Sub
    End Class
End Namespace
