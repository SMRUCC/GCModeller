' ============================================================================
' TermWeighting.vb
'
' 表型 GSEA 知识背景构建的「输入适配层 + 权重层」。
'
' 统一中间表示：一条有效的表型注释 = 一篇文档，该表型的关键 Pfam 结构域 = 词条。
' TF-IDF 与 BM25 两套适配器各自只负责产出统一的 PfamTermScore 序列，下游的
' Cluster / Background 组装层完全共用该结果，新增第三种算法不会波及组装层。
'
' 权重语义：重要性(KeyFeature.Importance / PearsonCorrelation 的绝对值) 决定词频 tf，
'           IDF 反映该 Pfam 结构域在全部表型之中的稀有程度，二者相乘得到词条得分。
' ============================================================================

Imports System.Linq
Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Analysis.metaTraits.Traitar
Imports SMRUCC.genomics.Analysis.metaTraits.Traitar.Modules
Imports BM25Engine = Microsoft.VisualBasic.Data.NLP.BM25.BM25Engine
Imports IdfVariant = Microsoft.VisualBasic.Data.NLP.BM25.IdfVariant
Imports TFIDF = Microsoft.VisualBasic.Data.NLP.TFIDF
Imports std = System.Math

''' <summary>
''' Pfam 结构域词条的重要性打分算法
''' </summary>
Public Enum PfamWeightingMethod
    ''' <summary>词频-逆文档频率，平滑 IDF 版本（默认）</summary>
    TFIDF = 0
    ''' <summary>Okapi/Lucene BM25 词项贡献（含词频饱和与文档长度归一）</summary>
    BM25 = 1
End Enum

''' <summary>
''' 表型 GSEA 知识背景构建参数
''' </summary>
Public Class PhenotypeBackgroundOptions

    ''' <summary>词条权重算法，默认 <see cref="PfamWeightingMethod.TFIDF"/></summary>
    Public Property Method As PfamWeightingMethod = PfamWeightingMethod.TFIDF

    ''' <summary>
    ''' 每个表型最多保留的 Pfam 词条数量；<c>&lt;= 0</c> 表示不限制。
    ''' </summary>
    Public Property TopN As Integer = 20

    ''' <summary>
    ''' 词条权重下限，低于该值的词条会被剔除；默认 0 表示不按分数过滤。
    ''' 先按 <see cref="MinScore"/> 过滤，再按 <see cref="TopN"/> 截取。
    ''' </summary>
    Public Property MinScore As Double = 0.0

    ''' <summary>BM25 词频饱和参数</summary>
    Public Property K1 As Double = 1.2

    ''' <summary>BM25 文档长度归一化参数</summary>
    Public Property B As Double = 0.75

    ''' <summary>生成的背景名称（一般为基因组名 / 物种名）</summary>
    Public Property GenomeName As String = "Unknown"

    ''' <summary>生成的背景标识（一般为 ncbi taxonomy id 或 kegg organism code）</summary>
    Public Property BackgroundId As String = "phenotype_background"

    ''' <summary>返回当前配置的一份浅拷贝，避免调用方的共享配置被意外改写</summary>
    Public Function Copy() As PhenotypeBackgroundOptions
        Return New PhenotypeBackgroundOptions With {
            .Method = Method,
            .TopN = TopN,
            .MinScore = MinScore,
            .K1 = K1,
            .B = B,
            .GenomeName = GenomeName,
            .BackgroundId = BackgroundId
        }
    End Function
End Class

''' <summary>
''' 单个 Pfam 结构域词条经过重要性打分之后的结果
''' </summary>
Public Class PfamTermScore

    ''' <summary>规范化的 Pfam 家族编号，例如 <c>PF00044</c></summary>
    Public Property PfamId As String

    ''' <summary>Pfam 家族描述文本</summary>
    Public Property Description As String

    ''' <summary>词条权重（TF-IDF 或 BM25 贡献值）</summary>
    Public Property Weight As Double

    ''' <summary>该词条在模型之中的原始重要性（排列重要性 / 相关系数的较大绝对值）</summary>
    Public Property Importance As Double

    Public Overrides Function ToString() As String
        Return $"{PfamId} = {Weight:F4}"
    End Function
End Class

''' <summary>
''' 一条有效表型注释所对应的「文档」中间表示。
''' </summary>
''' <remarks>
''' <see cref="Terms"/> / <see cref="Tf"/> / <see cref="Strength"/> 三者使用相同的
''' Pfam 编号作为键，Pfam 编号已经过 <see cref="PfamEmbedding.ParsePfamId"/> 规范化。
''' </remarks>
Public Class PhenotypeTermDocument

    ''' <summary>全局唯一的表型标识，同时用作 <see cref="Cluster.ID"/> 与 TF-IDF 文档 id</summary>
    Public Property Id As String

    ''' <summary>文档序号，用作 BM25 的整数 docId</summary>
    Public Property Index As Integer

    ''' <summary>原始的表型注释报告</summary>
    Public Property Report As ReportJSON

    ''' <summary>Pfam 编号 -&gt; 去重之后的关键特征</summary>
    Public Property Terms As Dictionary(Of String, KeyFeature)

    ''' <summary>Pfam 编号 -&gt; 量化之后的整数词频（基础 1 + 归一化重要性 × 10）</summary>
    Public Property Tf As Dictionary(Of String, Integer)

    ''' <summary>Pfam 编号 -&gt; 模型给出的原始重要性</summary>
    Public Property Strength As Dictionary(Of String, Double)

    Public Overrides Function ToString() As String
        Return $"Dim {Id} = {Terms.Count} pfam term(s)"
    End Function
End Class

''' <summary>
''' 表型关键 Pfam 结构域词条的重要性打分与筛选
''' </summary>
Public Module TermWeighting

    ''' <summary>
    ''' 把表型注释报告转换为文档集合：跳过未训练/无关键特征的表型、规范化 Pfam 编号、
    ''' 合并重复词条、保证 <see cref="PhenotypeTermDocument.Id"/> 唯一并编码整数词频。
    ''' </summary>
    ''' <param name="reports">该基因组注释出来的全部表型预测报告</param>
    ''' <returns>可被两种权重算法共用的文档集合，输入为空时返回空数组</returns>
    <Extension>
    Public Function AsTermDocuments(reports As IEnumerable(Of ReportJSON)) As PhenotypeTermDocument()
        Dim docs As New List(Of PhenotypeTermDocument)()
        Dim used As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        If reports Is Nothing Then
            Return New PhenotypeTermDocument() {}
        End If

        Dim valid As New List(Of ReportJSON)()
        Dim maxStrength As Double = 0.0

        For Each report As ReportJSON In reports
            If Not IsValidReport(report) Then
                Continue For
            End If

            valid.Add(report)

            For Each feature As KeyFeature In report.KeyFeatures
                If feature IsNot Nothing Then
                    maxStrength = std.Max(maxStrength, FeatureStrength(feature))
                End If
            Next
        Next

        For Each report As ReportJSON In valid
            Dim terms As New Dictionary(Of String, KeyFeature)(StringComparer.Ordinal)
            Dim strength As New Dictionary(Of String, Double)(StringComparer.Ordinal)

            For Each feature As KeyFeature In report.KeyFeatures
                If feature Is Nothing Then
                    Continue For
                End If

                Dim pfamId As String = NormalizePfamId(feature.PfamId)

                If pfamId Is Nothing Then
                    Continue For
                End If

                Dim value As Double = FeatureStrength(feature)

                If Not terms.ContainsKey(pfamId) Then
                    terms(pfamId) = feature
                    strength(pfamId) = value
                ElseIf value > strength(pfamId) Then
                    ' 同一个 Pfam 被重复列出时，保留判别能力最强的那一条记录
                    terms(pfamId) = feature
                    strength(pfamId) = value
                End If
            Next

            If terms.Count = 0 Then
                Continue For
            End If

            Dim tf As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

            For Each item As KeyValuePair(Of String, Double) In strength
                Dim normalized As Double = If(maxStrength > 0.0, item.Value / maxStrength, 0.0)
                ' TFIDF.Add 只接受整数 tf：基础 1 保证所有词条权重非零，
                ' 量化后的归一化重要性再叠加到 [1, 11] 区间
                tf(item.Key) = 1 + CInt(std.Round(normalized * 10.0))
            Next

            docs.Add(New PhenotypeTermDocument With {
                .Id = UniqueId(PhenotypeIdOf(report), used),
                .Index = docs.Count,
                .Report = report,
                .Terms = terms,
                .Strength = strength,
                .Tf = tf
            })
        Next

        Return docs.ToArray()
    End Function

    ''' <summary>
    ''' 对全部文档之中的 Pfam 词条打分并且按 <see cref="PhenotypeBackgroundOptions.MinScore"/>
    ''' 与 <see cref="PhenotypeBackgroundOptions.TopN"/> 筛选。
    ''' </summary>
    ''' <param name="docs">由 <see cref="AsTermDocuments"/> 得到的文档集合</param>
    ''' <param name="options">构建参数，省略时全部使用默认值</param>
    ''' <returns>表型唯一 Id -&gt; 按权重降序排列的词条数组；词条被全部过滤时返回空数组</returns>
    <Extension>
    Public Function ScoreTerms(docs As IEnumerable(Of PhenotypeTermDocument),
                               Optional options As PhenotypeBackgroundOptions = Nothing) As Dictionary(Of String, PfamTermScore())

        options = If(options, New PhenotypeBackgroundOptions())

        Dim array As PhenotypeTermDocument() = If(docs Is Nothing,
                                                  New PhenotypeTermDocument() {},
                                                  docs.ToArray())
        Dim raw As Dictionary(Of String, IEnumerable(Of PfamTermScore))

        If options.Method = PfamWeightingMethod.BM25 Then
            raw = ScoreByBM25(array, options)
        Else
            raw = ScoreByTFIDF(array, options)
        End If

        Dim result As New Dictionary(Of String, PfamTermScore())(StringComparer.Ordinal)

        For Each doc As PhenotypeTermDocument In array
            Dim scores As IEnumerable(Of PfamTermScore) = Nothing

            If raw.TryGetValue(doc.Id, scores) Then
                result(doc.Id) = ApplyFilter(scores, options)
            Else
                result(doc.Id) = New PfamTermScore() {}
            End If
        Next

        Return result
    End Function

    ''' <summary>
    ''' TF-IDF 适配器：IDF 采用 NLP 库的平滑公式 <c>ln((N+1)/(df+1)) + 1</c>。
    ''' </summary>
    Private Function ScoreByTFIDF(docs As PhenotypeTermDocument(),
                                  options As PhenotypeBackgroundOptions) As Dictionary(Of String, IEnumerable(Of PfamTermScore))

        Dim result As New Dictionary(Of String, IEnumerable(Of PfamTermScore))(StringComparer.Ordinal)
        Dim tfidf As New TFIDF()

        For Each doc As PhenotypeTermDocument In docs
            Call tfidf.Add(doc.Id, doc.Tf)
        Next

        Dim vocabulary As String() = docs _
            .SelectMany(Function(d) d.Terms.Keys) _
            .Distinct(StringComparer.Ordinal) _
            .OrderBy(Function(id) id, StringComparer.Ordinal) _
            .ToArray()

        ' TFIDF.Add 每次都会清空词表缓存，SetWords 必须放在全部 Add 之后
        Call tfidf.SetWords(vocabulary)

        ' 预计算 IDF：TFIDF.IDF(v) 内部会线性扫描全部文档，
        ' 这里每个词条只算一次，把 O(V*P) 降为 O(V + 词条总数)
        Dim idf As New Dictionary(Of String, Double)(StringComparer.Ordinal)

        For Each term As String In vocabulary
            idf(term) = tfidf.IDF(term)
        Next

        For Each doc As PhenotypeTermDocument In docs
            Dim scores As New List(Of PfamTermScore)()

            For Each term As String In doc.Terms.Keys
                scores.Add(New PfamTermScore With {
                    .PfamId = term,
                    .Description = doc.Terms(term).Description,
                    .Importance = doc.Strength(term),
                    .Weight = doc.Tf(term) * idf(term)
                })
            Next

            result(doc.Id) = scores
        Next

        Return result
    End Function

    ''' <summary>
    ''' BM25 适配器：以重复插入词条体现 tf，由 <see cref="PhenotypeBackgroundOptions.K1"/>
    ''' 与 <see cref="PhenotypeBackgroundOptions.B"/> 做词频饱和与文档长度归一。
    ''' 这里直接复用引擎公开的 IDF / 词频 / 文档长度统计量复现 ScoreDocument 的词项贡献，
    ''' 避免对每个表型做一次全库检索（O(P^2*V) -&gt; O(P*F)）。
    ''' </summary>
    Private Function ScoreByBM25(docs As PhenotypeTermDocument(),
                                 options As PhenotypeBackgroundOptions) As Dictionary(Of String, IEnumerable(Of PfamTermScore))

        Dim result As New Dictionary(Of String, IEnumerable(Of PfamTermScore))(StringComparer.Ordinal)
        Dim engine As New BM25Engine(k1:=options.K1, b:=options.B, idfMode:=IdfVariant.Lucene)

        For Each doc As PhenotypeTermDocument In docs
            Dim tokens As New List(Of String)()

            For Each item As KeyValuePair(Of String, Integer) In doc.Tf
                ' BM25Engine 内部对 token 做 ToLowerInvariant，这里统一传入小写形式
                Dim lower As String = item.Key.ToLowerInvariant()

                For i As Integer = 1 To std.Max(1, item.Value)
                    tokens.Add(lower)
                Next
            Next

            Call engine.AddDocument(doc.Index, tokens.ToArray(), doc.Id)
        Next

        ' Search 依赖 _avgDocLength，只有 BuildIndex() 之后才有意义
        Call engine.BuildIndex()

        Dim avgDocLength As Double = engine.AverageDocumentLength

        For Each doc As PhenotypeTermDocument In docs
            Dim scores As New List(Of PfamTermScore)()
            Dim docLength As Double = engine.GetDocumentLength(doc.Index)
            Dim lengthFactor As Double = If(avgDocLength > 0.0,
                                            1.0 - options.B + options.B * (docLength / avgDocLength),
                                            1.0)

            If lengthFactor <= 0.0 Then
                lengthFactor = 1.0
            End If

            For Each term As String In doc.Terms.Keys
                Dim lower As String = term.ToLowerInvariant()
                Dim tf As Integer = engine.GetTermFrequency(lower, doc.Index)

                If tf <= 0 Then
                    Continue For
                End If

                Dim contributionIdf As Double = engine.GetIdf(lower)
                Dim saturation As Double = (tf * (options.K1 + 1.0)) /
                                           (tf + options.K1 * lengthFactor)

                scores.Add(New PfamTermScore With {
                    .PfamId = term,
                    .Description = doc.Terms(term).Description,
                    .Importance = doc.Strength(term),
                    .Weight = contributionIdf * saturation
                })
            Next

            result(doc.Id) = scores
        Next

        Return result
    End Function

    ''' <summary>先按分数下限过滤，再按权重降序取 TopN</summary>
    Private Function ApplyFilter(scores As IEnumerable(Of PfamTermScore),
                                 options As PhenotypeBackgroundOptions) As PfamTermScore()

        Dim list As List(Of PfamTermScore) = scores _
            .Where(Function(s) s.Weight >= options.MinScore) _
            .OrderByDescending(Function(s) s.Weight) _
            .ThenBy(Function(s) s.PfamId, StringComparer.Ordinal) _
            .ToList()

        If options.TopN > 0 AndAlso list.Count > options.TopN Then
            list = list.GetRange(0, options.TopN)
        End If

        Return list.ToArray()
    End Function

    ''' <summary>模型给出的特征判别能力：排列重要性与相关系数取较大绝对值</summary>
    Private Function FeatureStrength(feature As KeyFeature) As Double
        Return std.Max(std.Abs(feature.Importance), std.Abs(feature.PearsonCorrelation))
    End Function

    ''' <summary>
    ''' 规范化 Pfam 编号（去版本号、去 ChouFasman 脏标记、大写）；
    ''' 解析失败时退回原始非空白文本，避免丢失模型自定义词表之中的词条。
    ''' </summary>
    Private Function NormalizePfamId(raw As String) As String
        If String.IsNullOrWhiteSpace(raw) Then
            Return Nothing
        End If

        Dim parsed As String = PfamEmbedding.ParsePfamId(raw)

        If parsed IsNot Nothing Then
            Return parsed
        End If

        Return raw.Trim().ToUpperInvariant()
    End Function

    Private Function PhenotypeIdOf(report As ReportJSON) As String
        If Not String.IsNullOrWhiteSpace(report.phenotypeId) Then
            Return report.phenotypeId.Trim()
        ElseIf Not String.IsNullOrWhiteSpace(report.accession) Then
            Return report.accession.Trim()
        Else
            Return "phenotype"
        End If
    End Function

    ''' <summary>仅跳过 skipped 与无关键特征的表型，不按预测阴阳性过滤</summary>
    Private Function IsValidReport(report As ReportJSON) As Boolean
        If report Is Nothing Then
            Return False
        End If
        If Not String.IsNullOrEmpty(report.status) AndAlso
           report.status.Equals("skipped", StringComparison.OrdinalIgnoreCase) Then
            Return False
        End If
        If report.KeyFeatures Is Nothing OrElse report.KeyFeatures.Length = 0 Then
            Return False
        End If

        Return report.KeyFeatures.Any(
            Function(f) f IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(f.PfamId))
    End Function

    ''' <summary>对空或重名的表型标识追加序号后缀，保证 Cluster.ID 全局唯一</summary>
    Private Function UniqueId(baseId As String, used As HashSet(Of String)) As String
        Dim id As String = If(String.IsNullOrWhiteSpace(baseId), "phenotype", baseId)
        Dim candidate As String = id
        Dim i As Integer = 1

        Do While Not used.Add(candidate)
            i += 1
            candidate = $"{id}#{i}"
        Loop

        Return candidate
    End Function
End Module
