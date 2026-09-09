' ============================================================================
' BenchDemo.vb — 串行 / 并行对比基准
' ----------------------------------------------------------------------------
' 目的：验证优化（元素预过滤 + 分子图缓存 + 规则拓扑预计算 + 确定性并行展开）后，
'   ① 搜索结果与串行模式逐位一致；② 拿到可量化的加速比。
'
' 运行：
'   dotnet run -c Release -- bench              # 跑全部基准项
'   dotnet run -c Release -- bench INDOLE       # 只跑目标为 INDOLE 的项
'
' 判据：同一查询在 MaxDegreeOfParallelism = 1（串行）与默认并行度下，
'   路径签名（步数 + 每步规则 id + 正向底物/产物 + 各项得分）必须完全相同。
' ============================================================================

Imports System.Diagnostics
Imports System.Text.Json
Imports SMRUCC.genomics.Analysis.RetroPath
Imports SMRUCC.genomics.Analysis.RetroPath.Model
Imports SMRUCC.genomics.Analysis.RetroPath.Search
Imports SMRUCC.genomics.Data.BioCyc
Imports SMRUCC.genomics.Model.Metabolic.RouterAdapter
Imports SMRUCC.genomics.Model.Metabolic.RouterAdapter.Data

Module BenchDemo

    ''' <summary>基准项：起点 A → 目标 B（用 SynthesisRoute，语义为「从 A 出发合成 B」）</summary>
    ReadOnly cases As (source As String, target As String, note As String)() = {
        ("TRP", "INDOLE", "色氨酸 → 吲哚（1 步）"),
        ("L-ORNITHINE", "PUTRESCINE", "鸟氨酸 → 腐胺（1 步）"),
        ("PUTRESCINE", "SPERMIDINE", "腐胺 → 亚精胺（2 步，需外源原料）"),
        ("SHIKIMATE", "CHORISMATE", "莽草酸 → 分支酸（2 步）")
    }

    ''' <summary>单条基准结果（落盘用）</summary>
    Class BenchRow
        Public Property caseName As String
        Public Property note As String
        Public Property serialMs As Double
        Public Property parallelMs As Double
        Public Property speedup As Double
        Public Property pathsSerial As Integer
        Public Property pathsParallel As Integer
        Public Property signatureMatch As Boolean
        Public Property signature As String
    End Class

    Sub Run(Optional args As String() = Nothing)
        args = If(args, Array.Empty(Of String)())
        Dim only As String = args.FirstOrDefault()

        Console.WriteLine("BioCyc → RetroPath 串行 / 并行对比基准")
        Console.WriteLine($"处理器核数: {Environment.ProcessorCount}")
        Console.WriteLine($"数据库    : F:\ecoli\29.0 (EcoCyc 29.0)")
        Console.WriteLine()

        Dim sw = Stopwatch.StartNew()
        Dim router As New BioCycAdapter(
            Workspace.Open("F:\ecoli\29.0"),
            New SearchOptions With {.Strategy = "beam", .BeamWidth = 50, .MaxDepth = 6, .MaxPaths = 20, .MatchLimit = 20},
            Nothing,
            sinkMode:=SinkModes.Core,
            coreDegree:=4,
            maxMoleculeAtoms:=80,
            maxPatternAtoms:=32,
            includeBuiltinRules:=False,
            verbose:=False)
        sw.Stop()

        Console.WriteLine($"装配完成：规则 {router.Rules.Count} 条，汇 {router.Stats("sink")} 个，耗时 {sw.Elapsed.TotalSeconds:F1}s")
        Console.WriteLine()

        ' 两个搜索器：同一套规则与汇，仅并行度不同
        Dim serialWalker As Netwalk = router.CreateNetwalk(
            New SearchOptions With {.Strategy = "beam", .BeamWidth = 50, .MaxDepth = 6, .MaxPaths = 20,
                                    .MatchLimit = 20, .MaxDegreeOfParallelism = 1})
        Dim parallelWalker As Netwalk = router.CreateNetwalk(
            New SearchOptions With {.Strategy = "beam", .BeamWidth = 50, .MaxDepth = 6, .MaxPaths = 20,
                                    .MatchLimit = 20, .MaxDegreeOfParallelism = 0})

        Dim rows As New List(Of BenchRow)()

        For Each c In cases
            If only IsNot Nothing AndAlso
               Not String.Equals(only, c.target, StringComparison.OrdinalIgnoreCase) Then
                Continue For
            End If

            Dim src = router.GetCompound(c.source)
            Dim tgt = router.GetCompound(c.target)

            If src Is Nothing OrElse tgt Is Nothing Then
                Console.WriteLine($"跳过 {c.source} → {c.target}：缺少可用结构")
                Continue For
            End If

            ' 预热：消除 JIT 与首次装配的影响
            serialWalker.SynthesisRoute(src.Smiles, tgt.Smiles, maxRoutes:=5, escalate:=True)

            Dim swS = Stopwatch.StartNew()
            Dim rSerial = serialWalker.SynthesisRoute(src.Smiles, tgt.Smiles, maxRoutes:=5, escalate:=True)
            swS.Stop()

            Dim swP = Stopwatch.StartNew()
            Dim rParallel = parallelWalker.SynthesisRoute(src.Smiles, tgt.Smiles, maxRoutes:=5, escalate:=True)
            swP.Stop()

            Dim sigS As String = Signature(rSerial)
            Dim sigP As String = Signature(rParallel)
            Dim same As Boolean = (sigS = sigP)

            rows.Add(New BenchRow With {
                .caseName = $"{c.source} → {c.target}",
                .note = c.note,
                .serialMs = Math.Round(swS.Elapsed.TotalMilliseconds, 1),
                .parallelMs = Math.Round(swP.Elapsed.TotalMilliseconds, 1),
                .speedup = Math.Round(If(swP.Elapsed.TotalMilliseconds > 0, swS.Elapsed.TotalMilliseconds / swP.Elapsed.TotalMilliseconds, 0), 2),
                .pathsSerial = If(rSerial.Found, 1 + rSerial.Candidates.Length, 0),
                .pathsParallel = If(rParallel.Found, 1 + rParallel.Candidates.Length, 0),
                .signatureMatch = same,
                .signature = sigS})

            Console.WriteLine($"=== {c.source} → {c.target}（{c.note}）===")
            Console.WriteLine($"   串行   : {swS.Elapsed.TotalMilliseconds,8:F1} ms（预过滤 {rSerial.Stats.RulesPrefiltered} 次）")
            Console.WriteLine($"   并行   : {swP.Elapsed.TotalMilliseconds,8:F1} ms（预过滤 {rParallel.Stats.RulesPrefiltered} 次）")
            Console.WriteLine($"   加速比 : {If(swP.Elapsed.TotalMilliseconds > 0, swS.Elapsed.TotalMilliseconds / swP.Elapsed.TotalMilliseconds, 0),8:F2} ×")
            Console.WriteLine($"   路径数 : 串行 {If(rSerial.Found, 1 + rSerial.Candidates.Length, 0)} / 并行 {If(rParallel.Found, 1 + rParallel.Candidates.Length, 0)}")
            Console.WriteLine($"   一致性 : {If(same, "✔ 路径签名完全一致", "✘ 结果不一致！")}")
            Console.WriteLine()
        Next

        Dim sumS As Double = 0
        Dim sumP As Double = 0
        For Each r In rows
            sumS += r.serialMs
            sumP += r.parallelMs
        Next

        Console.WriteLine($"合计：串行 {sumS:F0} ms，并行 {sumP:F0} ms，整体加速比 " &
                          $"{If(sumP > 0, sumS / sumP, 0):F2} ×")

        Dim allMatch As Boolean = True
        For Each r In rows
            If Not r.signatureMatch Then allMatch = False
        Next
        Console.WriteLine($"一致性：{If(allMatch, "全部通过（并行结果与串行逐位一致）", "存在不一致项，需排查")}")

        Dim outJson As String = IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bench_result.json")
        IO.File.WriteAllText(outJson, JsonSerializer.Serialize(rows, New JsonSerializerOptions With {.WriteIndented = True}))
        Console.WriteLine($"结果 JSON 已写入: {outJson}")
    End Sub

    ''' <summary>
    ''' 路径签名：把一次搜索的全部候选通路压成一个可比较的字符串，
    ''' 用于断言串行与并行产出逐位一致（不比对统计计数，因为预过滤会减少无效尝试次数）。
    ''' </summary>
    Private Function Signature(r As RouteReport) As String
        Dim sb As New Text.StringBuilder()

        sb.Append(If(r.Found, "found", "none")).Append("|")

        If r.Found Then
            AppendRoute(sb, r.Best)
            For Each c In r.Candidates
                sb.Append(";")
                AppendRoute(sb, c)
            Next
        End If

        Return sb.ToString()
    End Function

    Private Sub AppendRoute(sb As Text.StringBuilder, d As RouteDto)
        sb.Append(d.NumSteps).Append(":").Append(d.ExternalCount).Append(":")
        sb.Append(d.DeltaGTotal.ToString("F2")).Append(":")
        sb.Append(d.GlobalScore.ToString("F5")).Append("[")

        For Each s In d.Steps
            sb.Append(s.RuleId).Append("(")
            sb.Append(String.Join("+", s.Substrates)).Append(">")
            sb.Append(String.Join("+", s.Products))
            sb.Append(")")
        Next

        sb.Append("]")
    End Sub

End Module
