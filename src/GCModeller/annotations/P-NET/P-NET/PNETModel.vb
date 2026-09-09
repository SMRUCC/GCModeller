Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

''' <summary>
''' 一次前向传播所产生的全部中间结果
''' </summary>
''' <remarks>
''' 除了最终的概率输出之外，这里还保留了每一层的线性变换结果 <c>z</c> 与激活值 <c>a</c>，
''' 以及每一个深度监督预测头的 logits 与概率输出。
''' DeepLIFT 归因需要同时对真实输入与参考态输入做前向传播，
''' 因此这些中间结果必须独立于模型状态保存下来（而不是只存在层对象的缓存里）。
''' </remarks>
Public Class PNETForwardResult

    ''' <summary>
    ''' 输入矩阵，形状为 [N, inputSize]
    ''' </summary>
    ''' <returns>输入张量</returns>
    Public Property Input As Tensor

    ''' <summary>
    ''' 逐层的线性变换结果 <c>z</c>，第 0 项为基因层
    ''' </summary>
    ''' <returns>形状为 [N, layerSize] 的张量数组</returns>
    Public Property Z As Tensor()

    ''' <summary>
    ''' 逐层的激活值 <c>a = tanh(z)</c>，第 0 项为基因层
    ''' </summary>
    ''' <returns>形状为 [N, layerSize] 的张量数组</returns>
    Public Property A As Tensor()

    ''' <summary>
    ''' 逐层预测头的 logits 输出，形状均为 [N, 1]
    ''' </summary>
    ''' <returns>logits 张量数组</returns>
    Public Property HeadZ As Tensor()

    ''' <summary>
    ''' 逐层预测头的 sigmoid 概率输出，形状均为 [N, 1]
    ''' </summary>
    ''' <returns>概率张量数组</returns>
    Public Property HeadP As Tensor()

    ''' <summary>
    ''' 最终的融合概率，等于所有预测头输出的平均值，长度为 N
    ''' </summary>
    ''' <returns>概率数组，取值范围为 (0, 1)</returns>
    Public Property Probability As Double()

    ''' <summary>
    ''' 样本数量
    ''' </summary>
    ''' <returns>批大小</returns>
    Public ReadOnly Property SampleCount As Integer
        Get
            If Probability Is Nothing Then
                Return 0
            End If

            Return Probability.Length
        End Get
    End Property

End Class

''' <summary>
''' P-NET：生物学先验驱动的稀疏可见神经网络
''' </summary>
''' <remarks>
''' 网络结构完全由 <see cref="PathwayHierarchy"/> 所描述的生物层级所决定：
'''
''' <code>
''' 输入层(每基因 3 个改变特征) ──► 基因层 ──► 5 层通路层(逐级抽象)
'''        │                          │            │
'''        └──► sigmoid 预测头 ◄──────┴────────────┘   (每一个隐藏层都挂一个预测头)
''' </code>
'''
''' 每一个隐藏层都由一个 <see cref="MaskedDenseLayer"/> 实现，其连接拓扑由二值掩码约束；
''' 每一个隐藏层之后都挂一个 <see cref="PredictionHead"/> 做深度监督，
''' 最终的概率输出为所有预测头输出的平均值。
''' </remarks>
Public Class PNETModel

    ''' <summary>
    ''' 构建当前网络所使用的生物层级本体
    ''' </summary>
    ''' <returns>层级本体对象</returns>
    Public ReadOnly Property Hierarchy As PathwayHierarchy

    ''' <summary>
    ''' 逐层排列的掩码稀疏层，第 0 项为基因层
    ''' </summary>
    ''' <returns>掩码层列表</returns>
    Public ReadOnly Property Layers As List(Of MaskedDenseLayer)

    ''' <summary>
    ''' 与 <see cref="Layers"/> 一一对应的深度监督预测头
    ''' </summary>
    ''' <returns>预测头列表</returns>
    Public ReadOnly Property Heads As List(Of PredictionHead)

    ''' <summary>
    ''' 输入层节点数量，等于基因数量的 3 倍
    ''' </summary>
    ''' <returns>输入特征维度</returns>
    Public ReadOnly Property InputSize As Integer

    ''' <summary>
    ''' 隐藏层数量，等于 1 层基因层加上所有通路层
    ''' </summary>
    ''' <returns>隐藏层数量</returns>
    Public ReadOnly Property LayerCount As Integer
        Get
            Return Layers.Count
        End Get
    End Property

    ''' <summary>
    ''' 每一个预测头在总损失中所占的权重，索引与 <see cref="Heads"/> 一致
    ''' </summary>
    ''' <returns>损失权重数组</returns>
    ''' <remarks>
    ''' 越深的层节点数越少、可训练参数越少、拟合越困难，
    ''' 因此这里给深层的预测头分配更高的损失权重以平衡各层的梯度贡献。
    ''' </remarks>
    Public Property HeadWeights As Double()

    ''' <summary>
    ''' 网络中实际可训练的参数数量
    ''' </summary>
    ''' <returns>可训练参数数量</returns>
    Public ReadOnly Property TrainableParameterCount As Integer
        Get
            Dim n As Integer = 0

            For Each layer As MaskedDenseLayer In Layers
                n += layer.TrainableCount
            Next
            For Each head As PredictionHead In Heads
                n += head.FanIn + 1
            Next

            Return n
        End Get
    End Property

    ''' <summary>
    ''' 若所有层都采用稠密连接时的参数数量，用于评估稀疏化带来的压缩比
    ''' </summary>
    ''' <returns>稠密网络的参数数量</returns>
    ''' <remarks>
    ''' 论文指出：P-NET 仅约 7.1 万个权重，
    ''' 而同等节点数的稠密网络需要超过 2.7 亿个权重，参数量下降 3 到 4 个数量级，
    ''' 这正是 P-NET 在小样本上优于稠密网络的根本原因 —— 先验约束起到了超强正则化的作用。
    ''' </remarks>
    Public ReadOnly Property DenseParameterCount As Integer
        Get
            Dim n As Integer = 0

            For Each layer As MaskedDenseLayer In Layers
                n += layer.DenseParameterCount
            Next
            For Each head As PredictionHead In Heads
                n += head.FanIn + 1
            Next

            Return n
        End Get
    End Property

    ''' <summary>
    ''' 创建 P-NET 模型
    ''' </summary>
    ''' <param name="hierarchy">生物层级本体</param>
    ''' <param name="layers">逐层排列的掩码稀疏层</param>
    ''' <param name="heads">与 <paramref name="layers"/> 一一对应的深度监督预测头</param>
    Public Sub New(hierarchy As PathwayHierarchy, layers As IEnumerable(Of MaskedDenseLayer),
                   heads As IEnumerable(Of PredictionHead))

        Me.Hierarchy = hierarchy
        Me.Layers = New List(Of MaskedDenseLayer)(layers)
        Me.Heads = New List(Of PredictionHead)(heads)
        Me.InputSize = hierarchy.InputSize

        If Me.Layers.Count <> Me.Heads.Count Then
            Throw New ArgumentException("每一个隐藏层都必须恰好对应一个深度监督预测头")
        End If

        Me.HeadWeights = New Double(Me.Layers.Count - 1) {}

        For i As Integer = 0 To Me.HeadWeights.Length - 1
            Me.HeadWeights(i) = 1.0
            Me.Heads(i).LossWeight = 1.0
        Next
    End Sub

    ''' <summary>
    ''' 按照深度线性递增的方式重新分配深度监督预测头的损失权重
    ''' </summary>
    ''' <param name="lambda">
    ''' 深度加权系数：第 <c>l</c> 个预测头的原始权重为 <c>1 + lambda · l / (L - 1)</c>，
    ''' 取值越大则深层预测头在总损失中的占比越高；取 0 时所有预测头等权
    ''' </param>
    ''' <remarks>
    ''' 权重在设置之前会被归一化，使得所有预测头的权重之和等于 1，
    ''' 这样总损失的数值尺度不会随着网络层数的变化而变化。
    ''' </remarks>
    Public Sub SetDeepSupervisionWeights(lambda As Double)
        Dim l As Integer = LayerCount
        Dim weights As Double() = New Double(l - 1) {}
        Dim total As Double = 0.0

        For i As Integer = 0 To l - 1
            Dim ratio As Double = If(l > 1, i / (l - 1), 0.0)

            weights(i) = 1.0 + lambda * ratio
            total += weights(i)
        Next

        For i As Integer = 0 To l - 1
            weights(i) = weights(i) / total
            Heads(i).LossWeight = weights(i)
        Next

        HeadWeights = weights
    End Sub

    ''' <summary>
    ''' 前向传播
    ''' </summary>
    ''' <param name="x">输入特征矩阵，形状为 [N, InputSize]，元素为 0 或者 1 的二值改变特征</param>
    ''' <returns>包含全部中间结果的前向传播结果对象</returns>
    Public Function Forward(x As Tensor) As PNETForwardResult
        If x.Shape(1) <> InputSize Then
            Throw New ArgumentException($"模型期望输入维度为 {InputSize}，实际得到 {x.Shape(1)}")
        End If

        Dim n As Integer = x.Shape(0)
        Dim result As New PNETForwardResult() With {
            .Input = x,
            .Z = New Tensor(LayerCount - 1) {},
            .A = New Tensor(LayerCount - 1) {},
            .HeadZ = New Tensor(LayerCount - 1) {},
            .HeadP = New Tensor(LayerCount - 1) {},
            .Probability = New Double(n - 1) {}
        }

        Dim current As Tensor = x

        For i As Integer = 0 To LayerCount - 1
            Dim a As Tensor = Layers(i).Forward(current)
            Dim p As Tensor = Heads(i).Forward(a)

            result.Z(i) = Layers(i).LastZ
            result.A(i) = a
            result.HeadZ(i) = Heads(i).LastZ
            result.HeadP(i) = p

            current = a
        Next

        Dim invL As Double = 1.0 / LayerCount

        For row As Integer = 0 To n - 1
            Dim acc As Double = 0.0

            For i As Integer = 0 To LayerCount - 1
                acc += result.HeadP(i).Data(row)
            Next

            result.Probability(row) = acc * invL
        Next

        Return result
    End Function

    ''' <summary>
    ''' 对给定样本做预测，返回每一个样本的患病概率
    ''' </summary>
    ''' <param name="x">输入特征矩阵，形状为 [N, InputSize]</param>
    ''' <returns>概率数组，取值范围为 (0, 1)</returns>
    Public Function Predict(x As Tensor) As Double()
        Return Forward(x).Probability
    End Function

    ''' <summary>
    ''' 对单个样本做预测
    ''' </summary>
    ''' <param name="features">长度为 <see cref="InputSize"/> 的特征向量</param>
    ''' <returns>患病概率，取值范围为 (0, 1)</returns>
    Public Function PredictSingle(features As Double()) As Double
        Dim x As New Tensor(features, 1, features.Length)

        Return Predict(x)(0)
    End Function

    ''' <summary>
    ''' 计算深度监督加权二元交叉熵损失
    ''' </summary>
    ''' <param name="result">前向传播结果</param>
    ''' <param name="y">真实标签数组，元素取值为 0 或者 1</param>
    ''' <param name="positiveWeight">正样本类别权重</param>
    ''' <param name="negativeWeight">负样本类别权重</param>
    ''' <returns>加权之后的平均损失值</returns>
    ''' <remarks>
    ''' 总损失为各个预测头损失的加权平均：<c>L = Σ_l w_l · BCE(y, p_l)</c>，
    ''' 其中 <c>w_l</c> 来自 <see cref="HeadWeights"/>，且已经在
    ''' <see cref="SetDeepSupervisionWeights"/> 中被归一化为和为 1。
    ''' </remarks>
    Public Function ComputeLoss(result As PNETForwardResult, y As Double(),
                                Optional positiveWeight As Double = 1.0,
                                Optional negativeWeight As Double = 1.0) As Double

        Dim n As Integer = result.SampleCount
        Dim total As Double = 0.0
        Dim norm As Double = 0.0

        For i As Integer = 0 To LayerCount - 1
            Dim w As Double = HeadWeights(i)
            Dim pd As Double() = result.HeadP(i).Data
            Dim sum As Double = 0.0

            For row As Integer = 0 To n - 1
                sum += TensorOps.WeightedBinaryCrossEntropy(pd(row), y(row), positiveWeight, negativeWeight)
            Next

            total += w * sum / n
            norm += w
        Next

        If norm = 0.0 Then
            Return 0.0
        End If

        Return total / norm
    End Function

    ''' <summary>
    ''' 反向传播：累积所有层与所有预测头的梯度
    ''' </summary>
    ''' <param name="result">
    ''' 前向传播结果，必须是对本次待求梯度的同一批输入做前向传播所得到的
    ''' </param>
    ''' <param name="y">真实标签数组，元素取值为 0 或者 1</param>
    ''' <param name="positiveWeight">正样本类别权重</param>
    ''' <param name="negativeWeight">负样本类别权重</param>
    ''' <remarks>
    ''' 每一个隐藏层的激活值同时接收两路梯度：一路来自挂在它自己后面的预测头，
    ''' 另一路来自更深的层反向传播回来的梯度，二者相加之后再传入该层的反向传播。
    '''
    ''' 梯度在这里已经被除以了批大小，因此 <see cref="MaskedDenseLayer.Backward"/> 与
    ''' <see cref="PredictionHead.Backward"/> 内部只做原始累加，
    ''' 最终得到的是整个批次上的平均梯度。
    ''' </remarks>
    Public Sub Backward(result As PNETForwardResult, y As Double(),
                        Optional positiveWeight As Double = 1.0,
                        Optional negativeWeight As Double = 1.0)

        Dim n As Integer = result.SampleCount
        Dim invN As Double = 1.0 / n
        Dim norm As Double = 0.0
        Dim i As Integer

        For i = 0 To LayerCount - 1
            norm += HeadWeights(i)
        Next

        If norm = 0.0 Then
            norm = 1.0
        End If

        ' 第一步：逐个预测头反传，得到损失对各个隐藏层激活值的梯度
        Dim headGrad As Tensor() = New Tensor(LayerCount - 1) {}

        For i = 0 To LayerCount - 1
            Dim scale As Double = HeadWeights(i) * invN / norm
            Dim dLdp As New Tensor(n, 1)
            Dim dpd As Double() = dLdp.Data
            Dim pd As Double() = result.HeadP(i).Data

            For row As Integer = 0 To n - 1
                Dim d As Double = TensorOps.WeightedBCEGradient(pd(row), y(row), positiveWeight, negativeWeight)

                dpd(row) = d * scale
            Next

            headGrad(i) = Heads(i).Backward(dLdp)
        Next

        ' 第二步：自顶向下逐层反传，把更深层的梯度累加进来
        Dim upstream As Tensor = Nothing

        For i = LayerCount - 1 To 0 Step -1
            Dim acc As Tensor = headGrad(i)

            If upstream IsNot Nothing Then
                Dim accd As Double() = acc.Data
                Dim upd As Double() = upstream.Data

                For k As Integer = 0 To accd.Length - 1
                    accd(k) += upd(k)
                Next
            End If

            upstream = Layers(i).Backward(acc)
        Next
    End Sub

    ''' <summary>
    ''' 清零所有层与所有预测头之中累积的梯度
    ''' </summary>
    Public Sub ZeroGrad()
        For Each layer As MaskedDenseLayer In Layers
            Call layer.ZeroGrad()
        Next
        For Each head As PredictionHead In Heads
            Call head.ZeroGrad()
        Next
    End Sub

    ''' <summary>
    ''' 按照固定顺序收集全部可训练参数与对应的梯度
    ''' </summary>
    ''' <param name="parameters">参数收集列表</param>
    ''' <param name="gradients">梯度收集列表</param>
    ''' <remarks>
    ''' 模型与优化器之间只通过这两个列表解耦，
    ''' 因此后续替换 SGD / 加权 Adam 等优化器时不需要改动模型本身。
    ''' </remarks>
    Public Sub CollectAll(parameters As List(Of Tensor), gradients As List(Of Tensor))
        For Each layer As MaskedDenseLayer In Layers
            Call layer.CollectParameters(parameters, gradients)
        Next
        For Each head As PredictionHead In Heads
            Call head.CollectParameters(parameters, gradients)
        Next
    End Sub

    ''' <summary>
    ''' 收集全部可训练参数
    ''' </summary>
    ''' <returns>参数张量列表，顺序与 <see cref="GetGradients"/> 一致</returns>
    Public Function GetParameters() As List(Of Tensor)
        Dim parameters As New List(Of Tensor)()
        Dim gradients As New List(Of Tensor)()

        Call CollectAll(parameters, gradients)

        Return parameters
    End Function

    ''' <summary>
    ''' 收集全部梯度，顺序与 <see cref="GetParameters"/> 一致
    ''' </summary>
    ''' <returns>梯度张量列表</returns>
    Public Function GetGradients() As List(Of Tensor)
        Dim parameters As New List(Of Tensor)()
        Dim gradients As New List(Of Tensor)()

        Call CollectAll(parameters, gradients)

        Return gradients
    End Function

    ''' <summary>
    ''' 生成网络结构的文字描述
    ''' </summary>
    ''' <returns>包含逐层节点数、连接数、参数量以及稀疏压缩比的多行文本</returns>
    Public Function PrintArchitecture() As String
        Dim sb As New System.Text.StringBuilder()

        Call sb.AppendLine($"P-NET architecture: {Hierarchy}")
        Call sb.AppendLine($"input features     : {InputSize} (genes x 3 alteration types)")
        Call sb.AppendLine($"hidden layers      : {LayerCount}")
        Call sb.AppendLine()

        For i As Integer = 0 To LayerCount - 1
            Dim layer As MaskedDenseLayer = Layers(i)

            Call sb.AppendLine($"[{i}] {layer}")
            Call sb.AppendLine($"     head: {Heads(i)}")
        Next

        Call sb.AppendLine()
        Call sb.AppendLine($"trainable params   : {TrainableParameterCount}")
        Call sb.AppendLine($"dense params       : {DenseParameterCount}")

        Dim ratio As Double = If(DenseParameterCount > 0, TrainableParameterCount / DenseParameterCount, 0.0)

        Call sb.AppendLine($"sparsity ratio     : {(ratio * 100).ToString("F4")}% of dense")

        Return sb.ToString()
    End Function

End Class
