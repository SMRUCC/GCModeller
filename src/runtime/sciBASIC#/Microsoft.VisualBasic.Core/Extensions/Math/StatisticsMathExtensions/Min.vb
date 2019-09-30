Namespace Math.Statistics

    Public Class Min

        Public ReadOnly Property MinValue As Double

        Sub New(value As Double)
            MinValue = value
        End Sub

        Public Sub Add(sample As Double)
            If sample < MinValue Then
                _MinValue = sample
            End If
        End Sub
    End Class
End Namespace