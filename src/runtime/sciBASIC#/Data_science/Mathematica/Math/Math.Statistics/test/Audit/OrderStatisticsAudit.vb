Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.Math.Statistics.OrderStatistics
Imports sysmath = System.Math

''' <summary>
''' 序统计表与分位剖面的验收测试。
''' </summary>
Friend Module OrderStatisticsAudit

    Private Const Section As String = "orderstat"

    Friend Function Run() As Integer
        Dim failures As Integer = 0

        ' 3 个基因：gene0 单调、gene1 全并列、gene2 含零
        Dim perGene As Double()() = {
            New Double() {3.0, 1.0, 4.0, 2.0},
            New Double() {5.0, 5.0, 5.0, 5.0},
            New Double() {0.0, 0.0, 7.0, 2.0}
        }
        Dim table As OrderStatisticsTable = OrderStatisticsTable.CreateFromGenes(perGene, ValuePrecision.Float64)

        failures += Assert("gene count", table.GeneCount = 3, table.GeneCount.ToString)
        failures += Assert("cell count", table.CellCount = 4, table.CellCount.ToString)
        failures += Assert("precision", table.Precision = ValuePrecision.Float64)

        ' ---- 排序结果 ----
        Dim gene0 As Double() = table.RowCopy(0)
        Dim gene0Expected As Double() = {1.0, 2.0, 3.0, 4.0}

        For i As Integer = 0 To 3
            failures += AssertClose($"sorted gene0[{i}]", gene0(i), gene0Expected(i), 0.0, Section)
        Next

        ' ---- 均值 ----
        failures += AssertClose("MeanCpm(0)", table.MeanCpm(0), 2.5, 1.0E-15, Section)
        failures += AssertClose("MeanCpm(1)", table.MeanCpm(1), 5.0, 1.0E-15, Section)
        failures += AssertClose("MeanCpm(2)", table.MeanCpm(2), 2.25, 1.0E-15, Section)

        ' ---- 并列和 ----
        failures += AssertClose("TieSum(1) all tied", table.TieSum(1), 60.0, 0.0, Section)
        failures += AssertClose("TieSum(2) two zeros tied", table.TieSum(2), 6.0, 0.0, Section)
        failures += AssertClose("TieSum(0) no ties", table.TieSum(0), 0.0, 0.0, Section)

        ' ---- 二分定位 ----
        failures += Assert("LowerBound(1.5) = 1", table.LowerBound(0, 1.5) = 1, table.LowerBound(0, 1.5).ToString)
        failures += Assert("LowerBound(2.0) = 1", table.LowerBound(0, 2.0) = 1, table.LowerBound(0, 2.0).ToString)
        failures += Assert("UpperBound(2.0) = 2", table.UpperBound(0, 2.0) = 2, table.UpperBound(0, 2.0).ToString)
        failures += Assert("CountEqual(5) = 4", table.CountEqual(1, 5.0) = 4, table.CountEqual(1, 5.0).ToString)
        failures += Assert("CountEqual(0) = 2", table.CountEqual(2, 0.0) = 2, table.CountEqual(2, 0.0).ToString)
        failures += Assert("LowerBound below min = 0", table.LowerBound(0, -100.0) = 0)
        failures += Assert("UpperBound above max = 4", table.UpperBound(0, 100.0) = 4)

        ' ---- 名次取值与截断 ----
        failures += AssertClose("ValueAtIndex clamp low", table.ValueAtIndex(0, -5), 1.0, 0.0, Section)
        failures += AssertClose("ValueAtIndex clamp high", table.ValueAtIndex(0, 99), 4.0, 0.0, Section)
        failures += AssertClose("IndexedQuantile(0, 0.5)", table.IndexedQuantile(0, 0.5), 3.0, 0.0, Section)

        ' ---- 平移分位 ----
        failures += AssertClose("ShiftedQuantile below zero", table.ShiftedQuantile(0, 0.125, -0.5), 0.0, 0.0, Section)
        failures += AssertClose("ShiftedQuantile at max", table.ShiftedQuantile(0, 0.875, 0.5), 4.0, 0.0, Section)

        ' ---- type-7 分位 ----
        Dim sortedGene0 As Double() = table.RowCopy(0)

        For Each p As Double In {0.0, 0.1, 0.25, 0.5, 0.75, 0.9, 1.0}
            failures += AssertClose($"Type7({p})",
                                    table.Type7Quantile(0, p),
                                    Quantiles.Type7(sortedGene0, p),
                                    1.0E-15, Section)
        Next

        ' ---- 分层分位 ----
        Dim stratified As Double() = Quantiles.Stratified(4)

        failures += Assert("stratified count", stratified.Length = 4)
        failures += AssertClose("stratified[0]", stratified(0), 0.125, 1.0E-15, Section)
        failures += AssertClose("stratified[3]", stratified(3), 0.875, 1.0E-15, Section)

        ' ---- 分位剖面 ----
        Dim profile As QuantileProfile = QuantileProfile.Create({4.0, 1.0, 3.0, 2.0}, 4)

        failures += Assert("profile count", profile.Count = 4, profile.Count.ToString)
        failures += AssertClose("profile[0]", profile.Values(0), 1.375, 1.0E-15, Section)
        failures += AssertClose("profile[1]", profile.Values(1), 2.125, 1.0E-15, Section)
        failures += AssertClose("profile[2]", profile.Values(2), 2.875, 1.0E-15, Section)
        failures += AssertClose("profile[3]", profile.Values(3), 3.625, 1.0E-15, Section)

        Dim rescaled As QuantileProfile = profile.RescaleTo(10.0)
        Dim sum As Double = 0.0

        For i As Integer = 0 To rescaled.Count - 1
            sum += rescaled.Values(i)
        Next

        failures += AssertClose("RescaleTo total", sum, 10.0, 1.0E-12, Section)
        failures += AssertClose("RescaleTo keeps ratio", rescaled.Values(3) / rescaled.Values(0), 3.625 / 1.375, 1.0E-13, Section)

        ' 幂变换：指数的两个端点行为（1 = 恒等，0 = 全部塌缩到中位数），2 = 拉大离散度
        Dim identity As QuantileProfile = profile.Spread(1.0)

        For i As Integer = 0 To identity.Count - 1
            failures += AssertClose($"Spread(1) is identity [{i}]", identity.Values(i), profile.Values(i), 1.0E-15, Section)
        Next

        Dim collapsed As QuantileProfile = profile.Spread(0.0)

        For i As Integer = 0 To collapsed.Count - 1
            failures += AssertClose($"Spread(0) collapses to median [{i}]", collapsed.Values(i), 2.5, 1.0E-13, Section)
        Next

        Dim spread As QuantileProfile = profile.Spread(2.0)

        failures += Assert("Spread(2) widens the upper tail", spread.Values(3) > profile.Values(3))
        failures += Assert("Spread(2) shrinks the lower tail", spread.Values(0) < profile.Values(0))

        Dim spreadRescaled As QuantileProfile = spread.RescaleTo(10.0)
        Dim spreadSum As Double = 0.0

        For i As Integer = 0 To spreadRescaled.Count - 1
            spreadSum += spreadRescaled.Values(i)
        Next

        failures += AssertClose("Spread+RescaleTo total", spreadSum, 10.0, 1.0E-12, Section)

        ' ---- 边界：空输入 ----
        Dim emptyProfile As QuantileProfile = QuantileProfile.Create(New Double() {}, 5)

        failures += Assert("empty profile", emptyProfile.Count = 0)
        failures += Assert("empty profile mean is 0", emptyProfile.Mean = 0.0)

        Call DebugAudit.Mark($"{Section} audit completed", failures)

        Return failures
    End Function

End Module
