Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports RDotNET.Extensions.GCModeller

Partial Module CLI

    <ExportAPI("/KEGG.compound.rda")>
    <Usage("/KEGG.compound.rda /repo <directory> [/out <save.rda>]")>
    <Description("Create a kegg organism-compound maps dataset and save in rda file.")>
    Public Function KEGGCompoundDataSet(args As CommandLine) As Integer
        Dim in$ = args <= "/repo"
        Dim dataset = OrganismCompounds.LoadData(repo:=[in])
        Dim out$ = args("/out") Or $"{[in].TrimDIR}/{dataset.code}.rda"

        Return dataset _
            .DoCall(Function(d) OrganismCompounds.WriteRda(d, rdafile:=out)) _
            .CLICode
    End Function
End Module