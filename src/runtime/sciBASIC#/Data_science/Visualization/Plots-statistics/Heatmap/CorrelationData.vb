Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.DataFrame

Namespace Heatmap

    Public Class CorrelationData

        Friend data As DistanceMatrix
        Friend min#, max#
        Friend range As DoubleRange

        Dim reoderKeys As String()

        Public ReadOnly Property Value(i As Integer, j As Integer) As Double
            Get
                If reoderKeys Is Nothing Then
                    Return data(i, j)
                Else
                    Return data(reoderKeys(i), reoderKeys(j))
                End If
            End Get
        End Property

        Sub New(data As DistanceMatrix, Optional range As DoubleRange = Nothing)
            With range Or data _
                .PopulateRows _
                .IteratesALL _
                .ToArray _
                .Range _
                .AsDefault

                min = .Min
                max = .Max

                range = {0, .Max}
            End With

            Me.data = data
            Me.range = range
        End Sub

        Public Function SetKeyOrders(orders As IEnumerable(Of String)) As CorrelationData
            reoderKeys = orders.ToArray
            Return Me
        End Function

        Public Function GetMatrix() As Double()()
            Dim rows As New List(Of Double())

            For Each row As IReadOnlyCollection(Of Double) In data.PopulateRows
                rows.Add(row.ToArray)
            Next

            Return rows.ToArray
        End Function
    End Class

End Namespace


