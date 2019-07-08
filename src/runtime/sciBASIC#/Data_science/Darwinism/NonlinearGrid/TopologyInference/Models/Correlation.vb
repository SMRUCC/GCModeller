Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.LinearAlgebra

''' <summary>
''' A linear correlation system
''' </summary>
Public Class Correlation

    Public Property B As Vector

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Evaluate(X As Vector) As Double
        Return (B * X).Sum
    End Function

    Public Overrides Function ToString() As String
        Return B.ToString
    End Function
End Class
