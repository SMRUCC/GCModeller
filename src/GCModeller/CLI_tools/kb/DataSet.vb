Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports RDotNet.Extensions.GCModeller
Imports SMRUCC.genomics.Data

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

    <ExportAPI("/KEGG.maps.background")>
    <Usage("/KEGG.maps.background /in <reference_maps.directory> [/out <gsea_background.rda>]")>
    Public Function KEGGMapsBackground(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in]}/gsea_background.rda"

        Return PathwayRepository.ScanModels([in]) _
            .AsEnumerable _
            .SaveBackgroundRda(rdafile:=out) _
            .CLICode
    End Function
End Module