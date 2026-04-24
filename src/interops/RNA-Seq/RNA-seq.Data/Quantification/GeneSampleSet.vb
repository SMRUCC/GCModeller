Namespace GeneQuantification

    Public Class GeneSampleSet

        Public Property GeneID As String
        Public Property Chr As String
        Public Property Length As Integer
        Public Property TPM As Dictionary(Of String, Double)
        Public Property FPKM As Dictionary(Of String, Double)

        Default Public ReadOnly Property Vector(sample_ids As IEnumerable(Of String), isFpkm As Boolean) As Double()
            Get
                Return (From id As String
                        In sample_ids
                        Select If(isFpkm, _FPKM(id), _TPM(id))).ToArray()
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{GeneID}@{Chr}"
        End Function
    End Class
End Namespace