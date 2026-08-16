#Region "Microsoft.VisualBasic::9e78223a4e611cf2acc6295de7705a82, src\Flute\Http\WebSocket\WebSocketDelegate.vb"

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

    '   Total Lines: 199
    '    Code Lines: 74 (37.19%)
    ' Comment Lines: 103 (51.76%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 22 (11.06%)
    '     File Size: 8.71 KB


    '     Delegate Sub
    ' 
    ' 
    '     Delegate Sub
    ' 
    ' 
    '     Delegate Sub
    ' 
    ' 
    '     Delegate Sub
    ' 
    ' 
    '     Class WebSocketMessage
    ' 
    '         Properties: Data, IsBinary, Length, Text
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    ' 
    '     Interface IWebSocketHandler
    ' 
    '         Sub: OnClose, OnConnect, OnError, OnMessage
    ' 
    '     Class WebSocketHandler
    ' 
    '         Properties: [Close], [Error], Connect, Message
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: Echo
    ' 
    '         Sub: OnClose, OnConnect, OnError, OnMessage
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text

Namespace Core.WebSocket

    ''' <summary>
    ''' the event handler which is raised just after a websocket connection
    ''' handshake has been completed successfully.
    ''' </summary>
    ''' <param name="connection">the established websocket connection</param>
    Public Delegate Sub OnConnectHandler(connection As WebSocketConnection)

    ''' <summary>
    ''' the event handler which is raised when a complete application message
    ''' has been received from the remote client. a message which is transferred
    ''' via multiple data frame fragments will be re-assembled before this event
    ''' handler is raised.
    ''' </summary>
    ''' <param name="connection">the websocket connection which received the message</param>
    ''' <param name="message">the received application message data</param>
    Public Delegate Sub OnMessageHandler(connection As WebSocketConnection, message As WebSocketMessage)

    ''' <summary>
    ''' the event handler which is raised when a websocket connection has been closed,
    ''' no matter the connection is closed by the close handshake or by an unexpected
    ''' network error.
    ''' </summary>
    ''' <param name="connection">the websocket connection which has been closed</param>
    ''' <param name="code">the close status code</param>
    ''' <param name="reason">an optional human readable close reason text</param>
    Public Delegate Sub OnCloseHandler(connection As WebSocketConnection, code As WebSocketCloseCode, reason As String)

    ''' <summary>
    ''' the event handler which is raised when an unhandled error occurs inside
    ''' the websocket data frame processing loop.
    ''' </summary>
    Public Delegate Sub OnErrorHandler(connection As WebSocketConnection, [error] As Exception)

    ''' <summary>
    ''' a complete websocket application message which has been re-assembled
    ''' from one or more websocket data frame fragments.
    ''' </summary>
    Public Class WebSocketMessage

        ''' <summary>
        ''' is current message a raw binary message? the <see cref="Text"/> property
        ''' will be nothing when this property value is true.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsBinary As Boolean
        ''' <summary>
        ''' the raw payload data of current application message, this property value
        ''' is always available for both of the text message and the binary message.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Data As Byte()
        ''' <summary>
        ''' the utf8 decoded text content of current message, this property value is
        ''' nothing when current message is a binary message.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Text As String

        ''' <summary>
        ''' the payload size in bytes of current application message
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Length As Integer
            Get
                Return If(Data Is Nothing, 0, Data.Length)
            End Get
        End Property

        ''' <summary>
        ''' create a new websocket application message object
        ''' </summary>
        ''' <param name="data">the raw payload data of the message</param>
        ''' <param name="binary">is the given payload data a raw binary message?</param>
        Sub New(data As Byte(), binary As Boolean)
            Me._Data = If(data, New Byte() {})
            Me._IsBinary = binary
            Me._Text = If(binary, Nothing, Encoding.UTF8.GetString(Me._Data))
        End Sub

        Public Overrides Function ToString() As String
            If IsBinary Then
                Return $"binary[{Length} bytes]"
            Else
                Return Text
            End If
        End Function
    End Class

    ''' <summary>
    ''' the application level websocket message handler, an application could
    ''' implements this interface for handling the websocket connection events
    ''' in an object oriented style. checkout the <see cref="WebSocketHandler"/>
    ''' class if the delegate function pointer style is preferred.
    ''' </summary>
    Public Interface IWebSocketHandler

        ''' <summary>
        ''' handling of a new established websocket connection
        ''' </summary>
        Sub OnConnect(connection As WebSocketConnection)
        ''' <summary>
        ''' handling of a complete application message which is received from the client
        ''' </summary>
        Sub OnMessage(connection As WebSocketConnection, message As WebSocketMessage)
        ''' <summary>
        ''' handling of the websocket connection close event
        ''' </summary>
        Sub OnClose(connection As WebSocketConnection, code As WebSocketCloseCode, reason As String)
        ''' <summary>
        ''' handling of an unexpected error which occurs in the data frame loop
        ''' </summary>
        Sub OnError(connection As WebSocketConnection, [error] As Exception)
    End Interface

    ''' <summary>
    ''' an <see cref="IWebSocketHandler"/> implementation which delegates all of the
    ''' websocket connection events to a set of the optional function pointers. all
    ''' of the event handlers are optional, an event which has no handler assigned
    ''' will just be ignored silently.
    ''' </summary>
    Public Class WebSocketHandler : Implements IWebSocketHandler

        ''' <summary>
        ''' raised after the websocket handshake has been completed
        ''' </summary>
        ''' <returns></returns>
        Public Property Connect As OnConnectHandler
        ''' <summary>
        ''' raised when a complete application message has been received
        ''' </summary>
        ''' <returns></returns>
        Public Property Message As OnMessageHandler
        ''' <summary>
        ''' raised when the websocket connection has been closed
        ''' </summary>
        ''' <returns></returns>
        Public Property [Close] As OnCloseHandler
        ''' <summary>
        ''' raised when an unexpected error occurs in the data frame loop
        ''' </summary>
        ''' <returns></returns>
        Public Property [Error] As OnErrorHandler

        Sub New()
        End Sub

        ''' <summary>
        ''' create a websocket event handler with the given function pointers
        ''' </summary>
        ''' <param name="message">the application message handler</param>
        ''' <param name="connect">an optional connection established handler</param>
        ''' <param name="close">an optional connection closed handler</param>
        ''' <param name="error">an optional error handler</param>
        Sub New(message As OnMessageHandler,
                Optional connect As OnConnectHandler = Nothing,
                Optional [close] As OnCloseHandler = Nothing,
                Optional [error] As OnErrorHandler = Nothing)

            Me.Message = message
            Me.Connect = connect
            Me.Close = [close]
            Me.Error = [error]
        End Sub

        Public Sub OnConnect(connection As WebSocketConnection) Implements IWebSocketHandler.OnConnect
            Call Connect?.Invoke(connection)
        End Sub

        Public Sub OnMessage(connection As WebSocketConnection, message As WebSocketMessage) Implements IWebSocketHandler.OnMessage
            Call Me.Message?.Invoke(connection, message)
        End Sub

        Public Sub OnClose(connection As WebSocketConnection, code As WebSocketCloseCode, reason As String) Implements IWebSocketHandler.OnClose
            Call Me.Close?.Invoke(connection, code, reason)
        End Sub

        Public Sub OnError(connection As WebSocketConnection, [error] As Exception) Implements IWebSocketHandler.OnError
            Call Me.Error?.Invoke(connection, [error])
        End Sub

        ''' <summary>
        ''' create a simple echo server message handler for the unit test
        ''' </summary>
        Public Shared Function Echo() As WebSocketHandler
            Return New WebSocketHandler(
                message:=Sub(connection, message)
                             If message.IsBinary Then
                                 Call connection.SendBinary(message.Data)
                             Else
                                 Call connection.SendText(message.Text)
                             End If
                         End Sub)
        End Function
    End Class
End Namespace
