Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.BNLearn.DBN
Imports SMRUCC.genomics.Analysis.BNLearn.ModularNetwork.WGCNA

Namespace ModularNetwork

    ' ==================== 基于 WGCNA 共表达模块的 DBN 子网络训练 + 全局级联虚拟扰动 ====================
    '
    ' 针对大型 WGCNA 网络，全局 DBN 训练与虚拟扰动极慢。本组函数实现"分而治之"优化：
    '   1) 按 WGCNA 模块划分（GeneModuleColor）将时间序列划分为模块基因子块；
    '   2) 对每个模块单独训练一个 DynamicBayesianNetwork 子网络（模块内定向边取自合并先验）；
    '   3) 基于模块 eigengene 轨迹相关度构建模块间关联图；
    '   4) 对每个显式指定的扰动基因，在其所属模块内固定 Low 并沿模块关联图做级联推演，
    '      汇总得到全局虚拟扰动响应，并导出结果文件。
    ' 与 TrainAndIntervene（全局高斯 BN）相比，单模块规模远小于全局，训练代价由 O(N^2·样本)
    ' 降为 Σ O(模块规模^2·样本)，在大型 WGCNA 网络下收益显著。

    ''' <summary>
    ''' 单个 WGCNA 模块的 DBN 子网络训练结果容器（模块内部使用，不污染公共 API）。
    ''' </summary>
    Public Class ModuleDBN
        ''' <summary>WGCNA 模块颜色标签</summary>
        Public Property ModuleColor As String
        ''' <summary>该模块参与训练的基因名（与 timeSeries 子矩阵对齐）</summary>
        Public Property Genes As String()
        ''' <summary>已构建拓扑并完成参数学习的动态贝叶斯网络子网络</summary>
        Public Property Net As DynamicBayesianNetwork
        ''' <summary>模块 eigengene 轨迹：各时间点的模块基因均值向量（用于模块间相关度）</summary>
        Public Property Eigengene As Double()
        ''' <summary>模块内基因名 → 在 Genes 数组中的索引</summary>
        Public Property GeneIndex As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

        ''' <summary>
        ''' 该模块各基因的**野生型（未受扰动）表达丰度**：基因 ID → 丰度值。
        ''' 
        ''' 由训练流程自动计算（各基因在时间序列上的中位数，对 dropout 零值与
        ''' 异常值稳健），作为虚拟扰动推演的初始状态与响应参照基准。早期实现把初始
        ''' 状态硬编码为"全部 Medium"，导致未受扰动影响的基因一律输出 Medium。
        ''' 可由 BlockBayesianNetwork.SetWildtypeBaseline 覆盖。
        ''' </summary>
        Public Property WildtypeAbundance As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)
    End Class

    Public Module BlockModules

        ''' <summary>
        ''' 按 WGCNA 模块划分将基因分组（跳过 grey 模块，仅保留出现在 timeSeries 中的基因）。
        ''' </summary>
        <Extension>
        Public Function SplitModules(assignment As IEnumerable(Of GeneModuleColor), timeSeries As Core.GeneExpressionData) As Dictionary(Of String, String())
            Dim result As New Dictionary(Of String, List(Of String))
            Dim present As New HashSet(Of String)(timeSeries.GeneNames, StringComparer.OrdinalIgnoreCase)

            For Each mc In assignment
                If String.Equals(mc.moduleColor, "grey", StringComparison.OrdinalIgnoreCase) Then Continue For
                If Not present.Contains(mc.geneID) Then Continue For
                If Not result.ContainsKey(mc.moduleColor) Then
                    result(mc.moduleColor) = New List(Of String)
                End If
                If Not result(mc.moduleColor).Contains(mc.geneID) Then
                    result(mc.moduleColor).Add(mc.geneID)
                End If
            Next

            Return result.ToDictionary(Function(kv) kv.Key, Function(kv) kv.Value.ToArray())
        End Function

        ''' <summary>
        ''' 从合并先验网络中筛选两端都属于当前模块基因的定向边，转为 DBN 拓扑所需的
        ''' <see cref="RegulatoryLink"/> 集合。调控方向沿用 prior 的 RegulationType；
        ''' 若某模块无任何模块内先验边，返回空集合（DBN 退化为无父节点拓扑，仅学习自身时序分布）。
        ''' </summary>
        Public Function BuildModuleRegulatoryLinks(prior As Core.PriorNetwork, moduleGenes As String()) As RegulatoryLink()
            Dim inModule As New HashSet(Of String)(moduleGenes, StringComparer.OrdinalIgnoreCase)
            Dim links As New List(Of RegulatoryLink)

            For Each e As RegulatoryEdge In prior.Edges
                If inModule.Contains(e.TF) AndAlso inModule.Contains(e.TargetGene) Then
                    ' 传递先验边上声明的调控方向（激活/抑制）：缺失它会导致网络中不存在
                    ' 抑制性调控，使激活得分恒为正、CPT 的 Low 分支不可达，
                    ' 虚拟扰动也就无法产生下调响应。
                    links.Add(New RegulatoryLink With {
                        .TF_id = e.TF,
                        .target_operon = e.TargetGene,
                        .regulate_genes = {e.TargetGene},
                        .effector = Nothing,
                        .RegulationType = e.RegulationType,
                        .Confidence = e.Confidence
                    })
                End If
            Next

            ' 方向分布诊断：抑制边为 0 即意味着方向信息在上游丢失
            Dim nActivate As Integer = links.Where(Function(l) l.RegulationType = Effector.Activator).Count()
            Dim nInhibit As Integer = links.Where(Function(l) l.RegulationType = Effector.Inhibitor).Count()
            Dim nUnknown As Integer = links.Where(Function(l) l.RegulationType = Effector.Unknown).Count()

            Call $"[GRN links] 模块内定向边={links.Count}（激活={nActivate}, 抑制={nInhibit}, 未知={nUnknown}）".info

            Return links.ToArray
        End Function

        ''' <summary>
        ''' 计算模块 eigengene 轨迹：各时间点上模块基因均值（顺序与 timeSeries 一致）。
        ''' 时间点不足时返回长度为 1 的均值向量，关联图仍可工作（相关退化为常数）。
        ''' </summary>
        <Extension>
        Public Function ComputeModuleEigengene(ts As IReadOnlyCollection(Of Dictionary(Of String, Double))) As Double()
            If ts Is Nothing OrElse ts.Count = 0 Then Return {0.0}
            Dim nT = ts.Count
            Dim vec As New List(Of Double)
            For t = 0 To nT - 1
                Dim frame = ts(t)
                If frame Is Nothing OrElse frame.Count = 0 Then
                    vec.Add(0.0)
                    Continue For
                End If
                Dim avg = frame.Values.Average()
                vec.Add(avg)
            Next
            Return vec.ToArray()
        End Function

        ''' <summary>
        ''' 基于模块 eigengene 轨迹的 Pearson 相关构建模块间关联图（邻接表，权重 = |cor|）。
        ''' 仅保留 |cor| 超过阈值的双向关联。
        ''' </summary>
        Public Function BuildModuleCorrelationGraph(modules As IReadOnlyCollection(Of ModuleDBN), threshold As Double) As Dictionary(Of String, List(Of (modColor As String, weight As Double)))
            Dim graph As New Dictionary(Of String, List(Of (String, Double)))

            For Each m In modules
                graph(m.ModuleColor) = New List(Of (String, Double))
            Next

            For i = 0 To modules.Count - 1
                For j = i + 1 To modules.Count - 1
                    Dim c = Pearson(modules(i).Eigengene, modules(j).Eigengene)
                    If Math.Abs(c) > threshold Then
                        graph(modules(i).ModuleColor).Add((modules(j).ModuleColor, c))
                        graph(modules(j).ModuleColor).Add((modules(i).ModuleColor, c))
                    End If
                Next
            Next

            Return graph
        End Function

        ''' <summary>
        ''' 按每个基因在时间序列上的经验分布计算分位数阈值，写入网络的 per-node 离散化阈值。
        ''' 
        ''' 时间序列通常是原始 log1p 表达值（量级 0~10+），而 DBN 的默认阈值（0.33 / 0.66）
        ''' 是按"已归一化到 [0,1]"的数据设计的：log1p(x)=0.66 仅对应原始 count≈0.94，
        ''' 直接使用会让几乎所有有表达的基因都被判为 High，使学习到的 CPT 与推理证据都偏向 High。
        ''' 用分位数自适应阈值可以保证 Low / Medium / High 三态在数据侧获得合理比例。
        ''' </summary>
        <Extension>
        Public Sub ApplyQuantileThresholds(net As DynamicBayesianNetwork, matrix As Core.GeneExpressionData)
            If net Is Nothing OrElse matrix Is Nothing Then Return
            If matrix.NGene <= 0 OrElse matrix.NSample <= 0 Then Return

            Dim thresholds As Dictionary(Of String, Tuple(Of Double, Double)) = net.Config.NodeThresholds
            Dim qLow As Double = net.Config.QuantileLow
            Dim qHigh As Double = net.Config.QuantileHigh
            Dim nLow As Integer = 0
            Dim nMid As Integer = 0
            Dim nHigh As Integer = 0

            thresholds.Clear()

            For i As Integer = 0 To matrix.NGene - 1
                Dim gene As String = matrix.GeneNames(i)
                Dim col(matrix.NSample - 1) As Double

                For j As Integer = 0 To matrix.NSample - 1
                    col(j) = matrix.Matrix(i, j)
                Next

                Array.Sort(col)

                Dim low As Double = Quantile(col, qLow)
                Dim high As Double = Quantile(col, qHigh)
                Dim vmin As Double = col(0)
                Dim vmax As Double = col(col.Length - 1)

                ' 离散化用的是"严格小于"：若 low 恰好等于数据最小值，则最小值永远判不到 Low
                ' （单细胞 log1p 数据里大量基因存在 dropout 零值，33% 分位数常常就是 0，
                '  这会让 Low 态在数据侧完全消失）
                If low <= vmin Then
                    low = vmin + (Math.Abs(vmin) * 0.001 + 0.001)
                End If

                ' 同理，high 等于最大值时最大值判不到 High
                If high >= vmax Then
                    high = vmax - (Math.Abs(vmax) * 0.001 + 0.001)
                End If

                ' 数据过于集中导致区间退化时，直接以最小/最大值为界划分三态
                If high <= low Then
                    low = vmin + (Math.Abs(vmin) * 0.001 + 0.001)
                    high = vmax - (Math.Abs(vmax) * 0.001 + 0.001)

                    If high <= low Then
                        high = low + 0.001
                    End If
                End If

                thresholds(gene) = New Tuple(Of Double, Double)(low, high)

                For j As Integer = 0 To col.Length - 1
                    Dim x As Double = col(j)

                    If x < low Then
                        nLow += 1
                    ElseIf x < high Then
                        nMid += 1
                    Else
                        nHigh += 1
                    End If
                Next
            Next

            Dim total As Integer = nLow + nMid + nHigh

            If total > 0 Then
                Call $"[GRN thres] 基因数={matrix.NGene}, 离散化三态占比: Low={100.0 * nLow / total:F1}%, Medium={100.0 * nMid / total:F1}%, High={100.0 * nHigh / total:F1}%".info
            End If
        End Sub

        ''' <summary>
        ''' 用模块内的时间序列表达数据，重新推断每条调控边的方向（激活 / 抑制）。
        ''' 
        ''' WGCNA 先验网络的权重是非负的共表达强度（|cor| 的软阈值变换），**不含方向符号**；
        ''' 伪速率先验也只生成激活边。因此若完全依赖先验，网络中将 100% 是激活边，
        ''' 激活得分恒为正、CPT 的 Low 分支不可达，虚拟扰动也就无法产生任何下调响应。
        ''' 
        ''' 这里改由表达数据本身推断方向：按 2TBN 的时序因果语义，取
        ''' TF[t] 与 target[t+1] 的**滞后相关**，正相关判为激活、负相关判为抑制。
        ''' </summary>
        <Extension>
        Public Sub InferRegulationDirections(links As RegulatoryLink(), matrix As Core.GeneExpressionData)
            If links Is Nothing OrElse links.Length = 0 OrElse matrix Is Nothing Then Return
            If matrix.NGene <= 0 OrElse matrix.NSample < 3 Then Return

            Dim vectors As New Dictionary(Of String, Double())

            For i As Integer = 0 To matrix.NGene - 1
                Dim v(matrix.NSample - 1) As Double

                For j As Integer = 0 To matrix.NSample - 1
                    v(j) = matrix.Matrix(i, j)
                Next

                vectors(matrix.GeneNames(i)) = v
            Next

            Dim nInhibit As Integer = 0
            Dim nResolved As Integer = 0

            For Each l In links
                Dim tf As Double() = Nothing
                Dim target As Double() = Nothing

                If Not vectors.TryGetValue(l.TF_id, tf) Then Continue For
                If Not vectors.TryGetValue(l.target_operon, target) Then Continue For

                Dim r As Double = DifferencedLaggedCorrelation(tf, target)

                If Double.IsNaN(r) Then Continue For

                nResolved += 1

                If r < 0 Then
                    l.RegulationType = Effector.Inhibitor
                    nInhibit += 1
                Else
                    l.RegulationType = Effector.Activator
                End If
            Next

            Call $"[GRN dir] 由表达数据推断调控方向: 可判定={nResolved}/{links.Length}, 抑制={nInhibit}".info
        End Sub

        ''' <summary>
        ''' 差分滞后相关：先对两条序列做一阶差分（去除伪时间轨迹的共同趋势），
        ''' 再取 cor(Δx[t], Δy[t+1])。
        ''' 
        ''' 直接用原始序列的滞后相关会被共同趋势主导：细胞沿伪时间连续变化，
        ''' 相邻 bin 高度相似，几乎所有基因对的滞后相关都为正（实测 100% 为正，
        ''' 完全无法区分激活与抑制）。差分后保留的是同步波动，才能反映真实的耦合方向。
        ''' </summary>
        Private Function DifferencedLaggedCorrelation(x As Double(), y As Double()) As Double
            Dim n As Integer = Math.Min(x.Length, y.Length)

            If n < 4 Then Return Double.NaN

            Dim m As Integer = n - 1
            Dim dx(m - 1) As Double
            Dim dy(m - 1) As Double

            For i As Integer = 0 To m - 1
                dx(i) = x(i + 1) - x(i)
                dy(i) = y(i + 1) - y(i)
            Next

            ' 再滞后一阶：cor(dx[0..m-2], dy[1..m-1])
            Dim k As Integer = m - 1

            If k < 2 Then Return Double.NaN

            Dim ax(k - 1) As Double
            Dim ay(k - 1) As Double

            For i As Integer = 0 To k - 1
                ax(i) = dx(i)
                ay(i) = dy(i + 1)
            Next

            Return Pearson(ax, ay)
        End Function

        ''' <summary>
        ''' 滞后 Pearson 相关 cor(x[0..n-2], y[1..n-1])：
        ''' 刻画"上游 t 时刻状态 → 下游 t+1 时刻状态"的时序关联（2TBN 语义）。
        ''' </summary>
        Private Function LaggedCorrelation(x As Double(), y As Double()) As Double
            Dim n As Integer = Math.Min(x.Length, y.Length) - 1

            If n < 2 Then Return Double.NaN

            Dim mx As Double = 0
            Dim my As Double = 0

            For i As Integer = 0 To n - 1
                mx += x(i)
                my += y(i + 1)
            Next

            mx /= n
            my /= n

            Dim num As Double = 0
            Dim dx As Double = 0
            Dim dy As Double = 0

            For i As Integer = 0 To n - 1
                Dim a As Double = x(i) - mx
                Dim b As Double = y(i + 1) - my

                num += a * b
                dx += a * a
                dy += b * b
            Next

            If dx <= 0 OrElse dy <= 0 Then Return 0.0

            Return num / Math.Sqrt(dx * dy)
        End Function

        ''' <summary>
        ''' 计算每个基因的野生型表达丰度：取该基因在全部时间点上的**中位数**。
        ''' 
        ''' 用中位数而非均值，是因为单细胞 log1p 表达存在大量 dropout 零值与右尾异常值，
        ''' 中位数刻画的是"典型表达水平"，对它们稳健；均值会被异常高表达拉偏。
        ''' </summary>
        Public Function ComputeWildtypeAbundance(timeSeries As List(Of Dictionary(Of String, Double)), genes As String()) As Dictionary(Of String, Double)
            Dim result As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

            If timeSeries Is Nothing OrElse genes Is Nothing Then Return result

            Dim series As New Dictionary(Of String, List(Of Double))(StringComparer.OrdinalIgnoreCase)

            For Each frame In timeSeries
                If frame Is Nothing Then Continue For

                For Each kv In frame
                    If Not series.ContainsKey(kv.Key) Then
                        series(kv.Key) = New List(Of Double)()
                    End If

                    series(kv.Key).Add(kv.Value)
                Next
            Next

            For Each g In genes
                Dim values As List(Of Double) = Nothing

                If Not series.TryGetValue(g, values) OrElse values.Count = 0 Then
                    Continue For
                End If

                result(g) = Median(values)
            Next

            Return result
        End Function

        ''' <summary>中位数（对升序排序后的数组取中值，偶数个元素时取中间两个的均值）</summary>
        Private Function Median(values As List(Of Double)) As Double
            If values Is Nothing OrElse values.Count = 0 Then Return 0.0

            Dim col As Double() = values.ToArray()

            Array.Sort(col)

            Dim n As Integer = col.Length

            If n Mod 2 = 1 Then
                Return col(n \ 2)
            End If

            Return (col(n \ 2 - 1) + col(n \ 2)) / 2.0
        End Function

        ''' <summary>取已升序排序数组的分位数（最近秩法）</summary>
        Private Function Quantile(sorted As Double(), q As Double) As Double
            If sorted Is Nothing OrElse sorted.Length = 0 Then Return 0.0

            Dim idx As Integer = CInt(Math.Floor(q * (sorted.Length - 1)))

            If idx < 0 Then idx = 0
            If idx > sorted.Length - 1 Then idx = sorted.Length - 1

            Return sorted(idx)
        End Function

        ''' <summary>
        ''' Pearson 相关（对不等长序列按较短者截断），用于模块 eigengene 轨迹相关度。
        ''' </summary>
        Private Function Pearson(x As Double(), y As Double()) As Double
            Dim n = Math.Min(x.Length, y.Length)
            If n < 2 Then Return 0
            Dim mx = x.Take(n).Average()
            Dim my = y.Take(n).Average()
            Dim num As Double = 0, dx As Double = 0, dy As Double = 0
            For i = 0 To n - 1
                Dim a = x(i) - mx
                Dim b = y(i) - my
                num += a * b
                dx += a * a
                dy += b * b
            Next
            If dx = 0 OrElse dy = 0 Then Return 0
            Return num / Math.Sqrt(dx * dy)
        End Function
    End Module
End Namespace