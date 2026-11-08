' ============================================================================
' PhenotypeBackgroundTest.vb
'
' 表型 GSEA 知识背景构建流程的 console 自测。
' 测试数据全部内联构造，不依赖任何外部数据文件；
' 测试结果通过 DebugAudit 记录并导出为 JSON 测试报告。
' ============================================================================

Imports System.Linq
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.HTS.GSEA.KnowledgeBase
Imports SMRUCC.genomics.Analysis.metaTraits.Traitar
Imports SMRUCC.genomics.Analysis.metaTraits.Traitar.Modules
Imports SMRUCC.genomics.ProteinModel

Friend Module PhenotypeBackgroundTest

    ''' <summary>构造一条关键 Pfam 结构域特征</summary>
    Private Function Feature(pfamId As String, r As Double, importance As Double) As KeyFeature
        Return New KeyFeature With {
            .PfamId = pfamId,
            .Description = $"domain {pfamId}",
            .PearsonCorrelation = r,
            .Importance = importance
        }
    End Function

    ''' <summary>构造一条表型预测报告</summary>
    Private Function Report(id As String,
                            accession As String,
                            category As String,
                            features As KeyFeature(),
                            Optional status As String = "trained",
                            Optional result As PredictionResults = PredictionResults.NA) As ReportJSON

        Return New ReportJSON With {
            .phenotypeId = id,
            .accession = accession,
            .category = category,
            .data_type = "boolean",
            .unit = "",
            .predict = If(result = PredictionResults.[TRUE], "true", "false"),
            .result = result,
            .confidence = 0.9,
            .score = 1.25,
            .cvScore = 0.85,
            .sampleCount = 100,
            .status = status,
            .KeyFeatures = features
        }
    End Function

    ''' <summary>
    ''' 7 条模拟表型：5 条有效、1 条 skipped、1 条无关键特征；
    ''' 其中含重名 phenotypeId 与空 phenotypeId 两种边界情况。
    ''' </summary>
    Private Function MakeReports() As ReportJSON()
        Return New ReportJSON() {
            Report("motility", "motility", "Cellular processes/Motility",
                   New KeyFeature() {
                       Feature("PF00001", 0.9, 0.8),
                       Feature("PF00002", 0.7, 0.5),
                       Feature("PF00003", 0.5, 0.3)
                   }, result:=PredictionResults.[TRUE]),
            Report("spore_formation", "spore_formation", "Cellular processes/Sporulation",
                   New KeyFeature() {
                       Feature("PF00001", 0.6, 0.2),
                       Feature("PF00004", 0.8, 0.7)
                   }, result:=PredictionResults.[FALSE]),
            Report("lactose_utilization", "lactose_utilization", "Metabolism/Carbohydrate",
                   New KeyFeature() {
                       Feature("PF00005", 0.4, 0.1),
                       Feature("PF00002", 0.3, 0.05)
                   }),
            Report("skipped_trait", "skipped_trait", "Unknown/Skipped",
                   New KeyFeature() {Feature("PF00008", 0.9, 0.9)}, status:="skipped"),
            Report("empty_trait", "empty_trait", "Unknown/Empty", Nothing),
            Report("motility", "motility_duplicated", "Cellular processes/Motility",
                   New KeyFeature() {Feature("PF00006", 0.6, 0.4)}),
            Report("", "fallback_trait", "Metabolism/Other",
                   New KeyFeature() {Feature("PF00007", 0.2, 0.1)})
        }
    End Function

    ''' <summary>
    ''' 6 条模拟蛋白：含版本号、ChouFasman 脏数据、Domains 回退、无结构域、空基因编号。
    ''' 有效基因共 5 个，涉及 5 个唯一 Pfam 结构域。
    ''' </summary>
    Private Function MakeProteome() As PfamString()
        Return New PfamString() {
            New PfamString With {
                .ProteinId = "gene_001",
                .Description = "protein A",
                .PfamString = New String() {"PF00001:Foo(1|50)", "PF00002:Bar(60|100)"}
            },
            New PfamString With {
                .ProteinId = "gene_002",
                .Description = "protein B",
                .PfamString = New String() {"PF00001.2:Foo(1|50)", "[helix]"}
            },
            New PfamString With {
                .ProteinId = "gene_003",
                .Description = "protein C",
                .PfamString = New String() {"PF00004:Baz(1|40)", "PF00005:Qux(50|90)"}
            },
            New PfamString With {
                .ProteinId = "gene_004",
                .Description = "protein D",
                .Domains = New String() {"PF00006:Quux(1|30)"}
            },
            New PfamString With {
                .ProteinId = "gene_005",
                .Description = "protein E",
                .PfamString = New String() {}
            },
            New PfamString With {
                .ProteinId = "",
                .Description = "invalid protein"
            }
        }
    End Function

    ''' <summary>运行全部测试用例</summary>
    Public Sub RunAll()
        Dim reports As ReportJSON() = MakeReports()
        Dim proteome As PfamString() = MakeProteome()
        Dim options As New PhenotypeBackgroundOptions With {
            .Method = PfamWeightingMethod.TFIDF,
            .TopN = 20,
            .MinScore = 0.0,
            .GenomeName = "Test genome",
            .BackgroundId = "test_taxid"
        }

        Call DebugAudit.Mark("test-data", $"{reports.Length} phenotype report(s), {proteome.Length} protein(s)")

        Call RunCase("case:input-adapter", Sub() TestInputAdapter(reports, options))
        Call RunCase("case:domain-background", Sub() TestDomainBackground(reports, options))
        Call RunCase("case:gene-background", Sub() TestGeneBackground(reports, proteome, options))
        Call RunCase("case:bm25-weighting", Sub() TestBM25(reports, options))
        Call RunCase("case:filtering", Sub() TestFiltering(reports, options))
        Call RunCase("case:single-report", Sub() TestSingleReport(reports, proteome, options))
        Call RunCase("case:gene-index", Sub() TestGeneIndex(proteome))
        Call RunCase("case:xml-serialization", Sub() TestSerialization(reports, proteome, options))
        Call RunCase("case:fisher-enrichment-demo", Sub() FisherEnrichmentDemo.Run())

        Call DebugAudit.Mark("summary", $"failureCount={DebugAudit.FailureCount}")
    End Sub

    ''' <summary>
    ''' 运行单个测试用例并记录 PASS/FAIL 结论。
    ''' DebugAudit 只在断言失败时写入事件，成功路径不产生记录，
    ''' 因此这里用 FailureCount 的增量把每个用例的结论显式写入测试报告。
    ''' </summary>
    Private Sub RunCase(name As String, test As System.Action)
        Dim before As Integer = DebugAudit.FailureCount

        Try
            Call test()
        Catch ex As Exception
            Call DebugAudit.Test(False, $"{name} threw {ex.GetType.Name}: {ex.Message}")
        End Try

        Dim delta As Integer = DebugAudit.FailureCount - before

        Call DebugAudit.Mark(name, If(delta = 0, "PASS", $"FAIL ({delta} assertion(s))"))
    End Sub

    ''' <summary>输入适配：有效表型过滤、ID 唯一化、TF-IDF 词条打分</summary>
    Private Sub TestInputAdapter(reports As ReportJSON(), options As PhenotypeBackgroundOptions)
        Dim docs As PhenotypeTermDocument() = reports.AsTermDocuments()

        Call DebugAudit.AreEqual(5, docs.Length, "跳过 skipped 与无 KeyFeatures 的表型之后剩余 5 条")
        Call DebugAudit.Test(docs.Select(Function(d) d.Id).Distinct().Count() = docs.Length,
                             "表型 Id 全局唯一")
        Call DebugAudit.Test(docs.Any(Function(d) d.Id = "motility#2"),
                             "重名 phenotypeId 追加序号后缀")
        Call DebugAudit.Test(docs.Any(Function(d) d.Id = "fallback_trait"),
                             "空 phenotypeId 使用 accession 兜底")
        Call DebugAudit.Test(Not docs.Any(Function(d) d.Id = "skipped_trait"),
                             "skipped 表型未被纳入")
        Call DebugAudit.Test(Not docs.Any(Function(d) d.Id = "empty_trait"),
                             "无关键特征的表型未被纳入")

        Dim scores As Dictionary(Of String, PfamTermScore()) = docs.ScoreTerms(options)

        Call DebugAudit.AreEqual(5, scores.Count, "每个有效表型都产出一组词条得分")
        Call DebugAudit.AreEqual(3, scores("motility").Length, "motility 的 3 个 Pfam 词条全部保留")
        Call DebugAudit.Test(scores("motility").All(Function(s) s.Weight > 0.0),
                             "TF-IDF 权重全部为正")
        Call DebugAudit.AreEqual("PF00001", scores("motility").First().PfamId,
                                 "重要性最高的 Pfam 词条排在最前")
        Call DebugAudit.Test(scores("motility").First().Importance > scores("motility").Last().Importance,
                             "词条得分保留模型重要性排序语义")

        Call DebugAudit.Mark("input-adapter", $"{docs.Length} doc(s), motility top={scores("motility").First()}")
    End Sub

    ''' <summary>结构域级背景：成员为 Pfam 编号，universe 为唯一 Pfam 数</summary>
    Private Sub TestDomainBackground(reports As ReportJSON(), options As PhenotypeBackgroundOptions)
        Dim background As Background = reports.CreateBackground(options:=options)

        Call DebugAudit.AreEqual(5, background.clusters.Length, "结构域级背景含 5 个聚类")
        Call DebugAudit.AreEqual(7, background.size, "结构域级 universe 等于全部唯一 Pfam 数")
        Call DebugAudit.Test(background.clusters.All(Function(c) c.members IsNot Nothing),
                             "所有 Cluster.members 均非 Nothing")
        Call DebugAudit.AreEqual(5, background.GetClusterTable().Count,
                                 "Cluster.ID 唯一且可被 GetClusterTable 索引")
        Call DebugAudit.AreEqual("Test genome", background.name, "背景名称来自配置")
        Call DebugAudit.AreEqual("test_taxid", background.id, "背景标识来自配置")
        Call DebugAudit.Test(background.comments.Contains("method=TFIDF"), "comments 记录权重算法")
        Call DebugAudit.Test(background.comments.Contains("members=pfam-domain"), "comments 记录成员粒度")
        Call DebugAudit.Test(background.clusters.All(Function(c) Not String.IsNullOrEmpty(c.description)),
                             "每个聚类都带有表型描述信息")

        Dim motility As Cluster = background("motility")

        Call DebugAudit.Test(motility IsNot Nothing, "可以通过表型 Id 取回对应聚类")
        Call DebugAudit.AreEqual("Cellular processes", motility.category, "主分类来自 category 第一段")
        Call DebugAudit.AreEqual("Motility", motility.[class], "次分类来自 category 第二段")
        Call DebugAudit.AreEqual(3, motility.members.Length, "结构域级成员数等于入选词条数")

        Call DebugAudit.Mark("domain-background", background.ToString())
    End Sub

    ''' <summary>基因级背景：成员为 ProteinId，universe 为全基因组唯一基因数</summary>
    Private Sub TestGeneBackground(reports As ReportJSON(),
                                  proteome As PfamString(),
                                  options As PhenotypeBackgroundOptions)

        Dim background As Background

        Using scope = DebugAudit.Watch("gene-level background", logOnExit:=False)
            background = reports.CreateBackground(proteome, options)
        End Using

        Call DebugAudit.AreEqual(5, background.clusters.Length, "基因级背景含 5 个聚类")
        Call DebugAudit.AreEqual(5, background.size, "基因级 universe 等于全基因组唯一基因数")
        Call DebugAudit.Test(background.clusters.All(Function(c) c.members IsNot Nothing),
                             "基因级所有 Cluster.members 均非 Nothing")
        Call DebugAudit.Test(background.comments.Contains("members=gene"), "comments 记录基因级粒度")

        Dim motility As Cluster = background("motility")

        Call DebugAudit.AreEqual(2, motility.members.Length,
                                 "PF00001/PF00002 命中 gene_001 与 gene_002")
        Call DebugAudit.Test(motility.memberIds.Contains("gene_001") AndAlso
                             motility.memberIds.Contains("gene_002"),
                             "带版本号与 ChouFasman 脏数据的 Pfam 可被正确解析")
        Call DebugAudit.Test(Not motility.memberIds.Contains(""), "空基因编号未被纳入")

        Dim gene1 As BackgroundGene = motility.GetMemberById("gene_001")

        Call DebugAudit.Test(gene1 IsNot Nothing, "可以通过基因编号取回背景成员")
        Call DebugAudit.AreEqual(2, gene1.term_id.Length,
                                 "同一基因命中多个 Pfam 时聚合到 term_id")

        Call DebugAudit.Test(background("motility#2").memberIds.Contains("gene_004"),
                             "PfamString 缺失时回退解析 Domains 属性")
        Call DebugAudit.AreEqual(0, background("fallback_trait").members.Length,
                                 "无基因命中的表型产出空数组成员")
        Call DebugAudit.Test(background("fallback_trait").members IsNot Nothing,
                             "无基因命中的表型成员非 Nothing")

        Call DebugAudit.Mark("gene-background", $"size={background.size}, clusters={background.clusters.Length}")
    End Sub

    ''' <summary>BM25 权重策略：可切换且产出有效词条得分</summary>
    Private Sub TestBM25(reports As ReportJSON(), options As PhenotypeBackgroundOptions)
        Dim bmOptions As PhenotypeBackgroundOptions = options.Copy()

        bmOptions.Method = PfamWeightingMethod.BM25
        bmOptions.K1 = 1.2
        bmOptions.B = 0.75

        Dim docs As PhenotypeTermDocument() = reports.AsTermDocuments()
        Dim tfidfScores As Dictionary(Of String, PfamTermScore()) = docs.ScoreTerms(options)
        Dim bmScores As Dictionary(Of String, PfamTermScore()) = docs.ScoreTerms(bmOptions)

        Call DebugAudit.AreEqual(5, bmScores.Count, "BM25 为每个有效表型产出词条得分")
        Call DebugAudit.AreEqual(3, bmScores("motility").Length, "BM25 保留 motility 的全部词条")
        Call DebugAudit.Test(bmScores("motility").All(Function(s) s.Weight > 0.0),
                             "BM25 词项贡献全部为正")
        Call DebugAudit.Test(System.Math.Abs(bmScores("motility").First().Weight -
                                             tfidfScores("motility").First().Weight) > 0.000001,
                             "BM25 与 TF-IDF 产出不同的权重标度")

        Dim background As Background = reports.CreateBackground(options:=bmOptions)

        Call DebugAudit.AreEqual(5, background.clusters.Length, "BM25 背景聚类数正确")
        Call DebugAudit.Test(background.comments.Contains("method=BM25"), "comments 记录 BM25 算法")

        Call DebugAudit.Mark("bm25", $"top={bmScores("motility").First()}")
    End Sub

    ''' <summary>TopN 与 MinScore 筛选语义，且筛选不改变 universe</summary>
    Private Sub TestFiltering(reports As ReportJSON(), options As PhenotypeBackgroundOptions)
        Dim topOptions As PhenotypeBackgroundOptions = options.Copy()

        topOptions.TopN = 1

        Dim topBackground As Background = reports.CreateBackground(options:=topOptions)

        Call DebugAudit.AreEqual(5, topBackground.clusters.Length, "TopN 不改变聚类数量")
        Call DebugAudit.Test(topBackground.clusters.All(Function(c) c.members.Length <= 1),
                             "TopN=1 限制每个聚类最多 1 个词条")

        Dim strictOptions As PhenotypeBackgroundOptions = options.Copy()

        strictOptions.MinScore = 1000000.0

        Dim strictBackground As Background = reports.CreateBackground(options:=strictOptions)

        Call DebugAudit.AreEqual(5, strictBackground.clusters.Length, "MinScore 不改变聚类数量")
        Call DebugAudit.Test(strictBackground.clusters.All(
                                 Function(c) c.members IsNot Nothing AndAlso c.members.Length = 0),
                             "词条被全部过滤时成员为空数组")
        Call DebugAudit.AreEqual(7, strictBackground.size,
                                 "universe 不因为词条筛选而被低估")

        Dim unlimited As PhenotypeBackgroundOptions = options.Copy()

        unlimited.TopN = 0

        Call DebugAudit.AreEqual(3, reports.AsTermDocuments().ScoreTerms(unlimited)("motility").Length,
                                 "TopN<=0 表示不限制词条数量")

        Call DebugAudit.Mark("filtering", "topN / minScore / unlimited verified")
    End Sub

    ''' <summary>单条 ReportJSON 直接转换为一个 Cluster</summary>
    Private Sub TestSingleReport(reports As ReportJSON(),
                                 proteome As PfamString(),
                                 options As PhenotypeBackgroundOptions)

        Dim domainCluster As Cluster = reports(0).AsCluster(options)

        Call DebugAudit.Test(domainCluster IsNot Nothing, "单表型 AsCluster 返回非空聚类")
        Call DebugAudit.AreEqual("motility", domainCluster.ID, "单表型聚类 ID 正确")
        Call DebugAudit.AreEqual(3, domainCluster.members.Length, "单表型结构域级成员数正确")

        Dim geneCluster As Cluster = reports(0).AsCluster(options, proteome.BuildGeneIndex())

        Call DebugAudit.AreEqual(2, geneCluster.members.Length, "单表型基因级成员数正确")

        Dim skipped As Cluster = reports(3).AsCluster(options)

        Call DebugAudit.Test(skipped IsNot Nothing AndAlso skipped.members IsNot Nothing,
                             "无效表型也能得到 members 非 Nothing 的空聚类")
        Call DebugAudit.AreEqual(0, skipped.members.Length, "无效表型的聚类成员为空数组")

        Call DebugAudit.Mark("single-report", $"{domainCluster.ID}: {domainCluster.members.Length} term(s)")
    End Sub

    ''' <summary>Pfam -> 基因倒排索引</summary>
    Private Sub TestGeneIndex(proteome As PfamString())
        Dim index As PfamGeneIndex = proteome.BuildGeneIndex()

        Call DebugAudit.AreEqual(5, index.GeneCount, "倒排索引包含 5 个有效基因")
        Call DebugAudit.AreEqual(5, index.DomainCount, "倒排索引包含 5 个唯一 Pfam 结构域")
        Call DebugAudit.AreEqual(5, index.GeneIds.Length, "GeneIds 返回全部唯一基因编号")
        Call DebugAudit.Test(index.ContainsDomain("PF00001"), "PF00001 存在于索引之中")
        Call DebugAudit.Test(Not index.ContainsDomain("PF00099"), "未出现的 Pfam 不在索引之中")

        Dim members As BackgroundGene() = index.MembersFor(
            New PfamTermScore() {
                New PfamTermScore With {.PfamId = "PF00001"},
                New PfamTermScore With {.PfamId = "PF00002"}
            })

        Call DebugAudit.AreEqual(2, members.Length, "两个 Pfam 共命中 2 个基因")
        Call DebugAudit.Test(members.All(Function(g) g.term_id IsNot Nothing),
                             "基因成员的 term_id 均已填充")

        Dim empty As BackgroundGene() = index.MembersFor(Nothing)

        Call DebugAudit.Test(empty IsNot Nothing AndAlso empty.Length = 0,
                             "词条为 Nothing 时返回空数组")

        Dim nullIndex As PfamGeneIndex = CType(Nothing, IEnumerable(Of PfamString)).BuildGeneIndex()

        Call DebugAudit.AreEqual(0, nullIndex.GeneCount, "空蛋白质组产生空索引")

        Call DebugAudit.Mark("gene-index", index.ToString())
    End Sub

    ''' <summary>Background 可按既有惯例序列化为 XML</summary>
    Private Sub TestSerialization(reports As ReportJSON(),
                                  proteome As PfamString(),
                                  options As PhenotypeBackgroundOptions)

        Dim background As Background = reports.CreateBackground(proteome, options)
        Dim xml As String = Nothing
        Dim failure As String = Nothing

        Try
            xml = background.GetXml()
        Catch ex As Exception
            failure = ex.Message
        End Try

        Call DebugAudit.Test(xml IsNot Nothing,
                             $"Background 可以序列化为 XML（failure={If(failure, "none")}）")
        Call DebugAudit.Test(xml IsNot Nothing AndAlso xml.Contains("background"),
                             "序列化结果包含 background 根元素")
        Call DebugAudit.Test(xml IsNot Nothing AndAlso xml.Contains("motility"),
                             "序列化结果包含表型聚类标识")

        Call DebugAudit.Mark("xml-serialization", $"{If(xml Is Nothing, 0, xml.Length)} char(s)")
    End Sub
End Module
