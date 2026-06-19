' ============================================================================
' IMMO Framework - Core Model
' ============================================================================
' IMMO (Integration Model for Incomplete Multi-Omics)
' 基于动态掩码机制的多组学数据整合模型
' 
' 核心组件:
'   1. IMMOConfig        - 超参数配置
'   2. OmicsData         - 单组学数据容器
'   3. PreparedData      - 预处理后的多组学数据
'   4. IMMOModel         - IMMO模型（多分支编码器+共享解码器+融合）
'   5. IMMOTrainer       - 训练器（动态掩码+WMSE损失+早停）
'   6. DataPrep          - 数据预处理（样本对齐+归一化）
' ============================================================================

Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

Namespace IMMO

    ' ========================================================================
    ''' <summary>
    ''' IMMO模型超参数配置
    ''' 默认值参考IMMO论文中的贝叶斯优化结果
    ''' </summary>
    Public Class IMMOConfig

        ' === 架构参数 ===

        ''' <summary>共享潜在空间维度</summary>
        Public Property LatentDim As Integer = 110

        ''' <summary>编码器隐藏层维度（从输入到潜在空间）</summary>
        Public Property EncoderHiddenDims As Integer() = {256, 128}

        ''' <summary>解码器隐藏层维度（从潜在空间到输出）</summary>
        Public Property DecoderHiddenDims As Integer() = {128, 256}

        ' === 训练参数 ===

        ''' <summary>最大训练轮数</summary>
        Public Property MaxEpochs As Integer = 200

        ''' <summary>初始学习率</summary>
        Public Property LearningRate As Double = 0.00183

        ''' <summary>Adam beta1</summary>
        Public Property Beta1 As Double = 0.9

        ''' <summary>Adam beta2</summary>
        Public Property Beta2 As Double = 0.999

        ''' <summary>Adam epsilon</summary>
        Public Property AdamEpsilon As Double = 1E-8

        ''' <summary>学习率衰减率</summary>
        Public Property DecayRate As Double = 0.99

        ''' <summary>梯度裁剪范数（0表示不裁剪）</summary>
        Public Property GradientClipNorm As Double = 5.0

        ' === 动态掩码参数 ===

        ''' <summary>初始保留概率 P_0</summary>
        Public Property InitialP As Double = 0.75

        ''' <summary>最大保留概率 P_max</summary>
        Public Property Pmax As Double = 0.99

        ''' <summary>最小保留概率 P_min（用于公式下界保护）</summary>
        Public Property Pmin As Double = 0.01

        ' === 正则化参数 ===

        ''' <summary>Dropout丢弃率</summary>
        Public Property DropoutRate As Double = 0.47329

        ' === 损失函数参数 ===

        ''' <summary>各组学的损失权重（需与组学数量一致）</summary>
        Public Property LossWeights As Double() = {0.5, 0.5}

        ' === 早停参数 ===

        ''' <summary>早停耐心值（连续多少轮无改善则停止）</summary>
        Public Property Patience As Integer = 20

        ''' <summary>早停最小改善阈值</summary>
        Public Property MinDelta As Double = 1E-6

        ' === BatchNorm参数 ===

        ''' <summary>BatchNorm动量</summary>
        Public Property BNMomentum As Double = 0.9

        ''' <summary>BatchNorm epsilon</summary>
        Public Property BNEpsilon As Double = 1E-5

        ' === 其他 ===

        ''' <summary>随机种子（Nothing表示不固定）</summary>
        Public Property Seed As Integer? = 42

        ''' <summary>是否打印训练日志</summary>
        Public Property Verbose As Boolean = True

        ''' <summary>日志打印间隔（每N轮打印一次）</summary>
        Public Property LogInterval As Integer = 1

    End Class

    ' ========================================================================
    ''' <summary>
    ''' 单组学数据容器
    ''' </summary>
    Public Class OmicsData

        ''' <summary>数据矩阵 [样本数, 特征数]</summary>
        Public Property Data As Tensor

        ''' <summary>真实缺失掩码 [样本数, 特征数]（1=有数据, 0=缺失）</summary>
        Public Property Mask As Tensor

        ''' <summary>样本ID列表</summary>
        Public Property SampleIDs As String()

        ''' <summary>特征名列表（可选）</summary>
        Public Property FeatureNames As String()

        ''' <summary>组学名称</summary>
        Public Property Name As String

        ''' <summary>归一化用的特征均值（用于反归一化）</summary>
        Public Property FeatureMean As Double()

        ''' <summary>归一化用的特征标准差</summary>
        Public Property FeatureStd As Double()

        Public ReadOnly Property NumSamples As Integer
            Get
                Return Data.Shape(0)
            End Get
        End Property

        Public ReadOnly Property NumFeatures As Integer
            Get
                Return Data.Shape(1)
            End Get
        End Property

    End Class

    ' ========================================================================
    ''' <summary>
    ''' 预处理后的多组学数据（统一样本集）
    ''' </summary>
    Public Class PreparedData

        ''' <summary>所有组学数据列表（已对齐到统一样本集）</summary>
        Public Property OmicsList As New List(Of OmicsData)

        ''' <summary>统一样本ID列表</summary>
        Public Property UnifiedSampleIDs As String()

        ''' <summary>总样本数</summary>
        Public ReadOnly Property NumSamples As Integer
            Get
                Return UnifiedSampleIDs.Length
            End Get
        End Property

        ''' <summary>组学数量</summary>
        Public ReadOnly Property NumOmics As Integer
            Get
                Return OmicsList.Count
            End Get
        End Property

    End Class

    ' ========================================================================
    ''' <summary>
    ''' IMMO模型
    ''' 架构: 多分支独立编码器 → 共享潜在空间融合 → 共享解码器联合重构
    ''' </summary>
    Public Class IMMOModel

        Private _encoders As New List(Of Sequential)
        Private _decoder As Sequential
        Private _config As IMMOConfig
        Private _featureDims As Integer()
        Private _latentDim As Integer

        ' 前向传播缓存（用于反向传播）
        Private _cachedLatents As List(Of Tensor)
        Private _cachedFusionWeights As List(Of Tensor)
        Private _cachedFusionSumW As Tensor
        Private _cachedFusedLatent As Tensor
        Private _cachedReconstruction As Tensor

        ''' <summary>所有可训练参数</summary>
        Private _allParameters As List(Of Parameter)

        Public ReadOnly Property NumOmics As Integer

        Public Sub New(featureDims As Integer(), config As IMMOConfig)
            _config = config
            _featureDims = featureDims
            _latentDim = config.LatentDim
            Me.NumOmics = featureDims.Length

            ' 构建每个组学的独立编码器
            For i = 0 To featureDims.Length - 1
                Dim encoder = BuildEncoder(featureDims(i), config)
                _encoders.Add(encoder)
            Next

            ' 构建共享解码器（输出维度 = 所有组学特征维度之和）
            Dim totalOutputDim = 0
            For Each d In featureDims
                totalOutputDim += d
            Next
            _decoder = BuildDecoder(config.LatentDim, totalOutputDim, config)

            ' 收集所有参数
            _allParameters = New List(Of Parameter)
            For Each enc In _encoders
                _allParameters.AddRange(enc.GetParameters())
            Next
            _allParameters.AddRange(_decoder.GetParameters())
        End Sub

        ''' <summary>
        ''' 构建编码器
        ''' 结构: Dense → BN → Swish → Dropout → ... → Dense(投影到潜在空间)
        ''' </summary>
        Private Function BuildEncoder(inputDim As Integer, config As IMMOConfig) As Sequential
            Dim enc As New Sequential()
            Dim hiddenDims = config.EncoderHiddenDims
            Dim seedOffset = 0

            ' 第一层: inputDim → hiddenDims[0]
            enc.Add(New DenseLayer(inputDim, hiddenDims(0), If(config.Seed.HasValue, config.Seed.Value + seedOffset, Nothing)))
            seedOffset += 1
            enc.Add(New BatchNormLayer(hiddenDims(0), config.BNMomentum, config.BNEpsilon))
            enc.Add(New SwishActivation())
            enc.Add(New DropoutLayer(config.DropoutRate, If(config.Seed.HasValue, config.Seed.Value + seedOffset, Nothing)))
            seedOffset += 1

            ' 中间隐藏层
            For i = 1 To hiddenDims.Length - 1
                enc.Add(New DenseLayer(hiddenDims(i - 1), hiddenDims(i), If(config.Seed.HasValue, config.Seed.Value + seedOffset, Nothing)))
                seedOffset += 1
                enc.Add(New BatchNormLayer(hiddenDims(i), config.BNMomentum, config.BNEpsilon))
                enc.Add(New SwishActivation())
                enc.Add(New DropoutLayer(config.DropoutRate, If(config.Seed.HasValue, config.Seed.Value + seedOffset, Nothing)))
                seedOffset += 1
            Next

            ' 投影层: 最后一层隐藏 → latent_dim（无激活函数）
            enc.Add(New DenseLayer(hiddenDims(hiddenDims.Length - 1), config.LatentDim, If(config.Seed.HasValue, config.Seed.Value + seedOffset, Nothing)))

            Return enc
        End Function

        ''' <summary>
        ''' 构建共享解码器
        ''' 结构: Dense → BN → Swish → Dropout → ... → Dense(线性输出)
        ''' </summary>
        Private Function BuildDecoder(latentDim As Integer, outputDim As Integer, config As IMMOConfig) As Sequential
            Dim dec As New Sequential()
            Dim hiddenDims = config.DecoderHiddenDims
            Dim seedOffset = 1000

            ' 第一层: latent_dim → hiddenDims[0]
            dec.Add(New DenseLayer(latentDim, hiddenDims(0), If(config.Seed.HasValue, config.Seed.Value + seedOffset, Nothing)))
            seedOffset += 1
            dec.Add(New BatchNormLayer(hiddenDims(0), config.BNMomentum, config.BNEpsilon))
            dec.Add(New SwishActivation())
            dec.Add(New DropoutLayer(config.DropoutRate, If(config.Seed.HasValue, config.Seed.Value + seedOffset, Nothing)))
            seedOffset += 1

            ' 中间隐藏层
            For i = 1 To hiddenDims.Length - 1
                dec.Add(New DenseLayer(hiddenDims(i - 1), hiddenDims(i), If(config.Seed.HasValue, config.Seed.Value + seedOffset, Nothing)))
                seedOffset += 1
                dec.Add(New BatchNormLayer(hiddenDims(i), config.BNMomentum, config.BNEpsilon))
                dec.Add(New SwishActivation())
                dec.Add(New DropoutLayer(config.DropoutRate, If(config.Seed.HasValue, config.Seed.Value + seedOffset, Nothing)))
                seedOffset += 1
            Next

            ' 输出层: 最后一层隐藏 → outputDim（线性激活）
            dec.Add(New DenseLayer(hiddenDims(hiddenDims.Length - 1), outputDim, If(config.Seed.HasValue, config.Seed.Value + seedOffset, Nothing)))
            dec.Add(New LinearActivation())

            Return dec
        End Function

        ''' <summary>
        ''' 前向传播
        ''' </summary>
        ''' <param name="omicsData">各组学数据列表</param>
        ''' <param name="totalMasks">各组学的总掩码（真实缺失⊗动态掩码）</param>
        ''' <param name="training">是否训练模式</param>
        ''' <returns>各组学的重构结果列表</returns>
        Public Function Forward(omicsData As List(Of OmicsData),
                                totalMasks As List(Of Tensor),
                                training As Boolean) As List(Of Tensor)

            ' === 1. 多分支独立编码 ===
            _cachedLatents = New List(Of Tensor)
            For i = 0 To NumOmics - 1
                Dim z = _encoders(i).Forward(omicsData(i).Data, training)
                _cachedLatents.Add(z)
            Next

            ' === 2. 计算融合权重（基于掩码覆盖率） ===
            _cachedFusionWeights = New List(Of Tensor)
            For i = 0 To NumOmics - 1
                ' w_i = mean(M_total_i, axis=1) → [batch, 1]
                ' 每个样本在该组学中的观测特征比例
                Dim w = TensorHelpers.MeanAxis1(totalMasks(i))
                _cachedFusionWeights.Add(w)
            Next

            ' === 3. 共享潜在空间融合（加权平均） ===
            ' z = Σ(w_i * z_i) / (Σ w_i + eps)
            Dim batchSize = omicsData(0).NumSamples
            Dim numerator = New Tensor(batchSize, _latentDim)
            For i = 0 To NumOmics - 1
                Dim weighted = TensorHelpers.MultiplyByColumn(_cachedLatents(i), _cachedFusionWeights(i))
                For j = 0 To numerator.Length - 1
                    numerator(j) += weighted(j)
                Next
            Next

            ' 计算权重和 Σ w_i → [batch, 1]
            _cachedFusionSumW = New Tensor(batchSize, 1)
            For i = 0 To NumOmics - 1
                For j = 0 To batchSize - 1
                    _cachedFusionSumW(j, 0) += _cachedFusionWeights(i)(j, 0)
                Next
            Next

            ' 融合: z = numerator / sumW
            _cachedFusedLatent = TensorHelpers.DivideByColumn(numerator, _cachedFusionSumW)

            ' === 4. 共享解码器联合重构 ===
            _cachedReconstruction = _decoder.Forward(_cachedFusedLatent, training)

            ' === 5. 按组学拆分重构结果 ===
            Dim reconstructions As New List(Of Tensor)
            Dim offset = 0
            For i = 0 To NumOmics - 1
                Dim [dim] = _featureDims(i)
                reconstructions.Add(TensorHelpers.ColumnSlice(_cachedReconstruction, offset, [dim]))
                offset += [dim]
            Next

            Return reconstructions
        End Function

        ''' <summary>
        ''' 反向传播
        ''' </summary>
        ''' <param name="gradReconstructions">各组学重构的梯度列表</param>
        Public Sub Backward(gradReconstructions As List(Of Tensor))
            ' === 1. 拼接重构梯度 ===
            Dim gradRecon = TensorHelpers.ConcatColumns(gradReconstructions.ToArray())

            ' === 2. 解码器反向传播 ===
            Dim gradFusedLatent = _decoder.Backward(gradRecon)

            ' === 3. 融合反向传播 ===
            ' z = Σ(w_i * z_i) / (Σ w_i + eps)
            ' dz/dz_i = w_i / (Σ w_i + eps)
            ' gradZ_i = gradFusedLatent * (w_i / sumW)
            Dim gradLatents As New List(Of Tensor)
            For i = 0 To NumOmics - 1
                Dim ratio = New Tensor(gradFusedLatent.Shape(0), 1)
                For j = 0 To gradFusedLatent.Shape(0) - 1
                    ratio(j, 0) = _cachedFusionWeights(i)(j, 0) / (_cachedFusionSumW(j, 0) + 1E-12)
                Next
                Dim gradZ_i = TensorHelpers.MultiplyByColumn(gradFusedLatent, ratio)
                gradLatents.Add(gradZ_i)
            Next

            ' === 4. 各编码器反向传播 ===
            For i = 0 To NumOmics - 1
                _encoders(i).Backward(gradLatents(i))
            Next
        End Sub

        ''' <summary>获取所有可训练参数</summary>
        Public Function GetParameters() As List(Of Parameter)
            Return _allParameters
        End Function

        ''' <summary>清零所有梯度</summary>
        Public Sub ZeroGrad()
            For Each enc In _encoders
                enc.ZeroGrad()
            Next
            _decoder.ZeroGrad()
        End Sub

        ''' <summary>
        ''' 获取潜在空间表示（推理模式）
        ''' </summary>
        Public Function GetLatentRepresentations(omicsData As List(Of OmicsData)) As Tensor
            Dim dummyMasks As New List(Of Tensor)
            For i = 0 To NumOmics - 1
                ' 推理时使用真实掩码（全1表示不额外掩码）
                dummyMasks.Add(Tensor.Ones(omicsData(i).Data.Shape))
            Next

            ' 前向传播（推理模式）
            Forward(omicsData, dummyMasks, training:=False)
            Return _cachedFusedLatent
        End Function

        ''' <summary>
        ''' 获取重构/插补后的数据（推理模式）
        ''' </summary>
        Public Function GetReconstructions(omicsData As List(Of OmicsData)) As List(Of Tensor)
            Dim dummyMasks As New List(Of Tensor)
            For i = 0 To NumOmics - 1
                dummyMasks.Add(Tensor.Ones(omicsData(i).Data.Shape))
            Next
            Return Forward(omicsData, dummyMasks, training:=False)
        End Function

    End Class

    ' ========================================================================
    ''' <summary>
    ''' IMMO训练器
    ''' 实现动态掩码生成、WMSE损失计算、训练循环、早停机制
    ''' </summary>
    Public Class IMMOTrainer

        Private _model As IMMOModel
        Private _optimizer As AdamOptimizer
        Private _config As IMMOConfig
        Private _rng As Random

        ' 训练历史
        Public Property LossHistory As New List(Of Double)

        Public Sub New(model As IMMOModel, config As IMMOConfig)
            _model = model
            _config = config
            _optimizer = New AdamOptimizer(config.LearningRate, config.Beta1, config.Beta2, config.AdamEpsilon)
            _rng = If(config.Seed.HasValue, New Random(config.Seed.Value + 9999), New Random())
        End Sub

        ''' <summary>
        ''' 计算动态保留概率 P(t)
        ''' P(t) = P_0 + (P_max - P_0) * (1 - decay_rate^t)
        ''' 训练初期保留率低 → 模拟高缺失场景
        ''' 训练后期保留率升高 → 暴露于更完整数据
        ''' </summary>
        Public Function ComputeRetentionProbability(epoch As Integer) As Double
            Dim p = _config.InitialP + (_config.Pmax - _config.InitialP) * (1.0 - std.Pow(_config.DecayRate, epoch))
            Return std.Min(p, _config.Pmax)
        End Function

        ''' <summary>
        ''' 生成动态掩码矩阵
        ''' M_dyn ~ Bernoulli(P(t))
        ''' </summary>
        Private Function GenerateDynamicMask(shape As Integer(), p As Double) As Tensor
            Dim mask = New Tensor(shape)
            For i = 0 To mask.Length - 1
                mask(i) = If(_rng.NextDouble() < p, 1.0, 0.0)
            Next
            Return mask
        End Function

        ''' <summary>
        ''' 计算总掩码: M_total = M_true ⊗ M_dyn
        ''' </summary>
        Private Function ComputeTotalMask(trueMask As Tensor, dynMask As Tensor) As Tensor
            Dim total = New Tensor(trueMask.Shape)
            For i = 0 To total.Length - 1
                total(i) = trueMask(i) * dynMask(i)
            Next
            Return total
        End Function

        ''' <summary>
        ''' 计算加权均方误差损失 (WMSE)
        ''' WMSE = (1/N) * Σ_i α_i * [Σ M_i * (X_i - X̂_i)²] / [Σ M_i]
        ''' </summary>
        Public Function ComputeWMSE(omicsData As List(Of OmicsData),
                                     reconstructions As List(Of Tensor),
                                     totalMasks As List(Of Tensor)) As Double

            Dim N = omicsData.Count
            Dim totalLoss = 0.0

            For i = 0 To N - 1
                Dim alpha = _config.LossWeights(i)
                Dim X = omicsData(i).Data
                Dim Xhat = reconstructions(i)
                Dim M = totalMasks(i)

                ' 计算掩码总和
                Dim maskSum = 0.0
                Dim weightedSqError = 0.0
                For j = 0 To X.Length - 1
                    maskSum += M(j)
                    Dim diff = X(j) - Xhat(j)
                    weightedSqError += M(j) * diff * diff
                Next

                If maskSum > 0 Then
                    totalLoss += alpha * weightedSqError / maskSum
                End If
            Next

            Return totalLoss / N
        End Function

        ''' <summary>
        ''' 计算WMSE损失对重构值的梯度
        ''' dL/dX̂_i = (1/N) * α_i * (-2) * M_i * (X_i - X̂_i) / ΣM_i
        ''' </summary>
        Private Function ComputeReconstructionGradients(omicsData As List(Of OmicsData),
                                                         reconstructions As List(Of Tensor),
                                                         totalMasks As List(Of Tensor)) As List(Of Tensor)

            Dim N = omicsData.Count
            Dim grads As New List(Of Tensor)

            For i = 0 To N - 1
                Dim alpha = _config.LossWeights(i)
                Dim X = omicsData(i).Data
                Dim Xhat = reconstructions(i)
                Dim M = totalMasks(i)

                ' 计算掩码总和
                Dim maskSum = 0.0
                For j = 0 To M.Length - 1
                    maskSum += M(j)
                Next

                Dim grad = New Tensor(X.Shape)
                If maskSum > 0 Then
                    Dim coeff = (1.0 / N) * alpha * (-2.0) / maskSum
                    For j = 0 To X.Length - 1
                        grad(j) = coeff * M(j) * (X(j) - Xhat(j))
                    Next
                End If
                grads.Add(grad)
            Next

            Return grads
        End Function

        ''' <summary>
        ''' 训练模型
        ''' </summary>
        ''' <param name="data">预处理后的多组学数据</param>
        ''' <returns>最终训练损失</returns>
        Public Function Train(data As PreparedData) As Double
            Dim omicsList = data.OmicsList
            Dim bestLoss = Double.MaxValue
            Dim patienceCounter = 0

            For epoch = 0 To _config.MaxEpochs - 1

                ' === 1. 计算动态保留概率 ===
                Dim p = ComputeRetentionProbability(epoch)

                ' === 2. 生成动态掩码并合成总掩码 ===
                Dim totalMasks As New List(Of Tensor)
                For i = 0 To omicsList.Count - 1
                    Dim dynMask = GenerateDynamicMask(omicsList(i).Data.Shape, p)
                    Dim totalMask = ComputeTotalMask(omicsList(i).Mask, dynMask)
                    totalMasks.Add(totalMask)
                Next

                ' === 3. 前向传播 ===
                _model.ZeroGrad()
                Dim reconstructions = _model.Forward(omicsList, totalMasks, training:=True)

                ' === 4. 计算损失 ===
                Dim loss = ComputeWMSE(omicsList, reconstructions, totalMasks)
                LossHistory.Add(loss)

                ' === 5. 反向传播 ===
                Dim gradRecons = ComputeReconstructionGradients(omicsList, reconstructions, totalMasks)
                _model.Backward(gradRecons)

                ' === 6. 参数更新（Adam + 梯度裁剪） ===
                _optimizer.Update(_model.GetParameters(), _config.GradientClipNorm)

                ' === 7. 学习率衰减 ===
                _optimizer.DecayLearningRate(_config.DecayRate)

                ' === 8. 早停检查 ===
                If loss < bestLoss - _config.MinDelta Then
                    bestLoss = loss
                    patienceCounter = 0
                Else
                    patienceCounter += 1
                End If

                ' === 9. 日志打印 ===
                If _config.Verbose AndAlso (epoch Mod _config.LogInterval = 0) Then
                    Console.WriteLine($"Epoch {epoch + 1,4}/{_config.MaxEpochs} | " &
                                      $"P(t)={p:F4} | " &
                                      $"Loss={loss:F6} | " &
                                      $"LR={_optimizer.LearningRate:F6} | " &
                                      $"Best={bestLoss:F6}")
                End If

                If patienceCounter >= _config.Patience Then
                    If _config.Verbose Then
                        Console.WriteLine($"Early stopping at epoch {epoch + 1} (patience={_config.Patience})")
                    End If
                    Exit For
                End If
            Next

            Return bestLoss
        End Function

    End Class

    ' ========================================================================
    ''' <summary>
    ''' 数据预处理模块
    ''' 处理样本不匹配问题：创建统一样本集、填充缺失样本、生成掩码、归一化
    ''' </summary>
    Public Module DataPrep

        ''' <summary>
        ''' 准备多组学数据（处理样本不匹配）
        ''' </summary>
        ''' <param name="matrices">各组学数据矩阵列表（每个为[样本数, 特征数]的2D数组）</param>
        ''' <param name="sampleIDLists">各组学对应的样本ID列表</param>
        ''' <param name="omicsNames">各组学名称</param>
        ''' <param name="normalize">是否进行Z-score归一化</param>
        ''' <returns>预处理后的数据（统一样本集）</returns>
        Public Function PrepareData(matrices As List(Of Double(,)),
                                     sampleIDLists As List(Of String()),
                                     omicsNames As String(),
                                     Optional normalize As Boolean = True) As PreparedData

            If matrices.Count <> sampleIDLists.Count OrElse matrices.Count <> omicsNames.Length Then
                Throw New ArgumentException("组学数据矩阵、样本ID列表和组学名称的数量必须一致")
            End If

            ' === 1. 创建统一样本集（并集） ===
            Dim allSampleIDs As New HashSet(Of String)
            For Each ids In sampleIDLists
                For Each id In ids
                    allSampleIDs.Add(id)
                Next
            Next
            Dim unifiedIDs = allSampleIDs.ToList()
            unifiedIDs.Sort()
            Dim N = unifiedIDs.Count

            ' === 2. 为每个组学创建填充矩阵和掩码 ===
            Dim result As New PreparedData()
            result.UnifiedSampleIDs = unifiedIDs.ToArray()

            For i = 0 To matrices.Count - 1
                Dim matrix = matrices(i)
                Dim ids = sampleIDLists(i)
                Dim ni = matrix.GetLength(0)
                Dim d = matrix.GetLength(1)

                ' 创建样本ID到原始行索引的映射
                Dim idToRow As New Dictionary(Of String, Integer)
                For j = 0 To ids.Length - 1
                    idToRow(ids(j)) = j
                Next

                ' 创建填充数据矩阵和掩码
                Dim paddedData = New Tensor(n, d)
                Dim mask = New Tensor(n, d)

                For j = 0 To n - 1
                    If idToRow.ContainsKey(unifiedIDs(j)) Then
                        ' 样本存在：填入真实数据，掩码为1
                        Dim srcRow = idToRow(unifiedIDs(j))
                        For k = 0 To d - 1
                            paddedData(j, k) = matrix(srcRow, k)
                            mask(j, k) = 1.0
                        Next
                    Else
                        ' 样本缺失：数据填0，掩码为0
                        For k = 0 To d - 1
                            paddedData(j, k) = 0.0
                            mask(j, k) = 0.0
                        Next
                    End If
                Next

                ' === 3. Z-score归一化（仅基于观测值） ===
                Dim featureMean As Double() = Nothing
                Dim featureStd As Double() = Nothing

                If normalize Then
                    featureMean = New Double(d - 1) {}
                    featureStd = New Double(d - 1) {}

                    For k = 0 To d - 1
                        Dim sum = 0.0
                        Dim count = 0
                        For j = 0 To n - 1
                            If mask(j, k) > 0 Then
                                sum += paddedData(j, k)
                                count += 1
                            End If
                        Next

                        If count > 0 Then
                            featureMean(k) = sum / count
                            ' 计算标准差
                            Dim sumSq = 0.0
                            For j = 0 To n - 1
                                If mask(j, k) > 0 Then
                                    Dim diff = paddedData(j, k) - featureMean(k)
                                    sumSq += diff * diff
                                End If
                            Next
                            featureStd(k) = std.Sqrt(sumSq / count)
                            If featureStd(k) < 1E-8 Then
                                featureStd(k) = 1.0
                            End If
                        Else
                            featureMean(k) = 0.0
                            featureStd(k) = 1.0
                        End If

                        ' 应用归一化
                        For j = 0 To n - 1
                            If mask(j, k) > 0 Then
                                paddedData(j, k) = (paddedData(j, k) - featureMean(k)) / featureStd(k)
                            End If
                        Next
                    Next
                End If

                ' 创建OmicsData
                Dim omics As New OmicsData() With {
                    .Data = paddedData,
                    .Mask = mask,
                    .SampleIDs = unifiedIDs.ToArray(),
                    .Name = omicsNames(i),
                    .FeatureMean = featureMean,
                    .FeatureStd = featureStd
                }
                Dim offset As Integer = i

                ' 生成默认特征名
                omics.FeatureNames = Enumerable.Range(0, d).Select(Function(k) $"{omicsNames(offset)}_Feature{k + 1}").ToArray()

                result.OmicsList.Add(omics)
            Next

            Return result
        End Function

        ''' <summary>
        ''' 反归一化（将模型输出还原为原始尺度）
        ''' </summary>
        Public Function Denormalize(normalizedData As Tensor, omics As OmicsData) As Tensor
            If omics.FeatureMean Is Nothing OrElse omics.FeatureStd Is Nothing Then
                Return normalizedData
            End If

            Dim result = New Tensor(normalizedData.Shape)
            Dim d = normalizedData.Shape(1)
            For i = 0 To normalizedData.Shape(0) - 1
                For j = 0 To d - 1
                    result(i, j) = normalizedData(i, j) * omics.FeatureStd(j) + omics.FeatureMean(j)
                Next
            Next
            Return result
        End Function

        ''' <summary>
        ''' 获取插补后的完整数据（用模型重构值填充缺失样本）
        ''' </summary>
        Public Function GetImputedData(model As IMMOModel, data As PreparedData) As List(Of Tensor)
            Dim reconstructions = model.GetReconstructions(data.OmicsList)
            Dim imputed As New List(Of Tensor)

            For i = 0 To data.NumOmics - 1
                Dim omics = data.OmicsList(i)
                Dim recon = reconstructions(i)

                ' 反归一化
                Dim reconOriginal = Denormalize(recon, omics)

                ' 合并：观测值用原始数据，缺失值用重构值
                Dim imputedData = New Tensor(omics.Data.Shape)
                For j = 0 To omics.Data.Length - 1
                    If omics.Mask(j) > 0 Then
                        ' 观测值：反归一化后的原始数据
                        imputedData(j) = omics.Data(j) * omics.FeatureStd(j Mod omics.NumFeatures) + omics.FeatureMean(j Mod omics.NumFeatures)
                    Else
                        ' 缺失值：用重构值（已反归一化）
                        imputedData(j) = reconOriginal(j)
                    End If
                Next

                imputed.Add(imputedData)
            Next

            Return imputed
        End Function

    End Module

End Namespace
