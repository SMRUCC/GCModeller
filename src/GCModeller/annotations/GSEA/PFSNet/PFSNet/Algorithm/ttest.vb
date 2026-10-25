Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis

''' <summary>
''' Statistical helpers used by the PFSNet implementation.
''' </summary>
Public Module PFSNetStatistics

    ''' <summary>
    ''' Computes the paired Student t statistic for two equally sized samples.
    ''' Non-finite pairs are omitted in the same way as missing paired values.
    ''' </summary>
    ''' <param name="x">The first score vector.</param>
    ''' <param name="y">The paired second score vector.</param>
    ''' <returns>The paired t statistic, or <see cref="Double.NaN"/> when fewer than two valid pairs remain.</returns>
    Public Function PairedTTest(x As Double(), y As Double()) As Double
        If x Is Nothing Then
            Throw New ArgumentNullException(NameOf(x))
        ElseIf y Is Nothing Then
            Throw New ArgumentNullException(NameOf(y))
        ElseIf x.Length <> y.Length Then
            Throw New ArgumentException("Paired samples must have the same length.")
        End If

        Dim differences As New System.Collections.Generic.List(Of Double)(x.Length)

        For i As Integer = 0 To x.Length - 1
            If Double.IsFinite(x(i)) AndAlso Double.IsFinite(y(i)) Then
                differences.Add(x(i) - y(i))
            End If
        Next

        If differences.Count < 2 Then
            Return Double.NaN
        End If

        Return t.Test(differences, mu:=0).TestValue
    End Function
End Module
