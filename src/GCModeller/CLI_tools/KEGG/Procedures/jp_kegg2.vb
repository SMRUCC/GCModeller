Public MustInherit Class jp_kegg2

    Public ReadOnly Property KEGG As Oracle.LinuxCompatibility.MySQL.MySQL

    Public Const DB_NAME As String = "jp_kegg2"

    Sub New(uri As Oracle.LinuxCompatibility.MySQL.ConnectionUri)
        uri.Database = DB_NAME
        Me.KEGG = uri
        Call $"{Me.KEGG.Ping}ms.....".__DEBUG_ECHO
    End Sub
End Class
