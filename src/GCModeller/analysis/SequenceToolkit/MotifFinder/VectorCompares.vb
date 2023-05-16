Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.genomics.Model.MotifGraph

Public Class VectorCompares

    ReadOnly cache As New Dictionary(Of String, Vector)

    Public Function Compare(q$, s$) As Double
        Dim g1 = GetVector(q)
        Dim g2 = GetVector(s)
        Dim score As Double = SSM(g1, g2)

        Return score
    End Function

    Private Function GetVector(s As String) As Vector
        If Not cache.ContainsKey(s) Then
            cache.Add(s, Builder.SequenceGraph(s, SequenceModel.NT).GetVector(SequenceModel.NT))
        End If

        Return cache(s)
    End Function
End Class