#Region "Microsoft.VisualBasic::74ccc848fe83da72926ec204543fb82f, Data_science\MachineLearning\VAE\GraphVAE.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xie (genetics@smrucc.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2018 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
' 
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program. If not, see <http://www.gnu.org/licenses/>.



' /********************************************************************************/

' Summaries:


' Code Statistics:

'   Total Lines: 930
'    Code Lines: 501 (53.87%)
' Comment Lines: 251 (26.99%)
'    - Xml Docs: 46.22%
' 
'   Blank Lines: 178 (19.14%)
'     File Size: 31.52 KB


' Module GraphUtils
' 
'     Function: AddBias, ApplyReLU, ApplySigmoid, BCELossSum, BinaryAccuracy
'               BroadcastRow, KLDivergence, NormalizeAdjacency, ReLUDerivFromInput, Sigmoid
'               Threshold
' 
' Class LinearLayer
' 
'     Constructor: (+1 Overloads) Sub New
' 
'     Function: Backward, Forward
' 
'     Sub: Update
' 
' Class GCNLayer
' 
'     Constructor: (+1 Overloads) Sub New
' 
'     Function: Backward, Forward
' 
'     Sub: Update
' 
' Structure GraphVAEConfig
' 
' 
' 
' Class GraphVAE
' 
'     Constructor: (+1 Overloads) Sub New
' 
'     Function: ComputeLoss, Evaluate, Forward, Generate, Predict
'               SampleEpsilon, Train
' 
'     Sub: Backward, Update
' 
' /********************************************************************************/

#End Region

' ============================================================================
' GraphVAE: 图变分自编码器 (Graph Variational Autoencoder)
' ============================================================================
' 基于 Simonovsky & Komodakis (2018) 和 Kipf & Welling (2016) 的图变分自编码器
' 输入一个图（邻接矩阵 + 节点特征），编码到潜在空间，再解码重构出图
'
' 架构:
'   编码器: GCN层 -> 图读出 -> 线性层(μ, logσ²)
'   重参数化: z = μ + σ * ε
'   解码器: 线性层 -> 节点嵌入 -> 内积(邻接) + 线性层(特征)
'
' 使用手动反向传播和Adam优化器进行训练
' ============================================================================

Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.Core
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.Core.Layers
Imports std = System.Math

' ============================================================================
' 辅助函数模块
' ============================================================================
Public Module GraphUtils

#Region "图操作"

    ''' <summary>
    ''' 归一化邻接矩阵: Â = D^(-1/2) (A + I) D^(-1/2)
    ''' 这是GCN中标准的邻接矩阵归一化方法
    ''' </summary>
    Public Function NormalizeAdjacency(adj As Tensor) As Tensor
        Dim n As Integer = adj.Shape(0)

        ' A + I (添加自环)
        Dim adjSelfLoops As New Tensor(n, n)
        For i As Integer = 0 To n - 1
            For j As Integer = 0 To n - 1
                adjSelfLoops(i, j) = adj(i, j)
            Next
            adjSelfLoops(i, i) += 1.0
        Next

        ' 计算度矩阵 D
        Dim degree(n - 1) As Double
        For i As Integer = 0 To n - 1
            Dim sum As Double = 0
            For j As Integer = 0 To n - 1
                sum += adjSelfLoops(i, j)
            Next
            degree(i) = sum
        Next

        ' D^(-1/2)
        Dim invSqrtDegree(n - 1) As Double
        For i As Integer = 0 To n - 1
            If degree(i) > 0 Then
                invSqrtDegree(i) = 1.0 / std.Sqrt(degree(i))
            Else
                invSqrtDegree(i) = 0.0
            End If
        Next

        ' Â = D^(-1/2) (A + I) D^(-1/2)
        Dim normAdj As New Tensor(n, n)
        For i As Integer = 0 To n - 1
            For j As Integer = 0 To n - 1
                normAdj(i, j) = adjSelfLoops(i, j) * invSqrtDegree(i) * invSqrtDegree(j)
            Next
        Next

        Return normAdj
    End Function

#End Region

#Region "激活函数"

    ''' <summary>
    ''' Sigmoid激活函数（标量版本）
    ''' </summary>
    Public Function Sigmoid(x As Double) As Double
        If x >= 0 Then
            Return 1.0 / (1.0 + std.Exp(-x))
        Else
            Dim e As Double = std.Exp(x)
            Return e / (1.0 + e)
        End If
    End Function

    ''' <summary>
    ''' 对张量逐元素应用Sigmoid
    ''' </summary>
    Public Function ApplySigmoid(t As Tensor) As Tensor
        Return t.Apply(Function(x) Sigmoid(x))
    End Function

    ''' <summary>
    ''' 对张量逐元素应用ReLU
    ''' </summary>
    Public Function ApplyReLU(t As Tensor) As Tensor
        Return t.Apply(Function(x) If(x > 0, x, 0.0))
    End Function

    ''' <summary>
    ''' ReLU的导数（给定预激活值和输出梯度）
    ''' </summary>
    Public Function ReLUDerivFromInput(preActivation As Tensor, gradOutput As Tensor) As Tensor
        Dim result As New Tensor(preActivation.Shape)
        For i As Integer = 0 To preActivation.Length - 1
            result.Data(i) = If(preActivation.Data(i) > 0, gradOutput.Data(i), 0.0)
        Next
        Return result
    End Function

#End Region

#Region "损失函数"

    ''' <summary>
    ''' 二元交叉熵损失（求和）
    ''' L = -sum(y * log(p) + (1-y) * log(1-p))
    ''' </summary>
    Public Function BCELossSum(pred As Tensor, target As Tensor) As Double
        Dim loss As Double = 0
        For i As Integer = 0 To pred.Length - 1
            Dim p As Double = pred.Data(i)
            ' 裁剪以避免log(0)
            p = std.Max(0.0000000001, std.Min(1.0 - 0.0000000001, p))
            Dim y As Double = target.Data(i)
            loss += -(y * std.Log(p) + (1.0 - y) * std.Log(1.0 - p))
        Next
        Return loss
    End Function

    ''' <summary>
    ''' KL散度: KL(N(μ,σ²) || N(0,1))
    ''' = -0.5 * sum(1 + logσ² - μ² - exp(logσ²))
    ''' </summary>
    Public Function KLDivergence(mu As Tensor, logVar As Tensor) As Double
        Dim kl As Double = 0
        For i As Integer = 0 To mu.Length - 1
            Dim m As Double = mu.Data(i)
            Dim lv As Double = logVar.Data(i)
            kl += -0.5 * (1.0 + lv - m * m - std.Exp(lv))
        Next
        Return kl
    End Function

#End Region

#Region "张量辅助函数"

    ''' <summary>
    ''' 将偏置向量(1, n)加到矩阵(m, n)的每一行
    ''' </summary>
    Public Function AddBias(mat As Tensor, bias As Tensor) As Tensor
        Dim m As Integer = mat.Shape(0)
        Dim n As Integer = mat.Shape(1)
        Dim result As New Tensor(m, n)
        For i As Integer = 0 To m - 1
            For j As Integer = 0 To n - 1
                result(i, j) = mat(i, j) + bias(0, j)
            Next
        Next
        Return result
    End Function

    ''' <summary>
    ''' 将(1, n)张量广播到(m, n)
    ''' </summary>
    Public Function BroadcastRow(t As Tensor, m As Integer) As Tensor
        Dim n As Integer = t.Shape(1)
        Dim result As New Tensor(m, n)
        For i As Integer = 0 To m - 1
            For j As Integer = 0 To n - 1
                result(i, j) = t(0, j)
            Next
        Next
        Return result
    End Function

    ''' <summary>
    ''' 将概率矩阵阈值化为二值矩阵
    ''' </summary>
    Public Function Threshold(t As Tensor, thresholdVal As Double) As Tensor
        Return t.Apply(Function(x) If(x >= thresholdVal, 1.0, 0.0))
    End Function

    ''' <summary>
    ''' 计算二值预测的准确率
    ''' </summary>
    Public Function BinaryAccuracy(pred As Tensor, target As Tensor, thresholdVal As Double) As Double
        Dim correct As Integer = 0
        Dim total As Integer = pred.Length
        For i As Integer = 0 To total - 1
            Dim p As Double = If(pred.Data(i) >= thresholdVal, 1.0, 0.0)
            If p = target.Data(i) Then
                correct += 1
            End If
        Next
        Return CDbl(correct) / CDbl(total)
    End Function

#End Region

End Module

' ============================================================================
' 线性层: y = x @ W + b
' ============================================================================
Public Class LinearLayer

#Region "属性"

    ''' <summary>权重矩阵 (inDim, outDim)</summary>
    Public Weights As Tensor

    ''' <summary>偏置向量 (1, outDim)</summary>
    Public Bias As Tensor

    ''' <summary>权重梯度</summary>
    Public WeightGrad As Tensor

    ''' <summary>偏置梯度</summary>
    Public BiasGrad As Tensor

    ' 前向传播缓存的输入
    Private _cachedInput As Tensor

    ' 统一的 Adam 优化器状态（Core.Layers.Adam）
    Private _adamWeights As Core.Layers.Adam
    Private _adamBias As Core.Layers.Adam

#End Region

#Region "构造函数"

    ''' <summary>
    ''' 创建线性层
    ''' </summary>
    Public Sub New(inDim As Integer, outDim As Integer, Optional seed As Integer? = Nothing)
        ' Xavier初始化权重
        Weights = Tensor.XavierInit(inDim, outDim, seed)
        ' 偏置初始化为零
        Bias = Tensor.Zeros(New Integer() {1, outDim})
        ' 初始化梯度
        WeightGrad = Tensor.Zeros(New Integer() {inDim, outDim})
        BiasGrad = Tensor.Zeros(New Integer() {1, outDim})
    End Sub

#End Region

#Region "前向/反向传播"

    ''' <summary>
    ''' 前向传播: output = input @ W + b
    ''' </summary>
    Public Function Forward(input As Tensor) As Tensor
        _cachedInput = input
        ' input @ W: (m, inDim) @ (inDim, outDim) = (m, outDim)
        Dim output As Tensor = input.MatMul(Weights)
        ' 加偏置
        output = GraphUtils.AddBias(output, Bias)
        Return output
    End Function

    ''' <summary>
    ''' 反向传播
    ''' gradOutput: (m, outDim)
    ''' 返回: gradInput (m, inDim)
    ''' </summary>
    Public Function Backward(gradOutput As Tensor) As Tensor
        ' WeightGrad = input^T @ gradOutput: (inDim, outDim)
        WeightGrad = _cachedInput.Transpose().MatMul(gradOutput)

        ' BiasGrad = sum(gradOutput, axis=0): (1, outDim)
        BiasGrad = gradOutput.Sum(0)

        ' gradInput = gradOutput @ W^T: (m, inDim)
        Dim gradInput As Tensor = gradOutput.MatMul(Weights.Transpose())
        Return gradInput
    End Function

#End Region

#Region "Adam优化器更新"

    ''' <summary>
    ''' 使用Adam优化器更新参数
    ''' </summary>
    Public Sub Update(lr As Single, beta1 As Single, beta2 As Single, eps As Single)
        If _adamWeights Is Nothing Then
            _adamWeights = New Core.Layers.Adam(Weights.Shape, lr)
            _adamBias = New Core.Layers.Adam(Bias.Shape, lr)
        End If

        _adamWeights.Beta1 = beta1
        _adamWeights.Beta2 = beta2
        _adamWeights.Epsilon = eps
        _adamBias.Beta1 = beta1
        _adamBias.Beta2 = beta2
        _adamBias.Epsilon = eps

        _adamWeights.Update(Weights, WeightGrad, lr)
        _adamBias.Update(Bias, BiasGrad, lr)
    End Sub

#End Region

End Class

' ============================================================================
' GCN层: H = ReLU(Â @ X @ W)
' ============================================================================
Public Class GCNLayer

#Region "属性"

    ''' <summary>权重矩阵 (inDim, outDim)</summary>
    Public Weights As Tensor

    ''' <summary>权重梯度</summary>
    Public WeightGrad As Tensor

    ' 前向传播缓存
    Private _cachedNormAdj As Tensor
    Private _cachedInput As Tensor
    Private _cachedAX As Tensor    ' Â @ X
    Private _cachedZ As Tensor     ' 预激活值 Â @ X @ W

    ' 统一的 Adam 优化器状态（Core.Layers.Adam）
    Private _adamWeights As Core.Layers.Adam

    ''' <summary>
    ''' 邻接矩阵是否对称。<c>True</c>（默认，无向图）时反向传播用 Â 代替 Âᵀ；
    ''' 有向图必须设为 <c>False</c>，否则梯度方向错误。
    ''' </summary>
    Public Property AssumeSymmetricAdjacency As Boolean = True

#End Region

#Region "构造函数"

    ''' <summary>
    ''' 创建GCN层
    ''' </summary>
    Public Sub New(inDim As Integer, outDim As Integer, Optional seed As Integer? = Nothing)
        ' He初始化（适用于ReLU激活）
        Weights = Tensor.HeInit(inDim, outDim, seed)
        WeightGrad = Tensor.Zeros(New Integer() {inDim, outDim})
    End Sub

#End Region

#Region "前向/反向传播"

    ''' <summary>
    ''' 前向传播: H = ReLU(Â @ X @ W)
    ''' </summary>
    Public Function Forward(normAdj As Tensor, input As Tensor) As Tensor
        _cachedNormAdj = normAdj
        _cachedInput = input

        ' AX = Â @ X: (N, inDim)
        _cachedAX = normAdj.MatMul(input)

        ' Z = AX @ W: (N, outDim)
        _cachedZ = _cachedAX.MatMul(Weights)

        ' H = ReLU(Z)
        Dim H As Tensor = GraphUtils.ApplyReLU(_cachedZ)
        Return H
    End Function

    ''' <summary>
    ''' 反向传播
    ''' gradOutput: (N, outDim) = dH
    ''' 返回: gradInput (N, inDim) = dX
    ''' </summary>
    Public Function Backward(gradOutput As Tensor) As Tensor
        ' dZ = dH * ReLU'(Z)
        Dim dZ As Tensor = GraphUtils.ReLUDerivFromInput(_cachedZ, gradOutput)

        ' WeightGrad = (Â @ X)^T @ dZ = AX^T @ dZ: (inDim, outDim)
        WeightGrad = _cachedAX.Transpose().MatMul(dZ)

        ' dX = Â^T @ dZ @ W^T。无向图的 Â 对称，可以直接复用；
        ' 有向图必须显式转置，否则梯度方向错误。
        Dim propagation As Tensor = If(AssumeSymmetricAdjacency, _cachedNormAdj, _cachedNormAdj.Transpose())
        Dim gradInput As Tensor = propagation.MatMul(dZ.MatMul(Weights.Transpose()))
        Return gradInput
    End Function

#End Region

#Region "Adam优化器更新"

    ''' <summary>
    ''' 使用Adam优化器更新参数
    ''' </summary>
    Public Sub Update(lr As Single, beta1 As Single, beta2 As Single, eps As Single)
        If _adamWeights Is Nothing Then
            _adamWeights = New Core.Layers.Adam(Weights.Shape, lr)
        End If

        _adamWeights.Beta1 = beta1
        _adamWeights.Beta2 = beta2
        _adamWeights.Epsilon = eps
        _adamWeights.Update(Weights, WeightGrad, lr)
    End Sub

#End Region

End Class

' ============================================================================
' GraphVAE配置结构
' ============================================================================
Public Structure GraphVAEConfig
    ''' <summary>节点数量</summary>
    Public NumNodes As Integer
    ''' <summary>输入特征维度</summary>
    Public InputDim As Integer
    ''' <summary>GCN第一层隐藏维度</summary>
    Public Hidden1 As Integer
    ''' <summary>GCN第二层隐藏维度（图嵌入维度）</summary>
    Public Hidden2 As Integer
    ''' <summary>潜在空间维度</summary>
    Public LatentDim As Integer
    ''' <summary>解码器隐藏维度（节点嵌入维度）</summary>
    Public DecHidden As Integer
End Structure

' ============================================================================
' GraphVAE: 图变分自编码器
' ============================================================================
Public Class GraphVAE

#Region "属性"

    ' 模型配置
    Private _config As GraphVAEConfig

    ' 编码器层
    Private _gcn1 As GCNLayer          ' GCN层1: (InputDim -> Hidden1)
    Private _gcn2 As GCNLayer          ' GCN层2: (Hidden1 -> Hidden2)
    Private _encMu As LinearLayer      ' μ投影: (Hidden2 -> LatentDim)
    Private _encLogVar As LinearLayer  ' logσ²投影: (Hidden2 -> LatentDim)

    ' 解码器层
    Private _decEmbed As LinearLayer   ' 嵌入生成: (LatentDim -> NumNodes * DecHidden)
    Private _decFeat As LinearLayer    ' 特征解码: (DecHidden -> InputDim)

    ' 前向传播缓存的中间值
    Private _adj As Tensor
    Private _features As Tensor
    Private _normAdj As Tensor
    Private _H1 As Tensor
    Private _H2 As Tensor
    Private _g As Tensor               ' 图嵌入
    Private _mu As Tensor
    Private _logVar As Tensor
    Private _epsilon As Tensor
    Private _std As Tensor             ' σ = exp(0.5 * logσ²)
    Private _z As Tensor               ' 潜在向量
    Private _h As Tensor               ' 解码器隐藏输出
    Private _E As Tensor               ' 节点嵌入（预激活）
    Private _E_act As Tensor           ' 节点嵌入（ReLU后）
    Private _S_adj As Tensor           ' 邻接矩阵预激活
    Private _reconAdj As Tensor        ' 重构的邻接矩阵
    Private _S_feat As Tensor          ' 特征预激活
    Private _reconFeat As Tensor       ' 重构的特征

    ' 随机数生成器（用于可复现的epsilon采样）
    Private _rand As Random
    Private _seed As Integer?

    ' 最近一次训练前向的中间量（供 Backward 使用）
    Private _cache As GraphForwardCache

    ' 扩展选项（有向图 / 变长图等）
    Private _options As GraphVAEOptions

    Private _fitted As Boolean = False

    ''' <summary>模型文件的数据结构版本。</summary>
    Public Const ModelFileSchemaVersion As Integer = 1

#End Region

#Region "构造函数"

    ''' <summary>
    ''' 创建GraphVAE模型
    ''' </summary>
    Public Sub New(config As GraphVAEConfig, Optional seed As Integer? = Nothing)
        Call InitializeCore(config, seed)
    End Sub

    Private Sub InitializeCore(config As GraphVAEConfig, seed As Integer?)
        _config = config
        _seed = seed
        _options = New GraphVAEOptions With {
            .MaxNodes = config.NumNodes,
            .InputDim = config.InputDim,
            .Hidden1 = config.Hidden1,
            .Hidden2 = config.Hidden2,
            .LatentDim = config.LatentDim,
            .DecHidden = config.DecHidden,
            .Seed = seed
        }

        If config.NumNodes <= 0 OrElse config.InputDim <= 0 OrElse config.LatentDim <= 0 Then
            Throw New ArgumentException(
                $"模型配置无效: NumNodes={config.NumNodes}, InputDim={config.InputDim}, LatentDim={config.LatentDim}")
        End If

        If seed.HasValue Then
            _rand = New Random(seed.Value)
        Else
            _rand = New Random()
        End If

        ' 初始化编码器
        Dim s1 As Integer? = If(seed.HasValue, seed.Value + 1, Nothing)
        Dim s2 As Integer? = If(seed.HasValue, seed.Value + 2, Nothing)
        Dim s3 As Integer? = If(seed.HasValue, seed.Value + 3, Nothing)
        Dim s4 As Integer? = If(seed.HasValue, seed.Value + 4, Nothing)
        Dim s5 As Integer? = If(seed.HasValue, seed.Value + 5, Nothing)
        Dim s6 As Integer? = If(seed.HasValue, seed.Value + 6, Nothing)

        _gcn1 = New GCNLayer(config.InputDim, config.Hidden1, s1)
        _gcn2 = New GCNLayer(config.Hidden1, config.Hidden2, s2)
        _encMu = New LinearLayer(config.Hidden2, config.LatentDim, s3)
        _encLogVar = New LinearLayer(config.Hidden2, config.LatentDim, s4)

        ' 初始化解码器
        _decEmbed = New LinearLayer(config.LatentDim, config.NumNodes * config.DecHidden, s5)
        _decFeat = New LinearLayer(config.DecHidden, config.InputDim, s6)
    End Sub

#End Region

#Region "前向传播"

    ''' <summary>
    ''' 完整的前向传播
    ''' 输入: adj (N,N), features (N,F)
    ''' 输出: (重构邻接, 重构特征, μ, logσ², z)
    ''' </summary>
    Public Function Forward(adj As Tensor, features As Tensor) As Tuple(Of Tensor, Tensor, Tensor, Tensor, Tensor)
        _cache = ForwardCore(adj, features, useSampling:=True, nodeMask:=Nothing)
        Call BindCache(_cache)

        Return Tuple.Create(_cache.ReconAdjacency, _cache.ReconFeatures, _cache.Mu, _cache.LogVar, _cache.Sample)
    End Function

    ''' <summary>
    ''' 变长图的前向：<paramref name="nodeCount"/> 之后的节点视为填充，不参与损失与梯度。
    ''' </summary>
    Public Function ForwardMasked(adj As Tensor, features As Tensor, nodeCount As Integer) As Tuple(Of Tensor, Tensor, Tensor, Tensor, Tensor)
        Dim mask As Double() = BuildNodeMask(_config.NumNodes, nodeCount)

        _cache = ForwardCore(adj, features, useSampling:=True, nodeMask:=mask)
        Call BindCache(_cache)

        Return Tuple.Create(_cache.ReconAdjacency, _cache.ReconFeatures, _cache.Mu, _cache.LogVar, _cache.Sample)
    End Function

    ''' <summary>
    ''' 唯一的前向实现。所有公开入口（<see cref="Forward"/> / <see cref="Predict"/> /
    ''' <see cref="Evaluate"/>）都走这里，避免多处重复实现导致行为漂移。
    ''' </summary>
    ''' <param name="useSampling">
    ''' <c>False</c> 时直接使用后验均值解码，不引入任何随机性——评估必须走这条路径。
    ''' </param>
    Private Function ForwardCore(adj As Tensor, features As Tensor, useSampling As Boolean,
                                 nodeMask As Double()) As GraphForwardCache
        If adj Is Nothing OrElse adj.Rank <> 2 Then
            Throw New ArgumentException("邻接矩阵必须是二维张量 [N, N]", NameOf(adj))
        End If

        Dim nodes As Integer = adj.Shape(0)

        If nodes <> _config.NumNodes OrElse adj.Shape(1) <> nodes Then
            Throw New ArgumentException(
                $"邻接矩阵形状应为 [{_config.NumNodes}, {_config.NumNodes}]，实际 [{String.Join(", ", adj.Shape)}]",
                NameOf(adj))
        End If

        If features Is Nothing OrElse features.Rank <> 2 Then
            Throw New ArgumentException("节点特征必须是二维张量 [N, F]", NameOf(features))
        End If

        If features.Shape(0) <> nodes Then
            Throw New ArgumentException($"特征矩阵行数 {features.Shape(0)} 与节点数 {nodes} 不一致", NameOf(features))
        End If

        Dim cache As New GraphForwardCache With {
            .NumNodes = nodes,
            .InputDim = features.Shape(1),
            .HiddenDim1 = _config.Hidden1,
            .HiddenDim2 = _config.Hidden2,
            .DecoderHidden = _config.DecHidden,
            .LatentDim = _config.LatentDim,
            .NodeMask = nodeMask,
            .Adjacency = adj,
            .Features = features
        }

        ' === 编码器 ===
        cache.NormAdjacency = GraphUtils.NormalizeAdjacency(adj)
        cache.Hidden1 = _gcn1.Forward(cache.NormAdjacency, features)
        cache.Hidden2 = _gcn2.Forward(cache.NormAdjacency, cache.Hidden1)
        cache.GraphEmbedding = cache.Hidden2.Mean(0)

        cache.Mu = _encMu.Forward(cache.GraphEmbedding)
        cache.LogVar = _encLogVar.Forward(cache.GraphEmbedding)

        ' === 重参数化 ===
        If useSampling Then
            cache.Epsilon = SampleEpsilon(_config.LatentDim)

            Dim halfLogVar As Tensor = cache.LogVar.Apply(Function(x) 0.5 * x)
            cache.Std = halfLogVar.Apply(Function(x) std.Exp(x))
            cache.Sample = cache.Mu + cache.Std.ElementwiseMultiply(cache.Epsilon)
        Else
            cache.Epsilon = Tensor.Zeros(New Integer() {1, _config.LatentDim})
            cache.Std = Tensor.Zeros(New Integer() {1, _config.LatentDim})
            cache.Sample = cache.Mu
        End If

        ' === 解码器 ===
        Dim decoded As Tuple(Of Tensor, Tensor, Tensor) = DecodeSample(cache.Sample)

        cache.DecoderH = decoded.Item1
        cache.NodeEmbedding = decoded.Item2
        cache.NodeEmbeddingActivated = decoded.Item3
        cache.AdjacencyLogits = cache.NodeEmbeddingActivated.MatMul(cache.NodeEmbeddingActivated.Transpose())
        cache.ReconAdjacency = GraphUtils.ApplySigmoid(cache.AdjacencyLogits)
        cache.FeatureLogits = _decFeat.Forward(cache.NodeEmbeddingActivated)
        cache.ReconFeatures = GraphUtils.ApplySigmoid(cache.FeatureLogits)

        Return cache
    End Function

    ''' <summary>解码单个潜向量，返回 (展平输出, 节点嵌入预激活, 节点嵌入激活)。</summary>
    Private Function DecodeSample(z As Tensor) As Tuple(Of Tensor, Tensor, Tensor)
        Dim nodes As Integer = _config.NumNodes
        Dim flattened As Tensor = _decEmbed.Forward(z)
        Dim embedding As Tensor = New Tensor(flattened.ToDoubleArray(), nodes, _config.DecHidden)
        Dim activated As Tensor = GraphUtils.ApplyReLU(embedding)

        Return Tuple.Create(flattened, embedding, activated)
    End Function

    ''' <summary>把缓存中的量绑定到供 <see cref="Backward"/> 使用的私有字段。</summary>
    Private Sub BindCache(cache As GraphForwardCache)
        _adj = cache.Adjacency
        _features = cache.Features
        _normAdj = cache.NormAdjacency
        _H1 = cache.Hidden1
        _H2 = cache.Hidden2
        _g = cache.GraphEmbedding
        _mu = cache.Mu
        _logVar = cache.LogVar
        _epsilon = cache.Epsilon
        _std = cache.Std
        _z = cache.Sample
        _h = cache.DecoderH
        _E = cache.NodeEmbedding
        _E_act = cache.NodeEmbeddingActivated
        _S_adj = cache.AdjacencyLogits
        _reconAdj = cache.ReconAdjacency
        _S_feat = cache.FeatureLogits
        _reconFeat = cache.ReconFeatures
    End Sub

    Private Shared Function BuildNodeMask(total As Integer, nodeCount As Integer) As Double()
        If nodeCount <= 0 OrElse nodeCount > total Then
            Throw New ArgumentOutOfRangeException(NameOf(nodeCount), $"有效节点数应落在 (0, {total}] 区间")
        End If

        Dim mask As Double() = New Double(total - 1) {}

        For i As Integer = 0 To nodeCount - 1
            mask(i) = 1
        Next

        Return mask
    End Function

    ''' <summary>
    ''' 从标准正态分布采样epsilon
    ''' </summary>
    Private Function SampleEpsilon([dim] As Integer) As Tensor
        Dim eps As New Tensor(1, [dim])
        For i As Integer = 0 To [dim] - 1
            ' Box-Muller变换
            Dim u1 As Double = 1.0 - _rand.NextDouble()
            Dim u2 As Double = 1.0 - _rand.NextDouble()
            Dim randStdNormal As Double = std.Sqrt(-2.0 * std.Log(u1)) * std.Sin(2.0 * std.PI * u2)
            eps.Data(i) = randStdNormal
        Next
        Return eps
    End Function

#End Region

#Region "损失计算"

    ''' <summary>
    ''' 计算总损失 = 重构损失 + β * KL散度
    ''' </summary>
    Public Function ComputeLoss(reconAdj As Tensor, adj As Tensor,
                                reconFeat As Tensor, features As Tensor,
                                mu As Tensor, logVar As Tensor,
                                beta As Single) As Double
        Dim N As Integer = _config.NumNodes
        Dim F As Integer = _config.InputDim
        Dim L As Integer = _config.LatentDim

        ' 变长图：只统计有效节点之间的条目（由最近一次 ForwardMasked 写入）
        Dim mask As Double() = If(_cache Is Nothing, Nothing, _cache.NodeMask)

        Dim adjLoss As Double
        Dim featLoss As Double

        If mask Is Nothing Then
            adjLoss = GraphUtils.BCELossSum(reconAdj, adj) / (N * N)
            featLoss = GraphUtils.BCELossSum(reconFeat, features) / (N * F)
        Else
            Dim validNodes As Integer = _cache.ValidNodeCount()

            adjLoss = MaskedBCELoss(reconAdj, adj, mask, N) / std.Max(1, validNodes * validNodes)
            featLoss = MaskedRowBCELoss(reconFeat, features, mask, N, F) / std.Max(1, validNodes * F)
        End If

        ' KL散度（均值）
        Dim klLoss As Double = GraphUtils.KLDivergence(mu, logVar) / L

        ' 总损失
        Dim totalLoss As Double = adjLoss + featLoss + beta * klLoss
        Return totalLoss
    End Function

    ''' <summary>标量二元交叉熵，供掩码求和使用（避免逐元素构造张量）。</summary>
    Private Shared Function BceScalar(probability As Double, target As Double) As Double
        Dim clamped As Double = std.Max(1.0E-10, std.Min(1.0 - 1.0E-10, probability))

        Return -(target * std.Log(clamped) + (1.0 - target) * std.Log(1.0 - clamped))
    End Function

    ''' <summary>只对掩码内节点对求 BCE（求和，未归一）。</summary>
    Private Shared Function MaskedBCELoss(recon As Tensor, target As Tensor, mask As Double(), nodes As Integer) As Double
        Dim acc As Double = 0
        Dim reconData As Double() = recon.Data
        Dim targetData As Double() = target.Data

        For i As Integer = 0 To nodes - 1
            If mask(i) <= 0 Then
                Continue For
            End If

            Dim rowBase As Integer = i * nodes

            For j As Integer = 0 To nodes - 1
                If mask(j) <= 0 Then
                    Continue For
                End If

                acc += BceScalar(reconData(rowBase + j), targetData(rowBase + j))
            Next
        Next

        Return acc
    End Function

    ''' <summary>只对掩码内节点的特征求 BCE（求和，未归一）。</summary>
    Private Shared Function MaskedRowBCELoss(recon As Tensor, target As Tensor, mask As Double(),
                                             nodes As Integer, columns As Integer) As Double
        Dim acc As Double = 0
        Dim reconData As Double() = recon.Data
        Dim targetData As Double() = target.Data

        For i As Integer = 0 To nodes - 1
            If mask(i) <= 0 Then
                Continue For
            End If

            Dim rowBase As Integer = i * columns

            For j As Integer = 0 To columns - 1
                acc += BceScalar(reconData(rowBase + j), targetData(rowBase + j))
            Next
        Next

        Return acc
    End Function

#End Region

#Region "反向传播"

    ''' <summary>
    ''' 完整的反向传播
    ''' 计算所有参数的梯度并存储在各层的WeightGrad/BiasGrad中
    ''' </summary>
    Public Sub Backward(adj As Tensor, features As Tensor,
                        reconAdj As Tensor, reconFeat As Tensor,
                        mu As Tensor, logVar As Tensor, z As Tensor,
                        beta As Single)
        Dim N As Integer = _config.NumNodes
        Dim F As Integer = _config.InputDim
        Dim L As Integer = _config.LatentDim
        Dim H3 As Integer = _config.DecHidden

        ' === 解码器反向传播 ===

        ' 变长图的节点掩码（由最近一次 ForwardMasked 写入）
        Dim mask As Double() = If(_cache Is Nothing, Nothing, _cache.NodeMask)
        Dim validNodes As Integer = If(mask Is Nothing, N, _cache.ValidNodeCount())

        ' 邻接矩阵 BCE 梯度（通过 sigmoid）：dS_adj = (A' - A) / (N_valid²)
        Dim dS_adj As New Tensor(N, N)
        Dim dSBuffer As Double() = dS_adj.Data
        Dim reconBuffer As Double() = reconAdj.Data
        Dim targetBuffer As Double() = adj.Data
        Dim adjScale As Double = 1.0 / std.Max(1, validNodes * validNodes)

        For i As Integer = 0 To N - 1
            Dim rowWeight As Double = If(mask Is Nothing, 1.0, mask(i))
            Dim rowBase As Integer = i * N

            For j As Integer = 0 To N - 1
                Dim columnWeight As Double = If(mask Is Nothing, 1.0, mask(j))
                dSBuffer(rowBase + j) =
                    (reconBuffer(rowBase + j) - targetBuffer(rowBase + j)) * adjScale * rowWeight * columnWeight
            Next
        Next

        ' dE_act_adj = (dS + dSᵀ)·E_act
        '
        ' 把它拆成两次 [N, H3] 乘法，而不是先构造 dSᵀ 与 (dS + dSᵀ) 两个 N×N 临时量：
        ' 峰值内存从「约 3 份额外 N×N」降到「0 份额外 N×N」。
        Dim dE_act_adj As Tensor =
            dS_adj.MatMul(_E_act) + _E_act.Transpose().MatMul(dS_adj).Transpose()

        ' 特征 BCE 梯度（通过 sigmoid）：dS_feat = (X' - X) / (N_valid · F)
        Dim dS_feat As New Tensor(N, F)
        Dim featBuffer As Double() = dS_feat.Data
        Dim reconFeatBuffer As Double() = reconFeat.Data
        Dim targetFeatBuffer As Double() = features.Data
        Dim featScale As Double = 1.0 / std.Max(1, validNodes * F)

        For i As Integer = 0 To N - 1
            Dim rowWeight As Double = If(mask Is Nothing, 1.0, mask(i))
            Dim rowBase As Integer = i * F

            For column As Integer = 0 To F - 1
                featBuffer(rowBase + column) =
                    (reconFeatBuffer(rowBase + column) - targetFeatBuffer(rowBase + column)) * featScale * rowWeight
            Next
        Next

        ' 通过 S_feat = E_act @ Wf + bf (decFeat层)
        ' 返回 dE_act_feat (N, H3)
        Dim dE_act_feat As Tensor = _decFeat.Backward(dS_feat)

        ' 总 dE_act
        Dim dE_act As Tensor = dE_act_adj + dE_act_feat

        ' 通过 ReLU
        Dim dE As Tensor = GraphUtils.ReLUDerivFromInput(_E, dE_act)

        ' 重塑 dE (N, H3) -> dh (1, N*H3)
        Dim dh As New Tensor(dE.ToDoubleArray(), 1, N * H3)

        ' 通过 h = z @ We + be (decEmbed层)
        ' 返回 dz_recon (1, L)
        Dim dz_recon As Tensor = _decEmbed.Backward(dh)

        ' === 重参数化反向传播 ===
        ' z = μ + σ * ε
        ' dz/dμ = 1
        ' dz/dlogσ² = 0.5 * σ * ε = 0.5 * (z - μ)
        Dim dMu_recon As Tensor = dz_recon
        Dim dLogVar_recon As New Tensor(1, L)
        For i As Integer = 0 To L - 1
            dLogVar_recon.Data(i) = dz_recon.Data(i) * 0.5 * (z.Data(i) - mu.Data(i))
        Next

        ' === KL散度梯度 ===
        ' L_KL = -0.5 * mean(1 + logσ² - μ² - exp(logσ²))
        ' dL_KL/dμ = μ / L
        ' dL_KL/dlogσ² = 0.5 * (exp(logσ²) - 1) / L
        Dim klScale As Double = 1.0 / L
        Dim dMu_KL As New Tensor(1, L)
        Dim dLogVar_KL As New Tensor(1, L)
        For i As Integer = 0 To L - 1
            dMu_KL.Data(i) = mu.Data(i) * klScale
            dLogVar_KL.Data(i) = 0.5 * (std.Exp(logVar.Data(i)) - 1.0) * klScale
        Next

        ' 总梯度（KL乘以beta）
        Dim dMu As Tensor = dMu_recon + (dMu_KL * beta)
        Dim dLogVar As Tensor = dLogVar_recon + (dLogVar_KL * beta)

        ' === 编码器线性层反向传播 ===
        ' 通过 μ = g @ Wμ + bμ
        Dim dg_mu As Tensor = _encMu.Backward(dMu)  ' (1, Hidden2)

        ' 通过 logσ² = g @ Wlv + blv
        Dim dg_lv As Tensor = _encLogVar.Backward(dLogVar)  ' (1, Hidden2)

        ' 总 dg
        Dim dg As Tensor = dg_mu + dg_lv

        ' === 图读出反向传播 ===
        ' g = mean(H2, axis=0) = (1/N) * sum(H2, axis=0)
        ' dH2 = broadcast(dg / N, (N, Hidden2))
        Dim dH2 As Tensor = GraphUtils.BroadcastRow(dg, N)
        Dim readoutScale As Single = 1.0F / CSng(N)
        dH2 = dH2 * readoutScale

        ' === GCN层反向传播 ===
        ' GCN层2: 返回 dH1
        Dim dH1 As Tensor = _gcn2.Backward(dH2)

        ' GCN层1: 返回 dX（不需要使用，但需要计算权重梯度）
        _gcn1.Backward(dH1)
    End Sub

#End Region

#Region "参数更新"

    ''' <summary>
    ''' 使用Adam优化器更新所有参数
    ''' </summary>
    Public Sub Update(lr As Single, Optional beta1 As Single = 0.9F, Optional beta2 As Single = 0.999F, Optional eps As Single = 0.00000001F)
        _gcn1.Update(lr, beta1, beta2, eps)
        _gcn2.Update(lr, beta1, beta2, eps)
        _encMu.Update(lr, beta1, beta2, eps)
        _encLogVar.Update(lr, beta1, beta2, eps)
        _decEmbed.Update(lr, beta1, beta2, eps)
        _decFeat.Update(lr, beta1, beta2, eps)
    End Sub

#End Region

#Region "训练"

    ''' <summary>
    ''' 训练模型
    ''' </summary>
    ''' <param name="graphs">图列表，每个元素是(邻接矩阵, 特征矩阵)的元组</param>
    ''' <param name="epochs">训练轮数</param>
    ''' <param name="lr">学习率</param>
    ''' <param name="beta">KL散度权重</param>
    ''' <param name="verbose">是否打印训练信息</param>
    ''' <returns>每个epoch的平均损失列表</returns>
    Public Function Train(graphs As List(Of Tuple(Of Tensor, Tensor)),
                          epochs As Integer,
                          lr As Single,
                          Optional beta As Single = 1.0F,
                          Optional verbose As Boolean = True) As List(Of Double)
        Dim losses As New List(Of Double)

        For epoch As Integer = 0 To epochs - 1
            Dim epochLoss As Double = 0

            For Each graph As Tuple(Of Tensor, Tensor) In graphs
                Dim adj As Tensor = graph.Item1
                Dim features As Tensor = graph.Item2

                ' 前向传播
                Dim result As Tuple(Of Tensor, Tensor, Tensor, Tensor, Tensor) = Forward(adj, features)
                Dim reconAdj As Tensor = result.Item1
                Dim reconFeat As Tensor = result.Item2
                Dim mu As Tensor = result.Item3
                Dim logVar As Tensor = result.Item4
                Dim z As Tensor = result.Item5

                ' 计算损失
                Dim loss As Double = ComputeLoss(reconAdj, adj, reconFeat, features, mu, logVar, beta)
                epochLoss += loss

                ' 反向传播
                Backward(adj, features, reconAdj, reconFeat, mu, logVar, z, beta)

                ' 参数更新
                Update(lr)
            Next

            epochLoss /= graphs.Count
            losses.Add(epochLoss)

            If verbose AndAlso (epoch Mod 50 = 0 OrElse epoch = epochs - 1) Then
                Console.WriteLine($"  Epoch {epoch,4:D}/{epochs}: loss = {epochLoss:F6}")
            End If
        Next

        Return losses
    End Function

#End Region

#Region "预测与生成"

    ''' <summary>
    ''' 预测（重构）图: 输入一个图，输出重构的图
    ''' 使用均值μ进行解码（不采样）
    ''' </summary>
    Public Function Predict(adj As Tensor, Optional features As Tensor = Nothing) As Tuple(Of Tensor, Tensor)
        Dim N As Integer = _config.NumNodes

        ' 如果没有提供特征，使用单位矩阵
        If features Is Nothing Then
            features = Tensor.Identity(N)
        End If

        ' 走统一前向：确定性模式（不采样 ε），因此多次调用结果完全一致
        Dim cache As GraphForwardCache = ForwardCore(adj, features, useSampling:=False, nodeMask:=Nothing)

        Return Tuple.Create(cache.ReconAdjacency, cache.ReconFeatures)
    End Function

    ''' <summary>
    ''' 从先验分布N(0,I)生成新图
    ''' </summary>
    Public Function Generate() As Tuple(Of Tensor, Tensor)
        Dim z As New Tensor(1, _config.LatentDim)

        For i As Integer = 0 To _config.LatentDim - 1
            Dim u1 As Double = 1.0 - _rand.NextDouble()
            Dim u2 As Double = 1.0 - _rand.NextDouble()
            z.Data(i) = std.Sqrt(-2.0 * std.Log(u1)) * std.Sin(2.0 * std.PI * u2)
        Next

        Dim decoded As Tuple(Of Tensor, Tensor, Tensor) = DecodeSample(z)
        Dim activated As Tensor = decoded.Item3
        Dim adjacencyLogits As Tensor = activated.MatMul(activated.Transpose())

        Return Tuple.Create(GraphUtils.ApplySigmoid(adjacencyLogits),
                            GraphUtils.ApplySigmoid(_decFeat.Forward(activated)))
    End Function

#End Region

#Region "评估"

    ''' <summary>
    ''' 评估模型在给定图上的重构质量。
    ''' 使用后验均值解码，不引入随机采样，因此同一模型的多次评估结果完全一致。
    ''' </summary>
    Public Function Evaluate(adj As Tensor, Optional features As Tensor = Nothing) As Tuple(Of Double, Double, Double)
        Dim input As Tensor = If(features, Tensor.Identity(_config.NumNodes))

        Dim cache As GraphForwardCache = ForwardCore(adj, input, useSampling:=False, nodeMask:=Nothing)

        Dim lossValue As Double = ComputeLoss(cache.ReconAdjacency, adj, cache.ReconFeatures, input, cache.Mu, cache.LogVar, 1.0F)
        Dim adjacencyAccuracy As Double = GraphUtils.BinaryAccuracy(cache.ReconAdjacency, adj, 0.5)
        Dim featureAccuracy As Double = GraphUtils.BinaryAccuracy(cache.ReconFeatures, input, 0.5)

        Return Tuple.Create(lossValue, adjacencyAccuracy, featureAccuracy)
    End Function

#End Region

#Region "生产化扩展"

    ''' <summary>模型是否已完成训练。</summary>
    Public ReadOnly Property IsFitted As Boolean
        Get
            Return _fitted
        End Get
    End Property

    ''' <summary>节点数量（即模型配置的上界）。</summary>
    Public ReadOnly Property NumNodes As Integer
        Get
            Return _config.NumNodes
        End Get
    End Property

    ''' <summary>使用扩展选项创建模型（新增重载）。</summary>
    Public Sub New(options As GraphVAEOptions)
        Call InitializeCore(options.ToConfig(), options.Seed)

        _options.AssumeSymmetricAdjacency = options.AssumeSymmetricAdjacency
        _gcn1.AssumeSymmetricAdjacency = options.AssumeSymmetricAdjacency
        _gcn2.AssumeSymmetricAdjacency = options.AssumeSymmetricAdjacency
    End Sub

    ''' <summary>
    ''' 使用统一的训练选项训练：支持 KL 退火、梯度裁剪与早停。
    ''' </summary>
    public Function Train(graphs As List(Of Tuple(Of Tensor, Tensor)), options As TrainingOptions) As List(Of Double)
        If graphs Is Nothing OrElse graphs.Count = 0 Then
            Throw New ArgumentException("训练集不得为空", NameOf(graphs))
        End If

        If options Is Nothing Then
            Throw New ArgumentNullException(NameOf(options))
        End If

        Dim epochs As Integer = std.Max(1, options.Epochs)
        Dim observer As ITrainingObserver = options.ResolveObserver(ConsoleTrainingObserver.ForGraphVae())
        Dim losses As New List(Of Double)
        Dim best As Double = Double.MaxValue
        Dim stale As Integer = 0

        For epoch As Integer = 1 To epochs
            Dim startTick As Integer = Environment.TickCount
            Dim beta As Single = CSng(options.BetaAt(epoch))
            Dim epochLoss As Double = 0

            Call observer.OnEpochBegin(epoch, epochs)

            For Each graph As Tuple(Of Tensor, Tensor) In graphs
                Dim adj As Tensor = graph.Item1
                Dim features As Tensor = graph.Item2
                Dim result As Tuple(Of Tensor, Tensor, Tensor, Tensor, Tensor) = Forward(adj, features)

                epochLoss += ComputeLoss(result.Item1, adj, result.Item2, features,
                                         result.Item3, result.Item4, beta)

                Call Backward(adj, features, result.Item1, result.Item2,
                              result.Item3, result.Item4, result.Item5, beta)
                Call ClipGradients(options.GradientClip)
                Call Update(CSng(options.LearningRate))
            Next

            epochLoss /= graphs.Count
            losses.Add(epochLoss)
            _fitted = True

            Dim metrics As New Dictionary(Of String, Double) From {{"loss", epochLoss}}

            Call observer.OnEpochEnd(epoch, metrics, Environment.TickCount - startTick)

            If options.Patience > 0 Then
                If epochLoss < best - options.MinDelta Then
                    best = epochLoss
                    stale = 0
                Else
                    stale += 1

                    If stale >= options.Patience Then
                        Call observer.OnConverged(epoch, $"损失连续 {stale} 轮未改善")
                        Exit For
                    End If
                End If
            End If
        Next

        Return losses
    End Function

    ''' <summary>
    ''' 变长图训练：每个元素为 (填充后的邻接, 填充后的特征, 有效节点数)。
    ''' 填充区域不参与损失与梯度。
    ''' </summary>
    public Function TrainMasked(graphs As List(Of Tuple(Of Tensor, Tensor, Integer)),
                                options As TrainingOptions) As List(Of Double)
        If graphs Is Nothing OrElse graphs.Count = 0 Then
            Throw New ArgumentException("训练集不得为空", NameOf(graphs))
        End If

        If options Is Nothing Then
            Throw New ArgumentNullException(NameOf(options))
        End If

        Dim epochs As Integer = std.Max(1, options.Epochs)
        Dim observer As ITrainingObserver = options.ResolveObserver(ConsoleTrainingObserver.ForGraphVae())
        Dim losses As New List(Of Double)

        For epoch As Integer = 1 To epochs
            Dim startTick As Integer = Environment.TickCount
            Dim beta As Single = CSng(options.BetaAt(epoch))
            Dim epochLoss As Double = 0

            Call observer.OnEpochBegin(epoch, epochs)

            For Each graph In graphs
                Dim adj As Tensor = graph.Item1
                Dim features As Tensor = graph.Item2
                Dim nodeCount As Integer = graph.Item3
                Dim result As Tuple(Of Tensor, Tensor, Tensor, Tensor, Tensor) = ForwardMasked(adj, features, nodeCount)

                epochLoss += ComputeLoss(result.Item1, adj, result.Item2, features,
                                         result.Item3, result.Item4, beta)

                Call Backward(adj, features, result.Item1, result.Item2,
                              result.Item3, result.Item4, result.Item5, beta)
                Call ClipGradients(options.GradientClip)
                Call Update(CSng(options.LearningRate))
            Next

            epochLoss /= graphs.Count
            losses.Add(epochLoss)
            _fitted = True

            Dim metrics As New Dictionary(Of String, Double) From {{"loss", epochLoss}}

            Call observer.OnEpochEnd(epoch, metrics, Environment.TickCount - startTick)
        Next

        Return losses
    End Function

    Private Sub ClipGradients(limit As Double)
        If limit <= 0 Then
            Return
        End If

        Call ClipLayerGradient(_gcn1.WeightGrad, limit)
        Call ClipLayerGradient(_gcn2.WeightGrad, limit)
        Call ClipLayerGradient(_encMu.WeightGrad, limit)
        Call ClipLayerGradient(_encMu.BiasGrad, limit)
        Call ClipLayerGradient(_encLogVar.WeightGrad, limit)
        Call ClipLayerGradient(_encLogVar.BiasGrad, limit)
        Call ClipLayerGradient(_decEmbed.WeightGrad, limit)
        Call ClipLayerGradient(_decEmbed.BiasGrad, limit)
        Call ClipLayerGradient(_decFeat.WeightGrad, limit)
        Call ClipLayerGradient(_decFeat.BiasGrad, limit)
    End Sub

    Private Shared Sub ClipLayerGradient(gradient As Tensor, limit As Double)
        If gradient Is Nothing Then
            Return
        End If

        Call Core.Numerics.GradientCheck.ClipByNorm(gradient.Data, limit)
        gradient.MarkHostModified()
    End Sub

    ''' <summary>把模型参数写入指定路径。</summary>
    Public Sub Save(path As String)
        Dim parameters As New ModelParameters("GraphVAE", ModelFileSchemaVersion)

        Call parameters.Add("gcn1", _gcn1.Weights)
        Call parameters.Add("gcn2", _gcn2.Weights)
        Call parameters.Add("encMu_W", _encMu.Weights)
        Call parameters.Add("encMu_b", _encMu.Bias)
        Call parameters.Add("encLv_W", _encLogVar.Weights)
        Call parameters.Add("encLv_b", _encLogVar.Bias)
        Call parameters.Add("decEmbed_W", _decEmbed.Weights)
        Call parameters.Add("decEmbed_b", _decEmbed.Bias)
        Call parameters.Add("decFeat_W", _decFeat.Weights)
        Call parameters.Add("decFeat_b", _decFeat.Bias)

        Call parameters.AddValue("numNodes", _config.NumNodes)
        Call parameters.AddValue("inputDim", _config.InputDim)
        Call parameters.AddValue("hidden1", _config.Hidden1)
        Call parameters.AddValue("hidden2", _config.Hidden2)
        Call parameters.AddValue("latentDim", _config.LatentDim)
        Call parameters.AddValue("decHidden", _config.DecHidden)
        Call parameters.AddValue("band_symmetric", If(_gcn1.AssumeSymmetricAdjacency, 1.0, 0.0))

        Call parameters.Save(path)
    End Sub

    ''' <summary>从指定路径还原模型参数。</summary>
    Public Function TryLoad(path As String, ByRef errorMessage As String) As Boolean
        errorMessage = Nothing

        Try
            Dim parameters As ModelParameters = ModelParameters.Load(path)

            If Not String.Equals(parameters.ModelType, "GraphVAE", StringComparison.Ordinal) Then
                errorMessage = $"模型类型不符: 期望 GraphVAE，实际 {parameters.ModelType}"
                Return False
            End If

            Dim nodes As Integer = CInt(parameters.GetValue("numNodes", 0))
            Dim inputDim As Integer = CInt(parameters.GetValue("inputDim", 0))
            Dim hidden1 As Integer = CInt(parameters.GetValue("hidden1", 0))
            Dim hidden2 As Integer = CInt(parameters.GetValue("hidden2", 0))
            Dim latentDim As Integer = CInt(parameters.GetValue("latentDim", 0))
            Dim decHidden As Integer = CInt(parameters.GetValue("decHidden", 0))

            If nodes <= 0 OrElse inputDim <= 0 OrElse latentDim <= 0 Then
                errorMessage = "模型文件缺少有效的维度信息"
                Return False
            End If

            Dim config As New GraphVAEConfig With {
                .NumNodes = nodes,
                .InputDim = inputDim,
                .Hidden1 = hidden1,
                .Hidden2 = hidden2,
                .LatentDim = latentDim,
                .DecHidden = decHidden
            }

            _config = config
            _gcn1 = New GCNLayer(config.InputDim, config.Hidden1, _seed)
            _gcn2 = New GCNLayer(config.Hidden1, config.Hidden2, If(_seed.HasValue, _seed.Value + 2, Nothing))
            _encMu = New LinearLayer(config.Hidden2, config.LatentDim, If(_seed.HasValue, _seed.Value + 3, Nothing))
            _encLogVar = New LinearLayer(config.Hidden2, config.LatentDim, If(_seed.HasValue, _seed.Value + 4, Nothing))
            _decEmbed = New LinearLayer(config.LatentDim, config.NumNodes * config.DecHidden, If(_seed.HasValue, _seed.Value + 5, Nothing))
            _decFeat = New LinearLayer(config.DecHidden, config.InputDim, If(_seed.HasValue, _seed.Value + 6, Nothing))

            Call LoadInto(_gcn1.Weights, parameters, "gcn1")
            Call LoadInto(_gcn2.Weights, parameters, "gcn2")
            Call LoadInto(_encMu.Weights, parameters, "encMu_W")
            Call LoadInto(_encMu.Bias, parameters, "encMu_b")
            Call LoadInto(_encLogVar.Weights, parameters, "encLv_W")
            Call LoadInto(_encLogVar.Bias, parameters, "encLv_b")
            Call LoadInto(_decEmbed.Weights, parameters, "decEmbed_W")
            Call LoadInto(_decEmbed.Bias, parameters, "decEmbed_b")
            Call LoadInto(_decFeat.Weights, parameters, "decFeat_W")
            Call LoadInto(_decFeat.Bias, parameters, "decFeat_b")

            Dim symmetric As Boolean = parameters.GetValue("band_symmetric", 1.0) > 0.5
            _gcn1.AssumeSymmetricAdjacency = symmetric
            _gcn2.AssumeSymmetricAdjacency = symmetric

            _fitted = True

            Return True
        Catch ex As Exception
            errorMessage = ex.Message
            Return False
        End Try
    End Function

    Private Shared Sub LoadInto(target As Tensor, parameters As ModelParameters, name As String)
        Dim value As Tensor = parameters.RequireTensor(name, target.Shape)

        Array.Copy(value.Data, target.Data, target.Data.Length)
        target.MarkHostModified()
    End Sub

#End Region

End Class
