#Region "Microsoft.VisualBasic::5900a5d0d10b3354c7606ebe47cd0ecd, G:/GCModeller/src/runtime/httpd/src/Flute//SessionManager.vb"

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
    '    Code Lines: 24
    ' Comment Lines: 0
    '   Blank Lines: 13
    '     File Size: 1021 B


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

Imports Flute.Http.Configurations
Imports Flute.Http.Core.Message
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

Public Class SessionManager : Inherits ServerComponent

    Public ReadOnly Property Id As String
    Public ReadOnly Property SetCookie As Boolean = False

    Public Const CookieName As String = "flute_session"

    Sub New(cookies As Cookies, settings As Configuration)
        Call MyBase.New(settings)

        If cookies.CheckCookie(CookieName) Then
            Id = cookies.GetCookie(CookieName)
        End If

        If Id.StringEmpty Then
            Id = settings.session.session_id_prefix & "_" & (Now.ToString & randf.NextDouble).MD5.Substring(8, 8)
            SetCookie = True
        End If
    End Sub

    Public Function GetSession(name As String) As Object

    End Function

    Public Sub SaveSession(name As String, value As String)

    End Sub

    Public Sub SaveSession(name As String, value As String())

    End Sub

End Class

