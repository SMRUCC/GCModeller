Namespace Search

    ''' <summary>
    ''' 逆向路径搜索的运行参数。
    ''' </summary>
    Public Class SearchOptions

        ''' <summary>
        ''' 搜索策略：<c>beam</c> 束搜索（每层保留得分最高的若干状态），
        ''' <c>dfs</c> 深度优先（等价于束宽 1 的单路径枚举）。
        ''' </summary>
        Public Strategy As String = "beam"

        ''' <summary>
        ''' 束宽：每层最多保留多少个候选状态。越大召回越高、耗时越长。
        ''' </summary>
        Public BeamWidth As Int32 = 50

        ''' <summary>
        ''' 最大搜索深度，即路径最多允许的反应步数。
        ''' </summary>
        Public MaxDepth As Int32 = 6

        ''' <summary>
        ''' 收集到多少条完整路径后提前停止搜索。
        ''' </summary>
        Public MaxPaths As Int32 = 20

        ''' <summary>
        ''' 单条规则在单个分子上最多枚举多少个匹配位置，用于抑制组合爆炸。
        ''' </summary>
        Public MatchLimit As Int32 = 50

    End Class
End Namespace