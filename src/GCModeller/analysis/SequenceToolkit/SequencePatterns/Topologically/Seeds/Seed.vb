Namespace Topologically.Seeding

    Public Structure Seed

        Public ReadOnly Property Sequence As String
        Public ReadOnly Property Parent As String

        Sub New(seq$)
            Sequence = seq
            Parent = Mid(seq, 1, seq.Length - 1)
        End Sub

        Public Function IsMyParent(seed$) As Boolean
            Return InStr(Sequence, seed) = 1
        End Function

        Public Overrides Function ToString() As String
            Return Parent & " --> " & Sequence
        End Function
    End Structure
End Namespace