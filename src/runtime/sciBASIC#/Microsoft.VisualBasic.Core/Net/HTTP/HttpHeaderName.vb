﻿
Imports System.ComponentModel
Imports System.Net

Namespace Net.Http

    ''' <summary>
    ''' 可在服务器响应中指定的 HTTP 标头。
    ''' </summary>
    ''' <remarks>
    ''' inherits from <see cref="HttpResponseHeader"/>
    ''' </remarks>
    Public Enum HttpHeaderName As Integer

        Unknown = -1

#Region "HttpResponseHeader"

        '
        ' 摘要:
        '     Cache-Control 标头，指定请求/响应链上所有缓存机制必须服从的缓存指令。
        CacheControl = 0
        '
        ' 摘要:
        '     Connection 标头，指定特定连接所需的选项。
        Connection = 1
        '
        ' 摘要:
        '     Date 标头，指定响应产生的日期和时间。
        [Date] = 2
        '
        ' 摘要:
        '     Keep-Alive 标头，指定用于维护持久连接的参数。
        KeepAlive = 3
        '
        ' 摘要:
        '     Pragma 标头，指定特定于实现的指令，这些指令可应用到请求/响应链上的任意代理。
        Pragma = 4
        '
        ' 摘要:
        '     Trailer 标头，指定指示的标头字段在消息（使用分块传输编码方法进行编码）的尾部显示。
        Trailer = 5
        '
        ' 摘要:
        '     Transfer-Encoding 标头，指定对消息正文应用哪种类型的转换（如果有）。
        TransferEncoding = 6
        '
        ' 摘要:
        '     Upgrade 标头，指定客户端支持的其他通信协议。
        Upgrade = 7
        '
        ' 摘要:
        '     Via 标头，指定网关和代理要使用的中间协议。
        Via = 8
        '
        ' 摘要:
        '     Warning 标头，指定消息中可能不会反映的有关消息的状态或转换的其他信息。
        Warning = 9
        '
        ' 摘要:
        '     Allow 标头，指定支持的 HTTP 方法集。
        Allow = 10
        '
        ' 摘要:
        '     Content-Length 标头，指定随附的正文数据的长度（以字节为单位）。
        ContentLength = 11
        '
        ' 摘要:
        '     Content-Type 标头，指定随附的正文数据的 MIME 类型。
        ContentType = 12
        '
        ' 摘要:
        '     Content-Encoding 标头，指定应用到随附的正文数据的编码。
        ContentEncoding = 13
        '
        ' 摘要:
        '     Content-Langauge 标头，指定自然语言或伴随正文数据的语言。
        ContentLanguage = 14
        '
        ' 摘要:
        '     Content-Location 标头，指定可以从中获取伴随正文的 URI。
        ContentLocation = 15
        '
        ' 摘要:
        '     Content-MD5 标头，指定随附的正文数据的 MD5 摘要，以便提供端到端消息完整性检查。
        ContentMd5 = 16
        '
        ' 摘要:
        '     Range 标头，指定客户端请求返回的响应的单个或多个子范围来代替整个响应。
        ContentRange = 17
        '
        ' 摘要:
        '     Expires 标头，指定日期和时间，在该日期和时间之后随附的正文数据将被视为已过期。
        Expires = 18
        '
        ' 摘要:
        '     Last-Modified 标头，指定上次修改随附的正文数据的日期和时间。
        LastModified = 19
        '
        ' 摘要:
        '     Accept-Ranges 标头，指定服务器接受的范围。
        AcceptRanges = 20
        '
        ' 摘要:
        '     Age 标头，指定自起始服务器生成响应以来的时间长度（以秒为单位）。
        Age = 21
        '
        ' 摘要:
        '     Etag 标头，指定请求的变量的当前值。
        ETag = 22
        '
        ' 摘要:
        '     Location 标头，指定为获取请求的资源而将客户端重定向到的 URI。
        Location = 23
        '
        ' 摘要:
        '     Proxy-Authenticate 标头，指定客户端必须对代理验证其自身。
        ProxyAuthenticate = 24
        '
        ' 摘要:
        '     Retry-After 标头，指定某个时间（以秒为单位）或日期和时间，在此时间之后客户端可以重试其请求。
        RetryAfter = 25
        '
        ' 摘要:
        '     Server 标头，指定关于起始服务器代理的信息。
        Server = 26
        '
        ' 摘要:
        '     Set-Cookie 标头，指定提供给客户端的 Cookie 数据。
        SetCookie = 27
        '
        ' 摘要:
        '     Vary 标头，指定用于确定缓存的响应是否为新响应的请求标头。
        Vary = 28
        '
        ' 摘要:
        '     WWW-Authenticate 标头，指定客户端必须对服务器验证其自身。
        WwwAuthenticate = 29
#End Region

        <Description("X-Frame-Options")> XFrameOptions
        <Description("X-Powered-By")> XPoweredBy

    End Enum

    <HideModuleName>
    Public Module HttpHeaderNameExtensions

        ReadOnly strMaps As Dictionary(Of String, HttpHeaderName)

        Sub New()
            strMaps = New Dictionary(Of String, HttpHeaderName)

            For Each val As HttpHeaderName In Enums(Of HttpHeaderName)()
                strMaps(val.Description) = val
            Next
        End Sub

        Public Function ParseHeaderName(key As String) As HttpHeaderName
            Dim nameKey As String = key.Replace("-", "")
            Dim val As HttpHeaderName = Nothing

            If [Enum].TryParse(Of HttpHeaderName)(nameKey, val) Then
                Return val
            End If

            If strMaps.ContainsKey(key) Then
                Return strMaps(key)
            Else
                Return HttpHeaderName.Unknown
            End If
        End Function
    End Module
End Namespace