Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Math.Calculus.Dynamics
Imports SMRUCC.genomics.Analysis.SSystem.Runner
Imports PriorNet = SMRUCC.genomics.Analysis.BNLearn.Core.PriorNetwork

Namespace GRN

    ''' <summary>
    ''' The available normalization methods of the gene expression matrix.
    ''' (基因表达矩阵的归一化方法)
    ''' </summary>
    Public Enum NormalizationMethods
        ''' <summary>
        ''' Do nothing on the raw expression data.(不做任何处理)
        ''' </summary>
        None = 0
        ''' <summary>
        ''' log2(x + pseudo) transform.(对数变换)
        ''' </summary>
        Log2 = 1
        ''' <summary>
        ''' z-score normalization on each gene row.(按行做 z-score 标准化)
        ''' </summary>
        RowZScore = 2
        ''' <summary>
        ''' Quantile normalization between samples.(分位数归一化)
        ''' </summary>
        Quantile = 3
    End Enum

    ''' <summary>
    ''' The configuration object of the S-system gene regulatory network inference.
    ''' (基因表达调控网络的S-system拟合参数配置)
    ''' </summary>
    ''' <remarks>
    ''' 参数的生物学取值范围直接参考 method.md 的表格：
    ''' alpha/beta 为速率常数（典型 [0,10]），g/h 为动力学阶（典型 [-3,3]）。
    ''' </remarks>
    Public Class FittingOptions

        ''' <summary>
        ''' The value bounds of the rate constant alpha.(合成速率常数的上下界)
        ''' </summary>
        ''' <returns></returns>
        Public Property AlphaBounds As DoubleRange = New DoubleRange(0, 10)
        ''' <summary>
        ''' The value bounds of the rate constant beta.(降解速率常数的上下界)
        ''' </summary>
        ''' <returns></returns>
        Public Property BetaBounds As DoubleRange = New DoubleRange(0, 10)
        ''' <summary>
        ''' The value bounds of the kinetic order g.(合成项动力学阶的上下界)
        ''' </summary>
        ''' <returns></returns>
        Public Property GBounds As DoubleRange = New DoubleRange(-3, 3)
        ''' <summary>
        ''' The value bounds of the kinetic order h.(降解项动力学阶的上下界)
        ''' </summary>
        ''' <returns></returns>
        Public Property HBounds As DoubleRange = New DoubleRange(-3, 3)

        ''' <summary>
        ''' 绝对值小于本阈值的交互会被置零（与 SPEM 包的 sparsity 参数同义，默认 0.2）
        ''' </summary>
        ''' <returns></returns>
        Public Property Sparsity As Double = 0.2
        ''' <summary>
        ''' 每一个靶基因最多保留的调控边的数量（与 SPEM 包的 n 参数同义，默认 3）
        ''' </summary>
        ''' <returns></returns>
        Public Property MaxEdgesPerGene As Integer = 3
        ''' <summary>
        ''' 稀疏惩罚项（L1）的权重系数，零表示不施加惩罚
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 本目标函数之中的惩罚项是**乘性**的（<c>f = 相对误差 × (1 + 惩罚)</c>），
        ''' L1 会与残差项直接争夺系数的大小：
        ''' 在干净数据（例如由解析方程积分得到的合成轨迹）之上它能有效地抑制虚假的调控关系，
        ''' 但是在噪声很强的单细胞 pseudotime 数据之上，即使 L1 只有 1E-4
        ''' 也会把优化结果压到「系数为零、相对误差为 1」的平凡解（实测 R² 由 0.65 降到 0）。
        ''' 
        ''' 故本参数保留库层面的缺省值以维持干净数据上的表现，
        ''' 而噪声数据（真实 pseudotime 场景）应当显式设为零，
        ''' 稀疏性交由 <see cref="Sparsity"/> 与 <see cref="MaxEdgesPerGene"/> 在拓扑阶段控制。
        ''' </remarks>
        Public Property L1Penalty As Double = 0.01R
        ''' <summary>
        ''' 稳定性惩罚项的权重系数：稳态的渐近稳定性要求对数增益矩阵的对角元为负数，
        ''' 故当拟合出正的自环净弹性的时候会施加惩罚。零表示关闭该约束
        ''' </summary>
        ''' <returns></returns>
        Public Property StabilityPenalty As Double = 0.2R
        ''' <summary>
        ''' 是否保留自环（X_i 调控其自身），默认保留
        ''' </summary>
        ''' <returns></returns>
        Public Property KeepSelfLoop As Boolean = True
        ''' <summary>
        ''' 预先使用相关性筛选出的候选调控因子的最大数量；零表示不筛选（使用全部基因作为候选）
        ''' </summary>
        ''' <returns></returns>
        Public Property MaxRegulators As Integer = 0

        ''' <summary>
        ''' 用于约束网络结构的先验知识网络；为空的时候不启用先验约束
        ''' </summary>
        ''' <returns></returns>
        Public Property Prior As PriorNet = Nothing
        ''' <summary>
        ''' 是否启用先验知识约束（<see cref="Prior"/> 不为空的时候才有效）
        ''' </summary>
        ''' <returns></returns>
        Public Property UsePrior As Boolean = True
        ''' <summary>
        ''' 先验不支持的交互的软惩罚强度：惩罚量取「非先验交互的质量占比」，
        ''' 故该项的取值是相对量，零表示关闭该约束
        ''' </summary>
        ''' <returns></returns>
        Public Property PriorPenalty As Double = 0.2R
        ''' <summary>
        ''' 与先验方向相冲突的交互的惩罚强度：惩罚量取「符号违背量的平均值」，
        ''' 零表示关闭该约束
        ''' </summary>
        ''' <returns></returns>
        Public Property PriorSignPenalty As Double = 0.2R
        ''' <summary>
        ''' 是否启用先验方向（激活/抑制）的符号约束
        ''' </summary>
        ''' <returns></returns>
        Public Property UsePriorSign As Boolean = True
        ''' <summary>
        ''' 稀疏化阶段对先验支持边的排序加权：排序分数为
        ''' <c>|net| * (1 + PriorRankBoost * confidenceWeight)</c>
        ''' </summary>
        ''' <returns></returns>
        Public Property PriorRankBoost As Double = 0.5R
        ''' <summary>
        ''' 是否输出性能剖析信息（目标函数调用次数，以及差分进化 / L-BFGS-B 两个阶段的累计耗时）
        ''' </summary>
        ''' <returns></returns>
        Public Property Profile As Boolean = False
        ''' <summary>
        ''' 差分进化的停滞代数上限：连续这么多代最优个体没有实质改进就提前结束；
        ''' 零或者负数表示跑满 <see cref="DEMaxIterations"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property DEStagnation As Integer = 100
        ''' <summary>
        ''' 是否启用先验的硬约束：候选调控因子只允许来自先验（加上自环）。
        ''' 默认关闭，即保留发现新边的能力（软约束）
        ''' </summary>
        ''' <returns></returns>
        Public Property PriorCandidateRestriction As Boolean = False

        ''' <summary>
        ''' 对表达矩阵施加的归一化方法
        ''' </summary>
        ''' <returns></returns>
        Public Property Normalize As NormalizationMethods = NormalizationMethods.Log2
        ''' <summary>
        ''' 对数变换的伪计数，避免出现 log(0)
        ''' </summary>
        ''' <returns></returns>
        Public Property PseudoCount As Double = 1.0R
        ''' <summary>
        ''' 用户在外部显式给定的时间轴；为 Nothing 的时候会尝试从样本编号之中解析
        ''' </summary>
        ''' <returns></returns>
        Public Property TimePoints As Double() = Nothing
        ''' <summary>
        ''' 是否使用三次样条平滑之后再做数值微分
        ''' </summary>
        ''' <returns></returns>
        Public Property Denoise As Boolean = True
        ''' <summary>
        ''' 数值微分之前的滑动平均窗口（奇数）；零或者小于 3 表示不做滑动平均。
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 单细胞 pseudotime 分箱之后得到的轨迹由于细胞数量有限，
        ''' 仍然带有很强的高频噪声，直接对插值样条求导会让斜率几乎全部由噪声构成
        ''' （实测 80 个分箱之上一阶自相关只有 0.1~0.6，相邻箱几乎独立），
        ''' 故默认在求导之前先做一次 5 点滑动平均。
        ''' </remarks>
        Public Property SmoothWindow As Integer = 5

        ''' <summary>
        ''' 差分进化算法的种群大小
        ''' </summary>
        ''' <returns></returns>
        Public Property DEPopulationSize As Integer = 60
        ''' <summary>
        ''' 差分进化算法的最大迭代次数
        ''' </summary>
        ''' <returns></returns>
        Public Property DEMaxIterations As Integer = 3000
        ''' <summary>
        ''' 差分进化算法的变异权重 F，取值区间 [0,2]
        ''' </summary>
        ''' <returns></returns>
        Public Property DEF As Double = 0.8R
        ''' <summary>
        ''' 差分进化算法的交叉概率 CR，取值区间 [0,1]
        ''' </summary>
        ''' <returns></returns>
        Public Property DECR As Double = 0.7R
        ''' <summary>
        ''' 差分进化算法的收敛阈值
        ''' </summary>
        ''' <returns></returns>
        Public Property DEThreshold As Double = 0.0001R
        ''' <summary>
        ''' L-BFGS-B 局部精修的最大迭代次数
        ''' </summary>
        ''' <returns></returns>
        Public Property LBFGSIterations As Integer = 500

        ''' <summary>
        ''' 是否执行第二阶段的轨迹精修（需要反复积分ODE，耗时较长，默认关闭）
        ''' </summary>
        ''' <returns></returns>
        Public Property RefineTrajectory As Boolean = False
        ''' <summary>
        ''' 轨迹精修阶段的最大迭代次数
        ''' </summary>
        ''' <returns></returns>
        Public Property RefineMaxIterations As Integer = 60

        ''' <summary>
        ''' 随机数种子，零表示使用系统种子
        ''' </summary>
        ''' <returns></returns>
        Public Property Seed As Integer = 0
        ''' <summary>
        ''' 是否并行求解多个基因的参数（逐基因之间互相独立）
        ''' </summary>
        ''' <returns></returns>
        Public Property Parallel As Boolean = True
        ''' <summary>
        ''' 是否输出拟合过程的详细信息
        ''' </summary>
        ''' <returns></returns>
        Public Property Verbose As Boolean = True

        ''' <summary>
        ''' 仿真（轨迹精修与虚拟扰动）所使用的终止时间；零表示使用表达矩阵的时间轴终点
        ''' </summary>
        ''' <returns></returns>
        Public Property FinalTime As Double = 0
        ''' <summary>
        ''' 仿真所使用的积分步长
        ''' </summary>
        ''' <returns></returns>
        Public Property Precise As Double = 0.1
        ''' <summary>
        ''' 仿真所使用的ODE求解引擎
        ''' </summary>
        ''' <returns></returns>
        Public Property Solver As ODESolverMethod = ODESolverMethod.RK4
        ''' <summary>
        ''' 仿真采样点的数量；零表示由终止时间与步长自动推导
        ''' </summary>
        ''' <returns></returns>
        Public Property Steps As Integer = 0

        ''' <summary>
        ''' Create a copy of current configuration object.
        ''' </summary>
        ''' <returns></returns>
        Public Function Clone() As FittingOptions
            Return DirectCast(Me.MemberwiseClone(), FittingOptions)
        End Function

        ''' <summary>
        ''' 创建用于仿真求解的运行配置对象
        ''' </summary>
        ''' <param name="finalTime"></param>
        ''' <returns></returns>
        Public Function CreateRunOptions(Optional finalTime As Double = 0) As RunOptions
            Dim options As New RunOptions With {
                .FinalTime = If(finalTime > 0, finalTime, Me.FinalTime),
                .Precise = Me.Precise,
                .Solver = Me.Solver,
                .Steps = Me.Steps,
                .UseODEs = True,
                .NonNegative = True
            }

            Return options
        End Function

        ''' <summary>
        ''' 检查配置参数是否合法
        ''' </summary>
        Public Sub Validate()
            If Sparsity < 0 Then
                Throw New ArgumentException("The sparsity threshold of the S-system inference should not be a negative value!")
            End If
            If MaxEdgesPerGene < 0 Then
                Throw New ArgumentException("The max edges per gene should not be a negative value!")
            End If
            If PseudoCount <= 0 Then
                Throw New ArgumentException("The pseudo count value of the log transform should be a positive value!")
            End If
            If DEPopulationSize < 4 Then
                DEPopulationSize = 4
            End If
            If DEMaxIterations < 1 Then
                DEMaxIterations = 1
            End If
            If LBFGSIterations < 1 Then
                LBFGSIterations = 1
            End If
            If Precise <= 0 Then
                Precise = 0.1
            End If
            If SmoothWindow > 0 AndAlso SmoothWindow < 3 Then
                SmoothWindow = 0
            ElseIf SmoothWindow >= 3 AndAlso SmoothWindow Mod 2 = 0 Then
                SmoothWindow += 1
            End If
            If PriorPenalty < 0 Then
                PriorPenalty = 0
            End If
            If PriorSignPenalty < 0 Then
                PriorSignPenalty = 0
            End If
            If PriorRankBoost < 0 Then
                PriorRankBoost = 0
            End If
        End Sub

        Public Overrides Function ToString() As String
            Return $"S-system fitting: sparsity={Sparsity}, maxEdges={MaxEdgesPerGene}, alpha[{AlphaBounds.Min},{AlphaBounds.Max}], g[{GBounds.Min},{GBounds.Max}], prior={If(UsePrior AndAlso Not Prior Is Nothing, Prior.EdgeCount().ToString, "off")}"
        End Function
    End Class
End Namespace
