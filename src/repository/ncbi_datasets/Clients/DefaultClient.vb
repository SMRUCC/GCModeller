' ============================================================================
' DefaultClient.vb
' 自动生成的 REST 客户端类 - 基于 OpenAPI 3.0.1 规范
' 分组标签: Default
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Text
Imports System.Threading.Tasks
Imports Newtonsoft.Json
Imports ncbi_datasets.Models
Imports ncbi_datasets.Infrastructure


Namespace ncbi_datasets.Clients

    ''' <summary>
    ''' Default 分组的 REST API 客户端。
    ''' API: NCBI Datasets API - ### NCBI Datasets is a resource that lets you easily gather data from NCBI. The NCBI Datasets version 2 API is updated often to add new features, fix bugs, and enhance usability.
    ''' </summary>
    Public Class DefaultClient

        Private ReadOnly _httpClient As ApiHttpClient
        Private ReadOnly _baseUrl As String

        ''' <summary>
        ''' 创建 DefaultClient 实例。
        ''' </summary>
        ''' <param name="baseUrl">API 基础地址</param>
        ''' <param name="httpClient">可选的自定义 HttpClient 实例</param>
        Public Sub New(baseUrl As String, Optional httpClient As HttpClient = Nothing)
            _baseUrl = baseUrl.TrimEnd("/"c)
            _httpClient = New ApiHttpClient(If(httpClient, New HttpClient()))
        End Sub

        ''' <summary>
        ''' Get a download summary (preview) of a genome data package by genome assembly accession
        ''' 
        ''' Get a download summary (preview) of a genome data package, including counts and file sizes, in JSON format.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GenomeDownloadSummary() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/genome/accession/{accessions}/download_summary"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a download summary (preview) of a genome data package by genome assembly accession
        ''' 
        ''' Get a downlaod summary (preview) of a genome data package, including counts and file sizes, in JSON format.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GenomeDownloadSummaryByPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/genome/download_summary"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a genome assembly report by genome assembly accession
        ''' 
        ''' Get a genome assembly report by assembly accession. By default, in paged JSON format, but also available in tabular (accept: text/tab-separated-values) or JSON Lines (accept: application/x-ndjson) formats.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GenomeDatasetReport() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/genome/accession/{accessions}/dataset_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a genome assembly report by taxon
        ''' 
        ''' Get a genome assembly report by taxon.  By default, in paged JSON format, but also available in tabular (accept: text/tab-separated-values) or JSON Lines (accept: application/x-ndjson) formats.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GenomeDatasetReportsByTaxon() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/genome/taxon/{taxons}/dataset_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get genome assembly reports by BioProject accession
        ''' 
        ''' Get genome assembly reports by BioProject accession.  By default, in paged JSON format, but also available in tabular (accept: text/tab-separated-values) or JSON Lines (accept: application/x-ndjson) formats.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GenomeDatasetReportsByBioproject() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/genome/bioproject/{bioprojects}/dataset_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get genome assembly reports by BioSample accession
        ''' 
        ''' Get genome assembly reports by BioSample accession.  By default, in paged JSON format, but also available in tabular (accept: text/tab-separated-values) or JSON Lines (accept: application/x-ndjson) formats.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GenomeDatasetReportsByBiosampleId() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/genome/biosample/{biosample_ids}/dataset_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a genome assembly data report by WGS accession
        ''' 
        ''' Get a genome assembly data report by WGS (whole genome shotgun) accession.  By default, in paged JSON format, but also available in tabular (accept: text/tab-separated-values) or JSON Lines (accept: application/x-ndjson) formats.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GenomeDatasetReportsByWgs() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/genome/wgs/{wgs_accessions}/dataset_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get genome assembly reports by assembly name
        ''' 
        ''' Get genome assembly reports by assembly name (exact matches only).  By default, in paged JSON format, but also available in tabular (accept: text/tab-separated-values) or JSON Lines (accept: application/x-ndjson) formats.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GenomeDatasetReportsByAssemblyName() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/genome/assembly_name/{assembly_names}/dataset_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a genome assembly report
        ''' 
        ''' Get a genome assembly report. By default, in paged JSON format, but also available in tabular (accept: text/tab-separated-values) or JSON Lines (accept: application/x-ndjson) formats.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GenomeDatasetReportByPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/genome/dataset_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a genome assembly accession for a nucleotide sequence accession
        ''' 
        ''' Get a genome assembly accession for a nucleotide sequence accession in JSON format.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function AssemblyAccessionsForSequenceAccession() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/genome/sequence_accession/{accession}/sequence_assemblies"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a genome assembly accession for a nucleotide sequence accession
        ''' 
        ''' Get a genome assembly accession for a nucleotide sequence accession in JSON format.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function AssemblyAccessionsForSequenceAccessionByPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/genome/sequence_assemblies"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a genome sequence report by genome assembly accession
        ''' 
        ''' Get a genome sequence report by genome assembly accession. By default, in paged JSON format, but also available in tabular (accept: text/tab-separated-values) or JSON Lines (accept: application/x-ndjson) formats.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GenomeSequenceReport() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/genome/accession/{accession}/sequence_reports"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a genome sequence report by genome assembly accession
        ''' 
        ''' Get a genome sequence report by genome assembly accession. By default, in paged JSON format, but also available in tabular (accept: text/tab-separated-values) or JSON Lines (accept: application/x-ndjson) formats.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GenomeSequenceReportByPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/genome/sequence_reports"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get assembly links by genome assembly accession
        ''' 
        ''' Get links to assembly resources by genome assembly accession in JSON format.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GenomeLinksByAccession() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/genome/accession/{accessions}/links"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get assembly links by genome assembly accession
        ''' 
        ''' Get links to assembly resources by genome assembly accession in JSON format.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GenomeLinksByAccessionByPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/genome/links"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get CheckM histogram data by species taxon
        ''' 
        ''' Get CheckM histogram data by species taxon. This data is used for rendering CheckM histograms on bacterial genome pages.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function CheckmHistogramByTaxon() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/genome/taxon/{species_taxon}/checkm_histogram"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get CheckM histogram data by species taxon
        ''' 
        ''' Get CheckM histogram data by species taxon. This data is used for rendering CheckM histograms on bacterial genome pages.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function CheckmHistogramByTaxonByPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/genome/checkm_histogram"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get BioSample dataset reports by accession(s)
        ''' 
        ''' Get BioSample dataset reports by accession(s).  By default, in paged JSON format, but also available as tabular (accept: text/tab-separated-values) or json-lines (accept: application/x-ndjson)
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function BiosampleDatasetReport() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/biosample/accession/{accessions}/biosample_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a genome data package by genome assembly accession
        ''' 
        ''' Download a genome data package including sequence, annotation, and a detailed data report by genome assembly accession.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function DownloadAssemblyPackage() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/genome/accession/{accessions}/download"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a genome data package by genome assembly accession
        ''' 
        ''' Download a genome data package including sequence, annotation, and a detailed data report by genome assembly accession.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function DownloadAssemblyPackagePost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/genome/download"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a gene data package by GeneID
        ''' 
        ''' Download a gene data package including sequence, annotation and data reports, as a compressed zip archive, by GeneID.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function DownloadGenePackage() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/gene/id/{gene_ids}/download"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a gene data package
        ''' 
        ''' Download a gene data package including sequence, annotation and data reports, as a compressed zip archive.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function DownloadGenePackagePost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/gene/download"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a prokaryote gene data package by RefSeq protein accession
        ''' 
        ''' Download a prokaryote gene data package including sequence, annotation and data reports by RefSeq non-redundant protein accession.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function DownloadProkaryoteGenePackage() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/protein/accession/{accessions}/download"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a prokaryote gene data package by RefSeq protein accession
        ''' 
        ''' Download a prokaryote gene data package including sequence, annotation and data reports by RefSeq non-redundant protein accession.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function DownloadProkaryoteGenePackagePost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/protein/accession/download"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a genome annotation data package by genome assembly accession
        ''' 
        ''' Download an annotation data package including sequence and a detailed annotation report by genome assembly accession.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function DownloadGenomeAnnotationPackage() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/genome/accession/{accession}/annotation_report/download"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a genome annotation data package by genome assembly accession
        ''' 
        ''' Download an annotation data package including sequence and a detailed annotation report by genome assembly accession.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function DownloadGenomeAnnotationPackageByPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/genome/annotation_report/download"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a taxonomy data package by Taxonomy ID
        ''' 
        ''' Download a taxonomy data package, including taxonomy and names reports, as a compressed zip archive.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function DownloadTaxonomyPackage() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/taxonomy/taxon/{tax_ids}/download"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a taxonomy data package by Taxonomy ID
        ''' 
        ''' Download a taxonomy data package, including taxonomy and names reports, as a compressed zip archive.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function DownloadTaxonomyPackageByPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/taxonomy/download"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Check the validity of a genome assembly accession
        ''' 
        ''' Check the validity of a genome assembly accession. Output in JSON format.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function CheckAssemblyAvailability() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/genome/accession/{accessions}/check"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Check the validity of a genome assembly accession
        ''' 
        ''' Check the validity of a genome assembly accession. Output in JSON format.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function CheckAssemblyAvailabilityPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/genome/check"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get an organelle data package by nucleotide accession
        ''' 
        ''' Download an organelle data package including sequence, annotation, and detailed data reports as a compressed zip archive.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function DownloadOrganellePackage() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/organelle/accession/{accessions}/download"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get an organelle data package
        ''' 
        ''' Download an organelle data package including sequence, annotation, and detailed data reports as a compressed zip archive.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function DownloadOrganellePackageByPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/organelle/download"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Retrieve service version
        ''' 
        ''' Retrieve the latest version of the Datasets services.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function Version() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/version"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get gene reports by GeneID
        ''' 
        ''' Get a gene summary by GeneID. By default, in paged JSON format, but also available as tabular (accept: text/tab-separated-values) or json-lines (accept: application/x-ndjson)
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        <Obsolete("此 API 操作已弃用。")>
        Public Async Function GeneReportsById() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/gene/id/{gene_ids}"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get gene metadata by RefSeq Accession
        ''' 
        ''' Get a gene summary by RefSeq Accession. By default, in paged JSON format, but also available as tabular (accept: text/tab-separated-values) or json-lines (accept: application/x-ndjson)
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        <Obsolete("此 API 操作已弃用。")>
        Public Async Function GeneMetadataByAccession() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/gene/accession/{accessions}"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get gene metadata by gene symbol
        ''' 
        ''' Get a gene summary by by gene symbol. By default, in paged JSON format, but also available as tabular (accept: text/tab-separated-values) or json-lines (accept: application/x-ndjson)
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        <Obsolete("此 API 操作已弃用。")>
        Public Async Function GeneMetadataByTaxAndSymbol() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/gene/symbol/{symbols}/taxon/{taxon}"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get gene reports by taxonomic identifier
        ''' 
        ''' Get a gene summary for a specified NCBI Taxonomy ID or name (common or scientific). By default, in paged JSON format, but also available as tabular (accept: text/tab-separated-values) or json-lines (accept: application/x-ndjson)
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        <Obsolete("此 API 操作已弃用。")>
        Public Async Function GeneReportsByTaxon() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/gene/taxon/{taxon}"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get gene metadata as JSON
        ''' 
        ''' Get a gene summary. By default, in paged JSON format, but also available as tabular (accept: text/tab-separated-values) or json-lines (accept: application/x-ndjson)
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        <Obsolete("此 API 操作已弃用。")>
        Public Async Function GeneMetadataByPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/gene"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a gene data report by GeneID
        ''' 
        ''' Get a gene data report by GeneID.  By default, in paged JSON format, but also available in tabular (accept: text/tab-separated-values) or JSON Lines (accept: application/x-ndjson) formats.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GeneDatasetReportsById() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/gene/id/{gene_ids}/dataset_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a gene data report by RefSeq nucleotide or protein accession
        ''' 
        ''' Get a gene data report by RefSeq nucleotide or protein accession. By default, in paged JSON format, but also available in tabular (accept: text/tab-separated-values) or JSON Lines (accept: application/x-ndjson) formats.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GeneDatasetReportByAccession() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/gene/accession/{accessions}/dataset_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a gene data report by symbol and taxon
        ''' 
        ''' Get a gene data report by gene symbol and taxon. By default, in paged JSON format, but also available in tabular (accept: text/tab-separated-values) or JSON Lines (accept: application/x-ndjson) formats.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GeneDatasetReportByTaxAndSymbol() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/gene/symbol/{symbols}/taxon/{taxon}/dataset_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a gene data report by taxon
        ''' 
        ''' Get a gene data report by taxon. By default, in paged JSON format, but also available in tabular (accept: text/tab-separated-values) or JSON Lines (accept: application/x-ndjson) formats.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GeneDatasetReportsByTaxon() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/gene/taxon/{taxon}/dataset_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a gene data report by locus tag
        ''' 
        ''' Get a gene data report by gene locus tag. By default, in paged JSON format, but also available in tabular (accept: text/tab-separated-values) or JSON Lines (accept: application/x-ndjson) formats.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GeneDatasetReportsByLocusTag() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/gene/locus_tag/{locus_tags}/dataset_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a gene data report
        ''' 
        ''' Get a gene data report. By default, in paged JSON format, but also available in tabular (accept: text/tab-separated-values) or JSON Lines (accept: application/x-ndjson) formats.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GeneDatasetReport(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/gene/dataset_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a gene product report by GeneID
        ''' 
        ''' Get a gene product report by GeneID. By default, in paged JSON format, but also available in tabular (accept: text/tab-separated-values) or JSON Lines (accept: application/x-ndjson) formats.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GeneProductReportsById() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/gene/id/{gene_ids}/product_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a gene product report by RefSeq nucleotide or protein accession
        ''' 
        ''' Get a gene product report by RefSeq nucleotide or protein accession. By default, in paged JSON format, but also available in tabular (accept: text/tab-separated-values) or JSON Lines (accept: application/x-ndjson) formats.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GeneProductReportByAccession() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/gene/accession/{accessions}/product_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a gene product report by symbol and taxon
        ''' 
        ''' Get a gene product report by symbol and taxon. By default, in paged JSON format, but also available in tabular (accept: text/tab-separated-values) or JSON Lines (accept: application/x-ndjson) formats.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GeneProductReportByTaxAndSymbol() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/gene/symbol/{symbols}/taxon/{taxon}/product_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a gene product report by taxon
        ''' 
        ''' Get a gene product report by taxon. By default, in paged JSON format, but also available in tabular (accept: text/tab-separated-values) or JSON Lines (accept: application/x-ndjson) formats.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GeneProductReportsByTaxon() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/gene/taxon/{taxon}/product_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a gene product report by locus tag
        ''' 
        ''' Get a gene product report by gene locus tag. By default, in paged JSON format, but also available in tabular (accept: text/tab-separated-values) or JSON Lines (accept: application/x-ndjson) formats.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GeneProductReportsByLocusTags() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/gene/locus_tag/{locus_tags}/product_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a gene product report
        ''' 
        ''' Get a gene product report. By default, in paged JSON format, but also available in tabular (accept: text/tab-separated-values) or JSON Lines (accept: application/x-ndjson) formats.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GeneProductReport(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/gene/product_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a download summary of a gene data package by GeneID
        ''' 
        ''' Get a download summary of a gene data package, including counts and estimated package size, in JSON format.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GeneDownloadSummaryById() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/gene/id/{gene_ids}/download_summary"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a download summary of a gene data package
        ''' 
        ''' Get a download summary of a gene data package, including counts and estimated package size, in JSON format.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GeneDownloadSummaryByPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/gene/download_summary"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get gene counts by taxon
        ''' 
        ''' Get gene counts by taxon in JSON format.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GeneCountsForTaxon() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/gene/taxon/{taxon}/counts"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get gene counts by taxon
        ''' 
        ''' Get gene counts by taxon in JSON format.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GeneCountsForTaxonByPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/gene/taxon/counts"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a gene data report for a gene ortholog set by GeneID
        ''' 
        ''' Get a gene data report for a gene ortholog set by GeneID in JSON format.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GeneOrthologsById() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/gene/id/{gene_id}/orthologs"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a gene data report for a gene ortholog set by GeneID
        ''' 
        ''' Get a gene data report for a gene ortholog set by GeneID in JSON format.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GeneOrthologsByPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/gene/orthologs"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get gene links by GeneID
        ''' 
        ''' Get links to available gene resources in JSON format.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GeneLinksById() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/gene/id/{gene_ids}/links"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get gene links by GeneID
        ''' 
        ''' Get links to available gene resources in JSON format.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GeneLinksByIdByPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/gene/links"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get gene counts per chromosome by taxon and annotation name
        ''' 
        ''' Get gene counts per chromosome by taxon and annotation name in JSON format.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GeneChromosomeSummary() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/gene/taxon/{taxon}/annotation/{annotation_name}/chromosome_summary"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get genome annotation reports by genome assembly accession
        ''' 
        ''' Get genome annotation reports by genome assembly accession, where each report represents a single feature annotated on the genome.  By default, in paged JSON format, but also available in tabular (accept: text/tab-separated-values) or JSON Lines (accept: application/x-ndjson) formats.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GenomeAnnotationReport() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/genome/accession/{accession}/annotation_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get genome annotation reports by genome assembly accession
        ''' 
        ''' Get genome annotation reports by genome assembly accession, where each report represents a single feature annotated on the genome.  By default, in paged JSON format, but also available in tabular (accept: text/tab-separated-values) or JSON Lines (accept: application/x-ndjson) formats.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GenomeAnnotationReportByPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/genome/annotation_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get genome annotation report summary information by genome assembly accession
        ''' 
        ''' Get genome annotation report summary information by genome assembly accession in JSON format, including chromosome names and gene types.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function AnnotationReportFacetsByAccession() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/genome/accession/{accession}/annotation_summary"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get genome annotation report summary information by genome assembly accession
        ''' 
        ''' Get genome annotation report summary information by genome assembly accession in JSON format, including chromosome names and gene types.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function AnnotationReportFacetsByPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/genome/annotation_summary"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a download summary (preview) of a genome annotation data package by genome assembly accession
        ''' 
        ''' Get a downlaod summary (preview) of a genome annotation data package, including counts and file sizes, in JSON format.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GenomeAnnotationDownloadSummary() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/genome/accession/{accession}/annotation_report/download_summary"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a download summary (preview) of a genome annotation data package by genome assembly accession
        ''' 
        ''' Get a download summary (preview) of a genome annotation data package, including counts and file sizes, by genome assembly accession, in JSON format.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function GenomeAnnotationDownloadSummaryByPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/genome/annotation_report/download_summary"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get an organelle data report by nucleotide accession
        ''' 
        ''' Get an organelle data report in JSON format.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function OrganelleDatareportByAccession() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/organelle/accessions/{accessions}/dataset_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get an organelle data report by taxon
        ''' 
        ''' Get an organelle data report  in JSON format.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function OrganelleDatareportByTaxon() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/organelle/taxon/{taxons}/dataset_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get an organelle data report
        ''' 
        ''' Get an organelle data report in JSON format.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function OrganelleDatareportByPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/organelle/dataset_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Use taxonomic identifiers to get taxonomic metadata
        ''' 
        ''' Using NCBI Taxonomy IDs or names (common or scientific) at any rank, get metadata about a taxonomic node including taxonomic identifiers, lineage information, child nodes, and gene and genome counts in JSON format.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        <Obsolete("此 API 操作已弃用。")>
        Public Async Function TaxonomyMetadata() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/taxonomy/taxon/{taxons}"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get taxonomy metadata by taxon
        ''' 
        ''' Get taxonomy metadata by taxon in JSON format.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        <Obsolete("此 API 操作已弃用。")>
        Public Async Function TaxonomyMetadataPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/taxonomy"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a taxonomy data report by taxon
        ''' 
        ''' Get a taxonomy data report by taxon. By default, in paged JSON format, but also available in tabular (accept: text/tab-separated-values) or JSON Lines (accept: application/x-ndjson) formats.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function TaxonomyDataReport() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/taxonomy/taxon/{taxons}/dataset_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a taxonomy data report by taxon
        ''' 
        ''' Get a taxonomy data report by taxon. By default, in paged JSON format, but also available in tabular (accept: text/tab-separated-values) or JSON Lines (accept: application/x-ndjson) formats.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function TaxonomyDataReportPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/taxonomy/dataset_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a taxonomy names report by taxon
        ''' 
        ''' Get a taxonomy names report, including common names and other synonyms, in JSON format.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function TaxonomyNames() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/taxonomy/taxon/{taxons}/name_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a taxonomy names report by taxon
        ''' 
        ''' Get a taxonomy names report, including common names and other synonyms, in JSON format.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function TaxonomyNamesPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/taxonomy/name_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get child nodes, and optionally parent nodes, for a given taxon by Taxonomy ID
        ''' 
        ''' Get child nodes, and optionally parent nodes, in JSON format.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function TaxonomyRelatedIds() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/taxonomy/taxon/{tax_id}/related_ids"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get child nodes, and optionally parent nodes, for a given taxon by Taxonomy ID
        ''' 
        ''' Get child nodes, and optionally parent nodes, in JSON format.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function TaxonomyRelatedIdsPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/taxonomy/related_ids"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a filtered taxonomic subtree by taxon
        ''' 
        ''' Get a filtered taxonomic subtree, including parent and child nodes, in JSON format.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function TaxonomyFilteredSubtree() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/taxonomy/taxon/{taxons}/filtered_subtree"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a filtered taxonomic subtree by taxon
        ''' 
        ''' Get a filtered taxonomic subtree, including parent and child nodes, in JSON format.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function TaxonomyFilteredSubtreePost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/taxonomy/filtered_subtree"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a list of taxonomy names and IDs by partial taxonomic name
        ''' 
        ''' Get a list of taxonomy names and IDs in JSON format.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function TaxNameQuery() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/taxonomy/taxon_suggest/{taxon_query}"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a list of taxonomy names and IDs by partial taxonomic name
        ''' 
        ''' Get a list of taxonomy names and IDs in JSON format.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function TaxNameQueryByPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/taxonomy/taxon_suggest"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get external links by Taxonomy ID
        ''' 
        ''' Get external links associated with a given taxon in JSON format.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function TaxonomyLinks() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/taxonomy/taxon/{taxon}/links"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get external links by Taxonomy ID
        ''' 
        ''' Get external links associated with a given taxon in JSON format.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function TaxonomyLinksByPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/taxonomy/links"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a taxonomy image by taxon
        ''' 
        ''' Get an image associated with the specified taxon. By default, in JPEG format, but also available in PNG (accept: image/png), TIFF (accept: accept: image/tiff), and SVG+XML (accept: image/svg+xml) formats.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function TaxonomyImage() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/taxonomy/taxon/{taxon}/image"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a taxonomy image by taxon
        ''' 
        ''' Get an image associated with the specified taxon. By default, in JPEG format, but also available in PNG (accept: image/png), TIFF (accept: accept: image/tiff), and SVG+XML (accept: image/svg+xml) formats.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function TaxonomyImagePost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/taxonomy/image"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get taxonomy image metadata by Taxonomy ID
        ''' 
        ''' Get taxonomy image metadata, including the image URL and license information, in JSON format.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function TaxonomyImageMetadata() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/taxonomy/taxon/{taxon}/image/metadata"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get taxonomy image metadata by Taxonomy ID
        ''' 
        ''' Get taxonomy image metadata, including the image URL and license information, in JSON format.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function TaxonomyImageMetadataPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/taxonomy/image/metadata"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a download summary of a virus genome data package by taxon
        ''' 
        ''' Get a download summary of a virus genome data package, including counts and file sizes, in JSON format.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function VirusGenomeSummary() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/virus/taxon/{taxon}/genome"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a download summary of a virus genome data package
        ''' 
        ''' Get a download summary of a virus genome data package, including counts and file sizes, in JSON format.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function VirusGenomeSummaryByPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/virus/genome"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a download summary of a SARS-CoV-2 protein data package by protein name
        ''' 
        ''' Get a download summary of a SARS-CoV-2 protein data package, including counts and file sizes, in JSON format.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function Sars2ProteinSummary() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/virus/taxon/sars2/protein/{proteins}"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a download summary of a SARS-CoV-2 protein data package by protein name
        ''' 
        ''' Get a download summary of a SARS-CoV-2 protein data package, including counts and file sizes, in JSON format.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function Sars2ProteinSummaryByPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/virus/taxon/sars2/protein"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get virus genome metadata in a tabular format
        ''' 
        ''' Get virus genome metadata in tabular format for virus genomes by taxon.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        <Obsolete("此 API 操作已弃用。")>
        Public Async Function VirusGenomeTable() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/virus/taxon/{taxon}/genome/table"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get SARS-CoV-2 protein metadata in a tabular format by protein name
        ''' 
        ''' Get SARS-CoV-2 protein metadata in a tabular format by protein name.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function Sars2ProteinTable() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/virus/taxon/sars2/protein/{proteins}/table"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a virus data report by taxon
        ''' 
        ''' Get a virus data report by taxon. By default, in paged JSON format, but also available in tabular (accept: text/tab-separated-values) or JSON Lines (accept: application/x-ndjson) formats.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function VirusReportsByTaxon() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/virus/taxon/{taxon}/dataset_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a virus data report by nucleotide accession
        ''' 
        ''' Get a virus data report by nucleotide accession. By default, in paged JSON format, but also available in tabular (accept: text/tab-separated-values) or JSON Lines (accept: application/x-ndjson) formats.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function VirusReportsByAcessions() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/virus/accession/{accessions}/dataset_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a virus data report
        ''' 
        ''' Get a virus data report. By default, in paged JSON format, but also available in tabular (accept: text/tab-separated-values) or JSON Lines (accept: application/x-ndjson) formats.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function VirusReportsByPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/virus"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a virus annotation report by taxon
        ''' 
        ''' Get virus annotation report by taxon. By default, in paged JSON format, but also available in tabular (accept: text/tab-separated-values) or JSON Lines (accept: application/x-ndjson) formats.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function VirusAnnotationReportsByTaxon() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/virus/taxon/{taxon}/annotation_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a virus annotation report by nucleotide accession
        ''' 
        ''' Get a virus annotation report by nucleotide accesion. By default, in paged JSON format, but also available in tabular (accept: text/tab-separated-values) or JSON Lines (accept: application/x-ndjson) formats.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function VirusAnnotationReportsByAcessions() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/virus/accession/{accessions}/annotation_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a virus annotation report
        ''' 
        ''' Get a virus annotation report. By default, in paged JSON format, but also available in tabular (accept: text/tab-separated-values) or JSON Lines (accept: application/x-ndjson) formats.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function VirusAnnotationReportsByPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/virus/annotation_report"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Check the validity of a virus genome nucleotide accession
        ''' 
        ''' Check the validity of a virus genome nucleotide accession. Output in JSON format.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function VirusAccessionAvailability() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/virus/accession/{accessions}/check"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Check the validity of a virus genome nucleotide accession
        ''' 
        ''' Check the validity of a virus genome nucleotide accession. Output in JSON format.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function VirusAccessionAvailabilityPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/virus/check"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a virus genome data package by taxon
        ''' 
        ''' Download a virus genome data package including sequence, annotation, BioSample data and a detailed data report by taxon.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function VirusGenomeDownload() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/virus/taxon/{taxon}/genome/download"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a virus genome data package by nucleotide accession
        ''' 
        ''' Download a virus genome data package including sequence, annotation, BioSample data and a detailed data report by nucleotide accession.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function VirusGenomeDownloadAccession() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/virus/accession/{accessions}/genome/download"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a virus genome data package
        ''' 
        ''' Download a virus genome data package including sequence, annotation, BioSample data and a detailed data report by nucleotide accession.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function VirusGenomeDownloadPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/virus/genome/download"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a SARS-CoV-2 protein data package by protein name
        ''' 
        ''' Download a SARS-CoV-2 protein data package including sequence, annotation, BioSample data and a detailed data report by protein name.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function Sars2ProteinDownload() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/virus/taxon/sars2/protein/{proteins}/download"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a SARS-CoV-2 protein data package
        ''' 
        ''' Download a SARS-CoV-2 protein data package including sequence, annotation, BioSample data and a detailed data report.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function Sars2ProteinDownloadPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/virus/taxon/sars2/protein/download"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a revision history for a genome assembly by genome assembly accession
        ''' 
        ''' Get a revision history, or list of all versions of a genome assembly, in JSON format.
        ''' </summary>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function AssemblyRevisionHistoryByGet() As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/genome/accession/{accession}/revision_history"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Get, url)

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

        ''' <summary>
        ''' Get a revision history for a genome assembly by genome assembly accession
        ''' 
        ''' Get a revision history, or list of all versions of a genome assembly, in JSON format.
        ''' </summary>
        ''' <param name="body">请求体</param>
        ''' <returns>异步任务，返回 Object 类型的结果</returns>
        Public Async Function AssemblyRevisionHistoryByPost(body As Object) As Task(Of Object)

            ' 构建 URL 路径
            Dim url As String = $"{_baseUrl}/genome/revision_history"

            ' 创建 HTTP 请求
            Dim request As New HttpRequestMessage(HttpMethod.Post, url)

            ' 设置请求体
            Dim jsonBody As String = JsonConvert.SerializeObject(body)
            request.Content = New StringContent(jsonBody, Encoding.UTF8, "application/json")

            ' 发送请求并处理响应
            Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request)

            ' 检查响应状态
            If Not response.IsSuccessStatusCode Then
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Throw New ApiException("API 请求失败: " & response.StatusCode.ToString() & " - " & errorContent, response.StatusCode)
            End If

            ' 反序列化响应内容
            Dim responseContent As String = Await response.Content.ReadAsStringAsync()
            Return JsonConvert.DeserializeObject(Of Object)(responseContent)
        End Function

    End Class

End Namespace
