' ============================================================================
' SelfTest.vb — 内置自检（EmMotif selftest）
' ----------------------------------------------------------------------------
' 1. E 步约束 [em.md §6]：OOPS ΣZ=1 / ZOOPS ≤1 / ANR 独立
' 2. 种植恢复（DNA ZOOPS）：共识序列、位点定位、LL 单调性 [em.md §4]
' 3. 蛋白序列恢复
' 4. 反义链恢复（--revcomp 语义）
' 5. ANR 多位点
' 6. χ² 生存函数 vs 文献分位数
' 7. PWM 列归一化 + 背景归一化
' 8. 多 motif 屏蔽：第二个 motif ≠ 第一个 [em.md §7]
' 9. JSON 序列化往返
' 10. FASTA 解析
' ============================================================================

Imports System.Text.Json
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif.EmMotif.Core
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif.EmMotif.Model
Imports SMRUCC.genomics.SequenceModel

Namespace EmMotif

    Public Module SelfTest

        Private _failures As Integer = 0
        Private _rng As New Random(42)

        Private Sub Check(cond As Boolean, name As String)
            If cond Then
                Console.WriteLine($"  [PASS] {name}")
            Else
                _failures += 1
                Console.WriteLine($"  [FAIL] {name}")
            End If
        End Sub

        Public Function RunAll() As Integer
            _failures = 0
            Console.WriteLine("=== EmMotif SelfTest ===")
            TestConstraints()
            TestDnaRecovery()
            TestProteinRecovery()
            TestRevcomp()
            TestAnr()
            TestChiSquare()
            TestNormalization()
            TestMultiMotif()
            TestJsonRoundTrip()

            Console.WriteLine($"=== {If(_failures = 0, "ALL TESTS PASSED", _failures & " TEST(S) FAILED")} ===")
            Return _failures
        End Function

        ' ---------------- 数据生成 ----------------

        Private Function PlantDna(n As Int32, seqLen As Int32, motif As String,
                                  withRatio As Double, rng As Random,
                                  ByRef truthPos As List(Of Int32)) As List(Of String)
            Dim seqs As New List(Of String)()
            Dim w = motif.Length
            For i = 0 To n - 1
                Dim ch(seqLen - 1) As Char
                For t = 0 To seqLen - 1
                    ch(t) = "ACGT"(rng.Next(4))
                Next
                Dim has = rng.NextDouble() < withRatio
                Dim pos = If(has, rng.Next(0, seqLen - w), -1)
                If has Then
                    Dim site = motif.ToCharArray()
                    For k = 0 To w - 1
                        If rng.NextDouble() < 0.12 Then
                            Dim others = "ACGT".Replace(site(k).ToString(), "")
                            site(k) = others(rng.Next(3))
                        End If
                    Next
                    Array.Copy(site, 0, ch, pos, w)
                End If
                truthPos.Add(pos)
                seqs.Add(New String(ch))
            Next
            Return seqs
        End Function

        Private Function EncodeAll(seqs As List(Of String), alpha As Alphabet) As List(Of Int32())
            Dim outList As New List(Of Int32())()
            For Each s In seqs
                outList.Add(alpha.Encode(s))
            Next
            Return outList
        End Function

        ' ---------------- 1. E 步约束 ----------------

        Private Sub TestConstraints()
            Console.WriteLine("-- E 步约束（三种模型）--")
            Dim alpha As New Alphabet(SeqTypes.DNA)
            Dim truth As New List(Of Int32)()
            Dim seqs = PlantDna(5, 200, "ACGTTACGTA", 0.8, _rng, truth)
            Dim encs = EncodeAll(seqs, alpha)
            Dim bg = BgOf(encs, alpha)
            Dim w As Int32 = 10

            Dim modelOops As New EmModel(w, alpha, SiteModel.Oops, bg, 0.1)
            modelOops.InitFromSeed(alpha.Encode("ACGTTACGTA"))
            Dim okOops = True
            For Each enc In encs
                Dim sites = modelOops.EStep(enc, False)
                Dim s = sites.Sum(Function(sp) sp.Z)
                If Math.Abs(s - 1.0) > 0.000001 Then okOops = False
            Next
            Check(okOops, "OOPS Σ_j Z_ij = 1（精确）")

            Dim modelZ As New EmModel(w, alpha, SiteModel.Zoops, bg, 0.1)
            modelZ.InitFromSeed(alpha.Encode("ACGTTACGTA"))
            Dim okZ = True
            For Each enc In encs
                Dim sites = modelZ.EStep(enc, False)
                Dim s = sites.Sum(Function(sp) sp.Z)
                If s > 1.0 + 0.000001 Then okZ = False
            Next
            Check(okZ, "ZOOPS Σ_j Z_ij ≤ 1")

            Dim modelA As New EmModel(w, alpha, SiteModel.Anr, bg, 0.1)
            modelA.InitFromSeed(alpha.Encode("ACGTTACGTA"))
            Dim sitesA = modelA.EStep(encs(0), False)
            Check(sitesA.Count = encs(0).Length - w + 1, $"ANR 候选数 = L−W+1（{sitesA.Count}）")
        End Sub

        Private Function BgOf(encs As List(Of Int32()), alpha As Alphabet) As Double()
            Dim cnt(alpha.Size - 1) As Double
            Dim total As Double = 0
            For Each enc In encs
                For Each a In enc
                    If a >= 0 Then
                        cnt(a) += 1.0
                        total += 1.0
                    End If
                Next
            Next
            Dim freq(alpha.Size - 1) As Double
            For a = 0 To alpha.Size - 1
                freq(a) = (cnt(a) + 0.1) / (total + 0.1 * alpha.Size)
            Next
            Return freq
        End Function

        ' ---------------- 2. DNA ZOOPS 种植恢复 ----------------

        Private Sub TestDnaRecovery()
            Console.WriteLine("-- DNA ZOOPS 种植恢复 --")
            Dim alpha As New Alphabet(SeqTypes.DNA)
            Dim motif = "ACGTTACGTA"
            Dim truth As New List(Of Int32)()
            Dim seqs = PlantDna(30, 200, motif, 0.8, _rng, truth)
            Dim encs = EncodeAll(seqs, alpha)
            Dim bg = BgOf(encs, alpha)

            Dim model As New EmModel(10, alpha, SiteModel.Zoops, bg, 0.1)
            model.InitFromSeed(alpha.Encode(motif))
            Dim trace As New List(Of Double)()
            Dim sitesList As New List(Of List(Of SitePosterior))()
            Dim ll = model.FullLogLik(encs, sitesList)
            trace.Add(ll)
            Dim monotone = True
            For it = 1 To 300
                sitesList.Clear()
                For Each enc In encs
                    sitesList.Add(model.EStep(enc, False))
                Next
                Dim nextModel = model.Clone()
                nextModel.MStep(encs, sitesList)
                model = nextModel
                Dim newLl = model.FullLogLik(encs, sitesList)
                If newLl < trace(trace.Count - 1) - 0.000001 Then monotone = False
                trace.Add(newLl)
                If Math.Abs(newLl - trace(trace.Count - 2)) < 0.0001 Then Exit For
            Next
            sitesList.Clear()
            For Each enc In encs
                sitesList.Add(model.EStep(enc, False))
            Next

            Dim consensus = model.Consensus()
            Dim match = consensus.Zip(motif, Function(a, b) a = b).Count(Function(x) x)
            Console.WriteLine($"  共识 = {consensus}（匹配 {match}/10）λ={model.Lambda:F3} 迭代 {trace.Count - 1} 轮")

            ' 位点定位
            Dim close As Int32 = 0
            Dim total As Int32 = 0
            For i = 0 To seqs.Count - 1
                If truth(i) < 0 Then Continue For
                total += 1
                Dim best = sitesList(i).OrderByDescending(Function(sp) sp.Z).First()
                If Math.Abs(best.Pos - truth(i)) <= 2 Then close += 1
            Next
            Console.WriteLine($"  位点定位（±2bp 内）: {close}/{total}")
            Check(match >= 9, "共识序列恢复 ≥9/10")
            Check(total > 0 AndAlso close >= total * 0.8, "位点定位 ≥80% 在 ±2bp")
            Check(monotone, "LL 逐轮单调不降 [em.md §4]")
            Check(model.Lambda > 0.7, $"ZOOPS λ 收敛到位点密度（λ={model.Lambda:F2} ≥ 0.7）")
        End Sub

        ' ---------------- 3. 蛋白恢复 ----------------

        Private Sub TestProteinRecovery()
            Console.WriteLine("-- 蛋白序列恢复 --")
            Dim alpha As New Alphabet(SeqTypes.Protein)
            Dim motif = "GASTLSKL"
            Dim w = motif.Length
            Dim seqs As New List(Of String)()
            For i = 0 To 24
                Dim ch(119) As Char
                For t = 0 To 119
                    ch(t) = alpha.Letters(_rng.Next(20))
                Next
                Dim pos = _rng.Next(0, 120 - w)
                Dim site = motif.ToCharArray()
                For k = 0 To w - 1
                    If rngNext(0.1) Then
                        site(k) = alpha.Letters(_rng.Next(20))
                    End If
                Next
                Array.Copy(site, 0, ch, pos, w)
                seqs.Add(New String(ch))
            Next
            Dim encs = EncodeAll(seqs, alpha)
            Dim bg = BgOf(encs, alpha)
            Dim model As New EmModel(w, alpha, SiteModel.Zoops, bg, 0.1)
            model.InitFromSeed(alpha.Encode(motif))
            For it = 1 To 300
                Dim sitesList As New List(Of List(Of SitePosterior))()
                For Each enc In encs
                    sitesList.Add(model.EStep(enc, False))
                Next
                Dim nextModel = model.Clone()
                nextModel.MStep(encs, sitesList)
                model = nextModel
            Next
            Dim consensus = model.Consensus()
            Dim match = consensus.Zip(motif, Function(a, b) a = b).Count(Function(x) x)
            Console.WriteLine($"  共识 = {consensus}（匹配 {match}/{w}）")
            Check(match >= 7, "蛋白共识恢复 ≥7/8")
        End Sub

        Private Function rngNext(p As Double) As Boolean
            Return _rng.NextDouble() < p
        End Function

        ' ---------------- 4. 反义链 ----------------

        Private Sub TestRevcomp()
            Console.WriteLine("-- 反义链恢复 --")
            Dim alpha As New Alphabet(SeqTypes.DNA)
            Dim motif = "ACGTCGTA"
            Dim w = motif.Length
            Dim rc = alpha.Revcomp(motif)
            Dim seqs As New List(Of String)()
            Dim strands As New List(Of Boolean)()
            Dim positions As New List(Of Int32)()
            For i = 0 To 24
                Dim ch(149) As Char
                For t = 0 To 149
                    ch(t) = "ACGT"(_rng.Next(4))
                Next
                Dim pos = _rng.Next(0, 150 - w)
                Dim minus = (i Mod 2 = 1)
                Dim site = If(minus, rc, motif).ToCharArray()
                Array.Copy(site, 0, ch, pos, w)
                seqs.Add(New String(ch))
                strands.Add(minus)
                positions.Add(pos)
            Next
            Dim encs = EncodeAll(seqs, alpha)
            Dim bg = BgOf(encs, alpha)
            Dim model As New EmModel(w, alpha, SiteModel.Zoops, bg, 0.1)
            model.InitFromSeed(alpha.Encode(motif))
            Dim sitesList As New List(Of List(Of SitePosterior))()
            For it = 1 To 250
                sitesList.Clear()
                For Each enc In encs
                    sitesList.Add(model.EStep(enc, True))    ' 双链
                Next
                Dim nextModel = model.Clone()
                nextModel.MStep(encs, sitesList)
                model = nextModel
            Next
            sitesList.Clear()
            For Each enc In encs
                sitesList.Add(model.EStep(enc, True))
            Next

            Dim consensus = model.Consensus()
            Dim match = consensus.Zip(motif, Function(a, b) a = b).Count(Function(x) x)
            Dim strandOk As Int32 = 0
            Dim total As Int32 = 0
            For i = 0 To seqs.Count - 1
                Dim best = sitesList(i).OrderByDescending(Function(sp) sp.Z).First()
                If best.Z < 0.5 Then Continue For
                total += 1
                If best.StrandMinus = strands(i) AndAlso Math.Abs(best.Pos - positions(i)) <= 2 Then strandOk += 1
            Next
            Console.WriteLine($"  共识 = {consensus}（匹配 {match}/{w}）链/位置正确 {strandOk}/{total}")
            Check(match >= 7 AndAlso total > 0 AndAlso strandOk >= total * 0.8, "双链扫描正确定位含链向")
        End Sub

        ' ---------------- 5. ANR 多位点 ----------------

        Private Sub TestAnr()
            Console.WriteLine("-- ANR 多位点 --")
            Dim alpha As New Alphabet(SeqTypes.DNA)
            Dim motif = "TTGACAAT"
            Dim w = motif.Length
            ' 每条序列植入 2~3 个位点
            Dim expectedSites As Int32 = 0
            Dim seqs As New List(Of String)()
            For i = 0 To 19
                Dim ch(299) As Char
                For t = 0 To 299
                    ch(t) = "ACGT"(_rng.Next(4))
                Next
                Dim nSite = 2 + _rng.Next(2)
                For s = 1 To nSite
                    Dim pos = _rng.Next(0, 300 - w)
                    Dim site = motif.ToCharArray()
                    For k = 0 To w - 1
                        If rngNext(0.08) Then site(k) = "ACGT"(_rng.Next(4))
                    Next
                    Array.Copy(site, 0, ch, pos, w)
                    expectedSites += 1
                Next
                seqs.Add(New String(ch))
            Next
            Dim encs = EncodeAll(seqs, alpha)
            Dim bg = BgOf(encs, alpha)
            Dim model As New EmModel(w, alpha, SiteModel.Anr, bg, 0.1)
            model.InitFromSeed(alpha.Encode(motif))
            Dim sitesList As New List(Of List(Of SitePosterior))()
            For it = 1 To 250
                sitesList.Clear()
                For Each enc In encs
                    sitesList.Add(model.EStep(enc, False))
                Next
                Dim nextModel = model.Clone()
                nextModel.MStep(encs, sitesList)
                model = nextModel
            Next
            sitesList.Clear()
            For Each enc In encs
                sitesList.Add(model.EStep(enc, False))
            Next
            Dim consensus = model.Consensus()
            Dim match = consensus.Zip(motif, Function(a, b) a = b).Count(Function(x) x)
            Dim strongSites = sitesList.Sum(Function(sl) sl.Where(Function(sp) sp.Z > 0.5).Count)
            Console.WriteLine($"  共识 = {consensus}（匹配 {match}/{w}）强位点 Z>0.5: {strongSites}（期望 ≈{expectedSites}）λ={model.Lambda:F4}")
            Check(match >= 7, "ANR 共识恢复 ≥7/8")
            Check(strongSites >= expectedSites * 0.7, "ANR 检出多数强位点")
        End Sub

        ' ---------------- 6. χ² ----------------

        Private Sub TestChiSquare()
            Console.WriteLine("-- χ² 生存函数 --")
            Dim cases As Double()() = {
                New Double() {1.0, 3.841, 0.05}, New Double() {2.0, 5.991, 0.05},
                New Double() {4.0, 9.488, 0.05}, New Double() {10.0, 18.307, 0.05},
                New Double() {1.0, 6.635, 0.01}, New Double() {2.0, 9.21, 0.01}}
            Dim ok = True
            For Each c In cases
                Dim sf = ChiSquare.ChiSquareSf(c(0), c(1))
                If Math.Abs(sf - c(2)) > 0.0005 Then
                    ok = False
                    Console.WriteLine($"    χ²({c(0)}) sf({c(1)}) = {sf:F5} ≠ {c(2)}")
                End If
            Next
            Check(ok, "χ² sf 与文献分位数一致（≤5e-4）")
            Dim ev = ChiSquare.MotifEValue(50.0, 21.0, 1000.0)
            Check(ev > 0 AndAlso ev < 1.0, $"E-value 合理范围（LLR=50, df=21: E={ev:E2}）")
        End Sub

        ' ---------------- 7. 归一化 ----------------

        Private Sub TestNormalization()
            Console.WriteLine("-- PWM/背景归一化 --")
            Dim alpha As New Alphabet(SeqTypes.DNA)
            Dim truth As New List(Of Int32)()
            Dim seqs = PlantDna(10, 150, "GCGCGTATA", 1.0, _rng, truth)
            Dim encs = EncodeAll(seqs, alpha)
            Dim bg = BgOf(encs, alpha)
            Check(Math.Abs(bg.Sum() - 1.0) < 0.000000001, "背景频率和 = 1")

            Dim model As New EmModel(9, alpha, SiteModel.Oops, bg, 0.1)
            model.InitFromSeed(alpha.Encode("GCGCGTATA"))
            For it = 1 To 50
                Dim sitesList As New List(Of List(Of SitePosterior))()
                For Each enc In encs
                    sitesList.Add(model.EStep(enc, False))
                Next
                Dim nextModel = model.Clone()
                nextModel.MStep(encs, sitesList)
                model = nextModel
            Next
            Dim colOk = True
            For k = 0 To 8
                Dim s As Double = 0
                For a = 0 To 3
                    s += model.Pwm(k, a)
                Next
                If Math.Abs(s - 1.0) > 0.000000001 Then colOk = False
            Next
            Check(colOk, "PWM 每列概率和 = 1（含伪计数 M 步）")
        End Sub

        ' ---------------- 8. 多 motif 屏蔽 ----------------

        Private Sub TestMultiMotif()
            Console.WriteLine("-- 多 motif 屏蔽重跑 --")
            Dim alpha As New Alphabet(SeqTypes.DNA)
            Dim motif1 = "ACGTTACGTA"
            Dim motif2 = "TTGGCCAGGA"
            Dim w1 = motif1.Length
            Dim w2 = motif2.Length
            Dim seqs As New List(Of String)()
            For i = 0 To 29
                Dim ch(219) As Char
                For t = 0 To 219
                    ch(t) = "ACGT"(_rng.Next(4))
                Next
                Array.Copy(motif1.ToCharArray(), 0, ch, _rng.Next(0, 220 - w1), w1)
                Array.Copy(motif2.ToCharArray(), 0, ch, _rng.Next(0, 220 - w2), w2)
                seqs.Add(New String(ch))
            Next
            Dim encs = EncodeAll(seqs, alpha)
            Dim opts As New SearchOptions With {
                .Model = SiteModel.Zoops, .MinW = 10, .MaxW = 10, .NumMotifs = 2,
                .Revcomp = False, .SeedStrategy = "enriched", .SeedCount = 10,
                .MaxIter = 150, .RngSeed = 5}
            Dim search As New EmSearch(encs, alpha, opts)
            Dim motifs = search.Discover()
            Check(motifs.Count = 2, $"发现 2 个 motif（实际 {motifs.Count}）")
            If motifs.Count = 2 Then
                Dim d1 = motifs(0).Consensus
                Dim d2 = motifs(1).Consensus
                Console.WriteLine($"  motif1 = {d1}  motif2 = {d2}")
                ' 两个 motif 应彼此不同且各自接近真值
                Dim m1ok = d1.Zip(motif1, Function(a, b) a = b).Count(Function(x) x) >= 8 OrElse
                           d1.Zip(motif2, Function(a, b) a = b).Count(Function(x) x) >= 8
                Dim m2ok = d2.Zip(motif1, Function(a, b) a = b).Count(Function(x) x) >= 8 OrElse
                           d2.Zip(motif2, Function(a, b) a = b).Count(Function(x) x) >= 8
                Check(m1ok AndAlso m2ok AndAlso String.CompareOrdinal(d1, d2) <> 0,
                      "两个植入 motif 分别恢复且互不相同")
            End If
        End Sub

        ' ---------------- 9. JSON 往返 ----------------

        Private Sub TestJsonRoundTrip()
            Console.WriteLine("-- JSON 序列化往返 --")
            Dim opts As New JsonSerializerOptions With {.WriteIndented = False}
            Dim dto As New MotifDto With {
                .Id = "motif_1", .Width = 4, .Model = "zoops", .Consensus = "ACGT",
                .Lambda = 0.5, .LogLikelihood = -123.45, .Evalue = 0.000123,
                .Iterations = 42, .Converged = True, .Letters = "ACGT"}
            Dim pwmDict As New Dictionary(Of String, Double()) From {{"A", {0.7, 0.1, 0.1, 0.1}}}
            dto.Pwm = pwmDict
            Dim sites As New List(Of SiteDto) From {
                New SiteDto With {.Sequence = "s1", .Start = 10, .Strand = "+", .Posterior = 0.95, .Segment = "ACGT"}}
            dto.Sites = sites
            Dim json = JsonSerializer.Serialize(dto, opts)
            Dim back = JsonSerializer.Deserialize(Of MotifDto)(json)
            Check(back IsNot Nothing AndAlso back.Id = "motif_1" AndAlso back.Width = 4 AndAlso
                  back.Pwm("A")(0) = 0.7 AndAlso back.Sites(0).Start = 10 AndAlso
                  back.Sites(0).Strand = "+", "MotifDto JSON 往返保真")
        End Sub

    End Module

End Namespace
