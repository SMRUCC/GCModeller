Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.HTS.WGCNA

Namespace RegulationNetwork

    ''' <summary>
    ''' 单个 WGCNA 模块在构建过程中的统计信息
    ''' </summary>
    Public Class ModuleStatistics

        ''' <summary>WGCNA 模块名称（通常为模块颜色）</summary>
        Public Property moduleName As String = ""

        ''' <summary>模块基因数（规模控制截断前的原始基因数）</summary>
        Public Property geneCount As Integer

        ''' <summary>实际参与计算的基因数（规模控制截断后）</summary>
        Public Property usedGenes As Integer

        ''' <summary>通过 |cor| 阈值的候选边数</summary>
        Public Property candidateEdges As Integer

        ''' <summary>通过 FDR 阈值的边数</summary>
        Public Property fdrPassedEdges As Integer

        ''' <summary>被 DPI 判定为间接而去除的边数</summary>
        Public Property dpiRemovedEdges As Integer

        ''' <summary>被偏相关判定为间接而去除的边数</summary>
        Public Property partialRemovedEdges As Integer

        ''' <summary>由 TF 注释成功定向的边数</summary>
        Public Property directedEdges As Integer

        ''' <summary>无法定向、作为候选保留的边数（TF-TF / 非 TF-非 TF）</summary>
        Public Property undirectedEdges As Integer

        ''' <summary>得到 STRING 证据支持的边数</summary>
        Public Property stringSupportedEdges As Integer

        ''' <summary>仅由 STRING 新增的边数</summary>
        Public Property stringOnlyEdges As Integer

        ''' <summary>该模块的耗时（毫秒）</summary>
        Public Property elapsedMilliseconds As Double

        Public Overrides Function ToString() As String
            Return $"[{moduleName}] genes={geneCount}(used {usedGenes}), edges={directedEdges}+{undirectedEdges} undirected, " &
                   $"cand={candidateEdges}, fdr={fdrPassedEdges}, dpi-={dpiRemovedEdges}, partial-={partialRemovedEdges}, " &
                   $"string={stringSupportedEdges}+{stringOnlyEdges}, {elapsedMilliseconds}ms"
        End Function
    End Class

    ''' <summary>
    ''' 单个 WGCNA 模块的先验调控网络
    ''' </summary>
    ''' <remarks>
    ''' <see cref="moduleNetwork"/> 只含模块内部的调控边；
    ''' <see cref="crossModule"/> 含以本模块基因为上游（TF）、指向其他模块基因的跨模块边。
    ''' 这样每条跨模块边只归属一个模块，合并时不会重复。
    ''' </remarks>
    Public Class ModulePriorNetwork

        ''' <summary>WGCNA 模块名称</summary>
        ''' <returns></returns>
        Public Property moduleName As String = ""

        ''' <summary>该模块包含的基因 ID</summary>
        ''' <returns></returns>
        Public Property genes As String() = {}

        ''' <summary>模块内的先验调控网络</summary>
        ''' <returns></returns>
        Public Property moduleNetwork As PriorNetwork

        ''' <summary>以本模块基因为上游的跨模块先验调控网络</summary>
        ''' <returns></returns>
        Public Property crossModule As PriorNetwork

        ''' <summary>构建过程统计</summary>
        ''' <returns></returns>
        Public Property statistics As ModuleStatistics

        ''' <summary>
        ''' 本模块的全部调控边（模块内 + 跨模块）
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property allEdges As IEnumerable(Of RegulatoryEdge)
            Get
                If moduleNetwork Is Nothing AndAlso crossModule Is Nothing Then
                    Return {}
                End If
                If moduleNetwork Is Nothing Then
                    Return crossModule.Edges
                End If
                If crossModule Is Nothing Then
                    Return moduleNetwork.Edges
                End If

                Return moduleNetwork.Edges.Concat(crossModule.Edges)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{moduleName}: {If(moduleNetwork Is Nothing, 0, moduleNetwork.Edges.Count)} intra, " &
                   $"{If(crossModule Is Nothing, 0, crossModule.Edges.Count)} cross"
        End Function
    End Class

    ''' <summary>
    ''' 整条流水线的汇总统计
    ''' </summary>
    Public Class BuildSummary

        ''' <summary>参与分析的基因数（预处理之后）</summary>
        Public Property geneCount As Integer

        ''' <summary>样本数</summary>
        Public Property sampleCount As Integer

        ''' <summary>WGCNA 模块数</summary>
        Public Property moduleCount As Integer

        ''' <summary>调控边总数（含跨模块与无向候选边）</summary>
        Public Property totalEdges As Integer

        ''' <summary>由 TF 注释成功定向的边数</summary>
        Public Property directedEdges As Integer

        ''' <summary>无向候选边数</summary>
        Public Property undirectedEdges As Integer

        ''' <summary>跨模块边数</summary>
        Public Property crossModuleEdges As Integer

        ''' <summary>得到 STRING 支持的边数</summary>
        Public Property stringSupportedEdges As Integer

        ''' <summary>仅由 STRING 新增的边数</summary>
        Public Property stringOnlyEdges As Integer

        ''' <summary>被 DPI 去除的边数</summary>
        Public Property dpiRemovedEdges As Integer

        ''' <summary>被偏相关去除的边数</summary>
        Public Property partialRemovedEdges As Integer

        ''' <summary>总耗时（毫秒）</summary>
        Public Property elapsedMilliseconds As Double

        ''' <summary>各模块的统计信息</summary>
        Public Property moduleStatistics As ModuleStatistics() = {}

        Public Overrides Function ToString() As String
            Return $"GRN prior: {geneCount} genes x {sampleCount} samples, {moduleCount} modules, " &
                   $"{totalEdges} edges ({directedEdges} directed / {undirectedEdges} undirected / {crossModuleEdges} cross)"
        End Function
    End Class

    ''' <summary>
    ''' 基因表达调控先验网络的构建结果：按 WGCNA 模块拆分的 <see cref="PriorNetwork"/> 集合
    ''' </summary>
    ''' <remarks>
    ''' 每个 <see cref="ModulePriorNetwork"/> 持有一个 <see cref="PriorNetwork"/>，
    ''' 其 <see cref="PriorNetwork.Edges"/> 元素类型为 <see cref="RegulatoryEdge"/>。
    ''' 需要单一网络的下游算法（DBN / GNN）可直接调用 <see cref="ToPriorNetwork"/> 合并。
    ''' </remarks>
    Public Class GRNBuildResult

        ''' <summary>按 WGCNA 模块拆分的先验网络集合</summary>
        ''' <returns></returns>
        Public Property modules As ModulePriorNetwork() = {}

        ''' <summary>本次构建使用的 WGCNA 分析结果（含 TOM、模块划分、eigengene 等）</summary>
        ''' <returns></returns>
        Public Property wgcna As Result = Nothing

        ''' <summary>汇总统计</summary>
        ''' <returns></returns>
        Public Property summary As BuildSummary = Nothing

        ''' <summary>
        ''' 全部调控边（模块内 + 跨模块）
        ''' </summary>
        ''' <returns><see cref="RegulatoryEdge"/> 的扁平集合</returns>
        Public ReadOnly Property allEdges As IEnumerable(Of RegulatoryEdge)
            Get
                If modules Is Nothing Then
                    Return {}
                End If

                Return modules.SelectMany(Function(m) m.allEdges)
            End Get
        End Property

        ''' <summary>
        ''' 按模块名取先验网络
        ''' </summary>
        ''' <param name="moduleName">WGCNA 模块名称（颜色）</param>
        ''' <returns>模块不存在时返回 Nothing</returns>
        Default Public ReadOnly Property ByModule(moduleName As String) As ModulePriorNetwork
            Get
                If modules Is Nothing Then
                    Return Nothing
                End If

                Return modules.FirstOrDefault(Function(m) String.Equals(m.moduleName, moduleName, StringComparison.OrdinalIgnoreCase))
            End Get
        End Property

        ''' <summary>
        ''' 合并为单一的先验网络，供不需要分块的 DBN / GNN 调用方使用
        ''' </summary>
        ''' <returns>包含全部有向与候选边的 <see cref="PriorNetwork"/></returns>
        Public Function ToPriorNetwork() As PriorNetwork
            Dim merged As New PriorNetwork()

            For Each edge As RegulatoryEdge In allEdges
                merged.AddEdge(edge.TF, edge.TargetGene, edge.RegulationType, edge.Confidence, edge.Evidence)
            Next

            Return merged
        End Function

        ''' <summary>
        ''' 只保留指定证据标签的边
        ''' </summary>
        ''' <param name="tag">证据标签（见 <see cref="EvidenceTags"/>）</param>
        ''' <returns>过滤后的 <see cref="PriorNetwork"/></returns>
        Public Function FilterByEvidence(tag As String) As PriorNetwork
            Dim net As New PriorNetwork()

            For Each edge As RegulatoryEdge In allEdges.Where(Function(e) EvidenceTags.Has(e.Evidence, tag))
                net.AddEdge(edge.TF, edge.TargetGene, edge.RegulationType, edge.Confidence, edge.Evidence)
            Next

            Return net
        End Function

        ''' <summary>
        ''' 转换为 bnlearn 结构学习使用的白名单边（使用表达矩阵基因顺序的索引）
        ''' </summary>
        ''' <param name="geneNames">表达矩阵的基因名顺序</param>
        ''' <returns>(上游索引, 下游索引) 序列</returns>
        Public Function ToWhitelist(geneNames As String()) As IEnumerable(Of (Integer, Integer))
            Return ToPriorNetwork().ToWhitelist(geneNames)
        End Function

        ''' <summary>
        ''' 按置信度阈值过滤后合并为单一网络
        ''' </summary>
        ''' <param name="minConfidence">置信度下限</param>
        ''' <returns></returns>
        Public Function ToPriorNetwork(minConfidence As Double) As PriorNetwork
            Dim net As New PriorNetwork()

            For Each edge As RegulatoryEdge In allEdges.Where(Function(e) e.Confidence >= minConfidence)
                net.AddEdge(edge.TF, edge.TargetGene, edge.RegulationType, edge.Confidence, edge.Evidence)
            Next

            Return net
        End Function

        Public Overrides Function ToString() As String
            If summary Is Nothing Then
                Return $"GRNBuildResult: {If(modules Is Nothing, 0, modules.Length)} modules"
            End If

            Return summary.ToString
        End Function
    End Class
End Namespace
