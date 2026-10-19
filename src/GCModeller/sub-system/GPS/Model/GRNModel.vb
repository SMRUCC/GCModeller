Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.MachineLearning.Transformer
Imports SMRUCC.genomics.Analysis.GPS.Config
Imports SMRUCC.genomics.Analysis.GPS.Graph
Imports GEARSGraph = SMRUCC.genomics.Analysis.GEARS.Graph

Namespace Model

    ''' <summary>
    ''' GNN + Transformer 融合的基因表达调控网络模型（method.md §4.3）。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 前向流程：
    ''' <list type="number">
    ''' <item><description>对窗口内每个时间步构建初始节点嵌入
    ''' <c>h⁰ = x·W_expr + e_gene + PE·W_pe + z_degIn + z_degOut</c>（Graphormer 中心性编码）；</description></item>
    ''' <item><description>逐时间步通过 L 层 <see cref="GPSLayer"/>（局部 MPNN ∥ 全局偏置注意力）；</description></item>
    ''' <item><description>把各时间步的基因表示堆叠成 <c>[w, N, d]</c>，交给 <see cref="BatchGRU"/> 沿时间编码；</description></item>
    ''' <item><description>用 <see cref="PerGeneDecoder"/> 把最终隐藏状态读出为下一时刻表达，可选输出不确定性。</description></item>
    ''' </list>
    ''' </para>
    ''' <para>
    ''' <b>梯度检查点</b>：GPS 层的反向依赖它自己的前向缓存，而同一组层在 w 个时间步上共享参数。
    ''' 为在"参数共享 + 单一缓存"的既有层实现上完成 BPTT，反向传播时按时间步<b>重算</b>一次
    ''' 前向（只保留 <c>[N, d]</c> 的最终表示），而不是缓存 w 套中间量。
    ''' 代价是多一次前向，收益是显存/内存与时间步数解耦。
    ''' 重算时固定 <c>isTraining:=False</c>（层内 dropout 关闭），
    ''' 模型级 dropout 只作用在 GRU 输出上，因此不会出现掩码不一致。
    ''' </para>
    ''' <para>
    ''' 混合精度边界：所有矩阵乘经 <see cref="TensorOps"/> / <see cref="MixedPrecisionOps"/>，
    ''' 规模达标时临时代码降单精度；参数、LayerNorm 统计量、GRU 门控与损失保持双精度。
    ''' </para>
    ''' </remarks>
    Public Class GRNModel

        Private ReadOnly cfg As GPSConfig
        Private ReadOnly regulatoryGraph As GEARSGraph.GeneRegulatoryGraph
        Private ReadOnly graphEncoding As GraphEncoding
        Private ReadOnly store As ParameterStore

        Private ReadOnly n As Integer
        Private ReadOnly d As Integer
        Private ReadOnly windowSize As Integer
        Private ReadOnly peDim As Integer
        Private ReadOnly dropoutRate As Double
        Private ReadOnly rand As Random

        ' 初始嵌入的可学习分量
        Private ReadOnly geneEmbedding As Tensor
        Private ReadOnly degInEmbedding As Tensor
        Private ReadOnly degOutEmbedding As Tensor
        Private ReadOnly peProj As Tensor
        Private ReadOnly exprProj As Tensor

        Private ReadOnly geneEntry As ParameterEntry
        Private ReadOnly degInEntry As ParameterEntry
        Private ReadOnly degOutEntry As ParameterEntry
        Private ReadOnly peEntry As ParameterEntry
        Private ReadOnly exprEntry As ParameterEntry

        Private ReadOnly layers As New List(Of GPSLayer)()
        Private ReadOnly gru As BatchGRU
        Private ReadOnly decoder As PerGeneDecoder
        Private ReadOnly varianceHead As PerGeneDecoder

        Private ReadOnly repMask As Boolean()

        ' 反向传播所需的状态
        Private _lastInput As Double(,)
        Private _dropoutApplied As Boolean
        Private _lastSigma As Tensor

        ''' <summary>配置</summary>
        Public ReadOnly Property Config As GPSConfig
            Get
                Return cfg
            End Get
        End Property

        ''' <summary>基因数量</summary>
        Public ReadOnly Property NumGenes As Integer
            Get
                Return n
            End Get
        End Property

        ''' <summary>隐藏维度</summary>
        Public ReadOnly Property HiddenDim As Integer
            Get
                Return d
            End Get
        End Property

        ''' <summary>窗口长度</summary>
        Public ReadOnly Property Window As Integer
            Get
                Return windowSize
            End Get
        End Property

        ''' <summary>参数登记表</summary>
        Public ReadOnly Property Parameters As ParameterStore
            Get
                Return store
            End Get
        End Property

        ''' <summary>静态图编码</summary>
        Public ReadOnly Property Encoding As GraphEncoding
            Get
                Return graphEncoding
            End Get
        End Property

        ''' <summary>基因调控图</summary>
        Public ReadOnly Property GraphData As GEARSGraph.GeneRegulatoryGraph
            Get
                Return regulatoryGraph
            End Get
        End Property

        ''' <summary>最近一次前向输出的不确定性（未启用时为 Nothing）</summary>
        Public ReadOnly Property LastSigma As Tensor
            Get
                Return _lastSigma
            End Get
        End Property

        ''' <summary>
        ''' 构建模型。
        ''' </summary>
        ''' <param name="graphData">基因调控图（节点顺序须与表达矩阵一致）</param>
        ''' <param name="encoding">静态图编码（SPD/LapPE/度）</param>
        ''' <param name="config">超参数</param>
        ''' <param name="parameterStore">外部参数登记表；为 Nothing 时内部新建</param>
        Public Sub New(graphData As GEARSGraph.GeneRegulatoryGraph,
                       encoding As GraphEncoding,
                       config As GPSConfig,
                       Optional parameterStore As ParameterStore = Nothing)

            If graphData Is Nothing Then Throw New ArgumentNullException(NameOf(graphData))
            If encoding Is Nothing Then Throw New ArgumentNullException(NameOf(encoding))
            If config Is Nothing Then Throw New ArgumentNullException(NameOf(config))
            If Not config.Validate() Then Throw New ArgumentException("GPSConfig 参数组合非法", NameOf(config))

            Me.cfg = config
            Me.regulatoryGraph = graphData
            Me.graphEncoding = encoding
            Me.store = If(parameterStore, New ParameterStore())
            Me.n = graphData.NumGenes
            Me.d = config.DModel
            Me.windowSize = config.Window
            Me.peDim = graphEncoding.LapPeDim
            Me.dropoutRate = config.Dropout
            Me.rand = New Random(config.Seed)

            If graphEncoding.NGene <> n Then
                Throw New ArgumentException($"图编码节点数 {graphEncoding.NGene} 与调控图节点数 {n} 不一致")
            End If

            ' ---- 初始嵌入分量 ----
            geneEmbedding = New Tensor(n, d)
            degInEmbedding = New Tensor(graphEncoding.DegreeBucketCount, d)
            degOutEmbedding = New Tensor(graphEncoding.DegreeBucketCount, d)

            geneEntry = store.Register("input.geneEmbedding", geneEmbedding)
            degInEntry = store.Register("input.degIn", degInEmbedding)
            degOutEntry = store.Register("input.degOut", degOutEmbedding)

            If peDim > 0 Then
                peProj = TensorOps.HeNormalInit(New Integer() {peDim, d})
                peEntry = store.Register("input.peProj", peProj)
            End If

            exprProj = Tensor.Filled(New Integer() {1, d}, CSng(1.0 / System.Math.Sqrt(d)))
            exprEntry = store.Register("input.exprProj", exprProj)

            ' ---- 融合主干 ----
            For l As Integer = 0 To config.NLayers - 1
                Dim layer As New GPSLayer(
                    dModel:=d,
                    ffnDim:=config.FfnDim,
                    nHeads:=config.NHeads,
                    spdBucketCount:=graphEncoding.SpdBucketCount,
                    store:=store,
                    useMpnn:=config.UseLocalMpnn,
                    useAttn:=config.UseGlobalAttention,
                    dropout:=0.0,   ' 层内 dropout 与梯度检查点冲突，改由模型级 dropout 承担
                    seed:=config.Seed + l,
                    name:=$"gps{l}")

                Call layers.Add(layer)
            Next

            ' ---- 时序编码与解码 ----
            gru = New BatchGRU(d, config.GruHidden, store, "gru", seed:=config.Seed + 900)
            decoder = New PerGeneDecoder(n, config.GruHidden, store, "decoder")

            If config.UseUncertainty Then
                varianceHead = New PerGeneDecoder(n, config.GruHidden, store, "uncertainty")
            End If

            repMask = New Boolean(config.GruHidden - 1) {}
        End Sub

        ''' <summary>
        ''' 前向传播：给定输入窗口 <c>[基因, w]</c>，预测下一时刻表达 <c>[基因, 1]</c>。
        ''' </summary>
        ''' <param name="input">输入窗口 <c>[基因, w]</c></param>
        ''' <param name="isTraining">是否处于训练模式（决定模型级 dropout）</param>
        ''' <returns>预测的下一时刻表达 <c>[N, 1]</c></returns>
        Public Function Forward(input As Double(,), Optional isTraining As Boolean = False) As Tensor
            ValidateWindow(input)

            ' 保存输入副本：反向传播的重算阶段必须使用与本次前向完全一致的数据
            _lastInput = CType(input.Clone(), Double(,))

            Dim sequence As Tensor = New Tensor(windowSize, n, d)

            For t As Integer = 0 To windowSize - 1
                Dim h As Tensor = BuildInitialEmbedding(_lastInput, t)

                For Each layer As GPSLayer In layers
                    h = layer.Forward(h, regulatoryGraph, graphEncoding.SpdBuckets, False)
                Next

                Call WriteTimeStep(sequence, t, h)
            Next

            Dim gruResult As (AllHidden As Tensor, FinalHidden As Tensor) = gru.ForwardSequence(sequence)
            Dim representation As Tensor = gruResult.FinalHidden

            ' ---- 模型级 dropout（单次掩码，无重算问题） ----
            _dropoutApplied = isTraining AndAlso dropoutRate > 0.0

            If _dropoutApplied Then
                Call FillMask(repMask)
                representation = TensorOps.DropoutMask(representation, repMask, dropoutRate)
            End If

            Dim prediction As Tensor = decoder.Forward(representation)

            If varianceHead IsNot Nothing Then
                Dim raw As Tensor = varianceHead.Forward(representation)
                ' softplus：σ = ln(1 + e^raw)
                _lastSigma = Tensor.computeKernel.Log(
                    Tensor.computeKernel.AddScalar(Tensor.computeKernel.Exp(raw), 1.0))
            Else
                _lastSigma = Nothing
            End If

            Return prediction
        End Function

        ''' <summary>
        ''' 反向传播：按时间步重算前向（梯度检查点），返回输入窗口的梯度。
        ''' </summary>
        ''' <param name="dOut">预测损失对输出的梯度 <c>[N, 1]</c></param>
        ''' <param name="dSigma">不确定性项的梯度 <c>[N, 1]</c>；未启用或不计入时传 Nothing</param>
        ''' <returns>输入窗口梯度 <c>[N, w]</c></returns>
        Public Function Backward(dOut As Tensor, Optional dSigma As Tensor = Nothing) As Double(,)
            If _lastInput Is Nothing Then
                Throw New InvalidOperationException("必须先执行前向传播才能反向传播")
            End If

            Dim dRepresentation As Tensor = decoder.Backward(dOut)

            If _dropoutApplied Then
                dRepresentation = TensorOps.DropoutMaskBackward(dRepresentation, repMask, dropoutRate)
            End If

            ' ---- 不确定性头 ----
            If dSigma IsNot Nothing AndAlso varianceHead IsNot Nothing Then
                ' softplus 的导数：σ'(raw) = 1 − e^(−σ)
                Dim softplusGrad As Tensor = Tensor.computeKernel.AddScalar(
                    Tensor.computeKernel.Negate(Tensor.computeKernel.Exp(Tensor.computeKernel.Negate(_lastSigma))), 1.0)
                Dim dRaw As Tensor = Tensor.computeKernel.Multiply(dSigma, softplusGrad)
                Dim dFromVariance As Tensor = varianceHead.Backward(dRaw)

                Call TensorOps.Accumulate(dRepresentation, dFromVariance)
            End If

            ' ---- GRU 反向：只有最后一步的表示进入解码器 ----
            Dim dAllHidden As Tensor = New Tensor(windowSize, n, cfg.GruHidden)
            Call WriteTimeStep(dAllHidden, windowSize - 1, dRepresentation)

            Dim dSequence As Tensor = gru.BackwardSequence(dAllHidden)

            ' ---- 逐时间步回传 GPS 主干（重算前向 + 立即反向） ----
            Dim dInput As Double(,) = New Double(n - 1, windowSize - 1) {}
            Dim spd As Integer(,) = graphEncoding.SpdBuckets

            For t As Integer = windowSize - 1 To 0 Step -1
                Dim h As Tensor = BuildInitialEmbedding(_lastInput, t)

                For Each layer As GPSLayer In layers
                    h = layer.Forward(h, regulatoryGraph, spd, False)
                Next

                Dim dH As Tensor = SliceTimeStep(dSequence, t)

                For l As Integer = layers.Count - 1 To 0 Step -1
                    dH = layers(l).Backward(dH, regulatoryGraph)
                Next

                Call AccumulateInitialGradient(dH, t, dInput)
            Next

            Return dInput
        End Function

        ''' <summary>
        ''' 构造某时间步的初始节点嵌入 <c>[N, d]</c>。
        ''' </summary>
        ''' <param name="input">输入窗口 <c>[基因, w]</c></param>
        ''' <param name="t">时间步下标</param>
        ''' <returns>初始嵌入 <c>[N, d]</c></returns>
        Private Function BuildInitialEmbedding(input As Double(,), t As Integer) As Tensor
            Dim result As Tensor = New Tensor(n, d)
            Dim rd As Double() = result.Data
            Dim ed As Double() = geneEmbedding.Data
            Dim din As Double() = degInEmbedding.Data
            Dim dout As Double() = degOutEmbedding.Data
            Dim exd As Double() = exprProj.Data
            Dim peData As Double(,) = Nothing
            Dim peW As Double() = Nothing

            If peProj IsNot Nothing Then
                peData = graphEncoding.LapPe
                peW = peProj.Data
            End If

            Dim bucketIn As Integer() = graphEncoding.InDegreeBucket
            Dim bucketOut As Integer() = graphEncoding.OutDegreeBucket

            For i As Integer = 0 To n - 1
                Dim offset As Integer = i * d
                Dim x As Double = input(i, t)
                Dim inBase As Integer = bucketIn(i) * d
                Dim outBase As Integer = bucketOut(i) * d

                For k As Integer = 0 To d - 1
                    Dim value As Double = x * exd(k) + ed(offset + k) + din(inBase + k) + dout(outBase + k)

                    If peData IsNot Nothing Then
                        For p As Integer = 0 To peDim - 1
                            value += peData(i, p) * peW(p * d + k)
                        Next
                    End If

                    rd(offset + k) = value
                Next
            Next

            Call result.MarkHostModified()

            Return result
        End Function

        ''' <summary>
        ''' 把初始嵌入的梯度散射回各参数，并累加输入窗口的梯度。
        ''' </summary>
        ''' <param name="dH">对初始嵌入的梯度 <c>[N, d]</c></param>
        ''' <param name="t">时间步下标</param>
        ''' <param name="dInput">输入窗口梯度累加目标 <c>[N, w]</c></param>
        Private Sub AccumulateInitialGradient(dH As Tensor, t As Integer, dInput As Double(,))
            Dim gd As Double() = dH.Data
            Dim geneGrad As Double() = geneEntry.Gradient.Data
            Dim dinGrad As Double() = degInEntry.Gradient.Data
            Dim doutGrad As Double() = degOutEntry.Gradient.Data
            Dim exprGrad As Double() = exprEntry.Gradient.Data
            Dim exd As Double() = exprProj.Data
            Dim peData As Double(,) = Nothing
            Dim peGrad As Double() = Nothing

            If peProj IsNot Nothing Then
                peData = graphEncoding.LapPe
                peGrad = peEntry.Gradient.Data
            End If

            Dim bucketIn As Integer() = graphEncoding.InDegreeBucket
            Dim bucketOut As Integer() = graphEncoding.OutDegreeBucket

            For i As Integer = 0 To n - 1
                Dim offset As Integer = i * d
                Dim x As Double = _lastInput(i, t)
                Dim inBase As Integer = bucketIn(i) * d
                Dim outBase As Integer = bucketOut(i) * d
                Dim dx As Double = 0.0

                For k As Integer = 0 To d - 1
                    Dim g As Double = gd(offset + k)

                    geneGrad(offset + k) += g
                    dinGrad(inBase + k) += g
                    doutGrad(outBase + k) += g
                    exprGrad(k) += g * x
                    dx += g * exd(k)

                    If peData IsNot Nothing Then
                        For p As Integer = 0 To peDim - 1
                            peGrad(p * d + k) += g * peData(i, p)
                        Next
                    End If
                Next

                dInput(i, t) += dx
            Next

            Call geneEntry.Gradient.MarkHostModified()
            Call degInEntry.Gradient.MarkHostModified()
            Call degOutEntry.Gradient.MarkHostModified()
            Call exprEntry.Gradient.MarkHostModified()

            If peProj IsNot Nothing Then
                Call peEntry.Gradient.MarkHostModified()
            End If
        End Sub

        ''' <summary>重新采样模型级 dropout 掩码。</summary>
        ''' <param name="mask">长度为隐藏维度的布尔掩码</param>
        Private Sub FillMask(mask As Boolean())
            For i As Integer = 0 To mask.Length - 1
                mask(i) = rand.NextDouble() < dropoutRate
            Next
        End Sub

        ''' <summary>校验输入窗口形状。</summary>
        ''' <param name="input">输入窗口</param>
        Private Sub ValidateWindow(input As Double(,))
            If input Is Nothing Then Throw New ArgumentNullException(NameOf(input))
            If input.GetLength(0) <> n OrElse input.GetLength(1) <> windowSize Then
                Throw New ArgumentException(
                    $"输入窗口形状应为 [{n},{windowSize}]，实际 [{input.GetLength(0)},{input.GetLength(1)}]")
            End If
        End Sub

        ''' <summary>把二维切片写入三维张量的指定时间步。</summary>
        Private Shared Sub WriteTimeStep(target As Tensor, t As Integer, source As Tensor)
            Dim batch As Integer = target.Shape(1)
            Dim features As Integer = target.Shape(2)

            Call System.Array.Copy(source.Data, 0, target.Data, t * batch * features, batch * features)
            Call target.MarkHostModified()
        End Sub

        ''' <summary>从三维张量取出指定时间步的二维切片。</summary>
        Private Shared Function SliceTimeStep(sequence As Tensor, t As Integer) As Tensor
            Dim batch As Integer = sequence.Shape(1)
            Dim features As Integer = sequence.Shape(2)
            Dim result As Tensor = New Tensor(batch, features)

            Call System.Array.Copy(sequence.Data, t * batch * features, result.Data, 0, batch * features)
            Call result.MarkHostModified()

            Return result
        End Function

    End Class

End Namespace
