Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.ComponentModel

Namespace Assembly.KEGG.DBGET.BriteHEntry

    Public Class KOCatalog : Inherits CatalogProfiling

        Public Property [Class] As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace