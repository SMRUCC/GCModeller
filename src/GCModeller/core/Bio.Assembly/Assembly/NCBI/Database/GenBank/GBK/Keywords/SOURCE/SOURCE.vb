Namespace Assembly.NCBI.GenBank.GBFF.Keywords

    Public Class SOURCE : Inherits KeyWord

        Public Property SpeciesName As String
        Public Property OrganismHierarchy As ORGANISM

        Public Overrides Function ToString() As String
            Return OrganismHierarchy.ToString
        End Function

        Public Shared Widening Operator CType(str As String()) As SOURCE
            Dim Source As SOURCE = New SOURCE
            If Not str.IsNullOrEmpty Then
                Call __trimHeadKey(str)
                Source.SpeciesName = str.First
                Source.OrganismHierarchy = ORGANISM.InternalParser(str.Skip(1).ToArray)
            End If
            Return Source
        End Operator
    End Class
End Namespace