Imports Microsoft.VisualBasic.Language

Namespace Assembly.KEGG.DBGET.bGetObject

    ''' <summary>
    ''' dbget-bin/www_bget
    ''' </summary>
    Public MustInherit Class bGetObject : Inherits ClassObject

        Public MustOverride ReadOnly Property Code As String

        Public Property Entry As String
        Public Property Name As String
        Public Property Definition As String

    End Class
End Namespace