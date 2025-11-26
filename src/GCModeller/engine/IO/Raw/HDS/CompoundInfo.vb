Public Class CompoundInfo

    Public Property id As String
    Public Property name As String
    Public Property db_xrefs As String()

    Sub New()
    End Sub

    Sub New(id As String, name As String)
        _id = id
        _name = name
    End Sub

    Public Overrides Function ToString() As String
        Return name
    End Function

End Class
