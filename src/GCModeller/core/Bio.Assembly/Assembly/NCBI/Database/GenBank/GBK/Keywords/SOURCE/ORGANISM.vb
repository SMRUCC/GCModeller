Namespace Assembly.NCBI.GenBank.GBFF.Keywords

    Public Class ORGANISM

        Public Property Categorys As String()
        Public Property SpeciesName As String

        Public Overrides Function ToString() As String
            Return SpeciesName
        End Function

        Public Shared Function InternalParser(str As String()) As ORGANISM
            Call KeyWord.__trimHeadKey(str)
            Dim Org As ORGANISM = New ORGANISM With {.SpeciesName = str.First}
            Org.Categorys = Strings.Split(String.Join(" ", (From s As String In str.Skip(1) Select s.Trim).ToArray), "; ")
            Return Org
        End Function
    End Class
End Namespace