Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class RankLevelView

    Public Property OTUs As String()
    Public Property TaxonomyName As String
    Public Property Tree As String

    <Meta(GetType(Double))>
    Public Property Samples As Dictionary(Of String, Double)

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class