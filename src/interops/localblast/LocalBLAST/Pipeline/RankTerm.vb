Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Linq
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

        Public Shared Iterator Function RankTopTerm(hits As IEnumerable(Of BestHit), Optional termMaps As Dictionary(Of String, String) = Nothing) As IEnumerable(Of RankTerm)
            For Each group As IGrouping(Of String, BestHit) In hits.SafeQuery.GroupBy(Function(a) a.QueryName)
                With MeasureTopTerm(group, termMaps)
                    If .Any Then
                        Yield .First
                    End If
                End With
            Next
        End Function

        Private Shared Function MeasureTopTerm(group As IGrouping(Of String, BestHit), termMaps As Dictionary(Of String, String)) As RankTerm()
            Dim scores As NamedValue(Of Double)() = group _
                .Select(Function(h)
                            Dim key As String = h.HitName.Split.First.Split("|"c).First
                            Dim score As Double = (h.score + 1) * (h.identities + 1)

                            Return New NamedValue(Of Double)(key, score)
                        End Function) _
                .ToArray
            Dim terms As RankTerm() = scores _
                .GroupBy(Function(a)
                             If termMaps Is Nothing Then
                                 Return a.Name
                             Else
                                 Return termMaps(a.Name)
                             End If
                         End Function) _
                .Select(Function(a)
                            Dim scoreSet = a.UniqueNames.ToDictionary(Function(x) x.Name, Function(x) x.Value)
                            Dim term As String = a.Key

                            Return (term, scoreSet)
                        End Function) _
                .Select(Function(a)
                            Return New RankTerm With {
                                .queryName = group.Key,
                                .scores = a.scoreSet,
                                .term = a.term
                            }
                        End Function) _
                .OrderByDescending(Function(t) t.score) _
                .ToArray

            Return terms
        End Function
    End Class
End Namespace