Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.ContextModel
Imports SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult.WebBlast
Imports SMRUCC.genomics.SequenceModel

''' <summary>
''' 多个引物共同覆盖的候选区域信息类
''' </summary>
Public Class CandidateRegion

    Public Property Chr As String
    Public Property CoreStart As Integer  ' 引物覆盖的核心区域起点
    Public Property CoreEnd As Integer    ' 引物覆盖的核心区域终点
    Public Property Span As Integer       ' 核心区域跨度 (CoreEnd - CoreStart)
    Public Property SupportingHits As List(Of HitRecord) ' 支持该区域的hit集合

    ' 延伸后的区间
    Public Property ExtendedStart As Integer
    Public Property ExtendedEnd As Integer
    Public Property ExtensionLength As Integer ' 实际延伸长度(2Mb或5Mb)

    Public Property GenesInCoreRegion As Feature() ' 区间内的基因列表
    Public Property GenesInExtendedRegion As Feature() ' 延伸区间内的基因列表

    ''' <summary>
    ''' 需要将3个引物序列比对到新测序的六倍体基因组上，确定它们共同覆盖的目标物理区间，并上下游各延伸2Mb（若该区间基因密度较低，可适当放宽至5Mb，请最终明确延伸长度）。
    ''' 提取该区间内所有基因，包括六倍体基因ID、染色体位置、功能注释及基因序列。
    ''' </summary>
    ''' <param name="blastHits"></param>
    ''' <param name="maxCoreSpan"></param>
    ''' <param name="eval_cutoff"></param>
    ''' <returns></returns>
    Public Shared Function FindCandidateRegions(blastHits As IEnumerable(Of HitRecord),
                                                Optional maxCoreSpan As Integer = 3 * ISequenceModel.KB,
                                                Optional eval_cutoff As Double = 1,
                                                Optional primerIds As String() = Nothing) As CandidateRegion()

        Dim candidates As New List(Of CandidateRegion)
        Dim hitPool As HitRecord() = blastHits.ToArray

        If primerIds.IsNullOrEmpty Then
            primerIds = hitPool _
                .Select(Function(h) h.QueryID) _
                .Distinct _
                .ToArray
        End If

        For i As Integer = 0 To hitPool.Length - 1
            Dim site As Integer() = New Integer() {hitPool(i).SubjectStart, hitPool(i).SubjectEnd}
            Dim normLeft = site.Min
            Dim normRight = site.Max

            hitPool(i).SubjectStart = normLeft
            hitPool(i).SubjectEnd = normRight
        Next

        ' 1. 过滤低质量Hit (例如: Identity < 90 或 E-value > 1e-5)
        ' 2. 按染色体分组
        Dim chrGroups = From h As HitRecord
                        In blastHits
                        Where h.Identity >= 90 AndAlso h.EValue <= eval_cutoff
                        Group By h.SubjectIDs Into Group
                        Select New NamedCollection(Of HitRecord)(SubjectIDs, Group)

        For Each group As NamedCollection(Of HitRecord) In chrGroups
            Dim chrName = group.name
            Dim hitsOnChr = group.ToList()

            ' 1. 动态获取每个引物在该染色体上的Hit集合，存入一个列表中
            Dim primerHitGroups As New List(Of List(Of HitRecord))
            Dim allPrimerPresent As Boolean = True

            For Each PID As String In primerIds
                Dim hits = hitsOnChr.Where(Function(h) h.QueryID = PID).ToList()
                If hits.Count = 0 Then
                    ' 只要有任何一个引物在该染色体上没有hit，就标记为不全
                    allPrimerPresent = False
                    Exit For
                End If
                primerHitGroups.Add(hits)
            Next

            ' 如果这条染色体上没有所有引物同时出现，则跳过
            If Not allPrimerPresent Then Continue For

            ' 2. 动态生成笛卡尔积 (迭代法)
            ' combinations 初始包含一个空组合，作为迭代的基础
            Dim combinations As New List(Of List(Of HitRecord)) From {New List(Of HitRecord)}

            For Each hitGroup In primerHitGroups
                Dim newCombinations As New List(Of List(Of HitRecord))

                For Each existingComb In combinations
                    For Each hit In hitGroup
                        ' 复制当前已有的组合，并追加当前引物的一个hit
                        Dim newComb = existingComb.ToList()
                        newComb.Add(hit)
                        newCombinations.Add(newComb)
                    Next
                Next

                ' 用新生成的组合列表替换旧的，进入下一轮迭代
                combinations = newCombinations
            Next

            ' 3. 遍历所有组合，计算跨度并筛选
            For Each combination As List(Of HitRecord) In combinations
                ' combination 现在是一个包含了每个引物一个hit的集合 (数量 = primerIds.Length)

                ' 使用 LINQ 动态计算任意数量 hit 的最小起始和最大结束
                Dim minStart = combination.Min(Function(h) h.SubjectStart)
                Dim maxEnd = combination.Max(Function(h) h.SubjectEnd)
                Dim span = maxEnd - minStart

                ' 4. 判断是否满足聚类条件
                If span <= maxCoreSpan Then
                    Dim cand As New CandidateRegion With {
                        .Chr = chrName,
                        .CoreStart = minStart,
                        .CoreEnd = maxEnd,
                        .Span = span,
                        .SupportingHits = combination ' 直接将整个组合赋值给SupportingHits
                    }
                    candidates.Add(cand)
                End If
            Next
        Next

        ' 5. 候选区域去重与合并 (如果在同一条染色体上找到多个重叠的候选区，取外边界合并)
        candidates = candidates.OrderBy(Function(c) c.Chr).ThenBy(Function(c) c.CoreStart).ToList()
        Dim mergedCandidates As New List(Of CandidateRegion)

        For Each cand In candidates
            Dim last = If(mergedCandidates.Count > 0, mergedCandidates.Last(), Nothing)
            If last IsNot Nothing AndAlso last.Chr = cand.Chr AndAlso cand.CoreStart <= last.CoreEnd Then
                ' 合并重叠区
                last.CoreEnd = Math.Max(last.CoreEnd, cand.CoreEnd)
                last.Span = last.CoreEnd - last.CoreStart
                last.SupportingHits.AddRange(cand.SupportingHits)
            Else
                mergedCandidates.Add(cand)
            End If
        Next

        Return mergedCandidates
    End Function

    Public Shared Sub CalculateExtensions(candidates As IEnumerable(Of CandidateRegion), genomeCtx As Dictionary(Of String, GenomeContext(Of Feature)))
        For Each cand As CandidateRegion In candidates
            ' 默认先延伸 2Mb 进行试探
            Dim probeStart = Math.Max(1, cand.CoreStart - 2000000)
            Dim probeEnd = cand.CoreEnd + 2000000
            Dim chrContext = genomeCtx(cand.Chr)

            ' 利用你的 GenomeContext 获取该 4Mb+Core 区间内的基因数量
            Dim genesInProbe = chrContext.SelectByRange(probeStart, probeEnd).ToArray
            Dim geneCount = genesInProbe.Length

            ' 计算基因密度 (基因数/Mb)
            Dim regionLengthMb = (probeEnd - probeStart) / 1000000.0
            Dim density = geneCount / regionLengthMb

            ' 判断基因密度是否较低 (阈值需根据你的物种调整，例如小麦基因组密度大约5-10个/Mb，低于3算低)
            Dim isLowDensity As Boolean = (density < 3.0)

            If isLowDensity Then
                ' 低密度，放宽至 5Mb
                cand.ExtensionLength = 5000000
            Else
                ' 正常密度，保持 2Mb
                cand.ExtensionLength = 2000000
            End If

            ' 计算最终延伸区间 (注意不要越过染色体起点 1)
            cand.ExtendedStart = Math.Max(1, cand.CoreStart - cand.ExtensionLength)
            cand.ExtendedEnd = cand.CoreEnd + cand.ExtensionLength

            cand.GenesInCoreRegion = chrContext.SelectByRange(cand.CoreStart, cand.CoreEnd).ToArray()
            cand.GenesInExtendedRegion = chrContext.SelectByRange(cand.ExtendedStart, cand.ExtendedEnd).ToArray()
        Next
    End Sub
End Class