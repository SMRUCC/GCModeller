' ============================================================================
' PathwayFinderDemo.vb — 以 EcoCyc（E. coli K-12 MG1655）中的细菌次级代谢产物
'   为目标，演示 BioCyc → RetroPath 的生物合成通路逆向搜索。
'
' 运行：
'   dotnet run -c Release                 # 跑全部目标
'   dotnet run -c Release -- CHORISMATE   # 只跑指定 UNIQUE-ID
'   dotnet run -c Release -- all          # 全库汇模式（All）
' ============================================================================

Imports System.Text.Json
Imports RouterAdapter
Imports SMRUCC.genomics.Analysis.RetroPath.Chem
Imports SMRUCC.genomics.Analysis.RetroPath.Model
Imports SMRUCC.genomics.Analysis.RetroPath.Search
Imports SMRUCC.genomics.Data.BioCyc

Module PathwayFinderDemo

    ''' <summary>测试目标：均为 EcoCyc 29.0 中已确认有可用结构的次级代谢产物/信号分子/分支点前体</summary>
    ReadOnly targets As (id As String, name As String, category As String)() = {
        ("CHORISMATE", "分支酸 chorismate", "莽草酸途径分支点前体（验证多步反推）"),
        ("INDOLE", "吲哚 indole", "种间/群体感应信号分子"),
        ("PUTRESCINE", "腐胺 putrescine", "多胺"),
        ("SPERMIDINE", "亚精胺 spermidine", "多胺"),
        ("TREHALOSE", "α,α-海藻糖 trehalose", "渗透保护剂/储能二糖"),
        ("CPD-10774", "autoinducer-2", "群体感应信号分子（AI-2）"),
        ("ENTEROBACTIN", "肠杆菌素 enterobactin", "儿茶酚型铁载体（经典次级代谢产物）")
    }

    ReadOnly keyToName As New Dictionary(Of String, String)()

    Sub Run(Optional arg As String = Nothing)
        Dim useAllSink As Boolean = (String.Equals(arg, "all", StringComparison.OrdinalIgnoreCase))
        Dim onlyId As String = If(useAllSink, Nothing, arg)

        Console.WriteLine("BioCyc → RetroPath 生物合成通路搜索演示")
        Console.WriteLine($"数据库: F:\ecoli\29.0 (EcoCyc 29.0, E. coli K-12 MG1655)")
        Console.WriteLine()

        If onlyId IsNot Nothing AndAlso onlyId.StartsWith("debug:", StringComparison.OrdinalIgnoreCase) Then
            Dim dbgRouter As New BioCycAdapter(Workspace.Open("F:\ecoli\29.0"), opts:=New SearchOptions)
            DebugMatch(dbgRouter, onlyId.Substring(6))
            Return
        End If

        Dim biocyc As Workspace = Workspace.Open("F:\ecoli\29.0")

        ' 束搜索参数：规则集来自全库反应（上千条），首次实测取较小的束宽与深度以控耗时
        Dim opts As New SearchOptions With {
            .Strategy = "beam",
            .BeamWidth = 20,
            .MaxDepth = 4,
            .MaxPaths = 5,
            .MatchLimit = 20
        }

        Dim sw = Diagnostics.Stopwatch.StartNew()
        Dim router As New BioCycAdapter(
            biocyc,
            opts,
            Nothing,
            sinkMode:=If(useAllSink, BioCycSink.SinkModes.All, BioCycSink.SinkModes.Core),
            coreDegree:=4,
            maxMoleculeAtoms:=80,
            maxPatternAtoms:=32,
            includeBuiltinRules:=False,
            keepRuleTrace:=True)
        sw.Stop()

        Console.WriteLine()
        Console.WriteLine($"装配完成：规则 {router.Rules.Count} 条，汇 {router.Stats("sink")} 个，耗时 {sw.Elapsed.TotalSeconds:F1}s")
        Console.WriteLine()

        ' 分子指纹 → 化合物名，便于把结果里的 SMILES 还原成代谢物名称
        BuildNameIndex(router)

        ' 关键反应是否成功进入规则库（用来核对"目标分子的真实合成反应是否被覆盖"）
        For Each rxnId As String In {"CHORISMATE-SYNTHASE-RXN"}
            Dim why As String = Nothing
            If router.RuleTrace.TryGetValue(rxnId, why) Then
                Console.WriteLine($"规则覆盖检查 {rxnId}: 未收录（原因 {why}）")
            Else
                Console.WriteLine($"规则覆盖检查 {rxnId}: 已收录")
            End If
        Next
        Console.WriteLine()

        ' 落盘挖掘出的规则库，便于核查规则质量（SMARTS 两侧模式 + ΔG + 酶层级）
        Dim rulesTsv As String = IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "biocyc_rules.tsv")
        Dim lines As New List(Of String) From {"id" & vbTab & "name" & vbTab & "tier" & vbTab & "dG" & vbTab & "reactant" & vbTab & "product"}

        For Each r In router.Rules
            lines.Add(String.Join(vbTab, {r.Id, r.Name, CInt(r.EnzymeTier).ToString(), r.DeltaG.ToString("0.##"), r.ReactantText, r.ProductText}))
        Next
        IO.File.WriteAllLines(rulesTsv, lines)
        Console.WriteLine($"规则库 TSV 已写入: {rulesTsv}")
        Console.WriteLine()

        Dim reports As New List(Of PathReport)()

        For Each t In targets
            If onlyId IsNot Nothing AndAlso
               Not String.Equals(onlyId, t.id, StringComparison.OrdinalIgnoreCase) Then
                Continue For
            End If

            Dim st As CompoundStructure = router.GetCompound(t.id)

            If st Is Nothing Then
                Console.WriteLine($"=== {t.name} [{t.id}]：当前库中没有可用结构（缺失 SMILES 或含不支持元素），跳过")
                Continue For
            End If

            Dim report As PathReport = Nothing

            Try
                report = router.FindPathway(st.Smiles)
            Catch ex As Exception
                Console.WriteLine($"=== {t.name} [{t.id}]：搜索失败 {ex.Message}")
                Continue For
            End Try

            reports.Add(report)
            PrintReport(t, st, report)
        Next

        Dim outJson As String = IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "biocyc_pathways.json")
        IO.File.WriteAllText(outJson, JsonSerializer.Serialize(reports, New JsonSerializerOptions With {.WriteIndented = True}))
        Console.WriteLine()
        Console.WriteLine($"结果 JSON 已写入: {outJson}")

        If Not Console.IsInputRedirected Then
            Console.WriteLine()
            Console.Write("按任意键退出...")
            Console.ReadKey(True)
        End If
    End Sub

    ''' <summary>诊断：列出所有能作用于该化合物的规则（正向 + 逆向），用于核查规则是否命中</summary>
    Private Sub DebugMatch(router As BioCycAdapter, targetId As String)
        Dim st As CompoundStructure = router.GetCompound(targetId)

        If st Is Nothing Then
            Console.WriteLine($"[{targetId}] 无可用结构")
            Return
        End If

        Dim mol As Molecule = SmilesIO.Parse(st.Smiles)
        Dim hits As Integer = 0

        Console.WriteLine($"=== 规则命中检查：{targetId} = {st.Smiles} ===")

        For Each r As Rule In router.Rules
            Dim apps As New List(Of ApplicationResult)()

            Try
                apps.AddRange(RuleEngine.ApplyReverse(mol, r, 8))
                apps.AddRange(RuleEngine.ApplyForward(mol, r, 8))
            Catch ex As Exception
                Continue For
            End Try

            If apps.Count = 0 Then Continue For

            hits += 1
            Console.WriteLine($"  {r.Id} [{r.Name}]  {r.ReactantText} >> {r.ProductText}")

            For Each a As ApplicationResult In apps.Take(3)
                Console.WriteLine("      ⟶ " & String.Join(" + ", a.Fragments.Select(Function(f) SmilesIO.Write(f))))
            Next
        Next

        Console.WriteLine($"共 {hits} / {router.Rules.Count} 条规则可作用于 {targetId}")
    End Sub

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

    Private Sub PrintReport(t As (id As String, name As String, category As String),
                            st As CompoundStructure, report As PathReport)
        Console.WriteLine($"=== {t.name} [{t.id}] ===")
        Console.WriteLine($"   类别     : {t.category}")
        Console.WriteLine($"   目标     : {st.Smiles}")
        Console.WriteLine($"   参数     : 规则 {report.Parameters.NumRules}，汇 {report.Parameters.SinkSize}，" &
                          $"策略 {report.Parameters.Strategy}，束宽 {report.Parameters.BeamWidth}，深度上限 {report.Parameters.MaxDepth}")
        Console.WriteLine($"   统计     : 路径 {report.Stats.PathsFound} 条，展开状态 {report.Stats.StatesGenerated}，" &
                          $"规则应用 {report.Stats.ApplicationsTried} 次，耗时 {report.Stats.ElapsedMs}ms")

        If report.Paths.Count = 0 Then
            Console.WriteLine("   结果     : 未找到完整通路（目标可能已属于汇集合，或所需反应超出规则/深度上限）")
            Console.WriteLine()
            Return
        End If

        For Each p As PathDto In report.Paths
            Console.WriteLine($"   -- {p.Id}: 全局分 {p.GlobalScore:F3}（热力学 {p.ThermoScore:F2}，" &
                              $"酶可得性 {p.EnzymeScore:F2}，长度 {p.LengthScore:F2}），ΔG 合计 {p.DeltaGTotal}，{p.NumSteps} 步")

            Dim i As Integer = 1
            For Each s As ForwardStepDto In p.Steps
                Console.WriteLine($"      {i}. [{s.RuleId}] {s.RuleName}")
                Console.WriteLine($"         {JoinNames(s.Substrates)}  ⟶  {JoinNames(s.Products)}")
                i += 1
            Next
        Next

        Console.WriteLine()
    End Sub

    Private Function JoinNames(smilesList As List(Of String)) As String
        Dim parts As New List(Of String)()

        For Each s As String In smilesList
            Dim nm As String = Nothing
            Try
                Dim key As String = SmilesIO.Parse(s).MolKey()
                keyToName.TryGetValue(key, nm)
            Catch ex As Exception
                nm = Nothing
            End Try

            parts.Add(If(nm Is Nothing, s, $"{nm}"))
        Next

        Return String.Join(" + ", parts)
    End Function

End Module
