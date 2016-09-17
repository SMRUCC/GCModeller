Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    <ExportAPI("/seqdiff", Usage:="/seqdiff /in <mla.fasta> [/winsize 250 /steps 50 /slides 5 /out <out.csv>]")>
    Public Function SeqDiffCLI(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim winsize As Integer = args.GetValue("/winsize", 250)
        Dim steps As Integer = args.GetValue("/steps", 50)
        Dim slides As Integer = args.GetValue("/slides", 5)
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & $".winsize={winsize},steps={steps},slides={slides}_seqdiff.csv")
        Dim mla As New FastaFile([in])
        Dim result = mla.ToArray(AddressOf SeqDiff.Parser)

        Call SeqDiff.GCOutlier(mla, result, {0.95, 0.99, 1}, winsize, steps, slides)

        Return result.SaveTo(out).CLICode
    End Function
End Module
