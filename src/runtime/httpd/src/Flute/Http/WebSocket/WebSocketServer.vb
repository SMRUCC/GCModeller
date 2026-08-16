#Region "Microsoft.VisualBasic::8aff677229343f1057b5222ffd9da681, src\Flute\Http\WebSocket\WebSocketServer.vb"

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

    '   Total Lines: 204
    '    Code Lines: 89 (43.63%)
    ' Comment Lines: 87 (42.65%)
    '    - Xml Docs: 96.55%
    ' 
    '   Blank Lines: 28 (13.73%)
    '     File Size: 8.48 KB


    '     Class WebSocketServer
    ' 
    '         Properties: Count, Port, WebSocket
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: (+2 Overloads) Broadcast, DefaultRoute, (+2 Overloads) Route, Run, RunAsync
    '                   ToString
    ' 
    '         Sub: (+2 Overloads) Dispose, handleHttpRequest, Shutdown
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Flute.Http.Configurations
Imports Flute.Http.Core.Message
Imports Microsoft.VisualBasic.Net.Http

Namespace Core.WebSocket

    ''' <summary>
    ''' a standalone websocket server which serves the RFC6455 websocket protocol
    ''' only, this server class is a thin wrapper of the <see cref="HttpSocket"/>
    ''' http server: the websocket protocol handshake is a http upgrade request in
    ''' its nature, so the http request parsing stage is always required even for a
    ''' pure websocket server endpoint.
    ''' </summary>
    ''' <remarks>
    ''' this server class shares the same <see cref="WebSocketConnection"/> data
    ''' frame implementation and the same <see cref="WebSocketManager"/> connection
    ''' registry with the <see cref="HttpServer"/> websocket integration, so the
    ''' application code will get an identical behaviour on the both of these two
    ''' websocket接入方式.
    ''' </remarks>
    ''' <example>
    ''' Dim server As New WebSocketServer(port:=8080)
    ''' 
    ''' Call server.Route("/ws/echo", WebSocketHandler.Echo)
    ''' Call server.RunAsync()
    ''' </example>
    Public Class WebSocketServer : Implements IDisposable

        ''' <summary>
        ''' the underlying http server which handles the websocket upgrade handshake
        ''' </summary>
        Private ReadOnly m_http As HttpSocket

        ''' <summary>
        ''' the websocket connection manager of current server, which could be used
        ''' for the connection routing and the message broadcast operation.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property WebSocket As WebSocketManager
            Get
                Return m_http.WebSocket
            End Get
        End Property

        ''' <summary>
        ''' the tcp port which current websocket server is listening on
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Port As Integer
            Get
                Return m_http.localPort
            End Get
        End Property

        ''' <summary>
        ''' the total number of the active websocket connections on current server
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Count As Integer
            Get
                Return WebSocket.Count
            End Get
        End Property

        ''' <summary>
        ''' create a new standalone websocket server object
        ''' </summary>
        ''' <param name="port">the tcp port which current server will be listening on</param>
        ''' <param name="threads">the max number of the concurrent connections, a negative value means the processor count of current machine</param>
        ''' <param name="configs">an optional server configuration object</param>
        Sub New(port As Integer,
                Optional threads As Integer = -1,
                Optional configs As Configuration = Nothing)

            Dim settings As Configuration = If(configs, Configuration.Default)

            ' the websocket protocol upgrade handshake must be enabled on the
            ' underlying http server for a pure websocket server endpoint.
            settings.websocket_enabled = True

            m_http = New HttpSocket(AddressOf handleHttpRequest, port, threads, settings)
        End Sub

        ''' <summary>
        ''' any of the regular http request which hits this pure websocket server
        ''' endpoint will be rejected with a ``400 Bad Request`` response, as the
        ''' websocket upgrade handshake request has already been intercepted by the
        ''' http request processor before this handler is reached.
        ''' </summary>
        Private Sub handleHttpRequest(request As HttpRequest, response As HttpResponse)
            Call response.WriteError(HTTP_RFC.RFC_BAD_REQUEST, "This server endpoint accepts the websocket protocol connection only.")
        End Sub

        ''' <summary>
        ''' associate an application level message handler with a specific url path
        ''' </summary>
        ''' <param name="path">the url path of the websocket endpoint, i.e. ``/ws/chat``</param>
        ''' <param name="handler">the application level message handler</param>
        ''' <returns>current server object for the method chain style invoke</returns>
        Public Function Route(path As String, handler As IWebSocketHandler) As WebSocketServer
            Call WebSocket.Route(path, handler)
            Return Me
        End Function

        ''' <summary>
        ''' associate a set of the event function pointers with a specific url path
        ''' </summary>
        ''' <returns>current server object for the method chain style invoke</returns>
        Public Function Route(path As String,
                              message As OnMessageHandler,
                              Optional connect As OnConnectHandler = Nothing,
                              Optional [close] As OnCloseHandler = Nothing,
                              Optional [error] As OnErrorHandler = Nothing) As WebSocketServer

            Call WebSocket.Route(path, message, connect, [close], [error])
            Return Me
        End Function

        ''' <summary>
        ''' set the fallback application message handler which will be used when the
        ''' requested url path hits none of the registered routes.
        ''' </summary>
        ''' <returns>current server object for the method chain style invoke</returns>
        Public Function DefaultRoute(handler As IWebSocketHandler) As WebSocketServer
            Call WebSocket.DefaultRoute(handler)
            Return Me
        End Function

        ''' <summary>
        ''' push a text message to all of the active websocket connections
        ''' </summary>
        ''' <returns>the number of the clients which received the given message successfully</returns>
        Public Function Broadcast(text As String) As Integer
            Return WebSocket.Broadcast(text)
        End Function

        ''' <summary>
        ''' push a text message to all of the active websocket connections which is
        ''' established on a specific url path.
        ''' </summary>
        ''' <returns>the number of the clients which received the given message successfully</returns>
        Public Function Broadcast(path As String, text As String) As Integer
            Return WebSocket.Broadcast(path, text)
        End Function

        ''' <summary>
        ''' run current websocket server on the caller thread, this method will be
        ''' blocked until the server is shutdown.
        ''' </summary>
        Public Function Run() As Integer
            Return m_http.Run()
        End Function

        ''' <summary>
        ''' start current websocket server on a background thread, the caller thread
        ''' will not be blocked by this method.
        ''' </summary>
        ''' <returns>current server object for the method chain style invoke</returns>
        Public Function RunAsync() As WebSocketServer
            Call Task.Run(Sub()
                              Try
                                  Call m_http.Run()
                              Catch ex As Exception
                                  Call App.LogException(ex)
                              End Try
                          End Sub)

            Return Me
        End Function

        ''' <summary>
        ''' close all of the active websocket connections and then shutdown current
        ''' websocket server.
        ''' </summary>
        Public Sub Shutdown()
            Call m_http.Shutdown()
        End Sub

        ''' <summary>
        ''' the string representation of this server: its listen address and port.
        ''' </summary>
        ''' <returns>a "websocket://0.0.0.0:port/" description string.</returns>
        Public Overrides Function ToString() As String
            Return $"websocket://0.0.0.0:{Port}/"
        End Function

#Region "IDisposable Support"

        Private disposedValue As Boolean

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    Call Shutdown()
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
