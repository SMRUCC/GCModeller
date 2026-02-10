Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace Pipeline

    Public Class GenomeVector : Implements INamedValue

        Public Property assembly_id As String Implements INamedValue.Key
        Public Property taxonomy As String
        Public Property terms As Dictionary(Of String, Integer)

        Public ReadOnly Property size As Integer
            Get
                If terms Is Nothing Then
                    Return 0
                Else
                    Return terms.Values.Sum
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return taxonomy
        End Function

        Private Shared Function groupByAssembly(terms As IEnumerable(Of RankTerm)) As IEnumerable(Of NamedCollection(Of RankTerm))
            Return From term As RankTerm
                   In terms
                   Let tag As NamedValue(Of String) = term.queryName.GetTagValue(".")
                   Group By tag.Name Into Group
                   Select New NamedCollection(Of RankTerm)(Name, Group.Select(Function(a) a.term))
        End Function

        Public Shared Iterator Function GroupByTaxonomy(vectors As IEnumerable(Of GenomeVector)) As IEnumerable(Of GenomeVector)
            For Each taxonomy As IGrouping(Of String, GenomeVector) In vectors.GroupBy(Function(vec) vec.taxonomy)
                Dim union As New Dictionary(Of String, Integer)

                For Each part As GenomeVector In taxonomy
                    For Each term As KeyValuePair(Of String, Integer) In part.terms
                        If union.ContainsKey(term.Key) Then
                            union(term.Key) += term.Value
                        Else
                            union.Add(term.Key, term.Value)
                        End If
                    Next
                Next

                Yield New GenomeVector With {
                    .taxonomy = taxonomy.Key,
                    .assembly_id = taxonomy.Keys.JoinBy(","),
                    .terms = union
                }
            Next
        End Function

        Public Shared Iterator Function CreateVectors(terms As IEnumerable(Of RankTerm)) As IEnumerable(Of GenomeVector)
            For Each asm As NamedCollection(Of RankTerm) In groupByAssembly(terms)
                Dim id As String = asm.name
                Dim taxon As String = asm.First.queryName.GetTagValue(" ").Value
                Dim counts As Dictionary(Of String, Integer) = asm _
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