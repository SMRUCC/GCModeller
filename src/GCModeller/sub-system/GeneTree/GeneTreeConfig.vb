Imports System.Text

Namespace SMRUCC.genomics.Analysis.GeneTree

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
        ''' 收敛阈值：每一轮所有基因的相对变化量（<c>|Δx| / max(1,|x|)</c>）的最大值低于该值时判定收敛。
        ''' </summary>
        Public Property ConvergenceTolerance As Double = 0.0001

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
            Call sb.Append($"alpha={DampingAlpha}, iterations={MaxIterations}, population={PopulationSize}, ")
            Call sb.Append($"tol={ConvergenceTolerance}")

            Return sb.ToString()
        End Function

    End Class
End Namespace
