
Public Class SimilarHit

    Public Property SeqID As String
    Public Property Similar As Dictionary(Of String, Double)

    Public ReadOnly Property IsUniqued As Boolean
        Get
            Return Similar.IsNullOrEmpty
        End Get
    End Property

    Public ReadOnly Property Size As Integer
        Get
            Return Similar.TryCount
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return SeqID
    End Function

End Class
