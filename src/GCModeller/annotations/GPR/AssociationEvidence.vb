''' <summary>
''' 基因-反应关联打分中所使用的证据类型。
''' 
''' 每一种证据都代表一条独立的推断依据，最终分数由 <see cref="EvidenceAggregator"/> 聚合得到，
''' 因此任何一个分数都可以被完整地追溯到"它是由哪些证据推导出来的"。
''' </summary>
Public Enum EvidenceKind

    ''' <summary>
    ''' 基因自身携带的 EC 编号与参考反应网络中的反应 EC 编号直接匹配
    ''' </summary>
    DirectEC
    ''' <summary>
    ''' 位于同一个潜在操纵子内部的邻居基因所携带的反应
    ''' </summary>
    OperonContext
    ''' <summary>
    ''' 位于物理位置滑动窗口内的邻居基因所携带的反应
    ''' </summary>
    WindowContext
    ''' <summary>
    ''' 潜在多亚基酶复合体的其它成员所携带的反应
    ''' </summary>
    EnzymeComplex
    ''' <summary>
    ''' 多结构域融合酶（一个基因对应多个 EC 编号），其多个 EC 在通路中形成连续步骤
    ''' </summary>
    FusionGene
    ''' <summary>
    ''' 表达数据中与当前基因共表达的基因所携带的反应
    ''' </summary>
    Coexpression
    ''' <summary>
    ''' 跨物种保守基因邻接（共线性）中所记录的功能
    ''' </summary>
    ConservedSynteny
    ''' <summary>
    ''' 当前基因所支持的反应在其所属通路中的覆盖完整程度
    ''' </summary>
    PathwayCompleteness
    ''' <summary>
    ''' 当前基因所支持的反应之间存在"产物即下游底物"的化学连续关系
    ''' </summary>
    ReactionContinuity

End Enum

''' <summary>
''' 单条关联证据。
''' 
''' 分数的最终取值为 noisy-OR 聚合（见 <see cref="EvidenceAggregator"/>），
''' 其中每一条证据的贡献为 <see cref="Weight"/> * <see cref="RawScore"/>。
''' </summary>
Public Class AssociationEvidence

    ''' <summary>
    ''' 证据类型
    ''' </summary>
    Public Property Kind As EvidenceKind

    ''' <summary>
    ''' 该类证据在本参数配置下的最大权重，取值范围 [0, 1]，来源：<see cref="GPRParameters"/>
    ''' </summary>
    Public Property Weight As Double

    ''' <summary>
    ''' 本次证据的原始强度，取值范围 [0, 1]（例如距离衰减系数、相关系数、通路完整度）
    ''' </summary>
    Public Property RawScore As Double

    ''' <summary>
    ''' 证据来源的可读描述，例如邻居基因的 locus_id、通路编号或者相关系数
    ''' </summary>
    Public Property Source As String

    ''' <summary>
    ''' 这条证据对最终分数的实际贡献：``Weight * RawScore``，取值范围 [0, 1]
    ''' </summary>
    Public ReadOnly Property Contribution As Double
        Get
            Return Normalize(Weight) * Normalize(RawScore)
        End Get
    End Property

    Friend Shared Function Normalize(x As Double) As Double
        If Double.IsNaN(x) Then Return 0
        If x < 0 Then Return 0
        If x > 1 Then Return 1
        Return x
    End Function

    Public Overrides Function ToString() As String
        Return $"{Kind}({RawScore.ToString("F3")}x{Weight.ToString("F3")}) <- {Source}"
    End Function

End Class

''' <summary>
''' 一条"反应 ID + 证据"的组合，是所有证据分析器的统一输出单元
''' </summary>
Public Structure ReactionEvidence

    Public Property ReactionID As String
    Public Property Evidence As AssociationEvidence

    Sub New(reactionID As String, evidence As AssociationEvidence)
        Me.ReactionID = reactionID
        Me.Evidence = evidence
    End Sub

End Structure
