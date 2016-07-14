Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Parallel.Linq
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Analysis.Metagenome.gast

Partial Module CLI

    <ExportAPI("/gast")>
    Public Function gastInvoke(args As CommandLine) As Integer
        Return gast.Invoke(args).CLICode
    End Function

    <ExportAPI("/Export.SSU.Refs",
           Usage:="/Export.SSU.Refs /in <ssu.fasta> [/out <out.DIR> /no-suffix]")>
    Public Function ExportSSURefs(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim EXPORT As String =
            If(args.GetBoolean("/no-suffix"),
            [in].TrimFileExt,
            [in].TrimFileExt & ".EXPORT/")
        EXPORT = args.GetValue("/out", EXPORT)
        Return [in].ExportSILVA(EXPORT).CLICode
    End Function

    <ExportAPI("/Export.SSU.Refs.Batch",
               Usage:="/Export.SSU.Refs /in <ssu.fasta.DIR> [/out <out.DIR>]")>
    Public Function ExportSSUBatch(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimDIR & ".EXPORT/")
        Dim api As String = GetType(CLI).API(NameOf(ExportSSURefs))
        Dim CLI As String() =
            LinqAPI.Exec(Of String) <= From fa As String
                                       In ls - l - r - wildcards("*.fna", "*.fasta", "*.fsa", "*.fa", "*.fas") <= [in]
                                       Select $"{api} /in {fa.CliPath} /no-suffix"
        For Each arg As String In CLI
            Call arg.__DEBUG_ECHO
        Next

        Return App.SelfFolks(CLI, LQuerySchedule.Recommended_NUM_THREADS)
    End Function
End Module
