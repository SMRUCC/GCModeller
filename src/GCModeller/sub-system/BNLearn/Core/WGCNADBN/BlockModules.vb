Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Analysis.BNLearn.DBN

Namespace Core.WGCNADBN

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
    End Class

    Public Module BlockModules

        ''' <summary>
        ''' 按 WGCNA 模块划分将基因分组（跳过 grey 模块，仅保留出现在 timeSeries 中的基因）。
        ''' </summary>
        <Extension>
        Public Function SplitModules(assignment As GeneModuleColor(), timeSeries As Core.GeneExpressionData) As Dictionary(Of String, String())
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
        Public Function BuildModuleRegulatoryLinks(prior As Core.PriorNetwork, moduleGenes As String()) As IEnumerable(Of RegulatoryLink)
            Dim inModule As New HashSet(Of String)(moduleGenes, StringComparer.OrdinalIgnoreCase)
            Dim links As New List(Of RegulatoryLink)

            For Each e In prior.Edges
                If inModule.Contains(e.TF) AndAlso inModule.Contains(e.TargetGene) Then
                    links.Add(New RegulatoryLink With {
                        .TF_id = e.TF,
                        .target_operon = e.TargetGene,
                        .regulate_genes = {e.TargetGene},
                        .effector = Nothing
                    })
                End If
            Next

            Return links
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
        Public Function BuildModuleCorrelationGraph(modules As List(Of ModuleDBN), threshold As Double) As Dictionary(Of String, List(Of (modColor As String, weight As Double)))
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