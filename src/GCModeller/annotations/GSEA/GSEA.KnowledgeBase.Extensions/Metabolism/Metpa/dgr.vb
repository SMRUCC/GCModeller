Namespace Metabolism.Metpa

    Public Class dgr

        Public Property kegg_id As String()
        Public Property dgr As Double()

    End Class

    Public Class dgrList

        Public Property pathways As Dictionary(Of String, dgr)

    End Class
End Namespace