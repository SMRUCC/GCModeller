Namespace NeuralNetwork

    Public Structure TrainingSample

        Dim sampleID As String
        Dim sample As Double()

        ''' <summary>
        ''' The output result.
        ''' </summary>
        Dim classify As Double()

        Public ReadOnly Property isEmpty As Boolean
            Get
                Return sample.IsNullOrEmpty OrElse classify.IsNullOrEmpty
            End Get
        End Property

    End Structure
End Namespace