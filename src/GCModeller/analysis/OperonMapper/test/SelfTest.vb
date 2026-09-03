' ============================================================================
' SelfTest.vb — 内置自检（OperonPredictor selftest）
' ----------------------------------------------------------------------------
' 1. UniOP 先验公式与退化截断 [§1.4]
' 2. KDE 闭式后验分离度（自洽合成，镜像 Python 验证）
' 3. 终止子扫描（正链 + 负链读框恢复）
' 4. 启动子扫描
' 5. 二项 LLR 单调性
' 6. Viterbi / 前向后向
' 7. GFF3/PTT 解析
' 8. 端到端：合成基因组种植恢复（对准确率 + 操纵子恢复）
' 9. 同源模块：条形码/保守对计数
' 10. JSON 往返
' ============================================================================

Imports System.IO
Imports System.Text.Json
Imports SMRUCC.genomics.Model.OperonMapper.OperonPredictor.Core
Imports SMRUCC.genomics.Model.OperonMapper.OperonPredictor.Model

Namespace OperonPredictor

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
            Console.WriteLine("=== OperonPredictor SelfTest ===")
            TestPrior()
            TestUniopPosterior()
            TestTerminator()
            TestPromoter()
            TestBinomLlr()
            TestHmm()
            TestParsers()
            TestEndToEnd()
            TestHomologyModule()
            TestJsonRoundTrip()
            Console.WriteLine($"=== {If(_failures = 0, "ALL TESTS PASSED", _failures & " TEST(S) FAILED")} ===")
            Return _failures
        End Function

        ' ---------------- 1. 先验 ----------------

        Private Sub TestPrior()
            Console.WriteLine("-- UniOP 先验 [§1.4] --")
            Dim q1 = UniopModel.ComputePrior(100, 28)
            Dim q2 = UniopModel.ComputePrior(10, 15)
            Dim q3 = UniopModel.ComputePrior(100, 0)
            Console.WriteLine($"  M=100,O=28 → {q1:F3}（(100−56)/72=0.611）  M<O → {q2}  O=0 → {q3}")
            Check(Math.Abs(q1 - 44.0 / 72.0) < 0.000000001, "公式 (M−2O)/(M−O)")
            Check(Math.Abs(q2 - 0.5) < 0.000000001, "M ≤ O → 0.5 退化")
            Check(Math.Abs(q3 - 0.95) < 0.000000001, "O=0 → 0.95 截断")
        End Sub

        ' ---------------- 2. KDE 后验 ----------------

        Private Sub TestUniopPosterior()
            Console.WriteLine("-- KDE 闭式后验分离度 --")
            Dim convPool As New List(Of Double)()
            Dim divPool As New List(Of Double)()
            For i = 0 To 13
                convPool.Add(60.0 + _rng.NextDouble() * 240.0)
                divPool.Add(80.0 + _rng.NextDouble() * 240.0)
            Next
            Dim opDist As New List(Of Double)()
            For i = 0 To 59
                Dim z = Gaussian() * 10.0 + 15.0
                opDist.Add(Math.Max(0.0, z))
            Next
            Dim bndDist As New List(Of Double)()
            For i = 0 To 39
                bndDist.Add((convPool(_rng.Next(14)) + divPool(_rng.Next(14))) / 2.0)
            Next
            Dim sameD As New List(Of Double)()
            sameD.AddRange(opDist)
            sameD.AddRange(bndDist)
            Dim uniop As New UniopModel(sameD, convPool, divPool, 100, 28)
            Dim shortMean = opDist.Average(Function(d) uniop.Posterior(d))
            Dim bndMean = bndDist.Average(Function(d) uniop.Posterior(d))
            Dim acc = (opDist.Where(Function(d) uniop.Posterior(d) > 0.5).Count +
                       bndDist.Where(Function(d) uniop.Posterior(d) <= 0.5).Count) / CDbl(opDist.Count + bndDist.Count)
            Console.WriteLine($"  q={uniop.QPrior:F3}  短距离后验均值={shortMean:F3}  边界后验均值={bndMean:F3}  准确率={acc:F3}")
            Check(0.4 < uniop.QPrior AndAlso uniop.QPrior < 0.8, "先验接近真实比例")
            Check(shortMean > 0.9, "短距离高后验")
            Check(bndMean < 0.35, "边界低后验")
            Check(acc > 0.85, "0.5 阈值准确率 >85% [operon.md 引言 >85% 目标]")
        End Sub

        Private Function Gaussian() As Double
            Dim u1 = Math.Max(0.000000000001, _rng.NextDouble())
            Dim u2 = _rng.NextDouble()
            Return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2)
        End Function

        ' ---------------- 3/4. 序列信号 ----------------

        Private Sub TestTerminator()
            Console.WriteLine("-- 终止子扫描 --")
            Dim t1 = "CCGCGCGCGGAATCCGCGCGCGGTTTTTT"     ' 茎10+环3+茎10+U6
            Dim s1 = SignalScan.ScanTerminator(t1, False)
            Dim negs As New List(Of Double)()
            For i = 0 To 4
                Dim sb As New System.Text.StringBuilder()
                For k = 0 To 59
                    sb.Append("ACGT"(_rng.Next(4)))
                Next
                negs.Add(SignalScan.ScanTerminator(sb.ToString(), False))
            Next
            Dim sn = negs.Max()
            Console.WriteLine($"  强终止子 = {s1:F3}  随机最大 = {sn:F3}")
            Check(s1 > 0.6, "强终止子检出")
            Check(sn < 0.4, "随机序列不误报")
            ' 负链读框：基因组上是 revcomp(t1)，扫描 revcomp 恢复读框
            Dim ch = t1.ToCharArray()
            Array.Reverse(ch)
            For k = 0 To ch.Length - 1
                Select Case ch(k)
                    Case "A"c : ch(k) = "T"c
                    Case "T"c : ch(k) = "A"c
                    Case "G"c : ch(k) = "C"c
                    Case "C"c : ch(k) = "G"c
                End Select
            Next
            Dim genomicMinus = New String(ch)
            Dim s2 = SignalScan.ScanTerminator(genomicMinus, True)
            Console.WriteLine($"  负链读框恢复 = {s2:F3}")
            Check(Math.Abs(s2 - s1) < 0.000000001, "负链读框扫描等价")
        End Sub

        Private Sub TestPromoter()
            Console.WriteLine("-- 启动子扫描 --")
            Dim p = "TTGACAACGTTCGACGTATAAT"      ' 间距 17
            Dim s = SignalScan.ScanPromoter(p, False)
            Dim sn As Double = 0
            For i = 0 To 19
                Dim sb As New System.Text.StringBuilder()
                For k = 0 To 59
                    sb.Append("ACGT"(_rng.Next(4)))
                Next
                sn = Math.Max(sn, SignalScan.ScanPromoter(sb.ToString(), False))
            Next
            Console.WriteLine($"  经典启动子 = {s:F3}  随机最大 = {sn:F3}")
            Check(s > 0.7 AndAlso s > sn + 0.05, "-35/-10 框检出（σ70 17bp 间距）")
        End Sub

        ' ---------------- 5. 二项 LLR ----------------

        Private Sub TestBinomLlr()
            Console.WriteLine("-- 二项 LLR --")
            Dim llr0 = HomologySignals.BarcodeLlr(0, 35, 0.15, 0.45)
            Dim llr18 = HomologySignals.BarcodeLlr(18, 35, 0.15, 0.45)
            Console.WriteLine($"  h=0: {llr0:F2}  h=18: {llr18:F2}")
            Check(llr0 > 3 AndAlso llr18 < -3, "条形码 LLR 方向正确")
            Dim mono = True
            Dim prev = Double.PositiveInfinity
            For h = 0 To 35
                Dim v = HomologySignals.BarcodeLlr(h, 35, 0.15, 0.45)
                If v > prev + 0.000000001 Then mono = False
                prev = v
            Next
            Check(mono, "LLR 随 Hamming 单调递减")
        End Sub

        ' ---------------- 6. HMM ----------------

        Private Sub TestHmm()
            Console.WriteLine("-- Viterbi / 前向后向 --")
            Dim truth = New List(Of Boolean)()
            For i = 0 To 7
                truth.Add(True)
            Next
            truth.Add(False)
            For i = 0 To 5
                truth.Add(True)
            Next
            Dim run As New List(Of PairSignals)()
            For Each t In truth
                Dim s As New PairSignals()
                s.CombinedLlr = If(t, 2.0, -2.5) + Gaussian()
                run.Add(s)
            Next
            Dim runs As New List(Of List(Of PairSignals)) From {run}
            Integrator.RunHmm(runs, 0.7, New IntegrationOptions())
            Dim acc = run.Zip(truth, Function(s, t) s.ViterbiState = t).Count(Function(x) x) / CDbl(truth.Count)
            Dim opSum As Double = 0
            Dim opCnt As Int32 = 0
            For i = 0 To truth.Count - 1
                If truth(i) Then
                    opSum += run(i).HmmPosterior
                    opCnt += 1
                End If
            Next
            Dim opPost = opSum / Math.Max(1, opCnt)
            Dim bndPost = run(8).HmmPosterior
            Console.WriteLine($"  Viterbi 准确率 = {acc:F3}  op 段后验 = {opPost:F2}  边界后验 = {bndPost:F2}")
            Check(acc >= 0.95, "Viterbi 解码准确")
            Check(opPost > 0.7 AndAlso bndPost < 0.5, "FB 后验分离")
            ' 极端发射
            Dim allBnd As New List(Of PairSignals)()
            For i = 0 To 9
                allBnd.Add(New PairSignals With {.CombinedLlr = -5.0})
            Next
            Dim runs2 As New List(Of List(Of PairSignals)) From {allBnd}
            Integrator.RunHmm(runs2, 0.7, New IntegrationOptions())
            Check(allBnd.All(Function(s) Not s.ViterbiState), "全负发射 → 全边界")
            Dim allOp As New List(Of PairSignals)()
            For i = 0 To 9
                allOp.Add(New PairSignals With {.CombinedLlr = 5.0})
            Next
            Dim runs3 As New List(Of List(Of PairSignals)) From {allOp}
            Integrator.RunHmm(runs3, 0.7, New IntegrationOptions())
            Check(allOp.All(Function(s) s.ViterbiState), "全正发射 → 全同操纵子")
        End Sub

        ' ---------------- 7. 解析 ----------------

        Private Sub TestParsers()
            Console.WriteLine("-- GFF3 / PTT 解析 --")
            Dim gffPath = Path.Combine(Path.GetTempPath(), "op_test.gff")
            File.WriteAllLines(gffPath, {
                "##gff-version 3", "# comment", "chr1	prod	gene	101	400	.	+	.	ID=g1;Name=geneA",
                "chr1	prod	CDS	501	900	.	+	.	0	ID=g2;locus_tag=locB",
                "chr1	prod	CDS	1200	1600	.	-	.	0	ID=g3",
                "chr2	prod	CDS	10	500	.	+	.	0	ID=g4"})
            Dim g = AnnotationIO.ReadGff(gffPath, "c1")
            Check(g.Count = 4, $"GFF 记录数 = 4（实际 {g.Count}）")
            Dim gg2 = g.First(Function(x) x.Id = "locB")
            Check(gg2.StartMin = 501 AndAlso gg2.EndMax = 900 AndAlso gg2.Strand = "+"c, "GFF 坐标/链")
            Dim pairs = GeneModel.EnumeratePairs(g)
            Check(pairs.Count = 3, $"相邻对数 = 3（实际 {pairs.Count}）")
            Check(pairs(0).Relation = StrandRelation.Same AndAlso pairs(0).Igd = 100, "同链对 IGD=100")
            Check(pairs(1).Relation = StrandRelation.Convergent, "+/− 相邻 → 趋同")
            Check(pairs(2).A.Contig = "chr2", "跨 contig 不相邻")

            Dim pttPath = Path.Combine(Path.GetTempPath(), "op_test.ptt")
            File.WriteAllLines(pttPath, {
                "Ptt header line 1", "2 - 3 line", "# loc lines below",
                "101..400	+	-	-	geneA	-",
                "501..900	+	-	-	geneB	-"})
            Dim p = AnnotationIO.ReadPtt(pttPath, "c")
            Check(p.Count = 2 AndAlso p(0).StartMin = 101 AndAlso p(1).Id = "geneB", "PTT 解析")
        End Sub

        ' ---------------- 8. 端到端 ----------------

        Private Sub TestEndToEnd()
            Console.WriteLine("-- 端到端种植恢复 --")
            ' 合成基因组：操纵子（2-4 基因，IGD 5-25）+ 边界（同链 150-400 / 发散 / 趋同）
            Dim genes As New List(Of Gene)()
            Dim truthPair As New List(Of Boolean)()    ' 相邻对真值（同操纵子）
            Dim pos As Int32 = 100
            Dim gid As Int32 = 0
            Dim nOp = 0
            For opI = 0 To 29
                Dim n = 2 + _rng.Next(3)
                For k = 0 To n - 1
                    Dim L = 300 + _rng.Next(600)
                    genes.Add(New Gene With {.Id = $"g{gid}", .Contig = "chr",
                                             .StartMin = pos, .EndMax = pos + L - 1,
                                             .Strand = "+"c, .Name = $"g{gid}"})
                    pos += L
                    If k < n - 1 Then
                        pos += 5 + _rng.Next(21)
                        truthPair.Add(True)
                        nOp += 1
                    End If
                    gid += 1
                Next
                ' 边界
                Dim r = _rng.NextDouble()
                If r < 0.5 Then
                    pos += 150 + _rng.Next(250)
                    truthPair.Add(False)
                ElseIf r < 0.75 Then
                    ' 趋同边界：插入 − 链基因（对1: +/− 趋同；对2: −/+ 发散）
                    Dim L = 300 + _rng.Next(400)
                    genes.Add(New Gene With {.Id = $"g{gid}", .Contig = "chr",
                                             .StartMin = pos, .EndMax = pos + L - 1,
                                             .Strand = "-"c, .Name = $"g{gid}"})
                    pos += L + 30 + _rng.Next(100)
                    gid += 1
                    truthPair.Add(False)
                    truthPair.Add(False)
                Else
                    ' 发散：− 链基因先出现，随后 + 链基因
                    Dim L = 300 + _rng.Next(400)
                    genes.Add(New Gene With {.Id = $"g{gid}", .Contig = "chr",
                                             .StartMin = pos, .EndMax = pos + L - 1,
                                             .Strand = "-"c, .Name = $"g{gid}"})
                    pos += L + 40 + _rng.Next(120)
                    gid += 1
                    Dim L2 = 300 + _rng.Next(600)
                    genes.Add(New Gene With {.Id = $"g{gid}", .Contig = "chr",
                                             .StartMin = pos, .EndMax = pos + L2 - 1,
                                             .Strand = "+"c, .Name = $"g{gid}"})
                    pos += L2
                    truthPair.Add(False)
                    truthPair.Add(False)
                    gid += 1
                End If
            Next

            Dim opts As New EngineOptions With {.Integration = New IntegrationOptions()}
            Dim result = Engine.Predict(genes, Nothing, Nothing, Nothing, opts)
            Dim pairs = result.Item1
            Dim signals = result.Item2
            ' 真值对齐：truthPair 按基因对顺序记录；反义对跳过对应条目数
            Dim tpIdx As Int32 = 0
            Dim hit As Int32 = 0
            Dim tot As Int32 = 0
            For i = 0 To pairs.Count - 1
                If Not pairs(i).IsSameStrand Then
                    ' 反义对：真值中对应 1 或 2 个 False——跳过相应数量
                    If pairs(i).Relation = StrandRelation.Divergent Then tpIdx += 2 Else tpIdx += 1
                    Continue For
                End If
                If tpIdx >= truthPair.Count Then Exit For
                Dim expected = truthPair(tpIdx)
                Dim predicted = signals(i).ViterbiState
                If expected = predicted Then hit += 1
                tot += 1
                tpIdx += 1
            Next
            Dim acc = hit / CDbl(Math.Max(1, tot))
            Dim operons = Engine.AssembleOperons(pairs, signals)
            Dim multiOp = operons.Where(Function(o) o.NumGenes >= 2).Count
            Console.WriteLine($"  对级准确率 = {acc:F3}（{hit}/{tot}）  操纵子数 = {operons.Count}（多基因 {multiOp}）")
            Check(acc >= 0.85, "端到端对级准确率 ≥85% [operon.md 引言目标]")
            Check(multiOp >= 15, $"恢复 ≥15 个多基因操纵子（实际 {multiOp}）")
        End Sub

        ' ---------------- 9. 同源模块 ----------------

        Private Sub TestHomologyModule()
            Console.WriteLine("-- 同源模块 --")
            ' 参考基因组：5 基因 a b c d e 相邻同链
            Dim refGenes As New List(Of Gene)()
            Dim p As Int32 = 100
            For Each nm In {"ra", "rb", "rc", "rd", "re"}
                refGenes.Add(New Gene With {.Id = nm, .Contig = "ref1",
                                            .StartMin = p, .EndMax = p + 299,
                                            .Strand = "+"c, .Name = nm})
                p += 310
            Next
            Dim refMap As New Dictionary(Of String, List(Of Gene)) From {{"ref1", refGenes}}
            Dim homMap As New Dictionary(Of String, Dictionary(Of String, Tuple(Of String, Double)))()
            homMap("ga") = New Dictionary(Of String, Tuple(Of String, Double)) From {{"ref1", Tuple.Create("ra", 100.0)}}
            homMap("gb") = New Dictionary(Of String, Tuple(Of String, Double)) From {{"ref1", Tuple.Create("rb", 100.0)}}
            homMap("gc") = New Dictionary(Of String, Tuple(Of String, Double)) From {{"ref1", Tuple.Create("rd", 100.0)}}
            Dim hs As New HomologySignals(homMap, refMap)
            Dim h As Int32 = 0
            Dim ru As Int32 = 0
            hs.BarcodeStats("ga", "gb", h, ru)
            Check(ru = 1 AndAlso h = 0, "条形码：两基因同 ref 均有同源 → h=0")
            hs.BarcodeStats("ga", "gc", h, ru)
            Check(ru = 1 AndAlso h = 1, "条形码：仅一方有同源 → h=1")
            Dim cons = hs.ConservedPairCount("ga", "gb")
            Check(cons = 1, $"保守对：ra/rb 相邻同序（计数 = {cons}）")
            Dim cons2 = hs.ConservedPairCount("gb", "gc")
            Check(cons2 = 0, "非同序对（rb/rd 隔一个）不保守")
            Dim pc = hs.PcbbhCount("gb", "gc")
            Check(pc = 0, "PCBBH：rd 与 rb 不相邻")
        End Sub

        ' ---------------- 10. JSON 往返 ----------------

        Private Sub TestJsonRoundTrip()
            Console.WriteLine("-- JSON 往返 --")
            Dim opts As New JsonSerializerOptions With {.WriteIndented = False}
            Dim dto As New OperonDto With {
                .OperonId = "op_1", .Contig = "chr", .Strand = "+",
                .Start = 100, .End = 2000, .NumGenes = 3,
                .Genes = New List(Of String) From {"g1", "g2", "g3"},
                .MeanPairPosterior = 0.95}
            Dim json = JsonSerializer.Serialize(dto, opts)
            Dim back = JsonSerializer.Deserialize(Of OperonDto)(json)
            Check(back IsNot Nothing AndAlso back.OperonId = "op_1" AndAlso back.NumGenes = 3 AndAlso
                  Math.Abs(back.MeanPairPosterior - 0.95) < 0.000000001 AndAlso
                  back.Genes(2) = "g3", "OperonDto JSON 往返保真")
        End Sub

    End Module

End Namespace
