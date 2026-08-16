#Region "Microsoft.VisualBasic::c606955be7ed0e77c1419af7c2cecd3d, src\Flute\Http\LongPoll\LongPollConnection.vb"

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

    '   Total Lines: 152
    '    Code Lines: 51 (33.55%)
    ' Comment Lines: 90 (59.21%)
    '    - Xml Docs: 96.67%
    ' 
    '   Blank Lines: 11 (7.24%)
    '     File Size: 6.52 KB


    '     Class LongPollConnection
    ' 
    '         Properties: Headers, Id, IsPending, Path, Remote
    '                     Session, Timestamp, Url
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: Complete, ToString, WaitForData
    ' 
    '         Sub: Cancel
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Net

Namespace Core.LongPoll

    ''' <summary>
    ''' a single pending long poll connection between this server and a remote
    ''' client. this object holds the request metadata and the synchronization
    ''' primitive for blocking the worker thread until a push operation arrives
    ''' or the poll is timed out / cancelled.
    ''' </summary>
    ''' <remarks>
    ''' the <see cref="WaitForData"/> method blocks the caller thread (the http
    ''' processor worker thread) until the push data arrives via <see cref="Complete"/>,
    ''' or the given timeout is exceeded, or the wait is cancelled via <see cref="Cancel"/>.
    ''' the push operation could be invoked safely from any other thread, as the
    ''' underlying <see cref="TaskCompletionSource(Of TResult)"/> is thread safe.
    ''' </remarks>
    Public Class LongPollConnection

        ''' <summary>
        ''' the unique id of current long poll connection
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Id As String
        ''' <summary>
        ''' the normalized url path of current long poll request, this value is
        ''' used by the <see cref="LongPollManager"/> for routing the push
        ''' operation to the pending connections on a specific endpoint.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Path As String
        ''' <summary>
        ''' the raw request url of the long poll request, the url query arguments
        ''' are included in this property value.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Url As String
        ''' <summary>
        ''' the http request headers of the long poll request
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Headers As Dictionary(Of String, String)
        ''' <summary>
        ''' the remote client network endpoint of current long poll connection
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Remote As EndPoint
        ''' <summary>
        ''' a user state data bag which could be used by the application code for
        ''' associating any custom session data with current connection.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Session As New Dictionary(Of String, Object)
        ''' <summary>
        ''' the utc timestamp at which current long poll connection is established
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Timestamp As DateTime = DateTime.UtcNow

        ''' <summary>
        ''' the synchronization primitive for blocking the worker thread until
        ''' the push data arrives or the wait is cancelled.
        ''' </summary>
        Private ReadOnly m_tcs As New TaskCompletionSource(Of LongPollMessage)()

        ''' <summary>
        ''' is current long poll connection still pending for the push data?
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsPending As Boolean
            Get
                Return Not m_tcs.Task.IsCompleted
            End Get
        End Property

        ''' <summary>
        ''' create a new pending long poll connection object
        ''' </summary>
        ''' <param name="path">the normalized url path of the long poll endpoint</param>
        ''' <param name="url">the raw request url</param>
        ''' <param name="headers">the http request headers</param>
        ''' <param name="remote">the remote client network endpoint</param>
        Sub New(path As String,
                url As String,
                headers As Dictionary(Of String, String),
                remote As EndPoint)

            Me._Id = Guid.NewGuid.ToString
            Me._Path = path
            Me._Url = url
            Me._Headers = If(headers, New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase))
            Me._Remote = remote
        End Sub

        Public Overrides Function ToString() As String
            Return $"longpoll://{Remote}{Path}"
        End Function

        ''' <summary>
        ''' block current thread and wait for the push data, returns the pushed
        ''' message when the push arrives, or nothing when the wait is timed out
        ''' or cancelled.
        ''' </summary>
        ''' <param name="timeoutMs">
        ''' the maximum time in milliseconds to wait for the push data. a value
        ''' which is less than or equals to zero means infinite waiting.
        ''' </param>
        ''' <returns>
        ''' returns the pushed message, or nothing when the wait is timed out or
        ''' cancelled.
        ''' </returns>
        Public Function WaitForData(timeoutMs As Integer) As LongPollMessage
            Try
                If timeoutMs <= 0 Then
                    ' wait indefinitely for the push data
                    m_tcs.Task.Wait()
                    Return m_tcs.Task.Result
                ElseIf m_tcs.Task.Wait(timeoutMs) Then
                    Return m_tcs.Task.Result
                Else
                    ' timed out
                    Return Nothing
                End If
            Catch ex As Exception
                ' the wait is cancelled or an unexpected error occurred
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' complete the pending poll with the given push message, this method
        ''' wakes up the worker thread which is blocked inside <see cref="WaitForData"/>.
        ''' </summary>
        ''' <returns>
        ''' returns true when the pending poll is successfully completed; returns
        ''' false when the poll has already been completed or cancelled.
        ''' </returns>
        Public Function Complete(message As LongPollMessage) As Boolean
            Return m_tcs.TrySetResult(message)
        End Function

        ''' <summary>
        ''' cancel the pending poll, this method wakes up the worker thread which
        ''' is blocked inside <see cref="WaitForData"/> with a cancelled state.
        ''' usually invoked by the <see cref="LongPollManager.CloseAll"/> on the
        ''' server shutdown.
        ''' </summary>
        Public Sub Cancel()
            m_tcs.TrySetCanceled()
        End Sub
    End Class
End Namespace
