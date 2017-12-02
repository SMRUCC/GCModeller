Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Webservices.Bing

Module CLI

    <ExportAPI("/kb.build.query")>
    <Usage("/kb.build.query /term <term> [/pages <default=20> /out <out.directory>]")>
    Public Function BingAcademicQuery(args As CommandLine) As Integer
        Dim term$ = args <= "/term"
        Dim out$ = args.GetValue("/out", App.CurrentDirectory & "/" & term.NormalizePathString)
        Dim pages% = args.GetValue("/pages", 20)

        Call Academic.Build_KB(term, out, pages)

        Return 0
    End Function
End Module
