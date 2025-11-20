Public Class CompoundInfo

    Public Property id As String
    Public Property name As String
    Public Property db_xrefs As String()

    Public Overrides Function ToString() As String
        Return name
    End Function

End Class
