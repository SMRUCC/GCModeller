Imports System

Namespace Config

    ''' <summary>
    ''' PBN（概率布尔网络）对 TSK 调控因子的作用方式。
    ''' </summary>
    Public Enum PbnRegularizationMode
        ''' <summary>不使用 PBN（默认，行为与旧版本一致）。</summary>
        None = 0
        ''' <summary>
        ''' 硬过滤：<c>CoD &lt; PbnMinCod</c> 的候选调控因子直接不参与 TSK 拟合。
        ''' </summary>
        Filter = 1
        ''' <summary>
        ''' 软加权：不删除候选，只把 CoD 折算成相关性打分的加成，用于排序。
        ''' </summary>
        Weighted = 2
    End Enum

    ''' <summary>
    ''' 拟合完成后的全局增益校准方式。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 逐靶基因独立拟合无法保证整张网络的 Jacobian 谱半径 <c>&lt; 1</c>；
    ''' 只要 <c>ρ(J) ≥ 1</c>，阻尼迭代就不收缩（<c>λ≈1</c> 时甚至是中立方向）。
    ''' 校准就是把 <c>J</c> 的斜率整体缩放 <c>γ = (1-ε)/ρ</c>，把拟合网络标定成压缩映射：
    ''' 符号、规则结构、相对强弱都不变，只有幅度被压进收敛域。
    ''' </para>
    ''' </remarks>
    Public Enum StabilityCalibrationMode
        ''' <summary>不校准（保留旧模型的行为）。</summary>
        None = 0
        ''' <summary>逐行 L1 上界的保守校准（<c>ρ(J) ≤ ‖J‖∞</c>）。</summary>
        RowL1 = 1
        ''' <summary>组装稀疏 Jacobian 做幂迭代求谱半径（默认）。</summary>
        SpectralRadius = 2
    End Enum

    ''' <summary>
    ''' 神经模糊网络与 PBN / FCM 融合的配置。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 三层各司其职：PBN 提供**定性**（哪条边真的存在、多大把握），
    ''' TSK 提供**定量**（局部线性斜率、连续剂量响应），
    ''' FCM 提供**有界粗粒度骨架**（保证有界、给求根一个接近不动点的初值）。
    ''' 因此这里的默认值全部取向「不改变原有数值结论」：
    ''' 正则关闭、FCM 热启动关闭、集成为 0 次采样。
    ''' </para>
    ''' <para>
    ''' 规模约束是硬门槛：<see cref="PbnMaxGenes"/> 与 <see cref="PbnMaxParents"/> 决定了
    ''' PBN 候选父集枚举的成本（每基因 <c>C(PbnCandidateSize, k≤PbnMaxParents)</c> 次真值表拟合），
    ''' 全基因组两两枚举是不可行的，超限时由 <see cref="Validate"/> 直接拒绝并给出可操作提示。
    ''' </para>
    ''' </remarks>
    Public Class HybridOptions

        ' ==================== PBN ====================

        ''' <summary>PBN 对 TSK 调控因子的作用方式。</summary>
        Public Property PbnRegularization As PbnRegularizationMode = PbnRegularizationMode.None

        ''' <summary>PBN 候选父集的最小 CoD；低于该值的父集不参与拟合。</summary>
        Public Property PbnMinCod As Double = 0.05

        ''' <summary>软加权模式下 CoD 折算成相关性打分的权重（0 = 不影响排序）。</summary>
        Public Property PbnWeight As Double = 1.0

        ''' <summary>每个靶基因进入 PBN 候选父集的基因个数上限。</summary>
        Public Property PbnCandidateSize As Integer = 5

        ''' <summary>
        ''' PBN 父集的组合上限；2 表示只枚举单基因与两基因父集（每基因 ≤ 21 个子集）。
        ''' </summary>
        Public Property PbnMaxParents As Integer = 2

        ''' <summary>启用 PBN 的最大基因数（超过则只对先验 TF 与高方差基因做）。</summary>
        Public Property PbnMaxGenes As Integer = 2000

        ''' <summary>布尔化阈值（按该基因 [-1, 1] 归一化后的分位数）。</summary>
        Public Property BinarizeQuantile As Double = 0.5

        ' ==================== 集成（不确定性） ====================

        ''' <summary>
        ''' PBN 规则采样的次数；0 表示不做集成（沿用确定性单点结果）。
        ''' </summary>
        ''' <remarks>
        ''' 这个数字正是 <c>InsilicoPerturbationExperiment</c> 的 <c>nSamples</c>：
        ''' 以前它在 FuzzyGene 里是「仅为接口兼容保留」，集成路径把它真正用起来。
        ''' </remarks>
        Public Property EnsembleSamples As Integer = 0

        ''' <summary>
        ''' 把布尔采样得到的模式作为软约束注入连续求解的权重；0 表示只统计、不改变数值结论。
        ''' </summary>
        Public Property ModeBiasWeight As Double = 0.0

        ' ==================== FCM ====================

        ''' <summary>是否用模块级 FCM 的稳态作为逐基因求根的热启动初值。</summary>
        Public Property FcmWarmStart As Boolean = False

        ''' <summary>FCM 的模块个数（概念数）；实际值会被 <c>N/50</c> 与 2 夹断。</summary>
        Public Property FcmModuleCount As Integer = 48

        ''' <summary>FCM 的 sigmoid 斜率。</summary>
        Public Property FcmAlpha As Double = 1.0

        ''' <summary>FCM 传播的最大步数。</summary>
        Public Property FcmMaxSteps As Integer = 200

        ''' <summary>FCM 收敛容差。</summary>
        Public Property FcmTolerance As Double = 0.000001

        ''' <summary>求根失败时，是否用 FCM 吸引子的周期均值作为代理结论。</summary>
        Public Property FcmFallbackAsSurrogate As Boolean = True

        ' ==================== 稳定性校准 ====================

        ''' <summary>
        ''' 全局增益校准方式；默认关闭。
        ''' </summary>
        ''' <remarks>
        ''' <para>
        ''' 默认关闭是有意的：默认求解器已经是牛顿/LM 求根，它对 <c>ρ(J) &gt; 1</c> 的映射同样能解
        ''' （求根不需要压缩性），因此校准不再是收敛的前提。
        ''' </para>
        ''' <para>
        ''' 校准会把环上的斜率整体缩小 <c>γ = (1-ε)/ρ</c>，代价是预测幅度同步缩小；
        ''' 在「相关补边造出大量反函数环」的网络上，实测平均 R² 会从 0.87 掉到负值。
        ''' 因此它应当是显式选择：只有在
        ''' ① 坚持用旧阻尼路径（<see cref="SolverMode.LegacyDamped"/>）、或
        ''' ② 需要扰动幅度有界、可解释时，才打开。
        ''' </para>
        ''' <para>
        ''' 打开之后训练报告会给出 <c>rho: 校准前 → 校准后</c> 与前后 R²，代价是可见的。
        ''' </para>
        ''' </remarks>
        Public Property Calibration As StabilityCalibrationMode = StabilityCalibrationMode.None

        ''' <summary>校准目标：<c>ρ(J) ≤ 1 - Epsilon</c>；取 0.05 留出 5% 的收敛余量。</summary>
        Public Property CalibrationEpsilon As Double = 0.05

        ''' <summary>校准后仍超限时的逐行再缩放下限（防止把某一行压成 0）。</summary>
        Public Property CalibrationMinScale As Double = 0.05

        ''' <summary>
        ''' 调控因子筛选的共线性上限：已选中的调控因子两两 <c>|Pearson|</c> 超过该值就不再选。
        ''' </summary>
        ''' <remarks>
        ''' <para>
        ''' 近共线的设计矩阵会让 LSE 给出大数值、反号的系数（沿数据流形的和正常，
        ''' 正交方向梯度极大），这是 <c>ρ(J) ≫ 1</c> 的主要来源。
        ''' 取 1.0 表示关闭该约束（默认）。
        ''' </para>
        ''' <para>
        ''' 默认关闭是为了不改变既有的候选筛选结果（共线约束会减少「数据发现的调控因子」数量）；
        ''' 稳定性敏感的场景建议显式设为 0.95 左右。
        ''' </para>
        ''' </remarks>
        Public Property MaxRegulatorCollinearity As Double = 1.0

        ''' <summary>逐属性校验并返回自身。</summary>
        Public Function Validate() As HybridOptions
            If PbnMinCod < 0.0 OrElse PbnMinCod > 1.0 Then
                Throw New ArgumentOutOfRangeException(NameOf(PbnMinCod), PbnMinCod, "the CoD threshold must be in [0, 1]")
            End If
            If PbnWeight < 0.0 Then
                Throw New ArgumentOutOfRangeException(NameOf(PbnWeight), PbnWeight, "the PBN weight must not be negative")
            End If
            If PbnCandidateSize < 1 Then
                Throw New ArgumentOutOfRangeException(NameOf(PbnCandidateSize), PbnCandidateSize, "the PBN candidate size must be positive")
            End If
            If PbnMaxParents < 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(PbnMaxParents), PbnMaxParents, "the PBN parent limit must not be negative")
            End If
            If PbnMaxGenes < 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(PbnMaxGenes), PbnMaxGenes, "the PBN gene limit must not be negative")
            End If
            If BinarizeQuantile <= 0.0 OrElse BinarizeQuantile >= 1.0 Then
                Throw New ArgumentOutOfRangeException(NameOf(BinarizeQuantile), BinarizeQuantile, "the binarization quantile must be in (0, 1)")
            End If
            If EnsembleSamples < 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(EnsembleSamples), EnsembleSamples, "the ensemble sample count must not be negative")
            End If
            If ModeBiasWeight < 0.0 Then
                Throw New ArgumentOutOfRangeException(NameOf(ModeBiasWeight), ModeBiasWeight, "the mode bias weight must not be negative")
            End If
            If FcmModuleCount < 2 Then
                Throw New ArgumentOutOfRangeException(NameOf(FcmModuleCount), FcmModuleCount, "at least two FCM modules are required")
            End If
            If FcmAlpha <= 0.0 Then
                Throw New ArgumentOutOfRangeException(NameOf(FcmAlpha), FcmAlpha, "the FCM slope must be positive")
            End If
            If FcmMaxSteps < 1 Then
                Throw New ArgumentOutOfRangeException(NameOf(FcmMaxSteps), FcmMaxSteps, "the FCM step budget must be positive")
            End If
            If FcmTolerance <= 0.0 Then
                Throw New ArgumentOutOfRangeException(NameOf(FcmTolerance), FcmTolerance, "the FCM tolerance must be positive")
            End If
            If CalibrationEpsilon <= 0.0 OrElse CalibrationEpsilon >= 1.0 Then
                Throw New ArgumentOutOfRangeException(NameOf(CalibrationEpsilon), CalibrationEpsilon, "the calibration epsilon must be in (0, 1)")
            End If
            If CalibrationMinScale <= 0.0 OrElse CalibrationMinScale >= 1.0 Then
                Throw New ArgumentOutOfRangeException(NameOf(CalibrationMinScale), CalibrationMinScale, "the calibration floor must be in (0, 1)")
            End If
            If MaxRegulatorCollinearity <= 0.0 OrElse MaxRegulatorCollinearity > 1.0 Then
                Throw New ArgumentOutOfRangeException(NameOf(MaxRegulatorCollinearity), MaxRegulatorCollinearity, "the collinearity cap must be in (0, 1]")
            End If

            ' 组合爆炸的硬门槛：PBN 父集枚举是 C(候选数, k≤上限)，与基因数相乘会迅速失控
            If PbnRegularization <> PbnRegularizationMode.None AndAlso PbnMaxParents > 3 Then
                Throw New ArgumentOutOfRangeException(
                    NameOf(PbnMaxParents), PbnMaxParents,
                    "PBN parent enumeration is combinatorial; cap PbnMaxParents at 3 or use a smaller PbnCandidateSize")
            End If

            Return Me
        End Function

        ''' <summary>浅拷贝（本类型没有数组字段）。</summary>
        Public Function Clone() As HybridOptions
            Return DirectCast(Me.MemberwiseClone(), HybridOptions)
        End Function

        ''' <summary>单行摘要（用于日志与训练汇总）。</summary>
        Public Overrides Function ToString() As String
            ' 不嵌套插值字符串：VB 对嵌套 $"" 的支持依赖编译器版本，这里先算好再拼
            Dim fcm As String = If(FcmWarmStart, "on:" & FcmModuleCount.ToString(), "off")

            Return $"pbn={PbnRegularization}/cod>={PbnMinCod:G3}/parents<={PbnMaxParents}, " &
                   $"ensemble={EnsembleSamples}, fcm={fcm}, " &
                   $"calibration={Calibration}/eps={CalibrationEpsilon:G3}, collinear<={MaxRegulatorCollinearity:G3}"
        End Function

    End Class

End Namespace
