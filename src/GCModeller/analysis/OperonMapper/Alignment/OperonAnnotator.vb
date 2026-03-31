Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Tasks.Models

Public Module OperonAnnotator

    Public Iterator Function ParseBlastn(blastn As String) As IEnumerable(Of HitCollection)
        For Each hits As HitCollection In BlastnOutputReader.RunParser(blastn).ExportHitsResult
            If Not hits.hits.IsNullOrEmpty Then
                For Each hit As Hit In hits.hits
                    With hit.hitName.GetTagValue("|")
                        hit.tag = .Value.Split("|"c).First
                        hit.hitName = .Name
                    End With
                Next
            End If

            Yield hits
        Next
    End Function

    ' --- 2. 核心逻辑模块 ---

    ''' <summary>
    ''' 基于BLASTN比对结果和已知Operon信息，对基因组进行Operon注释。
    ''' </summary>
    ''' <param name="allGenes">基因组中所有基因的数组。</param>
    ''' <param name="blastResults">所有基因的BLASTN比对结果数组。</param>
    ''' <param name="knownOperonsDict">包含所有已知Operon信息的字典。</param>
    ''' <returns>一个包含所有注释到的Operon的列表。</returns>
    Public Iterator Function AnnotateOperons(
    allGenes As GeneTable(),
    blastResults As HitCollection(),
    knownOperonsDict As Dictionary(Of String, WebJSON.Operon),
    Optional geneDistCutoff As Integer = 100
) As IEnumerable(Of AnnotatedOperon)

        ' --- 步骤 1: 为每个基因投票，确定其最可能的Operon ID及得分 ---
        Dim geneToOperonMap As New Dictionary(Of String, (operonId As String, score As Double))()
        Dim blastDict = blastResults.ToDictionary(Function(hc) hc.QueryName)

        For Each gene As GeneTable In allGenes
            If blastDict.ContainsKey(gene.locus_id) Then
                Dim hc As HitCollection = blastDict(gene.locus_id)
                If hc.hits IsNot Nothing AndAlso hc.hits.Length > 0 Then
                    ' 分组计算每个Operon ID的总得分
                    Dim operonScores = hc.hits.GroupBy(Function(h) h.tag) _
                                      .ToDictionary(
                                          Function(g) g.Key,
                                          Function(g) g.Sum(Function(h) h.score * h.identities * (1 - h.gaps))
                                      )
                    If operonScores.Any() Then
                        Dim bestOperon = operonScores.OrderByDescending(Function(kvp) kvp.Value).First()
                        geneToOperonMap(gene.locus_id) = (bestOperon.Key, bestOperon.Value)
                    End If
                End If
            End If
        Next

        ' --- 步骤 2: 在基因组上寻找连续的、具有相同Operon ID的基因区块，考虑距离阈值 ---
        Dim sortedGenes = allGenes.OrderBy(Function(g) g.strand) _
                         .ThenBy(Function(g) If(g.strand = "+", g.left, g.right)) _
                         .ToList()

        Dim i As Integer = 0
        While i < sortedGenes.Count
            Dim currentGene = sortedGenes(i)

            If geneToOperonMap.ContainsKey(currentGene.locus_id) Then
                Dim currentOperonId = geneToOperonMap(currentGene.locus_id).operonId
                Dim operonBlock As New List(Of GeneTable) From {currentGene}
                Dim currentStrand = currentGene.strand

                ' 向后查找扩展区块
                i += 1
                While i < sortedGenes.Count
                    Dim nextGene = sortedGenes(i)

                    ' 检查链方向是否一致
                    If nextGene.strand <> currentStrand Then
                        Exit While
                    End If

                    ' 计算当前基因与下一个基因的距离
                    Dim distance As Integer
                    If currentStrand = "+" Then
                        ' Forward链：距离 = nextGene.left - 当前区块最后一个基因的right
                        distance = nextGene.left - operonBlock.Last().right
                    Else
                        ' Reverse链：距离 = 当前区块最后一个基因的left - nextGene.right
                        ' 注意：Reverse链基因按right升序排序，但物理位置是递减的
                        distance = Math.Abs(operonBlock.Last().left - nextGene.right)
                    End If

                    ' 如果距离超过阈值，中断Operon扩展
                    If distance > geneDistCutoff Then
                        Exit While
                    End If

                    ' 检查下一个基因是否属于同一Operon或是插入基因
                    If geneToOperonMap.ContainsKey(nextGene.locus_id) Then
                        If geneToOperonMap(nextGene.locus_id).operonId = currentOperonId Then
                            operonBlock.Add(nextGene)
                            i += 1
                        Else
                            ' 下一个基因属于不同Operon，中断扩展
                            Exit While
                        End If
                    Else
                        ' 下一个基因无Operon注释，视为潜在插入基因，加入区块但不影响Operon ID
                        operonBlock.Add(nextGene)
                        i += 1
                    End If
                End While

                ' --- 步骤 3: 对区块进行分类并生成注释结果 ---
                If knownOperonsDict.ContainsKey(currentOperonId) Then
                    Yield ClassifyOperonBlock(
                    operonBlock,
                    currentOperonId,
                    knownOperonsDict(currentOperonId),
                    blastDict,
                    geneToOperonMap
                )
                End If
            Else
                i += 1
            End If
        End While
    End Function

    ''' <summary>
    ''' 辅助函数，用于对一个连续的基因区块进行Operon类型分类。
    ''' </summary>
    Private Function ClassifyOperonBlock(block As List(Of GeneTable),
                                         operonId As String,
                                         knownOperon As WebJSON.Operon,
                                         blastDict As Dictionary(Of String, HitCollection),
                                         geneToOperonMap As Dictionary(Of String, (operonId As String, score As Double))) As AnnotatedOperon
        ' 1. 准备数据
        Dim blockLocusIds = block.Select(Function(g) g.locus_id).ToHashSet()
        Dim knownHitNames = knownOperon.members.ToHashSet() ' 假设members是基因ID列表

        ' 2. 识别匹配的基因
        Dim matchedHitNames = blockLocusIds _
        .SelectMany(Function(locusId)
                        If Not blastDict.ContainsKey(locusId) Then Return {}
                        Return blastDict(locusId).hits
                    End Function) _
        .Where(Function(hit) hit.tag = operonId AndAlso knownHitNames.Contains(hit.hitName)) _
        .Select(Function(hit) hit.hitName) _
        .ToHashSet()

        ' 3. 识别缺失基因（参考Operon中未匹配的基因）
        Dim missingGeneIds = knownHitNames.Except(matchedHitNames).ToList()

        ' 4. 识别插入基因（区块中未注释到当前Operon的基因）
        Dim insertedLocusIds = blockLocusIds _
        .Where(Function(locusId)
                   If Not geneToOperonMap.ContainsKey(locusId) Then Return True
                   Return geneToOperonMap(locusId).operonId <> operonId
               End Function) _
        .ToList()

        ' 5. 确定Operon类型
        Dim opType As OperonType
        If insertedLocusIds.Any() Then
            opType = OperonType.Insertion
        ElseIf missingGeneIds.Any() Then
            opType = OperonType.Deletion
        Else
            opType = OperonType.Conserved
        End If

        ' 6. 构建Scores数组：每个基因对当前OperonID的得分
        Dim scoresArray = block _
        .Select(Function(gene)
                    If geneToOperonMap.ContainsKey(gene.locus_id) AndAlso
                       geneToOperonMap(gene.locus_id).operonId = operonId Then
                        Return geneToOperonMap(gene.locus_id).score
                    Else
                        Return 0.0
                    End If
                End Function) _
        .ToArray()

        ' 7. 计算Operon的物理位置和链方向
        Dim operonStrand = block.First().strand
        Dim operonLeft = block.Min(Function(g) g.left)
        Dim operonRight = block.Max(Function(g) g.right)

        Return New AnnotatedOperon With {
        .OperonID = operonId,
        .name = knownOperon.name,
        .Type = opType,
        .Genes = block.Select(Function(g) g.locus_id).ToArray(),
        .Scores = scoresArray,
        .KnownGeneIds = knownHitNames.ToArray(),
        .InsertedGeneIds = insertedLocusIds.ToArray(),
        .MissingGeneIds = missingGeneIds.ToArray(),
        .strand = operonStrand,
        .left = operonLeft,
        .right = operonRight
    }
    End Function
End Module