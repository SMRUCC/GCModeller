#Region "Microsoft.VisualBasic::61038d24f4e403d58a33d581c09e360c, localblast\LocalBLAST\Diamond\TermStreamAssignment.vb"

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

    '   Total Lines: 86
    '    Code Lines: 64 (74.42%)
    ' Comment Lines: 10 (11.63%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 12 (13.95%)
    '     File Size: 3.20 KB


    ' Module TermStreamAssignment
    ' 
    '     Function: HitCollection, HitsCollection, (+2 Overloads) MakeTerms
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Tasks.Models

Public Module TermStreamAssignment

    ReadOnly rank As Func(Of DiamondAnnotation, Double) = Function(hit) (hit.Pident + 1) * (hit.BitScore + 1)

    ''' <summary>
    ''' Processing term assignment via stream processing of the large diamond annotation result output
    ''' </summary>
    ''' <param name="m8"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function MakeTerms(m8 As IEnumerable(Of DiamondAnnotation), Optional topBest As Boolean = True) As IEnumerable(Of RankTerm)
        Dim group As New List(Of DiamondAnnotation)
        Dim query As String = ""

        For Each hit As DiamondAnnotation In m8.SafeQuery
            If hit.QseqId <> query Then
                If group.Count > 0 Then
                    For Each term As RankTerm In MakeTerms(query, group, topBest)
                        Yield term
                    Next

                    Call group.Clear()
                End If

                query = hit.QseqId
            End If

            Call group.Add(hit)
        Next

        If group.Count > 0 Then
            For Each term As RankTerm In MakeTerms(query, group, topBest)
                Yield term
            Next
        End If
    End Function

    Private Iterator Function MakeTerms(query As String, group As IEnumerable(Of DiamondAnnotation), topBest As Boolean) As IEnumerable(Of RankTerm)
        Dim data As New NamedCollection(Of DiamondAnnotation)(query, group)
        Dim terms = RankTerm.MeasureTopTerm(data, rank, Nothing)

        If topBest AndAlso terms.Any Then
            Yield terms.First
        Else
            For Each term As RankTerm In terms
                Yield term
            Next
        End If
    End Function

    ''' <summary>
    ''' Make query group and convert as query hit collection
    ''' </summary>
    ''' <param name="diamond"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function HitCollection(diamond As IEnumerable(Of DiamondAnnotation)) As IEnumerable(Of HitCollection)
        For Each query As IGrouping(Of String, DiamondAnnotation) In diamond.GroupBy(Function(a) a.QseqId)
            Yield New HitCollection With {
                .QueryName = query.Key,
                .hits = query.HitsCollection.ToArray
            }
        Next
    End Function

    <Extension>
    Private Iterator Function HitsCollection(query As IGrouping(Of String, DiamondAnnotation)) As IEnumerable(Of Hit)
        For Each hit As DiamondAnnotation In query
            Yield New Hit With {
                .evalue = hit.EValue,
                .gaps = hit.GapOpen,
                .hitName = hit.SseqId,
                .identities = hit.Pident,
                .positive = hit.Pident,
                .score = hit.BitScore,
                .tag = hit.SseqId
            }
        Next
    End Function
End Module

