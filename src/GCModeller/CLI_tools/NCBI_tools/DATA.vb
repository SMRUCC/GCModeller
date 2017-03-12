Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class MapHits

    <Collection("MapHits",)>
    Public Property MapHits As String()
    Public Property Data As Dictionary(Of String, String)
    Public Property taxid As Integer

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class