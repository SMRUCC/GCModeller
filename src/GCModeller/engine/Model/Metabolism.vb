Public Structure Reaction

    Public ID As String

    Public substrates As String()
    Public products As String()
    Public enzyme As String()

    Public Overrides Function ToString() As String
        Return ID
    End Function

End Structure