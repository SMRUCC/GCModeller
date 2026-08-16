#Region "Microsoft.VisualBasic::90561f3167113bb1264e2cb79a29a35a, src\Flute\SessionManager.vb"

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

    '   Total Lines: 67
    '    Code Lines: 41 (61.19%)
    ' Comment Lines: 13 (19.40%)
    '    - Xml Docs: 69.23%
    ' 
    '   Blank Lines: 13 (19.40%)
    '     File Size: 2.60 KB


    ' Class SessionManager
    ' 
    '     Properties: Id, SetCookie
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetSession, GetSessionArray
    ' 
    '     Sub: (+2 Overloads) SaveSession
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Concurrent
Imports Flute.Http.Configurations
Imports Flute.Http.Core.Message
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

Public Class SessionManager : Inherits ServerComponent

    ''' <summary>
    ''' the unique session identifier for the current client, either taken
    ''' from the incoming request cookie or freshly generated on first visit.
    ''' </summary>
    Public ReadOnly Property Id As String
    ''' <summary>
    ''' indicates that a new session id was generated during construction and a
    ''' <c>Set-Cookie</c> response header must be emitted to the client.
    ''' </summary>
    Public ReadOnly Property SetCookie As Boolean = False

    ''' <summary>
    ''' the name of the cookie that carries the flute session identifier.
    ''' </summary>
    Public Const CookieName As String = "flute_session"

    ''' <summary>
    ''' in-memory session store. the default implementation keeps values in
    ''' process memory; a persistent store can be provided by overriding
    ''' <see cref="GetSession"/> / <see cref="SaveSession"/> (e.g. the
    ''' Flute.SessionManager package).
    ''' </summary>
    ReadOnly store As New ConcurrentDictionary(Of String, Object)

    ''' <summary>
    ''' initialize a session for the incoming request. the session id is
    ''' recovered from the request cookie when present, otherwise a new secure
    ''' random id is generated (and <see cref="SetCookie"/> is set to true).
    ''' </summary>
    ''' <param name="cookies">the cookies parsed from the incoming http request.</param>
    ''' <param name="settings">the server wide configuration instance.</param>
    Sub New(cookies As Cookies, settings As Configuration)
        Call MyBase.New(settings)

        If cookies.CheckCookie(CookieName) Then
            Id = cookies.GetCookie(CookieName)
        End If

        If Id.StringEmpty Then
            ' use a cryptographically secure random id (32 hex chars) instead
            ' of the predictable time + random MD5 substring.
            Dim bytes() As Byte = randf.GetBytes(16)
            Dim sessionId As String = String.Join("", bytes.Select(Function(b) b.ToString("x2")))

            Id = If(settings.session.session_id_prefix.StringEmpty(, True), "flute", settings.session.session_id_prefix) & "_" & sessionId
            SetCookie = True
        End If
    End Sub

    ''' <summary>
    ''' retrieve a previously saved session value by name. the default
    ''' implementation reads from the in-memory store; override this to back
    ''' onto a persistent session store.
    ''' </summary>
    ''' <param name="name">the session key to look up.</param>
    ''' <returns>the stored value, or <c>Nothing</c> when not present.</returns>
    Public Overridable Function GetSession(name As String) As Object
        ' default in-memory implementation; override to back onto a persistent store
        Dim value As Object = Nothing
        Call store.TryGetValue(name, value)
        Return value
    End Function

    ''' <summary>
    ''' save a single string value into the session store under the given key.
    ''' </summary>
    ''' <param name="name">the session key.</param>
    ''' <param name="value">the string value to store.</param>
    Public Sub SaveSession(name As String, value As String)
        Call store.AddOrUpdate(name, value, Function(k, v) value)
    End Sub

    ''' <summary>
    ''' save a string array into the session store, encoded as a tab separated
    ''' string so it can be round-tripped by <see cref="GetSessionArray"/>.
    ''' </summary>
    ''' <param name="name">the session key.</param>
    ''' <param name="value">the string array to store.</param>
    Public Sub SaveSession(name As String, value As String())
        ' join the array with a tab so it can be round-tripped by GetSessionArray
        Call store.AddOrUpdate(name, String.Join(vbTab, value), Function(k, v) String.Join(vbTab, value))
    End Sub

    ''' <summary>
    ''' retrieve a previously saved session value as a tab-split string array.
    ''' </summary>
    Public Overridable Function GetSessionArray(name As String) As String()
        Dim value As Object = GetSession(name)
        If value Is Nothing Then
            Return {}
        Else
            Return CStr(value).Split(vbTab)
        End If
    End Function

End Class
