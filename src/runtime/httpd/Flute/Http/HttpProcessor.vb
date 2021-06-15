﻿#Region "Microsoft.VisualBasic::67c08e37a636e1d4fd22702d8b2f3e3a, WebCloud\SMRUCC.HTTPInternal\Core\HttpProcessor.vb"

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

'     Class HttpProcessor
' 
'         Properties: _404Page, http_method, http_protocol_versionstring, http_url, httpHeaders
'                     IsWWWRoot, Out
' 
'         Constructor: (+1 Overloads) Sub New
' 
'         Function: __streamReadLine, parseRequest, ToString, writeTemp
' 
'         Sub: __processInvoker, __writeFailure, __writeSuccess, (+2 Overloads) Dispose, handleGETRequest
'              HandlePOSTRequest, Process, readHeaders, WriteData, writeFailure
'              WriteLine, (+2 Overloads) writeSuccess
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Net.Sockets
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports Flute.Http.Core.Message
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports ResponseHeaders = Flute.Http.Core.Message.HttpHeader.ResponseHeaders
Imports stdNum = System.Math

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

        Friend ReadOnly _inputStream As Stream
        Friend ReadOnly _silent As Boolean = False

        Public outputStream As StreamWriter

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' http方法名是大写的
        ''' </remarks>
        Public Property http_method As String

        ''' <summary>
        ''' File location or GET/POST request arguments
        ''' </summary>
        ''' <returns></returns>
        Public Property http_url As String
        Public Property http_protocol_versionstring As String
        Public Property httpHeaders As New Dictionary(Of String, String)

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
        ''' 10MB
        ''' </summary>
        ''' <remarks></remarks>
        ReadOnly MAX_POST_SIZE% = 128 * 1024 * 1024

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

        Public Sub New(socket As TcpClient, srv As HttpServer, MAX_POST_SIZE%, Optional silent As Boolean = False)
            Me.socket = socket
            Me.srv = srv
            Me.MAX_POST_SIZE = MAX_POST_SIZE
            Me.MAX_POST_SIZE = -1
            Me._silent = silent
        End Sub

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
            Return New HttpResponse(outputStream, AddressOf writeFailure)
        End Function

        Private Function streamReadLine(inputStream As Stream) As String
            Dim nextChar As Integer
            Dim chrbuf As New List(Of Char)
            Dim n As Integer

            While True
                nextChar = inputStream.ReadByte()

                If nextChar = ASCII.Byte.LF Then
                    Exit While
                End If
                If nextChar = ASCII.Byte.CR Then
                    Continue While
                End If

                If nextChar = -1 Then
                    Call Thread.Sleep(1)
                    n += 1
                    If n > 1024 Then
                        Exit While
                    Else
                        Continue While
                    End If
                End If

                Call chrbuf.Add(Convert.ToChar(nextChar))
            End While

            Return New String(chrbuf.ToArray)
        End Function

        Public Sub Process()
            ' we can't use a StreamReader for input, because it buffers up extra data on us inside it's
            ' "processed" view of the world, and we want the data raw after the headers
            _inputStream = New BufferedStream(socket.GetStream())

            ' we probably shouldn't be using a streamwriter for all output from handlers either
            ' 2017-3-25 使用utf8来尝试解决中文乱码问题
            outputStream = New StreamWriter(New BufferedStream(socket.GetStream()), TextEncodings.UTF8WithoutBOM) With {
                .NewLine = vbCrLf
            }

            Try
                Call doProcessInvoker()
            Catch e As Exception
                Call e.PrintException
                writeFailure(HTTP_RFC.RFC_INTERNAL_SERVER_ERROR, e.ToString)
            End Try

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

            End Try
        End Sub

        ''' <summary>
        ''' 在这个方法之中完成对一次http请求的解析到相对应的API处理的完整过程，当这个方法执行完毕之后就会关闭socket断开与浏览器的连接了
        ''' </summary>
        Private Sub doProcessInvoker()
            ' 解析http请求
            If Not parseRequest() Then
                ' 没有解析到请求的头部，则不会再做进一步的处理了，直接退出断开连接
                ' 不在抛出错误了，因为抛出错误的整个处理过程开销比较大
                Call $"[{socket.Client.RemoteEndPoint.ToString}] Empty request header, this request will not be processed!".Warning
                Return
            Else
                Call readHeaders()
            End If

            ' 调用相对应的API进行请求的处理
            If http_method.Equals("GET", StringComparison.OrdinalIgnoreCase) Then
                handleGETRequest()
            ElseIf http_method.Equals("POST", StringComparison.OrdinalIgnoreCase) Then
                HandlePOSTRequest()
            Else
                Call srv.handleOtherMethod(Me)
            End If
        End Sub

        ''' <summary>
        ''' 对于非法的header格式会直接抛出错误，对于空的请求则会返回False
        ''' </summary>
        ''' <returns></returns>
        Private Function parseRequest() As Boolean
            Dim request As String = streamReadLine(_inputStream)

            If request.StringEmpty Then
                ' 2017-3-25 因为在__streamReadLine函数之中可能会出现没有数据导致休眠时间长度可能会超过1024ms
                ' 所以在这里只需要等待3次就行了，以避免当前线程占用系统资源的时间过长而导致对其他的请求响应过低
                Dim wait% = 3

                Do While request.StringEmpty
                    ' 可能是网络传输速度比较慢，在这里等待一段时间再解析流之中的数据
                    ' 但是当前的这条处理线程最多只等待wait次数
                    Call Thread.Sleep(5)

                    If wait <= 0 Then
                        Return False
                    Else
                        request = streamReadLine(_inputStream)
                        wait -= 1
                    End If
                Loop
            End If

            Dim tokens As String() = request.Split(" "c)

            If tokens.Length <> 3 Then
                Call ("invalid http request line: " & request).PrintException
                Return False
            Else
                http_method = tokens(0).ToUpper()
                http_url = tokens(1)
                http_protocol_versionstring = tokens(2)

                Call $"starting: {request}".__INFO_ECHO(_silent)
            End If

            Return True
        End Function

        Public Sub readHeaders()
            Dim line As String = "", s As New Value(Of String)
            Dim separator As Integer

            Call NameOf(readHeaders).__DEBUG_ECHO(mute:=_silent)

            While (s = streamReadLine(_inputStream)) IsNot Nothing
                If s.Value.StringEmpty Then
                    Call "got headers".__DEBUG_ECHO(mute:=_silent)
                    Return
                Else
                    line = s.Value
                    separator = line.IndexOf(":"c)
                End If

                If separator = -1 Then
                    Throw New Exception("invalid http header line: " & line)
                End If

                Dim name As String = line.Substring(0, separator)
                Dim pos As Integer = separator + 1

                While (pos < line.Length) AndAlso (line(pos) = " "c)
                    ' strip any spaces
                    pos += 1
                End While

                Dim value As String = line.Substring(pos, line.Length - pos)
                Call $"header: {name}:{value}".__DEBUG_ECHO(mute:=_silent)
                httpHeaders(name) = value
            End While
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub handleGETRequest()
            Call srv.handleGETRequest(Me)
        End Sub

        Public BUF_SIZE As Integer = 4096

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
            Dim content_len% = Convert.ToInt32(httpHeaders(ResponseHeaders.ContentLength))

            ' 小于零的时候不进行限制
            If MAX_POST_SIZE > 0 AndAlso content_len > MAX_POST_SIZE Then
                Return (413, String.Format(packageTooLarge, content_len))
            End If

            Using content As Stream = handle.Open()
                Dim buf As Byte() = New Byte(BUF_SIZE - 1) {}
                Dim to_read As Integer = content_len
                Dim numread As i32 = 0

                While to_read > 0
                    If (numread = _inputStream.Read(buf, 0, stdNum.Min(BUF_SIZE, to_read))) = 0 Then
                        If to_read = 0 Then
                            Exit While
                        Else
                            Return (900, "client disconnected during post")
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
        Public Const XPoweredBy$ = "X-Powered-By: " & VBS_platform

        Private Sub writeSuccess(content_type As String, content As Content)
            ' this is the successful HTTP response line
            Call outputStream.WriteLine("HTTP/1.0 200 OK")
            ' these are the HTTP headers...          
            Call outputStream.WriteLine("Content-Length: " & content.length)
            Call outputStream.WriteLine("Content-Type: " & content_type)
            Call outputStream.WriteLine("Connection: close")
            ' ..add your own headers here if you like

            ' Call content.WriteHeader(outputStream)

            Call outputStream.WriteLine(XPoweredBy)
            ' 2018-1-31 
            ' The server committed a protocol violation. 
            ' Section = ResponseHeader  
            ' Detail  = CR must be followed by LF
            '
            ' RFC 822中的httpHeader必须以CRLF结束的规定的服务器响应。
            '
            ' app.config配置文件修改
            '
            ' <?xml version="1.0" encoding="utf-8" ?>
            ' <configuration>
            ' <system.net> 
            '        <settings> 
            '               <httpWebRequest useUnsafeHeaderParsing = "true" />
            '        </settings>
            ' </system.net>
            ' </configuration>
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
        Public Sub writeFailure(error_code%, ex As String)
            Try
                Call writeFailureInternal(error_code, ex)
            Catch e As Exception
                Call App.LogException(e)
            End Try
        End Sub

        ''' <summary>
        ''' 404
        ''' </summary>
        Private Sub writeFailureInternal(error_code%, ex As String)
            ' this is an http 404 failure response
            Call outputStream.WriteLine("HTTP/1.0 404 Not Found")
            ' these are the HTTP headers
            Call outputStream.WriteLine("Content-Type: text/html")
            Call outputStream.WriteLine("Connection: close")
            ' ..add your own headers here
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
