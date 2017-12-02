Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Webservices.Bing
Imports Microsoft.VisualBasic.Webservices.Bing.Academic
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base

Module CLI

    <ExportAPI("/kb.build.query")>
    <Usage("/kb.build.query /term <term> [/pages <default=20> /out <out.directory>]")>
    Public Function BingAcademicQuery(args As CommandLine) As Integer
        Dim term$ = args <= "/term"
        Dim out$ = args("/out") Or (App.CurrentDirectory & "/" & term.NormalizePathString)
        Dim pages% = args.GetValue("/pages", 20)

        Call Academic.Build_KB(term, out, pages, flat:=False)

        Return 0
    End Function

    <ExportAPI("/kb.abstract")>
    <Usage("/kb.abstract /in <kb.directory> [/min.weight <default=0.05> /out <out.txt>]")>
    Public Function GetKBAbstractInformation(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim minWeight# = args.GetValue("/min.weight", 0.05)
        Dim out$ = args("/out") Or $"{[in].TrimDIR}.abstract(min_weight={minWeight}).xml"
        Dim kb As IEnumerable(Of ArticleProfile) = (ls - l - r - "*.xml" <= [in]).Select(AddressOf LoadXml(Of ArticleProfile))
        Dim abstract = kb.InformationAbstract(minWeight:=minWeight)
        Dim abstractText$ = abstract.Keys.JoinBy(" ")
        Dim scores = abstract.GetJson(indent:=True)

        Return (abstractText & ASCII.LF & ASCII.LF & scores) _
            .SaveTo(out, TextEncodings.UTF8WithoutBOM) _
            .CLICode
    End Function
End Module
