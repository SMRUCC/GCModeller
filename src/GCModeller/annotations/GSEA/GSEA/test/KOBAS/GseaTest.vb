Imports System.Globalization
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.HTS.GSEA.KOBAS
Imports std = System.Math

Namespace Global.KOBAS.test

    ''' <summary>
    ''' GSEA 排序、running sum、置换检验与显著性评估的自检。
    ''' </summary>
    Public Module GseaTest

        ''' <summary>
        ''' 执行 GSEA 的全部断言。
        ''' </summary>
        Public Sub Run()
            Call Console.WriteLine("[gsea] 基因排序 / running sum / 置换 / NES / FDR")

            Call HandComputedCase()
            Call PlantedSignalCase()
        End Sub

        ''' <summary>
        ''' 与手工推导值直接比对的简单例子。
        ''' </summary>
        Private Sub HandComputedCase()
            Dim scores As Double() = {3, 2, 1, -1, -2, -3}
            Dim ranks As RankedGene() = scores _
                .Select(Function(score, i)
                            Return New RankedGene With {
                                .GeneID = $"g{i + 1}",
                                .Score = score,
                                .Index = i
                            }
                        End Function) _
                .ToArray
            Dim mask As Boolean() = {True, False, True, False, False, False}

            Dim unweighted As EnrichmentScoreResult = EnrichmentScore.Compute(ranks, mask, 0)
            Dim weighted As EnrichmentScoreResult = EnrichmentScore.Compute(ranks, mask, 1)

            Call KobasSelfTest.CheckClose("权重 0 时的 ES", 0.75, unweighted.ES, 1.0E-12)
            Call KobasSelfTest.Check("权重 0 时的 ES 位置", unweighted.ESIndex = 2, $"index={unweighted.ESIndex}")
            Call KobasSelfTest.CheckClose("running sum 末尾归零", 0.0, unweighted.RES(unweighted.RES.Length - 1), 1.0E-12)
            Call KobasSelfTest.CheckClose("权重 1 时的 ES", 0.75, weighted.ES, 1.0E-12)
            Call KobasSelfTest.Check("权重 1 时的 ES 位置", weighted.ESIndex = 0, $"index={weighted.ESIndex}")
            Call KobasSelfTest.CheckClose("权重 1 时 running sum 末尾归零", 0.0, weighted.RES(5), 1.0E-12)

            Dim core As String() = EnrichmentScore.CoreGenes(ranks, mask, unweighted.ES, unweighted.ESIndex)

            Call KobasSelfTest.Check("核心富集基因包含全部命中基因", core.Length = 2 AndAlso core.Contains("g1") AndAlso core.Contains("g3"), core.JoinBy(","))

            ' 全部命中时 running sum 单调上升到 1
            Dim allHits As EnrichmentScoreResult = EnrichmentScore.Compute(ranks, New Boolean() {True, True, True, True, True, True}, 1)

            Call KobasSelfTest.CheckClose("全部命中时 ES 为 1", 1.0, allHits.ES, 1.0E-12)

            ' 加权分支
            Call KobasSelfTest.CheckClose("权重 2 使用平方加权", 9.0, EnrichmentScore.WeightOf(-3, 2), 1.0E-12)
            Call KobasSelfTest.CheckClose("权重 1.5 使用绝对值幂", std.Pow(3, 1.5), EnrichmentScore.WeightOf(-3, 1.5), 1.0E-12)
            Call KobasSelfTest.CheckClose("权重 0 恒为 1", 1.0, EnrichmentScore.WeightOf(42, 0), 1.0E-12)
        End Sub

        ''' <summary>
        ''' 在合成表达矩阵上检查端到端的富集行为。
        ''' </summary>
        Private Sub PlantedSignalCase()
            Const geneCount As Integer = 60
            Const sampleCount As Integer = 20

            Dim genes As String() = Enumerable.Range(0, geneCount).Select(Function(i) $"G{i:000}").ToArray
            Dim samples As String() = Enumerable.Range(0, sampleCount).Select(Function(i) $"S{i:00}").ToArray
            Dim labels As Integer() = Enumerable.Range(0, sampleCount).Select(Function(i) If(i < 10, 0, 1)).ToArray
            Dim noise As Double() = KobasSelfTest.DeterministicUniform(20261026, geneCount * sampleCount)
            Dim values As Double()() = New Double(geneCount - 1)() {}

            For g As Integer = 0 To geneCount - 1
                values(g) = New Double(sampleCount - 1) {}

                For s As Integer = 0 To sampleCount - 1
                    Dim signal As Double = 0

                    ' KOBAS 的排序分数为 (mean_0 - mean_1)，因此「在 A 组高表达」的基因
                    ' 会获得正的排序分数、正的 ES（对应 KOBAS 输出中的 phnameA 侧）
                    If g < 10 Then
                        signal = If(labels(s) = 0, 2.0, -2.0)
                    ElseIf g >= 10 AndAlso g < 20 Then
                        signal = If(labels(s) = 0, -2.0, 2.0)
                    End If

                    values(g)(s) = signal + noise(g * sampleCount + s)
                Next
            Next

            Dim expr = KobasSelfTest.MakeMatrix(genes, samples, values)
            Dim pheno As phenotype = KobasSelfTest.MakePhenotype(labels)
            Dim sets As New Dictionary(Of String, String()) From {
                {"UP", genes.Take(10).ToArray},
                {"DOWN", genes.Skip(10).Take(10).ToArray},
                {"FLAT", genes.Skip(20).Take(10).ToArray}
            }
            Dim background As Background = KobasSelfTest.MakeBackground(sets)
            Dim options As New GseaOptions With {
                .Method = GeneRankingMethod.Snr,
                .Weight = 1,
                .Permutations = 50,
                .MinSize = 5,
                .MaxSize = 500,
                .Seed = 20261026,
                .KeepRunningSum = True,
                .Verbose = False
            }
            Dim result As GseaResult() = KobasGsea.Compute(expr, pheno, background, options)

            Call KobasSelfTest.Check("GSEA 结果数量与基因集数量一致", result.Length = 3, $"{result.Length} 条")

            Dim up As GseaResult = result.First(Function(item) item.ID = "UP")
            Dim down As GseaResult = result.First(Function(item) item.ID = "DOWN")

            Call KobasSelfTest.Check("植入信号通路 UP 的 ES 为正", up.ES > 0.8, $"ES={up.ES.ToString("F4", CultureInfo.InvariantCulture)}")
            Call KobasSelfTest.Check("植入信号通路 UP 的 p 值显著", up.PValue <= 0.1, $"p={up.PValue.ToString("G4", CultureInfo.InvariantCulture)}")
            Call KobasSelfTest.Check("反向通路 DOWN 的 ES 为负", down.ES < -0.8, $"ES={down.ES.ToString("F4", CultureInfo.InvariantCulture)}")
            Call KobasSelfTest.Check("DOWN 的 p 值显著", down.PValue <= 0.1, $"p={down.PValue.ToString("G4", CultureInfo.InvariantCulture)}")
            Call KobasSelfTest.Check("结果按 ES 降序排列", result(0).ES >= result(result.Length - 1).ES)
            Call KobasSelfTest.Check("NES 与 ES 同号", std.Sign(up.NES) = std.Sign(up.ES))
            Call KobasSelfTest.Check("FDR 落在 [0,1]", result.All(Function(item) item.FDR >= 0 AndAlso item.FDR <= 1))
            Call KobasSelfTest.Check("所有 ES 落在 [-1,1]", result.All(Function(item) std.Abs(item.ES) <= 1 + 1.0E-12))
            Call KobasSelfTest.Check("所有 running sum 末尾归零", result.All(Function(item) std.Abs(item.RES(item.RES.Length - 1)) <= 1.0E-12))

            Dim split = GseaResultExtensions.SplitPhenotypes(result)

            Call KobasSelfTest.Check("按表型方向拆分结果",
                split.Positive.Length + split.Negative.Length = result.Length,
                $"{split.Positive.Length}/{split.Negative.Length}")
            Call KobasSelfTest.Check("正向表包含 UP 通路", split.Positive.Any(Function(item) item.ID = "UP"))
            Call KobasSelfTest.Check("负向表包含 DOWN 通路", split.Negative.Any(Function(item) item.ID = "DOWN"))

            ' 确定性复跑
            Dim again As GseaResult() = KobasGsea.Compute(expr, pheno, background, options)
            Dim identical As Boolean = result.Length = again.Length

            If identical Then
                For i As Integer = 0 To result.Length - 1
                    If result(i).ID <> again(i).ID OrElse
                        std.Abs(result(i).ES - again(i).ES) > 0 OrElse
                        std.Abs(result(i).PValue - again(i).PValue) > 0 Then
                        identical = False
                    End If
                Next
            End If

            Call KobasSelfTest.Check("相同种子下结果逐位一致", identical)

            ' 稳定排序：并列的排序统计量保持原始基因顺序
            Dim tiedValues As Double()() = New Double(2)() {}
            tiedValues(0) = {1, 1, 1, 1}
            tiedValues(1) = {2, 2, 2, 2}
            tiedValues(2) = {2, 2, 2, 2}
            Dim tiedExpr = KobasSelfTest.MakeMatrix({"A", "B", "C"}, {"s1", "s2", "s3", "s4"}, tiedValues)
            Dim tiedPheno As phenotype = KobasSelfTest.MakePhenotype(New Integer() {0, 0, 1, 1})
            Dim tiedRank As RankedGene() = GeneRanking.GeneRank(tiedExpr, tiedPheno)

            Call KobasSelfTest.Check("并列分数时排序稳定", tiedRank(0).Index < tiedRank(1).Index, $"{tiedRank(0).GeneID},{tiedRank(1).GeneID}")

            ' 全部基因集都被尺寸过滤时抛出异常
            Dim thrown As Boolean = False

            Try
                Dim strict As New GseaOptions With {
                    .MinSize = 50,
                    .MaxSize = 500,
                    .Permutations = 0,
                    .Verbose = False
                }

                Call KobasGsea.Compute(expr, pheno, background, strict)
            Catch ex As InvalidOperationException
                thrown = True
            End Try

            Call KobasSelfTest.Check("基因集被全部过滤时抛出异常", thrown)

            ' 关闭置换时 p/FDR 为 NaN、NES 等于 ES
            Dim noPerm As New GseaOptions With {
                .MinSize = 5,
                .MaxSize = 500,
                .Permutations = 0,
                .Verbose = False
            }
            Dim plain As GseaResult() = KobasGsea.Compute(expr, pheno, background, noPerm)

            Call KobasSelfTest.Check("跳过置换时 p 值为 NaN", plain.All(Function(item) Double.IsNaN(item.PValue)))
            Call KobasSelfTest.Check("跳过置换时 NES 等于 ES", plain.All(Function(item) item.NES = item.ES))
        End Sub
    End Module
End Namespace
