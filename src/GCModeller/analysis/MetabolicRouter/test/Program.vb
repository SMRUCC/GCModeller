' ============================================================================
' Program.vb — RetroPath 命令行入口
' ----------------------------------------------------------------------------
' 用法：
'   RetroPath search --target "SMILES" [--sink sink.tsv] [--rules rules.tsv]
'       [--strategy beam|dfs] [--beam-width 50] [--max-depth 6] [--max-paths 20]
'       [--w-thermo 0.4 --w-enzyme 0.3 --w-length 0.3] [--out paths.json] [--pretty]
'   RetroPath enumerate-rules [--out rules.json]
'   RetroPath selftest
'
' 汇 TSV：name <TAB> SMILES（底盘内源代谢物集合，可从 GEM 模型提取 [readme.md §一]）。
' 未提供 --sink 时使用内置 E. coli 核心代谢物演示集合。
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Text.Json
Imports System.Text.Json.Serialization
Imports RetroPath.Chem
Imports RetroPath.Model
Imports RetroPath.Search

Namespace RetroPath

    Public Module Program

        Private Const VersionString As String = "1.0.0"

        ''' <summary>内置 E. coli 核心汇集合（演示用；生产请从 GEM 提取）</summary>
        Private Function BuiltinSink() As List(Of Tuple(Of String, String))
            Dim s As New List(Of Tuple(Of String, String))()
            s.Add(Tuple.Create("pyruvate", "CC(=O)C(=O)O"))
            s.Add(Tuple.Create("oxaloacetate", "OC(=O)C(=O)CC(=O)O"))
            s.Add(Tuple.Create("acetate", "CC(=O)O"))
            s.Add(Tuple.Create("acetaldehyde", "CC=O"))
            s.Add(Tuple.Create("glycine", "NCC(=O)O"))
            s.Add(Tuple.Create("glyoxylate", "OC(=O)C=O"))
            s.Add(Tuple.Create("oxalate", "OC(=O)C(=O)O"))
            s.Add(Tuple.Create("akg", "OC(=O)CCC(=O)C(=O)O"))
            s.Add(Tuple.Create("succinate", "OC(=O)CCC(=O)O"))
            s.Add(Tuple.Create("fumarate", "OC(=O)C=CC(=O)O"))
            s.Add(Tuple.Create("malate", "OC(=O)CC(O)C(=O)O"))
            s.Add(Tuple.Create("ethanol", "CCO"))
            s.Add(Tuple.Create("glycerol", "OCC(O)CO"))
            s.Add(Tuple.Create("co2", "O=C=O"))
            s.Add(Tuple.Create("h2o", "O"))
            s.Add(Tuple.Create("nh3", "N"))
            Return s
        End Function

        Public Function Main(args As String()) As Integer
            If args.Length = 0 OrElse args(0) = "--help" OrElse args(0) = "-h" Then
                PrintUsage()
                Return 0
            End If
            Dim cmd = args(0).ToLowerInvariant()
            If cmd = "selftest" Then Return SelfTest.RunAll()
            Try
                If cmd = "search" Then
                    Return RunSearch(args)
                ElseIf cmd = "enumerate-rules" Then
                    Return RunEnumerate(args)
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

        Private Function DblArg(args As String(), name As String, defVal As Double) As Double
            Dim v = FlagValue(args, name)
            If v Is Nothing Then Return defVal
            Return Double.Parse(v, CultureInfo.InvariantCulture)
        End Function

        Private Function IntArg(args As String(), name As String, defVal As Int32) As Int32
            Dim v = FlagValue(args, name)
            If v Is Nothing Then Return defVal
            Return Int32.Parse(v, CultureInfo.InvariantCulture)
        End Function

        Private Function RunEnumerate(args As String()) As Integer
            Dim outPath = FlagValue(args, "--out")
            Dim dtos = RuleLibrary.BuiltinRules().Select(Function(r) New RuleDto With {
                .Id = r.Id, .Name = r.Name,
                .ReactantSmarts = r.ReactantText,
                .ProductSmarts = r.ProductText,
                .DeltaG = r.DeltaG, .EnzymeTier = r.EnzymeTier, .Reversible = r.Reversible}).ToList()
            Dim jsonOpts As New JsonSerializerOptions With {.WriteIndented = HasFlag(args, "--pretty")}
            Dim json = JsonSerializer.Serialize(dtos, jsonOpts)
            If outPath IsNot Nothing Then
                File.WriteAllText(outPath, json)
                Console.Error.WriteLine($"规则库已写入 {outPath}")
            Else
                Console.Out.WriteLine(json)
            End If
            Return 0
        End Function

        Private Function RunSearch(args As String()) As Integer
            Dim targetSmiles = FlagValue(args, "--target")
            If targetSmiles Is Nothing Then
                Console.Error.WriteLine("必须提供 --target ""SMILES""")
                Return 2
            End If
            Dim outPath = FlagValue(args, "--out")

            ' 规则库
            Dim rules = RuleLibrary.BuiltinRules()
            Dim rulesPath = FlagValue(args, "--rules")
            If rulesPath IsNot Nothing Then
                rules.AddRange(RuleLibrary.LoadRulesTsv(rulesPath))
            End If

            ' 汇集合
            Dim sink As List(Of Tuple(Of String, String))
            Dim sinkPath = FlagValue(args, "--sink")
            If sinkPath IsNot Nothing Then
                sink = New List(Of Tuple(Of String, String))()
                For Each raw In File.ReadLines(sinkPath)
                    Dim line = raw.TrimEnd(Convert.ToChar(13), Convert.ToChar(10))
                    If line.Length = 0 OrElse line.StartsWith("#"c) Then Continue For
                    Dim cols = line.Split(ControlChars.Tab)
                    If cols.Length >= 2 Then
                        sink.Add(Tuple.Create(cols(0).Trim(), cols(1).Trim()))
                    End If
                Next
            Else
                sink = BuiltinSink()
            End If

            ' 选项
            Dim opts As New SearchOptions With {
                .Strategy = If(FlagValue(args, "--strategy"), "beam").ToLowerInvariant(),
                .BeamWidth = IntArg(args, "--beam-width", 50),
                .MaxDepth = IntArg(args, "--max-depth", 6),
                .MaxPaths = IntArg(args, "--max-paths", 20)}
            Dim w As New ScoreWeights With {
                .Thermo = DblArg(args, "--w-thermo", 0.4),
                .Enzyme = DblArg(args, "--w-enzyme", 0.3),
                .Length = DblArg(args, "--w-length", 0.3)}

            ' 解析目标与汇
            Dim target = SmilesIO.Parse(targetSmiles)
            Dim sinkKeys As New HashSet(Of String)()
            For Each s In sink
                sinkKeys.Add(SmilesIO.Parse(s.Item2).MolKey())
            Next
            Dim currencyKeys As New HashSet(Of String)()
            For Each cs In RuleLibrary.CurrencySmiles()
                currencyKeys.Add(SmilesIO.Parse(cs).MolKey())
            Next

            Console.Error.WriteLine($"RetroPath {VersionString}: 目标 = {targetSmiles}  " &
                                    $"规则 = {rules.Count}  汇 = {sinkKeys.Count}  策略 = {opts.Strategy}")

            ' 搜索
            Dim sw = System.Diagnostics.Stopwatch.StartNew()
            Dim searcher As New BeamSearch(rules, sinkKeys, currencyKeys, opts)
            Dim completed = searcher.Search(target)
            sw.Stop()
            searcher.Stats.ElapsedMs = CLng(sw.Elapsed.TotalMilliseconds)

            ' 评分与排序
            Dim scored = completed.
                Select(Function(st) Tuple.Create(st, Scoring.ScorePath(st, w))).
                OrderByDescending(Function(t) t.Item2.GlobalScore).
                ThenBy(Function(t) t.Item2.NumSteps).
                Take(opts.MaxPaths).ToList()

            ' 报告
            Dim pathDtos As New List(Of PathDto)()
            For pi = 0 To scored.Count - 1
                Dim st = scored(pi).Item1
                Dim ps = scored(pi).Item2
                Dim fwdSteps = Scoring.AssembleForward(st)
                pathDtos.Add(New PathDto With {
                    .Id = $"path_{pi + 1}",
                    .GlobalScore = Math.Round(ps.GlobalScore, 5),
                    .ThermoScore = Math.Round(ps.ThermoScore, 5),
                    .EnzymeScore = Math.Round(ps.EnzymeScore, 5),
                    .LengthScore = Math.Round(ps.LengthScore, 5),
                    .DeltaGTotal = Math.Round(ps.DeltaGTotal, 2),
                    .NumSteps = ps.NumSteps,
                    .Steps = fwdSteps.Select(Function(fs) New ForwardStepDto With {
                        .RuleId = fs.RuleId, .RuleName = fs.RuleName,
                        .Substrates = fs.Substrates, .Products = fs.Products,
                        .DeltaG = Math.Round(fs.DeltaG, 2),
                        .EnzymeTier = fs.EnzymeTier}).ToList()})
            Next

            Dim report As New PathReport With {
                .Program = "RetroPath",
                .Version = VersionString,
                .Target = targetSmiles,
                .Parameters = New SearchParameters With {
                    .Strategy = opts.Strategy,
                    .BeamWidth = opts.BeamWidth,
                    .MaxDepth = opts.MaxDepth,
                    .SinkSize = sinkKeys.Count,
                    .NumRules = rules.Count,
                    .Weights = New Dictionary(Of String, Double) From {
                        {"thermo", w.Thermo}, {"enzyme", w.Enzyme}, {"length", w.Length}}},
                .Stats = New SearchStatsDto With {
                    .ApplicationsTried = searcher.Stats.ApplicationsTried,
                    .StatesGenerated = searcher.Stats.StatesGenerated,
                    .MaxDepthReached = searcher.Stats.MaxDepthReached,
                    .ElapsedMs = searcher.Stats.ElapsedMs,
                    .PathsFound = pathDtos.Count},
                .Paths = pathDtos}

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
            Console.Error.WriteLine($"完成: {completed.Count} 条完整路径（展示前 {pathDtos.Count}），" &
                                    $"{searcher.Stats.ApplicationsTried} 次规则应用，{sw.Elapsed.TotalMilliseconds:F0}ms")
            Return 0
        End Function

        Private Sub PrintUsage()
            Console.WriteLine("RetroPath — 基于广义反应规则的代谢网络逆向路径合成搜索（纯 BCL）")
            Console.WriteLine()
            Console.WriteLine("用法:")
            Console.WriteLine("  RetroPath search --target ""OC(=O)CC(O)(CC(=O)O)C(=O)O""")
            Console.WriteLine("      [--sink sink.tsv] [--rules rules.tsv] [--strategy beam]")
            Console.WriteLine("      [--beam-width 50] [--max-depth 6] [--max-paths 20]")
            Console.WriteLine("      [--out paths.json] [--pretty]")
            Console.WriteLine("  RetroPath enumerate-rules [--out rules.json]")
            Console.WriteLine("  RetroPath selftest")
            Console.WriteLine()
            Console.WriteLine("流程 [readme.md]: 目标 B →（广义规则逆向应用：反应中心匹配+原子映射）")
            Console.WriteLine("  → 前体逐步分解 → 全部落入底盘汇集合 → 正向组装 + ΔG/酶可得性/长度评分。")
        End Sub

    End Module

End Namespace
