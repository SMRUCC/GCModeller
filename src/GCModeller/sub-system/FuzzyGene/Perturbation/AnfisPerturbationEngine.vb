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
        End Sub

        ''' <summary>恢复训练数据推导出的对照基线。</summary>
        Public Sub ResetBaseline()
            _baseline = CType(_network.BaselineRaw.Clone(), Double())
            _control = Nothing
        End Sub

        ''' <summary>未扰动时的稳态（缓存；基线变化时自动失效）。</summary>
        Public Function ControlSteadyState() As Double()
            'If _control Is Nothing Then
            '    _control = _network.SteadyState(_baseline)
            'End If

            'Return _control
            Return _baseline
        End Function

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

            Dim mutant As Double() = _network.SteadyState(
                _baseline, plan.Clamps,
                _network.Config.PropagationSteps,
                _network.Config.Relaxation,
                _network.Config.ConvergenceTolerance)

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

            Dim mutantTrace As Data.SteadyStateTrace = _network.SteadyStateTraceOf(
                _baseline, plan.Clamps, count,
                relaxation,
                _network.Config.ConvergenceTolerance)

            ' 末帧就是求解器的返回值：就地回写干预值，让曲线终点与报告出来的
            ' MutantMeans 完全一致（否则二者会差一个 ToZ/ToRaw 往返的舍入）
            Dim mutant As Double() = mutantTrace.Final

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
