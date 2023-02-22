Namespace Metabolism.Metpa

    Public Class mset

        Public Property metaboliteNames As String()
        Public Property kegg_id As String()

    End Class

    Public Class msetList

        Public Property list As Dictionary(Of String, mset)

    End Class
End Namespace