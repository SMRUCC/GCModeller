' ============================================================================
'  PubMedEUtilities.vb
'  ---------------------------------------------------------------------------
'  通过 NCBI E-utilities 接口检索 PubMed 文献，获取标题、引用信息、摘要，
'  并在存在 PMC 全文时下载 PMC 全文（优先 HTML / 纯文本格式）。
'
'  依赖：
'    - .NET Framework 4.6.1+ / .NET Core 2.0+ / .NET 5+
'    - System.Net.Http
'    - System.Xml / System.Xml.Linq
'    - (可选) System.Text.Json 或 Newtonsoft.Json 用于解析 ESearch/ESummary
'             本模块默认使用 XML 接口以避免额外依赖，JSON 仅为可选。
'
'  使用约定：
'    - NCBI 要求在 User-Agent 中标注工具名与联系邮箱
'    - 无 API Key 时每秒最多 3 次请求；有 API Key 时每秒最多 10 次
'    - 大批量检索请使用 retstart / retmax 分页，并启用历史记录（WebEnv/QueryKey）
'
'  作者: Qingyan Agent
' ============================================================================

Option Explicit On
Option Strict On
Option Infer On

Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Xml
Imports System.Xml.Linq

Namespace PubMedFetcher

    ' ===========================================================================
    '  数据类
    ' ===========================================================================

    ''' <summary>单篇 PubMed 文献的完整信息。</summary>
    Public Class PubMedArticle

        ''' <summary>PubMed 唯一标识符。</summary>
        Public Property Pmid As String

        ''' <summary>文献标题。</summary>
        Public Property Title As String

        ''' <summary>作者列表（"姓 名首字母" 形式）。</summary>
        Public Property Authors As New List(Of String)()

        ''' <summary>期刊全名。</summary>
        Public Property Journal As String

        ''' <summary>期刊缩写。</summary>
        Public Property JournalAbbrev As String

        ''' <summary>ISSN。</summary>
        Public Property Issn As String

        ''' <summary>发表日期（自由文本，来自 PubMed）。</summary>
        Public Property PubDate As String

        ''' <summary>卷。</summary>
        Public Property Volume As String

        ''' <summary>期。</summary>
        Public Property Issue As String

        ''' <summary>页码。</summary>
        Public Property Pages As String

        ''' <summary>DOI。</summary>
        Public Property Doi As String

        ''' <summary>格式化引用字符串（Vancouver 风格）。</summary>
        Public Property Citation As String

        ''' <summary>摘要全文（多段以换行连接）。</summary>
        Public Property Abstract As String

        ''' <summary>PMC ID（如 "1234567"，不含 "PMC" 前缀）。</summary>
        Public Property PmcId As String

        ''' <summary>是否成功获取到 PMC 全文。</summary>
        Public Property HasPmcFullText As Boolean

        ''' <summary>PMC 全文内容（HTML 或纯文本）。</summary>
        Public Property PmcFullText As String

        ''' <summary>PMC 全文格式："html" / "text" / ""（未获取）。</summary>
        Public Property PmcFullTextFormat As String

        ''' <summary>获取过程中产生的错误或警告信息。</summary>
        Public Property Messages As New List(Of String)()

        Public Overrides Function ToString() As String
            Return $"PMID={Pmid} | {JournalAbbrev} {PubDate} | {Title}"
        End Function

    End Class


    ''' <summary>检索选项。</summary>
    Public Class SearchOptions

        ''' <summary>检索词，例如 "covid-19 vaccine[Title/Abstract] AND 2023[dp]"。</summary>
        Public Property Term As String

        ''' <summary>每页返回条数（默认 20，最大 10000）。</summary>
        Public Property RetMax As Integer = 20

        ''' <summary>起始偏移（用于分页，从 0 开始）。</summary>
        Public Property RetStart As Integer = 0

        ''' <summary>排序方式：relevance / pub_date / Author 等。</summary>
        Public Property Sort As String = "relevance"

        ''' <summary>是否在获取摘要后尝试下载 PMC 全文。</summary>
        Public Property FetchPmcFullText As Boolean = True

        ''' <summary>PMC 全文优先格式："html" 或 "text"（默认 html）。</summary>
        Public Property PreferredPmcFormat As String = "html"

        ''' <summary>是否将 PMC 全文写入磁盘（为空则不写）。</summary>
        Public Property OutputDirectory As String = ""

        ''' <summary>单篇文献请求超时（秒）。</summary>
        Public Property TimeoutSeconds As Integer = 60

        ''' <summary>取消令牌。</summary>
        Public Property CancellationToken As CancellationToken = CancellationToken.None

    End Class


    ' ===========================================================================
    '  主模块
    ' ===========================================================================

    ''' <summary>
    ''' NCBI E-utilities 客户端：检索 PubMed、获取摘要、下载 PMC 全文。
    ''' 线程安全；内部使用信号量做速率限制。
    ''' </summary>
    Public NotInheritable Class PubMedEUtilities

        ' --- 基础常量 ----------------------------------------------------------
        Private Const BaseUrl As String = "https://eutils.ncbi.nlm.nih.gov/entrez/eutils/"
        Private Const EsearchUrl As String = BaseUrl & "esearch.fcgi"
        Private Const EsummaryUrl As String = BaseUrl & "esummary.fcgi"
        Private Const EfetchUrl As String = BaseUrl & "efetch.fcgi"
        Private Const ElinkUrl As String = BaseUrl & "elink.fcgi"

        ' --- 配置 -------------------------------------------------------------
        Private Shared ReadOnly _client As HttpClient
        Private Shared _apiKey As String = ""
        Private Shared _email As String = ""
        Private Shared _toolName As String = "PubMedFetcher"

        ' 速率限制：无 key 3 req/s，有 key 10 req/s
        Private Shared _minIntervalMs As Integer = 340   ' ≈ 1000/3
        Private Shared ReadOnly _gate As New SemaphoreSlim(1, 1)
        Private Shared _lastRequestUtc As DateTime = DateTime.UtcNow

        ' --- 静态构造 ---------------------------------------------------------
        Shared Sub New()
            _client = New HttpClient()
            _client.DefaultRequestHeaders.ConnectionClose = False
            _client.Timeout = TimeSpan.FromMinutes(5)
            UpdateUserAgent()
        End Sub

        Private Shared Sub UpdateUserAgent()
            _client.DefaultRequestHeaders.UserAgent.Clear()
            Dim product As New Headers.ProductInfoHeaderValue(_toolName, "1.0")
            _client.DefaultRequestHeaders.UserAgent.Add(product)
            If Not String.IsNullOrEmpty(_email) Then
                Dim comment As New Headers.ProductInfoHeaderValue("(" & _email & ")")
                _client.DefaultRequestHeaders.UserAgent.Add(comment)
            End If
        End Sub

        ''' <summary>
        ''' 配置客户端。建议在程序启动时调用一次。
        ''' </summary>
        ''' <param name="email">联系邮箱（NCBI 要求标注）。</param>
        ''' <param name="apiKey">NCBI API Key（可选，可大幅提升速率上限）。</param>
        ''' <param name="toolName">工具名（默认 PubMedFetcher）。</param>
        Public Shared Sub Configure(Optional email As String = "",
                                    Optional apiKey As String = "",
                                    Optional toolName As String = "PubMedFetcher")
            _email = If(email, "")
            _apiKey = If(apiKey, "")
            If Not String.IsNullOrEmpty(toolName) Then _toolName = toolName
            ' 有 API Key 时允许 10 req/s
            _minIntervalMs = If(String.IsNullOrEmpty(_apiKey), 340, 100)
            UpdateUserAgent()
        End Sub

        ''' <summary>设置 HTTP 超时（默认 5 分钟）。</summary>
        Public Shared Sub SetTimeout(timeout As TimeSpan)
            _client.Timeout = timeout
        End Sub


        ' ======================================================================
        '  公开高层 API
        ' ======================================================================

        ''' <summary>
        ''' 一站式检索：执行 ESearch → EFetch(pubmed) 解析题录与摘要 →
        ''' 若存在 PMC ID 则下载 PMC 全文（HTML 或纯文本）。
        ''' </summary>
        ''' <returns>检索到的文献列表。</returns>
        Public Shared Async Function SearchAndFetchAsync(
            options As SearchOptions) As Task(Of List(Of PubMedArticle))

            If options Is Nothing Then Throw New ArgumentNullException(NameOf(options))
            If String.IsNullOrWhiteSpace(options.Term) Then
                Throw New ArgumentException("SearchOptions.Term 不能为空。")
            End If

            Dim ct As CancellationToken = options.CancellationToken

            ' 1) ESearch 获取 PMID 列表
            Dim pmids As List(Of String) = Await ESearchAsync(
                options.Term, options.RetMax, options.RetStart, options.Sort, ct)

            If pmids.Count = 0 Then Return New List(Of PubMedArticle)()

            ' 2) EFetch(pubmed, xml) 一次性获取所有 PMID 的题录 + 摘要 + ArticleIdList
            Dim articles As List(Of PubMedArticle) = Await FetchPubmedRecordsAsync(pmids, ct)

            ' 3) 对存在 PMC ID 的文献下载全文
            If options.FetchPmcFullText Then
                For Each art In articles
                    ct.ThrowIfCancellationRequested()
                    If String.IsNullOrEmpty(art.PmcId) Then Continue For
                    Try
                        Await FetchPmcFullTextAsync(art, options.PreferredPmcFormat,
                                                    options.OutputDirectory, ct)
                    Catch ex As Exception
                        art.Messages.Add($"PMC 全文获取失败: {ex.Message}")
                    End Try
                Next
            End If

            Return articles
        End Function


        ''' <summary>根据单个 PMID 获取文献（含摘要，可选 PMC 全文）。</summary>
        Public Shared Async Function FetchByPmidAsync(
            pmid As String,
            Optional fetchPmcFullText As Boolean = True,
            Optional preferredPmcFormat As String = "html",
            Optional outputDirectory As String = "",
            Optional ct As CancellationToken = Nothing) As Task(Of PubMedArticle)

            If String.IsNullOrWhiteSpace(pmid) Then Throw New ArgumentNullException(NameOf(pmid))

            Dim list As List(Of PubMedArticle) = Await FetchPubmedRecordsAsync(
                New List(Of String) From {pmid}, ct)
            If list.Count = 0 Then Return Nothing

            Dim art As PubMedArticle = list(0)

            If fetchPmcFullText AndAlso Not String.IsNullOrEmpty(art.PmcId) Then
                Try
                    Await FetchPmcFullTextAsync(art, preferredPmcFormat, outputDirectory, ct)
                Catch ex As Exception
                    art.Messages.Add($"PMC 全文获取失败: {ex.Message}")
                End Try
            End If
            Return art
        End Function


        ' ======================================================================
        '  ESearch
        ' ======================================================================

        ''' <summary>
        ''' 执行 ESearch，返回 PMID 列表。
        ''' 使用 retmode=xml 以避免 JSON 依赖。
        ''' </summary>
        Public Shared Async Function ESearchAsync(
            term As String,
            Optional retMax As Integer = 20,
            Optional retStart As Integer = 0,
            Optional sort As String = "relevance",
            Optional ct As CancellationToken = Nothing) As Task(Of List(Of String))

            Dim sb As New StringBuilder()
            sb.Append("db=pubmed")
            sb.Append("&retmode=xml")
            sb.Append("&rettype=core")
            sb.Append("&usehistory=n")
            sb.Append("&retmax=").Append(retMax)
            sb.Append("&retstart=").Append(retStart)
            If Not String.IsNullOrEmpty(sort) Then sb.Append("&sort=").Append(UrlEncode(sort))
            sb.Append("&term=").Append(UrlEncode(term))
            AppendCommonParams(sb)

            Dim xml As String = Await HttpGetAsync(EsearchUrl & "?" & sb.ToString(), ct)

            Dim doc As XDocument = XDocument.Parse(xml)
            Dim root As XElement = doc.Root
            ' 命中总数
            ' Dim count As String = root.Element("Count")?.Value

            Dim pmids As New List(Of String)()
            Dim idList As XElement = root.Element("IdList")
            If idList IsNot Nothing Then
                For Each idEl As XElement In idList.Elements("Id")
                    pmids.Add(idEl.Value)
                Next
            End If
            Return pmids
        End Function


        ' ======================================================================
        '  ESummary（可选，本模块主要使用 EFetch 解析题录；保留以备使用）
        ' ======================================================================

        ''' <summary>执行 ESummary，返回原始 XML 字符串。</summary>
        Public Shared Async Function ESummaryAsync(
            pmids As IEnumerable(Of String),
            Optional ct As CancellationToken = Nothing) As Task(Of String)

            Dim idList As String = String.Join(",", pmids)
            Dim sb As New StringBuilder()
            sb.Append("db=pubmed")
            sb.Append("&retmode=xml")
            sb.Append("&id=").Append(UrlEncode(idList))
            AppendCommonParams(sb)

            Return Await HttpGetAsync(EsummaryUrl & "?" & sb.ToString(), ct)
        End Function


        ' ======================================================================
        '  EFetch (pubmed)
        ' ======================================================================

        ''' <summary>
        ''' 调用 EFetch(db=pubmed, retmode=xml) 获取题录与摘要，并解析为 PubMedArticle 列表。
        ''' 一次最多 200 个 PMID（NCBI 建议值）。
        ''' </summary>
        Public Shared Async Function FetchPubmedRecordsAsync(
            pmids As IEnumerable(Of String),
            Optional ct As CancellationToken = Nothing) As Task(Of List(Of PubMedArticle))

            Dim idList As String = String.Join(",", pmids)
            Dim sb As New StringBuilder()
            sb.Append("db=pubmed")
            sb.Append("&retmode=xml")
            sb.Append("&rettype=abstract")
            sb.Append("&id=").Append(UrlEncode(idList))
            AppendCommonParams(sb)

            Dim xml As String = Await HttpGetAsync(EfetchUrl & "?" & sb.ToString(), ct)
            Return ParsePubmedXml(xml)
        End Function


        ''' <summary>解析 PubMed EFetch XML，返回 PubMedArticle 列表。</summary>
        Public Shared Function ParsePubmedXml(xml As String) As List(Of PubMedArticle)
            Dim result As New List(Of PubMedArticle)()
            If String.IsNullOrWhiteSpace(xml) Then Return result

            Dim doc As XDocument
            Try
                doc = XDocument.Parse(xml)
            Catch ex As Exception
                Throw New InvalidOperationException("PubMed XML 解析失败: " & ex.Message, ex)
            End Try

            Dim root As XElement = doc.Root  ' PubmedArticleSet
            If root Is Nothing Then Return result

            For Each pa As XElement In root.Elements("PubmedArticle")
                Dim art As New PubMedArticle()

                Dim medlineCitation As XElement = pa.Element("MedlineCitation")
                If medlineCitation Is Nothing Then Continue For

                Dim articleEl As XElement = medlineCitation.Element("Article")
                If articleEl Is Nothing Then Continue For

                ' PMID
                Dim pmidEl As XElement = medlineCitation.Element("PMID")
                If pmidEl IsNot Nothing Then art.Pmid = pmidEl.Value

                ' 标题
                Dim titleEl As XElement = articleEl.Element("ArticleTitle")
                If titleEl IsNot Nothing Then art.Title = CleanText(titleEl.Value)

                ' 作者
                Dim authorList As XElement = articleEl.Element("AuthorList")
                If authorList IsNot Nothing Then
                    For Each au As XElement In authorList.Elements("Author")
                        Dim last As String = GetElementValue(au, "LastName")
                        Dim initials As String = GetElementValue(au, "Initials")
                        Dim fore As String = GetElementValue(au, "ForeName")
                        Dim collective As String = GetElementValue(au, "CollectiveName")
                        If Not String.IsNullOrEmpty(collective) Then
                            art.Authors.Add(collective)
                        ElseIf Not String.IsNullOrEmpty(last) Then
                            If Not String.IsNullOrEmpty(initials) Then
                                art.Authors.Add($"{last} {initials}")
                            ElseIf Not String.IsNullOrEmpty(fore) Then
                                art.Authors.Add($"{last} {fore}")
                            Else
                                art.Authors.Add(last)
                            End If
                        End If
                    Next
                End If

                ' 期刊信息
                Dim journalEl As XElement = articleEl.Element("Journal")
                If journalEl IsNot Nothing Then
                    art.Journal = GetElementValue(journalEl, "Title")
                    art.JournalAbbrev = GetElementValue(journalEl, "ISOAbbreviation")
                    Dim issnEl As XElement = journalEl.Element("ISSN")
                    If issnEl IsNot Nothing Then art.Issn = issnEl.Value

                    Dim journalIssue As XElement = journalEl.Element("JournalIssue")
                    If journalIssue IsNot Nothing Then
                        art.Volume = GetElementValue(journalIssue, "Volume")
                        art.Issue = GetElementValue(journalIssue, "Issue")
                        Dim pubDateEl As XElement = journalIssue.Element("PubDate")
                        If pubDateEl IsNot Nothing Then art.PubDate = FormatPubDate(pubDateEl)
                    End If
                End If

                ' 页码
                Dim pagination As XElement = articleEl.Element("Pagination")
                If pagination IsNot Nothing Then
                    Dim medlinePgn As XElement = pagination.Element("MedlinePgn")
                    If medlinePgn IsNot Nothing Then art.Pages = medlinePgn.Value
                End If

                ' ELocationID 中的 DOI
                For Each eloc As XElement In articleEl.Elements("ELocationID")
                    If String.Equals(eloc.Attribute("EIdType")?.Value, "doi", StringComparison.OrdinalIgnoreCase) Then
                        art.Doi = eloc.Value
                        Exit For
                    End If
                Next

                ' 摘要
                Dim abstractEl As XElement = articleEl.Element("Abstract")
                If abstractEl IsNot Nothing Then
                    Dim parts As New List(Of String)()
                    For Each at As XElement In abstractEl.Elements("AbstractText")
                        Dim label As String = at.Attribute("Label")?.Value
                        Dim txt As String = CleanText(at.Value)
                        If Not String.IsNullOrEmpty(label) Then
                            parts.Add($"{label}: {txt}")
                        Else
                            parts.Add(txt)
                        End If
                    Next
                    art.Abstract = String.Join(Environment.NewLine & Environment.NewLine, parts)
                End If

                ' ArticleIdList：DOI / PMC / pubmed
                Dim pubmedData As XElement = pa.Element("PubmedData")
                If pubmedData IsNot Nothing Then
                    Dim articleIdList As XElement = pubmedData.Element("ArticleIdList")
                    If articleIdList IsNot Nothing Then
                        For Each aid As XElement In articleIdList.Elements("ArticleId")
                            Dim idType As String = aid.Attribute("IdType")?.Value
                            If String.IsNullOrEmpty(idType) Then Continue For
                            Select Case idType.ToLowerInvariant()
                                Case "pmc"
                                    art.PmcId = aid.Value.Replace("PMC", "").Trim()
                                Case "doi"
                                    If String.IsNullOrEmpty(art.Doi) Then art.Doi = aid.Value
                                Case "pubmed"
                                    If String.IsNullOrEmpty(art.Pmid) Then art.Pmid = aid.Value
                            End Select
                        Next
                    End If
                End If

                ' 组装 Vancouver 风格引用
                art.Citation = BuildCitation(art)

                result.Add(art)
            Next

            Return result
        End Function


        ' ======================================================================
        '  EFetch (pmc) - 全文
        ' ======================================================================

        ''' <summary>
        ''' 下载 PMC 全文。优先尝试 HTML，失败则回退纯文本。
        ''' </summary>
        ''' <param name="article">目标文献（需已填充 PmcId）。</param>
        ''' <param name="preferredFormat">"html" 或 "text"。</param>
        ''' <param name="outputDirectory">若非空，则将全文写入该目录。</param>
        Public Shared Async Function FetchPmcFullTextAsync(
            article As PubMedArticle,
            Optional preferredFormat As String = "html",
            Optional outputDirectory As String = "",
            Optional ct As CancellationToken = Nothing) As Task

            If article Is Nothing Then Throw New ArgumentNullException(NameOf(article))
            If String.IsNullOrEmpty(article.PmcId) Then
                Throw New InvalidOperationException("该文献没有 PMC ID，无法获取全文。")
            End If

            preferredFormat = (If(preferredFormat, "html")).ToLowerInvariant().Trim()
            If preferredFormat <> "html" AndAlso preferredFormat <> "text" Then
                preferredFormat = "html"
            End If

            ' EFetch(db=pmc) 默认返回 XML；NCBI 不直接提供 HTML 输出。
            ' 因此策略为：先取 XML，再本地转换为 HTML 或纯文本。
            Dim xml As String = Await FetchPmcXmlAsync(article.PmcId, ct)

            If String.IsNullOrWhiteSpace(xml) Then
                article.Messages.Add("PMC EFetch 返回空内容。")
                Return
            End If

            ' 检查是否为错误响应（如 "Cannot retrieve article"）
            If xml.Contains("<ERROR>") OrElse xml.Contains("cannot be found") Then
                article.Messages.Add("PMC 全文不可用: " & xml.Substring(0, Math.Min(200, xml.Length)))
                Return
            End If

            Dim formatsTried As New List(Of String)()

            ' 按优先级尝试
            Dim order As String() = If(preferredFormat = "html",
                                       New String() {"html", "text"},
                                       New String() {"text", "html"})

            For Each fmt As String In order
                formatsTried.Add(fmt)
                Try
                    If fmt = "html" Then
                        article.PmcFullText = ConvertPmcXmlToHtml(xml, article)
                        article.PmcFullTextFormat = "html"
                    Else
                        article.PmcFullText = ConvertPmcXmlToText(xml)
                        article.PmcFullTextFormat = "text"
                    End If
                    article.HasPmcFullText = Not String.IsNullOrWhiteSpace(article.PmcFullText)
                    If article.HasPmcFullText Then Exit For
                Catch ex As Exception
                    article.Messages.Add($"PMC 全文转换为 {fmt} 失败: {ex.Message}")
                End Try
            Next

            ' 写入磁盘
            If article.HasPmcFullText AndAlso Not String.IsNullOrEmpty(outputDirectory) Then
                Try
                    WritePmcToFile(article, outputDirectory)
                Catch ex As Exception
                    article.Messages.Add($"PMC 全文写入磁盘失败: {ex.Message}")
                End Try
            End If
        End Function


        ''' <summary>调用 EFetch(db=pmc, retmode=xml) 获取 PMC 全文 XML。</summary>
        Public Shared Async Function FetchPmcXmlAsync(
            pmcId As String,
            Optional ct As CancellationToken = Nothing) As Task(Of String)

            If String.IsNullOrEmpty(pmcId) Then Throw New ArgumentNullException(NameOf(pmcId))
            pmcId = pmcId.Replace("PMC", "").Trim()

            Dim sb As New StringBuilder()
            sb.Append("db=pmc")
            sb.Append("&retmode=xml")
            sb.Append("&rettype=full")
            sb.Append("&id=PMC").Append(pmcId)
            AppendCommonParams(sb)

            Return Await HttpGetAsync(EfetchUrl & "?" & sb.ToString(), ct)
        End Function


        ''' <summary>使用 ELINK 检查 PMID 是否有对应的 PMC 全文，返回 PMC ID（无则空）。</summary>
        Public Shared Async Function FindPmcIdByPmidAsync(
            pmid As String,
            Optional ct As CancellationToken = Nothing) As Task(Of String)

            Dim sb As New StringBuilder()
            sb.Append("dbfrom=pubmed")
            sb.Append("&db=pmc")
            sb.Append("&retmode=xml")
            sb.Append("&id=").Append(UrlEncode(pmid))
            AppendCommonParams(sb)

            Dim xml As String = Await HttpGetAsync(ElinkUrl & "?" & sb.ToString(), ct)
            Dim doc As XDocument = XDocument.Parse(xml)
            Dim root As XElement = doc.Root
            If root Is Nothing Then Return ""

            For Each linkSet As XElement In root.Elements("LinkSet")
                Dim linkSetDb As XElement = linkSet.Element("LinkSetDb")
                If linkSetDb IsNot Nothing Then
                    For Each link As XElement In linkSetDb.Elements("Link")
                        Dim idEl As XElement = link.Element("Id")
                        If idEl IsNot Nothing Then Return idEl.Value
                    Next
                End If
            Next
            Return ""
        End Function


        ' ======================================================================
        '  PMC XML → HTML / 纯文本 转换
        ' ======================================================================

        ''' <summary>
        ''' 将 PMC 全文 XML 转换为结构化 HTML 文档。
        ''' 保留标题、章节、段落、表格（简化）、图说明。
        ''' </summary>
        Public Shared Function ConvertPmcXmlToHtml(xml As String, article As PubMedArticle) As String
            Dim doc As XDocument = XDocument.Parse(xml)
            Dim sb As New StringBuilder()

            sb.AppendLine("<!DOCTYPE html>")
            sb.AppendLine("<html lang=""en"">")
            sb.AppendLine("<head>")
            sb.AppendLine("<meta charset=""utf-8""/>")
            sb.AppendLine("<meta name=""viewport"" content=""width=device-width, initial-scale=1""/>")
            sb.AppendLine("<title>").Append(HtmlEncode(article.Title)).AppendLine("</title>")
            sb.AppendLine("<style>")
            sb.AppendLine("body{font-family:Georgia,'Times New Roman',serif;max-width:820px;margin:2em auto;padding:0 1em;line-height:1.7;color:#222;}")
            sb.AppendLine("h1{font-size:1.6em;border-bottom:2px solid #333;padding-bottom:.3em;}")
            sb.AppendLine("h2{font-size:1.25em;margin-top:1.6em;border-left:4px solid #2a6496;padding-left:.5em;}")
            sb.AppendLine("h3{font-size:1.1em;color:#444;}")
            sb.AppendLine(".meta{color:#555;font-size:.95em;margin-bottom:1.5em;}")
            sb.AppendLine(".abstract{background:#f7f7f7;padding:1em 1.2em;border-radius:4px;}")
            sb.AppendLine("table{border-collapse:collapse;width:100%;margin:1em 0;}")
            sb.AppendLine("th,td{border:1px solid #ccc;padding:.4em .6em;text-align:left;}")
            sb.AppendLine("th{background:#eee;}")
            sb.AppendLine(".fig-caption,.table-caption{font-size:.9em;color:#666;font-style:italic;margin:.5em 0;}")
            sb.AppendLine("</style>")
            sb.AppendLine("</head>")
            sb.AppendLine("<body>")

            ' 标题与元信息
            sb.AppendLine("<h1>" & HtmlEncode(article.Title) & "</h1>")
            sb.Append("<div class=""meta"">")
            If article.Authors.Count > 0 Then
                sb.Append(HtmlEncode(String.Join(", ", article.Authors)))
            End If
            sb.Append("<br/>")
            sb.Append(HtmlEncode(article.Journal))
            If Not String.IsNullOrEmpty(article.PubDate) Then sb.Append(". ").Append(HtmlEncode(article.PubDate))
            If Not String.IsNullOrEmpty(article.Volume) Then sb.Append(";").Append(HtmlEncode(article.Volume))
            If Not String.IsNullOrEmpty(article.Issue) Then sb.Append("(").Append(HtmlEncode(article.Issue)).Append(")")
            If Not String.IsNullOrEmpty(article.Pages) Then sb.Append(":").Append(HtmlEncode(article.Pages))
            sb.Append("<br/>")
            sb.Append($"PMID: {article.Pmid}")
            If Not String.IsNullOrEmpty(article.PmcId) Then sb.Append($" | PMCID: PMC{article.PmcId}")
            If Not String.IsNullOrEmpty(article.Doi) Then sb.Append($" | DOI: <a href=""https://doi.org/{article.Doi}"">{article.Doi}</a>")
            sb.AppendLine("</div>")

            ' 摘要
            If Not String.IsNullOrEmpty(article.Abstract) Then
                sb.AppendLine("<div class=""abstract"">")
                sb.AppendLine("<h2>Abstract</h2>")
                For Each para In article.Abstract.Split({Environment.NewLine & Environment.NewLine}, StringSplitOptions.None)
                    sb.AppendLine("<p>" & HtmlEncode(para) & "</p>")
                Next
                sb.AppendLine("</div>")
            End If

            ' 解析 <article> 主体
            Dim articleEl As XElement = doc.Root
            ' PMC XML 根为 <pmc-articleset>，其下为 <article>
            If articleEl.Name.LocalName = "pmc-articleset" Then
                articleEl = articleEl.Element(XName.Get("article", articleEl.GetDefaultNamespace().NamespaceName))
            End If
            If articleEl Is Nothing OrElse articleEl.Name.LocalName <> "article" Then
                sb.AppendLine("<p><em>(未找到 &lt;article&gt; 节点，可能是预印本或撤稿记录。)</em></p>")
                sb.AppendLine("</body></html>")
                Return sb.ToString()
            End If

            Dim ns As XNamespace = articleEl.GetDefaultNamespace()

            ' body
            Dim body As XElement = articleEl.Element(ns + "body")
            If body IsNot Nothing Then
                RenderSectionToHtml(body, ns, sb, level:=2)
            Else
                sb.AppendLine("<p><em>(该 PMC 记录未包含 &lt;body&gt; 全文。)</em></p>")
            End If

            ' 参考文献（可选）
            Dim back As XElement = articleEl.Element(ns + "back")
            If back IsNot Nothing Then
                Dim refList As XElement = back.Element(ns + "ref-list")
                If refList IsNot Nothing Then
                    sb.AppendLine("<h2>References</h2>")
                    sb.AppendLine("<ol>")
                    For Each refEl As XElement In refList.Elements(ns + "ref")
                        Dim citation As XElement = refEl.Element(ns + "mixed-citation")
                        If citation Is Nothing Then citation = refEl.Element(ns + "element-citation")
                        If citation Is Nothing Then citation = refEl.Element(ns + "citation")
                        If citation IsNot Nothing Then
                            sb.AppendLine("<li>" & HtmlEncode(CleanText(citation.Value)) & "</li>")
                        End If
                    Next
                    sb.AppendLine("</ol>")
                End If
            End If

            sb.AppendLine("</body>")
            sb.AppendLine("</html>")
            Return sb.ToString()
        End Function


        ''' <summary>递归渲染章节为 HTML。</summary>
        Private Shared Sub RenderSectionToHtml(el As XElement, ns As XNamespace,
                                               sb As StringBuilder, level As Integer)
            For Each child As XElement In el.Elements()
                Select Case child.Name.LocalName
                    Case "sec"
                        Dim titleEl As XElement = child.Element(ns + "title")
                        If titleEl IsNot Nothing Then
                            Dim tag As String = If(level <= 6, "h" & level, "h6")
                            sb.AppendLine($"<{tag}>{HtmlEncode(CleanText(titleEl.Value))}</{tag}>")
                        End If
                        RenderSectionToHtml(child, ns, sb, level + 1)

                    Case "p"
                        sb.AppendLine("<p>" & RenderInlineHtml(child, ns) & "</p>")

                    Case "title"
                        ' 已在 sec 中处理；其他位置忽略

                    Case "list"
                        Dim listType As String = If(child.Attribute("list-type")?.Value = "order", "ol", "ul")
                        sb.AppendLine($"<{listType}>")
                        For Each li As XElement In child.Elements(ns + "list-item")
                            sb.AppendLine("<li>" & RenderInlineHtml(li, ns) & "</li>")
                        Next
                        sb.AppendLine($"</{listType}>")

                    Case "table-wrap"
                        Dim caption As XElement = child.Element(ns + "caption")
                        If caption IsNot Nothing Then
                            sb.AppendLine("<div class=""table-caption"">" & HtmlEncode(CleanText(caption.Value)) & "</div>")
                        End If
                        Dim tbl As XElement = child.Element(ns + "table")
                        If tbl IsNot Nothing Then
                            sb.AppendLine(ConvertTableElementToHtml(tbl, ns))
                        End If

                    Case "fig"
                        Dim caption As XElement = child.Element(ns + "caption")
                        If caption IsNot Nothing Then
                            sb.AppendLine("<div class=""fig-caption"">" & HtmlEncode(CleanText(caption.Value)) & "</div>")
                        End If

                    Case "table"
                        sb.AppendLine(ConvertTableElementToHtml(child, ns))

                    Case Else
                        ' 其他节点：尝试递归
                        If child.HasElements Then
                            RenderSectionToHtml(child, ns, sb, level)
                        End If
                End Select
            Next
        End Sub


        ''' <summary>渲染段落内联内容（粗体、斜体、链接、上标、下标）。</summary>
        Private Shared Function RenderInlineHtml(el As XElement, ns As XNamespace) As String
            Dim sb As New StringBuilder()
            For Each node As XNode In el.Nodes()
                RenderNodeInline(node, ns, sb)
            Next
            Return sb.ToString()
        End Function


        Private Shared Sub RenderNodeInline(node As XNode, ns As XNamespace, sb As StringBuilder)
            Dim el As XElement = TryCast(node, XElement)
            If el Is Nothing Then
                Dim t As XText = TryCast(node, XText)
                If t IsNot Nothing Then sb.Append(HtmlEncode(t.Value))
                Return
            End If

            Select Case el.Name.LocalName
                Case "bold", "b"
                    sb.Append("<strong>")
                    For Each c As XNode In el.Nodes() : RenderNodeInline(c, ns, sb) : Next
                    sb.Append("</strong>")
                Case "italic", "i"
                    sb.Append("<em>")
                    For Each c As XNode In el.Nodes() : RenderNodeInline(c, ns, sb) : Next
                    sb.Append("</em>")
                Case "sub"
                    sb.Append("<sub>")
                    For Each c As XNode In el.Nodes() : RenderNodeInline(c, ns, sb) : Next
                    sb.Append("</sub>")
                Case "sup"
                    sb.Append("<sup>")
                    For Each c As XNode In el.Nodes() : RenderNodeInline(c, ns, sb) : Next
                    sb.Append("</sup>")
                Case "ext-link", "uri"
                    Dim href As String = el.Value
                    ' JATS 中 ext-link 通常使用 xlink:href 属性
                    Dim hrefAttr As XAttribute = FindAttributeByLocalName(el, "href")
                    If hrefAttr IsNot Nothing Then href = hrefAttr.Value
                    sb.Append($"<a href=""{HtmlEncode(href)}"" target=""_blank"">")
                    For Each c As XNode In el.Nodes() : RenderNodeInline(c, ns, sb) : Next
                    sb.Append("</a>")
                Case "xref"
                    ' 内部交叉引用，仅显示文字
                    For Each c As XNode In el.Nodes() : RenderNodeInline(c, ns, sb) : Next
                Case Else
                    ' 默认：递归子节点 + 文本
                    For Each c As XNode In el.Nodes() : RenderNodeInline(c, ns, sb) : Next
            End Select
        End Sub


        ''' <summary>将 JATS &lt;table&gt; 元素转为 HTML 表格。</summary>
        Private Shared Function ConvertTableElementToHtml(tbl As XElement, ns As XNamespace) As String
            Dim sb As New StringBuilder()
            sb.AppendLine("<table>")
            For Each row As XElement In tbl.Descendants(ns + "tr")
                sb.AppendLine("<tr>")
                For Each cell As XElement In row.Elements()
                    Dim tag As String = If(cell.Name.LocalName = "th", "th", "td")
                    sb.AppendLine($"<{tag}>{HtmlEncode(CleanText(cell.Value))}</{tag}>")
                Next
                sb.AppendLine("</tr>")
            Next
            sb.AppendLine("</table>")
            Return sb.ToString()
        End Function


        ''' <summary>
        ''' 将 PMC 全文 XML 转换为纯文本（去除所有标签，保留段落结构）。
        ''' </summary>
        Public Shared Function ConvertPmcXmlToText(xml As String) As String
            Dim doc As XDocument = XDocument.Parse(xml)
            Dim root As XElement = doc.Root
            If root.Name.LocalName = "pmc-articleset" Then
                root = root.Element(root.GetDefaultNamespace() + "article")
            End If
            If root Is Nothing OrElse root.Name.LocalName <> "article" Then Return ""

            Dim ns As XNamespace = root.GetDefaultNamespace()
            Dim sb As New StringBuilder()

            ' front 中的标题与作者
            Dim front As XElement = root.Element(ns + "front")
            If front IsNot Nothing Then
                Dim articleMeta As XElement = front.Element(ns + "article-meta")
                If articleMeta IsNot Nothing Then
                    Dim titleGrp As XElement = articleMeta.Element(ns + "title-group")
                    If titleGrp IsNot Nothing Then
                        Dim at As XElement = titleGrp.Element(ns + "article-title")
                        If at IsNot Nothing Then
                            sb.AppendLine(CleanText(at.Value))
                            sb.AppendLine(New String("="c, 60))
                            sb.AppendLine()
                        End If
                    End If
                End If
            End If

            Dim body As XElement = root.Element(ns + "body")
            If body IsNot Nothing Then
                RenderSectionToText(body, ns, sb, level:=0)
            End If

            ' 参考文献
            Dim back As XElement = root.Element(ns + "back")
            If back IsNot Nothing Then
                Dim refList As XElement = back.Element(ns + "ref-list")
                If refList IsNot Nothing Then
                    sb.AppendLine()
                    sb.AppendLine("References")
                    sb.AppendLine(New String("-"c, 60))
                    Dim idx As Integer = 1
                    For Each refEl As XElement In refList.Elements(ns + "ref")
                        Dim citation As XElement = refEl.Element(ns + "mixed-citation")
                        If citation Is Nothing Then citation = refEl.Element(ns + "element-citation")
                        If citation Is Nothing Then citation = refEl.Element(ns + "citation")
                        If citation IsNot Nothing Then
                            sb.AppendLine($"[{idx}] {CleanText(citation.Value)}")
                            idx += 1
                        End If
                    Next
                End If
            End If

            Return sb.ToString().TrimEnd()
        End Function


        Private Shared Sub RenderSectionToText(el As XElement, ns As XNamespace,
                                               sb As StringBuilder, level As Integer)
            For Each child As XElement In el.Elements()
                Select Case child.Name.LocalName
                    Case "sec"
                        Dim titleEl As XElement = child.Element(ns + "title")
                        If titleEl IsNot Nothing Then
                            sb.AppendLine()
                            sb.AppendLine(CleanText(titleEl.Value))
                            sb.AppendLine(New String("-"c, Math.Max(3, CleanText(titleEl.Value).Length)))
                        End If
                        RenderSectionToText(child, ns, sb, level + 1)

                    Case "p"
                        Dim txt As String = CleanText(child.Value)
                        If Not String.IsNullOrEmpty(txt) Then
                            sb.AppendLine(txt)
                            sb.AppendLine()
                        End If

                    Case "list"
                        For Each li As XElement In child.Elements(ns + "list-item")
                            sb.AppendLine("  • " & CleanText(li.Value))
                        Next
                        sb.AppendLine()

                    Case "table-wrap"
                        Dim caption As XElement = child.Element(ns + "caption")
                        If caption IsNot Nothing Then
                            sb.AppendLine("[Table] " & CleanText(caption.Value))
                        End If
                        Dim tbl As XElement = child.Element(ns + "table")
                        If tbl IsNot Nothing Then
                            RenderTableToText(tbl, ns, sb)
                        End If

                    Case "fig"
                        Dim caption As XElement = child.Element(ns + "caption")
                        If caption IsNot Nothing Then
                            sb.AppendLine("[Figure] " & CleanText(caption.Value))
                            sb.AppendLine()
                        End If

                    Case "title"
                        ' 已在 sec 处理

                    Case Else
                        If child.HasElements Then
                            RenderSectionToText(child, ns, sb, level)
                        End If
                End Select
            Next
        End Sub


        Private Shared Sub RenderTableToText(tbl As XElement, ns As XNamespace, sb As StringBuilder)
            For Each row As XElement In tbl.Descendants(ns + "tr")
                Dim cells As New List(Of String)()
                For Each cell As XElement In row.Elements()
                    cells.Add(CleanText(cell.Value))
                Next
                sb.AppendLine("  | " & String.Join(" | ", cells) & " |")
            Next
            sb.AppendLine()
        End Sub


        ' ======================================================================
        '  HTTP 与速率限制
        ' ======================================================================

        Private Shared Async Function HttpGetAsync(url As String,
                                                   ct As CancellationToken) As Task(Of String)
            Await ThrottleAsync(ct)
            Using req As New HttpRequestMessage(HttpMethod.Get, url)
                Using resp As HttpResponseMessage = Await _client.SendAsync(req, ct)
                    Dim content As String = Await resp.Content.ReadAsStringAsync()
                    If Not resp.IsSuccessStatusCode Then
                        Throw New HttpRequestException(
                            $"HTTP {CInt(resp.StatusCode)} {resp.StatusCode}: {content.Substring(0, Math.Min(300, content.Length))}")
                    End If
                    Return content
                End Using
            End Using
        End Function


        ''' <summary>速率限制：确保两次请求间隔不小于 _minIntervalMs。</summary>
        Private Shared Async Function ThrottleAsync(ct As CancellationToken) As Task
            Await _gate.WaitAsync(ct)
            Try
                Dim elapsed As TimeSpan = DateTime.UtcNow - _lastRequestUtc
                Dim wait As Integer = _minIntervalMs - CInt(elapsed.TotalMilliseconds)
                If wait > 0 Then
                    Try
                        Await Task.Delay(wait, ct)
                    Catch ex As TaskCanceledException
                        ' 取消则直接返回
                        Return
                    End Try
                End If
                _lastRequestUtc = DateTime.UtcNow
            Finally
                _gate.Release()
            End Try
        End Function


        ' ======================================================================
        '  辅助方法
        ' ======================================================================

        Private Shared Sub AppendCommonParams(sb As StringBuilder)
            If Not String.IsNullOrEmpty(_toolName) Then sb.Append("&tool=").Append(UrlEncode(_toolName))
            If Not String.IsNullOrEmpty(_email) Then sb.Append("&email=").Append(UrlEncode(_email))
            If Not String.IsNullOrEmpty(_apiKey) Then sb.Append("&api_key=").Append(UrlEncode(_apiKey))
        End Sub

        Private Shared Function GetElementValue(parent As XElement, name As String) As String
            If parent Is Nothing Then Return ""
            Dim el As XElement = parent.Element(name)
            Return If(el?.Value, "")
        End Function

        ''' <summary>按本地名查找属性（忽略命名空间前缀，用于 xlink:href 等）。</summary>
        Private Shared Function FindAttributeByLocalName(el As XElement, localName As String) As XAttribute
            If el Is Nothing Then Return Nothing
            For Each a As XAttribute In el.Attributes()
                If a.Name.LocalName = localName Then Return a
            Next
            Return Nothing
        End Function

        Private Shared Function CleanText(s As String) As String
            If String.IsNullOrEmpty(s) Then Return ""
            ' 合并空白
            Dim t As String = Regex.Replace(s, "\s+", " ").Trim()
            Return t
        End Function

        Private Shared Function FormatPubDate(pubDateEl As XElement) As String
            If pubDateEl Is Nothing Then Return ""
            Dim y As String = GetElementValue(pubDateEl, "Year")
            Dim m As String = GetElementValue(pubDateEl, "Month")
            Dim d As String = GetElementValue(pubDateEl, "Day")
            Dim medline As String = GetElementValue(pubDateEl, "MedlineDate")
            If Not String.IsNullOrEmpty(y) Then
                If Not String.IsNullOrEmpty(m) AndAlso Not String.IsNullOrEmpty(d) Then
                    Return $"{y} {m} {d}"
                ElseIf Not String.IsNullOrEmpty(m) Then
                    Return $"{y} {m}"
                Else
                    Return y
                End If
            ElseIf Not String.IsNullOrEmpty(medline) Then
                Return medline
            End If
            Return ""
        End Function

        Private Shared Function BuildCitation(art As PubMedArticle) As String
            Dim sb As New StringBuilder()
            ' 作者
            If art.Authors.Count > 0 Then
                Dim authorStr As String
                If art.Authors.Count <= 6 Then
                    authorStr = String.Join(", ", art.Authors)
                Else
                    authorStr = String.Join(", ", art.Authors.Take(6)) & ", et al"
                End If
                sb.Append(authorStr).Append(". ")
            End If
            ' 标题
            If Not String.IsNullOrEmpty(art.Title) Then sb.Append(art.Title).Append(". ")
            ' 期刊
            If Not String.IsNullOrEmpty(art.JournalAbbrev) Then
                sb.Append(art.JournalAbbrev).Append(". ")
            ElseIf Not String.IsNullOrEmpty(art.Journal) Then
                sb.Append(art.Journal).Append(". ")
            End If
            ' 日期
            If Not String.IsNullOrEmpty(art.PubDate) Then sb.Append(art.PubDate).Append(";")
            ' 卷期页
            If Not String.IsNullOrEmpty(art.Volume) Then
                sb.Append(art.Volume)
                If Not String.IsNullOrEmpty(art.Issue) Then sb.Append("(").Append(art.Issue).Append(")")
                sb.Append(":")
            End If
            If Not String.IsNullOrEmpty(art.Pages) Then sb.Append(art.Pages).Append(".")
            ' DOI
            If Not String.IsNullOrEmpty(art.Doi) Then sb.Append(" doi: ").Append(art.Doi)
            ' PMID
            If Not String.IsNullOrEmpty(art.Pmid) Then sb.Append(" PMID: ").Append(art.Pmid)
            Return sb.ToString().Trim()
        End Function

        Private Shared Function UrlEncode(s As String) As String
            Return WebUtility.UrlEncode(s)
        End Function

        Private Shared Function HtmlEncode(s As String) As String
            Return WebUtility.HtmlEncode(s)
        End Function

        Private Shared Sub WritePmcToFile(article As PubMedArticle, outputDir As String)
            If Not Directory.Exists(outputDir) Then Directory.CreateDirectory(outputDir)
            Dim ext As String = If(article.PmcFullTextFormat = "html", "html", "txt")
            Dim safeTitle As String = MakeSafeFileName(article.Pmid & "_" & article.Title)
            If safeTitle.Length > 80 Then safeTitle = safeTitle.Substring(0, 80)
            Dim path As String = outputDir & "/" & $"PMC{article.PmcId}_{safeTitle}.{ext}"
            File.WriteAllText(path, article.PmcFullText, New UTF8Encoding(False))
            article.Messages.Add($"PMC 全文已保存: {path}")
        End Sub

        Private Shared Function MakeSafeFileName(s As String) As String
            Dim invalid As Char() = Path.GetInvalidFileNameChars()
            Dim sb As New StringBuilder()
            For Each c As Char In s
                If Array.IndexOf(invalid, c) >= 0 Then
                    sb.Append("_"c)
                Else
                    sb.Append(c)
                End If
            Next
            Return sb.ToString().Trim()
        End Function

    End Class

End Namespace
