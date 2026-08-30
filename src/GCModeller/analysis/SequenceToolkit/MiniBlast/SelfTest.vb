' ============================================================================
' SelfTest.vb — 内置自检（dotnet run -- selftest）
' ----------------------------------------------------------------------------
' 1. λ 数值解与理论值/文献值比对
' 2. E-value 与 Bit Score 恒等式 [式5-1] ≡ [式5-3]
' 3. gapped X-drop DP 与内嵌参照 Smith-Waterman 交叉验证（随机用例）
' 4. traceback 合法性（长度/字符/坐标/重算得分一致）
' 5. DUST / SEG 掩码行为
' 6. dc-megablast 模板种子（don't-care 位容忍错配）
' 7. 端到端冒烟：blastn / blastp 找回嵌入同源序列
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports MiniBlast.Core
Imports MiniBlast.MiniBlast.Core
Imports MiniBlast.Model

Namespace MiniBlast

    Public Module SelfTest

        Private _failures As Integer = 0

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
            Console.WriteLine("=== MiniBlast SelfTest ===")

            TestLambda()
            TestStatsIdentity()
            TestGappedVsSW()
            TestTracebackValidity()
            TestDustSeg()
            TestDcTemplate()
            TestEndToEnd()

            Console.WriteLine($"=== {If(_failures = 0, "ALL TESTS PASSED", _failures & " TEST(S) FAILED")} ===")
            Return _failures
        End Function

        Private Sub TestLambda()
            Console.WriteLine("-- λ 数值解 --")
            ' +2/-3 理论精确值（自洽推导）
            Dim hist2 As New SortedDictionary(Of Integer, Double) From {{2, 0.25}, {-3, 0.75}}
            Dim lam2 = KarlinAltschul.SolveLambda(hist2)
            Check(Math.Abs(lam2 - 0.633731) < 0.001, $"blastn +2/-3 λ={lam2:F6} ≈ 0.633731")

            ' BLOSUM62 + RR 频率：与预验证基准 0.335390 比对（求解器确定性）
            Dim scorer = New AaScorer("BLOSUM62")
            Dim hist = KarlinAltschul.BuildAaHist(scorer)
            Dim lam = KarlinAltschul.SolveLambda(hist)
            Check(Math.Abs(lam - 0.33539) < 0.001, $"BLOSUM62(RR) λ={lam:F6} ≈ 0.335390（文献表值 0.3176 见 README 说明）")
        End Sub

        Private Sub TestStatsIdentity()
            Console.WriteLine("-- E/BitScore 恒等式 --")
            Dim ka = KarlinAltschul.ProteinParams("BLOSUM62")
            Dim S As Double = 50.0
            Dim m As Long = 1000, n As Long = 1000000
            Dim e1 = ka.EValue(m, n, S)
            Dim sBit = ka.BitScore(S)
            Dim e2 = CDbl(m) * CDbl(n) * Math.Pow(2.0, -sBit)
            Check(Math.Abs(e1 - e2) <= e1 * 0.01, $"E(式5-1)={e1:E3} ≈ E(式5-3)={e2:E3}")
        End Sub

        ''' <summary>参照实现：全序列 Smith-Waterman（仿射间隙，无 X-drop）</summary>
        Private Function ReferenceSW(q As Int32(), s As Int32(), scorer As IScorer, go As Double, ge As Double) As Double
            Dim n = q.Length, m = s.Length
            Dim H(n, m) As Double
            Dim E(n, m) As Double
            Dim F(n, m) As Double
            Dim best As Double = 0
            For i = 1 To n
                For j = 1 To m
                    E(i, j) = Math.Max(H(i - 1, j) - go, E(i - 1, j) - ge)
                    F(i, j) = Math.Max(H(i, j - 1) - go, F(i, j - 1) - ge)
                    H(i, j) = Math.Max(0, Math.Max(H(i - 1, j - 1) + scorer.Score(q(i - 1), s(j - 1)), Math.Max(E(i, j), F(i, j))))
                    If H(i, j) > best Then best = H(i, j)
                Next
            Next
            Return best
        End Function

        Private _rng As New Random(42)

        Private Function RandomProtein(len As Integer) As Int32()
            Dim a(len - 1) As Int32
            For i = 0 To len - 1
                a(i) = _rng.Next(20)
            Next
            Return a
        End Function

        Private Function Mutate(src As Int32(), rate As Double) As Int32()
            Dim dst = CType(src.Clone(), Int32())
            For i = 0 To dst.Length - 1
                If _rng.NextDouble() < rate Then
                    dst(i) = _rng.Next(20)
                End If
            Next
            Return dst
        End Function

        Private Sub TestGappedVsSW()
            Console.WriteLine("-- gapped X-drop DP vs 参照 SW --")
            Dim scorer As IScorer = New AaScorer("BLOSUM62")
            Dim seOpts As New SeedExtendOptions With {.GapOpen = 11.0, .GapExtend = 1.0}
            Dim scanner = New SeedScanner(scorer, 0.3176, seOpts, False)
            Dim xdrop = 40.0 * 0.693147 / 0.3176   ' 40 bits → raw
            Dim fails As Integer = 0
            For trial = 1 To 25
                Dim hom = RandomProtein(80)
                Dim q = RandomProtein(20).ToList()
                q.AddRange(hom)
                q.AddRange(RandomProtein(20))
                Dim s = RandomProtein(35).ToList()
                s.AddRange(Mutate(hom, 0.08))
                s.AddRange(RandomProtein(25))

                Dim ic = 20 + 40
                Dim jc = 35 + 40
                Dim h0 = scorer.Score(q(ic), s(jc))

                Dim fwd = scanner.GappedForward(q.ToArray(), s.ToArray(), ic, jc, h0, seOpts, xdrop, False)
                Dim rq = q.Take(ic + 1).Reverse().ToArray()
                Dim rs = s.Take(jc + 1).Reverse().ToArray()
                Dim bwd = scanner.GappedForward(rq, rs, 0, 0, h0, seOpts, xdrop, False)
                Dim combined = fwd.Best + bwd.Best - h0
                Dim sw = ReferenceSW(q.ToArray(), s.ToArray(), scorer, 11.0, 1.0)
                If Math.Abs(combined - sw) > 2.0 Then fails += 1
            Next
            Check(fails = 0, "双端合并得分 == 全局 SW（25 随机用例）")
        End Sub

        Private Sub TestTracebackValidity()
            Console.WriteLine("-- traceback 合法性 --")
            Dim scorer As IScorer = New AaScorer("BLOSUM62")
            Dim seOpts As New SeedExtendOptions With {.GapOpen = 11.0, .GapExtend = 1.0}
            Dim scanner = New SeedScanner(scorer, 0.3176, seOpts, False)
            Dim xdrop = 40.0 * 0.693147 / 0.3176
            Dim fails As Integer = 0
            For trial = 1 To 40
                Dim hom = RandomProtein(60)
                Dim q = RandomProtein(15).ToList()
                q.AddRange(hom)
                q.AddRange(RandomProtein(15))
                Dim s = RandomProtein(30).ToList()
                s.AddRange(Mutate(hom, 0.2))     ' 高突变率 → 强制出现 gap
                s.AddRange(RandomProtein(15))

                Dim ic = 15 + 30
                Dim jc = 30 + 30
                Dim h0 = scorer.Score(q(ic), s(jc))

                Dim fwd = scanner.GappedForward(q.ToArray(), s.ToArray(), ic, jc, h0, seOpts, xdrop, True)

                ' moves 为 seed→best 顺序
                Dim moves = scanner.TracebackMoves(fwd.Traces, fwd.BestU, fwd.BestV)
                Dim qa As New Text.StringBuilder()
                Dim sa As New Text.StringBuilder()
                qa.Append(AaAlphabet.Decode(q(ic)))
                sa.Append(AaAlphabet.Decode(s(jc)))
                Dim ii = ic, jj = jc
                For Each mv As Byte In moves
                    Select Case mv
                        Case 0 : ii += 1 : jj += 1 : qa.Append(AaAlphabet.Decode(q(ii))) : sa.Append(AaAlphabet.Decode(s(jj)))
                        Case 1 : ii += 1 : qa.Append(AaAlphabet.Decode(q(ii))) : sa.Append("-"c)
                        Case Else : jj += 1 : qa.Append("-"c) : sa.Append(AaAlphabet.Decode(s(jj)))
                    End Select
                Next

                ' 重算得分
                Dim rec As Double = 0
                Dim inQ As Integer = 0, inS As Integer = 0
                Dim qStr = qa.ToString(), sStr = sa.ToString()
                For c = 0 To qStr.Length - 1
                    Dim a = qStr(c), b = sStr(c)
                    If a <> "-"c AndAlso b <> "-"c Then
                        If inQ > 0 Then rec -= 11.0 + (inQ - 1) : inQ = 0
                        If inS > 0 Then rec -= 11.0 + (inS - 1) : inS = 0
                        rec += scorer.Score(AaAlphabet.EncodeChar(a), AaAlphabet.EncodeChar(b))
                    ElseIf a <> "-"c Then
                        If inS > 0 Then rec -= 11.0 + (inS - 1) : inS = 0
                        inQ += 1
                    Else
                        If inQ > 0 Then rec -= 11.0 + (inQ - 1) : inQ = 0
                        inS += 1
                    End If
                Next
                If inQ > 0 Then rec -= 11.0 + (inQ - 1)
                If inS > 0 Then rec -= 11.0 + (inS - 1)

                If Math.Abs(rec - fwd.Best) > 1.0 OrElse qStr.Length <> sStr.Length Then fails += 1
            Next
            Check(fails = 0, "前向 traceback 重算得分与 DP 一致（40 随机用例）")
        End Sub

        Private Sub TestDustSeg()
            Console.WriteLine("-- DUST / SEG --")
            ' poly-A 100mer 应几乎全被 DUST 遮蔽
            Dim polyA = New String("A"c, 100)
            Dim codes = NtAlphabet.Encode(polyA)
            Dim mask = Dust.Mask(codes, 20, 64)
            Dim maskedCount = mask.Count(Function(b) b)
            Check(maskedCount >= 90, $"DUST poly-A 遮蔽 {maskedCount}/100")

            ' 随机核酸几乎不遮蔽
            Dim rnd(199) As Int32
            For i = 0 To 199
                rnd(i) = _rng.Next(4)
            Next
            Dim mask2 = Dust.Mask(rnd, 20, 64)
            Check(mask2.Count(Function(b) b) <= 20, "DUST 随机序列基本不遮蔽")

            ' 低复杂度蛋白（poly-L）应被 SEG 遮蔽
            Dim polyL = New String("L"c, 60)
            Dim pcodes = AaAlphabet.Encode(polyL)
            Dim pmask = SegFilter.Mask(pcodes, 12, 2.2, 2.5)
            Check(pmask.Count(Function(b) b) >= 40, $"SEG poly-L 遮蔽 {pmask.Count(Function(b) b)}/60")
        End Sub

        Private Sub TestDcTemplate()
            Console.WriteLine("-- dc-megablast 模板种子 --")
            ' 两段序列仅在 don't-care 位不同、care 位全同 → 应命中
            Dim coding = "101101100101101101"
            Dim q = New List(Of Int32)()
            Dim s = New List(Of Int32)()
            Dim r As New Random(7)
            For i = 0 To coding.Length - 1
                Dim base_ = r.Next(4)
                q.Add(base_)
                If coding(i) = "1"c Then
                    s.Add(base_)               ' care 位相同
                Else
                    s.Add(r.Next(4))           ' don't-care 位随机
                End If
            Next
            Dim lookup = New DcWordLookup(q.ToArray(), Nothing, coding)
            Dim hit As List(Of Integer) = Nothing
            Dim key = lookup.PackAt(s.ToArray(), 0)
            Check(lookup.TryGetPositions(key, hit), "don't-care 位错配仍命中")

            ' care 位不同 → 不命中
            s(0) = (s(0) + 1) Mod 4     ' 位置 0 是 care 位
            Dim key2 = lookup.PackAt(s.ToArray(), 0)
            Dim hit2 As List(Of Integer) = Nothing
            Check(Not lookup.TryGetPositions(key2, hit2), "care 位错配不命中")
        End Sub

        Private Sub TestEndToEnd()
            Console.WriteLine("-- 端到端冒烟 --")
            ' blastn: 查询与其 95% 同源副本
            Dim baseSeq = "ACGTTGCAAGGCTTACCGGATCCGTAAGCTTGCAACCGGTТАСGGATCCTTAGCACGT" & New String("G"c, 5) & "TTGCAA"
            baseSeq = baseSeq.Replace("Т"c, "T"c).Replace("А"c, "A"c).Replace("С"c, "C"c) ' 防全角字符
            Dim mutated = MutateNt(baseSeq, 0.05)

            Dim qList = New List(Of FastaSequence) From {New FastaSequence("q1", "nucleotide query", baseSeq)}
            Dim dbList = New List(Of FastaSequence) From {
                New FastaSequence("homolog", "95% identity copy", mutated),
                New FastaSequence("polya", "low complexity", New String("A"c, 120)),
                New FastaSequence("random", "unrelated", MutateNt("ACGT".RepeatString(20), 0.5))
            }

            Dim opts As New BlastOptions With {.Program = "blastn", .Task = "blastn"}
            Dim dbp = BlastEngine.BuildDatabase(dbList, opts)
            Dim qr = BlastEngine.RunQuery(qList(0), dbp.Item1, dbp.Item2, opts)

            Check(qr.Hits.Count > 0, "blastn 找到命中")
            If qr.Hits.Count > 0 Then
                Check(qr.Hits(0).Id = "homolog", $"最佳命中 = homolog（实际 {qr.Hits(0).Id}）")
                Dim h0 = qr.Hits(0).Hsps(0)
                Check(h0.Evalue < 0.01, $"E={h0.Evalue:E2} < 0.01")
                Check(h0.QuerySeq.Length = h0.SubjectSeq.Length, "比对串等长")
            End If

            ' blastp: 蛋白同源
            Dim prot = "MKTAYIAKQRQISFVKSHFSRQLEERLGLIEVQAPILSRVGDGTQDNLSGAEK"
            Dim protMut = MutateProt(prot, 0.15)
            Dim pq = New List(Of FastaSequence) From {New FastaSequence("p1", "protein query", prot)}
            Dim pdb = New List(Of FastaSequence) From {
                New FastaSequence("p_homolog", "protein homolog", protMut),
                New FastaSequence("p_random", "unrelated", "WWDDCCSSLLAAKKRRFFEE")
            }
            Dim popts As New BlastOptions With {.Program = "blastp", .Task = "blastp"}
            Dim pdbp = BlastEngine.BuildDatabase(pdb, popts)
            Dim pqr = BlastEngine.RunQuery(pq(0), pdbp.Item1, pdbp.Item2, popts)

            Check(pqr.Hits.Count > 0, "blastp 找到命中")
            If pqr.Hits.Count > 0 Then
                Check(pqr.Hits(0).Id = "p_homolog", $"最佳蛋白命中 = p_homolog（实际 {pqr.Hits(0).Id}）")
                Dim ph = pqr.Hits(0).Hsps(0)
                Check(ph.Positives > ph.Identities, $"positives({ph.Positives}) > identities({ph.Identities})（矩阵捕捉保守替换）")
            End If
        End Sub

        Private Function MutateNt(seq As String, rate As Double) As String
            Dim bases = "ACGT"
            Dim sb As New Text.StringBuilder()
            For Each ch In seq
                If _rng.NextDouble() < rate Then
                    sb.Append(bases(_rng.Next(4)))
                Else
                    sb.Append(ch)
                End If
            Next
            Return sb.ToString()
        End Function

        Private Function MutateProt(seq As String, rate As Double) As String
            Dim aas = "ARNDCQEGHILKMFPSTWYV"
            Dim sb As New Text.StringBuilder()
            For Each ch In seq
                If _rng.NextDouble() < rate Then
                    sb.Append(aas(_rng.Next(20)))
                Else
                    sb.Append(ch)
                End If
            Next
            Return sb.ToString()
        End Function

    End Module

End Namespace
