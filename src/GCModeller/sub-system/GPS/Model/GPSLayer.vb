Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.MachineLearning.Transformer
Imports SMRUCC.genomics.Analysis.GEARS.Layers
Imports SMRUCC.genomics.Analysis.GPS.Graph
Imports GEARSGraph = SMRUCC.genomics.Analysis.GEARS.Graph
Imports GNN = Microsoft.VisualBasic.DeepLearning.GNN

Namespace Model

    ''' <summary>
    ''' GPS 并行融合层（method.md §3.2：GPS 骨架 + Graphormer 偏置）。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 单层计算流程：
    ''' <code>
    ''' h_local  = MPNN(H)                    ' 沿先验调控入边的边类型感知消息传递
    ''' h_global = BiasedMHA(H, SPD)          ' 带空间偏置的全局自注意力
    ''' H1       = LayerNorm(h_local + h_global)
    ''' H2       = H1 + FFN(H1)               ' Linear → ReLU → Linear
    ''' H_out    = LayerNorm(H2)
    ''' </code>
    ''' 与 <c>Transformer.EncoderLayer</c> 的区别是两条支路<b>并行后相加</b>而不是串联：
    ''' 串联会先经全局注意力把局部结构平滑掉（GPS 论文的核心论点），
    ''' 因此这里不复用 <c>EncoderLayer</c>。
    ''' </para>
    ''' <para>
    ''' MPNN 支路直接复用 <c>GEARS.GEARSConvLayer</c>（稀疏 O(|E|·d)、带手写反向），
    ''' 其权重梯度张量由该层自己累加，通过 <see cref="ParameterStore.RegisterExternal"/>
    ''' 借用统一的 Adam 与梯度裁剪，无需重写图卷积。
    ''' </para>
    ''' <para>
    ''' 精度边界：GEMM 走 <see cref="TensorOps"/>（内部已接入
    ''' <c>MixedPrecisionOps</c>，规模达标时临时降单精度）；
    ''' LayerNorm 统计量、softmax 与全部参数保持双精度。
    ''' </para>
    ''' </remarks>
    Public Class GPSLayer

        Private ReadOnly dModel As Integer
        Private ReadOnly ffnDim As Integer
        Private ReadOnly useMpnn As Boolean
        Private ReadOnly useAttn As Boolean
        Private ReadOnly dropoutRate As Double
        Private ReadOnly rand As Random

        Private ReadOnly mpnnLayer As GEARSConvLayer
        Private ReadOnly attn As BiasedMultiHeadAttention

        Private ReadOnly W1 As Tensor
        Private ReadOnly b1 As Tensor
        Private ReadOnly W2 As Tensor
        Private ReadOnly b2 As Tensor

        Private ReadOnly w1Entry As ParameterEntry
        Private ReadOnly b1Entry As ParameterEntry
        Private ReadOnly w2Entry As ParameterEntry
        Private ReadOnly b2Entry As ParameterEntry

        ' 特征维 dropout 掩码（True = 丢弃），长度 = dModel
        Private ReadOnly maskLocal As Boolean()
        Private ReadOnly maskGlobal As Boolean()
        Private ReadOnly maskFfn As Boolean()

        ''' <summary>
        ''' 一次前向传播的中间量，反向传播时需要全部回读。
        ''' </summary>
        Public Class Cache
            ''' <summary>本层输入 [N, d]</summary>
            Public Input As Tensor
            ''' <summary>MPNN 支路输出（未 dropout）</summary>
            Public Local As Tensor
            ''' <summary>注意力支路输出（未 dropout）</summary>
            Public GlobalBranch As Tensor
            ''' <summary>MPNN 支路输出（dropout 后）</summary>
            Public LocalDropped As Tensor
            ''' <summary>注意力支路输出（dropout 后）</summary>
            Public GlobalDropped As Tensor
            ''' <summary>两支路相加的结果 <c>local + global</c>（未含输入残差）</summary>
            Public Merged As Tensor
            ''' <summary>第一次 AddNorm 的输出</summary>
            Public Normalized1 As Tensor
            ''' <summary>第一次 LayerNorm 的均值</summary>
            Public Norm1Mean As Double()
            ''' <summary>第一次 LayerNorm 的逆标准差</summary>
            Public Norm1InvStd As Double()
            ''' <summary>FFN 第一层激活前的值</summary>
            Public PreActivation As Tensor
            ''' <summary>FFN 第一层激活后的值</summary>
            Public Activation As Tensor
            ''' <summary>FFN 输出（未 dropout）</summary>
            Public FeedForward As Tensor
            ''' <summary>FFN 输出（dropout 后）</summary>
            Public FeedForwardDropped As Tensor
            ''' <summary>第二次 LayerNorm 的均值</summary>
            Public Norm2Mean As Double()
            ''' <summary>第二次 LayerNorm 的逆标准差</summary>
            Public Norm2InvStd As Double()
            ''' <summary>本次前向是否应用了 dropout</summary>
            Public DropoutApplied As Boolean
        End Class

        Private _lastCache As Cache

        ''' <summary>最近一次前向传播的缓存</summary>
        Public ReadOnly Property LastCache As Cache
            Get
                Return _lastCache
            End Get
        End Property

        ''' <summary>注意力支路（被禁用时为 Nothing）</summary>
        Public ReadOnly Property Attention As BiasedMultiHeadAttention
            Get
                Return attn
            End Get
        End Property

        ''' <summary>消息传递支路（被禁用时为 Nothing）</summary>
        Public ReadOnly Property Mpnn As GEARSConvLayer
            Get
                Return mpnnLayer
            End Get
        End Property

        ''' <summary>
        ''' 创建一个 GPS 融合层。
        ''' </summary>
        ''' <param name="dModel">隐藏维度</param>
        ''' <param name="ffnDim">FFN 中间维度</param>
        ''' <param name="nHeads">注意力头数（需整除 dModel）</param>
        ''' <param name="spdBucketCount">SPD 桶总数</param>
        ''' <param name="store">参数登记表</param>
        ''' <param name="useMpnn">是否启用局部消息传递支路（消融开关）</param>
        ''' <param name="useAttn">是否启用全局注意力支路（消融开关）</param>
        ''' <param name="dropout">特征 dropout 比例</param>
        ''' <param name="seed">dropout 随机种子</param>
        ''' <param name="name">层名称（参数名前缀）</param>
        Public Sub New(dModel As Integer,
                       ffnDim As Integer,
                       nHeads As Integer,
                       spdBucketCount As Integer,
                       store As ParameterStore,
                       Optional useMpnn As Boolean = True,
                       Optional useAttn As Boolean = True,
                       Optional dropout As Double = 0.0,
                       Optional seed As Integer? = Nothing,
                       Optional name As String = "gps")

            If store Is Nothing Then Throw New ArgumentNullException(NameOf(store))
            If dModel <= 0 Then Throw New ArgumentException("隐藏维度必须为正", NameOf(dModel))
            If nHeads <= 0 OrElse dModel Mod nHeads <> 0 Then
                Throw New ArgumentException($"头数 {nHeads} 必须整除隐藏维度 {dModel}", NameOf(nHeads))
            End If

            Me.dModel = dModel
            Me.ffnDim = ffnDim
            Me.useMpnn = useMpnn
            Me.useAttn = useAttn
            Me.dropoutRate = System.Math.Max(0.0, System.Math.Min(0.999, dropout))
            Me.rand = New Random(If(seed.HasValue, seed.Value, Environment.TickCount))

            ' ---- 局部消息传递支路 ----
            If useMpnn Then
                ' 关闭层内激活：GNN.ActivationFunctions 的 Tanh 是 Single 精度，
                ' 会把前向/反向都拖到 float32。这里让图卷积只做线性变换，
                ' 由本层在双精度域应用 tanh（见 Forward/Backward）。
                mpnnLayer = New GEARSConvLayer(
                    inFeatures:=dModel,
                    outFeatures:=dModel,
                    activation:=GNN.ActivationType.None,
                    usePerRelationTransform:=False,
                    useDense:=False,
                    seed:=seed,
                    name:=$"{name}.mpnn")

                ' 复用该层自己的梯度累加器：GEARSConvLayer 的 GetParameters 顺序为
                ' {wSelf, bias, relW(0)}，GetGradients 与之逐一对应
                Dim mp As List(Of Tensor) = mpnnLayer.GetParameters()
                Dim mg As List(Of Tensor) = mpnnLayer.GetGradients()

                Call store.RegisterExternal($"{name}.mpnn.wSelf", mp(0), mg(0))
                Call store.RegisterExternal($"{name}.mpnn.bias", mp(1), mg(1))
                Call store.RegisterExternal($"{name}.mpnn.wRel", mp(2), mg(2))
            End If

            ' ---- 全局注意力支路 ----
            If useAttn Then
                Dim dk As Integer = dModel \ nHeads

                attn = New BiasedMultiHeadAttention(
                    dk:=dk,
                    dv:=dk,
                    nrHeads:=nHeads,
                    embeddingSize:=dModel,
                    spdBucketCount:=spdBucketCount,
                    store:=store,
                    name:=$"{name}.attn")
            End If

            ' ---- 前馈网络 ----
            W1 = TensorOps.HeNormalInit(New Integer() {dModel, ffnDim})
            b1 = New Tensor(ffnDim)
            W2 = TensorOps.HeNormalInit(New Integer() {ffnDim, dModel})
            b2 = New Tensor(dModel)

            w1Entry = store.Register($"{name}.ffn.w1", W1)
            b1Entry = store.Register($"{name}.ffn.b1", b1)
            w2Entry = store.Register($"{name}.ffn.w2", W2)
            b2Entry = store.Register($"{name}.ffn.b2", b2)

            maskLocal = New Boolean(dModel - 1) {}
            maskGlobal = New Boolean(dModel - 1) {}
            maskFfn = New Boolean(dModel - 1) {}
        End Sub

        ''' <summary>
        ''' 前向传播：局部消息传递与全局注意力并行后相加，再经 FFN 与残差/归一化。
        ''' </summary>
        ''' <param name="input">节点嵌入 [N, d]</param>
        ''' <param name="graphData">基因调控图（MPNN 支路使用）</param>
        ''' <param name="spdBuckets">SPD 桶索引矩阵 [N, N]（注意力支路使用）</param>
        ''' <param name="isTraining">是否处于训练模式（决定是否应用 dropout）</param>
        ''' <returns>更新后的节点嵌入 [N, d]</returns>
        Public Function Forward(input As Tensor,
                                graphData As GEARSGraph.GeneRegulatoryGraph,
                                spdBuckets As Integer(,),
                                isTraining As Boolean) As Tensor

            Return ForwardCore(input, graphData, spdBuckets, Nothing, isTraining)
        End Function

        ''' <summary>
        ''' 前向传播（稀疏注意力）：注意力支路只在每基因的候选集上计算。
        ''' </summary>
        ''' <param name="input">节点嵌入 [N, d]</param>
        ''' <param name="graphData">基因调控图（MPNN 支路使用）</param>
        ''' <param name="pattern">稀疏候选模式（注意力支路使用）</param>
        ''' <param name="isTraining">是否处于训练模式（决定是否应用 dropout）</param>
        ''' <returns>更新后的节点嵌入 [N, d]</returns>
        Public Function Forward(input As Tensor,
                                graphData As GEARSGraph.GeneRegulatoryGraph,
                                pattern As SparseAttentionPattern,
                                isTraining As Boolean) As Tensor

            Return ForwardCore(input, graphData, Nothing, pattern, isTraining)
        End Function

        ''' <summary>前向传播的公共实现：按传入的桶矩阵 / 候选模式选择稠密或稀疏注意力。</summary>
        ''' <param name="input">节点嵌入 [N, d]</param>
        ''' <param name="graphData">基因调控图（MPNN 支路使用）</param>
        ''' <param name="spdBuckets">稠密 SPD 桶矩阵；稀疏模式下传 Nothing</param>
        ''' <param name="pattern">稀疏候选模式；稠密模式下传 Nothing</param>
        ''' <param name="isTraining">是否处于训练模式（决定是否应用 dropout）</param>
        ''' <returns>更新后的节点嵌入 [N, d]</returns>
        Private Function ForwardCore(input As Tensor,
                                     graphData As GEARSGraph.GeneRegulatoryGraph,
                                     spdBuckets As Integer(,),
                                     pattern As SparseAttentionPattern,
                                     isTraining As Boolean) As Tensor

            If input Is Nothing Then Throw New ArgumentNullException(NameOf(input))

            Dim dropoutApplied As Boolean = isTraining AndAlso dropoutRate > 0.0

            ' ---- 支路 1：局部消息传递（线性聚合后在本层做双精度 tanh） ----
            Dim local As Tensor = If(useMpnn, Tensor.computeKernel.Tanh(mpnnLayer.Forward(input, graphData)), input)

            ' ---- 支路 2：全局注意力（显式类型化，避免重载歧义） ----
            Dim global_ As Tensor

            If useAttn Then
                If pattern Is Nothing Then
                    global_ = attn.Forward(input, spdBuckets)
                Else
                    global_ = attn.Forward(input, pattern)
                End If
            Else
                global_ = input
            End If

            Dim localDropped As Tensor = local
            Dim globalDropped As Tensor = global_

            If dropoutApplied Then
                Call FillMask(maskLocal)
                Call FillMask(maskGlobal)
                localDropped = TensorOps.DropoutMask(local, maskLocal, dropoutRate)
                globalDropped = TensorOps.DropoutMask(global_, maskGlobal, dropoutRate)
            End If

            ' ---- 并行融合 + 残差/归一化 ----
            ' method.md §4.2 的伪代码是 h_local = h_local + H; LayerNorm_1(h_local)，
            ' 即残差必须带上本层输入 H。原实现只做 LayerNorm(local + global)，
            ' 把输入残差丢掉了：每层的 LayerNorm 都会以"全局主导分量"为尺度重新归一化，
            ' 单基因扰动这类小信号经过叠加后会在 1e-4 量级被完全抹平。
            Dim merged As Tensor = Tensor.computeKernel.Add(localDropped, globalDropped)
            Dim mean1 As Double() = Nothing
            Dim invStd1 As Double() = Nothing
            Dim normalized1 As Tensor = TensorOps.AddNormForward(input, merged, mean1, invStd1)

            ' ---- FFN：Linear → ReLU → Linear ----
            Dim preActivation As Tensor = TensorOps.VecAdd(TensorOps.BatchedMatMul(normalized1, W1), b1)
            Dim activation As Tensor = Tensor.computeKernel.Relu(preActivation)
            Dim feedForward As Tensor = TensorOps.VecAdd(TensorOps.BatchedMatMul(activation, W2), b2)
            Dim feedForwardDropped As Tensor = feedForward

            If dropoutApplied Then
                Call FillMask(maskFfn)
                feedForwardDropped = TensorOps.DropoutMask(feedForward, maskFfn, dropoutRate)
            End If

            Dim mean2 As Double() = Nothing
            Dim invStd2 As Double() = Nothing
            Dim output As Tensor = TensorOps.AddNormForward(normalized1, feedForwardDropped, mean2, invStd2)

            _lastCache = New Cache With {
                .Input = input,
                .Local = local,
                .GlobalBranch = global_,
                .LocalDropped = localDropped,
                .GlobalDropped = globalDropped,
                .Merged = merged,
                .Normalized1 = normalized1,
                .Norm1Mean = mean1,
                .Norm1InvStd = invStd1,
                .PreActivation = preActivation,
                .Activation = activation,
                .FeedForward = feedForward,
                .FeedForwardDropped = feedForwardDropped,
                .Norm2Mean = mean2,
                .Norm2InvStd = invStd2,
                .DropoutApplied = dropoutApplied
            }

            Return output
        End Function

        ''' <summary>
        ''' 反向传播：按前向的逆序回传，累积本层全部参数梯度并返回输入梯度。
        ''' </summary>
        ''' <param name="dOut">对输出的梯度 [N, d]</param>
        ''' <param name="graphData">基因调控图（MPNN 支路使用）</param>
        ''' <returns>对输入的梯度 [N, d]</returns>
        Public Function Backward(dOut As Tensor, graphData As GEARSGraph.GeneRegulatoryGraph) As Tensor
            Dim cache As Cache = _lastCache

            If cache Is Nothing Then
                Throw New InvalidOperationException("必须先执行前向传播才能反向传播")
            End If

            ' ---- 第二次 AddNorm：out = LayerNorm(normalized1 + ffDropped) ----
            Dim dX As Tensor = TensorOps.AddNormBackward(
                dOut, cache.Normalized1, cache.FeedForwardDropped, cache.Norm2Mean, cache.Norm2InvStd)

            ' 加法对两个分支对称：残差支路直接使用 dX，FFN 支路需要独立副本
            Dim dNormed As Tensor = dX
            Dim dFfDropped As Tensor = TensorOps.CloneTensor(dX)

            If cache.DropoutApplied Then
                dFfDropped = TensorOps.DropoutMaskBackward(dFfDropped, maskFfn, dropoutRate)
            End If

            ' ---- FFN 反向：ff = activation · W2 + b2 ----
            Dim dActivation As Tensor = Nothing
            Dim dW2 As Tensor = Nothing

            Call TensorOps.BatchedMatMulBackward(dFfDropped, cache.Activation, W2, dActivation, dW2)
            Call TensorOps.Accumulate(w2Entry.Gradient, dW2)
            Call TensorOps.Accumulate(b2Entry.Gradient, TensorOps.VecAddBackward(dFfDropped))

            ' ReLU 反向：用预激活值的阶跃掩码门控
            Dim dPreActivation As Tensor = TensorOps.ElementwiseMultiply(
                dActivation, TensorOps.Heaviside(cache.PreActivation))

            Call TensorOps.Accumulate(b1Entry.Gradient, TensorOps.VecAddBackward(dPreActivation))

            Dim dNormedFromFfn As Tensor = Nothing
            Dim dW1 As Tensor = Nothing

            Call TensorOps.BatchedMatMulBackward(dPreActivation, cache.Normalized1, W1, dNormedFromFfn, dW1)
            Call TensorOps.Accumulate(w1Entry.Gradient, dW1)

            ' ---- 第一次 AddNorm：normalized1 = LayerNorm(input + (local + global)) ----
            Dim dNormedTotal As Tensor = dNormed + dNormedFromFfn
            Dim dMergedTotal As Tensor = TensorOps.AddNormBackward(
                dNormedTotal, cache.Input, cache.Merged, cache.Norm1Mean, cache.Norm1InvStd)

            ' 加法处对称：输入残差支路与两支路之和各自拿到同一份梯度
            Dim dResidual As Tensor = TensorOps.CloneTensor(dMergedTotal)
            Dim dLocal As Tensor = TensorOps.CloneTensor(dMergedTotal)
            Dim dGlobal As Tensor = TensorOps.CloneTensor(dMergedTotal)

            If cache.DropoutApplied Then
                dLocal = TensorOps.DropoutMaskBackward(dLocal, maskLocal, dropoutRate)
                dGlobal = TensorOps.DropoutMaskBackward(dGlobal, maskGlobal, dropoutRate)
            End If

            ' ---- 两条支路分别回传 ----
            Dim dInput As Tensor = Nothing

            If useMpnn Then
                ' tanh 反向：d/dz = 1 − tanh(z)²，其中 tanh(z) 即前向缓存的 Local
                Dim tanhDerivative As Tensor = Tensor.computeKernel.AddScalar(
                    Tensor.computeKernel.Negate(Tensor.computeKernel.Square(cache.Local)), 1.0)

                dInput = mpnnLayer.Backward(TensorOps.ElementwiseMultiply(dLocal, tanhDerivative), graphData)
            End If

            If useAttn Then
                Dim dAttention As Tensor = attn.Backward(dGlobal)

                If dInput Is Nothing Then
                    dInput = dAttention
                Else
                    Call TensorOps.Accumulate(dInput, dAttention)
                End If
            End If

            ' 两条支路都被关闭时（纯 FFN 消融），梯度直接沿输入返回
            If dInput Is Nothing Then dInput = dLocal

            ' 叠加输入残差支路的梯度
            Call TensorOps.Accumulate(dInput, dResidual)

            Return dInput
        End Function

        ''' <summary>
        ''' 重新采样特征 dropout 掩码。
        ''' </summary>
        ''' <param name="mask">长度为 dModel 的布尔掩码（True = 丢弃）</param>
        Private Sub FillMask(mask As Boolean())
            For i As Integer = 0 To mask.Length - 1
                mask(i) = rand.NextDouble() < dropoutRate
            Next
        End Sub

    End Class

End Namespace
