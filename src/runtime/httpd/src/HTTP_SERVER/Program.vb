Imports System.ComponentModel
Imports Flute.Http.Core
Imports Flute.Http.FileSystem
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
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
        Dim localfs As New WebFileSystemListener(New FileSystem(wwwroot))
        Dim localhost As New HttpSocket(
            app:=AddressOf localfs.WebHandler,
            port:=port
        )

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

        If Not Tcp.PortIsAvailable(port) Then
            Call Console.WriteLine($"local tcp port(={port}) is in used!")
            Return 500
        Else
            Return localhost.Run
        End If
    End Function
End Module