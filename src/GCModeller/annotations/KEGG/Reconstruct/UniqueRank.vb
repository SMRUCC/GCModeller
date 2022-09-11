#Region "Microsoft.VisualBasic::07435ca85cf5b1565d781715bcfc65e4, GCModeller\annotations\KEGG\Reconstruct\UniqueRank.vb"

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

    '   Total Lines: 105
    '    Code Lines: 78
    ' Comment Lines: 15
    '   Blank Lines: 12
    '     File Size: 4.50 KB


    ' Module UniqueRank
    ' 
    '     Function: EvaluateCompoundUniqueRank, EvaluateEnzymeUniqueRank, EvaluateUniqueRank, UniquePathwayCompounds
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Quantile
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

''' <summary>
''' compound filter algorithm of the pathway maps
''' </summary>
Public Module UniqueRank

    <Extension>
    Public Iterator Function UniquePathwayCompounds(maps As IEnumerable(Of Pathway), Optional cutoff As Double = 0.3, Optional quantile# = 0.8) As IEnumerable(Of Pathway)
        With maps.ToArray
            Dim uniqueRanks = .EvaluateCompoundUniqueRank(cutoff, quantile).ToDictionary

            For Each map As Pathway In .AsEnumerable
                map.compound = map.compound _
                    .SafeQuery _
                    .Where(Function(a)
                               Return uniqueRanks(map.EntryId)(a.name) > 0
                           End Function) _
                    .ToArray

                Yield map
            Next
        End With
    End Function

    ''' <summary>
    ''' the more pathway of one compound occurs in, the less unique rank of the compound it have
    ''' </summary>
    ''' <param name="pathwayProfile"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function EvaluateCompoundUniqueRank(pathwayProfile As IEnumerable(Of Pathway), Optional cutoff As Double = 0.3, Optional quantile# = 0.8) As IEnumerable(Of DataSet)
        Return pathwayProfile.EvaluateUniqueRank(Function(map) map.compound.Keys.ToArray, cutoff, quantile)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function EvaluateEnzymeUniqueRank(pathwayProfile As IEnumerable(Of Pathway), Optional cutoff As Double = 0.3, Optional quantile# = 0.8) As IEnumerable(Of DataSet)
        Return pathwayProfile.EvaluateUniqueRank(Function(map) map.genes.Keys.ToArray, cutoff, quantile)
    End Function

    ''' <summary>
    ''' the more pathway of one compound occurs in, the less unique rank of the compound it have
    ''' </summary>
    ''' <param name="pathwayProfile"></param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Iterator Function EvaluateUniqueRank(pathwayProfile As IEnumerable(Of Pathway),
                                                getObjId As Func(Of Pathway, String()),
                                                Optional cutoff As Double = 0.3,
                                                Optional quantile# = 0.8) As IEnumerable(Of DataSet)

        Dim maps As Pathway() = pathwayProfile.ToArray
        Dim allCompounds As String() = maps _
            .Select(getObjId) _
            .IteratesALL _
            .Distinct _
            .ToArray
        Dim mapIndex = maps.ToDictionary(Function(a) a.EntryId, Function(a) getObjId(a).Indexing)
        Dim occurs As Dictionary(Of String, String()) = allCompounds _
            .ToDictionary(Function(a) a,
                          Function(a)
                              Return maps _
                                  .Where(Function(p)
                                             Return mapIndex(p.EntryId).IndexOf(a) > -1
                                         End Function) _
                                  .Select(Function(p) p.EntryId) _
                                  .ToArray
                          End Function)

        For Each pathway As Pathway In maps
            Dim unique As New Dictionary(Of String, Double)
            Dim total = (From cpd As String
                         In getObjId(pathway)
                         Let nmaps = occurs(cpd).Length
                         Select CDbl(nmaps)).GKQuantile
            Dim uniqueScore As Double
            Dim max As Double = total.Query(quantile)

            For Each cpd As String In getObjId(pathway)
                uniqueScore = 1 - occurs(cpd).Length / max

                If uniqueScore >= cutoff Then
                    unique(cpd) = uniqueScore
                Else
                    unique(cpd) = 0
                End If
            Next

            Yield New DataSet With {
                .ID = pathway.EntryId,
                .Properties = unique
            }
        Next
    End Function
End Module
