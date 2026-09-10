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

Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.IO
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Flute.Http
Imports Flute.Http.Configurations
Imports Flute.Http.Core
Imports Flute.Http.Core.Message
Imports Flute.Http.FileSystem
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Net
Imports FluteHttpHeader = Flute.Http.Core.Message.HttpHeader

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

    ''' <summary>
    ''' run a dynamically loaded web application module: the given dll is loaded
    ''' through reflection, every http controller it contains (a public class
    ''' with HttpGet/HttpPost/HttpPut/HttpDelete annotated methods, or a class
    ''' implementing <see cref="IHttpAppModule"/>) is instantiated and registered
    ''' into the router, then the http server is started.
    ''' </summary>
    <ExportAPI("/run")>
    <Description("Run a dynamically loaded web application module (a controller class library) on this http server")>
    <Usage("/run --app <app.dll> [--listen <port, default=80> --wwwroot <directory_path> --data <data_directory> --config <config.ini> --max-post-size <bytes> --base-url <http://host>]")>
    Public Function Run(args As CommandLine) As Integer
        Dim appModule As String = args("--app")

        If appModule.StringEmpty Then
            Call Console.WriteLine("missing required argument: --app <app.dll>")
            Return 404
        End If

        Dim configs As Dictionary(Of String, String) = loadRunConfiguration(args)

        Dim apiFile As String = appModule
        If Not Path.IsPathRooted(apiFile) Then
            apiFile = Path.GetFullPath(apiFile)
        End If
        If Not File.Exists(apiFile) Then
            Call Console.WriteLine($"application module not found: {apiFile}")
            Return 404
        End If

        Dim port As Integer = configValue(configs, "listen", 80)
        Dim wwwroot As String = configValue(configs, "wwwroot", App.CurrentDirectory)
        Dim data As String = configValue(configs, "data", Path.Combine(App.CurrentDirectory, "data"))
        Dim maxPostSize As Integer = configValue(configs, "max-post-size", 0)

        If Not Tcp.PortIsAvailable(port) Then
            Call Console.WriteLine($"local tcp port(={port}) is in used!")
            Return 500
        End If

        Dim assembly As Assembly
        Try
            assembly = Assembly.LoadFrom(apiFile)
        Catch ex As Exception
            Call App.LogException(ex)
            Call Console.WriteLine($"failed to load application module: {ex.Message}")
            Return 500
        End Try

        Dim wfs As New WebFileSystemListener(wwwroot)
        Dim router As New HttpRouter()
        Call router.MountFs(wfs)

        ' map the data directory as a virtual static resource folder so that the
        ' downloaded package files can also be served as plain static files.
        Dim packagesDir As String = Path.Combine(data, "packages")
        If packagesDir.DirectoryExists Then
            Call wfs.fs(0).AttachFolder(packagesDir, "/packages/").ToArray
            Call $"attached static packages folder: {packagesDir} -> /packages/".info()
        End If

        Dim modules As Integer = 0
        For Each type As Type In getLoadableTypes(assembly)
            If Not isHttpController(type) Then
                Continue For
            End If

            Dim controller As Object
            Try
                controller = Activator.CreateInstance(type)
            Catch ex As Exception
                Call App.LogException(ex)
                Call $"skip controller '{type.FullName}': {ex.Message}".warning()
                Continue For
            End Try

            Call router.RegisterController(controller)

            Dim [module] As IHttpAppModule = TryCast(controller, IHttpAppModule)
            If [module] IsNot Nothing Then
                Call [module].Mount(router, configs)
            End If

            modules += 1
            Call $"controller mounted: {type.FullName}".info()
        Next

        If modules = 0 Then
            Call Console.WriteLine($"no http controller found in '{apiFile}'")
            Return 404
        End If

        Dim settings As New Configuration With {
            .silent = False,
            .session = New Session()
        }
        If maxPostSize > 0 Then
            settings.max_post_size = maxPostSize
        End If

        Dim localhost As New HttpSocket(router, port, configs:=settings)

        Call $"http server started: http://localhost:{port}/ (wwwroot={wwwroot}, data={data})".info()

        Return localhost.Run
    End Function

    ''' <summary>
    ''' build the runtime configuration dictionary from an optional ini-like
    ''' config file (lowest priority) overridden by the command line arguments.
    ''' </summary>
    Private Function loadRunConfiguration(args As CommandLine) As Dictionary(Of String, String)
        Dim configs As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

        Dim configFile As String = args("--config")
        If Not configFile.StringEmpty AndAlso File.Exists(configFile) Then
            For Each line As String In File.ReadAllLines(configFile)
                Dim text As String = line.Trim()

                If text.StringEmpty OrElse text.StartsWith("#"c) OrElse text.StartsWith(";"c) Then
                    Continue For
                End If

                Dim i As Integer = text.IndexOf("="c)
                If i > 0 Then
                    configs(text.Substring(0, i).Trim()) = text.Substring(i + 1).Trim()
                End If
            Next
        End If

        Call setConfig(configs, "app", args("--app"))
        Call setConfig(configs, "listen", args("--listen"))
        Call setConfig(configs, "wwwroot", args("--wwwroot"))
        Call setConfig(configs, "data", args("--data"))
        Call setConfig(configs, "base-url", args("--base-url"))
        Call setConfig(configs, "max-post-size", args("--max-post-size"))
        Call setConfig(configs, "config", configFile)

        Return configs
    End Function

    Private Sub setConfig(configs As Dictionary(Of String, String), name As String, value As String)
        If Not value.StringEmpty Then
            configs(name) = value
        End If
    End Sub

    Private Function configValue(configs As Dictionary(Of String, String), name As String, fallback As Integer) As Integer
        Dim value As String = Nothing
        If configs.TryGetValue(name, value) AndAlso Not value.StringEmpty Then
            Dim parsed As Integer
            If Integer.TryParse(value, parsed) Then
                Return parsed
            End If
        End If
        Return fallback
    End Function

    Private Function configValue(configs As Dictionary(Of String, String), name As String, fallback As String) As String
        Dim value As String = Nothing
        If configs.TryGetValue(name, value) AndAlso Not value.StringEmpty Then
            Return value
        End If
        Return fallback
    End Function

    ''' <summary>
    ''' test whether the given type is an http controller: either it implements
    ''' <see cref="IHttpAppModule"/> or it exposes at least one public instance
    ''' method annotated with one of the http route attributes.
    ''' </summary>
    Private Function isHttpController(type As Type) As Boolean
        If type Is Nothing OrElse Not type.IsClass OrElse type.IsAbstract Then
            Return False
        End If
        If GetType(IHttpAppModule).IsAssignableFrom(type) Then
            Return True
        End If

        For Each method As MethodInfo In type.GetMethods(BindingFlags.Public Or BindingFlags.Instance)
            If method.GetCustomAttribute(Of FluteHttpHeader.HttpGet)() IsNot Nothing OrElse
               method.GetCustomAttribute(Of FluteHttpHeader.HttpPost)() IsNot Nothing OrElse
               method.GetCustomAttribute(Of FluteHttpHeader.HttpPut)() IsNot Nothing OrElse
               method.GetCustomAttribute(Of FluteHttpHeader.HttpDelete)() IsNot Nothing Then
                Return True
            End If
        Next

        Return False
    End Function

    ''' <summary>
    ''' get all loadable types of the given assembly, ignoring the types that
    ''' failed to load (for example a missing optional dependency).
    ''' </summary>
    Private Function getLoadableTypes(assembly As Assembly) As Type()
        Try
            Return assembly.GetTypes()
        Catch ex As ReflectionTypeLoadException
            Dim list As New List(Of Type)
            For Each type As Type In ex.Types
                If type IsNot Nothing Then
                    list.Add(type)
                End If
            Next
            Return list.ToArray
        End Try
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
