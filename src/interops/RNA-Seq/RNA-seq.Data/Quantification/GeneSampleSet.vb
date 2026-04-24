Namespace GeneQuantification

    Public Class GeneSampleSet

        Public Property GeneID As String
        Public Property Chr As String
        Public Property Length As Integer
        Public Property TPM As Dictionary(Of String, Double)
        Public Property FPKM As Dictionary(Of String, Double)

    End Class
End Namespace