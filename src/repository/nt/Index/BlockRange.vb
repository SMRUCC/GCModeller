Public Structure BlockRange

    Dim start&, len%

    Public Overrides Function ToString() As String
        Return $"{start} --> {start& + len}"
    End Function
End Structure