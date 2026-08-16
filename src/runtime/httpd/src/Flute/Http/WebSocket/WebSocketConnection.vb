#Region "Microsoft.VisualBasic::39401268a41988f07999924707174a3f, src\Flute\Http\WebSocket\WebSocketConnection.vb"

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

    '   Total Lines: 601
    '    Code Lines: 328 (54.58%)
    ' Comment Lines: 183 (30.45%)
    '    - Xml Docs: 84.15%
    ' 
    '   Blank Lines: 90 (14.98%)
    '     File Size: 26.32 KB


    '     Class WebSocketConnection
    ' 
    '         Properties: Headers, Id, IsOpen, Path, Remote
    '                     Session, SubProtocol, Url
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: CreateAcceptKey, frameLoop, IsWebSocketUpgrade, NegotiateSubProtocol, SendBinary
    '                   SendFrame, SendPing, SendText, ToString
    ' 
    '         Sub: CloseConnection, (+2 Overloads) Dispose, raiseClose, raiseConnect, raiseError
    '              raiseMessage, RunLoop, WriteHandshakeResponse
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Runtime.CompilerServices
Imports System.Security.Cryptography
Imports System.Text
Imports Flute.Http.Core.Message.HttpHeader

Namespace Core.WebSocket

    ''' <summary>
    ''' a single established websocket connection between this server and a remote
    ''' client, this object takes over the whole lifecycle of the underlying tcp
    ''' connection after the RFC6455 handshake has been completed successfully.
    ''' </summary>
    ''' <remarks>
    ''' the data frame reading loop is running on the caller thread inside the
    ''' <see cref="RunLoop"/> method, and the frame writing operation is protected
    ''' by an internal lock, so a message could be pushed to the client safely from
    ''' any other thread, i.e. the broadcast operation of the <see cref="WebSocketManager"/>.
    ''' </remarks>
    Public Class WebSocketConnection : Implements IDisposable

        ''' <summary>
        ''' the unique id of current websocket connection
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Id As String
        ''' <summary>
        ''' the requested url path of current websocket connection, this value is
        ''' used by the <see cref="WebSocketManager"/> for routing the connection
        ''' to its corresponding application message handler.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Path As String
        ''' <summary>
        ''' the raw request url of the websocket handshake request, the url query
        ''' arguments are included in this property value.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Url As String
        ''' <summary>
        ''' the negotiated websocket sub-protocol name, this value is nothing when
        ''' no sub-protocol has been negotiated in the handshake stage.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property SubProtocol As String
        ''' <summary>
        ''' the http request headers of the websocket handshake request
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Headers As Dictionary(Of String, String)
        ''' <summary>
        ''' the remote client network endpoint of current websocket connection
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Remote As EndPoint
        ''' <summary>
        ''' a user state data bag which could be used by the application code for
        ''' associating any custom session data with current connection.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Session As New Dictionary(Of String, Object)

        ''' <summary>
        ''' is current websocket connection still alive for the message exchange?
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsOpen As Boolean
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Not m_closed
            End Get
        End Property

        Private ReadOnly m_socket As TcpClient
        Private ReadOnly m_input As Stream
        Private ReadOnly m_output As Stream
        Private ReadOnly m_handler As IWebSocketHandler
        Private ReadOnly m_manager As WebSocketManager
        Private ReadOnly m_maxMessageSize As Integer
        Private ReadOnly m_silent As Boolean
        ''' <summary>
        ''' the writing operation of the network stream is not thread safe, and a
        ''' data frame must be written onto the wire as an atomic unit, otherwise
        ''' the data frames from the different threads will be interleaved.
        ''' </summary>
        Private ReadOnly m_writeLock As New Object()

        Private m_closed As Boolean = False
        ''' <summary>
        ''' has the close control frame been sent to the remote client already?
        ''' </summary>
        Private m_closeSent As Boolean = False

        ''' <summary>
        ''' create a new websocket connection object of an established tcp connection
        ''' whose RFC6455 handshake has been completed successfully.
        ''' </summary>
        ''' <param name="socket">the underlying tcp connection</param>
        ''' <param name="input">
        ''' the input stream for reading the client data frames. this stream object
        ''' should be the same stream object which was used for reading the http
        ''' handshake request headers, as such buffered stream object may already
        ''' hold some of the data bytes beyond the request header block.
        ''' </param>
        ''' <param name="output">the raw output stream for writing the server data frames</param>
        ''' <param name="handler">the application level message handler of current connection</param>
        Sub New(socket As TcpClient, input As Stream, output As Stream,
                path As String,
                url As String,
                subProtocol As String,
                headers As Dictionary(Of String, String),
                handler As IWebSocketHandler,
                Optional manager As WebSocketManager = Nothing,
                Optional maxMessageSize As Integer = 16 * 1024 * 1024,
                Optional silent As Boolean = False)

            Me.m_socket = socket
            Me.m_input = input
            Me.m_output = output
            Me.m_handler = handler
            Me.m_manager = manager
            Me.m_maxMessageSize = maxMessageSize
            Me.m_silent = silent

            Me._Id = Guid.NewGuid.ToString
            Me._Path = path
            Me._Url = url
            Me._SubProtocol = subProtocol
            Me._Headers = If(headers, New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase))

            Try
                Me._Remote = socket.Client.RemoteEndPoint
            Catch ex As Exception
                ' the socket may have already been disconnected
                Me._Remote = Nothing
            End Try
        End Sub

        Public Overrides Function ToString() As String
            Return $"websocket://{Remote}{Path}"
        End Function

#Region "Handshake"

        ''' <summary>
        ''' test of the given http request headers is a RFC6455 websocket upgrade
        ''' handshake request or not.
        ''' </summary>
        ''' <param name="method">the http request method name, must be ``GET``</param>
        ''' <param name="headers">the parsed http request headers</param>
        ''' <returns>
        ''' this function returns true only when all of the required websocket
        ''' handshake request headers are presented in the given http request.
        ''' </returns>
        Public Shared Function IsWebSocketUpgrade(method As String, headers As Dictionary(Of String, String)) As Boolean
            If headers Is Nothing Then
                Return False
            End If
            ' RFC6455 section-4.1: the handshake must be a http GET request
            If Not method.TextEquals("GET") Then
                Return False
            End If

            Dim upgrade As String = Nothing
            Dim connection As String = Nothing

            If Not headers.TryGetValue(RequestHeaders.Upgrade, upgrade) Then
                Return False
            End If
            If Not headers.TryGetValue(RequestHeaders.Connection, connection) Then
                Return False
            End If
            If Not headers.ContainsKey(RequestHeaders.SecWebSocketKey) Then
                Return False
            End If

            ' the ``Connection`` header may carry a comma separated token list,
            ' i.e. ``Connection: keep-alive, Upgrade``
            Dim hasUpgradeToken As Boolean = connection _
                .Split(","c) _
                .Any(Function(token) token.Trim.TextEquals(WebSocketProtocol.ConnectionToken))

            Return upgrade.Trim.TextEquals(WebSocketProtocol.UpgradeToken) AndAlso hasUpgradeToken
        End Function

        ''' <summary>
        ''' create the ``Sec-WebSocket-Accept`` response header value from the client
        ''' offered ``Sec-WebSocket-Key`` nonce value.
        ''' </summary>
        ''' <remarks>
        ''' RFC6455 section-4.2.2: the accept value is generated by concat the client
        ''' key with the websocket protocol GUID, then take the SHA1 hash of this
        ''' concated string and encode the hash bytes with base64.
        ''' </remarks>
        Public Shared Function CreateAcceptKey(secWebSocketKey As String) As String
            Using sha1 As SHA1 = SHA1.Create()
                Dim raw As Byte() = Encoding.ASCII.GetBytes(secWebSocketKey.Trim & WebSocketProtocol.WebSocketGuid)
                Dim hash As Byte() = sha1.ComputeHash(raw)

                Return Convert.ToBase64String(hash)
            End Using
        End Function

        ''' <summary>
        ''' negotiate the websocket sub-protocol between the client offered candidate
        ''' list and the server supported protocol name list.
        ''' </summary>
        ''' <param name="clientOffer">the raw value of the ``Sec-WebSocket-Protocol`` request header</param>
        ''' <param name="serverSupports">the sub-protocol name list which is supported by this server</param>
        ''' <returns>
        ''' the first client offered sub-protocol name which is also supported by this
        ''' server, returns nothing when no sub-protocol could be negotiated.
        ''' </returns>
        Public Shared Function NegotiateSubProtocol(clientOffer As String, serverSupports As String()) As String
            If clientOffer.StringEmpty OrElse serverSupports.IsNullOrEmpty Then
                Return Nothing
            End If

            ' the client candidates are ordered by the client preference, so the
            ' first client candidate which hits the server support list wins.
            For Each candidate As String In clientOffer.Split(","c).Select(Function(str) str.Trim)
                If candidate.StringEmpty Then
                    Continue For
                End If

                For Each supports As String In serverSupports
                    If candidate.TextEquals(supports) Then
                        Return supports
                    End If
                Next
            Next

            Return Nothing
        End Function

        ''' <summary>
        ''' write the ``HTTP/1.1 101 Switching Protocols`` handshake response onto
        ''' the given raw network stream.
        ''' </summary>
        ''' <remarks>
        ''' the handshake response must be written onto the raw network stream as the
        ''' ascii encoded bytes directly, as the whole connection will be switched to
        ''' the binary websocket data frame protocol immediately after this response.
        ''' </remarks>
        ''' <param name="output">the raw output network stream</param>
        ''' <param name="acceptKey">the ``Sec-WebSocket-Accept`` header value</param>
        ''' <param name="subProtocol">the negotiated sub-protocol name, this header is omitted when the value is nothing</param>
        Public Shared Sub WriteHandshakeResponse(output As Stream, acceptKey As String, Optional subProtocol As String = Nothing)
            Dim response As New StringBuilder

            Call response.Append("HTTP/1.1 101 Switching Protocols" & vbCrLf)
            Call response.Append($"{ResponseHeaders.Upgrade}: {WebSocketProtocol.UpgradeToken}" & vbCrLf)
            Call response.Append($"{RequestHeaders.Connection}: {WebSocketProtocol.ConnectionToken}" & vbCrLf)
            Call response.Append($"{ResponseHeaders.SecWebSocketAccept}: {acceptKey}" & vbCrLf)

            If Not subProtocol.StringEmpty Then
                Call response.Append($"{ResponseHeaders.SecWebSocketProtocol}: {subProtocol}" & vbCrLf)
            End If

            ' this empty line terminates the handshake response header block
            Call response.Append(vbCrLf)

            Dim buffer As Byte() = Encoding.ASCII.GetBytes(response.ToString)

            Call output.Write(buffer, Scan0, buffer.Length)
            Call output.Flush()
        End Sub

#End Region

#Region "Frame loop"

        ''' <summary>
        ''' run the websocket data frame reading loop on current thread, this method
        ''' will be blocked until the remote client disconnected or the close
        ''' handshake has been completed.
        ''' </summary>
        ''' <remarks>
        ''' all of the errors which occurs inside this data frame loop are captured
        ''' and reported via the <see cref="IWebSocketHandler.OnError"/> event, so an
        ''' error of a single connection will never break the other connections.
        ''' </remarks>
        Public Sub RunLoop()
            Dim closeCode As WebSocketCloseCode = WebSocketCloseCode.AbnormalClosure
            Dim closeReason As String = ""

            Try
                Call m_manager?.Register(Me)
                Call raiseConnect()
                Call $"websocket connection [{Id}] on {Path} has been established from {Remote}.".info(m_silent)

                Dim result = frameLoop()

                closeCode = result.code
                closeReason = result.reason
            Catch ex As Exception
                ' an unexpected error, i.e. the network cable was unplugged
                Call raiseError(ex)
                closeCode = WebSocketCloseCode.AbnormalClosure
                closeReason = ex.Message
            Finally
                Try
                    Call raiseClose(closeCode, closeReason)
                Catch ex As Exception
                    Call App.LogException(ex)
                End Try

                Call m_manager?.Unregister(Me)
                Call Dispose()
                Call $"websocket connection [{Id}] has been closed: {closeCode.ToString}.".info(m_silent)
            End Try
        End Sub

        ''' <summary>
        ''' the websocket data frame reading loop
        ''' </summary>
        ''' <returns>the close status code and the close reason of current connection</returns>
        Private Function frameLoop() As (code As WebSocketCloseCode, reason As String)
            ' the message fragmentation re-assembly buffer, RFC6455 section-5.4
            Dim fragments As MemoryStream = Nothing
            Dim fragmentOpcode As WebSocketOpcode = WebSocketOpcode.Continuation

            Do While Not m_closed
                Dim frame As WebSocketFrame

                Try
                    frame = WebSocketFrame.ReadFrame(m_input, m_maxMessageSize)
                Catch ex As WebSocketProtocolException
                    ' the remote client violated the RFC6455 specification
                    Call raiseError(ex)
                    Call CloseConnection(ex.CloseCode, ex.Message)
                    Return (ex.CloseCode, ex.Message)
                End Try

                If frame Is Nothing Then
                    ' the remote client disconnected without the close handshake
                    Return (WebSocketCloseCode.AbnormalClosure, "The remote client disconnected unexpectedly.")
                End If

                ' RFC6455 section-5.1: the server must close the connection when it
                ' received an unmasked data frame from the client.
                If Not frame.Masked Then
                    Const unmasked As String = "All of the data frames which is sent from the client must be masked."

                    Call CloseConnection(WebSocketCloseCode.ProtocolError, unmasked)
                    Return (WebSocketCloseCode.ProtocolError, unmasked)
                End If

                ' the control frame could be injected in the middle of a fragmented
                ' application message, so it must be handled before the re-assembly.
                If frame.IsControlFrame Then
                    Select Case frame.Opcode
                        Case WebSocketOpcode.Close
                            Dim code As WebSocketCloseCode = WebSocketFrame.ParseCloseCode(frame.Payload)
                            Dim reason As String = WebSocketFrame.ParseCloseReason(frame.Payload)

                            ' echo back the close frame for finish the close handshake
                            Call CloseConnection(If(code = WebSocketCloseCode.NoStatusReceived, WebSocketCloseCode.NormalClosure, code), Nothing)
                            Return (code, reason)

                        Case WebSocketOpcode.Ping
                            ' RFC6455 section-5.5.3: a pong frame must carry the
                            ' identical application data of the ping frame.
                            Call SendFrame(WebSocketOpcode.Pong, frame.Payload)

                        Case WebSocketOpcode.Pong
                            ' an unsolicited pong frame is just a heartbeat, ignore it
                    End Select

                    Continue Do
                End If

                If frame.Opcode = WebSocketOpcode.Continuation Then
                    If fragments Is Nothing Then
                        Const orphan As String = "Received a continuation data frame without an initial data frame."

                        Call CloseConnection(WebSocketCloseCode.ProtocolError, orphan)
                        Return (WebSocketCloseCode.ProtocolError, orphan)
                    End If
                ElseIf fragments IsNot Nothing Then
                    Const interleaved As String = "Received a new data frame before the previous fragmented message was finished."

                    Call CloseConnection(WebSocketCloseCode.ProtocolError, interleaved)
                    Return (WebSocketCloseCode.ProtocolError, interleaved)
                End If

                If frame.Fin AndAlso fragments Is Nothing Then
                    ' a complete un-fragmented application message
                    Call raiseMessage(frame.Payload, frame.Opcode = WebSocketOpcode.Binary)
                    Continue Do
                End If

                If fragments Is Nothing Then
                    ' the initial fragment of a new application message
                    fragments = New MemoryStream()
                    fragmentOpcode = frame.Opcode
                End If

                If m_maxMessageSize > 0 AndAlso fragments.Length + frame.Length > m_maxMessageSize Then
                    Dim oversized As String = $"The re-assembled application message is greater than the server limit({m_maxMessageSize} bytes)."

                    Call fragments.Dispose()
                    Call CloseConnection(WebSocketCloseCode.MessageTooBig, oversized)
                    Return (WebSocketCloseCode.MessageTooBig, oversized)
                End If

                Call fragments.Write(frame.Payload, Scan0, frame.Length)

                If frame.Fin Then
                    ' the last fragment, the application message is completed now
                    Dim message As Byte() = fragments.ToArray

                    Call fragments.Dispose()

                    fragments = Nothing

                    Call raiseMessage(message, fragmentOpcode = WebSocketOpcode.Binary)
                End If
            Loop

            Return (WebSocketCloseCode.NormalClosure, "")
        End Function

#End Region

#Region "Send message"

        ''' <summary>
        ''' push an utf8 encoded text message to the remote client
        ''' </summary>
        ''' <param name="text">the text message content</param>
        ''' <returns>returns false when the message could not be delivered</returns>
        Public Function SendText(text As String) As Boolean
            Return SendFrame(WebSocketOpcode.Text, Encoding.UTF8.GetBytes(If(text, "")))
        End Function

        ''' <summary>
        ''' push a raw binary message to the remote client
        ''' </summary>
        ''' <param name="data">the binary message payload</param>
        ''' <returns>returns false when the message could not be delivered</returns>
        Public Function SendBinary(data As Byte()) As Boolean
            Return SendFrame(WebSocketOpcode.Binary, data)
        End Function

        ''' <summary>
        ''' send a ping heartbeat probe control frame to the remote client
        ''' </summary>
        ''' <param name="data">an optional application data of the ping frame</param>
        Public Function SendPing(Optional data As Byte() = Nothing) As Boolean
            Return SendFrame(WebSocketOpcode.Ping, data)
        End Function

        ''' <summary>
        ''' write a single websocket data frame onto the network stream.
        ''' </summary>
        ''' <remarks>
        ''' the writing operation is protected by an internal lock, so this method
        ''' could be invoked safely from the multiple threads concurrently, i.e. the
        ''' broadcast operation of the <see cref="WebSocketManager"/>.
        ''' </remarks>
        ''' <returns>returns false when the data frame could not be delivered</returns>
        Public Function SendFrame(opcode As WebSocketOpcode, payload As Byte()) As Boolean
            If m_closed Then
                Return False
            End If

            Dim buffer As Byte() = WebSocketFrame.EncodeFrame(opcode, payload)

            SyncLock m_writeLock
                If m_closed Then
                    Return False
                End If

                Try
                    Call m_output.Write(buffer, Scan0, buffer.Length)
                    Call m_output.Flush()
                Catch ex As Exception
                    ' the remote client has been disconnected, mark current
                    ' connection as dead so that it could be cleaned up by the
                    ' connection manager.
                    m_closed = True

                    Call App.LogException(ex)

                    Return False
                End Try
            End SyncLock

            Return True
        End Function

        ''' <summary>
        ''' close current websocket connection gracefully with the RFC6455 close
        ''' handshake: a close control frame will be sent to the remote client
        ''' before the underlying tcp connection is shutdown.
        ''' </summary>
        ''' <param name="code">the close status code</param>
        ''' <param name="reason">an optional human readable close reason text</param>
        Public Sub CloseConnection(Optional code As WebSocketCloseCode = WebSocketCloseCode.NormalClosure,
                                   Optional reason As String = Nothing)
            SyncLock m_writeLock
                If m_closeSent OrElse m_closed Then
                    m_closed = True
                    Return
                Else
                    m_closeSent = True
                End If
            End SyncLock

            Try
                ' RFC6455 section-7.4: the reserved status code must not be sent on the wire
                Dim payload As Byte() = If(code = WebSocketCloseCode.NoStatusReceived OrElse code = WebSocketCloseCode.AbnormalClosure,
                    New Byte() {},
                    WebSocketFrame.EncodeClosePayload(code, reason))
                Dim buffer As Byte() = WebSocketFrame.EncodeFrame(WebSocketOpcode.Close, payload)

                SyncLock m_writeLock
                    Call m_output.Write(buffer, Scan0, buffer.Length)
                    Call m_output.Flush()
                End SyncLock
            Catch ex As Exception
                ' the connection may have already been broken down
                Call App.LogException(ex)
            Finally
                m_closed = True
            End Try
        End Sub

#End Region

#Region "Event raise"

        Private Sub raiseConnect()
            Try
                Call m_handler?.OnConnect(Me)
            Catch ex As Exception
                Call raiseError(ex)
            End Try
        End Sub

        Private Sub raiseMessage(payload As Byte(), binary As Boolean)
            Try
                Call m_handler?.OnMessage(Me, New WebSocketMessage(payload, binary))
            Catch ex As Exception
                ' an error which is thrown from the application code should never
                ' break the websocket data frame loop of current connection.
                Call raiseError(ex)
            End Try
        End Sub

        Private Sub raiseClose(code As WebSocketCloseCode, reason As String)
            Try
                Call m_handler?.OnClose(Me, code, reason)
            Catch ex As Exception
                Call App.LogException(ex)
            End Try
        End Sub

        Private Sub raiseError(ex As Exception)
            Call App.LogException(ex)

            Try
                Call m_handler?.OnError(Me, ex)
            Catch err As Exception
                Call App.LogException(err)
            End Try
        End Sub

#End Region

#Region "IDisposable Support"

        Private disposedValue As Boolean

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    m_closed = True

                    Try
                        Call m_socket?.Close()
                    Catch ex As Exception
                        Call App.LogException(ex)
                    End Try
                End If
            End If

            disposedValue = True
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Call Dispose(True)
            Call GC.SuppressFinalize(Me)
        End Sub

#End Region
    End Class
End Namespace
