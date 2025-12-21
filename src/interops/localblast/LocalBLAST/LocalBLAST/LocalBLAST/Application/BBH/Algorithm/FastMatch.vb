#Region "Microsoft.VisualBasic::ba67984fc2b635621e59dea9e7220e08, localblast\LocalBLAST\LocalBLAST\LocalBLAST\Application\BBH\Algorithm\FastMatch.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 94
    '    Code Lines: 79 (84.04%)
    ' Comment Lines: 3 (3.19%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 12 (12.77%)
    '     File Size: 4.07 KB


    '     Module FastMatch
    ' 
    '         Function: BinaryMatch
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports stdNum = System.Math

Namespace LocalBLAST.Application.BBH

    Public Module FastMatch

        <Extension>
        Public Iterator Function BinaryMatch(forward As IEnumerable(Of BestHit), reverse As IEnumerable(Of BestHit),
                                             Optional score As Double = 60,
                                             Optional BHR As Double = 0.8) As IEnumerable(Of BiDirectionalBesthit)

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
                        .level = Levels.BHR,
                        .positive = stdNum.Max(query.positive, rev.positive),
                        .reverse = rev.SBHScore,
                        .term = query.HitName
                    }
                End If
            Next
        End Function
    End Module
End Namespace
