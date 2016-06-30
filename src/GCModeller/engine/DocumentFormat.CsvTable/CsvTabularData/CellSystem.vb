Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions

Namespace FileStream

    Public Class ObjectiveFunction
        Public Property Direction As String
        <CollectionAttribute("Factors")> Public Property Factors As String()
    End Class

End Namespace