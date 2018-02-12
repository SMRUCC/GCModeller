Public Class ScoreMatrix

    Public Matrix As Char()()

    Sub New(filePath$)
        Matrix = filePath _
            .ReadAllLines _
            .Select(Function(l) l.Replace(" "c, "").ToArray) _
            .ToArray
    End Sub
End Class
