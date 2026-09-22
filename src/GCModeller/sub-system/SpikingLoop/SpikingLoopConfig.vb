' ============================================================================
' SpikingLoopConfig.vb — SNN-GRN 建模流水线的全部超参数与开关
'
' 对应 readme 中散落在各小节的超参数，集中在一处便于扫描与复现实验：
'   一.1  先验图构建      TopKWgcna / WgcnaThreshold / PriorNormalization / UseRegulationSign
'   一.2  伪时间离散化    NumBins / SmoothingWindow / Normalization
'   二.1  LIF 神经元      Beta(λ) / Threshold(V_th) / ResetMode / MembraneInitGain
'   二.3  输入编码        EncodingMode / CurrentGain(I_max)
'   二.4  输出解码        ReadoutSource
'   三.1  训练目标        PriorRegAlpha(α) / SparsityBeta(β)
'   3.3   梯度流细节      Alpha / AlphaGrowth（自适应替代梯度形状）
'   六    P0 子网络规模   MaxGenes
'
' 命名与 readme 的对应关系（重要，避免混淆）：
'   NumBins          readme 一.2 的 T          —— 伪时间分箱数 = 表达轨迹长度
'   SimulationSteps  readme 二.1 前向内的 T    —— 单个预测窗口内 SNN 展开的步数
'   Horizon          readme 三.1 的 Δt         —— 单步预测的前瞻量（预测 t+Δt）
' ============================================================================

Imports Microsoft.VisualBasic.DeepLearning.SpikingNeuralNetwork
Imports std = System.Math

''' <summary>先验权重的归一化方式（readme 一.1 第三步）</summary>
Public Enum PriorNormalization
    ''' <summary>不做归一化，保留先验权重的绝对尺度</summary>
    None
    ''' <summary>
    ''' 按突触前（矩阵行）归一化：每个突触前神经元的所有出边权重和为 1。
    ''' readme 一.1 明确采用这一方式（稳定下游发放率），故为默认值。
    ''' </summary>
    FanOut
    ''' <summary>按突触后（矩阵列）归一化</summary>
    FanIn
    ''' <summary>按全局最大绝对权重缩放</summary>
    GlobalMax
End Enum

''' <summary>输出解码方式（readme 二.4 的方案 A/B/C）</summary>
Public Enum ReadoutSource
    ''' <summary>方案 A：膜电位读取（梯度路径更平滑，回归首选）</summary>
    MembranePotential
    ''' <summary>方案 B：脉冲计数 / 发放率解码</summary>
    FiringRate
    ''' <summary>方案 C：发放率与膜电位拼接后线性回归</summary>
    Concat
End Enum

''' <summary>伪时间到 SNN 时间步的分箱方式</summary>
Public Enum PseudotimeBinning
    ''' <summary>分位数分箱：每个时间窗包含近似相同数量的样本（readme 一.2 的 QUANTILE_BINNning）</summary>
    Quantile
    ''' <summary>等宽分箱：伪时间轴等分，适合样本分布均匀的情形</summary>
    Uniform
End Enum

''' <summary>SNN-GRN 建模的配置</summary>
Public Class SpikingLoopConfig

#Region "常规"

    ''' <summary>随机种子（保证分箱、初始化、ADAM 全流程可复现）</summary>
    Public Property Seed As Integer = 2024

    ''' <summary>是否输出流水线各阶段的日志</summary>
    Public Property Verbose As Boolean = True

#End Region

#Region "P0 · 核心子网络筛选"

    ''' <summary>
    ''' 参与建模的最大基因数（0 = 不限制）。readme 六明确指出"全基因组规模不可训练"，
    ''' 建议先用几百~几千个核心基因做概念验证。
    ''' </summary>
    Public Property MaxGenes As Integer = 300

#End Region

#Region "一.1 · 先验图构建"

    ''' <summary>WGCNA 补充边：每个基因保留的共表达关联数（Top-K），0 = 关闭 WGCNA 补充边</summary>
    Public Property TopKWgcna As Integer = 20

    ''' <summary>WGCNA 补充边的最小关联强度阈值（低于该值不建边）</summary>
    Public Property WgcnaThreshold As Double = 0.3

    ''' <summary>先验权重归一化方式（readme 一.1 默认按突触前归一化）</summary>
    Public Property PriorNormalization As PriorNormalization = PriorNormalization.FanOut

    ''' <summary>
    ''' 是否用调控方向决定先验权重的符号：Inhibitor（抑制型 TF）的边取负权重，
    ''' 从而使抑制型转录因子能够降低靶基因神经元的膜电位。
    ''' 这是对 readme 一.1 伪代码的一处有意增强——readme 只写正值，但
    ''' BNLearn 的先验网络（RegulatoryEdge.RegulationType）明确携带
    ''' Activator / Inhibitor 语义，忽略符号会完全丢失抑制型调控信息。
    ''' </summary>
    Public Property UseRegulationSign As Boolean = True

    ''' <summary>是否允许自环（基因调控自身的突触）</summary>
    Public Property AllowSelfLoop As Boolean = False

#End Region

#Region "一.2 · 伪时间离散化"

    ''' <summary>伪时间分箱数 = 训练用表达轨迹长度</summary>
    Public Property NumBins As Integer = 24

    ''' <summary>分箱方式</summary>
    Public Property Binning As PseudotimeBinning = PseudotimeBinning.Quantile

    ''' <summary>
    ''' 沿时间的滑动平均窗口（奇数，1 = 不平滑）。
    ''' readme 一.2 强调必须平滑以缓解单细胞 Dropout 造成的虚假零值。
    ''' </summary>
    Public Property SmoothingWindow As Integer = 3

    ''' <summary>是否把表达轨迹 MinMax 归一到 [0,1]（readme 一.2 最后一步）</summary>
    Public Property NormalizeExpression As Boolean = True

    ''' <summary>MinMax 归一化的下限/上限（编码为输入电流的取值区间）</summary>
    Public Property ExpressionMin As Double = 0.0

    ''' <summary>MinMax 归一化的上限</summary>
    Public Property ExpressionMax As Double = 1.0

#End Region

#Region "二 · LIF 神经元与编解码"

    ''' <summary>每个预测窗口内 SNN 展开的时间步数（每个时间步都注入同一表达电流）</summary>
    Public Property SimulationSteps As Integer = 8

    ''' <summary>单步预测的前瞻量 Δt：用 t 时刻的状态预测 t+Δt 时刻的表达（readme 三.1）</summary>
    Public Property Horizon As Integer = 1

    ''' <summary>膜电位衰减系数 β = exp(−Δt/τ_m)，作为超参数扫描（readme 3.3 第 2 条）</summary>
    Public Property Beta As Double = 0.9

    ''' <summary>发放阈值 V_th</summary>
    Public Property Threshold As Double = 1.0

    ''' <summary>复位模式：硬复位（置零）或软复位（减去阈值，保留残余信息）</summary>
    Public Property ResetMode As LIFResetMode = LIFResetMode.ZeroOnSpike

    ''' <summary>替代梯度函数</summary>
    Public Property Surrogate As SurrogateKind = SurrogateKind.FastSigmoid

    ''' <summary>替代梯度陡度 α（初始值）</summary>
    Public Property Alpha As Double = 2.0

    ''' <summary>
    ''' α 的每轮退火倍率（1.0 = 不退火）。readme 3.3 建议"自适应替代梯度形状"
    ''' 以缓解梯度不匹配随时间的累积：α 增大 → 替代导数更尖 → 逐渐逼近真实阶跃。
    ''' </summary>
    Public Property AlphaGrowth As Double = 1.0

    ''' <summary>α 的上限（退火时不超过该值）</summary>
    Public Property AlphaMax As Double = 20.0

    ''' <summary>
    ''' 初始膜电位增益：u0 = MembraneInitGain · 表达状态。
    ''' readme 三.2 的 <c>u0 = MAP_TO_MEMBRANE(U_seq[t])</c>。
    ''' </summary>
    Public Property MembraneInitGain As Double = 0.5

    ''' <summary>输入电流增益 I_max：表达值编码为电流时的缩放系数（readme 二.3 的 SCALE）</summary>
    Public Property CurrentGain As Double = 1.0

    ''' <summary>输出解码方式</summary>
    Public Property ReadoutSource As ReadoutSource = ReadoutSource.MembranePotential

    ''' <summary>解码头是否用单位阵初始化（解码目标与网络状态同维时推荐）</summary>
    Public Property IdentityReadoutInit As Boolean = True

#End Region

#Region "三 · 训练"

    ''' <summary>训练轮数</summary>
    Public Property Epochs As Integer = 200

    ''' <summary>学习率</summary>
    Public Property LearningRate As Double = 0.001

    ''' <summary>梯度 L2 范数裁剪上限（SNN 脉冲稀疏易产生梯度尖峰，readme 三节）</summary>
    Public Property ClipNorm As Double = 1.0

    ''' <summary>先验结构正则权重 α（readme 三.1 的 α·loss_prior）</summary>
    Public Property PriorRegAlpha As Double = 0.1

    ''' <summary>稀疏正则权重 β（readme 三.1 的 β·loss_sparse，L1）</summary>
    Public Property SparsityBeta As Double = 0.0001

    ''' <summary>训练集占伪时间轨迹的比例（其余作为验证集，readme 三.2 的"留出部分伪时间区间"）</summary>
    Public Property TrainSplit As Double = 0.8

    ''' <summary>验证集连续多少轮无改善即早停（0 = 不早停）</summary>
    Public Property EarlyStopPatience As Integer = 0

    ''' <summary>
    ''' 训练结束后是否把参数回滚到"验证损失最优"的那一轮。
    ''' True（默认）：报告与导出的指标对应最优模型；否则对应最后一轮。
    ''' 小样本场景下（训练窗口仅十几~几十个）模型极易过拟合，
    ''' 若不回滚，最终指标会明显差于训练过程中出现过的最好水平。
    ''' </summary>
    Public Property RestoreBestWeights As Boolean = True

    ''' <summary>每隔多少轮输出一次训练日志</summary>
    Public Property PrintEvery As Integer = 20

    ''' <summary>权重是否冻结为初始先验值（readme P2 的最小可行模型：只做固定权重前向演化）</summary>
    Public Property FrozenWeights As Boolean = False

    ''' <summary>是否在每轮更新后把权重裁剪回结构掩码（保证先验之外的突触恒为 0）</summary>
    Public Property EnforceMaskAfterUpdate As Boolean = True

#End Region

#Region "四 · 虚拟扰动"

    ''' <summary>虚拟扰动的模拟时间步数（可长于训练时的窗口，用于观察长时间演化）</summary>
    Public Property PerturbationSteps As Integer = 16

    ''' <summary>
    ''' 虚拟扰动闭环是否跨决策步保持膜电位。
    ''' False（默认）：每个决策步都按训练协议从当前表达状态重新初始化膜电位，
    '''               闭环退化为"学到的一步预测映射反复迭代"，数值良态（推荐）；
    ''' True ：膜电位跨步携带（原生有状态动力学），但等效增益可能 &gt; 1 而发散，
    '''        需用 PerturbationTrajectory.ClampRatio 监控。
    ''' </summary>
    Public Property PerturbationCarryMembrane As Boolean = False

    ''' <summary>
    ''' 闭环迭代的松弛系数 η ∈ (0,1]：state ← state + η·(f(state) − state)。
    ''' 1.0（默认）= 严格迭代学到的一步预测映射（readme 四的原始语义）。
    ''' 取小于 1 会阻尼每步更新，等价于"用更小的有效伪时间步长积分"——
    ''' 当一步预测误差在多次迭代中被放大（ClampRatio 偏高）时，它是让闭环保持
    ''' 良态的最直接手段（readme 3.3 也提示 τ_m/步长需要作为超参数调优）。
    ''' </summary>
    Public Property PerturbationRelaxation As Double = 1.0

    ''' <summary>
    ''' 每个决策步内注入扰动的内层时间步数。
    ''' 0（默认）= 在整个决策窗口内持续注入——重复注入是必要的：每当表达状态被重新
    ''' 编码为输入电流，被敲除基因的电流就会"复活"、被过表达基因的增益也会失效，
    ''' 因此只有每步重新施加才能表达"持续敲除/持续过表达"的语义。
    ''' 设为正数则只在每个决策步的前 N 个内层时间步注入（readme 四的"仅起始阶段注入"）。
    ''' </summary>
    Public Property PerturbationInjectSteps As Integer = 0

#End Region

#Region "校验"

    ''' <summary>
    ''' 校验配置的合法性；返回人类可读的告警列表（空列表表示未见异常）。
    ''' 抛出异常的只有"必然导致运行失败"的项（如负的步数）。
    ''' </summary>
    Public Function Validate() As List(Of String)
        Dim warns As New List(Of String)()

        If NumBins < 3 Then
            Throw New ArgumentException($"NumBins 至少为 3（当前 {NumBins}）：分箱过少无法构造时序轨迹")
        End If
        If SimulationSteps < 1 Then
            Throw New ArgumentException($"SimulationSteps 必须为正整数（当前 {SimulationSteps}）")
        End If
        If Horizon < 1 Then
            Throw New ArgumentException($"Horizon(Δt) 必须为正整数（当前 {Horizon}）")
        End If
        If Beta <= 0.0 OrElse Beta >= 1.0 Then
            Throw New ArgumentException($"Beta(λ) 必须落在 (0,1) 区间（当前 {Beta}）")
        End If
        If Threshold <= 0.0 Then
            Throw New ArgumentException($"Threshold(V_th) 必须为正数（当前 {Threshold}）")
        End If
        If LearningRate <= 0.0 Then
            Throw New ArgumentException($"LearningRate 必须为正数（当前 {LearningRate}）")
        End If
        If TrainSplit <= 0.0 OrElse TrainSplit >= 1.0 Then
            Throw New ArgumentException($"TrainSplit 必须落在 (0,1) 区间（当前 {TrainSplit}）")
        End If
        If PerturbationRelaxation <= 0.0 OrElse PerturbationRelaxation > 1.0 Then
            Throw New ArgumentException(
                $"PerturbationRelaxation 必须落在 (0,1] 区间（当前 {PerturbationRelaxation}）")
        End If
        If NumBins <= (SimulationSteps + Horizon) Then
            warns.Add($"NumBins({NumBins}) 过小：可用训练窗口仅 {NumBins - Horizon} 个，建议增大分箱数")
        End If
        If MaxGenes > RecurrentLIFLayer.DenseNeuronLimit Then
            warns.Add($"MaxGenes({MaxGenes}) 超过稠密递归层推荐上限 {RecurrentLIFLayer.DenseNeuronLimit}：" &
                      "单步计算 O(N²)，BPTT 会明显变慢（readme 六的风险清单）")
        End If
        If TopKWgcna > 0 AndAlso WgcnaThreshold <= 0.0 Then
            warns.Add("已启用 WGCNA 补充边，但阈值为 0，可能引入大量弱连接")
        End If
        If PriorRegAlpha <= 0.0 Then
            warns.Add("PriorRegAlpha = 0：先验结构约束被关闭，突触可能漂离先验调控关系")
        End If

        Return warns
    End Function

    Public Overrides Function ToString() As String
        Return $"SpikingLoopConfig(bins={NumBins}, simSteps={SimulationSteps}, Δt={Horizon}, " &
               $"β={Beta}, θ={Threshold}, reset={ResetMode}, α={Alpha}, " &
               $"readout={ReadoutSource}, epochs={Epochs}, lr={LearningRate}, " &
               $"αprior={PriorRegAlpha}, βsparse={SparsityBeta}, maxGenes={MaxGenes})"
    End Function

#End Region

End Class
