#Region "Microsoft.VisualBasic::04de131f6353fe2417b48cae2b41a8d8, src\Flute\Http\WebSocket\WebSocketFrame.vb"

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

    '   Total Lines: 415
    '    Code Lines: 207 (49.88%)
    ' Comment Lines: 154 (37.11%)
    '    - Xml Docs: 93.51%
    ' 
    '   Blank Lines: 54 (13.01%)
    '     File Size: 17.05 KB


    '     Enum WebSocketOpcode
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    '     Enum WebSocketCloseCode
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class WebSocketFrame
    ' 
    '         Properties: Fin, IsControlFrame, Length, Masked, Opcode
    '                     Payload, Rsv1, Rsv2, Rsv3
    ' 
    '         Function: EncodeClosePayload, EncodeFrame, ParseCloseCode, ParseCloseReason, readExact
    '                   ReadFrame, ToString
    ' 
    '         Sub: ApplyMask
    ' 
    '     Class WebSocketProtocolException
    ' 
    '         Properties: CloseCode
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text

Namespace Core.WebSocket

    ''' <summary>
    ''' the websocket data frame opcode which is defined in the RFC6455 section-5.2
    ''' </summary>
    Public Enum WebSocketOpcode As Byte
        ''' <summary>
        ''' a continuation fragment of the previous data frame
        ''' </summary>
        Continuation = &H0
        ''' <summary>
        ''' an utf8 encoded text message payload
        ''' </summary>
        Text = &H1
        ''' <summary>
        ''' a raw binary message payload
        ''' </summary>
        Binary = &H2
        ''' <summary>
        ''' the connection close control frame
        ''' </summary>
        [Close] = &H8
        ''' <summary>
        ''' the heartbeat probe control frame
        ''' </summary>
        Ping = &H9
        ''' <summary>
        ''' the response control frame of a <see cref="Ping"/> frame
        ''' </summary>
        Pong = &HA
    End Enum

    ''' <summary>
    ''' the websocket connection close status code which is defined in RFC6455 section-7.4.1
    ''' </summary>
    Public Enum WebSocketCloseCode As UShort
        NormalClosure = 1000
        GoingAway = 1001
        ProtocolError = 1002
        UnsupportedData = 1003
        ''' <summary>
        ''' reserved value, must not be sent on the wire
        ''' </summary>
        NoStatusReceived = 1005
        ''' <summary>
        ''' reserved value, must not be sent on the wire
        ''' </summary>
        AbnormalClosure = 1006
        InvalidPayloadData = 1007
        PolicyViolation = 1008
        MessageTooBig = 1009
        MandatoryExtension = 1010
        InternalServerError = 1011
    End Enum

    ''' <summary>
    ''' a single websocket data frame which is defined in the RFC6455 section-5.2
    ''' </summary>
    ''' <remarks>
    ''' <code>
    '''  0                   1                   2                   3
    '''  0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
    ''' +-+-+-+-+-------+-+-------------+-------------------------------+
    ''' |F|R|R|R| opcode|M| Payload len |    Extended payload length    |
    ''' |I|S|S|S|  (4)  |A|     (7)     |             (16/64)           |
    ''' |N|V|V|V|       |S|             |   (if payload len==126/127)   |
    ''' | |1|2|3|       |K|             |                               |
    ''' +-+-+-+-+-------+-+-------------+ - - - - - - - - - - - - - - - +
    ''' </code>
    ''' </remarks>
    Public Class WebSocketFrame

        ''' <summary>
        ''' is this data frame the final fragment of an application message?
        ''' </summary>
        ''' <returns></returns>
        Public Property Fin As Boolean
        ''' <summary>
        ''' the reserved bits, must be all zero when no extension has been negotiated
        ''' </summary>
        ''' <returns></returns>
        Public Property Rsv1 As Boolean
        ''' <summary>
        ''' the reserved bit 2, must be zero when no extension has been negotiated
        ''' </summary>
        Public Property Rsv2 As Boolean
        ''' <summary>
        ''' the reserved bit 3, must be zero when no extension has been negotiated
        ''' </summary>
        Public Property Rsv3 As Boolean
        ''' <summary>
        ''' the payload data type of current data frame
        ''' </summary>
        ''' <returns></returns>
        Public Property Opcode As WebSocketOpcode
        ''' <summary>
        ''' is the <see cref="Payload"/> data masked on the wire? a data frame
        ''' which is sent from the client to the server must be masked.
        ''' </summary>
        ''' <returns></returns>
        Public Property Masked As Boolean
        ''' <summary>
        ''' the unmasked payload data of current data frame
        ''' </summary>
        ''' <returns></returns>
        Public Property Payload As Byte()

        ''' <summary>
        ''' is current data frame a control frame? a control frame could not be
        ''' fragmented and its payload size must be less than or equals to 125 bytes.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsControlFrame As Boolean
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return (CByte(Opcode) And &H8) <> 0
            End Get
        End Property

        ''' <summary>
        ''' the payload data size in bytes of current data frame
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Length As Integer
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return If(Payload Is Nothing, 0, Payload.Length)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{Opcode.ToString}[fin={Fin}, mask={Masked}, len={Length}]"
        End Function

        ''' <summary>
        ''' the max payload size in bytes of a websocket control frame
        ''' </summary>
        Public Const MaxControlFramePayload As Integer = 125

        ''' <summary>
        ''' read a single websocket data frame from the given network stream.
        ''' </summary>
        ''' <param name="input">the network stream of an established websocket connection</param>
        ''' <param name="maxPayloadSize">
        ''' the max size in bytes of a single data frame payload, a value which is less than
        ''' or equals to zero means that no size limit will be checked.
        ''' </param>
        ''' <returns>
        ''' this function returns nothing if the remote client has been disconnected
        ''' before a complete data frame could be read from the network stream.
        ''' </returns>
        ''' <exception cref="WebSocketProtocolException">
        ''' the protocol error will be thrown when the data frame is malformed, i.e. an
        ''' oversized control frame, a fragmented control frame or an oversized payload.
        ''' </exception>
        Public Shared Function ReadFrame(input As Stream, Optional maxPayloadSize As Integer = 0) As WebSocketFrame
            Dim header As Byte() = readExact(input, 2)

            If header Is Nothing Then
                ' the remote client closed the connection
                Return Nothing
            End If

            Dim b0 As Byte = header(0)
            Dim b1 As Byte = header(1)
            Dim frame As New WebSocketFrame With {
                .Fin = (b0 And &H80) <> 0,
                .Rsv1 = (b0 And &H40) <> 0,
                .Rsv2 = (b0 And &H20) <> 0,
                .Rsv3 = (b0 And &H10) <> 0,
                .Opcode = CType(b0 And &HF, WebSocketOpcode),
                .Masked = (b1 And &H80) <> 0
            }

            ' no extension is negotiated by this server, so all of the reserved
            ' bits must be zero, otherwise the peer is speaking an unknown dialect.
            If frame.Rsv1 OrElse frame.Rsv2 OrElse frame.Rsv3 Then
                Throw New WebSocketProtocolException(WebSocketCloseCode.ProtocolError, "Reserved bits must be zero as no extension was negotiated.")
            End If
            If Not [Enum].IsDefined(GetType(WebSocketOpcode), frame.Opcode) Then
                Throw New WebSocketProtocolException(WebSocketCloseCode.ProtocolError, $"Unknown websocket frame opcode: &H{CByte(frame.Opcode):X}.")
            End If

            Dim payloadLen As Long = b1 And &H7F

            If payloadLen = 126 Then
                Dim ext As Byte() = readExact(input, 2)

                If ext Is Nothing Then
                    Return Nothing
                Else
                    payloadLen = (CLng(ext(0)) << 8) Or ext(1)
                End If
            ElseIf payloadLen = 127 Then
                Dim ext As Byte() = readExact(input, 8)

                If ext Is Nothing Then
                    Return Nothing
                End If

                payloadLen = 0

                For i As Integer = 0 To 7
                    payloadLen = (payloadLen << 8) Or ext(i)
                Next

                ' the most significant bit of a 64bits payload length must be zero
                If payloadLen < 0 Then
                    Throw New WebSocketProtocolException(WebSocketCloseCode.ProtocolError, "The most significant bit of the 64bits payload length must be zero.")
                End If
            End If

            If frame.IsControlFrame Then
                ' RFC6455 section-5.5: all control frames must have a payload length
                ' of 125 bytes or less and must not be fragmented.
                If payloadLen > MaxControlFramePayload Then
                    Throw New WebSocketProtocolException(WebSocketCloseCode.ProtocolError, $"The control frame payload({payloadLen} bytes) is oversized.")
                ElseIf Not frame.Fin Then
                    Throw New WebSocketProtocolException(WebSocketCloseCode.ProtocolError, "The control frame could not be fragmented.")
                End If
            ElseIf maxPayloadSize > 0 AndAlso payloadLen > maxPayloadSize Then
                Throw New WebSocketProtocolException(WebSocketCloseCode.MessageTooBig, $"The data frame payload({payloadLen} bytes) is greater than the server limit({maxPayloadSize} bytes).")
            End If

            Dim maskKey As Byte() = Nothing

            If frame.Masked Then
                maskKey = readExact(input, 4)

                If maskKey Is Nothing Then
                    Return Nothing
                End If
            End If

            If payloadLen = 0 Then
                frame.Payload = {}
            Else
                Dim payload As Byte() = readExact(input, CInt(payloadLen))

                If payload Is Nothing Then
                    Return Nothing
                End If

                If frame.Masked Then
                    ' in-place XOR unmasking, RFC6455 section-5.3
                    Call ApplyMask(payload, maskKey)
                End If

                frame.Payload = payload
            End If

            Return frame
        End Function

        ''' <summary>
        ''' apply the 4 bytes masking key onto the payload buffer with the in-place
        ''' XOR transform. the masking transform is symmetric, so this method could
        ''' be used for both of the masking and the unmasking operation.
        ''' </summary>
        ''' <param name="payload">the payload buffer which will be modified in-place</param>
        ''' <param name="maskKey">the 4 bytes masking key</param>
        Public Shared Sub ApplyMask(payload As Byte(), maskKey As Byte())
            For i As Integer = 0 To payload.Length - 1
                payload(i) = payload(i) Xor maskKey(i And 3)
            Next
        End Sub

        ''' <summary>
        ''' read the exact required number of bytes from the given network stream.
        ''' </summary>
        ''' <returns>
        ''' returns nothing when the stream reaches its end before the required
        ''' number of bytes could be filled up.
        ''' </returns>
        Private Shared Function readExact(input As Stream, count As Integer) As Byte()
            Dim buffer As Byte() = New Byte(count - 1) {}
            Dim offset As Integer = 0

            Do While offset < count
                Dim read As Integer = input.Read(buffer, offset, count - offset)

                If read <= 0 Then
                    ' remote client disconnected in the middle of a data frame
                    Return Nothing
                Else
                    offset += read
                End If
            Loop

            Return buffer
        End Function

        ''' <summary>
        ''' encode a websocket data frame which is sent from the server to the client.
        ''' </summary>
        ''' <remarks>
        ''' RFC6455 section-5.1: a server must not mask any frames that it sends to
        ''' the client, so the mask bit of the encoded frame is always zero here.
        ''' </remarks>
        ''' <param name="opcode">the payload data type of the encoded data frame</param>
        ''' <param name="payload">the raw payload data, a null value is treated as an empty payload</param>
        ''' <param name="fin">is the encoded data frame the final fragment of an application message?</param>
        ''' <returns>the encoded raw bytes of a single websocket data frame</returns>
        Public Shared Function EncodeFrame(opcode As WebSocketOpcode, payload As Byte(), Optional fin As Boolean = True) As Byte()
            Dim data As Byte() = If(payload, New Byte() {})
            Dim length As Integer = data.Length
            Dim headerSize As Integer

            If length <= 125 Then
                headerSize = 2
            ElseIf length <= UShort.MaxValue Then
                headerSize = 4
            Else
                headerSize = 10
            End If

            Dim buffer As Byte() = New Byte(headerSize + length - 1) {}

            buffer(0) = CByte(If(fin, &H80, &H0) Or (CByte(opcode) And &HF))

            If headerSize = 2 Then
                buffer(1) = CByte(length)
            ElseIf headerSize = 4 Then
                buffer(1) = 126
                buffer(2) = CByte((length >> 8) And &HFF)
                buffer(3) = CByte(length And &HFF)
            Else
                buffer(1) = 127

                Dim len64 As Long = length

                For i As Integer = 0 To 7
                    buffer(2 + i) = CByte((len64 >> ((7 - i) * 8)) And &HFF)
                Next
            End If

            If length > 0 Then
                Call Array.Copy(data, 0, buffer, headerSize, length)
            End If

            Return buffer
        End Function

        ''' <summary>
        ''' create the payload buffer of a websocket close control frame.
        ''' </summary>
        ''' <param name="code">the close status code which is defined in RFC6455 section-7.4.1</param>
        ''' <param name="reason">an optional utf8 encoded human readable close reason text</param>
        Public Shared Function EncodeClosePayload(code As WebSocketCloseCode, Optional reason As String = Nothing) As Byte()
            Dim value As UShort = CUShort(code)
            Dim text As Byte() = If(reason.StringEmpty, New Byte() {}, Encoding.UTF8.GetBytes(reason))

            ' RFC6455 section-5.5: the close frame payload is limited to 125 bytes,
            ' 2 bytes of them has already been taken by the status code.
            If text.Length > MaxControlFramePayload - 2 Then
                text = text.Take(MaxControlFramePayload - 2).ToArray
            End If

            Dim buffer As Byte() = New Byte(text.Length + 1) {}

            buffer(0) = CByte((value >> 8) And &HFF)
            buffer(1) = CByte(value And &HFF)

            If text.Length > 0 Then
                Call Array.Copy(text, 0, buffer, 2, text.Length)
            End If

            Return buffer
        End Function

        ''' <summary>
        ''' parse the close status code from the payload of a close control frame
        ''' </summary>
        ''' <param name="payload">the payload data of a close control frame</param>
        ''' <returns>
        ''' returns <see cref="WebSocketCloseCode.NoStatusReceived"/> when the given
        ''' close frame carrys an empty payload data.
        ''' </returns>
        Public Shared Function ParseCloseCode(payload As Byte()) As WebSocketCloseCode
            If payload Is Nothing OrElse payload.Length < 2 Then
                Return WebSocketCloseCode.NoStatusReceived
            Else
                Return CType((CUShort(payload(0)) << 8) Or payload(1), WebSocketCloseCode)
            End If
        End Function

        ''' <summary>
        ''' parse the optional close reason text from the payload of a close control frame
        ''' </summary>
        Public Shared Function ParseCloseReason(payload As Byte()) As String
            If payload Is Nothing OrElse payload.Length <= 2 Then
                Return ""
            Else
                Return Encoding.UTF8.GetString(payload, 2, payload.Length - 2)
            End If
        End Function
    End Class

    ''' <summary>
    ''' an error which indicates that the websocket peer has violated the RFC6455
    ''' protocol specification, the connection should be closed with the
    ''' <see cref="CloseCode"/> status code when this error occurs.
    ''' </summary>
    Public Class WebSocketProtocolException : Inherits Exception

        ''' <summary>
        ''' the close status code that should be sent to the remote peer
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property CloseCode As WebSocketCloseCode

        Sub New(code As WebSocketCloseCode, message As String)
            MyBase.New(message)
            Me._CloseCode = code
        End Sub
    End Class
End Namespace
