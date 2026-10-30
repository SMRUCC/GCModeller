Imports System.IO
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

    Private ReadOnly _expression As GeneExpressionData
    Private ReadOnly _config As FuzzyGeneConfig

    Private _network As FuzzyGeneNetwork
    Private _engine As AnfisPerturbationEngine
    Private _lastResult As FuzzyGeneTrainingResult
    Private _strict As Boolean = True

    ''' <summary>当前注入的上下文（原始表达量，长度 = 基因数）；未注入时为 Nothing。</summary>
    Private _contextState As Double()
    ''' <summary>最近一次上下文注入的诊断。</summary>
    Private _contextCoverage As ContextCoverage

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

    ''' <summary>当前注入的上下文（原始表达量，长度 = 基因数）；未注入时为 Nothing。</summary>
    Public ReadOnly Property Context As Double()
        Get
            Return _contextState
        End Get
    End Property

    ''' <summary>是否已注入上下文（注入后对照基线被整体替换）。</summary>
    Public ReadOnly Property IsContextSet As Boolean
        Get
            Return _contextState IsNot Nothing
        End Get
    End Property

    ''' <summary>最近一次上下文注入的覆盖率诊断；未注入时为 Nothing。</summary>
    Public ReadOnly Property LastContextCoverage As ContextCoverage
        Get
            Return _contextCoverage
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
        _engine = New AnfisPerturbationEngine(_network) With {.Strict = _strict}
        ' 重新训练后扰动引擎换了实例，之前注入的上下文不再适用
        _contextState = Nothing
        _contextCoverage = Nothing

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
    ''' <param name="genes">基因名（大小写不敏感，可只给出部分基因）</param>
    ''' <param name="values">与 <paramref name="genes"/> 一一对应的原始表达量</param>
    ''' <param name="minCoverage">
    ''' 要求的覆盖率下限（0–1，0 表示不检查）：低于该值时直接抛异常，
    ''' 避免在"其实没设上"的上下文里继续做分析
    ''' </param>
    ''' <returns>覆盖率诊断（命中数、缺失基因、越界取值等）</returns>
    ''' <remarks>
    ''' 注入之后，对照（野生型）态与突变体态都**从该初始条件出发**各自迭代到稳态，
    ''' 因此 <see cref="InterventionResult.FoldChanges"/>
    ''' 的含义变成「同一 context 下的突变体 vs 野生型」。
    ''' </remarks>
    Public Function SetContext(genes As String(), values As Double(), Optional minCoverage As Double = 0.0) As ContextCoverage
        Call EnsureTrained()

        If genes Is Nothing Then
            Throw New ArgumentNullException(NameOf(genes))
        End If
        If values Is Nothing Then
            Throw New ArgumentNullException(NameOf(values))
        End If
        If genes.Length <> values.Length Then
            Throw New ArgumentException($"got {genes.Length} gene names but {values.Length} values", NameOf(values))
        End If

        Dim n As Integer = _network.NGene
        Dim state As Double() = CType(_network.BaselineRaw.Clone(), Double())
        Dim covered(n - 1) As Boolean
        Dim coverage As New ContextCoverage With {.NGene = n}
        Dim hit As Integer = 0

        For k As Integer = 0 To genes.Length - 1
            Dim idx As Integer = _network.IndexOfGene(genes(k))

            If idx < 0 Then
                Continue For
            End If

            Dim value As Double = values(k)

            ' 非有限数不能进入状态向量：算一次迭代就会把整条传播链污染成 NaN
            If value.IsNaNImaginary Then
                coverage.InvalidValues += 1
                Continue For
            End If

            state(idx) = value

            If Not covered(idx) Then
                covered(idx) = True
                hit += 1
            End If
        Next

        coverage.Matched = hit
        coverage.Provided = genes.Length - coverage.InvalidValues

        If hit = 0 Then
            Throw New ArgumentException("none of the given genes is present in the model", NameOf(genes))
        End If
        If minCoverage > 0.0 AndAlso coverage.Coverage < minCoverage Then
            Throw New ArgumentException(
                $"the context covers only {coverage.Coverage:P1} of the model genes " &
                $"({hit}/{n}), below the required minimum of {minCoverage:P1}", NameOf(genes))
        End If

        ' 越界诊断：迭代时限幅会把超出 [支持域 ± 1] 的输入剪平，这部分解释力要打折
        For i As Integer = 0 To n - 1
            If Not covered(i) Then
                Continue For
            End If

            Dim z As Double = _network.ToZValue(i, state(i))

            If z < _network.SupportMin(i) - 1.0 OrElse z > _network.SupportMax(i) + 1.0 Then
                coverage.OutsideSupport += 1
            End If
        Next

        Dim missing As New List(Of String)

        For i As Integer = 0 To n - 1
            If Not covered(i) Then
                Call missing.Add(_network.GeneNames(i))
            End If
        Next

        coverage.MissingGenes = missing.ToArray()

        _engine.SetBaseline(state)

        _contextState = state
        _contextCoverage = coverage

        Return coverage
    End Function

    ''' <summary>
    ''' 从表格文件（csv / tsv）读取转录组背景并注入为对照基线。
    ''' </summary>
    ''' <param name="path">
    ''' 表格路径，例如 pseudobulk 的 <c>gene_name,gene_id,mean_counts</c>
    ''' </param>
    ''' <param name="valueColumn">数值列的列名；为空时取最后一列</param>
    ''' <param name="minCoverage">要求的覆盖率下限（0–1，0 表示不检查）</param>
    ''' <returns>覆盖率诊断</returns>
    ''' <remarks>
    ''' 表里通常同时有基因符号与数据库 ID 两列，这里会逐个候选列去比对模型基因，
    ''' 选命中数最多的那一列使用（例如模型是按 Ensembl ID 建的，就会自动改用 <c>gene_id</c> 列）。
    ''' </remarks>
    Public Function LoadContext(path As String,
                                Optional valueColumn As String = Nothing,
                                Optional minCoverage As Double = 0.0) As ContextCoverage
        Call EnsureTrained()

        Dim table As ContextTable = GeneContextReader.ReadTable(path, valueColumn)
        Dim best As Integer = 0
        Dim bestHits As Integer = -1

        For c As Integer = 0 To table.Columns.Length - 1
            Dim hits As Integer = CountContextMatches(table.Columns(c))

            If hits > bestHits Then
                bestHits = hits
                best = c
            End If
        Next

        Dim coverage As ContextCoverage = SetContext(table.Columns(best), table.Values, minCoverage)

        coverage.GeneColumn = table.Headers(best)
        coverage.InvalidValues += table.Invalid

        Return coverage
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

        _contextState = Nothing
        _contextCoverage = Nothing
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
    Public Function PredictCombination(geneNames As IEnumerable(Of String),
                                       Optional mode As InterventionMode = InterventionMode.Knockout) As CombinationPerturbationResult
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
