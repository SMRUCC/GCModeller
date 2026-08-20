' Linclust 蛋白序列无监督聚类 - 主入口
'
' 五阶段流程:
'   阶段一:缩减字母表编码 + 每序列取哈希最小 m 个 k-mer,构造 mN 行 16 字节表
'   阶段二:按 k-mer 索引排序分桶,每组选最长序列为中心,合并共享中心的组
'   阶段三:级联快速过滤(汉明距离 + 无缺口局部比对)淘汰假阳性
'   阶段四:Smith-Waterman 带缺口比对,通过判据者连成员 -> 中心有向边
'   阶段五:贪心集合覆盖(按长度降序),输出簇与代表序列

Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.SequenceAlignment.BestLocalAlignment
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Linclust

    Public Module Linclust

        ''' <summary>
        ''' 自动选择 k-mer 长度。
        ''' k_spec >= floor(log(NL) / log(A_eff)),并取 max(k_spec, k_seqid)。
        ''' seqid >= 0.9 时 k_seqid = 14,否则 10。
        ''' </summary>
        Public Function SelectK(nSeq As Integer, avgLen As Double, opts As LinclustOptions) As Integer
            If nSeq <= 0 OrElse avgLen <= 0 Then
                Return If(opts.seqidThreshold >= 0.9, 14, 10)
            End If

            Dim kSpec = Math.Floor(Math.Log(CDbl(nSeq) * avgLen) / Math.Log(opts.Aeff))
            Dim kSeqid = If(opts.seqidThreshold >= 0.9, 14, 10)

            Return CInt(Math.Max(kSpec, kSeqid))
        End Function

        ''' <summary>
        ''' 对一组蛋白序列执行 Linclust 无监督聚类。
        ''' </summary>
        ''' <param name="seqs">输入蛋白序列(FASTA 对象)</param>
        ''' <param name="opts">聚类配置</param>
        ''' <returns>簇划分与每簇代表序列</returns>
        Public Function Cluster(seqs As IEnumerable(Of FastaSeq), Optional opts As LinclustOptions = Nothing) As ClusterResult
            If opts Is Nothing Then
                opts = New LinclustOptions
            End If

            Dim list = seqs.SafeQuery.ToArray

            If list.Length = 0 Then
                Return New ClusterResult With {.clusters = New List(Of Cluster), .k = 0, .nSeq = 0}
            End If

            ' ---------- 准备:编码与长度 ----------
            Dim rawSeqs(list.Length - 1) As String
            Dim encoded(list.Length - 1) As String
            Dim seqLengths As New Dictionary(Of Integer, Integer)
            Dim avgLen As Double = 0

            For i As Integer = 0 To list.Length - 1
                Dim s = list(i).SequenceData
                rawSeqs(i) = If(s, "")
                encoded(i) = ReducedAlphabet.Encode(rawSeqs(i))
                seqLengths(i) = rawSeqs(i).Length
                avgLen += rawSeqs(i).Length
            Next
            avgLen /= list.Length

            ' ---------- 阶段一 & 二 ----------
            Dim k = SelectK(list.Length, avgLen, opts)
            Dim rows = KmerTable.Build(encoded, k, opts.m)
            Dim centers = KmerTable.SelectCenters(rows)
            Dim byCenter = KmerTable.MergeByCenter(rows, centers)

            ' 记录 k-mer 位置,便于阶段三从锚点延伸
            Dim kmerPos = BuildKmerPosition(rows)

            ' ---------- 阶段三 & 四:对每个中心组做级联过滤 + SW ----------
            Dim edges As New List(Of (From As Integer, [To] As Integer))

            For Each centerPair In byCenter
                Dim centerId = centerPair.Key
                Dim centerRaw = rawSeqs(centerId)
                Dim centerEnc = encoded(centerId)

                For Each memberId In centerPair.Value
                    If memberId = centerId Then
                        Continue For
                    End If

                    Dim memberRaw = rawSeqs(memberId)
                    Dim memberEnc = encoded(memberId)

                    ' 阶段三:快速过滤(从 k-mer 位置无缺口延伸)
                    ' 作用:廉价淘汰明显无关的 (member, center) 对,避免进入昂贵的阶段四 SW。
                    ' 注意:单 k-mer 锚点的无缺口延伸在有突变处会被截断,不足以代表整体
                    ' 覆盖率,因此阶段三仅做"锚点有效性"粗筛(延伸匹配长度 >= k 即视为
                    ' 可能同源),严格的覆盖率/一致性/E-value 判据统一交给阶段四 SW。
                    Dim pos = kmerPos((memberId, centerId))
                    Dim fast = CascadeFilter.Filter(
                        memberEnc, centerEnc,
                        pos.memberPos, pos.centerPos, k,
                        opts.fastFilterCoverage, opts.fastFilterSeqid)

                    If fast.MatchLength < k Then
                        ' 锚点处无有效无缺口匹配,直接淘汰
                        Continue For
                    End If

                    ' 阶段四:Smith-Waterman 带缺口比对(对原始序列)
                    Dim sw = SmithWaterman _
                        .Align(DirectCast(list(memberId), IPolymerSequenceModel),
                               DirectCast(list(centerId), IPolymerSequenceModel))
                    Dim output = sw.GetOutput(opts.seqidThreshold * 100, k)
                    Dim hsp = GetBestHSP(output)

                    If hsp Is Nothing Then
                        Continue For
                    End If

                    ' 从对齐字符串计算一致性与覆盖率
                    Dim identity = AlignmentIdentity(hsp.Query, hsp.Subject)
                    Dim coverage = CDbl(Math.Min(hsp.LengthQuery, hsp.LengthHit)) / Math.Min(memberRaw.Length, centerRaw.Length)

                    ' 阶段四:E-value 显著性判据(Karlin-Altschul)
                    Dim eval = EValue.Compute(hsp.score, memberRaw.Length, centerRaw.Length)

                    If identity >= opts.seqidThreshold AndAlso
                       coverage >= opts.coverage AndAlso
                       eval <= opts.evalue Then
                        ' 成员 -> 中心 有向边(一致性 + 覆盖率 + E-value 三者均满足)
                        edges.Add((memberId, centerId))
                    End If
                Next
            Next

            ' ---------- 阶段五:贪心集合覆盖 ----------
            Dim clusters = GreedyCover.Cluster(edges, seqLengths)

            Return New ClusterResult With {
                .clusters = clusters,
                .k = k,
                .nSeq = list.Length
            }
        End Function

        ' 构建 (memberId, centerId) -> (memberPos, centerPos) 的位置查表
        Private Function BuildKmerPosition(rows As KmerEntry()) As Dictionary(Of (Integer, Integer), (memberPos As Integer, centerPos As Integer))
            Dim dict As New Dictionary(Of (Integer, Integer), (Integer, Integer))
            Dim i As Integer = 0

            While i < rows.Length
                Dim kmer = rows(i).KmerIndex
                Dim centerId = -1

                ' 找出该 k-mer 组的中心(最长)
                Dim maxLen = -1
                Dim j = i
                While j < rows.Length AndAlso rows(j).KmerIndex = kmer
                    If rows(j).SeqLen > maxLen Then
                        maxLen = rows(j).SeqLen
                        centerId = rows(j).SeqId
                    End If
                    j += 1
                End While

                j = i
                While j < rows.Length AndAlso rows(j).KmerIndex = kmer
                    Dim memberId = rows(j).SeqId
                    If memberId <> centerId Then
                        dict((memberId, centerId)) = (rows(j).Position, GetCenterPos(rows, kmer, centerId))
                    End If
                    j += 1
                End While

                i = j
            End While

            Return dict
        End Function

        Private Function GetCenterPos(rows As KmerEntry(), kmer As Long, centerId As Integer) As Integer
            For Each r In rows
                If r.KmerIndex = kmer AndAlso r.SeqId = centerId Then
                    Return r.Position
                End If
            Next
            Return 0
        End Function

        Private Function GetBestHSP(output As Output) As HSP
            If output?.HSP Is Nothing OrElse output.HSP.Length = 0 Then
                Return Nothing
            End If

            ' 取 score 最高的 HSP
            Dim best = output.HSP(0)
            For Each h In output.HSP
                If h.score > best.score Then
                    best = h
                End If
            Next
            Return best
        End Function

        ''' <summary>
        ''' 基于对齐字符串(含 '-')计算一致性(匹配数 / 对齐列数,不含两端全空列已隐含)
        ''' </summary>
        Private Function AlignmentIdentity(query As String, subject As String) As Double
            If String.IsNullOrEmpty(query) OrElse String.IsNullOrEmpty(subject) Then
                Return 0
            End If

            Dim n = Math.Min(query.Length, subject.Length)
            Dim match = 0

            For i As Integer = 0 To n - 1
                If query(i) = subject(i) AndAlso query(i) <> "-"c Then
                    match += 1
                End If
            Next

            Return CDbl(match) / n
        End Function
    End Module
End Namespace
