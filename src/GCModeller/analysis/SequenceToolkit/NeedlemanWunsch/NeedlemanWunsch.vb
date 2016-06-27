Public Class NeedlemanWunsch : Inherits NeedlemanWunsch(Of Char)

    Sub New(query As String, subject As String)
        Call MyBase.New(AddressOf __charEquals, "-"c, Function(x) x)

        Me.Sequence1 = query.ToCharArray
        Me.Sequence2 = subject.ToCharArray
    End Sub

    Private Shared Function __charEquals(a As Char, b As Char) As Boolean
        Return a = b
    End Function
End Class
