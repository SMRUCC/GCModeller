#Region "Microsoft.VisualBasic::252bc714ea55fa826f51893fe7e42564, src\Flute\SessionManager.vb"

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

'   Total Lines: 37
'    Code Lines: 25 (67.57%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 12 (32.43%)
'     File Size: 1.05 KB


' Class SessionManager
' 
'     Properties: Id, SetCookie
' 
'     Constructor: (+1 Overloads) Sub New
' 
'     Function: GetSession
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

    Public ReadOnly Property Id As String
    Public ReadOnly Property SetCookie As Boolean = False

    Public Const CookieName As String = "flute_session"

    ''' <summary>
    ''' in-memory session store. the default implementation keeps values in
    ''' process memory; a persistent store can be provided by overriding
    ''' <see cref="GetSession"/> / <see cref="SaveSession"/> (e.g. the
    ''' Flute.SessionManager package).
    ''' </summary>
    ReadOnly store As New ConcurrentDictionary(Of String, Object)

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

    Public Overridable Function GetSession(name As String) As Object
        ' default in-memory implementation; override to back onto a persistent store
        Dim value As Object = Nothing
        Call store.TryGetValue(name, value)
        Return value
    End Function

    Public Sub SaveSession(name As String, value As String)
        Call store.AddOrUpdate(name, value, Function(k, v) value)
    End Sub

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
