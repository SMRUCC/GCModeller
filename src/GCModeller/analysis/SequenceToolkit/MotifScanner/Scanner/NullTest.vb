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

    Public Overrides Function Score(x As String) As Double
        Return ProbabilityScanner.score(x.ToCharArray, motifSlice)
    End Function
End Class