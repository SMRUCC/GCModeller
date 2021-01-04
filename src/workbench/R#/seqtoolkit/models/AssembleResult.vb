Public Class AssembleResult

    Friend alignments As String()

    Sub New(result As String())
        alignments = result
    End Sub

    Public Function GetAssembledSequence() As String
        Return alignments(Scan0)
    End Function
End Class
