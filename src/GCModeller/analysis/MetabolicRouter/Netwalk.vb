Imports SMRUCC.genomics.Analysis.RetroPath.Chem
Imports SMRUCC.genomics.Analysis.RetroPath.Model
Imports SMRUCC.genomics.Analysis.RetroPath.Search

''' <summary>
''' 代谢网络逆向合成通路搜索器：整个 RetroPath 的对外主入口。
''' </summary>
''' <remarks>
''' 一次典型调用：准备广义规则集与底盘汇集合 → 构造本对象 → 反复调用
''' <see cref="Search(String)"/> 查询不同目标。构造期只做参数固化，规则与汇可被多个查询复用。
''' </remarks>
Public Class Netwalk

    ''' <summary>
    ''' 程序版本号，会写入结果报告的 <c>version</c> 字段。
    ''' </summary>
    Public Const VersionString As String = "1.0.0"

    ''' <summary>构造时传入的搜索参数。</summary>
    Public ReadOnly Property opts As SearchOptions

    ''' <summary>构造时传入的评分权重。</summary>
    Public ReadOnly Property w As ScoreWeights

    ''' <summary>参与搜索的广义反应规则集（构造时拷贝，外部改动不影响本对象）。</summary>
    Public ReadOnly Property rules As List(Of Rule)

    ''' <summary>
    ''' 底盘内源代谢物集合（汇），每项为 (化合物标识, SMILES)。
    ''' </summary>
    Public ReadOnly Property sink As List(Of (String, smiles As String))

    ''' <summary>
    ''' 构造搜索器。
    ''' </summary>
    ''' <param name="rules">广义反应规则集（可空集合，但那样搜索不出任何路径）。</param>
    ''' <param name="sink">
    ''' 底盘内源代谢物集合，每项为 (名称或 ID, SMILES)；SMILES 会被解析成分子指纹用于终止判定。
    ''' </param>
    ''' <param name="opts">搜索参数；传入 Nothing 时调用方需自行保证后续不为 Nothing。</param>
    ''' <param name="w">评分权重。</param>
    Sub New(rules As IReadOnlyCollection(Of Rule), sink As IReadOnlyCollection(Of (String, smiles As String)), opts As SearchOptions, w As ScoreWeights)
        _rules = New List(Of Rule)(rules)
        _w = w
        _opts = opts
        _sink = New List(Of (String, String))(sink)
    End Sub

    ''' <summary>
    ''' 以指定目标分子做逆向合成通路搜索，返回完整结果报告。
    ''' </summary>
    ''' <param name="targetSmiles">
    ''' 目标分子 B 的 SMILES（须为 <see cref="Chem.SmilesIO"/> 支持的子集）。
    ''' </param>
    ''' <returns>
    ''' 含参数快照、统计与候选通路的 <see cref="PathReport"/>；
    ''' 目标已属于汇集合或未找到通路时，<c>Paths</c> 为空列表。
    ''' </returns>
    ''' <remarks>
    ''' 流程：解析目标与汇 → 束搜索展开 → 按全局分排序取前 maxPaths → 正向组装并评分。
    ''' 过程中会向 <see cref="Console.Error"/> 输出一行进度摘要。
    ''' </remarks>
    Public Function Search(targetSmiles As String) As PathReport
        ' 解析目标与汇
        Dim target = SmilesIO.Parse(targetSmiles)
        Dim sinkKeys As New HashSet(Of String)()
        For Each s In sink
            sinkKeys.Add(SmilesIO.Parse(s.smiles).MolKey())
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
        Dim completed As List(Of SearchState) = searcher.Search(target)
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
                .Steps = fwdSteps _
                    .Select(Function(fs)
                                Return New ForwardStepDto With {
                    .RuleId = fs.RuleId, .RuleName = fs.RuleName,
                    .Substrates = fs.Substrates.ToArray, .Products = fs.Products.ToArray,
                    .DeltaG = Math.Round(fs.DeltaG, 2),
                    .EnzymeTier = fs.EnzymeTier}
                            End Function).ToArray})
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
            .Paths = pathDtos.ToArray}

        Console.Error.WriteLine($"完成: {completed.Count} 条完整路径（展示前 {pathDtos.Count}），" &
                                    $"{searcher.Stats.ApplicationsTried} 次规则应用，{sw.Elapsed.TotalMilliseconds:F0}ms")

        Return report
    End Function
End Class
