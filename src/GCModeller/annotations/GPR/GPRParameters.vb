''' <summary>
''' GPR 关联算法的参数集合。
''' 
''' 约定：所有参与打分的数值都必须来自这里，算法实现中不允许出现硬编码的魔法数字。
''' 每一项证据的贡献都是 ``Weight * RawScore``，其中 <c>Weight</c> 由本类提供，
''' <c>RawScore</c> 由具体的证据分析器根据实际情况（距离、相关系数、完整度等）计算。
''' </summary>
Public Class GPRParameters

#Region "直接证据"

    ''' <summary>
    ''' 基因 EC 编号与反应 EC 编号直接匹配时的证据权重。
    ''' 
    ''' 默认取 0.9 而不是 1.0：EC 注释本身存在错误标注的可能，因此保留 0.1 的不确定性，
    ''' 允许操纵子、共表达等其它独立证据把这个分数进一步提升到 1.0。
    ''' </summary>
    Public Property DirectMatchScore As Double = 0.9

#End Region

#Region "操纵子与上下文窗口"

    ''' <summary>
    ''' 操纵子内允许的最大基因间距
    ''' </summary>
    Public Property MaxOperonDistance As Integer = 500
    ''' <summary>
    ''' 滑动窗口跨度（向上下游各看几个基因）
    ''' </summary>
    Public Property MaxWindowSpan As Integer = 10
    ''' <summary>
    ''' 最大物理距离阈值，超过此距离的基因不再视为处于同一基因簇
    ''' </summary>
    Public Property MaxPhysicalDistance As Integer = 15000
    ''' <summary>
    ''' 同链权重
    ''' </summary>
    Public Property SameStrandWeight As Double = 1.0
    ''' <summary>
    ''' 异链权重
    ''' </summary>
    Public Property DiffStrandWeight As Double = 0.3
    ''' <summary>
    ''' 同操纵子奖励系数，操纵子上下文的权重为 ``BaseContextScore * (1 + SameOperonBonus)``
    ''' </summary>
    Public Property SameOperonBonus As Double = 0.3
    ''' <summary>
    ''' 基于基因物理邻接上下文推断的证据基础权重
    ''' </summary>
    Public Property BaseContextScore As Double = 0.5

#End Region

#Region "酶复合体"

    ''' <summary>
    ''' 复合体成员之间允许的最大物理距离
    ''' </summary>
    Public Property ComplexMaxDistance As Integer = 1000
    ''' <summary>
    ''' 构成复合体所需的最少基因数
    ''' </summary>
    Public Property MinComplexGenes As Integer = 2
    ''' <summary>
    ''' 酶复合体证据权重
    ''' </summary>
    Public Property BaseComplexScore As Double = 0.4

#End Region

#Region "融合基因"

    ''' <summary>
    ''' 融合基因证据的基础权重
    ''' </summary>
    Public Property FusionBaseScore As Double = 0.3
    ''' <summary>
    ''' 融合基因证据随通路连续性的增长量
    ''' </summary>
    Public Property FusionContinuityGain As Double = 0.5
    ''' <summary>
    ''' 触发融合基因分析所需的最少 EC 数目
    ''' </summary>
    Public Property FusionMinECNumbers As Integer = 2

#End Region

#Region "通路"

    ''' <summary>
    ''' 通路完整度阈值，只有覆盖率不低于该阈值时才产生完整度证据
    ''' </summary>
    Public Property PathwayCompletenessThreshold As Double = 0.7
    ''' <summary>
    ''' 通路完整度证据权重
    ''' </summary>
    Public Property PathwayCompletenessWeight As Double = 0.4
    ''' <summary>
    ''' 通路中允许的最大反应间隔（用于融合基因的通路连续性判定）
    ''' </summary>
    Public Property MaxGapInPathway As Integer = 3
    ''' <summary>
    ''' 反应连续性证据权重
    ''' </summary>
    Public Property ReactionContinuityWeight As Double = 0.35

#End Region

#Region "共表达"

    ''' <summary>
    ''' 判定共表达所需的相关系数阈值
    ''' </summary>
    Public Property CoexpressionThreshold As Double = 0.7
    ''' <summary>
    ''' 共表达证据权重
    ''' </summary>
    Public Property BaseCoexpressionScore As Double = 0.4
    ''' <summary>
    ''' 参与共表达推断的最大共表达基因数量，防止单个基因被过度"传染"
    ''' </summary>
    Public Property MaxCoexpressionPartners As Integer = 10

#End Region

#Region "保守共线性"

    ''' <summary>
    ''' 保守共线性证据权重
    ''' </summary>
    Public Property BaseSyntenyScore As Double = 0.6
    ''' <summary>
    ''' 计算保守簇相似度时向两侧扩展的基因数目
    ''' </summary>
    Public Property SyntenyClusterSize As Integer = 5
    ''' <summary>
    ''' 保守簇匹配所需的相似度阈值
    ''' </summary>
    Public Property SyntenySimilarityThreshold As Double = 0.7

#End Region

#Region "输出"

    ''' <summary>
    ''' 关联结果的最低置信度阈值，低于该分数的关联不会出现在结果中
    ''' </summary>
    Public Property ConfidenceThreshold As Double = 0.3
    ''' <summary>
    ''' 分数上限
    ''' </summary>
    Public Property ScoreCap As Double = 1.0
    ''' <summary>
    ''' 未映射 EC 的占位分数（只用于标记，不进入关联表）
    ''' </summary>
    Public Property UnmappedScore As Double = 0.2

#End Region

End Class
