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

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Class
End Namespace