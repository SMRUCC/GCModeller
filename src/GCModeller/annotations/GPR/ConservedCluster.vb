Public Class ConservedCluster

    Public Property ClusterID As String

    Public Property geneIDs As String()
    Public Property functions As String()

    Public ReadOnly Property GeneSetSize As Integer
        Get
            Return geneIDs.TryCount
        End Get
    End Property

End Class
