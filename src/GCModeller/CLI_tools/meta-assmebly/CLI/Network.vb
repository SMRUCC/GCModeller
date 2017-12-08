Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection

Partial Module CLI

    <ExportAPI("/microbiome.metabolic.network")>
    <Usage("/microbiome.metabolic.network /metagenome <list.txt/OTU.tab> /ref <reaction.repository.XML> /uniprot <repository.directory> [/out <network.directory>]")>
    Public Function MetabolicComplementationNetwork(args As CommandLine) As Integer

    End Function
End Module