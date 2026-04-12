''' <summary>
''' 算法参数
''' </summary>
Public Class GPRParameters

    ''' <summary>
    ''' 操纵子内最大距离
    ''' </summary>
    ''' <returns></returns>
    Public Property MaxOperonDistance As Integer = 500
    ''' <summary>
    ''' 同操纵子奖励
    ''' </summary>
    ''' <returns></returns>
    Public Property SameOperonBonus As Double = 0.3
    ''' <summary>
    ''' 通路完整度阈值
    ''' </summary>
    ''' <returns></returns>
    Public Property PathwayCompletenessThreshold As Double = 0.7
    ''' <summary>
    ''' 上下文窗口大小（向上下游各看几个基因）
    ''' </summary>
    ''' <returns></returns>
    Public Property MaxWindowSpan As Integer = 10
    ''' <summary>
    ''' 最大物理距离阈值，超过此距离认为不在同一基因簇
    ''' </summary>
    ''' <returns></returns>
    Public Property MaxPhysicalDistance As Integer = 15000
    ''' <summary>
    ''' 基于上下文推断的基础分
    ''' </summary>
    ''' <returns></returns>
    Public Property BaseContextScore As Double = 0.5
    ''' <summary>
    ''' 直接EC匹配的满分
    ''' </summary>
    ''' <returns></returns>
    Public Property DirectMatchScore As Double = 1.0
    ''' <summary>
    ''' 同链权重
    ''' </summary>
    ''' <returns></returns>
    Public Property SameStrandWeight As Double = 1.0
    ''' <summary>
    ''' 异链权重
    ''' </summary>
    ''' <returns></returns>
    Public Property DiffStrandWeight As Double = 0.3
    ''' <summary>
    ''' 通路中允许的最大反应间隔
    ''' </summary>
    ''' <returns></returns>
    Public Property MaxGapInPathway As Integer = 3

End Class