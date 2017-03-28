#Region "Microsoft.VisualBasic::4585e07efeb212542d4d8f4fcaa66027, ..\visualbasic.DBI\mysql-nt\TinyServer.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.CommandLine

''' <summary>
''' Only works on Windows
''' </summary>
Public Class TinyServer

    Dim Resource As CliResCommon

    Public ReadOnly Property libmySQL As String
        Get
            Return Resource.TryRelease(NameOf(My.Resources.libmySQL), "dll")
        End Get
    End Property

    Public ReadOnly Property mysql As String
        Get
            Return Resource.TryRelease(NameOf(My.Resources.mysql))
        End Get
    End Property

    Public ReadOnly Property mysqladmin As String
        Get
            Return Resource.TryRelease(NameOf(My.Resources.mysqladmin))
        End Get
    End Property

    Public ReadOnly Property mysqld As String
        Get
            Return Resource.TryRelease(NameOf(My.Resources.mysqld))
        End Get
    End Property

    Sub New()
        Resource = New CliResCommon(App.HOME & "/MySQL.Tiny/", GetType(My.Resources.Resources))
    End Sub

    ''' <summary>
    ''' Run server process, the thread will be stuck at this function until the server is stop.
    ''' </summary>
    Public Sub Start()
        Call New IORedirectFile(mysqld).Run()
    End Sub

End Class
