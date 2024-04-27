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
Imports Flute.Template
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.MIME.application.json

Module Program

    Public Function Main(args As String()) As Integer
        Return GetType(Program).RunCLI(args)
    End Function

    <ExportAPI("/compile")>
    <Description("Compile the html files from a collection of template source files.")>
    <Usage("/compile /view <directory_to_templates> /wwwroot <output_dir_for_html>")>
    Public Function Build(view As String, wwwroot As String, args As CommandLine) As Integer
        Dim viewfiles As String() = view.EnumerateFiles("*.vbhtml").ToArray
        Dim name As String
        Dim vars As New Dictionary(Of String, Object)
        Dim excludes As Index(Of String) = {"view", "wwwroot", "args"}

        For Each arg As NamedValue(Of String) In args.AsEnumerable
            name = arg.Name.Trim("-"c, "/"c, "\"c)

            If name.ToLower Like excludes Then
                Continue For
            End If

            vars(name) = JsonParser.Parse(arg.Value, strictVectorSyntax:=False)
        Next

        For Each template As String In viewfiles
            Call VBHtml _
                .ReadHTML(template, vars) _
                .SaveTo(wwwroot & "/" & template.BaseName & ".html")
        Next

        Return 0
    End Function
End Module

