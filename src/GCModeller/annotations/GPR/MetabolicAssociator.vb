
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.MetabolicModel

''' <summary>
''' Algorithm module for mke association between the gene and reactions
''' </summary>
Public Class MetabolicAssociator

    ReadOnly opt As GPRParameters
    ReadOnly genome As Genome
    ReadOnly context As ContextIndices

    ReadOnly coexpressionAnalyzer As CoexpressionAnalyzer
    ReadOnly syntenyAnalyzer As ConservedSyntenyAnalyzer
    ReadOnly complexDetector As EnzymeComplexDetector
    ReadOnly fusionAnalyzer As FusionGeneAnalyzer
    ReadOnly continuityChecker As ReactionContinuityChecker

    Private operonGroups As List(Of List(Of Integer))
    Private geneComplexes As List(Of List(Of GeneTable))

    Sub New(opt As GPRParameters, genome As IEnumerable(Of GeneTable), pathways As Pathway(),
            Optional coexpData As CoexpressionAnalyzer = Nothing,
            Optional syntenyData As ConservedSyntenyAnalyzer = Nothing)

        Me.opt = opt
        Me.genome = New Genome(genome)
        Me.context = New ContextIndices(pathways)

        ' 初始化分析器
        Me.coexpressionAnalyzer = coexpData
        Me.syntenyAnalyzer = syntenyData
        Me.fusionAnalyzer = New FusionGeneAnalyzer(Me.context)
        Me.continuityChecker = New ReactionContinuityChecker(Me.context.ECtoReactions.Values.SelectMany(Function(v) v).ToDictionary(Function(r) r.id))
        Me.complexDetector = New EnzymeComplexDetector()

        ' 预计算不依赖动态打分的结构
        Me.operonGroups = IdentifyPotentialOperons().ToList()
        Me.geneComplexes = complexDetector.DetectComplexes(Me.genome.AsEnumerable.ToArray())
    End Sub

    ''' <summary>
    ''' 增强的主关联函数
    ''' </summary>
    Public Iterator Function AssociateGenesToReactions() As IEnumerable(Of GeneAssociation)
        ' 使用全局字典暂存所有打分，方便后续网络级推断
        Dim globalGeneScores As New Dictionary(Of String, Dictionary(Of String, Double))(StringComparer.OrdinalIgnoreCase)

        ' ==========================================
        ' 阶段 1 & 2: 基因级打分 (直接证据 + 上下文)
        ' ==========================================
        For i As Integer = 0 To genome.N - 1
            Dim gene As GeneTable = genome(i)
            Dim geneScores As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

            ' 1. 直接EC匹配
            AddDirectECMatches(gene, geneScores)

            ' 2. 操纵子与滑动窗口上下文
            AddContextAssociations(i, geneScores)

            ' 3. 酶复合体关联
            AddComplexAssociations(gene, geneScores)

            ' 4. 融合基因关联
            fusionAnalyzer.AnalyzeFusionGenes({gene}, context.Pathways, globalGeneScores) ' 修改AnalyzeFusionGenes使其支持单基因传入或在此内联逻辑

            ' 5. 共表达关联 (如果提供了数据)
            If coexpressionAnalyzer IsNot Nothing Then
                coexpressionAnalyzer.ApplyCoexpressionRules(gene, geneScores, context)
            End If

            ' 6. 保守共线性关联 (如果提供了数据)
            If syntenyAnalyzer IsNot Nothing Then
                syntenyAnalyzer.ApplyConservationRules(gene, genome.AsEnumerable.ToArray(), i, geneScores)
            End If

            globalGeneScores(gene.locus_id) = geneScores
        Next

        ' ==========================================
        ' 阶段 3: 网络级打分 (全局推断)
        ' ==========================================
        ' 将全局打分结果注入Genome对象，供ReactionContinuityChecker查询
        UpdateGenomeNetwork(globalGeneScores)

        ' 3.1 通路完整度推断
        For Each gene As GeneTable In genome.AsEnumerable
            AddPathwayCompletenessInferences(gene, globalGeneScores(gene.locus_id))
        Next

        ' 3.2 反应连续性推断
        For Each pathway As Pathway In context.Pathways
            ' 这里的CheckContinuity需要重写，基于globalGeneScores增强分数
            EnhanceNetworkContinuity(pathway, globalGeneScores)
        Next

        ' ==========================================
        ' 整理输出
        ' ==========================================
        For Each gene As GeneTable In genome.AsEnumerable
            Yield CreateFilteredAssociation(gene, globalGeneScores(gene.locus_id))
        Next
    End Function

    Private Sub AddComplexAssociations(gene As GeneTable, ByRef geneScores As Dictionary(Of String, Double))
        ' 查找当前基因是否在已检测到的复合体中
        Dim targetComplex = geneComplexes.FirstOrDefault(Function(c) c.Any(Function(g) g.locus_id = gene.locus_id))
        If targetComplex Is Nothing Then Return

        ' 收集复合体所有EC号
        Dim allECs = targetComplex.SelectMany(Function(g) g.EC_Number).Distinct()
        ' 找到这些EC号共同参与的通路
        Dim commonPathways = context.FindCommonPathways(allECs)

        For Each pathway In commonPathways
            For Each reaction In pathway.metabolicNetwork
                Dim complexScore = opt.BaseComplexScore
                If Not geneScores.ContainsKey(reaction.id) OrElse geneScores(reaction.id) < complexScore Then
                    geneScores(reaction.id) = complexScore
                End If
            Next
        Next
    End Sub

    Private Sub EnhanceNetworkContinuity(pathway As Pathway, ByRef globalScores As Dictionary(Of String, Dictionary(Of String, Double)))
        ' 基于你提供的ReactionNetwork图结构，遍历边
        For Each edge In pathway.ReactionNetwork.graphEdges
            Dim uRxn = edge.U.ID
            Dim vRxn = edge.V.ID

            ' 检查化学相容性 (如果产物底物有重叠，edge理应存在)
            ' 找到催化这两个反应的基因
            Dim genesForU = genome.GetGenesForReaction(uRxn)
            Dim genesForV = genome.GetGenesForReaction(vRxn)

            ' 如果反应U有基因支持，且反应V也有部分支持，增强V的分数；反之亦然
            If genesForU.Any() AndAlso genesForV.Any() Then
                Dim continuityScore = 0.3 ' 基础连续性得分
                ' 互相增强
                For Each geneU In genesForU
                    If globalScores(geneU.locus_id).ContainsKey(vRxn) Then
                        globalScores(geneU.locus_id)(vRxn) = Math.Max(globalScores(geneU.locus_id)(vRxn), continuityScore)
                    End If
                Next
                For Each geneV In genesForV
                    If globalScores(geneV.locus_id).ContainsKey(uRxn) Then
                        globalScores(geneV.locus_id)(uRxn) = Math.Max(globalScores(geneV.locus_id)(uRxn), continuityScore)
                    End If
                Next
            End If
        Next
    End Sub

    Private Sub UpdateGenomeNetwork(globalScores As Dictionary(Of String, Dictionary(Of String, Double)))
        genome.MetabolicNetwork.Clear()
        For Each kvp In globalScores
            Dim assoc As New GeneAssociation With {
                .GeneId = kvp.Key,
                .Reactions = kvp.Value.Select(Function(r) New ScoredReaction With {.Id = r.Key, .Score = r.Value}).ToList()
            }
            genome.MetabolicNetwork(kvp.Key) = assoc
        Next
    End Sub

    ''' <summary>
    ''' 识别潜在操纵子
    ''' </summary>
    Private Iterator Function IdentifyPotentialOperons() As IEnumerable(Of List(Of Integer))
        Dim currentOperon As New List(Of Integer)()
        Dim geneCount As Integer = genome.N

        For i As Integer = 0 To geneCount - 1
            If currentOperon.Count = 0 Then
                currentOperon.Add(i)
            Else
                Dim prevGene As GeneTable = genome(currentOperon.Last())
                Dim currGene As GeneTable = genome(i)

                ' 判断是否可能在同一操纵子
                Dim distance = currGene.left - prevGene.right
                Dim sameStrand = currGene.strand = prevGene.strand
                Dim closeEnough = distance <= opt.MaxOperonDistance

                If sameStrand AndAlso closeEnough Then
                    currentOperon.Add(i)
                Else
                    If currentOperon.Count > 1 Then
                        Yield New List(Of Integer)(currentOperon)
                    End If
                    currentOperon = New List(Of Integer) From {i}
                End If
            End If
        Next
    End Function

    ''' <summary>
    ''' 直接EC匹配
    ''' </summary>
    Private Sub AddDirectECMatches(gene As GeneTable, ByRef geneScores As Dictionary(Of String, Double))
        For Each ec In gene.EC_Number
            If context.ECtoReactions.ContainsKey(ec) Then
                For Each reaction As MetabolicReaction In context.ECtoReactions(ec)
                    ' 直接匹配给满分
                    geneScores(reaction.id) = opt.DirectMatchScore
                Next
            End If
        Next
    End Sub

    ''' <summary>
    ''' 上下文关联
    ''' </summary>
    Private Sub AddContextAssociations(geneIndex As Integer, ByRef geneScores As Dictionary(Of String, Double))
        ' 查找基因所在的潜在操纵子
        Dim operon = operonGroups.FirstOrDefault(Function(o) o.Contains(geneIndex))
        Dim geneCount As Integer = genome.N

        ' 如果基因在操纵子中，考虑操纵子内所有基因
        If operon IsNot Nothing Then
            For Each neighborIdx In operon
                If neighborIdx = geneIndex Then Continue For

                Dim neighbor = genome(neighborIdx)
                AddNeighborAssociations(neighbor, geneScores, opt.SameOperonBonus, isOperon:=True)
            Next
        End If

        ' 传统窗口上下文（针对不在操纵子中的情况或补充）
        Dim windowStart = Math.Max(0, geneIndex - opt.MaxWindowSpan)
        Dim windowEnd = Math.Min(geneCount - 1, geneIndex + opt.MaxWindowSpan)

        For j As Integer = windowStart To windowEnd
            If j = geneIndex Then Continue For

            Dim neighbor = genome(j)
            If operon IsNot Nothing AndAlso operon.Contains(j) Then
                Continue For ' 已经在上面的操纵子分析中处理
            End If

            ' 计算距离和方向权重
            Dim distance = Math.Abs(genome(geneIndex).left - neighbor.left)

            If distance > opt.MaxPhysicalDistance Then
                Continue For
            End If

            Dim distanceScore = 1.0 - (distance / opt.MaxPhysicalDistance)
            Dim strandWeight = If(genome(geneIndex).strand = neighbor.strand, opt.SameStrandWeight, opt.DiffStrandWeight)

            AddNeighborAssociations(neighbor, geneScores, distanceScore * strandWeight, isOperon:=False)
        Next
    End Sub

    ''' <summary>
    ''' 邻居基因关联
    ''' </summary>
    Private Sub AddNeighborAssociations(neighbor As GeneTable, ByRef geneScores As Dictionary(Of String, Double), baseWeight As Double, isOperon As Boolean)
        For Each ec As String In neighbor.EC_Number
            If context.ECtoReactions.ContainsKey(ec) Then
                For Each reaction In context.ECtoReactions(ec)
                    ' 获取反应所在的所有通路
                    If context.ReactionToPathways.ContainsKey(reaction.id) Then
                        For Each pathway In context.ReactionToPathways(reaction.id)
                            ' 通路内所有反应都获得分数
                            For Each pathwayReaction In pathway.metabolicNetwork
                                Dim score = opt.BaseContextScore * baseWeight

                                ' 操纵子内额外奖励
                                If isOperon Then
                                    score *= (1.0 + opt.SameOperonBonus)
                                End If

                                ' 更新最高分
                                If Not geneScores.ContainsKey(pathwayReaction.id) OrElse
                                   geneScores(pathwayReaction.id) < score Then
                                    geneScores(pathwayReaction.id) = score
                                End If
                            Next
                        Next
                    End If
                Next
            End If
        Next
    End Sub

    ''' <summary>
    ''' 通路完整度推断
    ''' </summary>
    Private Sub AddPathwayCompletenessInferences(gene As GeneTable, geneScores As Dictionary(Of String, Double))
        ' 对每个通路，检查在基因组中的完整度
        For Each pathway As Pathway In context.Pathways
            Dim pathwayRxns = pathway.metabolicNetwork.Select(Function(r) r.id).ToList()

            ' 统计通路中已经有基因支持的反应
            Dim supportedRxns = pathwayRxns.Where(
                Function(rxnId) geneScores.ContainsKey(rxnId) AndAlso
                               geneScores(rxnId) >= 0.5).Count()

            Dim completeness = CDbl(supportedRxns) / pathwayRxns.Count

            ' 如果通路已有一定完整度，推测缺失的反应
            If completeness >= opt.PathwayCompletenessThreshold Then
                For Each rxn In pathway.metabolicNetwork
                    If Not geneScores.ContainsKey(rxn.id) OrElse
                       geneScores(rxn.id) < 0.3 Then

                        ' 给予通路完整度推断分数
                        Dim inferredScore = 0.3 + (completeness * 0.4)
                        If Not geneScores.ContainsKey(rxn.id) OrElse
                           geneScores(rxn.id) < inferredScore Then
                            geneScores(rxn.id) = inferredScore
                        End If
                    End If
                Next
            End If
        Next
    End Sub

    ''' <summary>
    ''' 基因簇分析
    ''' </summary>
    Private Sub AddGeneClusterAnalysis(targetGene As GeneTable, geneIndex As Integer, ByRef geneScores As Dictionary(Of String, Double))
        ' 检查是否存在已知的代谢基因簇模式
        Dim clusterRadius = 5
        Dim clusterStart = Math.Max(0, geneIndex - clusterRadius)
        Dim geneCount As Integer = genome.N
        Dim clusterEnd = Math.Min(geneCount - 1, geneIndex + clusterRadius)

        Dim clusterECNumbers = New List(Of String)()

        ' 收集簇内所有EC号
        For i = clusterStart To clusterEnd
            clusterECNumbers.AddRange(genome(i).EC_Number)
        Next

        ' 查找这些EC号共同参与的通路
        Dim commonPathways As Pathway() = context.FindCommonPathways(clusterECNumbers).ToArray

        ' 对这些通路的反应给予额外分数
        For Each pathway In commonPathways
            For Each reaction In pathway.metabolicNetwork
                Dim clusterScore = 0.4 ' 基因簇关联基础分

                If Not geneScores.ContainsKey(reaction.id) OrElse
                   geneScores(reaction.id) < clusterScore Then
                    geneScores(reaction.id) = clusterScore
                End If
            Next
        Next
    End Sub

    ''' <summary>
    ''' 创建过滤后的关联结果
    ''' </summary>
    Private Function CreateFilteredAssociation(gene As GeneTable, geneScores As Dictionary(Of String, Double)) As GeneAssociation
        Dim association = New GeneAssociation With {
            .GeneId = gene.locus_id,
            .Reactions = New List(Of ScoredReaction)()
        }

        ' 应用阈值过滤
        Const ConfidenceThreshold As Double = 0.3

        For Each kvp In geneScores
            If kvp.Value >= ConfidenceThreshold Then
                association.Reactions.Add(New ScoredReaction With {
                    .Id = kvp.Key,
                    .Score = Math.Round(kvp.Value, 4)
                })
            End If
        Next

        ' 按分数降序排列
        association.Reactions = association.Reactions.OrderByDescending(Function(r) r.Score).ToList()

        ' 如果没有任何关联，但基因有EC号，添加一个低置信度的注释
        If association.Reactions.Count = 0 AndAlso gene.EC_Number.Any() Then
            For Each ec In gene.EC_Number
                association.Reactions.Add(New ScoredReaction With {
                    .Id = $"EC:{ec} (unmapped)",
                    .Score = 0.2
                })
            Next
        End If

        Return association
    End Function
End Class