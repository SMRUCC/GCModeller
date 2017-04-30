Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Oracle.LinuxCompatibility.MySQL.CodeSolution
Imports Oracle.LinuxCompatibility.MySQL.Reflection.Schema

Partial Module CLIProgram

    <ExportAPI("/MySQL.Markdown",
               Usage:="/MySQL.Markdown /sql <database.sql> [/out <out.md>]")>
    Public Function MySQLMarkdown(args As CommandLine) As Integer
        Dim sql$ = args <= "/sql"
        Dim out$ = args.GetValue("/out", sql.TrimSuffix & "-dev-docs.md")
        Dim schema As Table() = SQLParser.LoadSQLDoc(path:=sql)
        Dim markdown$ = schema.Documentation
        Return markdown _
            .SaveTo(out, Encoding.UTF8) _
            .CLICode
    End Function
End Module