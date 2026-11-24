Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.GPS.Data
Imports SMRUCC.genomics.Analysis.GPS.Model
Imports SMRUCC.genomics.BioSystem.Intervention
Imports GEARSGraph = SMRUCC.genomics.Analysis.GEARS.Graph
Imports std = System.Math

Namespace Perturbation

    ''' <summary>
    ''' 组合扰动的非加性效应分析结果（method.md §6.3）。
    ''' </summary>
    Public Class CombinationResult

        ''' <summary>联合扰动的完整结果</summary>
        Public Property Joint As InterventionResult

        ''' <summary>Bliss 独立性假设下的加性期望 <c>[基因]</c>（raw log-CPM 尺度）</summary>
        Public Property Additive As Double()

        ''' <summary>非加性效应 <c>joint − additive</c> <c>[基因]</c></summary>
        Public Property Synergy As Double()

        ''' <summary>非加性效应的最大绝对值</summary>
        Public Property SynergyScore As Double

        ''' <summary>非加性效应最强的基因（按 |synergy| 降序）</summary>
        Public Property TopSynergyGenes As List(Of (Gene As String, Effect As Double))

    End Class

    ''' <summary>
    ''' 虚拟扰动引擎（method.md §6）。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 三类扰动都通过改写<b>输入窗口</b>实现（表达式扰动）：
    ''' 把目标基因在窗口内所有时间步的标准化取值改写为干预值对应的标准化值，
    ''' 再前向传播得到扰动后的全基因组预测。这与"删除调控边"的网络扰动相比
    ''' 更贴近 CRISPR 实验的语义（蛋白功能缺失 → 转录本被清除），
    ''' 且不需要模型暴露可学习的邻接矩阵。
    ''' </para>
    ''' <para>
    ''' 扰动效应定义为 <c>DE = ŷ_pert − ŷ_ctrl</c>：两者都由同一个模型预测，
    ''' 因而模型自身的系统偏差在差分中抵消，<c>DE</c> 只反映扰动带来的变化。
    ''' 这也意味着 <see cref="InterventionResult.WildtypeMeans"/> 存放的是
    ''' <b>模型给出的对照预测</b>（不是实测均值）；实测均值可由调用方另行对照。
    ''' </para>
    ''' <para>
    ''' 全部推理在双精度下进行，且不启用 dropout，保证同一输入多次调用结果一致。
    ''' </para>
    ''' </remarks>
    Public Class VirtualPerturbationEngine

        ''' <summary>
        ''' 敲低后目标基因保留的表达比例的默认取值（对应 CRISPRi 的 80% 敲低）。
        ''' </summary>
        Public Const DefaultKnockdownResidual As Double = 0.2

        Private ReadOnly model As GRNModel
        Private ReadOnly expression As PreprocessedExpression

        ''' <summary>
        ''' 当前生效的对照窗口 <c>[基因, w]</c>；构造时取训练集尾部时间窗，
        ''' 之后可由 <see cref="SetBaselineWindow"/> 替换为外部传入的细胞上下文。
        ''' </summary>
        Private baselineWindow As Double(,)
        Private ReadOnly geneIndex As Dictionary(Of String, Integer)
        Private ReadOnly significanceZ As Double

        ''' <summary>敲低后目标基因保留的表达比例（0 表示与敲除等价）</summary>
        Private _knockdownResidual As Double = DefaultKnockdownResidual

        ''' <summary>单次注入的 z 变化上限，避免把 tanh / LayerNorm 推到饱和</summary>
        Private Const MaxInjectedZ As Double = 8.0

        ''' <summary>是否启用扰动通道</summary>
        Private _perturbationChannel As Boolean = True

        ''' <summary>扰动通道的注入增益</summary>
        Private _perturbationChannelGain As Double = 1.0

        ''' <summary>被扰动基因没有出边时，是否退化到"共调控"先验继续传播</summary>
        Private _perturbationChannelCoRegulation As Boolean = False

        ''' <summary>共调控退化路径参与计算的上游调节因子上限</summary>
        Private Const MaxCoRegulationSources As Integer = 8

        ''' <summary>共调控退化路径中每个上游因子展开的靶基因上限</summary>
        Private Const MaxCoRegulationTargets As Integer = 256

        Private _control As Double()

        ''' <summary>基因名称</summary>
        Public ReadOnly Property GeneNames As String()
            Get
                Return expression.GeneNames
            End Get
        End Property

        ''' <summary>基因数量</summary>
        Public ReadOnly Property NumGenes As Integer
            Get
                Return expression.NGene
            End Get
        End Property

        ''' <summary>
        ''' 敲低后目标基因保留的表达比例。
        ''' </summary>
        ''' <returns>取值区间 <c>[0,1]</c>；0 表示敲低与敲除等价，默认 0.2（CRISPRi 的 80% 敲低）</returns>
        ''' <remarks>
        ''' 干预值按"当前 context 基线的线性 CPM 比例"计算：
        ''' <c>cpm_mut = residual × cpm_ctrl</c>（<c>cpm_ctrl</c> 由
        ''' <see cref="BaselineRawValue"/> 反算），而不是 BNLearn 的 <c>mean − 2σ</c>
        ''' （后者对大多数基因为负，被非负钳位后恒等于 0，使敲低与敲除完全等价），
        ''' 也不是在 log 尺度上做 <c>GeneMeans × residual</c>（残余比例会随表达水平漂移）。
        ''' </remarks>
        Public Property KnockdownResidual As Double
            Get
                Return _knockdownResidual
            End Get
            Set(value As Double)
                If value < 0.0 OrElse value >= 1.0 Then
                    Throw New ArgumentOutOfRangeException(NameOf(KnockdownResidual), $"敲低残余比例必须落在 [0,1) 区间内，实际为 {value}")
                End If

                _knockdownResidual = value
                _control = Nothing
            End Set
        End Property

        ''' <summary>
        ''' 是否把被扰动基因的输入变化沿先验调控边一阶注入它的靶基因。
        ''' </summary>
        ''' <returns>默认 True；关闭后退化为"只改被扰动基因自身输入"的旧行为</returns>
        Public Property PerturbationChannel As Boolean
            Get
                Return _perturbationChannel
            End Get
            Set(value As Boolean)
                _perturbationChannel = value
            End Set
        End Property

        ''' <summary>
        ''' 扰动通道的注入增益；0 等价于关闭通道。
        ''' </summary>
        ''' <returns>非负实数</returns>
        Public Property PerturbationChannelGain As Double
            Get
                Return _perturbationChannelGain
            End Get
            Set(value As Double)
                If value < 0.0 Then
                    Throw New ArgumentOutOfRangeException(
                        NameOf(PerturbationChannelGain), $"扰动通道增益不能为负，实际为 {value}")
                End If

                _perturbationChannelGain = value
            End Set
        End Property

        ''' <summary>
        ''' 被扰动基因没有出边（例如非 TF 基因）时，是否退化到"共调控"先验继续传播。
        ''' </summary>
        ''' <returns>默认 False（启发式路径，需要显式开启）</returns>
        Public Property PerturbationChannelCoRegulation As Boolean
            Get
                Return _perturbationChannelCoRegulation
            End Get
            Set(value As Boolean)
                _perturbationChannelCoRegulation = value
            End Set
        End Property

        ''' <summary>
        ''' 创建扰动引擎。
        ''' </summary>
        ''' <param name="model">已训练的模型</param>
        ''' <param name="expression">预处理后的表达数据（提供均值/标准差用于反标准化）</param>
        ''' <param name="baselineWindow">对照窗口 <c>[基因, w]</c>（标准化空间）</param>
        ''' <param name="significanceZ">显著性判据阈值</param>
        ''' <param name="knockdownResidual">敲低后目标基因保留的表达比例</param>
        ''' <param name="perturbationChannel">是否启用扰动通道（沿出边一阶注入靶基因）</param>
        ''' <param name="perturbationChannelGain">扰动通道的注入增益</param>
        ''' <param name="perturbationChannelCoRegulation">无出边时是否退化到共调控先验</param>
        Public Sub New(model As GRNModel,
                       expression As PreprocessedExpression,
                       baselineWindow As Double(,),
                       Optional significanceZ As Double = 0.5,
                       Optional knockdownResidual As Double = DefaultKnockdownResidual,
                       Optional perturbationChannel As Boolean = True,
                       Optional perturbationChannelGain As Double = 1.0,
                       Optional perturbationChannelCoRegulation As Boolean = False)

            If model Is Nothing Then Throw New ArgumentNullException(NameOf(model))
            If expression Is Nothing Then Throw New ArgumentNullException(NameOf(expression))
            If baselineWindow Is Nothing Then Throw New ArgumentNullException(NameOf(baselineWindow))

            Me.model = model
            Me.expression = expression
            Me.baselineWindow = CType(baselineWindow.Clone(), Double(,))
            Me.significanceZ = significanceZ
            Me._knockdownResidual = knockdownResidual
            Me._perturbationChannel = perturbationChannel
            Me._perturbationChannelGain = perturbationChannelGain
            Me._perturbationChannelCoRegulation = perturbationChannelCoRegulation

            Me.geneIndex = New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

            For i As Integer = 0 To expression.GeneNames.Length - 1
                Me.geneIndex(expression.GeneNames(i)) = i
            Next
        End Sub

        ''' <summary>
        ''' 把一条静态表达谱（长度 N 的标准化向量）铺成 <c>[N, w]</c> 的对照窗口。
        ''' </summary>
        ''' <param name="z">标准化空间下的逐基因表达向量，长度必须等于 <see cref="NumGenes"/></param>
        ''' <returns>窗口 <c>[基因, w]</c>，每一列都是同一条谱（与扰动改写写法一致）</returns>
        ''' <remarks>
        ''' 单细胞/伪 bulk 上下文没有时间维度，因此用"同一条谱重复 w 列"喂给滑窗模型。
        ''' </remarks>
        Public Function BuildStaticWindow(z As Double()) As Double(,)
            If z Is Nothing Then Throw New ArgumentNullException(NameOf(z))
            If z.Length <> NumGenes Then
                Throw New ArgumentException($"静态上下文谱的长度 {z.Length} 与建模基因数 {NumGenes} 不一致", NameOf(z))
            End If

            Dim w As Integer = baselineWindow.GetLength(1)
            Dim window As Double(,) = New Double(NumGenes - 1, w - 1) {}

            For i As Integer = 0 To NumGenes - 1
                For t As Integer = 0 To w - 1
                    window(i, t) = z(i)
                Next
            Next

            Return window
        End Function

        ''' <summary>
        ''' 替换当前生效的对照窗口（用于把外部细胞上下文注入模型）。
        ''' </summary>
        ''' <param name="window">新的对照窗口 <c>[基因, w]</c>（标准化空间）</param>
        ''' <remarks>
        ''' 对照预测 <c>_control</c> 是懒缓存：替换基线后必须清空它，
        ''' 否则扰动效应 <c>Δ = mutant − control</c> 会继续用旧基线的对照而恒为 0。
        ''' </remarks>
        Public Sub SetBaselineWindow(window As Double(,))
            If window Is Nothing Then Throw New ArgumentNullException(NameOf(window))
            If window.GetLength(0) <> NumGenes Then
                Throw New ArgumentException($"对照窗口的基因数 {window.GetLength(0)} 与建模基因数 {NumGenes} 不一致", NameOf(window))
            End If
            If window.GetLength(1) <> baselineWindow.GetLength(1) Then
                Throw New ArgumentException(
                    $"对照窗口的列数 {window.GetLength(1)} 与滑窗长度 {baselineWindow.GetLength(1)} 不一致", NameOf(window))
            End If

            Me.baselineWindow = CType(window.Clone(), Double(,))
            Me._control = Nothing
        End Sub

        ''' <summary>
        ''' 当前对照窗口（最后时间点）在该基因上的 raw log-CPM 基线值。
        ''' </summary>
        ''' <param name="idx">基因行号</param>
        ''' <returns>当前 context 下该基因的未扰动表达水平（log2(CPM+1)）</returns>
        ''' <remarks>
        ''' 基线取自已注入的对照窗口（<see cref="SetBaselineWindow"/>），因此它随细胞上下文变化，
        ''' 而不是训练集的全局均值。<see cref="BuildStaticWindow"/> 写入的窗口每列相同，
        ''' 取最后一列与取任意列等价。
        ''' </remarks>
        Private Function BaselineRawValue(idx As Integer) As Double
            Dim sd As Double = std.Max(expression.GeneSDs(idx), 1.0E-08)
            Dim last As Integer = baselineWindow.GetLength(1) - 1
            Dim z As Double = baselineWindow(idx, last)

            Return std.Max(0.0, z * sd + expression.GeneMeans(idx))
        End Function

        ''' <summary>
        ''' 按干预模式求目标基因的干预值（raw log-CPM 尺度）。
        ''' </summary>
        ''' <param name="spec">干预定义</param>
        ''' <param name="idx">目标基因在建模基因中的行号</param>
        ''' <param name="rawBaseline">该基因在当前 context 下的基线表达（log2(CPM+1)）</param>
        ''' <returns>干预后该基因应取的原始尺度表达值</returns>
        ''' <remarks>
        ''' <para>
        ''' 敲低按 <b>线性 CPM 空间的比例</b> 定义：<c>cpm_mut = residual × cpm_ctrl</c>，
        ''' 再变换回 <c>log2(CPM+1)</c>。基线 <paramref name="rawBaseline"/> 取自当前注入的
        ''' 细胞上下文（见 <see cref="BaselineRawValue"/>），而不是训练集全局均值
        ''' <see cref="PreprocessedExpression.GeneMeans"/>：后者把"20% 残余"实现在 log 尺度上
        ''' 做乘法（<c>0.2 × mean</c>），残余比例会随表达水平漂移
        ''' （实测 ABCD1 只剩约 12%），并且完全忽略 context 之间的基线差异。
        ''' </para>
        ''' <para>
        ''' 这里同样不使用 <see cref="InterventionSpec.GetInterventionValue"/>：该实现把敲低
        ''' 定义为 <c>mean − 2σ</c>，对大多数基因为负，随后被非负钳位压成 0，
        ''' 导致敲低与敲除逐位相同。
        ''' </para>
        ''' </remarks>
        Private Function InterventionValueOf(spec As InterventionSpec,
                                            idx As Integer,
                                            rawBaseline As Double) As Double
            Dim mean As Double = expression.GeneMeans(idx)

            Select Case spec.Mode
                Case InterventionMode.Knockout
                    Return 0.0
                Case InterventionMode.Knockdown
                    Dim cpmCtrl As Double = std.Pow(2.0, std.Max(0.0, rawBaseline)) - 1.0

                    If cpmCtrl <= 0.0 Then
                        Return 0.0
                    End If

                    Return std.Log(_knockdownResidual * cpmCtrl + 1.0, 2.0)
                Case InterventionMode.Overexpression
                    Return mean + 3.0 * std.Max(expression.GeneSDs(idx), 1.0E-08)
                Case Else
                    Return spec.Value
            End Select
        End Function

        ''' <summary>对照（未扰动）预测，raw log-CPM 尺度。</summary>
        ''' <returns>每个基因的对照预测</returns>
        Public Function ControlPrediction() As Double()
            If _control Is Nothing Then
                _control = PredictRaw(baselineWindow)
            End If

            Return _control
        End Function

        ''' <summary>
        ''' 预测（组合）扰动后的全基因组响应。
        ''' </summary>
        ''' <param name="specs">干预定义集合；多个元素表示组合扰动</param>
        ''' <returns>标准干预结果</returns>
        Public Function Predict(specs As IEnumerable(Of InterventionSpec)) As InterventionResult
            Dim list As List(Of InterventionSpec) = If(specs, Enumerable.Empty(Of InterventionSpec)()).ToList()

            If list.Count = 0 Then
                Throw New ArgumentException("至少需要一个干预定义", NameOf(specs))
            End If

            Dim n As Integer = NumGenes
            Dim indices As New List(Of Integer)()
            Dim names As New List(Of String)()
            ' 与 indices 一一对应的已解析干预定义：index 会跳过无法解析的基因，
            ' 因此不能再用 list(k) 去取第 k 个干预值，否则会错位
            Dim resolvedSpecs As New List(Of InterventionSpec)()

            For Each spec As InterventionSpec In list
                Dim idx As Integer = -1

                If spec.GeneIndex >= 0 AndAlso spec.GeneIndex < n AndAlso
                   String.Equals(expression.GeneNames(spec.GeneIndex), spec.GeneName, StringComparison.OrdinalIgnoreCase) Then
                    idx = spec.GeneIndex
                Else
                    ' 注意：TryGetValue 失败时会把 ByRef 参数写成默认值 0，
                    ' 因此必须先用临时变量接收，否则未知基因会被误当作 0 号基因
                    Dim resolved As Integer

                    If geneIndex.TryGetValue(spec.GeneName, resolved) Then
                        idx = resolved
                    End If
                End If

                If idx >= 0 AndAlso Not indices.Contains(idx) Then
                    Call indices.Add(idx)
                    Call names.Add(expression.GeneNames(idx))
                    Call resolvedSpecs.Add(spec)
                End If
            Next

            If indices.Count = 0 Then
                Return CreateUndefinedResult(list(0))
            End If

            ' ---- 构造扰动后的输入窗口 ----
            Dim window As Double(,) = CType(baselineWindow.Clone(), Double(,))
            Dim perturbedSet As New HashSet(Of Integer)(indices)
            Dim interventionRaw As Double() = New Double(indices.Count - 1) {}
            Dim perturbedDelta As Double() = New Double(indices.Count - 1) {}

            For k As Integer = 0 To indices.Count - 1
                Dim i As Integer = indices(k)
                Dim sd As Double = std.Max(expression.GeneSDs(i), 1.0E-08)
                ' 干预值的基线取自当前注入的 context（baselineWindow），
                ' 而不是训练集全局均值，否则残余比例会随表达水平漂移
                Dim raw As Double = std.Max(0.0, InterventionValueOf(resolvedSpecs(k), i, BaselineRawValue(i)))
                Dim normalized As Double = (raw - expression.GeneMeans(i)) / sd

                interventionRaw(k) = raw
                ' z 空间的变化量：输入由"未扰动水平"改为"敲低后水平"，这个差值就是要下传的信号
                perturbedDelta(k) = normalized - baselineWindow(i, window.GetLength(1) - 1)

                For t As Integer = 0 To window.GetLength(1) - 1
                    window(i, t) = normalized
                Next
            Next

            ' 沿先验调控边把该变化一阶注入靶基因（只改输入，模型仍然做完整前向）
            Call ApplyPerturbationChannel(window, indices, perturbedDelta)

            Dim control As Double() = ControlPrediction()
            Dim mutant As Double() = PredictRaw(window)

            ' 被扰动基因的表达由干预值直接决定，不叠加网络预测
            ' （与 GEARS 的处理一致：敲除/敲低的表型是实验施加的结果，不是模型的推断）
            For k As Integer = 0 To indices.Count - 1
                mutant(indices(k)) = interventionRaw(k)
            Next
            Dim result As New InterventionResult With {
                .Spec = If(list.Count = 1, list(0), CombineSpec(list, names, indices)),
                .WildtypeMeans = CType(control.Clone(), Double()),
                .WildtypeSDs = CType(expression.GeneSDs.Clone(), Double()),
                .MutantMeans = mutant,
                .GeneNames = CType(expression.GeneNames.Clone(), String()),
                .FoldChanges = New Double(n - 1) {},
                .PercentChanges = New Double(n - 1) {},
                .ZScores = New Double(n - 1) {},
                .IsSignificant = New Boolean(n - 1) {}
            }

            For i As Integer = 0 To n - 1
                Dim delta As Double = mutant(i) - control(i)

                result.FoldChanges(i) = delta

                If std.Abs(control(i)) > 1.0E-10 Then
                    result.PercentChanges(i) = delta / std.Abs(control(i)) * 100.0
                End If

                Dim sd As Double = std.Max(expression.GeneSDs(i), 1.0E-08)

                result.ZScores(i) = delta / sd
                result.IsSignificant(i) = perturbedSet.Contains(i) OrElse
                                          std.Abs(result.ZScores(i)) > significanceZ
            Next

            Return result
        End Function

        ''' <summary>
        ''' 组合扰动的非加性（协同/拮抗）分析。
        ''' </summary>
        ''' <param name="specs">组合干预定义</param>
        ''' <returns>联合结果、加性期望与非加性效应</returns>
        Public Function PredictCombination(specs As List(Of InterventionSpec)) As CombinationResult
            If specs Is Nothing OrElse specs.Count < 2 Then
                Throw New ArgumentException("组合扰动至少需要两个基因", NameOf(specs))
            End If

            Dim joint As InterventionResult = Predict(specs)
            Dim n As Integer = NumGenes
            Dim control As Double() = ControlPrediction()
            Dim additive As Double() = CType(control.Clone(), Double())

            ' 加性期望 = 对照 + Σ(单基因扰动效应)
            For Each spec As InterventionSpec In specs
                Dim singleResult As InterventionResult = Predict(New InterventionSpec() {spec})

                For i As Integer = 0 To n - 1
                    additive(i) += singleResult.MutantMeans(i) - control(i)
                Next
            Next

            Dim synergy As Double() = New Double(n - 1) {}
            Dim score As Double = 0.0

            For i As Integer = 0 To n - 1
                synergy(i) = joint.MutantMeans(i) - additive(i)
                score = std.Max(score, std.Abs(synergy(i)))
            Next

            Dim top As List(Of (Gene As String, Effect As Double)) =
                Enumerable.Range(0, n).
                    OrderByDescending(Function(i) std.Abs(synergy(i))).
                    Take(20).
                    Select(Function(i) (Gene:=expression.GeneNames(i), Effect:=synergy(i))).
                    ToList()

            Return New CombinationResult With {
                .Joint = joint,
                .Additive = additive,
                .Synergy = synergy,
                .SynergyScore = score,
                .TopSynergyGenes = top
            }
        End Function

        ''' <summary>
        ''' 多步递归级联模拟：扰动效应随时间推移的传播轨迹（method.md §6.3）。
        ''' </summary>
        ''' <param name="specs">干预定义</param>
        ''' <param name="steps">前向推进的时间步数</param>
        ''' <returns>
        ''' 轨迹 <c>[时间步, 基因]</c>，每个元素是长度为 1 的样本数组
        ''' （写入 <see cref="InterventionResult.DynamicTrajectory"/>）；
        ''' 第 0 步为对照预测
        ''' </returns>
        Public Function SimulateCascade(specs As IEnumerable(Of InterventionSpec), steps As Integer) As Double(,)()
            If steps < 1 Then steps = 1

            Dim list As List(Of InterventionSpec) = specs.ToList()
            Dim n As Integer = NumGenes
            Dim window As Double(,) = CType(baselineWindow.Clone(), Double(,))

            ' 施加扰动
            Dim cascadeIndices As New List(Of Integer)()
            Dim cascadeDelta As New List(Of Double)()

            For Each spec As InterventionSpec In list
                Dim idx As Integer

                If Not geneIndex.TryGetValue(spec.GeneName, idx) Then Continue For

                Dim sd As Double = std.Max(expression.GeneSDs(idx), 1.0E-08)
                Dim raw As Double = std.Max(0.0, InterventionValueOf(spec, idx, BaselineRawValue(idx)))
                Dim normalized As Double = (raw - expression.GeneMeans(idx)) / sd

                Call cascadeIndices.Add(idx)
                Call cascadeDelta.Add(normalized - baselineWindow(idx, window.GetLength(1) - 1))

                For t As Integer = 0 To window.GetLength(1) - 1
                    window(idx, t) = normalized
                Next
            Next

            Call ApplyPerturbationChannel(window, cascadeIndices, cascadeDelta.ToArray())

            Dim trajectory As Double(,)() = New Double(steps, n - 1)() {}
            Dim control As Double() = ControlPrediction()

            For i As Integer = 0 To n - 1
                trajectory(0, i) = New Double() {control(i)}
            Next

            Dim width As Integer = window.GetLength(1)

            For t As Integer = 1 To steps
                Dim normalized As Tensor = model.Forward(window, False)
                Dim raw As Double() = Denormalize(normalized)

                For i As Integer = 0 To n - 1
                    trajectory(t, i) = New Double() {raw(i)}
                Next

                ' 滑窗前进：丢弃最早一列，把本次预测的标准化值追加到末尾
                For c As Integer = 0 To width - 2
                    For i As Integer = 0 To n - 1
                        window(i, c) = window(i, c + 1)
                    Next
                Next

                For i As Integer = 0 To n - 1
                    window(i, width - 1) = normalized.Data(i)
                Next
            Next

            Return trajectory
        End Function

        ''' <summary>
        ''' 把被扰动基因在 z 空间的输入变化沿先验调控边一阶注入其靶基因。
        ''' </summary>
        ''' <param name="window">已写入扰动的输入窗口 <c>[基因, w]</c>（原地修改）</param>
        ''' <param name="perturbed">被扰动的基因行号</param>
        ''' <param name="delta">与 <paramref name="perturbed"/> 一一对应的 z 空间变化量</param>
        ''' <remarks>
        ''' <para>
        ''' 注入量 = 增益 × 出边归一化权重 × 调控方向（激活 +1 / 抑制 −1）× 该基因的 z 变化。
        ''' 靶基因的全部时间列加上同一增量，与 <see cref="BuildStaticWindow"/>
        ''' "同一条谱铺满窗口"的约定一致。
        ''' </para>
        ''' <para>
        ''' 这里只改模型的输入，不直接改写预测结果，因此下游变化仍由网络前向给出；
        ''' 关闭通道（<see cref="PerturbationChannel"/>=False）即退化为旧行为。
        ''' </para>
        ''' </remarks>
        Private Sub ApplyPerturbationChannel(window As Double(,),
                                             perturbed As List(Of Integer),
                                             delta As Double())
            If Not _perturbationChannel OrElse _perturbationChannelGain <= 0.0 Then
                Return
            End If

            Dim graph As GEARSGraph.GeneRegulatoryGraph = model.GraphData

            If graph Is Nothing Then
                Return
            End If

            Dim signs As Double() = GEARSGraph.EdgeRelationTypes.SignTable()
            Dim last As Integer = window.GetLength(1) - 1

            For k As Integer = 0 To perturbed.Count - 1
                If std.Abs(delta(k)) < 1.0E-12 Then
                    Continue For
                End If

                Dim source As Integer = perturbed(k)
                Dim targets As Integer() = graph.OutEdgeTargets(source)
                Dim types As Integer() = graph.OutEdgeTypes(source)
                Dim weights As Double() = graph.OutEdgeWeights(source)

                If targets.Length = 0 Then
                    ' 先验图只有 TF -> target 的边，非 TF 基因没有出边；退化到共调控先验
                    If _perturbationChannelCoRegulation Then
                        Call ApplyCoRegulationChannel(window, source, delta(k), last)
                    End If

                    Continue For
                End If

                For e As Integer = 0 To targets.Length - 1
                    Dim injected As Double = _perturbationChannelGain * weights(e) * signs(types(e)) * delta(k)

                    If std.Abs(injected) > MaxInjectedZ Then
                        injected = std.Sign(injected) * MaxInjectedZ
                    End If

                    Dim target As Integer = targets(e)

                    For c As Integer = 0 To last
                        window(target, c) += injected
                    Next
                Next
            Next
        End Sub

        ''' <summary>
        ''' 共调控退化路径：被扰动基因没有出边时，按"共享上游 TF"把变化摊给它的共调控基因。
        ''' </summary>
        ''' <param name="window">输入窗口（原地修改）</param>
        ''' <param name="source">被扰动基因行号</param>
        ''' <param name="delta">该基因在 z 空间的变化量</param>
        ''' <param name="last">窗口最后一列下标</param>
        ''' <remarks>
        ''' <para>
        ''' 先验图只有 TF → target 的边，本任务 300 个扰动里 232 个基因不是 TF、没有出边，
        ''' 一阶注入对它们为空操作。这里用"共享上游 TF"作为替代先验：
        ''' 对每个上游 TF 展开它的其它靶基因，按 <c>w(p←TF) × w(t←TF)</c> 累积权重，
        ''' 符号取两条边调控方向的乘积。
        ''' </para>
        ''' <para>
        ''' 这是启发式（方向不代表真实的因果方向），因此默认关闭；开启后每个扰动最多展开
        ''' <see cref="MaxCoRegulationSources"/> 个上游因子 × <see cref="MaxCoRegulationTargets"/> 个靶点，
        ''' 注入的绝对量之和约等于 增益 × |Δz|。
        ''' </para>
        ''' </remarks>
        Private Sub ApplyCoRegulationChannel(window As Double(,),
                                             source As Integer,
                                             delta As Double,
                                             last As Integer)
            Dim graph As GEARSGraph.GeneRegulatoryGraph = model.GraphData

            If graph Is Nothing Then
                Return
            End If

            Dim regulators As Integer() = graph.InEdgeSources(source)
            Dim regTypes As Integer() = graph.InEdgeTypes(source)
            Dim regWeights As Double() = graph.InEdgeWeights(source)

            If regulators.Length = 0 Then
                Return
            End If

            Dim signs As Double() = GEARSGraph.EdgeRelationTypes.SignTable()
            ' 只取调控强度最高的若干上游因子，避免枢纽 TF 造成组合爆炸
            Dim order As Integer() = Enumerable.Range(0, regulators.Length).
                OrderByDescending(Function(i) std.Abs(regWeights(i))).
                Take(MaxCoRegulationSources).
                ToArray()

            Dim acc As New Dictionary(Of Integer, Double)()

            For Each r As Integer In order
                Dim tf As Integer = regulators(r)
                Dim wSource As Double = regWeights(r) * signs(regTypes(r))

                If std.Abs(wSource) < 1.0E-12 Then
                    Continue For
                End If

                Dim targets As Integer() = graph.OutEdgeTargets(tf)
                Dim types As Integer() = graph.OutEdgeTypes(tf)
                Dim weights As Double() = graph.OutEdgeWeights(tf)
                Dim take As Integer = std.Min(targets.Length, MaxCoRegulationTargets)

                For e As Integer = 0 To take - 1
                    Dim t As Integer = targets(e)

                    If t = source Then
                        Continue For
                    End If

                    Dim contribution As Double = wSource * weights(e) * signs(types(e))
                    Dim old As Double

                    If acc.TryGetValue(t, old) Then
                        acc(t) = old + contribution
                    Else
                        acc(t) = contribution
                    End If
                Next
            Next

            If acc.Count = 0 Then
                Return
            End If

            Dim norm As Double = 0.0

            For Each value As Double In acc.Values
                norm += std.Abs(value)
            Next

            If norm <= 0.0 Then
                Return
            End If

            Dim mass As Double = _perturbationChannelGain * std.Abs(delta)
            Dim direction As Double = std.Sign(delta)

            For Each pair As KeyValuePair(Of Integer, Double) In acc
                Dim injected As Double = pair.Value / norm * mass * direction

                If std.Abs(injected) > MaxInjectedZ Then
                    injected = std.Sign(injected) * MaxInjectedZ
                End If

                For c As Integer = 0 To last
                    window(pair.Key, c) += injected
                Next
            Next
        End Sub

        ''' <summary>对输入窗口做前向并反标准化为 raw log-CPM。</summary>
        ''' <param name="window">标准化窗口</param>
        ''' <returns>raw log-CPM 尺度预测</returns>
        Private Function PredictRaw(window As Double(,)) As Double()
            Return Denormalize(model.Forward(window, False))
        End Function

        ''' <summary>把标准化预测反算回 raw log-CPM 并裁剪为非负。</summary>
        ''' <param name="normalized">标准化预测 <c>[N, 1]</c></param>
        ''' <returns>raw log-CPM 预测</returns>
        Private Function Denormalize(normalized As Tensor) As Double()
            Dim n As Integer = NumGenes
            Dim result As Double() = New Double(n - 1) {}

            For i As Integer = 0 To n - 1
                result(i) = std.Max(0.0, normalized.Data(i) * expression.GeneSDs(i) + expression.GeneMeans(i))
            Next

            Return result
        End Function

        ''' <summary>构造组合扰动的合并 spec。</summary>
        Private Function CombineSpec(specs As List(Of InterventionSpec),
                                     names As List(Of String),
                                     indices As List(Of Integer)) As InterventionSpec
            Return New InterventionSpec With {
                .GeneName = String.Join("+", names),
                .GeneIndex = indices(0),
                .Mode = specs(0).Mode
            }
        End Function

        ''' <summary>目标基因全部不在建模范围内时的降级结果。</summary>
        ''' <param name="spec">原始干预定义</param>
        ''' <returns>Undefined=True 的空结果</returns>
        Private Function CreateUndefinedResult(spec As InterventionSpec) As InterventionResult
            Dim n As Integer = NumGenes

            Return New InterventionResult With {
                .Spec = spec,
                .Undefined = True,
                .GeneNames = CType(expression.GeneNames.Clone(), String()),
                .WildtypeMeans = CType(ControlPrediction().Clone(), Double()),
                .WildtypeSDs = CType(expression.GeneSDs.Clone(), Double()),
                .MutantMeans = CType(ControlPrediction().Clone(), Double()),
                .FoldChanges = New Double(n - 1) {},
                .PercentChanges = New Double(n - 1) {},
                .ZScores = New Double(n - 1) {},
                .IsSignificant = New Boolean(n - 1) {}
            }
        End Function

    End Class

End Namespace
