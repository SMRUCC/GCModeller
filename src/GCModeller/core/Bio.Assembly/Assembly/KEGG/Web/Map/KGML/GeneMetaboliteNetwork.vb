Namespace Assembly.KEGG.WebServices.KGML

    Public Class GeneMetaboliteNetwork
        Public Property gene_id As String
        Public Property ko_id As String
        Public Property compound_id As String
        Public Property reaction_id As String
        Public Property pathway_id As String
        Public Property pathway_title As String

        Public Shared Iterator Function ExtractNetwork(kgml As pathway) As IEnumerable(Of GeneMetaboliteNetwork)

        End Function
    End Class
End Namespace