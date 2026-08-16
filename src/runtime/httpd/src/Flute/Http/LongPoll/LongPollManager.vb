#Region "Microsoft.VisualBasic::9c015ab6a278a283c8b33cbdac577814, src\Flute\Http\LongPoll\LongPollManager.vb"

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

    '   Total Lines: 388
    '    Code Lines: 160 (41.24%)
    ' Comment Lines: 179 (46.13%)
    '    - Xml Docs: 96.65%
    ' 
    '   Blank Lines: 49 (12.63%)
    '     File Size: 17.14 KB


    '     Class LongPollManager
    ' 
    '         Properties: Count, Routes
    ' 
    '         Function: Broadcast, BroadcastJSON, BroadcastText, CanHandle, DefaultRoute
    '                   GetConnection, (+2 Overloads) GetConnections, GetPendingCount, NormalizePath, (+2 Overloads) Push
    '                   PushBinary, PushJSON, PushText, pushTo, RemoveRoute
    '                   ResolveHandler, (+3 Overloads) Route
    ' 
    '         Sub: CloseAll, Register, Unregister
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Concurrent
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices

Namespace Core.LongPoll

    ''' <summary>
    ''' the long polling connection manager which keeps tracking of all of the
    ''' pending long poll connections on current server, it also works as the
    ''' routing table for dispatch a long poll request to its corresponding
    ''' application level handler via the requested url path.
    ''' </summary>
    ''' <remarks>
    ''' all of the members of this class are thread safe, as the long poll
    ''' connections are running on the multiple background threads concurrently
    ''' and the push operation could be invoked from any other thread.
    ''' </remarks>
    Public Class LongPollManager

        ''' <summary>
        ''' the url path routing table of the application level long poll handlers,
        ''' the routing path key is case-insensitive.
        ''' </summary>
        Private ReadOnly m_routes As New ConcurrentDictionary(Of String, ILongPollHandler)(StringComparer.OrdinalIgnoreCase)
        ''' <summary>
        ''' all of the pending long poll connections which is indexed via the
        ''' <see cref="LongPollConnection.Id"/> value.
        ''' </summary>
        Private ReadOnly m_connections As New ConcurrentDictionary(Of String, LongPollConnection)
        ''' <summary>
        ''' the fallback long poll handler which is used when no url path route hits
        ''' </summary>
        Private m_default As ILongPollHandler

        ''' <summary>
        ''' the total number of the pending long poll connections on current server
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Count As Integer
            Get
                Return m_connections.Count
            End Get
        End Property

        ''' <summary>
        ''' all of the url path routes which has an application long poll handler
        ''' associated with it.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Routes As String()
            Get
                Return m_routes.Keys.ToArray
            End Get
        End Property

        ''' <summary>
        ''' get all of the pending long poll connections
        ''' </summary>
        ''' <returns>
        ''' a snapshot array of the pending connections, a connection inside this
        ''' snapshot result may have already been completed when the caller code
        ''' is consuming this collection.
        ''' </returns>
        Public Function GetConnections() As LongPollConnection()
            Return m_connections.Values.ToArray
        End Function

        ''' <summary>
        ''' get all of the pending long poll connections on a specific url path
        ''' </summary>
        Public Function GetConnections(path As String) As LongPollConnection()
            Dim route As String = NormalizePath(path)

            Return m_connections.Values _
                .Where(Function(c) c.Path.TextEquals(route)) _
                .ToArray
        End Function

        ''' <summary>
        ''' get a pending long poll connection via its unique connection id
        ''' </summary>
        ''' <returns>returns nothing when no connection could be found</returns>
        Public Function GetConnection(id As String) As LongPollConnection
            Dim connection As LongPollConnection = Nothing
            Return If(m_connections.TryGetValue(id, connection), connection, Nothing)
        End Function

        ''' <summary>
        ''' get the number of the pending long poll connections on a specific url path
        ''' </summary>
        Public Function GetPendingCount(path As String) As Integer
            Return GetConnections(path).Length
        End Function

#Region "Routing"

        ''' <summary>
        ''' associate an application level long poll handler with a specific url path.
        ''' </summary>
        ''' <param name="path">
        ''' the url path of the long poll endpoint, i.e. ``/poll/messages``. an
        ''' existed handler on the same url path will be replaced by the new given
        ''' handler.
        ''' </param>
        ''' <param name="handler">the application level long poll handler</param>
        ''' <returns>current manager object for the method chain style invoke</returns>
        Public Function Route(path As String, handler As ILongPollHandler) As LongPollManager
            m_routes(NormalizePath(path)) = handler
            Return Me
        End Function

        ''' <summary>
        ''' associate a set of the event function pointers with a specific url path
        ''' </summary>
        ''' <param name="path">the url path of the long poll endpoint</param>
        ''' <param name="poll">
        ''' the poll arrival handler, returns a non-null message for an immediate
        ''' response, or nothing to block the request and wait for a push operation.
        ''' </param>
        ''' <param name="complete">an optional completion handler</param>
        Public Function Route(path As String,
                              poll As OnPollHandler,
                              Optional complete As OnCompleteHandler = Nothing) As LongPollManager

            Return Route(path, New LongPollHandler(poll, complete))
        End Function

        ''' <summary>
        ''' register a long poll endpoint with the default blocking behaviour,
        ''' i.e. the poll request always blocks and waits for a push operation,
        ''' no application level handler is required.
        ''' </summary>
        ''' <param name="path">the url path of the long poll endpoint</param>
        Public Function Route(path As String) As LongPollManager
            Return Route(path, New LongPollHandler(Nothing, Nothing))
        End Function

        ''' <summary>
        ''' set the fallback application long poll handler which will be used when
        ''' the requested url path hits none of the registered routes.
        ''' </summary>
        Public Function DefaultRoute(handler As ILongPollHandler) As LongPollManager
            m_default = handler
            Return Me
        End Function

        ''' <summary>
        ''' remove the application long poll handler of a specific url path
        ''' </summary>
        ''' <returns>returns false when no handler is associated with the given url path</returns>
        Public Function RemoveRoute(path As String) As Boolean
            Dim removed As ILongPollHandler = Nothing
            Return m_routes.TryRemove(NormalizePath(path), removed)
        End Function

        ''' <summary>
        ''' resolve the application level long poll handler of a given request url path.
        ''' </summary>
        ''' <param name="path">the requested url path of the long poll request</param>
        ''' <returns>
        ''' returns the fallback handler which is configured via the
        ''' <see cref="DefaultRoute"/> method when the given url path hits none of
        ''' the registered routes, and nothing will be returned when no fallback
        ''' handler is configured on current server.
        ''' </returns>
        Public Function ResolveHandler(path As String) As ILongPollHandler
            Dim handler As ILongPollHandler = Nothing

            If m_routes.TryGetValue(NormalizePath(path), handler) Then
                Return handler
            Else
                Return m_default
            End If
        End Function

        ''' <summary>
        ''' does current server has an application long poll handler which is able
        ''' to serve the long poll request on a given url path?
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
            ' the trailing slash is trimmed so that ``/poll/chat`` and ``/poll/chat/``
            ' will be routed to the same application long poll handler.
            If path.Length > 1 AndAlso path.EndsWith("/"c) Then
                path = path.TrimEnd("/"c)
            End If

            Return If(path.StringEmpty, "/", path)
        End Function

#End Region

#Region "Connection lifecycle"

        ''' <summary>
        ''' register a new pending long poll connection into current manager,
        ''' this method is invoked by the http processor automatically.
        ''' </summary>
        Friend Sub Register(connection As LongPollConnection)
            m_connections(connection.Id) = connection
        End Sub

        ''' <summary>
        ''' remove a completed long poll connection from current manager, this
        ''' method is invoked by the http processor automatically.
        ''' </summary>
        Friend Sub Unregister(connection As LongPollConnection)
            Dim removed As LongPollConnection = Nothing
            Call m_connections.TryRemove(connection.Id, removed)
        End Sub

        ''' <summary>
        ''' cancel all of the pending long poll connections which is managed by
        ''' current manager object, this method is usually invoked on the server
        ''' shutdown for wake up all of the blocked worker threads.
        ''' </summary>
        Public Sub CloseAll()
            For Each connection As LongPollConnection In GetConnections()
                Try
                    Call connection.Cancel()
                Catch ex As Exception
                    Call App.LogException(ex)
                End Try
            Next
        End Sub

#End Region

#Region "Push"

        ''' <summary>
        ''' push a message to a single pending long poll connection via its
        ''' unique connection id.
        ''' </summary>
        ''' <returns>
        ''' returns true when the message is delivered to the pending connection;
        ''' returns false when the connection is not found or already completed.
        ''' </returns>
        Public Function Push(id As String, message As LongPollMessage) As Boolean
            Dim connection As LongPollConnection = Nothing

            If Not m_connections.TryGetValue(id, connection) Then
                Return False
            End If

            Dim success As Boolean = connection.Complete(message)

            If success Then
                ' the connection is completed, lazy cleanup
                Call Unregister(connection)
            End If

            Return success
        End Function

        ''' <summary>
        ''' push a message to all of the pending long poll connections on a
        ''' specific url path. the dead connection which is already completed or
        ''' cancelled will be cleaned up lazily inside current push operation.
        ''' </summary>
        ''' <param name="path">the url path of the target long poll endpoint</param>
        ''' <param name="message">the push message payload</param>
        ''' <param name="exclude">
        ''' an optional connection which will be skipped in current push operation,
        ''' i.e. skip the message sender itself in a chat room.
        ''' </param>
        ''' <returns>the number of the clients which received the given message successfully</returns>
        Public Function Push(path As String, message As LongPollMessage, Optional exclude As LongPollConnection = Nothing) As Integer
            Return pushTo(GetConnections(path), message, exclude)
        End Function

        ''' <summary>
        ''' push a text message to all of the pending long poll connections on a
        ''' specific url path.
        ''' </summary>
        ''' <returns>the number of the clients which received the given message successfully</returns>
        Public Function PushText(path As String, text As String, Optional exclude As LongPollConnection = Nothing) As Integer
            Return Push(path, LongPollMessage.Text(text), exclude)
        End Function

        ''' <summary>
        ''' push a json message to all of the pending long poll connections on a
        ''' specific url path.
        ''' </summary>
        ''' <returns>the number of the clients which received the given message successfully</returns>
        Public Function PushJSON(path As String, json As String, Optional exclude As LongPollConnection = Nothing) As Integer
            Return Push(path, LongPollMessage.JSON(json), exclude)
        End Function

        ''' <summary>
        ''' push a binary message to all of the pending long poll connections on
        ''' a specific url path.
        ''' </summary>
        ''' <returns>the number of the clients which received the given message successfully</returns>
        Public Function PushBinary(path As String, data As Byte(), Optional contentType As String = "application/octet-stream", Optional exclude As LongPollConnection = Nothing) As Integer
            Return Push(path, LongPollMessage.Binary(data, contentType), exclude)
        End Function

        ''' <summary>
        ''' push a message to all of the pending long poll connections on current
        ''' server, no matter what url path the connection is on.
        ''' </summary>
        ''' <returns>the number of the clients which received the given message successfully</returns>
        Public Function Broadcast(message As LongPollMessage, Optional exclude As LongPollConnection = Nothing) As Integer
            Return pushTo(GetConnections(), message, exclude)
        End Function

        ''' <summary>
        ''' push a text message to all of the pending long poll connections on
        ''' current server.
        ''' </summary>
        ''' <returns>the number of the clients which received the given message successfully</returns>
        Public Function BroadcastText(text As String, Optional exclude As LongPollConnection = Nothing) As Integer
            Return Broadcast(LongPollMessage.Text(text), exclude)
        End Function

        ''' <summary>
        ''' push a json message to all of the pending long poll connections on
        ''' current server.
        ''' </summary>
        ''' <returns>the number of the clients which received the given message successfully</returns>
        Public Function BroadcastJSON(json As String, Optional exclude As LongPollConnection = Nothing) As Integer
            Return Broadcast(LongPollMessage.JSON(json), exclude)
        End Function

        ''' <summary>
        ''' push a message to a set of the pending long poll connections, the dead
        ''' connection which is already completed or cancelled will be cleaned up
        ''' from the connection registry lazily inside current push operation.
        ''' </summary>
        ''' <returns>the number of the clients which received the given message successfully</returns>
        Private Function pushTo(connections As LongPollConnection(),
                                message As LongPollMessage,
                                exclude As LongPollConnection) As Integer
            Dim success As Integer = 0

            For Each connection As LongPollConnection In connections
                If exclude IsNot Nothing AndAlso connection.Id = exclude.Id Then
                    Continue For
                End If

                Try
                    If connection.IsPending AndAlso connection.Complete(message) Then
                        success += 1
                        ' lazy cleanup of the completed connection
                        Call Unregister(connection)
                    Else
                        ' the connection is already completed or cancelled
                        Call Unregister(connection)
                    End If
                Catch ex As Exception
                    ' a broken connection should never break the whole push
                    Call App.LogException(ex)
                    Call Unregister(connection)
                End Try
            Next

            Return success
        End Function

#End Region

    End Class
End Namespace
