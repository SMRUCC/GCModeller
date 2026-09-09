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

        ''' <summary>
        ''' 束搜索展开阶段的并行度：0 或负数 = 取 <see cref="Environment.ProcessorCount"/>；
        ''' 1 = 完全串行（便于回归对比、排查，或资源受限环境降级）。
        ''' </summary>
        ''' <remarks>
        ''' 并行只改变吞吐、不改变结果：展开按「状态序 → 待分解物序 → 规则序」分块并行，
        ''' 产出的状态按序号有序归并后再去重与剪枝，因此与串行版本逐位一致。
        ''' </remarks>
        Public MaxDegreeOfParallelism As Int32 = 0

        ''' <summary>展开阶段实际使用的并行度（把 0/负数解析为处理器核数）。</summary>
        Public Function EffectiveParallelism() As Int32
            If MaxDegreeOfParallelism <= 0 Then
                Return Math.Max(1, Environment.ProcessorCount)
            End If
            Return Math.Max(1, MaxDegreeOfParallelism)
        End Function

        ''' <summary>
        ''' 少于这个工作单元数时直接串行展开：任务调度开销会超过并行收益
        ''' （典型情形是搜索第 1 层只有 1 个状态）。
        ''' </summary>
        Public Shared ReadOnly MinParallelUnits As Int32 = 64

    End Class
End Namespace