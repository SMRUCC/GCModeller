#Region "Microsoft.VisualBasic::410dee79aa12bc812f4e608c834768ec, G:/GCModeller/src/runtime/httpd/src/FluteBuild//Program.vb"

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

    '   Total Lines: 42
    '    Code Lines: 34
    ' Comment Lines: 0
    '   Blank Lines: 8
    '     File Size: 1.57 KB


    ' Module Program
    ' 
    '     Function: Build, Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.IO
Imports System.Threading
Imports Flute.Template
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.Net.Http

Module Program

    Public Function Main(args As String()) As Integer
        Return GetType(Program).RunCLI(args)
    End Function

    <ExportAPI("/compile")>
    <Description("Compile the html files from a collection of template source files.")>
    <Usage("/compile /view <directory_to_templates> /wwwroot <output_dir_for_html> [--listen]")>
    Public Function Build(view As String, wwwroot As String, args As CommandLine) As Integer
        Dim name As String
        Dim vars As New Dictionary(Of String, Object)
        Dim excludes As Index(Of String) = {"view", "wwwroot", "args", "listen"}
        Dim listenMode As Boolean = args("--listen")

        For Each arg As NamedValue(Of String) In args.AsEnumerable
            name = arg.Name.Trim("-"c, "/"c, "\"c)

            If name.ToLower Like excludes Then
                Continue For
            End If

            vars(name) = JsonParser.Parse(arg.Value, strictVectorSyntax:=False)
        Next

        If listenMode Then
            Dim watcher As New FileSystemWatcher() With {
                .Path = view,
                .Filter = "*.*",
                .NotifyFilter = NotifyFilters.LastWrite Or NotifyFilters.Size Or NotifyFilters.FileName
            }

            AddHandler watcher.Changed, Sub(sender, e)
                                            Call "build template folder on updates...".debug
                                            Call build(view, wwwroot, vars)
                                        End Sub

            AddHandler watcher.Created, Sub(sender, e)
                                            Call "build template folder on updates...".debug
                                            Call build(view, wwwroot, vars)
                                        End Sub

            watcher.EnableRaisingEvents = True

            Console.WriteLine($"Listening of the html template workdir: {view.GetDirectoryFullPath}")
            Console.WriteLine("Press key 'q' to exit...")

            ' 保持控制台运行
            While Console.Read() <> Asc("q")
                Call Thread.Sleep(500)
            End While
        Else
            Call build(view, wwwroot, vars)
        End If

        Return 0
    End Function

    Private Sub build(view As String, wwwroot As String, vars As Dictionary(Of String, Object))
        Dim viewfiles As String() = view.EnumerateFiles("*.vbhtml").ToArray

        For Each template As String In viewfiles
            Try
                Call VBHtml _
                    .ReadHTML(template, vars) _
                    .SaveTo(wwwroot & "/" & template.BaseName & ".html")
            Catch ex As Exception
                Call ex.Message.warning
            End Try
        Next
    End Sub

    <ExportAPI("/stress_test")>
    <Usage("/stress_test /url <test_url> [/batch_size <default=10000> /post]")>
    Public Function StressTest(url As String,
                               Optional batch_size As Integer = 10000,
                               Optional post As Boolean = False,
                               Optional args As CommandLine = Nothing) As Integer

        Dim buildAction As Action(Of Integer)
        Dim urldata As New URL(url)

        If post Then
            buildAction = Sub(i) Call New URL(urldata).Refresh(i).ToString.POST
        Else
            buildAction = Sub(i) Call New URL(urldata).Refresh(i).ToString.GET
        End If

        Call Parallel.For(0, batch_size, buildAction)
        Return 0
    End Function
End Module

