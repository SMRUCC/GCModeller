Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class SNPVcf

    <Column("#CHROM")> Public Property CHROM As String
    Public Property POS As Integer
    Public Property ID As String
    Public Property REF As String
    Public Property ALT As String
    Public Property QUAL As String
    Public Property FILTER As String
    Public Property INFO As String
    Public Property FORMAT As String
    <Meta> Public Property Sequences As Dictionary(Of String, String)

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class
