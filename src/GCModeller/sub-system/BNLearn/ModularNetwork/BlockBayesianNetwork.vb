Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.BNLearn.DBN
Imports SMRUCC.genomics.Analysis.BNLearn.Inference

Namespace ModularNetwork

    Public Class BlockBayesianNetwork

        Public Property moduleDBs As ModuleDBN()
        ''' <summary>
        ''' ③ 模块间关联图（基于 eigengene 轨迹相关度）
        ''' </summary>
        ''' <returns></returns>
        Public Property graph As Dictionary(Of String, List(Of (modColor As String, weight As Double)))

        Public Property TF As String()

        Public ReadOnly Property blocks As Integer
            Get
                Return moduleDBs.TryCount
            End Get
        End Property

        Public ReadOnly Property allgenes As String()
            Get
                Return moduleDBs.SelectMany(Function(m) m.Genes).Distinct().ToArray()
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="subblocks"></param>
        ''' <param name="TFs"></param>
        ''' <param name="crossModuleCorThreshold">
        ''' 模块 eigengene 相关阈值：|cor| 超过才建立模块间关联，默认 0.3。
        ''' </param>
        Sub New(subblocks As IEnumerable(Of ModuleDBN), TFs As IEnumerable(Of String), Optional crossModuleCorThreshold As Double = 0.3)
            moduleDBs = subblocks.SafeQuery.ToArray
            graph = BuildModuleCorrelationGraph(moduleDBs, crossModuleCorThreshold)
            TF = TFs.SafeQuery.ToArray

            Call $"GRN.TrainModularDBNIntervene: 模块关联边数={graph.Values.Sum(Function(l) l.Count)}".info
        End Sub

        ''' <summary>
        ''' 对单个扰动基因执行全局级联虚拟扰动：
        '''   - 在其所属模块内固定 Low 并多步推演本模块基因状态轨迹；
        '''   - 计算本模块 eigengene 变化，沿模块关联图 BFS 逐级注入下游模块（作为模块整体状态偏置），
        '''     在下游模块内做受迫推演，形成级联；
        '''   - 汇总所有模块基因的最终状态为全局响应向量（按 allGenes 顺序，Low=0/Med=1/High=2）。
        ''' </summary>
        Public Function CascadeIntervene(knockGene As String, steps As Integer, trajectories As Dictionary(Of String, Dictionary(Of String, List(Of Double)))) As Double()
            ' 定位扰动基因所属模块
            Dim m0 As ModuleDBN = Nothing
            Dim allGenes As String() = Me.allgenes
            Dim tfSet As New HashSet(Of String)(TF)

            For Each m In moduleDBs
                If m.GeneIndex.ContainsKey(knockGene) Then
                    m0 = m
                    Exit For
                End If
            Next
            If m0 Is Nothing Then
                Call $"GRN.CascadeIntervene: 警告: 扰动基因 '{knockGene}' 不在任何模块中，跳过".info
                Dim zero As Double() = allGenes.Select(Function(g) 1.0).ToArray()
                trajectories(knockGene) = New Dictionary(Of String, List(Of Double))
                Return zero
            End If

            ' 每个模块维护基因离散状态（初始 Medium），以及各自的轨迹容器
            Dim moduleStates As New Dictionary(Of String, Dictionary(Of String, String))
            Dim moduleTraj As New Dictionary(Of String, Dictionary(Of String, List(Of Double)))
            For Each m In moduleDBs
                Dim st As New Dictionary(Of String, String)
                Dim tr As New Dictionary(Of String, List(Of Double))
                For Each g In m.Genes
                    st(g) = "Medium"
                    tr(g) = New List(Of Double)(New Double(steps - 1) {})
                Next
                moduleStates(m.ModuleColor) = st
                moduleTraj(m.ModuleColor) = tr
            Next

            ' 初始步：扰动基因固定 Low
            moduleStates(m0.ModuleColor)(knockGene) = "Low"
            For Each g In m0.Genes
                moduleTraj(m0.ModuleColor)(g)(0) = StateToValue(moduleStates(m0.ModuleColor)(g))
            Next

            ' 本模块多步推演
            Dim m0Rates = RunModuleSteps(m0, moduleStates(m0.ModuleColor), knockGene, steps, tfSet, moduleTraj(m0.ModuleColor))
            ' 计算本模块 eigengene 变化（最终步 RNA 速率均值）
            Dim delta0 = If(m0Rates.Count > 0, m0Rates.Values.Average(), 0.0)

            ' 沿模块关联图 BFS 级联
            Dim visited As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {m0.ModuleColor}
            Dim queue As New Queue(Of (modColor As String, delta As Double))
            queue.Enqueue((modColor:=m0.ModuleColor, delta:=delta0))

            While queue.Count > 0
                Dim cur = queue.Dequeue()
                If Not graph.ContainsKey(cur.modColor) Then Continue While
                For Each adj In graph(cur.modColor)
                    If visited.Contains(adj.modColor) Then Continue For
                    visited.Add(adj.modColor)
                    Dim mNext = moduleDBs.First(Function(m) String.Equals(m.ModuleColor, adj.modColor, StringComparison.OrdinalIgnoreCase))
                    ' 上游变化按关联权重注入下游模块（作为模块整体状态偏置）
                    Dim upstreamDelta = cur.delta * adj.weight
                    Dim fixedInNext = If(mNext.GeneIndex.ContainsKey(knockGene), knockGene, Nothing)
                    Dim nextRates = RunModuleForced(mNext, upstreamDelta, fixedInNext, steps, tfSet, moduleStates(mNext.ModuleColor), moduleTraj(mNext.ModuleColor))
                    Dim deltaNext = If(nextRates.Count > 0, nextRates.Values.Average(), 0.0)
                    queue.Enqueue((modColor:=mNext.ModuleColor, delta:=deltaNext))
                Next
            End While

            ' 汇总全局最终响应向量（显式双层循环，避免 SelectMany 对 Double() 轨迹的深层展平）
            Dim geneToTraj As New Dictionary(Of String, List(Of Double))(StringComparer.OrdinalIgnoreCase)
            For Each kvModule In moduleTraj
                For Each kvGene In kvModule.Value
                    geneToTraj(kvGene.Key) = kvGene.Value
                Next
            Next

            Dim resp(allGenes.Length - 1) As Double
            For i = 0 To allGenes.Length - 1
                Dim g = allGenes(i)
                If geneToTraj.ContainsKey(g) Then
                    resp(i) = geneToTraj(g)(steps - 1)
                Else
                    resp(i) = 1.0  ' 未参与任何模块：中性 Medium
                End If
            Next

            Dim trajMerged As New Dictionary(Of String, List(Of Double))(StringComparer.OrdinalIgnoreCase)
            For Each kvModule In moduleTraj
                For Each kvGene In kvModule.Value
                    trajMerged(kvGene.Key) = kvGene.Value
                Next
            Next
            trajectories(knockGene) = trajMerged

            Call $"GRN.CascadeIntervene: 对基因 '{knockGene}'（模块 {m0.ModuleColor}）完成级联虚拟扰动，本模块 eigengene 变化 δ={delta0:F4}".info
            Return resp
        End Function

        ''' <summary>
        ''' 在单个模块子网络内多步推演（扰动基因固定 Low）。返回各基因最终 RNA 丰度变化率。
        ''' </summary>
        Private Function RunModuleSteps(m As ModuleDBN,
                                        geneStates As Dictionary(Of String, String),
                                        fixedGene As String,
                                        steps As Integer,
                                        tfSet As HashSet(Of String),
                                        traj As Dictionary(Of String, List(Of Double))) As Dictionary(Of String, Double)
            Dim lastRates As New Dictionary(Of String, Double)

            For t As Integer = 1 To steps - 1
                ' 模块内 TF 基因的连续 abundance（由当前离散状态映射，与证据一致）
                Dim tfAbund As New Dictionary(Of String, Double)

                For Each gene_id As String In m.Genes
                    If tfSet.Contains(gene_id) Then
                        tfAbund(gene_id) = StateToScore(geneStates(gene_id))
                    End If
                Next

                Dim result As DBNPredictionResult = m.Net.PredictNextState(Nothing, tfAbund, geneStates)

                For Each gene_id As String In m.Genes
                    If result.GeneStates.ContainsKey(gene_id) Then
                        geneStates(gene_id) = result.GeneStates(gene_id)
                    End If
                    ' 持续固定扰动基因 Low，避免被反馈回路恢复
                    If Not String.IsNullOrEmpty(fixedGene) Then
                        geneStates(fixedGene) = "Low"
                    End If

                    traj(gene_id)(t) = StateToValue(geneStates(gene_id))
                Next

                For Each gene_id As String In m.Genes
                    If result.RNAAbundanceChanges.ContainsKey(gene_id) Then
                        lastRates(gene_id) = result.RNAAbundanceChanges(gene_id)
                    End If
                Next
            Next

            Return lastRates
        End Function

        ''' <summary>
        ''' 受迫推演：下游模块接收上游 eigengene 变化偏置，初始整体状态偏移后多步推演。
        ''' </summary>
        Private Function RunModuleForced(m As ModuleDBN,
                                         upstreamDelta As Double,
                                         fixedGene As String,
                                         steps As Integer,
                                         tfSet As HashSet(Of String),
                                         geneStates As Dictionary(Of String, String),
                                         traj As Dictionary(Of String, List(Of Double))) As Dictionary(Of String, Double)
            ' 初始整体状态偏置：上游正向变化 → High，负向 → Low，近 0 → Medium
            Dim initState As String = If(upstreamDelta > 0.1, "High", If(upstreamDelta < -0.1, "Low", "Medium"))
            For Each g In m.Genes
                geneStates(g) = initState
            Next
            If Not String.IsNullOrEmpty(fixedGene) Then geneStates(fixedGene) = "Low"
            For Each g In m.Genes
                traj(g)(0) = StateToValue(geneStates(g))
            Next

            Dim lastRates As New Dictionary(Of String, Double)
            For t = 1 To steps - 1
                Dim tfAbund As New Dictionary(Of String, Double)
                For Each g In m.Genes
                    If tfSet.Contains(g) Then
                        ' 上游变化注入 TF abundance（clamp 到合理范围）
                        tfAbund(g) = Math.Max(0.0, Math.Min(2.0, StateToScore(geneStates(g)) * (1.0 + upstreamDelta)))
                    End If
                Next

                Dim result = m.Net.PredictNextState(Nothing, tfAbund, geneStates)
                For Each g In m.Genes
                    If result.GeneStates.ContainsKey(g) Then
                        geneStates(g) = result.GeneStates(g)
                    End If
                    If Not String.IsNullOrEmpty(fixedGene) Then geneStates(fixedGene) = "Low"
                    traj(g)(t) = StateToValue(geneStates(g))
                Next
                For Each g In m.Genes
                    If result.RNAAbundanceChanges.ContainsKey(g) Then lastRates(g) = result.RNAAbundanceChanges(g)
                Next
            Next

            Return lastRates
        End Function
    End Class
End Namespace