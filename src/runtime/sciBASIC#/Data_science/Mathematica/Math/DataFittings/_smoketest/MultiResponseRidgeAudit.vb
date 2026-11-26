Imports System
Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.Bootstrapping.Ridge
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports std = System.Math

''' <summary>
''' 多响应岭回归（<see cref="MultiResponseRidge"/>）的自检：与 QR 最小二乘对拍、
''' 行掩码等价性、收缩单调性、秩亏稳健性、逐组批量入口与并行确定性、诊断量与异常输入防护。
''' </summary>
''' <remarks>
''' <para>
''' 断言一律使用 <see cref="DebugAudit"/> 中<b>不带</b> <c>&lt;Conditional("DEBUG")&gt;</c> 的入口
''' （<c>Test</c> / <c>AreEqual</c> / <c>CheckThrows</c> / <c>Mark</c>），因此 Release 下同样生效；
''' 结束时用 <see cref="DebugAudit.SaveReport"/> 导出 JSON 报告。
''' </para>
''' <para>
''' 数据按"无截距 + 响应已零均值"的约定生成：先抽设计矩阵 <c>X</c>，取列均值 <c>μ</c>，
''' 再令 <c>Y = (X − μ)·B₀</c>。这样 λ → 0 时模型应当精确还原 <c>B₀</c>，
''' 与上游（CellularAI 的残差坐标本身就是中心化后的量）的语义一致。
''' </para>
''' </remarks>
Public Module MultiResponseRidgeAudit

    Private Const ReportName As String = "multi-response-ridge-test-report.json"

    Private Const Rows As Integer = 200
    Private Const Features As Integer = 4
    Private Const Responses As Integer = 3

    ''' <summary>运行全部多响应岭回归审计用例并生成测试报告。</summary>
    ''' <returns>失败断言的数量（0 表示全部通过）。</returns>
    Public Function Run() As Integer
        Dim reportPath As String = Path.Combine(AppContext.BaseDirectory, ReportName)

        Call DebugAudit.Reset()
        DebugAudit.MaxEvents = 8192
        Call DebugAudit.Mark("multi-response ridge audit started")

        Dim rnd As New Random(20261126)

        Try
            Call CheckLeastSquaresMatch(rnd)
            Call CheckMaskEquivalence(rnd)
            Call CheckShrinkage(rnd)
            Call CheckRankDeficiency(rnd)
            Call CheckPrediction(rnd)
            Call CheckBatch(rnd)
            Call CheckBoundary(rnd)
        Catch ex As Exception
            Call DebugAudit.Mark("multi-response-ridge.audit.crash", ex.ToString())
            Call DebugAudit.Test(False, $"the multi-response ridge audit should not throw unhandled exceptions: {ex.Message}")
        End Try

        Call DebugAudit.SaveReport(reportPath)
        Call Console.WriteLine($"   [audit] report -> {reportPath}")
        Call Console.WriteLine($"   [audit] {DebugAudit.FailureCount} failure(s)")

        Return DebugAudit.FailureCount
    End Function

#Region "cases"

    ''' <summary>λ → 0 时必须退化为普通最小二乘（用 QR 求解作为独立参考）。</summary>
    Private Sub CheckLeastSquaresMatch(rnd As Random)
        Dim x As Double()() = Nothing
        Dim y As Double()() = Nothing
        Dim b0 As Double()() = MakeDesign(rnd, x, y, correlated:=False)
        Dim fit As MultiResponseRidgeFit = MultiResponseRidge.Fit(x, y, lambda:=0.00000001, center:=True)

        Dim reference As Double()() = LeastSquares(x, y, fit.FeatureCenter)
        Dim gap As Double = MaxAbsDiff(fit.Coefficients, reference)

        Call DebugAudit.Mark("ridge.ls_gap", gap)
        DebugAudit.Test(gap < 1e-8, $"a tiny lambda should reproduce the least squares solution, max |diff| = {gap:E3}")

        Dim recovery As Double = MaxAbsDiff(fit.Coefficients, b0)

        Call DebugAudit.Mark("ridge.coefficient_recovery", recovery)
        DebugAudit.Test(recovery < 1e-8, $"the coefficients should recover the generating matrix, max |diff| = {recovery:E3}")
        Call DebugAudit.AreEqual(Rows, fit.SampleCount, "all rows should participate when no mask is given")
        Call DebugAudit.AreEqual(Features, fit.Rank, "the normal equation should be full rank")
        DebugAudit.Test(fit.RSquared.All(Function(v) v > 0.999999), $"R^2 should be 1 on noise-free data, got {fit.RSquared(0):F9}")
        DebugAudit.Test(fit.DegreesOfFreedom > 0.0 AndAlso fit.DegreesOfFreedom <= Features,
                        $"the effective degrees of freedom {fit.DegreesOfFreedom:F4} should stay within [0, f]")
        DebugAudit.Test(fit.Supported, "the fit should be marked as supported")
    End Sub

    ''' <summary>行掩码必须等价于"先切片再拟合"。</summary>
    Private Sub CheckMaskEquivalence(rnd As Random)
        Dim x As Double()() = Nothing
        Dim y As Double()() = Nothing
        Dim b0 As Double()() = MakeDesign(rnd, x, y, correlated:=False)

        Dim mask(Rows - 1) As Boolean
        Dim kept As New List(Of Integer)

        For i As Integer = 0 To Rows - 1
            mask(i) = (i Mod 5) <> 0

            If mask(i) Then
                Call kept.Add(i)
            End If
        Next

        Dim full As MultiResponseRidgeFit = MultiResponseRidge.Fit(x, y, mask, lambda:=0.5, center:=True)
        Dim sliced As MultiResponseRidgeFit = MultiResponseRidge.Fit(Slice(x, kept), Slice(y, kept), Nothing, 0.5, True)

        Dim gap As Double = MaxAbsDiff(full.Coefficients, sliced.Coefficients)
        Dim centerGap As Double = MaxAbsDiff(full.FeatureCenter, sliced.FeatureCenter)

        Call DebugAudit.Mark("ridge.mask_coefficient_gap", gap)
        Call DebugAudit.Mark("ridge.mask_center_gap", centerGap)
        DebugAudit.Test(gap < 1e-12, $"the row mask should equal slicing, max |diff| = {gap:E3}")
        DebugAudit.Test(centerGap < 1e-12, $"the feature center should be computed on the masked rows only, max |diff| = {centerGap:E3}")
        Call DebugAudit.AreEqual(kept.Count, full.SampleCount, "SampleCount should count the masked rows only")
    End Sub

    ''' <summary>惩罚越强，系数的 Frobenius 范数越小。</summary>
    Private Sub CheckShrinkage(rnd As Random)
        Dim x As Double()() = Nothing
        Dim y As Double()() = Nothing

        Call MakeDesign(rnd, x, y, correlated:=False)

        Dim weak As MultiResponseRidgeFit = MultiResponseRidge.Fit(x, y, Nothing, 1.0, True)
        Dim strong As MultiResponseRidgeFit = MultiResponseRidge.Fit(x, y, Nothing, 100.0, True)
        Dim normWeak As Double = Frobenius(weak.Coefficients)
        Dim normStrong As Double = Frobenius(strong.Coefficients)

        Call DebugAudit.Mark("ridge.norm_lambda1", normWeak)
        Call DebugAudit.Mark("ridge.norm_lambda100", normStrong)
        DebugAudit.Test(normStrong < normWeak, $"a stronger penalty should shrink the coefficients: {normStrong:E3} < {normWeak:E3}")

        Dim r2Weak As Double = weak.RSquared(0)
        Dim r2Strong As Double = strong.RSquared(0)

        Call DebugAudit.Mark("ridge.r2_lambda1", r2Weak)
        Call DebugAudit.Mark("ridge.r2_lambda100", r2Strong)
        DebugAudit.Test(r2Strong <= r2Weak + 1e-12, $"a stronger penalty should not improve the training fit: {r2Strong:F6} <= {r2Weak:F6}")
    End Sub

    ''' <summary>共线设计矩阵在 λ &gt; 0 时必须给出有限系数（不产生 NaN/Inf）。</summary>
    Private Sub CheckRankDeficiency(rnd As Random)
        Dim x As Double()() = Nothing
        Dim y As Double()() = Nothing

        Call MakeDesign(rnd, x, y, correlated:=True)

        Dim fit As MultiResponseRidgeFit = MultiResponseRidge.Fit(x, y, Nothing, 10.0, True)
        Dim finite As Boolean = True

        For j As Integer = 0 To Features - 1
            For c As Integer = 0 To Responses - 1
                If Not Double.IsFinite(fit.Coefficients(j)(c)) Then
                    finite = False
                End If
            Next
        Next

        Call DebugAudit.Mark("ridge.rank_deficient_rank", fit.Rank)
        DebugAudit.Test(finite, "the coefficients must stay finite on a collinear design")
        DebugAudit.Test(fit.Supported, "a collinear design should still be solvable because lambda > 0")
        DebugAudit.Test(fit.Rank <= Features, $"the reported rank {fit.Rank} should not exceed f")
    End Sub

    ''' <summary>预测接口：无噪声数据上 ŷ 应当还原 y。</summary>
    Private Sub CheckPrediction(rnd As Random)
        Dim x As Double()() = Nothing
        Dim y As Double()() = Nothing

        Call MakeDesign(rnd, x, y, correlated:=False)

        Dim fit As MultiResponseRidgeFit = MultiResponseRidge.Fit(x, y, Nothing, 0.000001, True)
        Dim predicted As Double()() = MultiResponseRidge.Predict(fit, x)
        Dim gap As Double = MaxAbsDiff(predicted, y)

        Call DebugAudit.Mark("ridge.predict_gap", gap)
        DebugAudit.Test(gap < 1e-8, $"the predictions should reproduce the responses, max |diff| = {gap:E3}")

        Dim oneRow As Double() = fit.Predict(x(0))
        Dim singleGap As Double = 0.0

        For c As Integer = 0 To Responses - 1
            singleGap = std.Max(singleGap, std.Abs(oneRow(c) - predicted(0)(c)))
        Next

        DebugAudit.Test(singleGap = 0.0, "Predict(vector) and Predict(matrix) should agree bit by bit")
        Call DebugAudit.AreEqual(Rows, predicted.Length, "the prediction should keep the row count")
        Call DebugAudit.AreEqual(Responses, predicted(0).Length, "the prediction should keep the response count")
    End Sub

    ''' <summary>逐组批量入口：与逐组手算一致、并行与串行逐位一致、最小行数门限生效。</summary>
    Private Sub CheckBatch(rnd As Random)
        Dim genes As Integer = 5
        Dim records As Integer = 80
        Dim featuresPerRow As Integer = 3
        Dim responsesPerRow As Integer = 2

        Dim features(genes - 1)()() As Double
        Dim responses(genes - 1)()() As Double
        Dim support(records - 1, genes - 1) As Boolean

        For g As Integer = 0 To genes - 1
            Dim block As Double()() = RectangularArray.Matrix(Of Double)(records, featuresPerRow)
            Dim target As Double()() = RectangularArray.Matrix(Of Double)(records, responsesPerRow)

            For i As Integer = 0 To records - 1
                For j As Integer = 0 To featuresPerRow - 1
                    block(i)(j) = rnd.NextDouble() * 2.0 - 1.0
                Next
            Next

            ' Y = (X − μ)·B0，保证无截距模型能精确表达
            Dim means(featuresPerRow - 1) As Double

            For j As Integer = 0 To featuresPerRow - 1
                For i As Integer = 0 To records - 1
                    means(j) += block(i)(j)
                Next

                means(j) /= records
            Next

            Dim b0(featuresPerRow - 1)() As Double

            For j As Integer = 0 To featuresPerRow - 1
                b0(j) = New Double(responsesPerRow - 1) {}

                For c As Integer = 0 To responsesPerRow - 1
                    b0(j)(c) = rnd.NextDouble() * 2.0 - 1.0
                Next
            Next

            For i As Integer = 0 To records - 1
                For c As Integer = 0 To responsesPerRow - 1
                    Dim acc As Double = 0.0

                    For j As Integer = 0 To featuresPerRow - 1
                        acc += (block(i)(j) - means(j)) * b0(j)(c)
                    Next

                    target(i)(c) = acc
                Next
            Next

            features(g) = block
            responses(g) = target
        Next

        For i As Integer = 0 To records - 1
            For g As Integer = 0 To genes - 1
                ' 第 4 组（下标 3）只保留 5 行，用来验证 minimumRows 门限
                support(i, g) = If(g = 3, i < 5, (i + g) Mod 7 <> 0)
            Next
        Next

        Dim batch As MultiResponseRidgeFit() = MultiResponseRidge.BatchFit(
            features, responses, support, lambda:=2.0, minimumRows:=32, degreeOfParallelism:=1)
        Dim parallel As MultiResponseRidgeFit() = MultiResponseRidge.BatchFit(
            features, responses, support, lambda:=2.0, minimumRows:=32, degreeOfParallelism:=0)

        Call DebugAudit.AreEqual(genes, batch.Length, "BatchFit should return one result per group")

        Dim mismatch As Integer = 0
        Dim serialGap As Double = 0.0

        For g As Integer = 0 To genes - 1
            Dim mask(records - 1) As Boolean
            Dim kept As New List(Of Integer)

            For i As Integer = 0 To records - 1
                mask(i) = support(i, g)

                If mask(i) Then
                    Call kept.Add(i)
                End If
            Next

            If batch(g).Supported Then
                Dim manual As MultiResponseRidgeFit = MultiResponseRidge.Fit(
                    Slice(features(g), kept), Slice(responses(g), kept), Nothing, 2.0, True)

                If MaxAbsDiff(batch(g).Coefficients, manual.Coefficients) > 1e-12 Then
                    mismatch += 1
                End If
            End If

            serialGap = std.Max(serialGap, MaxAbsDiff(batch(g).Coefficients, parallel(g).Coefficients))
        Next

        Call DebugAudit.AreEqual(0, mismatch, "BatchFit should agree with the manual per-group fit")
        Call DebugAudit.Mark("ridge.batch_parallel_gap", serialGap)
        DebugAudit.Test(serialGap = 0.0, "the parallel and serial batch results must be bit identical")

        DebugAudit.Test(Not batch(3).Supported, "a group with fewer rows than minimumRows should be unsupported")
        Call DebugAudit.AreEqual(5, batch(3).SampleCount, "the unsupported group should still report its row count")
        DebugAudit.Test(IsAllZero(batch(3).Coefficients), "the unsupported group should keep all-zero coefficients")

        Dim supportedGroups As Integer = 0

        For g As Integer = 0 To genes - 1
            If batch(g).Supported Then
                supportedGroups += 1
            End If
        Next

        Call DebugAudit.Mark("ridge.batch_supported_groups", supportedGroups)
        DebugAudit.Test(supportedGroups = genes - 1, $"only the short group should be unsupported, got {supportedGroups} supported")
    End Sub

    ''' <summary>异常输入防护。</summary>
    Private Sub CheckBoundary(rnd As Random)
        Dim x As Double()() = Nothing
        Dim y As Double()() = Nothing

        Call MakeDesign(rnd, x, y, correlated:=False)

        Call DebugAudit.CheckThrows(Of ArgumentException)(
            Sub()
                Dim bad = MultiResponseRidge.Fit(x, y, Nothing, 0.0, True)
            End Sub, "lambda = 0 should be rejected")

        Call DebugAudit.CheckThrows(Of ArgumentException)(
            Sub()
                Dim bad = MultiResponseRidge.Fit(x, y, Nothing, -1.0, True)
            End Sub, "a negative lambda should be rejected")

        Call DebugAudit.CheckThrows(Of ArgumentException)(
            Sub()
                Dim bad = MultiResponseRidge.Fit(x, y, New Boolean(10) {}, 1.0, True)
            End Sub, "a mask with the wrong length should be rejected")

        Call DebugAudit.CheckThrows(Of ArgumentException)(
            Sub()
                Dim bad = MultiResponseRidge.Fit(x, Slice(y, New List(Of Integer) From {0, 1}), Nothing, 1.0, True)
            End Sub, "a response matrix with the wrong row count should be rejected")

        Call DebugAudit.CheckThrows(Of InvalidOperationException)(
            Sub()
                Dim none(Rows - 1) As Boolean
                Dim bad = MultiResponseRidge.Fit(x, y, none, 1.0, True)
            End Sub, "a mask selecting no row should be rejected")

        ' 行掩码为 Nothing 与"全 True 掩码"必须等价
        Dim all(Rows - 1) As Boolean

        For i As Integer = 0 To Rows - 1
            all(i) = True
        Next

        Dim without = MultiResponseRidge.Fit(x, y, Nothing, 3.0, True)
        Dim withMask = MultiResponseRidge.Fit(x, y, all, 3.0, True)

        DebugAudit.Test(MaxAbsDiff(without.Coefficients, withMask.Coefficients) = 0.0,
                        "an all-true mask must equal the mask-less fit bit by bit")
    End Sub

#End Region

#Region "helpers"

    ''' <summary>
    ''' 生成设计矩阵与"可被无截距模型精确表达"的响应：<c>Y = (X − μ)·B₀</c>。
    ''' </summary>
    ''' <param name="correlated">True 时把第 4 列设成第 1 列的 2 倍，制造共线</param>
    ''' <returns>生成用的 <c>B₀</c>（f×k）</returns>
    Private Function MakeDesign(rnd As Random, ByRef x As Double()(), ByRef y As Double()(),
                                correlated As Boolean) As Double()()
        Dim design As Double()() = RectangularArray.Matrix(Of Double)(Rows, Features)

        For i As Integer = 0 To Rows - 1
            For j As Integer = 0 To Features - 1
                design(i)(j) = rnd.NextDouble() * 2.0 - 1.0
            Next

            If correlated Then
                design(i)(3) = design(i)(0) * 2.0
            End If
        Next

        Dim means(Features - 1) As Double

        For j As Integer = 0 To Features - 1
            For i As Integer = 0 To Rows - 1
                means(j) += design(i)(j)
            Next

            means(j) /= Rows
        Next

        Dim b0(Features - 1)() As Double

        For j As Integer = 0 To Features - 1
            b0(j) = New Double(Responses - 1) {}

            For c As Integer = 0 To Responses - 1
                b0(j)(c) = rnd.NextDouble() * 2.0 - 1.0
            Next
        Next

        Dim target As Double()() = RectangularArray.Matrix(Of Double)(Rows, Responses)

        For i As Integer = 0 To Rows - 1
            For c As Integer = 0 To Responses - 1
                Dim acc As Double = 0.0

                For j As Integer = 0 To Features - 1
                    acc += (design(i)(j) - means(j)) * b0(j)(c)
                Next

                target(i)(c) = acc
            Next
        Next

        x = design
        y = target

        Return b0
    End Function

    ''' <summary>
    ''' 独立的普通最小二乘参考：把中心化后的设计矩阵交给 <see cref="NumericMatrix.Solve(GeneralMatrix)"/>，
    ''' 非方阵时走 QR 分解，与正规方程是两条不同的数值路径。
    ''' </summary>
    Private Function LeastSquares(x As Double()(), y As Double()(), center As Double()) As Double()()
        Dim n As Integer = x.Length
        Dim f As Integer = x(0).Length
        Dim k As Integer = y(0).Length
        Dim design As Double()() = RectangularArray.Matrix(Of Double)(n, f)

        For i As Integer = 0 To n - 1
            For j As Integer = 0 To f - 1
                design(i)(j) = x(i)(j) - center(j)
            Next
        Next

        Dim solution As GeneralMatrix = New NumericMatrix(design, n, f).Solve(New NumericMatrix(y, n, k))
        Dim values As Double()() = solution.ArrayPack()
        Dim result As Double()() = RectangularArray.Matrix(Of Double)(f, k)

        For j As Integer = 0 To f - 1
            For c As Integer = 0 To k - 1
                result(j)(c) = values(j)(c)
            Next
        Next

        Return result
    End Function

    Private Function Slice(data As Double()(), rows As List(Of Integer)) As Double()()
        Dim result As Double()() = RectangularArray.Matrix(Of Double)(rows.Count, If(data.Length = 0, 0, data(0).Length))

        For i As Integer = 0 To rows.Count - 1
            Dim source As Double() = data(rows(i))

            For j As Integer = 0 To source.Length - 1
                result(i)(j) = source(j)
            Next
        Next

        Return result
    End Function

    Private Function Frobenius(B As Double()()) As Double
        Dim sum As Double = 0.0

        For j As Integer = 0 To B.Length - 1
            For c As Integer = 0 To B(j).Length - 1
                sum += B(j)(c) * B(j)(c)
            Next
        Next

        Return std.Sqrt(sum)
    End Function

    Private Function IsAllZero(B As Double()()) As Boolean
        For j As Integer = 0 To B.Length - 1
            For c As Integer = 0 To B(j).Length - 1
                If B(j)(c) <> 0.0 Then
                    Return False
                End If
            Next
        Next

        Return True
    End Function

    Private Function MaxAbsDiff(a As Double()(), b As Double()()) As Double
        Dim diff As Double = 0.0
        Dim rows As Integer = std.Min(a.Length, b.Length)

        For i As Integer = 0 To rows - 1
            Dim columns As Integer = std.Min(a(i).Length, b(i).Length)

            For j As Integer = 0 To columns - 1
                diff = std.Max(diff, std.Abs(a(i)(j) - b(i)(j)))
            Next
        Next

        Return diff
    End Function

    Private Function MaxAbsDiff(a As Double(), b As Double()) As Double
        Dim diff As Double = 0.0
        Dim count As Integer = std.Min(a.Length, b.Length)

        For i As Integer = 0 To count - 1
            diff = std.Max(diff, std.Abs(a(i) - b(i)))
        Next

        Return diff
    End Function

#End Region

End Module
