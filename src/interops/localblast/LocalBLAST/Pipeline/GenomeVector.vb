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

        Private Shared Iterator Function streamGroupByAssembly(terms As IEnumerable(Of RankTerm)) As IEnumerable(Of NamedCollection(Of RankTerm))
            Dim asm_id As String = ""
            Dim group As New List(Of RankTerm)

            For Each term As RankTerm In terms
                Dim tag As NamedValue(Of String) = term.queryName.GetTagValue(".")

                If tag.Name <> asm_id Then
                    If group.Count > 0 Then
                        Yield New NamedCollection(Of RankTerm)(asm_id, group)
                        group.Clear()
                    End If

                    asm_id = tag.Name
                End If

                Call group.Add(term)
            Next

            If group.Count > 0 Then
                Yield New NamedCollection(Of RankTerm)(asm_id, group)
            End If
        End Function

        ''' <summary>
        ''' Make union of the taxonomy assembly contig result
        ''' </summary>
        ''' <param name="vectors"></param>
        ''' <param name="size_cutoff">test of the contigby gene size cutoff.</param>
        ''' <returns></returns>
        Public Shared Iterator Function GroupByTaxonomy(vectors As IEnumerable(Of GenomeVector), Optional size_cutoff As Integer = 1000) As IEnumerable(Of GenomeVector)
            Call $"Make union of the taxonomy assembly contig result via annotated gene size cutoff {size_cutoff}".debug

            For Each taxonomy As IGrouping(Of String, GenomeVector) In vectors.GroupBy(Function(vec) vec.taxonomy)
                Dim contigs As New List(Of GenomeVector)

                For Each asm As GenomeVector In taxonomy
                    If asm.size < size_cutoff Then
                        Call contigs.Add(asm)
                    Else
                        Yield asm
                    End If
                Next

                If Not contigs.Any Then
                    Continue For
                End If

                Dim union As New Dictionary(Of String, Integer)

                For Each part As GenomeVector In contigs
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
                    .assembly_id = contigs.Keys.JoinBy(","),
                    .terms = union
                }
            Next
        End Function

        Public Shared Iterator Function CreateVectors(terms As IEnumerable(Of RankTerm), Optional stream As Boolean = False) As IEnumerable(Of GenomeVector)
            For Each asm As NamedCollection(Of RankTerm) In If(stream, streamGroupByAssembly(terms), groupByAssembly(terms))
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