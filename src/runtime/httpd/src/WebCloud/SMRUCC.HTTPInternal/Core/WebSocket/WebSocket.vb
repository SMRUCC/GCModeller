#Region "Microsoft.VisualBasic::ff316fa7b791d33de5c351192d2390dd, WebCloud\SMRUCC.HTTPInternal\Core\WebSocket\WebSocket.vb"

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

    '     Class WsServer
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: isClientConnected, isClientDisconnected, Run
    ' 
    '         Sub: Client_Connected, Client_Disconnected, ClientDataAvailableTimer_Elapsed, PendingCheckTimer_Elapsed, StartServer
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Net
Imports System.Net.Sockets
Imports System.Threading
Imports System.Timers
Imports Microsoft.VisualBasic.ComponentModel
Imports Tick = System.Timers.Timer

Namespace Core.WebSocket

    ''' <summary>
    ''' WebSocket server
    ''' </summary>
    Public Class WsServer : Inherits TcpListener
        Implements ITaskDriver

        Public Event OnClientConnect As OnClientConnectDelegate

        Dim WithEvents pendingCheckTimer As New Tick(100)

        ''' <summary>
        ''' Use this timer thread for read new data that send from client
        ''' </summary>
        Dim WithEvents listener As New Tick(50)

        Dim connected As New List(Of WsProcessor)
        Dim activator As WebsocketActivator

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="url"></param>
        ''' <param name="port"></param>
        ''' <param name="activator">
        ''' Function(<see cref="TcpClient"/>) As <see cref="WsProcessor"/>
        ''' </param>
        Sub New(url$, port%, activator As WebsocketActivator)
            MyBase.New(IPAddress.Parse(url), port)

            ' 这个函数指针描述了如何创建一个业务逻辑对象实例
            Me.activator = activator
        End Sub

        Sub StartServer()
            Me.Start()
            pendingCheckTimer.Start()
        End Sub

        Public Function Run() As Integer Implements ITaskDriver.Run
            Call StartServer()

            Do While Me.Active
                Call Thread.Sleep(100)
            Loop

            Return 0
        End Function

        Public Sub Client_Connected(sender As Object, ByRef client As WsProcessor) Handles Me.OnClientConnect
            connected.Add(client)
            AddHandler client.onClientDisconnect, AddressOf Client_Disconnected
            client.HandShake()
            listener.Start()
        End Sub

        Private Sub Client_Disconnected()

        End Sub

        Private Function isClientDisconnected(client As WsProcessor) As Boolean
            isClientDisconnected = False

            If Not client.isConnected Then
                Return True
            End If
        End Function

        Private Function isClientConnected(client As WsProcessor) As Boolean
            isClientConnected = False

            If client.isConnected Then
                Return True
            End If
        End Function

        Private Sub PendingCheckTimer_Elapsed(sender As Object, e As ElapsedEventArgs) Handles pendingCheckTimer.Elapsed
            If Pending() Then
                RaiseEvent OnClientConnect(Me, Me.activator(Me.AcceptTcpClient))
            End If
        End Sub

        Private Sub ClientDataAvailableTimer_Elapsed(sender As Object, e As ElapsedEventArgs) Handles listener.Elapsed
            Call connected.RemoveAll(AddressOf isClientDisconnected)

            If Me.connected.Count < 1 Then
                Call listener.Stop()
            End If

            For Each client As WsProcessor In Me.connected
                client.CheckForDataAvailability()
            Next
        End Sub
    End Class
End Namespace
