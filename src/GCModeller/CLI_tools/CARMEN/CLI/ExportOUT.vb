Imports SMRUCC.genomics.AnalysisTools.CARMEN
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv

Partial Module CLI

    <ExportAPI("/Export.Anno", Usage:="/Export.Anno /in <inDIR> [/out <out.Csv>]")>
    Public Function ExportAnno(args As CommandLine.CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim out As String = args.GetValue("/out", inDIR & ".CARMEN.Csv")
        Dim rxnDefs As Reaction() = CARMEN.Merge(inDIR, False)
        Return rxnDefs.SaveTo(out).CLICode
    End Function
End Module
