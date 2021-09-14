Imports System.Runtime.CompilerServices
Imports stdNum = System.Math

Namespace LocalBLAST.Application.BBH

    Public Module FastMatch

        <Extension>
        Public Iterator Function BinaryMatch(forward As IEnumerable(Of BestHit), reverse As IEnumerable(Of BestHit),
                                             Optional score As Double = 60,
                                             Optional BHR As Double = 0.95) As IEnumerable(Of BiDirectionalBesthit)

            Dim rfilter = reverse _
                .Where(Function(r)
                           Return r.score >= score AndAlso r.SBHScore >= BHR AndAlso Not r.SBHScore.IsNaNImaginary
                       End Function) _
                .GroupBy(Function(r) r.QueryName) _
                .ToDictionary(Function(r) r.Key,
                              Function(r)
                                  Dim desc = r.OrderByDescending(Function(d) d.SBHScore).ToArray
                                  Dim orders = desc.Select(Function(d) d.HitName).Indexing

                                  Return (desc, orders)
                              End Function)

            For Each query As BestHit In forward _
                .Where(Function(q)
                           Return q.score >= score AndAlso
                               q.SBHScore >= BHR AndAlso
                               Not q.SBHScore.IsNaNImaginary
                       End Function) _
                .GroupBy(Function(q) q.QueryName) _
                .Select(Function(q)
                            Return q.OrderByDescending(Function(d) d.SBHScore).First
                        End Function)

                If Not rfilter.ContainsKey(query.HitName) Then
                    Yield New BiDirectionalBesthit With {
                        .QueryName = query.QueryName,
                        .HitName = HITS_NOT_FOUND,
                        .length = query.query_length,
                        .description = query.description,
                        .level = Levels.NA
                    }

                    Continue For
                End If

                Dim hits = rfilter(query.HitName)
                Dim isTop = hits.orders(query.QueryName)

                If isTop = -1 Then
                    ' sbh
                    Yield New BiDirectionalBesthit With {
                        .QueryName = query.QueryName,
                        .HitName = query.HitName,
                        .description = query.description,
                        .forward = query.SBHScore,
                        .length = query.query_length,
                        .level = Levels.SBH,
                        .positive = query.positive,
                        .reverse = 0,
                        .term = query.HitName
                    }
                ElseIf isTop = 0 Then
                    ' bbh
                    Dim rev = hits.desc(0)

                    Yield New BiDirectionalBesthit With {
                        .QueryName = query.QueryName,
                        .description = query.description,
                        .forward = query.SBHScore,
                        .HitName = query.HitName,
                        .length = query.query_length,
                        .level = Levels.BBH,
                        .positive = stdNum.Max(query.positive, rev.positive),
                        .reverse = rev.SBHScore,
                        .term = query.HitName
                    }
                Else
                    ' bhr
                    Dim rev = hits.desc(isTop)

                    Yield New BiDirectionalBesthit With {
                        .QueryName = query.QueryName,
                        .description = query.description,
                        .forward = query.SBHScore,
                        .HitName = query.HitName,
                        .length = query.query_length,
                        .level = Levels.PartialBBH,
                        .positive = stdNum.Max(query.positive, rev.positive),
                        .reverse = rev.SBHScore,
                        .term = query.HitName
                    }
                End If
            Next
        End Function
    End Module
End Namespace