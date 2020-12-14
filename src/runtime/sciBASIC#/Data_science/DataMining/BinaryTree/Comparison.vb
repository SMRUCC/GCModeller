Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.DataFrame

Public Class Comparison

    ReadOnly d As DistanceMatrix
    ReadOnly equalsDbl As Double
    ReadOnly gt As Double

    Sub New(d As DistanceMatrix, equals As Double, gt As Double)
        Me.d = d
        Me.equalsDbl = equals
        Me.gt = gt
    End Sub

    Public Function Compares(x As String, y As String) As Integer
        Dim similarity As Double = d(x, y)

        If similarity >= equalsDbl Then
            Return 0
        ElseIf similarity >= gt Then
            Return 1
        Else
            Return -1
        End If
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetComparer() As Comparison(Of String)
        Return Me
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Narrowing Operator CType(c As Comparison) As Comparison(Of String)
        Return New Comparison(Of String)(AddressOf c.Compares)
    End Operator
End Class
