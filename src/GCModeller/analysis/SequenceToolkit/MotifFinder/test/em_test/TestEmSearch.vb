' ============================================================================
' TestEmSearch.vb — 搜索编排层的集成测试
' ----------------------------------------------------------------------------
' 覆盖 [em.md §5] 初始化策略、[em.md §4] 收敛、[em.md §6] 三种位点分布模型的
' 实际发现能力、[em.md §7] 多 motif 屏蔽重跑、[em.md §9] 双链与宽度范围，
' 以及确定性与边界输入。
'
' 这类用例同时是「缺陷 #1/#2/#3 是否修好」的最强证据：
' 修复前一致序列退化为全 A、位点定位接近随机，修复后应恢复到 ≥9/10 与 ≥80%。
' ============================================================================

Option Strict On

Imports System.Text.Json
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif.EmMotif.Core
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif.EmMotif.Model
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace EmMotif

    ''' <summary>位点恢复质量的评分</summary>
    Public Class RecoveryScore

        Public Property Total As Integer
        Public Property PosOk As Integer
        Public Property StrandOk As Integer

        Public ReadOnly Property PosRate As Double
            Get
                Return If(Total = 0, 0.0, CDbl(PosOk) / Total)
            End Get
        End Property

        Public ReadOnly Property StrandRate As Double
            Get
                Return If(Total = 0, 0.0, CDbl(StrandOk) / Total)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{PosOk}/{Total}（{PosRate * 100:F1}%）"
        End Function

    End Class

    Public Module TestEmSearch

        ' ====================================================================
        ' 辅助
        ' ====================================================================

        ''' <summary>按序列分组取后验最大的预测位点，与真值比对</summary>
        Public Function ScoreSites(result As EmMotifResult, truth As PlantResult,
                                   posTol As Integer) As RecoveryScore
            Dim sc As New RecoveryScore()

            Dim bySeq As New Dictionary(Of Integer, List(Of SiteTruth))()
            For Each t In truth.Sites
                If Not bySeq.ContainsKey(t.SeqIndex) Then bySeq(t.SeqIndex) = New List(Of SiteTruth)()
                bySeq(t.SeqIndex).Add(t)
            Next

            For Each kv In bySeq
                Dim bestZ As Double = -1
                Dim bestPos As Integer = -1
                Dim bestMinus As Boolean = False

                For i = 0 To result.Sites.Count - 1
                    If result.SiteSeqIndex(i) <> kv.Key Then Continue For
                    If result.Sites(i).Z > bestZ Then
                        bestZ = result.Sites(i).Z
                        bestPos = result.Sites(i).Pos
                        bestMinus = result.Sites(i).StrandMinus
                    End If
                Next

                sc.Total += 1
                If bestPos < 0 Then Continue For

                For Each t In kv.Value
                    If Math.Abs(bestPos - t.Pos) <= posTol Then
                        sc.PosOk += 1
                        If bestMinus = t.StrandMinus Then sc.StrandOk += 1
                        Exit For
                    End If
                Next
            Next

            Return sc
        End Function

        Private Function MakeOptions(model As SiteModel, w As Integer,
                                     Optional nmotifs As Integer = 1,
                                     Optional revcomp As Boolean = False,
                                     Optional strategy As String = "enriched",
                                     Optional seedCount As Integer = 8,
                                     Optional maxIter As Integer = 100,
                                     Optional rngSeed As Integer = 7) As SearchOptions
            Return New SearchOptions With {
                .Model = model, .MinW = w, .MaxW = w, .NumMotifs = nmotifs,
                .Revcomp = revcomp, .SeedStrategy = strategy, .SeedCount = seedCount,
                .MaxSeeds = 200, .Pseudocount = 0.1, .MaxIter = maxIter,
                .Epsilon = 0.0001, .EvalueMax = 10.0, .RngSeed = rngSeed}
        End Function

        Private Function Discover(seqs As List(Of String), alpha As Alphabet,
                                  opts As SearchOptions) As List(Of EmMotifResult)
            Dim encs = TestData.EncodeAll(seqs, alpha)
            Dim search As New EmSearch(encs, alpha, opts)
            Return search.Discover()
        End Function

        ' ====================================================================
        ' 用例组
        ' ====================================================================

        ''' <summary>DNA + ZOOPS 种植恢复 [em.md §4/§6]</summary>
        Public Sub TestDnaRecovery()
            TestAssert.Section("DNA ZOOPS 种植恢复 [em.md §4]")

            Dim alpha As New Alphabet(SeqTypes.DNA)
            Dim motif = "ACGTTACGTA"
            Dim planted = TestData.PlantDna(30, 200, motif, 20240903)
            Dim opts = MakeOptions(SiteModel.Zoops, 10)
            Dim results = Discover(planted.Sequences, alpha, opts)

            TestAssert.Check(results.Count = 1, $"发现 1 个 motif（实际 {results.Count}）")
            If results.Count = 0 Then Return

            Dim r = results(0)
            Dim match = TestData.BestShiftedMatch(r.Consensus, motif)
            Dim score = ScoreSites(r, planted, 2)

            TestAssert.Note($"共识 = {r.Consensus}（匹配 {match}/10）λ={r.Lambda:F3} " &
                            $"LLR={r.LogLikelihoodRatio:F1} E={r.Evalue:G3} 迭代 {r.Iterations} 轮")
            TestAssert.Note($"位点定位（±2bp）：{score}")

            TestAssert.Check(match >= 9, $"共识恢复 ≥9/10（实际 {match}/10）[缺陷 #3]")
            TestAssert.Check(score.PosRate >= 0.8, $"位点定位 ≥80% 在 ±2bp（实际 {score}）[缺陷 #2]")
            TestAssert.Check(r.Lambda > 0.7, $"ZOOPS λ 收敛到位点密度（λ={r.Lambda:F2}）[em.md §3]")
            TestAssert.Check(r.Converged, $"在 {r.Iterations} 轮内收敛 [em.md §4]")
            TestAssert.Check(r.LogLikTrace.Count >= 2, "LL 轨迹非空")
        End Sub

        ''' <summary>DNA + OOPS（每条序列恰 1 个位点）[em.md §6]</summary>
        Public Sub TestOopsRecovery()
            TestAssert.Section("DNA OOPS 种植恢复（每条序列恰 1 个位点）[em.md §6]")

            Dim alpha As New Alphabet(SeqTypes.DNA)
            Dim motif = "GATTACAGGT"
            ' withSiteRatio = 1：OOPS 假设每条序列都恰有一个位点
            Dim planted = TestData.PlantDna(24, 160, motif, 606, withSiteRatio:=1.0)
            Dim opts = MakeOptions(SiteModel.Oops, 10)
            Dim results = Discover(planted.Sequences, alpha, opts)

            TestAssert.Check(results.Count = 1, $"发现 1 个 motif（实际 {results.Count}）")
            If results.Count = 0 Then Return

            Dim r = results(0)
            Dim match = TestData.BestShiftedMatch(r.Consensus, motif)
            Dim score = ScoreSites(r, planted, 2)
            TestAssert.Note($"共识 = {r.Consensus}（匹配 {match}/10）定位 {score}")

            TestAssert.Check(match >= 8, $"OOPS 共识恢复 ≥8/10（实际 {match}/10）")
            TestAssert.Check(score.PosRate >= 0.7, $"OOPS 位点定位 ≥70% 在 ±2bp（实际 {score}）")

            ' OOPS 的 Σ_j Z_ij = 1：每条序列的后验之和（近似）应为 1
            Dim sums As New List(Of Double)()
            For si = 0 To planted.Sequences.Count - 1
                Dim s As Double = 0
                For i = 0 To r.Sites.Count - 1
                    If r.SiteSeqIndex(i) = si Then s += r.Sites(i).Z
                Next
                sums.Add(s)
            Next
            Dim worst As Double = 0
            For Each s In sums
                worst = Math.Max(worst, Math.Abs(s - 1.0))
            Next
            TestAssert.Check(worst < 0.000001, $"OOPS 每条序列 Σ_j Z_ij = 1（最大偏差 {worst:G3}）[em.md §6]")
        End Sub

        ''' <summary>蛋白序列恢复 [em.md §1]</summary>
        Public Sub TestProteinRecovery()
            TestAssert.Section("蛋白序列种植恢复 [em.md §1]")

            Dim alpha As New Alphabet(SeqTypes.Protein)
            Dim motif = "GASTLSKL"
            Dim planted = TestData.PlantProtein(25, 120, motif, 424242)
            Dim opts = MakeOptions(SiteModel.Zoops, 8)
            Dim results = Discover(planted.Sequences, alpha, opts)

            TestAssert.Check(results.Count = 1, $"发现 1 个 motif（实际 {results.Count}）")
            If results.Count = 0 Then Return

            Dim r = results(0)
            Dim match = TestData.BestShiftedMatch(r.Consensus, motif)
            Dim score = ScoreSites(r, planted, 1)
            TestAssert.Note($"共识 = {r.Consensus}（匹配 {match}/8）定位 {score}")

            TestAssert.Check(match >= 7, $"蛋白共识恢复 ≥7/8（实际 {match}/8）[缺陷 #3]")
            TestAssert.Check(score.PosRate >= 0.8, $"蛋白位点定位 ≥80%（实际 {score}）[缺陷 #2]")
        End Sub

        ''' <summary>双链扫描：链向与位置 [em.md §9]</summary>
        Public Sub TestRevcompRecovery()
            TestAssert.Section("双链扫描恢复（含链向判读）[em.md §9]")

            Dim alpha As New Alphabet(SeqTypes.DNA)
            Dim motif = "ACGTCGTA"
            ' 一半序列把 motif 植入负链（写入其反向互补）
            Dim planted = TestData.PlantDna(24, 150, motif, 999, revcompFraction:=0.5)
            Dim minusCount = planted.Sites.Where(Function(s) s.StrandMinus).Count()

            Dim opts = MakeOptions(SiteModel.Zoops, 8, revcomp:=True)
            Dim results = Discover(planted.Sequences, alpha, opts)

            TestAssert.Check(results.Count = 1, $"发现 1 个 motif（实际 {results.Count}）")
            TestAssert.Check(minusCount > 0, $"测试数据确实包含负链位点（{minusCount}/{planted.Sites.Count}）")
            If results.Count = 0 Then Return

            Dim r = results(0)
            Dim match = TestData.BestShiftedMatch(r.Consensus, motif)
            Dim score = ScoreSites(r, planted, 2)
            TestAssert.Note($"共识 = {r.Consensus}（匹配 {match}/8）定位 {score} 链向正确 {score.StrandOk}/{score.Total}")

            TestAssert.Check(match >= 7, $"双链共识恢复 ≥7/8（实际 {match}/8）")
            TestAssert.Check(score.PosRate >= 0.8, $"双链位点定位 ≥80%（实际 {score}）")
            TestAssert.Check(score.StrandRate >= 0.8,
                             $"链向判读 ≥80% 正确（实际 {score.StrandOk}/{score.Total}）[缺陷 #9]")

            ' 负链位点确实被检出
            Dim minusSites = 0
            For Each sp In r.Sites
                If sp.StrandMinus AndAlso sp.Z > 0.5 Then minusSites += 1
            Next
            TestAssert.Check(minusSites > 0, $"检出负链强位点（{minusSites} 个）[em.md §9]")
        End Sub

        ''' <summary>ANR：每条序列多个位点 [em.md §6]</summary>
        Public Sub TestAnrMultipleSites()
            TestAssert.Section("ANR 多位点发现（每条序列 2~3 个位点）[em.md §6]")

            Dim alpha As New Alphabet(SeqTypes.DNA)
            Dim motif = "TTGACAAT"
            Dim planted = TestData.PlantDna(20, 260, motif, 13579,
                                            mutationRate:=0.08, withSiteRatio:=1.0,
                                            sitesPerSequence:=3)
            Dim opts = MakeOptions(SiteModel.Anr, 8)
            Dim results = Discover(planted.Sequences, alpha, opts)

            TestAssert.Check(results.Count = 1, $"发现 1 个 motif（实际 {results.Count}）")
            If results.Count = 0 Then Return

            Dim r = results(0)
            Dim match = TestData.BestShiftedMatch(r.Consensus, motif)
            Dim strong = 0
            For Each sp In r.Sites
                If sp.Z > 0.5 Then strong += 1
            Next
            TestAssert.Note($"共识 = {r.Consensus}（匹配 {match}/8）强位点 Z>0.5：{strong}（植入 {planted.Sites.Count}）λ={r.Lambda:F4}")

            TestAssert.Check(match >= 7, $"ANR 共识恢复 ≥7/8（实际 {match}/8）[缺陷 #3]")
            TestAssert.Check(strong >= planted.Sites.Count * 0.5,
                             $"ANR 检出过半强位点（{strong}/{planted.Sites.Count}）[缺陷 #2]")
        End Sub

        ''' <summary>多 motif：屏蔽 + 重跑 [em.md §7]</summary>
        Public Sub TestMultiMotif()
            TestAssert.Section("多 motif 屏蔽重跑 [em.md §7]")

            Dim alpha As New Alphabet(SeqTypes.DNA)
            Dim m1 = "ACGTTACGTA"
            Dim m2 = "TTGGCCAGGA"
            Dim rng = TestData.MakeRng(2468)
            Dim seqs As New List(Of String)()

            For i = 0 To 29
                Dim ch(219) As Char
                For t = 0 To 219
                    ch(t) = TestData.DnaLetters(rng.Next(4))
                Next
                ' 两个 motif 可能重叠，这是 EM 的难点之一；此处接受偶然重叠
                Array.Copy(m1.ToCharArray(), 0, ch, rng.Next(0, 220 - m1.Length), m1.Length)
                Array.Copy(m2.ToCharArray(), 0, ch, rng.Next(0, 220 - m2.Length), m2.Length)
                seqs.Add(New String(ch))
            Next

            Dim opts = MakeOptions(SiteModel.Zoops, 10, nmotifs:=2)
            Dim results = Discover(seqs, alpha, opts)

            TestAssert.Check(results.Count = 2, $"发现 2 个 motif（实际 {results.Count}）[em.md §7]")
            If results.Count < 2 Then Return

            Dim c1 = results(0).Consensus
            Dim c2 = results(1).Consensus
            TestAssert.Note($"motif_1 = {c1}   motif_2 = {c2}")

            Dim s1 = TestData.BestShiftedMatchAny(c1, m1, m2)
            Dim s2 = TestData.BestShiftedMatchAny(c2, m1, m2)
            TestAssert.Check(s1 >= 8, $"motif_1 命中某个植入 motif（≥8/10，实际 {s1}）")
            TestAssert.Check(s2 >= 8, $"motif_2 命中某个植入 motif（≥8/10，实际 {s2}）")
            TestAssert.Check(String.CompareOrdinal(c1, c2) <> 0, "两个 motif 互不相同（屏蔽生效）[em.md §7]")
        End Sub

        ''' <summary>三种种子策略均可用 [em.md §5]</summary>
        Public Sub TestSeedStrategies()
            TestAssert.Section("种子初始化策略：enriched / random / all [em.md §5]")

            Dim alpha As New Alphabet(SeqTypes.DNA)
            Dim motif = "CAGGTAGCA"
            Dim planted = TestData.PlantDna(20, 160, motif, 777)

            For Each strategy In New String() {"enriched", "random"}
                Dim opts = MakeOptions(SiteModel.Zoops, 9, strategy:=strategy, seedCount:=8, rngSeed:=11)
                Dim results = Discover(planted.Sequences, alpha, opts)
                TestAssert.Check(results.Count = 1, $"策略 {strategy} 能产出结果")
                If results.Count > 0 Then
                    Dim m = TestData.BestShiftedMatch(results(0).Consensus, motif)
                    TestAssert.Check(m >= 7, $"策略 {strategy} 共识恢复 ≥7/9（实际 {m}/9，共识 {results(0).Consensus}）")
                End If
            Next

            ' all 策略：小数据集 + 限流，避免组合爆炸
            Dim small = TestData.PlantDna(6, 60, motif, 888)
            Dim optsAll = MakeOptions(SiteModel.Zoops, 6, strategy:="all", seedCount:=20, rngSeed:=3, maxIter:=40)
            optsAll.MaxSeeds = 20
            Dim rAll = Discover(small.Sequences, alpha, optsAll)
            TestAssert.Check(rAll.Count >= 1, "策略 all 能产出结果（MaxSeeds 限流生效）")
        End Sub

        ''' <summary>同种子 → 同结果（EM 的确定性）[em.md §8]</summary>
        Public Sub TestDeterminism()
            TestAssert.Section("结果可复现（相同 rng-seed → 相同结果）[em.md §8]")

            Dim alpha As New Alphabet(SeqTypes.DNA)
            Dim motif = "ACGTTACGTA"
            Dim planted = TestData.PlantDna(16, 160, motif, 31415)

            Dim opts1 = MakeOptions(SiteModel.Zoops, 10, strategy:="random", seedCount:=6, rngSeed:=2024)
            Dim opts2 = MakeOptions(SiteModel.Zoops, 10, strategy:="random", seedCount:=6, rngSeed:=2024)

            Dim a = Discover(planted.Sequences, alpha, opts1)
            Dim b = Discover(planted.Sequences, alpha, opts2)

            TestAssert.Check(a.Count = 1 AndAlso b.Count = 1, "两次运行都产出结果")
            If a.Count = 0 OrElse b.Count = 0 Then Return

            TestAssert.CheckEqual(a(0).Consensus, b(0).Consensus, "两次运行一致序列相同")
            TestAssert.CheckNear(a(0).LogLikelihood, b(0).LogLikelihood, 0.000000001, "两次运行对数似然相同")
            TestAssert.CheckNear(a(0).Lambda, b(0).Lambda, 0.000000001, "两次运行 λ 相同")
        End Sub

        ''' <summary>[缺陷 #10] 宽度范围择优不应系统性偏向 maxw [em.md §9]</summary>
        Public Sub TestWidthSelection()
            TestAssert.Section("宽度范围择优 [-−minw/-−maxw] [em.md §9]")

            Dim alpha As New Alphabet(SeqTypes.DNA)
            Dim motif = "ACGTCGTA"      ' 真实宽度 8
            Dim planted = TestData.PlantDna(20, 170, motif, 24680)

            Dim opts = MakeOptions(SiteModel.Zoops, 8, seedCount:=6, maxIter:=80)
            opts.MinW = 6
            opts.MaxW = 14

            Dim encs = TestData.EncodeAll(planted.Sequences, alpha)
            Dim search As New EmSearch(encs, alpha, opts)
            Dim results = search.Discover()

            TestAssert.Check(results.Count = 1, $"发现 1 个 motif（实际 {results.Count}）")
            If results.Count = 0 Then Return

            Dim r = results(0)
            TestAssert.Note($"选中宽度 W={r.Width}（真实 8，搜索范围 6..14）共识 = {r.Consensus}")

            ' 不同宽度下的对数似然不可比（ZOOPS 的 ΣR 随 W 放大），
            ' 若按原始 LL 择优会稳定选中 maxw=14 —— 见 CODE_REVIEW 缺陷 #10
            TestAssert.Check(r.Width >= 7 AndAlso r.Width <= 11,
                             $"选中宽度落在真实宽度附近 [7,11]，而非边界 14 [缺陷 #10]")
            TestAssert.Check(TestData.BestShiftedMatch(r.Consensus, motif) >= 7,
                             $"变宽搜索后仍能恢复 motif（匹配 {TestData.BestShiftedMatch(r.Consensus, motif)}/8）")
        End Sub

        ''' <summary>边界与异常输入不应崩溃</summary>
        Public Sub TestEdgeCases()
            TestAssert.Section("边界与异常输入")

            Dim alpha As New Alphabet(SeqTypes.DNA)

            ' 1) 序列短于 W
            Dim shortSeqs As New List(Of String)() From {"ACGT", "ACGTACGT"}
            Dim encs = TestData.EncodeAll(shortSeqs, alpha)
            TestAssert.CheckNoThrow(
                Sub()
                    Dim s As New EmSearch(encs, alpha, MakeOptions(SiteModel.Zoops, 10))
                    s.Discover()
                End Sub, "序列短于 motif 宽度时不抛异常")

            ' 2) 全部歧义字符
            Dim ambSeqs As New List(Of String)() From {"NNNNNNNNNN", "NNNNNNNNNN"}
            TestAssert.CheckNoThrow(
                Sub()
                    Dim s As New EmSearch(TestData.EncodeAll(ambSeqs, alpha), alpha,
                                          MakeOptions(SiteModel.Zoops, 6))
                    s.Discover()
                End Sub, "全歧义序列不抛异常")

            ' 3) 单条序列
            Dim one = TestData.PlantDna(1, 200, "ACGTTACGTA", 4321)
            TestAssert.CheckNoThrow(
                Sub()
                    Dim s As New EmSearch(TestData.EncodeAll(one.Sequences, alpha), alpha,
                                          MakeOptions(SiteModel.Zoops, 10))
                    s.Discover()
                End Sub, "单条序列不抛异常")

            ' 4) 最小宽度 W=2
            Dim tiny = TestData.PlantDna(10, 80, "CG", 1122, mutationRate:=0.0, withSiteRatio:=1.0)
            TestAssert.CheckNoThrow(
                Sub()
                    Dim s As New EmSearch(TestData.EncodeAll(tiny.Sequences, alpha), alpha,
                                          MakeOptions(SiteModel.Zoops, 2))
                    s.Discover()
                End Sub, "最小宽度 W=2 不抛异常")

            ' 5) 空 E 步：长度恰好等于 W
            Dim exact = alpha.Encode("ACGTACGTAC")
            Dim m As New EmModel(10, alpha, SiteModel.Zoops, TestData.UniformBg(4), 0.1)
            m.InitFromSeed(alpha.Encode("ACGTACGTAC"))
            TestAssert.CheckEqual(m.EStep(exact, False).Count, 1, "L = W 时恰有 1 个候选窗口")
        End Sub

        ''' <summary>端到端：读取 em_test 下种植的 FASTA 并完成 JSON 往返</summary>
        Public Sub TestFastaEndToEnd()
            TestAssert.Section("端到端：FASTA → 发现 → JSON 往返")

            For Each spec In New String()() {
                    New String() {"dna.fa", "dna", "ACGTTACGTA"},
                    New String() {"protein.fa", "protein", "GASTLSKL"}}

                Dim file = spec(0)
                Dim kind = spec(1)
                Dim expectMotif = spec(2)

                Dim path = TestData.FindDataFile(file)
                If path Is Nothing Then
                    TestAssert.Check(False, $"定位测试数据文件 {file}（请确认已随生成输出复制）")
                    Continue For
                End If

                Dim alpha As New Alphabet(If(kind = "dna", SeqTypes.DNA, SeqTypes.Protein))
                Dim records = FastaFile.Read(path)
                Dim seqs As New List(Of String)()
                For Each rec In records
                    seqs.Add(rec.SequenceData)
                Next

                TestAssert.Check(seqs.Count > 0, $"{file} 解析出 {seqs.Count} 条序列")
                If seqs.Count = 0 Then Continue For

                Dim opts = MakeOptions(SiteModel.Zoops, expectMotif.Length, seedCount:=8, maxIter:=120)
                Dim results = Discover(seqs, alpha, opts)
                TestAssert.Check(results.Count >= 1, $"{file} 至少发现 1 个 motif")
                If results.Count = 0 Then Continue For

                Dim r = results(0)
                Dim match = TestData.BestShiftedMatch(r.Consensus, expectMotif)
                TestAssert.Note($"{file}：{seqs.Count} 条序列，共识 = {r.Consensus}（与种植 motif 匹配 {match}/{expectMotif.Length}）")
                TestAssert.Check(match >= expectMotif.Length - 1,
                                 $"{file} 恢复出种植 motif（{match}/{expectMotif.Length}）")

                ' JSON 往返
                Dim dto As New MotifDto With {
                    .Id = "motif_1", .Width = r.Width, .Model = kind,
                    .Consensus = r.Consensus, .Lambda = r.Lambda,
                    .LogLikelihood = r.LogLikelihood, .LogLikelihoodRatio = r.LogLikelihoodRatio,
                    .Evalue = r.Evalue, .Iterations = r.Iterations, .Converged = r.Converged,
                    .Letters = alpha.Letters, .LogLikTrace = r.LogLikTrace}
                Dim pwm As New Dictionary(Of String, Double())()
                For a = 0 To alpha.Size - 1
                    Dim arr(r.Width - 1) As Double
                    For col = 0 To r.Width - 1
                        arr(col) = r.Pwm(col, a)
                    Next
                    pwm(alpha.Letters(a).ToString()) = arr
                Next
                dto.Pwm = pwm

                Dim sites As New List(Of SiteDto)()
                For i = 0 To r.Sites.Count - 1
                    sites.Add(New SiteDto With {
                        .Sequence = seqs(r.SiteSeqIndex(i)).Substring(0, Math.Min(8, seqs(r.SiteSeqIndex(i)).Length)),
                        .Start = r.Sites(i).Pos + 1,
                        .Strand = If(r.Sites(i).StrandMinus, "-", "+"),
                        .Posterior = r.Sites(i).Z,
                        .WindowLogR = r.Sites(i).LogR})
                Next
                dto.Sites = sites

                Dim json = JsonSerializer.Serialize(dto)
                Dim back = JsonSerializer.Deserialize(Of MotifDto)(json)
                TestAssert.Check(back IsNot Nothing AndAlso back.Consensus = r.Consensus AndAlso
                                 back.Pwm(alpha.Letters(0)).Length = r.Width,
                                 $"{file} 结果的 JSON 往返保真（{json.Length} 字节）")
            Next
        End Sub

    End Module

End Namespace
