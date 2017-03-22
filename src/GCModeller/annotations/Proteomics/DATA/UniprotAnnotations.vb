Imports Microsoft.VisualBasic.Serialization.JSON

Public Class UniprotAnnotations

    Public Property ID As String
    Public Property uniprot As String
    Public Property ORF As String
    Public Property geneName As String
    Public Property fullName As String
    Public Property GO As String()
    Public Property EC As String()
    Public Property KO As String
    Public Property organism As String
    Public Property Data As Dictionary(Of String, String)

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class
