Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult

Partial Module CLI

    <ExportAPI("/Export.AlignmentTable",
               Usage:="/Export.AlignmentTable /in <alignment.txt> [/split /header.split /out <outDIR/file>]")>
    <Group(CLIGrouping.WebTools)>
    Public Function ExportWebAlignmentTable(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim isSplit As Boolean = args.GetBoolean("/split")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix)
        Dim headerSplit? = args.GetBoolean("/header.split")
        Dim tables As IEnumerable(Of AlignmentTable) =
            [in].IterateTables(headerSplit)

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

    <ExportAPI("/Export.AlignmentTable.giList",
               Usage:="/Export.AlignmentTable.giList /in <table.csv> [/out <gi.txt>]")>
    <Group(CLIGrouping.WebTools)>
    Public Function ParseAlignmentTableGIlist(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-gi-list.txt")
        Dim table = [in].LoadCsv(Of HitRecord)
        Dim list$() = table _
            .Select(Function(x) x.GI) _
            .MatrixAsIterator _
            .Distinct _
            .OrderBy(Function(s) s) _
            .ToArray

        Return list.FlushAllLines(out).CLICode
    End Function
End Module