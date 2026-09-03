' ============================================================================
' Program.vb — EmMotif 命令行入口
' ----------------------------------------------------------------------------
' 用法：
'   EmMotif discover --input seqs.fa [--alphabet dna|protein|auto] [--model zoops]
'       [--minw 8 --maxw 8] [--nmotifs 1] [--revcomp]
'       [--seed-strategy enriched|random|all] [--seed-count 20]
'       [--pseudocount 0.1] [--max-iter 200] [--epsilon 0.0001]
'       [--evalue-max 10] [--rng-seed 0] [--out motifs.json] [--pretty]
'   EmMotif selftest
' ============================================================================

Imports System.Globalization
Imports System.IO
Imports System.Text.Json
Imports System.Text.Json.Serialization
Imports EmMotif.EmMotif.Core
Imports EmMotif.EmMotif.Model

Namespace EmMotif

    Public Module Program

        Private Const VersionString As String = "1.0.0"

        Public Function Main(args As String()) As Integer
            If args.Length = 0 OrElse args(0) = "--help" OrElse args(0) = "-h" Then
                PrintUsage()
                Return 0
            End If
            Dim cmd = args(0).ToLowerInvariant()
            If cmd = "selftest" Then Return SelfTest.RunAll()

            Try
                If cmd = "discover" Then
                    Return RunDiscover(args)
                Else
                    Console.Error.WriteLine($"未知子命令: {cmd}")
                    Return 2
                End If
            Catch ex As Exception
                Console.Error.WriteLine($"错误: {ex.Message}")
                Return 1
            End Try
        End Function

        Private Function FlagValue(args As String(), name As String) As String
            For i = 0 To args.Length - 2
                If args(i).ToLowerInvariant() = name Then Return args(i + 1)
            Next
            Return Nothing
        End Function

        Private Function HasFlag(args As String(), name As String) As Boolean
            For i = 0 To args.Length - 1
                If args(i).ToLowerInvariant() = name Then Return True
            Next
            Return False
        End Function

        Private Function IntArg(args As String(), name As String, defVal As Int32) As Int32
            Dim v = FlagValue(args, name)
            If v Is Nothing Then Return defVal
            Return Integer.Parse(v, CultureInfo.InvariantCulture)
        End Function

        Private Function DblArg(args As String(), name As String, defVal As Double) As Double
            Dim v = FlagValue(args, name)
            If v Is Nothing Then Return defVal
            Return Double.Parse(v, CultureInfo.InvariantCulture)
        End Function

        Private Function StrArg(args As String(), name As String, defVal As String) As String
            Dim v = FlagValue(args, name)
            If v Is Nothing Then Return defVal
            Return v.ToLowerInvariant()
        End Function

        Private Function RunDiscover(args As String()) As Integer
            Dim inPath = FlagValue(args, "--input")
            If inPath Is Nothing Then
                Console.Error.WriteLine("必须提供 --input <fasta>")
                Return 2
            End If
            Dim outPath = FlagValue(args, "--out")

            Dim records = FastaIO.Read(inPath)

            ' 字母表
            Dim alpha As Alphabet
            Dim alphaStr = StrArg(args, "--alphabet", "auto")
            If alphaStr = "dna" Then
                alpha = New Alphabet(AlphabetKind.Dna)
            ElseIf alphaStr = "protein" Then
                alpha = New Alphabet(AlphabetKind.Protein)
            Else
                Dim longest = records(0).Seq
                For Each r In records
                    If r.Seq.Length > longest.Length Then longest = r.Seq
                Next
                alpha = New Alphabet(Alphabet.Detect(longest))
            End If

            ' 模型
            Dim modelStr = StrArg(args, "--model", "zoops")
            Dim model As SiteModel
            Select Case modelStr
                Case "oops" : model = SiteModel.Oops
                Case "zoops" : model = SiteModel.Zoops
                Case "anr" : model = SiteModel.Anr
                Case Else
                    Console.Error.WriteLine($"未知模型: {modelStr}（可选 oops|zoops|anr）")
                    Return 2
            End Select

            Dim opts As New SearchOptions With {
                .Model = model,
                .MinW = IntArg(args, "--minw", 8),
                .MaxW = IntArg(args, "--maxw", 8),
                .NumMotifs = IntArg(args, "--nmotifs", 1),
                .Revcomp = HasFlag(args, "--revcomp"),
                .SeedStrategy = StrArg(args, "--seed-strategy", "enriched"),
                .SeedCount = IntArg(args, "--seed-count", 20),
                .Pseudocount = DblArg(args, "--pseudocount", 0.1),
                .MaxIter = IntArg(args, "--max-iter", 200),
                .Epsilon = DblArg(args, "--epsilon", 0.0001),
                .EvalueMax = DblArg(args, "--evalue-max", 10.0),
                .RngSeed = IntArg(args, "--rng-seed", 0)}
            If opts.MinW < 2 OrElse opts.MaxW < opts.MinW Then
                Console.Error.WriteLine("宽度范围无效（需 2 ≤ minw ≤ maxw）")
                Return 2
            End If
            If opts.Revcomp AndAlso Not alpha.SupportsRevcomp Then
                Console.Error.WriteLine("氨基酸字母表不支持 --revcomp，已忽略")
                opts.Revcomp = False
            End If

            ' 编码
            Dim encList As New List(Of Int32())()
            Dim seqSummaries As New List(Of SeqSummary)()
            For Each r In records
                encList.Add(alpha.Encode(r.Seq))
                Dim amb As Int32 = 0
                For Each a In encList(encList.Count - 1)
                    If a < 0 Then amb += 1
                Next
                seqSummaries.Add(New SeqSummary With {
                    .Id = r.Id, .Length = r.Seq.Length, .AmbiguousPositions = amb})
            Next

            Console.Error.WriteLine($"EmMotif {VersionString}  字母表={If(alpha.Kind = AlphabetKind.Dna, "dna", "protein")}  " &
                                    $"模型={modelStr}  序列数={records.Count}  W=[{opts.MinW},{opts.MaxW}]")

            ' 背景频率（结果 JSON 也用）
            Dim search As New EmSearch(encList, alpha, opts)
            Dim sw = System.Diagnostics.Stopwatch.StartNew()
            Dim motifs = search.Discover()
            sw.Stop()
            Console.Error.WriteLine($"发现 {motifs.Count} 个 motif（{sw.Elapsed.TotalSeconds:F1}s）")

            ' 组装报告
            Dim bgDict As New Dictionary(Of String, Double)()
            If motifs.Count > 0 Then
                ' 背景：重算一次（与 EmSearch 内部一致）
                Dim cnt(alpha.Size - 1) As Double
                Dim total As Double = 0
                For Each enc In encList
                    For Each a In enc
                        If a >= 0 Then
                            cnt(a) += 1.0
                            total += 1.0
                        End If
                    Next
                Next
                For a = 0 To alpha.Size - 1
                    bgDict(alpha.Letters(a).ToString()) = Math.Round((cnt(a) + 0.1) / (total + 0.1 * alpha.Size), 6)
                Next
            End If

            Dim motifDtos As New List(Of MotifDto)()
            For mi = 0 To motifs.Count - 1
                Dim r = motifs(mi)
                Dim dto As New MotifDto With {
                    .Id = $"motif_{mi + 1}",
                    .Width = r.Width,
                    .Model = modelStr,
                    .Consensus = r.Consensus,
                    .Lambda = Math.Round(r.Lambda, 6),
                    .LogLikelihood = Math.Round(r.LogLikelihood, 4),
                    .LogLikelihoodRatio = Math.Round(r.LogLikelihoodRatio, 4),
                    .Evalue = r.Evalue,
                    .Iterations = r.Iterations,
                    .Converged = r.Converged,
                    .Letters = alpha.Letters,
                    .LogLikTrace = r.LogLikTrace.Select(Function(v) Math.Round(v, 4)).ToList()}
                ' PWM：按字母名键
                Dim pwmDict As New Dictionary(Of String, Double())()
                For a = 0 To alpha.Size - 1
                    Dim arr(r.Width - 1) As Double
                    For k = 0 To r.Width - 1
                        arr(k) = Math.Round(r.Pwm(k, a), 5)
                    Next
                    pwmDict(alpha.Letters(a).ToString()) = arr
                Next
                dto.Pwm = pwmDict
                Dim bgM As New Dictionary(Of String, Double)()
                For a = 0 To alpha.Size - 1
                    bgM(alpha.Letters(a).ToString()) = Math.Round((CntLetter(encList, a) + 0.1) /
                        (TotalLetters(encList) + 0.1 * alpha.Size), 6)
                Next
                dto.Background = bgM
                ' 位点
                Dim siteDtos As New List(Of SiteDto)()
                For idx = 0 To r.Sites.Count - 1
                    Dim sp = r.Sites(idx)
                    Dim si = r.SiteSeqIndex(idx)
                    Dim rec = records(si)
                    Dim seg As String
                    If sp.StrandMinus Then
                        ' 负链位点段：原串 [j, j+W) 的反向互补
                        seg = alpha.Revcomp(rec.Seq.Substring(sp.Pos, r.Width))
                    Else
                        seg = rec.Seq.Substring(sp.Pos, r.Width).ToUpperInvariant()
                    End If
                    siteDtos.Add(New SiteDto With {
                        .Sequence = rec.Id,
                        .Start = sp.Pos + 1,               ' 1-based
                        .Strand = If(sp.StrandMinus, "-", "+"),
                        .Posterior = Math.Round(sp.Z, 5),
                        .WindowLogR = If(Double.IsNegativeInfinity(sp.LogR), -9999, Math.Round(sp.LogR, 4)),
                        .Segment = seg})
                Next
                siteDtos.Sort(Function(a, b)
                                  Dim c = a.Posterior.CompareTo(b.Posterior)
                                  If c <> 0 Then Return -c
                                  Dim c2 = String.CompareOrdinal(a.Sequence, b.Sequence)
                                  If c2 <> 0 Then Return c2
                                  Return a.Start.CompareTo(b.Start)
                              End Function)
                dto.Sites = siteDtos
                motifDtos.Add(dto)
            Next

            Dim report As New MotifReport With {
                .Program = "EmMotif",
                .Version = VersionString,
                .Alphabet = If(alpha.Kind = AlphabetKind.Dna, "dna", "protein"),
                .Parameters = New MotifParameters With {
                    .Model = modelStr,
                    .MinWidth = opts.MinW,
                    .MaxWidth = opts.MaxW,
                    .NumMotifs = opts.NumMotifs,
                    .Revcomp = opts.Revcomp,
                    .SeedStrategy = opts.SeedStrategy,
                    .SeedCount = opts.SeedCount,
                    .Pseudocount = opts.Pseudocount,
                    .MaxIterations = opts.MaxIter,
                    .Epsilon = opts.Epsilon,
                    .EvalueMax = opts.EvalueMax,
                    .RngSeed = opts.RngSeed,
                    .NumSequences = records.Count},
                .Sequences = seqSummaries,
                .BackgroundFrequencies = bgDict,
                .Motifs = motifDtos}

            Dim jsonOpts As New JsonSerializerOptions With {
                .WriteIndented = HasFlag(args, "--pretty"),
                .DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull}
            Dim json = JsonSerializer.Serialize(report, jsonOpts)
            If outPath IsNot Nothing Then
                File.WriteAllText(outPath, json)
                Console.Error.WriteLine($"结果已写入 {outPath}")
            Else
                Console.Out.WriteLine(json)
            End If
            Return 0
        End Function

        Private Function CntLetter(encList As List(Of Int32()), a As Int32) As Double
            Dim c As Double = 0
            For Each enc In encList
                For Each x In enc
                    If x = a Then c += 1.0
                Next
            Next
            Return c
        End Function

        Private Function TotalLetters(encList As List(Of Int32())) As Double
            Dim c As Double = 0
            For Each enc In encList
                For Each x In enc
                    If x >= 0 Then c += 1.0
                Next
            Next
            Return c
        End Function

        Private Sub PrintUsage()
            Console.WriteLine("EmMotif — EM 算法 motif 发现（MEME 三种位点分布模型；核酸+氨基酸；纯 BCL）")
            Console.WriteLine()
            Console.WriteLine("用法:")
            Console.WriteLine("  EmMotif discover --input seqs.fa [--alphabet dna|protein|auto] [--model zoops]")
            Console.WriteLine("      [--minw 8 --maxw 8] [--nmotifs 1] [--revcomp]")
            Console.WriteLine("      [--seed-strategy enriched|random|all] [--seed-count 20]")
            Console.WriteLine("      [--pseudocount 0.1] [--max-iter 200] [--epsilon 0.0001]")
            Console.WriteLine("      [--evalue-max 10] [--rng-seed 0] [--out motifs.json] [--pretty]")
            Console.WriteLine("  EmMotif selftest")
            Console.WriteLine()
            Console.WriteLine("模型: oops（每序列恰 1 位点）| zoops（每序列 ≤1，推荐默认）| anr（任意多）")
        End Sub

    End Module

End Namespace
