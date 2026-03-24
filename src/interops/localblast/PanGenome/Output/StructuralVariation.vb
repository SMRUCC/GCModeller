''' <summary>
''' 结构变异类型枚举
''' </summary>
Public Enum SVType
    ''' <summary>
    ''' 缺失：该基因组相对于其他基因组缺少该基因家族
    ''' </summary>
    PAV_Absence
    ''' <summary>
    ''' 获得：该基因组相对于其他基因组特有该基因家族
    ''' </summary>
    PAV_Presence
    ''' <summary>
    ''' 拷贝数增加：相对于核心拷贝数增加
    ''' </summary>
    CNV_Gain
    ''' <summary>
    ''' 拷贝数减少：相对于核心拷贝数减少
    ''' </summary>
    CNV_Loss
    ''' <summary>
    ''' 共线性断裂（倒位/易位）
    ''' </summary>
    Collinearity_Break
End Enum

''' <summary>
''' 结构变异事件记录
''' </summary>
Public Class StructuralVariation : Inherits SVTable

    ''' <summary>
    ''' 如果是共线性断裂，记录断点信息
    ''' </summary>
    ''' <returns></returns>
    Public Property Breakpoint_Chromosome As String
    Public Property Breakpoint_Position As Integer

End Class