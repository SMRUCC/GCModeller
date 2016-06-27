Imports Microsoft.VisualBasic.ComponentModel

Namespace Assembly.Uniprot.Web

    Public Class Entry
        Public Property Entry As KeyValuePair
        Public Property EntryName As String
        Public Property StatusReviewed As Boolean
        Public Property ProteinNames As String
        Public Property GeneNames As String
        Public Property Organism As String
        Public Property Length As String

        Public Overrides Function ToString() As String
            Return Entry.ToString
        End Function
    End Class
End Namespace