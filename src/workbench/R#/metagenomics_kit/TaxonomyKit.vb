
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy

<Package("taxonomy_kit")>
Module TaxonomyKit

    <ExportAPI("Ncbi.taxonomy_tree")>
    Public Function LoadNcbiTaxonomyTree(repo$) As NcbiTaxonomyTree
        Return New NcbiTaxonomyTree(repo)
    End Function

End Module
