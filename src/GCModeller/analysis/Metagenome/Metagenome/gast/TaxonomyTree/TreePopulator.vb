Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Metagenomics

Namespace gast

    Module TreePopulator

        <Extension>
        Public Function PopulateTaxonomy(root As TaxonomyTree, level As TaxonomyRanks) As IEnumerable(Of TaxonomyTree)
            Return root.PopulateNodeAtRank(level)
        End Function

        <Extension>
        Private Iterator Function PopulateNodeAtRank(root As TaxonomyTree, level As TaxonomyRanks) As IEnumerable(Of TaxonomyTree)
            Dim current As TaxonomyTree() = root.childs

            For Each node As TaxonomyTree In current
                If node.depth = level Then
                    Yield node
                Else
                    For Each item In PopulateNodeAtRank(root:=node, level:=level)
                        Yield item
                    Next
                End If
            Next
        End Function
    End Module
End Namespace