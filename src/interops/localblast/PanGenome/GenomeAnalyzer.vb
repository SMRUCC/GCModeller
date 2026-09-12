#Region "Microsoft.VisualBasic::bd55f202051fe996a721f05267c12756, localblast\PanGenome\GenomeAnalyzer.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 757
    '    Code Lines: 456 (60.24%)
    ' Comment Lines: 191 (25.23%)
    '    - Xml Docs: 30.89%
    ' 
    '   Blank Lines: 110 (14.53%)
    '     File Size: 35.10 KB


    ' Class GenomeAnalyzer
    ' 
    '     Properties: CNV_Gain_Factor, CNV_Loss_Factor, CoreThreshold, MinCollinearGenes, ShellThreshold
    '                 SoftCoreThreshold
    ' 
    '     Constructor: (+4 Overloads) Sub New
    ' 
    '     Function: AnalyzePanGenome, CalculateCollinearity, CalculateCopyNumber, CalculatePangenomeCurve, DetectInversion
    '               DetectStructuralVariations, MakeFamilyMapping, OrderKey, SplitBlockByChromosome
    ' 
    '     Sub: CalculateGeneticDistance, CalculatePanGenomeJaccardDistance, CategorizeGeneFamilies, Initialize, SetGenesElements
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Diagnostics
Imports System.Numerics
Imports System.Threading
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Correlations
Imports Microsoft.VisualBasic.Math.Statistics.Linq
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports rand = Microsoft.VisualBasic.Math.RandomExtensions

''' <summary>
''' 泛基因组分析器
''' </summary>
''' <remarks>
''' 整个分析过程被重构为：
''' 
''' 1. 索引化：在分析开始之前，一次性把所有的字符串主键（基因ID、基因组名、家族ID）
'''    映射为连续的整数下标，后续的热循环全部在并行数组(<see cref="Integer"/>)上进行，
'''    避免上百万次的字符串哈希查找；
''' 2. 去二次化：直系同源分组（例如cd-hit的cluster）直接做 k-1 次并查集合并，
'''    不再生成 O(k^2) 的两两配对中间对象；共线性分析不再对每个基因组对做全基因表扫描；
'''    泛基因组曲线由 O(迭代数 x 基因组数 x 家族数) 的全表扫描改为 O(迭代数 x 基因数) 的增量计数；
''' 3. 并行化：家族聚类之后相互独立的计算单元（基因家族、基因组对、蒙特卡洛迭代）
'''    全部通过 <see cref="Parallel"/> 做数据并行。
''' </remarks>
Public Class GenomeAnalyzer

    ''' <summary>
    ''' 当基因组的数量超过这个阈值的时候，共线性结果中将只会保留区块的统计信息，
    ''' 不再保留逐基因的同源配对数据，以避免占用过大的内存
    ''' </summary>
    Const MaxRetainLinkGenomes As Integer = 32

    ReadOnly genomeNames As New HashSet(Of String)()
    Dim result As New PanGenomeResult()
    Dim uf As UnionFind

    ''' <summary>
    ''' 全局基因注释字典（用于查询基因所属基因组）
    ''' </summary>
    ReadOnly geneAnnotations As Dictionary(Of String, GeneInfo)
    Dim totalGenomes As Integer

#Region "整数索引化的分析上下文"

    ''' <summary>下标 -> 基因ID</summary>
    Dim geneIds As String()
    ''' <summary>基因ID -> 下标</summary>
    Dim geneIndex As Dictionary(Of String, Integer)
    ''' <summary>下标 -> 所属基因组下标</summary>
    Dim geneGenome As Integer()
    Dim geneChr As String()
    Dim geneStart As Integer()
    Dim geneEnd As Integer()

    ''' <summary>有序的基因组名称列表</summary>
    Dim genomeList As String()
    Dim genomeIndex As Dictionary(Of String, Integer)
    ''' <summary>每个基因组内按照(染色体, 起始位点)排序好的基因下标，只排序一次供所有算法复用</summary>
    Dim genomeGenes As Integer()()

    ''' <summary>下标 -> 家族ID</summary>
    Dim familyIds As String()
    ''' <summary>家族下标 -> 该家族内的基因下标</summary>
    Dim familyMembers As Integer()()
    ''' <summary>家族下标 -> 该家族内的基因ID</summary>
    Dim familyGeneIds As String()()
    ''' <summary>基因下标 -> 家族下标（-1表示未聚类）</summary>
    Dim geneFamily As Integer()

    ''' <summary>基因组下标 -> (家族下标 -> 该基因组内的唯一基因下标，多拷贝或者不存在为-1)</summary>
    Dim familyToGene As Dictionary(Of Integer, Integer)()

#End Region

    Public Property CoreThreshold As Double = 1.0  ' 100%
    Public Property SoftCoreThreshold As Double = 0.95  ' 95% 
    Public Property ShellThreshold As Double = 0.15   ' 15%
    Public Property CNV_Gain_Factor As Double = 2.0
    Public Property CNV_Loss_Factor As Double = 0.5
    Public Property MinCollinearGenes As Integer = 5

    ''' <summary>
    ''' 是否在共线性结果之中保留逐基因的同源配对数据？
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' 在基因组数量非常多的时候（例如几百个基因组的两两比较），同源配对的数据量是
    ''' O(N^2 x 平均每基因组基因数)，会占用非常巨大的内存。默认情况下当基因组数量
    ''' 大于 <see cref="MaxRetainLinkGenomes"/> 的时候这个开关会被自动关闭，
    ''' 只保留 <see cref="CollinearBlock.LinkCount"/> 统计信息。
    ''' </remarks>
    Public Property RetainOrthologyLinks As Boolean = True

    ''' <summary>
    ''' 泛基因组曲线的蒙特卡洛模拟迭代次数
    ''' </summary>
    ''' <returns></returns>
    Public Property CurveIterations As Integer = 100

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="geneAnnotations">
    ''' 所有基因的详细信息字典，Key为GeneID
    ''' </param>
    Sub New(geneAnnotations As Dictionary(Of String, GeneInfo), Optional uf As UnionFind = Nothing)
        Me.geneAnnotations = geneAnnotations
        Me.uf = If(uf, New UnionFind)

        Call BuildIndex()
        Call Initialize()
    End Sub

    Sub New(genomes As Dictionary(Of String, GeneInfo()), Optional uf As UnionFind = Nothing)
        Call Me.New(genomes.Values.IteratesALL.MakeUniqueNames.ToDictionary(Function(gene) gene.GeneID), uf)
    End Sub

    Sub New(genomes As Dictionary(Of String, GeneTable()), Optional uf As UnionFind = Nothing, Optional uniqueByAccessionId As Boolean = False)
        Call Me.New(GeneInfo.CastTable(genomes, uniqueByAccessionId), uf)
    End Sub

    Sub New(genomes As IEnumerable(Of GFFTable), Optional uf As UnionFind = Nothing)
        Call Me.New(GeneInfo.GenomeSet(genomes), uf)
    End Sub

    ''' <summary>
    ''' 一次性构建好所有的整数索引，后续的分析过程将不再需要针对上百万个基因
    ''' 反复做字符串哈希查找
    ''' </summary>
    Private Sub BuildIndex()
        Dim n As Integer = geneAnnotations.Count
        Dim sw As Stopwatch = Stopwatch.StartNew

        geneIds = New String(n - 1) {}
        geneIndex = New Dictionary(Of String, Integer)(n)
        geneGenome = New Integer(n - 1) {}
        geneChr = New String(n - 1) {}
        geneStart = New Integer(n - 1) {}
        geneEnd = New Integer(n - 1) {}
        genomeIndex = New Dictionary(Of String, Integer)()

        Dim buckets As New Dictionary(Of Integer, List(Of Integer))()
        Dim i As Integer = 0

        For Each geneKvp As KeyValuePair(Of String, GeneInfo) In geneAnnotations
            Dim gInfo As GeneInfo = geneKvp.Value
            Dim gi As Integer

            If Not genomeIndex.ContainsKey(gInfo.GenomeName) Then
                gi = genomeIndex.Count
                genomeIndex.Add(gInfo.GenomeName, gi)
                genomeNames.Add(gInfo.GenomeName)
                buckets.Add(gi, New List(Of Integer)(512))
            Else
                gi = genomeIndex(gInfo.GenomeName)
            End If

            geneIds(i) = gInfo.GeneID
            geneIndex(gInfo.GeneID) = i
            geneGenome(i) = gi
            geneChr(i) = gInfo.Chromosome
            geneStart(i) = gInfo.Start
            geneEnd(i) = gInfo.[End]
            buckets(gi).Add(i)

            i += 1
        Next

        totalGenomes = genomeNames.Count
        genomeList = genomeNames.ToArray()
        genomeGenes = New Integer(totalGenomes - 1)() {}

        Dim cmp As New Comparison(Of Integer)(AddressOf CompareGeneLocus)

        For Each kvp As KeyValuePair(Of Integer, List(Of Integer)) In buckets
            Dim genesOfGenome As Integer() = kvp.Value.ToArray()

            ' 按照(染色体, 起始位点)排序，共线性分析依赖于这个顺序
            Call Array.Sort(genesOfGenome, cmp)

            genomeGenes(kvp.Key) = genesOfGenome
            result.TotalGenesInGenomes.Add(genomeList(kvp.Key), genesOfGenome.Length)
        Next

        Call $"[pan-genome] index {n} genes of {totalGenomes} genomes, elapsed {sw.ElapsedMilliseconds} ms".debug
    End Sub

    ''' <summary>
    ''' 基因组内的基因排序比较函数：先按照染色体，再按照起始位点
    ''' </summary>
    Private Function CompareGeneLocus(a As Integer, b As Integer) As Integer
        Dim c As Integer = String.CompareOrdinal(geneChr(a), geneChr(b))

        If c = 0 Then
            Return geneStart(a).CompareTo(geneStart(b))
        Else
            Return c
        End If
    End Function

    Private Sub Initialize()
        ' 默认在大基因组集合上面关闭逐基因的共线性配对输出
        Me.RetainOrthologyLinks = totalGenomes <= MaxRetainLinkGenomes
        Me.SetGenesElements(geneIds)
    End Sub

    Private Sub SetGenesElements(gene_ids As IEnumerable(Of String))
        ' 初始化所有基因
        Call uf.AddElements(gene_ids)
    End Sub

    ''' <summary>
    ''' 通过BBH两两比对结果建立基因家族（并查集合并）
    ''' </summary>
    Private Sub MakeFamilyMapping(orthologDict As Dictionary(Of String, BiDirectionalBesthit()))
        ' 建立连接
        For Each kvp In orthologDict
            For Each ortho In kvp.Value
                If ortho IsNot Nothing AndAlso Not String.IsNullOrEmpty(ortho.QueryName) AndAlso Not String.IsNullOrEmpty(ortho.HitName) Then
                    Call uf.Union(ortho.QueryName, ortho.HitName)
                End If
            Next
        Next
    End Sub

    ''' <summary>
    ''' 从并查集之中提取出基因家族，并且建立家族的整数索引
    ''' </summary>
    Private Sub BuildFamilyIndex()
        Dim clusters As Dictionary(Of String, List(Of String)) = uf.GetClusters
        Dim f As Integer = 0

        familyIds = New String(clusters.Count - 1) {}
        familyMembers = New Integer(clusters.Count - 1)() {}
        familyGeneIds = New String(clusters.Count - 1)() {}
        geneFamily = New Integer(geneIds.Length - 1) {}

        For i As Integer = 0 To geneFamily.Length - 1
            geneFamily(i) = -1
        Next

        For Each kvp As KeyValuePair(Of String, List(Of String)) In clusters
            Dim members As New List(Of Integer)(kvp.Value.Count)

            For Each geneId As String In kvp.Value
                Dim gi As Integer = -1

                If geneIndex.TryGetValue(geneId, gi) Then
                    members.Add(gi)
                    geneFamily(gi) = f
                End If
            Next

            familyIds(f) = kvp.Key
            familyMembers(f) = members.ToArray()
            familyGeneIds(f) = kvp.Value.ToArray()

            f += 1
        Next
    End Sub

    ''' <summary>
    ''' 执行泛基因组分析的主函数
    ''' </summary>
    ''' <param name="orthologDict">直系同源比对结果（BBH）</param>
    ''' <returns>分析结果对象</returns>
    Public Function AnalyzePanGenome(orthologDict As Dictionary(Of String, BiDirectionalBesthit())) As PanGenomeResult
        Dim sw As Stopwatch = Stopwatch.StartNew

        Call MakeFamilyMapping(orthologDict)
        Call $"[pan-genome] merge {orthologDict.Values.Sum(Function(a) a.Length)} BBH links, elapsed {sw.ElapsedMilliseconds} ms".debug

        Return RunAnalysis(orthologDict)
    End Function

    ''' <summary>
    ''' 执行泛基因组分析的主函数（直接基于直系同源分组，例如cd-hit的聚类结果）
    ''' </summary>
    ''' <param name="orthoGroups">
    ''' Key为家族/聚类ID，Value为该家族之内的基因ID集合。
    ''' 一个包含k个基因的家族只需要 k-1 次并查集合并操作，
    ''' 不需要先展开为 O(k^2) 个两两配对关系。
    ''' </param>
    ''' <returns>分析结果对象</returns>
    Public Function AnalyzePanGenome(orthoGroups As Dictionary(Of String, String())) As PanGenomeResult
        Dim sw As Stopwatch = Stopwatch.StartNew

        For Each group As KeyValuePair(Of String, String()) In orthoGroups
            Call uf.UnionRange(group.Value)
        Next

        Call $"[pan-genome] merge {orthoGroups.Count} ortholog groups, elapsed {sw.ElapsedMilliseconds} ms".debug

        Return RunAnalysis(Nothing)
    End Function

    ''' <summary>
    ''' 家族聚类完成之后的所有分析步骤
    ''' </summary>
    Private Function RunAnalysis(orthologDict As Dictionary(Of String, BiDirectionalBesthit())) As PanGenomeResult
        Dim sw As Stopwatch = Stopwatch.StartNew
        Dim F As Integer
        Dim N As Integer = totalGenomes

        Call BuildFamilyIndex()

        F = familyIds.Length

        Call $"[pan-genome] build {F} gene families from {geneIds.Length} genes / {N} genomes, elapsed {sw.ElapsedMilliseconds} ms".debug

        If F = 0 OrElse N = 0 Then
            Call "[pan-genome] empty gene set, analysis terminated!".warning
            Return result
        End If

        ' ==========================================
        ' 步骤 1+2: PAV 矩阵构建与基因家族分类（并行）
        ' ==========================================
        sw.Restart()
        Call BuildPAVAndClassify(F, N)
        Call $"[pan-genome] PAV matrix + family categorize done, {F} families, elapsed {sw.ElapsedMilliseconds} ms".debug

        ' ==========================================
        ' 步骤 3: 遗传距离矩阵
        ' ==========================================
        sw.Restart()

        If orthologDict Is Nothing OrElse result.SingleCopyOrthologFamilies.IsNullOrEmpty Then
            Call CalculatePanGenomeJaccardDistance()
        Else
            Call CalculateGeneticDistance(orthologDict, genomeList.ToList())
        End If

        Call $"[pan-genome] genetic distance matrix done, {result.GeneticDistanceMatrix.Count} pairs, elapsed {sw.ElapsedMilliseconds} ms".debug

        ' ==========================================
        ' 步骤 4: 共线性分析
        ' ==========================================
        sw.Restart()
        result.CollinearBlocks = CalculateCollinearity(orthologDict)
        Call $"[pan-genome] collinearity done, {result.CollinearBlocks.Length} blocks (retain links: {RetainOrthologyLinks}), elapsed {sw.ElapsedMilliseconds} ms".debug

        ' ==========================================
        ' 步骤 5: 结构变异检测
        ' ==========================================
        sw.Restart()
        result.StructuralVariations = DetectStructuralVariations()
        Call $"[pan-genome] structural variation done, {result.StructuralVariations.Length} events, elapsed {sw.ElapsedMilliseconds} ms".debug

        ' ==========================================
        ' 步骤 6: 泛基因组曲线计算
        ' ==========================================
        sw.Restart()
        result.PangenomeCurveData = CalculatePangenomeCurve(CurveIterations)
        Call $"[pan-genome] pangenome curve done, elapsed {sw.ElapsedMilliseconds} ms".debug

        Return result
    End Function

    ''' <summary>
    ''' 并行构建PAV矩阵并同时对基因家族做分类
    ''' </summary>
    Private Sub BuildPAVAndClassify(F As Integer, N As Integer)
        Dim pavRows(F - 1) As Dictionary(Of String, Integer)
        Dim isCore As Boolean() = New Boolean(F - 1) {}
        Dim isSoftCore As Boolean() = New Boolean(F - 1) {}
        Dim isShell As Boolean() = New Boolean(F - 1) {}
        Dim isCloud As Boolean() = New Boolean(F - 1) {}
        Dim isSpecific As Boolean() = New Boolean(F - 1) {}
        Dim isDispensable As Boolean() = New Boolean(F - 1) {}
        Dim isSingleCopy As Boolean() = New Boolean(F - 1) {}
        Dim strictSingleCopy As Boolean = False
        Dim coreThreshold As Double = Me.CoreThreshold
        Dim softCore As Double = Me.SoftCoreThreshold
        Dim shell As Double = Me.ShellThreshold

        Parallel.For(0, F, Sub(k As Integer)
                               Dim members As Integer() = familyMembers(k)
                               Dim counts As Integer() = New Integer(N - 1) {}
                               Dim presence As Integer = 0
                               Dim g As Integer

                               For Each gi As Integer In members
                                   counts(geneGenome(gi)) += 1
                               Next

                               For g = 0 To N - 1
                                   If counts(g) > 0 Then
                                       presence += 1
                                   End If
                               Next

                               ' 稀疏行：只保存非零的拷贝数
                               ' 所有的读取端(PAVTable/GetPAVMatrix/报告)对缺失的键都是当作0来处理的
                               Dim row As New Dictionary(Of String, Integer)(If(presence > 0, presence, 1))

                               For g = 0 To N - 1
                                   If counts(g) > 0 Then
                                       row.Add(genomeList(g), counts(g))
                                   End If
                               Next

                               pavRows(k) = row

                               Dim ratio As Double = presence / N

                               isDispensable(k) = (presence < N)
                               isSpecific(k) = (presence = 1)

                               If ratio = coreThreshold Then
                                   isCore(k) = True
                               ElseIf ratio >= softCore AndAlso ratio < coreThreshold Then
                                   isSoftCore(k) = True
                               ElseIf ratio >= shell AndAlso ratio < softCore Then
                                   isShell(k) = True
                               Else
                                   isCloud(k) = True
                               End If

                               If presence = N Then
                                   ' 单拷贝判断
                                   If strictSingleCopy Then
                                       Dim allOne As Boolean = True

                                       For g = 0 To N - 1
                                           If counts(g) <> 1 Then
                                               allOne = False
                                               Exit For
                                           End If
                                       Next

                                       isSingleCopy(k) = allOne
                                   Else
                                       Dim allSmall As Boolean = True

                                       For g = 0 To N - 1
                                           If counts(g) >= 5 Then
                                               allSmall = False
                                               Exit For
                                           End If
                                       Next

                                       isSingleCopy(k) = allSmall
                                   End If
                               End If
                           End Sub)

        ' 按照家族下标的顺序写入结果，保证输出顺序是确定的
        Dim coreList As New List(Of String)()
        Dim softCoreList As New List(Of String)()
        Dim shellList As New List(Of String)()
        Dim cloudList As New List(Of String)()
        Dim specificList As New List(Of String)()
        Dim dispensableList As New List(Of String)()
        Dim singleList As New List(Of String)()

        For k As Integer = 0 To F - 1
            Dim id As String = familyIds(k)

            Call result.GeneFamilies.Add(id, familyGeneIds(k))
            Call result.PAVMatrix.Add(id, pavRows(k))

            If isCore(k) Then coreList.Add(id)
            If isSoftCore(k) Then softCoreList.Add(id)
            If isShell(k) Then shellList.Add(id)
            If isCloud(k) Then cloudList.Add(id)
            If isSpecific(k) Then specificList.Add(id)
            If isDispensable(k) Then dispensableList.Add(id)
            If isSingleCopy(k) Then singleList.Add(id)
        Next

        result.CoreGeneFamilies = coreList.ToArray
        result.SoftCoreGeneFamilies = softCoreList.ToArray
        result.ShellGeneFamilies = shellList.ToArray
        result.CloudGeneFamilies = cloudList.ToArray
        result.SpecificGeneFamilies = specificList.ToArray
        result.DispensableGeneFamilies = dispensableList.ToArray
        result.SingleCopyOrthologFamilies = singleList.ToArray
    End Sub

    ''' <summary>
    ''' 计算特定基因家族在特定基因组中的拷贝数
    ''' </summary>
    ''' <param name="familyGenes">该基因家族包含的所有基因ID列表</param>
    ''' <param name="targetGenomeName">目标基因组名称</param>
    ''' <returns>拷贝数</returns>
    Public Function CalculateCopyNumber(familyGenes As List(Of String), targetGenomeName As String) As Integer
        Dim count As Integer = 0
        Dim target As Integer = -1

        If genomeIndex IsNot Nothing AndAlso genomeIndex.ContainsKey(targetGenomeName) Then
            target = genomeIndex(targetGenomeName)
        End If

        If target < 0 Then
            Return 0
        End If

        ' 遍历该家族内的每一个基因
        For Each geneId In familyGenes
            Dim gi As Integer = -1

            ' 安全校验：确保基因ID存在于注释信息中
            If geneIndex.TryGetValue(geneId, gi) Then
                ' 判断该基因是否属于目标基因组
                If geneGenome(gi) = target Then
                    count += 1
                End If
            End If
        Next

        Return count
    End Function

    ''' <summary>
    ''' 计算泛基因组曲线（基于蒙特卡洛模拟）
    ''' </summary>
    ''' <remarks>
    ''' 原来的实现在每一次迭代的每一个基因组上面都要对全部基因家族做一次LINQ全表扫描，
    ''' 复杂度为 O(迭代数 x 基因组数 x 家族数)。
    ''' 
    ''' 这里改为增量计数：为每一次迭代维护一个家族计数器，加入第 i 个基因组的时候，
    ''' 该基因组的家族集合 S 之内：
    ''' 
    '''   * 计数为0的家族 -> 泛基因组大小 +1；
    '''   * 计数为i的家族 -> 说明该家族在已经加入的i个基因组之中都存在，
    '''     加入当前基因组之后仍然为核心基因，核心基因数量就是这类家族的数量。
    ''' 
    ''' 复杂度降低为 O(迭代数 x 基因总数)。
    ''' </remarks>
    Private Function CalculatePangenomeCurve(iterations As Integer) As PangenomeCurveData()
        Dim N As Integer = totalGenomes
        Dim F As Integer = familyIds.Length

        If N = 0 OrElse F = 0 Then
            Return New PangenomeCurveData() {}
        End If

        ' 每个基因组去重之后的家族下标集合
        Dim genomeFams As Integer()() = New Integer(N - 1)() {}

        Parallel.For(0, N, Sub(g As Integer)
                               Dim seen As New HashSet(Of Integer)()

                               For Each gi As Integer In genomeGenes(g)
                                   Dim famIdx As Integer = geneFamily(gi)

                                   If famIdx >= 0 Then
                                       seen.Add(famIdx)
                                   End If
                               Next

                               genomeFams(g) = seen.ToArray()
                           End Sub)

        ' 预先生成随机排列，保证并行模拟的结果是可以复现的
        Dim perms As Integer()() = New Integer(iterations - 1)() {}
        Dim rnd As New Random(20260101)

        For it As Integer = 0 To iterations - 1
            Dim ord As Integer() = New Integer(N - 1) {}

            For i As Integer = 0 To N - 1
                ord(i) = i
            Next
            For i As Integer = N - 1 To 1 Step -1
                Dim j As Integer = rnd.Next(i + 1)
                Dim tmp As Integer = ord(i)

                ord(i) = ord(j)
                ord(j) = tmp
            Next

            perms(it) = ord
        Next

        Dim sumPan As Long() = New Long(N - 1) {}
        Dim sumCore As Long() = New Long(N - 1) {}

        Parallel.For(0, iterations, Sub(it As Integer)
                                        Dim cnt As Integer() = New Integer(F - 1) {}
                                        Dim ord As Integer() = perms(it)
                                        Dim pan As Integer = 0

                                        For [step] As Integer = 0 To N - 1
                                            Dim fams As Integer() = genomeFams(ord([step]))
                                            Dim core As Integer = 0

                                            For Each famIdx As Integer In fams
                                                Dim c As Integer = cnt(famIdx)

                                                If c = 0 Then
                                                    pan += 1
                                                End If
                                                If c = [step] Then
                                                    core += 1
                                                End If

                                                cnt(famIdx) = c + 1
                                            Next

                                            Call Interlocked.Add(sumPan([step]), pan)
                                            Call Interlocked.Add(sumCore([step]), core)
                                        Next
                                    End Sub)

        ' 计算平均值并填充结果
        Dim curve As PangenomeCurveData() = New PangenomeCurveData(N - 1) {}

        For i As Integer = 0 To N - 1
            curve(i) = New PangenomeCurveData With {
                .GenomeCount = i + 1,
                .TotalGenes = CInt(sumPan(i) / iterations),
                .CoreGenes = CInt(sumCore(i) / iterations)
            }
        Next

        Return curve
    End Function

    ''' <summary>
    ''' 建立 基因组 -> (家族 -> 基因) 的映射，用于快速的查找某个家族在某个基因组内的唯一同源基因
    ''' </summary>
    Private Sub BuildFamilyToGeneMap()
        If familyToGene IsNot Nothing Then
            Return
        End If

        Dim maps(totalGenomes - 1) As Dictionary(Of Integer, Integer)()

        familyToGene = maps

        Parallel.For(0, totalGenomes, Sub(g As Integer)
                                          Dim genes As Integer() = genomeGenes(g)
                                          Dim map As New Dictionary(Of Integer, Integer)(genes.Length)

                                          For Each gi As Integer In genes
                                              Dim famIdx As Integer = geneFamily(gi)

                                              If famIdx < 0 Then
                                                  Continue For
                                              End If
                                              If map.ContainsKey(famIdx) Then
                                                  ' 该家族在这个基因组内存在多个拷贝，标记为非单拷贝
                                                  map(famIdx) = -1
                                              Else
                                                  map.Add(famIdx, gi)
                                              End If
                                          Next

                                          familyToGene(g) = map
                                      End Sub)
    End Sub

    ''' <summary>
    ''' 计算基因组间的共线性区块（并行版）
    ''' </summary>
    ''' <param name="orthologDict">
    ''' BBH两两比对结果；如果这个参数为空，则直接基于基因家族索引推导同源关系
    ''' </param>
    ''' <remarks>
    ''' 原来的实现对每一个基因组对都做了一次全基因表的扫描与排序，
    ''' 复杂度为 O(N^2 x 基因总数)，在几百个基因组的数据集上面是不可用的。
    ''' 这里改为：基因按照基因组预分组并且只排序一次；同源关系通过预建的
    ''' 家族索引做 O(1) 查找；基因组对之间并行处理，处理完之后立即丢弃中间结果。
    ''' </remarks>
    Private Function CalculateCollinearity(orthologDict As Dictionary(Of String, BiDirectionalBesthit())) As CollinearBlock()
        Dim N As Integer = totalGenomes

        If N < 2 Then
            Return New CollinearBlock() {}
        End If

        ' BBH路径：保留基于真实比对配对的共线性语义
        Dim orthoLookup As Dictionary(Of String, List(Of BiDirectionalBesthit)) = Nothing

        If orthologDict IsNot Nothing Then
            orthoLookup = New Dictionary(Of String, List(Of BiDirectionalBesthit))()

            For Each orthos In orthologDict.Values
                For Each o In orthos
                    If Not orthoLookup.ContainsKey(o.QueryName) Then orthoLookup.Add(o.QueryName, New List(Of BiDirectionalBesthit)())
                    If Not orthoLookup.ContainsKey(o.HitName) Then orthoLookup.Add(o.HitName, New List(Of BiDirectionalBesthit)())
                    orthoLookup(o.QueryName).Add(o)
                    orthoLookup(o.HitName).Add(o)
                Next
            Next
        Else
            ' 分组路径：直接由基因家族索引推导同源关系
            Call BuildFamilyToGeneMap()
        End If

        Dim blocksOfGenome(N - 1) As List(Of CollinearBlock)

        ' 以第一个基因组为外层做并行，保证最后合并出来的区块顺序是确定的
        Parallel.For(0, N - 1, Sub(i As Integer)
                                   Dim list As New List(Of CollinearBlock)()

                                   For j As Integer = i + 1 To N - 1
                                       For Each block As CollinearBlock In CalculatePairCollinearity(i, j, orthoLookup)
                                           list.Add(block)
                                       Next
                                   Next

                                   blocksOfGenome(i) = list
                               End Sub)

        Dim all As New List(Of CollinearBlock)()

        For i As Integer = 0 To N - 1
            If blocksOfGenome(i) IsNot Nothing Then
                all.AddRange(blocksOfGenome(i))
            End If
        Next

        Return all.ToArray()
    End Function

    ''' <summary>
    ''' 计算一对基因组之间的共线性区块
    ''' </summary>
    Private Iterator Function CalculatePairCollinearity(i As Integer, j As Integer,
                                                        orthoLookup As Dictionary(Of String, List(Of BiDirectionalBesthit))) As IEnumerable(Of CollinearBlock)

        Dim links As New List(Of OrthologyLink)()
        Dim queryIdx As New List(Of Integer)()
        Dim g1Genes As Integer() = genomeGenes(i)
        Dim chr1 As String = Nothing
        Dim chr2 As String = Nothing

        If orthoLookup Is Nothing Then
            ' 基于基因家族索引：查找该家族在基因组j内的唯一基因
            Dim map2 As Dictionary(Of Integer, Integer) = familyToGene(j)

            For Each gi As Integer In g1Genes
                Dim famIdx As Integer = geneFamily(gi)

                If famIdx < 0 Then
                    Continue For
                End If

                Dim gj As Integer = -1

                If map2.TryGetValue(famIdx, gj) AndAlso gj >= 0 Then
                    links.Add(New OrthologyLink(geneIds(gi), geneIds(gj)))
                    queryIdx.Add(gi)
                    chr1 = geneChr(gi)
                    chr2 = geneChr(gj)
                End If
            Next
        Else
            ' BBH路径：只有存在唯一一条指向基因组j的比对记录的时候才认为是1:1的同源基因
            For Each gi As Integer In g1Genes
                Dim geneId As String = geneIds(gi)
                Dim orthos As List(Of BiDirectionalBesthit) = Nothing

                If Not orthoLookup.TryGetValue(geneId, orthos) Then
                    Continue For
                End If

                Dim hit As Integer = -1
                Dim n As Integer = 0

                For Each o As BiDirectionalBesthit In orthos
                    Dim otherId As String = If(o.QueryName = geneId, o.HitName, o.QueryName)
                    Dim oj As Integer = -1

                    If geneIndex.TryGetValue(otherId, oj) AndAlso geneGenome(oj) = j Then
                        hit = oj
                        n += 1
                    End If
                Next

                If n = 1 Then
                    links.Add(New OrthologyLink(geneId, geneIds(hit)))
                    queryIdx.Add(gi)
                    chr1 = geneChr(gi)
                    chr2 = geneChr(hit)
                End If
            Next
        End If

        If links.Count = 0 Then
            Return
        End If

        Dim block As New CollinearBlock() With {
            .Genome1 = genomeList(i),
            .Genome2 = genomeList(j),
            .Chr1 = chr1,
            .Chr2 = chr2
        }

        ' 按照染色体切换自动切割区块
        Dim subBlock As New List(Of OrthologyLink)()
        Dim lastChr As String = Nothing
        Dim p As Integer

        For p = 0 To links.Count - 1
            Dim currentChr As String = geneChr(queryIdx(p))

            If lastChr IsNot Nothing AndAlso currentChr <> lastChr Then
                ' 染色体切换，切割区块
                If subBlock.Count >= MinCollinearGenes Then
                    Yield MakeCollinearBlock(block, subBlock)
                End If

                subBlock.Clear()
            End If

            subBlock.Add(links(p))
            lastChr = currentChr
        Next

        ' 保存最后一个子区块
        If subBlock.Count >= MinCollinearGenes Then
            Yield MakeCollinearBlock(block, subBlock)
        End If
    End Function

    ''' <summary>
    ''' 生成共线性区块；在不保留逐基因配对数据的模式下只生成统计摘要
    ''' </summary>
    Private Function MakeCollinearBlock(source As CollinearBlock, links As List(Of OrthologyLink)) As CollinearBlock
        If RetainOrthologyLinks Then
            Return New CollinearBlock(source, links)
        Else
            Return New CollinearBlock(source, links.Count)
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
    ''' 基于泛基因组聚类结果和共线性分析结构变异（并行版）
    ''' </summary>
    ''' <remarks>
    ''' 这个函数要求在调用前需要完成共线性检测计算
    ''' </remarks>
    Private Function DetectStructuralVariations() As StructuralVariation()
        Dim F As Integer = familyIds.Length
        Dim N As Integer = totalGenomes

        If F = 0 Then
            Return New StructuralVariation() {}
        End If

        Dim perFamily(F - 1) As List(Of StructuralVariation)
        Dim cnvGain As Double = Me.CNV_Gain_Factor
        Dim cnvLoss As Double = Me.CNV_Loss_Factor

        ' =========================================
        ' 1. 基于 PAV 和 CNV 的检测（按基因家族并行）
        ' =========================================
        Parallel.For(0, F, Sub(k As Integer)
                               Dim members As Integer() = familyMembers(k)
                               Dim genes As String() = familyGeneIds(k)
                               Dim counts As Integer() = New Integer(N - 1) {}
                               Dim presence As Integer = 0

                               For Each gi As Integer In members
                                   counts(geneGenome(gi)) += 1
                               Next
                               For g As Integer = 0 To N - 1
                                   If counts(g) > 0 Then
                                       presence += 1
                                   End If
                               Next

                               ' 计算平均拷贝数（排除0）作为基准，或者以众数为基准
                               ' 这里简化逻辑：如果 >50% 的基因组有该基因，则认为它是“潜在核心”
                               Dim isCoreFamily As Boolean = (presence > N / 2)
                               Dim nonZeroCopies As New List(Of Integer)(presence)

                               For g As Integer = 0 To N - 1
                                   If counts(g) > 0 Then
                                       nonZeroCopies.Add(counts(g))
                                   End If
                               Next

                               Dim medianCopy As Double = If(nonZeroCopies.Count > 0, nonZeroCopies.Median, 0)
                               Dim local As List(Of StructuralVariation) = Nothing

                               ' lookup each genome for this gene family
                               For g As Integer = 0 To N - 1
                                   Dim gName As String = genomeList(g)
                                   Dim copyNum As Integer = counts(g)

                                   ' --- 情况 A: 缺失 ---
                                   If isCoreFamily AndAlso copyNum = 0 Then
                                       If local Is Nothing Then local = New List(Of StructuralVariation)()

                                       local.Add(New StructuralVariation With {
                                           .Type = SVType.PAV_Absence,
                                           .GenomeName = gName,
                                           .FamilyID = familyIds(k),
                                           .Description = $"Genome {gName} lacks gene family {familyIds(k)} which is present in most genomes.",
                                           .RelatedGenes = genes,
                                           .CopyNumber = copyNum,
                                           .Median = medianCopy
                                       })
                                   End If

                                   ' --- 情况 B: 特有/获得 ---
                                   If Not isCoreFamily AndAlso presence <= 2 AndAlso copyNum > 0 Then
                                       If local Is Nothing Then local = New List(Of StructuralVariation)()

                                       local.Add(New StructuralVariation With {
                                           .Type = SVType.PAV_Presence,
                                           .GenomeName = gName,
                                           .FamilyID = familyIds(k),
                                           .Description = $"Genome {gName} contains unique gene family {familyIds(k)}.",
                                           .RelatedGenes = GenesOfGenome(members, g),
                                           .CopyNumber = copyNum,
                                           .Median = medianCopy
                                       })
                                   End If

                                   ' --- 情况 C: 拷贝数变异 (CNV) ---
                                   If isCoreFamily AndAlso copyNum > 0 Then
                                       If copyNum >= medianCopy * 2 AndAlso copyNum > 1 Then
                                           If local Is Nothing Then local = New List(Of StructuralVariation)()

                                           local.Add(New StructuralVariation With {
                                               .Type = SVType.CNV_Gain,
                                               .GenomeName = gName,
                                               .FamilyID = familyIds(k),
                                               .Description = $"Copy number expansion in {gName} (Copy: {copyNum}, Median: {medianCopy}).",
                                               .RelatedGenes = GenesOfGenome(members, g),
                                               .CopyNumber = copyNum,
                                               .Median = medianCopy
                                           })
                                       ElseIf copyNum > 0 AndAlso medianCopy > 1 AndAlso copyNum <= medianCopy * cnvLoss Then
                                           If local Is Nothing Then local = New List(Of StructuralVariation)()

                                           local.Add(New StructuralVariation With {
                                               .Type = SVType.CNV_Loss,
                                               .GenomeName = gName,
                                               .FamilyID = familyIds(k),
                                               .Description = $"Copy number loss in {gName} (Copy: {copyNum}, Median: {medianCopy:F1}).",
                                               .CopyNumber = copyNum,
                                               .Median = medianCopy
                                           })
                                       End If
                                   End If
                               Next

                               perFamily(k) = local
                           End Sub)

        Dim all As New List(Of StructuralVariation)()

        ' 按照家族下标顺序合并，保证 SV_ID 的编号是可以复现的
        For k As Integer = 0 To F - 1
            If perFamily(k) IsNot Nothing Then
                all.AddRange(perFamily(k))
            End If
        Next
        For i As Integer = 0 To all.Count - 1
            all(i).SV_ID = "SV_" & (i + 1)
        Next

        ' =========================================
        ' 2. 基于共线性的 SV 检测 (简易版)
        ' =========================================
        If result.CollinearBlocks IsNot Nothing Then
            For Each block As CollinearBlock In result.CollinearBlocks
                If block.Chr1 <> block.Chr2 Then
                    ' 检测易位事件
                    all.Add(New StructuralVariation With {
                        .Type = SVType.Collinearity_Break,
                        .Description = $"Translocation: {block.Chr1} -> {block.Chr2}"
                    })
                End If
            Next
        End If

        Return all.ToArray()
    End Function

    ''' <summary>
    ''' 取出基因家族之内属于某一个基因组的基因ID
    ''' </summary>
    Private Function GenesOfGenome(members As Integer(), g As Integer) As String()
        Dim list As New List(Of String)()

        For Each gi As Integer In members
            If geneGenome(gi) = g Then
                list.Add(geneIds(gi))
            End If
        Next

        Return list.ToArray()
    End Function

    ''' <summary>
    ''' 辅助函数：生成唯一的基因组对Key（无论顺序）
    ''' </summary>
    ''' <param name="g1"></param>
    ''' <param name="g2"></param>
    ''' <returns></returns>
    Private Function OrderKey(g1 As String, g2 As String) As String
        ' 按字母顺序排序，确保 G1_vs_G2 和 G2_vs_G1 生成相同的 Key
        If String.Compare(g1, g2, StringComparison.Ordinal) <= 0 Then
            Return $"{g1}_vs_{g2}"
        Else
            Return $"{g2}_vs_{g1}"
        End If
    End Function

    ''' <summary>
    ''' 基于泛基因组基因家族计算基因组间的 Jaccard 遗传距离矩阵（位图 + 并行）
    ''' </summary>
    ''' <remarks>
    ''' 使用位图(bitmap)来表示每一个基因组的基因家族集合，
    ''' 两个基因组之间的交集大小可以通过位图的 AND + popcount 在 O(家族数/64) 之内得到，
    ''' 相比原来的 HashSet 遍历要快一个数量级，并且内存占用也很小。
    ''' </remarks>
    Private Sub CalculatePanGenomeJaccardDistance()
        Dim N As Integer = totalGenomes
        Dim F As Integer = familyIds.Length

        If N < 2 OrElse F = 0 Then
            result.GeneticDistanceMatrix = New Dictionary(Of String, Double)()
            Return
        End If

        Dim words As Integer = (F + 63) \ 64
        Dim bits As ULong()() = New ULong(N - 1)() {}
        Dim sizes As Integer() = New Integer(N - 1) {}

        ' 1. 将 "基因组 -> 基因集合" 转换为 "基因组 -> 基因家族位图"
        Parallel.For(0, N, Sub(g As Integer)
                               Dim b As ULong() = New ULong(words - 1) {}
                               Dim count As Integer = 0

                               For Each gi As Integer In genomeGenes(g)
                                   Dim famIdx As Integer = geneFamily(gi)

                                   If famIdx < 0 Then
                                       Continue For
                                   End If

                                   Dim w As Integer = famIdx \ 64
                                   Dim mask As ULong = 1UL << (famIdx Mod 64)

                                   If (b(w) And mask) = 0UL Then
                                       b(w) = b(w) Or mask
                                       count += 1
                                   End If
                               Next

                               bits(g) = b
                               sizes(g) = count
                           End Sub)

        ' 2. 两两计算 Jaccard 距离
        Dim npairs As Integer = N * (N - 1) \ 2
        Dim keys As String() = New String(npairs - 1) {}
        Dim values As Double() = New Double(npairs - 1) {}
        Dim offsets As Integer() = New Integer(N - 1) {}

        For i As Integer = 0 To N - 1
            offsets(i) = i * N - (i * (i + 1)) \ 2
        Next

        Parallel.For(0, N - 1, Sub(i As Integer)
                                   Dim off As Integer = offsets(i)
                                   Dim a As ULong() = bits(i)

                                   For j As Integer = i + 1 To N - 1
                                       Dim b As ULong() = bits(j)
                                       Dim intersectionCount As Integer = 0

                                       For w As Integer = 0 To words - 1
                                           intersectionCount += BitOperations.PopCount(a(w) And b(w))
                                       Next

                                       ' 计算并集大小: |A ∪ B| = |A| + |B| - |A ∩ B|
                                       Dim unionCount As Integer = sizes(i) + sizes(j) - intersectionCount
                                       Dim jaccardDistance As Double

                                       If unionCount > 0 Then
                                           jaccardDistance = 1.0 - (CDbl(intersectionCount) / CDbl(unionCount))
                                       Else
                                           jaccardDistance = 0.0
                                       End If

                                       keys(off) = OrderKey(genomeList(i), genomeList(j))
                                       values(off) = jaccardDistance
                                       off += 1
                                   Next
                               End Sub)

        ' 3. 按照确定的顺序写入结果矩阵
        result.GeneticDistanceMatrix = New Dictionary(Of String, Double)(npairs)

        For i As Integer = 0 To npairs - 1
            result.GeneticDistanceMatrix.Add(keys(i), values(i))
        Next
    End Sub

    ''' <summary>
    ''' 基于直系同源比对计算基因组间的遗传距离矩阵
    ''' </summary>
    Private Sub CalculateGeneticDistance(orthologDict As Dictionary(Of String, BiDirectionalBesthit()), genomeNames As List(Of String))
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

        result.GeneticDistanceMatrix = New Dictionary(Of String, Double)()

        ' 2. 遍历所有单拷贝家族
        For Each familyId As String In TqdmWrapper.Wrap(result.SingleCopyOrthologFamilies)
            Dim genes As String() = result.GeneFamilies(familyId)

            ' 单拷贝家族中只有 N 个基因 (N=基因组数)
            ' 我们需要找到这 N 个基因两两之间的 Ortholog 记录
            ' 实际上，单拷贝家族意味着两两之间必然有 RBH 关系
            For i As Integer = 0 To genes.Count - 1
                For j As Integer = i + 1 To genes.Count - 1
                    Dim g1 As String = genes(i)
                    Dim g2 As String = genes(j)

                    ' 查找它们之间的 Ortholog 记录
                    ' 因为是 RBH，g1 和 g2 必然在同一个 Ortholog 对象中
                    If geneToOrtholog.ContainsKey(g1) Then
                        Dim o = geneToOrtholog(g1)
                        Dim target = If(o.QueryName = g1, o.HitName, o.QueryName)

                        If target = g2 Then
                            Dim i1 As Integer = -1
                            Dim i2 As Integer = -1

                            If Not geneIndex.TryGetValue(g1, i1) OrElse Not geneIndex.TryGetValue(g2, i2) Then
                                Continue For
                            End If

                            Dim gName1 As String = genomeList(geneGenome(i1))
                            Dim gName2 As String = genomeList(geneGenome(i2))
                            Dim key As String = OrderKey(gName1, gName2)

                            ' 记录距离 (1 - Identity)
                            Dim dist As Double = 1.0 - ((o.forward + o.reverse) / 2.0)

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
        Dim singleCopyCount As Integer = result.SingleCopyOrthologFamilies.Length

        If singleCopyCount > 0 Then
            Dim keys As List(Of String) = result.GeneticDistanceMatrix.Keys.ToList()

            For Each k As String In keys
                ' 每对基因组在每个单拷贝家族中都会贡献一次距离
                result.GeneticDistanceMatrix(k) /= singleCopyCount
            Next
        End If
    End Sub
End Class
