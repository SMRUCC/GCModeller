Imports SMRUCC.genomics.Metagenomics

Namespace metaTraits

    Public Class metaTraitData

        Public Property taxon_id As UInteger
        Public Property taxon_name As String
        Public Property taxon_lineage As Taxonomy
        Public Property traits As TraitData()

        Public Overrides Function ToString() As String
            Return $"{taxon_lineage} [{traits.TryCount} traits]"
        End Function

    End Class
End Namespace