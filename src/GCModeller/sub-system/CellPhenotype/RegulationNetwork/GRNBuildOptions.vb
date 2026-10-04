Imports SMRUCC.genomics.Analysis.HTS.WGCNA

Namespace RegulationNetwork

    ''' <summary>
    ''' 基因表达调控先验网络（PriorNetwork）构建流水线的配置对象
    ''' </summary>
    ''' <remarks>
    ''' 所有字段均带有推荐默认值，因此无参构造即可跑通完整流程；需要调参时只修改关心的字段。
    '''
    ''' 流水线依次为：批次标准化 → 基因过滤 → WGCNA 模块划分 → 模块内 bicor 相关 →
    ''' p 值 / BH FDR 过滤 → ARACNe DPI 去间接 → 偏相关筛选 → TF 定向 → STRING 补全/加权 → 跨模块边。
    ''' </remarks>
    Public Class GRNBuildOptions

        ' ==================== 数据预处理 ====================

        ''' <summary>
        ''' 数据集分组（批次信息）：数据集名称 → 该数据集包含的 sampleID 列表。
        ''' 为 Nothing 或空时视为单一批次，只做全局 z-score 标准化。
        ''' </summary>
        ''' <returns></returns>
        Public Property batches As Dictionary(Of String, String()) = Nothing

        ''' <summary>
        ''' 是否在组内 z-score 之后再减去批次内的基因均值（批次中心化），用于削弱多数据集合并带来的批次效应。
        ''' </summary>
        ''' <returns>默认 True</returns>
        Public Property centerBatch As Boolean = True

        ''' <summary>
        ''' 允许的基因缺失（NaN）比例上限，超过则剔除该基因。
        ''' </summary>
        ''' <returns>默认 0.2；0 表示不做缺失过滤</returns>
        Public Property maxMissingRate As Double = 0.2

        ''' <summary>
        ''' 只保留方差最大的前 N 个基因（方差过滤，R WGCNA 流程的常规做法）。
        ''' </summary>
        ''' <returns>默认 8000；0 表示不过滤</returns>
        Public Property filterTopN As Integer = 8000

        ''' <summary>
        ''' 基因方差下限，低于该值的基因被剔除。
        ''' </summary>
        ''' <returns>默认 0（不启用）</returns>
        Public Property minVariance As Double = 0

        ' ==================== WGCNA 模块划分 ====================

        ''' <summary>
        ''' WGCNA 分析配置；为 Nothing 时使用 <see cref="WGCNAConfig"/> 默认值，
        ''' 并强制 <see cref="WGCNAConfig.buildGraph"/> = False（大规模数据下物化 n^2 条边必然 OOM）。
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 注意：基因过滤由本流水线统一执行，因此配置中的
        ''' <see cref="WGCNAConfig.filterTopN"/> / <see cref="WGCNAConfig.minVariance"/> /
        ''' <see cref="WGCNAConfig.maxMissingRate"/> 会被重置为 0，避免重复过滤导致基因集不一致。
        ''' </remarks>
        Public Property wgcnaConfig As WGCNAConfig = Nothing

        ''' <summary>
        ''' 是否使用分块模式（<c>Analysis.RunBlockwise</c>）做模块划分。基因数超过数千时应保持 True。
        ''' </summary>
        ''' <returns>默认 True</returns>
        Public Property runBlockwise As Boolean = True

        ' ==================== 相关性 ====================

        ''' <summary>
        ''' 模块内相关是否使用稳健相关 bicor（biweight midcorrelation）；关闭时使用经典 Pearson。
        ''' </summary>
        ''' <returns>默认 True</returns>
        Public Property useBicor As Boolean = True

        ''' <summary>
        ''' bicor 的 Tukey 调节常数 c。
        ''' </summary>
        ''' <returns>默认 9（WGCNA 默认值）</returns>
        Public Property bicorConstant As Double = Bicor.DefaultConstant

        ''' <summary>
        ''' bicor 在 MAD = 0（近常数向量）时是否回退到 Pearson。
        ''' </summary>
        ''' <returns>默认 True</returns>
        Public Property pearsonFallback As Boolean = True

        ''' <summary>
        ''' 候选边的最小相关绝对值，低于该值的基因对不进入后续计算。
        ''' </summary>
        ''' <returns>默认 0.3</returns>
        Public Property minAbsCorrelation As Double = 0.3

        ''' <summary>
        ''' BH 校正后 FDR（q 值）的过滤阈值。
        ''' </summary>
        ''' <returns>默认 0.05</returns>
        Public Property fdrThreshold As Double = 0.05

        ' ==================== ARACNe DPI 去间接 ====================

        ''' <summary>
        ''' 是否启用 DPI 三元组过滤（去除由共同上游调控造成的间接边）。
        ''' </summary>
        ''' <returns>默认 True</returns>
        Public Property dpiEnabled As Boolean = True

        ''' <summary>
        ''' 参与 DPI 的最小的相关绝对值。该值高于 <see cref="minAbsCorrelation"/> 可以使
        ''' DPI 的邻接矩阵保持稀疏（DPI 是 O(n^3) 的三角形枚举，稠密邻接时代价极高）。
        ''' 位于 [<see cref="minAbsCorrelation"/>, 本值) 之间的弱边不参与 DPI，直接保留。
        ''' </summary>
        ''' <returns>默认 0.5</returns>
        Public Property dpiMinAbsCorrelation As Double = 0.5

        ''' <summary>
        ''' ARACNe 的 ε 容差：最弱边必须比另外两条边低超过该比例才被判定为间接边，接近平局时保留。
        ''' </summary>
        ''' <returns>默认 0.1</returns>
        Public Property dpiTolerance As Double = 0.1

        ''' <summary>
        ''' 单模块内参与 DPI 的最大基因数，超出时只取 hub 基因（按 kME 排序）。
        ''' </summary>
        ''' <returns>默认 500</returns>
        Public Property maxDpiGenes As Integer = 500

        ' ==================== 偏相关筛选 ====================

        ''' <summary>
        ''' 是否启用偏相关二次筛选（控制模块内高相关基因后的残差相关）。
        ''' </summary>
        ''' <returns>默认 True</returns>
        Public Property partialEnabled As Boolean = True

        ''' <summary>
        ''' 偏相关条件集大小：取模块内与该边两端相关最高的 K 个基因作为条件变量。
        ''' </summary>
        ''' <returns>默认 5</returns>
        Public Property partialConditionTopK As Integer = 5

        ''' <summary>
        ''' 偏相关保留比例下限：|pcor| / |cor| 低于该值（或符号发生翻转）时判定为间接关系并剔除该边。
        ''' </summary>
        ''' <returns>默认 0.5</returns>
        Public Property partialMinRetainRatio As Double = 0.5

        ''' <summary>
        ''' 偏相关衰减阈值：|pcor| / |cor| 低于该值但高于 <see cref="partialMinRetainRatio"/> 时保留该边，
        ''' 但置信度打折并标记 <c>partial:attenuated</c>。
        ''' </summary>
        ''' <returns>默认 0.8</returns>
        Public Property partialAttenuatedRatio As Double = 0.8

        ''' <summary>
        ''' 偏相关残差之间是否使用 bicor 而非 Pearson。
        ''' </summary>
        ''' <returns>默认 False</returns>
        Public Property partialUseRobust As Boolean = False

        ''' <summary>
        ''' 单模块内参与偏相关计算的候选边数量上限（按 |cor| 降序截断），控制 OLS 的总开销。
        ''' </summary>
        ''' <returns>默认 50000</returns>
        Public Property maxPartialEdgesPerModule As Integer = 50000

        ' ==================== TF 定向 ====================

        ''' <summary>
        ''' 是否保留无法由 TF 注释定向的候选边（TF-TF 与非 TF-非 TF）。
        ''' 保留时这些边会打折并标记 <c>undirected</c> 证据，供下游 DBN / GNN 作为软先验裁决；
        ''' 关闭时行为与既有的 <c>GeneRegulatoryNetwork.BuildPriorNetwork</c> 一致（直接丢弃）。
        ''' </summary>
        ''' <returns>默认 True</returns>
        Public Property keepUndirectedCandidates As Boolean = True

        ''' <summary>
        ''' 无向候选边的置信度折扣系数。
        ''' </summary>
        ''' <returns>默认 0.5</returns>
        Public Property undirectedConfidenceScale As Double = 0.5

        ' ==================== STRING 蛋白互作 ====================

        ''' <summary>
        ''' STRING 的 <c>9606.protein.links.vXX.txt</c> 文件路径；为 Nothing 时不启用 STRING 证据。
        ''' </summary>
        ''' <returns></returns>
        Public Property stringLinks As String = Nothing

        ''' <summary>
        ''' 基因 ID → STRING protein id 的映射表（可由 <see cref="Tabular.Tsv.entrez_gene_id_vs_string.BuildMapsFromFile"/> 构造）。
        ''' 为 Nothing 时尝试用表达矩阵的基因 ID 与 STRING 的 protein id 直接匹配。
        ''' </summary>
        ''' <returns></returns>
        Public Property stringIdMap As Dictionary(Of String, String) = Nothing

        ''' <summary>
        ''' STRING combined_score 的最低阈值（0-1000）。
        ''' </summary>
        ''' <returns>默认 700（high confidence）</returns>
        Public Property stringMinScore As Double = 700

        ''' <summary>
        ''' 是否新增共表达网络中缺失、但 STRING 中存在的 PPI 边（拓扑补全）。
        ''' </summary>
        ''' <returns>默认 True</returns>
        Public Property stringAddEdges As Boolean = True

        ''' <summary>
        ''' STRING 证据在置信度融合中的权重 w：conf = (1-w)·conf_expr + w·score_norm。
        ''' </summary>
        ''' <returns>默认 0.3；为 0 时退化为只打证据标记、不改变置信度</returns>
        Public Property stringWeight As Double = 0.3

        ' ==================== 规模控制 ====================

        ''' <summary>
        ''' 单模块参与 bicor / DPI / 偏相关的最大基因数，超出时按 kME（模块成员资格）取 hub 子集。
        ''' </summary>
        ''' <returns>默认 1000</returns>
        Public Property maxModuleGenes As Integer = 1000

        ' ==================== 跨模块边 ====================

        ''' <summary>
        ''' 是否构建跨模块调控边。
        ''' </summary>
        ''' <returns>默认 True</returns>
        Public Property crossModuleEnabled As Boolean = True

        ''' <summary>
        ''' 模块特征基因（eigengene）相关阈值：|cor| 超过该值的模块对之间才计算跨模块边。
        ''' </summary>
        ''' <returns>默认 0.3</returns>
        Public Property crossModuleCorThreshold As Double = 0.3

        ''' <summary>
        ''' 参与跨模块计算的模块对数量上限（按 eigengene 相关绝对值降序）。
        ''' </summary>
        ''' <returns>默认 20</returns>
        Public Property maxCrossModulePairs As Integer = 20

        ''' <summary>
        ''' 每个模块参与跨模块计算的 TF 数量上限（按 kME 降序）。
        ''' </summary>
        ''' <returns>默认 30</returns>
        Public Property maxCrossModuleTfs As Integer = 30

        ''' <summary>
        ''' 每个模块参与跨模块计算的靶基因数量上限（按 kME 降序）。
        ''' 跨模块相关为逐对计算（O(TF 数 × 靶基因数)），该值控制总开销。
        ''' </summary>
        ''' <returns>默认 300</returns>
        Public Property maxCrossModuleTargets As Integer = 300

        ''' <summary>
        ''' 跨模块边的置信度折扣系数（跨模块相关的可靠性低于模块内共表达）。
        ''' </summary>
        ''' <returns>默认 0.8</returns>
        Public Property crossModuleConfidenceScale As Double = 0.8

        ' ==================== 其他 ====================

        ''' <summary>
        ''' 是否输出各阶段进度日志。
        ''' </summary>
        ''' <returns>默认 True</returns>
        Public Property verbose As Boolean = True

    End Class
End Namespace
