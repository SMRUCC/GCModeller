Namespace Pipeline

    Public Class GenomeVector

        Public Property assembly_id As String
        Public Property taxonomy As String
        Public Property terms As Dictionary(Of String, Integer)

        Public Shared Iterator Function CreateVectors(terms As IEnumerable(Of RankTerm)) As IEnumerable(Of GenomeVector)
            For Each asm As IGrouping(Of String, RankTerm) In terms.GroupBy(Function(t) t.queryName.GetTagValue(".").Name)
                Dim id As String = asm.Key
                Dim taxon As String = asm.First.queryName.GetTagValue(" ").Value
                Dim counts = asm _
                    .GroupBy(Function(t) t.term) _
                    .ToDictionary(Function(a) a.Key,
                                  Function(a)
                                      Return a.Count
                                  End Function)

                Yield New GenomeVector With {
                    .assembly_id = id,
                    .taxonomy = taxon,
                    .terms = counts
                }
            Next
        End Function

    End Class
End Namespace