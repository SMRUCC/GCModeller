' ============================================================================
' SpikingLoop.vb — 流水线外观类（readme 〇 的"总体架构总览"）
'
' 把五个阶段串成一条可复用的调用链，调用方只需关心三步：
'
'   Dim loop = New SpikingLoop(expr, prior, wgcnaAdj, config)
'   loop.Setup()          ' P0/P1：子网络筛选 → 伪时间离散化 → 先验图 → 模型装配
'   loop.Train()          ' P3  ：代理梯度 + BPTT + 结构正则训练
'   loop.VirtualPerturbate(...)   ' P4：KO/KD/OE 虚拟扰动与批量扫描
'   loop.Evaluate() / loop.LinkPrediction() / ...  ' P5：评估
'
' 阶段间的数据契约：
'   子网络筛选    →  GeneNames（决定神经元索引）+ 子先验网络
'   伪时间离散化  →  PseudotimeResult.U [T, N]（训练轨迹，归一化尺度）
'   先验图构建    →  PriorGraph（Adjacency / WInit / Mask / Confidence / Skeleton）
'   模型装配      →  SNNGRNModel（递归 LIF + 回归解码头）
'   训练          →  TrainingResult（逐轮损失与验证 PCC）
'
' 设计上有意让每个阶段都可单独调用（对外暴露 Subnetwork / Trajectory / PriorGraph /
' Model / Trainer 属性），这样才能实现 readme 六的 P2 验证点：
' "固定权重（仅用 W_init）前向演化，检查脉冲传播路径是否合理"——
' 即：Setup() 之后直接调用 loop.Model.PredictNext(...) 而不训练。
' ============================================================================

Imports Microsoft.VisualBasic.DeepLearning.SpikingNeuralNetwork
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.SpikingLoop.Data
Imports SMRUCC.genomics.Analysis.SpikingLoop.Evaluation
Imports SMRUCC.genomics.Analysis.SpikingLoop.Graph
Imports SMRUCC.genomics.Analysis.SpikingLoop.Model
Imports SMRUCC.genomics.Analysis.SpikingLoop.Perturbation
Imports SMRUCC.genomics.Analysis.SpikingLoop.Training
Imports SMRUCC.genomics.BioSystem
Imports SMRUCC.genomics.BioSystem.Core
Imports SMRUCC.genomics.BioSystem.Intervention

' 说明：本文件不使用 Namespace 语句——编译后自动落在工程 RootNamespace
' SMRUCC.genomics.Analysis.SpikingLoop 之下（与 BNLearn 的既有文件风格一致）。
''' <summary>
''' 基于脉冲神经网络的基因表达调控网络建模与虚拟扰动流水线。
''' </summary>
Public Class SpikingLoop : Implements InsilicoPerturbationExperiment

#Region "输入"

    ''' <summary>配置（超参数与开关）</summary>
    Public ReadOnly Property Config As SpikingLoopConfig

    ''' <summary>原始表达矩阵（行 = 基因，列 = 样本）</summary>
    Public ReadOnly Property Expression As GeneExpressionData

    ''' <summary>原始先验调控网络（全部基因）</summary>
    Public ReadOnly Property Prior As PriorNetwork

    ''' <summary>WGCNA 共表达关联矩阵 [N, N]（对称、[0,1]）；为 Nothing 时只用 TF-Target 骨架</summary>
    Public ReadOnly Property WgcnaAdjacency As Double(,)

    ''' <summary>
    ''' WGCNA 共表达关联矩阵的<b>提供者</b>：由调用方按子网络基因顺序返回对称矩阵 [N, N]。
    '''
    ''' 为什么需要它：矩阵的行列顺序必须与"子网络筛选之后的基因顺序"一致，
    ''' 而这个顺序只有在 <see cref="Setup"/> 内部才能确定。用委托可以在顺序确定后
    ''' 再即时生成矩阵，避免调用方重复执行一遍子网络筛选。
    ''' 与 <see cref="WgcnaAdjacency"/> 二选一，本属性优先（非 Nothing 时忽略后者）。
    '''
    ''' 注意：共表达关联矩阵的计算与文件读取属于上游 WGCNA 分析的职责，
    ''' 本流水线只消费矩阵，不负责它的产生。
    ''' </summary>
    Public Property WgcnaAdjacencyProvider As Func(Of String(), Double(,))

#End Region

#Region "阶段产物"

    ''' <summary>P0：核心子网络筛选结果</summary>
    Public Property Subnetwork As SubnetworkSelection

    ''' <summary>P1：伪时间离散化后的训练轨迹</summary>
    Public Property Trajectory As PseudotimeResult

    ''' <summary>P1：先验调控图（邻接 / 初始权重 / 掩码 / 置信度）</summary>
    Public Property PriorGraph As PriorGraph

    ''' <summary>SNN-GRN 模型</summary>
    Public Property Model As SNNGRNModel

    ''' <summary>训练器</summary>
    Public Property Trainer As SNNGRNTrainer

    ''' <summary>训练结果（未训练时为 Nothing）</summary>
    Public Property Training As TrainingResult

    ''' <summary>各阶段的诊断信息（配置告警、数据质量、脉冲健康检查等）</summary>
    Public ReadOnly Property Diagnostics As New List(Of String)()

#End Region

    Public Sub New(expr As GeneExpressionData, prior As PriorNetwork,
                   Optional wgcnaAdj As Double(,) = Nothing,
                   Optional config As SpikingLoopConfig = Nothing)

        If expr Is Nothing Then Throw New ArgumentNullException(NameOf(expr))
        If prior Is Nothing Then Throw New ArgumentNullException(NameOf(prior))

        Me.Expression = expr
        Me.Prior = prior
        Me.WgcnaAdjacency = wgcnaAdj
        Me.Config = If(config, New SpikingLoopConfig())
    End Sub

#Region "阶段一：数据准备（P0 + P1）"

    ''' <summary>
    ''' 完成全部离线数据准备与模型装配：子网络筛选 → 伪时间离散化 → 先验图 → 模型。
    ''' 该函数是幂等的（重复调用会重新构建）。
    ''' </summary>
    ''' <param name="externalPseudotime">
    ''' 外部伪时间（长度 = 原始表达矩阵的样本数）；为 Nothing 时按
    ''' 时间点标签 / 样本名时间标签自动回退。
    ''' </param>
    Public Function Setup(Optional externalPseudotime As Double() = Nothing) As SpikingLoop
        Diagnostics.Clear()

        ' ---- 配置校验 ----
        For Each w In Config.Validate()
            Diagnostics.Add($"[配置] {w}")
        Next
        If Config.Verbose Then
            Call $"[SpikingLoop] 配置：{Config}".info
        End If

        ' ---- P0：核心子网络筛选（readme 六：先选定几百~几千个核心基因） ----
        Dim selector As New GeneSubnetworkSelector(Config)
        Subnetwork = selector.Extract(Expression, Prior)
        Diagnostics.Add($"[P0 子网络] {Subnetwork}")

        If Subnetwork.MissingGenes IsNot Nothing AndAlso Subnetwork.MissingGenes.Length > 0 Then
            Diagnostics.Add($"[P0 子网络] {Subnetwork.MissingGenes.Length} 个先验基因未出现在表达矩阵中" &
                            $"（例如 {String.Join(", ", Subnetwork.MissingGenes.Take(5))}）")
        End If

        ' ---- P1：伪时间离散化（readme 一.2） ----
        Dim discretizer As New PseudotimeDiscretizer(Config)
        Trajectory = discretizer.Build(Subnetwork.Expression, externalPseudotime)
        Diagnostics.Add($"[P1 轨迹] {Trajectory}")
        For Each w In Trajectory.Diagnostics
            Diagnostics.Add($"[P1 轨迹] {w}")
        Next

        ' ---- P1：先验图构建（readme 一.1） ----
        ' WGCNA 矩阵的基因顺序必须与子网络一致，故在此（顺序已确定）才向提供者索取
        Dim wgcna = WgcnaAdjacency
        If WgcnaAdjacencyProvider IsNot Nothing Then
            wgcna = WgcnaAdjacencyProvider(Subnetwork.GeneNames)
        End If

        Dim builder As New PriorGraphBuilder(Config)
        PriorGraph = builder.Build(Subnetwork.GeneNames, Subnetwork.Prior, wgcna)
        Diagnostics.Add($"[P1 先验图] {PriorGraph}")
        If PriorGraph.NumSkippedEdges > 0 Then
            Diagnostics.Add($"[P1 先验图] 跳过 {PriorGraph.NumSkippedEdges} 条调控边" &
                            "（自环 / 重复边 / 基因名未命中）")
        End If
        If PriorGraph.NumSynapses = 0 Then
            Diagnostics.Add("[P1 先验图] 邻接矩阵为空：网络没有任何突触，模型只能靠输入电流拟合")
        End If

        ' ---- 模型装配 ----
        Model = New SNNGRNModel(PriorGraph, Config)
        Trainer = New SNNGRNTrainer(Model, Trajectory, Config)
        Diagnostics.Add($"[模型] {Model}")
        Diagnostics.Add($"[训练窗口] 总计 {Trainer.TotalWindows}（训练 {Trainer.TrainWindows} / " &
                        $"验证 {Trainer.ValidationWindows}，Δt={Config.Horizon}）")

        If Config.Verbose Then
            For Each d In Diagnostics
                Call d.info
            Next
        End If

        Return Me
    End Function

#End Region

#Region "阶段二：训练（P3）"

    ''' <summary>
    ''' 训练模型（readme 三）。
    ''' </summary>
    ''' <param name="onEpoch">每轮结束后的回调（用于打印损失曲线）</param>
    Public Function Train(Optional onEpoch As Action(Of TrainingRecord) = Nothing) As TrainingResult
        EnsureReady()

        ' ---- P2 验证点：训练前的固定权重前向诊断 ----
        Dim probe = ProbeForward()
        For Each w In probe
            Diagnostics.Add($"[P2 前向自检] {w}")
            If Config.Verbose Then Call w.warning
        Next

        Training = Trainer.Train(onEpoch)

        ' ---- 训练后的脉冲健康检查 ----
        For Each w In Model.Layer.Diagnose()
            Diagnostics.Add($"[训练后自检] {w}")
        Next

        Return Training
    End Function

    ''' <summary>
    ''' readme 六 的 P2 验证点：用当前（未训练）权重做一次前向演化，
    ''' 检查"脉冲传播路径是否合理、有无全体持续发放/静默"。
    ''' </summary>
    Public Function ProbeForward() As List(Of String)
        EnsureReady()

        ' 用第一个时间窗作为输入，观察整批时间窗的脉冲统计
        Dim n = Trajectory.NumGenes
        Dim batch As Tensor = Trajectory.U
        Dim output = Model.Forward(Model.EncodeCurrents(batch), Model.MapToMembrane(batch))

        Dim warns = output.Diagnose()
        Dim total = SpikeDecoders.TotalSpikeCount(output.SHistory)
        Diagnostics.Add($"[P2 前向自检] 一次性前向 {Trajectory.NumBins} 个时间窗，" &
                        $"总脉冲数 = {total:F0}，平均发放率 = " &
                        $"{total / (output.TimeSteps * CDbl(Trajectory.NumBins) * n):P1}")

        Return warns
    End Function

#End Region

#Region "阶段三：评估（P5）"

    ''' <summary>表达预测精度（readme 五.1，在整条训练轨迹上评估）</summary>
    Public Function Evaluate() As RegressionMetrics
        EnsureReady()
        Dim predicted = Trainer.PredictAll()
        Return RegressionMetrics.Compute(predicted.predicted, predicted.actual)
    End Function

    ''' <summary>调控关系合理性 / 权重恢复质量（readme 五.2）</summary>
    Public Function LinkPrediction(Optional topK As Integer = 50) As LinkPredictionReport
        EnsureReady()
        Return LinkPredictionMetrics.Evaluate(PriorGraph, Model.LearnedWeights, topK)
    End Function

    ''' <summary>虚拟扰动方向一致性（readme 五.3 的等价替代）</summary>
    Public Function PerturbationDirectionConsistency(Optional threshold As Double = 0.001,
                                                    Optional maxTfs As Integer = 0) As PerturbationConsistencyReport
        EnsureReady()
        Return PerturbationConsistency.Evaluate(Engine(), Baseline(), Subnetwork.Prior,
                                                Subnetwork.GeneNames, threshold, maxTfs)
    End Function

    ''' <summary>扰动传播时序合理性（readme 五.4）</summary>
    ''' <remarks>
    ''' 注意：必须使用扰动轨迹中<b>已解析基因索引</b>的 spec（<c>trajectory.Specs</c>）。
    ''' 调用方传入的 spec 通常只填了 GeneName、GeneIndex 仍为 −1，而 BFS 起点依赖索引；
    ''' 若沿用未解析的 spec，所有基因的层级深度都会是"不可达"，评估结果为空。
    ''' </remarks>
    Public Function ResponseOnset(spec As PerturbationSpec,
                                 Optional threshold As Double = 0.01) As ResponseOnsetReport
        EnsureReady()

        Dim trajectory = VirtualPerturbate(New PerturbationSpec() {spec})
        Dim resolved = If(trajectory.Specs IsNot Nothing AndAlso trajectory.Specs.Length > 0,
                          trajectory.Specs(0), spec)

        Return ResponseOnsetAnalysis.Evaluate(trajectory, PriorGraph, resolved, threshold)
    End Function

#End Region

#Region "阶段四：虚拟扰动（P4）"

    ''' <summary>
    ''' 基线表达状态 [1, N]（readme 四的 <c>U_seq[baseline_timepoint]</c>）。
    ''' </summary>
    ''' <param name="timepoint">伪时间窗索引；−1（默认）表示取轨迹末端——即"发育终点"状态</param>
    Public Function Baseline(Optional timepoint As Integer = -1) As Tensor
        EnsureReady()

        Dim t = If(timepoint < 0, Trajectory.NumBins - 1, timepoint)
        If t < 0 OrElse t >= Trajectory.NumBins Then
            Throw New ArgumentOutOfRangeException(NameOf(timepoint),
                $"伪时间窗索引应在 [0, {Trajectory.NumBins}) 内，实际 {timepoint}")
        End If

        Dim n = Trajectory.NumGenes
        Dim row(n - 1) As Double
        Array.Copy(Trajectory.U.Data, t * n, row, 0, n)
        Return Tensor.Wrap(row, 1, n)
    End Function

    ''' <summary>在给定基线上执行一次虚拟扰动（单基因或多基因组合）</summary>
    ''' <param name="specs">扰动方案（可多基因组合）</param>
    ''' <param name="baseExpression">
    ''' 基线表达状态 [1, N]；为 Nothing 时使用 <see cref="Baseline"/> 的默认取值。
    ''' 注意参数名不可写作 <c>baseline</c>——VB 名称解析不区分大小写，会遮蔽同名的
    ''' <see cref="Baseline"/> 方法，导致 <c>Baseline()</c> 被解析为 Tensor 的默认索引器。
    ''' </param>
    Public Function VirtualPerturbate(specs As PerturbationSpec(),
                                      Optional baseExpression As Tensor = Nothing) As PerturbationTrajectory
        EnsureReady()

        Dim baseState As Tensor = baseExpression
        If baseState Is Nothing Then baseState = Baseline()

        Return Engine().Run(baseState, specs)
    End Function

    ''' <summary>
    ''' 批量扰动扫描（Perturb-seq 模拟）：逐个敲低候选基因并汇总下游效应，
    ''' 按影响强度排序得到关键调控节点。
    ''' </summary>
    ''' <param name="baseExpression">基线表达状态 [1, N]（见 <see cref="VirtualPerturbate"/> 的参数说明）</param>
    Public Function BatchScan(genes As String(),
                              Optional mode As InterventionMode = InterventionMode.Knockdown,
                              Optional strength As Double = 0.5,
                              Optional keepTrajectories As Boolean = False,
                              Optional baseExpression As Tensor = Nothing) As BatchPerturbationResult
        EnsureReady()

        Dim baseState As Tensor = baseExpression
        If baseState Is Nothing Then baseState = Baseline()

        Return Engine().Scan(baseState, genes, mode, strength, keepTrajectories)
    End Function

    ''' <summary>构建虚拟扰动引擎（模型 + 轨迹反变换器）</summary>
    Public Function Engine() As VirtualPerturbationEngine
        EnsureReady()
        Return New VirtualPerturbationEngine(Model, Trajectory)
    End Function

#End Region

#Region "辅助"

    ''' <summary>当前网络中的转录因子（= 在先验网络中作为 TF 出现、且自身在子网络内的基因）</summary>
    Public Function TranscriptionFactors() As String()
        EnsureReady()

        Dim known As New HashSet(Of String)(Subnetwork.GeneNames, StringComparer.OrdinalIgnoreCase)
        Dim tfs As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        For Each e In Subnetwork.Prior.Edges
            If known.Contains(e.TF) Then tfs.Add(e.TF)
        Next

        Return tfs.ToArray()
    End Function

    ''' <summary>一次可读的流水线摘要（供控制台演示打印）</summary>
    Public Function Summary() As String
        Dim sb As New System.Text.StringBuilder()
        sb.AppendLine($"  配置          : {Config}")
        If Subnetwork IsNot Nothing Then
            sb.AppendLine($"  子网络        : {Subnetwork.NumGenes} 基因（候选 {Subnetwork.NumCandidates}），" &
                          $"{Subnetwork.NumEdges} 条调控边")
        End If
        If Trajectory IsNot Nothing Then
            sb.AppendLine($"  训练轨迹      : T={Trajectory.NumBins}, N={Trajectory.NumGenes}，" &
                          $"来源={Trajectory.Source}，平均 {Trajectory.MeanSamplesPerBin:F1} 样本/窗")
        End If
        If PriorGraph IsNot Nothing Then
            sb.AppendLine($"  先验图        : {PriorGraph.NumSynapses} 突触（TF-Target {PriorGraph.NumPriorEdges}，" &
                          $"WGCNA {PriorGraph.NumWgcnaEdges}），密度 {PriorGraph.Density:P3}，" &
                          $"冻结位置 {PriorGraph.NumFrozenSynapses}")
        End If
        If Model IsNot Nothing Then
            sb.AppendLine($"  模型          : {Model}")
        End If
        If Training IsNot Nothing Then
            sb.AppendLine($"  训练          : {Training}")
        End If
        Return sb.ToString()
    End Function

    ''' <summary>检查流水线是否已完成 Setup()</summary>
    Public Function IsReady() As Boolean
        Return Model IsNot Nothing AndAlso Trajectory IsNot Nothing AndAlso PriorGraph IsNot Nothing
    End Function

    Private Sub EnsureReady()
        If IsReady() Then Return
        Throw New InvalidOperationException(
            "流水线尚未完成数据准备：请先调用 Setup()（子网络筛选 → 伪时间离散化 → 先验图 → 模型装配）")
    End Sub

#End Region

    Public Overrides Function ToString() As String
        Return $"SpikingLoop(genes={If(Subnetwork Is Nothing, 0, Subnetwork.NumGenes)}, " &
               $"bins={If(Trajectory Is Nothing, 0, Trajectory.NumBins)}, trained={Training IsNot Nothing})"
    End Function

#Region "上下文与虚拟扰动（InsilicoPerturbationExperiment）"

    ''' <summary>
    ''' 上下文状态：默认 <see cref="ContextAlignmentMode.Affine"/>。
    ''' </summary>
    ''' <remarks>
    ''' 网络的输入是 MinMax 归一化到 <c>[0,1]</c> 的表达量，因此外部上下文必须先换算到同一空间。
    ''' 这里把 <c>Means = GlobalMin</c>、<c>StandardDeviations = GlobalMax - GlobalMin</c> 交给
    ''' 公共组件的仿射换算，得到的正是 <c>(x - min) / range</c>，即模型训练时用的同一变换。
    ''' </remarks>
    Private m_context As ContextAlignmentState
    Private m_contextBaseline As Tensor

    Private ReadOnly Property ContextState As ContextAlignmentState
        Get
            If m_context Is Nothing Then
                m_context = New ContextAlignmentState(ContextAlignmentMode.Affine)
            End If

            Return m_context
        End Get
    End Property

    ''' <summary>当前生效的上下文对齐模式。</summary>
    Public ReadOnly Property ContextAlignmentMode As ContextAlignmentMode
        Get
            Return ContextState.Mode
        End Get
    End Property

    ''' <summary>最近一次上下文注入的诊断。</summary>
    Public ReadOnly Property ContextDiagnostics As ContextAlignmentDiagnostics
        Get
            Return ContextState.Diagnostics
        End Get
    End Property

    ''' <summary>是否已经注入了上下文。</summary>
    Public ReadOnly Property HasContext As Boolean
        Get
            Return m_contextBaseline IsNot Nothing
        End Get
    End Property

    ''' <summary>
    ''' 把外部表达状态注入为虚拟扰动的基线。
    ''' </summary>
    ''' <param name="context">基因名 → 原始表达量（大小写不敏感；可以只覆盖部分基因）</param>
    ''' <returns>当前对象（便于链式调用）</returns>
    ''' <remarks>
    ''' 未覆盖的基因沿用训练轨迹末端的基线（<see cref="Baseline"/>）在模型空间中的取值，
    ''' 因此上下文可以是部分覆盖的“样本环境”。
    ''' </remarks>
    ''' <exception cref="InvalidOperationException">上下文没有任何基因命中子网络</exception>
    Public Function SetContext(context As Dictionary(Of String, Double)) As InsilicoPerturbationExperiment Implements InsilicoPerturbationExperiment.SetContext
        Call EnsureReady()

        Dim aligned As ContextAlignmentResult = ContextState.SetContext(context, buildContextReference())

        If aligned.Diagnostics.Matched = 0 Then
            Throw New InvalidOperationException("上下文中没有任何基因能够映射到当前子网络的基因集合上")
        End If

        Call applyContextBaseline(aligned)

        Return Me
    End Function

    ''' <summary>切换上下文对齐模式；已注入上下文时会立即重新对齐。</summary>
    ''' <param name="mode">新的对齐模式</param>
    ''' <returns>当前对象（便于链式调用）</returns>
    Public Function SetContextAlignmentMode(mode As ContextAlignmentMode) As InsilicoPerturbationExperiment Implements InsilicoPerturbationExperiment.SetContextAlignmentMode
        Call EnsureReady()
        Call ContextState.SetMode(mode)

        If ContextState.HasContext Then
            Call applyContextBaseline(ContextState.Result)
        End If

        Return Me
    End Function

    ''' <summary>撤销上下文，回到训练轨迹末端推导出的基线。</summary>
    Public Sub ClearContext()
        Call ContextState.Clear()

        m_contextBaseline = Nothing
    End Sub

    ''' <summary>虚拟敲除：把目标基因的输入电流置零。</summary>
    Public Function KnockoutGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult Implements InsilicoPerturbationExperiment.KnockoutGene
        Return perturb(PerturbationSpec.Knockout(geneName))
    End Function

    ''' <summary>虚拟过表达：给目标基因叠加一个最大幅度的输入电流。</summary>
    Public Function OverexpressGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult Implements InsilicoPerturbationExperiment.OverexpressGene
        Return perturb(PerturbationSpec.Overexpress(geneName))
    End Function

    ''' <summary>虚拟敲低：把目标基因的输入电流衰减 50%。</summary>
    Public Function KnockDownGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult Implements InsilicoPerturbationExperiment.KnockDownGene
        Return perturb(PerturbationSpec.Knockdown(geneName))
    End Function

    ''' <summary>执行一次扰动并积分，把终末状态转成标准的干预结果（原始表达量尺度）。</summary>
    Private Function perturb(spec As PerturbationSpec) As InterventionResult
        Call EnsureReady()
        Call spec.ResolveIndex(NetworkGeneNames())

        Dim trajectory As PerturbationTrajectory = VirtualPerturbate({spec}, m_contextBaseline)
        Dim n As Integer = NetworkGeneNames().Length
        Dim control As Double() = denormalize(trajectory.Baseline)
        Dim mutant As Double() = control

        If trajectory.Denormalized IsNot Nothing AndAlso trajectory.Denormalized.Length > 0 Then
            mutant = trajectory.Denormalized(trajectory.Denormalized.Length - 1)
        ElseIf trajectory.Trajectory IsNot Nothing AndAlso trajectory.Trajectory.Length > 0 Then
            mutant = denormalize(trajectory.Trajectory(trajectory.Trajectory.Length - 1))
        End If

        Dim fold(n - 1) As Double
        Dim percent(n - 1) As Double
        Dim z(n - 1) As Double
        Dim significant(n - 1) As Boolean

        For i As Integer = 0 To n - 1
            fold(i) = mutant(i) - control(i)

            If System.Math.Abs(control(i)) > 1.0E-10 Then
                percent(i) = fold(i) / System.Math.Abs(control(i)) * 100.0R
            End If

            significant(i) = i = spec.GeneIndex OrElse System.Math.Abs(fold(i)) > 1.0E-09
        Next

        Return New InterventionResult With {
            .Spec = New InterventionSpec With {
                .GeneName = spec.GeneName,
                .GeneIndex = spec.GeneIndex,
                .Mode = spec.Mode
            },
            .GeneNames = NetworkGeneNames(),
            .WildtypeMeans = control,
            .MutantMeans = mutant,
            .FoldChanges = fold,
            .PercentChanges = percent,
            .ZScores = z,
            .IsSignificant = significant,
            .Undefined = Not trajectory.Applied
        }
    End Function

    ''' <summary>把对齐结果写进模型空间的基线张量（未覆盖的基因沿用训练基线）。</summary>
    Private Sub applyContextBaseline(aligned As ContextAlignmentResult)
        Dim names As String() = NetworkGeneNames()
        Dim data As Double() = CType(Baseline().Data.Clone(), Double())

        For Each item As KeyValuePair(Of String, Double) In aligned.Values
            Dim idx As Integer = Array.FindIndex(
                names,
                Function(g) String.Equals(g, item.Key, StringComparison.OrdinalIgnoreCase))

            If idx >= 0 Then
                data(idx) = item.Value
            End If
        Next

        m_contextBaseline = Tensor.Wrap(data, 1, names.Length)
    End Sub

    ''' <summary>构造公共对齐组件所需的训练侧统计量（MinMax 参数放在均值/标准差位置）。</summary>
    Private Function buildContextReference() As ContextAlignmentReference
        Dim names As String() = NetworkGeneNames()
        Dim n As Integer = names.Length
        Dim mins(n - 1) As Double
        Dim ranges(n - 1) As Double
        Dim population(n - 1)() As Double
        Dim bins As Integer = Trajectory.NumBins
        Dim u As Double() = Trajectory.U.Data

        For i As Integer = 0 To n - 1
            Dim values(bins - 1) As Double

            For t As Integer = 0 To bins - 1
                values(t) = u(t * n + i)
            Next

            population(i) = values
            mins(i) = Trajectory.GlobalMin

            Dim range As Double = Trajectory.GlobalMax - Trajectory.GlobalMin

            ' 极差为 0（未归一化或常量矩阵）时退化为恒等映射
            ranges(i) = If(range > 0.0, range, 1.0R)
        Next

        Return New ContextAlignmentReference With {
            .GeneNames = names,
            .Means = mins,
            .StandardDeviations = ranges,
            .Population = population
        }
    End Function

    ''' <summary>子网络基因名（顺序与模型输入一致）。</summary>
    Private Function NetworkGeneNames() As String()
        Return Subnetwork.GeneNames
    End Function

    ''' <summary>把归一化空间的一行数据反变换回原始表达量。</summary>
    Private Function denormalize(values As Double()) As Double()
        If values Is Nothing Then
            Return Nothing
        End If

        If Trajectory Is Nothing Then
            Return CType(values.Clone(), Double())
        End If

        Dim scale As Double = Trajectory.GlobalMax - Trajectory.GlobalMin
        Dim result(values.Length - 1) As Double

        If scale <= 0.0 Then
            Return CType(values.Clone(), Double())
        End If

        For i As Integer = 0 To values.Length - 1
            result(i) = Trajectory.GlobalMin + values(i) * scale
        Next

        Return result
    End Function

#End Region

End Class
