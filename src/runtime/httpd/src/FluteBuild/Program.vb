Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection

Module Program

    Public Function Main(args As String()) As Integer
        Return GetType(Program).RunCLI(args)
    End Function

    <ExportAPI("/compile")>
    <Description("Compile the html files from a collection of template source files.")>
    <Usage("/compile /view <directory_to_templates> /wwwroot <output_dir_for_html>")>
    Public Function Build(view As String, wwwroot As String, args As CommandLine) As Integer

    End Function
End Module
