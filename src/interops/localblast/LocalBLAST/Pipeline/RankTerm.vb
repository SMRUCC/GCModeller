Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq

Namespace Pipeline

    Public Class RankTerm

        Public Property queryName As String
        Public Property term As String
        Public Property scores As Dictionary(Of String, Double)

        Public ReadOnly Property score As Double
            Get
                Return scores.SafeQuery.Values.Sum
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{queryName} = {score}"
        End Function

    End Class
End Namespace