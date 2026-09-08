Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

''' <summary>
''' 深度监督预测头：挂在每一个隐藏层之后的 sigmoid 二分类输出节点
''' </summary>
''' <remarks>
''' P-NET 在每一个隐藏层后面都挂一个 sigmoid 预测节点，最终预测为所有预测头输出的平均值：
'''
''' <c>&#375; = (1 / L) · Σ_l σ(z_l)</c>
'''
''' 这样设计有三个作用：
'''
''' 1. **正则化**：迫使每一层的中间表示本身就要具备判别力，防止信息只在前几层"打包"，
'''    后面的层退化为恒等映射；
''' 2. **逐层可解释**：每一个预测头都可以单独审视，从而判断哪一层抽象层级携带了主要的判别信号；
''' 3. **损失加权**：越深的层节点数越少、参数越少、拟合更困难，
'''    因此训练时给深层的预测头更高的损失权重以平衡梯度贡献（见 <see cref="LossWeight"/>）。
''' </remarks>
Public Class PredictionHead

    ''' <summary>
    ''' 预测头的名称，通常采用其所挂载的隐藏层名称
    ''' </summary>
    ''' <returns>名称字符串</returns>
    Public ReadOnly Property Name As String

    ''' <summary>
    ''' 所挂载的隐藏层的节点数量
    ''' </summary>
    ''' <returns>输入维度</returns>
    Public ReadOnly Property FanIn As Integer

    ''' <summary>
    ''' 权重向量 <c>w</c>，形状为 [1, FanIn]
    ''' </summary>
    ''' <returns>权重张量</returns>
    Public ReadOnly Property Weights As Tensor

    ''' <summary>
    ''' 偏置标量 <c>b</c>，形状为 [1]
    ''' </summary>
    ''' <returns>偏置张量</returns>
    Public ReadOnly Property Bias As Tensor

    ''' <summary>
    ''' 累积的权重梯度，形状与 <see cref="Weights"/> 相同
    ''' </summary>
    ''' <returns>权重梯度张量</returns>
    Public ReadOnly Property WeightGrad As Tensor

    ''' <summary>
    ''' 累积的偏置梯度
    ''' </summary>
    ''' <returns>偏置梯度张量</returns>
    Public ReadOnly Property BiasGrad As Tensor

    ''' <summary>
    ''' 该预测头在总损失之中的权重，越深的层取值越大
    ''' </summary>
    ''' <returns>损失权重</returns>
    Public Property LossWeight As Double = 1.0

    ''' <summary>
    ''' 最近一次前向传播所得到的 logits 值 <c>z = a·w + b</c>，形状为 [N, 1]
    ''' </summary>
    ''' <returns>logits 张量，供 DeepLIFT 归因使用</returns>
    Public ReadOnly Property LastZ As Tensor
        Get
            Return _lastZ
        End Get
    End Property

    ''' <summary>
    ''' 最近一次前向传播所得到的概率值 <c>σ(z)</c>，形状为 [N, 1]
    ''' </summary>
    ''' <returns>概率张量</returns>
    Public ReadOnly Property LastP As Tensor
        Get
            Return _lastP
        End Get
    End Property

    ''' <summary>
    ''' 最近一次前向传播所输入进来的隐藏层激活值，形状为 [N, FanIn]
    ''' </summary>
    ''' <returns>激活值张量</returns>
    Public ReadOnly Property LastInput As Tensor
        Get
            Return _lastInput
        End Get
    End Property

    Private _lastZ As Tensor
    Private _lastP As Tensor
    Private _lastInput As Tensor

    ''' <summary>
    ''' 创建深度监督预测头
    ''' </summary>
    ''' <param name="fanIn">所挂载的隐藏层的节点数量</param>
    ''' <param name="name">预测头名称</param>
    ''' <param name="lossWeight">该预测头在总损失中的权重</param>
    ''' <param name="seed">权重初始化随机数种子</param>
    Public Sub New(fanIn As Integer, Optional name As String = Nothing,
                   Optional lossWeight As Double = 1.0,
                   Optional seed As Integer? = Nothing)

        Me.FanIn = fanIn
        Me.Name = If(name, $"head_{fanIn}")
        Me.LossWeight = lossWeight
        Me.Weights = TensorOps.XavierInit(1, fanIn, seed)
        Me.Bias = New Tensor(1)
        Me.WeightGrad = New Tensor(1, fanIn)
        Me.BiasGrad = New Tensor(1)
    End Sub

    ''' <summary>
    ''' 前向传播：<c>p = σ(a·w + b)</c>
    ''' </summary>
    ''' <param name="a">隐藏层激活值矩阵，形状为 [N, FanIn]</param>
    ''' <returns>概率矩阵，形状为 [N, 1]</returns>
    Public Function Forward(a As Tensor) As Tensor
        Dim n As Integer = a.Shape(0)

        If a.Shape(1) <> FanIn Then
            Throw New ArgumentException($"预测头 {Name} 期望输入维度为 {FanIn}，实际得到 {a.Shape(1)}")
        End If

        Dim z As New Tensor(n, 1)
        Dim p As New Tensor(n, 1)
        Dim ad As Double() = a.Data
        Dim wd As Double() = Weights.Data
        Dim zd As Double() = z.Data
        Dim pd As Double() = p.Data
        Dim b As Double = Bias.Data(0)

        For row As Integer = 0 To n - 1
            Dim off As Integer = row * FanIn
            Dim acc As Double = b

            For i As Integer = 0 To FanIn - 1
                acc += ad(off + i) * wd(i)
            Next

            zd(row) = acc
            pd(row) = TensorOps.Sigmoid(acc)
        Next

        _lastInput = a
        _lastZ = z
        _lastP = p

        Return p
    End Function

    ''' <summary>
    ''' 反向传播：累积权重与偏置梯度，并返回传递给隐藏层激活值的梯度
    ''' </summary>
    ''' <param name="dLoss_dP">损失对预测概率的梯度，形状为 [N, 1]</param>
    ''' <returns>损失对隐藏层激活值的梯度 <c>∂L/∂a</c>，形状为 [N, FanIn]</returns>
    ''' <remarks>
    ''' 记 <c>dz = ∂L/∂p · p · (1 - p)</c>，则有
    ''' <c>∂L/∂w = Σ a · dz</c>、<c>∂L/∂b = Σ dz</c>、<c>∂L/∂a = dz ⊗ w</c>。
    ''' </remarks>
    Public Function Backward(dLoss_dP As Tensor) As Tensor
        Dim a As Tensor = _lastInput
        Dim p As Tensor = _lastP

        If a Is Nothing OrElse p Is Nothing Then
            Throw New InvalidOperationException($"预测头 {Name} 尚未执行前向传播，无法进行反向传播")
        End If

        Dim n As Integer = a.Shape(0)
        Dim da As New Tensor(n, FanIn)
        Dim ad As Double() = a.Data
        Dim dad As Double() = da.Data
        Dim pd As Double() = p.Data
        Dim dpd As Double() = dLoss_dP.Data
        Dim wd As Double() = Weights.Data
        Dim wg As Double() = WeightGrad.Data

        For row As Integer = 0 To n - 1
            Dim pv As Double = pd(row)
            Dim dz As Double = dpd(row) * pv * (1.0 - pv)

            If dz = 0.0 Then
                Continue For
            End If

            BiasGrad.Data(0) += dz

            Dim off As Integer = row * FanIn

            For i As Integer = 0 To FanIn - 1
                wg(i) += ad(off + i) * dz
                dad(off + i) = dz * wd(i)
            Next
        Next

        Return da
    End Function

    ''' <summary>
    ''' 清零累积的权重与偏置梯度
    ''' </summary>
    Public Sub ZeroGrad()
        Array.Clear(WeightGrad.Data, 0, WeightGrad.Length)
        Array.Clear(BiasGrad.Data, 0, BiasGrad.Length)
    End Sub

    ''' <summary>
    ''' 把参数与梯度收集到给定的列表之中，供优化器使用
    ''' </summary>
    ''' <param name="parameters">参数收集列表</param>
    ''' <param name="gradients">梯度收集列表</param>
    Public Sub CollectParameters(parameters As List(Of Tensor), gradients As List(Of Tensor))
        parameters.Add(Weights)
        parameters.Add(Bias)
        gradients.Add(WeightGrad)
        gradients.Add(BiasGrad)
    End Sub

    ''' <summary>
    ''' 生成预测头的字符串描述
    ''' </summary>
    ''' <returns>形如 ``head_Genes[48 -> 1, weight=1.000]`` 的描述文本</returns>
    Public Overrides Function ToString() As String
        Return $"{Name}[{FanIn} -> 1, weight={LossWeight.ToString("F3")}]"
    End Function

End Class
