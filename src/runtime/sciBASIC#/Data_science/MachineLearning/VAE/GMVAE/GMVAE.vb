' ============================================================================
' GMVAE.vb - 高斯混合变分自编码器（潜空间高斯混合先验）
'
' 生成模型：
'   z ~ p(z) = Σ_k π_k · N(z | μ_k, Σ_k)     潜空间上的高斯混合先验
'   x | z ~ p(x | z)                          解码器
'
' 变分后验：
'   q(z|x) = N(μ(x), diag(σ²(x)))              单一高斯编码器
'   类别责任度 r(y=k|x) ∝ π_k · N(μ(x) | μ_k, Σ_k)   由潜空间几何直接给出
'
' 目标函数（ELBO 的负数，按样本平均）：
'   L = recon + β · Σ_k r_k·ln(r_k/π_k)
'             + γ · Σ_k r_k·KL( N(μ,σ²) || N(μ_k,Σ_k) )
'
' ---------------------------------------------------------------------------
' 为什么要这样重构（相对旧实现的三处结构性修正）：
'
' 1. 旧实现把 q(z|x) 这个**单峰**后验同时朝 K 个先验成分拉，而且 q(y|x) 是
'    一个独立分类器，其唯一梯度来自 KL(q(y)||p(y))——而 p(y) 又被学成
'    mean(q(y|x))，于是 KL 无需任何判别性即可消掉，聚类退化为常数标签。
'
' 2. 引入「每成分独立解码器」会走向另一个极端：一旦某个成分重建略好，
'    责任度就趋于 one-hot；而 one-hot 处 softmax 的 Jacobian 恒为零，
'    梯度永久消失，模型卡死在「只用一个成分」的退化解上。
'
' 3. 因此这里把类别变量交给**潜空间的高斯混合**：责任度由 μ(x) 与成分
'    先验的几何关系解析给出，既不会饱和，也不需要额外的判别式分类器；
'    编码器只是一个普通的高斯编码器，梯度路径干净。
'
' 另外三处数值修复：
'   4. 重参数化梯度透传 ε：∂L/∂logσ² = ∂L/∂z·0.5·ε（旧实现误用 σ）；
'   5. 先验方差梯度采用正确的 0.5·(1 − (σ²+Δ²)/σ²_prior)（旧实现方向相反，
'      会使 var → 0 正反馈坍缩）；
'   6. 重建损失按 批次×维度 归一（旧实现只按批次，尺度随维度线性增长）。
' ============================================================================

Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.Core
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.Core.Layers
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.Core.Numerics
Imports std = System.Math

''' <summary>
''' 高斯混合变分自编码器 (Gaussian Mixture VAE)
''' </summary>
''' <remarks>
''' 通过引入类别隐变量 y 和连续隐变量 z, 实现对数据的多模态建模
''' 适合于聚类 + 生成任务
''' </remarks>
Public Class GMVAE
    Implements IGenerativeModel
    Implements IProbabilisticClusterer
    Implements IModelPersistable

    ''' <summary>模型文件的数据结构版本。</summary>
    Public Const ModelFileSchemaVersion As Integer = 3

#Region "私有字段"

    Private _inputDim As Integer
    Private _latentDim As Integer
    Private _nComponents As Integer
    Private _hiddenDim As Integer
    Private _seed As Integer?

    ' 编码器 q(z|x)（统一 [in, out] 布局，前向直接走 MatMul）
    Private _enc_W1, _enc_b1 As Tensor   ' [inputDim, hidden] / [1, hidden]
    Private _enc_W2, _enc_b2 As Tensor   ' μ 头    [hidden, latent]
    Private _enc_W3, _enc_b3 As Tensor   ' logσ² 头 [hidden, latent]

    ' 解码器 p(x|z)
    Private _dec_W1, _dec_b1 As Tensor   ' [latent, hidden]
    Private _dec_W2, _dec_b2 As Tensor   ' [hidden, inputDim]

    ' 潜空间高斯混合先验 p(z)
    Private _priorMu As Tensor        ' [K, latent]
    Private _priorLogVar As Tensor    ' [K, latent]
    Private _priorLogits As Tensor    ' [K]

    ' 训练历史
    Private _lossHistory As New List(Of Double)

    ' 每个参数张量一个 Adam 状态
    Private ReadOnly _optimizers As New Dictionary(Of String, Adam)

    Private _rng As RandomGaussian
    Private _fitted As Boolean = False

#End Region

#Region "属性"

    ''' <summary>输入维度</summary>
    Public ReadOnly Property InputDim As Integer Implements IGenerativeModel.InputDim
        Get
            Return _inputDim
        End Get
    End Property

    ''' <summary>连续隐变量维度</summary>
    Public ReadOnly Property LatentDim As Integer
        Get
            Return _latentDim
        End Get
    End Property

    ''' <summary>混合成分数量 (类别数)</summary>
    Public ReadOnly Property NComponents As Integer Implements IProbabilisticClusterer.NComponents
        Get
            Return _nComponents
        End Get
    End Property

    ''' <summary>训练损失历史</summary>
    Public ReadOnly Property LossHistory As List(Of Double)
        Get
            Return _lossHistory
        End Get
    End Property

    ''' <summary>模型是否已完成训练。</summary>
    Public ReadOnly Property IsFitted As Boolean Implements IGenerativeModel.IsFitted
        Get
            Return _fitted
        End Get
    End Property

    ''' <summary>先验 p(z|y) 的均值，形状 [K, latent]（只读诊断）。</summary>
    Public ReadOnly Property PriorMean As Tensor
        Get
            Return _priorMu
        End Get
    End Property

    ''' <summary>先验 p(z|y) 的对数方差，形状 [K, latent]（只读诊断）。</summary>
    Public ReadOnly Property PriorLogVariance As Tensor
        Get
            Return _priorLogVar
        End Get
    End Property

    ''' <summary>混合权重 π_k，形状 [K]（只读诊断）。</summary>
    Public ReadOnly Property PriorWeights As Double()
        Get
            Return PriorProbability()
        End Get
    End Property

#End Region

#Region "行为开关"

    ''' <summary>
    ''' 类别先验 p(y) 的形式。默认固定均匀先验；
    ''' 设为 <see cref="GmvaePriorMode.Learned"/> 可以复现重构前的行为。
    ''' </summary>
    Public Property PriorMode As GmvaePriorMode = GmvaePriorMode.Uniform

    ''' <summary>重建项的似然类型。</summary>
    Public Property OutputLikelihood As GmvaeLikelihood = GmvaeLikelihood.Gaussian

    ''' <summary>
    ''' 重建损失的归一化方式。
    '''
    ''' 默认按批归一（ELBO 的标准形式：对数似然对数据维度求和之后按批平均）。
    ''' 若改为按元素归一，重建项的梯度会被额外除以输入维度，与不随维度缩放的
    ''' KL 项严重失衡——KL 会压垮重建项，导致后验坍缩（μ 退化为常数）。
    ''' <see cref="GmvaeReconstructionScale.PerElement"/> 仅用于跨数据集比较损失的尺度。
    ''' </summary>
    Public Property ReconstructionScale As GmvaeReconstructionScale = GmvaeReconstructionScale.PerBatch

    ''' <summary>优化器类型。</summary>
    Public Property Optimizer As GmvaeOptimizer = GmvaeOptimizer.Adam

    ''' <summary>梯度 L2 范数上限；零或负数表示不裁剪。</summary>
    Public Property GradientClip As Double = 0

#End Region

#Region "构造函数"

    ''' <summary>
    ''' 创建 GMVAE 模型
    ''' </summary>
    ''' <param name="inputDim">输入数据维度</param>
    ''' <param name="latentDim">连续隐变量维度</param>
    ''' <param name="nComponents">混合成分数量 K</param>
    ''' <param name="hiddenDim">隐藏层维度</param>
    ''' <param name="seed">随机种子</param>
    Public Sub New(inputDim As Integer,
                   Optional latentDim As Integer = 8,
                   Optional nComponents As Integer = 5,
                   Optional hiddenDim As Integer = 64,
                   Optional seed As Integer? = Nothing)
        If inputDim <= 0 Then
            Throw New ArgumentException("输入维度必须大于 0", NameOf(inputDim))
        End If

        If latentDim <= 0 Then
            Throw New ArgumentException("隐变量维度必须大于 0", NameOf(latentDim))
        End If

        If nComponents <= 0 Then
            Throw New ArgumentException("混合成分数量必须大于 0", NameOf(nComponents))
        End If

        _inputDim = inputDim
        _latentDim = latentDim
        _nComponents = nComponents
        _hiddenDim = hiddenDim
        _seed = seed

        Call InitializeNetwork()
    End Sub

    ''' <summary>
    ''' 初始化所有网络参数
    ''' </summary>
    Private Sub InitializeNetwork()
        _rng = If(_seed.HasValue, New RandomGaussian(_seed.Value), New RandomGaussian())

        Dim baseSeed As Integer = If(_seed.HasValue, _seed.Value, Environment.TickCount)

        _enc_W1 = Tensor.XavierInit(_inputDim, _hiddenDim, baseSeed)
        _enc_b1 = Tensor.Zeros(New Integer() {1, _hiddenDim})
        _enc_W2 = Tensor.XavierInit(_hiddenDim, _latentDim, baseSeed + 1)
        _enc_b2 = Tensor.Zeros(New Integer() {1, _latentDim})
        _enc_W3 = Tensor.XavierInit(_hiddenDim, _latentDim, baseSeed + 2)
        _enc_b3 = Tensor.Zeros(New Integer() {1, _latentDim})

        _dec_W1 = Tensor.XavierInit(_latentDim, _hiddenDim, baseSeed + 3)
        _dec_b1 = Tensor.Zeros(New Integer() {1, _hiddenDim})
        _dec_W2 = Tensor.XavierInit(_hiddenDim, _inputDim, baseSeed + 4)
        _dec_b2 = Tensor.Zeros(New Integer() {1, _inputDim})

        ' 先验均值需要足够散开：若 K 个成分几乎重合，
        ' 责任度退化为均匀分布，KL 项无法区分成分。
        Dim priorMuData As Double() = New Double(_nComponents * _latentDim - 1) {}

        For k As Integer = 0 To _nComponents - 1
            For d As Integer = 0 To _latentDim - 1
                priorMuData(k * _latentDim + d) = _rng.NextNormal(0.0, 2.0)
            Next
        Next

        _priorMu = New Tensor(priorMuData, _nComponents, _latentDim)
        _priorLogVar = Tensor.Zeros(New Integer() {_nComponents, _latentDim})
        _priorLogits = Tensor.Zeros(New Integer() {_nComponents})
    End Sub

#End Region

#Region "基础算子"

    ''' <summary>
    ''' 线性层：y = x·W + b。
    '''
    ''' 刻意不在 <see cref="Tensor.MatMul"/> 的返回值上就地累加偏置：Tensor 内部会缓存
    ''' 行视图 / 转置视图，就地写入可能污染被复用的缓冲区。这里一律分配新张量。
    ''' </summary>
    Private Shared Function Affine(x As Tensor, w As Tensor, b As Tensor) As Tensor
        Dim product As Tensor = x.MatMul(w)
        Dim rows As Integer = product.Shape(0)
        Dim columns As Integer = product.Shape(1)
        Dim source As Double() = product.Data
        Dim result As New Tensor(rows, columns)
        Dim buffer As Double() = result.Data
        Dim bias As Double() = b.Data

        For i As Integer = 0 To rows - 1
            Dim baseIndex As Integer = i * columns

            For j As Integer = 0 To columns - 1
                buffer(baseIndex + j) = source(baseIndex + j) + bias(j)
            Next
        Next

        Return result
    End Function

    ''' <summary>线性层 + ReLU。</summary>
    Private Shared Function ReluAffine(x As Tensor, w As Tensor, b As Tensor) As Tensor
        Dim z As Tensor = Affine(x, w, b)
        Dim buffer As Double() = z.Data

        For i As Integer = 0 To buffer.Length - 1
            If buffer(i) < 0 Then
                buffer(i) = 0
            End If
        Next

        Return z
    End Function

    ''' <summary>用激活输出的符号作为 ReLU 掩码裁剪梯度。</summary>
    Private Shared Function MaskRelu(gradient As Tensor, activation As Tensor) As Tensor
        Dim data As Double() = CType(gradient.Data.Clone(), Double())
        Dim mask As Double() = activation.Data

        For i As Integer = 0 To data.Length - 1
            If mask(i) <= 0 Then
                data(i) = 0
            End If
        Next

        Return New Tensor(data, gradient.Shape)
    End Function

    Private Shared Function Sigmoid(value As Double) As Double
        If value >= 0 Then
            Return 1.0 / (1.0 + std.Exp(-value))
        End If

        Dim exponent As Double = std.Exp(value)

        Return exponent / (1.0 + exponent)
    End Function

    ''' <summary>采样标准正态噪声。</summary>
    Private Shared Function SampleStandardNormal(rows As Integer, columns As Integer, rng As RandomGaussian) As Tensor
        Dim result As New Tensor(rows, columns)
        Dim buffer As Double() = result.Data

        Call rng.NextNormals(buffer, 0, buffer.Length)

        Return result
    End Function

    ''' <summary>重参数化：z = μ + exp(0.5·logσ²)·ε。</summary>
    Private Shared Function Reparameterize(mu As Tensor, logVar As Tensor, epsilon As Tensor) As Tensor
        Dim data As Double() = New Double(mu.Length - 1) {}
        Dim meanValues As Double() = mu.Data
        Dim logVariances As Double() = logVar.Data
        Dim noises As Double() = epsilon.Data

        For i As Integer = 0 To data.Length - 1
            data(i) = meanValues(i) + std.Exp(0.5 * logVariances(i)) * noises(i)
        Next

        Return New Tensor(data, mu.Shape)
    End Function

    ''' <summary>把对数方差夹取到安全区间（返回新张量）。</summary>
    Private Shared Function ClampLogVariance(logVar As Tensor) As Tensor
        Dim result As New Tensor(logVar.Shape)
        Dim source As Double() = logVar.Data
        Dim buffer As Double() = result.Data

        For i As Integer = 0 To buffer.Length - 1
            buffer(i) = LogSumExpOps.ClipLogVariance(source(i))
        Next

        Return result
    End Function

    Private Function EncodeMean(x As Tensor) As Tensor
        Dim hidden As Tensor = ReluAffine(x, _enc_W1, _enc_b1)

        Return Affine(hidden, _enc_W2, _enc_b2)
    End Function

    Private Function EncodeLogVariance(x As Tensor) As Tensor
        Dim hidden As Tensor = ReluAffine(x, _enc_W1, _enc_b1)

        Return ClampLogVariance(Affine(hidden, _enc_W3, _enc_b3))
    End Function

    ''' <summary>解码器 p(x|z)：输出 logits（伯努利）或原始值（高斯）。</summary>
    Private Function Decode(z As Tensor) As Tensor
        Dim hidden As Tensor = Nothing

        Return DecodeWithHidden(z, hidden)
    End Function

    ''' <summary>解码器前向，同时返回隐含层激活（反向传播需要其 ReLU 掩码）。</summary>
    Private Function DecodeWithHidden(z As Tensor, ByRef hidden As Tensor) As Tensor
        hidden = ReluAffine(z, _dec_W1, _dec_b1)

        Return Affine(hidden, _dec_W2, _dec_b2)
    End Function

#End Region

#Region "潜空间高斯混合先验"

    ''' <summary>当前使用的类别先验概率 π。</summary>
    Private Function PriorProbability() As Double()
        Dim result As Double() = New Double(_nComponents - 1) {}

        If PriorMode = GmvaePriorMode.Learned Then
            Dim logits As Double() = _priorLogits.Data
            Dim maxValue As Double = Double.NegativeInfinity

            For k As Integer = 0 To _nComponents - 1
                If logits(k) > maxValue Then
                    maxValue = logits(k)
                End If
            Next

            Dim acc As Double = 0

            For k As Integer = 0 To _nComponents - 1
                result(k) = std.Exp(logits(k) - maxValue)
                acc += result(k)
            Next

            For k As Integer = 0 To _nComponents - 1
                result(k) /= acc
            Next
        Else
            For k As Integer = 0 To _nComponents - 1
                result(k) = 1.0 / _nComponents
            Next
        End If

        Return result
    End Function

    ''' <summary>
    ''' 计算每个样本对每个成分的责任度 r(y=k|x) ∝ π_k·N(μ(x) | μ_k, Σ_k)。
    ''' 责任度由潜空间几何解析给出，不经过可训练的 softmax 分类器，
    ''' 因此不存在 one-hot 饱和导致的梯度消失。
    ''' </summary>
    ''' <param name="meanValues">编码器均值 [N, latent]</param>
    ''' <param name="logProbability">输出的对数责任度 [N, K]（可为 Nothing）</param>
    Public Function ComputeResponsibilities(meanValues As Tensor) As Double(,)
        Dim logProbability As Double(,) = Nothing

        Return ResponsibilitiesCore(meanValues, logProbability)
    End Function

    Private Function ResponsibilitiesCore(meanValues As Tensor, ByRef logProbability As Double(,)) As Double(,)
        Dim rows As Integer = meanValues.Shape(0)
        Dim latent As Integer = _latentDim
        Dim components As Integer = _nComponents
        Dim prior As Double() = PriorProbability()
        Dim priorMuValues As Double() = _priorMu.Data
        Dim priorLogVarValues As Double() = _priorLogVar.Data
        Dim meanData As Double() = meanValues.Data
        Dim logDensity As Double(,) = New Double(rows - 1, components - 1) {}

        For k As Integer = 0 To components - 1
            Dim priorOffset As Integer = k * latent
            Dim logWeight As Double = LogSumExpOps.SafeLog(prior(k))

            For i As Integer = 0 To rows - 1
                Dim baseIndex As Integer = i * latent
                Dim squared As Double = 0
                Dim logDeterminant As Double = 0

                For d As Integer = 0 To latent - 1
                    Dim priorVariance As Double = std.Max(std.Exp(priorLogVarValues(priorOffset + d)), LinearAlgebraOps.MinVariance)
                    Dim delta As Double = meanData(baseIndex + d) - priorMuValues(priorOffset + d)

                    squared += delta * delta / priorVariance
                    logDeterminant += std.Log(priorVariance)
                Next

                logDensity(i, k) = logWeight - 0.5 * (latent * std.Log(2.0 * std.PI) + logDeterminant + squared)
            Next
        Next

        Dim result As Double(,) = New Double(rows - 1, components - 1) {}

        For i As Integer = 0 To rows - 1
            Dim norm As Double = LogSumExpOps.ComputeRow(logDensity, i)

            For k As Integer = 0 To components - 1
                Dim value As Double = logDensity(i, k) - norm

                logDensity(i, k) = value
                result(i, k) = std.Exp(value)
            Next
        Next

        logProbability = logDensity

        Return result
    End Function

#End Region

#Region "前向与损失"

    Private Class BatchCache
        Public Input As Tensor
        Public Hidden As Tensor
        Public Mean As Tensor
        Public LogVar As Tensor
        Public Epsilon As Tensor
        Public Sample As Tensor
        Public Decoded As Tensor
        Public DecodedHidden As Tensor
        Public Responsibilities As Double(,)
        Public ReconLoss As Double
        Public KlCategorical As Double
        Public KlComponent As Double
        Public TotalLoss As Double
    End Class

    Private Function ForwardBatch(x As Tensor, rng As RandomGaussian) As BatchCache
        Dim rows As Integer = x.Shape(0)
        Dim cache As New BatchCache With {.Input = x}

        cache.Hidden = ReluAffine(x, _enc_W1, _enc_b1)
        cache.Mean = Affine(cache.Hidden, _enc_W2, _enc_b2)
        cache.LogVar = ClampLogVariance(Affine(cache.Hidden, _enc_W3, _enc_b3))
        cache.Epsilon = SampleStandardNormal(rows, _latentDim, rng)
        cache.Sample = Reparameterize(cache.Mean, cache.LogVar, cache.Epsilon)

        Dim decodedHidden As Tensor = Nothing
        cache.Decoded = DecodeWithHidden(cache.Sample, decodedHidden)
        cache.DecodedHidden = decodedHidden

        Dim logProbability As Double(,) = Nothing
        cache.Responsibilities = ResponsibilitiesCore(cache.Mean, logProbability)

        Return cache
    End Function

    ''' <summary>计算并缓存损失分解。</summary>
    Private Function ComputeLoss(cache As BatchCache, beta As Double, gamma As Double) As Double
        Dim rows As Integer = cache.Input.Shape(0)
        Dim latent As Integer = _latentDim
        Dim components As Integer = _nComponents
        Dim denominator As Double = If(ReconstructionScale = GmvaeReconstructionScale.PerElement,
                                       rows * _inputDim, CDbl(rows))

        Dim inputData As Double() = cache.Input.Data
        Dim decodedData As Double() = cache.Decoded.Data
        Dim meanData As Double() = cache.Mean.Data
        Dim logVarData As Double() = cache.LogVar.Data
        Dim priorMuValues As Double() = _priorMu.Data
        Dim priorLogVarValues As Double() = _priorLogVar.Data
        Dim prior As Double() = PriorProbability()

        ' ---- 重建项 ----
        Dim reconstruction As Double = 0

        For i As Integer = 0 To inputData.Length - 1
            If OutputLikelihood = GmvaeLikelihood.Gaussian Then
                Dim diff As Double = inputData(i) - decodedData(i)
                reconstruction += diff * diff
            Else
                Dim probability As Double = Sigmoid(decodedData(i))
                probability = std.Max(1.0E-12, std.Min(1.0 - 1.0E-12, probability))
                reconstruction += -(inputData(i) * std.Log(probability) + (1.0 - inputData(i)) * std.Log(1.0 - probability))
            End If
        Next

        reconstruction /= denominator

        ' ---- 类别项 + 成分 KL ----
        Dim klCategorical As Double = 0
        Dim klComponent As Double = 0

        For i As Integer = 0 To rows - 1
            Dim baseIndex As Integer = i * latent

            For k As Integer = 0 To components - 1
                Dim responsibility As Double = cache.Responsibilities(i, k)

                If responsibility <= 1.0E-12 Then
                    Continue For
                End If

                klCategorical += responsibility * std.Log(responsibility / prior(k))

                Dim priorOffset As Integer = k * latent
                Dim acc As Double = 0

                For d As Integer = 0 To latent - 1
                    Dim variance As Double = std.Max(std.Exp(logVarData(baseIndex + d)), LinearAlgebraOps.MinVariance)
                    Dim priorVariance As Double = std.Max(std.Exp(priorLogVarValues(priorOffset + d)), LinearAlgebraOps.MinVariance)
                    Dim delta As Double = meanData(baseIndex + d) - priorMuValues(priorOffset + d)

                    acc += std.Log(priorVariance / variance) + (variance + delta * delta) / priorVariance - 1.0
                Next

                klComponent += responsibility * 0.5 * acc
            Next
        Next

        klCategorical /= rows
        klComponent /= rows

        cache.ReconLoss = reconstruction
        cache.KlCategorical = klCategorical
        cache.KlComponent = klComponent
        cache.TotalLoss = reconstruction + beta * klCategorical + gamma * klComponent

        Return cache.TotalLoss
    End Function

#End Region

#Region "反向传播"

    Private Sub BackwardAndUpdate(cache As BatchCache, beta As Double, gamma As Double, learningRate As Double)
        Dim rows As Integer = cache.Input.Shape(0)
        Dim latent As Integer = _latentDim
        Dim components As Integer = _nComponents
        Dim hidden As Integer = _hiddenDim
        Dim denominator As Double = If(ReconstructionScale = GmvaeReconstructionScale.PerElement,
                                       rows * _inputDim, CDbl(rows))

        Dim inputData As Double() = cache.Input.Data
        Dim decodedData As Double() = cache.Decoded.Data
        Dim meanData As Double() = cache.Mean.Data
        Dim logVarData As Double() = cache.LogVar.Data
        Dim epsilonData As Double() = cache.Epsilon.Data
        Dim priorMuValues As Double() = _priorMu.Data
        Dim priorLogVarValues As Double() = _priorLogVar.Data
        Dim prior As Double() = PriorProbability()

        ' ================= 解码器 =================
        Dim dDecoded As New Tensor(rows, _inputDim)

        For i As Integer = 0 To dDecoded.Length - 1
            If OutputLikelihood = GmvaeLikelihood.Gaussian Then
                dDecoded.Data(i) = 2.0 * (decodedData(i) - inputData(i)) / denominator
            Else
                dDecoded.Data(i) = (Sigmoid(decodedData(i)) - inputData(i)) / denominator
            End If
        Next

        Dim dDecW2 As Tensor = cache.DecodedHidden.Transpose().MatMul(dDecoded)
        Dim dDecB2 As Tensor = dDecoded.Sum(0)
        Dim dDecHidden As Tensor = MaskRelu(dDecoded.MatMul(_dec_W2.Transpose()), cache.DecodedHidden)
        Dim dDecW1 As Tensor = cache.Sample.Transpose().MatMul(dDecHidden)
        Dim dDecB1 As Tensor = dDecHidden.Sum(0)
        Dim dSample As Tensor = dDecHidden.MatMul(_dec_W1.Transpose())
        Dim dSampleData As Double() = dSample.Data

        ' ================= 编码器：μ 与 logσ² =================
        Dim dMean As New Tensor(rows, latent)
        Dim dLogVar As New Tensor(rows, latent)
        Dim dMeanData As Double() = dMean.Data
        Dim dLogVarData As Double() = dLogVar.Data

        For i As Integer = 0 To rows - 1
            Dim baseIndex As Integer = i * latent

            For d As Integer = 0 To latent - 1
                Dim index As Integer = baseIndex + d
                Dim variance As Double = std.Max(std.Exp(logVarData(index)), LinearAlgebraOps.MinVariance)
                Dim gradMean As Double = dSampleData(index)
                Dim gradLogVar As Double = dSampleData(index) * 0.5 * epsilonData(index)

                For k As Integer = 0 To components - 1
                    Dim responsibility As Double = cache.Responsibilities(i, k)

                    If responsibility <= 1.0E-12 Then
                        Continue For
                    End If

                    Dim priorOffset As Integer = k * latent + d
                    Dim priorVariance As Double = std.Max(std.Exp(priorLogVarValues(priorOffset)), LinearAlgebraOps.MinVariance)
                    Dim delta As Double = meanData(index) - priorMuValues(priorOffset)

                    ' ∂KL/∂μ = (μ-μ_k)/Σ_k
                    gradMean += gamma * responsibility * delta / priorVariance
                    ' ∂KL/∂logσ² = 0.5·(σ²/Σ_k − 1)
                    gradLogVar += gamma * responsibility * 0.5 * (variance / priorVariance - 1.0)
                Next

                dMeanData(index) = gradMean
                dLogVarData(index) = gradLogVar
            Next
        Next

        ' 编码器反向
        Dim dEncW2 As Tensor = cache.Hidden.Transpose().MatMul(dMean)
        Dim dEncB2 As Tensor = dMean.Sum(0)
        Dim dEncW3 As Tensor = cache.Hidden.Transpose().MatMul(dLogVar)
        Dim dEncB3 As Tensor = dLogVar.Sum(0)
        Dim dHidden As Tensor = dMean.MatMul(_enc_W2.Transpose()) + dLogVar.MatMul(_enc_W3.Transpose())
        Dim dHiddenMasked As Tensor = MaskRelu(dHidden, cache.Hidden)
        Dim dEncW1 As Tensor = cache.Input.Transpose().MatMul(dHiddenMasked)
        Dim dEncB1 As Tensor = dHiddenMasked.Sum(0)

        ' ================= 参数更新 =================
        Call ApplyGradient("dec_W1", _dec_W1, dDecW1, learningRate)
        Call ApplyGradient("dec_b1", _dec_b1, dDecB1, learningRate)
        Call ApplyGradient("dec_W2", _dec_W2, dDecW2, learningRate)
        Call ApplyGradient("dec_b2", _dec_b2, dDecB2, learningRate)

        Call ApplyGradient("enc_W1", _enc_W1, dEncW1, learningRate)
        Call ApplyGradient("enc_b1", _enc_b1, dEncB1, learningRate)
        Call ApplyGradient("enc_W2", _enc_W2, dEncW2, learningRate)
        Call ApplyGradient("enc_b2", _enc_b2, dEncB2, learningRate)
        Call ApplyGradient("enc_W3", _enc_W3, dEncW3, learningRate)
        Call ApplyGradient("enc_b3", _enc_b3, dEncB3, learningRate)

        ' 先验参数 p(z) 不参与梯度下降：它们由每轮结束时的 EM M 步更新。
        ' 若改用梯度下降，K 个成分中心会互相合并（GMM 的经典退化），
        ' 责任度随之退化为单成分。
    End Sub

#End Region

#Region "先验的 EM M 步"

    ''' <summary>
    ''' 用闭式 M 步更新潜空间高斯混合先验 π、μ_k、Σ_k。
    '''
    ''' 相比梯度下降，EM 的 M 步不会出现「所有中心一起漂移到全局均值」的退化：
    ''' 责任度为零的成分保持原位，因此成分之间不会互相吞并。
    ''' </summary>
    Private Sub UpdatePriorByExpectationMaximization(X As Tensor)
        Dim sampleCount As Integer = X.Shape(0)
        Dim latent As Integer = _latentDim
        Dim components As Integer = _nComponents
        Dim batchSize As Integer = 256

        Dim counts As Double() = New Double(components - 1) {}
        Dim sums As Double(,) = New Double(components - 1, latent - 1) {}
        Dim squares As Double(,) = New Double(components - 1, latent - 1) {}

        For batchStart As Integer = 0 To sampleCount - 1 Step batchSize
            Dim count As Integer = std.Min(batchSize, sampleCount - batchStart)
            Dim batch As New Tensor(count, _inputDim)

            Array.Copy(X.Data, batchStart * _inputDim, batch.Data, 0, count * _inputDim)

            Dim meanValues As Tensor = EncodeMean(batch)
            Dim responsibilities As Double(,) = ResponsibilitiesCore(meanValues, Nothing)
            Dim meanData As Double() = meanValues.Data

            For i As Integer = 0 To count - 1
                Dim baseIndex As Integer = i * latent

                For k As Integer = 0 To components - 1
                    Dim weight As Double = responsibilities(i, k)

                    If weight <= 1.0E-12 Then
                        Continue For
                    End If

                    counts(k) += weight

                    For d As Integer = 0 To latent - 1
                        Dim value As Double = meanData(baseIndex + d)
                        sums(k, d) += weight * value
                        squares(k, d) += weight * value * value
                    Next
                Next
            Next
        Next

        Dim weightsData As Double() = New Double(components - 1) {}
        Dim total As Double = 0

        For k As Integer = 0 To components - 1
            weightsData(k) = std.Max(counts(k), 1.0)
            total += weightsData(k)
        Next

        For k As Integer = 0 To components - 1
            weightsData(k) /= total
        Next

        Dim logits As Double() = _priorLogits.Data

        If PriorMode = GmvaePriorMode.Learned Then
            For k As Integer = 0 To components - 1
                logits(k) = std.Log(std.Max(weightsData(k), 1.0E-12))
            Next
        End If

        Dim priorMuData As Double() = _priorMu.Data
        Dim priorLogVarData As Double() = _priorLogVar.Data

        For k As Integer = 0 To components - 1
            ' 空成分：保留原位，避免被拉向全局均值
            If counts(k) < 1.0 Then
                Continue For
            End If

            For d As Integer = 0 To latent - 1
                Dim meanValue As Double = sums(k, d) / counts(k)
                Dim variance As Double = squares(k, d) / counts(k) - meanValue * meanValue

                priorMuData(k * latent + d) = meanValue
                priorLogVarData(k * latent + d) = std.Log(std.Max(variance, LinearAlgebraOps.MinVariance))
            Next
        Next
    End Sub

    ''' <summary>按当前优化器设置更新单个参数张量。</summary>
    ''' <param name="name">参数名，用于稳定地索引优化器状态。
    ''' 不能用张量对象作为字典键：张量的哈希可能随其可变数据变化，
    ''' 那样每步都会查不到旧状态而新建优化器，Adam 的偏差校正会退化为
    ''' 「符号 SGD」，学习几乎停滞。</param>
    Private Sub ApplyGradient(name As String, parameter As Tensor, gradient As Tensor, learningRate As Double)
        If parameter Is Nothing OrElse gradient Is Nothing Then
            Return
        End If

        If parameter.Length <> gradient.Length Then
            Throw New InvalidOperationException(
                $"梯度形状不符：参数 {parameter.Length}，梯度 {gradient.Length}")
        End If

        If GradientClip > 0 Then
            Call GradientCheck.ClipByNorm(gradient.Data, GradientClip)
            gradient.MarkHostModified()
        End If

        If Optimizer = GmvaeOptimizer.Adam Then
            Dim state As Adam = Nothing

            If Not _optimizers.TryGetValue(name, state) Then
                state = New Adam(parameter.Shape, learningRate)
                _optimizers(name) = state
            End If

            state.Update(parameter, gradient, learningRate)
        Else
            Dim parameters As Double() = parameter.Data
            Dim gradients As Double() = gradient.Data

            For i As Integer = 0 To parameters.Length - 1
                parameters(i) -= learningRate * gradients(i)
            Next

            parameter.MarkHostModified()
        End If
    End Sub

#End Region

#Region "训练"

    ''' <summary>
    ''' 训练 GMVAE 模型
    ''' </summary>
    ''' <param name="X">训练数据 [N, inputDim]</param>
    ''' <param name="epochs">训练轮数</param>
    ''' <param name="batchSize">批大小</param>
    ''' <param name="learningRate">学习率</param>
    ''' <param name="beta">类别 KL 散度权重</param>
    ''' <param name="gamma">
    ''' 连续 KL 散度权重。取值过大（例如 1.0）时 KL 项会压过重建项，
    ''' 在高维 / 强可分的多峰数据上容易触发后验坍缩（重构退化为常数）。
    ''' 经验上 0.1 ~ 0.5 更稳健；重构质量与聚类质量会同时受益。
    ''' </param>
    ''' <param name="verbose">是否打印训练进度</param>
    Public Sub Fit(X As Tensor,
                   Optional epochs As Integer = 100,
                   Optional batchSize As Integer = 32,
                   Optional learningRate As Double = 0.001,
                   Optional beta As Double = 1.0,
                   Optional gamma As Double = 1.0,
                   Optional verbose As Boolean = True)
        Dim options As New TrainingOptions With {
            .Epochs = epochs,
            .BatchSize = batchSize,
            .LearningRate = learningRate,
            .Beta = beta,
            .Gamma = gamma,
            .Verbose = verbose,
            .Seed = _seed
        }

        ' KL 退火：训练初期把 KL 权重从 0 线性升到目标值。
        ' 这是抑制后验坍缩（μ 不携带输入信息、重构退化为常数）的标准手段；
        ' 缺少退火时 KL 项在训练早期就会压垮重建项。
        options.KlWarmupEpochs = std.Max(1, epochs \ 5)

        Call FitCore(X, options, If(verbose, ConsoleTrainingObserver.ForGmvae(), New NullTrainingObserver()))
    End Sub

    ''' <summary>使用统一的训练选项训练模型（新增重载）。</summary>
    Public Function Fit(X As Tensor, options As TrainingOptions) As GMVAE
        Call FitCore(X, options, ConsoleTrainingObserver.ForGmvae())

        Return Me
    End Function

    Private Sub FitCore(X As Tensor, options As TrainingOptions, fallbackObserver As ITrainingObserver)
        If X Is Nothing Then
            Throw New ArgumentNullException(NameOf(X))
        End If

        If X.Rank <> 2 Then
            Throw New ArgumentException("输入数据必须是二维张量 [N, D]")
        End If

        If X.Shape(1) <> _inputDim Then
            Throw New ArgumentException($"输入维度不符: 期望 {_inputDim}, 实际 {X.Shape(1)}")
        End If

        Dim sampleCount As Integer = X.Shape(0)
        Dim epochs As Integer = std.Max(1, options.Epochs)
        Dim batchSize As Integer = If(options.BatchSize <= 0, sampleCount, options.BatchSize)

        _rng = If(_seed.HasValue, New RandomGaussian(_seed.Value), _rng)
        _lossHistory.Clear()
        _optimizers.Clear()

        Dim observer As ITrainingObserver = options.ResolveObserver(fallbackObserver)
        Dim shuffle As New RandomGaussian(If(_seed.HasValue, _seed.Value + 7919, Environment.TickCount))
        Dim indices As Integer() = New Integer(sampleCount - 1) {}

        For i As Integer = 0 To sampleCount - 1
            indices(i) = i
        Next

        For epoch As Integer = 1 To epochs
            Dim startTick As Integer = Environment.TickCount
            Dim beta As Double = options.BetaAt(epoch)
            Dim gamma As Double = options.GammaAt(epoch)

            Call observer.OnEpochBegin(epoch, epochs)

            For i As Integer = sampleCount - 1 To 1 Step -1
                Dim j As Integer = shuffle.NextIndex(i + 1)
                Dim temp As Integer = indices(i)
                indices(i) = indices(j)
                indices(j) = temp
            Next

            Dim epochLoss As Double = 0
            Dim epochRecon As Double = 0
            Dim epochKlY As Double = 0
            Dim epochKlZ As Double = 0
            Dim batches As Integer = 0

            For batchStart As Integer = 0 To sampleCount - 1 Step batchSize
                Dim count As Integer = std.Min(batchSize, sampleCount - batchStart)
                Dim batch As New Tensor(count, _inputDim)
                Dim buffer As Double() = batch.Data

                For i As Integer = 0 To count - 1
                    Dim source As Integer = indices(batchStart + i) * _inputDim

                    Array.Copy(X.Data, source, buffer, i * _inputDim, _inputDim)
                Next

                Dim cache As BatchCache = ForwardBatch(batch, _rng)
                Dim lossValue As Double = ComputeLoss(cache, beta, gamma)

                Call BackwardAndUpdate(cache, beta, gamma, options.LearningRate)

                epochLoss += lossValue
                epochRecon += cache.ReconLoss
                epochKlY += cache.KlCategorical
                epochKlZ += cache.KlComponent
                batches += 1
            Next

            If batches = 0 Then
                batches = 1
            End If

            epochLoss /= batches
            epochRecon /= batches
            epochKlY /= batches
            epochKlZ /= batches

            _lossHistory.Add(epochLoss)

            ' 每轮结束后用 EM 的 M 步更新潜空间混合先验
            Call UpdatePriorByExpectationMaximization(X)

            Dim losses As New Dictionary(Of String, Double) From {
                {"loss", epochLoss},
                {"recon", epochRecon},
                {"kl-y", epochKlY},
                {"kl-z", epochKlZ}
            }

            Call observer.OnEpochEnd(epoch, losses, Environment.TickCount - startTick)
        Next

        _fitted = True
    End Sub

#End Region

#Region "推理与生成"

    ''' <summary>
    ''' 编码输入数据, 返回 (类别概率, 隐变量均值, 隐变量 log 方差)
    ''' </summary>
    Public Sub Encode(x As Tensor,
                      ByRef yProba As Tensor,
                      ByRef zMean As Tensor, ByRef zLogVar As Tensor)
        Call EnsureFitted()
        Call ValidateInput(x)

        Dim meanValues As Tensor = EncodeMean(x)
        Dim logProbability As Double(,) = Nothing
        Dim responsibilities As Double(,) = ResponsibilitiesCore(meanValues, logProbability)
        Dim rows As Integer = x.Shape(0)

        Dim probability As New Tensor(rows, _nComponents)

        For i As Integer = 0 To rows - 1
            For k As Integer = 0 To _nComponents - 1
                probability.Data(i * _nComponents + k) = responsibilities(i, k)
            Next
        Next

        yProba = probability
        zMean = meanValues
        zLogVar = EncodeLogVariance(x)
    End Sub

    ''' <summary>
    ''' 重构输入数据
    ''' </summary>
    Public Function Reconstruct(x As Tensor, Optional useMean As Boolean = True) As Tensor
        Call EnsureFitted()
        Call ValidateInput(x)

        Dim rng As RandomGaussian = If(_rng, New RandomGaussian())
        Dim meanValues As Tensor = EncodeMean(x)
        Dim sample As Tensor

        If useMean Then
            sample = meanValues
        Else
            Dim logVar As Tensor = EncodeLogVariance(x)
            sample = Reparameterize(meanValues, logVar, SampleStandardNormal(x.Shape(0), _latentDim, rng))
        End If

        Dim decoded As Double() = Decode(sample).Data
        Dim result As New Tensor(x.Shape(0), _inputDim)
        Dim output As Double() = result.Data

        For i As Integer = 0 To output.Length - 1
            Dim value As Double = decoded(i)

            If OutputLikelihood = GmvaeLikelihood.Bernoulli Then
                value = Sigmoid(value)
            End If

            output(i) = value
        Next

        Return result
    End Function

    ''' <summary>
    ''' 从指定类别采样生成数据
    ''' </summary>
    ''' <param name="n">采样数量</param>
    ''' <param name="componentIdx">类别索引 (-1 表示按先验混合采样)</param>
    ''' <param name="seed">随机种子</param>
    Public Function Generate(n As Integer, Optional componentIdx As Integer = -1, Optional seed As Integer? = Nothing) As Tensor
        If n <= 0 Then
            Throw New ArgumentException("采样数量必须大于 0", NameOf(n))
        End If

        Call EnsureFitted()

        Dim rng As RandomGaussian = If(seed.HasValue, New RandomGaussian(seed.Value),
                                       If(_rng, New RandomGaussian()))

        Dim components As Integer = _nComponents
        Dim latent As Integer = _latentDim
        Dim prior As Double() = PriorProbability()

        Dim cumulative As Double() = New Double(components - 1) {}
        Dim acc As Double = 0

        For k As Integer = 0 To components - 1
            acc += prior(k)
            cumulative(k) = acc
        Next

        Dim result As New Tensor(n, _inputDim)
        Dim output As Double() = result.Data
        Dim priorMeans As Double() = _priorMu.Data
        Dim priorLogVariances As Double() = _priorLogVar.Data

        For i As Integer = 0 To n - 1
            Dim selected As Integer = componentIdx

            If selected < 0 OrElse selected >= components Then
                Dim threshold As Double = rng.NextDouble() * acc
                selected = components - 1

                For k As Integer = 0 To components - 1
                    If threshold <= cumulative(k) Then
                        selected = k
                        Exit For
                    End If
                Next
            End If

            Dim priorOffset As Integer = selected * latent
            Dim buffer As Double() = New Double(latent - 1) {}

            For d As Integer = 0 To latent - 1
                Dim variance As Double = std.Max(std.Exp(priorLogVariances(priorOffset + d)), LinearAlgebraOps.MinVariance)
                buffer(d) = priorMeans(priorOffset + d) + std.Sqrt(variance) * rng.NextNormal()
            Next

            Dim decoded As Double() = Decode(New Tensor(buffer, 1, latent)).Data

            For d As Integer = 0 To _inputDim - 1
                Dim value As Double = decoded(d)

                If OutputLikelihood = GmvaeLikelihood.Bernoulli Then
                    value = Sigmoid(value)
                End If

                output(i * _inputDim + d) = value
            Next
        Next

        Return result
    End Function

    Private Function GenerateFromPrior(count As Integer, seed As Integer?) As Tensor Implements IGenerativeModel.Generate
        Return Generate(count, -1, seed)
    End Function

    Private Function ReconstructDefault(x As Tensor) As Tensor Implements IGenerativeModel.Reconstruct
        Return Reconstruct(x, True)
    End Function

    ''' <summary>
    ''' 聚类: 返回每个样本最可能属于的类别
    ''' </summary>
    Public Function PredictClusters(X As Tensor) As Integer() Implements IProbabilisticClusterer.Predict
        Dim probabilities As Tensor = PredictProba(X)
        Dim rows As Integer = X.Shape(0)
        Dim components As Integer = _nComponents
        Dim buffer As Double() = probabilities.Data
        Dim labels As Integer() = New Integer(rows - 1) {}

        For i As Integer = 0 To rows - 1
            Dim maxValue As Double = Double.NegativeInfinity
            Dim maxIndex As Integer = 0
            Dim baseIndex As Integer = i * components

            For k As Integer = 0 To components - 1
                If buffer(baseIndex + k) > maxValue Then
                    maxValue = buffer(baseIndex + k)
                    maxIndex = k
                End If
            Next

            labels(i) = maxIndex
        Next

        Return labels
    End Function

    ''' <summary>类别后验责任度矩阵 [N, K]。</summary>
    Public Function PredictProba(X As Tensor) As Tensor Implements IProbabilisticClusterer.PredictProba
        Call EnsureFitted()
        Call ValidateInput(X)

        Dim meanValues As Tensor = EncodeMean(X)
        Dim responsibilities As Double(,) = ResponsibilitiesCore(meanValues, Nothing)
        Dim rows As Integer = X.Shape(0)
        Dim result As New Tensor(rows, _nComponents)

        For i As Integer = 0 To rows - 1
            For k As Integer = 0 To _nComponents - 1
                result.Data(i * _nComponents + k) = responsibilities(i, k)
            Next
        Next

        Return result
    End Function

#End Region

#Region "公开的前向接口（保持既有签名）"

    ''' <summary>
    ''' 完整前向传播
    ''' </summary>
    ''' <param name="x">输入 [batch, inputDim]</param>
    ''' <param name="rng">随机数生成器 (用于重参数化采样)</param>
    Public Sub Forward(x As Tensor, rng As Random,
                       ByRef yLogits As Tensor,
                       ByRef zMean As Tensor, ByRef zLogVar As Tensor,
                       ByRef zSample As Tensor, ByRef xRecon As Tensor)
        Call ValidateInput(x)

        Dim source As RandomGaussian = If(rng Is Nothing, New RandomGaussian(), New RandomGaussian(rng))
        Dim cache As BatchCache = ForwardBatch(x, source)
        Dim rows As Integer = x.Shape(0)
        Dim logProbability As Double(,) = Nothing

        Call ResponsibilitiesCore(cache.Mean, logProbability)

        Dim logits As New Tensor(rows, _nComponents)

        For i As Integer = 0 To rows - 1
            For k As Integer = 0 To _nComponents - 1
                logits.Data(i * _nComponents + k) = logProbability(i, k)
            Next
        Next

        yLogits = logits
        zMean = cache.Mean
        zLogVar = cache.LogVar
        zSample = cache.Sample
        xRecon = cache.Decoded
    End Sub

#End Region

#Region "持久化"

    Private ReadOnly Property SchemaVersionValue As Integer Implements IModelPersistable.SchemaVersion
        Get
            Return ModelFileSchemaVersion
        End Get
    End Property

    ''' <summary>把模型参数写入指定路径。</summary>
    Public Sub Save(path As String) Implements IModelPersistable.Save
        Call EnsureFitted()

        Dim parameters As New ModelParameters("GMVAE", ModelFileSchemaVersion)

        Call parameters.Add("enc_W1", _enc_W1)
        Call parameters.Add("enc_b1", _enc_b1)
        Call parameters.Add("enc_W2", _enc_W2)
        Call parameters.Add("enc_b2", _enc_b2)
        Call parameters.Add("enc_W3", _enc_W3)
        Call parameters.Add("enc_b3", _enc_b3)
        Call parameters.Add("dec_W1", _dec_W1)
        Call parameters.Add("dec_b1", _dec_b1)
        Call parameters.Add("dec_W2", _dec_W2)
        Call parameters.Add("dec_b2", _dec_b2)
        Call parameters.Add("priorMu", _priorMu)
        Call parameters.Add("priorLogVar", _priorLogVar)
        Call parameters.Add("priorLogits", _priorLogits)

        Call parameters.AddValue("inputDim", _inputDim)
        Call parameters.AddValue("latentDim", _latentDim)
        Call parameters.AddValue("nComponents", _nComponents)
        Call parameters.AddValue("hiddenDim", _hiddenDim)
        Call parameters.AddValue("priorMode", CInt(PriorMode))
        Call parameters.AddValue("likelihood", CInt(OutputLikelihood))

        Call parameters.Save(path)
    End Sub

    ''' <summary>从指定路径还原模型参数。</summary>
    Public Function TryLoad(path As String, ByRef errorMessage As String) As Boolean Implements IModelPersistable.TryLoad
        errorMessage = Nothing

        Try
            Dim parameters As ModelParameters = ModelParameters.Load(path)

            If Not String.Equals(parameters.ModelType, "GMVAE", StringComparison.Ordinal) Then
                errorMessage = $"模型类型不符: 期望 GMVAE，实际 {parameters.ModelType}"
                Return False
            End If

            Dim inputDim As Integer = CInt(parameters.GetValue("inputDim", 0))
            Dim latentDim As Integer = CInt(parameters.GetValue("latentDim", 0))
            Dim components As Integer = CInt(parameters.GetValue("nComponents", 0))
            Dim hiddenDim As Integer = CInt(parameters.GetValue("hiddenDim", 0))

            If inputDim <= 0 OrElse latentDim <= 0 OrElse components <= 0 OrElse hiddenDim <= 0 Then
                errorMessage = "模型文件缺少有效的维度信息"
                Return False
            End If

            _inputDim = inputDim
            _latentDim = latentDim
            _nComponents = components
            _hiddenDim = hiddenDim

            _enc_W1 = parameters.RequireTensor("enc_W1", New Integer() {inputDim, hiddenDim})
            _enc_b1 = parameters.RequireTensor("enc_b1", New Integer() {1, hiddenDim})
            _enc_W2 = parameters.RequireTensor("enc_W2", New Integer() {hiddenDim, latentDim})
            _enc_b2 = parameters.RequireTensor("enc_b2", New Integer() {1, latentDim})
            _enc_W3 = parameters.RequireTensor("enc_W3", New Integer() {hiddenDim, latentDim})
            _enc_b3 = parameters.RequireTensor("enc_b3", New Integer() {1, latentDim})
            _dec_W1 = parameters.RequireTensor("dec_W1", New Integer() {latentDim, hiddenDim})
            _dec_b1 = parameters.RequireTensor("dec_b1", New Integer() {1, hiddenDim})
            _dec_W2 = parameters.RequireTensor("dec_W2", New Integer() {hiddenDim, inputDim})
            _dec_b2 = parameters.RequireTensor("dec_b2", New Integer() {1, inputDim})
            _priorMu = parameters.RequireTensor("priorMu", New Integer() {components, latentDim})
            _priorLogVar = parameters.RequireTensor("priorLogVar", New Integer() {components, latentDim})
            _priorLogits = parameters.RequireTensor("priorLogits", New Integer() {components})

            PriorMode = CType(CInt(parameters.GetValue("priorMode", 0)), GmvaePriorMode)
            OutputLikelihood = CType(CInt(parameters.GetValue("likelihood", 0)), GmvaeLikelihood)

            _optimizers.Clear()
            _rng = If(_seed.HasValue, New RandomGaussian(_seed.Value), New RandomGaussian())
            _fitted = True

            Return True
        Catch ex As Exception
            errorMessage = ex.Message
            Return False
        End Try
    End Function

#End Region

#Region "辅助"

    Private Sub EnsureFitted()
        If Not _fitted Then
            Throw New InvalidOperationException("模型尚未训练, 请先调用 Fit 方法")
        End If
    End Sub

    Private Sub ValidateInput(x As Tensor)
        If x Is Nothing Then
            Throw New ArgumentNullException(NameOf(x))
        End If

        If x.Rank <> 2 Then
            Throw New ArgumentException($"需要二维张量 [batch, {_inputDim}]，实际秩为 {x.Rank}")
        End If

        If x.Shape(1) <> _inputDim Then
            Throw New ArgumentException($"输入维度不符: 期望 {_inputDim}, 实际 {x.Shape(1)}")
        End If
    End Sub

    Public Overrides Function ToString() As String
        Return $"GMVAE(inputDim={_inputDim}, latentDim={_latentDim}, K={_nComponents}, hiddenDim={_hiddenDim})"
    End Function

#End Region

End Class
