Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.MetabolicModel

''' <summary>
''' 基因 - 代谢反应关联（GPR link）推断算法。
''' 
''' 算法被组织为一条显式的阶段化流水线，每一个阶段只负责一类证据：
''' 
''' <list type="number">
''' <item>阶段 1：**基因级直接证据** —— 基因自身 EC 编号匹配到的反应、多结构域融合酶；</item>
''' <item>阶段 2：**结构证据** —— 潜在操纵子、物理位置滑动窗口、潜在酶复合体；</item>
''' <item>阶段 3：**跨组学证据** —— 共表达、跨物种保守共线性；</item>
''' <item>阶段 4：**网络一致性证据** —— 通路完整度补缺、反应之间化学连续性增益；</item>
''' <item>阶段 5：**聚合与过滤** —— 按 noisy-OR 聚合全部证据并应用置信度阈值；</item>
''' <item>阶段 6：**物化** —— 把结果同时写入 <see cref="Genome.MetabolicNetwork"/> 与反向索引。</item>
''' </list>
''' 
''' 关键设计约束（这是与旧实现最本质的差别）：
''' 
''' + **作用域收敛**：上下文类的证据只作用于"证据直接指向的那个反应"，
'''   绝不会扩散到该反应所在通路的全部反应；
''' + **单一数据源**：算法内部累积的结果与对外输出的结果来自同一份数据；
''' + **可解释**：任何分数都能追溯回具体的证据来源。
''' </summary>
Public Class MetabolicAssociator

    ReadOnly opt As GPRParameters
    ReadOnly genome As Genome
    ReadOnly context As ContextIndices

    ReadOnly complexDetector As EnzymeComplexDetector
    ReadOnly fusionAnalyzer As FusionGeneAnalyzer
    ReadOnly continuityChecker As ReactionContinuityChecker
    ReadOnly coexpressionAnalyzer As CoexpressionAnalyzer
    ReadOnly syntenyAnalyzer As ConservedSyntenyAnalyzer

    ''' <summary>
    ''' 潜在操纵子分组，元素为基因在 <see cref="Genome"/> 中的物理索引
    ''' </summary>
    ReadOnly operonGroups As List(Of List(Of Integer))

    ''' <summary>
    ''' 潜在酶复合体分组
    ''' </summary>
    ReadOnly geneComplexes As List(Of List(Of GeneTable))

    ''' <summary>
    ''' 基因组上下文与关联结果（算法内部使用的唯一实例）
    ''' </summary>
    Public ReadOnly Property GenomeModel As Genome
        Get
            Return genome
        End Get
    End Property

    ''' <summary>
    ''' 参考代谢网络的倒排索引
    ''' </summary>
    Public ReadOnly Property ContextIndex As ContextIndices
        Get
            Return context
        End Get
    End Property

    Public ReadOnly Property Parameters As GPRParameters
        Get
            Return opt
        End Get
    End Property

    ''' <summary>
    ''' 识别出来的潜在操纵子分组
    ''' </summary>
    Public ReadOnly Property Operons As List(Of List(Of Integer))
        Get
            Return operonGroups
        End Get
    End Property

    ''' <summary>
    ''' 检测出来的潜在酶复合体
    ''' </summary>
    Public ReadOnly Property Complexes As List(Of List(Of GeneTable))
        Get
            Return geneComplexes
        End Get
    End Property

    Sub New(opt As GPRParameters,
            genome As IEnumerable(Of GeneTable),
            pathways As Pathway(),
            Optional coexpData As CoexpressionAnalyzer = Nothing,
            Optional syntenyData As ConservedSyntenyAnalyzer = Nothing)

        Me.opt = If(opt, New GPRParameters)
        Me.genome = New Genome(genome)
        Me.context = New ContextIndices(pathways)

        ' 初始化各类证据分析器
        Me.coexpressionAnalyzer = coexpData
        Me.syntenyAnalyzer = syntenyData
        Me.fusionAnalyzer = New FusionGeneAnalyzer(Me.context)
        Me.continuityChecker = ReactionContinuityChecker.LoadFromContext(Me.context)
        Me.complexDetector = New EnzymeComplexDetector()

        ' 预计算不依赖动态打分的结构
        Dim genes As GeneTable() = Me.genome.AsEnumerable.ToArray

        Me.operonGroups = IdentifyPotentialOperons(genes)
        Me.geneComplexes = complexDetector.DetectComplexes(genes, Me.opt)
    End Sub

    ''' <summary>
    ''' 执行完整的关联推断流程。
    ''' 
    ''' 返回的集合与 <see cref="Genome.MetabolicNetwork"/> 完全一致。
    ''' </summary>
    Public Function AssociateGenesToReactions() As IEnumerable(Of GeneAssociation)
        Dim genes As GeneTable() = genome.AsEnumerable.ToArray()
        Dim evidences As New Dictionary(Of String, Dictionary(Of String, List(Of AssociationEvidence)))(StringComparer.OrdinalIgnoreCase)

        ' ======================================================
        ' 阶段 1：基因级直接证据
        ' ======================================================
        Call "phase 1/6: gene level direct evidence...".info

        For Each gene As GeneTable In genes
            Dim bucket As New Dictionary(Of String, List(Of AssociationEvidence))(StringComparer.OrdinalIgnoreCase)
            evidences(gene.locus_id) = bucket

            For Each hit As ReactionEvidence In CollectDirectEvidence(gene)
                AddEvidence(bucket, hit)
            Next
        Next

        ' 物化"直接证据"种子集合。
        ' 这一步是必需的：共表达等跨基因证据必须建立在其它基因已知的直接关联之上，
        ' 旧实现把共表达放在网络为空的时候执行，因此该证据永远不可能生效。
        Call genome.SetSeedReactions(CollectSeedReactions(genes, evidences))

        ' ======================================================
        ' 阶段 2：结构与上下文证据
        ' ======================================================
        Call "phase 2/6: operon / context window / enzyme complex evidence...".info

        For i As Integer = 0 To genes.Length - 1
            For Each hit As ReactionEvidence In CollectStructuralEvidence(i, genes)
                AddEvidence(evidences(genes(i).locus_id), hit)
            Next
        Next

        ' ======================================================
        ' 阶段 3：跨组学证据
        ' ======================================================
        Call "phase 3/6: coexpression / conserved synteny evidence...".info

        For i As Integer = 0 To genes.Length - 1
            Dim bucket As Dictionary(Of String, List(Of AssociationEvidence)) = evidences(genes(i).locus_id)

            If coexpressionAnalyzer IsNot Nothing Then
                For Each hit As ReactionEvidence In coexpressionAnalyzer.CollectEvidence(genes(i), genome, opt)
                    AddEvidence(bucket, hit)
                Next
            End If

            If syntenyAnalyzer IsNot Nothing Then
                For Each hit As ReactionEvidence In syntenyAnalyzer.CollectEvidence(genes(i), genes, i, context, opt)
                    AddEvidence(bucket, hit)
                Next
            End If
        Next

        ' ======================================================
        ' 阶段 4：网络一致性证据
        ' ======================================================
        Call "phase 4/6: pathway completeness / reaction continuity evidence...".info

        For Each gene As GeneTable In genes
            Dim bucket As Dictionary(Of String, List(Of AssociationEvidence)) = evidences(gene.locus_id)

            ' 参与网络一致性推断的"已支持"反应，只使用阶段 1~3 的证据，避免自我强化
            Dim supported As String() = bucket _
                .Where(Function(kv) kv.Value.Combine(opt.ScoreCap, opt.CorroborationGain) >= opt.ConfidenceThreshold) _
                .Select(Function(kv) kv.Key) _
                .ToArray

            If supported.Length = 0 Then Continue For

            Dim directSupported As String() = bucket _
                .Where(Function(kv) kv.Value.Any(Function(e) e.Kind = EvidenceKind.DirectEC)) _
                .Select(Function(kv) kv.Key) _
                .ToArray

            For Each hit As ReactionEvidence In CollectCompletenessEvidence(directSupported)
                AddEvidence(bucket, hit)
            Next

            For Each hit As ReactionEvidence In continuityChecker.CollectEvidence(supported, context, opt)
                AddEvidence(bucket, hit)
            Next
        Next

        ' ======================================================
        ' 阶段 5：聚合与过滤
        ' ======================================================
        Call "phase 5/6: aggregate evidences and filter by confidence...".info

        Dim associations As GeneAssociation() = genes _
            .Select(Function(gene) BuildAssociation(gene, evidences(gene.locus_id))) _
            .ToArray

        ' ======================================================
        ' 阶段 6：统一物化（正向关联表 + 反应反向索引）
        ' ======================================================
        Call "phase 6/6: materialize genome metabolic network...".info

        Call genome.RebuildNetwork(associations)

        Return associations
    End Function

#Region "阶段 1：直接证据"

    ''' <summary>
    ''' 基因级直接证据：自身 EC 匹配 + 多结构域融合酶
    ''' </summary>
    Private Iterator Function CollectDirectEvidence(gene As GeneTable) As IEnumerable(Of ReactionEvidence)
        For Each ec As String In DistinctEC(gene)
            For Each reaction As MetabolicReaction In context.GetReactionsByEC(ec)
                Yield New ReactionEvidence(reaction.id, New AssociationEvidence With {
                    .Kind = EvidenceKind.DirectEC,
                    .Weight = opt.DirectMatchScore,
                    .RawScore = 1.0,
                    .Source = $"gene EC {ec}"
                })
            Next
        Next

        For Each hit As ReactionEvidence In fusionAnalyzer.CollectEvidence(gene, opt)
            Yield hit
        Next
    End Function

    Private Function CollectSeedReactions(genes As GeneTable(),
                                          evidences As Dictionary(Of String, Dictionary(Of String, List(Of AssociationEvidence)))) As Dictionary(Of String, String())

        Dim seeds As New Dictionary(Of String, String())(StringComparer.OrdinalIgnoreCase)

        For Each gene As GeneTable In genes
            seeds(gene.locus_id) = evidences(gene.locus_id) _
                .Where(Function(kv) kv.Value.Any(Function(e) e.Kind = EvidenceKind.DirectEC)) _
                .Select(Function(kv) kv.Key) _
                .ToArray
        Next

        Return seeds
    End Function

#End Region

#Region "阶段 2：结构与上下文证据"

    ''' <summary>
    ''' 潜在操纵子识别。
    ''' 
    ''' 修复：原实现在循环结束之后没有 flush 最后一个操纵子，导致基因组尾部的操纵子永远丢失；
    ''' 同时把间距改为受下界 0 保护，避免基因重叠时产生负数间距进而吞并整条染色体。
    ''' </summary>
    Private Function IdentifyPotentialOperons(genes As GeneTable()) As List(Of List(Of Integer))
        Dim groups As New List(Of List(Of Integer))
        Dim current As New List(Of Integer)

        If genes Is Nothing OrElse genes.Length = 0 Then Return groups

        For i As Integer = 0 To genes.Length - 1
            If current.Count = 0 Then
                current.Add(i)
                Continue For
            End If

            Dim previous As GeneTable = genes(current(current.Count - 1))
            Dim target As GeneTable = genes(i)

            Dim distance As Integer = Math.Max(0, target.left - previous.right)
            Dim sameStrand As Boolean = String.Equals(target.strand, previous.strand, StringComparison.Ordinal)
            Dim closeEnough As Boolean = distance <= opt.MaxOperonDistance

            If sameStrand AndAlso closeEnough Then
                current.Add(i)
            Else
                If current.Count >= 2 Then groups.Add(current)
                current = New List(Of Integer) From {i}
            End If
        Next

        ' 循环结束之后必须补一次 flush
        If current.Count >= 2 Then groups.Add(current)

        Return groups
    End Function

    ''' <summary>
    ''' 结构证据：操纵子内部邻接 + 物理位置滑动窗口 + 酶复合体
    ''' </summary>
    Private Function CollectStructuralEvidence(geneIndex As Integer, genes As GeneTable()) As IEnumerable(Of ReactionEvidence)
        Dim results As New List(Of ReactionEvidence)

        Dim gene As GeneTable = genes(geneIndex)
        Dim operon As List(Of Integer) = operonGroups.FirstOrDefault(Function(o) o.Contains(geneIndex))

        ' 操纵子内部：权重为 BaseContextScore * (1 + SameOperonBonus)
        If operon IsNot Nothing Then
            Dim operonWeight As Double = AssociationEvidence.Normalize(opt.BaseContextScore * (1.0 + opt.SameOperonBonus))

            For Each neighbourIndex As Integer In operon
                If neighbourIndex = geneIndex Then Continue For
                results.AddRange(NeighbourReactions(genes(neighbourIndex), EvidenceKind.OperonContext, operonWeight, 1.0))
            Next
        End If

        ' 滑动窗口：按物理距离线性衰减，并考虑链方向
        Dim startIdx As Integer = Math.Max(0, geneIndex - opt.MaxWindowSpan)
        Dim endIdx As Integer = Math.Min(genes.Length - 1, geneIndex + opt.MaxWindowSpan)

        For j As Integer = startIdx To endIdx
            If j = geneIndex Then Continue For
            If operon IsNot Nothing AndAlso operon.Contains(j) Then Continue For

            Dim neighbour As GeneTable = genes(j)
            Dim distance As Integer = Math.Abs(gene.left - neighbour.left)
            If distance > opt.MaxPhysicalDistance Then Continue For

            Dim distanceScore As Double = 1.0 - CDbl(distance) / opt.MaxPhysicalDistance
            Dim strandWeight As Double = If(
                String.Equals(gene.strand, neighbour.strand, StringComparison.Ordinal),
                opt.SameStrandWeight,
                opt.DiffStrandWeight)

            results.AddRange(NeighbourReactions(neighbour, EvidenceKind.WindowContext, opt.BaseContextScore, distanceScore * strandWeight))
        Next

        ' 酶复合体
        Dim complex As List(Of GeneTable) = geneComplexes.FirstOrDefault(
            Function(c) c.Any(Function(g) String.Equals(g.locus_id, gene.locus_id, StringComparison.OrdinalIgnoreCase)))

        If complex IsNot Nothing Then
            For Each member As GeneTable In complex
                If String.Equals(member.locus_id, gene.locus_id, StringComparison.OrdinalIgnoreCase) Then Continue For
                results.AddRange(NeighbourReactions(member, EvidenceKind.EnzymeComplex, opt.BaseComplexScore, 1.0))
            Next
        End If

        Return results
    End Function

    ''' <summary>
    ''' 邻居基因所"直接携带"的反应。
    ''' 
    ''' 这是本算法最重要的一个约束：证据只落在邻居基因自身能够催化的反应上，
    ''' 绝不会扩散到该反应所在通路的全部反应。旧实现正是因为在这一点上不受约束，
    ''' 才导致几乎所有基因都被关联到几乎所有的反应、结果完全失去区分度。
    ''' </summary>
    Private Function NeighbourReactions(neighbour As GeneTable, kind As EvidenceKind, weight As Double, raw As Double) As IEnumerable(Of ReactionEvidence)
        Dim results As New List(Of ReactionEvidence)

        If neighbour Is Nothing Then Return results
        If weight <= 0 OrElse raw <= 0 Then Return results

        For Each ec As String In DistinctEC(neighbour)
            For Each reaction As MetabolicReaction In context.GetReactionsByEC(ec)
                results.Add(New ReactionEvidence(reaction.id, New AssociationEvidence With {
                    .Kind = kind,
                    .Weight = weight,
                    .RawScore = raw,
                    .Source = $"{neighbour.locus_id} ({ec})"
                }))
            Next
        Next

        Return results
    End Function

#End Region

#Region "阶段 4：网络一致性证据"

    ''' <summary>
    ''' 通路完整度补缺。
    ''' 
    ''' 当基因自身的直接证据已经覆盖了某条通路的大部分反应时，说明它确实参与了这条通路，
    ''' 此时对"与该基因已支持反应在通路中直接相邻"的缺口补充一条受限的证据。
    ''' 
    ''' 与旧实现相比的两点关键差别：
    ''' 
    ''' + 只补齐**间隔不超过 <see cref="GPRParameters.MaxGapInPathway"/> 步**的缺口，
    '''   而不是把整条通路的每一个反应都灌上分数；
    ''' + 证据强度由通路完整度决定，且权重显著低于直接证据。
    ''' </summary>
    Private Function CollectCompletenessEvidence(directSupported As String()) As IEnumerable(Of ReactionEvidence)
        Dim results As New List(Of ReactionEvidence)

        If directSupported Is Nothing OrElse directSupported.Length = 0 Then Return results

        Dim supported As New HashSet(Of String)(directSupported, StringComparer.OrdinalIgnoreCase)
        Dim maxGap As Integer = Math.Max(1, opt.MaxGapInPathway)

        For Each pathway As Pathway In context.Pathways
            Dim key As String = If(String.IsNullOrEmpty(pathway.ID), pathway.name, pathway.ID)

            Dim members As List(Of MetabolicReaction) = Nothing
            If Not context.PathwayReactions.TryGetValue(key, members) Then Continue For
            If members.Count = 0 Then Continue For

            Dim matched As MetabolicReaction() = members _
                .Where(Function(r) supported.Contains(r.id)) _
                .ToArray

            If matched.Length = 0 Then Continue For

            Dim completeness As Double = CDbl(matched.Length) / members.Count
            If completeness < opt.PathwayCompletenessThreshold Then Continue For

            For Each gap As MetabolicReaction In members
                If supported.Contains(gap.id) Then Continue For

                Dim connected As Boolean = matched.Any(
                    Function(m)
                        Return context.GetReactionGap(key, m.id, gap.id, maxGap) > 0 OrElse
                               context.GetReactionGap(key, gap.id, m.id, maxGap) > 0
                    End Function)

                If Not connected Then Continue For

                results.Add(New ReactionEvidence(gap.id, New AssociationEvidence With {
                    .Kind = EvidenceKind.PathwayCompleteness,
                    .Weight = opt.PathwayCompletenessWeight,
                    .RawScore = completeness,
                    .Source = key
                }))
            Next
        Next

        Return results
    End Function

#End Region

#Region "聚合与输出"

    Private Shared Sub AddEvidence(bucket As Dictionary(Of String, List(Of AssociationEvidence)), hit As ReactionEvidence)
        If bucket Is Nothing Then Return
        If String.IsNullOrEmpty(hit.ReactionID) OrElse hit.Evidence Is Nothing Then Return
        If hit.Evidence.Contribution <= 0 Then Return

        Dim list As List(Of AssociationEvidence) = Nothing

        If Not bucket.TryGetValue(hit.ReactionID, list) Then
            list = New List(Of AssociationEvidence)
            bucket(hit.ReactionID) = list
        End If

        ' 同一来源、同一类型的证据只记一次，避免重复计数破坏分数的可复现性
        Dim duplicated As Boolean = list.Any(
            Function(e)
                Return e.Kind = hit.Evidence.Kind AndAlso
                       String.Equals(e.Source, hit.Evidence.Source, StringComparison.OrdinalIgnoreCase)
            End Function)

        If duplicated Then Return

        list.Add(hit.Evidence)
    End Sub

    Private Function BuildAssociation(gene As GeneTable,
                                      bucket As Dictionary(Of String, List(Of AssociationEvidence))) As GeneAssociation

        Dim association As New GeneAssociation With {.GeneId = gene.locus_id}

        Dim accepted As New List(Of ScoredReaction)

        For Each item As KeyValuePair(Of String, List(Of AssociationEvidence)) In bucket
            Dim score As Double = item.Value.Combine(opt.ScoreCap, opt.CorroborationGain)

            If score < opt.ConfidenceThreshold Then Continue For

            accepted.Add(New ScoredReaction With {
                .Id = item.Key,
                .Score = score,
                .Unmapped = False,
                .Evidences = item.Value _
                    .OrderByDescending(Function(e) e.Contribution) _
                    .ToList()
            })
        Next

        ' 按分数降序写入，保证结果表的可读性
        For Each reaction As ScoredReaction In accepted.OrderByDescending(Function(r) r.Score)
            association.Reactions(reaction.Id) = reaction
        Next

        ' 未映射的 EC 编号单独输出，不再伪装成反应编号混入结果表
        Dim unmapped As New List(Of String)

        For Each ec As String In DistinctEC(gene)
            If context.GetReactionsByEC(ec).Count = 0 Then unmapped.Add(ec)
        Next

        association.UnmappedECNumbers = unmapped.ToArray

        Return association
    End Function

    Private Shared Function DistinctEC(gene As GeneTable) As String()
        If gene Is Nothing OrElse gene.EC_Number Is Nothing Then Return New String() {}

        Return gene.EC_Number _
            .Where(Function(ec) Not String.IsNullOrEmpty(ec)) _
            .Select(Function(ec) ec.Trim) _
            .Distinct(StringComparer.OrdinalIgnoreCase) _
            .ToArray
    End Function

#End Region

End Class
