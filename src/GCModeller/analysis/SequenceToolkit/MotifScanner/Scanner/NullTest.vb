Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis

Public Class NullTest : Inherits NullHypothesis(Of String)

    ReadOnly length As Integer
    ReadOnly zero As ZERO
    ReadOnly motifSlice As Residue()

    Sub New(zero As ZERO, motifSlice As Residue(), length As Integer, Optional permutation As Integer = 1000)
        Call MyBase.New(permutation)

        Me.motifSlice = motifSlice
        Me.zero = zero
        Me.length = length
    End Sub

    Public Overrides Iterator Function ZeroSet() As IEnumerable(Of String)
        For i As Integer = 1 To Permutation
            Yield zero.NextSequence(length)
        Next
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function Score(x As String) As Double
        Return score(x.ToCharArray, motifSlice)
    End Function

    Friend Overloads Shared Function score(seq As String, PWM As IReadOnlyCollection(Of Residue)) As Double
        Dim total As Double = 0

        For i As Integer = 0 To seq.Length - 1
            total += PWM(i)(seq(i))
        Next

        Return total
    End Function
End Class