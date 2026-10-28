Imports SMRUCC.genomics.Analysis.BNLearn.Intervention
Imports SMRUCC.genomics.Analysis.FuzzyGene.Config
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
            If _control Is Nothing Then
                _control = _network.SteadyState(_baseline)
            End If

            Return _control
        End Function

        ''' <summary>
        ''' 预测（可组合的）干预结果。
        ''' </summary>
        ''' <param name="specs">干预定义；多个元素表示组合扰动</param>
        ''' <returns>标准干预结果</returns>
        Public Function Predict(specs As IEnumerable(Of InterventionSpec)) As InterventionResult
            Dim list As List(Of InterventionSpec) = If(specs, Enumerable.Empty(Of InterventionSpec)()).ToList()

            If list.Count = 0 Then
                Throw New ArgumentException("at least one intervention spec is required", NameOf(specs))
            End If

            Dim indices As New List(Of Integer)
            Dim names As New List(Of String)
            Dim resolved As New List(Of InterventionSpec)
            Dim values As New List(Of Double)
            Dim missing As New List(Of String)

            For Each spec As InterventionSpec In list
                Dim idx As Integer = ResolveIndex(spec)

                If idx < 0 Then
                    Call missing.Add(spec.GeneName)
                    Continue For
                End If
                If indices.Contains(idx) Then
                    Continue For
                End If

                Call indices.Add(idx)
                Call names.Add(_network.GeneNames(idx))
                Call resolved.Add(spec)
                Call values.Add(InterventionValueOf(spec, idx))
            Next

            If missing.Count > 0 Then
                If Strict Then
                    Throw New MissingMemberException("missing target gene: " & String.Join(", ", missing))
                End If

                Call ($"[AnfisPerturbationEngine] warning: target gene(s) not found in the fitted network: {String.Join(", ", missing)}. " &
                      "The virtual perturbation was skipped and the wildtype state is returned.").debug
            End If

            Dim control As Double() = ControlSteadyState()

            If indices.Count = 0 Then
                Return CreateUndefinedResult(list(0), control)
            End If

            Dim clamps As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

            For k As Integer = 0 To indices.Count - 1
                clamps(names(k)) = values(k)
            Next

            Dim mutant As Double() = _network.SteadyState(
                _baseline, clamps,
                _network.Config.PropagationSteps,
                _network.Config.Relaxation,
                _network.Config.ConvergenceTolerance)

            ' 被扰动基因的表达是实验施加的结果，不是模型的推断，直接写回干预值
            For k As Integer = 0 To indices.Count - 1
                mutant(indices(k)) = values(k)
            Next

            Dim resultSpec As InterventionSpec = If(resolved.Count = 1, resolved(0), CombineSpec(list, names, indices))

            Return BuildResult(resultSpec, control, mutant, New HashSet(Of Integer)(indices))
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
                {_network.GeneNames(idx), InterventionValueOf(spec, idx)}
            }

            Dim result As InterventionResult = Predict(New InterventionSpec() {spec})

            result.DynamicTrajectory = _network.SimulateCascade(_baseline, clamps, steps)

            Return result
        End Function

        ' ==================== 内部方法 ====================

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
        Private Function InterventionValueOf(spec As InterventionSpec, idx As Integer) As Double
            Dim baseline As Double = _baseline(idx)

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

    ''' <summary>组合扰动分析结果。</summary>
    Public Class CombinationPerturbationResult

        ''' <summary>联合扰动的标准结果。</summary>
        Public Property Joint As InterventionResult

        ''' <summary>加性期望（对照 + 各单基因扰动效应之和）。</summary>
        Public Property AdditiveMeans As Double()

        ''' <summary>非加性效应（联合结果 − 加性期望）：正为协同、负为拮抗。</summary>
        Public Property NonAdditive As Double()

        ''' <summary>基因名。</summary>
        Public Property GeneNames As String()

        ''' <summary>取非加性效应最强的前 n 个基因。</summary>
        Public Function GetTopSynergy(n As Integer) As List(Of (GeneName As String, Effect As Double))
            Dim result As New List(Of (GeneName As String, Effect As Double))
            Dim order As Integer() = Enumerable.Range(0, NonAdditive.Length) _
                .OrderByDescending(Function(i) System.Math.Abs(NonAdditive(i))) _
                .ToArray()

            For i As Integer = 0 To System.Math.Min(n, order.Length) - 1
                Dim idx As Integer = order(i)
                Call result.Add((GeneName:=GeneNames(idx), Effect:=NonAdditive(idx)))
            Next

            Return result
        End Function

        Public Overrides Function ToString() As String
            Return $"combination: {If(Joint Is Nothing, "?", Joint.Spec.GeneName)}, " &
                   $"top synergy = {String.Join("; ", GetTopSynergy(5).Select(Function(x) $"{x.GeneName}:{x.Effect:G3}"))}"
        End Function

    End Class

End Namespace
