Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Model
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Residual
Imports std = System.Math

''' <summary>
''' M3 残差审计：响应基（秩/正交/重构/自身靶替换）、比值限幅、符号保持、
''' 一致性融合、以及控制条件残差模型的预测边界。
''' </summary>
Public Module M3ResidualAudit

    Private Const ReportName As String = "cellularai-m3-report.json"

    Public Function Run() As Integer
        Dim reportPath As String = Path.Combine(AppContext.BaseDirectory, ReportName)

        Call DebugAudit.Reset()
        DebugAudit.MaxEvents = 4096
        Call DebugAudit.Mark("cellularai-m3 audit started")

        Try
            Call CheckResponseBasis()
            Call CheckBoundedRatio()
            Call CheckPreserveSigns()
            Call CheckAgreement()
            Call CheckControlResidual()
        Catch ex As Exception
            Call DebugAudit.Mark("m3.audit.crash", ex.ToString())
            Call DebugAudit.Test(False, $"the M3 audit should not throw unhandled exceptions: {ex.Message}")
        End Try

        Call DebugAudit.SaveReport(reportPath)
        Call Console.WriteLine($"   [audit] report -> {reportPath}")
        Call Console.WriteLine($"   [audit] {DebugAudit.FailureCount} failure(s)")

        Return DebugAudit.FailureCount
    End Function

#Region "cases"

    ''' <summary>响应基：秩恢复、载荷正交、中心化重构、自身靶被替换。</summary>
    Private Sub CheckResponseBasis()
        Dim nGenes As Integer = 12
        Dim nPerts As Integer = 6
        Dim genes(nGenes - 1) As String
        Dim perts(nPerts - 1) As String

        For i As Integer = 0 To nGenes - 1
            genes(i) = $"g{i + 1}"
        Next
        For p As Integer = 0 To nPerts - 1
            perts(p) = $"g{p + 1}"
        Next

        ' 秩 3 的合成矩阵：basis(12×3) × coordinates(3×6)
        Dim rnd As New Random(7)
        Dim basis(nGenes - 1)() As Double
        Dim coords(2)() As Double

        For i As Integer = 0 To nGenes - 1
            basis(i) = New Double() {rnd.NextDouble() - 0.5, rnd.NextDouble() - 0.5, rnd.NextDouble() - 0.5}
        Next
        For c As Integer = 0 To 2
            coords(c) = New Double(nPerts - 1) {}

            For p As Integer = 0 To nPerts - 1
                coords(c)(p) = rnd.NextDouble() * 2.0 - 1.0
            Next
        Next

        Dim matrix(nGenes - 1)() As Double

        For i As Integer = 0 To nGenes - 1
            matrix(i) = New Double(nPerts - 1) {}

            For p As Integer = 0 To nPerts - 1
                Dim acc As Double = 0.0

                For c As Integer = 0 To 2
                    acc += basis(i)(c) * coords(c)(p)
                Next

                matrix(i)(p) = acc
            Next
        Next

        ' 把 (g1,p1)、(g2,p2) 设成极端的自身靶效应
        matrix(0)(0) = -8.0
        matrix(1)(1) = -7.0

        Dim model As ResponseBasis = ResponseBasis.Fit(matrix, New Integer() {0, 1}, New Integer() {0, 1}, 3, 0)

        Call DebugAudit.Mark("m3.basis_rank", model.Rank)
        DebugAudit.Test(model.Rank >= 3, $"a rank-3 matrix should recover at least 3 components, got {model.Rank}")

        ' 载荷列正交
        Dim worst As Double = 0.0

        For a As Integer = 0 To 2
            For b As Integer = 0 To 2
                Dim acc As Double = 0.0

                For i As Integer = 0 To nGenes - 1
                    acc += model.Basis(i)(a) * model.Basis(i)(b)
                Next

                If a = b Then
                    acc -= 1.0
                End If

                worst = std.Max(worst, std.Abs(acc))
            Next
        Next

        Call DebugAudit.Mark("m3.basis_orthogonality", worst)
        DebugAudit.Test(worst < 1e-10, $"the basis columns should be orthonormal, worst = {worst:E3}")

        ' 自身靶位置被替换为行均值：该位置不再等于 -8
        Call DebugAudit.Mark("m3.basis_own_target", model.Centered(0)(0))
        DebugAudit.Test(std.Abs(model.Centered(0)(0)) < 8.0, "the own-target effect is replaced by the row mean")
        DebugAudit.Test(std.Abs(model.Intercept(0)) < 8.0, "the intercept comes from the off-diagonal mean")
    End Sub

    ''' <summary>比值限幅：因子恒在 [0.5, 2]，方向不变，零输入保持零。</summary>
    Private Sub CheckBoundedRatio()
        Dim query As Double()() = {New Double() {1.0, -1.0, 0.0, 0.0001}}
        Dim correction As Double()() = {New Double() {5.0, -5.0, 3.0, 1.0}}
        Dim result As Double()() = EffectShaping.BoundedRatio(query, correction)

        For j As Integer = 0 To 3
            Dim ratio As Double

            If query(0)(j) = 0.0 Then
                Call DebugAudit.AreEqual(0.0, result(0)(j), "a zero effect stays zero")
                Continue For
            End If

            ratio = result(0)(j) / query(0)(j)

            Call DebugAudit.Mark($"m3.bounded_ratio_{j}", ratio)
            DebugAudit.Test(ratio >= 0.5 - 1e-12 AndAlso ratio <= 2.0 + 1e-12,
                            $"the ratio should stay in [0.5, 2], got {ratio:F12}")
            DebugAudit.Test(std.Sign(result(0)(j)) = std.Sign(query(0)(j)), "the sign must not flip")
        Next
    End Sub

    ''' <summary>符号保持：翻转与归零被回滚，自身靶始终保留原值。</summary>
    Private Sub CheckPreserveSigns()
        Dim query As Double()() = {New Double() {2.0, -3.0}, New Double() {0.0, 0.0}}
        Dim adjusted As Double()() = {New Double() {-1.0, 0.0}, New Double() {0.0, 0.0}}
        Dim result As Double()() = EffectShaping.PreserveSigns(query, adjusted,
            New String() {"a", "b"}, New String() {"a", "p2"})

        Call DebugAudit.AreEqual(2.0, result(0)(0), "a sign flip is rolled back to the query value")
        Call DebugAudit.AreEqual(-3.0, result(0)(1), "a zeroed effect is rolled back to the query value")

        ' 自身靶（基因 a 对应扰动 a）必须保留原值
        Call DebugAudit.AreEqual(2.0, result(0)(0), "the own target keeps the query value")
    End Sub

    ''' <summary>一致性融合：同量级同号时取修正值，反向时保持原值。</summary>
    Private Sub CheckAgreement()
        Dim x As Double()() = {New Double() {2.0, 2.0, 2.0}}
        Dim t As Double()() = {New Double() {2.0, -2.0, 2.0}}
        Dim p As Double()() = {New Double() {3.0, 3.0, 2.0}}
        Dim result As Double()() = EffectShaping.EvidenceAgreement(x, t, p)

        Call DebugAudit.AreEqual(3.0, result(0)(0), "equal magnitudes and the same sign give w = 1")
        Call DebugAudit.AreEqual(2.0, result(0)(1), "opposite signs keep the original evidence")
        Call DebugAudit.AreEqual(2.0, result(0)(2), "an unchanged correction keeps the original value")
        DebugAudit.Test(result(0)(0) <= std.Max(x(0)(0), p(0)(0)) + 1e-12, "the blend stays between the endpoints")
    End Sub

    ''' <summary>控制条件残差：可拟合、支持集非空、预测有界且保持符号与自身靶。</summary>
    Private Sub CheckControlResidual()
        Dim options As New TotalsOptions With {
            .ResidualRank = 3,
            .ResidualRidge = 1.0,
            .ResidualMinMeasured = 2,
            .ResidualGlobal = 2,
            .ResidualAlpha = 0.25,
            .ResidualClip = 3.0
        }

        Dim genes As String() = {"g1", "g2", "g3", "g4", "g5"}
        Dim perts As String() = {"g1", "g2"}
        Dim nRecords As Integer = 6
        Dim nGenes As Integer = genes.Length
        Dim nPerts As Integer = perts.Length
        Dim rnd As New Random(11)
        Dim stack(nRecords - 1)()() As Double
        Dim counts(nRecords - 1)() As Double

        For t As Integer = 0 To nRecords - 1
            stack(t) = RectangularArray.Matrix(Of Double)(nGenes, nPerts)
            counts(t) = New Double(nGenes - 1) {}

            For g As Integer = 0 To nGenes - 1
                counts(t)(g) = 50.0 + rnd.NextDouble() * 200.0

                For p As Integer = 0 To nPerts - 1
                    stack(t)(g)(p) = If(g = p, -3.0 - rnd.NextDouble(), 0.5 * (rnd.NextDouble() - 0.5))
                Next
            Next
        Next

        Dim model As ControlResidual = New ControlResidual(options).Fit(stack, counts, genes, perts)

        Call DebugAudit.Mark("m3.residual_rank", model.Rank)
        DebugAudit.Test(model.Rank > 0, "the residual model should keep a non-zero rank")
        DebugAudit.Test(model.Supported.Any(Function(b) b), "at least one gene should be supported")

        Dim query As Double()() = {New Double() {0.0, 0.0}, New Double() {1.0, -1.0}, New Double() {0.0, 0.5},
                                   New Double() {-0.8, 0.0}, New Double() {0.0, 0.0}}
        Dim ctrlCounts As Double() = {80.0, 120.0, 60.0, 200.0, 40.0}
        Dim predicted As Double()() = model.Predict(query, ctrlCounts, genes, perts)

        For g As Integer = 0 To nGenes - 1
            For p As Integer = 0 To nPerts - 1
                If query(g)(p) = 0.0 Then
                    Call DebugAudit.AreEqual(0.0, predicted(g)(p), "a zero effect stays zero")
                    Continue For
                End If

                Dim ratio As Double = predicted(g)(p) / query(g)(p)

                DebugAudit.Test(ratio >= 0.5 - 1e-12 AndAlso ratio <= 2.0 + 1e-12,
                                $"the correction ratio stays in [0.5, 2] at ({g},{p}), got {ratio:F6}")
                DebugAudit.Test(std.Sign(predicted(g)(p)) = std.Sign(query(g)(p)), "the sign must not flip")
            Next
        Next

        ' 自身靶列必须原样保留
        Call DebugAudit.AreEqual(0.0, predicted(0)(0), "the own target keeps the query value")
        Call DebugAudit.AreEqual(-1.0, predicted(1)(1), "the own target keeps the query value")
    End Sub

#End Region

End Module
