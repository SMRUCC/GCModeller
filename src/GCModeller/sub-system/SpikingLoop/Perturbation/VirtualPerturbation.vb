' ============================================================================
' VirtualPerturbation.vb — readme 四 · 虚拟扰动实验
'
' 核心思想（readme 七的总结）：模型训练完成后，只需在<b>输入层</b>修改特定基因的
' 电流，网络就会自动沿 TF→Target 有向突触把扰动级联传播下去——这正是 SNN
' "事件驱动 + 原生时序"相对于静态回归模型的独到之处。
'
' 单步 rollout（每个决策步）：
'   1. I_ext = ENCODE(当前表达估计)              把当前状态编码为持续电流
'   2. I_ext = INJECT(I_ext, specs)              在起始若干内层时间步注入扰动
'   3. S, u = LIF_FORWARD(I_ext, u_state, W, A)  递归演化（扰动沿突触级联传播）
'   4. state = DECODE(S, u)                      解码为新的表达状态，作为下一步输入
'   5. u_state = H_last                          膜电位跨步保持（SNN 有状态，与 ANN 不同）
'
' 为什么扰动要在每个决策步重复注入：
'   每当状态被重新编码，被敲除基因的输入电流就会"复活"，被过表达基因的增益也会失效。
'   因此模型一次持续扰动 = 每个决策步都重新施加同一扰动（KO 在第二步之后通常已自然归零，
'   重复注入是无副作用的；OE 则需要持续注入才能体现"过表达"的语义）。
'
' ---- 闭环协议（PerturbationCarryMembrane）------------------------------------
' 训练协议是"给定恒流 X 持续 SimulationSteps 步，从膜电位 0.5·X 出发 → 预测下一时刻"。
' 闭环 rollout 若把膜电位也跨步携带（u_state = H_last），等效增益会接近
' 1 + β·(1−S) > 1：因为本步电流又等于上一步的解码输出，膜电位被反复累加，
' 状态会在十几步内被推到表达区间的两端，整条轨迹被边界裁剪（ClampRatio → 1）而失去信息量。
' 因此默认（PerturbationCarryMembrane = False）在<b>每个决策步都按训练协议
' 重新初始化膜电位</b>，使闭环成为"由同一张学到的一步预测映射反复迭代"的良态系统，
' 其不动点即模型预测的稳态表达谱。
' 若确实需要"原生有状态动力学"（例如研究振荡/持续发放），可把该开关置为 True，
' 并同时用 <see cref="PerturbationTrajectory.ClampRatio"/> 监控是否发散。
'
' 单基因扫描（Perturb-seq 模拟）：
'   对候选基因逐个做 KD/KO，统计"最终表达相对基线的偏移向量"，
'   按影响强度排序即可得到下游效应基因与关键调控节点（readme 四 的 batch_perturbation_scan）。
' ============================================================================

Imports System.Text
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.BNLearn.Intervention
Imports SMRUCC.genomics.Analysis.SpikingLoop.Model
Imports std = System.Math

Namespace Perturbation

    ''' <summary>单个扰动方案下的时空表达轨迹（形状 T_sim × N）</summary>
    Public Class PerturbationTrajectory

        ''' <summary>基因名（与轨迹列一一对应）</summary>
        Public Property GeneNames As String()

        ''' <summary>本次扰动方案</summary>
        Public Property Specs As PerturbationSpec()

        ''' <summary>扰动后的表达轨迹 [T_sim][N]（归一化尺度）</summary>
        Public Property Trajectory As Double()()

        ''' <summary>基线表达（扰动前的状态，[N]）</summary>
        Public Property Baseline As Double()

        ''' <summary>
        ''' 原始表达尺度的轨迹 [T_sim][N]；未提供反变换器时为 Nothing。
        ''' </summary>
        Public Property Denormalized As Double()()

        ''' <summary>被裁剪回 [min,max] 的单元数（数值发散时的诊断信号）</summary>
        Public Property ClampCount As Integer

        ''' <summary>
        ''' 裁剪比例 = 裁剪次数 / (时间步 × 基因数)。
        ''' 接近 0 表示轨迹稳定；接近 1 表示闭环外推发散、轨迹被压到边界而失去信息量，
        ''' 此时应降低 <c>PerturbationSteps</c>、提高 <c>PriorRegAlpha</c>，
        ''' 或改用带扰动数据训练模型。
        ''' </summary>
        Public ReadOnly Property ClampRatio As Double
            Get
                Dim cells = NumSteps * CDbl(NumGenes)
                If cells <= 0.0 Then Return 0.0
                Return ClampCount / cells
            End Get
        End Property

        ''' <summary>轨迹是否发生明显发散（裁剪比例 &gt; 25%）</summary>
        Public ReadOnly Property Diverged As Boolean
            Get
                Return ClampRatio > 0.25
            End Get
        End Property

        ''' <summary>扰动是否被真实注入（扰动基因是否落在网络中）</summary>
        Public Property Applied As Boolean

        Public ReadOnly Property NumSteps As Integer
            Get
                Return If(Trajectory Is Nothing, 0, Trajectory.Length)
            End Get
        End Property

        Public ReadOnly Property NumGenes As Integer
            Get
                Return If(GeneNames Is Nothing, 0, GeneNames.Length)
            End Get
        End Property

        ''' <summary>扰动终点状态（轨迹最后一步）</summary>
        Public Function FinalState() As Double()
            If Trajectory Is Nothing OrElse Trajectory.Length = 0 Then Return New Double() {}
            Return Trajectory(Trajectory.Length - 1)
        End Function

        ''' <summary>终点相对基线的表达偏移 Δg = expr_end − expr_baseline</summary>
        Public Function [Delta]() As Double()
            Dim final = FinalState()
            Dim d(final.Length - 1) As Double
            For i = 0 To d.Length - 1
                d(i) = final(i) - Baseline(i)
            Next
            Return d
        End Function

        ''' <summary>
        ''' 每个基因的响应起始时刻（首次 |Δ| 超过阈值的步号）；−1 表示全程未响应。
        ''' 用于检查 readme 五.4 的"上游先响应、下游延迟响应"结构。
        ''' </summary>
        Public Function ResponseOnset(Optional threshold As Double = 0.01) As Integer()
            Dim n = NumGenes
            Dim onset(n - 1) As Integer
            For g = 0 To n - 1
                onset(g) = -1
            Next

            For t = 0 To NumSteps - 1
                For g = 0 To n - 1
                    If onset(g) < 0 AndAlso std.Abs(Trajectory(t)(g) - Baseline(g)) > threshold Then
                        onset(g) = t
                    End If
                Next
            Next

            Return onset
        End Function

        ''' <summary>受影响最强烈的 k 个基因（按 |Δ| 降序，含非直接靶基因）</summary>
        Public Function TopAffected(Optional k As Integer = 10) As List(Of (gene As String, delta As Double))
            Dim d = [Delta]()
            Dim items As New List(Of (Integer, Double))()
            For i = 0 To d.Length - 1
                items.Add((i, std.Abs(d(i))))
            Next

            items.Sort(Function(a, b) b.Item2.CompareTo(a.Item2))

            Dim take = std.Min(k, items.Count)
            Dim result As New List(Of (gene As String, delta As Double))()
            For i = 0 To take - 1
                result.Add((GeneNames(items(i).Item1), d(items(i).Item1)))
            Next
            Return result
        End Function

        ''' <summary>把"扰动轨迹相对基线"渲染成 ASCII 强度图（前若干基因）</summary>
        Public Function TrajectoryText(Optional maxGenes As Integer = 6,
                                       Optional ramp As String = " ·:-=+*#%@") As String
            If Trajectory Is Nothing OrElse Trajectory.Length = 0 Then
                Return "(empty trajectory)"
            End If

            Dim genes = std.Min(maxGenes, NumGenes)
            Dim sb As New StringBuilder()
            For g = 0 To genes - 1
                Dim delta(NumSteps - 1) As Double
                Dim peak = 0.0
                For t = 0 To NumSteps - 1
                    delta(t) = Trajectory(t)(g) - Baseline(g)
                    peak = std.Max(peak, std.Abs(delta(t)))
                Next

                sb.Append($"    {GeneNames(g).PadRight(14)}|")
                For t = 0 To NumSteps - 1
                    Dim v = If(peak > 0.0, std.Abs(delta(t)) / peak, 0.0)
                    Dim idx = CInt(std.Round(v * (ramp.Length - 1)))
                    sb.Append(ramp(std.Max(0, std.Min(ramp.Length - 1, idx))))
                Next
                sb.AppendLine($"| Δmax={peak * If(delta(NumSteps - 1) >= 0.0, 1.0, -1.0):F4}")
            Next
            Return sb.ToString()
        End Function

        Public Overrides Function ToString() As String
            Dim spec = If(Specs Is Nothing OrElse Specs.Length = 0, "none", String.Join("+", Specs.Select(Function(s) s.ToString())))
            Return $"PerturbationTrajectory({spec}, T_sim={NumSteps}, N={NumGenes})"
        End Function

    End Class

    ''' <summary>批量扰动扫描结果（Perturb-seq 模拟）</summary>
    Public Class BatchPerturbationResult

        ''' <summary>基因名（与 <see cref="Delta"/> 的列一一对应）</summary>
        Public Property GeneNames As String()

        ''' <summary>被扰动的基因列表（与 <see cref="Delta"/> 的行一一对应）</summary>
        Public Property PerturbedGenes As String()

        ''' <summary>扰动模式</summary>
        Public Property Mode As InterventionMode

        ''' <summary>扰动强度</summary>
        Public Property Strength As Double

        ''' <summary>每个被扰动基因引起的表达偏移向量 Δ [nPerturbed][N]</summary>
        Public Property Delta As Double()()

        ''' <summary>每个被扰动基因的总体影响强度 Σ|Δ|（不含自身）</summary>
        Public Property ImpactScore As Double()

        ''' <summary>按影响强度降序排列的基因名</summary>
        Public Property RankedGenes As String()

        ''' <summary>与 <see cref="RankedGenes"/> 对应的影响强度</summary>
        Public Property RankedScore As Double()

        ''' <summary>每个被扰动基因的完整轨迹（可选保留，便于画时序曲线）</summary>
        Public Property Trajectories As Dictionary(Of String, PerturbationTrajectory)

        ''' <summary>取某个基因造成的下游效应最强的 k 个基因</summary>
        Public Function TopDownstream(gene As String, Optional k As Integer = 10) As List(Of (gene As String, delta As Double))
            Dim row = Array.IndexOf(PerturbedGenes, gene)
            If row < 0 Then Return New List(Of (gene As String, delta As Double))()

            Dim d = Delta(row)
            Dim items As New List(Of (Integer, Double))()
            For i = 0 To d.Length - 1
                If String.Equals(GeneNames(i), gene, StringComparison.OrdinalIgnoreCase) Then Continue For
                items.Add((i, std.Abs(d(i))))
            Next
            items.Sort(Function(a, b) b.Item2.CompareTo(a.Item2))

            Dim take = std.Min(k, items.Count)
            Dim result As New List(Of (gene As String, delta As Double))()
            For i = 0 To take - 1
                result.Add((GeneNames(items(i).Item1), d(items(i).Item1)))
            Next
            Return result
        End Function

        Public Overrides Function ToString() As String
            Dim count = If(PerturbedGenes Is Nothing, 0, PerturbedGenes.Length)
            Return $"BatchPerturbation({count} genes, mode={Mode}, strength={Strength:G3})"
        End Function

    End Class

    ''' <summary>虚拟扰动引擎：在输入层注入/移除电流，观察网络级联响应</summary>
    Public Class VirtualPerturbationEngine

        Private ReadOnly _model As SNNGRNModel
        Private ReadOnly _config As SpikingLoopConfig
        Private ReadOnly _trajectory As Data.PseudotimeResult

        ''' <summary>
        ''' 构建扰动引擎。
        ''' </summary>
        ''' <param name="model">已训练（或固定权重）的 SNN-GRN 模型</param>
        ''' <param name="trajectory">
        ''' 训练轨迹（可选）：提供后会把扰动结果反变换回原始表达尺度，便于与实验数据比较。
        ''' </param>
        Public Sub New(model As SNNGRNModel, Optional trajectory As Data.PseudotimeResult = Nothing)
            If model Is Nothing Then
                Throw New ArgumentNullException(NameOf(model))
            End If
            _model = model
            _config = model.Config
            _trajectory = trajectory
        End Sub

#Region "单方案扰动"

        ''' <summary>
        ''' 执行一次虚拟扰动实验，返回扰动后的时空表达轨迹。
        ''' </summary>
        ''' <param name="baseline">基线表达状态 [1, N]（归一化尺度，通常取 U_seq 的某个时间窗）</param>
        ''' <param name="specs">扰动方案（可多基因组合）</param>
        Public Function Run(baseline As Tensor, specs As PerturbationSpec()) As PerturbationTrajectory
            If baseline Is Nothing Then
                Throw New ArgumentNullException(NameOf(baseline))
            End If
            If baseline.Rank <> 2 OrElse baseline.Shape(1) <> _model.NumGenes Then
                Throw New ArgumentException(
                    $"基线表达形状应为 [1, {_model.NumGenes}]，实际 [{String.Join(",", baseline.Shape)}]")
            End If

            Dim genes = _model.Graph.GeneNames
            Dim resolved As New List(Of PerturbationSpec)()

            For Each spec In If(specs, New PerturbationSpec() {})
                Dim copy = spec.Copy()
                copy.ResolveIndex(genes)
                resolved.Add(copy)
            Next

            Dim n = _model.NumGenes
            Dim lo = _config.ExpressionMin
            Dim hi = _config.ExpressionMax
            Dim steps = std.Max(1, _config.PerturbationSteps)

            Dim baselineData = baseline.Data
            Dim state As New Tensor(CType(baselineData.Clone(), Double()), 1, n)
            Dim u0 = _model.MapToMembrane(state)

            Dim trajectory(steps - 1)() As Double
            Dim clampCount = 0

            For t = 0 To steps - 1
                ' 1. 编码 + 2. 注入扰动 + 3. 递归演化
                Dim currents = Inject(_model.EncodeCurrents(state), resolved, n)
                Dim output = _model.Forward(currents, u0)

                ' 4. 解码为新的表达状态（并裁剪回表达区间，防止发散）
                Dim rawState = output.YHat.Data
                Dim clamped(n - 1) As Double
                For i = 0 To n - 1
                    Dim v = rawState(i)
                    If v < lo Then
                        v = lo
                        clampCount += 1
                    ElseIf v > hi Then
                        v = hi
                        clampCount += 1
                    End If
                    clamped(i) = v
                Next

                trajectory(t) = clamped

                ' 松弛更新：η = 1 时即"严格迭代一步预测映射"（readme 四的原始语义）；
                ' η < 1 阻尼每步更新，等价于用更小的有效伪时间步长积分，抑制误差放大。
                Dim eta = _config.PerturbationRelaxation
                If eta >= 1.0 Then
                    state = Tensor.Wrap(clamped, 1, n)
                Else
                    Dim blended(n - 1) As Double
                    Dim previous = state.Data
                    For i = 0 To n - 1
                        blended(i) = previous(i) + eta * (clamped(i) - previous(i))
                    Next
                    state = Tensor.Wrap(blended, 1, n)
                End If

                ' 5. 膜电位处理（协议一致性，详见文件头的"闭环协议"说明）
                If _config.PerturbationCarryMembrane Then
                    ' 有状态 rollout：膜电位跨步保持（更贴近"原生时序动力学"，但增益可能 >1）
                    u0 = output.HLast
                Else
                    ' 默认：每一步都按训练协议从当前表达状态重新初始化膜电位。
                    ' 训练时的输入是与本步状态对应的恒流（持续 SimulationSteps 步），
                    ' 若在闭环中再把膜电位也携带过来，等效增益会接近 1 + β·(1−S) > 1，
                    ' 使状态在十几步内被推向表达区间的两端（全部被裁剪），轨迹失去信息量。
                    u0 = _model.MapToMembrane(state)
                End If
            Next

            Dim baseValues(n - 1) As Double
            Array.Copy(baselineData, baseValues, n)

            Return New PerturbationTrajectory With {
                .GeneNames = CType(genes.Clone(), String()),
                .Specs = resolved.ToArray(),
                .Trajectory = trajectory,
                .Baseline = baseValues,
                .Denormalized = Denormalize(trajectory),
                .ClampCount = clampCount,
                .Applied = resolved.Count > 0
            }
        End Function

        Private Function Denormalize(values As Double()()) As Double()()
            If _trajectory Is Nothing OrElse values Is Nothing Then Return Nothing

            Dim scale = _trajectory.GlobalMax - _trajectory.GlobalMin
            Dim result(values.Length - 1)() As Double
            For t = 0 To values.Length - 1
                result(t) = New Double(values(t).Length - 1) {}
                For g = 0 To values(t).Length - 1
                    result(t)(g) = _trajectory.GlobalMin + values(t)(g) * scale
                Next
            Next
            Return result
        End Function

#End Region

#Region "批量扫描"

        ''' <summary>
        ''' 批量扰动扫描（readme 四 的 batch_perturbation_scan）：对一组候选基因逐个做扰动，
        ''' 返回"扰动基因 → 最终表达偏移"的汇总与影响强度排名，可用于识别关键调控节点。
        ''' </summary>
        ''' <param name="baseline">基线表达状态 [1, N]</param>
        ''' <param name="genes">候选基因名列表</param>
        ''' <param name="mode">扰动模式（默认敲低，强度适中，避免全网络饱和）</param>
        ''' <param name="strength">扰动强度</param>
        ''' <param name="keepTrajectories">是否保留每个基因的完整时序轨迹（占用内存较多）</param>
        Public Function Scan(baseline As Tensor, genes As String(),
                             Optional mode As InterventionMode = InterventionMode.Knockdown,
                             Optional strength As Double = 0.5,
                             Optional keepTrajectories As Boolean = False) As BatchPerturbationResult

            If genes Is Nothing OrElse genes.Length = 0 Then
                Throw New ArgumentException("候选基因列表不能为空", NameOf(genes))
            End If

            Dim n = genes.Length
            Dim allGenes = _model.Graph.GeneNames
            Dim delta(n - 1)() As Double
            Dim score(n - 1) As Double
            Dim trajs As Dictionary(Of String, PerturbationTrajectory) = Nothing

            If keepTrajectories Then
                trajs = New Dictionary(Of String, PerturbationTrajectory)(StringComparer.OrdinalIgnoreCase)
            End If

            For i = 0 To n - 1
                Dim spec As New PerturbationSpec With {
                    .GeneName = genes(i),
                    .Mode = mode,
                    .Strength = strength
                }

                Dim tr = Run(baseline, New PerturbationSpec() {spec})
                delta(i) = tr.Delta()

                ' 影响强度：排除自身，只看它对下游基因造成的偏移
                Dim selfIdx = Array.IndexOf(allGenes, genes(i))
                Dim s = 0.0
                For g = 0 To delta(i).Length - 1
                    If g = selfIdx Then Continue For
                    s += std.Abs(delta(i)(g))
                Next
                score(i) = s

                If keepTrajectories Then trajs(genes(i)) = tr
            Next

            Dim order = Enumerable.Range(0, n).OrderByDescending(Function(i) score(i)).ToArray()
            Dim rankedGenes(n - 1) As String
            Dim rankedScore(n - 1) As Double
            For k = 0 To n - 1
                rankedGenes(k) = genes(order(k))
                rankedScore(k) = score(order(k))
            Next

            Return New BatchPerturbationResult With {
                .GeneNames = CType(allGenes.Clone(), String()),
                .PerturbedGenes = CType(genes.Clone(), String()),
                .Mode = mode,
                .Strength = strength,
                .Delta = delta,
                .ImpactScore = score,
                .RankedGenes = rankedGenes,
                .RankedScore = rankedScore,
                .Trajectories = trajs
            }
        End Function

#End Region

#Region "扰动注入"

        ''' <summary>
        ''' 把扰动注入到输入电流序列（readme 2.3 的 inject_perturbation）：
        '''   KO     → I[:, g] = 0
        '''   KD     → I[:, g] *= (1 − strength)
        '''   OE     → I[:, g] += strength · I_max
        '''   Custom → I[:, g] = CustomValue
        ''' </summary>
        Private Function Inject(currents As List(Of Tensor), specs As List(Of PerturbationSpec),
                                n As Integer) As List(Of Tensor)
            If specs.Count = 0 Then Return currents

            ' PerturbationInjectSteps ≤ 0 表示在整个决策窗口内持续注入
            Dim injectSteps = If(_config.PerturbationInjectSteps <= 0,
                                 _config.SimulationSteps,
                                 std.Min(_config.PerturbationInjectSteps, _config.SimulationSteps))

            Dim result As New List(Of Tensor)()
            For t = 0 To currents.Count - 1
                If t >= injectSteps Then
                    result.Add(currents(t))
                    Continue For
                End If

                Dim d = CType(currents(t).Data.Clone(), Double())
                For Each spec In specs
                    Dim g = spec.GeneIndex
                    If g < 0 OrElse g >= n Then Continue For

                    Select Case spec.Mode
                        Case InterventionMode.Knockout
                            d(g) = 0.0
                        Case InterventionMode.Knockdown
                            d(g) *= (1.0 - spec.Strength)
                        Case InterventionMode.Overexpression
                            d(g) += spec.Strength * _config.CurrentGain
                        Case Else ' Custom
                            d(g) = spec.CustomValue
                    End Select
                Next

                result.Add(Tensor.Wrap(d, currents(t).Shape))
            Next

            Return result
        End Function

#End Region

    End Class

End Namespace
