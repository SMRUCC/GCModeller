
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy
Imports SMRUCC.genomics.Model.Network.Microbiome

<Package("metagenomics_kit")>
Module metagenomicsKit

    <ExportAPI("compounds.origin.profile")>
    Public Function createEmptyCompoundOriginProfile(taxonomy As NcbiTaxonomyTree, organism As String) As CompoundOrigins
        Return CompoundOrigins.CreateEmptyCompoundsProfile(taxonomy, organism)
    End Function
End Module
