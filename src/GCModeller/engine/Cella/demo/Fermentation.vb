' ============================================================
' Fermentation.vb - 多物种发酵群落演示
' ============================================================
' 一键跑完的流程：
'   1. 合成数据 + 装配 4 个代谢分工物种的蓝图
'   2. 逐物种训练 GEARS（转录调控）与 Metaboliq 液态网络（代谢）
'   3. 构建锥形摇瓶发酵空间与培养基
'   4. 混合接种 4 个物种并推进若干时间步（含扩散 / 分裂 / 死亡 / 鞭毛运动）
'   5. 导出：每个 Spot 的细胞数量分布、代际谱系进化树、Spot 内交叉喂养代谢流
' ============================================================

Imports Cella
Imports System.Diagnostics
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.GEARS
Imports SMRUCC.genomics.Analysis.Metaboliq

Public Module Fermentation

    Const N_TICKS As Integer = 200
    Const REPORT_EVERY As Integer = 15
    Const TOTAL_SEED_CELLS As Integer = 12

    ' 锥形摇瓶形状
    Const BOTTOM_RADIUS As Integer = 2
    Const NECK_RADIUS As Integer = 1
    Const CONE_HEIGHT As Integer = 2
    Const NECK_HEIGHT As Integer = 1

    Public Function Run() As Integer
        Call Report.Section("GCModeller Cella - 多物种发酵群落演示")

        ' ==================== 1. 合成数据与物种装配 ====================
        Call Report.Section("阶段 1 / 6  装配发酵群落物种")

        Dim definitions As SpeciesDefinition() = Species.Definitions()

        For Each def As SpeciesDefinition In definitions
            Dim graph As MetabolicNetworkGraph = CellaFactory.GetMetabolicGraph(def.Blueprint)

            Call Report.KeyValue($"物种 {def.id}",
                $"{def.name} | 基因 {def.Genes.Length} | 反应 {graph.ReactionCount} | " &
                $"胞内代谢物 {graph.MetaboliteCount} | 胞外 {graph.BoundaryCount} | 播种比例 {def.Fraction:P0}")
        Next

        ' ==================== 2. 训练各物种的共享模型 ====================
        Call Report.Section("阶段 2 / 6  训练各物种的共享模型")

        For Each def As SpeciesDefinition In definitions
            Dim blueprint As CellaBlueprint = def.Blueprint
            Dim gears As GEARS = CellaFactory.TrainGears(blueprint)

            Dim data As MetabolicTrainingSet = SyntheticData.BuildTrainingSet(
                CellaFactory.GetMetabolicGraph(blueprint), nSteps:=30, dt:=1.0)

            data.Config.Epochs = 25
            data.Config.LogEvery = 100
            data.Config.Verbose = False

            Dim model As MetabolicLiquidNetwork = CellaFactory.TrainMetabolicTemplate(blueprint, data)

            Dim loss As String = "(无)"

            If Not blueprint.MetabolicLoss.IsNullOrEmpty Then
                Dim last As EpochLoss = blueprint.MetabolicLoss(blueprint.MetabolicLoss.Count - 1)

                loss = last.Total.ToString("F6")
            End If

            Call Report.KeyValue($"{def.id} 训练完成",
                $"{gears.GraphData.NumPriorEdges} 条先验边 / {gears.TrainingSamples.Count} 样本 / " &
                $"GEARS 末损失 {If(gears.LossCurve.IsNullOrEmpty, 0.0, gears.LossCurve.Last()):F6} / " &
                $"Metaboliq 末损失 {loss} / 掩码率 {model.MaskedRatio():F3}")
        Next

        ' ==================== 3. 发酵环境 ====================
        Call Report.Section("阶段 3 / 6  构建发酵环境")

        Dim medium As Dictionary(Of String, Double) = SpaceInitializer.CreateMedium(
            ("glc_e", 20.0), ("o2_e", 2.0), ("pi_e", 5.0), ("nh4_e", 1.0),
            ("aa_e", 0.0), ("lac_e", 0.0), ("etoh_e", 0.0), ("ac_e", 0.0),
            ("co2_e", 0.0), ("ppa_e", 0.0), ("but_e", 0.0), ("h2_e", 0.0)
        )

        Dim env As Cella.Environment = SpaceInitializer.CreateFlaskSpace(
            bottomRadius:=BOTTOM_RADIUS,
            neckRadius:=NECK_RADIUS,
            coneHeight:=CONE_HEIGHT,
            neckHeight:=NECK_HEIGHT,
            medium:=medium,
            timeStep:=1.0
        )

        ' 扩散 / 补料配置取自第一个物种（各物种这些参数一致）
        env.Diffusion = DiffusionConfig.FromBlueprint(definitions(0).Blueprint)

        Dim blueprints As CellaBlueprint() = definitions.Select(Function(d) d.Blueprint).ToArray()

        Call CellaFactory.EnsureMediumCoverage(env, blueprints)

        Call Report.KeyValue("形状", $"锥形摇瓶 bottomR={BOTTOM_RADIUS}, neckR={NECK_RADIUS}, cone={CONE_HEIGHT}, neck={NECK_HEIGHT}")
        Call Report.KeyValue("有效格点数", env.Volume)
        Call Report.KeyValue("培养基初始配方", String.Join(", ", medium.Where(Function(m) m.Value > 0).Select(Function(m) $"{m.Key}={m.Value:F1}")))
        Call Report.KeyValue("扩散系数", env.Diffusion.Coefficient)
        Call Report.KeyValue("贴壁补料", $"rate={env.Diffusion.FeedRate}, 成分={String.Join("/", env.Diffusion.FeedMetabolites)}")

        ' ==================== 4. 混合接种 ====================
        Call Report.Section("阶段 4 / 6  混合接种")

        Dim seeded As Integer = Species.SeedCommunity(env, definitions, totalCells:=TOTAL_SEED_CELLS)

        Call Report.KeyValue("接种细胞数", seeded)
        Call Report.KeyValue("初始物种分布", Fmt.Population(env.PopulationBySpecies()))

        ' ==================== 5. 群落演化 ====================
        Call Report.Section("阶段 5 / 6  群落演化")

        Dim sw As Stopwatch = Stopwatch.StartNew()
        Dim crossFeeding As New CrossFeedingRecorder()
        Dim populationSamples As New List(Of PopulationSample)()
        Dim communitySeries As New List(Of Dictionary(Of String, Double))()
        Dim times As New List(Of Double)()
        Dim migrations As New List(Of MigrationEvent)()
        Dim totalDivisions As Integer = 0
        Dim totalDeaths As Integer = 0

        For tick As Integer = 1 To N_TICKS
            Call env.Tick()

            totalDivisions += env.LastEvents.Divisions
            totalDeaths += env.LastEvents.Deaths

            If Not env.LastMigrations.IsNullOrEmpty Then
                migrations.AddRange(env.LastMigrations)
            End If

            Call crossFeeding.Collect(env, env.CurrentTime)

            times.Add(env.CurrentTime)
            communitySeries.Add(SampleCommunity(env, definitions))
            Call SamplePopulation(env, populationSamples)

            If tick Mod REPORT_EVERY = 0 OrElse tick = 1 Then
                Call PrintProgress(env, tick, crossFeeding)
            End If
        Next

        sw.Stop()

        Dim failures = env.GetAllCells() _
            .SelectMany(Function(c) c.SubNetworks()) _
            .Where(Function(n) n IsNot Nothing) _
            .GroupBy(Function(n) n.Name) _
            .Select(Function(g) $"{g.Key}={g.Sum(Function(n) n.FailedSteps)}")

        Call Report.Line()
        Call Report.KeyValue("演化耗时", $"{sw.Elapsed.TotalSeconds:F1} s（{sw.Elapsed.TotalMilliseconds / N_TICKS:F2} ms/step）")
        Call Report.KeyValue("子系统推进失败计数", String.Join(", ", failures))
        Call Report.KeyValue("累计分裂次数", totalDivisions)
        Call Report.KeyValue("累计死亡次数", totalDeaths)
        Call Report.KeyValue("累计迁移次数", migrations.Count)
        Call Report.KeyValue("谱系登记细胞数", env.Lineage.Count)
        Call Report.KeyValue("存活细胞数", env.Lineage.AliveCount())
        Call Report.KeyValue("最大代次", env.Lineage.MaxGeneration)
        Call Report.KeyValue("平均代次（存活）", env.Lineage.MeanGeneration().ToString("F2"))
        Call Report.KeyValue("死因统计", Fmt.Population(env.Lineage.DeathsByCause()))
        Call Report.KeyValue("末端物种分布", Fmt.Population(env.PopulationBySpecies()))

        ' ==================== 6. 导出 ====================
        Call Report.Section("阶段 6 / 6  结果导出")

        Dim dir As String = Report.ResultDirectory()

        ' ---- (1) 每个 Spot 的细胞数量分布 ----
        Call System.IO.File.WriteAllText(
            System.IO.Path.Combine(dir, "spot_population.csv"),
            Report.SpotPopulationCsv(populationSamples))

        Call Report.SaveSeries(
            System.IO.Path.Combine(dir, "population.csv"),
            "time", times,
            times.Select(Function(t, i) AggregateSpecies(populationSamples, t)).ToList(),
            definitions.Select(Function(d) d.id).ToArray())

        ' ---- (2) 代际生长繁殖的进化树 ----
        Call System.IO.File.WriteAllText(
            System.IO.Path.Combine(dir, "lineage.csv"), env.Lineage.ToCsv())
        Call System.IO.File.WriteAllText(
            System.IO.Path.Combine(dir, "lineage.nwk"), env.Lineage.ToNewick(env.CurrentTime))

        ' ---- (3) 交叉喂养网络代谢流 ----
        Call System.IO.File.WriteAllText(
            System.IO.Path.Combine(dir, "crossfeeding_flux.csv"), crossFeeding.ToSpotCsv())
        Call System.IO.File.WriteAllText(
            System.IO.Path.Combine(dir, "crossfeeding_summary.csv"), crossFeeding.ToSummaryCsv())
        Call System.IO.File.WriteAllText(
            System.IO.Path.Combine(dir, "crossfeeding_timeline.csv"), crossFeeding.ToTimelineCsv())

        ' ---- (4) 迁移事件 ----
        Call System.IO.File.WriteAllText(
            System.IO.Path.Combine(dir, "migration.csv"), Report.MigrationCsv(migrations))

        ' ---- (5) 群落状态时间序列 ----
        Dim communityColumns As String() = NutrientMetaboliteIds() _
            .Concat(NutrientMetaboliteIds().Select(Function(id) id & "_int")) _
            .Concat({"cell_count_total", "nutrient_mean"}) _
            .Concat(definitions.Select(Function(d) "cell_count_" & d.id)) _
            .ToArray()

        Call Report.SaveSeries(
            System.IO.Path.Combine(dir, "community.csv"),
            "time", times, communitySeries, communityColumns)

        ' ---- (6) 每个 Spot 的营养 / 占用快照 ----
        Call System.IO.File.WriteAllText(
            System.IO.Path.Combine(dir, "spot_occupancy.txt"), Report.NutrientMap(env, definitions(0).Blueprint, env.CurrentTime))

        ' ---- 控制台汇总 ----
        Call Report.Line()
        Call Report.Line(Report.NutrientMap(env, definitions(0).Blueprint, env.CurrentTime))

        Call Report.Line()
        Call Report.Line("  -- 交叉喂养网络（物种级，按累计通量降序 Top 15）--")

        For Each edge In crossFeeding.SummaryEdges.Take(15)
            Call Report.KeyValue($"{edge.producer} → {edge.consumer}", $"{edge.metabolite}  flux={edge.flux:F4}")
        Next

        Call Report.Line()
        Call Report.KeyValue("交叉喂养累计总通量", crossFeeding.TotalFlux.ToString("F4"))
        Call Report.KeyValue("交叉喂养边数（物种级）", crossFeeding.SummaryEdges.Count())
        Call Report.KeyValue("交叉喂养边数（Spot 级）", crossFeeding.SpotEdges.Count())
        Call Report.KeyValue("谱系树文件", "lineage.nwk")
        Call Report.KeyValue("结果输出目录", dir)
        Call Report.Line()
        Call Report.Line("发酵演示完成。")

        Return 0
    End Function

    Private Function NutrientMetaboliteIds() As String()
        Return Nutrients.Ids
    End Function

    Private Sub PrintProgress(env As Cella.Environment, tick As Integer, crossFeeding As CrossFeedingRecorder)
        Dim population As Dictionary(Of String, Integer) = env.PopulationBySpecies()
        Dim nutrients As Double = env.GetAllSpots() _
            .Select(Function(s) s.NutrientLevel(Nothing)) _
            .DefaultIfEmpty(0.0) _
            .Average()

        Console.WriteLine(
            $"  [t={env.CurrentTime,6:F1}] cells={env.GetAllCells().Count(),4} " &
            $"pop={Fmt.Population(population),-42} " &
            $"nutrient={nutrients,7:F2} xfeed={crossFeeding.LastStepFlux,8:F4} " &
            $"gen={env.Lineage.MaxGeneration}")
    End Sub

    ''' <summary>把整个群落的平均状态整理成一条时间序列记录</summary>
    Private Function SampleCommunity(env As Cella.Environment, definitions As SpeciesDefinition()) As Dictionary(Of String, Double)
        Dim cells As VirtualCella() = env.GetAllCells().ToArray()
        Dim out As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

        ' 胞外：所有 Spot 的平均浓度
        If env.Volume > 0 Then
            Dim spots As Spot() = env.GetAllSpots().ToArray()

            For Each id As String In Nutrients.Ids
                Dim sum As Double = 0.0

                For Each spot As Spot In spots
                    Dim level As Double = 0.0

                    Call spot.Medium.TryGetValue(id, level)
                    sum += level
                Next

                out(id) = sum / spots.Length
            Next

            out("nutrient_mean") = spots.Select(Function(s) s.NutrientLevel(definitions(0).Blueprint)).Average()
        End If

        ' 胞内：所有细胞的平均值
        For Each id As String In NutrientMetaboliteIds()
            Dim sum As Double = 0.0
            Dim hits As Integer = 0

            For Each cella As VirtualCella In cells
                Dim idx As Integer = -1

                If cella.State.MetaboliteIndex.TryGetValue(id, idx) Then
                    sum += cella.State.Metabolite(idx)
                    hits += 1
                End If
            Next

            If hits > 0 Then
                out(id & "_int") = sum / hits
            End If
        Next

        out("cell_count_total") = cells.Length

        For Each def As SpeciesDefinition In definitions
            Dim n As Integer = 0

            For Each cella As VirtualCella In cells
                If String.Equals(cella.Species, def.id, StringComparison.OrdinalIgnoreCase) Then
                    n += 1
                End If
            Next

            out("cell_count_" & def.id) = n
        Next

        Return out
    End Function

    Private Sub SamplePopulation(env As Cella.Environment, samples As List(Of PopulationSample))
        Dim time As Double = env.CurrentTime

        For Each spot As Spot In env.GetAllSpots()
            For Each item In spot.PopulationBySpecies()
                samples.Add(New PopulationSample With {
                    .time = time,
                    .x = spot.index.X,
                    .y = spot.index.Y,
                    .z = spot.index.Z,
                    .species = item.Key,
                    .cell_count = item.Value
                })
            Next
        Next
    End Sub

    Private Function AggregateSpecies(samples As List(Of PopulationSample), time As Double) As Dictionary(Of String, Double)
        Dim out As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

        For Each s In samples
            If s.time <> time Then
                Continue For
            End If

            Dim n As Double = 0.0

            Call out.TryGetValue(s.species, n)
            out(s.species) = n + s.cell_count
        Next

        Return out
    End Function

End Module
