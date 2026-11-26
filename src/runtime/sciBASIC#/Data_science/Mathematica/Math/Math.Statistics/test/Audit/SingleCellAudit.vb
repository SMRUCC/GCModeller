Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed
Imports Microsoft.VisualBasic.Math.Statistics.OrderStatistics
Imports Microsoft.VisualBasic.Math.Statistics.SingleCell
Imports sysmath = System.Math

''' <summary>
''' 单细胞计数包装与计数统计算子的验收测试。
''' </summary>
Friend Module SingleCellAudit

    Private Const Section As String = "singlecell"

    ''' <summary>
    ''' 3 个细胞 × 4 个基因；细胞 1 是空细胞（库大小 0）
    ''' </summary>
    Private ReadOnly Counts As Double()() = {
        New Double() {1.0, 0.0, 2.0, 0.0},
        New Double() {0.0, 0.0, 0.0, 0.0},
        New Double() {2.0, 4.0, 0.0, 1.0}
    }

    Friend Function Run() As Integer
        Dim failures As Integer = 0
        Dim cells As New SingleCellCounts(CompressedBuilder.FromDense(Counts))

        failures += Assert("cells", cells.Cells = 3, cells.Cells.ToString)
        failures += Assert("genes", cells.Genes = 4, cells.Genes.ToString)
        failures += AssertClose("cell totals[0]", cells.CellTotals(0), 3.0, 0.0, Section)
        failures += AssertClose("cell totals[1]", cells.CellTotals(1), 0.0, 0.0, Section)
        failures += AssertClose("cell totals[2]", cells.CellTotals(2), 7.0, 0.0, Section)
        failures += AssertClose("gene sums[1]", cells.GeneSums(1), 4.0, 0.0, Section)
        failures += AssertClose("MedianDepth", cells.MedianDepth, 3.0, 0.0, Section)

        ' ---- 平均 CPM（与手算对拍）----
        Dim meanCpm As Double() = cells.MeanCpm()
        Dim gene0Expected As Double = (1000000.0 * 1.0 / 3.0 + 0.0 + 1000000.0 * 2.0 / 7.0) / 3.0
        Dim gene1Expected As Double = (0.0 + 0.0 + 1000000.0 * 4.0 / 7.0) / 3.0

        failures += AssertClose("mean cpm gene0", meanCpm(0), gene0Expected, 1.0E-8, Section)
        failures += AssertClose("mean cpm gene1", meanCpm(1), gene1Expected, 1.0E-8, Section)
        failures += AssertClose("mean cpm gene3", meanCpm(3), (1000000.0 * 1.0 / 7.0) / 3.0, 1.0E-8, Section)

        ' ---- 检出率 ----
        Dim rate As Double() = cells.DetectionRate(5.0)

        failures += AssertClose("detection gene0", rate(0), 2.0 / 3.0, 1.0E-12, Section)
        failures += AssertClose("detection gene1", rate(1), 1.0 / 3.0, 1.0E-12, Section)
        failures += AssertClose("detection gene2", rate(2), 1.0 / 3.0, 1.0E-12, Section)

        ' ---- 子集与拼接 ----
        Dim subset As SingleCellCounts = cells.Subset({0, 2})

        failures += Assert("subset cells", subset.Cells = 2, subset.Cells.ToString)
        failures += AssertClose("subset(1,1)", subset.Matrix.X(1, 1), 4.0, 0.0, Section)

        Dim appended As SingleCellCounts = SingleCellCounts.Append({cells, subset})

        failures += Assert("append cells", appended.Cells = 5, appended.Cells.ToString)
        failures += AssertClose("append(3,2)", appended.Matrix.X(3, 2), 2.0, 0.0, Section)

        ' ---- 非负整数契约 ----
        failures += AssertThrows("negative count rejected",
                                 Sub()
                                     Dim rejected As SingleCellCounts = New SingleCellCounts(
                                         CompressedBuilder.FromDense(New Double()() {New Double() {-1.0}}))
                                 End Sub)
        failures += AssertThrows("fractional count rejected",
                                 Sub()
                                     Dim rejected As SingleCellCounts = New SingleCellCounts(
                                         CompressedBuilder.FromDense(New Double()() {New Double() {2.5}}))
                                 End Sub)

        ' ---- 库大小剖面 ----
        Dim librarySizes As Double() = {1000.0, 1200.0, 800.0, 1500.0, 900.0, 0.0}
        Dim totalDepth As Double = 100 * 1000.0
        Dim profile As QuantileProfile = CountStatistics.LibraryProfile(librarySizes, totalDepth, 0.85, 100)
        Dim sum As Double = 0.0
        Dim allPositive As Boolean = True

        For i As Integer = 0 To profile.Count - 1
            sum += profile.Values(i)

            If profile.Values(i) <= 0.0 Then allPositive = False
        Next

        failures += Assert("library profile count", profile.Count = 100, profile.Count.ToString)
        failures += AssertClose("library profile total", sum, totalDepth, 1.0E-6, Section)
        failures += Assert("library profile drops empty cells", allPositive)

        ' ---- delete-one Jackknife 矩（与暴力实现对比）----
        Dim matrix As CsrMatrix = CompressedBuilder.FromDense(Counts)
        Dim moment As Double = CountStatistics.JackknifeMoment(matrix, 2, 50000.0)
        Dim expectedMoment As Double = BruteForceJackknifeMoment(Counts, 50000.0)

        failures += AssertClose("jackknife moment", moment, expectedMoment, 1.0E-8 * sysmath.Max(1.0, expectedMoment), Section)

        ' ---- 留一法方差 ----
        failures += AssertClose("leave-one-out variance",
                               CountStatistics.LeaveOneOutVariance({1.0, 2.0, 3.0}),
                               4.0 / 3.0, 1.0E-14, Section)

        ' ---- 深度分位 ----
        Dim quantiles As Double() = CountStatistics.DepthQuantiles({3.0, 0.0, 7.0}, {0.0, 0.5, 1.0})

        failures += AssertClose("depth quantile min", quantiles(0), 0.0, 0.0, Section)
        failures += AssertClose("depth quantile median", quantiles(1), 3.0, 0.0, Section)
        failures += AssertClose("depth quantile max", quantiles(2), 7.0, 0.0, Section)

        Call DebugAudit.Mark($"{Section} audit completed", failures)

        Return failures
    End Function

    ''' <summary>
    ''' 逐元素的暴力实现：直接按定义计算 <c>((n-1)/n) Σ_g Σ_i (v_ig - mean_g)²</c>
    ''' </summary>
    Private Function BruteForceJackknifeMoment(counts As Double()(), normalizer As Double) As Double
        Dim n As Integer = counts.Length
        Dim genes As Integer = counts(0).Length

        If n < 2 Then Return 0.0

        Dim geneSums As Double() = New Double(genes - 1) {}
        Dim total As Double = 0.0

        For i As Integer = 0 To n - 1
            For g As Integer = 0 To genes - 1
                geneSums(g) += counts(i)(g)
                total += counts(i)(g)
            Next
        Next

        Dim accumulator As Double = 0.0

        For g As Integer = 0 To genes - 1
            Dim values As Double() = New Double(n - 1) {}
            Dim mean As Double = 0.0

            For i As Integer = 0 To n - 1
                Dim librarySize As Double = 0.0

                For j As Integer = 0 To genes - 1
                    librarySize += counts(i)(j)
                Next

                Dim remaining As Double = sysmath.Max(total - librarySize, 1.0E-12)

                values(i) = sysmath.Log(1.0 + normalizer * (geneSums(g) - counts(i)(g)) / remaining)
                mean += values(i)
            Next

            mean /= n

            For i As Integer = 0 To n - 1
                accumulator += (values(i) - mean) * (values(i) - mean)
            Next
        Next

        Return (n - 1.0) / n * accumulator
    End Function

    Private Function AssertThrows(name As String, code As Action) As Integer
        Try
            Call code()
        Catch ex As Exception
            Return 0
        End Try

        Call DebugAudit.Check(False, $"assertion failed: {name} should throw but did not")

        Return 1
    End Function

End Module
