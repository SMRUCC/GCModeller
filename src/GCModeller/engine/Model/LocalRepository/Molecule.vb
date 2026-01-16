Namespace WebJSON

    Public Class Molecule
        Public Property id As String
        Public Property name As String
        Public Property formula As String
        Public Property db_xrefs As DBXref()
    End Class

    Public Class DBXref

        Public Property dbname As String
        Public Property xref_id As String

    End Class
End Namespace