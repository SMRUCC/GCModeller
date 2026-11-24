Namespace Config

    ''' <summary>
    ''' GPS（GNN + Transformer）基因表达调控网络模型的超参数配置。
    ''' </summary>
    ''' <remarks>
    ''' 默认取值对应 method.md §4.1 的 CONFIG 表，并针对"先验网络覆盖率低、时间点少"的
    ''' 实际数据条件做了保守调整：
    ''' <list type="bullet">
    ''' <item><description>隐藏维度取 128、融合层取 3，避免在只有十几个滑窗样本时过拟合；</description></item>
    ''' <item><description>最短路径距离截断为 5，超过该距离的基因对与不连通基因对共用专用桶；</description></item>
    ''' <item><description>基因规模第一期限制在 2000 以内（<see cref="MaxGenes"/>），
    ''' 因为全局注意力的时间复杂度为 O(N²)。</description></item>
    ''' </list>
    ''' </remarks>
    Public Class GPSConfig

        ' ==================== 数据规模 ====================

        ''' <summary>
        ''' 建模基因数上限；0 表示不做高变异过滤、使用全部基因。
        ''' </summary>
        ''' <returns>默认 2000</returns>
        ''' <remarks>
        ''' 超过该值时按跨样本方差取前 N 个高变异基因，并额外保留先验网络中的转录因子，
        ''' 以免过滤掉全部有调控边的节点。
        ''' </remarks>
        Public Property MaxGenes As Integer = 2000

        ''' <summary>是否按时间点（列）做 CPM 文库归一化</summary>
        ''' <returns>默认 True</returns>
        Public Property CpmNormalize As Boolean = True

        ''' <summary>是否做 log2(x + 1) 方差稳定变换</summary>
        ''' <returns>默认 True</returns>
        Public Property LogTransform As Boolean = True

        ''' <summary>是否做逐基因 z-score 标准化（跨时间点）</summary>
        ''' <returns>默认 True</returns>
        Public Property ZScore As Boolean = True

        ' ==================== 图与图编码 ====================

        ''' <summary>拉普拉斯位置编码维度（取前 K 个非平凡特征向量）</summary>
        ''' <returns>默认 16</returns>
        Public Property PeDim As Integer = 16

        ''' <summary>最短路径距离的最大编码距离，超过该值的基因对归入"远距离"桶</summary>
        ''' <returns>默认 5；桶数 = 该值 + 2（另有"不连通"专用桶）</returns>
        Public Property SpdMaxDist As Integer = 5

        ''' <summary>
        ''' 是否为每个基因追加 top-K 共表达边（GEARS 双通道设计中的共表达协方差图）
        ''' </summary>
        ''' <returns>默认 0，即只用先验调控边</returns>
        Public Property CoexpressionTopK As Integer = 0

        ''' <summary>共表达边的最小 Pearson 相关系数</summary>
        ''' <returns>默认 0.7</returns>
        Public Property MinCoexpression As Double = 0.7

        ' ==================== 注意力规模（稠密 / 稀疏分派） ====================

        ''' <summary>
        ''' 稠密注意力的基因数上限；超过该值时自动改用稀疏候选注意力。
        ''' </summary>
        ''' <returns>默认 1024</returns>
        ''' <remarks>
        ''' 稠密路径的分数矩阵与权重矩阵均为 <c>[N, N]</c>：N=1024 时单头约 8 MB，
        ''' 4 头 3 层（含反向缓存）约 100 MB；N 再大就不可接受。
        ''' 全基因组规模（N≈6 万）必须走稀疏路径。
        ''' </remarks>
        Public Property DenseAttentionMaxNodes As Integer = 1024

        ''' <summary>
        ''' 稀疏模式下每个基因的候选预算（含自身）。
        ''' </summary>
        ''' <returns>默认 32</returns>
        ''' <remarks>
        ''' 候选由"自身 + 图邻域（预算截断 BFS）+ 高连接枢纽 + 随机长程链接"组成，
        ''' 注意力复杂度为 <c>O(N·c·d)</c>；<c>c</c> 越大越接近稠密注意力，但代价线性增长。
        ''' </remarks>
        Public Property AttentionNeighbors As Integer = 32

        ''' <summary>
        ''' 稀疏模式下每个基因期望的随机长程链接数（用于跨连通分量的全局混合）。
        ''' </summary>
        ''' <returns>默认 8</returns>
        ''' <remarks>
        ''' 当图邻域不足预算时，剩余槽位会并入随机链接；因此空图（无先验网络）下
        ''' 每个基因仍会获得 <c>c − 1</c> 个确定性随机候选，注意力不会退化为恒等。
        ''' </remarks>
        Public Property AttentionRandomLinks As Integer = 8

        ''' <summary>
        ''' 稀疏模式下作为全局枢纽纳入候选集的高连接基因数（按入度 + 出度排序）。
        ''' </summary>
        ''' <returns>默认 8</returns>
        ''' <remarks>没有枢纽基因的连通分量通过随机链接触达全图。</remarks>
        Public Property AttentionHubGenes As Integer = 8

        ' ==================== 融合主干 ====================

        ''' <summary>隐藏维度 d_model</summary>
        ''' <returns>默认 128</returns>
        Public Property DModel As Integer = 128

        ''' <summary>GPS 融合层数</summary>
        ''' <returns>默认 3</returns>
        Public Property NLayers As Integer = 3

        ''' <summary>注意力头数；必须能整除 <see cref="DModel"/></summary>
        ''' <returns>默认 8</returns>
        Public Property NHeads As Integer = 8

        ''' <summary>前馈网络中间维度</summary>
        ''' <returns>默认 256</returns>
        Public Property FfnDim As Integer = 256

        ''' <summary>Dropout 比例</summary>
        ''' <returns>默认 0.1</returns>
        Public Property Dropout As Double = 0.1

        ''' <summary>基因身份嵌入维度</summary>
        ''' <returns>默认 32</returns>
        Public Property GeneEmbedDim As Integer = 32

        ''' <summary>是否启用 Graphormer 空间偏置（消融开关）</summary>
        ''' <returns>默认 True</returns>
        Public Property UseSpdBias As Boolean = True

        ''' <summary>是否启用全局注意力支路（消融开关：False 时退化为纯 GNN）</summary>
        ''' <returns>默认 True</returns>
        Public Property UseGlobalAttention As Boolean = True

        ''' <summary>是否启用局部消息传递支路（消融开关：False 时退化为纯 Transformer）</summary>
        ''' <returns>默认 True</returns>
        Public Property UseLocalMpnn As Boolean = True

        ' ==================== 时序与解码 ====================

        ''' <summary>输入时间窗长度 w</summary>
        ''' <returns>默认 5</returns>
        Public Property Window As Integer = 5

        ''' <summary>时序编码器（GRU）隐层维度</summary>
        ''' <returns>默认 128</returns>
        Public Property GruHidden As Integer = 128

        ''' <summary>是否输出不确定性估计（softplus 方差头）</summary>
        ''' <returns>默认 False；开启后损失改用高斯负对数似然</returns>
        Public Property UseUncertainty As Boolean = False

        ' ==================== 训练 ====================

        ''' <summary>预训练（掩码重建）轮数</summary>
        ''' <returns>默认 30</returns>
        Public Property PretrainEpochs As Integer = 30

        ''' <summary>微调（下一时刻预测）轮数</summary>
        ''' <returns>默认 100</returns>
        Public Property FinetuneEpochs As Integer = 100

        ''' <summary>Adam 学习率</summary>
        ''' <returns>默认 0.0001</returns>
        Public Property LearningRate As Double = 0.0001

        ''' <summary>autofocus 损失的指数 gamma（总指数 = 2 + gamma）</summary>
        ''' <returns>默认 1.0</returns>
        Public Property AutofocusGamma As Double = 1.0

        ''' <summary>方向感知损失权重</summary>
        ''' <returns>默认 0.5</returns>
        Public Property LambdaDir As Double = 0.5

        ''' <summary>稀疏正则（L1）权重</summary>
        ''' <returns>默认 0.0001</returns>
        Public Property LambdaL1 As Double = 0.0001

        ''' <summary>先验距离正则权重（当前版本 A_prior 固定，故默认 0）</summary>
        ''' <returns>默认 0</returns>
        Public Property LambdaPrior As Double = 0.0

        ''' <summary>梯度裁剪的全局 L2 范数上限</summary>
        ''' <returns>默认 1.0</returns>
        Public Property GradientClip As Double = 1.0

        ''' <summary>早停耐心值（连续多少个 epoch 验证集未改善即停止）；0 表示不早停</summary>
        ''' <returns>默认 15</returns>
        Public Property EarlyStopPatience As Integer = 15

        ''' <summary>预训练阶段的基因掩码比例</summary>
        ''' <returns>默认 0.15</returns>
        Public Property MaskRatio As Double = 0.15

        ''' <summary>训练集时间点占比（按时间顺序切分）</summary>
        ''' <returns>默认 0.7</returns>
        Public Property TrainRatio As Double = 0.7

        ''' <summary>验证集时间点占比（按时间顺序切分）</summary>
        ''' <returns>默认 0.15</returns>
        Public Property ValidationRatio As Double = 0.15

        ''' <summary>随机种子；给定后初始化、dropout 与掩码均可复现</summary>
        ''' <returns>默认 2024</returns>
        Public Property Seed As Integer = 2024

        ''' <summary>
        ''' checkpoint 间隔（单位：epoch）；0 表示关闭周期性 checkpoint。
        ''' </summary>
        ''' <returns>默认 0（关闭）。间隔在 epoch 边界生效，粒度不可能更细</returns>
        ''' <remarks>
        ''' 开启后训练循环每到间隔 epoch 就把模型包（含权重、Adam 动量、
        ''' 训练步计数、早停状态与损失曲线）原子写入
        ''' <see cref="GPSModel.CheckpointPath"/>；进程意外中止后可从中断点精确续训。
        ''' </remarks>
        Public Property CheckpointIntervalEpochs As Integer = 0

        ' ==================== 推理与扰动 ====================

        ''' <summary>显著性判据：|Z-score| 大于该阈值才标记为显著变化</summary>
        ''' <returns>默认 0.5，与 BNLearn 现有干预分析口径一致</returns>
        Public Property SignificanceZScore As Double = 0.5

        ''' <summary>多步级联模拟的默认步数</summary>
        ''' <returns>默认 5</returns>
        Public Property CascadeSteps As Integer = 5

        ''' <summary>
        ''' 虚拟敲低（CRISPRi）后目标基因保留的表达比例。
        ''' </summary>
        ''' <returns>默认 0.2，即敲低后保留 20% 表达（对应赛事描述的 &gt;80% on-target knockdown）</returns>
        ''' <remarks>
        ''' 干预值按 <c>基因均值 × KnockdownResidual</c> 计算。之所以不使用
        ''' <c>均值 − 2σ</c>：后者对大多数基因为负，非负钳位后恒为 0，
        ''' 使敲低与敲除完全等价，敲低实验因此失去意义。
        ''' </remarks>
        Public Property KnockdownResidual As Double = 0.2

        ''' <summary>
        ''' 图卷积的边权重归一化方式：<c>"mean"</c> / <c>"sqrt"</c> / <c>"sum"</c>。
        ''' </summary>
        ''' <returns>默认 <c>"sqrt"</c></returns>
        ''' <remarks>
        ''' <para>
        ''' <c>mean</c> 是 GCN 的行归一化（GEARS 与旧模型的行为）：每跳聚合为凸组合平均，
        ''' 单点扰动经过 3 层后幅度只剩约 <c>1e-4</c>，在全基因组预测里表现为
        ''' "除被敲低基因外几乎没有变化"。
        ''' </para>
        ''' <para>
        ''' <c>sqrt</c> 改为平方根归一化，在入度中位数 22 ~ 均值 70 的真实先验图上
        ''' 每跳可放大约 5~8 倍；<c>sum</c> 完全不归一化，幅度最强但容易饱和。
        ''' 这里用字符串而不是枚举，是因为 config.json 的反射序列化只支持
        ''' Integer/Double/Boolean/String 四种类型。
        ''' </para>
        ''' </remarks>
        Public Property EdgeNormalization As String = "sqrt"

        ''' <summary>
        ''' 是否启用扰动通道：把被扰动基因的输入变化沿先验调控边一阶传播到它的靶基因。
        ''' </summary>
        ''' <returns>默认 True</returns>
        ''' <remarks>
        ''' 没有这条通道时，扰动只能靠 3 层消息传递间接传播，而先验图里每个靶基因
        ''' 从上千条入边中只分到 <c>1/入度</c> 的权重；另有 4,062 个基因根本没有入边。
        ''' 结果是单基因扰动的下游响应被压到 1e-4 甚至精确 0。通道把"该基因的 z 变化"
        ''' 按出边权重与调控方向（激活/抑制）直接注入靶基因的输入窗口。
        ''' </remarks>
        Public Property PerturbationChannel As Boolean = True

        ''' <summary>
        ''' 扰动通道的注入增益：注入量 = 增益 × 出边归一化权重 × 被扰动基因的 z 变化。
        ''' </summary>
        ''' <returns>默认 1.0；0 等价于关闭通道</returns>
        Public Property PerturbationChannelGain As Double = 1.0

        ''' <summary>
        ''' 被扰动基因没有出边（非 TF 基因）时，是否退化到"共调控"先验继续传播。
        ''' </summary>
        ''' <returns>默认 False</returns>
        ''' <remarks>
        ''' 先验图只有 TF → target 的边：本任务 300 个扰动里有 232 个基因不是 TF，
        ''' 一阶通道对它们本来是空操作。开启后按"共享上游 TF"把变化摊给共调控基因，
        ''' 属于启发式（方向不代表真实因果），建议先做对照实验再决定生产配置。
        ''' </remarks>
        Public Property PerturbationChannelCoRegulation As Boolean = False

        ''' <summary>
        ''' 校验参数组合是否合法。
        ''' </summary>
        ''' <returns>合法返回 True；否则返回 False</returns>
        Public Function Validate() As Boolean
            If MaxGenes < 0 Then Return False
            If PeDim < 0 Then Return False
            If SpdMaxDist < 1 Then Return False
            If DModel <= 0 Then Return False
            If NHeads <= 0 OrElse DModel Mod NHeads <> 0 Then Return False
            If NLayers < 1 OrElse NLayers > 12 Then Return False
            If FfnDim <= 0 Then Return False
            If Dropout < 0.0 OrElse Dropout >= 1.0 Then Return False
            If GeneEmbedDim <= 0 Then Return False
            If Window < 2 Then Return False
            If GruHidden <= 0 Then Return False
            If PretrainEpochs < 0 OrElse FinetuneEpochs < 0 Then Return False
            If LearningRate <= 0.0 Then Return False
            If AutofocusGamma < 0.0 Then Return False
            If LambdaDir < 0.0 OrElse LambdaL1 < 0.0 OrElse LambdaPrior < 0.0 Then Return False
            If GradientClip < 0.0 Then Return False
            If MaskRatio < 0.0 OrElse MaskRatio >= 1.0 Then Return False
            If TrainRatio <= 0.0 OrElse TrainRatio >= 1.0 Then Return False
            If ValidationRatio < 0.0 OrElse ValidationRatio >= 1.0 Then Return False
            If TrainRatio + ValidationRatio >= 1.0 Then Return False
            If CoexpressionTopK < 0 Then Return False
            If MinCoexpression < -1.0 OrElse MinCoexpression > 1.0 Then Return False
            If KnockdownResidual < 0.0 OrElse KnockdownResidual >= 1.0 Then Return False
            If Not IsKnownEdgeNormalization(EdgeNormalization) Then Return False
            If PerturbationChannelGain < 0.0 Then Return False
            If CheckpointIntervalEpochs < 0 Then Return False
            If DenseAttentionMaxNodes < 1 Then Return False
            If AttentionNeighbors < 2 Then Return False
            If AttentionRandomLinks < 0 OrElse AttentionHubGenes < 0 Then Return False
            ' 枢纽与随机链接之外的槽位必须留给图邻域，否则先验图结构完全失效
            If AttentionRandomLinks + AttentionHubGenes >= AttentionNeighbors Then Return False

            Return True
        End Function

        ''' <summary>
        ''' <see cref="EdgeNormalization"/> 的合法取值判断（大小写不敏感）。
        ''' </summary>
        ''' <param name="mode">配置中的字符串</param>
        ''' <returns>属于 mean / sqrt / sum 时返回 True</returns>
        Public Shared Function IsKnownEdgeNormalization(mode As String) As Boolean
            Select Case If(mode, "").Trim().ToLowerInvariant()
                Case "mean", "sqrt", "sum"
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        ''' <summary>
        ''' 创建配置副本，便于在同一进程内比较不同超参数组合。
        ''' </summary>
        ''' <returns>逐字段复制的配置对象</returns>
        Public Function Clone() As GPSConfig
            Return New GPSConfig With {
                .MaxGenes = MaxGenes,
                .CpmNormalize = CpmNormalize,
                .LogTransform = LogTransform,
                .ZScore = ZScore,
                .PeDim = PeDim,
                .SpdMaxDist = SpdMaxDist,
                .CoexpressionTopK = CoexpressionTopK,
                .MinCoexpression = MinCoexpression,
                .DenseAttentionMaxNodes = DenseAttentionMaxNodes,
                .AttentionNeighbors = AttentionNeighbors,
                .AttentionRandomLinks = AttentionRandomLinks,
                .AttentionHubGenes = AttentionHubGenes,
                .DModel = DModel,
                .NLayers = NLayers,
                .NHeads = NHeads,
                .FfnDim = FfnDim,
                .Dropout = Dropout,
                .GeneEmbedDim = GeneEmbedDim,
                .UseSpdBias = UseSpdBias,
                .UseGlobalAttention = UseGlobalAttention,
                .UseLocalMpnn = UseLocalMpnn,
                .Window = Window,
                .GruHidden = GruHidden,
                .UseUncertainty = UseUncertainty,
                .PretrainEpochs = PretrainEpochs,
                .FinetuneEpochs = FinetuneEpochs,
                .LearningRate = LearningRate,
                .AutofocusGamma = AutofocusGamma,
                .LambdaDir = LambdaDir,
                .LambdaL1 = LambdaL1,
                .LambdaPrior = LambdaPrior,
                .GradientClip = GradientClip,
                .EarlyStopPatience = EarlyStopPatience,
                .MaskRatio = MaskRatio,
                .TrainRatio = TrainRatio,
                .ValidationRatio = ValidationRatio,
                .Seed = Seed,
                .SignificanceZScore = SignificanceZScore,
                .CascadeSteps = CascadeSteps,
                .CheckpointIntervalEpochs = CheckpointIntervalEpochs,
                .KnockdownResidual = KnockdownResidual,
                .EdgeNormalization = EdgeNormalization,
                .PerturbationChannel = PerturbationChannel,
                .PerturbationChannelGain = PerturbationChannelGain,
                .PerturbationChannelCoRegulation = PerturbationChannelCoRegulation
            }
        End Function

        ''' <summary>输出单行配置摘要</summary>
        ''' <returns>配置摘要文本</returns>
        Public Overrides Function ToString() As String
            Return $"GPSConfig(d_model={DModel}, layers={NLayers}, heads={NHeads}, ffn={FfnDim}, " &
                   $"window={Window}, gru={GruHidden}, pe={PeDim}, spd_max={SpdMaxDist}, " &
                   $"dense_attn<={DenseAttentionMaxNodes}, attn_c={AttentionNeighbors}, " &
                   $"lr={LearningRate}, pretrain={PretrainEpochs}, finetune={FinetuneEpochs})"
        End Function

    End Class

End Namespace
