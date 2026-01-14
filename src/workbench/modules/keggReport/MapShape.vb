Public Class MapShape

    Public Property shape As String
    Public Property location As Double()
    ''' <summary>
    ''' kegg id list
    ''' </summary>
    ''' <returns></returns>
    Public Property entities As String()
    Public Property title As String
    Public Property isEntity As Boolean

    Public Overrides Function ToString() As String
        Return title
    End Function

End Class