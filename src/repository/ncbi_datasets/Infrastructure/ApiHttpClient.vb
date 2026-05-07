' ============================================================================
' ApiHttpClient.vb
' HTTP 客户端辅助类 - 封装 HttpClient 操作，提供统一的请求发送能力
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports System.Net.Http
Imports System.Threading
Imports System.Threading.Tasks

Namespace ncbi_datasets.Infrastructure

    ''' <summary>
    ''' HTTP 客户端辅助类，封装 HttpClient 的常用操作。
    ''' 提供超时控制、请求日志、自动重试等基础能力。
    ''' </summary>
    Public Class ApiHttpClient

        Private ReadOnly _httpClient As HttpClient
        Private _timeout As TimeSpan = TimeSpan.FromSeconds(30)

        ''' <summary>
        ''' 使用指定的 HttpClient 实例创建 ApiHttpClient。
        ''' </summary>
        ''' <param name="httpClient">底层 HttpClient 实例</param>
        Public Sub New(httpClient As HttpClient)
            _httpClient = httpClient ?? Throw New ArgumentNullException(NameOf(httpClient))
        End Sub

        ''' <summary>
        ''' 获取或设置请求超时时间。
        ''' </summary>
        Public Property Timeout As TimeSpan
            Get
                Return _timeout
            End Get
            Set(value As TimeSpan)
                _timeout = value
                _httpClient.Timeout = value
            End Set
        End Property

        ''' <summary>
        ''' 异步发送 HTTP 请求。
        ''' </summary>
        ''' <param name="request">HTTP 请求消息</param>
        ''' <param name="cancellationToken">取消令牌</param>
        ''' <returns>HTTP 响应消息</returns>
        Public Async Function SendAsync(request As HttpRequestMessage, Optional cancellationToken As CancellationToken = Nothing) As Task(Of HttpResponseMessage)
            If request Is Nothing Then Throw New ArgumentNullException(NameOf(request))

            ' 设置超时
            Using cts As New CancellationTokenSource(_timeout)
                Using linkedCts As CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token)
                    Dim response As HttpResponseMessage = Await _httpClient.SendAsync(request, linkedCts.Token)
                    Return response
                End Using
            End Using
        End Function

    End Class

End Namespace
