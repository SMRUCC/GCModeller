Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports GNN = Microsoft.VisualBasic.DeepLearning.GNN
Imports SMRUCC.genomics.Analysis.GEARS.Graph
Imports SMRUCC.genomics.Analysis.GEARS.Layers

Namespace Model

    ''' <summary>
    ''' GEARS 模型：面向基因表达调控网络虚拟扰动的图神经网络
    ''' </summary>
    ''' <remarks>
    ''' 本类型把 readme 的 Step 2 ~ Step 4 编排为一个可端到端训练的模型：
    '''
    ''' <list type="number">
    ''' <item><description>
    ''' <strong>Step 2 节点特征与扰动编码</strong>：
    ''' <c>h_i^(0) = [ x̄_i ‖ p_i ‖ e_i ‖ z_pert ]</c>，
    ''' 其中 x̄ 为 control 基线表达（建议先做 Z-score 标准化）、p 为扰动 multi-hot 标记、
    ''' e 为可学习的基因身份嵌入、z_pert 为被扰动基因集合经 Deep Sets 均值池化得到的全局扰动向量。
    ''' </description></item>
    ''' <item><description>
    ''' <strong>Step 3 多层消息传递</strong>：堆叠若干 <see cref="GEARSConvLayer"/>，
    ''' 每层沿调控图入边做边类型感知聚合，L 层对应 L-hop 的间接调控效应。
    ''' </description></item>
    ''' <item><description>
    ''' <strong>Step 4 解码预测</strong>：解码器把最终节点嵌入映射为 Δ表达，
    ''' 最终预测表达为 <c>x̂^pert = x̄^control + Δx̂</c>。
    ''' </description></item>
    ''' </list>
    '''
    ''' 扰动标记 <c>p</c> 的取值由调用方给出（GEARS 门面类会按
    ''' <c>InterventionMode</c> 把被扰动基因的表达值改写后一并编码进 x̄ 通道），
    ''' 因此同一个模型可以同时支持单基因与多基因组合扰动。
    ''' </remarks>
    Public Class GEARSModel
        Inherits GNN.GNNModel

        ''' <summary>基因调控图（提供稀疏入边缓存）</summary>
        ReadOnly graphData As GeneRegulatoryGraph

        ''' <summary>基因身份嵌入层</summary>
        ReadOnly embeddingLayer As GeneEmbeddingLayer

        ''' <summary>扰动集合的 Deep Sets 池化层</summary>
        ReadOnly poolingLayer As GNN.GlobalPoolingLayer

        ''' <summary>多层边类型感知图卷积</summary>
        ReadOnly convLayers As New List(Of GEARSConvLayer)()

        ''' <summary>Δ表达解码器</summary>
        ReadOnly decoder As DenseLayer

        ''' <summary>反向传播时用于回传 z_pert 梯度的缓冲区</summary>
        Dim zPertGrad As Tensor

        ''' <summary>最近一次前向传播构建的初始节点特征（调试用）</summary>
        Dim lastFeatures As Tensor

        ''' <summary>基因数量（图中节点数量）</summary>
        ''' <returns>节点数量</returns>
        Public ReadOnly Property NumGenes As Integer
            Get
                Return graphData.NumGenes
            End Get
        End Property

        ''' <summary>基因身份嵌入维度</summary>
        ''' <returns>嵌入向量长度</returns>
        Public ReadOnly Property EmbeddingDim As Integer
            Get
                Return embeddingLayer.EmbeddingDim
            End Get
        End Property

        ''' <summary>隐藏层维度</summary>
        ''' <returns>图卷积层的输出维度</returns>
        Public ReadOnly Property HiddenDim As Integer

        ''' <summary>图卷积层数（对应可捕捉的间接效应跳数）</summary>
        ''' <returns>层数</returns>
        Public ReadOnly Property NumLayers As Integer
            Get
                Return convLayers.Count
            End Get
        End Property

        ''' <summary>
        ''' 初始节点特征的维度：1(表达) + 1(扰动标记) + d(身份嵌入) + d(扰动集合向量)
        ''' </summary>
        ''' <returns>特征维度大小</returns>
        Public ReadOnly Property FeatureDim As Integer
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return 2 + 2 * EmbeddingDim
            End Get
        End Property

        ''' <summary>
        ''' 获取最近一次前向传播构建的初始节点特征 [numGenes, FeatureDim]
        ''' </summary>
        ''' <returns>节点特征张量；尚未执行前向传播时为 Nothing</returns>
        Public ReadOnly Property LastNodeFeatures As Tensor
            Get
                Return lastFeatures
            End Get
        End Property

        ''' <summary>
        ''' 创建 GEARS 模型
        ''' </summary>
        ''' <param name="graph">基因调控图</param>
        ''' <param name="embeddingDim">基因身份嵌入维度，建议取 16~64</param>
        ''' <param name="hiddenDim">图卷积隐藏层维度，建议取 32~64</param>
        ''' <param name="numLayers">
        ''' 图卷积层数，等于可捕捉的间接调控跳数；readme 建议取 2~4，过深会导致过平滑
        ''' </param>
        ''' <param name="activation">图卷积层的激活函数</param>
        ''' <param name="usePerRelationTransform">是否为每种边关系类型分配独立的变换矩阵</param>
        ''' <param name="useDense">是否使用稠密邻接矩阵聚合（默认稀疏，稠密仅用于小图对照）</param>
        ''' <param name="seed">随机初始化种子；给定后实验可复现</param>
        ''' <param name="name">模型名称</param>
        Public Sub New(graph As GeneRegulatoryGraph,
                       Optional embeddingDim As Integer = 16,
                       Optional hiddenDim As Integer = 32,
                       Optional numLayers As Integer = 2,
                       Optional activation As GNN.ActivationType = GNN.ActivationType.Tanh,
                       Optional usePerRelationTransform As Boolean = False,
                       Optional useDense As Boolean = False,
                       Optional seed As Integer? = Nothing,
                       Optional name As String = "GEARS")

            Me.graphData = graph
            Me.HiddenDim = hiddenDim
            MyBase.Name = name

            ' 由主种子派生各层的初始化种子，保证给定种子时整网初始化完全可复现
            Dim seeder As Random = New Random(If(seed.HasValue, seed.Value, Environment.TickCount))

            embeddingLayer = New GeneEmbeddingLayer(graph.NumGenes, embeddingDim, seed:=seeder.Next())
            poolingLayer = New GNN.GlobalPoolingLayer(GNN.GlobalPoolingLayer.PoolingType.Mean)

            _layers.Add(embeddingLayer)
            _layers.Add(poolingLayer)

            Dim dimIn As Integer = 2 + 2 * embeddingDim

            For i As Integer = 0 To numLayers - 1
                Dim layer As New GEARSConvLayer(
                    inFeatures:=dimIn,
                    outFeatures:=hiddenDim,
                    activation:=activation,
                    usePerRelationTransform:=usePerRelationTransform,
                    useDense:=useDense,
                    seed:=seeder.Next(),
                    name:=$"GEARSConv_{i}"
                )

                convLayers.Add(layer)
                _layers.Add(layer)

                dimIn = hiddenDim
            Next

            decoder = New DenseLayer(hiddenDim, 1, useBias:=True, scale:=0.1, seed:=seeder.Next(), name:="DeltaDecoder")
            _layers.Add(decoder)
        End Sub

        ''' <summary>
        ''' 构建初始节点特征 h^(0) = [ x̄ ‖ p ‖ e ‖ z_pert ]
        ''' </summary>
        ''' <param name="controlExpr">control 基线表达向量 [numGenes]（建议已做 Z-score 标准化）</param>
        ''' <param name="pertFlag">
        ''' 扰动标记向量 [numGenes]；被扰动基因位置为 1（或干预强度），其余为 0
        ''' </param>
        ''' <returns>初始节点特征 [numGenes, <see cref="FeatureDim"/>]</returns>
        ''' <remarks>
        ''' z_pert 的计算遵循 GEARS 的「扰动基因集合编码器」设计：
        ''' 先取被扰动基因的身份嵌入，再经 Deep Sets 均值池化聚合为全局扰动向量，
        ''' 最后广播拼接回每一个节点，使得所有节点都能感知到「本次扰动了哪些基因」。
        ''' 均值池化保证结果与扰动基因的列举顺序无关。
        ''' </remarks>
        Public Function BuildNodeFeatures(controlExpr As Double(), pertFlag As Double()) As Tensor
            Dim n As Integer = NumGenes
            Dim d As Integer = EmbeddingDim

            If controlExpr.Length <> n Then
                Throw New ArgumentException($"control 表达向量长度 {controlExpr.Length} 与基因数量 {n} 不一致")
            End If
            If pertFlag.Length <> n Then
                Throw New ArgumentException($"扰动标记向量长度 {pertFlag.Length} 与基因数量 {n} 不一致")
            End If

            ' ---- 扰动集合编码器：Deep Sets 均值池化 ----
            ' 把 multi-hot 标记按扰动基因数量放大，使池化结果等于被扰动基因嵌入的真实均值
            Dim count As Integer = 0

            For i As Integer = 0 To n - 1
                If pertFlag(i) <> 0.0 Then
                    count += 1
                End If
            Next

            Dim poolFlag As Tensor = New Tensor(n)
            Dim pfd As Double() = poolFlag.Data
            Dim scale As Double = If(count > 0, CDbl(n) / count, 0.0)

            For i As Integer = 0 To n - 1
                pfd(i) = pertFlag(i) * scale
            Next

            Dim masked As Tensor = embeddingLayer.Forward(poolFlag)
            Dim zPert As Tensor = poolingLayer.Forward(masked)
            Dim zd As Double() = zPert.Data
            Dim ed As Double() = embeddingLayer.Embeddings.Data

            ' ---- 拼接四类特征通道 ----
            Dim features As Tensor = New Tensor(n, FeatureDim)
            Dim fd As Double() = features.Data
            Dim dims As Integer = FeatureDim

            For i As Integer = 0 To n - 1
                Dim rowOff As Integer = i * dims

                fd(rowOff) = controlExpr(i)
                fd(rowOff + 1) = pertFlag(i)

                For k As Integer = 0 To d - 1
                    fd(rowOff + 2 + k) = ed(i * d + k)
                    fd(rowOff + 2 + d + k) = zd(k)
                Next
            Next

            lastFeatures = features

            Return features
        End Function

        ''' <summary>
        ''' 前向传播：由 control 表达与扰动标记预测每个基因的 Δ表达
        ''' </summary>
        ''' <param name="controlExpr">control 基线表达向量 [numGenes]</param>
        ''' <param name="pertFlag">扰动标记向量 [numGenes]</param>
        ''' <returns>预测的表达变化量 Δ，形状为 [numGenes, 1]</returns>
        Public Overloads Function Forward(controlExpr As Double(), pertFlag As Double()) As Tensor
            Dim h As Tensor = BuildNodeFeatures(controlExpr, pertFlag)

            Return ForwardFeatures(h)
        End Function

        ''' <summary>
        ''' 前向传播：直接给定初始节点特征
        ''' </summary>
        ''' <param name="features">节点特征 [numGenes, <see cref="FeatureDim"/>]</param>
        ''' <returns>预测的表达变化量 Δ，形状为 [numGenes, 1]</returns>
        Public Function ForwardFeatures(features As Tensor) As Tensor
            Dim h As Tensor = features

            For Each layer As GEARSConvLayer In convLayers
                h = layer.Forward(h, graphData)
            Next

            Return decoder.Forward(h)
        End Function

        ''' <summary>
        ''' 反向传播：累积所有层参数梯度，并把 z_pert 的梯度回传至基因身份嵌入表
        ''' </summary>
        ''' <param name="gradient">损失函数对 Δ 预测的梯度 [numGenes, 1]</param>
        ''' <returns>相对于初始节点特征的梯度 [numGenes, <see cref="FeatureDim"/>]</returns>
        ''' <remarks>
        ''' 梯度链路：解码器 → 各图卷积层（逆序） → 拆分 h0 梯度 →
        ''' z_pert 通道按行求和 → <see cref="GNN.GlobalPoolingLayer"/> 反池化 →
        ''' <see cref="GeneEmbeddingLayer"/> 累积嵌入梯度。
        ''' </remarks>
        Public Function BackwardFrom(gradient As Tensor) As Tensor
            Dim g As Tensor = decoder.Backward(gradient)

            For i As Integer = convLayers.Count - 1 To 0 Step -1
                g = convLayers(i).Backward(g, graphData)
            Next

            ' ---- 把 z_pert 通道的梯度回传到扰动集合编码器 ----
            Dim n As Integer = NumGenes
            Dim d As Integer = EmbeddingDim
            Dim dims As Integer = FeatureDim

            If zPertGrad Is Nothing OrElse zPertGrad.Shape(1) <> d Then
                zPertGrad = New Tensor(1, d)
            Else
                Call MatOps.Zero(zPertGrad)
            End If

            Dim gd As Double() = g.Data
            Dim zgd As Double() = zPertGrad.Data

            For i As Integer = 0 To n - 1
                Dim rowOff As Integer = i * dims + 2 + d

                For k As Integer = 0 To d - 1
                    zgd(k) += gd(rowOff + k)
                Next
            Next

            Dim gMasked As Tensor = poolingLayer.Backward(zPertGrad)

            Call embeddingLayer.Backward(gMasked)

            Return g
        End Function

        ''' <summary>
        ''' 推理：返回预测的表达变化量向量
        ''' </summary>
        ''' <param name="controlExpr">control 基线表达向量 [numGenes]</param>
        ''' <param name="pertFlag">扰动标记向量 [numGenes]</param>
        ''' <returns>每个基因的 Δ表达预测值</returns>
        Public Function PredictDelta(controlExpr As Double(), pertFlag As Double()) As Double()
            Dim delta As Tensor = Forward(controlExpr, pertFlag)
            Dim dd As Double() = delta.Data
            Dim result As Double() = New Double(NumGenes - 1) {}

            For i As Integer = 0 To NumGenes - 1
                result(i) = dd(i)
            Next

            Return result
        End Function

        ''' <summary>
        ''' 实现基类接口：使用内部缓存的调控图做前向传播
        ''' </summary>
        ''' <param name="nodeFeatures">初始节点特征 [numGenes, <see cref="FeatureDim"/>]</param>
        ''' <param name="graph">
        ''' 图结构参数；本模型已持有 <see cref="GeneRegulatoryGraph"/>，
        ''' 该参数仅用于满足基类签名，可传入 Nothing
        ''' </param>
        ''' <returns>预测的表达变化量 [numGenes, 1]</returns>
        Public Overrides Function Forward(nodeFeatures As Tensor, graph As GNN.Graph) As Tensor
            Return ForwardFeatures(nodeFeatures)
        End Function

        ''' <summary>
        ''' 实现基类接口：反向传播
        ''' </summary>
        ''' <param name="gradient">损失函数对 Δ 预测的梯度 [numGenes, 1]</param>
        ''' <param name="graph">图结构参数；本模型已持有调控图，该参数可传入 Nothing</param>
        ''' <returns>相对于初始节点特征的梯度</returns>
        Public Overrides Function Backward(gradient As Tensor, graph As GNN.Graph) As Tensor
            Return BackwardFrom(gradient)
        End Function
    End Class
End Namespace
