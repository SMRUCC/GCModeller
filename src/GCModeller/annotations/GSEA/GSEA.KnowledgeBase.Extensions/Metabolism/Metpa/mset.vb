Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

Namespace Metabolism.Metpa

    Public Class mset

        Public Property metaboliteNames As String()
        Public Property kegg_id As String()

    End Class

    Public Class msetList

        Public Property list As Dictionary(Of String, mset)

        Public Shared Function CountUnique(models As Pathway()) As Integer
            Return Aggregate cpd As NamedValue
                   In models.Select(Function(a) a.compound).IteratesALL
                   Group By cpd.name Into Group
                   Into Count
        End Function

    End Class
End Namespace