#Region "Microsoft.VisualBasic::4bceaf76bb3058282e4f8f2758302ca3, WebCloud\SMRUCC.HTTPInternal\Core\WebSocket.vb"

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

'     Class WebSocket
' 
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.Net.Sockets
Imports System.Net
Imports System
Imports System.Text
Imports System.Text.RegularExpressions

Namespace Core

    ' https://developer.mozilla.org/zh-CN/docs/Web/API/WebSockets_API/WebSocket_Server_Vb.NET

    Public Class WebSocketClient
        Dim _TcpClient As System.Net.Sockets.TcpClient

        Public Delegate Sub OnClientDisconnectDelegateHandler()
        Public Event onClientDisconnect As OnClientDisconnectDelegateHandler


        Sub New(ByVal tcpClient As System.Net.Sockets.TcpClient)
            Me._TcpClient = tcpClient
        End Sub


        Function isConnected() As Boolean
            Return Me._TcpClient.Connected
        End Function


        Sub HandShake()
            Dim stream As NetworkStream = Me._TcpClient.GetStream()
            Dim bytes As Byte()
            Dim data As String

            While Me._TcpClient.Connected
                While (stream.DataAvailable)
                    ReDim bytes(Me._TcpClient.Client.Available)
                    stream.Read(bytes, 0, bytes.Length)
                    data = System.Text.Encoding.UTF8.GetString(bytes)

                    If (New System.Text.RegularExpressions.Regex("^GET").IsMatch(data)) Then

                        Dim response As Byte() = System.Text.Encoding.UTF8.GetBytes("HTTP/1.1 101 Switching Protocols" & Environment.NewLine & "Connection: Upgrade" & Environment.NewLine & "Upgrade: websocket" & Environment.NewLine & "Sec-WebSocket-Accept: " & Convert.ToBase64String(System.Security.Cryptography.SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(New Regex("Sec-WebSocket-Key: (.*)").Match(data).Groups(1).Value.Trim() & "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"))) & Environment.NewLine & Environment.NewLine)

                        stream.Write(response, 0, response.Length)
                        Exit Sub
                    Else
                        'We're going to disconnect the client here, because he's not handshacking properly (or at least to the scope of this code sample)
                        Me._TcpClient.Close() 'The next While Me._TcpClient.Connected Loop Check should fail.. and raise the onClientDisconnect Event Thereafter
                    End If
                End While
            End While
            RaiseEvent onClientDisconnect()
        End Sub


        Sub CheckForDataAvailability()
            If (Me._TcpClient.GetStream().DataAvailable) Then
                Dim stream As NetworkStream = Me._TcpClient.GetStream()
                Dim frameCount = 2
                Dim bytes As Byte()
                Dim data As String
                ReDim bytes(Me._TcpClient.Client.Available)
                stream.Read(bytes, 0, bytes.Length) 'Read the stream, don't close it.. 

                Try
                    Dim length As UInteger = bytes(1) - 128 'this should obviously be a byte (unsigned 8bit value)

                    If length > -1 Then
                        If length = 126 Then
                            length = 4
                        ElseIf length = 127 Then
                            length = 10
                        End If
                    End If

                    'the following is very inefficient and likely unnecessary.. 
                    'the main purpose is to just get the lower 4 bits of byte(0) - which is the OPCODE

                    Dim value As Integer = bytes(0)
                    Dim bitArray As BitArray = New BitArray(8)

                    For c As Integer = 0 To 7 Step 1
                        If value - (2 ^ (7 - c)) >= 0 Then
                            bitArray.Item(c) = True
                            value -= (2 ^ (7 - c))
                        Else
                            bitArray.Item(c) = False
                        End If
                    Next


                    Dim FRRR_OPCODE As String = ""

                    For Each bit As Boolean In bitArray
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
                    Dim opCode As Integer = Convert.ToInt32(FRRR_OPCODE.Substring(4, 4), 2)



                    Dim decoded(bytes.Length - (frameCount + 4)) As Byte
                    Dim key As Byte() = {bytes(frameCount), bytes(frameCount + 1), bytes(frameCount + 2), bytes(frameCount + 3)}

                    Dim j As Integer = 0
                    For i As Integer = (frameCount + 4) To (bytes.Length - 2) Step 1
                        decoded(j) = Convert.ToByte((bytes(i) Xor masks(j Mod 4)))
                        j += 1
                    Next



                    Select Case opCode
                        Case Is = 1
                            'Text Data Sent From Client

                            data = System.Text.Encoding.UTF8.GetString(decoded)
                            'handle this data

                            Dim Payload As Byte() = System.Text.Encoding.UTF8.GetBytes("Text Recieved")
                            Dim FRRROPCODE As Byte = Convert.ToByte("10000001", 2) 'FIN is set, and OPCODE is 1 or Text
                            Dim header As Byte() = {FRRROPCODE, Convert.ToByte(Payload.Length)}


                            Dim ResponseData As Byte()
                            ReDim ResponseData((header.Length + Payload.Length) - 1)
                            'NOTEWORTHY: if you Redim ResponseData(header.length + Payload.Length).. you'll add a 0 value byte at the end of the response data.. 
                            'which tells the client that your next stream write will be a continuation frame..

                            Dim index As Integer = 0

                            Buffer.BlockCopy(header, 0, ResponseData, index, header.Length)
                            index += header.Length

                            Buffer.BlockCopy(Payload, 0, ResponseData, index, Payload.Length)
                            index += Payload.Length
                            stream.Write(ResponseData, 0, ResponseData.Length)
                        Case Is = 2
                            '// Binary Data Sent From Client 
                            data = System.Text.Encoding.UTF8.GetString(decoded)
                            Dim response As Byte() = System.Text.Encoding.UTF8.GetBytes("Binary Recieved")
                            stream.Write(response, 0, response.Length)
                        Case Is = 9 '// Ping Sent From Client 
                        Case Is = 10 '// Pong Sent From Client 
                        Case Else '// Improper opCode.. disconnect the client 
                            _TcpClient.Close()
                            RaiseEvent onClientDisconnect()
                    End Select
                Catch ex As Exception
                    _TcpClient.Close()
                    RaiseEvent onClientDisconnect()
                End Try
            End If
        End Sub
    End Class

    Public Class WebSocket
        Inherits System.Net.Sockets.TcpListener

        Delegate Sub OnClientConnectDelegate(ByVal sender As Object, ByRef Client As WebSocketClient)
        Event OnClientConnect As OnClientConnectDelegate


        Dim WithEvents PendingCheckTimer As Timers.Timer = New Timers.Timer(500)
        Dim WithEvents ClientDataAvailableTimer As Timers.Timer = New Timers.Timer(50)
        Property ClientCollection As New List(Of WebSocketClient)



        Sub New(ByVal url As String, ByVal port As Integer)
            MyBase.New(IPAddress.Parse(url), port)
        End Sub


        Sub startServer()
            Me.Start()
            PendingCheckTimer.Start()
        End Sub



        Sub Client_Connected(ByVal sender As Object, ByRef client As WebSocketClient) Handles Me.OnClientConnect
            Me.ClientCollection.Add(client)
            AddHandler client.onClientDisconnect, AddressOf Client_Disconnected
            client.HandShake()
            ClientDataAvailableTimer.Start()
        End Sub


        Sub Client_Disconnected()

        End Sub


        Function isClientDisconnected(ByVal client As WebSocketClient) As Boolean
            isClientDisconnected = False
            If Not client.isConnected Then
                Return True
            End If
        End Function


        Function isClientConnected(ByVal client As WebSocketClient) As Boolean
            isClientConnected = False
            If client.isConnected Then
                Return True
            End If
        End Function


        Private Sub PendingCheckTimer_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles PendingCheckTimer.Elapsed
            If Pending() Then
                RaiseEvent OnClientConnect(Me, New WebSocketClient(Me.AcceptTcpClient()))
            End If
        End Sub


        Private Sub ClientDataAvailableTimer_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles ClientDataAvailableTimer.Elapsed
            Me.ClientCollection.RemoveAll(AddressOf isClientDisconnected)
            If Me.ClientCollection.Count < 1 Then ClientDataAvailableTimer.Stop()

            For Each Client As WebSocketClient In Me.ClientCollection
                Client.CheckForDataAvailability()
            Next
        End Sub
    End Class
End Namespace
