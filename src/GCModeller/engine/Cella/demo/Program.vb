' ============================================================
' Program.vb - Cella 虚拟细胞演示（一键跑完 + 导出 CSV）
' ============================================================
' 流程：
'   1. 合成数据（TF 先验网络 / 基线表达矩阵 / 代谢反应网络 / 时序训练集）
'   2. 构建培养皿形状的环境与培养基
'   3. 训练 GEARS 图神经网络（共享一份，供所有细胞推理）
'   4. 拟合 Metaboliq 液态神经网络模板（逐细胞复制参数）
'   5. 播种细胞并推进若干时间步
'   6. 控制台打印关键统计 + 导出 CSV 到 demo/result/
' ============================================================

Imports Cella
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.GEARS
Imports SMRUCC.genomics.Analysis.Metaboliq

Module Program

    Const N_TICKS As Integer = 60
    Const PETRI_RADIUS As Integer = 3
    Const PETRI_HEIGHT As Integer = 1

    Function Main(args As String()) As Integer
        Try
            Call Run()

            Return 0
        Catch ex As Exception
            Console.WriteLine()
            Console.WriteLine("演示执行失败:")
            Console.WriteLine(ex.ToString())

            Return 1
        End Try
    End Function

    Private Sub Run()
        Call Report.Section("GCModeller Cella - 虚拟细胞系统演示")

        ' ==================== 1. 合成数据 ====================
        Call Report.Section("阶段 1 / 6  合成演示数据")

        Dim genes As String() = SyntheticData.GeneSet()
        Dim prior = SyntheticData.BuildPrior()
        Dim expression = SyntheticData.BuildExpression(genes)
        Dim reactions = SyntheticData.BuildReactions()
        Dim boundary As String() = SyntheticData.BoundaryMetabolites()

        Call Report.KeyValue("基因数量", genes.Length)
        Call Report.KeyValue("转录因子数量", SyntheticData.TranscriptionFactors.Length)
        Call Report.KeyValue("先验调控边数", prior.Edges.Count)
        Call Report.KeyValue("代谢反应数量", reactions.Length)
        Call Report.KeyValue("胞外（边界）代谢物", String.Join(", ", boundary))

        Dim blueprint As New CellaBlueprint With {
            .Prior = prior,
            .Expression = expression,
            .Reactions = reactions,
            .ExplicitBoundary = boundary,
            .ReactionGeneMap = SyntheticData.ReactionGeneMap(),
            .Transporters = SyntheticData.TransporterMap(),
            .Exporters = SyntheticData.ExporterMap(),
            .Effectors = SyntheticData.EffectorMap(),
            .ExternalStimuli = SyntheticData.StimulusMap(),
            .TFGenes = SyntheticData.TranscriptionFactors,
            .SignalChannels = SyntheticData.TranscriptionFactors,
            .RecycleTargetMetabolite = "aa_pool",
            .TimeStep = 1.0,
            .GearsConfig = New GEARSConfig With {
                .EmbeddingDim = 16,
                .HiddenDim = 32,
                .NumLayers = 2,
                .Epochs = 30,
                .LearningRate = 0.003F,
                .NSinglePerturbation = 24,
                .NComboPerturbation = 12,
                .ComboSize = 2,
                .PrintEvery = 10,
                .Seed = 2024
            }
        }

        Call blueprint.Validate()

        Dim graph As MetabolicNetworkGraph = CellaFactory.GetMetabolicGraph(blueprint)

        Call Report.KeyValue("胞内代谢物（液态神经元数）", graph.MetaboliteCount)
        Call Report.KeyValue("反应通道数", graph.ReactionCount)
        Call Report.KeyValue("边界通道数", graph.BoundaryCount)
        Call Report.KeyValue("液态网络输入维度", graph.InputSize)

        ' ==================== 2. 环境 ====================
        Call Report.Section("阶段 2 / 6  构建培养空间")

        Dim medium As Dictionary(Of String, Double) = SpaceInitializer.CreateMedium(
            ("glc_e", 10.0), ("o2_e", 8.0), ("pi_e", 5.0),
            ("lac_e", 0.0), ("etoh_e", 0.0), ("ac_e", 0.0), ("co2_e", 0.0)
        )

        Dim env As Environment = SpaceInitializer.CreatePetriDishSpace(
            radius:=PETRI_RADIUS,
            height:=PETRI_HEIGHT,
            medium:=medium,
            timeStep:=blueprint.TimeStep
        )

        Call Report.KeyValue("形状", $"培养皿 radius={PETRI_RADIUS}, height={PETRI_HEIGHT}")
        Call Report.KeyValue("有效格点数", env.Volume)
        Call Report.KeyValue("培养基初始配方", String.Join(", ", medium.Select(Function(m) $"{m.Key}={m.Value:F1}")))

        ' ==================== 3. 训练转录调控网络 ====================
        Call Report.Section("阶段 3 / 6  训练 GEARS 图神经网络（转录调控）")

        Dim gears As GEARS = CellaFactory.TrainGears(blueprint)

        Call Report.KeyValue("图中实际生效的先验边", gears.GraphData.NumPriorEdges)
        Call Report.KeyValue("训练样本数", gears.TrainingSamples.Count)
        Call Report.KeyValue("训练轮数", gears.Options.Epochs)

        If Not gears.LossCurve.IsNullOrEmpty Then
            Call Report.KeyValue("损失 首/末", $"{gears.LossCurve(0):F6} -> {gears.LossCurve(gears.LossCurve.Length - 1):F6}")
        End If

        ' ==================== 4. 拟合代谢网络 ====================
        Call Report.Section("阶段 4 / 6  拟合 Metaboliq 液态神经网络（代谢）")

        Dim training As MetabolicTrainingSet = SyntheticData.BuildTrainingSet(graph, nSteps:=40, dt:=1.0)
        Dim template As MetabolicLiquidNetwork = CellaFactory.TrainMetabolicTemplate(blueprint, training)

        Call Report.KeyValue("液态神经元模式", template.Mode.ToString())
        Call Report.KeyValue("ODE 积分器", template.Liquid.SolverType)
        Call Report.KeyValue("被拓扑掩码掉的连接比例", $"{template.MaskedRatio():F3}")

        If Not blueprint.MetabolicLoss.IsNullOrEmpty Then
            Dim last As EpochLoss = blueprint.MetabolicLoss(blueprint.MetabolicLoss.Count - 1)

            Call Report.KeyValue("拟合轮数", blueprint.MetabolicLoss.Count)
            Call Report.KeyValue("末轮损失 数据项", last.Data.ToString("F6"))
            Call Report.KeyValue("末轮损失 质量守恒项", last.Mass.ToString("F6"))
        End If

        ' ==================== 5. 播种并推进 ====================
        Call Report.Section("阶段 5 / 6  播种细胞并推进仿真")

        Dim nCells As Integer = CellaFactory.SeedCells(env, blueprint, cellsPerSpot:=1)

        Call Report.KeyValue("播种细胞数", nCells)
        Call Report.KeyValue("推进步数", N_TICKS)

        Dim times As New List(Of Double)()
        Dim metaboliteSeries As New List(Of Dictionary(Of String, Double))()
        Dim rnaSeries As New List(Of Dictionary(Of String, Double))()
        Dim proteinSeries As New List(Of Dictionary(Of String, Double))()
        Dim fluxSeries As New List(Of Dictionary(Of String, Double))()
        Dim signalSeries As New List(Of Dictionary(Of String, Double))()
        Dim mediumSeries As New List(Of Dictionary(Of String, Double))()
        Dim spot0 As Spot = env.GetAllSpots().FirstOrDefault()

        For tick As Integer = 1 To N_TICKS
            Call env.Tick()

            Dim cells As VirtualCella() = env.GetAllCells().ToArray()

            metaboliteSeries.Add(Report.Average(cells.Select(Function(c) c.State.AsDictionary(StatePool.Metabolite))))
            rnaSeries.Add(Report.Average(cells.Select(Function(c) c.State.AsDictionary(StatePool.mRNA))))
            proteinSeries.Add(Report.Average(cells.Select(Function(c) c.State.AsDictionary(StatePool.Protein))))
            signalSeries.Add(Report.Average(cells.Select(Function(c) c.State.AsDictionary(StatePool.Signal))))
            fluxSeries.Add(Report.Average(cells.Select(Function(c) FluxTable(c))))

            If spot0 IsNot Nothing AndAlso spot0.Medium IsNot Nothing Then
                mediumSeries.Add(New Dictionary(Of String, Double)(spot0.Medium, StringComparer.OrdinalIgnoreCase))
            End If

            times.Add(env.CurrentTime)

            If tick Mod 10 = 0 Then
                Dim m As Dictionary(Of String, Double) = metaboliteSeries(metaboliteSeries.Count - 1)
                Dim r As Dictionary(Of String, Double) = rnaSeries(rnaSeries.Count - 1)

                Console.WriteLine($"  [t={env.CurrentTime,6:F1}] " &
                    $"atp={V(m, "atp"),7:F3} nadh={V(m, "nadh"),7:F3} pyr={V(m, "pyr"),7:F3} " &
                    $"aa={V(m, "aa_pool"),7:F3} | glc_e={V(mediumSeries.Last(), "glc_e"),7:F3} " &
                    $"| crp_mRNA={V(r, "crp"),7:F2} ptsG_mRNA={V(r, "ptsG"),7:F2}")
            End If
        Next

        ' ==================== 6. 汇总与导出 ====================
        Call Report.Section("阶段 6 / 6  汇总统计与结果导出")

        Dim lastMetabolite As Dictionary(Of String, Double) = metaboliteSeries.Last()
        Dim lastRna As Dictionary(Of String, Double) = rnaSeries.Last()
        Dim lastFlux As Dictionary(Of String, Double) = fluxSeries.Last()
        Dim lastSignal As Dictionary(Of String, Double) = signalSeries.Last()

        Call Report.Line("  -- 关键代谢物浓度（归一化空间，全体细胞均值）--")

        For Each id In {"g6p", "fdp", "pep", "pyr", "accoa", "cit", "akg", "succ", "atp", "adp", "nad", "nadh", "aa_pool", "lac", "etoh", "ac"}
            If lastMetabolite.ContainsKey(id) Then
                Call Report.KeyValue(id, lastMetabolite(id).ToString("F4"))
            End If
        Next

        Call Report.Line()
        Call Report.Line("  -- 关键反应通量 --")

        For Each id In {"PTS", "GLK", "PFK", "PYK", "PDH", "CS", "ICD", "CYTBO3", "ATPS4R", "LDH", "ADH", "GDH"}
            If lastFlux.ContainsKey(id) Then
                Call Report.KeyValue(id, lastFlux(id).ToString("F4"))
            End If
        Next

        Call Report.Line()
        Call Report.Line("  -- 转录因子活性与关键基因表达 --")

        For Each tf In SyntheticData.TranscriptionFactors
            Call Report.KeyValue($"TF {tf} 活性", V(lastSignal, tf).ToString("F4"))
        Next

        For Each g In {"ptsG", "pfkA", "pykA", "gltA", "icd", "cyoA", "ldhA", "adhE"}
            Call Report.KeyValue($"mRNA {g}", V(lastRna, g).ToString("F4"))
        Next

        Call Report.Line()
        Call Report.Line("  -- 环境（中心格点的培养基）--")

        For Each id In SyntheticData.BoundaryMetabolites()
            Call Report.KeyValue(id, V(mediumSeries.Last(), id).ToString("F4"))
        Next

        ' ---- 导出 CSV ----
        Dim dir As String = Report.ResultDirectory()

        Call Report.SaveSeries(System.IO.Path.Combine(dir, "metabolites.csv"), "time", times, metaboliteSeries, graph.InternalIds)
        Call Report.SaveSeries(System.IO.Path.Combine(dir, "expression.csv"), "time", times, rnaSeries, genes)
        Call Report.SaveSeries(System.IO.Path.Combine(dir, "proteins.csv"), "time", times, proteinSeries, genes)
        Call Report.SaveSeries(System.IO.Path.Combine(dir, "fluxes.csv"), "time", times, fluxSeries, graph.ReactionIds)
        Call Report.SaveSeries(System.IO.Path.Combine(dir, "signals.csv"), "time", times, signalSeries, SyntheticData.TranscriptionFactors)
        Call Report.SaveSeries(System.IO.Path.Combine(dir, "medium.csv"), "time", times, mediumSeries, SyntheticData.BoundaryMetabolites())

        If Not gears.LossCurve.IsNullOrEmpty Then
            Call Report.SaveCurve(System.IO.Path.Combine(dir, "gears_loss.csv"), "epoch,loss", gears.LossCurve)
        End If

        If Not blueprint.MetabolicLoss.IsNullOrEmpty Then
            Call Report.SaveTable(
                System.IO.Path.Combine(dir, "metaboliq_loss.csv"),
                {"epoch", "data", "mass", "thermo", "flux", "total"},
                blueprint.MetabolicLoss.Select(Function(l, i) New String() {
                    i.ToString(), l.Data.ToString("F8"), l.Mass.ToString("F8"),
                    l.Thermo.ToString("F8"), l.Flux.ToString("F8"), l.Total.ToString("F8")
                })
            )
        End If

        ' ---- 子系统健康度 ----
        Dim failures = env.GetAllCells() _
            .SelectMany(Function(c) c.SubNetworks()) _
            .Where(Function(n) n IsNot Nothing) _
            .GroupBy(Function(n) n.Name) _
            .Select(Function(g) $"{g.Key}={g.Sum(Function(n) n.FailedSteps)}")

        Dim odeStatus = env.GetAllCells() _
            .SelectMany(Function(c) c.SubNetworks().OfType(Of OdeSubNetwork)()) _
            .GroupBy(Function(n) n.Name) _
            .Select(Function(g) $"{g.Key}={g.First().LastStatus}")

        Call Report.Line()
        Call Report.KeyValue("子系统推进失败计数", String.Join(", ", failures))
        Call Report.KeyValue("CVODE 末次返回状态", String.Join(", ", odeStatus))
        Call Report.KeyValue("结果输出目录", dir)
        Call Report.Line()
        Call Report.Line("演示完成。")
    End Sub

    Private Function FluxTable(cella As VirtualCella) As Dictionary(Of String, Double)
        Dim out As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)
        Dim ids As String() = cella.metabolic.Graph.ReactionIds
        Dim v As Double() = cella.metabolic.Fluxes

        For i As Integer = 0 To ids.Length - 1
            out(ids(i)) = v(i)
        Next

        Return out
    End Function

    Private Function V(table As Dictionary(Of String, Double), key As String) As Double
        Dim x As Double = 0.0

        If table IsNot Nothing Then
            table.TryGetValue(key, x)
        End If

        Return x
    End Function

End Module
