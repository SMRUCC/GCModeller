Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

Partial Module CLI

    <ExportAPI("/UniProt.bbh.mappings")>
    <Usage("/UniProt.bbh.mappings /in <bbh.csv> [/reverse /out <mappings.txt>]")>
    Public Function UniProtBBHMapTable(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.mappings.txt"
        Dim mappings = [in].LoadCsv(Of BiDirectionalBesthit)
        Dim reversed As Boolean = args("/reverse")
        Dim table As Dictionary(Of String, String) = mappings _
            .Where(Function(query)
                       Return Not (query.HitName.StringEmpty OrElse query.HitName.TextEquals(HITS_NOT_FOUND))
                   End Function) _
            .ToDictionary(Function(query)
                              Return query.QueryName
                          End Function,
                          Function(query)
                              Return query.HitName.Split("|"c)(1)
                          End Function)

        Return table.Tsv(out,, reversed:=reversed).CLICode
    End Function
End Module