Namespace gast

    Public Class TaxonomySort

        Public Property tax_id As String
        Public Property taxonomy As Metagenomics.Taxonomy
            Get
                Return _tax
            End Get
            Set(value As Metagenomics.Taxonomy)
                _tax = value
                _list = value.CreateTable.Value
            End Set
        End Property

        Public Property score As Double

        Dim _tax As Metagenomics.Taxonomy
        Dim _list As Dictionary(Of String, String)

        Default Public ReadOnly Property taxname(rank As String) As String
            Get
                Return _list(rank)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return taxonomy.ToString
        End Function

    End Class
End Namespace