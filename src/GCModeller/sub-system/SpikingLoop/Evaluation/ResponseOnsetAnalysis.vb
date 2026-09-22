' ============================================================================
' ResponseOnsetAnalysis.vb — 扰动传播时序合理性（readme 五.4）
'
' readme 五.4："检查扰动信号是否按'上游先响应、下游延迟响应'的顺序传播"。
'
' 检验方法：
'   1. 以被扰动基因为起点，在<b>先验有向图</b>（TF→Target）上做 BFS，
'      得到每个基因的层级深度 depth（1 = 直接靶基因，2 = 靶基因的靶基因，…）；
'   2. 从虚拟扰动轨迹中提取每个基因的"响应起始时刻"onset
'      （首次 |Δ表达| 超过阈值的步号）；
'   3. 计算 depth 与 onset 的 Spearman 秩相关：
'        显著正相关 → 上游先响应、下游延迟响应（符合调控级联的因果结构）
'        接近 0     → 扰动在网络中近乎同时传播（可能权重归一化过强或泄漏过快）
'        显著负相关 → 下游反而先响应（网络动力学异常，需检查权重符号与循环连接）
'
' 注意：这里衡量的是"相对时序结构"，不是绝对时间——伪时间本身是相对量
' （readme 六 的风险清单明确提示不要做绝对时间的断言）。
' ============================================================================

Imports SMRUCC.genomics.Analysis.SpikingLoop.Graph
Imports SMRUCC.genomics.Analysis.SpikingLoop.Perturbation
Imports std = System.Math

Namespace Evaluation

    ''' <summary>扰动传播时序报告</summary>
    Public Class ResponseOnsetReport

        ''' <summary>被扰动（源头）基因</summary>
        Public Property SourceGene As String

        ''' <summary>每个基因的响应起始时刻（−1 = 全程未响应）</summary>
        Public Property Onsets As Integer()

        ''' <summary>每个基因相对源头的有向图层级深度（−1 = 不可达）</summary>
        Public Property Depths As Integer()

        ''' <summary>参与时序相关性计算的可达且已响应基因数</summary>
        Public Property GenesEvaluated As Integer

        ''' <summary>层级深度与响应时刻的 Spearman 相关（NaN = 样本不足）</summary>
        Public Property Spearman As Double

        ''' <summary>是否满足"上游先响应、下游延迟响应"</summary>
        Public Property UpstreamFirst As Boolean

        ''' <summary>按层级统计的平均响应时刻</summary>
        Public Property MeanOnsetByDepth As List(Of (depth As Integer, meanOnset As Double, genes As Integer))

        Public Overrides Function ToString() As String
            Return $"扰动 {SourceGene} 的传播时序：Spearman(depth, onset) = {Spearman:F4}，" &
                   $"上游先响应 = {UpstreamFirst}，评估基因数 = {GenesEvaluated}"
        End Function

    End Class

    ''' <summary>扰动传播时序分析</summary>
    Public Module ResponseOnsetAnalysis

        ''' <summary>
        ''' 分析一次虚拟扰动的响应时序结构。
        ''' </summary>
        ''' <param name="trajectory">虚拟扰动轨迹</param>
        ''' <param name="graph">先验图（提供有向邻接矩阵，行 = 突触前，列 = 突触后）</param>
        ''' <param name="spec">被注入的扰动（用于确定 BFS 起点）</param>
        ''' <param name="threshold">响应判定阈值（|Δ表达|）</param>
        Public Function Evaluate(trajectory As PerturbationTrajectory, graph As PriorGraph,
                                 spec As PerturbationSpec,
                                 Optional threshold As Double = 0.01) As ResponseOnsetReport

            If trajectory Is Nothing Then Throw New ArgumentNullException(NameOf(trajectory))
            If graph Is Nothing Then Throw New ArgumentNullException(NameOf(graph))
            If spec Is Nothing Then Throw New ArgumentNullException(NameOf(spec))

            Dim n = graph.Units
            Dim depths = BreadthFirstDepths(graph, spec.GeneIndex, n)
            Dim onsets = trajectory.ResponseOnset(threshold)

            Dim report As New ResponseOnsetReport With {
                .SourceGene = spec.GeneName,
                .Onsets = onsets,
                .Depths = depths,
                .MeanOnsetByDepth = New List(Of (depth As Integer, meanOnset As Double, genes As Integer))()
            }

            ' ---- 仅对"可达且已响应"的基因做时序相关性 ----
            Dim depthList As New List(Of Double)()
            Dim onsetList As New List(Of Double)()
            For i = 0 To n - 1
                If depths(i) >= 1 AndAlso onsets(i) >= 0 Then
                    depthList.Add(depths(i))
                    onsetList.Add(onsets(i))
                End If
            Next

            report.GenesEvaluated = depthList.Count
            report.Spearman = If(depthList.Count >= 3,
                                 RegressionMetrics.Spearman(depthList.ToArray(), onsetList.ToArray()),
                                 Double.NaN)

            report.UpstreamFirst = If(Double.IsNaN(report.Spearman),
                                      FallsBackToDepthMeans(depths, onsets),
                                      report.Spearman > 0.0)

            ' ---- 按层级汇总平均响应时刻 ----
            Dim maxDepth = 0
            For i = 0 To n - 1
                If depths(i) > maxDepth Then maxDepth = depths(i)
            Next

            For d = 1 To maxDepth
                Dim sum = 0.0
                Dim c = 0
                For i = 0 To n - 1
                    If depths(i) = d AndAlso onsets(i) >= 0 Then
                        sum += onsets(i)
                        c += 1
                    End If
                Next
                If c > 0 Then
                    report.MeanOnsetByDepth.Add((d, sum / c, c))
                End If
            Next

            Return report
        End Function

        ''' <summary>
        ''' 在先验有向图上从 <paramref name="source"/> 出发做 BFS，返回每个基因的最短跳数
        ''' （源点 = 0，不可达 = −1）。
        ''' </summary>
        Private Function BreadthFirstDepths(graph As PriorGraph, source As Integer, n As Integer) As Integer()
            Dim depths(n - 1) As Integer
            For i = 0 To n - 1
                depths(i) = -1
            Next

            If source < 0 OrElse source >= n Then Return depths

            Dim a = graph.Adjacency.Data
            Dim queue As New Queue(Of Integer)()
            depths(source) = 0
            queue.Enqueue(source)

            While queue.Count > 0
                Dim i = queue.Dequeue()
                Dim off = i * n

                For j = 0 To n - 1
                    If a(off + j) <> 0.0 AndAlso depths(j) < 0 Then
                        depths(j) = depths(i) + 1
                        queue.Enqueue(j)
                    End If
                Next
            End While

            Return depths
        End Function

        ''' <summary>
        ''' 当相关样本不足时的兜底判据：深度 1 的平均响应时刻是否早于更深的层级。
        ''' </summary>
        Private Function FallsBackToDepthMeans(depths As Integer(), onsets As Integer()) As Boolean
            Dim first = 0.0
            Dim firstCount = 0
            Dim deeper = 0.0
            Dim deeperCount = 0

            For i = 0 To depths.Length - 1
                If onsets(i) < 0 Then Continue For
                If depths(i) = 1 Then
                    first += onsets(i)
                    firstCount += 1
                ElseIf depths(i) > 1 Then
                    deeper += onsets(i)
                    deeperCount += 1
                End If
            Next

            If firstCount = 0 OrElse deeperCount = 0 Then Return False
            Return (first / firstCount) <= (deeper / deeperCount)
        End Function

    End Module

End Namespace
