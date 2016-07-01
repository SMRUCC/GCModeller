Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language

Public Class ChromosomePartitioningEntry : Inherits ClassObject

    <Column("QueryProtein")>
    Public Property ORF As String
    <Column("Tag")>
    Public Property PartitioningTag As String
End Class