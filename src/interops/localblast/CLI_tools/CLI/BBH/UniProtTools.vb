Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

Partial Module CLI

    <ExportAPI("/UniProt.bbh.mappings")>
    <Usage("/UniProt.bbh.mappings /in <bbh.csv> [/out <mappings.txt>]")>
    Public Function UniProtBBHMapTable(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.mappings.txt"
        Dim mappings = [in].LoadCsv(Of BiDirectionalBesthit)
        Dim table As Dictionary(Of String, String) = mappings _
            .ToDictionary(Function(query)
                              Return query.QueryName
                          End Function,
                          Function(query)
                              If query.HitName.StringEmpty OrElse query.HitName.TextEquals(HITS_NOT_FOUND) Then
                                  Return ""
                              Else
                                  Return query.HitName.Split("|"c)(1)
                              End If
                          End Function)

        Return table.Tsv(out).CLICode
    End Function
End Module