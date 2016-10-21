Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language

Namespace Assembly.NCBI.Taxonomy

    Public Structure Ranks

        Public species As TaxonomyNode(),
            genus As TaxonomyNode(),
            family As TaxonomyNode(),
            order As TaxonomyNode(),
            [class] As TaxonomyNode(),
            phylum As TaxonomyNode(),
            superkingdom As TaxonomyNode()

        Sub New(tree As NcbiTaxonomyTree)
            Dim species As New List(Of TaxonomyNode),
                genus As New List(Of TaxonomyNode),
                family As New List(Of TaxonomyNode),
                order As New List(Of TaxonomyNode),
                [class] As New List(Of TaxonomyNode),
                phylum As New List(Of TaxonomyNode),
                superkingdom As New List(Of TaxonomyNode)

            For Each x In tree.Taxonomy
                Select Case x.Value.rank
                    Case NcbiTaxonomyTree.class
                        [class] += x.Value
                    Case NcbiTaxonomyTree.family
                        family += x.Value
                    Case NcbiTaxonomyTree.genus
                        genus += x.Value
                    Case NcbiTaxonomyTree.order
                        order += x.Value
                    Case NcbiTaxonomyTree.phylum
                        phylum += x.Value
                    Case NcbiTaxonomyTree.species
                        species += x.Value
                    Case NcbiTaxonomyTree.superkingdom
                        superkingdom += x.Value
                    Case Nothing
                    Case Else
                        Throw New InvalidConstraintException(x.Value.GetJson)
                End Select

                x.Value.taxid = x.Key
            Next
        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure
End Namespace