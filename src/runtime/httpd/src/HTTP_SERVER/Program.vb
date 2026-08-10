#Region "Microsoft.VisualBasic::2e8ca49f88a0f22d4ffd73333949f227, src\HTTP_SERVER\Program.vb"

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

    '   Total Lines: 64
    '    Code Lines: 44 (68.75%)
    ' Comment Lines: 11 (17.19%)
    '    - Xml Docs: 36.36%
    ' 
    '   Blank Lines: 9 (14.06%)
    '     File Size: 2.30 KB


    ' Module Program
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: listen, listenCurrentFolder, Main
    ' 
    ' /********************************************************************************/

#End Region

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

        ' check port availability BEFORE creating the server to avoid
        ' a race condition where another process grabs the port between
        ' the check and the actual bind.
        If Not Tcp.PortIsAvailable(port) Then
            Call Console.WriteLine($"local tcp port(={port}) is in used!")
            Return 500
        End If

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

        Return localhost.Run
    End Function
End Module
