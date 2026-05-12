Namespace gast

    Public Class TaxonomySort

        Public Property tax_id As String
        Public Property taxonomy As Metagenomics.Taxonomy
        Public Property score As Double

        Public Overrides Function ToString() As String
            Return taxonomy.ToString
        End Function

    End Class
End Namespace