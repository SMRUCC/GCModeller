
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.MetabolicModel

''' <summary>
''' Algorithm module for mke association between the gene and reactions
''' </summary>
Public Class MetabolicAssociator

    ' 算法参数
    Private Const MaxOperonDistance As Integer = 500 ' 操纵子内最大距离
    Private Const SameOperonBonus As Double = 0.3 ' 同操纵子奖励
    Private Const PathwayCompletenessThreshold As Double = 0.7 ' 通路完整度阈值
    Private Const MaxWindowSpan As Integer = 10 ' 上下文窗口大小（向上下游各看几个基因）
    Private Const MaxPhysicalDistance As Integer = 15000 ' 最大物理距离阈值，超过此距离认为不在同一基因簇
    Private Const BaseContextScore As Double = 0.5 ' 基于上下文推断的基础分
    Private Const DirectMatchScore As Double = 1.0 ' 直接EC匹配的满分
    Private Const SameStrandWeight As Double = 1.0 ' 同链权重
    Private Const DiffStrandWeight As Double = 0.3 ' 异链权重

    ''' <summary>
    ''' 增强的主关联函数
    ''' </summary>
    Public Function AssociateGenesToReactionsEnhanced(
        genome As GeneTable(),
        pathways As Pathway()) As List(Of GeneAssociation)

        ' 0. 预处理和多EC号处理
        Dim sortedGenome = PreprocessGenome(genome)
        Dim operonGroups = IdentifyPotentialOperons(sortedGenome)

        ' 1. 构建增强的索引
        Dim indices = New EnhancedIndices(pathways)

        ' 2. 多阶段打分
        Dim results = New List(Of GeneAssociation)()

        For i As Integer = 0 To sortedGenome.Length - 1
            Dim gene = sortedGenome(i)
            Dim geneScores = New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

            ' 阶段1: 直接EC匹配（支持多EC号）
            AddDirectECMatches(gene, indices.ECtoReactions, geneScores)

            ' 阶段2: 上下文关联
            AddContextAssociations(i, sortedGenome, operonGroups,
                                 indices, geneScores)

            ' 阶段3: 通路完整性推断
            AddPathwayCompletenessInferences(gene, geneScores, indices, pathways)

            ' 阶段4: 基因簇分析
            AddGeneClusterAnalysis(gene, i, sortedGenome, indices, geneScores)

            ' 整理并过滤结果
            results.Add(CreateFilteredAssociation(gene, geneScores))
        Next

        Return results
    End Function

    ''' <summary>
    ''' 预处理基因组，处理多EC号
    ''' </summary>
    Private Function PreprocessGenome(genome As GeneTable()) As GeneTable()
        Return genome.OrderBy(Function(g) g.left).ToArray()
    End Function

    ''' <summary>
    ''' 识别潜在操纵子
    ''' </summary>
    Private Function IdentifyPotentialOperons(genome As GeneTable()) As List(Of List(Of Integer))
        Dim operons = New List(Of List(Of Integer))()
        Dim currentOperon = New List(Of Integer)()

        For i As Integer = 0 To genome.Length - 1
            If currentOperon.Count = 0 Then
                currentOperon.Add(i)
            Else
                Dim prevGene = genome(currentOperon.Last())
                Dim currGene = genome(i)

                ' 判断是否可能在同一操纵子
                Dim distance = currGene.left - prevGene.right
                Dim sameStrand = currGene.strand = prevGene.strand
                Dim closeEnough = distance <= MaxOperonDistance

                If sameStrand AndAlso closeEnough Then
                    currentOperon.Add(i)
                Else
                    If currentOperon.Count > 1 Then
                        operons.Add(New List(Of Integer)(currentOperon))
                    End If
                    currentOperon = New List(Of Integer) From {i}
                End If
            End If
        Next

        Return operons
    End Function

    ''' <summary>
    ''' 直接EC匹配
    ''' </summary>
    Private Sub AddDirectECMatches(gene As GeneTable,
                                   ecToReactions As Dictionary(Of String, List(Of MetabolicReaction)),
                                   ByRef geneScores As Dictionary(Of String, Double))

        For Each ec In gene.EC_Number
            If ecToReactions.ContainsKey(ec) Then
                For Each reaction In ecToReactions(ec)
                    ' 直接匹配给满分
                    geneScores(reaction.id) = DirectMatchScore
                Next
            End If
        Next
    End Sub

    ''' <summary>
    ''' 上下文关联
    ''' </summary>
    Private Sub AddContextAssociations(geneIndex As Integer,
                                      genome As GeneTable(),
                                      operonGroups As List(Of List(Of Integer)),
                                      indices As EnhancedIndices,
                                      ByRef geneScores As Dictionary(Of String, Double))

        ' 查找基因所在的潜在操纵子
        Dim operon = operonGroups.FirstOrDefault(
            Function(o) o.Contains(geneIndex))

        ' 如果基因在操纵子中，考虑操纵子内所有基因
        If operon IsNot Nothing Then
            For Each neighborIdx In operon
                If neighborIdx = geneIndex Then Continue For

                Dim neighbor = genome(neighborIdx)
                AddNeighborAssociations(neighbor, indices, geneScores,
                                        SameOperonBonus, isOperon:=True)
            Next
        End If

        ' 传统窗口上下文（针对不在操纵子中的情况或补充）
        Dim windowStart = Math.Max(0, geneIndex - MaxWindowSpan)
        Dim windowEnd = Math.Min(genome.Length - 1, geneIndex + MaxWindowSpan)

        For j As Integer = windowStart To windowEnd
            If j = geneIndex Then Continue For

            Dim neighbor = genome(j)
            If operon IsNot Nothing AndAlso operon.Contains(j) Then
                Continue For ' 已经在上面的操纵子分析中处理
            End If

            ' 计算距离和方向权重
            Dim distance = Math.Abs(genome(geneIndex).left - neighbor.left)
            If distance > MaxPhysicalDistance Then Continue For

            Dim distanceScore = 1.0 - (distance / MaxPhysicalDistance)
            Dim strandWeight = If(genome(geneIndex).strand = neighbor.strand,
                                  SameStrandWeight, DiffStrandWeight)

            AddNeighborAssociations(neighbor, indices, geneScores,
                                   distanceScore * strandWeight, isOperon:=False)
        Next
    End Sub

    ''' <summary>
    ''' 邻居基因关联
    ''' </summary>
    Private Sub AddNeighborAssociations(neighbor As GeneTable,
                                        indices As EnhancedIndices,
                                        ByRef geneScores As Dictionary(Of String, Double),
                                        baseWeight As Double,
                                        isOperon As Boolean)

        For Each ec In neighbor.EC_Number
            If indices.ECtoReactions.ContainsKey(ec) Then
                For Each reaction In indices.ECtoReactions(ec)
                    ' 获取反应所在的所有通路
                    If indices.ReactionToPathways.ContainsKey(reaction.Id) Then
                        For Each pathway In indices.ReactionToPathways(reaction.Id)
                            ' 通路内所有反应都获得分数
                            For Each pathwayReaction In pathway.metabolicNetwork
                                Dim score = BaseContextScore * baseWeight

                                ' 操纵子内额外奖励
                                If isOperon Then
                                    score *= (1.0 + SameOperonBonus)
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
    Private Sub AddPathwayCompletenessInferences(gene As GeneTable,
                                                geneScores As Dictionary(Of String, Double),
                                                indices As EnhancedIndices,
                                                pathways As Pathway())

        ' 对每个通路，检查在基因组中的完整度
        For Each pathway In pathways
            Dim pathwayRxns = pathway.metabolicNetwork.Select(Function(r) r.id).ToList()

            ' 统计通路中已经有基因支持的反应
            Dim supportedRxns = pathwayRxns.Where(
                Function(rxnId) geneScores.ContainsKey(rxnId) AndAlso
                               geneScores(rxnId) >= 0.5).Count()

            Dim completeness = CDbl(supportedRxns) / pathwayRxns.Count

            ' 如果通路已有一定完整度，推测缺失的反应
            If completeness >= PathwayCompletenessThreshold Then
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
    Private Sub AddGeneClusterAnalysis(targetGene As GeneTable,
                                      geneIndex As Integer,
                                      genome As GeneTable(),
                                      indices As EnhancedIndices,
                                      ByRef geneScores As Dictionary(Of String, Double))

        ' 检查是否存在已知的代谢基因簇模式
        Dim clusterRadius = 5
        Dim clusterStart = Math.Max(0, geneIndex - clusterRadius)
        Dim clusterEnd = Math.Min(genome.Length - 1, geneIndex + clusterRadius)

        Dim clusterECNumbers = New List(Of String)()

        ' 收集簇内所有EC号
        For i = clusterStart To clusterEnd
            clusterECNumbers.AddRange(genome(i).EC_Number)
        Next

        ' 查找这些EC号共同参与的通路
        Dim commonPathways As Pathway() = FindCommonPathways(clusterECNumbers, indices)

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
    Private Function CreateFilteredAssociation(gene As GeneTable,
                                              geneScores As Dictionary(Of String, Double)) As GeneAssociation

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
        association.Reactions = association.Reactions.OrderByDescending(
            Function(r) r.Score).ToList()

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