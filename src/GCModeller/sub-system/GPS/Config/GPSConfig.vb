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

        ''' <summary>每隔多少个 epoch 打印一次损失；0 表示静默</summary>
        ''' <returns>默认 5</returns>
        Public Property PrintEvery As Integer = 5

        ''' <summary>训练集时间点占比（按时间顺序切分）</summary>
        ''' <returns>默认 0.7</returns>
        Public Property TrainRatio As Double = 0.7

        ''' <summary>验证集时间点占比（按时间顺序切分）</summary>
        ''' <returns>默认 0.15</returns>
        Public Property ValidationRatio As Double = 0.15

        ''' <summary>随机种子；给定后初始化、dropout 与掩码均可复现</summary>
        ''' <returns>默认 2024</returns>
        Public Property Seed As Integer = 2024

        ' ==================== 推理与扰动 ====================

        ''' <summary>显著性判据：|Z-score| 大于该阈值才标记为显著变化</summary>
        ''' <returns>默认 0.5，与 BNLearn 现有干预分析口径一致</returns>
        Public Property SignificanceZScore As Double = 0.5

        ''' <summary>多步级联模拟的默认步数</summary>
        ''' <returns>默认 5</returns>
        Public Property CascadeSteps As Integer = 5

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

            Return True
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
                .PrintEvery = PrintEvery,
                .TrainRatio = TrainRatio,
                .ValidationRatio = ValidationRatio,
                .Seed = Seed,
                .SignificanceZScore = SignificanceZScore,
                .CascadeSteps = CascadeSteps
            }
        End Function

        ''' <summary>输出单行配置摘要</summary>
        ''' <returns>配置摘要文本</returns>
        Public Overrides Function ToString() As String
            Return $"GPSConfig(d_model={DModel}, layers={NLayers}, heads={NHeads}, ffn={FfnDim}, " &
                   $"window={Window}, gru={GruHidden}, pe={PeDim}, spd_max={SpdMaxDist}, " &
                   $"lr={LearningRate}, pretrain={PretrainEpochs}, finetune={FinetuneEpochs})"
        End Function

    End Class

End Namespace
