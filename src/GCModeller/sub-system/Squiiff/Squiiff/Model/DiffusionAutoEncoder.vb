Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.Squiiff.Diffusion
Imports SMRUCC.genomics.Analysis.Squiiff.NN
Imports std = System.Math

Namespace Model

    ''' <summary>单次训练步的损失明细。</summary>
    Public Class TrainStepResult

        ''' <summary>简化噪声预测损失 <c>E‖ε − ε_θ(x_t, t, z_sem)‖²</c>。</summary>
        Public Property DiffusionLoss As Double

        ''' <summary>KL 正则项（按批量平均）。</summary>
        Public Property KLLoss As Double

        ''' <summary>加权后的总损失 <c>diffusion + β·KL</c>。</summary>
        Public Property TotalLoss As Double

        ''' <summary>裁剪前的全局梯度范数。</summary>
        Public Property GradientNorm As Double
    End Class

    ''' <summary>
    ''' 扩散自编码器：语义编码器 + 条件 DDIM 去噪网络 + 噪声调度的组合体。
    '''
    ''' 双隐变量设计：
    ''' <list type="table">
    ''' <item>
    '''   <term><c>z_sem</c></term>
    '''   <description>由语义编码器 <c>Enc(x_0)</c> 得到，携带细胞类型 / 状态 / 扰动等高层语义，
    '''   作为条件去噪网络的显式条件，决定生成的"内容"。</description>
    ''' </item>
    ''' <item>
    '''   <term><c>x_T</c></term>
    '''   <description>由确定性 DDIM 反演得到，只携带低层随机细节（表达噪声），
    '''   提供生成的"细节纹理"。</description>
    ''' </item>
    ''' </list>
    ''' 训练目标是标准简化噪声预测损失，优化器为 Adam（学习率 <c>1e-3</c>）。
    ''' </summary>
    Public Class DiffusionAutoEncoder
        Implements IParameterized

        Private ReadOnly _config As SquiiffConfig
        Private ReadOnly _rng As Random
        Private ReadOnly _params As Parameter()

        Public Sub New(config As SquiiffConfig, geneCount As Integer, Optional seed As Integer? = Nothing)
            If config Is Nothing Then Throw New ArgumentNullException(NameOf(config))

            Dim err = config.Validate()
            If Not String.IsNullOrEmpty(err) Then Throw New ArgumentException($"配置非法: {err}", NameOf(config))

            Me._config = config
            Me.GeneCount = geneCount
            Me._rng = If(seed.HasValue, New Random(seed.Value), New Random(config.Seed))

            Me.Schedule = New DiffusionSchedule(config.DiffusionSteps, config.BetaStart, config.BetaEnd, config.AlphaBarFloor)
            Me.Sampler = New DdimSampler(Me.Schedule, config.InferenceSteps)
            Me.Encoder = New SemanticEncoder(config, geneCount, Me._rng)
            Me.Denoiser = New Denoiser(config, geneCount, config.SemanticDim)

            Me._params = ParameterGroups.FlattenMany(New IParameterized() {Me.Encoder, Me.Denoiser})
        End Sub

        Public ReadOnly Property Config As SquiiffConfig
            Get
                Return _config
            End Get
        End Property

        ''' <summary>基因维度（表达谱长度）。</summary>
        Public ReadOnly Property GeneCount As Integer

        ''' <summary>语义隐变量维度。</summary>
        Public ReadOnly Property SemanticDim As Integer
            Get
                Return _config.SemanticDim
            End Get
        End Property

        ''' <summary>噪声调度（前向加噪）。</summary>
        Public ReadOnly Property Schedule As DiffusionSchedule

        ''' <summary>DDIM 采样 / 反演器。</summary>
        Public ReadOnly Property Sampler As DdimSampler

        ''' <summary>语义编码器（VAE）。</summary>
        Public ReadOnly Property Encoder As SemanticEncoder

        ''' <summary>条件去噪网络。</summary>
        Public ReadOnly Property Denoiser As Denoiser

        Public ReadOnly Property Parameters As IEnumerable(Of Parameter) Implements IParameterized.Parameters
            Get
                Return _params
            End Get
        End Property

        ''' <summary>可训练参数总元素数（诊断用）。</summary>
        Public ReadOnly Property ParameterSize As Integer
            Get
                Dim n As Integer = 0
                For Each p In _params
                    n += p.Size
                Next
                Return n
            End Get
        End Property

#Region "编码 / 解码"

        ''' <summary>语义编码。<paramref name="training"/> 为 False 时返回确定性的 <c>μ</c>。</summary>
        Public Function Encode(x0 As Tensor, Optional training As Boolean = False) As Tensor
            Return Me.Encoder.Forward(x0, training)
        End Function

        ''' <summary>生成标准正态噪声（与模型随机种子一致，可复现）。</summary>
        Public Function Noise(shape As Integer()) As Tensor
            Return TensorUtil.StandardNormal(shape, Me._rng)
        End Function

        ''' <summary>
        ''' 确定性编码到随机子码：<c>x_0 → x_T</c>（DDIM 反演）。
        ''' </summary>
        Public Function EncodeToSubcode(x0 As Tensor, Optional snapshots As List(Of SamplingSnapshot) = Nothing) As Tensor
            Dim zSem = Me.Encode(x0, training:=False)
            Return Me.Sampler.Invert(Me.Denoiser, zSem, x0, snapshots)
        End Function

        ''' <summary>
        ''' 条件解码：以联合隐变量 <c>(z_sem, x_T)</c> 为输入，用条件 DDIM 生成转录组。
        ''' </summary>
        Public Function DecodeFromLatent(zSem As Tensor, xT As Tensor, Optional snapshots As List(Of SamplingSnapshot) = Nothing) As Tensor
            Return Me.Sampler.Sample(Me.Denoiser, zSem, xT, snapshots)
        End Function

        ''' <summary>
        ''' 自编码重建：<c>x_0 → (z_sem, x_T) → x̂_0</c>，用于评估扩散自编码器的重建保真度。
        ''' </summary>
        Public Function Reconstruct(x0 As Tensor, Optional snapshots As List(Of SamplingSnapshot) = Nothing) As Tensor
            Dim zSem = Me.Encode(x0, training:=False)
            Dim xT = Me.Sampler.Invert(Me.Denoiser, zSem, x0)
            Return Me.Sampler.Sample(Me.Denoiser, zSem, xT, snapshots)
        End Function

#End Region

#Region "训练"

        ''' <summary>
        ''' 单次训练步：编码 → 随机时间步加噪 → 条件去噪预测 → 反向 → 梯度裁剪 → Adam 更新。
        ''' </summary>
        ''' <param name="x0">一个批量的干净表达谱 <c>[B,G]</c>。</param>
        ''' <param name="optimizer">优化器（内部完成梯度裁剪与参数更新）。</param>
        ''' <param name="learningRate">本步学习率（由训练器按 warmup + 余弦退火给出）。</param>
        Public Function TrainStep(x0 As Tensor, optimizer As AdamOptimizer, learningRate As Double) As TrainStepResult
            Dim batch = x0.Shape(0)

            ' 1) 语义编码（训练模式下重参数化采样）
            Dim zSem = Me.Encoder.Forward(x0, training:=True)

            ' 2) 逐样本随机时间步 + 一步式加噪
            Dim t = Me.Schedule.SampleTimesteps(batch, Me._rng)
            Dim eps = TensorUtil.StandardNormal(x0.Shape, Me._rng)
            Dim xt = Me.Schedule.AddNoise(x0, t, eps)

            ' 3) 条件去噪预测
            Dim epsHat = Me.Denoiser.Predict(xt, t, zSem, training:=True)

            ' 4) 损失：L = mean((ε̂ − ε)²) + β·KL
            Dim diff = epsHat - eps
            Dim diffusionLoss = TensorUtil.MeanAll(diff.ElementwiseMultiply(diff))
            Dim kl = Me.Encoder.KLDivergence()
            Dim totalLoss = diffusionLoss + Me._config.BetaKL * kl

            ' 5) 反向：dL/dε̂ = 2·(ε̂ − ε) / N
            Dim dEps = TensorUtil.Scale(diff, 2.0 / diff.Length)

            Dim dZsem As Tensor = Nothing
            Call Me.Denoiser.Backward(dEps, dZsem)

            ' 6) 反向：语义编码器（含重参数化与 KL 的梯度）
            Call Me.Encoder.Backward(dZsem, Me._config.BetaKL)

            ' 7) 梯度裁剪 + 参数更新
            Dim gradNorm = optimizer.ClipGradients()
            optimizer.LearningRate = learningRate
            Call optimizer.Step()

            Return New TrainStepResult With {
                .DiffusionLoss = diffusionLoss,
                .KLLoss = kl,
                .TotalLoss = totalLoss,
                .GradientNorm = gradNorm
            }
        End Function

#End Region
    End Class
End Namespace
