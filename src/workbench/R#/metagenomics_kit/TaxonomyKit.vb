
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy

<Package("taxonomy_kit")>
Module TaxonomyKit

    Public Function LoadNcbiTaxonomyTree(repo$) As NcbiTaxonomyTree
        Return New NcbiTaxonomyTree(repo)
    End Function

End Module
