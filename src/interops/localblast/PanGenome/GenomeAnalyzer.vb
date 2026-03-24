Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Math.Statistics.Linq
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

Public Class GenomeAnalyzer

    Dim genomeNames As New HashSet(Of String)()
    Dim genomeGeneSets As New Dictionary(Of String, HashSet(Of String))()
    Dim result As New PanGenomeResult()
    Dim uf As New UnionFind()

    ''' <summary>
    ''' 全局基因注释字典（用于查询基因所属基因组）
    ''' </summary>
    Dim geneAnnotations As Dictionary(Of String, GeneInfo)
    Dim totalGenomes As Integer

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="geneAnnotations">
    ''' 所有基因的详细信息字典，Key为GeneID
    ''' </param>
    Sub New(geneAnnotations As Dictionary(Of String, GeneInfo))
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

        Call Initialize()
    End Sub

    Sub New(genomes As Dictionary(Of String, GeneInfo()))
        geneAnnotations = New Dictionary(Of String, GeneInfo)

        For Each genome In genomes
            Call genomeNames.Add(genome.Key)
            Call genomeGeneSets.Add(genome.Key, New HashSet(Of String)(From gene In genome.Value Select gene.GeneID))

            For Each gene As GeneInfo In genome.Value
                Call geneAnnotations.Add(gene.GeneID, gene)
            Next
        Next

        totalGenomes = genomeNames.Count

        Call Initialize()
    End Sub

    Sub New(genomes As IEnumerable(Of GFFTable))
        Call Me.New(genomes.ToDictionary(Function(gn) gn.species, Function(gn) GeneInfo.CreateGeneModel(gn).ToArray))
    End Sub

    Private Sub Initialize()
        ' 统计总数
        For Each kvp In genomeGeneSets
            result.TotalGenesInGenomes.Add(kvp.Key, kvp.Value.Count)
        Next

        ' ==========================================
        ' 步骤 1: 并查集聚类 (逻辑不变)
        ' ==========================================

        ' 初始化所有基因
        For Each geneId In geneAnnotations.Keys
            uf.AddElement(geneId)
        Next
    End Sub

    ''' <summary>
    ''' 执行泛基因组分析的主函数（扩展版）
    ''' </summary>
    ''' <param name="orthologDict">直系同源比对结果</param>
    ''' <returns>分析结果对象</returns>
    Public Function AnalyzePanGenome(orthologDict As Dictionary(Of String, BiDirectionalBesthit())) As PanGenomeResult
        Dim dispensableGeneFamilies As New List(Of String)
        Dim coreGeneFamilies As New List(Of String)
        Dim singleCopyOrthologFamilies As New List(Of String)
        Dim specificGeneFamilies As New List(Of String)

        ' 建立连接
        For Each kvp In orthologDict
            For Each ortho In kvp.Value
                If ortho IsNot Nothing AndAlso Not String.IsNullOrEmpty(ortho.QueryName) AndAlso Not String.IsNullOrEmpty(ortho.HitName) Then
                    uf.Union(ortho.QueryName, ortho.HitName)
                End If
            Next
        Next

        ' 构建家族映射
        Dim familyMap As New Dictionary(Of String, List(Of String))()
        For Each geneId In geneAnnotations.Keys
            Dim root = uf.Find(geneId)
            If Not familyMap.ContainsKey(root) Then familyMap.Add(root, New List(Of String)())
            familyMap(root).Add(geneId)
        Next


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

            For Each gName In genomeNames
                ' 计算该家族在当前基因组中的拷贝数
                Dim count = genes.Where(Function(g) geneAnnotations(g).GenomeName = gName).Count()
                pavRow.Add(gName, count)
                If count > 0 Then presenceCount += 1
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
        Dim rand As New Random()
        ' 曲线点：Key为加入的基因组数量，Value为(总基因平均, 核心基因平均)
        Dim curvePoints As New Dictionary(Of Integer, (SumPan As Long, SumCore As Long, Count As Integer))

        ' 模拟 iterations 次
        For i As Integer = 1 To iterations
            ' 随机打乱基因组顺序
            Dim shuffled = genomeList.OrderBy(Function(x) rand.Next()).ToList()

            Dim currentPanGenes As New HashSet(Of String)()
            Dim currentCoreCandidates As HashSet(Of String) = Nothing ' 初始化为第一个基因组的所有基因

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
                    currentBlock.OrthologyLinks = orthologyLinks.ToArray
                    Yield currentBlock
                End If
            Next
        Next
    End Function

    ''' <summary>
    ''' 基于泛基因组聚类结果和共线性分析结构变异
    ''' </summary>
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

            For Each gName In genomeNames
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
                        .RelatedGenes = genes ' 列出家族所有基因供参考
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
                        .RelatedGenes = genes.Where(Function(g) geneAnnotations(g).GenomeName = gName).ToArray
                    }
                End If

                ' --- 情况 C: 拷贝数变异 (CNV) ---
                ' 如果家族普遍存在，计算“正常”拷贝数（例如中位数）
                If isCoreFamily AndAlso copyNum > 0 Then
                    Dim medianCopy = pavRow.Values.Median

                    ' 如果拷贝数显著高于中位数（如 >= 2倍），视为扩增
                    If copyNum >= medianCopy * 2 AndAlso copyNum > 1 Then
                        svIdCounter += 1
                        Yield New StructuralVariation With {
                            .SV_ID = "SV_" & svIdCounter,
                            .Type = SVType.CNV_Gain,
                            .GenomeName = gName,
                            .FamilyID = familyId,
                            .Description = $"Copy number expansion in {gName} (Copy: {copyNum}, Median: {medianCopy}).",
                            .RelatedGenes = genes.Where(Function(g) geneAnnotations(g).GenomeName = gName).ToArray
                        }
                    ElseIf copyNum < medianCopy AndAlso copyNum > 0 Then
                        ' 这种情况较少见（核心基因拷贝减少），可视具体情况添加
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

    End Function

    ''' <summary>
    ''' 执行扩展的基因家族分类
    ''' </summary>
    Private Sub CategorizeGeneFamilies(result As PanGenomeResult, totalGenomes As Integer)
        ' 定义阈值百分比
        Dim coreThreshold As Double = 1.0 ' 100%
        Dim softCoreThreshold As Double = 0.95 ' 95%
        Dim shellThreshold As Double = 0.15 ' 15%

        For Each familyKvp In result.GeneFamilies
            Dim familyId = familyKvp.Key
            Dim pavRow = result.PAVMatrix(familyId)

            ' 计算该家族在多少个基因组中存在
            Dim presenceCount = pavRow.Values.Where(Function(c) c > 0).Count()
            Dim presenceRatio = presenceCount / totalGenomes

            ' 分类判断
            If presenceRatio = coreThreshold Then
                ' 1. 核心基因
                result.CoreGeneFamilies.Add(familyId)

            ElseIf presenceRatio >= softCoreThreshold AndAlso presenceRatio < coreThreshold Then
                ' 2. 软核心基因
                result.SoftCoreGeneFamilies.Add(familyId)

            ElseIf presenceRatio >= shellThreshold AndAlso presenceRatio < softCoreThreshold Then
                ' 3. 壳基因
                result.ShellGeneFamilies.Add(familyId)

            Else ' presenceRatio < shellThreshold
                ' 4. 云基因
                result.CloudGeneFamilies.Add(familyId)

                ' 5. 特异基因 (云基因的特例)
                If presenceCount = 1 Then
                    result.SpecificGeneFamilies.Add(familyId)
                End If
            End If
        Next
    End Sub
End Class

