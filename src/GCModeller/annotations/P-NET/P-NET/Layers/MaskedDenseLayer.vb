Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

''' <summary>
''' P-NET 的核心算子：受二值掩码约束的稀疏全连接层
''' </summary>
''' <remarks>
''' 普通的稠密层为 <c>y = f(W&#8407;x + b)</c>，而 P-NET 引入二值掩码矩阵
''' <c>M ∈ {0,1}^(dx × dy)</c> 之后，前向传播变为：
'''
''' <c>y = f[(M * W)&#8407;x + b]</c>
'''
''' 其中 <c>*</c> 为 Hadamard 积（逐元素相乘）。掩码取值为 0 的位置表示
''' 通路数据库中不存在"子节点 i → 父节点 j"的关系，对应权重被强制置零；
''' 在反向传播时被掩码权重的梯度 <c>M ⊙ ∂L/∂W</c> 同样恒为 0，
''' 因此这些权重永远不会被更新 —— 这等价于该条边根本不存在，
''' 而不是"训练完成之后再剪掉"（即论文所强调的 de novo 稀疏）。
'''
''' 由于掩码在整个训练过程中保持不变，本类型在构建阶段就把 <c>M</c> 编译为
''' <see cref="SparseConnectivity"/> 边表，前向与反向均只遍历真实存在的连接，
''' 复杂度为 O(N * nnz) 而不是 O(N * fanIn * fanOut)。
''' </remarks>
Public Class MaskedDenseLayer

    ''' <summary>
    ''' 层的名称
    ''' </summary>
    ''' <returns>层名称字符串</returns>
    Public ReadOnly Property Name As String

    ''' <summary>
    ''' 输入维度（子节点数量）
    ''' </summary>
    ''' <returns>输入维度</returns>
    Public ReadOnly Property FanIn As Integer

    ''' <summary>
    ''' 输出维度（父节点数量）
    ''' </summary>
    ''' <returns>输出维度</returns>
    Public ReadOnly Property FanOut As Integer

    ''' <summary>
    ''' 由生物层级编译出来的稀疏连接边表
    ''' </summary>
    ''' <returns>稀疏连接对象</returns>
    Public ReadOnly Property Connectivity As SparseConnectivity

    ''' <summary>
    ''' 二值掩码矩阵 <c>M</c>，形状为 [FanIn, FanOut]
    ''' </summary>
    ''' <returns>掩码张量，元素取值为 0 或者 1</returns>
    Public ReadOnly Property Mask As Tensor

    ''' <summary>
    ''' 权重矩阵 <c>W</c>，形状为 [FanIn, FanOut]
    ''' </summary>
    ''' <returns>权重张量</returns>
    Public ReadOnly Property Weights As Tensor

    ''' <summary>
    ''' 偏置向量 <c>b</c>，形状为 [1, FanOut]
    ''' </summary>
    ''' <returns>偏置张量</returns>
    Public ReadOnly Property Bias As Tensor

    ''' <summary>
    ''' 累积的权重梯度，形状与 <see cref="Weights"/> 相同
    ''' </summary>
    ''' <returns>权重梯度张量</returns>
    Public ReadOnly Property WeightGrad As Tensor

    ''' <summary>
    ''' 累积的偏置梯度，形状与 <see cref="Bias"/> 相同
    ''' </summary>
    ''' <returns>偏置梯度张量</returns>
    Public ReadOnly Property BiasGrad As Tensor

    ''' <summary>
    ''' 最近一次前向传播所得到的线性变换结果 <c>z</c>，形状为 [N, FanOut]
    ''' </summary>
    ''' <returns>激活前张量，供 DeepLIFT 归因使用</returns>
    Public ReadOnly Property LastZ As Tensor
        Get
            Return _lastZ
        End Get
    End Property

    ''' <summary>
    ''' 最近一次前向传播所得到的激活值 <c>tanh(z)</c>，形状为 [N, FanOut]
    ''' </summary>
    ''' <returns>激活值张量</returns>
    Public ReadOnly Property LastA As Tensor
        Get
            Return _lastA
        End Get
    End Property

    ''' <summary>
    ''' 最近一次前向传播的输入，形状为 [N, FanIn]
    ''' </summary>
    ''' <returns>输入张量</returns>
    Public ReadOnly Property LastInput As Tensor
        Get
            Return _lastInput
        End Get
    End Property

    Private _lastZ As Tensor
    Private _lastA As Tensor
    Private _lastInput As Tensor

    ''' <summary>
    ''' 当前层中真实存在的连接数量
    ''' </summary>
    ''' <returns>连接边数量</returns>
    Public ReadOnly Property ConnectionCount As Integer
        Get
            Return Connectivity.EdgeCount
        End Get
    End Property

    ''' <summary>
    ''' 当前层中可训练参数的实际数量（仅统计未被掩码屏蔽的连接与偏置）
    ''' </summary>
    ''' <returns>可训练参数数量</returns>
    Public ReadOnly Property TrainableCount As Integer
        Get
            Return Connectivity.EdgeCount + FanOut
        End Get
    End Property

    ''' <summary>
    ''' 若当前层为稠密连接时的参数数量，用于对比稀疏化带来的压缩比
    ''' </summary>
    ''' <returns>稠密参数数量</returns>
    Public ReadOnly Property DenseParameterCount As Integer
        Get
            Return FanIn * FanOut + FanOut
        End Get
    End Property

    ''' <summary>
    ''' 创建受掩码约束的稀疏全连接层
    ''' </summary>
    ''' <param name="connectivity">由生物层级编译出来的稀疏连接边表</param>
    ''' <param name="name">层名称</param>
    ''' <param name="seed">权重初始化随机数种子</param>
    ''' <param name="weightInitStd">
    ''' 权重初始化的标准差；给出 Nothing 时使用 Xavier 初始化
    ''' <c>sqrt(2 / (fanIn + fanOut))</c>
    ''' </param>
    Public Sub New(connectivity As SparseConnectivity, Optional name As String = Nothing,
                   Optional seed As Integer? = Nothing,
                   Optional weightInitStd As Double? = Nothing)

        Me.Connectivity = connectivity
        Me.FanIn = connectivity.FanIn
        Me.FanOut = connectivity.FanOut
        Me.Name = If(name, $"masked_{FanIn}_{FanOut}")

        Me.Mask = connectivity.BuildMask()
        Me.Weights = TensorOps.XavierInit(FanIn, FanOut, seed)
        Me.Bias = New Tensor(1, FanOut)
        Me.WeightGrad = New Tensor(FanIn, FanOut)
        Me.BiasGrad = New Tensor(1, FanOut)

        If weightInitStd.HasValue Then
            Me.Weights = TensorOps.RandomNormal(New Integer() {FanIn, FanOut}, 0.0, weightInitStd.Value, seed)
        End If

        ' de novo 稀疏：被屏蔽的连接在初始化时就被强制置零，之后梯度恒为 0
        Call ApplyMask()
    End Sub

    ''' <summary>
    ''' 把权重与权重梯度逐元素乘以掩码，强制被屏蔽的连接恒为零
    ''' </summary>
    ''' <remarks>
    ''' 由于前向与反向传播只遍历边表，未被掩码覆盖的位置本来就不会被读写；
    ''' 这里额外做一次逐元素相乘，是为了保证 <see cref="Weights"/> 对外可见的数值
    ''' 与"该连接不存在"这一语义严格一致。
    ''' </remarks>
    Public Sub ApplyMask()
        Dim w As Double() = Weights.Data
        Dim m As Double() = Mask.Data

        For i As Integer = 0 To w.Length - 1
            If m(i) = 0.0 Then
                w(i) = 0.0
            End If
        Next
    End Sub

    ''' <summary>
    ''' 前向传播：<c>a = tanh[(M * W)&#8407;x + b]</c>
    ''' </summary>
    ''' <param name="x">输入矩阵，形状为 [N, FanIn]</param>
    ''' <returns>激活值矩阵，形状为 [N, FanOut]</returns>
    Public Function Forward(x As Tensor) As Tensor
        Dim n As Integer = x.Shape(0)

        If x.Shape(1) <> FanIn Then
            Throw New ArgumentException($"层 {Name} 期望输入维度为 {FanIn}，实际得到 {x.Shape(1)}")
        End If

        Dim z As New Tensor(n, FanOut)

        Call TensorOps.FillBias(z, Bias)

        Dim xd As Double() = x.Data
        Dim zd As Double() = z.Data
        Dim wd As Double() = Weights.Data
        Dim child As Integer() = Connectivity.ChildIdx
        Dim start As Integer() = Connectivity.ParentStart

        For row As Integer = 0 To n - 1
            Dim xOff As Integer = row * FanIn
            Dim zOff As Integer = row * FanOut

            For j As Integer = 0 To FanOut - 1
                Dim acc As Double = zd(zOff + j)

                For p As Integer = start(j) To start(j + 1) - 1
                    Dim ci As Integer = child(p)
                    acc += xd(xOff + ci) * wd(ci * FanOut + j)
                Next

                zd(zOff + j) = acc
            Next
        Next

        _lastInput = x
        _lastZ = z
        _lastA = TensorOps.TanhActivate(z)

        Return _lastA
    End Function

    ''' <summary>
    ''' 反向传播：累积权重与偏置梯度，并返回传递给下一层的梯度
    ''' </summary>
    ''' <param name="upstream">
    ''' 损失对当前层激活值的梯度 <c>∂L/∂a</c>，形状为 [N, FanOut]
    ''' </param>
    ''' <returns>损失对当前层输入的梯度 <c>∂L/∂x</c>，形状为 [N, FanIn]</returns>
    ''' <remarks>
    ''' 记 <c>dZ = ∂L/∂a ⊙ (1 - a²)</c>，则有：
    '''
    ''' + <c>∂L/∂W = x&#8407; · dZ</c>（只累加边表上的元素）
    ''' + <c>∂L/∂b = Σ dZ</c>
    ''' + <c>∂L/∂x = dZ · (M * W)</c>
    '''
    ''' 梯度会被累加到 <see cref="WeightGrad"/> 与 <see cref="BiasGrad"/> 之上，
    ''' 需要在一个训练步开始之前调用 <see cref="ZeroGrad"/> 清零。
    ''' </remarks>
    Public Function Backward(upstream As Tensor) As Tensor
        Dim a As Tensor = _lastA
        Dim x As Tensor = _lastInput

        If a Is Nothing OrElse x Is Nothing Then
            Throw New InvalidOperationException($"层 {Name} 尚未执行前向传播，无法进行反向传播")
        End If

        Dim n As Integer = a.Shape(0)
        Dim dx As New Tensor(n, FanIn)
        Dim ad As Double() = a.Data
        Dim ud As Double() = upstream.Data
        Dim xd As Double() = x.Data
        Dim dxd As Double() = dx.Data
        Dim wd As Double() = Weights.Data
        Dim wg As Double() = WeightGrad.Data
        Dim bg As Double() = BiasGrad.Data
        Dim child As Integer() = Connectivity.ChildIdx
        Dim start As Integer() = Connectivity.ParentStart

        For row As Integer = 0 To n - 1
            Dim aOff As Integer = row * FanOut
            Dim xOff As Integer = row * FanIn

            For j As Integer = 0 To FanOut - 1
                Dim dz As Double = ud(aOff + j) * (1.0 - ad(aOff + j) * ad(aOff + j))

                If dz = 0.0 Then
                    Continue For
                End If

                bg(j) += dz

                For p As Integer = start(j) To start(j + 1) - 1
                    Dim ci As Integer = child(p)
                    Dim wij As Double = wd(ci * FanOut + j)

                    wg(ci * FanOut + j) += xd(xOff + ci) * dz
                    dxd(xOff + ci) += dz * wij
                Next
            Next
        Next

        Return dx
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
    ''' 生成当前层的字符串描述
    ''' </summary>
    ''' <returns>形如 ``Genes[144 -> 48, nnz=144, params=192, dense=6960]`` 的描述文本</returns>
    Public Overrides Function ToString() As String
        Return $"{Name}[{FanIn} -> {FanOut}, nnz={ConnectionCount}, params={TrainableCount}, dense={DenseParameterCount}]"
    End Function

End Class
