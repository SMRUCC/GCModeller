Imports SMRUCC.genomics.Analysis.RetroPath.Chem
Imports SMRUCC.genomics.Analysis.RetroPath.Model
Imports SMRUCC.genomics.Analysis.RetroPath.Search

Public Class Netwalk

    Public Const VersionString As String = "1.0.0"

    Public ReadOnly Property opts As SearchOptions
    Public ReadOnly Property w As ScoreWeights
    Public ReadOnly Property rules As List(Of Rule)
    Public ReadOnly Property sink As List(Of Tuple(Of String, String))

    Sub New(rules As List(Of Rule), sink As List(Of Tuple(Of String, String)), opts As SearchOptions, w As ScoreWeights)
        _rules = rules
        _w = w
        _opts = opts
        _sink = sink
    End Sub

    Public Function Search(targetSmiles As String) As PathReport
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
        Dim sw = Stopwatch.StartNew()
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

        Return report
    End Function
End Class
