Public Structure Genotype

    Public CentralDogmas As CentralDogma()

    Public Overrides Function ToString() As String
        Return $"{CentralDogmas.Length} genes"
    End Function
End Structure

