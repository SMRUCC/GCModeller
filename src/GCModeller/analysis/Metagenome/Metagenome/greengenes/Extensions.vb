Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.genomics.Analysis.Metagenome.gast
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus
Imports SMRUCC.genomics.Metagenomics

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
                                 Return New gast.Taxonomy(taxonomy(hitName).ToString)
                             End Function,
                getOTU:=OTUs,
                min_pct:=min_pct
            )
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Iterator Public Function OTUgreengenesTaxonomyTreeAssign(blastn As IEnumerable(Of Query),
                                                                 OTUs As Dictionary(Of String, NamedValue(Of Integer)),
                                                                 taxonomy As Dictionary(Of String, otu_taxonomy),
                                                                 Optional min_pct# = 0.97) As IEnumerable(Of gastOUT)
            For Each query As Query In blastn
                If Not OTUs.ContainsKey(query.QueryName) Then
                    Continue For
                Else
                    Dim result As gastOUT = query.TreeAssign(OTUs(query.QueryName), taxonomy, min_pct)

                    If result.rank <> "NA" Then
                        Yield result
                    End If
                End If
            Next
        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="align"></param>
        ''' <param name="OTU"></param>
        ''' <param name="taxonomy"></param>
        ''' <param name="min_pct#">
        ''' ``[0-1]`` or ``[0-100]``
        ''' 
        ''' 0.97 equals to 97
        ''' </param>
        ''' <returns></returns>
        <Extension>
        Public Function TreeAssign(align As Query, OTU As NamedValue(Of Integer), taxonomy As Dictionary(Of String, otu_taxonomy), Optional min_pct# = 0.05) As gastOUT
            Dim hits = align _
                .SubjectHits _
                .Select(Function(h) taxonomy(h.Name)) _
                .ToArray
            Dim tax_counts%() = Nothing
            Dim minrank$ = Nothing
            Dim tree As TaxonomyTree = TaxonomyTree.BuildTree(
                hits:=hits.Select(Function(t) t.Taxonomy),
                taxa_counts:=tax_counts,
                minrank:=minrank
            )
            Dim cutoff% = If(min_pct > 1, min_pct / 100, min_pct) * hits.Length
            Dim n As New List(Of Integer)

            ' 遍历整颗树，取hits最大的分支作为最终的赋值结果
            Do While tree.hits >= cutoff AndAlso tree.Childs.Count > 0
                tree = tree.Childs.OrderByDescending(Function(t) t.hits).First
                n += tree.hits
            Loop

            Dim rank As TaxonomyRanks

            Call tree.GetDepth(rank)

            Dim pcts As Vector = Vector.round((New Vector(n) / hits.Length), 2) * 100
            Dim result As New gastOUT With {
                .taxonomy = DirectCast(tree, gast.Taxonomy).BIOMTaxonomyString.Trim(";"c),
                .counts = OTU.Value,
                .minrank = minrank,
                .read_id = OTU.Name,
                .refhvr_ids = align.QueryName,
                .max_pcts = pcts.JoinBy(";"),
                .refssu_count = hits.Length,
                .taxa_counts = tax_counts.JoinBy(";"),
                .na_pcts = (100 - pcts).JoinBy(";"),
                .rank = rank.ToString,
                .vote = n.JoinBy(";")
            }

            Return result
        End Function
    End Module
End Namespace