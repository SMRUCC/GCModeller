#Region "Microsoft.VisualBasic::d1c4b303cf359ec8aa5b094a8dbc1e45, httpd\Program\CLI.vb"

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

    ' Module CLI
    ' 
    '     Function: RunApp, RunDll, RunSocket, Start
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Reflection
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService.SharedORM
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.WebCloud.HTTPInternal.Core
Imports SMRUCC.WebCloud.HTTPInternal.Core.WebSocket
Imports SMRUCC.WebCloud.HTTPInternal.Platform

<GroupingDefine(CLI.httpdServerCLI, Description:="Server CLI for running this httpd web server.")>
<CLI> Module CLI

    Public Const httpdServerCLI$ = NameOf(httpdServerCLI)
    Public Const Utility$ = NameOf(Utility)

    <ExportAPI("/socket")>
    <Description("Start a new websocket server.")>
    <Usage("/socket /app <appName> [/hostName <default=127.0.0.1> /port <default=81>]")>
    Public Function RunSocket(args As CommandLine) As Integer
        Dim port% = args("/port") Or 81
        Dim app As WebsocketActivator = WebSocket.GetActivator(HOME, args <= "/app")
        Dim socket As New WsServer(args("/hostName") Or "127.0.0.1", port, activator:=app)

        Return socket.Run
    End Function

    <ExportAPI("/start",
               Info:="Run start the httpd web server.",
               Usage:="/start [/port 80 /wwwroot <wwwroot_DIR> /threads <default=-1> /cache]")>
    <ArgumentAttribute("/port", True, CLITypes.Integer,
              AcceptTypes:={GetType(Integer)},
              Description:="The server port of this httpd web server to listen.")>
    <ArgumentAttribute("/wwwroot", True, CLITypes.File, PipelineTypes.std_in,
              AcceptTypes:={GetType(String)},
              Description:="The website html root directory path.")>
    <ArgumentAttribute("/threads", True, CLITypes.Integer,
              AcceptTypes:={GetType(Integer)},
              Description:="The number of threads of this web server its thread pool.")>
    <ArgumentAttribute("/cache", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="Is this server running in file system cache mode? Not recommended for open.")>
    <Group(httpdServerCLI)>
    Public Function Start(args As CommandLine) As Integer
        Dim port As Integer = args.GetValue("/port", 80)
        Dim HOME As String = args("/wwwroot") Or App.CurrentDirectory
        Dim threads As Integer = args.GetValue("/threads", -1)
        Dim cacheMode As Boolean = args.IsTrue("/cache")

#If DEBUG Then
        ' 使用单线程模式方便做调试
        threads = 1
#End If

        Dim server As New PlatformEngine(
            HOME, port,
            nullExists:=True,
            threads:=threads,
            cache:=cacheMode
        )

        Return server.Run()
    End Function

    <ExportAPI("/run",
               Info:="Run start the web server with specific Web App.",
               Usage:="/run /dll <app.dll> [/port <default=80> /wwwroot <wwwroot_DIR>]")>
    <Group(httpdServerCLI)>
    Public Function RunApp(args As CommandLine) As Integer
        Dim port As Integer = args.GetValue("/port", 80)
        Dim HOME As String = args.GetValue("/wwwroot", App.CurrentDirectory)
        Dim dll As String = args.GetValue("/dll", "")
        Return New PlatformEngine(HOME, port, True, dll).Run
    End Function

    ''' <summary>
    ''' 可以使用这个API来运行内部的配置API，例如调用内部的函数配置mysql链接
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/run.dll", Usage:="/run.dll /api <namespace::apiName> [....]")>
    Public Function RunDll(args As CommandLine) As Integer
        Dim api$ = args <= "/api"
        Dim run As Boolean = False
        Dim params$() = args.Tokens.Skip(3).ToArray
        Dim method As MethodInfo

        For Each dll As String In ls - l - r - "*.dll" <= App.HOME

            method = Nothing

            Try
                method = RunDllEntryPoint.GetDllMethod(Assembly.LoadFrom(dll), api)
            Catch ex As Exception
#If DEBUG Then
                Call ex.PrintException
#Else
                Call App.LogException(ex)
#End If
            End Try

#If DEBUG Then
            Call dll.__INFO_ECHO
#End If
            If Not method Is Nothing Then
                run = True
                Call method.Invoke(Nothing, params)
            End If
        Next

        If Not run Then
            Call $"No dll api which is named {api} was found!".Warning
        End If

        Return 0
    End Function
End Module
