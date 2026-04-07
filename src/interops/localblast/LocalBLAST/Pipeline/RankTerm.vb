#Region "Microsoft.VisualBasic::cb07af7866e73c82b4e1cd35e325aafa, localblast\LocalBLAST\Pipeline\RankTerm.vb"

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

    '   Total Lines: 171
    '    Code Lines: 132 (77.19%)
    ' Comment Lines: 14 (8.19%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 25 (14.62%)
    '     File Size: 7.69 KB


    '     Class RankTerm
    ' 
    '         Properties: queryName, score, scores, source, supports
    '                     term, topHit
    ' 
    '         Function: GenericHitIdentities, MakeTerms, MeasureTopTerm, (+3 Overloads) RankTopTerm, ScoreQuery
    '                   SingleBestHitScore, ToString, WrapID
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Tasks.Models

Namespace Pipeline

    Public Class RankTerm : Implements INamedValue, IBlastHit

        Public Property queryName As String Implements INamedValue.Key, IBlastHit.queryName
        Public Property term As String Implements IBlastHit.hitName, IBlastHit.description

        Public Property source As String()
        Public Property scores As Double()

        ''' <summary>
        ''' SUM of the all scores for this term
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property score As Double
            Get
                Return scores.SafeQuery.Sum
            End Get
        End Property

        Public ReadOnly Property supports As Integer
            Get
                Return source.TryCount
            End Get
        End Property

        Public ReadOnly Property topHit As String
            Get
                If scores.IsNullOrEmpty OrElse source.IsNullOrEmpty Then
                    Return Nothing
                End If

                Return source(which.Max(scores))
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"[{term}] {queryName} = {score}"
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function WrapID(id As String) As RankTerm
            Return New RankTerm With {
                .queryName = id,
                .term = id,
                .scores = {100},
                .source = {id}
            }
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

        Public Shared Iterator Function RankTopTerm(hits As HitCollection) As IEnumerable(Of RankTerm)
            For Each group As IGrouping(Of String, Hit) In hits.AsEnumerable.GroupBy(Function(a) a.tag)
                Dim scoreSet = group.Select(Function(a) New NamedValue(Of Double)(a.hitName, (a.score + 1) * (a.identities + 1))).ToArray
                Dim termName As String = group.Key
                Dim sourceNames As String() = scoreSet.Select(Function(a) a.Name).ToArray
                Dim vec As Double() = scoreSet.Select(Function(a) a.Value).ToArray

                Yield New RankTerm With {
                    .queryName = hits.QueryName,
                    .scores = vec,
                    .term = termName,
                    .source = sourceNames
                }
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

        Public Const Unknown As String = NameOf(Unknown)

        Private Shared Iterator Function MakeTerms(scores As IEnumerable(Of NamedValue(Of Double)), queryId As String, termMaps As Dictionary(Of String, String)) As IEnumerable(Of RankTerm)
            Dim missingMap As Boolean = termMaps.IsNullOrEmpty
            Dim buildTerms = From a As NamedValue(Of Double)
                             In scores
                             Let term As String = If(missingMap, a.Name, termMaps.TryGetValue(a.Name, [default]:=Unknown))
                             Group By term Into Group
                             Select New NamedCollection(Of NamedValue(Of Double))(term, Group.Select(Function(i) i.a))

            For Each term As IGrouping(Of String, NamedValue(Of Double)) In buildTerms
                Dim scoreSet As NamedValue(Of Double)() = term.ToArray
                Dim termName As String = term.Key
                Dim sourceNames As String() = scoreSet.Select(Function(a) a.Name).ToArray
                Dim vec As Double() = scoreSet.Select(Function(a) a.Value).ToArray

                Yield New RankTerm With {
                    .queryName = queryId,
                    .scores = vec,
                    .term = termName,
                    .source = sourceNames
                }
            Next
        End Function

        ''' <summary>
        ''' make terms assignment result 
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="group"></param>
        ''' <param name="eval"></param>
        ''' <param name="termMaps"></param>
        ''' <returns>
        ''' terms has been ordered by its score in desc order
        ''' </returns>
        Public Shared Function MeasureTopTerm(Of T As IQueryHits)(group As IGrouping(Of String, T), eval As Func(Of T, Double), termMaps As Dictionary(Of String, String)) As RankTerm()
            Dim scores As NamedValue(Of Double)() = ScoreQuery(group, eval).ToArray
            Dim terms As RankTerm() = MakeTerms(scores, group.Key, termMaps) _
                .OrderByDescending(Function(ti) ti.score) _
                .ToArray

            Return terms
        End Function
    End Class
End Namespace
