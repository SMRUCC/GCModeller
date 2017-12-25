Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy

Module TaxonomyTreeTest
    Sub Main()
        Dim tree As New NcbiTaxonomyTree("T:\Resources\NCBI_taxonomy")

        Dim nodes = tree.GetAscendantsWithRanksAndNames(526962, only_std_ranks:=True)

        Pause()
    End Sub
End Module
