Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports SMRUCC.genomics.Data.Functional

Partial Module CLI

    <ExportAPI("/ARO.fasta.header.table")>
    <Usage("/ARO.fasta.header.table /in <directory> [/out <out.csv>]")>
    <Group(CLIGroups.AntibioticResistance_cli)>
    Public Function AROSeqTable(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimDIR}.ARO_fasta.headers.csv"
        Dim table = CARDdata.FastaParser([in]).ToArray

        Call table.Keys.Distinct.FlushAllLines(out.TrimSuffix & ".accession.list")
        Return table.SaveTo(out).CLICode
    End Function
End Module