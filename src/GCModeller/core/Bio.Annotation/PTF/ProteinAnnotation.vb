Namespace Ptf

    Public Class ProteinAnnotation

        Public Property geneId As String
        Public Property description As String
        Public Property attributes As Dictionary(Of String, String())

        Public Overrides Function ToString() As String
            Return $"{geneId}: {description}"
        End Function
    End Class
End Namespace