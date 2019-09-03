Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Organism
Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.HTS.GSEA.KnowledgeBase

Partial Module CLI

    <ExportAPI("/kegg.metabolites.background")>
    <Description("Create background model for KEGG pathway enrichment based on the kegg metabolites, used for LC-MS metabolism data analysis.")>
    <Usage("/kegg.metabolites.background /in <organism.repository_directory> [/out <background_model.Xml>]")>
    Public Function MetaboliteBackground(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in]}/metabolites.Xml"
        Dim info As OrganismInfo = $"{[in]}/kegg.json".LoadJSON(Of OrganismInfo)
        Dim maps As IEnumerable(Of Pathway) = (ls - l - r - "*.Xml" <= [in]).Select(AddressOf LoadXml(Of Pathway))
        Dim background As Background = KEGGCompounds.CreateBackground(info, maps)

        Return background _
            .GetXml _
            .SaveTo(out) _
            .CLICode
    End Function
End Module