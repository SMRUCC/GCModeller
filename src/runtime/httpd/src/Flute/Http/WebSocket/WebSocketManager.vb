#Region "Microsoft.VisualBasic::41d801bd9849fcb14e08b6ce1af66933, src\Flute\Http\WebSocket\WebSocketManager.vb"

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

    '   Total Lines: 311
    '    Code Lines: 136 (43.73%)
    ' Comment Lines: 135 (43.41%)
    '    - Xml Docs: 97.04%
    ' 
    '   Blank Lines: 40 (12.86%)
    '     File Size: 13.81 KB


    '     Class WebSocketManager
    ' 
    '         Properties: Count, Routes
    ' 
    '         Function: (+2 Overloads) Broadcast, (+2 Overloads) BroadcastBinary, broadcastFrame, CanHandle, DefaultRoute
    '                   GetConnection, (+2 Overloads) GetConnections, NormalizePath, RemoveRoute, ResolveHandler
    '                   (+2 Overloads) Route
    ' 
    '         Sub: CloseAll, Register, Unregister
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Concurrent
Imports System.Text

Namespace Core.WebSocket

    ''' <summary>
    ''' the websocket connection manager which keeps tracking of all of the active
    ''' websocket connections on current server, it also works as the routing table
    ''' for dispatch a websocket connection to its corresponding application level
    ''' message handler via the requested url path.
    ''' </summary>
    ''' <remarks>
    ''' all of the members of this class are thread safe, as the websocket
    ''' connections are running on the multiple background threads concurrently.
    ''' </remarks>
    Public Class WebSocketManager

        ''' <summary>
        ''' the url path routing table of the application level message handlers,
        ''' the routing path key is case-insensitive.
        ''' </summary>
        Private ReadOnly m_routes As New ConcurrentDictionary(Of String, IWebSocketHandler)(StringComparer.OrdinalIgnoreCase)
        ''' <summary>
        ''' all of the active websocket connections which is indexed via the
        ''' <see cref="WebSocketConnection.Id"/> value.
        ''' </summary>
        Private ReadOnly m_connections As New ConcurrentDictionary(Of String, WebSocketConnection)
        ''' <summary>
        ''' the fallback message handler which is used when no url path route hits
        ''' </summary>
        Private m_default As IWebSocketHandler

        ''' <summary>
        ''' the total number of the active websocket connections on current server
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Count As Integer
            Get
                Return m_connections.Count
            End Get
        End Property

        ''' <summary>
        ''' all of the url path routes which has an application message handler
        ''' associated with it.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Routes As String()
            Get
                Return m_routes.Keys.ToArray
            End Get
        End Property

        ''' <summary>
        ''' get all of the active websocket connections
        ''' </summary>
        ''' <returns>
        ''' a snapshot array of the active connections, a connection inside this
        ''' snapshot result may have already been closed when the caller code is
        ''' consuming this collection.
        ''' </returns>
        Public Function GetConnections() As WebSocketConnection()
            Return m_connections.Values.ToArray
        End Function

        ''' <summary>
        ''' get all of the active websocket connections on a specific url path
        ''' </summary>
        Public Function GetConnections(path As String) As WebSocketConnection()
            Dim route As String = NormalizePath(path)

            Return m_connections.Values _
                .Where(Function(c) c.Path.TextEquals(route)) _
                .ToArray
        End Function

        ''' <summary>
        ''' get a websocket connection via its unique connection id
        ''' </summary>
        ''' <returns>returns nothing when no connection could be found</returns>
        Public Function GetConnection(id As String) As WebSocketConnection
            Dim connection As WebSocketConnection = Nothing
            Return If(m_connections.TryGetValue(id, connection), connection, Nothing)
        End Function

#Region "Routing"

        ''' <summary>
        ''' associate an application level message handler with a specific url path.
        ''' </summary>
        ''' <param name="path">
        ''' the url path of the websocket endpoint, i.e. ``/ws/chat``. an existed
        ''' handler on the same url path will be replaced by the new given handler.
        ''' </param>
        ''' <param name="handler">the application level message handler</param>
        ''' <returns>current manager object for the method chain style invoke</returns>
        Public Function Route(path As String, handler As IWebSocketHandler) As WebSocketManager
            m_routes(NormalizePath(path)) = handler
            Return Me
        End Function

        ''' <summary>
        ''' associate a set of the event function pointers with a specific url path
        ''' </summary>
        Public Function Route(path As String,
                              message As OnMessageHandler,
                              Optional connect As OnConnectHandler = Nothing,
                              Optional [close] As OnCloseHandler = Nothing,
                              Optional [error] As OnErrorHandler = Nothing) As WebSocketManager

            Return Route(path, New WebSocketHandler(message, connect, [close], [error]))
        End Function

        ''' <summary>
        ''' set the fallback application message handler which will be used when the
        ''' requested url path hits none of the registered routes.
        ''' </summary>
        Public Function DefaultRoute(handler As IWebSocketHandler) As WebSocketManager
            m_default = handler
            Return Me
        End Function

        ''' <summary>
        ''' remove the application message handler of a specific url path
        ''' </summary>
        ''' <returns>returns false when no handler is associated with the given url path</returns>
        Public Function RemoveRoute(path As String) As Boolean
            Dim removed As IWebSocketHandler = Nothing
            Return m_routes.TryRemove(NormalizePath(path), removed)
        End Function

        ''' <summary>
        ''' resolve the application level message handler of a given request url path.
        ''' </summary>
        ''' <param name="path">the requested url path of the websocket handshake request</param>
        ''' <returns>
        ''' returns the fallback handler which is configured via the
        ''' <see cref="DefaultRoute"/> method when the given url path hits none of
        ''' the registered routes, and nothing will be returned when no fallback
        ''' handler is configured on current server.
        ''' </returns>
        Public Function ResolveHandler(path As String) As IWebSocketHandler
            Dim handler As IWebSocketHandler = Nothing

            If m_routes.TryGetValue(NormalizePath(path), handler) Then
                Return handler
            Else
                Return m_default
            End If
        End Function

        ''' <summary>
        ''' does current server has an application message handler which is able to
        ''' serve the websocket connection on a given url path?
        ''' </summary>
        Public Function CanHandle(path As String) As Boolean
            Return Not ResolveHandler(path) Is Nothing
        End Function

        ''' <summary>
        ''' removes the url query arguments part and the trailing slash character
        ''' from a raw request url for get the routing table key.
        ''' </summary>
        ''' <returns>
        ''' this function always returns a url path string which is leading with
        ''' the slash character.
        ''' </returns>
        Public Shared Function NormalizePath(url As String) As String
            If url.StringEmpty Then
                Return "/"
            End If

            Dim path As String = url
            Dim query As Integer = path.IndexOfAny({"?"c, "#"c})

            If query > -1 Then
                path = path.Substring(0, query)
            End If
            If Not path.StartsWith("/"c) Then
                path = "/" & path
            End If
            ' the trailing slash is trimmed so that ``/ws/chat`` and ``/ws/chat/``
            ' will be routed to the same application message handler.
            If path.Length > 1 AndAlso path.EndsWith("/"c) Then
                path = path.TrimEnd("/"c)
            End If

            Return If(path.StringEmpty, "/", path)
        End Function

#End Region

#Region "Connection lifecycle"

        ''' <summary>
        ''' register a new established websocket connection into current manager,
        ''' this method is invoked by the <see cref="WebSocketConnection.RunLoop"/>
        ''' method automatically.
        ''' </summary>
        Friend Sub Register(connection As WebSocketConnection)
            m_connections(connection.Id) = connection
        End Sub

        ''' <summary>
        ''' remove a closed websocket connection from current manager, this method is
        ''' invoked by the <see cref="WebSocketConnection.RunLoop"/> method automatically.
        ''' </summary>
        Friend Sub Unregister(connection As WebSocketConnection)
            Dim removed As WebSocketConnection = Nothing
            Call m_connections.TryRemove(connection.Id, removed)
        End Sub

        ''' <summary>
        ''' close all of the active websocket connections which is managed by current
        ''' manager object, this method is usually invoked on the server shutdown.
        ''' </summary>
        ''' <param name="code">the close status code which will be sent to the clients</param>
        Public Sub CloseAll(Optional code As WebSocketCloseCode = WebSocketCloseCode.GoingAway,
                            Optional reason As String = "The server is shutting down.")

            For Each connection As WebSocketConnection In GetConnections()
                Try
                    Call connection.CloseConnection(code, reason)
                Catch ex As Exception
                    Call App.LogException(ex)
                End Try
            Next
        End Sub

#End Region

#Region "Broadcast"

        ''' <summary>
        ''' push a text message to all of the active websocket connections
        ''' </summary>
        ''' <param name="text">the text message which will be pushed to the clients</param>
        ''' <param name="exclude">
        ''' an optional connection which will be skipped in current broadcast
        ''' operation, i.e. skip the message sender itself in a chat room.
        ''' </param>
        ''' <returns>the number of the clients which received the given message successfully</returns>
        Public Function Broadcast(text As String, Optional exclude As WebSocketConnection = Nothing) As Integer
            Return broadcastFrame(GetConnections(), WebSocketOpcode.Text, Encoding.UTF8.GetBytes(If(text, "")), exclude)
        End Function

        ''' <summary>
        ''' push a text message to all of the active websocket connections which is
        ''' established on a specific url path.
        ''' </summary>
        ''' <param name="path">the url path of the target websocket endpoint</param>
        ''' <returns>the number of the clients which received the given message successfully</returns>
        Public Function Broadcast(path As String, text As String, Optional exclude As WebSocketConnection = Nothing) As Integer
            Return broadcastFrame(GetConnections(path), WebSocketOpcode.Text, Encoding.UTF8.GetBytes(If(text, "")), exclude)
        End Function

        ''' <summary>
        ''' push a binary message to all of the active websocket connections
        ''' </summary>
        ''' <returns>the number of the clients which received the given message successfully</returns>
        Public Function BroadcastBinary(data As Byte(), Optional exclude As WebSocketConnection = Nothing) As Integer
            Return broadcastFrame(GetConnections(), WebSocketOpcode.Binary, data, exclude)
        End Function

        ''' <summary>
        ''' push a binary message to all of the active websocket connections on a
        ''' specific url path.
        ''' </summary>
        ''' <returns>the number of the clients which received the given message successfully</returns>
        Public Function BroadcastBinary(path As String, data As Byte(), Optional exclude As WebSocketConnection = Nothing) As Integer
            Return broadcastFrame(GetConnections(path), WebSocketOpcode.Binary, data, exclude)
        End Function

        ''' <summary>
        ''' push a data frame to a set of the websocket connections, the dead
        ''' connection will be cleaned up from the connection registry lazily
        ''' inside current broadcast operation.
        ''' </summary>
        ''' <returns>the number of the clients which received the given data frame successfully</returns>
        Private Function broadcastFrame(connections As WebSocketConnection(),
                                        opcode As WebSocketOpcode,
                                        payload As Byte(),
                                        exclude As WebSocketConnection) As Integer
            Dim success As Integer = 0

            For Each connection As WebSocketConnection In connections
                If exclude IsNot Nothing AndAlso connection.Id = exclude.Id Then
                    Continue For
                End If

                Try
                    If connection.IsOpen AndAlso connection.SendFrame(opcode, payload) Then
                        success += 1
                    Else
                        ' lazy cleanup of the dead connection
                        Call Unregister(connection)
                    End If
                Catch ex As Exception
                    ' a broken connection should never break the whole broadcast
                    Call App.LogException(ex)
                    Call Unregister(connection)
                End Try
            Next

            Return success
        End Function

#End Region

    End Class
End Namespace
