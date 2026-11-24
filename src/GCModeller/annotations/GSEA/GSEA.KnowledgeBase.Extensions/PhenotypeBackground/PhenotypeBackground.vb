' ============================================================================
' PhenotypeBackground.vb
'
' 表型 GSEA 知识背景构建的「对外入口 / 输出层」。
'
' 输入：一个微生物基因组注释出来的全部 ReportJSON 表型预测结果
'       （可选：该基因组的 PfamString 蛋白质组结构域注释）
' 输出：可直接用于 Fisher 精确检验富集计算的 Background 对象
'
' 一条表型 = 一个 Cluster，聚类成员由表型的关键 Pfam 结构域决定：
'   CreateBackground(reports)                 -> Pfam 结构域级成员
'   CreateBackground(reports, proteins)      -> 基因级成员（可直接对接 DEG 列表）
'
' 本模块只返回 Background 对象，不写文件；落盘由调用方按既有惯例执行
' background.GetXml.SaveTo(out)。
' ============================================================================

Imports System.Linq
Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Analysis.metaTraits.Traitar
Imports SMRUCC.genomics.ProteinModel

''' <summary>
''' 基于 Traitar 表型注释结果构建 GSEA 富集计算所需的知识背景
''' </summary>
Public Module PhenotypeBackground

    ''' <summary>
    ''' 把单条表型注释转换为一个 GSEA 功能聚类（完整的「打分 + 筛选 + 组装」流程）。
    ''' </summary>
    ''' <param name="report">单条表型预测报告</param>
    ''' <param name="options">权重算法与筛选参数，省略时使用默认值</param>
    ''' <param name="geneIndex">
    ''' 目标基因组倒排索引；传入时生成基因级成员，省略时生成 Pfam 结构域级成员。
    ''' </param>
    ''' <returns>一个 <see cref="Cluster"/>；无效表型会得到成员为空数组的空聚类</returns>
    <Extension>
    Public Function AsCluster(report As ReportJSON,
                              Optional options As PhenotypeBackgroundOptions = Nothing,
                              Optional geneIndex As PfamGeneIndex = Nothing) As Cluster

        If report Is Nothing Then
            Throw New ArgumentNullException(NameOf(report))
        End If

        options = If(options, New PhenotypeBackgroundOptions())

        Dim docs As PhenotypeTermDocument() = New ReportJSON() {report}.AsTermDocuments()

        If docs.Length = 0 Then
            Return report.AsCluster(
                id:=If(String.IsNullOrWhiteSpace(report.phenotypeId), "phenotype", report.phenotypeId),
                terms:=New PfamTermScore() {},
                geneIndex:=geneIndex)
        End If

        Dim scores As Dictionary(Of String, PfamTermScore()) = docs.ScoreTerms(options)
        Dim doc As PhenotypeTermDocument = docs(0)

        Return doc.Report.AsCluster(doc.Id, scores(doc.Id), geneIndex)
    End Function

    ''' <summary>
    ''' 构建「Pfam 结构域级」知识背景：每个聚类的成员是筛选后的 Pfam 编号，
    ''' 富集计算的输入基因列表也应当是 Pfam 结构域编号列表。
    ''' </summary>
    ''' <param name="reports">该基因组注释出来的全部表型预测报告</param>
    ''' <param name="options">权重算法与筛选参数，省略时使用默认值</param>
    <Extension>
    Public Function CreateBackground(reports As IEnumerable(Of ReportJSON),
                                     Optional options As PhenotypeBackgroundOptions = Nothing) As Background

        Return BuildBackground(reports, Nothing, options)
    End Function

    ''' <summary>
    ''' 构建「基因级」知识背景（推荐的默认用法）：每个聚类的成员是携带该表型
    ''' 关键 Pfam 结构域的基因（ProteinId），可直接对接转录组／蛋白质组 DEG 列表。
    ''' </summary>
    ''' <param name="reports">该基因组注释出来的全部表型预测报告</param>
    ''' <param name="proteins">该基因组所预测出来的蛋白质组 Pfam 注释</param>
    ''' <param name="options">权重算法与筛选参数，省略时使用默认值</param>
    <Extension>
    Public Function CreateBackground(reports As IEnumerable(Of ReportJSON),
                                     proteins As IEnumerable(Of PfamString),
                                     Optional options As PhenotypeBackgroundOptions = Nothing) As Background

        Return BuildBackground(reports, proteins.BuildGeneIndex(), options)
    End Function

    ''' <summary>
    ''' 共用的背景组装流程：有效表型过滤 -&gt; 词条打分筛选 -&gt; 逐表型生成 Cluster -&gt; 汇总 Background。
    ''' </summary>
    Private Function BuildBackground(reports As IEnumerable(Of ReportJSON),
                                     geneIndex As PfamGeneIndex,
                                     options As PhenotypeBackgroundOptions) As Background

        options = If(options, New PhenotypeBackgroundOptions())

        ' 输入适配：跳过 skipped / 无关键特征的表型，保证表型 Id 唯一
        Dim docs As PhenotypeTermDocument() = reports.AsTermDocuments()
        ' 权重层：TF-IDF（默认）或 BM25，统一输出 PfamId -> 权重
        Dim scores As Dictionary(Of String, PfamTermScore()) = docs.ScoreTerms(options)
        Dim clusters As New List(Of Cluster)()

        For Each doc As PhenotypeTermDocument In docs
            clusters.Add(doc.Report.AsCluster(doc.Id, scores(doc.Id), geneIndex))
        Next

        ' universe(N) 必须反映目标基因组的真实规模，不能因为词条筛选而被低估：
        '  基因级      -> 全基因组唯一基因数
        '  结构域级    -> 全部有效表型涉及的唯一 Pfam 编号数
        Dim size As Integer
        Dim granularity As String

        If geneIndex Is Nothing Then
            size = docs _
                .SelectMany(Function(d) d.Terms.Keys) _
                .Distinct(StringComparer.Ordinal) _
                .Count()
            granularity = "pfam-domain"
        Else
            size = geneIndex.GeneCount
            granularity = "gene"
        End If

        Return New Background With {
            .build = Now,
            .clusters = clusters.ToArray(),
            .comments = $"GSEA background model build from Traitar phenotype reports by GCModeller; " &
                        $"method={options.Method}, members={granularity}, " &
                        $"topN={options.TopN}, minScore={options.MinScore}, " &
                        $"phenotypes={clusters.Count}, universe={size}.",
            .name = options.GenomeName,
            .id = options.BackgroundId,
            .size = size
        }
    End Function
End Module
