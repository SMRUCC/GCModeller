Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace Matrix

    Public Class WeightMatrix

        Friend rowSum As Integer = 1000000
        Friend countsMatrix As Integer()()

        Friend Sub New(countsLists As IList(Of IList(Of Integer)))
            SetCountsMatrixProp(listsToArrays(countsLists))
        End Sub

        Friend Sub New()
        End Sub

        Private Function listsToArrays(countsLists As IList(Of IList(Of Integer))) As Integer()()
            Dim countMatrix = RectangularArray.Matrix(Of Integer)(countsLists.Count, countsLists(0).Count)
            Dim rows As IList(Of Integer()) = countsLists.Select(Function(r) r.ToArray()).ToList()

            For i As Integer = 0 To countsLists.Count - 1
                countMatrix(i) = rows(i)
            Next

            Return countMatrix
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Friend Overridable Sub initMatrix(rows As Integer)
            countsMatrix = RectangularArray.Matrix(Of Integer)(rows, 4)
        End Sub

        Private Sub SetCountsMatrixProp(value As Integer()())
            Dim sumByRow As IList(Of Integer) = Enumerable.Range(0, value.Length) _
                .Select(Function(i)
                            Return value(i).Sum()
                        End Function) _
                .ToList()
            Dim sum As Integer = sumByRow(0)

            countsMatrix = value
            rowSum = sum
        End Sub

        Public Overrides Function ToString() As String
            Return countsMatrix _
                .Select(Function(b)
                            Return String.Format(CStr("{0:D} {1:D} {2:D} {3:D}"), CObj(b(CInt(0))), CObj(b(CInt(1))), CObj(b(CInt(2))), CObj(b(CInt(3))))
                        End Function) _
                .JoinBy(vbLf)
        End Function
    End Class

End Namespace
