Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.HTS.GSEA.KOBAS
Imports std = System.Math

Namespace Global.KOBAS.test

    ''' <summary>
    ''' 词条相似度与聚类（KOBAS <c>sim.py</c>）的自检。
    ''' </summary>
    Public Module SimilarityTest

        ''' <summary>
        ''' 执行相似度与聚类的全部断言。
        ''' </summary>
        Public Sub Run()
            Call Console.WriteLine("[similarity] kappa / Jaccard / 簇合并 / 簇评分")

            Dim first As String() = {"a", "b", "c"}
            Dim second As String() = {"b", "c", "d"}

            Call KobasSelfTest.CheckClose("Jaccard 相似度", 0.5, TermSimilarity.Jaccard(first, second), 1.0E-12)
            Call KobasSelfTest.CheckClose("Kappa 相似度", 0.22 / 0.42, TermSimilarity.Kappa(first, second, 10), 1.0E-12)

            Dim contingency As TermContingency = TermSimilarity.Contingency(first, second, 10)

            Call KobasSelfTest.Check("列联表交集计数", contingency.Both = 2, $"both={contingency.Both}")
            Call KobasSelfTest.Check("列联表非交集计数", contingency.Neither = 6, $"neither={contingency.Neither}")

            Dim similarity As Double()() = TermSimilarity.Matrix({first, second}, 10, SimilarityMethod.Jaccard)

            Call KobasSelfTest.CheckClose("相似度矩阵对角为 1", 1.0, similarity(0)(0), 1.0E-12)
            Call KobasSelfTest.CheckClose("相似度矩阵对称", similarity(0)(1), similarity(1)(0), 0.0)

            ' KOBAS sim.py __main__ 中的示例矩阵
            Dim matrix As Double()() = {
                New Double() {1, 1, 1, 0.35, -0.5, -0.5, -0.5, 0},
                New Double() {1, 1, 1, 0.35, -0.5, -0.5, -0.5, 0},
                New Double() {1, 1, 1, 0.35, -0.5, -0.5, -0.5, 0},
                New Double() {0.35, 0.35, 0.35, 1, 0.35, 0.35, 0.35, -0.11},
                New Double() {-0.5, -0.5, -0.5, 0.35, 1, 1, 1, 0},
                New Double() {-0.5, -0.5, -0.5, 0.35, 1, 1, 1, 0},
                New Double() {-0.5, -0.5, -0.5, 0.35, 1, 1, 1, 0},
                New Double() {0, 0, 0, -0.11, 0, 0, 0, 1}
            }

            Call KobasSelfTest.Check("簇自检：紧凑簇通过", TermClustering.TestSelf(matrix, New Integer() {0, 1, 2, 3}, 0.35))
            Call KobasSelfTest.Check("簇自检：单元素簇不通过", Not TermClustering.TestSelf(matrix, New Integer() {7}, 0.35))

            Dim merged As Integer() = TermClustering.TestTwo(New Integer() {0, 1, 2, 3}, New Integer() {0, 1, 2, 3, 4, 5, 6})

            Call KobasSelfTest.Check("簇合并判据：满足时返回并集", merged IsNot Nothing AndAlso merged.Length = 7, $"size={If(merged Is Nothing, 0, merged.Length)}")
            Call KobasSelfTest.Check("簇合并判据：不满足时返回 Nothing", TermClustering.TestTwo(New Integer() {0}, New Integer() {1, 2, 3, 4, 5}) Is Nothing)

            Dim initial As List(Of Integer()) = TermClustering.Initialize(matrix, 0.35)

            Call KobasSelfTest.Check("初始候选簇数量", initial.Count = 7, $"{initial.Count} 个")

            Dim clusters As List(Of Integer()) = TermClustering.Iterate(matrix, initial)

            Call KobasSelfTest.Check("迭代合并之后只剩一个簇", clusters.Count = 1, $"{clusters.Count} 个")
            Call KobasSelfTest.Check(
                "合并后的簇包含全部 7 个词条",
                clusters.Count = 1 AndAlso clusters(0).Length = 7,
                If(clusters.Count = 0, "n/a", clusters(0).JoinBy(",")))

            Dim pvalues As Double() = {0.01, 0.001}

            Call KobasSelfTest.CheckClose("簇评分（-log10 p 的几何平均）", std.Sqrt(6.0), TermClustering.Score(New Integer() {0, 1}, pvalues), 1.0E-12)
            Call KobasSelfTest.Check("空簇评分为 0", TermClustering.Score(New Integer() {}, pvalues) = 0)

            ' 结果表层面的聚类入口（簇自检要求成员数 >= 4 才能形成有效簇）
            Dim rows As New List(Of EnrichmentRow) From {
                New EnrichmentRow With {.Term = "T1", .ID = "T1", .Input = {"a", "b", "c"}, .PValue = 0.01},
                New EnrichmentRow With {.Term = "T2", .ID = "T2", .Input = {"a", "b", "c"}, .PValue = 0.001},
                New EnrichmentRow With {.Term = "T3", .ID = "T3", .Input = {"a", "b", "c"}, .PValue = 0.02},
                New EnrichmentRow With {.Term = "T4", .ID = "T4", .Input = {"a", "b", "c"}, .PValue = 0.03},
                New EnrichmentRow With {.Term = "T5", .ID = "T5", .Input = {"x", "y", "z"}, .PValue = 0.5}
            }
            Dim termClusters As List(Of TermCluster) = rows.Clusterize(100, 0.35, SimilarityMethod.Jaccard)

            Call KobasSelfTest.Check("结果表聚类可执行", termClusters.Count >= 1, $"{termClusters.Count} 个簇")
            Call KobasSelfTest.Check("结果表聚类评分降序", termClusters.All(Function(c) c.Score >= 0))
        End Sub
    End Module
End Namespace
