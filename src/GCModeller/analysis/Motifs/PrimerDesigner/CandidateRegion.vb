Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.ContextModel
Imports SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult.WebBlast

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

    Public Shared Function FindCandidateRegions(blastHits As List(Of HitRecord),
                                                primerIds As String(),
                                                maxCoreSpan As Integer,
                                                Optional eval_cutoff As Double = 1) As List(Of CandidateRegion)

        Dim candidates As New List(Of CandidateRegion)

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

            ' 分别获取3个引物在该染色体上的Hit集合
            Dim p1Hits = hitsOnChr.Where(Function(h) h.QueryID = primerIds(0)).ToList()
            Dim p2Hits = hitsOnChr.Where(Function(h) h.QueryID = primerIds(1)).ToList()
            Dim p3Hits = hitsOnChr.Where(Function(h) h.QueryID = primerIds(2)).ToList()

            ' 如果这条染色体上没有3个引物同时出现，则跳过 (视实验严谨度也可放宽为2个引物)
            If p1Hits.Count = 0 OrElse p2Hits.Count = 0 OrElse p3Hits.Count = 0 Then
                Continue For
            End If

            ' 3. 枚举该染色体上的所有组合 (笛卡尔积)
            For Each h1 In p1Hits
                For Each h2 In p2Hits
                    For Each h3 In p3Hits
                        ' 计算三个hit的最小起始和最大结束
                        Dim minStart = Math.Min(Math.Min(h1.SubjectStart, h2.SubjectStart), h3.SubjectStart)
                        Dim maxEnd = Math.Max(Math.Max(h1.SubjectEnd, h2.SubjectEnd), h3.SubjectEnd)
                        Dim span = maxEnd - minStart

                        ' 4. 判断是否满足聚类条件 (核心假设：3个引物不会相隔太远)
                        If span <= maxCoreSpan Then
                            Dim cand As New CandidateRegion With {
                                .Chr = chrName,
                                .CoreStart = minStart,
                                .CoreEnd = maxEnd,
                                .Span = span,
                                .SupportingHits = New List(Of HitRecord) From {h1, h2, h3}
                            }
                            candidates.Add(cand)
                        End If
                    Next
                Next
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

    Public Shared Sub CalculateExtensions(candidates As List(Of CandidateRegion), genomeCtx As GenomeContext(Of Feature))
        For Each cand In candidates
            ' 默认先延伸 2Mb 进行试探
            Dim probeStart = Math.Max(1, cand.CoreStart - 2000000)
            Dim probeEnd = cand.CoreEnd + 2000000

            ' 利用你的 GenomeContext 获取该 4Mb+Core 区间内的基因数量
            Dim genesInProbe = genomeCtx.SelectByRange(probeStart, probeEnd).ToArray
            Dim geneCount = genesInProbe.Count

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

        Next
    End Sub
End Class