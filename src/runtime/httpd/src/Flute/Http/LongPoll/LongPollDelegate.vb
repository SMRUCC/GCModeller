#Region "Microsoft.VisualBasic::f8c51ba5d348630ac08c5754cd50c686, src\Flute\Http\LongPoll\LongPollDelegate.vb"

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

    '   Total Lines: 160
    '    Code Lines: 50 (31.25%)
    ' Comment Lines: 90 (56.25%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 20 (12.50%)
    '     File Size: 7.05 KB


    '     Class LongPollMessage
    ' 
    '         Properties: ContentType, Data, Length
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Binary, JSON, Text, ToString
    ' 
    '     Delegate Function
    ' 
    ' 
    '     Delegate Sub
    ' 
    ' 
    '     Interface ILongPollHandler
    ' 
    '         Function: OnPoll
    ' 
    '         Sub: OnComplete
    ' 
    '     Class LongPollHandler
    ' 
    '         Properties: Complete, Poll
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: OnPoll
    ' 
    '         Sub: OnComplete
    ' 
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text

Namespace Core.LongPoll

    ''' <summary>
    ''' a pending long poll message payload which carries both the raw data bytes
    ''' and the http content type which will be used for writing the response
    ''' header when the pending poll is waken up by a push operation.
    ''' </summary>
    Public Class LongPollMessage

        ''' <summary>
        ''' the raw payload data bytes of the push message
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Data As Byte()
        ''' <summary>
        ''' the http content type of the push message, this value will be used
        ''' for writing the ``Content-Type`` header of the long poll response.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ContentType As String

        ''' <summary>
        ''' the payload size in bytes of current push message
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Length As Integer
            Get
                Return If(Data Is Nothing, 0, Data.Length)
            End Get
        End Property

        ''' <summary>
        ''' create a new long poll push message
        ''' </summary>
        ''' <param name="data">the raw payload data bytes</param>
        ''' <param name="contentType">
        ''' the http content type, default is ``application/json``
        ''' </param>
        Sub New(data As Byte(), Optional contentType As String = "application/json")
            Me._Data = If(data, New Byte() {})
            Me._ContentType = If(contentType.StringEmpty, "application/json", contentType)
        End Sub

        Public Overrides Function ToString() As String
            Return $"{ContentType}[{Length} bytes]"
        End Function

        ''' <summary>
        ''' create a text push message with the ``text/plain`` content type
        ''' </summary>
        Public Shared Function Text(content As String) As LongPollMessage
            Return New LongPollMessage(Encoding.UTF8.GetBytes(If(content, "")), "text/plain")
        End Function

        ''' <summary>
        ''' create a json push message with the ``application/json`` content type
        ''' </summary>
        Public Shared Function JSON(content As String) As LongPollMessage
            Return New LongPollMessage(Encoding.UTF8.GetBytes(If(content, "")), "application/json")
        End Function

        ''' <summary>
        ''' create a binary push message with the given content type
        ''' </summary>
        Public Shared Function Binary(data As Byte(), Optional contentType As String = "application/octet-stream") As LongPollMessage
            Return New LongPollMessage(data, contentType)
        End Function
    End Class

    ''' <summary>
    ''' the event handler which is raised when a new long poll request arrives
    ''' at the registered endpoint. the application code could returns a non-null
    ''' <see cref="LongPollMessage"/> for an immediate response without blocking,
    ''' or returns nothing to let the request block and wait for a push operation.
    ''' </summary>
    ''' <param name="connection">the pending long poll connection</param>
    Public Delegate Function OnPollHandler(connection As LongPollConnection) As LongPollMessage

    ''' <summary>
    ''' the event handler which is raised when a long poll request is finished,
    ''' no matter the request is waken up by a push operation or timed out.
    ''' </summary>
    ''' <param name="connection">the long poll connection</param>
    ''' <param name="message">
    ''' the push message which wakes up the pending poll, nothing when the
    ''' request is timed out or cancelled by the server shutdown.
    ''' </param>
    ''' <param name="timedOut">
    ''' a logical value indicates that the poll is finished by the timeout
    ''' instead of a push operation.
    ''' </param>
    Public Delegate Sub OnCompleteHandler(connection As LongPollConnection, message As LongPollMessage, timedOut As Boolean)

    ''' <summary>
    ''' the application level long poll handler interface, an application could
    ''' implements this interface for handling the long poll events in an object
    ''' oriented style. checkout the <see cref="LongPollHandler"/> class if the
    ''' delegate function pointer style is preferred.
    ''' </summary>
    Public Interface ILongPollHandler

        ''' <summary>
        ''' handling of a new arrived long poll request. returns a non-null
        ''' message for an immediate response without blocking, or returns
        ''' nothing to let the request block and wait for a push operation.
        ''' </summary>
        Function OnPoll(connection As LongPollConnection) As LongPollMessage

        ''' <summary>
        ''' handling of the long poll completion event, which is raised when the
        ''' poll is waken up by a push operation, timed out, or cancelled by the
        ''' server shutdown.
        ''' </summary>
        Sub OnComplete(connection As LongPollConnection, message As LongPollMessage, timedOut As Boolean)
    End Interface

    ''' <summary>
    ''' an <see cref="ILongPollHandler"/> implementation which delegates all of
    ''' the long poll events to a set of the optional function pointers. all of
    ''' the event handlers are optional, an event which has no handler assigned
    ''' will just be ignored silently.
    ''' </summary>
    Public Class LongPollHandler : Implements ILongPollHandler

        ''' <summary>
        ''' raised when a new long poll request arrives, returns a non-null
        ''' message for an immediate response, or nothing to block the request.
        ''' </summary>
        ''' <returns></returns>
        Public Property Poll As OnPollHandler
        ''' <summary>
        ''' raised when the long poll is finished (push waken, timeout, or cancel)
        ''' </summary>
        ''' <returns></returns>
        Public Property Complete As OnCompleteHandler

        Sub New()
        End Sub

        ''' <summary>
        ''' create a long poll event handler with the given function pointers
        ''' </summary>
        ''' <param name="poll">the poll arrival handler</param>
        ''' <param name="complete">an optional completion handler</param>
        Sub New(poll As OnPollHandler, Optional complete As OnCompleteHandler = Nothing)
            Me.Poll = poll
            Me.Complete = complete
        End Sub

        Public Function OnPoll(connection As LongPollConnection) As LongPollMessage Implements ILongPollHandler.OnPoll
            Return Poll?.Invoke(connection)
        End Function

        Public Sub OnComplete(connection As LongPollConnection, message As LongPollMessage, timedOut As Boolean) Implements ILongPollHandler.OnComplete
            Call Complete?.Invoke(connection, message, timedOut)
        End Sub
    End Class
End Namespace
