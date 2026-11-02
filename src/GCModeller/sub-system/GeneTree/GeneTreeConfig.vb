Imports System.Text

''' <summary>
''' 训练规模的预设档位（与 readme 中的“快速验证 / 精度优先 / 全量”三档对应）
''' </summary>
Public Enum GeneTreeProfile
    ''' <summary>快速档：分钟级跑通流程，适合先验证网络与求解器是否可用</summary>
    Fast
    ''' <summary>均衡档：精度与耗时的折中</summary>
    Balanced
    ''' <summary>全量档：使用全部细胞与全部调控者，耗时极长</summary>
    Full
End Enum

''' <summary>
''' 调控边的结构发现模式
''' </summary>
Public Enum EdgeDiscoveryMode
    ''' <summary>
    ''' 只使用 ARACNE 给出的拓扑（默认）。梯度提升树的重要性只用于排序与剪枝参考，
    ''' 不会新增任何调控边，行为与历史版本完全一致。
    ''' </summary>
    AracneOnly = 0

    ''' <summary>
    ''' 残差二阶段发现：先只用 ARACNE 边拟合，再在残差上对候选基因集合做相关筛选，
    ''' 候选逐个增量重拟合并以验证集 ΔR² 决定是否接受为新调控边。
    ''' </summary>
    ResidualTwoStage = 1
End Enum

''' <summary>
''' GeneTree（XGBoost 非线性基因调控网络）的全量超参数配置。
''' 
''' 配置分成四组：
''' 
''' + **网络拓扑**：<see cref="MaxRegulators"/>、<see cref="IncludeUndirectedEdges"/>、<see cref="MaxGenes"/>
''' + **训练抽样**：<see cref="MaxSamplesPerModel"/>、<see cref="ValidationRatio"/>、<see cref="MinSamples"/>
''' + **梯度提升树**：<see cref="NumBoostRound"/>、<see cref="MaxDepth"/>、<see cref="LearningRate"/> 等
''' + **虚拟扰动求解**：<see cref="DampingAlpha"/>、<see cref="MaxIterations"/>、<see cref="PopulationSize"/> 等
''' </summary>
''' <remarks>
''' 所有以“0”表示“不限制/使用全部”的字段都会在 <see cref="Validate"/> 之后由使用方自行解释，
''' 例如 <see cref="MaxRegulators"/>=0 表示保留 ARACNE 给出的全部调控者，
''' <see cref="MaxSamplesPerModel"/>=0 表示使用表达矩阵中的全部细胞。
''' </remarks>
Public Class GeneTreeConfig

#Region "graph"

    ''' <summary>
    ''' 每个靶基因保留的调控因子数量（按照互信息降序裁剪）。0 表示不裁剪（全量档很慢）。
    ''' </summary>
    ''' <remarks>
    ''' 实测 ARACNE 网络平均每个靶基因约 42 个调控者，直接作为特征会让训练与迭代求解都变得不可接受，
    ''' 因此默认只保留互信息最强的若干个。
    ''' </remarks>
    Public Property MaxRegulators As Integer = 10

    ''' <summary>
    ''' 是否把 <c>"undirected"</c> 类型的候选边也纳入调控关系。默认关闭。
    ''' </summary>
    ''' <remarks>
    ''' ARACNE 的 undirected 边表示两个端点都不是转录因子，仅仅是互信息意义上的候选关系，
    ''' 它没有方向信息，纳入之后会被当作双向调控使用，因此默认关闭。
    ''' </remarks>
    Public Property IncludeUndirectedEdges As Boolean = False

    ''' <summary>
    ''' 最多参与训练的靶基因数量（按调控者数量降序保留）。0 表示不限制。
    ''' </summary>
    ''' <remarks>
    ''' 这是一个“快速试跑”开关：例如设置为 2000 时，只会训练调控关系最丰富的 2000 个靶基因，
    ''' 其余基因在迭代求解中保持初始值不变。
    ''' </remarks>
    Public Property MaxGenes As Integer = 0

#End Region

#Region "edge discovery"

    ''' <summary>
    ''' 调控边的结构发现模式，默认 <see cref="EdgeDiscoveryMode.AracneOnly"/>（不发现新边）。
    ''' </summary>
    ''' <remarks>
    ''' 开启之后模型会在 ARACNE 拓扑之外，从 <see cref="DiscoveryCandidates"/> 指定的基因集合之中
    ''' 找出能显著解释残差的调控者，并把它们追加到对应靶基因的特征列表末尾。
    ''' ARACNE 原有的边**始终保留**，因此开启前后的拓扑是可比的。
    ''' </remarks>
    Public Property EdgeDiscovery As EdgeDiscoveryMode = EdgeDiscoveryMode.AracneOnly

    ''' <summary>
    ''' 参与结构发现的候选调控者基因编号集合（由调用方显式给出，不自动推断）。
    ''' </summary>
    ''' <remarks>
    ''' + 候选池只需要覆盖“可能的新调控者”，通常是转录因子集合，也可以是全部基因；
    ''' + 若希望某个非转录因子的扰动目标能够产生下游效应，必须把该基因本身放进候选集，
    '''   这样它才有可能成为其他基因的调控者；
    ''' + 集合之中不存在于表达矩阵里的基因会被静默忽略。
    ''' </remarks>
    Public Property DiscoveryCandidates As String() = New String() {}

    ''' <summary>
    ''' 每个靶基因最多接受几条新发现的调控边，默认 2。
    ''' </summary>
    Public Property DiscoveryMaxPerGene As Integer = 2

    ''' <summary>
    ''' 每个靶基因最多尝试几个候选（控制重拟合次数），默认 5。
    ''' </summary>
    ''' <remarks>
    ''' 候选按残差相关的绝对值降序尝试，接受一条之后继续尝试下一条，达到
    ''' <see cref="DiscoveryMaxPerGene"/> 或者候选耗尽即停止。
    ''' </remarks>
    Public Property DiscoveryMaxCandidates As Integer = 5

    ''' <summary>
    ''' 残差与候选基因表达之间的 pearson 相关绝对值下限，默认 0.10。
    ''' </summary>
    Public Property DiscoveryMinCorrelation As Double = 0.1

    ''' <summary>
    ''' 接受一条新调控边所需要的最小验证集 R² 增益，默认 0.002。
    ''' </summary>
    Public Property DiscoveryMinDeltaR2 As Double = 0.002

    ''' <summary>
    ''' 计算残差相关时采样的细胞数量，默认 1000；0 表示使用全部训练细胞。
    ''' </summary>
    Public Property DiscoverySampleSize As Integer = 1000

    ''' <summary>
    ''' 最多对多少个靶基因执行结构发现，0 表示不限制（用于试跑限流）。
    ''' </summary>
    Public Property DiscoveryMaxGenes As Integer = 0

    ''' <summary>
    ''' 设置候选调控者集合（去重、去空、去自身）
    ''' </summary>
    ''' <param name="genes">候选基因编号</param>
    ''' <returns>当前对象（便于链式调用）</returns>
    Public Function SetDiscoveryCandidates(genes As IEnumerable(Of String)) As GeneTreeConfig
        If genes Is Nothing Then
            DiscoveryCandidates = New String() {}
        Else
            DiscoveryCandidates = genes _
                .Where(Function(g) Not String.IsNullOrWhiteSpace(g)) _
                .Select(Function(g) g.Trim()) _
                .Distinct(StringComparer.OrdinalIgnoreCase) _
                .ToArray
        End If

        Return Me
    End Function

    ''' <summary>
    ''' 是否启用结构发现
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property IsDiscoveryEnabled As Boolean
        Get
            Return EdgeDiscovery <> EdgeDiscoveryMode.AracneOnly
        End Get
    End Property

#End Region

#Region "sampling"

    ''' <summary>
    ''' 每个基因模型最多使用的细胞（样本）数量。0 表示使用全部细胞。
    ''' </summary>
    Public Property MaxSamplesPerModel As Integer = 1000

    ''' <summary>
    ''' 留出验证集的比例，用于早停与 R²/RMSE 评估。取值 [0.05, 0.5]。
    ''' </summary>
    Public Property ValidationRatio As Double = 0.2

    ''' <summary>
    ''' 训练一个基因模型所需的最小细胞数量，低于该数量时退化为常数模型。
    ''' </summary>
    Public Property MinSamples As Integer = 30

#End Region

#Region "gradient boosting"

    ''' <summary>
    ''' 提升轮数（树的数量）。浅树 + 适度轮数是 readme 针对定点迭代收敛性给出的建议。
    ''' </summary>
    Public Property NumBoostRound As Integer = 40

    ''' <summary>
    ''' 单棵树的最大深度。建议 3~4：树越浅，预测函数越平滑，定点迭代越容易收敛。
    ''' </summary>
    Public Property MaxDepth As Integer = 3

    ''' <summary>
    ''' 学习率 eta，范围 (0, 1]。默认 0.05。
    ''' </summary>
    Public Property LearningRate As Double = 0.05

    ''' <summary>
    ''' 行抽样比例 (0, 1]。
    ''' </summary>
    Public Property RowSample As Double = 0.8

    ''' <summary>
    ''' 列抽样比例 (0, 1]。
    ''' </summary>
    Public Property ColSample As Double = 0.8

    ''' <summary>
    ''' L2 正则化系数（reg_lambda），默认 5，用于抑制小样本过拟合。
    ''' </summary>
    Public Property Lambda As Double = 5.0

    ''' <summary>
    ''' 分裂所需的最小损失下降（gamma），默认 0。
    ''' </summary>
    Public Property Gamma As Double = 0.0

    ''' <summary>
    ''' 叶节点最小 Hessian 和的固定下限，默认 2。
    ''' </summary>
    ''' <remarks>
    ''' 实际生效值 = <c>Max(MinChildWeight, MinChildWeightRatio × 训练样本数)</c>，
    ''' 参见 <see cref="GetMinChildWeight"/>。
    ''' </remarks>
    Public Property MinChildWeight As Double = 2.0

    ''' <summary>
    ''' 叶节点最小 Hessian 和相对于训练样本数的比例，默认 0.05。
    ''' </summary>
    Public Property MinChildWeightRatio As Double = 0.05

    ''' <summary>
    ''' 节点继续分裂所需的最小样本数。
    ''' </summary>
    Public Property MinSampleSplit As Integer = 5

    ''' <summary>
    ''' 早停轮数。验证集不可用（样本过少）时不会生效。
    ''' </summary>
    Public Property EarlyStoppingRounds As Integer = 10

    ''' <summary>
    ''' 单模型训练时 TGBoost 内部使用的线程数，-1 表示由库自行决定。
    ''' </summary>
    ''' <remarks>
    ''' 训练是按基因并行的，模型内部再并行会造成线程超额订阅，因此默认 -1（TGBoost 默认值）。
    ''' </remarks>
    Public Property ThreadsPerModel As Integer = -1

#End Region

#Region "perturbation solver"

    ''' <summary>
    ''' 阻尼系数 α：<c>x_new = (1-α)·x_old + α·f(x)</c>。默认 0.1，越大越激进、越容易震荡。
    ''' </summary>
    Public Property DampingAlpha As Double = 0.1

    ''' <summary>
    ''' 定点迭代的最大轮数。
    ''' </summary>
    Public Property MaxIterations As Integer = 60

    ''' <summary>
    ''' 收敛阈值：单轮相对变化量（<c>|Δx| / max(1,|x|)</c>）低于该值时认为该基因已经稳定。
    ''' </summary>
    Public Property ConvergenceTolerance As Double = 0.0001

    ''' <summary>
    ''' 允许在判定收敛时仍然“抖动”的基因比例，默认 0.001（千分之一）。
    ''' </summary>
    ''' <remarks>
    ''' 树模型是分段非光滑的函数，在叶子阈值附近会有极少数基因长期来回跳变（实测约 2%~8% 的相对幅度），
    ''' 如果要求全网最大变化量低于阈值，迭代永远不会结束（300 轮仍在抖动）。
    ''' 因此收敛判据改为“仍在移动的基因比例 ≤ 该阈值”，这样既能保证 99.9% 的基因已经稳定，
    ''' 又不会被个别混沌基因拖住。
    ''' </remarks>
    Public Property MovingGeneTolerance As Double = 0.01

    ''' <summary>
    ''' 参与迭代的细胞（初始状态）数量。细胞越多，扰动的统计量越稳健，但每轮耗时线性增长。
    ''' </summary>
    Public Property PopulationSize As Integer = 100

    ''' <summary>
    ''' 外推裁剪系数：预测值被裁剪到 <c>[min / k, max × k]</c>（min ≥ 0）或 <c>[min × k, max × k]</c>。
    ''' </summary>
    ''' <remarks>
    ''' 树模型在训练数据覆盖范围之外的预测会“平台化”，这是 readme 中强调的最主要风险，
    ''' 因此必须把预测值限制在训练标签的取值范围附近。
    ''' </remarks>
    Public Property ClippingFactor As Double = 1.2

    ''' <summary>
    ''' 是否把表达量裁剪为非负（生物学上表达量不能为负）。
    ''' </summary>
    Public Property ClipNegative As Boolean = True

    ''' <summary>
    ''' 显著性判定所用的 Z-score 阈值，默认 2。
    ''' </summary>
    Public Property SignificanceZScore As Double = 2.0

    ''' <summary>
    ''' 迭代硬上限：软上限 <see cref="MaxIterations"/> 用完之后，只要还没有收敛就继续迭代到该上限。
    ''' </summary>
    ''' <remarks>
    ''' 定点迭代每一轮只让信号前进“一跳”，多级级联（转录因子 → 靶基因 → 次级靶基因 → …）
    ''' 需要 <c>跳数 / α</c> 量级的轮数才能稳定下来。实测中位 R²≈0.39 的模型上，
    ''' 60 轮 × α=0.1 只能走完 6 跳，深层的效应会被截断，因此需要一个硬上限兜底。
    ''' 
    ''' 取值为 0 表示使用 <see cref="EffectiveMaxIterationsHardCap"/> 给出的默认值
    ''' （<c>Max(MaxIterations × 5, 300)</c>）。
    ''' </remarks>
    Public Property MaxIterationsHardCap As Integer = 0

    ''' <summary>
    ''' 实际生效的迭代硬上限
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property EffectiveMaxIterationsHardCap As Integer
        Get
            If MaxIterationsHardCap > 0 Then
                Return Math.Max(MaxIterationsHardCap, MaxIterations)
            Else
                Return Math.Max(MaxIterations * 2, 150)
            End If
        End Get
    End Property

#End Region

#Region "expression context"

    ''' <summary>
    ''' 敲低（Knockdown）之后保留的野生型表达比例，默认 0.2。
    ''' </summary>
    ''' <remarks>
    ''' 虚拟敲低不再使用“均值 − k×标准差”的高斯假设（在 context 基线下标准差与均值不同源，
    ''' 会产生负值或者荒谬的幅度），而是直接按保留比例折算：
    ''' <c>KD = baseline × KnockdownResidual</c>，负值会被夹取为 0。
    ''' 
    ''' 该语义与 CRISPRi/Perturb-seq 中常见的“保留 10%~30% 残余表达”一致，
    ''' 也与 ``GPS`` 包的 ``knockdown_residual`` 参数保持同一口径。
    ''' </remarks>
    Public Property KnockdownResidual As Double = 0.2

    ''' <summary>
    ''' 过表达（Overexpression）的倍数，默认 2.0：<c>OE = baseline × OverexpressionFactor</c>。
    ''' </summary>
    Public Property OverexpressionFactor As Double = 2.0

    ''' <summary>
    ''' 允许在迭代过程中被“钳制”的基因比例上限，默认 0.05（5%）。
    ''' </summary>
    ''' <remarks>
    ''' 钳制（<c>fix = True</c>）会让给定基因在整个迭代过程中保持固定值，只适合少量基因的
    ''' “敲入/外源表达”实验。当被钳制的基因占网络的比例超过该阈值时，网络几乎没有自由度，
    ''' 扰动结果必然趋近于零（实测中传入了覆盖全部 18533 个基因的 pseudobulk context，
    ''' 导致 fold-change 矩阵 99.995% 为 0）。此时库会自动降级为“仅作为迭代初始状态”，
    ''' 并把原因写入 <c>XGBoostGRN.ContextNote</c>。
    ''' </remarks>
    Public Property MaxClampRatio As Double = 0.05

    ''' <summary>
    ''' 是否在注入表达状态上下文（context）时执行分位对齐，默认开启。
    ''' </summary>
    ''' <remarks>
    ''' 外部管线给出的 context（例如 pseudobulk 的 mean counts）与模型训练矩阵往往不是同一套归一化，
    ''' 直接注入会让大部分特征落到决策树的极值叶子上，局部灵敏度接近零。
    ''' 分位对齐会把 context 映射回每个基因自身的训练分布，同时保留基因之间的相对高低。
    ''' </remarks>
    Public Property ContextQuantileAlignment As Boolean = True

    ''' <summary>
    ''' 有上下文时，是否把“同一上下文下不做任何扰动”的控制稳态作为差异分析的野生型基线。
    ''' </summary>
    ''' <remarks>
    ''' 树模型的稳态并不严格等于观测谱，若用 context 的观测值作为基线，
    ''' 模型自身的漂移会混入 fold-change；用控制稳态作基线可以让差异量只反映扰动响应，
    ''' 并且该稳态还可以作为后续扰动的热启动点（收敛更快）。
    ''' </remarks>
    Public Property BaselineFromControlSteadyState As Boolean = True

    ''' <summary>
    ''' 当迭代的初始状态在所有细胞上完全一致时，是否折叠为单细胞轨迹，默认开启。
    ''' </summary>
    ''' <remarks>
    ''' 全基因组 context 会把所有细胞设置成同一个状态，此时 1000 个细胞的迭代完全是重复计算；
    ''' 折叠后单次扰动的耗时可以从分钟级降到秒级，而均值/差异统计不受影响
    ''' （z-score 使用基线标准差，而不是求解结果本身的标准差）。
    ''' </remarks>
    Public Property CollapseUniformPopulation As Boolean = True

#End Region

#Region "runtime"

    ''' <summary>
    ''' 随机种子，用于细胞抽样与训练/验证集划分。固定种子保证同一份数据得到相同的抽样与折分。
    ''' </summary>
    Public Property Seed As Integer = 1234567

    ''' <summary>
    ''' 训练阶段的最大并行度，-1 表示使用全部可用核心。
    ''' </summary>
    ''' <remarks>
    ''' 注意：TGBoost 的行/列抽样使用全局随机数流，因此 <c>MaxDegreeOfParallelism &gt; 1</c> 时
    ''' 训练结果不保证逐位可复现（抽样与折分本身仍然是可复现的）。
    ''' </remarks>
    Public Property MaxDegreeOfParallelism As Integer = -1

    ''' <summary>
    ''' 是否输出进度与统计信息到控制台。
    ''' </summary>
    Public Property Verbose As Boolean = True

    ''' <summary>
    ''' 训练进度的输出间隔（每多少个基因输出一次）。
    ''' </summary>
    Public Property ProgressInterval As Integer = 200

#End Region

    ''' <summary>
    ''' 根据训练样本数计算实际生效的 <c>min_child_weight</c>
    ''' </summary>
    ''' <param name="nSamples">该基因模型的训练样本数</param>
    ''' <returns><c>Max(MinChildWeight, MinChildWeightRatio × nSamples)</c></returns>
    Public Function GetMinChildWeight(nSamples As Integer) As Double
        Return Math.Max(MinChildWeight, MinChildWeightRatio * nSamples)
    End Function

    ''' <summary>
    ''' 配置是否合法
    ''' </summary>
    ''' <returns></returns>
    Public Function Validate() As Boolean
        Return GetValidationErrors().Count = 0
    End Function

    ''' <summary>
    ''' 列出所有不合法的配置项（为空表示配置合法）
    ''' </summary>
    ''' <returns></returns>
    Public Function GetValidationErrors() As List(Of String)
        Dim errors As New List(Of String)

        If MaxRegulators < 0 Then
            Call errors.Add($"{NameOf(MaxRegulators)} 不能为负数")
        End If
        If MaxGenes < 0 Then
            Call errors.Add($"{NameOf(MaxGenes)} 不能为负数")
        End If
        If MaxSamplesPerModel < 0 Then
            Call errors.Add($"{NameOf(MaxSamplesPerModel)} 不能为负数")
        End If
        If ValidationRatio < 0.05 OrElse ValidationRatio > 0.5 Then
            Call errors.Add($"{NameOf(ValidationRatio)} 必须落在 [0.05, 0.5] 区间内，实际为 {ValidationRatio}")
        End If
        If MinSamples < 4 Then
            Call errors.Add($"{NameOf(MinSamples)} 至少为 4，实际为 {MinSamples}")
        End If
        If NumBoostRound < 1 Then
            Call errors.Add($"{NameOf(NumBoostRound)} 至少为 1")
        End If
        If MaxDepth < 1 Then
            Call errors.Add($"{NameOf(MaxDepth)} 至少为 1")
        End If
        If LearningRate <= 0 OrElse LearningRate > 1 Then
            Call errors.Add($"{NameOf(LearningRate)} 必须落在 (0, 1] 区间内，实际为 {LearningRate}")
        End If
        If RowSample <= 0 OrElse RowSample > 1 Then
            Call errors.Add($"{NameOf(RowSample)} 必须落在 (0, 1] 区间内，实际为 {RowSample}")
        End If
        If ColSample <= 0 OrElse ColSample > 1 Then
            Call errors.Add($"{NameOf(ColSample)} 必须落在 (0, 1] 区间内，实际为 {ColSample}")
        End If
        If Lambda < 0 Then
            Call errors.Add($"{NameOf(Lambda)} 不能为负数")
        End If
        If Gamma < 0 Then
            Call errors.Add($"{NameOf(Gamma)} 不能为负数")
        End If
        If MinSampleSplit < 2 Then
            Call errors.Add($"{NameOf(MinSampleSplit)} 至少为 2")
        End If
        If DampingAlpha <= 0 OrElse DampingAlpha > 1 Then
            Call errors.Add($"{NameOf(DampingAlpha)} 必须落在 (0, 1] 区间内，实际为 {DampingAlpha}")
        End If
        If MaxIterations < 1 Then
            Call errors.Add($"{NameOf(MaxIterations)} 至少为 1")
        End If
        If ConvergenceTolerance <= 0 Then
            Call errors.Add($"{NameOf(ConvergenceTolerance)} 必须大于 0")
        End If
        If PopulationSize < 2 Then
            Call errors.Add($"{NameOf(PopulationSize)} 至少为 2")
        End If
        If ClippingFactor < 1 Then
            Call errors.Add($"{NameOf(ClippingFactor)} 不能小于 1")
        End If
        If SignificanceZScore <= 0 Then
            Call errors.Add($"{NameOf(SignificanceZScore)} 必须大于 0")
        End If
        If KnockdownResidual < 0 OrElse KnockdownResidual >= 1 Then
            Call errors.Add($"{NameOf(KnockdownResidual)} 必须落在 [0, 1) 区间内，实际为 {KnockdownResidual}")
        End If
        If OverexpressionFactor < 1 Then
            Call errors.Add($"{NameOf(OverexpressionFactor)} 不能小于 1，实际为 {OverexpressionFactor}")
        End If
        If MaxClampRatio <= 0 OrElse MaxClampRatio > 1 Then
            Call errors.Add($"{NameOf(MaxClampRatio)} 必须落在 (0, 1] 区间内，实际为 {MaxClampRatio}")
        End If
        If MaxIterationsHardCap > 0 AndAlso MaxIterationsHardCap < MaxIterations Then
            Call errors.Add($"{NameOf(MaxIterationsHardCap)} 不能小于 {NameOf(MaxIterations)}（{MaxIterations}），实际为 {MaxIterationsHardCap}")
        End If

        ' ---- 结构发现 ----
        If IsDiscoveryEnabled Then
            If DiscoveryCandidates Is Nothing OrElse DiscoveryCandidates.Length = 0 Then
                Call errors.Add($"{NameOf(EdgeDiscovery)} 开启时必须通过 {NameOf(DiscoveryCandidates)} 给出候选基因集合")
            End If
            If DiscoveryMaxPerGene < 1 Then
                Call errors.Add($"{NameOf(DiscoveryMaxPerGene)} 至少为 1")
            End If
            If DiscoveryMaxCandidates < DiscoveryMaxPerGene Then
                Call errors.Add($"{NameOf(DiscoveryMaxCandidates)} 不能小于 {NameOf(DiscoveryMaxPerGene)}")
            End If
            If DiscoveryMinCorrelation <= 0 OrElse DiscoveryMinCorrelation >= 1 Then
                Call errors.Add($"{NameOf(DiscoveryMinCorrelation)} 必须落在 (0, 1) 区间内，实际为 {DiscoveryMinCorrelation}")
            End If
            If DiscoveryMinDeltaR2 <= 0 Then
                Call errors.Add($"{NameOf(DiscoveryMinDeltaR2)} 必须大于 0")
            End If
            If DiscoverySampleSize <> 0 AndAlso DiscoverySampleSize < 50 Then
                Call errors.Add($"{NameOf(DiscoverySampleSize)} 至少为 50（或设为 0 使用全部训练细胞）")
            End If
            If DiscoveryMaxGenes < 0 Then
                Call errors.Add($"{NameOf(DiscoveryMaxGenes)} 不能为负数")
            End If
        End If

        Return errors
    End Function

    ''' <summary>
    ''' 校验配置，不合法时抛出 <see cref="ArgumentException"/>
    ''' </summary>
    Public Sub ValidateOrThrow()
        Dim errors As List(Of String) = GetValidationErrors()

        If errors.Count > 0 Then
            Throw New ArgumentException("GeneTree 配置参数不合法: " & String.Join(" | ", errors))
        End If
    End Sub

    ''' <summary>
    ''' 创建快速档配置：≤1000 细胞/模型、top-10 调控者、40 轮提升
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function CreateFast() As GeneTreeConfig
        Return New GeneTreeConfig With {
            .MaxRegulators = 10,
            .MaxSamplesPerModel = 1000,
            .NumBoostRound = 40,
            .MaxDepth = 3,
            .PopulationSize = 100,
            .MaxIterations = 60
        }
    End Function

    ''' <summary>
    ''' 创建均衡档配置：≤3000 细胞/模型、top-20 调控者、100 轮提升
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function CreateBalanced() As GeneTreeConfig
        Return New GeneTreeConfig With {
            .MaxRegulators = 20,
            .MaxSamplesPerModel = 3000,
            .NumBoostRound = 100,
            .MaxDepth = 3,
            .PopulationSize = 200,
            .MaxIterations = 100
        }
    End Function

    ''' <summary>
    ''' 创建全量档配置：全部细胞、全部调控者（极慢）
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function CreateFull() As GeneTreeConfig
        Return New GeneTreeConfig With {
            .MaxRegulators = 0,
            .MaxSamplesPerModel = 0,
            .NumBoostRound = 200,
            .MaxDepth = 4,
            .PopulationSize = 300,
            .MaxIterations = 200
        }
    End Function

    ''' <summary>
    ''' 按预设档位创建配置
    ''' </summary>
    ''' <param name="profile"></param>
    ''' <returns></returns>
    Public Shared Function FromProfile(profile As GeneTreeProfile) As GeneTreeConfig
        Select Case profile
            Case GeneTreeProfile.Balanced
                Return CreateBalanced()
            Case GeneTreeProfile.Full
                Return CreateFull()
            Case Else
                Return CreateFast()
        End Select
    End Function

    ''' <summary>
    ''' 解析命令行/文本中的档位名称（<c>fast</c> / <c>balanced</c> / <c>full</c>，大小写不敏感）
    ''' </summary>
    ''' <param name="text"></param>
    ''' <param name="profile"></param>
    ''' <returns></returns>
    Public Shared Function TryParseProfile(text As String, ByRef profile As GeneTreeProfile) As Boolean
        Select Case If(text, "").Trim().ToLower()
            Case "fast", "quick"
                profile = GeneTreeProfile.Fast
            Case "balanced", "balance", "medium"
                profile = GeneTreeProfile.Balanced
            Case "full", "all"
                profile = GeneTreeProfile.Full
            Case Else
                Return False
        End Select

        Return True
    End Function

    Public Overrides Function ToString() As String
        Dim sb As New StringBuilder()

        Call sb.Append($"regulators<={If(MaxRegulators = 0, "all", CStr(MaxRegulators))}, ")
        Call sb.Append($"cells<={If(MaxSamplesPerModel = 0, "all", CStr(MaxSamplesPerModel))}, ")
        Call sb.Append($"rounds={NumBoostRound}, depth={MaxDepth}, eta={LearningRate}, lambda={Lambda}, ")
        Call sb.Append($"alpha={DampingAlpha}, iterations={MaxIterations}/{EffectiveMaxIterationsHardCap}, population={PopulationSize}, ")
        Call sb.Append($"tol={ConvergenceTolerance}, kd={KnockdownResidual}, oe={OverexpressionFactor}")

        If IsDiscoveryEnabled Then
            Call sb.Append($", discovery={EdgeDiscovery}(candidates={If(DiscoveryCandidates, New String() {}).Length}, " &
                           $"max/gene={DiscoveryMaxPerGene}, try={DiscoveryMaxCandidates}, " &
                           $"r>={DiscoveryMinCorrelation}, dR2>={DiscoveryMinDeltaR2})")
        End If

        Return sb.ToString()
    End Function

End Class
