#Region "Microsoft.VisualBasic::cfd2e8bf43a5dc4786d7dbf94a2862b4, localblast\LocalBLAST\Pipeline\KO\KOAssignment.vb"

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

    '     Module KOAssignment
    ' 
    '         Function: KOassignmentBBH, KOAssignmentSBH, pickDrfBesthit, populateHitGroup
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports r = System.Text.RegularExpressions.Regex

Namespace Pipeline

    Public Module KOAssignment

        ''' <summary>
        ''' 在这里主要是将hit_name之中的KO编号提取出来就好了
        ''' </summary>
        ''' <param name="result"></param>
        ''' <param name="removesNohit">Removes all of the alignment that produce hit not found...</param>
        ''' <returns></returns>
        <Extension>
        Public Function KOAssignmentSBH(result As IEnumerable(Of BestHit), Optional removesNohit As Boolean = True) As BestHit()
            Return result _
                .Select(Function(align)
                            align.HitName = r.Match(align.HitName, "K\d+", RegexICSng).Value Or align.HitName.Split.First.AsDefault
                            Return align
                        End Function) _
                .Where(Function(align)
                           If removesNohit Then
                               If align.HitName.IsPattern("K\d+") Then
                                   Return True
                               Else
                                   Return False
                               End If
                           Else
                               Return True
                           End If
                       End Function) _
                .ToArray
        End Function

        ''' <summary>
        ''' KO number assignment in bbh method
        ''' </summary>
        ''' <param name="drf">Besthits in direction forward</param>
        ''' <param name="drr">Besthits in direction reverse</param>
        ''' <returns></returns>
        Public Iterator Function KOassignmentBBH(drf As IEnumerable(Of BestHit), drr As IEnumerable(Of BestHit)) As IEnumerable(Of BiDirectionalBesthit)
            Dim drfBesthits = drf.pickDrfBesthit.ToDictionary(Function(q) q.Name)
            Dim bbh As New List(Of (forward As BestHit, reverse As BestHit))
            Dim KO1, KO2 As String

            For Each query In drr.populateHitGroup
                ' [queryName => [taxonomy => alignment]]
                Dim queryName = query.Name.Split.First
                Dim forwards = drfBesthits.TryGetValue(queryName)
                Dim forwardTaxonomy = forwards.Value

                ' current query have no bbh result
                If forwardTaxonomy.IsNullOrEmpty Then
                    Continue For
                End If

                bbh *= 0

                ' get bbh result in each taxonomy result
                For Each taxonomy In query.Value
                    If forwardTaxonomy.ContainsKey(taxonomy.Key) Then
                        KO1 = forwardTaxonomy(taxonomy.Key).HitName.Split("|"c).First
                        KO2 = taxonomy.Value.QueryName.Split("|"c).First

                        If KO1 = KO2 Then
                            ' is a bbh result
                            bbh += (forwardTaxonomy(taxonomy.Key), taxonomy.Value)
                        End If
                    End If
                Next

                If bbh = 0 Then
                    ' current query have no bbh result
                    Continue For
                End If

                ' get top KO supports result
                Dim topSupportsKO = bbh _
                    .GroupBy(Function(b)
                                 Return b.forward.HitName.Split("|"c).First
                             End Function) _
                    .OrderByDescending(Function(a) a.Count) _
                    .First
                ' and then get the top score hits
                Dim topScoreAlignment = topSupportsKO _
                    .OrderByDescending(Function(b)
                                           Return Math.Min(b.forward.score, b.reverse.score)
                                       End Function) _
                    .First

                ' finally
                ' we can returns the new bbh result of current query
                Yield New BiDirectionalBesthit With {
                    .QueryName = query.Name.Split().First,
                    .HitName = topScoreAlignment.forward.HitName.Split("|"c).First,
                    .description = topScoreAlignment.forward.HitName,
                    .term = .HitName,
                    .forward = topScoreAlignment.forward.identities,
                    .reverse = topScoreAlignment.reverse.identities,
                    .length = topScoreAlignment.forward.query_length,
                    .positive = (topScoreAlignment.forward.positive + topScoreAlignment.reverse.positive) / 2
                }
            Next
        End Function

        ''' <summary>
        ''' Populate hit group of each query
        ''' </summary>
        ''' <param name="drr"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Private Function populateHitGroup(drr As IEnumerable(Of BestHit)) As IEnumerable(Of NamedValue(Of Dictionary(Of String, BestHit)))
            ' hit name is the query genome member
            Return drr.pickDrfBesthit(Function(a) a.QueryName, Function(a) a.HitName)
        End Function

        ''' <summary>
        ''' 因为在导出结果的时候是没有取tophit的，所以会在物种间或者物种内存在多个重复的结果
        ''' 在这里对每一个query进行物种分组，取物种内最佳
        ''' </summary>
        ''' <param name="drf"></param>
        ''' <returns></returns>
        <Extension>
        Private Iterator Function pickDrfBesthit(drf As IEnumerable(Of BestHit),
                                                 Optional getKey As Func(Of BestHit, String) = Nothing,
                                                 Optional getQuery As Func(Of BestHit, String) = Nothing) As IEnumerable(Of NamedValue(Of Dictionary(Of String, BestHit)))

            Dim queries As IGrouping(Of String, BestHit)()

            If getQuery Is Nothing Then
                getQuery = Function(a) a.QueryName
            End If
            If getKey Is Nothing Then
                getKey = Function(a) a.HitName
            End If

            queries = drf.GroupBy(Function(a) getQuery(a)).ToArray

            For Each query As IGrouping(Of String, BestHit) In queries
                Dim groupByTaxonomy = query.GroupBy(Function(a) getKey(a).Split("|"c).Last).ToArray
                Dim taxonomyTopHit = groupByTaxonomy _
                    .Select(Function(rtax)
                                Return rtax.OrderByDescending(Function(a) a.Score).First
                            End Function) _
                    .ToDictionary(Function(best)
                                      Return getKey(best).Split("|"c).Last
                                  End Function)

                ' [queryName => [taxonomy => alignment]]
                Yield New NamedValue(Of Dictionary(Of String, BestHit)) With {
                    .Name = query.Key,
                    .Value = taxonomyTopHit
                }
            Next
        End Function
    End Module
End Namespace
