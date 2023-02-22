Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

Namespace Metabolism.Metpa

    Public Class pathIds

        Public Property pathwayNames As String()
        Public Property ids As String()

        Public Shared Function FromPathways(pathways As Pathway(), Optional keggId As String = Nothing) As pathIds
            Dim ids = pathways.Select(Function(m) If(keggId.StringEmpty, m.EntryId, keggId & m.briteID)).ToArray
            Dim pathwayNames = pathways.Select(Function(m) m.name.Replace(" - Reference pathway", "")).ToArray

            Return New pathIds With {
                .ids = ids,
                .pathwayNames = pathwayNames
            }
        End Function
    End Class
End Namespace