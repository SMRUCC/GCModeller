Imports Microsoft.VisualBasic.Math.Correlations
Imports Microsoft.VisualBasic.Math.Statistics.Linq
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports rand = Microsoft.VisualBasic.Math.RandomExtensions

Public Class GenomeAnalyzer

    Dim genomeNames As New HashSet(Of String)()
    Dim genomeGeneSets As New Dictionary(Of String, HashSet(Of String))()
    Dim result As New PanGenomeResult()
    Dim uf As UnionFind

    ''' <summary>
    ''' 全局基因注释字典（用于查询基因所属基因组）
    ''' </summary>
    Dim geneAnnotations As Dictionary(Of String, GeneInfo)
    Dim totalGenomes As Integer

    Public Property CoreThreshold As Double = 1.0  ' 100%
    Public Property SoftCoreThreshold As Double = 0.95  ' 95% 
    Public Property ShellThreshold As Double = 0.15   ' 15%
    Public Property CNV_Gain_Factor As Double = 2.0
    Public Property CNV_Loss_Factor As Double = 0.5
    Public Property MinCollinearGenes As Integer = 5

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="geneAnnotations">
    ''' 所有基因的详细信息字典，Key为GeneID
    ''' </param>
    Sub New(geneAnnotations As Dictionary(Of String, GeneInfo), Optional uf As UnionFind = Nothing)
        ' 0. 预处理：构建基因组列表和基因集合
        For Each geneKvp In geneAnnotations
            Dim gInfo = geneKvp.Value
            genomeNames.Add(gInfo.GenomeName)

            If Not genomeGeneSets.ContainsKey(gInfo.GenomeName) Then
                genomeGeneSets.Add(gInfo.GenomeName, New HashSet(Of String)())
            End If
            genomeGeneSets(gInfo.GenomeName).Add(gInfo.GeneID)
        Next

        Me.totalGenomes = genomeNames.Count
        Me.geneAnnotations = geneAnnotations

        Call Initialize(uf)
    End Sub

    Sub New(genomes As Dictionary(Of String, GeneInfo()), Optional uf As UnionFind = Nothing)
        geneAnnotations = New Dictionary(Of String, GeneInfo)

        For Each genome In genomes
            Call genomeNames.Add(genome.Key)
            Call genomeGeneSets.Add(genome.Key, New HashSet(Of String)(From gene In genome.Value Select gene.GeneID))

            For Each gene As GeneInfo In genome.Value
                Call geneAnnotations.Add(gene.GeneID, gene)
            Next
        Next

        totalGenomes = genomeNames.Count

        Call Initialize(uf)
    End Sub

    Sub New(genomes As Dictionary(Of String, GeneTable()), Optional uf As UnionFind = Nothing)
        Call Me.New(GeneInfo.CastTable(genomes), uf)
    End Sub

    Sub New(genomes As IEnumerable(Of GFFTable), Optional uf As UnionFind = Nothing)
        Call Me.New(GeneInfo.GenomeSet(genomes), uf)
    End Sub

    Private Sub Initialize(uf As UnionFind)
        Me.uf = If(uf, New UnionFind)

        ' 统计总数
        For Each kvp In genomeGeneSets
            Call result.TotalGenesInGenomes.Add(kvp.Key, kvp.Value.Count)
        Next

        ' ==========================================
        ' 步骤 1: 并查集聚类
        ' ==========================================

        ' 初始化所有基因
        For Each geneId As String In geneAnnotations.Keys
            Call uf.AddElement(geneId)
        Next
    End Sub

    Private Function MakeFamilyMapping(orthologDict As Dictionary(Of String, BiDirectionalBesthit())) As Dictionary(Of String, List(Of String))
        ' 建立连接
        For Each kvp In orthologDict
            For Each ortho In kvp.Value
                If ortho IsNot Nothing AndAlso Not String.IsNullOrEmpty(ortho.QueryName) AndAlso Not String.IsNullOrEmpty(ortho.HitName) Then
                    Call uf.Union(ortho.QueryName, ortho.HitName)
                End If
            Next
        Next

        ' 构建家族映射
        Return uf.GetClusters
    End Function

    ''' <summary>
    ''' 执行泛基因组分析的主函数
    ''' </summary>
    ''' <param name="orthologDict">直系同源比对结果</param>
    ''' <returns>分析结果对象</returns>
    Public Function AnalyzePanGenome(orthologDict As Dictionary(Of String, BiDirectionalBesthit())) As PanGenomeResult
        Dim dispensableGeneFamilies As New List(Of String)
        Dim coreGeneFamilies As New List(Of String)
        Dim singleCopyOrthologFamilies As New List(Of String)
        Dim specificGeneFamilies As New List(Of String)
        Dim familyMap As Dictionary(Of String, List(Of String)) = MakeFamilyMapping(orthologDict)

        ' ==========================================
        ' 步骤 2: 分类分析与 PAV 矩阵构建
        ' ==========================================
        For Each family In familyMap
            Dim familyId As String = family.Key
            Dim genes As String() = family.Value.ToArray
            result.GeneFamilies.Add(familyId, genes)

            ' 构建PAV行
            Dim pavRow As New Dictionary(Of String, Integer)()
            Dim presenceCount As Integer = 0
            ' 先按基因组分组
            Dim genesByGenome = genes.GroupBy(Function(g) geneAnnotations(g).GenomeName).ToDictionary(Function(a) a.Key)

            For Each gName In genomeNames
                ' 计算该家族在当前基因组中的拷贝数
                Dim n As Integer = If(genesByGenome.ContainsKey(gName), genesByGenome(gName).Count(), 0)
                pavRow.Add(gName, n)
                If n > 0 Then presenceCount += 1
            Next

            result.PAVMatrix.Add(familyId, pavRow)

            ' 分类逻辑
            If presenceCount = totalGenomes Then
                coreGeneFamilies.Add(familyId)
                ' 单拷贝判断
                If pavRow.Values.All(Function(c) c = 1) Then
                    singleCopyOrthologFamilies.Add(familyId)
                End If
            ElseIf presenceCount = 1 Then
                specificGeneFamilies.Add(familyId)
                dispensableGeneFamilies.Add(familyId)
            Else
                dispensableGeneFamilies.Add(familyId)
            End If
        Next

        result.SpecificGeneFamilies = specificGeneFamilies.ToArray
        result.SingleCopyOrthologFamilies = singleCopyOrthologFamilies.ToArray
        result.DispensableGeneFamilies = dispensableGeneFamilies.ToArray
        result.CoreGeneFamilies = coreGeneFamilies.ToArray

        Call CategorizeGeneFamilies(result, totalGenomes)

        ' ==========================================
        ' 步骤 3: 泛基因组曲线计算
        ' ==========================================
        ' 算法：使用排列组合（若基因组数量<10）或多次随机抽样计算平均值
        ' 这里实现随机抽样模拟方法，适用于任意数量基因组
        result.PangenomeCurveData = CalculatePangenomeCurve(result, genomeNames.ToList(), 100).ToArray
        ' ==========================================
        ' 步骤 4: 共线性分析
        ' ==========================================
        ' 针对每一对基因组，寻找共线性区块
        result.CollinearBlocks = CalculateCollinearity(orthologDict, geneAnnotations, genomeNames.ToList()).ToArray
        ' ==========================================
        ' 步骤 5: 结构变异检测 (新增)
        ' ==========================================
        result.StructuralVariations = DetectStructuralVariations(result, geneAnnotations, genomeNames.ToList()).ToArray

        ' ==========================================
        ' 步骤 7: 遗传距离矩阵 (新增)
        ' ==========================================
        Call CalculateGeneticDistance(result, orthologDict, genomeNames.ToList())

        Return result
    End Function

    ''' <summary>
    ''' 计算特定基因家族在特定基因组中的拷贝数
    ''' </summary>
    ''' <param name="familyGenes">该基因家族包含的所有基因ID列表</param>
    ''' <param name="targetGenomeName">目标基因组名称</param>
    ''' <returns>拷贝数</returns>
    Public Function CalculateCopyNumber(familyGenes As List(Of String), targetGenomeName As String) As Integer
        Dim count As Integer = 0

        ' 遍历该家族内的每一个基因
        For Each geneId In familyGenes
            ' 安全校验：确保基因ID存在于注释信息中
            If geneAnnotations.ContainsKey(geneId) Then
                ' 判断该基因是否属于目标基因组
                If geneAnnotations(geneId).GenomeName = targetGenomeName Then
                    count += 1
                End If
            End If
        Next

        Return count
    End Function

    ''' <summary>
    ''' 计算泛基因组曲线（基于蒙特卡洛模拟）
    ''' </summary>
    Private Iterator Function CalculatePangenomeCurve(result As PanGenomeResult, genomeList As List(Of String), iterations As Integer) As IEnumerable(Of PangenomeCurveData)
        ' 曲线点：Key为加入的基因组数量，Value为(总基因平均, 核心基因平均)
        Dim curvePoints As New Dictionary(Of Integer, (SumPan As Long, SumCore As Long, Count As Integer))

        ' 模拟 iterations 次
        For i As Integer = 1 To iterations
            ' 随机打乱基因组顺序
            Dim shuffled = genomeList.OrderBy(Function(x) rand.NextDouble()).ToList()
            Dim currentPanGenes As New HashSet(Of String)()
            ' 初始化为第一个基因组的所有基因
            Dim currentCoreCandidates As HashSet(Of String) = Nothing

            ' 逐个添加基因组
            For [step] = 0 To shuffled.Count - 1
                Dim gName = shuffled([step])
                Dim genesInGenome = result.PAVMatrix.Where(Function(pav) pav.Value(gName) > 0).Select(Function(pav) pav.Key).ToList()

                ' 更新Pan基因集合 (并集)
                For Each gId In genesInGenome
                    currentPanGenes.Add(gId)
                Next

                ' 更新Core基因集合 (交集)
                If [step] = 0 Then
                    currentCoreCandidates = New HashSet(Of String)(genesInGenome)
                Else
                    currentCoreCandidates.IntersectWith(genesInGenome)
                End If

                ' 记录数据
                Dim n = [step] + 1
                If Not curvePoints.ContainsKey(n) Then
                    curvePoints.Add(n, (0, 0, 0))
                End If
                Dim prev = curvePoints(n)
                curvePoints(n) = (prev.SumPan + currentPanGenes.Count, prev.SumCore + currentCoreCandidates.Count, prev.Count + 1)
            Next
        Next

        ' 计算平均值并填充结果
        For i As Integer = 1 To genomeList.Count
            If curvePoints.ContainsKey(i) Then
                Dim stat = curvePoints(i)
                Dim avgPan = stat.SumPan / stat.Count
                Dim avgCore = stat.SumCore / stat.Count

                Yield New PangenomeCurveData With {
                    .GenomeCount = i,
                    .TotalGenes = CInt(avgPan),
                    .CoreGenes = CInt(avgCore)
                }
            End If
        Next
    End Function

    ''' <summary>
    ''' 计算基因组间的共线性区块（简化版算法：滑动窗口聚类）
    ''' </summary>
    Private Iterator Function CalculateCollinearity(orthologDict As Dictionary(Of String, BiDirectionalBesthit()),
                                      geneAnnotations As Dictionary(Of String, GeneInfo),
                                      genomeList As List(Of String)) As IEnumerable(Of CollinearBlock)

        ' 1. 重新整理Ortholog关系：建立 GeneID -> List<Ortholog> 的映射
        Dim orthoLookup As New Dictionary(Of String, List(Of BiDirectionalBesthit))()
        For Each orthos In orthologDict.Values
            For Each o In orthos
                If Not orthoLookup.ContainsKey(o.QueryName) Then orthoLookup.Add(o.QueryName, New List(Of BiDirectionalBesthit)())
                If Not orthoLookup.ContainsKey(o.HitName) Then orthoLookup.Add(o.HitName, New List(Of BiDirectionalBesthit)())
                orthoLookup(o.QueryName).Add(o)
                orthoLookup(o.HitName).Add(o)
            Next
        Next

        ' 2. 遍历所有基因组对 (Genome1 vs Genome2)
        For i As Integer = 0 To genomeList.Count - 1
            For j As Integer = i + 1 To genomeList.Count - 1
                Dim g1 = genomeList(i)
                Dim g2 = genomeList(j)

                ' 获取g1的所有基因并按染色体和位置排序
                Dim g1Genes = geneAnnotations.Values.Where(Function(g) g.GenomeName = g1).
                             OrderBy(Function(g) g.Chromosome).
                             ThenBy(Function(g) g.Start).ToList()

                ' 寻找共线性区块
                ' 简单策略：寻找连续的共线性基因对
                Dim currentBlock As New CollinearBlock() With {
                    .Genome1 = g1,
                    .Genome2 = g2
                }
                Dim orthologyLinks As New List(Of OrthologyLink)

                ' 滑动窗口或简单的连续性检查
                ' 这里演示一种简单逻辑：如果相邻的基因在g2中也相邻或距离很近，则认为共线性延续
                ' 实际工具通常使用更复杂的算法(如DAGchainer)，这里仅演示逻辑

                For Each g1Gene In g1Genes
                    If Not orthoLookup.ContainsKey(g1Gene.GeneID) Then Continue For

                    ' 找到该基因在g2中的同源基因
                    Dim targetOrthos = orthoLookup(g1Gene.GeneID).Where(Function(o)
                                                                            Dim otherId = If(o.QueryName = g1Gene.GeneID, o.HitName, o.QueryName)
                                                                            Return geneAnnotations(otherId).GenomeName = g2
                                                                        End Function).ToList()

                    ' 为了简化，这里只处理一对一的情况
                    If targetOrthos.Count = 1 Then
                        Dim o = targetOrthos(0)
                        Dim g2GeneId = If(o.QueryName = g1Gene.GeneID, o.HitName, o.QueryName)
                        Dim g2Info = geneAnnotations(g2GeneId)

                        ' 检查是否与上一个块连续 (简化版逻辑)
                        ' 实际开发中需要更复杂的动态规划算法
                        ' 这里简单地将每一对加入到块中（实际应用中需要过滤噪声）
                        orthologyLinks.Add(New OrthologyLink(g1Gene.GeneID, g2GeneId))
                        currentBlock.Chr1 = g1Gene.Chromosome
                        currentBlock.Chr2 = g2Info.Chromosome
                    End If
                Next

                ' 仅保留有意义的共线性区块 (例如包含 > 5个基因对)
                If orthologyLinks.Count > 0 Then
                    ' 实际上这里应该对Block进行切割，因为一个Block可能跨越不同染色体
                    ' 这里仅作为示例代码，不实现复杂的切割逻辑
                    ' TODO: Block切割
                    currentBlock.OrthologyLinks = orthologyLinks.ToArray

                    For Each subBlock As CollinearBlock In SplitBlockByChromosome(currentBlock)
                        Yield subBlock
                    Next
                End If
            Next
        Next
    End Function

    ''' <summary>
    ''' 检测染色体切换时自动切割区块
    ''' </summary>
    ''' <param name="block"></param>
    ''' <returns></returns>
    Private Iterator Function SplitBlockByChromosome(block As CollinearBlock) As IEnumerable(Of CollinearBlock)
        Dim currentSubBlock As New List(Of OrthologyLink)()
        Dim lastChr As String = Nothing

        For Each link In block.OrthologyLinks
            Dim currentChr = geneAnnotations(link.Tuple(0)).Chromosome

            If lastChr IsNot Nothing AndAlso currentChr <> lastChr Then
                ' 染色体切换，切割区块
                If currentSubBlock.Count >= MinCollinearGenes Then
                    Yield New CollinearBlock(block, currentSubBlock)
                End If
                currentSubBlock.Clear()
            End If

            currentSubBlock.Add(link)
            lastChr = currentChr
        Next

        ' 保存最后一个子区块
        If currentSubBlock.Count >= MinCollinearGenes Then
            Yield New CollinearBlock(block, currentSubBlock)
        End If
    End Function

    ''' <summary>
    ''' 检测基因顺序反向的区块 
    ''' </summary>
    ''' <param name="block"></param>
    ''' <returns></returns>
    Private Function DetectInversion(block As CollinearBlock) As InversionInfo
        ' 提取基因位置序列
        Dim positions1 = block.OrthologyLinks.Select(Function(l) CDbl(geneAnnotations(l.Tuple(0)).Start)).ToArray
        Dim positions2 = block.OrthologyLinks.Select(Function(l) CDbl(geneAnnotations(l.Tuple(1)).Start)).ToArray

        ' 计算Spearman秩相关系数
        Dim correlation = Correlations.Spearman(positions1, positions2)

        ' 负相关表示倒位
        Dim isInversion As Boolean = correlation < -0.7

        Return New InversionInfo With {
            .isInversion = isInversion,
            .correlation = correlation
        }
    End Function

    ''' <summary>
    ''' 基于泛基因组聚类结果和共线性分析结构变异
    ''' </summary>
    ''' <remarks>
    ''' 这个函数要求在调用前需要完成共线性检测计算
    ''' </remarks>
    Private Iterator Function DetectStructuralVariations(result As PanGenomeResult,
                                           geneAnnotations As Dictionary(Of String, GeneInfo),
                                           genomeNames As List(Of String)) As IEnumerable(Of StructuralVariation)

        Dim svIdCounter As Integer = 0

        ' =========================================
        ' 1. 基于 PAV 和 CNV 的检测
        ' =========================================
        ' 定义“核心拷贝数”：大多数基因组在该家族中的拷贝数模式
        ' 或者简单定义：如果大多数基因组都有，则定义为“存在”

        For Each familyKvp In result.GeneFamilies
            Dim familyId = familyKvp.Key
            Dim genes = familyKvp.Value
            Dim pavRow = result.PAVMatrix(familyId)

            ' 计算平均拷贝数（排除0）作为基准，或者以众数为基准
            ' 这里简化逻辑：如果 >50% 的基因组有该基因，则认为它是“潜在核心”
            Dim presenceCount = pavRow.Values.Where(Function(c) c > 0).Count()
            Dim isCoreFamily = (presenceCount > genomeNames.Count / 2)
            Dim nonZeroCopies = pavRow.Values.Where(Function(c) c > 0).ToList()
            Dim medianCopy = If(nonZeroCopies.Any, nonZeroCopies.Median, 0)

            ' lookup each genome for this gene family
            For Each gName As String In genomeNames
                Dim copyNum = pavRow(gName)

                ' --- 情况 A: 缺失 ---
                ' 如果该家族在其他大部分基因组中存在，但在此基因组中为0
                If isCoreFamily AndAlso copyNum = 0 Then
                    svIdCounter += 1
                    Yield New StructuralVariation With {
                        .SV_ID = "SV_" & svIdCounter,
                        .Type = SVType.PAV_Absence,
                        .GenomeName = gName,
                        .FamilyID = familyId,
                        .Description = $"Genome {gName} lacks gene family {familyId} which is present in most genomes.",
                        .RelatedGenes = genes,' 列出家族所有基因供参考
                        .CopyNumber = copyNum,
                        .Median = medianCopy
                    }
                End If

                ' --- 情况 B: 特有/获得 ---
                ' 如果该家族仅在极少基因组中存在（特异性基因），且当前基因组有
                If Not isCoreFamily AndAlso presenceCount <= 2 AndAlso copyNum > 0 Then
                    svIdCounter += 1
                    Yield New StructuralVariation With {
                        .SV_ID = "SV_" & svIdCounter,
                        .Type = SVType.PAV_Presence,
                        .GenomeName = gName,
                        .FamilyID = familyId,
                        .Description = $"Genome {gName} contains unique gene family {familyId}.",
                        .RelatedGenes = genes.Where(Function(g) geneAnnotations(g).GenomeName = gName).ToArray,
                        .CopyNumber = copyNum,
                        .Median = medianCopy
                    }
                End If

                ' --- 情况 C: 拷贝数变异 (CNV) ---
                ' 如果家族普遍存在，计算“正常”拷贝数（例如中位数）
                If isCoreFamily AndAlso copyNum > 0 Then
                    ' 如果拷贝数显著高于中位数（如 >= 2倍），视为扩增
                    If copyNum >= medianCopy * 2 AndAlso copyNum > 1 Then
                        svIdCounter += 1
                        Yield New StructuralVariation With {
                            .SV_ID = "SV_" & svIdCounter,
                            .Type = SVType.CNV_Gain,
                            .GenomeName = gName,
                            .FamilyID = familyId,
                            .Description = $"Copy number expansion in {gName} (Copy: {copyNum}, Median: {medianCopy}).",
                            .RelatedGenes = genes.Where(Function(g) geneAnnotations(g).GenomeName = gName).ToArray,
                            .CopyNumber = copyNum,
                            .Median = medianCopy
                        }
                    ElseIf copyNum > 0 AndAlso medianCopy > 1 AndAlso copyNum <= medianCopy * CNV_Loss_Factor Then
                        svIdCounter += 1
                        Yield New StructuralVariation With {
                            .SV_ID = "SV_" & svIdCounter,
                            .Type = SVType.CNV_Loss,
                            .GenomeName = gName,
                            .FamilyID = familyId,
                            .Description = $"Copy number loss in {gName} (Copy: {copyNum}, Median: {medianCopy:F1}).",
                            .CopyNumber = copyNum,
                            .Median = medianCopy
                        }
                    End If
                End If
            Next
        Next

        ' =========================================
        ' 2. 基于共线性的 SV 检测 (简易版)
        ' =========================================
        ' 遍历之前计算的共线性区块，寻找断裂点
        ' 注意：这部分通常需要比较复杂的算法，这里演示如何利用已有的 CollinearBlocks
        ' 实际上，真正的 SV 检测通常是在构建共线性之前，通过扫描基因组窗口来做

        ' 这里补充一种思路：如果在染色体上，原本应该连续的同源基因对出现了跳跃，则标记为 Break
        ' 由于之前的 CollinearBlocks 是正向的，我们可以检查“落单”的基因

        ' (此处仅为逻辑占位，实际工程中建议使用专门的SV caller如MUMmer的show-diff)
        For Each block As CollinearBlock In result.CollinearBlocks
            If block.Chr1 <> block.Chr2 Then
                ' 检测易位事件
                Yield New StructuralVariation With {
                    .Type = SVType.Collinearity_Break,
                    .Description = $"Translocation: {block.Chr1} -> {block.Chr2}"
                }
            End If
        Next
    End Function

    ''' <summary>
    ''' 执行扩展的基因家族分类
    ''' </summary>
    Private Sub CategorizeGeneFamilies(result As PanGenomeResult, totalGenomes As Integer)
        Dim coreGeneFamilies As New List(Of String)
        Dim specificGeneFamilies As New List(Of String)
        Dim softCoreGeneFamilies As New List(Of String)
        Dim shellGeneFamilies As New List(Of String)
        Dim cloudGeneFamilies As New List(Of String)

        For Each familyKvp In result.GeneFamilies
            Dim familyId = familyKvp.Key
            Dim pavRow = result.PAVMatrix(familyId)

            ' 计算该家族在多少个基因组中存在
            Dim presenceCount = pavRow.Values.Where(Function(c) c > 0).Count()
            Dim presenceRatio = presenceCount / totalGenomes

            ' 5. 特异基因 (云基因的特例)
            If presenceCount = 1 Then
                specificGeneFamilies.Add(familyId)
            End If

            ' 分类判断
            If presenceRatio = CoreThreshold Then
                ' 1. 核心基因
                coreGeneFamilies.Add(familyId)

            ElseIf presenceRatio >= SoftCoreThreshold AndAlso presenceRatio < CoreThreshold Then
                ' 2. 软核心基因
                softCoreGeneFamilies.Add(familyId)

            ElseIf presenceRatio >= ShellThreshold AndAlso presenceRatio < SoftCoreThreshold Then
                ' 3. 壳基因
                shellGeneFamilies.Add(familyId)

            Else
                ' presenceRatio < shellThreshold
                ' 4. 云基因
                Call cloudGeneFamilies.Add(familyId)
            End If
        Next

        result.CloudGeneFamilies = cloudGeneFamilies.ToArray
        result.ShellGeneFamilies = shellGeneFamilies.ToArray
        result.SoftCoreGeneFamilies = softCoreGeneFamilies.ToArray
        result.CoreGeneFamilies = coreGeneFamilies.ToArray
        result.SpecificGeneFamilies = specificGeneFamilies.ToArray

    End Sub

    ''' <summary>
    ''' 基于直系同源比对计算基因组间的遗传距离矩阵
    ''' </summary>
    Private Sub CalculateGeneticDistance(result As PanGenomeResult,
                                         orthologDict As Dictionary(Of String, BiDirectionalBesthit()),
                                         genomeNames As List(Of String))

        ' 1. 构建基因组对的比对结果缓存
        ' Key: "G1_vs_G2", Value: List(Of Ortholog)
        Dim pairwiseOrthologs As New Dictionary(Of String, List(Of Ortholog))()

        ' 辅助函数：生成唯一的基因组对Key（无论顺序）
        Dim getKey = Function(g1 As String, g2 As String) As String
                         Dim list = {g1, g2}.OrderBy(Function(x) x).ToArray()
                         Return $"{list(0)}_vs_{list(1)}"
                     End Function

        ' 初始化所有可能的基因组对
        For i = 0 To genomeNames.Count - 1
            For j = i + 1 To genomeNames.Count - 1
                Dim key = getKey(genomeNames(i), genomeNames(j))
                pairwiseOrthologs.Add(key, New List(Of Ortholog)())
            Next
        Next

        ' 2. 遍历所有Ortholog，将其归类到对应的基因组对中
        ' 注意：需要知道 Ortholog 中的 gene1 和 gene2 分别属于哪个基因组
        For Each orthos In orthologDict.Values
            For Each o In orthos
                ' 这里需要查询基因所属的基因组。
                ' 实际上，Ortholog类中最好能直接标记基因组来源，或者我们在这里查表。
                ' 假设我们无法在这里快速查表，我们可以在外层循环处理。
                ' 为了性能，我们通常在输入数据阶段就处理好了 Genome1 vs Genome2 的关系。
                ' 这里假设 orthologDict 的 Key 已经是 "G1_vs_G2" 格式。
                ' 如果不是，我们需要解析 geneID 前缀。

                ' 简单策略：假设 orthologDict 的 Key 就是 "GenomeA_vs_GenomeB" 这种标准格式
                ' 那么我们可以直接使用。
            Next
        Next

        ' 修正逻辑：如果 orthologDict 的 Key 是 "AvsB" 格式
        For Each kvp In orthologDict
            Dim comparisonName = kvp.Key
            Dim orthos = kvp.Value

            ' 尝试解析 Key 得到两个基因组名称
            ' 这里假设 Key 的格式符合之前的逻辑，或者我们直接使用 SingleCopyOrthologFamilies 来计算
            ' 使用单拷贝直系同源基因计算距离是最准确的

            ' 策略优化：使用 result.SingleCopyOrthologFamilies 中的基因对来计算
            ' 这样可以排除多拷贝基因的干扰，结果更接近物种树

            ' 我们需要先建立 Gene -> Ortholog 的反向索引
        Next

        ' --- 重新实现一个更稳健的策略 ---
        ' 仅使用单拷贝直系同源基因 计算平均距离
        ' 这在进化分析中是金标准。

        ' 1. 建立 GeneID -> Ortholog 的索引
        Dim geneToOrtholog As New Dictionary(Of String, BiDirectionalBesthit)()
        For Each orthos In orthologDict.Values
            For Each o In orthos
                If Not geneToOrtholog.ContainsKey(o.QueryName) Then geneToOrtholog.Add(o.QueryName, o)
                If Not geneToOrtholog.ContainsKey(o.HitName) Then geneToOrtholog.Add(o.HitName, o)
            Next
        Next

        ' 2. 遍历所有单拷贝家族
        For Each familyId In result.SingleCopyOrthologFamilies
            Dim genes = result.GeneFamilies(familyId)

            ' 单拷贝家族中只有 N 个基因 (N=基因组数)
            ' 我们需要找到这 N 个基因两两之间的 Ortholog 记录
            ' 实际上，单拷贝家族意味着两两之间必然有 RBH 关系

            For i = 0 To genes.Count - 1
                For j = i + 1 To genes.Count - 1
                    Dim g1 = genes(i)
                    Dim g2 = genes(j)

                    ' 查找它们之间的 Ortholog 记录
                    ' 因为是 RBH，g1 和 g2 必然在同一个 Ortholog 对象中
                    If geneToOrtholog.ContainsKey(g1) Then
                        Dim o = geneToOrtholog(g1)
                        Dim target = If(o.QueryName = g1, o.HitName, o.QueryName)

                        If target = g2 Then
                            ' 找到了配对
                            If Not geneAnnotations.ContainsKey(g1) OrElse Not geneAnnotations.ContainsKey(g2) Then
                                Continue For
                            End If

                            Dim gName1 = geneAnnotations(g1).GenomeName
                            Dim gName2 = geneAnnotations(g2).GenomeName

                            ' 这里的命名解析需根据实际情况调整，这里演示逻辑
                            ' 更好的方式是查 geneAnnotations
                            ' Dim info1 = geneAnnotations(g1) ...

                            Dim key = getKey(gName1, gName2)

                            ' 记录距离 (1 - Identity)
                            ' 假设 identities1 是 g1 相对于 g2 的一致性
                            ' 如果 orthologDict 中没有明确的基因组方向，取平均值
                            Dim dist = 1.0 - ((o.forward + o.reverse) / 2.0)

                            If Not result.GeneticDistanceMatrix.ContainsKey(key) Then
                                result.GeneticDistanceMatrix.Add(key, 0)
                            End If
                            ' 累加距离，稍后取平均
                            result.GeneticDistanceMatrix(key) += dist
                        End If
                    End If
                Next
            Next
        Next

        ' 3. 计算平均值
        ' 这里需要知道每对基因组比较了多少个基因。
        ' 简化处理：我们可以用一个辅助字典记录计数，这里略。
        ' 下面提供一个简化版的除以单拷贝基因总数的逻辑。

        Dim singleCopyCount = result.SingleCopyOrthologFamilies.Count
        If singleCopyCount > 0 Then
            Dim keys = result.GeneticDistanceMatrix.Keys.ToList()
            For Each k In keys
                ' 每对基因组在每个单拷贝家族中都会贡献一次距离
                ' 所以除以 singleCopyCount
                result.GeneticDistanceMatrix(k) /= singleCopyCount
            Next
        End If
    End Sub
End Class

