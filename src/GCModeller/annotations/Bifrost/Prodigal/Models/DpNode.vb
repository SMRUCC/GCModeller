''' <summary>
''' DP节点（用于动态规划选基因）
''' </summary>
Public Class DpNode
    ''' <summary>位置（基因组坐标）</summary>
    Public Property Position As Integer

    ''' <summary>到该位置的最优累积得分</summary>
    Public Property Score As Double

    ''' <summary>前驱节点索引</summary>
    Public Property PrevNode As Integer = -1

    ''' <summary>关联的ORF索引（-1表示非基因节点）</summary>
    Public Property OrfIndex As Integer = -1
End Class