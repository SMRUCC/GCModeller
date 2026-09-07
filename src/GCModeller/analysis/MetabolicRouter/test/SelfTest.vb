' ============================================================================
' SelfTest.vb — 内置自检（RetroPath selftest）
' ----------------------------------------------------------------------------
' 1. SMILES 解析 + 指纹不变性（原子重排/环/电荷）[readme.md §2.1 EC]
' 2. 子图匹配正负例（含 !O 否定）
' 3. 规则应用（9 条规则 × 正反向，价态闸门）
' 4. 束搜索端到端（柠檬酸/苏氨酸/苹果酸）+ 循环消除 [readme.md §3]
' 5. 评分范围与单调性 [readme.md §4]
' 6. SMILES 写出往返
' 7. 规则 TSV 加载
' 8. JSON 往返
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Text.Json
Imports RetroPath.Chem
Imports RetroPath.Model
Imports RetroPath.Search

Namespace RetroPath

    Public Module SelfTest

        Private _failures As Integer = 0

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
            Console.WriteLine("=== RetroPath SelfTest ===")
            TestSmilesAndKey()
            TestMatching()
            TestRules()
            TestSearch()
            TestScoring()
            TestSmilesRoundTrip()
            TestRulesTsv()
            TestJsonRoundTrip()
            Console.WriteLine($"=== {If(_failures = 0, "ALL TESTS PASSED", _failures & " TEST(S) FAILED")} ===")
            Return _failures
        End Function

        Private Function Key(smiles As String) As String
            Return SmilesIO.Parse(smiles).MolKey()
        End Function

        ' ---------------- 1. SMILES + 指纹 ----------------

        Private Sub TestSmilesAndKey()
            Console.WriteLine("-- SMILES 解析 + Morgan 指纹不变性 --")
            Dim m1 = SmilesIO.Parse("OC(=O)CC(O)C(=O)O")       ' 苹果酸
            Dim m3 = SmilesIO.Parse("C(C(=O)O)C(O)C(=O)O")     ' 同分子不同写法
            Dim k1 = m1.MolKey()
            Dim k3 = m3.MolKey()
            Console.WriteLine($"  苹果酸: 原子={m1.NumAtoms()} 键={m1.Bonds.Count} 指纹一致={k1 = k3}")
            Check(m1.NumAtoms() = 9 AndAlso m1.Bonds.Count = 8, "苹果酸图规模")
            Check(k1 = k3, "原子重排 → 指纹不变 [readme.md §2.1]")
            ' 隐式氢
            Dim e = SmilesIO.Parse("CCO")
            Dim hs = e.TotalH(0) & "," & e.TotalH(1) & "," & e.TotalH(2)
            Check(hs = "3,2,1", $"乙醇隐式氢 = {hs}（期望 3,2,1）")
            ' 环闭合
            Dim bz = SmilesIO.Parse("C1=CC=CC=C1")
            Dim orderSum = bz.Bonds.Sum(Function(b) b.Item3)
            Check(bz.NumAtoms() = 6 AndAlso bz.Bonds.Count = 6 AndAlso orderSum = 9,
                  $"苯（Kekulé）6 原子 6 键 键级和 {orderSum}")
            ' 电价
            Dim am = SmilesIO.Parse("[NH4+]")
            Check(am.Charges(0) = 1 AndAlso am.TotalH(0) = 4, "[NH4+] 电荷+氢")
            ' 价态校验
            Dim ac = SmilesIO.Parse("CC(=O)O")
            Check(ac.ValenceViolations().Count = 0, "乙酸价态合法")
        End Sub

        ' ---------------- 2. 匹配 ----------------

        Private Sub TestMatching()
            Console.WriteLine("-- 子图匹配 --")
            Dim propanol = SmilesIO.Parse("CCCO")
            Dim ohPat = PatternMatcher.ParsePattern("[C:1]-[OH1:2]")
            Dim n1 = PatternMatcher.Match(propanol, ohPat).Count
            Console.WriteLine($"  [C]-[OH1] 在丙醇匹配 = {n1}")
            Check(n1 = 1, "醇模式正例")
            Dim propane = SmilesIO.Parse("CCC")
            Check(PatternMatcher.Match(propane, ohPat).Count = 0, "无 OH 负例")
            Dim pyr = SmilesIO.Parse("CC(=O)C(=O)O")
            Dim ketoPat = PatternMatcher.ParsePattern("[CD3H0!O:1]=[O:2]")
            Dim n2 = PatternMatcher.Match(pyr, ketoPat).Count
            Console.WriteLine($"  [CD3H0!O]=[O] 在丙酮酸匹配 = {n2}（!O 排除羧基 C）")
            Check(n2 = 1, "!O 否定约束区分酮 C 与羧基 C")
            ' 价态闸门：乙酸氧化被拒
            Dim acetic = SmilesIO.Parse("CC(=O)O")
            Dim apps = RuleEngine.Apply(acetic, ohPat, PatternMatcher.ParsePattern("[C:1]=[O:2]"))
            Check(apps.Count = 0, "羧基氧化应用被价态闸门拒绝")
        End Sub

        ' ---------------- 3. 规则应用 ----------------

        Private Sub TestRules()
            Console.WriteLine("-- 规则应用（双向 + 价态闸门）--")
            Dim rules = RuleLibrary.BuiltinRules()
            Dim byId = rules.ToDictionary(Function(r) r.Id)
            ' R001 乙醇氧化 → 乙醛
            Dim eth = SmilesIO.Parse("CCO")
            Dim apps1 = RuleEngine.ApplyForward(eth, byId("R001"))
            Check(apps1.Count = 1 AndAlso apps1(0).Fragments.Any(
                Function(f) f.MolKey() = Key("CC=O")), "乙醇→乙醛（原子映射保留）")
            Check(apps1(0).AtomMap.Count = 2, "原子映射输出（2 类原子）")
            ' R001 苹果酸 → OAA
            Dim mal = SmilesIO.Parse("OC(=O)CC(O)C(=O)O")
            Dim oaaK = Key("OC(=O)CC(=O)C(=O)O")
            Dim apps2 = RuleEngine.ApplyForward(mal, byId("R001"))
            Check(apps2.Count = 1 AndAlso apps2(0).Fragments.Any(Function(f) f.MolKey() = oaaK),
                  "苹果酸→草酰乙酸（羧基匹配被价态排除）")
            ' R003 柠檬酸裂解 → OAA + 乙酸
            Dim cit = SmilesIO.Parse("OC(=O)CC(O)(CC(=O)O)C(=O)O")
            Dim apps3 = RuleEngine.ApplyForward(cit, byId("R003"))
            Dim keys3 = apps3.SelectMany(Function(a) a.Fragments).Select(Function(f) f.MolKey()).ToList()
            Check(apps3.Count = 2 AndAlso keys3.Contains(oaaK) AndAlso keys3.Contains(Key("CC(=O)O")),
                  "柠檬酸逆醛缩 → OAA + 乙酸（2 对称臂）")
            ' R002 苏氨酸转氨（逆向应用）→ 2-氧-3-羟丁酸 + NH3
            Dim thr = SmilesIO.Parse("CC(O)C(N)C(=O)O")
            Dim apps4 = RuleEngine.ApplyReverse(thr, byId("R002"))
            Dim keys4 = apps4.SelectMany(Function(a) a.Fragments).Select(Function(f) f.MolKey()).ToList()
            Check(apps4.Count = 1 AndAlso keys4.Contains(Key("CC(O)C(=O)C(=O)O")) AndAlso
                  keys4.Contains(Key("N")), "苏氨酸转氨 → 酮酸 + NH3（断键碎片=共产物）")
            ' R003 苏氨酸直接醛缩 → 甘氨酸 + 乙醛（苏氨酸醛缩酶）
            Dim apps5 = RuleEngine.ApplyForward(thr, byId("R003"))
            Dim keys5 = apps5.SelectMany(Function(a) a.Fragments).Select(Function(f) f.MolKey()).ToList()
            Check(keys5.Contains(Key("NCC(=O)O")) AndAlso keys5.Contains(Key("CC=O")),
                  "苏氨酸醛缩 → 甘氨酸 + 乙醛")
            ' R005 苹果酸脱水（逆向）→ 延胡索酸 + H2O
            Dim apps6 = RuleEngine.ApplyReverse(mal, byId("R005"))
            Dim keys6 = apps6.SelectMany(Function(a) a.Fragments).Select(Function(f) f.MolKey()).ToList()
            Check(keys6.Contains(Key("OC(=O)C=CC(=O)O")) AndAlso keys6.Contains(Key("O")),
                  "苹果酸脱水 → 延胡索酸 + H2O")
            ' R004 OAA 脱羧 → 丙酮酸 + CO2
            Dim apps7 = RuleEngine.ApplyForward(SmilesIO.Parse("OC(=O)C(=O)CC(=O)O"), byId("R004"))
            Dim keys7 = apps7.SelectMany(Function(a) a.Fragments).Select(Function(f) f.MolKey()).ToList()
            Check(keys7.Contains(Key("CC(=O)C(=O)O")) AndAlso keys7.Contains(Key("O=C=O")),
                  "OAA 脱羧 → 丙酮酸 + CO2")
        End Sub

        ' ---------------- 4. 束搜索 ----------------

        Private Sub TestSearch()
            Console.WriteLine("-- 束搜索端到端 [readme.md §3] --")
            Dim rules = RuleLibrary.BuiltinRules()
            Dim sinkSmiles As New List(Of String) From {
                "CC(=O)C(=O)O", "OC(=O)C(=O)CC(=O)O", "CC(=O)O", "CC=O",
                "NCC(=O)O", "OC(=O)C=O", "O", "O=C=O", "N", "OC(=O)C(=O)O"}
            Dim sinkKeys As New HashSet(Of String)()
            For Each s In sinkSmiles
                sinkKeys.Add(Key(s))
            Next
            Dim currencyKeys As New HashSet(Of String)()
            For Each cs In RuleLibrary.CurrencySmiles()
                currencyKeys.Add(Key(cs))
            Next
            Dim opts As New SearchOptions With {.BeamWidth = 30, .MaxDepth = 4}
            Dim searcher As New BeamSearch(rules, sinkKeys, currencyKeys, opts)

            ' 柠檬酸：1 步
            Dim p1 = searcher.Search(SmilesIO.Parse("OC(=O)CC(O)(CC(=O)O)C(=O)O"))
            Dim r1 = p1.Where(Function(p) p.Steps.Count = 1 AndAlso p.Steps(0).RuleId = "R003").ToList()
            Console.WriteLine($"  柠檬酸: 完整路径 {p1.Count}，1 步醛缩 {r1.Count}")
            Check(r1.Count >= 1, "柠檬酸 1 步 → OAA + 乙酸（双汇命中）")

            ' 苏氨酸：1 步醛缩 + 2 步转氨→醛缩
            Dim searcher2 As New BeamSearch(rules, sinkKeys, currencyKeys, opts)
            Dim p2 = searcher2.Search(SmilesIO.Parse("CC(O)C(N)C(=O)O"))
            Dim rDirect = p2.Where(Function(p) p.Steps.Count = 1 AndAlso p.Steps(0).RuleId = "R003").ToList()
            Dim rTwo = p2.Where(Function(p) p.Steps.Count = 2 AndAlso
                                p.Steps(0).RuleId = "R002" AndAlso p.Steps(1).RuleId = "R003").ToList()
            Console.WriteLine($"  苏氨酸: 完整路径 {p2.Count}，1 步 {rDirect.Count}，2 步 {rTwo.Count}")
            Check(rDirect.Count >= 1, "苏氨酸 1 步醛缩（甘氨酸 + 乙醛）")
            Check(rTwo.Count >= 1, "苏氨酸 2 步（转氨 → 醛缩，乙醛酸 + 乙醛）")

            ' 苹果酸：1 步氧化
            Dim searcher3 As New BeamSearch(rules, sinkKeys, currencyKeys, opts)
            Dim p3 = searcher3.Search(SmilesIO.Parse("OC(=O)CC(O)C(=O)O"))
            Check(p3.Any(Function(p) p.Steps.Count = 1 AndAlso p.Steps(0).RuleId = "R001"),
                  "苹果酸 1 步氧化 → OAA")

            ' 循环消除：所有路径无重复底物
            Dim cycOk = True
            For Each p In p2
                Dim seen As New HashSet(Of String)()
                For Each s In p.Steps
                    If seen.Contains(s.SubstrateKey) Then cycOk = False
                    seen.Add(s.SubstrateKey)
                Next
            Next
            Check(cycOk, "循环消除：路径内无重复化合物 [readme.md §3]")
        End Sub

        ' ---------------- 5. 评分 ----------------

        Private Sub TestScoring()
            Console.WriteLine("-- 评分 [readme.md §4] --")
            Dim st As New SearchState()
            st.Steps.Add(New RetroStep With {.RuleId = "R003", .DeltaG = -15, .EnzymeTier = 1})
            Dim w As New ScoreWeights()
            Dim ps1 = Scoring.ScorePath(st, w)
            st.Steps.Add(New RetroStep With {.RuleId = "R001", .DeltaG = 18, .EnzymeTier = 1})
            Dim ps2 = Scoring.ScorePath(st, w)
            Console.WriteLine($"  1 步全局 {ps1.GlobalScore:F3}  2 步全局 {ps2.GlobalScore:F3}")
            Check(ps1.GlobalScore > ps2.GlobalScore, "短路径全局分更高（长度项）")
            Check(ps1.GlobalScore > 0 AndAlso ps1.GlobalScore <= 1, "全局分 ∈ (0,1]")
            Check(Math.Abs(ps1.ThermoScore - Scoring.Sigmoid(1.5)) < 1e-9, "thermo = σ(−ΔG/10)")
            Check(Math.Abs(ps1.EnzymeScore - 1.0) < 1e-9, "tier=1 → 酶分 1.0")
        End Sub

        ' ---------------- 6. SMILES 往返 ----------------

        Private Sub TestSmilesRoundTrip()
            Console.WriteLine("-- SMILES 写出往返 --")
            Dim mols = {"CCO", "OC(=O)CC(O)C(=O)O", "CC(=O)O", "OCC(O)CO",
                        "C1CCCCC1", "CC(=O)N", "[NH4+]", "OP(=O)(O)O"}
            Dim ok = True
            For Each smi In mols
                Dim m1 = SmilesIO.Parse(smi)
                Dim out_ = SmilesIO.Write(m1)
                Dim m2 = SmilesIO.Parse(out_)
                Dim same = m1.MolKey() = m2.MolKey()
                If Not same Then
                    ok = False
                    Console.WriteLine($"    {smi} → {out_} → 指纹不一致")
                End If
            Next
            Check(ok, $"{mols.Length} 个分子 write→parse 指纹保真")
        End Sub

        ' ---------------- 7. 规则 TSV ----------------

        Private Sub TestRulesTsv()
            Console.WriteLine("-- 规则 TSV 加载 --")
            Dim tmp = Path.Combine(Path.GetTempPath(), "retropath_rules.tsv")
            File.WriteAllLines(tmp, {
                "# id	name	dG	tier	reversible	reactant	product",
                "U001	测试卤化	-5	3	false	[C:1]-[OH1:2]	[C:1]-[Cl]"})
            Dim rules = RuleLibrary.LoadRulesTsv(tmp)
            Check(rules.Count = 1 AndAlso rules(0).Id = "U001" AndAlso rules(0).EnzymeTier = 3,
                  "用户规则 TSV 解析")
            ' 应用：乙醇 → 氯乙烷
            Dim apps = RuleEngine.ApplyForward(SmilesIO.Parse("CCO"), rules(0))
            Check(apps.Count = 1 AndAlso apps(0).Fragments.Any(Function(f) f.MolKey() = Key("CCCl")),
                  "用户规则可应用（卤素替换，HCl 碎片）")
        End Sub

        ' ---------------- 8. JSON 往返 ----------------

        Private Sub TestJsonRoundTrip()
            Console.WriteLine("-- JSON 往返 --")
            Dim opts As New JsonSerializerOptions With {.WriteIndented = False}
            Dim dto As New PathDto With {
                .Id = "path_1", .GlobalScore = 0.87, .NumSteps = 2,
                .Steps = New List(Of ForwardStepDto) From {
                    New ForwardStepDto With {
                        .RuleId = "R001", .RuleName = "醇脱氢酶",
                        .Substrates = New List(Of String) From {"CCO"},
                        .Products = New List(Of String) From {"CC=O"},
                        .DeltaG = 18.0, .EnzymeTier = 1}}}
            Dim json = JsonSerializer.Serialize(dto, opts)
            Dim back = JsonSerializer.Deserialize(Of PathDto)(json)
            Check(back IsNot Nothing AndAlso back.Id = "path_1" AndAlso back.NumSteps = 2 AndAlso
                  back.Steps(0).Substrates(0) = "CCO" AndAlso back.Steps(0).DeltaG = 18.0,
                  "PathDto JSON 往返保真")
        End Sub

    End Module

End Namespace
