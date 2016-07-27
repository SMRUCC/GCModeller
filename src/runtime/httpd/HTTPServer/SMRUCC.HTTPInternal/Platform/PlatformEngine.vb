#Region "Microsoft.VisualBasic::dbda62ed14726f6506b0a9e6cb6886d0, ..\httpd\HTTPServer\SMRUCC.HTTPInternal\Platform\PlatformEngine.vb"

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

Imports System.IO
Imports System.Net.Sockets
Imports System.Text
Imports SMRUCC.HTTPInternal.AppEngine.POSTParser
Imports SMRUCC.HTTPInternal.Core
Imports SMRUCC.HTTPInternal.Platform.Plugins

Namespace Platform

    ''' <summary>
    ''' 服务基础类，REST API的开发需要引用当前的项目
    ''' </summary>
    Public Class PlatformEngine : Inherits HttpFileSystem

        Public ReadOnly Property AppManager As AppEngine.APPManager
        Public ReadOnly Property TaskPool As New TaskPool
        Public ReadOnly Property EnginePlugins As Plugins.PluginBase()

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="port"></param>
        ''' <param name="root"></param>
        ''' <param name="nullExists"></param>
        Sub New(root As String, Optional port As Integer = 80, Optional nullExists As Boolean = False, Optional appDll As String = "")
            Call MyBase.New(port, root, nullExists)
            Call __init(appDll)
        End Sub

        Private Sub __init(dll As String)
            _AppManager = New AppEngine.APPManager(Me)
            If dll.FileExists Then
                Call AppEngine.ExternalCall.ParseDll(dll, Me)
            Else
                Call AppEngine.ExternalCall.Scan(Me)
            End If
            Me._EnginePlugins = Plugins.ExternalCall.Scan(Me)
        End Sub

        Public Overrides Sub handlePOSTRequest(p As HttpProcessor, inputData As MemoryStream)
            Dim out As String = ""
            Dim args As PostReader = New PostReader(inputData, p.httpHeaders("Content-Type"), Encoding.UTF8)
            Dim success As Boolean = AppManager.InvokePOST(p.http_url, args, out)
            Call __handleSend(p, success, out)
        End Sub

        ''' <summary>
        ''' GET
        ''' </summary>
        ''' <param name="p"></param>
        Protected Overrides Sub __handleREST(p As HttpProcessor)
            Dim out As String = ""
            Dim success As Boolean = AppManager.Invoke(p.http_url, out)
            Call __handleSend(p, success, out)
        End Sub

        Private Sub __handleSend(p As HttpProcessor, success As Boolean, out As String)
            Call p.outputStream.WriteLine(out)

            For Each plugin As PluginBase In EnginePlugins
                Call plugin.handleVisit(p, success)
            Next
        End Sub

        Protected Overrides Sub Dispose(disposing As Boolean)
            For Each plugin As Plugins.PluginBase In EnginePlugins
                Call plugin.Dispose()
            Next
            MyBase.Dispose(disposing)
        End Sub
    End Class
End Namespace
