Imports System.Globalization
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.HTS.GSEA.KOBAS
Imports std = System.Math

Namespace Global.KOBAS.test

    ''' <summary>
    ''' SNBEA（PLAGE / GAGE / 数据准备）的自检。
    ''' </summary>
    Public Module SnbeaTest

        ''' <summary>
        ''' 执行 SNBEA 的全部断言。
        ''' </summary>
        Public Sub Run()
            Call Console.WriteLine("[snbea] 归一化 / 基因集映射 / PLAGE / GAGE")

            Call NormalizationCase()
            Call EnrichmentCase()
        End Sub

        ''' <summary>
        ''' 表达矩阵预变换的断言。
        ''' </summary>
        Private Sub NormalizationCase()
            Dim expr = KobasSelfTest.MakeMatrix(
                {"g1", "g2"},
                {"s1", "s2"},
                New Double()() {
                    New Double() {1, 3},
                    New Double() {7, 15}
                })
            Dim logged = SnbeaData.Log2(expr, 1)

            Call KobasSelfTest.CheckClose("log2(x+1) 变换（第 1 行）", 1.0, logged.expression(0).experiments(0), 1.0E-12)
            Call KobasSelfTest.CheckClose("log2(x+1) 变换（第 2 行）", 4.0, logged.expression(1).experiments(1), 1.0E-12)

            Dim counts = KobasSelfTest.MakeMatrix(
                {"g1", "g2", "g3"},
                {"s1", "s2"},
                New Double()() {
                    New Double() {10, 100},
                    New Double() {20, 200},
                    New Double() {30, 300}
                })
            Dim cpm = SnbeaData.CpmLog(counts, 3)

            Call KobasSelfTest.Check("对数 CPM 结果有限", cpm.expression.All(Function(row) row.experiments.All(Function(x) Not Double.IsNaN(x) AndAlso Not Double.IsInfinity(x))))
            Call KobasSelfTest.Check("对数 CPM 保持单调性", cpm.expression(0).experiments(0) < cpm.expression(1).experiments(0))

            Dim quantiled = SnbeaData.QuantileNormalize(
                KobasSelfTest.MakeMatrix(
                    {"g1", "g2", "g3"},
                    {"s1", "s2"},
                    New Double()() {
                        New Double() {1, 3},
                        New Double() {2, 2},
                        New Double() {3, 1}
                    }))
            Dim sortedFirst As Double() = quantiled.expression.Select(Function(row) row.experiments(0)).OrderBy(Function(x) x).ToArray
            Dim sortedSecond As Double() = quantiled.expression.Select(Function(row) row.experiments(1)).OrderBy(Function(x) x).ToArray

            Call KobasSelfTest.Check("分位数归一化之后两列分布一致",
                sortedFirst.Zip(sortedSecond, Function(x, y) std.Abs(x - y) < 1.0E-12).All(Function(flag) flag))

            Dim prepared = SnbeaData.Prepare(expr, ExpressionDataType.RnaSeq, normalize:=False)

            Call KobasSelfTest.CheckClose("Prepare 对 RNA-seq 默认只做 log2(x+1)", logged.expression(1).experiments(1), prepared.expression(1).experiments(1), 1.0E-12)

            ' 基因集映射与尺寸过滤
            Dim background As Background = KobasSelfTest.MakeBackground(
                New Dictionary(Of String, String()) From {
                    {"SET_A", {"g1", "g2", "g3", "missing"}},
                    {"SET_B", {"g1", "g2"}}
                })
            Dim mapping As Dictionary(Of String, Integer()) = counts.MapGeneSets(background, 3, 500)

            Call KobasSelfTest.Check("基因集映射只保留达到尺寸阈值的集合", mapping.Count = 1 AndAlso mapping.ContainsKey("SET_A"), $"{mapping.Count} 个")
            Call KobasSelfTest.Check("基因集映射剔除缺失基因", mapping.ContainsKey("SET_A") AndAlso mapping("SET_A").Length = 3)
        End Sub

        ''' <summary>
        ''' PLAGE 与 GAGE 的端到端断言。
        ''' </summary>
        Private Sub EnrichmentCase()
            Const geneCount As Integer = 40
            Const sampleCount As Integer = 12

            Dim genes As String() = Enumerable.Range(0, geneCount).Select(Function(i) $"G{i:00}").ToArray
            Dim samples As String() = Enumerable.Range(0, sampleCount).Select(Function(i) $"S{i:00}").ToArray
            Dim labels As Integer() = Enumerable.Range(0, sampleCount).Select(Function(i) If(i < sampleCount / 2, 0, 1)).ToArray
            Dim noise As Double() = KobasSelfTest.DeterministicUniform(19700101, geneCount * sampleCount)
            Dim values As Double()() = New Double(geneCount - 1)() {}

            For g As Integer = 0 To geneCount - 1
                values(g) = New Double(sampleCount - 1) {}

                For s As Integer = 0 To sampleCount - 1
                    Dim signal As Double = If(g < 15, If(labels(s) = 1, 2.0, -2.0), 0.0)

                    values(g)(s) = signal + noise(g * sampleCount + s)
                Next
            Next

            Dim expr = KobasSelfTest.MakeMatrix(genes, samples, values)
            Dim pheno As phenotype = KobasSelfTest.MakePhenotype(labels)
            Dim background As Background = KobasSelfTest.MakeBackground(
                New Dictionary(Of String, String()) From {
                    {"BIG", genes.Take(15).ToArray},
                    {"SMALL", genes.Skip(15).Take(8).ToArray},
                    {"FLAT", genes.Skip(25).Take(12).ToArray}
                })

            Dim plage As SnbeaResult() = PlageAnalysis.Compute(expr, background, pheno, 5, 500, FdrMethod.BH)

            Call KobasSelfTest.Check("PLAGE 结果数量", plage.Length = 3, $"{plage.Length} 条")
            Call KobasSelfTest.Check("PLAGE p 值范围", plage.All(Function(row) row.PValue >= 0 AndAlso row.PValue <= 1))
            Call KobasSelfTest.Check("PLAGE 校正 p 值范围", plage.All(Function(row) row.AdjustedPValue >= 0 AndAlso row.AdjustedPValue <= 1))
            Call KobasSelfTest.Check("PLAGE 结果按 p 值升序", plage(0).PValue <= plage(plage.Length - 1).PValue)

            Dim plageBig As SnbeaResult = plage.First(Function(row) row.ID = "BIG")
            Dim plageFlat As SnbeaResult = plage.First(Function(row) row.ID = "FLAT")

            Call KobasSelfTest.Check("PLAGE 检出植入信号", plageBig.PValue < 0.05, $"p={plageBig.PValue.ToString("G4", CultureInfo.InvariantCulture)}")
            Call KobasSelfTest.Check("PLAGE 信号强于噪声通路", plageBig.PValue < plageFlat.PValue,
                $"signal={plageBig.PValue.ToString("G4", CultureInfo.InvariantCulture)} flat={plageFlat.PValue.ToString("G4", CultureInfo.InvariantCulture)}")

            Dim gage As SnbeaResult() = GageAnalysis.Compute(expr, background, pheno, 5, 500, FdrMethod.BH)

            Call KobasSelfTest.Check("GAGE 结果数量", gage.Length = 3, $"{gage.Length} 条")
            Call KobasSelfTest.Check("GAGE p 值范围", gage.All(Function(row) row.PValue >= 0 AndAlso row.PValue <= 1))

            Dim gageBig As SnbeaResult = gage.First(Function(row) row.ID = "BIG")

            Call KobasSelfTest.Check("GAGE 检出植入信号（上调）",
                gageBig.PValue < 0.05 AndAlso gageBig.Statistic > 0,
                $"z={gageBig.Statistic.ToString("F3", CultureInfo.InvariantCulture)} p={gageBig.PValue.ToString("G4", CultureInfo.InvariantCulture)}")

            ' PLAGE 的活性分数矩阵形状
            Dim mapping As Dictionary(Of String, Integer()) = expr.MapGeneSets(background, 5, 500)
            Dim scores As Double()() = expr.PlageScores(mapping)

            Call KobasSelfTest.Check("PLAGE 分数矩阵形状", scores.Length = mapping.Count AndAlso scores.All(Function(row) row.Length = sampleCount))
            Call KobasSelfTest.Check("PLAGE 分数有限", scores.All(Function(row) row.All(Function(x) Not Double.IsNaN(x) AndAlso Not Double.IsInfinity(x))))
        End Sub
    End Module
End Namespace
