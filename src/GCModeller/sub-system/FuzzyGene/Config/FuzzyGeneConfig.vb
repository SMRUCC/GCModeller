Imports Microsoft.VisualBasic.DeepLearning.ANFIS
Imports Microsoft.VisualBasic.DataMining.FuzzyLogic.TSK

Namespace Config

    ''' <summary>
    ''' 候选调控因子的筛选策略。
    ''' </summary>
    Public Enum RegulatorSelection
        ''' <summary>
        ''' 先验调控网络优先：先取先验边（按置信度降序），不足 <see cref="FuzzyGeneConfig.MaxRegulators"/> 个时
        ''' 用与靶基因相关性最强的基因补齐；没有先验时自动退化为 <see cref="CorrelationOnly"/>。
        ''' </summary>
        PriorFirst = 0
        ''' <summary>只使用表达数据的 |Pearson| 相关性排序，完全忽略先验网络。</summary>
        CorrelationOnly = 1
        ''' <summary>只使用先验调控网络的边，缺少先验时该靶基因会被跳过。</summary>
        PriorOnly = 2
    End Enum

    ''' <summary>
    ''' 拟合路线。
    ''' </summary>
    ''' <remarks>
    ''' 成员名不能是 <c>Static</c>：那是 VB 的保留关键字。
    ''' </remarks>
    Public Enum FuzzyTargetMode
        ''' <summary>按输入数据自动判定：唯一时间点数达到 <see cref="FuzzyGeneConfig.MinTimePoints"/> 时用滞后，否则静态。</summary>
        Auto = 0
        ''' <summary>静态路线：学习 <c>x_g = F(regulators)</c>。</summary>
        StaticRoute = 1
        ''' <summary>动态滞后路线：学习 <c>x_g(t+1) = F(regulators(t))</c>，需要带 <c>TimePoints</c> 的时序矩阵。</summary>
        LaggedRoute = 2
    End Enum

    ''' <summary>
    ''' 神经模糊基因调控网络（ANFIS/TSK）的建模配置。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 网络的**结构**参数（隶属函数类型与个数、规则启用掩膜、规模上限、分块行数）与
    ''' **训练**参数（学习率、优化器、早停、岭项……）全部继承自 <see cref="AnfisOptions"/>，
    ''' 这里只补充调控网络建模专属的选项：候选调控因子规模、筛选策略、拟合路线、
    ''' 稳态传播参数与虚拟扰动的默认行为。
    ''' </para>
    ''' <para>
    ''' 构造函数把 <see cref="TskStructureOptions.MaxRules"/> 与
    ''' <see cref="TskStructureOptions.MaxConsequentParams"/> 从 ANFIS 的通用默认值（1024/4096）
    ''' 下调到 128/512：GRN 是「逐靶基因一个小网络」的批量场景，512 个结论参数对应的
    ''' <c>A'A</c> 约 2 MB，上千个基因并行训练时内存才可控。确实需要更大子网时可显式调高。
    ''' </para>
    ''' <para>
    ''' VB 不支持协变返回类型，因此链式校验方法命名为 <see cref="ValidateGene"/>，
    ''' 克隆方法用 <c>Shadows</c> 隐藏基类的 <see cref="AnfisOptions.Clone"/> 并返回本类型。
    ''' </para>
    ''' </remarks>
    Public Class FuzzyGeneConfig
        Inherits AnfisOptions

        ''' <summary>
        ''' 每个靶基因保留的候选调控因子个数上限（建议 3–6）。
        ''' </summary>
        ''' <remarks>
        ''' 网格划分的规则数是 <c>m^n</c>，输入维数是规则爆炸的唯一来源，
        ''' 因此这个上限是控制模型复杂度的第一道闸门。
        ''' </remarks>
        Public Property MaxRegulators As Integer = 4

        ''' <summary>候选调控因子的筛选策略。</summary>
        Public Property Selection As RegulatorSelection = RegulatorSelection.PriorFirst

        ''' <summary>
        ''' 相关性补齐时采用的最小 |Pearson|；低于该值的基因不会被选为调控因子。
        ''' </summary>
        Public Property MinCorrelation As Double = 0.3

        ''' <summary>拟合路线（默认按数据自动判定）。</summary>
        Public Property TargetMode As FuzzyTargetMode = FuzzyTargetMode.Auto

        ''' <summary>稳态求解的最大迭代步数。</summary>
        Public Property PropagationSteps As Integer = 50

        ''' <summary>
        ''' 稳态迭代的阻尼系数（0, 1]：<c>z ← z + ω(F(z) − z)</c>。
        ''' </summary>
        ''' <remarks>
        ''' 逐靶基因的回归模型拼成的映射不一定收缩，全额迭代（ω = 1）在真实数据上可能振荡；
        ''' 0.5 是兼顾收敛速度与稳定性的常用取值。
        ''' </remarks>
        Public Property Relaxation As Double = 0.5

        ''' <summary>稳态判据：单步最大变化量小于该值即认为收敛。</summary>
        Public Property ConvergenceTolerance As Double = 0.000001

        ''' <summary>
        ''' 扰动效应的显著性阈值（Z 分数）：<c>|Δ / SD|</c> 超过该值的基因标记为显著变化。
        ''' </summary>
        Public Property SignificanceZScore As Double = 2.0

        ''' <summary>
        ''' 虚拟敲低的残余比例：敲低后目标基因保留 <c>KnockdownResidual × 基线表达</c>。
        ''' </summary>
        Public Property KnockdownResidual As Double = 0.2

        ''' <summary>虚拟过表达的提升幅度（以训练期标准差为单位）：<c>mean + nσ</c>。</summary>
        Public Property OverexpressionSigmas As Double = 3.0

        ''' <summary>
        ''' 规则剪枝阈值：训练集上平均归一化触发强度低于该值的规则被移除；0 表示不剪枝。
        ''' </summary>
        Public Property PruneRuleThreshold As Double = 0.01

        ''' <summary>训练前是否对表达量做逐基因 z-score 标准化。</summary>
        Public Property Standardize As Boolean = True

        ''' <summary>
        ''' 是否把传播过程中的取值限制在训练支持域内（<c>[min−1, max+1]</c>，z 空间）。
        ''' </summary>
        ''' <remarks>
        ''' 虚拟扰动是分布外外推，高斯隶属函数在支持域外会饱和、结论的局部线性模型会给出
        ''' 失控斜率；限幅是防御性措施，代价是极大扰动下的非线性响应被压平。
        ''' </remarks>
        Public Property ClampToSupport As Boolean = True

        ''' <summary>逐基因训练的并行度；0 表示自动（使用全部逻辑核心），1 表示串行。</summary>
        Public Property DegreeOfParallelism As Integer = 0

        ''' <summary>建模的最大基因数（按方差从高到低保留）；0 表示全部基因。</summary>
        ''' <remarks>
        ''' 相关性补齐是对全部基因的线性扫描（<c>O(n²·m)</c>），全基因组规模下需要先做高变异过滤，
        ''' 或者先用先验/差异分析把范围缩小到模块尺度。
        ''' </remarks>
        Public Property MaxGenes As Integer = 2000

        ''' <summary>判定为时序数据所需的最少唯一时间点数（滞后模式）。</summary>
        Public Property MinTimePoints As Integer = 3

        ''' <summary>训练一个靶基因模型所需的最少样本（状态）行数。</summary>
        Public Property MinSamples As Integer = 4

        ''' <summary>导出调控边时的最小 |权重|，低于该值的边被视为噪声丢弃。</summary>
        Public Property MinEdgeWeight As Double = 0.0001

        ''' <summary>
        ''' 参与训练的最大行数；0 表示使用全部行。
        ''' </summary>
        ''' <remarks>
        ''' <para>
        ''' 训练成本里压倒性的一项是「每个 epoch 重解一次结论参数的正规方程」，量级为
        ''' <c>行数 × P²</c>（P = 规则数 ×(输入数+1)）。几万行样本对几百个结论参数是严重过定的，
        ''' 把行数压到 3000–5000 几乎不影响 R²，却能把训练时间同比例砍下来。
        ''' </para>
        ''' <para>
        ''' 抽样是**等间隔确定性**的（不是随机抽样）：结果可复现，时序数据下也仍覆盖整段轨迹。
        ''' 同时它直接作用在预处理阶段——只把采样到的行写进列里，常驻内存也同比例下降。
        ''' </para>
        ''' </remarks>
        Public Property MaxTrainRows As Integer = 0

        ''' <summary>
        ''' 相关性候选池的规模上限；0 表示不限制（对全部基因计算相关性）。
        ''' </summary>
        ''' <remarks>
        ''' 相关性补齐的复杂度是 <c>O(基因数² × 行数)</c>，全基因组规模下这一项会与训练本身同量级。
        ''' 给定时候选池为「先验调控网络的 TF ∪ 方差最高的若干个基因」，低方差基因本来也主要是噪声。
        ''' 建议全基因组规模取 2000–4000。
        ''' </remarks>
        Public Property MaxCandidateGenes As Integer = 0

        ''' <summary>
        ''' 靶基因有效性门槛：先验无入边且与全部候选的最大 |Pearson| 低于该值时跳过该靶基因；
        ''' 0 表示不跳过（默认）。
        ''' </summary>
        ''' <remarks>
        ''' 这类靶基因的模型 R² 基本为 0，纯属浪费算力；全基因组规模下通常占 30–50%。
        ''' 只有在确认要「只为可解释的靶基因建模」时才建议打开（例如 0.05–0.1）。
        ''' </remarks>
        Public Property MinBestCorrelation As Double = 0.0

        ''' <summary>默认配置（同时把规则预算下调到 GRN 场景的合理量级）。</summary>
        Public Sub New()
            MyBase.MaxRules = 128
            MyBase.MaxConsequentParams = 512
        End Sub

        ''' <summary>逐属性校验并返回自身。</summary>
        ''' <remarks>
        ''' 名称不能是 <c>Validate</c>：基类已有同签名、返回 <see cref="AnfisOptions"/> 的方法，
        ''' VB 不允许仅靠返回类型区分的重载。
        ''' </remarks>
        Public Function ValidateGene() As FuzzyGeneConfig
            Call MyBase.Validate()

            If MaxRegulators < 1 Then
                Throw New ArgumentOutOfRangeException(NameOf(MaxRegulators), MaxRegulators, "the regulator count must be positive")
            End If
            If MinCorrelation < 0.0 OrElse MinCorrelation > 1.0 Then
                Throw New ArgumentOutOfRangeException(NameOf(MinCorrelation), MinCorrelation, "the correlation threshold must be in [0, 1]")
            End If
            If PropagationSteps < 1 Then
                Throw New ArgumentOutOfRangeException(NameOf(PropagationSteps), PropagationSteps, "the propagation step count must be positive")
            End If
            If Relaxation <= 0.0 OrElse Relaxation > 1.0 Then
                Throw New ArgumentOutOfRangeException(NameOf(Relaxation), Relaxation, "the relaxation factor must be in (0, 1]")
            End If
            If ConvergenceTolerance <= 0.0 Then
                Throw New ArgumentOutOfRangeException(NameOf(ConvergenceTolerance), ConvergenceTolerance, "the tolerance must be positive")
            End If
            If SignificanceZScore <= 0.0 Then
                Throw New ArgumentOutOfRangeException(NameOf(SignificanceZScore), SignificanceZScore, "the significance z-score must be positive")
            End If
            If KnockdownResidual < 0.0 OrElse KnockdownResidual >= 1.0 Then
                Throw New ArgumentOutOfRangeException(NameOf(KnockdownResidual), KnockdownResidual, "the knockdown residual must be in [0, 1)")
            End If
            If OverexpressionSigmas < 0.0 Then
                Throw New ArgumentOutOfRangeException(NameOf(OverexpressionSigmas), OverexpressionSigmas, "the overexpression fold must not be negative")
            End If
            If PruneRuleThreshold < 0.0 OrElse PruneRuleThreshold >= 1.0 Then
                Throw New ArgumentOutOfRangeException(NameOf(PruneRuleThreshold), PruneRuleThreshold, "the pruning threshold must be in [0, 1)")
            End If
            If DegreeOfParallelism < 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(DegreeOfParallelism), DegreeOfParallelism, "the parallelism degree must not be negative")
            End If
            If MaxGenes < 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(MaxGenes), MaxGenes, "the gene limit must not be negative")
            End If
            If MinTimePoints < 2 Then
                Throw New ArgumentOutOfRangeException(NameOf(MinTimePoints), MinTimePoints, "at least 2 time points are required for the lagged mode")
            End If
            If MinSamples < 2 Then
                Throw New ArgumentOutOfRangeException(NameOf(MinSamples), MinSamples, "at least 2 samples are required to fit a target gene")
            End If
            If MinEdgeWeight < 0.0 Then
                Throw New ArgumentOutOfRangeException(NameOf(MinEdgeWeight), MinEdgeWeight, "the edge weight threshold must not be negative")
            End If
            If MaxTrainRows < 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(MaxTrainRows), MaxTrainRows, "the training row limit must not be negative")
            End If
            If MaxCandidateGenes < 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(MaxCandidateGenes), MaxCandidateGenes, "the candidate gene limit must not be negative")
            End If
            If MinBestCorrelation < 0.0 OrElse MinBestCorrelation > 1.0 Then
                Throw New ArgumentOutOfRangeException(NameOf(MinBestCorrelation), MinBestCorrelation, "the best correlation threshold must be in [0, 1]")
            End If

            Return Me
        End Function

        ''' <summary>浅拷贝（数组字段按基类的规则单独复制）。</summary>
        Public Shadows Function Clone() As FuzzyGeneConfig
            Dim copy As FuzzyGeneConfig = DirectCast(Me.MemberwiseClone(), FuzzyGeneConfig)

            Call CopyStructureArraysTo(copy)

            Return copy
        End Function

        ''' <summary>单行摘要（用于日志与训练汇总）。</summary>
        Public Overrides Function ToString() As String
            Return $"{Selection}/{TargetMode}, K={MaxRegulators}, mf={MfKind}x{MfCount}, " &
                   $"rows<={If(MaxTrainRows <= 0, "all", MaxTrainRows.ToString)}, " &
                   $"pool<={If(MaxCandidateGenes <= 0, "all", MaxCandidateGenes.ToString)}, " &
                   $"{MyBase.ToString()}, steps={PropagationSteps}, omega={Relaxation:G3}"
        End Function

    End Class

End Namespace
