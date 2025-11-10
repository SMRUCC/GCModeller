Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

Namespace Pipeline

    Public Class RankTerm

        Public Property queryName As String
        Public Property term As String
        Public Property scores As Dictionary(Of String, Double)

        Public ReadOnly Property score As Double
            Get
                Return scores.SafeQuery.Values.Sum
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{queryName} = {score}"
        End Function

        Public Shared Iterator Function RankTopTerm(hits As IEnumerable(Of BestHit),
                                                    Optional termMaps As Dictionary(Of String, String) = Nothing,
                                                    Optional topBest As Boolean = True) As IEnumerable(Of RankTerm)

            For Each group As IGrouping(Of String, BestHit) In hits.SafeQuery.GroupBy(Function(a) a.QueryName)
                With MeasureTopTerm(group, AddressOf SingleBestHitScore, termMaps)
                    If topBest AndAlso .Any Then
                        Yield .First
                    Else
                        For Each term As RankTerm In .AsEnumerable
                            Yield term
                        Next
                    End If
                End With
            Next
        End Function

        Public Shared Iterator Function RankTopTerm(hits As IEnumerable(Of IQueryHits),
                                                    Optional termMaps As Dictionary(Of String, String) = Nothing,
                                                    Optional topBest As Boolean = True) As IEnumerable(Of RankTerm)

            For Each group As IGrouping(Of String, IQueryHits) In hits.SafeQuery.GroupBy(Function(a) a.queryName)
                With MeasureTopTerm(group, AddressOf GenericHitIdentities, termMaps)
                    If topBest AndAlso .Any Then
                        Yield .First
                    Else
                        For Each term As RankTerm In .AsEnumerable
                            Yield term
                        Next
                    End If
                End With
            Next
        End Function

        Private Shared Function SingleBestHitScore(h As BestHit) As Double
            Return (h.score + 1) * (h.identities + 1)
        End Function

        Private Shared Function GenericHitIdentities(h As IQueryHits) As Double
            Return h.identities
        End Function

        Private Shared Function ScoreQuery(Of T As IQueryHits)(group As IGrouping(Of String, T), eval As Func(Of T, Double)) As IEnumerable(Of NamedValue(Of Double))
            Return From h As T
                   In group
                   Where h.identities > 0 AndAlso h.hitName <> "HITS_NOT_FOUND"
                   Let key As String = h.hitName.Split.First.Split("|"c).First
                   Let score As Double = eval(h)
                   Select New NamedValue(Of Double)(key, score)
        End Function

        Private Shared Iterator Function MakeTerms(scores As IEnumerable(Of NamedValue(Of Double)), queryId As String, termMaps As Dictionary(Of String, String)) As IEnumerable(Of RankTerm)
            For Each term As IGrouping(Of String, NamedValue(Of Double)) In scores.GroupBy(Function(a) If(termMaps Is Nothing, a.Name, termMaps.TryGetValue(a.Name, [default]:="Unknown")))
                Dim scoreSet As Dictionary(Of String, Double) = term.UniqueNames.ToDictionary(Function(x) x.Name, Function(x) x.Value)
                Dim termName As String = term.Key

                Yield New RankTerm With {
                    .queryName = queryId,
                    .scores = scoreSet,
                    .term = termName
                }
            Next
        End Function

        Private Shared Function MeasureTopTerm(Of T As IQueryHits)(group As IGrouping(Of String, T), eval As Func(Of T, Double), termMaps As Dictionary(Of String, String)) As RankTerm()
            Dim scores As NamedValue(Of Double)() = ScoreQuery(group, eval).ToArray
            Dim terms As RankTerm() = MakeTerms(scores, group.Key, termMaps) _
                .OrderByDescending(Function(ti) ti.score) _
                .ToArray

            Return terms
        End Function
    End Class
End Namespace