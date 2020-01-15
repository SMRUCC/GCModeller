
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy
Imports Taxonomy = SMRUCC.genomics.Metagenomics.Taxonomy
Imports REnv = SMRUCC.Rsharp.Runtime.Internal.ConsolePrinter

<Package("taxonomy_kit")>
Module TaxonomyKit

    Sub New()
        REnv.AttachConsoleFormatter(Of Taxonomy)(AddressOf printTaxonomy)
    End Sub

    Private Function printTaxonomy(taxonomy As Taxonomy) As String
        Return $"<{taxonomy.lowestLevel}> {taxonomy.ToString(BIOMstyle:=True)}"
    End Function

    <ExportAPI("Ncbi.taxonomy_tree")>
    Public Function LoadNcbiTaxonomyTree(repo$) As NcbiTaxonomyTree
        Return New NcbiTaxonomyTree(repo)
    End Function

End Module
