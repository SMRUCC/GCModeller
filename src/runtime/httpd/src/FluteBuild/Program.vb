Imports System.ComponentModel
Imports Flute.Template
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

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

            vars(name) = arg.Value
        Next

        For Each template As String In viewfiles
            Call VBHtml _
                .ReadHTML(template, vars) _
                .SaveTo(wwwroot & "/" & template.BaseName & ".html")
        Next

        Return 0
    End Function
End Module
