Imports Microsoft.VisualBasic.DeepLearning.LiquidNeuralNetwork
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

''' <summary>
''' 结构化液态神经网络代谢动力学模型
''' </summary>
''' <remarks>
''' 状态映射（与 readme 第四节"结构化 LTC"一致）：
''' <list type="number">
''' <item><description>隐藏状态 h ∈ R^m：内部代谢物浓度（log+z-score 归一化后）</description></item>
''' <item><description>外部输入 u ∈ R^(r+nB)：各反应的酶表达量 + 边界代谢物浓度</description></item>
''' <item><description>循环权重 W 用代谢物邻接掩码 A_adj 约束；输入权重 U 用参与掩码约束</description></item>
''' <item><description>反应通量由独立读取头给出：<c>v = e ⊙ σ(Wv·[h;u] + bv)</c></description></item>
''' <item><description>浓度读出：<c>ĉ = W_out·h + b_out</c>（W_out 被约束为对角，即逐代谢物仿射校准）</description></item>
''' </list>
''' 底层 ODE 与反向模式 AD 全部复用 <see cref="LiquidNeuralNetwork"/>（LTC / CfC 模式）。
''' </remarks>
Public Class MetabolicLiquidNetwork

#Region "属性"

    ''' <summary>代谢网络拓扑</summary>
    Public ReadOnly Property Graph As MetabolicNetworkGraph

    ''' <summary>底层的液态神经网络内核</summary>
    Public ReadOnly Property Liquid As LiquidNeuralNetwork

    ''' <summary>通量读取头权重 Wv，形状 (m + 输入维度 × r)</summary>
    Public Property FluxWeight As Tensor

    ''' <summary>通量读取头偏置 bv，形状 (r)</summary>
    Public Property FluxBias As Tensor

    ''' <summary>通量读取头权重梯度</summary>
    Public Property FluxWeightGradient As Tensor

    ''' <summary>通量读取头偏置梯度</summary>
    Public Property FluxBiasGradient As Tensor

    ''' <summary>输出层掩码（对角：逐代谢物仿射读出）</summary>
    Public ReadOnly Property OutputMask As Tensor

    ''' <summary>状态维度（内部代谢物数）</summary>
    Public ReadOnly Property MetaboliteCount As Integer

    ''' <summary>反应维度</summary>
    Public ReadOnly Property ReactionCount As Integer

    ''' <summary>输入维度 = 酶通道 + 边界通道</summary>
    Public ReadOnly Property InputSize As Integer

    ''' <summary>动力学模式</summary>
    Public ReadOnly Property Mode As LiquidMode

    ''' <summary>
    ''' ODE 积分的最大子步长。
    ''' </summary>
    ''' <remarks>
    ''' 代谢组学的时间采样往往很不规则（相邻间隔可能相差几十倍），
    ''' 而显式 RK4 的稳定域要求 decay·dt ≲ 2.8（decay = 1/τ + f）。
    ''' 因此在每个观测间隔内部再细分为若干子步，保证最快的时间尺度也被解析到；
    ''' 这是对 LNN"支持不规则采样"能力的正确实现方式。
    ''' </remarks>
    Public Property MaxSubStep As Double = 0.5

#End Region

#Region "扰动状态"

    ''' <summary>逐反应的酶水平系数，1 = 野生型，0 = 完全敲除</summary>
    Private ReadOnly _enzymeLevel As Double()

    ''' <summary>边界代谢物被固定覆盖时的取值</summary>
    Private ReadOnly _boundaryOverride As Double()

    ''' <summary>该边界代谢物是否被固定覆盖</summary>
    Private ReadOnly _boundaryFixed As Boolean()

#End Region

#Region "构造函数"

    ''' <summary>
    ''' 由代谢网络拓扑构建结构化 LTC 模型
    ''' </summary>
    ''' <param name="graph">代谢网络拓扑</param>
    ''' <param name="mode">液态神经元动力学模式（LTC 或 CFC）</param>
    ''' <param name="solver">ODE 求解器（CfC 模式下被忽略）</param>
    ''' <param name="seed">随机种子</param>
    Public Sub New(graph As MetabolicNetworkGraph,
                   Optional mode As LiquidMode = LiquidMode.LTC,
                   Optional solver As String = "rk4",
                   Optional seed As Integer? = 42)
        Me.Graph = graph
        Me.MetaboliteCount = graph.MetaboliteCount
        Me.ReactionCount = graph.ReactionCount
        Me.InputSize = graph.InputSize
        Me.Mode = mode

        Dim m = MetaboliteCount
        Dim nIn = InputSize
        Dim r = ReactionCount

        ' 隐藏状态 = 代谢物浓度，输出也是代谢物浓度
        _Liquid = New LiquidNeuralNetwork(nIn, m, m, 1, "tanh", "none", seed, mode)
        Liquid.SolverType = solver

        ' 输出层初始化为单位矩阵：让 ĉ 在训练开始时就等于 h（状态本身即浓度）
        Dim identity = Tensor.Identity(m)

        Liquid.OutputWeight = CType(identity.Clone(), Tensor)
        Liquid.OutputBias = Tensor.Zeros({m})
        _OutputMask = CType(identity.Clone(), Tensor)

        ' 通量读取头：v = e ⊙ σ(Wv·[h;u] + bv)
        _FluxWeight = Tensor.XavierInit(m + nIn, r, If(seed.HasValue, seed.Value + 11, Nothing))
        _FluxBias = Tensor.Zeros({r})
        _FluxWeightGradient = Tensor.Zeros({m + nIn, r})
        _FluxBiasGradient = Tensor.Zeros({r})

        _enzymeLevel = New Double(r - 1) {}
        _boundaryOverride = New Double(graph.BoundaryCount - 1) {}
        _boundaryFixed = New Boolean(graph.BoundaryCount - 1) {}

        For j = 0 To r - 1
            _enzymeLevel(j) = 1.0
        Next

        Call ApplyStructuralMasks()
    End Sub

    ''' <summary>
    ''' 设置液态时间常数的取值范围（代谢系统往往是 stiff 系统，跨度较大）
    ''' </summary>
    Public Sub SetTauBounds(tauMin As Double, tauMax As Double)
        For Each cell In Liquid.LiquidLayer.Cells
            cell.TauMin = tauMin
            cell.TauMax = tauMax
        Next
    End Sub

#End Region

#Region "结构化掩码"

    ''' <summary>
    ''' 把生化拓扑约束重新施加到权重与梯度上。
    ''' 必须在每一次优化器 step 之后调用，否则梯度会把被掩码的元素"推回"非零。
    ''' </summary>
    Public Sub ApplyStructuralMasks()
        Dim cell = Liquid.LiquidLayer.Cells(0)

        Call Mask(cell.WeightRecurrent, cell.WeightRecurrentGradient, Graph.AdjacencyMask)
        Call Mask(cell.WeightInput, cell.WeightInputGradient, Graph.InputMask)

        If cell.HasGate Then
            Call Mask(cell.WeightGate, cell.WeightGateGradient, Graph.AdjacencyMask)
            Call Mask(cell.WeightGateInput, cell.WeightGateInputGradient, Graph.InputMask)
        End If

        Call Mask(Liquid.OutputWeight, Liquid.OutputWeightGradient, OutputMask)
    End Sub

    Private Shared Sub Mask(param As Tensor, gradient As Tensor, mask As Tensor)
        Dim rows = param.Shape(0)
        Dim cols = param.Shape(1)

        For i = 0 To rows - 1
            For j = 0 To cols - 1
                If mask(i, j) = 0.0 Then
                    param(i, j) = 0.0
                    gradient(i, j) = 0.0
                End If
            Next
        Next
    End Sub

    ''' <summary>统计当前被掩码掉的连接比例（用于验证结构化约束是否生效）</summary>
    Public Function MaskedRatio() As Double
        Dim cell = Liquid.LiquidLayer.Cells(0)
        Dim total As Integer = 0
        Dim masked As Integer = 0
        Dim m = MetaboliteCount

        For i = 0 To m - 1
            For j = 0 To m - 1
                total += 1
                If Graph.AdjacencyMask(i, j) = 0.0 Then masked += 1
            Next
        Next

        Return masked / std.Max(1, total)
    End Function

#End Region

#Region "输入构建与扰动"

    ''' <summary>
    ''' 组装网络输入 u = [酶表达量(含敲除系数) | 边界代谢物浓度]
    ''' </summary>
    ''' <param name="enzymes">酶表达量，长度 = 反应数</param>
    ''' <param name="boundary">边界代谢物浓度，长度 = 边界代谢物数</param>
    Public Function BuildInput(enzymes As Tensor, boundary As Tensor) As Tensor
        Dim nIn = InputSize
        Dim r = ReactionCount
        Dim nB = Graph.BoundaryCount
        Dim u = New Tensor(nIn)

        For j = 0 To r - 1
            u(j) = enzymes(j) * _enzymeLevel(j)
        Next

        For k = 0 To nB - 1
            If _boundaryFixed(k) Then
                u(r + k) = _boundaryOverride(k)
            Else
                u(r + k) = boundary(k)
            End If
        Next

        Return u
    End Function

    ''' <summary>把某条反应的酶水平设为 level（0 = 完全敲除，1 = 野生型）</summary>
    Public Sub SetEnzymeLevel(reactionId As String, level As Double)
        Dim j = Graph.IndexOfReaction(reactionId)

        If j < 0 Then
            Throw New ArgumentException($"未知反应：{reactionId}")
        End If

        _enzymeLevel(j) = level
    End Sub

    ''' <summary>敲除某条反应（等价把该酶水平置 0）</summary>
    Public Sub KnockOut(reactionId As String)
        Call SetEnzymeLevel(reactionId, 0.0)
    End Sub

    ''' <summary>
    ''' 固定某个边界代谢物的浓度（例如把 o2_e 设为 0 来模拟厌氧条件）。
    ''' 覆盖会一直生效，直到调用 <see cref="ResetPerturbation"/>。
    ''' </summary>
    Public Sub SetBoundary(metaboliteId As String, value As Double)
        Dim k = Graph.IndexOfBoundary(metaboliteId)

        If k < 0 Then
            Throw New ArgumentException($"不是边界代谢物：{metaboliteId}")
        End If

        _boundaryOverride(k) = value
        _boundaryFixed(k) = True
    End Sub

    ''' <summary>清除全部酶敲除与边界覆盖，恢复野生型条件</summary>
    Public Sub ResetPerturbation()
        For j = 0 To _enzymeLevel.Length - 1
            _enzymeLevel(j) = 1.0
        Next

        For k = 0 To _boundaryFixed.Length - 1
            _boundaryFixed(k) = False
            _boundaryOverride(k) = 0.0
        Next
    End Sub

#End Region

#Region "前向计算"

    ''' <summary>
    ''' 通量读取头：<c>v = e ⊙ σ(Wv·[h;u] + bv)</c>
    ''' </summary>
    ''' <remarks>
    ''' 不可逆反应的饱和因子取 σ(·) ∈ (0,1)，通量恒为非负；
    ''' 可逆反应取 2σ(·)−1 ∈ (−1,1)，允许通量为负（表示逆向流动）。
    ''' </remarks>
    ''' <param name="h">隐藏状态（代谢物浓度）</param>
    ''' <param name="u">网络输入（酶表达量 + 边界浓度）</param>
    ''' <returns>各反应的通量，长度 = 反应数</returns>
    Public Function ComputeFlux(h As Tensor, u As Tensor) As Tensor
        Dim r = ReactionCount
        Dim x = ConcatStateInput(h, u)
        Dim z = x.MatMul(_FluxWeight)
        Dim v = New Tensor(r)

        For j = 0 To r - 1
            Dim sat = 1.0 / (1.0 + std.Exp(-Clamp(z(0, j) + _FluxBias(j))))

            If Graph.Reversible(j) Then
                v(j) = u(j) * (2.0 * sat - 1.0)
            Else
                v(j) = u(j) * sat
            End If
        Next

        Return v
    End Function

    ''' <summary>
    ''' 通量读取头的反向传播：累加 Wv / bv 梯度，并返回对隐藏状态 h 的伴随向量
    ''' </summary>
    ''' <param name="adjV">对通量的梯度 dL/dv</param>
    ''' <param name="h">前向时的隐藏状态</param>
    ''' <param name="u">前向时的网络输入</param>
    ''' <returns>对隐藏状态 h 的梯度 dL/dh</returns>
    Public Function FluxBackward(adjV As Tensor, h As Tensor, u As Tensor) As Tensor
        Dim r = ReactionCount
        Dim m = MetaboliteCount
        Dim nIn = InputSize
        Dim x = ConcatStateInput(h, u)
        Dim z = x.MatMul(_FluxWeight)
        Dim dz = New Double(r - 1) {}

        For j = 0 To r - 1
            Dim sat = 1.0 / (1.0 + std.Exp(-Clamp(z(0, j) + _FluxBias(j))))
            ' 可逆反应用 2σ−1，其导数为 2σ(1−σ)
            Dim dSat = If(Graph.Reversible(j), 2.0 * sat * (1.0 - sat), sat * (1.0 - sat))

            ' v_j = e_j · gsat(g_j)
            dz(j) = adjV(j) * u(j) * dSat
            _FluxBiasGradient(j) += dz(j)
        Next

        For j = 0 To r - 1
            For i = 0 To m - 1
                _FluxWeightGradient(i, j) += h(i) * dz(j)
            Next
            For i = 0 To nIn - 1
                _FluxWeightGradient(m + i, j) += u(i) * dz(j)
            Next
        Next

        Dim adjH = New Tensor(m)

        For i = 0 To m - 1
            Dim acc As Double = 0.0

            For j = 0 To r - 1
                acc += _FluxWeight(i, j) * dz(j)
            Next

            adjH(i) = acc
        Next

        Return adjH
    End Function

    Private Function ConcatStateInput(h As Tensor, u As Tensor) As Tensor
        Dim x = New Tensor(1, MetaboliteCount + InputSize)

        For i = 0 To MetaboliteCount - 1
            x(0, i) = h(i)
        Next
        For i = 0 To InputSize - 1
            x(0, MetaboliteCount + i) = u(i)
        Next

        Return x
    End Function

    Private Shared Function Clamp(v As Double) As Double
        If v > 30.0 Then Return 30.0
        If v < -30.0 Then Return -30.0
        Return v
    End Function

#End Region

#Region "模拟"

    ''' <summary>
    ''' 在给定初始状态、酶程序与边界条件下自由运行模拟
    ''' </summary>
    ''' <param name="h0">初始代谢物浓度（长度 m）</param>
    ''' <param name="enzymeSeries">酶表达序列，形状 (T × r)</param>
    ''' <param name="boundarySeries">边界代谢物浓度序列，形状 (T × nB)</param>
    ''' <param name="times">时间网格（可不规则，需严格单调递增）</param>
    ''' <returns>浓度 / 通量 / τ^sys 三条轨迹</returns>
    Public Function Simulate(h0 As Tensor, enzymeSeries As Tensor, boundarySeries As Tensor,
                             times As Double()) As MetabolicTrajectory
        ' 注意：VB 不区分大小写，标量不要命名为 T 以免与循环变量 t 冲突
        Dim steps = times.Length
        Dim m = MetaboliteCount
        Dim r = ReactionCount

        If h0.Length <> m Then
            Throw New ArgumentException($"初始状态维度不匹配：期望 {m}，实际 {h0.Length}")
        End If
        If enzymeSeries.Shape(0) <> steps Then
            Throw New ArgumentException($"酶序列长度 {enzymeSeries.Shape(0)} 与时间网格 {steps} 不一致")
        End If

        Dim conc = New Tensor(steps, m)
        Dim flux = New Tensor(steps, r)
        Dim tau = New Tensor(steps, m)

        Dim cell = Liquid.LiquidLayer.Cells(0)

        Liquid.ResetState()
        cell.SetState(h0)

        For t = 0 To steps - 1
            If t > 0 Then
                Dim dt = times(t) - times(t - 1)

                If dt <= 0 Then
                    Throw New ArgumentException($"时间网格必须严格单调递增，索引 {t} 处出现 dt={dt}")
                End If
            End If

            Dim u = BuildInput(RowOf(enzymeSeries, t), RowOf(boundarySeries, t))
            Dim h = cell.State
            Dim v = ComputeFlux(h, u)
            Dim sysTau = cell.GetSystemTau(h, u)

            For i = 0 To m - 1
                conc(t, i) = h(i)
                tau(t, i) = sysTau(i)
            Next
            For j = 0 To r - 1
                flux(t, j) = v(j)
            Next

            ' 用当前时刻的驱动外推下一时刻（区间内部会按 MaxSubStep 细分积分）
            If t < steps - 1 Then
                Call StepInterval(u, times(t + 1) - times(t))
            End If
        Next

        Return New MetabolicTrajectory With {
            .Times = times,
            .MetaboliteIds = Graph.InternalIds,
            .ReactionIds = Graph.ReactionIds,
            .Concentrations = conc,
            .Fluxes = flux,
            .Tau = tau
        }
    End Function

    ''' <summary>
    ''' 把状态从 t 推进到 t+span（区间内部按 <see cref="MaxSubStep"/> 细分）
    ''' </summary>
    ''' <param name="u">该区间内保持恒定的驱动输入</param>
    ''' <param name="span">区间长度</param>
    ''' <returns>实际执行的 ODE 步数（反向传播时需要对同样多步做逆序回传）</returns>
    Public Function StepInterval(u As Tensor, span As Double) As Integer
        If span <= 0 Then
            Throw New ArgumentException($"时间区间长度必须为正，当前为 {span}")
        End If

        Dim n As Integer = CInt(std.Ceiling(span / std.Max(0.0000001, MaxSubStep)))

        If n < 1 Then n = 1
        If n > MaxSubStepsPerInterval Then n = MaxSubStepsPerInterval

        Dim dt = span / n

        For k = 1 To n
            Call Liquid.Forward(u, dt)
        Next

        Return n
    End Function

    ''' <summary>
    ''' 单个观测区间内允许的最大子步数（防御性上限，避免极端时间尺度拖垮训练）
    ''' </summary>
    Public Property MaxSubStepsPerInterval As Integer = 512

    Private Function RowOf(mat As Tensor, row As Integer) As Tensor
        Dim width = mat.Shape(1)
        Dim v = New Tensor(width)

        For j = 0 To width - 1
            v(j) = mat(row, j)
        Next

        Return v
    End Function

#End Region

#Region "参数与梯度管理"

    ''' <summary>通量读取头的 (参数名, 参数, 梯度) 配对（LNN 侧参数由 LNNTrainer 管理）</summary>
    Public Function GetFluxHeadPairs() As List(Of ParameterPair)
        Return New List(Of ParameterPair) From {
            New ParameterPair("flux_weight", _FluxWeight, _FluxWeightGradient),
            New ParameterPair("flux_bias", _FluxBias, _FluxBiasGradient)
        }
    End Function

    ''' <summary>清零通量读取头的梯度</summary>
    Public Sub ZeroFluxGradients()
        For Each pair In GetFluxHeadPairs()
            Dim g = pair.Gradient

            For i = 0 To g.Length - 1
                g(i) = 0.0
            Next
        Next
    End Sub

    ''' <summary>模型全部可训练参数的数量</summary>
    Public Function GetParameterCount() As Integer
        Dim count = Liquid.GetParameterCount()

        For Each pair In GetFluxHeadPairs()
            count += pair.Value.Length
        Next

        Return count
    End Function

#End Region

    Public Overrides Function ToString() As String
        Return $"MetabolicLiquidNetwork[{Mode}] {Graph}, 参数量={GetParameterCount()}"
    End Function

End Class
