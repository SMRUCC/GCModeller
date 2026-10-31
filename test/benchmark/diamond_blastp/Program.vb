' DIAMOND blastp 并行搜索基准测试 (Console Benchmark)
'
' 用途:
'   1) 量化"查询集 vs 参考库"批量比对的总耗时与阶段耗时(索引 / 扫描 / 局部比对);
'   2) 校验结果的可复现性(同一输入重复运行,结果集合必须完全一致 —— 用于确认
'      并行改造之后不再存在跨查询共享状态导致的数据竞争);
'   3) 可选地对照"带内 SW"与历史 GSW 全窗口实现的得分一致性。
'
' 测试结果统一通过 DebugAudit(ApplicationServices.Debugging)记录,
' 结束时输出文本表格并保存 JSON 报告。

Imports System.Diagnostics
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports SMRUCC.genomics.Analysis.SequenceAlignment.DIAMOND
Imports SMRUCC.genomics.SequenceModel.FASTA

Module Program

    Private Const DefaultQuery As String = "F:\ecoli\29.0\data\protseq.fsa"
    Private Const DefaultSubject As String = "G:\cell-render\data\KO.fasta"

    Function Main(args As String()) As Integer
        Dim queryPath As String = DefaultQuery
        Dim subjectPath As String = DefaultSubject
        Dim maxHits As Integer = 0
        Dim repeat As Integer = 1
        Dim compareGsw As Boolean = False
        Dim report As String = IO.Path.Combine(Environment.CurrentDirectory, "diamond_bench_report.json")
        Dim positional As New List(Of String)

        For i As Integer = 0 To args.Length - 1
            Select Case LCase(args(i))
                Case "--query"
                    i += 1 : queryPath = args(i)
                Case "--subject"
                    i += 1 : subjectPath = args(i)
                Case "--max-hits"
                    i += 1 : maxHits = CInt(args(i))
                Case "--repeat"
                    i += 1 : repeat = Math.Max(1, CInt(args(i)))
                Case "--compare-gsw"
                    compareGsw = True
                Case "--report"
                    i += 1 : report = args(i)
                Case Else
                    Call positional.Add(args(i))
            End Select
        Next

        If positional.Count >= 1 Then
            queryPath = positional(0)
        End If

        If positional.Count >= 2 Then
            subjectPath = positional(1)
        End If

        Call DebugAudit.Mark("query_file", queryPath)
        Call DebugAudit.Mark("subject_file", subjectPath)

        Dim clock As Stopwatch = Stopwatch.StartNew()
        Dim queries As FastaSeq() = FastaFile.Read(queryPath).ToArray
        Dim subjects As FastaSeq() = FastaFile.Read(subjectPath).ToArray

        clock.Stop()
        Call DebugAudit.Mark("load_ms", Math.Round(clock.Elapsed.TotalMilliseconds, 1))
        Call DebugAudit.Mark("queries", queries.Length)
        Call DebugAudit.Mark("subjects", subjects.Length)

        If queries.Length = 0 OrElse subjects.Length = 0 Then
            Call Console.Error.WriteLine("empty input")
            Return 1
        End If

        Dim diamond As New DiamondBlastp(SensitivityMode.Fast)
        Dim options As DiamondSearchOptions = DiamondSearchOptions.CreateDefault()
        options.ProgressLogInterval = 0

        Dim signatures As New List(Of String)
        Dim elapsed As New List(Of Double)
        Dim lastProfile As DiamondSearchProfile = Nothing

        For round As Integer = 1 To repeat
            clock.Restart()

            Dim hits As DiamondHit() = diamond.Search(queries, subjects, options, Nothing, maxHits).ToArray

            clock.Stop()
            lastProfile = diamond.LastSearchProfile
            Call elapsed.Add(clock.Elapsed.TotalMilliseconds)
            Call signatures.Add(Signature(hits))

            Call DebugAudit.Mark($"round{round}_ms", Math.Round(clock.Elapsed.TotalMilliseconds, 1))
            Call DebugAudit.Mark($"round{round}_hits", hits.Length)
            Call Console.WriteLine($"round {round}: {hits.Length} hits in {clock.Elapsed.TotalMilliseconds:F0} ms")
        Next

        Call DebugAudit.Mark("profile", lastProfile?.ToString())

        If lastProfile IsNot Nothing Then
            Call DebugAudit.Mark("index_ms", Math.Round(lastProfile.IndexMilliseconds, 1))
            Call DebugAudit.Mark("scan_ms", Math.Round(lastProfile.ScanMilliseconds, 1))
            Call DebugAudit.Mark("align_ms", Math.Round(lastProfile.AlignMilliseconds, 1))
            Call DebugAudit.Mark("candidates", lastProfile.Candidates)
            Call DebugAudit.Mark("residues", lastProfile.Residues)
        End If

        ' 可复现性:所有轮次的结果集合签名必须完全一致
        Dim stable As Boolean = True

        For i As Integer = 1 To signatures.Count - 1
            If Not String.Equals(signatures(0), signatures(i), StringComparison.Ordinal) Then
                stable = False
            End If
        Next

        Call DebugAudit.Mark("result_signature", signatures(0).Substring(0, Math.Min(32, signatures(0).Length)))
        Call DebugAudit.Test(stable, "repeated runs must produce identical result sets")

        If compareGsw Then
            ' 抽样校验:历史 GSW 实现是 O(m·n) 的全窗口动态规划,在百万级参考库上
            ' 全量对照会耗时数十分钟;这里固定抽取前 200 条查询做一致性抽样。
            Dim sampleQueries As FastaSeq() = queries.Take(Math.Min(200, queries.Length)).ToArray
            Dim gswOptions As DiamondSearchOptions = DiamondSearchOptions.CreateDefault()
            Dim reference As New DiamondBlastp(SensitivityMode.Fast)
            Dim hitsBanded As DiamondHit() = diamond.Search(sampleQueries, subjects, options, Nothing, maxHits).ToArray

            clock.Restart()
            ' 对照:EnableBandSw = False 等价于回退到历史 GSW 全窗口实现
            gswOptions.EnableBandSw = False
            Dim hitsGsw As DiamondHit() = reference.Search(sampleQueries, subjects, gswOptions, Nothing, maxHits).ToArray
            clock.Stop()

            Dim diff As Integer = CompareScores(hitsBanded, hitsGsw)
            Dim ratio As Double = If(hitsBanded.Length = 0, 0, 100.0 * diff / hitsBanded.Length)

            Call DebugAudit.Mark("gsw_sample_queries", sampleQueries.Length)
            Call DebugAudit.Mark("gsw_hits", hitsGsw.Length)
            Call DebugAudit.Mark("gsw_ms", Math.Round(clock.Elapsed.TotalMilliseconds, 1))
            Call DebugAudit.Mark("score_mismatch", diff)
            Call DebugAudit.Mark("score_mismatch_pct", Math.Round(ratio, 3))
            Call DebugAudit.Test(ratio < 1.0, "banded SW score should match GSW for >99% of hits")
        End If

        Dim process As Process = Process.GetCurrentProcess()
        Call DebugAudit.Mark("peak_working_set_mb", Math.Round(process.PeakWorkingSet64 / 1024.0 / 1024.0, 1))
        Call DebugAudit.Mark("gc_heap_mb", Math.Round(GC.GetTotalMemory(False) / 1024.0 / 1024.0, 1))
        Call DebugAudit.Mark("failures", DebugAudit.FailureCount)

        Call Console.WriteLine()
        Call Console.WriteLine(DebugAudit.Dump())
        Call DebugAudit.SaveReport(report)
        Call Console.WriteLine($"report saved: {report}")

        Return If(DebugAudit.FailureCount = 0, 0, 2)
    End Function

    ''' <summary>
    ''' 结果集合签名:与并行调度顺序无关(排序后再拼接),用于判断多次运行是否等价。
    ''' </summary>
    Private Function Signature(hits As DiamondHit()) As String
        Dim lines As String() = hits _
            .Select(Function(h) $"{h.QseqId}|{h.SseqId}|{h.RawScore:F4}|{h.QStart}|{h.QEnd}|{h.SStart}|{h.SEnd}") _
            .OrderBy(Function(s) s, StringComparer.Ordinal) _
            .ToArray

        ' 使用"异或 + 循环移位"的滚动哈希:只涉及移位与异或,
        ' 不触发 VB.NET 默认开启的整数溢出检查(避免基准程序本身抛异常)
        Dim hash As ULong = 1469598103934665603UL

        For Each line As String In lines
            For Each ch As Char In line
                hash = hash Xor CULng(AscW(ch))
                hash = (hash << 7) Or (hash >> 57)
                hash = hash Xor (hash >> 23)
            Next
        Next

        Return $"{lines.Length}:{hash:X16}"
    End Function

    Private Function CompareScores(a As DiamondHit(), b As DiamondHit()) As Integer
        Dim left As New Dictionary(Of String, Double)(StringComparer.Ordinal)

        For Each hit As DiamondHit In a
            left($"{hit.QseqId}|{hit.SseqId}") = hit.RawScore
        Next

        Dim diff As Integer = 0

        For Each hit As DiamondHit In b
            Dim key As String = $"{hit.QseqId}|{hit.SseqId}"
            Dim score As Double = 0

            If Not left.TryGetValue(key, score) Then
                diff += 1
            ElseIf Math.Abs(score - hit.RawScore) > 0.001 Then
                diff += 1
            End If
        Next

        Return diff
    End Function
End Module
