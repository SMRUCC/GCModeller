Imports Microsoft.VisualBasic.Math.LinearAlgebra

''' <summary>
''' The Nonlinear Grid Dynamics System
''' </summary>
Public Class GridSystem

    Public Property A As Vector
    Public Property C As Correlation()

    ''' <summary>
    ''' Evaluate the system dynamics
    ''' </summary>
    ''' <param name="X"></param>
    ''' <returns></returns>
    Public Function Evaluate(X As Vector) As Double
        Dim C As Vector = Me.C.Select(Function(ci) ci.Evaluate(X)).AsVector
        Dim fx As Vector = A * (X ^ C)
        Dim result = fx.Sum

        Return result
    End Function
End Class
