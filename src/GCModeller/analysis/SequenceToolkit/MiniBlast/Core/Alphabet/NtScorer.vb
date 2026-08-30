Namespace Core

    ''' <summary>核酸打分器</summary>
    Public Class NtScorer : Implements IScorer

        Private ReadOnly _table(4, 4) As Double

        ''' <summary>reward：匹配加分；penalty：错配扣分（负值）</summary>
        Public Sub New(reward As Double, penalty As Double)
            For i As Integer = 0 To 4
                For j As Integer = 0 To 4
                    If i <= 3 AndAlso i = j Then
                        _table(i, j) = reward
                    Else
                        _table(i, j) = penalty
                    End If
                Next
            Next
        End Sub

        Public Function Score(a As Int32, b As Int32) As Double Implements IScorer.Score
            Return _table(a, b)
        End Function

    End Class

End Namespace