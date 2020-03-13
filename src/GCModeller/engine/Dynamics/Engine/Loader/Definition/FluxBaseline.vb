Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Engine.Definitions

    ''' <summary>
    ''' The baseline value of the flux controls and dynamics
    ''' </summary>
    Public Class FluxBaseline

        Public Property transcriptionBaseline As Double = 100
        Public Property transcriptionCapacity As Double = 1000

        ''' <summary>
        ''' 对于翻译过程，因为需要有核糖体来介导，所以就不设置最低下限了
        ''' </summary>
        ''' <returns></returns>
        Public Property translationCapacity As Double = 1000
        Public Property proteinMatureBaseline As Double = 1000
        Public Property proteinMatureCapacity As Double = 10000
        Public Property productInhibitionFactor As Double = 1.25E-20

        Public Property tRNAChargeBaseline As Double = 1
        Public Property tRNAChargeCapacity As Double = 10

        Public Property ribosomeAssemblyBaseline As Double = 5
        Public Property ribosomeDisassemblyBaseline As Double = 3

        Public Property ribosomeAssemblyCapacity As Double = 10
        Public Property ribosomeDisassemblyCapacity As Double = 5

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Class
End Namespace