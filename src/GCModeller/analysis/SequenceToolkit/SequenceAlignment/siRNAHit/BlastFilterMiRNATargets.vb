Imports System.Globalization
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Namespace siRNAHit

    ''' <summary>
    ''' 通过ncbi blastn进行靶基因搜索加速
    ''' </summary>
    ''' <remarks>
    ''' # ============ 第一轮：BLASTN 快速预筛 ============
    ''' blastn -task blastn-short \
    '''   -query miRNA.fa \
    '''   -subject plant_cDNA.fasta \
    '''   -evalue 10000 -word_size 7 \
    '''   -gapopen 4 -gapextend 2 \
    '''   -reward 1 -penalty -1 \
    '''   -outfmt "6 qseqid sseqid sstart send qstart qend sstrand qseq sseq length evalue bitscore" \
    '''   -num_threads 32 -max_target_seqs 5000 \
    '''   > round1_hits.tsv
    ''' </remarks>
    Public Module BlastFilterMiRNATargets

        ''' <summary>
        ''' ---------- 打分结果容器 ----------
        ''' </summary>
        Public Structure AlignmentScore
            Public Score As Double            ' 总罚分（越低越好）
            Public SeedMismatches As Integer  ' 种子区非 G:U 错配数
            Public TotalMismatches As Integer ' 全比对区非 G:U 错配数
            Public GuPairs As Integer         ' G:U wobble 配对数
            Public GapCount As Integer        ' 缺口（单侧凸起）列数
        End Structure

        ' 罚分常数（与 psRNATarget 保持一致，便于两条流程的结果直接比较/求交集）
        Private Const PEN_WOBBLE As Double = 0.5
        Private Const PEN_MISMATCH As Double = 1.0
        Private Const PEN_GAP As Double = 1.0
        Private Const GAP_OPEN As Double = 2.0
        Private Const GAP_EXTEND As Double = 0.5

        ''' <summary>
        ''' ---------- psRNATarget 打分函数 ----------
        ''' </summary>
        ''' <param name="qseq">
        ''' BLASTN HSP 的 qseq。minus 链命中时 query 恒为 plus 链，
        ''' 因此它就是 miRNA 的 5'->3' 片段。
        ''' </param>
        ''' <param name="sseq">
        ''' BLASTN HSP 的 sseq。minus 链命中时 BLAST 输出的是 mRNA 片段的**反向互补**，
        ''' 因此它与 <paramref name="qseq"/> 同向 5'->3'——匹配位点是**相同字母**。
        ''' </param>
        ''' <param name="seedStart">种子区起始（miRNA 5'->3' 1-based，psRNATarget V2 = 2）。</param>
        ''' <param name="seedEnd">种子区结束（miRNA 5'->3' 1-based，psRNATarget V2 = 13）。</param>
        ''' <param name="penaltyMultiplier">种子区错配的罚分倍数。</param>
        ''' <param name="qstart">
        ''' HSP 在 query（miRNA）上的起始坐标（BLAST 的 qstart，1-based）。
        ''' 用于将比对列号换算成 miRNA 真实坐标；局部比对只覆盖 miRNA 一部分时必须传入。
        ''' </param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 关键点：BLASTN 的 qseq/sseq 是**同向一致（identity）**框架而非互补框架。
        ''' 实测数据显示 minus 链 qseq 与 sseq 的逐位一致率约 90%，若按互补规则
        ''' （A-U / G-C）判定会把几乎每个位点都算成错配，直接导致过滤结果为空。
        ''' 配对判定统一委托给 <see cref="RNASeqHelper.ClassifyBlastPair"/>。
        ''' </remarks>
        Public Function ScoreAlignment(qseq As String,
                                       sseq As String,
                                       Optional seedStart As Integer = 2,
                                       Optional seedEnd As Integer = 13,
                                       Optional penaltyMultiplier As Double = 1.5,
                                       Optional qstart As Integer = 1) As AlignmentScore

            Dim result As New AlignmentScore With {
                .Score = 0.0, .SeedMismatches = 0,
                .TotalMismatches = 0, .GuPairs = 0, .GapCount = 0}

            If qseq Is Nothing OrElse sseq Is Nothing Then
                Return result
            End If

            Dim n As Integer = Math.Min(qseq.Length, sseq.Length)
            ' 比对列 i 对应的 miRNA 坐标：BLAST 的 query 恒为 plus 链，
            ' 故 = qstart + 已消耗的非 gap query 碱基数
            Dim mirnaPos As Integer = qstart
            Dim inGapRun As Boolean = False

            For i As Integer = 0 To n - 1
                Dim qBase As Char = qseq(i)
                Dim sBase As Char = sseq(i)
                Dim pair As RNASeqHelper.PairType = RNASeqHelper.ClassifyBlastPair(qBase, sBase)

                Select Case pair
                    Case RNASeqHelper.PairType.WC
                        ' 完美 Watson-Crick 配对，不罚分

                    Case RNASeqHelper.PairType.Wobble
                        ' G:U wobble：罚 0.5 分
                        result.GuPairs += 1
                        result.Score += PEN_WOBBLE

                    Case RNASeqHelper.PairType.Gap
                        ' 缺口单独统计，并按 open/extend 罚分（对齐 psRNATarget 口径）
                        result.GapCount += 1
                        result.Score += PEN_GAP

                        If inGapRun Then
                            result.Score += GAP_EXTEND
                        Else
                            result.Score += GAP_OPEN
                            inGapRun = True
                        End If

                    Case Else
                        ' 非 G:U 错配
                        result.TotalMismatches += 1

                        If mirnaPos >= seedStart AndAlso mirnaPos <= seedEnd Then
                            result.SeedMismatches += 1
                            result.Score += PEN_MISMATCH * penaltyMultiplier   ' 种子区错配 ×1.5
                        Else
                            result.Score += PEN_MISMATCH
                        End If
                End Select

                If pair <> RNASeqHelper.PairType.Gap Then
                    inGapRun = False
                End If
                ' target 侧的凸起不消耗 miRNA 序列
                If qBase <> "-"c Then
                    mirnaPos += 1
                End If
            Next

            Return result
        End Function

        ''' <summary>
        ''' 对 blastn 第一轮预筛的 HSP 集合做 psRNATarget 风格的打分过滤。
        ''' </summary>
        ''' <param name="hits">由 <see cref="BlastnMapTable.Parse"/> 解析出的 HSP 集合。</param>
        ''' <param name="evalueCutoff">
        ''' BLAST e-value 预筛阈值。注意它与 <paramref name="maxExpectation"/> 是**两种完全不同的量纲**：
        ''' 前者是 BLAST 的统计显著性（命令行 -evalue 用的就是它，故这里取同量级的 1000），
        ''' 后者是本函数按 psRNATarget 打分体系算出的期望分。二者不可混用。
        ''' </param>
        ''' <param name="maxExpectation">psRNATarget 期望分上限（越低越好）。</param>
        ''' <param name="minHitLength">最小 HSP 长度，避免极短随机命中（对齐 psRNATarget.MinHitLength）。</param>
        ''' <param name="seedStart">种子区起始（miRNA 坐标）。</param>
        ''' <param name="seedEnd">种子区结束（miRNA 坐标）。</param>
        ''' <param name="maxSeedMm">种子区内允许的最大非 G:U 错配数。</param>
        ''' <param name="maxTotalMm">全比对区允许的最大非 G:U 错配数。</param>
        ''' <param name="maxGu">允许的最大 G:U wobble 数。</param>
        ''' <param name="verbose">是否把每条命中打印到控制台（数据量大时请保持 False）。</param>
        <Extension>
        Public Iterator Function BlastnFilter(hits As IEnumerable(Of BlastnMapTable),
                                Optional evalueCutoff As Double = 1000,
                                Optional maxExpectation As Double = 5.0,
                                Optional minHitLength As Integer = 17,
                                Optional seedStart As Integer = 2,
                                Optional seedEnd As Integer = 13,
                                Optional maxSeedMm As Integer = 2,
                                Optional maxTotalMm As Integer = 8,
                                Optional maxGu As Integer = 7,
                                Optional verbose As Boolean = False) As IEnumerable(Of siRNAHit)

            If verbose Then
                Console.WriteLine(String.Join(vbTab,
                    "sRNA_id", "target_id", "target_start", "target_end",
                    "strand", "evalue", "score", "seed_mm", "total_mm",
                    "gu_pairs", "gaps", "qseq", "sseq"))
            End If

            For Each map As BlastnMapTable In hits.SafeQuery
                Dim hit As siRNAHit = map.ParseHit(
                    evalueCutoff:=evalueCutoff,
                    maxExpectation:=maxExpectation,
                    minHitLength:=minHitLength,
                    seedStart:=seedStart,
                    seedEnd:=seedEnd,
                    maxSeedMm:=maxSeedMm,
                    maxTotalMm:=maxTotalMm,
                    maxGu:=maxGu,
                    verbose:=verbose
                )

                If hit IsNot Nothing Then
                    Yield hit
                End If
            Next
        End Function

        ''' <summary>
        ''' 对单条 HSP 施加过滤与打分，不通过时返回 Nothing。
        ''' </summary>
        ''' <remarks>
        ''' 过滤顺序刻意把廉价条件（链方向 / e-value / 长度 / 种子区覆盖）放在
        ''' 逐字符打分之前：实测 44 万行 HSP 经廉价过滤后仅约 8 千行需要进入 O(n) 打分循环。
        ''' </remarks>
        <Extension>
        Private Function ParseHit(map As BlastnMapTable,
                                  evalueCutoff As Double,
                                  maxExpectation As Double,
                                  minHitLength As Integer,
                                  seedStart As Integer,
                                  seedEnd As Integer,
                                  maxSeedMm As Integer,
                                  maxTotalMm As Integer,
                                  maxGu As Integer,
                                  verbose As Boolean) As siRNAHit

            ' 1) 只有 minus 链命中才可能是靶位点：
            '    靶位点 = revcomp(miRNA) 出现在 mRNA 正义链上，对应 BLASTN 的 minus 链 HSP。
            '    plus 链命中表示 mRNA 含有与 miRNA 同向的序列，无法反向互补结合。
            If Not map.IsMinus Then
                Return Nothing
            End If

            ' 2) BLAST e-value 预筛
            If map.evalue > evalueCutoff Then
                Return Nothing
            End If

            ' 3) 最小比对长度，过滤掉 word_size=7 产生的极短随机命中
            If map.length < minHitLength Then
                Return Nothing
            End If

            ' 4) HSP 必须完整覆盖 miRNA 的种子区，否则种子区错配恒为 0 会造成大量假阳性
            If map.qstart > seedStart OrElse map.qend < seedEnd Then
                Return Nothing
            End If

            Dim scored As AlignmentScore = ScoreAlignment(
                map.qseq, map.sseq,
                seedStart:=seedStart,
                seedEnd:=seedEnd,
                qstart:=map.qstart)

            ' 5) psRNATarget 过滤条件
            If scored.Score > maxExpectation OrElse
               scored.SeedMismatches > maxSeedMm OrElse
               scored.TotalMismatches > maxTotalMm OrElse
               scored.GuPairs > maxGu Then

                Return Nothing
            End If

            If verbose Then
                ' score 统一保留 1 位小数（psRNATarget 分数均为 0.5 的整数倍）
                Console.WriteLine(String.Join(vbTab,
                    map.qseqid, map.sseqid, map.SiteStart, map.SiteEnd, map.sstrand,
                    map.evalue,
                    scored.Score.ToString("F1", CultureInfo.InvariantCulture),
                    scored.SeedMismatches.ToString(),
                    scored.TotalMismatches.ToString(),
                    scored.GuPairs.ToString(),
                    scored.GapCount.ToString(),
                    map.qseq, map.sseq))
            End If

            Return New siRNAHit With {
                .WobbleCount = scored.GuPairs,
                .MismatchCount = scored.TotalMismatches,
                .GapCount = scored.GapCount,
                .StartSite = map.SiteStart,
                .EndSite = map.SiteEnd,
                .Length = map.SiteLength,
                .miRNA = map.qseqid,
                .Target = map.sseqid,
                .Source = "NCBI Blastn",
                .Expectation = scored.Score,
                .Alignment = map.qseq & vbCrLf & map.sseq,
                .TranslationInhibition = HasCenterMismatch(map)
            }
        End Function

        ''' <summary>
        ''' 切割位点（miRNA 第 10–11 位）存在严格错配 → 翻译抑制候选。
        ''' </summary>
        ''' <remarks>
        ''' 判定必须用 miRNA 坐标而非比对列号：局部比对的 qstart 通常大于 1。
        ''' </remarks>
        Private Function HasCenterMismatch(map As BlastnMapTable,
                                           Optional cleavageStart As Integer = 10,
                                           Optional cleavageEnd As Integer = 11) As Boolean

            Dim qseq As String = map.qseq
            Dim sseq As String = map.sseq

            If qseq Is Nothing OrElse sseq Is Nothing Then
                Return False
            End If

            Dim n As Integer = Math.Min(qseq.Length, sseq.Length)
            Dim mirnaPos As Integer = map.qstart

            For i As Integer = 0 To n - 1
                If qseq(i) <> "-"c Then
                    If mirnaPos >= cleavageStart AndAlso mirnaPos <= cleavageEnd Then
                        If RNASeqHelper.ClassifyBlastPair(qseq(i), sseq(i)) = RNASeqHelper.PairType.Mismatch Then
                            Return True
                        End If
                    End If

                    mirnaPos += 1
                End If
            Next

            Return False
        End Function
    End Module
End Namespace