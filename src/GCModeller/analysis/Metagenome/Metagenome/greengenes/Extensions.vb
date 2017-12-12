Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.Analysis.Metagenome.gast
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus

Namespace greengenes

    Public Module Extensions

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="blastn"></param>
        ''' <param name="OTUs"></param>
        ''' <param name="taxonomy"></param>
        ''' <returns></returns>
        ''' <remarks>使用gast最多只能够注释到species，不能够具体的注释到某一个菌株？？</remarks>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function OTUgreengenesTaxonomy(blastn As IEnumerable(Of Query), OTUs As Dictionary(Of String, NamedValue(Of Integer)), taxonomy As Dictionary(Of String, otu_taxonomy)) As IEnumerable(Of gastOUT)
            Return blastn.gastTaxonomyInternal(
                getTaxonomy:=Function(hitName)
                                 Return New Taxonomy(taxonomy(hitName).ToString)
                             End Function,
                getOTU:=OTUs
            )
        End Function
    End Module
End Namespace