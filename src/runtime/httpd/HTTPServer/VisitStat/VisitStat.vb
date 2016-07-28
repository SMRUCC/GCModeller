#Region "Microsoft.VisualBasic::95157a2a416986d95fd825a7fd78d29b, ..\httpd\HTTPServer\VisitStat\VisitStat.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports Microsoft.VisualBasic.Net
Imports Microsoft.VisualBasic.Parallel
Imports Oracle.LinuxCompatibility.MySQL
Imports SMRUCC.REST.HttpInternal
Imports SMRUCC.REST.Platform

Public Class VisitStat : Inherits Plugins.PluginBase

    ReadOnly _transactions As New List(Of visitor_stat)
    ReadOnly _commitThread As UpdateThread
    ReadOnly _mySQL As MySQL

    Sub New(platform As PlatformEngine)
        Call MyBase.New(platform)
        _commitThread = New UpdateThread(60 * 1000, AddressOf __commits)
    End Sub

    Private Sub __commits()
        If _transactions.IsNullOrEmpty Then
            Return
        End If

        Call _mySQL.CommitInserts(_transactions)
        Call _transactions.Clear()
    End Sub

    Public Overrides Sub handleVisit(p As HttpProcessor, success As Boolean)
        Dim ip As String = DirectCast(p.socket.Client.RemoteEndPoint, System.Net.IPEndPoint).Address.ToString
        Dim visit As New visitor_stat With {
            .ip = ip,
            .method = p.http_method,
            .success = success,
            .time = Now,
            .ua = p.httpHeaders(""),
            .url = p.http_url
        }
        Call _transactions.Add(visit)
    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        _commitThread.Stop()
        __commits()
        MyBase.Dispose(disposing)
    End Sub
End Class

