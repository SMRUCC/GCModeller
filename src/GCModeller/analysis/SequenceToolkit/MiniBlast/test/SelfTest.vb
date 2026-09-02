' ============================================================================
' SelfTest.vb — MiniBlast 内置自检
' ----------------------------------------------------------------------------
' 运行：  cd test && dotnet run
' 退出码：失败用例数（0 = 全绿）
'
' 测试分四层：
'   第1层 单元自检   λ 数值解 / E·BitScore 恒等式 / gapped DP vs 参照 SW /
'                    traceback 合法性 / DUST·SEG / dc 模板种子
'   第2层 触发回归   扫描器必须能触发延伸（锁死两-hit 死锁回归）
'   第3层 端到端     用 test/*.fa 跑 blastn / megablast / dc-megablast /
'                    blastn-short / blastp / blastp-short
'   第4层 不变量导出 每条 HSP 过 8 条结构不变量；报告落盘 JSON 并回读校验
'
' 第3、4 层走的是 CLI 完全相同的代码路径（TaskPresets → BlastSearch →
' BlastReportJson），因此自检覆盖的是真实调用链而非测试内私有逻辑。
' ============================================================================

Imports System.IO
Imports MiniBlast.Core
Imports MiniBlast.Model
Imports MiniBlast.Options
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module SelfTest

    Private _failures As Integer = 0
    Private _checks As Integer = 0

    ' ---------------------------------------------------------------- 基础设施

    Private Sub Section(title As String)
        Console.WriteLine()
        Console.WriteLine($"-- {title} --")
    End Sub

    Private Sub Check(cond As Boolean, name As String)
        _checks += 1
        If cond Then
            Console.WriteLine($"  [PASS] {name}")
        Else
            _failures += 1
            Console.WriteLine($"  [FAIL] {name}")
        End If
    End Sub

    ''' <summary>输出目录：JSON 报告落在这里，不污染源码目录</summary>
    Private ReadOnly Property OutDir As String
        Get
            Return Path.Combine(AppContext.BaseDirectory, "selftest_results")
        End Get
    End Property

    ''' <summary>定位测试数据：优先输出目录（*.fa 已随产物拷贝），其次向上查找源码目录</summary>
    Private Function FindData(name As String) As String
        Dim p = Path.Combine(AppContext.BaseDirectory, name)
        If File.Exists(p) Then Return p

        Dim d As DirectoryInfo = New DirectoryInfo(AppContext.BaseDirectory)
        While d IsNot Nothing
            Dim c = Path.Combine(d.FullName, name)
            If File.Exists(c) Then Return c
            Dim t = Path.Combine(d.FullName, "test", name)
            If File.Exists(t) Then Return t
            d = d.Parent
        End While
        Throw New FileNotFoundException($"找不到测试数据 {name}（已从 {AppContext.BaseDirectory} 向上搜索）")
    End Function

    Private Function LoadFasta(name As String) As Dictionary(Of String, FastaSeq)
        Dim map As New Dictionary(Of String, FastaSeq)()
        For Each s As FastaSeq In FastaFile.Read(FindData(name))
            map(s.locus_tag) = s
        Next
        Return map
    End Function

    ' ---------------------------------------------------------------- 入口

    Public Function Main(args As String()) As Integer
        Return RunAll()
    End Function

    Public Function RunAll() As Integer
        _failures = 0
        _checks = 0
        Directory.CreateDirectory(OutDir)

        Console.WriteLine("=== MiniBlast SelfTest ===")
        Console.WriteLine($"运行时目录: {AppContext.BaseDirectory}")
        Console.WriteLine($"结果输出:   {OutDir}")

        ' 第 1 层
        Section("单元测试 · 统计与 DP")
        TestLambda()
        TestStatsIdentity()
        TestGappedVsSW()
        TestTracebackValidity()

        Section("单元测试 · 过滤与种子")
        TestDustSeg()
        TestDcTemplate()

        ' 第 2 层
        Section("回归 · 扫描器延伸触发")
        TestScannerTrigger()

        ' 第 3 + 4 层
        Section("端到端 · 核酸（nt_query.fa vs nt_db.fa）")
        TestNucleotideSearch()

        Section("端到端 · 蛋白（aa_query.fa vs aa_db.fa）")
        TestProteinSearch()

        Section("导出链路 · JSON 往返")
        TestReportRoundTrip()

        Console.WriteLine()
        Console.WriteLine($"=== {_checks} 项检查：" &
                          If(_failures = 0, "全部通过", $"{_failures} 项失败") & " ===")
        Return _failures
    End Function

    ' ================================================================ 第1层

    Private Sub TestLambda()
        ' +2/-3 理论精确值（自洽推导）
        Dim hist2 As New SortedDictionary(Of Integer, Double) From {{2, 0.25}, {-3, 0.75}}
        Dim lam2 = KarlinAltschul.SolveLambda(hist2)
        Check(Math.Abs(lam2 - 0.633731) < 0.001, $"blastn +2/-3 λ={lam2:F6} ≈ 0.633731")

        ' BLOSUM62 + RR 频率：与预验证基准 0.335390 比对（求解器确定性）
        Dim scorer As New AaScorer("BLOSUM62")
        Dim hist = KarlinAltschul.BuildAaHist(scorer)
        Dim lam = KarlinAltschul.SolveLambda(hist)
        Check(Math.Abs(lam - 0.33539) < 0.001,
              $"BLOSUM62(RR) λ={lam:F6} ≈ 0.335390（文献表值 0.3176 见 README 说明）")
    End Sub

    Private Sub TestStatsIdentity()
        Dim ka = KarlinAltschul.ProteinParams("BLOSUM62")
        Dim S As Double = 50.0
        Dim m As Long = 1000, n As Long = 1000000
        Dim e1 = ka.EValue(m, n, S)
        Dim sBit = ka.BitScore(S)
        Dim e2 = CDbl(m) * CDbl(n) * Math.Pow(2.0, -sBit)
        Check(Math.Abs(e1 - e2) <= e1 * 0.01, $"E(式5-1)={e1:E3} ≈ E(式5-3)={e2:E3}")
    End Sub

    ''' <summary>参照实现：全序列 Smith-Waterman（仿射间隙，无 X-drop）</summary>
    Private Function ReferenceSW(q As Int32(), s As Int32(), scorer As IScorer,
                                 go As Double, ge As Double) As Double
        ' [NCBI] 长度 k 的 gap 代价 = go + k·ge ⇒ 首个 gap 残基扣 go+ge
        Dim openCost = go + ge
        Dim n = q.Length, m = s.Length
        Dim H(n, m) As Double
        Dim E(n, m) As Double
        Dim F(n, m) As Double
        Dim best As Double = 0
        For i = 1 To n
            For j = 1 To m
                E(i, j) = Math.Max(H(i - 1, j) - openCost, E(i - 1, j) - ge)
                F(i, j) = Math.Max(H(i, j - 1) - openCost, F(i, j - 1) - ge)
                H(i, j) = Math.Max(0, Math.Max(H(i - 1, j - 1) + scorer.Score(q(i - 1), s(j - 1)),
                                               Math.Max(E(i, j), F(i, j))))
                If H(i, j) > best Then best = H(i, j)
            Next
        Next
        Return best
    End Function

    Private Sub TestGappedVsSW()
        Dim rng As New Random(42)
        Dim scorer As IScorer = New AaScorer("BLOSUM62")
        Dim go As Double = 11.0, ge As Double = 1.0
        Dim seOpts As New SeedExtendOptions With {.GapOpen = go, .GapExtend = ge}
        Dim scanner As New SeedScanner(scorer, 0.3176, seOpts, False)
        Dim xdrop = 40.0 * 0.693147 / 0.3176   ' 40 bits → raw
        Dim fails As Integer = 0
        For trial = 1 To 25
            Dim hom = RandomAa(rng, 80)
            Dim q = New List(Of Int32)(RandomAa(rng, 20))
            q.AddRange(hom)
            q.AddRange(RandomAa(rng, 20))
            Dim s = New List(Of Int32)(RandomAa(rng, 35))
            s.AddRange(MutateAa(rng, hom, 0.08))
            s.AddRange(RandomAa(rng, 25))

            Dim ic = 20 + 40
            Dim jc = 35 + 40
            Dim h0 = scorer.Score(q(ic), s(jc))

            Dim fwd = scanner.GappedForward(q.ToArray(), s.ToArray(), ic, jc, h0, seOpts, xdrop, False)
            Dim rq = q.Take(ic + 1).Reverse().ToArray()
            Dim rs = s.Take(jc + 1).Reverse().ToArray()
            Dim bwd = scanner.GappedForward(rq, rs, 0, 0, h0, seOpts, xdrop, False)
            Dim combined = fwd.Best + bwd.Best - h0
            Dim sw = ReferenceSW(q.ToArray(), s.ToArray(), scorer, go, ge)
            If Math.Abs(combined - sw) > 2.0 Then fails += 1
        Next
        Check(fails = 0, "双端合并得分 == 全局参照 SW（25 随机用例）")
    End Sub

    Private Sub TestTracebackValidity()
        Dim rng As New Random(4242)
        Dim scorer As IScorer = New AaScorer("BLOSUM62")
        Dim go As Double = 11.0, ge As Double = 1.0
        Dim seOpts As New SeedExtendOptions With {.GapOpen = go, .GapExtend = ge}
        Dim scanner As New SeedScanner(scorer, 0.3176, seOpts, False)
        Dim xdrop = 40.0 * 0.693147 / 0.3176
        Dim fails As Integer = 0
        For trial = 1 To 40
            Dim hom = RandomAa(rng, 60)
            Dim q = New List(Of Int32)(RandomAa(rng, 15))
            q.AddRange(hom)
            q.AddRange(RandomAa(rng, 15))
            Dim s = New List(Of Int32)(RandomAa(rng, 30))
            s.AddRange(MutateAa(rng, hom, 0.2))     ' 高突变率 → 强制出现 gap
            s.AddRange(RandomAa(rng, 15))

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

            ' 重算得分（NCBI gap：长度 L 的 gap 代价 = go + L·ge）
            Dim rec As Double = 0
            Dim inQ As Integer = 0, inS As Integer = 0
            Dim qStr = qa.ToString(), sStr = sa.ToString()
            For c = 0 To qStr.Length - 1
                Dim a = qStr(c), b = sStr(c)
                If a <> "-"c AndAlso b <> "-"c Then
                    If inQ > 0 Then rec -= go + inQ * ge : inQ = 0
                    If inS > 0 Then rec -= go + inS * ge : inS = 0
                    rec += scorer.Score(AaAlphabet.EncodeChar(a), AaAlphabet.EncodeChar(b))
                ElseIf a <> "-"c Then
                    If inS > 0 Then rec -= go + inS * ge : inS = 0
                    inQ += 1
                Else
                    If inQ > 0 Then rec -= go + inQ * ge : inQ = 0
                    inS += 1
                End If
            Next
            If inQ > 0 Then rec -= go + inQ * ge
            If inS > 0 Then rec -= go + inS * ge

            If Math.Abs(rec - fwd.Best) > 1.0 OrElse qStr.Length <> sStr.Length Then fails += 1
        Next
        Check(fails = 0, "前向 traceback 重算得分与 DP 一致（40 随机用例）")
    End Sub

    Private Sub TestDustSeg()
        Dim rng As New Random(7)
        ' poly-A 100mer 应几乎全被 DUST 遮蔽
        Dim polyA = New String("A"c, 100)
        Dim codes = NtAlphabet.Encode(polyA)
        Dim mask = Dust.Mask(codes, 20, 64)
        Dim maskedCount = mask.Count(Function(b) b)
        Check(maskedCount >= 90, $"DUST poly-A 遮蔽 {maskedCount}/100")

        ' 随机核酸几乎不遮蔽
        Dim rnd = RandomNt(rng, 200)
        Dim mask2 = Dust.Mask(rnd, 20, 64)
        Check(mask2.Count(Function(b) b) <= 20, "DUST 随机序列基本不遮蔽")

        ' 低复杂度蛋白（poly-L）应被 SEG 遮蔽
        Dim polyL = New String("L"c, 60)
        Dim pcodes = AaAlphabet.Encode(polyL)
        Dim pmask = SegFilter.Mask(pcodes, 12, 2.2, 2.5)
        Check(pmask.Count(Function(b) b) >= 40, $"SEG poly-L 遮蔽 {pmask.Count(Function(b) b)}/60")
    End Sub

    Private Sub TestDcTemplate()
        Dim rng As New Random(7)
        ' 两段序列仅在 don't-care 位不同、care 位全同 → 应命中
        Dim coding = "101101100101101101"
        Dim q = New List(Of Int32)()
        Dim s = New List(Of Int32)()
        For i = 0 To coding.Length - 1
            Dim base_ = rng.Next(4)
            q.Add(base_)
            If coding(i) = "1"c Then
                s.Add(base_)               ' care 位相同
            Else
                s.Add(rng.Next(4))         ' don't-care 位随机
            End If
        Next
        Dim lookup As New DcWordLookup(q.ToArray(), Nothing, coding)
        Dim hit As List(Of Integer) = Nothing
        Dim key = lookup.PackAt(s.ToArray(), 0)
        Check(lookup.TryGetPositions(key, hit), "don't-care 位错配仍命中")

        ' care 位不同 → 不命中
        s(0) = (s(0) + 1) Mod 4     ' 位置 0 是 care 位
        Dim key2 = lookup.PackAt(s.ToArray(), 0)
        Dim hit2 As List(Of Integer) = Nothing
        Check(Not lookup.TryGetPositions(key2, hit2), "care 位错配不命中")

        ' WordSize 必须等于模板 weight（扫描器用它判断两-hit 的非重叠距离）
        Check(lookup.WordSize = 11, $"dc 模板 WordSize = weight = 11（实际 {lookup.WordSize}）")

        ' 窗口滑动建表键一致性：每个窗口的键都必须能查回该窗口自身的起点。
        ' 这是 DcWordLookup 滚动打包错误（键未屏蔽高位 / caresFilled 双扣减）的回归锁。
        Dim qn = RandomNt(New Random(11), 120)
        Dim lut As New DcWordLookup(qn, Nothing, TaskPresets.DcTemplateCoding)
        Dim span = lut.Span
        Dim mismatched As Integer = 0, windows As Integer = 0
        For start = 0 To qn.Length - span
            Dim k = lut.PackAt(qn, start)
            Dim pos As List(Of Integer) = Nothing
            If lut.TryGetPositions(k, pos) Then
                windows += 1
                If Not pos.Contains(start) Then mismatched += 1
            End If
        Next
        Check(mismatched = 0, $"dc 模板每个窗口的键都能查回自身起点（{windows} 窗，{mismatched} 处错）")
    End Sub

    ' ================================================================ 第2层

    ''' <summary>直接用 SeedScanner 扫描一对核酸序列（绕开 BlastEngine，构造最小复现）</summary>
    Private Function RunScan(q As Int32(), s As Int32(), wordSize As Integer,
                             Optional reward As Double = 2.0,
                             Optional penalty As Double = -3.0) As List(Of RawHsp)
        Dim seOpts As New SeedExtendOptions With {
            .WordSize = wordSize,
            .WindowTwoHit = 40,
            .UseTwoHit = True,
            .GapOpen = 5.0,
            .GapExtend = 2.0
        }
        Dim scorer As IScorer = New NtScorer(reward, penalty)
        Dim ka = KarlinAltschul.NtParams(reward, penalty)
        Dim scanner As New SeedScanner(scorer, ka.Lambda, seOpts, True)
        Dim lookup As IWordLookup = New NtWordLookup(q, Nothing, wordSize)
        Return scanner.ScanSequence(lookup, s, Nothing, q, 15.0)
    End Function

    Private Sub TestScannerTrigger()
        ' 用例1：精确自匹配（q == s）。这是两-hit 死锁的最小复现——
        ' 对角线上每个位置都命中，若 lastHit 被无条件覆盖，则 d 恒为 1 < W，永不触发。
        Dim rng As New Random(20260903)
        Dim seq = RandomNt(rng, 200)
        Dim self = RunScan(seq, seq, 11)
        Check(self.Count > 0, "精确自匹配能触发延伸（两-hit 死锁回归）")
        If self.Count > 0 Then
            Dim best = self.OrderByDescending(Function(h) h.RawScore).First()
            Check(best.Identities = 200, $"精确自匹配 identities=200（实际 {best.Identities}）")
            Check(best.QueryFrom = 0 AndAlso best.QueryTo = 199,
                  $"精确自匹配覆盖全长 0..199（实际 {best.QueryFrom}..{best.QueryTo}）")
        End If

        ' 用例2：仅中段同源（两端随机）。同源区内含错配，仍需触发且落在同源区内。
        Dim r2 As New Random(777)
        Dim hom = RandomNt(r2, 80)
        Dim q = New List(Of Int32)(RandomNt(r2, 40))
        q.AddRange(hom)
        q.AddRange(RandomNt(r2, 40))
        Dim s = New List(Of Int32)(RandomNt(r2, 30))
        s.AddRange(MutateNtCodes(r2, hom, 0.08))
        s.AddRange(RandomNt(r2, 30))

        Dim hsps = RunScan(q.ToArray(), s.ToArray(), 11)
        Check(hsps.Count > 0, "局部同源能触发延伸")
        If hsps.Count > 0 Then
            Dim best = hsps.OrderByDescending(Function(h) h.RawScore).First()
            ' 覆盖度判据而非严格包含：X-drop 延伸可能把侧翼偶然匹配的残基一并纳入
            ' （边界处 1/4 概率同字符），这是正常行为。
            Dim qCov = OverlapLen(best.QueryFrom, best.QueryTo, 40, 119)
            Dim sCov = OverlapLen(best.SubjectFrom, best.SubjectTo, 30, 109)
            Check(qCov >= 72 AndAlso sCov >= 72,
                  $"HSP 覆盖同源区 ≥90%（q 覆盖 {qCov}/80，s 覆盖 {sCov}/80；" &
                  $"q={best.QueryFrom}..{best.QueryTo} s={best.SubjectFrom}..{best.SubjectTo}）")
            ' 且不能整体落在同源区之外
            Check(OverlapLen(best.QueryFrom, best.QueryTo, 40, 119) > 0, "HSP 与同源区有重叠")
        End If
    End Sub

    ''' <summary>两个闭区间的重叠长度</summary>
    Private Function OverlapLen(a1 As Integer, a2 As Integer, b1 As Integer, b2 As Integer) As Integer
        Dim lo = Math.Max(a1, b1)
        Dim hi = Math.Min(a2, b2)
        If hi < lo Then Return 0
        Return hi - lo + 1
    End Function

    ' ================================================================ 第3+4层

    Private Class CaseResult
        Public Name As String
        Public Report As BlastReport
        Public Opts As BlastOptions
        Public Ka As KaParams
        Public JsonPath As String
        Public QuerySeqs As Dictionary(Of String, FastaSeq)
        Public DbSeqs As Dictionary(Of String, FastaSeq)
        Public ElapsedMs As Long
    End Class

    Private Function RunCase(name As String, queryFile As String, dbFile As String,
                             program As String, task As String,
                             Optional configure As Action(Of BlastOptions) = Nothing) As CaseResult
        Dim opts As New BlastOptions With {.Program = program, .Task = task}
        TaskPresets.Apply(opts)
        If configure IsNot Nothing Then configure(opts)

        Dim qSeqs = LoadFasta(queryFile)
        Dim dSeqs = LoadFasta(dbFile)

        Dim sw = Diagnostics.Stopwatch.StartNew()
        Dim report = BlastSearch.Run(FindData(queryFile), FindData(dbFile), opts)
        sw.Stop()

        Dim jsonPath = BlastReportJson.Save(report, Path.Combine(OutDir, name & ".json"), True)

        Dim res As New CaseResult With {
            .Name = name, .Report = report, .Opts = opts,
            .Ka = BlastSearch.StatsFor(opts),
            .JsonPath = jsonPath, .QuerySeqs = qSeqs, .DbSeqs = dSeqs,
            .ElapsedMs = sw.ElapsedMilliseconds
        }

        Console.WriteLine($"  [{name}] {sw.ElapsedMilliseconds} ms → {jsonPath}")
        ' 第 4 层：每条 HSP 过结构不变量
        ValidateAllHsps(res)
        Return res
    End Function

    Private Function QueryById(r As BlastReport, id As String) As QueryResult
        If r Is Nothing OrElse r.Queries Is Nothing Then Return Nothing
        For Each q In r.Queries
            If q.Id = id Then Return q
        Next
        Return Nothing
    End Function

    Private Function FindHit(qr As QueryResult, id As String) As Hit
        If qr Is Nothing OrElse qr.Hits Is Nothing Then Return Nothing
        For Each h In qr.Hits
            If h.Id = id Then Return h
        Next
        Return Nothing
    End Function

    ''' <summary>最佳 HSP（BuildResult 已按 evalue 升序排）</summary>
    Private Function Best(h As Hit) As Hsp
        Return h.Hsps(0)
    End Function

    Private Function Pct(part As Integer, whole As Integer) As Double
        If whole <= 0 Then Return 0
        Return 100.0 * part / whole
    End Function

    Private Sub DumpHits(qr As QueryResult)
        If qr Is Nothing Then
            Console.WriteLine("     (无查询结果)")
            Return
        End If
        If qr.Hits Is Nothing OrElse qr.Hits.Count = 0 Then
            Console.WriteLine($"     {qr.Id}: 0 hits")
            Return
        End If
        For Each h In qr.Hits
            Dim b = Best(h)
            ' 一致率按「比对长度」计（NCBI pident 惯例），而非整条 subject 长度
            Dim alnLen = If(b.QuerySeq Is Nothing, 0, b.QuerySeq.Length)
            Console.WriteLine($"     {qr.Id} → {h.Id,-16} bit={b.BitScore,8:F1} E={b.Evalue,10:E2} " &
                              $"id={b.Identities}/{alnLen} ({Pct(b.Identities, alnLen):F1}%) " &
                              $"pos={b.Positives} gap={b.Gaps} q={b.QueryFrom}..{b.QueryTo} s={b.SubjectFrom}..{b.SubjectTo}")
        Next
    End Sub

    ' ---------------------------------------------------------------- 核酸

    Private Sub TestNucleotideSearch()
        ' ---------- blastn (W=11) ----------
        Dim bn = RunCase("blastn", "nt_query.fa", "nt_db.fa", "blastn", "blastn")
        Dim nt1 = QueryById(bn.Report, "nt1")
        Dim nt2 = QueryById(bn.Report, "nt2_short")

        Check(nt1 IsNot Nothing AndAlso nt1.Hits.Count > 0, "nt1 有命中")
        If nt1 IsNot Nothing AndAlso nt1.Hits.Count > 0 Then
            DumpHits(nt1)
            Check(nt1.Hits.Count >= 3, $"nt1 至少 3 个命中（实际 {nt1.Hits.Count}）")
            Check(nt1.Hits(0).Id = "nt1_exact", $"nt1 首个命中 = nt1_exact（实际 {nt1.Hits(0).Id}）")

            Dim ex = FindHit(nt1, "nt1_exact")
            Check(ex IsNot Nothing, "nt1 找回 nt1_exact")
            If ex IsNot Nothing Then
                Dim h = Best(ex)
                Check(h.Identities = 250, $"nt1_exact identities=250（实际 {h.Identities}）")
                Check(h.Gaps = 0, $"nt1_exact gaps=0（实际 {h.Gaps}）")
                Check(h.QueryFrom = 1 AndAlso h.QueryTo = 250,
                      $"nt1_exact query 1..250（实际 {h.QueryFrom}..{h.QueryTo}）")
                Check(h.SubjectFrom = 1 AndAlso h.SubjectTo = 250,
                      $"nt1_exact subject 1..250（实际 {h.SubjectFrom}..{h.SubjectTo}）")
                Check(h.Evalue < 1.0E-100, $"nt1_exact E={h.Evalue:E2} < 1e-100")
                Check(h.BitScore > 100, $"nt1_exact bit={h.BitScore:F1} > 100")
            End If

            Dim m5 = FindHit(nt1, "nt1_mut5")
            Check(m5 IsNot Nothing, "nt1 找回 nt1_mut5（5% 突变）")
            If m5 IsNot Nothing Then
                Dim h = Best(m5)
                Check(Pct(h.Identities, 250) >= 95.0,
                      $"nt1_mut5 一致率 {Pct(h.Identities, 250):F1}% ≥ 95%")
                Check(h.Evalue < 1.0E-50, $"nt1_mut5 E={h.Evalue:E2} < 1e-50")
            End If

            Dim m25 = FindHit(nt1, "nt1_mut25")
            Check(m25 IsNot Nothing, "nt1 找回 nt1_mut25（25% 分歧）")
            If m25 IsNot Nothing Then
                Dim h = Best(m25)
                Check(Pct(h.Identities, 250) >= 75.0,
                      $"nt1_mut25 最佳 HSP 一致率 {Pct(h.Identities, 250):F1}% ≥ 75%")
                Check(h.Evalue < 10.0, $"nt1_mut25 E={h.Evalue:E2} < 10")
            End If

            ' 反例：低复杂度与无关序列都不应被召回
            Check(FindHit(nt1, "polya_lowcompl") Is Nothing, "nt1 不召回 polya_lowcompl（DUST 遮蔽）")
            Check(FindHit(nt1, "unrelated") Is Nothing, "nt1 不召回 unrelated")
        End If

        Check(nt2 IsNot Nothing, "nt2_short 有查询结果")
        If nt2 IsNot Nothing Then
            DumpHits(nt2)
            Check(nt2.Hits.Count > 0, "nt2_short 有命中")
            If nt2.Hits.Count > 0 Then
                Check(nt2.Hits(0).Id = "nt1_exact", $"nt2_short 首个命中 = nt1_exact（实际 {nt2.Hits(0).Id}）")
                Dim h = Best(nt2.Hits(0))
                Check(h.Identities = 40, $"nt2_short identities=40（实际 {h.Identities}）")
                Check(h.Gaps = 0, $"nt2_short gaps=0（实际 {h.Gaps}）")
                ' 短查询是 nt1_exact 的 0-based [50,90) 精确子串 ⇒ 1-based 51..90
                Check(h.SubjectFrom = 51 AndAlso h.SubjectTo = 90,
                      $"nt2_short subject 51..90（实际 {h.SubjectFrom}..{h.SubjectTo}）")
                Check(h.QueryFrom = 1 AndAlso h.QueryTo = 40,
                      $"nt2_short query 1..40（实际 {h.QueryFrom}..{h.QueryTo}）")
            End If
        End If

        ' ---------- megablast (W=28) ----------
        Dim mb = RunCase("megablast", "nt_query.fa", "nt_db.fa", "blastn", "megablast")
        Dim m1 = QueryById(mb.Report, "nt1")
        If m1 IsNot Nothing Then DumpHits(m1)
        Check(m1 IsNot Nothing AndAlso FindHit(m1, "nt1_exact") IsNot Nothing, "megablast 找回 nt1_exact")
        Check(m1 IsNot Nothing AndAlso FindHit(m1, "nt1_mut5") IsNot Nothing, "megablast 找回 nt1_mut5")
        Check(m1 IsNot Nothing AndAlso FindHit(m1, "nt1_mut25") Is Nothing,
              "megablast 不召回 nt1_mut25（W=28 敏感度边界）")

        ' ---------- dc-megablast (11/18 模板) ----------
        Dim dc = RunCase("dc-megablast", "nt_query.fa", "nt_db.fa", "blastn", "dc-megablast")
        Dim d1 = QueryById(dc.Report, "nt1")
        If d1 IsNot Nothing Then DumpHits(d1)
        Check(d1 IsNot Nothing AndAlso FindHit(d1, "nt1_exact") IsNot Nothing, "dc-megablast 找回 nt1_exact")
        Check(d1 IsNot Nothing AndAlso FindHit(d1, "nt1_mut5") IsNot Nothing, "dc-megablast 找回 nt1_mut5")
        Check(d1 IsNot Nothing AndAlso FindHit(d1, "nt1_mut25") IsNot Nothing, "dc-megablast 找回 nt1_mut25")
        Check(d1 IsNot Nothing AndAlso FindHit(d1, "polya_lowcompl") Is Nothing,
              "dc-megablast 不召回 polya_lowcompl")

        ' ---------- blastn-short (W=7) ----------
        Dim bs = RunCase("blastn-short", "nt_query.fa", "nt_db.fa", "blastn", "blastn-short")
        Dim b2 = QueryById(bs.Report, "nt2_short")
        If b2 IsNot Nothing Then DumpHits(b2)
        Check(b2 IsNot Nothing AndAlso FindHit(b2, "nt1_exact") IsNot Nothing, "blastn-short 找回 nt1_exact")
        If b2 IsNot Nothing AndAlso FindHit(b2, "nt1_exact") IsNot Nothing Then
            Dim h = Best(FindHit(b2, "nt1_exact"))
            Check(h.SubjectFrom = 51 AndAlso h.SubjectTo = 90,
                  $"blastn-short subject 51..90（实际 {h.SubjectFrom}..{h.SubjectTo}）")
        End If
        Dim b1 = QueryById(bs.Report, "nt1")
        Check(b1 IsNot Nothing AndAlso FindHit(b1, "nt1_mut25") IsNot Nothing, "blastn-short 找回 nt1_mut25")
    End Sub

    ' ---------------------------------------------------------------- 蛋白

    Private Sub TestProteinSearch()
        ' ---------- blastp (W=3 / BLOSUM62 / T=11 / gap 11+1) ----------
        Dim bp = RunCase("blastp", "aa_query.fa", "aa_db.fa", "blastp", "blastp")
        Dim hba = QueryById(bp.Report, "hba_human")
        Check(hba IsNot Nothing, "hba_human 有查询结果")
        If hba IsNot Nothing Then
            DumpHits(hba)
            Check(hba.Hits.Count > 0, "hba_human 有命中")
            If hba.Hits.Count > 0 Then
                Check(hba.Hits(0).Id = "hba_exact", $"hba 首个命中 = hba_exact（实际 {hba.Hits(0).Id}）")

                Dim ex = FindHit(hba, "hba_exact")
                Check(ex IsNot Nothing, "hba 找回 hba_exact")
                If ex IsNot Nothing Then
                    Dim h = Best(ex)
                    Check(h.Identities = 142, $"hba_exact identities=142（实际 {h.Identities}）")
                    Check(h.Gaps = 0, $"hba_exact gaps=0（实际 {h.Gaps}）")
                    Check(h.QueryFrom = 1 AndAlso h.QueryTo = 142,
                          $"hba_exact query 1..142（实际 {h.QueryFrom}..{h.QueryTo}）")
                    Check(h.SubjectFrom = 1 AndAlso h.SubjectTo = 142,
                          $"hba_exact subject 1..142（实际 {h.SubjectFrom}..{h.SubjectTo}）")
                    Check(h.Evalue < 1.0E-50, $"hba_exact E={h.Evalue:E2} < 1e-50")
                End If

                Dim m12 = FindHit(hba, "hba_mut12")
                Check(m12 IsNot Nothing, "hba 找回 hba_mut12（12% 突变）")
                If m12 IsNot Nothing Then
                    Dim h = Best(m12)
                    Check(Pct(h.Identities, 142) >= 80.0,
                          $"hba_mut12 一致率 {Pct(h.Identities, 142):F1}% ≥ 80%")
                    Check(h.Positives > h.Identities,
                          $"hba_mut12 positives({h.Positives}) > identities({h.Identities})")
                End If

                ' 旁系同源：diag0 一致率仅 10.6%，必须靠 gapped 延伸才能对齐
                Dim hbb = FindHit(hba, "hbb_human")
                Check(hbb IsNot Nothing, "hba 找回 hbb_human（旁系同源，依赖 gapped 延伸）")
                If hbb IsNot Nothing Then
                    Dim h = Best(hbb)
                    Dim pc = Pct(h.Identities, hbb.Length)
                    Check(pc >= 20.0 AndAlso pc <= 60.0, $"hbb_human 一致率 {pc:F1}% ∈ [20%,60%]")
                    Check(h.Positives > h.Identities,
                          $"hbb_human positives({h.Positives}) > identities({h.Identities})（保守替换）")
                    Check(h.Evalue < 1.0E-5, $"hbb_human E={h.Evalue:E2} < 1e-5")
                End If

                ' 反例
                Check(FindHit(hba, "ubq_human") Is Nothing, "hba 不召回 ubq_human")
                Check(FindHit(hba, "lysc_human") Is Nothing, "hba 不召回 lysc_human")
                Check(FindHit(hba, "random180") Is Nothing, "hba 不召回 random180")
            End If
        End If

        ' ---------- blastp + comp-based-stats 1 ----------
        Dim cs = RunCase("blastp-comp-stats", "aa_query.fa", "aa_db.fa", "blastp", "blastp",
                         Sub(o) o.CompBasedStats = 1)
        Dim ch = QueryById(cs.Report, "hba_human")
        If ch IsNot Nothing Then DumpHits(ch)
        Check(ch IsNot Nothing AndAlso FindHit(ch, "hba_exact") IsNot Nothing,
              "comp-based-stats=1 下仍找回 hba_exact")
        Check(ch IsNot Nothing AndAlso FindHit(ch, "hba_mut12") IsNot Nothing,
              "comp-based-stats=1 下仍找回 hba_mut12")

        ' ---------- blastp-short (W=2 / BLOSUM80) ----------
        Dim ps = RunCase("blastp-short", "aa_query.fa", "aa_db.fa", "blastp", "blastp-short")
        Dim ph = QueryById(ps.Report, "hba_human")
        If ph IsNot Nothing Then DumpHits(ph)
        Check(ph IsNot Nothing AndAlso FindHit(ph, "hba_exact") IsNot Nothing,
              "blastp-short 找回 hba_exact（预设可用性冒烟）")
    End Sub

    ' ================================================================ 第4层

    Private Function Enc(isNt As Boolean, ch As Char) As Int32
        If isNt Then Return NtAlphabet.EncodeChar(ch)
        Return AaAlphabet.EncodeChar(ch)
    End Function

    ''' <summary>
    ''' HSP 结构不变量（8 条）：
    '''  1 三条串等长且非空
    '''  2/3 坐标跨度 == 非 gap 字符数
    '''  4  坐标在 [1, len] 内且递增
    '''  5  去 gap 后 == 源序列对应切片
    '''  6  midline 与 identities/positives/gaps 计数一致
    '''  7  按 scorer + gap 参数重算的 raw score == 报告 score
    '''  8  bit_score / evalue 满足 Karlin-Altschul 式 5-2 / 5-1
    ''' </summary>
    ''' <param name="checkStats">组成校正会改写 λ 且不导出，此时跳过第 8 条</param>
    Private Function ValidateHsp(h As Hsp, querySeq As String, subjectSeq As String,
                                 scorer As IScorer, isNt As Boolean,
                                 go As Double, ge As Double,
                                 lambda As Double, k As Double,
                                 m As Long, n As Long,
                                 checkStats As Boolean,
                                 ByRef reason As String) As Boolean
        Dim qa = h.QuerySeq, sa = h.SubjectSeq, mid = h.Midline

        ' 1
        If String.IsNullOrEmpty(qa) OrElse String.IsNullOrEmpty(sa) OrElse String.IsNullOrEmpty(mid) Then
            reason = "比对串为空" : Return False
        End If
        If qa.Length <> sa.Length OrElse qa.Length <> mid.Length Then
            reason = $"三条串长度不一致 q={qa.Length} s={sa.Length} mid={mid.Length}" : Return False
        End If

        ' 2/3
        Dim qCount = qa.Count(Function(ch) ch <> "-"c)
        Dim sCount = sa.Count(Function(ch) ch <> "-"c)
        If h.QueryTo - h.QueryFrom + 1 <> qCount Then
            reason = $"query 跨度 {h.QueryTo - h.QueryFrom + 1} ≠ 非 gap 字符数 {qCount}" : Return False
        End If
        If h.SubjectTo - h.SubjectFrom + 1 <> sCount Then
            reason = $"subject 跨度 {h.SubjectTo - h.SubjectFrom + 1} ≠ 非 gap 字符数 {sCount}" : Return False
        End If

        ' 4
        If h.QueryFrom < 1 OrElse h.QueryTo > querySeq.Length OrElse h.QueryTo < h.QueryFrom Then
            reason = $"query 坐标越界/倒序 {h.QueryFrom}..{h.QueryTo}（长度 {querySeq.Length}）" : Return False
        End If
        If h.SubjectFrom < 1 OrElse h.SubjectTo > subjectSeq.Length OrElse h.SubjectTo < h.SubjectFrom Then
            reason = $"subject 坐标越界/倒序 {h.SubjectFrom}..{h.SubjectTo}（长度 {subjectSeq.Length}）" : Return False
        End If

        ' 5
        Dim qSlice = querySeq.Substring(h.QueryFrom - 1, h.QueryTo - h.QueryFrom + 1)
        Dim sSlice = subjectSeq.Substring(h.SubjectFrom - 1, h.SubjectTo - h.SubjectFrom + 1)
        If qa.Replace("-", "") <> qSlice Then
            reason = "query 比对串去 gap ≠ 源序列切片" : Return False
        End If
        If sa.Replace("-", "") <> sSlice Then
            reason = "subject 比对串去 gap ≠ 源序列切片" : Return False
        End If

        ' 6 + 7
        Dim ids = 0, poss = 0, gaps = 0
        Dim raw As Double = 0
        Dim inQ = 0, inS = 0
        For i = 0 To qa.Length - 1
            Dim a = qa(i), b = sa(i), c = mid(i)
            If a <> "-"c AndAlso b <> "-"c Then
                If inQ > 0 Then raw -= go + inQ * ge : inQ = 0
                If inS > 0 Then raw -= go + inS * ge : inS = 0
                Dim sc = scorer.Score(Enc(isNt, a), Enc(isNt, b))
                raw += sc
                If a = b Then
                    ids += 1 : poss += 1
                    If c <> a Then reason = $"第 {i} 列恒同但 midline='{c}' ≠ '{a}'" : Return False
                Else
                    If sc > 0 Then poss += 1
                    Dim expect As Char = If(sc > 0, "+"c, " "c)
                    If c <> expect Then reason = $"第 {i} 列 midline='{c}'，期望 '{expect}'" : Return False
                End If
            ElseIf a <> "-"c Then
                gaps += 1
                If inS > 0 Then raw -= go + inS * ge : inS = 0
                inQ += 1
                If c <> " "c Then reason = $"第 {i} 列为 gap 但 midline='{c}'" : Return False
            ElseIf b <> "-"c Then
                gaps += 1
                If inQ > 0 Then raw -= go + inQ * ge : inQ = 0
                inS += 1
                If c <> " "c Then reason = $"第 {i} 列为 gap 但 midline='{c}'" : Return False
            Else
                reason = $"第 {i} 列两侧同时为 gap" : Return False
            End If
        Next
        If inQ > 0 Then raw -= go + inQ * ge
        If inS > 0 Then raw -= go + inS * ge

        If ids <> h.Identities Then reason = $"identities {h.Identities} ≠ 重算 {ids}" : Return False
        If poss <> h.Positives Then reason = $"positives {h.Positives} ≠ 重算 {poss}" : Return False
        If gaps <> h.Gaps Then reason = $"gaps {h.Gaps} ≠ 重算 {gaps}" : Return False

        ' 7
        If Math.Abs(raw - h.Score) > Math.Max(1.0E-6, Math.Abs(h.Score) * 1.0E-9) Then
            reason = $"重算 raw score {raw:F3} ≠ 报告 {h.Score:F3}" : Return False
        End If

        ' 8
        If checkStats Then
            Dim bit = (lambda * h.Score - Math.Log(k)) / Math.Log(2.0)
            If Math.Abs(bit - h.BitScore) > Math.Max(0.01, Math.Abs(bit) * 0.001) Then
                reason = $"bit_score {h.BitScore} ≠ (λS−lnK)/ln2 = {bit:F3}" : Return False
            End If
            Dim ev = k * CDbl(m) * CDbl(n) * Math.Exp(-lambda * h.Score)
            If Math.Abs(ev - h.Evalue) > Math.Max(1.0E-300, Math.Abs(ev) * 0.01) Then
                reason = $"evalue {h.Evalue:E3} ≠ K·m·n·e^(−λS) = {ev:E3}" : Return False
            End If
        End If
        Return True
    End Function

    Private Sub ValidateAllHsps(c As CaseResult)
        Dim isNt = (c.Opts.Program = "blastn")
        Dim scorer As IScorer
        If isNt Then
            scorer = New NtScorer(c.Opts.Reward, c.Opts.Penalty)
        Else
            scorer = New AaScorer(c.Opts.Matrix)
        End If
        Dim go = c.Opts.GapOpen, ge = c.Opts.GapExtend
        Dim checkStats = (c.Opts.CompBasedStats = 0)

        Dim total = 0, bad = 0
        For Each qr In c.Report.Queries
            Dim qf As FastaSeq = Nothing
            If Not c.QuerySeqs.TryGetValue(qr.Id, qf) Then
                bad += 1
                Console.WriteLine($"     [不变量失败] 查询 {qr.Id} 不在 FASTA 中")
                Continue For
            End If
            Dim qseq = qf.SequenceData
            If qr.Hits IsNot Nothing Then
                For Each hit In qr.Hits
                    Dim sf As FastaSeq = Nothing
                    If Not c.DbSeqs.TryGetValue(hit.Id, sf) Then
                        bad += 1
                        Console.WriteLine($"     [不变量失败] 命中 {hit.Id} 不在 FASTA 中")
                        Continue For
                    End If
                    Dim sseq = sf.SequenceData
                    For Each h In hit.Hsps
                        total += 1
                        Dim reason As String = Nothing
                        If Not ValidateHsp(h, qseq, sseq, scorer, isNt, go, ge,
                                           c.Ka.Lambda, c.Ka.K, qr.Length, hit.Length,
                                           checkStats, reason) Then
                            bad += 1
                            If bad <= 3 Then
                                Console.WriteLine($"     [不变量失败] {qr.Id} vs {hit.Id}: {reason}")
                            End If
                        End If
                    Next
                Next
            End If
        Next
        Check(bad = 0, $"{c.Name}: {total} 条 HSP 全部通过结构不变量（{bad} 条失败）")
    End Sub

    ' ---------------------------------------------------------------- JSON 往返

    Private Sub TestReportRoundTrip()
        ' 局部变量不要叫 path：VB 大小写不敏感，会遮蔽 System.IO.Path
        Dim reportFile = Path.Combine(OutDir, "blastp.json")
        If Not File.Exists(reportFile) Then
            Check(False, $"导出文件不存在: {reportFile}")
            Return
        End If
        Check(New FileInfo(reportFile).Length > 0,
              $"导出文件非空（{New FileInfo(reportFile).Length} 字节）")

        Dim back = BlastReportJson.Load(reportFile)
        Check(back IsNot Nothing, "JSON 可反序列化回 BlastReport")
        If back Is Nothing Then Return

        Check(back.Program = "blastp", $"回读 program = blastp（实际 {back.Program}）")
        Check(back.Task = "blastp", $"回读 task = blastp（实际 {back.Task}）")
        Check(back.Parameters IsNot Nothing, "回读 parameters 非空")
        If back.Parameters IsNot Nothing Then
            Check(back.Parameters.DbSequences = 6, $"回读 db_sequences = 6（实际 {back.Parameters.DbSequences}）")
            Check(back.Parameters.Matrix = "BLOSUM62", $"回读 matrix = BLOSUM62（实际 {back.Parameters.Matrix}）")
            Check(back.Parameters.WordSize = 3, $"回读 word_size = 3（实际 {back.Parameters.WordSize}）")
            Check(back.Parameters.GapOpen = 11.0 AndAlso back.Parameters.GapExtend = 1.0,
                  $"回读 gap = 11/1（实际 {back.Parameters.GapOpen}/{back.Parameters.GapExtend}）")
        End If
        Check(back.Queries IsNot Nothing AndAlso back.Queries.Count = 1,
              $"回读 queries 数 = 1（实际 {If(back.Queries Is Nothing, -1, back.Queries.Count)}）")

        If back.Queries IsNot Nothing AndAlso back.Queries.Count > 0 Then
            Dim q = back.Queries(0)
            Check(q.Id = "hba_human", $"回读 query id = hba_human（实际 {q.Id}）")
            Check(q.Hits IsNot Nothing AndAlso q.Hits.Count > 0, "回读 hits 非空")
            If q.Hits IsNot Nothing AndAlso q.Hits.Count > 0 Then
                Dim h = q.Hits(0)
                Check(h.Id = "hba_exact", $"回读首个 hit = hba_exact（实际 {h.Id}）")
                Dim hsp = h.Hsps(0)
                Check(hsp.Identities = 142, $"回读 identities = 142（实际 {hsp.Identities}）")
                Check(hsp.QuerySeq IsNot Nothing AndAlso hsp.SubjectSeq IsNot Nothing AndAlso
                      hsp.Midline IsNot Nothing, "回读比对三串非空")
                Check(hsp.QuerySeq.Length = hsp.SubjectSeq.Length AndAlso
                      hsp.QuerySeq.Length = hsp.Midline.Length, "回读比对三串等长")
            End If
        End If

        ' 所有导出文件都能回读
        Dim allFiles = Directory.GetFiles(OutDir, "*.json")
        Dim unreadable = 0
        For Each f In allFiles
            Try
                If BlastReportJson.Load(f) Is Nothing Then unreadable += 1
            Catch ex As Exception
                unreadable += 1
            End Try
        Next
        Check(unreadable = 0, $"{allFiles.Length} 个导出 JSON 全部可回读（{unreadable} 个失败）")
        Console.WriteLine($"     导出清单: {String.Join(", ", allFiles.Select(Function(f) Path.GetFileName(f)))}")
    End Sub

    ' ---------------------------------------------------------------- 随机工具

    Private Function RandomAa(rng As Random, len As Integer) As Int32()
        Dim a(len - 1) As Int32
        For i = 0 To len - 1
            a(i) = rng.Next(20)
        Next
        Return a
    End Function

    Private Function RandomNt(rng As Random, len As Integer) As Int32()
        Dim a(len - 1) As Int32
        For i = 0 To len - 1
            a(i) = rng.Next(4)
        Next
        Return a
    End Function

    Private Function MutateAa(rng As Random, src As Int32(), rate As Double) As Int32()
        Dim dst = CType(src.Clone(), Int32())
        For i = 0 To dst.Length - 1
            If rng.NextDouble() < rate Then dst(i) = rng.Next(20)
        Next
        Return dst
    End Function

    Private Function MutateNtCodes(rng As Random, src As Int32(), rate As Double) As Int32()
        Dim dst = CType(src.Clone(), Int32())
        For i = 0 To dst.Length - 1
            If rng.NextDouble() < rate Then dst(i) = rng.Next(4)
        Next
        Return dst
    End Function

End Module
