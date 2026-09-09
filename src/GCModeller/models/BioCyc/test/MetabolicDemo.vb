' ============================================================================
' MetabolicDemo.vb — MetabolicAdapter（GCModeller 内部代谢模型）通路搜索演示
' ----------------------------------------------------------------------------
' Part A：用 DemoNetwork.vb 手写的自包含 E. coli 代谢网络（MetabolicCompound /
'         MetabolicReaction）装配 MetabolicAdapter，对铁载体 / 信号分子 / 多胺 /
'         海藻糖 / 分支点前体等目标做通路搜索。
' Part B：把真实 EcoCyc 库（F:\ecoli\29.0）经 BioCycMetabolicConvertor 转换成内部
'         模型后做规模化验证，并与 BioCycAdapter（同一份数据）的结果对拍——
'         两者共用同一套通用挖掘引擎，理论上规则数与路径数应完全一致。
'
' 运行：dotnet run -c Release -- metabolic
' ============================================================================

Imports System.Text.Json
Imports SMRUCC.genomics.Model.Metabolic.RouterAdapter
Imports SMRUCC.genomics.Analysis.RetroPath.Chem
Imports SMRUCC.genomics.Analysis.RetroPath.Model
Imports SMRUCC.genomics.Analysis.RetroPath.Search
Imports SMRUCC.genomics.Data.BioCyc
Imports SMRUCC.genomics.MetabolicModel

Public Module MetabolicDemo

    ''' <summary>演示目标（均为知识库手写网络中的化合物）。</summary>
    ReadOnly demoTargets As String() = {
        "CHORISMATE", "INDOLE", "PUTRESCINE", "TREHALOSE", "ENTEROBACTIN"
    }

    ''' <summary>真实库对拍时的目标（与 PathwayFinderDemo 保持一致，便于结果对拍）。</summary>
    ReadOnly realTargets As (id As String, name As String, category As String)() = {
        ("CHORISMATE", "分支酸 chorismate", "莽草酸途径分支点前体"),
        ("INDOLE", "吲哚 indole", "种间/群体感应信号分子"),
        ("PUTRESCINE", "腐胺 putrescine", "多胺"),
        ("SPERMIDINE", "亚精胺 spermidine", "多胺"),
        ("TREHALOSE", "α,α-海藻糖 trehalose", "渗透保护剂/储能二糖"),
        ("CPD-10774", "autoinducer-2", "群体感应信号分子（AI-2）"),
        ("ENTEROBACTIN", "肠杆菌素 enterobactin", "儿茶酚型铁载体（经典次级代谢产物）")
    }

    ReadOnly keyToName As New Dictionary(Of String, String)()

    Public Sub Run(Optional arg As String = Nothing)
        Console.WriteLine("MetabolicAdapter — GCModeller 内部代谢模型 生物合成通路搜索演示")
        Console.WriteLine(New String("="c, 78))

        ' ---------------- Part A：手写网络（自包含） ----------------
        PartA_HandWrittenNetwork()

        ' ---------------- Part B：真实库规模验证 + 与 BioCycAdapter 对拍 ----------------
        If String.Equals(arg, "no-real", StringComparison.OrdinalIgnoreCase) Then
            Console.WriteLine()
            Console.WriteLine("（按参数跳过 Part B 真实库规模验证）")
            Return
        End If

        PartB_RealDatabase()
    End Sub

    ' -------------------------------------------------------------------------
    ' Part A：手写网络
    ' -------------------------------------------------------------------------
    Private Sub PartA_HandWrittenNetwork()
        Console.WriteLine()
        Console.WriteLine("Part A. 手写内置 E. coli 代谢网络（MetabolicCompound / MetabolicReaction）")
        Console.WriteLine(New String("-"c, 78))

        Dim compounds As List(Of MetabolicCompound) = DemoNetwork.GetCompounds()
        Dim reactions As List(Of MetabolicReaction) = DemoNetwork.GetReactions()
        Dim opts As New SearchOptions With {
            .Strategy = "beam",
            .BeamWidth = 20,
            .MaxDepth = 6,
            .MaxPaths = 5,
            .MatchLimit = 20
        }

        ' 小网络反应数少，coreDegree 取 2 以便枢纽机制能选到一部分汇成员
        Dim sw = Diagnostics.Stopwatch.StartNew()
        Dim router As New MetabolicAdapter(compounds, reactions, opts,
                                           sinkMode:=SinkModes.Core, coreDegree:=2,
                                           keepRuleTrace:=True)
        sw.Stop()

        PrintStats("MetabolicAdapter", router, sw.Elapsed.TotalMilliseconds)
        PrintRejectReasons(router)

        BuildNameIndex(router.Compounds)

        For Each targetId As String In demoTargets
            Dim st As CompoundStructure = router.GetCompound(targetId)

            If st Is Nothing Then
                Console.WriteLine($"=== {targetId}：该化合物在演示网络中没有可用结构")
                Continue For
            End If

            Dim report As PathReport = Nothing

            Try
                report = router.FindPathway(st.Smiles)
            Catch ex As Exception
                Console.WriteLine($"=== {targetId}：搜索失败 {ex.Message}")
                Continue For
            End Try

            PrintReport(targetId, st, report)
        Next
    End Sub

    ' -------------------------------------------------------------------------
    ' Part B：真实 EcoCyc 库 → 内部模型 → 规模化验证 + 与 BioCycAdapter 对拍
    ' -------------------------------------------------------------------------
    Private Sub PartB_RealDatabase()
        Const db As String = "F:\ecoli\29.0"

        Console.WriteLine()
        Console.WriteLine("Part B. 真实 EcoCyc 库 → 内部代谢模型（规模化验证与 BioCycAdapter 对拍）")
        Console.WriteLine(New String("-"c, 78))

        If Not $"{db}\data".DirectoryExists Then
            Console.WriteLine($"   未找到真实库 {db}，Part B 已优雅跳过（可把脚本中的 db 常量改成本地 BioCyc 库路径）。")
            Return
        End If

        ' 1) BioCyc → 内部标准模型
        Dim biocyc As Workspace = Workspace.Open(db)
        Dim mCompounds As List(Of MetabolicCompound) = BioCycMetabolicConvertor.GetCompounds(biocyc)
        Dim mReactions As List(Of MetabolicReaction) = BioCycMetabolicConvertor.GetReactions(biocyc)

        Console.WriteLine($"   转换：MetabolicCompound {mCompounds.Count} 个，MetabolicReaction {mReactions.Count} 条")

        ' 2) 内部模型 → MetabolicAdapter
        Dim opts As New SearchOptions With {
            .Strategy = "beam",
            .BeamWidth = 50,
            .MaxDepth = 6,
            .MaxPaths = 5,
            .MatchLimit = 20
        }

        Dim sw = Diagnostics.Stopwatch.StartNew()
        Dim router As New MetabolicAdapter(mCompounds, mReactions, opts,
                                           sinkMode:=SinkModes.Core, coreDegree:=4)
        sw.Stop()

        PrintStats("MetabolicAdapter", router, sw.Elapsed.TotalMilliseconds)
        PrintRejectReasons(router)
        BuildNameIndex(router.Compounds)

        ' 3) 与 BioCycAdapter 对拍（两套适配器共用同一通用引擎）
        Dim bioCycRouter As New BioCycAdapter(biocyc, opts, sinkMode:=SinkModes.Core, verbose:=False)

        Console.WriteLine()
        Console.WriteLine("   对拍（MetabolicAdapter vs BioCycAdapter，同一份数据源、同一通用引擎）：")
        Console.WriteLine($"   {"目标",-14} {"规则数",10} {"汇大小",10} {"内部模型路径",14} {"BioCyc 路径",14}   一致性")

        Dim agree As Integer = 0
        Dim total As Integer = 0

        For Each t In realTargets
            Dim mReport As PathReport = Nothing
            Dim bReport As PathReport = Nothing

            Try
                mReport = router.FindPathwayById(t.id)
            Catch ex As Exception
                mReport = Nothing
            End Try

            Try
                bReport = bioCycRouter.FindPathwayById(t.id)
            Catch ex As Exception
                bReport = Nothing
            End Try

            Dim mp = If(mReport Is Nothing, -1, mReport.Stats.PathsFound)
            Dim bp = If(bReport Is Nothing, -1, bReport.Stats.PathsFound)
            Dim same As Boolean = (mp = bp)
            If same Then agree += 1
            total += 1

            Dim rules = If(mReport Is Nothing, 0, mReport.Parameters.NumRules)
            Dim sinkSz = If(mReport Is Nothing, 0, mReport.Parameters.SinkSize)

            Console.WriteLine($"   {If(t.name.Length > 12, t.name.Substring(0, 12), t.name),-14} {rules,10} {sinkSz,10} {mp,14} {bp,14}   {If(same, "OK", "MISMATCH")}")
        Next

        Console.WriteLine($"   一致性：{agree}/{total}")
    End Sub

    ' -------------------------------------------------------------------------
    ' 输出辅助
    ' -------------------------------------------------------------------------
    Private Sub PrintStats(tag As String, router As MetabolicAdapter, elapsedMs As Double)
        Console.Error.WriteLine($"[{tag}] 化合物 {router.Stats("compounds")}（可用结构 {router.Stats("structures")}，" &
                                $"无 SMILES {router.Stats("no_smiles")}，不可用 {router.Stats("bad_smiles")}）")
        Console.Error.WriteLine($"[{tag}] 反应 {router.Stats("reactions")} → 广义规则 {router.Stats("rules")}")
        Console.Error.WriteLine($"[{tag}] 汇集合 = {router.Stats("sink")}（模式 {router.Stats("sink_mode")}，" &
                                $"coreDegree≥{router.Stats("core_degree")}），装配耗时 {elapsedMs:F0}ms")
    End Sub

    Private Sub PrintRejectReasons(router As MetabolicAdapter)
        ' 把跳过原因以控制台（stdout）形式一并展示，便于检查手写数据的质量
        Dim all As New Dictionary(Of String, Integer)(router.Skipped)

        If all.Count > 0 Then
            Console.WriteLine("   规则挖掘跳过原因：" & String.Join(", ",
                all.OrderByDescending(Function(kv) kv.Value).Select(Function(kv) $"{kv.Key}={kv.Value}")))
        End If

        If router.RuleTrace.Count > 0 Then
            Console.WriteLine("   未收录反应明细：" & String.Join(", ",
                router.RuleTrace.Select(Function(kv) $"{kv.Key}({kv.Value})")))
        End If
    End Sub

    Private Sub BuildNameIndex(compounds As Dictionary(Of String, CompoundStructure))
        keyToName.Clear()

        For Each kvp In compounds
            Dim key As String = Nothing
            Try
                key = kvp.Value.Mol.MolKey()
            Catch ex As Exception
                Continue For
            End Try
            If Not keyToName.ContainsKey(key) Then keyToName(key) = kvp.Key
        Next
    End Sub

    Private Sub PrintReport(targetId As String, st As CompoundStructure, report As PathReport)
        Console.WriteLine($"=== {targetId} ===")
        Console.WriteLine($"   目标     : {st.Smiles}")
        Console.WriteLine($"   统计     : 路径 {report.Stats.PathsFound} 条，展开状态 {report.Stats.StatesGenerated}，" &
                          $"规则应用 {report.Stats.ApplicationsTried} 次，耗时 {report.Stats.ElapsedMs}ms")

        If report.Paths.Count = 0 Then
            Console.WriteLine("   结果     : 未找到完整通路")
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

    Private Function JoinNames(smilesList As String()) As String
        Dim parts As New List(Of String)()

        For Each s As String In smilesList
            Dim nm As String = Nothing
            Try
                Dim key As String = SmilesIO.Parse(s).MolKey()
                keyToName.TryGetValue(key, nm)
            Catch ex As Exception
                nm = Nothing
            End Try
            parts.Add(If(nm Is Nothing, s, nm))
        Next

        Return String.Join(" + ", parts)
    End Function

End Module
