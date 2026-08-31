#Region "Microsoft.VisualBasic::d59d6197138905104fe776208e813ab9, src\HTTP_SERVER\Program.vb"

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

    '   Total Lines: 108
    '    Code Lines: 68 (62.96%)
    ' Comment Lines: 23 (21.30%)
    '    - Xml Docs: 17.39%
    ' 
    '   Blank Lines: 17 (15.74%)
    '     File Size: 4.65 KB


    ' Module Program
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: listen, listenCurrentFolder, Main
    ' 
    '     Sub: ProcessRequest
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports Flute.Http
Imports Flute.Http.Core
Imports Flute.Http.Core.Message
Imports Flute.Http.FileSystem
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Net

Module Program

    Sub New()

    End Sub

    Public Function Main() As Integer
        Return GetType(Program).RunCLI(App.CommandLine, executeEmpty:=AddressOf listenCurrentFolder)
    End Function

    ''' <summary>
    ''' run ``--listen`` command for current folder by default
    ''' </summary>
    ''' <returns></returns>
    Private Function listenCurrentFolder() As Integer
        Return listen("--listen")
    End Function

    <ExportAPI("--listen")>
    <Description("Start a local static web server for hosting statics web page files")>
    <Usage("--listen [/wwwroot <directory_path> --attach <other_directory_path/streampack> --parent <parent_process_id> /port <http_port, default=80>]")>
    Public Function listen(args As CommandLine) As Integer
        Dim wwwroot As String = args("/wwwroot") Or App.CurrentDirectory
        Dim port As Integer = args("/port") Or 80
        Dim attach As String = args("--attach")
        Dim parent As String = args("--parent")

        ' check port availability BEFORE creating the server to avoid
        ' a race condition where another process grabs the port between
        ' the check and the actual bind.
        If Not Tcp.PortIsAvailable(port) Then
            Call Console.WriteLine($"local tcp port(={port}) is in used!")
            Return 500
        End If

        Dim localfs As New WebFileSystemListener(New FileSystem(wwwroot))
        ' wrap the static file handler with a long poll push endpoint demo:
        '   GET  /poll/messages  -> long poll, blocks until a push arrives
        '   POST /push           -> push a text message to all pending polls
        Dim longpollEndpoint As String = "/poll/messages"
        Dim localhost As New HttpSocket(
            app:=Sub(request As HttpRequest, response As HttpResponse)
                     Call localhost.ProcessRequest(localfs, longpollEndpoint, request, response)
                 End Sub,
            port:=port
        )

        ' register the long poll endpoint so that a GET /poll/messages request
        ' will be blocked for waiting a push operation instead of being served
        ' as a static file.
        Call localhost.LongPoll.Route(longpollEndpoint)
        Call $"long poll endpoint registered on '{longpollEndpoint}'.".info()

        If Not attach.StringEmpty Then
            If attach.DirectoryExists Then
                Call localfs.fs(0) _
                    .AttachFolder(attach) _
                    .ToArray
            Else
                'Call localfs.fs(0) _
                '    .AttachFolder(New StreamPack(
                '        buffer:=attach.Open(FileMode.Open, doClear:=False, [readOnly]:=True),
                '        [readonly]:=True
                '    )) _
                '    .ToArray
            End If
        End If

        ' Call BackgroundTaskUtils.BindToMaster(parentId:=parent, kill:=localhost)

        Return localhost.Run
    End Function

    <Extension>
    Private Sub ProcessRequest(localhost As HttpSocket, localfs As WebFileSystemListener, longpollEndpoint As String, request As HttpRequest, response As HttpResponse)
        ' handle the /push endpoint for pushing a message to the
        ' pending long poll connections on the /poll/messages path.
        If request.HTTPMethod = "POST" AndAlso request.URL.path.TextEquals("/push") Then
            Dim payload As String = ""

            If TypeOf request Is HttpPOSTRequest Then
                Dim post As HttpPOSTRequest = DirectCast(request, HttpPOSTRequest)
                payload = post("message").DefaultValue
            End If

            If payload.StringEmpty AndAlso request.URL.query.ContainsKey("message") Then
                payload = request.URL.query("message").ElementAtOrNull(Scan0)
            End If

            Dim delivered As Integer = localhost.LongPoll.PushText(longpollEndpoint, If(payload, ""))

            Call $"long poll push: delivered to {delivered} client(s), message: {payload}.".info()
            response.WriteJSON(New With {.ok = True, .delivered = delivered, .message = payload})
        Else
            ' delegate all of the other requests to the static file handler
            Call localfs.WebHandler(request, response)
        End If
    End Sub

    <ExportAPI("/parse_apache")>
    <Usage("/parse_apache --log <apache_access/error.log> [--save <save.csv>]")>
    Public Function ParseApacheLog(log As String, Optional save As String = Nothing, Optional args As CommandLine = Nothing) As Integer
        Dim logdata As HttpLogEntry() = HttpLogEntry.ParseApacheLogFile(log).ToArray
        Dim save_csv As String = If(save, If(log.FileName.IndexOf("."c) < 0, log & ".csv", log.ChangeSuffix("csv")))

        Return logdata.SaveTo(save_csv).CLICode
    End Function
End Module
