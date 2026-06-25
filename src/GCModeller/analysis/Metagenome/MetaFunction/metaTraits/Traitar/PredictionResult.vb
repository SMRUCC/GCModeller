Namespace TraitarVB

    Public Class PredictionResult

        Public Property phenotypeId As String
        Public Property accession As String
        Public Property category As String
        Public Property predict As PredictionResults
        Public Property positive As Integer
        Public Property negative As Integer
        Public Property confidence As Double
        Public Property scores As Double()
        Public Property labels As Integer()

    End Class

    Public Enum PredictionResults
        NA
        [TRUE]
        [FALSE]
    End Enum
End Namespace