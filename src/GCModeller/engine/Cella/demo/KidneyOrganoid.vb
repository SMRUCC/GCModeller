' ============================================================
' KidneyOrganoid.vb - 人肾虚拟类器官演示
' ============================================================
' 一键跑完的流程：
'   1. 装配 8 种人肾细胞命运（基因集 / 代谢网络 / 调控网络 / 几何偏好）
'   2. 逐命运训练 GEARS（转录调控）与 Metaboliq 液态网络（代谢）
'   3. 构建球形类器官空间：只有表层浸润培养基，营养必须扩散进核心
'   4. 在核心播种后肾间充质祖细胞（NPC）
'   5. 按培养方案分阶段换液，推进 60 个时间步（≈ 培养天数）：
'        生长因子节拍 + 邻域诱导/侧向抑制 + 径向位置 → 细胞命运决定
'        终末分化细胞按径向偏好做低速率位置校正（自组织分带）
'   6. 导出：细胞组成、空间分布、径向分区、分化谱系、代谢交换、类器官指标
' ============================================================

Imports Cella
Imports System.Diagnostics
Imports SMRUCC.genomics.Analysis.Metaboliq
Imports Microsoft.VisualBasic.Linq

Public Module KidneyOrganoid

    Const N_TICKS As Integer = 60
    Const SPHEROID_RADIUS As Integer = 3
    Const SEED_CELLS As Integer = 16
    Const SEED_RADIUS As Double = 0.50
    Const REPORT_EVERY As Integer = 5
    Const RADIAL_BANDS As Integer = 5

    ''' <summary>径向分带的标签</summary>
    Public ReadOnly Property BandLabels As String()
        Get
            Return {"核心 (r<0.2)", "内层 (0.2-0.4)", "中层 (0.4-0.6)", "外层 (0.6-0.8)", "表层 (r≥0.8)"}
        End Get
    End Property

    Public Function Run() As Integer
        Call Report.Section("GCModeller Cella - 人肾虚拟类器官演示")

        ' ==================== 1. 装配细胞命运 ====================
        Call Report.Section("阶段 1 / 6  装配人肾细胞命运")

        Dim catalog As FateCatalog = HumanKidney.BuildCatalog()

        For Each def As CellFateDefinition In catalog.All
            Dim graph As MetabolicNetworkGraph = CellaFactory.GetMetabolicGraph(def.Blueprint)

            Call Report.KeyValue($"命运 {def.Id}",
                $"{def.Name} | 基因 {def.Blueprint.Genes.Length} | 反应 {graph.ReactionCount} | " &
                $"胞内代谢物 {graph.MetaboliteCount} | 胞外 {graph.BoundaryCount} | " &
                $"偏好半径 {def.Blueprint.PreferredRadius:F2}±{def.Blueprint.RadialTolerance:F2}")
        Next

        Call Report.Line()
        Call Report.Line("  -- 分化谱系 --")

        For Each def As CellFateDefinition In catalog.All
            If Not def.Precursors.IsNullOrEmpty Then
                Call Report.KeyValue($"{String.Join("/", def.Precursors)} → {def.Id}",
                    $"p0={def.BaseProbability:F3} 因子={FormatSignals(def.RequiredSignals)} " &
                    $"诱导={FormatSignals(def.InducedBy)} 抑制={FormatSignals(def.InhibitedBy)}")
            End If
        Next

        ' ==================== 2. 训练共享模型 ====================
        Call Report.Section("阶段 2 / 6  逐命运训练共享模型")

        For Each def As CellFateDefinition In catalog.All
            Dim blueprint As CellaBlueprint = def.Blueprint
            Dim graph As MetabolicNetworkGraph = CellaFactory.GetMetabolicGraph(blueprint)
            Dim gears = CellaFactory.TrainGears(blueprint)

            Dim data As MetabolicTrainingSet = SyntheticData.BuildTrainingSet(graph, nSteps:=24, dt:=1.0)

            data.Config.Epochs = 20
            data.Config.LogEvery = 100
            data.Config.Verbose = False

            Dim model = CellaFactory.TrainMetabolicTemplate(blueprint, data)
            Dim mse As String = "(无)"

            If Not blueprint.MetabolicLoss.IsNullOrEmpty Then
                mse = blueprint.MetabolicLoss.Last().Total.ToString("F6")
            End If

            Call Report.KeyValue($"{def.Id} 训练完成",
                $"{gears.GraphData.NumPriorEdges} 条先验边 / {gears.TrainingSamples.Count} 样本 / " &
                $"GEARS 末损失 {If(gears.LossCurve.IsNullOrEmpty, 0.0, gears.LossCurve.Last()):F6} / " &
                $"Metaboliq 末损失 {mse} / 掩码率 {model.MaskedRatio():F3}")
        Next

        ' ==================== 3. 类器官空间 ====================
        Call Report.Section("阶段 3 / 6  构建球形类器官空间")

        Dim medium As Dictionary(Of String, Double) = CultureProtocol.Recipe(0.0)
        Dim env As Environment = SpaceInitializer.CreateSpheroidSpace(
            radius:=SPHEROID_RADIUS,
            medium:=medium,
            timeStep:=1.0,
            shellRadius:=0.78,
            coreRetention:=0.0
        )

        ' 首次换液：保证所有配方成分（含生长因子）在培养基中存在
        Call CultureProtocol.Apply(env, 0.0)

        Call Report.KeyValue("形状", $"球形类器官 radius={SPHEROID_RADIUS}，表层浸润培养基")
        Call Report.KeyValue("有效格点数", env.Volume)
        Call Report.KeyValue("表层格点数", env.GetAllSpots().Count(Function(s) s.IsSurface))
        Call Report.KeyValue("基础配方", String.Join(", ", CultureProtocol.Basal.Where(Function(m) m.Value > 0).Select(Function(m) $"{m.Key}={m.Value:F1}")))
        Call Report.KeyValue("阶段 I 因子", String.Join(", ", CultureProtocol.Phases(0).factors.Select(Function(f) $"{f.Key}={f.Value:F1}")))

        ' ==================== 4. 播种 NPC ====================
        Call Report.Section("阶段 4 / 6  在核心播种肾单位祖细胞")

        Dim npc As CellFateDefinition = catalog.ById("npc")
        Dim rand As New Random(2024)
        Dim coreSpots As Spot() = env.GetAllSpots() _
            .Where(Function(s) s.NormalizedRadius >= 0 AndAlso s.NormalizedRadius <= SEED_RADIUS) _
            .ToArray()

        If coreSpots.Length = 0 Then
            Throw New InvalidOperationException("球形空间里找不到核心格点，无法播种")
        End If

        Dim seeded As Integer = 0

        For Each spot As Spot In coreSpots
            If seeded >= SEED_CELLS Then
                Exit For
            End If

            Dim cella As VirtualCella = CellaFactory.BuildCell(npc.Blueprint, Nothing)

            Call CellaFactory.BindToSpot(cella, spot, env)
            seeded += 1
        Next

        Call Report.KeyValue("接种 NPC 数", seeded)
        Call Report.KeyValue("接种格点数", coreSpots.Length)
        Call Report.KeyValue("初始物种分布", Fmt.Population(env.PopulationBySpecies()))

        ' ==================== 5. 类器官发育 ====================
        Call Report.Section("阶段 5 / 6  类器官发育（生长因子节拍 + 自组织分化）")

        env.Differentiation = New DifferentiationSystem(catalog, rand)

        Dim crossFeeding As New CrossFeedingRecorder()
        Dim sw As Stopwatch = Stopwatch.StartNew()
        Dim times As New List(Of Double)()
        Dim compositionSeries As New List(Of Dictionary(Of String, Double))()
        Dim communitySeries As New List(Of Dictionary(Of String, Double))()
        Dim radialSeries As New List(Of RadialSample)()
        Dim spotSamples As New List(Of PopulationSample)()
        Dim metrics As New List(Of Dictionary(Of String, Double))()
        Dim currentPhase As String = Nothing
        Dim totalDeaths As Integer = 0

        For tick As Integer = 1 To N_TICKS
            ' 换液（阶段切换时才真正执行）
            Dim phase As CulturePhase = CultureProtocol.PhaseAt(env.CurrentTime)

            If Not String.Equals(phase.name, currentPhase, StringComparison.Ordinal) Then
                currentPhase = phase.name
                Call CultureProtocol.Apply(env, env.CurrentTime)

                Call Report.Line()
                Call Report.KeyValue($"换液 @t={env.CurrentTime:F0}", $"{phase.name}  " &
                    String.Join(", ", phase.factors.Select(Function(f) $"{f.Key}={f.Value:F1}")))
            End If

            Call env.Tick()

            totalDeaths += env.LastEvents.Deaths
            Call crossFeeding.Collect(env, env.CurrentTime)

            ' ---- 采样 ----
            times.Add(env.CurrentTime)
            compositionSeries.Add(SampleComposition(env, catalog))
            communitySeries.Add(SampleCommunity(env, npc.Blueprint))
            metrics.Add(SampleMetrics(env, catalog, npc.Blueprint))
            Call SampleRadial(env, radialSeries)
            Call SampleSpots(env, spotSamples)

            If tick Mod REPORT_EVERY = 0 Then
                Call PrintProgress(env, catalog, crossFeeding)
            End If
        Next

        sw.Stop()

        Call Report.Line()
        Call Report.KeyValue("发育耗时", $"{sw.Elapsed.TotalSeconds:F1} s（{sw.Elapsed.TotalMilliseconds / N_TICKS:F1} ms/step）")
        Call Report.KeyValue("分化事件总数", env.Differentiation.Switches.Count())
        Call Report.KeyValue("位置校正次数", env.Differentiation.Sortings.Count())
        Call Report.KeyValue("死亡细胞数", totalDeaths)
        Call Report.KeyValue("存活细胞数", env.GetAllCells().Count())
        Call Report.KeyValue("分化谱系登记", String.Join(", ", LineageStats(env.Differentiation, catalog)))
        Call Report.KeyValue("末端细胞组成", Fmt.Population(env.PopulationBySpecies()))

        ' ==================== 6. 导出 ====================
        Call Report.Section("阶段 6 / 6  结果导出")

        Dim dir As String = Report.ResultDirectory()

        Call System.IO.File.WriteAllText(System.IO.Path.Combine(dir, "organoid_composition.csv"),
            Report.SaveSeriesText("time", times, compositionSeries, CompositionColumns(catalog)))
        Call System.IO.File.WriteAllText(System.IO.Path.Combine(dir, "organoid_metrics.csv"),
            Report.SaveSeriesText("time", times, metrics, MetricsColumns()))
        Call System.IO.File.WriteAllText(System.IO.Path.Combine(dir, "organoid_community.csv"),
            Report.SaveSeriesText("time", times, communitySeries, CommunityColumns()))
        Call System.IO.File.WriteAllText(System.IO.Path.Combine(dir, "organoid_spot_composition.csv"),
            Report.SpotPopulationCsv(spotSamples))
        Call System.IO.File.WriteAllText(System.IO.Path.Combine(dir, "organoid_radial_bands.csv"),
            Report.RadialBandCsv(radialSeries))
        Call System.IO.File.WriteAllText(System.IO.Path.Combine(dir, "organoid_fate_switch.csv"),
            Report.FateSwitchCsv(env.Differentiation.Switches))
        Call System.IO.File.WriteAllText(System.IO.Path.Combine(dir, "organoid_sorting.csv"),
            Report.SortingCsv(env.Differentiation.Sortings))
        Call System.IO.File.WriteAllText(System.IO.Path.Combine(dir, "organoid_lineage.csv"),
            env.Lineage.ToCsv())
        Call System.IO.File.WriteAllText(System.IO.Path.Combine(dir, "organoid_lineage.nwk"),
            env.Lineage.ToNewick(env.CurrentTime))
        Call System.IO.File.WriteAllText(System.IO.Path.Combine(dir, "organoid_exchange_flux.csv"),
            crossFeeding.ToSpotCsv())
        Call System.IO.File.WriteAllText(System.IO.Path.Combine(dir, "organoid_exchange_summary.csv"),
            crossFeeding.ToSummaryCsv())
        Call System.IO.File.WriteAllText(System.IO.Path.Combine(dir, "organoid_slice.txt"),
            Fmt.OrganoidSlice(env, Report.CenterSlice(env)) & System.Environment.NewLine &
            RadialProfile(env, catalog))

        ' ---- 控制台汇总 ----
        Call Report.Line()
        Call Report.Line(Fmt.OrganoidSlice(env, Report.CenterSlice(env)))
        Call Report.Line(RadialProfile(env, catalog))

        Call Report.Line()
        Call Report.Line("  -- 细胞类型间代谢交换（按累计通量降序 Top 12）--")

        For Each edge In crossFeeding.SummaryEdges.Take(12)
            Call Report.KeyValue($"{edge.producer} → {edge.consumer}", $"{edge.metabolite}  flux={edge.flux:F4}")
        Next

        Call Report.Line()
        Call Report.KeyValue("代谢交换累计总通量", crossFeeding.TotalFlux.ToString("F4"))
        Call Report.KeyValue("交换边数（细胞类型级）", crossFeeding.SummaryEdges.Count())
        Call Report.KeyValue("交换边数（格点级）", crossFeeding.SpotEdges.Count())
        Call Report.KeyValue("结果输出目录", dir)
        Call Report.Line()
        Call Report.Line("人肾类器官演示完成。")

        Return 0
    End Function

    ' ==================== 采样 ====================

    Private Function SampleComposition(env As Environment, catalog As FateCatalog) As Dictionary(Of String, Double)
        Dim out As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)
        Dim population As Dictionary(Of String, Integer) = env.PopulationBySpecies()
        Dim total As Integer = 0

        For Each def As CellFateDefinition In catalog.All
            Dim n As Integer = 0

            Call population.TryGetValue(def.Id, n)

            out("cells_" & def.Id) = n
            out("genes_" & def.Id) = def.Blueprint.Genes.Length
            total += n
        Next

        out("cells_total") = total
        out("fates_present") = catalog.All.Count(Function(d) out("cells_" & d.Id) > 0)

        Return out
    End Function

    Private Function SampleCommunity(env As Environment, blueprint As CellaBlueprint) As Dictionary(Of String, Double)
        Dim out As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)
        Dim spots As Spot() = env.GetAllSpots().ToArray()

        If spots.Length = 0 Then
            Return out
        End If

        For Each id As String In HumanKidney.BasalMedium
            Dim sum As Double = 0.0

            For Each spot As Spot In spots
                Dim level As Double = 0.0

                Call spot.Medium.TryGetValue(id, level)
                sum += level
            Next

            out(id) = sum / spots.Length
        Next

        For Each gf As GrowthFactor In HumanKidney.GrowthFactors
            Dim sum As Double = 0.0

            For Each spot As Spot In spots
                Dim level As Double = 0.0

                Call spot.Medium.TryGetValue(gf.medium, level)
                sum += level
            Next

            out(gf.medium) = sum / spots.Length
        Next

        ' 胞内关键代谢物（全体细胞平均）
        Dim cells As VirtualCella() = env.GetAllCells().ToArray()

        For Each id As String In {"atp", "nadh", "pyr", "lac", "akg", "nh4", "hco3", "aa_pool"}
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

        out("nutrient_mean") = spots.Select(Function(s) s.NutrientLevel(blueprint)).Average()

        Return out
    End Function

    Private Function SampleMetrics(env As Environment, catalog As FateCatalog, blueprint As CellaBlueprint) As Dictionary(Of String, Double)
        Dim out As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)
        Dim spots As Spot() = env.GetAllSpots().ToArray()
        Dim occupied As Spot() = spots.Where(Function(s) s.cells.Count > 0).ToArray()
        Dim core As Spot() = spots.Where(Function(s) s.NormalizedRadius >= 0 AndAlso s.NormalizedRadius < 0.4).ToArray()
        Dim surface As Spot() = spots.Where(Function(s) s.IsSurface).ToArray()

        out("cells") = env.GetAllCells().Count()
        out("occupied_spots") = occupied.Length
        out("occupancy") = If(spots.Length = 0, 0.0, occupied.Length / CDbl(spots.Length))
        out("mean_radius") = If(occupied.Length = 0, 0.0, occupied.Select(Function(s) s.NormalizedRadius).Average())
        out("core_nutrient") = If(core.Length = 0, 0.0, core.Select(Function(s) s.NutrientLevel(blueprint)).Average())
        out("surface_nutrient") = If(surface.Length = 0, 0.0, surface.Select(Function(s) s.NutrientLevel(blueprint)).Average())
        out("differentiated") = env.GetAllCells().Count(Function(c) Not String.Equals(c.Species, "npc", StringComparison.OrdinalIgnoreCase))
        out("radial_spread") = RadialSpread(env)

        Return out
    End Function

    ''' <summary>细胞占据半径的标准差：越小说明空间聚集越紧致</summary>
    Private Function RadialSpread(env As Environment) As Double
        Dim radii As Double() = env.GetAllCells() _
            .Where(Function(c) c.Spot IsNot Nothing AndAlso c.Spot.NormalizedRadius >= 0) _
            .Select(Function(c) c.Spot.NormalizedRadius) _
            .ToArray()

        If radii.Length < 2 Then
            Return 0.0
        End If

        Dim mean As Double = radii.Average()

        Return System.Math.Sqrt(radii.Select(Function(r) (r - mean) * (r - mean)).Sum() / (radii.Length - 1))
    End Function

    ''' <summary>按径向分带统计每种细胞类型的分布</summary>
    Private Sub SampleRadial(env As Environment, samples As List(Of RadialSample))
        Dim time As Double = env.CurrentTime
        Dim bands As Dictionary(Of String, Integer()) = Nothing

        For Each spot As Spot In env.GetAllSpots()
            If spot.cells.Count = 0 OrElse spot.NormalizedRadius < 0 Then
                Continue For
            End If

            Dim band As Integer = System.Math.Min(RADIAL_BANDS - 1, CInt(System.Math.Floor(spot.NormalizedRadius * RADIAL_BANDS)))

            If bands Is Nothing Then
                bands = New Dictionary(Of String, Integer())(StringComparer.OrdinalIgnoreCase)
            End If

            For Each item In spot.PopulationBySpecies()
                Dim row As Integer() = Nothing

                If Not bands.TryGetValue(item.Key, row) Then
                    row = New Integer(RADIAL_BANDS - 1) {}
                    bands(item.Key) = row
                End If

                row(band) += item.Value
            Next
        Next

        If bands Is Nothing Then
            Return
        End If

        For Each item In bands
            For b As Integer = 0 To RADIAL_BANDS - 1
                samples.Add(New RadialSample With {
                    .time = time,
                    .band = b,
                    .band_label = BandLabels(b),
                    .fate = item.Key,
                    .count = item.Value(b)
                })
            Next
        Next
    End Sub

    Private Sub SampleSpots(env As Environment, samples As List(Of PopulationSample))
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

    ' ==================== 打印 ====================

    Private Sub PrintProgress(env As Environment, catalog As FateCatalog, crossFeeding As CrossFeedingRecorder)
        Dim population As Dictionary(Of String, Integer) = env.PopulationBySpecies()
        Dim spots As Spot() = env.GetAllSpots().ToArray()
        Dim npcBp As CellaBlueprint = catalog.ById("npc").Blueprint
        Dim core As Double = spots.Where(Function(s) s.NormalizedRadius < 0.4).Select(Function(s) s.NutrientLevel(npcBp)).DefaultIfEmpty(0.0).Average()
        Dim shell As Double = spots.Where(Function(s) s.IsSurface).Select(Function(s) s.NutrientLevel(npcBp)).DefaultIfEmpty(0.0).Average()

        Console.WriteLine(
            $"  [d={env.CurrentTime,5:F0}] cells={env.GetAllCells().Count(),4} " &
            $"pop={Fmt.BriefPopulation(population),-46} " &
            $"O2/glc 核心={core,6:F2} 表层={shell,6:F2} " &
            $"分化={env.Differentiation.Switches.Count(),4} 重排={env.Differentiation.Sortings.Count(),4}")
    End Sub

    Private Function LineageStats(system As DifferentiationSystem, catalog As FateCatalog) As String()
        Dim list As New List(Of String)()

        For Each def As CellFateDefinition In catalog.All
            If def.Precursors.IsNullOrEmpty Then
                Continue For
            End If

            For Each precursor As String In def.Precursors
                Dim n As Integer = system.SwitchCount(precursor, def.Id)

                list.Add($"{precursor}→{def.Id}:{n}")
            Next
        Next

        Return list.ToArray()
    End Function

    ''' <summary>径向分带的细胞类型组成（控制台表格）</summary>
    Private Function RadialProfile(env As Environment, catalog As FateCatalog) As String
        Dim sb As New System.Text.StringBuilder()

        sb.AppendLine("  -- 径向分带（自组织分区结果）--")
        sb.AppendLine("     径向带            细胞数   主导类型")

        Dim table As New Dictionary(Of Integer, Dictionary(Of String, Integer))()

        For Each spot As Spot In env.GetAllSpots()
            If spot.cells.Count = 0 OrElse spot.NormalizedRadius < 0 Then
                Continue For
            End If

            Dim band As Integer = System.Math.Min(RADIAL_BANDS - 1, CInt(System.Math.Floor(spot.NormalizedRadius * RADIAL_BANDS)))
            Dim row As Dictionary(Of String, Integer) = Nothing

            If Not table.TryGetValue(band, row) Then
                row = New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
                table(band) = row
            End If

            For Each item In spot.PopulationBySpecies()
                Dim n As Integer = 0

                Call row.TryGetValue(item.Key, n)
                row(item.Key) = n + item.Value
            Next
        Next

        For b As Integer = 0 To RADIAL_BANDS - 1
            Dim row As Dictionary(Of String, Integer) = Nothing

            If Not table.TryGetValue(b, row) Then
                sb.AppendLine($"     {BandLabels(b),-16} {0,8}")
                Continue For
            End If

            sb.AppendLine($"     {BandLabels(b),-16} {row.Values.Sum(),8}   {Fmt.Population(row)}")
        Next

        Return sb.ToString()
    End Function

    Private Function FormatSignals(table As Dictionary(Of String, Double)) As String
        If table.IsNullOrEmpty Then
            Return "-"
        End If

        Return String.Join("/", table.Select(Function(kv) $"{kv.Key}>{kv.Value:F2}"))
    End Function

    Private Function CompositionColumns(catalog As FateCatalog) As String()
        Return catalog.All.Select(Function(d) "cells_" & d.Id) _
            .Concat({"cells_total", "fates_present"}) _
            .ToArray()
    End Function

    Private Function MetricsColumns() As String()
        Return {"cells", "differentiated", "occupancy", "occupied_spots",
                "mean_radius", "radial_spread", "core_nutrient", "surface_nutrient"}
    End Function

    Private Function CommunityColumns() As String()
        Return HumanKidney.BasalMedium _
            .Concat(HumanKidney.GrowthFactors.Select(Function(g) g.medium)) _
            .Concat({"atp_int", "nadh_int", "pyr_int", "lac_int", "akg_int", "nh4_int", "hco3_int", "aa_pool_int", "nutrient_mean"}) _
            .ToArray()
    End Function

End Module

''' <summary>某时刻、某径向带上、某细胞类型的细胞数</summary>
Public Class RadialSample

    Public Property time As Double
    Public Property band As Integer
    Public Property band_label As String
    Public Property fate As String
    Public Property count As Integer

End Class
