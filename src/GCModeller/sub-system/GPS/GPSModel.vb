Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.BNLearn
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.BNLearn.Intervention
Imports SMRUCC.genomics.Analysis.GPS.Config
Imports SMRUCC.genomics.Analysis.GPS.Data
Imports SMRUCC.genomics.Analysis.GPS.Graph
Imports SMRUCC.genomics.Analysis.GPS.Model
Imports SMRUCC.genomics.Analysis.GPS.Perturbation
Imports SMRUCC.genomics.Analysis.GPS.Training
Imports GEARSGraph = SMRUCC.genomics.Analysis.GEARS.Graph

''' <summary>
''' GPS（GNN + Transformer）基因表达调控网络模型的门面。
''' </summary>
''' <remarks>
''' <para>
''' 一个实例完整封装"先验网络 + 时序表达矩阵 → 预处理 → 图编码 → 模型 → 训练 →
''' 虚拟扰动"的全流程，并实现 <see cref="InsilicoPerturbationExperiment"/>，
''' 因此可以与 BNLearn 的贝叶斯网络实现、GEARS 实现互换使用。
''' </para>
''' <para>
''' 典型用法：
''' <code>
''' Dim gps As New GPSModel(expression, prior)
''' gps.Train()
''' Dim ko As InterventionResult = gps.KnockoutGene("TF1")
''' </code>
''' </para>
''' <para>
''' 规模约束：全局注意力的复杂度是 O(N²)，第一期建议 <see cref="GPSConfig.MaxGenes"/>
''' 不超过 2000。超过 <see cref="GraphEncoding.LapPeMaxNodes"/> 时位置编码会自动置零。
''' </para>
''' </remarks>
Public Class GPSModel
    Implements InsilicoPerturbationExperiment

    Private ReadOnly rawExpression As GeneExpressionData
    Private ReadOnly priorNetwork As PriorNetwork
    Private ReadOnly cfg As GPSConfig

    Private ReadOnly preprocessedExpression As PreprocessedExpression
    Private ReadOnly windowDataset As TimeSeriesWindowDataset
    Private ReadOnly regulatoryGraph As GEARSGraph.GeneRegulatoryGraph
    Private ReadOnly graphEncoding As GraphEncoding
    Private ReadOnly grn As GRNModel
    Private ReadOnly trainer As GPSTrainer
    Private ReadOnly engine As VirtualPerturbationEngine

    ''' <summary>
    ''' 训练数据推导出的对照窗口；<see cref="ResetContext"/> 用它把上下文条件恢复到训练基线。
    ''' </summary>
    Private ReadOnly trainingBaseline As Double(,)

    Private _lastResult As GPSTrainingResult
    Private _trainerState As GPSTrainingState

    ''' <summary>实际生效的配置（窗口长度可能因时间点不足而被裁剪）</summary>
    Public ReadOnly Property Config As GPSConfig
        Get
            Return cfg
        End Get
    End Property

    ''' <summary>
    ''' 周期性 checkpoint 的输出路径；Nothing 或空表示不落盘。
    ''' </summary>
    ''' <remarks>
    ''' 与 <see cref="GPSConfig.CheckpointIntervalEpochs"/> 配合使用：间隔 &gt; 0 且路径非空时，
    ''' 训练循环每到间隔 epoch 就把模型包原子写入该路径（覆盖式滚动更新）。
    ''' 进程意外中止后，可对该文件执行 <see cref="GPSFile.Load"/> 再以
    ''' <see cref="Train(Boolean, Integer)"/> 的 <c>resumeFromCheckpoint:=True</c> 续训。
    ''' </remarks>
    Public Property CheckpointPath As String

    ''' <summary>
    ''' 最近一次 checkpoint（或训练收尾）时捕获的 trainer 侧状态；
    ''' 未经训练/未加载含状态的包时为 Nothing。
    ''' </summary>
    Public ReadOnly Property TrainerState As GPSTrainingState
        Get
            Return _trainerState
        End Get
    End Property

    ''' <summary>底层 GRN 模型</summary>
    Public ReadOnly Property Model As GRNModel
        Get
            Return grn
        End Get
    End Property

    ''' <summary>输入的时间序列表达矩阵（原始值，未标准化）</summary>
    ''' <remarks>持久化时需要保存该输入以在加载端重建预处理与滑窗数据集。</remarks>
    Public ReadOnly Property Expression As GeneExpressionData
        Get
            Return rawExpression
        End Get
    End Property

    ''' <summary>输入的先验调控网络</summary>
    Public ReadOnly Property Prior As PriorNetwork
        Get
            Return priorNetwork
        End Get
    End Property

    ''' <summary>预处理后的表达数据</summary>
    Public ReadOnly Property Preprocessed As PreprocessedExpression
        Get
            Return preprocessedExpression
        End Get
    End Property

    ''' <summary>滑窗数据集（含严格时间切分）</summary>
    Public ReadOnly Property Dataset As TimeSeriesWindowDataset
        Get
            Return windowDataset
        End Get
    End Property

    ''' <summary>基因调控图</summary>
    Public ReadOnly Property GraphData As GEARSGraph.GeneRegulatoryGraph
        Get
            Return regulatoryGraph
        End Get
    End Property

    ''' <summary>静态图编码</summary>
    Public ReadOnly Property Encoding As GraphEncoding
        Get
            Return graphEncoding
        End Get
    End Property

    ''' <summary>基因名称（建模顺序）</summary>
    Public ReadOnly Property GeneNames As String()
        Get
            Return preprocessedExpression.GeneNames
        End Get
    End Property

    ''' <summary>是否已执行过训练</summary>
    Public ReadOnly Property IsTrained As Boolean
        Get
            Return _lastResult IsNot Nothing
        End Get
    End Property

    ''' <summary>最近一次训练结果</summary>
    Public ReadOnly Property LastTrainingResult As GPSTrainingResult
        Get
            Return _lastResult
        End Get
    End Property

    ''' <summary>
    ''' 由时序表达矩阵与先验调控网络构建模型。
    ''' </summary>
    ''' <param name="expression">时序基因表达矩阵（行=基因，列=样本，需带 TimePoints）</param>
    ''' <param name="prior">先验调控网络（TF + motif 扫描结果）</param>
    ''' <param name="config">超参数；Nothing 时使用默认配置</param>
    Public Sub New(expression As GeneExpressionData,
                   Optional prior As PriorNetwork = Nothing,
                   Optional config As GPSConfig = Nothing)

        If expression Is Nothing Then Throw New ArgumentNullException(NameOf(expression))

        Me.rawExpression = expression
        Me.priorNetwork = If(prior, New PriorNetwork())
        Me.cfg = If(config, New GPSConfig())

        If Not Me.cfg.Validate() Then
            Throw New ArgumentException("GPSConfig 参数组合非法", NameOf(config))
        End If

        ' ---- 1. 预处理（高变异过滤时保留先验网络中的 TF） ----
        Me.preprocessedExpression = ExpressionPreprocess.Preprocess(Me.rawExpression, Me.cfg, Me.priorNetwork.TFNames)

        ' ---- 2. 图构建与静态编码 ----
        Me.regulatoryGraph = New GEARSGraph.GeneRegulatoryGraph(
            Me.preprocessedExpression.GeneNames,
            Me.priorNetwork,
            Me.preprocessedExpression.Matrix,
            Me.cfg.CoexpressionTopK,
            Me.cfg.MinCoexpression)

        Me.graphEncoding = GraphEncoding.Compute(Me.regulatoryGraph, Me.cfg)

        ' ---- 3. 滑窗数据集（按时间严格切分） ----
        Me.windowDataset = TimeSeriesWindowDataset.Create(Me.preprocessedExpression, Me.cfg)

        If Me.windowDataset.WindowSize < 2 Then
            Throw New ArgumentException($"时序表达至少需要 3 个不同时间点，实际 {Me.windowDataset.NTimePoint} 个")
        End If

        ' 时间点不足时窗口会被裁剪，模型必须使用裁剪后的窗口长度
        Dim effective As GPSConfig = Me.cfg.Clone()
        effective.Window = Me.windowDataset.WindowSize
        Me.cfg = effective

        ' ---- 4. 模型与训练器 ----
        Me.grn = New GRNModel(Me.regulatoryGraph, Me.graphEncoding, Me.cfg)
        Me.trainer = New GPSTrainer(Me.grn, Me.cfg)
        Me.trainingBaseline = BuildBaselineWindow()
        Me.engine = New VirtualPerturbationEngine(
            Me.grn, Me.Preprocessed, Me.trainingBaseline,
            Me.cfg.SignificanceZScore, Me.cfg.KnockdownResidual)
    End Sub

    ''' <summary>
    ''' 训练模型：掩码重建预训练 + 下一时刻预测微调。
    ''' </summary>
    ''' <param name="printEvery">打印间隔；0 表示静默，-1 表示使用配置值</param>
    ''' <returns>训练结果</returns>
    ''' <remarks>
    ''' 每次都从第 0 epoch 全新开始（内部先重置 trainer 状态），
    ''' 与启用 checkpoint 之前的行为逐位一致。
    ''' </remarks>
    Public Function Train(Optional printEvery As Integer = -1) As GPSTrainingResult
        Return Train(resumeFromCheckpoint:=False, printEvery:=printEvery)
    End Function

    ''' <summary>
    ''' 训练模型（可从中断的 checkpoint 续训）。
    ''' </summary>
    ''' <param name="resumeFromCheckpoint">为 True 时从 <see cref="TrainerState"/> 记录的进度续跑：
    ''' 跳过已完成的阶段与 epoch，恢复早停状态与损失曲线前缀；为 False 时全新训练</param>
    ''' <param name="printEvery">打印间隔；0 表示静默，-1 表示使用配置值</param>
    ''' <returns>训练结果（损失曲线为历史前缀与新段拼接后的完整曲线）</returns>
    ''' <remarks>
    ''' <paramref name="resumeFromCheckpoint"/> 为 True 但没有任何已持久化的训练状态时，
    ''' 退化为全新训练（与 <see cref="Train(Integer)"/> 等价）。
    ''' </remarks>
    Public Function Train(resumeFromCheckpoint As Boolean, Optional printEvery As Integer = -1) As GPSTrainingResult
        If resumeFromCheckpoint Then
            Call trainer.RestoreState(_trainerState)
        Else
            Call trainer.ResetTrainingState()
        End If

        trainer.CheckpointInterval = cfg.CheckpointIntervalEpochs

        If cfg.CheckpointIntervalEpochs > 0 AndAlso Not String.IsNullOrWhiteSpace(CheckpointPath) Then
            trainer.OnCheckpoint =
                Sub(state)
                    _trainerState = state
                    Call SaveCheckpoint()
                End Sub
        Else
            trainer.OnCheckpoint = Nothing
        End If

        Dim pretrain As Double() = trainer.Pretrain(windowDataset.Train(), -1, printEvery).ToArray()
        Dim result As GPSTrainingResult = trainer.Finetune(windowDataset.Train(), windowDataset.Validation(), -1, printEvery)

        result.PretrainLoss = pretrain
        _lastResult = result

        ' 训练收尾时刷新一次 trainer 状态，使收尾后的 Save 同样携带完整现场
        _trainerState = trainer.CaptureState()

        Return result
    End Function

    ''' <summary>
    ''' 把当前模型与 trainer 状态写入 <see cref="CheckpointPath"/>（原子替换）。
    ''' 写盘失败仅打印警告，不会中断训练。
    ''' </summary>
    Private Sub SaveCheckpoint()
        Try
            Call GPSFile.SaveCheckpoint(Me, CheckpointPath, _trainerState)
        Catch ex As Exception
            Console.WriteLine($"  [GPS] checkpoint 写入失败（训练继续）：{ex.Message}")
        End Try
    End Sub

    ''' <summary>
    ''' 回填训练结果，用于从持久化包恢复模型时标记"已训练"状态。
    ''' </summary>
    ''' <param name="result">训练结果；传入 Nothing 会清除已训练标记</param>
    ''' <remarks>
    ''' 参数由 <see cref="GPSFile.Load"/> 在重建模型后写入；
    ''' 常规训练路径仍然通过 <see cref="Train"/> 自动设置。
    ''' </remarks>
    Public Sub AttachTrainingResult(result As GPSTrainingResult)
        _lastResult = result
    End Sub

    ''' <summary>
    ''' 回填训练结果与 trainer 侧状态（从 checkpoint 恢复续训用）。
    ''' </summary>
    ''' <param name="result">训练结果（损失曲线与最优 epoch 元信息）</param>
    ''' <param name="state">训练器状态（步计数/阶段进度/早停/最优快照）；可为 Nothing</param>
    ''' <remarks>
    ''' 由 <see cref="GPSFile.Load"/> 调用；之后即可用
    ''' <see cref="Train(Boolean, Integer)"/> 的 <c>resume:=True</c> 续训。
    ''' </remarks>
    Public Sub AttachTrainingResult(result As GPSTrainingResult, state As GPSTrainingState)
        _lastResult = result
        _trainerState = state
    End Sub

    ''' <summary>
    ''' 在测试集（时间上最晚的一段）上评估；测试集为空时回退到验证集。
    ''' </summary>
    ''' <returns>评估指标</returns>
    Public Function Evaluate() As EvaluationMetrics
        Dim windows As List(Of TimeSeriesWindow) = windowDataset.Test()

        If windows.Count = 0 Then windows = windowDataset.Validation()

        Return Evaluator.Evaluate(grn, windows, 20, Evaluator.IsolatedGeneMask(grn))
    End Function

    ''' <summary>
    ''' 历史持续性基线（同一测试集）。
    ''' </summary>
    ''' <returns>基线指标</returns>
    Public Function EvaluateBaseline() As EvaluationMetrics
        Dim windows As List(Of TimeSeriesWindow) = windowDataset.Test()

        If windows.Count = 0 Then windows = windowDataset.Validation()

        Return Evaluator.EvaluatePersistence(windows, Evaluator.IsolatedGeneMask(grn))
    End Function

    ' ==================== 上下文条件化 ====================

    ''' <summary>
    ''' 敲低（CRISPRi）后目标基因保留的表达比例。
    ''' </summary>
    ''' <returns>取值区间 <c>[0,1)</c>，默认 0.2</returns>
    ''' <remarks>
    ''' 修改该值会清空扰动引擎的对照预测缓存，下一次调用会按新比例重新推理。
    ''' </remarks>
    Public Property KnockdownResidual As Double
        Get
            Return engine.KnockdownResidual
        End Get
        Set(value As Double)
            engine.KnockdownResidual = value
        End Set
    End Property

    ''' <summary>
    ''' 逐基因的 log-CPM 均值（预处理时统计得到，反标准化用）。
    ''' </summary>
    ''' <returns>长度等于 <see cref="GeneNames"/> 的均值向量</returns>
    Public ReadOnly Property GeneMeans As Double()
        Get
            Return preprocessedExpression.GeneMeans
        End Get
    End Property

    ''' <summary>
    ''' 逐基因的 log-CPM 标准差（预处理时统计得到，反标准化用）。
    ''' </summary>
    ''' <returns>长度等于 <see cref="GeneNames"/> 的标准差向量</returns>
    Public ReadOnly Property GeneSDs As Double()
        Get
            Return preprocessedExpression.GeneSDs
        End Get
    End Property

    ''' <summary>
    ''' 每个训练样本的原始文库大小（CPM 归一化的分母）。
    ''' </summary>
    ''' <returns>长度等于样本数的文库大小向量</returns>
    Public ReadOnly Property LibrarySizes As Double()
        Get
            Return preprocessedExpression.LibrarySizes
        End Get
    End Property

    ''' <summary>
    ''' 把外部细胞上下文注入模型，作为后续虚拟扰动的对照基线。
    ''' </summary>
    ''' <param name="genes">上下文表达谱的基因名（大小写不敏感；可只给出一部分基因）</param>
    ''' <param name="logCpm">
    ''' 与 <paramref name="genes"/> 一一对应的 <b>log2(CPM + 1)</b> 表达值；
    ''' 由调用方完成文库归一化与 log 变换，模型只负责 z-score。
    ''' </param>
    ''' <remarks>
    ''' 未出现在 <paramref name="genes"/> 中的建模基因会填入该基因的
    ''' <see cref="GeneMeans"/>（标准化后为 0，即"等同对照均值"），
    ''' 因此上下文可以只覆盖测序面板内的基因。
    ''' </remarks>
    Public Sub SetContext(genes As String(), logCpm As Double())
        If genes Is Nothing Then Throw New ArgumentNullException(NameOf(genes))
        If logCpm Is Nothing Then Throw New ArgumentNullException(NameOf(logCpm))
        If genes.Length <> logCpm.Length Then
            Throw New ArgumentException($"基因名数量 {genes.Length} 与表达值数量 {logCpm.Length} 不一致", NameOf(logCpm))
        End If

        Dim index As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
        Dim n As Integer = Me.GeneNames.Length
        Dim z As Double() = New Double(n - 1) {}

        For i As Integer = 0 To n - 1
            index(Me.GeneNames(i)) = i
        Next

        ' 数组默认全 0：缺失基因填自身均值 ⇒ 标准化后为 0，等效于"该基因取对照水平"
        Dim hit As Integer = 0

        For k As Integer = 0 To genes.Length - 1
            Dim name As String = genes(k)
            Dim i As Integer

            If String.IsNullOrEmpty(name) Then Continue For
            If Not index.TryGetValue(name, i) Then Continue For

            Dim sd As Double = System.Math.Max(Me.GeneSDs(i), 1.0E-08)

            z(i) = (logCpm(k) - Me.GeneMeans(i)) / sd
            hit += 1
        Next

        If hit = 0 Then
            Throw New ArgumentException("上下文中没有任何基因与建模基因匹配，请检查基因标识空间（模型使用 Ensembl ID）", NameOf(genes))
        End If

        Call engine.SetBaselineWindow(engine.BuildStaticWindow(z))
    End Sub

    ''' <summary>
    ''' 撤销 <see cref="SetContext"/>，把对照基线恢复为训练数据推导出的尾部时间窗。
    ''' </summary>
    Public Sub ResetContext()
        Call engine.SetBaselineWindow(trainingBaseline)
    End Sub

    ' ==================== InsilicoPerturbationExperiment ====================

    ''' <summary>
    ''' 虚拟敲除：把目标基因的表达置零后预测全基因组响应。
    ''' </summary>
    ''' <param name="geneName">目标基因名</param>
    ''' <param name="nSamples">采样数（当前实现为确定性推理，该参数仅为接口兼容保留）</param>
    ''' <returns>干预结果</returns>
    Public Function KnockoutGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult Implements InsilicoPerturbationExperiment.KnockoutGene
        Return Predict(New InterventionSpec() {CreateSpec(geneName, InterventionMode.Knockout)})
    End Function

    ''' <summary>
    ''' 虚拟过表达：把目标基因提升到 <c>均值 + 3σ</c> 后预测全基因组响应。
    ''' </summary>
    ''' <param name="geneName">目标基因名</param>
    ''' <param name="nSamples">采样数（保留参数，见 <see cref="KnockoutGene"/>）</param>
    ''' <returns>干预结果</returns>
    Public Function OverexpressGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult Implements InsilicoPerturbationExperiment.OverexpressGene
        Return Predict(New InterventionSpec() {CreateSpec(geneName, InterventionMode.Overexpression)})
    End Function

    ''' <summary>
    ''' 虚拟敲低：把目标基因下调到 <c>均值 − 2σ</c> 后预测全基因组响应。
    ''' </summary>
    ''' <param name="geneName">目标基因名</param>
    ''' <param name="nSamples">采样数（保留参数，见 <see cref="KnockoutGene"/>）</param>
    ''' <returns>干预结果</returns>
    Public Function KnockDownGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult Implements InsilicoPerturbationExperiment.KnockDownGene
        Return Predict(New InterventionSpec() {CreateSpec(geneName, InterventionMode.Knockdown)})
    End Function

    ''' <summary>
    ''' 预测任意（组合）干预。
    ''' </summary>
    ''' <param name="specs">干预定义集合</param>
    ''' <returns>干预结果</returns>
    Public Function Predict(specs As IEnumerable(Of InterventionSpec)) As InterventionResult
        Return engine.Predict(specs)
    End Function

    ''' <summary>
    ''' 多基因组合扰动：返回联合结果与协同/拮抗分析。
    ''' </summary>
    ''' <param name="geneNames">目标基因名集合（至少两个）</param>
    ''' <param name="mode">干预模式</param>
    ''' <returns>组合扰动分析结果</returns>
    Public Function PredictCombination(geneNames As IEnumerable(Of String),
                                       Optional mode As InterventionMode = InterventionMode.Knockout) As CombinationResult
        Dim specs As List(Of InterventionSpec) =
            geneNames.SafeQuery.Select(Function(g) CreateSpec(g, mode)).ToList()

        If specs.Count < 2 Then
            Throw New ArgumentException("组合扰动至少需要两个基因", NameOf(geneNames))
        End If

        Return engine.PredictCombination(specs)
    End Function

    ''' <summary>
    ''' 多步递归级联模拟：把扰动效应沿时间向前推进，结果写入
    ''' <see cref="InterventionResult.DynamicTrajectory"/>。
    ''' </summary>
    ''' <param name="geneName">目标基因</param>
    ''' <param name="mode">干预模式</param>
    ''' <param name="steps">推进步数；&lt;= 0 时取配置值</param>
    ''' <returns>首步干预结果，附带完整动态轨迹</returns>
    Public Function SimulateCascade(geneName As String,
                                     Optional mode As InterventionMode = InterventionMode.Knockout,
                                     Optional steps As Integer = 0) As InterventionResult
        Dim spec As InterventionSpec = CreateSpec(geneName, mode)
        Dim count As Integer = If(steps > 0, steps, cfg.CascadeSteps)

        Dim result As InterventionResult = engine.Predict(New InterventionSpec() {spec})

        result.DynamicTrajectory = engine.SimulateCascade(New InterventionSpec() {spec}, count)

        Return result
    End Function

    ''' <summary>
    ''' 构造干预定义（基因索引留待引擎解析，大小写不敏感）。
    ''' </summary>
    ''' <param name="geneName">基因名</param>
    ''' <param name="mode">干预模式</param>
    ''' <returns>干预定义</returns>
    Private Shared Function CreateSpec(geneName As String, mode As InterventionMode) As InterventionSpec
        Return New InterventionSpec With {
            .GeneName = geneName,
            .GeneIndex = -1,
            .Mode = mode
        }
    End Function

    ''' <summary>
    ''' 取时序数据最后 <c>w</c> 个时间点的标准化表达作为对照窗口。
    ''' </summary>
    ''' <returns>对照窗口 <c>[基因, w]</c></returns>
    Private Function BuildBaselineWindow() As Double(,)
        Dim n As Integer = preprocessedExpression.NGene
        Dim w As Integer = windowDataset.WindowSize
        Dim nTime As Integer = windowDataset.NTimePoint
        Dim window As Double(,) = New Double(n - 1, w - 1) {}

        For i As Integer = 0 To n - 1
            For t As Integer = 0 To w - 1
                window(i, t) = windowDataset.Profiles(i, nTime - w + t)
            Next
        Next

        Return window
    End Function

End Class
