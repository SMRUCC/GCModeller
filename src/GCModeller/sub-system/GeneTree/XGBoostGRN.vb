Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.BioSystem
Imports SMRUCC.genomics.BioSystem.Intervention
Imports SMRUCC.genomics.Model.Network.Regulons.ARACNE
Imports std = System.Math

''' <summary>
''' 基于 XGBoost 的非线性基因调控网络虚拟扰动实验（门面类）。
''' 
''' 把 readme 中的三段流程串起来：
''' 
''' <list type="number">
''' <item><description><strong>拓扑</strong>：由 <c>aracne.zip</c> 得到 <see cref="GeneRegulationGraph"/>（靶基因 → 调控因子）；</description></item>
''' <item><description><strong>拟合</strong>：用原始表达矩阵训练 <c>N</c> 个梯度提升树模型 <c>f_i(X_Reg(i))</c>；</description></item>
''' <item><description><strong>扰动</strong>：阻尼定点迭代求解扰动之后的新稳态，返回 <see cref="InterventionResult"/>。</description></item>
''' </list>
''' 
''' 典型用法：
''' 
''' <code>
''' Dim grn = XGBoostGRN.Open("./aracne.zip", "./GRN_expression_matrix.csv")
''' 
''' Call grn.Fit()
''' 
''' Dim ko = grn.KnockoutGene("TP53")
''' Dim oe = grn.OverexpressGene("MYC")
''' </code>
''' 
''' 本类实现了 <see cref="InsilicoPerturbationExperiment"/> 接口，
''' 因此可以与 <c>GEARS</c>、<c>BNLearn</c> 之中已有的虚拟扰动实现互换使用。
''' </summary>
''' <remarks>
''' <para>
''' **数值尺度**：训练与扰动的数值一律来自传入的表达矩阵（原始表达量，而不是 ARACNE 存档之中
''' rank 变换之后的秩值），因为树模型的外推裁剪区间与“敲除到 0”的语义都依赖于原始表达量。
''' </para>
''' <para>
''' **野生型基线**：<see cref="WildtypeMeans"/> / <see cref="WildtypeSDs"/> 由参与迭代的细胞种群
''' （<see cref="PopulationSize"/> 个细胞）估计，这样没有被任何模型预测的源节点在扰动前后的变化量恰好为 0；
''' 代价是基线本身带有抽样噪声，需要更高精度时应该增大 <see cref="GeneTreeConfig.PopulationSize"/>。
''' </para>
''' <para>
''' **控制漂移**：树模型的稳态并不严格等于观测到的表达谱，因此即使不做任何扰动，迭代也会让部分基因发生位移。
''' <see cref="ControlSteadyState"/> 给出了“不施加任何干预”时的稳态均值，可用于对扰动结果做基线校正与诊断。
''' </para>
''' <para>
''' **表达状态上下文**：<see cref="SetContext(IDictionary(Of String, Double), Boolean)"/> 可以把模型从
''' “训练时的野生型基线”切换到任意给定的表达状态（例如某个疾病态、处理态或者外部扰动实验的对照态），
''' 之后的 <see cref="Predict"/> / <see cref="KnockoutGene"/> / <see cref="OverexpressGene"/> /
''' <see cref="KnockDownGene"/> 都会以这个状态作为迭代起点与差异分析的基线。
''' </para>
''' </remarks>
Public Class XGBoostGRN : Implements InsilicoPerturbationExperiment

    Private ReadOnly m_graph As GeneRegulationGraph
    Private ReadOnly m_config As GeneTreeConfig
    Private ReadOnly m_expr As Matrix
    Private ReadOnly m_genes As String()
    Private ReadOnly m_geneIndex As Dictionary(Of String, Integer)

    Private m_tfFlags As Boolean() = New Boolean() {}
    Private m_models As GeneModel() = New GeneModel() {}
    Private m_means As Double() = New Double() {}
    Private m_sds As Double() = New Double() {}
    Private m_population As Double()() = New Double()() {}
    Private m_cells As Integer() = New Integer() {}
    Private m_report As TrainingReport
    Private m_solver As PerturbationSolver
    Private m_control As Double()
    Private m_controlSolve As SolveResult
    Private m_lastSolve As SolveResult
    Private m_context As Dictionary(Of String, Double)
    Private m_contextFix As Boolean = False
    Private m_contextAligned As Boolean = False
    Private m_contextNote As String = Nothing
    Private m_contextClampGenes As Integer = 0
    Private m_unknownContext As String() = New String() {}

    ''' <summary>调控网络拓扑</summary>
    Public ReadOnly Property Graph As GeneRegulationGraph
        Get
            Return m_graph
        End Get
    End Property

    ''' <summary>训练与求解参数</summary>
    Public ReadOnly Property Config As GeneTreeConfig
        Get
            Return m_config
        End Get
    End Property

    ''' <summary>用于训练的表达矩阵（从存档加载的模型为 <c>Nothing</c>）</summary>
    Public ReadOnly Property Expression As Matrix
        Get
            Return m_expr
        End Get
    End Property

    ''' <summary>基因名称（顺序与 <see cref="InterventionResult.GeneNames"/> 一致）</summary>
    Public ReadOnly Property GeneNames As String()
        Get
            Return m_genes
        End Get
    End Property

    ''' <summary>转录因子名称列表</summary>
    Public ReadOnly Property TFNames As String()
        Get
            Return m_genes _
                .SeqIterator() _
                .Where(Function(x) x.i < m_tfFlags.Length AndAlso m_tfFlags(x.i)) _
                .Select(Function(x) x.value) _
                .ToArray()
        End Get
    End Property

    ''' <summary>
    ''' 结构发现的汇总结果（未开启发现或者模型尚未训练时为 <c>Nothing</c>）
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Discovery As DiscoveryReport
        Get
            If m_report Is Nothing Then
                Return Nothing
            Else
                Return m_report.Discovery
            End If
        End Get
    End Property

    ''' <summary>
    ''' 模型之中由结构发现阶段新增的调控边数量
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property DiscoveredEdgesCount As Integer
        Get
            If Discovery Is Nothing Then
                Return 0
            Else
                Return Discovery.Accepted
            End If
        End Get
    End Property

    ''' <summary>逐基因模型（下标与 <see cref="GeneNames"/> 一致）</summary>
    Public ReadOnly Property Models As GeneModel()
        Get
            Return m_models
        End Get
    End Property

    ''' <summary>是否已经完成了拟合（或者已经从存档加载了模型）</summary>
    Public ReadOnly Property IsFitted As Boolean
        Get
            Return m_models.Length = m_genes.Length AndAlso m_models.Length > 0 AndAlso
                m_population.Length > 0
        End Get
    End Property

    ''' <summary>野生型（对照）表达均值</summary>
    Public ReadOnly Property WildtypeMeans As Double()
        Get
            Return m_means
        End Get
    End Property

    ''' <summary>野生型（对照）表达标准差</summary>
    Public ReadOnly Property WildtypeSDs As Double()
        Get
            Return m_sds
        End Get
    End Property

    ''' <summary>参与迭代求解的细胞数量</summary>
    Public ReadOnly Property PopulationSize As Integer
        Get
            Return m_population.Length
        End Get
    End Property

    ''' <summary>训练报告（从存档加载时为存档之中保存的那一份）</summary>
    Public ReadOnly Property TrainingReport As TrainingReport
        Get
            Return m_report
        End Get
    End Property

    ''' <summary>最近一次扰动求解的详细结果（收敛轮数、轨迹等）</summary>
    Public ReadOnly Property LastSolve As SolveResult
        Get
            Return m_lastSolve
        End Get
    End Property

    ''' <summary>
    ''' 不施加任何干预时的网络稳态均值（模拟表达谱自身的“吸引子”）
    ''' </summary>
    ''' <returns>每个基因的稳态均值</returns>
    ''' <remarks>
    ''' 该值第一次访问时才会计算，之后缓存。用它减去扰动结果即可得到“纯扰动效应”，
    ''' 从而剔除树模型自身的控制漂移。
    ''' 
    ''' 设置了表达状态上下文之后，控制稳态是“从该上下文出发、不做任何扰动”迭代得到的结果，
    ''' 因此它同时也是 <c>GeneTreeConfig.BaselineFromControlSteadyState</c> 生效时的差异分析基线。
    ''' </remarks>
    Public ReadOnly Property ControlSteadyState As Double()
        Get
            If m_control Is Nothing Then
                m_control = GetControlSolve().GetMeans()
            End If

            Return m_control
        End Get
    End Property

    ''' <summary>
    ''' 求解（并缓存）当前上下文下的控制稳态，即“不做任何扰动”时的网络吸引子。
    ''' </summary>
    ''' <returns>控制稳态的求解结果（包含逐细胞的状态与收敛信息）</returns>
    ''' <remarks>
    ''' 该结果同时被用作扰动的热启动点：从控制稳态出发，被扰动基因之外的部分已经在稳态上，
    ''' 因此只需要传播扰动本身，收敛速度远快于从原始观测谱出发。
    ''' 上下文（<see cref="SetContext(IDictionary(Of String, Double), Boolean)"/>）、
    ''' 求解参数（<see cref="ResetSolver"/>）或者模型本身发生变化时缓存会失效。
    ''' </remarks>
    Public Function GetControlSolve() As SolveResult
        ensureFitted()

        If m_controlSolve Is Nothing Then
            Dim solver As PerturbationSolver = getSolver()
            Dim means As Double() = CType(m_means.Clone(), Double())
            Dim sds As Double() = CType(m_sds.Clone(), Double())
            Dim initial As Double()() = clonePopulation()
            Dim contextPins As Dictionary(Of Integer, Double) = applyContext(means, sds, initial)
            Dim watch As Stopwatch = Stopwatch.StartNew()

            ' 控制稳态是所有扰动共享的基线，值得多花一些轮数把它真正迭代到位：
            ' 只有基线收敛了，单次扰动才能靠“脏传播剪枝”把计算限制在受影响的下游锥之内。
            m_controlSolve = solver.Solve(initial, contextPins, hardCap:=m_config.EffectiveMaxIterationsHardCap * 4)

            watch.Stop()

            If m_config.Verbose Then
                Console.WriteLine($"[GeneTree] 控制稳态求解完成（{watch.Elapsed.TotalSeconds.ToString("F1")}s）: {m_controlSolve.ToString}")
            End If
        End If

        Return m_controlSolve
    End Function

    ''' <summary>
    ''' 应用状态上下文时是否执行过分位对齐
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ContextAligned As Boolean
        Get
            Return m_contextAligned
        End Get
    End Property

    ''' <summary>
    ''' 上下文注入过程的诊断信息（对齐、钳制降级等），没有需要提示的信息时为<c>Nothing</c>
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ContextNote As String
        Get
            Return m_contextNote
        End Get
    End Property

    ''' <summary>
    ''' 上下文之中真正被钳制的基因数量（钳制被自动降级时为 0）
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ContextClampGenes As Integer
        Get
            Return m_contextClampGenes
        End Get
    End Property

    ''' <summary>
    ''' 重新创建稳态求解器（在运行期修改了求解参数之后调用）
    ''' </summary>
    ''' <returns>当前对象（便于链式调用）</returns>
    ''' <remarks>
    ''' 会同时失效控制稳态与上一次扰动结果的缓存，因此之后第一次扰动会重新求解控制稳态。
    ''' </remarks>
    Public Function ResetSolver() As XGBoostGRN
        m_solver = Nothing
        m_controlSolve = Nothing
        m_control = Nothing
        m_lastSolve = Nothing

        Return Me
    End Function

    ''' <summary>
    ''' 当前生效的表达状态上下文（基因名 → 表达值）。
    ''' 
    ''' 没有被 <see cref="SetContext(IDictionary(Of String, Double), Boolean)"/> 设置过的时候为<c>Nothing</c>。
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Context As IReadOnlyDictionary(Of String, Double)
        Get
            If m_context Is Nothing Then
                Return Nothing
            Else
                Return m_context
            End If
        End Get
    End Property

    ''' <summary>
    ''' 上下文之中的基因是否在迭代过程中被钳制（固定不变）
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ContextFix As Boolean
        Get
            Return m_contextFix
        End Get
    End Property

    ''' <summary>
    ''' 是否已经设置了表达状态上下文
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property HasContext As Boolean
        Get
            Return m_context IsNot Nothing AndAlso m_context.Count > 0
        End Get
    End Property

    ''' <summary>
    ''' 上下文之中无法映射到模型基因集的名字（用于提示 ID 空间不匹配）
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property UnknownContextGenes As String()
        Get
            Return m_unknownContext
        End Get
    End Property

    Private ReadOnly Property GeneIndex As Dictionary(Of String, Integer)
        Get
            Return m_geneIndex
        End Get
    End Property

#Region "constructors"

    Private Sub New(graph As GeneRegulationGraph,
                    config As GeneTreeConfig,
                    expr As Matrix,
                    genes As String(),
                    tfFlags As Boolean())

        m_graph = graph
        m_config = config
        m_expr = expr
        m_genes = genes
        m_tfFlags = If(tfFlags, New Boolean(genes.Length - 1) {})
        m_geneIndex = New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

        For i As Integer = 0 To genes.Length - 1
            m_geneIndex(genes(i)) = i
        Next
    End Sub

    ''' <summary>
    ''' 由 ARACNE 拓扑与表达矩阵打开一个尚未训练的 GeneTree 实验
    ''' </summary>
    ''' <param name="topologyZip">ARACNE 模型压缩包的路径（只会读取拓扑相关的三个条目）</param>
    ''' <param name="expressionCsv">原始基因表达矩阵的 CSV 路径（行=基因、列=样本）</param>
    ''' <param name="config">训练与求解配置；为空时使用快速档</param>
    ''' <returns></returns>
    Public Shared Function Open(topologyZip As String,
                                expressionCsv As String,
                                Optional config As GeneTreeConfig = Nothing) As XGBoostGRN

        If String.IsNullOrEmpty(expressionCsv) Then
            Throw New ArgumentNullException(NameOf(expressionCsv))
        End If
        If Not File.Exists(expressionCsv) Then
            Throw New FileNotFoundException($"找不到基因表达矩阵文件: {expressionCsv}", expressionCsv)
        End If

        Dim opt As GeneTreeConfig = If(config, GeneTreeConfig.CreateFast())
        Dim graph As GeneRegulationGraph = GeneRegulationGraph.LoadFromArchive(topologyZip, opt)

        Return Open(graph, Matrix.LoadData(expressionCsv), opt)
    End Function

    ''' <summary>
    ''' 由已加载的 ARACNE 网络与表达矩阵打开一个尚未训练的 GeneTree 实验
    ''' </summary>
    ''' <param name="network">ARACNE 推断结果</param>
    ''' <param name="expression">原始基因表达矩阵</param>
    ''' <param name="config">训练与求解配置；为空时使用快速档</param>
    ''' <returns></returns>
    Public Shared Function FromAracne(network As AracneNetwork,
                                      expression As Matrix,
                                      Optional config As GeneTreeConfig = Nothing) As XGBoostGRN

        Dim opt As GeneTreeConfig = If(config, GeneTreeConfig.CreateFast())

        Return Open(GeneRegulationGraph.FromAracne(network, opt), expression, opt)
    End Function

    ''' <summary>
    ''' 由调控拓扑与表达矩阵打开一个尚未训练的 GeneTree 实验
    ''' </summary>
    ''' <param name="graph">已经完成 top-K 裁剪的调控拓扑</param>
    ''' <param name="expression">原始基因表达矩阵</param>
    ''' <param name="config">训练与求解配置；为空时使用快速档</param>
    ''' <returns></returns>
    Public Shared Function Open(graph As GeneRegulationGraph,
                                expression As Matrix,
                                Optional config As GeneTreeConfig = Nothing) As XGBoostGRN

        If graph Is Nothing Then
            Throw New ArgumentNullException(NameOf(graph))
        End If
        If expression Is Nothing Then
            Throw New ArgumentNullException(NameOf(expression))
        End If

        Dim opt As GeneTreeConfig = If(config, GeneTreeConfig.CreateFast())

        Call opt.ValidateOrThrow()

        Dim genes As String() = expression.rownames

        If genes Is Nothing OrElse genes.Length = 0 Then
            Throw New InvalidDataException("表达矩阵之中没有任何基因行")
        End If

        Dim tfFlags As Boolean() = mapTfFlags(graph, genes)

        Return New XGBoostGRN(graph, opt, expression, genes, tfFlags)
    End Function

    Private Shared Function mapTfFlags(graph As GeneRegulationGraph, genes As String()) As Boolean()
        Dim flags As Boolean() = New Boolean(genes.Length - 1) {}
        Dim index As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)

        For i As Integer = 0 To graph.Genes.Length - 1
            If graph.TfFlags IsNot Nothing AndAlso graph.TfFlags.Length > i Then
                index(graph.Genes(i)) = graph.TfFlags(i)
            End If
        Next

        For i As Integer = 0 To genes.Length - 1
            Dim isTf As Boolean

            If index.TryGetValue(genes(i), isTf) Then
                flags(i) = isTf
            End If
        Next

        Return flags
    End Function

#End Region

#Region "fit"

    ''' <summary>
    ''' 训练逐基因的梯度提升树模型，并估计野生型基线与迭代种群
    ''' </summary>
    ''' <param name="progress">训练进度回调 <c>(已完成, 总数)</c></param>
    ''' <returns>训练报告</returns>
    Public Function Fit(Optional progress As Action(Of Integer, Integer) = Nothing) As TrainingReport
        If m_expr Is Nothing Then
            Throw New InvalidOperationException(
                "当前对象是从存档加载的，没有携带表达矩阵，无法重新训练；" &
                "请改用 XGBoostGRN.Open(topologyZip, expressionCsv) 从头训练")
        End If

        Dim trainer As GeneTreeTrainer = GeneTreeTrainer.Train(m_graph, m_expr, m_config, progress)

        m_models = trainer.Models
        m_report = trainer.Report

        Call buildPopulation()

        ' 模型与基线都变了，缓存必须失效
        m_solver = Nothing
        m_control = Nothing

        Return m_report
    End Function

    ''' <summary>
    ''' 估计野生型基线（均值/标准差）并构建迭代种群。
    ''' 
    ''' 基线只在参与迭代的细胞种群上统计，这样没有被任何模型预测的源节点
    ''' 在扰动前后的变化量恰好为 0。
    ''' </summary>
    Private Sub buildPopulation()
        If m_expr Is Nothing Then
            Return
        End If

        Dim rows As DataFrameRow() = m_expr.expression
        Dim nSamples As Integer = m_expr.sample_count

        If nSamples <= 0 Then
            nSamples = rows(0).samples
        End If

        Dim cells As Integer() = GeneTreeTrainer.SampleCells(nSamples, m_config.PopulationSize, m_config.Seed Xor &H5C1D)
        Dim nGenes As Integer = m_genes.Length
        Dim population As Double()() = New Double(nGenes - 1)() {}
        Dim means As Double() = New Double(nGenes - 1) {}
        Dim sds As Double() = New Double(nGenes - 1) {}

        For g As Integer = 0 To nGenes - 1
            Dim values As Double() = New Double(cells.Length - 1) {}
            Dim experiments As Double() = rows(g).experiments

            For c As Integer = 0 To cells.Length - 1
                Dim x As Double = experiments(cells(c))

                If Double.IsNaN(x) OrElse Double.IsInfinity(x) Then
                    x = 0.0R
                End If

                values(c) = x
            Next

            population(g) = values

            Dim sum As Double = 0.0R

            For c As Integer = 0 To values.Length - 1
                sum += values(c)
            Next

            Dim mean As Double = sum / values.Length

            means(g) = mean

            If values.Length > 1 Then
                Dim sse As Double = 0.0R

                For c As Integer = 0 To values.Length - 1
                    sse += (values(c) - mean) ^ 2
                Next

                sds(g) = std.Sqrt(sse / (values.Length - 1))
            End If
        Next

        m_population = population
        m_cells = cells
        m_means = means
        m_sds = sds
    End Sub

#End Region

#Region "expression context"

    ''' <summary>
    ''' 设置虚拟扰动的**表达状态上下文**：一个“基因名 → 表达值”的字典。
    ''' 
    ''' 上下文给出的是迭代求解的**初始状态**（背景表达谱）以及扰动分析的**基线**：
    ''' 
    ''' + 上下文之中的基因在种群的所有细胞上都被设置为给定的表达值；
    ''' + 上下文之外的其他基因沿用种群的野生型细胞值（保留单细胞异质性）；
    ''' + <see cref="InterventionResult.WildtypeMeans"/> 会用上下文的取值替换对应基因的基线均值，
    '''   因此 <see cref="InterventionResult.FoldChanges"/> 是相对“上下文状态”的变化量，
    '''   这样可以模拟在某个特定细胞状态（例如疾病态、处理态）之上再施加基因扰动；
    ''' + <paramref name="fix"/> 为<c>True</c> 时，上下文之中的基因在整个迭代过程中被钳制（固定为给定值），
    '''   相当于“基因敲入/外源表达固定”的实验，此时它们的标准差被置零（不做 z 检验）；
    '''   为<c>False</c> 时它们只作为起点，会随网络一起演化，标准差仍然是细胞种群上的统计量。
    ''' </summary>
    ''' <param name="context">基因名 → 表达值。名字大小写不敏感，无法映射的名字会记录在 <see cref="UnknownContextGenes"/> 之中</param>
    ''' <param name="fix">
    ''' 是否在迭代过程中钳制上下文基因的取值。
    ''' 
    ''' 钳制只对少量基因有意义：当钳制的基因数超过 <c>GeneTreeConfig.MaxClampRatio</c>（默认 5%）时，
    ''' 网络会失去几乎全部自由度、扰动结果必然趋近于零，因此库会**自动降级**为“仅作为迭代初始状态”，
    ''' 并把原因记录到 <see cref="ContextNote"/>。
    ''' </param>
    ''' <returns>当前对象（便于链式调用）</returns>
    ''' <remarks>
    ''' 外部管线的 context 与模型训练矩阵往往不是同一套归一化，直接注入会让特征落到决策树的极值叶子上，
    ''' 因此当 <c>GeneTreeConfig.ContextQuantileAlignment</c> 开启（默认）时，
    ''' 每次调用都会按每基因自身的种群分布做**分位对齐**，把值映射回模型尺度并保留基因间的相对高低，
    ''' 对齐结果可以通过 <see cref="ContextAligned"/> 与 <see cref="Context"/> 检查。
    ''' </remarks>
    ''' <exception cref="InvalidDataException">表达值不是有效数值</exception>
    ''' <exception cref="InvalidOperationException">上下文之中没有任何基因能够映射到模型上，或者模型尚未训练（无法做分位对齐）</exception>
    Public Function SetContext(context As IDictionary(Of String, Double), Optional fix As Boolean = False) As XGBoostGRN
        If context Is Nothing OrElse context.Count = 0 Then
            Return ClearContext()
        End If

        Dim values As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)
        Dim unknown As New List(Of String)

        For Each item As KeyValuePair(Of String, Double) In context
            Dim name As String = If(item.Key, "").Trim()

            If name.Length = 0 Then
                Continue For
            End If
            If Double.IsNaN(item.Value) OrElse Double.IsInfinity(item.Value) Then
                Throw New InvalidDataException($"上下文基因 '{name}' 的表达值 {item.Value} 不是有效数值")
            End If

            If m_geneIndex.ContainsKey(name) Then
                values(name) = item.Value
            Else
                Call unknown.Add(name)
            End If
        Next

        If values.Count = 0 Then
            Dim detail As String = If(unknown.Count = 0,
                "",
                $"（无法匹配的基因示例: {String.Join(", ", unknown.Take(10))}）")

            Throw New InvalidOperationException(
                $"表达状态上下文之中没有任何基因能够映射到模型的 {m_genes.Length} 个基因上{detail}")
        End If

        Dim notes As New List(Of String)
        Dim aligned As Boolean = False

        ' ---- 1. 分位对齐：把外部管线的数值映射回模型自身的表达尺度 ----
        If m_config.ContextQuantileAlignment Then
            If Not IsFitted Then
                Throw New InvalidOperationException(
                    "对上下文做分位对齐需要模型已经训练完成（或者从存档加载），请先调用 Fit() / XGBoostGRN.Load()")
            End If

            If values.Count >= 2 Then
                values = ExpressionContextAlignment.Align(values, m_population, m_geneIndex)
                aligned = True

                Call notes.Add($"context 已按每基因种群分布做分位对齐（{values.Count} 个基因）")
            Else
                ' 只有一个基因时“跨基因分位”没有定义，保留调用方给出的显式取值
                Call notes.Add("上下文只包含 1 个基因，分位对齐无定义，已按给定值直接注入")
            End If
        End If

        ' ---- 2. 钳制规模检查：全基因组 context 的钳制会让网络失去全部自由度 ----
        Dim clampGenes As Integer = 0
        Dim effectiveFix As Boolean = fix

        If fix Then
            Dim limit As Integer = std.Max(1, CInt(std.Truncate(m_config.MaxClampRatio * m_genes.Length)))

            If values.Count > limit Then
                effectiveFix = False

                Dim ratio As String = (m_config.MaxClampRatio * 100.0R).ToString("F1") & "%"

                Call notes.Add(
                    $"钳制基因数 {values.Count} 超过上限 {limit}（{ratio} × {m_genes.Length}），" &
                    "已自动降级为[仅作为迭代初始状态]")
            Else
                clampGenes = values.Count
            End If
        End If

        m_context = values
        m_contextFix = effectiveFix
        m_contextAligned = aligned
        m_contextClampGenes = clampGenes
        m_contextNote = If(notes.Count = 0, Nothing, String.Join("；", notes))
        m_unknownContext = unknown.ToArray
        ' 控制稳态与上下文相关，必须失效重算
        m_control = Nothing
        m_controlSolve = Nothing
        m_lastSolve = Nothing

        If m_config.Verbose AndAlso m_contextNote IsNot Nothing Then
            Console.WriteLine($"[GeneTree] {m_contextNote}")
        End If

        Return Me
    End Function

    ''' <summary>
    ''' 设置表达状态上下文（平行数组形式）
    ''' </summary>
    ''' <param name="genes">基因名数组</param>
    ''' <param name="values">与 <paramref name="genes"/> 一一对应的表达值</param>
    ''' <param name="fix">是否在迭代过程中钳制上下文基因的取值</param>
    ''' <returns>当前对象（便于链式调用）</returns>
    Public Function SetContext(genes As String(), values As Double(), Optional fix As Boolean = False) As XGBoostGRN
        If genes Is Nothing OrElse genes.Length = 0 Then
            Return ClearContext()
        End If
        If values Is Nothing OrElse values.Length <> genes.Length Then
            Throw New ArgumentException(
                $"基因名数组的长度 {If(genes Is Nothing, 0, genes.Length)} 与表达值数组的长度 " &
                $"{If(values Is Nothing, 0, values.Length)} 不一致")
        End If

        Dim context As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

        For i As Integer = 0 To genes.Length - 1
            Dim name As String = If(genes(i), "").Trim()

            If name.Length > 0 Then
                context(name) = values(i)
            End If
        Next

        Return SetContext(context, fix)
    End Function

    ''' <summary>
    ''' 清除表达状态上下文，回到训练时的野生型基线
    ''' </summary>
    ''' <returns>当前对象（便于链式调用）</returns>
    Public Function ClearContext() As XGBoostGRN
        m_context = Nothing
        m_contextFix = False
        m_contextAligned = False
        m_contextNote = Nothing
        m_contextClampGenes = 0
        m_unknownContext = New String() {}
        m_control = Nothing
        m_controlSolve = Nothing
        m_lastSolve = Nothing

        Return Me
    End Function

    ''' <summary>
    ''' 把当前的表达状态上下文叠加到基线与初始状态上
    ''' </summary>
    ''' <param name="means">基线均值（原地修改）</param>
    ''' <param name="sds">基线标准差（原地修改，上下文基因会被置零）</param>
    ''' <param name="initial">迭代的初始状态（原地修改）</param>
    ''' <returns>需要在迭代过程之中钳制的基因以及它们的取值</returns>
    Private Function applyContext(means As Double(),
                                  sds As Double(),
                                  initial As Double()()) As Dictionary(Of Integer, Double)

        Dim pinned As New Dictionary(Of Integer, Double)

        If Not HasContext Then
            Return pinned
        End If

        For Each item As KeyValuePair(Of String, Double) In m_context
            Dim idx As Integer = -1

            If Not m_geneIndex.TryGetValue(item.Key, idx) Then
                Continue For
            End If

            means(idx) = item.Value

            Dim values As Double() = initial(idx)

            For c As Integer = 0 To values.Length - 1
                values(c) = item.Value
            Next

            If m_contextFix Then
                ' 上下文给出了确切的取值并且在迭代之中被钳制，其抽样标准差不再有意义
                sds(idx) = 0.0R
                pinned(idx) = item.Value
            End If
        Next

        Return pinned
    End Function

#End Region

#Region "perturbation"

    ''' <summary>
    ''' 对一组干预定义执行虚拟扰动预测
    ''' </summary>
    ''' <param name="specs">干预定义集合；多个元素表示组合扰动</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 如果已经通过 <see cref="SetContext(IDictionary(Of String, Double), Boolean)"/> 设置了表达状态上下文，
    ''' 那么迭代会从上下文状态出发，并且变化量是相对上下文基线的。
    ''' 
    ''' 当 <c>GeneTreeConfig.BaselineFromControlSteadyState</c> 开启（默认）时，
    ''' 基线取“同一上下文下不做任何扰动”的控制稳态，迭代也从该稳态热启动：
    ''' 这样 FoldChange 只反映扰动响应，树模型自身的稳态漂移被自动扣除。
    ''' </remarks>
    Public Function Predict(specs As IEnumerable(Of InterventionSpec)) As InterventionResult
        Dim list As List(Of InterventionSpec) = specs.SafeQuery.ToList()

        If list.Count = 0 Then
            Throw New ArgumentException("至少需要一个干预定义")
        End If

        ensureFitted()

        Dim means As Double()
        Dim sds As Double() = CType(m_sds.Clone(), Double())
        Dim initial As Double()()
        Dim contextPins As Dictionary(Of Integer, Double)
        Dim warmStart As Boolean

        If HasContext AndAlso m_config.BaselineFromControlSteadyState Then
            Dim control As SolveResult = GetControlSolve()

            means = control.GetMeans()
            initial = cloneState(control.SteadyState)
            contextPins = getContextPins()
            warmStart = True
        Else
            means = CType(m_means.Clone(), Double())
            initial = clonePopulation()
            contextPins = applyContext(means, sds, initial)
            warmStart = False
        End If

        Return predictInternal(list, means, sds, initial, contextPins, warmStart)
    End Function

    ''' <summary>
    ''' 把被钳制的上下文基因整理成“基因下标 → 取值”的表（不修改任何状态）
    ''' </summary>
    ''' <returns></returns>
    Private Function getContextPins() As Dictionary(Of Integer, Double)
        Dim pinned As New Dictionary(Of Integer, Double)

        If Not HasContext OrElse Not m_contextFix Then
            Return pinned
        End If

        For Each item As KeyValuePair(Of String, Double) In m_context
            Dim idx As Integer = -1

            If m_geneIndex.TryGetValue(item.Key, idx) Then
                pinned(idx) = item.Value
            End If
        Next

        Return pinned
    End Function

    ''' <summary>
    ''' 克隆一个基因主序的迭代状态
    ''' </summary>
    ''' <param name="state"><c>state(gene)(cell)</c></param>
    ''' <returns></returns>
    Private Shared Function cloneState(state As Double()()) As Double()()
        Dim result As Double()() = New Double(state.Length - 1)() {}

        For g As Integer = 0 To state.Length - 1
            result(g) = CType(state(g).Clone(), Double())
        Next

        Return result
    End Function

    ''' <summary>
    ''' 组合扰动：同时干预多个基因，捕捉非加性的协同/拮抗效应
    ''' </summary>
    ''' <param name="geneNames">同时扰动的基因名集合</param>
    ''' <param name="mode">干预模式，默认敲除</param>
    ''' <returns></returns>
    Public Function PredictCombination(geneNames As IEnumerable(Of String),
                                       Optional mode As InterventionMode = InterventionMode.Knockout) As InterventionResult

        Dim list As New List(Of InterventionSpec)

        For Each gene As String In geneNames.SafeQuery
            Call list.Add(NewSpec(gene, mode))
        Next

        Return Predict(list)
    End Function

    ''' <summary>
    ''' 批量虚拟扰动：逐个基因执行一次单基因扰动
    ''' </summary>
    ''' <param name="geneNames">待扰动的基因名集合</param>
    ''' <param name="mode">干预模式，默认敲除</param>
    ''' <returns>每个基因对应的干预分析结果</returns>
    Public Function BatchPerturbation(geneNames As IEnumerable(Of String),
                                      Optional mode As InterventionMode = InterventionMode.Knockout) As List(Of InterventionResult)

        Dim result As New List(Of InterventionResult)

        For Each gene As String In geneNames.SafeQuery
            Call result.Add(Predict({NewSpec(gene, mode)}))
        Next

        Return result
    End Function

    Private Function predictInternal(specList As List(Of InterventionSpec),
                                     means As Double(),
                                     sds As Double(),
                                     initial As Double()(),
                                     Optional contextPins As Dictionary(Of Integer, Double) = Nothing,
                                     Optional warmStart As Boolean = False) As InterventionResult

        Dim n As Integer = m_genes.Length
        Dim pinned As New Dictionary(Of Integer, Double)
        Dim names As New List(Of String)

        If contextPins IsNot Nothing Then
            For Each item As KeyValuePair(Of Integer, Double) In contextPins
                pinned(item.Key) = item.Value
            Next
        End If

        For Each spec As InterventionSpec In specList
            Dim idx As Integer = resolveIndex(spec)

            If idx < 0 Then
                Continue For
            End If

            ' 干预幅度统一按“保留比例 / 倍数”计算，不再使用 mean ± k×SD 的高斯假设：
            ' 在 context 基线下 SD 与均值不同源（被钳制基因的 SD 还是 0），
            ' 会让敲低值退化为野生型均值、或者出现负值与荒谬的幅度。
            Dim value As Double

            Select Case spec.Mode
                Case InterventionMode.Knockout
                    value = 0.0R
                Case InterventionMode.Knockdown
                    value = means(idx) * m_config.KnockdownResidual
                Case InterventionMode.Overexpression
                    value = means(idx) * m_config.OverexpressionFactor
                Case Else
                    value = spec.Value
            End Select

            If m_config.ClipNegative AndAlso value < 0.0R Then
                ' 表达量不能为负
                value = 0.0R
            End If

            pinned(idx) = value

            If Not names.Contains(m_genes(idx)) Then
                Call names.Add(m_genes(idx))
            End If
        Next

        If pinned.Count = 0 Then
            Return CreateUndefinedResult(specList(0))
        End If

        ' 把干预值写入初始状态（每个细胞相同），之后由求解器逐轮钳制
        For Each item As KeyValuePair(Of Integer, Double) In pinned
            Dim values As Double() = initial(item.Key)

            For c As Integer = 0 To values.Length - 1
                values(c) = item.Value
            Next
        Next

        Dim solver As PerturbationSolver = getSolver()
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim solved As SolveResult = solver.Solve(initial, pinned, warmStart:=warmStart)

        watch.Stop()
        m_lastSolve = solved

        If m_config.Verbose Then
            Console.WriteLine($"[GeneTree] 扰动稳态求解完成（{watch.Elapsed.TotalSeconds.ToString("F2")}s）: {solved.ToString}")
        End If

        Dim mutant As Double() = solved.GetMeans()
        Dim fold As Double() = New Double(n - 1) {}
        Dim percent As Double() = New Double(n - 1) {}
        Dim zscore As Double() = New Double(n - 1) {}
        Dim significant As Boolean() = New Boolean(n - 1) {}

        For i As Integer = 0 To n - 1
            fold(i) = mutant(i) - means(i)

            If std.Abs(means(i)) > 0.0000000001R Then
                percent(i) = fold(i) / std.Abs(means(i)) * 100.0R
            End If

            Dim sd As Double = sds(i)

            If sd <= 0.000001R Then
                ' 没有可用的对照标准差（例如上下文基因被固定），不做 z 检验
                zscore(i) = 0.0R
            Else
                zscore(i) = fold(i) / sd
            End If

            significant(i) = pinned.ContainsKey(i) OrElse std.Abs(zscore(i)) > m_config.SignificanceZScore
        Next

        Dim resultSpec As InterventionSpec

        If specList.Count = 1 Then
            resultSpec = specList(0)
        Else
            resultSpec = New InterventionSpec With {
                .GeneName = String.Join("+", names),
                .GeneIndex = pinned.Keys.First(),
                .Mode = specList(0).Mode
            }
        End If

        Return New InterventionResult With {
            .Spec = resultSpec,
            .WildtypeMeans = CType(means.Clone(), Double()),
            .WildtypeSDs = CType(sds.Clone(), Double()),
            .MutantMeans = mutant,
            .FoldChanges = fold,
            .PercentChanges = percent,
            .ZScores = zscore,
            .IsSignificant = significant,
            .GeneNames = CType(m_genes.Clone(), String())
        }
    End Function

    ''' <summary>
    ''' 构造“目标基因不存在、未执行虚拟扰动”的降级结果
    ''' </summary>
    Private Function CreateUndefinedResult(spec As InterventionSpec) As InterventionResult
        Dim n As Integer = m_genes.Length

        Return New InterventionResult With {
            .Spec = spec,
            .WildtypeMeans = CType(m_means.Clone(), Double()),
            .WildtypeSDs = CType(m_sds.Clone(), Double()),
            .MutantMeans = CType(m_means.Clone(), Double()),
            .FoldChanges = New Double(n - 1) {},
            .PercentChanges = New Double(n - 1) {},
            .ZScores = New Double(n - 1) {},
            .IsSignificant = New Boolean(n - 1) {},
            .GeneNames = CType(m_genes.Clone(), String()),
            .Undefined = True
        }
    End Function

    Private Function NewSpec(geneName As String, mode As InterventionMode) As InterventionSpec
        Dim idx As Integer = -1

        If Not String.IsNullOrEmpty(geneName) Then
            Call m_geneIndex.TryGetValue(geneName.Trim(), idx)
        End If

        Return New InterventionSpec With {
            .GeneIndex = idx,
            .GeneName = geneName,
            .Mode = mode
        }
    End Function

    Private Function resolveIndex(spec As InterventionSpec) As Integer
        Dim idx As Integer = -1

        If Not String.IsNullOrEmpty(spec.GeneName) AndAlso m_geneIndex.TryGetValue(spec.GeneName.Trim(), idx) Then
            Return idx
        End If
        If spec.GeneIndex >= 0 AndAlso spec.GeneIndex < m_genes.Length Then
            Return spec.GeneIndex
        End If

        Return -1
    End Function

    Private Function getSolver() As PerturbationSolver
        If m_solver Is Nothing Then
            m_solver = New PerturbationSolver(m_models, m_config)
        End If

        Return m_solver
    End Function

    Private Function clonePopulation() As Double()()
        Dim result As Double()() = New Double(m_population.Length - 1)() {}

        For g As Integer = 0 To m_population.Length - 1
            result(g) = CType(m_population(g).Clone(), Double())
        Next

        Return result
    End Function

    Private Sub ensureFitted()
        If Not IsFitted Then
            Throw New InvalidOperationException("GeneTree 模型尚未拟合：请先调用 Fit()，或者通过 XGBoostGRN.Load() 加载已保存的模型")
        End If
    End Sub

#End Region

#Region "InsilicoPerturbationExperiment"

    ''' <summary>
    ''' 虚拟基因敲除：把目标基因表达置为 0，迭代求解网络的新稳态
    ''' </summary>
    ''' <param name="geneName">目标基因名</param>
    ''' <param name="nSamples">
    ''' 用于估计野生型基线的细胞数量；0 表示使用整个迭代种群。
    ''' </param>
    ''' <returns>干预分析结果</returns>
    Public Function KnockoutGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult Implements InsilicoPerturbationExperiment.KnockoutGene
        Return PredictWithBaseline(geneName, InterventionMode.Knockout, nSamples)
    End Function

    ''' <summary>
    ''' 虚拟基因过表达：把目标基因表达提升到基线的 <c>OverexpressionFactor</c> 倍（默认 2 倍），迭代求解新稳态
    ''' </summary>
    ''' <param name="geneName">目标基因名</param>
    ''' <param name="nSamples">用于估计野生型基线的细胞数量；0 表示使用整个迭代种群</param>
    ''' <returns>干预分析结果</returns>
    Public Function OverexpressGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult Implements InsilicoPerturbationExperiment.OverexpressGene
        Return PredictWithBaseline(geneName, InterventionMode.Overexpression, nSamples)
    End Function

    ''' <summary>
    ''' 虚拟基因下调：把目标基因表达降低到基线的 <c>KnockdownResidual</c> 比例（默认保留 20%），迭代求解新稳态
    ''' </summary>
    ''' <param name="geneName">目标基因名</param>
    ''' <param name="nSamples">用于估计野生型基线的细胞数量；0 表示使用整个迭代种群</param>
    ''' <returns>干预分析结果</returns>
    Public Function KnockDownGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult Implements InsilicoPerturbationExperiment.KnockDownGene
        Return PredictWithBaseline(geneName, InterventionMode.Knockdown, nSamples)
    End Function

    ''' <summary>
    ''' 单次预测的便捷入口：可以临时指定用于估计基线的细胞数量
    ''' </summary>
    ''' <param name="geneName">目标基因名</param>
    ''' <param name="mode">干预模式</param>
    ''' <param name="nSamples">参与统计的细胞数量；≤0 或者不小于种群大小时沿用训练时的基线</param>
    ''' <returns></returns>
    Private Function PredictWithBaseline(geneName As String, mode As InterventionMode, nSamples As Integer) As InterventionResult
        ensureFitted()

        Dim spec As InterventionSpec = NewSpec(geneName, mode)

        ' 控制稳态基线由迭代求得，与细胞抽样无关，此时 nSamples 不再参与
        If HasContext AndAlso m_config.BaselineFromControlSteadyState Then
            Return Predict({spec})
        End If

        If nSamples <= 0 OrElse nSamples >= m_population.Length OrElse resolveIndex(spec) < 0 Then
            Return Predict({spec})
        End If

        ' 临时基线：从迭代种群之中抽取 nSamples 个细胞重新统计均值/标准差
        Dim picked As Integer() = GeneTreeTrainer.SampleCells(m_population.Length, nSamples, m_config.Seed Xor &H7E31)
        Dim population As Double()() = New Double(m_population.Length - 1)() {}
        Dim means As Double() = New Double(m_genes.Length - 1) {}
        Dim sds As Double() = New Double(m_genes.Length - 1) {}

        For g As Integer = 0 To m_population.Length - 1
            Dim source As Double() = m_population(g)
            Dim values As Double() = New Double(picked.Length - 1) {}
            Dim sum As Double = 0.0R

            For c As Integer = 0 To picked.Length - 1
                values(c) = source(picked(c))
                sum += values(c)
            Next

            population(g) = values
            means(g) = sum / values.Length

            If values.Length > 1 Then
                Dim sse As Double = 0.0R

                For c As Integer = 0 To values.Length - 1
                    sse += (values(c) - means(g)) ^ 2
                Next

                sds(g) = std.Sqrt(sse / (values.Length - 1))
            End If
        Next

        Dim contextPins As Dictionary(Of Integer, Double) = applyContext(means, sds, population)

        Return predictInternal(New List(Of InterventionSpec) From {spec}, means, sds, population, contextPins)
    End Function

#End Region

#Region "storage"

    Friend ReadOnly Property PopulationData As Double()()
        Get
            Return m_population
        End Get
    End Property

    Friend ReadOnly Property PopulationCells As Integer()
        Get
            Return m_cells
        End Get
    End Property

    Friend ReadOnly Property ReportData As TrainingReport
        Get
            Return m_report
        End Get
    End Property

    Friend ReadOnly Property ExpressionData As Matrix
        Get
            Return m_expr
        End Get
    End Property

    ''' <summary>
    ''' 从存档还原模型的内部状态（仅供 <see cref="GeneTreeStorage"/> 使用）
    ''' </summary>
    Friend Sub RestoreState(models As GeneModel(),
                            means As Double(),
                            sds As Double(),
                            population As Double()(),
                            cells As Integer(),
                            report As TrainingReport)

        m_models = If(models, New GeneModel() {})
        m_means = If(means, New Double() {})
        m_sds = If(sds, New Double() {})
        m_population = If(population, New Double()() {})
        m_cells = If(cells, New Integer() {})
        m_report = report
        m_solver = Nothing
        m_control = Nothing
    End Sub

    ''' <summary>
    ''' 从存档创建对象（仅供 <see cref="GeneTreeStorage"/> 使用）
    ''' </summary>
    Friend Shared Function FromArchive(graph As GeneRegulationGraph,
                                       config As GeneTreeConfig,
                                       genes As String(),
                                       tfFlags As Boolean()) As XGBoostGRN

        Return New XGBoostGRN(graph, config, Nothing, genes, tfFlags)
    End Function

    ''' <summary>
    ''' 把当前模型保存为一个 zip 存档（加载后无需重新训练即可继续扰动实验）
    ''' </summary>
    ''' <param name="file">目标可写流；调用方负责其生命周期，本方法不会关闭它</param>
    Public Sub Save(file As Stream)
        Call GeneTreeStorage.Save(Me, file)
    End Sub

    ''' <summary>
    ''' 从 zip 存档加载模型
    ''' </summary>
    ''' <param name="file">zip 压缩包的可读流；调用方负责其生命周期，本方法不会关闭它</param>
    ''' <returns></returns>
    Public Shared Function Load(file As Stream) As XGBoostGRN
        Return GeneTreeStorage.Load(file)
    End Function

    ''' <summary>
    ''' 从 zip 存档加载模型
    ''' </summary>
    ''' <param name="zipFile">zip 压缩包的路径</param>
    ''' <returns></returns>
    Public Shared Function Load(zipFile As String) As XGBoostGRN
        If String.IsNullOrEmpty(zipFile) Then
            Throw New ArgumentNullException(NameOf(zipFile))
        End If
        If Not File.Exists(zipFile) Then
            Throw New FileNotFoundException($"找不到 GeneTree 模型文件: {zipFile}", zipFile)
        End If

        Using stream As Stream = File.OpenRead(zipFile)
            Return Load(stream)
        End Using
    End Function

#End Region

    ''' <summary>
    ''' 输出实验配置与模型的摘要
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function ToString() As String
        Dim sb As New StringBuilder()

        Call sb.AppendLine($"GeneTree: {m_genes.Length} genes, population={m_population.Length}, " &
                           $"{m_models.Count(Function(m) m IsNot Nothing)} fitted models")
        Call sb.AppendLine($"topology: {m_graph.ToString}")
        Call sb.Append($"config: {m_config.ToString}")

        Return sb.ToString()
    End Function

End Class

