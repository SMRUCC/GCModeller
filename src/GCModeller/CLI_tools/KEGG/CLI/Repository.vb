Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Data

Partial Module CLI

    <ExportAPI("/Maps.Repository.Build")>
    <Usage("/Maps.Repository.Build /imports <directory> [/out <repository.XML>]")>
    <Group(CLIGroups.Repository_cli)>
    Public Function BuildPathwayMapsRepository(args As CommandLine) As Integer
        Dim imports$ = args("/imports")
        Dim out$ = args("/out") Or $"{[imports].TrimDIR}.repository.Xml"

        Return MapRepository _
            .BuildRepository(directory:=[imports]) _
            .GetXml _
            .SaveTo(out, TextEncodings.UTF8WithoutBOM) _
            .CLICode
    End Function

    <ExportAPI("/Build.Reactions.Repository")>
    <Usage("/Build.Reactions.Repository /in <directory> [/out <repository.XML>]")>
    <Group(CLIGroups.Repository_cli)>
    Public Function BuildReactionsRepository(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimDIR}.repository.Xml"

        Return ReactionRepository _
            .ScanModel(directory:=[in]) _
            .GetXml _
            .SaveTo(out, TextEncodings.UTF8WithoutBOM) _
            .CLICode
    End Function

    <ExportAPI("/Build.Compounds.Repository")>
    <Usage("/Build.Compounds.Repository /in <directory> [/out <repository.XML>]")>
    <Group(CLIGroups.Repository_cli)>
    Public Function BuildCompoundsRepository(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimDIR}.repository.Xml"

        Return CompoundRepository _
            .ScanModels(directory:=[in]) _
            .GetXml _
            .SaveTo(out) _
            .CLICode
    End Function
End Module