Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Engine.Definitions

    ''' <summary>
    ''' The baseline value of the flux controls and dynamics
    ''' </summary>
    Public Class FluxBaseline

        Public Property transcriptionBaseline As Double = 100
        Public Property translationBaseline As Double = 10
        Public Property transcriptionCapacity As Double = 1000
        Public Property translationCapacity As Double = 1000
        Public Property proteinMatureBaseline As Double = 1000
        Public Property proteinMatureCapacity As Double = 10000
        Public Property productInhibitionFactor As Double = 1.25E-20

        Public Property tRNAChargeBaseline As Double = 1
        Public Property tRNAChargeCapacity As Double = 10

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Class
End Namespace