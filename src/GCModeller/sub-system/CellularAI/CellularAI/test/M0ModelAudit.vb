Imports System
Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports CellularAI
Imports CellularAI.Model
Imports CellularAI.Summary
Imports std = System.Math

''' <summary>
''' M0 骨架审计：模型默认值、深拷贝独立性、护栏计数语义、虚构任务装配、
''' 面板轴对齐、带符号 z、逐基因中心化、并集堆叠。
''' </summary>
''' <remarks>
''' 断言一律使用 <see cref="DebugAudit"/> 中不带 <c>&lt;Conditional("DEBUG")&gt;</c> 的入口，
''' 结束时用 <see cref="DebugAudit.SaveReport"/> 导出 JSON 报告。
''' </remarks>
Public Module M0ModelAudit

    Private Const ReportName As String = "cellularai-m0-report.json"

    ''' <summary>运行 M0 的全部审计用例。</summary>
    ''' <returns>失败断言的数量（0 表示全部通过）。</returns>
    Public Function Run() As Integer
        Dim reportPath As String = Path.Combine(AppContext.BaseDirectory, ReportName)

        Call DebugAudit.Reset()
        DebugAudit.MaxEvents = 4096
        Call DebugAudit.Mark("cellularai-m0 audit started")

        Try
            Call CheckModelDefaults()
            Call CheckCloneIndependence()
            Call CheckGuards()
            Call CheckFixture()
            Call CheckAlignment()
            Call CheckSignedZ()
            Call CheckOffsets()
            Call CheckStacking()
        Catch ex As Exception
            Call DebugAudit.Mark("m0.audit.crash", ex.ToString())
            Call DebugAudit.Test(False, $"the M0 audit should not throw unhandled exceptions: {ex.Message}")
        End Try

        Call DebugAudit.SaveReport(reportPath)
        Call Console.WriteLine($"   [audit] report -> {reportPath}")
        Call Console.WriteLine($"   [audit] {DebugAudit.FailureCount} failure(s)")

        Return DebugAudit.FailureCount
    End Function

#Region "cases"

    ''' <summary>参数默认值必须与参考配置逐项一致。</summary>
    Private Sub CheckModelDefaults()
        Dim model As CellularAiModel = CellularAiModel.Default()

        Call DebugAudit.AreEqual("inv:mad:dl", model.Pooling.L2Row, "pooling.l2_row")
        Call DebugAudit.AreEqual("sef*kd", model.Pooling.L2Col, "pooling.l2_col")
        Call DebugAudit.AreEqual("auxiliary*sim*pair", model.Pooling.L2Mat, "pooling.l2_mat")
        Call DebugAudit.AreEqual("nr", model.Pooling.ZRow, "pooling.z_row")
        Call DebugAudit.AreEqual(0.25, model.Pooling.Sim, "pooling.sim")
        Call DebugAudit.AreEqual(2000, model.Pooling.SimHvg, "pooling.sim_hvg")
        Call DebugAudit.AreEqual(0.6, model.Pooling.KdRef, "pooling.kd_ref")
        Call DebugAudit.AreEqual(4.0, model.Pooling.SeClip, "pooling.se_clip")
        Call DebugAudit.AreEqual(2.0, model.Pooling.AuxiliaryWeight, "pooling.auxiliary_weight")
        Call DebugAudit.AreEqual(0.5, model.Pooling.PairWeight, "pooling.pair_weight")
        Call DebugAudit.AreEqual(2, model.Pooling.Views.Length, "pooling.views length")
        Call DebugAudit.AreEqual("view_1", model.Pooling.Views(1), "pooling.views[1]")

        Call DebugAudit.AreEqual(5.0, model.Calls.EligibleCpm, "calls.eligible_cpm")
        Call DebugAudit.AreEqual(0.02, model.Calls.MinLfc, "calls.min_lfc")
        Call DebugAudit.AreEqual(2.5, model.Calls.Gamma, "calls.gamma")
        Call DebugAudit.AreEqual(5, model.Calls.NMin, "calls.n_min")
        Call DebugAudit.AreEqual(6000, model.Calls.NMax, "calls.n_max")
        Call DebugAudit.AreEqual(0.1, model.Calls.Phi, "calls.phi")
        Call DebugAudit.AreEqual(111.2, model.Calls.StrengthK, "calls.strength_k")

        Call DebugAudit.AreEqual(3.0, model.Plan.CurveSlope, "plan.curve_slope")
        Call DebugAudit.AreEqual(3000, model.Plan.SimNtop, "plan.sim_ntop")
        Call DebugAudit.AreEqual(0.3, model.Plan.SimCap, "plan.sim_cap")
        Call DebugAudit.AreEqual(20000.0, model.Plan.PowerDepth, "plan.power_depth")
        Call DebugAudit.AreEqual(0.1, model.Plan.ShrinkFactor, "plan.shrink_factor")

        Call DebugAudit.AreEqual(64, model.Totals.CreditDraws, "totals.credit_draws")
        Call DebugAudit.AreEqual(160, model.Totals.ResidualRank, "totals.residual_rank")
        Call DebugAudit.AreEqual(10.0, model.Totals.ResidualRidge, "totals.residual_ridge")
        Call DebugAudit.AreEqual(32, model.Totals.ResidualMinMeasured, "totals.residual_min_measured")
        Call DebugAudit.AreEqual(5, model.Totals.ResidualGlobal, "totals.residual_global")
        Call DebugAudit.AreEqual(100, model.Totals.PwK, "totals.pw_k")
        Call DebugAudit.AreEqual(10, model.Totals.CpJ0, "totals.cp_j0")
        Call DebugAudit.AreEqual(50, model.Totals.CpK, "totals.cp_k")
        Call DebugAudit.AreEqual(3, model.Totals.FixedSteps, "totals.fixed_steps")
        Call DebugAudit.AreEqual(0.001, model.Totals.FixedRtol, "totals.fixed_rtol")

        Call DebugAudit.AreEqual(0.6, model.Placement.Rule.Amp, "placement.rule.amp")
        Call DebugAudit.AreEqual(0.8, model.Placement.Frozen.Cap, "placement.frozen.cap")
        Call DebugAudit.AreEqual(1.88, model.Placement.Boost, "placement.boost")
        Call DebugAudit.AreEqual(0.9, model.Placement.Damp, "placement.damp")
        Call DebugAudit.AreEqual(0.85, model.Placement.LibSpread, "placement.lib_spread")

        Call DebugAudit.AreEqual(5.5, model.Synthesis.ZPlan(0), "synthesis.z_plan[0]")
        Call DebugAudit.AreEqual(9.0, model.Synthesis.ZPlan(1), "synthesis.z_plan[1]")
        Call DebugAudit.AreEqual(1.0, model.Synthesis.Ztol, "synthesis.ztol")
        Call DebugAudit.AreEqual(0.2, model.Synthesis.ZtolPlan, "synthesis.ztol_plan")
        Call DebugAudit.AreEqual(0.01, model.Synthesis.Ltol, "synthesis.ltol")
        Call DebugAudit.AreEqual(4, model.Synthesis.NeBurst, "synthesis.ne_burst")
        Call DebugAudit.AreEqual(5.0, model.Synthesis.PlannedZ, "synthesis.planned_z")
        Call DebugAudit.AreEqual(40, model.Synthesis.SolveNtop, "synthesis.solve_ntop")
        Call DebugAudit.AreEqual(0.025, model.Synthesis.SparsifyStep, "synthesis.sparsify_step")

        Call DebugAudit.AreEqual(6.25, model.Guards.L2AbsMax, "guards.l2_abs_max")
        Call DebugAudit.AreEqual(187.5, model.Guards.ZAbsMax, "guards.z_abs_max")
        Call DebugAudit.AreEqual(0.0024, model.Guards.VFloorRel, "guards.v_floor_rel")
        Call DebugAudit.AreEqual(0.04, model.Guards.SimSdMin, "guards.sim_sd_min")
        Call DebugAudit.AreEqual(8.125, model.Guards.SimZClip, "guards.sim_z_clip")
        Call DebugAudit.AreEqual(320.0, model.Guards.NrClip, "guards.nr_clip")
        Call DebugAudit.AreEqual(12.5, model.Guards.Log2Max, "guards.log2_max")
        Call DebugAudit.AreEqual(7.5, model.Guards.MtLsClip, "guards.mt_ls_clip")
        Call DebugAudit.AreEqual(0.028, model.Guards.OrderSdMin, "guards.order_sd_min")
        Call DebugAudit.AreEqual(0.24, model.Guards.ShareMaxFor("plan"), "guards.sim_share_max.plan")
        Call DebugAudit.AreEqual(5.9, model.Guards.NeffMinFor("sim"), "guards.sim_neff_min.sim")
        Call DebugAudit.AreEqual(0.0075, model.Guards.MaxHitRateFor("log2_max"), "guards.max_hit_rate.log2_max")
        Call DebugAudit.AreEqual(0.975, model.Guards.CreditBandLo, "guards.credit_band_lo")
    End Sub

    ''' <summary>深拷贝必须与源对象完全解耦。</summary>
    Private Sub CheckCloneIndependence()
        Dim model As CellularAiModel = CellularAiModel.Default()
        Dim copy As CellularAiModel = model.Clone()

        Call DebugAudit.AreEqual(model.Calls.NMax, copy.Calls.NMax, "clone keeps the call n_max")
        Call DebugAudit.AreEqual(model.Plan.SimCap, copy.Plan.SimCap, "clone keeps the plan sim_cap")

        copy.Calls.NMax = 123
        copy.Pooling.Views(0) = "changed"
        copy.Guards.ShareMax("plan") = 0.99
        copy.Placement.Rule.Amp = 9.9
        copy.Totals.ResidualRank = 7

        Call DebugAudit.AreEqual(6000, model.Calls.NMax, "mutating the clone must not touch the source n_max")
        Call DebugAudit.AreEqual("view_0", model.Pooling.Views(0), "mutating the clone must not touch the source views")
        Call DebugAudit.AreEqual(0.24, model.Guards.ShareMax("plan"), "mutating the clone must not touch the source guard table")
        Call DebugAudit.AreEqual(0.6, model.Placement.Rule.Amp, "mutating the clone must not touch the source placement rule")
        Call DebugAudit.AreEqual(160, model.Totals.ResidualRank, "mutating the clone must not touch the source residual rank")
    End Sub

    ''' <summary>护栏计数：裁剪、事件比例、权重集中度。</summary>
    Private Sub CheckGuards()
        Call Guards.Reset()
        Call DebugAudit.AreEqual(0, Guards.Snapshot().Count, "a reset guard table should be empty")

        Dim clipped As Double() = Guards.ClipAbs(New Double() {1.0, -2.0, 0.5}, 1.5, "clip")
        Call DebugAudit.AreEqual(1.0, clipped(0), "values inside the limit are kept")
        Call DebugAudit.AreEqual(-1.5, clipped(1), "negative values are clipped to -limit")
        Call DebugAudit.AreEqual(0.5, clipped(2), "positive values inside the limit are kept")

        Dim snapshot As Dictionary(Of String, Double()) = Guards.Snapshot()
        Call DebugAudit.Test(snapshot.ContainsKey("clip"), "the clip guard should be recorded")
        Call DebugAudit.AreEqual(1.0, snapshot("clip")(0), "clip hits")
        Call DebugAudit.AreEqual(3.0, snapshot("clip")(1), "clip opportunities")
        Call DebugAudit.Test(std.Abs(snapshot("clip")(2) - 1.0 / 3.0) < 1e-12, "clip hit rate")

        Call Guards.Fired("event", True)
        Call Guards.Fired("event4", 2, 4)
        Call DebugAudit.AreEqual(1.0, Guards.HitRate("event"), "a fired flag has hit rate 1")
        Call DebugAudit.AreEqual(0.5, Guards.HitRate("event4"), "2 of 4 gives hit rate 0.5")
        Call DebugAudit.AreEqual(0.0, Guards.HitRate("absent"), "an unknown guard has hit rate 0")

        Dim options As New GuardOptions
        Call Guards.Share("plan", New Double() {0.5, 0.3, 0.2}, options)

        snapshot = Guards.Snapshot()
        Call DebugAudit.AreEqual(0.5, snapshot("plan_share")(0), "max weight share")
        Call DebugAudit.Test(std.Abs(snapshot("plan_neff")(0) - 1.0 / 0.38) < 1e-9, "effective weight count")
        Call DebugAudit.AreEqual(1.0, snapshot("plan_share_exceeded")(0), "0.5 exceeds the 0.24 cap")
        Call DebugAudit.AreEqual(1.0, snapshot("plan_neff_below")(0), "2.63 is below the 16 minimum")

        Call Guards.Reset()
        Call DebugAudit.AreEqual(0, Guards.Snapshot().Count, "reset clears every counter")
    End Sub

    ''' <summary>虚构任务必须能装配并通过自校验。</summary>
    Private Sub CheckFixture()
        Dim task As SyntheticTask = SyntheticFixture.Create()

        Call DebugAudit.AreEqual(30, task.GeneCount, "the fixture exposes 30 genes")
        Call DebugAudit.AreEqual(3, task.PerturbationCount, "the fixture exposes 3 perturbations")
        Call DebugAudit.AreEqual(2, task.Contexts.Length, "the fixture exposes 2 contexts")
        Call DebugAudit.AreEqual(2, task.Bundle.Catalog.Count, "the catalog holds 2 summary rows")
        Call DebugAudit.AreEqual(2, task.Controls.Length, "two control matrices")

        Try
            Call task.Bundle.Validate()
            Call DebugAudit.Test(True, "the bundle passes validation")
        Catch ex As Exception
            Call DebugAudit.Test(False, $"the bundle should validate: {ex.Message}")
        End Try

        Dim profiles As Dictionary(Of String, ControlProfile) = task.Bundle.ControlProfiles()

        Call DebugAudit.AreEqual(2, profiles.Count, "two control profiles are exposed")

        For Each line As String In profiles.Keys
            Dim cpm As Double() = profiles(line).Cpm()
            Dim total As Double = 0.0

            For i As Integer = 0 To cpm.Length - 1
                total += cpm(i)
            Next

            Call DebugAudit.Test(std.Abs(total - 1000000.0) < 1e-6, $"CPM of '{line}' should sum to 1e6, got {total:F3}")
        Next
    End Sub

    ''' <summary>面板重索引：缺失填 NaN、重复基因取最大值、可按扰动排除。</summary>
    Private Sub CheckAlignment()
        Dim panelGenes As String() = {"a", "a", "b"}
        Dim panelPerts As String() = {"p1", "p2"}
        Dim panel As Double()() = {New Double() {1.0, 10.0}, New Double() {2.0, 20.0}, New Double() {3.0, 30.0}}

        Dim aligned As Double()() = SummaryBundle.AlignPanel(
            panelGenes, panelPerts, panel,
            New String() {"a", "b", "zz"}, New String() {"p1", "p2"})

        Call DebugAudit.AreEqual(3, aligned.Length, "aligned rows follow the target gene axis")
        Call DebugAudit.AreEqual(2, aligned(0).Length, "aligned columns follow the target perturbation axis")
        Call DebugAudit.AreEqual(2.0, aligned(0)(0), "duplicated genes collapse to the maximum")
        Call DebugAudit.AreEqual(20.0, aligned(0)(1), "duplicated genes collapse on every column")
        Call DebugAudit.AreEqual(3.0, aligned(1)(0), "a present gene keeps its value")
        Call DebugAudit.Test(Double.IsNaN(aligned(2)(0)), "a gene missing from the panel becomes NaN")
        Call DebugAudit.Test(Double.IsNaN(aligned(2)(1)), "a gene missing from the panel becomes NaN on every column")

        Dim excluded As Double()() = SummaryBundle.AlignPanel(
            panelGenes, panelPerts, panel,
            New String() {"a"}, New String() {"p1", "p2"}, New String() {"p2"})

        Call DebugAudit.AreEqual(2.0, excluded(0)(0), "the non-excluded perturbation is aligned")
        Call DebugAudit.Test(Double.IsNaN(excluded(0)(1)), "the excluded perturbation is left empty")
    End Sub

    ''' <summary>带符号正态分数与上游 signed_z 同式。</summary>
    Private Sub CheckSignedZ()
        Dim effect As Double()() = {New Double() {2.0, -2.0, Double.NaN}}
        Dim probability As Double()() = {New Double() {0.05, 0.05, 0.1}}
        Dim z As Double()() = SummaryBundle.SignedZ(effect, probability)

        Call DebugAudit.Test(std.Abs(z(0)(0) - 1.959963984540054) < 1e-12, $"the positive score should be +1.9599..., got {z(0)(0):F12}")
        Call DebugAudit.Test(std.Abs(z(0)(1) + 1.959963984540054) < 1e-12, $"the negative score should be -1.9599..., got {z(0)(1):F12}")
        Call DebugAudit.Test(Double.IsNaN(z(0)(2)), "an unmeasured effect stays NaN")
    End Sub

    ''' <summary>逐基因中心化：中心缺失时透传，面板缺失保持 NaN。</summary>
    Private Sub CheckOffsets()
        Dim offset As New OffsetTable With {
            .CellLine = "line",
            .View = "view_0",
            .Genes = {"a", "b"},
            .EffectCenter = {0.5, Double.NaN},
            .EffectCount = {1.0, 1.0},
            .SignedScoreCenter = {0.0, 0.0},
            .ResponseVariance = {0.1, 0.1}
        }

        Dim panel As Double()() = {New Double() {1.0, 2.0}, New Double() {3.0, Double.NaN}}
        Dim shifted As Double()() = SummaryBundle.ApplyOffsets(panel, offset, New String() {"a", "b"}, "l2")

        Call DebugAudit.AreEqual(0.5, shifted(0)(0), "the effect centre is subtracted")
        Call DebugAudit.AreEqual(1.5, shifted(0)(1), "the effect centre is subtracted on every column")
        Call DebugAudit.AreEqual(3.0, shifted(1)(0), "a NaN centre passes the value through")
        Call DebugAudit.Test(Double.IsNaN(shifted(1)(1)), "an unmeasured entry stays NaN")
    End Sub

    ''' <summary>并集堆叠：行顺序、基因并集、缺失填 NaN。</summary>
    Private Sub CheckStacking()
        Dim task As SyntheticTask = SyntheticFixture.Create()
        Dim stacked = task.Bundle.StackEffects(task.Perturbations)

        Call DebugAudit.AreEqual(2, stacked.Lines.Length, "two primary rows are stacked")
        Call DebugAudit.AreEqual("line_A", stacked.Lines(0), "the row order follows the catalog")
        Call DebugAudit.AreEqual(30, stacked.Union.Length, "the union covers the 30 task genes")
        Call DebugAudit.AreEqual(2, stacked.Stack.Length, "the stack has one slice per row")
        Call DebugAudit.AreEqual(30, stacked.Stack(0).Length, "each slice follows the union axis")
        Call DebugAudit.AreEqual(3, stacked.Stack(0)(0).Length, "each slice follows the perturbation axis")

        Dim sorted As Boolean = True

        For i As Integer = 1 To stacked.Union.Length - 1
            If String.CompareOrdinal(stacked.Union(i - 1), stacked.Union(i)) > 0 Then
                sorted = False
            End If
        Next

        DebugAudit.Test(sorted, "the union axis is sorted ordinally")

        ' line_B 缺少 g004：该切片上必须是 NaN，而 line_A 上有值
        Dim at As Integer = Array.IndexOf(stacked.Union, "g004")

        Call DebugAudit.Test(at >= 0, "g004 is part of the union")
        Call DebugAudit.Test(Not Double.IsNaN(stacked.Stack(0)(at)(0)), "line_A measures g004")
        Call DebugAudit.Test(Double.IsNaN(stacked.Stack(1)(at)(0)), "line_B leaves g004 unmeasured")
    End Sub

#End Region

End Module
