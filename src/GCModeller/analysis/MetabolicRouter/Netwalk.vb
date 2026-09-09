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

    ''' <summary>
    ''' <see cref="SynthesisRoute"/> 自动升级（加大束宽/深度重试）的耗时预算（毫秒）。
    ''' 超过预算即停止重试，避免单个查询把整个批量 demo 拖垮。
    ''' </summary>
    Public Const EscalateBudgetMs As Integer = 30000

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

    ''' <summary>
    ''' 从起点代谢物 A 出发，搜索合成目标代谢物 B 的最经济通路。
    ''' </summary>
    ''' <param name="sourceSmiles">起点化合物 A 的 SMILES。</param>
    ''' <param name="targetSmiles">目标化合物 B 的 SMILES。</param>
    ''' <param name="role">
    ''' A 在通路中的角色：<see cref="SourceRoles.Source"/> 要求 A 是最上游的叶子原料；
    ''' <see cref="SourceRoles.Anywhere"/> 允许 A 出现在通路任意位置（召回更高）。
    ''' </param>
    ''' <param name="strict">
    ''' 严格模式：汇集合只保留 A（以及 <paramref name="allowedExtra"/> 显式放行的化合物），
    ''' 即除 A 与货币分子外不依赖任何底盘代谢物；默认 False = A 与底盘汇均可作为起点。
    ''' </param>
    ''' <param name="allowedExtra">strict 模式下额外允许作为起点的化合物（名称, SMILES）。</param>
    ''' <param name="maxRoutes">最多返回多少条候选（0 = 默认 10 条）。</param>
    ''' <param name="escalate">首轮未命中时是否自动加大束宽/深度重试（最多 3 轮）。</param>
    ''' <returns>
    ''' <see cref="RouteReport"/>：<c>Best</c> 为最经济的一条通路，排序准则为
    ''' 「除 A 外所需的外源起始原料数（越少越好）→ 反应步数（越少越好）→ 全局分（越高越好）」。
    ''' 未找到满足起点约束的通路时 <c>Best</c> 为 Nothing、<c>Found</c> 为 False。
    ''' </returns>
    ''' <remarks>
    ''' 与 <see cref="Search(String)"/> 的关键差异：A 会被注入本次查询的汇集合，因此逆向展开时
    ''' A 一旦作为前体出现即判定为「已落地」，不会再被继续分解；同时把 A 的指纹作为导向键传给
    ''' <see cref="BeamSearch"/>，让通往 A 的分支在束剪枝中优先存活。这解决了「先枚举全部路径再
    ''' 事后筛选是否含 A」时命中率低的问题。
    ''' </remarks>
    Public Function SynthesisRoute(sourceSmiles As String,
                                   targetSmiles As String,
                                   Optional role As SourceRoles = SourceRoles.Source,
                                   Optional strict As Boolean = False,
                                   Optional allowedExtra As IEnumerable(Of (String, String)) = Nothing,
                                   Optional maxRoutes As Integer = 0,
                                   Optional escalate As Boolean = True) As RouteReport

        Dim sourceMol As Molecule = ParseOrThrow(sourceSmiles, "起点化合物 A")
        Dim targetMol As Molecule = ParseOrThrow(targetSmiles, "目标化合物 B")

        Dim sourceKey As String = sourceMol.MolKey()
        Dim targetKey As String = targetMol.MolKey()
        Dim sourceText As String = SmilesIO.Write(sourceMol)
        Dim targetText As String = SmilesIO.Write(targetMol)

        Dim report As New RouteReport With {
            .Program = "RetroPath",
            .Version = VersionString,
            .Source = sourceText,
            .Target = targetText,
            .Role = role.ToString(),
            .Candidates = {},
            .Found = False,
            .Stats = New RouteStatsDto With {.Strict = strict}}

        ' A 与 B 本就同一分子：无需设计通路
        If sourceKey = targetKey Then
            report.Found = True
            report.Best = New RouteDto With {
                .Id = "route_1", .EconomyRank = 1, .NumSteps = 0,
                .LengthScore = 1.0, .EnzymeScore = 1.0, .ThermoScore = 1.0,
                .GlobalScore = 1.0, .EconomyScore = 1.0,
                .ExternalCount = 0, .ExternalPrecursors = {},
                .SourceIsStart = True, .SourceStage = 0, .Steps = {}}
            Console.Error.WriteLine($"RetroPath {VersionString}: A 与 B 为同一分子，无需合成步骤")
            Return report
        End If

        ' ---------- 本次查询的汇集合：底盘汇（剔除 B 自身） + A ----------
        Dim sinkKeys As New HashSet(Of String)()

        If strict Then
            sinkKeys.Add(sourceKey)

            If allowedExtra IsNot Nothing Then
                For Each ex In allowedExtra
                    Dim k As String = TryMolKey(ex.Item2)
                    If k IsNot Nothing Then sinkKeys.Add(k)
                Next
            End If
        Else
            For Each s In sink
                Dim k As String = TryMolKey(s.smiles)
                If k Is Nothing Then Continue For
                ' B 自身必须剔除：BeamSearch 判定"目标已属于汇"会直接返回 0 条路径
                If k = targetKey Then Continue For
                sinkKeys.Add(k)
            Next
            sinkKeys.Add(sourceKey)
        End If

        Dim currencyKeys As New HashSet(Of String)()
        For Each cs In RuleLibrary.CurrencySmiles()
            Dim k As String = TryMolKey(cs)
            If k IsNot Nothing Then currencyKeys.Add(k)
        Next

        Console.Error.WriteLine($"RetroPath {VersionString}: {sourceText} → {targetText}  " &
                                  $"规则 = {rules.Count}  汇 = {sinkKeys.Count}  角色 = {role}  严格 = {strict}")

        ' ---------- 定向搜索（必要时自动升级束宽/深度） ----------
        Dim keep As Integer = If(maxRoutes > 0, maxRoutes, 10)
        Dim prefer As New HashSet(Of String) From {sourceKey}
        Dim curOpts As SearchOptions = CloneOptions(opts)
        If curOpts.MaxPaths < keep Then curOpts.MaxPaths = Math.Max(keep, 20)

        Dim sw = Stopwatch.StartNew()
        Dim all As New List(Of SearchState)()
        Dim totalApps As Int64 = 0
        Dim totalStates As Int64 = 0
        Dim maxDepthReached As Int32 = 0
        Dim rounds As Int32 = 0

        Do
            rounds += 1

            Dim searcher As New BeamSearch(rules, sinkKeys, currencyKeys, curOpts, prefer)
            Dim completed As List(Of SearchState) = searcher.Search(targetMol)

            all.AddRange(completed)
            totalApps += searcher.Stats.ApplicationsTried
            totalStates += searcher.Stats.StatesGenerated
            maxDepthReached = Math.Max(maxDepthReached, searcher.Stats.MaxDepthReached)

            Dim hitThisRound As Boolean = completed.Any(Function(st) Evaluate(st, sourceKey, role) IsNot Nothing)

            Console.Error.WriteLine($"  第 {rounds} 轮：束宽 {curOpts.BeamWidth}，深度上限 {curOpts.MaxDepth}，" &
                                      $"完整路径 {completed.Count}（命中 A 的 {If(hitThisRound, "有", "无")}），" &
                                      $"累计 {sw.Elapsed.TotalMilliseconds:F0}ms")

            If hitThisRound Then Exit Do
            If Not escalate OrElse rounds >= 3 Then Exit Do
            If sw.Elapsed.TotalMilliseconds > EscalateBudgetMs Then Exit Do

            curOpts = New SearchOptions With {
                .Strategy = opts.Strategy,
                .BeamWidth = Math.Min(curOpts.BeamWidth * 2, 400),
                .MaxDepth = curOpts.MaxDepth + 1,
                .MaxPaths = Math.Min(Math.Max(curOpts.MaxPaths * 2, 40), 400),
                .MatchLimit = opts.MatchLimit}
        Loop
        sw.Stop()

        ' ---------- 过滤 + 经济性排序 ----------
        Dim filtered As New List(Of RouteCandidate)()

        For Each st In all
            Dim cand As RouteCandidate = Evaluate(st, sourceKey, role)
            If cand IsNot Nothing Then filtered.Add(cand)
        Next

        Dim ranked = filtered.
            OrderBy(Function(c) c.ExternalCount).
            ThenBy(Function(c) c.Scores.NumSteps).
            ThenByDescending(Function(c) c.Scores.GlobalScore).
            Take(keep).ToList()

        Dim dtos As New List(Of RouteDto)()
        For i = 0 To ranked.Count - 1
            dtos.Add(ToRouteDto(ranked(i), sourceKey, i + 1))
        Next

        If dtos.Count > 0 Then
            report.Found = True
            report.Best = dtos(0)
            report.Candidates = dtos.Skip(1).ToArray()
        End If

        report.Parameters = New SearchParameters With {
            .Strategy = curOpts.Strategy,
            .BeamWidth = curOpts.BeamWidth,
            .MaxDepth = curOpts.MaxDepth,
            .SinkSize = sinkKeys.Count,
            .NumRules = rules.Count,
            .Weights = New Dictionary(Of String, Double) From {
                {"thermo", w.Thermo}, {"enzyme", w.Enzyme}, {"length", w.Length}}}

        report.Stats.ApplicationsTried = totalApps
        report.Stats.StatesGenerated = totalStates
        report.Stats.MaxDepthReached = maxDepthReached
        report.Stats.ElapsedMs = CLng(sw.Elapsed.TotalMilliseconds)
        report.Stats.PathsFound = dtos.Count
        report.Stats.Rounds = rounds
        report.Stats.CandidatesScanned = all.Count
        report.Stats.SourceHits = filtered.Count
        report.Stats.Strict = strict

        Console.Error.WriteLine($"完成: {rounds} 轮，完整路径 {all.Count} 条，命中 A 的 {filtered.Count} 条，" &
                                    $"最经济通路 {If(report.Found, report.Best.NumSteps & " 步 / 外源原料 " &
                                    report.Best.ExternalCount & " 个", "无")}，{sw.Elapsed.TotalMilliseconds:F0}ms")

        Return report
    End Function

    ''' <summary>解析 SMILES，失败时抛出带上下文的异常</summary>
    Private Shared Function ParseOrThrow(smiles As String, what As String) As Molecule
        If String.IsNullOrWhiteSpace(smiles) Then
            Throw New ArgumentException($"{what} 的 SMILES 为空")
        End If

        Try
            Dim m As Molecule = SmilesIO.Parse(smiles)
            If m Is Nothing OrElse m.NumAtoms() = 0 Then
                Throw New ArgumentException($"{what} 的 SMILES 解析结果为空: {smiles}")
            End If
            Return m
        Catch ex As ArgumentException
            Throw
        Catch ex As Exception
            Throw New ArgumentException($"{what} 的 SMILES 无法解析（{ex.Message}）: {smiles}", ex)
        End Try
    End Function

    ''' <summary>取分子指纹；SMILES 不可解析时返回 Nothing（用于汇/货币分子这类辅助集合）</summary>
    Private Shared Function TryMolKey(smiles As String) As String
        Try
            Return SmilesIO.Parse(smiles).MolKey()
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Private Shared Function CloneOptions(o As SearchOptions) As SearchOptions
        Return New SearchOptions With {
            .Strategy = o.Strategy,
            .BeamWidth = o.BeamWidth,
            .MaxDepth = o.MaxDepth,
            .MaxPaths = o.MaxPaths,
            .MatchLimit = o.MatchLimit}
    End Function

    ''' <summary>
    ''' 判定一条完整逆合成路径是否满足起点约束；满足时返回其经济性指标，否则返回 Nothing。
    ''' </summary>
    Private Function Evaluate(st As SearchState, sourceKey As String, role As SourceRoles) As RouteCandidate
        Dim produced As New HashSet(Of String)()
        Dim precursorMols As New Dictionary(Of String, Molecule)()
        Dim touched As Boolean = False

        For Each s As RetroStep In st.Steps
            produced.Add(s.SubstrateKey)
            If s.SubstrateKey = sourceKey Then touched = True

            For Each p In s.Precursors
                If Not precursorMols.ContainsKey(p.Item1) Then precursorMols(p.Item1) = p.Item2
                If p.Item1 = sourceKey Then touched = True
            Next
        Next

        If Not touched Then Return Nothing

        ' 叶子起始原料 = 从未被本路径任何一步生成过的前体（正向看即最上游原料）
        Dim leaves As New List(Of (key As String, smiles As String))()
        For Each kv In precursorMols
            If produced.Contains(kv.Key) Then Continue For
            leaves.Add((kv.Key, SmilesIO.Write(kv.Value)))
        Next

        Dim sourceIsStart As Boolean = leaves.Any(Function(x) x.Item1 = sourceKey)

        If role = SourceRoles.Source AndAlso Not sourceIsStart Then Return Nothing

        Dim externalCount As Integer = leaves.Count - If(sourceIsStart, 1, 0)

        Return New RouteCandidate With {
            .State = st,
            .Scores = Scoring.ScorePath(st, w),
            .Leaves = leaves,
            .ExternalCount = externalCount,
            .SourceIsStart = sourceIsStart}
    End Function

    ''' <summary>把候选通路转成 JSON DTO：正向组装、主链标注、经济性评分</summary>
    Private Function ToRouteDto(cand As RouteCandidate, sourceKey As String, rank As Integer) As RouteDto
        Dim st As SearchState = cand.State
        Dim fwd As List(Of ForwardStep) = Scoring.AssembleForward(st)
        Dim revSteps As List(Of RetroStep) = st.Steps.AsEnumerable().Reverse().ToList()

        ' 主链 = 从 A 出发经正向步骤可达的所有反应；可达性沿正向传播
        Dim available As New HashSet(Of String) From {sourceKey}
        Dim steps As New List(Of RouteStepDto)()
        Dim stageCounter As Integer = 0
        Dim sourceStage As Integer = If(cand.SourceIsStart, 0, -1)

        For i = 0 To fwd.Count - 1
            Dim keys As List(Of String) = revSteps(i).Precursors.Select(Function(p) p.Item1).ToList()
            Dim onChain As Boolean = keys.Any(Function(k) available.Contains(k))
            Dim stage As Integer = 0

            If onChain Then
                stageCounter += 1
                stage = stageCounter
                available.Add(revSteps(i).SubstrateKey)
            End If

            If sourceStage < 0 AndAlso keys.Contains(sourceKey) Then
                sourceStage = i + 1
            End If

            steps.Add(New RouteStepDto With {
                .RuleId = fwd(i).RuleId,
                .RuleName = fwd(i).RuleName,
                .Substrates = fwd(i).Substrates.ToArray(),
                .Products = fwd(i).Products.ToArray(),
                .DeltaG = Math.Round(fwd(i).DeltaG, 2),
                .EnzymeTier = fwd(i).EnzymeTier,
                .FromSource = onChain,
                .Stage = stage})
        Next

        Dim externals As String() = cand.Leaves.
            Where(Function(x) x.key <> sourceKey).
            Select(Function(x) x.smiles).ToArray()

        Dim economy As Double = 0.5 * (1.0 / (1.0 + cand.ExternalCount)) +
                                0.3 * cand.Scores.LengthScore +
                                0.2 * cand.Scores.GlobalScore

        Return New RouteDto With {
            .Id = $"route_{rank}",
            .EconomyRank = rank,
            .GlobalScore = Math.Round(cand.Scores.GlobalScore, 5),
            .ThermoScore = Math.Round(cand.Scores.ThermoScore, 5),
            .EnzymeScore = Math.Round(cand.Scores.EnzymeScore, 5),
            .LengthScore = Math.Round(cand.Scores.LengthScore, 5),
            .DeltaGTotal = Math.Round(cand.Scores.DeltaGTotal, 2),
            .NumSteps = cand.Scores.NumSteps,
            .ExternalCount = cand.ExternalCount,
            .ExternalPrecursors = externals,
            .SourceIsStart = cand.SourceIsStart,
            .SourceStage = If(sourceStage < 0, 0, sourceStage),
            .EconomyScore = Math.Round(economy, 5),
            .Steps = steps.ToArray()}
    End Function

    ''' <summary>一条满足起点约束的候选通路（中间结构，供排序与 DTO 组装使用）</summary>
    Private Class RouteCandidate
        Public State As SearchState
        Public Scores As PathScores
        ''' <summary>最上游起始原料：(指纹, SMILES)</summary>
        Public Leaves As List(Of (key As String, smiles As String))
        ''' <summary>除 A 之外还需几个外源起始原料</summary>
        Public ExternalCount As Integer
        ''' <summary>A 是否作为叶子原料出现</summary>
        Public SourceIsStart As Boolean
    End Class
End Class
