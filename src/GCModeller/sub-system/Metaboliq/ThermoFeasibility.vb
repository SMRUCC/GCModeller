Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

''' <summary>
''' 热力学可行性项（λ2）的配置
''' </summary>
''' <remarks>
''' 该项的判据是：<strong>有净通量的反应必须沿浓度梯度方向运行</strong>。
''' 无量纲推动力定义为 <c>dg_j = ln(Q_j/Keq_j) = ΔG_j/RT</c>，
''' 反应商 <c>ln Q_j = Σ_i S(i,j)·ln c_i</c> 由模型自身预测的浓度算出。
''' </remarks>
Public Class ThermoConfig

    ''' <summary>
    ''' RT（气体常数 × 温度）。本项中 ΔG 以 RT 为单位无量纲化（dg = ΔG/RT），
    ''' 因此 RT 只在需要输出有量纲的 ΔG 时才用到，默认 1.0。
    ''' </summary>
    Public Property RT As Double = 1.0

    ''' <summary>
    ''' 活跃度门控尺度：<c>â_j = tanh(v_j / FluxScale)</c>。
    ''' 越小则"判定为有通量"的阈值越低。â 的引入使**无通量的反应完全不受约束**——
    ''' 这是物理上正确的（热力学方向性只约束真正在流的反应），
    ''' 同时天然规避了痕量代谢物（c → 1e-6）带来的数值噪声。
    ''' </summary>
    Public Property FluxScale As Double = 0.05

    ''' <summary>
    ''' 物理浓度下限。防止 <c>ln c → −∞</c>。默认 1e-3 兼顾两点：
    ''' <c>ln c ≥ −6.9</c> 不至于让推动力失真，且链式因子 <c>(c+1)/c ≤ 1001</c> 不至于让梯度爆炸。
    ''' </summary>
    Public Property MinConcentration As Double = 0.001

    ''' <summary>
    ''' de-normalize 时指数参数的上界，防止 <c>exp</c> 溢出
    ''' </summary>
    Public Property MaxLogArgument As Double = 50.0

    ''' <summary>
    ''' 推动力的钳制范围 <c>dg ∈ [−DFmax, +DFmax]</c>。
    ''' 痕量代谢物会把 <c>ln Q</c> 放大到数十，平方后将主导整个损失；钳制后梯度按 straight-through 置 0。
    ''' </summary>
    Public Property MaxDrivingForce As Double = 20.0

    ''' <summary>
    ''' 反归一化链式因子 <c>(c+1)/c</c> 的上界，兜住 <c>c → 0</c> 处的梯度爆炸
    ''' </summary>
    Public Property MaxChainFactor As Double = 1000.0

    ''' <summary>
    ''' 不可逆反应在 <c>keq</c> 数据缺失时使用的"有效"平衡常数。
    ''' 真值速率律对不可逆反应没有反向项 ⇒ Keq 实为 ∞；取有限大值是为了让 ΔG 可计算，
    ''' 效果上等价于"强烈偏向正向"，因此这类反应几乎不会触发惩罚（其方向本已由结构硬保证）。
    ''' </summary>
    Public Property KeqIrreversible As Double = 1000.0

End Class

''' <summary>
''' 热力学可行性项所需的静态上下文：反归一化参数 + Keq 先验 + 配置
''' </summary>
''' <remarks>
''' 一次性构建、训练/评估/预测全程共享。注意 <c>TimeSeriesMatrix.Reorder</c> 只返回数据张量、
''' 不同步 RowMeans/RowStds，因此这里必须按 id 从原始矩阵查统计量。
''' </remarks>
Public Class ThermoContext

    Public ReadOnly Property Config As ThermoConfig

    ''' <summary>内部代谢物的反归一化均值，按 <c>graph.InternalIds</c> 顺序</summary>
    Public ReadOnly Property InternalMeans As Double()

    ''' <summary>内部代谢物的反归一化标准差，按 <c>graph.InternalIds</c> 顺序</summary>
    Public ReadOnly Property InternalStds As Double()

    ''' <summary>边界代谢物的反归一化均值，按 <c>graph.BoundaryIds</c> 顺序（边界是给定值，不回传梯度）</summary>
    Public ReadOnly Property BoundaryMeans As Double()

    ''' <summary>边界代谢物的反归一化标准差，按 <c>graph.BoundaryIds</c> 顺序</summary>
    Public ReadOnly Property BoundaryStds As Double()

    ''' <summary>各反应的平衡常数，按 <c>graph.ReactionIds</c> 顺序</summary>
    Public ReadOnly Property Keq As Double()

    ''' <summary>ln(Keq)，预计算以避免热路径上反复取对数</summary>
    Public ReadOnly Property LogKeq As Double()

    ''' <summary>缺失 Keq 的反应数量（这些反应退化为使用 <see cref="ThermoConfig.KeqIrreversible"/> 或 1.0）</summary>
    Public ReadOnly Property MissingKeqCount As Integer

    Public Sub New(internalMeans As Double(), internalStds As Double(),
                   boundaryMeans As Double(), boundaryStds As Double(),
                   keq As Double(), Optional missingKeqCount As Integer = 0,
                   Optional config As ThermoConfig = Nothing)
        Me.InternalMeans = internalMeans
        Me.InternalStds = internalStds
        Me.BoundaryMeans = boundaryMeans
        Me.BoundaryStds = boundaryStds
        Me.Keq = keq
        Me.MissingKeqCount = missingKeqCount
        Me.Config = If(config, New ThermoConfig())

        Dim logK(keq.Length - 1) As Double

        For j = 0 To keq.Length - 1
            logK(j) = std.Log(std.Max(1.0E-12, keq(j)))
        Next

        Me.LogKeq = logK
    End Sub

    ''' <summary>
    ''' 由代谢组时序矩阵与网络图构建热力学上下文
    ''' </summary>
    ''' <param name="metabolome">已做过 log1p+z-score 归一化的代谢组矩阵（内部与边界代谢物都出自这里）</param>
    ''' <param name="graph">代谢网络拓扑</param>
    ''' <param name="keqById">反应 id → Keq；为 Nothing 时全部退化为 1.0（约束退化为"净通量须沿质量作用比梯度方向"）</param>
    ''' <param name="config">热力项配置</param>
    Public Shared Function FromMetabolome(metabolome As TimeSeriesMatrix,
                                          graph As MetabolicNetworkGraph,
                                          Optional keqById As Dictionary(Of String, Double) = Nothing,
                                          Optional config As ThermoConfig = Nothing) As ThermoContext
        Dim cfg = If(config, New ThermoConfig())
        Dim missing As Integer = 0

        Dim intMeans = StatsOf(metabolome, graph.InternalIds, means:=True)
        Dim intStds = StatsOf(metabolome, graph.InternalIds, means:=False)
        Dim bndMeans = StatsOf(metabolome, graph.BoundaryIds, means:=True)
        Dim bndStds = StatsOf(metabolome, graph.BoundaryIds, means:=False)

        Dim ids = graph.ReactionIds
        Dim keq = New Double(ids.Length - 1) {}

        For j = 0 To ids.Length - 1
            If keqById IsNot Nothing AndAlso keqById.ContainsKey(ids(j)) Then
                keq(j) = std.Max(1.0E-12, keqById(ids(j)))
            ElseIf graph.Reversible(j) Then
                ' 可逆反应没有 Keq 数据时取 1.0
                keq(j) = 1.0
                missing += 1
            Else
                keq(j) = cfg.KeqIrreversible
            End If
        Next

        Return New ThermoContext(intMeans, intStds, bndMeans, bndStds, keq, missing, cfg)
    End Function

    ''' <summary>
    ''' 按 id 列表取出反归一化统计量；未归一化、或矩阵中查不到该 id 时退化为恒等变换
    ''' </summary>
    Private Shared Function StatsOf(m As TimeSeriesMatrix, ids As String(), means As Boolean) As Double()
        Dim out = New Double(ids.Length - 1) {}
        Dim identity = m.RowMeans Is Nothing OrElse m.RowStds Is Nothing OrElse m.Normalization <> "log1p+zscore"

        For i = 0 To ids.Length - 1
            If identity Then
                out(i) = If(means, 0.0, 1.0)
                Continue For
            End If

            Dim src = Array.IndexOf(m.FeatureIds, ids(i))

            If src < 0 Then
                out(i) = If(means, 0.0, 1.0)
            Else
                out(i) = If(means, m.RowMeans(src), std.Max(1.0E-8, m.RowStds(src)))
            End If
        Next

        Return out
    End Function

End Class

''' <summary>
''' 单步热力学评估的缓存，供反向传播复用（避免重复的反归一化与对数运算）
''' </summary>
Public Class ThermoStep

    ''' <summary>物理浓度，按 <c>graph.MetaboliteIds</c> 顺序</summary>
    Public physConc As Double()
    ''' <summary>ln(c)，已施加浓度下限</summary>
    Public lnC As Double()
    ''' <summary>ln Q_j</summary>
    Public lnQ As Double()
    ''' <summary>钳制后的无量纲推动力 dg_j = ΔG_j/RT</summary>
    Public dg As Double()
    ''' <summary>活跃度门控 â_j = tanh(v_j/FluxScale)</summary>
    Public ahat As Double()
    ''' <summary>违反量 w_j = max(0, â_j·dg_j)</summary>
    Public w As Double()
    ''' <summary>该反应的 dg 是否触及钳制边界（触及处梯度按 straight-through 置 0）</summary>
    Public clamped As Boolean()
    ''' <summary>Σ_j w_j²（未除以 T·r，也未乘 λ2）</summary>
    Public Penalty As Double
    ''' <summary>w_j &gt; 0 的反应数</summary>
    Public ActiveCount As Integer
    ''' <summary>最大的违反量</summary>
    Public Worst As Double
    ''' <summary>不可逆反应中出现负通量的条数（只读诊断；结构应保证恒为 0）</summary>
    Public NegativeIrreversibleCount As Integer

End Class

''' <summary>热力学项反向传播得到的两个 adjoint</summary>
Public Class ThermoAdjoint

    ''' <summary>对归一化浓度输出 ĉ 的梯度（长度 = 内部代谢物数）</summary>
    Public ReadOnly Property dOut As Tensor
    ''' <summary>对反应通量 v 的梯度（长度 = 反应数）</summary>
    Public ReadOnly Property adjV As Tensor

    Public Sub New(dOut As Tensor, adjV As Tensor)
        Me.dOut = dOut
        Me.adjV = adjV
    End Sub

End Class

''' <summary>
''' 热力学可行性项：净通量必须沿浓度梯度方向
''' </summary>
''' <remarks>
''' 数学定义：
''' <code>
'''   c_i    = exp(σ_i·ĉ_i + m_i) − 1              （log1p+z-score 逆变换）
'''   ln Q_j = Σ_i S(i,j)·ln c_i
'''   dg_j   = clamp(ln Q_j − ln Keq_j, ±DFmax)    （= ΔG_j/RT，无量纲）
'''   â_j    = tanh(v_j / FluxScale)
'''   w_j    = max(0, â_j·dg_j)
'''   L      = λ2 · Σ_t Σ_j w_j² / (T·r)
''' </code>
''' 梯度分两路汇入既有链路：对通量的进 <c>adjV</c>；对浓度的进 <c>dOut</c>，
''' 与 MSE 梯度相加后统一走 <c>BackwardOutput → BackwardLiquid</c>，因此**不改动 LNN 内核**。
''' </remarks>
Public Class ThermoFeasibility

    Private ReadOnly _graph As MetabolicNetworkGraph
    Private ReadOnly _ctx As ThermoContext
    Private ReadOnly _cfg As ThermoConfig
    Private ReadOnly _nAll As Integer
    Private ReadOnly _nInt As Integer
    Private ReadOnly _nRxn As Integer
    Private ReadOnly _nBnd As Integer
    Private ReadOnly _intToAll As Integer()
    Private ReadOnly _bndToAll As Integer()
    Private ReadOnly _logKeq As Double()
    Private ReadOnly _reversible As Boolean()

    Public Sub New(graph As MetabolicNetworkGraph, context As ThermoContext)
        If graph Is Nothing Then Throw New ArgumentNullException(NameOf(graph))
        If context Is Nothing Then Throw New ArgumentNullException(NameOf(context))

        _graph = graph
        _ctx = context
        _cfg = context.Config
        _logKeq = context.LogKeq
        _nAll = graph.MetaboliteIds.Length
        _nInt = graph.MetaboliteCount
        _nRxn = graph.ReactionCount
        _nBnd = graph.BoundaryCount
        _reversible = graph.Reversible

        If _logKeq.Length <> _nRxn Then
            Throw New ArgumentException($"Keq 维度（{_logKeq.Length}）与反应数（{_nRxn}）不一致")
        End If

        _intToAll = New Integer(_nInt - 1) {}
        For i = 0 To _nInt - 1
            _intToAll(i) = graph.ToMetaboliteIndex(i)

            If _intToAll(i) < 0 Then
                Throw New InvalidOperationException($"内部代谢物 {graph.InternalIds(i)} 在全代谢物表中查不到索引")
            End If
        Next

        _bndToAll = New Integer(_nBnd - 1) {}
        For k = 0 To _nBnd - 1
            _bndToAll(k) = graph.IndexOfMetabolite(graph.BoundaryIds(k))

            If _bndToAll(k) < 0 Then
                Throw New InvalidOperationException($"边界代谢物 {graph.BoundaryIds(k)} 在全代谢物表中查不到索引")
            End If
        Next
    End Sub

    ''' <summary>底层网络拓扑</summary>
    Public ReadOnly Property Graph As MetabolicNetworkGraph
        Get
            Return _graph
        End Get
    End Property

#Region "反归一化"

    ''' <summary>归一化浓度 → 物理浓度（内部代谢物），返回长度 = 内部代谢物数</summary>
    Public Function ToPhysicalInternal(outNorm As Tensor) As Double()
        Return DeNormalize(ToArray(outNorm), _ctx.InternalMeans, _ctx.InternalStds)
    End Function

    ''' <summary>归一化浓度 → 物理浓度（边界代谢物），返回长度 = 边界代谢物数</summary>
    Public Function ToPhysicalBoundary(boundaryNorm As Tensor) As Double()
        Return DeNormalize(ToArray(boundaryNorm), _ctx.BoundaryMeans, _ctx.BoundaryStds)
    End Function

    ''' <summary>
    ''' log1p+z-score 的逆变换：<c>c = exp(σ·x + m) − 1</c>
    ''' </summary>
    Private Function DeNormalize(x As Double(), means As Double(), stds As Double()) As Double()
        Dim n = x.Length
        Dim out = New Double(n - 1) {}

        For i = 0 To n - 1
            Dim u As Double = x(i) * stds(i) + means(i)

            ' 指数参数保护：上界防溢出，下界由 MinConcentration 兜住
            If u > _cfg.MaxLogArgument Then u = _cfg.MaxLogArgument
            If u < -_cfg.MaxLogArgument Then u = -_cfg.MaxLogArgument

            Dim c As Double = std.Exp(u) - 1.0

            If c < _cfg.MinConcentration Then
                c = _cfg.MinConcentration
            End If

            out(i) = c
        Next

        Return out
    End Function

    Private Shared Function ToArray(t As Tensor) As Double()
        Dim out = New Double(t.Length - 1) {}

        For i = 0 To t.Length - 1
            out(i) = t(i)
        Next

        Return out
    End Function

#End Region

#Region "前向"

    ''' <summary>
    ''' 用模型预测的归一化浓度评估热力学违反度
    ''' </summary>
    ''' <param name="outNorm">模型的浓度读出 ĉ（归一化空间，长度 = 内部代谢物数）</param>
    ''' <param name="boundaryNorm">边界代谢物浓度（归一化空间，长度 = 边界代谢物数）</param>
    ''' <param name="v">反应通量（长度 = 反应数）</param>
    Public Function Evaluate(outNorm As Tensor, boundaryNorm As Tensor, v As Tensor) As ThermoStep
        Dim cInt = ToPhysicalInternal(outNorm)
        Dim cBnd = ToPhysicalBoundary(boundaryNorm)
        Dim physConc = New Double(_nAll - 1) {}

        For i = 0 To _nInt - 1
            physConc(_intToAll(i)) = cInt(i)
        Next
        For k = 0 To _nBnd - 1
            physConc(_bndToAll(k)) = cBnd(k)
        Next

        Return EvaluatePhysical(physConc, ToArray(v))
    End Function

    ''' <summary>
    ''' 直接用物理浓度评估（用于真值数据的正确性佐证）
    ''' </summary>
    ''' <param name="physConc">物理浓度，按 <c>graph.MetaboliteIds</c> 顺序</param>
    ''' <param name="v">反应通量，按 <c>graph.ReactionIds</c> 顺序</param>
    Public Function EvaluatePhysical(physConc As Double(), v As Double()) As ThermoStep
        If physConc.Length <> _nAll Then
            Throw New ArgumentException($"物理浓度维度不匹配：期望 {_nAll}，实际 {physConc.Length}")
        End If
        If v.Length <> _nRxn Then
            Throw New ArgumentException($"通量维度不匹配：期望 {_nRxn}，实际 {v.Length}")
        End If

        ' ln c（施加浓度下限）
        Dim lnC = New Double(_nAll - 1) {}
        For i = 0 To _nAll - 1
            Dim c = physConc(i)
            If c < _cfg.MinConcentration Then c = _cfg.MinConcentration
            lnC(i) = std.Log(c)
        Next

        ' ln Q_j = Σ_i S(i,j)·ln c_i
        Dim S = _graph.Stoichiometry
        Dim lnQ = New Double(_nRxn - 1) {}

        For j = 0 To _nRxn - 1
            Dim acc As Double = 0.0

            For i = 0 To _nAll - 1
                Dim sij = S(i, j)

                If sij <> 0.0 Then
                    acc += sij * lnC(i)
                End If
            Next

            lnQ(j) = acc
        Next

        Dim dg = New Double(_nRxn - 1) {}
        Dim ahat = New Double(_nRxn - 1) {}
        Dim w = New Double(_nRxn - 1) {}
        Dim clamped = New Boolean(_nRxn - 1) {}
        Dim penalty As Double = 0.0
        Dim active As Integer = 0
        Dim worst As Double = 0.0
        Dim negIrrev As Integer = 0

        For j = 0 To _nRxn - 1
            ' 只读诊断：不可逆反应在结构上应当恒有 v ≥ 0
            If Not _reversible(j) AndAlso v(j) < -1.0E-12 Then
                negIrrev += 1
            End If

            Dim raw = lnQ(j) - _logKeq(j)

            If raw > _cfg.MaxDrivingForce Then
                raw = _cfg.MaxDrivingForce
                clamped(j) = True
            ElseIf raw < -_cfg.MaxDrivingForce Then
                raw = -_cfg.MaxDrivingForce
                clamped(j) = True
            End If

            dg(j) = raw
            ahat(j) = std.Tanh(v(j) / _cfg.FluxScale)

            Dim vio = ahat(j) * raw

            If vio > 0.0 Then
                w(j) = vio
                penalty += vio * vio
                active += 1

                If vio > worst Then worst = vio
            End If
        Next

        Return New ThermoStep With {
            .physConc = physConc,
            .lnC = lnC,
            .lnQ = lnQ,
            .dg = dg,
            .ahat = ahat,
            .w = w,
            .clamped = clamped,
            .Penalty = penalty,
            .ActiveCount = active,
            .Worst = worst,
            .NegativeIrreversibleCount = negIrrev
        }
    End Function

#End Region

#Region "反向"

    ''' <summary>
    ''' 求热力学项的两个 adjoint
    ''' </summary>
    ''' <param name="cache"><see cref="Evaluate"/> 产出的缓存</param>
    ''' <param name="steps">序列长度 T（用于 1/(T·r) 的归一化）</param>
    ''' <param name="lambda">λ2</param>
    Public Function Backward(cache As ThermoStep, steps As Integer, lambda As Double) As ThermoAdjoint
        Dim dOut = New Tensor(_nInt)
        Dim adjV = New Tensor(_nRxn)

        If lambda <= 0.0 OrElse cache Is Nothing Then
            Return New ThermoAdjoint(dOut, adjV)
        End If

        ' L = λ2·Σ_t Σ_j w_j²/(T·r)  ⇒  ∂L/∂w_j = s·w_j，s = 2λ2/(T·r)
        Dim s = 2.0 * lambda / (std.Max(1, steps) * _nRxn)

        ' ---- 对通量：∂L/∂v_j = s·w_j·dg_j·(1 − â_j²)/FluxScale ----
        For j = 0 To _nRxn - 1
            If cache.w(j) <= 0.0 Then Continue For

            Dim dAhat = s * cache.w(j) * cache.dg(j)
            adjV(j) += dAhat * (1.0 - cache.ahat(j) * cache.ahat(j)) / _cfg.FluxScale
        Next

        ' ---- 对 ln c：∂L/∂ln c_i = Σ_j s·w_j·â_j·S(i,j)（钳制处梯度置 0）----
        Dim S = _graph.Stoichiometry
        Dim dLnC = New Double(_nAll - 1) {}

        For j = 0 To _nRxn - 1
            If cache.w(j) <= 0.0 OrElse cache.clamped(j) Then Continue For

            Dim dDg = s * cache.w(j) * cache.ahat(j)

            For i = 0 To _nAll - 1
                Dim sij = S(i, j)

                If sij <> 0.0 Then
                    dLnC(i) += dDg * sij
                End If
            Next
        Next

        ' ---- 回传到归一化浓度空间：∂ln c/∂ĉ = σ·(c+1)/c ----
        For i = 0 To _nInt - 1
            Dim ai = _intToAll(i)
            Dim c = cache.physConc(ai)

            If c < _cfg.MinConcentration Then c = _cfg.MinConcentration

            Dim chain = (c + 1.0) / c

            If chain > _cfg.MaxChainFactor Then chain = _cfg.MaxChainFactor

            dOut(i) = dLnC(ai) * _ctx.InternalStds(i) * chain
        Next

        Return New ThermoAdjoint(dOut, adjV)
    End Function

#End Region

End Class
