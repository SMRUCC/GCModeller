Namespace SVM

    ''' <summary>
    ''' Class representing a grid square result.
    ''' </summary>
    Public Class GridSquare
        ''' <summary>
        ''' The C value
        ''' </summary>
        Public C As Double
        ''' <summary>
        ''' The Gamma value
        ''' </summary>
        Public Gamma As Double
        ''' <summary>
        ''' The cross validation score
        ''' </summary>
        Public Score As Double

        Public Overrides Function ToString() As String
            Return String.Format("{0} {1} {2}", C, Gamma, Score)
        End Function
    End Class
End Namespace