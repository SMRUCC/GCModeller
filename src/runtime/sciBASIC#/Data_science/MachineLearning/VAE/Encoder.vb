Imports Microsoft.VisualBasic.Math.LinearAlgebra

Public Class Encoder

    Dim weights As Double()()
    Dim strides As Integer()
    Dim dims As Integer()
    Dim learning_rate As Double

    Public ReadOnly Property loss As Double

    Sub New(kernel As Integer(), strides As Integer())

    End Sub

    ''' <summary>
    ''' initialize weights with Gaussian distribution
    ''' </summary>
    Private Sub init_random()

    End Sub

    Public Sub update(output As Vector, input As Vector)

    End Sub

    Public Sub get_output(output As Vector, input As Vector)

    End Sub

End Class
