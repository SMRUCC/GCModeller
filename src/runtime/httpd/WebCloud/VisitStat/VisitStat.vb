#Region "Microsoft.VisualBasic::1791bed3f2f7b0cc02c86497abc5d059, ..\httpd\WebCloud\VisitStat\VisitStat.vb"

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

Imports Microsoft.VisualBasic.Net
Imports Microsoft.VisualBasic.Parallel.Tasks
Imports Oracle.LinuxCompatibility.MySQL
Imports SMRUCC.WebCloud.HTTPInternal.Platform
Imports SMRUCC.WebCloud.HTTPInternal
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.Arguments

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

    Protected Overrides Sub Dispose(disposing As Boolean)
        _commitThread.Stop()
        __commits()
        MyBase.Dispose(disposing)
    End Sub

    Public Overrides Sub handleVisit(p As HttpRequest, success As Boolean)
        Dim ip As String = p.Remote
        Dim visit As New visitor_stat With {
            .ip = ip,
            .method = p.HTTPMethod,
            .success = success,
            .time = Now,
            .ua = p.HttpHeaders(""),
            .url = p.URL
        }
        Call _transactions.Add(visit)
    End Sub
End Class
