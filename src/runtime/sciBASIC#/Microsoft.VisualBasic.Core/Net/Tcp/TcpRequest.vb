﻿#Region "Microsoft.VisualBasic::a3473a28b510eade8ad556ad3a6366ba, Microsoft.VisualBasic.Core\Net\Tcp\TcpRequest.vb"

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

    '     Class TcpRequest
    ' 
    '         Properties: LocalIPAddress
    ' 
    '         Constructor: (+4 Overloads) Sub New
    '         Function: LocalConnection, OperationTimeOut, (+2 Overloads) SendMessage, ToString
    '         Delegate Function
    ' 
    '             Function: (+4 Overloads) SendMessage
    ' 
    '             Sub: __send, ConnectCallback, (+2 Overloads) Dispose, Receive, ReceiveCallback
    '                  SendCallback
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Net
Imports System.Net.Sockets
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.ExceptionExtensions
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Net.Protocols
Imports TcpEndPoint = System.Net.IPEndPoint

Namespace Net.Tcp

    ''' <summary>
    ''' The server socket should returns some data string to this client or this client will stuck at the <see cref="SendMessage"></see> function.
    ''' (服务器端<see cref="TcpServicesSocket"></see>必须要返回数据，否则本客户端会在<see cref="SendMessage
    ''' "></see>函数位置一直处于等待的状态)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class TcpRequest : Implements IDisposable

#Region "Internal Fields"

        ''' <summary>
        ''' The port number for the remote device.
        ''' </summary>
        ''' <remarks></remarks>
        Dim port As Integer

        ''' <summary>
        ''' The response from the remote device.
        ''' </summary>
        ''' <remarks></remarks>
        Dim response As Byte()

        ''' <summary>
        ''' ' ManualResetEvent instances signal completion.
        ''' </summary>
        ''' <remarks></remarks>
        Dim connectDone As ManualResetEvent
        Dim sendDone As ManualResetEvent
        Dim receiveDone As ManualResetEvent
        Dim __exceptionHandler As ExceptionHandler
        Dim remoteHost As String

        ''' <summary>
        ''' Remote End Point
        ''' </summary>
        ''' <remarks></remarks>
        Protected ReadOnly remoteEP As TcpEndPoint
#End Region

        ''' <summary>
        ''' Gets the IP address of this local machine.
        ''' (获取本机对象的IP地址，请注意这个属性获取得到的仅仅是本机在局域网内的ip地址，假若需要获取得到公网IP地址，还需要外部服务器的帮助才行)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property LocalIPAddress As String
            Get
#Disable Warning
                Dim IP As System.Net.IPAddress = Dns.Resolve(Dns.GetHostName).AddressList(0)
                Dim IPAddr As String = IP.ToString
#Enable Warning
                Return IPAddr
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"Remote_connection={remoteHost}:{port},  local_host={LocalIPAddress}"
        End Function

        Sub New(remoteDevice As TcpEndPoint, Optional exceptionHandler As ExceptionHandler = Nothing)
            Call Me.New(remoteDevice.Address.ToString, remoteDevice.Port, exceptionHandler)
        End Sub

        Sub New(remoteDevice As IPEndPoint, Optional exceptionHandler As ExceptionHandler = Nothing)
            Call Me.New(remoteDevice.IPAddress, remoteDevice.Port, exceptionHandler)
        End Sub

        Shared ReadOnly defaultHandler As New DefaultValue(Of ExceptionHandler)(AddressOf VBDebugger.PrintException)

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="client">
        ''' Copy the TCP client connection profile data from this object.
        ''' (从本客户端对象之中复制出连接配置参数以进行初始化操作)
        ''' </param>
        ''' <param name="exceptionHandler"></param>
        ''' <remarks></remarks>
        Sub New(client As TcpRequest, Optional exceptionHandler As ExceptionHandler = Nothing)
            remoteHost = client.remoteHost
            port = client.port
            __exceptionHandler = exceptionHandler Or defaultHandler
            remoteEP = New TcpEndPoint(System.Net.IPAddress.Parse(remoteHost), port)
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="remotePort"></param>
        ''' <param name="exceptionHandler">
        ''' Public <see cref="System.Delegate"/> Sub ExceptionHandler(ex As <see cref="Exception"/>)
        ''' </param>
        ''' <remarks></remarks>
        Sub New(hostName$, remotePort%, Optional exceptionHandler As ExceptionHandler = Nothing)
            remoteHost = hostName

            If String.Equals(remoteHost, "localhost", StringComparison.OrdinalIgnoreCase) Then
                remoteHost = LocalIPAddress
            End If

            port = remotePort
            __exceptionHandler = exceptionHandler Or defaultHandler
            remoteEP = New TcpEndPoint(System.Net.IPAddress.Parse(remoteHost), port)
        End Sub

        ''' <summary>
        ''' 初始化一个在本机进行进程间通信的Socket对象
        ''' </summary>
        ''' <param name="localPort"></param>
        ''' <param name="exceptionHandler"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function LocalConnection(localPort%, Optional exceptionHandler As ExceptionHandler = Nothing) As TcpRequest
            Return New TcpRequest(LocalIPAddress, localPort, exceptionHandler)
        End Function

        ''' <summary>
        ''' 判断服务器所返回来的数据是否为操作超时
        ''' </summary>
        ''' <param name="str"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function OperationTimeOut(str As String) As Boolean
            Return String.Equals(str, NetResponse.RFC_REQUEST_TIMEOUT.GetUTF8String)
        End Function

        ''' <summary>
        ''' Returns the server reply.(假若操作超时的话，则会返回<see cref="NetResponse.RFC_REQUEST_TIMEOUT"></see>)
        ''' </summary>
        ''' <param name="Message"></param>
        ''' <param name="OperationTimeOut">操作超时的时间长度，默认为30秒</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SendMessage(Message As String,
                                    Optional OperationTimeOut As Integer = 30 * 1000,
                                    Optional OperationTimeoutHandler As Action = Nothing) As String
            Dim request As New RequestStream(0, 0, Message)
            Dim response = SendMessage(request, OperationTimeOut, OperationTimeoutHandler).GetUTF8String
            Return response
        End Function 'Main

        ''' <summary>
        ''' Returns the server reply.(假若操作超时的话，则会返回<see cref="NetResponse.RFC_REQUEST_TIMEOUT"></see>，
        ''' 请注意，假若目标服务器启用了ssl加密服务的话，假若这个请求是明文数据，则服务器会直接拒绝请求返回<see cref="HTTP_RFC.RFC_NO_CERT"/> 496错误代码，
        ''' 所以调用前请确保参数<paramref name="Message"/>已经使用证书加密)
        ''' </summary>
        ''' <param name="Message"></param>
        ''' <param name="timeOut">操作超时的时间长度，默认为30秒</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SendMessage(Message As RequestStream,
                                    Optional timeout% = 30 * 1000,
                                    Optional timeoutHandler As Action = Nothing) As RequestStream

            Dim response As RequestStream = Nothing
            Dim bResult As Boolean = Parallel.OperationTimeOut(
                AddressOf SendMessage,
                [In]:=Message,
                Out:=response,
                TimeOut:=timeout / 1000)

            If bResult Then
                If Not timeoutHandler Is Nothing Then Call timeoutHandler() '操作超时了

                If Not connectDone Is Nothing Then Call connectDone.Set()  ' ManualResetEvent instances signal completion.
                If Not sendDone Is Nothing Then Call sendDone.Set()
                If Not receiveDone Is Nothing Then Call receiveDone.Set() '中断服务器的连接

                Dim ex As Exception = New Exception("[OPERATION_TIME_OUT] " & Message.GetUTF8String)
                Dim ret As New RequestStream(0, HTTP_RFC.RFC_REQUEST_TIMEOUT, "HTTP/408  " & Me.ToString)
                Call __exceptionHandler(New Exception(ret.GetUTF8String, ex))
                Return ret
            Else
                Return response
            End If
        End Function

        Public Delegate Function SendMessageInvoke(Message As String) As String

        Public Function SendMessage(Message As String, Callback As Action(Of String)) As IAsyncResult
            Dim SendMessageClient As New TcpRequest(Me, exceptionHandler:=Me.__exceptionHandler)
            Return (Sub() Call Callback(SendMessageClient.SendMessage(Message))).BeginInvoke(Nothing, Nothing)
        End Function

        ''' <summary>
        ''' This function returns the server reply for this request <paramref name="Message"></paramref>.
        ''' </summary>
        ''' <param name="Message">The client request to the server.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SendMessage(Message As String) As String
            Dim byteData As Byte() = System.Text.Encoding.UTF8.GetBytes(Message)
            byteData = SendMessage(byteData)
            Dim response As String = New RequestStream(byteData).GetUTF8String
            Return response
        End Function 'Main

        ''' <summary>
        ''' Send a request message to the remote server.
        ''' </summary>
        ''' <param name="Message"></param>
        ''' <returns></returns>
        Public Function SendMessage(Message As RequestStream) As RequestStream
            Dim byteData As Byte() = SendMessage(Message.Serialize)

            If RequestStream.IsAvaliableStream(byteData) Then
                Return New RequestStream(byteData)
            Else
                Return New RequestStream(0, 0, byteData)
            End If
        End Function

        ''' <summary>
        ''' 最底层的消息发送函数
        ''' </summary>
        ''' <param name="Message"></param>
        ''' <returns></returns>
        Public Function SendMessage(Message As Byte()) As Byte()
            If Not RequestStream.IsAvaliableStream(Message) Then
                Message = New RequestStream(0, 0, Message).Serialize
            End If

            connectDone = New ManualResetEvent(False) ' ManualResetEvent instances signal completion.
            sendDone = New ManualResetEvent(False)
            receiveDone = New ManualResetEvent(False)
            response = Nothing

            ' Establish the remote endpoint for the socket.
            ' For this example use local machine.
            ' Create a TCP/IP socket.
            Dim client As New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            Call client.Bind(New System.Net.IPEndPoint(System.Net.IPAddress.Any, 0))
            ' Connect to the remote endpoint.
            Call client.BeginConnect(remoteEP, New AsyncCallback(AddressOf ConnectCallback), client)
            ' Wait for connect.
            Call connectDone.WaitOne()
            ' Send test data to the remote device.
            Call __send(client, Message)
            Call sendDone.WaitOne()

            ' Receive the response from the remote device.
            Call Receive(client)
            Call receiveDone.WaitOne()

            On Error Resume Next

            ' Release the socket.
            Call client.Shutdown(SocketShutdown.Both)
            Call client.Close()

            Return response
        End Function

        Private Sub ConnectCallback(ar As IAsyncResult)

            ' Retrieve the socket from the state object.
            Dim client As Socket = DirectCast(ar.AsyncState, Socket)

            ' Complete the connection.
            Try
                client.EndConnect(ar)
                ' Signal that the connection has been made.
                connectDone.Set()
            Catch ex As Exception
                Call __exceptionHandler(ex)
            End Try
        End Sub 'ConnectCallback

        ''' <summary>
        ''' An exception of type '<see cref="SocketException"/>' occurred in System.dll but was not handled in user code
        ''' Additional information: A request to send or receive data was disallowed because the socket is not connected and
        ''' (when sending on a datagram socket using a sendto call) no address was supplied
        ''' </summary>
        ''' <param name="client"></param>
        Private Sub Receive(client As Socket)
            ' Create the state object.
            Dim state As New StateObject With {
                .workSocket = client
            }

            ' Begin receiving the data from the remote device.
            Try
                Call client.BeginReceive(state.readBuffer, 0, StateObject.BufferSize, 0, New AsyncCallback(AddressOf ReceiveCallback), state)
            Catch ex As Exception
                Call Me.__exceptionHandler(ex)
            End Try
        End Sub 'Receive

        ''' <summary>
        ''' Retrieve the state object and the client socket from the asynchronous state object.
        ''' </summary>
        ''' <param name="ar"></param>
        Private Sub ReceiveCallback(ar As IAsyncResult)
            Dim state As StateObject = DirectCast(ar.AsyncState, StateObject)
            Dim client As Socket = state.workSocket
            Dim bytesRead As Integer

            Try
                ' Read data from the remote device.
                bytesRead = client.EndReceive(ar)
            Catch ex As Exception
                Call __exceptionHandler(ex)
                GoTo EX_EXIT
            End Try

            If bytesRead > 0 Then
                ' There might be more data, so store the data received so far.
                state.ChunkBuffer.AddRange(state.readBuffer.Takes(bytesRead))
                ' Get the rest of the data.
                client.BeginReceive(state.readBuffer, 0, StateObject.BufferSize, 0, New AsyncCallback(AddressOf ReceiveCallback), state)
            Else
                ' All the data has arrived; put it in response.
                If state.ChunkBuffer.Count > 1 Then
                    response = state.ChunkBuffer.ToArray
                Else
EX_EXIT:            response = Nothing
                End If
                ' Signal that all bytes have been received.
                Call receiveDone.Set()
            End If
        End Sub

        ''' <summary>
        ''' ????
        ''' An exception of type 'System.Net.Sockets.SocketException' occurred in System.dll but was not handled in user code
        ''' Additional information: A request to send or receive data was disallowed because the socket is not connected and
        ''' (when sending on a datagram socket using a sendto call) no address was supplied
        ''' </summary>
        ''' <param name="client"></param>
        ''' <param name="byteData"></param>
        ''' <remarks></remarks>
        Private Sub __send(client As Socket, byteData As Byte())

            ' Begin sending the data to the remote device.
            Try
                Call client.BeginSend(byteData, 0, byteData.Length, 0, New AsyncCallback(AddressOf SendCallback), client)
            Catch ex As Exception
                Call Me.__exceptionHandler(ex)
            End Try
        End Sub 'Send

        Private Sub SendCallback(ar As IAsyncResult)

            ' Retrieve the socket from the state object.
            Dim client As Socket = DirectCast(ar.AsyncState, Socket)
            ' Complete sending the data to the remote device.
            Dim bytesSent As Integer = client.EndSend(ar)
            'Console.WriteLine("Sent {0} bytes to server.", bytesSent)
            ' Signal that all bytes have been sent.
            sendDone.Set()
        End Sub 'SendCallback

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Call connectDone.Set()  ' ManualResetEvent instances signal completion.
                    Call sendDone.Set()
                    Call receiveDone.Set() '中断服务器的连接
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(      disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(      disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class 'AsynchronousClient
End Namespace
