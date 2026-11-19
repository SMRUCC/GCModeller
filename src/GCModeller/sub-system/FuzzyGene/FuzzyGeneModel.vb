Imports System.Diagnostics
Imports System.IO
Imports ana = SMRUCC.genomics.Analysis.FuzzyGene.Perturbation.Analysis
Imports SMRUCC.genomics.Analysis.FuzzyGene.Config
Imports SMRUCC.genomics.Analysis.FuzzyGene.Data
Imports SMRUCC.genomics.Analysis.FuzzyGene.Fitting
Imports SMRUCC.genomics.Analysis.FuzzyGene.IO
Imports SMRUCC.genomics.Analysis.FuzzyGene.Perturbation
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.BioSystem
Imports SMRUCC.genomics.BioSystem.Core
Imports SMRUCC.genomics.BioSystem.Intervention

''' <summary>
''' 神经模糊（ANFIS/TSK）基因调控网络模型的门面。
''' </summary>
''' <remarks>
''' <para>
''' 一个实例封装「表达矩阵 + 可选先验网络 → 逐靶基因模糊模型 → 虚拟扰动」的全流程，
''' 并实现 <see cref="InsilicoPerturbationExperiment"/>，因此可以与 BNLearn 的贝叶斯网络实现、
''' GEARS / GPS 的实现互换使用——``bnlearn`` 包里的 ``knockouts`` / ``knockdown`` /
''' ``overexpress`` 可以直接作用在本对象上。
''' </para>
''' <para>
''' 典型用法：
''' <code>
''' Dim model As New FuzzyGeneModel(expression, prior, config)
''' Dim report = model.Train()
''' Dim ko As InterventionResult = model.KnockoutGene("geneA")
''' </code>
''' </para>
''' <para>
''' 规模提示：这是**模块/子网尺度**的建模方法。逐靶基因的规则数随输入维数指数增长，
''' 请通过 <see cref="FuzzyGeneConfig.MaxRegulators"/>（默认 4）与
''' <see cref="FuzzyGeneConfig.MaxGenes"/>（默认 2000）控制规模；
''' 全基因组两两建模既不必要、也不可辨识。
''' </para>
''' </remarks>
Public Class FuzzyGeneModel : Implements InsilicoPerturbationExperiment

    Private _expression As GeneExpressionData
    Private ReadOnly _config As FuzzyGeneConfig

    Private _network As FuzzyGeneNetwork
    Private _engine As AnfisPerturbationEngine
    Private _lastResult As FuzzyGeneTrainingResult
    Private _strict As Boolean = True

    ''' <summary>
    ''' 拟合期构建的 PBN 定性层（集成预测用）。
    ''' </summary>
    ''' <remarks>
    ''' 它不随包文件持久化：从 <c>*.fuzzygene.zip</c> 恢复的模型没有这一层，
    ''' 因此 <see cref="PredictEnsemble"/> 会明确报错而不是悄悄退化成单点结果。
    ''' </remarks>
    Private _pbn As Hybrid.PbnRegulatorPrior

    ''' <summary>上下文状态（原始值 + 对齐结果 + 诊断）。</summary>
    Private m_context As ContextAlignmentState
    ''' <summary>对齐之后的完整基线向量（原始表达量，长度 = 基因数）；未注入时为 Nothing。</summary>
    Private _contextState As Double()
    Private m_populationCache As Double()()

    ''' <summary>
    ''' 上下文状态：默认 <see cref="ContextAlignmentMode.Quantile"/>。
    ''' </summary>
    ''' <remarks>
    ''' 本模型的<strong>基线是原始表达量尺度</strong>，z-score 换算是网络在迭代内部按固定公式
    ''' 完成的（<see cref="FuzzyGeneNetwork.ToZValue"/>）。因此把外部 context 换算成 z 再反算回原始值
    ''' 等于什么都没做，真正需要的是把另一条归一化管线的分布映射到训练矩阵的原始尺度上。
    ''' </remarks>
    Private ReadOnly Property ContextState As ContextAlignmentState
        Get
            If m_context Is Nothing Then
                m_context = New ContextAlignmentState(ContextAlignmentMode.Quantile)
            End If

            Return m_context
        End Get
    End Property

    ''' <summary>
    ''' 上下文覆盖率的下限（0–1，0 表示不检查）。
    ''' </summary>
    ''' <remarks>低于该值时 <see cref="SetContext"/> 直接抛出异常，避免在“其实没设上”的上下文里继续做分析。</remarks>
    Public Property MinContextCoverage As Double = 0.0

    ''' <summary>
    ''' 由表达矩阵与（可选的）先验调控网络构建模型（此时尚未训练）。
    ''' </summary>
    ''' <param name="expression">基因表达矩阵（行 = 基因，列 = 样本）；带 <c>TimePoints</c> 时自动使用滞后路线</param>
    ''' <param name="prior">先验调控网络，用于候选调控因子的优先排序</param>
    ''' <param name="config">建模配置；Nothing 时使用默认值</param>
    Public Sub New(expression As GeneExpressionData,
                   Optional prior As PriorNetwork = Nothing,
                   Optional config As FuzzyGeneConfig = Nothing)

        If expression Is Nothing Then
            Throw New ArgumentNullException(NameOf(expression))
        End If

        _expression = expression
        _prior = prior
        _config = If(config, New FuzzyGeneConfig()).ValidateGene()
    End Sub

    ''' <summary>从持久化的网络包恢复模型（不保留训练现场，可直接推理与扰动）。</summary>
    Private Sub New(network As FuzzyGeneNetwork)
        _network = network
        _config = network.Config
        _engine = New AnfisPerturbationEngine(network) With {.Strict = _strict}
    End Sub

    ''' <summary>训练用的表达矩阵；从包文件恢复时为 Nothing。</summary>
    Public ReadOnly Property Expression As GeneExpressionData
        Get
            Return _expression
        End Get
    End Property

    ''' <summary>先验调控网络（可能为 Nothing）。</summary>
    Public ReadOnly Property Prior As PriorNetwork

    ''' <summary>建模配置（恢复的模型同样携带原始配置）。</summary>
    Public ReadOnly Property Config As FuzzyGeneConfig
        Get
            Return _config
        End Get
    End Property

    ''' <summary>拟合出的调控网络；未训练时为 Nothing。</summary>
    Public ReadOnly Property Network As FuzzyGeneNetwork
        Get
            Return _network
        End Get
    End Property

    ''' <summary>是否已完成训练（或已从包恢复）。</summary>
    Public ReadOnly Property IsTrained As Boolean
        Get
            Return _network IsNot Nothing
        End Get
    End Property

    ''' <summary>最近一次训练的汇总诊断。</summary>
    Public ReadOnly Property LastTrainingResult As FuzzyGeneTrainingResult
        Get
            Return _lastResult
        End Get
    End Property

    ''' <summary>拟合期构建的 PBN 定性层；未启用 PBN 或从包恢复时为 <c>Nothing</c>。</summary>
    Public ReadOnly Property PbnPrior As Hybrid.PbnRegulatorPrior
        Get
            Return _pbn
        End Get
    End Property

    ''' <summary>
    ''' 集成预测：按 PBN 规则概率采样多次，给出逐基因的均值、标准差、置信区间与符号一致率。
    ''' </summary>
    ''' <param name="specs">干预定义（与 <see cref="Predict"/> 相同）</param>
    ''' <param name="nSamples">采样次数；&lt;= 0 时取 <see cref="Config.HybridOptions.EnsembleSamples"/></param>
    ''' <returns>集成结果</returns>
    ''' <remarks>
    ''' <para>
    ''' 这里的 <paramref name="nSamples"/> 正是 <c>InsilicoPerturbationExperiment</c> 里
    ''' 一直被忽略的那个参数：以前它是「仅为接口兼容保留」，现在它真的驱动采样次数。
    ''' </para>
    ''' <para>
    ''' 前提是拟合时启用了 PBN（<see cref="Config.HybridOptions.PbnRegularization"/>）；
    ''' 否则抛 <see cref="InvalidOperationException"/>，而不是返回一个看上去有置信区间、
    ''' 实际上标准差全为 0 的假结果。
    ''' </para>
    ''' </remarks>
    Public Function PredictEnsemble(specs As IEnumerable(Of InterventionSpec),
                                    Optional nSamples As Integer = 0) As Hybrid.EnsemblePerturbationResult
        Call EnsureTrained()

        Dim count As Integer = If(nSamples > 0, nSamples, _config.Hybrid.EnsembleSamples)

        Return _engine.PredictEnsemble(specs, count, _pbn)
    End Function

    ''' <summary>当前注入的上下文（对齐之后的原始表达量，长度 = 基因数）；未注入时为 Nothing。</summary>
    Public ReadOnly Property Context As Double()
        Get
            Return _contextState
        End Get
    End Property

    ''' <summary>是否已注入上下文（注入后对照基线被整体替换）。</summary>
    Public ReadOnly Property IsContextSet As Boolean
        Get
            Return _contextState IsNot Nothing AndAlso ContextState.HasContext
        End Get
    End Property

    ''' <summary>最近一次上下文注入的诊断；未注入时为 Nothing。</summary>
    Public ReadOnly Property ContextDiagnostics As ContextAlignmentDiagnostics
        Get
            Return ContextState.Diagnostics
        End Get
    End Property

    ''' <summary>当前生效的上下文对齐模式。</summary>
    Public ReadOnly Property ContextAlignmentMode As ContextAlignmentMode
        Get
            Return ContextState.Mode
        End Get
    End Property

    ''' <summary>带符号调控边（未训练时为空数组）。</summary>
    Public ReadOnly Property Edges As RegulationEdgeModel()
        Get
            Return If(_network?.Edges, New RegulationEdgeModel() {})
        End Get
    End Property

    ''' <summary>
    ''' 严格模式：目标基因不在模型里时抛 <see cref="MissingMemberException"/>；
    ''' 为 False 时返回 <see cref="InterventionResult.Undefined"/> = True 的野生型降级结果。
    ''' </summary>
    Public Property Strict As Boolean
        Get
            Return _strict
        End Get
        Set(value As Boolean)
            _strict = value

            If _engine IsNot Nothing Then
                _engine.Strict = value
            End If
        End Set
    End Property

    ''' <summary>执行拟合。</summary>
    ''' <returns>训练诊断（网络对象通过 <see cref="Network"/> 获取）</returns>
    Public Function Train() As FuzzyGeneTrainingResult
        If _expression Is Nothing Then
            Throw New InvalidOperationException(
                "this model was restored from a package and carries no expression matrix; construct a new FuzzyGeneModel to retrain")
        End If

        Dim output As FuzzyGeneFitOutput = GeneNetworkFitter.Fit(_expression, _prior, _config)

        _network = output.Network
        _lastResult = output.Report
        _pbn = output.PbnPrior
        _engine = New AnfisPerturbationEngine(_network) With {.Strict = _strict}
        ' 重新训练后扰动引擎换了实例，之前注入的上下文不再适用
        Call ContextState.Clear()

        _contextState = Nothing
        m_populationCache = Nothing

        Return _lastResult
    End Function

    ''' <summary>导出可解释内容（规则文本 + SIF/CSV 边表 + 摘要）到目录。</summary>
    ''' <param name="dir">输出目录</param>
    ''' <returns>写入的文件路径</returns>
    Public Function Export(dir As String) As String()
        Call EnsureTrained()

        Return NetworkExporter.Export(_network, dir)
    End Function

    ''' <summary>全部 IF-THEN 规则文本。</summary>
    Public Function Rules() As String()
        Call EnsureTrained()

        Return _network.RulesText()
    End Function

    ''' <summary>
    ''' 导出 <c>基因 × 调控因子</c> 的带符号调控权重矩阵。
    ''' </summary>
    ''' <param name="layout">行列布局；默认严格 <c>N×N</c></param>
    ''' <param name="minWeight">绝对值低于该值的边按 0 处理；0 保留全部</param>
    ''' <param name="genes">只导出这些靶基因（行），<c>Nothing</c> 表示全部</param>
    ''' <returns>
    ''' 标准 <see cref="Matrix"/>：行 = 靶基因（<c>geneID</c>），列 = 调控因子（<c>sampleID</c>），
    ''' 元素 = 调控因子对靶基因的带符号影响力（正激活、负抑制）
    ''' </returns>
    ''' <remarks>
    ''' <para>
    ''' 矩阵可以直接用 <c>geneExpression::write.expr_matrix()</c> 写成 csv，
    ''' 也可以作为后续网络可视化（如 WGCNA / igraph）的输入。
    ''' </para>
    ''' <para>
    ''' 规模提醒：全基因组模型用 <see cref="RegulationMatrixLayout.Square"/> 会得到
    ''' <c>N²</c> 的稠密矩阵（1.85 万基因约 2.7 GB），这时应改用
    ''' <see cref="RegulationMatrixLayout.RegulatorsOnly"/>（只保留真正的调控因子列）
    ''' 或 <see cref="RegulationMatrixLayout.Compact"/>，并用 <paramref name="genes"/> 限定行范围。
    ''' </para>
    ''' </remarks>
    Public Function RegulationMatrix(Optional layout As RegulationMatrixLayout = RegulationMatrixLayout.Square,
                                     Optional minWeight As Double = 0.0,
                                     Optional genes As String() = Nothing) As Matrix
        Call EnsureTrained()

        Return RegulationMatrixExporter.ToMatrix(_network, layout, minWeight, genes)
    End Function

    ''' <summary>
    ''' 导出与 <see cref="RegulationMatrix"/> 同形状的**置信度**矩阵。
    ''' </summary>
    ''' <param name="layout">行列布局；默认严格 <c>N×N</c></param>
    ''' <param name="minConfidence">低于该置信度的边按 0 处理；0 保留全部</param>
    ''' <param name="genes">只导出这些靶基因（行），<c>Nothing</c> 表示全部</param>
    ''' <returns>
    ''' 标准 <see cref="Matrix"/>：行 = 靶基因、列 = 调控因子、元素 = 该调控关系的置信度（非负）
    ''' </returns>
    ''' <remarks>
    ''' 置信度在边被先验网络支持时取先验置信度，否则取 <c>|Pearson|</c>。
    ''' 它与权重矩阵逐元素对应（同样的行列顺序），可以一起导出：
    ''' 权重决定网络里边的方向与粗细，置信度决定边的可信度（阈值/透明度）。
    ''' </remarks>
    Public Function ConfidenceMatrix(Optional layout As RegulationMatrixLayout = RegulationMatrixLayout.Square,
                                     Optional minConfidence As Double = 0.0,
                                     Optional genes As String() = Nothing) As Matrix
        Call EnsureTrained()

        Return RegulationMatrixExporter.ToConfidenceMatrix(_network, layout, minConfidence, genes)
    End Function

    ''' <summary>把外部样本背景注入为扰动分析的对照基线（未给出的基因保持训练基线）。</summary>
    ''' <param name="context">基因名 → 原始表达量（大小写不敏感，可只给出部分基因）</param>
    ''' <returns>当前对象（便于链式调用）</returns>
    ''' <remarks>
    ''' <para>
    ''' 注入之后，对照（野生型）态与突变体态都**从该初始条件出发**各自迭代到稳态，
    ''' 因此 <see cref="InterventionResult.FoldChanges"/> 的含义变成「同一 context 下的突变体 vs 野生型」。
    ''' </para>
    ''' <para>
    ''' 对齐由公共组件完成，默认 <see cref="ContextAlignmentMode.Quantile"/>；
    ''' 覆盖率下限改由 <see cref="MinContextCoverage"/> 控制，注入结果见 <see cref="ContextDiagnostics"/>。
    ''' </para>
    ''' </remarks>
    ''' <exception cref="ArgumentException">没有任何基因命中模型，或覆盖率低于 <see cref="MinContextCoverage"/></exception>
    Public Function SetContext(context As Dictionary(Of String, Double)) As InsilicoPerturbationExperiment Implements InsilicoPerturbationExperiment.SetContext
        Call EnsureTrained()

        Dim aligned As ContextAlignmentResult = ContextState.SetContext(context, buildContextReference())

        If aligned.Diagnostics.Matched = 0 Then
            Throw New ArgumentException("none of the given genes is present in the model", NameOf(context))
        End If
        If MinContextCoverage > 0.0 AndAlso aligned.Diagnostics.Coverage < MinContextCoverage Then
            Throw New ArgumentException(
                $"the context covers only {aligned.Diagnostics.Coverage:P1} of the model genes " &
                $"({aligned.Diagnostics.Matched}/{_network.NGene}), " &
                $"below the required minimum of {MinContextCoverage:P1}", NameOf(context))
        End If

        Call applyContextBaseline(aligned)

        Return Me
    End Function

    ''' <summary>切换上下文对齐模式；已注入上下文时会立即用同一份原始上下文重新对齐。</summary>
    ''' <param name="mode">新的对齐模式</param>
    ''' <returns>当前对象（便于链式调用）</returns>
    Public Function SetContextAlignmentMode(mode As ContextAlignmentMode) As InsilicoPerturbationExperiment Implements InsilicoPerturbationExperiment.SetContextAlignmentMode
        Call EnsureTrained()
        Call ContextState.SetMode(mode)

        If ContextState.HasContext Then
            Call applyContextBaseline(ContextState.Result)
        End If

        Return Me
    End Function

    ''' <summary>把对齐结果写进扰动引擎的对照基线（未覆盖的基因沿用训练基线）。</summary>
    Private Sub applyContextBaseline(aligned As ContextAlignmentResult)
        Dim state As Double() = CType(_network.BaselineRaw.Clone(), Double())

        For Each item As KeyValuePair(Of String, Double) In aligned.Values
            Dim idx As Integer = _network.IndexOfGene(item.Key)

            If idx >= 0 Then
                state(idx) = item.Value
            End If
        Next

        _engine.SetBaseline(state)
        _contextState = state
    End Sub

    ''' <summary>构造公共对齐组件所需的训练侧统计量（原始表达量尺度）。</summary>
    Private Function buildContextReference() As ContextAlignmentReference
        If _expression Is Nothing Then
            _expression = GeneExpressionData.FromMeanExpression(_network.GeneNames, _network.Means)
        End If

        Return New ContextAlignmentReference With {
            .GeneNames = _network.GeneNames,
            .Means = _network.Means,
            .StandardDeviations = _network.SDs,
            .Population = trainingPopulation()
        }
    End Function

    ''' <summary>训练矩阵的逐基因种群 <c>population(gene)(sample)</c>（惰性构建并缓存）。</summary>
    Private Function trainingPopulation() As Double()()
        If m_populationCache Is Nothing Then
            Dim n As Integer = _network.NGene
            Dim rows(n - 1)() As Double

            For i As Integer = 0 To n - 1
                Dim row As Integer = _expression.GetGeneIndex(_network.GeneNames(i))

                If row < 0 OrElse _expression.NSample <= 0 Then
                    rows(i) = New Double() {}
                Else
                    Dim values(_expression.NSample - 1) As Double

                    For j As Integer = 0 To values.Length - 1
                        values(j) = _expression.Matrix(row, j)
                    Next

                    rows(i) = values
                End If
            Next

            m_populationCache = rows
        End If

        Return m_populationCache
    End Function

    ''' <summary>
    ''' 把**上游扰动实验的结果**注入为对照基线，实现迭代扰动
    ''' （本轮突变体稳态 → 下一轮的基线）。
    ''' </summary>
    ''' <param name="upstream">上游扰动结果，例如 <c>KnockoutGene("A")</c></param>
    ''' <returns>覆盖率诊断</returns>
    ''' <remarks>
    ''' <para>
    ''' 取的是上游的 <see cref="InterventionResult.MutantMeans"/>（扰动之后的表达谱）。
    ''' 注入之后，下一轮扰动的 <c>foldchange</c> 含义变成
    ''' 「在 A 已经失活的前提下再敲 B」，而不是「从原始基线出发敲 B」。
    ''' </para>
    ''' <para>
    ''' 想回到上游的**对照态**（等价于撤销这一轮），改用
    ''' <c>LoadContext(ContextTable.FromUpstreamIntervention(result, useMutant:=False), 0)</c>。
    ''' </para>
    ''' <para>
    ''' 注意上下文是**初始条件**而不是终态：野生型/突变体都会从它出发各自迭代到稳态，
    ''' 因此报告的 <c>WildtypeMeans</c> 是该 context 的不动点近似，不一定逐位等于注入值。
    ''' </para>
    ''' </remarks>
    Public Function LoadContext(upstream As InterventionResult, Optional align As ContextAlignmentMode = ContextAlignmentMode.None) As ContextAlignmentDiagnostics
        Call EnsureTrained()

        ' 上游结果是同一个模型迭代出来的，已经处在模型空间：不能再按外部管线的口径二次对齐
        Return loadTable(ContextTable.FromUpstreamIntervention(upstream), align)
    End Function

    ''' <summary>
    ''' 把任意 <see cref="ContextTable"/> 注入为对照基线（多列时会自动选命中模型基因最多的一列）。
    ''' </summary>
    ''' <param name="table">上下文表（来自文件，或 <see cref="ContextTable.FromUpstreamIntervention"/>）</param>
    ''' <returns>对齐诊断（覆盖率、缺失基因、实际使用的基因列等）</returns>
    ''' <remarks>使用当前生效的对齐模式（默认 <see cref="ContextAlignmentMode.Quantile"/>）。</remarks>
    Public Function LoadContext(table As ContextTable) As ContextAlignmentDiagnostics
        Return loadTable(table, ContextState.Mode)
    End Function

    ''' <summary>按指定模式把上下文表注入为对照基线。</summary>
    Private Function loadTable(table As ContextTable, mode As ContextAlignmentMode) As ContextAlignmentDiagnostics
        Call EnsureTrained()

        Dim best As Integer = 0
        Dim bestHits As Integer = -1

        For c As Integer = 0 To table.Columns.Length - 1
            Dim hits As Integer = CountContextMatches(table.Columns(c))

            If hits > bestHits Then
                bestHits = hits
                best = c
            End If
        Next

        Dim context As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

        For i As Integer = 0 To table.Columns(best).Length - 1
            Dim name As String = table.Columns(best)(i)

            If String.IsNullOrEmpty(name) Then
                Continue For
            End If

            context(name) = table.Values(i)
        Next

        Dim aligned As ContextAlignmentResult =
            ContextState.SetContextWith(context, buildContextReference(), mode)

        If aligned.Diagnostics.Matched = 0 Then
            Throw New ArgumentException("none of the given genes is present in the model", NameOf(table))
        End If
        If MinContextCoverage > 0.0 AndAlso aligned.Diagnostics.Coverage < MinContextCoverage Then
            Throw New ArgumentException(
                $"the context covers only {aligned.Diagnostics.Coverage:P1} of the model genes " &
                $"({aligned.Diagnostics.Matched}/{_network.NGene}), " &
                $"below the required minimum of {MinContextCoverage:P1}", NameOf(table))
        End If

        Call applyContextBaseline(aligned)

        Dim diagnosis As ContextAlignmentDiagnostics = aligned.Diagnostics

        diagnosis.InvalidValues += table.Invalid
        diagnosis.AddNote($"gene column: {table.Headers(best)}")

        Return diagnosis
    End Function

    ''' <summary>
    ''' 从表格文件（csv / tsv）读取转录组背景并注入为对照基线。
    ''' </summary>
    ''' <param name="path">
    ''' 表格路径，例如 pseudobulk 的 <c>gene_name,gene_id,mean_counts</c>
    ''' </param>
    ''' <param name="valueColumn">数值列的列名；为空时取最后一列</param>
    ''' <returns>对齐诊断</returns>
    ''' <remarks>
    ''' 表里通常同时有基因符号与数据库 ID 两列，这里会逐个候选列去比对模型基因，
    ''' 选命中数最多的那一列使用（例如模型是按 Ensembl ID 建的，就会自动改用 <c>gene_id</c> 列）。
    ''' </remarks>
    Public Function LoadContext(path As String,
                                Optional valueColumn As String = Nothing) As ContextAlignmentDiagnostics
        Call EnsureTrained()
        Return LoadContext(GeneContextReader.ReadTable(path, valueColumn))
    End Function

    Private Function CountContextMatches(names As String()) As Integer
        Dim hits As Integer = 0

        For k As Integer = 0 To names.Length - 1
            If _network.IndexOfGene(names(k)) >= 0 Then
                hits += 1
            End If
        Next

        Return hits
    End Function

    ''' <summary>撤销 <see cref="SetContext"/>，恢复训练数据推导出的对照基线。</summary>
    Public Sub ResetContext()
        Call EnsureTrained()

        _engine.ResetBaseline()
        Call ContextState.Clear()

        _contextState = Nothing
    End Sub

    ' ==================== InsilicoPerturbationExperiment ====================

    ''' <summary>虚拟敲除：把目标基因置零后预测全基因组响应。</summary>
    ''' <param name="geneName">目标基因名</param>
    ''' <param name="nSamples">采样数（本实现为确定性稳态推理，该参数仅为接口兼容保留）</param>
    ''' <returns>干预结果</returns>
    Public Function KnockoutGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult Implements InsilicoPerturbationExperiment.KnockoutGene
        Return PredictOne(geneName, InterventionMode.Knockout)
    End Function

    ''' <summary>虚拟过表达：把目标基因提升到 <c>基线 + nσ</c> 后预测全基因组响应。</summary>
    ''' <param name="geneName">目标基因名</param>
    ''' <param name="nSamples">采样数（保留参数，见 <see cref="KnockoutGene"/>）</param>
    ''' <returns>干预结果</returns>
    Public Function OverexpressGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult Implements InsilicoPerturbationExperiment.OverexpressGene
        Return PredictOne(geneName, InterventionMode.Overexpression)
    End Function

    ''' <summary>虚拟敲低：把目标基因压到基线的 <see cref="FuzzyGeneConfig.KnockdownResidual"/> 比例。</summary>
    ''' <param name="geneName">目标基因名</param>
    ''' <param name="nSamples">采样数（保留参数，见 <see cref="KnockoutGene"/>）</param>
    ''' <returns>干预结果</returns>
    Public Function KnockDownGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult Implements InsilicoPerturbationExperiment.KnockDownGene
        Return PredictOne(geneName, InterventionMode.Knockdown)
    End Function

    ''' <summary>预测任意（可组合的）干预。</summary>
    ''' <param name="specs">干预定义集合</param>
    ''' <returns>干预结果</returns>
    Public Function Predict(specs As IEnumerable(Of InterventionSpec)) As InterventionResult
        Call EnsureTrained()

        Return _engine.Predict(specs)
    End Function

    ''' <summary>多基因组合扰动（含协同/拮抗分析）。</summary>
    ''' <param name="geneNames">目标基因名集合（至少两个）</param>
    ''' <param name="mode">干预模式</param>
    ''' <returns>组合扰动结果</returns>
    Public Function PredictCombination(geneNames As IEnumerable(Of String), Optional mode As InterventionMode = InterventionMode.Knockout) As CombinationPerturbationResult
        Call EnsureTrained()

        Dim specs As New List(Of InterventionSpec)

        For Each gene As String In geneNames
            Call specs.Add(New InterventionSpec With {.GeneName = gene, .GeneIndex = -1, .Mode = mode})
        Next

        If specs.Count < 2 Then
            Throw New ArgumentException("the combination analysis requires at least two genes", NameOf(geneNames))
        End If

        Return _engine.PredictCombination(specs)
    End Function

    ''' <summary>多步级联模拟，结果写入 <see cref="InterventionResult.DynamicTrajectory"/>。</summary>
    ''' <param name="geneName">目标基因</param>
    ''' <param name="mode">干预模式</param>
    ''' <param name="steps">推进步数；&lt;= 0 时取配置值</param>
    ''' <returns>首步干预结果，附带完整轨迹</returns>
    Public Function SimulateCascade(geneName As String,
                                    Optional mode As InterventionMode = InterventionMode.Knockout,
                                    Optional steps As Integer = 0) As InterventionResult
        Call EnsureTrained()

        Dim spec As New InterventionSpec With {.GeneName = geneName, .GeneIndex = -1, .Mode = mode}
        Dim count As Integer = If(steps > 0, steps, _config.PropagationSteps)

        Return _engine.SimulateCascade(spec, count)
    End Function

    ' ==================== 轨迹与候选排序 ====================

    ''' <summary>
    ''' 单次虚拟扰动的完整稳态求解轨迹（突变型 + 野生型对照）。
    ''' </summary>
    ''' <param name="geneName">目标基因</param>
    ''' <param name="mode">干预模式</param>
    ''' <param name="steps">最大迭代步数；&lt;= 0 时取配置值</param>
    ''' <param name="relaxation">阻尼系数；<c>NaN</c> 时取配置值（ω 扫描靠它按次覆盖）</param>
    ''' <param name="value">
    ''' <see cref="InterventionMode.Custom"/> 模式下的钳制值（原始表达量）；
    ''' 其他模式忽略。剂量扫描用它把 <c>baseline × (1 − dose)</c> 传进来。
    ''' </param>
    ''' <returns>干预结果与两条轨迹</returns>
    ''' <remarks>
    ''' 与 <see cref="SimulateCascade"/> 的区别：级联是「无阻尼地反复应用 F」，
    ''' 不判收敛、步数固定；本方法是带阻尼的稳态求解轨迹，帧号是求解迭代步，
    ''' 并且带 <see cref="Data.SteadyStateTrace.Converged"/> 诊断。
    ''' </remarks>
    Public Function TracePerturbation(geneName As String,
                                      Optional mode As InterventionMode = InterventionMode.Knockout,
                                      Optional steps As Integer = 0,
                                      Optional relaxation As Double = Double.NaN,
                                      Optional value As Double = 0.0) As PerturbationTrace
        Call EnsureTrained()

        Dim spec As New InterventionSpec With {
            .GeneName = geneName,
            .GeneIndex = -1,
            .Mode = mode,
            .Value = value
        }

        Return _engine.PredictWithTrace(New InterventionSpec() {spec}, steps, relaxation)
    End Function

    ''' <summary>
    ''' 组合扰动（多个干预定义）的稳态求解轨迹。
    ''' </summary>
    ''' <param name="specs">干预定义集合；多个元素表示联合扰动</param>
    ''' <param name="steps">最大迭代步数；&lt;= 0 时取配置值</param>
    ''' <param name="relaxation">阻尼系数；<c>NaN</c> 时取配置值</param>
    ''' <returns>联合结果与两条轨迹</returns>
    ''' <remarks>
    ''' 与单基因重载并列存在：第一个参数是基因名时走单扰动，是干预定义集合时走组合。
    ''' 组合非加性分析（<c>Perturbation.Analysis.CombinationTrace</c>）依赖这个入口。
    ''' </remarks>
    Public Function TracePerturbation(specs As IEnumerable(Of InterventionSpec),
                                      Optional steps As Integer = 0,
                                      Optional relaxation As Double = Double.NaN) As PerturbationTrace
        Call EnsureTrained()

        If specs Is Nothing Then
            Throw New ArgumentNullException(NameOf(specs))
        End If

        Return _engine.PredictWithTrace(specs, steps, relaxation)
    End Function

    ''' <summary>
    ''' 在有限候选池上评估扰动响应强度，按「未被扰动基因的最大 |Z|」降序返回。
    ''' </summary>
    ''' <param name="candidates">候选基因数量；&lt;= 0 表示对全部有模型的基因排序</param>
    ''' <param name="mode">干预模式</param>
    ''' <returns>按响应强度降序排列的候选</returns>
    ''' <remarks>
    ''' <para>
    ''' 全基因组逐个扰动在真实规模（约 1.8 万基因）下需要数小时，所以默认只在
    ''' <paramref name="candidates"/> 个等距抽样的有模型基因上评估。
    ''' 抽样不带随机数，同一模型与同一候选数总是给出同一批基因，便于复现。
    ''' </para>
    ''' <para>
    ''' 评分刻意排除被扰动基因自身：它的 Z 分数是「施加值相对基线的偏离」，
    ''' 不是模型推断出来的响应，算进去会让排名退化成「谁的方差最小」。
    ''' </para>
    ''' </remarks>
    Public Function RankPerturbations(Optional candidates As Integer = 24,
                                      Optional mode As InterventionMode = InterventionMode.Knockout) As PerturbationCandidate()
        Call EnsureTrained()

        Dim ranked As New List(Of PerturbationCandidate)

        For Each gene As String In SelectCandidates(candidates)
            Dim watch As Stopwatch = Stopwatch.StartNew()
            Dim result As InterventionResult

            Try
                result = PredictOne(gene, mode)
            Catch ex As MissingMemberException
                ' 非严格模式下不会走到这里；严格模式也不应该因为单个候选缺失就中断整轮排序
                Continue For
            End Try

            watch.Stop()

            If result Is Nothing OrElse result.Undefined OrElse result.ZScores Is Nothing Then
                Continue For
            End If

            Call ranked.Add(New PerturbationCandidate With {
                .GeneName = gene,
                .MaxAbsZScore = MaxAbsZScoreExcept(result.ZScores, _network.IndexOfGene(gene)),
                .NAffected = result.NAffected,
                .ElapsedMs = watch.Elapsed.TotalMilliseconds
            })
        Next

        Return ranked _
            .OrderByDescending(Function(c) c.MaxAbsZScore) _
            .ThenByDescending(Function(c) c.NAffected) _
            .ToArray()
    End Function

    ''' <summary>
    ''' 选出响应最强的扰动靶基因。
    ''' </summary>
    ''' <param name="candidates">候选基因数量</param>
    ''' <param name="mode">干预模式</param>
    ''' <returns>最强响应的基因名；没有任何可用候选时返回 <c>Nothing</c></returns>
    Public Function FindStrongestPerturbation(Optional candidates As Integer = 24,
                                              Optional mode As InterventionMode = InterventionMode.Knockout) As String
        Dim ranked As PerturbationCandidate() = RankPerturbations(candidates, mode)

        If ranked.Length = 0 Then
            Return Nothing
        End If

        Return ranked(0).GeneName
    End Function

    ''' <summary>
    ''' 等距抽样「有调控因子」的基因；数量不足或要求全量时直接全给。
    ''' </summary>
    Private Function SelectCandidates(count As Integer) As String()
        Dim modelled As New List(Of String)

        For i As Integer = 0 To _network.NGene - 1
            Dim model As GeneRegulationModel = _network.Models(i)

            If model IsNot Nothing AndAlso model.Regulators IsNot Nothing AndAlso model.Regulators.Length > 0 Then
                Call modelled.Add(_network.GeneNames(i))
            End If
        Next

        If count <= 0 OrElse modelled.Count <= count Then
            Return modelled.ToArray()
        End If

        ' 不能叫 step —— 那是 VB 的关键字（For ... Step）
        Dim stride As Double = modelled.Count / count
        Dim picked(count - 1) As String

        For k As Integer = 0 To count - 1
            picked(k) = modelled(CInt(System.Math.Floor(k * stride)))
        Next

        Return picked
    End Function

    ''' <summary>除 <paramref name="skip"/> 之外的最大绝对 Z 分数。</summary>
    Private Shared Function MaxAbsZScoreExcept(values As Double(), skip As Integer) As Double
        Dim max As Double = 0.0

        For i As Integer = 0 To values.Length - 1
            If i = skip Then
                Continue For
            End If

            Dim z As Double = System.Math.Abs(values(i))

            If z > max Then
                max = z
            End If
        Next

        Return max
    End Function

    ' ==================== 多运行自检与剂量分析 ====================

    ''' <summary>在若干阻尼系数上各求解一次，检查终态是否一致。</summary>
    ''' <param name="geneName">被扰动的靶基因</param>
    ''' <param name="mode">干预模式</param>
    ''' <param name="omegas">被扫描的 ω；<c>Nothing</c> 时取 {0.2, 0.5, 0.8, 1.0}</param>
    ''' <param name="steps">每次求解的最大迭代步数</param>
    ''' <param name="referenceOmega">比较基准；<c>NaN</c> 时取最接近 0.5 的那个</param>
    ''' <param name="tolerance">判定「不变」的相对偏差阈值</param>
    ''' <returns>扫描结果</returns>
    Public Function SweepOmega(geneName As String,
                               Optional mode As InterventionMode = InterventionMode.Knockout,
                               Optional omegas As Double() = Nothing,
                               Optional steps As Integer = 50,
                               Optional referenceOmega As Double = Double.NaN,
                               Optional tolerance As Double = 0.001) As ana.OmegaSweepResult
        Call EnsureTrained()

        Return ana.SolverStability.SweepOmega(Me, geneName, mode, omegas, steps, referenceOmega, tolerance)
    End Function

    ''' <summary>在若干迭代步数上各求解一次，检查终态是否随步数改变。</summary>
    ''' <param name="geneName">被扰动的靶基因</param>
    ''' <param name="mode">干预模式</param>
    ''' <param name="stepsList">被扫描的步数；<c>Nothing</c> 时取 {50, 200, 500}</param>
    ''' <param name="omega">本次使用的 ω；<c>NaN</c> 取配置值</param>
    ''' <param name="tolerance">判定「不变」的相对偏差阈值</param>
    ''' <returns>扫描结果</returns>
    Public Function SweepSteps(geneName As String,
                               Optional mode As InterventionMode = InterventionMode.Knockout,
                               Optional stepsList As Integer() = Nothing,
                               Optional omega As Double = Double.NaN,
                               Optional tolerance As Double = 0.001) As ana.StepSweepResult
        Call EnsureTrained()

        Return ana.SolverStability.SweepSteps(Me, geneName, mode, stepsList, omega, tolerance)
    End Function

    ''' <summary>跨 ω 比较 lag（首次响应帧号）排序的稳健性。</summary>
    ''' <param name="geneName">被扰动的靶基因（自身不参与 lag 统计）</param>
    ''' <param name="mode">干预模式</param>
    ''' <param name="omegas">被扫描的 ω；<c>Nothing</c> 时取默认网格</param>
    ''' <param name="steps">每次求解的最大迭代步数</param>
    ''' <param name="thresholdRatio">触发阈值占该基因终态变化量的比例</param>
    ''' <returns>lag 矩阵与 Spearman 矩阵</returns>
    Public Function AnalyzeLag(geneName As String,
                               Optional mode As InterventionMode = InterventionMode.Knockout,
                               Optional omegas As Double() = Nothing,
                               Optional steps As Integer = 50,
                               Optional thresholdRatio As Double = 0.1) As ana.LagRobustnessResult
        Call EnsureTrained()

        Return ana.SolverStability.LagRobustness(Me, geneName, mode, omegas, steps, thresholdRatio)
    End Function

    ''' <summary>对若干剂量点各求解一次，得到全基因的终态剂量响应。</summary>
    ''' <param name="geneName">被扰动的靶基因</param>
    ''' <param name="doses">剂量点（0 = 不改变、1 = 完全敲除）；<c>Nothing</c> 时取 5 点网格</param>
    ''' <param name="steps">每次求解的最大迭代步数</param>
    ''' <param name="omega">本次使用的 ω；<c>NaN</c> 取配置值</param>
    ''' <param name="top">保留的 Top-K 基因数</param>
    ''' <returns>剂量-响应结果</returns>
    Public Function AnalyzeDose(geneName As String,
                                Optional doses As Double() = Nothing,
                                Optional steps As Integer = 60,
                                Optional omega As Double = Double.NaN,
                                Optional top As Integer = 50) As ana.DoseResponseResult
        Call EnsureTrained()

        Return ana.DoseScan.DoseResponse(Me, geneName, doses, steps, omega, top)
    End Function

    ''' <summary>剂量 × 迭代步的二维响应曲面（只保留 Top-K 基因）。</summary>
    ''' <param name="geneName">被扰动的靶基因</param>
    ''' <param name="doses">剂量点；<c>Nothing</c> 时取 5 点网格</param>
    ''' <param name="steps">每次求解的最大迭代步数</param>
    ''' <param name="omega">本次使用的 ω；<c>NaN</c> 取配置值</param>
    ''' <param name="top">保留的基因数</param>
    ''' <returns>曲面结果</returns>
    Public Function AnalyzeDoseTime(geneName As String,
                                    Optional doses As Double() = Nothing,
                                    Optional steps As Integer = 60,
                                    Optional omega As Double = Double.NaN,
                                    Optional top As Integer = 20) As ana.DoseTimeSurfaceResult
        Call EnsureTrained()

        Return ana.DoseScan.DoseTimeSurface(Me, geneName, doses, steps, omega, top)
    End Function

    ''' <summary>两个基因联合扰动的非加性分析（终态 + 逐帧）。</summary>
    ''' <param name="geneA">第一个靶基因</param>
    ''' <param name="geneB">第二个靶基因</param>
    ''' <param name="mode">干预模式</param>
    ''' <param name="steps">每次求解的最大迭代步数</param>
    ''' <param name="omega">本次使用的 ω；<c>NaN</c> 取配置值</param>
    ''' <param name="top">逐帧口径保留的基因数</param>
    ''' <returns>非加性结果</returns>
    Public Function AnalyzeCombination(geneA As String,
                                       geneB As String,
                                       Optional mode As InterventionMode = InterventionMode.Knockout,
                                       Optional steps As Integer = 60,
                                       Optional omega As Double = Double.NaN,
                                       Optional top As Integer = 50) As ana.CombinationTraceResult
        Call EnsureTrained()

        Return ana.CombinationTrace.Analyze(Me, geneA, geneB, mode, steps, omega, top)
    End Function

    ''' <summary>
    ''' 一次跑完 ω / 步数 / lag / 剂量 / 组合五项分析，并把结果写成 CSV 与 JSON。
    ''' </summary>
    ''' <param name="dir">输出目录（不存在时自动创建）</param>
    ''' <param name="gene">单扰动分析的靶基因</param>
    ''' <param name="geneA">组合分析的第一个基因；为空时跳过组合部分</param>
    ''' <param name="geneB">组合分析的第二个基因；为空时跳过组合部分</param>
    ''' <param name="options">网格与开关；<c>Nothing</c> 时用默认值</param>
    ''' <returns>实际写出的文件路径</returns>
    ''' <remarks>
    ''' 每一步求解都会同时产出突变型与野生型轨迹，因此一次编排的总求解次数约为
    ''' <c>2 × (ω 个数 + 步数个数 + ω 个数 + 剂量点个数 × 2 + 3)</c>。
    ''' 在 18533 基因的真实模型上，默认网格约需 1~2 分钟。
    ''' </remarks>
    Public Function ExportAnalysisReport(dir As String,
                                         gene As String,
                                         geneA As String,
                                         geneB As String,
                                         Optional options As ana.AnalysisOptions = Nothing) As String()
        Call EnsureTrained()

        Return ana.AnalysisExporter.ExportAll(Me, gene, geneA, geneB, dir, options)
    End Function

    ' ==================== 持久化 ====================

    ''' <summary>把模型保存为 <c>*.fuzzygene.zip</c>。</summary>
    ''' <param name="path">输出路径</param>
    Public Sub Save(path As String)
        Call EnsureTrained()

        Call FuzzyGeneFile.Save(_network, path)
    End Sub

    ''' <summary>把模型写入流（流由调用方负责关闭）。</summary>
    ''' <param name="stream">输出流</param>
    Public Sub Save(stream As Stream)
        Call EnsureTrained()

        Call FuzzyGeneFile.Save(_network, stream)
    End Sub

    ''' <summary>从 <c>*.fuzzygene.zip</c> 恢复模型。</summary>
    ''' <param name="path">包路径</param>
    ''' <returns>可直接推理与扰动的模型对象</returns>
    Public Shared Function Load(path As String) As FuzzyGeneModel
        Return New FuzzyGeneModel(FuzzyGeneFile.Load(path))
    End Function

    ''' <summary>从流恢复模型（流由调用方负责关闭）。</summary>
    ''' <param name="stream">输入流</param>
    ''' <returns>可直接推理与扰动的模型对象</returns>
    Public Shared Function Load(stream As Stream) As FuzzyGeneModel
        Return New FuzzyGeneModel(FuzzyGeneFile.Load(stream))
    End Function

    ' ==================== 内部方法 ====================

    Private Sub EnsureTrained()
        If _network Is Nothing OrElse _engine Is Nothing Then
            Throw New InvalidOperationException("the model has not been trained yet, call Train() first")
        End If
    End Sub

    Private Function PredictOne(geneName As String, mode As InterventionMode) As InterventionResult
        Call EnsureTrained()

        Dim spec As New InterventionSpec With {
            .GeneName = geneName,
            .GeneIndex = -1,
            .Mode = mode
        }

        Return _engine.Predict(New InterventionSpec() {spec})
    End Function

    ''' <summary>单行摘要。</summary>
    Public Overrides Function ToString() As String
        If _network Is Nothing Then
            Return $"FuzzyGene model (untrained, {If(_expression Is Nothing, 0, _expression.NGene)} genes)"
        End If

        Return _network.ToString()
    End Function

End Class
