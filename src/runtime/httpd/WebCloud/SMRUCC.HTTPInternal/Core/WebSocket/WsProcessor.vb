#Region "Microsoft.VisualBasic::83a76e1732ce7b883ee206a998b34e02, WebCloud\SMRUCC.HTTPInternal\Core\WebSocket\WsProcessor.vb"

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

    '     Class WsProcessor
    ' 
    '         Properties: isConnected
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: handshakePayload
    ' 
    '         Sub: CheckForDataAvailability, DecodeFrame, doChecks, HandShake, Response
    '              SendBinary, (+2 Overloads) SendText
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Net.Sockets
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Language

Namespace Core.WebSocket

    ' https://developer.mozilla.org/zh-CN/docs/Web/API/WebSockets_API/WebSocket_Server_Vb.NET

    ''' <summary>
    ''' A websocket client
    ''' </summary>
    Public MustInherit Class WsProcessor

        Public Event onClientDisconnect As OnClientDisconnectDelegateHandler
        Public Event onClientTextMessage As OnClientTextMessage
        Public Event onClientBinaryMessage As OnClinetBinaryMessage

        ''' <summary>
        ''' ^GET
        ''' </summary>
        ReadOnly HttpGet As New Regex("^GET")
        ReadOnly WsSeckey As New Regex("Sec-WebSocket-Key: (.*)")
        ReadOnly sha1 As SHA1 = SHA1.Create()

        ''' <summary>
        ''' 会需要一直保持网络连接
        ''' </summary>
        ReadOnly tcp As TcpClient

        Const WsMagic As String = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"

        Public ReadOnly Property isConnected As Boolean
            Get
                Return tcp.Connected
            End Get
        End Property

        Sub New(tcp As TcpClient)
            Me.tcp = tcp
        End Sub

        Sub HandShake()
            Dim stream As NetworkStream = Me.tcp.GetStream()
            Dim bytes As Byte()
            Dim data As String

            While Me.tcp.Connected
                While (stream.DataAvailable)
                    ReDim bytes(Me.tcp.Client.Available)

                    stream.Read(bytes, 0, bytes.Length)
                    data = Encoding.UTF8.GetString(bytes)

                    If (HttpGet.IsMatch(data)) Then
                        Dim response As Byte()

                        SyncLock sha1
                            response = handshakePayload(data)
                            stream.Write(response, 0, response.Length)
                        End SyncLock

                        Return
                    Else
                        ' We're going to disconnect the client here, because he's not handshacking properly 
                        ' (or at least to the scope of this code sample)
                        ' The next While Me._TcpClient.Connected Loop Check should fail.. 
                        ' And raise the onClientDisconnect Event Thereafter
                        Me.tcp.Close()
                    End If
                End While
            End While

            RaiseEvent onClientDisconnect(Me)
        End Sub

        Private Function handshakePayload(data As String) As Byte()
            Dim magicKey$ = WsSeckey.Match(data).Groups(1).Value.Trim() & WsMagic
            Dim verify$ = Convert.ToBase64String(sha1.ComputeHash(Encoding.UTF8.GetBytes(magicKey)))
            ' 下面的headers的文本最末尾必须以两个newline结束
            ' 所以在下面的数组之中最末尾以两个空白行结束
            Dim httpHeaders = {
                "HTTP/1.1 101 Switching Protocols",
                "Connection: Upgrade",
                "Upgrade: websocket",
                "Sec-WebSocket-Accept: " & verify,
                "",
                ""
            }

            Return Encoding.UTF8.GetBytes(httpHeaders.JoinBy(Environment.NewLine))
        End Function

        Const frameCount = 2

        Sub doChecks()
            Dim stream As NetworkStream = Me.tcp.GetStream()
            Dim bufferSize As Integer = tcp.Client.Available
            Dim bytes As Byte() = New Byte(bufferSize - 1) {}
            Dim decoded As Byte() = Nothing
            Dim operation As Operations

            ' Read the stream, don't close it.. 
            Call stream.Read(bytes, 0, bytes.Length)
            Call DecodeFrame(bytes, operation, decoded)
            Call Response(operation, decoded, stream)
        End Sub

        Private Shared Sub DecodeFrame(bytes As Byte(), ByRef operation As Operations, ByRef decoded As Byte())
            ' this should obviously be a byte (unsigned 8bit value)
            Dim length As UInteger = bytes(1) - 128

            If length > -1 Then
                If length = 126 Then
                    length = 4
                ElseIf length = 127 Then
                    length = 10
                End If
            End If

            ' the following is very inefficient and likely unnecessary.. 
            ' the main purpose is to just get the lower 4 bits of byte(0) - which is the OPCODE
            Dim value As Integer = bytes(0)
            Dim bits As New BitArray(8)

            For c As Integer = 0 To 7 Step 1
                If value - (2 ^ (7 - c)) >= 0 Then
                    bits.Item(c) = True
                    value -= (2 ^ (7 - c))
                Else
                    bits.Item(c) = False
                End If
            Next

            Dim FRRR_OPCODE As String = ""

            For Each bit As Boolean In bits
                If bit Then
                    FRRR_OPCODE &= "1"
                Else
                    FRRR_OPCODE &= "0"
                End If
            Next

            Dim FIN As Integer = FRRR_OPCODE.Substring(0, 1)
            Dim RSV1 As Integer = FRRR_OPCODE.Substring(1, 1)
            Dim RSV2 As Integer = FRRR_OPCODE.Substring(2, 1)
            Dim RSV3 As Integer = FRRR_OPCODE.Substring(3, 1)

            operation = Convert.ToInt32(FRRR_OPCODE.Substring(4, 4), 2)
            decoded = New Byte(bytes.Length - (frameCount + 4) - 1) {}

            ' 20190628 原来这里的变量名是key
            ' 并且下面的masks变量是丢失的
            Dim masks As Byte() = {
                bytes(frameCount),
                bytes(frameCount + 1),
                bytes(frameCount + 2),
                bytes(frameCount + 3)
            }

            Dim j As i32 = Scan0

            For i As Integer = (frameCount + 4) To (bytes.Length - 2) Step 1
                decoded(j) = Convert.ToByte((bytes(i) Xor masks(++j Mod 4)))
            Next
        End Sub

        Private Sub Response(code As Operations, decoded As Byte(), stream As NetworkStream)
            Select Case code
                Case Is = Operations.TextRecieved
                    RaiseEvent onClientTextMessage(Me, Encoding.UTF8.GetString(decoded), stream)

                Case Is = Operations.BinaryRecieved
                    RaiseEvent onClientBinaryMessage(Me, New MemoryStream(decoded), stream)

                Case Is = Operations.Ping
                Case Is = Operations.Pong
                Case Else
                    ' Improper opCode.. disconnect the client 
                    Call tcp.Close()
                    RaiseEvent onClientDisconnect(Me)
            End Select
        End Sub

        Public Sub SendBinary(data As Byte(), stream As NetworkStream)
            Call stream.Write(data, 0, data.Length)
        End Sub

        ''' <summary>
        ''' Send text message to client
        ''' </summary>
        ''' <param name="text"></param>
        Public Sub SendText(text As String)
            Call doChecks()
            Call SendText(text, tcp.GetStream)
        End Sub

        ''' <summary>
        ''' Send text message to client
        ''' </summary>
        ''' <param name="text"></param>
        Public Sub SendText(text As String, stream As NetworkStream)
            Dim Payload As Byte() = Encoding.UTF8.GetBytes(text)
            Dim FRRROPCODE As Byte = Convert.ToByte("10000001", 2) 'FIN is set, and OPCODE is 1 or Text
            Dim header As Byte() = {FRRROPCODE, Convert.ToByte(Payload.Length)}
            Dim ResponseData As Byte()
            Dim index As Integer = 0

            ReDim ResponseData((header.Length + Payload.Length) - 1)
            ' NOTEWORTHY: if you Redim ResponseData(header.length + Payload.Length).. 
            ' you 'll add a 0 value byte at the end of the response data.. 
            ' which tells the client that your next stream write will be a continuation frame..
            Buffer.BlockCopy(header, 0, ResponseData, index, header.Length)
            index += header.Length

            Buffer.BlockCopy(Payload, 0, ResponseData, index, Payload.Length)
            index += Payload.Length

            Call stream.Write(ResponseData, 0, ResponseData.Length)
        End Sub

        ''' <summary>
        ''' Check for new data that send from browser
        ''' </summary>
        Sub CheckForDataAvailability()
            If (Me.tcp.GetStream().DataAvailable) Then
                Try
                    Call doChecks()
                Catch ex As Exception
                    tcp.Close()
                    RaiseEvent onClientDisconnect(Me)
                End Try
            End If
        End Sub
    End Class

End Namespace
