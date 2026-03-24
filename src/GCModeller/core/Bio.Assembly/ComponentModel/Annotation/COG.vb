#Region "Microsoft.VisualBasic::8bd551c982257c3ad8b3589078bc0697, core\Bio.Assembly\ComponentModel\Annotation\COG.vb"

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

'   Total Lines: 3
'    Code Lines: 2 (66.67%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 1 (33.33%)
'     File Size: 52 B


' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Namespace ComponentModel.Annotation

    Public Interface IOrthologyCluster

        Property FamilyID As String
        Property GeneCluster As String()

    End Interface

    Public Module ClusterExtensions

        <Extension>
        Private Iterator Function MakeFlat(Of T As IOrthologyCluster)(clusters As IEnumerable(Of T)) As IEnumerable(Of (id$, cluster As T))
            For Each c As T In clusters
                Yield (c.FamilyID, c)

                For Each id As String In c.GeneCluster.SafeQuery
                    Yield (id, c)
                Next
            Next
        End Function

        Private Function GetLargest(Of T As IOrthologyCluster)(group As IGrouping(Of String, (id$, cluster As T))) As T
            Dim largest As (id$, cluster As T) = group _
                .OrderByDescending(Function(i)
                                       Return i.cluster.GeneCluster.TryCount
                                   End Function) _
                .First

            Return largest.cluster
        End Function

        <Extension>
        Public Iterator Function GetClusters(Of T As {New, IOrthologyCluster})(index As Dictionary(Of String, String()), clusters As IEnumerable(Of T)) As IEnumerable(Of T)
            Dim geneIndex As Dictionary(Of String, T) = clusters _
                .MakeFlat _
                .Where(Function(cluster) Not cluster.id.StringEmpty) _
                .GroupBy(Function(cluster) cluster.id) _
                .ToDictionary(Function(group) group.Key,
                              Function(group)
                                  Return GetLargest(group)
                              End Function)

            For Each offset As KeyValuePair(Of String, String()) In index
                If offset.Value Is Nothing Then
                    Yield New T With {
                        .FamilyID = offset.Key,
                        .GeneCluster = offset.Value
                    }

                    Continue For
                End If

                Dim hits As T() = offset.Value _
                    .Where(Function(id) geneIndex.ContainsKey(id)) _
                    .Select(Function(id) geneIndex(id)) _
                    .ToArray

                If hits.Length = 0 Then
                    Yield New T With {
                        .FamilyID = offset.Key,
                        .GeneCluster = offset.Value
                    }
                Else
                    Dim top As IGrouping(Of String, T) = hits _
                        .GroupBy(Function(c) c.FamilyID) _
                        .OrderByDescending(Function(c) c.Count) _
                        .First

                    Yield top.First
                End If
            Next
        End Function

    End Module
End Namespace
