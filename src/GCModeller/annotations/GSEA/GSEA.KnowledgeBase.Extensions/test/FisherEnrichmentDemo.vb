' ============================================================================
' FisherEnrichmentDemo.vb
'
' 端到端 demo：表型注释 JSON + 蛋白质组 Pfam 注释 -> GSEA 知识背景(Background.Xml)
'              -> 差异基因列表 -> Fisher 精确检验富集计算 -> 富集结果 CSV。
'
' demo 测试数据全部内联生成并物化到 <输出目录>/demo/ 之下，可以直接拿去复用：
'   phenotypes.json   表型预测报告（ReportJSON 数组）
'   proteome.json     基因组蛋白质组 Pfam 注释（PfamString 数组）
'   de_genes.txt      模拟差异表达基因列表（每行一个基因编号）
'   background.Xml    Fisher 富集所需的知识背景
'   enrichment.csv    Fisher 富集结果（已做 BH/FDR 校正）
'
' 数据设计：200 个基因、6 个有效表型聚类；差异基因列表刻意让 motility 表型
'           强富集，同时保留一个成员数很少的 stress_response 用于演示 cutSize 参数。
' ============================================================================

Imports System
Imports System.IO
Imports System.Linq
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.HTS.GSEA.KnowledgeBase
Imports SMRUCC.genomics.Analysis.metaTraits.Traitar
Imports SMRUCC.genomics.Analysis.metaTraits.Traitar.Modules
Imports SMRUCC.genomics.ProteinModel

Friend Module FisherEnrichmentDemo

    ''' <summary>模拟基因组的基因总数（超几何检验的 universe，N）</summary>
    Private Const GenomeSize As Integer = 200

    Private Function GeneId(i As Integer) As String
        Return $"gene_{i:0000}"
    End Function

    Private Function PfamId(i As Integer) As String
        Return $"PF{i:00000}"
    End Function

    ''' <summary>构造 Pfam-string 之中的一个条目，格式与 PfamEmbedding.ParsePfamId 兼容</summary>
    Private Function Token(pfam As Integer) As String
        Return $"{PfamId(pfam)}:domain_{pfam}(1|50)"
    End Function

    Private Function Feature(pfam As Integer, rank As Integer) As KeyFeature
        Return New KeyFeature With {
            .PfamId = PfamId(pfam),
            .Description = $"pfam domain {PfamId(pfam)}",
            .PearsonCorrelation = 0.9 - rank * 0.15,
            .Importance = 0.8 - rank * 0.12
        }
    End Function

    Private Function MakeReport(id As String, category As String, pfams As Integer(),
                                Optional status As String = "trained") As ReportJSON

        Dim features As KeyFeature() = pfams _
            .Select(Function(p, i) Feature(p, i)) _
            .ToArray()

        Return New ReportJSON With {
            .phenotypeId = id,
            .accession = id,
            .category = category,
            .data_type = "boolean",
            .predict = "true",
            .result = PredictionResults.[TRUE],
            .confidence = 0.9,
            .cvScore = 0.85,
            .sampleCount = GenomeSize,
            .status = status,
            .KeyFeatures = features
        }
    End Function

    ''' <summary>
    ''' 8 条模拟表型报告：6 条有效，另有 1 条 skipped、1 条无关键特征用于验证过滤。
    ''' 每个表型的关键 Pfam 集合会被映射到下面蛋白质组中的一批基因。
    ''' </summary>
    Private Function MakeReports() As ReportJSON()
        Return New ReportJSON() {
            MakeReport("motility", "Cellular processes/Motility", New Integer() {1, 2, 3, 4}),
            MakeReport("spore_formation", "Cellular processes/Sporulation", New Integer() {5, 6}),
            MakeReport("lactose_utilization", "Metabolism/Carbohydrate", New Integer() {7, 8}),
            MakeReport("antibiotic_resistance", "Resistance/Antibiotic", New Integer() {9, 10}),
            MakeReport("biofilm_formation", "Cellular processes/Biofilm", New Integer() {11, 12}),
            MakeReport("stress_response", "Response/Stress", New Integer() {21, 22}),
            MakeReport("skipped_trait", "Unknown/Skipped", New Integer() {23}, status:="skipped"),
            New ReportJSON With {
                .phenotypeId = "empty_trait",
                .accession = "empty_trait",
                .status = "trained",
                .KeyFeatures = Nothing
            }
        }
    End Function

    ''' <summary>按基因序号分配 Pfam 结构域，形成 6 个表型各自的基因集合</summary>
    Private Function DomainsFor(i As Integer) As String()
        Dim pfams As Integer()

        Select Case i
            Case 1 To 40
                pfams = New Integer() {1, 2, 3, 4}        ' motility -> 40 个基因
            Case 41 To 52
                pfams = New Integer() {5, 6}               ' spore_formation -> 12 个基因
            Case 53 To 62
                pfams = New Integer() {7, 8}               ' lactose_utilization -> 10 个基因
            Case 63 To 70
                pfams = New Integer() {9, 10}              ' antibiotic_resistance -> 8 个基因
            Case 71 To 76
                pfams = New Integer() {11, 12}             ' biofilm_formation -> 6 个基因
            Case 180 To 181
                pfams = New Integer() {21, 22}             ' stress_response -> 2 个基因（小聚类）
            Case Else
                pfams = New Integer() {13 + (i Mod 8)}     ' PF00013..PF00020 背景噪音
        End Select

        Return pfams.Select(AddressOf Token).ToArray()
    End Function

    Private Function MakeProteome() As PfamString()
        Dim proteome As New List(Of PfamString)()

        For i As Integer = 1 To GenomeSize
            proteome.Add(New PfamString With {
                .ProteinId = GeneId(i),
                .Description = $"protein {GeneId(i)}",
                .Length = 150,
                .PfamString = DomainsFor(i)
            })
        Next

        Return proteome.ToArray()
    End Function

    ''' <summary>
    ''' 模拟差异表达基因列表：12 个 motility 基因 + 2 个 spore 基因 +
    ''' 1 个 stress_response 基因 + 6 个背景噪音基因，共 21 个。
    ''' </summary>
    Private Function MakeDeGenes() As String()
        Dim genes As New List(Of String)()

        For i As Integer = 1 To 12
            genes.Add(GeneId(i))
        Next
        Call genes.Add(GeneId(41))
        Call genes.Add(GeneId(42))
        Call genes.Add(GeneId(180))
        For i As Integer = 80 To 85
            genes.Add(GeneId(i))
        Next

        Return genes.ToArray()
    End Function

    ''' <summary>运行 demo：生成数据文件、构建背景、执行 Fisher 富集、输出与断言</summary>
    Public Sub Run()
        Dim demoDir As String = Path.Combine(AppContext.BaseDirectory, "demo")
        Call Directory.CreateDirectory(demoDir)

        Dim phenotypeFile As String = Path.Combine(demoDir, "phenotypes.json")
        Dim proteomeFile As String = Path.Combine(demoDir, "proteome.json")
        Dim deGenesFile As String = Path.Combine(demoDir, "de_genes.txt")
        Dim backgroundFile As String = Path.Combine(demoDir, "background.Xml")
        Dim enrichmentFile As String = Path.Combine(demoDir, "enrichment.csv")

        ' ---- 1. 物化 demo 测试数据 ----
        Call File.WriteAllText(phenotypeFile, MakeReports().GetJson(indent:=True))
        Call File.WriteAllText(proteomeFile, MakeProteome().GetJson(indent:=True))
        Call File.WriteAllLines(deGenesFile, MakeDeGenes())

        ' ---- 2. 从 demo 数据文件读回，构建 GSEA 知识背景 ----
        Dim reports As ReportJSON() = phenotypeFile.LoadJSON(Of ReportJSON())()
        Dim proteome As PfamString() = proteomeFile.LoadJSON(Of PfamString())()
        Dim deGenes As String() = File.ReadAllLines(deGenesFile)
        Dim options As New PhenotypeBackgroundOptions With {
            .Method = PfamWeightingMethod.TFIDF,
            .TopN = 20,
            .MinScore = 0.0,
            .GenomeName = "Demo genome",
            .BackgroundId = "demo_taxid"
        }

        Dim background As Background = reports.CreateBackground(proteome, options)

        Call File.WriteAllText(backgroundFile, background.GetXml())

        ' 演示既有惯例：背景模型以 Xml 文件落盘/读回
        Dim loadedBackground As Background = backgroundFile.LoadXml(Of Background)()

        ' ---- 3. 调用 FisherCore 的 Fisher 精确检验富集 API ----
        ' Enrichment 是 Iterator，必须落地；返回值未做多重检验校正，需再调用 FDR()
        Dim result As EnrichmentResult() = loadedBackground _
            .Enrichment(list:=deGenes, resize:=-1, cutSize:=0,
                        outputAll:=True, isLocustag:=False, showProgress:=False) _
            .FDR() _
            .OrderBy(Function(r) r.pvalue) _
            .ThenBy(Function(r) r.term) _
            .ToArray()

        Call result.SaveTo(enrichmentFile, silent:=True)

        ' cutSize 参数演示：过滤掉成员数过小的功能聚类
        Dim filtered As EnrichmentResult() = loadedBackground _
            .Enrichment(list:=deGenes, cutSize:=3, outputAll:=True, showProgress:=False) _
            .ToArray()

        ' ---- 4. 控制台输出 demo 结果 ----
        Call PrintDemo(demoDir, loadedBackground, deGenes, result, filtered)

        ' ---- 5. DebugAudit 断言 ----
        Call DebugAudit.AreEqual(GenomeSize, proteome.Length, "demo 蛋白质组基因数")
        Call DebugAudit.AreEqual(GenomeSize, background.size, "基因级背景 universe 等于全基因组基因数")
        Call DebugAudit.AreEqual(6, background.clusters.Length, "跳过 skipped/empty 之后剩余 6 个表型聚类")
        Call DebugAudit.AreEqual(21, deGenes.Length, "差异基因列表规模")
        Call DebugAudit.Test(result.Length >= 3, "Fisher 富集产出 motility/spore/stress 三个 term")

        Dim motility As EnrichmentResult = result.FirstOrDefault(Function(r) r.term = "motility")

        Call DebugAudit.Test(motility IsNot Nothing, "motility 表型被富集检出")

        If motility IsNot Nothing Then
            Call DebugAudit.AreEqual(40, motility.cluster, "motility 聚类成员总数(b)")
            Call DebugAudit.AreEqual(12, motility.enriched, "motility 命中的差异基因数(a)")
            Call DebugAudit.AreEqual(12, motility.IDs.Length, "富集结果携带命中的基因 ID 列表")
            Call DebugAudit.Test(motility.pvalue < 0.05, "motility 原始 pvalue 显著")
            Call DebugAudit.Test(motility.FDR < 0.05, "BH 校正之后 FDR 仍然显著")
        End If

        Call DebugAudit.Test(result.First().term = "motility", "按 pvalue 升序后 motility 排在首位")
        Call DebugAudit.Test(filtered.Length < result.Length, "cutSize=3 过滤掉成员过少的 stress_response 聚类")
        Call DebugAudit.Test(Not filtered.Any(Function(r) r.term = "stress_response"),
                             "被过滤的小聚类不出现在结果之中")

        For Each filePath As String In {phenotypeFile, proteomeFile, deGenesFile, backgroundFile, enrichmentFile}
            Call DebugAudit.Test(System.IO.File.Exists(filePath) AndAlso New FileInfo(filePath).Length > 0,
                                 $"demo 数据文件已生成: {System.IO.Path.GetFileName(filePath)}")
        Next

        Call DebugAudit.Mark("demo-enrichment", $"universe={loadedBackground.size}, terms={result.Length}, top={result.First().term}")
    End Sub

    Private Sub PrintDemo(demoDir As String,
                          background As Background,
                          deGenes As String(),
                          result As EnrichmentResult(),
                          filtered As EnrichmentResult())

        Console.WriteLine()
        Console.WriteLine("=== Fisher exact test enrichment demo ===")
        Console.WriteLine($"demo data  : {demoDir}")
        Console.WriteLine($"background : {background.name}, universe={background.size}, clusters={background.clusters.Length}")
        Console.WriteLine($"DE genes   : {deGenes.Length}")
        Console.WriteLine()
        Console.WriteLine("term                   cluster   enriched   score      pvalue        FDR")

        For Each term As EnrichmentResult In result
            Console.WriteLine($"{term.term,-22}{term.cluster,7}{term.enriched,11}{term.score,10:F4}{term.pvalue,14:E3}{term.FDR,14:E3}")
        Next

        Console.WriteLine()
        Console.WriteLine($"cutSize=3 -> {filtered.Length} term(s) (members > 3)")
        Console.WriteLine()
    End Sub
End Module
