' ============================================================
' CellaBlueprint.vb - 虚拟细胞蓝图
' ============================================================
' 轻量构建入口 VirtualCella.BuildFrom 的输入契约。
'
' 一份蓝图 = 「先验调控网络 + 基线表达矩阵 + 代谢反应网络 + 耦合映射 +
' 动力学参数」，CellaFactory 据此装配出六个子网络。
'
' 注意：GEARS 的图神经网络与 Metaboliq 的液态网络都需要训练，而训练开销
' 不应该随细胞数量线性增长。因此蓝图持有：
'   * Gears            —— 预训练好的共享 GNN（推理无副作用，可跨细胞共享）
'   * MetabolicTemplate—— 预训练好的液态网络模板（逐细胞复制参数）
' 二者为 Nothing 时，Factory 会用未训练（随机初始化）的模型构建，此时
' 动力学只反映随机初值，适合做结构自检而非生物学解释。
' ============================================================

Imports Microsoft.VisualBasic.DeepLearning.LiquidNeuralNetwork
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.GEARS
Imports SMRUCC.genomics.Analysis.Metaboliq
Imports SMRUCC.genomics.MetabolicModel

Public Class CellaBlueprint

    ' ==================== 转录调控网络（GEARS 图神经网络）====================

    ''' <summary>TF → 靶基因 的先验调控网络，直接作为 GNN 的图结构</summary>
    Public Property Prior As PriorNetwork

    ''' <summary>基线基因表达矩阵（行=基因，列=样本）；可用合成数据</summary>
    Public Property Expression As GeneExpressionData

    ''' <summary>GEARS 超参；为 Nothing 时使用默认配置</summary>
    Public Property GearsConfig As GEARSConfig

    ''' <summary>预训练好的 GEARS 模型；为 Nothing 时 Factory 会用未训练模型</summary>
    Public Property Gears As GEARS

    ''' <summary>转录因子基因 id 列表；为 Nothing 时从 <see cref="Prior"/> 的 TFNames 推断</summary>
    Public Property TFGenes As String()

    ' ==================== 代谢网络（Metaboliq 液态神经网络）====================

    ''' <summary>代谢反应网络</summary>
    Public Property Reactions As MetabolicReaction()

    ''' <summary>显式指定的胞外（边界）代谢物 id；为 Nothing 时由 Metaboliq 启发式判定</summary>
    Public Property ExplicitBoundary As String()

    ''' <summary>液态神经元模式，默认 LTC</summary>
    Public Property MetabolicMode As LiquidMode = LiquidMode.LTC

    ''' <summary>ODE 积分器，默认 rk4</summary>
    Public Property MetabolicSolver As String = "rk4"

    ''' <summary>τ 取值下界；代谢系统偏 stiff，默认放大到 2.0</summary>
    Public Property TauMin As Double = 2.0
    ''' <summary>τ 取值上界</summary>
    Public Property TauMax As Double = 60.0
    ''' <summary>Metaboliq ODE 子步长上限（显式 RK4 稳定域要求）</summary>
    Public Property MaxSubStep As Double = 1.0

    ''' <summary>预训练好的液态网络模板；逐细胞复制其参数</summary>
    Public Property MetabolicTemplate As MetabolicLiquidNetwork

    ''' <summary>由 <see cref="Reactions"/> 派生的拓扑（首次装配时计算并缓存，供所有细胞共享）</summary>
    Public Property MetabolicGraph As MetabolicNetworkGraph

    ''' <summary>代谢网络模板的训练损失历史</summary>
    Public Property MetabolicLoss As List(Of EpochLoss)

    ' ==================== 子系统耦合映射 ====================

    ''' <summary>反应 id → 催化该反应的基因 id（酶水平来源）</summary>
    Public Property ReactionGeneMap As Dictionary(Of String, String)

    ''' <summary>边界代谢物 id → 负责转运它的转运蛋白基因 id</summary>
    Public Property Transporters As Dictionary(Of String, String)

    ''' <summary>边界代谢物 id → 与之对应的胞内代谢物 id（用于把产物外排到环境中）</summary>
    Public Property Exporters As Dictionary(Of String, String)

    ''' <summary>代谢物效应物 id → 受其调控的转录因子基因 id</summary>
    Public Property Effectors As Dictionary(Of String, String)

    ''' <summary>信号通道名；为 Nothing 时取 <see cref="TFGenes"/></summary>
    Public Property SignalChannels As String()

    ''' <summary>信号通道 → 触发它的环境代谢物 id（环境刺激）</summary>
    Public Property ExternalStimuli As Dictionary(Of String, String)

    ''' <summary>
    ''' 信号通道 → { 靶基因 id → 权重 }。
    ''' 语义：外源生长因子通路的活性直接改写靶基因的输入 z 值与扰动标记，
    ''' 使 GEARS 能预报「生长因子 → 转录响应」；权重用于区分直接靶点与弱响应基因。
    ''' </summary>
    Public Property SignalGeneCoupling As Dictionary(Of String, Dictionary(Of String, Double))

    ' ==================== 生态位 / 分化 ====================

    ''' <summary>
    ''' 生态位信号通道名；为 Nothing 时使用 <see cref="DifferentiationSystem.StandardNicheChannels"/>
    ''' </summary>
    Public Property NicheChannels As String()

    ''' <summary>
    ''' 生态位 → 基因表达的调制表：生态位通道 → {基因 id → 权重}。
    ''' 语义：<c>target_gene *= 1 + Σ_c w(c,gene) · (niche(c) − 0.5)</c>，
    ''' 即生态位读数高于 / 低于中性值 0.5 时把该基因的表达设定点整体上移 / 下移。
    ''' </summary>
    Public Property NicheGeneCoupling As Dictionary(Of String, Dictionary(Of String, Double))

    ''' <summary>该细胞命运偏好的归一化径向位置（0 = 核心，1 = 表层）；负数表示不偏好</summary>
    Public Property PreferredRadius As Double = -1.0

    ''' <summary>可容忍的径向偏离（超出即触发位置校正重排）</summary>
    Public Property RadialTolerance As Double = 0.28

    ''' <summary>该命运允许的位置校正重排速率（每步概率，0 = 完全不动）</summary>
    Public Property RadialSortingRate As Double = 0.0

    ''' <summary>
    ''' 子代是否向外扩散。True 时若亲代所在格点已满，子代被放到还有空位的相邻格点，
    ''' 类器官/菌落得以向外生长；False 时子代必须与亲代同格点，格点填满即停止增殖。
    ''' </summary>
    Public Property DaughterDispersal As Boolean = False

    ''' <summary>显示名称（用于报告）</summary>
    Public Property DisplayName As String

    ''' <summary>该命运的分子标志基因</summary>
    Public Property Markers As String()

    ''' <summary>生态位信号通道列表</summary>
    Public Function GetNicheChannels() As String()
        If Not NicheChannels.IsNullOrEmpty Then
            Return NicheChannels
        End If

        Return DifferentiationSystem.StandardNicheChannels
    End Function

    ' ==================== 动力学参数 ====================

    ''' <summary>仿真时间步长</summary>
    Public Property TimeStep As Double = 1.0

    ''' <summary>mRNA 松弛到 GNN 预测目标的时间常数</summary>
    Public Property RnaRelaxationTau As Double = 5.0

    ''' <summary>翻译速率常数（按基因；缺失时用 <see cref="DefaultTranslationRate"/>）</summary>
    Public Property TranslationRate As Dictionary(Of String, Double)
    Public Property DefaultTranslationRate As Double = 0.05

    ''' <summary>蛋白质降解速率常数（按基因）</summary>
    Public Property ProteinDegradation As Dictionary(Of String, Double)
    Public Property DefaultProteinDegradation As Double = 0.05

    ''' <summary>生长稀释速率（对所有蛋白一视同仁）</summary>
    Public Property DilutionRate As Double = 0.01

    ''' <summary>mRNA 降解速率常数（按基因）</summary>
    Public Property MessengerDegradation As Dictionary(Of String, Double)
    Public Property DefaultMessengerDegradation As Double = 0.12

    ''' <summary>单位 mRNA 降解可回收的物质当量</summary>
    Public Property RecycleYieldRNA As Double = 0.01
    ''' <summary>单位蛋白质降解可回收的物质当量</summary>
    Public Property RecycleYieldProtein As Double = 0.005
    ''' <summary>回收池的消耗速率常数</summary>
    Public Property RecycleUseRate As Double = 0.3

    ''' <summary>回收物质注入的目标代谢物 id；为 Nothing 时不注入</summary>
    Public Property RecycleTargetMetabolite As String

    ''' <summary>酶水平归一化参考值：level = protein / (protein + reference) ∈ [0,1]</summary>
    Public Property EnzymeReference As Double = 1.0

    ''' <summary>
    ''' 转运系统的最大转运速率。
    ''' 注意：该值决定环境被消耗的快慢，取值过大会让培养基在几个时间步内耗尽。
    ''' </summary>
    Public Property TransportVmax As Double = 0.05
    ''' <summary>转运系统的半饱和常数</summary>
    Public Property TransportKm As Double = 0.5

    ' ==================== 信号转导参数 ====================

    ''' <summary>传感器激酶自磷酸化速率（按信号通道）</summary>
    Public Property KinaseAutophosphorylation As Double = 0.8
    ''' <summary>传感器激酶去磷酸化速率</summary>
    Public Property KinaseDephosphorylation As Double = 0.3
    ''' <summary>磷酸基团向响应调节因子的转移速率</summary>
    Public Property PhosphoTransferRate As Double = 0.5
    ''' <summary>响应调节因子去磷酸化速率</summary>
    Public Property RegulatorDephosphorylation As Double = 0.2
    ''' <summary>细胞周期蛋白合成速率</summary>
    Public Property CyclinSynthesis As Double = 0.15
    ''' <summary>细胞周期蛋白降解速率</summary>
    Public Property CyclinDegradation As Double = 0.1
    ''' <summary>细胞周期相位推进速率</summary>
    Public Property CycleAngularVelocity As Double = 0.25

    ' ==================== 细胞生命周期参数 ====================

    ''' <summary>物种 / 细胞类型标识（用于种群统计与交叉喂养归因）</summary>
    Public Property SpeciesName As String

    ''' <summary>触发二分裂所需的生物量阈值</summary>
    Public Property DivisionBiomassThreshold As Double = 2.0

    ''' <summary>单位「平均比通量」贡献的生物量系数</summary>
    Public Property BiomassYieldPerFlux As Double = 0.02

    ''' <summary>单位「平均蛋白水平」贡献的生物量系数</summary>
    Public Property BiomassYieldPerProtein As Double = 0.01

    ''' <summary>分裂所需的最小年龄（避免刚出生就分裂）</summary>
    Public Property MinDivisionAge As Double = 3.0

    ''' <summary>子代继承亲代各状态池的比例（0.5 即各得一半）</summary>
    Public Property DaughterStateFraction As Double = 0.5

    ''' <summary>最大寿命；超过即老化死亡</summary>
    Public Property MaxCellAge As Double = 200.0

    ''' <summary>判定为「饥饿」的营养指标阈值</summary>
    Public Property StarvationThreshold As Double = 0.05

    ''' <summary>连续饥饿多少个时间步之后死亡</summary>
    Public Property StarvationDeathTicks As Integer = 25

    ''' <summary>单个 Spot 的承载上限（达到上限时分裂被抑制）</summary>
    Public Property MaxCellsPerSpot As Integer = 8

    ' ==================== 鞭毛运动参数 ====================

    ''' <summary>鞭毛结构蛋白基因 id（决定运动能力）</summary>
    Public Property FlagellarGenes As String()

    ''' <summary>趋化受体基因 id（决定营养梯度偏置的强度）</summary>
    Public Property ChemotaxisGenes As String()

    ''' <summary>每个时间步的基础迁移概率（乘以运动能力之后生效）</summary>
    Public Property MotilityBaseProbability As Double = 0.15

    ''' <summary>营养梯度偏置强度（叠加在随机游走之上）</summary>
    Public Property MotilityGradientBias As Double = 1.5

    ''' <summary>营养梯度归一化尺度</summary>
    Public Property MotilityGradientScale As Double = 1.0

    ''' <summary>运动能力参考蛋白水平：motility = p / (p + reference) ∈ [0,1]</summary>
    Public Property MotilityReference As Double = 1.0

    ''' <summary>
    ''' 用于计算 Spot 营养指标的代谢物 id 集合；为 Nothing 时取该 Spot 培养基全部键之和
    ''' </summary>
    Public Property NutrientMetabolites As String()

    ' ==================== 环境扩散参数 ====================

    ''' <summary>Spot 之间的 Fickian 扩散系数（0 = 不扩散）</summary>
    Public Property DiffusionCoefficient As Double = 0.05

    ''' <summary>按代谢物覆盖的扩散系数（未登记者用 <see cref="DiffusionCoefficient"/>）</summary>
    Public Property DiffusionByMetabolite As Dictionary(Of String, Double)

    ''' <summary>贴壁格点的补料速率（0 = 不补料），模拟恒化器 / 补料发酵</summary>
    Public Property BoundaryFeedRate As Double = 0.0

    ''' <summary>参与补料的代谢物 id 集合</summary>
    Public Property BoundaryFeedMetabolites As String()

    ''' <summary>补料将浓度拉回的增量比例（按营养指标的相对缺口计算）</summary>
    Public Property BoundaryFeedLevel As Double = 1.0

    ' ==================== 派生属性 ====================

    ''' <summary>基因列表（取自基线表达矩阵的行名）</summary>
    Public ReadOnly Property Genes As String()
        Get
            If Expression Is Nothing Then
                Return {}
            End If

            Return Expression.GeneNames
        End Get
    End Property

    ''' <summary>转录因子基因列表</summary>
    Public Function GetTFGenes() As String()
        If Not TFGenes.IsNullOrEmpty Then
            Return TFGenes
        End If
        If Prior Is Nothing Then
            Return {}
        End If

        Dim genes As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        For Each edge In Prior.Edges.SafeQuery
            genes.Add(edge.TF)
        Next

        Return genes.ToArray()
    End Function

    ''' <summary>信号通道列表</summary>
    Public Function GetSignalChannels() As String()
        If Not SignalChannels.IsNullOrEmpty Then
            Return SignalChannels
        End If

        Return GetTFGenes()
    End Function

    Public Function TranslationRateOf(gene As String) As Double
        Return RateOf(TranslationRate, gene, DefaultTranslationRate)
    End Function

    Public Function ProteinDegradationOf(gene As String) As Double
        Return RateOf(ProteinDegradation, gene, DefaultProteinDegradation)
    End Function

    Public Function MessengerDegradationOf(gene As String) As Double
        Return RateOf(MessengerDegradation, gene, DefaultMessengerDegradation)
    End Function

    Private Shared Function RateOf(table As Dictionary(Of String, Double), gene As String, defaultValue As Double) As Double
        If table Is Nothing Then
            Return defaultValue
        End If

        Dim v As Double = defaultValue

        If table.TryGetValue(gene, v) Then
            Return v
        End If

        Return defaultValue
    End Function

    ''' <summary>反应 id → 基因 id；未登记的反应返回 Nothing</summary>
    Public Function GeneOfReaction(reactionId As String) As String
        If ReactionGeneMap Is Nothing Then
            Return Nothing
        End If

        Dim gene As String = Nothing

        If ReactionGeneMap.TryGetValue(reactionId, gene) Then
            Return gene
        End If

        Return Nothing
    End Function

    Public Function TransporterOf(boundaryMetabolite As String) As String
        If Transporters Is Nothing Then
            Return Nothing
        End If

        Dim gene As String = Nothing

        If Transporters.TryGetValue(boundaryMetabolite, gene) Then
            Return gene
        End If

        Return Nothing
    End Function

    ''' <summary>边界代谢物对应的胞内代谢物（外排来源）；未登记返回 Nothing</summary>
    Public Function ExportSourceOf(boundaryMetabolite As String) As String
        If Exporters Is Nothing Then
            Return Nothing
        End If

        Dim source As String = Nothing

        If Exporters.TryGetValue(boundaryMetabolite, source) Then
            Return source
        End If

        Return Nothing
    End Function

    ' ==================== 生命周期 / 运动 / 扩散 查询 ====================

    ''' <summary>某个胞外代谢物的扩散系数（可被 <see cref="DiffusionByMetabolite"/> 覆盖）</summary>
    Public Function DiffusionOf(metabolite As String) As Double
        If DiffusionByMetabolite Is Nothing Then
            Return DiffusionCoefficient
        End If

        Dim d As Double = DiffusionCoefficient

        If DiffusionByMetabolite.TryGetValue(metabolite, d) Then
            Return d
        End If

        Return DiffusionCoefficient
    End Function

    ''' <summary>该代谢物是否参与 Spot 营养指标的计算</summary>
    Public Function IsNutrient(metabolite As String) As Boolean
        If NutrientMetabolites.IsNullOrEmpty Then
            ' 未指定时，培养基里的所有成分都算营养
            Return True
        End If

        Return NutrientMetabolites.Contains(metabolite, StringComparer.OrdinalIgnoreCase)
    End Function

    ''' <summary>该代谢物是否参与贴壁格点的补料</summary>
    Public Function IsFeedComponent(metabolite As String) As Boolean
        If BoundaryFeedRate <= 0 OrElse BoundaryFeedMetabolites.IsNullOrEmpty Then
            Return False
        End If

        Return BoundaryFeedMetabolites.Contains(metabolite, StringComparer.OrdinalIgnoreCase)
    End Function

    ''' <summary>
    ''' 一组基因的平均蛋白水平（鞭毛 / 趋化能力的度量）
    ''' </summary>
    Public Shared Function MeanProteinOf(state As CellularState, genes As String()) As Double
        If state Is Nothing OrElse genes.IsNullOrEmpty Then
            Return 0.0
        End If

        Dim sum As Double = 0.0
        Dim hits As Integer = 0

        For Each gene As String In genes
            Dim idx As Integer = -1

            If state.GeneIndex.TryGetValue(gene, idx) Then
                sum += state.Protein(idx)
                hits += 1
            End If
        Next

        If hits = 0 Then
            Return 0.0
        End If

        Return sum / hits
    End Function

    ''' <summary>
    ''' 蓝图自检
    ''' </summary>
    Public Function Validate() As Boolean
        If Prior Is Nothing Then
            Throw New InvalidOperationException("蓝图缺少先验调控网络 Prior")
        End If
        If Expression Is Nothing Then
            Throw New InvalidOperationException("蓝图缺少基线表达矩阵 Expression")
        End If
        If Reactions.IsNullOrEmpty Then
            Throw New InvalidOperationException("蓝图缺少代谢反应网络 Reactions")
        End If
        If TimeStep <= 0 Then
            Throw New InvalidOperationException("TimeStep 必须为正数")
        End If

        Return True
    End Function

End Class
