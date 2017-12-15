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

        <Extension>
        Public Function TreeAssign(align As Query, OTU As NamedValue(Of Integer), taxonomy As Dictionary(Of String, otu_taxonomy), Optional min_pct# = 0.3) As gastOUT
            Dim hits = align _
                .SubjectHits _
                .Select(Function(h) taxonomy(h.Name)) _
                .ToArray
            Dim tree As TaxonomyTree = TaxonomyTree.BuildTree(hits.Select(Function(t) t.Taxonomy))
        End Function
    End Module
End Namespace