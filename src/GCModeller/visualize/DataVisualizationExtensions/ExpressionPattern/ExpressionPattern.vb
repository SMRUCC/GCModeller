#Region "Microsoft.VisualBasic::9aa4443648f21893556f8e2a317792fc, visualize\DataVisualizationExtensions\ExpressionPattern\ExpressionPattern.vb"

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

'     Class ExpressionPattern
' 
'         Properties: [dim], centers, Patterns, sampleNames
' 
'         Function: CMeansCluster, GetPartitionMatrix, populatePartitions
' 
' 
' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.DataMining.FuzzyCMeans
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Namespace ExpressionPattern

    ''' <summary>
    ''' 表达模式聚类
    ''' </summary>
    Public Class ExpressionPattern

        Public Property Patterns As FuzzyCMeansEntity()
        Public Property sampleNames As String()
        Public Property [dim] As Integer()
        Public Property centers As Classify()

        Public Function ToSummaryText() As String
            Dim sb As New StringBuilder

            Call sb.AppendLine($"fuzzy cmeans partitions: [{[dim](0)}, {[dim](1)}]")
            Call sb.AppendLine("base on samples(or groups):")
            Call sb.AppendLine(sampleNames.JoinBy(", "))
            Call sb.AppendLine($"clusters (should be #0 ~ #{[dim](0) * [dim](1) - 1}):")

            For Each cluster In Patterns.GroupBy(Function(a) a.cluster).OrderBy(Function(a) a.Key)
                Call sb.AppendLine($" # {cluster.Key}: {cluster.Count}")
            Next

            Return sb.ToString
        End Function

        Public Function GetPartitionMatrix() As IEnumerable(Of Matrix())
            Return populatePartitions(Patterns, [dim], sampleNames)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="matrix"></param>
        ''' <param name="dim">``[row, columns]``</param>
        ''' <returns></returns>
        ''' 
        Public Shared Function CMeansCluster(matrix As Matrix, [dim] As Integer()) As ExpressionPattern
            Dim nsize As Integer = [dim](Scan0) * [dim](1)
            Dim sampleNames As String() = matrix.sampleID
            Dim geneNodes As ClusterEntity() = matrix.expression _
                .AsParallel _
                .Select(Function(gene)
                            Dim vector As New List(Of Double)

                            For i As Integer = 0 To sampleNames.Length - 1
                                Call vector.Add(gene.experiments(i))
                            Next

                            Return New ClusterEntity With {
                                .uid = gene.geneID,
                                .entityVector = vector.ToArray
                            }
                        End Function) _
                .ToArray
            Dim centers As Classify() = geneNodes.CMeans(classCount:=nsize)

            Return New ExpressionPattern With {
                .Patterns = centers _
                    .Select(Function(c) c.members) _
                    .IteratesALL _
                    .ToArray,
                .sampleNames = sampleNames,
                .[dim] = [dim],
                .centers = centers
            }
        End Function

        Private Shared Iterator Function populatePartitions(clusters As IEnumerable(Of FuzzyCMeansEntity), dim%(), sampleNames As String()) As IEnumerable(Of Matrix())
            Dim row As New List(Of Matrix)
            Dim clusterGroups = clusters.GroupBy(Function(c) c.cluster).ToArray

            For Each cluster As IGrouping(Of Integer, FuzzyCMeansEntity) In clusterGroups
                Dim matrix = New Matrix With {
                    .sampleID = sampleNames,
                    .expression = cluster _
                        .Select(Function(a)
                                    Return New DataFrameRow With {
                                        .geneID = a.uid,
                                        .experiments = a.entityVector
                                    }
                                End Function) _
                        .ToArray,
                    .tag = cluster.Key
                }

                row += matrix

                If row = [dim](1) Then
                    Yield row.PopAll
                End If
            Next

            If row > 0 Then
                Yield row.PopAll
            End If
        End Function
    End Class
End Namespace
