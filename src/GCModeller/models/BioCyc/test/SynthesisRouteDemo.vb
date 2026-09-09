' ============================================================================
' SynthesisRouteDemo.vb — 以 EcoCyc（E. coli K-12 MG1655）演示「从代谢物 A 出发，
'   找到最经济的一条合成代谢物 B 的通路」。
'
' 运行：
'   dotnet run -c Release -- route                      # 跑全部 A → B 对（默认 role = Source）
'   dotnet run -c Release -- route INDOLE               # 只跑目标为 INDOLE 的那一对
'   dotnet run -c Release -- route strict               # strict 模式（只允许 A + 货币分子）
'   dotnet run -c Release -- route anywhere             # role = Anywhere（A 可作中间体）
'   dotnet run -c Release -- route strict anywhere      # 组合
'
' 排序准则（Netwalk.SynthesisRoute 内建）：
'   除 A 外所需的外源起始原料数（越少越好）→ 反应步数（越少越好）→ 全局分（越高越好）
' ============================================================================

Imports System.Text.Json
Imports SMRUCC.genomics.Analysis.RetroPath.Chem
Imports SMRUCC.genomics.Analysis.RetroPath.Model
Imports SMRUCC.genomics.Analysis.RetroPath.Search
Imports SMRUCC.genomics.Data.BioCyc
Imports SMRUCC.genomics.Model.Metabolic.RouterAdapter
Imports SMRUCC.genomics.Model.Metabolic.RouterAdapter.Data

Module SynthesisRouteDemo

    ''' <summary>
    ''' 测试用的 A → B 化合物对（均为 EcoCyc 29.0 中已确认有可用结构的 frame id）。
    ''' heavy = True 的那一对目标分子较大，使用降级的束搜索参数单独装配一个搜索器。
    ''' </summary>
    ReadOnly pairs As (source As String, target As String, note As String, heavy As Boolean)() = {
        ("TRP", "INDOLE", "色氨酸 → 吲哚（色氨酸酶 TnaA，验证 1 步通路）", False),
        ("L-ORNITHINE", "PUTRESCINE", "鸟氨酸 → 腐胺（鸟氨酸脱羧酶，验证 1 步通路）", False),
        ("PUTRESCINE", "SPERMIDINE", "腐胺 → 亚精胺（需丙胺基供体，验证外源原料识别）", False),
        ("SHIKIMATE", "CHORISMATE", "莽草酸 → 分支酸（莽草酸途径）", False),
        ("GLT", "PRO", "谷氨酸 → 脯氨酸（多步环化还原）", False),
        ("SER", "TRP", "丝氨酸 → 色氨酸（多步，芳香族氨基酸合成）", False),
        ("CHORISMATE", "ENTEROBACTIN", "分支酸 → 肠杆菌素（儿茶酚型铁载体；真实途径 ≥7 步且需 3 次对称缩合，当前规则集难以收束）", True)
    }

    ''' <summary>分子指纹 → 化合物 frame id，用于把结果里的 SMILES 还原成代谢物名</summary>
    ReadOnly keyToName As New Dictionary(Of String, String)()

    Sub Run(Optional args As String() = Nothing)
        args = If(args, Array.Empty(Of String)())

        Dim strict As Boolean = args.Any(Function(s) s.Equals("strict", StringComparison.OrdinalIgnoreCase))
        Dim role As SourceRoles = If(
            args.Any(Function(s) s.Equals("anywhere", StringComparison.OrdinalIgnoreCase)),
            SourceRoles.Anywhere, SourceRoles.Source)
        Dim onlyTarget As String = args.FirstOrDefault(
            Function(s) Not s.Equals("strict", StringComparison.OrdinalIgnoreCase) AndAlso
                        Not s.Equals("anywhere", StringComparison.OrdinalIgnoreCase))

        Console.WriteLine("BioCyc → RetroPath 定向合成通路搜索演示（A → B，最经济通路）")
        Console.WriteLine($"数据库: F:\ecoli\29.0 (EcoCyc 29.0, E. coli K-12 MG1655)")
        Console.WriteLine($"模式   : role = {role}，strict = {strict}")
        Console.WriteLine()

        Dim biocyc As Workspace = Workspace.Open("F:\ecoli\29.0")

        ' 常规束搜索参数
        Dim opts As New SearchOptions With {
            .Strategy = "beam",
            .BeamWidth = 50,
            .MaxDepth = 6,
            .MaxPaths = 20,
            .MatchLimit = 20
        }

        Dim sw = Diagnostics.Stopwatch.StartNew()
        Dim router As New BioCycAdapter(
            biocyc,
            opts,
            Nothing,
            sinkMode:=SinkModes.Core,
            coreDegree:=4,
            maxMoleculeAtoms:=80,
            maxPatternAtoms:=32,
            includeBuiltinRules:=False,
            verbose:=True)
        sw.Stop()

        Console.WriteLine()
        Console.WriteLine($"装配完成：规则 {router.Rules.Count} 条，汇 {router.Stats("sink")} 个，耗时 {sw.Elapsed.TotalSeconds:F1}s")
        Console.WriteLine()

        BuildNameIndex(router)

        ' diag 模式：只做「A 与 B 能否一步连通」的规则级诊断，用于排查命中率为何偏低
        If args.Any(Function(s) s.Equals("diag", StringComparison.OrdinalIgnoreCase)) Then
            Dim ids As String() = args.Where(
                Function(s) Not {"diag", "strict", "anywhere"}.Contains(s.ToLower())).ToArray()

            If ids.Length >= 2 Then
                DiagPair(router, ids(0), ids(1))
            Else
                For Each p In pairs
                    DiagPair(router, p.source, p.target)
                Next
            End If
            Return
        End If

        Dim reports As New List(Of RouteReport)()
        Dim heavyRouter As BioCycAdapter = Nothing

        For Each pair In pairs
            If onlyTarget IsNot Nothing AndAlso
               Not String.Equals(onlyTarget, pair.target, StringComparison.OrdinalIgnoreCase) Then
                Continue For
            End If

            Dim use As BioCycAdapter = router

            If pair.heavy Then
                If heavyRouter Is Nothing Then
                    Console.WriteLine($"-- 目标 {pair.target} 分子较大，单独装配搜索器（beam 32 / depth 6）--")

                    heavyRouter = New BioCycAdapter(
                        biocyc,
                        New SearchOptions With {.Strategy = "beam", .BeamWidth = 32, .MaxDepth = 6, .MaxPaths = 10, .MatchLimit = 10},
                        Nothing,
                        sinkMode:=SinkModes.Core,
                        coreDegree:=4,
                        maxMoleculeAtoms:=80,
                        maxPatternAtoms:=32,
                        includeBuiltinRules:=False,
                        verbose:=False)
                End If

                use = heavyRouter
            End If

            Dim report As RouteReport = Nothing
            Dim pairSw = Diagnostics.Stopwatch.StartNew()

            Try
                report = use.SynthesisRouteById(
                    sourceId:=pair.source,
                    targetId:=pair.target,
                    role:=role,
                    strict:=strict,
                    maxRoutes:=5,
                    escalate:=True)
            Catch ex As Exception
                pairSw.Stop()
                Console.WriteLine($"=== {pair.source} → {pair.target}：搜索失败 {ex.Message}")
                Console.WriteLine()
                Continue For
            End Try

            pairSw.Stop()
            reports.Add(report)
            PrintReport(pair, report, pairSw.Elapsed.TotalMilliseconds)
        Next

        Dim hit As Integer = 0
        For Each r In reports
            If r.Found Then hit += 1
        Next

        Console.WriteLine($"汇总：{hit} / {reports.Count} 对找到满足起点约束的通路")

        Dim outJson As String = IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "biocyc_routes.json")
        IO.File.WriteAllText(outJson, JsonSerializer.Serialize(reports, New JsonSerializerOptions With {.WriteIndented = True}))
        Console.WriteLine()
        Console.WriteLine($"结果 JSON 已写入: {outJson}")

        If Not Console.IsInputRedirected Then
            Console.WriteLine()
            Console.Write("按任意键退出...")
            Console.ReadKey(True)
        End If
    End Sub

    Private Sub PrintReport(pair As (source As String, target As String, note As String, heavy As Boolean),
                            report As RouteReport,
                            elapsedMs As Double)
        Console.WriteLine($"=== {pair.source} → {pair.target} ===")
        Console.WriteLine($"   说明     : {pair.note}")
        Console.WriteLine($"   起点 A   : {report.Source}")
        Console.WriteLine($"   目标 B   : {report.Target}")
        Console.WriteLine($"   统计     : {report.Stats.Rounds} 轮，完整路径 {report.Stats.CandidatesScanned} 条，" &
                          $"命中 A 的 {report.Stats.SourceHits} 条，规则应用 {report.Stats.ApplicationsTried} 次，" &
                          $"耗时 {report.Stats.ElapsedMs}ms（含打印 {elapsedMs:F0}ms）")

        If Not report.Found Then
            Console.WriteLine("   结果     : 未找到满足起点约束的通路")

            If report.Stats.CandidatesScanned = 0 Then
                Console.WriteLine("   原因     : 当前规则集无法把目标分子碎片化成更小的前体（该分解反应未被挖掘成规则），" &
                                  "逆向搜索在深度上限内收束不了")
            Else
                Console.WriteLine("   原因     : 已枚举到完整通路，但没有一条经过起点 A——通常是 A 与 B 之间的反应未被挖掘成规则" &
                                  If(report.Stats.Strict, "；strict 模式下如需 ATP/NADH 等辅因子，请用 allowedExtraIds 放行", ""))
            End If

            Console.WriteLine()
            Return
        End If

        Dim best As RouteDto = report.Best

        Console.WriteLine($"   最经济   : {best.NumSteps} 步，除 A 外还需外源起始原料 {best.ExternalCount} 个，" &
                          $"ΔG 合计 {best.DeltaGTotal}，经济性分 {best.EconomyScore:F3}（全局分 {best.GlobalScore:F3}）")
        Console.WriteLine($"   A 的角色 : " & If(best.SourceIsStart,
                          "最上游起始原料（叶子）",
                          $"通路中间体（首次出现于第 {best.SourceStage} 步底物）"))

        If best.ExternalCount > 0 Then
            Console.WriteLine($"   外源原料 : {JoinNames(best.ExternalPrecursors)}")
        End If

        Console.WriteLine($"   正向通路 :")

        Dim i As Integer = 1
        For Each s As RouteStepDto In best.Steps
            Dim mark As String = If(s.FromSource, $"[主链#{s.Stage}]", "[支链  ]")
            Console.WriteLine($"      {i}. {mark} [{s.RuleId}] {s.RuleName}")
            Console.WriteLine($"         {JoinNames(s.Substrates)}  ⟶  {JoinNames(s.Products)}")
            i += 1
        Next

        If report.Candidates.Length > 0 Then
            Console.WriteLine($"   其余候选 : {report.Candidates.Length} 条（" &
                              String.Join("；", report.Candidates.Select(
                                  Function(c) $"{c.NumSteps} 步 / 外源 {c.ExternalCount}")) & "）")
        End If

        Console.WriteLine()
    End Sub

    ''' <summary>
    ''' 规则级诊断：检查 A 与 B 之间是否存在「一步连通」的规则应用（正向 A → B 与逆向 B → A 各查一遍）。
    ''' 用于定位「SynthesisRoute 命中 0 条」的根因：规则是否覆盖了该反应、应用后生成的分子指纹是否与目标一致。
    ''' </summary>
    Private Sub DiagPair(router As BioCycAdapter, sourceId As String, targetId As String)
        Dim aSt As CompoundStructure = router.GetCompound(sourceId)
        Dim bSt As CompoundStructure = router.GetCompound(targetId)

        If aSt Is Nothing OrElse bSt Is Nothing Then
            Console.WriteLine($"=== {sourceId} → {targetId}：某一侧无可用结构，跳过")
            Return
        End If

        Dim aKey As String = aSt.Mol.MolKey()
        Dim bKey As String = bSt.Mol.MolKey()
        Dim molA As Molecule = SmilesIO.Parse(aSt.Smiles)
        Dim molB As Molecule = SmilesIO.Parse(bSt.Smiles)

        Console.WriteLine($"=== {sourceId} → {targetId} 一步连通性诊断 ===")
        Console.WriteLine($"   A = {aSt.Smiles}")
        Console.WriteLine($"   B = {bSt.Smiles}")

        Dim revHits As Integer = 0
        Dim fwdHits As Integer = 0
        Dim nearMiss As New List(Of String)()

        For Each r As Rule In router.Rules
            ' 逆向：把规则作用在 B 上，看能否得到 A
            Dim apps As New List(Of ApplicationResult)()

            Try
                apps.AddRange(RuleEngine.ApplyReverse(molB, r, 20))
            Catch ex As Exception
            End Try

            For Each a In apps
                For Each f In a.Fragments
                    If f.MolKey() = aKey Then
                        Console.WriteLine($"   [逆向命中] {r.Id}: {SmilesIO.Write(f)}")
                        revHits += 1
                    ElseIf SameHeavyAtoms(f, aSt.Mol) Then
                        nearMiss.Add($"reverse {r.Id}: {SmilesIO.Write(f)}")
                    End If
                Next
            Next

            ' 正向：把规则作用在 A 上，看能否得到 B
            apps.Clear()

            Try
                apps.AddRange(RuleEngine.ApplyForward(molA, r, 20))
            Catch ex As Exception
            End Try

            For Each a In apps
                For Each f In a.Fragments
                    If f.MolKey() = bKey Then
                        Console.WriteLine($"   [正向命中] {r.Id}: {SmilesIO.Write(f)}")
                        fwdHits += 1
                    End If
                Next
            Next
        Next

        Console.WriteLine($"   逆向 B→A 命中 {revHits} 次，正向 A→B 命中 {fwdHits} 次")

        If revHits = 0 AndAlso fwdHits = 0 AndAlso nearMiss.Count > 0 Then
            Console.WriteLine("   存在「重原子组成相同但指纹不一致」的近失结果（多为显式氢/电荷差异）：")
            For Each s In nearMiss.Take(5)
                Console.WriteLine($"      {s}")
            Next
        End If

        Console.WriteLine()
    End Sub

    ''' <summary>两个分子的重原子组成（元素计数）是否相同——用于识别"近失"结果</summary>
    Private Function SameHeavyAtoms(x As Molecule, y As Molecule) As Boolean
        If x.NumAtoms() <> y.NumAtoms() Then Return False

        Dim cnt As New Dictionary(Of String, Integer)()

        For Each el As String In x.Elements
            Dim n As Integer = 0
            cnt.TryGetValue(el, n)
            cnt(el) = n + 1
        Next
        For Each el As String In y.Elements
            Dim n As Integer = 0
            cnt.TryGetValue(el, n)
            If n = 0 Then Return False
            cnt(el) = n - 1
        Next

        Return True
    End Function

    Private Sub BuildNameIndex(router As BioCycAdapter)
        keyToName.Clear()

        For Each kvp In router.Compounds
            Dim key As String = Nothing
            Try
                key = kvp.Value.Mol.MolKey()
            Catch ex As Exception
                Continue For
            End Try
            If Not keyToName.ContainsKey(key) Then keyToName(key) = kvp.Key
        Next
    End Sub

    Private Function JoinNames(smilesList As IEnumerable(Of String)) As String
        Dim parts As New List(Of String)()

        For Each s As String In smilesList
            Dim nm As String = Nothing
            Try
                keyToName.TryGetValue(SmilesIO.Parse(s).MolKey(), nm)
            Catch ex As Exception
                nm = Nothing
            End Try
            parts.Add(If(nm Is Nothing, s, nm))
        Next

        Return String.Join(" + ", parts)
    End Function

End Module
