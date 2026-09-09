Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

''' <summary>
''' DeepLIFT（Rescale 规则）对 P-NET 做逐层归因
''' </summary>
''' <remarks>
''' 论文在"模型 → 生物学发现"这一环节使用 DeepLIFT（Shrikumar et al.）的 Rescale 规则，
''' 而不是普通的梯度显著性方法。原因在于：当激活函数进入饱和区（例如 tanh 的平坦段）时
''' 梯度趋近于 0，会把重要特征错误地判定为"无贡献"；而 DeepLIFT 使用差分乘数
'''
''' <c>m_(Δy/Δx) = (y - y0) / (x - x0)</c>
'''
''' 代替梯度，即使在饱和区也非零。
'''
''' 具体到 P-NET 的掩码层，记 <c>Δz_i = z_i - z0_i</c>、<c>Δa_j = a_j - a0_j</c>，
''' 则由链式法则 <c>m_(Δt/Δa_i) · m_(Δa_i/Δz_i) = C_i / Δz_i</c> 可以把 Rescale 规则化简为
'''
''' <c>C_j^(l-1) = Δa_j^(l-1) · Σ_i [ C_i^(l) · W_eff[j, i] / Δz_i^(l) ]</c>
'''
''' 也就是"以线性贡献 <c>W_eff[j,i] · Δa_j</c> 占 <c>Δz_i</c> 的比例来分摊 <c>C_i</c>"。
''' 该形式天然满足守恒性：
'''
''' <c>Σ_j C_j^(l-1) = Σ_i (C_i / Δz_i) · Σ_j W_eff[j,i] · Δa_j = Σ_i C_i</c>
'''
''' 由于 P-NET 在每一个隐藏层都挂了一个预测头，且最终输出为各头概率的平均值
''' <c>p = (1/L) Σ_l σ(z_l)</c>，所以 <c>Δp = (1/L) Σ_l Δp_l</c> 精确成立。
''' 因此第 <c>l</c> 条链路所承担的份额为 <c>(1/L) · Δp_l</c>，
''' 自顶向下反传时每一层都要把来自更深层链路的贡献**累加**进来，
''' 最终在输入层上恰好满足 <c>Σ C = Δp</c> —— 这就是 DeepLIFT 的可加性守恒性质。
'''
''' 参考态 <c>x0</c> 默认取全零向量，即"该患者身上没有任何基因组改变"这一生物学基线。
''' </remarks>
Public Class DeepLIFT

    ''' <summary>
    ''' 判断差值是否可以视作零的阈值（1e-12），用于规避除零
    ''' </summary>
    Public Const ZeroTolerance As Double = 0.000000000001

    ''' <summary>
    ''' 对给定样本做 DeepLIFT 归因
    ''' </summary>
    ''' <param name="model">训练完成的 P-NET 模型</param>
    ''' <param name="x">样本特征矩阵，形状为 [N, inputSize]</param>
    ''' <param name="reference">
    ''' 参考态输入矩阵；取 Nothing 时使用全零矩阵，
    ''' 即"没有任何基因组改变"的基线状态
    ''' </param>
    ''' <param name="labels">样本标签数组，仅用于后续的分组激活分析，可以为 Nothing</param>
    ''' <returns>归因结果对象</returns>
    Public Shared Function Attribute(model As PNETModel, x As Tensor,
                                     Optional reference As Tensor = Nothing,
                                     Optional labels As Double() = Nothing) As DeepLIFTResult

        Dim n As Integer = x.Shape(0)
        Dim d As Integer = x.Shape(1)

        If reference Is Nothing Then
            reference = New Tensor(n, d)
        End If

        Dim forward As PNETForwardResult = model.Forward(x)
        Dim baseline As PNETForwardResult = model.Forward(reference)
        Dim l As Integer = model.LayerCount
        Dim deltaA As Double()()() = New Double(l - 1)()() {}
        Dim deltaZ As Double()()() = New Double(l - 1)()() {}
        Dim activation As Double()()() = New Double(l - 1)()() {}
        Dim deltaP As Double()() = New Double(l - 1)() {}
        Dim deltaHeadZ As Double()() = New Double(l - 1)() {}

        For i As Integer = 0 To l - 1
            deltaA(i) = Diff(forward.A(i), baseline.A(i))
            deltaZ(i) = Diff(forward.Z(i), baseline.Z(i))
            activation(i) = ToJagged(forward.A(i))
            deltaP(i) = DiffVector(forward.HeadP(i), baseline.HeadP(i))
            deltaHeadZ(i) = DiffVector(forward.HeadZ(i), baseline.HeadZ(i))
        Next

        Dim deltaX As Double()() = Diff(x, reference)
        Dim contributions As Double()()() = New Double(l - 1)()() {}
        Dim propagated As Double()() = Nothing
        Dim inputContribution As Double()() = Nothing

        For i As Integer = l - 1 To 0 Step -1
            ' 第一步：把第 i 个预测头所承担的份额分摊到本层的各个节点上
            Dim headShare As Double = 1.0 / l
            Dim headC As Double()() = HeadContribution(model.Heads(i), deltaA(i), deltaHeadZ(i), deltaP(i), headShare)

            ' 第二步：把来自更深层链路的贡献累加进来
            If propagated IsNot Nothing Then
                Call AddInto(headC, propagated)
            End If

            contributions(i) = headC

            ' 第三步：继续向更精细的层反传
            If i > 0 Then
                propagated = PropagateDown(model.Layers(i), deltaA(i - 1), contributions(i), deltaZ(i))
            Else
                inputContribution = PropagateDown(model.Layers(0), deltaX, contributions(i), deltaZ(i))
            End If
        Next

        Dim deltaOutput As Double() = New Double(n - 1) {}

        For s As Integer = 0 To n - 1
            deltaOutput(s) = forward.Probability(s) - baseline.Probability(s)
        Next

        Return New DeepLIFTResult With {
            .Model = model,
            .Labels = labels,
            .LayerContributions = contributions,
            .LayerActivations = activation,
            .InputContributions = inputContribution,
            .DeltaOutput = deltaOutput
        }
    End Function

    ''' <summary>
    ''' 把某一个预测头所承担的份额按照线性贡献的比例分摊到它所挂载的隐藏层节点上
    ''' </summary>
    Private Shared Function HeadContribution(head As PredictionHead, deltaA As Double()(),
                                             deltaHeadZ As Double(), deltaP As Double(),
                                             share As Double) As Double()()

        Dim n As Integer = deltaA.Length
        Dim fanIn As Integer = head.FanIn
        Dim w As Double() = head.Weights.Data
        Dim out As Double()() = New Double(n - 1)() {}

        For s As Integer = 0 To n - 1
            out(s) = New Double(fanIn - 1) {}

            Dim dz As Double = deltaHeadZ(s)

            If std.Abs(dz) < ZeroTolerance Then
                Continue For
            End If

            Dim scale As Double = share * deltaP(s) / dz
            Dim da As Double() = deltaA(s)
            Dim target As Double() = out(s)

            For i As Integer = 0 To fanIn - 1
                target(i) = w(i) * da(i) * scale
            Next
        Next

        Return out
    End Function

    ''' <summary>
    ''' 把某一层的贡献分数继续向下反传到更精细的一层
    ''' </summary>
    ''' <param name="layer">当前层（贡献分数所在的层）</param>
    ''' <param name="deltaChild">更精细一层的激活值差分，索引为 [样本][子节点]</param>
    ''' <param name="contribution">当前层节点的贡献分数，索引为 [样本][父节点]</param>
    ''' <param name="deltaZ">当前层的线性变换差分，索引为 [样本][父节点]</param>
    ''' <returns>更精细一层的贡献分数，索引为 [样本][子节点]</returns>
    Private Shared Function PropagateDown(layer As MaskedDenseLayer, deltaChild As Double()(),
                                          contribution As Double()(), deltaZ As Double()()) As Double()()

        Dim n As Integer = contribution.Length
        Dim fanIn As Integer = layer.FanIn
        Dim fanOut As Integer = layer.FanOut
        Dim w As Double() = layer.Weights.Data
        Dim child As Integer() = layer.Connectivity.ChildIdx
        Dim start As Integer() = layer.Connectivity.ParentStart
        Dim out As Double()() = New Double(n - 1)() {}

        For s As Integer = 0 To n - 1
            out(s) = New Double(fanIn - 1) {}

            Dim parentContribution As Double() = contribution(s)
            Dim parentDeltaZ As Double() = deltaZ(s)
            Dim childDeltaA As Double() = deltaChild(s)
            Dim outRow As Double() = out(s)

            For j As Integer = 0 To fanOut - 1
                Dim c As Double = parentContribution(j)

                If c = 0.0 Then
                    Continue For
                End If

                Dim dz As Double = parentDeltaZ(j)

                If std.Abs(dz) < ZeroTolerance Then
                    Continue For
                End If

                Dim scale As Double = c / dz

                For p As Integer = start(j) To start(j + 1) - 1
                    Dim ci As Integer = child(p)

                    outRow(ci) += childDeltaA(ci) * w(ci * fanOut + j) * scale
                Next
            Next
        Next

        Return out
    End Function

    ''' <summary>
    ''' 计算两个形状相同的矩阵之差，结果为锯齿数组 [行][列]
    ''' </summary>
    Private Shared Function Diff(a As Tensor, b As Tensor) As Double()()
        Dim rows As Integer = a.Shape(0)
        Dim cols As Integer = a.Shape(1)
        Dim da As Double() = a.Data
        Dim db As Double() = b.Data
        Dim out As Double()() = New Double(rows - 1)() {}

        For i As Integer = 0 To rows - 1
            out(i) = New Double(cols - 1) {}
            Array.Copy(da, i * cols, out(i), 0, cols)

            For j As Integer = 0 To cols - 1
                out(i)(j) -= db(i * cols + j)
            Next
        Next

        Return out
    End Function

    ''' <summary>
    ''' 把单列矩阵转换为锯齿数组
    ''' </summary>
    Private Shared Function DiffVector(a As Tensor, b As Tensor) As Double()
        Dim n As Integer = a.Shape(0)
        Dim da As Double() = a.Data
        Dim db As Double() = b.Data
        Dim out As Double() = New Double(n - 1) {}

        For i As Integer = 0 To n - 1
            out(i) = da(i) - db(i)
        Next

        Return out
    End Function

    ''' <summary>
    ''' 把矩阵复制为锯齿数组 [行][列]
    ''' </summary>
    Private Shared Function ToJagged(a As Tensor) As Double()()
        Dim rows As Integer = a.Shape(0)
        Dim cols As Integer = a.Shape(1)
        Dim da As Double() = a.Data
        Dim out As Double()() = New Double(rows - 1)() {}

        For i As Integer = 0 To rows - 1
            out(i) = New Double(cols - 1) {}
            Array.Copy(da, i * cols, out(i), 0, cols)
        Next

        Return out
    End Function

    ''' <summary>
    ''' 把增量数组原地累加到目标数组之上
    ''' </summary>
    Private Shared Sub AddInto(target As Double()(), delta As Double()())
        For s As Integer = 0 To target.Length - 1
            Dim t As Double() = target(s)
            Dim d As Double() = delta(s)

            For i As Integer = 0 To t.Length - 1
                t(i) += d(i)
            Next
        Next
    End Sub

End Class

''' <summary>
''' DeepLIFT 归因结果
''' </summary>
''' <remarks>
''' 归因分数 <c>C</c> 是**有符号**的：正数表示该节点把样本推向"转移性 / 耐药"，
''' 负数表示推向"原发性"。
'''
''' 节点本身的有符号激活 <c>a ∈ [-1, 1]</c> 与跨样本聚合之后的重要性
''' <c>C_i,l = Σ_s |C_i,l^s|</c> 是解耦的：前者回答"它把样本往哪个方向推"，
''' 后者回答"它有多重要"。
''' </remarks>
Public Class DeepLIFTResult

    ''' <summary>
    ''' 被解释的模型
    ''' </summary>
    ''' <returns>P-NET 模型</returns>
    Public Property Model As PNETModel

    ''' <summary>
    ''' 样本标签数组，可能为 Nothing
    ''' </summary>
    ''' <returns>标签数组</returns>
    Public Property Labels As Double()

    ''' <summary>
    ''' 逐层逐样本的节点贡献分数，索引顺序为 [层][样本][节点]
    ''' </summary>
    ''' <returns>三维锯齿数组</returns>
    Public Property LayerContributions As Double()()()

    ''' <summary>
    ''' 逐层逐样本的有符号激活值，索引顺序为 [层][样本][节点]
    ''' </summary>
    ''' <returns>三维锯齿数组</returns>
    Public Property LayerActivations As Double()()()

    ''' <summary>
    ''' 输入特征上的贡献分数，索引顺序为 [样本][特征]
    ''' </summary>
    ''' <returns>二维锯齿数组</returns>
    ''' <remarks>
    ''' 这里的归因结果可以直接用于论文图 3 的 Sankey 图：
    ''' 即揭示"改变类型 → 基因"的对应关系（例如 AR 主要由扩增驱动、
    ''' TP53 主要由突变驱动、PTEN 主要由缺失驱动）。
    ''' </remarks>
    Public Property InputContributions As Double()()

    ''' <summary>
    ''' 每一个样本上模型输出相对于参考态的变化量 <c>Δt = p - p0</c>
    ''' </summary>
    ''' <returns>差分数组</returns>
    Public Property DeltaOutput As Double()

    ''' <summary>
    ''' 样本数量
    ''' </summary>
    ''' <returns>样本数</returns>
    Public ReadOnly Property SampleCount As Integer
        Get
            If DeltaOutput Is Nothing Then
                Return 0
            End If

            Return DeltaOutput.Length
        End Get
    End Property

    ''' <summary>
    ''' 网络层数
    ''' </summary>
    ''' <returns>隐藏层数量</returns>
    Public ReadOnly Property LayerCount As Integer
        Get
            If LayerContributions Is Nothing Then
                Return 0
            End If

            Return LayerContributions.Length
        End Get
    End Property

    ''' <summary>
    ''' 校验 DeepLIFT 的可加性守恒性质
    ''' </summary>
    ''' <returns>
    ''' 输入层归因分数之和与 <c>Δt</c> 之间的平均绝对误差；
    ''' 该值应当非常接近 0，说明分数在逐层反传的过程中没有泄漏
    ''' </returns>
    ''' <remarks>
    ''' DeepLIFT 保证目标输出的总差分恰好等于某一层所有节点贡献分数之和
    ''' （在 P-NET 的多预测头结构中，这一层指的是最精细的基因层以及其下的输入层）。
    ''' 这是"逐层读出基因 / 通路重要性"在数学上能够成立的前提。
    ''' </remarks>
    Public Function ConservationError() As Double
        Dim total As Double = 0.0

        For s As Integer = 0 To SampleCount - 1
            Dim sum As Double = 0.0

            For Each v As Double In InputContributions(s)
                sum += v
            Next

            total += std.Abs(sum - DeltaOutput(s))
        Next

        If SampleCount = 0 Then
            Return 0.0
        End If

        Return total / SampleCount
    End Function

End Class
