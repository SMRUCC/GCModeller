Imports System.Diagnostics
Imports SMRUCC.genomics.BioSystem.Intervention
Imports std = System.Math

Namespace Perturbation

    ''' <summary>
    ''' 基于神经模糊调控网络的虚拟扰动引擎。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 扰动在数学上就是「钳制 + 迭代传播」：
    ''' 把目标基因固定在干预值上（切断它自己的入边），然后让 <c>x ← F(x)</c>
    ''' 迭代到新的稳态。由于 <c>F</c> 由可解释的 TSK 规则组成，
    ''' 部分敲降（连续扰动）在这里是**原生支持**的——不像布尔网络只能做 0/1 开关。
    ''' </para>
    ''' <para>
    ''' 对照（野生型）与突变型都取**各自的稳态**，因此 <see cref="InterventionResult.FoldChanges"/>
    ''' 反映的是「两种稳态之差」而不是单步响应；这也是 do-演算分析的常规口径。
    ''' </para>
    ''' </remarks>
    Public Class AnfisPerturbationEngine

        Private ReadOnly _network As FuzzyGeneNetwork
        Private _baseline As Double()
        Private _control As Double()
        Private _controlReport As Solver.SteadyStateReport
        Private _lastReport As Solver.SteadyStateReport
        Private _fcmResult As Hybrid.FcmModuleResult

        ''' <summary>
        ''' 严格模式：目标基因不在建模基因集合中时抛 <see cref="MissingMemberException"/>；
        ''' 非严格模式下打印告警并返回标记了 <see cref="InterventionResult.Undefined"/> 的野生型降级结果。
        ''' </summary>
        Public Property Strict As Boolean = True

        ''' <summary>绑定到已拟合的网络（基线取训练数据的稳态背景）。</summary>
        Public Sub New(network As FuzzyGeneNetwork)
            If network Is Nothing Then
                Throw New ArgumentNullException(NameOf(network))
            End If

            _network = network
            _baseline = CType(network.BaselineRaw.Clone(), Double())
        End Sub

        ''' <summary>底层网络。</summary>
        Public ReadOnly Property Network As FuzzyGeneNetwork
            Get
                Return _network
            End Get
        End Property

        ''' <summary>当前使用的对照基线（原始表达量）。</summary>
        Public ReadOnly Property Baseline As Double()
            Get
                Return _baseline
            End Get
        End Property

        ''' <summary>敲低后的残余比例（默认取配置值 0.2）。</summary>
        Public Property KnockdownResidual As Double
            Get
                Return _network.Config.KnockdownResidual
            End Get
            Set(value As Double)
                _network.Config.KnockdownResidual = value
                _control = Nothing
            End Set
        End Property

        ''' <summary>过表达的提升幅度（以训练期标准差为单位）。</summary>
        Public Property OverexpressionSigmas As Double
            Get
                Return _network.Config.OverexpressionSigmas
            End Get
            Set(value As Double)
                _network.Config.OverexpressionSigmas = value
                _control = Nothing
            End Set
        End Property

        ''' <summary>显著性阈值（Z 分数）。</summary>
        Public Property SignificanceZScore As Double
            Get
                Return _network.Config.SignificanceZScore
            End Get
            Set(value As Double)
                _network.Config.SignificanceZScore = value
            End Set
        End Property

        ''' <summary>
        ''' 替换对照基线（原始表达量），用于把外部样本背景注入扰动分析。
        ''' </summary>
        Public Sub SetBaseline(state As Double())
            If state Is Nothing Then
                Throw New ArgumentNullException(NameOf(state))
            End If
            If state.Length <> _network.NGene Then
                Throw New ArgumentException(
                    $"the baseline has {state.Length} values but the network has {_network.NGene} genes", NameOf(state))
            End If

            _baseline = CType(state.Clone(), Double())
            _control = Nothing
            _controlReport = Nothing
        End Sub

        ''' <summary>恢复训练数据推导出的对照基线。</summary>
        Public Sub ResetBaseline()
            _baseline = CType(_network.BaselineRaw.Clone(), Double())
            _control = Nothing
            _controlReport = Nothing
        End Sub

        ''' <summary>
        ''' 未扰动时的稳态（真正迭代到不动点，缓存；基线/配置变化时自动失效）。
        ''' </summary>
        ''' <remarks>
        ''' <para>
        ''' 这里必须真的迭代：<c>F</c> 是非线性且带输出夹断的，<c>F(均值) ≠ 均值</c>，
        ''' 直接把训练基线当作野生型会让 <see cref="InterventionResult.FoldChanges"/>
        ''' 混进「模型自己会漂到哪」的系统性偏移（<c>test</c> 里的 <c>baseline.drift</c>）。
        ''' </para>
        ''' <para>
        ''' 旧实现把这个迭代注释掉了，等于「突变型迭代到不动点、野生型停在初始点」，
        ''' 两者口径不一致。恢复迭代后，<c>FoldChanges</c> 才是「同一模型下的两个稳态之差」。
        ''' </para>
        ''' <para>
        ''' 返回的是缓存数组本身，调用方不得就地修改（需要改写请先 <c>Clone</c>）。
        ''' </para>
        ''' </remarks>
        Public Function ControlSteadyState() As Double()
            If _control Is Nothing Then
                Dim report As Solver.SteadyStateReport = _network.SteadyStateWithReport(_baseline)

                _control = report.FinalState
                _controlReport = report
            End If

            Return _control
        End Function

        ''' <summary>最近一次野生型稳态求解的诊断（未求解时为 <c>Nothing</c>）。</summary>
        Public ReadOnly Property ControlReport As Solver.SteadyStateReport
            Get
                Return _controlReport
            End Get
        End Property

        ''' <summary>最近一次突变型稳态求解的诊断（未求解时为 <c>Nothing</c>）。</summary>
        ''' <remarks>
        ''' 这是「这次扰动到底解没解出来」的唯一可靠来源：
        ''' <see cref="ControlSteadyState"/> 的野生型与这里的突变型走同一个求解器，
        ''' 因此可以直接比较两者的 <c>ExitReason</c>。
        ''' </remarks>
        Public ReadOnly Property LastSolveReport As Solver.SteadyStateReport
            Get
                Return _lastReport
            End Get
        End Property

        ''' <summary>
        ''' 预测（可组合的）干预结果。
        ''' </summary>
        ''' <param name="specs">干预定义；多个元素表示组合扰动</param>
        ''' <returns>标准干预结果</returns>
        Public Function Predict(specs As IEnumerable(Of InterventionSpec)) As InterventionResult
            Dim list As List(Of InterventionSpec) = ToSpecList(specs)
            Dim plan As InterventionPlan = ResolvePlan(list)

            If plan.Indices.Count = 0 Then
                Return CreateUndefinedResult(list(0), plan.Control)
            End If

            ' ---- FCM 热启动：模块级有界传播给出一个全局自洽的起点 ----
            ' 它只影响「落到哪个吸引子」，不改变方程的根（牛顿法在同一个不动点上收敛）
            Dim initial As Double() = PrepareInitial(plan)

            Dim report As Solver.SteadyStateReport = _network.SteadyStateWithReport(
                initial, plan.Clamps,
                _network.Config.PropagationSteps,
                _network.Config.Relaxation,
                _network.Config.ConvergenceTolerance)

            ' 热启动只是加速器：它把求解带偏（落到无不动点的分支）时必须退回基线重解，
            ' 否则「打开 FCM」会变成一个降低收敛率的开关。只有热启动真的失败才付这份代价。
            If Not report.Converged AndAlso _fcmResult IsNot Nothing Then
                Dim fallback As Solver.SteadyStateReport = _network.SteadyStateWithReport(
                    _baseline, plan.Clamps,
                    _network.Config.PropagationSteps,
                    _network.Config.Relaxation,
                    _network.Config.ConvergenceTolerance)

                If fallback.Converged Then
                    report = fallback
                End If
            End If

            _lastReport = report

            Call AttachStabilizer(report, Nothing)

            ' 终态优先级：不动点 > 极限环周期均值 > FCM 模块稳态 > 尾段均值 > 最后一帧。
            ' 绝不悄悄把代理值当成定量预测——StabilizedBy 会如实写明是哪种
            Dim mutant As Double() = ResolveReportedState(report)

            ' 被扰动基因的表达是实验施加的结果，不是模型的推断，直接写回干预值
            Call ApplyClamps(mutant, plan)

            Dim resultSpec As InterventionSpec =
                If(plan.Resolved.Count = 1, plan.Resolved(0), CombineSpec(list, plan.Names, plan.Indices))

            Return BuildResult(resultSpec, plan.Control, mutant, New HashSet(Of Integer)(plan.Indices))
        End Function

        ''' <summary>
        ''' 预测干预结果，同时给出突变型与野生型的完整稳态求解轨迹。
        ''' </summary>
        ''' <param name="specs">干预定义；多个元素表示组合扰动</param>
        ''' <param name="steps">最大迭代步数；&lt;= 0 时取配置值</param>
        ''' <param name="relaxation">
        ''' 阻尼系数；<c>NaN</c> 时取配置值。ω 扫描必须能按次覆盖它，
        ''' 否则就只能靠改全局配置来切换，扫描途中容易被上一次调用改掉。
        ''' </param>
        ''' <returns>结果 + 两条轨迹</returns>
        ''' <remarks>
        ''' <para>
        ''' <see cref="PerturbationTrace.Result"/> 与 <see cref="Predict"/> 的返回值逐位一致：
        ''' 两者走同一个 <c>Solve</c>，轨迹记录只读状态、不参与任何浮点运算。
        ''' </para>
        ''' <para>
        ''' <see cref="PerturbationTrace.Control"/> 从基线重新迭代到不动点，
        ''' 因此可以把它当成「不做扰动时系统自己会漂到哪」的参照线。
        ''' </para>
        ''' </remarks>
        Public Function PredictWithTrace(specs As IEnumerable(Of InterventionSpec),
                                         Optional steps As Integer = 0,
                                         Optional relaxation As Double = Double.NaN) As PerturbationTrace

            Dim list As List(Of InterventionSpec) = ToSpecList(specs)
            Dim watch As Stopwatch = Stopwatch.StartNew()
            Dim plan As InterventionPlan = ResolvePlan(list)
            Dim count As Integer = If(steps > 0, steps, _network.Config.PropagationSteps)

            ' 对照轨迹总是要给：即使目标基因缺失，它也说明了「系统本来的走向」
            Dim controlTrace As Data.SteadyStateTrace = _network.SteadyStateTraceOf(
                _baseline, Nothing, count,
                relaxation,
                _network.Config.ConvergenceTolerance)

            If plan.Indices.Count = 0 Then
                Dim undefined As InterventionResult = CreateUndefinedResult(list(0), plan.Control)

                watch.Stop()

                Return New PerturbationTrace With {
                    .Result = undefined,
                    .Mutant = Nothing,
                    .Control = controlTrace,
                    .TargetGene = Nothing,
                    .Mode = list(0).Mode,
                    .ClampValue = Double.NaN,
                    .ElapsedMs = watch.Elapsed.TotalMilliseconds
                }
            End If

            ' 与 Predict 走同一套热启动：否则两条路径的起点不同，结果无法逐位一致
            Dim initial As Double() = PrepareInitial(plan)

            Dim mutantTrace As Data.SteadyStateTrace = _network.SteadyStateTraceOf(
                initial, plan.Clamps, count,
                relaxation,
                _network.Config.ConvergenceTolerance)

            ' 与 Predict 一致：热启动把求解带偏时退回基线重解，取收敛的那一次
            If Not mutantTrace.Converged AndAlso _fcmResult IsNot Nothing Then
                Dim retry As Data.SteadyStateTrace = _network.SteadyStateTraceOf(
                    _baseline, plan.Clamps, count,
                    relaxation,
                    _network.Config.ConvergenceTolerance)

                If retry.Converged Then
                    mutantTrace = retry
                End If
            End If

            ' 与 Predict 同一套终态判据（不动点 > 周期均值 > FCM 代理 > 尾段均值）
            Dim mutant As Double() = ResolveTraceState(mutantTrace)

            Call ApplyClamps(mutant, plan)

            Dim resultSpec As InterventionSpec =
                If(plan.Resolved.Count = 1, plan.Resolved(0), CombineSpec(list, plan.Names, plan.Indices))
            Dim result As InterventionResult =
                BuildResult(resultSpec, plan.Control, mutant, New HashSet(Of Integer)(plan.Indices))

            watch.Stop()

            Return New PerturbationTrace With {
                .Result = result,
                .Mutant = mutantTrace,
                .Control = controlTrace,
                .TargetGene = String.Join("+", plan.Names),
                .Mode = resultSpec.Mode,
                .ClampValue = If(plan.Values.Count = 1, plan.Values(0), Double.NaN),
                .ElapsedMs = watch.Elapsed.TotalMilliseconds
            }
        End Function

        ''' <summary>FCM 热启动：模块级有界传播给出一个全局自洽的起点。</summary>
        ''' <remarks>
        ''' 只影响「落到哪个吸引子」，不改变方程的根；模块层失败时静默退回训练基线，
        ''' 因为粗粒度层只是加速器，不是主路径。
        ''' </remarks>
        Private Function PrepareInitial(plan As InterventionPlan) As Double()
            _fcmResult = Nothing

            If _network.Modules Is Nothing OrElse Not _network.Config.Hybrid.FcmWarmStart Then
                Return _baseline
            End If

            Try
                Dim layer As New Hybrid.FcmModuleLayer(_network)
                Dim warm As Hybrid.FcmModuleResult = layer.BuildInitial(_baseline, plan.Clamps)

                If warm IsNot Nothing AndAlso warm.GeneInitial IsNot Nothing Then
                    _fcmResult = warm

                    Return warm.GeneInitial
                End If
            Catch ex As Exception
                ' 粗粒度层失败只意味着「没有热启动」，主路径必须继续
            End Try

            Return _baseline
        End Function

        ''' <summary>
        ''' 没有不动点也没有周期时挂上代理值：先 FCM 模块稳态，再轨迹尾段均值。
        ''' </summary>
        ''' <remarks>
        ''' 优先级刻意是「真实周期均值 &gt; FCM &gt; 尾段均值」：前两者都有明确语义，
        ''' 尾段均值只是「跑完预算还没收敛」时的最后兜底。三者都会写进
        ''' <see cref="Solver.SteadyStateReport.StabilizedBy"/>，下游可据此决定要不要采信。
        ''' </remarks>
        Private Sub AttachStabilizer(report As Solver.SteadyStateReport, trace As Data.SteadyStateTrace)
            If report Is Nothing OrElse report.Converged Then
                Return
            End If
            ' 已经有真实周期均值就不再覆盖
            If report.AttractorMean IsNot Nothing AndAlso
               Not String.Equals(report.StabilizedBy, "tail mean", StringComparison.OrdinalIgnoreCase) Then
                Return
            End If

            If _fcmResult IsNot Nothing AndAlso _network.Config.Hybrid.FcmFallbackAsSurrogate Then
                report.AttractorMean = _fcmResult.Surrogate
                report.StabilizedBy = "fcm surrogate"
                report.AddMessage("the continuous solver did not reach a fixed point; the FCM module steady state is attached as a surrogate (AttractorMean)")

                Return
            End If

            If trace Is Nothing Then
                Return
            End If

            Dim mean As Double() = TailMean(trace)

            If mean IsNot Nothing Then
                report.AttractorMean = mean
                report.StabilizedBy = "tail mean"
                report.AddMessage("no fixed point and no exact cycle; the mean of the last frames is attached as a time-average surrogate")
            End If
        End Sub

        ''' <summary>求解报告 → 应当汇报的终态。</summary>
        Private Shared Function ResolveReportedState(report As Solver.SteadyStateReport) As Double()
            If report Is Nothing Then
                Return Nothing
            End If
            If report.AttractorMean IsNot Nothing Then
                Return CType(report.AttractorMean.Clone(), Double())
            End If

            Return CType(report.FinalState.Clone(), Double())
        End Function

        ''' <summary>轨迹 → 应当汇报的终态（与 <see cref="ResolveReportedState"/> 同一套优先级）。</summary>
        Private Function ResolveTraceState(trace As Data.SteadyStateTrace) As Double()
            If trace Is Nothing Then
                Return CType(_baseline.Clone(), Double())
            End If
            If trace.Converged Then
                Return CType(trace.Final.Clone(), Double())
            End If

            ' 没有真实周期均值时，FCM 代理优先于尾段均值
            If trace.AttractorMean Is Nothing OrElse
               String.Equals(trace.StabilizedBy, "tail mean", StringComparison.OrdinalIgnoreCase) Then

                If _fcmResult IsNot Nothing AndAlso _network.Config.Hybrid.FcmFallbackAsSurrogate Then
                    trace.AttractorMean = _fcmResult.Surrogate
                    trace.StabilizedBy = "fcm surrogate"

                    Return CType(_fcmResult.Surrogate.Clone(), Double())
                End If
            End If

            If trace.AttractorMean IsNot Nothing Then
                Return CType(trace.AttractorMean.Clone(), Double())
            End If

            Return CType(trace.Final.Clone(), Double())
        End Function

        ''' <summary>轨迹最后若干帧的均值（帧数不足时返回 <c>Nothing</c>）。</summary>
        Private Shared Function TailMean(trace As Data.SteadyStateTrace,
                                         Optional frames As Integer = 8) As Double()
            If trace Is Nothing OrElse trace.States Is Nothing OrElse trace.States.Count < 4 Then
                Return Nothing
            End If

            Dim count As Integer = System.Math.Min(frames, trace.States.Count - 1)

            If count < 2 Then
                Return Nothing
            End If

            Dim genes As Integer = trace.States(0).Length
            Dim mean(genes - 1) As Double
            Dim start As Integer = trace.States.Count - count

            For i As Integer = start To trace.States.Count - 1
                Dim frame As Double() = trace.States(i)

                For g As Integer = 0 To genes - 1
                    mean(g) += frame(g)
                Next
            Next

            For g As Integer = 0 To genes - 1
                mean(g) /= count
            Next

            Return mean
        End Function

        ''' <summary>
        ''' 集成预测：按 PBN 规则概率重建网络多次，把结构不确定性传播到扰动结果上。
        ''' </summary>
        ''' <param name="specs">干预定义</param>
        ''' <param name="nSamples">采样次数；&lt;= 1 时退化为确定性单点结果</param>
        ''' <param name="pbn">拟合期构建的 PBN 定性层</param>
        ''' <returns>逐基因均值/标准差/置信区间/符号一致率与吸引子周期分布</returns>
        ''' <remarks>
        ''' 确定性参考解只求一次（<see cref="Predict"/>），各样本共用；
        ''' 只有 <see cref="Config.HybridOptions.ModeBiasWeight"/> &gt; 0 时才会逐样本重新求解，
        ''' 因此默认配置下的额外成本几乎全在布尔层。
        ''' </remarks>
        Public Function PredictEnsemble(specs As IEnumerable(Of InterventionSpec),
                                        nSamples As Integer,
                                        pbn As Hybrid.PbnRegulatorPrior) As Hybrid.EnsemblePerturbationResult
            Dim list As List(Of InterventionSpec) = ToSpecList(specs)

            If pbn Is Nothing Then
                Throw New InvalidOperationException(
                    "the PBN layer is not available for this model (either PBN regularization was disabled at fit time " &
                    "or the model was restored from a package); retrain with Hybrid.PbnRegularization = Filter/Weighted")
            End If

            Dim plan As InterventionPlan = ResolvePlan(list)
            Dim reference As Double() = Nothing
            Dim converged As Boolean = False

            If plan.Indices.Count > 0 Then
                reference = Predict(list).MutantMeans
                converged = _lastReport IsNot Nothing AndAlso _lastReport.Converged
            End If

            Dim ensemble As New Hybrid.PbnEnsemble(_network, pbn, _network.Config)

            Return ensemble.Predict(_baseline, plan.Clamps, reference, nSamples, converged)
        End Function

        ''' <summary>
        ''' 组合扰动的非加性（协同/拮抗）分析。
        ''' </summary>
        ''' <param name="specs">至少两个干预定义</param>
        ''' <returns>联合结果、加性期望与非加性效应</returns>
        Public Function PredictCombination(specs As List(Of InterventionSpec)) As CombinationPerturbationResult
            If specs Is Nothing OrElse specs.Count < 2 Then
                Throw New ArgumentException("the combination analysis requires at least two interventions", NameOf(specs))
            End If

            Dim joint As InterventionResult = Predict(specs)
            Dim control As Double() = ControlSteadyState()
            Dim additive As Double() = CType(control.Clone(), Double())

            For Each spec As InterventionSpec In specs
                Dim one As InterventionResult = Predict(New InterventionSpec() {spec})

                For i As Integer = 0 To additive.Length - 1
                    additive(i) += one.FoldChanges(i)
                Next
            Next

            Dim nonAdditive(additive.Length - 1) As Double

            For i As Integer = 0 To additive.Length - 1
                nonAdditive(i) = joint.MutantMeans(i) - additive(i)
            Next

            Return New CombinationPerturbationResult With {
                .Joint = joint,
                .AdditiveMeans = additive,
                .NonAdditive = nonAdditive,
                .GeneNames = joint.GeneNames
            }
        End Function

        ''' <summary>
        ''' 多步级联模拟：把钳制后的状态逐步向前推进，结果写入
        ''' <see cref="InterventionResult.DynamicTrajectory"/>。
        ''' </summary>
        Public Function SimulateCascade(spec As InterventionSpec, steps As Integer) As InterventionResult
            Dim idx As Integer = ResolveIndex(spec)

            If idx < 0 Then
                If Strict Then
                    Throw New MissingMemberException("missing target gene: " & spec.GeneName)
                End If

                Dim control As Double() = ControlSteadyState()
                Dim undefined As InterventionResult = CreateUndefinedResult(spec, control)

                Return undefined
            End If

            Dim clamps As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase) From {
                {_network.GeneNames(idx), InterventionValueOf(spec, idx, ControlSteadyState())}
            }

            Dim result As InterventionResult = Predict(New InterventionSpec() {spec})

            result.DynamicTrajectory = _network.SimulateCascade(_baseline, clamps, steps)

            Return result
        End Function

        ' ==================== 内部方法 ====================

        ''' <summary>
        ''' 一次干预解析后的执行计划（<see cref="Predict"/> 与 <see cref="PredictWithTrace"/> 共用）。
        ''' </summary>
        ''' <remarks>
        ''' 把「解析干预 → 去重 → 计算钳制值」这段与「怎么求解」分开，
        ''' 两个入口才不会因为复制而漂移：约束是同一组 clamps、同一个初始状态。
        ''' </remarks>
        Private Class InterventionPlan

            ''' <summary>调用方给的原始干预定义。</summary>
            Public Property Specs As List(Of InterventionSpec)

            ''' <summary>命中的基因下标（去重后，顺序即施加顺序）。</summary>
            Public Property Indices As New List(Of Integer)

            ''' <summary>命中基因的规范名（与 <see cref="Indices"/> 一一对应）。</summary>
            Public Property Names As New List(Of String)

            ''' <summary>命中的干预定义（与 <see cref="Indices"/> 一一对应）。</summary>
            Public Property Resolved As New List(Of InterventionSpec)

            ''' <summary>钳制值（原始表达量，与 <see cref="Indices"/> 一一对应）。</summary>
            Public Property Values As New List(Of Double)

            ''' <summary>模型里找不到的目标基因。</summary>
            Public Property Missing As New List(Of String)

            ''' <summary>对照状态（原始表达量），干预值以它为参考。</summary>
            Public Property Control As Double()

            ''' <summary>真正交给网络的钳制条件（基因名 → 原始表达量）。</summary>
            Public ReadOnly Property Clamps As Dictionary(Of String, Double)
                Get
                    Dim items As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

                    For k As Integer = 0 To Names.Count - 1
                        items(Names(k)) = Values(k)
                    Next

                    Return items
                End Get
            End Property

        End Class

        ''' <summary>把调用方给的集合转成列表，并拒绝空集。</summary>
        Private Shared Function ToSpecList(specs As IEnumerable(Of InterventionSpec)) As List(Of InterventionSpec)
            Dim list As List(Of InterventionSpec) = If(specs, Enumerable.Empty(Of InterventionSpec)()).ToList()

            If list.Count = 0 Then
                Throw New ArgumentException("at least one intervention spec is required", NameOf(specs))
            End If

            Return list
        End Function

        ''' <summary>
        ''' 解析干预目标并计算钳制值。
        ''' </summary>
        ''' <remarks>
        ''' 干预值以「对照稳态」为参考（而不是训练均值）：这样报告出来的
        ''' <c>WildtypeMeans</c> 与 knockdown / overexpression 的施加值完全自洽
        ''' （敲低后的表达恰好是野生型的 residual 倍）。
        ''' </remarks>
        Private Function ResolvePlan(list As List(Of InterventionSpec)) As InterventionPlan
            Dim plan As New InterventionPlan With {
                .Specs = list,
                .Control = ControlSteadyState()
            }

            For Each spec As InterventionSpec In list
                Dim idx As Integer = ResolveIndex(spec)

                If idx < 0 Then
                    Call plan.Missing.Add(spec.GeneName)
                    Continue For
                End If
                If plan.Indices.Contains(idx) Then
                    Continue For
                End If

                Call plan.Indices.Add(idx)
                Call plan.Names.Add(_network.GeneNames(idx))
                Call plan.Resolved.Add(spec)
                Call plan.Values.Add(InterventionValueOf(spec, idx, plan.Control))
            Next

            If plan.Missing.Count > 0 Then
                If Strict Then
                    Throw New MissingMemberException("missing target gene: " & String.Join(", ", plan.Missing))
                End If

                Call ($"[AnfisPerturbationEngine] warning: target gene(s) not found in the fitted network: {String.Join(", ", plan.Missing)}. " &
                      "The virtual perturbation was skipped and the wildtype state is returned.").debug
            End If

            Return plan
        End Function

        ''' <summary>把干预值就地写回状态（被扰动基因的表达由实验施加，不由模型推断）。</summary>
        Private Shared Sub ApplyClamps(state As Double(), plan As InterventionPlan)
            For k As Integer = 0 To plan.Indices.Count - 1
                state(plan.Indices(k)) = plan.Values(k)
            Next
        End Sub

        ''' <summary>解析干预目标在建模基因集合中的下标；缺失时返回 -1。</summary>
        Private Function ResolveIndex(spec As InterventionSpec) As Integer
            Dim n As Integer = _network.NGene

            If spec.GeneIndex >= 0 AndAlso spec.GeneIndex < n AndAlso
               String.Equals(_network.GeneNames(spec.GeneIndex), spec.GeneName, StringComparison.OrdinalIgnoreCase) Then
                Return spec.GeneIndex
            End If

            Return _network.IndexOfGene(spec.GeneName)
        End Function

        ''' <summary>按扰动模式计算钳制值（原始表达量）。</summary>
        ''' <param name="spec">干预定义</param>
        ''' <param name="idx">目标基因下标</param>
        ''' <param name="reference">参考基线（通常是对照稳态，见 <see cref="ControlSteadyState"/>）</param>
        Private Function InterventionValueOf(spec As InterventionSpec, idx As Integer, reference As Double()) As Double
            Dim baseline As Double = reference(idx)

            Select Case spec.Mode
                Case InterventionMode.Knockout
                    Return 0.0
                Case InterventionMode.Knockdown
                    Return _network.Config.KnockdownResidual * baseline
                Case InterventionMode.Overexpression
                    Return baseline + _network.Config.OverexpressionSigmas * _network.SDs(idx)
                Case InterventionMode.Custom
                    Return spec.Value
                Case Else
                    Return spec.Value
            End Select
        End Function

        ''' <summary>组装标准干预结果（含变化量、百分比、Z 分数与显著性）。</summary>
        Private Function BuildResult(spec As InterventionSpec,
                                     control As Double(),
                                     mutant As Double(),
                                     perturbed As HashSet(Of Integer)) As InterventionResult

            Dim n As Integer = _network.NGene
            Dim result As New InterventionResult With {
                .Spec = spec,
                .WildtypeMeans = CType(control.Clone(), Double()),
                .WildtypeSDs = CType(_network.SDs.Clone(), Double()),
                .MutantMeans = mutant,
                .GeneNames = CType(_network.GeneNames.Clone(), String()),
                .FoldChanges = New Double(n - 1) {},
                .PercentChanges = New Double(n - 1) {},
                .ZScores = New Double(n - 1) {},
                .IsSignificant = New Boolean(n - 1) {}
            }

            For i As Integer = 0 To n - 1
                Dim delta As Double = mutant(i) - control(i)

                result.FoldChanges(i) = delta

                If std.Abs(control(i)) > 0.0000000001 Then
                    result.PercentChanges(i) = delta / std.Abs(control(i)) * 100.0
                End If

                Dim sd As Double = std.Max(_network.SDs(i), 0.00000001)

                result.ZScores(i) = delta / sd
                result.IsSignificant(i) = perturbed.Contains(i) OrElse
                                          std.Abs(result.ZScores(i)) > _network.Config.SignificanceZScore
            Next

            Return result
        End Function

        ''' <summary>组合扰动时构造一个代表整体的干预定义（用于结果显示）。</summary>
        Private Shared Function CombineSpec(list As List(Of InterventionSpec),
                                            names As List(Of String),
                                            indices As List(Of Integer)) As InterventionSpec
            Return New InterventionSpec With {
                .GeneIndex = -1,
                .GeneName = String.Join("+", names),
                .Mode = If(list.Count > 0, list(0).Mode, InterventionMode.Knockout)
            }
        End Function

        ''' <summary>
        ''' 目标基因缺失时的野生型降级结果：突变型 == 野生型、无显著变化，并标记 <see cref="InterventionResult.Undefined"/>。
        ''' </summary>
        Private Function CreateUndefinedResult(spec As InterventionSpec, control As Double()) As InterventionResult
            Dim n As Integer = _network.NGene

            Return New InterventionResult With {
                .Spec = spec,
                .WildtypeMeans = CType(control.Clone(), Double()),
                .WildtypeSDs = CType(_network.SDs.Clone(), Double()),
                .MutantMeans = CType(control.Clone(), Double()),
                .GeneNames = CType(_network.GeneNames.Clone(), String()),
                .FoldChanges = New Double(n - 1) {},
                .PercentChanges = New Double(n - 1) {},
                .ZScores = New Double(n - 1) {},
                .IsSignificant = New Boolean(n - 1) {},
                .Undefined = True
            }
        End Function

    End Class
End Namespace
