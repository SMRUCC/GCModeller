Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed
Imports stdf = System.Math

''' <summary>
''' 截断 PCA（<see cref="TruncatedPCA"/>）的自检：列均值、得分与载荷的一致性、
''' 投影与逆变换的往返、解释方差的分解、标准化口径、稀疏入口一致性、seed 可复现与异常输入防护。
''' </summary>
''' <remarks>
''' 断言一律使用 <see cref="DebugAudit"/> 中<b>不带</b> <c>&lt;Conditional("DEBUG")&gt;</c> 的入口
''' （<c>Test</c> / <c>AreEqual</c> / <c>CheckThrows</c> / <c>Mark</c>），因此 Release 下同样生效；
''' 结束时用 <see cref="DebugAudit.SaveReport"/> 导出 JSON 报告。
''' </remarks>
Public Module TruncatedPcaAudit

    Private Const ReportName As String = "truncated-pca-test-report.json"

    Private Const Samples As Integer = 120
    Private Const Features As Integer = 30
    Private Const TrueRank As Integer = 3
    Private Const SeedValue As Integer = 20261126

    ''' <summary>运行全部截断 PCA 审计用例并生成测试报告。</summary>
    ''' <returns>失败断言的数量（0 表示全部通过）。</returns>
    Public Function RunAll() As Integer
        Dim reportPath As String = Path.Combine(AppContext.BaseDirectory, ReportName)

        Call DebugAudit.Reset()
        DebugAudit.MaxEvents = 4096
        Call DebugAudit.Mark("truncated-pca audit started")

        Try
            Dim rnd As New Random(SeedValue)
            Dim data As Double()() = MakeData(rnd)

            Call CheckMeanAndComponents(data)
            Call CheckScoresAndProjection(data)
            Call CheckInverseTransform(data)
            Call CheckVarianceDecomposition(data)
            Call CheckStandardization(data)
            Call CheckSparseEntry(data)
            Call CheckDeterminism(data)
            Call CheckBoundary(data)
        Catch ex As Exception
            Call DebugAudit.Mark("truncated-pca.audit.crash", ex.ToString())
            Call DebugAudit.Test(False, $"TruncatedPCA audit should not throw unhandled exceptions: {ex.Message}")
        End Try

        Call DebugAudit.SaveReport(reportPath)
        Call Console.WriteLine($"   [audit] report -> {reportPath}")
        Call Console.WriteLine($"   [audit] {DebugAudit.FailureCount} failure(s)")

        Return DebugAudit.FailureCount
    End Function

#Region "cases"

    ''' <summary>列均值与载荷形状：秩 3 的数据请求 5 个主成分时只保留 3 个。</summary>
    Private Sub CheckMeanAndComponents(data As Double()())
        Dim pca As New TruncatedPCA(data, 5, center:=True, scale:=False, oversampling:=8, powerIterations:=2, seed:=13)
        Dim mean As Double() = pca.Mean
        Dim worst As Double = 0.0

        For j As Integer = 0 To Features - 1
            Dim acc As Double = 0.0

            For i As Integer = 0 To Samples - 1
                acc += data(i)(j)
            Next

            worst = stdf.Max(worst, stdf.Abs(mean(j) - acc / Samples))
        Next

        Call DebugAudit.Mark("pca.mean_error", worst)
        DebugAudit.Test(worst < 1e-12, $"the mean vector should equal the column means, max |diff| = {worst:E3}")

        Call DebugAudit.AreEqual(TrueRank, pca.RetainedRank, "a rank-3 matrix should retain 3 components")
        Call DebugAudit.AreEqual(TrueRank, pca.Components.Length, "Components should have r rows")
        Call DebugAudit.AreEqual(Features, pca.Components(0).Length, "Components should have p columns (gene space)")

        Dim orthogonality As Double = GramDeviation(pca.Components)
        Call DebugAudit.Mark("pca.components_orthogonality", orthogonality)
        DebugAudit.Test(orthogonality < 1e-10, $"||C·C' - I||_F = {orthogonality:E3} should be below 1e-10")

        Dim sigma As Double() = pca.SingularValues
        Dim descending As Boolean = True

        For i As Integer = 1 To sigma.Length - 1
            If sigma(i) > sigma(i - 1) + 1e-12 Then
                descending = False
            End If
        Next

        DebugAudit.Test(descending, "the singular values should be sorted in descending order")
        Call DebugAudit.AreEqual(Samples, pca.SampleCount, "SampleCount should be the row count")
        Call DebugAudit.AreEqual(Features, pca.FeatureCount, "FeatureCount should be the column count")
    End Sub

    ''' <summary>得分一致性与投影：Scores == (X − μ)·Componentsᵀ，且 Project(X) 与之一致。</summary>
    Private Sub CheckScoresAndProjection(data As Double()())
        Dim pca As New TruncatedPCA(data, TrueRank, True, False, 8, 2, 13)
        Dim scores As Double()() = pca.Scores
        Dim components As Double()() = pca.Components
        Dim mean As Double() = pca.Mean
        Dim worst As Double = 0.0

        For i As Integer = 0 To Samples - 1
            For c As Integer = 0 To TrueRank - 1
                Dim acc As Double = 0.0

                For j As Integer = 0 To Features - 1
                    acc += (data(i)(j) - mean(j)) * components(c)(j)
                Next

                worst = stdf.Max(worst, stdf.Abs(acc - scores(i)(c)))
            Next
        Next

        Call DebugAudit.Mark("pca.scores_error", worst)
        DebugAudit.Test(worst < 1e-10, $"Scores should equal (X - mean)·Components', max |diff| = {worst:E3}")

        Dim projected As Double()() = pca.Project(data)
        Dim projectionError As Double = MaxAbsDiff(projected, scores)

        Call DebugAudit.Mark("pca.project_error", projectionError)
        DebugAudit.Test(projectionError < 1e-10, $"Project(X) should reproduce Scores, max |diff| = {projectionError:E3}")
        Call DebugAudit.AreEqual(Samples, projected.Length, "Project should keep the sample count")
        Call DebugAudit.AreEqual(TrueRank, projected(0).Length, "Project should emit r components")
    End Sub

    ''' <summary>逆变换：k 覆盖全部有效秩时，还原应当精确。</summary>
    Private Sub CheckInverseTransform(data As Double()())
        Dim pca As New TruncatedPCA(data, TrueRank, True, False, 8, 2, 13)
        Dim restored As Double()() = pca.InverseTransform(pca.Scores)
        Dim relative As Double = RelativeFrobenius(data, restored)

        Call DebugAudit.Mark("pca.inverse_relative_error", relative)
        DebugAudit.Test(relative < 1e-10, $"InverseTransform(Project(X)) should restore X, relative error = {relative:E3}")
        Call DebugAudit.AreEqual(Samples, restored.Length, "InverseTransform should keep the sample count")
        Call DebugAudit.AreEqual(Features, restored(0).Length, "InverseTransform should restore the feature count")
    End Sub

    ''' <summary>解释方差：σ² 之和与总方差一致、比值自洽且单调。</summary>
    Private Sub CheckVarianceDecomposition(data As Double()())
        Dim pca As New TruncatedPCA(data, 5, True, False, 8, 2, 13)
        Dim variance As Double() = pca.ExplainedVariance
        Dim ratio As Double() = pca.ExplainedVarianceRatio
        Dim sigma As Double() = pca.SingularValues

        Dim total As Double = 0.0
        Dim ratioSum As Double = 0.0
        Dim worst As Double = 0.0

        For i As Integer = 0 To variance.Length - 1
            total += variance(i)
            ratioSum += ratio(i)
            worst = stdf.Max(worst, stdf.Abs(variance(i) - sigma(i) * sigma(i)))
        Next

        Call DebugAudit.Mark("pca.variance_total", pca.TotalVariance)
        Call DebugAudit.Mark("pca.variance_ratio_sum", ratioSum)
        Call DebugAudit.Mark("pca.variance_sigma_gap", worst)

        DebugAudit.Test(worst < 1e-12, $"ExplainedVariance should equal sigma^2, max |diff| = {worst:E3}")
        DebugAudit.Test(stdf.Abs(total / pca.TotalVariance - 1.0) < 1e-8,
                        $"the captured variance should be exhausted by the retained components: {total:E6} vs {pca.TotalVariance:E6}")
        DebugAudit.Test(stdf.Abs(ratioSum - 1.0) < 1e-8, $"the explained variance ratio should sum to 1, got {ratioSum:F12}")
        DebugAudit.Test(variance(0) > variance(variance.Length - 1),
                        "the first component should explain more variance than the last one")
    End Sub

    ''' <summary>标准化口径：<c>scale:=True</c> 之后每列的样本方差应当为 1（总体方差）。</summary>
    Private Sub CheckStandardization(data As Double()())
        Dim pca As New TruncatedPCA(data, TrueRank, center:=True, scale:=True, oversampling:=8, powerIterations:=2, seed:=13)
        Dim mean As Double() = pca.Mean
        Dim scale As Double() = pca.Scale
        Dim worst As Double = 0.0

        For j As Integer = 0 To Features - 1
            Dim acc As Double = 0.0

            For i As Integer = 0 To Samples - 1
                Dim z As Double = (data(i)(j) - mean(j)) / scale(j)

                acc += z * z
            Next

            worst = stdf.Max(worst, stdf.Abs(acc / Samples - 1.0))
        Next

        Call DebugAudit.Mark("pca.standardized_variance_gap", worst)
        DebugAudit.Test(worst < 1e-10, $"standardized columns should have unit variance, worst gap = {worst:E3}")
        DebugAudit.Test(pca.Standardized, "the model should report that it standardised the columns")

        ' 常数列必须被守卫成尺度 1，而不是产生 NaN
        Dim constant As Double()() = RectangularArray.Matrix(Of Double)(Samples, Features)

        For i As Integer = 0 To Samples - 1
            For j As Integer = 0 To Features - 1
                constant(i)(j) = 1.0 + j
            Next
        Next

        Dim guarded As New TruncatedPCA(constant, 1, True, True, 8, 2, 13)
        Dim finite As Boolean = True

        For j As Integer = 0 To Features - 1
            If Not Double.IsFinite(guarded.Scale(j)) OrElse guarded.Scale(j) <= 0.0 Then
                finite = False
            End If
        Next

        DebugAudit.Test(finite, "constant columns should keep a finite positive scale (no division by zero)")
    End Sub

    ''' <summary>稀疏入口与稠密入口必须一致。</summary>
    Private Sub CheckSparseEntry(data As Double()())
        Dim dense As New TruncatedPCA(data, TrueRank, True, False, 8, 2, 21)
        Dim sparse As New TruncatedPCA(CompressedBuilder.FromDense(data), TrueRank, True, False, 8, 2, 21)

        Dim componentDiff As Double = MaxAbsDiff(dense.Components, sparse.Components)
        Dim meanDiff As Double = MaxAbsDiff(dense.Mean, sparse.Mean)

        Call DebugAudit.Mark("pca.sparse_component_gap", componentDiff)
        Call DebugAudit.Mark("pca.sparse_mean_gap", meanDiff)
        DebugAudit.Test(componentDiff < 1e-10, $"the CSR entry should agree with the dense entry, diff = {componentDiff:E3}")
        DebugAudit.Test(meanDiff < 1e-12, $"the column means should agree, diff = {meanDiff:E3}")

        Dim projected As Double()() = sparse.Project(CompressedBuilder.FromDense(data))
        Dim projectionError As Double = MaxAbsDiff(projected, dense.Scores)

        Call DebugAudit.Mark("pca.sparse_projection_gap", projectionError)
        DebugAudit.Test(projectionError < 1e-10, $"Project(CSR) should reproduce the dense scores, diff = {projectionError:E3}")
    End Sub

    ''' <summary>同一 seed 的结果必须逐位一致。</summary>
    Private Sub CheckDeterminism(data As Double()())
        Dim first As New TruncatedPCA(data, 4, True, False, 8, 2, 77)
        Dim again As New TruncatedPCA(data, 4, True, False, 8, 2, 77)

        DebugAudit.Test(ExactEqual(first.Components, again.Components),
                        "the same seed must reproduce the loadings bit by bit")
        DebugAudit.Test(ExactEqual(first.SingularValues, again.SingularValues),
                        "the same seed must reproduce the singular values bit by bit")
    End Sub

    ''' <summary>异常输入与边界。</summary>
    Private Sub CheckBoundary(data As Double()())
        Call DebugAudit.CheckThrows(Of ArgumentException)(
            Sub()
                Dim bad As New TruncatedPCA(data, 0)
            End Sub, "k = 0 should be rejected")

        Call DebugAudit.CheckThrows(Of ArgumentException)(
            Sub()
                Dim bad As New TruncatedPCA(data, Samples + 1)
            End Sub, "k > min(n, p) should be rejected")

        Call DebugAudit.CheckThrows(Of ArgumentException)(
            Sub()
                Dim bad As New TruncatedPCA(data, 2, oversampling:=-1)
            End Sub, "a negative oversampling should be rejected")

        Call DebugAudit.CheckThrows(Of ArgumentException)(
            Sub()
                Dim bad As New TruncatedPCA(data, 2, powerIterations:=-1)
            End Sub, "a negative power iteration count should be rejected")

        ' 单成分模型
        Dim oneComponent As New TruncatedPCA(data, 1, True, False, 8, 2, 13)
        Call DebugAudit.AreEqual(1, oneComponent.RetainedRank, "k = 1 should retain a single component")
        Call DebugAudit.AreEqual(1, oneComponent.Components.Length, "Components should have exactly one row")

        ' 行数不一致的样本必须在投影时被拒绝
        Dim ragged As Double()() = {New Double() {1.0, 2.0}}

        Call DebugAudit.CheckThrows(Of ArgumentException)(
            Sub()
                Dim bad As Double()() = oneComponent.Project(ragged)
            End Sub, "Project should reject a sample with the wrong feature count")

        ' 空矩阵返回空结果而不是抛异常
        Dim empty As New TruncatedPCA(New Double(-1)() {}, 2)
        Call DebugAudit.AreEqual(0, empty.RetainedRank, "an empty matrix should yield an empty model")
    End Sub

#End Region

#Region "helpers"

    ''' <summary>
    ''' 构造精确秩为 <see cref="TrueRank"/> 的"样本 × 特征"数据：
    ''' <c>X = 1·μᵀ + S·L</c>，其中 S 为 n×r 的得分、L 为 r×p 的载荷。
    ''' </summary>
    Private Function MakeData(rnd As Random) As Double()()
        Dim scores As Double()() = RectangularArray.Matrix(Of Double)(Samples, TrueRank)
        Dim loadings As Double()() = RectangularArray.Matrix(Of Double)(TrueRank, Features)
        Dim mean(Features - 1) As Double

        For i As Integer = 0 To Samples - 1
            For t As Integer = 0 To TrueRank - 1
                scores(i)(t) = rnd.NextDouble() * 2.0 - 1.0
            Next
        Next

        For t As Integer = 0 To TrueRank - 1
            For j As Integer = 0 To Features - 1
                loadings(t)(j) = rnd.NextDouble() * 2.0 - 1.0
            Next
        Next

        For j As Integer = 0 To Features - 1
            mean(j) = 0.5 + j * 0.1
        Next

        Dim data As Double()() = RectangularArray.Matrix(Of Double)(Samples, Features)

        For i As Integer = 0 To Samples - 1
            For j As Integer = 0 To Features - 1
                Dim acc As Double = mean(j)

                For t As Integer = 0 To TrueRank - 1
                    acc += scores(i)(t) * loadings(t)(j)
                Next

                data(i)(j) = acc
            Next
        Next

        Return data
    End Function

    ''' <summary><c>‖QᵀQ − I‖_F</c>，Q 为 <c>k×p</c>（行正交）。</summary>
    Private Function GramDeviation(Q As Double()()) As Double
        If Q.Length = 0 Then
            Return 0.0
        End If

        Dim k As Integer = Q.Length
        Dim p As Integer = Q(0).Length
        Dim sum As Double = 0.0

        For a As Integer = 0 To k - 1
            For b As Integer = 0 To k - 1
                Dim acc As Double = 0.0

                For j As Integer = 0 To p - 1
                    acc += Q(a)(j) * Q(b)(j)
                Next

                If a = b Then
                    acc -= 1.0
                End If

                sum += acc * acc
            Next
        Next

        Return stdf.Sqrt(sum)
    End Function

    ''' <summary><c>‖X − Ŷ‖_F / ‖X‖_F</c>。</summary>
    Private Function RelativeFrobenius(X As Double()(), Y As Double()()) As Double
        Dim numerator As Double = 0.0
        Dim denominator As Double = 0.0

        For i As Integer = 0 To X.Length - 1
            For j As Integer = 0 To X(i).Length - 1
                Dim d As Double = X(i)(j) - Y(i)(j)

                numerator += d * d
                denominator += X(i)(j) * X(i)(j)
            Next
        Next

        Return If(denominator > 0.0, stdf.Sqrt(numerator / denominator), 0.0)
    End Function

    Private Function MaxAbsDiff(a As Double()(), b As Double()()) As Double
        Dim diff As Double = 0.0
        Dim rows As Integer = stdf.Min(a.Length, b.Length)

        For i As Integer = 0 To rows - 1
            Dim columns As Integer = stdf.Min(a(i).Length, b(i).Length)

            For j As Integer = 0 To columns - 1
                diff = stdf.Max(diff, stdf.Abs(a(i)(j) - b(i)(j)))
            Next
        Next

        Return diff
    End Function

    Private Function MaxAbsDiff(a As Double(), b As Double()) As Double
        Dim diff As Double = 0.0
        Dim count As Integer = stdf.Min(a.Length, b.Length)

        For i As Integer = 0 To count - 1
            diff = stdf.Max(diff, stdf.Abs(a(i) - b(i)))
        Next

        Return diff
    End Function

    Private Function ExactEqual(a As Double()(), b As Double()()) As Boolean
        If a.Length <> b.Length Then
            Return False
        End If

        For i As Integer = 0 To a.Length - 1
            If a(i).Length <> b(i).Length Then
                Return False
            End If

            For j As Integer = 0 To a(i).Length - 1
                If a(i)(j) <> b(i)(j) Then
                    Return False
                End If
            Next
        Next

        Return True
    End Function

    Private Function ExactEqual(a As Double(), b As Double()) As Boolean
        If a.Length <> b.Length Then
            Return False
        End If

        For i As Integer = 0 To a.Length - 1
            If a(i) <> b(i) Then
                Return False
            End If
        Next

        Return True
    End Function

#End Region

End Module
