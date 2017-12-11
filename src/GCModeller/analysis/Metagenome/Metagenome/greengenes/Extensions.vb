Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.Analysis.Metagenome.gast
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus

Namespace greengenes

    Public Module Extensions

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function OTUgreengenesTaxonomy(blastn As IEnumerable(Of Query),
                                              OTUs As Dictionary(Of String, NamedValue(Of Integer)),
                                              taxonomy As Dictionary(Of String, otu_taxonomy),
                                              Optional min_pct# = 0.97) As IEnumerable(Of gastOUT)

            Return blastn.gastTaxonomyInternal(
                getTaxonomy:=Function(hitName)
                                 Return New Taxonomy(taxonomy(hitName).ToString)
                             End Function,
                getOTU:=OTUs,
                min_pct:=min_pct
            )
        End Function
    End Module
End Namespace