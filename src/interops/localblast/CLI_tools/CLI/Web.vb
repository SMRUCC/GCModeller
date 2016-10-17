Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult

Partial Module CLI

    <ExportAPI("/Export.AlignmentTable",
               Usage:="/Export.AlignmentTable /in <alignment.txt> [/split /out <outDIR/file>]")>
    <Group(CLIGrouping.WebTools)>
    Public Function ExportWebAlignmentTable(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim isSplit As Boolean = args.GetBoolean("/split")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix)
        Dim tables As IEnumerable(Of AlignmentTable) =
            [in].IterateTables

        If isSplit Then
            out = out & "-EXPORT/"

            For Each table In tables
                Call table.Hits _
                    .SaveTo(out & "/" & table.Query & ".csv")
            Next
        Else
            Return tables _
                .Select(Function(x) x.Hits) _
                .MatrixToList _
                .SaveTo(out & ".csv") _
                .CLICode
        End If

        Return 0
    End Function
End Module