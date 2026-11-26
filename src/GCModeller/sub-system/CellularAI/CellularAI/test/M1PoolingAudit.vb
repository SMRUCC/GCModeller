Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Math.Statistics.MetaAnalysis
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Evidence
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Model
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Summary
Imports std = System.Math

''' <summary>
''' M1 池化审计：因子表达式、手算小例、τ² 交叉验证、缺失语义、中心化、护栏、端到端装配。
''' </summary>
''' <remarks>
''' 断言一律使用 <see cref="DebugAudit"/> 中不带 <c>&lt;Conditional("DEBUG")&gt;</c> 的入口。
''' </remarks>
Public Module M1PoolingAudit

    Private Const ReportName As String = "cellularai-m1-report.json"

    ''' <summary>运行 M1 的全部审计用例。</summary>
    Public Function Run() As Integer
        Dim reportPath As String = Path.Combine(AppContext.BaseDirectory, ReportName)

        Call DebugAudit.Reset()
        DebugAudit.MaxEvents = 8192
        Call DebugAudit.Mark("cellularai-m1 audit started")

        Try
            Call CheckExpressionParsing()
            Call CheckIdentityFactors()
            Call CheckSingleRowOffset()
            Call CheckMissingAsZero()
            Call CheckTau2AgainstRandomEffects()
            Call CheckClipping()
            Call CheckUnmeasured()
            Call CheckBundlePooling()
        Catch ex As Exception
            Call DebugAudit.Mark("m1.audit.crash", ex.ToString())
            Call DebugAudit.Test(False, $"the M1 audit should not throw unhandled exceptions: {ex.Message}")
        End Try

        Call DebugAudit.SaveReport(reportPath)
        Call Console.WriteLine($"   [audit] report -> {reportPath}")
        Call Console.WriteLine($"   [audit] {DebugAudit.FailureCount} failure(s)")

        Return DebugAudit.FailureCount
    End Function

#Region "cases"

    ''' <summary>因子表达式：合法解析、未知因子与重复因子必须被拒绝。</summary>
    Private Sub CheckExpressionParsing()
        Dim parsed As String() = FactorExpression.Parse("sef*kd", FactorAxis.Col)

        Call DebugAudit.AreEqual(2, parsed.Length, "two factors are parsed")
        Call DebugAudit.AreEqual("sef", parsed(0), "the factor order is preserved")
        Call DebugAudit.AreEqual("kd", parsed(1), "the factor order is preserved")

        Call DebugAudit.AreEqual(0, FactorExpression.Parse("1", FactorAxis.Row).Length, "'1' means no factor")
        Call DebugAudit.AreEqual(0, FactorExpression.Parse("", FactorAxis.Row).Length, "an empty expression means no factor")

        Call DebugAudit.Test(FactorExpression.RequiresLocal(FactorExpression.Parse("inv:mad:dl", FactorAxis.Row)),
                             "inv:mad:dl is a local factor")
        Call DebugAudit.Test(Not FactorExpression.RequiresLocal(FactorExpression.Parse("nr", FactorAxis.Row)),
                             "nr is a cell line factor")

        Call DebugAudit.CheckThrows(Of ArgumentException)(
            Sub()
                Dim bad As String() = FactorExpression.Parse("sef*kd*zzz", FactorAxis.Col)
            End Sub, "an unknown factor is rejected")

        Call DebugAudit.CheckThrows(Of ArgumentException)(
            Sub()
                Dim bad As String() = FactorExpression.Parse("sef*sef", FactorAxis.Col)
            End Sub, "a duplicated factor is rejected")

        Call DebugAudit.CheckThrows(Of ArgumentException)(
            Sub()
                Dim bad As String() = FactorExpression.Parse("nr", FactorAxis.Col)
            End Sub, "a factor used on the wrong axis is rejected")
    End Sub

    ''' <summary>全 1 因子：所有方向的权重都是 1。</summary>
    Private Sub CheckIdentityFactors()
        Dim factors As PoolingFactors = PoolingFactors.Identity(
            New String() {"a", "b"}, New String() {"g1", "g2"},
            New String() {"g1"}, New String() {"g1", "g2"})

        Call DebugAudit.AreEqual(1.0, factors.RowWeight(0, New String() {"nr"}, 0), "row weight is 1")
        Call DebugAudit.AreEqual(1.0, factors.ColWeight(0, 0, New String() {"sef", "kd"}), "column weight is 1")
        Call DebugAudit.AreEqual(1.0, factors.MatWeight(0, New String() {"sim", "pair"}), "row multiplier is 1")
        Call DebugAudit.AreEqual(1, factors.Index(1), "identity maps row t to line t")

        Dim missing As PoolingFactors = PoolingFactors.Identity(
            New String() {"a"}, New String() {"g1"}, New String() {"g1"}, New String() {"g1", "zz"})

        Call DebugAudit.AreEqual(1.0, missing.RowWeight(0, New String() {"nr"}, 1), "a gene outside the union weighs 1")
    End Sub

    ''' <summary>单行单点：l2 等于减中心后的效应，z 等于该点的正态分数。</summary>
    Private Sub CheckSingleRowOffset()
        Dim table As New OffsetTable With {
            .CellLine = "a",
            .View = "view_0",
            .Genes = {"g1"},
            .EffectCenter = {0.5},
            .EffectCount = {1.0},
            .SignedScoreCenter = {0.0},
            .ResponseVariance = {0.25}
        }

        Dim evidence As ContextEvidence = Combine(
            stack:=Cube(Row({2.0})),
            probability:=Cube(Row({0.05})),
            tables:={table},
            genes:={"g1"}, perts:={"g1"})

        Call DebugAudit.Test(std.Abs(evidence.L2(0)(0) - 1.5) < 1e-12, $"l2 should be 2.0 - 0.5 = 1.5, got {evidence.L2(0)(0):F12}")
        Call DebugAudit.Test(std.Abs(evidence.Z(0)(0) - 1.959963984540054) < 1e-10, $"z should be 1.9599..., got {evidence.Z(0)(0):F12}")
        Call DebugAudit.AreEqual(0, evidence.Unmeasured.Length, "the single perturbation is measured")
    End Sub

    ''' <summary>log2 通道的"未测按 0 计入分母"，z 通道的"未测完全剔除"。</summary>
    Private Sub CheckMissingAsZero()
        Dim table As New OffsetTable With {
            .CellLine = "a",
            .View = "view_0",
            .Genes = {"g1"},
            .EffectCenter = {0.0},
            .EffectCount = {1.0},
            .SignedScoreCenter = {0.0},
            .ResponseVariance = {0.25}
        }

        Dim evidence As ContextEvidence = Combine(
            stack:=Cube(Row({2.0}), Row({Double.NaN})),
            probability:=Cube(Row({0.05}), Row({Double.NaN})),
            tables:={table, table},
            genes:={"g1"}, perts:={"g1"})

        Call DebugAudit.Test(std.Abs(evidence.L2(0)(0) - 1.0) < 1e-12,
                             $"an unmeasured row still counts in the denominator, so l2 = (2 + 0)/2 = 1, got {evidence.L2(0)(0):F12}")
        Call DebugAudit.Test(std.Abs(evidence.Z(0)(0) - 1.959963984540054) < 1e-10,
                             $"the z stream drops the unmeasured row, got {evidence.Z(0)(0):F12}")
    End Sub

    ''' <summary>τ² 必须与 <see cref="RandomEffects.DerSimonianLaird"/> 在单扰动上一致。</summary>
    Private Sub CheckTau2AgainstRandomEffects()
        Dim effects As Double() = {0.4, 1.6, 3.0}
        Dim variances As Double() = {0.09, 0.36, 0.25}

        Dim expected As Double = RandomEffects.DerSimonianLaird(effects, variances)

        Dim tables(2) As OffsetTable

        For t As Integer = 0 To 2
            tables(t) = New OffsetTable With {
                .CellLine = $"line{t}",
                .View = "view_0",
                .Genes = {"g1"},
                .EffectCenter = {0.0},
                .EffectCount = {1.0},
                .SignedScoreCenter = {0.0},
                .ResponseVariance = {variances(t)}
            }
        Next

        Dim evidence As ContextEvidence = Combine(
            stack:=Cube(Row({effects(0)}), Row({effects(1)}), Row({effects(2)})),
            probability:=Cube(Row({0.5}), Row({0.5}), Row({0.5})),
            tables:=tables, genes:={"g1"}, perts:={"g1"},
            vFloorRel:=0.0)

        ' 手算：τ² 已知后，权重 w_t = m/(V_t + τ²)，m = median(V) = 0.25
        Dim m As Double = 0.25
        Dim den As Double = 0.0
        Dim num As Double = 0.0

        For t As Integer = 0 To 2
            Dim w As Double = m / (variances(t) + expected)

            num += w * effects(t)
            den += w
        Next

        Dim expectedL2 As Double = num / den

        Call DebugAudit.Mark("m1.tau2_expected", expected)
        Call DebugAudit.Mark("m1.l2_hand_computed", expectedL2)
        DebugAudit.Test(expected > 0.0, $"the test data should produce a positive tau2, got {expected:G6}")
        DebugAudit.Test(std.Abs(evidence.L2(0)(0) - expectedL2) < 1e-12,
                        $"l2 should follow the random-effects weights: expected {expectedL2:F12}, got {evidence.L2(0)(0):F12}")
    End Sub

    ''' <summary>护栏：合并结果超过上限时被裁剪并记录。</summary>
    Private Sub CheckClipping()
        Dim table As New OffsetTable With {
            .CellLine = "a",
            .View = "view_0",
            .Genes = {"g1"},
            .EffectCenter = {0.0},
            .EffectCount = {1.0},
            .SignedScoreCenter = {0.0},
            .ResponseVariance = {0.25}
        }

        Dim evidence As ContextEvidence = Combine(
            stack:=Cube(Row({2.0})),
            probability:=Cube(Row({0.5})),
            tables:={table}, genes:={"g1"}, perts:={"g1"},
            l2AbsMax:=1.5)

        Call DebugAudit.AreEqual(1.5, evidence.L2(0)(0), "the pooled effect is clipped to the guard limit")
        DebugAudit.Test(evidence.Guards.ContainsKey("l2_abs_max"), "the clip is recorded in the guards")
        Call DebugAudit.AreEqual(1.0, evidence.Guards("l2_abs_max")(2), "every value was clipped")
    End Sub

    ''' <summary>完全没有证据的扰动必须被标出来。</summary>
    Private Sub CheckUnmeasured()
        Dim table As New OffsetTable With {
            .CellLine = "a",
            .View = "view_0",
            .Genes = {"g1"},
            .EffectCenter = {0.0},
            .EffectCount = {1.0},
            .SignedScoreCenter = {0.0},
            .ResponseVariance = {0.25}
        }

        Dim evidence As ContextEvidence = Combine(
            stack:=Cube(Row({1.0, Double.NaN})),
            probability:=Cube(Row({0.5, Double.NaN})),
            tables:={table}, genes:={"g1"}, perts:={"p1", "p2"})

        Call DebugAudit.AreEqual(1, evidence.Unmeasured.Length, "one perturbation has no evidence")
        Call DebugAudit.AreEqual("p2", evidence.Unmeasured(0), "the unmeasured perturbation is reported by name")

        ' 未测列的分母仍然由权重构成（missingAsZero），因此数值是 0 而不是 NaN；
        ' "没有证据"这件事由 Unmeasured 列表承载，由门控单独处理
        Call DebugAudit.AreEqual(0.0, evidence.L2(0)(1), "an unmeasured column pools to zero, not NaN")
        Call DebugAudit.AreEqual(0.0, evidence.Z(0)(1), "an unmeasured column has no significance")
    End Sub

    ''' <summary>端到端：在虚构任务上跑完整的池化。</summary>
    Private Sub CheckBundlePooling()
        Dim task As SyntheticTask = SyntheticFixture.Create()
        Dim model As CellularAiModel = CellularAiModel.Default()
        Dim targetCpm As Double() = ControlCpm(task)

        Call Guards.Reset()

        Dim evidence As ContextEvidence = MetaPooling.Pool(task.Bundle, task.Genes, task.Perturbations, targetCpm, model)

        Call DebugAudit.AreEqual(task.GeneCount, evidence.L2.Length, "l2 keeps the gene axis")
        Call DebugAudit.AreEqual(task.PerturbationCount, evidence.L2(0).Length, "l2 keeps the perturbation axis")
        Call DebugAudit.AreEqual(0, evidence.Unmeasured.Length, "the fixture measures every perturbation")

        Dim finite As Integer = 0

        For g As Integer = 0 To evidence.L2.Length - 1
            For p As Integer = 0 To evidence.L2(g).Length - 1
                If Not Double.IsNaN(evidence.L2(g)(p)) Then
                    finite += 1
                End If
            Next
        Next

        Call DebugAudit.Mark("m1.finite_effects", finite)
        DebugAudit.Test(finite > 0, "the pooled evidence should contain measured effects")

        ' 自靶基因的效应必须为负（虚构数据里靶点被敲低）
        For p As Integer = 0 To task.PerturbationCount - 1
            Dim at As Integer = Array.IndexOf(task.Genes, task.Perturbations(p))

            Call DebugAudit.Mark($"m1.self_effect_{p}", evidence.L2(at)(p))
            DebugAudit.Test(evidence.L2(at)(p) < 0.0, $"the target gene {task.Perturbations(p)} should be knocked down")
            DebugAudit.Test(evidence.Z(at)(p) > 3.0, $"the target gene {task.Perturbations(p)} should be highly significant")
        Next

        DebugAudit.Test(evidence.Guards.ContainsKey("pooling_share"), "the pooling similarity share is recorded")
        DebugAudit.Test(evidence.Guards.ContainsKey("l2_abs_max"), "the l2 clip guard is recorded")
        DebugAudit.Test(evidence.Guards.ContainsKey("v_floor_rel"), "the variance floor guard is recorded")
    End Sub

#End Region

#Region "helpers"

    ''' <summary>把一行数值包装成"单行面板"。</summary>
    Private Function Row(values As Double()) As Double()()
        Return New Double()() {values}
    End Function

    ''' <summary>把若干"行×扰动"面板堆成 行数×基因数×扰动数 的栈。</summary>
    Private Function Cube(ParamArray slices As Double()()()) As Double()()()
        Return slices
    End Function

    ''' <summary>构造一次池化请求（全 1 因子，便于手算）。</summary>
    Private Function Combine(stack As Double()()(),
                             probability As Double()()(),
                             tables As OffsetTable(),
                             genes As String(),
                             perts As String(),
                             Optional vFloorRel As Double = 0.0024,
                             Optional l2AbsMax As Double = 6.25,
                             Optional zAbsMax As Double = 187.5) As ContextEvidence

        Dim nRows As Integer = stack.Length
        Dim multipliers(nRows - 1) As Double

        ' 每个手算用例都从干净的护栏表开始，避免上一例的命中数污染比例断言
        Call Guards.Reset()

        For t As Integer = 0 To nRows - 1
            multipliers(t) = 1.0
        Next

        Dim factors As PoolingFactors = PoolingFactors.Identity(
            Enumerable.Range(0, nRows).Select(Function(i) $"line{i}").ToArray(),
            genes, perts, genes)

        Return MetaPooling.Combine(New PoolingRequest With {
            .Stack = stack,
            .Probability = probability,
            .Tables = tables,
            .Genes = genes,
            .Perts = perts,
            .Multipliers = multipliers,
            .Ops = PoolingOps.FromModel(CellularAiModel.Default()),
            .VFloorRel = vFloorRel,
            .Factors = factors,
            .L2AbsMax = l2AbsMax,
            .ZAbsMax = zAbsMax
        })
    End Function

    ''' <summary>控制细胞的平均 CPM（与上游 <c>control_cpm_mean</c> 同口径）。</summary>
    Private Function ControlCpm(task As SyntheticTask) As Double()
        Dim matrix As Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed.CsrMatrix = task.Controls(0)
        Dim counts As Double() = matrix.ColumnSums()
        Dim total As Double = 0.0

        For i As Integer = 0 To counts.Length - 1
            total += counts(i)
        Next

        If total <= 0.0 Then
            total = 1.0
        End If

        Dim mean As Double() = New Double(counts.Length - 1) {}

        For i As Integer = 0 To counts.Length - 1
            mean(i) = 1000000.0 * counts(i) / total
        Next

        Return mean
    End Function

#End Region

End Module
