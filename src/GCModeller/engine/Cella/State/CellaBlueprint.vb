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
