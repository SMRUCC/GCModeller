Public Class Compound

    Public Property entry As String
    Public Property name As String
    Public Property formula As String
    Public Property reactions As String()
    Public Property enzyme As String()

    Public Overrides Function ToString() As String
        Return name
    End Function

End Class