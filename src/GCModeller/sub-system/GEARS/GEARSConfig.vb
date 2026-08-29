Imports GNN = Microsoft.VisualBasic.DeepLearning.GNN

''' <summary>
''' GEARS 虚拟扰动实验的超参数配置
''' </summary>
''' <remarks>
''' 默认取值针对 demo\TestData1 这样规模的数据（约 370 个基因、约 350 条先验边）做过权衡：
''' 在单核 CPU 上完成一次完整训练约需数十秒。基因规模更大时建议下调
''' <see cref="NSinglePerturbation"/>、<see cref="NComboPerturbation"/> 或 <see cref="Epochs"/>。
''' </remarks>
Public Class GEARSConfig

    ' ==================== 模型结构 ====================

    ''' <summary>基因身份嵌入维度（对应 readme 中 e_i 的维度 d）</summary>
    ''' <returns>嵌入向量长度，默认 16</returns>
    Public Property EmbeddingDim As Integer = 16

    ''' <summary>图卷积隐藏层维度</summary>
    ''' <returns>隐藏层维度，默认 32</returns>
    Public Property HiddenDim As Integer = 32

    ''' <summary>
    ''' 图卷积层数，等于模型可捕捉的间接调控跳数
    ''' </summary>
    ''' <returns>层数，默认 2</returns>
    ''' <remarks>readme 第十节指出过深的 GNN 会导致过平滑，通常 2~4 层为宜。</remarks>
    Public Property NumLayers As Integer = 2

    ''' <summary>图卷积层的激活函数</summary>
    ''' <returns><see cref="GNN.ActivationType"/> 枚举值，默认 Tanh</returns>
    Public Property Activation As GNN.ActivationType = GNN.ActivationType.Tanh

    ''' <summary>
    ''' 是否为每种边关系类型（激活/抑制/共表达）分配独立的变换矩阵
    ''' </summary>
    ''' <returns>默认 False，即共享变换矩阵、仅用符号区分激活与抑制</returns>
    ''' <remarks>置为 True 时严格对应 readme §5.4 的异质图消息传递，但训练开销会增大。</remarks>
    Public Property UsePerRelationTransform As Boolean = False

    ''' <summary>是否使用稠密归一化邻接矩阵聚合（默认 False，使用稀疏入边聚合）</summary>
    ''' <returns>稠密模式返回 True</returns>
    Public Property UseDense As Boolean = False

    ' ==================== 训练过程 ====================

    ''' <summary>训练轮数</summary>
    ''' <returns>默认 30</returns>
    Public Property Epochs As Integer = 30

    ''' <summary>Adam 优化器学习率</summary>
    ''' <returns>默认 0.01</returns>
    Public Property LearningRate As Single = 0.01F

    ''' <summary>L2 正则化（权重衰减）系数，0 表示不启用</summary>
    ''' <returns>默认 0</returns>
    Public Property L2Lambda As Double = 0.0

    ''' <summary>每隔多少个 epoch 打印一次训练损失；0 表示静默</summary>
    ''' <returns>默认 5</returns>
    Public Property PrintEvery As Integer = 5

    ' ==================== 训练样本生成 ====================

    ''' <summary>仿真生成的单基因扰动样本数量</summary>
    ''' <returns>默认 24</returns>
    Public Property NSinglePerturbation As Integer = 24

    ''' <summary>仿真生成的组合扰动样本数量</summary>
    ''' <returns>默认 16</returns>
    Public Property NComboPerturbation As Integer = 16

    ''' <summary>组合扰动中同时扰动的基因个数</summary>
    ''' <returns>默认 2</returns>
    Public Property ComboSize As Integer = 2

    ''' <summary>仿真器的每跳信号衰减系数</summary>
    ''' <returns>默认 0.6</returns>
    Public Property PropagationDecay As Double = 0.6

    ''' <summary>仿真器的最大传播跳数</summary>
    ''' <returns>默认 3</returns>
    Public Property MaxHops As Integer = 3

    ''' <summary>仿真器中组合扰动的协同放大系数，0 表示退化为线性叠加</summary>
    ''' <returns>默认 0.35</returns>
    Public Property SynergyStrength As Double = 0.35

    ''' <summary>仿真标签的噪声水平（以基因标准差为单位）</summary>
    ''' <returns>默认 0.02</returns>
    Public Property SimulatorNoise As Double = 0.02

    ' ==================== 图构建 ====================

    ''' <summary>
    ''' 每个基因追加的共表达边数量（GEARS 的共表达协方差图）
    ''' </summary>
    ''' <returns>默认 0，即关闭共表达图</returns>
    ''' <remarks>置为正数时会先按 control 表达的 Pearson 相关取 Top-K 再追加无向共表达边，
    ''' 可提升信息传播覆盖面，但也会显著增加建图与训练耗时。</remarks>
    Public Property CoexpressionTopK As Integer = 0

    ''' <summary>共表达边的最小相关系数阈值</summary>
    ''' <returns>默认 0.7</returns>
    Public Property MinCoexpression As Double = 0.7

    ' ==================== 推理与结果判读 ====================

    ''' <summary>
    ''' 显著性判据：|Z-score| 大于该阈值才被标记为显著变化
    ''' </summary>
    ''' <returns>默认 0.5，与 BNLearn 现有干预分析「变化超过 0.5 个标准差」的口径保持一致</returns>
    Public Property SignificanceZScore As Double = 0.5

    ''' <summary>随机种子；给定后建图、初始化、样本生成与推理均可复现</summary>
    ''' <returns>默认 2024</returns>
    Public Property Seed As Integer = 2024

    ''' <summary>
    ''' 校验参数取值是否合法
    ''' </summary>
    ''' <returns>参数合法则返回 True</returns>
    Public Function Validate() As Boolean
        If EmbeddingDim <= 0 Then Return False
        If HiddenDim <= 0 Then Return False
        If NumLayers < 1 OrElse NumLayers > 6 Then Return False
        If Epochs < 0 Then Return False
        If LearningRate <= 0 Then Return False
        If ComboSize < 2 Then Return False
        If PropagationDecay <= 0 OrElse PropagationDecay >= 1 Then Return False
        If MaxHops < 1 Then Return False
        If CoexpressionTopK < 0 Then Return False

        Return True
    End Function

    ''' <summary>
    ''' 输出配置摘要
    ''' </summary>
    ''' <returns>多行配置描述</returns>
    Public Overrides Function ToString() As String
        Return $"GEARSConfig(embed={EmbeddingDim}, hidden={HiddenDim}, layers={NumLayers}, " &
               $"epochs={Epochs}, lr={LearningRate}, " &
               $"samples={NSinglePerturbation}+{NComboPerturbation}, seed={Seed})"
    End Function
End Class
