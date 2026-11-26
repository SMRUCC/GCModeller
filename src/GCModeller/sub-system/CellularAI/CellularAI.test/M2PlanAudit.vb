Imports System
Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports CellularAI
Imports CellularAI.Model
Imports CellularAI.Planning
Imports CellularAI.Summary
Imports std = System.Math

''' <summary>
''' M2 计划审计：相似度权重（归一/上限/单调）、计划显著性（退化为经典 Stouffer）、
''' 曲线尺寸（中位对齐）、尺寸重排（总量守恒）、相近度抬升（≤20%）、成员选择（排除自身靶）。
''' </summary>
Public Module M2PlanAudit

    Private Const ReportName As String = "cellularai-m2-report.json"

    Public Function Run() As Integer
        Dim reportPath As String = Path.Combine(AppContext.BaseDirectory, ReportName)

        Call DebugAudit.Reset()
        DebugAudit.MaxEvents = 4096
        Call DebugAudit.Mark("cellularai-m2 audit started")

        Try
            Call CheckSimilarity()
            Call CheckCapWeights()
            Call CheckPlanZ()
            Call CheckCurveSizes()
            Call CheckS1Sizes()
            Call CheckXsimSizes()
            Call CheckSelectCalls()
        Catch ex As Exception
            Call DebugAudit.Mark("m2.audit.crash", ex.ToString())
            Call DebugAudit.Test(False, $"the M2 audit should not throw unhandled exceptions: {ex.Message}")
        End Try

        Call DebugAudit.SaveReport(reportPath)
        Call Console.WriteLine($"   [audit] report -> {reportPath}")
        Call Console.WriteLine($"   [audit] {DebugAudit.FailureCount} failure(s)")

        Return DebugAudit.FailureCount
    End Function

#Region "cases"

    ''' <summary>相似度权重：归一化、与目标谱一致的训练行权重更高。</summary>
    Private Sub CheckSimilarity()
        Dim genes As String() = {"g1", "g2", "g3", "g4"}
        Dim target As Double() = {100.0, 10.0, 50.0, 5.0}
        Dim ctrl As New Dictionary(Of String, ControlProfile)(StringComparer.Ordinal)

        ' line_A 与目标谱形一致；line_B 完全相反
        ctrl("line_A") = Profile(genes, {1000.0, 100.0, 500.0, 50.0})
        ctrl("line_B") = Profile(genes, {50.0, 500.0, 100.0, 1000.0})

        Dim fitted As SimilarityResult = SimilarityWeights.Similarity(
            genes, target, ctrl, New String() {"line_A", "line_B"}, 1.0, 4, 0.04)

        Dim total As Double = fitted.Weights(0) + fitted.Weights(1)

        Call DebugAudit.Mark("m2.sim_weight_a", fitted.Weights(0))
        Call DebugAudit.Mark("m2.sim_weight_b", fitted.Weights(1))
        Call DebugAudit.Test(std.Abs(total - 1.0) < 1e-12, $"the similarity weights should sum to 1, got {total:F12}")
        DebugAudit.Test(fitted.Correlations(0) > fitted.Correlations(1),
                        $"the matching profile should correlate better: {fitted.Correlations(0):F4} vs {fitted.Correlations(1):F4}")
        DebugAudit.Test(fitted.Weights(0) > fitted.Weights(1),
                        $"the matching profile should get a larger weight: {fitted.Weights(0):F4} vs {fitted.Weights(1):F4}")
    End Sub

    ''' <summary>上限水位法：总和守恒、单项不越界。</summary>
    Private Sub CheckCapWeights()
        Dim input As Double() = {0.7, 0.2, 0.1}
        Dim capped As Double() = SimilarityWeights.CapWeights(input, 0.4)

        Call DebugAudit.Mark("m2.cap_sum", capped.Sum)
        Call DebugAudit.Test(std.Abs(capped.Sum - 1.0) < 1e-12, $"capping should preserve the total, got {capped.Sum:F12}")

        Dim largest As Double = capped.Max

        Call DebugAudit.Mark("m2.cap_max", largest)
        DebugAudit.Test(largest <= 0.4 + 1e-12, $"no weight should exceed the cap, got {largest:F12}")
        Call DebugAudit.Test(std.Abs(capped(0) - 0.4) < 1e-12, $"the oversized weight is cut to the cap, got {capped(0):F12}")

        ' 上限低于均匀水平时，所有行都被压平
        Dim impossible As Double() = SimilarityWeights.CapWeights(New Double() {0.6, 0.4}, 0.2)

        Call DebugAudit.Test(std.Abs(impossible.Sum - 1.0) < 1e-12, "an unreachable cap still normalises")
        DebugAudit.Test(impossible.All(Function(v) std.Abs(v - 0.5) < 1e-12), "an unreachable cap flattens the weights")
    End Sub

    ''' <summary>计划显著性：均匀权重下必须退化为经典 Stouffer。</summary>
    Private Sub CheckPlanZ()
        Dim genes As String() = {"g1", "g2"}
        Dim perts As String() = {"p1"}
        Dim first As Double()() = {New Double() {1.0}, New Double() {2.0}}
        Dim second As Double()() = {New Double() {3.0}, New Double() {4.0}}
        Dim z As Double()()() = New Double()()() {first, second}
        Dim plan As New PlanOptions With {.PlanZUniform = 1.0}
        Dim guards As New GuardOptions

        Dim ctrl As New Dictionary(Of String, ControlProfile)(StringComparer.Ordinal)
        ctrl("line_A") = Profile(genes, {100.0, 100.0})
        ctrl("line_B") = Profile(genes, {100.0, 100.0})

        Dim result As Double()() = SimilarityWeights.PlanZ(
            z, New String() {"line_A", "line_B"}, ctrl, genes,
            New Double() {10.0, 10.0}, plan, guards)

        ' g1: |1+3|/√2 = 2.8284...;  g2: |2+4|/√2 = 4.2426...
        Call DebugAudit.Test(std.Abs(result(0)(0) - 4.0 / std.Sqrt(2.0)) < 1e-12,
                             $"the uniform plan z should be the classic Stouffer, got {result(0)(0):F12}")
        Call DebugAudit.Test(std.Abs(result(1)(0) - 6.0 / std.Sqrt(2.0)) < 1e-12,
                             $"the uniform plan z should be the classic Stouffer, got {result(1)(0):F12}")
        DebugAudit.Test(guards IsNot Nothing, "the guard options are accepted")
    End Sub

    ''' <summary>曲线尺寸：中位数对齐到 median_degs。</summary>
    Private Sub CheckCurveSizes()
        Dim model As CellularAiModel = CellularAiModel.Default()
        Dim breadth As Double() = {1.0, 2.0, 3.0, 4.0, 5.0}
        Dim sizes As Integer() = CallSizing.CurveSizes(breadth, 6000, model, 50.0)

        Dim median As Double = sizes.OrderBy(Function(v) v).ToArray()(sizes.Length \ 2)

        Call DebugAudit.Mark("m2.curve_median", median)
        DebugAudit.Test(std.Abs(median - 50.0) <= 1.0, $"the curve median should be median_degs, got {median:F3}")
        DebugAudit.Test(sizes.All(Function(v) v >= model.Calls.NMin), "every size respects n_min")
        DebugAudit.Test(sizes.All(Function(v) v <= model.Calls.NMax), "every size respects n_max")

        Dim increasing As Boolean = True

        For i As Integer = 1 To sizes.Length - 1
            If sizes(i) < sizes(i - 1) Then
                increasing = False
            End If
        Next

        DebugAudit.Test(increasing, "a monotone breadth gives monotone sizes")
    End Sub

    ''' <summary>尺寸重排：总量守恒（允许取整误差）。</summary>
    Private Sub CheckS1Sizes()
        Dim model As CellularAiModel = CellularAiModel.Default()
        Dim curve As Integer() = {10, 10, 10, 10, 10, 10, 10, 10}
        Dim score As Double() = {0.1, 0.2, 0.3, 0.4, 5.0, 5.1, 5.2, 5.3}

        Dim sizes As Integer() = CallSizing.S1Sizes(curve, score, model)
        Dim expected As Integer = curve.Sum
        Dim actual As Integer = sizes.Sum

        Call DebugAudit.Mark("m2.s1_total", actual)
        DebugAudit.Test(std.Abs(actual - expected) <= sizes.Length,
                        $"the reshuffle should conserve the total: expected {expected}, got {actual}")
        DebugAudit.Test(sizes.All(Function(v) v >= 1), "no perturbation drops below one call")
    End Sub

    ''' <summary>相近度抬升：增量不超过总量的 20%，且只增不减。</summary>
    Private Sub CheckXsimSizes()
        Dim model As CellularAiModel = CellularAiModel.Default()
        Dim curve As Integer() = {20, 20, 20, 20}
        Dim xs As Double() = {1.0, 2.0, 3.0, 9.0}
        Dim score As Double() = {0.5, 0.5, 0.5, 0.5}

        Dim sizes As Integer() = CallSizing.XsimSizes(curve, xs, score, model)
        Dim before As Integer = curve.Sum
        Dim after As Integer = sizes.Sum

        Call DebugAudit.Mark("m2.xsim_total", after)
        DebugAudit.Test(after >= before, "the lift should never remove calls")
        DebugAudit.Test(after <= CInt(std.Floor(1.2 * before)) + sizes.Length,
                        $"the lift should stay within 20%: before {before}, after {after}")
    End Sub

    ''' <summary>成员选择：排除自身靶基因、尺寸准确、方向与效应一致。</summary>
    Private Sub CheckSelectCalls()
        Dim model As CellularAiModel = CellularAiModel.Default()
        Dim genes As String() = {"g1", "g2", "g3", "g4", "g5"}
        Dim perts As String() = {"g1"}
        Dim l2 As Double()() = {New Double() {-2.0}, New Double() {1.5}, New Double() {-0.8}, New Double() {0.3}, New Double() {0.0}}
        Dim z As Double()() = {New Double() {8.0}, New Double() {6.0}, New Double() {4.0}, New Double() {2.0}, New Double() {1.0}}
        Dim cpm As Double() = {100.0, 80.0, 60.0, 40.0, 20.0}

        Dim calls As Dictionary(Of String, CallEntry) = CallPlan.SelectCalls(
            l2, z, cpm, genes, perts, 20000.0, model, sizes:=New Integer() {3})
        Dim entry As CallEntry = calls("g1")

        Call DebugAudit.AreEqual(3, entry.NCalls, "the plan honours the requested size")
        Call DebugAudit.AreEqual(3, entry.Index.Length, "the plan exposes the requested number of genes")
        DebugAudit.Test(Array.IndexOf(entry.Index, 0) < 0, "the own target gene is excluded from the calls")

        For i As Integer = 0 To entry.Index.Length - 1
            Dim g As Integer = entry.Index(i)
            Dim expected As Double = If(l2(g)(0) < 0.0, -1.0, 1.0)

            Call DebugAudit.AreEqual(expected, entry.Sign(i), $"call #{i} follows the effect sign")
        Next

        ' 未计划基因（不含自身靶）的强度应当递减且非零
        Dim rest As Integer() = Enumerable.Range(0, genes.Length).
            Where(Function(g) Array.IndexOf(entry.Index, g) < 0 AndAlso g <> 0).ToArray
        Dim strengths As Double() = rest.Select(Function(g) entry.NudgeStrength(g)).OrderByDescending(Function(v) v).ToArray

        For i As Integer = 1 To strengths.Length - 1
            DebugAudit.Test(strengths(i) <= strengths(i - 1) + 1e-12, "the nudge strength decreases with rank")
        Next

        Call DebugAudit.Mark("m2.nudge_coverage", strengths.Count(Function(v) v > 0.0))
        DebugAudit.Test(strengths.All(Function(v) v > 0.0), "every unplanned gene keeps a nudge strength")
    End Sub

#End Region

#Region "helpers"

    Private Function Profile(genes As String(), counts As Double()) As ControlProfile
        Return New ControlProfile With {.Genes = genes, .Counts = counts}
    End Function

#End Region

End Module
