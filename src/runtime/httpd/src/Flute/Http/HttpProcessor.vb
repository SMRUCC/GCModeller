#Region "Microsoft.VisualBasic::27022bd464373733c5e1d90e7956731e, src\Flute\Http\HttpProcessor.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 529
    '    Code Lines: 306 (57.84%)
    ' Comment Lines: 140 (26.47%)
    '    - Xml Docs: 55.71%
    ' 
    '   Blank Lines: 83 (15.69%)
    '     File Size: 20.08 KB


    '     Class HttpProcessor
    ' 
    '         Properties: http_method, http_protocol_versionstring, http_url, httpHeaders, IsWWWRoot
    '                     Out, raw
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: flushPOSTPayload, GetSettings, openResponseStream, parseRequest, processHttpRequest
    '                   streamReadLine, ToString
    ' 
    '         Sub: (+2 Overloads) Dispose, handleGETRequest, HandlePOSTRequest, Process, readHeaders
    '              WriteData, writeFailure, writeFailureInternal, WriteLine, (+3 Overloads) writeSuccess
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Net.Sockets
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Threading
Imports Flute.Http.Configurations
Imports Flute.Http.Core.HttpOptions
Imports Flute.Http.Core.Message
Imports Flute.Http.Core.WebSocket
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports ASCII = Microsoft.VisualBasic.Text.ASCII
Imports RequestHeaders = Flute.Http.Core.Message.HttpHeader.RequestHeaders
Imports ResponseHeaders = Flute.Http.Core.Message.HttpHeader.ResponseHeaders
Imports WebSocketProtocol = Flute.Http.Core.Message.HttpHeader.WebSocketProtocol
Imports std = System.Math

' offered to the public domain for any use with no restriction
' and also with no warranty of any kind, please enjoy. - David Jeske. 

' simple HTTP explanation
' http://www.jmarshall.com/easy/http/

Namespace Core

    ''' <summary>
    ''' 这个对象包含有具体的http request的处理方法
    ''' </summary>
    Public Class HttpProcessor : Implements IDisposable

        Public socket As TcpClient
        Public srv As HttpServer

        Dim _inputStream As Stream
        Dim _raw As New StringBuilder

        Friend ReadOnly _settings As Configuration

        Public outputStream As StreamWriter

        ''' <summary>
        ''' has current tcp connection been taken over by the websocket protocol?
        ''' </summary>
        ''' <remarks>
        ''' the whole lifecycle of the underlying tcp connection is managed by the
        ''' <see cref="WebSocketConnection"/> object after the RFC6455 handshake has
        ''' been completed, so the http response stream flush/close operation and
        ''' the socket close operation inside the <see cref="Process"/> method must
        ''' be skipped when this flag is true, otherwise the websocket data frame
        ''' will be written onto a stream which has already been closed.
        ''' </remarks>
        Private m_webSocketHijacked As Boolean = False

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' http方法名是大写的
        ''' </remarks>
        Public Property http_method As String

        ''' <summary>
        ''' returns the raw http request header
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property raw As String
            Get
                Return _raw.ToString
            End Get
        End Property

        ''' <summary>
        ''' File location or GET/POST request arguments
        ''' </summary>
        ''' <returns></returns>
        Public Property http_url As String
        Public Property http_protocol_versionstring As String
        Public Property httpHeaders As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

        ''' <summary>
        ''' 可以向这里面写入数据从而回传数据
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Out As Stream
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return outputStream.BaseStream
            End Get
        End Property

        ''' <summary>
        ''' default maximum POST body size: 16MB
        ''' a negative value (e.g. -1) means no limit.
        ''' </summary>
        ''' <remarks></remarks>
        ReadOnly MAX_POST_SIZE% = 16 * 1024 * 1024

        ''' <summary>
        ''' If current request url is indicates the HTTP root:  index.html
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsWWWRoot As Boolean
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return String.Equals("/", http_url)
            End Get
        End Property

        Public Sub New(socket As TcpClient, srv As HttpServer, MAX_POST_SIZE%, settings As Configuration)
            Me.socket = socket
            Me.srv = srv
            ' a negative incoming value (e.g. -1) keeps the default 16MB limit disabled
            Me.MAX_POST_SIZE = If(MAX_POST_SIZE > 0, MAX_POST_SIZE, Me.MAX_POST_SIZE)
            Me._settings = settings
        End Sub

        Public Function GetSettings() As Configuration
            Return _settings
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub WriteData(data As Byte())
            Call outputStream.BaseStream.Write(data, Scan0, data.Length)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub WriteLine(s As String)
            Call outputStream.WriteLine(s)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return http_url
        End Function

        Public Function openResponseStream() As HttpResponse
            Dim response As New HttpResponse(outputStream, AddressOf writeFailure, _settings)
            response.m_requestHeaders = httpHeaders
            Return response
        End Function

        ''' <summary>
        ''' each stream line is end with ``cr + lf``. 
        ''' </summary>
        ''' <param name="inputStream"></param>
        ''' <returns></returns>
        Private Function streamReadLine(inputStream As Stream) As String
            ' read one byte at a time until LF or end-of-stream.
            ' BufferedStream wrapping the socket stream already coalesces
            ' underlying IO, so each ReadByte() is a fast in-memory read.
            Dim nextChar As Integer
            Dim chrbuf As New StringBuilder(256)

            While True
                nextChar = inputStream.ReadByte()

                ' stream end or client disconnect: stop reading
                If nextChar = -1 Then
                    Exit While
                End If

                If nextChar = ASCII.Byte.LF Then
                    Exit While
                End If
                If nextChar = ASCII.Byte.CR Then
                    Continue While
                End If

                Call chrbuf.Append(Convert.ToChar(nextChar))
            End While

            Return chrbuf.ToString
        End Function

        Public Sub Process()
            ' we can't use a StreamReader for input, because it buffers up extra data on us inside it's
            ' "processed" view of the world, and we want the data raw after the headers
            If _settings IsNot Nothing AndAlso _settings.request_timeout > 0 Then
                socket.ReceiveTimeout = _settings.request_timeout
            End If

            _inputStream = New BufferedStream(socket.GetStream())

            ' we probably shouldn't be using a streamwriter for all output from handlers either
            ' 2017-3-25 使用utf8来尝试解决中文乱码问题
            outputStream = New StreamWriter(New BufferedStream(socket.GetStream()), TextEncodings.UTF8WithoutBOM) With {
                .NewLine = vbCrLf
            }

            Try
                Dim flag = processHttpRequest()

                If flag IsNot Nothing AndAlso Not flag Then
                    ' http header parser error!
                    Call writeFailure(HTTP_RFC.RFC_INTERNAL_SERVER_ERROR, "Invalid request header data!")
                End If
            Catch e As Exception
                Call e.PrintException

                ' the http response stream is no longer available for reporting an
                ' error after the connection has been switched to the websocket
                ' protocol, and the websocket connection object has already handled
                ' its own error internally.
                If Not m_webSocketHijacked Then
                    writeFailure(HTTP_RFC.RFC_INTERNAL_SERVER_ERROR, e.ToString)
                End If
            End Try

            If m_webSocketHijacked Then
                ' the underlying tcp connection and its network streams are owned by
                ' the websocket connection object now, which has already released all
                ' of these resources when its data frame loop exits. so just drop the
                ' stream references at here and leave the socket untouched.
                _inputStream = Nothing
                outputStream = Nothing

                Return
            End If

            Try
                Call outputStream.Flush()
            Catch ex As Exception
                Call App.LogException(ex)
            Finally
                Try
                    Call outputStream.Close()
                    Call outputStream.Dispose()
                Catch ex As Exception
                    Call App.LogException(ex)
                End Try
            End Try

            ' bs.Flush(); // flush any remaining output
            _inputStream = Nothing
            outputStream = Nothing

            Try
                Call socket.Close()
            Catch ex As Exception
                Call App.LogException(ex)
            End Try
        End Sub

        ''' <summary>
        ''' 在这个方法之中完成对一次http请求的解析到相对应的API处理的完整过程，当这个方法执行完毕之后就会关闭socket断开与浏览器的连接了
        ''' </summary>
        Private Function processHttpRequest() As Boolean?
            ' 解析http请求
            If Not parseRequest() Then
                ' 没有解析到请求的头部，则不会再做进一步的处理了，直接退出断开连接
                ' 不在抛出错误了，因为抛出错误的整个处理过程开销比较大
                Return False
            Else
                Call readHeaders()
            End If

            ' the RFC6455 websocket upgrade handshake is a http GET request which
            ' reuses current tcp connection, so it must be detected at here before
            ' the request is dispatched to the regular http request handlers.
            If isWebSocketRequest() Then
                Call handleWebSocketUpgrade()
                Return Nothing
            End If

            ' 调用相对应的API进行请求的处理
            If http_method = "GET" Then
                handleGETRequest()
            ElseIf http_method = "POST" Then
                HandlePOSTRequest()
            ElseIf http_method = "OPTIONS" AndAlso Preflight.IsPreflightRequest(Me) Then
                Preflight.HandlePreflightRequest(Me)
            Else
                Call srv.handleOtherMethod(Me)
            End If

            Return Nothing
        End Function

#Region "WebSocket protocol upgrade"

        ''' <summary>
        ''' test of current http request is a RFC6455 websocket upgrade handshake
        ''' request which should be served by this server or not.
        ''' </summary>
        ''' <returns>
        ''' the websocket handshake request will be treated as a regular http request
        ''' when the websocket feature has been disabled via the server configuration,
        ''' or no application message handler is published on the requested url path.
        ''' </returns>
        ''' <summary>
        ''' get a http request header value via the plain dictionary lookup, which
        ''' never writes a missing key warning message into the server log for an
        ''' optional request header.
        ''' </summary>
        ''' <returns>
        ''' this function always returns a string value, an empty string will be
        ''' returned when the given request header is not presented in the request.
        ''' </returns>
        Private Function getHeader(name As String) As String
            Dim value As String = Nothing
            Return If(httpHeaders.TryGetValue(name, value), value, "")
        End Function

        Private Function isWebSocketRequest() As Boolean
            If _settings IsNot Nothing AndAlso Not _settings.websocket_enabled Then
                Return False
            ElseIf Not WebSocketConnection.IsWebSocketUpgrade(http_method, httpHeaders) Then
                Return False
            Else
                ' let the regular http request handler produces a 404 response when
                ' no websocket endpoint is published on the requested url path.
                Return srv.WebSocket.CanHandle(http_url)
            End If
        End Function

        ''' <summary>
        ''' complete the RFC6455 websocket upgrade handshake and then hands over
        ''' current tcp connection to the websocket data frame processing loop.
        ''' </summary>
        ''' <remarks>
        ''' current worker thread will be blocked inside this method until the
        ''' websocket connection is closed, which is the expected behaviour as one
        ''' websocket connection occupies one connection slot of the server
        ''' connection semaphore during its whole lifecycle.
        ''' </remarks>
        Private Sub handleWebSocketUpgrade()
            Dim version As String = getHeader(RequestHeaders.SecWebSocketVersion).Trim

            ' RFC6455 section-4.4: the server must reply a 426 response with the
            ' supported protocol version when the client speaks another version.
            If Not version.TextEquals(WebSocketProtocol.SupportedVersion) Then
                Call writeWebSocketVersionMismatch(version)
                Return
            End If

            Dim key As String = getHeader(RequestHeaders.SecWebSocketKey)

            If key.StringEmpty Then
                Call writeFailure(HTTP_RFC.RFC_BAD_REQUEST, "Missing the Sec-WebSocket-Key request header.")
                Return
            End If

            Dim handler As WebSocket.IWebSocketHandler = srv.WebSocket.ResolveHandler(http_url)

            If handler Is Nothing Then
                ' the route table was modified by another thread just after the
                ' isWebSocketRequest() check has been passed.
                Call writeFailure(HTTP_RFC.RFC_NOT_FOUND, $"No websocket endpoint is published on '{http_url}'.")
                Return
            End If

            Dim accept As String = WebSocketConnection.CreateAcceptKey(key)
            ' the ``Sec-WebSocket-Protocol`` request header is optional, so the
            ' plain dictionary lookup is used at here for avoid the missing key
            ' warning log of the collection extension helper.
            Dim subProtocol As String = WebSocketConnection.NegotiateSubProtocol(
                clientOffer:=getHeader(RequestHeaders.SecWebSocketProtocol),
                serverSupports:=_settings.GetWebSocketSubProtocols
            )
            ' the handshake response must be written onto the raw network stream
            ' directly instead of the buffered text mode ``outputStream`` writer,
            ' as the connection turns into the binary data frame protocol right
            ' after this handshake response.
            Dim network As Stream = socket.GetStream()

            ' mark the hijack flag before the handshake response is written out, so
            ' that the Process() method will never touch this connection again even
            ' if the handshake response writing failed.
            m_webSocketHijacked = True

            Call WebSocketConnection.WriteHandshakeResponse(network, accept, subProtocol)
            Call $"websocket handshake accepted on '{http_url}'.".info(_settings.silent)

            ' an established websocket connection may live for a very long time, so
            ' the socket receive timeout of the http request stage must be reset,
            ' otherwise the connection will be dropped when the client keeps silent.
            Try
                socket.ReceiveTimeout = If(_settings.websocket_read_timeout > 0, _settings.websocket_read_timeout, 0)
            Catch ex As Exception
                Call App.LogException(ex)
            End Try

            ' the ``_inputStream`` buffered stream is reused at here on purpose: it
            ' may already hold some of the bytes beyond the request header block,
            ' those buffered bytes will be lost when a brand new stream is created.
            Dim connection As New WebSocketConnection(
                socket:=socket,
                input:=_inputStream,
                output:=network,
                path:=WebSocketManager.NormalizePath(http_url),
                url:=http_url,
                subProtocol:=subProtocol,
                headers:=httpHeaders,
                handler:=handler,
                manager:=srv.WebSocket,
                maxMessageSize:=_settings.websocket_max_message_size,
                silent:=_settings.silent
            )

            ' blocks current worker thread until the websocket connection is closed
            Call connection.RunLoop()
        End Sub

        ''' <summary>
        ''' write a ``426 Upgrade Required`` response for a websocket handshake
        ''' request which speaks an unsupported protocol version.
        ''' </summary>
        Private Sub writeWebSocketVersionMismatch(clientVersion As String)
            Call $"reject the websocket handshake of an unsupported protocol version: '{clientVersion}'.".warning(_settings.silent)

            Try
                Call outputStream.WriteLine("HTTP/1.1 426 Upgrade Required")
                Call outputStream.WriteLine($"{ResponseHeaders.SecWebSocketVersion}: {WebSocketProtocol.SupportedVersion}")
                Call outputStream.WriteLine("Content-Type: text/plain")
                Call outputStream.WriteLine("Connection: close")
                Call outputStream.WriteLine("Date: " & DateTime.UtcNow.ToString("R"))
                Call outputStream.WriteLine("Server: " & VBS_platform)
                Call outputStream.WriteLine()
                Call outputStream.WriteLine($"Only the websocket protocol version {WebSocketProtocol.SupportedVersion} is supported by this server.")
            Catch ex As Exception
                Call App.LogException(ex)
            End Try
        End Sub

#End Region

        ''' <summary>
        ''' 对于非法的header格式会直接抛出错误，对于空的请求则会返回False
        ''' </summary>
        ''' <returns></returns>
        Private Function parseRequest() As Boolean
            Dim request As String = streamReadLine(_inputStream)

            If request.StringEmpty Then
                ' no data received within the socket receive timeout window,
                ' the client probably disconnected or sent an empty request.
                Return False
            End If

            Dim tokens As String() = request.Split(" "c)

            If tokens.Length <> 3 Then
                Return False
            Else
                http_method = tokens(0).ToUpper()
                http_url = tokens(1)
                http_protocol_versionstring = tokens(2)

                Call _raw.AppendLine(request)
                Call $"starting: {request}".info(_settings.silent)
            End If

            Return True
        End Function

        Public Sub readHeaders()
            Dim line As String = "", s As New Value(Of String)
            Dim separator As Integer

            While (s = streamReadLine(_inputStream)) IsNot Nothing
                If s.Value.StringEmpty Then
                    Return
                Else
                    line = s.Value
                    separator = line.IndexOf(":"c)
                End If

                If separator = -1 Then
                    Continue While
                End If

                Dim name As String = line.Substring(0, separator)
                Dim pos As Integer = separator + 1

                While (pos < line.Length) AndAlso (line(pos) = " "c)
                    ' strip any spaces
                    pos += 1
                End While

                Dim value As String = line.Substring(pos, line.Length - pos)

                _raw.AppendLine(s.Value)
                httpHeaders(name) = value
            End While
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub handleGETRequest()
            Call srv.handleGETRequest(Me)
        End Sub

        Public Const BUF_SIZE% = 4096

        Public Const packageTooLarge$ = "POST Content-Length({0}) too big for this web server"

        ''' <summary>
        ''' This post data processing just reads everything into a memory stream.
        ''' this is fine for smallish things, but for large stuff we should really
        ''' hand an input stream to the request processor. However, the input stream 
        ''' we hand him needs to let him see the "end of the stream" at this content 
        ''' length, because otherwise he won't know when he's seen it all! 
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub HandlePOSTRequest()
            Dim handle$ = TempFileSystem.GetAppSysTempFile(, sessionID:=App.PID)
            Dim result As (error%, message$) = Nothing

            ' nodejs is content-length
            ' the httpHeaders dictionary is now case-insensitive, so a single
            ' ContainsKey check covers "Content-Length", "content-length", etc.
            If httpHeaders.ContainsKey(ResponseHeaders.ContentLength) Then
                result = flushPOSTPayload(handle)
            End If

            If Not result.message Is Nothing Then
                Call writeFailure(result.error, result.message)
            Else
                Call srv.handlePOSTRequest(Me, handle)
            End If
        End Sub

        ''' <summary>
        ''' save the payload data of the POST request to a given temp file
        ''' </summary>
        ''' <param name="handle">
        ''' the given temp file for save the POST payload
        ''' </param>
        ''' <returns></returns>
        Private Function flushPOSTPayload(handle As String) As (error%, message$)
            Dim content_len% = Convert.ToInt32(httpHeaders.TryGetValue({ResponseHeaders.ContentLength, "content-length"}))

            ' reject negative Content-Length values as malformed requests
            If content_len < 0 Then
                Return (400, "Invalid Content-Length (negative value)")
            End If

            ' 小于零的时候不进行限制
            If MAX_POST_SIZE > 0 AndAlso content_len > MAX_POST_SIZE Then
                Return (413, String.Format(packageTooLarge, content_len))
            End If

            Using content As Stream = handle.Open()
                Dim buf As Byte() = New Byte(BUF_SIZE - 1) {}
                Dim to_read As Integer = content_len
                Dim numread As i32 = 0

                While to_read > 0
                    If (numread = _inputStream.Read(buf, 0, std.Min(BUF_SIZE, to_read))) = 0 Then
                        If to_read = 0 Then
                            Exit While
                        Else
                            Return (900, "remote client disconnected during read post data")
                        End If
                    End If

                    to_read -= numread
                    content.Write(buf, 0, numread)
                End While

                Call content.Flush()
            End Using

            Return Nothing
        End Function

        ''' <summary>
        ''' 默认是html文件类型
        ''' </summary>
        ''' <param name="len"></param>
        ''' <param name="content_type"></param>
        Public Sub writeSuccess(len&, Optional content_type As String = "text/html")
            Try
                Call writeSuccess(
                    content_type, New Content With {
                        .length = len
                    })
            Catch ex As Exception
                Call App.LogException(ex)
            End Try
        End Sub

        ''' <summary>
        ''' VB server script http platform
        ''' </summary>
        Public Const VBS_platform$ = "microsoft-visualbasic-servlet(*.vbs)"
        Public Const XPoweredBy$ = "X-Powered-By: "

        Private Sub writeSuccess(content_type As String, content As Content)
            ' HTTP/1.1 keeps the connection alive by default unless the client
            ' explicitly asked to close it.
            Dim keepAlive As Boolean = Not httpHeaders.ContainsKey("connection") OrElse
                Not httpHeaders("connection").TextEquals("close")

            ' this is the successful HTTP response line
            Call outputStream.WriteLine("HTTP/1.1 200 OK")
            ' these are the HTTP headers...          
            Call outputStream.WriteLine("Content-Length: " & content.length)
            Call outputStream.WriteLine("Content-Type: " & content_type)
            Call outputStream.WriteLine("Connection: " & If(keepAlive, "keep-alive", "close"))
            Call outputStream.WriteLine("Date: " & DateTime.UtcNow.ToString("R"))
            Call outputStream.WriteLine("Server: " & VBS_platform)
            ' ..add your own headers here if you like

            ' Call content.WriteHeader(outputStream)

            Call outputStream.WriteLine(XPoweredBy & _settings.x_powered_by)
            Call outputStream.WriteLine()
            ' this terminates the HTTP headers.. everything after this is HTTP body..
            Call outputStream.Flush()
        End Sub

        Public Sub writeSuccess(content As Content)
            Try
                Call writeSuccess(content.type, content)
            Catch ex As Exception
                ex = New Exception(content.GetJson)
                Call App.LogException(ex)
            End Try
        End Sub

        ''' <summary>
        ''' You can customize your 404 error page at here.
        ''' </summary>
        ''' <remarks>
        ''' 因为并不是每一次请求都会产生404错误的，并且由于404页面是需要通过vbhtml脚本来实现的，
        ''' 所以在这里使用函数指针，仅在发生错误的时候才会调用404的页面构造的过程，以提高网页
        ''' 服务器的性能
        ''' </remarks>
        Public errorPage As New HttpHeader.HttpError

        ''' <summary>
        ''' 404
        ''' </summary>
        Public Sub writeFailure(error_code As HTTP_RFC, ex As String)
            Try
                Call writeFailureInternal(error_code, ex)
            Catch e As Exception
                Call App.LogException(e)
            End Try
        End Sub

        ''' <summary>
        ''' 404
        ''' </summary>
        Private Sub writeFailureInternal(error_code As HTTP_RFC, ex As String)
            Static error_status As Dictionary(Of HTTP_RFC, String) = Enums(Of HTTP_RFC)() _
                .ToDictionary(Function(c) c,
                              Function(c)
                                  Dim text As String = c.Description
                                  Dim str As String = If(text.StringEmpty, c.ToString, text)
                                  Return str
                              End Function)

            ' this is an http 404 failure response
            Call outputStream.WriteLine($"HTTP/1.1 {CLng(error_code)} " & error_status(error_code))
            ' these are the HTTP headers
            Call outputStream.WriteLine("Content-Type: text/html")
            Call outputStream.WriteLine("Connection: close")
            Call outputStream.WriteLine("Date: " & DateTime.UtcNow.ToString("R"))
            Call outputStream.WriteLine("Server: " & VBS_platform)
            ' ..add your own headers here
            Call outputStream.WriteLine(XPoweredBy & _settings.x_powered_by)
            ' this terminates the HTTP headers.
            Call outputStream.WriteLine("")

            Call outputStream.WriteLine(errorPage.GetErrorPage(ex))
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Call outputStream.Flush()
                    Call outputStream.Close()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
End Namespace
