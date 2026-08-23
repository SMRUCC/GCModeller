
Public Class AdjacencyWeight

    Public Property Target As String
    Public Property Weight As Double

    Sub New()
    End Sub

    Sub New(target As String, Optional w As Double = 1)
        Me.Target = target
        Me.Weight = w
    End Sub

    Public Overrides Function ToString() As String
        Return $"{Target} = {Weight}"
    End Function

End Class